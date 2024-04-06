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

                    bool areСonditionsMet = AreСonditionsMet(work);

                    if (work != null && areСonditionsMet)
                    {
                        _db.Add(work);


                        // Condition 2
                        _db.Works.OrderBy(x => x.SenderWorkCode);

                        await _db.SaveChangesAsync();
                        dataInserted++;
                    }
                }
            }

            return dataInserted;
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
                    work.Role = cellValue.Trim();
                }
                else if (head == "Shareholder")
                {
                    work.ShareHolder = cellValue.Trim();
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
                else if (head == "ISWC" && !string.IsNullOrWhiteSpace(cellValue))
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

        public bool AreСonditionsMet(Work work)
        {
            bool conditionsMet = true;

            // Condition 2.2
            if (IsDuplicatedISWC(work))
            {
                conditionsMet = false;
                _errorsList.Add($"WORK already in database, omitted. ISWC = {work.ISWC}, Sender Work Code = {work.SenderWorkCode}");
            }

            // Condition 3a and 3c
            if (!IsTitleAndISWCNeeded(work)) 
            {
                work.Title = null;
                work.ISWC = null;
            }

            // Condition 3b
            if (IsShareholderEqualP(work))
            {
                work.Language = "PL";
            }

            //Condition 4
            if (IsRecordCodeEqualD(work))
            {
                work.Title = "AKA TITLE";
            }

            // Check whether there are duplicates in the database, Condition 2.1

            if (IsDuplicate(work))
            {
                conditionsMet = false;
                _errorsList.Add($"WORK already in database, it can`t be duplicated. Sender Work Code = {work.SenderWorkCode}");
            }

            return conditionsMet;
        }

        //Condition 5
        public bool IsRecordCodeEqualUAndIpiSame(Work work)
        {
            if (work.RecordCode != 'U')
                return false;

            Work? foundWork = _db.Works.Where(x => x.IPI == work.IPI && x.RecordCode == 'U').FirstOrDefault();

            if (foundWork == null)
                return false;

            //Condition 5a
            if (foundWork.Role == "C" && work.Role == "A" || foundWork.Role == "A" && work.Role == "C")
            {
                foundWork.Role = "CA";
            }
            else if (foundWork.Role == "C" && work.Role == "AD" || foundWork.Role == "AD" && work.Role == "C")
            {
                foundWork.Role = "CA";
            }
            else if (foundWork.Role == "A" && work.Role == "AR" || foundWork.Role == "AR" && work.Role == "A")
            {
                foundWork.Role = "CA";
            }
            else if (foundWork.Role == "A" && work.Role == "AD" || foundWork.Role == "AD" && work.Role == "A")
            {
                foundWork.Role = "A";
            }
            else if (foundWork.Role == "C" && work.Role == "AR" || foundWork.Role == "AR" && work.Role == "C")
            {
                foundWork.Role = "C";
            }

            //Condition 5b
            foundWork.InWorkPR += work.InWorkPR;

            //Condition 5c
            if (foundWork.Controlled != null || work.Controlled != null) 
            {
                _errorsList.Add($"No uncontrolled for all lines with the same IPI Name Number. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }

            //Condition 5e
            if (foundWork.AgreementNumber != work.AgreementNumber)
            {
                _errorsList.Add($"No Agreement for all lines with the same IPI Name Number. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }









            return true;
        }


        //Condition 4
        public bool IsRecordCodeEqualD(Work work) 
        {
            if (work.RecordCode == 'D')
                return true;
            else
                return false;
        }

        // Condition 3b
        public bool IsShareholderEqualP(Work work) 
        {
            if (work.ShareHolder == "P")
                return true;
            else
                return false;
        }

        // Condition 3 and 3c
        public bool IsTitleAndISWCNeeded(Work work) 
        {
            if (work.RecordCode == 'T')
                return true;
            else
                return false;
        }


        // Condition 2.2
        public bool IsDuplicatedISWC(Work work)
        {
            if (work.ISWC == null) 
            {
                return false;
            }

            return _db.Works.Where(x => x.ISWC == work.ISWC).Count() > 0;
        }


        // Condition 2.1
        public bool IsDuplicate(Work work)
        {
            return _db.Works.Where(x => x.SenderWorkCode == work.SenderWorkCode && x.RecordCode == work.RecordCode
            && x.Title == work.Title && x.Role == work.Role && x.ShareHolder == work.ShareHolder && x.IPI == work.IPI
            && x.InWorkPR == work.InWorkPR && x.InWorkMR == work.InWorkMR && x.Controlled == work.Controlled
            && x.ISWC == work.ISWC && x.AgreementNumber == work.AgreementNumber).Count() > 0;
        }
    }
}
