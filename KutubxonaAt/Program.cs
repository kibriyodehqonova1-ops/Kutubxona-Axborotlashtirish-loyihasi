using KutubxonaAt.Data;
using KutubxonaAt.Models;
using KutubxonaAt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Application services
builder.Services.AddScoped<IBildirishnomaService, BildirishnomaService>();
builder.Services.AddScoped<AutoBerishService>();

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// Session support (CartController uchun zarur)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    var timeoutMinutes = builder.Configuration.GetValue<int>("Session:IdleTimeoutMinutes", 60);
    options.IdleTimeout = TimeSpan.FromMinutes(timeoutMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Session middleware (UseRouting dan keyin bo'lishi kerak)
app.UseSession();

// Area route must come before default route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database and roles (asynchronous)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // LocalDB avtomatik ishga tushirish (retry bilan)
    const int maxRetries = 3;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            break; // Muvaffaqiyatli bo'lsa chiqamiz
        }
        catch (Exception ex) when (attempt < maxRetries && ex.Message.Contains("LocalDB"))
        {
            logger.LogWarning("LocalDB ishlamayapti. Qayta ishga tushirilmoqda... (urinish {Attempt}/{Max})", attempt, maxRetries);
            EnsureLocalDbRunning();
            await Task.Delay(3000); // 3 soniya kutamiz
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Baza bilan ulanishda xato yuz berdi.");
            throw;
        }
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Librarian", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin foydalanuvchisini config dan olish.
    // Parol kodda yoki appsettings.json da SAQLANMAYDI: user-secrets yoki environment o'zgaruvchisi
    // (AdminSettings__Password) orqali beriladi.
    var adminEmail = builder.Configuration["AdminSettings:Email"];
    var adminPassword = builder.Configuration["AdminSettings:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
    {
        logger.LogWarning("AdminSettings:Email yoki AdminSettings:Password berilmagan - admin foydalanuvchi yaratilmadi. " +
                          "README dagi 'dotnet user-secrets' yo'riqnomasiga qarang.");
    }
    else if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Ism = "Admin",
            Familiya = "Kutubxona",
            Turi = "Admin",
            Faol = true,
            AmalMuddati = DateTime.Now.AddYears(10),
            RoyxatSanasi = DateTime.Now,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Namunaviy (demo) ma'lumotlar faqat Development muhitida yoki SeedDemoData=true bo'lganda yuklanadi
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SeedDemoData"))
    {
        await DbInitializer.SeedAsync(scope.ServiceProvider);
    }
}

app.Run();

// LocalDB ni ishga tushirish funksiyasi
static void EnsureLocalDbRunning()
{
    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sqllocaldb",
                Arguments = "start MSSQLLocalDB",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit(10000); // 10 soniya kutamiz
    }
    catch
    {
        // sqllocaldb topilmasa yoki xato bo'lsa, davom etamiz
    }
}
