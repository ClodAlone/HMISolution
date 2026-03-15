using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;
using Utilities;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MSZUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Declarations
        static string languageValue = @"UICulture";
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TryApplyCurrentLanguage();
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !", "DebugMe", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            if (e.Args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(e.Args);
                if (cl.IsValid)
                {
                    MainWindow = new MainWindow() { DataContext = cl, WindowStartupLocation = WindowStartupLocation.CenterScreen, WindowState = WindowState.Maximized };
                    MainWindow.Show();
                }
                else
                {
                    System.Windows.MessageBox.Show(MSZUtils.Properties.Resources.InvalidOptions);
                    Application.Current.Shutdown(-10);
                }

            }
            else
            {
                MainWindow = new MainWindow() { DataContext = null };
                MainWindow.Show();
            }
        }

        static void TryApplyCurrentLanguage()
        {
            string culture = ReadCurrentLanguage();

            try
            {
                if(String.IsNullOrWhiteSpace(culture))
                {
                    culture = MSZUtils.Properties.Settings.Default.DefaultCulture;
                    ChangeCurrentLanguage(culture);
                }

                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture =
                   System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Threading.Thread.CurrentThread.CurrentCulture = 
                   new System.Globalization.CultureInfo(culture, true);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error applying new language for current thred to '{0}', error '{1}'", "en-US", e.Message);
            }
        }
        
        static string ReadCurrentLanguage()
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(SoftwareProductKey);
                if (key != null)
                {
                    Object o = key.GetValue(languageValue);
                    if (o != null)
                    {
                        return (string)o;
                    }
                }
            }
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                //react appropriately
            }

            return null;
        }

        static void ChangeCurrentLanguage(string culture)
        {
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(SoftwareProductKey);
            if (key != null)
            {
                key.SetValue(languageValue, culture);
            }
        }

        internal static string AssemblyVersion
        {
            get
            {
                return typeof(App).Assembly.GetName().Version.ToString();
            }
        }

        static string Company
        {
            get
            {
                string result = string.Empty;
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((System.Reflection.AssemblyCompanyAttribute)customAttributes[0]).Company;
                }
                return result;
            }
        }

        static string Product
        {
            get
            {
                string result = string.Empty;
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((System.Reflection.AssemblyProductAttribute)customAttributes[0]).Product;
                }
                return result;
            }
        }

        static string SoftwareProductKey
        {
            get
            {
                return  string.Format(@"SOFTWARE\{0}\{1}", Company, Product);
            }
        }
    }
}
