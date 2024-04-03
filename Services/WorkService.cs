using Microsoft.AspNetCore.Http;
using ServiceContracts;

namespace Services
{
    public class WorkService : IWorkService
    {
        public Task<int> UploadWorkDataFromExcelFile(IFormFile formFile)
        {
            throw new NotImplementedException();
        }
    }
}
