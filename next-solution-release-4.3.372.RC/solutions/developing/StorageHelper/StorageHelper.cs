using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VFS;
using log4net;
using Utilities;
using UFProjectManager.ComponentService;
using DevExpress.Data;
using UFUserEditor.ComponentService;

namespace StorageHelper
{
    public class StorageHelper
    {
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.StorageHelper);
        public static string GetStorageName(IDocument document, string defaultName, string username, bool useParent = false)
        {
            if (document != null)
            {
                var file = document.FilePath;
                file = System.IO.Path.GetFileNameWithoutExtension(file);
                var name = string.Empty;
                name = $"{file}_{defaultName}";
                if (!String.IsNullOrEmpty(username))
                    name = String.Format("{0}_{1}", name, username);

                if (useParent && document.Parent != null)
                    name = $"{document.Parent.Title}_{name}{Properties.Settings.Default.OldStorageFileExtension}";
                else
                    name = $"{name}{Properties.Settings.Default.StorageFileExtension}";

                return name;
            }
            return defaultName;
        }

        public static void ReplaceDefaultSettings<T>(IDocument document, string name,string settingpropertyname, string settingname) where T:class
        {
            string path;
            string subFolder;
            string filepath;
           T memories = null;
            object toremove = null;

            if (document != null && !string.IsNullOrEmpty(name))
            {
                path = document.GetSpecialFolder(SpecialFolders.Documents).OriginalString;
                subFolder = Path.GetDirectoryName(document.MakeRelativeUri(new Uri(document.FilePath, UriKind.RelativeOrAbsolute)).OriginalString);
                filepath = Path.Combine(path, subFolder);
                if (Directory.Exists(filepath))
                { 
                    foreach (string filename in Directory.EnumerateFiles(filepath, $"{document.Title}_{name}**{Properties.Settings.Default.StorageFileExtension}"))
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                            memories = ReadProjectDataFromStream<T>(fs);

                        if (memories is System.Collections.IList list)
                        {
                            foreach (var item in list)
                            {
                                if (item.GetType().GetProperty(settingpropertyname)?.GetValue(item).ToString() != settingname)
                                    continue;

                                toremove = item;
                                break;
                            }

                            if (toremove != null)
                                list.Remove(toremove);
                        }

                        using (var outstream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite))
                            WriteProjectDataStream(outstream, memories);
                    }
            }
          }
        }
        public static Dictionary<string, T> LoadMemoryMaps<T>(IDocument document, string defaultName) where T : class
        {
            var ret = (Dictionary<string, T>)Activator.CreateInstance(typeof(Dictionary<string, T>));
            try
            {
                if (document == null || string.IsNullOrEmpty(defaultName))
                    return ret;

                var settingsFilePrefix = $"{Path.GetFileNameWithoutExtension(document.FilePath)}_{defaultName}";
                string subFolder = Path.GetDirectoryName(document.MakeRelativeUri(new Uri(document.FilePath, UriKind.RelativeOrAbsolute)).OriginalString);
                var diPath = Path.Combine(document.GetSpecialFolder(SpecialFolders.Documents).OriginalString, subFolder);
                var di = new DirectoryInfo(diPath);
                var settingsFullFilePrefix = $"{di.FullName}{Path.DirectorySeparatorChar}{settingsFilePrefix}";

                try
                {
                    using (var fileStream = new FileStream(Path.Combine(diPath, $"{settingsFilePrefix}{Properties.Settings.Default.StorageFileExtension}"), FileMode.Open, FileAccess.Read))
                    {
                        ret.Add("", ReadProjectDataFromStream<T>(fileStream));
                    }
                }
                catch { }

                var userManager = document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                var usernames = userManager.GetListUserNames(document, true);
                if (usernames != null)
                {
                    foreach (var userName in usernames)
                    {
                        var fileName = $"{settingsFilePrefix}_{userName}{Properties.Settings.Default.StorageFileExtension}";
                        var fullFileName = Path.Combine(diPath, fileName);
                        try
                        {
                            using (var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
                            {
                                ret.Add(userName, ReadProjectDataFromStream<T>(fileStream));
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return ret;
        }
        public static T LoadMemoryMap<T>(IDocument document, string defaultName, string username) where T : class
        {
            try
            {
                bool needToSave = false;
                string storagename = GetStorageName(document, defaultName, username);
                if (document == null || string.IsNullOrEmpty(storagename))
                    return (T)Activator.CreateInstance(typeof(T));

                string path = document.GetSpecialFolder(SpecialFolders.Documents).OriginalString;
                string subFolder = System.IO.Path.GetDirectoryName(document.MakeRelativeUri(new Uri(document.FilePath, UriKind.RelativeOrAbsolute)).OriginalString);
                string file = System.IO.Path.Combine(path,subFolder,storagename);

                if (document.fileSystemProviderBase != null)
                {
                    subFolder = subFolder.Remove(0, document.rootBaseDB.Length);
                    file = System.IO.Path.Combine($"{path}{subFolder}", storagename);

                    byte[] data;

                    if (!document.fileSystemProviderBase.Exists(new FileManagerFile(document.fileSystemProviderBase, file)))
                    {
                        needToSave = true;
                        file = System.IO.Path.Combine(path, storagename);

                        if (!document.fileSystemProviderBase.Exists(new FileManagerFile(document.fileSystemProviderBase, file)))
                        {
                            needToSave = true;
                            storagename = GetStorageName(document, defaultName, username, true);
                            file = System.IO.Path.Combine(path, storagename);

                            if (!document.fileSystemProviderBase.Exists(new FileManagerFile(document.fileSystemProviderBase, file)))
                                return (T)Activator.CreateInstance(typeof(T));
                        }
                    }

                    data = document.fileSystemProviderBase.ReadFile(new FileManagerFile(document.fileSystemProviderBase, file));
                    using (MemoryStream reader = new MemoryStream(data))
                    {
                        T ret = ReadProjectDataFromStream<T>(reader);
                        if (needToSave)
                            SaveMemoryMap<T>(ret, document, defaultName, username);
                        return ret;
                    }
                }
                else
                {
                    if (!File.Exists(file))
                    {
                        needToSave = true;
                        file = System.IO.Path.Combine(path, storagename);

                        if (!File.Exists(file))
                        {
                            needToSave = true;
                            storagename = GetStorageName(document, defaultName, username, true);
                            file = System.IO.Path.Combine(path, storagename);
                            if (String.IsNullOrEmpty(file) || !File.Exists(file))
                                return (T)Activator.CreateInstance(typeof(T));
                        }
                    }

                    //if (!Utilities.IO.FileSystem.IsXmlFile(file))
                    //{
                    //    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(file));
                    //    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    //    {
                    //        var ret = ReadProjectDataFromStream<T>(reader);
                    //        if (needToSave)
                    //            SaveMemoryMap<T>(ret, document, defaultName, username);
                    //        return ret;
                    //    }
                    //}
                    //else
                    {
                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            var ret = ReadProjectDataFromStream<T>(fileStream);
                            if (needToSave)
                                SaveMemoryMap<T>(ret, document, defaultName, username);
                            return ret;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IUFProjectManager iUFProjectManager = document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(document, Properties.Resources.StorageHelper,
                        DateTime.UtcNow, $"{Properties.Resources.LoadMemoryError}: {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                log.Error(Properties.Resources.LoadMemoryError, ex);
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
        public static bool SaveMemoryMap<T>(T memories, IDocument document, string defaultName, string username) where T : class
        {
            string storagename = GetStorageName(document, defaultName, username);
            if (document == null || string.IsNullOrEmpty(storagename) || memories == null)
                return false;
            string path = document.GetSpecialFolder(SpecialFolders.Documents).OriginalString;
            string subFolder = System.IO.Path.GetDirectoryName(document.MakeRelativeUri(new Uri(document.FilePath, UriKind.RelativeOrAbsolute)).OriginalString);
            string file = System.IO.Path.Combine(path, storagename);
            try
            {
                if (document.fileSystemProviderBase != null)
                {
                    subFolder = subFolder.Remove(0, document.rootBaseDB.Length);
                    path = $"{path}{subFolder}";
                    file = System.IO.Path.Combine(path, storagename);
                    document.fileSystemProviderBase.CreateFolder(null, path);
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream, memories))
                            return false;

                        document.fileSystemProviderBase.UploadFile(null, file, memoryStream.ToArray());
                        return true;
                    }
                }
                else
                {
                    path = System.IO.Path.Combine(path, subFolder);
                    Directory.CreateDirectory(path);
                    file = System.IO.Path.Combine(path, storagename);
                    using (var ostrm = File.Open(file, FileMode.Create, FileAccess.ReadWrite))
                    {
                        WriteProjectDataStream(ostrm, memories);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                IUFProjectManager iUFProjectManager = document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(document, Properties.Resources.StorageHelper,
                        DateTime.UtcNow, $"{Properties.Resources.SaveMemoryError}: {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                log.Error(Properties.Resources.SaveMemoryError, ex);
                return false;
            }
        }
        private static T ReadProjectDataFromStream<T>(Stream stream) where T : class
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                T ret = serializer.ReadObject(stream) as T;
                return ret;
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.LoadMemoryError, ex);
                return  (T)Activator.CreateInstance(typeof(T));
            }
        }
        private static bool WriteProjectDataStream<T>(Stream ostrm, T memories) where T : class
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.Unicode,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer, memories);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }
    }
    
    public class ConvertGridLayout : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender = null)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            IGridLayoutUser gridLayoutUser = sender as IGridLayoutUser;
            if (gridLayoutUser == null)
                return null;
            var gridLayout = value as string;
            List<object> res = new List<object>();
            List<StorageColumn> columns = gridLayoutUser.GetColumns();
            var columnlist = (from column in columns where column.Visible == true orderby column.VisibleIndex select column).ToList();
            columnlist.ForEach(c =>
            {
                res.Add(new Dictionary<string, object>() {
                            {"FieldName", c.FieldName },
                            {"ActualWidth", c.ActualWidth },
                            {"ColumnTag", c.ColumnTag},
                            {"ColumnOrder", c.ColumnOrder},
                            {"SortIndex", c.SortIndex },
                            {"ResourceKey", c.ResourceKey }
                });
            });

            return res;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }

    public class StorageColumn
    {
        public string FieldName { get; set; }
        public string ColumnTag { get; set; }
        public string ResourceKey { get; set; }
        public double ActualWidth { get; set; }
        public bool Visible { get; set; }
        public int VisibleIndex { get; set; }
        public ColumnSortOrder ColumnOrder { get; set; }
        public int SortIndex { get; set; }
    }
}
