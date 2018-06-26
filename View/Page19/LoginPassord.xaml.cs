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
using System.Windows.Navigation;
using RenJiCaoZuo;

namespace RenJiCaoZuo.View.Page19
{
    /// <summary>
    /// Interaction logic for LoginPassord.xaml
    /// </summary>
    public partial class LoginPassord : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }

        public LoginPassord()
        {
            InitializeComponent();
            //System.Diagnostics.Process.Start("osk.exe");
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            string strPassword = ConfigurationManager.AppSettings["Password"];
            if (strPassword == Password_Edit.Password)
            {
                NavigationService.Navigate(new Uri(@"View/Page19/SettingWindow.xaml", UriKind.Relative));
            }
            else{
                Password_Edit.Clear();
                MessageBox.Show(@"密码输入错误，请重新输入！");
            }
        }

        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"View/Page19/MainPage.xaml", UriKind.Relative));
        }

        private void Password_Edit_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

    }
}
