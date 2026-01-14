using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController(INotificationManager notificationManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IResult> CreateSubscriptionAsync([FromBody] Subscription payload, [FromQuery(Name = "appId")] Guid appId)
        {
            try
            {
                await notificationManager.AddSubscription(
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
