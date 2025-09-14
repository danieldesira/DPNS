using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface INotificationRepository
    {
        public void AddNotification(string title, string text, string appUrl);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly NeondbContext _dbContext;

        public NotificationRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public void AddNotification(string title, string text, string appUrl)
        {
            _dbContext.PushNotifications.Add(new PushNotification
            {
                Title = title,
                Text = text,
                AppUrl = appUrl,
                CreatedAt = DateTime.UtcNow,
            });
            _dbContext.SaveChanges();
        }
    }
}
