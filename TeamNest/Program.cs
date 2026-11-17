using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamNest.Data;
using TeamNest.Identity;
using TeamNest.Models;
using TeamNest.Models.Entities;
using TeamNest.Repositories;
using TeamNest.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// ---------------------------------------------------------------
// 1. EF Core: used ONLY for Identity tables and for creating the
//    database schema with migrations. App data goes through Dapper.
// ---------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------------
// 2. Identity with our customized user (ApplicationUser)
// ---------------------------------------------------------------
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    // Adds OrganizationId and FullName to the login cookie as claims
    .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// ---------------------------------------------------------------
// 3. Authorization policies (who can do what)
// ---------------------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppPolicies.CanManageProjects,
        policy => policy.RequireRole(AppRoles.Owner, AppRoles.Admin));

    options.AddPolicy(AppPolicies.OwnerOnly,
        policy => policy.RequireRole(AppRoles.Owner));
});

// AJAX requests send the anti-forgery token in this header
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

// ---------------------------------------------------------------
// 4. Our own services (Dependency Injection)
// ---------------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddControllersWithViews(options =>
{
    // Every POST/PUT/DELETE must carry a valid anti-forgery token
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

var app = builder.Build();

// Create the Owner / Admin / Member roles if they don't exist yet
await DbSeeder.SeedRolesAsync(app.Services);

// ---------------------------------------------------------------
// 5. HTTP request pipeline (middleware order matters!)
// ---------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
