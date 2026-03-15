using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using UFInterfaces.Service;
using UFProjectManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;

namespace UFProjectManager.Service
{
    internal class UFWebClientServiceControl : IServiceControl
    {
        enum CommandType
        {
            Open,
            Install,
            Uninstall,
            Start,
            Stop
        }

        #region Declarations
        readonly string applicationName;
        readonly string projectUri;
        readonly int maxInvalidPasswordAttempts;
        readonly static string processFileName = "UFWebClient.Service.exe";

        readonly static ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);

        string arguments;
        string configFilePath;
        #endregion

        #region Constructors
        public UFWebClientServiceControl(string applicationName, string projectUri, int maxInvalidPasswordAttempts)
        {
            this.applicationName = applicationName;
            this.projectUri = projectUri;
            this.maxInvalidPasswordAttempts = maxInvalidPasswordAttempts;
        }
        #endregion

        #region Properties
        string DisplayName
        {
            get
            {
                return String.Format("{0} ({1})", Properties.Resources.UFWebClientServiceFriendlyName, applicationName);
            }
        }

        #endregion

        #region IServiceControl
        public string FriendlyName
        {
            get
            {
                return Properties.Resources.UFWebClientServiceFriendlyName;
            }
        }

        public string Name
        {
            get
            {
                var processName = System.IO.Path.GetFileNameWithoutExtension(processFileName);
                return ServiceInstaller.ServiceInstaller.GetValidServiceName(String.Format("{0} ({1})", processName, applicationName));
            }
        }

        public bool UseCredentialProvider
        {
            get
            {
                return true;
            }
        }

        public void Install()
        {
            CreateSelfSignedCertificate();
            AddHttpAccessRules();
            PrepareConfigurationFile();
            PrepareArguments(CommandType.Install);
            Execute();
        }

        public void Install(string username, string password)
        {
            CreateSelfSignedCertificate();
            AddHttpAccessRules();
            PrepareConfigurationFile();
            PrepareArguments(CommandType.Install, username, password);
            Execute();
        }

        public void OpenServiceControl()
        {
            PrepareArguments(CommandType.Open);
            Execute();
        }

        public void Start()
        {
            PrepareArguments(CommandType.Start);
            Execute();
        }

        public void Stop()
        {
            PrepareArguments(CommandType.Stop);
            Execute();
        }

        public void Uninstall()
        {
            PrepareArguments(CommandType.Uninstall);
            Execute();
        }
        #endregion

        #region Methods
        void CreateSelfSignedCertificate(bool silent = false)
        {
            var tmpFilePath = System.IO.Path.GetTempFileName();
            var hostname = "localhost";
            try
            {
                hostname = System.Net.Dns.GetHostName();
            }
            catch { }
            var command = Encoding.Unicode.GetBytes(
                String.Format(
                    "$certs = dir cert: -Recurse | Where-Object {{ $_.FriendlyName -eq \"{1}\" -and $_.PSParentPath -like \"*LocalMachine\\My\" -and $_.NotAfter -gt (Get-Date) }}; " +
                    "if ($certs) {{ exit }}; " +
                    "$d = (Get-Date).AddYears(10); " +
                    "$newcert = New-SelfSignedCertificate -DnsName {2} -FriendlyName {1} -CertStoreLocation Cert:\\LocalMachine\\My -NotAfter $d; " +
                    "$newCertThumbprint = $newCert.Thumbprint; " +
                    "$store = new-object system.security.cryptography.X509Certificates.X509Store -argumentlist \"Root\", LocalMachine; " +
                    "$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]\"ReadWrite\"); " +
                    "$store.Add($newCert); " +
                    "Set-Content {0} -Value $newCertThumbprint", tmpFilePath, Properties.Settings.Default.WebClientServiceCertificateFriendlyName, hostname
                )
            );
            var encodedCommand = Convert.ToBase64String(command);

            var process = new Process() { EnableRaisingEvents = true };
            var startInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas",
                FileName = "cmd.exe",
                Arguments = String.Format("/C powershell.exe -executionpolicy bypass -encodedCommand {0}", encodedCommand),
                RedirectStandardError = false,
                RedirectStandardOutput = false
            };

            process.StartInfo = startInfo;

            process.Exited += (s, e) =>
            {
                string certificateThumbPrint = System.IO.File.ReadAllText(tmpFilePath).Trim();
                System.IO.File.Delete(tmpFilePath);
                if (!String.IsNullOrEmpty(certificateThumbPrint))
                {
                    string serverUri = null;
                    var webServiceFullPath = GetWebServiceFullPath();
                    var config = ConfigurationManager.OpenExeConfiguration(webServiceFullPath);
                    if (config != null)
                    {
                        var appSetting = config.AppSettings.Settings;
                        serverUri = appSetting["ServerURI"].Value;
                    }

                    if (serverUri != null)
                    {
                        try
                        {
                            Regex r = new Regex(@"^(?<proto>\w+)://?[^/]+?(?<port>:\d+)?", RegexOptions.None, TimeSpan.FromMilliseconds(150));
                            var uris = serverUri.Split(';');
                            foreach (var uri in uris)
                            {
                                Match m = r.Match(uri);
                                if (m.Success) {
                                    var proto = m.Groups["proto"].Value;
                                    var bPortParsed = int.TryParse(m.Groups["port"].Value.Replace(":", ""), out int port);
                                    if (bPortParsed && proto == "https") {
                                        Utilities.HttpHelper.NetshCommands.BindCertificateToPort(certificateThumbPrint, port);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var message = Properties.Resources.CertificateBindingError;
                            message = message.Replace("'newline'", Environment.NewLine);
                            message = String.Format(message, ex.Message);
                            if (!silent)
                            {

                                if (UFProjectManagerComponent.projectManagerComponent.UIInterface != null)
                                {
                                    UFProjectManagerComponent.projectManagerComponent.UIInterface.ShowYesNo(message,
                                        CustomDialogIcons.Warning);
                                }
                                else
                                {
                                    var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                                    var title = doc != null ? doc.Title : "CreateSelfSignedCertificate";
                                    MessageBox.Show(message, title, MessageBoxButton.YesNo);
                                }
                            }
                            else
                                log.Error(message);
                        }
                    }

                }
            };
            process.Start();
        }

        bool AddHttpAccessRules()
        {
            return AddHttpAccessRules(false);
        }

        bool AddHttpAccessRules(string username)
        {
            return AddHttpAccessRules(false, username);
        }

        bool AddHttpAccessRules(bool silent)
        {
            return AddHttpAccessRules(silent, null);
        }

        bool AddHttpAccessRules(bool silent, string username)
        {
            string serverUri = null;
            var webServiceFullPath = GetWebServiceFullPath();
            var config = ConfigurationManager.OpenExeConfiguration(webServiceFullPath);
            if (config != null)
            {
                var appSetting = config.AppSettings.Settings;
                serverUri = appSetting["ServerURI"].Value;
            }

            if (serverUri != null)
            {
                try
                {
                    var uris = serverUri.Split(';');
                    foreach (var uri in uris)
                    {
                        var httpAccessRules = new Utilities.HTTPAccessRules(uri, username);
                        httpAccessRules.CheckUrlsAndAddAccessRules();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    var message = Properties.Resources.HttpRegistrationFailed;
                    message = message.Replace("'newline'", Environment.NewLine);
                    message = String.Format(message, ex.Message);
                    if (!silent)
                    {

                        if (UFProjectManagerComponent.projectManagerComponent.UIInterface != null)
                        {
                            return UFProjectManagerComponent.projectManagerComponent.UIInterface.ShowYesNo(message, 
                                CustomDialogIcons.Warning) == CustomDialogResults.Yes;
                        }
                        else
                        {
                            var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                            var title = doc != null ? doc.Title : "AddHttpAccessRules";
                            return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                        }
                    }
                    else
                        log.Error(message);
                }
            }

            return false;
        }

        void PrepareConfigurationFile()
        {
            var webServiceFullPath = GetWebServiceFullPath();
            var config = ConfigurationManager.OpenExeConfiguration(webServiceFullPath);
            if (config != null)
            {
                var appSetting = config.AppSettings.Settings;
                if (appSetting["ProjectUri"] != null)
                    appSetting["ProjectUri"].Value = projectUri;
                if (appSetting["ClientSessionName"] != null)
                    appSetting["ClientSessionName"].Value = applicationName;

                if (maxInvalidPasswordAttempts > 0)
                {
                    var section = (System.Web.Configuration.MembershipSection)config.GetSection("system.web/membership");
                    if (section != null)
                    {
                        foreach (System.Configuration.ProviderSettings providerSettings in section.Providers)
                        {
                            if (providerSettings.Parameters.AllKeys.Contains("maxInvalidPasswordAttempts"))
                                providerSettings.Parameters["maxInvalidPasswordAttempts"] = maxInvalidPasswordAttempts.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }

                var newConfigFilePath = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                newConfigFilePath = string.Format("{0}.Config\\{1}\\{2}.config", newConfigFilePath, applicationName, processFileName);

                try
                {
                    config.SaveAs(newConfigFilePath, ConfigurationSaveMode.Minimal);
                    configFilePath = newConfigFilePath;
                }
                catch
                {
                    configFilePath = null;
                }
            }
            else
                configFilePath = null;
        }

        void PrepareArguments(CommandType type)
        {
            PrepareArguments(type, null, null);
        }

        void PrepareArguments(CommandType type, string username, string password)
        {
            arguments = string.Format(@"""/N{0}"" ""/Y{1}"" ""/F{2}""", Name, DisplayName, GetWebServiceFullPath());
            if (String.IsNullOrEmpty(configFilePath))
                arguments = string.Format(@"{0} ""/P01-ProjectUri={1}""", arguments, projectUri);
            else
                arguments = string.Format(@"{0} ""/P01-ConfigFilePath={1}""", arguments, configFilePath);
            if (type == CommandType.Install)
            {
                arguments = string.Format("/I {0}", arguments);
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                    arguments = string.Format("{0} /A{1} /W{2}", arguments, username, password);
            }
            else if (type == CommandType.Uninstall)
                arguments = string.Format("/U {0}", arguments);
            else if (type == CommandType.Start)
                arguments = string.Format("/S {0}", arguments);
            else if (type == CommandType.Stop)
                arguments = string.Format("/T {0}", arguments);
        }

        void Execute()
        {
            string path = Properties.Settings.Default.ServiceManagerExecutable;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }

        string GetWebServiceFullPath()
        {
            string rootPath = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                rootPath = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            rootPath = String.Format("{0}\\{1}\\{2}", rootPath ?? ".", "WebService", processFileName);

            return rootPath;
        }
        #endregion
    }
}
