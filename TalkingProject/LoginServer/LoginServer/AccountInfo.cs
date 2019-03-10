using System;

public class AccountInfo
{
    public string m_strAccount;
    public string m_strPassword;
    public int m_iSenderID;
    public AccountInfo(string _strAccount, string _strPassword, int _iSenderID)
    {
        m_strAccount = _strAccount;
        m_strPassword = _strPassword;
        m_iSenderID = _iSenderID;
    }
}
