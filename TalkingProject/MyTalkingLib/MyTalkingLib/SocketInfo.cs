using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace MyTalkingLib
{
    public class SocketInfo
    {
        public int m_iID;
        public byte[] m_ReceiveBuffer = new byte[100000];
        public Socket m_Socket;

        private MyPriorityQueue m_PriorityQueue = new MyPriorityQueue();

        public SocketInfo(int _iID, Socket _Socket)
        {
            m_Socket = _Socket;
            m_iID = _iID;
            m_PriorityQueue.SetPriorityRule((byte)EventType.MESSAGE, 0);
            m_PriorityQueue.SetPriorityRule((byte)EventType.FILEMESSAGE, 0);
        }
        public void InsertToQueue(List<byte> _ListByte)
        {
            byte MyEventType = _ListByte[0];
            //Console.WriteLine("InsertEvent : " + (EventType)MyEventType);
            m_PriorityQueue.Enqueue(MyEventType, _ListByte);
            //Console.WriteLine("Command Count : " + m_PriorityQueue.Count);
        }
        public List<byte> Dequeue()
        {
            return m_PriorityQueue.Dequeue();
        }

    }
}
