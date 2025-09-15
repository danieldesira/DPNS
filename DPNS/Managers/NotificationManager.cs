using DPNS.Repositories;
using Newtonsoft.Json;
using WebPush;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        void AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid);
        void AddNotification(string title, string text, Guid appGuid);
        IList<WebPush.PushSubscription> GetPushSubscriptionList(Guid appGuid);
        void SendNotification(string title, string text, IList<PushSubscription> pushSubscriptions);
    }

    public class NotificationManager : INotificationManager
    {
        private readonly IAppRepository _appRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebPushClient _webPushClient;

        public NotificationManager(
            IAppRepository appRepository,
            ISubscriptionRepository subscriptionRepository,
            INotificationRepository notificationRepository,
            IConfiguration configuration,
            IWebPushClient webPushClient
        )
        {
            _appRepository = appRepository;
            _subscriptionRepository = subscriptionRepository;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _webPushClient = webPushClient;
        }

        public void AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid)
        {
            if (_subscriptionRepository.GetSubscription(endpoint, p256dh, auth) != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            var app = _appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            _subscriptionRepository.AddSubscription(endpoint, p256dh, auth, app.Id);
        }

        public void AddNotification(string title, string text, Guid appGuid)
        {
            var app = _appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            _notificationRepository.AddNotification(title, text, app.Url);
        }

        public IList<WebPush.PushSubscription> GetPushSubscriptionList(Guid appGuid)
        {
            var app = _appRepository.GetApp(appGuid);

            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }

            return [.. _subscriptionRepository.GetSubscriptions(app.Id)
                .Select(s => new WebPush.PushSubscription(s.Endpoint, s.P256dh, s.Auth))];
        }

        public void SendNotification(string title, string text, IList<PushSubscription> pushSubscriptions)
        {
            VapidDetails vapidDetails = new(
                "mailto:desiradaniel2007@gmail.com",
                _configuration["PublicWebPushKey"],
                _configuration["PrivateWebPushKey"]
            );

            string notificationContent = JsonConvert.SerializeObject(new
            {
                title,
                text,
            });
            foreach (var sub in pushSubscriptions)
            {
                _webPushClient.SendNotification(sub, notificationContent, vapidDetails);
            }
        }
    }
}
