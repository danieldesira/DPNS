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
        public async Task<IResult> CreateSubscription([FromBody] Subscription payload, [FromQuery(Name = "appId")] Guid appId)
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

        [HttpDelete]
        public async Task<IResult> DeleteSubscription([FromBody] RemoveSubscriptionRequest payload)
        {
            try
            {
                await notificationManager.DeleteSubscription(payload.Endpoint);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(new { e.Message });
            }
            return Results.Ok(new { Message = "Client unsubscribed successfully!" });
        }
    }
}
