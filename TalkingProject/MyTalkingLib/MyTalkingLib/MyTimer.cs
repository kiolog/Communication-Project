using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace MyTalkingLib
{
    public class MyTimer
    {
        public enum ETimerMode
        {
            NORMAL,
            REPEAT,
        }
        private class TimerEvent
        {
            private ETimerMode m_Mode = ETimerMode.NORMAL;
            private int m_iOriginalTargetTime = 0;
            private int m_iTargetTime;
            private WaitCallback m_DoFunction;
            private object m_Parameter;
            public TimerEvent() { }
            public bool Tick(int _iDeltaTime)
            {
                m_iTargetTime -= _iDeltaTime;
                return m_iTargetTime <= 0;
            }
            public void DoFunction()
            {
                m_DoFunction(m_Parameter);
            }
            public void SetParameter(int _iTargetTime, WaitCallback _CallBack, object _Parameter, ETimerMode _Mode)
            {
                m_iOriginalTargetTime = _iTargetTime;
                m_iTargetTime = _iTargetTime;
                m_DoFunction = _CallBack;
                m_Parameter = _Parameter;
                m_Mode = _Mode;
            }
            public ETimerMode GetTimerMode()
            {
                return m_Mode;
            }
            public void ReStart()
            {
                m_iTargetTime = m_iOriginalTargetTime;
            }
        }
        private ObjectPool<TimerEvent> m_TimerEventPool = new ObjectPool<TimerEvent>();
        private List<TimerEvent> m_ListTimerEvent = new List<TimerEvent>();
        private List<int> m_iListTimerEventID = new List<int>();
        private Thread m_Thread;
        private int m_iDeltaTime;
        private bool m_bPause = false;
        public MyTimer(int _iDeltaTime)
        {
            m_Thread = new Thread(Loop);
            m_iDeltaTime = _iDeltaTime;
        }

        public void Start()
        {
            m_Thread.Start();
        }
        public int AddTimeEvent(int _iTargetTime = 0, WaitCallback _CallBack = null, object _Parameter = null,ETimerMode _Mode = ETimerMode.NORMAL)
        {
            int iReturnID = 0;
            lock (m_ListTimerEvent)
            {
                TimerEvent NewTimerEvent = m_TimerEventPool.GetObject();
                iReturnID = m_TimerEventPool.GetIDByObject(NewTimerEvent);
                NewTimerEvent.SetParameter(_iTargetTime, _CallBack, _Parameter, _Mode);
                m_ListTimerEvent.Add(NewTimerEvent);
                m_iListTimerEventID.Add(iReturnID);
            }
            return iReturnID;
        }
        public void RemoveTimeEvent(int _iID)
        {
            lock (m_ListTimerEvent)
            {
                int iIndex = m_iListTimerEventID.IndexOf(_iID);
                m_iListTimerEventID.RemoveAt(iIndex);
                m_ListTimerEvent.RemoveAt(iIndex);
                m_TimerEventPool.RecycleObject(_iID);
            }
        }
        public void Pause()
        {
            m_bPause = true;
        }
        private void Loop()
        {
            while (true)
            {
                Thread.Sleep(m_iDeltaTime);
                if (!m_bPause)
                {
                    Update(m_iDeltaTime);
                }
            }
        }
        private void Update(int _iDeltaTime)
        {
            int iListCount = m_ListTimerEvent.Count;
            for(int i= iListCount - 1; i >= 0; --i)
            {
                TimerEvent TimeEventNow = m_ListTimerEvent[i];
                if (TimeEventNow.Tick(_iDeltaTime))
                {
                    TimeEventNow.DoFunction();
                    if(TimeEventNow.GetTimerMode() == ETimerMode.REPEAT)
                    {
                        TimeEventNow.ReStart();
                    }
                    else
                    {
                        RemoveTimeEvent(m_iListTimerEventID[i]);
                    }
                }
            }
        }
        

    }
}
