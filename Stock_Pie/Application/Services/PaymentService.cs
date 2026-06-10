//using Stripe;
//using Stripe.Checkout;
//using System.Net.Http;
//using System.Text.Json;
//using System.Text;
//using Stock_Pie.Application.Dto;
//using Stock_Pie.Application.Interfaces;
//using Stock_Pie.Domain.Entities;
//using System.Threading.Tasks;

//namespace Stock_Pie.Application.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly IPaymentRepository _repo;
//        private readonly IConfiguration _config;
//        private readonly IHttpClientFactory _httpFactory;

//        public PaymentService(IPaymentRepository repo, IConfiguration config, IHttpClientFactory httpFactory)
//        {
//            _repo = repo;
//            _config = config;
//            _httpFactory = httpFactory;
//            // initialize Stripe
//            StripeConfiguration.ApiKey = _config["Stripe:Secret"] ?? string.Empty;
//        }

//        public async Task<PaymentOrder> CreateOrder(User User, decimal amount, Domain.Entities.PaymentMethod paymentMethod)
//        {
//            var order = new PaymentOrder
//            {
//                Id = Guid.NewGuid(),
//                Amount = amount,
//                PaymentMethod = paymentMethod,
//                Status = PaymentOrderStatus.Pending,
//                UserId = User.Id
//            };
//           await _repo.AddAsync(order);
//            await _repo.SaveChangesAsync();
//            return order;
//        }

//        public async Task<PaymentOrder> GetPaymentOrderById(Guid Id)
//        {
//            return await _repo.GetByIdAsync(Id) ?? throw new InvalidOperationException("Order not found");
//        }

//        public async Task<Boolean> ProccedPaymentOrder(PaymentOrder order, Guid Id)
//        {
//            var existing = await _repo.GetByIdAsync(Id) ?? throw new InvalidOperationException("Order not found");
//            existing.Status = order.Status;
//            await _repo.UpdateAsync(existing);
//            await _repo.SaveChangesAsync();
//            return true;
//        }

//        public async Task<PaymentResponse> CreateRazorPayPaymentLink(User user, decimal amount)
//        {
//            // Use Razorpay REST API to create payment link because the Razorpay .NET SDK package is not available in this environment.
//            var key = _config["Razorpay:Key"] ?? string.Empty;
//            var secret = _config["Razorpay:Secret"] ?? string.Empty;
//            var client = _httpFactory.CreateClient();

//            var payload = new
//            {
//                amount = (int)(amount * 100),
//                currency = "INR",
//                accept_partial = false,
//                description = $"Payment for user {user.Id}",
//                reference_id = Guid.NewGuid().ToString()
//            };

//            var json = JsonSerializer.Serialize(payload);
//            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.razorpay.com/v1/payment_links")
//            {
//                Content = new StringContent(json, Encoding.UTF8, "application/json")
//            };
//            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(key + ":" + secret));
//            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

//            var resp =await client.SendAsync(req);
//            if (!resp.IsSuccessStatusCode) throw new Exception("Razorpay create link failed: " + resp.StatusCode);

//            var respJson = await resp.Content.ReadAsStringAsync();
//            using var doc = JsonDocument.Parse(respJson);
//            var shortUrl = doc.RootElement.GetProperty("short_url").GetString();
//            return new PaymentResponse { Payment_Url = shortUrl };
//        }

//        public PaymentResponse CreateStripePayPaymentLink(User user, decimal amount, Guid OrderId)
//        {
//            var successUrl = _config["Stripe:SuccessUrl"] ?? "https://example.com/success";
//            var cancelUrl = _config["Stripe:CancelUrl"] ?? "https://example.com/cancel";

//            var options = new SessionCreateOptions
//            {
//                PaymentMethodTypes = new List<string> { "card" },
//                Mode = "payment",
//                LineItems = new List<SessionLineItemOptions>
//                {
//                    new SessionLineItemOptions
//                    {
//                        PriceData = new SessionLineItemPriceDataOptions
//                        {
//                            UnitAmount = (long)(amount * 100),
//                            Currency = "usd",
//                            ProductData = new SessionLineItemPriceDataProductDataOptions
//                            {
//                                Name = $"Order {OrderId}"
//                            }
//                        },
//                        Quantity = 1
//                    }
//                },
//                SuccessUrl = successUrl,
//                CancelUrl = cancelUrl
//            };

//            var service = new SessionService();
//            var session = service.Create(options);
//            return new PaymentResponse { Payment_Url = session.Url };
//        }
//    }
//}
using Stripe;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Api.Controllers;

namespace Stock_Pie.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpFactory;
        private readonly IWalletService _walletService;


        public PaymentService(IPaymentRepository repo, IConfiguration config, IHttpClientFactory httpFactory, IWalletService walletService)
        {
            _repo = repo;
            _config = config;
            _httpFactory = httpFactory;
            StripeConfiguration.ApiKey = _config["Stripe:Secret"] ?? string.Empty;
            _walletService = walletService;
        }

        // ─── Shared ──────────────────────────────────────────────────────

        public async Task<PaymentOrder> CreateOrder(User user, decimal amount, Domain.Entities.PaymentMethod paymentMethod)
        {
            var order = new PaymentOrder
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = PaymentOrderStatus.Pending,
                UserId = user.Id
            };
            await _repo.AddAsync(order);
            await _repo.SaveChangesAsync();
            return order;
        }

        public async Task<PaymentOrder> GetPaymentOrderById(Guid id)
        {
            return await _repo.GetByIdAsync(id) ?? throw new InvalidOperationException("Order not found");
        }

        public async Task<bool> ProccedPaymentOrder(PaymentOrder order, Guid id)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new InvalidOperationException("Order not found");
            existing.Status = order.Status;
            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();
            return true;
        }

        // ─── Razorpay ────────────────────────────────────────────────────

        // Step 1 - Create Razorpay Order (for popup)
        public async Task<PaymentResponse> CreateRazorPayOrder(User user, decimal amount)
        {
            var key = _config["Razorpay:Key"] ?? string.Empty;
            var secret = _config["Razorpay:Secret"] ?? string.Empty;
            var client = _httpFactory.CreateClient();

            var payload = new
            {
                amount = (int)(amount * 100),   // in paise
                currency = "INR",
                receipt = Guid.NewGuid().ToString()
            };

            var json = JsonSerializer.Serialize(payload);
            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.razorpay.com/v1/orders")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(key + ":" + secret));
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            var resp = await client.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Razorpay order creation failed: " + resp.StatusCode);

            var respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respJson);

            var razorpayOrderId = doc.RootElement.GetProperty("id").GetString();

            // Return order_id + public key to frontend for popup
            return new PaymentResponse
            {
                RazorpayOrderId = razorpayOrderId,
                RazorpayKey = key         // public key, safe to send to frontend
            };
        }

        // Step 2 - Verify Razorpay Signature after popup success
        public async Task<bool> VerifyRazorPayPayment(RazorpayVerifyDto dto)
        {
            var secret = _config["Razorpay:Secret"] ?? string.Empty;

            // Razorpay signature = HMAC SHA256 of "order_id|payment_id" using secret
            var payload = $"{dto.RazorpayOrderId}|{dto.RazorpayPaymentId}";
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(payloadBytes);
            var generatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            // Compare generated signature with Razorpay signature
            if (generatedSignature != dto.RazorpaySignature)
                return false;

            // Signature valid → update order status to Success
            var order = await _repo.GetByIdAsync(dto.InternalOrderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.Status == PaymentOrderStatus.Success)
                return true;

            order.Status = PaymentOrderStatus.Success;
            await _repo.UpdateAsync(order);
            await _repo.SaveChangesAsync();
            await _walletService.AddBalanceAsync(order.UserId, order.Amount);
            return true;
        }

        // ─── Stripe ──────────────────────────────────────────────────────

        // Step 1 - Create Stripe Payment Intent (for Payment Element)
        public async Task<PaymentResponse> CreateStripePaymentIntent(User user, decimal amount, Guid orderId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),  // in cents
                Currency = "usd",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() },
                    { "userId", user.Id.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            // Return clientSecret to frontend for Payment Element
            return new PaymentResponse
            {
                ClientSecret = intent.ClientSecret,
                StripePublishableKey = _config["Stripe:PublishableKey"]
            };
        }

        // Step 2 - Confirm Stripe Payment after Payment Element success
        public async Task<bool> ConfirmStripePayment(Guid orderId, string paymentIntentId)
        {
            // Fetch PaymentIntent from Stripe to verify status
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId);

            if (intent.Status != "succeeded")
                return false;

            // Status is succeeded → update order in DB
            var order = await _repo.GetByIdAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.Status == PaymentOrderStatus.Success)
                return true;

            order.Status = PaymentOrderStatus.Success;
            await _repo.UpdateAsync(order);
            await _repo.SaveChangesAsync();
            await _walletService.AddBalanceAsync(order.UserId, order.Amount);

            return true;
        }
    }
}