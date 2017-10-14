using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace tack
{
    class Program
    {

        static void Main(string[] args)
        {
            string root = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "fire");
            if (args.Length > 0)
            {
                root = args[0];
            }

            string trainPath = Path.Combine(root, "train");
            string filesFileter = "*.png";
            SearchOption directorySearchOption = SearchOption.AllDirectories;
            var analizator = new PixelRatingAnalizator(trainPath, filesFileter, directorySearchOption);
            List<HueSignificance> rating = analizator.GetHueSignificance();
            string validationPath = Path.Combine(root, "validation");


            //double res = 0, cur = 0, v = 0;
            //for (double i = 0; i < 200; i+= 0.1)
            //{
            //    cur = AnalizeFilesViewWraper(i, "train - pos", Path.Combine(trainPath, "pos"), filesFileter, directorySearchOption, rating, true);
            //    cur += AnalizeFilesViewWraper(i, "train - neg", Path.Combine(trainPath, "neg"), filesFileter, directorySearchOption, rating, false);
            //    cur += AnalizeFilesViewWraper(i, "validation - pos", Path.Combine(validationPath, "pos"), filesFileter, directorySearchOption, rating, true);
            //    cur += AnalizeFilesViewWraper(i, "validation - neg", Path.Combine(validationPath, "neg"), filesFileter, directorySearchOption, rating, false);
            //    if(res < cur)
            //    {
            //        res = cur;
            //        v = i;
            //    }
            //}
            //Console.WriteLine(res);

            const double v = 91.299;
            AnalizeFilesViewWraper(v, "train - pos", Path.Combine(trainPath, "pos"), filesFileter, directorySearchOption, rating, true);
            AnalizeFilesViewWraper(v, "train - neg", Path.Combine(trainPath, "neg"), filesFileter, directorySearchOption, rating, false);
            AnalizeFilesViewWraper(v, "validation - pos", Path.Combine(validationPath, "pos"), filesFileter, directorySearchOption, rating, true);
            AnalizeFilesViewWraper(v, "validation - neg", Path.Combine(validationPath, "neg"), filesFileter, directorySearchOption, rating, false);
            Console.ReadKey();
        }

     
       

        static double AnalizeFilesViewWraper(double v, string title, string dir, string filesFileter, SearchOption directorySearchOption, List<HueSignificance> rating, bool isPos)
        {
            Console.WriteLine("**************************************");
            Console.WriteLine(title);
            Console.WriteLine("**************************************");
            FolderSettings fs = new FolderSettings
            {
                dir = dir,
                filesFileter = filesFileter,
                directorySearchOption = directorySearchOption,
                isPos = isPos
            };
            Action<string, bool> writeStep = (f, analRes) => Console.WriteLine($"{ f } - { analRes }");
            double res = AnalizeFilesInFolder(fs, rating, v, writeStep);
            Console.WriteLine($"Correct - { res } %");
            Console.WriteLine();
            return res;
        }

        static double AnalizeFilesInFolder(FolderSettings fs, List<HueSignificance> rating, double v, Action<string, bool> show = null)
        {
            int pos = 0;
            int count = 0;
            foreach (string f in Directory.GetFiles(fs.dir, fs.filesFileter, fs.directorySearchOption))
            {
                var img = new Image<Hsv, byte>(f);
                bool analRes = ImgFireAnalize(rating, img, v);
                if (analRes == fs.isPos) pos++;
                count++;
                show?.Invoke(f, analRes);
            }
            return pos * 100 / count;       
        }



        static bool ImgFireAnalize(List<HueSignificance> rating, Image<Hsv, byte> img, double value)
        {
            double res = 0;
            for (int i = 0; i < img.Rows; i++)
            {
                for (int j = 0; j < img.Cols; j++)
                {
                    var p = img[i, j];
                    var rp = rating.FirstOrDefault(x => x.Hue == p.Hue);
                    if (rp != null)
                        res += rp.Significance;
                }
            }
            return res >= value;
        }

        private struct FolderSettings
        {
            public string dir;
            public string filesFileter;
            public SearchOption directorySearchOption;
            public bool isPos;
        }
    }
}
