using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MyTalkingLib;
namespace MyTalkingUILib
{
    public partial class LoginForm : Form
    {
        private delegate void DelSetMessage(string _strMessage);
        private EncryptModel m_EncryptModel = null;
        private WaitCallback m_InsertToServerQueue;
        public LoginForm(WaitCallback _CallBack, EncryptModel _EncryptModel)
        {
            InitializeComponent();
            m_InsertToServerQueue = _CallBack;
            m_EncryptModel = _EncryptModel;
        }

        public void CloseTheForm()
        {
            if (this.Message.InvokeRequired)
            {
                WaitCallback MyUIOperation = new WaitCallback(MyCloseTheForm);
                this.Invoke(MyUIOperation, new object[] { null });
            }
            else
            {
                MyCloseTheForm(null);
            }
        }

        public void SetMessage(object _Parameter)
        {
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
            SendCallBackToServer(EventType.REGISTER);

            Console.WriteLine("ClickRegisterButton");
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            SendCallBackToServer(EventType.SIGNIN);

            Console.WriteLine("ClickSinINButton");
        }
        private byte[] GetEncryptAccount()
        {
            string strAccount = AccountBox.Text;
            return m_EncryptModel.Encrypt(strAccount);
        }
        private byte[] GetEncryptPassword()
        {
            string strPassword = PasswordBox.Text;
            return m_EncryptModel.Encrypt(strPassword);
        }
        private void SendCallBackToServer(EventType _Event)
        {
            List<byte> ListSendByte = new List<byte>();
            ListSendByte.AddRange(BitConverter.GetBytes((int)ServerType.LOGINSERVER));
            ListSendByte.Add((byte)EventType.INSERTTOSOCKETQUEUE);
            ListSendByte.Add((byte)_Event);
            byte[] ByteArrayAccount = GetEncryptAccount();
            byte[] ByteArrayPassword = GetEncryptPassword();

            ListSendByte.AddRange(BitConverter.GetBytes(ByteArrayAccount.Length));
            ListSendByte.AddRange(ByteArrayAccount);
            ListSendByte.AddRange(BitConverter.GetBytes(ByteArrayPassword.Length));
            ListSendByte.AddRange(ByteArrayPassword);
            m_InsertToServerQueue(ListSendByte);
        }
    }
}
