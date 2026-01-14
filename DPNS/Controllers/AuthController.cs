using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserManager authenticationManager) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IResult> RegisterUser([FromBody] User payload)
        {
            try
            {
                await authenticationManager.RegisterUser(payload);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(new { e.Message });
            }

            return Results.Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("verify-email")]
        public async Task<IResult> VerifyEmail([FromBody] EmailVerificationRequest payload)
        {
            try
            {
                await authenticationManager.VerifyEmail(payload.VerificationCode);
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new { e.Message });
            }

            return Results.Ok(new { Message = "Email verified successfully" });
        }

        [HttpPost("login")]
        public async Task<IResult> Login([FromBody] LoginRequest payload)
        {
            try
            {
                string token = await authenticationManager.Login(payload.Email, payload.Password);
                return Results.Ok(new { token });
            }
            catch (InvalidOperationException)
            {
                return Results.Unauthorized();
            }
        }
    }
}
