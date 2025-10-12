using Microsoft.EntityFrameworkCore;
using CampusLearn.Data;
using CampusLearn.Hubs;
using CampusLearn.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Use PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Other service registrations
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

// Map routes and SignalR hub
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chathub");

app.Run();
