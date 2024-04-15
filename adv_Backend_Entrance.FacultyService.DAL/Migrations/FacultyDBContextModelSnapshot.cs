﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using adv_Backend_Entrance.FacultyService.MVCPanel.Data;

#nullable disable

namespace adv_Backend_Entrance.FacultyService.DAL.Migrations
{
    [DbContext(typeof(FacultyDBContext))]
    partial class FacultyDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.DAL.Data.Models.EducationDocumentTypeNextEducationLevel", b =>
                {
                    b.Property<Guid>("EducationDocumentTypeId")
                        .HasColumnType("uuid");

                    b.Property<int>("EducationLevelId")
                        .HasColumnType("integer");

                    b.Property<string>("EducationLevelName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EducationDocumentTypeId", "EducationLevelId");

                    b.HasIndex("EducationLevelId");

                    b.ToTable("EducationDocumentTypeNextEducationLevels");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.DAL.Data.Models.Import", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Imports");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.EducationDocumentTypeModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EducationLevelEnum")
                        .HasColumnType("integer");

                    b.Property<int>("EducationLevelId")
                        .HasColumnType("integer");

                    b.Property<string>("EducationLevelName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EducationDocumentTypes");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.EducationLevelModel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<int>("EducationLevelName")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EducationLevelModels");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.EducationProgrammModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EducationForm")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EducationFormEnum")
                        .HasColumnType("integer");

                    b.Property<int>("EducationLevelEnum")
                        .HasColumnType("integer");

                    b.Property<int>("EducationLevelId")
                        .HasColumnType("integer");

                    b.Property<string>("EducationLevelName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("FacultyCreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("FacultyId")
                        .HasColumnType("uuid");

                    b.Property<string>("FacultyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LanguageEnum")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EducationProgrammModels");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.FacultyModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("FacultyModels");
                });

            modelBuilder.Entity("adv_Backend_Entrance.FacultyService.DAL.Data.Models.EducationDocumentTypeNextEducationLevel", b =>
                {
                    b.HasOne("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.EducationDocumentTypeModel", "EducationDocumentType")
                        .WithMany()
                        .HasForeignKey("EducationDocumentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("adv_Backend_Entrance.FacultyService.MVCPanel.Data.Models.EducationLevelModel", "EducationLevel")
                        .WithMany()
                        .HasForeignKey("EducationLevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EducationDocumentType");

                    b.Navigation("EducationLevel");
                });
#pragma warning restore 612, 618
        }
    }
}
