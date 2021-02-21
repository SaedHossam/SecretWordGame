﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            
            
            ui.DrawKeyBoard(this, LetterClick);
        }

        private void NetworkServerStoped(object sender, EventArgs e)
        {
            MessageBox.Show($"Result\nServer: {serverResult} #### Client: {clientResult}", "Game Ended", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                SaveResultsToFile();
                MessageBox.Show("Client Dissconnected", "Server");
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
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        //private void DrawKeyBoard()
        //{
        //    Button[] buttons = new Button[26];
        //    FlowLayoutPanel keyBoardPanel = new FlowLayoutPanel();
        //    keyBoardPanel.Name = "keyBoardPanel";

        //    for (int i = 0; i < 26; ++i)
        //    {
        //        buttons[i] = new Button();
        //        buttons[i].Text = ((char)(i + 'A')).ToString();
        //        buttons[i].Width = 65;
        //        buttons[i].Height = 40;
        //        buttons[i].Font = new Font("Time New Romans", 16);
        //        buttons[i].Cursor = Cursors.Hand;
        //        buttons[i].Click += Letter_Click;
        //    }
        //    keyBoardPanel.Width = 520;
        //    keyBoardPanel.Height = 200;
        //    keyBoardPanel.Padding = new Padding(10);
        //    keyBoardPanel.Location = new Point((this.Width - keyBoardPanel.Size.Width) / 2, this.Height - 300);
        //    keyBoardPanel.Anchor = AnchorStyles.Bottom;
        //    keyBoardPanel.Controls.AddRange(buttons);
        //    this.Controls.Add(keyBoardPanel);
        //}

        //private void DrawWord()
        //{
        //    if (this.Controls["lettersPanel"] != null)
        //    {
        //        this.Controls.Remove(this.Controls["lettersPanel"]);
        //    }

        //    FlowLayoutPanel panel = new FlowLayoutPanel();
        //    panel.Name = "lettersPanel";

        //    Label[] labels = new Label[secretWord.Length];
        //    for (int i = 0; i < secretWord.Length; ++i)
        //    {
        //        labels[i] = new Label();
        //        labels[i].Text = "_";
        //        labels[i].Font = new Font("Time New Romans", 18);
        //        labels[i].Width = 25;
        //        labels[i].Height = 30;
        //    }

        //    panel.Controls.AddRange(labels);

        //    panel.AutoSize = true;
        //    panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        //    panel.Location = new Point((this.Width - panel.Size.Width) / 2, 50);
        //    panel.Anchor = AnchorStyles.Top;
        //    panel.Padding = new Padding(10);
        //    panel.BackColor = Color.FromArgb(150, Color.White);

        //    this.Controls.Add(panel);
        //}

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

        private void NetworkClientPressedLetter(object sender, LetterPressedArgs e)
        {
            char letter = e.Letter;
            pressedKeys.Add(letter);

            if (Guess(letter))
            {
                if (CheckFinshed())
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
