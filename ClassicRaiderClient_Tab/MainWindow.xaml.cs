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
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("LOGO-ICON.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            ni.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            ni.ContextMenuStrip.Items.Add("Show", null, (s,e)=> {
                this.Show();
            });
            ni.ContextMenuStrip.Items.Add("Exit", null, (s,e)=> {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            });


            Closing += (s, e) => {
                e.Cancel = true;
                this.Hide();
            };

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
