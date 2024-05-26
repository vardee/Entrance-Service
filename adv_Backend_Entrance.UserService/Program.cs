using adv_Backend_Entrance.Common.Enums;
using adv_Backend_Entrance.Common.Helpers.TokenRequirment;
using adv_Backend_Entrance.Common.JWT;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.UserService.BL.Configurations;
using adv_Backend_Entrance.UserService.BL.Services;
using adv_Backend_Entrance.UserService.DAL;
using adv_Backend_Entrance.UserService.DAL.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

// Add business logic service dependencies
builder.Services.AddAuthBlServiceDependencies(builder.Configuration);

// Add Identity dependencies configuration
builder.Services.AddIdentityDependenciesConfiguration();

builder.Services.QueueSubscribe();
// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Entrance API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddAuthorization(services =>
{
    services.AddPolicy("TokenNotInBlackList", policy => policy.Requirements.Add(new TokenBlackListRequirment()));
});
builder.Services.AddSingleton<IAuthorizationHandler, TokenInBlackListHandler>();
builder.Services.CreateJWT(builder.Configuration);

// Create JWT
// Build the application
var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<AuthDbContext>();
dbContext?.Database.Migrate();
// Add middleware for exception handling
app.UseMiddleware<ExceptionMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var roles = Enum.GetNames(typeof(RoleType)); 
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
    }
}


// Run the application
app.Run();
