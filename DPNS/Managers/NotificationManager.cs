using DPNS.Repositories;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        Task AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid);
        Task AddNotification(string title, string text, Guid appGuid, int currentUserId);
        Task<IEnumerable<PushSubscription>> GetPushSubscriptionList(Guid appGuid);
        void SendNotification(string title, string text, IEnumerable<PushSubscription> pushSubscriptions);
        Task DeleteSubscription(string endpoint);
    }

    public class NotificationManager(
        IAppRepository appRepository,
        ISubscriptionRepository subscriptionRepository,
        INotificationRepository notificationRepository,
        IConfiguration configuration,
        IWebPushClient webPushClient,
        IUserRepository userRepository
    ) : INotificationManager
    {
        public async Task AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid)
        {
            if (await subscriptionRepository.GetSubscription(endpoint, p256dh, auth) != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            var app = await appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            await subscriptionRepository.AddSubscription(endpoint, p256dh, auth, app.Id);
        }

        public async Task AddNotification(string title, string text, Guid appGuid, int currentUserId)
        {
            var app = await appRepository.GetApp(appGuid) ?? throw new InvalidOperationException("App not found");

            if (!await appRepository.ExistAppUserLink(app.Id, currentUserId))
            {
                throw new InvalidOperationException("User does not have access permission for this app");
            }

            var user = await userRepository.GetUser(currentUserId) ?? throw new InvalidOperationException("User not found");

            await notificationRepository.AddNotification(title, text, app.Url, user.Email);
            await notificationRepository.AddNotificationToCache(title, text, app.Url, user.Email);
        }

        public async Task<IEnumerable<PushSubscription>> GetPushSubscriptionList(Guid appGuid)
        {
            var app = await appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            return [.. (await subscriptionRepository.GetSubscriptions(app.Id))
                .Select(s => new PushSubscription(s.Endpoint, s.P256dh, s.Auth))];
        }

        public void SendNotification(string title, string text, IEnumerable<PushSubscription> pushSubscriptions)
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

        public async Task DeleteSubscription(string endpoint)
        {
            var subscription = await subscriptionRepository.GetSubscriptionByEndpoint(endpoint) ?? throw new InvalidOperationException("Subscription not found");
            await subscriptionRepository.DeleteSubscription(subscription.Id);
        }
    }
}
