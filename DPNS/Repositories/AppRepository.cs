using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface IAppRepository
    {
        void AddApp(string name, string url, string publicKey, string privateKey);
        App? GetApp(Guid guid);
        App? GetApp(string name, string url);
    }

    public class AppRepository : IAppRepository
    {
        private readonly NeondbContext _dbContext;

        public AppRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public void AddApp(string name, string url, string publicKey, string privateKey)
        {
            _dbContext.Apps.Add(new App
            {
                AppName = name,
                Url = url,
                PublicKey = publicKey,
                PrivateKey = privateKey,
                Guid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            });
            _dbContext.SaveChanges();
        }

        public App? GetApp(Guid guid)
        {
            return _dbContext.Apps.FirstOrDefault(p => p.Guid == guid);
        }

        public App? GetApp(string name, string url)
        {
            return _dbContext.Apps.FirstOrDefault(p => p.AppName == name || p.Url == url);
        }
    }
}
