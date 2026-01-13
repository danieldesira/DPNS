using DPNS.Entities;
using DPNS.Managers;
using DPNS.Repositories;
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

builder.Services.AddDbContext<DpnsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationManager, NotificationManager>();
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IAppManager, AppManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddSingleton<IWebPushClient, WebPushClient>();

builder.Services.AddCors(options => options.AddPolicy("Origins",
    policy => policy.WithOrigins(
                    "https://localhost:5173",
                    "https://turtle-quest.vercel.app", 
                    "https://subnodulous-kaelyn-matrimonially.ngrok-free.dev"
                )
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

if (!app.Environment.IsDevelopment())
{ 
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    if (context.Request.Method == HttpMethods.Options)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
    }

    await next();
});

app.UseAuthentication(); // <--- MUST be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
