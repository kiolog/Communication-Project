using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkClient
{
    public class UIInfoClass
    {
        public string m_strMessage;
        public bool m_bMine;
        public int m_iID;
        public int m_iValue;
        public int m_iOwnerID;
        public UIInfoClass(string _strMessage = "",bool _bMine = true,int _iID = -1,int _iValue = 0,int _iOwnerID = -1)
        {
            m_strMessage = _strMessage;
            m_bMine = _bMine;
            m_iID = _iID;
            m_iValue = _iValue;
            m_iOwnerID = _iOwnerID;
        }
    }
}
