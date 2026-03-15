using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenerateHash
{
    class Program
    {
        #region Declarations
        static string hashText = "var {0} = ";
        static string hashTextCrypted = "public static string {0} = ";
        #endregion

        static void Main(string[] args)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - RestoreManager", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            if (args.Length > 0)
            {
                CommandLineOptions cl = new CommandLineOptions(args);
                if (cl.IsValid)
                {
                    try
                    {
                        var array = File.ReadAllBytes(cl.Source);
                        var hashValue = UFSolutionNext.Numerical.GetNumerical(array);
                        if (String.IsNullOrWhiteSpace(hashValue))
                            throw new InvalidOperationException("Invalid hash");
                        WriteHash(cl.Destination, cl.HashVarName, hashValue, cl.Crypted);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Environment.ExitCode = ex.HResult;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid options");
                    Environment.ExitCode = -1;
                }
            }
            else
            {
                Console.WriteLine("Invalid options");
                Environment.ExitCode = -1;
            }
        }

        static void WriteHash(string csFile, string hashName, string hashValue, bool isCrypted)
        {
            string text = File.ReadAllText(csFile);
            string searchText;
            if (isCrypted)
                searchText = string.Format(hashTextCrypted, hashName);
            else
                searchText = string.Format(hashText, hashName);
            int nStart = text.IndexOf(searchText);
            if (nStart == -1)
                throw new InvalidOperationException("Missing hash to replace in the destination file");
            var nEnd = text.IndexOf(';', nStart);
            if (nEnd == -1)
                throw new InvalidOperationException("Missing hash to replace in the destination file");
            text = text.Replace(text.Substring(nStart, nEnd - nStart), 
                string.Format("{0}\"{1}\"", searchText, isCrypted ? WPFUtilities.CryptString.CryptString.EncryptString(hashValue) : hashValue));
            File.WriteAllText(csFile, text);
        }
    }
}
