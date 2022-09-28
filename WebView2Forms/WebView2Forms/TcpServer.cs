using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebView2Forms
{
    public class TcpServer
    {
        private const int Port = 8052;
        
        private readonly Thread _serverThread;
        private readonly IPAddress _ip = IPAddress.Parse("127.0.0.1");

        private TcpClient _connectedClient;
        private TcpListener _listener;

        public event Action<ClickData> Click;

        public TcpServer()
        {
            _serverThread = new Thread(ListenRequests) { IsBackground = true };
            _serverThread.Start();
        }

        private void ListenRequests()
        {
            try
            {
                _listener = new TcpListener(_ip, Port);
                _listener.Start();
                var bytes = new byte[ushort.MaxValue];
                while (true)
                {
                    using (_connectedClient = _listener.AcceptTcpClient())
                    {
                        using (var stream = _connectedClient.GetStream())
                        {
                            int length;
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var data = new byte[length];
                                Array.Copy(bytes, 0, data, 0, length);
                                var clientMessage = Encoding.ASCII.GetString(data);
                                var clickData = new ClickData(clientMessage);
                                Click?.Invoke(clickData);
                            }
                        }
                    }
                }
            }
            catch (SocketException) { }
        }

        public void SendMessage(byte[] bytes)
        {
            if (_connectedClient == null)
                return;

            try
            {
                var stream = _connectedClient.GetStream();
                if (stream.CanWrite)
                    stream.Write(bytes, 0, bytes.Length);
            }
            catch (SocketException) { }
        }

        public void Close()
        {
            _connectedClient?.Close();
            _listener?.Stop();
            _serverThread?.Abort();
        }
    }
}
