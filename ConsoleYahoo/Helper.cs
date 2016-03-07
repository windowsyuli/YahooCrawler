using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleYahoo
{
    class Helper
    {
        public static List<string> MatchContent(string pattern, string content)
        {
            List<string> result = new List<string>();
            MatchCollection match = new Regex(pattern, RegexOptions.Singleline).Matches(content);
            foreach (Match t in match)
                result.Add(t.Value);
            return result;
        }

        public static List<string> MatchContent(string before, string end, string content)
        {
            string pattern = "(?<=" + before + ").+?(?=" + end + ")";
            return MatchContent(pattern, content);
        }

        public static void Log(string url, string message)
        {
            Console.WriteLine("url: " + url);
            Console.WriteLine("message: " + message);
            using (StreamWriter sr = new StreamWriter("log.txt", true))
            {
                sr.WriteLine("url:  " + url);
                sr.WriteLine("message: " + message);
            }
        }

        public static string WebDownload(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                Byte[] pageData = wc.DownloadData(url);
                return Encoding.UTF8.GetString(pageData);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
