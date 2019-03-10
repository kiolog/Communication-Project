using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using MyTalkingLib;
using System.Threading;
public class EventInterface
{
    protected Dictionary<int, SocketInfo> m_DicSocketInfo = new Dictionary<int, SocketInfo>();
    protected ConnectModel m_ConnectModel = null;
    protected WaitCallback m_InsertToQueue;
    private List<int> m_iListUnUsedID = new List<int>();
    private List<int> m_iListUsedID = new List<int>();
    public EventInterface(WaitCallback _CallBack = null,WaitCallback _RemoveSocket = null)
    {
        m_InsertToQueue = _CallBack;
        m_ConnectModel = new ConnectModel(m_InsertToQueue, _RemoveSocket);
    }
    public virtual void AddNewClient(Socket _Socket)
    {
        int iClientID;
        lock (m_DicSocketInfo)
        {
            iClientID = GetEnableSocketID();
            Console.WriteLine("NewClientID : " + iClientID);
            m_DicSocketInfo.Add(iClientID, new SocketInfo(iClientID, _Socket));
        }
        List<byte> ListByte = new List<byte>();
        ListByte.AddRange(BitConverter.GetBytes(iClientID));
        ListByte.Add((byte)EventType.RECEIVE);
        m_InsertToQueue(ListByte);


        ListByte = new List<byte>();
        ListByte.AddRange(BitConverter.GetBytes(iClientID));
        ListByte.Add((byte)EventType.SEND);
        m_InsertToQueue(ListByte);
    }
    public virtual void RemoveClient(object _Parameter)
    {
        SocketInfo MySocketInfo = _Parameter as SocketInfo;
        int iSocketID = MySocketInfo.m_iID;
        lock (m_DicSocketInfo)
        {
            Console.WriteLine(iSocketID + " : Is Leaved");
            if (m_DicSocketInfo.ContainsKey(iSocketID))
            {
                m_DicSocketInfo.Remove(iSocketID);
                RecycleSocketID(iSocketID);
            }
            
        }
    }
    public void CaseReceive(object _Parameter)
    {
        //Console.WriteLine("CaseReceive");
        CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
        int iSenderID = MyInfo.m_iSenderID;
        List<byte> ListByte = MyInfo.m_ListByte;
        int iSocketID = iSenderID;
        if (m_DicSocketInfo.ContainsKey(iSocketID))
        {
            m_ConnectModel.ReceiveMessage(m_DicSocketInfo[iSocketID]);
        }
    }
    public void CaseSend(object _Parameter)
    {
        //Console.WriteLine("CaseSend");
        CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
        int iSenderID = MyInfo.m_iSenderID;
        List<byte> ListByte = MyInfo.m_ListByte;
        int iSocketID = iSenderID;
        if (m_DicSocketInfo.ContainsKey(iSocketID))
        {
            m_ConnectModel.SendMessage(m_DicSocketInfo[iSocketID]);
        }
    }
    public virtual void CaseInsertToSocketQueue(object _Parameter) {
        //Console.WriteLine("Insert");
        lock (m_DicSocketInfo)
        {
            CaseInfoClass MyInfo = _Parameter as CaseInfoClass;
            int iSenderID = MyInfo.m_iSenderID;
            //Console.WriteLine(iSenderID + "Insert");
            List<byte> ListByte = MyInfo.m_ListByte;
            //Console.WriteLine("InsertSocketMessage : " + MyConverter.GetStringFromByteArray(ListByte.ToArray()));
            if (m_DicSocketInfo.ContainsKey(iSenderID))
            {
                m_DicSocketInfo[iSenderID].InsertToQueue(ListByte);
            }
        }
    }
    protected Socket Connect(string _strIP, int _iPort)
    {
        IPEndPoint ServerIP = new IPEndPoint(IPAddress.Parse(_strIP), _iPort);
        Socket ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ConnectSocket.Connect(ServerIP);

        return ConnectSocket;
    }
    
    private int GetEnableSocketID()
    {
        int iReturnValue = 0;
        lock (m_iListUnUsedID)
        {
            if (m_iListUnUsedID.Count > 0)
            {
                iReturnValue = m_iListUnUsedID[0];
                m_iListUnUsedID.RemoveAt(0);
                m_iListUsedID.Add(iReturnValue);
            }
            else
            {
                iReturnValue = m_iListUnUsedID.Count + m_iListUsedID.Count;
                m_iListUsedID.Add(iReturnValue);
            }
        }
        return iReturnValue;
    }
    private void RecycleSocketID(int _iSocketID)
    {
        lock (m_iListUnUsedID)
        {
            m_iListUsedID.Remove(_iSocketID);
            m_iListUnUsedID.Add(_iSocketID);
        }
    }

}

