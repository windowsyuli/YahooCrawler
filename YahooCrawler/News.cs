using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YahooCrawler
{
    public enum State
    {
        NULL,
        NEWS,
        NEWSANDCOMMENT
    }
    public class News
    {
        State _sta = State.NULL;
        string _newsUrl = "";
        string _content = "";
        string _contentBefore = "<div class=\"body yom-art-content clearfix\"";
        string _contentAfter = "<!-- google_ad_section_end -->";
        string _idBefore = "\"content_id\":\"";
        string _idAfter = "\"";
        string _commentsBaseUrl = "https://news.yahoo.com/_xhr/contentcomments/get_comments/?content_id=";
        string _commentBefore = "<p class=\\\\\"comment-content\\\\\">";
        string _commentAfter = "<\\\\/p>";
        string _id = "";
        string _newsName = "";
        public string MainBody = "";
        public List<string> Comments = new List<string>();

        public News(string url)
        {
            _newsUrl = url;
            try
            {
                //http://news.yahoo.com/ex-two-half-men-star-charlie-sheen-says-124752276.html
                _newsName = url.Split('/')[3].Split('.')[0];
            }
            catch (Exception)
            {
                _newsName = Guid.NewGuid().ToString();
            }
        }

        public bool Deal()
        {
            if (DownloadNews())
            {
                WriteNews();
                int count = 100;
                while (count > 0)
                {
                    if (DownloadCommments(count))
                    {
                        WriteComment();
                        break;
                    }
                    else
                        count -= 10;
                }
                return true;
            }
            return false;
        }

        public List<string> MatchContent(string before, string end, string content)
        {
            List<string> result = new List<string>();
            string str = "(?<=" + before + ").+?(?=" + end + ")";
            MatchCollection temp = new Regex(str, RegexOptions.Singleline).Matches(content);
            foreach (Match t in temp)
            {
                result.Add(t.Value);
            }
            return result;
        }

        public string GetResult(string str)
        {
            return str.Replace("&#39;", "'").Replace("&quot;", "").Replace("\\n", "").Replace("<br \\/>", "").Replace("<p>", "").Replace("</p>", "").Trim();
        }
        public bool DownloadNews()
        {
            bool result = true;
            try
            {
                _content = Helper.WebDownloads(_newsUrl);
                if (_content == "")
                    throw new Exception("NULL Content");
                List<string> tmp = MatchContent(_contentBefore, _contentAfter, _content);
                if (tmp.Count == 0)
                    throw new Exception("Can't match mainbody");
                string mainBody = tmp[0];
                tmp = MatchContent("<p>", "</p>", mainBody);
                if (tmp.Count == 0)
                    throw new Exception("Can't find <p>");
                mainBody = "";
                tmp.ForEach(x =>
                {
                    x = x.Trim();
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        x = GetResult(x);
                        mainBody += x + " ";
                    }
                });
                if (mainBody.Length == 0)
                    throw new Exception("Can't find MainBody");
                MainBody = mainBody;
                _sta = State.NEWS;
            }
            catch (Exception e)
            {
                _sta = State.NULL;
                Logging("content error: " + e.Message);
                result = false;
            }
            return result;
        }

        public bool DownloadCommments(int count)
        {
            bool result = true;
            try
            {
                List<string> tmp = MatchContent(_idBefore, _idAfter, _content);
                if (tmp.Count == 0)
                    throw new Exception("ID no found");
                _id = tmp[0];
                string CommentUrl = _commentsBaseUrl + _id + "&_device=full&count=" + count + "&sortBy=highestRated";
                string urlContent = Helper.WebDownloads(CommentUrl);
                tmp = MatchContent(_commentBefore, _commentAfter, urlContent);
                if (tmp.Count == 0)
                    throw new Exception("Comments no found");
                tmp.ForEach(x =>
                {
                    x = x.Trim();
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        x = GetResult(x);
                        Comments.Add(x);
                    }
                });
                _sta = State.NEWSANDCOMMENT;
                if (count != 100)
                    Console.WriteLine("commment " + count + " complete!");
            }
            catch (Exception e)
            {
                _sta = State.NEWS;
                Logging("comment " + count + " error:" + e.Message);
                result = false;
            }
            return result;
        }

        public void Logging(string exceptionMessage)
        {
            Console.WriteLine("error: " + exceptionMessage);
            using (StreamWriter sr = new StreamWriter("log.txt", true))
            {
                sr.WriteLine("url:  " + _newsUrl);
                sr.WriteLine("Mess: " + exceptionMessage);
            }
        }

        public void WriteNews()
        {
            if (!Directory.Exists("Download"))
                Directory.CreateDirectory("Download");
            if (_sta == State.NEWS || _sta == State.NEWSANDCOMMENT)
            {
                using (StreamWriter sw = new StreamWriter(".\\Download\\" + _newsName + ".txt"))
                {
                    sw.WriteLine("Content: " + MainBody);
                }
            }
        }

        public void WriteComment()
        {
            if (!Directory.Exists("Download"))
                Directory.CreateDirectory("Download");
            if (_sta == State.NEWSANDCOMMENT)
            {
                using (StreamWriter sw = new StreamWriter(".\\Download\\" + _newsName + ".txt", true))
                {
                    Comments.ForEach(x => { sw.WriteLine("Comment: " + x); });
                }
            }
        }
    }
}
