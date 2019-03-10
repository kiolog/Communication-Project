using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

using MyPriorityQueueLib;
using MyThreadPoolLib;
using MyConverterLib;
using MyTalkingLib;
namespace TalkClient
{
    
    public delegate void DelInsertToQueue(List<byte> _ListByte);
    public delegate void DelSetUIInfo(object _Parameter);
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public enum ServerType
        {
            LOGINSERVER = 0,
            TALKINGSERVER = 1,
        }
        public class UploadFileClass
        {
            public BinaryReader m_Reader = null;
            public TalkingForm m_TalkingForm = null;
            public BinaryWriter m_Writer = null;
            public long m_lFileSize;
            public UploadFileClass(BinaryReader _Reader = null,BinaryWriter _Writer = null, TalkingForm _TalkingForm = null)
            {
                m_Reader = _Reader;
                m_TalkingForm = _TalkingForm;
                m_Writer = _Writer;
            }
        }

        private enum FlowState
        {
            LOGIN = 0,
            TALKING = 1,
        }
        
        public delegate void DelCheckEvent(List<byte> _ListByte);
        static DelCheckEvent m_CheckEvent = null;
        static ConnectModel m_Connection = null;
        static Dictionary<ServerType, SocketInfo> m_DicServerSocket = new Dictionary<ServerType, SocketInfo>();
        static Dictionary<int, TalkingForm> m_DicTalkingForm = new Dictionary<int, TalkingForm>();

        static MyPriorityQueue<List<byte>> m_PriorityQueue = new MyPriorityQueue<List<byte>>();
        static MyPriorityQueue<List<byte>> m_IOPriorityQueue = new MyPriorityQueue<List<byte>>();

        static MyThreadPool m_ThreadPool = null;
        static LoginForm m_LoginForm;
        static MenuWindow m_MenuForm;

        static int m_iSleepTime = 10;
        static int m_iIOSleepTime = 10;

        static FlowState m_FlowState = FlowState.LOGIN;
        static int m_iTalkingID = -1;
        static int m_iTalkingConnectID = -1;
        static List<UploadFileClass> ListFileReader = new List<UploadFileClass>();
        static List<int> iListReaderUsingID = new List<int>();
        static List<int> iListReaderUnUsingID = new List<int>();
        static object m_oFileIOLock = new object();
        static string m_strTalkServerIP = "";
        static int m_iTalkServerPort = 0;
        private static int iCounter = 0;
        private static string m_strDownloadPathBase = @"C:\Users\yulin.sung\Desktop\TalkingProject\Talking\Downloads\";
        private static object m_oQueueLock = new object();
        private static int m_iReadFileBytes = 1000000;
        private static int m_iUploadFileNumber = 0;
        private static object m_oUploadFileLock = new object();
        private static float m_fUploadSleepTimeMultiplyer = 1.5f; 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //TestForm m_MenuWindow = new TestForm();
            //Application.Run(m_MenuWindow);

            m_PriorityQueue.SetPriorityRule((byte)EventType.MESSAGE, 0);
            m_PriorityQueue.SetPriorityRule((byte)EventType.FILEMESSAGE, 0);

            m_Connection = new ConnectModel(InsertToQueue);
            m_ThreadPool = new MyThreadPool(10);
            Thread ObserveThread = new Thread(CheckAllEvent);
            ObserveThread.Start();
            Thread ObserveIOThread = new Thread(CheckIOEvent);
            ObserveIOThread.Start();

            ChangeState(FlowState.LOGIN);
        }
        static void StartLogin(object _Parameter)
        {
            m_LoginForm = new LoginForm(InsertToQueue);
            Socket ConnectSocket = Connect("127.0.0.1", 9001);
            m_DicServerSocket.Add(ServerType.LOGINSERVER, new SocketInfo((int)ServerType.LOGINSERVER, ConnectSocket));

            List<byte> ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.LOGINSERVER));
            ListByte.Add((byte)EventType.RECEIVE);
            InsertToQueue(ListByte);

            ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes((int)ServerType.LOGINSERVER));
            ListByte.Add((byte)EventType.SEND);
            InsertToQueue(ListByte);


            m_CheckEvent = CheckLoginEvent;

            Application.Run(m_LoginForm);
            //Console.ReadKey();
            //Environment.Exit(1);
        }
        static public Socket Connect(string _strIP, int _iPort)
        {
            IPEndPoint ServerIP = new IPEndPoint(IPAddress.Parse(_strIP), _iPort);
            Socket ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectSocket.Connect(ServerIP);

            return ConnectSocket;
        }
        static public void InsertToQueue(List<byte> _ListByte)
        {
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(_ListByte.ToArray()));
            byte byteEventType = _ListByte[4];
            if(byteEventType == (byte)EventType.READFILE)
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
        static void CheckAllEvent()
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
        static void CheckIOEvent()
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
        static void CheckLoginEvent(List<byte> _ListByte)
        {
            //Console.WriteLine("CheckLoginEvent");
            List<byte> ListMessage = _ListByte;
            //Console.WriteLine("ReceiveLength : " + ListMessage.Count);
            int iSocketID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
            byte EventNumber = (byte)MyConverter.CastToVariable(MyConverter.VariableType.BYTE, ListMessage);
            SocketInfo ReceiveSocketInfo;

            switch ((EventType)EventNumber)
            {
                case EventType.RECEIVE:
                    //Console.WriteLine("Receive");
                    ReceiveSocketInfo = m_DicServerSocket[(ServerType)iSocketID];
                    m_ThreadPool.QueueUserWorkItem(m_Connection.ReceiveMessage, ReceiveSocketInfo);
                    break;
                case EventType.SEND:
                    ReceiveSocketInfo = m_DicServerSocket[(ServerType)iSocketID];
                    m_ThreadPool.QueueUserWorkItem(m_Connection.SendMessage, ReceiveSocketInfo);
                    //Console.WriteLine("Send");
                    break;
                case EventType.SIGNINSUCCESS:
                    //Console.WriteLine("SignInSuccess");
                    m_ThreadPool.QueueUserWorkItem(m_LoginForm.SetMessage, "SignIn Success");
                    //m_QueueBuffer.Clear();
                    m_iTalkingID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
                    int iIPLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
                    m_strTalkServerIP = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListMessage, iIPLength);
                    m_iTalkServerPort = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
                    ChangeState(FlowState.TALKING);
                    break;
                case EventType.SIGNINFAILED:
                    m_ThreadPool.QueueUserWorkItem(m_LoginForm.SetMessage, "SignIn Failed");
                    break;
                case EventType.REGISTERSUCCESS:
                    m_ThreadPool.QueueUserWorkItem(m_LoginForm.SetMessage, "Register Success");
                    break;
                case EventType.REGISTERFAILED:
                    m_ThreadPool.QueueUserWorkItem(m_LoginForm.SetMessage, "Register Failed");
                    break;
                case EventType.INSERTTOSOCKETQUEUE:
                    //Console.WriteLine("Event Insert To Socket");
                    m_ThreadPool.QueueUserWorkItem(CaseInsertToSocketQueue, new CaseInfoClass(iSocketID, ListMessage));
                    break;
                default:
                    Console.WriteLine("Else : " + EventNumber);
                    break;
            }
        }
        static void CheckTalkingEvent(List<byte> _ListByte)
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
                    int iReaderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
                    m_ThreadPool.QueueUserWorkItem(ReadFile, iReaderID);
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
                default:
                    Console.WriteLine("Else : " + EventNumber);
                    break;
            }
        }
        private static void CaseInsertToSocketQueue(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            m_DicServerSocket[(ServerType)iSenderID].InsertToQueue(ListByte);
            //Console.WriteLine("Insert To " + (ServerType)iSenderID + " Socket");

        }
        private static void CaseStartSendFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            long lFileSize = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);

            ListFileReader[iFileID].m_lFileSize = lFileSize;
            //Console.WriteLine("DownloadFileSize : " + lFileSize);
        }
        private static void CaseDownloadFile(object _Parameter)
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
                if (iListReaderUnUsingID.Count > 0)
                {
                    iIndex = iListReaderUnUsingID[0];
                    ListFileReader[iIndex].m_Writer = new BinaryWriter(File.Open(m_strDownloadPathBase + strMessage, FileMode.Create));
                    ListFileReader[iIndex].m_TalkingForm = m_DicTalkingForm[iRoomID];
                    iListReaderUnUsingID.RemoveAt(0);
                    iListReaderUsingID.Add(iIndex);
                }
                else
                {
                    ListFileReader.Add(new UploadFileClass(_Writer:new BinaryWriter(File.Open(m_strDownloadPathBase + strMessage, FileMode.Create)), _TalkingForm: m_DicTalkingForm[iRoomID]));
                    iIndex = ListFileReader.Count - 1;
                    iListReaderUsingID.Add(iIndex);
                }
                m_DicTalkingForm[iRoomID].AddLoadingFileMain(new UIInfoClass(strMessage.Substring(strMessage.LastIndexOf((char)92) + 1), true, iIndex, (int)ProgressContainer.ProgressType.DOWNLOAD));
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
        private static void CaseFileEnd(object _Parameter)
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
                MyFileClass.m_TalkingForm.UpdateLoadingBarMain(new UIInfoClass("", true, iFileID, (int)(100 * (MyWriter.BaseStream.Position / (float)MyFileClass.m_lFileSize))));
                MyWriter.Close();
                //Console.WriteLine("RemoveReaderID : " + iDicFileKey);
            }
            //Console.WriteLine("SendEnd : " + ListByte.Count);
            //Console.WriteLine("DownloadEnd");

        }
        private static void CaseFileLoop(object _Parameter)
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
                MyFileClass.m_TalkingForm.UpdateLoadingBarMain(new UIInfoClass("", true, iFileID, (int)(100 * (MyWriter.BaseStream.Position / (float)MyFileClass.m_lFileSize))));
            }
            //Console.WriteLine(" ReceiveBytes : " + ListByte.Count);
        }
        static void CaseFileMessage(object _Parameter)
        {
            //Console.WriteLine("ReceiveFileMessage");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);
            if (m_DicTalkingForm.ContainsKey(iRoomID))
            {
                m_ThreadPool.QueueUserWorkItem(m_DicTalkingForm[iRoomID].AddInfo, new UIInfoClass(strMessage, iSenderID == m_iTalkingConnectID, iMessageID,(int)TalkingForm.MessageType.FILEMESSAGE, iSenderID));
            }
            else
            {
                Console.WriteLine("NotThere");
            }
            //Console.WriteLine("ReceiveFileMessageEnd");
        }
        static void CaseCheckFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            int iReaderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            long lFileLength = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            //Console.WriteLine("CheckFile");
            if(lFileLength == ListFileReader[iReaderID].m_Reader.BaseStream.Length)
            {
                Console.WriteLine("UploadSuccess");
                ListFileReader[iReaderID].m_Reader.Close();
                iListReaderUsingID.Remove(iReaderID);
                iListReaderUnUsingID.Add(iReaderID);
                //Console.WriteLine("ReadOver : " + iReaderID);
                ListFileReader[iReaderID].m_TalkingForm.SetLoadingBoxSuccessMain(new UIInfoClass(_iID: iReaderID));
                lock (m_oUploadFileLock)
                {
                    --m_iUploadFileNumber;
                    if(m_iUploadFileNumber == 0)
                    {
                        m_iIOSleepTime = 1000;
                    }
                    else
                    {
                        m_iIOSleepTime = (int)(m_iUploadFileNumber * m_iSleepTime * m_fUploadSleepTimeMultiplyer);
                    }
                }
            }
            else
            {
                Console.WriteLine("UploadFailed");
            }
        }
        static void CaseSendUploadFile(object _Parameter)
        {
            //Console.WriteLine("SendUploadFileStart");
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
                for(int i=0;i< iListCount; ++i)
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
                MyTalkingForm.RemoveLoadingFileMain(new UIInfoClass(_iID:-1,_iValue:(int)ProgressContainer.ProgressType.UPLOAD));

            }
            //Console.WriteLine("SendUploadFileEnd");
        }
        static void CaseGetConnectID(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            m_iTalkingConnectID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
        }
        static void CaseUploadFile(object _Parameter)
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
                    ListFileReader[iIndex].m_TalkingForm = m_DicTalkingForm[iReceiverID];
                    iListReaderUnUsingID.RemoveAt(0);
                    iListReaderUsingID.Add(iIndex);
                }
                else
                {
                    ListFileReader.Add(new UploadFileClass(_Reader:new BinaryReader(File.Open(strMessage, FileMode.Open,FileAccess.Read,FileShare.Read)),_TalkingForm: m_DicTalkingForm[iReceiverID]));
                    iIndex = ListFileReader.Count - 1;
                    iListReaderUsingID.Add(iIndex);
                }
                
                m_DicTalkingForm[iReceiverID].AddLoadingFileMain(new UIInfoClass(strMessage.Substring(strMessage.LastIndexOf((char)92) + 1), true, iIndex, (int)ProgressContainer.ProgressType.UPLOAD));
                
                SendStart(strMessage, iIndex);

                lock (m_oUploadFileLock)
                {
                    ++m_iUploadFileNumber;
                    
                }


            }
            //m_iReadFileBytes = m_iReadFileBytesBase / ListFileReader.Count;
        }
        static void SendStart(string _strFileName,int _iFileID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.STARTSENDFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            ListSendByte.AddRange(BitConverter.GetBytes(_strFileName.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(_strFileName));

            InsertToQueue(ListSendByte);

            //AddReadFileEvent(_iFileID);
        }
        static void AddReadFileEvent(int _iFileID)
        {
            List<byte>  ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.READFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iFileID));
            InsertToQueue(ListSendByte);
        }
        static void ReadFile(object _Parameter)
        {
            
            int iReaderID = (int)_Parameter;
            
            UploadFileClass MyUploadFileClass = ListFileReader[iReaderID];
            BinaryReader MyReader = MyUploadFileClass.m_Reader;
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);

            int iSendLength = 0;
            List<byte> ListSendByteTemp = new List<byte>(ListSendByte);

            
            if (MyReader.BaseStream.Position + m_iReadFileBytes >= MyReader.BaseStream.Length)
            {
                iSendLength = (int)(MyReader.BaseStream.Length - MyReader.BaseStream.Position);
                ListSendByteTemp.Add((byte)EventType.FILEEND);
            }
            else
            {
                iSendLength = m_iReadFileBytes;
                ListSendByteTemp.Add((byte)EventType.FILE);
            }
            ListSendByteTemp.AddRange(BitConverter.GetBytes(iReaderID));
            ListSendByteTemp.AddRange(BitConverter.GetBytes(iSendLength));
            long lFilePosition;
            long lFileSize = MyReader.BaseStream.Length;
            lock (m_oFileIOLock)
            {
                ListSendByteTemp.AddRange(MyReader.ReadBytes(iSendLength));
                lFilePosition = MyReader.BaseStream.Position;
            }
            InsertToQueue(ListSendByteTemp);
            MyUploadFileClass.m_TalkingForm.UpdateLoadingBarMain(new UIInfoClass("", true, iReaderID, (int)(100 * (lFilePosition / (float)lFileSize))));

            if (lFilePosition != lFileSize)
            {
                AddReadFileEvent(iReaderID);
                //Console.WriteLine("AddReadFileEvent : " + iReaderID);
            }
            //Console.WriteLine("FileCounter : " + iCounter);
            ++iCounter;


        }
        static void CaseMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;

            List<byte> ListByte = MyInfo.m_ListByte;
            int iSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);

            int iMessageLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte, iMessageLength);
            if (m_DicTalkingForm.ContainsKey(iRoomID))
            {
                m_ThreadPool.QueueUserWorkItem(m_DicTalkingForm[iRoomID].AddInfo, new UIInfoClass(strMessage, iSenderID == m_iTalkingConnectID, iMessageID, (int)TalkingForm.MessageType.TEXTMESSAGE));
            }
            else
            {
                Console.WriteLine("NotThere");
            }
        }
        
        static void CaseCreateTalkingWindow(List<byte> _ListByte)
        {
            int iID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, _ListByte);
            m_ThreadPool.QueueUserWorkItem(RunTalkingWindow, iID);
        }
        
        static void RunTalkingWindow(object _Parameter)
        {

            int iID = (int)_Parameter;
            TalkingForm MyTalkingForm = new TalkingForm(iID, InsertToQueue);
            if (!m_DicTalkingForm.ContainsKey(iID))
            {
                m_DicTalkingForm.Add(iID, MyTalkingForm);
            }else
            {
                m_DicTalkingForm[iID] = MyTalkingForm;
            }
            GetHistoryMessage(iID);
            Application.Run(MyTalkingForm);
        }
        static void GetHistoryMessage(int _iReceiverID)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)EventType.GETHISTORYMESSAGE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iReceiverID));

            InsertToQueue(ListSendByte);
        }
        static void CaseMenuData(List<byte> _ListByte)
        {
            while(_ListByte.Count > 0)
            {
                int iID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, _ListByte);
                int iNameLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, _ListByte);
                string strName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, _ListByte, iNameLength);

                m_ThreadPool.QueueUserWorkItem(m_MenuForm.AddInfo, new MenuWindow.PersonInfo(iID, strName));
            }
        }
        static void ChangeState(FlowState _State)
        {
            switch (_State)
            {
                case FlowState.LOGIN:
                    m_ThreadPool.QueueUserWorkItem(StartLogin, null);
                    break;
                case FlowState.TALKING:
                    m_ThreadPool.QueueUserWorkItem(StartTalking, null);
                    break;
            }
        }
        static void StartTalking(object _Parameter)
        {
            m_CheckEvent = CheckTalkingEvent;

            Socket ConnectSocket = Connect(m_strTalkServerIP, m_iTalkServerPort);
            m_DicServerSocket.Add(ServerType.TALKINGSERVER, new SocketInfo((int)ServerType.TALKINGSERVER, ConnectSocket));

            m_LoginForm.CloseTheForm();
            m_MenuForm = new MenuWindow(InsertToQueue);

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

            InsertToQueue(ListByte);
            

            
            Application.Run(m_MenuForm);
            Environment.Exit(1);
        }
    }
    
}
