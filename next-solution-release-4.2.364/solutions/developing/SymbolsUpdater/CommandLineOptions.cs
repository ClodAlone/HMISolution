using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectUpdater
{
    class CommandLineOptions
    {
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        //const string parScreenList = "/L";
        //const string parSep = "/S";
        const string parProject = "/P";

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
            
            //if (argTable.ContainsKey(parScreenList) && argTable.ContainsKey(parSep) && !string.IsNullOrEmpty(argTable[parSep]))
            //{
            //    ScreenList = argTable[parScreenList]?.Split(argTable[parSep].ToCharArray());
            //}

            if (argTable.ContainsKey(parProject))
            {
                Project = argTable[parProject];
            }

            if (/*ScreenList != null && ScreenList.Count() > 0 &&*/ !string.IsNullOrEmpty(Project))
                _IsValid = true;
        }

        //private string[] _Screen;
        //public string[] ScreenList
        //{
        //    get { return _Screen; }
        //    set
        //    {
        //        _Screen = value;
        //    }
        //}

        private string _Project = string.Empty;
        public string Project
        {
            get { return _Project; }
            set
            {
                _Project = value;
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
