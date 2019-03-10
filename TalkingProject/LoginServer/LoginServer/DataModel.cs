using System;
using System.Collections.Generic;
using System.Linq;
using MyTalkingLib;
public class DataModel
{
    private MySqlDatabase m_Database = new MySqlDatabase();
    private List<int> m_iListUsingID = new List<int>();
    private List<int> m_iListUnUsingID = new List<int>();
    
    
    public DataModel(string _strHost, string _strDBUser, string _strDBPassword, string _strDBName)
	{
        m_Database.Connect(_strHost, _strDBUser, _strDBPassword, _strDBName);
        Console.WriteLine("ConnectSuccess");
        m_iListUsingID = GetUsingID();
        m_iListUnUsingID = GetUnUsingID(m_iListUsingID);
    }
    public bool HasData(string _strAccount = "",string _strPassword = "",int _iType = 0)
    {
        List<List<string>> ListResult = new List<List<string>>();
        string strQuery = "";
        if(_iType == 0)
        {
            strQuery = "select Count(*) from Account where AccountNumber = @AccountNumber";
            List<string> ListParameterName = new List<string>();
            List<object> ListParameterValue = new List<object>();
            ListParameterName.Add("@AccountNumber");
            ListParameterValue.Add(_strAccount);
            ListResult = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);
        }
        else
        {
            strQuery = "select Count(*) from Account where AccountNumber = @AccountNumber AND Password = @Password";
            List<string> ListParameterName = new List<string>();
            List<object> ListParameterValue = new List<object>();
            ListParameterName.Add("@AccountNumber");
            ListParameterName.Add("@Password");
            ListParameterValue.Add(_strAccount);
            ListParameterValue.Add(_strPassword);
            ListResult = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);
        }
        //Console.WriteLine("HasData Account : " + _strAccount);
        //Console.WriteLine("HasData Password : " + _strPassword);
        return Convert.ToInt32(ListResult[0][0]) == 1;
    }
    public int GetID(string _strAccount = "", string _strPassword = "")
    {
        List<List<string>> ListResult = new List<List<string>>();
        string strQuery = "select ID from Account where AccountNumber = @AccountNumber AND Password = @Password";
        List<string> ListParameterName = new List<string>();
        List<object> ListParameterValue = new List<object>();
        ListParameterName.Add("@AccountNumber");
        ListParameterName.Add("@Password");
        ListParameterValue.Add(_strAccount);
        ListParameterValue.Add(_strPassword);
        ListResult = m_Database.GetQueryData(strQuery, ListParameterName, ListParameterValue);

        return Convert.ToInt32(ListResult[0][0]);
    }
    public int InsertData(string _strAccount = "", string _strPassword = "")
    {
        int iID;
        
        iID = GetEnableID();
        string strQuery = "insert into Account(AccountNumber,Password,ID) values(@AccountNumber,@Password,@ID)";
        List<string> ListParameterName = new List<string>();
        List<object> ListParameterValue = new List<object>();
        ListParameterName.Add("@AccountNumber");
        ListParameterName.Add("@Password");
        ListParameterName.Add("@ID");
        ListParameterValue.Add(_strAccount);
        ListParameterValue.Add(_strPassword);
        ListParameterValue.Add(iID);
        m_Database.ExecuteQuery(strQuery, ListParameterName, ListParameterValue);
        return iID;
    }
    private List<int> GetUsingID()
    {
        List<List<string>> ListResult = new List<List<string>>();

        ListResult = m_Database.GetQueryData("select ID from Account order by ID asc",null,null);

        List<int> ReturnList = new List<int>();
        int iAllListCount = ListResult.Count;
        for(int i=0;i< iAllListCount; ++i)
        {
            ReturnList.Add(Convert.ToInt32(ListResult[i][0]));
        }
        return ReturnList;
    }
    private List<int> GetUnUsingID(List<int> _ListUsingID)
    {
        List<int> ReturnList = new List<int>();
        if(_ListUsingID.Count > 0)
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
        }else
        {
            int iID = m_iListUnUsingID.Count + m_iListUsingID.Count;
            m_iListUsingID.Add(iID);
            iReturnValue = iID;
        }

        return iReturnValue;
    }
}
