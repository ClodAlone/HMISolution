using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using VFS;
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using UnitConverterManager.ComponentService;
using System.Windows;
using System.Xml;
using System.Dynamic;
using Utilities;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using log4net;
using System.Globalization;
using UIMsgBoxAlertService.ComponentService;
using System.Data;

namespace UnitConverterManager.Document
{
    public class UnitConverterEditorDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations

        UnitOfWork uow;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        InMemoryDataStore InMemory;
        IDataLayer dl;
        IDataLayer dlClipboard;
        String connectionString;
        String fileBase;

        internal static String idText = "ID";

        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        #endregion

        #region Methods

        void CreateDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFUserModel.UFUser).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
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
                dl = new SimpleDataLayer(InMemory);
            }

            uow = new UnitOfWork(dl);
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.AfterRollbackTransaction += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.AfterBeginTrackingChanges += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.ObjectDeleting += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.ObjectsSaved += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };
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

        internal void CopyListToClipbaord(List<UnitConverterModel.UFConverterItem> list)
        {

            list.ForEach(hs =>
            {
                hs.Culture = hs.UFConverter.Name;
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
                return ((from c in new XPQuery<UnitConverterModel.UFConverterItem>(uowClipboard).AsParallel() select c).ToList().Count > 0 || Clipboard.ContainsText());
            }
            catch
            {
                return false;
            }
        }

        internal List<UnitConverterModel.UFConverterItem> PasteClipboardLocaleTexts()
        {
            var localList = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();

            var ret = new List<UnitConverterModel.UFConverterItem>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetListStringIDs();
            var listToCopy = (from c in new XPQuery<UnitConverterModel.UFConverterItem>(uowClipboard).AsParallel() orderby c.Name select c).ToList();
            string currID = string.Empty;
            string newID = string.Empty;
            if (listToCopy.Count > 0)
            {
                listToCopy.ForEach(hs =>
                {
                    if (currID != hs.Name)
                    {
                        newID = NewStringLocaleTextID(hs.Name, mapStartCounter, listName);
                        currID = hs.Name;
                    }
                    var newhs = CloneFromClipboard(hs, hs.Name != newID) as UnitConverterModel.UFConverterItem;
                    newhs.Name = newID;
                    newhs.UFConverter = localList.Find((o) => { return o.Name == hs.Culture; });
                    ret.Add(newhs);
                });
            }

            return ret;
        }

        internal List<UnitConverterModel.UFConverterItem> PasteListLocaleText(List<object> list, string[] mapname, bool update = true)
        {
            var ret = new List<UnitConverterModel.UFConverterItem>();
            var localList = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listName = GetListStringIDs();
            string currID = string.Empty;
            string newID = string.Empty;
            bool bCreateNew = false;
            bool bAsk = true;

            if (list.Count > 0)
            {
                var currlist = (from tag in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true).AsParallel() where tag.UFConverter != null select tag).ToList();
                list.ForEach(hs =>
                {
                    var p = hs as string[];
                    if (p != null)
                    {
                        //var currlist = (from tag in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true).AsParallel() where tag.Text == p[0] && tag.UFConverter != null select tag).ToList();


                        for (int j = 1; (j < p.Length && j < mapname.Length); j++)
                        {
                            UnitConverterModel.UFConverterItem newhs = null;
                            bool bLocalCreateNew = true;
                            if (update)
                            {
                                newID = p[0];
                                newhs = currlist.Find(o => { return (o.UFConverter.Name == mapname[j] && o.Name == p[0]); });
                                if (newhs != null)
                                {
                                    if(bAsk)
                                    {
                                        bAsk = false;
                                        bCreateNew = EditorManagerComponent.UIInterface.ShowYesNo(string.Format(Properties.Resources.OverrideConverter), CustomDialogIcons.Question) == CustomDialogResults.No;
                                    }
                                    if (bCreateNew && bLocalCreateNew)
                                    {
                                        bLocalCreateNew = false;
                                        newhs = new UnitConverterModel.UFConverterItem(uow)
                                        {
                                            Name = NewStringLocaleTextID(newID, mapStartCounter, listName),
                                            Locale = string.Empty,
                                            UFConverter = localList.Find((o) => { return o.Name == mapname[j]; })
                                        };
                                        p[0] = newhs.Name;
                                    }
                                }
                                else
                                    newhs = new UnitConverterModel.UFConverterItem(uow)
                                    {
                                        Name = newID,
                                        Locale = string.Empty,
                                        UFConverter = localList.Find((o) => { return o.Name == mapname[j]; })
                                    };

                                var ci = p[j].Split(UnitConverterModel.ExportUtils.ciseparator.ToCharArray());
                                newhs.InputExpression = ci.Count() > 0 ? ci[0] : string.Empty;
                                newhs.OutputExpression = ci.Count() > 1 ? ci[1] : string.Empty;
                                newhs.InputUnit = ci.Count() > 2 ? ci[2] : string.Empty;
                                newhs.Description = ci.Count() > 3 ? ci[3] : string.Empty;
                            }
                            else
                            {
                                if (currID != p[0])
                                {
                                    newID = NewStringLocaleTextID(p[0], mapStartCounter, listName);
                                    currID = p[0];
                                }
                                newhs = new UnitConverterModel.UFConverterItem(uow)
                                {
                                    Name = newID,
                                    Locale = string.Empty,
                                    UFConverter = localList.Find((o) => { return o.Name == mapname[j]; })
                                };
                                var ci = p[j].Split(UnitConverterModel.ExportUtils.ciseparator.ToCharArray());
                                newhs.InputExpression = ci.Count() > 0 ? ci[0] : string.Empty;
                                newhs.OutputExpression = ci.Count() > 1 ? ci[1] : string.Empty;
                                newhs.InputUnit = ci.Count() > 2 ? ci[2] : string.Empty;
                                newhs.Description = ci.Count() > 3 ? ci[3] : string.Empty;
                            }
                            ret.Add(newhs);
                        }
                        for (int k = p.Length; k < mapname.Length; k++)
                        {
                            ret.Add(new UnitConverterModel.UFConverterItem(uow)
                            {
                                Name = p[0],
                                Locale = string.Empty,
                                UFConverter = localList.Find((o) => { return o.Name == mapname[k]; })
                            });
                        }
                        for (int k = p.Length; k < mapname.Length; k++)
                        {
                            ret.Add(new UnitConverterModel.UFConverterItem(uow)
                            {
                                Name = p[0],
                                Locale = string.Empty,
                                UFConverter = localList.Find((o) => { return o.Name == mapname[k]; })
                            });
                        }
                    }
                });
            }
            return ret;
        }
        internal string AddLocale(string locale)
        {
            if (locale == idText)
                return null;

            if (GetListLocalConverter().ToList().Contains(locale))
                return null;
            var locobj = new UnitConverterModel.UFConverter(uow) { Locale = locale, Name = locale };
            var stringIds = GetListStringIDs();
            foreach (var id in stringIds)
            {
                locobj.UFConverterItems.Add(new UnitConverterModel.UFConverterItem(uow)
                {
                    Name = id,
                    InputExpression = null,
                    OutputExpression = null,
                    InputUnit = null,
                    Description = null,
                    Locale = locale
                });
            }
            return locale;
        }
        internal string UpdateLocale(UserControl control, string oldlocale, string newlocale)
        {
            if (newlocale == idText)
                return null;

            var locals = GetListLocalConverter().ToList();
            if (!locals.Contains(oldlocale))
                return null;

            if (locals.Contains(newlocale))
            {
                return null;
            }

            var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
            if (locales.Count > 0)
            {
                var culture = locales.Find((o) => { return o.Name == oldlocale; });
                if (culture == null)
                    return null;

                if (GetListLocalConverter().ToList().Contains(newlocale))
                    return null;

                culture.Name = culture.Locale = newlocale;

                (from ci in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true)/*.AsParallel()*/
                 where ci.Locale == oldlocale
                 select ci).ToList().ForEach(x => x.Locale = newlocale);

                //uow.CommitChanges();
            }


            return newlocale;
        }
        #endregion

        static String GetConnectionString(String path, FileSystemProviderBase vfs)
        {
            String connString = null;
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        internal static String GetBaseFilename(String path, FileSystemProviderBase vfs)
        {
            if (vfs != null)
                return null;

            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

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
                        (from tag in new XPQuery<UnitConverterModel.UFConverter>(uowTarget, true).AsParallel()
                         select tag).ToList().ForEach(tag => tag.Delete());

                        // following is only for ensuring the complitely deletion of unassociated object elements
                        (from p in new XPQuery<UnitConverterModel.UFConverterItem>(uowTarget, true).AsParallel()
                         where p.UFConverter == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////

                        var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(sourceDoc.GetSession(), true)/*.AsParallel()*/ select tag).ToList();
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
                    var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(sourceDoc.GetSession(), true)/*.AsParallel()*/ select tag).ToList();
                    foreach (var locale in locales)
                    {
                        locale.Delete();
                    }
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }

        public static UnitConverterEditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
                String connString = GetConnectionString(path, vfs);
                String xmlfile = GetBaseFilename(path, vfs);

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                var doc = new UnitConverterEditorDocument(null)
                {
                    connectionString = connString,
                    EditorManagerComponent = c as UnitConverterEditorManagerComponent,
                    fileBase = xmlfile,
                    Parent = parent
                };

                doc.CreateDataLayer();
                if (!bCreateNew && bCheckEmpty && doc.IsEmpty)
                {
                    doc.Dispose();
                    return null;
                }
                else if (vfs == null && !doc.IsBelongFromParent(parent))
                {
                    doc.Dispose();
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
                    return null;
                }

                return doc;
            }
            catch
            {
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
                return null;
            }
        }

        internal bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false, bool silent = false)
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

                if (!discargechanges && ActiveView != null)
                {
                    (ActiveView as UnitConverterEditorControl).CommitChanges();
                }

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
                if (silent)
                    logGeneral.ErrorFormat(Properties.Resources.ErrorSavingDocument, ex.Message);
                else
                    MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                            Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

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

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

        UnitConverterEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public UnitConverterEditorManagerComponent EditorManagerComponent
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

        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
        }
        #endregion

        #region Locale Settings

        internal String MatchConverter(String language)
        {
            var list = GetListLocalConverter();

            foreach (var l in list)
            {
                if (l.Contains(language))
                    return l;
            }

            return String.Empty;
        }

        internal IList<String> GetListLocalConverter()
        {
            return (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ orderby tag.Locale select tag.Locale).ToList();
        }

        internal IDictionary<String, String> GetMapStrings(String culture)
        {
            var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/
                           where tag.Locale == culture
                           select tag).ToList();
            if (locales.Count < 1)
                return null;

            var map = new Dictionary<String, String>();
            Parallel.ForEach(locales[0].UFConverterItems, localeText =>
            {
                if (localeText.Locale != null)
                {
                    lock (map)
                    {
                        map.Add(localeText.Name, localeText.Locale);
                    }
                }
            });
            return map;
        }
        
        internal IList<UnitConverterModel.UFConverterItem> GetListItems(string locale)
        {
            return (from tag in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true)/*.AsParallel()*/
                           where tag.Locale == locale
                           select tag).ToList();
        }


        internal IList<String> GetListStringIDs()
        {
            var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/
                           select tag).ToList();
            if (locales.Count < 1)
                return null;

            var list = new List<String>();
            Parallel.ForEach(locales[0].UFConverterItems, localeText =>
            {
                lock (list)
                {
                    if (!list.Contains(localeText.Name))
                        list.Add(localeText.Name);
                }
            });
            return list;
        }
        
        internal String GetUnitLabel(string converter, string id)
        {
            var converters = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/
                              where tag.Locale == converter
                              select tag).ToList();
            if (converters.Count < 1)
                return null;
            return (from item in converters[0].UFConverterItems where item.Name == id select item.InputUnit).FirstOrDefault();
        }
        internal String GetInputExpression(string converter, string id)
        {
            var converters = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/
                              where tag.Locale == converter
                              select tag).ToList();
            if (converters.Count < 1)
                return null;
            return (from item in converters[0].UFConverterItems where item.Name == id select item.InputExpression).FirstOrDefault();
        }

        internal String GetOutputExpression(string converter, string id)
        {
            var converters = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/
                              where tag.Locale == converter
                              select tag).ToList();
            if (converters.Count < 1)
                return null;
            return (from item in converters[0].UFConverterItems where item.Name == id select item.OutputExpression).FirstOrDefault();
        }
        String NewStringLocaleTextID(String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Resources.NewConverterID;

            if (listname == null)
                listname = GetListStringIDs();

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }
        bool LocaleTextIDExists(String name)
        {
            return (from hs in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true).AsParallel()
                    where hs.Name == name
                    select hs).ToList().Count > 0;
        }
        internal List<UnitConverterModel.UFConverterItem> GetLocaleTextFromId(string id)
        {
            return (from tag in new XPQuery<UnitConverterModel.UFConverterItem>(uow, true)
                    where tag.Name == id
                    select tag).ToList();
        }

        internal UnitConverterModel.UFConverter GetLocaleFromId(string id)
        {
            return (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)
                    where tag.Name == id
                    select tag).FirstOrDefault();
        }

        internal List<ExpandoObject> GetListLocaleFlat()
        {
            using (var cursor = new WaitCursor())
            {
                var map = new Dictionary<String, ExpandoObject>();
                var listData = new List<ExpandoObject>();
                var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
                
                foreach (var item in locales)
                {
                    foreach (var localeText in item.UFConverterItems.OrderBy(o => o.Name))
                    {
                        lock (map)
                        {
                            ExpandoObject dynObject;
                            if (!map.ContainsKey(localeText.Name))
                            {
                                dynObject = new ExpandoObject();
                                map.Add(localeText.Name, dynObject);
                                listData.Add(dynObject);
                            }
                            else
                                dynObject = map[localeText.Name];

                            var p = dynObject as IDictionary<String, object>;
                            p[idText] = localeText.Name;
                            p[item.Locale] = localeText;
                        }
                    }
                }

                return listData;
            }
        }

        internal void UpdateLocaleData(List<ExpandoObject> listData, List<String> listLocales)
        {
            using (var cursor = new WaitCursor())
            {
                //uow.CommitChanges();
                var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
                locales.ForEach(locale => locale.Delete());

                foreach (var loc in listLocales)
                {
                    var locobj = new UnitConverterModel.UFConverter(uow) { Locale = loc, Name = loc };
                    foreach (var data in listData)
                    {
                        var p = data as IDictionary<String, object>;
                        if (/*!p.ContainsKey(loc) || */!p.ContainsKey(idText))
                            continue;
                        var cp = p.ContainsKey(loc) ? p[loc] as UnitConverterModel.UFConverterItem : null;
                        var loctext = new UnitConverterModel.UFConverterItem(uow)
                        {
                            Name = p[idText] as String,
                            InputExpression = cp != null ? cp.InputExpression : null,
                            OutputExpression = cp != null ? cp.OutputExpression : null,
                            InputUnit = cp != null ? cp.InputUnit : null,
                            Description = cp != null ? cp.Description : null,
                            Locale = loc
                        };
                        if (!p.ContainsKey(loc))
                            p[loc] = loctext;

                        locobj.UFConverterItems.Add(loctext);
                    }
                }
            }
        }
        internal void UpdateLocaleText(List<XPObject> list)
        {
            foreach (var i in list)
            {
                var word = i as UnitConverterModel.UFConverterItem;
                if (word != null)
                {
                    var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true) where tag.Name == word.Culture select tag).ToList();
                    if (locales.Count > 0)
                    {
                        var lt = locales[0].UFConverterItems.ToList().Find((o) => { return o.Name == word.Name; });
                        if (lt != null)
                            lt.Locale = word.Locale;
                    }
                }
            }
        }

        internal void RemoveLocale(string lang)
        {
            var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
            if (locales.Count > 0)
            {
                var culture = locales.Find((o) => { return o.Name == lang; });
                if (culture != null)
                    culture.Delete();
            }
        }

        internal void RemoveLocaleText(List<XPObject> list)
        {
            var items = new List<UnitConverterModel.UFConverterItem>();
            foreach (var i in list)
            {
                var word = i as UnitConverterModel.UFConverterItem;
                if (word != null)
                {
                    if (items.Count > 0 && items.Find((o) => { return o.Name == word.Name; }) != null)
                        continue;
                    string id = word.Name;
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

        internal List<UnitConverterModel.UFConverterItem> AddLocaleText(string id)
        {
            List<UnitConverterModel.UFConverterItem> ret = new List<UnitConverterModel.UFConverterItem>();
            using (var cursor = new WaitCursor())
            {
                var locales = (from tag in new XPQuery<UnitConverterModel.UFConverter>(uow, true)/*.AsParallel()*/ select tag).ToList();
                foreach (var locale in locales)
                {
                    var loctext = new UnitConverterModel.UFConverterItem(uow)
                    {
                        Name = id,
                        UFConverter = locale
                    };
                    locale.UFConverterItems.Add(loctext);
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
        #endregion

        #region ICloneable Members

        UnitConverterEditorDocument(UnitConverterEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new UnitConverterEditorDocument(this);
        }

        #endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

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
            if(EditorManagerComponent.Workspace != null)
            EditorManagerComponent.Workspace.ContextObject = null;

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
        }

        #endregion
    }
}
