using DPNS.Entities;
using DPNS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NUnitTests
{
    public class Tests
    {
        private DpnsDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DpnsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new DpnsDbContext(options);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Test_Registration_UserUnverified()
        {
            var userRepository = new UserRepository(_context);
            await userRepository.AddUser("Example", "example@dpns.net", "pass@123ABC");
            var user = await userRepository.GetUser("example@dpns.net");
            Assert.That(user?.VerifiedAt, Is.Null);
        }
    }
}
