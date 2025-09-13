using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        void AddSubscription(string endpoint, string p256dh, string auth, Guid appGuid);
        void AddNotification(string title, string text, Guid appGuid);
        IList<WebPush.PushSubscription> GetPushSubscriptionList(Guid appGuid);
    }

    public class NotificationManager : INotificationManager
    {
        private readonly IAppRepository _appRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationRepository _notificationRepository;

        public NotificationManager(
            IAppRepository appRepository,
            ISubscriptionRepository subscriptionRepository,
            INotificationRepository notificationRepository
        )
        {
            _appRepository = appRepository;
            _subscriptionRepository = subscriptionRepository;
            _notificationRepository = notificationRepository;
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
    }
}
