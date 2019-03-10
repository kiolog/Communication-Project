using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
namespace TalkingServerCmd
{

    class MessageModel
    {
        private MySqlDatabase m_Database = new MySqlDatabase();
        private List<int> m_iListUsingID = new List<int>();
        private List<int> m_iListUnUsingID = new List<int>();
        private object m_IOLock = new object();
        
        public MessageModel(string _strHost, string _strDBUser, string _strDBPassword, string _strDBName)
        {
            m_Database.Connect(_strHost, _strDBUser, _strDBPassword, _strDBName);
            m_iListUsingID = GetUsingID();
            m_iListUnUsingID = GetUnUsingID(m_iListUsingID);
        }
        public string GetFileNameByMessageID(int _iMessageID)
        {
            List<List<string>> ListAllHistoryMessage;
            string strQuery = "select strMessage from message where ID = @MessageID";
            List<string> ListParameterName = new List<string>();
            List<object> ListParameterValue = new List<object>();
            ListParameterName.Add("@MessageID");
            ListParameterValue.Add(_iMessageID);
            ListAllHistoryMessage = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);
            return ListAllHistoryMessage[0][0];
        }
        public List<List<string>> GetHistoryMessage(int _iFromID,int _iToID)
        {
            List<List<string>> ListAllHistoryMessage;
            string strQuery = "select ID,IDFROM,IDTO,strMessage,MessageType,UnixTime from message where (IDFROM = @IDFROM AND IDTO = @IDTO) OR (IDFROM = @IDTO AND IDTO = @IDFROM) order by UnixTime asc";
            List<string> ListParameterName = new List<string>();
            List<object> ListParameterValue = new List<object>();
            ListParameterName.Add("@IDFROM");
            ListParameterName.Add("@IDTO");
            ListParameterValue.Add(_iFromID);
            ListParameterValue.Add(_iToID);
            ListAllHistoryMessage = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);
            return ListAllHistoryMessage;
        }

        public int InsertHistoryMessage(int _iFromID, long _iToID,string _strMessage,int _iMessageType,long _lUnixTime)
        {
            Console.WriteLine("Time : " + _lUnixTime);
            int iID;
            lock (m_IOLock)
            {
                Console.WriteLine("HistoryTime : " + DateTime.Now);
                iID = GetEnableID();
                string strQuery = "insert into message(ID,IDFROM, IDTO, UnixTime,strMessage,MessageType) values(@MessageID, @IDFROM,@IDTO,@UnixTime,@StrMessage,@MessageType)";
                List<string> ListParameterName = new List<string>();
                List<object> ListParameterValue = new List<object>();
                ListParameterName.Add("@MessageID");
                ListParameterName.Add("@IDFROM");
                ListParameterName.Add("@IDTO");
                ListParameterName.Add("@UnixTime");
                ListParameterName.Add("@StrMessage");
                ListParameterName.Add("@MessageType");
                ListParameterValue.Add(iID);
                ListParameterValue.Add(_iFromID);
                ListParameterValue.Add(_iToID);
                ListParameterValue.Add(_lUnixTime);
                ListParameterValue.Add(_strMessage);
                ListParameterValue.Add(_iMessageType);
                m_Database.ExecuteQuery(strQuery, ListParameterName, ListParameterValue);
            }

            return iID;
        }
        private List<int> GetUsingID()
        {
            List<List<string>> ListResult = new List<List<string>>();

            ListResult = m_Database.GetQueryData("select ID from message order by ID asc",null,null);

            List<int> ReturnList = new List<int>();
            int iAllListCount = ListResult.Count;
            for (int i = 0; i < iAllListCount; ++i)
            {
                ReturnList.Add(Convert.ToInt32(ListResult[i][0]));
            }
            return ReturnList;
        }
        private List<int> GetUnUsingID(List<int> _ListUsingID)
        {
            List<int> ReturnList = new List<int>();
            if (_ListUsingID.Count > 0)
            {
                int iMaxValue = _ListUsingID[_ListUsingID.Count - 1];
                for (int i = 0; i < iMaxValue; ++i)
                {
                    if (!_ListUsingID.Contains(i))
                    {
                        ReturnList.Add(i);
                    }
                }
            }

            return ReturnList;
        }
        private int GetEnableID()
        {
            int iReturnValue = -1;
            if (m_iListUnUsingID.Count > 0)
            {
                int iID = m_iListUnUsingID[0];
                iReturnValue = iID;
                m_iListUnUsingID.RemoveAt(0);
                m_iListUsingID.Add(iID);
            }
            else
            {
                int iID = m_iListUnUsingID.Count + m_iListUsingID.Count;
                m_iListUsingID.Add(iID);
                iReturnValue = iID;
            }

            return iReturnValue;
        }
    }
}
