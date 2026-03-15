using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WixToolset.Dtf.WindowsInstaller;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using System.Data.Common;
using System.Data;
using System.Runtime.InteropServices;

namespace CustomActions
{
    public class CustomActions
    {
        static string SetupType { get; set; }
        static string UserType { get; set; }
        static string CFR21UserName { get; set; }
        static string CFR21DomainName { get; set; }
        static string CFR21DomainUser { get; set; }
        static string CFR21DomainPassword { get; set; }
        static string SQLServer { get; set; }
        static string SQLConnectionType { get; set; }
        static string SQLPassword { get; set; }
        static string SQLUser { get; set; }
        static string Result { get; set; }
        static string ErrorMessage { get; set; }

        [CustomAction]
        public static ActionResult Initialize(Session session)
        {
            if (session == null)
                return ActionResult.Failure;
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                session["CFR21_ERRORMESSAGE"] = "DEBUG operation denied";
                session["CFR21_VALIDATE_OK"] = "0";
                return ActionResult.Failure;
            }
#endif
#if DEBUG
            session.Log("Start Initialize");
#endif
            InitSessionValues(session);

            if (Initialize() != ActionResult.Success)
            {
#if DEBUG
                session.Log("Initialize error: " + ErrorMessage);
                session.Log("End Initialize");
#endif
                session["CFR21_ERRORMESSAGE"] = ErrorMessage;
                session["CFR21_VALIDATE_OK"] = "0";
                return ActionResult.Failure;
            }

#if DEBUG
            session.Log("End Initialize");
#endif
            session["CFR21_VALIDATE_OK"] = "1";
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult Finalize(Session session)
        {
            if (session == null)
                return ActionResult.Failure;
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                session["CFR21_ERRORMESSAGE"] = "DEBUG operation denied";
                session["CFR21_VALIDATE_OK"] = "0";
                return ActionResult.Failure;
            }
#endif


#if DEBUG
                session.Log("Start Finalize");
#endif
            InitSessionValues(session);

            if (Finalize() != ActionResult.Success)
            {
#if DEBUG
                session.Log("Finalize error: " + ErrorMessage);
                session.Log("End Finalize");
#endif
                session["CFR21_ERRORMESSAGE"] = ErrorMessage;
                session["CFR21_VALIDATE_OK"] = "0";
                return ActionResult.Failure;
            }

#if DEBUG
            session.Log("End Finalize");
#endif
            session["CFR21_VALIDATE_OK"] = "1";
            return ActionResult.Success;
        }

        private static void InitSessionValues(Session session)
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ErrorMessage = Properties.Resources.OperationDenied;
                Result = "0";
                return;
            }
#endif

            SetupType = session["CFR21SETUPPROP"];
            UserType = session["CFR21DOMAINPROP"];
            CFR21UserName = session["CFR21_USER"];
            CFR21DomainName = session["CFR21_DOMAINNAME"]; ;
            CFR21DomainUser = session["CFR21_IMPERSONATION_USER"];
            CFR21DomainPassword = session["CFR21_IMPERSONATION_PASSWORD"];
            SQLConnectionType = session["SQLEXIST_CONTYPE"];
            SQLPassword = session["PASSWORD_PROP"];
            SQLUser = session["USERNAME_PROP"];
            SQLServer = session["SQLSERVER_PROP"];
            Result = "0";
            ErrorMessage = "";
        }

#if DEBUG
        public static void InitSessionValues(string setupType, string userType, string domain, 
            string impUser, string impPassword, string connectionType, string sqlServer, 
            string sqlUser, string sqlPassword)
#endif
#if !DEBUG
        private static void InitSessionValues(string setupType, string userType, string domain,
            string impUser, string impPassword, string connectionType, string sqlServer,
            string sqlUser, string sqlPassword)
#endif
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ErrorMessage = Properties.Resources.OperationDenied;
                Result = "0";
                return;
            }
#endif
            SetupType = setupType;
            UserType = userType;
            CFR21UserName = "NEXT_IO_User";
            CFR21DomainName = domain;
            CFR21DomainUser = impUser;
            CFR21DomainPassword = impPassword;
            SQLConnectionType = connectionType;
            SQLPassword = sqlPassword;
            SQLUser = sqlUser;
            SQLServer = sqlServer;
            Result = "0";
            ErrorMessage = "";
        }

#if DEBUG
       public static string GetLastMessage()
        {
            return ErrorMessage;
        }
#endif

#if DEBUG
        public static ActionResult Initialize()
#endif
#if !DEBUG
        private static ActionResult Initialize()
#endif
        {
#if DEBUG
            // display message to allow attaching the debugger
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    String.Format("DebugMe - RunAs"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ErrorMessage = Properties.Resources.OperationDenied;
                Result = "0";
                return ActionResult.Failure;
            }
#endif
            var cioserveruser = CFR21UserName;

            bool present = false;
            if (SetupType == "CUSTOM" && UserType == "REMOTE")
            {
                try
                {
                    ContextType ctx = ContextType.Domain;
                    var cuser = CFR21DomainUser;
                    var domain = CFR21DomainName;
                    if (string.IsNullOrEmpty(domain))
                    {
                        ctx = ContextType.Machine;
                        domain = System.Environment.MachineName;
                        using (PrincipalContext context = new PrincipalContext(ctx, domain))
                        {
                            present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cioserveruser) != null);
                        }
                    }
                    else
                    {
                        using (PrincipalContext context = new PrincipalContext(ctx, domain, cuser, CFR21DomainPassword))
                        {
                            present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cioserveruser) != null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(Properties.Resources.InizializationFailure, ex.Message);
                    Result = "0";

                    return ActionResult.Failure;
                }
            }
            else
            {
                try
                {
                    ContextType ctx = ContextType.Machine;
                    var domain = System.Environment.MachineName;
                    using (PrincipalContext context = new PrincipalContext(ctx, domain))
                    {
                        present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cioserveruser) != null);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(Properties.Resources.InizializationFailure, ex.Message);
                    Result = "0";


                    return ActionResult.Failure;
                }
            }

            return ActionResult.Success;
        }

#if DEBUG
        public static ActionResult Finalize()
#endif
#if !DEBUG
        private static ActionResult Finalize()
#endif
        {
#if DEBUG
            // display message to allow attaching the debugger
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    String.Format("DebugMe - Finalize"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ErrorMessage = Properties.Resources.OperationDenied;
                Result = "0";
                return ActionResult.Failure;
            }
#endif

            var cioserveruser = CFR21UserName;
            var domain = CFR21DomainName;
            if (SetupType == "CUSTOM" && UserType == "REMOTE")
            {
                try
                {
                    ContextType ctx = ContextType.Domain;
                    if (string.IsNullOrEmpty(domain))
                    {
                        ctx = ContextType.Machine;
                        domain = System.Environment.MachineName;
                        using (PrincipalContext context = new PrincipalContext(ctx, domain))
                        {
                            if (Finalize(context) != ActionResult.Success)
                                return ActionResult.Failure;
                        }
                    }
                    else
                    {
                        using (PrincipalContext context = new PrincipalContext(ctx, domain, CFR21DomainUser, CFR21DomainPassword))
                        {
                            if (Finalize(context) != ActionResult.Success)
                                return ActionResult.Failure;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(Properties.Resources.FinalizeFailure, ex.Message);
                    Result = "0";
                    return ActionResult.Failure;
                }
            }
            else
            {
                try
                {
                    ContextType ctx = ContextType.Machine;
                    domain = System.Environment.MachineName;
                    using (PrincipalContext context = new PrincipalContext(ctx, domain))
                    {
                        if (Finalize(context) != ActionResult.Success)
                            return ActionResult.Failure;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(Properties.Resources.FinalizeFailure, ex.Message);
                    Result = "0";
                    return ActionResult.Failure;
                }
            }

            try
            {
                string dataProvider = "System.Data.SqlClient";
                string connectionString = $"Data Source={SQLServer};Initial Catalog=master;Integrated Security=true";
                if (SQLConnectionType != "TrustedConnection")
                    connectionString = $"Data Source={SQLServer};Initial Catalog=master;User Id={SQLUser};Password={SQLPassword}";

                if (!String.IsNullOrEmpty(dataProvider) && !String.IsNullOrEmpty(connectionString))
                {
                    DbConnection dbConnection = null;
                    DbCommand dbCommand = null;
                    DbTransaction dbTransaction = null;

                    try
                    {
                        dbConnection = LsaUtility.CreateDbConnection(dataProvider, connectionString);
                        if (dbConnection.State == ConnectionState.Broken)
                            dbConnection.Close();
                        if (dbConnection.State == ConnectionState.Closed)
                            dbConnection.Open();

                        dbTransaction = dbConnection.BeginTransaction();
                        dbCommand = LsaUtility.CreateDbCommand(dataProvider);
                        dbCommand.CommandType = CommandType.Text;
                        dbCommand.Connection = dbConnection;
                        dbCommand.Transaction = dbTransaction;

                        StringBuilder command = new StringBuilder("USE [master]");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"declare @lname nvarchar(255)");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"select DISTINCT @lname = name from sys.server_principals where name like '%\\{CFR21UserName}'");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"IF @lname IS NULL");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"BEGIN");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"   USE [master] CREATE USER[{domain}\\{CFR21UserName}] FOR LOGIN [{domain}\\{CFR21UserName}];");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"   EXEC sp_addsrvrolemember @loginame= N'{domain}\\{CFR21UserName}', @rolename = N'sysadmin';");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"END");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"ELSE");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"BEGIN");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"   EXEC sp_addsrvrolemember @loginame= @lname, @rolename = N'sysadmin';");
                        command.Append($"{Environment.NewLine}");
                        command.Append($"END");
                        dbCommand.CommandText = command.ToString();
                        dbCommand.ExecuteNonQuery();
                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = string.Format(Properties.Resources.DBTransactionFailure, ex.Message);
                        Result = "0";

                        try
                        {
                            if (dbTransaction != null)
                                dbTransaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                        }

                        return ActionResult.Failure;
                    }
                    finally
                    {
                        if (dbTransaction != null)
                            dbTransaction.Dispose();
                        dbTransaction = null;

                        if (dbConnection != null)
                            dbConnection.Close();
                        dbConnection = null;

                        if (dbCommand != null)
                            dbCommand.Dispose();
                        dbCommand = null;
                    }
                }
            }
            catch (Exception exm)
            {
                ErrorMessage = string.Format(Properties.Resources.DBTransactionFailure, exm.Message);
                Result = "0";
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public static bool TryLogOnUser(string user, string password, string domain)
        {
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_INTERACTIVE = 2;
            bool loggedOn = false;
            try
            {
                if (domain == "")
                    domain = System.Environment.MachineName;

                loggedOn = LogonUser(user,
                                            domain,
                                            password,
                                            LOGON32_LOGON_INTERACTIVE,
                                            LOGON32_PROVIDER_DEFAULT,
                                            ref userHandle);
            }
            catch (Exception ex)
            {
                loggedOn = false;
            }
            finally
            {
                if (userHandle != IntPtr.Zero)
                {
                    CloseHandle(userHandle);
                }
            }

            return loggedOn;
        }

        private static ActionResult Finalize(PrincipalContext context)
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ErrorMessage = Properties.Resources.OperationDenied;
                Result = "0";
                return ActionResult.Failure;
            }
#endif

            var sid = WPFUtilities.CryptString.CryptString.DecryptString("ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */);
            var cfr21User = CFR21UserName;
            bool present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cfr21User) != null);

            if (present)
            {
                try
                {
                    if (!context.ValidateCredentials(cfr21User, sid))
                    {
                        ErrorMessage = Properties.Resources.UserAlreadyPresent_CreationFailure;
                        Result = "0";
                        return ActionResult.Failure;
                    }
                }
                catch
                {
                    try
                    {
                        if (!TryLogOnUser(cfr21User, sid, context.Name))
                        {
                            ErrorMessage = Properties.Resources.UserAlreadyPresent_CreationFailure;
                            Result = "0";
                            return ActionResult.Failure;
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = string.Format(Properties.Resources.UserValidationFailure, ex.Message);
                        Result = "0";
                        return ActionResult.Failure;
                    }
                }
            }
            else
            {
                try
                {
                    using (UserPrincipal newuser = new UserPrincipal(context, cfr21User, sid, true))
                    {
                        newuser.DisplayName = cfr21User;
                        newuser.PasswordNeverExpires = true;
                        newuser.UserCannotChangePassword = true;
                        newuser.Save();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = string.Format(Properties.Resources.UserCreationFailure, ex.Message);
                    Result = "0";
                    return ActionResult.Failure;
                }
            }

            try
            {
                LsaUtility.SetRight(cfr21User, "SeServiceLogonRight");
            }
            catch (Exception ex)
            {
                ErrorMessage = string.Format(Properties.Resources.UserGrantFailure, ex.Message);
                Result = "0";
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        private static string GetUserName(string userName)
        {
            var user = userName;
            if (user.IndexOf(".\\") != -1)
                return user.Substring(user.IndexOf(".\\") + 2);
            else if (user.IndexOf("\\") != -1)
                return user.Split('\\')[1];
            else if (user.IndexOf('@') != -1)
                return user.Split('@')[0];
            else
                return user;
        }

        private static string GetDomainName(string userName)
        {
            var domainName = userName;
            if (domainName.IndexOf(".\\") != -1)
                return null;
            else if (domainName.IndexOf("\\") != -1)
                return domainName.Split('\\')[0];
            else if (domainName.IndexOf('@') != -1)
                return domainName.Split('@')[1];
            else
                return null;
        }
    }
}
