using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    public class ServerConnector
    {

        int port = 54321;
        IPAddress address;
        TcpListener server;
        List<TcpClient> clients;

        public byte[] bytes { get; private set; }

        public ServerConnector()
        {
            port = 54321;
            address = IPAddress.Any;
            server = new TcpListener(address, port);
            clients = new List<TcpClient>();
        }
        public void start()
        {
            server.Start();
            Task task = new Task(async () =>
            {
                while (true)
                {
                    if (server.Pending())
                    {
                        clients.Add(await server.AcceptTcpClientAsync());
                    }
                }
            });
            task.Start();
            ReceiveMessage();
        }
        public void ReceiveMessage()
        {
            Task task = new Task(() =>
            {
                while (true)
                {
                    Parallel.ForEach(clients, async client =>
                    {
                        var requestStream = client.GetStream();
                        if (requestStream.DataAvailable)
                        {
                            byte[] bytes = new byte[256];
                            await requestStream.ReadAsync(bytes, 0, bytes.Length);
                            var requestMessage = Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);
                            Broadcast(requestMessage);
                        }
                    });
                }
            });
            task.Start();
        }
        public void Broadcast(string message)
        {
            foreach(TcpClient client in clients)
            {
                sendToClient(message, client);
            }
        }
        private async void sendToClient(string message,TcpClient client)
        {
            var stream = client.GetStream();
            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
         ~ServerConnector()
        {
            foreach (TcpClient client in clients)
            {
                client.Close();
            }
            server.Stop();            
        }
    }
}
