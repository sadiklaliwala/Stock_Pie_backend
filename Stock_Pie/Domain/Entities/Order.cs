namespace Stock_Pie.Domain.Entities
{

    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        //many to one relationship with user
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public required OrderType OrderType { get; set; }
        public required OrderStatus Status { get; set; }
        public OrderItem? OrderItem { get; set; }
        public required decimal Price { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
