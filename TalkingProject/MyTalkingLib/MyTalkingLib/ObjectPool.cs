using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTalkingLib
{
    public class ObjectPool<T> where T : new()
    {
        private List<T> m_ListObject = new List<T>();
        private List<int> m_iListUsedID = new List<int>();
        private List<int> m_iListUnUsedID = new List<int>();

        //private int m_iLimit = -1;
        public ObjectPool()
        {
        }
        public T GetObject()
        {
            int iIndex = 0;
            lock (m_ListObject)
            {
                if (m_iListUnUsedID.Count > 0)
                {
                    iIndex = m_iListUnUsedID[0];
                    m_iListUnUsedID.RemoveAt(0);
                    m_iListUsedID.Add(iIndex);
                }
                else
                {
                    T NewObject = new T();
                    m_ListObject.Add(NewObject);
                    iIndex = m_ListObject.Count - 1;
                    m_iListUsedID.Add(iIndex);
                }
            }
            return m_ListObject[iIndex];
        }
        public int GetIDByObject(T _Object)
        {
            return m_ListObject.IndexOf(_Object);
        }
        public void RecycleObject(int _iID)
        {
            lock (m_ListObject)
            {
                m_iListUsedID.Remove(_iID);
                m_iListUnUsedID.Add(_iID);
            }
                
        }
        public void RecycleObject(T _Object)
        {
            RecycleObject(GetIDByObject(_Object));
        }
    }
}
