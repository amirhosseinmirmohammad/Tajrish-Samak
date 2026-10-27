using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ViewModels.ContractorServiceViewModels
{
    public class ContractorServiceViewModels
    {
        public class ContractorComplaint
        {
            public int? OrderId { get; set; }
            public string UserId { get; set; }
            public int? ContractorComplaintId { get; set; }
            public string Description { get; set; }
        }
    }
}
