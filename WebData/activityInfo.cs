using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
    public class ActivityInfo
    {
        public bool success { get; set; }
        public int errorCode { get; set; }
        public string msg { get; set; }
        public ActivityInfobody body { get; set; }

    }

    public class ActivityInfobody
    {
        public List<ActivityInfoDatabody> data { get; set; }
    }

    public class ActivityInfoDatabody
    {
        public string id { get; set; }
        public Createby createby { get; set; }
        public string createDate { get; set; }
        public UpdateBy updateBy { get; set; }
        public string updateDate { get; set; }

        public Owner owner { get; set; }

        public string dt { get; set; }
        public string activity { get; set; }
        public string display { get; set; }
        public string detail { get; set; }

    }
}
