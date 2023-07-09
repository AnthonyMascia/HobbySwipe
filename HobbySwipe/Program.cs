//dotnet ef dbcontext scaffold “Server=tcp:hobbyswipe-test-eastus.database.windows.net,1433;Initial Catalog=HobbySwipe;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;” Microsoft.EntityFrameworkCore.SqlServer -o Entities

using HobbySwipe.Data.Entities;
using HobbySwipe.Data.Repositories;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Configuration
    .SetBasePath(environment.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add connection strings.
builder.Services.AddDbContext<HobbySwipeContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("AzureSQLConnection")), ServiceLifetime.Scoped);

// Add services to the container.
builder.Services.AddScoped<IQuestionsRepository, QuestionsRepository>();
builder.Services.AddControllersWithViews();

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
app.UseAuthorization();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
