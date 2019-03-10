namespace MyTalkingUILib
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
            this.MyOnlineUserPage = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.MyDownloadPage = new System.Windows.Forms.TabPage();
            this.MyOnlineUserPage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FriendMenu
            // 
            this.FriendMenu.AutoScroll = true;
            this.FriendMenu.AutoSize = true;
            this.FriendMenu.ColumnCount = 1;
            this.FriendMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FriendMenu.Location = new System.Drawing.Point(3, 6);
            this.FriendMenu.MaximumSize = new System.Drawing.Size(250, 300);
            this.FriendMenu.Name = "FriendMenu";
            this.FriendMenu.RowCount = 1;
            this.FriendMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.FriendMenu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.FriendMenu.Size = new System.Drawing.Size(250, 86);
            this.FriendMenu.TabIndex = 0;
            // 
            // MyOnlineUserPage
            // 
            this.MyOnlineUserPage.Controls.Add(this.tabPage1);
            this.MyOnlineUserPage.Controls.Add(this.MyDownloadPage);
            this.MyOnlineUserPage.Font = new System.Drawing.Font("Microsoft JhengHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.MyOnlineUserPage.Location = new System.Drawing.Point(1, 2);
            this.MyOnlineUserPage.Name = "MyOnlineUserPage";
            this.MyOnlineUserPage.SelectedIndex = 0;
            this.MyOnlineUserPage.Size = new System.Drawing.Size(600, 482);
            this.MyOnlineUserPage.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.FriendMenu);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(592, 449);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "OnlineUser";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // MyDownloadPage
            // 
            this.MyDownloadPage.Location = new System.Drawing.Point(4, 29);
            this.MyDownloadPage.Name = "MyDownloadPage";
            this.MyDownloadPage.Padding = new System.Windows.Forms.Padding(3);
            this.MyDownloadPage.Size = new System.Drawing.Size(592, 449);
            this.MyDownloadPage.TabIndex = 1;
            this.MyDownloadPage.Text = "Download";
            this.MyDownloadPage.UseVisualStyleBackColor = true;
            // 
            // MenuWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(600, 484);
            this.Controls.Add(this.MyOnlineUserPage);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MenuWindow";
            this.Text = "MenuWindow";
            this.MyOnlineUserPage.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel FriendMenu;
        private System.Windows.Forms.TabControl MyOnlineUserPage;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage MyDownloadPage;
    }
}