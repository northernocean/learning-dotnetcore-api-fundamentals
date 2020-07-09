using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        public object Get()
        {
            return new { Moniker = "ATL2018", Name = "Atlanta Code Camp" };
        }
    }
}
