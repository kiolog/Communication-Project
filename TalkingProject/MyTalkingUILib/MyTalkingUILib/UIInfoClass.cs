using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTalkingUILib
{
    public class UIInfoClass
    {
        private List<object> m_ListParameter = new List<object>();
        public UIInfoClass(List<object> _ListParameter)
        {
            m_ListParameter = _ListParameter;
        }
        public UIInfoClass(object[] _ListParameter)
        {
            m_ListParameter = new List<object>(_ListParameter);
        }
        public object Dequeue()
        {
            object ReturnObject = null;
            if(m_ListParameter.Count > 0)
            {
                ReturnObject = m_ListParameter[0];
                m_ListParameter.RemoveAt(0);
            }
            return ReturnObject;
        }
        public void Enqueue(object _Parameter)
        {
            m_ListParameter.Add(_Parameter);
        }
    }
}
