using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        public void AddNotification(string title, string text, string appUrl);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly NeondbContext dbContext;

        public NotificationRepository(NeondbContext dbContext) => this.dbContext = dbContext;

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
