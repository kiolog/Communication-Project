namespace TalkClient
{
    partial class LoginForm
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
            this.AccountBox = new System.Windows.Forms.TextBox();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.SignInButton = new System.Windows.Forms.Button();
            this.Message = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // AccountBox
            // 
            this.AccountBox.Location = new System.Drawing.Point(88, 134);
            this.AccountBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.AccountBox.Name = "AccountBox";
            this.AccountBox.Size = new System.Drawing.Size(188, 22);
            this.AccountBox.TabIndex = 0;
            this.AccountBox.TextChanged += new System.EventHandler(this.AccountBox_TextChanged);
            // 
            // PasswordBox
            // 
            this.PasswordBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PasswordBox.Location = new System.Drawing.Point(88, 170);
            this.PasswordBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(188, 22);
            this.PasswordBox.TabIndex = 1;
            this.PasswordBox.TextChanged += new System.EventHandler(this.PasswordBox_TextChanged);
            // 
            // RegisterButton
            // 
            this.RegisterButton.Location = new System.Drawing.Point(88, 240);
            this.RegisterButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(62, 26);
            this.RegisterButton.TabIndex = 2;
            this.RegisterButton.Text = "註冊";
            this.RegisterButton.UseVisualStyleBackColor = true;
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
            // 
            // SignInButton
            // 
            this.SignInButton.Location = new System.Drawing.Point(214, 240);
            this.SignInButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SignInButton.Name = "SignInButton";
            this.SignInButton.Size = new System.Drawing.Size(62, 26);
            this.SignInButton.TabIndex = 3;
            this.SignInButton.Text = "登入";
            this.SignInButton.UseVisualStyleBackColor = true;
            this.SignInButton.Click += new System.EventHandler(this.SignInButton_Click);
            // 
            // Message
            // 
            this.Message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Message.ForeColor = System.Drawing.Color.Red;
            this.Message.Location = new System.Drawing.Point(120, 283);
            this.Message.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.Size = new System.Drawing.Size(122, 15);
            this.Message.TabIndex = 4;
            this.Message.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Message.TextChanged += new System.EventHandler(this.Message_TextChanged);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 341);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.SignInButton);
            this.Controls.Add(this.RegisterButton);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.AccountBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "LoginForm";
            this.Text = "LoginWindow";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AccountBox;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button SignInButton;
        private System.Windows.Forms.TextBox Message;
    }
}