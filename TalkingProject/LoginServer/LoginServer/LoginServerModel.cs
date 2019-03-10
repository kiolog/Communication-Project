using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTalkingLib;
using System.Threading;
using System.Net.Sockets;
namespace LoginServer
{
    public class LoginServerModel:EventInterface
    {
        private int m_iTalkingServerSocketID = 0;
        
        private LoginModel m_LoginModel;
        private int m_iClientNumber = 0;

        private string m_strTalkServerIP = "127.0.0.1";
        private int m_iTalkServerPort = 9002;
        private object m_oDataBaseLock = new object();
        private EncryptModel m_LoginEncryptModel;
        private EncryptModel m_TalkingEncryptModel;
        private string strLoginServerKey = "CzgT7TA15C7JABGN+cdMtdaWeEwj8eUz";
        private string strLoginServerIV = "9M53h3zlZ/E=";
        private string strTalkingServerKey = "+y3txUT3ynClLkDTi2551Da3W2JYrqaf";
        private string strTalkingServerIV = "kSXHSqR5Pqc=";

        private int m_iTalkingServerID = 0;

        public LoginServerModel(WaitCallback _CallBack) : base(_CallBack){
            m_LoginEncryptModel = new EncryptModel(strLoginServerKey, strLoginServerIV);
            m_TalkingEncryptModel = new EncryptModel(strTalkingServerKey, strTalkingServerIV);
            m_LoginModel = new LoginModel("localhost", "user2", "user2", "Account");

            m_ConnectModel.SetRemoveSocketCallBack(RemoveClient);
        }
        public void Start()
        {
            Socket TalkingServerSocket = Connect(m_strTalkServerIP, m_iTalkServerPort);
            AddNewClient(TalkingServerSocket);
        }
        
        public void CaseSignIn(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            byte[] ByteArrayAccount = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            byte[] ByteArrayPassword = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);

            string strAccount = MyConverter.GetStringFromByteArray(m_LoginEncryptModel.Decrypt(ByteArrayAccount));
            string strPassword = MyConverter.GetStringFromByteArray(m_LoginEncryptModel.Decrypt(ByteArrayPassword));

            int iID;
            lock (m_oDataBaseLock)
            {
                iID = m_LoginModel.Login(strAccount, strPassword);
            }

            if (iID == -1)
            {
                SendMessageToClient("SigIn Failed", iSenderID);
            }
            else
            {
                SendMessageToClient("SigIn Success", iSenderID);

                List<byte> EncodingBytes = new List<byte>();
                EncodingBytes.AddRange(BitConverter.GetBytes(iID));
                EncodingBytes.AddRange(BitConverter.GetBytes(GetNowUnixTime()));
                byte[] EncryptResult = m_TalkingEncryptModel.Encrypt(EncodingBytes.ToArray());

                List<byte> SendMessage = new List<byte>();
                SendMessage.AddRange(BitConverter.GetBytes(m_iTalkingServerSocketID));
                SendMessage.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                SendMessage.Add((byte)EventType.ADDLOGINUSER);
                SendMessage.AddRange(BitConverter.GetBytes(EncryptResult.Length));
                SendMessage.AddRange(EncryptResult);

                m_InsertToQueue(SendMessage);

                SendMessage = new List<byte>();
                SendMessage.AddRange(BitConverter.GetBytes(iSenderID));
                SendMessage.Add((byte)EventType.INSERTTOSOCKETQUEUE);
                SendMessage.Add((byte)EventType.SIGNINSUCCESS);
                SendMessage.AddRange(BitConverter.GetBytes(EncryptResult.Length));
                SendMessage.AddRange(EncryptResult);
                byte[] strArrIP = System.Text.Encoding.UTF8.GetBytes(m_strTalkServerIP);
                SendMessage.AddRange(BitConverter.GetBytes(strArrIP.Length));
                SendMessage.AddRange(strArrIP);
                SendMessage.AddRange(BitConverter.GetBytes(m_iTalkServerPort));
                //Console.WriteLine("SignInSuccess");
                Console.WriteLine("TalkingserverID : " + m_iTalkingServerSocketID);
                Console.WriteLine("OtherID : " + iSenderID);
                m_InsertToQueue(SendMessage);
            }




        }
        public void CaseRegister(object _Parameter)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            List<byte> ListByte = MyInfo.m_ListByte;

            byte[] ByteArrayAccount = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);
            byte[] ByteArrayPassword = (byte[])MyConverter.CastToVariable(MyConverter.VariableType.BYTEARRAY, ListByte);

            string strAccount = MyConverter.GetStringFromByteArray(m_LoginEncryptModel.Decrypt(ByteArrayAccount));
            string strPassword = MyConverter.GetStringFromByteArray(m_LoginEncryptModel.Decrypt(ByteArrayPassword));

            int iID;
            lock (m_oDataBaseLock)
            {
                iID = m_LoginModel.Register(strAccount, strPassword);
            }

            if (iID == -1)
            {
                SendMessageToClient("Register Failed", iSenderID);
            }
            else
            {
                SendMessageToClient("Register Success", iSenderID);
            }

        }
        private int GetNowUnixTime()
        {
            return (Int32)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }
        private void SendMessageToClient(string _strMessage, int _iSenderID)
        {
            List<byte> SendMessage = new List<byte>();
            SendMessage.AddRange(BitConverter.GetBytes(_iSenderID));
            SendMessage.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            SendMessage.Add((byte)EventType.LOGINMESSAGE);
            byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(_strMessage);
            SendMessage.AddRange(BitConverter.GetBytes(strByteArray.Length));
            SendMessage.AddRange(strByteArray);

            m_InsertToQueue(SendMessage);
        }
    }
}
