using System;
using System.Collections.Generic;
using System.Linq;

namespace KinectControls
{
    /// <summary>
    /// Interaction logic for KinectWindow.xaml
    /// </summary>
    /// 
    public class SpeechCommandEventArgs : EventArgs
    {
        String command;

        public SpeechCommandEventArgs(string c)
        {
            command = c;
        }
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
    }
}
