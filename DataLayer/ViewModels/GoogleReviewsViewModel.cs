using System.Collections.Generic;

namespace DataLayer.ViewModels
{
    public class GoogleReviewsViewModel
    {
        public GoogleReviewsViewModel()
        {
            Reviews = new List<GoogleReviewItemViewModel>();
        }

        public string PlaceId { get; set; }
        public string BusinessName { get; set; }
        public double Rating { get; set; }
        public int UserRatingCount { get; set; }
        public string GoogleMapsUrl { get; set; }
        public string AttributionText { get; set; }
        public string ErrorMessage { get; set; }
        public List<GoogleReviewItemViewModel> Reviews { get; set; }
    }

    public class GoogleReviewItemViewModel
    {
        public string AuthorName { get; set; }
        public string AuthorUri { get; set; }
        public string AuthorPhotoUri { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public string RelativePublishTimeDescription { get; set; }
        public string PublishTime { get; set; }
        public string ReviewUri { get; set; }
    }
}
