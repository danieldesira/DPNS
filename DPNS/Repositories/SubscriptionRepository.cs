using DPNS.DbModels;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Org.BouncyCastle.Security;
using WebPush;

namespace DPNS.Repositories
{
    public interface ISubscriptionRepository
    {
        void AddSubscription(WebPush.PushSubscription subscription);
        IList<DbModels.PushSubscription> GetSubscriptions();
    }


    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly NeondbContext dbContext;

        public SubscriptionRepository(NeondbContext dbContext) => this.dbContext = dbContext;

        public void AddSubscription(WebPush.PushSubscription subscription)
        {
            dbContext.PushSubscriptions.Add(new DbModels.PushSubscription
            {
                Auth = ToStandardBase64(subscription.Auth),
                P256dh = ToStandardBase64(subscription.P256DH),
                Endpoint = subscription.Endpoint,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }

        public IList<DbModels.PushSubscription> GetSubscriptions()
        {
            return [.. dbContext.PushSubscriptions];
        }

        string ToStandardBase64(string input)
        {
            string output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            return output;
        }
    }
}
