using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
namespace TalkingServerCmd
{
    public class GroupModel
    {
        private MySqlDatabase m_Database = new MySqlDatabase();
        private List<int> m_iListUsingID = new List<int>();
        private List<int> m_iListUnUsingID = new List<int>();
        private object m_IOLock = new object();

        public GroupModel(string _strHost, string _strDBUser, string _strDBPassword, string _strDBName)
        {
            m_Database.Connect(_strHost, _strDBUser, _strDBPassword, _strDBName);
            m_iListUsingID = GetUsingID();
            m_iListUnUsingID = GetUnUsingID(m_iListUsingID);
        }
        public List<int> GetMembersByGroupID(int _iGroupID)
        {
            List<List<string>> ListMembers;
            string strQuery = "select Members from GroupMember where GroupID = @GroupID";
            List<string> ListParameterName = new List<string>();
            List<object> ListParameterValue = new List<object>();
            ListParameterName.Add("@GroupID");
            ListParameterValue.Add(_iGroupID);
            ListMembers = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);

            string strMembers = ListMembers[0][0];
            List<string> strSplitString = SplitString(strMembers, ',');

            List<int> ReturnList = new List<int>();

            int iListCount = strSplitString.Count;
            for(int i = 0;i< iListCount; ++i)
            {
                ReturnList.Add(Convert.ToInt32(strSplitString[i]));
            }

            return ReturnList;
        }
        public int AddNewGroup(List<int> _iListMemberID)
        {
            int iID;
            lock (m_IOLock)
            {
                Console.WriteLine("HistoryTime : " + DateTime.Now);
                iID = GetEnableID();
                string strQuery = "insert into GroupMember(GroupID,Members) values(@GroupID, @Members)";
                List<string> ListParameterName = new List<string>();
                List<object> ListParameterValue = new List<object>();
                ListParameterName.Add("@GroupID");
                ListParameterName.Add("@Members");
                ListParameterValue.Add(iID);
                ListParameterValue.Add(GetMembersIDToString(_iListMemberID));
                m_Database.ExecuteQuery(strQuery, ListParameterName, ListParameterValue);
            }
            return iID;
        }
        public void UpdateGroupMembers(GroupEvent _Event,int _GroupID,int _iUpdateMemberID)
        {
            lock (m_IOLock)
            {
                string strMembers = UpdateMember(_Event, GetMembersByGroupID(_GroupID), _iUpdateMemberID);
                string strQuery = "UPDATE GROUPMEMBER SET Members = @Members where GroupID = @GroupID";
                List<string> ListParameterName = new List<string>();
                List<object> ListParameterValue = new List<object>();
                ListParameterName.Add("@Members");
                ListParameterName.Add("@GroupID");
                ListParameterValue.Add(strMembers);
                ListParameterValue.Add(_GroupID);
                m_Database.ExecuteQuery(strQuery, ListParameterName, ListParameterValue);
            }
        }
        private string UpdateMember(GroupEvent _Event,List<int> _iListOriginalMember,int _iUpdateMemberID)
        {
            switch (_Event)
            {
                case GroupEvent.ADDMEMBER:
                    _iListOriginalMember.Add(_iUpdateMemberID);
                    break;
                case GroupEvent.REMOVEMEMBER:
                    _iListOriginalMember.Remove(_iUpdateMemberID);
                    break;
            }
            return GetMembersIDToString(_iListOriginalMember);
        }
        private string GetMembersIDToString(List<int> _iListMemberID)
        {
            string ReturnString = "";
            int iListCount = _iListMemberID.Count;
            for(int i=0;i< iListCount; ++i)
            {
                ReturnString += Convert.ToString(_iListMemberID[i]);
                if(i != iListCount - 1)
                {
                    ReturnString += ",";
                }
            }
            return ReturnString;
        }
        private List<string> SplitString(string _strInput, char _Divider)
        {
            List<string> ReturnList = new List<string>();
            while (_strInput.Length > 0)
            {
                int iDividePoint = _strInput.IndexOf(_Divider);
                if (iDividePoint == -1)
                {
                    ReturnList.Add(_strInput.Substring(0, _strInput.Length));
                    _strInput = "";
                }
                else
                {
                    ReturnList.Add(_strInput.Substring(0, iDividePoint));
                    _strInput = _strInput.Substring(iDividePoint + 1);
                }
            }
            return ReturnList;
        }
        private List<int> GetUsingID()
        {
            List<List<string>> ListResult = new List<List<string>>();

            ListResult = m_Database.GetQueryData("select GroupID from GROUPMEMBER order by GroupID asc", null, null);

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

