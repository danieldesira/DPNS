using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(string endpoint, string p256dh, string auth);
        IList<PushSubscription> GetSubscriptions();
        PushSubscription? GetSubscription(string endpoint, string p256dh, string auth);
    }


    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NeondbContext dbContext;

        public SubscriptionRepository(NeondbContext dbContext) => this.dbContext = dbContext;

        public void AddSubscription(string endpoint, string p256dh, string auth)
        {
            dbContext.PushSubscriptions.Add(new PushSubscription
            {
                Auth = auth,
                P256dh = p256dh,
                Endpoint = endpoint,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }

        public IList<PushSubscription> GetSubscriptions()
        {
            return [.. dbContext.PushSubscriptions];
        }

        public PushSubscription? GetSubscription(string endpoint, string p256dh, string auth)
        {
            return dbContext.PushSubscriptions
                    .FirstOrDefault(s => s.Endpoint == endpoint && s.P256dh == p256dh && s.Auth == auth);
        }
    }
}
