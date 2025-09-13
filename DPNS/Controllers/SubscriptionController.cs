using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IAppManager _appManager;
        private readonly INotificationManager _notificationManager;

        public SubscriptionController(IAppManager appManager, INotificationManager notificationManager)
        {
            _appManager = appManager;
            _notificationManager = notificationManager;
        }

        [HttpPost]
        public IResult CreateSubscription([FromBody] Subscription payload, [FromQuery(Name = "appId")] Guid appId)
        {
            try
            {
                _notificationManager.AddSubscription(
                    payload.Endpoint,
                    payload.Keys.P256dh,
                    payload.Keys.Auth,
                    appId
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
