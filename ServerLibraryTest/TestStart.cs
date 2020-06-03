using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ServerLibrary;
using System.Net.Sockets;
using System.Net;

namespace ServerLibraryTest
{
    [TestClass]
    public class TestStart
    {
        int port = 54321;
        IPAddress address = IPAddress.Parse("127.0.0.1");
        [TestMethod]
        public void StartTest()
        {

            ServerConnector connector = new ServerConnector();
            connector.start();
            AddClients();
        }
        private void AddClients()
        {
            for(int i = 0; i < 10; i++)
            {
                TcpClient client = new TcpClient();
                client.Connect(address,port);
            }
        }        

    }
}
