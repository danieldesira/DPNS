using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DPNS.Extensions;

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
                await appManager.AddApp(payload.AppName, payload.Url, User.GetUserId() ?? 0);
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
            var userId = User.GetUserId();
            var apps = await appManager.GetUserApps(userId ?? 0);
            return Results.Ok(apps);
        }

        [HttpPost("add-user")]
        [Authorize]
        public async Task<IResult> AddAppUser([FromBody] AddAppUserRequest payload)
        {
            try
            {
                await appManager.AddAppUser(payload.AppGuid, payload.Email, User.GetUserId() ?? 0);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }
            return Results.Ok(new { Message = "User added to app successfully!" });
        }


        [HttpGet("subscription-count/{appId}")]
        [Authorize]
        public async Task<IResult> GetSubscriptionCount([FromRoute] int appId)
        {
            try
            { 
                var subscriptionCount = await appManager.GetSubscriptionCount(appId, User.GetUserId() ?? 0);
                return Results.Ok(new { SubscriptionCount = subscriptionCount });
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }
        }
    }
}
