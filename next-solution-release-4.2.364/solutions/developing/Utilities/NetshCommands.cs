using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.HttpHelper
{
    public class NetshCommands
    {
        #region Static Methods
        /// <summary>
        /// Reserves the specified URL for non-administrator users and accounts.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true when successfully executed</returns>
        public static bool TryToRegisterUrl(String url)
        {
            return TryToRegisterUrl(url, null);
        }

        /// <summary>
        /// Reserves the specified URL for non-administrator users and accounts.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <returns>true when successfully executed</returns>
        public static bool TryToRegisterUrl(String url, String user)
        {
            try
            {
                RegisterUrl(url, user);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reserves the specified URL for non-administrator users and accounts.
        /// throw exception if an error occurs
        /// </summary>
        /// <param name="url"></param>
        public static void RegisterUrl(String url)
        {
            RegisterUrl(url, null);
        }

        /// <summary>
        /// Reserves the specified URL for non-administrator users and accounts.
        /// throw exception if an error occurs
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        public static void RegisterUrl(String url, String user)
        {
            // check first if the registration is necessary (not elevation required)
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = String.Format("/C netsh.exe http show urlacl url={0}", url),
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                // this exception not avoid to execute the registration anyway
            }

            if (user == null)
                user = Utilities.CurrentUser.Name();

            // make registration only if is necessary (elevation required)
            var output = process.StandardOutput.ReadToEnd();
            bool urlFound = output.ToLower().Contains(url.ToLower());
            bool userFound = output.ToLower().Contains(user.ToLower());
            if (!urlFound || !userFound)
            {
                StringBuilder arguments = new StringBuilder("/C ");
                if (urlFound && !userFound)
                {
                    arguments.AppendFormat("netsh.exe http delete urlacl url={0}&&", url);
                }
                arguments.AppendFormat("netsh.exe http add urlacl url={0} user={1}", url, user);

                startInfo.Arguments = arguments.ToString();
                startInfo.Verb = "runas";
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = false;
                startInfo.UseShellExecute = true;
                process.Start();
            }
        }

        /// <summary>
        /// Deletes a reserved URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true when successfully executed</returns>
        public static bool TryToUnRegisterUrl(String url)
        {
            try
            {
                UnRegisterUrl(url);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Deletes a reserved URL.
        /// throw exception if an error occurs
        /// </summary>
        /// <param name="url"></param>
        public static void UnRegisterUrl(String url)
        {
            // check first if the unregistration is necessary (not elevation required)
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = String.Format("/C netsh.exe http show urlacl url={0}", url),
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                // this exception not avoid to execute the registration anyway
            }

            // make unregistration only if is necessary (elevation required)
            if (String.IsNullOrEmpty(process.StandardError.ReadToEnd()) &&
                process.StandardOutput.ReadToEnd().ToLower().Contains(url.ToLower()))
            {
                startInfo.Arguments = String.Format("/C netsh.exe http delete urlacl url={0}", url);
                startInfo.Verb = "runas";
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = false;
                startInfo.UseShellExecute = true;
                process.Start();
            }
        }

        public static void BindCertificateToPort(String thumbprint, int port)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = String.Format("/C netsh http add sslcert ipport=0.0.0.0:{0} appid={{12345678-db90-4b66-8b01-88f7af2e36bf}} certhash={1}", port, thumbprint),
                Verb = "runas",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = true
            };
            process.StartInfo = startInfo;
            process.Start();
        }
        #endregion
    }
}
