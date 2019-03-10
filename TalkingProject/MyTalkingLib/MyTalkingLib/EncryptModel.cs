using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace MyTalkingLib
{
    public class EncryptModel
    {
        private TripleDESCryptoServiceProvider m_3DESEncrytor = new TripleDESCryptoServiceProvider();
        public EncryptModel(string _strKey,string _strIV)
        {
            SetKey(_strKey);
            SetIV(_strIV);
        }
        public EncryptModel() {
        }
        public void SetKey(byte[] _Key)
        {
            m_3DESEncrytor.Key = _Key;
        }
        public void SetIV(byte[] _IV)
        {
            m_3DESEncrytor.IV = _IV;
        }
        public void SetKey(string _Key)
        {
            m_3DESEncrytor.Key = Convert.FromBase64String(_Key);
        }
        public void SetIV(string _IV)
        {
            m_3DESEncrytor.IV = Convert.FromBase64String(_IV);
        }
        public byte[] Decrypt(byte[] _Input)
        {
            ICryptoTransform DESDescrptor = m_3DESEncrytor.CreateDecryptor();
            return DESDescrptor.TransformFinalBlock(_Input, 0, _Input.Length);
        }
        public byte[] Decrypt(string _strInput)
        {

            return Decrypt(System.Text.Encoding.UTF8.GetBytes(_strInput));
        }
        public byte[] Encrypt(byte[] _Input)
        {
            ICryptoTransform DESEncryptor = m_3DESEncrytor.CreateEncryptor();
            return DESEncryptor.TransformFinalBlock(_Input, 0, _Input.Length);
        }
        public byte[] Encrypt(string _strInput)
        {
            ICryptoTransform DESEncryptor = m_3DESEncrytor.CreateEncryptor();
            return Encrypt(System.Text.Encoding.UTF8.GetBytes(_strInput));
        }
    }
}
