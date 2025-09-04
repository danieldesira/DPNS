using DPNS.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private ICacheProvider cacheProvider;
        private ICacheRepository cacheRepository;

        public SubscriptionController(ICacheRepository cacheRepository, ICacheProvider cacheProvider)
        {
            this.cacheRepository = cacheRepository;
            this.cacheProvider = cacheProvider;
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

            var subscriptions = this.cacheProvider.Get<List<PushSubscription>>("subs");
            if (subscriptions == null)
            {
                subscriptions = [payload];
            }
            else
            {
                subscriptions.Add(payload);
            }
            this.cacheRepository.Set<List<PushSubscription>>("subs", subscriptions);

            return Results.Ok(new { message = "Client subscribed successfully!" });
        }
    }
}
