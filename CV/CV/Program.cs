using System;
using System.Collections.Generic;
using System.IO;

namespace CV
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] images = Directory.GetFiles(@"..\..\..\..\images");
            Console.WriteLine("\nПолучение изображений для тренировки алгоритмов:");
            foreach (string str in images)
            {
                Console.WriteLine(str);
            }

            string[] ann = Directory.GetFiles(@"..\..\..\..\annotations");
            Console.WriteLine("\nПолучение файлов разметки для изображений:");
            foreach (string str in ann)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine("\nСоздание фрагментов изображений по файлам разметки");
            for (int i = 0; i < images.Length; i++)
            {
                List<List<int>> currentCords = Images.GetCords(ann[i]);
                Images.GetFragments(images[i], currentCords);
            }

            Console.WriteLine("\nПреобразование фрагментов в серый цвет");
            Console.WriteLine("Отражение фрагментов по горизонтали");
            Console.WriteLine("Добавление шума на исходные изображения");
            string[] fragments = Directory.GetFiles(@"..\..\..\..\fragments");
            foreach (string fragment in fragments)
            {
                Images.Greyscale(fragment);
                Images.Flip(fragment);
                Images.AddNoise(fragment);
            }

            Console.WriteLine("Нормализация серых фрагментов");
            string[] greyFragments = Directory.GetFiles(@"..\..\..\..\fragments_greyscale");
            foreach (string fragment in greyFragments)
            {
                Images.Normalization(fragment);
            }

            Console.WriteLine("\n\nВсе операции успешно завершены");
            Console.ReadKey();
        }
    }
}
