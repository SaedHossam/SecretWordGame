using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGameClient
{
    public partial class Form1 : Form
    {
        IPAddress iP;
        int port;

        private TcpClient socketConnection;
        GamePlay gamePlay;

        public Form1()
        {
            InitializeComponent();

            iP = new IPAddress(new byte[] { 127, 0, 0, 1 });
            port = 2000;

        }

        // Use this for initialization,	Setup socket connection. 	
        public void Start()
        {
            try
            {
                socketConnection = new TcpClient("localhost", port);

                Task.Run(() => ListenForData()).Wait();

                if (gamePlay != null)
                {
                    this.StartGame();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server not started");
            }
        }

        // Runs in background clientReceiveThread; Listens for incomming data. 	
        private void ListenForData()
        {
            try
            {
                while (socketConnection.Connected)
                {
                    string serverMessage = Receive();
                    string[] command = serverMessage.Split(',');
                    switch (command[0])
                    {
                        case ("askStart"):
                            var result = MessageBox.Show(command[1], command[0], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            Send(result.ToString());
                            break;
                        case ("start"):
                            gamePlay = new GamePlay(socketConnection) { Difficulty = "", Category = "" };
                            return;
                            break;
                    }
                }

                MessageBox.Show("Server closed");
            }
            catch (SocketException socketException)
            {
                //   MessageBox.Show("Socket exception: " + socketException);
                if (!socketConnection.Connected)
                {
                    MessageBox.Show("Server Colsed");
                }
            }
        }

        private void Send(string message)
        {
            // Convert string message to byte array.                 
            byte[] MessageAsByteArray = Encoding.ASCII.GetBytes(message);
            // Write byte array to socketConnection stream.               
            socketConnection.Client.Send(MessageAsByteArray);
        }

        private string Receive()
        {
            Byte[] bytes = new Byte[1024];
            int length;

            length = socketConnection.Client.Receive(bytes);
            // Get a stream object for reading 				
            var incommingData = new byte[length];
            Array.Copy(bytes, 0, incommingData, 0, length);
            // Convert byte array to string message. 						
            return Encoding.ASCII.GetString(incommingData);
        }

        public void StartGame()
        {
            gamePlay.Show();
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
