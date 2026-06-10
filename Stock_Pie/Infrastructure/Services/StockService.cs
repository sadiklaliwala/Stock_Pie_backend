//using EFCore.BulkExtensions;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Conventions;
//using Stock_Pie.Application.Dto;
//using Stock_Pie.Domain.Entities;
//using Stock_Pie.Infrastructure.Api;
//using Stock_Pie.Infrastructure.Persistence;
//using System.Threading.Tasks;

//namespace Stock_Pie.Infrastructure.Services
//{
//    public class StockService(AppDbContext context, StockApi stockApi)
//    {
//        private readonly AppDbContext _context = context;
//        private readonly StockApi _stockApi = stockApi;

//        public async Task InsertStockData()
//        {
//            var stockData = await _stockApi.GetStockDataAsync();
//            if (stockData == null || !(stockData.Count > 0))
//                return;

//            // deduplicate
//            var allStocks = stockData
//                .Where(s => !string.IsNullOrWhiteSpace(s.Symbol))
//                .GroupBy(s => s.Symbol.Trim(), StringComparer.OrdinalIgnoreCase)
//                .Select(g => g.First())
//                .Select(s => new {
//                    Name = s.Name.Trim(),
//                    Symbol = s.Symbol.Trim(),
//                    Exchange = s.Exchange.Trim(),
//                    MicCode = s.MicCode?.Trim(),
//                    Country = s.Country?.Trim(),
//                    Currency = s.Currency.Trim(),
//                    Type = s.Type.Trim(),
//                    IsActive = true,
//                    CreatedAt = DateTime.UtcNow,
//                    UpdatedAt = DateTime.UtcNow,
//                })
//                .ToList();

//            if (!(allStocks.Count > 0))
//                return;

//            // existing symbols set
//            var existingSymbols = _context.Set<dynamic>()
//                .Select(x => (string)x.Symbol)
//                .AsEnumerable()
//                .ToHashSet(StringComparer.OrdinalIgnoreCase);

//            var toInsert = allStocks
//                .Where(s => !existingSymbols.Contains(s.Symbol))
//                .ToList();

//            var toUpdate = allStocks
//                .Where(s => existingSymbols.Contains(s.Symbol))
//                .ToList();

//            _context.ChangeTracker.AutoDetectChangesEnabled = false;

//            if (toInsert.Count > 0)
//            {
//                var insertConfig = new BulkConfig
//                {
//                    BatchSize = 5000,
//                    BulkCopyTimeout = 300,
//                };

//                int batchSize = 10000;
//                for (int i = 0; i < toInsert.Count; i += batchSize)
//                {
//                    var batch = toInsert.Skip(i).Take(batchSize).ToList();
//                    await _context.BulkInsertAsync(batch, insertConfig);
//                }
//            }

//            if (toUpdate.Count > 0)
//            {
//                var updateConfig = new BulkConfig
//                {
//                    UpdateByProperties = new List<string> { "Symbol" },
//                    PropertiesToExcludeOnUpdate = new List<string> { "Id", "CreatedAt" },
//                    BatchSize = 5000,
//                    BulkCopyTimeout = 300,
//                };

//                int batchSize = 10000;
//                for (int i = 0; i < toUpdate.Count; i += batchSize)
//                {
//                    var batch = toUpdate.Skip(i).Take(batchSize).ToList();
//                    await _context.BulkUpdateAsync(batch, updateConfig);
//                }
//            }
//        }

//        public async Task<List<StockListResponseDto>> FetchData(
//    int take,
//    int size,
//    string? exchange = "NSE",
//    string? country = "India",
//    string? currency = "INR",
//    string? type = null,
//    string? sortBy = "symbol",
//    string? sortOrder = "asc")
//        {
//            var query =  _context.Set<dynamic>().AsQueryable();

//            // Filtering
//            if (!string.IsNullOrWhiteSpace(exchange))
//                query = query.Where(x => x.Exchange == exchange);

//            if (!string.IsNullOrWhiteSpace(country))
//                query = query.Where(x => x.Country == country);

//            if (!string.IsNullOrWhiteSpace(currency))
//                query = query.Where(x => x.Currency == currency);

//            if (!string.IsNullOrWhiteSpace(type))
//                query = query.Where(x => x.Type == type);

//            // Sorting
//            sortBy = sortBy?.ToLower();
//            sortOrder = sortOrder?.ToLower();

//            switch (sortBy)
//            {
//                case "exchange":
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.Exchange)
//                        : query.OrderBy(x => x.Exchange);
//                    break;

//                case "miccode":
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.MicCode)
//                        : query.OrderBy(x => x.MicCode);
//                    break;

//                case "country":
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.Country)
//                        : query.OrderBy(x => x.Country);
//                    break;

//                case "currency":
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.Currency)
//                        : query.OrderBy(x => x.Currency);
//                    break;

//                case "type":
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.Type)
//                        : query.OrderBy(x => x.Type);
//                    break;

//                default:
//                    query = sortOrder == "desc"
//                        ? query.OrderByDescending(x => x.Symbol)
//                        : query.OrderBy(x => x.Symbol);
//                    break;
//            }

//            var stocks = await query
//                .Skip((take - 1) * size)
//                .Take(size)
//                .AsNoTracking()
//                .ToListAsync();

//            var result = new List<StockListResponseDto>();

//            foreach (var s in stocks)
//            {
//                var basePrice = s.LastPrice > 0 ? s.LastPrice : 100;
//                var newPrice = _stockApi.GetNextPrice(basePrice);

//                result.Add(new StockListResponseDto
//                {
//                    Name = s.Name,
//                    Symbol = s.Symbol,
//                    Exchange = s.Exchange,
//                    Country = s.Country,
//                    Currency = s.Currency,
//                    Type = s.Type,
//                    CurrentPrice = newPrice
//                });

//                s.LastPrice = newPrice;
//            }
//            _context.UpdateRange(stocks);
//            await _context.SaveChangesAsync();

//            return result;
//        }

//        public async Task<List<StockSearchResponse>> SearchStock(string SearchPara,int size = 7)
//        {
//            var data = await _context.Set<dynamic>().Where(s => s.Symbol.Contains(SearchPara) || s.Name.Contains(SearchPara)).Take(size).ToListAsync();
//            return data.Select(s => new StockSearchResponse { Name = s.Name, Symbol = s.Symbol }).ToList();
//        }

//        public async Task<StockByNameResponseDto> SearchByName(string name)
//        {
//            var data = await _context.Set<dynamic>().Where(s => s.Name.Contains(name)).FirstOrDefaultAsync() ?? throw new Exception("Stock not found");
//                var basePrice = data.LastPrice > 0 ? data.LastPrice : 100;

//                var newPrice = _stockApi.GetNextPrice(basePrice);
//            StockByNameResponseDto response = new()
//            { 
//                Currency = data.Currency, 
//                Exchange = data.Exchange, 
//                Name = data.Name, 
//                Symbol = data.Symbol, 
//                Type = data.Type, 
//                Country = data.Country, 
//                MicCode = data.MicCode , 
//                UpdateTime  = data.UpdatedAt,
//                LastPrice = data.LastPrice
//            };
//            return response;
//        }
//    }
//}