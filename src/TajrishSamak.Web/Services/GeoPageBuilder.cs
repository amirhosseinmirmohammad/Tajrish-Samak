using System;
using System.Collections.Generic;
using System.Linq;

namespace TajrishSamak.Web.Services
{
    public class GeoLandingPage
    {
        public string Slug { get; set; }
        public string Canonical { get; set; }
        public string Title { get; set; }
        public string H1 { get; set; }
        public string Kicker { get; set; }
        public string MetaDescription { get; set; }
        public string ShortAnswer { get; set; }
        public string PrimaryIntent { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string MainService { get; set; }
        public string CtaText { get; set; }
        public string RouteNote { get; set; }
        public string CompetitorAngle { get; set; }
        public string SearchIntentText { get; set; }
        public string[] Keywords { get; set; }
        public string[] Areas { get; set; }
        public string[] Benefits { get; set; }
        public string[] Steps { get; set; }
        public string[] WhoNeeds { get; set; }
        public string[] LocalProofs { get; set; }
        public GeoRelatedLink[] RelatedLinks { get; set; }
        public GeoFaqItem[] Faqs { get; set; }
        public string RelatedTitle { get; set; }
        public string RelatedDescription { get; set; }
    }

    public class GeoRelatedLink
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }

    public class GeoFaqItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    /// <summary>
    /// Faithful port of the legacy GeoController.BuildPage content generator.
    /// Same variants, same seed selection, same wording.
    /// </summary>
    public static class GeoPageBuilder
    {
        private const string BaseUrl = "https://tajrish-samak.ir";

        public static GeoLandingPage Build(GeoLocation location, GeoServiceIntent service)
        {
            var area = location.Name;
            var serviceName = service.Name;
            var slug = location.Slug + "/" + service.Slug;

            var seed = (location.Slug + service.Slug).GetHashCode();

            var introVariants = new[]
            {
                $"{serviceName} در {area} یکی از پرجستجوترین خدمات شنوایی برای ساکنان این محدوده است.",
                $"اگر در {area} زندگی می‌کنید و به خدمات شنوایی نیاز دارید، {serviceName} می‌تواند اولین قدم باشد.",
                $"{serviceName} در محدوده {area} معمولاً برای بررسی، تشخیص و انتخاب سمعک استفاده می‌شود.",
                $"بسیاری از کاربران در {area} برای مشکلات شنوایی ابتدا سراغ {serviceName} می‌روند."
            };

            var selectedIntro = introVariants[Math.Abs(seed) % introVariants.Length];

            var faqPool = new[]
            {
                "چه زمانی باید برای خدمات شنوایی مراجعه کنیم؟",
                "آیا تست شنوایی قبل از سمعک ضروری است؟",
                "هزینه خدمات شنوایی در این محدوده چطور تعیین می‌شود؟",
                "آیا امکان تنظیم مجدد سمعک وجود دارد؟"
            };

            var selectedFaq = faqPool[Math.Abs(seed + 3) % faqPool.Length];

            var related = BuildRelatedLinks(location, service);

            return new GeoLandingPage
            {
                Slug = slug,
                Canonical = BaseUrl + "/" + slug,

                Title = $"{serviceName} در {area} | کلینیک شنوایی شکوه تجریش",
                H1 = $"{serviceName} در {area}",

                Kicker = $"{serviceName} تخصصی برای ساکنان {area} و اطراف آن",

                City = "تهران",
                District = area,
                MainService = serviceName,

                PrimaryIntent = "Local SEO / " + service.Slug,

                CtaText = "مشاوره و تماس فوری",

                MetaDescription =
                    $"خدمات {serviceName} در {area} شامل بررسی شنوایی، مشاوره سمعک، تنظیم و پیگیری تخصصی در کلینیک شنوایی شکوه تجریش.",

                ShortAnswer =
                    selectedIntro + " این صفحه برای آشنایی بهتر با روند مراجعه، خدمات و تصمیم‌گیری آگاهانه طراحی شده است.",

                SearchIntentText =
                    $"کاربرانی که عبارت «{serviceName} {area}» یا «{serviceName} نزدیک من» را جستجو می‌کنند معمولاً به دنبال تشخیص، قیمت، تنظیم یا انتخاب سمعک هستند.",

                RouteNote = location.Hint,

                CompetitorAngle =
                    "برخلاف صفحات تکراری که فقط اطلاعات سطحی ارائه می‌دهند، این صفحه مسیر تصمیم‌گیری واقعی از مشکل تا درمان یا انتخاب سمعک را توضیح می‌دهد.",

                Keywords = BuildKeywords(location, service),
                Areas = BuildAreas(location),

                Benefits = new[]
                {
                    $"دسترسی سریع برای ساکنان {area} و محله‌های اطراف",
                    $"ارائه مشاوره تخصصی درباره {serviceName}",
                    "بررسی دقیق وضعیت شنوایی قبل از هر تصمیم",
                    "پشتیبانی بعد از انتخاب یا تنظیم سمعک"
                },

                Steps = new[]
                {
                    "تماس اولیه و توضیح مشکل یا نیاز",
                    "بررسی شرایط و تعیین نوع ارزیابی",
                    "انجام تست یا مشاوره تخصصی",
                    "ارائه مسیر درمان یا انتخاب سمعک"
                },

                WhoNeeds = new[]
                {
                    $"افرادی در {area} که در شنیدن گفت‌وگو مشکل دارند",
                    "سالمندان با افت شنوایی یا وزوز گوش",
                    "کاربران دارای سمعک با مشکل تنظیم یا کیفیت صدا",
                    "افرادی که قصد خرید سمعک دارند"
                },

                LocalProofs = new[]
                {
                    $"دسترسی از {area} به کلینیک شنوایی در شمال تهران",
                    "امکان انجام تست، مشاوره و تنظیم در یک مسیر واحد",
                    "پشتیبانی تخصصی برای انتخاب سمعک مناسب"
                },

                RelatedTitle = $"خدمات مرتبط در {area}",
                RelatedDescription = "لینک‌های مرتبط برای ادامه مسیر کاربر و جلوگیری از تکرار محتوا",

                RelatedLinks = related,

                Faqs = new[]
                {
                    new GeoFaqItem
                    {
                        Question = selectedFaq,
                        Answer = $"در {area} این خدمات معمولاً شامل بررسی شنوایی، مشاوره و در صورت نیاز انتخاب یا تنظیم سمعک است."
                    },
                    new GeoFaqItem
                    {
                        Question = $"آیا {serviceName} برای همه افراد در {area} لازم است؟",
                        Answer = "خیر، اما برای افرادی که علائم افت شنوایی یا وزوز دارند بسیار توصیه می‌شود."
                    },
                    new GeoFaqItem
                    {
                        Question = "اولین قدم چیست؟",
                        Answer = "معمولاً تماس اولیه و بررسی شرایط فرد بهترین شروع مسیر است."
                    }
                }
            };
        }

        private static string[] BuildKeywords(GeoLocation location, GeoServiceIntent service)
        {
            var a = location.Name;
            var s = service.Name;
            return new[]
            {
                s + " " + a,
                s + " در " + a,
                s + " نزدیک " + a,
                s + " شمال تهران",
                s + " تجریش",
                "کلینیک شنوایی " + a,
                "سمعک " + a,
                "مشاوره سمعک " + a,
                "قیمت سمعک " + a,
                "تنظیم سمعک " + a,
                "تعمیر سمعک " + a,
                "باتری سمعک " + a
            }.Distinct().ToArray();
        }

        private static string[] BuildAreas(GeoLocation location)
        {
            var core = new[] { location.Name, location.Cluster, "تجریش", "شمیرانات", "شمال تهران", "منطقه ۱ تهران" };
            return core.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
        }

        private static GeoRelatedLink[] BuildRelatedLinks(GeoLocation location, GeoServiceIntent service)
        {
            var area = location.Slug;
            var list = new List<GeoRelatedLink>
            {
                new GeoRelatedLink { Text = "تست شنوایی در " + location.Name, Url = "/" + area + "/hearing-test" },
                new GeoRelatedLink { Text = "خرید سمعک در " + location.Name, Url = "/" + area + "/hearing-aid" },
                new GeoRelatedLink { Text = "تنظیم سمعک در " + location.Name, Url = "/" + area + "/hearing-aid-adjustment" },
                new GeoRelatedLink { Text = "قیمت سمعک در " + location.Name, Url = "/" + area + "/hearing-aid-price" },
                new GeoRelatedLink { Text = "باتری سمعک در " + location.Name, Url = "/" + area + "/hearing-aid-battery" },
                new GeoRelatedLink { Text = "تعمیر سمعک در " + location.Name, Url = "/" + area + "/hearing-aid-repair" },
                new GeoRelatedLink { Text = "برندهای سمعک", Url = "/hearing-aid-brands" },
                new GeoRelatedLink { Text = "تماس با کلینیک", Url = "/contactus" }
            };

            return list
                .Where(x => !string.Equals(x.Url, "/" + area + "/" + service.Slug, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
    }
}
