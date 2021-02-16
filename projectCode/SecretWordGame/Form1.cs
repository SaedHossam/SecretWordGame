using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGame
{
    public partial class Form1 : Form
    {
        private string _difficulty;
        private string _category;

        GamePlay gamePlay;
        Network network;

        public string Difficulty
        {
            get
            {
                return _difficulty;
            }
            set
            {
                _difficulty = value;
                tsslDifficulty.Text = _difficulty;
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
                tsslCategory.Text = _category;
            }
        }

        public Form1()
        {
            InitializeComponent();

            Difficulty = "Medium";
            Category = "Animals";

            IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            int port = 2000;

            network = new Network(ip, port);
            network.ServerStarted += Network_ServerStarted;
            network.ServerStoped += Network_ServerStoped;
            network.GameStarted += Network_GameStarted;
            network.ClientConnected += Network_ClientConnected;
            network.ClientDisconnected += Network_ClientDisconnected;
        }

        private void Network_ServerStarted(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void Network_ServerStoped(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            });
        }

        private void Network_ClientConnected(object sender, EventArgs e)
        {
            //MessageBox.Show("Client Connected");

            network.Send("askStart", $"Do tou want to play a game with difficulty {Difficulty} and category {Category}?");
        }

        private void Network_ClientDisconnected(object sender, EventArgs e)
        {
            MessageBox.Show("Client Disconnected");
        }

        private void Network_GameStarted(object sender, EventArgs e)
        {
            gamePlay = new GamePlay(network) { Difficulty = this.Difficulty, Category = this.Category };
            network.Send("runGame");
            this.Invoke((MethodInvoker)delegate ()
            {
                gamePlay.Show();
            });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            network.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            network.Stop();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            network.Stop();
        }
    }
}
