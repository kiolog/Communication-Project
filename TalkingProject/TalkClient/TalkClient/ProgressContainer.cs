using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkClient
{
    public partial class ProgressContainer : UserControl
    {
        public enum ProgressType
        {
            UPLOAD = 0,
            DOWNLOAD = 1,
        }
        private class FileLoadingBoxClass
        {
            public FileMessageClass.FileLoadingBox m_LoadingBox;
            public bool m_bSuccess;
            public string m_strFileName;

            public FileLoadingBoxClass(FileMessageClass.FileLoadingBox _LoadingBox, string _strFileName)
            {
                m_LoadingBox = _LoadingBox;
                m_bSuccess = false;
                m_strFileName = _strFileName;
            }
        }
        private List<TableLayoutPanel> m_ListProgressContainer = new List<TableLayoutPanel>();
        private List<Dictionary<int, FileLoadingBoxClass>> m_DicLoadingBoxPair = new List<Dictionary<int, FileLoadingBoxClass>>();
        private TalkClient.DelInsertToQueue m_InsertToServerQueue;
        public ProgressContainer(TalkClient.DelInsertToQueue _CallBack)
        {
            InitializeComponent();
            m_ListProgressContainer.Add(UploadContainer);
            m_ListProgressContainer.Add(DownloadContainer);
            m_InsertToServerQueue = _CallBack;
            m_DicLoadingBoxPair.Add(new Dictionary<int, FileLoadingBoxClass>());
            m_DicLoadingBoxPair.Add(new Dictionary<int, FileLoadingBoxClass>());
        }
        public void AddLoadingFileMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(AddLoadingFile);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddLoadingFile(_Parameter);
            }
        }
        public void SetLoadingBoxSuccessMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(SetLoadingBoxSuccess);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                SetLoadingBoxSuccess(_Parameter);
            }
        }
        public void RemoveLoadingFileMain(object _Parameter)
        {
            if (this.FileProgressContainer.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(RemoveLoadingFile);
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
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(UpdateLoadingBar);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                UpdateLoadingBar(_Parameter);
            }
        }
        public bool IsAllUploadSuccess()
        {
            bool bReturnValue = true;
            Dictionary<int, FileLoadingBoxClass> MyLoadingContainer = m_DicLoadingBoxPair[(int)ProgressType.UPLOAD];
            List<int> iListKey = new List<int>(MyLoadingContainer.Keys);

            int iListCount = iListKey.Count;

            for (int i = 0; i < iListCount; ++i)
            {
                if (!MyLoadingContainer[iListKey[i]].m_bSuccess)
                {
                    bReturnValue = false;
                    break;
                }
            }

            return bReturnValue;
        }
        public List<UIInfoClass> GetAllFileInfo(int _iType)
        {

            Console.WriteLine("GetType : " + _iType);
            List<UIInfoClass> ReturnList = new List<UIInfoClass>();
            Dictionary<int, FileLoadingBoxClass> MyLoadingContainer = m_DicLoadingBoxPair[_iType];
            List<int> iListKey = new List<int>(MyLoadingContainer.Keys);

            int iListCount = iListKey.Count;

            for (int i = 0; i < iListCount; ++i)
            {
                ReturnList.Add(new UIInfoClass(_iID: iListKey[i], _strMessage: MyLoadingContainer[iListKey[i]].m_strFileName));
            }
            return ReturnList;
        }
        private void RemoveLoadingFile(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            int iFileID = MyUIInfo.m_iID;
            int iType = MyUIInfo.m_iValue;
            TableLayoutPanel LoadingContainer = m_ListProgressContainer[iType];
            if(iFileID == -1)
            {
                LoadingContainer.Controls.Clear();
                LoadingContainer.RowStyles.Clear();
                LoadingContainer.RowCount = 1;
                m_DicLoadingBoxPair[iType].Clear();
            }
            else
            {
                FileMessageClass.FileLoadingBox MyLoadingBox = m_DicLoadingBoxPair[iType][iFileID].m_LoadingBox;
                int iIndex = LoadingContainer.Controls.IndexOf(MyLoadingBox);
                LoadingContainer.Controls.RemoveAt(iIndex);
                LoadingContainer.RowStyles.RemoveAt(iIndex);
                m_DicLoadingBoxPair[iType].Remove(iFileID);
            }
        }
        private void SetLoadingBoxSuccess(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            int iFileID = MyUIInfo.m_iID;
            int iProgressValue = MyUIInfo.m_iValue;
            m_DicLoadingBoxPair[iProgressValue][iFileID].m_LoadingBox.SetSuccess();
            m_DicLoadingBoxPair[iProgressValue][iFileID].m_bSuccess = true;
        }
        private void UpdateLoadingBar(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            int iFileID = MyUIInfo.m_iID;
            int iProgressValue = MyUIInfo.m_iValue;
            int iListCount = m_DicLoadingBoxPair.Count;
            for(int i=0;i< iListCount; ++i)
            {
                Dictionary<int, FileLoadingBoxClass> MyDicLoadingBox = m_DicLoadingBoxPair[i];
                if (MyDicLoadingBox.ContainsKey(iFileID))
                {
                    MyDicLoadingBox[iFileID].m_LoadingBox.UpdateProgressBar(iProgressValue);
                }
            }
        }
        private void AddLoadingFile(object _Parameter)
        {
            UIInfoClass MyUIInfo = _Parameter as UIInfoClass;
            string strFileName = MyUIInfo.m_strMessage;
            int iFileID = MyUIInfo.m_iID;
            int iLoadingBarType = MyUIInfo.m_iValue;
            TableLayoutPanel panel = m_ListProgressContainer[iLoadingBarType];
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            FileMessageClass.FileLoadingBox NewFileLoadingBox = new FileMessageClass.FileLoadingBox(strFileName, m_InsertToServerQueue,(ProgressType)iLoadingBarType);
            panel.Controls.Add(NewFileLoadingBox, 0, panel.RowCount - 1);

            m_DicLoadingBoxPair[iLoadingBarType].Add(iFileID, new FileLoadingBoxClass(NewFileLoadingBox, strFileName));

            Console.WriteLine("LoadingBoxSize : " + NewFileLoadingBox.Size);
        }
    }
}
