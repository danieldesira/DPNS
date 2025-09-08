using DPNS.Extensions;
using DPNS.Models;
using DPNS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IConfiguration _configuration;

        public NotificationController(
            INotificationRepository notificationRepository,
            ISubscriptionRepository subscriptionRepository,
            IConfiguration configuration
        )
        {
            _notificationRepository = notificationRepository;
            _subscriptionRepository = subscriptionRepository;
            _configuration = configuration;
        }

        [HttpPost]
        public IResult SendNotification([FromBody] Notification payload)
        {
            var subscriptions = _subscriptionRepository.GetSubscriptions()
                .Select(s => new PushSubscription(s.Endpoint, s.P256dh, s.Auth));

            string vapidPublicKey = _configuration["PublicWebPushKey"];
            string vapidPrivateKey = _configuration["PrivateWebPushKey"];

            WebPushClient webPushClient = new();
            VapidDetails vapidDetails = new("mailto:desiradaniel2007@gmail.com", vapidPublicKey, vapidPrivateKey);

            string notificationContent = JsonConvert.SerializeObject(payload);

            foreach (var sub in subscriptions)
            {
                webPushClient.SendNotification(sub, notificationContent, vapidDetails);
            }

            _notificationRepository.AddNotification(payload.Title, payload.Text);

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
