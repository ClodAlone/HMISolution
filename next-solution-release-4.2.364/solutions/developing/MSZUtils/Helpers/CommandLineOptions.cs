using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSZUtils
{
    public enum Operations : int
    {
        OpInvalid,
        OpLocal,
        OpRemote
    }
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parLocal = "/H";
        const string parPort = "/P";
        const string parFile = "/F";

        public void Parse(string[] args)
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

            if (argTable.ContainsKey(parFile))
            {
                Filename = argTable[parFile];
            }

            if (argTable.ContainsKey(parPort))
            {
                Port = argTable[parPort];
            }

            if (argTable.ContainsKey(parLocal))
            {
                Operation = Operations.OpLocal;
            }
            else
            {
                Operation = Operations.OpRemote;
            }


            switch (Operation)
            {
                case Operations.OpLocal:
                case Operations.OpRemote:
                    if (!string.IsNullOrEmpty(_hostname))
                        _IsValid = true;
                    else
                        Operation = Operations.OpRemote;
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
        private string _hostname = string.Empty;
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                _hostname = value;
            }
        }

        private string _port = string.Empty;
        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
            }
        }

        private string _filename = string.Empty;
        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
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
