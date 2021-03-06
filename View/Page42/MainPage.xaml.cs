﻿using System;
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
using RenJiCaoZuo.Common;
using System.ComponentModel;

namespace RenJiCaoZuo.View.Page42
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
        //activity 的label更新的timer
        private DispatcherTimer dispatcherTimerList = new System.Windows.Threading.DispatcherTimer() 
                        { Interval = TimeSpan.FromSeconds(5) };
        private DispatcherTimer dispatcherDonateHouseTimer = null;
        private DispatcherTimer dispatcherTimerRefresh = null;

        private DispatcherTimer dispatcherAllPageRefreshTimer = new System.Windows.Threading.DispatcherTimer()
                        { Interval = TimeSpan.FromSeconds(5) };


        public GetWebData pWebData;
        public Uri ImageFilePathUri;
        private string Activity_MonkImage;
        private string Media_play_Path;
        Queue<ActivityList> myActivityInfoQueue = new Queue<ActivityList>();
        Queue<PayListHistory> myPayQueue = new Queue<PayListHistory>();
        //显示法师ListView内容
        List<monkinfoDisp> m_MonkList = new List<monkinfoDisp>();
        private List<string> m_MonkinfoDetail = new List<string>();

        private string m_strMonkinfoDetail;
        private string m_strActivityinfoDetail;
        //显示活动横向list内容
        List<ActivityList> m_pActivityListInfo = new List<ActivityList>();
        string strMode = ConfigurationManager.AppSettings["FirstPageName"];
        private int m_nRefreshTimeOutCount;

        Queue<PicTransationControl> myPicTransationPageQueue = new Queue<PicTransationControl>();
        PicTransationControl currentPage = new PicTransationControl();

        private void getPageRefreshTime()
        {
            string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
            int nRefreshTime = Convert.ToInt16(sRefreshTime);

            dispatcherTimerRefresh = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(nRefreshTime) };

            string sDonateHouseTime = ConfigurationManager.AppSettings["DonateHouseRefreshTime"];
            int nDonateHouseTime = Convert.ToInt16(sDonateHouseTime);
            dispatcherDonateHouseTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(nDonateHouseTime) };
        }

        //add the picture change 
        #region picture change
        int m_nChangeTime;
        public void setPicTransactionPage()
        {
            PicTransationPath PicTransation = new PicTransationPath();
            if (PicTransation.lstPicPath.Count <= 0)
            {
                return;
            }

            foreach (string strTemp in PicTransation.lstPicPath)
            {
                PicTransationControl newPage = new PicTransationControl();
                PicInfo PicTempInfo = new PicInfo();
                PicTempInfo.nHeight = 780;
                PicTempInfo.nWidth = 1038;
                PicTempInfo.strImgLink = strTemp;
                newPage.DataContext = PicTempInfo; 
                myPicTransationPageQueue.Enqueue(newPage);
                currentPage = newPage;
            }

            //pageTransitionControl.ShowPage(currentPage);
            getThePicChangeTime();
            creatThePicChangeThread();
        }

        void getThePicChangeTime()
        {
            m_nChangeTime = 0;
            string strPage_Change_time = ConfigurationManager.AppSettings["Pic_ChangTime"];
            m_nChangeTime = Convert.ToInt16(strPage_Change_time);
            m_nChangeTime = m_nChangeTime * 1000;
        }

        void creatThePicChangeThread()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (myPicTransationPageQueue.Count > 0)
                {
                    System.Threading.Thread.Sleep(m_nChangeTime); 
                    currentPage = myPicTransationPageQueue.Dequeue();
                    myPicTransationPageQueue.Enqueue(currentPage);
                    //pageTransitionControl.ShowPage(currentPage);
                }
                else
                {
                    break;
                }
            }
        }
        //End the picture change 
        #endregion picture change
        public MainPage()
        {
            m_nRefreshTimeOutCount = 0;
            pWebData =  MainWindow.m_pAllWebData;
            string strDisplayInch = ConfigurationManager.AppSettings["DisplayInch"];

            InitializeComponent();
            getPageRefreshTime();
            setDisplayByMode(strMode);

            if (strMode == "3")
            {
                try
                {
                    Media_play_Path = ConfigurationManager.AppSettings["Video_Path"];
                    string Media_play_Mode = ConfigurationManager.AppSettings["Video_Play_Mode"];
                    if (File.Exists(Media_play_Path))
                    {
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
                        this.MediaPlay.Close();
                        this.MediaPlay.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {

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

                if (strDisplayInch != "42_2" && strDisplayInch != "42_3")
                {
                    if (pWebData.m_pqRCodeInfoData.success == true)
                    {
                        //设定在线功德二维码
                        setQRCodePic_Zxgdx();
                        //设定公众号二维码
                        setQRCodePic_Gzgzh();
                    }
                }

                if (pWebData.m_pHousePayHistoryData.success == true)
                {
                    getDonateHouseContent();
                }
            }
            //显示捐赠人内容
            displayDonateHouse();
            displayDonateList();
            if (strDisplayInch != "42_2" && strDisplayInch != "42_3")
            {
                Page_All_Refresh();
            }

            if (strDisplayInch == "42_3")
            {
                //setPicTransactionPage();
            }

        }

        private void Page_All_Refresh()
        {
            try
            {
                dispatcherAllPageRefreshTimer.Tick += delegate
                {
                    if (m_nRefreshTimeOutCount > 10)
                    {
                        dispatcherAllPageRefreshTimer.Stop();
                    }
                    m_nRefreshTimeOutCount++;
                    if (TemplInfo_TextBlock.Text.Length == 0 && strMode != "3")
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

                        //pWebData.GetTemplePayHistorybyWebService();
                        //getDonateHouseContent();
                        //显示捐赠House内容
                        displayDonateHouse();
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
                        pWebData.GetTempleInfobyWebService();
                        //设定二维码
                        setQRCodePic_Gzgzh();
                    }

                    if (QRCode_Image_Zxgdx.Source != null &&
                        QRCode_Image_Gzgzh.Source != null &&
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
                Activity_MonkImage = "pack://SiteOfOrigin:,,,/Res/Page42/Dzjsbt.png";
                Uri ImageFilePathUri = new Uri(Activity_MonkImage);
                this.MonkOrActive_Intrduce_Pic.Source = new BitmapImage(ImageFilePathUri);
            }
            else if (strMode == "2")
            {
                Activity_MonkImage = "pack://SiteOfOrigin:,,,/Res/Page42/hdxx.png";
                Uri ImageFilePathUri = new Uri(Activity_MonkImage);
                this.MonkOrActive_Intrduce_Pic.Source = new BitmapImage(ImageFilePathUri);
            }
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

                ListBoxPicChange.Visibility = Visibility.Hidden;
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
                ListBoxPicChange.Visibility = Visibility.Hidden;
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
 //               TempInfo_Image.Visibility = Visibility.Hidden;
                ActivityAndMonk_Img.Visibility = Visibility.Hidden;
                TempInfo_Detail.Visibility = Visibility.Hidden;
                TempInfo_Intrduce.Visibility = Visibility.Hidden;
                Temple_Intrduce_Frame.Visibility = Visibility.Hidden;
                TempInfo_Intrduce_Pic.Visibility = Visibility.Hidden;
                this.MonkOrActive_Intrduce_Pic.Visibility = Visibility.Hidden;
                Seprate_Line1.Visibility = Visibility.Hidden;
                Seprate_Line2.Visibility = Visibility.Hidden;
                Ggjs_Page_Flow.Visibility = Visibility.Hidden;
                Temple_Intrduce_Back.Visibility = Visibility.Hidden;
                Temple_Intrduce_Frame_Back.Visibility = Visibility.Hidden;
                ListBoxPicChange.Visibility = Visibility.Hidden;
                
                this.MediaPlay.Visibility = Visibility.Visible;
            }

            string strDisplayInch = ConfigurationManager.AppSettings["DisplayInch"];
            if (strDisplayInch == "42_2")
            {
                this.Frame_Backgound_Zxgdx.Visibility = Visibility.Hidden;
                this.Frame_Pic_Zxgdx.Visibility = Visibility.Hidden;
                this.QRCode_Title_Zxgdx.Visibility = Visibility.Hidden;
                this.QRCode_Image_Zxgdx.Visibility = Visibility.Hidden;
                this.Temple_Name_Title.Visibility = Visibility.Hidden;
                this.Zxgd_Prompt_Text.Visibility = Visibility.Hidden;
                this.Gzgzh_Frame_Backgound.Visibility = Visibility.Hidden;
                this.Gzgzh_Frame_Pic.Visibility = Visibility.Hidden;
                this.Gzwm_Prompt_Text.Visibility = Visibility.Hidden;
                this.QRCode_Title_Gzgzh.Visibility = Visibility.Hidden;
                this.Temple_Name_Title_Copy.Visibility = Visibility.Hidden;
                this.QRCode_Image_Gzgzh.Visibility = Visibility.Hidden;
                this.Seprate_Line_Gzgzh.Visibility = Visibility.Hidden;
                this.Seprate_Line_Zxgdx.Visibility = Visibility.Hidden;

                ListBoxPicChange.Visibility = Visibility.Hidden;

            }

            if (strDisplayInch == "42_3")
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
                //               TempInfo_Image.Visibility = Visibility.Hidden;
                ActivityAndMonk_Img.Visibility = Visibility.Hidden;
                TempInfo_Detail.Visibility = Visibility.Hidden;
                TempInfo_Intrduce.Visibility = Visibility.Hidden;
                Temple_Intrduce_Frame.Visibility = Visibility.Hidden;
                TempInfo_Intrduce_Pic.Visibility = Visibility.Hidden;
                this.MonkOrActive_Intrduce_Pic.Visibility = Visibility.Hidden;
                Seprate_Line1.Visibility = Visibility.Hidden;
                Seprate_Line2.Visibility = Visibility.Hidden;
                Ggjs_Page_Flow.Visibility = Visibility.Hidden;
                Temple_Intrduce_Back.Visibility = Visibility.Hidden;
                Temple_Intrduce_Frame_Back.Visibility = Visibility.Hidden;

                this.Frame_Backgound_Zxgdx.Visibility = Visibility.Hidden;
                this.Frame_Pic_Zxgdx.Visibility = Visibility.Hidden;
                this.QRCode_Title_Zxgdx.Visibility = Visibility.Hidden;
                this.QRCode_Image_Zxgdx.Visibility = Visibility.Hidden;
                this.Temple_Name_Title.Visibility = Visibility.Hidden;
                this.Zxgd_Prompt_Text.Visibility = Visibility.Hidden;
                this.Gzgzh_Frame_Backgound.Visibility = Visibility.Hidden;
                this.Gzgzh_Frame_Pic.Visibility = Visibility.Hidden;
                this.Gzwm_Prompt_Text.Visibility = Visibility.Hidden;
                this.QRCode_Title_Gzgzh.Visibility = Visibility.Hidden;
                this.Temple_Name_Title_Copy.Visibility = Visibility.Hidden;
                this.QRCode_Image_Gzgzh.Visibility = Visibility.Hidden;
                this.Seprate_Line_Gzgzh.Visibility = Visibility.Hidden;
                this.Seprate_Line_Zxgdx.Visibility = Visibility.Hidden;
                this.MediaPlay.Visibility = Visibility.Hidden;

                ListBoxPicChange.Visibility = Visibility.Visible;
            }
            string strDonateDisplayMode = ConfigurationManager.AppSettings["DonateListDisplayMode"];
            if (strDonateDisplayMode == "3")
            {
                Donate_Title_Pic.Visibility = Visibility.Hidden;
                Donate_List_Frame.Visibility = Visibility.Hidden;
                DonateListHost.Visibility = Visibility.Hidden;
            }
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            //设置一下视频进度，确保从头开始播放
            MediaPlay.Position = TimeSpan.Zero;
            MediaPlay.Play();
        }

        //获取捐赠TextBox的内容
        private void getDonateHouseContent()
        {
            HouseName.Text = "";
            HousepayTypeName.Text = "";
            Houseamount.Text = "";
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

        public int mDonate_nCount = 0;

        //显示捐赠DonateList内容
        private void displayDonateList()
        {
            if (DonateInfo_List.Items.Count >= 24)
            {
                DonateInfo_List.ScrollIntoView(DonateInfo_List.Items[24]);
            }
            
        }

        //显示捐赠DonateHouse内容
        private void displayDonateHouse()
        {
            
        }

        private void getActiveInfoContent()
        {
            try
            {
                this.ActivityInfo_ListView.ItemsSource = m_pActivityListInfo.ToList();
                if (pWebData == null ||
                    pWebData.m_pActivityInfoData == null || 
                    pWebData.m_pActivityInfoData.body == null || 
                    pWebData.m_pActivityInfoData.body.data == null)
                {
                    return;
                }

                int nIndex = 0;
                foreach (ActivityInfoDatabody ActivityInfTemp in pWebData.m_pActivityInfoData.body.data)
                {
                    if (nIndex == 0)
                    {
                        m_strActivityinfoDetail = ActivityInfTemp.detail;
                    }
                    nIndex++;
                    ActivityList pTemp = new ActivityList();
                    pTemp.ActivityMain = ActivityInfTemp.activity;
                    pTemp.ActivityMainDetail = ActivityInfTemp.detail;
                    m_pActivityListInfo.Add(pTemp);
                    myActivityInfoQueue.Enqueue(pTemp);
                }

                if (m_pActivityListInfo.Count == 0)
                {
                    return;
                }
                //this.ActivityInfo_ListView.ItemsSource = null;
                //this.ActivityInfo_ListView.ItemsSource = m_pActivityListInfo.ToList();

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
                    if (strMode == "1")
                    {
                        this.NewsBackground_Img.Visibility = Visibility.Visible;
                        this.ActivityInfo_Label.Visibility = Visibility.Visible;
                    }
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
                else
                {
                    this.NewsBackground_Img.Visibility = Visibility.Hidden;
                    this.ActivityInfo_Label.Visibility = Visibility.Hidden;
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
                img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn01.png"));
                this.ActivityInfo_Prev_Button.Content = img;

                if (m_pActivityListInfo.Count <= 1)
                {
                    Image imgDown = new Image();
                    imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn03.png"));
                    this.ActivityInfo_Next_Button.Content = imgDown;
                }
                else
                {
                    Image imgDown = new Image();
                    imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn04.png"));
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
                    if(nIndex == 0)
                    {
                        //add the display monk information detail
                        m_strMonkinfoDetail = MonkTemp.detail;
                    }
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

                    m_MonkinfoDetail.Add(MonkTemp.detail);
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
                //        temp.MonkName = nIndex.ToString() + MonkTemp.name;
                //    }

                //    temp.MonkInfoIndex = nIndex;
                //    m_MonkList.Add(temp);

                //    MonkTemp.detail = nIndex.ToString() + MonkTemp.detail;
                //    m_MonkinfoDetail.Add(MonkTemp.detail);
                //}
                ////Test Source
    
            }
            catch (Exception ex){ }

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn01.png"));
            this.UpPage_Button.Content = img;

            if (m_MonkList.Count <= 1)
            {
                Image imgDown = new Image();
                imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn03.png"));
                this.DownPage_Button.Content = imgDown;
            }
            else
            {
                Image imgDown = new Image();
                imgDown.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn04.png"));
                this.DownPage_Button.Content = imgDown;
            }


            this.MonkInfo_ListView.ItemsSource = m_MonkList.ToList();
        }


        //获取在线功德二维码
        private void setQRCodePic_Zxgdx()
        {
            try
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
            catch (Exception ex)
            {

            }
        }

        //获取关注公众号二维码
        private void setQRCodePic_Gzgzh()
        {
            try
            {
                if (pWebData != null &&
                pWebData.m_pTempInfoData != null &&
                pWebData.m_pTempInfoData.body != null &&
                pWebData.m_pTempInfoData.body.data != null &&
                pWebData.m_pTempInfoData.body.data.wcqr != null)
                {
                    if (pWebData.m_pTempInfoData.body.data.wcqr.Length > 0)
                    {
                        Uri ImageFilePathUri = new Uri(pWebData.m_pTempInfoData.body.data.wcqr);
                        QRCode_Image_Gzgzh.Source = new BitmapImage(ImageFilePathUri);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        //获取寺庙名字的图片
        private void setTemplInfoNamePic()
        {
            if (pWebData != null &&
                pWebData.m_pTempInfoData != null &&
                pWebData.m_pTempInfoData.body != null &&
                pWebData.m_pTempInfoData.body.data != null &&
                pWebData.m_pTempInfoData.body.data.url != null &&
                pWebData.m_pTempInfoData.body.data.url.Length > 0)
            {
                Uri ImageFilePathUri = new Uri(pWebData.m_pTempInfoData.body.data.url);
                TempInfo_Image.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(ImageFilePathUri)
                };
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
                Window currentWin = Application.Current.MainWindow;
                MainWindow mgen = currentWin as MainWindow;
                mgen.LoginPassord.IsSelected = true;
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
                    int nPosOf = (int)(scroll.HorizontalOffset / nListViewOffset);
                    if (nPosOf == (m_MonkList.Count - 2))
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page42/btn03.png"));
                        this.DownPage_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page42/btn02.png"));
                    this.UpPage_Button.Content = img2;

                    if (nPosOf >=0 && (nPosOf) < m_MonkinfoDetail.Count())
                    {
                        m_strMonkinfoDetail = m_MonkinfoDetail[nPosOf+1];
                    }
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
                    int nPosOf = (int)(scroll.HorizontalOffset / nListViewOffset);
                    if ((scroll.HorizontalOffset < nListViewOffset) || 
                        (scroll.HorizontalOffset / nListViewOffset) == 1)
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn01.png"));
                        this.UpPage_Button.Content = img;
                    }

                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn04.png"));
                    this.DownPage_Button.Content = img2;

                    if (nPosOf >= 0 && (nPosOf - 1) >= 0)
                    {
                        m_strMonkinfoDetail = m_MonkinfoDetail[nPosOf - 1];
                    }

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

                int nSelIndex = (int)scroll.ContentHorizontalOffset /(int) scroll.ViewportWidth;

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
                if ((m_pActivityListInfo.Count > 1) && (scroll.HorizontalOffset / nListViewOffset) >= 0)
                {
                    int nPosOf = (int)(scroll.HorizontalOffset / nListViewOffset);
                    if ((scroll.HorizontalOffset < nListViewOffset) ||
                        (scroll.HorizontalOffset / nListViewOffset) == 1)
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn01.png"));
                        this.ActivityInfo_Prev_Button.Content = img;
                    }

                    if (nPosOf >= 0 && (nPosOf - 1) >= 0)
                    {
                        m_strActivityinfoDetail = m_pActivityListInfo[nPosOf - 1].ActivityMainDetail;
                    }
                    
                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri(@"pack://SiteOfOrigin:,,,/Res/Page42/btn04.png"));
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
                if ((m_pActivityListInfo.Count > 1) &&
                    (scroll.HorizontalOffset / nListViewOffset) <= (m_pActivityListInfo.Count - 2))
                {
                    int nPosOf = (int)(scroll.HorizontalOffset / nListViewOffset);
                    if ((scroll.HorizontalOffset / nListViewOffset) == (m_pActivityListInfo.Count - 2))
                    {
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page42/btn03.png"));
                        this.ActivityInfo_Next_Button.Content = img;
                    }

                    if (nPosOf >= 0 && (nPosOf) < m_pActivityListInfo.Count())
                    {
                        m_strActivityinfoDetail = m_pActivityListInfo[nPosOf + 1].ActivityMainDetail;
                    }
                    Image img2 = new Image();
                    img2.Source = new BitmapImage(new Uri("pack://SiteOfOrigin:,,,/Res/Page42/btn02.png"));
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
                //monkinfoDisp emp = MonkInfo_ListView.SelectedItem as monkinfoDisp;
                //if (emp != null)
                //{
                //    if (m_MonkinfoDetail.ContainsKey(emp.MonkInfoIndex))
                //    {
                //        string strDetail = m_MonkinfoDetail[emp.MonkInfoIndex];
                //        Introduction IntroductionWin = new Introduction(strDetail, 3);
                //        IntroductionWin.ShowDialog();
                //    }
                //    MonkInfo_ListView.UnselectAll();
                //}
            }
            catch (Exception ex) { }

        }

        private void Image_Unloaded(object sender, RoutedEventArgs e)
        {
            int n = 10;

        }

        private void Ggjs_Page_Flow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (strMode == "1")
                {
                    Introduction IntroductionWin = new Introduction(m_strMonkinfoDetail, 3);
                    IntroductionWin.ShowDialog();
                }
                else if (strMode == "2")
                {
                    Introduction IntroductionWin = new Introduction(m_strActivityinfoDetail, 2);
                    IntroductionWin.ShowDialog();
                }
            }
            catch (Exception ex) { }
        }
    }
}
