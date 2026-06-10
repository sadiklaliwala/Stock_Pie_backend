using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using System.Threading.Tasks;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WithdrawlController : ControllerBase
    {
        private readonly IWithdrawlService _service;
        private readonly IUserContext _userContext;

        public WithdrawlController(IWithdrawlService service, IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestWithDrawal([FromBody] RequestWithdrawalDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            var wd = await _service.RequestWithdrawal(dto.Amount, userId, dto.BankAccountNumber);
            return Ok(wd);
        }

        [HttpPost("process/{id}")]
        public async Task<IActionResult> Process(Guid id, [FromQuery] bool accept)
        {
            // admin-only in real app
            var wd = await _service.ProcedWithWidrawal(id, accept);
            return Ok(wd);
        }

        [HttpGet("me")]
        public async Task<IActionResult> MyHistory()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var list = await _service.GetUsersWithdrawalHistory(userId);
            return Ok(list);
        }

        [HttpGet("all")]
        public async Task<IActionResult> AllRequests()
        {
            // should be admin-only in prod
            var list = await _service.GetAllWithdrawalRequest();
            return Ok(list);
        }
    }

    public record RequestWithdrawalDto(decimal Amount, string? BankAccountNumber);
}
