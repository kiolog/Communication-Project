using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace MyTalkingLib
{
    public class ConnectModel
    {
        public delegate void DelInsertToQueue(List<byte> _ListByte);
        //private Queue<List<byte>> m_SendBuffer = new Queue<List<byte>>();
        private WaitCallback m_InsertToServerQueue = null;
        private WaitCallback m_RemoveSocket = null;
        private object SendLock = new object();
        public ConnectModel(WaitCallback _CallBack = null,WaitCallback _RemoveSocket = null)
        {
            m_InsertToServerQueue = _CallBack;
            m_RemoveSocket = _RemoveSocket;
        }
        public void SetRemoveSocketCallBack(WaitCallback _RemoveSocket = null)
        {
            m_RemoveSocket = _RemoveSocket;
        }
        public void ReceiveMessage(object _Parameter)
        {
            //Console.WriteLine("StartReceive");
            SocketInfo _SocketInfo = _Parameter as SocketInfo;
            try
            {
                _SocketInfo.m_Socket.BeginReceive(_SocketInfo.m_ReceiveBuffer, 0, _SocketInfo.m_ReceiveBuffer.Length, 0,
                    new AsyncCallback(ReceiveCallback), _SocketInfo);
            }
            catch (SocketException e)
            {
                Console.WriteLine("BeforeReceiveException : " + e.ToString());
            }

        }
        private void ReceiveCallback(IAsyncResult _Result)
        {
            //Console.WriteLine("EndReceive");
            SocketInfo MySocketInfo = (SocketInfo)_Result.AsyncState;
            Socket MySocket = MySocketInfo.m_Socket;
            int iReadLength = 0;
            bool bReadSuccess = true;
            try
            {
                iReadLength = MySocket.EndReceive(_Result);
            }
            catch (SocketException _Exception)
            {
                if (_Exception.ErrorCode == 10054)
                {
                    if(m_RemoveSocket != null)
                    {
                        m_RemoveSocket(MySocketInfo);
                    }
                }
                Console.WriteLine("AfterReceiveException : " + _Exception.ErrorCode);
                bReadSuccess = false;
            }
            if (bReadSuccess)
            {
                List<byte> ListSendbyte = new List<byte>(MySocketInfo.m_ReceiveBuffer);
                ListSendbyte = ListSendbyte.GetRange(0, iReadLength);

                while (ListSendbyte.Count > 0)
                {
                    int iPacketLength = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListSendbyte);
                    //Console.WriteLine("PacketLength : " + iPacketLength);
                    //Console.WriteLine(MySocketInfo.m_iID + " : Receive Count : " + iPacketLength);
                    List<byte> ListInsertByte = new List<byte>();
                    if (ListSendbyte.Count >= iPacketLength)
                    {
                        ListInsertByte = ListSendbyte.GetRange(0, iPacketLength);
                        ListInsertByte.InsertRange(0, BitConverter.GetBytes(MySocketInfo.m_iID));
                        ListSendbyte.RemoveRange(0, iPacketLength);
                    }
                    else
                    {
                        ListInsertByte = ListSendbyte.GetRange(0, ListSendbyte.Count);
                        ListInsertByte.InsertRange(0, BitConverter.GetBytes(MySocketInfo.m_iID));
                        iPacketLength -= ListSendbyte.Count;
                        //Console.WriteLine("RemainPacketLength : " + iPacketLength);
                        while (iPacketLength > 0)
                        {
                            int iReceiveLength = iPacketLength;
                            if (iPacketLength > MySocketInfo.m_ReceiveBuffer.Length)
                            {
                                iReceiveLength = MySocketInfo.m_ReceiveBuffer.Length;
                            }
                            int iReadLengthNow = MySocket.Receive(MySocketInfo.m_ReceiveBuffer, 0, iReceiveLength, 0);
                            ListInsertByte.AddRange((new List<byte>(MySocketInfo.m_ReceiveBuffer)).GetRange(0, iReadLengthNow));
                            iPacketLength -= iReadLengthNow;
                        }
                        ListSendbyte.RemoveRange(0, ListSendbyte.Count);
                    }
                    //Console.WriteLine("ReceiveMessage : " + MyConverter.GetStringFromByteArray(ListInsertByte.ToArray()));
                    m_InsertToServerQueue(ListInsertByte);
                }

                ListSendbyte = new List<byte>();
                ListSendbyte.AddRange(BitConverter.GetBytes(MySocketInfo.m_iID));
                ListSendbyte.Add((byte)EventType.RECEIVE);

                //Console.WriteLine("ReceiveLength : " + iReadLength);

                m_InsertToServerQueue(ListSendbyte);
                //Console.WriteLine("AddRecevieEvent");
            }


        }
        public void ImmediateSend(SocketInfo _SocketInfo, List<byte> _ListMessage)
        {
            _ListMessage.InsertRange(0, BitConverter.GetBytes(_ListMessage.Count));
            _SocketInfo.m_Socket.Send(_ListMessage.ToArray());
        }
        public void SendMessage(object _Parameter)
        {
            //Console.WriteLine("StartSend");
            SocketInfo _SocketInfo = _Parameter as SocketInfo;
            
            List<byte> ListBufferByte = _SocketInfo.Dequeue();
            //Console.WriteLine("SendStart");
            if (ListBufferByte != null)
            {
                try
                {
                    //Console.WriteLine("SendMessage : " + MyConverter.GetStringFromByteArray(ListBufferByte.ToArray()));
                    byte EventByte = ListBufferByte[0];
                    ListBufferByte.InsertRange(0, BitConverter.GetBytes(ListBufferByte.Count));
                    _SocketInfo.m_Socket.BeginSend(ListBufferByte.ToArray(), 0, ListBufferByte.Count, 0,
                        new AsyncCallback(SendCallBack), _SocketInfo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                //Console.WriteLine("SendBuffer Is Empty!!");
                AddSendEventToMain(_SocketInfo.m_iID);
            }
        }
        private void SendCallBack(IAsyncResult _Result)
        {
            SocketInfo MySocketInfo = (SocketInfo)_Result.AsyncState;

            Socket MySocket = MySocketInfo.m_Socket;
            int bytesSent = 0;
            bool bSendSuccess = true;
            try
            {
                bytesSent = MySocket.EndSend(_Result);
            }
            catch (SocketException _Exception)
            {
                if (_Exception.ErrorCode == 10054)
                {
                    if (m_RemoveSocket != null)
                    {
                        m_RemoveSocket(MySocketInfo);
                    }
                }
                Console.WriteLine("AfterReceiveException : " + _Exception.ErrorCode);
                bSendSuccess = false;
            }
            //Console.WriteLine("SendEndBytes : " + bytesSent);

            if (bSendSuccess)
            {
                AddSendEventToMain(MySocketInfo.m_iID);
            }
        }
        private void AddSendEventToMain(int _iID)
        {
            //Console.WriteLine("AddSendEvent");
            List<byte> ListByte = new List<byte>();
            ListByte.AddRange(BitConverter.GetBytes(_iID));
            ListByte.Add((byte)EventType.SEND);

            m_InsertToServerQueue(ListByte);
        }

    }
}
