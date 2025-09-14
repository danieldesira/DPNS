using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface IUserRepository
    {
        void AddUser(string name, string email, string password);
    }

    public class UserRepository : IUserRepository
    {
        public readonly NeondbContext _dbContext;

        public UserRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public void AddUser(string name, string email, string password)
        {
            _dbContext.Users.Add(new User
            {
                Name = name,
                Email = email,
                Password = password,
            });
            _dbContext.SaveChanges();
        }
    }
}
