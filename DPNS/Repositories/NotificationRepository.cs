using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        public void AddNotification(string title, string text, string appUrl);
    }

    public class NotificationRepository(NeondbContext dbContext) : INotificationRepository
    {
        public void AddNotification(string title, string text, string appUrl)
        {
            dbContext.PushNotifications.Add(new PushNotification
            {
                Title = title,
                Text = text,
                AppUrl = appUrl,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }
    }
}
