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
using System.ComponentModel;
using System.Threading;
using System.Configuration;

namespace RenJiCaoZuo.View.ListViewControl
{
    /// <summary>
    /// Interaction logic for ListBoxPicTrans.xaml
    /// </summary>
    public partial class ListBoxPicTrans : UserControl
    {
        public ListBoxPicTrans()
        {
            InitializeComponent();
            this.DataContext = new ListBoxPicViewModel();
        }
        DispatcherTimer timer = new DispatcherTimer();
        private void pageTransitionListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (pageTransitionListBox.Items.Count >= 3)
            {
                pageTransitionListBox.ScrollIntoView(pageTransitionListBox.Items[2]);
            }
            else
            {
                if (pageTransitionListBox.Items.Count > 0)
                {
                    pageTransitionListBox.ScrollIntoView(pageTransitionListBox.Items[pageTransitionListBox.Items.Count - 1]);
                }
            }
            //else
            //{
            //    if (pageTransitionListBox.Items.Count == 1)
            //    {
            //        pageTransitionListBox.ScrollIntoView(pageTransitionListBox.Items[0]);
            //    }
            //    else if (pageTransitionListBox.Items.Count == 2)
            //    {
            //        pageTransitionListBox.ScrollIntoView(pageTransitionListBox.Items[1]);
            //    }                 
            //}
            

            //string sRefreshTime = ConfigurationManager.AppSettings["Pic_ChangTime"];
            //int nRefreshTime = Convert.ToInt16(sRefreshTime);
            //timer.Interval = TimeSpan.FromMilliseconds(nRefreshTime * 1000);
            //timer.Tick += new EventHandler(timer_Tick);
            //pageTransitionListBox.SelectedIndex = 0;
            //timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (pageTransitionListBox.Items.Count > 0)
            {

                if (pageTransitionListBox.SelectedIndex == pageTransitionListBox.Items.Count - 1)
                {
                    pageTransitionListBox.SelectedIndex = 0;

                }
                else
                {
                    pageTransitionListBox.SelectedIndex += 1;
                    pageTransitionListBox.ScrollIntoView(pageTransitionListBox.Items[pageTransitionListBox.SelectedIndex]);
                }
            }
        }
    }
}
