﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(WorksDbContext))]
    partial class WorksDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Work", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AgreementNumber")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Controlled")
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("IPI")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ISWC")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("InWorkMR")
                        .HasColumnType("int");

                    b.Property<int?>("InWorkPR")
                        .HasColumnType("int");

                    b.Property<string>("Language")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RecordCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("Role")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("SenderWorkCode")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ShareHolder")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Title")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ID");

                    b.ToTable("Works", (string)null);

                    b.HasData(
                        new
                        {
                            ID = 1,
                            AgreementNumber = "3573330000005",
                            Controlled = "Y",
                            IPI = "783755784",
                            ISWC = "T9330044821",
                            InWorkMR = 30,
                            InWorkPR = 30,
                            Language = "EN",
                            RecordCode = "U",
                            Role = "K",
                            SenderWorkCode = "4683465",
                            ShareHolder = "P",
                            Title = "TEST"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
