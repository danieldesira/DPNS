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

    public class UserRepository : IUserRepository
    {
        public readonly NeondbContext _dbContext;

        public UserRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public int AddUser(string name, string email, string password)
        {
            var userEntry = _dbContext.Users.Add(new User
            {
                Name = name,
                Email = email,
                Password = password,
                CreatedAt = DateTime.UtcNow,
            });
            _dbContext.SaveChanges();
            return userEntry.Entity.Id;
        }

        public User? GetUser(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public void CreateVerificationToken(int userId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _dbContext.UserVerificationTokens.Add(new UserVerificationToken
            {
                UserId = userId,
                VerificationCode = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
            _dbContext.SaveChanges();
        }

        public UserVerificationToken? GetUserVerificationToken(string token)
        {
            return _dbContext.UserVerificationTokens
                .FirstOrDefault(t => t.VerificationCode == token);
        }

        public void VerifyEmail(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user != null)
            {
                user.VerifiedAt = DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
        }

        public void DeleteVerificationToken(int userId)
        {
            var token = _dbContext.UserVerificationTokens
                .FirstOrDefault(t => t.UserId == userId);
            if (token != null)
            {
                _dbContext.UserVerificationTokens.Remove(token);
                _dbContext.SaveChanges();
            }
        }
    }
}
