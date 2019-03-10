using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileMessageClass
{
    public partial class FileLoadingBox : UserControl
    {
        
        private int iMaxFileNameLength = 22;
        private bool m_iSuccess = false;
        private TalkClient.ProgressContainer.ProgressType m_ProgressType;
        private TalkClient.DelInsertToQueue m_InsertToServerQueue;
        public FileLoadingBox(string _FileName, TalkClient.DelInsertToQueue _InsertToQueue, TalkClient.ProgressContainer.ProgressType _ProgressType)
        {
            InitializeComponent();
            LoadingImage.Image = TalkClient.Properties.Resources.X_Image;
            LoadingBar.Value = 0;
            m_InsertToServerQueue = _InsertToQueue;
            m_ProgressType = _ProgressType;
            SetFileName(_FileName);
        }
        
        public void UpdateProgressBar(int _iValue)
        {
            LoadingBar.Value = _iValue;
            //Console.WriteLine("UpdateProgressBar : " + _iValue);
        }

        public void SetSuccess()
        {
            LoadingImage.Image = TalkClient.Properties.Resources.Tick;
            m_iSuccess = true;
        }
        public bool IsSuccess()
        {
            return m_iSuccess;
        }
        public void SetFileName(string _strFileName)
        {
            if(_strFileName.Length > iMaxFileNameLength)
            {
                _strFileName = _strFileName.Substring(0, iMaxFileNameLength - 3);
                _strFileName += "...";
            }
            FileName.Text = _strFileName;
        }

        private void FileName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
