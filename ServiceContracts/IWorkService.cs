using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IWorkService
    {
        Task<int> UploadWorkDataFromExcelFile(IFormFile formFile);

        List<string> GetErrors();

        Task<List<WorkResponse>> GetAllWorks();
    }
}
