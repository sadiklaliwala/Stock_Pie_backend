using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;
using System.Linq;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IUserContext _userContext;
        private readonly ICoinService _coinService;

        public AssetController(IAssetService assetService, IUserContext userContext, ICoinService coinService)
        {
            _assetService = assetService;
            _userContext = userContext;
            _coinService = coinService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyAssets()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var assets = await _assetService.GetUsersAssetsAsync(userId);
            var dtos = assets.Select(a => MapToAssetDto(a)).ToList();

            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsset(Guid id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();

            var dto = MapToAssetDto(asset);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssetDto dto)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var user = new User { Id = userId };
            var coin = await _coinService.FindByIdAsync(dto.CoinId);
            if (coin == null) return NotFound("Coin not found");

            var asset = await _assetService.CreateAssetAsync(user, coin, dto.Quantity);

            var created = await _assetService.GetAssetByIdAsync(asset.Id);
            if (created == null) return NotFound();
            var createdDto = MapToAssetDto(created);

            return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssetDto dto)
        {
            var asset = await _assetService.UpdateAssetAsync(id, dto.Quantity, dto.BuyPrice);
            var result = await _assetService.GetAssetByIdAsync(asset.Id);
            if(result == null) return NotFound();
            return Ok(MapToAssetDto(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _assetService.DeleteAssetAsync(id);
            return NoContent();
        }

        private static AssetDto MapToAssetDto(Asset a)
        {
            var coin = a.Coin;
            var user = a.User;
            Console.WriteLine("User" + user);
            var coinDto = coin == null ? null : new CoinDto
            {
                Id = coin.Id,
                Symbol = coin.Symbol,
                Name = coin.Name,
                Image = coin.Image,
                CurrentPrice = coin.CurrentPrice,
                MarketCap = coin.MarketCap,
                MarketCapRank = coin.MarketCapRank,
                TotalVolume = coin.TotalVolume,
                High24h = coin.High24h,
                Low24h = coin.Low24h,
                PriceChange24h = coin.PriceChange24h,
                PriceChangePercentage24h = coin.PriceChangePercentage24h,
                LastUpdated = coin.LastUpdated
            };

            var userDto = user == null ? null : new UserSummaryDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };

            return new AssetDto
            {
                Id = a.Id,
                Coin = coinDto,
                User = userDto,
                Quantity = a.Quantity,
                BuyPrice = a.BuyPrice
            };
        }
    }
}
