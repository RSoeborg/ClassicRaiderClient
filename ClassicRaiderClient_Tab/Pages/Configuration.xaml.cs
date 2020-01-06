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

namespace ClassicRaiderClient_Tab.Pages
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : UserControl
    {
        public Configuration()
        {
            InitializeComponent();
            

            Loaded += (s, e) => {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.Path) && !GlobalVariables.ConfigurationTabRedirected)
                {
                    SetAddonPathLabel();
                    GlobalVariables.ConfigurationTabRedirected = true;
                    NavigationCommands.GoToPage.Execute("/Pages/Upload.xaml", this);
                }
            };

            GlobalVariables.Environment = new ClassicRaiderEnvironment();

            select_file.Click += (s, e) =>
            {
                var UserAddonPath = GlobalVariables.Environment.GetAddonPath();

                if (!string.IsNullOrWhiteSpace(UserAddonPath))
                {
                    Properties.Settings.Default.Path = UserAddonPath;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();

                    SetAddonPathLabel();
                    GlobalVariables.Environment.Setup(UserAddonPath);
                    SwitchToUploadTab();
                }
            };
        }

        private void SetAddonPathLabel()
        {
            selected_file_path.Text = Properties.Settings.Default.Path;
        }

        private void SwitchToUploadTab()
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.AddUploadTab();
            NavigationCommands.GoToPage.Execute("/Pages/Upload.xaml", this);
        }
    }
}
