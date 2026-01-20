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
        Task AddAppUser(Guid guid, string email, int currentUserId);
    }

    public class AppManager(
        IAppRepository appRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository
    ) : IAppManager
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

        public async Task AddAppUser(Guid guid, string email, int currentUserId)
        {
            var app = await appRepository.GetApp(guid) ?? throw new InvalidOperationException("App not found");
            var user = await userRepository.GetUser(email) ?? throw new InvalidOperationException("User not found");
            if (user.VerifiedAt == null)
            {
                throw new InvalidOperationException("User is not verified");
            }
            if (user.Id == currentUserId)
            {
                throw new InvalidOperationException("Cannot add yourself to the app");
            }
            if (!await appRepository.IsUserAppAdmin(app.Id, currentUserId)) 
            {
                throw new InvalidOperationException("User is not an admin of the app");
            }
            if (await appRepository.ExistAppUserLink(app.Id, user.Id))
            {
                throw new InvalidOperationException("User is already added to the app");
            }
            await appRepository.AddAppUser(app.Id, user.Id);
        }
    }
}
