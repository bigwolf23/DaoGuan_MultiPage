using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
    public class MonkInfo
    {
        public bool success;
        public int errorCode;
        public string msg;
        public MonkInfobody body;

    }

    public class MonkInfobody
    {
        public List<MonkInfoDatabody> data;
    }

    public class MonkInfoDatabody
    {
        public string id { get; set; }
        public Createby createby { get; set; }
        public string createDate { get; set; }
        public UpdateBy updateBy { get; set; }
        public string updateDate { get; set; }

        public Owner owner { get; set; }

        public string name { get; set; }
        public string info { get; set; }
        public string url { get; set; }
        public string paixu { get; set; }
        public string detail { get; set; }
    }
}
