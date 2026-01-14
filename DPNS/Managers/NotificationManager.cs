using DPNS.Repositories;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        void AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid);
        Task AddNotificationAsync(string title, string text, Guid appGuid);
        IList<PushSubscription> GetPushSubscriptionList(Guid appGuid);
        void SendNotification(string title, string text, IList<PushSubscription> pushSubscriptions);
    }

    public class NotificationManager(
        IAppRepository appRepository,
        ISubscriptionRepository subscriptionRepository,
        INotificationRepository notificationRepository,
        IConfiguration configuration,
        IWebPushClient webPushClient
    ) : INotificationManager
    {
        public void AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid)
        {
            if (subscriptionRepository.GetSubscription(endpoint, p256dh, auth) != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            var app = appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            subscriptionRepository.AddSubscription(endpoint, p256dh, auth, app.Id);
        }

        public async Task AddNotificationAsync(string title, string text, Guid appGuid)
        {
            var app = await appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            await notificationRepository.AddNotification(title, text, app.Url);
        }

        public IList<PushSubscription> GetPushSubscriptionList(Guid appGuid)
        {
            var app = appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            return [.. subscriptionRepository.GetSubscriptions(app.Id)
                .Select(s => new PushSubscription(s.Endpoint, s.P256dh, s.Auth))];
        }

        public void SendNotification(string title, string text, IList<PushSubscription> pushSubscriptions)
        {
            VapidDetails vapidDetails = new(
                "mailto:desiradaniel2007@gmail.com",
                configuration["PublicWebPushKey"],
                configuration["PrivateWebPushKey"]
            );

            string notificationContent = JsonConvert.SerializeObject(new
            {
                title,
                body = text,
            });
            foreach (var sub in pushSubscriptions)
            {
                webPushClient.SendNotification(sub, notificationContent, vapidDetails);
            }
        }
    }
}
