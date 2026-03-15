using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverBaseInterfaces;

namespace ADPluginInterfaces
{
    
    public interface IPlugin
    {
        bool Init(String strSettingPath);
        int OnSendMessage(object message);
        event EventHandler<SystemEventArgs> SystemEvent;
        string GetLastError();

        event EventHandler<SendResultEventArgs> SendResultEvent;
        void StopPlugin();
        bool SendMultiple();
    }
}
