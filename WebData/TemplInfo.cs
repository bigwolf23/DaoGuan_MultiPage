using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
//     public class DispData
//     {
//         List<TemplePayHistory> lstTemplePay;
//         List<HousePayHistory> lstHostPay;
//     }

    public class TempleInfo
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string msg { get; set; }
        public TempleInfobody body { get; set; }
    }

    public class TempleInfobody
    {
        public TempleInfoData data { get; set; }
    }

    public class TempleInfoData
    {
        public string id { get; set; }
        public Createby createby { get; set; }
        public string createDate { get; set; }
        public UpdateBy updateBy { get; set; }
        public string updateDate { get; set; }

        public Owner owner { get; set; }
        
        public string info { get; set; }
        public string url { get; set; }
        public string detail { get; set; }
    }

 

}
