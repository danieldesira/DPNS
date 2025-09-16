using DPNS.Models;
using DPNS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace DPNS.Managers
{
    public interface IUserManager
    {
        void RegisterUser(User user);
        void VerifyEmail(string token);
        string Login(string email, string password);
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

        public string Login(string email, string password)
        {
            var user = _userRepository.GetUser(email);
            if (user == null || user.VerifiedAt == null)
            {
                throw new InvalidOperationException("Invalid email or email not verified");
            }
            var passwordHasher = new PasswordHasher<DbModels.User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid password");
            }
            return GenerateJwtToken(email);
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
