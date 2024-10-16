using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.ViewModels.PagerViewModel
{
    public class PagerViewModels<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItemCount { get; set; }

    }
}
