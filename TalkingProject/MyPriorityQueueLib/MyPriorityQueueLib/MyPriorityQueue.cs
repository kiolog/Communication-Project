using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPriorityQueueLib
{
    public class MyPriorityQueue
    {
        public int Count
        {
            get { return m_iComponentNumber; }
        }

        private int m_iMaxPriorityNumber = 10;
        private List<Queue<List<byte>>> m_PriorityQueue = new List<Queue<List<byte>>>();
        private Dictionary<byte, int> m_DicCommandPriority = new Dictionary<byte, int>();
        private List<int> m_iListPriority = new List<int>();
        private int m_iComponentNumber = 0;
        public MyPriorityQueue()
        {
            for (int i = 0; i <= m_iMaxPriorityNumber; ++i)
            {
                m_PriorityQueue.Add(new Queue<List<byte>>());
            }
        }

        public void Enqueue(byte _oPriorityObject, List<byte> _Value)
        {
            int iPriority = m_iMaxPriorityNumber;
            lock (m_DicCommandPriority)
            {
                if (m_DicCommandPriority.ContainsKey(_oPriorityObject))
                {
                    iPriority = m_DicCommandPriority[_oPriorityObject];
                }
            }
            lock (m_PriorityQueue)
            {
                m_PriorityQueue[iPriority].Enqueue(_Value);
                ++m_iComponentNumber;
            }
        }
        public List<byte> Dequeue()
        {
            List<byte> ReturnValue = new List<byte>();
            lock (m_PriorityQueue)
            {
                for (int i = 0; i <= m_iMaxPriorityNumber; ++i)
                {
                    Queue<List<byte>> QueueThisTime = m_PriorityQueue[i];
                    if (QueueThisTime.Count > 0)
                    {
                        ReturnValue = QueueThisTime.Dequeue();
                        --m_iComponentNumber;
                        break;
                    }
                }
            }

            return ReturnValue;
        }

        public void SetPriorityRule(byte _oRuleObject, int _iPriority)
        {

            if (_iPriority < m_iMaxPriorityNumber)
            {
                lock (m_DicCommandPriority)
                {
                    m_DicCommandPriority.Add(_oRuleObject, _iPriority);
                }
            }
        }

    }
}
