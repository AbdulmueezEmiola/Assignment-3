using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLibraryTest
{
    [TestClass]
    public class TestReceive
    {
        int port = 54321;
        IPAddress address = IPAddress.Parse("127.0.0.1");
        TcpClient client = new TcpClient();
        TcpClient client2 = new TcpClient();
        ServerConnector connector;
        [TestInitialize]
        public void initialize()
        {
            connector = new ServerConnector();
            connector.start();
            client.Connect(address, port);
            client2.Connect(address, port);
        }
        [TestMethod]
        public async Task ReceiveTestAsync()
        {
            Thread.Sleep(10000);
            connector.ReceiveMessage();
            SendClientAsync();
            Thread.Sleep(10000);
            await ReadClientAsync();
            await ReadClient2Async();
        }
        private async void SendClientAsync()
        {
            string msg = "Hello here";
            var sendStream = client.GetStream();            
            var bytes = Encoding.UTF8.GetBytes(msg);
            var requestMessage = Encoding.UTF8.GetString(bytes).Replace("\0", string.Empty);
            await sendStream.WriteAsync(bytes, 0, bytes.Length);            
        }
        private async Task ReadClientAsync()
        {
            var requestStream = client.GetStream();
            var responseBytes = new Byte[256];
            await requestStream.ReadAsync(responseBytes, 0, responseBytes.Length);
            var responseMessage = Encoding.UTF8.GetString(responseBytes).Replace("\0", string.Empty);
            Assert.AreEqual(responseMessage, "Hello here");            
        }
        private async Task ReadClient2Async()
        {
            using (var requestStream = client2.GetStream())
            {
                var responseBytes = new Byte[256];
                await requestStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                var responseMessage = Encoding.UTF8.GetString(responseBytes).Replace("\0", string.Empty);
                Assert.AreEqual(responseMessage, "Hello here");
            }
        }
    }
}
