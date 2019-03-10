using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTalkingUILib;
namespace MyTalkingUILib
{
    public partial class MessageBoxClass : UserControl
    {
        public int m_iID;
        public MessageBoxClass(bool _bMine,int _iID)
        {
            InitializeComponent();
            if (_bMine)
            {
                
                MessageContainer.BackgroundImage = MyTalkingUILib.Properties.Resources.MyTalkingBoxImage;
                TextBox.BackColor = Color.FromArgb(68, 200, 245);
                TextBox.ForeColor = System.Drawing.SystemColors.Window;
            }
            else
            {
                MessageContainer.BackgroundImage = MyTalkingUILib.Properties.Resources.OtherTalkingBoxImage;
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

        
    }
}
