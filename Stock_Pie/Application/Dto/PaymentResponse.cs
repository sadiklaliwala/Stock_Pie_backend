namespace Stock_Pie.Application.Dto
{
    public class PaymentResponse
    {
        // Razorpay
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayKey { get; set; }

        // Stripe
        public string? ClientSecret { get; set; }
        public string? StripePublishableKey { get; set; }
    }
}
