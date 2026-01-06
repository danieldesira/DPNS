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
        public IResult SendNotification([FromBody] Notification payload, [FromQuery(Name = "appId")] Guid appId)
        {
            notificationManager.AddNotification(payload.Title, payload.Text, appId);

            var subscriptionList = notificationManager.GetPushSubscriptionList(appId);
            notificationManager.SendNotification(payload.Title, payload.Text, subscriptionList);

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
