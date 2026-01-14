using DPNS.Entities;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotification(string title, string text, string appUrl);
    }

    public class NotificationRepository(DpnsDbContext dbContext) : INotificationRepository
    {
        public async Task AddNotification(string title, string text, string appUrl)
        {
            dbContext.PushNotifications.Add(new PushNotification
            {
                Title = title,
                Text = text,
                AppUrl = appUrl,
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }
    }
}
