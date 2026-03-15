using MSZ.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WPFUtilities.CryptString;

namespace MSZ.Services
{
    public class LicenseKeyService : ILicenseKeyService
    {
        private const string UNIXFilePath = "LuUqHyxnEbGt7vuaToc0+w=="; // "/etc/movicon"
        private const string UNIXFileName = "EWDgn5phlF1PIOl0hg+YTA=="; // "keys.lic"
        private readonly string _encryptedFilePath;
        private readonly string _modifier;

        public LicenseKeyService(string modifier, string encryptedFilePath = null)
        {
            _modifier = modifier;

            try
            {
                if (encryptedFilePath != null)
                {
                    _encryptedFilePath = encryptedFilePath;
                }
                else
                {
                    var decryptedPath = CryptString.DecryptString(UNIXFilePath);
                    var fileName = CryptString.DecryptString(UNIXFileName);
                    var fullPath = Path.Combine(decryptedPath, fileName);
                    _encryptedFilePath = CryptString.EncryptString(fullPath);
                }
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }

        }


        public List<string> ReadAll(List<string> removeKeys = null)
        {
            var ret = removeKeys ?? new List<string>();
            try
            {
                var decryptedFilePath = CryptString.DecryptString(_encryptedFilePath);

                if (!File.Exists(decryptedFilePath))
                    return ret;

                var lines = File.ReadAllLines(decryptedFilePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var serial = CryptString.DecryptString(line.Trim());
                    if (serial.StartsWith(_modifier))
                        serial = serial.Substring(_modifier.Length);
                    if (!ret.Contains(serial))
                        ret.Add(serial);
                }
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }

            return ret;
        }

        public void Write(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return;
                var filePath = CryptString.DecryptString(_encryptedFilePath);
                if(File.Exists(filePath) && File.ReadLines(filePath).Contains(code)) return;

                //var fileInfo = new FileInfo(filePath);
                //if (!Directory.Exists(fileInfo.Directory.FullName))
                //    Directory.CreateDirectory(fileInfo.Directory.FullName);
                //if (!File.Exists(filePath))
                //    File.Create(filePath);
                
                File.AppendAllText(filePath, code + Environment.NewLine);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        public void CleanAll()
        {
            try
            {
                var decryptedFilePath = CryptString.DecryptString(_encryptedFilePath);
                if (File.Exists(decryptedFilePath))
                    File.Delete(decryptedFilePath);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }

        public string Remove(string value)
        {
            try
            {
                var decryptedFilePath = CryptString.DecryptString(_encryptedFilePath);
                if (!File.Exists(decryptedFilePath))
                    return null;

                var allLines = File.ReadAllLines(decryptedFilePath);
                if (!allLines.Contains(value))
                    return null;

                var decryptedFileName = CryptString.DecryptString(value);
                var keyCode = decryptedFileName.StartsWith(_modifier)
                    ? decryptedFileName.Substring(_modifier.Length)
                    : decryptedFileName;

                var lines = File.ReadAllLines(decryptedFilePath)
                    .Where(line => line.Trim() != value)
                    .ToArray();
                File.WriteAllLines(decryptedFilePath, lines);

                return CryptString.EncryptString(keyCode);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                return null;
            }
        }

        private static void WriteError(Exception ex)
        {
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
        }
    }
}