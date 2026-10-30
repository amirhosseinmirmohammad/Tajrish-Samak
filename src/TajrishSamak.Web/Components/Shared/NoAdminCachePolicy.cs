using Microsoft.AspNetCore.OutputCaching;

namespace TajrishSamak.Web.Components.Shared;

/// <summary>
/// Base output-cache policy: never cache /admin paths or any authenticated
/// user's responses (prevents stale admin data and cross-user leaks).
/// </summary>
public sealed class NoAdminCachePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext ctx, CancellationToken cancellationToken)
    {
        var path = ctx.HttpContext.Request.Path;
        var authed = ctx.HttpContext.User.Identity?.IsAuthenticated == true;

        if (path.StartsWithSegments("/admin") ||
            path.StartsWithSegments("/not-found") ||
            authed)
        {
            ctx.EnableOutputCaching = false;
            ctx.AllowCacheLookup = false;
            ctx.AllowCacheStorage = false;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext ctx, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext ctx, CancellationToken cancellationToken)
        => ValueTask.CompletedTask;
}
