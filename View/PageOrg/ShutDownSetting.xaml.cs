﻿using System;
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
using System.Timers;
using System.Configuration; 
using System.Text.RegularExpressions;

namespace RenJiCaoZuo.View.PageOrg
{
    /// <summary>
    /// Interaction logic for ShutDownSetting.xaml
    /// </summary>
    public partial class ShutDownSetting : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }
        public ShutDownSetting()
        {
            InitializeComponent();
          
            string strShutDownTime = ConfigurationManager.AppSettings["ShutDownTime"];
            ShuDownTime_Edit.Text = strShutDownTime;
        }

        private void ResetTime_Button_Click(object sender, RoutedEventArgs e)
        {
            string strShutDownTime = ShuDownTime_Edit.Text;
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["ShutDownTime"].Value = strShutDownTime;
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            CommonFuntion common = new CommonFuntion();
            common.setWindowsShutDown();
        }

        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri(@"View\PageOrg\SettingWindow.xaml", UriKind.Relative));
        }

        private void ShuDownTime_Edit_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = (sender as TextBox).Text; //1234567

            if (input.Length == 5 && !Regex.IsMatch(input, @"\d{1,2}:\d{1,2}"))
            {
                MessageBox.Show("Error!, check and try again");
            }
        }
    }
}
