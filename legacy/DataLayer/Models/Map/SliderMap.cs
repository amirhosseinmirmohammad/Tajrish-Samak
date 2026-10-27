using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataLayer.Models.Mapping
{
    public class SliderMap : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Slider>
    {
        public SliderMap()
        {

            // Primary Key
            this.HasKey(t => t.Id);
        }
    }
}