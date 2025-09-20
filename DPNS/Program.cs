using DPNS.Caching;
using DPNS.DbModels;
using DPNS.Managers;
using DPNS.Repositories;
using Enyim.Caching.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebPush;

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
builder.Services.AddTransient<IAppRepository, AppRepository>();
builder.Services.AddTransient<IAppManager, AppManager>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserManager, UserManager>();

builder.Services.AddSingleton<IWebPushClient, WebPushClient>();

builder.Services.AddCors(options => options.AddPolicy("Origins",
    policy => policy.WithOrigins("https://localhost:5173", "https://turtle-quest.vercel.app/")
                .AllowAnyHeader()
                .AllowAnyMethod()));

// Validate presence of required JWT config early and use it
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Configuration 'Jwt:Key' is missing or empty. Set Jwt:Key in appsettings.json or as an environment variable.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Origins");

app.UseHttpsRedirection();

app.UseAuthentication(); // <--- MUST be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
