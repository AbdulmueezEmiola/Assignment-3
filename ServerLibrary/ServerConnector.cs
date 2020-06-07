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
                            byte[] bytes = new byte[1024];                            
                            await requestStream.ReadAsync(bytes, 0, bytes.Length);
                            Broadcast(bytes);
                        }
                        
                    });
                }
            });
            task.Start();
        }
        public void Broadcast(byte[] bytes)
        {
            foreach(TcpClient client in clients)
            {
                sendToClient(bytes, client);
            }
        }
        private async void sendToClient(byte[] bytes,TcpClient client)
        {
            var stream = client.GetStream();
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

