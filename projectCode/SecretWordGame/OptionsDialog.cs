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
        CFDB Ent = new CFDB();
        public string Difficulty { get; set; }
        public string Category { get; set; }

        public OptionsDialog()
        {
            InitializeComponent();

            //fetch Difficulty From DB
            var Word = (from D in Ent.WordsGames
                        select D.Difficulty).Distinct();

            foreach (var W in Word)
            {
                cbDifficulty.Items.Add(W);
            }


            //fetch Category From DB
            var Cat = (from C in Ent.WordsGames
                       select C.Category).Distinct();

            foreach (var C in Cat)
            {
                cbCategory.Items.Add(C);
            }

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
