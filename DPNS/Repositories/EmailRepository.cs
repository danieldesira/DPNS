using StackExchange.Redis;

namespace DPNS.Repositories
{
    public interface IEmailRepository
    {
        Task AddEmail(string to, string subject, string body);
    }

    public class EmailRepository(IConfiguration configuration) : IEmailRepository
    {
        public async Task AddEmail(string to, string subject, string body)
        {
            string redisHost = configuration?["Redis:Host"] ?? "localhost";
            int redisPort = int.TryParse(configuration?["Redis:Port"], out int port) ? port : 6379;

            var muxer = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions
            {
                EndPoints = { { redisHost, redisPort } },
                User = configuration?["Redis:User"],
                Password = configuration?["Redis:Password"],
            });
            var db = muxer.GetDatabase();
            await db.ListLeftPushAsync("emails", System.Text.Json.JsonSerializer.Serialize(new
            {
                Receiver = to,
                Subject = subject,
                Body = body,
                CreatedAt = DateTime.Now,
            }));
        }
    }
}
