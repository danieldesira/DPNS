using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController(IAppManager appManager) : ControllerBase
    {
        [HttpPost, Authorize]
        public async Task<IResult> CreateApp([FromBody] App payload)
        {
            try
            {
                await appManager.AddApp(payload.AppName, payload.Url);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }

            return Results.Ok(new { Message = "Project added successfully!" });
        }

        [HttpGet, Authorize]
        public async Task<IResult> GetUserApps()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Results.Unauthorized();
            }
            var apps = await appManager.GetUserApps(userId);
            return Results.Ok(apps);
        }

        [HttpGet("subscription-count/{appId}")]
        [Authorize]
        public async Task<IResult> GetSubscriptionCount([FromRoute] int appId)
        {
            var subscriptionCount = await appManager.GetSubscriptionCount(appId);
            return Results.Ok(new { SubscriptionCount = subscriptionCount });
        }
    }
}
