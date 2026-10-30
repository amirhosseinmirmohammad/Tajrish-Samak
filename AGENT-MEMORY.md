# 🧠 AGENT-MEMORY — حافظه مشترک پروژه (Tajrish Samak V2)

> **دستورالعمل برای هر Agent:** این فایل حافظه مشترک همه agentهای این پروژه است.
> - **قبل از شروع هر کار این فایل را کامل بخوان.**
> - **پس از هر تغییر مهم (فیکس، فیچر، تصمیم، یافته)، یک رکورد جدید در بخش «تاریخچه» اضافه کن.**
> - تاریخ امروز را با فرمت `YYYY-MM-DD` بنویس. رکوردهای جدید را بالای تاریخچه اضافه کن (جدیدترین اول).
> - هرگز رکوردهای قبلی را حذف نکن؛ فقط اضافه کن یا اگر اطلاعات قدیمی غلط شد، اصلاح کن و بنویس «اصلاح شد».
>
> **🎯 دستور صریح کارفرما (2026-10-28):** کارفرما تأیید کرده که حافظه اشتراکی همین فایل MD باشد و خواسته «همیشه آپدیتش کن» — این تعهد برای همه sessionهای آینده الزامی است. ابزار Graffiti MCP در محیط نصب نیست؛ این فایل جایگزین رسمی آن است.

---

## 📌 شناسنامه پروژه

| مورد | مقدار |
|------|-------|
| پروژه | کلینیک شنوایی و سمعک شکوه تجریش — tajrish-samak.ir |
| مایگریشن | ASP.NET MVC (پوشه `legacy/`) → **.NET 10 + Blazor SSR** (پوشه `src/TajrishSamak.Web`) |
| دیتابیس | SQL Server — `TajrishSamakStore` (اسکیمای production قدیمی، **بدون migration جدید** — `IDbContextFactory` + `AsNoTracking`) |
| اجرا | `http://localhost:5001` (پروفایل http) — سرور production قدیمی: پورت 5301 از Visual Studio |
| الگوی CSS | بنفش `#920ab6` / صورتی `#c60069` / لایم `#d1f411` / فیروزه‌ای `#0ba6b8` — فونت‌ها IRANSans |
| فایل‌های CSS کلیدی | `wwwroot/css/gp-tajrish-modern.css` (گلوبال، ~7000 خط)، `gp-layout-legacy.css` (پچ‌های layout — **آخرین فایل base که لود می‌شود**)، `gp-page-*.css` (هر صفحه) |
| فایل‌های JS کلیدی | `gp-tajrish-modern.js` (تم شب/روز، reveal، اسلایدر، PWA)، `gp-layout-legacy.js` (لودر، زبان، خبرنامه، autocomplete)، `gp-home-legacy.js`، `gp-contact-legacy.js` (نقشه) |
| ساختار صفحات | Blazor SSR استاتیک + فقط `ContactForm` کامپوننت InteractiveServer است |

## ⚠️ نکات حیاتی که نباید فراموش شود

1. **`dotnet build` وقتی سرور در حال اجراست fail می‌شود** (apphost.exe قفل است). اول پروسه `TajrishSamak.Web` را با PID بکش (هرگز `Stop-Process -Name dotnet` نزن — شل خودت را می‌کشی! exit 255)، بعد بیلد، بعد `schtasks /Run /TN "TajrishSamakWatchdog"`.
2. **Watchdog**: تسک زمان‌بندی‌شده `TajrishSamakWatchdog` هر دقیقه پورت 5001 را چک می‌کند و اگر سرور پایین باشد بالا می‌آورد (اسکریپت: `D:\opencode-work\watchdog-tajrish.ps1`، لاگ: `D:\opencode-work\tajrish-server.log`).
3. **ترتیب اسکریپت‌ها در `App.razor`**: jQuery و بقیه در `<head>` به‌صورت blocking (قبل از بدنه) — چون اسکریپت‌های صفحه (gp-home/gp-contact) در زمان parse از `$` استفاده می‌کنند.
4. **Enhanced navigation غیرفعال است** (`Blazor.start({ disableEnhancedNavigation: true })`) — چون اسکریپت‌های jQuery صفحه‌به‌صفحه با DOM-swap سازگار نیستند. تمام ناوبری‌ها full page load است.
5. **Output cache پایه (۱۰ دقیقه) روی همه GETها فعال است** — endpointهایی که می‌نویسند (مثل خبرنامه) حتماً `.CacheOutput(p => p.NoCache())` بگیرند.
6. **CSS با `!important` زیاد است** — قبل از هر استایل جدید، cascade را در مرورگر با `elementFromPoint` / matched-rules تست کن.
7. **HTML صفحات حالا `Cache-Control: no-cache` دارند** (middleware در Program.cs) — برای جلوگیری از کش شدن HTML قدیمی با fingerprint قدیمی در مرورگر.
8. **تب مخفی مرورگر**: IntersectionObserver، transition، lazy-image و تایمرهای debounce کار نمی‌کنند/throttle می‌شوند — نتایج تست در تب مخفی قابل اعتماد نیست.
9. **CDNهای خارجی (unpkg و...) از ایران ناپایدارند** — همه کتابخانه‌ها را self-host کن (نمونه: Leaflet در `wwwroot/lib/leaflet/`).
10. **فوتر هیچ‌وقت نباید reveal/animat داشته باشد** — محتوای حیاتی (تماس + سئو) است. کلاس `gp-reveal` در مارکاپ فوتر ممنوع.

## ✅ وضعیت فیچرها (آخرین بازبینی: 2026-10-29)

| فیچر | وضعیت | محل |
|------|-------|-----|
| **پنل ادمین Blazor** (۲۱+ صفحه) | ✅ کامل | `Pages/Admin/*.razor` + `AdminLayout.razor` + `gp-admin.css` |
| لاگین ادمین (cookie auth) | ✅ | `/admin/login` + `/admin/do-login` endpoint |
| پروفایل مدیر (ویرایش + رمز) | ✅ | `/admin/profile` + `/admin/save/profile` |
| CRUD مقالات (لیست+حذف+toggle+ویرایش+جدید) | ✅ | `/admin/blogs` + `BlogEdit.razor` |
| CRUD محصولات (لیست+حذف+ویرایش+جدید) | ✅ | `/admin/products` + `ProductEdit.razor` |
| CRUD خدمات (لیست+toggle+حذف+ویرایش+جدید) | ✅ | `/admin/services` + `ServiceEdit.razor` |
| CRUD برندها (جدول DB) | ✅ | `/admin/brands` + `BrandEdit.razor` |
| CRUD دسته‌بندی‌ها | ✅ | `/admin/categories` |
| CRUD اسلایدرها | ✅ | `/admin/sliders` |
| CRUD تخفیف‌ها | ✅ | `/admin/discounts` |
| CRUD گالری‌ها | ✅ | `/admin/galleries` |
| شهرها/استان‌ها | ✅ | `/admin/cities` + `/admin/states` |
| تراکنش‌ها/پرداخت‌ها | ✅ | `/admin/transactions` + `/admin/payments` |
| Autocomplete جستجو (عکس‌دار) | ✅ | `gp-layout-legacy.js` + `/search/suggest` |
| نقشه تماس (Leaflet self-host) | ✅ | `wwwroot/lib/leaflet/` |
| صفحات برند `/hearing-aid-brands/{slug}` | ✅ | `Brands.razor` |
| خبرنامه (پیام رنگی) | ✅ | `/Home/NewsLetter` endpoint |

## ❌ جامانده از مایگریشن

- صفحه ورود کاربر عادی (ثبت‌نام با کد پیامکی)
- صفحه پروفایل کاربر عادی (سفارشات، علاقه‌مندی‌ها)
- صفحه invoice (فاکتور سفارش‌ها)
- پنل مدیریت محتوای کامل (ویرایش بدنه مقالات/خدمات با rich text editor)

## ❌ جامانده از مایگریشن (تصمیم گرفته نشده)

- **Account**: ثبت‌نام/ورود با کد پیامکی، پروفایل، علاقه‌مندی‌ها (کل زیرساخت Identity نیست)
- **Manage**: مدیریت پروفایل کاربر (رمز، شماره، 2FA)
- **invoice**: فاکتور سفارش‌ها
- کاربر هنوز تصمیم نگرفته؛ پیشنهاد: پورت کامل Identity به‌صورت پروژه جداگانه

---

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-29 — V105 ردیف‌های تماس جمع‌وجور
- هر li تماس فوتر: `width: fit-content` + `margin-left: auto` (چسبیده راست) + flex rtl با gap ۱۰ → آیکن راست، متن چسبیده، همه ردیف‌ها هم‌لبه. تست: rightEdges هر ۴ = ۳۹۹ ✓.

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-29 — فیکس نهایی گرید فوتر (V104)
- ریشه: `gp-layout-legacy.css` با !important ستون‌های minmax(180px+...) برای `.gp-footer-grid-v80` داشت → در عرض‌های <۱۰۰۰px ستون‌ها جا نمی‌شدند.
- V104 (انتهای همان فایل، !important): ۴ستون ≥۱۱۰۱px / ۲ستون ۶۴۱-۱۱۰۰ / ۱ستون ≤۶۴۰ + min-width:0 ستون‌ها + جداکننده موبایل. تست ۸۳۰px: ۲ ستون ۳۱۷px بدون سرریز + legal وسط.
- درس: تست «تداخل» گرید نباید bottom>top هم‌ردیفی را تداخل بگیرد؛ فقط horizontal clamp چک شود.

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-29 — تکمیل CRUD پنل ادمین (۱۳ صفحه جدید)
- صفحات جدید: Blogs (toggle نمایش+حذف)، Products (قیمت/موجودی/تخفیف ادمین)، Services (toggle+حذف)، Brands(جدول)، Categories، Sliders، Discounts، Galleries، SentEmails، Notifications، Cities، States + Transactions/Payments فقط-خواندنی.
- endpoint عمومی `/admin/delete` (switch typed ۱۵ entity + catch FK) و `/admin/toggle` (raw SQL flip IsVisible برای Blogs/Services) — هردو antiforgery + redirect-back.
- سایدبار ادمین گروه‌بندی شد (فروشگاه/محتوا/ارتباط/پایه) + `.adm-nav-sep`.
- تست: ۲۱ صفحه ادمین بدون کوکی → همه ریدایرکت به لاگین ✓ (Authorize enforced).
- درس PS: here-string هدر نمی‌تواند inline در آرگومان باشد؛ مسیر مطلق در اسکریپت‌ها.

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-29 — 🛡 پنل ادمین Blazor + سکوریتی کامل سایت
- **احراز هویت**: Cookie auth + `LegacyPasswordHasher` (Verify V2 حافظه Identity 2 = PBKDF2-SHA1 1000iter + fallback V3) — کاربران قدیمی با همون پسورد وارد می‌شوند (IdentityConfig سفارشی‌سازی نداشت ✓). نقش‌ها از AspNetUserRoles/AspNetRoles (انتیتی‌های سبک + HasKey مرکب — بدون PK کل سایت ۵۰۰ می‌شد!).
- **لاگین** `/admin/login`: SSR فرم + antiforgery + خطاهای تفکیک‌شده (e=1..4) + rate limit 5/10min per-IP + قفل legacy + IsActive check. فقط نقش Administrator → /admin. خروج GET /admin/logout. ریدایرکت legacy: /Account/Login، /Account/AdminPanel.
- **صفحات ادمین** (Layout چسبان سایدبار تیره + [Authorize(Roles=Administrator)]): داشبورد (۶ آمار واقعی + چارت ۷روزه ثبت‌نام/سفارش CSS خالص)، Users (جستجو + نقش‌ها + وضعیت)، Messages/Newsletters (حذف با antiforgery فرم)، Comments (تایید/حذف)، Orders (لیست). فرم‌های ادمین SSR هستند (کوکی در HTTP response ست می‌شود — interactive circuit جواب نمی‌دهد).
- **سکوریتی سای**: هدرها (CSP pragmatic/nosniff/DENY/Referrer/Permissions)، RateLimiter (write-api 10/min, suggest 40/min, login 5/10min)، UseHttpsRedirection، UseAuthentication/UseAuthorization بعد از MapStaticAssets. مثلث **دقت کن: [Inject] تکراری در razor = CS0102**.
- تست E2E: ۲۰ مسیر همه ۲۰۰، ریدایرکت‌های slug محصول/بلاگ ۳۰۱، فوتر contact وسط‌چین (تست geometric)، هدرها روی / حاضر.

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-28 — بازطراحی فروشگاهی محصولات (لیست + جزئیات)
- **Products.razor (gpc-*)**: هیرو تیره با آمار (محصول/تخفیف‌دار/دسته‌ها — اعداد واقعی)، سایدبار sticky دسته‌ها با active state + CTA تست، محصولات با **قیمت تومان + بج ٪تخفیف + del قیمت قبلی**، مرتب‌سازی تخفیفی اول، ItemList JSON-LD، زیرگروه‌ها به‌صورت کارت افقی، empty/pager/CTA مدرن، ریسپانسیو ۲ستونه موبایل.
- **ProductDetails.razor (gppd-*)**: اسپلیت media/info، بج ٪تخفیف روی عکس، چیپ دسته/برند/موجودی، **باکس قیمت با del و «سود شما»**، دکمه تماس+واتساپ، share، جدول مشخصات، prose توضیحات، مرتبط‌ها. Product JSON-LD حفظ شد (offers/brand/sku) + BreadcrumbList.
- درس: بازنویسی razor یادت نره [Inject]ها — CS0103 Http.

## 📜 تاریخچه (جدیدترین اول)

### 2026-10-28 — بازطراحی خدمات (لیست + جزئیات) مثل بلاگ‌ها
- **ServiceList.razor بازنویسی**: هیرو گرادیانی با آمار (تعداد خدمت/گروه)، کارت‌ها با عکس ۱۶:۱۰ + بج دسته + دکمه «مشاهده و مشاوره» به‌صورت دکمه واقعی، CTA پایانی «پیدا نکردید؟ تماس بگیرید»، ItemList JSON-LD برای سئو، عنوان H1 با نام دسته هنگام فیلتر.
- **ServiceDetails.razor بازنویسی کامل**: breadcrumb، هیرو با چیپ دسته + متا (بازدید فارسی/تاریخ) + ۴ دکمه اشتراک‌گذاری + CTA تماس، فیگور عکس، prose styling بدنه، خدمات مرتبط کارت‌دار، CTA گرادیانی پایانی. **شمارنده Survey حفظ شد** (UPDATE اتمیک legacy). سئو: og/twitter کامل + Service JSON-LD با areaServed/serviceType/image.
- رکورد نکته: `@` در URL داخل Razor (نشان/هر لینک خارجی) باید `@@` باشد.
- CSS: V99 به `gp-page-Service_All.css` و `gp-page-Service_Index.css` اضافه شد (بعد از base فایل صفحه لود می‌شود).

### 2026-10-28 — کلیدواژه‌های خراب دیتابیس (دیتای `????`)
- **دیتای قدیمی MetaKey برخی مقاله‌ها در DB خودش `????` ذخیره شده** (خرابی تاریخی انکودینگ) — رندر می‌شد `#????`.
- فیکس: فیلتر توکن‌ها با `t.Any(char.IsLetter)` (حروف فارسی/لاتین) — اگه هیچ کلیدواژه معتبری نبود، **هم باکس تگ‌ها و هم متا تگ Keywords حذف می‌شوند**. محاسبه gpTags به OnParametersSetAsync منتقل شد (هم head هم body از یک لیست).

### 2026-10-28 — امکانات کامل مجله: زمان مطالعه دقیق، TOC، نوار پیشرفت، لایک، تگ، قبلی/بعدی، فرم نظرات
- **زمان مطالعه دقیق**: حذف بلاک‌های script/style + strip تگ‌ها + `HtmlDecode` + شمارش کلمات واقعی ÷ ۱۸۰ wpm.
- **لایت‌باکس + کپشن hover-only** (رکورد قبلی).
- **BlogDetails جدید**: نوار پیشرفت مطالعه (`gpReadProgress` ثابت بالای صفحه)، **فهرست مطالب خودکار** از h2ها (فقط ≥۲ تیتر، لینک‌دار با scroll-margin)، **لایک/دیسلایک** با endpoint POST `/blog/{id}/vote` (افزایش Like/DissLike در DB + محافظت localStorage یک‌رای‌برمرورگر + شمارنده فارسی)، **چیپ کلیدواژه‌ها** از MetaKey (لینک به /Blog/All?q=)، **مقاله قبلی/بعدی** بر اساس تاریخ، **فرم ثبت دیدگاه** (کامپوننت InteractiveServer `BlogCommentForm.razor` → insert با IsApprove=false برای تایید ادمین + پیام موفقیت).
- endpoint رای: `internal record BlogVoteRequest` حتماً **بعد از همه top-level statementها** در Program.cs (CS8803).
- تست زنده: POST رای → `{ok:true, likes:1}` + کلیک واقعی دکمه → «رای شما ثبت شد ✓».
- درس: سرور dev بین پیام‌های کاربر ممکن است پایین بیاید (502) — اول چک پورت، بعد `schtasks /Run /TN TajrishSamakWatchdog`.

### 2026-10-28 — بازطراحی کامل مجله (لیست + جزئیات) با سئو/AEO و اشتراک‌گذاری
- **باگ مهاجرت قبلی**: regex جایگزینی @Assets باعث `/> >/>` دوبل در ۱۳ صفحه شده بود (به‌صورت متن ">" رندر می‌شد) → پاک‌سازی با regex `/> \s* />` → `/>`.
- **BlogList.razor بازنویسی**: هیرو گرادیانی با آمار (تعداد مقاله/پربازدید)، کارت‌ها با بج دسته + تاریخ شمسی + خلاصه، سایدبار پربازدیدها با thumbnail + رتبه + بازدید، JSON-LD نوع Blog + Breadcrumb، فیلتر دسته حالا نام دسته را در H1 نشان می‌دهد.
- **BlogDetails.razor بازنویسی کامل**: Breadcrumb بصری، هیرو با چیپ دسته + متا (تاریخ شمسی/بازدید/زمان مطالعه از تعداد کلمات ÷۱۸۰)، **۵ دکمه اشتراک‌گذاری** (تلگرام/واتساپ/X/ایمیل/کپی لینک با clipboard)، فیگور عکس اصلی، **prose styling** برای HTML قدیمی دیتابیس (h2 با خط گرادیانی، لیست‌ها با بول گرادیانی، جدول، تصاویر گرد)، **مقالات مرتبط** (هم‌دسته با fallback پربازدیدها)، کامنت‌ها با آواتار حرف اول، ۴۰۴ + noindex برای مقاله ناموجود.
- **SEO/AEO کامل**: og:image/description، article:published_time، twitter:summary_large_image، BlogPosting JSON-LD با mainEntityOfPage/image/publisher logo/interactionStatistic (بازدید)، Blog JSON-LD در لیست. نکته Razor: `@` در URL نشان (neshan.org/maps/@...) باید `@@` باشد.
- reveal selector برای .gp-blog-card و المان‌های مقاله گسترش یافت.

### 2026-10-28 — گالری: کپشن فقط hover + نکته کش مرورگر برای کارفرما
- کپشن گالری About فقط روی **hover دسکتاپ** نمایش داده می‌شود (موبایل/تاچ کاملاً مخفی) تا عکس‌ها تمیز دیده شوند. لایت‌باکس با کلیک شبیه‌سازی‌شده تست شد: باز می‌شود، عکس درست را لود می‌کند (IMG_5885) و دکمه بستن کار می‌کند.
- **درس مهم برای گزارش‌های «کار نمی‌کند» از کارفرما**: مرورگر کارفرما ممکن است HTML/JS قدیمی (قبل از آخرین دیپلوی) را از کش نمایش دهد. اولین قدم عیب‌یابی: Ctrl+F5 (رفرش کامل) + چک اینکه URL همان localhost:5001 است.

### 2026-10-28 — بازطراحی اختصاصی برندها (شوروم + لندینگ)
- `Brands.razor` بازنویسی با کلاس‌های اختصاصی `gpb-*` + CSS اختصاصی جدید `gp-page-Brands.css` (قبلاً از کلاس‌های Product_All قرض می‌گرفت).
- شوروم: هیرو تیره premium لایم/فیروزه‌ای + آمار، ۱۵ کارت برند با تایل حرف گرادیانی (۴ لهجه رنگی چرخشی)، فلش hover، نوار underline متحرک، بند راهنمای انتخاب.
- لندینگ برند: هیرو با تایل بزرگ + ۴ لهجه رنگی per-brand، گرید محصولات، سایدبار سایر برندها (active state) + CTA تست شنوایی، empty state، صفحه‌بندی.
- سئو/AEO: OG/Twitter کامل، BreadcrumbList JSON-LD، Keywords فیلترشده. reveal برای gpb-*.

### 2026-10-28 — 🚨 خرابی فایل CSS جزئیات محصول (C# داخل CSS)
- فایل `gp-page-Product_Details.css` **۶۱۶۷ بایت اولش با کد C# (Regex.Replace/RegexOptions) بازنویسی شده بود** → پارسر CSS کل فایل را دور ریخت → صفر رول اعمال می‌شد («اصلا استایل نگرفته»). ترمیم: truncate تا اولین `:root` (بایت ۶۱۶۷) + rebuild.
- بعد از ترمیم: ۲۵۲ رول در CSSOM، ۷۸ رول gppd اعمال، radius/shadow درست.
- **درس**: خروجی شل‌ها را هرگز مستقیم داخل فایل‌های سورس ننویس بدون verify؛ بعد از هر append/dark لایه CSS، بالانس آکولاد + parse test (`new CSSStyleSheet().replaceSync(prefix)`) لازم است.
- ریدایرکت محصول/بلاگ: ۳۰۱ با `Uri.EscapeDataString(Slugify)` در Location (فارسی خام در هدر = کرش). مرورگر فارسی را decoded نمایش می‌دهد.

### 2026-10-28 — URL اسلاگ‌دار محصولات + حذف قیمت + فیکس ۵۰۰ ریدایرکت
- قیمت/تخفیف از **لیست و جزئیات محصول کلاً حذف** شد (کارفرما). جزئیات: باکس مزایا (گارانتی/تنظیم/پیگیری) + بج برند روی عکس (یا «اصالت کالا تضمینی» اگر بی‌برند). لیست: چیپ برند+دسته + چیپ‌های مزیت + دکمه واتساپ (tel در دسکتاپ کار نمی‌کرد).
- **URL جدید**: `/Product/Details/{id}/{slug-farsi}` — Slugify helper (حروف/ارقام نگه، فاصله→خط تیر). لیست/برندها/sitemap همه لینک اسلاگ‌دار.
- **فیکس ۵۰۰ ریدایرکت**: `Response.Redirect` با Location حاوی فارسی خام → کرش (هدر HTTP باید ASCII باشد). حل: `Uri.EscapeDataString(Slugify(...))` در Location + 301 permanent. `NavigationManager.NavigateTo` در static SSR هم کار می‌کند (۳۰۲) ولی ۳۰۱ بهتر است.
- متا Keywords هم با فیلتر حروف — در صورت نبود کلیدواژه معتبر omit.

### 2026-10-28 — گالری و زوم جزئیات محصول + قاب قشنگ
- گالری چندعکسی محصول: SiteFirst/Second/ThirdImage + SliderImage (dedupe) → عکس اصلی + بندانگشتی‌ها (فقط اگر >۱ عکس).
- **زوم برای تک‌عکس هم کار می‌کند** (باگ اولیه: شرط thumbs.length کل بلاک را skip می‌کرد → sources fallback به main.src).
- قاب قشنگ: پس‌زمینه گرادیانی ملایم بنفش/آبی، padding، hover زوم ملایم عکس، نشانگر ⤢، عکس contain با max-height ۴۳۰ (دیگر غول‌آسا نیست).
- لایت‌باکس global `.gp-lightbox` reuse (CSS از tajrish-modern) + دکمه‌های ناوبری فقط وقتی >۱ عکس.
- CSS V102 در gp-page-Product_Details.css.

### 2026-10-28 — فیکس «خدمات مرتبط» صفحه خدمت
- باکس مرتبط‌ها از کلاس‌های `.gp-related-card/.gp-related-grid` استفاده می‌کرد که **فقط در gp-page-Blog_Index.css** تعریف شده بودند → روی صفحه خدمت بدون استایل منفجر می‌شد (عکس native-size + تیتر شناور). استایل‌ها (V101b) به `gp-page-Service_Index.css` اضافه شد + `max-width:100%` برای img داخل بدنه.
- **درس**: CSS فینگرپرینت‌شده (@Assets) بعد از هر append نیاز به rebuild دارد — فایل plain link فوراً اعمال می‌شود.

### 2026-10-28 — بازطراحی «شوکیس» جزئیات خدمت (کاملاً متفاوت)
- طرح جدید `gsd-*`: **هیرو دوستونی گرادیانی تیره‌تر** (کپی راست + عکس شناور کج ۱.۲- درجه با بج شیشه‌ای بازدید)، **گرید body + سایدبار sticky** (باکس «چطور دریافت کنم؟» ۳ مرحله‌ای + دکمه‌های تماس/واتساپ + ردیف اشتراک‌گذاری)، مرتبط‌ها، CTA پایانی.
- sticky panel در ≤۹۸۰px می‌شود static و با `order: -1` می‌آید بعد از هیرو (UX موبایل).
- **ریشه «بیلد شد ولی قدیمی سرو می‌شد»**: چند پروسه TajrishSamak.Web همزمان (watchdog + قدیمی) — فقط یکی پورت دارد. حل: kill همه پروسه‌های TajrishSamak.Web → چک `Get-NetTCPConnection -LocalPort 5001` → بیلد → watchdog.
- CSS V101 در `gp-page-Service_Index.css`.

### 2026-10-28 — دکمه‌های مسیریابی تماس (گوگل مپ/ویز/نشان) + لایت‌باکس گالری
- **مسیریابی با نشان** اضافه شد: `https://neshan.org/maps/@@lat,lng,16z` (در Razor حتماً `@@` — تکی `@` خطای RZ1005 می‌دهد). هر دو بلوک `.gp-direction-buttons` حالا ۳ دکمه با آیکن‌اند: 🗺️ گوگل مپ / 🚙 ویز / 🧭 نشان. گرید ۳ستونه + hover گرادیانی (CSS V99 در gp-page-ContactUs.css).
- **لایت‌باکس گالری About**: کلیک روی هر عکس → تمام‌صفحه (ماژول در `gp-tajrish-modern.js` فقط با وجود `.gp-about-gallery-grid` فعال می‌شود). ناوبری قبلی/بعدی + کیبورد (Esc/فلش‌ها) + شمارنده فارسی «۳ / ۱۲» + جلوگیری از اسکرول body.
- **کپشن گالری کوچک شد** (بج ۹px + تیتر ۱۰.۵px + گرادیان کوتاه) که عکس‌ها دیده شوند + نشانگر ⤢ zoom-in گوشه هر کارت هنگام hover + `cursor: zoom-in`.
- درس: `Get-Content | Set-Content` بدون encoding صریح خطر خرابی UTF-8 دارد — همیشه `[System.IO.File]::ReadAllText/WriteAllText` با UTF8.

### 2026-10-28 — مهاجرت CSS/JS صفحات به @Assets + فیکس رنگی تماس + گالری بنتو
- **ریشه باگ «CSS قدیمی»**: لینک‌های CSS/JS صفحات plain بودند (`href="css/gp-page-X.css"`) و مرورگر آن‌ها را heuristic کش می‌کرد → تغییرات CSS دیده نمی‌شد. **۱۳ صفحه** به `@Assets[...]` (fingerprint خودکار) مهاجرت کردند. درس: هر reference جدید استاتیک حتماً `@Assets`.
- **باج regex**: جایگزینی اسکریپت‌ها باعث `</script>` دوبل در ContactUs/Home شد → RZ9981 → دستی فیکس شد.
- **تماس (CSS V98)**: آیکن‌های لیست گرادیانی بنفش-صورتی با گلیف سفید، دکمه‌های گوگل‌مپ/ویز یکدست شیشه‌ای (hover گرادیانی)، سوشال‌ها یکدست (قبلاً rainbow بود) — همه با specificity `.gp-contact-page ...` + `!important` چون رول‌های قدیمی per-child بودند.
- **گالری About (CSS V98)**: گرید بنتو موزاییک ۴ستونه (featured 2×2) با `grid-auto-rows`، حذف `min-height: 430px` قدیمی، کپشن همیشه‌روشن، ریسپانسیو ۴→۳→۲→۱ ستونه. علت به‌هم‌ریختگی: تداخل min-height قدیمی با aspect-ratio جدید.

### 2026-10-28 — فوتر در صفحات داخلی + نقشه + مدرن‌سازی About/Contact
- **باگ فوتر (ریشه‌ای)**: کلاس `gp-reveal` هاردکد در مارکاپ `MainLayout.razor` باعث می‌شد فوتر با opacity:0 متولد شود و در صفحات داخلی دیده نشود. کلاس حذف شد + فوتر از selector reveal در `gp-tajrish-modern.js` خارج شد. فونت فوتر الان همیشه opacity:1 در همه صفحات.
- **نقشه تماس**: Leaflet از unpkg (CDN خارجی) لود نمی‌شد → **self-host شد** در `wwwroot/lib/leaflet/` (js+css+marker icons). `ContactUs.razor` به `@Assets["lib/leaflet/..."]` تغییر کرد.
- **About/Contact مدرن (CSS V97)**: هیرو گرادیانی + آمار شیشه‌ای، کارت یادبود با نقل‌قول، گالری hover-zoom، چیپ‌های ارزش، فرم مدرن، CTA گرادیانی، حالت شب کامل. CSS به **فایل خود صفحه** اضافه شد (بعد از base لود می‌شود → cascade برنده).
- reveal selector برای المان‌های About/Contact هم گسترش یافت.

### 2026-10-28 — Autocomplete عکس‌دار + باگ‌های پنهان
- عکس واقعی محصول/خدمت/مقاله و لوگوی برند (از جدول Brands) به پیشنهادها اضافه شد؛ برند بدون لوگو → تایل گرادیانی fallback. `SuggestItem` فیلد `Img` گرفت.
- **باگ سرریز متن**: `overflow: visible` روی host + نگه‌داشتن گردی در `.form-search` + `z-index: 999` برای `.header-bot` (بالای nav sticky 998).
- **حالت لودینگ sync** («در حال جستجو...») بلافاصله بعد از تایپ + حالت «نتیجه‌ای پیدا نشد» + هدر/فوتر راهنما در پنل.
- **باگ مرورگر-کش HTML**: صفحات HTML بدون Cache-Control بودند و مرورگر HTML قدیمی (با fingerprint قدیمی) را کش می‌کرد → middleware `no-cache` برای text/html اضافه شد.

### 2026-10-28 — خبرنامه: باگ Output Cache + رنگ پیام‌ها
- **Output cache پایه** جواب GET خبرنامه را کش می‌کرد → کلیک دوم هیچ‌وقت به سرور نمی‌رسید → `.CacheOutput(NoCache)` + `Cache-Control: no-store`.
- رنگ پیام‌ها: موفقیت **لایم `#d1f411`**، خطا **صورتی `#ff8ac4`** (کلاس `is-error`). علت قبلی: رول `.gp-footer-newsletter #msg { color: var(--gp-v5-purple) !important }` + کارت تیره جدید فوتر.
- Enter در input خبرنامه حالا عضویت را trigger می‌کند (قبلاً فرم به /search submit می‌شد).
- EN/FA و حالت شب از **فوتر حذف شد** (فقط در هدر).

### 2026-10-28 — فوتر تیره premium + مدرن‌سازی Home
- فوتر: کارت شیشه‌ای تیره + نوار aurora متحرک + پنل AEO شیشه‌ای با چیپ‌های کلیدواژه (استایل در `gp-layout-legacy.css` V54 بلاک — فایل آخرین base است).
- Home: سکشن «مسیر شنوایی شما» (تایم‌لاین ۴ مرحله‌ای `gp-journey-v93`) + «آمار در یک نگاه» (`gp-stats-v93` با اعداد واقعی GeoCatalog: ۱۵ برند، ۱۰۶ محدوده، ۵۱ خدمت) + `content-visibility: auto` روی سکشن‌های زیر فولد.
- منوی بالای سایت: پیل شیشه‌ای شناور + sticky + underline گرادیانی (`.top_nav_left { display:flow-root }` — باگ له‌شدن هیرو به ۲۸۰px به‌خاطر فرار float منو بود).

### 2026-10-28 — رفع باگ‌های بنیادین استایل و JS
- **هیرو له شده (۲۸۰px)**: `.navbar-default { float:right }` از `.top_nav_left` فرار می‌کرد (height:0) → `display:flow-root` + `float:none`. روش تشخیص: sheet-disable bisect + elementFromPoint.
- **`gp-layout-legacy.js` اصلاً parse نمی‌شد**: سینتکس Razor (`@Url.Action`) داخل JS استاتیک → endpoint `/Home/NewsLetter` در Program.cs ساخته شد (پورت ۱:۱ از HomeController قدیمی).
- **`gp-tajrish-modern.js` اصلاً لود نمی‌شد** → به App.razor اضافه شد (reveal فوتر، تم، اسلایدر، PWA).
- **`gp-contact-legacy.js`**: `}` اضافه انتهای فایل → syntax error → نقشه کار نمی‌کرد.
- **ترتیب اسکریپت‌ها**: jQuery به `<head>` منتقل شد + `disableEnhancedNavigation: true` (با `autostart="false"` روی blazor.web.js).
- **`gp-contact-legacy.js` نقشه**: بعد از فیکس brace تأیید شد ۱۲ tile لود می‌شود.

### 2026-10-28 — صفحات ۴۰۴ جامانده
- `/hearing-aid-brands` + `/hearing-aid-brands/{slug}` → `Brands.razor` ساخته شد (لیست برند + لندینگ با جستجوی UNION مثل قدیمی، صفحه‌بندی ۹تایی). `LegacyBrands.cs` گسترش یافت (Slug/PersianName/EnglishName/SearchTerms).
- `/Home/Services/{id}` → alias در ServiceList.razor.
- حالت‌های not-found → `<meta name="robots" content="noindex">`.

### 2026-10-28 — شروع پروژه: ممیزی مایگریشن MVC → Blazor
- ممیزی کامل legacy در برابر Blazor؛ تست همه‌چیز با مرورگر واقعی (CDP) روی localhost.
- Best practiceها اعمال‌شده: `IDbContextFactory`، output cache، brotli/gzip، sitemap داینامیک، MapStaticAssets، JSON-LD حفظ شده.

---
*این فایل توسط agent به‌روزرسانی می‌شود. آخرین به‌روزرسانی: 2026-10-28*
