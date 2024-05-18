using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using adv_Backend_Entrance.Common.DTO.UserService;
using adv_Backend_Entrance.Common.Interfaces.UserService;
using adv_Backend_Entrance.UserService.BL.Services;
using adv_Backend_Entrance.Common.DTO.ApplicantService;
using adv_Backend_Entrance.Common.DTO.UserService.ManagerAccountService;

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
        }
    }
}
