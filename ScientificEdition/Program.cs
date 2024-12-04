using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Business;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Mvc.Helpers;
using ScientificEdition.Utilities.Files;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

RegisterRoutes(app);

app.MapRazorPages();

app.Run();

static void ConfigureServices(IServiceCollection services)
{
    services.AddLocalization(options => options.ResourcesPath = "Resources");

    services.AddSingleton<FileManager>();
    services.AddSingleton<TextHelper>();

    services.AddScoped<ArticleManager>();
    services.AddScoped<CategoryManager>();
    services.AddScoped<ReviewerManager>();
    services.AddScoped<JournalManager>();
}

static void RegisterRoutes(WebApplication app)
{
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    ).RequireAuthorization(new AuthorizeAttribute { Roles = UserRoles.Admin });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
}
