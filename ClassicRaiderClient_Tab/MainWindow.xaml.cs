using FirstFloor.ModernUI.Windows.Controls;
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

namespace ClassicRaiderClient_Tab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private bool UploadTabAdded = false;

        public MainWindow()
        {
            InitializeComponent();

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Path))
            {
                MenuLinkGroups.Remove(uploadtab);
            } else
            {
                UploadTabAdded = true;
            }
        }

        public void AddUploadTab() {
            if (!UploadTabAdded)
            {
                MenuLinkGroups.Add(uploadtab);
                UploadTabAdded = true;
            }
        }
    }
}
