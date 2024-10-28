using AutoMapper;
using Delivery.Controllers;
using Delivery.Data;
using Delivery.Logging;
using Delivery.Models.Domain;
using Delivery.Models.DTO;
using Delivery.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrdersController _ordersController;

        public OrdersControllerTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _mockMapper = new Mock<IMapper>();
            _ordersController = new OrdersController(
                new DeliveryDbContext(new DbContextOptionsBuilder<DeliveryDbContext>().UseInMemoryDatabase("TestDatabase").Options),
                _mockOrderRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                new FilteredOrdersLogger(string.Empty) // Вставьте реальную строку подключения, если необходимо
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOkObjectResult_WhenOrdersExist()
        {
            // Arrange
            var ordersDomain = new List<Order> { new Order { Id = 1, Name = "Order 1" } };
            var ordersDto = new List<OrderDto> { new OrderDto { Id = 1, Name = "Order 1" } };

            _mockOrderRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<DateTime?>(), It.IsAny<string>()))
                .ReturnsAsync(ordersDomain);

            _mockMapper
                .Setup(mapper => mapper.Map<List<OrderDto>>(ordersDomain))
                .Returns(ordersDto);

            // Act
            var result = await _ordersController.GetAll(null, null);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(ordersDto, okResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequest_WhenDistrictInvalid()
        {
            // Arrange
            _mockOrderRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<DateTime?>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _ordersController.GetAll(null, "Invalid District");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal("Район может содержать только буквы и пробелы.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFound_WhenNoOrdersFound()
        {
            // Arrange
            _mockOrderRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<DateTime?>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Order>());

            // Act
            var result = await _ordersController.GetAll(null, "ValidDistrict");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Empty((List<OrderDto>)okResult.Value);
        }

        [Fact]
        public async Task GetAll_LogsInformation_WhenOrdersFound()
        {
            // Arrange
            var ordersDomain = new List<Order> { new Order { Id = 1, Name = "Order 1" } };

            _mockOrderRepository
                .Setup(repo => repo.GetAllAsync(It.IsAny<DateTime?>(), It.IsAny<string>()))
                .ReturnsAsync(ordersDomain);

            // Act
            await _ordersController.GetAll(null, null);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("Запрос на получение всех заказов с фильтрами: fromDate={FromDate}, district={District}", null, null), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Найдено {Count} заказов", ordersDomain.Count), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Найдено {Count} заказов после фильтрации", ordersDomain.Count), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsOkObjectResult_WhenOrderExists()
        {
            // Arrange
            var orderDomain = new Order { Id = 1, Name = "Order 1" };
            var orderDto = new OrderDto { Id = 1, Name = "Order 1" };

            _mockOrderRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(orderDomain);

            _mockMapper
                .Setup(mapper => mapper.Map<OrderDto>(orderDomain))
                .Returns(orderDto);

            // Act
            var result = await _ordersController.GetById(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(orderDto, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _mockOrderRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _ordersController.GetById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_LogsInformation_WhenOrderFound()
        {
            // Arrange
            var orderDomain = new Order { Id = 1, Name = "Order 1" };

            _mockOrderRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(orderDomain);

            // Act
            await _ordersController.GetById(1);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("Запрос на получение заказа с ID: {Id}", 1), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Заказ с ID {Id} найден", 1), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenOrderCreated()
        {
            // Arrange
            var addOrderRequestDto = new AddOrderRequestDto { Name = "New Order" };
            var orderDomain = new Order { Id = 1, Name = "New Order" };
            var orderDto = new OrderDto { Id = 1, Name = "New Order" };

            _mockMapper
                .Setup(mapper => mapper.Map<Order>(addOrderRequestDto))
                .Returns(orderDomain);

            _mockOrderRepository
                .Setup(repo => repo.CreateAsync(orderDomain))
                .ReturnsAsync(orderDomain);

            _mockMapper
                .Setup(mapper => mapper.Map<OrderDto>(orderDomain))
                .Returns(orderDto);

            // Act
            var result = await _ordersController.Create(addOrderRequestDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdAtActionResult = (CreatedAtActionResult)result;
            Assert.Equal(nameof(_ordersController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(orderDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Create_LogsInformation_WhenOrderCreated()
        {
            // Arrange
            var addOrderRequestDto = new AddOrderRequestDto { Name = "New Order" };
            var orderDomain = new Order { Id = 1, Name = "New Order" };

            _mockMapper
                .Setup(mapper => mapper.Map<Order>(addOrderRequestDto))
                .Returns(orderDomain);

            _mockOrderRepository
                .Setup(repo => repo.CreateAsync(orderDomain))
                .ReturnsAsync(orderDomain);

            // Act
            await _ordersController.Create(addOrderRequestDto);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("Создание нового заказа: {OrderData}", addOrderRequestDto), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Создан новый заказ с ID: {Id}", 1), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsOkObjectResult_WhenOrderUpdated()
        {
            // Arrange
            var updateOrderRequestDto = new UpdateOrderRequestDto { Name = "Updated Order" };
            var orderDomain = new Order { Id = 1, Name = "Updated Order" };
            var orderDto = new OrderDto { Id = 1, Name = "Updated Order" };

            _mockMapper
                .Setup(mapper => mapper.Map<Order>(updateOrderRequestDto))
                .Returns(orderDomain);

            _mockOrderRepository
                .Setup(repo => repo.UpdateAsync(1, orderDomain))
                .ReturnsAsync(orderDomain);

            _mockMapper
                .Setup(mapper => mapper.Map<OrderDto>(orderDomain))
                .Returns(orderDto);

            // Act
            var result = await _ordersController.Update(1, updateOrderRequestDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(orderDto, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var updateOrderRequestDto = new UpdateOrderRequestDto { Name = "Updated Order" };

            _mockOrderRepository
                .Setup(repo => repo.UpdateAsync(1, It.IsAny<Order>()))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _ordersController.Update(1, updateOrderRequestDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_LogsInformation_WhenOrderUpdated()
        {
            // Arrange
            var updateOrderRequestDto = new UpdateOrderRequestDto { Name = "Updated Order" };
            var orderDomain = new Order { Id = 1, Name = "Updated Order" };

            _mockMapper
                .Setup(mapper => mapper.Map<Order>(updateOrderRequestDto))
                .Returns(orderDomain);

            _mockOrderRepository
                .Setup(repo => repo.UpdateAsync(1, orderDomain))
                .ReturnsAsync(orderDomain);

            // Act
            await _ordersController.Update(1, updateOrderRequestDto);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("Обновление заказа с ID: {Id}, данные: {OrderData}", 1, updateOrderRequestDto), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Заказ с ID {Id} обновлен", 1), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsOkObjectResult_WhenOrderDeleted()
        {
            // Arrange
            var orderDomain = new Order { Id = 1, Name = "Order 1" };
            var orderDto = new OrderDto { Id = 1, Name = "Order 1" };

            _mockOrderRepository
                .Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(orderDomain);

            _mockMapper
                .Setup(mapper => mapper.Map<OrderDto>(orderDomain))
                .Returns(orderDto);

            // Act
            var result = await _ordersController.Delete(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(orderDto, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            _mockOrderRepository
                .Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _ordersController.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_LogsInformation_WhenOrderDeleted()
        {
            // Arrange
            var orderDomain = new Order { Id = 1, Name = "Order 1" };

            _mockOrderRepository
                .Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(orderDomain);

            // Act
            await _ordersController.Delete(1);

            // Assert
            _mockLogger.Verify(logger => logger.LogInformation("Удаление заказа с ID: {Id}", 1), Times.Once);
            _mockLogger.Verify(logger => logger.LogInformation("Заказ с ID {Id} удален", 1), Times.Once);
        }
    }
}
