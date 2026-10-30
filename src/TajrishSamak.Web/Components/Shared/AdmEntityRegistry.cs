using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TajrishSamak.Web.Data.Entities;

namespace TajrishSamak.Web.Components.Shared;

public sealed record AdmFieldDef(string Name, string Label, string Type, bool Required = false, string? Folder = null)
{
    // Type: text | number | checkbox | textarea | image | date | select:states | select:categories
}

public sealed record AdmEntityDef(
    string Key,
    string Title,
    string ListUrl,
    AdmFieldDef[] Fields,
    Func<IDbContextFactory<TajrishSamak.Web.Data.TajrishSamakDbContext>, long, Task<object?>> Load,
    Func<IDbContextFactory<TajrishSamak.Web.Data.TajrishSamakDbContext>, long, IFormCollection, Task<bool>> Save);

/// <summary>
/// Whitelisted generic-CRUD entities. Every read/write goes through the explicit
/// typed lambdas below — no reflection on save, no unknown fields accepted.
/// </summary>
public static class AdmEntityRegistry
{
    private static string S(IFormCollection f, string k) => f[k].ToString().Trim();

    private static bool B(IFormCollection f, string k) =>
        f[k].Count > 0 && f[k][0] is "true" or "on" or "1";

    private static int I(IFormCollection f, string k)
    {
        var digits = new string(S(f, k).Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var v) ? v : 0;
    }

    private static DateTime D(IFormCollection f, string k)
    {
        var s = S(f, k);
        var map = new[] { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };
        s = new string(s.Select(c => c >= '0' && c <= '9' ? c : map.Contains(c) ? (char)('0' + Array.IndexOf(map, c)) : '/').ToArray());
        try
        {
            var parts = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3)
            {
                var cal = new System.Globalization.PersianCalendar();
                return cal.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), 0, 0, 0, 0);
            }
            return DateTime.Parse(s);
        }
        catch { return DateTime.Now; }
    }

    public static readonly Dictionary<string, AdmEntityDef> All = new(StringComparer.OrdinalIgnoreCase)
    {
        ["slider"] = new("slider", "اسلایدر", "/admin/sliders",
            new[]
            {
                new AdmFieldDef("Image", "تصویر دسکتاپ", "image", true, "sliders"),
                new AdmFieldDef("MobImage", "تصویر موبایل", "image", false, "sliders"),
                new AdmFieldDef("Link", "لینک مقصد", "text"),
                new AdmFieldDef("IsApp", "نمایش در اپلیکیشن", "checkbox"),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.Sliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                if (id > 0)
                {
                    var rows = await db.Sliders.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.Image, S(form, "Image"))
                            .SetProperty(x => x.MobImage, S(form, "MobImage"))
                            .SetProperty(x => x.Link, S(form, "Link"))
                            .SetProperty(x => x.IsApp, B(form, "IsApp"))
                            .SetProperty(x => x.LastDate, DateTime.Now));
                    return rows > 0;
                }
                db.Sliders.Add(new Slider
                {
                    Image = S(form, "Image"),
                    MobImage = S(form, "MobImage"),
                    Link = S(form, "Link"),
                    IsApp = B(form, "IsApp"),
                    CreateDate = DateTime.Now,
                    LastDate = DateTime.Now
                });
                await db.SaveChangesAsync();
                return true;
            }),

        ["category"] = new("category", "دسته‌بندی", "/admin/categories",
            new[]
            {
                new AdmFieldDef("PersianName", "نام فارسی", "text", true),
                new AdmFieldDef("EnglishName", "نام انگلیسی", "text"),
                new AdmFieldDef("ParentId", "دسته والد", "select:categories"),
                new AdmFieldDef("IsBlog", "دسته مقالات (بلاگ)", "checkbox"),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                int? parent = I(form, "ParentId") > 0 ? I(form, "ParentId") : null;
                bool isBlog = B(form, "IsBlog");
                if (id > 0)
                {
                    var rows = await db.Categories.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.PersianName, S(form, "PersianName"))
                            .SetProperty(x => x.EnglishName, S(form, "EnglishName"))
                            .SetProperty(x => x.ParentId, parent)
                            .SetProperty(x => x.IsBlog, isBlog));
                    return rows > 0;
                }
                db.Categories.Add(new Category
                {
                    PersianName = S(form, "PersianName"),
                    EnglishName = S(form, "EnglishName"),
                    ParentId = parent,
                    IsBlog = isBlog
                });
                await db.SaveChangesAsync();
                return true;
            }),

        ["discount"] = new("discount", "کد تخفیف", "/admin/discounts",
            new[]
            {
                new AdmFieldDef("Code", "کد تخفیف", "text", true),
                new AdmFieldDef("Title", "عنوان", "text"),
                new AdmFieldDef("Percent", "درصد تخفیف", "number"),
                new AdmFieldDef("Amount", "مبلغ ثابت (تومان)", "text"),
                new AdmFieldDef("Count", "تعداد مجاز", "number"),
                new AdmFieldDef("IsActived", "فعال است؟", "checkbox"),
                new AdmFieldDef("IsPublic", "عمومی است؟", "checkbox"),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.Discounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                float? percent = float.TryParse(S(form, "Percent"), out var p) && p > 0 ? p : null;
                if (id > 0)
                {
                    var rows = await db.Discounts.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.Code, S(form, "Code"))
                            .SetProperty(x => x.Title, S(form, "Title"))
                            .SetProperty(x => x.Percent, percent)
                            .SetProperty(x => x.Amount, S(form, "Amount"))
                            .SetProperty(x => x.Count, I(form, "Count"))
                            .SetProperty(x => x.IsActived, B(form, "IsActived"))
                            .SetProperty(x => x.IsPublic, B(form, "IsPublic")));
                    return rows > 0;
                }
                db.Discounts.Add(new Discount
                {
                    Code = S(form, "Code"),
                    Title = S(form, "Title"),
                    Percent = percent,
                    Amount = S(form, "Amount"),
                    Count = I(form, "Count"),
                    IsActived = B(form, "IsActived"),
                    IsPublic = B(form, "IsPublic")
                });
                await db.SaveChangesAsync();
                return true;
            }),

        ["city"] = new("city", "شهر", "/admin/cities",
            new[]
            {
                new AdmFieldDef("Name", "نام شهر", "text", true),
                new AdmFieldDef("StateId", "استان", "select:states"),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.Cities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                var stateId = I(form, "StateId");
                if (id > 0)
                {
                    var rows = await db.Cities.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.Name, S(form, "Name"))
                            .SetProperty(x => x.StateId, stateId));
                    return rows > 0;
                }
                db.Cities.Add(new City { Name = S(form, "Name"), StateId = stateId });
                await db.SaveChangesAsync();
                return true;
            }),

        ["state"] = new("state", "استان", "/admin/states",
            new[]
            {
                new AdmFieldDef("Name", "نام استان", "text", true),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.States.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                if (id > 0)
                {
                    var rows = await db.States.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.Name, S(form, "Name")));
                    return rows > 0;
                }
                db.States.Add(new State { Name = S(form, "Name") });
                await db.SaveChangesAsync();
                return true;
            }),

        ["notification"] = new("notification", "نوتیفیکیشن", "/admin/notifications",
            new[]
            {
                new AdmFieldDef("Text", "متن اعلان", "textarea", true),
                new AdmFieldDef("ForwardDate", "تاریخ ارسال", "date"),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                var text = S(form, "Text");
                if (id > 0)
                {
                    var hasDate = !string.IsNullOrWhiteSpace(S(form, "ForwardDate"));
                    var rows = await db.Notifications.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.Text, text)
                            .SetProperty(x => x.ForwardDate, hasDate ? D(form, "ForwardDate") : DateTime.Now));
                    return rows > 0;
                }
                db.Notifications.Add(new Notification
                {
                    Text = text,
                    ForwardDate = string.IsNullOrWhiteSpace(S(form, "ForwardDate")) ? DateTime.Now : D(form, "ForwardDate")
                });
                await db.SaveChangesAsync();
                return true;
            }),

        ["newsletter"] = new("newsletter", "عضو خبرنامه", "/admin/newsletters",
            new[]
            {
                new AdmFieldDef("Email", "ایمیل", "text", true),
            },
            async (f, id) => { await using var db = f.CreateDbContext(); return await db.NewsLetters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == (int)id); },
            async (f, id, form) =>
            {
                await using var db = f.CreateDbContext();
                var email = S(form, "Email");
                if (!email.Contains('@')) return false;
                if (id > 0)
                {
                    var rows = await db.NewsLetters.Where(x => x.Id == (int)id)
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.Email, email));
                    return rows > 0;
                }
                db.NewsLetters.Add(new NewsLetter { Email = email });
                await db.SaveChangesAsync();
                return true;
            }),
    };

    public static AdmEntityDef? Get(string key) =>
        string.IsNullOrWhiteSpace(key) ? null : All.TryGetValue(key.Trim(), out var v) ? v : null;
}
