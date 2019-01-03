using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenJiCaoZuo.Common
{
    public class PicInfo
    {
        public string strImgLink { set; get; }
        public int nWidth { set; get; }
        public int nHeight { set; get; }
    }

    public class PayListHistory
    {
        public string Name { get; set; }
        public string payTypeName { get; set; }
        public double amount { get; set; }
    }

    public class monkinfoDisp
    {
        public string MonkInfoImage { get; set; }
        public string MonkName { get; set; }
        public string MonkInfo { get; set; }
        public int MonkInfoIndex { get; set; }
    }

    //获取寺庙活动的内容
    public class ActivityList
    {
        public string ActivityMain { get; set; }
        public string ActivityMainDetail { get; set; }
    }
}
