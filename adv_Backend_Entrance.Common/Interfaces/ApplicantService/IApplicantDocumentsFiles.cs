using adv_Backend_Entrance.Common.DTO.ApplicantService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Interfaces.ApplicantService
{
    public interface IApplicantDocumentsFiles
    {
        Task UploadPassportFile(AddFileDTO addFileDTO,string token);
        Task UploadEducationDocumentFile(AddFileDTO addFileDTO, string token);
        Task<byte[]> GetPassportFile(string token);
        Task<byte[]> GetEducationDocumentFile(string token);
        Task DeletePassport(string token);  
        Task DeleteEducationLevel(string token);
    }
}
