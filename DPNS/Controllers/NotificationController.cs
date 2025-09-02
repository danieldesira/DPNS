using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DPNS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public NotificationController() { }

        [HttpPost]
        public HttpResponse SendNotification()
        {
            throw new NotImplementedException();
        }
    }
}
