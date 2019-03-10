using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTalkingUILib;
using System.Threading;
using MyTalkingLib;
namespace TalkingClientMain
{
    public class ViewModel
    {
        private enum FormType
        {
            LOGINFORM,
            MENUWINDOW,
            TALKINGFORM,
        }
        private LoginForm m_LoginForm = null;
        private MenuWindow m_MenuForm = null;
        private TalkingForm m_TalkingForm = null;
        private WaitCallback m_InsertToQueue = null;
        private WaitCallback m_InsertToViewQueue = null;
        private EncryptModel m_EncryptModel = null;

        private Dictionary<int, int> m_DicFileIDRoomIDPair = new Dictionary<int, int>();
        private int m_iAccountID = -1;
        private object m_oTalkingFormLock = new object();
        private MyThreadPool m_ThreadPool = new MyThreadPool(10);
        public ViewModel(WaitCallback _CallBack,WaitCallback _ViewCallBack, EncryptModel _EncryptModel)
        {
            m_InsertToQueue = _CallBack;
            m_InsertToViewQueue = _ViewCallBack;
            m_EncryptModel = _EncryptModel;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        public void Exit()
        {
            if(m_TalkingForm != null)
            {
                Console.WriteLine("CloseTalkingForm");
                try
                {
                    m_TalkingForm.CloseTheForm();
                }
                catch(InvalidOperationException _Exception)
                {
                    Console.WriteLine("Error : " + _Exception);
                }
            }
            if(m_MenuForm != null)
            {
                Console.WriteLine("CloseMenuForm");
                m_MenuForm.CloseTheForm();
            }
            if(m_LoginForm != null)
            {
                Console.WriteLine("CloseLoginForm");
                m_LoginForm.CloseTheForm();
            }
            m_ThreadPool.Exit();
            Console.WriteLine("ViewModel Exit Finished");
        }
        public void CaseChangeState(object _Parameter)
        {
            Console.WriteLine("ViewChangeState");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iState = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            switch ((FlowState)iState)
            {
                case FlowState.LOGIN:
                    RunForm(FormType.LOGINFORM);
                    break;
                case FlowState.TALKING:
                    RunForm(FormType.MENUWINDOW);
                    break;
            }
        }
        private void RunForm(FormType _FormType)
        {
            WaitCallback RunFunction = null;
            switch (_FormType)
            {
                case FormType.LOGINFORM:
                    RunFunction = RunLoginForm;
                    break;
                case FormType.MENUWINDOW:
                    RunFunction = RunMenuWindow;
                    break;
                case FormType.TALKINGFORM:
                    RunFunction = RunTalkingForm;
                    break;
            }
            m_ThreadPool.QueueUserWorkItem(RunFunction, null);
        }
        private void RunLoginForm(object _Parameter = null)
        {
            Console.WriteLine("RunLoginForm");
            m_LoginForm = new LoginForm(m_InsertToQueue, m_EncryptModel);
            Application.Run(m_LoginForm);
        }
        private void RunMenuWindow(object _Parameter = null)
        {
            m_LoginForm.CloseTheForm();
            m_LoginForm = null;
            m_MenuForm = new MenuWindow(m_InsertToQueue);
            Application.Run(m_MenuForm);
        }
        private void RunTalkingForm(object _Parameter = null)
        {
            Console.WriteLine("Talking Form !!!!");
            Application.Run(m_TalkingForm);
            //m_TalkingForm = null;
        }
        public void CaseLoginMessage(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
            m_LoginForm.SetMessage(strMessage);
        }
        public void CaseSetAccountID(object _Parameter)
        {
            Console.WriteLine("SetAccountID");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iAccountID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            m_iAccountID = iAccountID;
        }
        public void CaseMenuData(object _Parameter)
        {
            Console.WriteLine("SetMenuData");
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            if(m_MenuForm == null)
            {
                ReInsertToViewQueue(EventType.MENUDATA, MyInfo);
            }else
            {
                while (ListByte.Count > 0)
                {
                    int iID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                    string strName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
                    lock (m_oTalkingFormLock)
                    {
                        m_MenuForm.AddInfo(new MenuWindow.PersonInfo(iID, strName));
                    }
                }
            }
            
        }
        public void AddTalkingPage(object _Parameter)
        {
            bool bFirst = false;
            lock (m_oTalkingFormLock)
            {
                CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
                int iSenderID = MyInfo.m_iSenderID;
                List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);

                int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                string strName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
                
                if (m_TalkingForm == null)
                {
                    Console.WriteLine("Create Talking From!!!!!!!!!!!!");
                    m_TalkingForm = new TalkingForm(m_InsertToQueue);
                    bFirst = true;
                }
                m_TalkingForm.AddTalkingPage(new UIInfoClass(new object[] { iRoomID, m_iAccountID, strName }));
            }
            

            if (bFirst)
            {
                Console.WriteLine("Run Talking Form");
                RunForm(FormType.TALKINGFORM);
            }
            
        }
        public void CaseMessage(object _Parameter)
        {
            Console.WriteLine("AddTalkingMessage");
            lock (m_oTalkingFormLock)
            {
                AddInfoToTalkingPage(_Parameter, TalkingPage.MessageType.TEXTMESSAGE);
            }
                
        }
        public void CaseFileMessage(object _Parameter)
        {
            lock (m_oTalkingFormLock)
            {
                AddInfoToTalkingPage(_Parameter, TalkingPage.MessageType.FILEMESSAGE);
            }
        }
        public void CaseUploadFile(object _Parameter)
        {
            lock (m_oTalkingFormLock)
            {
                AddLoadingBox(_Parameter, ProgressContainer.ProgressType.UPLOAD);
            }
        }
        public void CaseDownloadFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            if (m_MenuForm == null)
            {
                ReInsertToViewQueue(EventType.MENUDATA, MyInfo);
            }

            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iOwnerID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            m_MenuForm.AddDownloadFile(new UIInfoClass(new object[] { iFileID, iMessageID, iOwnerID, strFileName }));
        }
        public void UpdateLoadingProgress(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            lock (m_DicFileIDRoomIDPair)
            {
                if (m_DicFileIDRoomIDPair.ContainsKey(iFileID))
                {
                    int iProgressPercent = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                    Console.WriteLine("Progress : " + iProgressPercent + "%");
                    int iRoomID = m_DicFileIDRoomIDPair[iFileID];
                    m_TalkingForm.UpdateProgressBar(new UIInfoClass(new object[] { iRoomID, iFileID, iProgressPercent }));
                }
            }
        }
        public void UpdateDownloadProgress(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            long lDownloadSpeed = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            long lFilePosition = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);

            m_MenuForm.UpdateDownloadProgress(new UIInfoClass(new object[] { iFileID, lDownloadSpeed, lFilePosition }));

        }
        public void SetDownloadFileSize(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            long lFileSize = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);

            m_MenuForm.SetDownloadFileSize(new UIInfoClass(new object[] { iFileID, lFileSize }));
        }
        public void CaseCloseFile(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            if(iRoomID == -1)
            {
                int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                CloseDownloadFile(iFileID);
            }else
            {
                while (ListByte.Count > 0)
                {
                    int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
                    lock (m_DicFileIDRoomIDPair)
                    {
                        if (m_DicFileIDRoomIDPair.ContainsKey(iFileID))
                        {
                            m_DicFileIDRoomIDPair.Remove(iFileID);
                            m_TalkingForm.CloseFile(new UIInfoClass(new object[] { iRoomID, iFileID }));
                        }
                    }

                }
            }
            
        }
        private void CloseDownloadFile(int _iFileID)
        {
            m_MenuForm.RemoveDownloadFile(new UIInfoClass(new object[] { _iFileID }));
        }
        private void AddLoadingBox(object _Parameter, ProgressContainer.ProgressType _Type)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            int iFileID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strFileName = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);

            m_DicFileIDRoomIDPair.Add(iFileID, iRoomID);
            lock (m_oTalkingFormLock)
            {
                m_TalkingForm.AddUploadFile(new UIInfoClass(new object[] { iRoomID, strFileName, iFileID, (int)_Type }));
            }
                
        }
        private void AddInfoToTalkingPage(object _Parameter, TalkingPage.MessageType _Type)
        {
            
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = new List<byte>(MyInfo.m_ListByte);
            long lUnixTime = (long)MyConverter.CastToVariable(MyConverter.VariableType.LONG, ListByte);
            int iTheSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iRoomID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            int iMessageID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListByte);
            string strMessage = (string)MyConverter.CastToVariable(MyConverter.VariableType.STRING, ListByte);
            if(m_TalkingForm == null)
            {
                Console.WriteLine("Is Null");
            }
            m_TalkingForm.AddTalkingInfo(new UIInfoClass(new object[] { iRoomID, lUnixTime, strMessage, iTheSenderID == m_iAccountID, (int)_Type, iMessageID, iTheSenderID }));
            
        }
        private void ReInsertToViewQueue(EventType _EventType,CaseInfoClass _Info)
        {
            List<byte> ListSendByte = MyLibFunction.RebuildCommand(_EventType, _Info);
            m_InsertToViewQueue(ListSendByte);
        }
        
    }
}
