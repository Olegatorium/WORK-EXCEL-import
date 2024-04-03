using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Entities
{
    public class Work
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(100)]
        public string? SenderWorkCode { get; set; }

        public char RecordCode { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        public char? Role { get; set; }

        [MaxLength(100)]
        public string? ShareHolder { get; set; }

        [MaxLength(100)]
        public string? IPI { get; set; }

        public int? InWorkPR { get; set; }

        public int? InWorkMR { get; set; }

        public char? Controlled { get; set; }

        [MaxLength(100)]
        public string? ISWC { get; set; }

        [MaxLength(100)]
        public string? AgreementNumber { get; set;}
    }
}
