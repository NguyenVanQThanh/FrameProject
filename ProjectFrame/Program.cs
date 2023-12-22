using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Utility.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager<SignInManager<IdentityUser>>()
    .AddDefaultTokenProviders();

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
//        options.SlidingExpiration = true;
//        options.AccessDeniedPath = "/Forbidden/";
//    });
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


//builder.Services.AddIdentity<IdentityUser,IdentityRole>()
//    .AddEntityFrameworkStores<AdminApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("AuthMessageSender"));

var app = builder.Build();

//Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
}

else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseMiddleware<RedirectToDashboardMiddleware>();
app.UseRouting();




app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Admin",
    pattern: "admin/{area=Admin}/{controller=Customer}/{action=Index}/{id?}");

app.Run();
