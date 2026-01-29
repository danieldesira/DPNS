using DPNS.Entities;
using DPNS.Managers;
using DPNS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NUnitTests
{
    public class Tests
    {
        private DpnsDbContext _context;
        private IUserManager _userManager;
        private IAppManager _appManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DpnsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new DpnsDbContext(options);

            var config = new ConfigurationManager();
            config["Jwt:Key"] = "test-key-should-be-long-enough-123456789";
            config["Jwt:Issuer"] = "test-issuer";
            config["Jwt:Audience"] = "test-audience";

            _userManager = new UserManager(
                new UserRepository(_context),
                config,
                new EmailRepository(_context)
            );

            _appManager = new AppManager(
                new AppRepository(_context),
                new SubscriptionRepository(_context),
                new UserRepository(_context)
            );
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Test_Registration_UserUnverified()
        {
            await _userManager.RegisterUser(new DPNS.Models.User
            {
                Name = "Example",
                Email = "example@dpns.net",
                Password = "pass@123ABC"
            });
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _userManager.Login("example@dpns.net", "pass@123ABC");
            });
            Assert.That(ex, Is.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public async Task Test_Registration_Login()
        {
            await _userManager.RegisterUser(new DPNS.Models.User
            {
                Name = "Example",
                Email = "example@dpns.net",
                Password = "pass@123ABC"
            });

            await _userManager.VerifyEmail(
                (await _context.UserVerificationTokens
                    .FirstAsync()).VerificationCode);
            

            string token = await _userManager.Login("example@dpns.net", "pass@123ABC");

            Assert.That(token, Is.InstanceOf<string>());
        }

        [Test]
        public async Task Test_Create_Duplicate_App()
        {
            await _userManager.RegisterUser(new DPNS.Models.User
            {
                Name = "Example",
                Email = "example@dpns.net",
                Password = "pass@123ABC"
            });

            await _userManager.VerifyEmail(
                (await _context.UserVerificationTokens
                    .FirstAsync()).VerificationCode);


            string token = await _userManager.Login("example@dpns.net", "pass@123ABC");

            await _appManager.AddApp("test_app", "https://test.app", (await _context.Users.FirstAsync())?.Id ?? 0);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _appManager.AddApp("test_app", "https://test.app", (await _context.Users.FirstAsync())?.Id ?? 0);
            });
            Assert.That(ex, Is.InstanceOf<InvalidOperationException>());
        }
    }
}
