namespace MyTalkingUILib
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
            this.UploadContainer = new System.Windows.Forms.TableLayoutPanel();
            this.UploadPage = new System.Windows.Forms.TabPage();
            this.FileProgressContainer.SuspendLayout();
            this.UploadPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileProgressContainer
            // 
            this.FileProgressContainer.Controls.Add(this.UploadPage);
            this.FileProgressContainer.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FileProgressContainer.Location = new System.Drawing.Point(0, 0);
            this.FileProgressContainer.Margin = new System.Windows.Forms.Padding(0);
            this.FileProgressContainer.Name = "FileProgressContainer";
            this.FileProgressContainer.SelectedIndex = 0;
            this.FileProgressContainer.Size = new System.Drawing.Size(240, 109);
            this.FileProgressContainer.TabIndex = 0;
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
            this.UploadContainer.Size = new System.Drawing.Size(232, 80);
            this.UploadContainer.TabIndex = 0;
            // 
            // UploadPage
            // 
            this.UploadPage.BackColor = System.Drawing.SystemColors.Control;
            this.UploadPage.Controls.Add(this.UploadContainer);
            this.UploadPage.Location = new System.Drawing.Point(4, 25);
            this.UploadPage.Margin = new System.Windows.Forms.Padding(0);
            this.UploadPage.Name = "UploadPage";
            this.UploadPage.Size = new System.Drawing.Size(232, 80);
            this.UploadPage.TabIndex = 0;
            this.UploadPage.Text = "上傳";
            // 
            // ProgressContainer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.FileProgressContainer);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ProgressContainer";
            this.Size = new System.Drawing.Size(240, 109);
            this.FileProgressContainer.ResumeLayout(false);
            this.UploadPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl FileProgressContainer;
        private System.Windows.Forms.TabPage UploadPage;
        private System.Windows.Forms.TableLayoutPanel UploadContainer;
    }
}
