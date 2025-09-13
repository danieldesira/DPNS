using DPNS.Managers;
using DPNS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly IAppManager _appManager;

        public AppController(IAppManager appManager) => _appManager = appManager;

        [HttpPost]
        public IResult CreateApp([FromBody] App payload)
        {
            try
            {
                _appManager.AddApp(payload.AppName, payload.Url);
            }
            catch (InvalidOperationException e)
            {
                return Results.Conflict(e.Message);
            }

            return Results.Ok(new { Message = "Project added successfully!" });
        }
    }
}
