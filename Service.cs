using System;
using System.Diagnostics;
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
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = new CancellationToken(false);

        private NamedPipeServer server;

        protected override void OnStart(string[] args)
        {
            work = true;
            server?.Dispose();
            cts = new CancellationTokenSource();
            ct = cts.Token;
            NewServer();
            if (!BW_NPipe.IsBusy) BW_NPipe.RunWorkerAsync();
        }


        protected override void OnStop()
        {
            work = false;
            cts.Cancel(true);
            server.ClientConnected -= ServerOnClientConnected;
            server.ClientDisconnected -= ServerOnClientDisconnected;
            BW_NPipe.CancelAsync();
        }

        private Stopwatch sw;
        private void BW_NPipe_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            /*
            var server = new NamedPipeServer("npstest");
            server.StartListen();

            while (work)
            {
                sw.Restart();
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
                sw.Stop();
                if(sw.ElapsedMilliseconds < 1000) Thread.Sleep((int)(1000-sw.ElapsedMilliseconds));
            }
            */
            Console.WriteLine("CALL" + work + "|" + (server == null));

            bool work_now = true;
            string s = "";
            int trys = 99;
            while (server == null)
            {
                NewServer();
                trys--;
                Task.Delay(1000);
                if (trys == 0) throw new Exception("BAD DAY");
            }
            //else
            if (server != null)
            {
                var _1 = server.AwaitSingleMessageAsync<char[]>(ct);
                _1.ConfigureAwait(false);

                var _3 = _1.GetAwaiter();
                NamedPipeMessage<char[]> _4 = null;
                try
                {
                    _4 = _3.GetResult();
                }
                catch (OperationCanceledException OCE)
                {
                    Console.WriteLine(OCE);
                    work_now = false;
                }
                catch (NullReferenceException NRE)
                {
                    Console.WriteLine(NRE);
                    work_now = false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }

                if (work_now && work)
                    Task.Run(async () =>
                    {
                        if (_4 == null) return;

                        s = new string(_4.Message);

                        //fastColoredTextBox1.AppendText("\n" + s + "<<");
                        if (await is_pinged().ConfigureAwait(false)) responce = GetFromNet();
                        else
                        {
                            responce = "NOT PINGED";
                        }

                        await server.SendMessageToAllAsync("\n" + responce + "|>>" + s, ct)
                            .ConfigureAwait(false);
                    }, ct).Wait(10000, ct);
            }
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

        static string ip = Properties.Resources.ip;
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

        private void NewServer()
        {
            server = new NamedPipeServer(Properties.Resources.name_pipe);
            server.StartListen();
            server.ClientConnected += ServerOnClientConnected;
            server.ClientDisconnected += ServerOnClientDisconnected;
        }

        private void ServerOnClientDisconnected(object sender, ClientConnectedArgs e)
        {
            Console.WriteLine("Dis Connected");
        }

        private void ServerOnClientConnected(object sender, ClientConnectedArgs e)
        {
            Console.WriteLine("Connected");
        }

        async Task<bool> is_pinged()
        {
            using (Ping p = new Ping())
            {
                var result = await p.SendPingAsync(ip, 1000).ConfigureAwait(false);
                return result.Status == IPStatus.Success;
            }
        }

        private void BW_NPipe_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (work && !BW_NPipe.IsBusy)
            {
                Task.Delay(1000);
                BW_NPipe.RunWorkerAsync();
            }
        }
    }
}
