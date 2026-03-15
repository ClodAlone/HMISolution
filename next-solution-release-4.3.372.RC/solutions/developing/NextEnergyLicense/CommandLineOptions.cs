using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextEnergyLicense
{
    public enum Operations : int
    {
        OpInvalid,
        OpSection
    }
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parConnectionString = "/C";

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
            if (argTable.ContainsKey(parConnectionString))
            {
                ConnectionString = argTable[parConnectionString].Replace("&nbsp;"," ");
                Operation = Operations.OpSection;
            }

            switch (Operation)
            {
                case Operations.OpSection:
                    if (!string.IsNullOrEmpty(ConnectionString))
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
        private string _ConnectionString = string.Empty;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
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
