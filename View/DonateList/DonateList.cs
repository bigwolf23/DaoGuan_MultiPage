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
            catch (Exception ex) { }
        }

        public DonateListViewModel()
        {
            PayListHistorys = new ObservableCollection<PayListHistory>();
            pWebData = MainWindow.m_pAllWebData;

            getDonateListContent();
            ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshList));
            string sDonateListDisplayMode = ConfigurationManager.AppSettings["DonateListDisplayMode"];

            if (sDonateListDisplayMode == @"1")
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReLoadData));
            }
        }

        #region 
        /// <summary>
        /// 获取捐赠信息
        /// </summary> 
        private async void getDonateHouseContent()
        {
            //Houseamount = "";
            //HouseName = "";
            //HousepayTypeName = "";
            await pWebData.GetHousePaybyWebService();
            if (pWebData != null &&
                pWebData.m_pHousePayHistoryData != null &&
                pWebData.m_pHousePayHistoryData.body != null &&
                pWebData.m_pHousePayHistoryData.body.data != null)
            {
                if (pWebData.m_pHousePayHistoryData.body.data.name != null)
                {
                    HouseName = pWebData.m_pHousePayHistoryData.body.data.name;
                }

                if (pWebData.m_pHousePayHistoryData.body.data.payTypeName != null)
                {
                    HousepayTypeName = pWebData.m_pHousePayHistoryData.body.data.payTypeName;
                }
                Houseamount = pWebData.m_pHousePayHistoryData.body.data.amount.ToString();
            }
        }
        /// <summary>
        /// 获取捐赠ListView的内容
        /// </summary>

        private async void getDonateListContent()
        {
            try
            {
                PayListHistorys.Clear();
                await pWebData.GetDonatlistbyWebService();
                if (pWebData != null &&
                    pWebData.m_pTemplePayHistoryData != null &&
                    pWebData.m_pTemplePayHistoryData.body != null &&
                    pWebData.m_pTemplePayHistoryData.body.data != null)
                {
                    //ObservableCollection<PayListHistory> PayListTemp = new ObservableCollection<PayListHistory>();
                    int x = 0;
                    foreach (TemplePayHistoryDatabody payHistTemp in pWebData.m_pTemplePayHistoryData.body.data)
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
                        //PayListTemp.Add(pTemp);
                        PayListHistorys.Add(pTemp);
                    }

                    //int i = 0;
                    //foreach (PayListHistory datatemp in PayListTemp)
                    //{
                    //    if (i < PayListHistorys.Count)
                    //    {
                    //        PayListHistorys[i] = datatemp;
                    //    }
                    //    else
                    //    {
                    //        PayListHistorys.Add(datatemp);
                    //    }
                    //    i++;
                    //}
                    //PayListTemp.Clear();
                }
            }
            catch (Exception ex) { }
        }
        #endregion

    }
}
