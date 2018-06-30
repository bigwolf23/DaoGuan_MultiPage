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
using RenJiCaoZuo;

namespace RenJiCaoZuo.View.Page21
{
    /// <summary>
    /// Interaction logic for ModifyPassword.xaml
    /// </summary>
    public partial class ModifyPassword : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }

        public ModifyPassword()
            //public ModifyPassword(MainWindow parent)
        {
            //parentWindow = parent;
            InitializeComponent();
            //System.Diagnostics.Process.Start("osk.exe");
        }

        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(@"View\Page21\SettingWindow.xaml", UriKind.Relative));
        }

        private void SavePassword_Button_Click(object sender, RoutedEventArgs e)
        {
            string strPassword = ConfigurationManager.AppSettings["Password"];
            if (strPassword == OldPassword_Edit.Password)
            {
                if (NewPassword_Edit.Password.Length == 0)
                {
                    MessageBox.Show(@"新密码不能为空，请输入！");
                    return;
                }
                if (NewPassword_Edit.Password == ConfirmPassword_Edit.Password )
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["Password"].Value = NewPassword_Edit.Password;
                    cfa.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");

                    NavigationService.Navigate(new Uri(@"View\Page21\SettingWindow.xaml", UriKind.Relative));
                }
                else
                {
                    NewPassword_Edit.Clear();
                    ConfirmPassword_Edit.Clear();
                    MessageBox.Show(@"新旧密码不一致，请重新输入！");
                }
            }
            else
            {
                OldPassword_Edit.Clear();
                MessageBox.Show(@"原密码输入错误，请重新输入！");
            }

        }
    }
}
