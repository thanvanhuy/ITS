using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VVA.ITS.WebApp.Data;
using VVA.ITS.WebApp.Repository;
using VVA.ITS.WebApp.Models;
using VVA.ITS.WebApp.Interfaces;
using VVA.ITS.WebApp.Hubs;
using VVA.ITS.WebApp.SubscribeTableDependencies;
using VVA.ITS.WebApp.MiddlewareExtensions;
using DinkToPdf.Contracts;
using DinkToPdf;
using VVA.ITS.WebApp.Services;

// Real-time update data:
// Part 1: https://www.c-sharpcorner.com/article/how-to-receive-real-time-data-in-net-6-client-application-using-signalr/
// Part 2: https://www.c-sharpcorner.com/article/how-to-receive-real-time-data-in-an-asp-net-core-client-application-using-signal/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//}, ServiceLifetime.Singleton);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}, ServiceLifetime.Scoped);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IIdentityRoleRepository, IdentityRoleRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDataxe, DataxeReponsitory>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Account/Login");
builder.Services.AddSignalR();
//builder.Services.AddSingleton<DashboardHub>();
builder.Services.AddScoped<DashboardHub>();
builder.Services.AddSingleton<SubscribeVehicleTableDependency>();
builder.Services.AddSingleton<IPdfService, PdfService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    await Seed.SeedUsersAndRolesAsync(app);
    Seed.SeedData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//// Initialize Data
//using (var scope = app.Services.CreateScope())
//{
//	var services = scope.ServiceProvider;
//	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//	DbInitializer.InitializeRole(roleManager);
//	var userManager = services.GetRequiredService<UserManager<AppUser>>();
//	DbInitializer.InitializeUser(userManager);
//}

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Index}/{ToastrMessage?}");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Role}/{action=Index}/{ToastrMessage?}");

app.MapHub<DashboardHub>("/dashboardHub");
app.UseSqlTableDependency<SubscribeVehicleTableDependency>(connectionString);
app.Run();
