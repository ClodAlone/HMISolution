using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;

namespace MSZUnitTests
{
    [TestClass]
    public class RemovedLicenseModeTests
    {
        #region Generals
        public void InitializeLicense()
        {
            var result = MSZ.MSZView.CheckState(true);
        }
        #endregion 

        #region Unit Tests
        [TestMethod]
        public void TestRegistryWriteDeleteWithEncryption()
        {
            String path = "C:\\Temp\\RegistryWriter.exe";

            // RegistryWriteValue.
            var arguments = String.Format("/E /C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1, 
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue", "",
                Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = WPFUtilities.CryptString.CryptString.EncryptString(arguments);
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryDeleteKey.
            arguments = String.Format("/E /C\"{0}\" /K\"{1}\"", 4,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID");

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = WPFUtilities.CryptString.CryptString.EncryptString(arguments);
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }
        }

        [TestMethod]
        public void TestRegistryWriteDeleteWithoutEncryption()
        {
            String path = "C:\\Temp\\RegistryWriter.exe";

            // RegistryWriteValue.
            var arguments = String.Format("/C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue", "",
                Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryDeleteKey.
            arguments = String.Format("/C\"{0}\" /K\"{1}\"", 4,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID");

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }
        }

        [TestMethod]
        public void TestRegistryUpdateValue()
        {
            String path = "C:\\Temp\\RegistryWriter.exe";

            // RegistryWriteValue.
            var arguments = String.Format("/C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue", "",
                Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryUpdateValue.
            arguments = String.Format("/C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 2,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue", "12345567890",
                Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryDeleteKey.
            arguments = String.Format("/C\"{0}\" /K\"{1}\"", 4,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID");

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }
        }

        [TestMethod]
        public void TestRegistryDeleteValue()
        {
            String path = "C:\\Temp\\RegistryWriter.exe";

            // RegistryWriteValue.
            var arguments = String.Format("/C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue", "",
                Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryDeleteValue.
            arguments = String.Format("/C\"{0}\" /K\"{1}\" /N\"{2}\"", 3,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID", "TestValue");

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }

            // RegistryDeleteKey.
            arguments = String.Format("/C\"{0}\" /K\"{1}\"", 4,
                    @"HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID");

            using (var winProcess = new System.Diagnostics.Process())
            {
                winProcess.StartInfo.FileName = path;
                winProcess.StartInfo.Arguments = arguments;
                winProcess.StartInfo.UseShellExecute = false;
                winProcess.StartInfo.CreateNoWindow = true;
                winProcess.Start();
            }
        }
        #endregion
    }
}
