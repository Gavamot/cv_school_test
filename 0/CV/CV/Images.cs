using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Random;

namespace CV
{
    class Images
    {
        public static void GetFragments(string file, List<List<int>> allCords)
        {
            var img = new Bitmap(file);
            int n = 0;

            foreach (List<int> cords in allCords)
            {
                int x1 = cords[0];
                int y1 = cords[1];
                int x2 = cords[2];
                int y2 = cords[3];

                int width = x2 - x1 + 1;
                int height = y2 - y1 + 1;

                var fragment = new Bitmap(width, height);

                for (int i = x1; i <= x2; i++)
                    for (int j = y1; j <= y2; j++)
                        fragment.SetPixel(i - x1, j - y1, img.GetPixel(i, j));

                Directory.CreateDirectory(@"..\..\..\..\fragments");
                fragment.Save(@"..\..\..\..\fragments\" + (Path.GetFileNameWithoutExtension(file)) + "_" + Convert.ToString(n) + ".png");
                n++;
            }
        }

        public static List<List<int>> GetCords(string file)
        {
            string[] stringArray = File.ReadAllLines(file);
            List<List<int>> allCords = new List<List<int>>();
            foreach (string str in stringArray)
            {
                List<int> cords = str
                    .Split(',')
                    .Select(int.Parse).ToList();
                allCords.Add(cords);
            }
            return allCords;
        }

        public static void Greyscale(string file)
        {
            var img = new Bitmap(file);
            int x;
            for (x = 0; x < img.Width; x++)
            {
                int y;
                for (y = 0; y < img.Height; y++)
                {
                    Color pixelColor = img.GetPixel(x, y);
                    int newColor = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    img.SetPixel(x, y, Color.FromArgb(newColor, newColor, newColor));
                }
            }
            Directory.CreateDirectory(@"..\..\..\..\fragments_greyscale");
            img.Save(@"..\..\..\..\fragments_greyscale\" + (Path.GetFileNameWithoutExtension(file)) + "_grayscale" + ".png");
        }

        public static void Flip(string file)
        {
            var img = new Bitmap(file);
            img.RotateFlip(RotateFlipType.Rotate180FlipY);
            Directory.CreateDirectory(@"..\..\..\..\fragments_flip");
            img.Save(@"..\..\..\..\fragments_flip\" + (Path.GetFileNameWithoutExtension(file)) + "_flip" + ".png");
        }

        public static void Normalization(string file)
        {
            var img = new Bitmap(file);

            uint pixels = (uint)img.Height * (uint)img.Width;
            decimal Const = 255 / (decimal)pixels;
            ImageStatistics statistics = new ImageStatistics(img);
            
            int[] cdfR = statistics.Red.Values.ToArray();
            int[] cdfG = statistics.Green.Values.ToArray();
            int[] cdfB = statistics.Blue.Values.ToArray();
            
            for (int i = 1; i <= 255; i++)
            {
                cdfR[i] = cdfR[i] + cdfR[i - 1];
                cdfG[i] = cdfG[i] + cdfG[i - 1];
                cdfB[i] = cdfB[i] + cdfB[i - 1];
            }
            
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color pixelColor = img.GetPixel(x, y);

                    var R = (int)((decimal)cdfR[pixelColor.R] * Const);
                    var G = (int)((decimal)cdfG[pixelColor.G] * Const);
                    var B = (int)((decimal)cdfB[pixelColor.B] * Const);

                    Color newColor = Color.FromArgb(R, G, B);
                    img.SetPixel(x, y, newColor);
                }
            }
            
            img.Save(@"..\..\..\..\fragments_greyscale\" + (Path.GetFileNameWithoutExtension(file)) + "_norm" + ".png");
        }
        
        public static void AddNoise(string file)
        {
            Bitmap img = new Bitmap(file);

            Random rnd = new Random();
            for(int x = 0; x < img.Width; x++)
            {
                for(int y = 0; y< img.Height; y++)
                {
                    if (rnd.Next(0, 100) < 25)
                    {
                        img.SetPixel(x, y, Color.White);
                    }
                }
            }
 
            Directory.CreateDirectory(@"..\..\..\..\fragments_noise");
            img.Save(@"..\..\..\..\fragments_noise\" + (Path.GetFileNameWithoutExtension(file)) + "_noise" + ".png");
        }
    }
}

