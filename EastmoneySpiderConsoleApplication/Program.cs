using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace EastmoneySpiderConsoleApplication
{
    class Program
    {

        #region 通用函数
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
                WebRequest wReq = WebRequest.Create(Url);
                // Get the response instance.
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (StreamReader reader = new StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static Stream GetUrlToStream(string Url)
        {
            WebRequest wReq = WebRequest.Create(Url);
            wReq.Timeout = 10000;
            ((HttpWebRequest)wReq).UserAgent= "Mozilla / 4.0(compatible; MSIE 8.0; Windows NT 5.1; Trident / 4.0; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            //try
            //{
            //    WebResponse wResp = wReq.GetResponse();
            //    Stream respStream = wResp.GetResponseStream();
            //    return respStream;
            //}
            //catch
            //{
            //    return null;
            //}
            WebResponse wResp = wReq.GetResponse();
            Stream respStream = wResp.GetResponseStream();
            return respStream;
        }

        #endregion

        #region 类定义

        /// <summary>
        /// 全局常量类
        /// </summary>
        public static class GlobalParament
        {
            //列表页
            public static string ContentListPage = "http://finance.eastmoney.com/news/cdfsd.html";
        }

        /// <summary>
        /// 通用函数类
        /// </summary>
        public static class CommonFunction
        {
            /// <summary>
            /// 系统休眠等待
            /// </summary>
            /// <param name="ms">休眠毫秒数</param>
            public static void TimeDelay(int ms)
            {
                Console.WriteLine("系统休眠 " + (ms / 1000) + " 秒");
                System.Threading.Thread.Sleep(ms);
            }

            /// <summary>
            /// 获取两个字符串中间的字符串
            /// </summary>
            /// <param name="givenString">给定字符串</param>
            /// <param name="startString">起始字符串</param>
            /// <param name="endString">结束字符串</param>
            /// <returns></returns>
            public static string GetMiddleString(string givenString, string startString, string endString)
            {
                Regex rg = new Regex("(?<=(" + startString + "))[.\\s\\S]*?(?=(" + endString + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                return rg.Match(givenString).Value;
            }
        }

        /// <summary>
        /// http请求类
        /// </summary>
        public static class HttpHelper
        {

        }

        /// <summary>
        /// 数据库操作类
        /// </summary>
        public static class MysqlHelper
        {

        }

        /// <summary>
        /// 文章列表类
        /// </summary>
        public class ContentList
        {
            public List<string> PubTimeList = new List<string>();
            public List<string> LinkList= new List<string>();
            public List<string> TitleList = new List<string>();

            /// <summary>
            /// 获取列表页信息，包括时间/链接/标题
            /// 读取URL成功后清空PubTimeList,LinkList,TitleList并重新写入，函数返回0
            /// 读取URL失败后则不清空之前的List，函数返回1
            /// </summary>
            /// <returns>成功获取列表信息返回1，否则返回0</returns>
            public int GetListInfo()
            {
                string listXPath = "//html[1]/body[1]/div[8]/div[1]/div[1]//li";
                HtmlDocument HtDoc = new HtmlDocument();
                Stream urlReadStream;
                urlReadStream = GetUrlToStream(GlobalParament.ContentListPage);
                HtDoc.Load(urlReadStream);
                HtmlNode rootNode = HtDoc.DocumentNode;
                HtmlNodeCollection listBoxNode =rootNode.SelectNodes(listXPath);
                foreach(HtmlNode node in listBoxNode)
                {
                    string listLineCode = node.InnerHtml;
                    string pubTime = CommonFunction.GetMiddleString(listLineCode, "<span>", "</span>");
                    string link = CommonFunction.GetMiddleString(listLineCode, "<a href=\"", "\" title=");
                    string title = CommonFunction.GetMiddleString(listLineCode, "blank\">", "</a>");
                    PubTimeList.Add(pubTime);
                    LinkList.Add(link);
                    TitleList.Add(title);
                }
                return 0;
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
            Console.WriteLine("工作已完成，按任意键退出...");
            Console.ReadKey();
        }
    }
}
