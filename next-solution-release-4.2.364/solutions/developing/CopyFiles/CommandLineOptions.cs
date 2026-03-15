using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFiles
{
    class CommandLineOptions
    {
        #region Declarations
        const string parPattern = "/P";
        const string parSource = "/S";
        const string parDest = "/D";
        const string parSubDir = "/A";
        #endregion

        #region Constructors
        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        #region Methods
        void Parse(string[] args)
        {
            IsValid = false;
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

            if (argTable.ContainsKey(parPattern))
            {
                SearchPattern = argTable[parPattern];
            }
            if (argTable.ContainsKey(parSource))
            {
                Source = argTable[parSource];
            }
            if (argTable.ContainsKey(parDest))
            {
                Destination = argTable[parDest];
            }

            SubDirectories = argTable.ContainsKey(parSubDir);

            if (!string.IsNullOrEmpty(parSource) && !string.IsNullOrEmpty(parDest))
                IsValid = true;
        }
        #endregion

        #region Properties
        public string SearchPattern { get; private set; }

        public string Source { get; private set; }

        public string Destination { get; private set; }

        public bool SubDirectories { get; private set; }

        public bool IsValid { get; private set; }
        #endregion
    }
}
