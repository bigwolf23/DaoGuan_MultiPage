using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
    public class qRCodeInfo
    {
        public bool success { get; set; }
        public int errorCode { get; set; }
        public string msg { get; set; }
        public qRCodeInfobody body { get; set; }

    }

    public class qRCodeInfobody
    {
        public qRCodeInfoDatabody data { get; set; }
    }

    public class qRCodeInfoDatabody
    {
        public string id { get; set; }
        public Createby createby { get; set; }
        public string createDate { get; set; }
        public UpdateBy updateBy { get; set; }
        public string updateDate { get; set; }

        public Owner owner { get; set; }
        public Owner parent { get; set; }

        public string url { get; set; }
        public string merchantid { get; set; }
        public string posid { get; set; }
        public string bankid { get; set; }
        public string isDynamic { get; set; }
        public string pkey { get; set; }
    }
}
