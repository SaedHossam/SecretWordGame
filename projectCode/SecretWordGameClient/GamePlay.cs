using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Business;

namespace SecretWordGameClient
{
    public partial class GamePlay : Form
    {
        string secretWord;

        List<char> pressedKeys;
        ClientNetworkServices network;
        bool serverDisconnected = false;
        int serverResult, clientResult;
        UIService ui;

        public GamePlay(ClientNetworkServices network)
        {
            InitializeComponent();

            serverResult = clientResult = 0;
            this.network = network;
            pressedKeys = new List<char>();
            ui = new UIService();
            network.Disconnected += NetworkDisconnected;
            network.ServerPressedLetter += NetworkServerPressedLetter;
            network.NewGame += NetworkNewGame;
            network.playagain += Networkplayagain;

            ui.DrawKeyBoard(this, LetterClick);
        }

        private void Networkplayagain(object sender, PlayAgainArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                progressBar1.Visible = false;
            });

            if (e.Response == "no")
            {
                MessageBox.Show("Server refused to play again", "Game ended", MessageBoxButtons.OK);

                network.Stop();
            }
            else
            {
                DialogResult result = MessageBox.Show("Do you want to play Again?", "play again", MessageBoxButtons.YesNo);
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

        private void NetworkNewGame(object sender, Business.NewGamePressedArgs e)
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
                     ui.DrawWord(this, secretWord);
                 });
            }
            else
            {
                ui.DrawWord(this, secretWord);
            }

            EnableKeyBoard(false);
        }

        private void NetworkServerPressedLetter(object sender, Business.LetterPressedArgs e)
        {
            char letter = e.Letter;
            pressedKeys.Add(letter);

            if (Guess(letter))
            {
                if (CheckFinshed())
                {
                    // server win
                    ++serverResult;
                    //MessageBox.Show("Server Win", "Client");
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        progressBar1.Visible = true;
                        EnableKeyBoard(false);
                    });
                }
            }
            else
            {
                EnableKeyBoard(true);
            }
        }

        private void NetworkDisconnected(object sender, System.EventArgs e)
        {

            network.Disconnected -= NetworkDisconnected;
            network.ServerPressedLetter -= NetworkServerPressedLetter;
            network.NewGame -= NetworkNewGame;
            serverDisconnected = true;
            // MessageBox.Show("Game Ended");
            try
            {
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

        private void GamePlayFormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverDisconnected)
            {
                MessageBox.Show($"Server: {serverResult}<==> Client: {clientResult}", "Game Ended", MessageBoxButtons.OK);
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
                    MessageBox.Show($"Server: {serverResult} \n\t Client: {clientResult}", "Game Ended", MessageBoxButtons.OK);

                    network.Disconnected -= NetworkDisconnected;
                    network.ServerPressedLetter -= NetworkServerPressedLetter;
                    network.NewGame -= NetworkNewGame;

                    network.Stop();
                }
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
                    ++clientResult;
                    progressBar1.Visible = true;
                    EnableKeyBoard(false);
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

        private void GamePlay_Load(object sender, EventArgs e)
        {
            Application.OpenForms["Form1"].Hide();
        }

        private void GamePlay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.OpenForms["Form1"].Show();
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