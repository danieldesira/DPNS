using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IAuthenticationManager
    {
        void RegisterUser(string name, string email, string password);
    }

    public class AuthenticationManager
    {
        private readonly IUserRepository _userRepository;
        
        public AuthenticationManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RegisterUser(string name, string email, string password)
        {
            //to-do create password hash and salt
            // Add user to the repository
            _userRepository.AddUser(name, email, password);
        }
    }
}
