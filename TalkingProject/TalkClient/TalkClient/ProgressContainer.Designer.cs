namespace TalkClient
{
    partial class ProgressContainer
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
            this.FileProgressContainer = new System.Windows.Forms.TabControl();
            this.UploadPage = new System.Windows.Forms.TabPage();
            this.UploadContainer = new System.Windows.Forms.TableLayoutPanel();
            this.DownloadPage = new System.Windows.Forms.TabPage();
            this.DownloadContainer = new System.Windows.Forms.TableLayoutPanel();
            this.FileProgressContainer.SuspendLayout();
            this.UploadPage.SuspendLayout();
            this.DownloadPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileProgressContainer
            // 
            this.FileProgressContainer.Controls.Add(this.UploadPage);
            this.FileProgressContainer.Controls.Add(this.DownloadPage);
            this.FileProgressContainer.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileProgressContainer.Location = new System.Drawing.Point(0, 0);
            this.FileProgressContainer.Margin = new System.Windows.Forms.Padding(0);
            this.FileProgressContainer.Name = "FileProgressContainer";
            this.FileProgressContainer.SelectedIndex = 0;
            this.FileProgressContainer.Size = new System.Drawing.Size(240, 109);
            this.FileProgressContainer.TabIndex = 0;
            // 
            // UploadPage
            // 
            this.UploadPage.BackColor = System.Drawing.SystemColors.Control;
            this.UploadPage.Controls.Add(this.UploadContainer);
            this.UploadPage.Location = new System.Drawing.Point(4, 25);
            this.UploadPage.Margin = new System.Windows.Forms.Padding(0);
            this.UploadPage.Name = "UploadPage";
            this.UploadPage.Size = new System.Drawing.Size(232, 71);
            this.UploadPage.TabIndex = 0;
            this.UploadPage.Text = "上傳";
            // 
            // UploadContainer
            // 
            this.UploadContainer.AutoScroll = true;
            this.UploadContainer.ColumnCount = 1;
            this.UploadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.UploadContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UploadContainer.Location = new System.Drawing.Point(0, 0);
            this.UploadContainer.Margin = new System.Windows.Forms.Padding(0);
            this.UploadContainer.MaximumSize = new System.Drawing.Size(262, 80);
            this.UploadContainer.Name = "UploadContainer";
            this.UploadContainer.RowCount = 1;
            this.UploadContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.UploadContainer.Size = new System.Drawing.Size(232, 71);
            this.UploadContainer.TabIndex = 0;
            // 
            // DownloadPage
            // 
            this.DownloadPage.BackColor = System.Drawing.SystemColors.Control;
            this.DownloadPage.Controls.Add(this.DownloadContainer);
            this.DownloadPage.Location = new System.Drawing.Point(4, 25);
            this.DownloadPage.Margin = new System.Windows.Forms.Padding(0);
            this.DownloadPage.Name = "DownloadPage";
            this.DownloadPage.Size = new System.Drawing.Size(232, 80);
            this.DownloadPage.TabIndex = 1;
            this.DownloadPage.Text = "下載";
            // 
            // DownloadContainer
            // 
            this.DownloadContainer.AutoScroll = true;
            this.DownloadContainer.ColumnCount = 1;
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DownloadContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DownloadContainer.Location = new System.Drawing.Point(0, 0);
            this.DownloadContainer.Margin = new System.Windows.Forms.Padding(0);
            this.DownloadContainer.MaximumSize = new System.Drawing.Size(262, 80);
            this.DownloadContainer.Name = "DownloadContainer";
            this.DownloadContainer.RowCount = 1;
            this.DownloadContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DownloadContainer.Size = new System.Drawing.Size(232, 80);
            this.DownloadContainer.TabIndex = 1;
            // 
            // ProgressContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.FileProgressContainer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ProgressContainer";
            this.Size = new System.Drawing.Size(240, 109);
            this.FileProgressContainer.ResumeLayout(false);
            this.UploadPage.ResumeLayout(false);
            this.DownloadPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl FileProgressContainer;
        private System.Windows.Forms.TabPage UploadPage;
        private System.Windows.Forms.TabPage DownloadPage;
        private System.Windows.Forms.TableLayoutPanel UploadContainer;
        private System.Windows.Forms.TableLayoutPanel DownloadContainer;
    }
}
