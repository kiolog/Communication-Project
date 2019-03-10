namespace MyTalkingUILib
{
    partial class MessageBoxClass
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MessageContainer = new System.Windows.Forms.Panel();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.MessageContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // MessageContainer
            // 
            this.MessageContainer.AutoSize = true;
            this.MessageContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessageContainer.BackColor = System.Drawing.Color.White;
            this.MessageContainer.BackgroundImage = global::MyTalkingUILib.Properties.Resources.OtherTalkingBoxImage;
            this.MessageContainer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MessageContainer.Controls.Add(this.TextBox);
            this.MessageContainer.Location = new System.Drawing.Point(0, 0);
            this.MessageContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MessageContainer.Name = "MessageContainer";
            this.MessageContainer.Padding = new System.Windows.Forms.Padding(4, 4, 0, 4);
            this.MessageContainer.Size = new System.Drawing.Size(106, 26);
            this.MessageContainer.TabIndex = 0;
            // 
            // TextBox
            // 
            this.TextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(200)))), ((int)(((byte)(245)))));
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.TextBox.Location = new System.Drawing.Point(4, 4);
            this.TextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TextBox.Multiline = true;
            this.TextBox.Name = "TextBox";
            this.TextBox.ReadOnly = true;
            this.TextBox.Size = new System.Drawing.Size(102, 18);
            this.TextBox.TabIndex = 0;
            this.TextBox.Text = "aaaaaaaaaaaaaaaaaaa";
            // 
            // MessageBoxClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.MessageContainer);
            this.Name = "MessageBoxClass";
            this.Size = new System.Drawing.Size(106, 26);
            this.MessageContainer.ResumeLayout(false);
            this.MessageContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel MessageContainer;
        private System.Windows.Forms.TextBox TextBox;
    }
}
