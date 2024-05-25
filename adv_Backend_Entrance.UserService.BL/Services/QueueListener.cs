using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.UserService.BL.Services;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;
using adv_Backend_Entrance.Common.DTO.AdminPanel;
using adv_Backend_Entrance.Common.DTO.EntranceService.Manager;

namespace adv_Backend_Entrance.UserService.BL.Services
{
    public static class QueueListener
    {
        public static void QueueSubscribe(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var bus = RabbitHutch.CreateBus("host=localhost");
            var additionService = serviceProvider.GetRequiredService<AdditionTokenService>();
            var userService = serviceProvider.GetRequiredService<IUserService>();
            bus.Rpc.Respond<GetUsersMVCDTO, GetUsersPageDTO>(async request =>
            {
                return await userService.GetQuerybleUsers(request.Page,request.Size,request.Email,request.Lastname,request.Firstname);
            }, x => x.WithQueueName("gettingUsers_withMvc"));
            bus.PubSub.Subscribe<AddRoleDTO>("application_created", async data =>
            {
                await additionService.AddRoleToUser(data.Role, data.UserId);
            });
            bus.Rpc.Respond<Guid, UserGetProfileDTO>(async request =>
            {
                return await userService.GetProfile(request);
            }, x => x.WithQueueName("application_profileResponse"));
            bus.Rpc.Respond<UserLoginDTO, TokenResponseDTO>(async request =>
            {
                return await userService.UserLogin(request);
            }, x => x.WithQueueName("userLogin_token"));
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("editUserProfile", async data =>
            {
                var editProfile = new EditUserProfileDTO
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Patronymic = data.Patronymic,
                    Gender = data.Gender,
                    Phone = data.Phone,
                    Email = data.Email,
                    BirthDate = data.BirthDate
                };
                await userService.EditProfile(data.UserId, editProfile);
            });
            bus.PubSub.Subscribe<EditApplicantProfileInformationDTO>("applicantEditProfileMVC", async data =>
            {
                var editProfile = new EditUserProfileDTO
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Patronymic = data.Patronymic,
                    Gender = data.Gender,
                    Phone = data.Phone,
                    Email = data.Email,
                    BirthDate = data.BirthDate
                };
                await userService.EditProfile(data.UserId, editProfile);
            });
            bus.PubSub.Subscribe<string>("logoutUser", async data =>
            {
                await userService.Logout(data);
            });
            bus.PubSub.Subscribe<AddRoleUserMVCDTO>("addRoleToUserMVC", async data =>
            {
                await additionService.AddRoleToUser(data.Role, data.UserId);
            });
            bus.PubSub.Subscribe<RemoveRoleUserMVCDTO>("removeRoleToUserMVC", async data =>
            {
                await additionService.RemoveRoleFromUser(data.Role, data.UserId);
            });
            bus.PubSub.Subscribe<ChangePasswordMVCDTO>("changePassword", async data =>
            {
                var passswordData = new changePasswordDTO
                {
                    ConfirmPassword = data.ConfirmPassword,
                    Password = data.Password,
                    OldPasssword = data.OldPasssword
                };
                await userService.ChangePassword(data.UserId, passswordData);
            });
            bus.Rpc.Respond<GetUserProfileMVCDTO, UserGetProfileDTO>(async request =>
            {
                return await userService.GetProfile(request.UserId);
            }, x => x.WithQueueName("getUserProfileMVC"));
        }
    }
}
