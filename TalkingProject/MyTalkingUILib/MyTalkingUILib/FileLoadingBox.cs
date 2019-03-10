using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTalkingUILib;
using MyTalkingLib;
namespace MyTalkingUILib
{
    public partial class FileLoadingBox : UserControl
    {
        private int m_iRoomID = -1;
        private int m_iFileID = -1;
        private int iMaxFileNameLength = 22;
        private WaitCallback m_InsertToServerQueue;
        public FileLoadingBox(int _iRoomID,int _iFileID,string _FileName, WaitCallback _InsertToQueue)
        {
            InitializeComponent();
            LoadingImage.Image = MyTalkingUILib.Properties.Resources.X_Image;
            LoadingBar.Value = 0;
            m_InsertToServerQueue = _InsertToQueue;
            m_iFileID = _iFileID;
            m_iRoomID = _iRoomID;
            SetFileName(_FileName);
        }
        
        public void UpdateProgressBar(int _iValue)
        {
            LoadingBar.Value = _iValue;
            if(_iValue >= 100)
            {
                LoadingImage.Image = MyTalkingUILib.Properties.Resources.Tick;
            }
            //Console.WriteLine("UpdateProgressBar : " + _iValue);
        }
        public void SetFileName(string _strFileName)
        {
            if(_strFileName.Length > iMaxFileNameLength)
            {
                _strFileName = _strFileName.Substring(0, iMaxFileNameLength - 3);
                _strFileName += "...";
            }
            FileName.Text = _strFileName;
        }
        private void LeaveButton_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.STOPSENDFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iRoomID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_iFileID));

            m_InsertToServerQueue(ListSendByte);
        }
        private void FileName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
