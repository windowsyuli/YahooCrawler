using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsBrowser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;
            using (FormBrowser b = new FormBrowser())
            {
                b.Deal(args[0]);
            }
        }
    }
}
