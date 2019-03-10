using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MyTalkingLib;
namespace PersonInfoDLL
{
    public partial class PersonInfoClass : UserControl
    {
        private int m_iID = -1;
        private WaitCallback m_InsertToServerQueue;
        private string m_strName = "";
        public PersonInfoClass(WaitCallback _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
        }
        public void SetInfo(int _iID = -1,string _strPicturePath = "",string _strName = "")
        {
            m_iID = _iID;
            TextInfo.Text = _strName;
            m_strName = _strName;
            Console.WriteLine("SetInfo");
        }
        
        private void InfoContainer_MouseHover(object sender, EventArgs e)
        {
            Border.BackColor = System.Drawing.Color.DarkOrange;
        }
        private void InfoContainer_MouseLeave(object sender, EventArgs e)
        {
            Border.BackColor = System.Drawing.Color.Transparent;
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.ADDTALKINGPAGE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iID));
            byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(m_strName);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
            ListSendByte.AddRange(strByteArray);

            m_InsertToServerQueue(ListSendByte);
        }

        private void TextInfo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
