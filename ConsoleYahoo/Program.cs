using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace ConsoleYahoo
{
    class Program
    {

        static string _fileName = "CompletedList";
        static string _baseUrl = "http://news.yahoo.com/";

        static List<string> _completedList = new List<string>();
        static List<string> _toDownloadList = new List<string>();
        static List<string> _searchedList = new List<string>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Initializing......");
                InitAndReadList();
                Console.WriteLine("Searching us ......");
                Search(_baseUrl + "us/", 15);
                Console.WriteLine("Searching odd news ......");
                Search(_baseUrl + "odd-news/", 15);
                Console.WriteLine("Downing ......");
                Download();
                Thread.Sleep(3600000);
            }
        }

        static void Search(string url, int depth)
        {
            if (depth == 0)
                return;
            List<string> res = new List<string>();
            //using (Browser b = new Browser())
            //{
            //    b.Deal(url);
            //    while (!b.Finished)
            //    {
            //        Thread.Sleep(100);
            //    }
            //    res.AddRange(b.Result);
            //}
            Process ps = new Process();
            ps.StartInfo.FileName = "FormsBrowser.exe";
            ps.StartInfo.Arguments = url;
            ps.Start();
            ps.WaitForExit();
            if (!File.Exists("SearchResult"))
                return;
            using (StreamReader sr = new StreamReader("SearchResult"))
            {
                while (!sr.EndOfStream)
                {
                    res.Add(sr.ReadLine());
                }
            }
            res.ForEach(newsUrl =>
                {
                    if (!_completedList.Contains(newsUrl) && !_toDownloadList.Contains(newsUrl))
                        _toDownloadList.Add(newsUrl);
                });
            Console.WriteLine("All: " + _toDownloadList.Count + " Depth: " + depth);
            using (StreamWriter sw = new StreamWriter("toDownloadList"))
            {
                _toDownloadList.ForEach(x =>
                {
                    sw.WriteLine(x);
                });
            }
            res.ForEach(newsUrl =>
            {
                if (!_searchedList.Contains(newsUrl))
                {
                    _searchedList.Add(newsUrl);
                    Search(newsUrl, depth - 1);
                    Thread.Sleep(1000);
                }
            });
        }

        static void Download()
        {
            Console.WriteLine("All: " + _toDownloadList.Count);
            _toDownloadList.ForEach(url =>
            {
                Console.WriteLine("Download: " + url);
                Crawler pc = new Crawler(url);
                if (pc.Deal())
                    WriteList(url);
            });
        }

        static void InitAndReadList()
        {
            _toDownloadList.Clear();
            _completedList.Clear();
            _searchedList.Clear();
            if (!File.Exists(_fileName))
                return;
            using (StreamReader sr = new StreamReader(_fileName))
            {
                while (!sr.EndOfStream)
                {
                    _completedList.Add(sr.ReadLine());
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
    }
}
