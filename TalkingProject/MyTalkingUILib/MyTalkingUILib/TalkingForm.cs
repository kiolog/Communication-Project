using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace MyTalkingUILib
{
    public partial class TalkingForm : Form
    {
        private WaitCallback m_InsertToServerQueue;
        private Dictionary<int, TalkingPage> m_DicTalkingPage = new Dictionary<int, TalkingPage>();
        public TalkingForm(WaitCallback _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
        }
        public void CloseTheForm()
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyUIOperation = new WaitCallback(CloseTheForm_Implement);
                this.Invoke(MyUIOperation, new object[] { null });
            }
            else
            {
                CloseTheForm_Implement(null);
            }
        }
        public void AddTalkingPage(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(AddTalkingPage_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                AddTalkingPage_Implement(_Parameter);
            }
        }
        public void AddTalkingInfo(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(AddTalkingInfo_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                AddTalkingInfo_Implement(_Parameter);
            }
        }
        public void AddUploadFile(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(AddUploadFile_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                AddUploadFile_Implement(_Parameter);
            }
        }
        public void UpdateProgressBar(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(UpdateProgressBar_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                UpdateProgressBar_Implement(_Parameter);
            }
        }
        public void CloseFile(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(CloseFile_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                CloseFile_Implement(_Parameter);
            }
        }
        private void CloseTheForm_Implement(object _Parameter = null)
        {
            this.Close();
        }
        private void CloseFile_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iRoomID = (int)MyInfo.Dequeue();
            m_DicTalkingPage[iRoomID].RemoveLoadingFileMain(MyInfo);
        }
        private void AddTalkingPage_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iRoomID = (int)MyInfo.Dequeue();
            int iMyAccountID = (int)MyInfo.Dequeue();
            string strTabPageText = (string)MyInfo.Dequeue();
            if (!m_DicTalkingPage.ContainsKey(iRoomID))
            {
                TabPage NewTabPage = new TabPage();
                NewTabPage.Text = strTabPageText;
                TalkingPage NewTalkingPage = new TalkingPage(iRoomID, iMyAccountID, m_InsertToServerQueue);
                NewTabPage.Controls.Add(NewTalkingPage);
                TalkingPageContainer.Controls.Add(NewTabPage);

                m_DicTalkingPage.Add(iRoomID, NewTalkingPage);
            }
        }
        private void AddTalkingInfo_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iRoomID = (int)MyInfo.Dequeue();
            if (m_DicTalkingPage.ContainsKey(iRoomID))
            {
                Console.WriteLine("Add Talking Info!!!!!!!!!!!1");
                m_DicTalkingPage[iRoomID].AddInfo(MyInfo);
            }
        }
        private void AddUploadFile_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iRoomID = (int)MyInfo.Dequeue();
            if (m_DicTalkingPage.ContainsKey(iRoomID))
            {
                m_DicTalkingPage[iRoomID].AddLoadingFileMain(MyInfo);
            }
        }
        private void UpdateProgressBar_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iRoomID = (int)MyInfo.Dequeue();
            if (m_DicTalkingPage.ContainsKey(iRoomID))
            {
                m_DicTalkingPage[iRoomID].UpdateLoadingBarMain(MyInfo);
            }
        }

        private void TalkingPageContainer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
