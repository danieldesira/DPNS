using DPNS.Entities;
using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        Task AddApp(string appName, string url, int userId);
        Task<App> GetApp(Guid guid);
        Task<IList<App>> GetUserApps(int userId);
        Task<int> GetSubscriptionCount(int userId);
    }

    public class AppManager(IAppRepository appRepository, ISubscriptionRepository subscriptionRepository) : IAppManager
    {
        public async Task AddApp(string appName, string url, int userId)
        {
            if (await appRepository.GetApp(appName, url) != null)
            {
                throw new InvalidOperationException("Project name already exists");
            }

            await appRepository.AddApp(appName, url, userId);
        }

        public async Task<App> GetApp(Guid guid)
        {
            var app = await appRepository.GetApp(guid);
            return app ?? throw new InvalidOperationException("App not found");
        }

        public async Task<IList<App>> GetUserApps(int userId)
        {
            return await appRepository.GetUserApps(userId);
        }

        public async Task<int> GetSubscriptionCount(int appId)
        {
            var apps = await subscriptionRepository.GetSubscriptions(appId);
            return apps.Count;
        }
    }
}
