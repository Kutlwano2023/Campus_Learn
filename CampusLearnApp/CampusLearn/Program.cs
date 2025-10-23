using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Models;
using CampusLearn.Hubs;
using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure MongoDB settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings")
);

// Get MongoDB configuration
var connectionString = builder.Configuration.GetSection("MongoDBSettings:ConnectionString").Value;
var databaseName = builder.Configuration.GetSection("MongoDBSettings:DatabaseName").Value;

// Add Identity with MongoDB
builder.Services.AddIdentity<Users, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddMongoDbStores<Users, ApplicationRole, Guid>(connectionString, databaseName)
.AddDefaultTokenProviders();

// Add MVC and SignalR services
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services
builder.Services.AddScoped<MongoService>();
builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddScoped<UserService>();

// Add HttpContextAccessor for accessing current user in services
builder.Services.AddHttpContextAccessor();

// Build the application
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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<MessagingHub>("/messagingHub");

// Seed roles and test connection
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync();

    // Test MongoDB connection
    var mongoService = scope.ServiceProvider.GetRequiredService<MongoService>();
    try
    {
        var usersCount = await mongoService.Users.CountDocumentsAsync(Builders<Users>.Filter.Empty);
        Console.WriteLine($"MongoDB connection successful. Users in database: {usersCount}");

        // Ensure messages collection exists and create indexes
        var database = mongoService.Users.Database;
        var messagesCollection = database.GetCollection<Message>("messages");

        // Create indexes for better performance
        var senderIndex = Builders<Message>.IndexKeys.Ascending(m => m.SenderId);
        var receiverIndex = Builders<Message>.IndexKeys.Ascending(m => m.ReceiverId);
        var sentAtIndex = Builders<Message>.IndexKeys.Descending(m => m.SentAt);
        var isReadIndex = Builders<Message>.IndexKeys.Ascending(m => m.IsRead);

        var indexKeys = Builders<Message>.IndexKeys
            .Combine(senderIndex, receiverIndex, sentAtIndex);

        await messagesCollection.Indexes.CreateOneAsync(new CreateIndexModel<Message>(indexKeys));
        await messagesCollection.Indexes.CreateOneAsync(new CreateIndexModel<Message>(isReadIndex));

        Console.WriteLine("Message collection indexes created successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"MongoDB connection failed: {ex.Message}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

//Contributors
//Magane Letsoalo (601101) 
//Reagile Motsepe (600665)
//Yanga Mazibuko (600459)
//Vunene Khoza (600676)
//Onalerona Lefoka (600453)
//Kutlwano Thaga (601349)