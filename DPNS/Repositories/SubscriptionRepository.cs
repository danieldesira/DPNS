using DPNS.DbModels;
using DPNS.Extensions;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(WebPush.PushSubscription subscription);
        IList<PushSubscription> GetSubscriptions();
    }


    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NeondbContext dbContext;

        public SubscriptionRepository(NeondbContext dbContext) => this.dbContext = dbContext;

        public void AddSubscription(WebPush.PushSubscription subscription)
        {
            dbContext.PushSubscriptions.Add(new PushSubscription
            {
                Auth = subscription.Auth,
                P256dh = subscription.P256DH,
                Endpoint = subscription.Endpoint,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }

        public IList<PushSubscription> GetSubscriptions()
        {
            return [.. dbContext.PushSubscriptions];
        }
    }
}
