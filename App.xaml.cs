using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Text.RegularExpressions;
using RenJiCaoZuo.Common;
using RenJiCaoZuo.WebData;

namespace RenJiCaoZuo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "RenJiCaoZuo", out ret);

            if (!ret)
            {
                MessageBox.Show(@"Touch程序已经运行！");
                Environment.Exit(0);
            }

        }  

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Ezhu.AutoUpdater.Updater.CheckUpdateStatus();
            //m_AllWebData = new GetWebData();
            Application.Current.StartupUri = new Uri(@"View\MainWindow.xaml", UriKind.Relative);
        }
        
    }

}
