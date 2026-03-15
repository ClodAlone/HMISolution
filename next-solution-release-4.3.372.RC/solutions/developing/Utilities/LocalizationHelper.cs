using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Utilities
{
    public static class LocalizationHelper
    {
        #region Declarations
        static string languageValue = @"UICulture";
        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static byte[] localizationHelperValue { get; private set; } // bytes value for runtime process

        #region Static Constructors
#if !DEBUG
        static LocalizationHelper()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                localizationHelperValue = System.IO.File.ReadAllBytes(assembly.Location);
            }
            catch
            { }
        }
#endif
        #endregion

        #region Static Methods
        /// <summary>
        /// Read current language from registry.
        /// </summary>
        /// <returns>
        /// Return the current culture format (ex. en-EN).
        /// </returns>
        public static string ReadCurrentLanguage()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(RegistryKeysHelper.SoftwareProductKey);
                if (key != null)
                {
                    Object o = key.GetValue(languageValue);
                    if (o is String && !String.IsNullOrWhiteSpace(o as String))
                    {
                        return (string)o;
                    }
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }

            return Properties.Settings.Default.DefaultUICulture;
        }

        /// <summary>
        /// Apply current language reading it from registry.
        /// </summary>
        public static void ApplyCurrentLanguage()
        {
            var culture = ReadCurrentLanguage();
            if (!String.IsNullOrWhiteSpace(culture))
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture =
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture, true);
            }

            if (Console.In != System.IO.StreamReader.Null)
            {
                var encoding = Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentUICulture.TextInfo.OEMCodePage);
                Console.OutputEncoding = encoding;
            }
        }

        /// <summary>
        /// Try to apply current language reading it from registry.
        /// </summary>
        public static void TryApplyCurrentLanguage()
        {
            var culture = ReadCurrentLanguage();
            try
            {
                if (!String.IsNullOrWhiteSpace(culture))
                {
                    System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture =
                        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture, true);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error applying new language for current thred to '{0}', error '{1}'", culture, e.Message);
            }

            if (Console.In != System.IO.StreamReader.Null)
            {
                Encoding encoding = Encoding.Default;
                try
                {
                    encoding = Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentUICulture.TextInfo.OEMCodePage);
                    Console.OutputEncoding = encoding;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Error applying the encoding for current console to '{0}', error '{1}'", encoding, e.Message);
                }
            }
        }

        /// <summary>
        /// Change current language with a new culture string value.
        /// </summary>
        /// <param name="culture">
        /// String who defines the new culture to use (ex. "en-EN").
        /// </param>
        public static void ChangeCurrentLanguage(string culture)
        {
            var key = Registry.CurrentUser.CreateSubKey(RegistryKeysHelper.SoftwareProductKey);
            if (key != null)
            {
                key.SetValue(languageValue, culture);
            }
        }
        #endregion
    }
}
