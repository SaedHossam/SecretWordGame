using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGameClient
{
    public partial class GamePlay : Form
    {
        string secretWord;
        string difficulty;
        string category;

        TcpClient connectedTcpClient;
        bool finished = false;

        bool myturn;

        public string Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
                lblDifficulty.Text = difficulty;
            }
        }

        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                lblCategory.Text = category;
            }
        }

        List<char> pressedKeys;
        SoundPlayer simpleSound;

        public GamePlay(TcpClient client)
        {
            InitializeComponent();
            simpleSound = new SoundPlayer();

            connectedTcpClient = client;

            pressedKeys = new List<char>();

            listBox1.DataSource = pressedKeys;

            DrawKeyBoard();



            //Task.Run(() => Run());
        }

        private void DrawWord()
        {
            if (this.Controls["lettersPanel"] != null)
            {
                this.Controls.Remove(this.Controls["lettersPanel"]);
            }

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Name = "lettersPanel";

            Label[] labels = new Label[secretWord.Length];
            for (int i = 0; i < secretWord.Length; ++i)
            {
                labels[i] = new Label();
                labels[i].Text = "_";
                //labels[i].Text = SecretWord[i].ToString();
                labels[i].Font = new Font("Time New Romans", 18);
                labels[i].Width = 25;
                labels[i].Height = 30;
            }

            panel.Controls.AddRange(labels);

            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.Location = new Point((this.Width - panel.Size.Width) / 2, 50);
            panel.Anchor = AnchorStyles.Top;
            panel.Padding = new Padding(10);
            panel.BackColor = Color.FromArgb(150, Color.White);

            this.Controls.Add(panel);
        }

        private void Letter_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;

            validate(btn.Text[0]);
        }


        private void validate(char letter)
        {
            myturn = Guess(letter);

            if (!myturn)
            {
                EnableKeyBoard(false);
                Task.Run(() => ReceiveLetters());
            }
            Send(pressedKeys.Last().ToString());

            if (checkFinshed())
            {
                ShowResults();
            }
        }

        private void ReceiveLetters()
        {
            string reply;

            while (true)
            {
                reply = Receive();

                if (!SocketConnected())
                {
                    connectedTcpClient.Close();
                    break;
                }


                if (Guess(reply[0]))
                {
                    if (checkFinshed())
                    {
                        break;
                    }
                }
                else
                {
                    EnableKeyBoard(true);
                    MessageBox.Show("your turn");
                    myturn = true;
                    break;
                }
            }

            if (checkFinshed())
            {
                ShowResults();
            }
        }

        private void DrawKeyBoard()
        {
            Button[] buttons = new Button[26];
            FlowLayoutPanel keyBoardPanel = new FlowLayoutPanel();
            keyBoardPanel.Name = "keyBoardPanel";

            for (int i = 0; i < 26; ++i)
            {
                buttons[i] = new Button();
                buttons[i].Text = ((char)(i + 'A')).ToString();
                buttons[i].Width = 65;
                buttons[i].Height = 40;
                buttons[i].Font = new Font("Time New Romans", 16);
                buttons[i].Cursor = Cursors.Hand;
                buttons[i].Click += Letter_Click;
            }
            keyBoardPanel.Width = 520;
            keyBoardPanel.Height = 200;
            keyBoardPanel.Padding = new Padding(10);
            keyBoardPanel.Location = new Point((this.Width - keyBoardPanel.Size.Width) / 2, this.Height - 300);
            keyBoardPanel.Anchor = AnchorStyles.Bottom;
            //keyBoardPanel.BackColor = Color.FromArgb(100, Color.White);

            keyBoardPanel.Controls.AddRange(buttons);
            this.Controls.Add(keyBoardPanel);
        }

        //private void Run()
        //{
        //    while (!finished)
        //    {
        //        string message = Receive();
        //    }
        //}

        private void Send(string message)
        {
            // Convert string message to byte array.                 
            byte[] MessageAsByteArray = Encoding.ASCII.GetBytes(message);
            // Write byte array to socketConnection stream.               
            connectedTcpClient.Client.Send(MessageAsByteArray);
        }

        private string Receive()
        {
            Byte[] bytes = new Byte[1024];
            int length;
            string ret = "";
            try
            {
                if (connectedTcpClient.Connected)
                {
                    length = connectedTcpClient.Client.Receive(bytes);
                    // Get a stream object for reading 				
                    var incommingData = new byte[length];
                    Array.Copy(bytes, 0, incommingData, 0, length);
                    // Convert byte array to string message. 						
                    ret = Encoding.ASCII.GetString(incommingData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return ret;
        }

        private bool Guess(char letter)
        {
            bool match = false;
            pressedKeys.Add(letter);

            if (secretWord.ToUpper().Contains(letter))
            {
                int index = secretWord.ToUpper().IndexOf(letter);
                while (index != -1)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        (this.Controls["lettersPanel"].Controls[index] as Label).Text = letter.ToString();
                    });

                    index = secretWord.ToUpper().IndexOf(letter, index + 1);
                }

                match = true;
            }

            return match;
        }

        private bool checkFinshed()
        {
            bool finished = true;
            for (int i = 0; i < secretWord.Length; ++i)
            {
                if ((this.Controls["lettersPanel"].Controls[i] as Label).Text == "_")
                {
                    finished = false;
                }
            }

            return finished;
        }

        private void ShowResults()
        {
            if (myturn)
            {
                MessageBox.Show("Win");

            }
            else if (!myturn)
            {
                MessageBox.Show("GG");
            }

            DialogResult result = MessageBox.Show("new game", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                NewGame();
            }
            else
            {
                // Close this window
            }
        }

        private void NewGame()
        {
            secretWord = "elephant"; // from server

            this.Invoke((MethodInvoker)delegate
            {
                DrawWord();
            });

            myturn = false;

            pressedKeys.Clear();
            EnableKeyBoard(false);
            Task.Run(() => ReceiveLetters());
        }

        private void EnableKeyBoard(bool enable)
        {
            foreach (Control c in this.Controls["keyBoardPanel"].Controls)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (enable && !pressedKeys.Contains(c.Text[0]))
                    {
                        c.Enabled = true;
                    }
                    else
                    {
                        c.Enabled = false;
                    }
                });
            }
        }

        private void btnFreeze_Click(object sender, EventArgs e)
        {
            EnableKeyBoard(false);
        }

        private void btnUnfreeze_Click(object sender, EventArgs e)
        {
            EnableKeyBoard(true);
        }

        private void GamePlay_Load(object sender, EventArgs e)
        {
            //simpleSound.SoundLocation = @"./410574__yummie__game-background-music-loop-short.wav";
            simpleSound.SoundLocation = @"./489035__michael-db__game-music-01.wav";
            //simpleSound.PlayLooping();

            NewGame();
        }

        private void btnStopSound_Click(object sender, EventArgs e)
        {
            simpleSound.Stop();
        }

        private void GamePlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            simpleSound.Stop();


            var result = MessageBox.Show("Are you sure ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                connectedTcpClient.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        bool SocketConnected() //check whether client is connected server
        {
            bool flag = false;
            try
            {
                bool part1 = connectedTcpClient.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (connectedTcpClient.Available == 0);
                if (part1 && part2)
                {
                    //this.Invoke((MethodInvoker)delegate // cross threads
                    //{
                    //    btnConnect.Enabled = true;
                    //    txtUserName.ReadOnly = false;
                    //});
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er);
            }
            return flag;
        }
    }
}
