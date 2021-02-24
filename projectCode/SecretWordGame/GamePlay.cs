using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Business;

namespace SecretWordGame
{
    public partial class GamePlay : Form
    {
        string secretWord;
        string difficulty;
        string category;
        bool formIsClosing = false;
        bool clientDisconnected = false;
        int serverResult, clientResult;
        List<char> pressedKeys;
        //Network network;
        NetworkServices network;
        UIService ui;

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

        public GamePlay(NetworkServices network)
        {
            InitializeComponent();
            serverResult = clientResult = 0;
            this.network = network;
            pressedKeys = new List<char>();
            ui = new UIService();
            network.ServerStoped += NetworkServerStoped;
            network.ClientPressedLetter += NetworkClientPressedLetter;
            network.ClientPlayAgainResponse += NetworkClientPlayAgainResponse;

            ui.DrawKeyBoard(this, LetterClick);
        }

        private void NetworkClientPlayAgainResponse(object sender, PlayAgainArgs e)
        {
            if (e.Response == "yes")
            {
                NewGame();
            }
            else
            {
                MessageBox.Show("Client refused to play again", "Game ended");
                network.Stop();
            }
        }

        private void NetworkServerStoped(object sender, EventArgs e)
        {
            network.ServerStoped -= NetworkServerStoped;
            clientDisconnected = true;
            if (!formIsClosing)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.Close();
                });
            }
        }

        private void GamePlayFormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientDisconnected)
            {
                MessageBox.Show("Client Dissconnected", "Server");
                SaveResultsToFile();
                MessageBox.Show($"Result\nServer: {serverResult} #### Client: {clientResult}", "Game Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var res = MessageBox.Show("Are you sure you want to Exit?", "Server", MessageBoxButtons.YesNo);
                if (res == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    SaveResultsToFile();
                    MessageBox.Show($"Result\nServer: {serverResult} #### Client: {clientResult}", "Game Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    formIsClosing = true;
                    network.Stop();
                }
            }
        }

        private void SaveResultsToFile()
        {
            try
            {
                StreamWriter writer = File.AppendText("Results.txt");
                writer.WriteLine($"Server: {serverResult} : Client: {clientResult}");
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        private void LetterClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;

            char letter = btn.Text[0];
            pressedKeys.Add(letter);
            network.Send("letter", letter.ToString());

            if (Guess(letter))
            {
                if (CheckFinshed())
                {
                    // server win
                    //MessageBox.Show("Server Win");
                    ++serverResult;

                    DialogResult result = MessageBox.Show("Do you want to play Again?", "play again", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        network.Send("playAgain", "yes");
                    }
                    else
                    {
                        network.Send("playAgain", "no");
                        network.Stop();
                    }
                }
            }
            else
            {
                EnableKeyBoard(false);
            }
        }

        private void NetworkClientPressedLetter(object sender, LetterPressedArgs e)
        {
            char letter = e.Letter;
            pressedKeys.Add(letter);

            if (Guess(letter))
            {
                if (CheckFinshed())
                {
                    // client win
                    //MessageBox.Show("Client Win");
                    ++clientResult;

                    DialogResult result = MessageBox.Show("Do you want to play Again?", "play again", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        network.Send("playAgain", "yes");
                    }
                    else
                    {
                        network.Send("playAgain", "no");
                        network.Stop();
                    }
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

        private bool CheckFinshed()
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
            CFDB ENT = new CFDB();
            var Sec = (from S in ENT.WordsGames
                       where S.Difficulty == this.Difficulty
                            && S.Category == this.category
                       orderby Guid.NewGuid()
                       select S.Word).FirstOrDefault();
            secretWord = Sec;
            pressedKeys.Clear();

            network.Send("newGame", secretWord);
            this.Invoke((MethodInvoker)delegate ()
            {
                this.lblClientResult.Text = clientResult.ToString();
                this.lblServerResult.Text = serverResult.ToString();
                ui.DrawWord(this, secretWord);
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

        private void GamePlayLoad(object sender, EventArgs e)
        {
            NewGame();
        }
    }
}
