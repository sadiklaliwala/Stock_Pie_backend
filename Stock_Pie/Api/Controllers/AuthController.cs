using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Services;
using Stock_Pie.Application.Interfaces;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService auth, IOtpService otp) : ControllerBase
    {
        private readonly IAuthService _auth = auth;
        private readonly IOtpService _otp = otp;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest req)
        {
            // For OTP flow, first call POST /auth/send-otp with email to send OTP
            // Here we support password login or OTP verify
            if (!string.IsNullOrEmpty(req.Password))
            {
                var (access, refresh) = await _auth.LoginAsync(new UserLoginDto { Email = req.Email, Password = req.Password });
                return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh, ExpiresIn = 60 * 15 });
            }

            return BadRequest("Password required for this endpoint. Use /auth/send-otp to request an OTP.");
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest req)
        {
            await _otp.SendOtpAsync(req.Email);
            return Accepted();
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            var ok = await _otp.VerifyOtpAsync(req.Email, req.Otp);
            if (!ok) return Unauthorized();

            // issue JWT (bypass password) using auth service - implement LoginWithEmail or similar
            var (access, refresh) = await _auth.LoginWithEmailAsync(req.Email);
            return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh, ExpiresIn = 60 * 15 });
        }

        // Google OAuth callback endpoint - accept code from client and exchange
        [HttpPost("google-response")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthRequest req)
        {
            var (access, refresh) = await _auth.LoginWithGoogleAsync(req.IdToken);
            return Ok(new AuthResponse { AccessToken = access, RefreshToken = refresh, ExpiresIn = 60 * 15 });
        }
    }
}