using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dax.Scrapping.Appraisal
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool bAflag_ = false;
            bool bPrestartPauseflag_ = true;
            bool bStartflag_ = false;
            CheckCommandLine(ref bAflag_, ref bPrestartPauseflag_, ref bStartflag_);
            //bStartflag_ = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(bStartflag_));
        }

        static void CheckCommandLine(ref bool bAflag_, ref bool bPrestartPauseflag_, ref bool bStartflag_)
        {
            // check command line args
            string[] cmdArgs = Environment.GetCommandLineArgs();
            string str = "";
            foreach (string arg in cmdArgs)
            {
                str += arg + " ";
            }

            str.ToLower();
            if (str.IndexOf("-a") != -1)
            {
                bAflag_ = true;
            }

            bStartflag_ = false;
            if (str.IndexOf("/start") != -1)
            {
                bStartflag_ = true;
            }

            bPrestartPauseflag_ = true;
            if (str.IndexOf("/withoutpause") != -1)
            {
                bPrestartPauseflag_ = false;
            }
        }
    }
}
