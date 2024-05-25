using adv_Backend_Entrance.Common.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<TokenHelper>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Добавляем поддержку сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Добавляем логгирование
builder.Services.AddLogging();

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

app.Run();
