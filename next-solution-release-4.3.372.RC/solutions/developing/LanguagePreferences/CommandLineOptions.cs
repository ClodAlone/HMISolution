using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguagePreferences
{
    class CommandLineOptions
    {
        #region Constructors
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        const string parCulture = "/C";
        const string parCallingProcessId = "/P";
        const string parCurrentSkin = "/Y";
        //const string parKey = "/K";
        //const string parValue = "/V";

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

            if (argTable.ContainsKey(parCulture))
            {
                CultureName = argTable[parCulture];
                SilentMode = true;
            }

            if (argTable.ContainsKey(parCallingProcessId))
            {
                int id;
                if (int.TryParse(argTable[parCallingProcessId], out id))
                    CallingProcessId = id;
            }

            if (argTable.ContainsKey(parCurrentSkin) && !String.IsNullOrEmpty(argTable[parCurrentSkin]))
            {
                CurrentSkin = argTable[parCurrentSkin];
            }

            //if (argTable.ContainsKey(parKey))
            //{
            //    KeyPath = argTable[parKey];
            //}
            //if (argTable.ContainsKey(parValue))
            //{
            //    KeyValue = argTable[parValue];
            //}

            //if (KeyPath == null)
            //    KeyPath = Properties.Settings.Default.DefaultKeyPath;
            //if (KeyValue == null)
            //    KeyValue = Properties.Settings.Default.DefaultKeyValue;

            IsValid = true;
            if (SilentMode && CultureName == null)
                IsValid = false;
            //if (String.IsNullOrWhiteSpace(KeyPath) || String.IsNullOrWhiteSpace(KeyValue))
            //    IsValid = false;
        }

        private string _CultureName;
        public string CultureName
        {
            get { return _CultureName; }
            set
            {
                _CultureName = value;
            }
        }

        private int _CallingProcessId;
        public int CallingProcessId
        {
            get { return _CallingProcessId; }
            set
            {
                _CallingProcessId = value;
            }
        }

        public string CurrentSkin
        {
            get;
            private set;
        } = "Blend";

        //private string _KeyPath;
        //public string KeyPath
        //{
        //    get { return _KeyPath; }
        //    set
        //    {
        //        _KeyPath = value;
        //    }
        //}

        //private string _KeyValue;
        //public string KeyValue
        //{
        //    get { return _KeyValue; }
        //    set
        //    {
        //        _KeyValue = value;
        //    }
        //}
        
        private bool _SilentMode;
        public bool SilentMode
        {
            get { return _SilentMode; }
            set
            {
                _SilentMode = value;
            }
        }

        private bool _IsValid;
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
