using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class WorkResponse
    {
        public string SenderWorkCode { get; set; }

        public char? RecordCode { get; set; }

        public string? Title { get; set; }

        public string? Role { get; set; }

        public string? ShareHolder { get; set; }

        public string? IPI { get; set; }

        public int? InWorkPR { get; set; }

        public int? InWorkMR { get; set; }

        public char? Controlled { get; set; }

        public string? ISWC { get; set; }

        public string? AgreementNumber { get; set; }

        public string? Language { get; set; }

        public string? Rightsholder { get; set; }
    }

    public static class WorkExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Work class into WorkResponse class
        /// </summary>
        /// <param name="work">The Work object to convert</param>
        /// /// <returns>Returns the converted WorkResponse object</returns>
        public static WorkResponse ToWorkResponse(this Work work)
        {
            return new WorkResponse()
            {
                SenderWorkCode = work.SenderWorkCode,
                RecordCode = work.RecordCode,
                Title = work.Title,
                Role = work.Role,
                ShareHolder = work.ShareHolder,
                IPI = work.IPI,
                InWorkPR = work.InWorkPR,
                InWorkMR = work.InWorkMR,
                Controlled = work.Controlled,
                ISWC = work.ISWC,
                AgreementNumber = work.AgreementNumber,
                Language = work.Language,
                Rightsholder = work.Rightsholder
            };
        }
    }
}
