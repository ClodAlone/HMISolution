using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
#endif
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using StringManager.ComponentService;
using System.Windows;
using System.Xml;
using System.Dynamic;
using Utilities;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using log4net;
using System.Globalization;
using DevExpress.Xpo.DB.Helpers;
using StringManager.Interfaces;
using StringManager.Services;

namespace StringManager.Document
{
    public class StringEditorDocument : ViewModelBase,
#if !NET_STANDARD
        ICloneable, 
#endif
        IDocument
    {
#region Declarations

        UnitOfWork uow;
        private IStringEditorService _stringEditorService;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        InMemoryDataStore InMemory;
        IDataLayer dl;
        IDataLayer dlClipboard;
        String connectionString;
        String fileBase;

        Dictionary<string, Dictionary<string, string>> runtimestrings = new Dictionary<string, Dictionary<string, string>>();

        internal static String idText = "ID";

#if !NET_STANDARD
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif

#endregion

#region Methods

        void CreateDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(StringModel.UFStringLocale).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                //dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                dl = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);
            }
            else
            {
                InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                if (File.Exists(fileBase))
                {
                    if (Protected || !Utilities.IO.FileSystem.IsXmlFile(fileBase))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            var xmlreader = XmlReader.Create(reader);
                            try
                            {
                                InMemory.ReadXml(xmlreader);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else
                        try
                        {
                            InMemory.ReadXml(fileBase);
                        }
                        catch (Exception ex)
                        {

                        }
                }
                //dl = new SimpleDataLayer(InMemory);
                dl = new DevExpress.Xpo.ThreadSafeDataLayer(dict, InMemory);
            }

            uow = new UnitOfWork(dl);
            _stringEditorService = new StringEditorService(uow);
#if !NET_STANDARD
            uow.ObjectChanged += (o, e) =>
            {
                if (!e.Session.TrackingChanges)
                    return;
                NeedsSave = true;
            };

            // check if the address space need to reload because some treview item has been deleted
            uow.ObjectDeleting += (o, e) =>
            {
                if (!e.Session.TrackingChanges)
                    return;
                NeedsSave = true;
            };

            uow.ObjectsSaved += (o, e) =>
            {
                NeedsSave = false;
            };
#endif
            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);
        }

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        internal UnitOfWork GetSession()
        {
            return uow;
        }

#region WinClipboard
#if !NET_STANDARD
        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            using (var xml = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                {
                    InMemoryClipboard.WriteXml(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                }

                Clipboard.SetText(xml.ToString());
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData(bool force = false)
        {
            try
            {
                if (force || Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
                    Clipboard.GetText(TextDataFormat.UnicodeText) != LastClipboardUnicodeText)
                {
                    CleanClipbaord();
                    LastClipboardUnicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    using (var xml = new StringReader(Clipboard.GetText(TextDataFormat.UnicodeText)))
                    {
                        using (var xmlTextReader = new XmlTextReader(xml))
                        {
                            var tempds = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                            {
                                bool bValid = true;
                                try
                                {
                                    tempds.ReadXml(xmlTextReader);
                                }
                                catch
                                {
                                    bValid = false;
                                }

                                if (bValid)
                                {
                                    InMemoryClipboard.ReadFromInMemoryDataStore(tempds);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();

        }

        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, uow, checkattributes);
            return cloneHelper.Clone(obj, false);
        }

        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
        }

        internal void CopyListToClipbaord(List<StringModel.UFStringLocaleText> list)
        {

            list.ForEach(hs =>
            {
                hs.Culture = hs.UFStringLocale.Name;
                CloneToClipboard(hs, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsLocaleTexts()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return ((from c in new XPQuery<StringModel.UFStringLocaleText>(uowClipboard).AsParallel() select c).ToList().Count > 0 || Clipboard.ContainsText());
            }
            catch
            {
                return false;
            }
        }

        internal List<StringModel.UFStringLocaleText> PasteClipboardLocaleTexts()
        {
            var localList = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();

            var ret = new List<StringModel.UFStringLocaleText>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetListStringIDs();
            var listToCopy = (from c in new XPQuery<StringModel.UFStringLocaleText>(uowClipboard).AsParallel() orderby c.Text select c).ToList();
            string currID = string.Empty;
            string newID = string.Empty;
            if (listToCopy.Count > 0)
            {
                listToCopy.ForEach(hs =>
                {
                    if (currID != hs.Text)
                    {
                        newID = NewStringLocaleTextID(hs.Text, mapStartCounter, listName);
                        currID = hs.Text;
                    }
                    var newhs = CloneFromClipboard(hs, hs.Text != newID) as StringModel.UFStringLocaleText;
                    newhs.Text = newID;
                    newhs.UFStringLocale = localList.Find((o) => { return o.Name == hs.Culture; });
                    ret.Add(newhs);
                });
            }
            
            return ret;
        }

        internal List<StringModel.UFStringLocaleText> PasteListLocaleText(List<object> list, string[] mapname, bool update = true)
        {
            var ret = new List<StringModel.UFStringLocaleText>();
            var localList = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetListStringIDs();
            string currID = string.Empty;
            string newID = string.Empty;
            if (list.Count > 0)
            {
                var currlist = (from tag in new XPQuery<StringModel.UFStringLocaleText>(uow, true).AsParallel() where tag.UFStringLocale != null select tag).ToList();
                list.ForEach(hs =>
                {
                    var p = hs as string[];
                    if (p != null)
                    {
                        //var currlist = (from tag in new XPQuery<StringModel.UFStringLocaleText>(uow, true).AsParallel() where tag.Text == p[0] && tag.UFStringLocale != null select tag).ToList();
                        

                        for (int j = 1; (j < p.Length && j < mapname.Length); j++)
                        {
                            StringModel.UFStringLocaleText newhs = null;
                            if (update)
                            {
                                newID = p[0];
                                newhs = currlist.Find(o => { return (o.UFStringLocale.Name == mapname[j] && o.Text == p[0]); });
                                if(newhs != null)
                                    newhs.Locale = p[j];
                                else
                                    newhs = new StringModel.UFStringLocaleText(uow)
                                    {
                                        Text = newID,
                                        Locale = p[j],
                                        UFStringLocale = localList.Find((o) => { return o.Name == mapname[j]; })
                                    };
                            }
                            else
                            {
                                if (currID != p[0])
                                {
                                    newID = NewStringLocaleTextID(p[0], mapStartCounter, listName);
                                    currID = p[0];
                                }
                                newhs = new StringModel.UFStringLocaleText(uow)
                                {
                                    Text = newID,
                                    Locale = p[j],
                                    UFStringLocale = localList.Find((o) => { return o.Name == mapname[j]; })
                                };
                            }
                            ret.Add(newhs);
                        }
                        for (int k = p.Length; k < mapname.Length; k++)
                        {
                            ret.Add(new StringModel.UFStringLocaleText(uow)
                            {
                                Text = p[0],
                                Locale = string.Empty,
                                UFStringLocale = localList.Find((o) => { return o.Name == mapname[k]; })
                            });
                        }
                        for (int k = p.Length; k < mapname.Length; k++)
                        {
                            ret.Add(new StringModel.UFStringLocaleText(uow)
                            {
                                Text = p[0],
                                Locale = string.Empty,
                                UFStringLocale = localList.Find((o) => { return o.Name == mapname[k]; })
                            });
                        }
                    }
                });
            }
            return ret;
        }
        public CultureInfo AddLocale(string locale)
        {
            var listAvailableLanguages = new List<CultureInfo>();
            
            listAvailableLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            listAvailableLanguages.RemoveAll(culture => culture.IsNeutralCulture);
            if (listAvailableLanguages.Contains(CultureInfo.InvariantCulture))
                listAvailableLanguages.Remove(CultureInfo.InvariantCulture);
            
            var foundculture = listAvailableLanguages.Find(o => { return o.Name == locale; });
            if (foundculture != null)
            {
                if (GetListLocalCultures().ToList().Contains(foundculture.Name))
                    return null;
                var locobj = new StringModel.UFStringLocale(uow) { Locale = foundculture.Name, Name = foundculture.Name };

                var listData = GetListLocaleFlat();
                if(listData.Count > 0)
                    foreach (ExpandoObject data in listData)
                    {
                        var p = data as IDictionary<String, object>;
                        if (/*!p.ContainsKey(loc) || */!p.ContainsKey(idText))
                            continue;

                        var loctext = new StringModel.UFStringLocaleText(uow)
                        {
                            Text = p[idText] as String,
                            Culture = foundculture.Name
                        };
                        locobj.UFStringLocaleTexts.Add(loctext);
                    }

                editorManagerComponent.OnLocalesChanged(this);
                return foundculture;
            }
            return null;
        }
#endif
#endregion

        static String GetConnectionString(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
            String connString = null;
#if !NET_STANDARD
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
#endif
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        internal static String GetBaseFilename(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
#if !NET_STANDARD
            if (vfs != null)
                return null;
#endif
            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

#if !NET_STANDARD
        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, IDocumentManager manager = null)
        {
            using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
            {
                if (sourceDoc == null)
                    return;

                var targetConn = newPath;
                if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                    targetConn = GetConnectionString(newPath, null);
                using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uowTarget = new UnitOfWork(dlTarget))
                    {
                        var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, false, true, true);

                        ////////////////////////////////////////////////////////////////////////////
                        // delete all first
                        (from tag in new XPQuery<StringModel.UFStringLocale>(uowTarget, true).AsParallel()
                         select tag).ToList().ForEach(tag => tag.Delete());

                        // following is only for ensuring the complitely deletion of unassociated object elements
                        (from p in new XPQuery<StringModel.UFStringLocaleText>(uowTarget, true).AsParallel()
                         where p.UFStringLocale == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////

                        var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(sourceDoc.GetSession(), true)/*.AsParallel()*/ select tag).ToList();
                        foreach (var locale in locales)
                        {
                            cloneHelper.Clone(locale, false);
                        }

                        uowTarget.CommitChanges();
                        uowTarget.PurgeDeletedObjects();
                    }
                }
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
        }

        public static void RenameFile(String fullPath, String oldName, String newName,
            FileSystemProviderBase fileSystemProvider = null)
        {
        }

        public static void RemoveFile(string fullPath, IDocument parent, IDocumentManager manager = null)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            var file = GetBaseFilename(fullPath, fileSystemProvider);
            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
                {
                    if (sourceDoc == null)
                        return;
                    var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(sourceDoc.GetSession(), true)/*.AsParallel()*/ select tag).ToList();
                    foreach (var locale in locales)
                    {
                        locale.Delete();
                    }
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
#endif

        public static StringEditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
#endif
                String connString = GetConnectionString(path
#if !NET_STANDARD
                    , vfs
#endif
                    );
                String xmlfile = GetBaseFilename(path
#if !NET_STANDARD
                    , vfs
#endif
                    );

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                var doc = new StringEditorDocument(null)
                {
                    connectionString = connString,
#if !NET_STANDARD
                    EditorManagerComponent = c as StringEditorManagerComponent,
#endif
                    fileBase = xmlfile,
                    Parent = parent
                };

                doc.CreateDataLayer();
                if (!bCreateNew && bCheckEmpty && doc.IsEmpty)
                {
                    doc.Dispose();
                    return null;
                }
                else if (
#if !NET_STANDARD
                    vfs == null && 
#endif
                    !doc.IsBelongFromParent(parent))
                {
                    doc.Dispose();
#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
#else
                    logGeneral.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                    return null;
                }

                return doc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, path);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        internal void RemoveStringIdList(IList<String> list)
        {
            var items = new List<StringModel.UFStringLocaleText>();
            list.ToList().ForEach(s =>
            {
                var idtext = GetLocaleTextFromId(s);
                if (idtext.Count > 0)
                    items.AddRange(idtext);
            });

            foreach (var ltext in items)
            {
                var slocale = ltext.UFStringLocale;
                if (slocale != null)
                {
                    var idx = slocale.UFStringLocaleTexts.IndexOf(ltext);
                    if (idx != -1)
                        slocale.UFStringLocaleTexts[idx].Delete();
                }
                ltext.Delete();
            }

            items.Clear();
        }

        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return true;

            try
            {
                bool bSave = NeedsSave;
                if (discargechanges &&
                    uow.TryPurgeDeletedObjects(logGeneral) > 0)
                    bSave = true;

                if (!bForceSave && !bSave)
                    return true;

                //if (!discargechanges && ActiveView != null)
                //{
                //    (ActiveView as StringEditorControl).CommitChanges();
                //}

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (forceEncryption || Protected)
                        XpoHelpers.XpoHelper.AddProtectionCode(uow, Id);
                    else
                        XpoHelpers.XpoHelper.RemoveProtectionCode(uow);
                }

                uow.CommitChanges();

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (File.Exists(fileBase))
                    {
                        try
                        {
                            File.Delete(fileBase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (forceEncryption || Protected)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
#endif
#endregion

#region Properties

        private bool _IsAutoTranslate = true;
        [Browsable(false)]
        public bool IsAutoTranslate
        {
            get { return _IsAutoTranslate; }
            set
            {
                if (_IsAutoTranslate == value)
                    return;

                _IsAutoTranslate = value;
                OnPropertyChanged("IsAutoTranslate");
            }
        }

#if !NET_STANDARD
        bool bChanged;
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return uow != null && uow.TrackingChanges || bChanged; }
            set
            {
            	bChanged = value;
                OnPropertyChanged("NeedsSave");
            }
        }
               
        bool needToReloadAddressSpace;
        [Browsable(false)]
        public bool NeedToReloadAddressSpace
        {
            get
            {
                return needToReloadAddressSpace;
            }
            set
            {
                if (needToReloadAddressSpace == value)
                    return;

                needToReloadAddressSpace = value;
                OnPropertyChanged("NeedToReloadAddressSpace");
            }
        }
#endif

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

#if !NET_STANDARD
        StringEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public StringEditorManagerComponent EditorManagerComponent
        {
            get
            {
                return editorManagerComponent;
            }
            private set
            {
                editorManagerComponent = value;
            }
        }
#endif

        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
        }
#endregion

#region Locale Settings

        internal String MatchLanguage(String language)
        {
            var list = GetListLocalCultures();

            foreach (var l in list)
            {
                if (l.Contains(language))
                    return l;
            }

            return String.Empty;
        }

        private static readonly String DataSourceHeader = "data source";
        bool bloaded = false;
        internal void LoadRuntimeStrings()
        {
            if (bloaded)
                return;

            lock (lockObject)
            {
                if (mapRuntimeCache != null)
                    mapRuntimeCache.Clear();
            }

            bloaded = true;
            String altResourceFile = null;
            String xmlFile = null;
            try
            {
                var helper = new ConnectionStringParser(connectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (providerType == InMemoryDataStore.XpoProviderTypeString)
                {
                    xmlFile = helper.GetPartByName(DataSourceHeader);
                    if (xmlFile.Length > 0)
                    {
                        xmlFile = xmlFile.Replace('/', '\\');
                        int pos = xmlFile.LastIndexOf('\\');
                        if (pos != -1)
                            altResourceFile = xmlFile.Substring(0, pos + 1);
                    }
                }
            }
            catch (Exception ex)
            { }
            bool altResource = false;
            if (!String.IsNullOrEmpty(altResourceFile))
            {
                try
                {
                    List<object> addlist = new List<object>();
                    string[] colnames = null;
                    var listResources = System.IO.Directory.GetFiles(altResourceFile, Properties.Settings.Default.CustomStringsExt /*"*.custstrings"*/,
                        SearchOption.TopDirectoryOnly);
                    foreach (var singleres in listResources)
                    {
                        addlist.Clear();
                        Utilities.ImportExportUtils.ImportExport.ImportFromFile(singleres, addlist, ref colnames);
                        altResource = (colnames.Length > 0 && addlist.Count > 0);
                        if (colnames.Length == 0)
                        {
                            continue;
                        }
                        for (int i = 1; i < colnames.Length; i++)
                        {
                            var culture = CultureInfo.GetCultureInfo(colnames[i]);

                            if (culture == null || culture.IsNeutralCulture || addlist.Count == 0)
                            {
                                continue;
                            }

                            Dictionary<string, string> newlang;
                            if (runtimestrings.ContainsKey(colnames[i]))
                                newlang = runtimestrings[colnames[i]];
                            else
                            {
                                newlang = new Dictionary<string, string>();
                                runtimestrings.Add(colnames[i], newlang);
                            }
                                
                            foreach (var line in addlist)
                            {
                                var p = line as string[];
                                if (p != null && p.Length >= i + 1)
                                {
                                    try
                                    {
                                        if (String.IsNullOrEmpty(p[i]))
                                            newlang.Add(p[0], p[0].Replace("\\n", "\r\n"));
                                        else
                                            newlang.Add(p[0], p[i].Replace("\\n", "\r\n"));
                                    }
                                    catch (Exception e)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        internal IList<String> GetListLocalCultures()
        {
            return (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ orderby tag.Locale select tag.Locale).ToList();
        }

        Dictionary<String, IDictionary<String, String>> mapRuntimeCache;

        internal IDictionary<String, String> GetMapStrings(String culture)
        {
            if (runtimestrings.ContainsKey(culture))
            {
                return runtimestrings[culture];
            }

            if (bloaded)
            {
                lock (lockObject)
                {
                    if (mapRuntimeCache == null)
                        mapRuntimeCache = new Dictionary<string, IDictionary<string, string>>();
                    if (mapRuntimeCache.ContainsKey(culture))
                        return mapRuntimeCache[culture];
                }
            }

            var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ 
                           where tag.Locale.ToLower() == culture.ToLower()
                           select tag).ToList();
            if (locales.Count < 1)
                return null;

            var map = new Dictionary<String, String>();
            foreach (var localeText in locales[0].UFStringLocaleTexts)
            {
                var locale = localeText.Locale;
                if (String.IsNullOrEmpty(locale))
                    locale = localeText.Text;

                if (!map.ContainsKey(localeText.Text))
                    map.Add(localeText.Text, locale.Replace("\\n", "\r\n"));
            };

            if (bloaded)
            {
                lock (lockObject)
                {
                    if (mapRuntimeCache.ContainsKey(culture))
                        mapRuntimeCache.Remove(culture);
                    mapRuntimeCache.Add(culture, map);
                }
            }

            return map;
        }

        internal IList<String> GetListStringIDs(bool inExecution = false)
        {
            IList<String> list = null;
            if(inExecution)
            {
                using(UnitOfWork uow = new UnitOfWork(dl))
                {
                    list = GetListStringIDs(uow);
                }
            }
            else
            {
                list = GetListStringIDs(uow);
            }
            return list;
        }

        internal IList<String> GetListStringIDs(UnitOfWork uow)
        {
            var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/
                           select tag).ToList();
            if (locales.Count < 1)
                return null;

            var list = new List<String>();
            Parallel.ForEach(locales[0].UFStringLocaleTexts, localeText =>
            {
                lock (list)
                {
                    if (!list.Contains(localeText.Text))
                        list.Add(localeText.Text);
                }
            });
            return list;
        }

#if !NET_STANDARD
        String NewStringLocaleTextID(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Resources.NewTextID;

            if (listname == null)
                listname = GetListStringIDs();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        public bool LocaleTextIDExists(String name)
        {
            return (from hs in new XPQuery<StringModel.UFStringLocaleText>(uow, true).AsParallel()
                    where hs.Text == name
                    select hs).ToList().Count > 0;
        }

        public List<StringModel.UFStringLocaleText> GetLocaleTextFromId(string id)
        {
            return (from tag in new XPQuery<StringModel.UFStringLocaleText>(uow, true) 
                                where tag.Text == id
                               select tag).ToList();
        }

        internal List<ExpandoObject> GetListLocaleFlat()
        {
            using (var cursor = new WaitCursor())
            {
                var map = new Dictionary<String, ExpandoObject>();
                var listData = new List<ExpandoObject>();
                var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();

                foreach (var item in locales)
                {
                    foreach (var localeText in item.UFStringLocaleTexts)
                    {
                        lock (map)
                        {
                            ExpandoObject dynObject;
                            if (!map.ContainsKey(localeText.Text))
                            {
                                dynObject = new ExpandoObject();
                                map.Add(localeText.Text, dynObject);
                                listData.Add(dynObject);
                            }
                            else
                                dynObject = map[localeText.Text];

                            var p = dynObject as IDictionary<String, object>;
                            p[idText] = localeText.Text;
                            p[item.Locale] = localeText.Locale;
                        }
                    }
                }

                return listData;
            }
        }

        internal void UpdateLocaleData(List<ExpandoObject> listData, List<String> listLocales)
        {
            if (listData == null)
                return;
            using (var cursor = new WaitCursor())
            {
                //uow.CommitChanges();
                var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();
                locales.ForEach(locale => locale.Delete());

                foreach (var loc in listLocales)
                {
                    var locobj = new StringModel.UFStringLocale(uow) { Locale = loc, Name = loc };
                    foreach (var data in listData)
                    {
                        var p = data as IDictionary<String, object>;
                        if (/*!p.ContainsKey(loc) || */!p.ContainsKey(idText))
                            continue;
                        var loctext = new StringModel.UFStringLocaleText(uow)
                        {
                            Text = p[idText] as String,
                            Locale = p.ContainsKey(loc) ? p[loc] as String : null,
                            Culture = loc
                        };
                        locobj.UFStringLocaleTexts.Add(loctext);
                    }
                }
            }
            NeedsSave = true;
        }

        internal void UpdateSingleLocaleData(object obj, List<String> listLocales, bool isIDChanging, string oldID)
        {
            string newID = string.Empty;
            var currlist = (from tag in new XPQuery<StringModel.UFStringLocaleText>(uow, true).AsParallel() where tag.UFStringLocale != null select tag).ToList();

            var h = obj as IDictionary<string, object>;
            if (h != null)
            {
                for (int j = 0; (j < listLocales.Count); j++)
                {
                    StringModel.UFStringLocaleText newhs = null;

                    newID = ((dynamic)h).ID;
                    if (isIDChanging)
                    {
                        newhs = currlist.Find(o => { return (o.UFStringLocale.Name == listLocales[j] && o.Text == oldID); });

                        if (newhs != null)
                        {
                            newhs.Text = newID;

                            if (newhs.Culture == null)
                                newhs.Culture = listLocales[j];

                            if (h.ContainsKey(newhs.Culture) && h[newhs.Culture] != null)
                            {
                                newhs.Locale = h[newhs.Culture].ToString();
                            }
                        }
                    }
                    else
                    {
                        newhs = currlist.Find(o => { return (o.UFStringLocale.Name == listLocales[j] && o.Text == newID); });

                        if (newhs != null)
                        {
                            if (newhs.Culture == null)
                                newhs.Culture = listLocales[j];
                            
                            if (h.ContainsKey(newhs.Culture) && h[newhs.Culture] != null)
                            {
                                newhs.Locale = h[newhs.Culture].ToString();
                            }
                            
                        }
                    }
                }
            }

            NeedsSave = true;
        }

        public bool UpdateStringID(string oldId, string newId)
        {
            if(_stringEditorService == null) 
                return false;

            return _stringEditorService.UpdateStringID(oldId, newId);
        }


        public void RemoveLocale(string lang)
        {
            var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();
            if (locales.Count > 0)
            {
                var culture = locales.Find((o) => { return o.Name == lang; });
                if (culture != null)
                {
                    culture.Delete();
                    editorManagerComponent.OnLocalesChanged(this);
                }
            }
        }

        public bool RemoveLocaleText(string stringid)
        {
            if (!LocaleTextIDExists(stringid))
                return false;
            (from p in new XPQuery<StringModel.UFStringLocaleText>(uow, true).AsParallel()
             where p.Text == stringid
             select p).ToList().ForEach(tag => tag.Delete());

            return true;
        }
        internal void RemoveLocaleText(List<XPObject> list)
        {
            var items = new List<StringModel.UFStringLocaleText>();
            foreach (var i in list)
            {
                var word = i as StringModel.UFStringLocaleText;
                if (word != null)
                {
                    if (items.Count > 0 && items.Find((o) => { return o.Text == word.Text; }) != null)
                        continue;
                    string id = word.Text;
                    var idtext = GetLocaleTextFromId(id);
                    if (idtext.Count > 0)
                        items.AddRange(idtext);
                }
            }
            foreach (var ltext in items)
            {
                ltext.Delete();
            }
            items.Clear();

        }

        public List<StringModel.UFStringLocaleText> AddLocaleText(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            List<StringModel.UFStringLocaleText> ret = new List<StringModel.UFStringLocaleText>();
            using (var cursor = new WaitCursor())
            {
                var locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow, true)/*.AsParallel()*/ select tag).ToList();
                foreach (var locale in locales)
                {
                    var loctext = new StringModel.UFStringLocaleText(uow) 
                    {
                        Text = id
                    };
                    locale.UFStringLocaleTexts.Add(loctext);
                    loctext.Culture = locale.Name;
                    ret.Add(loctext);
                }
            }
            return ret;
        }

        internal void LogGeneralInfo(string msg)
        {
             logGeneral.Info(msg);
        }
        internal void LogGeneralError(string msg, Exception ex)
        {
            logGeneral.Error(msg, ex);
        }
#endif
        #endregion

        #region ICloneable Members

        StringEditorDocument(StringEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

#if !NET_STANDARD        
        public object Clone()
        {
            return new StringEditorDocument(this);
        }
#endif
#endregion

#region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

#if !NET_STANDARD  
        UserControl activeView;
        [Browsable(false)]
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                activeView = value;
            }
        }

        [Browsable(false)]
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif

        IDocument parent;
        [Browsable(false)]
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        [Browsable(false)]
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            // return new Uri(new Uri(System.IO.Path.GetDirectoryName(FullPath) + "\\"), relative);
            return null;
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
        }

#if !NET_STANDARD  
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return new PhysicalFileSystemProvider("");
            }
        }
#endif

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(fileBase);
            }
        }

        [Browsable(false)]
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(fileBase);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return fileBase;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return GetListStringIDs().Count == 0;
            }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

#endregion

#region IDisposable

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            OnDisposing(this);

            base.OnDispose();

#if !NET_STANDARD  
            if(EditorManagerComponent.Workspace != null)
                EditorManagerComponent.Workspace.ContextObject = null;
#endif
            
            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }

            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
            if (dlClipboard != null)
            {
                dlClipboard.Dispose();
                dlClipboard = null;
            }

            bloaded = false;
            runtimestrings.Clear();
        }

#endregion
    }
}
