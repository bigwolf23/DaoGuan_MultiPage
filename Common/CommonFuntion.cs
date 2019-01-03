using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Text.RegularExpressions;
using RenJiCaoZuo.WebData;
using System.Configuration;
using System.IO;

namespace RenJiCaoZuo
{
    public class CommonFuntion
    {
        public void setWindowsShutDown()
        {
            DateTime dt;
            DateTime today = DateTime.Now.Date;
            string sDate = today.Year + "/" + today.Month + "/" + today.Day;
            string strShutDownTime = ConfigurationManager.AppSettings["ShutDownTime"];
            if (Regex.IsMatch(strShutDownTime, @"\d{1,2}:\d{1,2}"))
            {
                // Successful match
            }
            else
            {
                strShutDownTime = @"23:30";
            } 
            sDate = sDate + " " + strShutDownTime + @":00";
            dt = Convert.ToDateTime(sDate);
            TimeSpan span = (TimeSpan)(dt - DateTime.Now);
            int diff = (int) span.TotalSeconds;
            string strShutdownParam = @"-s -t " + diff.ToString();
            System.Diagnostics.Process.Start(@"c:/windows/system32/shutdown.exe", strShutdownParam);
        }
    }

    public class PicTransationPath
    {
        public List<string> lstPicPath = new List<string>();
        public PicTransationPath()
        {
            string strTransation = "Pic_Transation";
            for(int i = 1;i<=5;i++)
            {
                string strTemp = strTransation + i.ToString();
                string strPath = ConfigurationManager.AppSettings[strTemp];
                if (File.Exists(strPath))
                {
                    lstPicPath.Add(strPath);
                }
            }
        }

    }
}
