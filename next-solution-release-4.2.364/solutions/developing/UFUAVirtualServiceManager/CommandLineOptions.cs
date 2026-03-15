using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFUAVirtualServiceManager
{
    public enum Operations : int
    {
        OpInvalid,
        OpSection,
        OpInteractive
    }

    public enum Results : int
    {
        Error,
        InvalidAdministrativeRight,
        InvalidCommandArg,
        EmptyCommandArg,
        Completed
    }

    public class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parServer = "/S";
        const string parUser = "/U";
        const string parPassword = "/P";
        const string parTrusted = "/E";

        public void Parse(string [] args)
        {
            _IsValid = false;
            if (args.Length < 1)
                return;
            Dictionary<string, string> argTable = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.Length > 2)
                {
                    argTable[a.Substring(0, 2).ToUpper()] = a.Substring(2);
                }
                else
                {
                    argTable[a.ToUpper()] = string.Empty;
                }
            }

            if (argTable.ContainsKey(parServer))
            {
                Server = argTable[parServer];
                Operation = Operations.OpSection;
            }
            
            if (argTable.ContainsKey(parUser))
            {
                User = argTable[parUser];
                Operation = Operations.OpSection;
            }

            if (argTable.ContainsKey(parPassword))
            {
                Password = argTable[parPassword];
                Operation = Operations.OpSection;
            }

            if (argTable.ContainsKey(parTrusted))
            {
                Trusted = true;
                Operation = Operations.OpSection;
            }

            switch (Operation)
            { 
                case Operations.OpSection:
                    if (Server.Length > 0 && 
                        ((User.Length > 0 && Password.Length > 0 && !Trusted) ||
                         (Trusted)))
                    {
                        Operation = Operations.OpSection;
                        _IsValid = true;
                    }
                    break;
            }
        }
        
        private Operations _Operation = Operations.OpInvalid;
        public Operations Operation
        {
            get { return _Operation; }
            set
            {
                _Operation = value;
            }
        }
        
        private string _Server;
        public string Server
        {
            get { return _Server; }
            set
            {
                _Server = value;
            }
        }

        private string _User = string.Empty;
        public string User
        {
            get { return _User; }
            set
            {
                _User = value;
            }
        }

        private string _Password = string.Empty;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
            }
        }

        private bool _Trusted = false;
        public bool Trusted
        {
            get { return _Trusted; }
            set
            {
                _Trusted = value;
            }
        }

        private bool _IsValid = false;
        public bool IsValid
        {
            get { return _IsValid; }
            set
            {
                _IsValid = value;
            }
        }
    }
}
