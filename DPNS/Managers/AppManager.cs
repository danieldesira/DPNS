using DPNS.Entities;
using DPNS.Repositories;
using System.Security.Cryptography.X509Certificates;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        Task AddApp(string appName, string url, int userId);
        Task<App> GetApp(Guid appGuid);
        Task<IList<App>> GetUserApps(int userId);
        Task<int> GetSubscriptionCount(Guid appGuid, int userId);
        Task AddAppUser(Guid appGuid, string email, int currentUserId);
        Task RemoveAppUser(Guid appGuid, string email, int currentUserId);
        Task DeleteApp(Guid appGuid, int currentUserId);
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

        public async Task<App> GetApp(Guid appGuid)
        {
            var app = await appRepository.GetApp(appGuid);
            return app ?? throw new InvalidOperationException("App not found");
        }

        public async Task<IList<App>> GetUserApps(int userId)
        {
            return await appRepository.GetUserApps(userId);
        }

        public async Task<int> GetSubscriptionCount(Guid appGuid, int userId)
        {
            var app = await appRepository.GetApp(appGuid) ?? throw new InvalidOperationException("App not found");

            if (!await appRepository.ExistAppUserLink(app.Id, userId))
            {
                throw new InvalidOperationException("User does not have access to this app");
            }

            var subscriptions = await subscriptionRepository.GetSubscriptions(app.Id);
            return subscriptions.Count;
        }

        public async Task AddAppUser(Guid appGuid, string email, int currentUserId)
        {
            var app = await appRepository.GetApp(appGuid) ?? throw new InvalidOperationException("App not found");
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

        public async Task RemoveAppUser(Guid appGuid, string email, int currentUserId)
        {
            var app = await appRepository.GetApp(appGuid) ?? throw new InvalidOperationException("App not found");
            var user = await userRepository.GetUser(email) ?? throw new InvalidOperationException("User not found");
            if (!await appRepository.IsUserAppAdmin(app.Id, currentUserId))
            {
                throw new InvalidOperationException("User is not an admin of the app");
            }
            if (!await appRepository.ExistAppUserLink(app.Id, user.Id))
            {
                throw new InvalidOperationException("User is not added to the app");
            }
            if (currentUserId == user.Id)
            {
                throw new InvalidOperationException("You may not remove yourself");
            }
            await appRepository.RemoveAppUser(app.Id, user.Id);
        }

        public async Task DeleteApp(Guid appGuid, int currentUserId)
        {
            var app = await appRepository.GetApp(appGuid) ?? throw new InvalidOperationException("App not found");
            if (!await appRepository.IsUserAppAdmin(app.Id, currentUserId))
            {
                throw new InvalidOperationException("User is not an admin of the app");
            }
            await appRepository.DeleteApp(app.Id);
        }
    }
}
