using ClaudeCollectionApp.Data;
using ClaudeCollectionApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(180);
    }));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add HttpClient for external API calls (LMS, Payment Gateway, etc.)
builder.Services.AddHttpClient();

// Add HttpContextAccessor for accessing user context
builder.Services.AddHttpContextAccessor();

// Add Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireRM", policy => policy.RequireRole("RelationshipManager", "SystemAdmin"));
    options.AddPolicy("RequireTeamLeader", policy => policy.RequireRole("TeamLeader", "VerticalHead", "SeniorManagement", "SystemAdmin"));
    options.AddPolicy("RequireVerticalHead", policy => policy.RequireRole("VerticalHead", "SeniorManagement", "SystemAdmin"));
    options.AddPolicy("RequireSeniorManagement", policy => policy.RequireRole("SeniorManagement", "SystemAdmin"));
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("SystemAdmin"));
});

// Register application services
builder.Services.AddScoped<ClaudeCollectionApp.Services.Interfaces.ICaseManagementService, ClaudeCollectionApp.Services.Implementations.CaseManagementService>();
builder.Services.AddScoped<ClaudeCollectionApp.Services.Interfaces.IPromiseToPayService, ClaudeCollectionApp.Services.Implementations.PromiseToPayService>();
builder.Services.AddScoped<ClaudeCollectionApp.Services.Interfaces.IPaymentService, ClaudeCollectionApp.Services.Implementations.PaymentService>();
// TODO: Add remaining services as they are implemented
// builder.Services.AddScoped<ICustomerService, CustomerService>();
// builder.Services.AddScoped<IFieldCollectionService, FieldCollectionService>();
// builder.Services.AddScoped<ILMSIntegrationService, LMSIntegrationService>();
// builder.Services.AddScoped<ICommunicationService, CommunicationService>();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Apply pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        // Seed initial data (roles, admin user, etc.)
        await SeedDataAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session before authentication
app.UseSession();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<ClaudeCollectionApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Data seeding method
static async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
{
    // Create roles if they don't exist
    string[] roleNames = { "SystemAdmin", "SeniorManagement", "VerticalHead", "TeamLeader", "RelationshipManager", "ExternalRecoveryAgent" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
        }
    }

    // Create default admin user if it doesn't exist
    var adminEmail = "admin@nabkisan.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            FullName = "System Administrator",
            EmployeeId = "EMP001",
            UserRole = ClaudeCollectionApp.Models.Enums.UserRole.SystemAdmin,
            IsActive = true,
            JoiningDate = DateTime.UtcNow,
            CanAccessFieldModule = true,
            CanApprovePTPs = true,
            CanProcessPayments = true,
            CanViewReports = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123456");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
        }
    }
}
