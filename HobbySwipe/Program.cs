//dotnet ef dbcontext scaffold “Server=tcp:hobbyswipe-test-eastus.database.windows.net,1433;Initial Catalog=HobbySwipe;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;” Microsoft.EntityFrameworkCore.SqlServer -o Entities

using AutoMapper;
using HobbySwipe.Data.Entities.Authentication;
using HobbySwipe.Data.Entities.HobbySwipe;
using HobbySwipe.Data.Repositories;
using HobbySwipe.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Configuration
    .SetBasePath(environment.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();


// Add automapper service.
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMappingService());
});

IMapper autoMapper = mappingConfig.CreateMapper();

// Add connection strings.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("AuthenticationConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<HobbySwipeContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("HobbySwipeConnection")), ServiceLifetime.Scoped);

// Add Identity authentication.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Google authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = File.ReadAllLines(configuration["Authentication:Google:ClientId"])[0];
        options.ClientSecret = File.ReadAllLines(configuration["Authentication:Google:ClientSecret"])[0];
    });


// Add services to the container.
builder.Services.AddSingleton<IConfigurationRoot>(configuration);
builder.Services.AddSingleton(autoMapper);
builder.Services.AddScoped<IHobbySwipeRepository, HobbySwipeRepository>();
builder.Services.AddControllersWithViews();

// Setup options for Identity.
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

// Set the expiration for tokens.
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(48));

// Setup the cookie the user receives.
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
