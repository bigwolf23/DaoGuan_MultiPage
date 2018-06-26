using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.WebData
{
    public class GetActivityInfoData
    {
        public int success;
        public int errorCode;
        public string msg;
        public GetActivityInfobody body;

    }

    public class GetActivityInfobody
    {
        public GetActivityInfoDatabody data;
    }

    public class GetActivityInfoDatabody
    {
        public string id;
        public bool isNewRecord;
        public string remarks;
        public string createDate;
        public string updateDate;
        InfoOwnerbody owner;
        public string dt;
        public string activity;
        public string display;
        public string detail;
    }
}
