using DPNS.Entities;
using Microsoft.EntityFrameworkCore;

namespace DPNS.Repositories
{
    public interface IAppRepository
    {
        Task AddApp(string name, string url, int userId);
        Task<App?> GetApp(Guid guid);
        Task<App?> GetApp(string name, string url);
        Task<IList<App>> GetUserApps(int userId);
        Task AddAppUser(int appId, int userId);
        Task RemoveAppUser(int appId, int userId);
        Task<bool> IsUserAppAdmin(int appId, int userId);
        Task<bool> ExistAppUserLink(int appId, int userId);
    }

    public class AppRepository(DpnsDbContext dbContext) : IAppRepository
    {
        public async Task AddApp(string name, string url, int userId)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

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

        public async Task AddAppUser(int appId, int userId)
        {
            dbContext.AppUsers.Add(new AppUser
            {
                AppId = appId,
                UserId = userId,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveAppUser(int appId, int userId)
        {
            var appUser = await dbContext.AppUsers.FirstOrDefaultAsync(au => au.AppId == appId && au.UserId == userId);
            if (appUser != null)
            {
                dbContext.AppUsers.Remove(appUser);
                await dbContext.SaveChangesAsync();
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

        public async Task<bool> IsUserAppAdmin(int appId, int userId)
        {
            return await dbContext.AppUsers.AnyAsync(au => au.AppId == appId && au.UserId == userId && au.IsAdmin);
        }

        public async Task<bool> ExistAppUserLink(int appId, int userId)
        {
            return await dbContext.AppUsers.AnyAsync(au => au.AppId == appId && au.UserId == userId);
        }
    }
}
