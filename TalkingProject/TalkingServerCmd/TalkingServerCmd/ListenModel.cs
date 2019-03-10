using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
public class ListenModel
{

    public delegate void DelAddNewClient(Socket _Socket);
    private const int LISTENNUMBER = 10;

    //private Thread m_Thread;
    private Thread m_Thread;
    private Socket m_ListenSocket;
    private static DelAddNewClient m_AddNewClient;
    private static ManualResetEvent m_ResetEvent = new ManualResetEvent(false);
    public ListenModel(int _iPort, DelAddNewClient _AddNewClient)
	{
        IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, _iPort);
        m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_ListenSocket.Bind(EndPoint);
        m_ListenSocket.Listen(LISTENNUMBER);
        m_AddNewClient = _AddNewClient;
        //m_Thread = new Thread(StartListen);
        //m_Thread.Start();
        //StartListen();
    }
    public void StartListen(object _Parameter)
    {
        /*try
        {
            Socket NewClient = m_ListenSocket.Accept();
            Console.WriteLine("AddNewClient");
            m_AddNewClient(NewClient);
        }
        catch(SocketException _Exception)
        {
            //Console.WriteLine("ErrorCode : " + _Exception.ErrorCode);
        }*/

        
        
        Console.WriteLine("StartListen");
        m_ListenSocket.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    m_ListenSocket);

    }
    public static void AcceptCallback(IAsyncResult _AsyncResult)
    {
        // Signal the main thread to continue.
        Socket ListenSocket = (Socket)_AsyncResult.AsyncState;
        Socket NewSocket = ListenSocket.EndAccept(_AsyncResult);
        m_AddNewClient(NewSocket);
        Console.WriteLine("NewClient");
        ListenSocket.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    ListenSocket);
        
    }
}
