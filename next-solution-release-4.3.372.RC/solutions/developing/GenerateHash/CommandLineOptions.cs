using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateHash
{
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }

        const string parHashVarName = "/V";
        const string parSource = "/S";
        const string parDest = "/D";
        const string parCrypted = "/C";

        public void Parse(string[] args)
        {
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

            if (argTable.ContainsKey(parHashVarName))
            {
                HashVarName = argTable[parHashVarName];
            }

            if (argTable.ContainsKey(parSource))
            {
                Source = argTable[parSource];
            }

            if (argTable.ContainsKey(parDest))
            {
                Destination = argTable[parDest];
            }

            if (argTable.ContainsKey(parCrypted))
            {
                Crypted = true;
            }
        }

        string _HashVarName;
        public string HashVarName
        {
            get { return _HashVarName; }
            set
            {
                _HashVarName = value;
            }
        }

        string _Source;
        public string Source
        {
            get { return _Source; }
            set
            {
                _Source = value;
            }
        }

        string _Destination;
        public string Destination
        {
            get { return _Destination; }
            set
            {
                _Destination = value;
            }
        }

        bool _Crypted;
        public bool Crypted
        {
            get { return _Crypted; }
            set
            {
                _Crypted = value;
            }
        }

        public bool IsValid
        {
            get 
            {
                return !String.IsNullOrWhiteSpace(Source) && 
                    !String.IsNullOrWhiteSpace(Destination) && 
                    !String.IsNullOrWhiteSpace(HashVarName);
            }
        }
    }
}
