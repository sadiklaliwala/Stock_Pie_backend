using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController(IOrderService orderService, ICoinService coinService, IUserService userService, IUserContext userContext) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IUserContext _userContext = userContext;
        private readonly ICoinService _coinService = coinService;
        private readonly IUserService _userService = userService;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId);
            var coin = await _coinService.FindByIdAsync(req.Symbol);

            if(user == null || coin == null)
            {
                return NotFound("Coin Or User Not Exist0");
            }

            // map request to domain objects
            // lightweight stub; OrderService should fetch user details as needed
            //var item = new OrderItem { Quantity = req.Quantity, BuyPrice = (double)req.Price };
            //var order = await _orderService.CreateOrderAsync(user, item, req.OrderType);
            var order = await _orderService.ProcessOrderAsync(coin, req.Quantity, req.OrderType, user);
            return Ok(new OrderResponseDto()
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderType = order.OrderType.ToString(),
                Status = order.Status.ToString(),
                Price = order.Price,
                TimeStamp = order.TimeStamp,
                Quantity = order.OrderItem?.Quantity ?? 0
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
