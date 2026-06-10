namespace Stock_Pie.Domain.Entities
{
    public enum OrderType
    {
        Buy,
        Sell
    }
    public enum OrderStatus
    {
        Pending,
        Failed,
        Canceled,
        PartiallyFilled,
        Error,
        Success
    }


}
