using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace TalkingServerCmd
{
    public class TalkingServerModel:EventInterface
    {
        public delegate int DelAddTimerEvent(int _iTargetTime = 0, WaitCallback _Function = null, object _Parameter = null,MyTimer.ETimerMode _Mode = MyTimer.ETimerMode.NORMAL);
        public delegate void DelRemoveTimerEvent(int _iID);
        
        private Dictionary<int, List<SocketInfo>> m_DicOnlineClient = new Dictionary<int, List<SocketInfo>>();
        private Dictionary<int, int> m_DicAccountIDPair = new Dictionary<int, int>();
        private Dictionary<int, int> m_DicFileIDPair = new Dictionary<int, int>();
        private Dictionary<int, List<int>> m_DicClientIDFileIDPair = new Dictionary<int, List<int>>();
        private List<int> m_iListUploadFileID = new List<int>();
        private MessageModel m_MessageModel;
        private GroupModel m_GroupModel;
        private EncryptModel m_EncryptModel;
        private FileIOModel m_FileIOModel;
        private string strTalkingServerKey = "+y3txUT3ynClLkDTi2551Da3W2JYrqaf";
        private string strTalkingServerIV = "kSXHSqR5Pqc=";
        
        private int m_iLoginWaitTime = 5 * 60 * 1000;
        
        private Dictionary<byte[], int> m_DicLoginKeyPair = new Dictionary<byte[], int>(new ByteArrayComparer());
        private string m_strServerFileBasePath = @"D:\TalkingProjectDownload\Server";
        private int m_iCounter = 0;
        private object m_oFileIOLock = new object();
        private int m_iReadFileBytes = 1000000;
        private object m_oUploadFileLock = new object();
        private List<byte> m_ListIOEvent = new List<byte>();
        private object m_oSqlLock = new object();

        private Dictionary<int, List<int>> m_DicGroupIDMemberIDPair = new Dictionary<int, List<int>>();

        private WaitCallback m_InsertToQueue;
        private DelAddTimerEvent AddTimeEvent = null;
        private DelRemoveTimerEvent RemoveTimerEvent = null;
        public TalkingServerModel(WaitCallback _CallBack, DelAddTimerEvent _AddTimerEvent, DelRemoveTimerEvent _RemoveTimerEvent):base(_CallBack)
        {
            m_InsertToQueue = _CallBack;
            AddTimeEvent = _AddTimerEvent;
            RemoveTimerEvent = _RemoveTimerEvent;
            m_MessageModel = new MessageModel("localhost", "user2", "user2", "HistoryMessage");
            m_GroupModel = new GroupModel("localhost", "user2", "user2", "GroupMember");
            m_EncryptModel = new EncryptModel(strTalkingServerKey, strTalkingServerIV);
            m_FileIOModel = new FileIOModel();
            m_ConnectModel.SetRemoveSocketCallBack(RemoveClient);
        }
        public override void RemoveClient(object _Parameter)
        {
            base.RemoveClient(_Parameter);
            SocketInfo MySocketInfo = _Parameter as SocketInfo;
            int iSenderID = MySocketInfo.m_iID;
            lock (m_DicClientIDFileIDPair)
            {
                if (m_DicClientIDFileIDPair.ContainsKey(iSenderID))
                {
                    List<int> ListFileID = m_DicClientIDFileIDPair[iSenderID];
                    int iListCount = ListFileID.Count;
                    for (int i = iListCount - 1; i >= 0; --i)
                    {
                        int iDicKey = ListFileID[i];
                        Console.WriteLine("RemoveDicID : " + iDicKey);
                        CloseFile(iDicKey);
                    }
                    m_DicClientIDFileIDPair.Remove(iSenderID);
                }
            }
            lock (m_DicAccountIDPair)
            {
                if (m_DicAccountIDPair.ContainsKey(iSenderID))
                {
                    int iAccountID = m_DicAccountIDPair[iSenderID];
                    m_DicAccountIDPair.Remove(iSenderID);
                    m_DicOnlineClient[iAccountID].Remove(MySocketInfo);
                }
            }
        }
        public void CaseSignIn(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            byte[] TheKey = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            //Console.WriteLine("SignIN");
            Console.WriteLine("LoginKey : " + Convert.ToBase64String(TheKey));
            if (m_DicLoginKeyPair.ContainsKey(TheKey))
            {
                Console.WriteLine("ContainsKey");
                int iTimerEventID = m_DicLoginKeyPair[TheKey];

                List<byte> ListDecryptKey = new List<byte>(m_EncryptModel.Decrypt(TheKey));
                int iAccountID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListDecryptKey);

                m_DicAccountIDPair.Add(iSenderID, iAccountID);

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.SETTALKINGACCOUNTID);
                ListSendByte.AddRange(BitConverter.GetBytes(iAccountID));

                m_InsertToQueue(ListSendByte);

                RemoveLoginUser(TheKey);

                InitEvent(iSenderID, iAccountID);
                Console.WriteLine(iSenderID + "SignIn as " + iAccountID);
            }
        }
        
        public void CaseAddLoginUser(object _Parameter)
        {
            Console.WriteLine("AddLoginUser : ");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            byte[] TheKey = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            int iTimerEventID = AddTimeEvent(m_iLoginWaitTime, RemoveLoginUser, TheKey);

            m_DicLoginKeyPair.Add(TheKey, iTimerEventID);
            Console.WriteLine("AddLoginUser : " + iSenderID);
            Console.WriteLine("UserKey : " + Convert.ToBase64String(TheKey));
            //Console.WriteLine("Insert To " + (ServerType)iSenderID + " Socket");

        }
        
        public void CaseDownloadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strFileName;
            lock (m_oSqlLock)
            {
                strFileName = m_MessageModel.GetFileNameByMessageID(iMessageID);
            }

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            long lStartPosition = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            int iDicFileKey = (iSenderID << 16) + iFileID;
            Console.WriteLine("DownloadFileID : " + iDicFileKey);
            int iServerFileID;
            lock (m_oFileIOLock)
            {
                string strFolderPath = m_strServerFileBasePath + @"\" + iOwnerID.ToString();
                iServerFileID = OpenFile(iSenderID,iDicFileKey, strFolderPath + @"\" + strFileName, FileClass.FileType.READ, lStartPosition);
            }

            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.SETFILESIZE);
            ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_FileIOModel.GetFileSize(iServerFileID)));

            m_InsertToQueue(ListSendByte);

            AddReadFileEvent(iSenderID, iFileID);

        }
        
        public void CaseReadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iDicKey = (iSenderID << 16) + iFileID;
            int iServerFileID = 0;
            if (m_DicFileIDPair.ContainsKey(iDicKey))
            {
                iServerFileID = m_DicFileIDPair[iDicKey];

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                byte[] ReadArray;
                lock (m_oFileIOLock)
                {
                    ReadArray = m_FileIOModel.ReadFile(iServerFileID, m_iReadFileBytes);
                    Console.WriteLine("Progress : " + m_FileIOModel.GetFilePercent(iServerFileID));
                }
                if (m_FileIOModel.IsFileFinished(iServerFileID))
                {
                    ListSendByte.Add((byte)EventType.SENDFILEEND);
                    Console.WriteLine("DownLoadFinished");
                    CloseFile(iDicKey);
                }
                else
                {
                    ListSendByte.Add((byte)EventType.SENDFILELOOP);
                }
                ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
                ListSendByte.AddRange(BitConverter.GetBytes(ReadArray.Length));
                ListSendByte.AddRange(ReadArray);
                m_InsertToQueue(ListSendByte);
                //Console.WriteLine("SendBytes : " + iSendLength);
                ++m_iCounter;
            }

            


        }
        public void CaseFileMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            if (m_DicAccountIDPair.ContainsKey(iSenderID))
            {
                int iSenderAccountID = m_DicAccountIDPair[iSenderID];
                long lSendUnixTime = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
                long lRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
                
                int iDicFileKey = (iSenderID << 16) + iFileID;
                int iServerFileID = m_DicFileIDPair[iDicFileKey];
                int iMessageID;
                string strServerFileName = m_FileIOModel.GetFileName(iServerFileID);

                lock (m_oSqlLock)
                {
                    iMessageID = m_MessageModel.InsertHistoryMessage(iSenderAccountID, lRoomID, strServerFileName, 1, lSendUnixTime);
                }
                if (!IsGroup(lRoomID))
                {
                    SendMessageToClient((byte)EventType.FILEMESSAGE, iSenderID, iSenderAccountID, (int)lRoomID, iSenderAccountID, iMessageID, strFileName, lSendUnixTime);

                    if (iSenderAccountID != lRoomID)
                    {
                        SendMessageToClient((byte)EventType.FILEMESSAGE, iSenderID, iSenderAccountID, iSenderAccountID, (int)lRoomID, iMessageID, strFileName, lSendUnixTime);
                    }
                }
                else
                {
                    List<int> iListMemberID = GetGroupMemberID(RoomIDToGroupID(lRoomID));
                    int iListCount = iListMemberID.Count;
                    for (int i = 0; i < iListCount; ++i)
                    {
                        SendMessageToClient((byte)EventType.FILEMESSAGE, iSenderID, iSenderAccountID, iListMemberID[i], (int)lRoomID, iMessageID, strFileName, lSendUnixTime);
                    }
                }

                lock (m_oFileIOLock)
                {
                    m_iListUploadFileID.Remove(iServerFileID);
                    CloseFile(iDicFileKey);
                }
            }
        }
        public void CaseMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            if (m_DicAccountIDPair.ContainsKey(iSenderID))
            {
                int iSenderAccountID = m_DicAccountIDPair[iSenderID];
                Console.WriteLine("Message Length : " + ListByte.Count);
                long lSendUnixTime = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
                long lRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
                Console.WriteLine("RoomID : " + lRoomID);
                Console.WriteLine("Message : " + strMessage);
                int iMessageID;
                lock (m_oSqlLock)
                {
                    iMessageID = m_MessageModel.InsertHistoryMessage(iSenderAccountID, lRoomID, strMessage, 0, lSendUnixTime);
                }
                if (!IsGroup(lRoomID))
                {
                    SendMessageToClient((byte)EventType.MESSAGE, iSenderID, iSenderAccountID, (int)lRoomID, iSenderAccountID, iMessageID, strMessage, lSendUnixTime);

                    if (iSenderAccountID != lRoomID)
                    {
                        SendMessageToClient((byte)EventType.MESSAGE, iSenderID, iSenderAccountID, iSenderAccountID, (int)lRoomID, iMessageID, strMessage, lSendUnixTime);
                    }
                }else
                {
                    List<int> iListMemberID = GetGroupMemberID(RoomIDToGroupID(lRoomID));
                    int iListCount = iListMemberID.Count;
                    for (int i=0;i< iListCount; ++i)
                    {
                        SendMessageToClient((byte)EventType.MESSAGE, iSenderID, iSenderAccountID, iListMemberID[i], (int)lRoomID, iMessageID, strMessage, lSendUnixTime);
                    }
                }
            }
            Console.WriteLine("CaseSendMessage");
        }
        public void CaseStopSendFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iDicFileKey = (iSenderID << 16) + iFileID;
            lock (m_oFileIOLock)
            {
                CloseFile(iDicFileKey);
            }
        }
        public void CaseSendFileEnd(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            byte[] iFileMessage = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            int iDicFileKey = (iSenderID << 16) + iFileID;
            lock (m_oFileIOLock)
            {
                if (m_DicFileIDPair.ContainsKey(iDicFileKey))
                {
                    int iServerFileID = m_DicFileIDPair[iDicFileKey];
                    m_FileIOModel.WriteFile(iServerFileID, iFileMessage);
                    SendCheckFileToClient(iSenderID, iFileID);
                }
            }
        }
        public void CaseSendFileLoop(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            byte[] iFileMessage = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            int iDicFileKey = (iSenderID << 16) + iFileID;
            lock (m_oFileIOLock)
            {
                if (m_DicFileIDPair.ContainsKey(iDicFileKey))
                {
                    int iServerFileID = m_DicFileIDPair[iDicFileKey];
                    m_FileIOModel.WriteFile(iServerFileID, iFileMessage);
                    //Console.WriteLine("FileCounter : " + m_iCounter);
                    ++m_iCounter;

                    List<byte> ListSendByte = new List<byte>();
                    ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                    ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                    ListSendByte.Add((byte)EventType.READFILE);
                    ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
                    m_InsertToQueue(ListSendByte);
                }
            }
        }
        public void CaseUploadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            if (m_DicAccountIDPair.ContainsKey(iSenderID))
            {
                int iSenderAccountID = m_DicAccountIDPair[iSenderID];
                int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

                lock (m_oFileIOLock)
                {
                    int iDicFileKey = (iSenderID << 16) + iFileID;
                    
                    string strFolderPath = m_strServerFileBasePath + @"\" + iSenderAccountID.ToString();
                    strFileName = GetNowTimeString() + "_" + strFileName;
                    string strFileFinalNameBase = strFolderPath + @"\" + strFileName;
                    string strFileFinalName = GetDownloadFilePath(strFileFinalNameBase);
                    OpenFile(iSenderID,iDicFileKey, strFileFinalName, FileClass.FileType.WRITE);

                    List<byte> ListSendByte = new List<byte>();
                    ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                    ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                    ListSendByte.Add((byte)EventType.STARTSENDFILE);
                    ListSendByte.AddRange(BitConverter.GetBytes(iFileID));

                    m_InsertToQueue(ListSendByte);
                    Console.WriteLine("UploadFileKey : " + iDicFileKey);
                }
            }

        }
        public void CaseGetHistoryMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            //Console.WriteLine(iSenderID + "GetHistoryMessage");
            if (m_DicAccountIDPair.ContainsKey(iSenderID))
            {
                int iSenderAccountID = m_DicAccountIDPair[iSenderID];
                //Console.WriteLine(iSenderAccountID + "GetHistoryMessage");
                int iReceiveID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                //Console.WriteLine("ReceiverID : " + iReceiveID);
                List<List<string>> ListString = m_MessageModel.GetHistoryMessage(iSenderAccountID, iReceiveID);
                int iListCount = ListString.Count;
                for (int i = 0; i < iListCount; ++i)
                {
                    List<string> HistoryMessage = ListString[i];
                    int iMessageID = Convert.ToInt32(HistoryMessage[0]);
                    int iFromID = Convert.ToInt32(HistoryMessage[1]);
                    int iToID = Convert.ToInt32(HistoryMessage[2]);
                    string strMessage = HistoryMessage[3];
                    int iMessageType = Convert.ToInt32(HistoryMessage[4]);
                    long lUnixTime = Convert.ToInt64(HistoryMessage[5]);

                    List<byte> ListSendByte = new List<byte>();
                    ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                    ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                    int iTheSenderID = iSenderAccountID;
                    if (iTheSenderID != iFromID)
                    {
                        iTheSenderID = iReceiveID;
                    }

                    if (iMessageType == 0)
                    {
                        ListSendByte.Add((byte)EventType.MESSAGE);
                    }
                    else
                    {
                        ListSendByte.Add((byte)EventType.FILEMESSAGE);
                    }
                    ListSendByte.AddRange(BitConverter.GetBytes(lUnixTime));
                    ListSendByte.AddRange(BitConverter.GetBytes(iTheSenderID));
                    ListSendByte.AddRange(BitConverter.GetBytes(iReceiveID));
                    ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
                    string strSendString = strMessage;
                    if (iMessageType == 1)
                    {
                        strSendString = strSendString.Substring(strSendString.IndexOf('_') + 1);
                    }
                    //Console.WriteLine("Message : " + strMessage);
                    byte[] strByteArr = System.Text.Encoding.UTF8.GetBytes(strSendString);
                    ListSendByte.AddRange(BitConverter.GetBytes(strByteArr.Length));
                    ListSendByte.AddRange(strByteArr);

                    m_InsertToQueue(ListSendByte);
                }
            }


        }
        public void CaseAddNewGroup(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            List<int> iListMemberID = new List<int>();
            while(ListByte.Count > 0)
            {
                iListMemberID.Add((int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte));
            }
            int iGroupID = m_GroupModel.AddNewGroup(iListMemberID);
            AddGroupMembersToDic(iGroupID);


        }
        public void CaseUpdateGroupMembers(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iGroupID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            GroupEvent MyGroupEvent = (GroupEvent)(int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iUpdateID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            m_GroupModel.UpdateGroupMembers(MyGroupEvent, iGroupID, iUpdateID);

            AddGroupMembersToDic(iGroupID);

            switch (MyGroupEvent)
            {
                case GroupEvent.ADDMEMBER:
                    m_DicGroupIDMemberIDPair[iGroupID].Add(iUpdateID);
                    break;
                case GroupEvent.REMOVEMEMBER:
                    m_DicGroupIDMemberIDPair[iGroupID].Remove(iUpdateID);
                    break;
            }
        }
        private int RoomIDToGroupID(long _lRoomID)
        {
            return (int)(_lRoomID >> 32);
        }
        private bool IsGroup(long _lID)
        {
            return (_lID >> 32) > 0;
        }
        private List<int> GetGroupMemberID(int _iGroupID)
        {
            if (!m_DicGroupIDMemberIDPair.ContainsKey(_iGroupID))
            {
                AddGroupMembersToDic(_iGroupID);
            }
            return m_DicGroupIDMemberIDPair[_iGroupID];
        }
        private void AddGroupMembersToDic(int _iGroupID)
        {
            if (!m_DicGroupIDMemberIDPair.ContainsKey(_iGroupID))
            {
                m_DicGroupIDMemberIDPair.Add(_iGroupID, m_GroupModel.GetMembersByGroupID(_iGroupID));
            }
        }
        private string GetDownloadFilePath(string _strFileName)
        {
            int iDividePoint = _strFileName.LastIndexOf('.');
            string strFileNameExtension = "";
            string strFileName = _strFileName;
            if (iDividePoint != -1)
            {
                strFileNameExtension = _strFileName.Substring(iDividePoint);
                strFileName = _strFileName.Substring(0, iDividePoint);
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
        private void SendCheckFileToClient(int _iSenderID, int _iFileID)
        {
            int iServerFileID = m_DicFileIDPair[(_iSenderID << 16) + _iFileID];
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(_iSenderID));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.CHECKFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(m_FileIOModel.GetFileSize(iServerFileID)));
            //Console.WriteLine("FileLength : " + _BinaryWriter.BaseStream.Length);

            m_InsertToQueue(ListSendByte);
        }
        private static string GetNowTimeString()
        {
            string strReturnValue = "";
            DateTime NowTime = DateTime.Now;
            strReturnValue = String.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", NowTime.Year, NowTime.Month, NowTime.Day, NowTime.Hour, NowTime.Minute, NowTime.Second);
            return strReturnValue;
        }
        private void SendMessageToClient(byte _EventType, int _iSenderID, int _iSenderAccountID, int _iReceiverAccountID, int _iRoomID, int _iMessageID, string _strMessage,long _lSendUnixTime)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add(_EventType);
            ListSendByte.AddRange(BitConverter.GetBytes(_lSendUnixTime));
            ListSendByte.AddRange(BitConverter.GetBytes(_iSenderAccountID));
            ListSendByte.AddRange(BitConverter.GetBytes(_iRoomID));
            ListSendByte.AddRange(BitConverter.GetBytes(_iMessageID));

            byte[] StringByteArr = System.Text.Encoding.UTF8.GetBytes(_strMessage);
            ListSendByte.AddRange(BitConverter.GetBytes(StringByteArr.Length));
            ListSendByte.AddRange(StringByteArr);
            if (m_DicOnlineClient.ContainsKey(_iReceiverAccountID))
            {
                List<SocketInfo> ListOnlineReceiver = m_DicOnlineClient[_iReceiverAccountID];
                int iListCount = ListOnlineReceiver.Count;
                for (int i = 0; i < iListCount; ++i)
                {
                    List<byte> ListSendByteTemp = new List<byte>(ListSendByte);
                    ListSendByteTemp.InsertRange(0, BitConverter.GetBytes(ListOnlineReceiver[i].m_iID));
                    m_InsertToQueue(ListSendByteTemp);
                }
            }
        }
        private void RemoveLoginUser(object _Parameter)
        {
            byte[] TheKey = _Parameter as byte[];
            if (m_DicLoginKeyPair.ContainsKey(TheKey))
            {
                int iTimerEventID = m_DicLoginKeyPair[TheKey];
                m_DicLoginKeyPair.Remove(TheKey);
                RemoveTimerEvent(iTimerEventID);
                List<byte> ListDecryptKey = new List<byte>(m_EncryptModel.Decrypt(TheKey));
                int iAccountID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListDecryptKey);
                Console.WriteLine("Remove User : " + iAccountID);
            }

        }
        private void AddReadFileEvent(int _iSenderID, int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(_iSenderID));
            ListSendByte.Add((byte)EventType.READFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            m_InsertToQueue(ListSendByte);
        }
        private void InitEvent(int _iSenderID, int _iAccountID)
        {
            int iSenderID = _iSenderID;
            int iID = _iAccountID;
            Console.WriteLine(iSenderID + " : InitID : " + iID);
            lock (m_DicOnlineClient)
            {
                if (m_DicOnlineClient.ContainsKey(iID))
                {
                    m_DicOnlineClient[iID].Add(m_DicSocketInfo[iSenderID]);
                }
                else
                {
                    m_DicOnlineClient.Add(iID, new List<SocketInfo>(new SocketInfo[] { m_DicSocketInfo[iSenderID] }));
                    
                }
                ClientEnterToRoom(iSenderID, iID);
            }


            string strFolderPath = m_strServerFileBasePath + @"\" + iID.ToString();


            if (!Directory.Exists(strFolderPath))
            {
                Directory.CreateDirectory(strFolderPath);
            }
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.READY);

            m_InsertToQueue(ListSendByte);
        }
        private void ClientEnterToRoom(int _iSenderID, int _iAccountID)
        {
            Console.WriteLine("EnterRoom");
            List<int> ListClientID;
            int iListCount;
            lock (m_DicOnlineClient)
            {
                ListClientID = new List<int>(m_DicOnlineClient.Keys);
                iListCount = ListClientID.Count;
            }

            List<byte> ListSendByte = new List<byte>();
            ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(_iSenderID));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.MENUDATA);
            string PersonNameBase;
            for (int i = 0; i < iListCount; ++i)
            {
                int iIDThisTime = ListClientID[i];
                if (iIDThisTime != _iAccountID || true)
                {
                    Console.WriteLine("AddOnlineMenu");
                    PersonNameBase = "Name : ";

                    List<byte> ListSendTemp = new List<byte>(ListSendByte);
                    ListSendTemp.AddRange(BitConverter.GetBytes(iIDThisTime));
                    PersonNameBase += iIDThisTime.ToString();
                    byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(PersonNameBase);
                    ListSendTemp.AddRange(BitConverter.GetBytes(strByteArray.Length));
                    ListSendTemp.AddRange(strByteArray);
                    m_InsertToQueue(ListSendTemp);
                }
            }
            PersonNameBase = "Name : " + _iAccountID.ToString();

            ListSendByte = new List<byte>();
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.MENUDATA);
            ListSendByte.AddRange(BitConverter.GetBytes(_iAccountID));
            byte[] strByteArrayBroadCast = System.Text.Encoding.UTF8.GetBytes(PersonNameBase);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArrayBroadCast.Length));
            ListSendByte.AddRange(strByteArrayBroadCast);

            for (int i = 0; i < iListCount; ++i)
            {
                int iIDThisTime = ListClientID[i];
                if (iIDThisTime != _iAccountID)
                {
                    List<SocketInfo> ListClientByID = m_DicOnlineClient[iIDThisTime];
                    int iListClientCount = ListClientByID.Count;
                    for (int j = 0; j < iListClientCount; ++j)
                    {
                        List<byte> ListByteTemp = new List<byte>(ListSendByte);
                        ListByteTemp.InsertRange(0, BitConverter.GetBytes(ListClientByID[j].m_iID));
                        //Console.WriteLine("WrongPlace");
                        m_InsertToQueue(ListByteTemp);
                    }

                }
            }
        }
        private void CloseFile(int _iFileID)
        {
            int iSenderID = _iFileID >> 16;
            lock (m_FileIOModel)
            {
                if (m_DicFileIDPair.ContainsKey(_iFileID))
                {
                    int iServerFileID = m_DicFileIDPair[_iFileID];
                    m_DicFileIDPair.Remove(_iFileID);
                    m_DicClientIDFileIDPair[iSenderID].Remove(_iFileID);
                    if (m_iListUploadFileID.Contains(iServerFileID))
                    {
                        m_FileIOModel.DeleteFile(iServerFileID);
                        m_iListUploadFileID.Remove(iServerFileID);
                    }
                    m_FileIOModel.RemoveFile(iServerFileID);
                }
            }
        }
        private int OpenFile(int _iSenderID = 0,int _iDicKey = 0,string _strFilePath = "",FileClass.FileType _FileType = FileClass.FileType.READ,long _lStartPosition = 0)
        {
            Console.WriteLine(_iSenderID + " OpenFileKey : " + _iDicKey);
            int iFileID = 0;
            lock (m_FileIOModel)
            {
                iFileID = m_FileIOModel.OpenFile(_strFilePath, _FileType, _lStartPosition);
                if (m_DicFileIDPair.ContainsKey(_iDicKey))
                {
                    m_DicFileIDPair[_iDicKey] = iFileID;
                }
                else
                {
                    m_DicFileIDPair.Add(_iDicKey, iFileID);
                }
                if(_FileType == FileClass.FileType.WRITE)
                {
                    m_iListUploadFileID.Add(iFileID);
                }
                if (m_DicClientIDFileIDPair.ContainsKey(_iSenderID))
                {
                    m_DicClientIDFileIDPair[_iSenderID].Add(_iDicKey);
                }else
                {
                    m_DicClientIDFileIDPair.Add(_iSenderID, new List<int>(new int[] { _iDicKey }));
                }
            }
            return iFileID;
        }

    }
}
