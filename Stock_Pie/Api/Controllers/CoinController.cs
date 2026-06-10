using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Dto;
using AutoMapper;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoinController(ICoinService coinService, IMapper mapper) : ControllerBase
    {
        private readonly ICoinService _coinService = coinService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("list/{page}")]
        public async Task<IActionResult> List(int page = 1)
        {
            var coins = await _coinService.GetCoinListAsync(page);
            var dtos = _mapper.Map<IEnumerable<CoinSummaryDto>>(coins);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var coin = await _coinService.FindByIdAsync(id);
            if (coin == null) return NotFound();
            var dto = _mapper.Map<CoinSummaryDto>(coin);
            return Ok(dto);
        }

        [HttpGet("{id}/chart/{days}")]
        public async Task<IActionResult> Chart(string id, int days = 7)
        {
            var chartJson = await _coinService.GetMarketChartAsync(id, days);
            return Content(chartJson, "application/json");
        }

        [HttpGet("top50")]
        public async Task<IActionResult> Top50()
        {
            var json = await _coinService.GetTop50CoinsByMarketCapRankAsync();
            return Content(json, "application/json");
        }

        [HttpGet("trending")]
        public async Task<IActionResult> Trending()
        {
            var json = await _coinService.GetTrendingCoinsAsync();
            return Content(json, "application/json");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var json = await _coinService.SearchCoinAsync(q);
            return Content(json, "application/json");
        }

        [HttpGet("details/{coinId}")]
        public async Task<IActionResult> GetDetailByCoin([FromRoute] string coinId)
        {
            var coin = await _coinService.GetCoinDetailsAsync(coinId);
            return Content(coin, "application/json");
        }

    }
}
