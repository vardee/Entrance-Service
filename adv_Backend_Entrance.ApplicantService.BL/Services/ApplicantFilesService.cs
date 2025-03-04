﻿using adv_Backend_Entrance.ApplicantService.DAL.Data;
using adv_Backend_Entrance.ApplicantService.DAL.Data.Entites;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Interfaces.ApplicantService;
using adv_Backend_Entrance.Common.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace adv_Backend_Entrance.ApplicantService.BL.Services
{
    public class ApplicantFilesService : IApplicantDocumentsFiles
    {
        private readonly ApplicantDBContext _applicantDBContext;
        private readonly IConfiguration _configuration;
        private readonly string _filePassportDirectory;
        private readonly string _fileEducationDocumentDirectory;
        private readonly TokenHelper _tokenHelper;

        public ApplicantFilesService(ApplicantDBContext applicantDBContext, IConfiguration configuration, TokenHelper tokenHelper)
        {
            _applicantDBContext = applicantDBContext;
            _configuration = configuration;
            _tokenHelper = tokenHelper;
            _filePassportDirectory = @"C:\Users\rusma\Documents\Files\FilesPassports";
            _fileEducationDocumentDirectory = @"C:\Users\rusma\Documents\Files\FilesEducationDocuments";
        }
        public async Task UploadEducationDocumentFile(AddFileDTO addFileDTO, Guid userId)
        {
            var education = await _applicantDBContext.EducationDocuments.FirstOrDefaultAsync(p => p.UserId == userId);
            if (addFileDTO.FormFile == null || addFileDTO.FormFile.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }
            string uniqueFileName = education.Id.ToString() + Path.GetExtension(addFileDTO.FormFile.FileName);
            string filePath = Path.Combine(_fileEducationDocumentDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await addFileDTO.FormFile.CopyToAsync(stream);
            }
            var educationScan = new EducationDocumentImportFile
            {
                UserId = userId,
                Path = filePath,
            };
            _applicantDBContext.educationDocumentImportFiles.Add(educationScan);
            education.FileId = educationScan.Id;
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task UploadPassportFile(AddFileDTO addFileDTO, Guid userId)
        {
            var passport = await _applicantDBContext.Passports.FirstOrDefaultAsync(p => p.UserId == userId);
            if (addFileDTO.FormFile == null || addFileDTO.FormFile.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }
            string uniqueFileName = passport.PassportNumber + Path.GetExtension(addFileDTO.FormFile.FileName);
            string filePath = Path.Combine(_filePassportDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await addFileDTO.FormFile.CopyToAsync(stream);
            }
            var passportScan = new PassportImportFile
            {
                UserId = userId,
                Path = filePath,
            };
            _applicantDBContext.passportImportFiles.Add(passportScan);
            passport.FileId = passportScan.Id;
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task<byte[]> GetEducationDocumentFile(Guid userId)
        {
            var educationDocument = await _applicantDBContext.educationDocumentImportFiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (educationDocument == null || string.IsNullOrEmpty(educationDocument.Path))
            {
                throw new NotFoundException("File not found.");
            }
            byte[] fileBytes = await File.ReadAllBytesAsync(educationDocument.Path);

            return fileBytes;
        }
        public async Task<byte[]> GetPassportFile(Guid userId)
        {
            var passportDocument = await _applicantDBContext.passportImportFiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (passportDocument == null || string.IsNullOrEmpty(passportDocument.Path))
            {
                throw new NotFoundException("Passport file not found.");
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(passportDocument.Path);

            return fileBytes;
        }

        public async Task DeletePassport(Guid userId)
        {
            var passportDocument = await _applicantDBContext.passportImportFiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (passportDocument == null || string.IsNullOrEmpty(passportDocument.Path))
            {
                throw new FileNotFoundException("Passport file not found.");
            }
            if (File.Exists(passportDocument.Path))
            {
                File.Delete(passportDocument.Path);
            }
            else
            {
                throw new NotFoundException("Education document file not found.");
            }

            File.Delete(passportDocument.Path);
            _applicantDBContext.passportImportFiles.Remove(passportDocument);
            await _applicantDBContext.SaveChangesAsync();
        }

        public async Task DeleteEducationLevel(Guid userId)
        {
            var educationDocument = await _applicantDBContext.educationDocumentImportFiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (educationDocument == null || string.IsNullOrEmpty(educationDocument.Path))
            {
                throw new NotFoundException("Education document file not found.");
            }
            if (File.Exists(educationDocument.Path))
            {
                File.Delete(educationDocument.Path);
            }
            else
            {
                throw new NotFoundException("Education document file not found.");
            }

            File.Delete(educationDocument.Path);
            _applicantDBContext.educationDocumentImportFiles.Remove(educationDocument);
            await _applicantDBContext.SaveChangesAsync();
        }

    }
}
