using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SecretWordGame
{
    public partial class GamePlay : Form
    {
        string secretWord;
        string difficulty;
        string category;
        bool clientDisconnected = false;
        int serverResult, clientResult;
        List<char> pressedKeys;
        Network network;

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

        public GamePlay(Network network)
        {
            InitializeComponent();
            serverResult = clientResult = 0;
            this.network = network;
            pressedKeys = new List<char>();

            network.ServerStoped += Network_ServerStoped;
            network.ClientPressedLetter += Network_ClientPressedLetter;
            DrawKeyBoard();
        }

        private void Network_ServerStoped(object sender, EventArgs e)
        {
            MessageBox.Show("Game Ended");

            network.ServerStoped -= Network_ServerStoped;
            clientDisconnected = true;
            // save results before exit
            //this.FormClosing -= GamePlay_FormClosing;
            this.Invoke((MethodInvoker)delegate ()
            {
                this.Close();
            });
        }

        private void GamePlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientDisconnected)
            {
                MessageBox.Show("Client Dissconnected", "Server");
            }
            else
            {
                var res = MessageBox.Show("Are you sure you want to Exit?", "Server", MessageBoxButtons.YesNo);
                if(res == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
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
                    // server win
                    MessageBox.Show("Server Win");
                    ++serverResult;
                    NewGame();
                }
            }
            else
            {
                EnableKeyBoard(false);
            }
        }

        private void Network_ClientPressedLetter(object sender, LetterPressedArgs e)
        {
            char letter = e.Letter;
            pressedKeys.Add(letter);

            if (Guess(letter))
            {
                if (checkFinshed())
                {
                    // client win
                    MessageBox.Show("Client Win");
                    ++clientResult;
                    NewGame();
                }
            }
            else
            {
                EnableKeyBoard(true);
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

        private void NewGame()
        {
            List<string> myList = new List<string>();
            myList.AddRange(new string[] { "Cow", "Rabbit", "Ducks", "Shrimp", "Pig", "Goat", "Crab", "Deer", "Bee", "Sheep", "Fish", "Turkey", "Dove", "Chicken", "Horse" });
            Random r = new Random();
            int index = r.Next(myList.Count);

            secretWord = myList[index];

            pressedKeys.Clear();

            network.Send("newGame", secretWord);
            this.Invoke((MethodInvoker)delegate ()
            {
                this.lblClientResult.Text = clientResult.ToString();
                this.lblServerResult.Text = serverResult.ToString();
                DrawWord();
            });

            EnableKeyBoard(true);
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

        private void GamePlay_Load(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}
