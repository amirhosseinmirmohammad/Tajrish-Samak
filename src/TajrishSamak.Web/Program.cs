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
builder.Services.AddHttpContextAccessor();

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

// ---------------- dynamic sitemap.xml (ported from legacy SitemapController) ----------------
app.MapGet("/sitemap.xml", async (TajrishSamakDbContext db) =>
{
    var lastMod = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    var sb = new System.Text.StringBuilder();
    sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

    void Add(string path, string freq, string priority)
    {
        sb.Append("<url><loc>https://tajrish-samak.ir").Append(path)
          .Append("</loc><lastmod>").Append(lastMod)
          .Append("</lastmod><changefreq>").Append(freq)
          .Append("</changefreq><priority>").Append(priority).Append("</priority></url>");
    }

    Add("/", "daily", "1.0");
    Add("/Product/All", "daily", "0.9");
    Add("/Service/All", "daily", "0.9");
    Add("/Blog/All", "daily", "0.9");
    Add("/hearing-aid-brands", "weekly", "0.8");

    foreach (var brand in TajrishSamak.Web.Controllers.LegacyBrands.All)
    {
        Add("/hearing-aid-brands/" + brand, "weekly", "0.8");
    }

    foreach (var area in TajrishSamak.Web.Services.GeoCatalog.LocationList)
    {
        foreach (var service in TajrishSamak.Web.Services.GeoCatalog.ServiceList)
        {
            Add("/" + area.Slug + "/" + service.Slug, "weekly", "0.64");
        }
    }

    var productIds = await db.Products
        .AsNoTracking()
        .Where(p => p.SiteFirstImage != null)
        .OrderByDescending(p => p.CreateDate)
        .Select(p => p.Id)
        .ToListAsync();
    foreach (var id in productIds)
    {
        Add("/Product/Details/" + id, "weekly", "0.8");
    }

    var blogs = await db.Blogs
        .AsNoTracking()
        .Where(b => b.IsVisible == true && b.Images.Any())
        .OrderByDescending(b => b.CreateDate)
        .Select(b => new { b.Id, b.SefUrl })
        .ToListAsync();
    foreach (var b in blogs)
    {
        Add("/Blog/" + b.Id + "/" + b.SefUrl, "weekly", "0.8");
    }

    var services = await db.Services
        .AsNoTracking()
        .Where(s => s.IsVisible == true && s.Images.Any())
        .OrderByDescending(s => s.Survey)
        .Select(s => new { s.Id, s.SefUrl })
        .ToListAsync();
    foreach (var s in services)
    {
        Add("/Service/" + s.Id + "/" + s.SefUrl, "weekly", "0.7");
    }

    sb.Append("</urlset>");
    return Results.Text(sb.ToString(), "application/xml; charset=utf-8");
}).CacheOutput(policy => policy.Expire(TimeSpan.FromHours(1)));

app.MapRazorComponents<TajrishSamak.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
