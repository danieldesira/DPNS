using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController(IAppManager appManager) : ControllerBase
    {
        [HttpPost, Authorize]
        public IResult CreateApp([FromBody] App payload)
        {
            try
            {
                appManager.AddApp(payload.AppName, payload.Url);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }

            return Results.Ok(new { Message = "Project added successfully!" });
        }
    }
}
