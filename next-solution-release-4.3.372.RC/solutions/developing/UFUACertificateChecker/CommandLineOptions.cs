using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFUACertificateChecker
{
    public enum Operations : int
    {
        OpInvalid,
        OpSection,
        OpInteractive
    }

    public class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parInteractive = "/I";
        const string parSection = "/S";
        const string parCreateNew = "/N";
        const string parExe = "/E";
        const string parClient = "/C";
        const string parAppName = "/A";
        const string parSkin = "/Y";

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
            if (argTable.ContainsKey(parSection))
            {
                Operation = Operations.OpSection;
                SectionName = argTable[parSection];
            }
            
            if (argTable.ContainsKey(parExe))
            {
                Operation = Operations.OpSection;
                ExePath = argTable[parExe];
            }

            if (argTable.ContainsKey(parCreateNew))
            {
                CrateNew = true;
            }

            if (argTable.ContainsKey(parInteractive))
            {
                Operation = Operations.OpInteractive;
            }

            if (argTable.ContainsKey(parClient))
            {
                ApplicationType = Opc.Ua.ApplicationType.Client;
            }

            if (argTable.ContainsKey(parAppName))
            {
                ApplicationName = argTable[parAppName];
            }

            if (argTable.ContainsKey(parSkin) && !String.IsNullOrEmpty(argTable[parSkin]))
            {
                CurrentSkin = argTable[parSkin];
            }

            switch (Operation)
            { 
                case  Operations.OpInteractive:
                case Operations.OpSection:
                    if (SectionName.Length > 0 && ExePath.Length > 0)
                        _IsValid = true;
                    break;
            }
        }
        private Opc.Ua.ApplicationType _ApplicationType = Opc.Ua.ApplicationType.Server;
        public Opc.Ua.ApplicationType ApplicationType
        {
            get { return _ApplicationType; }
            set
            {
                _ApplicationType = value;
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

        private bool _CreateNew;
        public bool CrateNew
        {
            get { return _CreateNew; }
            set
            {
                _CreateNew = value;
            }
        }

        private string _ExePath = string.Empty;
        public string ExePath
        {
            get { return _ExePath; }
            set
            {
                _ExePath = value;
            }
        }
        
        private string _SectionName;
        public string SectionName
        {
            get { return _SectionName; }
            set
            {
                _SectionName = value;
            }
        }

        private string _ApplicationName;
        public string ApplicationName
        {
            get { return _ApplicationName; }
            set
            {
                _ApplicationName = value;
            }
        }

        public string CurrentSkin
        {
            get;
            set;
        } = "Blend";

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
