using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFSolutionNext
{
    static internal class Numerical
    {
        static internal EventHandler processed;
        static void FireProcessed(bool ok)
        {
            var e = processed;
            if (e != null)
                e(ok ? processed : null, EventArgs.Empty);
        }

        static bool initialized = false;
        static internal void VerifyNumerical()
        {
            if (initialized)
                return;
            initialized = true;
            Task.Factory.StartNew(() =>
            {
                var oldPriority = Thread.CurrentThread.Priority;
                try
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var assemblyPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assembly.Location), 
                        WPFUtilities.CryptString.CryptString.DecryptString("u2MUIZoiwsHg0ARAi4/J/w==")); // "MSZ.dll"
                    var array = File.ReadAllBytes(assemblyPath);
                    var hash = GetNumerical(array);
#if !NET_STANDARD
                    var ohash1 = ""; // to change every time msz.dll changes
                    FireProcessed(hash == ohash1);
#else
                    var ohash2 = ""; // to change every time msz.dll changes
                    FireProcessed(hash == ohash2);
#endif
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

        static internal string GetNumerical(byte[] array)
        {
            var algo = new SHA256Managed();
            return Convert.ToBase64String(algo.ComputeHash(array));
        }
    }
}
