using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private IMemoryCache _cache;

        public SubscriptionController()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddMemoryCache();
            this._cache = builder.Build().Services.GetRequiredService<IMemoryCache>();
        }

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

            var subscriptions = this._cache.Get<List<PushSubscription>>("subscriptions");
            if (subscriptions == null)
            {
                subscriptions = new List<PushSubscription> { payload };
            }
            else
            {
                subscriptions.Add(payload);
            }
            this._cache.Set("subscriptions", subscriptions);

            return Results.Ok(new { message = "Client subscribed successfully!" });
        }
    }
}
