using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGame
{
    public partial class OptionsDialog : Form
    {
        public string Difficulty { get; set; }
        public string Category { get; set; }

        public OptionsDialog(string difficulty, string category)
        {
            InitializeComponent();

            this.Difficulty = difficulty;
            this.Category = category;

            cbDifficulty.Items.AddRange(new object[] {"Easy", "Medium", "Hard"});
            cbCategory.Items.AddRange(new object[] {"Animals", "Cities", "Food"});

            cbDifficulty.SelectedIndex = cbDifficulty.Items.IndexOf(Difficulty);
            cbCategory.SelectedIndex = cbCategory.Items.IndexOf(Category);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Difficulty = (string)cbDifficulty.SelectedItem;
            Category = (string)cbCategory.SelectedItem;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
