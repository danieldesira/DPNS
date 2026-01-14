using DPNS.Entities;
using Microsoft.EntityFrameworkCore;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        Task AddSubscription(string endpoint, string p256dh, string auth, int appId);
        Task<IList<PushSubscription>> GetSubscriptions(int appId);
        Task<PushSubscription?> GetSubscription(string endpoint, string p256dh, string auth);
    }


    public class SubscriptionRepository(DpnsDbContext dbContext) : ISubscriptionRepository
    {
        public async Task AddSubscription(string endpoint, string p256dh, string auth, int appId)
        {
            dbContext.PushSubscriptions.Add(new PushSubscription
            {
                Auth = auth,
                P256dh = p256dh,
                Endpoint = endpoint,
                AppId = appId,
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<IList<PushSubscription>> GetSubscriptions(int appId)
        {
            return await dbContext.PushSubscriptions.Where(s => s.AppId == appId).ToListAsync();
        }

        public async Task<PushSubscription?> GetSubscription(string endpoint, string p256dh, string auth)
        {
            return await dbContext.PushSubscriptions
                    .FirstOrDefaultAsync(s => s.Endpoint == endpoint && s.P256dh == p256dh && s.Auth == auth);
        }
    }
}
