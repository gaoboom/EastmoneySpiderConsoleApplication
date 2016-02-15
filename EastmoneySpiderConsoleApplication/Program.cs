using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace EastmoneySpiderConsoleApplication
{
    class Program
    {
        #region 全局常量
        public static string ContentListPage1 = "http://finance.eastmoney.com/news/cdfsd.html";
        public static string ContentListPage2 = "http://finance.eastmoney.com/news/cdfsd_2.html";
        #endregion

        #region 通用函数
        public static void TimeDelay(int ms)
        {
            Console.WriteLine("系统休眠 " + (ms / 1000) + " 秒");
            System.Threading.Thread.Sleep(ms);
        }
        /// <summary>
        /// 根据URL地址获取网页信息 get方法
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetUrltoHtml(string Url, string type)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }

        public static System.IO.Stream GetUrlToStream(string Url)
        {
            System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            return respStream;
        }
        #endregion

        #region 类定义
        public class ContentList
        {
            public string listpagecode;
            
            
            //获取列表页代码
            public void GetListCode()
            {
                string listXPath = "//html[1]/body[1]/div[8]/div[1]/div[1]//li";
                HtmlDocument HtDoc = new HtmlDocument();
                HtDoc.Load(GetUrlToStream(ContentListPage1));
                HtmlNode rootNode = HtDoc.DocumentNode;
                HtmlNodeCollection listBoxNode =rootNode.SelectNodes(listXPath);
                listpagecode = listBoxNode[1].InnerHtml;
            }
            
        }
        public class ContentDedail
        {

        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("抓取系统开始工作...");
            Console.WriteLine("实例化列表页类并尝试获取前2页列表页信息...");
            ContentList CL = new ContentList();
            CL.GetListCode();
            Console.WriteLine(CL.listpagecode);
            Console.WriteLine("工作已完成，按任意键退出...");
            Console.ReadKey();
        }
    }
}
