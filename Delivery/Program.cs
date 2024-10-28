using Delivery.Data;
using Delivery.Logging;
using Delivery.Mapping;
using Delivery.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Настройка стандартного логгера
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Регистрация логгера для SQL Server
builder.Logging.AddProvider(new MySqlServerLoggerProvider(builder.Configuration.GetConnectionString("DeliveryConnectionString")));
builder.Services.AddScoped<FilteredOrdersLogger>(sp => new FilteredOrdersLogger(builder.Configuration.GetConnectionString("DeliveryConnectionString")));



// Регистрация сервисов
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Регистрация контекстов
builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeliveryConnectionString")));



// Регистрация репозиториев
builder.Services.AddScoped<IOrderRepository, SQLOrderRepository>();

// Регистрация AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();