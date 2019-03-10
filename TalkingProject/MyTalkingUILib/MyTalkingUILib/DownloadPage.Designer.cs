namespace MyTalkingUILib
{
    partial class DownloadPage
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
            this.SuspendLayout();
            // 
            // DownloadContainer
            // 
            this.DownloadContainer.AutoScroll = true;
            this.DownloadContainer.AutoSize = true;
            this.DownloadContainer.ColumnCount = 1;
            this.DownloadContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 481F));
            this.DownloadContainer.Location = new System.Drawing.Point(10, 10);
            this.DownloadContainer.Margin = new System.Windows.Forms.Padding(2);
            this.DownloadContainer.Name = "DownloadContainer";
            this.DownloadContainer.RowCount = 1;
            this.DownloadContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DownloadContainer.Size = new System.Drawing.Size(564, 20);
            this.DownloadContainer.TabIndex = 2;
            // 
            // DownloadPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(584, 446);
            this.Controls.Add(this.DownloadContainer);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DownloadPage";
            this.Size = new System.Drawing.Size(585, 482);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.TableLayoutPanel DownloadContainer;
    }
}
