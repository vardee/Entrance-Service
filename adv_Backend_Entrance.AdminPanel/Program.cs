using adv_Backend_Entrance.AdminPanel.Configurations;
using adv_Backend_Entrance.Common.Data;
using adv_Backend_Entrance.Common.Helpers;
using adv_Backend_Entrance.Common.Helpers.TokenRequirment;
using adv_Backend_Entrance.Common.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAdminPanelConfiguration(builder.Configuration);
builder.Services.AddControllersWithViews();

// Добавляем поддержку сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Добавляем логгирование
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization(services =>
{
    services.AddPolicy("TokenNotInBlackList", policy => policy.Requirements.Add(new TokenBlackListRequirment()));
});
builder.Services.AddSingleton<IAuthorizationHandler, TokenInBlackListHandler>();
builder.Services.AddAuthenticationWithSession(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Добавляем поддержку сессий в middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ApplicantProfile",
    pattern: "ApplicantProfile/{userId:Guid}",
    defaults: new { controller = "ApplicantProfile", action = "ApplicantProfile" }
);

app.MapControllerRoute(
    name: "ApplicantEntrance",
    pattern: "ApplicantEntrance/{action}/{userId:Guid?}",
    defaults: new { controller = "ApplicantEntrance", action = "Index" }
);

app.MapControllerRoute(
    name: "ManagerProfile",
    pattern: "ManagerProfile/{action}/{managerId:Guid?}",
    defaults: new { controller = "ManagerProfile", action = "Index" }
);

app.Run();
