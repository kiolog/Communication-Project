namespace MyTalkingUILib
{
    partial class TalkingForm
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
            this.TalkingPageContainer = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // TalkingPageContainer
            // 
            this.TalkingPageContainer.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TalkingPageContainer.Location = new System.Drawing.Point(0, 0);
            this.TalkingPageContainer.Margin = new System.Windows.Forms.Padding(0);
            this.TalkingPageContainer.Name = "TalkingPageContainer";
            this.TalkingPageContainer.Padding = new System.Drawing.Point(0, 0);
            this.TalkingPageContainer.SelectedIndex = 0;
            this.TalkingPageContainer.Size = new System.Drawing.Size(633, 655);
            this.TalkingPageContainer.TabIndex = 0;
            this.TalkingPageContainer.SelectedIndexChanged += new System.EventHandler(this.TalkingPageContainer_SelectedIndexChanged);
            // 
            // TalkingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 652);
            this.Controls.Add(this.TalkingPageContainer);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TalkingForm";
            this.Text = "TalkingForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl TalkingPageContainer;
    }
}