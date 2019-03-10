using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TalkingClientCmd
{
    class Program
    {
        private static List<ClientMain> ListClientMain = new List<ClientMain>();
        static void Main()
        {
            //Console.WriteLine(Environment.CurrentDirectory);
            //Console.ReadKey();
            int iSenderFileNumber = 200;
            for(int i=0;i< iSenderFileNumber; ++i)
            {
                //Thread.Sleep(200);
                ClientMain NewClient = new ClientMain("127.0.0.1", 9002, i);
                ListClientMain.Add(NewClient);
                NewClient.Start();
                //Thread.Sleep(100);
                //UploadFile(i, str1000MBFileName, 1);
            }
            /*for (int i = 0; i < iSenderFileNumber; ++i)
            {
                UploadFile(i, str1000MBFileName, 1);
            }*/
        }
        static public void UploadFile(int _iClientID,string _strFileName, int _iReceiverID)
        {
            /*List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkingClientCmd.ClientMain.ServerType.TALKINGSERVER));
            ListSendByte.Add((byte)UPLOADFILE);
            ListSendByte.AddRange(BitConverter.GetBytes(_iReceiverID));
            byte[] strByteArray = System.Text.Encoding.UTF8.GetBytes(_strFileName);
            ListSendByte.AddRange(BitConverter.GetBytes(strByteArray.Length));
            ListSendByte.AddRange(strByteArray);
            ListClientMain[_iClientID].InsertToQueue(ListSendByte);*/
        }
    }
}
