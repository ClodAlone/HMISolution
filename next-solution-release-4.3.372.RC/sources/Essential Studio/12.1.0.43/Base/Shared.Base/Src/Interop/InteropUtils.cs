#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

namespace Syncfusion.Runtime.InteropServices
{
	/// <summary>
	/// Provides some utility methods regarding the runtime.
	/// </summary>
	public class RuntimeEnvironment
	{
		private static int majorVersion = -1;
		private static int minorVersion = -1;
		/// <summary>
		/// Returns the major runtime version.
		/// </summary>
		public static int MajorRuntimeVersion
		{
			get
			{
				if(majorVersion == -1)
					GetVersionInfo();

				return majorVersion;
			}
		}
		/// <summary>
		/// Returns the minor runtime version.
		/// </summary>
		public static int MinorRuntimeVersion
		{
			get
			{
				if(minorVersion == -1)
					GetVersionInfo();

				return minorVersion;
			}
		}

		private static void GetVersionInfo()
		{
			string runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			int indexofv = runtimeDir.LastIndexOf("v");
			string versionString = runtimeDir.Substring(indexofv + 1);
			versionString.Trim();
			versionString.Trim('\\', '/');

			string[] versions = versionString.Split('.');
			majorVersion = Int32.Parse(versions[0]);
			minorVersion = Int32.Parse(versions[1]);
		}
	}
	/// <summary>
	/// This class will provide more information that the .Net equivalent ignored.
	/// </summary>
	public class SystemInformationExt
	{
		private static bool appTypeKnown = false;
		private static bool isDotNetApp = true;
		/// <summary>
		/// Indicates whether menu access keys are always underlined.
		/// </summary>
		public static bool KeyboardCuesAlwaysOn
		{
			get
			{
				bool alwaysOn = true;
				Syncfusion.Runtime.InteropServices.NativeMethods.SystemParametersInfo(0x100A/*SPI_GETKEYBOARDCUES*/, 0, ref alwaysOn, 0);
				return alwaysOn;
			}
		}

		/// <summary>
		/// Indicates whether the current application is a .Net application.
		/// </summary>
		/// <value>True if .Net; false otherwise.</value>
		/// <remarks>
		/// By default, this method will automatically determine whether or not the current active
		/// app is a .Net app or a native app. However, to speed up performance (by a fraction of a second),
		/// you can set this value appropriately at the beginning of your app. Note that
		/// an incorrect setting would cause unforeseen behavior.
		/// </remarks>
		public static bool IsDotNetApp
		{
			get
			{
				if(SystemInformationExt.appTypeKnown)
					return SystemInformationExt.isDotNetApp;

                else
				{
					bool bHasAppMainType = false;

					try
					{
                        Assembly asmEntry = Assembly.GetEntryAssembly();
                        if (asmEntry != null)
                        {
                            bHasAppMainType = (null != asmEntry.EntryPoint);
                        }

					}
					catch( Exception e )
					{
						// If GetAppMainType is removed or code security denies access, we suppose that application is managed
						Debug.WriteLine( "Exception occurred in SystemInformationExt.IsDotNetApp: " + e.Message );
					}

					bool noException = true;
					try
					{
						// Upon investigation this should fire an exception if
						// hosted within a win32 app.
						// AllowQuit used to be another option but it returns
						// false for dot net apps if called from the start of Main.
						string productName = Application.ProductName;
					}
					catch(Exception)
					{
						noException = false;
					}
					SystemInformationExt.appTypeKnown = true;
					if(noException && bHasAppMainType)
						SystemInformationExt.isDotNetApp = true;
					else
						SystemInformationExt.isDotNetApp = false;
				}

				return SystemInformationExt.isDotNetApp;
			}
			set
			{
				SystemInformationExt.appTypeKnown = true;
				SystemInformationExt.isDotNetApp = value;
			}
		}

        static string executablePathURI = null;
        static bool isDevStudio = false;

        /// <summary>
        /// Indicates whether the component is used inside developer studio.
        /// </summary>
        public static bool IsDevStudio
        {
            get
            {
                if (executablePathURI == null)
                {
                    try
                    {
                        Assembly asm = Assembly.GetEntryAssembly();
                        if (asm != null)
                            executablePathURI = asm.EscapedCodeBase.ToLower();
                        else // Fallback to old original code which is dependant on windows forms
                            executablePathURI = Application.ExecutablePath.ToLower();
                    }
                    catch (Exception e)
                    {
                        // If GetAppMainType is removed or code security denies access, we suppose that application is not run inside dev studio.
                        Debug.WriteLine("Exception occurred in SystemInformationExt.IsDevStudio: " + e.Message);
                        executablePathURI = "";
                        // isDevStudio will be false;
                    }

                    isDevStudio = (executablePathURI.IndexOf("devenv.exe") >= 0) ||
                        (executablePathURI.IndexOf("vbexpress.exe") >= 0) ||
                        (executablePathURI.IndexOf("vcsexpress.exe") >= 0) ||
                        (executablePathURI.IndexOf("vcexpress.exe") >= 0) ||
                        (executablePathURI.IndexOf("vwdexpress.exe") >= 0);
                }

                return isDevStudio;
            }
        }
	}
}