using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteHistoryData
{
    public enum Operations : int
    {
        OpInvalid,
        OpSection,
        OpInteractive
    }

    class CommandLineOptions
    {
        #region Declarations
        const string parConnection = "/C";
        const string parMaxAge = "/A";
        const string parMaxTake = "/T";
        const string parNodeId = "/N";
        const string parCallingProcessId = "/P";
        const string parResetTime = "/R";
        #endregion

        #region Constructors
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        #region Methods
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

            if (argTable.ContainsKey(parConnection))
            {
                Connection = argTable[parConnection];
            }
            if (argTable.ContainsKey(parMaxAge))
            {
                var age = argTable[parMaxAge];
                TimeSpan.TryParse(age, out maxAge);
            }
            if (argTable.ContainsKey(parMaxTake))
            {
                var take = argTable[parMaxTake];
                int.TryParse(take, out maxTake);

            }
            if (argTable.ContainsKey(parNodeId))
            {
                NodeId = argTable[parNodeId];
            }
            if (argTable.ContainsKey(parCallingProcessId))
            {
                int id;
                if (int.TryParse(argTable[parCallingProcessId], out id))
                    CallingProcessId = id;
            }
            if (argTable.ContainsKey(parResetTime))
            {
                var dateTime = argTable[parResetTime];
                DateTime.TryParseExact(dateTime, "G", CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out resetTime);
            }
            else
                resetTime = DateTime.MinValue;
        }
        #endregion

        #region Properties
        private string connection;
        public string Connection
        {
            get { return connection; }
            set
            {
                connection = value;
            }
        }

        private string nodeId;
        public string NodeId
        {
            get { return nodeId; }
            set
            {
                nodeId = value;
            }
        }

        private TimeSpan maxAge;
        public TimeSpan MaxAge
        {
            get { return maxAge; }
            set
            {
                maxAge = value;
            }
        }

        private int maxTake = 10000;
        public int MaxTake
        {
            get { return maxTake; }
            set
            {
                maxTake = value;
            }
        }

        private DateTime resetTime;
        public DateTime ResetTime
        {
            get { return resetTime; }
            set
            {
                resetTime = value;
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

        public SchemaType SchemaType
        {
            get
            {
                if (String.IsNullOrEmpty(NodeId))
                    return DeleteHistoryData.SchemaType.EventLogger;
                else
                    return DeleteHistoryData.SchemaType.Historian;
            }
        }

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                isLocked = value;
            }
        }

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(Connection) || MaxAge == TimeSpan.Zero && ResetTime == DateTime.MinValue || maxTake <= 0)
                    return false;

                return XpoHelpers.XpoHelper.IsDataSource(Connection);
            }
        }
        #endregion
    }
}
