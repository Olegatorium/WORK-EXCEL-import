using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Works"];

                if (workSheet == null)
                    return 0;

                int rowCount = workSheet.Dimension.Rows;
                int columnCount = workSheet.Dimension.Columns;

                for (int row = 2; row <= rowCount; row++)
                {
                    Work work = new Work();

                    for (int column = 1; column <= columnCount; column++)
                    {
                        string? head = Convert.ToString(workSheet.Cells[1, column].Value);
                        string? cellValue = Convert.ToString(workSheet.Cells[row, column].Value);

                        if (head == "Sender Work Code")
                        {
                            work.SenderWorkCode = cellValue;
                        }
                        else if (head == "Record Code" && !string.IsNullOrWhiteSpace(cellValue)) 
                        {
                            work.RecordCode = cellValue[0];
                        }
                        else if (head == "Title")
                        {
                            work.Title = cellValue;
                        }
                        else if (head == "Role" && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            work.Role = cellValue[0];
                        }
                        else if (head == "Shareholder")
                        {
                            work.ShareHolder = cellValue;
                        }
                        else if (head == "IPI")
                        {
                            work.IPI = cellValue;
                        }
                        else if (head == "InWork PR" && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            work.InWorkPR = int.Parse(cellValue);
                        }
                        else if (head == "InWork MR" && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            work.InWorkMR = int.Parse(cellValue);
                        }
                        else if (head == "Controlled" && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            work.Controlled = cellValue[0];
                        }
                        else if (head == "ISWC")
                        {
                            work.ISWC = cellValue;
                        }
                        else if (head == "AGREEMENT NO")
                        {
                            work.AgreementNumber = cellValue;
                        }
                    }

                    _db.Add(work);
                    await _db.SaveChangesAsync();
                    dataInserted++;
                }
            }

            return dataInserted;
        }


        //public async Task<int> UploadWorkDataFromExcelFile(IFormFile formFile)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    await formFile.CopyToAsync(memoryStream);
        //    int dataInserted = 0;

        //    Dictionary<string, List<string>> worksData = new Dictionary<string, List<string>>();

        //    using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        //    {
        //        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Works"];

        //        if (workSheet == null)
        //            return 0;

        //        int rowCount = workSheet.Dimension.Rows;
        //        int columnCount = workSheet.Dimension.Columns;

        //        for (int column = 1; column <= columnCount; column++)
        //        {
        //            string? headValue = Convert.ToString(workSheet.Cells[1, column].Value);

        //            if (headValue != null)
        //                worksData.Add(headValue, new List<string>());

        //            List<string> values = new List<string>();

        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                string? cellValue = Convert.ToString(workSheet.Cells[row, column].Value);

        //                if (!string.IsNullOrEmpty(cellValue))
        //                {
        //                    values.Add(cellValue);
        //                    dataInserted++;
        //                }
        //            }

        //            worksData[headValue] = values;
        //        }
        //    }

        //   return dataInserted;
        //}
    }
}
