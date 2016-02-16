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
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

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
            //默认休眠时间
            public static int DefaltSleepTime = 5000;
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
            /// <returns>返回匹配字符</returns>
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
            private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            /// <summary>  
            /// 创建GET方式的HTTP请求  
            /// </summary>  
            /// <param name="url">请求的URL</param>  
            /// <param name="timeout">请求的超时时间</param>  
            /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
            /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
            /// <returns></returns>  
            public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                return request.GetResponse() as HttpWebResponse;
            }
            /// <summary>  
            /// 创建POST方式的HTTP请求  
            /// </summary>  
            /// <param name="url">请求的URL</param>  
            /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>  
            /// <param name="timeout">请求的超时时间</param>  
            /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
            /// <param name="requestEncoding">发送HTTP请求时所用的编码</param>  
            /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
            /// <returns></returns>  
            public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int? timeout, string userAgent, Encoding requestEncoding, CookieCollection cookies)
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                if (requestEncoding == null)
                {
                    throw new ArgumentNullException("requestEncoding");
                }
                HttpWebRequest request = null;
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                else
                {
                    request.UserAgent = DefaultUserAgent;
                }

                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                //如果需要POST数据  
                if (!(parameters == null || parameters.Count == 0))
                {
                    StringBuilder buffer = new StringBuilder();
                    int i = 0;
                    foreach (string key in parameters.Keys)
                    {
                        if (i > 0)
                        {
                            buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                        }
                        else
                        {
                            buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        }
                        i++;
                    }
                    byte[] data = requestEncoding.GetBytes(buffer.ToString());
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                return request.GetResponse() as HttpWebResponse;
            }

            private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true; //总是接受  
            }
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
            Console.WriteLine("初始化...");
            Console.WriteLine("请求开始...");
            HttpHelper.CreateGetHttpResponse(GlobalParament.ContentListPage,GlobalParament.DefaltSleepTime,null,null);
            Console.WriteLine("工作已完成，按任意键退出...");
            Console.ReadKey();
        }
    }
}
