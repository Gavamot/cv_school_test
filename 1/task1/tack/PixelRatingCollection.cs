using System.Collections.Generic;
using System.Linq;

namespace tack
{
    public class PixelRatingCollection : List<PixelRatingItem>
    {
        protected double ValueDelta { get; set; } = 10;

        protected double RaundValue(double val)
        {
            return ((int)(val / ValueDelta)) * ValueDelta;
        }

        public new void Add(PixelRatingItem item)
        {
            //item.Value = RaundValue(item.Value); // С учетом яркости
            var el = this.FirstOrDefault(x => x.Hue == item.Hue);
                //x => x.Hue == item.Hue && item.Value == x.Value); // С учетом яркости
            if (el == null)
                base.Add(item);
            else
                el.Count++;
        }

        public int GetPixelCount ()=> this.Sum(x => x.Count);
    }
}