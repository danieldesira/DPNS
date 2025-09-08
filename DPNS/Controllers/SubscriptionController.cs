using DPNS.Caching;
using DPNS.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ICacheRepository _cacheRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionController(
            ICacheRepository cacheRepository,
            ICacheProvider cacheProvider,
            ISubscriptionRepository subscriptionRepository
        )
        {
            _cacheRepository = cacheRepository;
            _cacheProvider = cacheProvider;
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpPost]
        public IResult CreateSubscription([FromBody] PushSubscription payload)
        {
            var subscriptions = _cacheProvider.Get<List<PushSubscription>>("subs");
            if (subscriptions == null)
            {
                subscriptions = [payload];
            }
            else
            {
                subscriptions.Add(payload);
            }
            _cacheRepository.Set("subs", subscriptions);

            _subscriptionRepository.AddSubscription(payload);

            return Results.Ok(new { Message = "Client subscribed successfully!" });
        }
    }
}
