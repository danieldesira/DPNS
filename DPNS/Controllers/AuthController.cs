using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserManager _authenticationManager;

        public AuthController(IUserManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        [HttpPost("register")]
        public IResult RegisterUser([FromBody] User payload)
        {
            try
            {
                _authenticationManager.RegisterUser(payload);
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
                _authenticationManager.VerifyEmail(payload.VerificationCode);
            }
            catch (InvalidOperationException e)
            {
                return Results.BadRequest(new { e.Message });
            }

            return Results.Ok(new { Message = "Email verified successfully" });
        }
    }
}
