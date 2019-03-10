using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace MyTalkingLib
{
    public class FileIOModel
    {
        private ObjectPool<FileClass> m_FilePool = new ObjectPool<FileClass>();
        private Dictionary<int, FileClass> m_DicFilePair = new Dictionary<int, FileClass>();
        private object m_oIOLock = new object();
        public int OpenFile(string _strFileName = "",FileClass.FileType _FileType = FileClass.FileType.READ,long _lStartPosition = 0)
        {
            FileClass NewFile = m_FilePool.GetObject();
            int iFileID = m_FilePool.GetIDByObject(NewFile);
            NewFile.OpenFile(_strFileName, _FileType, _lStartPosition);
            lock (m_DicFilePair)
            {
                m_DicFilePair.Add(iFileID, NewFile);
            }
            Console.WriteLine("OpenFileID : " + iFileID);
            return iFileID;
        }
        public void CloseFile(int _iFileID)
        {
            Console.WriteLine("CloseFileID : " + _iFileID);
            m_DicFilePair[_iFileID].CloseFile();
            
        }
        public byte[] ReadFile(int _iFileID,int _iReadLength = 0)
        {
            byte[] ReturnResult = null;
            lock (m_oIOLock)
            {
                ReturnResult = m_DicFilePair[_iFileID].ReadFile(_iReadLength);
            }
            return ReturnResult;
        }
        public void WriteFile(int _iFileID,byte[] WriteBytes)
        {
            lock (m_oIOLock)
            {
                m_DicFilePair[_iFileID].WriteFile(WriteBytes);
            }

        }
        public void WriteFile(int _iFileID, List<byte> WriteBytes)
        {
            WriteFile(_iFileID, WriteBytes.ToArray());
        }
        public long GetFileSize(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetFileSize();
        }
        public bool IsFileFinished(int _iFileID)
        {
            return m_DicFilePair[_iFileID].IsFinished();
        }
        public int GetFilePercent(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetFilePercent();
        }
        public string GetFileDirectory(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetFileDirectory();
        }
        public string GetFileName(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetFileName();
        }
        public void SetFileSize(int _iFileID,long _lFileSize)
        {
            m_DicFilePair[_iFileID].SetFileSize(_lFileSize);
        }
        public bool HasFile(int _iFileID)
        {
            return m_DicFilePair.ContainsKey(_iFileID);
        }
        public void DeleteFile(int _iFileID)
        {
            m_DicFilePair[_iFileID].DeleteFile();
        }
        public long GetFileSpeed(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetSpeed();
        }
        public long GetFilePosition(int _iFileID)
        {
            return m_DicFilePair[_iFileID].GetFilePosition();
        }
        public void RemoveFile(int _iFileID)
        {
            Console.WriteLine("RemoveFileID : " + _iFileID);
            CloseFile(_iFileID);
            lock (m_DicFilePair)
            {
                m_DicFilePair.Remove(_iFileID);
                m_FilePool.RecycleObject(_iFileID);
            }
        }
    }
    
}
