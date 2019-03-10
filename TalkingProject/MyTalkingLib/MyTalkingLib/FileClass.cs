using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace MyTalkingLib
{
    public class FileClass
    {
        public enum FileType
        {
            READ,
            WRITE,
            APPEND,
        }
        private FileType m_FileMode;
        private long m_lPreUnixTime = 0;
        private long m_lPreFilePosition = 0;
        private long m_lFilePosition = 0;
        private long m_lFileSize = 0;
        private long m_lSpeed = 0;
        private BinaryReader m_Reader = null;
        private BinaryWriter m_Writer = null;
        private string m_strFileDirectory = "";
        private string m_strFileName = "";
        public FileClass() { }
        public void OpenFile(string _strFileName = "", FileType _FileType = FileType.READ,long _lStartPosition = 0)
        {
            switch (_FileType)
            {
                case FileType.READ:
                    m_Reader = new BinaryReader(File.Open(_strFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read));
                    m_lFileSize = m_Reader.BaseStream.Length;
                    m_Reader.BaseStream.Position = _lStartPosition;
                    break;
                case FileType.WRITE:
                    m_Writer = new BinaryWriter(File.Open(_strFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
                    m_lFileSize = m_Writer.BaseStream.Length;
                    break;
                case FileType.APPEND:
                    m_Writer = new BinaryWriter(File.Open(_strFileName, FileMode.Append, FileAccess.Write, FileShare.Read));
                    m_lFileSize = m_Writer.BaseStream.Length;
                    break;
            }
            int iDividePoint = _strFileName.LastIndexOf('\\');
            m_strFileDirectory = _strFileName.Substring(0, iDividePoint + 1);
            m_strFileName = _strFileName.Substring(iDividePoint + 1);

            m_lFilePosition = _lStartPosition;
            m_FileMode = _FileType;
            m_lPreUnixTime = GetNowUnixTime();
        }
        public byte[] ReadFile(int _iReadLength = 0)
        {
            int iActualReadLength = _iReadLength;
            if (m_lFilePosition + _iReadLength >= m_lFileSize)
            {
                iActualReadLength = (int)(m_lFileSize - m_lFilePosition);
            }
            byte[] ReturnArray = m_Reader.ReadBytes(iActualReadLength);
            m_lPreFilePosition = m_lFilePosition;
            m_lFilePosition = m_Reader.BaseStream.Position;
            UpdateSpeed();
            m_lPreUnixTime = GetNowUnixTime();
            return ReturnArray;
        }
        public void WriteFile(byte[] WriteBytes)
        {
            m_Writer.Write(WriteBytes);
            m_lPreFilePosition = m_lFilePosition;
            m_lFilePosition = m_Writer.BaseStream.Position;
            UpdateSpeed();
            m_lPreUnixTime = GetNowUnixTime();
            //m_lFileSize = m_Writer.BaseStream.Length;
        }
        public void CloseFile()
        {
            if (m_FileMode == FileType.READ)
            {
                m_Reader.Close();
            }else
            {
                m_Writer.Close();
            }
        }
        public bool IsFinished()
        {
            return GetFilePercent() >= 100;
        }
        public long GetFileSize()
        {
            long lReturnValue = m_lFileSize;
            if(m_FileMode == FileType.WRITE)
            {
                lReturnValue = m_Writer.BaseStream.Position;
            }
            return lReturnValue;
        }
        public long GetFilePosition()
        {
            return m_lFilePosition;
        }
        public int GetFilePercent()
        {
            int iReturnValue = 0;
            if(m_lFileSize != 0)
            {
                Console.WriteLine("Divide : " + m_lFileSize);
                iReturnValue = (int)Math.Floor(100 * (m_lFilePosition / (double)m_lFileSize));
            }
            return iReturnValue;
        }
        public string GetFileDirectory()
        {
            return m_strFileDirectory;
        }
        public string GetFileName()
        {
            return m_strFileName;
        }
        public void SetFileSize(long _lFileSize)
        {
            m_lFileSize = _lFileSize;
        }
        public void DeleteFile()
        {
            CloseFile();
            File.Delete(m_strFileDirectory + m_strFileName);
        }
        public long GetSpeed()
        {
            return m_lSpeed;
        }
        private long GetNowUnixTime()
        {
            return (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
        }
        private void UpdateSpeed()
        {
            //Console.WriteLine("ByteDiffer : " + (m_lFilePosition - m_lPreFilePosition));
            //Console.WriteLine("TimeDiffer : " + (GetNowUnixTime() - m_lPreUnixTime));
            long TimerInterval = GetNowUnixTime() - m_lPreUnixTime;
            if(TimerInterval != 0)
            {
                m_lSpeed = (m_lFilePosition - m_lPreFilePosition) / TimerInterval * 1000;
            }
        }
    }
}
