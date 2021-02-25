using System;
using System.IO;
using System.Windows.Forms;

namespace SecretWordGame
{
    public partial class Score : Form
    {
        public Score()
        {
            InitializeComponent();
        }

        private void ScoreLoad(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines("./Results.txt");
            foreach (string line in lines)
            {
                var splited = line.Split(':');
                dgvScore.Rows.Add(splited[1], splited[3]);
            }
        }
    }
}
