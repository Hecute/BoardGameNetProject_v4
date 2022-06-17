using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BoardGameNetProject_v3.Areas.Identity.Data;
using BoardGameNetProject_v3.Data;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlite(connectionString));;

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<IdentityContext>();;

builder.Services.AddDbContext<SQLiteDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDBContext")));

builder.Services.AddMvc();
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//create role admin
var scope = app.Services
    .GetService<IServiceScopeFactory>()
    ?.CreateScope();

if (scope is not null)
{
    using (scope)
    {
        var roleManager = scope
            .ServiceProvider
            .GetService<RoleManager<IdentityRole>>();

        // .. then use the RoleManager
        var userManager = scope
            .ServiceProvider
            .GetService < UserManager<IdentityUser>>();
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            var role = new IdentityRole();
            role.Name = "Admin";
            roleManager.CreateAsync(role);
        }
        
        var user = new IdentityUser();
        user.UserName = "admin.admin@gmail.com";
        user.Email = "admin.admin@gmail.com";
        user.EmailConfirmed = true;
        var superUser = userManager.CreateAsync(user, "Admin123!@#");
        if (superUser.Result.Succeeded)
        {
            userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();