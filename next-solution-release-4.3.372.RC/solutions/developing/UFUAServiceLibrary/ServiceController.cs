using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Utilities.ProgressDialog;

namespace UFUAServiceLibrary
{
    public class ServiceController
    {
        #region Declarations
        readonly CommadLineOptions cmdOptions;
        readonly ServiceUser serviceUser;
        readonly Window owner;
        #endregion

        #region Constructors
        public ServiceController(CommadLineOptions cmdOptions) :
           this(cmdOptions, null, null)
        {
        }

        public ServiceController(CommadLineOptions cmdOptions, ServiceUser serviceUser) :
           this(cmdOptions, serviceUser, null)
        {
        }

        public ServiceController(CommadLineOptions cmdOptions, Window owner) : 
            this(cmdOptions, null, owner)
        {
        }

        public ServiceController(CommadLineOptions cmdOptions, ServiceUser serviceUser, Window owner)
        {
            this.cmdOptions = cmdOptions;
            this.serviceUser = serviceUser;
            this.owner = owner;
        }
        #endregion

        #region Public Methods
        public void InstallAsService()
        {
            string user = serviceUser.UserName;
            string pwd = serviceUser.Password;

            bool add = true;
            string domain = System.Environment.MachineName;
            ContextType ctx = ContextType.Machine;
            string cuser = serviceUser.UserName;
            if (cuser.IndexOf(".\\") != -1)
            {
                int n = cuser.IndexOf(".\\");
                cuser = cuser.Substring(n + 2);
                add = false;
            }
            else if (cuser.IndexOf("\\") != -1)
            {
                string[] arrT = cuser.Split('\\');
                domain = arrT[0];
                cuser = arrT[1];
                ctx = ContextType.Domain;
                add = false;
            }
            else if (cuser.IndexOf('@') != -1)
            {
                string[] arrT = cuser.Split('@');
                domain = arrT[1];
                cuser = arrT[0];
                ctx = ContextType.Domain;
                add = false;
            }

            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pwd))
            {
                try
                {
                    bool isCFR21UserName = false;
                    if (/*ctx == ContextType.Machine && */cuser == UFUAServerInfo.UFUAServerInfo.GetCFR21UserName())
                    {
                        isCFR21UserName = true;
                        pwd = WPFUtilities.CryptString.CryptString.DecryptString(pwd);
                    }

                    using (PrincipalContext context = new PrincipalContext(ctx, domain))
                    {
                        bool present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cuser) != null);
                        if (present && !context.ValidateCredentials(cuser, pwd))
                        {
                            MessageBox.Show(Properties.Resources.InvalidCredentials, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (!present &&
                            MessageBox.Show(string.Format(Properties.Resources.AskCreateUser, user).Replace("--newline--", Environment.NewLine), cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            using (UserPrincipal newuser = new UserPrincipal(context, cuser, pwd, true))
                            {
                                newuser.DisplayName = cuser;
                                newuser.PasswordNeverExpires = isCFR21UserName;
                                newuser.UserCannotChangePassword = isCFR21UserName;
                                newuser.Save();

                                using (var group = GroupPrincipal.FindByIdentity(context, "Users"))
                                {
                                    if (group != null && !group.Members.Contains(newuser))
                                    {
                                        group.Members.Add(newuser);
                                        group.Save();
                                    }
                                }

                                Helpers.LsaUtility.SetRight(user, "SeServiceLogonRight");
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(string.Format(Properties.Resources.UserCreationFailed, cuser, f.Message), cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (add)
                {
                    user = string.Format(".\\{0}", user);
                }

                if (!String.IsNullOrEmpty(cmdOptions.ProjectPath))
                {
                    try
                    {
                        DirectoryInfo dInfo = new DirectoryInfo(Path.GetDirectoryName(cmdOptions.ProjectPath));
                        var accessRule = new System.Security.AccessControl.FileSystemAccessRule(serviceUser.UserName,
                            System.Security.AccessControl.FileSystemRights.Read |
                            System.Security.AccessControl.FileSystemRights.Write |
                            System.Security.AccessControl.FileSystemRights.Delete |
                            System.Security.AccessControl.FileSystemRights.Modify,
                            System.Security.AccessControl.InheritanceFlags.ContainerInherit |
                            System.Security.AccessControl.InheritanceFlags.ObjectInherit,
                            System.Security.AccessControl.PropagationFlags.InheritOnly,
                            System.Security.AccessControl.AccessControlType.Allow);

                        System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl();
                        dSecurity.RemoveAccessRuleAll(accessRule);
                        dSecurity.AddAccessRule(accessRule);
                        dInfo.SetAccessControl(dSecurity);
                    }
                    catch { }
                }
            }

            String error = null;
            if (owner != null)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = owner,
                    ProgressBarIndeterminate = true,
                    DialogText = String.Format(Properties.Resources.ServiceInstalling, cmdOptions.ServiceName),
                    IsCancellingEnabled = false
                };

                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    try
                    {
                        ServiceInstaller.ServiceInstaller.Install(cmdOptions.ServiceName,
                            ((cmdOptions.DisplayName != null && cmdOptions.DisplayName.Length > 0) ? cmdOptions.DisplayName : cmdOptions.ServiceName),
                            String.Format("\"{0}\" {1}", cmdOptions.FilePath, ((cmdOptions.Parameters != null && cmdOptions.Parameters.Length > 0) ? cmdOptions.Parameters : string.Empty)),
                            ((cmdOptions.Dependencies != null && cmdOptions.Dependencies.Length > 0) ? cmdOptions.Dependencies : null), (user.Length == 0 ? null : user), (pwd.Length == 0 ? null : pwd));
                        if (cmdOptions.DelayedAutoStart)
                            ServiceInstaller.ServiceInstaller.ChangeServiceDelayedAutoStart(cmdOptions.ServiceName, true);
                    }
                    catch (Exception ex)
                    {
                        error = String.Format(Properties.Resources.ServiceInstallFailed, ex.Message);
                    }
                });
            }
            else
            {
                try
                {
                    ServiceInstaller.ServiceInstaller.Install(cmdOptions.ServiceName,
                        ((cmdOptions.DisplayName != null && cmdOptions.DisplayName.Length > 0) ? cmdOptions.DisplayName : cmdOptions.ServiceName),
                        String.Format("\"{0}\" {1}", cmdOptions.FilePath, ((cmdOptions.Parameters != null && cmdOptions.Parameters.Length > 0) ? cmdOptions.Parameters : string.Empty)),
                        ((cmdOptions.Dependencies != null && cmdOptions.Dependencies.Length > 0) ? cmdOptions.Dependencies : null), (user.Length == 0 ? null : user), (pwd.Length == 0 ? null : pwd));
                    if (cmdOptions.DelayedAutoStart)
                        ServiceInstaller.ServiceInstaller.ChangeServiceDelayedAutoStart(cmdOptions.ServiceName, true);
                }
                catch (Exception ex)
                {
                    error = String.Format(Properties.Resources.ServiceInstallFailed, ex.Message);
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (cmdOptions.AddHttpsCertificate)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(user))
                            AddHttpsCertificate(user, pwd);
                        else
                            AddHttpsCertificate("SYSTEM", null);
                    }
                    catch
                    { }
                }

                if (!String.IsNullOrEmpty(cmdOptions.Dependencies))
                {
                    StringBuilder dependencyServicesUninstalled = new StringBuilder();
                    var dependencyServices = cmdOptions.Dependencies.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
                    if (dependencyServices != null && dependencyServices.Length > 0)
                    {
                        for (int ii = 0; ii < dependencyServices.Length; ii++)
                        {
                            if (!ServiceInstaller.ServiceInstaller.ServiceIsInstalled(dependencyServices[ii]))
                            {
                                dependencyServicesUninstalled.AppendLine(dependencyServices[ii]);
                            }
                        }
                    }

                    if (dependencyServicesUninstalled.Length > 0)
                    {
                        string warning = String.Format(Properties.Resources.DependencyServicesWarning.Replace("'newline'", System.Environment.NewLine),
                            dependencyServicesUninstalled, cmdOptions.ServiceName);
                        MessageBox.Show(warning, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                if (cmdOptions.CopySourceFolderImages != null && cmdOptions.CopyDestFolderImages != null)
                    TryCopyDirectory(cmdOptions.CopySourceFolderImages, cmdOptions.CopyDestFolderImages);

                if (cmdOptions.CopySourceFolderDocuments != null && cmdOptions.CopyDestFolderDocuments != null)
                    TryCopyDirectory(cmdOptions.CopySourceFolderDocuments, cmdOptions.CopyDestFolderDocuments);
            }
        }

        void TryDeleteDirectories(List<string> dirs)
        {
            dirs.ForEach(dir =>
            {
                try
                {
                    System.IO.Directory.Delete(dir, true);
                }
                catch
                { }
            });
        }

        void TryCopyDirectory(string sourceDir, string destDir)
        {
            TryDeleteDirectories(new List<string> { destDir });
            try
            {
                Utilities.DirectoryHelper.DirectoryCopy(sourceDir, destDir, copySubDirs: true, showReasonOnError: true, overwrite: true);
            }
            catch
            { }
        }

        public void UninstallAsService()
        {
            String error = null;
            if (owner != null)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = owner,
                    ProgressBarIndeterminate = true,
                    DialogText = String.Format(Properties.Resources.ServiceUninstalling, cmdOptions.ServiceName),
                    IsCancellingEnabled = false
                };

                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    try
                    {
                        InternalStopService();
                        ServiceInstaller.ServiceInstaller.Uninstall(cmdOptions.ServiceName);
                        if (cmdOptions.DeleteFolders != null)
                            TryDeleteDirectories(cmdOptions.DeleteFolders.Split(';').ToList());
                    }
                    catch (Exception ex)
                    {
                        error = String.Format(Properties.Resources.ServiceUninstallFailed, ex.Message);
                    }
                });
            }
            else
            {
                try
                {
                    InternalStopService();
                    ServiceInstaller.ServiceInstaller.Uninstall(cmdOptions.ServiceName);
                    if (cmdOptions.DeleteFolders != null)
                        TryDeleteDirectories(cmdOptions.DeleteFolders.Split(';').ToList());
                }
                catch (Exception ex)
                {
                    error = String.Format(Properties.Resources.ServiceUninstallFailed, ex.Message);
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void StartService()
        {
            String error = null;
            if (owner != null)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = owner,
                    ProgressBarIndeterminate = true,
                    DialogText = String.Format(Properties.Resources.ServiceStarting, cmdOptions.ServiceName),
                    IsCancellingEnabled = false
                };

                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    try
                    {
                        //ServiceInstaller.ServiceInstaller.StartService(cmdOptions.ServiceName);
                        InternalStartService();
                    }
                    catch (Exception ex)
                    {
                        error = String.Format(Properties.Resources.ServiceStartFailed, ex.Message);
                    }
                });
            }
            else
            {
                try
                {
                    //ServiceInstaller.ServiceInstaller.StartService(cmdOptions.ServiceName);
                    InternalStartService();
                }
                catch (Exception ex)
                {
                    error = String.Format(Properties.Resources.ServiceStartFailed, ex.Message);
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void StopService()
        {
            String error = null;
            if (owner != null)
            {
                ProgressDialog dlg = new ProgressDialog()
                {
                    AutoShowDelay = 0,
                    Owner = owner,
                    ProgressBarIndeterminate = true,
                    DialogText = String.Format(Properties.Resources.ServiceStopping, cmdOptions.ServiceName),
                    IsCancellingEnabled = false
                };

                dlg.RunWorkerThread(null, (o, ev) =>
                {
                    try
                    {
                        //ServiceInstaller.ServiceInstaller.StopService(cmdOptions.ServiceName);
                        InternalStopService();
                    }
                    catch (Exception ex)
                    {
                        error = String.Format(Properties.Resources.ServiceStopFailed, ex.Message);
                    }
                });
            }
            else
            {
                try
                {
                    //ServiceInstaller.ServiceInstaller.StopService(cmdOptions.ServiceName);
                    InternalStopService();
                }
                catch (Exception ex)
                {
                    error = String.Format(Properties.Resources.ServiceStopFailed, ex.Message);
                }
            }

            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Private Methods
        void AddHttpsCertificate(string username, string password)
        {
            using (var td = TaskService.Instance.NewTask())
            {
                var companyAttribute = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
                td.RegistrationInfo.Author = companyAttribute != null ? companyAttribute.Company : String.Empty;
                td.RegistrationInfo.Description = "'dotnet dev-certs https' task";

                td.Actions.Add("dotnet", "dev-certs https");
                td.Settings.Hidden = true;

                td.Triggers.AddNew(TaskTriggerType.Registration);

                var taskName = Properties.Settings.Default.AddHttpsCertificateTaskName;
                if (TaskService.Instance.RootFolder.Tasks.Exists(taskName))
                {
                    int i = 0;
                    var newTaskName = String.Format("{0} {1}", taskName, ++i);
                    while (TaskService.Instance.RootFolder.Tasks.Exists(newTaskName))
                        newTaskName = String.Format("{0} {1}", taskName, ++i);
                    taskName = newTaskName;
                }

                var task = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td,
                    TaskCreation.Create,
                    String.IsNullOrEmpty(username) ? null : username,
                    String.IsNullOrEmpty(password) ? null : password,
                    String.IsNullOrEmpty(password) ? TaskLogonType.ServiceAccount : TaskLogonType.Password);

                TaskService.Instance.RootFolder.DeleteTask(taskName);
            }
        }

        void InternalStartService()
        {
            using (var serviceControl = new System.ServiceProcess.ServiceController(cmdOptions.ServiceName))
            {
                if (serviceControl.Status == System.ServiceProcess.ServiceControllerStatus.Running ||
                    serviceControl.Status == System.ServiceProcess.ServiceControllerStatus.StartPending)
                    return;

                if (serviceControl.ServicesDependedOn != null)
                {
                    var dependencyService = new StringBuilder();
                    foreach (var service in serviceControl.ServicesDependedOn)
                    {
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running ||
                            service.Status == System.ServiceProcess.ServiceControllerStatus.StartPending)
                            continue;

                        if (dependencyService.Length > 0)
                            dependencyService.Append(Environment.NewLine);
                        dependencyService.Append(service.DisplayName);
                    }

                    if (dependencyService.Length > 0)
                    {
                        var message = String.Format(Properties.Resources.AskStartDependencyServices.Replace("'newline'", Environment.NewLine),
                            serviceControl.DisplayName, dependencyService);
                        if (MessageBox.Show(message, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                                return;
                    }
                }

                serviceControl.Start();
            }
        }

        void InternalStopService()
        {
            using (var serviceControl = new System.ServiceProcess.ServiceController(cmdOptions.ServiceName))
            {
                if (serviceControl.Status == System.ServiceProcess.ServiceControllerStatus.Stopped ||
                    serviceControl.Status == System.ServiceProcess.ServiceControllerStatus.StopPending)
                    return;

                if (serviceControl.DependentServices != null)
                {
                    var dependencyService = new StringBuilder();
                    foreach (var service in serviceControl.DependentServices)
                    {
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped ||
                            service.Status == System.ServiceProcess.ServiceControllerStatus.StopPending)
                            continue;

                        if (dependencyService.Length > 0)
                            dependencyService.Append(Environment.NewLine);
                        dependencyService.Append(service.DisplayName);
                    }

                    if (dependencyService.Length > 0)
                    {
                        var message = String.Format(Properties.Resources.AskStopDependencyServices.Replace("'newline'", Environment.NewLine),
                            serviceControl.DisplayName, dependencyService);
                        if (MessageBox.Show(message, cmdOptions.Title ?? Properties.Resources.AppTitle, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            return;
                    }

                }

                serviceControl.Stop();
            }
        }
        #endregion
    }
}
