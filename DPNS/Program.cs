using DPNS.Caching;
using DPNS.DbModels;
using DPNS.Managers;
using DPNS.Repositories;
using Enyim.Caching.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddLogging();
builder.Services.AddEnyimMemcached(options => options.Servers = [
    new Server
    {
        Address = builder.Configuration["MemCachedHost"],
        Port = Convert.ToInt32(builder.Configuration["MemCachedPort"])
    }
]);

builder.Services.AddSingleton<ICacheProvider, CacheProvider>();
builder.Services.AddSingleton<ICacheRepository, CacheRepository>();

builder.Services.AddDbContext<NeondbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<INotificationManager, NotificationManager>();

builder.Services.AddCors(options => options.AddPolicy("Origins",
    policy => policy.WithOrigins("https://localhost:5173", "https://turtle-quest.vercel.app/")
                .AllowAnyHeader()
                .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Origins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
