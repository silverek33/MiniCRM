using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// EF Core + SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity + RoleManager
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Serwisy DI
builder.Services.AddScoped<IEmailService, FakeEmailService>();

var app = builder.Build();

// Zastosuj migracje i wykonaj seeding ról/admina przed kontynuacj¹ uruchomienia
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    // Jeœli nie ma migracji zastosuj je — to utworzy brakuj¹ce tabele (np. AspNetRoles)
    await db.Database.MigrateAsync();

    await DbSeeder.SeedRolesAndAdmin(services);
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contacts}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();