using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTalkingLib
{
    public static class MyConverter
    {
        public enum VariableType
        {
            INT,
            BYTE,
            STRING,
            LONG,
            LISTBYTE,
            BYTEARRAY,
        }
        public static object CastToVariable(VariableType _Type = VariableType.INT, List<byte> _ListSource = null)
        {
            object ReturnValue = null;
            int iLength;
            switch (_Type)
            {
                case VariableType.INT:
                    ReturnValue = BitConverter.ToInt32(_ListSource.GetRange(0, 4).ToArray(), 0);
                    _ListSource.RemoveRange(0, 4);
                    break;
                case VariableType.BYTE:
                    ReturnValue = _ListSource[0];
                    _ListSource.RemoveAt(0);
                    break;
                case VariableType.STRING:
                    iLength = (int)MyConverter.CastToVariable(VariableType.INT, _ListSource);
                    ReturnValue = System.Text.Encoding.UTF8.GetString(_ListSource.GetRange(0, iLength).ToArray());
                    _ListSource.RemoveRange(0, iLength);
                    break;
                case VariableType.LONG:
                    ReturnValue = BitConverter.ToInt64(_ListSource.GetRange(0, 8).ToArray(), 0);
                    _ListSource.RemoveRange(0, 8);
                    break;
                case VariableType.LISTBYTE:
                    iLength = (int)MyConverter.CastToVariable(VariableType.INT, _ListSource);
                    ReturnValue = new List<byte>(_ListSource.GetRange(0, iLength));
                    _ListSource.RemoveRange(0, iLength);
                    break;
                case VariableType.BYTEARRAY:
                    iLength = (int)MyConverter.CastToVariable(VariableType.INT, _ListSource);
                    ReturnValue = (new List<byte>(_ListSource.GetRange(0, iLength))).ToArray();
                    _ListSource.RemoveRange(0, iLength);
                    break;
            }
            return ReturnValue;
        }
        public static string GetStringFromByteArray(byte[] _Input)
        {
            return System.Text.Encoding.UTF8.GetString(_Input);
        }
        public static byte[] GetByteArrayFromString(string _strInput)
        {
            return System.Text.Encoding.UTF8.GetBytes(_strInput);
        }
    }
}
