using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        public void AddNotification(string title, string text);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly NeondbContext dbContext;

        public NotificationRepository(NeondbContext dbContext) => this.dbContext = dbContext;

        public void AddNotification(string title, string text)
        {
            dbContext.PushNotifications.Add(new PushNotification
            {
                Title = title,
                Text = text,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }
    }
}
