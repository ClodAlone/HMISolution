// <copyright file="DocumentContainer_PersistState.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Constants
        /// <summary>
        /// Presents DocumentValues
        /// </summary>
        private const string REG_SUBKEY_NAME = "DocumentValues";
        
        /// <summary>
        /// Presents SaveDocumentState
        /// </summary>
        private const string REG_PARAM_NAME = "SaveDocumentState";
        
        /// <summary>
        /// Presents BUFFER_SIZE
        /// </summary>
        private const int REG_BUFFER_SIZE = 950;
        
        /// <summary>
        /// Presents PARAMETERS_COUNT
        /// </summary>
        private const int CHILD_PARAMETERS_COUNT = 12;
        
        /// <summary>
        /// Presents MAIN_PARAMETERS_COUNT 
        /// </summary>
        private const int MAIN_PARAMETERS_COUNT = 8;
        
        /// <summary>
        /// Presents NAME_PARAMNAME
        /// </summary>
        private const string NAME_PARAMNAME = "Name";
        
        /// <summary>
        /// Presents MDIBOUNDS_PARAMNAME
        /// </summary>
        private const string MDIBOUNDS_PARAMNAME = "MDIBounds";
        
        /// <summary>
        /// Presents MDIMINIMIZEDBOUNDS_PARAMNAME
        /// </summary>
        private const string MDIMINIMIZEDBOUNDS_PARAMNAME = "MDIMinimizedBounds";
        
        /// <summary>
        /// Presents MDIWINDOWSTATE_PARAMNAME 
        /// </summary>
        private const string MDIWINDOWSTATE_PARAMNAME = "MDIWindowState";
        
        /// <summary>
        /// Presents CANCLOSE_PARAMNAME
        /// </summary>
        private const string CANCLOSE_PARAMNAME = "CanClose";
        
        /// <summary>
        /// Presents ALLOWMDIRESIZE_PARAMNAME
        /// </summary>
        private const string ALLOWMDIRESIZE_PARAMNAME = "AllowMDIResize";
        
        /// <summary>
        /// Presents STATE_PARAMNAME
        /// </summary>
        private const string STATE_PARAMNAME = "State";
        
        /// <summary>
        /// Presents FORMAT_STRING
        /// </summary>
        private const string FORMAT_STRING = "{0:G}";
        
        /// <summary>
        /// Presents EMPTY
        /// </summary>
        private const string EMPTY = "Empty";
        
        /// <summary>
        /// Presents IsKeepCircle
        /// </summary>
        private const string ISKEEPCIRCLE_PARAMNAME = "IsKeepCircle";
        
        /// <summary>
        /// Presents SwitchMode
        /// </summary>
        private const string SWITCHMODE_PARAMNAME = "SwitchMode";
        
        /// <summary>
        /// Presents Mode
        /// </summary>
        private const string MODE_PARAMNAME = "Mode";
        
        /// <summary>
        /// Presents CanMDIMaximize
        /// </summary>
        private const string CANMDIMAXIMIZE_PARAMNAME = "CanMDIMaximize";
        
        /// <summary>
        /// Presents CanMDIMinimize
        /// </summary>
        private const string CANMDIMINIMIZE_PARAMNAME = "CanMDIMinimize";
        
        /// <summary>
        /// Presents DelayPreviewTime
        /// </summary>
        private const string DELAYPREVIEWTIME_PARAMNAME = "DelayPreviewTime";
        
        /// <summary>
        /// Presents IsEnabledScroll
        /// </summary>
        private const string ISENABLEDSCROLL_PARAMNAME = "IsEnabledScroll";
        
        /// <summary>
        /// Presents IsActive
        /// </summary>
        private const string ISACTIVE_PARAMNAME = "IsActive";
        
        /// <summary>
        /// Presents TDIIndex
        /// </summary>
        private const string TDIINDEX_PARAMNAME = "TDIIndex";
        
        /// <summary>
        /// Presents IsSelected
        /// </summary>
        private const string ISSELECTED_PARAMNAME = "IsSelected";
        
        /// <summary>
        /// Presents IsAllowMDIResize
        /// </summary> 
        private const string ISALLOWMDIRESIZE__PARAMNAME = "IsAllowMDIResize";
        
        /// <summary>
        /// Presents TDIGroupOrientation
        /// </summary>
        private const string TDIGROUP_ORIENTATION_PARAMNAME = "TDIGroupOrientation";
        
        /// <summary>
        /// Presents WayOfTDIGroup
        /// </summary>
        private const string WAY_OF_TDIGROUP_PARAMNAME = "WayOfTDIGroup";

        private const string SPLIT_PANEL_OFFSET = "SplitPanelOffset";
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when new Persist state was set.
        /// </summary>
        public event RoutedEventHandler PersistStateSet;
        #endregion

        #region Private members
        /// <summary>
        /// Presents store file name.
        /// </summary>
        private readonly string m_StoreFileName = AppDomain.CurrentDomain.FriendlyName + ".dat";

        /// <summary>
        /// Presents default state for persist state.
        /// </summary>
        private string m_stateDefault;

        /// <summary>
        /// represents the group flag
        /// </summary>
        internal bool m_Group = true;
        #endregion

        #region Public methods
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        public void SaveDockState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            SaveDockState(isoStorage, m_StoreFileName);
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="isoStorage">The isolated storage file.</param>
        /// <param name="storeFileName">Name of the store file.</param>
        public void SaveDockState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !string.IsNullOrEmpty(storeFileName))
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage);
                try
                {
                    XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                    WriteDateToWriter(writer);
                    writer.Flush();
                }
                finally
                {
                    stream.Close();
                }
            }
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public void SaveDockState(BinaryFormatter serializer)
        {
            MemoryStream memStream = new MemoryStream(REG_BUFFER_SIZE);
            byte[] byteArr = GetByteData();

            serializer.Serialize(memStream, byteArr);
            SaveToRegistry(memStream);
            m_Group = true;
            memStream.Close();
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="serializer">The Serializer value.</param>
        /// <param name="format">The format value.</param>
        /// <param name="path">The path value.</param>
        public void SaveDockState(IFormatter serializer, StorageFormat format, string path)
        {
            if (format == StorageFormat.Xml)
            {
                SaveDockState(path);
            }
            else
            {
                List<DocumentParamsBase> docParamsList = GetDocumentParams();
                DocumentParamsBase[] docParams = new DocumentParamsBase[docParamsList.Count];

                docParamsList.CopyTo(docParams);

                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                serializer.Serialize(fileStream, docParams);
                fileStream.Close();
            }
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="path">The path value.</param>
        public void SaveDockState(string path)
        {
            using (XmlTextWriter writer = new XmlTextWriter(Path.GetFullPath(path), Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                SaveDockState(writer);
                writer.Flush();
            }
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveDockState(XmlWriter writer)
        {
            if (writer != null)
            {
                WriteDateToWriter(writer);
            }
            else
            {
                throw new ArgumentNullException("writer");
            }
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        public void LoadDockState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            LoadingPersistState = true;
            LoadDockState(isoStorage, m_StoreFileName);
            m_layoutPanel.UpdateAfterPersistLoad();
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="isoStorage">The isolated storage file.</param>
        /// <param name="storeFileName">Name of the store file.</param>
        public void LoadDockState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !string.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Open, isoStorage);

                try
                {
                    XmlTextReader xmlTextReader = new XmlTextReader(stream);
                    LoadState(xmlTextReader);
                }
                finally
                {
                    stream.Close();
                }
            }
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public void LoadDockState(BinaryFormatter serializer)
        {
            LoadingPersistState = true;
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REG_SUBKEY_NAME);

            if (regKey != null)
            {
                MemoryStream memStream = new MemoryStream(REG_BUFFER_SIZE);
                byte[] byteArr = (byte[])regKey.GetValue(REG_PARAM_NAME);

                if (byteArr != null)
                {
                    memStream.Write(byteArr, 0, byteArr.Length);
                    memStream.Position = 0;
                    serializer.Deserialize(memStream);
                    LoadState(byteArr);
                }

                memStream.Close();
            }
            m_layoutPanel.UpdateAfterPersistLoad();
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="serializer">The Serializer value.</param>
        /// <param name="format">The format value.</param>
        /// <param name="path">The path value.</param>
        public void LoadDockState(IFormatter serializer, StorageFormat format, string path)
        {
            if (File.Exists(path))
            {
                LoadingPersistState = true;

                if (format == StorageFormat.Xml)
                {
                    LoadDockState(path);
                }
                else
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                    try
                    {
                        DocumentParamsBase[] paramsArr = (DocumentParamsBase[])serializer.Deserialize(fileStream);
                        fileStream.Close();
                        Dictionary<object, object> dockParamsTable = ParamsTable.Params;
                        RemoveRecentItems(dockParamsTable);
                        LoadState(dockParamsTable);
                        dockParamsTable.Clear();
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }

                m_layoutPanel.UpdateAfterPersistLoad();
                UpdateActiveDocumentInternal();
            }
        }

        /// <summary>
        /// Updates the ActiveDocumentInternal
        /// </summary>
        private void UpdateActiveDocumentInternal()
        {
            if (m_ActiveDocumentInternal != null)
            {
                ActiveDocument = m_ActiveDocumentInternal;
                if (Mode==DocumentContainerMode.TDI)
                {
                    (m_layoutPanel as TDILayoutPanel).SetActiveItem(m_ActiveDocumentInternal as FrameworkElement);
                }
                else
                {
                    (m_layoutPanel as MDILayoutPanel).SetActiveItem(m_ActiveDocumentInternal as FrameworkElement);
                }
            }
        }

        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="path">The path value.</param>
        public void LoadDockState(string path)
        {
            using (XmlTextReader reader = new XmlTextReader(Path.GetFullPath(path)))
            {
                LoadState(reader);
            }
        }
        
        /// <summary>
        /// Deletes the internal isolated storage.
        /// </summary>
        public void DeleteInternalIsolatedStorage()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            if (!string.IsNullOrEmpty(m_StoreFileName) && 0 < isoStorage.GetFileNames(m_StoreFileName).Length)
            {
                isoStorage.DeleteFile(m_StoreFileName);
            }
        }
        
        /// <summary>
        /// Resets the state.
        /// </summary>
        public void ResetState()
        {
            StringReader readerStr = new StringReader(m_stateDefault);

            XmlTextReader reader = new XmlTextReader(readerStr);
            LoadState(reader);
            reader.Close();
        }
        
        /// <summary>
        /// Deletes the state of the dock.
        /// </summary>
        /// <param name="path">The path value.</param>
        public void DeleteDockState(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        
        /// <summary>
        /// Deletes the state of the dock.
        /// </summary>
        public void DeleteDockState()
        {
            RegistryKey regKey = Registry.CurrentUser;
            m_Group = false;
            if (regKey.OpenSubKey(REG_SUBKEY_NAME) != null)
            {
                regKey.DeleteSubKeyTree(REG_SUBKEY_NAME);
            }
        }
        
        /// <summary>
        /// Saves the state of the dock.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveDockState(TextWriter writer)
        {
            XmlWriter writerXML = new XmlTextWriter(writer);
            SaveDockState(writerXML);
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void LoadDockState(TextReader reader)
        {
            XmlTextReader readerXML = new XmlTextReader(reader);
            LoadState(readerXML);
            m_layoutPanel.UpdateAfterPersistLoad();
            UpdateActiveDocumentInternal();
        }
        
        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void LoadDockState(XmlTextReader reader)
        {
            LoadState(reader);
        }

        public void RestoreDocument(UIElement element)
        {
            if (ILayoutPanel != null &&  DockingManager.GetState(element)==DockState.Hidden)
            {
                if (Mode == DocumentContainerMode.TDI)
                {
                    TDILayoutPanel panel = ILayoutPanel as TDILayoutPanel;
                    panel.RestoreTDI(element);
                }
                else if (Mode == DocumentContainerMode.MDI)
                {
                    DockingManager.SetState(element, DockState.Document);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves default state of the docking window.
        /// </summary>
        protected virtual void SaveDefaultState()
        {
            if (dockingManager !=null && !dockingManager.canUpdateTDIindex)
            dockingManager.updateTDIindexInternal = true;
            StringWriter writerStr = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(writerStr);
            SaveDockState(writer);
            writer.Close();

            m_stateDefault = writerStr.ToString();
        }
        
        /// <summary>
        /// Called when [persist state set].
        /// </summary>
        protected virtual void OnPersistStateSet()
        {
            if (null != PersistStateSet)
            {
                RoutedEventArgs arg = new RoutedEventArgs();
                PersistStateSet(this, arg);
            }
        }
        
        /// <summary>
        /// Prepares the state of the persist.
        /// </summary>
        private void PreperePersistState()
        {
            if (PersistState)
            {
                try
                {
                    LoadDockState();
                }
                catch (XmlException ex)
                {
                    Debug.Print(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

		private static readonly Dictionary<Type, XmlSerializer> _xmlSerializerCache = new Dictionary<Type, XmlSerializer>();

		public static XmlSerializer CreateDefaultXmlSerializer(Type type)
		{
			XmlSerializer serializer;
			if (_xmlSerializerCache.TryGetValue(type, out serializer))
			{
				return serializer;
			}
			else
			{
				var importer = new XmlReflectionImporter();
				var mapping = importer.ImportTypeMapping(type, null, null);
				serializer = new XmlSerializer(mapping);
				return _xmlSerializerCache[type] = serializer;
			}
		}
        
        /// <summary>
        /// Writes the date to writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void WriteDateToWriter(XmlWriter writer)
        {
            List<DocumentParamsBase> paramsList = GetDocumentParams();
            XmlSerializer serializer = CreateDefaultXmlSerializer(typeof(List<DocumentParamsBase>));
            serializer.Serialize(writer, paramsList);
        }
        
        /// <summary>
        /// Gets the document params.
        /// </summary>
        /// <returns> List DocumentParamsBase</returns>
        private List<DocumentParamsBase> GetDocumentParams()
        {
            List<DocumentParamsBase> documentParams = new List<DocumentParamsBase> { new MainDocumentParams(this, PropertiesMode.Main) };
            SetSplitPanelOffset();
            foreach (object element in Items)
            {
                if (element is FrameworkElement)
                {
                    documentParams.Add(CreateChildDocumentParams(element as FrameworkElement));
                }
                else
                {
                    ContentControl control = new ContentControl();
                    control.DataContext = element;
                    documentParams.Add(CreateChildDocumentParams(control));
                }
            }

            return documentParams;
        }

        private void SetSplitPanelOffset()
        {
            foreach (FrameworkElement item in Items)
            {
                TDISplitPanel splitpanel = (TDISplitPanel)VisualUtils.FindAncestor(item, typeof(TDISplitPanel));
                if (splitpanel != null)
                {
                    item.SetValue(TDILayoutPanel.SplitPanelOffsetProperty, splitpanel.Offset);
                }
            }
        }
        
        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void LoadState(XmlReader reader)
        {
            XmlSerializer serializer = CreateDefaultXmlSerializer(typeof(List<DocumentParamsBase>));

            if (serializer.CanDeserialize(reader))
            {
                try
                {
                    List<DocumentParamsBase> docParamsList = (List<DocumentParamsBase>)serializer.Deserialize(reader);
                    RectConverter converter = new RectConverter();                    
                    RemoveRecentItems(docParamsList);
                    foreach (DocumentParamsBase docParams in docParamsList)
                    {
                        LoadState(docParams, converter);
                    }
                    LoadingPersistState = true;
                    m_layoutPanel.UpdateAfterPersistLoad();
                    OnPersistStateSet();
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.InnerException is XmlException)
                    {
                        throw ex.InnerException;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                Debug.Print("XmlReader can't deserialize.");
            }
        }
        
        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        private void LoadState(byte[] byteArr)
        {
            char[] ch = Encoding.UTF8.GetChars(byteArr);
            string endStr = new string(ch);

            string[] kvPairs = endStr.Split(';');
            int iterationsCnt = (kvPairs.Length - MAIN_PARAMETERS_COUNT) / CHILD_PARAMETERS_COUNT;
            RectConverter converter = new RectConverter();

            MainDocumentParams mainParams = new MainDocumentParams
            {
                IsKeepCircle = bool.Parse(GetParamFromStr(ISKEEPCIRCLE_PARAMNAME, ref endStr)),
                SwitchMode = (SwitchMode)Enum.Parse(typeof(SwitchMode), GetParamFromStr(SWITCHMODE_PARAMNAME, ref endStr)),
                Mode = (DocumentContainerMode)Enum.Parse(typeof(DocumentContainerMode), GetParamFromStr(MODE_PARAMNAME, ref endStr)),
                CanMDIMaximize = bool.Parse(GetParamFromStr(CANMDIMAXIMIZE_PARAMNAME, ref endStr)),
                CanMDIMinimize = bool.Parse(GetParamFromStr(CANMDIMINIMIZE_PARAMNAME, ref endStr)),
                DelayPreviewTime = TimeSpan.Parse(GetParamFromStr(DELAYPREVIEWTIME_PARAMNAME, ref endStr)),
                IsEnabledScroll = bool.Parse(GetParamFromStr(ISENABLEDSCROLL_PARAMNAME, ref endStr)),
                IsAllowMDIResize = bool.Parse(GetParamFromStr(ISALLOWMDIRESIZE__PARAMNAME, ref endStr))
            };

            LoadState(mainParams, converter);
            List<string> names = GetChildrenNames();

            for (int i = 0; i < iterationsCnt; i++)
            {
                ChildDocumentParams docParams = new ChildDocumentParams
                {
                    Name = GetParamFromStr(NAME_PARAMNAME, ref endStr),
                    MDIBounds = GetParamFromStr(MDIBOUNDS_PARAMNAME, ref endStr),
                    MDIMinimizedBounds = GetParamFromStr(MDIMINIMIZEDBOUNDS_PARAMNAME, ref endStr),
                    MDIWindowState = (MDIWindowState)Enum.Parse(typeof(MDIWindowState), GetParamFromStr(MDIWINDOWSTATE_PARAMNAME, ref endStr)),
                    CanClose = bool.Parse(GetParamFromStr(CANCLOSE_PARAMNAME, ref endStr)),
                    AllowMDIResize = bool.Parse(GetParamFromStr(ALLOWMDIRESIZE_PARAMNAME, ref endStr)),
                    State = (DockState)Enum.Parse(typeof(DockState), GetParamFromStr(STATE_PARAMNAME, ref endStr)),
                    IsActive = bool.Parse(GetParamFromStr(ISACTIVE_PARAMNAME, ref endStr)),
                    TDIIndex = int.Parse(GetParamFromStr(TDIINDEX_PARAMNAME, ref endStr)),
                    IsSelected = bool.Parse(GetParamFromStr(ISSELECTED_PARAMNAME, ref endStr)),
                    TDIGroupOrientation = (Orientation)Enum.Parse(typeof(Orientation), GetParamFromStr(TDIGROUP_ORIENTATION_PARAMNAME, ref endStr)),
                    WayOfTDIGroup = GetParamFromStr(WAY_OF_TDIGROUP_PARAMNAME, ref endStr),
                    SplitPanelOffset = double.Parse(GetParamFromStr(SPLIT_PANEL_OFFSET, ref endStr))
                };

                names.Remove(docParams.Name);
                LoadState(docParams, converter);
            }

            RemoveNotPersistedItems(names);

            OnPersistStateSet();
        }
        
        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="paramsTable">The params table.</param>
        private void LoadState(Dictionary<object, object> paramsTable)
        {
            RectConverter converter = new RectConverter();
            CultureInfo invarInfo = CultureInfo.InvariantCulture;
            bool isntFirstCicle = false;

            try
            {
                foreach (KeyValuePair<object, object> elementPairData in paramsTable)
                {
                    Hashtable subtable = (Hashtable)elementPairData.Value;
                    Dictionary<string, string> keyVal = new Dictionary<string, string>();

                    foreach (DictionaryEntry pair in subtable)
                    {
                        string key = string.Format(invarInfo, FORMAT_STRING, pair.Key);
                        string value = string.Format(invarInfo, FORMAT_STRING, pair.Value);
                        keyVal.Add(key, value);
                    }

                    if (isntFirstCicle)
                    {
                        ChildDocumentParams docParams = new ChildDocumentParams
                        {
                            Name = keyVal[NAME_PARAMNAME],
                            MDIMinimizedBounds = keyVal[MDIMINIMIZEDBOUNDS_PARAMNAME],
                            MDIBounds = keyVal[MDIBOUNDS_PARAMNAME],
                            MDIWindowState = (MDIWindowState)Enum.Parse(typeof(MDIWindowState), keyVal[MDIWINDOWSTATE_PARAMNAME]),
                            CanClose = bool.Parse(keyVal[CANCLOSE_PARAMNAME]),
                            AllowMDIResize = bool.Parse(keyVal[ALLOWMDIRESIZE_PARAMNAME]),
                            State = (DockState)Enum.Parse(typeof(DockState), keyVal[STATE_PARAMNAME]),
                            IsActive = bool.Parse(keyVal[ISACTIVE_PARAMNAME]),
                            TDIIndex = int.Parse(keyVal[TDIINDEX_PARAMNAME]),
                            IsSelected = bool.Parse(keyVal[ISSELECTED_PARAMNAME]),
                            TDIGroupOrientation = (Orientation)Enum.Parse(typeof(Orientation), keyVal[TDIGROUP_ORIENTATION_PARAMNAME]),
                            WayOfTDIGroup = keyVal[WAY_OF_TDIGROUP_PARAMNAME],
                            SplitPanelOffset = double.Parse(keyVal[SPLIT_PANEL_OFFSET])
                        };
                        LoadState(docParams, converter);
                    }
                    else
                    {
                        MainDocumentParams docParams = new MainDocumentParams
                        {
                            IsKeepCircle = bool.Parse(keyVal[ISKEEPCIRCLE_PARAMNAME]),
                            SwitchMode = (SwitchMode)Enum.Parse(typeof(SwitchMode), keyVal[SWITCHMODE_PARAMNAME]),
                            Mode = (DocumentContainerMode)Enum.Parse(typeof(DocumentContainerMode), keyVal[MODE_PARAMNAME]),
                            CanMDIMaximize = bool.Parse(keyVal[CANMDIMAXIMIZE_PARAMNAME]),
                            CanMDIMinimize = bool.Parse(keyVal[CANMDIMINIMIZE_PARAMNAME]),
                            DelayPreviewTime = TimeSpan.Parse(keyVal[DELAYPREVIEWTIME_PARAMNAME]),
                            IsEnabledScroll = bool.Parse(keyVal[ISENABLEDSCROLL_PARAMNAME]),
                            ////VisualStyle = keyVal[VISUALSTYLE_PARAMNAME],
                            IsAllowMDIResize = bool.Parse(keyVal[ISALLOWMDIRESIZE__PARAMNAME])
                        };
                        LoadState(docParams, converter);
                        isntFirstCicle = true;
                    }
                }

                OnPersistStateSet();
            }
            catch (ArgumentException ex)
            {
                throw new XmlException("Data was parsed incorrect", ex);
            }
        }
        
        /// <summary>
        /// Loads the state.
        /// </summary>
        /// <param name="docParams">The doc params.</param>
        /// <param name="converter">The converter.</param>
        private void LoadState(DocumentParamsBase docParams, TypeConverter converter)
        {
            ChildDocumentParams childParams = docParams as ChildDocumentParams;

            if (null != childParams)
            {
                FrameworkElement element = FindChild(childParams.Name);

                if (element != null)
                {
                    DocumentContainer.SetMDIWindowState(element, childParams.MDIWindowState);
                    DocumentContainer.SetCanClose(element, childParams.CanClose);
                    DocumentContainer.SetAllowMDIResize(element, childParams.AllowMDIResize);
                    DockingManager.SetState(element, childParams.State);

                    if (childParams.IsActive)
                    {
                        ActiveDocument = element;
                        m_ActiveDocumentInternal = element;
                    }

                    SetMDIBounds(converter, element, childParams.MDIBounds, childParams.MDIMinimizedBounds);
                    PrepareTDIProperties(element, childParams);
                }
            }
            else
            {
                MainDocumentParams mainParams = (MainDocumentParams)docParams;
                IsKeepCircle = mainParams.IsKeepCircle;
                SwitchMode = mainParams.SwitchMode;
                Mode = mainParams.Mode;
                CanMDIMaximize = mainParams.CanMDIMaximize;
                CanMDIMinimize = mainParams.CanMDIMinimize;
                DelayPreviewTime = mainParams.DelayPreviewTime;
                IsEnabledScroll = mainParams.IsEnabledScroll;
                ////SkinStorage.SetVisualStyle( this, mainParams.VisualStyle );
                IsAllowMDIResize = mainParams.IsAllowMDIResize;
                ApplyTemplate();
            }
        }
        
        /// <summary>
        /// Prepares the wrapper in incorrect position
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="childParams">The child params.</param>
        private void PrepareTDIProperties(DependencyObject element, ChildDocumentParams childParams)
        {
            if (DocumentContainerMode.TDI == Mode)
            {
                element.SetValue(TDILayoutPanel.TDIIndexProperty, childParams.TDIIndex);
                TDILayoutPanel.SetIsSelected(element,childParams.IsSelected);
                element.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, childParams.TDIGroupOrientation);
                element.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, childParams.WayOfTDIGroup);
                element.SetValue(TDILayoutPanel.SplitPanelOffsetProperty, childParams.SplitPanelOffset);
            }
        }
        
        /// <summary>
        /// Gets the byte data.
        /// </summary>
        /// <returns>byte array values</returns>
        private byte[] GetByteData()
        {
            MemoryStream memStream = new MemoryStream(REG_BUFFER_SIZE);
            MainDocumentParams mainParams = new MainDocumentParams(this, PropertiesMode.Main);
            WriteDockParams(memStream, mainParams);

            foreach (FrameworkElement element in Items)
            {
                ChildDocumentParams childParams = CreateChildDocumentParams(element);
                WriteDockParams(memStream, childParams);
            }

            memStream.Seek(0, SeekOrigin.Begin);
            long memStreamLen = memStream.Length;

            byte[] byteArray = new byte[memStreamLen];
            Array.Copy(memStream.ToArray(), byteArray, memStreamLen);
            memStream.Close();

            return byteArray;
        }

        /// <summary>
        /// Finds the child.
        /// </summary>
        /// <param name="name">The name element.</param>
        /// <returns>Framework Element</returns>
        private FrameworkElement FindChild(string name)
        {
            FrameworkElement child = null;

            foreach (FrameworkElement element in Items)
            {
                if (element.Name == name)
                {
                    child = element;
                    break;
                }
            }

            return child;
        }
        
        /// <summary>
        /// Gets the children names.
        /// </summary>
        /// <returns>List string</returns>
        private List<string> GetChildrenNames()
        {
            List<string> names = new List<string>(Items.Count);

            foreach (FrameworkElement element in Items)
            {
                names.Add(element.Name);
            }

            return names;
        }
        
        /// <summary>
        /// Removes the not persisted items.
        /// </summary>
        /// <param name="names">The names.</param>
        private void RemoveNotPersistedItems(List<string> names)
        {
            foreach (string name in names)
            {
                FrameworkElement element = FindChild(name);
                Items.Remove(element);
            }
        }
        
        /// <summary>
        /// Creates the child document params.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns> ChildDocument Params</returns>
        private ChildDocumentParams CreateChildDocumentParams(FrameworkElement element)
        {
            bool isActive = element == ActiveDocument;
            return m_layoutPanel.CreateChildDocumentParams(element, isActive);
        }
        #endregion

        #region Static method
        /// <summary>
        /// Gets the list serialized properties.
        /// </summary>
        /// <returns>List DependencyProperty </returns>
        internal static List<DependencyProperty> GetListSerializedProperties()
        {
            List<DependencyProperty> returnList = new List<DependencyProperty>
            {
                NameProperty,
                MDIBoundsProperty,
                MDIMinimizedBoundsProperty,
                MDIWindowStateProperty,
                CanCloseProperty,
                AllowMDIResizeProperty,
                DockingManager.StateProperty
            };

            return returnList;
        }

        /// <summary>
        /// Gets the list serialized properties.
        /// </summary>
        /// <param name="mode">The  PropertiesMode mode.</param>
        /// <returns>List DependencyProperty</returns>
        internal static List<DependencyProperty> GetListSerializedProperties(PropertiesMode mode)
        {
            List<DependencyProperty> returnList = new List<DependencyProperty>();

            if (PropertiesMode.Child == mode)
            {
                returnList.Add(NameProperty);
                returnList.Add(MDIBoundsProperty);
                returnList.Add(MDIMinimizedBoundsProperty);
                returnList.Add(MDIWindowStateProperty);
                returnList.Add(CanCloseProperty);
                returnList.Add(AllowMDIResizeProperty);
                returnList.Add(DockingManager.StateProperty);
                returnList.Add(TDILayoutPanel.TDIGroupOrientationProperty);
                returnList.Add(TDILayoutPanel.WayOfTDIGroupProperty);
                returnList.Add(TDILayoutPanel.TDIIndexProperty);
                returnList.Add(TDILayoutPanel.IsSelectedProperty);
                returnList.Add(TDILayoutPanel.SplitPanelOffsetProperty);
            }
            else if (PropertiesMode.Main == mode)
            {
                returnList.Add(IsKeepCircleProperty);
                returnList.Add(SwitchModeProperty);
                returnList.Add(ModeProperty);
                returnList.Add(CanMDIMaximizeProperty);
                returnList.Add(CanMDIMinimizeProperty);
                returnList.Add(DelayPreviewTimeProperty);
                returnList.Add(IsEnabledScrollProperty);
                ////returnList.Add( SkinStorage.VisualStyleProperty );
                returnList.Add(IsAllowMDIResizeProperty);
            }
            else
            {
#if DEBUG
                throw new NotImplementedException("This mode " + mode + ", wasn't implemented");
#endif
            }

            return returnList;
        }

        /// <summary>
        /// Sets the MDI bounds.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="element">The element.</param>
        /// <param name="mdiBounds">The MDI bounds.</param>
        /// <param name="mdiMinimizedBounds">The MDI minimized bounds.</param>
        private static void SetMDIBounds(TypeConverter converter, Visual element, string mdiBounds, string mdiMinimizedBounds)
        {
            if (!string.IsNullOrEmpty(mdiBounds) && mdiBounds != EMPTY)
            {
                mdiBounds = mdiBounds.Replace(';', ',');
                mdiMinimizedBounds = mdiMinimizedBounds.Replace(';', ',');

                Rect rect = (Rect)converter.ConvertFromInvariantString(mdiBounds);
                Rect minRect = (Rect)converter.ConvertFromInvariantString(mdiMinimizedBounds);
                DocumentContainer.SetMDIBounds(element, rect);
                DocumentContainer.SetMDIMinimizedBounds(element, minRect);
                MDIWindow window = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;

                if (window != null)
                {
                    window.IsPanelLayout = minRect.IsEmpty;
                    window.WasMinimizedDragged = !minRect.IsEmpty;
                }
            }
        }
        
        /// <summary>
        /// Saves to registry.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        private static void SaveToRegistry(MemoryStream memStream)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(REG_SUBKEY_NAME);

            if (regKey != null)
            {
                Registry.SetValue(regKey.ToString(), REG_PARAM_NAME, memStream.ToArray(), RegistryValueKind.Binary);
            }
        }
        
        /// <summary>
        /// Writes to stream.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="paramValue">The param value.</param>
        private static void WriteToStream(Stream memStream, string paramName, string paramValue)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(paramName + "=" + paramValue + ";");
            memStream.Write(byteArray, 0, byteArray.Length);
        }
        
        /// <summary>
        /// Writes the dock params.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        /// <param name="docParams">The doc params.</param>
        private static void WriteDockParams(Stream memStream, ChildDocumentParams docParams)
        {
            CultureInfo invarInfo = CultureInfo.InvariantCulture;
            WriteToStream(memStream, NAME_PARAMNAME, docParams.Name);
            WriteToStream(memStream, MDIBOUNDS_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.MDIBounds));
            WriteToStream(memStream, MDIMINIMIZEDBOUNDS_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.MDIMinimizedBounds));
            WriteToStream(memStream, MDIWINDOWSTATE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.MDIWindowState));
            WriteToStream(memStream, CANCLOSE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.CanClose));
            WriteToStream(memStream, ALLOWMDIRESIZE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.AllowMDIResize));
            WriteToStream(memStream, STATE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.State));
            WriteToStream(memStream, ISACTIVE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.IsActive));
            WriteToStream(memStream, TDIINDEX_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.TDIIndex));
            WriteToStream(memStream, ISSELECTED_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.IsSelected));
            WriteToStream(memStream, TDIGROUP_ORIENTATION_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.TDIGroupOrientation));
            WriteToStream(memStream, WAY_OF_TDIGROUP_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.WayOfTDIGroup));
        }
        
        /// <summary>
        /// Writes the dock params.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        /// <param name="docParams">The doc params.</param>
        private static void WriteDockParams(Stream memStream, MainDocumentParams docParams)
        {
            CultureInfo invarInfo = CultureInfo.InvariantCulture;
            WriteToStream(memStream, ISKEEPCIRCLE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.IsKeepCircle));
            WriteToStream(memStream, SWITCHMODE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.SwitchMode));
            WriteToStream(memStream, MODE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.Mode));
            WriteToStream(memStream, CANMDIMAXIMIZE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.CanMDIMaximize));
            WriteToStream(memStream, CANMDIMINIMIZE_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.CanMDIMinimize));
            WriteToStream(memStream, DELAYPREVIEWTIME_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.DelayPreviewTime));
            WriteToStream(memStream, ISENABLEDSCROLL_PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.IsEnabledScroll));
            ////WriteToStream( memStream, VISUALSTYLE_PARAMNAME, string.Format( invarInfo, FORMAT_STRING, docParams.VisualStyle ) );
            WriteToStream(memStream, ISALLOWMDIRESIZE__PARAMNAME, string.Format(invarInfo, FORMAT_STRING, docParams.IsAllowMDIResize));
        }
        
        /// <summary>
        /// Gets the param from STR.
        /// </summary>
        /// <param name="paramName">Name of the param.</param>
        /// <param name="fromStr">From STR values.</param>
        /// <returns>string value</returns>
        private static string GetParamFromStr(string paramName, ref string fromStr)
        {
            int index = fromStr.IndexOf(paramName);
            string result;

            if (index >= 0)
            {
                int i = fromStr.IndexOf('=', index);
                int cnt = fromStr.IndexOf(';', index);

                result = fromStr.Substring(++i, cnt - i);
                fromStr = fromStr.Remove(index, cnt - index);
            }
            else
            {
                throw new FormatException(paramName + " not found.");
            }

            return result;
        }
        #endregion
    }
}
