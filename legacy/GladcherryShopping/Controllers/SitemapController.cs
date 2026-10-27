using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace GladcherryShopping.Controllers
{
    // Dynamic sitemap.xml (replaces the stale static file that drifted from the
    // real routes and caused hundreds of GSC "Submitted URL not found (404)"
    // reports). Generated from the exact same slug lists GeoController and
    // BrandController use, plus products / blog posts / services from the DB.
    [OutputCache(Duration = 3600, VaryByParam = "none")]
    public class SitemapController : Controller
    {
        private const string BaseUrl = "https://tajrish-samak.ir";

        private readonly GladcherryShopping.Models.ApplicationDbContext db =
            new GladcherryShopping.Models.ApplicationDbContext();

        public ActionResult Index()
        {
            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            Response.ContentType = "application/xml";

            var lastMod = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

            var settings = new XmlWriterSettings
            {
                Indent = true,
                // The declaration is written manually below: when XmlWriter runs
                // over a StringBuilder it always reports the in-memory encoding
                // (utf-16), while the HTTP response is served as UTF-8. A
                // mismatched declaration makes GSC reject the sitemap.
                OmitXmlDeclaration = true
            };

            // Write to a StringBuilder first so any DB hiccup returns a clean
            // 500 instead of a half-written XML document.
            var buffer = new StringBuilder();

            buffer.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            using (var writer = XmlWriter.Create(buffer, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                // 1) Core static pages
                Add(writer, "/", lastMod, "daily", "1.0");
                Add(writer, "/Product/All", lastMod, "daily", "0.9");
                Add(writer, "/Service/All", lastMod, "daily", "0.9");
                Add(writer, "/Blog/All", lastMod, "daily", "0.9");
                Add(writer, "/hearing-aid-brands", lastMod, "weekly", "0.8");
                Add(writer, "/aboutus", lastMod, "monthly", "0.6");
                Add(writer, "/contactus", lastMod, "monthly", "0.6");
                Add(writer, "/faq", lastMod, "monthly", "0.6");
                Add(writer, "/terms", lastMod, "yearly", "0.3");

                // 2) Brand landing pages (same slugs as BrandController)
                foreach (var brand in BrandController.Brands)
                {
                    Add(writer, "/hearing-aid-brands/" + brand.Slug, lastMod, "weekly", "0.8");
                }

                // 3) GEO landing pages - every area x service combination,
                //    generated from the exact lists the route constraint allows.
                foreach (var area in GeoController.LocationList)
                {
                    foreach (var service in GeoController.ServiceList)
                    {
                        Add(writer, "/" + area.Slug + "/" + service.Slug, lastMod, "weekly", "0.64");
                    }
                }

                // 4) Product detail pages (same visibility rule as the listing pages)
                var productIds = db.Products
                    .AsNoTracking()
                    .Where(p => p.SiteFirstImage != null)
                    .OrderByDescending(p => p.CreateDate)
                    .Select(p => p.Id)
                    .ToList();

                foreach (var id in productIds)
                {
                    Add(writer, "/Product/Details/" + id, lastMod, "weekly", "0.8");
                }

                // 5) Blog posts
                var blogPosts = db.Blogs
                    .AsNoTracking()
                    .Where(b => b.IsVisible == true && b.Images.Any())
                    .OrderByDescending(b => b.CreateDate)
                    .Select(b => new { b.Id, b.SefUrl })
                    .ToList();

                foreach (var post in blogPosts)
                {
                    Add(writer, "/Blog/" + post.Id + "/" + post.SefUrl, lastMod, "weekly", "0.8");
                }

                // 6) Services
                var services = db.Services
                    .AsNoTracking()
                    .Where(s => s.IsVisible == true && s.Images.Any())
                    .OrderByDescending(s => s.Survey)
                    .Select(s => new { s.Id, s.SefUrl })
                    .ToList();

                foreach (var service in services)
                {
                    Add(writer, "/Service/" + service.Id + "/" + service.SefUrl, lastMod, "weekly", "0.7");
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return Content(buffer.ToString(), "application/xml", Encoding.UTF8);
        }

        private static void Add(XmlWriter writer, string path, string lastMod, string changeFreq, string priority)
        {
            writer.WriteStartElement("url");
            writer.WriteElementString("loc", BaseUrl + path);
            writer.WriteElementString("lastmod", lastMod);
            writer.WriteElementString("changefreq", changeFreq);
            writer.WriteElementString("priority", priority);
            writer.WriteEndElement();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
