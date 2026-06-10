using Stock_Pie.Api.Controllers;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;
using System.Net;

namespace Stock_Pie.Application.Interfaces
{
    public interface IPaymentService
    {
        // ─── Shared ──────────────────────────────────────────────────────

        // Create a new payment order in DB
        Task<PaymentOrder> CreateOrder(User user, decimal amount, Domain.Entities.PaymentMethod paymentMethod);

        // Get order by id
        Task<PaymentOrder> GetPaymentOrderById(Guid id);

        // Update order status
        Task<bool> ProccedPaymentOrder(PaymentOrder order, Guid id);

        // ─── Razorpay ────────────────────────────────────────────────────

        // Step 1 - Create Razorpay order for popup
        // Returns RazorpayOrderId + RazorpayKey to frontend
        Task<PaymentResponse> CreateRazorPayOrder(User user, decimal amount);

        // Step 2 - Verify signature after popup success
        // Returns true if signature valid + updates order status to Success
        Task<bool> VerifyRazorPayPayment(RazorpayVerifyDto dto);

        // ─── Stripe ──────────────────────────────────────────────────────

        // Step 1 - Create Stripe PaymentIntent for Payment Element
        // Returns ClientSecret + PublishableKey to frontend
        Task<PaymentResponse> CreateStripePaymentIntent(User user, decimal amount, Guid orderId);

        // Step 2 - Confirm payment after Payment Element success
        // Fetches PaymentIntent from Stripe + updates order status to Success
        Task<bool> ConfirmStripePayment(Guid orderId, string paymentIntentId);
    }
}
