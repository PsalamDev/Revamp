using Microsoft.AspNetCore.Mvc;
using Service.Controllers;

namespace Service.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VersionedBaseController : BaseController
    {
    }


}
