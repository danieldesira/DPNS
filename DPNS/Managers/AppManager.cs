using DPNS.Entities;
using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        Task AddApp(string appName, string url);
        Task<App> GetApp(Guid guid);
        Task<IList<App>> GetUserApps(int userId);
    }

    public class AppManager(IAppRepository appRepository) : IAppManager
    {
        public async Task AddApp(string appName, string url)
        {
            if (await appRepository.GetApp(appName, url) != null)
            {
                throw new InvalidOperationException("Project name already exists");
            }

            await appRepository.AddApp(appName, url);
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
    }
}
