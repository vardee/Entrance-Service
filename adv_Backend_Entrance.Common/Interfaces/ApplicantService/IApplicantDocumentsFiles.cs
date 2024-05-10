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
        Task UploadPassportFile(AddFileDTO addFileDTO, Guid userId);
        Task UploadEducationDocumentFile(AddFileDTO addFileDTO, Guid userId);
        Task<byte[]> GetPassportFile(Guid userId);
        Task<byte[]> GetEducationDocumentFile(Guid userId);
        Task DeletePassport(Guid userId);  
        Task DeleteEducationLevel(Guid userId);
    }
}
