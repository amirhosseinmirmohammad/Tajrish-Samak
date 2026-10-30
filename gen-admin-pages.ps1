# Generates simple admin list+delete pages (Messages.razor pattern)
# Run: pwsh gen-admin-pages.ps1  (from src/TajrishSamak.Web)
$ErrorActionPreference = "Stop"
$dir = "D:\opencode-work\tajrish-samak-v2\src\TajrishSamak.Web\Components\Pages\Admin"

function Make-Page {
    param([string]$Route, [string]$Title, [string]$Entity, [string]$Query, [string[]]$Heads, [string]$RowCells)

    $headsHtml = ""
    foreach ($h in $Heads) { $headsHtml += "                <th>$h</th>`r`n" }
    $headsHtml = $headsHtml.TrimEnd("`r`n")

    $razor = "@page `"/admin/$Route`"`r`n"
    $razor += "@attribute [Microsoft.AspNetCore.Authorization.Authorize(Roles = `"Administrator`")]`r`n"
    $razor += "@layout TajrishSamak.Web.Components.Layout.AdminLayout`r`n"
    $razor += "@using TajrishSamak.Web.Data.Entities`r`n"
    $razor += "@using Microsoft.EntityFrameworkCore`r`n"
    $razor += "@inject IDbContextFactory<TajrishSamak.Web.Data.TajrishSamakDbContext> DbFactory`r`n"
    $razor += "@inject Microsoft.AspNetCore.Http.IHttpContextAccessor Http`r`n"
    $razor += "@inject Microsoft.AspNetCore.Antiforgery.IAntiforgery Antiforgery`r`n`r`n"
    $razor += "<PageTitle>$Title | پنل مدیریت</PageTitle>`r`n"
    $razor += "<link rel=`"stylesheet`" href=`"@Assets[`"css/gp-admin.css`"]`" />`r`n`r`n"
    $razor += "<div class=`"adm-card adm-table-wrap`">`r`n"
    $razor += "    <table class=`"adm-table`">`r`n"
    $razor += "        <thead>`r`n            <tr>`r`n$headsHtml`r`n                <th></th>`r`n            </tr>`r`n        </thead>`r`n"
    $razor += "        <tbody>`r`n"
    $razor += "            @foreach (var item in items)`r`n"
    $razor += "            {`r`n"
    $razor += $RowCells
    $razor += "                <td>`r`n"
    $razor += "                    <form method=`"post`" action=`"/admin/delete`">`r`n"
    $razor += "                        <input type=`"hidden`" name=`"entity`" value=`"$Entity`" />`r`n"
    $razor += "                        <input type=`"hidden`" name=`"id`" value=`"@item.Id`" />`r`n"
    $razor += "                        <input type=`"hidden`" name=`"returnUrl`" value=`"/admin/$Route`" />`r`n"
    $razor += "                        <input type=`"hidden`" name=`"__RequestVerificationToken`" value=`"@token`" />`r`n"
    $razor += "                        <button type=`"submit`" class=`"adm-btn adm-btn-danger`">حذف</button>`r`n"
    $razor += "                    </form>`r`n"
    $razor += "                </td>`r`n"
    $razor += "            }`r`n"
    $razor += "            @if (items.Count == 0)`r`n"
    $razor += "            {`r`n"
    $razor += "                <tr><td colspan=`"99`">موردی ثبت نشده است.</td></tr>`r`n"
    $razor += "            }`r`n"
    $razor += "        </tbody>`r`n"
    $razor += "    </table>`r`n"
    $razor += "</div>`r`n`r`n"
    $razor += "@code {`r`n"
    $razor += "    private List<$Entity> items = new();`r`n"
    $razor += "    private string token = `"`";`r`n`r`n"
    $razor += "    protected override void OnInitialized()`r`n    {`r`n"
    $razor += "        if (Http.HttpContext != null)`r`n        {`r`n"
    $razor += "            token = Antiforgery.GetAndStoreTokens(Http.HttpContext).RequestToken;`r`n"
    $razor += "        }`r`n    }`r`n`r`n"
    $razor += "    protected override async Task OnParametersSetAsync()`r`n    {`r`n"
    $razor += "        await using var db = DbFactory.CreateDbContext();`r`n"
    $razor += "        items = await db.Set<$Entity>().AsNoTracking().$Query.ToListAsync();`r`n"
    $razor += "    }`r`n`r`n"
    $razor += "    private static string FaDate(DateTime? d)`r`n    {`r`n"
    $razor += "        if (!d.HasValue) return `"—`";`r`n"
    $razor += "        var cal = new System.Globalization.PersianCalendar();`r`n"
    $razor += "        return `$`"`$({cal.GetYear(d.Value):0000}/{cal.GetMonth(d.Value):00}/{cal.GetDayOfMonth(d.Value):00}`$)`";`r`n"
    $razor += "    }`r`n"
    $razor += "}`r`n"

    $file = Join-Path $dir "$($Route.Substring(0,1).ToUpper())$($Route.Substring(1)).razor"
    [System.IO.File]::WriteAllText($file, $razor, [System.Text.UTF8Encoding]::new($false))
    Write-Output ("created " + $file)
}

# NOTE: RowCells placeholders — @item razor syntax kept literally.
$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td><img src="@item.Image" width="110" style="border-radius:8px" /></td>' + "`r`n"
$rc += '                <td dir="ltr" style="text-align:right">@item.Link</td>' + "`r`n"
$rc += '                <td>@FaDate(item.LastDate)</td>'
Make-Page -Route "sliders" -Title "اسلایدرها" -Entity "Slider" -Query "OrderByDescending(s => s.LastDate)" -Heads @("#", "تصویر", "لینک", "تاریخ") -RowCells $rc

$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td>@item.Code</td>' + "`r`n"
$rc += '                <td>@item.Title</td>' + "`r`n"
$rc += '                <td>@(item.Percent.HasValue ? item.Percent.Value.ToString() + "٪" : item.Amount)</td>' + "`r`n"
$rc += '                <td>@item.Count</td>'
Make-Page -Route "discounts" -Title "تخفیف‌ها" -Entity "Discount" -Query "OrderByDescending(d => d.Id)" -Heads @("#", "کد", "عنوان", "درصد/مبلغ", "تعداد") -RowCells $rc

$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td>@item.Title</td>' + "`r`n"
$rc += '                <td dir="ltr" style="text-align:right">@item.FromEmail</td>' + "`r`n"
$rc += '                <td>@FaDate(item.SendDate)</td>'
Make-Page -Route "sentemails" -Title "ایمیل‌های ارسالی" -Entity "SentEmail" -Query "OrderByDescending(s => s.SendDate)" -Heads @("#", "عنوان", "فرستنده", "تاریخ") -RowCells $rc

$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td>@item.Text</td>' + "`r`n"
$rc += '                <td>@FaDate(item.ForwardDate)</td>'
Make-Page -Route "notifications" -Title "نوتیفیکیشن‌ها" -Entity "Notification" -Query "OrderByDescending(n => n.ForwardDate)" -Heads @("#", "متن", "تاریخ") -RowCells $rc

$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td>@item.Name</td>'
Make-Page -Route "cities" -Title "شهرها" -Entity "City" -Query "OrderBy(c => c.Name)" -Heads @("#", "نام شهر") -RowCells $rc

$rc = '                <td>@item.Id</td>' + "`r`n"
$rc += '                <td>@item.Name</td>'
Make-Page -Route "states" -Title "استان‌ها" -Entity "State" -Query "OrderBy(s => s.Name)" -Heads @("#", "نام استان") -RowCells $rc

Write-Output "ALL SIMPLE PAGES DONE"
