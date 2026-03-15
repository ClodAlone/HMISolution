using System;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Data.Common;
using System.Data;
using System.IO;

namespace UFUAVirtualServiceManager
{
    class VirtualServiceMng
    {
        internal void CreateVirtualUser(CommandLineOptions cl)
        {
            ContextType ctx = ContextType.Domain;
            var cuser = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
            var domain = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
            if (string.IsNullOrEmpty(domain))
            {
                ctx = ContextType.Machine;
                domain = System.Environment.MachineName;
            }
            var cryptedPassword = WPFUtilities.CryptString.CryptString.DecryptString("ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */);

            bool isRedirected;

            try
            {
                isRedirected = Console.CursorVisible && false;
            }
            catch
            {
                isRedirected = true;
            }
            using (PrincipalContext context = new PrincipalContext(ctx, domain))
            {
                bool present = (Principal.FindByIdentity(context, IdentityType.SamAccountName, cuser) != null);
                try
                {
                    if (present && !context.ValidateCredentials(cuser, cryptedPassword))
                    {
#if DEBUG
                        using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                            s.WriteLine($"{DateTime.Now.ToString()} ValidateCredentials: User already present with different credentials!!");

#endif
                        if (!isRedirected)
                            Console.Error.WriteLine("ValidateCredentials: User already present with different credentials!!");
                        return;
                    }

                }
                catch (Exception ex)
                {
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                        s.WriteLine($"{DateTime.Now.ToString()} ValidateCredentials  Message: {ex.Message}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine($"ValidateCredentials  Message: {ex.Message}");
                }


                if (!present)
                    using (UserPrincipal newuser = new UserPrincipal(context, cuser, cryptedPassword.ToString(), true))
                    {
                        newuser.DisplayName = cuser;
                        newuser.PasswordNeverExpires = true;
                        newuser.UserCannotChangePassword = true;
                        newuser.Save();
                    }

                try
                {
                    UFUAServiceLibrary.Helpers.LsaUtility.SetRight(UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting(), "SeServiceLogonRight");
                }
                catch (Exception ex)
                {
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                        s.WriteLine($"{DateTime.Now.ToString()} SeServiceLogonRight  Message: {ex.Message}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine("SeServiceLogonRight  Message: {0}", ex.Message);
                }

                try
                {

                    string dataProvider = "System.Data.SqlClient";
                    string connectionString = $"Data Source={cl.Server};Initial Catalog=master;Integrated Security=true";
                    if (!cl.Trusted)
                        connectionString = $"Data Source={cl.Server};Initial Catalog=master;User Id={cl.User};Password={cl.Password}";

                    if (!String.IsNullOrEmpty(dataProvider) && !String.IsNullOrEmpty(connectionString))
                    {
                        DbConnection dbConnection = null;
                        DbCommand dbCommand = null;
                        DbTransaction dbTransaction = null;

                        try
                        {
                            dbConnection = DataReader.DataReader.CreateDbConnection(dataProvider, connectionString);
                            if (dbConnection.State == ConnectionState.Broken)
                                dbConnection.Close();
                            if (dbConnection.State == ConnectionState.Closed)
                                dbConnection.Open();

                            dbTransaction = dbConnection.BeginTransaction();
                            dbCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                            dbCommand.CommandType = CommandType.Text;
                            dbCommand.Connection = dbConnection;
                            dbCommand.Transaction = dbTransaction;

                            StringBuilder command = new StringBuilder("USE [master]");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"IF DATABASE_PRINCIPAL_ID('{domain}\\{cuser}') IS NULL");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"BEGIN");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"   USE [master] CREATE USER[{domain}\\{cuser}] FOR LOGIN [{domain}\\{cuser}];");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"   EXEC sp_addsrvrolemember @loginame= N'{domain}\\{cuser}', @rolename = N'sysadmin';");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"END");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"ELSE");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"BEGIN");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"   EXEC sp_addsrvrolemember @loginame= N'{domain}\\{cuser}', @rolename = N'sysadmin';");
                            command.Append($"{Environment.NewLine}");
                            command.Append($"END");
                            dbCommand.CommandText = command.ToString();
                            dbCommand.ExecuteNonQuery();
                            dbTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                                s.WriteLine($"{DateTime.Now.ToString()} ExecuteNonQuery Exception Type: {ex.Message}");
#endif
                            if (!isRedirected)
                            {
                                Console.WriteLine("ExecuteNonQuery Exception Type: {0}", ex.GetType());
                                Console.WriteLine("  Message: {0}", ex.Message);
                            }
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            catch (Exception ex2)
                            {
#if DEBUG
                                using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                                    s.WriteLine($"{DateTime.Now.ToString()} Rollback Exception Type: {ex2.Message}");
#endif
                                // This catch block will handle any errors that may have occurred
                                // on the server that would cause the rollback to fail, such as
                                // a closed connection.
                                if (!isRedirected)
                                {
                                    Console.Error.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                                    Console.Error.WriteLine("  Message: {0}", ex2.Message);
                                }
                            }
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
#if DEBUG
                    using (StreamWriter s = File.AppendText("c:\\temp\\setup.log"))
                        s.WriteLine($"{DateTime.Now.ToString()} Main connection  Message: {exm.Message}");
#endif
                    if (!isRedirected)
                        Console.Error.WriteLine("Main connection  Message: {0}", exm.Message);
                }
            };
        }
    }
}
