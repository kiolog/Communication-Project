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
    public partial class LoginForm : Form
    {
        private delegate void DelSetMessage(string _strMessage);
        TalkClient.DelInsertToQueue m_InsertToServerQueue;
        public LoginForm(TalkClient.DelInsertToQueue _CallBack)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
        }

        public void CloseTheForm()
        {
            if (this.Message.InvokeRequired)
            {
                TalkClient.DelSetUIInfo MyUIOperation = new TalkClient.DelSetUIInfo(MyCloseTheForm);
                this.Invoke(MyUIOperation, new object[] { null });
            }
            else
            {
                MyCloseTheForm(null);
            }
        }

        public void SetMessage(object _Parameter)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            string strMessage = _Parameter as string;
            if (this.Message.InvokeRequired)
            {
                DelSetMessage MySetMessage = new DelSetMessage(SetMessageCallBack);
                this.Invoke(MySetMessage, new object[] { strMessage });
            }
            else
            {
                SetMessageCallBack(strMessage);
            }
        }
        private void MyCloseTheForm(object _Parameter)
        {
            this.Close();
        }
        private void SetMessageCallBack(string _strMessage)
        {
            Console.WriteLine("RecevieMessage");
            Message.BackColor = Message.BackColor;
            Message.Text = _strMessage;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.LOGINSERVER));
            ListSendByte.Add((byte)TalkClient.EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)TalkClient.EventType.REGISTER);
            ListSendByte.AddRange(BitConverter.GetBytes(AccountBox.Text.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(AccountBox.Text));
            ListSendByte.AddRange(BitConverter.GetBytes(PasswordBox.Text.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(PasswordBox.Text));
            m_InsertToServerQueue(ListSendByte);
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)TalkClient.Program.ServerType.LOGINSERVER));
            ListSendByte.Add((byte)TalkClient.EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)TalkClient.EventType.SIGNIN);
            ListSendByte.AddRange(BitConverter.GetBytes(AccountBox.Text.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(AccountBox.Text));
            ListSendByte.AddRange(BitConverter.GetBytes(PasswordBox.Text.Length));
            ListSendByte.AddRange(System.Text.Encoding.UTF8.GetBytes(PasswordBox.Text));
            m_InsertToServerQueue(ListSendByte);

            Console.WriteLine("ClickSinINButton");
        }

        private void Message_TextChanged(object sender, EventArgs e)
        {

        }

        private void PasswordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void AccountBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
