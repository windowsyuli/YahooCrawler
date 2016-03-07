using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleYahoo
{
    class Crawler
    {
        string _newsName = "";
        string _newsUrl = "";
        string _dirName = "result";

        public string Content = "";
        string _contentBefore = "<div class=\"body yom-art-content clearfix\"";
        string _contentAfter = "<!-- google_ad_section_end -->";

        string _idBefore = "\"content_id\":\"";
        string _idAfter = "\"";
        string _commentsBaseUrl = "https://news.yahoo.com/_xhr/contentcomments/get_comments/?content_id=";
        string _commentBefore = "<p class=\\\\\"comment-content\\\\\">";
        string _commentAfter = "<\\\\/p>";

        public string NewsContent = "";
        public List<string> Comments = new List<string>();

        public Crawler(string url)
        {
            //http://news.yahoo.com/ex-two-half-men-star-charlie-sheen-says-124752276.html
            _newsUrl = url;
            _newsName = url.Split('/')[3].Split('.')[0];
        }

        public string ProcessText(string text)
        {
            text = text.Replace("&#39;", "'").Replace("&quot;", "").Replace("\\n", "");
            List<string> result = Helper.MatchContent("<.{1,6}?>", text);
            result.ForEach(x => { text.Replace(x, ""); });
            return text.Trim();
        }

        public bool DownloadNews()
        {
            try
            {
                Content = Helper.WebDownload(_newsUrl);
                if (Content == "")
                    throw new Exception("NULL Content.");

                List<string> matchContent = Helper.MatchContent(_contentBefore, _contentAfter, Content);
                if (matchContent.Count == 0)
                    throw new Exception("Can't find NewsContent");

                matchContent = Helper.MatchContent("<p>", "</p>", matchContent[0]);
                if (matchContent.Count == 0)
                    throw new Exception("Can't find <p> in NewsContent");

                string tmpContent = "";
                matchContent.ForEach(x =>
                {
                    x = x.Trim();
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        x = ProcessText(x);
                        tmpContent += x + " ";
                    }
                });

                if (tmpContent.Length == 0)
                    throw new Exception("It's a pity that the match content is null.");
                NewsContent = tmpContent;
                return true;
            }
            catch (Exception e)
            {
                Helper.Log(_newsUrl, e.Message);
                return false;
            }
        }

        public bool DownloadCommments(int count, string message = "")
        {
            if (count <= 0)
            {
                Helper.Log(_newsUrl, message);
                return false;
            }
            try
            {
                List<string> matchTmp = Helper.MatchContent(_idBefore, _idAfter, Content);
                if (matchTmp.Count == 0)
                    throw new Exception("ID no found");

                string CommentUrl = _commentsBaseUrl + matchTmp[0] + "&_device=full&count=" + count + "&sortBy=highestRated";
                string urlContent = Helper.WebDownload(CommentUrl);
                matchTmp = Helper.MatchContent(_commentBefore, _commentAfter, urlContent);
                if (matchTmp.Count == 0)
                    throw new Exception("Comments no found");

                matchTmp.ForEach(x =>
                {
                    x = x.Trim();
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        x = ProcessText(x);
                        Comments.Add(x);
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                return DownloadCommments(count - 10, e.Message);
            }
        }

        public void WriteNews()
        {
            if (!Directory.Exists(_dirName))
                Directory.CreateDirectory(_dirName);
            using (StreamWriter sw = new StreamWriter(".\\" + _dirName + "\\" + _newsName + ".txt"))
            {
                sw.WriteLine("Content: " + NewsContent);
            }
        }

        public void WriteComment()
        {
            if (!Directory.Exists(_dirName))
                Directory.CreateDirectory(_dirName);
            using (StreamWriter sw = new StreamWriter(".\\" + _dirName + "\\" + _newsName + ".txt", true))
            {
                Comments.ForEach(x => { sw.WriteLine("Comment: " + x); });
            }
        }

        public bool Deal()
        {
            if (DownloadNews())
            {
                WriteNews();
                if (DownloadCommments(100))
                {
                    WriteComment();
                }
                return true;
            }
            return false;
        }

    }
}
