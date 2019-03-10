using System;
using System.Collections.Generic;
using System.Linq;
public class LoginModel
{
    DataModel m_DataModel;
    List<int> m_iListOnlineUser = new List<int>();
    public LoginModel(string _strHost, string _strDBUser, string _strDBPassword, string _strDBName)
	{
        m_DataModel = new DataModel(_strHost, _strDBUser, _strDBPassword, _strDBName);
    }
    public int Login(string _strAccount,string _strPassword)
    {
        int iReturnValue = -1;
        if(m_DataModel.HasData(_strAccount, _strPassword,1))
        {
            iReturnValue = m_DataModel.GetID(_strAccount, _strPassword);
            m_iListOnlineUser.Add(iReturnValue);
        }
        return iReturnValue;
    }
    public int Register(string _strAccount, string _strPassword)
    {
        int iReturnValue = -1;
        if (!m_DataModel.HasData(_strAccount:_strAccount,_iType:0))
        {
            iReturnValue = m_DataModel.InsertData(_strAccount, _strPassword);
        }

        return iReturnValue;
    }
    
}
