using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using MyTalkingLib;
namespace TalkingClientMain
{
    public class TalkClientModel : EventInterface
    {
        private class DownloadFileInfo
        {
            public int m_iFileID;
            public int m_iMessageID;
            public int m_iOwnerID;
            public string m_strFileName;
            public DownloadFileInfo(int _iFileID,int _iMessageID,int _iOwnerID,string _strFileName)
            {
                m_iFileID = _iFileID;
                m_iMessageID = _iMessageID;
                m_iOwnerID = _iOwnerID;
                m_strFileName = _strFileName;
            }
        }

        public delegate int DelAddTimerEvent(int _iTargetTime, WaitCallback _CallBack, object _Parameter,MyTimer.ETimerMode _Mode);
        public delegate void DelRemoveTimerEvent(int _iID);
        private FileIOModel m_FileIOModel = new FileIOModel();

        private int m_iAccountID = -1;
        private int m_iUpdateScheduleDeltaTime = 200;
        private byte[] m_ByteArrayAccountKey;
        private string m_strTalkServerIP = "";
        private int m_iTalkServerPort = 0;
        private object m_oQueueLock = new object();
        private int m_iReadFileBytes = 1000000;
        private object m_oUploadFileLock = new object();
        private string m_strLoginServerIP = "127.0.0.1";
        private int m_iLoginServerPort = 9001;
        private string m_strDownloadBasePath = @"D:\TalkingProjectDownload\Download\";
        private string m_strSaveFileName = @"UnFinishedDownloadFile.txt";
        private object m_oMessageLock = new object();

        private Dictionary<int,List<int>> m_DicUploadFileIDPair = new Dictionary<int, List<int>>();
        private List<DownloadFileInfo> m_ListDownloadFile = new List<DownloadFileInfo>();

        private Dictionary<int, int> m_DicUpdateFileIDTimerIDPair = new Dictionary<int, int>();
        
        private WaitCallback m_InsertToServerViewQueue = null;

        private DelAddTimerEvent m_AddTimerEvent = null;
        private DelRemoveTimerEvent m_RemoveTimerEvent = null;

        public TalkClientModel(WaitCallback _CallBack, WaitCallback _ViewCallBack, DelAddTimerEvent _AddTimerEvent, DelRemoveTimerEvent _RemoveTimerEvent) : base(_CallBack){
            m_InsertToServerViewQueue = _ViewCallBack;
            m_AddTimerEvent = _AddTimerEvent;
            m_RemoveTimerEvent = _RemoveTimerEvent;
        }
        public void CaseSignInSuccess(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            m_ByteArrayAccountKey = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            m_strTalkServerIP = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
            m_iTalkServerPort = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            Console.WriteLine("TalkingServerIp : " + m_strTalkServerIP + " TalkingServerPort : " + m_iTalkServerPort);

            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.CHANGESTATE);
            ListSendByte.AddRange(BitConverter.GetBytes((int)FlowState.TALKING));
            m_InsertToQueue(ListSendByte);
            if(m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.SIGNINSUCCESS, MyInfo));
            }
        }
        public void CaseLoginMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            Console.WriteLine(strMessage);
            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.LOGINMESSAGE, MyInfo));
            }
        }
        public void CaseMenuData(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            while (ListByte.Count > 0)
            {
                int iID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                string strName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

                Console.WriteLine(String.Format("Online User : ID {0} , Name {1}", iID, strName));
            }
            if(m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.MENUDATA, MyInfo));
            }
        }
        public void CaseMessage(object _Parameter)
        {
            
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            Console.WriteLine("Message Length : " + ListByte.Count);
            long lUnixTime = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            int iTheSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            Console.WriteLine("Time : " + lUnixTime + " "+ iTheSenderID + "SendMessage To Room " + iRoomID + " " + strMessage);
            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.MESSAGE, MyInfo));
            }
            
        }
        public void CaseFileMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            long lUnixTime = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            int iTheSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            Console.WriteLine("Time : " + lUnixTime + " " + iTheSenderID + "SendFileMessage To Room " + iRoomID + " " + strMessage);
            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.FILEMESSAGE, MyInfo));
            }
        }
        public void CaseSetAccountID(object _Parameter)
        {
            Console.WriteLine("SetAccountID");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iAccountID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            m_iAccountID = iAccountID;

            CreateFileDirectory();
            InitUnFinishedDownloadFile();
            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.SETTALKINGACCOUNTID, MyInfo));
            }
        }
        public void CaseAddTalkingPage(object _Parameter)
        {
            Console.WriteLine("Model : AddTalkingPage");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iReceiverID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.ADDTALKINGPAGE, MyInfo));
            }

            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)(ServerType.TALKINGSERVER)));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.GETHISTORYMESSAGE);
            ListSendByte.AddRange(BitConverter.GetBytes(iReceiverID));

            m_InsertToQueue(ListSendByte);
            
        }
        public void CaseUploadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iReceiverID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            int iFileID = m_FileIOModel.OpenFile(strFileName, FileClass.FileType.READ);
            if (m_DicUploadFileIDPair.ContainsKey(iReceiverID))
            {
                m_DicUploadFileIDPair[iReceiverID].Add(iFileID);
            }else
            {
                m_DicUploadFileIDPair.Add(iReceiverID, new List<int>(new int[] { iFileID }));
            }

            strFileName = strFileName.Substring(strFileName.LastIndexOf('\\') + 1);
            Console.WriteLine("UploadFile : " + strFileName);
            SendStart(strFileName, iFileID);

            if (m_InsertToServerViewQueue != null)
            {
                List<byte> ListByteToView = new List<byte>();
                ListByteToView.AddRange(BitConverter.GetBytes(iSenderID));
                ListByteToView.Add((byte)EventType.UPLOADFILE);
                ListByteToView.AddRange(BitConverter.GetBytes(iFileID));
                ListByteToView.AddRange(BitConverter.GetBytes(iReceiverID));
                byte[] strByteArray = MyConverter.GetByteArrayFromString(m_FileIOModel.GetFileName(iFileID));
                ListByteToView.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListByteToView.AddRange(strByteArray);

                m_InsertToServerViewQueue(ListByteToView);
            }
        }
        public void CaseStartSendFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.READFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
            m_InsertToQueue(ListSendByte);
        }
        public void CaseReadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            lock (m_FileIOModel)
            {
                if (m_FileIOModel.HasFile(iFileID))
                {
                    byte[] ReadBytes = m_FileIOModel.ReadFile(iFileID, m_iReadFileBytes);

                    byte EventByte = (byte)EventType.SENDFILELOOP;
                    if (m_FileIOModel.IsFileFinished(iFileID))
                    {
                        Console.WriteLine("FileIsOver");
                        EventByte = (byte)EventType.SENDFILEEND;
                    }

                    List<byte> ListSendByte = new List<byte>();
                    ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                    ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                    ListSendByte.Add(EventByte);
                    ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
                    ListSendByte.AddRange(BitConverter.GetBytes(ReadBytes.Length));
                    ListSendByte.AddRange(ReadBytes);

                    Console.WriteLine("ReadFile : " + ReadBytes.Length);
                    m_InsertToQueue(ListSendByte);

                    if (m_InsertToServerViewQueue != null)
                    {
                        List<byte> ListByteToView = RebuildCommand(EventType.READFILE,MyInfo);
                        ListByteToView.AddRange(BitConverter.GetBytes(m_FileIOModel.GetFilePercent(iFileID)));

                        m_InsertToServerViewQueue(ListByteToView);
                    }
                }
            }
                
            
        }
        public void CaseCheckFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            long lFileSize = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            Console.WriteLine("CheckFileSize : " + lFileSize);
            Console.WriteLine("ActualFileSize : " + m_FileIOModel.GetFileSize(iFileID));
            /*if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.CHANGESTATE, MyInfo));
            }*/
            if(lFileSize == m_FileIOModel.GetFileSize(iFileID))
            {
                Console.WriteLine("UploadSuccess");
                //CloseFile(iFileID);
            }
            else
            {
                Console.WriteLine("UploadFailed");
            }
        }
        public void CaseSendUploadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            if (IsAllUploadSuccess(iRoomID))
            {
                Console.WriteLine("AllUploadSuccess");
                if (m_InsertToServerViewQueue != null)
                {
                    List<byte> ListByteToView = RebuildCommand(EventType.SENDUPLOADFILE, MyInfo);
                    ListByteToView.AddRange(GetListUploadFileID(iRoomID));

                    m_InsertToServerViewQueue(ListByteToView);
                }
                SendRoomUploadFile(iRoomID);
                CloseRoomUploadFile(iRoomID);
            }

        }
        public void CaseDownloadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
            long lStartPosition = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            Console.WriteLine("DownloadFile : " + strFileName);
            int iFileID = 0;
            string strDownloadFilePath = GetDownloadFilePath(strFileName);
            if (lStartPosition == 0)
            {
                iFileID = m_FileIOModel.OpenFile(strDownloadFilePath, FileClass.FileType.WRITE);
                strFileName = strDownloadFilePath.Substring(strDownloadFilePath.LastIndexOf('\\') + 1);
            }
            else
            {
                strDownloadFilePath = m_strDownloadBasePath + strFileName;
                iFileID = m_FileIOModel.OpenFile(strDownloadFilePath, FileClass.FileType.APPEND);
            }

            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.DOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
            ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(iOwnerID));
            ListSendByte.AddRange(BitConverter.GetBytes(lStartPosition));
            m_InsertToQueue(ListSendByte);


            if (m_InsertToServerViewQueue != null)
            {
                List<byte> ListByteToView = new List<byte>();
                ListByteToView.AddRange(BitConverter.GetBytes(iSenderID));
                ListByteToView.Add((byte)EventType.DOWNLOADFILE);
                ListByteToView.AddRange(BitConverter.GetBytes(iFileID));
                ListByteToView.AddRange(BitConverter.GetBytes(iMessageID));
                ListByteToView.AddRange(BitConverter.GetBytes(iOwnerID));
                byte[] strByteArray = MyConverter.GetByteArrayFromString(strFileName);
                ListByteToView.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListByteToView.AddRange(strByteArray);

                m_InsertToServerViewQueue(ListByteToView);
            }

            int iTimerID = m_AddTimerEvent(m_iUpdateScheduleDeltaTime, UpdateProcessSchedule, iFileID,MyTimer.ETimerMode.REPEAT);
            m_DicUpdateFileIDTimerIDPair.Add(iFileID, iTimerID);

            m_ListDownloadFile.Add(new DownloadFileInfo(iFileID, iMessageID, iOwnerID, strFileName));
        }
        public void CasePauseDownloadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            if (!m_FileIOModel.IsFileFinished(iFileID))
            {
                SendStopSendFile(-1, iFileID);
                RemoveUpdateEvent(iFileID);
            }
        }
        public void CaseResumeDownloadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            if (!m_FileIOModel.IsFileFinished(iFileID))
            {
                long lStartPosition = m_FileIOModel.GetFilePosition(iFileID);

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.DOWNLOADFILE);
                ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
                ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
                ListSendByte.AddRange(BitConverter.GetBytes(iOwnerID));
                ListSendByte.AddRange(BitConverter.GetBytes(lStartPosition));
                m_InsertToQueue(ListSendByte);

                int iTimerID = m_AddTimerEvent(m_iUpdateScheduleDeltaTime, UpdateProcessSchedule, iFileID, MyTimer.ETimerMode.REPEAT);
                m_DicUpdateFileIDTimerIDPair.Add(iFileID, iTimerID);
            }
            
        }
        public void CaseSendFileLoop(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            byte[] FileContent = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            lock (m_FileIOModel)
            {
                if (m_FileIOModel.HasFile(iFileID))
                {
                    m_FileIOModel.WriteFile(iFileID, FileContent);
                    SendAckToServer(iFileID);
                }
            }
            
        }
        public void CaseSendFileEnd(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            byte[] FileContent = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            lock (m_FileIOModel)
            {
                if (m_FileIOModel.HasFile(iFileID))
                {
                    m_FileIOModel.WriteFile(iFileID, FileContent);

                    RemoveUpdateEvent(iFileID);
                    UpdateProcessSchedule(iFileID);

                    m_FileIOModel.CloseFile(iFileID);
                    RemoveDownloadFile(iFileID);
                }
            }
        }
        public void CaseSetFileSize(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            long lFileSize = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            m_FileIOModel.SetFileSize(iFileID, lFileSize);
            Console.WriteLine("SetFileSize : " + lFileSize);

            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.SETFILESIZE, MyInfo));
            }
        }
        public void CaseStopSendFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            SendStopSendFile(iRoomID,iFileID);
            lock (m_FileIOModel)
            {
                DeleteFile(iRoomID, iFileID);
                CloseFile(iRoomID, iFileID);
            }
            RemoveUpdateEvent(iFileID);
            if (m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.STOPSENDFILE, MyInfo));
            }
        }
        public void SaveUnFinishedDownloadFile()
        {
            lock (m_FileIOModel)
            {
                int iFileID = m_FileIOModel.OpenFile(m_strDownloadBasePath + m_strSaveFileName, FileClass.FileType.WRITE);
                int iListCount = m_ListDownloadFile.Count;
                for (int i = 0; i < iListCount; ++i)
                {
                    Console.WriteLine("SaveFile : " + i);
                    DownloadFileInfo MyDownloadFileInfo = m_ListDownloadFile[i];
                    int iDownloadFileID = MyDownloadFileInfo.m_iFileID;
                    long lFilePosition = m_FileIOModel.GetFilePosition(iDownloadFileID);

                    List<byte> ListWriteByte = new List<byte>();
                    byte[] strByteArray = MyConverter.GetByteArrayFromString(MyDownloadFileInfo.m_strFileName);
                    ListWriteByte.AddRange(BitConverter.GetBytes(strByteArray.Count()));
                    ListWriteByte.AddRange(strByteArray);
                    ListWriteByte.AddRange(BitConverter.GetBytes(lFilePosition));
                    ListWriteByte.AddRange(BitConverter.GetBytes(MyDownloadFileInfo.m_iMessageID));
                    ListWriteByte.AddRange(BitConverter.GetBytes(MyDownloadFileInfo.m_iOwnerID));

                    m_FileIOModel.WriteFile(iFileID, ListWriteByte.ToArray());
                }
                Console.WriteLine("SaveFinished");
                m_FileIOModel.RemoveFile(iFileID);
                Console.WriteLine("SaveFinished");
            }
        }
        private void UpdateProcessSchedule(object _Parameter)
        {
            int iFileID = (int)_Parameter;
            if (m_FileIOModel.HasFile(iFileID) && m_InsertToServerViewQueue != null)
            {
                List<byte> ListByteToView = new List<byte>();
                ListByteToView.AddRange(BitConverter.GetBytes(-1));
                ListByteToView.Add((byte)EventType.SENDFILELOOP);
                ListByteToView.AddRange(BitConverter.GetBytes(iFileID));
                ListByteToView.AddRange(BitConverter.GetBytes(m_FileIOModel.GetFileSpeed(iFileID)));
                ListByteToView.AddRange(BitConverter.GetBytes(m_FileIOModel.GetFilePosition(iFileID)));
                m_InsertToServerViewQueue(ListByteToView);
            }
        }
        private void DeleteFile(int _iRoomID,int _iFileID)
        {
            if (m_FileIOModel.HasFile(_iFileID) && (!IsUploadFile(_iRoomID, _iFileID) && !m_FileIOModel.IsFileFinished(_iFileID)))
            {
                m_FileIOModel.DeleteFile(_iFileID);
            }
        }
        private void SendAckToServer(int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.READFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            m_InsertToQueue(ListSendByte);
        }
        private bool IsUploadFile(int _iRoomID,int _iFileID)
        {
            return _iRoomID != -1;
        }
        private void SendStopSendFile(int _iRoomID,int _iFileID)
        {
            if(m_FileIOModel.HasFile(_iFileID) && (IsUploadFile(_iRoomID, _iFileID) || !m_FileIOModel.IsFileFinished(_iFileID)))
            {
                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.STOPSENDFILE);
                ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
                m_InsertToQueue(ListSendByte);
            }
        }
        private string GetDownloadFilePath(string _strFileName)
        {
            int iDividePoint = _strFileName.LastIndexOf('.');
            string strFileNameExtension = "";
            string strFileName = m_strDownloadBasePath + _strFileName;
            if (iDividePoint != -1)
            {
                strFileNameExtension = _strFileName.Substring(iDividePoint);
                strFileName = m_strDownloadBasePath + _strFileName.Substring(0, iDividePoint);
            }
            string strReturnString = strFileName + strFileNameExtension;
            if (File.Exists(strReturnString))
            {
                int iFileCounter = 0;
                while (File.Exists(strFileName + "(" + iFileCounter.ToString() + ")" + strFileNameExtension))
                {
                    ++iFileCounter;
                }
                strReturnString = strFileName + "(" + iFileCounter.ToString() + ")" + strFileNameExtension;
            }
            return strReturnString;
        }
        private List<byte> GetListUploadFileID(int _iRoomID)
        {
            List<byte> ReturnList = new List<byte>();
            List<int> ListFileID = m_DicUploadFileIDPair[_iRoomID];
            int iListCount = ListFileID.Count;
            for (int i = 0; i < iListCount; ++i)
            {
                ReturnList.AddRange(BitConverter.GetBytes(ListFileID[i]));
            }
            return ReturnList;
        }
        private void CloseRoomUploadFile(int _iRoomID)
        {
            List<int> ListFileID = m_DicUploadFileIDPair[_iRoomID];
            int iListCount = ListFileID.Count;
            for(int i = iListCount - 1;i >= 0; --i)
            {
                CloseFile(_iRoomID,ListFileID[i]);
            }
        }
        private void SendRoomUploadFile(int _iRoomID)
        {
            List<int> ListFileID = m_DicUploadFileIDPair[_iRoomID];
            int iListCount = ListFileID.Count;
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.FILEMESSAGE);
            for (int i=0;i< iListCount; ++i)
            {
                Thread.Sleep(1);
                List<byte> ListSendByteTemp = new List<byte>(ListSendByte);
                ListSendByteTemp.AddRange(BitConverter.GetBytes(MyLibFunction.GetNowUnixTime()));
                ListSendByteTemp.AddRange(BitConverter.GetBytes(_iRoomID));
                ListSendByteTemp.AddRange(BitConverter.GetBytes(ListFileID[i]));
                byte[] strByteArray = MyConverter.GetByteArrayFromString(m_FileIOModel.GetFileName(ListFileID[i]));
                ListSendByteTemp.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListSendByteTemp.AddRange(strByteArray);
                Console.WriteLine("SendTime : " + GetNowUnixTime());
                m_InsertToQueue(ListSendByteTemp);
            }
        }
        private long GetNowUnixTime()
        {
            return (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
        }
        private bool IsAllUploadSuccess(int _iRoomID)
        {
            bool bReturnValue = false;
            if (m_DicUploadFileIDPair.ContainsKey(_iRoomID))
            {
                List<int> ListFileID = m_DicUploadFileIDPair[_iRoomID];
                int iListCount = ListFileID.Count;
                for(int i=0;i< iListCount; ++i)
                {
                    if (m_FileIOModel.IsFileFinished(ListFileID[i]))
                    {
                        bReturnValue = true;
                    }else
                    {
                        bReturnValue = false;
                        break;
                    }
                }
            }
            return bReturnValue;
        }
        private void CloseFile(int _iRoomID,int _iFileID)
        {
            RemoveDownloadFile(_iFileID);
            if (IsUploadFile(_iRoomID, _iFileID))
            {
                m_DicUploadFileIDPair[_iRoomID].Remove(_iFileID);
            }
            if (m_FileIOModel.HasFile(_iFileID))
            {
                m_FileIOModel.RemoveFile(_iFileID);
            }
        }
        private void RemoveDownloadFile(int _iFileID)
        {
            int iListCount = m_ListDownloadFile.Count;
            for(int i=0;i< iListCount; ++i)
            {
                if(m_ListDownloadFile[i].m_iFileID == _iFileID)
                {
                    m_ListDownloadFile.RemoveAt(i);
                    break;
                }
            }
        }
        public void ChangeState(object _Parameter)
        {
            Console.WriteLine("NowChangeState");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iState = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            FlowState NextState = (FlowState)(iState);
            if(m_InsertToServerViewQueue != null)
            {
                m_InsertToServerViewQueue(RebuildCommand(EventType.CHANGESTATE, MyInfo));
            }
            switch (NextState)
            {
                case FlowState.LOGIN:
                    StartLogin();
                    break;
                case FlowState.TALKING:
                    StartTalking();
                    break;
            }
        }
        private void SendStart(string _strFileName, int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.UPLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            byte[] strByteArray = MyConverter.GetByteArrayFromString(_strFileName);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
            ListSendByte.AddRange(strByteArray);

            m_InsertToQueue(ListSendByte);

            //AddReadFileEvent(_iFileID);
        }
        private void StartLogin()
        {
            Socket ConnectSocket = Connect(m_strLoginServerIP, m_iLoginServerPort);
            m_DicSocketInfo.Add((int)ServerType.LOGINSERVER, new SocketInfo((int)ServerType.LOGINSERVER, ConnectSocket));

            List<byte> ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.LOGINSERVER));
            ListByte.Add((byte)EventType.RECEIVE);
            m_InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.LOGINSERVER));
            ListByte.Add((byte)EventType.SEND);
            m_InsertToQueue(ListByte);
            
        }
        private void StartTalking()
        {
            Socket ConnectSocket = Connect(m_strTalkServerIP, m_iTalkServerPort);
            m_DicSocketInfo.Add((int)ServerType.TALKINGSERVER, new SocketInfo((int)ServerType.TALKINGSERVER, ConnectSocket));

            List<byte> ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.RECEIVE);
            m_InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.SEND);
            m_InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListByte.Add((byte)EventType.SIGNIN);
            ListByte.AddRange(BitConverter.GetBytes(m_ByteArrayAccountKey.Length));
            ListByte.AddRange(m_ByteArrayAccountKey);
            m_InsertToQueue(ListByte);

            
        }
        private void InitUnFinishedDownloadFile()
        {
            int iFileID = m_FileIOModel.OpenFile(m_strDownloadBasePath + m_strSaveFileName, FileClass.FileType.READ);
            Console.WriteLine("InitOpenFile");
            Console.WriteLine("FileName : " + m_strDownloadBasePath + m_strSaveFileName);
            List<byte> ListReadBytes = new List<byte>(m_FileIOModel.ReadFile(iFileID, (int)m_FileIOModel.GetFileSize(iFileID)));
            while (ListReadBytes.Count > 0)
            {
                Console.WriteLine("Init ReadFile");
                string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListReadBytes);
                long lFilePosition = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListReadBytes);
                int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListReadBytes);
                int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListReadBytes);

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes(-1));
                ListSendByte.Add((byte)EventType.DOWNLOADFILE);
                ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
                ListSendByte.AddRange(BitConverter.GetBytes(iOwnerID));
                byte[] strByteArray = MyConverter.GetByteArrayFromString(strFileName);
                ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListSendByte.AddRange(strByteArray);
                ListSendByte.AddRange(BitConverter.GetBytes(lFilePosition));
                m_InsertToQueue(ListSendByte);
            }
            m_FileIOModel.DeleteFile(iFileID);
            m_FileIOModel.RemoveFile(iFileID);
        }
        private List<string> SplitString(string _strInput,char _Divider)
        {
            List<string> ReturnList = new List<string>();
            while(_strInput.Length > 0)
            {
                int iDividePoint = _strInput.IndexOf(_Divider);
                if(iDividePoint == -1)
                {
                    ReturnList.Add(_strInput.Substring(0, _strInput.Length));
                    _strInput = "";
                }
                else
                {
                    ReturnList.Add(_strInput.Substring(0, iDividePoint));
                    _strInput = _strInput.Substring(iDividePoint + 1);
                }
            }
            return ReturnList;
        }
        private void CreateFileDirectory()
        {
            m_strDownloadBasePath += m_iAccountID.ToString();
            if (!Directory.Exists(m_strDownloadBasePath))
            {
                Directory.CreateDirectory(m_strDownloadBasePath);
            }
            m_strDownloadBasePath += @"\";
        }
        private void RemoveUpdateEvent(int _iFileID)
        {
            if (m_DicUpdateFileIDTimerIDPair.ContainsKey(_iFileID))
            {
                int iTimerID = m_DicUpdateFileIDTimerIDPair[_iFileID];
                m_DicUpdateFileIDTimerIDPair.Remove(_iFileID);
                m_RemoveTimerEvent(iTimerID);
            }
        }
        private List<byte> RebuildCommand(EventType _EventType, CaseInfoClass _MyInfo)
        {
            return MyLibFunction.RebuildCommand(_EventType, _MyInfo);
        }

    }
}
