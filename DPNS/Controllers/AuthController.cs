using DPNS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IResult RegisterUser([FromBody] User payload)
        {


            return Results.Ok(new { Message = "User registered successfully" });
        }
    }
}
