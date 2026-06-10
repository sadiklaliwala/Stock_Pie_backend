namespace Stock_Pie.Application.Dto
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? OrderType { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Quantity { get; set; }
    }
}
