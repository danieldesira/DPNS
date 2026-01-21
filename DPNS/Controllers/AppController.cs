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

        [HttpDelete("{appId}/remove-user")]
        [Authorize]
        public async Task<IResult> RemoveAppUser([FromRoute] Guid appId, [FromBody] RemoveAppUserRequest payload)
        {
            try
            {
                await appManager.RemoveAppUser(appId, payload.Email, User.GetUserId() ?? 0);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }
            return Results.Ok(new { Message = "User removed from app successfully!" });
        }

        [HttpGet("{appGuid}/subscription-count")]
        [Authorize]
        public async Task<IResult> GetSubscriptionCount([FromRoute] Guid appGuid)
        {
            try
            { 
                var subscriptionCount = await appManager.GetSubscriptionCount(appGuid, User.GetUserId() ?? 0);
                return Results.Ok(new { SubscriptionCount = subscriptionCount });
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }
        }
    }
}
