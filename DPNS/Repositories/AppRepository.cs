using DPNS.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Transactions;

namespace DPNS.Repositories
{
    public interface IAppRepository
    {
        Task AddApp(string name, string url, int userId);
        Task<App?> GetApp(Guid guid);
        Task<App?> GetApp(string name, string url);
        Task<IList<App>> GetUserApps(int userId);
    }

    public class AppRepository(DpnsDbContext dbContext) : IAppRepository
    {
        public async Task AddApp(string name, string url, int userId)
        {
            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                var app = dbContext.Apps.Add(new App
                {
                    AppName = name,
                    Url = url,
                    Guid = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                });
                await dbContext.SaveChangesAsync();

                dbContext.AppUsers.Add(new AppUser
                {
                    AppId = app.Entity.Id,
                    UserId = userId,
                    IsAdmin = true,
                    CreatedAt = DateTime.UtcNow,
                });
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
        }

        public async Task<App?> GetApp(Guid guid)
        {
            return await dbContext.Apps.FirstOrDefaultAsync(p => p.Guid == guid);
        }

        public async Task<App?> GetApp(string name, string url)
        {
            return await dbContext.Apps.FirstOrDefaultAsync(p => p.AppName == name || p.Url == url);
        }

        public async Task<IList<App>> GetUserApps(int userId)
        {
            return await dbContext
                            .AppUsers
                            .Where(au => au.UserId == userId)
                            .Select(au => au.App)
                            .ToListAsync();
        }
    }
}
