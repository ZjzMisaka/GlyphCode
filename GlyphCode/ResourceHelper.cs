using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GlyphCode
{
    internal static class ResourceHelper
    {
        internal static void ReadStrokeOrder(string path, Dictionary<string, string> strokeOrderDic)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new[] { ": " }, StringSplitOptions.None);

                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            string value = parts[1];

                            if (!strokeOrderDic.ContainsKey(key))
                            {
                                strokeOrderDic.Add(key, value);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        internal static void ReadPng(string path, Dictionary<string, bool[,]> dictionary)
        {
            string[] pngFiles = Directory.GetFiles(path, "*.png");

            foreach (var pngFile in pngFiles)
            {
                Bitmap bitmap = new Bitmap(pngFile);
                if (bitmap.Width != 16 || bitmap.Height != 16)
                {
                    throw new Exception($"Image {pngFile} is not 16x16 pixels.");
                }

                var transparencyData = new bool[16, 16];
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        System.Drawing.Color pixel = bitmap.GetPixel(x, y);
                        transparencyData[x, y] = pixel.A == 0;
                    }
                }

                string filename = System.IO.Path.GetFileNameWithoutExtension(pngFile);
                dictionary[filename] = transparencyData;
            }
        }
    }
}
