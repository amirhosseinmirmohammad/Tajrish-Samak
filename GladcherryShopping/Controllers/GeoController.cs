using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GladcherryShopping.Controllers
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

    internal class GeoLocation
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Cluster { get; set; }
        public string Hint { get; set; }
    }

    internal class GeoServiceIntent
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public string H1Template { get; set; }
        public string TitleTemplate { get; set; }
        public string MetaTemplate { get; set; }
        public string ShortTemplate { get; set; }
    }

    public class GeoController : Controller
    {
        // GEO full v81: all requested areas + expanded service intents including قیمت سمعک سال ۱۴۰۵
        private const string BaseUrl = "https://tajrish-samak.ir";

        private static readonly GeoLocation[] LocationList = new[]
        {
            new GeoLocation { Slug = "tehran", Name = "تهران", Cluster = "تهران", Hint = "صفحه عمومی برای جستجوهای شهر تهران و دسترسی به خدمات شنوایی." },
            new GeoLocation { Slug = "tajrish", Name = "تجریش", Cluster = "شمیرانات", Hint = "نزدیک میدان تجریش، بازار تجریش، امامزاده صالح و خیابان شهرداری." },
            new GeoLocation { Slug = "shemiran", Name = "شمیران", Cluster = "شمیرانات", Hint = "برای جستجوهای قدیمی و محلی محدوده شمیران." },
            new GeoLocation { Slug = "shemiranat", Name = "شمیرانات", Cluster = "شمیرانات", Hint = "برای کاربران شمال تهران و محدوده شمیرانات." },
            new GeoLocation { Slug = "mantaghe-1", Name = "منطقه ۱ تهران", Cluster = "شمال تهران", Hint = "برای کاربران منطقه یک تهران و محله‌های اطراف تجریش." },
            new GeoLocation { Slug = "north-tehran", Name = "شمال تهران", Cluster = "شمال تهران", Hint = "برای جستجوهای عمومی شمال تهران، از ونک و پارک‌وی تا تجریش و نیاوران." },
            new GeoLocation { Slug = "northeast-tehran", Name = "شمال شرق تهران", Cluster = "شمال شرق تهران", Hint = "برای محدوده‌های هروی، پاسداران، اقدسیه، نوبنیاد، ارتش و سوهانک." },
            new GeoLocation { Slug = "near-me", Name = "نزدیک من", Cluster = "جستجوی نزدیک من", Hint = "برای کاربرانی که عبارت نزدیک من را جستجو می‌کنند." },
            new GeoLocation { Slug = "mahale-ma", Name = "محله ما", Cluster = "جستجوی محلی", Hint = "برای جستجوهای محلی و عبارت محله ما." },
            new GeoLocation { Slug = "darabad", Name = "دارآباد", Cluster = "شمیرانات", Hint = "دسترسی مناسب از نیاوران، آجودانیه و اقدسیه." },
            new GeoLocation { Slug = "kashanak", Name = "کاشانک", Cluster = "شمیرانات", Hint = "نزدیک نیاوران، دارآباد و صاحبقرانیه." },
            new GeoLocation { Slug = "jamaran", Name = "جماران", Cluster = "شمیرانات", Hint = "نزدیک نیاوران، دزاشیب و تجریش." },
            new GeoLocation { Slug = "dezashib", Name = "دزاشیب", Cluster = "شمیرانات", Hint = "بین تجریش، جماران و نیاوران." },
            new GeoLocation { Slug = "yaser", Name = "یاسر", Cluster = "نیاوران", Hint = "در محور نیاوران و نزدیک فرمانیه و دزاشیب." },
            new GeoLocation { Slug = "farmanieh", Name = "فرمانیه", Cluster = "شمال تهران", Hint = "نزدیک نیاوران، کامرانیه و قیطریه." },
            new GeoLocation { Slug = "jamalabad", Name = "جمال‌آباد", Cluster = "شمیرانات", Hint = "در محدوده شمال شرق تهران و نزدیک مسیرهای دارآباد و سوهانک." },
            new GeoLocation { Slug = "azgol", Name = "ازگل", Cluster = "شمال شرق تهران", Hint = "نزدیک اراج، نوبنیاد، سوهانک و ارتش." },
            new GeoLocation { Slug = "sohanak", Name = "سوهانک", Cluster = "شمال شرق تهران", Hint = "دسترسی از ارتش، اقدسیه و مینی‌سیتی." },
            new GeoLocation { Slug = "artesh", Name = "ارتش", Cluster = "شمال شرق تهران", Hint = "برای محدوده بلوار ارتش، مینی‌سیتی، سوهانک و ازگل." },
            new GeoLocation { Slug = "aghdasiyeh", Name = "اقدسیه", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، آجودانیه، نیاوران و ارتش." },
            new GeoLocation { Slug = "heravi", Name = "هروی", Cluster = "شمال شرق تهران", Hint = "نزدیک پاسداران، موسوی، گلستان و سیدخندان." },
            new GeoLocation { Slug = "ghaem", Name = "قائم", Cluster = "شمال شرق تهران", Hint = "برای محدوده قائم، هروی و پاسداران." },
            new GeoLocation { Slug = "shahrak-naft", Name = "شهرک نفت", Cluster = "شمال تهران", Hint = "نزدیک مینی‌سیتی، ارتش و سوهانک." },
            new GeoLocation { Slug = "shariati", Name = "شریعتی", Cluster = "تهران", Hint = "برای مسیر خیابان شریعتی تا تجریش و قلهک." },
            new GeoLocation { Slug = "hekmat", Name = "حکمت", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، قیطریه و دزاشیب." },
            new GeoLocation { Slug = "gholhak", Name = "قلهک", Cluster = "شمال تهران", Hint = "دسترسی از شریعتی، دولت و قیطریه." },
            new GeoLocation { Slug = "dowlat", Name = "دولت", Cluster = "شمال تهران", Hint = "نزدیک قلهک، دیباجی و پاسداران." },
            new GeoLocation { Slug = "yakhchal", Name = "یخچال", Cluster = "شمال تهران", Hint = "در محور قلهک، شریعتی و دولت." },
            new GeoLocation { Slug = "valiasr", Name = "ولیعصر", Cluster = "تهران", Hint = "برای مسیر خیابان ولیعصر تا تجریش و پارک‌وی." },
            new GeoLocation { Slug = "asef", Name = "آصف", Cluster = "زعفرانیه", Hint = "نزدیک زعفرانیه، مقدس اردبیلی و ولنجک." },
            new GeoLocation { Slug = "pessian", Name = "پسیان", Cluster = "شمال تهران", Hint = "نزدیک زعفرانیه، ولیعصر و باغ فردوس." },
            new GeoLocation { Slug = "moghadas-ardabili", Name = "مقدس اردبیلی", Cluster = "زعفرانیه", Hint = "در محدوده زعفرانیه، آصف، ولنجک و الهیه." },
            new GeoLocation { Slug = "maghsoudbeik", Name = "مقصودبیک", Cluster = "تجریش", Hint = "نزدیک تجریش، دربند، باغ فردوس و دزاشیب." },
            new GeoLocation { Slug = "darband", Name = "دربند", Cluster = "شمیرانات", Hint = "نزدیک تجریش، امامزاده صالح و سعدآباد." },
            new GeoLocation { Slug = "elahiyeh", Name = "الهیه", Cluster = "شمال تهران", Hint = "نزدیک فرشته، تجریش، پارک‌وی و ولیعصر." },
            new GeoLocation { Slug = "fereshteh", Name = "فرشته", Cluster = "الهیه", Hint = "در محدوده الهیه، چناران، بوسنی و پل رومی." },
            new GeoLocation { Slug = "amanieh", Name = "امانیه", Cluster = "شمال تهران", Hint = "نزدیک الهیه، پارک‌وی و ولیعصر." },
            new GeoLocation { Slug = "chamran", Name = "چمران", Cluster = "شمال تهران", Hint = "برای دسترسی از بزرگراه چمران، پارک‌وی و ولنجک." },
            new GeoLocation { Slug = "parkway", Name = "پارک‌وی", Cluster = "شمال تهران", Hint = "نقطه اتصال ولیعصر، چمران، ولنجک و الهیه." },
            new GeoLocation { Slug = "velenjak", Name = "ولنجک", Cluster = "شمال تهران", Hint = "نزدیک بام تهران، زعفرانیه، مقدس اردبیلی و دانشگاه شهید بهشتی." },
            new GeoLocation { Slug = "vanak", Name = "ونک", Cluster = "شمال تهران", Hint = "برای دسترسی از ونک، ملاصدرا و ولیعصر به تجریش." },
            new GeoLocation { Slug = "seoul", Name = "سئول", Cluster = "شمال تهران", Hint = "نزدیک ولنجک، نمایشگاه، اوین و چمران." },
            new GeoLocation { Slug = "jafarabad", Name = "جعفرآباد", Cluster = "شمیرانات", Hint = "نزدیک تجریش، سعدآباد و دربند." },
            new GeoLocation { Slug = "sadabad", Name = "سعدآباد", Cluster = "شمیرانات", Hint = "نزدیک دربند، تجریش و زعفرانیه." },
            new GeoLocation { Slug = "chaharrah-hesabi", Name = "چهارراه حسابی", Cluster = "شمال تهران", Hint = "در مسیر ولنجک، زعفرانیه و مقدس اردبیلی." },
            new GeoLocation { Slug = "ajodanieh", Name = "آجودانیه", Cluster = "شمال شرق تهران", Hint = "نزدیک اقدسیه، دارآباد، نیاوران و ارتش." },
            new GeoLocation { Slug = "oshan", Name = "اوشان", Cluster = "لواسانات", Hint = "برای مسیر اوشان، فشم و شمال شرق تهران." },
            new GeoLocation { Slug = "mahak", Name = "محک", Cluster = "شمال شرق تهران", Hint = "نزدیک دارآباد، آجودانیه و نیاوران." },
            new GeoLocation { Slug = "mahalati", Name = "محلاتی", Cluster = "شمال شرق تهران", Hint = "نزدیک مینی‌سیتی، ارتش و اقدسیه." },
            new GeoLocation { Slug = "sadr", Name = "صدر", Cluster = "شمال تهران", Hint = "برای مسیر بزرگراه صدر، قیطریه، دیباجی و فرمانیه." },
            new GeoLocation { Slug = "bouali", Name = "بوعلی", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، قیطریه و کامرانیه." },
            new GeoLocation { Slug = "qanat-kosar", Name = "قنات کوثر", Cluster = "شمال شرق تهران", Hint = "نزدیک هروی، پاسداران و حکیمیه." },
            new GeoLocation { Slug = "emamzadeh-ghasem", Name = "امامزاده قاسم", Cluster = "شمیرانات", Hint = "نزدیک تجریش، دزاشیب و دربند." },
            new GeoLocation { Slug = "abk", Name = "آبک", Cluster = "شمیرانات", Hint = "در محدوده تجریش، دزاشیب و امامزاده قاسم." },
            new GeoLocation { Slug = "meydan-ghods", Name = "میدان قدس", Cluster = "تجریش", Hint = "نزدیک مترو تجریش، بازار تجریش و خیابان شهرداری." },
            new GeoLocation { Slug = "sahebqaranieh", Name = "صاحبقرانیه", Cluster = "نیاوران", Hint = "نزدیک نیاوران، کاشانک و جماران." },
            new GeoLocation { Slug = "falahi", Name = "فلاحی", Cluster = "زعفرانیه", Hint = "نزدیک زعفرانیه، ولنجک و مقدس اردبیلی." },
            new GeoLocation { Slug = "zaferanieh", Name = "زعفرانیه", Cluster = "شمال تهران", Hint = "نزدیک تجریش، ولنجک، آصف و مقدس اردبیلی." },
            new GeoLocation { Slug = "bagh-shater", Name = "باغ شاطر", Cluster = "شمیرانات", Hint = "نزدیک تجریش، سعدآباد و دربند." },
            new GeoLocation { Slug = "ghoba", Name = "قبا", Cluster = "شمال تهران", Hint = "نزدیک پاسداران، دولت و شریعتی." },
            new GeoLocation { Slug = "jolfa", Name = "جلفا", Cluster = "شمال تهران", Hint = "نزدیک شریعتی، قلهک و میرداماد." },
            new GeoLocation { Slug = "dibaji", Name = "دیباجی", Cluster = "شمال تهران", Hint = "نزدیک قیطریه، فرمانیه، پاسداران و دولت." },
            new GeoLocation { Slug = "hosseinabad", Name = "حسین‌آباد", Cluster = "شمال شرق تهران", Hint = "نزدیک هروی، پاسداران و مجیدیه شمالی." },
            new GeoLocation { Slug = "langari", Name = "لنگری", Cluster = "شمال شرق تهران", Hint = "نزدیک پاسداران، هروی و ضرابخانه." },
            new GeoLocation { Slug = "saghdoush", Name = "ساقدوش", Cluster = "شمال شرق تهران", Hint = "در محدوده پاسداران، هروی و دولت." },
            new GeoLocation { Slug = "nobonyad", Name = "نوبنیاد", Cluster = "شمال شرق تهران", Hint = "نزدیک پاسداران، اقدسیه، ارتش و مینی‌سیتی." },
            new GeoLocation { Slug = "saeedi", Name = "سعیدی", Cluster = "شمال تهران", Hint = "برای محدوده‌های محلی اطراف شمیران و تجریش." },
            new GeoLocation { Slug = "araj", Name = "اراج", Cluster = "شمال شرق تهران", Hint = "نزدیک نوبنیاد، ازگل، اقدسیه و ارتش." },
            new GeoLocation { Slug = "mahmoodieh", Name = "محمودیه", Cluster = "شمال تهران", Hint = "نزدیک ولنجک، پارک‌وی، زعفرانیه و ولیعصر." },
            new GeoLocation { Slug = "tandis", Name = "تندیس", Cluster = "تجریش", Hint = "نزدیک مرکز خرید تندیس، میدان تجریش و خیابان شهرداری." },
            new GeoLocation { Slug = "palladium", Name = "پالادیوم", Cluster = "زعفرانیه", Hint = "نزدیک مرکز خرید پالادیوم، مقدس اردبیلی و زعفرانیه." },
            new GeoLocation { Slug = "kamranieh", Name = "کامرانیه", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، قیطریه و نیاوران." },
            new GeoLocation { Slug = "andarzgoo", Name = "اندرزگو", Cluster = "شمال تهران", Hint = "نزدیک قیطریه، فرمانیه و کامرانیه." },
            new GeoLocation { Slug = "manzarieh", Name = "منظریه", Cluster = "شمال تهران", Hint = "در محدوده شمال تهران و مسیرهای نزدیک تجریش." },
            new GeoLocation { Slug = "nakhjavan", Name = "نخجوان", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، قیطریه و پاسداران." },
            new GeoLocation { Slug = "bookan", Name = "بوکان", Cluster = "شمال تهران", Hint = "نزدیک الهیه، فرشته و جردن شمالی." },
            new GeoLocation { Slug = "shahrak-omid", Name = "شهرک امید", Cluster = "شمال شرق تهران", Hint = "نزدیک مینی‌سیتی، ارتش، سوهانک و ازگل." },
            new GeoLocation { Slug = "mini-city", Name = "مینی‌سیتی", Cluster = "شمال شرق تهران", Hint = "نزدیک ارتش، اقدسیه، سوهانک و شهرک نفت." },
            new GeoLocation { Slug = "kolahdooz", Name = "کلاهدوز", Cluster = "شمال شرق تهران", Hint = "نزدیک پاسداران، دولت و اختیاریه." },
            new GeoLocation { Slug = "chizar", Name = "چیذر", Cluster = "شمیرانات", Hint = "نزدیک قیطریه، نیاوران و فرمانیه." },
            new GeoLocation { Slug = "qeytarieh", Name = "قیطریه", Cluster = "شمال تهران", Hint = "نزدیک شریعتی، فرمانیه، کامرانیه و چیذر." },
            new GeoLocation { Slug = "lavasani", Name = "لواسانی", Cluster = "شمال تهران", Hint = "نزدیک فرمانیه، نیاوران و پاسداران." },
            new GeoLocation { Slug = "moosivand", Name = "موسیوند", Cluster = "شمال تهران", Hint = "در محدوده‌های محلی شمال تهران و مسیرهای نزدیک تجریش." },
            new GeoLocation { Slug = "pol-roumi", Name = "پل رومی", Cluster = "الهیه", Hint = "نزدیک الهیه، فرشته، دزاشیب و تجریش." },
            new GeoLocation { Slug = "valiasr-sadr", Name = "ولیعصر صدر", Cluster = "شمال تهران", Hint = "برای محدوده اتصال ولیعصر، صدر، پارک‌وی و تجریش." },
            new GeoLocation { Slug = "takhti", Name = "تختی", Cluster = "شمال تهران", Hint = "برای محدوده‌های محلی شمیران و شمال تهران." },
            new GeoLocation { Slug = "zahir-dowleh", Name = "ظهیرالدوله", Cluster = "شمیرانات", Hint = "نزدیک تجریش، دزاشیب و امامزاده قاسم." },
            new GeoLocation { Slug = "pol-tajrish", Name = "پل تجریش", Cluster = "تجریش", Hint = "نزدیک میدان قدس، بازار تجریش و خیابان شهرداری." },
            new GeoLocation { Slug = "bagh-ferdos", Name = "باغ فردوس", Cluster = "تجریش", Hint = "نزدیک ولیعصر، تجریش، موزه سینما و زعفرانیه." },
            new GeoLocation { Slug = "emamzadeh-saleh", Name = "امامزاده صالح", Cluster = "تجریش", Hint = "نزدیک بازار تجریش، میدان تجریش و مترو تجریش." },
            new GeoLocation { Slug = "bazar-tajrish", Name = "بازار تجریش", Cluster = "تجریش", Hint = "نزدیک میدان تجریش، امامزاده صالح و خیابان شهرداری." },
            new GeoLocation { Slug = "zarabkhaneh", Name = "سه‌راه ضرابخانه", Cluster = "شمال شرق تهران", Hint = "نزدیک پاسداران، شریعتی، هروی و سیدخندان." },
            new GeoLocation { Slug = "yekta", Name = "یکتا", Cluster = "شمال تهران", Hint = "برای محدوده‌های محلی شمال تهران و شمیرانات." },
            new GeoLocation { Slug = "kashanchi", Name = "کاشانچی", Cluster = "شمال تهران", Hint = "برای مسیرهای محلی شمال تهران و تجریش." },
            new GeoLocation { Slug = "golsang", Name = "گل‌سنگ", Cluster = "شمال تهران", Hint = "در محدوده‌های محلی شمال تهران و شمیرانات." },
            new GeoLocation { Slug = "afshar", Name = "افشار", Cluster = "شمال تهران", Hint = "نزدیک زعفرانیه، ولنجک و محمودیه." },
            new GeoLocation { Slug = "mojdeh", Name = "مژده", Cluster = "شمال تهران", Hint = "برای محدوده‌های محلی شمال تهران و دسترسی به تجریش." },
            new GeoLocation { Slug = "moghaddasi", Name = "مقدسی", Cluster = "شمال تهران", Hint = "نزدیک خیابان‌های محلی شمال تهران و شمیران." },
            new GeoLocation { Slug = "lavasan", Name = "لواسان", Cluster = "لواسانات", Hint = "برای کاربران لواسان و مسیرهای شرق و شمال شرق تهران." },
            new GeoLocation { Slug = "roudehen", Name = "رودهن", Cluster = "شرق تهران", Hint = "برای کاربران رودهن و مسیرهای شرق تهران که به خدمات شنوایی نیاز دارند." },
            new GeoLocation { Slug = "boomehen", Name = "بومهن", Cluster = "شرق تهران", Hint = "برای کاربران بومهن و محدوده شرق تهران." },
            new GeoLocation { Slug = "feshm", Name = "فشم", Cluster = "لواسانات", Hint = "برای مسیر فشم، اوشان، رودبارقصران و شمال شرق تهران." },
            new GeoLocation { Slug = "niavaran", Name = "نیاوران", Cluster = "شمیرانات", Hint = "نزدیک دزاشیب، جماران، کاشانک، فرمانیه و تجریش." },
            new GeoLocation { Slug = "pasdaran", Name = "پاسداران", Cluster = "شمال شرق تهران", Hint = "نزدیک هروی، دولت، ضرابخانه، نوبنیاد و قیطریه." },
            new GeoLocation { Slug = "ekhtiyariyeh", Name = "اختیاریه", Cluster = "شمال تهران", Hint = "نزدیک پاسداران، دولت و دیباجی." },
            new GeoLocation { Slug = "darrous", Name = "دروس", Cluster = "شمال تهران", Hint = "نزدیک قلهک، پاسداران و شریعتی." }
        };

        private static readonly GeoServiceIntent[] ServiceList = new[]
        {
            new GeoServiceIntent {
                Slug = "hearing-test",
                Name = "تست شنوایی",
                H1Template = "تست شنوایی {area}",
                TitleTemplate = "تست شنوایی در {area} | کلینیک شنوایی شکوه تجریش",
                MetaTemplate = "تست شنوایی، ادیومتری و بررسی کم‌شنوایی برای محدوده {area} در کلینیک شنوایی و سمعک شکوه تجریش؛ مناسب برای وزوز، افت شنوایی، سالمندان و انتخاب سمعک.",
                ShortTemplate = "برای افرادی که در {area} زندگی یا کار می‌کنند و در شنیدن گفت‌وگو، صدای تلویزیون یا مکالمه در محیط شلوغ مشکل دارند، تست شنوایی اولین قدم دقیق برای تصمیم‌گیری است."
            },
            new GeoServiceIntent {
                Slug = "audiology-clinic",
                Name = "کلینیک شنوایی",
                H1Template = "کلینیک شنوایی در {area}",
                TitleTemplate = "کلینیک شنوایی در {area} | ارزیابی شنوایی و مشاوره سمعک",
                MetaTemplate = "کلینیک شنوایی نزدیک {area} برای ارزیابی شنوایی، بررسی کم‌شنوایی، مشاوره خرید سمعک، تنظیم و پیگیری شنوایی در شکوه تجریش.",
                ShortTemplate = "اگر به دنبال کلینیک شنوایی در {area} هستید، صفحه حاضر مسیر خدمات، مراحل مراجعه و گزینه‌های مرتبط با تست و سمعک را واضح توضیح می‌دهد."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid",
                Name = "خرید سمعک",
                H1Template = "خرید سمعک در {area}",
                TitleTemplate = "خرید سمعک در {area} | مشاوره انتخاب سمعک شکوه تجریش",
                MetaTemplate = "خرید و انتخاب سمعک نزدیک {area} با مشاوره تخصصی، بررسی نتیجه تست شنوایی، مقایسه برندها و راهنمایی درباره مدل مناسب، قیمت و گارانتی.",
                ShortTemplate = "برای خرید سمعک در {area} بهتر است قبل از تصمیم، نتیجه تست شنوایی، سبک زندگی، نیاز شنیداری و بودجه بررسی شود تا انتخاب فقط بر اساس نام برند یا قیمت نباشد."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-adjustment",
                Name = "تنظیم سمعک",
                H1Template = "تنظیم سمعک در {area}",
                TitleTemplate = "تنظیم سمعک در {area} | تنظیم تخصصی و پیگیری شنوایی",
                MetaTemplate = "تنظیم سمعک نزدیک {area} برای بهبود وضوح گفتار، کاهش آزار صوتی، بررسی فیدبک، تنظیم برنامه‌های شنیداری و پیگیری بعد از تجویز.",
                ShortTemplate = "اگر سمعک دارید اما صدا واضح نیست، در محیط شلوغ اذیت می‌شوید یا صدای سمعک طبیعی نیست، تنظیم مجدد سمعک می‌تواند کیفیت استفاده روزانه را بهتر کند."
            },
            new GeoServiceIntent {
                Slug = "home-visit-hearing-aid",
                Name = "ویزیت سمعک در منزل",
                H1Template = "ویزیت سمعک در منزل {area}",
                TitleTemplate = "ویزیت در منزل سمعک در {area} | مشاوره و پیگیری شنوایی",
                MetaTemplate = "ویزیت در منزل برای سمعک و شنوایی نزدیک {area}؛ مناسب سالمندان، افراد کم‌توان، بررسی اولیه، مشاوره و هماهنگی پیگیری تخصصی.",
                ShortTemplate = "برای سالمندان یا افرادی که رفت‌وآمد سخت دارند، هماهنگی ویزیت یا مشاوره اولیه در منزل می‌تواند شروع مسیر ارزیابی و انتخاب سمعک را ساده‌تر کند."
            },
            new GeoServiceIntent {
                Slug = "in-home-hearing-aid-prescription",
                Name = "تجویز سمعک در منزل",
                H1Template = "تجویز سمعک در منزل {area}",
                TitleTemplate = "تجویز سمعک در منزل {area} | راهنمای ارزیابی و انتخاب",
                MetaTemplate = "تجویز سمعک در منزل نزدیک {area} برای افرادی که نیاز به شروع مسیر انتخاب سمعک، بررسی شرایط و هماهنگی ارزیابی تخصصی دارند.",
                ShortTemplate = "تجویز دقیق سمعک به نتیجه تست شنوایی، بررسی سبک زندگی و امکان تنظیم وابسته است؛ در منزل می‌توان مسیر اولیه را ساده‌تر کرد."
            },
            new GeoServiceIntent {
                Slug = "in-home-hearing-aid-adjustment",
                Name = "تنظیم سمعک در منزل",
                H1Template = "تنظیم سمعک در منزل {area}",
                TitleTemplate = "تنظیم سمعک در منزل {area} | پیگیری و اصلاح صدا",
                MetaTemplate = "تنظیم سمعک در منزل نزدیک {area} برای بررسی وضوح گفتار، سوت کشیدن، آزار صوتی و هماهنگی پیگیری تخصصی در کلینیک.",
                ShortTemplate = "اگر کاربر سالمند است یا رفت‌وآمد دشوار دارد، تنظیم و پیگیری سمعک در منزل می‌تواند مشکلات اولیه استفاده از سمعک را کمتر کند."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-repair",
                Name = "تعمیر سمعک",
                H1Template = "تعمیر سمعک در {area}",
                TitleTemplate = "تعمیر سمعک در {area} | بررسی خرابی و سرویس سمعک",
                MetaTemplate = "تعمیر و بررسی سمعک نزدیک {area} برای مشکلاتی مثل قطع و وصل صدا، افت کیفیت، خرابی تیوب، هوک، فیلتر، میکروفن یا باتری.",
                ShortTemplate = "اگر سمعک روشن نمی‌شود، سوت می‌کشد، صدا ضعیف شده یا مصرف باتری غیرعادی است، بهتر است قبل از خرید جدید، وضعیت دستگاه بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-battery",
                Name = "باتری سمعک",
                H1Template = "باتری سمعک در {area}",
                TitleTemplate = "باتری سمعک در {area} | راهنمای خرید و مصرف باتری",
                MetaTemplate = "باتری سمعک نزدیک {area} همراه با راهنمای انتخاب سایز باتری، مصرف باتری، نگهداری، تعویض و بررسی علت خالی شدن سریع باتری.",
                ShortTemplate = "مصرف باتری سمعک به نوع دستگاه، میزان استفاده، رطوبت، تنظیمات و سلامت قطعات وابسته است؛ انتخاب باتری مناسب جلوی خاموشی‌های مکرر را می‌گیرد."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-filter",
                Name = "فیلتر سمعک",
                H1Template = "فیلتر سمعک در {area}",
                TitleTemplate = "فیلتر سمعک در {area} | تعویض فیلتر و نظافت سمعک",
                MetaTemplate = "فیلتر سمعک نزدیک {area} برای جلوگیری از گرفتگی صدا، کاهش خرابی و حفظ کیفیت شنیدن؛ همراه با راهنمای تعویض و نظافت.",
                ShortTemplate = "گرفتگی فیلتر می‌تواند باعث ضعیف شدن یا قطع شدن صدا شود و خیلی وقت‌ها قبل از تعمیر جدی، با بررسی فیلتر مشکل مشخص می‌شود."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-price",
                Name = "قیمت سمعک",
                H1Template = "قیمت سمعک در {area}",
                TitleTemplate = "قیمت سمعک در {area} | مشاوره هزینه، برند و مدل مناسب",
                MetaTemplate = "راهنمای قیمت سمعک نزدیک {area}؛ بررسی عوامل موثر بر هزینه سمعک، برند، نوع تکنولوژی، گارانتی، بیمه و نیاز واقعی فرد.",
                ShortTemplate = "قیمت سمعک فقط با نام برند مشخص نمی‌شود؛ نتیجه تست شنوایی، سطح تکنولوژی، نوع قالب، شارژی بودن، خدمات تنظیم و گارانتی هم مهم هستند."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-price-1405",
                Name = "قیمت سمعک سال ۱۴۰۵",
                H1Template = "قیمت سمعک سال ۱۴۰۵ در {area}",
                TitleTemplate = "قیمت سمعک سال ۱۴۰۵ در {area} | راهنمای هزینه و انتخاب",
                MetaTemplate = "راهنمای قیمت سمعک سال ۱۴۰۵ نزدیک {area}؛ توضیح عوامل اثرگذار بر هزینه، بیمه، گارانتی، برند و سطح تکنولوژی بدون اعلام قیمت قطعی غیرمستند.",
                ShortTemplate = "برای قیمت سمعک سال ۱۴۰۵ باید نوع کم‌شنوایی، تکنولوژی دستگاه، برند، گارانتی، خدمات تنظیم و پوشش بیمه کنار هم بررسی شوند."
            },
            new GeoServiceIntent {
                Slug = "installment-hearing-aid",
                Name = "سمعک قسطی",
                H1Template = "سمعک قسطی در {area}",
                TitleTemplate = "سمعک قسطی در {area} | شرایط پرداخت و مشاوره خرید",
                MetaTemplate = "راهنمای خرید سمعک قسطی نزدیک {area}، بررسی شرایط پرداخت، انتخاب مدل مناسب و هماهنگی مشاوره قبل از خرید.",
                ShortTemplate = "اگر پرداخت یکجای هزینه سخت است، بهتر است اول مدل مناسب از نظر شنوایی انتخاب شود و بعد درباره شرایط پرداخت و پوشش بیمه تصمیم گرفته شود."
            },
            new GeoServiceIntent {
                Slug = "insurance-hearing-aid",
                Name = "سمعک با بیمه",
                H1Template = "سمعک با بیمه در {area}",
                TitleTemplate = "سمعک با بیمه در {area} | راهنمای تعرفه، مدارک و مشاوره",
                MetaTemplate = "راهنمای سمعک با بیمه نزدیک {area}؛ بررسی مدارک، تعرفه، امکان استفاده از پوشش بیمه و مشاوره قبل از خرید سمعک.",
                ShortTemplate = "برای استفاده از بیمه در خرید سمعک، بهتر است قبل از خرید درباره مدارک، نوع پوشش و شرایط پرداخت اطلاعات دقیق بگیرید."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-insurance-tariff",
                Name = "تعرفه سمعک با بیمه",
                H1Template = "تعرفه سمعک با بیمه در {area}",
                TitleTemplate = "تعرفه سمعک با بیمه در {area} | راهنمای پوشش بیمه",
                MetaTemplate = "توضیح تعرفه سمعک با بیمه نزدیک {area}، مدارک لازم، مسیر استعلام و نکات مهم قبل از خرید سمعک.",
                ShortTemplate = "تعرفه و سهم بیمه ممکن است با نوع بیمه و شرایط پرونده فرق کند؛ بهتر است پیش از خرید، مدارک و پوشش دقیق بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "earmold",
                Name = "قالب سمعک",
                H1Template = "قالب سمعک در {area}",
                TitleTemplate = "قالب سمعک در {area} | قالب سیلیکونی، ضدآب و تعویض قالب",
                MetaTemplate = "قالب سمعک نزدیک {area} شامل قالب سیلیکونی، قالب ضدآب، تعویض قالب، بررسی نشتی صدا، راحتی گوش و فیت دقیق سمعک.",
                ShortTemplate = "قالب نامناسب می‌تواند باعث سوت کشیدن، درد، افت کیفیت صدا یا عدم ثبات سمعک شود؛ قالب‌گیری دقیق برای استفاده راحت ضروری است."
            },
            new GeoServiceIntent {
                Slug = "waterproof-earmold",
                Name = "قالب ضد آب سمعک",
                H1Template = "قالب ضد آب سمعک در {area}",
                TitleTemplate = "قالب ضد آب سمعک در {area} | مشاوره قالب مناسب",
                MetaTemplate = "قالب ضد آب سمعک نزدیک {area} برای شرایط خاص استفاده، محافظت بهتر و بررسی نیاز واقعی کاربر.",
                ShortTemplate = "قالب ضد آب برای همه کاربران ضروری نیست؛ نوع استفاده، رطوبت، سبک زندگی و وضعیت گوش باید بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "silicone-earmold",
                Name = "قالب سیلیکونی سمعک",
                H1Template = "قالب سیلیکونی سمعک در {area}",
                TitleTemplate = "قالب سیلیکونی سمعک در {area} | قالب‌گیری و فیت بهتر",
                MetaTemplate = "قالب سیلیکونی سمعک نزدیک {area} برای راحتی بیشتر، فیت بهتر، کاهش سوت و بهبود کیفیت استفاده روزانه.",
                ShortTemplate = "قالب سیلیکونی می‌تواند برای بعضی کاربران راحت‌تر باشد، اما انتخاب جنس قالب باید بر اساس شرایط گوش و نوع سمعک انجام شود."
            },
            new GeoServiceIntent {
                Slug = "earmold-replacement",
                Name = "تعویض قالب سمعک",
                H1Template = "تعویض قالب سمعک در {area}",
                TitleTemplate = "تعویض قالب سمعک در {area} | رفع نشتی و سوت سمعک",
                MetaTemplate = "تعویض قالب سمعک نزدیک {area} برای قالب فرسوده، تغییر فرم گوش، سوت کشیدن یا کاهش کیفیت صدا.",
                ShortTemplate = "با تغییر فرم گوش یا فرسوده شدن قالب، سمعک ممکن است سوت بکشد یا خوب در گوش ننشیند؛ در این حالت تعویض قالب کمک‌کننده است."
            },
            new GeoServiceIntent {
                Slug = "hearing-test-price",
                Name = "هزینه تست شنوایی",
                H1Template = "هزینه تست شنوایی در {area}",
                TitleTemplate = "هزینه تست شنوایی در {area} | راهنمای آزمایش شنوایی",
                MetaTemplate = "راهنمای هزینه تست شنوایی نزدیک {area}، نوع ارزیابی، مراحل انجام و زمان مناسب مراجعه.",
                ShortTemplate = "هزینه تست شنوایی به نوع ارزیابی و نیاز فرد بستگی دارد؛ برای تصمیم دقیق بهتر است ابتدا نوع مشکل شنیداری مشخص شود."
            },
            new GeoServiceIntent {
                Slug = "audiometry-price",
                Name = "آزمایش شنوایی چنده",
                H1Template = "آزمایش شنوایی چنده در {area}",
                TitleTemplate = "آزمایش شنوایی چنده در {area} | پاسخ سریع و مسیر مراجعه",
                MetaTemplate = "پاسخ به جستجوی آزمایش شنوایی چنده نزدیک {area} همراه با توضیح نوع تست، مراحل مراجعه و نقش نتیجه در انتخاب سمعک.",
                ShortTemplate = "عبارت آزمایش شنوایی چنده معمولاً یعنی کاربر می‌خواهد قبل از مراجعه بداند چه تستی لازم است و هزینه به چه عواملی وابسته است."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-cost",
                Name = "سمعک چنده",
                H1Template = "سمعک چنده در {area}",
                TitleTemplate = "سمعک چنده در {area} | راهنمای قیمت و انتخاب سمعک",
                MetaTemplate = "راهنمای پاسخ به سوال سمعک چنده نزدیک {area} با توضیح عوامل قیمت، برند، تکنولوژی، بیمه و خدمات پس از خرید.",
                ShortTemplate = "برای اینکه بدانیم سمعک چنده، اول باید بدانیم چه نوع کم‌شنوایی و چه سطح تکنولوژی لازم است؛ قیمت تنها معیار انتخاب نیست."
            },
            new GeoServiceIntent {
                Slug = "rechargeable-hearing-aid",
                Name = "سمعک شارژی",
                H1Template = "سمعک شارژی در {area}",
                TitleTemplate = "سمعک شارژی در {area} | مشاوره انتخاب سمعک بدون باتری",
                MetaTemplate = "مشاوره سمعک شارژی نزدیک {area} برای انتخاب سمعک بدون باتری، بررسی شارژدهی، دوام، گارانتی و سبک زندگی مناسب.",
                ShortTemplate = "سمعک شارژی برای بسیاری از کاربران راحت‌تر است، اما انتخاب آن باید با توجه به شدت کم‌شنوایی، زمان استفاده روزانه و نیاز شنیداری انجام شود."
            },
            new GeoServiceIntent {
                Slug = "battery-free-hearing-aid",
                Name = "سمعک بدون باتری",
                H1Template = "سمعک بدون باتری در {area}",
                TitleTemplate = "سمعک بدون باتری در {area} | راهنمای سمعک شارژی",
                MetaTemplate = "راهنمای سمعک بدون باتری نزدیک {area}، تفاوت مدل شارژی با باتری‌خور، دوام شارژ و نکات نگهداری.",
                ShortTemplate = "منظور بیشتر کاربران از سمعک بدون باتری، سمعک شارژی است؛ قبل از انتخاب باید شارژدهی و شرایط استفاده روزانه بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-battery-consumption",
                Name = "مصرف باتری سمعک",
                H1Template = "مصرف باتری سمعک در {area}",
                TitleTemplate = "مصرف باتری سمعک در {area} | علت خالی شدن سریع باتری",
                MetaTemplate = "بررسی مصرف باتری سمعک نزدیک {area}، علت خالی شدن سریع، نقش تنظیمات، رطوبت، نوع باتری و سلامت دستگاه.",
                ShortTemplate = "مصرف زیاد باتری همیشه به معنی خرابی دستگاه نیست؛ تنظیمات، نوع استفاده، قدرت خروجی و حتی رطوبت هم اثر دارند."
            },
            new GeoServiceIntent {
                Slug = "invisible-hearing-aid",
                Name = "سمعک نامرئی",
                H1Template = "سمعک نامرئی در {area}",
                TitleTemplate = "سمعک نامرئی در {area} | مشاوره انتخاب سمعک داخل گوشی",
                MetaTemplate = "مشاوره سمعک نامرئی نزدیک {area} برای بررسی امکان استفاده از سمعک داخل گوشی، شرایط کانال گوش، شدت کم‌شنوایی و نیاز روزانه.",
                ShortTemplate = "سمعک نامرئی برای همه افراد مناسب نیست؛ فرم گوش، میزان کم‌شنوایی، توانایی نگهداری و نوع نیاز شنیداری باید بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "tinnitus-hearing-aid",
                Name = "وزوز گوش و سمعک",
                H1Template = "وزوز گوش و سمعک در {area}",
                TitleTemplate = "وزوز گوش و سمعک در {area} | بررسی شنوایی و مشاوره تخصصی",
                MetaTemplate = "بررسی وزوز گوش نزدیک {area} همراه با تست شنوایی، مشاوره سمعک، بررسی کم‌شنوایی همراه و راهنمایی برای پیگیری تخصصی.",
                ShortTemplate = "وزوز گوش ممکن است با افت شنوایی همراه باشد؛ ارزیابی شنوایی کمک می‌کند مسیر پیگیری، درمان یا استفاده از سمعک دقیق‌تر انتخاب شود."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-warranty",
                Name = "گارانتی و ضمانت سمعک",
                H1Template = "گارانتی سمعک در {area}",
                TitleTemplate = "گارانتی و ضمانت سمعک در {area} | خدمات پس از خرید",
                MetaTemplate = "راهنمای گارانتی سمعک، ضمانت، خدمات پس از خرید و پیگیری مشکلات دستگاه نزدیک {area}.",
                ShortTemplate = "گارانتی فقط یک برگه نیست؛ دسترسی به تنظیم، سرویس، پیگیری و توضیح شرایط ضمانت در تجربه استفاده از سمعک بسیار مهم است."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-cleaning",
                Name = "نظافت سمعک",
                H1Template = "نظافت سمعک در {area}",
                TitleTemplate = "نظافت سمعک در {area} | مراقبت و افزایش عمر دستگاه",
                MetaTemplate = "آموزش و بررسی نظافت سمعک نزدیک {area} برای کاهش خرابی، گرفتگی فیلتر، رطوبت و افت کیفیت صدا.",
                ShortTemplate = "نظافت درست سمعک و قالب می‌تواند عمر دستگاه را بیشتر کند و جلوی گرفتگی صدا یا خرابی زودرس را بگیرد."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-durability",
                Name = "دوام سمعک",
                H1Template = "دوام سمعک در {area}",
                TitleTemplate = "دوام سمعک در {area} | نگهداری، گارانتی و مراقبت",
                MetaTemplate = "راهنمای دوام سمعک نزدیک {area}، عوامل اثرگذار بر عمر دستگاه، رطوبت، سرویس دوره‌ای، گارانتی و نگهداری.",
                ShortTemplate = "دوام سمعک به کیفیت دستگاه، شرایط نگهداری، رطوبت، سرویس و نحوه استفاده روزانه وابسته است."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-tube",
                Name = "شلنگ و تیوب سمعک",
                H1Template = "شلنگ و تیوب سمعک در {area}",
                TitleTemplate = "شلنگ و تیوب سمعک در {area} | تعویض و بررسی صدا",
                MetaTemplate = "تعویض شلنگ سمعک، تیوب سمعک و بررسی گرفتگی یا پارگی نزدیک {area}.",
                ShortTemplate = "شلنگ یا تیوب فرسوده می‌تواند باعث افت صدا، سوت کشیدن یا انتقال بد صدا شود و گاهی با تعویض ساده مشکل حل می‌شود."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-hook",
                Name = "هوک سمعک",
                H1Template = "هوک سمعک در {area}",
                TitleTemplate = "هوک سمعک در {area} | بررسی و تعویض قطعه",
                MetaTemplate = "بررسی و تعویض هوک سمعک نزدیک {area} برای مدل‌های پشت گوشی و رفع مشکلات اتصال یا انتقال صدا.",
                ShortTemplate = "هوک سمعک اگر آسیب ببیند یا درست فیت نشود، کیفیت شنیدن و راحتی استفاده از دستگاه کاهش پیدا می‌کند."
            },
            new GeoServiceIntent {
                Slug = "waterproof-hearing-aid",
                Name = "سمعک ضد آب",
                H1Template = "سمعک ضد آب در {area}",
                TitleTemplate = "سمعک ضد آب در {area} | مشاوره انتخاب و مراقبت",
                MetaTemplate = "مشاوره سمعک ضد آب نزدیک {area}، توضیح مقاومت در برابر رطوبت، محدودیت‌ها، نگهداری و انتخاب مناسب.",
                ShortTemplate = "ضد آب بودن سمعک درجات مختلف دارد و نباید با امکان استفاده دائمی زیر آب اشتباه گرفته شود؛ شرایط واقعی استفاده باید بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "sweatproof-hearing-aid",
                Name = "سمعک ضد عرق",
                H1Template = "سمعک ضد عرق در {area}",
                TitleTemplate = "سمعک ضد عرق در {area} | انتخاب مناسب برای رطوبت و فعالیت",
                MetaTemplate = "مشاوره سمعک ضد عرق نزدیک {area} برای کاربرانی که فعالیت زیاد، تعریق یا رطوبت محیطی دارند.",
                ShortTemplate = "اگر تعریق زیاد دارید، انتخاب مدل مناسب، خشک‌کن، نگهداری و سرویس دوره‌ای اهمیت زیادی دارد."
            },
            new GeoServiceIntent {
                Slug = "german-hearing-aid",
                Name = "سمعک آلمانی",
                H1Template = "سمعک آلمانی در {area}",
                TitleTemplate = "سمعک آلمانی در {area} | مشاوره برند و مدل",
                MetaTemplate = "مشاوره سمعک آلمانی نزدیک {area} و بررسی برند، تکنولوژی، نیاز شنیداری، قیمت و خدمات تنظیم.",
                ShortTemplate = "کشور سازنده فقط یکی از معیارهاست؛ مدل مناسب باید با نتیجه تست شنوایی و نیاز روزانه انتخاب شود."
            },
            new GeoServiceIntent {
                Slug = "american-hearing-aid",
                Name = "سمعک آمریکایی",
                H1Template = "سمعک آمریکایی در {area}",
                TitleTemplate = "سمعک آمریکایی در {area} | مشاوره انتخاب برند",
                MetaTemplate = "مشاوره سمعک آمریکایی نزدیک {area} برای مقایسه برند، مدل، خدمات، گارانتی و نیاز شنیداری.",
                ShortTemplate = "برای انتخاب سمعک آمریکایی باید قابلیت‌ها، تنظیم‌پذیری و تناسب با کم‌شنوایی فرد بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "danish-hearing-aid",
                Name = "سمعک دانمارکی",
                H1Template = "سمعک دانمارکی در {area}",
                TitleTemplate = "سمعک دانمارکی در {area} | مشاوره برندهای دانمارکی",
                MetaTemplate = "مشاوره سمعک دانمارکی نزدیک {area} و بررسی گزینه‌هایی مثل ویدکس و دیگر برندهای مرتبط بر اساس نیاز فرد.",
                ShortTemplate = "برندهای دانمارکی شناخته‌شده‌اند، اما انتخاب نهایی باید بر اساس شنوایی‌سنجی و تنظیم تخصصی انجام شود."
            },
            new GeoServiceIntent {
                Slug = "swiss-hearing-aid",
                Name = "سمعک سوئیسی",
                H1Template = "سمعک سوئیسی در {area}",
                TitleTemplate = "سمعک سوئیسی در {area} | مشاوره برند و تکنولوژی",
                MetaTemplate = "مشاوره سمعک سوئیسی نزدیک {area} و بررسی گزینه‌هایی مثل فوناک، سطح تکنولوژی، گارانتی و نیاز شنیداری.",
                ShortTemplate = "سمعک سوئیسی هم باید بر اساس نتیجه تست، نوع کم‌شنوایی و سبک زندگی انتخاب شود."
            },
            new GeoServiceIntent {
                Slug = "iranian-hearing-aid",
                Name = "سمعک ایرانی",
                H1Template = "سمعک ایرانی در {area}",
                TitleTemplate = "سمعک ایرانی در {area} | مشاوره گزینه‌های اقتصادی",
                MetaTemplate = "راهنمای سمعک ایرانی نزدیک {area} برای بررسی گزینه‌های اقتصادی، خدمات، گارانتی و تناسب با نیاز فرد.",
                ShortTemplate = "انتخاب سمعک ایرانی یا خارجی باید بر اساس نیاز شنیداری، بودجه، خدمات و امکان تنظیم انجام شود."
            },
            new GeoServiceIntent {
                Slug = "government-hearing-aid",
                Name = "سمعک دولتی",
                H1Template = "سمعک دولتی در {area}",
                TitleTemplate = "سمعک دولتی در {area} | راهنمای مسیر دریافت و مشاوره",
                MetaTemplate = "راهنمای سمعک دولتی نزدیک {area}، مسیرهای حمایتی، مدارک و نکات مهم قبل از انتخاب دستگاه.",
                ShortTemplate = "برای سمعک دولتی یا حمایتی بهتر است مدارک، شرایط پوشش و مدل‌های قابل ارائه بررسی شود."
            },
            new GeoServiceIntent {
                Slug = "free-behzisti-hearing-aid",
                Name = "سمعک رایگان بهزیستی",
                H1Template = "سمعک رایگان بهزیستی در {area}",
                TitleTemplate = "سمعک رایگان بهزیستی در {area} | راهنمای حمایت و مدارک",
                MetaTemplate = "راهنمای سمعک رایگان بهزیستی نزدیک {area}، شرایط حمایتی، مدارک احتمالی و مسیر پیگیری.",
                ShortTemplate = "دریافت سمعک حمایتی به شرایط پرونده و ضوابط سازمانی وابسته است؛ صفحه راهنما مسیر بررسی را روشن می‌کند."
            },
            new GeoServiceIntent {
                Slug = "deaf-support",
                Name = "حمایت ناشنوایان",
                H1Template = "حمایت ناشنوایان در {area}",
                TitleTemplate = "حمایت ناشنوایان در {area} | راهنمای خدمات شنوایی و سمعک",
                MetaTemplate = "راهنمای حمایت ناشنوایان و کم‌شنوایان نزدیک {area}، مسیر ارزیابی، مشاوره و گزینه‌های کمک‌شنوایی.",
                ShortTemplate = "حمایت از افراد کم‌شنوا فقط خرید سمعک نیست؛ ارزیابی، آموزش، پیگیری و تنظیم هم در کیفیت زندگی اثر دارد."
            },
            new GeoServiceIntent {
                Slug = "hearing-aid-consultation",
                Name = "مشاوره سمعک",
                H1Template = "مشاوره سمعک در {area}",
                TitleTemplate = "مشاوره سمعک در {area} | انتخاب مطمئن‌تر بر اساس تست شنوایی",
                MetaTemplate = "مشاوره سمعک نزدیک {area} برای انتخاب برند، مدل، قیمت، گارانتی، بیمه و بررسی نیاز واقعی فرد.",
                ShortTemplate = "مشاوره قبل از خرید کمک می‌کند بین برند، قیمت، ظاهر دستگاه و نیاز واقعی شنیداری انتخاب منطقی‌تری انجام شود."
            },
            new GeoServiceIntent {
                Slug = "quality-hearing-aid",
                Name = "سمعک عیار",
                H1Template = "سمعک عیار در {area}",
                TitleTemplate = "سمعک عیار در {area} | بررسی کیفیت و انتخاب سمعک",
                MetaTemplate = "راهنمای انتخاب سمعک با کیفیت نزدیک {area}، بررسی عیار واقعی دستگاه، تکنولوژی، تنظیم، گارانتی و خدمات.",
                ShortTemplate = "عیار سمعک فقط به برند نیست؛ کیفیت تنظیم، خدمات بعد از خرید و تناسب با کم‌شنوایی هم تعیین‌کننده است."
            },
            new GeoServiceIntent {
                Slug = "signia-hearing-aid-price",
                Name = "قیمت سمعک سیگنیا",
                H1Template = "قیمت سمعک سیگنیا در {area}",
                TitleTemplate = "قیمت سمعک سیگنیا در {area} | مشاوره مدل و هزینه",
                MetaTemplate = "راهنمای قیمت سمعک سیگنیا نزدیک {area}، بررسی مدل، سطح تکنولوژی، گارانتی، بیمه و تناسب با نیاز فرد.",
                ShortTemplate = "قیمت مدل‌های سیگنیا بسته به تکنولوژی، امکانات و خدمات تنظیم متفاوت است و باید با نتیجه تست شنوایی سنجیده شود."
            },
            new GeoServiceIntent {
                Slug = "siemens-hearing-aid-price",
                Name = "قیمت سمعک زیمنس",
                H1Template = "قیمت سمعک زیمنس در {area}",
                TitleTemplate = "قیمت سمعک زیمنس در {area} | راهنمای مدل‌های سیگنیا/زیمنس",
                MetaTemplate = "راهنمای قیمت سمعک زیمنس و سیگنیا نزدیک {area} با توضیح مدل‌ها، تکنولوژی و خدمات تنظیم.",
                ShortTemplate = "خیلی از کاربران هنوز عبارت زیمنس را جستجو می‌کنند؛ در مشاوره باید ارتباط مدل‌های جدید سیگنیا و نیاز شنیداری فرد توضیح داده شود."
            },
            new GeoServiceIntent {
                Slug = "widex-hearing-aid-price",
                Name = "قیمت سمعک ویدکس",
                H1Template = "قیمت سمعک ویدکس در {area}",
                TitleTemplate = "قیمت سمعک ویدکس در {area} | مشاوره مدل و هزینه",
                MetaTemplate = "راهنمای قیمت سمعک ویدکس نزدیک {area}، بررسی مدل، کیفیت صدا، گارانتی و تناسب با کم‌شنوایی.",
                ShortTemplate = "برای انتخاب ویدکس، کیفیت صدا، برنامه‌های شنیداری، خدمات تنظیم و نیاز واقعی فرد مهم‌تر از نگاه صرفاً قیمتی است."
            },
            new GeoServiceIntent {
                Slug = "phonak-hearing-aid-price",
                Name = "قیمت سمعک فوناک",
                H1Template = "قیمت سمعک فوناک در {area}",
                TitleTemplate = "قیمت سمعک فوناک در {area} | مشاوره برند و مدل",
                MetaTemplate = "راهنمای قیمت سمعک فوناک نزدیک {area}، بررسی مدل، سطح تکنولوژی، گارانتی و نیاز شنیداری.",
                ShortTemplate = "فوناک گزینه‌های متنوعی دارد و انتخاب درست باید بر اساس تست شنوایی، بودجه، سبک زندگی و خدمات تنظیم انجام شود."
            }
        };

        private static readonly Dictionary<string, GeoLocation> Locations =
            LocationList.ToDictionary(x => x.Slug, StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, GeoServiceIntent> Services =
            ServiceList.ToDictionary(x => x.Slug, StringComparer.OrdinalIgnoreCase);

        [HttpGet]
        public ActionResult ByAreaService(string areaSlug, string serviceSlug)
        {
            GeoLocation location;
            GeoServiceIntent service;

            if (!Locations.TryGetValue((areaSlug ?? "").Trim(), out location) ||
                !Services.TryGetValue((serviceSlug ?? "").Trim(), out service))
            {
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
                return HttpNotFound();
            }

            return Landing(BuildPage(location, service));
        }

        // Backward-compatible actions for existing RouteConfig mappings.
        [HttpGet] public ActionResult HearingTestTehran() { return ByAreaService("tehran", "hearing-test"); }
        [HttpGet] public ActionResult HearingTestTajrish() { return ByAreaService("tajrish", "hearing-test"); }
        [HttpGet] public ActionResult HearingClinicTehran() { return ByAreaService("tehran", "audiology-clinic"); }
        [HttpGet] public ActionResult HearingClinicTajrish() { return ByAreaService("tajrish", "audiology-clinic"); }
        [HttpGet] public ActionResult HearingAidTehran() { return ByAreaService("tehran", "hearing-aid"); }
        [HttpGet] public ActionResult HearingAidTajrish() { return ByAreaService("tajrish", "hearing-aid"); }
        [HttpGet] public ActionResult HearingAidAdjustmentTehran() { return ByAreaService("tehran", "hearing-aid-adjustment"); }
        [HttpGet] public ActionResult HearingAidAdjustmentTajrish() { return ByAreaService("tajrish", "hearing-aid-adjustment"); }
        [HttpGet] public ActionResult HearingTestNiavaran() { return ByAreaService("niavaran", "hearing-test"); }
        [HttpGet] public ActionResult HearingTestQeytarieh() { return ByAreaService("qeytarieh", "hearing-test"); }
        [HttpGet] public ActionResult HearingTestZaferanieh() { return ByAreaService("zaferanieh", "hearing-test"); }
        [HttpGet] public ActionResult HearingTestElahiyeh() { return ByAreaService("elahiyeh", "hearing-test"); }
        [HttpGet] public ActionResult HearingTestFarmanieh() { return ByAreaService("farmanieh", "hearing-test"); }

        private GeoLandingPage BuildPage(GeoLocation location, GeoServiceIntent service)
        {
            var area = location.Name;
            var serviceName = service.Name;
            var slug = location.Slug + "/" + service.Slug;

            var related = BuildRelatedLinks(location, service);

            return new GeoLandingPage
            {
                Slug = slug,
                Canonical = BaseUrl + "/" + slug,
                Title = Apply(service.TitleTemplate, area, serviceName),
                H1 = Apply(service.H1Template, area, serviceName),
                Kicker = serviceName + " برای " + area + " و محدوده " + location.Cluster,
                MainService = serviceName,
                City = "تهران",
                District = area,
                PrimaryIntent = "Local SEO landing / " + service.Slug,
                CtaText = "تماس و مشاوره",
                MetaDescription = Apply(service.MetaTemplate, area, serviceName),
                ShortAnswer = Apply(service.ShortTemplate, area, serviceName),
                SearchIntentText = "این صفحه برای جستجوهای محلی مثل «" + serviceName + " " + area + "»، «" + serviceName + " نزدیک من»، «" + serviceName + " شمال تهران» و ترکیب‌های مرتبط با سمعک، تست شنوایی، قیمت، بیمه، گارانتی و خدمات در منزل طراحی شده است؛ اما متن صفحه برای کاربر واقعی نوشته شده و فقط جایگزینی اسم محله نیست.",
                RouteNote = location.Hint,
                CompetitorAngle = "برخلاف صفحات فهرست‌محور که فقط نام چند مرکز یا شماره تماس را نمایش می‌دهند، این صفحه مسیر تصمیم‌گیری را هم توضیح می‌دهد: چه زمانی مراجعه لازم است، چه مواردی قبل از خرید یا تنظیم سمعک بررسی می‌شود و کدام خدمات بعدی ممکن است برای کاربر مهم باشد.",
                Keywords = BuildKeywords(location, service),
                Areas = BuildAreas(location),
                Benefits = BuildBenefits(area, serviceName),
                Steps = BuildSteps(area, serviceName),
                WhoNeeds = BuildWhoNeeds(area, serviceName),
                LocalProofs = new[] {
                    "آدرس کلینیک در محدوده میدان تجریش و خیابان شهرداری، برای بسیاری از محله‌های شمال تهران دسترسی محلی ایجاد می‌کند.",
                    "امکان پیگیری خدمات مرتبط مثل تست شنوایی، مشاوره انتخاب سمعک، تنظیم، باتری، قالب و تعمیرات در یک مسیر واحد وجود دارد.",
                    "برای تصمیم‌گیری بهتر، نتیجه ارزیابی شنوایی، نیاز روزمره، بودجه، برند، گارانتی و امکان استفاده از بیمه باید هم‌زمان بررسی شود."
                },
                RelatedTitle = "صفحات مرتبط با " + serviceName + " در شمال تهران",
                RelatedDescription = "برای جلوگیری از محتوای تکراری، هر صفحه فقط روی یک نیت اصلی تمرکز دارد و لینک‌های مرتبط، مسیر بعدی کاربر را مشخص می‌کنند.",
                RelatedLinks = related,
                Faqs = BuildFaqs(location, service)
            };
        }

        private static string Apply(string template, string area, string service)
        {
            return (template ?? "").Replace("{area}", area).Replace("{service}", service);
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
            return core.Where(x => !String.IsNullOrWhiteSpace(x)).Distinct().ToArray();
        }

        private static string[] BuildBenefits(string area, string service)
        {
            return new[]
            {
                "دسترسی مناسب برای ساکنان و شاغلان " + area + " و محله‌های اطراف.",
                "توضیح ساده و قابل فهم درباره " + service + "، هزینه‌ها، مراحل و گزینه‌های بعدی.",
                "امکان پیگیری تست شنوایی، انتخاب سمعک، تنظیم، قالب، باتری یا تعمیرات در یک مسیر واحد.",
                "تمرکز روی نیاز واقعی فرد، نه فقط فروش دستگاه یا معرفی یک برند خاص."
            };
        }

        private static string[] BuildSteps(string area, string service)
        {
            return new[]
            {
                "تماس با کلینیک و توضیح کوتاه نیاز یا مشکل شنوایی در محدوده " + area + ".",
                "هماهنگی زمان مراجعه یا مشاوره اولیه بر اساس شرایط فرد.",
                "بررسی علائم، سابقه استفاده از سمعک، بودجه، بیمه و سبک زندگی.",
                "ارائه مسیر بعدی؛ از تست شنوایی تا انتخاب، تنظیم، تعمیر یا پیگیری سمعک."
            };
        }

        private static string[] BuildWhoNeeds(string area, string service)
        {
            return new[]
            {
                "افرادی در " + area + " که در مکالمه روزمره یا محیط شلوغ شنیدن برایشان سخت شده است.",
                "سالمندان یا خانواده‌هایی که متوجه افت شنوایی، وزوز یا نیاز به تکرار جملات شده‌اند.",
                "کاربرانی که سمعک دارند اما از وضوح صدا، سوت کشیدن، مصرف باتری یا قالب ناراضی‌اند.",
                "افرادی که قبل از خرید سمعک به مشاوره قیمت، برند، بیمه، گارانتی یا مدل مناسب نیاز دارند."
            };
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
                .Where(x => !String.Equals(x.Url, "/" + area + "/" + service.Slug, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        private static GeoFaqItem[] BuildFaqs(GeoLocation location, GeoServiceIntent service)
        {
            var area = location.Name;
            var s = service.Name;

            return new[]
            {
                new GeoFaqItem {
                    Question = "آیا " + s + " برای محدوده " + area + " انجام می‌شود؟",
                    Answer = "بله. کلینیک شکوه تجریش برای مراجعان " + area + " و محله‌های اطراف شمال تهران خدمات شنوایی، مشاوره و پیگیری سمعک ارائه می‌دهد."
                },
                new GeoFaqItem {
                    Question = "برای " + s + " از کجا شروع کنم؟",
                    Answer = "بهتر است ابتدا تماس بگیرید و مشکل یا نیاز اصلی را توضیح دهید تا زمان مراجعه، نوع ارزیابی و مرحله مناسب بعدی مشخص شود."
                },
                new GeoFaqItem {
                    Question = "آیا قبل از خرید یا تنظیم سمعک باید تست شنوایی انجام شود؟",
                    Answer = "در بیشتر موارد بله. نتیجه تست شنوایی کمک می‌کند انتخاب یا تنظیم سمعک بر اساس نیاز واقعی گوش انجام شود، نه صرفاً بر اساس مدل یا قیمت."
                },
                new GeoFaqItem {
                    Question = "آیا درباره قیمت، بیمه و گارانتی سمعک هم مشاوره داده می‌شود؟",
                    Answer = "بله. در زمان مشاوره می‌توانید درباره قیمت سمعک، شرایط گارانتی، امکان استفاده از بیمه، نوع برند، باتری، قالب و خدمات بعد از خرید سؤال کنید."
                }
            };
        }

        private ActionResult Landing(GeoLandingPage page)
        {
            ViewBag.Title = page.Title;
            ViewBag.Page = page;
            return View("GeoLanding");
        }
    }
}
