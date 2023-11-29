using System;
using System.Collections.Generic;
using System.Drawing;
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
        public MainWindow()
        {
            InitializeComponent();

            ResourceHelper.ReadStrokeOrder("StrokeOrder.txt", strokeOrderDic);

            ResourceHelper.ReadPng("img/Characters", charactersDic);
            ResourceHelper.ReadPng("img/Numbers", numbersDic);
        }

        
    }
}
