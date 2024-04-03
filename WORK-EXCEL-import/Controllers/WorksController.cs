using Microsoft.AspNetCore.Mvc;

namespace WORK_EXCEL_import.Controllers
{
    [Route("[controller]")]
    public class WorksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
