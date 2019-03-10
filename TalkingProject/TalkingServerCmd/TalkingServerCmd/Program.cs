using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using MyTalkingLib;
namespace TalkingServerCmd
{
    static class Program
    {
        static void Main()
        {
            TalkingServerController m_TalkingServerController = new TalkingServerController();
            m_TalkingServerController.Start();
        }
    }
}
       
