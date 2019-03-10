using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using MyPriorityQueueLib;
using MyThreadPoolLib;
using MyTalkingLib;
namespace TalkingClientCmd
{
    
    public delegate void DelInsertToQueue(List<byte> _ListByte);
    class ClientMain
    {
        public enum ServerType
        {
            LOGINSERVER = 0,
            TALKINGSERVER = 1,
        }
        public class UploadFileClass
        {
            public BinaryReader m_Reader = null;
            public BinaryWriter m_Writer = null;
            public long m_lFileSize;
            public UploadFileClass(BinaryReader _Reader = null, BinaryWriter _Writer = null)
            {
                m_Reader = _Reader;
                m_Writer = _Writer;
            }
        }
        public delegate void DelCheckEvent(List<byte> _ListByte);
        private DelCheckEvent m_CheckEvent = null;
        private ConnectModel m_Connection = null;
        private Dictionary<ServerType, SocketInfo> m_DicServerSocket = new Dictionary<ServerType, SocketInfo>();

        private MyPriorityQueue<List<byte>> m_PriorityQueue = new MyPriorityQueue<List<byte>>();
        private MyPriorityQueue<List<byte>> m_IOPriorityQueue = new MyPriorityQueue<List<byte>>();

        private MyThreadPool m_ThreadPool = null;

        private int m_iSleepTime = 1;
        private int m_iIOSleepTime = 40;
        private int m_iIOSleepTimeBase = 40;

        private int m_iTalkingID = -1;
        private int m_iTalkingConnectID = -1;
        private List<UploadFileClass> ListFileReader = new List<UploadFileClass>();
        private List<int> iListReaderUsingID = new List<int>();
        private List<int> iListReaderUnUsingID = new List<int>();
        private object m_oFileIOLock = new object();
        private string m_strTalkServerIP = "";
        private int m_iTalkServerPort = 0;
        private int iCounter = 0;
        private string m_strDownloadPathBase = @"D:\TalkingProjectDownload\Download\";
        private object m_oQueueLock = new object();
        private int m_iReadFileBytes = 1000000;
        private int m_iUploadFileNumber = 0;
        private object m_oUploadFileLock = new object();
        private float m_fUploadSleepTimeMultiplyer = 1.5f;
        private int m_iCounter = 0;
        private static List<byte> m_ListIOEvent = new List<byte>();

        public ClientMain(string _strIP,int _iPort,int _iTalkingID)
        {
            m_strTalkServerIP = _strIP;
            m_iTalkServerPort = _iPort;
            m_iTalkingID = _iTalkingID;
            m_strDownloadPathBase += m_iTalkingID.ToString() + @"\";
        }
        public void Start()
        {
            m_ListIOEvent.Add((byte)EventType.FILE);
            m_ListIOEvent.Add((byte)EventType.STARTSENDFILE);
            m_ListIOEvent.Add((byte)EventType.FILEEND);
            //m_ListIOEvent.Add((byte)EventType.READFILE);

            m_PriorityQueue.SetPriorityRule((byte)EventType.SETIOSLEEPTIME, 0);
            m_PriorityQueue.SetPriorityRule((byte)EventType.MESSAGE, 1);
            m_PriorityQueue.SetPriorityRule((byte)EventType.FILEMESSAGE, 1);

            m_Connection = new ConnectModel(this.InsertToQueue);
            m_ThreadPool = new MyThreadPool(1);
            
            StartTalking(null);

            Thread ObserveThread = new Thread(CheckAllEvent);
            ObserveThread.Start();
            Thread ObserveIOThread = new Thread(CheckIOEvent);
            ObserveIOThread.Start();
        }
        private Socket Connect(string _strIP, int _iPort)
        {
            IPEndPoint ServerIP = new IPEndPoint(IPAddress.Parse(_strIP), _iPort);
            Socket ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectSocket.Connect(ServerIP);

            return ConnectSocket;
        }
        public void SendTextMessage(int _iTargetID,string _strMessage)
        {
            List<byte>  ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.MESSAGE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iTargetID));
            byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(_strMessage);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
            ListSendByte.AddRange(strByteArray);
            InsertToQueue(ListSendByte);
        }
        public void UploadFile(string _strFileName,int _iReceiverID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.UPLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iReceiverID));
            byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(_strFileName);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
            ListSendByte.AddRange(strByteArray);
            InsertToQueue(ListSendByte);
        }
        public void InsertToQueue(List<byte> _ListByte)
        {
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(_ListByte.ToArray()));
            byte byteEventType = _ListByte[4];
            //m_PriorityQueue.Enqueue(byteEventType, _ListByte);
            if (m_ListIOEvent.Contains(byteEventType))
            {
                m_IOPriorityQueue.Enqueue(byteEventType, _ListByte);
            }
            else
            {
                m_PriorityQueue.Enqueue(byteEventType, _ListByte);
            }
            //m_LoginForm.SetMessage("HEllo");
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(_ListByte.ToArray()));
        }
        private void CheckAllEvent()
        {
            while (true)
            {
                Thread.Sleep(m_iSleepTime);
                if (m_PriorityQueue.Count > 0)
                {
                    List<byte> ListMessage = m_PriorityQueue.Dequeue();


                    m_CheckEvent(ListMessage);
                }
            }
        }
        private void CheckIOEvent()
        {
            while (true)
            {
                Thread.Sleep(m_iIOSleepTime);
                if (m_IOPriorityQueue.Count > 0)
                {
                    List<byte> ListMessage = m_IOPriorityQueue.Dequeue();
                    m_CheckEvent(ListMessage);
                }
            }
        }
        private void CheckTalkingEvent(List<byte> _ListByte)
        {
            List<byte> ListMessage = _ListByte;
            //Console.WriteLine("CheckEventStart");
            int iSocketID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
            byte EventNumber = (byte)MyConverter.CastToVariable(MyConverter.VariableType.BYTE, ListMessage);
            SocketInfo ReceiveSocketInfo;
            //Console.WriteLine("CheckEventEnd");

            switch ((EventType)EventNumber)
            {
                case EventType.RECEIVE:
                    ReceiveSocketInfo = m_DicServerSocket[(ServerType)iSocketID];
                    //Console.WriteLine("Receive Server : " + iSocketID);
                    m_ThreadPool.QueueUserWorkItem(m_Connection.ReceiveMessage, ReceiveSocketInfo);
                    break;
                case EventType.SEND:
                    //Console.WriteLine(m_iTalkingID + " : Send");
                    ReceiveSocketInfo = m_DicServerSocket[(ServerType)iSocketID];
                    m_ThreadPool.QueueUserWorkItem(m_Connection.SendMessage, ReceiveSocketInfo);
                    break;
                case EventType.MENUDATA:
                    //Console.WriteLine("MenuData");
                    CaseMenuData(ListMessage);
                    break;
                case EventType.CREATETALKINGWINDOW:
                    CaseCreateTalkingWindow(ListMessage);
                    break;
                case EventType.MESSAGE:
                    m_ThreadPool.QueueUserWorkItem(CaseMessage, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.UPLOADFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseUploadFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.READFILE:
                    m_ThreadPool.QueueUserWorkItem(ReadFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.GETCONNECTID:
                    m_ThreadPool.QueueUserWorkItem(CaseGetConnectID, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.CHECKFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseCheckFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.SENDUPLOADFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseSendUploadFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.FILEMESSAGE:
                    m_ThreadPool.QueueUserWorkItem(CaseFileMessage, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.DOWNLOADFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseDownloadFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.FILE:
                    m_ThreadPool.QueueUserWorkItem(CaseFileLoop, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.FILEEND:
                    m_ThreadPool.QueueUserWorkItem(CaseFileEnd, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.STARTSENDFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseStartSendFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.INSERTTOSOCKETQUEUE:
                    m_ThreadPool.QueueUserWorkItem(CaseInsertToSocketQueue, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.SETIOSLEEPTIME:
                    m_ThreadPool.QueueUserWorkItem(CaseSetIOSleepTime, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.STARTREADERFILE:
                    m_ThreadPool.QueueUserWorkItem(CaseStartReadFile, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                case EventType.READY:
                    m_ThreadPool.QueueUserWorkItem(CaseReady, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                default:
                    Console.WriteLine("Else : " + EventNumber);
                    break;
            }
        }
        private void CaseReady(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            string strFileName = @"C:\Users\yulin.sung\Desktop\TalkingProject\Talking\10MB";
            List<byte> ListByte = MyInfo.m_ListByte;

            for(int i = 0; i < 1; ++i)
            {
                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes((int)TalkingClientCmd.ClientMain.ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.UPLOADFILE);
                ListSendByte.AddRange(BitConverter.GetBytes(1));
                byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(strFileName);
                ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListSendByte.AddRange(strByteArray);
                InsertToQueue(ListSendByte);
            }

            Console.WriteLine(m_iTalkingID + " is Ready");
        }
        private void CaseStartReadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            Console.WriteLine("StartRead : " + iFileID);
            AddReadFileEvent(iFileID);

        }
        private void CaseSetIOSleepTime(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iMultiplyer = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            m_iIOSleepTime = m_iIOSleepTimeBase * iMultiplyer;
            Console.WriteLine("SetIOSleepTime : " + m_iIOSleepTime);

        }
        private void CaseInsertToSocketQueue(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            m_DicServerSocket[(ServerType)iSenderID].InsertToQueue(ListByte);
            //Console.WriteLine("Insert To " + (ServerType)iSenderID + " Socket");

        }
        private void CaseStartSendFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            long lFileSize = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);

            ListFileReader[iFileID].m_lFileSize = lFileSize;
            //Console.WriteLine("DownloadFileSize : " + lFileSize);
        }
        private void CaseDownloadFile(object _Parameter)
        {
            //Console.WriteLine("StartToDownload");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);
            //Console.WriteLine("DownloadFileID : " + iMessageID);
            //Console.WriteLine("DownloadFileName : " + strMessage);
            int iIndex = 0;
            lock (m_oFileIOLock)
            {
                if (!Directory.Exists(m_strDownloadPathBase))
                {
                    Directory.CreateDirectory(m_strDownloadPathBase);
                }
                if (iListReaderUnUsingID.Count > 0)
                {
                    iIndex = iListReaderUnUsingID[0];
                    ListFileReader[iIndex].m_Writer = new BinaryWriter(File.Open(m_strDownloadPathBase + strMessage, FileMode.Create));
                    iListReaderUnUsingID.RemoveAt(0);
                    iListReaderUsingID.Add(iIndex);
                }
                else
                {
                    ListFileReader.Add(new UploadFileClass(_Writer: new BinaryWriter(File.Open(m_strDownloadPathBase + strMessage, FileMode.Create))));
                    iIndex = ListFileReader.Count - 1;
                    iListReaderUsingID.Add(iIndex);
                }
                
            }


            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.DOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
            ListSendByte.AddRange(BitConverter.GetBytes(iIndex));
            ListSendByte.AddRange(BitConverter.GetBytes(iOwnerID));

            InsertToQueue(ListSendByte);


        }
        private void CaseFileEnd(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            UploadFileClass MyFileClass = ListFileReader[iFileID];
            lock (m_oFileIOLock)
            {
                BinaryWriter MyWriter = MyFileClass.m_Writer;
                MyWriter.Write(ListByte.ToArray());
                MyWriter.Close();
                //Console.WriteLine("RemoveReaderID : " + iDicFileKey);
            }
            //Console.WriteLine("SendEnd : " + ListByte.Count);
            Console.WriteLine("DownloadEnd : " + iSenderID);

        }
        private void CaseFileLoop(object _Parameter)
        {


            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;

            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iDicFileKey = (iSenderID << 16) + iFileID;
            UploadFileClass MyFileClass = ListFileReader[iFileID];
            lock (m_oFileIOLock)
            {
                //Console.WriteLine("ReaderLoopID : " + iDicFileKey);
                BinaryWriter MyWriter = MyFileClass.m_Writer;
                MyWriter.Write(ListByte.ToArray());

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes(iSenderID));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.READFILE);
                ListSendByte.AddRange(BitConverter.GetBytes(iFileID));
                Console.WriteLine("ReadID : " + iSenderID);
                InsertToQueue(ListSendByte);

            }
            Console.WriteLine("FileCounter : " + m_iCounter);
            ++m_iCounter;
        }
        private void CaseFileMessage(object _Parameter)
        {
            //Console.WriteLine("ReceiveFileMessage");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iTheSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);
            
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.DOWNLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(iRoomID));
            ListSendByte.AddRange(BitConverter.GetBytes(iMessageID));
            ListSendByte.AddRange(BitConverter.GetBytes(iTheSenderID));
            ListSendByte.AddRange(BitConverter.GetBytes(iMessageLength));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(strMessage));
            InsertToQueue(ListSendByte);
            //Console.WriteLine("ReceiveFileMessageEnd");
        }
        private void CaseCheckFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iReaderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            long lFileLength = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            //Console.WriteLine("CheckFile");
            if (lFileLength == ListFileReader[iReaderID].m_Reader.BaseStream.Length)
            {
                Console.WriteLine("UploadSuccess");
                ListFileReader[iReaderID].m_Reader.Close();
                iListReaderUsingID.Remove(iReaderID);
                iListReaderUnUsingID.Add(iReaderID);

                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.FILEMESSAGE);
                ListSendByte.AddRange(BitConverter.GetBytes(m_iTalkingConnectID));
                ListSendByte.AddRange(BitConverter.GetBytes(iReaderID));
                string strFileName = "100MB";
                byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(strFileName);
                ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
                ListSendByte.AddRange(strByteArray);
                InsertToQueue(ListSendByte);
                //Console.WriteLine("ReadOver : " + iReaderID);
            }
            else
            {
                Console.WriteLine("UploadFailed");
            }
        }
        private void CaseSendUploadFile(object _Parameter)
        {
            /*//Console.WriteLine("SendUploadFileStart");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iReceiverID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            TalkingForm MyTalkingForm = m_DicTalkingForm[iReceiverID];
            if (MyTalkingForm.IsAllUploadSuccess())
            {
                List<UIInfoClass> ListFileName = MyTalkingForm.GetAllFileInfo((int)ProgressContainer.ProgressType.UPLOAD);

                //Console.WriteLine("UploadFileCount : " + ListFileName.Count);
                List<byte> ListSendByte = new List<byte>();
                ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
                ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                ListSendByte.Add((byte)EventType.FILEMESSAGE);
                ListSendByte.AddRange(BitConverter.GetBytes(iReceiverID));

                int iListCount = ListFileName.Count;
                for (int i = 0; i < iListCount; ++i)
                {
                    UIInfoClass MyUIInfo = ListFileName[i];
                    List<byte> ListSendByteTemp = new List<byte>(ListSendByte);
                    ListSendByteTemp.AddRange(BitConverter.GetBytes(MyUIInfo.m_iID));
                    //Console.WriteLine("UploadFileName : " + MyUIInfo.m_strMessage);
                    byte[] strMessageArr = System.Text.Encoding.UTF8.GetBytes(MyUIInfo.m_strMessage);

                    ListSendByteTemp.AddRange(BitConverter.GetBytes(strMessageArr.Length));
                    ListSendByteTemp.AddRange(strMessageArr);

                    InsertToQueue(ListSendByteTemp);
                }

            }*/
            //Console.WriteLine("SendUploadFileEnd");
        }
        private void CaseGetConnectID(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            m_iTalkingConnectID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            Console.WriteLine(iSenderID + " Get ConnectID : " + m_iTalkingConnectID);
        }
        private void CaseUploadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iReceiverID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);

            lock (m_oFileIOLock)
            {
                int iIndex = 0;
                if (iListReaderUnUsingID.Count > 0)
                {
                    iIndex = iListReaderUnUsingID[0];
                    ListFileReader[iIndex].m_Reader = new BinaryReader(File.Open(strMessage, FileMode.Open, FileAccess.Read, FileShare.Read));
                    iListReaderUnUsingID.RemoveAt(0);
                    iListReaderUsingID.Add(iIndex);
                }
                else
                {
                    ListFileReader.Add(new UploadFileClass(_Reader: new BinaryReader(File.Open(strMessage, FileMode.Open, FileAccess.Read, FileShare.Read))));
                    iIndex = ListFileReader.Count - 1;
                    iListReaderUsingID.Add(iIndex);
                }
                SendStart(strMessage, iIndex);

                lock (m_oUploadFileLock)
                {
                    ++m_iUploadFileNumber;
                    //m_iIOSleepTime =  m_iIOSleepTime;
                }


            }
            //m_iReadFileBytes = m_iReadFileBytesBase / ListFileReader.Count;
        }
        private void SendStart(string _strFileName, int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.STARTSENDFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(_strFileName.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(_strFileName));
            Console.WriteLine("SendStart : " + _iFileID);
     
            InsertToQueue(ListSendByte);

            //AddReadFileEvent(_iFileID);
        }
        private void AddReadFileEvent(int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.READFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            InsertToQueue(ListSendByte);
        }
        private void ReadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iReaderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            UploadFileClass MyUploadFileClass = ListFileReader[iReaderID];
            BinaryReader MyReader = MyUploadFileClass.m_Reader;
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);

            int iSendLength = 0;


            if (MyReader.BaseStream.Position + m_iReadFileBytes >= MyReader.BaseStream.Length)
            {
                iSendLength = (int)(MyReader.BaseStream.Length - MyReader.BaseStream.Position);
                ListSendByte.Add((byte)EventType.FILEEND);
            }
            else
            {
                iSendLength = m_iReadFileBytes;
                ListSendByte.Add((byte)EventType.FILE);
            }
            ListSendByte.AddRange(BitConverter.GetBytes(iReaderID));
            ListSendByte.AddRange(BitConverter.GetBytes(iSendLength));
            long lFilePosition;
            long lFileSize = MyReader.BaseStream.Length;
            lock (m_oFileIOLock)
            {
                ListSendByte.AddRange(MyReader.ReadBytes(iSendLength));
                lFilePosition = MyReader.BaseStream.Position;
            }
            InsertToQueue(ListSendByte);

            if (lFilePosition != lFileSize)
            {
                //AddReadFileEvent(iReaderID);
                //Console.WriteLine("AddReadFileEvent : " + iReaderID);
            }
            //Console.WriteLine("FileCounter : " + iCounter);
            ++iCounter;


        }
        private void CaseMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iTheSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);
        }

        private void CaseCreateTalkingWindow(List<byte> _ListByte)
        {
        }

        private void GetHistoryMessage(int _iReceiverID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.GETHISTORYMESSAGE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iReceiverID));

            InsertToQueue(ListSendByte);
        }
        private void CaseMenuData(List<byte> _ListByte)
        {
        }
        private void StartTalking(object _Parameter)
        {
            m_CheckEvent = CheckTalkingEvent;

            Socket ConnectSocket = Connect(m_strTalkServerIP, m_iTalkServerPort);
            m_DicServerSocket.Add(ServerType.TALKINGSERVER, new SocketInfo((int)ServerType.TALKINGSERVER, ConnectSocket));

            List<byte> ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.RECEIVE);
            InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.SEND);
            InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.TALKINGSERVER));
            ListByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListByte.Add((byte)EventType.INIT);
            ListByte.AddRange(BitConverter.GetBytes(m_iTalkingID));
            Console.WriteLine("Init : " + m_iTalkingID);
            InsertToQueue(ListByte);
        }
    }
}
    

