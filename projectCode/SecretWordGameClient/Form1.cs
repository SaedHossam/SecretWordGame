using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGameClient
{
    public partial class Form1 : Form
    {
        GamePlay gamePlay;
        Network network;

        public Form1()
        {
            InitializeComponent();

            network = new Network();
            network.Connected += Network_Connected;
            network.Disconnected += Network_Disconnected;
            network.GameStarted += Network_GameStarted;
        }

        private void Network_GameStarted(object sender, EventArgs e)
        {
            gamePlay = new GamePlay(network);

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    gamePlay.Show();
                });
            }
            else
            {
                gamePlay.Show();
            }

        }

        private void Network_Disconnected(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                btnPlay.Enabled = true;
            });
        }

        private void Network_Connected(object sender, EventArgs e)
        {
            //MessageBox.Show("Connected to server");
            this.Invoke((MethodInvoker)delegate ()
            {
                btnPlay.Enabled = false;
            });
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            var result = IPAddress.TryParse(txtIp.Text, out IPAddress ip);
            if (result == true)
            {
                int port = 2000;
                network.Start(ip, port);
            }
            else
            {
                MessageBox.Show("invalid Ip address");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            network.Disconnected -= Network_Disconnected;
            network.Stop();
        }
    }
}
