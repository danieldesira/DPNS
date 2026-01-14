using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(INotificationManager notificationManager) : ControllerBase
    {
        [HttpPost, Authorize]
        public async Task<IResult> SendNotification([FromBody] Notification payload, [FromQuery(Name = "appId")] Guid appId)
        {
            await notificationManager.AddNotification(payload.Title, payload.Text, appId);

            var subscriptionList = await notificationManager.GetPushSubscriptionList(appId);
            notificationManager.SendNotification(payload.Title, payload.Text, subscriptionList);

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
