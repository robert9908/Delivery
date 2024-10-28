using Delivery.Models.Domain;
using Microsoft.Data.SqlClient;

namespace Delivery.Logging
{
    public class FilteredOrdersLogger
    {
        private readonly string _connectionString;

        public FilteredOrdersLogger(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void ClearTable()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("TRUNCATE TABLE dbo.FilteredOrders", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public void LogOrders(List<Order> orders)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                // Включить IDENTITY_INSERT для таблицы 
                using (var command = new SqlCommand("SET IDENTITY_INSERT dbo.FilteredOrders ON;", connection))
                {
                    command.ExecuteNonQuery();
                }
                foreach (var order in orders)
                {
                    // Используйте правильный запрос
                    using (var command = new SqlCommand(
                        "INSERT INTO dbo.FilteredOrders (Id, Name, Weight, District, DeliveryDateTime) VALUES (@id, @name, @weight, @district, @DeliveryDateTime)",
                        connection))
                    {
                        if (order.DeliveryDateTime < new DateTime(1753, 1, 1))
                        {
                            order.DeliveryDateTime = new DateTime(1753, 1, 1);
                        }
                        else if (order.DeliveryDateTime > new DateTime(9999, 12, 31))
                        {
                            order.DeliveryDateTime = new DateTime(9999, 12, 31);
                        }
                        command.Parameters.AddWithValue("@id", order.Id);
                        command.Parameters.AddWithValue("@name", order.Name);
                        command.Parameters.AddWithValue("@weight", order.Weight);
                        command.Parameters.AddWithValue("@district", order.District);
                        command.Parameters.AddWithValue("@deliverydatetime", order.DeliveryDateTime);
                        command.ExecuteNonQuery();
                    }
                }
                using (var command = new SqlCommand("SET IDENTITY_INSERT dbo.FilteredOrders OFF;", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
