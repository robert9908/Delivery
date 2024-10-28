using Microsoft.Data.SqlClient;

namespace Delivery.Logging
{
    public class MySqlServerLoggerProvider : ILoggerProvider
    {
        private readonly string _connectionString;

        public MySqlServerLoggerProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MySqlServerLogger(categoryName, _connectionString);
        }

        public void Dispose()
        {
            // Не требуется реализовывать Dispose()
        }
    }

    public class MySqlServerLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _connectionString;

        public MySqlServerLogger(string categoryName, string connectionString)
        {
            _categoryName = categoryName;
            _connectionString = connectionString;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Не требуется реализовывать BeginScope()
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information; // Запись в базу только для уровня Information и выше
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string message = formatter(state, exception);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("INSERT INTO dbo.LogEntries (Level, Message, Source, Exception) VALUES (@Level, @Message, @Source, @Exception)", connection))
                {
                    command.Parameters.AddWithValue("@Level", logLevel.ToString());
                    command.Parameters.AddWithValue("@Message", message);
                    command.Parameters.AddWithValue("@Source", _categoryName);
                    command.Parameters.AddWithValue("@Exception", exception?.ToString() ?? string.Empty);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
