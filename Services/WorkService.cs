using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using ServiceContracts;
using System.Diagnostics.Metrics;

namespace Services
{
    public class WorkService : IWorkService
    {
        private readonly WorksDbContext _db;

        private List<string> _errorsList;

        private bool _excelFormatCorrect = true;

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
            MemoryStream memoryStream = await GetMemoryStreamFromFormFile(formFile);
            int dataInserted = await ProcessExcelFile(memoryStream);
            return dataInserted;
        }

        public async Task<MemoryStream> GetMemoryStreamFromFormFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream;
        }

        public async Task<int> ProcessExcelFile(MemoryStream memoryStream)
        {
            int dataInserted = 0;
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                foreach (ExcelWorksheet workSheet in excelPackage.Workbook.Worksheets)
                {
                    if (workSheet == null)
                        continue;

                    int rowCount = workSheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        Work work = CreateWorkFromExcelRow(workSheet, row);

                        if (!_excelFormatCorrect)
                        {
                            _excelFormatCorrect = true;
                            return 0;
                        }

                        if (work == null)
                            continue;

                        // Check whether all conditions met
                        bool areConditionsMet = await AreСonditionsMet(work);

                        if (!areConditionsMet)
                            continue;

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

                if (IsSenderWorkCodeEmpty(head, cellValue))
                    return null;

                if (!IsExcelFormatCorrect(head, cellValue, column))
                {
                    _errorsList.Add("Incorrect excel format. Please provide correct excel format.");
                    _excelFormatCorrect = false;
                    return null;
                }

                if (string.IsNullOrWhiteSpace(cellValue))
                    continue;

                //delete spaces to put date correct
                cellValue = cellValue.Trim();

                SetWorkProperty(head, cellValue, work);
            }

            return work;
        }

        public bool IsSenderWorkCodeEmpty(string? head, string? cellValue)
        {
            if (head == "Sender Work Code" && string.IsNullOrWhiteSpace(cellValue))
            {
                _errorsList.Add("Sender Work Code can't be empty");
                return true;
            }
            return false;
        }

        public bool IsExcelFormatCorrect(string? head, string? cellValue, int column)
        {
            if (column == 1 && (string.IsNullOrWhiteSpace(head) || string.IsNullOrWhiteSpace(cellValue)))
                return false;

            if (string.IsNullOrWhiteSpace(head))
                return false;

            return true;
        }

        public void SetWorkProperty(string head, string cellValue, Work work)
        {
            if (head == "Sender Work Code")
            {
                work.SenderWorkCode = cellValue;
            }
            else if (head == "Record Code")
            {
                work.RecordCode = cellValue[0];
            }
            else if (head == "Title")
            {
                work.Title = cellValue;
            }
            else if (head == "Role")
            {
                work.Role = cellValue;
            }
            else if (head == "Shareholder")
            {
                work.ShareHolder = cellValue;
            }
            else if (head == "IPI")
            {
                work.IPI = cellValue;
            }
            else if (head == "InWork PR" && int.TryParse(cellValue, out int inWorkPr))
            {
                work.InWorkPR = inWorkPr;
            }
            else if (head == "InWork MR" && int.TryParse(cellValue, out int inWorkMr))
            {
                work.InWorkMR = inWorkMr;
            }
            else if (head == "Controlled")
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

        public async Task<bool> AreСonditionsMet(Work work)
        {
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

            //Condition 5

            Work foundWork = await IsRecordCodeEqualUAndIpiSame(work);

            if (foundWork != null)
            {
                await CompareWorksWithRecordCodeEqualUAndIpiSame(work, foundWork);
            }

            //Condition 6
            if (work.RecordCode == 'U' && !await RecordCodeEqualU(work))
            {
                return false;
            }

            // Check whether there are duplicates in the database, Condition 2.1
            if (IsDuplicate(work))
            {
                _errorsList.Add($"WORK already in database, it can`t be duplicated. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }
            // Condition 2.2
            else if (IsDuplicatedISWC(work))
            {
                _errorsList.Add($"WORK already in database, omitted. ISWC = {work.ISWC}. Sender Work Code = {work.SenderWorkCode}");
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

            return _db.Works.Any(x => x.ISWC == work.ISWC);
        }

        // Condition 2.1
        public bool IsDuplicate(Work work)
        {
            return _db.Works.Any(x => x.SenderWorkCode == work.SenderWorkCode && x.RecordCode == work.RecordCode
                && x.Title == work.Title && x.Role == work.Role && x.ShareHolder == work.ShareHolder && x.IPI == work.IPI
                && x.InWorkPR == work.InWorkPR && x.InWorkMR == work.InWorkMR && x.Controlled == work.Controlled
                && x.ISWC == work.ISWC && x.AgreementNumber == work.AgreementNumber);
        }

        //Condition 5
        public async Task<Work> IsRecordCodeEqualUAndIpiSame(Work work)
        {
            if (work.RecordCode != 'U')
                return null;

            Work? foundWork = await _db.Works.Where(x => x.IPI == work.IPI &&
            x.SenderWorkCode == work.SenderWorkCode &&
            x.RecordCode == 'U').FirstOrDefaultAsync();

            if (foundWork == null)
                return null;

            return foundWork;
        }

        //Condition 5
        public void CheckIsFieldsCorrectOfFoundWork(Work workToAdd, Work foundWork) 
        {
            //Condition 5a
            if (foundWork.Role == "C" && workToAdd.Role == "A" || foundWork.Role == "A" && workToAdd.Role == "C")
            {
                foundWork.Role = "CA";
                workToAdd.Role = "CA";
            }
            else if (foundWork.Role == "C" && workToAdd.Role == "AD" || foundWork.Role == "AD" && workToAdd.Role == "C")
            {
                foundWork.Role = "CA";
                workToAdd.Role = "CA";
            }
            else if (foundWork.Role == "A" && workToAdd.Role == "AR" || foundWork.Role == "AR" && workToAdd.Role == "A")
            {
                foundWork.Role = "CA";
                workToAdd.Role = "CA";
            }
            else if (foundWork.Role == "A" && workToAdd.Role == "AD" || foundWork.Role == "AD" && workToAdd.Role == "A")
            {
                foundWork.Role = "A";
                workToAdd.Role = "A";
            }
            else if (foundWork.Role == "C" && workToAdd.Role == "AR" || foundWork.Role == "AR" && workToAdd.Role == "C")
            {
                foundWork.Role = "C";
                workToAdd.Role = "C";
            }

            //Condition 5b
            foundWork.InWorkPR += workToAdd.InWorkPR;
            workToAdd.InWorkPR = foundWork.InWorkPR;
        }

        //Condition 5
        public void SearchErrorsFoundWork(Work workToAdd, Work foundWork) 
        {
            //Condition 5e
            if (foundWork.AgreementNumber != workToAdd.AgreementNumber)
            {
                _errorsList.Add($"No Agreement for all lines with the same IPI Name Number. Sender Work Code = {workToAdd.SenderWorkCode}");
                return;
            }

            //Condition 5c
            if (foundWork.Controlled != null || workToAdd.Controlled != null)
            {
                _errorsList.Add($"No uncontrolled for all lines with the same IPI Name Number. Sender Work Code = {workToAdd.SenderWorkCode}");
                return;
            }
            //Condition 5d
            else if (foundWork.Controlled != 'Y' || workToAdd.Controlled != 'Y')
            {
                _errorsList.Add($"No controlled for all lines with the same IPI Name Number. Sender Work Code = {workToAdd.SenderWorkCode}");
                return;
            }
        }

        //Condition 5
        public async Task CompareWorksWithRecordCodeEqualUAndIpiSame(Work workToAdd, Work foundWork)
        {
            CheckIsFieldsCorrectOfFoundWork(workToAdd, foundWork);

            await _db.SaveChangesAsync();

            SearchErrorsFoundWork(workToAdd, foundWork);
        }

        //Condition 6
        public async Task<bool> RecordCodeEqualU(Work work)
        {
            // Condition 6
            Work? foundWork = await _db.Works.Where(x => x.IPI == work.IPI && x.Rightsholder != null).FirstOrDefaultAsync();

            if (foundWork != null)
            {
                _errorsList.Add($"The same IPI is already in the database and it has data for Rightsholder. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }

            work.Rightsholder = $"{work.ShareHolder} {work.IPI}";

            // Skip Condition 6.a.i

            //Condition 6.a.ii.

            if (work.Controlled != 'Y')
            {
                _errorsList.Add($"No SWR in transaction. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }

            // Skip Condition 6b

            // Condition 6c

            if (work.Controlled == 'Y' && work.AgreementNumber == null)
            {
                _errorsList.Add($"No agreement with FinalSE. Sender Work Code = {work.SenderWorkCode}");
                return false;
            }

            return true;
        }
    }
}
