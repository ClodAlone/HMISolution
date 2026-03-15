using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryWriter
{
    public enum CommandType : int
    {
        None,
        RegistryWriteValue,
        RegistryUpdateValue,
        RegistryDeleteValue,
        RegistryDeleteKey,
    }

    public enum ValueType : int
    {
        StringValue,
        DWORDValue,
        QWORDValue
    }

    class CommandLineOptions
    {
        #region Declarations
        const string parEncryption = "/E";
        const string parCommand = "/C";
        const string parKey = "/K";
        const string parName = "/N";
        const string parValue = "/V";
        const string parType = "/T";

        const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
        const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";
        const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
        const string HKEY_DYN_DATA = "HKEY_DYN_DATA";
        const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
        const string HKEY_PERFORMANCE_DATA = "HKEY_PERFORMANCE_DATA";
        const string HKEY_USERS = "HKEY_USERS";
        #endregion

        #region Constructors
        public CommandLineOptions() { }

        public CommandLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        #region Methods
        void Parse(string[] args)
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

            if (argTable.ContainsKey(parCommand))
            {
                int value;
                if (int.TryParse(argTable[parCommand], out value))
                    CommandType = (CommandType)value;
            }

            if (argTable.ContainsKey(parEncryption))
            {
                Encryption = true;
            }

            if (argTable.ContainsKey(parKey))
            {
                RegistryKey = argTable[parKey];
            }

            if (argTable.ContainsKey(parName))
            {
                KeyName = argTable[parName];
            }

            if (argTable.ContainsKey(parValue))
            {
                KeyValue = argTable[parValue];
            }

            if (argTable.ContainsKey(parType))
            {
                RegistryValueKind kind;
                if (Enum.TryParse<RegistryValueKind>(argTable[parType], out kind))
                    KeyType = kind;
            }
        }

        RegistryHive FindRegistryHive(String fullKeyPath, out String partialKeyPath)
        {
            var registryhive = Microsoft.Win32.RegistryHive.CurrentUser;
            partialKeyPath = fullKeyPath;
            
            fullKeyPath = fullKeyPath.ToUpper();
            if (fullKeyPath.StartsWith(HKEY_CLASSES_ROOT))
            {
                registryhive = Microsoft.Win32.RegistryHive.ClassesRoot;
                partialKeyPath = partialKeyPath.Substring(HKEY_CLASSES_ROOT.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_CURRENT_CONFIG))
            {
                registryhive = Microsoft.Win32.RegistryHive.CurrentConfig;
                partialKeyPath = partialKeyPath.Substring(HKEY_CURRENT_CONFIG.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_CURRENT_USER))
            {
                registryhive = Microsoft.Win32.RegistryHive.CurrentUser;
                partialKeyPath = partialKeyPath.Substring(HKEY_CURRENT_USER.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_DYN_DATA))
            {
                registryhive = Microsoft.Win32.RegistryHive.DynData;
                partialKeyPath = partialKeyPath.Substring(HKEY_DYN_DATA.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_LOCAL_MACHINE))
            {
                registryhive = Microsoft.Win32.RegistryHive.LocalMachine;
                partialKeyPath = partialKeyPath.Substring(HKEY_LOCAL_MACHINE.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_PERFORMANCE_DATA))
            {
                registryhive = Microsoft.Win32.RegistryHive.LocalMachine;
                partialKeyPath = partialKeyPath.Substring(HKEY_PERFORMANCE_DATA.Length + 1);
            }
            else if (fullKeyPath.StartsWith(HKEY_USERS))
            {
                registryhive = Microsoft.Win32.RegistryHive.LocalMachine;
                partialKeyPath = partialKeyPath.Substring(HKEY_USERS.Length + 1);
            }

            return registryhive;
        }
        #endregion

        #region Properties
        CommandType commandType = CommandType.None;
        public CommandType CommandType
        {
            get { return commandType; }
            set
            {
                commandType = value;
            }
        }

        bool encryption;
        public bool Encryption
        {
            get { return encryption; }
            set
            {
                encryption = value;
            }
        }

        RegistryHive registryHive = RegistryHive.CurrentUser;
        public RegistryHive RegistryHive
        {
            get { return registryHive; }
            set
            {
                registryHive = value;
            }
        }

        String registryKey;
        public String RegistryKey
        {
            get { return registryKey; }
            set
            {
                registryHive = FindRegistryHive(value, out registryKey);
            }
        }

        String keyName;
        public String KeyName
        {
            get { return keyName; }
            set
            {
                keyName = value;
            }
        }

        String keyValue;
        public String KeyValue
        {
            get { return keyValue; }
            set
            {
                keyValue = value;
            }
        }

        RegistryValueKind keyType = RegistryValueKind.Unknown;
        public RegistryValueKind KeyType
        {
            get { return keyType; }
            set
            {
                keyType = value;
            }
        }

        public bool IsValid
        {
            get
            {
                if (CommandType == RegistryWriter.CommandType.RegistryDeleteKey)
                    return !String.IsNullOrEmpty(RegistryKey);
                else if (CommandType == RegistryWriter.CommandType.RegistryDeleteValue)
                    return !String.IsNullOrEmpty(RegistryKey) && !String.IsNullOrEmpty(KeyName);
                else if (CommandType == RegistryWriter.CommandType.RegistryWriteValue || 
                    CommandType == RegistryWriter.CommandType.RegistryUpdateValue)
                    return !String.IsNullOrEmpty(RegistryKey) && !String.IsNullOrEmpty(KeyName) && KeyValue != null;

                return KeyType != RegistryValueKind.None && KeyType != RegistryValueKind.Unknown;
            }
        }
       
        #endregion
    }
}
