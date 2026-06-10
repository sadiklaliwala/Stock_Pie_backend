using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Services;
using Stock_Pie.Domain.Entities;
using Stripe;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IPortfolioService _portfolioService;
        private readonly ITradingService _trading;

        public PortfoliosController(IMediator mediator, IUserContext userContext, IMapper mapper, IPortfolioService portfolioService,ITradingService tradingService)
        {
            _mediator = mediator;
            _userContext = userContext;
            _mapper = mapper;
            _portfolioService = portfolioService;
            _trading = tradingService;
        }

        [HttpPost("buy")]
        public async Task<ActionResult<PortfolioResponseDto>> Buy([FromBody] TransactionCreateDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
                
            var p = await _portfolioService.UpsertPortfolioForBuyAsync(userId, dto.Symbol, dto.Quantity, dto.PriceAtTransaction);
            //var p = await _mediator.Send(new CreateOrUpdatePortfolioCommand(userId, dto.Symbol, dto.Quantity, dto.PriceAtTransaction));
            var outDto = _mapper.Map<PortfolioResponseDto>(p);
            return Ok(outDto);
        }

        [HttpPost("sell")]
        public async Task<ActionResult<PortfolioResponseDto>> Sell([FromBody] TransactionCreateDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            await _trading.SellAsync(userId, dto);

            var portfolio = await _portfolioService.GetByUserAndSymbolAsync(userId, dto.Symbol);
            if (portfolio == null) return NoContent();
            var outDto = _mapper.Map<PortfolioResponseDto>(portfolio);
            return Ok(outDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var ok = await _portfolioService.DeleteAsync(Id);
            //var ok = await _mediator.Send(new DeletePortfolioCommand(id));
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}