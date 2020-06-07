using ClientLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
            Message message = new Message("Abdulmueez", "Hello", DateTime.Now);
            
            client.SendAsync(message);
            Message value = await CheckClientOfListener();            
            Assert.AreEqual(value.Msg, message.Msg);
            Assert.AreEqual(value.UserName, message.UserName);
            Assert.AreEqual(value.time, message.time);
            message.Msg += "2";
            client.SendAsync(message);
            value = await CheckClientOfListener();
            Assert.AreEqual(value.Msg, message.Msg);
            Assert.AreEqual(value.UserName, message.UserName);
            Assert.AreEqual(value.time, message.time);
        }
        public async Task<Message> CheckClientOfListener()
        {
            byte[] bytes = new byte[256];
            var tcpStream = clientOfListener.GetStream();
            await tcpStream.ReadAsync(bytes, 0, bytes.Length);
            var requestMessage = ToMessageObject(bytes);
            return requestMessage;
        }
        private byte[] ToByteArray(Message msg)
        {
            if (msg == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, msg);
                return ms.ToArray();
            }
        }
        private Message ToMessageObject(Byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                Message msg = (Message)bf.Deserialize(ms);
                return msg;
            }
        }
    }
}
