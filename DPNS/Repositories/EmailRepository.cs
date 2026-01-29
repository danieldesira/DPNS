using DPNS.Entities;

namespace DPNS.Repositories
{
    public interface IEmailRepository
    {
        Task AddEmail(string to, string subject, string body);
    }

    public class EmailRepository(DpnsDbContext dbContext) : IEmailRepository
    {
        public Task AddEmail(string to, string subject, string body)
        {
            dbContext.EmailMessages.Add(new EmailMessage
            {
                ToEmail = to,
                Subject = subject,
                Body = body,
                CreatedAt = DateTime.UtcNow,
            });
            return dbContext.SaveChangesAsync();
        }
    }
}
