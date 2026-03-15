using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryWriter
{
    class RegistryUpdateValue : RegistryBase
    {
        #region Declarations
        protected RegistryKey key;
        #endregion

        #region Constructors
        public RegistryUpdateValue(CommandLineOptions options)
            : base(options)
        { }
        #endregion

        #region Overrides
        public override void Execute()
        {
            if (options.Encryption)
                base.Obfuscation();

            key = baseKey.OpenSubKey(options.RegistryKey, true);
            if (key != null)
                key.SetValue(options.KeyName, options.KeyValue, options.KeyType);

            if (options.Encryption)
                base.Obfuscation();
        }

        protected override void OnDispose()
        {
            if (key != null)
            {
                key.Close();
                key.Dispose();
            }

            base.OnDispose();
        }
        #endregion
    }
}
