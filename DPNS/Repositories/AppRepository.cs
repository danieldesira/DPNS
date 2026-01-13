using DPNS.Entities;

namespace DPNS.Repositories
{
    public interface IAppRepository
    {
        void AddApp(string name, string url);
        App? GetApp(Guid guid);
        App? GetApp(string name, string url);
    }

    public class AppRepository(DpnsDbContext dbContext) : IAppRepository
    {
        public void AddApp(string name, string url)
        {
            dbContext.Apps.Add(new App
            {
                AppName = name,
                Url = url,
                Guid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
        }

        public App? GetApp(Guid guid)
        {
            return dbContext.Apps.FirstOrDefault(p => p.Guid == guid);
        }

        public App? GetApp(string name, string url)
        {
            return dbContext.Apps.FirstOrDefault(p => p.AppName == name || p.Url == url);
        }
    }
}
