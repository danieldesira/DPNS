using DPNS.Models;
using DPNS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository notificationRepository;
        private readonly ISubscriptionRepository subscriptionRepository;

        public NotificationController(INotificationRepository notificationRepository, ISubscriptionRepository subscriptionRepository)
        {
            this.notificationRepository = notificationRepository;
            this.subscriptionRepository = subscriptionRepository;
        }

        [HttpPost]
        public IResult SendNotification([FromBody] Notification payload)
        {
            var subscriptions = subscriptionRepository.GetSubscriptions().Select(s => new WebPush.PushSubscription
            {
                Endpoint = s.Endpoint,
                Auth = s.Auth,
                P256DH = s.P256dh,
            });

            WebPush.WebPushClient webPushClient = new WebPush.WebPushClient();
            foreach (var sub in subscriptions)
            {
                webPushClient.SendNotification(sub, payload.Text);
            }

            notificationRepository.AddNotification(payload.Title, payload.Text);

            return Results.Ok(new { Message = "Notification sent successfully" });
        }
    }
}
