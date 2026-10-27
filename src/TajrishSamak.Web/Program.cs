using TajrishSamak.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------- database (existing production schema, no migrations) ----------------
builder.Services.AddDbContext<TajrishSamakDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        sql =>
        {
            sql.EnableRetryOnFailure(3);
            sql.CommandTimeout(60);
        })
    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
#if DEBUG
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
#endif
);

// ---------------- Blazor (interactive server) ----------------
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DisconnectedCircuitMaxRetained = 200;
        options.DetailedErrors = false;
    });

// ---------------- memory cache: the legacy layout ran 4 DB queries per request,
// these are now cached (categories/brands/service categories) ----------------
builder.Services.AddMemoryCache();

// ---------------- response output caching for public pages ----------------
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
});

// ---------------- response compression (brotli + gzip) ----------------
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes
        .Concat(new[] { "application/xml", "text/plain", "image/svg+xml", "image/webp" });
});

var app = builder.Build();

// ---------------- pipeline ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseResponseCompression();

// static assets: long-lived immutable caching for versioned files
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl = "public,max-age=604800";
    }
});

app.UseOutputCache();

app.UseAntiforgery();

app.MapRazorComponents<TajrishSamak.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
