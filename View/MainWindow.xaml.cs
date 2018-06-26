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
    public partial class MainWindow : NavigationWindow
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
            this.Top = 0;
            if (strDisplayInch == "Org")
            {
                this.Source = new Uri(@"\View\PageOrg\MainPage.xaml", UriKind.Relative);
                this.Height = 1024;
                this.Width = 1280;
            }
            else if (strDisplayInch == "19")
            {
                this.Source = new Uri(@"\View\Page19\MainPage.xaml", UriKind.Relative);
                this.Height = 1139;
                this.Width = 1424;
            }
            else if (strDisplayInch == "21")
            {
                this.Source = new Uri(@"\View\Page21\MainPage.xaml", UriKind.Relative);
                this.Height = 1055;
                this.Width = 1874;
            }
            else if (strDisplayInch == "42")
            {
                this.Source = new Uri(@"\View\Page42\MainPage.xaml", UriKind.Relative);
                this.Height = 1024;
                this.Width = 1280;
            }
            else if (strDisplayInch == "50")
            {
                this.Source = new Uri(@"\View\Page50\MainPage.xaml", UriKind.Relative);
                this.Height = 1024;
                this.Width = 1280;
            }



        }

        private void setWindowsShutDown()
        {
            CommonFuntion pCommon = new CommonFuntion();
            pCommon.setWindowsShutDown();
        }

        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;
                _navArgs = e;
                this.IsHitTestVisible = false;
                DoubleAnimation da = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(100)));
                da.Completed += FadeOutCompleted;
                this.BeginAnimation(OpacityProperty, da);
            }
            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            (sender as AnimationClock).Completed -= FadeOutCompleted;

            this.IsHitTestVisible = true;

            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                    {
                        NavigationService.Navigate(_navArgs.Content);
                    }
                    else
                    {
                        NavigationService.Navigate(_navArgs.Uri);
                    }
                    break;
                case NavigationMode.Back:
                    NavigationService.GoBack();
                    break;

                case NavigationMode.Forward:
                    NavigationService.GoForward();
                    break;
                case NavigationMode.Refresh:
                    NavigationService.Refresh();
                    break;
            }

//             Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
//                 (ThreadStart)delegate()
//             {
//                 DoubleAnimation da = new DoubleAnimation(2.0d, new Duration(TimeSpan.FromMilliseconds(100)));
//                 this.BeginAnimation(OpacityProperty, da);
//             });
        }

        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;
    }
}
