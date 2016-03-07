using System.Collections.Generic;
using System.Timers;
using System.Windows.Forms;

namespace ConsoleYahoo
{
    public partial class Browser : Form
    {

        string _urlPattern = "(?<=href=).{20,120}?(?=.html)";
        int _posY = 1000000;
        int _timeInteval = 10000;
        System.Timers.Timer _stt;
        public bool Finished = false;
        public List<string> Result = new List<string>();
        string _content = "";

        public Browser()
        {
            InitializeComponent();
            _stt = new System.Timers.Timer(_timeInteval);
            _stt.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        public void Deal(string url)
        {
            Finished = false;
            Result.Clear();
            YahooBrowser.Navigate(url);
            ShowDialog();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _stt.Stop();
            Result = SearchNews(_content);
            Finished = true;
            Close();
            YahooBrowser.Dispose();
        }

        private void YahooBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            YahooBrowser.Document.Window.ScrollTo(0, _posY);
            _content = YahooBrowser.Document.Body.OuterHtml;
            _stt.Stop();
            _stt.Start();
        }

        private List<string> SearchNews(string content)
        {
            List<string> result = new List<string>();
            Helper.MatchContent(_urlPattern, content).ForEach(value =>
            {
                int startIndex = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if ((value[i] >= 'a' && value[i] <= 'z') || (value[i] >= '0' && value[i] <= '9'))
                    {
                        startIndex = i;
                        break;
                    }
                }
                value = value.Substring(startIndex).Replace("\\/", "/");
                if (!result.Contains(value) && !value.Contains("="))
                {
                    if (!value.Contains("/"))
                    {
                        result.Add("http://news.yahoo.com/" + value + ".html");
                    }
                    else if (value.Contains("news.yahoo.com") && !value.Contains("login.yahoo.com") && !value.Contains("video/") && !value.Contains("photo/"))
                    {
                        result.Add(value + ".html");
                    }
                }
            });
            return result;
        }
    }
}
