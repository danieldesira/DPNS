using DPNS.Entities;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(string endpoint, string p256dh, string auth, int appId);
        IList<PushSubscription> GetSubscriptions(int appId);
        PushSubscription? GetSubscription(string endpoint, string p256dh, string auth);
    }


    public class SubscriptionRepository(DpnsDbContext dbContext) : ISubscriptionRepository
    {
        public void AddSubscription(string endpoint, string p256dh, string auth, int appId)
        {
            dbContext.PushSubscriptions.Add(new PushSubscription
            {
                Auth = auth,
                P256dh = p256dh,
                Endpoint = endpoint,
                AppId = appId,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }

        public IList<PushSubscription> GetSubscriptions(int appId)
        {
            return [.. dbContext.PushSubscriptions.Where(s => s.AppId == appId)];
        }

        public PushSubscription? GetSubscription(string endpoint, string p256dh, string auth)
        {
            return dbContext.PushSubscriptions
                    .FirstOrDefault(s => s.Endpoint == endpoint && s.P256dh == p256dh && s.Auth == auth);
        }
    }
}
