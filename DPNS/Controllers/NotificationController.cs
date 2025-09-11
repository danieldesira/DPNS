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
        private readonly IConfiguration _configuration;
        private readonly INotificationManager _notificationManager;

        public NotificationController(
            IConfiguration configuration,
            INotificationManager notificationManager
        )
        {
            _configuration = configuration;
            _notificationManager = notificationManager;
        }

        [HttpPost]
        public IResult SendNotification([FromBody] Notification payload)
        {
            _notificationManager.AddNotification(payload.Title, payload.Text);

            string vapidPublicKey = _configuration["PublicWebPushKey"];
            string vapidPrivateKey = _configuration["PrivateWebPushKey"];

            WebPushClient webPushClient = new();
            VapidDetails vapidDetails = new("mailto:desiradaniel2007@gmail.com", vapidPublicKey, vapidPrivateKey);
            
            string notificationContent = JsonConvert.SerializeObject(payload);
            foreach (var sub in _notificationManager.GetPushSubscriptionList())
            {
                webPushClient.SendNotification(sub, notificationContent, vapidDetails);
            }

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
