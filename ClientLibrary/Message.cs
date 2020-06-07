using System;
using System.Collections.Generic;
using System.Text;

namespace ClientLibrary
{
    [Serializable]
    public class Message
    {
        public Message(string name, string msg, DateTime time)
        {
            this.UserName = name;
            this.Msg = msg;
            this.time = time;
        }
        public string UserName { get; set; }
        public string Msg { get; set; }
        public DateTime time { get; set; }
    }
}
