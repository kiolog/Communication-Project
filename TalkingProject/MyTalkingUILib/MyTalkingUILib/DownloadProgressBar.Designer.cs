namespace MyTalkingUILib
{
    partial class DownloadProgressBar
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
            this.DownloadContainer = new System.Windows.Forms.TableLayoutPanel();
            this.Index = new System.Windows.Forms.Label();
            this.FileName = new System.Windows.Forms.Label();
            this.ProgressPercent = new System.Windows.Forms.Label();
            this.DownloadSpeed = new System.Windows.Forms.Label();
            this.FilePosition = new System.Windows.Forms.Label();
            this.FileSize = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.CancelButton = new System.Windows.Forms.PictureBox();
            this.Pause_Resume_Button = new System.Windows.Forms.PictureBox();
            this.DownloadContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CancelButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pause_Resume_Button)).BeginInit();
            this.SuspendLayout();
            // 
            // DownloadContainer
            // 
            this.DownloadContainer.AutoSize = true;
            this.DownloadContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DownloadContainer.BackColor = System.Drawing.Color.Transparent;
            this.DownloadContainer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DownloadContainer.ColumnCount = 9;
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DownloadContainer.Controls.Add(this.Index, 0, 0);
            this.DownloadContainer.Controls.Add(this.FileName, 1, 0);
            this.DownloadContainer.Controls.Add(this.ProgressPercent, 3, 0);
            this.DownloadContainer.Controls.Add(this.DownloadSpeed, 4, 0);
            this.DownloadContainer.Controls.Add(this.FilePosition, 5, 0);
            this.DownloadContainer.Controls.Add(this.FileSize, 6, 0);
            this.DownloadContainer.Controls.Add(this.ProgressBar, 2, 0);
            this.DownloadContainer.Controls.Add(this.CancelButton, 8, 0);
            this.DownloadContainer.Controls.Add(this.Pause_Resume_Button, 7, 0);
            this.DownloadContainer.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.DownloadContainer.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.DownloadContainer.Location = new System.Drawing.Point(3, 3);
            this.DownloadContainer.Name = "DownloadContainer";
            this.DownloadContainer.RowCount = 1;
            this.DownloadContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DownloadContainer.Size = new System.Drawing.Size(470, 31);
            this.DownloadContainer.TabIndex = 7;
            // 
            // Index
            // 
            this.Index.AutoSize = true;
            this.Index.Dock = System.Windows.Forms.DockStyle.Right;
            this.Index.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Index.Location = new System.Drawing.Point(3, 0);
            this.Index.Name = "Index";
            this.Index.Size = new System.Drawing.Size(22, 31);
            this.Index.TabIndex = 0;
            this.Index.Text = "10";
            this.Index.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FileName
            // 
            this.FileName.AutoSize = true;
            this.FileName.Dock = System.Windows.Forms.DockStyle.Right;
            this.FileName.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileName.Location = new System.Drawing.Point(31, 0);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(50, 31);
            this.FileName.TabIndex = 1;
            this.FileName.Text = "321       ";
            this.FileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressPercent
            // 
            this.ProgressPercent.AutoSize = true;
            this.ProgressPercent.Dock = System.Windows.Forms.DockStyle.Right;
            this.ProgressPercent.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ProgressPercent.Location = new System.Drawing.Point(193, 0);
            this.ProgressPercent.Name = "ProgressPercent";
            this.ProgressPercent.Size = new System.Drawing.Size(40, 31);
            this.ProgressPercent.TabIndex = 2;
            this.ProgressPercent.Text = "100%";
            this.ProgressPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DownloadSpeed
            // 
            this.DownloadSpeed.AutoSize = true;
            this.DownloadSpeed.Dock = System.Windows.Forms.DockStyle.Right;
            this.DownloadSpeed.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.DownloadSpeed.Location = new System.Drawing.Point(239, 0);
            this.DownloadSpeed.Name = "DownloadSpeed";
            this.DownloadSpeed.Size = new System.Drawing.Size(58, 31);
            this.DownloadSpeed.TabIndex = 3;
            this.DownloadSpeed.Text = "100MB/s";
            this.DownloadSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FilePosition
            // 
            this.FilePosition.AutoSize = true;
            this.FilePosition.Dock = System.Windows.Forms.DockStyle.Right;
            this.FilePosition.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FilePosition.Location = new System.Drawing.Point(303, 0);
            this.FilePosition.Name = "FilePosition";
            this.FilePosition.Size = new System.Drawing.Size(48, 31);
            this.FilePosition.TabIndex = 4;
            this.FilePosition.Text = "100MB";
            this.FilePosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FileSize
            // 
            this.FileSize.AutoSize = true;
            this.FileSize.Dock = System.Windows.Forms.DockStyle.Right;
            this.FileSize.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileSize.Location = new System.Drawing.Point(357, 0);
            this.FileSize.Name = "FileSize";
            this.FileSize.Size = new System.Drawing.Size(48, 31);
            this.FileSize.TabIndex = 5;
            this.FileSize.Text = "500MB";
            this.FileSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(87, 3);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(100, 23);
            this.ProgressBar.TabIndex = 6;
            this.ProgressBar.Value = 50;
            // 
            // CancelButton
            // 
            this.CancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CancelButton.Image = global::MyTalkingUILib.Properties.Resources.CrossButton;
            this.CancelButton.Location = new System.Drawing.Point(442, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(25, 25);
            this.CancelButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CancelButton.TabIndex = 8;
            this.CancelButton.TabStop = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // Pause_Resume_Button
            // 
            this.Pause_Resume_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Pause_Resume_Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Pause_Resume_Button.Image = global::MyTalkingUILib.Properties.Resources.PauseButton;
            this.Pause_Resume_Button.Location = new System.Drawing.Point(411, 3);
            this.Pause_Resume_Button.Name = "Pause_Resume_Button";
            this.Pause_Resume_Button.Size = new System.Drawing.Size(25, 25);
            this.Pause_Resume_Button.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Pause_Resume_Button.TabIndex = 7;
            this.Pause_Resume_Button.TabStop = false;
            this.Pause_Resume_Button.Click += new System.EventHandler(this.Pause_Resume_Button_Click);
            // 
            // DownloadProgressBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.DownloadContainer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DownloadProgressBar";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(479, 40);
            this.DownloadContainer.ResumeLayout(false);
            this.DownloadContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CancelButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pause_Resume_Button)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel DownloadContainer;
        private System.Windows.Forms.Label Index;
        private System.Windows.Forms.Label FileName;
        private System.Windows.Forms.Label ProgressPercent;
        private System.Windows.Forms.Label DownloadSpeed;
        private System.Windows.Forms.Label FilePosition;
        private System.Windows.Forms.Label FileSize;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.PictureBox Pause_Resume_Button;
        private System.Windows.Forms.PictureBox CancelButton;
    }
}
