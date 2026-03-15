using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace UFUAVirtualServiceManager
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            if (!System.IO.Directory.Exists("c:\\temp"))
                System.IO.Directory.CreateDirectory("c:\\temp");
#endif
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - UFUAVirtualServiceManager", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
            bool isRedirected;

            try
            {
                isRedirected = Console.CursorVisible && false;
            }
            catch
            {
                isRedirected = true;
            }

            if (!hasAdministrativeRight)
            {
#if DEBUG
                using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                {
                    s.WriteLine(DateTime.Now.ToString() + "\t" + "No administrative right");
                    s.WriteLine($"RunElevated at {System.Windows.Forms.Application.ExecutablePath}");
                }
#endif
                RunElevated(System.Windows.Forms.Application.ExecutablePath);

                if (!isRedirected)
                    Console.Error.WriteLine($"InvalidAdministrativeRight: RunElevated {System.Windows.Forms.Application.ExecutablePath}");
                Environment.Exit((int)Results.Completed);
            }

#if DEBUG
            string msg = string.Empty;

            try
            {
                bool fIsRunAsAdmin = IsRunAsAdmin();
                msg += string.Format("RunAsAdmin: {0}\n", fIsRunAsAdmin);
            }
            catch (Exception ex)
            {
#if DEBUG
                using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                s.WriteLine($"{DateTime.Now.ToString()} IsRunAsAdmin: {ex.Message}");
#endif
                if (!isRedirected)
                    Console.Error.WriteLine(ex.Message);
            }

            if (Environment.OSVersion.Version.Major >= 6)
            {
                // Running Windows Vista or later (major version >= 6). 

                try
                {
                    // Get and display the process elevation information.
                    bool fIsElevated = IsProcessElevated();
                    msg += string.Format("IsElevated: {0}\n", fIsElevated);
                }
                catch (Exception ex)
                {
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                    s.WriteLine($"{DateTime.Now.ToString()} IsProcessElevated: {ex.Message}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine(ex.Message);
                }

                try
                {
                    // Get and display the process integrity level.
                    int IL = GetProcessIntegrityLevel();
                    switch (IL)
                    {
                        case NativeMethods.SECURITY_MANDATORY_UNTRUSTED_RID:
                            msg += "Integrity Level: Untrusted\n"; break;
                        case NativeMethods.SECURITY_MANDATORY_LOW_RID:
                            msg += "Integrity Level: Low\n"; break;
                        case NativeMethods.SECURITY_MANDATORY_MEDIUM_RID:
                            msg += "Integrity Level: Medium\n"; break;
                        case NativeMethods.SECURITY_MANDATORY_HIGH_RID:
                            msg += "Integrity Level: High\n"; break;
                        case NativeMethods.SECURITY_MANDATORY_SYSTEM_RID:
                            msg += "Integrity Level: System\n"; break;
                        default:
                            msg += "Integrity Level: Unknown\n"; break;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                        s.WriteLine($"{DateTime.Now.ToString()} GetProcessIntegrityLevel: {ex.Message}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine(ex.Message);
                }
            }
#if DEBUG
            using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                s.WriteLine($"{DateTime.Now.ToString()} Process Integrity Level: {msg}");
#endif
            if (!isRedirected)
                Console.WriteLine($"Process Integrity Level: {msg}");
            //Debug.Print($"Process Integrity Level: {msg}");
#endif
            if (args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(args);
                if (cl.IsValid)
                {
                    switch (cl.Operation)
                    {
                        case Operations.OpSection:
                            {
                                VirtualServiceMng vsMng = new VirtualServiceMng();
                                try
                                {
                                    vsMng.CreateVirtualUser(cl);
                                }
                                catch (Exception ex)
                                {
#if DEBUG
                                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                                    s.WriteLine($"{DateTime.Now.ToString()} CreateVirtualUser: {ex.Message}");
#endif
                                    if (!isRedirected)
                                        Console.Error.WriteLine(ex.Message);
                                    Environment.Exit((int)Results.Error);
                                }
                            }
                            break;
                        default:
#if DEBUG
                            using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                            s.WriteLine($"{DateTime.Now.ToString()} {Results.InvalidCommandArg.ToString()}");
#endif
                            if (!isRedirected)
                                Console.Error.WriteLine(Results.InvalidCommandArg.ToString());
                            Environment.Exit((int)Results.InvalidCommandArg);
                            break;
                    }
                }
                else
                {
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                    s.WriteLine($"{DateTime.Now.ToString()} {Results.InvalidCommandArg.ToString()}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine(Results.InvalidCommandArg.ToString());
                    Environment.Exit((int)Results.InvalidCommandArg);
                }
            }
            else
            {
#if DEBUG
                using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                    s.WriteLine($"{DateTime.Now.ToString()} {Results.EmptyCommandArg.ToString()}");
#endif
                if (!isRedirected)
                    Console.Error.WriteLine(Results.EmptyCommandArg.ToString());
                Environment.Exit((int)Results.EmptyCommandArg);
            }
#if DEBUG
            using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                s.WriteLine($"{DateTime.Now.ToString()} {Results.Completed.ToString()}");
#endif
            if (!isRedirected)
                Console.Error.WriteLine(Results.Completed.ToString());
            Environment.Exit((int)Results.Completed);
        }
        /// <summary>
        /// The function gets the integrity level of the current process. Integrity 
        /// level is only available on Windows Vista and newer operating systems, thus 
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
        /// prior to Windows Vista.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of 
        /// these values:
        /// 
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used 
        ///    by processes started by the Anonymous group. Blocks most write access.
        ///    (SID: S-1-16-0x0)
        ///    
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects 
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
        /// 
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is 
        ///    used by normal applications being launched while UAC is enabled. 
        ///    (SID: S-1-16-0x2000)
        ///    
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used 
        ///    by administrative applications launched through elevation when UAC is 
        ///    enabled, or normal applications if UAC is disabled and the user is an 
        ///    administrator. (SID: S-1-16-0x3000)
        ///    
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is 
        ///    used by services and other system-level applications (such as Wininit, 
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
        /// 
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        static int GetProcessIntegrityLevel()
        {
            int IL = -1;
            SafeTokenHandle hToken = null;
            int cbTokenIL = 0;
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Then we must query the size of the integrity level information 
                // associated with the token. Note that we expect GetTokenInformation 
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code 
                // because we've given it a null buffer. On exit cbTokenIL will tell 
                // the size of the group information.
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0,
                    out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to 
                        // Windows Vista, GetTokenInformation returns false with the 
                        // ERROR_INVALID_PARAMETER error code because 
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Now we ask for the integrity level information again. This may fail 
                // if an administrator has added this account to an additional group 
                // between our first call to GetTokenInformation and this one.
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
                // S-1-16-0x1000 stands for low integrity level SID). There is one 
                // and only one subauthority.
                IntPtr pIL = NativeMethods.GetSidSubAuthority(tokenIL.Label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenIL != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenIL);
                    pTokenIL = IntPtr.Zero;
                    cbTokenIL = 0;
                }
            }

            return IL;
        }
        static bool RunElevated(string fileName)
        {
            //MessageBox.Show("Run: " + fileName);
            ProcessStartInfo processInfo = new ProcessStartInfo() { Verb = "runas", FileName = fileName };
            processInfo.Arguments = Environment.CommandLine;
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }
        /// <summary>
        /// The function checks whether the current process is run as administrator.
        /// In other words, it dictates whether the primary access token of the 
        /// process belongs to user account that is a member of the local 
        /// Administrators group and it is elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group and it is 
        /// elevated. Returns false if the token does not.
        /// </returns>
        static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// The function gets the elevation information of the current process. It 
        /// dictates whether the process is elevated or not. Token elevation is only 
        /// available on Windows Vista and newer operating systems, thus 
        /// IsProcessElevated throws a C++ exception if it is called on systems prior 
        /// to Windows Vista. It is not appropriate to use this function to determine 
        /// whether a process is run as administartor.
        /// </summary>
        /// <returns>
        /// Returns true if the process is elevated. Returns false if it is not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        /// <remarks>
        /// TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
        /// type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
        /// TokenElevationTypeFull) of the process. It is different from TokenElevation 
        /// in that, when UAC is turned off, elevation type always returns 
        /// TokenElevationTypeDefault even though the process is elevated (Integrity 
        /// Level == High). In other words, it is not safe to say if the process is 
        /// elevated based on elevation type. Instead, we should use TokenElevation. 
        /// </remarks>
        static bool IsProcessElevated()
        {
            bool fIsElevated = false;
            SafeTokenHandle hToken = null;
            int cbTokenElevation = 0;
            IntPtr pTokenElevation = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Allocate a buffer for the elevation information.
                cbTokenElevation = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
                pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation);
                if (pTokenElevation == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Retrieve token elevation information.
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenElevation, pTokenElevation,
                    cbTokenElevation, out cbTokenElevation))
                {
                    // When the process is run on operating systems prior to Windows 
                    // Vista, GetTokenInformation returns false with the error code 
                    // ERROR_INVALID_PARAMETER because TokenElevation is not supported 
                    // on those operating systems.
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_ELEVATION struct from native to .NET object.
                TOKEN_ELEVATION elevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(
                    pTokenElevation, typeof(TOKEN_ELEVATION));

                // TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the token 
                // has elevated privileges; otherwise, a zero value.
                fIsElevated = (elevation.TokenIsElevated != 0);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenElevation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenElevation);
                    pTokenElevation = IntPtr.Zero;
                    cbTokenElevation = 0;
                }
            }

            return fIsElevated;
        }
    }
}
