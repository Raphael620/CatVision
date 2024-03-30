using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using CatVision.Common;
using CatVision.Common.Enum;
using CatVision.Common.Model;

namespace CatVision.Communication
{
    public class TcpServerDevice : IConnector, IPubish, ISubscribe
    {
        // IDevice
        public DeviceInfo DevInfo { get; set; } = new DeviceInfo() { DeviceType = ConnectorProvider.TcpServer.ToString() };
        // IConnector
        public ConnectorInfo ConnInfo { get; set; } = new ConnectorInfo { Provider = ConnectorProvider.TcpServer.ToString() };
        // IPubSub
        public Action<GlobalValModel> Publish { get; set; }
        // private
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        private bool isRunning = false;
        private CancellationTokenSource cancelToken = new CancellationTokenSource();

        public void Init(params object[] paras)
        {
            // clients = new List<TcpClient>();
        }
        public void Uninit(params object[] paras)
        {
            // clients?.Clear();
        }
        public void Connect(params object[] paras)
        {
            if (paras.Length > 0) ConnInfo = (ConnectorInfo)paras[0];
            if (ConnInfo.Port == default(int)) return;
            ConnInfo.SubscribeList.Add(new GlobalValModel(mValueType.mstr, "AnyRecv", "AnyRecv", string.Empty));
            if (string.IsNullOrEmpty(ConnInfo.Ip) || !IPAddress.TryParse(ConnInfo.Ip, out _))
            {
                ConnInfo.Ip = "127.0.0.1";
            }
            listener = new TcpListener(IPAddress.Parse(ConnInfo.Ip), ConnInfo.Port);
            Start();
        }
        public async Task Start()
        {
            listener.Start();
            isRunning = true;
            DevInfo.DState = DeviceState.Connectted;
            System.Diagnostics.Debug.Print("Server started. Listening for connections...");

            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                clients.Add(client);
                System.Diagnostics.Debug.Print(@"New client connected: {0}", (IPEndPoint)client.Client.LocalEndPoint);
                _ = HandleClient(client, cancelToken.Token);
            }
        }
        private async Task HandleClient(TcpClient client, CancellationToken cancelToken)
        {
            try
            {
                using (NetworkStream networkStream = client.GetStream())
                {
                    byte[] buffer = new byte[2048];
                    while (!cancelToken.IsCancellationRequested)
                    {
                        var bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancelToken).ConfigureAwait(false);
                        if (bytesRead == 0) continue; // Client disconnected?

                        HandleRecv(buffer, bytesRead);

                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        System.Diagnostics.Debug.Print($"Received: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
                clients.Remove(client);
                System.Diagnostics.Debug.Print(@"Client disconnected: {0}", (IPEndPoint)client.Client.LocalEndPoint);
            }
        }
        private void HandleRecv(byte[] buf, int len)
        {
            string msg = Encoding.UTF8.GetString(buf, 0, len);
            string[] msgs = msg.Split(' ');
            if (msgs.Length == 2)
            {
                if (ConnInfo.SubscribeList.Select(o => o.address).Contains(msgs[0]))
                {
                    ConnInfo.SubscribeList.FindLast(o => o.address == msgs[0]).SetTypedValue(msgs[1]);
                    // TODO test here
                    Publish?.BeginInvoke(ConnInfo.SubscribeList.FindLast(o => o.address == msgs[0]), null, null);
                }
            }
            else
            {
                ConnInfo.SubscribeList.FindLast(o => o.address == "AnyRecv").value = msg;
                Publish?.BeginInvoke(ConnInfo.SubscribeList.FindLast(o => o.address == "AnyRecv"), null, null);
            }
        }
        public void DisConnect(params object[] paras)
        {
            isRunning = false;
            Publish = null;
            cancelToken.Cancel();
            foreach(TcpClient client in clients) { client.Close(); }
            clients.Clear();
            System.Diagnostics.Debug.Print(@"TcpServer[{0}] stop.", listener.LocalEndpoint);
            if (DevInfo.IsConnected) listener.Stop();
        }

        public void Recieve(ref GlobalValModel val)
        {
            //
        }
        public void SendAsync(GlobalValModel val)
        {
            if (clients.Count < 1) { return; }
            byte[] response = Encoding.ASCII.GetBytes(string.Format(@"{0} {1}", val.address, val.value));
            clients.Last().GetStream().WriteAsync(response, 0, response.Length).GetAwaiter().GetResult();
        }

        public void Subscribe(int timeSpan, GlobalValModel val) { }
        public void DisSubscribe(GlobalValModel val) { }
    }
}
