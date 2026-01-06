using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface IUserRepository
    {
        int AddUser(string name, string email, string password);
        User? GetUser(string email);
        void CreateVerificationToken(int userId);
        UserVerificationToken? GetUserVerificationToken(string token);
        void VerifyEmail(int userId);
        void DeleteVerificationToken(int userId);
    }

    public class UserRepository(NeondbContext dbContext) : IUserRepository
    {
        public int AddUser(string name, string email, string password)
        {
            var userEntry = dbContext.Users.Add(new User
            {
                Name = name,
                Email = email,
                Password = password,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
            return userEntry.Entity.Id;
        }

        public User? GetUser(string email)
        {
            return dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public void CreateVerificationToken(int userId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            dbContext.UserVerificationTokens.Add(new UserVerificationToken
            {
                UserId = userId,
                VerificationCode = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
            dbContext.SaveChanges();
        }

        public UserVerificationToken? GetUserVerificationToken(string token)
        {
            return dbContext.UserVerificationTokens
                .FirstOrDefault(t => t.VerificationCode == token);
        }

        public void VerifyEmail(int userId)
        {
            var user = dbContext.Users.Find(userId);
            if (user != null)
            {
                user.VerifiedAt = DateTime.UtcNow;
                dbContext.SaveChanges();
            }
        }

        public void DeleteVerificationToken(int userId)
        {
            var token = dbContext.UserVerificationTokens
                .FirstOrDefault(t => t.UserId == userId);
            if (token != null)
            {
                dbContext.UserVerificationTokens.Remove(token);
                dbContext.SaveChanges();
            }
        }
    }
}
