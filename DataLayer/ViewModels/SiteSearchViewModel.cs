using System.Collections.Generic;

namespace DataLayer.ViewModels
{
    public class SiteSearchViewModel
    {
        public string Query { get; set; }
        public string ActiveType { get; set; }
        public int TotalCount { get; set; }
        public int ProductCount { get; set; }
        public int BlogCount { get; set; }
        public int ServiceCount { get; set; }
        public int GeoCount { get; set; }
        public int BrandCount { get; set; }
        public int PageCount { get; set; }
        public List<SiteSearchResult> Results { get; set; }
    }

    public class SiteSearchResult
    {
        public string TypeKey { get; set; }
        public string TypeName { get; set; }
        public int TypeOrder { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public int Score { get; set; }
    }
}
