using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationManager _notificationManager;

        public NotificationController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        [HttpPost, Authorize]
        public IResult SendNotification([FromBody] Notification payload, [FromQuery(Name = "appId")] Guid appId)
        {
            _notificationManager.AddNotification(payload.Title, payload.Text, appId);

            var subscriptionList = _notificationManager.GetPushSubscriptionList(appId);
            _notificationManager.SendNotification(payload.Title, payload.Text, subscriptionList);

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
