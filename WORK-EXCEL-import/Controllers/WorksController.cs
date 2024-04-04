using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace WORK_EXCEL_import.Controllers
{
    public class WorksController : Controller
    {
        private readonly IWorkService _workService;
        public WorksController(IWorkService workService)
        {
            _workService = workService;
        }

        [Route("[action]")]
        [Route("/")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("[action]")]
        [Route("/")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";
                return View();
            }

            int dataCountInserted = await _workService.UploadWorkDataFromExcelFile(excelFile);

            List<string> errors = _workService.GetErrors();

            ViewBag.Message = $"{dataCountInserted} Uploaded";

            return View(errors);
        }
    }
}
