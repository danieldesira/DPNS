using DPNS.Caching;
using DPNS.Managers;
using DPNS.Models;
using DPNS.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ICacheRepository _cacheRepository;
        private readonly INotificationManager _notificationManager;

        public SubscriptionController(
            ICacheRepository cacheRepository,
            ICacheProvider cacheProvider,
            INotificationManager notificationManager
        )
        {
            _cacheRepository = cacheRepository;
            _cacheProvider = cacheProvider;
            _notificationManager = notificationManager;
        }

        [HttpPost]
        public IResult CreateSubscription([FromBody] Subscription payload)
        {
            var subscriptions = _cacheProvider.Get<List<Subscription>>("subs");
            if (subscriptions == null)
            {
                subscriptions = [payload];
            }
            else
            {
                subscriptions.Add(payload);
            }
            _cacheRepository.Set("subs", subscriptions);

            try
            {
                _notificationManager.AddSubscription(
                    payload.Endpoint,
                    payload.Keys.P256dh,
                    payload.Keys.Auth
                );
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(new { Message = e.Message });
            }

            return Results.Ok(new { Message = "Client subscribed successfully!" });
        }
    }
}
