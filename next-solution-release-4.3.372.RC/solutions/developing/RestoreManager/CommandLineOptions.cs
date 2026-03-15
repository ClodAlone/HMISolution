using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreManager
{
    public enum Operations : int
    {
        OpInvalid,
        OpSection,
        OpInteractive
    }
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parSource = "/S";
        const string parDest = "/D";
        const string parOption = "/O";
        const string parDLROptions = "/R";
        const string parRedundancy = "/Y";
        const string parCallingProcessId = "/I";

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

            if (argTable.ContainsKey(parSource))
            {
                Operation = Operations.OpSection;
                Source = argTable[parSource];
            }
            if (argTable.ContainsKey(parDest))
            {
                Operation = Operations.OpSection;
                Destination = argTable[parDest];
            }
            if (argTable.ContainsKey(parOption))
            {
                Option = int.Parse(argTable[parOption]);
                Operation = Operations.OpSection;
            }
            if (argTable.ContainsKey(parDLROptions))
            {
                DLROptions = argTable[parDLROptions];
                Operation = Operations.OpSection;
            }
            if (argTable.ContainsKey(parRedundancy))
            {
                Redundancy = true;
            }

            if (argTable.ContainsKey(parCallingProcessId))
            {
                int id;
                if (int.TryParse(argTable[parCallingProcessId], out id))
                    CallingProcessId = id;
            }

            switch (Operation)
            {
                case Operations.OpSection:
                    if (!string.IsNullOrEmpty(parSource) && !string.IsNullOrEmpty(parDest) && !string.IsNullOrEmpty(parOption))
                        _IsValid = true;
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
        private string _Source = string.Empty;
        public string Source
        {
            get { return _Source; }
            set
            {
                _Source = value;
            }
        }

        private string _Destination;
        public string Destination
        {
            get { return _Destination; }
            set
            {
                _Destination = value;
            }
        }
        
        private int _Option = 0;
        public int Option
        {
            get { return _Option; }
            set
            {
                _Option = value;
            }
        }
        private string _DLROptions = string.Empty;
        public string DLROptions
        {
            get { return _DLROptions; }
            set
            {
                _DLROptions = value;
            }
        }
        public bool Redundancy { get; set; }

        private int _CallingProcessId;
        public int CallingProcessId
        {
            get { return _CallingProcessId; }
            set
            {
                _CallingProcessId = value;
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
