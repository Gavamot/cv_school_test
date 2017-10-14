using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace tack
{
    class PixelRatingAnalizator
    {
        public PixelRatingAnalizator(string rootDir, string filesFileter, SearchOption directorySearchOption)
        {
            RootDir = rootDir;
            FilesFileter = filesFileter;
            DirectorySearchOption = directorySearchOption;
        }

        protected string RootDir { get; set; }

        public string FilesFileter { get; set; }
        public SearchOption DirectorySearchOption { get; set; }

        protected PixelRatingCollection PixelNeg { get; set; }
        protected PixelRatingCollection PixelPos { get; set; }

        protected string GetNegPath => Path.Combine(RootDir, "neg");
        protected string GetPosPath => Path.Combine(RootDir, "pos");
        protected int GetPosImgCount => Directory.GetFiles(GetPosPath, FilesFileter, DirectorySearchOption).Count();
        protected double minPointCof = 0.04;

        public List<HueSignificance> GetHueSignificance()
        {
            var res = new List<HueSignificance>();
            PixelPos = TotalStats(GetPosPath);
            PixelNeg = TotalStats(GetNegPath);
            int posImgCount = GetPosImgCount;
            int pc = PixelPos.GetPixelCount() / posImgCount;
            int minCountPixels = (int)((pc / posImgCount) * minPointCof);
            foreach (var p in PixelPos)
            {
                if (p.Count < minCountPixels)
                    continue;
                var neg = PixelNeg.FirstOrDefault(x => x.Hue == p.Hue);
                int negCount = neg == null ? 0 : neg.Count;
                double significance = (p.Count - negCount) / ((double)pc);
                if(significance > 0) 
                    res.Add(new HueSignificance { Hue = p.Hue, Significance = significance});
            }
            return res;
        }

        protected PixelRatingCollection TotalStats(string dir)
        {
            var files = Directory.GetFiles(dir, FilesFileter, DirectorySearchOption);
            var res = new PixelRatingCollection();
            foreach (string f in files)
            {
                var img = new Image<Hsv, byte>(f);
                var rating = GetHsvPixelRating(img);
                AddHsvPixelRatingItems(img, ref res);
            }
            return res;
        }

        /// <summary>
        /// Сколько каких пикселей на рисунке
        /// </summary>
        /// <param name="img"></param>
        /// <returns> Key - Hue(Hsv), Value - Count </returns>
        protected Dictionary<double, int> GetHsvPixelRating(Image<Hsv, byte> img)
        {
            var res = new Dictionary<double, int>();
            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    var p = img[i, j];
                    if (res.ContainsKey(p.Hue))
                        res[p.Hue]++;
                    else
                        res[p.Hue] = 1;
                }
            }
            return res;
        }

        protected void AddHsvPixelRatingItems(Image<Hsv, byte> img, ref PixelRatingCollection col)
        {
            var res = new List<PixelRatingItem>();
            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    var p = new PixelRatingItem();
                    p.Hue = img[i, j].Hue;
                    p.Value = img[i, j].Value;
                    col.Add(p);
                }
            }
        }
    }
}
