using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ViewModels.UserServiceViewModel
{
    public class NearOrderViewModels : object
    {
        public double d { get; set; }
        public double distance { get; set; }
        public Order order { get; set; }
    }

    //public class NearAdverViewModels : object
    //{
    //    public double d { get; set; }
    //    public double distance { get; set; }
    //    public Advertisement advertisement { get; set; }
    //}
}
