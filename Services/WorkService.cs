using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ServiceContracts;
using System.Diagnostics.Metrics;

namespace Services
{
    public class WorkService : IWorkService
    {
        private readonly WorksDbContext _db;

        public WorkService(WorksDbContext db)
        {
            _db = db;
        }

        public async Task<int> UploadWorkDataFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int dataInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                Dictionary<string, List<string>> worksData = new Dictionary<string, List<string>>();

                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Works"];

                if (workSheet == null)
                    return 0;

                int rowCount = workSheet.Dimension.Rows;
                int columnCount = workSheet.Dimension.Columns;

                for (int column = 1; column <= columnCount; column++)
                {
                    string? headValue = Convert.ToString(workSheet.Cells[1, column].Value);

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string? cellValue = Convert.ToString(workSheet.Cells[row, column].Value);

                        if (!string.IsNullOrEmpty(cellValue))
                        {
                            

                            //Country country = new Country() { CountryName = countryName };
                            //_db.Countries.Add(country);

                            //await _db.SaveChangesAsync();

                            dataInserted++;
                        }
                    }
                }

            }  
                  return dataInserted;
        }
    }
}
