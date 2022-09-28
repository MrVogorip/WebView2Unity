using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace WebView2
{
    public class TcpServer
    {
        private const string HostName = "localhost";
        private const int Port = 8052;
        
        private readonly Thread _clientThread;
        private TcpClient _connection;

        public event Action<byte[]> UpdateData;

        public TcpServer()
        {
            _clientThread = new Thread(ListenData) { IsBackground = true };
        }

        public void Start() => 
            _clientThread.Start();

        private void ListenData()
        {
            try
            {
                _connection = new TcpClient(HostName, Port);
                var bytes = new byte[int.MaxValue];
                while (true)
                {
                    using (var stream = _connection.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var data = new byte[length];
                            Array.Copy(bytes, 0, data, 0, length);
                            UpdateData?.Invoke(data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Socket exception {e.Message}");
            }
        }
        
        public void SendMessage(string data)
        {
            if (_connection == null)
                return;

            try
            {
                var stream = _connection.GetStream();
                if (!stream.CanWrite) 
                    return;
                
                var bytes = Encoding.ASCII.GetBytes(data);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Debug.Log($"Socket exception {e.Message}");
            }
        }

        public void Close()
        {
            _clientThread?.Abort();
            _connection?.Close();
        }
    }
}