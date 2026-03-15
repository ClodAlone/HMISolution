using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCompiler
{
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        const string parScreen = "/S";
        const string parProject = "/P";
        const string parKeepExtension = "/K";
        const string parCreateBaml = "/B";
        const string parOutputForTest = "/T";
        const string parScreenCompiledPath = "/C";

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
            
            if (argTable.ContainsKey(parScreenCompiledPath))
            {
                ScreenCompiledPath = argTable[parScreenCompiledPath];
            }
            if (argTable.ContainsKey(parScreen))
            {
                Screen = argTable[parScreen];
            }
            if (argTable.ContainsKey(parProject))
            {
                Project = argTable[parProject];
            }

            OutputForTest = argTable.ContainsKey(parOutputForTest);
            KeepExtension = argTable.ContainsKey(parKeepExtension);
            CreateBaml = argTable.ContainsKey(parCreateBaml);

            if (!string.IsNullOrEmpty(Screen) && !string.IsNullOrEmpty(Project))
                _IsValid = true;
        }

        private bool _KeepExtension = false;
        public bool KeepExtension
        {
            get { return _KeepExtension; }
            set
            {
                _KeepExtension = value;
            }
        }

        private bool _OutputForTest = false;
        public bool OutputForTest
        {
            get { return _OutputForTest; }
            set
            {
                _OutputForTest = value;
            }
        }

        private bool _CreateBaml = false;
        public bool CreateBaml
        {
            get { return _CreateBaml; }
            set
            {
                _CreateBaml = value;
            }
        }

        private string _Project = string.Empty;
        public string Project
        {
            get { return _Project; }
            set
            {
                _Project = value;
            }
        }

        private string _Screen = string.Empty;
        public string Screen
        {
            get { return _Screen; }
            set
            {
                _Screen = value;
            }
        }

        private string _ScreenCompiledExtension = string.Empty;
        public string ScreenCompiledPath
        {
            get { return _ScreenCompiledExtension; }
            set
            {
                _ScreenCompiledExtension = value;
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
