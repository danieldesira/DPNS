using DPNS.Caching;
using DPNS.Repositories;
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
        private readonly ICacheProvider cacheProvider;
        private readonly ICacheRepository cacheRepository;
        private readonly ISubscriptionRepository subscriptionRepository;

        public SubscriptionController(
            ICacheRepository cacheRepository,
            ICacheProvider cacheProvider,
            ISubscriptionRepository subscriptionRepository
        )
        {
            this.cacheRepository = cacheRepository;
            this.cacheProvider = cacheProvider;
            this.subscriptionRepository = subscriptionRepository;
        }

        [HttpPost]
        public IResult CreateSubscription([FromBody] WebPush.PushSubscription payload)
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

            var subscriptions = this.cacheProvider.Get<List<WebPush.PushSubscription>>("subs");
            if (subscriptions == null)
            {
                subscriptions = [payload];
            }
            else
            {
                subscriptions.Add(payload);
            }
            this.cacheRepository.Set("subs", subscriptions);

            subscriptionRepository.AddSubscription(payload);

            return Results.Ok(new { Message = "Client subscribed successfully!" });
        }
    }
}
