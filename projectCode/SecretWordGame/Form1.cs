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
using Business;

namespace SecretWordGame
{
    public partial class Form1 : Form
    {
        private string _difficulty;
        private string _category;

        GamePlay gamePlay;
        //Network network;
        NetworkServices network;

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

            Difficulty = "";
            Category = "";

            var ipList = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();

            cbIp.Items.Add("127.0.0.1");
            foreach (var ip in ipList)
            {
                cbIp.Items.Add(ip);
            }

            cbIp.SelectedIndex = 0;
        }

        private void NetworkServerStarted(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void NetworkServerStoped(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            });
        }

        private void NetworkClientConnected(object sender, EventArgs e)
        {
            //MessageBox.Show("Client Connected");

            network.Send("askStart", $"Do tou want to play a game with difficulty {Difficulty} and category {Category}?");
        }

        private void NetworkGameStarted(object sender, EventArgs e)
        {
            gamePlay = new GamePlay(network) { Difficulty = this.Difficulty, Category = this.Category };
            network.Send("runGame");
            this.Invoke((MethodInvoker)delegate ()
            {
                gamePlay.Show();
            });
        }

        private void btnStartClick(object sender, EventArgs e)
        {
            var options = new OptionsDialog();
            if (options.ShowDialog() == DialogResult.OK)
            {
                IPAddress ip = IPAddress.Parse(cbIp.SelectedItem.ToString());
                int port = 2000;

                Difficulty = options.Difficulty;
                Category = options.Category;

                network = new NetworkServices();

                network.ServerStarted += NetworkServerStarted;
                network.ServerStoped += NetworkServerStoped;
                network.GameStarted += NetworkGameStarted;
                network.ClientConnected += NetworkClientConnected;

                _ = network.Start(ip, port);
            }
        }

        private void btnStopClick(object sender, EventArgs e)
        {
            network.Stop();
        }

        private void btnExitClick(object sender, EventArgs e)
        {
            // close thread, connection, etc.
            this.Close();
        }

        private void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
            if (network != null)
            {
                network.Stop();
            }
        }

        private void ScoreToolStripMenuItemClick(object sender, EventArgs e)
        {
            var form = new Score();
            form.ShowDialog();
        }
    }
}
