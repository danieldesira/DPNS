using DPNS.PayloadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        [HttpPost]
        public IResult CreateSubscription([FromBody] PushSubscription payload)
        {
            var builder = WebApplication.CreateBuilder();
            string? privateKey = builder.Configuration["PrivateWebPushKey"];
            string? publicKey = builder.Configuration["PublicWebPushKey"];
            
            if (privateKey == null || publicKey == null)
            {
                return Results.BadRequest("Configuration incomplete");
            }

            if (privateKey != payload.Auth)
            {
                return Results.BadRequest("Private key does not match!");
            }

            if (publicKey != payload.P256DH)
            {
                return Results.BadRequest("Public key does not match!");
            }

            return Results.Ok(new { message = "Client subscribed successfully!" });
        }
    }
}
