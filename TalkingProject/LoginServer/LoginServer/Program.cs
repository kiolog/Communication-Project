using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
namespace LoginServer
{
    static class Program
    {
        static void Main()
        {
            LoginServerController m_LoginServerController = new LoginServerController();
            m_LoginServerController.Start();
        }
    }
}
       
