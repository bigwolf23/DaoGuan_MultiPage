using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RenJiCaoZuo;
using RenJiCaoZuo.WebData;
using System.Configuration;
using System.Threading;
using System.Windows.Input;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace RenJiCaoZuo.View.DonateList
{

    public class PayListHistory : INotifyPropertyChanged
    {
        private string _Name;
        private string _payTypeName;
        private double _amount;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string payTypeName
        {
            get { return _payTypeName; }
            set
            {
                _payTypeName = value;
                OnPropertyChanged("payTypeName");
            }
        }

        public double amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                OnPropertyChanged("amount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
 

    public class DonateListViewModel : INotifyPropertyChanged
    {
        #region 
        /// <summary>
        /// 捐赠list信息
        /// </summary> 
        private ObservableCollection<PayListHistory> _PaylistHistory;
        //private ICommand _getFiles;

        public ObservableCollection<PayListHistory> PayListHistorys
        {
            get { return _PaylistHistory; }
            set
            {
                _PaylistHistory = value;
                OnPropertyChanged("PayListHistorys");
            }
        }
        #endregion
        #region 
        /// <summary>
        /// 捐赠信息显示
        /// </summary> 
        private string _HouseName;
        public string HouseName
        {
            get { return _HouseName; }
            set
            {
                _HouseName = value;
                OnPropertyChanged("HouseName");
            }
        }

        private string _Houseamount;
        public string Houseamount
        {
            get { return _Houseamount; }
            set
            {
                _Houseamount = value;
                OnPropertyChanged("Houseamount");
            }
        }

        private string _HousepayTypeName;
        public string HousepayTypeName
        {
            get { return _HousepayTypeName; }
            set
            {
                _HousepayTypeName = value;
                OnPropertyChanged("HousepayTypeName");
            }
        }
        #endregion

        /// <summary>
        /// 捐赠信息显示
        /// </summary> 
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public GetWebData pWebData;
        int m_nCountDonateList = 0;
        int m_nCountDonateHouse = 0;
        //view modle main process
        private delegate void updateDelegate();
        private void RefreshList(object state)
        {
            while (true)
            {
                string sDonateListDisplayMode = ConfigurationManager.AppSettings["DonateListDisplayMode"];
                Thread.Sleep(1000);
                m_nCountDonateList++;
                if (sDonateListDisplayMode == @"2")
                {
                    string sListRefreshTime = ConfigurationManager.AppSettings["DonateListRefreshTime"];
                    int nListRefreshTime = Convert.ToInt16(sListRefreshTime);

                    if (m_nCountDonateList % nListRefreshTime == 0)
                    {
                        App.Current.Dispatcher.BeginInvoke(new updateDelegate(DataLoad));
                        m_nCountDonateList = 0;
                    }
                }
                else
                {
                    if (m_nCountDonateList % 2 == 0)
                    {
                        App.Current.Dispatcher.BeginInvoke(new updateDelegate(DataExchange));
                        m_nCountDonateList = 0;
                    }
                }

                string sHouseRefreshTime = ConfigurationManager.AppSettings["DonateHouseRefreshTime"];
                int nHouseRefreshTime = Convert.ToInt16(sHouseRefreshTime);
                if (m_nCountDonateHouse % nHouseRefreshTime == 0)
                {
                    App.Current.Dispatcher.BeginInvoke(new updateDelegate(getDonateHouseContent));
                    m_nCountDonateHouse = 0;
                }
                m_nCountDonateHouse++;
            }
        }

        //private void RefreshHouseData(object state)
        //{
        //    while (true)
        //    {
        //        string sHouseRefreshTime = ConfigurationManager.AppSettings["DonateHouseRefreshTime"];
        //        int nHouseRefreshTime = Convert.ToInt16(sHouseRefreshTime);
        //        if (m_nCountDonateHouse % nHouseRefreshTime == 0)
        //        {
        //            App.Current.Dispatcher.BeginInvoke(new updateDelegate(getDonateHouseContent));
        //            m_nCountDonateHouse = 0;
        //        }
        //        m_nCountDonateHouse++;
        //    }
        //}


        private void DataExchange()
        {
            for (int i = 1; i < (PayListHistorys.Count); i++)
            {
                PayListHistorys.Move(i - 1, i);
            }
        }
      
        private void ReLoadData(object state)
        {
            while (true)
            {
                string sRefreshTime = ConfigurationManager.AppSettings["DonateListRefreshTime"];
                int nRefreshTime = Convert.ToInt16(sRefreshTime);
                Thread.Sleep(nRefreshTime*1000);
                
                App.Current.Dispatcher.BeginInvoke(new updateDelegate(DataLoad));
            }
        }

        private void DataLoad()
        {
            try
            {
                string sDonateListDisplayMode = ConfigurationManager.AppSettings["DonateListDisplayMode"];

                //if (sDonateListDisplayMode == @"1")
                //{
                //    PayListHistorys.Clear();
                //}
                getDonateListContent();
            }
            catch (Exception ex) {
                string strDebug = @"Failed" + ex.Message;
                Debug.WriteLine(strDebug);
            }
        }

        public DonateListViewModel()
        {
            PayListHistorys = new ObservableCollection<PayListHistory>();
            pWebData = MainWindow.m_pAllWebData;

            getDonateListContent();
            getDonateHouseContent();
            ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshList));
            string sDonateListDisplayMode = ConfigurationManager.AppSettings["DonateListDisplayMode"];

            if (sDonateListDisplayMode == @"1")
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReLoadData));
            }

            //ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshHouseData));
        }

        #region 
        /// <summary>
        /// 获取捐赠信息
        /// </summary> 
        public HousePayHistory m_pHousePayData = new HousePayHistory();
        public async Task<string> GetHouseInfobyWebService()
        {
            return await Task.Run(() =>
            {
                if (m_pHousePayData.body != null)
                {
                    m_pHousePayData.success = false;
                    m_pHousePayData.msg = "";

                    m_pHousePayData.body.data = null;
                    m_pHousePayData.body = null;
                }
                getHouseInfoFromInterFace("housePayHistory_Interface", "housePayHistory_Param", "housePayHistory_id");
                Thread.Sleep(6000);
                return "";
            });
        }
        public void getHouseInfoFromInterFace(string Inferface_Field, string Param_Field, string Id_Field)
        {
            string strFullInterface = pWebData.getFullLink(Inferface_Field, Param_Field, Id_Field);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strFullInterface);
            request.BeginGetResponse(new AsyncCallback(_GetHouseInfo), request);
        }
        private async void _GetHouseInfo(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
                Stream stream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                {
                    string _WebInfo = await sr.ReadToEndAsync();
                    if (_WebInfo.Length > 0)
                    {
                        m_pHousePayData = JsonConvert.DeserializeObject<HousePayHistory>(_WebInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                string strDebug = @"Failed" + ex.Message;
                Debug.WriteLine(strDebug);
            }
            
        }
        private async void getDonateHouseContent()
        {
            try
            {
                //await pWebData.GetHousePaybyWebService();
                await GetHouseInfobyWebService();
                //pWebData.GetHousePayHistorybyWebService
                //HouseName = @"";
                //HousepayTypeName = @"";
                //Houseamount = @"";
                if (pWebData != null &&
                    m_pHousePayData != null &&
                    m_pHousePayData.body != null &&
                    m_pHousePayData.body.data != null)
                {
                    if (m_pHousePayData.body.data.name != null)
                    {
                        HouseName = m_pHousePayData.body.data.name;
                    }

                    if (m_pHousePayData.body.data.payTypeName != null)
                    {
                        HousepayTypeName = m_pHousePayData.body.data.payTypeName;
                    }
                    Houseamount = m_pHousePayData.body.data.amount.ToString();
                }
            }
            catch (Exception ex)
            {
                string strDebug = @"Failed" + ex.Message;
                Debug.WriteLine(strDebug);
            }
        }
        /// <summary>
        /// 获取捐赠ListView的内容
        /// </summary>
        public TemplePayHistory m_pDonatelistData = new TemplePayHistory();
        public async Task<string> GetDonatlistbyWebService()
        {
            return await Task.Run(() =>
            {
                if (m_pDonatelistData.body != null)
                {
                    m_pDonatelistData.success = false;
                    m_pDonatelistData.msg = "";
                    m_pDonatelistData.errorCode = 0;

                    m_pDonatelistData.body.data.Clear();
                    m_pDonatelistData.body = null;
                }
                
                getDonateListFromInterFace("TemplePayHistory_Interface", "Interface_Param", "Interface_id");
                Thread.Sleep(13000);
                return "";
            });
        }
        public void getDonateListFromInterFace(string Inferface_Field, string Param_Field, string Id_Field)
        {
            string strFullInterface = pWebData.getFullLink(Inferface_Field, Param_Field, Id_Field);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strFullInterface);
            request.BeginGetResponse(new AsyncCallback(_GetDonateInfo), request);
        }
        private async void _GetDonateInfo(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
                Stream stream = response.GetResponseStream();
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                {
                    string _WebInfo = await sr.ReadToEndAsync();
                    if (_WebInfo.Length > 0)
                    {
                        m_pDonatelistData = JsonConvert.DeserializeObject<TemplePayHistory>(_WebInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                string strDebug = @"Failed" + ex.Message;
                Debug.WriteLine(strDebug);
            }
            
        }
        private async void getDonateListContent()
        {
            try
            {
                //PayListHistorys.Clear();
                await GetDonatlistbyWebService();
                //pWebData.GetTemplePayHistorybyWebService();
                //pWebData.GetHousePayHistorybyWebService
                if (pWebData != null &&
                    m_pDonatelistData != null &&
                    m_pDonatelistData.body != null &&
                    m_pDonatelistData.body.data != null)
                {
                    List<PayListHistory> PayListTemp = new List<PayListHistory>();
                    foreach (TemplePayHistoryDatabody payHistTemp in m_pDonatelistData.body.data)
                    {
                        PayListHistory pTemp = new PayListHistory();
                        pTemp.amount = payHistTemp.amount;
                        if (payHistTemp.name != null)
                        {
                            pTemp.Name = payHistTemp.name;
                        }

                        if (payHistTemp.payTypeName != null)
                        {
                            pTemp.payTypeName = payHistTemp.payTypeName;
                        }
                        PayListTemp.Add(pTemp);
                        //PayListHistorys.Add(pTemp);
                    }

                    int i = 0;
                    foreach (PayListHistory datatemp in PayListTemp)
                    {
                        if (i < PayListHistorys.Count)
                        {
                            PayListHistorys[i] = datatemp;
                        }
                        else
                        {
                            PayListHistorys.Add(datatemp);
                        }
                        i++;
                    }
                    PayListTemp.Clear();
                }
            }
            catch (Exception ex) { }
        }
        #endregion

    }
}
