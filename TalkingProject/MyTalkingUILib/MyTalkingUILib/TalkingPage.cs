using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using MyTalkingLib;
using MyTalkingUILib;
namespace MyTalkingUILib
{
    public partial class TalkingPage : UserControl
    {
        public enum MessageType
        {
            TEXTMESSAGE = 0,
            FILEMESSAGE = 1,
        }

        private class FileLoadingBoxClass
        {
            public FileLoadingBox m_LoadingBox;
            public bool m_bSuccess;
            public string m_strFileName;

            public FileLoadingBoxClass(FileLoadingBox _LoadingBox,string _strFileName)
            {
                m_LoadingBox = _LoadingBox;
                m_bSuccess = false;
                m_strFileName = _strFileName;
            }
        }

        private int m_iRoomID = -1;
        private int m_iMyAccountID = -1;
        private WaitCallback m_InsertToServerQueue;
        private ProgressContainer m_ProgressContainer;
        private Point m_ProgressContainerPoint = new System.Drawing.Point(37, 370);

        private Dictionary<int, Control> m_DicMessageIDMessagePair = new Dictionary<int, Control>();
        private List<long> m_lListUnixTime = new List<long>();
        public TalkingPage(int _iRoomID,int _iMyAccountID, WaitCallback _CallBack)
        {
            InitializeComponent();
            MessageWindow.Controls.Clear();
            MessageWindow.RowCount = 0;
            MessageWindow.RowStyles.Clear();

            m_iRoomID = _iRoomID;
            m_iMyAccountID = _iMyAccountID;
            m_InsertToServerQueue = _CallBack;
            m_ProgressContainer = new ProgressContainer(m_InsertToServerQueue, _iRoomID);
            m_ProgressContainer.Location = m_ProgressContainerPoint;
            this.Controls.Add(m_ProgressContainer);
            
        }
        public void AddInfo(object _Parameter)
        {
            if (this.MessageWindow.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(AddTalkingInfo);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddTalkingInfo(_Parameter);
            }
        }
        public void AddLoadingFileMain(object _Parameter)
        {
            m_ProgressContainer.AddLoadingFileMain(_Parameter);
        }
        public void RemoveLoadingFileMain(object _Parameter)
        {
            m_ProgressContainer.RemoveLoadingFileMain(_Parameter);
        }
        public void UpdateLoadingBarMain(object _Parameter)
        {
            m_ProgressContainer.UpdateLoadingBarMain(_Parameter);
        }
        

        private void AddTalkingInfo(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            long lUnixTime = (long)MyUIInfo.Dequeue();
            string InputText = (string)MyUIInfo.Dequeue();
            bool bMine = (bool)MyUIInfo.Dequeue();
            int iInfoType = (int)MyUIInfo.Dequeue();
            int iMessageID = (int)MyUIInfo.Dequeue();
            int iOwnerID = (int)MyUIInfo.Dequeue();
            int iInsertIndex = 0;
            lock (m_lListUnixTime)
            {
                iInsertIndex = GetInsertPoint(lUnixTime);
                m_lListUnixTime.Insert(iInsertIndex, lUnixTime);
                
            }
            
            TableLayoutPanel panel = MessageWindow;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Control NewMessageBox;
            if (iInfoType == (int)MessageType.TEXTMESSAGE)
            {
                NewMessageBox = new MessageBoxClass(bMine, iMessageID);
                (NewMessageBox as MessageBoxClass).InsertText(InputText);
            }
            else
            {
                NewMessageBox = new FileMessageBox(iMessageID,m_InsertToServerQueue, iOwnerID, m_iRoomID);
                (NewMessageBox as FileMessageBox).SetInfo(InputText);
            }
            lock (panel.Controls)
            {
                //Console.WriteLine("InsertIndex : " + iInsertIndex + " Message : " + InputText);
                int iListCount = panel.Controls.Count;
                for(int i= 0; i< iListCount; ++i)
                {
                    Control ThisControl = panel.Controls[i];
                    if(panel.GetRow(ThisControl) >= iInsertIndex)
                    {
                        panel.SetRow(ThisControl, panel.GetRow(ThisControl) + 1);
                    }
                }

                panel.Controls.Add(NewMessageBox, 0, iInsertIndex);
                //Console.WriteLine("ActualIndex : " + panel.GetPositionFromControl(NewMessageBox));
            }
            if (!bMine)
            {
                NewMessageBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            }
            else
            {
                NewMessageBox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            }
            //m_DicMessageIDMessagePair.Add(iMessageID, NewMessageBox);
            panel.ScrollControlIntoView(NewMessageBox);

            m_DicMessageIDMessagePair.Add(iMessageID, NewMessageBox);
            


        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                //Do custom stuff
                SendMessage();
                //true if key was processed by control, false otherwise
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        private void CheckButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }
        private void SendMessage()
        {
            List<byte> ListSendByte = new List<byte>();
            string strSendString = InputWindow.Text;
            if (strSendString.Length > 0)
            {
                ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.MESSAGE);
                ListSendByte.AddRange(BitConverter.GetBytes(MyLibFunction.GetNowUnixTime()));
                ListSendByte.AddRange(BitConverter.GetBytes(m_iRoomID));
                byte[] ByteArray = System.Text.Encoding.UTF8.GetBytes(strSendString);
                ListSendByte.AddRange(BitConverter.GetBytes(ByteArray.Length));
                ListSendByte.AddRange(ByteArray);

                m_InsertToServerQueue(ListSendByte);
            }

            ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.SENDUPLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iRoomID));

            m_InsertToServerQueue(ListSendByte);

            InputWindow.Clear();
        }
        
        private void FileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog MyopenFileDialog = new OpenFileDialog();
            MyopenFileDialog.Multiselect = true;
            if (MyopenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] strFileNames = MyopenFileDialog.FileNames;
                int iArrLength = strFileNames.Length;

                for(int i=0;i< iArrLength; ++i)
                {
                    string strFileName = strFileNames[i];
                    List<byte> ListSendByte = new List<byte>();
                    ListSendByte.AddRange(BitConverter.GetBytes(-1));
                    ListSendByte.Add((byte)EventType.UPLOADFILE);
                    ListSendByte.AddRange(BitConverter.GetBytes(m_iRoomID));
                    ListSendByte.AddRange(BitConverter.GetBytes(strFileName.Length));
                    ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(strFileName));

                    m_InsertToServerQueue(ListSendByte);
                }
            }
        }
        private int GetInsertPoint(long _UnixTime)
        {
            int iListCount = m_lListUnixTime.Count;
            int iReturnValue = 0;
            for(int i = iListCount - 1;i >= 0; --i)
            {
                if(m_lListUnixTime[i] < _UnixTime)
                {
                    iReturnValue = i + 1;
                    break;
                }
            }
            return iReturnValue;
        }

        
    }
}
