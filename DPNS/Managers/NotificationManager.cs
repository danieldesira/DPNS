using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        void AddSubscription(string endpoint, string p256dh, string auth);
        void AddNotification(string title, string text);
        IList<WebPush.PushSubscription> GetPushSubscriptionList();
    }

    public class NotificationManager : INotificationManager
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationRepository _notificationRepository;

        public NotificationManager(
            ISubscriptionRepository subscriptionRepository,
            INotificationRepository notificationRepository
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _notificationRepository = notificationRepository;
        }

        public void AddSubscription(string endpoint, string p256dh, string auth)
        {
            if (_subscriptionRepository.GetSubscription(endpoint, p256dh, auth) != null)
            {
                throw new InvalidOperationException("Subscription already exists");
            }

            _subscriptionRepository.AddSubscription(endpoint, p256dh, auth);
        }

        public void AddNotification(string title, string text)
        {
            _notificationRepository.AddNotification(title, text);
        }

        public IList<WebPush.PushSubscription> GetPushSubscriptionList()
        {
            return [.. _subscriptionRepository.GetSubscriptions()
                .Select(s => new WebPush.PushSubscription(s.Endpoint, s.P256dh, s.Auth))];
        }
    }
}
