using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using MyTalkingLib;
namespace TestLoginServer
{
    class Program
    {
        
        private static int iClientNumber = 200;
        private static int iCommandInterval = 1000;
        private static int iCommandNumber = 1;
        private static int iAccountLength = 10;
        private static int iPasswordLength = 10;
        private static int iRegisterNumber = 0;
        private static EncryptModel m_EncryptModel;
        private static string strLoginServerKey = "CzgT7TA15C7JABGN+cdMtdaWeEwj8eUz";
        private static string strLoginServerIV = "9M53h3zlZ/E=";
        private static object iolock = new object();
        static void Main()
        {
            m_EncryptModel = new EncryptModel(strLoginServerKey, strLoginServerIV);
            for (int i=0;i< iClientNumber; ++i)
            {
                //Thread.Sleep(1000);
                Thread NewThread = new Thread(ProcessLoop);
                NewThread.Start();
                Thread.Sleep(10);
            }
        }
        static public Socket Connect(string _strIP, int _iPort)
        {
            IPEndPoint ServerIP = new IPEndPoint(IPAddress.Parse(_strIP), _iPort);
            Socket ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectSocket.Connect(ServerIP);

            return ConnectSocket;
        }
        static void ProcessLoop()
        {
            Socket ConnectSocket = Connect("127.0.0.1", 9001);


            //Thread.Sleep(10);

            Random rnd = new Random();
            int iSendNumber = rnd.Next(1, iCommandNumber);
            while (true)
            {
                
                for (int i = 0; i < iCommandNumber; ++i)
                {
                    List<byte> ListSendByte = new List<byte>();

                    string strAccount = "";
                    string strPassword = "";

                    int iRandomAccountLength = rnd.Next(1, iAccountLength);
                    int iRandomPasswordLength = rnd.Next(1, iPasswordLength);

                    for (int j = 0; j < iRandomAccountLength; ++j)
                    {
                        char iRandomChar = (char)(rnd.Next(33, 126));
                        strAccount += iRandomChar;
                    }
                    for (int j = 0; j < iRandomPasswordLength; ++j)
                    {
                        char iRandomChar = (char)(rnd.Next(33, 126));
                        strPassword += iRandomChar;
                    }
                    int iRandomValue = rnd.Next(1, 3);
                    /*if(iRandomValue == 1)
                    {
                        ListSendByte.Add((byte)EventType.REGISTER);
                    }else
                    {
                        ListSendByte.Add((byte)EventType.SIGNIN);
                    }*/
                    ListSendByte.Add((byte)EventType.REGISTER);
                    //ListSendByte.Add((byte)EventType.REGISTER);
                    byte[] ByteArrayAccount = m_EncryptModel.Encrypt(strAccount);
                    byte[] ByteArrayPassword = m_EncryptModel.Encrypt(strPassword);
                    ListSendByte.AddRange(BitConverter.GetBytes(ByteArrayAccount.Length));
                    ListSendByte.AddRange(ByteArrayAccount);
                    ListSendByte.AddRange(BitConverter.GetBytes(ByteArrayPassword.Length));
                    ListSendByte.AddRange(ByteArrayPassword);

                    ListSendByte.InsertRange(0, BitConverter.GetBytes(ListSendByte.Count));

                    int iSendBytes = ConnectSocket.Send(ListSendByte.ToArray());
                    lock (iolock)
                    {
                        ++iRegisterNumber;
                    }
                    Console.WriteLine("RegisNumber : " + iRegisterNumber);
                    //Console.WriteLine("SendBytes : " + iSendBytes);
                }
                Thread.Sleep(iCommandInterval);

            }
            

        }
    }
}
