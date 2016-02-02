using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

        #region 类定义
        public class ContentList
        {

        }
        public class ContentDedail
        {

        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("抓取系统开始工作");
            TimeDelay(1000);
            Console.WriteLine("工作已完成，按任意键退出...");
            Console.ReadKey();
        }
    }
}
