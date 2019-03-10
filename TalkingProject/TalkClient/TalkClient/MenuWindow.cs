using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkClient
{
    public partial class MenuWindow : Form
    {
        public class PersonInfo
        {
            public int m_iID;
            public string m_strName;
            public PersonInfo(int _iID, string _strName)
            {
                m_iID = _iID;
                m_strName = _strName;
            }
        }
        private TalkClient.DelInsertToQueue m_InsertToServerQueue;

        public MenuWindow(TalkClient.DelInsertToQueue _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
        }
        
        public void AddInfo(object _Parameter)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.FriendMenu.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MySetInfo = new TalkClient.DelSetUIInfo(AddPersonInfo);
                this.Invoke(MySetInfo, new object[] { _Parameter });
            }
            else
            {
                AddPersonInfo(_Parameter);
            }
        }
        private void AddPersonInfo(object _Parameter)
        {
            PersonInfo MyInfo = _Parameter as PersonInfo;

            TableLayoutPanel panel = FriendMenu;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            PersonInfoDLL.PersonInfoClass NewInfo = new PersonInfoDLL.PersonInfoClass(m_InsertToServerQueue);
            panel.Controls.Add(NewInfo, 0, panel.RowCount - 1);
            Console.WriteLine("Name : " + MyInfo.m_strName);
            NewInfo.SetInfo(_iID: MyInfo.m_iID, _strName: MyInfo.m_strName);
        }
        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TableLayoutPanel panel = FriendMenu;
            // For Add New Row (Loop this code for add multiple rows)
            panel.RowCount = panel.RowCount + 1;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(new PersonInfoDLL.PersonInfoClass(m_InsertToServerQueue), 0, panel.RowCount - 1);

        }

        private void MenuWindow_Load(object sender, EventArgs e)
        {

        }

        private void FriendMenu_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
