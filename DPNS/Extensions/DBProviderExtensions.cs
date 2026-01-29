using DPNS.Entities;

namespace DPNS.Extensions
{
    public static class DBProviderExtensions
    {
        public static bool IsInMemory(this DpnsDbContext dbContext)
        {
            return dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        }
    }
}
