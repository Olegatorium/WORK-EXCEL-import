using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class WorksDbContext : DbContext
    {
        public WorksDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Work> Works { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Work>().ToTable("Works");

            modelBuilder.Entity<Work>().HasData(
                new Work 
                {
                   ID = 1,
                   SenderWorkCode = "4683465",
                   RecordCode = 'U',
                   Title = "TEST",
                   Role = 'K',
                   ShareHolder = "P",
                   IPI = "783755784",
                   InWorkPR = 30,
                   InWorkMR = 30,
                   Controlled = 'Y',
                   ISWC = "T9330044821",
                   AgreementNumber = "3573330000005"
                }
                  );   
        }
    }
}