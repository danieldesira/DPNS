using DPNS.Entities;
using StackExchange.Redis;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotification(string title, string text, string appUrl, string userEmail);
        Task AddNotificationToCache(string title, string text, string appUrl, string userEmail);
    }

    public class NotificationRepository(DpnsDbContext dbContext) : INotificationRepository
    {
        public async Task AddNotification(string title, string text, string appUrl, string userEmail)
        {
            dbContext.PushNotifications.Add(new PushNotification
            {
                Title = title,
                Text = text,
                AppUrl = appUrl,
                UserEmail = userEmail,
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task AddNotificationToCache(string title, string text, string appUrl, string userEmail)
        {
            var notification = new PushNotification
            {
                Title = title,
                Text = text,
                AppUrl = appUrl,
                UserEmail = userEmail,
                CreatedAt = DateTime.UtcNow,
            };

            var muxer = await ConnectionMultiplexer.ConnectAsync("localhost");
            var db = muxer.GetDatabase();
            await db.ListLeftPushAsync("notifications", System.Text.Json.JsonSerializer.Serialize(notification));
        }
    }
}
