
## 2026-10-29 — Generic CRUD engine + security hardening
- **CRITICAL GOTCHA**: DbContext has global UseQueryTrackingBehavior(NoTracking) (Program.cs line ~28). Any FirstOrDefaultAsync WITHOUT .AsTracking() returns a DETACHED entity -> property mutations are SILENTLY DROPPED by SaveChangesAsync (no error!). INSERT via Add() works fine. Fixed by adding .AsTracking() to every mutating endpoint. Generic registry uses ExecuteUpdateAsync instead.
- **AdminGuard middleware** (after UseAuthentication): every /admin path needs Administrator role; POST -> 401, GET -> redirect login. /admin/login + /admin/do-login exempt.
- **OutputCache**: base 10min policy was caching /admin pages (stale data). NoAdminCachePolicy (Components/Shared) disables caching for /admin + authenticated users.
- **Generic CRUD**: Components/Shared/AdmEntityRegistry.cs (whitelist: slider, category, discount, city, state, notification, newsletter) + EntityEdit.razor at /admin/edit/{entity}/{id?}. Save endpoint: POST /admin/save/entity. Image fields use .gpc-upload-zone markup; jalali inputs use class gp-jalali.
- **Extra endpoints**: POST /admin/order/status (done|cancel|reopen), POST /admin/user/toggle (self-deactivation blocked), POST /admin/payment/block.
- AdmListBase (Components/Shared) = query-string driven list pages (q/page/sort/dir). [SupplyParameterFromQuery] does NOT bind on base classes -> pages declare proxy props (QP/PageQP/SortQP/DirQP) writing to base fields.

## 2026-10-29 — PRODUCTION DEPLOYED LIVE (https://tajrish-samak.ir)
- Server: 87.107.104.81 (RDP port 15226), WinRM 5985 enabled for remote admin. Win Server 2019 DC, host WIN-PUD9TJSCPQ8.
- IIS sites: Tajrish-Samak -> C:\Websites\tajrish-deploy (pool: TajrishSamakCore, No Managed Code), OLD mvc site kept at C:\Websites\TajrishSamak (+ backup C:\Backups\TajrishSamak). Other sites MirMohammad/Motoreto share the box - DO NOT touch their pools.
- Production DB: TajrishSamak (sql auth samakdbuser, password in server appsettings.Production.json - masked). Tables: AspNetUsers (not Users!). 12 products, 70 blogs (ids 3077+).
- GOTCHAS learned live:
  1. MapStaticAssets serves ONLY files present at publish time -> robocopy'd/uploaded files 404. FIX: app.UseStaticFiles() fallback after MapStaticAssets.
  2. StatusCodePages ReExecute to /not-found + OutputCache hit => InvalidOperationException 'RemoteNavigationManager already initialized' (500). FIX: /not-found excluded from output cache (NoAdminCachePolicy).
  3. In-process DLL hot-swap: Stop pool + kill w3wp (-ap TajrishSamakCore) then copy; file stays locked otherwise.
  4. Global NoTracking: mutating endpoints need .AsTracking() (see 2026-10-29 note above).
- Hotfix flow used: publish locally -> zip dll+pdb -> base64 chunks via WinRM -> stage/backup/swap -> pool restart. DLL backups kept on server (TajrishSamak.Web.dll.bak-*).
- fix-server.ps1 in both deploy folders installs runtime + configures IIS (was used).
- SECURITY: administrator RDP password was shared in chat - MUST be rotated.

## 2026-10-29 (2) — product page redesign + mobile safety net + SEO/GEO/AEO live fixes
- ProductDetails V110: 3-zone grid (thumb rail 64px sticky | media | sticky info 330-440px), .no-thumbs variant, container 1320px. Gallery = SiteFirst/Second/Third + SliderImage.
- 'ناموجود' removed from ALL public pages (stock badge only when in stock; specs row says 'ثبت سفارش با تماس'). Admin still shows it.
- MOBILE SAFETY NET appended to gp-tajrish-modern.css: collapses all 2-col shells (blog/contact/geo/terms/brands/category/gpc/finder) <=900px with !important.
- **DEPLOY GOTCHAS (production)**:
  1. @Assets emits RELATIVE css hrefs -> 404 on deep routes (slugs). FIXED: all 40 page razor files now use plain href='/css/x.css?v=7'. NEVER go back to @Assets for page css.
  2. MapStaticAssets serves BUILD-TIME css content (from .br/.gz + staticwebassets.endpoints.json) - server-side css file edits are ignored until republish. Deploy = dll + endpoints.json + whole wwwroot\css together.
  3. WinRM chunk transfer: '+' in base64 gets corrupted -> use base64url (replace + with -, / with _) + verify each chunk length + final MD5 before decode.
  4. Hotfix flow: publish -> zip {dll} -> base64url chunks (100KB, verified) -> server decode -> stop pool -> kill w3wp -> backup dll as .bak-* -> swap -> start pool. Temp staging folders (hotfix*-tmp) are disposable.
- SEO/GEO/AEO live status: robots+sitemap ok; JSON-LD on every page: MedicalBusiness+GeoCoordinates+areaServed+SpeakableSpecification+FAQPage+BreadcrumbList (+Product/Offer/BlogPosting on their pages); openingHoursSpecification added (Sat-Thu 09-20 - VERIFY REAL CLINIC HOURS!).

## 2026-10-30 (3) — FINAL: mobile style root-cause chain + SW neutralized
- SERVICE WORKER: the LIVE site was running the OLD /sw.js from the MVC era (10/26) - registered under scope '/' BEFORE the port. New register('/service-worker.js') silently coexisted/failed; old worker intercepted navigations (empty pages!), mp4 (player errors) and its own update check (deadlock). FIX: sw.js + service-worker.js replaced by a passive KILL-SWITCH (no fetch handler, purges old caches, skipWaiting+claim) + middleware 404s /service-worker.js (browsers auto-unregister on 404 update). js registers /sw.js (kill-switch). If offline-PWA is ever rebuilt: bypass mp4/range/SW-script in the fetch handler FROM DAY ONE.
- CSS SERVING: MapStaticAssets serves BUILD-TIME content - server-side file edits invisible. Fix: app.UseStaticFiles() BEFORE app.MapStaticAssets() (disk wins) + Cache-Control no-cache on /css,/js + version query ?v=N bumped EVERY deploy (v11 current). Incremental publish went STALE once - after css/js edits delete obj+bin before publish.
- MOBILE FINAL STATE: .top_nav_left hidden <=1024 (bottom nav bar + header search cover it); all 2-col shells/heroes collapse <=900 (safety net v1+v2 in gp-tajrish-modern); header search+tools rows <=900; fonts IRANSansWeb*.woff2 copied to /fonts root.
- Deploy pipeline that WORKS: clean obj/bin publish -> zip {dll, endpoints.json, wwwroot\css incl .br/.gz, fonts, sw files} -> base64url chunks 100KB (verify each chunk len) -> server concat -> MD5 -> extract -> swap -> recycle pool. NEVER text-concat base64 chunks.

## 2026-10-30 (4) — PWA install card + bottom-nav clearance + responsive deps
- PWA card is JS-INJECTED (gp-tajrish-modern.js, pwaBuildCard) with INLINE styles — no razor/css deploy needed for it. Card id gpPwaHearingCard; responsive bottom via injected <style id=gpPwaCardStyle>: <=900px bottom:92px (clears gp-mobile-bottom-nav), >=901px bottom:24px.
- Frequency cap: gpPwaDismissedAt (re-ask after 5 days), gpPwaInstalled=1 never-again, gpPwaShownThisSession once-per-session. iOS: guide card (Safari share -> Add to Home Screen). Desktop without beforeinstallprompt: 4s fallback guide card (browser menu Install). BIP path: prompt() + appinstalled -> installed flag.
- SW v21 (all 3 files sw.js/service-worker.js/service-worker-v20.js identical): fetch handler ONLY for navigate-mode with pure passthrough fetch() - satisfies PWA installability, zero caching bugs. Videos/range/SW-script bypassed.
- SW history: old MVC-era /sw.js worker deadlocked its own update via caches.match on corrupted v17 cache -> caused empty pages + video errors. Solved by: v20/v21 content on BOTH /sw.js and /service-worker.js + js unregisters stale workers + (middleware 404 on /service-worker.js was temporary, removed when files unified).
- WinRM transfer gotcha: '+' in base64 corrupted over remoting -> ALWAYS base64url (+->-, /->_) + per-chunk length verify.
- 'محصولات سمعک' renamed to 'سمعک' everywhere (nav, titles, GeoLanding, Products).
- Homepage: new DB-driven services section gp-svcdb-* (ServiceCategories -> /Home/Services/{id}).
- Footer: gp-footer-quicklinks row (درباره ما/تماس با ما/FAQ/قوانین).
