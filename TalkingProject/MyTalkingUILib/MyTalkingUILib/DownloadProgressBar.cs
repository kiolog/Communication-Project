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
namespace MyTalkingUILib
{
    public partial class DownloadProgressBar : UserControl
    {
        public enum EPropertyType
        {
            SETALLPROPERTY,
            SETINDEX,
            UPDATEPROGRESS,
            SETFILESIZE,
        }
        private enum DownloadState
        {
            PLAY,
            PAUSE,
        }
        private int m_iFileID;
        private int m_iMessageID;
        private int m_iOwnerID;
        private WaitCallback m_InsertToServerQueue;
        private long m_lFileSize = 0;
        private long m_lFilePosition = 0;
        private string m_strFileName = "";
        private int m_iIndex = 0;
        private int m_iFileNamePadding = 19;
        private int m_iPercentPadding = 4;
        private int m_iIndexPadding = 2;
        private const int KBSIZE = 1024 * 1024;
        private int m_iCharBaseLength = 0;
        private int m_iPaddingUnitLength = 0;

        private DownloadState m_DownloadState = DownloadState.PLAY;

        public DownloadProgressBar(int _iFileID,int _iMessageID,int _iOwnerID,WaitCallback _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
            m_iFileID = _iFileID;
            m_iMessageID = _iMessageID;
            m_iOwnerID = _iOwnerID;
            m_iCharBaseLength = (TextRenderer.MeasureText(new string('0', 2),FileName.Font)).Width - (TextRenderer.MeasureText(new string('0', 1), FileName.Font)).Width;
            m_iPaddingUnitLength = (TextRenderer.MeasureText(new string(' ',2), FileName.Font)).Width - (TextRenderer.MeasureText(new string(' ', 1), FileName.Font)).Width;
        }
        public void SetProperty(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(SetProperty_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                SetProperty_Implement(_Parameter);
            }
        }
        private void SetProperty_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iType = (int)MyInfo.Dequeue();
            switch ((EPropertyType)iType)
            {
                case EPropertyType.SETALLPROPERTY:
                    SetAllProperty(MyInfo);
                    break;
                case EPropertyType.SETFILESIZE:
                    SetFileSize_Implement(MyInfo);
                    break;
                case EPropertyType.SETINDEX:
                    SetIndex_Implement(MyInfo);
                    break;
                case EPropertyType.UPDATEPROGRESS:
                    UpdateProgress_Implement(MyInfo);
                    break;
            }
        }
        private void SetAllProperty(UIInfoClass _UIInfo)
        {
            int iIndex = (int)_UIInfo.Dequeue();
            string strFileName = (string)_UIInfo.Dequeue();

            SetIndex(iIndex);
            SetFileName(strFileName);
            SetDownloadSpeed(0);
            SetFilePosition(0);
            SetFileSize(0);
            UpdateProcessPercent();
        }
        private void SetIndex_Implement(UIInfoClass _UIInfo)
        {
            int iIndex = (int)_UIInfo.Dequeue();
            SetIndex(iIndex);
        }
        private void UpdateProgress_Implement(UIInfoClass _UIInfo)
        {
            long lDownloadSpeed = (long)_UIInfo.Dequeue();
            long lFilePosition = (long)_UIInfo.Dequeue();
            
            SetDownloadSpeed(lDownloadSpeed);
            SetFilePosition(lFilePosition);
            UpdateProcessPercent();
        }
        private void SetFileSize_Implement(UIInfoClass _UIInfo)
        {
            long lFileSize = (long)_UIInfo.Dequeue();
            SetFileSize(lFileSize);
        }
        private void SetIndex(int _iIndex)
        {
            m_iIndex = _iIndex;
            string strIndex = _iIndex.ToString();
            int iIndexLength = m_iCharBaseLength * m_iIndexPadding;
            int iDifferValue = (iIndexLength - (TextRenderer.MeasureText(strIndex,Index.Font)).Width)/ m_iPaddingUnitLength;
            string strPadding = new string(' ', iDifferValue);
            strIndex += strPadding;
            Index.Text = strIndex;
        }
        private void SetFileName(string _strFileName)
        {
            m_strFileName = _strFileName;
            string strFileName = _strFileName;
            int iLength = m_iCharBaseLength * m_iFileNamePadding;
            int iDifferValue = (iLength - (TextRenderer.MeasureText(strFileName, FileName.Font)).Width) / m_iPaddingUnitLength;
            string strPadding = new string(' ', iDifferValue);
            strFileName += strPadding;
            FileName.Text = strFileName;
        }
        private void UpdateProcessPercent()
        {
            int iFilePercent = 0;
            if(m_lFileSize != 0)
            {
                iFilePercent = (int)Math.Floor(100 * m_lFilePosition / (double)m_lFileSize);
            }
            ProgressBar.Value = iFilePercent;
            ProgressPercent.Text = (iFilePercent.ToString() + "%").PadLeft(m_iPercentPadding, ' ');
        }
        private void SetDownloadSpeed(long _lDownloadSpeed)
        {
            DownloadSpeed.Text = GetTextString(_lDownloadSpeed,1024,new string[] { "B/S","KB/S","MB/S","GB/S"});
        }
        private void SetFilePosition(long _lFilePosition)
        {
            FilePosition.Text = GetTextString(_lFilePosition, 1024,new string[] { "BY","KB","MB","GB"});
            m_lFilePosition = _lFilePosition;
        }
        private void SetFileSize(long _lFileSize)
        {
            FileSize.Text = GetTextString(_lFileSize, 1024, new string[] { "BY", "KB", "MB", "GB" });
            m_lFileSize = _lFileSize;
        }
        private string GetTextString(long _lValue,int _iInterval,string[] _strArray)
        {
            double fValue = 0;
            string strSizeType = "";
            int iListCount = _strArray.Length;
            for(int i=0;i< iListCount; ++i)
            {
                double fDivideValue = _lValue / (double)Math.Pow(_iInterval, i);
                if (fDivideValue > 1)
                {
                    fValue = fDivideValue;
                    strSizeType = _strArray[i];
                }
                else
                {
                    break;
                }
            }
            string strValue = fValue.ToString("0.00");
            //Console.WriteLine("Value : " + strValue);
            int iDividePoint = strValue.IndexOf('.');
            if (iDividePoint >= 3)
            {
                iDividePoint = 3;
            }
            else
            {
                iDividePoint = 4;
            }
            strValue = strValue.Substring(0, iDividePoint) + strSizeType;
            return strValue;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.STOPSENDFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.AddRange(BitConverter.GetBytes(m_iFileID));

            m_InsertToServerQueue(ListSendByte);
        }

        private void Pause_Resume_Button_Click(object sender, EventArgs e)
        {
            switch (m_DownloadState)
            {
                case DownloadState.PLAY:
                    PauseDownloadFile();
                    break;
                case DownloadState.PAUSE:
                    ResumeDownloadFile();
                    break;
            }
        }
        private void PauseDownloadFile()
        {
            m_DownloadState = DownloadState.PAUSE;
            Pause_Resume_Button.Image = MyTalkingUILib.Properties.Resources.ResumeButton;
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.PAUSEDOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iFileID));

            m_InsertToServerQueue(ListSendByte);
        }
        private void ResumeDownloadFile()
        {
            m_DownloadState = DownloadState.PLAY;
            Pause_Resume_Button.Image = MyTalkingUILib.Properties.Resources.PauseButton;
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.RESUMEDOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_iMessageID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_iOwnerID));

            m_InsertToServerQueue(ListSendByte);

        }
    }
}
