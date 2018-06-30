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
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using RenJiCaoZuo;
using RenJiCaoZuo.WebData;
using System.Windows.Automation.Peers;
using RenJiCaoZuo;

namespace RenJiCaoZuo.View.Page21
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        MainWindow parentWindow;
        public MainWindow ParentWindow
        {
            get { return parentWindow; }
            set { parentWindow = value; }
        }  

        //public GetWebData m_pAllWebData = new GetWebData();
        class PayListHistory
        {
            public string Name { get; set; }
            public string payTypeName { get; set; }
            public double amount { get; set; }
        }

        public class monkinfoDisp
        {
            public string MonkInfoImage { get; set; }
            public string MonkName { get; set; }
            public string MonkInfo { get; set; }
            public int MonkInfoIndex { get; set; }
        }

        //获取寺庙活动的内容
        public class ActivityList
        {
            public string ActivityMain { get; set; }
            public string ActivityMainDetail { get; set; }
        }


        private DispatcherTimer dispatcherDonateTimerList = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
        //activity 的label更新的timer
        private DispatcherTimer dispatcherTimerList = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };

        private DispatcherTimer dispatcherTimerRefresh = null;
        //dispatcherSrcollBarTimer这个timer暂时不用，保留
        private DispatcherTimer dispatcherSrcollBarTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(40) };

        private DispatcherTimer dispatcherAllPageRefreshTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(5) };


        public GetWebData pWebData;
        public Uri ImageFilePathUri;
        private string Activity_MonkImage;
        private string Media_play_Path;
        Queue<ActivityList> myActivityInfoQueue = new Queue<ActivityList>();
        Queue<PayListHistory> myPayQueue = new Queue<PayListHistory>();
        //显示法师ListView内容
        List<monkinfoDisp> m_MonkList = new List<monkinfoDisp>();
        private Dictionary<int, string> m_MonkinfoDetail = new Dictionary<int, string>();
        //显示活动横向list内容
        List<ActivityList> m_pActivityListInfo = new List<ActivityList>();
        string strMode = ConfigurationManager.AppSettings["FirstPageName"];

        private void setWindowsShutDown()
        {
            CommonFuntion pCommon = new CommonFuntion();
            pCommon.setWindowsShutDown();

        }
        private void getPageRefreshTime()
        {
            string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
            int nRefreshTime = Convert.ToInt16(sRefreshTime);

            dispatcherTimerRefresh = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(nRefreshTime) };

        }

        public MainPage()
        {
            pWebData =  MainWindow.m_pAllWebData;
            InitializeComponent();
            getPageRefreshTime();
            setDisplayByMode(strMode);

            if (strMode == "3")
            {
                Media_play_Path = ConfigurationManager.AppSettings["Video_Path"];
                string Media_play_Mode = ConfigurationManager.AppSettings["Video_Play_Mode"];
                MediaPlay.Source = new Uri(Media_play_Path);
                if (Media_play_Mode == "None")
                {
                    MediaPlay.Stretch = Stretch.Uniform;
                }
                else
                {
                    MediaPlay.Stretch = Stretch.Fill;
                }
                
            }
            else
            {
                if (pWebData != null)
                {
                    if (pWebData.m_pTempInfoData.success == true)
                    {
                        //显示寺庙介绍
                        setTempInfoData();
                        //设定寺院名字的图片
                        setTemplInfoNamePic();
                    }

                    if (pWebData.m_pMonkInfoData.success == true)
                    {
                        //显示法师ListView内容
                        displayMonkList();
                    }

                    if (pWebData.m_pActivityInfoData.success == true)
                    {
                        //获取寺庙活动的内容
                        getActiveInfoContent();
                        //显示寺庙活动在label上
                        if (strMode == "1")
                        {
                            DisplayActiveInfoContent();
                        }
                        //显示寺庙活动在listview中
                        DisplayActiveInfoContentInList();
                    }
                

                   
                }
            }

            if (pWebData != null)
            {
                if (pWebData.m_pTempInfoData.success == true)
                {
                    //设定寺院名字的图片
                    setTemplInfoNamePic();
                }

                if (pWebData.m_pqRCodeInfoData.success == true)
                {
                    //设定在线功德二维码
                    setQRCodePic_Zxgdx();
                    //设定公众号二维码
                    setQRCodePic_Gzgzh();
                }

                if (pWebData.m_pHousePayHistoryData.success == true)
                {
                    getDonateHouseContent();
                }
                if (pWebData.m_pTemplePayHistoryData.success == true)
                {
                    ////获取捐赠listview的内容
                    getDonateListContent();
                    //显示捐赠listview内容
                    displayDonateList();

                    //                     DisplayDonateListByTimer();

                }
            }
            Page_All_Refresh();
        }

        private void Page_All_Refresh()
        {
            try
            { 
                dispatcherAllPageRefreshTimer.Tick += delegate
                {
                    if(TemplInfo_TextBlock.Text.Length == 0)
                    {
                        pWebData.GetTempleInfobyWebService();
                        //显示寺庙介绍
                        setTempInfoData();
                        //设定寺院名字的图片
                        setTemplInfoNamePic();

                        pWebData.GetMonkInfobyWebService();
                        //显示法师ListView内容
                        displayMonkList();

                        pWebData.GetActivityInfobyWebService();
                        //获取寺庙活动的内容
                        getActiveInfoContent();
                        //显示寺庙活动在label上
                        if (strMode == "1")
                        {
                            DisplayActiveInfoContent();
                        }
                        //显示寺庙活动在listview中
                        DisplayActiveInfoContentInList();

                        pWebData.GetTemplePayHistorybyWebService();
                        getDonateHouseContent();
                        ////获取捐赠listview的内容
                        getDonateListContent();
                        //显示捐赠listview内容
                        displayDonateList();

                    }

                    if (QRCode_Image_Zxgdx.Source == null)
                    {
                        pWebData.GetqRCodeInfobyWebService();
                        //设定二维码
                        setQRCodePic_Zxgdx();
                    }

                    if (QRCode_Image_Gzgzh.Source == null)
                    {
                        pWebData.GetqRCodeInfobyWebService();
                        //设定二维码
                        setQRCodePic_Gzgzh();
                    }



                    if (QRCode_Image_Zxgdx.Source != null &&
                        TemplInfo_TextBlock.Text.Length != 0)
                    {
                        dispatcherAllPageRefreshTimer.Stop();
                    }
                };
                dispatcherAllPageRefreshTimer.Start();
            }
            catch (Exception ex)
            {

            }
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void setDisplayByMode(string strMode)
        {
            setActivityAndMonk_Img(strMode);
            setMonk_PageShow(strMode);
        }

        //设定法师或者活动的图片

        private void setActivityAndMonk_Img(string strMode)
        {
            if (strMode == "1")
            {
                Activity_MonkImage = "pack://SiteOfOrigin:,,,/Res/Page21/Dzjsbt.png";
            }
            else
            {
                Activity_MonkImage = "pack://SiteOfOrigin:,,,/Res/Page21/zxgd.png";
            }
            Uri ImageFilePathUri = new Uri(Activity_MonkImage);
            this.MonkOrActive_Intrduce_Pic.Source = new BitmapImage(ImageFilePathUri);
        }

        private void setMonk_PageShow(string strMode)
        {
            if (strMode == "1")
            {
                this.UpPage_Button.Visibility = Visibility.Visible;
                this.DownPage_Button.Visibility = Visibility.Visible;
                this.NewsBackground_Img.Visibility = Visibility.Visible;
                this.ActivityInfo_Label.Visibility = Visibility.Visible;
                this.MonkInfo_ListView.Visibility = Visibility.Visible;

                Activity_Detail.Visibility = Visibility.Hidden;
                ActivityInfo_Next_Button.Visibility = Visibility.Hidden;
                ActivityInfo_Prev_Button.Visibility = Visibility.Hidden;
                ActivityInfo_ListView.Visibility = Visibility.Hidden;
                this.MediaPlay.Visibility = Visibility.Hidden;
               

                this.ActivityAndMonk_Img.Visibility = Visibility.Hidden;
            }
            else if (strMode == "2")
            {
                this.UpPage_Button.Visibility = Visibility.Hidden;
                this.DownPage_Button.Visibility = Visibility.Hidden;
                this.NewsBackground_Img.Visibility = Visibility.Hidden;
                this.ActivityInfo_Label.Visibility = Visibility.Hidden;
                this.MonkInfo_ListView.Visibility = Visibility.Hidden;


                Activity_Detail.Visibility = Visibility.Hidden;
                ActivityInfo_Next_Button.Visibility = Visibility.Visible;
                ActivityInfo_Prev_Button.Visibility = Visibility.Visible;
                ActivityInfo_ListView.Visibility = Visibility.Visible;

                this.MediaPlay.Visibility = Visibility.Hidden;
            }
            else
            {
                this.UpPage_Button.Visibility = Visibility.Hidden;
                this.DownPage_Button.Visibility = Visibility.Hidden;
                //Activity label infor
                this.NewsBackground_Img.Visibility = Visibility.Hidden;
                this.ActivityInfo_Label.Visibility = Visibility.Hidden;
                this.MonkInfo_ListView.Visibility = Visibility.Hidden;
                
                Activity_Detail.Visibility = Visibility.Hidden;
                ActivityInfo_Next_Button.Visibility = Visibility.Hidden;
                ActivityInfo_Prev_Button.Visibility = Visibility.Hidden;
                ActivityInfo_ListView.Visibility = Visibility.Hidden;

                TemplInfo_TextBlock.Visibility = Visibility.Hidden;
                //TempInfo_Image.Visibility = Visibility.Hidden;
                ActivityAndMonk_Img.Visibility = Visibility.Hidden;
                TempInfo_Detail.Visibility = Visibility.Hidden;
                TempInfo_Intrduce.Visibility = Visibility.Hidden;
                Temple_Intrduce_Frame.Visibility = Visibility.Hidden;
                TempInfo_Intrduce_Pic.Visibility = Visibility.Hidden;
                this.MonkOrActive_Intrduce_Pic.Visibility = Visibility.Hidden;
                Seprate_Line1.Visibility = Visibility.Hidden;
                Seprate_Line2.Visibility = Visibility.Hidden;

                this.MediaPlay.Visibility = Visibility.Visible;
            }


        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            //设置一下视频进度，确保从头开始播放
            MediaPlay.Position = TimeSpan.Zero;
            MediaPlay.Play();
        }

        //
        private void DisplayDonateListByTimer()
        {

            try
                {
                //this.DonateInfo_List.ClearValue();
                dispatcherTimerRefresh.Tick += delegate
                {

                    //                 myPayQueue.Clear();
                    //                 dispatcherDonateTimerList.Stop();
                    //                 this.DonateInfo_List.ItemsSource = null;
                    //                 this.DonateInfo_List.Items.Clear();
                    //                 this.DonateInfo_List.Items.Refresh();
                    //获取捐赠TextBox的内容
                    getDonateHouseContent();
                    //                 pWebData.GetTemplePayHistorybyWebService();
                    //                 if (pWebData.m_pTemplePayHistoryData.success == true)
                    //                 {
                    //                     //获取捐赠listview的内容
                    //                     getDonateListContent();
                    //                     //显示捐赠listview内容
                    //                     displayDonateList();
                    //                 }
                };
                dispatcherTimerRefresh.Start();
            }
            catch (Exception ex){ }
        }

        //获取捐赠TextBox的内容
        private void getDonateHouseContent()
        {
            pWebData.GetHousePayHistorybyWebService();
            if (pWebData != null &&
                pWebData.m_pHousePayHistoryData != null &&
                pWebData.m_pHousePayHistoryData.body != null &&
                pWebData.m_pHousePayHistoryData.body.data != null)
            {
                if (pWebData.m_pHousePayHistoryData.body.data.name != null)
                {
                    HouseName.Text = pWebData.m_pHousePayHistoryData.body.data.name;
                }

                if (pWebData.m_pHousePayHistoryData.body.data.payTypeName != null)
                {
                    HousepayTypeName.Text = pWebData.m_pHousePayHistoryData.body.data.payTypeName;
                }
                Houseamount.Text = pWebData.m_pHousePayHistoryData.body.data.amount.ToString();
            }
        }


        //获取捐赠ListView的内容
        private void getDonateListContent()
        {
            try
            {
                myPayQueue.Clear();
                if (pWebData != null &&
                    pWebData.m_pTemplePayHistoryData != null &&
                    pWebData.m_pTemplePayHistoryData.body != null &&
                    pWebData.m_pTemplePayHistoryData.body.data != null)
                {
                    foreach (TemplePayHistoryDatabody payHistTemp in pWebData.m_pTemplePayHistoryData.body.data)
                    {
                        PayListHistory temp = new PayListHistory();
                        temp.amount = payHistTemp.amount;
                        if (payHistTemp.name != null)
                        {
                            temp.Name = payHistTemp.name;
                        }

                        if (payHistTemp.payTypeName != null)
                        {
                            temp.payTypeName = payHistTemp.payTypeName;
                        }

                        myPayQueue.Enqueue(temp);
                    }
                }
            }
            catch (Exception ex){ }
        }



        //显示捐赠ListView内容
        private void displayDonateList()
        {
            int nCount = 0;
            this.DonateInfo_List.ItemsSource = myPayQueue.ToList();
            try
            {
                if (myPayQueue.Count > 0)
                {
                    dispatcherDonateTimerList.Tick += delegate
                    {
                        nCount++;
                        string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
                        int nRefreshTime = Convert.ToInt16(sRefreshTime);
                        if (nCount % nRefreshTime == 0)
                        {
                            nCount = 0;
                            myPayQueue.Clear();
                            this.DonateInfo_List.ItemsSource = null;
                            this.DonateInfo_List.Items.Clear();
                            this.DonateInfo_List.Items.Refresh();

                            pWebData.GetTemplePayHistorybyWebService();
                            if (pWebData == null ||
                                pWebData.m_pTemplePayHistoryData == null || 
                                pWebData.m_pTemplePayHistoryData.body == null)
                            {
                                //dispatcherDonateTimerList.Stop();
                                return;
                            }
                            else
                            {
                                getDonateListContent();

                                this.DonateInfo_List.ItemsSource = myPayQueue.ToList();

                                //获取捐赠TextBox的内容
                                getDonateHouseContent();
                            }

                        }
                        else
                        {
                            if (myPayQueue.Count > 0)
                            {
                                myPayQueue.Enqueue(myPayQueue.Dequeue());  // 把队列中派头的放到队尾
                                this.DonateInfo_List.ItemsSource = myPayQueue.ToList();
                            }

                        }

                    };
                    dispatcherDonateTimerList.Start();
                }

            }
            catch (Exception ex)
            {
                //dispatcherDonateTimerList.Stop();
            }

        }

        private void getActiveInfoContent()
        {
            try
            {
                if (m_pActivityListInfo.Count == 0)
                {
                    return;
                }
                this.ActivityInfo_ListView.ItemsSource = m_pActivityListInfo.ToList();
                if (pWebData == null ||
                    pWebData.m_pActivityInfoData == null || 
                    pWebData.m_pActivityInfoData.body == null || 
                    pWebData.m_pActivityInfoData.body.data == null)
                {
                    return;
                }

                foreach (ActivityInfoDatabody ActivityInfTemp in pWebData.m_pActivityInfoData.body.data)
                {
                    ActivityList pTemp = new ActivityList();
                    pTemp.ActivityMain = ActivityInfTemp.activity;
                    pTemp.ActivityMainDetail = ActivityInfTemp.detail;
                    m_pActivityListInfo.Add(pTemp);
                    myActivityInfoQueue.Enqueue(pTemp);
                }

                //             foreach (ActivityInfoDatabody ActivityInfTemp in pWebData.m_pActivityInfoData.body.data)
                //             {
                //                 ActivityList pTemp = new ActivityList();
                //                 pTemp.ActivityMain =  "1" + ActivityInfTemp.activity ;
                //                 pTemp.ActivityMainDetail = "1" + ActivityInfTemp.detail;
                //                 m_pActivityListInfo.Add(pTemp);
                //                 myActivityInfoQueue.Enqueue(ActivityInfTemp.activity);
                //             }
                // 
                //             foreach (ActivityInfoDatabody ActivityInfTemp in pWebData.m_pActivityInfoData.body.data)
                //             {
                //                 ActivityList pTemp = new ActivityList();
                //                 pTemp.ActivityMain = "2" + ActivityInfTemp.activity;
                //                 pTemp.ActivityMainDetail = "2" + ActivityInfTemp.detail;
                //                 m_pActivityListInfo.Add(pTemp);
                //                 myActivityInfoQueue.Enqueue(ActivityInfTemp.activity);
                //             }

                this.ActivityInfo_ListView.ItemsSource = m_pActivityListInfo.ToList();
            }
            catch (Exception ex){ }
        }
        public string strActivityInfoDetail;
        //显示寺庙活动
        private void DisplayActiveInfoContent()
        {
            try
            {
                if (myActivityInfoQueue.Count > 0)
                {
                    ActivityList pTemp = myActivityInfoQueue.Dequeue();
                    string strDisp = pTemp.ActivityMain;
                    ActivityInfo_Label.Content = strDisp;
                    strActivityInfoDetail = pTemp.ActivityMainDetail;
                    myActivityInfoQueue.Enqueue(pTemp);

                    dispatcherTimerList.Tick += delegate
                    {
                        pTemp = myActivityInfoQueue.Dequeue();
                        strDisp = pTemp.ActivityMain;
                        ActivityInfo_Label.Content = strDisp;
                        strActivityInfoDetail = pTemp.ActivityMainDetail;
                        myActivityInfoQueue.Enqueue(pTemp);  // 把队列中派头的放到队尾
                    };
                    dispatcherTimerList.Start();
                }
            }
            catch (Exception ex) { }

        }

        //显示寺庙活动

        private void DisplayActiveInfoContentInList()
        {
            if (myActivityInfoQueue.Count > 0)
            {
                this.ActivityInfo_ListView.ItemsSource = m_pActivityListInfo.ToList();

                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn01.png"));
                this.ActivityInfo_Prev_Button.Content = img;

                if (m_pActivityListInfo.Count <= 1)
                {
                    Image imgDown = new Image();
                    imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn03.png"));
                    this.ActivityInfo_Next_Button.Content = imgDown;
                }
                else
                {
                    Image imgDown = new Image();
                    imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn04.png"));
                    this.ActivityInfo_Next_Button.Content = imgDown;
                }
            }

        }

        private void displayMonkList()
        {
            try
            { 
                this.MonkInfo_ListView.ItemsSource = m_MonkList.ToList();
                int nIndex = 0;
                if (pWebData == null ||
                    pWebData.m_pMonkInfoData == null ||
                    pWebData.m_pMonkInfoData.body == null ||
                    pWebData.m_pMonkInfoData.body.data == null)
                {
                    return;
                }

                foreach (MonkInfoDatabody MonkTemp in pWebData.m_pMonkInfoData.body.data)
                {
                    nIndex++;
                    monkinfoDisp temp = new monkinfoDisp();
                    if (MonkTemp.info!=null)
                    {
                        temp.MonkInfo = MonkTemp.info;
                    }
                    if (MonkTemp.url != null)
                    {
                        temp.MonkInfoImage = MonkTemp.url;
                    }
                    if (MonkTemp.name != null)
                    {
                        temp.MonkName = MonkTemp.name;
                    }
                
                    temp.MonkInfoIndex = nIndex;
                    m_MonkList.Add(temp);

                    m_MonkinfoDetail.Add(nIndex, MonkTemp.detail);
                }

                ////Test Source
                //foreach (MonkInfoDatabody MonkTemp in pWebData.m_pMonkInfoData.body.data)
                //{
                //    nIndex++;
                //    monkinfoDisp temp = new monkinfoDisp();
                //    if (MonkTemp.info != null)
                //    {
                //        temp.MonkInfo = MonkTemp.info;
                //    }
                //    if (MonkTemp.url != null)
                //    {
                //        temp.MonkInfoImage = MonkTemp.url;
                //    }
                //    if (MonkTemp.name != null)
                //    {
                //        temp.MonkName = MonkTemp.name;
                //    }

                //    temp.MonkInfoIndex = nIndex;
                //    m_MonkList.Add(temp);

                //    m_MonkinfoDetail.Add(nIndex, MonkTemp.detail);
                //}
                ////Test Source
    
            }
            catch (Exception ex){ }

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn01.png"));
            this.UpPage_Button.Content = img;

            if (m_MonkList.Count <= 1)
            {
                Image imgDown = new Image();
                imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn03.png"));
                this.DownPage_Button.Content = imgDown;
            }
            else
            {
                Image imgDown = new Image();
                imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn04.png"));
                this.DownPage_Button.Content = imgDown;
            }


            this.MonkInfo_ListView.ItemsSource = m_MonkList.ToList();
        }


        //获取在线功德二维码
        private void setQRCodePic_Zxgdx()
        {
            if (pWebData != null && 
                pWebData.m_pqRCodeInfoData != null && 
                pWebData.m_pqRCodeInfoData.body != null && 
                pWebData.m_pqRCodeInfoData.body.data != null && 
                pWebData.m_pqRCodeInfoData.body.data.url != null)
            {
                if (pWebData.m_pqRCodeInfoData.body.data.url.Length > 0)
                {
                    Uri ImageFilePathUri = new Uri(pWebData.m_pqRCodeInfoData.body.data.url);
                    QRCode_Image_Zxgdx.Source = new BitmapImage(ImageFilePathUri);
                }

            }

        }

        //获取关注公众号二维码
        private void setQRCodePic_Gzgzh()
        {
            if (pWebData != null &&
                pWebData.m_pqRCodeInfoData != null &&
                pWebData.m_pqRCodeInfoData.body != null &&
                pWebData.m_pqRCodeInfoData.body.data != null &&
                pWebData.m_pqRCodeInfoData.body.data.url != null)
            {
                if (pWebData.m_pqRCodeInfoData.body.data.url.Length > 0)
                {
                    Uri ImageFilePathUri = new Uri(pWebData.m_pqRCodeInfoData.body.data.url);
                    QRCode_Image_Gzgzh.Source = new BitmapImage(ImageFilePathUri);
                }

            }

        }

        //获取寺庙名字的图片
        private void setTemplInfoNamePic()
        {
            if (pWebData != null &&
                pWebData.m_pTempInfoData != null &&
                pWebData.m_pTempInfoData.body != null &&
                pWebData.m_pTempInfoData.body.data != null &&
                pWebData.m_pTempInfoData.body.data.url != null )
            {
                Uri ImageFilePathUri = new Uri(pWebData.m_pTempInfoData.body.data.url);
                //unused source
                //TempInfo_Image.Source = new BitmapImage(ImageFilePathUri);
            }

        }


        //获取寺庙的基本信息
        private void setTempInfoData()
        {
            if (pWebData != null &&
                pWebData.m_pTempInfoData != null && 
                pWebData.m_pTempInfoData.body != null &&
                pWebData.m_pTempInfoData.body.data != null &&
                pWebData.m_pTempInfoData.body.data.info != null)
            {
                TemplInfo_TextBlock.Text = pWebData.m_pTempInfoData.body.data.info;
                //test source
                //TemplInfo_TextBlock.Text = pWebData.m_pTempInfoData.body.data.info.ToString() + pWebData.m_pTempInfoData.body.data.info.ToString() + pWebData.m_pTempInfoData.body.data.info.ToString();
            }
        }

        //左上角双击银行商标
        int nCount = 0;
        private void SettingBorder_DoubleClick_MouseDown(object sender, MouseButtonEventArgs e)
        {
            nCount += 1;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) => { timer.IsEnabled = false; nCount = 0; };
            timer.IsEnabled = true;

            if (nCount % 2 == 0)
            {
                timer.IsEnabled = false;
                nCount = 0;
                NavigationService.Navigate(new Uri(@"View\Page21\LoginPassord.xaml", UriKind.Relative));
            }
        }
        //法师下一页预览
        private void DownPage_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListViewAutomationPeer lvap = new ListViewAutomationPeer(MonkInfo_ListView);
                var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
                var scroll = svap.Owner as ScrollViewer;
                double nListViewOffset = scroll.ViewportWidth;
                if ((m_MonkList.Count > 1) && (scroll.HorizontalOffset / nListViewOffset) <= (m_MonkList.Count - 2))
                {
                    if ((scroll.HorizontalOffset / nListViewOffset) == (m_MonkList.Count - 2))
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page21/btn03.png"));
                        this.DownPage_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page21/btn02.png"));
                    this.UpPage_Button.Content = img2;
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + nListViewOffset);
                }
            }
            catch (Exception ex) { }
        }

        //法师上一页预览
        private void UpPage_Button_Click_1(object sender, RoutedEventArgs e)
        {
            try{
                ListViewAutomationPeer lvap = new ListViewAutomationPeer(MonkInfo_ListView);
                var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
                var scroll = svap.Owner as ScrollViewer;
                double nListViewOffset = scroll.ViewportWidth;
                if ((m_MonkList.Count > 1) && (scroll.HorizontalOffset / nListViewOffset) >= 0)
                {
                    if ((scroll.HorizontalOffset < nListViewOffset) || 
                        (scroll.HorizontalOffset / nListViewOffset) == 1)
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn01.png"));
                        this.UpPage_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn04.png"));
                    this.DownPage_Button.Content = img2;
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - nListViewOffset);
                }
             }
            catch (Exception ex){ }
        }

        private void Activity_Detail_Click(object sender, RoutedEventArgs e)
        {
            string strDetail = @"";
            if (pWebData != null)
            {
                ListViewAutomationPeer lvap = new ListViewAutomationPeer(ActivityInfo_ListView);
                var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
                var scroll = svap.Owner as ScrollViewer;
                int nSelIndex = (int)scroll.ContentHorizontalOffset / 922;

                int n = 0;
                foreach (ActivityList temp in m_pActivityListInfo)
                {
                    if (nSelIndex == n)
                    {
                        strDetail = temp.ActivityMainDetail;
                        break;
                    }
                    n++;
                }
            }

            Introduction IntroductionWin = new Introduction(strDetail, 2);
            IntroductionWin.ShowDialog();

        }

        private void TemplInfo_Detail_Click(object sender, RoutedEventArgs e)
        {
            string strDetail = @"";
            if (pWebData != null &&
                pWebData.m_pTempInfoData != null &&
                pWebData.m_pTempInfoData.body != null &&
                pWebData.m_pTempInfoData.body.data != null &&
                pWebData.m_pTempInfoData.body.data.detail != null)
            {
                strDetail = pWebData.m_pTempInfoData.body.data.detail;
            }

            Introduction IntroductionWin = new Introduction(strDetail, 1);
            IntroductionWin.ShowDialog();
        }

        private void ActivityInfo_Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (m_pActivityListInfo.Count == 0)
                {
                    return;
                }
                ListViewAutomationPeer lvap = new ListViewAutomationPeer(ActivityInfo_ListView);
                var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
                var scroll = svap.Owner as ScrollViewer;
                double nListViewOffset = scroll.ViewportWidth;
                if ((m_MonkList.Count > 1) && (scroll.HorizontalOffset / nListViewOffset) >= 0)
                {
                    if ((scroll.HorizontalOffset < nListViewOffset) ||
                        (scroll.HorizontalOffset / nListViewOffset) == 1)
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn01.png"));
                        this.ActivityInfo_Prev_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page21/btn04.png"));
                    this.ActivityInfo_Next_Button.Content = img2;
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset - nListViewOffset);
                }

            }
            catch (Exception ex) { }
        }

        private void ActivityInfo_Next_Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (m_pActivityListInfo.Count == 0)
                {
                    return;
                }

                ListViewAutomationPeer lvap = new ListViewAutomationPeer(ActivityInfo_ListView);
                var svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
                var scroll = svap.Owner as ScrollViewer;
                double nListViewOffset = scroll.ViewportWidth;
                if ((m_MonkList.Count > 1) && (scroll.HorizontalOffset / nListViewOffset) <= (m_MonkList.Count - 2))
                {
                    if ((scroll.HorizontalOffset / nListViewOffset) == (m_MonkList.Count - 2))
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page21/btn03.png"));
                        this.ActivityInfo_Next_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page21/btn02.png"));
                    this.ActivityInfo_Prev_Button.Content = img2;
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + nListViewOffset);
                }
            }
            catch (Exception ex) { }
        }

        private void ActivityInfo_Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ActivityInfo_Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            string strDetail = strActivityInfoDetail;

            if (strDetail != null && strDetail.Length > 0)
            {
                Introduction IntroductionWin = new Introduction(strDetail, 2);
                IntroductionWin.ShowDialog();
            }
        }


        private void MonkInfo_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try 
            {
                monkinfoDisp emp = MonkInfo_ListView.SelectedItem as monkinfoDisp;
                if (emp != null)
                {
                    if (m_MonkinfoDetail.ContainsKey(emp.MonkInfoIndex))
                    {
                        string strDetail = m_MonkinfoDetail[emp.MonkInfoIndex];
                        Introduction IntroductionWin = new Introduction(strDetail, 3);
                        //IntroductionWin.Owner = ParentWindow;
                        IntroductionWin.ShowDialog();
                    }
                    MonkInfo_ListView.UnselectAll();
                }
            }
            catch (Exception ex) { }

        }

        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            int n = 10;

        }


    }
}
