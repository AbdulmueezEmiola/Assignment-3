using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        public List<Message> messages { get; private set; }
        public ClientConnector(int port, IPAddress address)
        {
            this.port = port;
            this.address = address;
            client = new TcpClient();
            messages = new List<Message>();
            messages.Add(new Message("", "Connecting.....", DateTime.Now));
        }
        public void Connect()
        {
            client.Connect(address, port);
        }
        public bool CheckForConnection()
        {
            return client.Connected;
        }
        public async Task SendAsync(Message message)
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
        private async Task SendMessage(Message message)
        {
            var bytes = ToByteArray(message);
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
                    var bytes = new byte[1024];
                    if (responseStream.DataAvailable)
                    {
                        await responseStream.ReadAsync(bytes, 0, bytes.Length);
                        var responseMessage =ToMessageObject(bytes);
                        messages.Add(responseMessage);
                    }
                }
            },token);
            task.Start();
        }
        private byte[] ToByteArray(Message msg)
        {
           if(msg == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, msg);                
                return ms.ToArray();
            }
        }
        private Message ToMessageObject(Byte[] bytes)
        {
            if(bytes == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                Message msg = (Message)bf.Deserialize(ms);
                return msg;
            }
        }
         ~ClientConnector()
        {
            client.Close();
        }
    }
}
