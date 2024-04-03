using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface IWorkService
    {
        Task<int> UploadWorkDataFromExcelFile(IFormFile formFile);
    }
}
