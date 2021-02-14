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

        SoundPlayer simpleSound;

        GamePlay gamePlay;

        // TCPListener to listen for incomming TCP connection requests.
        TcpListener tcpListener;

        // Create handle to connected tcp client.
        TcpClient connectedTcpClient;

        CancellationTokenSource source;
        CancellationToken token;

        IPAddress iP;
        int port;

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

            simpleSound = new SoundPlayer();

            iP = new IPAddress(new byte[] { 127, 0, 0, 1 });
            port = 2000;

            // Create listener using ip and port 			
            tcpListener = new TcpListener(iP, port);
        }

        // Use this for initialization
        public async Task Start()
        {
            connectedTcpClient = null;
            source = new CancellationTokenSource();
            token = source.Token;

            // TcpServer task
            await Task.Run(() => ListenForIncommingRequest(), token);
            if (connectedTcpClient != null)
            {
                await Task.Run(() => AskForStart());
            }
            if (gamePlay != null)
            {
                this.StartGame();
            }
        }

        // Handles incomming TcpClient request 	
        private void ListenForIncommingRequest()
        {
            try
            {
                tcpListener.Start();

                Byte[] bytes = new Byte[1024];
                while (tcpListener.Server.IsBound && connectedTcpClient == null && !token.IsCancellationRequested)
                {
                    if (tcpListener.Pending())
                    {
                        connectedTcpClient = tcpListener.AcceptTcpClient();
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (SocketException socketException)
            {
                MessageBox.Show("SocketException " + socketException.ToString());
            }
            finally
            {
                if (connectedTcpClient == null)
                {
                    tcpListener.Stop();
                }
            }
        }

        private void AskForStart()
        {
            string reply;
            Send($"askStart,Play a game with difficulty {Difficulty} and category {Category}?");

            reply = Receive();
            if (reply == "Yes")
            {
                var result = MessageBox.Show("Client Connected, start game?", "Game start", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Send("start");
                    gamePlay = new GamePlay(connectedTcpClient) { Difficulty = this.Difficulty, Category = this.Category };
                }
                else
                {
                    this.StopServer();
                }
            }
            else
            {
                MessageBox.Show("Client refused to play");
                this.StopServer();
            }
        }

        public void StopServer()
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                btnStart.Enabled = true;
            });

            source.Cancel();
        }

        public void StartGame()
        {
            gamePlay.Show();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionsDialog dialog = new OptionsDialog(Difficulty, Category);
            dialog.ShowDialog();

            if (dialog.DialogResult == DialogResult.OK)
            {
                Difficulty = dialog.Difficulty;
                Category = dialog.Category;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // stop server and close any used resource

            this.Close();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //simpleSound.Stop();
            //GamePlay game = new GamePlay() { Difficulty = this.Difficulty , Category = this.Category};
            //game.Show();

            OptionsDialog dialog = new OptionsDialog(Difficulty, Category);
            dialog.ShowDialog();

            if (dialog.DialogResult == DialogResult.OK)
            {
                Difficulty = dialog.Difficulty;
                Category = dialog.Category;

                this.Start();
                btnStart.Enabled = false;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            simpleSound.SoundLocation = @"./414046__tyops__fantasy-gaming-intro.wav";
            //simpleSound.PlayLooping();
        }

        private void btnStopSound_Click_1(object sender, EventArgs e)
        {
            simpleSound.Stop();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopServer();
        }

        public void Send(string message)
        {
            // Convert string message to byte array.                 
            byte[] MessageAsByteArray = Encoding.ASCII.GetBytes(message);
            // Write byte array to socketConnection stream.               
            connectedTcpClient.Client.Send(MessageAsByteArray);
        }

        public string Receive()
        {
            Byte[] bytes = new Byte[1024];
            int length;

            length = connectedTcpClient.Client.Receive(bytes);
            // Get a stream object for reading 				
            var incommingData = new byte[length];
            Array.Copy(bytes, 0, incommingData, 0, length);
            // Convert byte array to string message. 						
            return Encoding.ASCII.GetString(incommingData);
        }
    }
}
