using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Api.Controllers
{
    /// <summary>
    /// Wallet endpoints for managing the authenticated user's wallet
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController(IWalletService walletService, IUserContext userContext, IMapper mapper) : ControllerBase
    {
        private readonly IWalletService _walletService = walletService;
        private readonly IUserContext _userContext = userContext;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Get current user's wallet
        /// </summary>
        /// <returns>Wallet summary</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var wallet = await _walletService.GetUserWalletAsync(userId);
            if (wallet == null) return NotFound();
            return Ok(new { wallet.Id, wallet.Balance, wallet.CreatedAt });
        }

        /// <summary>
        /// Create a wallet for the authenticated user
        /// </summary>
        /// <returns>Created wallet info</returns>
        //[HttpPost("create")]
        //public async Task<IActionResult> CreateWallet()
        //{
        //    var userId = _userContext.UserId;
        //    if (userId == Guid.Empty) return Unauthorized();

        //    var wallet = await _walletService.CreateWalletForUserAsync(userId);
        //    return CreatedAtAction(nameof(GetMyWallet), new { }, new { wallet.Id, wallet.Balance });
        //}

        /// <summary>
        /// Add balance to the authenticated user's wallet
        /// </summary>
        /// <param name="req">Amount to add</param>
        /// <returns>Updated wallet</returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddBalance([FromBody] AddBalanceRequest req)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var wallet = await _walletService.AddBalanceAsync(userId, req.Amount);
            return Ok(new { wallet.Id, wallet.Balance });
        }

        /// <summary>
        /// Transfer funds from authenticated user's wallet to another wallet
        /// </summary>
        /// <param name="req">Transfer details</param>
        /// <returns>Sender wallet after transfer</returns>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] WalletTransferRequest req)
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();

            var wallet = await _walletService.WalletToWalletTransferAsync(userId, req.ReceiverWalletId, req.Amount);
            return Ok(new { wallet.Id, wallet.Balance });
        }

        /// <summary>
        /// Find a wallet by user email (for transfers)
        /// </summary>
        [HttpGet("find")]
        public async Task<IActionResult> FindByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email is required");
            var wallet = await _walletService.FindByEmailAsync(email);
            if (wallet == null) return NotFound("No wallet found for that email");
            return Ok(new { wallet.Id, wallet.Balance });
        }
    }
}
