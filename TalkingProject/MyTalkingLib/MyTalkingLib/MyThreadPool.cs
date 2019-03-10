using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MyTalkingLib
{
    public class MyThreadPool
    {
        private const int WAITTIMEOUT = 30000;
        private const int DEALWITHNUMBER = 10;

        private List<Thread> m_ListThread = new List<Thread>();
        private Queue<WorkItem> m_QueueWorkItem = new Queue<WorkItem>();
        private ManualResetEvent m_ResetEvent = new ManualResetEvent(false);
        private int iMaxThread = 20;
        private int m_iAvailableThreadNumber = 0;
        private object m_oLockThreadNumber = new object();
        private int m_iSleepTime = 1;
        private bool m_bExit = false;
        private class WorkItem
        {
            WaitCallback m_CallBack = null;
            object m_Parameter = null;
            public WorkItem(WaitCallback _CallBack, object _Parameter)
            {
                m_CallBack = _CallBack;
                m_Parameter = _Parameter;
            }
            public void Execute()
            {
                m_CallBack(m_Parameter);
            }
        }
        public MyThreadPool(int _iMaxThread = 10)
        {
            iMaxThread = _iMaxThread;
        }
        public void QueueUserWorkItem(WaitCallback _CallBack = null, object _Parameter = null)
        {
            WorkItem NewWorkItem = new WorkItem(_CallBack, _Parameter);
            lock (m_QueueWorkItem)
            {
                m_QueueWorkItem.Enqueue(NewWorkItem);
            }
            if (m_ListThread.Count < iMaxThread && (m_iAvailableThreadNumber <= 0 || (m_QueueWorkItem.Count > m_ListThread.Count * DEALWITHNUMBER)))
            {
                CreateThread();
            }
            //m_ResetEvent.Reset();
            m_ResetEvent.Set();
        }
        public void Exit()
        {
            lock (m_QueueWorkItem)
            {
                m_QueueWorkItem.Clear();
            }
            m_bExit = true;
            m_ResetEvent.Set();

            int iListCount = m_ListThread.Count;
            for(int i=0;i< iListCount; ++i)
            {
                m_ListThread[i].Join();
            }
            m_ListThread.Clear();
            Console.WriteLine("ThreadPool Exit Finished");
        }
        private void CreateThread()
        {
            Thread NewThread = new Thread(ThreadDoFunction);
            NewThread.SetApartmentState(ApartmentState.STA);
            lock (m_ListThread)
            {
                m_ListThread.Add(NewThread);
            }
            NewThread.Start();

            lock (m_oLockThreadNumber)
            {
                ++m_iAvailableThreadNumber;
            }
            Console.WriteLine("CreateThread : " + m_ListThread.Count);
        }
        private void ThreadDoFunction()
        {
            WorkItem MyWorkItem;
            while (!m_bExit)
            {
                m_ResetEvent.Reset();
                while (m_QueueWorkItem.Count > 0)
                {
                    MyWorkItem = null;
                    lock (m_QueueWorkItem)
                    {
                        if (m_QueueWorkItem.Count > 0)
                        {
                            lock (m_oLockThreadNumber)
                            {
                                --m_iAvailableThreadNumber;
                            }
                            MyWorkItem = m_QueueWorkItem.Dequeue();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (MyWorkItem != null)
                    {
                        MyWorkItem.Execute();
                        lock (m_oLockThreadNumber)
                        {
                            ++m_iAvailableThreadNumber;
                        }
                    }
                }
                //Console.WriteLine("StartWait");
                m_ResetEvent.WaitOne();
                //Console.WriteLine("EndWait");
            }
            Console.WriteLine("ThreadExit");
        }
    }
}
