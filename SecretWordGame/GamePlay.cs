using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGame
{
    public partial class GamePlay : Form
    {
        private string secretWord;
        private string difficulty;
        private string category;

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

        public GamePlay()
        {
            InitializeComponent();
            simpleSound = new SoundPlayer();

            secretWord = "elephant";

            pressedKeys = new List<char>();
            pressedKeys.Add('A');
            pressedKeys.Add('V');
            pressedKeys.Add('R');
            pressedKeys.Add('M');

            listBox1.DataSource = pressedKeys;

            DrawWord();
            DrawKeyBoard();
        }

        private void DrawWord()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel();

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

        private void EnableKeyBoard(bool enable)
        {
            foreach (Control c in this.Controls["keyBoardPanel"].Controls)
            {
                if (enable && !pressedKeys.Contains(c.Text[0]))
                {
                    c.Enabled = true;
                }
                else
                {
                    c.Enabled = false;
                }
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
            simpleSound.PlayLooping();

            btnUnfreeze_Click(null, null);
        }

        private void btnStopSound_Click(object sender, EventArgs e)
        {
            simpleSound.Stop();
        }

        private void GamePlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            simpleSound.Stop();
        }
    }
}
