using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyConverterLib
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
        public static object CastToVariable(VariableType _Type = VariableType.INT, List<byte> _ListSource = null, int iLength = 0)
        {
            object ReturnValue = null;
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
                    ReturnValue = System.Text.Encoding.UTF8.GetString(_ListSource.GetRange(0, iLength).ToArray());
                    _ListSource.RemoveRange(0, iLength);
                    break;
                case VariableType.LONG:
                    ReturnValue = BitConverter.ToInt64(_ListSource.GetRange(0, 8).ToArray(), 0);
                    _ListSource.RemoveRange(0, 8);
                    break;
                case VariableType.LISTBYTE:
                    ReturnValue = new List<byte>(_ListSource.GetRange(0, iLength));
                    _ListSource.RemoveRange(0, iLength);
                    break;
                case VariableType.BYTEARRAY:
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
    }
}
