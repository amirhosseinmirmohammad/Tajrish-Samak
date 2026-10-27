using DataLayer.Models;
using System.Collections.Generic;

namespace DataLayer.ViewModels
{
    public class SubmitOrderViewModel
    {
        public IEnumerable<BasketViewModel> basket { get; set; }
        public IEnumerable<DiscountViewModel> discount { get; set; }
        public FinalViewModel order { get; set; }
        public double FinalPrice { get; set; }
    }
}
