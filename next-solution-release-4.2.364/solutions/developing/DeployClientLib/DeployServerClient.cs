using DeployServer.Processes;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Utilities;

namespace DeployClientLib
{
    public sealed class DeployServerClient: Observable, IDisposable
    {
        HubConnection connection;

        bool connected;
        public bool Connected
        {
            get { return connected; }
            set
            {
                Active = value;
                Set(ref connected, value, "Connected");
            }
        }
        bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                Set(ref active, value, "Active");
            }
        }

        public async Task<string> GetWinConnextMoviconPath(string connextProcessName, string conNextSubKey, string conNextKeyName, string versionKeyName)
        {
            return await connection.InvokeAsync<String>("GetWinConnextMoviconPath", connextProcessName, conNextSubKey, conNextKeyName, versionKeyName);
        }

        public async Task<BoolTaskResult> RemoveProcessesExcept(string documentTitle)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return ret;

            try
            {
                await connection.InvokeAsync("RemoveProcessesExcept", documentTitle);
                ret.Success = true;
            }
            catch (Exception e) {
                ret.SetException(e);
            }
            return ret;
        }

        public async Task<bool> InitiateConnection(String url, string user, string password, int reconnectTimeout,
            bool bAcceptAllCertificates = false)
        {
            if (connection != null)
                return false;

            Cookie authCookie;
            authCookie = await AuthenticateUser(url, user, password, bAcceptAllCertificates);
            if (authCookie == null)
                return false;

            if (bAcceptAllCertificates)
                ServicePointManager.ServerCertificateValidationCallback +=
                      (sender, certificate, chain, sslPolicyErrors) => true;

            var reconnectTimeouts = reconnectTimeout > 0 ? new List<int>() { reconnectTimeout, reconnectTimeout * 2, reconnectTimeout * 4 } : new List<int>() { 0, 2, 4 };

            connection = new HubConnectionBuilder()
                .WithUrl(String.Format("{0}/deployserverhub", url), options =>
                {
                    options.Cookies.Add(authCookie);
                })
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(reconnectTimeouts[0]), TimeSpan.FromSeconds(reconnectTimeouts[1]), TimeSpan.FromSeconds(reconnectTimeouts[2]) })
                .AddMessagePackProtocol()
                .Build();

            connection.Closed += async (error) =>
            {
                Connected = false;
            };
            connection.Reconnecting += error =>
            {
                // Debug.Assert(connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.
                Active = false;
                return Task.CompletedTask;
            };
            connection.Reconnected += error =>
            {
                // Debug.Assert(connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                Connected = true;
                return Task.CompletedTask;
            };

            try
            {
                await connection.StartAsync();
                Connected = true;
                return true;
            }
            catch(Exception ex)
            {

            }

            return false;
        }

        async Task<Cookie> AuthenticateUser(String url, string user, string password, bool bAcceptAllCertificates = false)
        {
            HttpClientHandler handler = new HttpClientHandler();
            CookieContainer cookies = new CookieContainer();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);

            handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            if (bAcceptAllCertificates)
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            var uri = new Uri(String.Format("{0}/Identity/Account/Login", url));
            string jsonInString = String.Format("Input.Email={0}&Input.Password={1}&Input.RememberMe=false", WebUtility.UrlEncode(user), WebUtility.UrlEncode(password));
            HttpResponseMessage response = await client.PostAsync(uri, new StringContent(jsonInString, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            var responseCookies = cookies.GetCookies(uri);
            return responseCookies[".AspNetCore.Identity.Application"];
        }

        public async Task<bool> CloseConnection()
        {
            if (connection == null)
                return false;

            await connection.StopAsync();
            await connection.DisposeAsync();
            connection = null;
            return true;
        }

        public async Task<String> FileVersion(String filePath)
        {
            if (!Active)
                return null;

            return await connection.InvokeAsync<String>("FileVersion", filePath);
        }

        public async Task<bool> FileExists(String filePath, string licenseFileName = null, Tuple<string, string> licenseFolders = null)
        {
            if (!Active)
                return false;

            return await connection.InvokeAsync<bool>("FileExist2", filePath, licenseFileName, licenseFolders);
        }

        public async Task<bool> DirectoryExist(String filePath)
        {
            if (!Active)
                return false;

            return await connection.InvokeAsync<bool>("DirectoryExist", filePath);
        }

        public async Task<bool> DeleteFile(String filePath)
        {
            if (!Active)
                return false;

            var bRet = false;
            try
            {
                await connection.InvokeAsync("DeleteFile2", filePath, null);
                bRet = true;
            }
            catch { }
            return bRet;
        }

        public async Task<bool> DeleteDirectory(String filePath)
        {
            if (!Active)
                return false;

            var bRet = false;
            try
            {
                await connection.InvokeAsync("DeleteDirectory", filePath);
                bRet = true;
            }
            catch { }
            return bRet;
        }

        public async Task<BoolTaskResult> CopyDirectory(String sourcepath, String destpath)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return ret;

            try
            {
                ret.ReturnValue = await connection.InvokeAsync<bool>("CopyDirectory", sourcepath, destpath);
                ret.Success = true;
            }
            catch (Exception e) {
                ret.SetException(e);
            }
            return ret;
        }

        public async Task<TaskResult> CopyRetentiveData(String sourcepath, String destpath, List<String> retentiveFolders)
        {
            var ret = new TaskResult();
            if (!Active)
                return ret;

            try
            {
                await connection.InvokeAsync<bool>("CopyRetentiveData", sourcepath, destpath, retentiveFolders);
                ret.Success = true;
            }
            catch (Exception e) { ret.Message = e.Message; }
            return ret;
        }

        public async Task<TaskResult> MoveDirectory(String sourcepath, String destpath)
        {
            var ret = new TaskResult();
            if (!Active)
                return ret;

            try
            {
                await connection.InvokeAsync("MoveDirectory", sourcepath, destpath);
                ret.Success = true;
            }
            catch (Exception e) { ret.Message = e.Message; }
            return ret;
        }

        public async Task<bool> CopyFiles(String sourcepath, String destpath, string extensionsPattern)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return false;

            try
            {
                await connection.InvokeAsync("CopyFiles", sourcepath, destpath, extensionsPattern);
            }
            catch (Exception ex) {
                ret.SetException(ex);
                if (!ret.SoftFail)
                    return false;
            }
            return true;
        }

        public async Task<bool> UploadLicense(String sourcePath, String licenseFileName, int maxSize, Tuple<string, string> licenseFolders)
        {
            if (!Active)
                return false;

            return await UploadFile(sourcePath, licenseFileName, maxSize, 0, null, null, licenseFolders);
        }

        public async Task<bool> UploadFile(String sourcePath, String filePath, int maxSize, long totalUploadedBytes = 0, IProgress<int> singleProgress = null, IProgress<long> totalProgress = null, Tuple<string, string> licenseFolders = null)
        {
            if (!Active)
                return false;

            long lastTotalProgress = 0;
            int lastSingleProgress = 0;
            using (var stream = File.OpenRead(sourcePath))
            {
                try
                {
                    if (!Active)
                        return false;
                    await connection.InvokeAsync("DeleteFile2", filePath, licenseFolders);
                }
                catch { }

                var bytes = new byte[maxSize];

                int offset = 0;
                int bytesread = 0;
                int chunksread = 0;
                do
                {
                    bytesread = stream.Read(bytes, offset, maxSize);
                    if (bytesread < bytes.Length)
                        Array.Resize(ref bytes, bytesread);
                    if (!Active)
                        return false;
                    await connection.InvokeAsync("UploadFileChunk2", filePath, bytes, licenseFolders);
                    var uploadedBytes = (long)(maxSize * chunksread + bytesread);
                    if (singleProgress != null && totalProgress != null && stream.Length > 0)
                    {
                        lastTotalProgress = totalUploadedBytes + uploadedBytes;
                        lastSingleProgress = (int)(uploadedBytes * 100 / stream.Length);
                        singleProgress.Report(lastSingleProgress);
                        totalProgress.Report(lastTotalProgress);
                    }
                    chunksread++;
                } while (bytesread == maxSize);
            }
            await connection.InvokeAsync("OnFileUploadEnd", filePath);
            if (singleProgress != null && totalProgress != null)
            {
                singleProgress.Report(lastSingleProgress + 1);
                totalProgress.Report(lastTotalProgress + 1);
            }

            return true;
        }

        public async Task<BoolTaskResult> StartBrowser(String name, String filename, string commandline, string documentTitle, string serverWebHMIRootFolder)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return ret;

            try
            {
                await connection.InvokeAsync("StartBrowser", name, filename, commandline, documentTitle, serverWebHMIRootFolder);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.SetException(ex);
                if (ret.SoftFail) {
                    try
                    {
                        await connection.InvokeAsync("StartBrowser", name, filename, commandline, documentTitle);
                        ret.Success = true;
                    }
                    catch (Exception ex2)
                    {
                        ret.SetException(ex2);
                    }
                }
            }

            return ret;
        }

        public async Task<bool> AddProcess(String name, String filename, string commandline, bool autorestart, string documentTitle, bool bReplaceSeparator = true)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return false;

            try
            {
                await connection.InvokeAsync("AddProcess", name, filename, commandline, autorestart, documentTitle, bReplaceSeparator);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.SetException(ex);
                if (!ret.SoftFail)
                    return false;
                else
                {
                    try
                    {
                        await connection.InvokeAsync("AddProcess", name, filename, commandline, autorestart, bReplaceSeparator);
                        ret.Success = true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> RemoveProces(String name, string documentTitle, bool bForceKill = false)
        {
            var ret = new BoolTaskResult();
            if (!Active)
                return false;
            try
            {
                await connection.InvokeAsync("RemoveProcess", name, documentTitle, bForceKill);
                ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.SetException(ex);
                if (!ret.SoftFail)
                    return false;
                else
                {
                    try
                    {
                        await connection.InvokeAsync("RemoveProcess", name, bForceKill);
                        ret.Success = true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<List<ProcessData>> GetProcessesData()
        {
            if (!Active)
                return null;

            return await connection.InvokeAsync<List<ProcessData>>("GetProcessesData");
        }

        public async Task<RuntimeInfo> GetPlatformDescription()
        {
            if (!Active)
                return null;

            return await connection.InvokeAsync<RuntimeInfo>("GetPlatformDescription");
        }

        public async Task<MSZ.LicenseDataModel> LoadLicenseData(Dictionary<string, string> OptionsTags, Tuple<string, string> licenseFolders)
        {
            if (!Active)
                return null;

            return await connection.InvokeAsync<MSZ.LicenseDataModel>("LoadLicenseData2", OptionsTags, licenseFolders);
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }

    public class TaskResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool SoftFail { get; set; }
        public TaskResult() { }
    }

    public class BoolTaskResult : TaskResult
    {
        public bool ReturnValue { get; set; }
        public void SetException(Exception e)
        {
            Message = e.Message;
            if ((e as Microsoft.AspNetCore.SignalR.HubException)?.HResult == -2146233088) //Missing method exception
                SoftFail = true;
        }
    }
}
