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

        //public ICommand GetPayListHistory
        //{
        //    get { return _PaylistHistory; }
        //    set
        //    {
        //        _PaylistHistory = value;
        //    }
        //}

        //public void ChangeFileName(object obj)
        //{
        //    Files[0].FileName = "File_" + new Random().Next().ToString(CultureInfo.InvariantCulture);
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private DispatcherTimer dispatcherTimerList =
            new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
        public GetWebData pWebData;
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int n = 0; n <= 100; n++)
            {
                for (int i = 1; i < (PayListHistorys.Count); i++)
                {
                    PayListHistorys.Move(i - 1, i);
                }
                Thread.Sleep(2000);
            }
        }

        private bool _isWorking = false;
        public bool isWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; }
        }

        void ThreadProcess(object sender)
        {
            while (true)
            {
                //for (int i = 1; i < (PayListHistorys.Count); i++)
                //{
                //    PayListHistorys.Move(i - 1, i);
                //}
                myPayQueue.Enqueue(myPayQueue.Dequeue());
                PayListHistorys.Clear();
                foreach (PayListHistory pTemp in myPayQueue)
                {
                    PayListHistorys.Add(pTemp);
                }
                Thread.Sleep(2000);
            }
        }

        private void moveData()
        {
            while (true)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    System.Threading.SynchronizationContext.SetSynchronizationContext(new
                        System.Windows.Threading.DispatcherSynchronizationContext(
                        System.Windows.Application.Current.Dispatcher));
                    System.Threading.SynchronizationContext.Current.Send(pl =>
                    {

                        for (int i = 1; i < (PayListHistorys.Count); i++)
                        {
                            PayListHistorys.Move(i - 1, i);
                        }

                    }, null);
                });
                Thread.Sleep(2000);
            }
        }

        
        public DonateListViewModel()
        {
            PayListHistorys = new ObservableCollection<PayListHistory>();
            pWebData = MainWindow.m_pAllWebData;
            getDonateListContent();
            try
            {
                int nCount = 0;
                //dispatcherTimerList.Tick += IndexAction;
                //dispatcherTimerList.Start();

                //Thread mThread = new Thread(moveData);
                //mThread.Name = "线程测试";
                //mThread.Start();
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                //worker.RunWorkerAsync();
                //var dispatcher = App.Current.MainWindow.Dispatcher;
                //Task.Factory.StartNew(() =>
                //{
                //    nCount = 0;
                //    System.Threading.Thread.Sleep(1000);
                //    while (nCount < 1000)
                //    {
                //        nCount += 1;
                //        System.Threading.Thread.Sleep(2000);
                //        //tmpArgs._message = $"the program precent is {ShowValue.ToString()}";
                //        dispatcher.BeginInvoke((Action)delegate () {
                //            for (int i = 1; i < (PayListHistorys.Count); i++)
                //            {
                //                PayListHistorys.Move(i - 1, i);
                //            }
                //        });
                //    }
                //    dispatcher.Invoke((Action)delegate () { 
                //        for (int i = 1; i < (PayListHistorys.Count); i++)
                //        {
                //            PayListHistorys.Move(i - 1, i);
                //        }
                //    });
                //});


                //new Thread(()=>{
                //    Application.Current.Dispatcher.Invoke(new Action(() =>
                //    {
                //        //while (true) {
                //        //    for (int i = 1; i < (PayListHistorys.Count); i++)
                //        //    {
                //        //        PayListHistorys.Move(i - 1, i);
                //        //    }
                //        //    Thread.Sleep(2000);
                //        //}
                //        dispatcherTimerList.Tick += IndexAction;
                //        dispatcherTimerList.Start();
                //    }));
                //}).Start();

                dispatcherTimerList.Tick += delegate
                {
                    nCount++;
                    string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
                    int nRefreshTime = Convert.ToInt16(sRefreshTime);
                    
                    if (nCount*2 > nRefreshTime)
                    {
                        nCount = 0;
                        PayListHistorys.Clear();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
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
                            for (int i = 1; i < (PayListHistorys.Count); i++)
                            {
                                PayListHistorys.Move(i - 1, i);
                            }
                            //获取捐赠TextBox的内容
                            //getDonateHouseContent();
                        }

                    }
                    else
                    {
                        for (int i = 1; i < (PayListHistorys.Count); i++)
                        {
                            PayListHistorys.Move(i - 1, i);
                        }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                };
                dispatcherTimerList.Start();
            }
            catch (Exception ex) { }
        }

        private int mDonate_nCount = 0;
        private void IndexAction(object sender, EventArgs e)
        {
            GC.Collect();
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            System.Threading.ThreadStart start = delegate()
            {
                mDonate_nCount++;
                string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
                int nRefreshTime = Convert.ToInt16(sRefreshTime);
                //if (mDonate_nCount % nRefreshTime == 0)
                //{
                    mDonate_nCount = 0;
                    x.BeginInvoke(new Action(() =>
                    {
                        for (int i = 1; i < (PayListHistorys.Count); i++)
                        {
                            PayListHistorys.Move(i - 1, i);
                        }
                    }), DispatcherPriority.Normal);
                //}
                
            };
            new System.Threading.Thread(start).Start(); //启动线程
        }
        Queue<PayListHistory> myPayQueue = new Queue<PayListHistory>();
        //获取捐赠ListView的内容
        private void getDonateListContent()
        {
            try
            {
                int nCount = 0;
                if (pWebData != null &&
                    pWebData.m_pTemplePayHistoryData != null &&
                    pWebData.m_pTemplePayHistoryData.body != null &&
                    pWebData.m_pTemplePayHistoryData.body.data != null)
                {
                    string sRefreshTime = ConfigurationManager.AppSettings["PageRefreshTime"];
                    int nRefreshTime = Convert.ToInt16(sRefreshTime);

                    foreach (TemplePayHistoryDatabody payHistTemp in pWebData.m_pTemplePayHistoryData.body.data)
                    {
                        //如果显示的数据超过刷新时间乘以2 加冗余23个数（针对42寸的显示），就不用加载多余的数据
                        if (nCount > nRefreshTime + 30)
                        {
                            break;
                        }
                        nCount++;
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

                        PayListHistorys.Add(pTemp);
                        myPayQueue.Enqueue(pTemp);
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
