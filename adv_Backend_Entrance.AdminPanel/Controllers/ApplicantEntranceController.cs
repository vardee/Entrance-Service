using adv_Backend_Entrance.AdminPanel.Models;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Helpers;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace adv_Backend_Entrance.AdminPanel.Controllers
{
    public class ApplicantEntranceController : Controller
    {
        private readonly ILogger<ApplicantEntranceController> _logger;
        private readonly IBus _bus;
        private readonly TokenHelper _tokenHelper;

        public ApplicantEntranceController(ILogger<ApplicantEntranceController> logger, TokenHelper tokenHelper)
        {
            _logger = logger;
            _bus = RabbitHutch.CreateBus("host=localhost");
            _tokenHelper = tokenHelper;
        }
        [HttpGet]
        public async Task<IActionResult> ApplicantEntrance(Guid userId)
        {
            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(userId, c => c.WithQueueName("getApplicantInformationMVC"));
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId,
                };
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var passportProfile = await GetPassportInformationWithErrorHandling(response.UserId);
                var educationLevelProfile = await GetEducationInformationWithErrorHandling(response.UserId); // Обработка ошибок для информации об образовании
                var viewModel = new ApplicantEntranceViewModel
                {
                    Id = userId,
                    PassportModel = new PassportModel
                    {
                        BirthPlace = passportProfile.birthPlace,
                        IssuedWhen = passportProfile.issuedWhen,
                        IssuedWhom = passportProfile.issuedWhom,
                        PassportNumber = passportProfile.passportNumber,
                    },
                    EducationDocumentModel = new EducationDocumentModel
                    {
                        EducationLevel = educationLevelProfile.EducationLevel,
                        id = educationLevelProfile.id,
                    }
                };

                return View("ApplicantEntrance", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving applicant information");
                return View("Error");
            }
        }

        private async Task<GetEducationInformationDTO> GetEducationInformationWithErrorHandling(Guid userId)
        {
            try
            {
                return await _bus.Rpc.RequestAsync<Guid, GetEducationInformationDTO>(userId, c => c.WithQueueName("getEducationLevelProfileMVC"));
            }
            catch (EasyNetQResponderException ex)
            {
                _logger.LogError(ex, "Education information not found");
                return new GetEducationInformationDTO();
            }
        }


        private async Task<GetPassportInformationDTO> GetPassportInformationWithErrorHandling(Guid userId)
        {
            try
            {
                return await _bus.Rpc.RequestAsync<Guid, GetPassportInformationDTO>(userId, c => c.WithQueueName("getPassportProfileMVC"));
            }
            catch (EasyNetQResponderException ex)
            {
                _logger.LogError(ex, "Passport information not found");
                return new GetPassportInformationDTO();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditPassport")]
        public async Task<IActionResult> EditPassport(PassportModel model,Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View("ApplicantEntrance", model);
            }

            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(id, c => c.WithQueueName("getApplicantInformationMVC"));
                var passportInfo = new AddPassportMVCDTO
                {
                    Id = response.UserId,
                    IssuedWhen = model.IssuedWhen,
                    IssuedWhom = model.IssuedWhom,
                    BirthPlace = model.BirthPlace,
                    PassportNumber = model.PassportNumber,
                };
                await _bus.PubSub.PublishAsync(passportInfo, "addApplicantPassportMVC");
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId,
                };
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var passportProfile = await _bus.Rpc.RequestAsync<Guid, GetPassportInformationDTO>(response.UserId, c => c.WithQueueName("getPassportProfileMVC"));
                var educationLevelProfile = await _bus.Rpc.RequestAsync<Guid, GetEducationInformationDTO>(response.UserId, c => c.WithQueueName("getEducationLevelProfileMVC"));
                var viewModel = new ApplicantEntranceViewModel
                {
                    Id = id,
                    PassportModel = new PassportModel
                    {
                        BirthPlace = passportProfile.birthPlace,
                        IssuedWhen = passportProfile.issuedWhen,
                        IssuedWhom = passportProfile.issuedWhom,
                        PassportNumber = passportProfile.passportNumber,
                    },
                    EducationDocumentModel = new EducationDocumentModel
                    {
                        EducationLevel = educationLevelProfile.EducationLevel,
                        id = educationLevelProfile.id,
                    }
                };

                return View("ApplicantEntrance", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during passport edit process");
                ModelState.AddModelError("", "Error during passport edit.");
                return PartialView("ApplicantEntrance", model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditEducationDocument")]
        public async Task<IActionResult> EditEducationDocument(EducationDocumentModel model, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View("ApplicantEntrance", model);
            }

            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(id, c => c.WithQueueName("getApplicantInformationMVC"));
                var educationInfo = new AddEducationDocumentMVCDTO
                {
                    Id = id,
                    EducationLevel = model.EducationLevel
                };
                await _bus.PubSub.PublishAsync(educationInfo, "addApplicantEducationDocumentMVC");
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId,
                };
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var passportProfile = await _bus.Rpc.RequestAsync<Guid, GetPassportInformationDTO>(response.UserId, c => c.WithQueueName("getPassportProfileMVC"));
                var educationLevelProfile = await _bus.Rpc.RequestAsync<Guid, GetEducationInformationDTO>(response.UserId, c => c.WithQueueName("getEducationLevelProfileMVC"));
                var viewModel = new ApplicantEntranceViewModel
                {
                    Id = id,
                    PassportModel = new PassportModel
                    {
                        BirthPlace = passportProfile.birthPlace,
                        IssuedWhen = passportProfile.issuedWhen,
                        IssuedWhom = passportProfile.issuedWhom,
                        PassportNumber = passportProfile.passportNumber,
                    },
                    EducationDocumentModel = new EducationDocumentModel
                    {
                        EducationLevel = educationLevelProfile.EducationLevel,
                        id = educationLevelProfile.id,
                    }
                };

                return View("ApplicantEntrance", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during passport edit process");
                ModelState.AddModelError("", "Error during passport edit.");
                return PartialView("ApplicantEntrance", model);
            }
        }
        [HttpPost]
        [Route("DeletePassport")]
        public async Task<IActionResult> DeletePassport(Guid id)
        {
            try
            {
                var response = await _bus.Rpc.RequestAsync<Guid, GetApplicantInformationDTO>(id, c => c.WithQueueName("getApplicantInformationMVC"));
                var request = new GetUserProfileMVCDTO
                {
                    UserId = response.UserId,
                };
                var requestPassport = new DeletePassportMVCDTO
                {
                    UserId = response.UserId,
                };
                await _bus.PubSub.PublishAsync(requestPassport, "deletePassportMVC");
                var applicantProfile = await _bus.Rpc.RequestAsync<GetUserProfileMVCDTO, UserGetProfileDTO>(request, c => c.WithQueueName("getUserProfileMVC"));
                var passportProfile = await GetPassportInformationWithErrorHandling(response.UserId);
                var educationLevelProfile = await GetEducationInformationWithErrorHandling(response.UserId);
                var viewModel = new ApplicantEntranceViewModel
                {
                    Id = id,
                    PassportModel = new PassportModel
                    {
                        BirthPlace = passportProfile.birthPlace,
                        IssuedWhen = passportProfile.issuedWhen,
                        IssuedWhom = passportProfile.issuedWhom,
                        PassportNumber = passportProfile.passportNumber,
                    },
                    EducationDocumentModel = new EducationDocumentModel
                    {
                        EducationLevel = educationLevelProfile.EducationLevel,
                        id = educationLevelProfile.id,
                    }
                };

                return View("ApplicantEntrance", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during rejecting application");
                return Json(new { success = false, message = "Error rejecting application." });
            }
        }
    }
}
