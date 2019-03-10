using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyTalkingLib;

public class CheckEventModel
{

    private MyPriorityQueue m_PriorityQueue = new MyPriorityQueue();
    private Dictionary<byte, WaitCallback> m_DicDoEvent = new Dictionary<byte, WaitCallback>();
    private WaitCallback m_ServerAddEvent;
    private int m_iDeltaTime;

    public CheckEventModel(int _iDeltaTime,WaitCallback _CallBack)
    {
        m_PriorityQueue.SetPriorityRule((byte)EventType.ADDLOGINUSER, 0);
        m_PriorityQueue.SetPriorityRule((byte)EventType.MESSAGE, 1);
        m_PriorityQueue.SetPriorityRule((byte)EventType.FILEMESSAGE, 1);
        m_ServerAddEvent = _CallBack;
        m_iDeltaTime = _iDeltaTime;
    }
    public void InsertToQueue(object _Parameter)
    {
        List<byte> _ListByte = _Parameter as List<byte>;
        byte byteEventType = _ListByte[4];
        if (m_DicDoEvent.ContainsKey(byteEventType))
        {
            //Console.WriteLine("Contains : " + (EventType)byteEventType);
            if (byteEventType == (byte)EventType.INSERTTOSOCKETQUEUE)
            {
                //Console.WriteLine("Message : " + MyConverter.GetStringFromByteArray(_ListByte.ToArray()));
            }
            m_PriorityQueue.Enqueue(byteEventType, _ListByte);
        }
    }
    public void AddCheckEvent(byte _EventType, WaitCallback _DoFunction)
    {
        m_DicDoEvent.Add(_EventType, _DoFunction);
    }
    public void Update(object _Parameter = null)
    {
        if (m_PriorityQueue.Count > 0)
        {
            List<byte> ListMessage = m_PriorityQueue.Dequeue();
            int iSenderID = (int)MyConverter.CastToVariable(MyConverter.VariableType.INT, ListMessage);
            byte EventNumber = (byte)MyConverter.CastToVariable(MyConverter.VariableType.BYTE, ListMessage);

            if (m_DicDoEvent.ContainsKey(EventNumber))
            {
                m_ServerAddEvent(new EventClass(m_DicDoEvent[EventNumber], new CaseInfoClass(iSenderID, ListMessage)));
            }
        }
    }
}

