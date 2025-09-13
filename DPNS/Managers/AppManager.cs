using DPNS.DbModels;
using DPNS.Repositories;
using WebPush;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        public void AddApp(string appName, string url);
        public App GetApp(Guid guid);
    }

    public class AppManager : IAppManager
    {
        private IAppRepository _appRepository;

        public AppManager(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public void AddApp(string appName, string url)
        {
            if (_appRepository.GetApp(appName, url) != null)
            {
                throw new InvalidOperationException("Project name already exists");
            }

            VapidDetails vapidDetails = VapidHelper.GenerateVapidKeys();

            _appRepository.AddApp(appName, url, vapidDetails.PublicKey, vapidDetails.PrivateKey);
        }

        public App GetApp(Guid guid)
        {
            var app = _appRepository.GetApp(guid);
            if (app == null)
            {
                throw new InvalidOperationException("App not found");
            }
            return app;
        }
    }
}
