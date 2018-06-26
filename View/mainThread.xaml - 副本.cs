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
using System.Windows.Shapes;
using System.Configuration;
using System.Text.RegularExpressions;
using RenJiCaoZuo.Common;
using RenJiCaoZuo.WebData;
using Ezhu.AutoUpdater;

namespace RenJiCaoZuo
{
    /// <summary>
    /// Interaction logic for mainThread.xaml
    /// </summary>
    public partial class mainThread : Window
    {
        public GetWebData AllWebData = new GetWebData();
        
        public mainThread()
        {
            InitializeComponent();
            setWindowsShutDown();
            callMainPage();
        }

        private void setWindowsShutDown()
        {
            CommonFuntion pCommon = new CommonFuntion();
            pCommon.setWindowsShutDown();
        }
        
        public void callMainPage()
        {
            //MainWindow MainWindowWin = new MainWindow(this);
            //MainWindowWin.ParentWindow = this;
            //MainWindowWin.Show();
        }
    }
}
