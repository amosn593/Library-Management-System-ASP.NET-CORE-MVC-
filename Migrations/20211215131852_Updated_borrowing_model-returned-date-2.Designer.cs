﻿// <auto-generated />
using System;
using LibraryMs.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LibraryMs.Migrations
{
    [DbContext(typeof(LibraryMsContext))]
    [Migration("20211215131852_Updated_borrowing_model-returned-date-2")]
    partial class Updated_borrowing_modelreturneddate2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("LibraryMs.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("FormID")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FormID");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("LibraryMs.Models.Borrowing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<string>("Issued")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ReturnedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookID");

                    b.HasIndex("StudentID");

                    b.ToTable("Borrowing");
                });

            modelBuilder.Entity("LibraryMs.Models.Form", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Form");
                });

            modelBuilder.Entity("LibraryMs.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AdminNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FormID")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FormID");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("LibraryMs.Models.Book", b =>
                {
                    b.HasOne("LibraryMs.Models.Form", "Form")
                        .WithMany("Books")
                        .HasForeignKey("FormID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("LibraryMs.Models.Borrowing", b =>
                {
                    b.HasOne("LibraryMs.Models.Book", "CurrentBook")
                        .WithMany("Borrowings")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LibraryMs.Models.Student", "CurrentStudent")
                        .WithMany("Borrowings")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentBook");

                    b.Navigation("CurrentStudent");
                });

            modelBuilder.Entity("LibraryMs.Models.Student", b =>
                {
                    b.HasOne("LibraryMs.Models.Form", "Form")
                        .WithMany("Students")
                        .HasForeignKey("FormID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("LibraryMs.Models.Book", b =>
                {
                    b.Navigation("Borrowings");
                });

            modelBuilder.Entity("LibraryMs.Models.Form", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("LibraryMs.Models.Student", b =>
                {
                    b.Navigation("Borrowings");
                });
#pragma warning restore 612, 618
        }
    }
}
