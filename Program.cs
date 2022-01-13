using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibraryMs.Data;
using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Identity;
using LibraryMs.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibraryMsContext");

builder.Services.AddDbContext<LibraryMsContext>(
    options => options.UseSqlServer(connectionString)
    );
builder.Services.AddDbContext<UserDbContext>(
        options => options.UseSqlServer(connectionString)
    );


builder.Services.AddDefaultIdentity<ApplicationUser>(
    options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization( options => {
    options.AddPolicy("AllUsers",
    policy => policy.RequireRole("Principal", "Admin", "Librarian", "Student"));
    options.AddPolicy("LibraryUsers",
      policy => policy.RequireRole("Librarian", "Student"));
    options.AddPolicy("PrincipalUsers",
      policy => policy.RequireRole("Principal"));
    options.AddPolicy("Admin&PrincipalUsers",
      policy => policy.RequireRole("Principal", "Admin"));
    options.AddPolicy("AdminUsers",
      policy => policy.RequireRole("Admin"));
});



/*
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
*/
// Add services to the container.
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});




builder.Services.AddControllersWithViews();



builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
   options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
   options.Lockout.MaxFailedAccessAttempts = 5;
   options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

   options.LoginPath = "/Identity/Account/Login";
   options.AccessDeniedPath = "/Identity/Account/AccessDenied";
   options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/");
    // The default HSTS value is 30 days. You may want to change this
    // for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Borrowings}/{action=Index}/{id?}");


app.Run();
