//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Stock_Pie.Application.Dto;
//using Stock_Pie.Application.Interfaces;
//using Stock_Pie.Domain.Entities;
//using System.Threading.Tasks;

//namespace Stock_Pie.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class PaymentController : ControllerBase
//    {
//        private readonly IPaymentService _service;
//        private readonly IUserContext _userContext;

//        public PaymentController(IPaymentService service, IUserContext userContext)
//        {
//            _service = service;
//            _userContext = userContext;
//        }

//        [HttpPost("create-order")]
//        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentOrderDto dto)
//        {
//            var userId = _userContext.UserId;
//            if (userId == Guid.Empty) return Unauthorized();
//            var user = new User { Id = userId };
//            var order = await _service.CreateOrder(user, dto.Amount, dto.Method);
//            return Ok(order);
//        }

//        [HttpPost("razorpay-link/{orderId}")]
//        public async Task<IActionResult> CreateRazorPayLink(Guid orderId)
//        {
//            var userId = _userContext.UserId;
//            if (userId == Guid.Empty) return Unauthorized();
//            var user = new User { Id = userId };
//            var order = await _service.GetPaymentOrderById(orderId);
//            if (order == null) return NotFound();
//            var resp = await _service.CreateRazorPayPaymentLink(user, order.Amount);
//            return Ok(resp);
//        }

//        [HttpPost("stripe-session/{orderId}")]
//        public async Task<IActionResult> CreateStripeSession(Guid orderId)
//        {
//            var userId = _userContext.UserId;
//            if (userId == Guid.Empty) return Unauthorized();
//            var user = new User { Id = userId };
//            var order =await _service.GetPaymentOrderById(orderId);
//            if (order == null) return NotFound();
//            var resp =  _service.CreateStripePayPaymentLink(user, order.Amount, order.Id);
//            return Ok(resp);
//        }
//    }

//    public record CreatePaymentOrderDto(decimal Amount, PaymentMethod Method);
//}






using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using System.Threading.Tasks;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        private readonly IUserContext _userContext;

        public PaymentController(IPaymentService service, IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentOrderDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var user = new User { Id = userId };
            var order = await _service.CreateOrder(user, dto.Amount, dto.Method);
            return Ok(order);
        }

        

        // Step 1 - Create Razorpay Order (returns order_id for popup)
        [HttpPost("razorpay-order/{orderId}")]
        public async Task<IActionResult> CreateRazorPayOrder(Guid orderId)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var user = new User { Id = userId };
            var order = await _service.GetPaymentOrderById(orderId);
            if (order == null) return NotFound();

            // Returns RazorpayOrderId + RazorpayKey for frontend popup
            var resp = await _service.CreateRazorPayOrder(user, order.Amount);
            return Ok(resp);
        }

        // Step 2 - Verify Razorpay Payment after popup success
        [HttpPost("razorpay-verify")]
        public async Task<IActionResult> VerifyRazorPay([FromBody] RazorpayVerifyDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var isValid = await _service.VerifyRazorPayPayment(dto);
            if (!isValid) return BadRequest(new { message = "Payment verification failed" });

            return Ok(new { message = "Payment verified successfully" });
        }

        

        // Step 1 - Create Stripe Payment Intent (returns clientSecret for Payment Element)
        [HttpPost("stripe-intent/{orderId}")]
        public async Task<IActionResult> CreateStripeIntent(Guid orderId)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var user = new User { Id = userId };
            var order = await _service.GetPaymentOrderById(orderId);
            if (order == null) return NotFound();

            // Returns ClientSecret for frontend Payment Element
            var resp = await _service.CreateStripePaymentIntent(user, order.Amount, order.Id);
            return Ok(resp);
        }

        // Step 2 - Confirm Stripe Payment after Payment Element success
        [HttpPost("stripe-confirm/{orderId}")]
        public async Task<IActionResult> ConfirmStripePayment(Guid orderId, [FromBody] StripeConfirmDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var isValid = await _service.ConfirmStripePayment(orderId, dto.PaymentIntentId);
            if (!isValid) return BadRequest(new { message = "Stripe payment confirmation failed" });

            return Ok(new { message = "Payment confirmed successfully" });
        }
    }

    
    public record CreatePaymentOrderDto(decimal Amount, PaymentMethod Method);

    public record RazorpayVerifyDto(
        string RazorpayOrderId,
        string RazorpayPaymentId,
        string RazorpaySignature,
        Guid InternalOrderId        // your DB order id to update status
    );

    public record StripeConfirmDto(
        string PaymentIntentId      // returned by Stripe on frontend after payment
    );
}