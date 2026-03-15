using MSZFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NextLicenseCnsl
{
    class Program
    {
        private static string ExpD { get { return WPFUtilities.CryptString.CryptString.DecryptString("xBRvCvaiIyolXzyKfTBYdA=="); } }
        private static string UnTd { get { return WPFUtilities.CryptString.CryptString.DecryptString("87/5JHLZro50L6530fKBIg=="); } }
        private static string NSrl { get { return WPFUtilities.CryptString.CryptString.DecryptString("WNDataAVWdeTCdxwIqfSYw=="); } }
        static void Main(string[] args)
        {
            try
            {
                StringBuilder argsList = new StringBuilder();
                args.ToList().ForEach(a =>
                {
                    argsList.Append($"{a} - ");
                });
                System.IO.File.WriteAllText("args.txt", argsList.ToString());
                if (args.Count() >= 5)
                {
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        string hash = GetHash(sha256Hash, $"FIT{args[0]}{args[3]}REV{args[1]}{args[2]}");
                        if (args.Count() >= 5 && args[4] == hash)
                        {
                            var path = args[3];
                            var dir = System.IO.Path.GetDirectoryName(args[3]);
                            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                            var par = args[2];

                            if (par.Contains($"{UnTd}=0;"))
                                par.Replace($"{UnTd}=0;", $"{UnTd}=1;");
                            if (!par.Contains($"{UnTd}="))
                                par = $"{par};{UnTd}=1";
                            if (!par.Contains($"{NSrl}="))
                                par = $"{par};{NSrl}=";

                            var craddle = new Craddle();
                            string lic = craddle.Generate(uint.Parse(args[0]),
                                                            args[1],
                                                            args[2]);

                            if (!System.IO.Directory.Exists(dir))
                                System.IO.Directory.CreateDirectory(dir);

                            System.IO.File.WriteAllText(args[3], lic);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string sysLog = "log\\syslog";
                var dir = System.IO.Path.GetDirectoryName(sysLog);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                System.IO.File.AppendAllText($"{sysLog}.txt", $"{DateTime.Now}:{Environment.NewLine}{args[1]} {args[2]} {args[3]}{Environment.NewLine}{ex.Message}{Environment.NewLine}");
                var lines = System.IO.File.ReadAllLines($"{sysLog}.txt");
                if(lines.Count() > 1024)
                {
                    int i = 1;
                    string sysLogRenamed = $"{sysLog}{i}";
                    while (System.IO.File.Exists($"{sysLogRenamed}.txt"))
                    {
                        i++;
                        sysLogRenamed = $"{sysLog}{i}";
                    }
                    System.IO.File.Copy($"{sysLog}.txt", $"{sysLogRenamed}.txt");
                    System.IO.File.WriteAllText($"{sysLog}.txt", string.Empty);
                }
            }
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
