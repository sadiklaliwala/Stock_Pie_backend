using AutoMapper;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.BankAccountLast4, opt => opt.MapFrom(src => src.BankAccountLast4));

            CreateMap<Transaction, TransactionDto>();
            CreateMap<Portfolio, PortfolioResponseDto>();
            CreateMap<Coin, CoinSummaryDto>();
        }
    }
}