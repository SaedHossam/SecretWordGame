using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.Control;

namespace Business
{
    public class UIService
    {
        void draw(Form form)
        {
            
        }

        public void DrawKeyBoard(Form form, EventHandler onClick)
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
                buttons[i].Click += onClick;
            }
            keyBoardPanel.Width = 520;
            keyBoardPanel.Height = 200;
            keyBoardPanel.Padding = new Padding(10);
            keyBoardPanel.Location = new Point((form.Width - keyBoardPanel.Size.Width) / 2, form.Height - 300);
            keyBoardPanel.Anchor = AnchorStyles.Bottom;
            keyBoardPanel.Controls.AddRange(buttons);
            form.Controls.Add(keyBoardPanel);
        }

        public void DrawWord(Form form, string secretWord)
        {
            if (form.Controls["lettersPanel"] != null)
            {
                form.Controls.Remove(form.Controls["lettersPanel"]);
            }

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Name = "lettersPanel";

            Label[] labels = new Label[secretWord.Length];
            for (int i = 0; i < secretWord.Length; ++i)
            {
                labels[i] = new Label();
                labels[i].Text = "_";
                labels[i].Font = new Font("Time New Romans", 18);
                labels[i].Width = 25;
                labels[i].Height = 30;
            }

            panel.Controls.AddRange(labels);

            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.Location = new Point((form.Width - panel.Size.Width) / 2, 50);
            panel.Anchor = AnchorStyles.Top;
            panel.Padding = new Padding(10);
            panel.BackColor = Color.FromArgb(150, Color.White);

            form.Controls.Add(panel);
        }
    }
}
