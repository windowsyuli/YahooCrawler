using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System;

namespace YahooCrawler
{
    class Program
    {
        static string _fileName = "NewsList";
        static List<string> _completeList = new List<string>();
        static string _baseUrl = "http://news.yahoo.com/";
        static List<string> _toBeDownload = new List<string>();
        static List<string> _searched = new List<string>();
        static void Main(string[] args)
        {
            int l = 20;
            if (args.Length == 1)
                l = int.Parse(args[0]);
            Console.WriteLine("Deeplength: " + l);
            while (true)
            {
                Console.WriteLine("readList");
                ReadList();
                Console.WriteLine("us");
                SearchNews(_baseUrl + "us/", l);
                Console.WriteLine("odd-news");
                SearchNews(_baseUrl + "odd-news/", l);
                DownloadNews();
                Console.WriteLine("sleep");
                Thread.Sleep(3600000);
            }
        }

        static void ReadList()
        {
            _toBeDownload.Clear();
            _completeList.Clear();
            _searched.Clear();
            if (!File.Exists(_fileName))
                return;
            using (StreamReader sr = new StreamReader(_fileName))
            {
                while (!sr.EndOfStream)
                {
                    _completeList.Add(sr.ReadLine());
                }
            }
        }

        static void WriteList(string url)
        {
            using (StreamWriter sw = new StreamWriter(_fileName, true))
            {
                sw.WriteLine(url);
            }
        }

        static string GetUrl(string tarUrl, string value)
        {
            if (value.Contains("video/") || value.Contains("http") || value.Contains("photos/") || value.Contains("photos\\") || value.Contains("video\\"))
                return "";
            int startIndex = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if ((value[i] >= 'a' && value[i] <= 'z') || (value[i] >= '0' && value[i] <= '9'))
                {
                    startIndex = i;
                    break;
                }
            }
            int endIndex = 0;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (value[i] == 'l')
                {
                    endIndex = i;
                    break;
                }
            }
            return tarUrl + value.Substring(startIndex, endIndex - startIndex + 1);
        }

        static void SearchNews(string searchUrl, int depth)
        {
            if (depth == 0)
            {
                return;
            }
            string content = Helper.WebDownloads(searchUrl);
            MatchCollection match = new Regex("(?<=href=).{20,120}?.html", RegexOptions.Singleline).Matches(content);
            foreach (Match m in match)
            {
                string url = GetUrl(_baseUrl, m.Value);
                if (url == "")
                    continue;
                if (!_searched.Contains(url))
                {
                    if (!_completeList.Contains(url) && !_toBeDownload.Contains(url))
                        _toBeDownload.Add(url);
                    _searched.Add(url);
                    SearchNews(url, depth - 1);
                }
            }
            Console.WriteLine("All: " + _toBeDownload.Count);
        }

        static void DownloadNews()
        {
            Console.WriteLine("All: " + _toBeDownload.Count);
            _toBeDownload.ForEach(url =>
            {
                Console.WriteLine("download " + url);
                News n = new News(url);
                if (n.Deal())
                    WriteList(url);
            });
        }

    }
}
