using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

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
        [HttpGet]
        public async Task <IActionResult> UploadFromExcel()
        {
            List<WorkResponse> allWorks = await _workService.GetAllWorks();
            return View(allWorks);
        }

        [Route("[action]")]
        [Route("/")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            List<WorkResponse> allWorks = await _workService.GetAllWorks();

            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select an xlsx file";
                return View(allWorks);
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";
                return View(allWorks);
            }

            int dataCountInserted = await _workService.UploadWorkDataFromExcelFile(excelFile);

            ViewBag.Message = $"{dataCountInserted} Uploaded";

            //Get Error Report to show it to user
            ViewBag.Errors = _workService.GetErrors();

            return View(allWorks);
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult GetErrorReport(List<string> errors) 
        {
            return View(errors);
        }
    }
}
