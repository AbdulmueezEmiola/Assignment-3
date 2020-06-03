using ClientLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientLibraryTest
{
    [TestClass]
    public class TestConnecting
    {
        int port = 54322;
        IPAddress address = IPAddress.Parse("127.0.0.1");

        [TestInitialize]
        public void TestInitialize()
        {
            TcpListener listener = new TcpListener(address, port);
            listener.Start();
        }
        [TestMethod]
        public void Test()
        {
            ClientConnector client = new ClientConnector(port,address);
            client.Connect();
        }
        [TestMethod]
        public void checkForConnection()
        {
            ClientConnector client = new ClientConnector(port, address);
            client.Connect();
            Assert.IsTrue(client.CheckForConnection());
            ClientConnector client2 = new ClientConnector(port, address);
            Assert.IsFalse(client2.CheckForConnection());
        }
        [TestMethod]
        public void TestWithoutServer()
        {
            try
            {
                ClientConnector client = new ClientConnector(12345, address);
            }catch(Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}",
                            e.GetType(), e.Message));
            }
        }
    }
}
