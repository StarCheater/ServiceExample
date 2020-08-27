using System;
using System.ServiceProcess;
using NamedPipeWrapper;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Leaf.xNet;

namespace ServiceExample
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            this.CanStop = true;
        }

        private bool work = true;

        protected override void OnStart(string[] args)
        {
            work = true;
            BW_NPipe.RunWorkerAsync();
            BW_last_check.RunWorkerAsync();
            t_last_check.Start();
        }


        protected override void OnStop()
        {
            work = false;
            t_last_check.Stop();
        }
        
        private void BW_NPipe_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var server = new NamedPipeServer("npstest");
            server.StartListen();

            while (work)
            {
                var _in = server.AwaitSingleMessageAsync<char[]>().Result;
                string s = new string(_in.Message);
                Console.WriteLine("<<" + s);
                Task.Run(async () =>
                {
                    if(await is_pinged()) responce = GetFromNet();
                        else
                        {
                            responce = "NOT PINGED";
                        }
                    ;
                    await server.SendMessageToAllAsync(responce+">>" + s);
                }).Wait(1000);
            }
        }

        private void t_last_check_Tick(object sender, EventArgs e)
        {
            if(work && !BW_last_check.IsBusy) BW_last_check.RunWorkerAsync();
        }

        private void BW_last_check_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            is_pinged().ContinueWith(o =>
            {
                if (o.Result) responce = GetFromNet();
                else
                {
                    responce = "NOT PINGED";
                }
            }).Wait(1000);
        }


        public static string MyMac
        {
            get
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                        if (nic.GetPhysicalAddress().ToString() != "")
                            return nic.GetPhysicalAddress().ToString();
                return "";
            }
            set { }
        }

        private string responce = "";

        static string ip = "192.168.11.10.test";
        public static string GetFromNet()
        {
            using (var request = new HttpRequest())
            {
                RequestParams d = new RequestParams();
                d["mac"] = MyMac;
                return request.Post(ip, d).ToString();
            }

            return "";
        }

        async Task<bool> is_pinged()
        {
            Ping p = new Ping();
            var result = await p.SendPingAsync(ip, 1000);
            return result.Status == IPStatus.Success;
        }

    }
}
