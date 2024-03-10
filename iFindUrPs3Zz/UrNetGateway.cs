using iFindUrPs3Zz;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using UrNetGateway;
namespace UrNetGateway
{
    public
   static class UrNetGateway
    {
        public static string NetworkGateway()
        {
            string ip = null;

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        ip = d.Address.ToString();
                    }
                }
            }

            return ip;
        }
    }

}
namespace pinger

{
    public static class Pinger
    {
        public static List<string> addresses = new List<string>();
        public static Form1 target;
        public static void Ping_all()
        {
            new Thread(() =>
            {
                string gate_ip = UrNetGateway.UrNetGateway.NetworkGateway();

                //Extracting and pinging all other ip's.
                string[] array = gate_ip.Split('.');
                addresses.Clear();
                for (int i = 2; i <= 255; i++)
                {

                    string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;

                    //time in milliseconds           
                    Ping(ping_var, 1, 500);

                }
            }).Start();


        }
        public static string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException)
            {
                // MessageBox.Show(e.Message.ToString());
            }

            return null;
        }


        //Get MAC address
        public static string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process Process = new System.Diagnostics.Process();
            Process.StartInfo.FileName = "arp";
            Process.StartInfo.Arguments = "-a " + ipAddress;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.Start();
            string strOutput = Process.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                         + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                         + "-" + substrings[7] + "-"
                         + substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "OWN Machine";
            }
        }

        public static void Ping(string host, int attempts, int timeout)
        {
            for (int i = 0; i < attempts; i++)
            {
                // new Thread(delegate ()
                //{

                try
                {
                    System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                    ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
                    ping.SendAsync(host, timeout, host);
                }
                catch
                {
                    // Do nothing and let it try again until the attempts are exausted.
                    // Exceptions are thrown for normal ping failurs like address lookup
                    // failed.  For this reason we are supressing errors.
                }
                //}).Start();
            }
        }
        private static void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string hostname = GetHostName(ip);
                string macaddres = GetMacAddress(ip);

                // Logic for Ping Reply Success
                ListViewItem item;
                if (target.InvokeRequired)
                {
                    if (!addresses.Contains(ip))
                    {
                        string url = $"http://{ip}/dev_hdd0/boot_plugins.txt";
                        try
                        {
                            bool ps3 = new WebClient().DownloadString(url).Length > 0;
                            target.Invoke(new Action(() =>
                            {
                                item = new ListViewItem(ip);
                                item.Tag = ip;
                                item.SubItems.Add(macaddres);

                                item.SubItems.Add("PlayStation 3 (" + hostname + ").");

                                item.SubItems.Add(hostname);

                                item.ForeColor = Color.White;
                                target.listView1.Items.Add(item);
                            }));
                            addresses.Add(ip);
                        }
                        catch
                        {

                        }

                    }

                }


            }
            else
            {
                // MessageBox.Show(e.Reply.Status.ToString());
            }
        }



    }
}