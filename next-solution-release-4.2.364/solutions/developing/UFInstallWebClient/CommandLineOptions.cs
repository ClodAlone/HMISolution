using System;
using System.Collections.Generic;

namespace UFInstallWebClient
{
    internal class CommandLineOptions
    {
        #region Declarations
        const string parWebSite = "/W";
        const string parAliasName = "/A";
        const string parApplicationName = "/N";
        const string parDeployPath = "/D";
        const string parProjectPath = "/P";
        const string parTheme = "/T";
        const string parAutoLogout = "/L";
        const string parRequireLogon = "/E";
        const string parMaxInvalidPasswordAttempts = "/M";
        const string parSpecialFolders = "/S";
        const string parChildProjects = "/C";
        const string parCallingProcessId = "/I";
        const string parSkin = "/Y";
        #endregion

        public static void Parse(string[] args, IISViewModel viewModel)
        {
            if (args.Length < 1)
                return;
            Dictionary<string, string> argTable = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.Length > 2)
                {
                    argTable[a.Substring(0, 2).ToUpper()] = a.Substring(2);
                }
                else
                {
                    argTable[a.ToUpper()] = string.Empty;
                }
            }

            if (argTable.ContainsKey(parAliasName))
                viewModel.AliasName = argTable[parAliasName];

            // this value cannot be change in the dialog and will be write in the web.config
            if (argTable.ContainsKey(parApplicationName))
                viewModel.ApplicationName = argTable[parApplicationName];
            else if (argTable.ContainsKey(parAliasName))
                viewModel.ApplicationName = argTable[parAliasName];

            if (argTable.ContainsKey(parDeployPath))
            {
                viewModel.DeployPath = argTable[parDeployPath];
            }
            else
            {
                try
                {
                    if (IIS7Manager.IISWebsite.Exist(viewModel.WebSite))
                    {
                        var website = IIS7Manager.IISWebsite.OpenWebsite(viewModel.WebSite);
                        if (website != null)
                            viewModel.DeployPath = Environment.ExpandEnvironmentVariables(website.Root.PhisicalPath);
                    }
                }
                catch (Exception ex)
                {
                    viewModel.DeployPath = "C:\\inetpub\\wwwroot";
                }
            }

            if (argTable.ContainsKey(parTheme))
            {
                viewModel.ApplicationTheme = argTable[parTheme];
            }

            if (argTable.ContainsKey(parProjectPath))
            {
                viewModel.WebClientType = WebClientType.Html5;
                viewModel.UriProjectPath = argTable[parProjectPath];
            }
            else
                viewModel.WebClientType = WebClientType.Silverlight;

            if (argTable.ContainsKey(parSpecialFolders))
            {
                viewModel.UriSpecialFolders = argTable[parSpecialFolders].Split('|');
            }

            if (argTable.ContainsKey(parChildProjects))
            {
                viewModel.UriChildProjects = argTable[parChildProjects].Split('|');
            }

            if (argTable.ContainsKey(parAutoLogout))
            {
                int result;
                if (int.TryParse(argTable[parAutoLogout], out result))
                    viewModel.AutoLogoutSeconds = result;
            }

            if (argTable.ContainsKey(parRequireLogon))
            {
                bool result;
                if (bool.TryParse(argTable[parRequireLogon], out result))
                    viewModel.EnableUserManager = result;
            }

            if (argTable.ContainsKey(parMaxInvalidPasswordAttempts))
            {
                int result;
                if (int.TryParse(argTable[parMaxInvalidPasswordAttempts], out result))
                    viewModel.MaxInvalidPasswordAttempts = result;
            }

            if (argTable.ContainsKey(parCallingProcessId))
            {
                int id;
                if (int.TryParse(argTable[parCallingProcessId], out id))
                    viewModel.CallingProcessId = id;
            }

            if (argTable.ContainsKey(parSkin) && !String.IsNullOrEmpty(argTable[parSkin]))
            {
                viewModel.CurrentSkin = argTable[parSkin];
            }
        }

        
    }
}
