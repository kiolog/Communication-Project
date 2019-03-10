namespace FileMessage
{
    partial class FileMessageBox
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
            this.FileName = new System.Windows.Forms.TextBox();
            this.FileImage = new System.Windows.Forms.PictureBox();
            this.MessageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FileImage)).BeginInit();
            this.SuspendLayout();
            // 
            // MessageContainer
            // 
            this.MessageContainer.AutoSize = true;
            this.MessageContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessageContainer.BackColor = System.Drawing.SystemColors.Window;
            this.MessageContainer.Controls.Add(this.FileName);
            this.MessageContainer.Controls.Add(this.FileImage);
            this.MessageContainer.Location = new System.Drawing.Point(0, 0);
            this.MessageContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MessageContainer.Name = "MessageContainer";
            this.MessageContainer.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MessageContainer.Size = new System.Drawing.Size(109, 43);
            this.MessageContainer.TabIndex = 0;
            // 
            // FileName
            // 
            this.FileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FileName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FileName.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileName.Location = new System.Drawing.Point(45, 12);
            this.FileName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(56, 22);
            this.FileName.TabIndex = 1;
            this.FileName.Text = "File.txt";
            this.FileName.Click += new System.EventHandler(this.MessageBox_Click);
            // 
            // FileImage
            // 
            this.FileImage.Image = global::TalkClient.Properties.Resources.FileMessageBox;
            this.FileImage.Location = new System.Drawing.Point(11, 10);
            this.FileImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FileImage.Name = "FileImage";
            this.FileImage.Size = new System.Drawing.Size(27, 25);
            this.FileImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.FileImage.TabIndex = 0;
            this.FileImage.TabStop = false;
            // 
            // FileMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.MessageContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FileMessageBox";
            this.Size = new System.Drawing.Size(109, 43);
            this.MessageContainer.ResumeLayout(false);
            this.MessageContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FileImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel MessageContainer;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.PictureBox FileImage;
    }
}
