using CampusLearn.Data;
using CampusLearn.Hubs;
using CampusLearn.Services;
using CampusLearn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
options.Password.RequireNonAlphanumeric=false;
options.Password.RequiredLength = 8;
options.Password.RequireUppercase=false;
options.Password.RequireLowercase=false;
options.User.RequireUniqueEmail=true;
options.SignIn.RequireConfirmedAccount = false;
options.SignIn.RequireConfirmedEmail=false;



})
  .AddEntityFrameworkStores< AppDbContext>()
  .AddDefaultTokenProviders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Add SignalR
builder.Services.AddSignalR();

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Enable WebSockets
app.UseWebSockets();

// Map routes and hubs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chathub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
