using DPNS.Entities;
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
        Task RegisterUser(Models.User user);
        Task VerifyEmail(string token);
        Task<string> Login(string email, string password);
    }

    public class UserManager(IUserRepository userRepository, IConfiguration configuration) : IUserManager
    {
        public async Task RegisterUser(Models.User user)
        {
            if (await userRepository.GetUser(user.Email) != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var passwordHasher = new PasswordHasher<Models.User>();
            string hash = passwordHasher.HashPassword(user, user.Password);

            await userRepository.AddUser(user.Name, user.Email, hash);

            SmtpClient smtpClient = new();
            smtpClient.Send(new MailMessage(
                "info@dpns.com",
                user.Email,
                "Welcome to DPNS",
                $"Hello {user.Name},<br/><br/>Thank you for registering at DPNS! We're excited to have you on board.<br/><br/>" +
                "Please verify your email <a href=''>here.</a><br/><br/>Best regards,<br/>The DPNS Team"
            ));
        }

        public async Task VerifyEmail(string token)
        {
            var verificationToken = await userRepository.GetUserVerificationToken(token);
            if (verificationToken == null || verificationToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invalid or expired verification token");
            }

            await userRepository.VerifyEmail(verificationToken.UserId);
            await userRepository.DeleteVerificationToken(verificationToken.UserId);
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await userRepository.GetUser(email);
            if (user == null || user.VerifiedAt == null)
            {
                throw new InvalidOperationException("Invalid email or email not verified");
            }
            var passwordHasher = new PasswordHasher<Entities.User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Invalid password");
            }
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Entities.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
