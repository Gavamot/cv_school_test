using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Runtime.InteropServices;
using Path = System.IO.Path;
namespace task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // **************  Получение битовой карты изображения для вставки в UI ********
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        // ************** End Получение битовой карты изображения для вставки в UI ********

        /// <summary>
        /// Получить рисунок цветовым градиентом Hsv
        /// </summary>
        /// <returns></returns>
        public Image<Hsv, byte> GetHsvColors()
        {
            var res = new Image<Hsv, byte>(50, 180);
            for (int i = 0; i < res.Rows; i++)
            {
                for (int j = 0; j < res.Cols; j++)
                {
                    res[i, j] = new Hsv(i, 500, 500);
                }
            }
            return res;
        }

        /// <summary>
        /// Сколько каких пикселей на рисунке
        /// </summary>
        /// <param name="img"></param>
        /// <returns> Key - Hue(Hsv), Value - Count </returns>
        public Dictionary<double, int> GetHsvPixelRating(Image<Hsv, byte> img) 
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

        public void AddHsvPixelRatingItems(Image<Hsv, byte> img, ref PixelRatingCollection col)
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

        //private Dictionary<double, int> SumStat(List<Dictionary<double, int>> stats)
        //{
        //    var res = new Dictionary<double, int>();
        //    return res;
        //}

        private void WriteDataToFile(Dictionary<double, int> pixelRating, string path)
        {
            using(var strem = new FileStream(path, FileMode.OpenOrCreate))
            {
                var items = pixelRating.OrderBy(x => x.Value);
                foreach (var item in items)
                {
                    string str = $"{item.Key} = {item.Value}\r\n";
                    byte[] info = new UTF8Encoding(true).GetBytes(str);
                    strem.Write(info, 0, info.Length);
                }
            }
        }

        private void WriteDataToFile(PixelRatingCollection pixelRating, string path)
        {
            var items = pixelRating.OrderBy(x => x.Count).Reverse();
            using (var strem = new FileStream(path, FileMode.OpenOrCreate))
            {
                foreach (var item in items)
                {
                    //string str = $"{item.Hue} ({item.Value}) = {item.Count}\r\n";
                    string str = $"{item.Hue} = {item.Count}\r\n";
                    byte[] info = new UTF8Encoding(true).GetBytes(str);
                    strem.Write(info, 0, info.Length);
                }
            }
        }

        private void WriteDataToFileAll(string dir)
        {
            var files = Directory.GetFiles(dir, "*.png", SearchOption.TopDirectoryOnly);
            foreach(var f in files)
            {
                var img = new Image<Hsv, byte>(f);
                var rating = GetHsvPixelRating(img);
                var ratingFileName = new FileInfo(f).Name + "_stat.txt";
                WriteDataToFile(rating, ratingFileName);
            }
        }

        private void TotalStats(string dir)
        {
            var files = Directory.GetFiles(dir, "*.png", SearchOption.TopDirectoryOnly);
            var stats = new PixelRatingCollection();
            foreach (var f in files)
            {
                var img = new Image<Hsv, byte>(f);
                var rating = GetHsvPixelRating(img);
                AddHsvPixelRatingItems(img, ref stats);
            }

            var ratingFileName = new DirectoryInfo(dir).Name + "_stat.txt";
            WriteDataToFile(stats, ratingFileName);
        }
        public MainWindow()
        {
            InitializeComponent();

            string file = Path.Combine(Directory.GetCurrentDirectory(), "fire");
            string train = Path.Combine(file, "train");
            string neg = Path.Combine(train, "neg");
            string pos = Path.Combine(train, "pos");
            TotalStats(neg);
            TotalStats(pos);
            //var img = GetHsvColors();
            //imgGradient.Source = ToBitmapSource(img);
        }

        
    }
}
