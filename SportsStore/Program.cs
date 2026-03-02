using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SportsStore.Infrastructure;
using SportsStore.Models;
using SportsStore.Services;

// ── Bootstrap logger (captures failures during host startup) ──────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SportsStore application");

    var builder = WebApplication.CreateBuilder(args);

    // ── Replace default logging with Serilog, reading from appsettings ─────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    // ── Services ───────────────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<StoreDbContext>(opts =>
        opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]));

    builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
    builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
    builder.Services.AddScoped<IPaymentService, StripePaymentService>();

    builder.Services.AddRazorPages();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession();
    builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddDbContext<AppIdentityDbContext>(options =>
        options.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]));

    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppIdentityDbContext>();

    // ── Pipeline ───────────────────────────────────────────────────────────────
    var app = builder.Build();

    if (app.Environment.IsProduction())
    {
        app.UseExceptionHandler("/error");
    }

    app.UseRequestLocalization(opts =>
        opts.AddSupportedCultures("en-US")
            .AddSupportedUICultures("en-US")
            .SetDefaultCulture("en-US"));

    // Enriches every request log with HTTP details (method, path, status, elapsed)
    app.UseSerilogRequestLogging(opts =>
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");

    // Adds CorrelationId to every log within the request scope
    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseStaticFiles();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute("catpage",
        "{category}/Page{productPage:int}",
        new { Controller = "Home", action = "Index" });

    app.MapControllerRoute("page", "Page{productPage:int}",
        new { Controller = "Home", action = "Index", productPage = 1 });

    app.MapControllerRoute("category", "{category}",
        new { Controller = "Home", action = "Index", productPage = 1 });

    app.MapControllerRoute("pagination", "Products/Page{productPage}",
        new { Controller = "Home", action = "Index", productPage = 1 });

    app.MapDefaultControllerRoute();
    app.MapRazorPages();
    app.MapBlazorHub();
    app.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

    SeedData.EnsurePopulated(app);
    IdentitySeedData.EnsurePopulated(app);

    Log.Information("SportsStore started successfully on {Environment}",
        app.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SportsStore terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
