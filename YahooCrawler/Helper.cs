using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace YahooCrawler
{
    class Helper
    {
        public static string HttpDownloads(string url, string Encode)
        {
            int time = 5;
            while (time-- != 0)
            {
                try
                {
                    Cache _cache = HttpRuntime.Cache;
                    IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                    while (CacheEnum.MoveNext())
                    {
                        _cache.Remove(CacheEnum.Key.ToString());
                    }
                    string content = "";
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Timeout = 10000;
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                    using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(Encode)))
                        content = reader.ReadToEnd();
                    return content;
                }
                catch (Exception)
                {

                }
            }
            Console.WriteLine("Try 5 times but failed.");
            return "";
        }
        public static string WebDownloads(string url)
        {
            try
            {
                WebClient wc = new WebClient();
                Byte[] pageData = wc.DownloadData(url);
                string s = Encoding.UTF8.GetString(pageData);
                return s;
            }
            catch(Exception)
            {
                return HttpDownloads(url);
            }
        }
        public static string HttpDownloads(string url)
        {
            return HttpDownloads(url, "UTF-8");
        }
    }
}
