using System;
using Microsoft.Win32;
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
using System.Windows.Shapes;
using System.Configuration;

namespace RenJiCaoZuo.View.PageOrg
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }
        //public SettingWindow(MainWindow parent)
        public SettingWindow()
        {
            //parentWindow = parent;
            InitializeComponent();
        }

        private void ShutDownWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"View\PageOrg\ShutDownSetting.xaml", UriKind.Relative));
        }

        private void ModiyPassword_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"View\PageOrg\ModifyPassword.xaml", UriKind.Relative));
        }

        private void ReturnMain_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"View\PageOrg\MainPage.xaml", UriKind.Relative));
        }

        private void ReturnDesktop_Button_Click(object sender, RoutedEventArgs e)
        {
            //App.WindowState = WindowState.Minimized;
            Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized; 
        }

        private void SetVideo_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Video File(*.avi;*.mp4;*.wmv)|*.avi;*.mp4;*.wmv|All File(*.*)|*.*";

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfa.AppSettings.Settings["Video_Path"].Value = dialog.FileName;
                cfa.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

        }
    }
}
