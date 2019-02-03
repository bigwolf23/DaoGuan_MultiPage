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
using RenJiCaoZuo.Common;

namespace RenJiCaoZuo.View
{
    /// <summary>
    /// Interaction logic for PicTransationControl.xaml
    /// </summary>
    public partial class PicTransationControl : UserControl
    {
        public PicTransationControl()
        {
            InitializeComponent();
            this.Width = 1080;
            this.Height = 733;
        }

        private void PicTransationControl_Loaded(object sender, RoutedEventArgs e)
        {
            PicInfo pData = (PicInfo)this.DataContext;
            //this.Width = pData.nWidth;
            //this.Height = pData.nHeight;
            ImgLink.Source = new BitmapImage(new Uri(pData.strImgLink));


        }
    }
}
