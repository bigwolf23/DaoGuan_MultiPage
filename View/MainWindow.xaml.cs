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
using System.Windows.Threading;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using RenJiCaoZuo.WebData;
using System.Windows.Automation.Peers;
using System.Windows.Media.Animation;

namespace RenJiCaoZuo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static GetWebData m_pAllWebData = new GetWebData();
        public GetWebData _GetWebData
        {
            get { return m_pAllWebData; }
            set { m_pAllWebData = value; }
        }  
        public MainWindow()
        {
            setWindowsShutDown();
            InitializeComponent();

            string strDisplayInch = ConfigurationManager.AppSettings["DisplayInch"];
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 0;
            //this.Left = -300;
            this.Top = 0;
            //this.Top = -200;
            if (strDisplayInch == "Org")
            {
                this.MainPage_Frame.Source = new Uri(@"\View\PageOrg\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\PageOrg\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\PageOrg\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\PageOrg\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\PageOrg\ShutDownSetting.xaml", UriKind.Relative);
//                this_Frame.Source = new Uri(@"\View\PageOrg\MainPage.xaml", UriKind.Relative);
                this.Height = 1024;
                this.Width = 1280;
            }
            else if (strDisplayInch == "19" || strDisplayInch == "19_3" || strDisplayInch == "19_4")
            {
                this.Height = 1024;
                this.Width = 1280;
                this.MainPage_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\Page19\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\Page19\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\Page19\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\Page19\ShutDownSetting.xaml", UriKind.Relative);
                this.MainPage_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                //this_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);

            }
            else if (strDisplayInch == "19_2" )
            {
                this.Height = 1024;
                this.Width = 1280;
                this.MainPage_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\Page19\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\Page19\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\Page19\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\Page19\ShutDownSetting.xaml", UriKind.Relative);
                this.MainPage_Frame.Source = new Uri(@"\View\Page19\MainPage_2.xaml", UriKind.Relative);
                //this_Frame.Source = new Uri(@"\View\Page19\MainPage_2.xaml", UriKind.Relative);

            }
            else if (strDisplayInch == "21" || strDisplayInch == "21_2")
            {
                //this_Frame.Source = new Uri(@"\View\Page21\MainPage.xaml", UriKind.Relative);
                this.MainPage_Frame.Source = new Uri(@"\View\Page21\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\Page21\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\Page21\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\Page21\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\Page21\ShutDownSetting.xaml", UriKind.Relative);
                this.Height = 1080;
                this.Width = 1920;
            }
            else if (strDisplayInch == "42" || strDisplayInch == "42_2" || strDisplayInch == "42_3")
            {
                //this_Frame.Source = new Uri(@"\View\Page42\MainPage.xaml", UriKind.Relative);
                this.MainPage_Frame.Source = new Uri(@"\View\Page42\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\Page42\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\Page42\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\Page42\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\Page42\ShutDownSetting.xaml", UriKind.Relative);
                this.Height = 1920;
                this.Width = 1080;
            }
            else if (strDisplayInch == "50")
            {
                //this_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                this.MainPage_Frame.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                this.LoginPassord_Frame.Source = new Uri(@"\View\Page19\LoginPassord.xaml", UriKind.Relative);
                this.SettingWindow_Frame.Source = new Uri(@"\View\Page19\SettingWindow.xaml", UriKind.Relative);
                this.ModifyPassword_Frame.Source = new Uri(@"\View\Page19\ModifyPassword.xaml", UriKind.Relative);
                this.ShutDownSetting_Frame.Source = new Uri(@"\View\Page19\ShutDownSetting.xaml", UriKind.Relative);

                this.Height = 1080;
                this.Width = 1920;
            }
        }

        private void setWindowsShutDown()
        {
            CommonFuntion pCommon = new CommonFuntion();
            pCommon.setWindowsShutDown();
        }
    }
}
