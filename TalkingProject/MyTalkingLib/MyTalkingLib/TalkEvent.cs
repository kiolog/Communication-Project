using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTalkingLib
{
    public enum EventType : byte
    {
        NONE,
        RECEIVE,
        SEND,
        CHANGESTATE,
        SIGNIN,
        REGISTER,
        GETMESSAGE,
        FILEMESSAGE,
        SIGNINSUCCESS,
        LOGINMESSAGE,
        SETTALKINGACCOUNTID,
        MENUDATA,
        ADDTALKINGPAGE,
        MESSAGE,
        GETHISTORYMESSAGE,
        STARTSENDFILE,
        SENDFILELOOP,
        SENDFILEEND,
        UPLOADFILE,
        READFILE,
        GETCONNECTID,
        CHECKFILE,
        SENDUPLOADFILE,
        DOWNLOADFILE,
        INSERTTOSOCKETQUEUE,
        STARTREADERFILE,
        READY,
        ADDLOGINUSER,
        SETFILESIZE,
        STOPSENDFILE,
        RESUMEDOWNLOADFILE,
        PAUSEDOWNLOADFILE,
    }
    public enum ServerType
    {
        LOGINSERVER = 0,
        TALKINGSERVER = 1,
    }
    public enum FlowState
    {
        LOGIN = 0,
        TALKING = 1,
    }
    public enum GroupEvent
    {
        ADDMEMBER,
        REMOVEMEMBER,
    }


}
