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
    public class TransactionsController : ControllerBase
    {
        //private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ITradingService _trading;
        private readonly ITransactionService _service;


        public TransactionsController(IUserContext userContext, IMapper mapper, ITradingService tradingService, ITransactionService service )
        {
            //_mediator = mediator;
            _userContext = userContext;
            _mapper = mapper;
            _trading = tradingService;
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] TransactionCreateDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            Transaction tx;
            //var tx = await _mediator.Send(new CreateTradeCommand(userId, dto));
            if (dto.Type == TransactionType.Buy)
            {
                tx = await _trading.BuyAsync(userId, dto);
            }
            else
            {
                tx = await _trading.SellAsync(userId, dto);
            }
            var outDto = _mapper.Map<TransactionDto>(tx);
            return CreatedAtAction(nameof(GetByUser), new { }, outDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByUser()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            var txs = await _service.GetByUserAsync(userId);
            //var txs = await _mediator.Send(new GetTransactionsByUserQuery(userId));
            var outDtos = _mapper.Map<IEnumerable<TransactionDto>>(txs);
            return Ok(outDtos);
        }
    }
}