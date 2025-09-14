using DPNS.DbModels;
using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IAppManager
    {
        void AddApp(string appName, string url);
        App GetApp(Guid guid);
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

            _appRepository.AddApp(appName, url);
        }

        public App GetApp(Guid guid)
        {
            var app = _appRepository.GetApp(guid);
            return app ?? throw new InvalidOperationException("App not found");
        }
    }
}
