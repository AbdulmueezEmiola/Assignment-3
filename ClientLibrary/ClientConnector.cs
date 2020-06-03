using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class ClientConnector
    {
        private int port;
        private IPAddress address;
        private TcpClient client;
        public List<string> messages { get; private set; }
        public ClientConnector(int port, IPAddress address)
        {
            this.port = port;
            this.address = address;
            client = new TcpClient();
            messages = new List<string>();
            messages.Add("Connecting...");
        }
        public void Connect()
        {
            client.Connect(address, port);
        }
        public bool CheckForConnection()
        {
            return client.Connected;
        }
        public async Task SendAsync(string message)
        {
            if (client.Connected)
            {
                await SendMessage(message);
            }
            else
            {
                throw new ClientLibraryException("Client is not connected");
            }
        }
        private async Task SendMessage(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var requestStream = client.GetStream();
            await requestStream.WriteAsync(bytes, 0, bytes.Length);            
        }
        public void ReceiveMessage(CancellationToken token)
        {
            Task task = new Task(async () =>
            {
                while (true)
                {
                    var responseStream = client.GetStream();
                    var bytes = new byte[256];
                    if (responseStream.DataAvailable)
                    {
                        await responseStream.ReadAsync(bytes, 0, bytes.Length);
                        var responseMessage = Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);
                        messages.Add(responseMessage);
                    }
                }
            },token);
            task.Start();
        }
         ~ClientConnector()
        {
            client.Close();
        }
    }
}
