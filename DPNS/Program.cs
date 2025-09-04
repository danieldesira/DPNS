using DPNS.Caching;
using Enyim.Caching.Configuration;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();
