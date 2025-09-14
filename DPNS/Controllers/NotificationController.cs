using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IAppManager _appManager;
        private readonly INotificationManager _notificationManager;
        private readonly IConfiguration _configuration;

        public NotificationController(
            IAppManager appManager,
            INotificationManager notificationManager,
            IConfiguration configuration
        )
        {
            _appManager = appManager;
            _notificationManager = notificationManager;
            _configuration = configuration;
        }

        [HttpPost]
        public IResult SendNotification([FromBody] Notification payload, [FromQuery(Name = "appId")] Guid appId)
        {
            _notificationManager.AddNotification(payload.Title, payload.Text, appId);

            WebPushClient webPushClient = new();
            VapidDetails vapidDetails = new(
                "mailto:desiradaniel2007@gmail.com",
                _configuration["PublicWebPushKey"],
                _configuration["PrivateWebPushKey"]
            );
            
            string notificationContent = JsonConvert.SerializeObject(payload);
            foreach (var sub in _notificationManager.GetPushSubscriptionList(appId))
            {
                webPushClient.SendNotification(sub, notificationContent, vapidDetails);
            }

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
