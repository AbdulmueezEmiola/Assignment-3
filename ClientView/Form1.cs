using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClientLibrary;
namespace ClientView
{
    public partial class Form1 : Form
    {
        ClientConnector connector;
        int port = 54321;
        IPAddress address = IPAddress.Parse("127.0.0.1");
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        public Form1()
        {
            connector = new ClientConnector(port, address);
            connector.Connect();
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var source = new BindingSource();
            source.DataSource = connector.messages.Select(x => new { Messages = x }).ToList();
            connector.ReceiveMessage(tokenSource.Token);                       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength != 0)
            {
                connector.SendAsync(textBox1.Text);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var source = new BindingSource();
            source.DataSource = connector.messages.Select(x => new { Messages = x }).ToList();
            dataGridView1.DataSource = source;
        }
    }
}
