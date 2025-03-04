using adv_Backend_Entrance.Common.Helpers.TokenRequirment;
using adv_Backend_Entrance.Common.Middlewares;
using adv_Backend_Entrance.Notificantion.BL.Configuration;
using adv_Backend_Entrance.Common.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using adv_Backend_Entrance_Notification.BL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    var enumConverter = new JsonStringEnumConverter();
    opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

// Add business logic service dependencies
builder.Services.AddNotificationBlServiceDependencies(builder.Configuration);


builder.Services.QueueNotificationSubscribe();
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
// Add HttpClient
builder.Services.AddHttpClient();
// Create JWT
// Build the application
var app = builder.Build();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add middleware for exception handling
app.UseMiddleware<ExceptionMiddleware>();

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();


// Run the application
app.Run();
