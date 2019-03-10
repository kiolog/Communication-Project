namespace FileMessageClass
{
    partial class FileLoadingBox
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
            this.FileName = new System.Windows.Forms.TextBox();
            this.LoadingBar = new System.Windows.Forms.ProgressBar();
            this.LoadingContainer = new System.Windows.Forms.Panel();
            this.LeaveButton = new System.Windows.Forms.PictureBox();
            this.FileImage = new System.Windows.Forms.PictureBox();
            this.LoadingImage = new System.Windows.Forms.PictureBox();
            this.LoadingContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeaveButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadingImage)).BeginInit();
            this.SuspendLayout();
            // 
            // FileName
            // 
            this.FileName.BackColor = System.Drawing.SystemColors.Window;
            this.FileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FileName.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileName.Location = new System.Drawing.Point(46, 13);
            this.FileName.Margin = new System.Windows.Forms.Padding(0);
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Size = new System.Drawing.Size(139, 16);
            this.FileName.TabIndex = 1;
            this.FileName.Text = "000000000000000000.txt";
            this.FileName.TextChanged += new System.EventHandler(this.FileName_TextChanged);
            // 
            // LoadingBar
            // 
            this.LoadingBar.Location = new System.Drawing.Point(8, 36);
            this.LoadingBar.Name = "LoadingBar";
            this.LoadingBar.Size = new System.Drawing.Size(196, 23);
            this.LoadingBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.LoadingBar.TabIndex = 3;
            this.LoadingBar.Value = 10;
            // 
            // LoadingContainer
            // 
            this.LoadingContainer.AutoSize = true;
            this.LoadingContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LoadingContainer.BackColor = System.Drawing.Color.White;
            this.LoadingContainer.Controls.Add(this.LeaveButton);
            this.LoadingContainer.Controls.Add(this.LoadingBar);
            this.LoadingContainer.Controls.Add(this.FileName);
            this.LoadingContainer.Controls.Add(this.FileImage);
            this.LoadingContainer.Controls.Add(this.LoadingImage);
            this.LoadingContainer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadingContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoadingContainer.Location = new System.Drawing.Point(0, 0);
            this.LoadingContainer.Margin = new System.Windows.Forms.Padding(0);
            this.LoadingContainer.Name = "LoadingContainer";
            this.LoadingContainer.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.LoadingContainer.Size = new System.Drawing.Size(212, 67);
            this.LoadingContainer.TabIndex = 1;
            // 
            // LeaveButton
            // 
            this.LeaveButton.Image = global::TalkClient.Properties.Resources.LoadingX;
            this.LeaveButton.Location = new System.Drawing.Point(189, 2);
            this.LeaveButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.LeaveButton.Name = "LeaveButton";
            this.LeaveButton.Size = new System.Drawing.Size(16, 18);
            this.LeaveButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LeaveButton.TabIndex = 4;
            this.LeaveButton.TabStop = false;
            // 
            // FileImage
            // 
            this.FileImage.Image = global::TalkClient.Properties.Resources.FileMessageBox;
            this.FileImage.Location = new System.Drawing.Point(23, 10);
            this.FileImage.Name = "FileImage";
            this.FileImage.Size = new System.Drawing.Size(20, 20);
            this.FileImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.FileImage.TabIndex = 0;
            this.FileImage.TabStop = false;
            // 
            // LoadingImage
            // 
            this.LoadingImage.Image = global::TalkClient.Properties.Resources.Tick;
            this.LoadingImage.Location = new System.Drawing.Point(8, 14);
            this.LoadingImage.Name = "LoadingImage";
            this.LoadingImage.Size = new System.Drawing.Size(14, 14);
            this.LoadingImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LoadingImage.TabIndex = 2;
            this.LoadingImage.TabStop = false;
            // 
            // FileLoadingBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.LoadingContainer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FileLoadingBox";
            this.Size = new System.Drawing.Size(212, 67);
            this.LoadingContainer.ResumeLayout(false);
            this.LoadingContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeaveButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FileImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadingImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox FileImage;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.PictureBox LoadingImage;
        private System.Windows.Forms.ProgressBar LoadingBar;
        private System.Windows.Forms.Panel LoadingContainer;
        private System.Windows.Forms.PictureBox LeaveButton;
    }
}
