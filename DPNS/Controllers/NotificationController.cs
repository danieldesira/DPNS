using DPNS.Extensions;
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
            try
            {
                await notificationManager.AddNotification(payload.Title, payload.Text, appId, User.GetUserId() ?? 0);

                var subscriptionList = await notificationManager.GetPushSubscriptionList(appId);
                notificationManager.SendNotification(payload.Title, payload.Text, subscriptionList);

                return Results.Ok(new { Message = "Notification sent successfully" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}
