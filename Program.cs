using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repository;
using MovieApp.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var server = Environment.GetEnvironmentVariable("DBServer");
var database = Environment.GetEnvironmentVariable("DBName");
var user = Environment.GetEnvironmentVariable("DBUser");
var password = Environment.GetEnvironmentVariable("DBPassword");
var port = Environment.GetEnvironmentVariable("DBPort");
var connectionString = $"Server={server}, {port}; Initial Catalog={database}; User ID={user}; Password={password}";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
        option =>
        {
            option.Password.RequiredLength = 8;
            option.Password.RequireUppercase = false;
            option.Password.RequireLowercase = false;
            option.Password.RequireDigit = false;
            option.Password.RequireNonAlphanumeric = false;
        }
    )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<MovieService>();

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Urls.Add("http://*:8090");
app.Run();
