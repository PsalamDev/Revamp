using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewerDashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
