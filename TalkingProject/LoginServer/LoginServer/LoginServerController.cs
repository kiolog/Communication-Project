using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
using System.Threading;
namespace LoginServer
{
    public class LoginServerController
    {
        private LoginServerModel m_LoginServerModel;
        private ListenModel m_ListenModel;
        private MyThreadPool m_ThreadPool;
        private CheckEventModel m_CheckEventModel;
        private MyTimer m_Timer;

        private int iLoginServerPort = 9001;
        private int iTimerDeltaTime = 1;
        private int m_iEventDeltaTime = 1;
        public void Start()
        {
            m_CheckEventModel = new CheckEventModel(m_iEventDeltaTime,AddEventToThreadPool);
            m_LoginServerModel = new LoginServerModel(InsertToQueue);
            m_ListenModel = new ListenModel(iLoginServerPort, m_LoginServerModel.AddNewClient);
            m_ThreadPool = new MyThreadPool(20);
            m_Timer = new MyTimer(iTimerDeltaTime);
            
            InitSettings();
            m_LoginServerModel.Start();
            m_Timer.AddTimeEvent(iTimerDeltaTime, m_CheckEventModel.Update, null,MyTimer.ETimerMode.REPEAT);
            m_Timer.Start();
            Console.WriteLine("Server Is Ready");
        }
        public void InsertToQueue(object _Parameter)
        {
            List<byte> ListByte = _Parameter as List<byte>;
            if (ListByte[4] != (byte)EventType.SEND && ListByte[4] != (byte)EventType.RECEIVE)
            {
                Console.WriteLine("InsertSocketQueue");
                Console.WriteLine((EventType)ListByte[4]);
            }

            m_CheckEventModel.InsertToQueue(_Parameter);
        }
        public void AddEventToThreadPool(object _Parameter)
        {
            EventClass NewEvent = _Parameter as EventClass;
            m_ThreadPool.QueueUserWorkItem(NewEvent.m_Function, NewEvent.m_Parameter);
        }
        private void InitSettings()
        {
            m_ThreadPool.QueueUserWorkItem(m_ListenModel.StartListen, null);

            m_CheckEventModel.AddCheckEvent((byte)EventType.SEND, m_LoginServerModel.CaseSend);
            m_CheckEventModel.AddCheckEvent((byte)EventType.RECEIVE, m_LoginServerModel.CaseReceive);
            m_CheckEventModel.AddCheckEvent((byte)EventType.INSERTTOSOCKETQUEUE, m_LoginServerModel.CaseInsertToSocketQueue);

            m_CheckEventModel.AddCheckEvent((byte)EventType.SIGNIN, m_LoginServerModel.CaseSignIn);
            m_CheckEventModel.AddCheckEvent((byte)EventType.REGISTER, m_LoginServerModel.CaseRegister);
        }
    }
}
