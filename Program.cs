using Microsoft.AspNetCore.Identity;
using BookSwap.Models;
using Microsoft.EntityFrameworkCore;
using BookSwap.Data;
using BookSwap.Models.Interfaces;
using BookSwap.Models.Repositories;
using BookSwap.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
});
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapHub<StockHub>("/stockHub");

app.Run();



