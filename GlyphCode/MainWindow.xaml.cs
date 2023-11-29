using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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

namespace GlyphCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> strokeOrderDic = new Dictionary<string, string>();
        private Dictionary<string, bool[,]> charactersDic = new Dictionary<string, bool[,]>();
        private Dictionary<string, bool[,]> numbersDic = new Dictionary<string, bool[,]>();
        private Dictionary<string, bool[,]> symbolsDic = new Dictionary<string, bool[,]>();

        private bool[,] spaceData;

        private int offsetX = 0;
        private int offsetY = 0;

        public MainWindow()
        {
            InitializeComponent();

            spaceData = new bool[16, 16];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    spaceData[i, j] = true;
                }
            }

            ResourceHelper.ReadStrokeOrder("StrokeOrder.txt", strokeOrderDic);

            ResourceHelper.ReadPng("img/Characters", charactersDic);
            ResourceHelper.ReadPng("img/Numbers", numbersDic);
            ResourceHelper.ReadPng("img/Symbols", symbolsDic);
        }

        private void BtnStartClick(object sender, RoutedEventArgs e)
        {
            string input = tbInput.Text;

            for (int i = 0; i < input.Length; ++i)
            {
                char text = input[i];
                string bText = null;
                if (i - 1 >= 0)
                {
                    bText = input[i - 1].ToString();
                }

                if (bText != null)
                {
                    if (char.GetUnicodeCategory(bText[0]) == UnicodeCategory.OtherLetter && char.GetUnicodeCategory(text) == UnicodeCategory.OtherLetter)
                    {
                        if (!DrawText(" ", true))
                        {
                            return;
                        }
                    }
                }

                if (!DrawText(text.ToString(), false))
                {
                    return;
                }
            }

            cvDrawing.Height = offsetY + 16;
        }

        private bool DrawText(string text, bool autoSpace)
        {
            if (text == "\n")
            {
                offsetX = 0;
                offsetY += 16;
                return true;
            }

            bool isNumeric = int.TryParse(text, out _);
            Dictionary<string, bool[,]> imgInfoDic;
            string infoText;
            if (isNumeric)
            {
                imgInfoDic = numbersDic;
                infoText = text;
            }
            else if (text.Length == 1 && "[]()<>{}⟪ ⟫（）［］｛｝《》【】「」『』,.，。!?！？:：".Contains(text))
            {
                imgInfoDic = symbolsDic;
                if (",，".Contains(text))
                {
                    infoText = "a";
                }
                else if (".。".Contains(text))
                {
                    infoText = "b";
                }
                else if ("[(<{⟪（［｛《【「『".Contains(text))
                {
                    infoText = "c";
                }
                else if ("])>}⟫）］｝》】」』".Contains(text))
                {
                    infoText = "d";
                }
                else if ("!！".Contains(text))
                {
                    infoText = "e";
                }
                else if ("?？".Contains(text))
                {
                    infoText = "f";
                }
                else if (":：".Contains(text))
                {
                    infoText = "g";
                }
                else
                {
                    infoText = " ";
                }
            }
            else
            {
                imgInfoDic = charactersDic;

                if (!strokeOrderDic.ContainsKey(text))
                {
                    infoText = " ";
                }
                else
                {
                    infoText = strokeOrderDic[text];
                }
            }

            double width;
            if (tbWidth.Text == "")
            {
                width = gCanvas.ActualWidth - 2;
                cvDrawing.Width = width - 1;
            }
            else
            {
                if (!double.TryParse(tbWidth.Text, out width))
                {
                    MessageBox.Show("Wrong width");
                    return false;
                }
                cvDrawing.Width = width;
            }
            if (offsetX + infoText.Length * 16 > width)
            {
                if (offsetX == 0)
                {
                    MessageBox.Show("Width too small");
                    return false;
                }

                offsetX = 0;
                offsetY += 16;
            }

            foreach (char info in infoText)
            {
                bool[,] imageData;
                if (info.ToString() == " ")
                {
                    if (autoSpace && offsetX == 0)
                    {
                        continue;
                    }

                    imageData = spaceData;
                }
                else
                {
                    imageData = imgInfoDic[info.ToString()];
                }
                DrawImage(imageData, offsetX, offsetY);
                offsetX += 16;
            }

            return true;
        }

        private void DrawImage(bool[,] imageData, int offsetX, int offsetY)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    if (!imageData[x, y])
                    {
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                        {
                            Width = 1,
                            Height = 1,
                            Fill = System.Windows.Media.Brushes.Black
                        };

                        Canvas.SetLeft(rectangle, x + offsetX);
                        Canvas.SetTop(rectangle, y + offsetY);

                        cvDrawing.Children.Add(rectangle);
                    }
                }
            }
        }

        private void BtnClearClick(object sender, RoutedEventArgs e)
        {
            cvDrawing.Children.Clear();
            offsetX = 0;
            offsetY = 0;
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)cvDrawing.ActualWidth, (int)cvDrawing.ActualHeight, 100.0, 100.0, PixelFormats.Default);
            bmp.Render(cvDrawing);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            System.IO.FileStream stream = System.IO.File.Create("Canvas.png");
            encoder.Save(stream);
            stream.Close();
        }
    }
}
