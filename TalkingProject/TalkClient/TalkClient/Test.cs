using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
public class Test
{
	
    static void Main()
    {
        for(int i = 0; i < 100; ++i)
        {
            Socket ConnectSocket = Connect("127.0.0.1", 9001);
            Thread.Sleep(10000);
        }
    }
    static public Socket Connect(string _strIP, int _iPort)
    {
        IPEndPoint ServerIP = new IPEndPoint(IPAddress.Parse(_strIP), _iPort);
        Socket ConnectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ConnectSocket.Connect(ServerIP);

        return ConnectSocket;
    }
}
