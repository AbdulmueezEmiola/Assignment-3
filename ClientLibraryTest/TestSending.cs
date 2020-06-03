using ClientLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibraryTest
{
    [TestClass]
    public class TestSending
    {
        int port = 54321;
        IPAddress address = IPAddress.Parse("127.0.0.1");
        TcpListener listener;
        TcpClient clientOfListener;
        [TestInitialize]
        public async Task Initialize()
        {
            listener = new TcpListener(address, port);
            listener.Start();
            Task task = new Task(async () =>{ 
                clientOfListener = await listener.AcceptTcpClientAsync();
            });
            task.Start();
        }
        [TestMethod]
        public async Task SendAsync()
        {
            ClientConnector client = new ClientConnector(port, address);
            client.Connect();
            string msg = "Sending...";
            client.SendAsync(msg);
            string value = await CheckClientOfListener();
            Assert.AreEqual(value, msg);
            msg = msg + "2";
            client.SendAsync(msg);
            value = await CheckClientOfListener();
            Assert.AreEqual(value, msg);
        }        
        public async Task<string> CheckClientOfListener()
        {
            byte[] bytes = new byte[256];
            var tcpStream = clientOfListener.GetStream();
            await tcpStream.ReadAsync(bytes, 0, bytes.Length);
            var requestMessage = Encoding.UTF8.GetString(bytes).Replace("\0",string.Empty);
            return requestMessage;
        }
    }
}
