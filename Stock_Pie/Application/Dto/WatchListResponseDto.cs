using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class WatchListResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }

        public List<WatchListCoinDto> Coins { get; set; } = new List<WatchListCoinDto>();
    }

    public class WatchListCoinDto
    {
        public string? Id { get; set; }
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public decimal CurrentPrice { get; set; }
        public string? Image { get; set; }
    }
}
