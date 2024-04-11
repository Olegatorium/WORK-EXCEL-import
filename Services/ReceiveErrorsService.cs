using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.Modules;

namespace Services
{
    public class ReceiveErrorsService : IReceiveErrorsService
    {
        public List<ErrorReport> GetErrorReport(List<string> errors) 
        {
            List<ErrorReport> errorReports = new List<ErrorReport>();

            foreach (var item in errors)
            {
                string error = "";
                string senderWorkCode = "";

                int errorToElemt = 0;

                string[] errorParts = item.Split(" ");

                for (int i = 0; i < errorParts.Length; i++)
                {
                    if (errorParts[i] == "Sender")
                    {
                        errorToElemt = i;
                        break;
                    }
                    else if (i == errorParts.Length - 1)
                    {
                        errorToElemt = i;
                    }                  
                }

                for (int i = 0; i < errorParts.Length; i++)
                {
                    if (i < errorToElemt)
                    {
                        error += errorParts[i] + " ";
                    }
                    else if (i > errorToElemt && errorParts[i].All(char.IsDigit))
                    {
                        senderWorkCode = errorParts[i];
                    }                
                }

                errorReports.Add(new ErrorReport()
                {
                    Error = error,
                    SenderWorkCode = senderWorkCode
                });
            }

            return errorReports;
        }
        public async Task<MemoryStream> GetErrorsExcel(List<string> stringErrors)
        {
            MemoryStream memoryStream = new MemoryStream();

            List<ErrorReport> errors = GetErrorReport(stringErrors);

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("ERROR REPORT");

                workSheet.Cells[1, 1].Value = "Sender Work Code";
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 12;
                workSheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Green);

                workSheet.Cells[1, 2].Value = "Error";
                workSheet.Cells[1, 2].Style.Font.Bold = true;
                workSheet.Cells[1, 2].Style.Font.Size = 12;
                workSheet.Cells[1, 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                int row = 2;
                foreach (var error in errors)
                {
                    workSheet.Cells[row, 1].Value = error.SenderWorkCode;
                    workSheet.Cells[row, 2].Value = error.Error;
                    row++;
                }

                workSheet.Cells[$"A1:B{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
