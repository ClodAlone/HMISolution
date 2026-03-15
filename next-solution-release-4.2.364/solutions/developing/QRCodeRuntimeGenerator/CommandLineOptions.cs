using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeRuntimeGenerator
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
        const string parQRCode = "/Q";
        const string parPrint = "/P";
        const string parImage = "/I";

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

            if (argTable.ContainsKey(parPrint))
            {
                Operation = Operations.OpSection;
                DirectPrint = true;
            }
            if (argTable.ContainsKey(parQRCode))
            {
                QRCode = argTable[parQRCode];
                Operation = Operations.OpSection;
            }
            if (argTable.ContainsKey(parImage))
            {
                ImagePath = argTable[parImage];
                Operation = Operations.OpSection;
            }

            switch (Operation)
            {
                case Operations.OpInteractive:
                case Operations.OpSection:
                    if (!string.IsNullOrEmpty(QRCode))
                        _IsValid = true;
                    else
                        Operation = Operations.OpInteractive;
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
        private string _QRCode = string.Empty;
        public string QRCode
        {
            get { return _QRCode; }
            set
            {
                _QRCode = value;
            }
        }

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set
            {
                _ImagePath = value;
            }
        }
        
        private bool _DirectPrint = false;
        public bool DirectPrint
        {
            get { return _DirectPrint; }
            set
            {
                _DirectPrint = value;
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
