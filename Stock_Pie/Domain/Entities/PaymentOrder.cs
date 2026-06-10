namespace Stock_Pie.Domain.Entities
{

    public enum PaymentOrderStatus
    {

        Pending,
        Failed,
        Success
    }
    public enum PaymentMethod
    {
        Stripe,
        Razorpay,
    }

    public class PaymentOrder
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentOrderStatus Status { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }

}


