using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestUserControl
{
    public partial class MessageBoxClass : UserControl
    {
        public int m_iID;
        public MessageBoxClass(bool _bMine,int _iID)
        {
            InitializeComponent();
            if (_bMine)
            {
                
                MessageContainer.BackgroundImage = TalkClient.Properties.Resources.MyTalkingBoxImage;
                TextBox.BackColor = Color.FromArgb(68, 200, 245);
                TextBox.ForeColor = System.Drawing.SystemColors.Window;
            }
            else
            {
                MessageContainer.BackgroundImage = TalkClient.Properties.Resources.OtherTalkingBoxImage;
                TextBox.BackColor = Color.FromArgb(220, 219, 226);
                TextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            m_iID = _iID;
        }
        
        public void InsertText(string _strInput)
        {
            int iMaxLength = 30;
            string strFinalString = "";
            bool bFirst = true;
            while(_strInput.Length > 0)
            {
                int iDivideLength = iMaxLength;
                if (_strInput.Length < iMaxLength)
                {
                    iDivideLength = _strInput.Length;
                }
                string strDivideString = _strInput.Substring(0, iDivideLength);
                _strInput = _strInput.Substring(iDivideLength);

                if (!bFirst)
                {
                    strFinalString += "\r\n";
                }else
                {
                    bFirst = false;
                }
                strFinalString += strDivideString;
            }

            Size size = TextRenderer.MeasureText(strFinalString, TextBox.Font);
            TextBox.Width = size.Width;
            TextBox.Height = size.Height;
            TextBox.Text = strFinalString;
        }

        private void MessageContainer_Paint(object sender, PaintEventArgs e)
        {
            /*System.Drawing.Drawing2D.GraphicsPath path =
        new System.Drawing.Drawing2D.GraphicsPath();
            

            // Set up and call AddArc, and close the figure.
            Rectangle rect = new Rectangle(20, 20, 50, 100);
            path.AddArc(rect, 0, 180);

            MessageContainer.Region = new System.Drawing.Region(path);*/
            /*System.Drawing.Rectangle newRectangle = MessageContainer.ClientRectangle;




            int radius = 4;
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(MessageContainer.Location, size);
            System.Drawing.Drawing2D.GraphicsPath path =
        new System.Drawing.Drawing2D.GraphicsPath();



            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = newRectangle.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = newRectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = newRectangle.Left;
            path.AddArc(arc, 90, 90);


            MessageContainer.Region = new System.Drawing.Region(path);*/
        }
    }
}
