using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MyTalkingLib;
public class EventClass
{
    public WaitCallback m_Function;
    public CaseInfoClass m_Parameter;
    public EventClass(WaitCallback _Function, CaseInfoClass _Parameter)
    {
        m_Function = _Function;
        m_Parameter = _Parameter;
    }
}

