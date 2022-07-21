using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SEBasicIV
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 1)
            {
                string text = File.ReadAllText(args[1]);
                string path = args[1];
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SyntaxEditor(text, path));
            }
            else
            { 
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG

                Application.Run(new SplashScreen());
#else
                Application.Run(new SplashScreen());

#endif
            }
        }
    }
}
