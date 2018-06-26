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
using System.Windows.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.IO;

namespace RenJiCaoZuo
{
    /// <summary>
    /// Interaction logic for Introduction.xaml
    /// </summary>
    public partial class Introduction : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int m_TimeCount ;
        public string Activity_content;

        public Introduction(string AllTempInfo,int nMod)
        {
            InitializeComponent();
            
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (nMod == 2)
            {
                 this.TitleName.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/title04.png")); 
            }

            if (nMod == 3)
            {
                this.TitleName.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/title02.png"));
            }
            setButtonAndTimer();
            setAllTempleInfoText(AllTempInfo);            
        }

        private void setTextControl(string sText)
        {
            TextBlock sTextBlock1 = new TextBlock();
            // RichTextBox 
            sTextBlock1.Text = sText;
            sTextBlock1.HorizontalAlignment = HorizontalAlignment.Left;
            sTextBlock1.TextWrapping = TextWrapping.Wrap;
            sTextBlock1.VerticalAlignment = VerticalAlignment.Top;
            //sTextBlock1.Margin =  "32,10,0,0";
            //sTextBlock1.FontWeight = 18;
            int nFontCollum = 59;
            sTextBlock1.FontSize = 18;
            sTextBlock1.LineHeight = 27;
            sTextBlock1.Width = nFontCollum * sTextBlock1.FontSize;

            int nReturn = getReturnNumber(sText);
            double nHeight = (((sTextBlock1.Text.Length / nFontCollum) + nReturn *2) * sTextBlock1.LineHeight ) ;
            sTextBlock1.Height = ((nHeight == 0) ? sTextBlock1.LineHeight : nHeight);

            //sTextBlock1.Width = 300;
            //sTextBlock1.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //sTextBlock1.FontFamily = new Uri("");
//             sTextBlock1.Style = 
//             Style="{DynamicResource FZXKJW}"
//             sTextBlock1.Style = 
            //sTextBlock1.TextOptions.TextHintingMode = TextHintingMode.Fixed;
            
            //sTextBlock1.Padding = "0,0,0,3";
            this.addControl.Children.Add(sTextBlock1);
            
        }
        private int getReturnNumberByKey(string sText,string Key)
        {
            int nCount = 0;
            string sTempText = sText;
            string strtempa = Key;
            while (true)
            {
                int IndexofA = sTempText.IndexOf(strtempa);

                if (IndexofA == -1)
                {
                    break;
                }
                sTempText = sTempText.Substring(IndexofA + Key.Length, sTempText.Length - IndexofA - Key.Length);
                nCount++;

            }
            return nCount;
        }
        private int getReturnNumber(string sText)
        {
            int nCount = getReturnNumberByKey(sText, "\r\n") + getReturnNumberByKey(sText, "\n\r"); ;
            return nCount;
        }
        private void setImgControl(BitmapImage Pic_img)
        {
            Image sImage = new Image();
            sImage.Width = Pic_img.PixelWidth > addControl.Width ? addControl.Width : Pic_img.PixelWidth; ;
            //sImage.Height = Pic_img.PixelHeight > 480 ? 480 : Pic_img.PixelHeight;
            sImage.Stretch = Stretch.Fill;
            sImage.Source = Pic_img;
            sImage.HorizontalAlignment = HorizontalAlignment.Left;
            this.addControl.Children.Add(sImage);
        }
        /// <summary>
        /// 定时器回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setAllTempleInfoText(string AllTempInfo)
        {
            if (AllTempInfo == null)
            {
                return;
            }

            if (AllTempInfo.Length < 0)
            {
                return;
            }

            string Wenzi = AllTempInfo;
            

            Wenzi = Wenzi.TrimStart((char[])"\n\r".ToCharArray());

            string strtempa = "<img src=\"data:image/jpeg;base64,";
            string strtempb = ">";
            int nKeyLength = strtempa.Length;
            int nFound = 0;
            while(true)
            {
                int IndexofA = Wenzi.IndexOf(strtempa);
                int IndexofB = Wenzi.IndexOf(strtempb);
                string ImgString = null;
                if (IndexofA == -1)
                {
                    setTextControl(Wenzi);
                    break;
                }

                if (IndexofA != -1 && IndexofB != -1)
                {
                    if (IndexofA > 0)
                    {
                        string WenziFirst = Wenzi.Substring(0, IndexofA);
                        setTextControl(WenziFirst);
                    }

                    ImgString = Wenzi.Substring(IndexofA, IndexofB - IndexofA - nKeyLength);
                    Wenzi = Wenzi.Substring(IndexofB + 1, Wenzi.Length - IndexofB - 1);

                    nFound = IndexofB;
                    seprateImg(ImgString);
                }
            }
            
            
           
        }

        private void seprateImg(string ImgString)
        {
            if (ImgString != null && ImgString.Length > 0)
            {
                string strtempa = "<img src=\"data:image/jpeg;base64,";
                string strtempb = "\" ";
                //string strtempb = "\" ";
                int IndexofA = ImgString.IndexOf(strtempa);
                int IndexofB = ImgString.IndexOf(strtempb);
                int nLength = strtempa.Length;
                ImgString = ImgString.Substring(IndexofA + nLength, IndexofB - IndexofA - nLength);
                BitmapImage Pic_img = byteArrayToImage(Convert.FromBase64String(ImgString));
                setImgControl(Pic_img);
            }
        }

        /// <summary>
        /// 定时器回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setButtonAndTimer()
        {
            m_TimeCount = 60;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            string sButtonText = @"收起(" + m_TimeCount.ToString() + @")s";
            Return_Button.Content = sButtonText;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            m_TimeCount--;
            string sButtonText = @"收起(" + m_TimeCount.ToString() + @")s";
            Return_Button.Content = sButtonText;
            if (m_TimeCount == 0)
            {
                dispatcherTimer.Stop();
                this.Close();
            }           
        }
        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            this.Close();
        }
        
        private BitmapImage byteArrayToImage(byte[] byteArrayIn)
        {
            
            try
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(byteArrayIn, 0, byteArrayIn.Length);
                stream.Position = 0;
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                BitmapImage returnImage = new BitmapImage();
                returnImage.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                returnImage.StreamSource = ms;
                returnImage.EndInit();

                return returnImage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

    }
}
