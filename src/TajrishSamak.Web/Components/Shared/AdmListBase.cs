using Microsoft.AspNetCore.Components;

namespace TajrishSamak.Web.Components.Shared;

/// <summary>
/// Base for admin list pages: query-string driven search (q), pagination (page),
/// and column sorting (sort/dir) — pure GET, works in static SSR without circuits.
/// </summary>
public abstract class AdmListBase : ComponentBase
{
    [Inject] protected NavigationManager Nav { get; set; } = default!;

    // NOTE: [SupplyParameterFromQuery] must be declared on the page component itself,
    // not in a base class. Pages expose proxy properties that write to these fields.
    protected string? Qs;
    protected int? PageN;
    protected string? SortS;
    protected string? DirS;

    protected int PageSize { get; set; } = 20;

    protected string Q => Qs ?? "";
    protected int Page => Math.Max(1, PageN ?? 1);
    protected string Sort => SortS ?? "";
    protected bool Asc => DirS != "desc";
    protected string DirStr => Asc ? "asc" : "desc";

    /// <summary>Builds the current URL with modified/removed query params (preserves all others).</summary>
    protected string UrlWith(params (string Key, string? Val)[] changes)
    {
        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
        foreach (var (k, v) in changes)
        {
            if (string.IsNullOrEmpty(v)) qs.Remove(k);
            else qs[k] = v;
        }
        var pathAndQuery = uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var qIndex = pathAndQuery.IndexOf('?');
        var pathOnly = qIndex >= 0 ? pathAndQuery[..qIndex] : pathAndQuery;
        var s = qs.ToString();
        return pathOnly + (string.IsNullOrEmpty(s) ? "" : "?" + s);
    }

    /// <summary>URL that toggles sorting to the given key (asc first, then desc), resetting page.</summary>
    protected string SortUrl(string key) =>
        UrlWith(("sort", key), ("dir", Sort == key && Asc ? "desc" : "asc"), ("page", "1"));

    /// <summary>URL for a pager page, preserving search/sort.</summary>
    protected string PageUrl(int p) => UrlWith(("page", p.ToString()));

    protected string Arrow(string key) => Sort == key ? (Asc ? "▲" : "▼") : "";
}
