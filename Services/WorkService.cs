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

        private List<string> _errorsList;

        public WorkService(WorksDbContext db)
        {
            _errorsList = new List<string>();
            _db = db;
        }

        public List<string> GetErrors()
        {
            return _errorsList;
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

                for (int row = 2; row <= rowCount; row++)
                {
                    Work work = CreateWorkFromExcelRow(workSheet, row);

                    bool workIsDuplicate = IsDuplicate(work);

                    if (work != null && !workIsDuplicate)
                    {
                        _db.Add(work);
                        await _db.SaveChangesAsync();
                        dataInserted++;
                    }
                }
            }

            return dataInserted;
        }

        public bool IsDuplicate(Work work)
        {
            return _db.Works.Where(x => x.SenderWorkCode == work.SenderWorkCode && x.RecordCode == work.RecordCode
            && x.Title == work.Title && x.Role == work.Role && x.ShareHolder == work.ShareHolder && x.IPI == work.IPI
            && x.InWorkPR == work.InWorkPR && x.InWorkMR == work.InWorkMR && x.Controlled == work.Controlled
            && x.ISWC == work.ISWC && x.AgreementNumber == work.AgreementNumber).Count() != 0;
        }

        public Work CreateWorkFromExcelRow(ExcelWorksheet workSheet, int row)
        {
            Work work = new Work();
            int columnCount = workSheet.Dimension.Columns;

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

            return work;
        }





















        //public async Task<int> UploadWorkDataFromExcelFile(IFormFile formFile)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    await formFile.CopyToAsync(memoryStream);
        //    int dataInserted = 0;

        //    using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        //    {
        //        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Works"];

        //        if (workSheet == null)
        //            return 0;

        //        int rowCount = workSheet.Dimension.Rows;
        //        int columnCount = workSheet.Dimension.Columns;

        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            Work work = new Work();

        //            for (int column = 1; column <= columnCount; column++)
        //            {
        //                string? head = Convert.ToString(workSheet.Cells[1, column].Value);
        //                string? cellValue = Convert.ToString(workSheet.Cells[row, column].Value);

        //                if (head == "Sender Work Code")
        //                {
        //                    work.SenderWorkCode = cellValue;
        //                }
        //                else if (head == "Record Code" && !string.IsNullOrWhiteSpace(cellValue)) 
        //                {
        //                    work.RecordCode = cellValue[0];
        //                }
        //                else if (head == "Title")
        //                {
        //                    work.Title = cellValue;
        //                }
        //                else if (head == "Role" && !string.IsNullOrWhiteSpace(cellValue))
        //                {
        //                    work.Role = cellValue[0];
        //                }
        //                else if (head == "Shareholder")
        //                {
        //                    work.ShareHolder = cellValue;
        //                }
        //                else if (head == "IPI")
        //                {
        //                    work.IPI = cellValue;
        //                }
        //                else if (head == "InWork PR" && !string.IsNullOrWhiteSpace(cellValue))
        //                {
        //                    work.InWorkPR = int.Parse(cellValue);
        //                }
        //                else if (head == "InWork MR" && !string.IsNullOrWhiteSpace(cellValue))
        //                {
        //                    work.InWorkMR = int.Parse(cellValue);
        //                }
        //                else if (head == "Controlled" && !string.IsNullOrWhiteSpace(cellValue))
        //                {
        //                    work.Controlled = cellValue[0];
        //                }
        //                else if (head == "ISWC")
        //                {
        //                    work.ISWC = cellValue;
        //                }
        //                else if (head == "AGREEMENT NO")
        //                {
        //                    work.AgreementNumber = cellValue;
        //                }
        //            }

        //            _db.Add(work);
        //            await _db.SaveChangesAsync();
        //            dataInserted++;
        //        }
        //    }

        //    return dataInserted;
        //}






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
