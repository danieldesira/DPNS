using DPNS.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DPNS.Repositories
{
    public interface IAppRepository
    {
        Task AddApp(string name, string url);
        Task<App?> GetApp(Guid guid);
        Task<App?> GetApp(string name, string url);
        IList<App> GetUserApps(int userId);
    }

    public class AppRepository(DpnsDbContext dbContext) : IAppRepository
    {
        public async Task AddApp(string name, string url)
        {
            dbContext.Apps.Add(new App
            {
                AppName = name,
                Url = url,
                Guid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<App?> GetApp(Guid guid)
        {
            return await dbContext.Apps.FirstOrDefaultAsync(p => p.Guid == guid);
        }

        public async Task<App?> GetApp(string name, string url)
        {
            return await dbContext.Apps.FirstOrDefaultAsync(p => p.AppName == name || p.Url == url);
        }

        public IList<App> GetUserApps(int userId)
        {
            return [.. dbContext.AppUsers
                .Where(au => au.UserId == userId)
                .Select(au => au.App)];
        }
    }
}
