using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1
{
    public class PixelRatingCollection : List<PixelRatingItem>
    {
        public PixelRatingCollection()
        {
            
        }

        double ValueDelta { get; set; } = 10;


        private double RaundValue(double val)
        {
            return ((int)(val / 10)) * 10;
        }

        public new void Add(PixelRatingItem item)
        {
            item.Value = RaundValue(item.Value);
            var el = this.FirstOrDefault(x => x.Hue == item.Hue);
                //x => x.Hue == item.Hue && item.Value == x.Value);
            if (el == null)
                base.Add(item);
            else
                el.Count++;
        }

    }

    public class PixelRatingItem
    {
        /// <summary>
        /// 0 <= Hue <= 180  
        /// </summary>
        public double Hue { get; set; }
        public double Value { get; set; }
        public int Count { get; set; } = 1;
    }
}
