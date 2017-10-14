namespace tack
{
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
