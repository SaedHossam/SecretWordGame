using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace SecretWordGameClient
{
    public partial class GamePlay : Form
    {
        string secretWord;

        List<char> pressedKeys;
        Network network;
        bool serverDisconnected = false;
        int serverResult, clientResult;
        public GamePlay(Network network)
        {
            InitializeComponent();
            serverResult = clientResult = 0;
            this.network = network;
            pressedKeys = new List<char>();

            network.Disconnected += Network_Disconnected;
            network.ServerPressedLetter += Network_ServerPressedLetter;
            network.NewGame += Network_NewGame;

            DrawKeyBoard();
        }

        private void Network_NewGame(object sender, NewGamePressedArgs e)
        {
            secretWord = e.SecretWord;
            pressedKeys.Clear();
            
            //Invalidate();
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                 {
                     this.lblClientResult.Text = clientResult.ToString();
                     this.lblServerResult.Text = serverResult.ToString();
                     DrawWord();
                 });
            }
            else
            {
                DrawWord();
            }

            EnableKeyBoard(false);
        }

        private void Network_ServerPressedLetter(object sender, LetterPressedArgs e)
        {
            char letter = e.Letter;
            pressedKeys.Add(letter);

            if (Guess(letter))
            {
                if (checkFinshed())
                {
                    // server win
                    ++serverResult;
                    MessageBox.Show("Server Win", "Client");
                }
            }
            else
            {
                EnableKeyBoard(true);
            }
        }

        private void Network_Disconnected(object sender, System.EventArgs e)
        {

            network.Disconnected -= Network_Disconnected;
            network.ServerPressedLetter -= Network_ServerPressedLetter;
            network.NewGame -= Network_NewGame;
            serverDisconnected = true;
            // MessageBox.Show("Game Ended");
            try
            {
                //if (!IsHandleCreated)
                //    this.CreateControl();
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void GamePlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverDisconnected)
            {
                MessageBox.Show($"Server: {serverResult}<==> Client: {clientResult}", "Game Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var res = MessageBox.Show("Are you sure you want to Exit?", "Client", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    MessageBox.Show($"Server: {serverResult}<==> Client: {clientResult}", "Game Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    network.Disconnected -= Network_Disconnected;
                    network.ServerPressedLetter -= Network_ServerPressedLetter;
                    network.NewGame -= Network_NewGame;

                    network.Stop();
                }
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

            char letter = btn.Text[0];
            pressedKeys.Add(letter);
            network.Send("letter", letter.ToString());

            if (Guess(letter))
            {
                if (checkFinshed())
                {
                    // client win show result and wait new word
                    ++clientResult;
                    MessageBox.Show("Client win", "Client");
                }
            }
            else
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    EnableKeyBoard(false);
                });
            }
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

        private void EnableKeyBoard(bool enable)
        {
            if (!IsHandleCreated)
                this.CreateControl();
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
    }
}