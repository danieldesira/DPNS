using DPNS.DbModels;
using System.ComponentModel;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(string endpoint, string p256dh, string auth, int appId);
        IList<PushSubscription> GetSubscriptions(int appId);
        PushSubscription? GetSubscription(string endpoint, string p256dh, string auth);
    }


    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NeondbContext _dbContext;

        public SubscriptionRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public void AddSubscription(string endpoint, string p256dh, string auth, int appId)
        {
            _dbContext.PushSubscriptions.Add(new PushSubscription
            {
                Auth = auth,
                P256dh = p256dh,
                Endpoint = endpoint,
                AppId = appId,
                CreatedAt = DateTime.UtcNow,
            });
            _dbContext.SaveChanges();
        }

        public IList<PushSubscription> GetSubscriptions(int appId)
        {
            return [.. _dbContext.PushSubscriptions.Where(s => s.AppId == appId)];
        }

        public PushSubscription? GetSubscription(string endpoint, string p256dh, string auth)
        {
            return _dbContext.PushSubscriptions
                    .FirstOrDefault(s => s.Endpoint == endpoint && s.P256dh == p256dh && s.Auth == auth);
        }
    }
}
