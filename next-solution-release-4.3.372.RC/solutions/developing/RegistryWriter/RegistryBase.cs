using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryWriter
{
    abstract class RegistryBase : IDisposable
    {
        #region Declarations
        readonly protected CommandLineOptions options;
        readonly protected RegistryKey baseKey;
        #endregion

        #region Constructors
        public RegistryBase(CommandLineOptions options)
        {
            this.options = options;
            baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
        }
        #endregion

        #region Methods
        public abstract void Execute();

        protected void Obfuscation()
        {
            System.Security.Principal.WindowsPrincipal pricipal = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) || options.RegistryHive == RegistryHive.CurrentUser;

            // generate guids;
            Guid[] guids = new Guid[10];
            for (int ii = 0; ii < guids.Length; ii++)
                guids[ii] = Guid.NewGuid();

            // read and write sub-keys
            for (int ii = 0; ii < guids.Length; ii++)
            {
                using (var key = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    var key2 = key.OpenSubKey(guids[ii].ToString());
                    if (hasAdministrativeRight && key2 == null)
                        key.CreateSubKey(guids[ii].ToString());
                }
            }

            // add values inside sub-keys
            for (int ii = 0; ii < guids.Length; ii++)
            {
                using (var key = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    var key2 = key.OpenSubKey(guids[ii].ToString(), hasAdministrativeRight);
                    if (hasAdministrativeRight && key2 != null)
                        key2.SetValue(WPFUtilities.CryptString.CryptString.EncryptString(guids[ii].ToString()), DateTime.UtcNow.Ticks);
                }
            }

            // read values inside sub-keys
            for (int ii = 0; ii < guids.Length; ii++)
            {
                using (var key = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    var key2 = key.OpenSubKey(guids[ii].ToString(), hasAdministrativeRight);
                    if (hasAdministrativeRight && key2 != null)
                        key2.GetValue(WPFUtilities.CryptString.CryptString.EncryptString(guids[ii].ToString()));
                }
            }

            // delete values inside sub-keys
            for (int ii = 0; ii < guids.Length; ii++)
            {
                using (var key = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {

                    var key2 = key.OpenSubKey(guids[ii].ToString(), hasAdministrativeRight);
                    if (hasAdministrativeRight && key2 != null)
                        key2.DeleteValue(WPFUtilities.CryptString.CryptString.EncryptString(guids[ii].ToString()));
                }
            }

            // delete sub-keys
            for (int ii = 0; ii < guids.Length; ii++)
            {
                using (var key = Microsoft.Win32.RegistryKey.OpenBaseKey(options.RegistryHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {

                    var key2 = key.OpenSubKey(guids[ii].ToString(), hasAdministrativeRight);
                    if (hasAdministrativeRight && key2 != null)
                        key.DeleteSubKey(guids[ii].ToString());
                }
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            if (baseKey != null)
            {
                baseKey.Close();
                baseKey.Dispose();
            }
        }
        #endregion
    }
}
