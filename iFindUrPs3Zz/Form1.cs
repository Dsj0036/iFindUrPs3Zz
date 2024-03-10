using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UrNetGateway;
namespace iFindUrPs3Zz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pinger.Pinger.target = this;
            pinger.Pinger.Ping_all();
            this.checkBox1.Checked = Startup.IsInStartup();
            timer1.Interval = 60000;
            timer1.Start();
            this.FormClosed += (s, e) => timer1.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
        void set_waiting(bool isit)
        {
            this.Enabled = !isit;
            this.LabelStatus.Text = isit ? "Updating..." : "OK";
        }
        private void request(string ip, string cmd)
        {
            Exception err = null;
            try
            {

                new WebClient().DownloadString($"http://{ip}/"+cmd);

            }
            catch (Exception ee)
            {
                err = ee;
            }
            if (err != null)
            {
                MessageBox.Show(err.Message, "Error (" + err.GetType().Name + ")", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Command executed successfully", "OK!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            this.Invoke(new Action(() =>
            {

                this.listView1.Items.Clear();
                pinger.Pinger.target = this;
                pinger.Pinger.Ping_all();
                set_waiting(false);
            }));
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                set_waiting(true);
                var selection = listView1.Items[0];
                var ip = selection.Text;
                new Thread(() =>
                {
                    request(ip, "shutdown.ps3");

                }).Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                set_waiting(true);
                var selection = listView1.Items[0];
                var ip = selection.Text;
                new Thread(() =>
                {
                    request(ip, "xmb.ps3$reloadgame");
                    this.Invoke(new Action(() => set_waiting(false)));
                }).Start();

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                set_waiting(true);
                var selection = listView1.Items[listView1.SelectedIndices[0]];
                var ip = selection.Text;
                new Thread(() =>
                {
                    request(ip, "xmb.ps3$exit");
                }).Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                set_waiting(true);
                var selection = listView1.Items[listView1.SelectedIndices[0]];
                var ip = selection.Text;
                new Thread(() =>
                {
                    request(ip, txtcmd.Text);
                }).Start();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                if (Startup.IsInStartup())
                {

                    Startup.RemoveFromStartup();
                }
            }
            else
            {
                if (!Startup.IsInStartup())
                {
                    Startup.RunOnStartup();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            this.Invoke(new Action(() =>
            {

                set_waiting(true);
                this.listView1.Items.Clear();
                pinger.Pinger.target = this;
                pinger.Pinger.Ping_all();
                set_waiting(false);
            }));
        }
    }
}
