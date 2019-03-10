using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkingClientMain
{
    static class Program
    {
        static void Main()
        {
            TalkClientController m_ClientController = new TalkClientController();
            m_ClientController.Start();
        }
    }
}
