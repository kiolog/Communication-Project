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
    public partial class MenuWindow : Form
    {
        public class PersonInfo
        {
            public int m_iID;
            public string m_strName;
            public PersonInfo(int _iID, string _strName)
            {
                m_iID = _iID;
                m_strName = _strName;
            }
        }
        private WaitCallback m_InsertToServerQueue;
        private DownloadPage m_DownloadPage;
        public MenuWindow(WaitCallback _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
            Console.WriteLine("CreateThreadID : " + Thread.CurrentThread.ManagedThreadId);
            m_DownloadPage = new DownloadPage(_CallBack);
            MyDownloadPage.Controls.Add(m_DownloadPage);
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
        public void AddInfo(object _Parameter)
        {
            if (this.FriendMenu.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(AddPersonInfo);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddPersonInfo(_Parameter);
            }
        }
        public void AddDownloadFile(object _Parameter)
        {
            if (this.m_DownloadPage.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(AddDownloadFile_Implement);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddDownloadFile_Implement(_Parameter);
            }
        }
        public void UpdateDownloadProgress(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(UpdateDownloadProgress_Implement);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                UpdateDownloadProgress_Implement(_Parameter);
            }
        }
        public void SetDownloadFileSize(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(SetDownloadFileSize_Implement);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                SetDownloadFileSize_Implement(_Parameter);
            }
        }
        public void RemoveDownloadFile(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(RemoveDownloadFile_Implement);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                RemoveDownloadFile_Implement(_Parameter);
            }
        }
        private void CloseTheForm_Implement(object _Parameter)
        {
            this.Close();
        }
        private void AddPersonInfo(object _Parameter)
        {
            Console.WriteLine("UseThreadID : " + Thread.CurrentThread.ManagedThreadId);
            PersonInfo MyInfo = _Parameter as PersonInfo;

            TableLayoutPanel panel = FriendMenu;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            PersonInfoDLL.PersonInfoClass NewInfo = new PersonInfoDLL.PersonInfoClass(m_InsertToServerQueue);
            panel.Controls.Add(NewInfo, 0, panel.RowCount - 1);
            Console.WriteLine("Name : " + MyInfo.m_strName);
            NewInfo.SetInfo(_iID: MyInfo.m_iID, _strName: MyInfo.m_strName);
        }
        private void AddDownloadFile_Implement(object _Parameter)
        {
            m_DownloadPage.AddNewDownloadBar(_Parameter);
        }
        private void UpdateDownloadProgress_Implement(object _Parameter)
        {
            m_DownloadPage.UpdateDownloadProgress(_Parameter);
        }
        private void SetDownloadFileSize_Implement(object _Parameter)
        {
            m_DownloadPage.SetFileSize(_Parameter);
        }
        private void RemoveDownloadFile_Implement(object _Parameter)
        {
            m_DownloadPage.RemoveDownloadFile(_Parameter);
        }
    }
}
