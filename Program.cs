using Microsoft.AspNetCore.Identity;
using BookSwap.Models;
using Microsoft.EntityFrameworkCore;
using BookSwap.Data;
using Rotativa.AspNetCore;
using BookSwap.Models.Interfaces;
using BookSwap.Models.Repositories;
using BookSwap.Hubs;
using BookSwap.Models.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DBHelper>();
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
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SellerOnly", policy => policy.RequireRole("Seller"));
    
    options.AddPolicy("BuyerOnly", policy => policy.RequireRole("Buyer"));
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Buyer/AccessDenied";   
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

    async Task SeedAdminAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string adminRole = "Admin";
        string adminUser = "admin";
        string adminPass = "admin123";

        // Create role if not exists
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        // Create admin user if not exists
        var user = await userManager.FindByNameAsync(adminUser);
        if (user == null)
        {
            user = new ApplicationUser { UserName = adminUser };
            await userManager.CreateAsync(user, adminPass);
            await userManager.AddToRoleAsync(user, adminRole);
        }
    }

    using (var scope = app.Services.CreateScope())
    {
        await SeedAdminAsync(scope.ServiceProvider);
    }

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapHub<StockHub>("/stockHub");
app.MapHub<SellerHub>("/sellerHub");
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.Run();



