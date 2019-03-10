using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
using System.Threading;
namespace TalkingServerCmd
{
    public class TalkingServerController
    {
        private TalkingServerModel m_TalkingServerModel;
        private ListenModel m_ListenModel;
        private MyThreadPool m_ThreadPool;
        private CheckEventModel m_CheckEventModel;
        private CheckEventModel m_CheckEventIOModel;
        private MyTimer m_Timer;

        private int iTalkingServerPort = 9002;
        private int iTimerDeltaTime = 1;
        private int m_iNormalEventDeltaTime = 1;
        private int m_iIOEventDeltaTime = 40;

        private List<byte> ListIOEvent = new List<byte>();
        public void Start()
        {
            ListIOEvent.Add((byte)EventType.SENDFILELOOP);
            ListIOEvent.Add((byte)EventType.SENDFILEEND);
            ListIOEvent.Add((byte)EventType.UPLOADFILE);
            ListIOEvent.Add((byte)EventType.DOWNLOADFILE);
            
            m_CheckEventModel = new CheckEventModel(m_iNormalEventDeltaTime,AddEventToThreadPool);
            m_CheckEventIOModel = new CheckEventModel(m_iIOEventDeltaTime,AddEventToThreadPool);
            m_Timer = new MyTimer(iTimerDeltaTime);
            m_TalkingServerModel = new TalkingServerModel(InsertToQueue, m_Timer.AddTimeEvent, m_Timer.RemoveTimeEvent);
            m_ListenModel = new ListenModel(iTalkingServerPort, m_TalkingServerModel.AddNewClient);
            m_ThreadPool = new MyThreadPool(20);
            
            InitIOSettings();
            InitNormalSettings();

            m_Timer.AddTimeEvent(m_iNormalEventDeltaTime, m_CheckEventModel.Update, null,MyTimer.ETimerMode.REPEAT);
            m_Timer.AddTimeEvent(m_iIOEventDeltaTime, m_CheckEventIOModel.Update, null, MyTimer.ETimerMode.REPEAT);
            m_Timer.Start();
            Console.WriteLine("Server Is Ready");

        }
        public void InsertToQueue(object _Parameter)
        {
            List<byte> ListByte = _Parameter as List<byte>;
            byte EventByte = ListByte[4];
            if (ListByte[4] == (byte)EventType.INSERTTOSOCKETQUEUE)
            {
                //Console.WriteLine("Info : " + MyConverter.GetStringFromByteArray(ListByte.GetRange(6, ListByte.Count - 6).ToArray()));
            }
            //Console.WriteLine("Event : ");
            //Console.WriteLine((EventType)ListByte[4]);
            if (ListIOEvent.Contains(EventByte))
            {
                m_CheckEventIOModel.InsertToQueue(_Parameter);
            }
            else
            {
                m_CheckEventModel.InsertToQueue(_Parameter);
            }

        }
        public void AddEventToThreadPool(object _Parameter)
        {
            EventClass NewEvent = _Parameter as EventClass;
            m_ThreadPool.QueueUserWorkItem(NewEvent.m_Function, NewEvent.m_Parameter);
        }
        private void InitIOSettings()
        {
            
            //CaseUploadFile
            m_CheckEventIOModel.AddCheckEvent((byte)EventType.UPLOADFILE, m_TalkingServerModel.CaseUploadFile);
            //CaseSendFile
            m_CheckEventIOModel.AddCheckEvent((byte)EventType.SENDFILELOOP, m_TalkingServerModel.CaseSendFileLoop);
            m_CheckEventIOModel.AddCheckEvent((byte)EventType.SENDFILEEND, m_TalkingServerModel.CaseSendFileEnd);
            //CaseDownloadFile
            m_CheckEventIOModel.AddCheckEvent((byte)EventType.DOWNLOADFILE, m_TalkingServerModel.CaseDownloadFile);
        }
        private void InitNormalSettings()
        {
            m_ThreadPool.QueueUserWorkItem(m_ListenModel.StartListen, null);
            //BaseOperation
            //m_CheckEventModel.AddCheckEvent((byte)EventType.SEND, m_TalkingServerModel.CaseSend);
            m_CheckEventModel.AddCheckEvent((byte)EventType.RECEIVE, m_TalkingServerModel.CaseReceive);
            m_CheckEventModel.AddCheckEvent((byte)EventType.SEND, m_TalkingServerModel.CaseSend);
            m_CheckEventModel.AddCheckEvent((byte)EventType.INSERTTOSOCKETQUEUE, m_TalkingServerModel.CaseInsertToSocketQueue);
            //CaseAddLoginUser
            m_CheckEventModel.AddCheckEvent((byte)EventType.ADDLOGINUSER, m_TalkingServerModel.CaseAddLoginUser);
            //CaseSignIn
            m_CheckEventModel.AddCheckEvent((byte)EventType.SIGNIN, m_TalkingServerModel.CaseSignIn);
            //CaseGetHistoryMessage
            m_CheckEventModel.AddCheckEvent((byte)EventType.GETHISTORYMESSAGE, m_TalkingServerModel.CaseGetHistoryMessage);
            //CaseMessage
            m_CheckEventModel.AddCheckEvent((byte)EventType.MESSAGE, m_TalkingServerModel.CaseMessage);
            m_CheckEventModel.AddCheckEvent((byte)EventType.FILEMESSAGE, m_TalkingServerModel.CaseFileMessage);
            //CaseStopSendFile
            m_CheckEventModel.AddCheckEvent((byte)EventType.STOPSENDFILE, m_TalkingServerModel.CaseStopSendFile);
            //CaseReadFile
            m_CheckEventModel.AddCheckEvent((byte)EventType.READFILE, m_TalkingServerModel.CaseReadFile);

        }
    }
}
