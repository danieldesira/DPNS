using DPNS.DbModels;
using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        void AddApp(string appName, string url);
        App GetApp(Guid guid);
    }

    public class AppManager(IAppRepository appRepository) : IAppManager
    {
        public void AddApp(string appName, string url)
        {
            if (appRepository.GetApp(appName, url) != null)
            {
                throw new InvalidOperationException("Project name already exists");
            }

            appRepository.AddApp(appName, url);
        }

        public App GetApp(Guid guid)
        {
            var app = appRepository.GetApp(guid);
            return app ?? throw new InvalidOperationException("App not found");
        }
    }
}
