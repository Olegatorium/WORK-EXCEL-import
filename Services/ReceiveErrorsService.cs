using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ReceiveErrorsService : IReceiveErrorsService
    {
        public async Task<MemoryStream> GetErrorsExcel(List<string> errors)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("ERROR REPORT");

                int row = 1;
                foreach (var error in errors)
                {
                    workSheet.Cells[row, 1].Value = error;
                    row++;
                }

                workSheet.Cells[$"A1:A{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
