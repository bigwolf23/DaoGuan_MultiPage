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

namespace RenJiCaoZuo.View.DonateList
{
    public class DonateHouse : INotifyPropertyChanged
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
}
