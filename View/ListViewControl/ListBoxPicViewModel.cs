using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading;
using System.Configuration;
using System.Windows;
using System.Windows.Media;

namespace RenJiCaoZuo.View.ListViewControl
{
    //------------------------------------------------------Model ------------------------------------------------------
    /// <summary>
    /// Modle data process
    /// </summary>
    public class PicPathList : INotifyPropertyChanged
    {
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

   
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    //------------------------------------------------------Model End-----------------------------------------------------

    //------------------------------------------------------ViewModel Begin-----------------------------------------------

    public class ListBoxPicViewModel : INotifyPropertyChanged
    {
        // data process
        private ObservableCollection<PicPathList> _Pathlist;

        public ObservableCollection<PicPathList> PathLists
        {
            get { return _Pathlist; }
            set
            {
                _Pathlist = value;
                OnPropertyChanged("PathLists");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private DispatcherTimer dispatcherTimerList = null;

        public void getTimer()
        {
            string sRefreshTime = ConfigurationManager.AppSettings["Pic_ChangTime"];
            int nRefreshTime = Convert.ToInt16(sRefreshTime);
            dispatcherTimerList = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(nRefreshTime) };
        }

        //view modle main process
        private delegate void updateDelegate();
        private void RefreshList(object state)
        {
            while (true)
            {
                int nRefreshTime = (int)state;
                System.Threading.Thread.Sleep(nRefreshTime*1000);
                App.Current.Dispatcher.BeginInvoke(new updateDelegate(DataExchange));
            }
        }

        private void DataExchange()
        {
            for (int i = 1; i < (PathLists.Count); i++)
            {
                PathLists.Move(i - 1, i);
            }
        }

        public ListBoxPicViewModel()
        {
            PathLists = new ObservableCollection<PicPathList>();
             
            RenJiCaoZuo.PicTransationPath PicTransation = new PicTransationPath();
            if (PicTransation.lstPicPath.Count <= 0)
            {
                return;
            }

            foreach (string strTemp in PicTransation.lstPicPath)
            {
                PicPathList PicTempInfo = new PicPathList();
                PicTempInfo.FilePath = strTemp;
                PathLists.Add(PicTempInfo);
            }
            string sRefreshTime = ConfigurationManager.AppSettings["Pic_ChangTime"];
            int nRefreshTime = Convert.ToInt16(sRefreshTime);
            //Thread thread = new Thread(new ThreadStart(RefreshList));
            //thread.Start();  

            ThreadPool.QueueUserWorkItem(new WaitCallback(RefreshList), nRefreshTime);
        }
    }
}
