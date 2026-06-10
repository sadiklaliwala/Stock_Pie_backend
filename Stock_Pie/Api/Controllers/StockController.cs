//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Stock_Pie.Application.Dto;
//using Stock_Pie.Infrastructure.Api;
//using Stock_Pie.Infrastructure.Services;
//using System.Threading.Tasks;

//namespace Stock_Pie.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StockController(StockService _stockService) : ControllerBase
//    {
//        [HttpGet("Sync")]
//        public async Task<IActionResult> FetchStockDataFromAPi()
//        {
//            await _stockService.InsertStockData();
//            return Ok("Stock data fetched successfully");
//        }

//        [HttpGet("{take:int}/{skip:int}")]
//        public async Task<List<StockListResponseDto>> GetAllStockDto(
//            [FromRoute] int take,
//            [FromRoute] int skip,

//            [FromQuery] string? exchange,
//            [FromQuery] string? country,
//            [FromQuery] string? currency,
//            [FromQuery] string? type,

//            [FromQuery] string? sortBy,
//            [FromQuery] string? sortOrder
//        )
//        {
//            return await _stockService.FetchData(
//                take,
//                skip,
//                exchange,
//                country,
//                currency,
//                type,
//                sortBy,
//                sortOrder
//            );
//        }

//        [HttpGet("Search/{symbol}")]
//        public async Task<List<StockSearchResponse>> SearchStock([FromRoute] string symbol)
//        {
//            var data = await _stockService.SearchStock(symbol);
//            return data;
//        }

//        [HttpGet("searchByName/{name}")]
//        public async Task<StockByNameResponseDto> SearchByName([FromRoute] string name)
//        {
//            var data = await _stockService.SearchByName(name);
//            return data;
//        }
//    }
//}
