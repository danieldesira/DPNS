using DPNS.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DPNS.Managers
{
    public interface INotificationManager
    {
        void AddSubscription(string endpoint, string p256dh, string auth);
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
    }
}
