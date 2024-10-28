using AutoMapper;
using Delivery.Data;
using Delivery.Logging;
using Delivery.Models.Domain;
using Delivery.Models.DTO;
using Delivery.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Delivery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DeliveryDbContext dbContext;
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly ILogger<OrdersController> _logger;
        private readonly FilteredOrdersLogger _filteredOrdersLogger;

        public OrdersController(DeliveryDbContext dbContext, IOrderRepository orderRepository,
            IMapper mapper, ILogger<OrdersController> logger, FilteredOrdersLogger filteredOrdersLogger)
        {
            this.dbContext = dbContext;
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this._logger = logger;
            _filteredOrdersLogger = filteredOrdersLogger;
        }

        //GET ALL ORDERS
        //GET: https//localhost:portnumber/api/orders?&fromdate=Name&District=name
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? fromDate, [FromQuery] string? district)
        {
            _logger.LogInformation("Запрос на получение всех заказов с фильтрами: fromDate={FromDate}, district={District}", fromDate, district);
            if (!string.IsNullOrWhiteSpace(district) && !Regex.IsMatch(district, @"^[a-zA-Zа-яА-ЯёЁ\s]+$")) // Изменено регулярное выражение
            {
                return BadRequest("Район может содержать только буквы и пробелы.");
            }
            try
            {
                var ordersDomain = await orderRepository.GetAllAsync(fromDate, district);

                _logger.LogInformation("Найдено {Count} заказов", ordersDomain.Count);

                _filteredOrdersLogger.ClearTable();
                _filteredOrdersLogger.LogOrders(ordersDomain);

                _logger.LogInformation("Найдено {Count} заказов после фильтрации", ordersDomain.Count);

                return Ok(mapper.Map<List<OrderDto>>(ordersDomain));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Ошибка валидации данных.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса.");
                return StatusCode(500, "Произошла ошибка на сервере.");
            }
        }



        //GET SINGLE ORDER
        //GET: https://localhost:portnumber/api/orders/{id}

        [HttpGet]
        [Route("{id}: int")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            _logger.LogInformation("Запрос на получение заказа с ID: {Id}", id);

            var orderDomain = await orderRepository.GetByIdAsync(id);
            if (orderDomain == null)
            {
                _logger.LogWarning("Заказ с ID {Id} не найден", id);
                return NotFound();
            }

            _logger.LogInformation("Заказ с ID {Id} найден", id);

            return Ok(mapper.Map<OrderDto>(orderDomain));
        }

        //POST To Create a New Order
        //POST: https://localhost:portnumber/api/order

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddOrderRequestDto addOrderRequestDto)
        {
            _logger.LogInformation("Создание нового заказа: {OrderData}", addOrderRequestDto);

            var orderDomain = mapper.Map<Order>(addOrderRequestDto);

            orderDomain = await orderRepository.CreateAsync(orderDomain);

            _logger.LogInformation("Создан новый заказ с ID: {Id}", orderDomain.Id);

            var orderDto = mapper.Map<OrderDto>(orderDomain);
            return CreatedAtAction(nameof(GetById), new { id = orderDomain.Id }, orderDto);
        }


        [HttpPut]
        [Route("{id}: int")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOrderRequestDto updateOrderRequestDto)
        {
            _logger.LogInformation("Обновление заказа с ID: {Id}, данные: {OrderData}", id, updateOrderRequestDto);

            var orderDomainModel = mapper.Map<Order>(updateOrderRequestDto);

            orderDomainModel = await orderRepository.UpdateAsync(id, orderDomainModel);

            if (orderDomainModel == null)
            {
                _logger.LogWarning("Заказ с ID {Id} не найден для обновления", id);
                return NotFound();
            }

            _logger.LogInformation("Заказ с ID {Id} обновлен", id);

            return Ok(mapper.Map<OrderDto>(orderDomainModel));
        }

        //Delete order
        //DELETE: https://localhotst:portnumber/api/regions/{id}

        [HttpDelete]
        [Route("{id}: int")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            _logger.LogInformation("Удаление заказа с ID: {Id}", id);

            var orderDomainModel = await orderRepository.DeleteAsync(id);

            if (orderDomainModel == null)
            {
                _logger.LogWarning("Заказ с ID {Id} не найден для удаления", id);
                return NotFound();
            }

            _logger.LogInformation("Заказ с ID {Id} удален", id);

            return Ok(mapper.Map<OrderDto>(orderDomainModel));
        }
    }
}
