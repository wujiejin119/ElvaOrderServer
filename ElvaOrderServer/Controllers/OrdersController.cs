using ElvaOrderServer.Application.DTOs;
using ElvaOrderServer.Application.Services;
using ElvaOrderServer.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElvaOrderServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="request">Order creation request</param>
        /// <returns>Created order ID</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var response = await _orderService.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetOrder), new { id = response.OrderId }, response);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { ErrorCode = ex.ErrorCode, ex.Message, ex.Details });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (OrderNotFoundException ex)
            {
                _logger.LogWarning("Order not found: {OrderId}", ex.OrderId);
                return NotFound(new { ex.OrderId, ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}