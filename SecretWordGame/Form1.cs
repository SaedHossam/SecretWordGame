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
    public partial class Form1 : Form
    {
        private string _difficulty;
        private string _category;

        SoundPlayer simpleSound;

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
            simpleSound = new SoundPlayer();

            Difficulty = "Medium";
            Category = "Animals";
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
            //MessageBox.Show($"Start server with Difficulty: {Difficulty} and Category: {Category}", "Starting Game",
            //    MessageBoxButtons.OK,MessageBoxIcon.Information);
            simpleSound.Stop();

            GamePlay game = new GamePlay() { Difficulty = this.Difficulty , Category = this.Category};
            game.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            simpleSound.SoundLocation = @"./414046__tyops__fantasy-gaming-intro.wav";
            simpleSound.PlayLooping();
        }

        private void btnStopSound_Click_1(object sender, EventArgs e)
        {
            simpleSound.Stop();
        }
    }
}
