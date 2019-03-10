namespace PersonInfoDLL
{
    partial class PersonInfoClass
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
            this.components = new System.ComponentModel.Container();
            this.Border = new System.Windows.Forms.Panel();
            this.InfoContainer = new System.Windows.Forms.Panel();
            this.TextInfo = new System.Windows.Forms.TextBox();
            this.PictureInfo = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Border.SuspendLayout();
            this.InfoContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // Border
            // 
            this.Border.AutoSize = true;
            this.Border.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Border.Controls.Add(this.InfoContainer);
            this.Border.Location = new System.Drawing.Point(0, 0);
            this.Border.Name = "Border";
            this.Border.Padding = new System.Windows.Forms.Padding(3);
            this.Border.Size = new System.Drawing.Size(152, 49);
            this.Border.TabIndex = 0;
            // 
            // InfoContainer
            // 
            this.InfoContainer.AutoSize = true;
            this.InfoContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.InfoContainer.BackColor = System.Drawing.SystemColors.ControlLight;
            this.InfoContainer.Controls.Add(this.TextInfo);
            this.InfoContainer.Controls.Add(this.PictureInfo);
            this.InfoContainer.Location = new System.Drawing.Point(3, 3);
            this.InfoContainer.Margin = new System.Windows.Forms.Padding(0);
            this.InfoContainer.Name = "InfoContainer";
            this.InfoContainer.Padding = new System.Windows.Forms.Padding(3);
            this.InfoContainer.Size = new System.Drawing.Size(146, 43);
            this.InfoContainer.TabIndex = 0;
            // 
            // TextInfo
            // 
            this.TextInfo.Font = new System.Drawing.Font("PMingLiU", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TextInfo.Location = new System.Drawing.Point(40, 7);
            this.TextInfo.Name = "TextInfo";
            this.TextInfo.ReadOnly = true;
            this.TextInfo.Size = new System.Drawing.Size(100, 30);
            this.TextInfo.TabIndex = 1;
            // 
            // PictureInfo
            // 
            this.PictureInfo.BackColor = System.Drawing.Color.White;
            this.PictureInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PictureInfo.Location = new System.Drawing.Point(7, 7);
            this.PictureInfo.Margin = new System.Windows.Forms.Padding(0);
            this.PictureInfo.Name = "PictureInfo";
            this.PictureInfo.Size = new System.Drawing.Size(30, 30);
            this.PictureInfo.TabIndex = 0;
            this.PictureInfo.TabStop = false;
            this.PictureInfo.MouseEnter += new System.EventHandler(this.InfoContainer_MouseHover);
            this.PictureInfo.MouseLeave += new System.EventHandler(this.InfoContainer_MouseLeave);
            this.PictureInfo.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // PersonInfoClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.Border);
            this.Name = "PersonInfoClass";
            this.Size = new System.Drawing.Size(155, 52);
            this.Border.ResumeLayout(false);
            this.Border.PerformLayout();
            this.InfoContainer.ResumeLayout(false);
            this.InfoContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Border;
        private System.Windows.Forms.Panel InfoContainer;
        private System.Windows.Forms.TextBox TextInfo;
        private System.Windows.Forms.PictureBox PictureInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
