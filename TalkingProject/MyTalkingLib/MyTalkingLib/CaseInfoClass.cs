using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTalkingLib
{
    public class CaseInfoClass
    {
        public int m_iSenderID;
        public List<byte> m_ListByte;
        public CaseInfoClass(int _iSenderID = -1, List<byte> _ListByte = null)
        {
            m_iSenderID = _iSenderID;
            m_ListByte = new List<byte>(_ListByte);
        }
    }
}
