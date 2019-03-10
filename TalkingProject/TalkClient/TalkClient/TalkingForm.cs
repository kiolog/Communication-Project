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
namespace TalkClient
{
    public partial class TalkingForm : Form
    {
        public enum MessageType
        {
            TEXTMESSAGE = 0,
            FILEMESSAGE = 1,
        }

        private class FileLoadingBoxClass
        {
            public FileMessageClass.FileLoadingBox m_LoadingBox;
            public bool m_bSuccess;
            public string m_strFileName;

            public FileLoadingBoxClass(FileMessageClass.FileLoadingBox _LoadingBox,string _strFileName)
            {
                m_LoadingBox = _LoadingBox;
                m_bSuccess = false;
                m_strFileName = _strFileName;
            }
        }

        private int m_iID = -1;
        private TalkClient.DelInsertToQueue m_InsertToServerQueue;
        private Dictionary<int, FileLoadingBoxClass> m_DicLoadingBoxPair = new Dictionary<int, FileLoadingBoxClass>();
        private ProgressContainer m_ProgressContainer;
        private Point m_ProgressContainerPoint = new System.Drawing.Point(37, 370);
        public TalkingForm(int _iID, TalkClient.DelInsertToQueue _CallBack)
        {
            InitializeComponent();
            m_iID = _iID;
            m_InsertToServerQueue = _CallBack;
            m_ProgressContainer = new ProgressContainer(m_InsertToServerQueue);
            m_ProgressContainer.Location = m_ProgressContainerPoint;
            this.Controls.Add(m_ProgressContainer);
        }
        public void AddInfo(object _Parameter)
        {
            if (this.MessageWindow.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(AddTalkingInfo);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddTalkingInfo(_Parameter);
            }
        }
        public bool IsAllUploadSuccess()
        {
            return m_ProgressContainer.IsAllUploadSuccess();
        }
        public List<UIInfoClass> GetAllFileInfo(int _iType)
        {
            return m_ProgressContainer.GetAllFileInfo(_iType);
        }
        public void AddLoadingFileMain(object _Parameter)
        {
            m_ProgressContainer.AddLoadingFileMain(_Parameter);
        }
        public void SetLoadingBoxSuccessMain(object _Parameter)
        {
            m_ProgressContainer.SetLoadingBoxSuccessMain(_Parameter);
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
            string InputText = MyUIInfo.m_strMessage;
            bool bMine = MyUIInfo.m_bMine;
            int iInfoType = MyUIInfo.m_iValue;
            int iMessageID = MyUIInfo.m_iID;
            int iOwnerID = MyUIInfo.m_iOwnerID;
            TableLayoutPanel panel = MessageWindow;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Control NewMessageBox;
            if (iInfoType == (int)MessageType.TEXTMESSAGE)
            {
                NewMessageBox = new TestUserControl.MessageBoxClass(bMine, iMessageID);
                (NewMessageBox as TestUserControl.MessageBoxClass).InsertText(InputText);
            }
            else
            {
                NewMessageBox = new FileMessage.FileMessageBox(iMessageID,m_InsertToServerQueue, iOwnerID, m_iID);
                (NewMessageBox as FileMessage.FileMessageBox).SetInfo(InputText);
            }
            panel.Controls.Add(NewMessageBox, 0, panel.RowCount - 1);
            if (!bMine)
            {
                NewMessageBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            }
            else
            {
                NewMessageBox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            }
            panel.ScrollControlIntoView(NewMessageBox);

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CheckButton_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            string strSendString = InputWindow.Text;
            if (strSendString.Length > 0)
            {
                ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.MESSAGE);
                ListSendByte.AddRange(BitConverter.GetBytes(m_iID));

                byte[] ByteArray = System.Text.Encoding.UTF8.GetBytes(strSendString);
                ListSendByte.AddRange(BitConverter.GetBytes(ByteArray.Length));
                ListSendByte.AddRange(ByteArray);

                m_InsertToServerQueue(ListSendByte);
            }

            ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.SENDUPLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(m_iID));

            m_InsertToServerQueue(ListSendByte);
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
                    ListSendByte.AddRange(BitConverter.GetBytes(m_iID));
                    ListSendByte.AddRange(BitConverter.GetBytes(strFileName.Length));
                    ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(strFileName));

                    m_InsertToServerQueue(ListSendByte);
                }
            }
        }

        
    }
}
