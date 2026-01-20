using DPNS.Entities;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotification(string title, string text, string appUrl, string userEmail);
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
    }
}
