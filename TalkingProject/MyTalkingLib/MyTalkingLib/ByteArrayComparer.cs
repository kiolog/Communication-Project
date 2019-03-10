using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTalkingLib
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] _Source, byte[] _Target)
        {
            bool bReturnValue = true;
            if(_Source == null || _Target == null || _Source.Length != _Target.Length)
            {
                bReturnValue = false;
            }
            else
            {
                int iLength = _Source.Length;
                for (int i = 0; i < iLength; ++i)
                {
                    if (_Source[i] != _Target[i])
                    {
                        bReturnValue = false;
                        break;
                    }
                }
            }
            return bReturnValue;
        }
        public int GetHashCode(byte[] _Source)
        {
            int iMaxLength = 4;
            if (_Source == null)
                throw new ArgumentNullException("key");
            List<byte> ListHash = new List<byte>(_Source);
            int iListCount = ListHash.Count;
            if (iListCount < iMaxLength)
            {
                int iDifferLength = iMaxLength - ListHash.Count;
                List<byte> ListAppendByte = new List<byte>();
                for(int i=0;i< iDifferLength; ++i)
                {
                    ListAppendByte.Add(0x0000);
                }
                ListHash.InsertRange(0, ListAppendByte);
            }else
            {
                ListHash = ListHash.GetRange(0, iMaxLength);
            }
            return BitConverter.ToInt32(ListHash.ToArray(),0);
        }
    }
}
