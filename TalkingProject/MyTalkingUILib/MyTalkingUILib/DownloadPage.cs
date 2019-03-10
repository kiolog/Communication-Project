using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace MyTalkingUILib
{
    public partial class DownloadPage : UserControl
    {
        private WaitCallback m_InsertToServerQueue;
        private Dictionary<int, DownloadProgressBar> m_DicFileIDDownloadBarPair = new Dictionary<int, DownloadProgressBar>();
        public DownloadPage(WaitCallback _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
            DownloadContainer.RowCount = 0;
            DownloadContainer.Controls.Clear();
        }
        public void AddNewDownloadBar(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(AddNewDownloadBar_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                AddNewDownloadBar_Implement(_Parameter);
            }
        }
        public void UpdateDownloadProgress(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(UpdateDownloadProgress_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                UpdateDownloadProgress_Implement(_Parameter);
            }
        }
        public void SetFileSize(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(SetFileSize_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                SetFileSize_Implement(_Parameter);
            }
        }
        public void RemoveDownloadFile(object _Parameter)
        {
            if (this.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(RemoveDownloadFile_Implement);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                RemoveDownloadFile_Implement(_Parameter);
            }
        }
        private void AddNewDownloadBar_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyInfo.Dequeue();
            int iMessageID = (int)MyInfo.Dequeue();
            int iOwnerID = (int)MyInfo.Dequeue();
            string strFileName = (string)MyInfo.Dequeue();

            TableLayoutPanel panel = DownloadContainer;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            DownloadProgressBar NewUIObject = new DownloadProgressBar(iFileID, iMessageID,iOwnerID, m_InsertToServerQueue);
            panel.Controls.Add(NewUIObject, 0, panel.RowCount - 1);
            m_DicFileIDDownloadBarPair.Add(iFileID, NewUIObject);
            NewUIObject.SetProperty(new UIInfoClass(new object[] {(int)DownloadProgressBar.EPropertyType.SETALLPROPERTY, panel.RowCount, strFileName}));
        }
        private void UpdateDownloadProgress_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyInfo.Dequeue();
            long lDownloadSpeed = (long)MyInfo.Dequeue();
            long lFilePosition = (long)MyInfo.Dequeue();
            m_DicFileIDDownloadBarPair[iFileID].SetProperty(new UIInfoClass(new object[] { (int)DownloadProgressBar.EPropertyType.UPDATEPROGRESS, lDownloadSpeed, lFilePosition }));
        }
        private void SetFileSize_Implement(object _Parameter)
        {
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyInfo.Dequeue();
            long lFileSize = (long)MyInfo.Dequeue();
            m_DicFileIDDownloadBarPair[iFileID].SetProperty(new UIInfoClass(new object[] { (int)DownloadProgressBar.EPropertyType.SETFILESIZE, lFileSize }));
        }
        private void RemoveDownloadFile_Implement(object _Parameter)
        {
            Console.WriteLine("TotalCount : " + DownloadContainer.Controls.Count);
            UIInfoClass MyInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyInfo.Dequeue();
            DownloadProgressBar RemoveDownloadBar = m_DicFileIDDownloadBarPair[iFileID];
            m_DicFileIDDownloadBarPair.Remove(iFileID);
            int iRemoveIndex = DownloadContainer.Controls.IndexOf(RemoveDownloadBar);
            DownloadContainer.Controls.RemoveAt(iRemoveIndex);
            RefreshIndex();
            Console.WriteLine("AfterTotalCount : " + DownloadContainer.Controls.Count);
        }
        private void RefreshIndex()
        {
            int iListCount = DownloadContainer.Controls.Count;
            for(int i = 0;i< iListCount; ++i)
            {
                (DownloadContainer.Controls[i] as DownloadProgressBar).SetProperty(new UIInfoClass(new object[] { (int)DownloadProgressBar.EPropertyType.SETINDEX, i + 1 }));
            }
        }
    }
}
