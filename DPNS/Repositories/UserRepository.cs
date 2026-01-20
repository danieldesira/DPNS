using DPNS.Entities;
using Microsoft.EntityFrameworkCore;

namespace DPNS.Repositories
{
    public interface IUserRepository
    {
        Task AddUser(string name, string email, string password);
        Task<User?> GetUser(string email);
        Task<UserVerificationToken?> GetUserVerificationToken(string token);
        Task VerifyEmail(int userId);
        Task DeleteVerificationToken(int userId);
    }

    public class UserRepository(DpnsDbContext dbContext) : IUserRepository
    {
        public async Task AddUser(string name, string email, string password)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            var userEntry = dbContext.Users.Add(new User
            {
                FullName = name,
                Email = email,
                HashedPassword = password,
                CreatedAt = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync();
            
            await CreateVerificationToken(userEntry.Entity.Id);

            await transaction.CommitAsync();
        }

        public async Task<User?> GetUser(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        private async Task CreateVerificationToken(int userId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            dbContext.UserVerificationTokens.Add(new UserVerificationToken
            {
                UserId = userId,
                VerificationCode = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
            await dbContext.SaveChangesAsync();
        }

        public async Task<UserVerificationToken?> GetUserVerificationToken(string token)
        {
            return await dbContext.UserVerificationTokens
                .FirstOrDefaultAsync(t => t.VerificationCode == token);
        }

        public async Task VerifyEmail(int userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.VerifiedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteVerificationToken(int userId)
        {
            var token = await dbContext.UserVerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == userId);
            if (token != null)
            {
                dbContext.UserVerificationTokens.Remove(token);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
