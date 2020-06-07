using ClientLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientLibraryTest
{
    [TestClass]
    public class TestReceiving
    {
        int port = 54323;
        IPAddress address = IPAddress.Parse("127.0.0.1");
        TcpListener listener;
        TcpClient clientOfListener;
        [TestInitialize]
        public async Task Initialize()
        {
            listener = new TcpListener(address, port);
            listener.Start();
            Task task = new Task(async () => {
                clientOfListener = await listener.AcceptTcpClientAsync();
            });
            task.Start();
        }
        [TestMethod]
        public async Task TestReceiveAsync()
        {
            ClientConnector client = new ClientConnector(port, address);
            client.Connect();
            CancellationTokenSource source = new CancellationTokenSource();            
            SendToClientOfListener();
            client.ReceiveMessage(source.Token);
            Thread.Sleep(10000);
            source.Cancel();            
        }
        
        public async Task SendToClientOfListener()
        {
            using (var tcpStream = clientOfListener.GetStream())
            {
                int i;
                for (i = 0; i < 10; i++)
                {
                    string message = $"Message {i}";
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await tcpStream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
