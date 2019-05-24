using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.Threading;
using Newtonsoft.Json;
using Dax.Scrapping.Appraisal.Core;

namespace Dax.Scrapping.Appraisal
{
    public partial class MainForm : Form
    {
        private AppraisalScrapper _scraper = null;
        private bool _AutoStart = false;


        public MainForm(bool autoStart = false)
        {
            _AutoStart = autoStart;
            InitializeComponent();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
            InitializeBrouser();
            Cef.Initialize();
            if (_AutoStart)
            {
                btnSearch_Click(null, null);
            }

        }

        private void InitializeBrouser()
        {


        }

        private void StopLoadingInProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                                                  {
                                                      progressBar1.MarqueeAnimationSpeed = 0;
                                                      progressBar1.Style = ProgressBarStyle.Blocks;
                                                  }));

            }
            else
            {
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
            }

        }

        private void StarLoadingInProgress()
        {
            progressBar1.MarqueeAnimationSpeed = 80;
            progressBar1.Style = ProgressBarStyle.Marquee;
            Log("Loading...");
        }

        private void Log(string msg)
        {
            try
            {
                var rMsg = string.Format("{0} : {1}{2}",DateTime.Now, msg, Environment.NewLine);
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => {
                        File.AppendAllText("./log.txt", rMsg);
                        txtLogs.AppendText(rMsg);
                    }));

                }
                else
                {
                    txtLogs.AppendText(rMsg);
                    File.AppendAllText("./log.txt", rMsg);
                }
            }
            catch (Exception ex)
            {

                
            }
    

        }

        private void ClearLog()
        {
            txtLogs.Clear();
        }



        private void LoadConfiguration()
        {
            txtUserId.Text = Helper.GetAppSettingAsString("User");
            txtUserPassword.Text = Helper.GetAppSettingAsString("Password");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Clear();
                this.panelBrowser.Controls.Clear();
                _scraper = new AppraisalScrapper(txtUserId.Text, txtUserPassword.Text);
                _scraper.OnLog += Log;
                _scraper.OnCompleted += _scraper_OnCompleted;
                var browser = _scraper.BrouserComponent;
                this.panelBrowser.Controls.Add(_scraper.BrouserComponent);
                browser.Dock = DockStyle.Fill;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void _scraper_OnCompleted()
        {
            if (_AutoStart)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(Close));

                }
                else
                {
                    this.Close();

                }
            }
        }

        private void Clear()
        {

            txtLogs.Clear();
        }

        private void UpdateGrid(List<Order> OrderList)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    dgvData.DataSource = OrderList;
                }));
            }
            else
            {
                dgvData.DataSource = OrderList;
            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_scraper != null)
                _scraper.Dispose();

            Cef.Shutdown();
        }


    }
}
