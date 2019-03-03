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
using RenJiCaoZuo;

namespace RenJiCaoZuo.View.Page21
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Page
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void ShutDownWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Window currentWin = Application.Current.MainWindow;
            MainWindow mgen = currentWin as MainWindow;
            mgen.ShutDownSetting.IsSelected = true;
        }

        private void ModiyPassword_Button_Click(object sender, RoutedEventArgs e)
        {
            Window currentWin = Application.Current.MainWindow;
            MainWindow mgen = currentWin as MainWindow;
            mgen.ModifyPassword.IsSelected = true;
        }

        private void ReturnMain_Button_Click(object sender, RoutedEventArgs e)
        {
            Window currentWin = Application.Current.MainWindow;
            MainWindow mgen = currentWin as MainWindow;
            mgen.MainPage.IsSelected = true;
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
