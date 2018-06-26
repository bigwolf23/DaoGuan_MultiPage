using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
    public class TemplePayHistory
    {
        public bool success { get; set; }
        public int errorCode { get; set; }
        public string msg { get; set; }
        public TemplePayHistorybody body { get; set; }

    }

    public class TemplePayHistorybody
    {
        public List<TemplePayHistoryDatabody> data { get; set; }
    }

    public class TemplePayHistoryDatabody
    {
        public string id;
        public string createDate;
        public string updateDate;
        InfoOwnerbody temple;
        InfoOwnerbody house;
        public string name;
        public string tel;
        public string payType;
        public string payTypeName;
        public double amount;
        public string tradeDtorderId;
    }
}
