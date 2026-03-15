using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VFS;
using System.IO;
using System.Timers;
using System.Xml.Linq;

namespace STRL
{
    public static class STRL
    {
        public class ControlSettings
        {
            public double? Width { get; set; }
            public double? Height { get; set; }
        }
        static Dictionary<string, ControlSettings> _settings;
        public static Dictionary<string, ControlSettings> Settings
        {
            get
            {
                if (_settings == null)
                    UpdateControlSettingList();
                return _settings;
            }
        }

        public readonly static string[] preChangeStyleFolders = new string[] { "Styles", "Styles4" };
        public readonly static string projectSymbolFolder = "Symbols";
        public readonly static string fileSystemStyleFolder = "Styles";
        public readonly static string styleFolder = "Styles42";

        const String settingsExt = ".settings";
        const String codeExt = ".code";
        const String bamlExt = ".baml";
        public readonly static char[] delimiters = new char[] { '@', '?' };
        const String xamlSignature = "xmlns=";
        readonly static byte[] bamlSignature = new byte[] { 0xc0, 0x00, 0x00, 0x00, 0x4d, 0x00, 0x53, 0x00, 0x42, 0x00, 0x41, 0x00, 0x4d, 0x00, 0x4c };
        const String xmlSignature = "<?xml version=";

        internal static String projectTag = "<project>";

        readonly static Dictionary<String, String> tempCacheElement = new Dictionary<String, String>();
        readonly static Dictionary<String, byte[]> tempCacheElementData = new Dictionary<String, byte[]>();
        readonly static Dictionary<String, DateTime> tempCacheElementDateTime = new Dictionary<String, DateTime>();
        readonly static Object lockObject = new Object();

        static public TimeSpan MaxAge = new TimeSpan(0, 5, 0);
        public static string UpdateStyleFolder(this string dependencyObject, bool clear = false)
        {
            string value = dependencyObject;
            preChangeStyleFolders.ToList().ForEach(x =>
            {
                if (dependencyObject.IndexOf($"\\{x}\\") != -1)
                {
                    if (clear)
                        dependencyObject = dependencyObject.Replace($"\\{x}\\", "\\");
                    else
                        dependencyObject = dependencyObject.Replace($"\\{x}\\", $"\\{styleFolder}\\");
                    return;
                }
            });
            if (clear)
                dependencyObject = dependencyObject.Replace($"\\{styleFolder}\\", "\\");
            return dependencyObject;
        }

        public static String GetSymbolElement(String sourceSymbolProvider, String sourceSymbolPath, String relativePath, bool bUploading = false)
        {
            var element = GetDataFromRepository(sourceSymbolProvider, sourceSymbolPath, null, relativePath, bUploading);
            var key = GetSymbolStyleKey(sourceSymbolProvider, sourceSymbolPath);
            if(!string.IsNullOrEmpty(key) && Settings.ContainsKey(key))
            {
                element = ReplaceAttributes(element, Settings[key]);
            }
            return element;
        }

        private static void UpdateControlSettingList()
        {
            _settings = new Dictionary<string, ControlSettings>();
            string startingPath = String.Format("{0}\\{1}\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), styleFolder);
            string filePath = string.Format("{0}\\ControlSettings.xml", startingPath);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    XDocument xml = XDocument.Load(filePath);
                    var _visibleHMIDescendants = xml.Root.Descendants("Settings").FirstOrDefault();
                    foreach (XElement node in _visibleHMIDescendants.Descendants("Name").ToList())
                    {
                        if (node.HasAttributes)
                        {
                            var key = node.Attribute("ID")?.Value;
                            if (!String.IsNullOrEmpty(key) && !_settings.ContainsKey(key))
                            {
                                ControlSettings controlSettings = new ControlSettings();
                                if (node.HasAttributes)
                                {
                                    double width;
                                    double height;
                                    if (double.TryParse(node.Attribute("Width")?.Value, System.Globalization.NumberStyles.AllowDecimalPoint, 
                                        System.Globalization.CultureInfo.InvariantCulture, out width))
                                        controlSettings.Width = width;
                                    if (double.TryParse(node.Attribute("Height")?.Value, System.Globalization.NumberStyles.AllowDecimalPoint,
                                        System.Globalization.CultureInfo.InvariantCulture, out height))
                                        controlSettings.Height = height;
                                }
                                _settings.Add(key, controlSettings);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static string ReplaceAttributes(string content, ControlSettings settings)
        {
            if (!string.IsNullOrEmpty(content) && settings != null)
            {
                try
                {
                    XDocument xml = XDocument.Parse(content);
                    XElement node = xml.Root;
                   if (node != null)
                    {
                        if(settings.Width != null)
                            node.SetAttributeValue("Width", settings.Width);
                        if(settings.Height!= null)
                            node.SetAttributeValue("Height", settings.Height);
                        return xml.ToString();
                    }
                }
                catch (Exception)
                {
                }
            }
            return content;
        }

        public static String GetSymbolStyle(String sourceSymbolProvider, String sourceSymbolPath, bool bUploading = false)
        {
            sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolPath);

            String fileCode;
            string[] parts = sourceSymbolPath.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
                return String.Empty;

            sourceSymbolPath = parts[1];
            if (sourceSymbolPath.Contains("\\CheckBoxControl\\") && !parts[0].Contains("CheckBoxControl"))
                sourceSymbolPath = sourceSymbolPath.Replace("\\CheckBoxControl\\", "\\CheckBox\\");
            else if (sourceSymbolPath.Contains("\\ButtonControl\\") && !parts[0].Contains("ButtonControl"))
                sourceSymbolPath = sourceSymbolPath.Replace("\\ButtonControl\\", "\\Button\\");

            if (bUploading)
                sourceSymbolPath = sourceSymbolPath.Replace("\\Styles", "\\StylesIoT");
            fileCode = sourceSymbolPath;

            var ret = GetFileDataFromRepository(sourceSymbolProvider, fileCode, bUploading);
            if (String.IsNullOrEmpty(ret))
            {
                fileCode = fileCode.UpdateStyleFolder();
                ret = GetFileDataFromRepository(sourceSymbolProvider, fileCode, bUploading);
            }

            return ret;
        }

        public static byte[] GetSymbolElementData(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return GetDataFromRepositoryData(sourceSymbolProvider, sourceSymbolPath, null, relativePath);
        }

        public static byte[] GetSymbolStyleData(String sourceSymbolProvider, String sourceSymbolPath)
        {
            sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolPath);

            String fileCode;
            string[] parts = sourceSymbolPath.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 1)
                return null;

            var cacheElement = String.Format("{0}{1}", sourceSymbolProvider ?? String.Empty,
                                                          sourceSymbolPath ?? String.Empty);
            lock (lockObject)
            {
                if (tempCacheElementData.ContainsKey(cacheElement))
                {
                    tempCacheElementDateTime.Remove(cacheElement);
                    tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);
                    return tempCacheElementData[cacheElement];
                }
            }

            sourceSymbolPath = parts[1];
            if (sourceSymbolPath.Contains("\\CheckBoxControl\\") && !parts[0].Contains("CheckBoxControl"))
                sourceSymbolPath = sourceSymbolPath.Replace("\\CheckBoxControl\\", "\\CheckBox\\");
            else if (sourceSymbolPath.Contains("\\ButtonControl\\") && !parts[0].Contains("ButtonControl"))
                sourceSymbolPath = sourceSymbolPath.Replace("\\ButtonControl\\", "\\Button\\");
            fileCode = sourceSymbolPath;

            var ret = GetFileDataFromRepositoryData(sourceSymbolProvider, fileCode);
            if (ret==null)
            {
                fileCode = fileCode.UpdateStyleFolder();
                ret = GetFileDataFromRepositoryData(sourceSymbolProvider, fileCode);
            }
            lock (lockObject)
            {
                if (tempCacheElementData.ContainsKey(cacheElement))
                    tempCacheElementData.Remove(cacheElement);
                if (tempCacheElementDateTime.ContainsKey(cacheElement))
                    tempCacheElementDateTime.Remove(cacheElement);

                tempCacheElementData.Add(cacheElement, ret);
                tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);

                StartDelayTimer();
            }

            return ret;
        }        

        public static String GetSymbolStyleKey(String sourceSymbolProvider, String sourceSymbolPath)
        {
            sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolPath);

            string[] parts = sourceSymbolPath.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 2)
                return String.Empty;

            return parts[2];
        }

        public static String GetSymbolSettings(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return GetDataFromRepository(sourceSymbolProvider, sourceSymbolPath, settingsExt, relativePath);
        }

        public static String GetSymbolCode(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return GetDataFromRepository(sourceSymbolProvider, sourceSymbolPath, codeExt, relativePath, bNoCache:true);
        }

        static String GetDataFromRepository(String sourceSymbolProvider, String sourceSymbolPath, 
                            String ext = null, String relativePath = null, bool bUploading = false, bool bNoCache = false)
        {
            var cacheElement = String.Format("{0}{1}{2}", sourceSymbolProvider ?? String.Empty, 
                                                          sourceSymbolPath ?? String.Empty, 
                                                          ext ?? String.Empty);
            if (!bUploading && !bNoCache)
            {
                lock (lockObject)
                {
                    if (tempCacheElement.ContainsKey(cacheElement))
                    {
                        tempCacheElementDateTime.Remove(cacheElement);
                        tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);
                        return tempCacheElement[cacheElement];
                    }
                }
            }

            sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolPath);
            if (!String.IsNullOrEmpty(relativePath))
                sourceSymbolPath = sourceSymbolPath.Replace(projectTag, String.Format("{0}\\Symbols", relativePath));
            if (bUploading)
            {
                sourceSymbolPath = sourceSymbolPath.Replace("\\Symbols", "\\SymbolsIoT");
                sourceSymbolPath = sourceSymbolPath.Replace("\\Styles", "\\StylesIoT");
            }

            String fileCode;
            string[] parts = sourceSymbolPath.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            sourceSymbolPath = parts[0];

            if (!String.IsNullOrEmpty(ext))
                fileCode = sourceSymbolPath + ext;
            else
                fileCode = sourceSymbolPath;

            var ret = GetFileDataFromRepository(sourceSymbolProvider, fileCode, bUploading, bNoCache);
            if (String.IsNullOrEmpty(ret) && fileCode.IndexOf("\\Symbols\\") != -1)
            {
                fileCode = fileCode.Replace("\\Symbols\\", "\\Symbols4\\");
                ret = GetFileDataFromRepository(sourceSymbolProvider, fileCode, bUploading, bNoCache);
            }
            else if (String.IsNullOrEmpty(ret))
            {
                fileCode = fileCode.UpdateStyleFolder();
                ret = GetFileDataFromRepository(sourceSymbolProvider, fileCode, bUploading, bNoCache);
            }
            if (!bUploading && !bNoCache)
            {
                lock (lockObject)
                {
                    if (tempCacheElement.ContainsKey(cacheElement))
                        tempCacheElement.Remove(cacheElement);
                    if (tempCacheElementDateTime.ContainsKey(cacheElement))
                        tempCacheElementDateTime.Remove(cacheElement);

                    tempCacheElement.Add(cacheElement, ret);
                    tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);

                    StartDelayTimer();
                }
            }

            return ret;
        }

        static byte[] GetDataFromRepositoryData(String sourceSymbolProvider, String sourceSymbolPath,
                            String ext = null, String relativePath = null)
        {
            var cacheElement = String.Format("{0}{1}{2}", sourceSymbolProvider ?? String.Empty,
                                                          sourceSymbolPath ?? String.Empty,
                                                          ext ?? String.Empty);
            lock (lockObject)
            {
                if (tempCacheElementData.ContainsKey(cacheElement))
                {
                    tempCacheElementDateTime.Remove(cacheElement);
                    tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);
                    return tempCacheElementData[cacheElement];
                }
            }

            sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolPath);
            if (!String.IsNullOrEmpty(relativePath))
                sourceSymbolPath = sourceSymbolPath.Replace(projectTag, String.Format("{0}\\Symbols", relativePath));

            String fileCode;
            string[] parts = sourceSymbolPath.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            sourceSymbolPath = parts[0];

            if (!String.IsNullOrEmpty(ext))
                fileCode = sourceSymbolPath + ext;
            else
                fileCode = sourceSymbolPath;

            var ret = GetFileDataFromRepositoryData(sourceSymbolProvider, fileCode); 
            if (ret == null)
            {
                fileCode = fileCode.UpdateStyleFolder();
                ret = GetFileDataFromRepositoryData(sourceSymbolProvider, fileCode);
            }
            if (ret == null)
                return null;

            lock (lockObject)
            {
                if (tempCacheElementData.ContainsKey(cacheElement))
                    tempCacheElementData.Remove(cacheElement);
                if (tempCacheElementDateTime.ContainsKey(cacheElement))
                    tempCacheElementDateTime.Remove(cacheElement);

                tempCacheElementData.Add(cacheElement, ret);
                tempCacheElementDateTime.Add(cacheElement, DateTime.UtcNow);

                StartDelayTimer();
            }

            return ret;
        }

        private static String GetFileDataFromRepository(String sourceSymbolProvider, String fileCode, bool bUploading = false, bool bNoCache = false)
        {
            if (!bUploading && !bNoCache)
            {
                lock (lockObject)
                {
                    if (tempCacheElement.ContainsKey(fileCode))
                    {
                        tempCacheElementDateTime.Remove(fileCode);
                        tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);
                        return tempCacheElement[fileCode];
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(sourceSymbolProvider))
            {
                sourceSymbolProvider = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolProvider);
                using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = sourceSymbolProvider })
                {
                    var file = new FileManagerFile(fileSystemProvider, fileCode);
                    if (fileSystemProvider.Exists(file))
                    {
                        var data = fileSystemProvider.ReadFile(file);
                        var ret = Encoding.Unicode.GetString(data);

                        var bException = false;

                        if (!ret.Contains(xamlSignature) && !ret.Contains(xmlSignature))
                        {
                            try
                            {
                                ret = WPFUtilities.CryptString.CryptString.DecryptString(ret);
                            }
                            catch (Exception ex)
                            {
                                bException = true;
                            }
                        }

                        if(bException)
                        {
                            ret = Encoding.UTF8.GetString(data);
                            if (!ret.Contains(xamlSignature) && !ret.Contains(xmlSignature))
                            {
                                ret = WPFUtilities.CryptString.CryptString.DecryptString(ret);
                            }
                        }                                                  

                        if (!bUploading && !bNoCache)
                        {
                            lock (lockObject)
                            {
                                if (tempCacheElement.ContainsKey(fileCode))
                                    tempCacheElement.Remove(fileCode);
                                if (tempCacheElementDateTime.ContainsKey(fileCode))
                                    tempCacheElementDateTime.Remove(fileCode);

                                tempCacheElement.Add(fileCode, ret);
                                tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);

                                StartDelayTimer();
                            }
                        }

                        return ret;
                    }
                }
            }
            else
            {
                if (File.Exists(fileCode))
                {
                    var ret = File.ReadAllText(fileCode);
                    try
                    {
                        if (!ret.Contains(xamlSignature))
                            ret = WPFUtilities.CryptString.CryptString.DecryptString(ret);
                    }
                    catch { }

                    if (!bUploading && !bNoCache)
                    {
                        lock (lockObject)
                        {
                            if (tempCacheElement.ContainsKey(fileCode))
                                tempCacheElement.Remove(fileCode);
                            if (tempCacheElementDateTime.ContainsKey(fileCode))
                                tempCacheElementDateTime.Remove(fileCode);

                            tempCacheElement.Add(fileCode, ret);
                            tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);

                            StartDelayTimer();
                        }
                    }
                    return ret;
                }
            }

            return String.Empty;
        }

        private static byte[] GetFileDataFromRepositoryData(String sourceSymbolProvider, String fileCode)
        {
            lock (lockObject)
            {
                if (tempCacheElementData.ContainsKey(fileCode))
                {
                    tempCacheElementDateTime.Remove(fileCode);
                    tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);
                    return tempCacheElementData[fileCode];
                }
            }

            var fileext = System.IO.Path.ChangeExtension(fileCode, bamlExt);

            if (!String.IsNullOrWhiteSpace(sourceSymbolProvider))
            {
                sourceSymbolProvider = WPFUtilities.CryptString.CryptString.DecryptString(sourceSymbolProvider);
                using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = sourceSymbolProvider })
                {
                    var file = new FileManagerFile(fileSystemProvider, fileext);
                    if (fileSystemProvider.Exists(file))
                    {
                        var ret = fileSystemProvider.ReadFile(file);
                        lock (lockObject)
                        {
                            if (tempCacheElementData.ContainsKey(fileCode))
                                tempCacheElementData.Remove(fileCode);
                            if (tempCacheElementDateTime.ContainsKey(fileCode))
                                tempCacheElementDateTime.Remove(fileCode);

                            tempCacheElementData.Add(fileCode, ret);
                            tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);

                            StartDelayTimer();
                        }

                        return ret;
                    }
                }
            }
            else
            {
                if (File.Exists(fileext))
                {
                    var ret = File.ReadAllBytes(fileext);
                    if (ret != null && ret.Length >= bamlSignature.Length)
                    {
                        var bDecrypt = false;
                        for (int i = 4; i < bamlSignature.Length; ++i)
                        {
                            if (ret[i] != bamlSignature[i])
                            {
                                bDecrypt = true;
                                break;
                            }
                        }
                        if (bDecrypt)
                            ret = WPFUtilities.CryptString.CryptString.DecryptData(ret);
                    }

                    lock (lockObject)
                    {
                        if (tempCacheElementData.ContainsKey(fileCode))
                            tempCacheElementData.Remove(fileCode);
                        if (tempCacheElementDateTime.ContainsKey(fileCode))
                            tempCacheElementDateTime.Remove(fileCode);

                        tempCacheElementData.Add(fileCode, ret);
                        tempCacheElementDateTime.Add(fileCode, DateTime.UtcNow);

                        StartDelayTimer();
                    }

                    return ret;
                }
            }

            return null;
        }

        static public void ForceUnload()
        {
            lock (lockObject)
            {
                tempCacheElementDateTime.Clear();
                tempCacheElement.Clear();
                tempCacheElementData.Clear();
            }
        }

        static Timer timer;
        static void StartDelayTimer()
        {
            if (timer != null)
                return;

            timer = new Timer(10000);
            timer.Start();
            timer.Elapsed += (o, e) =>
                {
                    var priority = System.Threading.Thread.CurrentThread.Priority;
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                    try
                    {
                        lock (lockObject)
                        {
                            var currUtc = DateTime.UtcNow;
                            var remove = (from c in tempCacheElementDateTime
                                          where tempCacheElementDateTime[c.Key] + MaxAge < currUtc
                                          select c.Key).ToList();
                            remove.ForEach(key =>
                                {
                                    tempCacheElementDateTime.Remove(key);
                                    if (tempCacheElement.ContainsKey(key))
                                        tempCacheElement.Remove(key);
                                    if (tempCacheElementData.ContainsKey(key))
                                        tempCacheElementData.Remove(key);
                                });
                        }
                    }
                    finally
                    {
                        System.Threading.Thread.CurrentThread.Priority = priority;
                    }
                };
        }
    }
}
