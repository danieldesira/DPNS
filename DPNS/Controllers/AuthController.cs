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
        public IResult RegisterUser([FromBody] User payload)
        {
            try
            {
                authenticationManager.RegisterUser(payload);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(new { e.Message });
            }

            return Results.Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("verify-email")]
        public IResult VerifyEmail([FromBody] EmailVerificationRequest payload)
        {
            try
            {
                authenticationManager.VerifyEmail(payload.VerificationCode);
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new { e.Message });
            }

            return Results.Ok(new { Message = "Email verified successfully" });
        }

        [HttpPost("login")]
        public IResult Login([FromBody] LoginRequest payload)
        {
            try
            {
                string token = authenticationManager.Login(payload.Email, payload.Password);
                return Results.Ok(new { token });
            }
            catch (InvalidOperationException)
            {
                return Results.Unauthorized();
            }
        }
    }
}
