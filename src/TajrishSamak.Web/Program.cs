using TajrishSamak.Web.Data;
using TajrishSamak.Web.Data.Entities;
using TajrishSamak.Web.Controllers;
using TajrishSamak.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ---------------- database (existing production schema, no migrations) ----------------
// IDbContextFactory is the recommended pattern for Blazor: layout and page
// components each create their own short-lived context, avoiding the
// "second operation started on this context instance" concurrency error.
builder.Services.AddDbContextFactory<TajrishSamakDbContext>(options =>
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

// ---------------- admin cookie authentication ----------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login?denied=1";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "TajrishAdmin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddAuthorization();

// ---------------- rate limiting (brute-force + abuse protection) ----------------
// route constraint: geo landing only for real catalog slugs
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["geo"] = typeof(TajrishSamak.Web.Services.GeoLandingConstraint);
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.AddPolicy("login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));

    options.AddPolicy("write-api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("suggest", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 40,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    // public form submissions: 5/min per IP (anti-spam / anti-bot)
    options.AddPolicy("comment", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

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
    // never cache /admin or authenticated responses (stale data / privacy leak):
    options.AddBasePolicy(new TajrishSamak.Web.Components.Shared.NoAdminCachePolicy());
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

// ---------- security headers (XSS/clickjacking/MIME sniffing) ----------
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(self), payment=()";
    // css/js must always revalidate (fingerprint-less urls) -> fixes stale styles on client devices
    var path = context.Request.Path;
    if (path.StartsWithSegments("/css") || path.StartsWithSegments("/js"))
    {
        headers["Cache-Control"] = "no-cache";
    }
    headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "img-src 'self' data: https:; " +
        "style-src 'self' 'unsafe-inline'; " +
        "script-src 'self' 'unsafe-inline'; " +
        "connect-src 'self' wss: https://tile.openstreetmap.org; " +
        "font-src 'self' data:; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'";
    await next();
});

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRateLimiter();

// SSR pages must always be revalidated by browsers; heuristic caching of HTML
// served stale fingerprinted asset URLs after every deployment.
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Response.ContentType != null &&
            context.Response.ContentType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.CacheControl = "no-cache";
        }
        return Task.CompletedTask;
    });
    await next();
});

// static assets: disk-first serving (UseStaticFiles) so any file on the server
// is served live; MapStaticAssets afterwards adds fingerprinted/perf behavior
app.UseStaticFiles();
app.MapStaticAssets();

// kill-switch for the haunted legacy /service-worker.js path: deterministic 404
// (browsers auto-UNREGISTER a service worker when its script 404s on update)
app.Use(async (ctx, next) =>
{
    var swPath = ctx.Request.Path;
    if (swPath.Equals("/sw.js", StringComparison.OrdinalIgnoreCase) ||
        swPath.Equals("/service-worker-v20.js", StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.Headers["Cache-Control"] = "no-cache";
        ctx.Response.Headers["Service-Worker-Allowed"] = "/";
        await next();
        return;
    }
    if (swPath.Equals("/service-worker.js", StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.StatusCode = StatusCodes.Status404NotFound;
        ctx.Response.ContentType = "text/javascript";
        ctx.Response.Headers["Cache-Control"] = "no-cache";
        return;
    }
    await next();
});
// fallback for files that appear AFTER publish (runtime uploads, server-side
// merged images) - MapStaticAssets only serves files known at publish time
app.UseStaticFiles();

app.UseAuthentication();

// AdminGuard: every /admin path (pages AND minimal-API endpoints) requires an
// authenticated Administrator. Login endpoints are the only exception.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/admin"))
    {
        var path = ctx.Request.Path;
        var isLoginPath = path.StartsWithSegments("/admin/login") || path.StartsWithSegments("/admin/do-login");
        if (!isLoginPath &&
            !(ctx.User.Identity?.IsAuthenticated == true && ctx.User.IsInRole("Administrator")))
        {
            if (ctx.Request.Method != "GET")
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            ctx.Response.Redirect("/admin/login?ReturnUrl=" + Uri.EscapeDataString(path + ctx.Request.QueryString));
            return;
        }
    }
    await next();
});

app.UseAuthorization();

// Friendly 404: re-execute unmatched requests at /not-found (keeps 404 status)
app.UseStatusCodePagesWithReExecute("/not-found");

app.UseOutputCache();

app.UseAntiforgery();

// ---------------- newsletter signup (ported 1:1 from legacy HomeController.NewsLetter) ----------------
// NoCache: this GET writes to the database - the base output-cache policy
// must never replay a previous answer for the same query string.
app.MapGet("/Home/NewsLetter", async (HttpContext ctx, string? text, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    // this GET writes to the database; the browser must never replay a cached answer
    ctx.Response.Headers.CacheControl = "no-store";

    if (string.IsNullOrWhiteSpace(text))
    {
        return Results.Json(new { text = "لطفا ایمیل خود را وارد نمایید .", status = 0 });
    }

    text = text.Trim();

    if (text.Length > 254 || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(text))
    {
        return Results.Json(new { text = "لطفا ایمیل معتبری را وارد نمایید .", status = 0 });
    }

    await using var db = dbFactory.CreateDbContext();
    var emailExists = await db.NewsLetters
        .AsNoTracking()
        .AnyAsync(n => n.Email == text);

    if (emailExists)
    {
        return Results.Json(new { text = "ایمیل شما قبلا در سیستم ثبت شده است .", status = 0 });
    }

    try
    {
        db.NewsLetters.Add(new NewsLetter { Email = text });
        await db.SaveChangesAsync();
        return Results.Json(new { text = "ایمیل شما با موفقیت در سیستم ثبت شد .", status = 1 });
    }
    catch
    {
        return Results.Json(new { text = "خطایی رخ داده است لطفا مجدد تلاش فرمایید .", status = 0 });
    }
}).CacheOutput(policy => policy.NoCache()).RequireRateLimiting("write-api");

// ---------------- dynamic sitemap.xml (ported from legacy SitemapController) ----------------
app.MapGet("/sitemap.xml", async (IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var lastMod = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    await using var db = dbFactory.CreateDbContext();
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

    Add("/aboutus", "monthly", "0.7");
    Add("/faq", "monthly", "0.6");
    Add("/contactus", "monthly", "0.7");
    Add("/terms", "monthly", "0.4");

    foreach (var svcCat in db.ServiceCategories.AsNoTracking().OrderBy(sc => sc.Id).ToList())
    {
        Add("/Home/Services/" + svcCat.Id, "weekly", "0.7");
    }
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

    var products = await db.Products
        .AsNoTracking()
        .Where(p => p.SiteFirstImage != null)
        .OrderByDescending(p => p.CreateDate)
        .Select(p => new { p.Id, p.PersianName })
        .ToListAsync();

    static string Slug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "product";
        var sb = new System.Text.StringBuilder();
        foreach (var ch in text.Trim())
        {
            if (char.IsLetterOrDigit(ch)) { sb.Append(ch); }
            else if (ch == ' ' || ch == '-' || ch == '_') { sb.Append('-'); }
        }
        var result = System.Text.RegularExpressions.Regex.Replace(sb.ToString().Trim('-'), "-{2,}", "-");
        return string.IsNullOrWhiteSpace(result) ? "product" : result;
    }

    foreach (var p in products)
    {
        Add("/Product/Details/" + p.Id + "/" + Slug(p.PersianName), "weekly", "0.8");
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

// ---------------- search autocomplete (header quick search) ----------------
app.MapGet("/search/suggest", async (string? q, IDbContextFactory<TajrishSamakDbContext> dbFactory, IMemoryCache cache) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.Json(Array.Empty<SuggestItem>());
    }

    var term = q.Trim();
    if (term.Length < 2 || term.Length > 60)
    {
        return Results.Json(Array.Empty<SuggestItem>());
    }

    // strip LIKE wildcards so user input cannot alter the pattern
    var safe = term.Replace("%", "").Replace("_", "").Replace("[", "").Trim();
    if (safe.Length < 2)
    {
        return Results.Json(Array.Empty<SuggestItem>());
    }

    var items = await cache.GetOrCreateAsync("suggest:" + safe.ToLowerInvariant(), async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        await using var db = dbFactory.CreateDbContext();

        var products = await db.Products
            .AsNoTracking()
            .Where(p => p.SiteFirstImage != null &&
                        (p.PersianName.Contains(safe) || p.EnglishName.Contains(safe)))
            .OrderByDescending(p => p.CreateDate)
            .Take(5)
            .Select(p => new { p.PersianName, p.Id, p.SiteFirstImage })
            .ToListAsync();

        var services = await db.Services
            .AsNoTracking()
            .Where(s => s.IsVisible && s.Title.Contains(safe))
            .Take(3)
            .Select(s => new { s.Title, s.Id, s.SefUrl, Img = s.Images.Select(i => i.FullPath).FirstOrDefault() })
            .ToListAsync();

        var blogs = await db.Blogs
            .AsNoTracking()
            .Where(b => b.IsVisible == true && b.Title.Contains(safe))
            .OrderByDescending(b => b.CreateDate)
            .Take(3)
            .Select(b => new { b.Title, b.Id, b.SefUrl, Img = b.Images.Select(i => i.FullPath).FirstOrDefault() })
            .ToListAsync();

        var list = new List<SuggestItem>();
        list.AddRange(products.Select(p => new SuggestItem("product", "محصول", p.PersianName, "/Product/Details/" + p.Id, p.SiteFirstImage)));
        list.AddRange(services.Select(s => new SuggestItem("service", "خدمت", s.Title, "/Service/" + s.Id + "/" + s.SefUrl, s.Img)));
        list.AddRange(blogs.Select(b => new SuggestItem("blog", "مقاله", b.Title, "/blog/" + b.Id + "/" + b.SefUrl, b.Img)));

        // brand landings come from the static catalog (no DB round-trip);
        // brand logo tiles are looked up from the Brands table when available
        var brandLogos = await db.Brands
            .AsNoTracking()
            .Select(b => new { b.PersianName, b.EnglishName, b.SmallImage })
            .Take(30)
            .ToListAsync();

        var brands = LegacyBrands.Brands
            .Where(b => b.PersianName.Contains(safe, StringComparison.OrdinalIgnoreCase) ||
                        b.EnglishName.Contains(safe, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .Select(b =>
            {
                var logo = brandLogos.FirstOrDefault(l =>
                    l.PersianName == b.PersianName ||
                    l.EnglishName != null && l.EnglishName.IndexOf(b.EnglishName, StringComparison.OrdinalIgnoreCase) >= 0);

                return new SuggestItem("brand", "برند", "سمعک " + b.PersianName, "/hearing-aid-brands/" + b.Slug, logo?.SmallImage);
            });

        var result = list.Concat(brands).Take(9).ToList();
        return result;
    });

    return Results.Json(items ?? new List<SuggestItem>());
}).RequireRateLimiting("suggest");

// ---------------- blog article like/dislike voting ----------------
app.MapPost("/blog/{id:long}/vote", async (long id, BlogVoteRequest req, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var isLike = string.Equals(req.Value, "like", StringComparison.OrdinalIgnoreCase);
    var isDislike = string.Equals(req.Value, "dislike", StringComparison.OrdinalIgnoreCase);

    if (!isLike && !isDislike)
    {
        return Results.Json(new { ok = false });
    }

    await using var db = dbFactory.CreateDbContext();
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Id == id && b.IsVisible);

    if (blog == null)
    {
        return Results.Json(new { ok = false });
    }

    if (isLike) { blog.Like = (blog.Like ?? 0) + 1; }
    else { blog.DissLike = (blog.DissLike ?? 0) + 1; }

    try
    {
        await db.SaveChangesAsync();
        return Results.Json(new { ok = true, likes = blog.Like ?? 0, dislikes = blog.DissLike ?? 0 });
    }
    catch
    {
        return Results.Json(new { ok = false });
    }
}).RequireRateLimiting("write-api");

// ---------------- admin upload (drag & drop) ----------------
app.MapPost("/admin/upload", async (HttpContext ctx, IFormFile file, string? folder) =>
{
    if (ctx.User?.Identity?.IsAuthenticated != true)
    {
        return Results.Json(new { ok = false, error = "unauthorized" }, statusCode: 401);
    }

    if (file == null || file.Length == 0)
    {
        return Results.Json(new { ok = false, error = "empty" });
    }

    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };
    if (!allowed.Contains(ext))
    {
        return Results.Json(new { ok = false, error = "فرمت فایل مجاز نیست (jpg/png/webp/gif/svg)" });
    }
    if (file.Length > 5 * 1024 * 1024)
    {
        return Results.Json(new { ok = false, error = "حجم فایل بیش از ۵ مگابایت است" });
    }

    var safeFolder = System.Text.RegularExpressions.Regex.Replace(folder ?? "uploads", "[^a-zA-Z0-9-]", "");
    var webRoot = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>().WebRootPath
        ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    var dir = Path.Combine(webRoot, "uploads", safeFolder);
    Directory.CreateDirectory(dir);

    var name = Guid.NewGuid().ToString("N") + ext;
    await using var stream = System.IO.File.Create(Path.Combine(dir, name));
    await file.CopyToAsync(stream);

    return Results.Json(new { ok = true, url = "/uploads/" + safeFolder + "/" + name });
}).RequireAuthorization().DisableAntiforgery();

// ---------------- admin upload (drag & drop) ----------------
app.MapPost("/admin/delete", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    var entity = form["entity"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/admin")) { returnUrl = "/admin"; }
    if (!int.TryParse(form["id"], out var id)) { return Results.Redirect(returnUrl + "?err=1"); }

    await using var db = dbFactory.CreateDbContext();
    try
    {
        switch (entity)
        {
            case "blog": { var item = await db.Blogs.FindAsync((long)id); if (item != null) db.Blogs.Remove(item); break; }
            case "product": { var item = await db.Products.FindAsync(id); if (item != null) db.Products.Remove(item); break; }
            case "service": { var item = await db.Services.FindAsync(id); if (item != null) db.Services.Remove(item); break; }
            case "brand": { var item = await db.Brands.FindAsync(id); if (item != null) db.Brands.Remove(item); break; }
            case "category": { var item = await db.Categories.FindAsync(id); if (item != null) db.Categories.Remove(item); break; }
            case "slider": { var item = await db.Sliders.FindAsync(id); if (item != null) db.Sliders.Remove(item); break; }
            case "discount": { var item = await db.Discounts.FindAsync(id); if (item != null) db.Discounts.Remove(item); break; }
            case "gallery": { var item = await db.Galleries.FindAsync(id); if (item != null) db.Galleries.Remove(item); break; }
            case "sentemail": { var item = await db.SentEmails.FindAsync(id); if (item != null) db.SentEmails.Remove(item); break; }
            case "notification": { var item = await db.Notifications.FindAsync(id); if (item != null) db.Notifications.Remove(item); break; }
            case "city": { var item = await db.Cities.FindAsync(id); if (item != null) db.Cities.Remove(item); break; }
            case "state": { var item = await db.States.FindAsync(id); if (item != null) db.States.Remove(item); break; }
            case "newsletter": { var item = await db.NewsLetters.FindAsync(id); if (item != null) db.NewsLetters.Remove(item); break; }
            case "message": { var item = await db.SiteMessages.FindAsync(id); if (item != null) db.SiteMessages.Remove(item); break; }
            case "comment": { var item = await db.BlogComments.FindAsync(id); if (item != null) db.BlogComments.Remove(item); break; }
            default: return Results.Redirect(returnUrl + "?err=1");
        }
        await db.SaveChangesAsync();
    }
    catch
    {
        // FK constraints etc.
        return Results.Redirect(returnUrl + "?err=2");
    }
    return Results.Redirect(returnUrl);
});

// ---------------- generic entity save (whitelisted in AdmEntityRegistry) ----------------
app.MapPost("/admin/save/entity", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    var def = TajrishSamak.Web.Components.Shared.AdmEntityRegistry.Get(form["entity"].ToString());
    if (def == null) return Results.Redirect("/admin?err=1");

    var id = long.TryParse(form["id"], out var eid) && eid > 0 ? eid : 0;
    var ok = await def.Save(dbFactory, id, form);
    return Results.Redirect(ok ? def.ListUrl + "?saved=1" : def.ListUrl + "?err=1");
});

// ---------------- public: blog comment submission (static SSR, honeypot + rate limit) ----------------
app.MapPost("/blog/comment", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    long.TryParse(form["blogId"], out var blogId);
    var name = form["fullName"].ToString().Trim();
    var text = form["text"].ToString().Trim();
    var honeypot = form["website"].ToString();
    var back = form["returnUrl"].ToString();
    if (string.IsNullOrEmpty(back) || !back.StartsWith("/blog/")) back = "/Blog/All";

    // bot filled the hidden field -> silently accept and drop
    if (!string.IsNullOrEmpty(honeypot)) return Results.Redirect(back + "?cmt=ok");

    if (blogId <= 0 || name.Length < 2 || text.Length < 5 || name.Length > 100 || text.Length > 2000)
        return Results.Redirect(back + "?cmt=err");

    await using var db = dbFactory.CreateDbContext();
    var exists = await db.Blogs.AnyAsync(b => b.Id == blogId);
    if (!exists) return Results.Redirect(back + "?cmt=err");

    db.BlogComments.Add(new BlogComment
    {
        FullName = name,
        Text = text,
        DateTime = DateTime.Now,
        IsApprove = false,
        BlogId = (int)blogId
    });
    await db.SaveChangesAsync();
    return Results.Redirect(back + "?cmt=ok");
}).RequireRateLimiting("comment");

// ---------------- public: contact message (static SSR, honeypot + rate limit) ----------------
app.MapPost("/contact/message", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    var name = form["fullName"].ToString().Trim();
    var email = form["email"].ToString().Trim();
    var body = form["body"].ToString().Trim();
    var honeypot = form["website"].ToString();
    var back = form["returnUrl"].ToString();
    if (string.IsNullOrEmpty(back) || back.StartsWith("/admin")) back = "/contactus";

    if (!string.IsNullOrEmpty(honeypot)) return Results.Redirect(back + "?contact=ok");

    if (name.Length < 2 || body.Length < 5 || name.Length > 100 || body.Length > 3000)
        return Results.Redirect(back + "?contact=err");

    await using var db = dbFactory.CreateDbContext();
    db.SiteMessages.Add(new SiteMessage
    {
        FullName = name,
        Body = body + (!string.IsNullOrWhiteSpace(email) ? " | ایمیل: " + email : ""),
        RegisterDate = DateTime.Now
    });
    await db.SaveChangesAsync();
    return Results.Redirect(back + "?contact=ok");
}).RequireRateLimiting("comment");

// ---------------- order status (U of Orders) ----------------
app.MapPost("/admin/order/status", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    if (!int.TryParse(form["id"], out var id)) return Results.Redirect("/admin/orders?err=1");
    var status = form["status"].ToString();

    await using var db = dbFactory.CreateDbContext();
    var order = await db.Orders.AsTracking().FirstOrDefaultAsync(o => o.Id == id);
    if (order == null) return Results.Redirect("/admin/orders?err=1");

    switch (status)
    {
        case "done": order.Done = true; order.IsCanceled = false; break;
        case "cancel": order.Done = false; order.IsCanceled = true; break;
        case "reopen": order.Done = false; order.IsCanceled = false; break;
        default: return Results.Redirect("/admin/orders?err=1");
    }
    await db.SaveChangesAsync();
    return Results.Redirect("/admin/orders");
});

// ---------------- user activate/deactivate (U of Users) ----------------
app.MapPost("/admin/user/toggle", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    var id = form["id"].ToString();

    await using var db = dbFactory.CreateDbContext();
    var user = await db.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == id);
    if (user == null) return Results.Redirect("/admin/users?err=1");

    // safety: never deactivate yourself
    var me = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (me == id) return Results.Redirect("/admin/users?err=self");

    user.IsActive = !user.IsActive;
    await db.SaveChangesAsync();
    return Results.Redirect("/admin/users");
});

// ---------------- payment block/unblock (U of Payments) ----------------
app.MapPost("/admin/payment/block", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    if (!int.TryParse(form["id"], out var id)) return Results.Redirect("/admin/payments?err=1");

    await using var db = dbFactory.CreateDbContext();
    var pay = await db.Payments.AsTracking().FirstOrDefaultAsync(p => p.Id == id);
    if (pay == null) return Results.Redirect("/admin/payments?err=1");

    pay.IsBlocked = !pay.IsBlocked;
    await db.SaveChangesAsync();
    return Results.Redirect("/admin/payments");
});

// ---------------- admin profile save (info + password change) ----------------
app.MapPost("/admin/save/profile", async (
    HttpContext ctx,
    [FromForm] string? firstName,
    [FromForm] string? lastName,
    [FromForm] string? email,
    [FromForm] string? mobile,
    [FromForm] string? currentPassword,
    [FromForm] string? newPassword,
    IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Redirect("/admin/login");

    await using var db = dbFactory.CreateDbContext();
    var user = await db.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null) return Results.Redirect("/admin/login");

    if (!string.IsNullOrWhiteSpace(firstName)) user.FirstName = firstName.Trim();
    if (!string.IsNullOrWhiteSpace(lastName)) user.LastName = lastName.Trim();
    if (!string.IsNullOrWhiteSpace(email)) user.Email = email.Trim();
    if (!string.IsNullOrWhiteSpace(mobile)) user.Mobile = mobile.Trim();

    if (!string.IsNullOrWhiteSpace(newPassword))
    {
        if (newPassword.Length < 6)
            return Results.Redirect("/admin/profile?pw=short");
        if (string.IsNullOrWhiteSpace(currentPassword) ||
            !LegacyPasswordHasher.Verify(user.PasswordHash ?? "", currentPassword))
            return Results.Redirect("/admin/profile?pw=wrong");

        user.PasswordHash = LegacyPasswordHasher.Hash(newPassword);
    }

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect("/admin/profile?err=1"); }
    return Results.Redirect("/admin/profile?saved=1");
});

app.MapPost("/admin/toggle", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var form = await ctx.Request.ReadFormAsync();
    var entity = form["entity"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    if (string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith("/admin")) { returnUrl = "/admin"; }
    if (!int.TryParse(form["id"], out var id)) { return Results.Redirect(returnUrl + "?err=1"); }

    await using var db = dbFactory.CreateDbContext();

    switch (entity)
    {
        case "blog":
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE Blogs SET IsVisible = CASE WHEN IsVisible = 1 THEN 0 ELSE 1 END WHERE Id = {id}");
            break;
        case "service":
            await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE Services SET IsVisible = CASE WHEN IsVisible = 1 THEN 0 ELSE 1 END WHERE Id = {id}");
            break;
    }
    return Results.Redirect(returnUrl);
});

// ---------------- admin save endpoints (create/update) ----------------
app.MapPost("/admin/save/blog", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/blogs";

    await using var db = dbFactory.CreateDbContext();
    var id = long.TryParse(form["id"], out var bid) ? bid : 0;
    var blog = id > 0 ? await db.Blogs.AsTracking().FirstOrDefaultAsync(b => b.Id == id) : new Blog { CreateDate = DateTime.Now, Survey = 0, UserId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "" };

    if (blog == null) return Results.Redirect(returnUrl + "?err=1");

    blog.Title = form["title"].ToString().Trim();
    blog.ShortDesc = form["shortDesc"].ToString();
    blog.Body = form["body"].ToString();
    blog.MetaKey = form["metaKey"].ToString();
    blog.MetaDesc = form["metaDesc"].ToString();
    blog.IsVisible = form["isVisible"] == "true";
    if (int.TryParse(form["categoryId"], out var catId)) blog.CategoryId = catId; else blog.CategoryId = null;
    blog.SefUrl = form["sefUrl"].ToString().Trim();
    if (string.IsNullOrEmpty(blog.SefUrl)) blog.SefUrl = "blog-" + DateTime.Now.Ticks;
    if (id == 0) db.Blogs.Add(blog);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

app.MapPost("/admin/save/product", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/products";

    await using var db = dbFactory.CreateDbContext();
    var id = int.TryParse(form["id"], out var pid) ? pid : 0;
    var product = id > 0 ? await db.Products.AsTracking().FirstOrDefaultAsync(p => p.Id == pid) : new Product { CreateDate = DateTime.Now, SiteFirstImage = "/images/product-1.jpg" };

    if (product == null) return Results.Redirect(returnUrl + "?err=1");

    product.PersianName = form["name"].ToString().Trim();
    product.EnglishName = form["enName"].ToString().Trim();
    int.TryParse(form["unitPrice"], out var prodPrice);
    int.TryParse(form["stock"], out var prodStock);
    int.TryParse(form["discount"], out var prodDiscount);
    int.TryParse(form["categoryId"], out var prodCat);
    product.UnitPrice = prodPrice;
    product.Stock = prodStock;
    product.DiscountPercent = (byte)Math.Clamp(prodDiscount, 0, 100);
    if (prodCat > 0) product.CategoryId = prodCat;
    if (int.TryParse(form["brandId"], out var brId) && brId > 0) product.BrandId = brId;
    product.SiteFirstImage = form["imageUrl"].ToString().Trim();
    product.Description = form["description"].ToString();
    product.IsSpecial = form["isSpecial"] == "true";

    if (id == 0) db.Products.Add(product);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

app.MapPost("/admin/save/service", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/services";

    await using var db = dbFactory.CreateDbContext();
    var id = int.TryParse(form["id"], out var sid) ? sid : 0;
    var service = id > 0 ? await db.Services.AsTracking().FirstOrDefaultAsync(s => s.Id == sid) : new Service { CreateDate = DateTime.Now };

    if (service == null) return Results.Redirect(returnUrl + "?err=1");

    service.Title = form["title"].ToString().Trim();
    service.ShortDesc = form["shortDesc"].ToString();
    service.Body = form["body"].ToString();
    service.MetaDesc = form["metaDesc"].ToString();
    service.MetaKey = form["metaKey"].ToString();
    service.IsVisible = form["isVisible"] == "true";
    if (int.TryParse(form["categoryId"], out var svcCatId)) service.CategoryId = svcCatId;
    service.SefUrl = form["sefUrl"].ToString().Trim();
    if (string.IsNullOrEmpty(service.SefUrl)) service.SefUrl = "service-" + DateTime.Now.Ticks;
    if (id == 0) db.Services.Add(service);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

app.MapPost("/admin/save/category", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/categories";

    await using var db = dbFactory.CreateDbContext();
    var id = int.TryParse(form["id"], out var cid) ? cid : 0;
    var category = id > 0 ? await db.Categories.AsTracking().FirstOrDefaultAsync(c => c.Id == cid) : new Category();

    if (category == null) return Results.Redirect(returnUrl + "?err=1");

    category.PersianName = form["name"].ToString().Trim();
    category.EnglishName = form["enName"].ToString().Trim();
    category.SmallImage = form["smallImage"].ToString().Trim();
    category.LargeImage = form["largeImage"].ToString().Trim();
    category.IsBlog = form["isBlog"] == "true";
    if (int.TryParse(form["parentId"], out var parentCat) && parentCat > 0) category.ParentId = parentCat; else category.ParentId = null;
    if (id == 0) db.Categories.Add(category);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

app.MapPost("/admin/save/brand", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/brands";

    await using var db = dbFactory.CreateDbContext();
    var id = int.TryParse(form["id"], out var bid) ? bid : 0;
    var brand = id > 0 ? await db.Brands.AsTracking().FirstOrDefaultAsync(b => b.Id == bid) : new Brand();

    if (brand == null) return Results.Redirect(returnUrl + "?err=1");

    brand.PersianName = form["name"].ToString().Trim();
    brand.EnglishName = form["enName"].ToString().Trim();
    brand.SmallImage = form["smallImage"].ToString().Trim();
    brand.LargeImage = form["largeImage"].ToString().Trim();
    if (id == 0) db.Brands.Add(brand);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

app.MapPost("/admin/save/slider", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    var returnUrl = "/admin/sliders";

    await using var db = dbFactory.CreateDbContext();
    var id = int.TryParse(form["id"], out var slid) ? slid : 0;
    var slider = id > 0 ? await db.Sliders.AsTracking().FirstOrDefaultAsync(s => s.Id == slid) : new Slider { CreateDate = DateTime.Now, LastDate = DateTime.Now };

    if (slider == null) return Results.Redirect(returnUrl + "?err=1");

    slider.Image = form["image"].ToString().Trim();
    slider.MobImage = form["mobImage"].ToString().Trim();
    slider.Link = form["link"].ToString().Trim();
    slider.LastDate = DateTime.Now;
    if (id == 0) db.Sliders.Add(slider);

    try { await db.SaveChangesAsync(); } catch { return Results.Redirect(returnUrl + "?err=2"); }
    return Results.Redirect(returnUrl + "?saved=1");
});

// ---------------- admin comments approve ----------------
app.MapPost("/admin/comments/approve", async (HttpContext ctx, IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);
    var form = await ctx.Request.ReadFormAsync();
    if (int.TryParse(form["id"], out var id))
    {
        await using var db = dbFactory.CreateDbContext();
        var comment = await db.BlogComments.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (comment != null) { comment.IsApprove = true; await db.SaveChangesAsync(); }
    }
    return Results.Redirect("/admin/comments");
});

// ---------------- admin auth: login submit (3 segments: no geo-route conflict) ----------------
app.MapPost("/admin/do-login", async (
    HttpContext ctx,
    [FromForm] string? email,
    [FromForm] string? password,
    [FromForm] string? remember,
    IDbContextFactory<TajrishSamakDbContext> dbFactory) =>
{
    // antiforgery validation
    var antiforgery = ctx.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
    await antiforgery.ValidateRequestAsync(ctx);

    var fail = (string code) => Results.Redirect("/admin/login?e=" + code);

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return fail("1");
    }

    await using var db = dbFactory.CreateDbContext();

    var identityUser = await db.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.UserName == email || u.Email == email);

    if (identityUser == null || !LegacyPasswordHasher.Verify(identityUser.PasswordHash, password))
    {
        return fail("1");
    }

    // legacy lockout support
    if (identityUser.LockoutEnabled &&
        identityUser.LockoutEndDateUtc.HasValue &&
        identityUser.LockoutEndDateUtc.Value > DateTime.UtcNow)
    {
        return Results.Redirect("/admin/login?e=2");
    }

    if (!identityUser.IsActive)
    {
        return Results.Redirect("/admin/login?e=3");
    }

    var roles = await db.IdentityUserRoles
        .Where(ur => ur.UserId == identityUser.Id)
        .Join(db.IdentityRoles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
        .ToListAsync();

    if (!roles.Contains("Administrator"))
    {
        return Results.Redirect("/admin/login?e=4");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, identityUser.Id),
        new(ClaimTypes.Name, identityUser.UserName ?? identityUser.Email ?? ""),
        new(ClaimTypes.Email, identityUser.Email ?? "")
    };
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = remember == "true",
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

    return Results.Redirect("/admin");
})
.RequireRateLimiting("login");

// ---------------- admin logout ----------------
app.MapPost("/admin/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/admin/login");
});

app.MapGet("/admin/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/admin/login");
});

// ---------------- legacy admin route redirects ----------------
app.MapGet("/Account/Login", (HttpContext ctx) => { ctx.Response.Redirect("/admin/login"); return Task.CompletedTask; });
app.MapGet("/Account/AdminPanel", (HttpContext ctx) => { ctx.Response.Redirect("/admin"); return Task.CompletedTask; });
app.MapGet("/Account/LogOutUser/{userId?}", (HttpContext ctx) => { ctx.Response.Redirect("/admin/login"); return Task.CompletedTask; });

app.MapRazorComponents<TajrishSamak.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

internal record SuggestItem(string Kind, string Type, string Title, string Url, string? Img = null);

internal record BlogVoteRequest(string Value);
