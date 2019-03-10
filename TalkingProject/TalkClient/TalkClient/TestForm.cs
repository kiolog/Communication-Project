using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkClient
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }
        private bool bAnchorLeft = true;
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*TableLayoutPanel panel = tableLayoutPanel1;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            
            FileMessageClass.FileLoadingBox NewMessageBox = new FileMessageClass.FileLoadingBox("Filefile.txt",);
            panel.Controls.Add(NewMessageBox, 0, panel.RowCount-1);*/
           
        }
    }
}
