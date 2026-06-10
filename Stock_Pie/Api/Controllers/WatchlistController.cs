using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Application.Dto;
using System.Threading.Tasks;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _service;
        private readonly IUserContext _userContext;

        public WatchlistController(IWatchlistService service, IUserContext userContext)
        {
            _service = service;
            _userContext = userContext;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWatchlist()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            var wl = await _service.FindUserWatchList(userId);
            if (wl == null) return NotFound();
            return Ok(wl);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            await _service.CreateWatchList(userId);

            var wlDto = await _service.FindUserWatchList(userId);
            return Ok(wlDto);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddToWatchlistDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            var coin = new Coin { Id = dto.CoinId };
            var added = await _service.AddItemToWatchListAsync(coin, userId);

            // return lightweight DTO
            var result = new WatchListCoinDto
            {
                Id = added.Id,
                Symbol = added.Symbol,
                Name = added.Name,
                CurrentPrice = added.CurrentPrice,
                Image = added.Image
            };

            return Ok(result);
        }

        [HttpDelete("{coinId}")]
        public async Task<IActionResult> RemoveCoin(string coinId)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var removed = await _service.RemoveCoinFromWatchlistAsync(coinId, userId);
            if (!removed) return NotFound("Coin not found in watchlist");

            return NoContent();
        }
    }

    public record AddToWatchlistDto(string CoinId);
}
