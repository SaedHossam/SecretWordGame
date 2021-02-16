
namespace SecretWordGame
{
    partial class GamePlay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GamePlay));
            this.lblDifficulty = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblClientResult = new System.Windows.Forms.Label();
            this.lblServerResult = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDifficulty
            // 
            this.lblDifficulty.AutoSize = true;
            this.lblDifficulty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDifficulty.Location = new System.Drawing.Point(10, 11);
            this.lblDifficulty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDifficulty.Name = "lblDifficulty";
            this.lblDifficulty.Size = new System.Drawing.Size(61, 17);
            this.lblDifficulty.TabIndex = 2;
            this.lblDifficulty.Text = "Difficulty";
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategory.Location = new System.Drawing.Point(10, 41);
            this.lblCategory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(73, 17);
            this.lblCategory.TabIndex = 3;
            this.lblCategory.Text = "Categeory";
            // 
            // lblClientResult
            // 
            this.lblClientResult.AutoSize = true;
            this.lblClientResult.Font = new System.Drawing.Font("Tahoma", 14F);
            this.lblClientResult.Location = new System.Drawing.Point(100, 41);
            this.lblClientResult.Name = "lblClientResult";
            this.lblClientResult.Size = new System.Drawing.Size(0, 23);
            this.lblClientResult.TabIndex = 8;
            // 
            // lblServerResult
            // 
            this.lblServerResult.AutoSize = true;
            this.lblServerResult.Font = new System.Drawing.Font("Tahoma", 14F);
            this.lblServerResult.Location = new System.Drawing.Point(531, 41);
            this.lblServerResult.Name = "lblServerResult";
            this.lblServerResult.Size = new System.Drawing.Size(0, 23);
            this.lblServerResult.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label1.Location = new System.Drawing.Point(101, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 23);
            this.label1.TabIndex = 10;
            this.label1.Text = "Client Score";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label2.Location = new System.Drawing.Point(519, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "Server Score";
            // 
            // GamePlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(736, 449);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblClientResult);
            this.Controls.Add(this.lblServerResult);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.lblDifficulty);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "GamePlay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GamePlay Server";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GamePlay_FormClosing);
            this.Load += new System.EventHandler(this.GamePlay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDifficulty;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblClientResult;
        private System.Windows.Forms.Label lblServerResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}