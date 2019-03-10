using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PersonInfoDLL
{
    public partial class PersonInfoClass : UserControl
    {
        private int m_iID = -1;
        private TalkClient.DelInsertToQueue m_InsertToServerQueue;
        public PersonInfoClass(TalkClient.DelInsertToQueue _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
        }
        public void SetInfo(int _iID = -1,string _strPicturePath = "",string _strName = "")
        {
            m_iID = _iID;
            TextInfo.Text = _strName;
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
            ListSendByte.Add((byte)TalkClient.EventType.CREATETALKINGWINDOW);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iID));

            m_InsertToServerQueue(ListSendByte);
        }
    }
}
