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
        private readonly INotificationManager _notificationManager;

        public SubscriptionController(INotificationManager notificationManager) => _notificationManager = notificationManager;

        [HttpPost]
        public IResult CreateSubscription([FromBody] Subscription payload)
        {
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
                return Results.Conflict(new { e.Message });
            }

            return Results.Ok(new { Message = "Client subscribed successfully!" });
        }
    }
}
