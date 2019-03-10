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
using MyTalkingUILib;
namespace MyTalkingUILib
{
    public partial class ProgressContainer : UserControl
    {
        public enum ProgressType
        {
            UPLOAD = 0,
            DOWNLOAD = 1,
        }
        private int m_iRoomID = -1;
        private Dictionary<int, FileLoadingBox> m_DicLoadingBoxPair = new Dictionary<int, FileLoadingBox>();
        private WaitCallback m_InsertToServerQueue;
        public ProgressContainer(WaitCallback _CallBack,int _iRoomID)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
            m_iRoomID = _iRoomID;
        }
        public void AddLoadingFileMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                WaitCallback MyInvoke = new WaitCallback(AddLoadingFile);
                this.Invoke(MyInvoke, new object[] { _Parameter });
            }
            else
            {
                AddLoadingFile(_Parameter);
            }
        }
        public void RemoveLoadingFileMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(RemoveLoadingFile);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                RemoveLoadingFile(_Parameter);
            }
        }
        public void UpdateLoadingBarMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                WaitCallback MySetInfo = new WaitCallback(UpdateLoadingBar);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                UpdateLoadingBar(_Parameter);
            }
        }
        private void RemoveLoadingFile(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyUIInfo.Dequeue();
            TableLayoutPanel LoadingContainer = UploadContainer;
            if(LoadingContainer != null)
            {
                FileLoadingBox MyLoadingBox = m_DicLoadingBoxPair[iFileID];
                int iIndex = LoadingContainer.Controls.IndexOf(MyLoadingBox);
                LoadingContainer.Controls.RemoveAt(iIndex);
                LoadingContainer.RowStyles.RemoveAt(iIndex);
                m_DicLoadingBoxPair.Remove(iFileID);
            }
        }
        private void UpdateLoadingBar(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            int iFileID = (int)MyUIInfo.Dequeue();
            int iProgressValue = (int)MyUIInfo.Dequeue();
            m_DicLoadingBoxPair[iFileID].UpdateProgressBar(iProgressValue);
        }
        private void AddLoadingFile(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            string strFileName = (string)MyUIInfo.Dequeue();
            int iFileID = (int)MyUIInfo.Dequeue();
            TableLayoutPanel panel = UploadContainer;

            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            FileLoadingBox NewFileLoadingBox = new FileLoadingBox(m_iRoomID,iFileID, strFileName, m_InsertToServerQueue);
            panel.Controls.Add(NewFileLoadingBox, 0, panel.RowCount - 1);

            m_DicLoadingBoxPair.Add(iFileID, NewFileLoadingBox);

            //Console.WriteLine("LoadingBoxSize : " + NewFileLoadingBox.Size);
        }
    }
}
