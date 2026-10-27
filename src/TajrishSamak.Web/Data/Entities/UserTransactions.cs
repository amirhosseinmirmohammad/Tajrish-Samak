using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajrishSamak.Web.Data.Entities
{
    public class UserTransactions : System.Object
    {
        /// <summary>
        /// this class is using for detecting the user transactions in a while
        /// </summary>
        public UserTransactions() { }
        public long id { get; set; }
        public string userId { get; set; }
        public DateTime visitDateTime { get; set; }
    }
}
