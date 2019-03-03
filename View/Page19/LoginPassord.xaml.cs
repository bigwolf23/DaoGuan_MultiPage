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
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            string strPassword = ConfigurationManager.AppSettings["Password"];
            if (strPassword == Password_Edit.Password)
            {
                Password_Edit.Clear();
                Window currentWin = Application.Current.MainWindow;
                MainWindow mgen = currentWin as MainWindow;
                mgen.SettingWindow.IsSelected = true;
            }
            else{
                Password_Edit.Clear();
                MessageBox.Show(@"密码输入错误，请重新输入！");
            }
        }

        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            string strDisplayInch = ConfigurationManager.AppSettings["DisplayInch"];
            Password_Edit.Clear();
            if (strDisplayInch == "19" || strDisplayInch == "19_3" || strDisplayInch == "19_4")
            {
                Window currentWin = Application.Current.MainWindow;
                MainWindow mgen = currentWin as MainWindow;
                mgen.MainPage.IsSelected = true;
            }
            else if (strDisplayInch == "19_2")
            {
                Window currentWin = Application.Current.MainWindow;
                MainWindow mgen = currentWin as MainWindow;
                mgen.MainPage.IsSelected = true;
            }

        }

        private void Password_Edit_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

    }
}
