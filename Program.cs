using JV_Imprest_Payment.Data;
using JV_Imprest_Payment.Models;
using JV_Imprest_Payment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Configure email confirmation options
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Add the service registration for IMailSender and its implementation
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Create roles if they don't exist
    if (!await roleManager.RoleExistsAsync("AFE"))
    {
        var role = new IdentityRole
        {
            Name = "AFE"
        };
        await roleManager.CreateAsync(role);
    }

    if (!await roleManager.RoleExistsAsync("Partner"))
    {
        var role = new IdentityRole
        {
            Name = "Partner"
        };
        await roleManager.CreateAsync(role);
    }

    if (!await roleManager.RoleExistsAsync("JVA"))
    {
        var role = new IdentityRole
        {
            Name = "JVA"
        };
        await roleManager.CreateAsync(role);
    }
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var role = new IdentityRole
        {
            Name = "Admin"
        };
        await roleManager.CreateAsync(role);
    }

    // Assign roles to users if needed
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var user = await userManager.FindByEmailAsync("oluwasenijbd@gmail.com");
    if (user != null)
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    pattern: "{controller=PayRequests}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
