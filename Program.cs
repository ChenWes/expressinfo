using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace ExpressInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            var mainUrl = @"http://api.wap.guoguo-app.com/h5/mtop.cnwireless.cnlogisticdetailservice.wapquerylogisticpackagebymailno/1.0/";
            var cookieUrl = @"http://api.wap.guoguo-app.com/h5/mtop.cnwireless.cncainiaoappservice.getlogisticscompanylist/1.0/?v=1.0&api=mtop.cnwireless.CNCainiaoAppService.getLogisticsCompanyList&appKey=12574478&t=1496741493353&callback=mtopjsonp1&type=jsonp&sign=b0f7376271effd90e311f998ad3a3efb&data=%7B%22version%22%3A0%2C%22cptype%22%3A%22all%22%7D";

            var mailNo = "9890758806931";//运单号码
            //准备参数
            var tikets = (DateTime.Now - Convert.ToDateTime("1970-01-01 00:00:00")).Ticks.ToString();
            var cookie = GetCookie(cookieUrl);//先请求一次获得cookie,可以先缓存下来。
            var key = new Regex(@"(?:^|;\s*)_m_h5_tk\=([^;]+)(?:;\s*|$)").Match(cookie).Value.Split('=')[1]
                .Split('_')[0];
            var sign = GetMD5Hash(key + "&" + tikets + "&12574478" + "&{\"mailNo\":\"" + mailNo + "\"}")
                .ToLower();

            //获得参数列表
            var urlParas = string.Format(
                "?v=1.0&api=mtop.cnwireless.CNLogisticDetailService.wapqueryLogisticPackageByMailNo&appKey=12574478&t={0}&callback=mtopjsonp&type=jsonp&sign={1}",
                tikets, sign);
            urlParas += "&data={\"mailNo\":\"" + mailNo + "\"}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mainUrl + urlParas);
            request.Method = "GET";
            request.Headers.Add("Cookie", cookie);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            Console.WriteLine(content);
            Console.ReadKey();
        }

        public static string GetMD5Hash(String input)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(input, "md5");
        }        

        //获得cookie
        public static string GetCookie(string url, int Timeout = 5000, bool isNeedProxy = true)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = false;
                request.ContentType = "application/x-www-form-urlencoded;charset=gbk";
                request.CookieContainer = new CookieContainer();
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                string content = reader.ReadToEnd();
                return response.Headers.Get("Set-Cookie");
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
