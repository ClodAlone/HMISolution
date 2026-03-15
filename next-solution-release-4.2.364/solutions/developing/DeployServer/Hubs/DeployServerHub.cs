using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DeployServer.Processes;
using DeployServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DeployServer.Hubs
{
    [Authorize]
    public class DeployServerHub : Hub
    {
        static Timer cpuloadTimer;
        static Object lockObject = new object();
        static int sessionNumber = 0;

        static public String pathSourceDeploy;
        static public String WebHMIProxyPath;
        static public String BrowserProcessName;
        static public String BrowserArguments;
        static public bool WaitForWebHMIInitialization;

        IHubContext<DeployServerHub> _hubContext = null;

        public DeployServerHub(IHubContext<DeployServerHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override Task OnConnectedAsync()
        {
            Debug.WriteLine("New connection : " + Context.ConnectionId);

            lock (lockObject)
            {
                ++sessionNumber;
                if (cpuloadTimer == null)
                {
                    cpuloadTimer = new Timer(TimerCallback, null, 2000, 2000);
                }
            }
            return base.OnConnectedAsync();
        }

        async void TimerCallback(object o)
        {
            var ret = await CpuProcess.GetCpuUsageForCurrentProcess();
            try
            {
                await _hubContext.Clients.All.SendAsync("cputotalload", ret);
            }
            catch { }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Debug.WriteLine("Disconnection : " + Context.ConnectionId);

            lock (lockObject)
            {
                --sessionNumber;
                if (sessionNumber == 0)
                {
                    cpuloadTimer.Dispose();
                    cpuloadTimer = null;
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<String> FileVersion(String path)
        {
            String ret = String.Empty;
            await Task.Run(() =>
            {
                string filename = (pathSourceDeploy + path).Replace('/', Path.DirectorySeparatorChar);
                var version = FileVersionInfo.GetVersionInfo(filename);
                ret = version.FileVersion;
            });

            return ret;
        }

        [Obsolete]
        public async Task<bool> FileExist(String path, string licenseFileName = null)
        {
            return await FileExist2(path, licenseFileName, null);
        }

        public async Task<bool> FileExist2(String path, string licenseFileName = null, Tuple<string, string> licenseFolders = null)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(licenseFolders);
                    ret = FileExistInner(path, licenseFileName);
                }
            });

            return ret;
        }

        bool FileExistInner(String path, string licenseFileName = null)
        {
            string filename;
            if (!String.IsNullOrEmpty(licenseFileName))
                filename = (String.Format("{0}{1}{2}", Path.GetDirectoryName(MSZ.MSZView.FilePath), Path.DirectorySeparatorChar, licenseFileName)).Replace('/', Path.DirectorySeparatorChar);
            else
                filename = (pathSourceDeploy + path).Replace('/', Path.DirectorySeparatorChar);
            return File.Exists(filename);
        }

        public async Task<bool> DirectoryExist(String path)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                string filename = (pathSourceDeploy + path).Replace('/', Path.DirectorySeparatorChar);
                ret = Directory.Exists(filename);
            });

            return ret;
        }

        [Obsolete]
        public async Task DeleteFile(String path, bool bIsLicense = false)
        {
            Tuple<string, string> licenseFolders = null;
            if (bIsLicense)
            {
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(null);
                    DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(MSZ.MSZView.FilePath));
                    licenseFolders = new Tuple<string, string>(di.Parent.Name, di.Name);
                }
            }

            await DeleteFile2(path, licenseFolders);
        }

        public async Task DeleteFile2(String path, Tuple<string, string> licenseFolders = null)
        {
            await Task.Run(() =>
            {
                string deployPath;
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(licenseFolders);
                    deployPath = licenseFolders != null ? String.Format("{0}{1}", Path.GetDirectoryName(MSZ.MSZView.FilePath), Path.DirectorySeparatorChar) : pathSourceDeploy;
                }

                string filename = (deployPath + path).Replace('/', Path.DirectorySeparatorChar);
                if (File.Exists(filename))
                    File.Delete(filename);
            });
        }

        public async Task DeleteDirectory(String path)
        {
            await Task.Run(() =>
            {
                string filename = (pathSourceDeploy + path).Replace('/', Path.DirectorySeparatorChar);
                if (Directory.Exists(filename))
                    Directory.Delete(filename, true);
            });
        }

        public async Task MoveDirectory(String sourcepath, String destpath)
        {
            await Task.Run(() =>
            {
                string sourceFullPath = (pathSourceDeploy + sourcepath).Replace('/', Path.DirectorySeparatorChar);
                if (Directory.Exists(sourceFullPath))
                {
                    string destFullPath = (pathSourceDeploy + destpath).Replace('/', Path.DirectorySeparatorChar);
                    MoveOverwriteDirectory(sourceFullPath, destFullPath);
                }
            });
        }

        public async Task<bool> CopyDirectory(String sourcepath, String destpath)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                string sourceFullPath = (pathSourceDeploy + sourcepath).Replace('/', Path.DirectorySeparatorChar);
                if (Directory.Exists(sourceFullPath))
                {
                    string destFullPath = (pathSourceDeploy + destpath).Replace('/', Path.DirectorySeparatorChar);
                    CopyDirectoryTo(sourceFullPath, destFullPath);
                    ret = true;
                }
            });
            return ret;
        }

        public async Task<bool> CopyFiles(String sourcepath, String destpath, string extensionsPattern)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                string sourceFullPath = (pathSourceDeploy + sourcepath).Replace('/', Path.DirectorySeparatorChar);
                if (Directory.Exists(sourceFullPath))
                {
                    string destFullPath = (pathSourceDeploy + destpath).Replace('/', Path.DirectorySeparatorChar);
                    CopyDirectoryTo(sourceFullPath, destFullPath, extensionsPattern);
                    ret = true;
                }
            });
            return ret;
        }

        public async Task<bool> CopyRetentiveData(String sourcepath, String destpath, List<String> retentiveFolders)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                string sourceFullPath = (pathSourceDeploy + sourcepath).Replace('/', Path.DirectorySeparatorChar);
                if (Directory.Exists(sourceFullPath))
                {
                    string destFullPath = (pathSourceDeploy + destpath).Replace('/', Path.DirectorySeparatorChar);
                    for (var i = 0; i < retentiveFolders.Count; i++)
                        retentiveFolders[i] = (pathSourceDeploy + retentiveFolders[i]).Replace('/', Path.DirectorySeparatorChar);
                    Utilities.IO.FileSystem.CopyTo(retentiveFolders, destFullPath);
                    ret = true;
                }
            });
            return ret;
        }

        public static void CopyDirectoryTo(string source, string target, string extensionsPattern = null)
        {
            var sourcePath = source.TrimEnd(Path.DirectorySeparatorChar, ' ');
            var targetPath = target.TrimEnd(Path.DirectorySeparatorChar, ' ');
            IEnumerable<IGrouping<string, string>> files;
            if (extensionsPattern == null)
                files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));
            else {
                var reSearchPattern = new Regex(extensionsPattern, RegexOptions.IgnoreCase);
                files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .Where(file => reSearchPattern.IsMatch(Path.GetExtension(file)))
                                 .GroupBy(s => Path.GetDirectoryName(s));
            }
            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Copy(file, targetFile);
                }
            }
        }

        public static void MoveOverwriteDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd(Path.DirectorySeparatorChar, ' ');
            var targetPath = target.TrimEnd(Path.DirectorySeparatorChar, ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));
            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            Directory.Delete(source, true);
        }

        public async Task<string> GetWinConnextMoviconPath(string processName, string subKey, string keyName, string versionKeyName)
        {
            string applicationPath = null;
            await Task.Run(() =>
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey))
                {
                    var keysFound = (from string k in key.GetSubKeyNames() where k.Substring(0, keyName.Length) == keyName select k).ToList();
                    Version greatestVersion = null;
                    foreach (var k in keysFound)
                    {
                        using (var sk = key.OpenSubKey(k))
                        {
                            var kpath = String.Format("{0}{1}", sk.GetValue("Path")?.ToString(), processName);
                            try
                            {
                                var kversion = new Version(sk.GetValue(versionKeyName)?.ToString());
                                if (File.Exists(kpath) && (greatestVersion == null || kversion.CompareTo(greatestVersion) > 0))
                                {
                                    greatestVersion = kversion;
                                    applicationPath = kpath;
                                }
                            }
                            catch { }
                        }
                    }
                }
            });
            return applicationPath;
        }

        [Obsolete]
        public async Task UploadFileChunk(String path, byte[] bytes, bool bIsLicense = false)
        {
            Tuple<string, string> licenseFolders = null;
            if (bIsLicense)
            {
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(null);
                    DirectoryInfo di = new DirectoryInfo(System.IO.Path.GetDirectoryName(MSZ.MSZView.FilePath));
                    licenseFolders = new Tuple<string, string>(di.Parent.Name, di.Name);
                }
            }

            await UploadFileChunk2(path, bytes, licenseFolders);
        }

        public async Task OnFileUploadEnd(string path)
        {
            await Task.Run(() => {
                string filePath = (pathSourceDeploy + path).Replace('/', Path.DirectorySeparatorChar);
                try
                {
                    ZipFile.ExtractToDirectory(filePath, Path.GetDirectoryName(filePath), true);
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {

                }
            });
        }

        public async Task UploadFileChunk2(String path, byte[] bytes, Tuple<string, string> licenseFolders)
        {
            await Task.Run(() =>
            {
                string deployPath;
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(licenseFolders);
                    deployPath = licenseFolders != null ? String.Format("{0}{1}", Path.GetDirectoryName(MSZ.MSZView.FilePath), Path.DirectorySeparatorChar) : pathSourceDeploy;
                }

                string filename = (deployPath + path).Replace('/', Path.DirectorySeparatorChar);

                var dir = Path.GetDirectoryName(filename);
                Directory.CreateDirectory(dir);

                using (var stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Seek(0, SeekOrigin.End);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                    stream.Close();
                }
            });
        }

        public async Task<bool> StartBrowser(String name, String filename, string commandline, string documentTitle, string serverWebHMIRootFolder)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                filename = filename.Replace(PlaceHolders.DeployRootPathPlaceholder, pathSourceDeploy);

                if (!String.IsNullOrEmpty(BrowserProcessName))
                {
                    filename = BrowserProcessName;
                    if (WaitForWebHMIInitialization) {
                        var loadingPageURL = "file://" + Uri.EscapeUriString((pathSourceDeploy + serverWebHMIRootFolder).Replace(Path.DirectorySeparatorChar, '/')) + "/wwwroot/scripts/static/loadingPage.html?webhmi={0}";
                        if (String.IsNullOrEmpty(BrowserArguments))
                            BrowserArguments = loadingPageURL;
                        else
                            commandline = String.Format(loadingPageURL, commandline);
                    }
                    if (!String.IsNullOrEmpty(BrowserArguments))
                        commandline = String.Format(BrowserArguments, commandline);
                }

                commandline = commandline.Replace(PlaceHolders.DeployRootPathPlaceholder, pathSourceDeploy);
                commandline = commandline.Replace(PlaceHolders.DeployWebHMIProxyPathPlaceholder, WebHMIProxyPath ?? String.Empty);
                //if (bReplaceSeparator)
                    commandline = commandline.Replace('/', Path.DirectorySeparatorChar);
                Console.WriteLine(String.Format("Adding process:{0}Name: {1}{0}Filename: {2}{0}ProjectTitle: {3}{0}CommandLineArguments: {4}", Environment.NewLine, name, filename, documentTitle, commandline));
                ret = ProcessInfo.AddProcess(name, filename, commandline, false, documentTitle);
            });

            return ret;
        }

        public async Task<bool> AddProcess(String name, String filename, string commandline, bool autorestart, string documentTitle, bool bReplaceSeparator = true)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                filename = filename.Replace(PlaceHolders.DeployRootPathPlaceholder, pathSourceDeploy);
                commandline = commandline.Replace(PlaceHolders.DeployRootPathPlaceholder, pathSourceDeploy);
                commandline = commandline.Replace(PlaceHolders.DeployWebHMIProxyPathPlaceholder, WebHMIProxyPath ?? String.Empty);
                if (bReplaceSeparator)
                    commandline = commandline.Replace('/', Path.DirectorySeparatorChar);
                Console.WriteLine(String.Format("Adding process:{0}Name: {1}{0}Filename: {2}{0}ProjectTitle: {3}{0}CommandLineArguments: {4}", Environment.NewLine, name, filename, documentTitle, commandline));
                ret = ProcessInfo.AddProcess(name, filename, commandline, autorestart, documentTitle);
            });

            return ret;
        }

        public async Task<bool> RemoveProcess(String name, string documentTitle, bool bForceKill = false)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                ret = ProcessInfo.RemoveProcess(name, documentTitle, bForceKill);
            });

            return ret;
        }

        public async Task<bool> RemoveProcessesExcept(String documentTitle)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                ret = ProcessInfo.RemoveProcessesExcept(documentTitle);
            });

            return ret;
        }

        public async Task<List<ProcessData>> GetProcessesData()
        {
            List<ProcessData> ret = null;
            await Task.Run(() =>
            {
                ret = ProcessInfo.GetProcessesData();
            });

            return ret;
        }

        public async Task<RuntimeInfo> GetPlatformDescription()
        {
            var deployServerVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            return new RuntimeInfo(deployServerVersion);
        }

        [Obsolete]
        public async Task<MSZ.LicenseDataModel> LoadLicenseData(Dictionary<string, string> optionsTags)
        {
            return await LoadLicenseData2(optionsTags, null);
        }

        public async Task<MSZ.LicenseDataModel> LoadLicenseData2(Dictionary<string, string> optionsTags, Tuple<string, string> licenseFolders)
        {
            MSZ.LicenseDataModel licenseDataModel = null;
            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    MSZ.MSZView.SetLicensePath(licenseFolders);
                    licenseDataModel = new MSZ.LicenseDataModel() { OptionsTags = optionsTags };
                    licenseDataModel.LoadData();
                }
            });

            return licenseDataModel;
        }
    }
}
