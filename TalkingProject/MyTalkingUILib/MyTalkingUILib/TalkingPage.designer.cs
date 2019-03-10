namespace MyTalkingUILib
{
    partial class TalkingPage
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkingPage));
            this.InputWindow = new System.Windows.Forms.TextBox();
            this.CheckButton = new System.Windows.Forms.Button();
            this.FileButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.MessageWindow = new System.Windows.Forms.TableLayoutPanel();
            this.MessagePanel = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MessagePanel)).BeginInit();
            this.SuspendLayout();
            
            // 
            // InputWindow
            // 
            this.InputWindow.Location = new System.Drawing.Point(37, 342);
            this.InputWindow.Margin = new System.Windows.Forms.Padding(2);
            this.InputWindow.Name = "InputWindow";
            this.InputWindow.Size = new System.Drawing.Size(270, 22);
            this.InputWindow.TabIndex = 0;
            // 
            // CheckButton
            // 
            this.CheckButton.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.CheckButton.Location = new System.Drawing.Point(367, 339);
            this.CheckButton.Margin = new System.Windows.Forms.Padding(2);
            this.CheckButton.Name = "CheckButton";
            this.CheckButton.Size = new System.Drawing.Size(56, 25);
            this.CheckButton.TabIndex = 1;
            this.CheckButton.Text = "傳送";
            this.CheckButton.UseVisualStyleBackColor = true;
            this.CheckButton.Click += new System.EventHandler(this.CheckButton_Click);
            // 
            // FileButton
            // 
            this.FileButton.ImageIndex = 0;
            this.FileButton.ImageList = this.imageList1;
            this.FileButton.Location = new System.Drawing.Point(323, 336);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(25, 30);
            this.FileButton.TabIndex = 3;
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "FileImage.png");
            // 
            // MessageWindow
            // 
            this.MessageWindow.AutoScroll = true;
            this.MessageWindow.AutoSize = true;
            this.MessageWindow.BackColor = System.Drawing.SystemColors.Window;
            this.MessageWindow.ColumnCount = 1;
            this.MessageWindow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MessageWindow.Location = new System.Drawing.Point(37, 12);
            this.MessageWindow.MaximumSize = new System.Drawing.Size(400, 318);
            this.MessageWindow.Name = "MessageWindow";
            this.MessageWindow.Padding = new System.Windows.Forms.Padding(3);
            this.MessageWindow.RowCount = 1;
            this.MessageWindow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MessageWindow.Size = new System.Drawing.Size(386, 30);
            this.MessageWindow.TabIndex = 0;
            // 
            // MessagePanel
            // 
            this.MessagePanel.BackColor = System.Drawing.SystemColors.Window;
            this.MessagePanel.Location = new System.Drawing.Point(37, 12);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(386, 318);
            this.MessagePanel.TabIndex = 5;
            this.MessagePanel.TabStop = false;
            // 
            // TalkingPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.MessageWindow);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.FileButton);
            this.Controls.Add(this.CheckButton);
            this.Controls.Add(this.InputWindow);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TalkingPage";
            this.Size = new System.Drawing.Size(464, 499);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MessagePanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputWindow;
        private System.Windows.Forms.Button CheckButton;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel MessageWindow;
        private System.Windows.Forms.PictureBox MessagePanel;
    }
}

