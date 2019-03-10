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
using System.Threading;
using MyTalkingLib;
namespace MyTalkingUILib
{
    public partial class FileMessageBox : UserControl
    {
        private int m_iID;
        private int m_iOnwerID;
        private int m_iRoomID;
        private int m_iMaxFileNameLength = 24;
        private WaitCallback m_InsertToServerQueue;
        private string m_strFileName;
        public FileMessageBox(int _iID, WaitCallback _CallBack,int _iOwnerID,int _iRoomID)
        {
            InitializeComponent();
            m_iID = _iID;
            m_InsertToServerQueue = _CallBack;
            m_iOnwerID = _iOwnerID;
            m_iRoomID = _iRoomID;
        }

        public void SetInfo(string _strFileName)
        {
            m_strFileName = _strFileName;
            Console.WriteLine("FileMessage SetInfo : " + m_strFileName);
            if (_strFileName.Length > m_iMaxFileNameLength)
            {
                _strFileName = _strFileName.Substring(0, m_iMaxFileNameLength - 3);
                _strFileName += "...";
            }


            Size size = TextRenderer.MeasureText(_strFileName, FileName.Font);
            FileName.Width = size.Width;
            FileName.Height = size.Height;
            FileName.Text = _strFileName;
        }
        public void MessageBox_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.DOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_iOnwerID));
            byte[] strMessageArr = System.Text.Encoding.UTF8.GetBytes(m_strFileName);
            ListSendByte.AddRange(BitConverter.GetBytes(strMessageArr.Length));
            ListSendByte.AddRange(strMessageArr);
            ListSendByte.AddRange(BitConverter.GetBytes((long)0));

            m_InsertToServerQueue(ListSendByte);
        }
        
    }
}
