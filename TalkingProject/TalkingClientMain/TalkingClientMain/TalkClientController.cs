using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
namespace TalkingClientMain
{
    public class TalkClientController
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        private EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            //CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            //CTRL_LOGOFF_EVENT = 5,
            //CTRL_SHUTDOWN_EVENT = 6
        }


        private CheckEventModel m_CheckEventModel;
        private CheckEventModel m_CheckViewEventModel;
        private MyTimer m_Timer;
        private MyThreadPool m_ThreadPool;
        private ViewModel m_ViewModel;
        private TalkClientModel m_TalkClientModel;
        private EncryptModel m_EncryptModel;
        private int m_iSleepTime = 1;
        private int m_iEventDeltaTime = 1;
        private int m_iViewEventDeltaTime = 10;
        private string strLoginServerKey = "CzgT7TA15C7JABGN+cdMtdaWeEwj8eUz";
        private string strLoginServerIV = "9M53h3zlZ/E=";
        public void Start()
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            m_Timer = new MyTimer(m_iSleepTime);
            m_TalkClientModel = new TalkClientModel(InsertToQueue, InsertToViewQueue, m_Timer.AddTimeEvent, m_Timer.RemoveTimeEvent);
            m_EncryptModel = new EncryptModel(strLoginServerKey, strLoginServerIV);
            m_ThreadPool = new MyThreadPool(20);
            m_ViewModel = new ViewModel(InsertToQueue,InsertToViewQueue, m_EncryptModel);
            m_CheckEventModel = new CheckEventModel(m_iEventDeltaTime,AddEventToThreadPool);
            m_CheckViewEventModel = new CheckEventModel(m_iViewEventDeltaTime, AddEventToThreadPool);
            
            InitViewSettings();
            InitSettings();

            StartLogin();

            m_Timer.AddTimeEvent(m_iEventDeltaTime, m_CheckEventModel.Update, null,MyTimer.ETimerMode.REPEAT);
            m_Timer.AddTimeEvent(m_iViewEventDeltaTime, m_CheckViewEventModel.Update, null, MyTimer.ETimerMode.REPEAT);
            m_Timer.Start();
        }
        public void InsertToQueue(object _Parameter)
        {
            List<byte> ListByte = _Parameter as List<byte>;
            if(ListByte[4] != (byte)EventType.SEND && ListByte[4] != (byte)EventType.RECEIVE)
            {
                //Console.WriteLine("InsertSocketQueue");
                //Console.WriteLine((EventType)ListByte[4]);
            }
            m_CheckEventModel.InsertToQueue(_Parameter);
        }
        public void InsertToViewQueue(object _Parameter)
        {
            List<byte> ListByte = _Parameter as List<byte>;
            if (ListByte[4] != (byte)EventType.SEND && ListByte[4] != (byte)EventType.RECEIVE)
            {
                //Console.WriteLine("InsertSocketQueue");
                //Console.WriteLine((EventType)ListByte[4]);
            }

            m_CheckViewEventModel.InsertToQueue(_Parameter);
        }
        public void AddEventToThreadPool(object _Parameter)
        {
            EventClass NewEvent = _Parameter as EventClass;
            m_ThreadPool.QueueUserWorkItem(NewEvent.m_Function, NewEvent.m_Parameter);
        }
        private bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            //do your cleanup here
            m_Timer.Pause();
            m_ViewModel.Exit();
            m_ThreadPool.Exit();
            Console.WriteLine("TalkingModel Exit Finished");

            m_TalkClientModel.SaveUnFinishedDownloadFile();

            Console.WriteLine("Cleanup complete");
            Environment.Exit(-1);

            return true;
        }

        private void InitViewSettings()
        {
            CheckEventModel CheckEventNow = m_CheckViewEventModel;
            //CaseChangeState
            CheckEventNow.AddCheckEvent((byte)EventType.CHANGESTATE, m_ViewModel.CaseChangeState);
            //CaseLoginMessage
            CheckEventNow.AddCheckEvent((byte)EventType.LOGINMESSAGE, m_ViewModel.CaseLoginMessage);
            //CaseSetAccountID
            CheckEventNow.AddCheckEvent((byte)EventType.SETTALKINGACCOUNTID, m_ViewModel.CaseSetAccountID);
            //CaseMenuData
            CheckEventNow.AddCheckEvent((byte)EventType.MENUDATA, m_ViewModel.CaseMenuData);
            //AddTalkingPage
            CheckEventNow.AddCheckEvent((byte)EventType.ADDTALKINGPAGE, m_ViewModel.AddTalkingPage);
            //CaseMessage
            CheckEventNow.AddCheckEvent((byte)EventType.MESSAGE, m_ViewModel.CaseMessage);
            //CaseFileMessage
            CheckEventNow.AddCheckEvent((byte)EventType.FILEMESSAGE, m_ViewModel.CaseFileMessage);
            //CaseUploadfile
            CheckEventNow.AddCheckEvent((byte)EventType.UPLOADFILE, m_ViewModel.CaseUploadFile);
            //CaseReadFile
            CheckEventNow.AddCheckEvent((byte)EventType.READFILE, m_ViewModel.UpdateLoadingProgress);
            //CaseSendUploadFile
            CheckEventNow.AddCheckEvent((byte)EventType.SENDUPLOADFILE, m_ViewModel.CaseCloseFile);
            //CaseDownloadFile
            CheckEventNow.AddCheckEvent((byte)EventType.DOWNLOADFILE, m_ViewModel.CaseDownloadFile);
            //CaseSendFile
            CheckEventNow.AddCheckEvent((byte)EventType.SENDFILELOOP, m_ViewModel.UpdateDownloadProgress);
            CheckEventNow.AddCheckEvent((byte)EventType.SENDFILEEND, m_ViewModel.UpdateDownloadProgress);
            //CaseStopSendFile
            CheckEventNow.AddCheckEvent((byte)EventType.STOPSENDFILE, m_ViewModel.CaseCloseFile);
            //CaseSetFileSize
            CheckEventNow.AddCheckEvent((byte)EventType.SETFILESIZE, m_ViewModel.SetDownloadFileSize);
        }
        private void InitSettings()
        {
            CheckEventModel CheckEventNow = m_CheckEventModel;
            //ChangeState
            CheckEventNow.AddCheckEvent((byte)EventType.CHANGESTATE, m_TalkClientModel.ChangeState);
            //Send Receive
            //CheckEventNow.AddCheckEvent((byte)EventType.SEND, m_TalkClientModel.CaseSend);
            CheckEventNow.AddCheckEvent((byte)EventType.RECEIVE, m_TalkClientModel.CaseReceive);
            CheckEventNow.AddCheckEvent((byte)EventType.SEND, m_TalkClientModel.CaseSend);
            //InsertToSocketQueue
            CheckEventNow.AddCheckEvent((byte)EventType.INSERTTOSOCKETQUEUE, m_TalkClientModel.CaseInsertToSocketQueue);
            //LoginServer
            CheckEventNow.AddCheckEvent((byte)EventType.LOGINMESSAGE, m_TalkClientModel.CaseLoginMessage);
            CheckEventNow.AddCheckEvent((byte)EventType.SIGNINSUCCESS, m_TalkClientModel.CaseSignInSuccess);


            //TalkingServer

            //MenuData
            CheckEventNow.AddCheckEvent((byte)EventType.MENUDATA, m_TalkClientModel.CaseMenuData);
            //AddTalkingPage
            CheckEventNow.AddCheckEvent((byte)EventType.ADDTALKINGPAGE, m_TalkClientModel.CaseAddTalkingPage);
            //SetTalkingID
            CheckEventNow.AddCheckEvent((byte)EventType.SETTALKINGACCOUNTID, m_TalkClientModel.CaseSetAccountID);
            //CaseMessage
            CheckEventNow.AddCheckEvent((byte)EventType.MESSAGE, m_TalkClientModel.CaseMessage);
            //CaseFileMessage
            CheckEventNow.AddCheckEvent((byte)EventType.FILEMESSAGE, m_TalkClientModel.CaseFileMessage);
            //CaseUploadfile
            CheckEventNow.AddCheckEvent((byte)EventType.UPLOADFILE, m_TalkClientModel.CaseUploadFile);
            //CaseStartSendFile
            CheckEventNow.AddCheckEvent((byte)EventType.STARTSENDFILE, m_TalkClientModel.CaseStartSendFile);
            //CaseReadFile
            CheckEventNow.AddCheckEvent((byte)EventType.READFILE, m_TalkClientModel.CaseReadFile);
            //CaseCheckFile
            CheckEventNow.AddCheckEvent((byte)EventType.CHECKFILE, m_TalkClientModel.CaseCheckFile);
            //CaseSendUploadFile
            CheckEventNow.AddCheckEvent((byte)EventType.SENDUPLOADFILE, m_TalkClientModel.CaseSendUploadFile);
            //CaseDownloadFile
            CheckEventNow.AddCheckEvent((byte)EventType.DOWNLOADFILE, m_TalkClientModel.CaseDownloadFile);
            //CaseSetFileSize
            CheckEventNow.AddCheckEvent((byte)EventType.SETFILESIZE, m_TalkClientModel.CaseSetFileSize);
            //CaseSendFile
            CheckEventNow.AddCheckEvent((byte)EventType.SENDFILELOOP, m_TalkClientModel.CaseSendFileLoop);
            CheckEventNow.AddCheckEvent((byte)EventType.SENDFILEEND, m_TalkClientModel.CaseSendFileEnd);
            //CaseStopSendFile
            CheckEventNow.AddCheckEvent((byte)EventType.STOPSENDFILE, m_TalkClientModel.CaseStopSendFile);
            //CasePauseDownloadFile
            CheckEventNow.AddCheckEvent((byte)EventType.PAUSEDOWNLOADFILE, m_TalkClientModel.CasePauseDownloadFile);
            //CaseResumeDownloadFile
            CheckEventNow.AddCheckEvent((byte)EventType.RESUMEDOWNLOADFILE, m_TalkClientModel.CaseResumeDownloadFile);
        }
        private WaitCallback CombineCallBack(WaitCallback[] _ArrayCallBack = null)
        {
            return CombineCallBack(new List<WaitCallback>(_ArrayCallBack));
        }
        private WaitCallback CombineCallBack(List<WaitCallback> _ListCallBack = null)
        {
            WaitCallback ReturnResult = null;
            if(_ListCallBack != null && _ListCallBack.Count > 1)
            {
                int iListCount = _ListCallBack.Count;
                ReturnResult = new WaitCallback(_ListCallBack[0]);
                for (int i = 1; i < iListCount; ++i)
                {
                    ReturnResult += _ListCallBack[i];
                }
            }
            return ReturnResult;

        }
        private void StartLogin()
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes(-1));
            ListSendByte.Add((byte)EventType.CHANGESTATE);
            ListSendByte.AddRange(BitConverter.GetBytes((int)FlowState.LOGIN));

            InsertToQueue(ListSendByte);
        }

        

        
    }
}
