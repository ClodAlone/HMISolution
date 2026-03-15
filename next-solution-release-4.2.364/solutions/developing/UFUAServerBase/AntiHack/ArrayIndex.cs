using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFSolutionNext
{
    static internal class ArrayIndex
    {
        static internal EventHandler processed;
        static void FireProcessed(bool ok)
        {
            var e = processed;
            if (e != null)
                e(ok ? processed : null, EventArgs.Empty);
        }

        static int initialized = 0;
        static internal void VerifyArrayIndex(int index)
        {
            if ((initialized & index) != 0)
                return;
            initialized |= index;
            Task.Factory.StartNew(() =>
            {
                var oldPriority = Thread.CurrentThread.Priority;
                string ohash = null;
                byte[] array = null;
                try
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    switch (index)
                    {
#if !NET_STANDARD
                        case 0x1:
                            if (RuntimeInformation.ProcessArchitecture == Architecture.X86 && RuntimeInformation.OSArchitecture == Architecture.X64)
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash1_x86); // crypted hash value for 'MoviconNExT.exe' (32 bits)
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            else
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash1); // crypted hash value for 'MoviconNExT.exe'
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            break;
                        case 0x2:
                            if (RuntimeInformation.ProcessArchitecture == Architecture.X86 && RuntimeInformation.OSArchitecture == Architecture.X64)
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash2_x86); // crypted hash value for 'MoviconNextRT.exe' (32 bits)
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            else
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash2); // crypted hash value for 'MoviconNextRT.exe'
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            break;
                        case 0x4:
                            if (RuntimeInformation.ProcessArchitecture == Architecture.X86 && RuntimeInformation.OSArchitecture == Architecture.X64)
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash3_x86); // crypted hash value for 'Connext.exe' (32 bits)
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            else
                            {
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash3); // crypted hash value for 'Connext.exe'
                                array = Utilities.LocalizationHelper.localizationHelperValue;
                            }
                            break;
#endif
                        case 0x8:
#if !NET_STANDARD
                            ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash4_1); // crypted hash value for 'UFUAServerBase.dll' net48
                            array = Utilities.AssemblyResolver.AssemblyResolver.assemblyResolverValue;
#else
                            ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash4_2); // crypted hash value for 'UFUAServerBase.dll' netstandard
                            array = Utilities.AssemblyResolver.AssemblyResolver.assemblyResolverValue;
#endif
                            break;
                        case 0x10:
#if !NET_STANDARD
                            ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash5_1); // crypted hash value for 'MSZ.dll' net48
                            array = Utilities.RegistryKeysHelper.registryKeysHelperValue;
#else
                            ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash5_2); // crypted hash value for 'MSZ.dll' netstandard
                            array = Utilities.RegistryKeysHelper.registryKeysHelperValue;
#endif
                            break;
                        case 0x20:
                            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                            //var isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#if !NET_STANDARD
                            ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash6_1); // crypted hash value for 'libipld.dll' net48 on Windows OS
#else
                            if (isWindows)
                                ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash6_1); // crypted hash value for 'libipld.dll' net48/netstandard on Windows OS
                            else if (isLinux)
                                switch (RuntimeInformation.OSArchitecture)
                                {
                                    case Architecture.X64:
                                    case Architecture.X86:
                                        ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash6_2); // crypted hash value for 'libipld.so' netstandard on Linux OS
                                        break;

                                    case Architecture.Arm:
                                    case Architecture.Arm64:
                                        ohash = WPFUtilities.CryptString.CryptString.DecryptString(Utilities.AssemblyInfo.ohash6_3); // crypted hash value for 'libipld.so' netstandard on Linux OS ARM/ARM64
                                        break;

                                    default:
                                        break;
                                }
#endif
                            array = Utilities.RegistryKeysHelper.registryKeysHelperValue2;
                            break;
                    }

                    //var array = File.ReadAllBytes(assemblyPath);
                    var hash = GetArrayIndex(array);
                    FireProcessed(hash == ohash);
                }
                catch (Exception ex)
                {
                    FireProcessed(false);
                }
                finally
                {
                    Thread.CurrentThread.Priority = oldPriority;
                }
            });
        }

        static internal string GetArrayIndex(byte[] array)
        {
            var algo = new SHA256Managed();
            return Convert.ToBase64String(algo.ComputeHash(array));
        }
    }
}
