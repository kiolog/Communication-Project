namespace TalkClient
{
    partial class MenuWindow
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
            this.FriendMenu = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FriendMenu
            // 
            this.FriendMenu.AutoScroll = true;
            this.FriendMenu.AutoSize = true;
            this.FriendMenu.ColumnCount = 1;
            this.FriendMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FriendMenu.Location = new System.Drawing.Point(1, 3);
            this.FriendMenu.MaximumSize = new System.Drawing.Size(250, 300);
            this.FriendMenu.Name = "FriendMenu";
            this.FriendMenu.RowCount = 1;
            this.FriendMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FriendMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.FriendMenu.Size = new System.Drawing.Size(250, 86);
            this.FriendMenu.TabIndex = 0;
            this.FriendMenu.Paint += new System.Windows.Forms.PaintEventHandler(this.FriendMenu_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(269, 291);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MenuWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(392, 353);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FriendMenu);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MenuWindow";
            this.Text = "MenuWindow";
            this.Load += new System.EventHandler(this.MenuWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel FriendMenu;
        private System.Windows.Forms.Button button1;
    }
}