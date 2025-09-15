using DPNS.Models;
using DPNS.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;

namespace DPNS.Managers
{
    public interface IUserManager
    {
        void RegisterUser(User user);
        void VerifyEmail(string token);
    }

    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void RegisterUser(User user)
        {
            if (_userRepository.GetUser(user.Email) != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var passwordHasher = new PasswordHasher<User>();
            string hash = passwordHasher.HashPassword(user, user.Password);

            int userId = _userRepository.AddUser(user.Name, user.Email, hash);

            _userRepository.CreateVerificationToken(userId);

            SmtpClient smtpClient = new();
            smtpClient.Send(new MailMessage(
                "info@dpns.com",
                user.Email, 
                "Welcome to DPNS",
                $"Hello {user.Name},<br/><br/>Thank you for registering at DPNS! We're excited to have you on board.<br/><br/>" +
                "Please verify your email <a href=''>here.</a><br/><br/>Best regards,<br/>The DPNS Team"
            ));
        }

        public void VerifyEmail(string token)
        {
            var verificationToken = _userRepository.GetUserVerificationToken(token);
            if (verificationToken == null || verificationToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invalid or expired verification token");
            }
            
            _userRepository.VerifyEmail(verificationToken.UserId);
            _userRepository.DeleteVerificationToken(verificationToken.UserId);
        }
    }
}
