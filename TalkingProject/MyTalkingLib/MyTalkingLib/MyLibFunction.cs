using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTalkingLib
{
    public static class MyLibFunction
    {
        public static long GetNowUnixTime()
        {
            return (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
        }
        public static List<byte> RebuildCommand(EventType _EventType, CaseInfoClass _Parameter)
        {
            int iSenderID = _Parameter.m_iSenderID;
            List<byte> ListByte = new List<byte>(_Parameter.m_ListByte);
            List<byte> ReturnList = new List<byte>();
            ReturnList.AddRange(BitConverter.GetBytes(iSenderID));
            ReturnList.Add((byte)_EventType);
            ReturnList.AddRange(ListByte);

            return ReturnList;
        }
    }
    
}
