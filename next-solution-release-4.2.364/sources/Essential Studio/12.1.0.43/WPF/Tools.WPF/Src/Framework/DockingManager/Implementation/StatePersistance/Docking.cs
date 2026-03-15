// <copyright file="Docking.cs" company="Syncfusion">
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using System.Xml.XPath;
using System.Reflection;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies the Docking Manager
    /// </summary>

    public partial class DockingManager
    {
        #region Constants

        /// <summary>
        /// Defining constants that presents current docking parameters
        /// </summary>
        private const string C_NAME = "Name";

        /// <summary>
        /// Specify the State.
        /// </summary>
        private const string C_STATE = "State";

        /// <summary>
        /// Specify the NoDock.
        /// </summary>
        private const string C_NODock = "NoDock";

        /// <summary>
        /// Specify the CanDock.
        /// </summary>
        private const string C_CANDock = "CanDock";

        /// <summary>
        /// Specify the CanClose.
        /// </summary>
        private const string C_CANClose = "CanClose";

        /// <summary>
        /// Specify IsSelectedTab.
        /// </summary>
        private const string C_ISSelectedTab = "IsSelectedTab";

        /// <summary>
        /// Specify IsActiveWindow.
        /// </summary>
        private const string C_ISActiveWindow = "IsActiveWindow";

        /// <summary>
        /// Specify the side in dock mode.
        /// </summary>
        private const string C_SIDEDocked = "SideInDockedMode";

        /// <summary>
        /// Specify the side in float mode.
        /// </summary>
        private const string C_SIDEFloating = "SideInFloatMode";

        /// <summary>
        /// Specify the target name in dock mode.
        /// </summary>
        private const string C_TARGETDocked = "TargetNameInDockedMode";

        /// <summary>
        /// Specify the target name in floating mode.
        /// </summary>
        private const string C_TARGETFloating = "TargetNameInFloatingMode";

        /// <summary>
        /// Specify the desired height in docked mode.
        /// </summary>
        private const string C_DesiredHeightInDockedMode = "DesiredHeightInDockedMode";

        /// <summary>
        /// Specify the desired height in floating mode.
        /// </summary>
        private const string C_DesiredHeightInFloatingMode = "DesiredHeightInFloatingMode";

        /// <summary>
        /// Specify the desired width in docked mode.
        /// </summary>
        private const string C_DesiredWidthInDockedMode = "DesiredWidthInDockedMode";

        /// <summary>
        /// Specify the desired width in floating mode.
        /// </summary>
        private const string C_DesiredWidthInFloatingMode = "DesiredWidthInFloatingMode";

        /// <summary>
        /// Specify the floating window rect.
        /// </summary>
        private const string C_WindowRect = "FloatingWindowRect";

        /// <summary>
        /// Specify the tab group name.
        /// </summary>
        private const string C_TabGroupName = "TabGroupName";
        private const string C_CanMaximize = "CanMaximize";
        private const string C_CanMinimize = "CanMinimize,";
        private const string C_DockWindowState = "DockWindowState";
        private const string C_CanResizeInDockedState = "CanResizeInDockedState";
        private const string C_CanResizeInFloatState = "CanResizeInFloatState";
        private const string C_CanResizeHeightInDockedState = "CanResizeHeightInDockedState";
        private const string C_CanResizeWidthInDockedState = "CanResizeWidthInDockedState";
        private const string C_CanResizeHeightInFloatState = "CanResizeHeightInFloatState";
        private const string C_CanResizeWidthInFloatState = "CanResizeWidthInFloatState";
        private const string C_CanFloatMaximize = "CanFloatMaximize";
        private const string C_IsFixedSize = "IsFixedSize";
        private const string C_IsFixedHeight = "IsFixedHeight";
        private const string C_IsFixedWidth = "IsFixedWidth";
        private const string C_FixedHeight = "FixedHeight";
        private const string C_FixedWidth = "FixedWidth";
        private const string C_PreviousContainerHeight = "PreviousContainerHeight";
        private const string C_PreviousContainerWidth = "PreviousContainerWidth";
        private const string C_PreviousHostHeight = "PreviousHostHeight";
        private const string C_PreviousHostWidth = "PreviousHostWidth";
        private const string C_IsSwapped = "IsSwapped ";
        private const string C_ZorderInFloatMode = "ZorderInFloatMode";
        private const string C_PreviousFloatWindowRect = "PreviousFloatingWindowRect";
        private const string C_FloatingWindowState = "FloatingWindowState";
        /// <summary>
        /// Specify IsTabGroupOwner.
        /// </summary>
        private const string C_ISTabGroupOwner = "IsTabGroupOwner";

        /// <summary>
        /// Specify the tab chile order.
        /// </summary>
        private const string C_SideTabOrder = "TabChildOrder";

        /// <summary>
        /// Specify the index in dock mode.
        /// </summary>
        private const string C_IndexInDockMode = "IndexInDockMode";

        /// <summary>
        /// Specify the index in float mode.
        /// </summary>
        private const string C_IndexInFloatMode = "IndexInFloatMode";

        /// <summary>
        /// Specify the tab order in dock mode.
        /// </summary>
        private const string C_TabOrderInDockMode = "TabOrderInDockMode";

        /// <summary>
        /// Specify the tab order in float mode.
        /// </summary>
        private const string C_TabOrderInFloatMode = "TabOrderInFloatMode";

        /// <summary>
        /// Specify the MDI Bounds.
        /// </summary>
        private const string C_MDIBounds = "MDIBounds";

        /// <summary>
        /// Specify the MDI minimized Bounds.
        /// </summary>
        private const string C_MDIMinimizedBounds = "MDIMinimizedBounds";

        /// <summary>
        /// Specify the MDI Window state.
        /// </summary>
        private const string C_MDIWindowState = "MDIWindowState";

        /// <summary>
        /// Specify the Allow MDI resize.
        /// </summary>
        private const string C_AllowMDIResize = "AllowMDIResize";

        /// <summary>
        /// Specify the TFI index.
        /// </summary>
        private const string C_TDIIndex = "TDIIndex";

        /// <summary>
        /// Specify IsSelected.
        /// </summary>
        private const string C_IsSelected = "IsSelected";

        /// <summary>
        /// Specify the TDI Group orientation.
        /// </summary>
        private const string C_TDIGroupOrientation = "TDIGroupOrientation";

        /// <summary>
        /// Specify the way of TDI group.
        /// </summary>
        private const string C_WayOfTDIGroup = "WayOfTDIGroup";


        private const string C_TDISplitPanelOffset = "SplitPanelOffset";
        /// <summary>
        /// Count of parameters to store.
        /// </summary>
		/// Update the value when we add a property in DockingParams .
        private const int C_DockingParametersCount = 54;

        /// <summary>
        /// Specify the docking values.
        /// </summary>
        private const string C_RegSubKeyName = "DockingValues";

        /// <summary>
        /// Specify the save dock state.
        /// </summary>
        private const string C_RegParamName = "SaveDockState";

        /// <summary>
        /// Buffer size for registry storage.
        /// </summary>
        private const int M_RegBufferSize = 900;

        /// <summary>
        /// specify the format string.
        /// </summary>
        private const string FORMAT_STRING = "{0:G}";
        #endregion

        #region Private members
        /// <summary>
        /// Present file name for save in internal isolated stored.
        /// </summary>
        private readonly string m_StoreFileName = AppDomain.CurrentDomain.FriendlyName + ".dat";

        /// <summary>
        /// Stores string representation of the docked elements state,
        /// stored after the initialization of the control.
        /// </summary>
        internal string m_stateDefault = string.Empty;

        ///// <summary>
        ///// Specify the instance of Tabbedelement 
        ///// </summary>
        //internal FrameworkElement m_tabbedelement = null;

        ///// <summary>
        ///// Specify the tabbedhost width
        ///// </summary>
        //internal double m_tabbedhostwidth = 0;

        ///// <summary>
        ///// Specify the tabbedhost height
        ///// </summary>
        //internal double m_tabbedhostheight = 0;

        /// <summary>
        /// Specify the loading state.
        /// </summary>
        internal bool m_loadingState = false;
        /// <summary>
        /// Specifies the first drag element.
        /// </summary>
        internal FrameworkElement firstdragelement = null;
        /// <summary>
        /// specifies  drag element is set or not.
        /// </summary>
        private bool dragelementflag = false;

        /// <summary>
        /// specifies shift
        /// </summary>
        private bool doShift = true;

        /// <summary>
        /// Specifies the side panel mose over
        /// </summary>
        public bool sidepanelmouseover = true;


        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when PersistState property is changed.
        /// </summary>
        public event PropertyChangedCallback PersistStateChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the value of the PersistState dependency
        /// property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Provides PersistState value for the <see cref="DockingManager"/>.
        /// </value>
        /// <remarks>
        /// If PersistState=true then state of DockingManager will be set from isolated storage file when loading.
        /// If PersistState=false then state of DockingManager will be default when loading.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set PersistState in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:Syncfusion="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        ///   <Syncfusion:DockingManager x:Name="dockingManager" PersistState="False" />
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        public bool PersistState
        {
            get
            {
                return (bool)GetValue(PersistStateProperty);
            }

            set
            {
                SetValue(PersistStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore names on deserialize].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [ignore names on deserialize]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreNamesOnDeserialize
        {
            get
            {
                return (bool)GetValue(IgnoreNamesOnDeserializeProperty);
            }
            set
            {
                SetValue(IgnoreNamesOnDeserializeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable optimized KeyHandling Mode while using DocumentContainer
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [ignore names on deserialize]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOptimizedKeyHandling
        {
            get
            {
                return (bool)GetValue(EnableOptimizedKeyHandlingProperty);
            }
            set
            {
                SetValue(EnableOptimizedKeyHandlingProperty, value);
            }
        }

        #endregion

        #region Public methods

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
        /// Saves dock state to the registry.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <property name="flag" value="Finished"/>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( BinaryFormatter serializer ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// dockingManager.SaveDockState( formatter1 );
        /// </code>
        /// </example>
        /// <seealso cref="BinaryFormatter"/>
        public void SaveDockState(BinaryFormatter serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            MemoryStream memStream = new MemoryStream(M_RegBufferSize);
            byte[] byteArr = GetByteData();

            serializer.Serialize(memStream, byteArr);
            SaveToRegistry(memStream);
        }

        /// <summary>
        /// Saves dock state to binary or Xml.
        /// </summary>
        /// <param name="serializer">SoapFormatter or BinaryFormatter
        /// for serialization.</param>
        /// <param name="format">Format to store params.</param>
        /// <param name="path">Path to data file.</param>
        /// <property name="flag" value="Finished"/>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( IFormatter serializer, StorageFormat format, string path ) in C#. BinaryFormatter example.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// dockingManager.SaveDockState( formatter1, StorageFormat.Binary, @"d:\docking_bin.bin" );
        /// </code>
        /// <para/>This example shows how to use SaveDockState( IFormatter serializer, StorageFormat format, string path ) in C#. SoapFormatter example.
        /// <code language="C#">
        /// SoapFormatter formatter1 = new SoapFormatter();
        /// dockingManager.SaveDockState( formatter1, StorageFormat.Binary, @"d:\docking_bin.bin" );
        /// </code>
        /// </example>
        /// <seealso cref="BinaryFormatter"/>
        /// <seealso cref="IFormatter"/>
        /// <seealso cref="StorageFormat"/>
        /// <seealso cref="string"/>
        public void SaveDockState(IFormatter serializer, StorageFormat format, string path)
        {
            if (format == StorageFormat.Xml)
            {
               // RearrangeTabbedElement();
                SaveDockState(path);
            }
            else
            {
                List<DockingParams> dockingParamsList = GetDockingParams();
                DockingParams[] dockingParams = new DockingParams[dockingParamsList.Count];

                dockingParamsList.CopyTo(dockingParams);

                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                serializer.Serialize(fileStream, dockingParams);
                fileStream.Close();
            }
        }
        /// <summary>
        /// Rearrange tab element in correct order
        /// </summary>
        private void RearrangeTabbedElement()
        {
            foreach (FrameworkElement element in Children)
            {
                if (DockingManager.GetSideRelativetoContainer(element) == DockSide.Tabbed)
                {
                    DockedElementTabbedHost tabbed = DockingManager.GetTabbedHost(element, DockState.Float);
                    if (tabbed != null && tabbed.TabChildren.Count>0)
                    {
                        if (element != tabbed.TabParent)
                        {
                            DockingManager.SetTargetNameInFloatingMode(element, tabbed.TabChildren[0].Name);
                            DockingManager.SetSideInFloatMode(element, DockSide.Tabbed);
                        }
                    }

                    DockedElementTabbedHost tabbed1 = DockingManager.GetTabbedHost(element, DockState.Dock);
                    if (tabbed1 != null && tabbed1.TabChildren.Count > 0)
                    {
                        if (element != tabbed1.TabParent)
                        {
                            updatedockflag = false;
                            DockingManager.SetTargetNameInDockedMode(element, tabbed1.TabChildren[0].Name);
                            DockingManager.SetSideInDockedMode(element, DockSide.Tabbed);
                        }
                    }
                    
                }
            }
            RemoveSameTargets();
        }

        /// <summary>
        /// Removes the same targets.
        /// </summary>
        private void RemoveSameTargets()
        {
            foreach (FrameworkElement element in Children)
            {
                updatedockflag = false;
                if (element.Name == DockingManager.GetTargetNameInDockedMode(element))
                {
                    DockingManager.SetTargetNameInDockedMode(element, String.Empty);
                }
                if (element.Name == DockingManager.GetTargetNameInFloatingMode(element))
                {
                    DockingManager.SetTargetNameInFloatingMode(element, String.Empty);
                }
            }
        }        

        /// <summary>
        /// Save state persisted for current docking location (to isolated storage file).
        /// </summary>
        /// <example>
        /// <para/>This example shows how to use SaveDockState() in C#.
        /// <code language="C#">
        /// dockingManager.SaveDockState();
        /// </code>
        /// </example>
        /// <seealso cref="IsolatedStorageFile"/>
        public void SaveDockState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            SaveDockState(isoStorage, m_StoreFileName);
        }

        /// <summary>
        /// Saves state persisted for current docking location using XmlWriter.
        /// </summary>
        /// <param name="writer">Writer for saving.</param>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( XmlWriter writer ) in C#.
        /// <code language="C#">
        /// using( XmlTextWriter writer = new XmlTextWriter( Path.GetFullPath( @"d:\docking_bin.bin" ), Encoding.UTF8 ) )
        /// {
        /// writer.Formatting = Formatting.Indented;
        /// dockingManager.SaveDockState( writer );
        /// writer.Flush();
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="XmlWriter"/>
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
        /// Corrects the target sidesfor tab.
        /// </summary>
        private void CorrectTargetSidesforTab()
        {
            foreach (FrameworkElement element in Children)
            {
                if (DockingManager.GetSideInDockedMode(element) == DockSide.Tabbed)
                {
                    FrameworkElement parent = FindChildSafe(DockingManager.GetTargetNameInDockedMode(element));
                    if (parent != null)
                    {
                        if (DockingManager.GetSideInDockedMode(parent) == DockSide.Tabbed)
                        {
                            DockingManager.SetSideInDockedMode(parent, DockingManager.GetSideInFloatMode(parent));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save state persisted for current docking location using TextWriter.
        /// </summary>
        /// <param name="writer">Writer for save.</param>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( TextWriter writer ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// TextWriter writer = new TextWriter( formatter1 );
        /// dockingManager.SaveDockState( writer );  
        /// </code>
        /// </example>
        /// <seealso cref="TextWriter"/>
        public void SaveDockState(TextWriter writer)
        {
            XmlWriter writerXML = new XmlTextWriter(writer);
            SaveDockState(writerXML);
        }

        /// <summary>
        /// Save state persisted for current docking location.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated store for
        /// save current docking location.</param>
        /// <param name="storeFileName">Present file name for isolated
        /// stored.</param>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( IsolatedStorageFile isoStorage, string storeFileName ) in C#.
        /// <code language="C#">
        /// IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore( IsolatedStorageScope.User
        ///        | IsolatedStorageScope.Assembly, null, null );
        /// dockingManager.SaveDockState( isoStorage, AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".dat" );
        /// </code>
        /// </example>
        /// <seealso cref="IsolatedStorageScope"/>
        /// <seealso cref="IsolatedStorageFile"/>
        /// <seealso cref="string"/>
        public void SaveDockState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !string.IsNullOrEmpty(storeFileName))
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage);

                try
                {
                    //m_tabbedelement = null;
                    //m_tabbedhostheight = 0;
                    //m_tabbedhostwidth = 0;
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
        /// Saves state persisted for current docking location.
        /// </summary>
        /// <param name="path">Present path to *.xml file for saving.</param>
        /// <example>
        /// <para/>This example shows how to use SaveDockState( string path ) in C#.
        /// <code language="C#">
        /// dockingManager.SaveDockState( @"d:\docking_bin.bin" );
        /// </code>
        /// </example>
        /// <seealso cref="string"/>
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
        /// Load state persisted.
        /// </summary>
        /// <example>
        /// <para/>This example shows how to use LoadDockState() in C#.
        /// <code language="C#">
        /// dockingManager.LoadDockState();
        /// </code>
        /// </example>
        public void LoadDockState()
        {
            m_builtindex.Clear();
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            LoadDockState(isoStorage, m_StoreFileName);
            AfterPersistload();
        }
        /// <summary>
        /// Show all float window after load Dockingmanager state
        /// </summary>
        internal void AfterPersistload()
        {
            foreach (var window in this.m_WindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null)
                {
                    window.IsOpen = true;
                }
            }
            foreach (NativeFloatWindow window in this.m_NativeWindowsRegistered)
            {
                if (!window.IsOpen && window.PrimaryElement != null && window.DockingManager.IsVisible)
                {
                    if (!m_NativeWindowsUnRegistered.Contains(window))
                        window.IsOpen = true;
                }
            }
        }
        /// <summary>
        /// Loads state persisted.
        /// </summary>
        /// <param name="reader">Writer for loading.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( TextReader reader ) in C#.
        /// <code language="C#">
        /// using(TextReader reader = new StreamReader( @"d:\docking_bin.bin" ))
        /// {
        ///    dockingManager.LoadDockState( reader );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TextReader"/>
        public void LoadDockState(TextReader reader)
        {
            m_builtindex.Clear();
            XmlTextReader readerXML = new XmlTextReader(reader);
            LoadState(readerXML);
            AfterPersistload();
        }

        /// <summary>
        /// Load state persisted.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated store for
        /// load current docking location.</param>
        /// <param name="storeFileName">Present file name for isolated
        /// store.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( IsolatedStorageFile isoStorage, string storeFileName ) in C#.
        /// <code language="C#">
        /// IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore( IsolatedStorageScope.User
        ///        | IsolatedStorageScope.Assembly, null, null );
        ///   dockingManager.LoadDockState( isoStorage, AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".dat" );
        /// </code>
        /// </example>
        /// <seealso cref="IsolatedStorageScope"/>
        /// <seealso cref="IsolatedStorageFile"/>
        /// <seealso cref="string"/>
        public void LoadDockState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            m_builtindex.Clear();
            if (null != isoStorage && !string.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage);

                try
                {
                    XmlTextReader xmlTextReader = new XmlTextReader(stream);
                    LoadState(xmlTextReader);
                    AfterPersistload();
                }
                catch (SystemException)
                {
                    throw;
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Loads state persisted.
        /// </summary>
        /// <param name="path">Presents path to *.xml file for loading.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( string path ) in C#.
        /// <code language="C#">
        /// dockingManager.LoadDockState( @"d:\docking_bin.bin" );
        /// </code>
        /// </example>
        /// <seealso cref="string"/>
        public void LoadDockState(string path)
        {
            m_builtindex.Clear();
            using (XmlTextReader reader = new XmlTextReader(Path.GetFullPath(path)))
            {
                LoadState(reader);
                AfterPersistload();
            }
        }

        /// <summary>
        /// Loads DockState from registry.
        /// </summary>
        /// <param name="serializer">BinaryFormatter to deserialize.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( BinaryFormatter serializer ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// dockingManager.LoadDockState( formatter1 );
        /// </code>
        /// </example>
        /// <seealso cref="BinaryFormatter"/>
        public void LoadDockState(BinaryFormatter serializer)
        {
            m_builtindex.Clear();
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(C_RegSubKeyName);

            if (regKey != null)
            {
                MemoryStream memStream = new MemoryStream(M_RegBufferSize);

                byte[] byteArr = (byte[])regKey.GetValue(C_RegParamName);

                if (byteArr != null)
                {
                    memStream.Write(byteArr, 0, byteArr.Length);
                    memStream.Position = 0;

                    try
                    {
                        serializer.Deserialize(memStream);
                    }
                    catch (SerializationException)
                    {
                        throw;
                    }

                    LoadState(byteArr);
                    AfterPersistload();
                }
            }
        }

        /// <summary>
        /// Load state persisted for current docking location.
        /// </summary>
        /// <param name="reader">Reader for loading state.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( XmlTextReader reader ) in C#.
        /// <code language="C#">
        /// using( XmlTextReader reader = new XmlTextReader( Path.GetFullPath( @"d:\docking_bin.bin" ) ) )
        ///    {
        ///        dockingManager.LoadDockState( reader );
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="XmlTextReader"/>
        public void LoadDockState(XmlTextReader reader)
        {
            m_builtindex.Clear();
            LoadState(reader);
            AfterPersistload();
        }

        /// <summary>
        /// Iterates the child nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private XmlNode IterateChildNodes(XmlNode node)
        {
            if (node != null && node.ChildNodes.Count > 0)
            {
                if (node.ChildNodes[0].LocalName.Equals("DockingParams"))
                {
                    return node;
                }
                else
                {
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        IterateChildNodes(node.ChildNodes[i] as XmlNode);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Des the serialize.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="dockingParamsList">The docking params list.</param>
        /// <returns></returns>
        private List<DockingParams> DeSerialize(XmlNode node, List<DockingParams> dockingParamsList)
        {
            if (node != null && node.ChildNodes.Count > 0)
            {
                XmlSerializer serializer = CreateDefaultXmlSerializer(typeof(List<DockingParams>));
                MemoryStream stream = new MemoryStream();
                StreamWriter writerstream = new StreamWriter(stream);
                try
                {
                    writerstream.Write(node.OuterXml);
                    writerstream.Flush();
                    stream.Position = 0;
                    dockingParamsList = (List<DockingParams>)serializer.Deserialize(stream);
                }
                catch { }
                finally
                {
                    stream.Close();
                    writerstream.Close();
                }
            }
            return dockingParamsList;
        }

        /// <summary>
        /// Loads the state of the dock.
        /// </summary>
        /// <param name="node">The node.</param>
        public void LoadDockState(XmlNode node)
        {
            m_builtindex.Clear();
            try
            {
                List<DockingParams> dockingParamsList = new List<DockingParams>();
                XmlNode paramsnode = IterateChildNodes(node);
                if (paramsnode != null)
                {
                    dockingParamsList = DeSerialize(paramsnode, dockingParamsList);
                }
                if (dockingParamsList.Count > 0)
                {
                    ResetDocking();
                    LockPropertyChangedAction = true;
                    RectConverter converter = new RectConverter();
                    List<FrameworkElement> inElements = new List<FrameworkElement>();
                    List<FrameworkElement> outElements = GetCopyThisChildren();
                    if (IgnoreNamesOnDeserialize)
                    {
                        ResolveConflict();
                        MapSavedName(dockingParamsList);
                    }
                    //m_tabbedelement = null;
                    //m_tabbedhostheight = 0;
                    //m_tabbedhostwidth = 0;
                    foreach (DockingParams dockingParams in dockingParamsList)
                    {
                        if (IgnoreNamesOnDeserialize)
                        {
                            MapTargetsName(dockingParams);
                        }
                        LoadState(dockingParams, inElements, outElements, converter);
                    }

                    sidepanelmouseover = false;

                    if (dockingParamsList.Count > 0)
                    {
                        dockingParamsList.Sort(new Comparison<DockingParams>((x, y) => x.IndexInDockMode.CompareTo(y.IndexInDockMode)));

                        if (dockingParamsList[0].IndexInDockMode == -1 && dockingParamsList[0].DoShift)
                        {
                            int maxIndex = dockingParamsList[dockingParamsList.Count - 1].IndexInDockMode;
                            foreach (DockingParams param in dockingParamsList)
                            {
                                FrameworkElement child = FindChildSafe(param.Name);
                                DockingManager.SetIndexInDockMode(child, ++maxIndex);
                                if (param.TargetDocked == String.Empty)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    foreach (DockingParams param in dockingParamsList)
                    {
                        FrameworkElement child = FindChildSafe(param.Name);
                        if (child != null)
                        {
                            List<String> childList = DockingManager.GetPreviousChildElements(child);
                            if (childList != null && childList.Count > 0)
                            {
                                GetTempTargetForAutoHide(child);
                            }
                        }
                    }


                    // AssignCorrectTargets();
                    DirectTabPanel.m_nameCreator = 0;
                    TDILayoutPanel.m_nameSufix = 0;

                    //AddListsToChildren(inElements, outElements);
                    LockPropertyChangedAction = false;
                    LockLayoutUpdate = false;
                    m_loadingState = true;
                    UpdateLayout();
                    foreach (DockingParams param in dockingParamsList)
                    {
                        if (param.IsSelectedTab && param.State != DockState.Hidden && param.State != DockState.AutoHidden)
                        {
                            ActivateWindow(param.Name);
                        }
                    }

                    foreach (DockingParams param in dockingParamsList)
                    {
                        if (param.IsActiveWindow && param.State != DockState.Hidden)
                        {
                            if (param.State == DockState.AutoHidden)
                            {
                                m_StopFireShow = true;
                            }
                            else
                            {
                                ActivateWindow(param.Name);
                            }
                            m_StopFireShow = false;
                        }
                    }
                    LockLayoutUpdate = true;
                }
                AfterPersistload();
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

        /// <summary>
        /// Load state persisted for current docking location.
        /// </summary>
        /// <param name="reader">Reader for loading state.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( XmlReader reader ) in C#.
        /// <code language="C#">
        /// using( XmlReader reader = XmlReader.Create( Path.GetFullPath( @"d:\docking_bin.bin" ) ) )
        ///    {
        ///        dockingManager.LoadDockState( reader );
        ///    }
        /// </code>
        /// </example>
        /// <seealso cref="XmlReader"/>
        public void LoadDockState(XmlReader reader)
        {
            m_builtindex.Clear();
            LoadState(reader);
            AfterPersistload();
        }

        /// <summary>
        /// Loads DockState from binary or xml file.
        /// </summary>
        /// <param name="serializer">SoapFormatter or BinaryFormatter to
        /// deserialize.</param>
        /// <param name="format">Format to store dock parameters.</param>
        /// <param name="path">File path.</param>
        /// <example>
        /// <para/>This example shows how to use LoadDockState( IFormatter serializer, StorageFormat format, string path ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// dockingManager.LoadDockState( formatter1, StorageFormat.Xml, @"d:\docking_xml.xml" );
        /// </code>
        /// </example>
        /// <seealso cref="IFormatter"/>
        /// <seealso cref="StorageFormat"/>
        /// <seealso cref="string"/>
        public void LoadDockState(IFormatter serializer, StorageFormat format, string path)
        {
            m_builtindex.Clear();
            if (File.Exists(path))
            {
                if (format == StorageFormat.Xml)
                {
                    LoadDockState(path);
                }
                else
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                    try
                    {
                        serializer.Deserialize(fileStream);
                        fileStream.Close();

                        Dictionary<object, object> dockParamsTable = ParamsTable.Params;
                        LoadState(dockParamsTable);
                        dockParamsTable.Clear();
                    }
                    catch (SerializationException)
                    {
                        throw;
                    }
                    catch (XmlException)
                    {
                        throw;
                    }
                }
                AfterPersistload();
            }
        }

        /// <summary>
        /// Resets docking state.
        /// </summary>
        /// <returns>true if succeeded, false otherwise.</returns>
        /// <example>
        /// <para/>This example shows how to use ResetState() in C#.
        /// <code language="C#">
        /// dockingManager.ResetState();
        /// </code>
        /// </example>
        public bool ResetState()
        {
            bool result = false;

            if (!String.IsNullOrEmpty(m_stateDefault))
            {
                m_builtindex.Clear();
                StringReader readerStr = new StringReader(m_stateDefault);
                XmlTextReader reader = new XmlTextReader(readerStr);
                result = LoadState(reader);
                reader.Close();
            }

            return result;
        }

        public List<string> GetPropertyList(DockingParams obj)
        {
            PropertyInfo[] propertyinfo = obj.GetType().GetProperties();
            List<string> properties = new List<string>();
            foreach (var property in propertyinfo)
            {
                switch (property.Name)
                {
                    case "SideDocked":
                        properties.Add("SideInDockedMode");
                        break;
                    case "SideFloat":
                        properties.Add("SideInFloatMode");
                        break;
                    case "TargetDocked":
                        properties.Add("TargetNameInDockedMode");
                        break;
                    case "TargetFloat":
                        properties.Add("TargetNameInFloatingMode");
                        break;
                    case "TargetAutoHide":
                        properties.Add("TargetNameInAutoHideMode");
                        break;
                    case "TabOrderIndex":
                        properties.Add("DocumentTabOrderIndex");
                        break;
                    case "SideTabOrder":
                        properties.Add("TabChildOrder");
                        break;
                    default:
                        properties.Add(property.Name);
                        break;
                }
            }
            return properties;
        }

        public void ClearState()
        {
            m_onStatechange = true;
            List<string> propertieslist = GetPropertyList(new DockingParams());
            foreach (DependencyObject element in Children)
            {
                LocalValueEnumerator locallySetProperties = element.GetLocalValueEnumerator();
                while (locallySetProperties.MoveNext())
                {
                    DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                    if (!propertyToClear.ReadOnly && propertieslist.Contains(propertyToClear.Name) && propertyToClear.Name != "Name")
                    {
                        element.ClearValue(propertyToClear);
                    }
                }
            }
            int i = 0;
            foreach (DependencyObject element in Children)
            {
                DockingManager.SetIndexInDockMode(element, i);
                DockingManager.SetIndexInFloatMode(element, i);
                i++;
            }
            maximizedelement = null;
            m_onStatechange = false;
            LockLayoutUpdate = false;
            UpdateLayout();
        }

        /// <summary>
        /// Deletes file where dock state was saved.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <example>
        /// <para/>This example shows how to use DeleteDockState( string path ) in C#.
        /// <code language="C#">
        /// dockingManager.DeleteDockState( @"d:\docking_xml.xml" );
        /// </code>
        /// </example>
        /// <seealso cref="string"/>
        public void DeleteDockState(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Deletes dock state parameters from registry.
        /// </summary>
        /// <example>
        /// <para/>This example shows how to use DeleteDockState() in C#.
        /// <code language="C#">
        /// dockingManager.DeleteDockState();
        /// </code>
        /// </example>
        public void DeleteDockState()
        {
            RegistryKey regKey = Registry.CurrentUser;

            if (regKey.OpenSubKey(C_RegSubKeyName) != null)
            {
                regKey.DeleteSubKeyTree(C_RegSubKeyName);
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
        /// Gets the is state loaded.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        internal static bool GetIsStateLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsStateLoadedProperty);
        }

        /// <summary>
        /// Sets the is state loaded.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal static void SetIsStateLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(IsStateLoadedProperty, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets dependency properties for serialization.
        /// </summary>
        /// <returns>
        /// List of dependency properties to serialize.
        /// </returns>
        internal static List<DependencyProperty> GetListSerializedProperties()
        {
            List<DependencyProperty> returnList = new List<DependencyProperty>
            {
                DesiredHeightInDockedModeProperty, 
                DesiredHeightInFloatingModeProperty, 
                DesiredWidthInDockedModeProperty,
                DesiredWidthInFloatingModeProperty,
                NameProperty,
                StateProperty,
                NoDockProperty,
                CanDockProperty,
                CanCloseProperty,
                IsSelectedTabProperty,
                IsActiveWindowProperty,
                SideInDockedModeProperty,
                SideInFloatModeProperty,
                TargetNameInDockedModeProperty,
                TargetNameInFloatingModeProperty,
                FloatingWindowRectProperty,
                SidePanel.TabGroupNameProperty,
                SidePanel.IsTabGroupOwnerProperty,
                SidePanel.TabChildOrderProperty,
                IndexInDockModeProperty,
                IndexInFloatModeProperty,
                DockedElementTabbedHost.TabOrderInDockModeProperty,
                DockedElementTabbedHost.TabOrderInFloatModeProperty,
                DocumentContainer.MDIBoundsProperty,
                DocumentContainer.MDIMinimizedBoundsProperty,
                DocumentContainer.MDIWindowStateProperty,
                DocumentContainer.AllowMDIResizeProperty,
                TDILayoutPanel.TDIIndexProperty,
                TDILayoutPanel.IsSelectedProperty,
                TDILayoutPanel.TDIGroupOrientationProperty,
                TDILayoutPanel.WayOfTDIGroupProperty,
                TDILayoutPanel.SplitPanelOffsetProperty
            };

            return returnList;
        }

        /// <summary>
        /// Saves default state of the docking window.
        /// </summary>
        protected virtual void SaveDefaultState()
        {
            StringWriter writerStr = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(writerStr);
            CorrectTargetSidesforTab();
            SaveDockState(writer);
            writer.Close();

            m_stateDefault = writerStr.ToString();
        }

        /// <summary>
        /// Updates property value cache and raises PersistStateChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnPersistStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PersistStateChanged != null)
            {
                PersistStateChanged(this, e);
            }
        }

        /// <summary>
        /// Gets copy of children elements of this class.
        /// </summary>
        /// <returns>list of children</returns>
        private List<FrameworkElement> GetCopyThisChildren()
        {
            List<FrameworkElement> returnList = new List<FrameworkElement>(Children.Count);

            foreach (FrameworkElement element in Children)
            {
                returnList.Add(element);
            }

            return returnList;
        }

        /// <summary>
        /// Updates the inner dock elements.
        /// </summary>
        private void UpdateInnerDockElements()
        {
            ///This method has been added for MT2257, 91433, Mt2246, MT2269, Mt2270, 92396 - Component Swapping issue 

            List<string> parentnames = new List<string>();
            foreach (FrameworkElement childelement in Children)
            {
                string targetname = DockingManager.GetTargetNameInDockedMode(childelement);
                if (!parentnames.Contains(targetname) && InnerDockElements[targetname] == null && targetname != string.Empty)
                    parentnames.Add(targetname);
            }
            foreach (string targetname in parentnames)
            {
                List<int> indexes = new List<int>();
                List<FrameworkElement> childcollection = new List<FrameworkElement>();
                List<string> innerdockelements = new List<string>();
                foreach (FrameworkElement child in Children)
                {
                    if (DockingManager.GetTargetNameInDockedMode(child).Equals(targetname))
                    {
                        childcollection.Add(child);
                        indexes.Add(DockingManager.GetIndexInDockMode(child));
                    }
                }
                indexes.Sort();
                foreach (int index in indexes)
                {
                    foreach (FrameworkElement element in childcollection)
                    {
                        if (index.Equals(DockingManager.GetIndexInDockMode(element)))
                            innerdockelements.Add(element.Name);
                    }
                }
                if (InnerDockElements[targetname] == null)
                    InnerDockElements.Add(targetname, innerdockelements);
            }
        }

        /// <summary>
        /// Adds list of elements to children collection of the docking manager.
        /// </summary>
        /// <param name="list">The FrameworkElement list.</param>
        private void AddListToChildren(IEnumerable<FrameworkElement> list)
        {
            foreach (FrameworkElement element in list)
            {
                Children.InternalAdd(element);
            }
        }

        /// <summary>
        /// Adds inner and outer elements to children of this class.
        /// </summary>
        /// <param name="inElements">inner elements</param>
        /// <param name="outElements">outer elements</param>
        private void AddListsToChildren(IEnumerable<FrameworkElement> inElements, IEnumerable<FrameworkElement> outElements)
        {
            Children.Clear();
            AddListToChildren(inElements);
            AddListToChildren(outElements);
        }

        /// <summary>
        /// Writes data of the docking window to XmlWriter
        /// </summary>
        /// <param name="writer">writer to write</param>
        private void WriteDateToWriter(XmlWriter writer)
        {
            List<DockingParams> dockingParamsList = GetDockingParams();
            XmlSerializer serializer = CreateDefaultXmlSerializer(typeof(List<DockingParams>));
            serializer.Serialize(writer, dockingParamsList);
        }

        /// <summary>
        /// Predicts the first element drag.
        /// </summary>
        private void PredictFirstElementDrag()
        {
            if (dragged)
            {
                if (firstdragelement != null && firstdragelement == Children[0])
                {
                    doShift = false;
                }
                else
                {
                    doShift = true;
                }
                dragged = false;
            }
            else
            {
                doShift = false;
                
            }
        }


        /// <summary>
        /// Adds docking parameters to byte array.
        /// </summary>
        /// <returns>Byte array of docking parameters.</returns>
        /// <property name="flag" value="Finished"/>
        private byte[] GetByteData()
        {
            MemoryStream memStream = new MemoryStream(M_RegBufferSize);
            PredictFirstElementDrag();

            foreach (FrameworkElement element in Children)
            {
                DockingParams dockingParams = new DockingParams(element,doShift);
                WriteDockParams(memStream, dockingParams);
            }

            memStream.Seek(0, SeekOrigin.Begin);
            long memStreamLen = memStream.Length;

            byte[] byteArray = new byte[memStreamLen];
            Array.Copy(memStream.ToArray(), byteArray, memStreamLen);

            return byteArray;
        }

        /// <summary>
        /// Adds docking params to Hash table
        /// </summary>
        /// <returns>Hash table of docking params.</returns>
        /// <property name="flag" value="Finished"/>
        private List<DockingParams> GetDockingParams()
        {
            if (IgnoreNamesOnDeserialize)
            {
                ResolveConflict();
            }
            List<DockingParams> dockingParams = new List<DockingParams>();
            PredictFirstElementDrag();
            SetSplitPanelOffset();
            foreach (FrameworkElement element in Children)
            {
                dockingParams.Add(new DockingParams(element,doShift));
            }

            return dockingParams;
        }

        private FrameworkElement FindElementInMapping(string name)
        {
            string result = null;
            nameMappings.TryGetValue(name, out result);
            if (result != null)
            {
                return FindChildSafe(result);
            }
            return null;
        }

        private void SetSplitPanelOffset()
        {
            foreach (FrameworkElement element in Children)
            {
                TDISplitPanel splitpanel = (TDISplitPanel)VisualUtils.FindAncestor(element, typeof(TDISplitPanel));
                if (splitpanel != null)
                {
                    element.SetValue(TDILayoutPanel.SplitPanelOffsetProperty, splitpanel.Offset);
                }
            }
        }

        private void ValidateTargetNames(FrameworkElement element, string targetName)
        {
            FrameworkElement targetelement = FindChild(targetName);
            if (targetelement != null)
            {
                if ((element as FrameworkElement).Name == DockingManager.GetTargetNameInDockedMode(targetelement)
                    && DockingManager.GetSideInDockedMode(element) != DockSide.Tabbed)
                {
                    DockingManager.SetTargetNameInDockedMode(element, string.Empty);
                }
            }
        }

        /// <summary>
        /// Loading dock state for xml, registry, binary formats.
        /// </summary>
        /// <param name="dockingParams">The docking params.</param>
        /// <param name="inElements">List of input elements.</param>
        /// <param name="outElements">List of output elements.</param>
        /// <param name="converter">RectConverter to convert
        /// WindowRect parameter.</param>
        /// <property name="flag" value="Finished"/>
        private void LoadState(DockingParams dockingParams, ICollection<FrameworkElement> inElements, ICollection<FrameworkElement> outElements, TypeConverter converter)
        {
            if (dockingParams.State == DockState.Document)
                return;

            m_loadingState = true;
            updatedockflag = false;
            FrameworkElement element = FindChildSafe(dockingParams.Name);

            if (element == null && IgnoreNamesOnDeserialize)
            {
                element = FindElementInMapping(dockingParams.Name);
            }

            if (element != null)
            {
                string rectWindow = string.Empty;
                if (dockingParams.WindowRect != null)
                {
                    rectWindow = dockingParams.WindowRect;
                }

                outElements.Remove(element);
                inElements.Add(element);
                DockingManager.SetNoHeader(element, dockingParams.NoHeader);
                DockingManager.SetState(element, dockingParams.State);
                DockingManager.SetNoDock(element, dockingParams.NoDock);
                DockingManager.SetCanDock(element, dockingParams.CanDock);
                DockingManager.SetSidePanelDock(element, dockingParams.DockForSide);
                DockingManager.SetIsSelectedTab(element, dockingParams.IsSelectedTab);
                DockingManager.SetSideInDockedMode(element, dockingParams.SideDocked);
                DockingManager.SetSideInFloatMode(element, dockingParams.SideFloating);
                DockingManager.SetTargetNameInDockedMode(element, ValidateTargetNames(dockingParams.TargetDocked));
                DockingManager.SetTargetNameInAutoHideMode(element, dockingParams.TargetAutoHide);
                DockingManager.SetTargetNameInFloatingMode(element, ValidateTargetNames(dockingParams.TargetFloating));
                if (Double.IsNaN(dockingParams.DesiredWidthInDockedMode))
                {
                    DockingManager.SetDesiredWidthInDockedMode(element, 90d);
                }
                else
                {
                    DockingManager.SetDesiredWidthInDockedMode(element, dockingParams.DesiredWidthInDockedMode);
                }
                if (Double.IsNaN(dockingParams.DesiredHeightInDockedMode))
                {
                    DockingManager.SetDesiredHeightInDockedMode(element, 90d);
                }
                else
                {
                     DockingManager.SetDesiredHeightInDockedMode(element, dockingParams.DesiredHeightInDockedMode);
                }
                DockingManager.SetDesiredWidthInFloatingMode(element, dockingParams.DesiredWidthInFloatingMode);
                DockingManager.SetDesiredHeightInFloatingMode(element, dockingParams.DesiredHeightInFloatingMode);
                DockingManager.SetPreviousContainerDesiredSize(element, dockingParams.PreviousContainerDesiredSize);
                DockingManager.SetDockedElementsContainerDesiredSize(element, dockingParams.ContainerSize);
                SidePanel.SetTabGroupName(element, dockingParams.TabGroupName);
                SidePanel.SetIsTabGroupOwner(element, dockingParams.IsTabGroupOwner);
                SidePanel.SetTabChildOrder(element, dockingParams.SideTabOrder);
                DockingManager.SetIndexInDockMode(element, dockingParams.IndexInDockMode);

                DockingManager.SetPreviousIndexInDockMode(element, dockingParams.PreviousIndexInDockMode);
                DockingManager.SetPreviousChildElements(element, dockingParams.PreviousChildElements);
                DockingManager.SetPreviousSideInDockMode(element, dockingParams.PreviousSideInDockMode);
                DockingManager.SetSideRelativetoContainer(element, dockingParams.SideRelativetoContainer);
                DockingManager.SetTabParent(element, dockingParams.TabParent);

                DockingManager.SetIndexInFloatMode(element, dockingParams.IndexInFloatMode);
                DockedElementTabbedHost.SetTabOrderInDockMode(element, dockingParams.TabOrderInDockMode);
                DockedElementTabbedHost.SetTabOrderInFloatMode(element, dockingParams.TabOrderInFloatMode);
                DockingManager.SetFloatingWindowRect(element, GetRectFromStr(rectWindow, converter));
                DockingManager.SetPreviousFloatingWindowRect(element,GetRectFromStr(dockingParams.PreviousFloatingWindowRect,converter));
                DockingManager.SetFloatWindowState(element,dockingParams.FloatWindowState);
                DocumentContainer.SetMDIBounds(element, GetRectFromStr(dockingParams.MDIBounds, converter));
                DocumentContainer.SetMDIMinimizedBounds(element, GetRectFromStr(dockingParams.MDIMinimizedBounds, converter));
                DocumentContainer.SetMDIWindowState(element, dockingParams.MDIWindowState);
                DocumentContainer.SetAllowMDIResize(element, dockingParams.AllowMDIResize);
                element.SetValue(TDILayoutPanel.TDIIndexProperty, dockingParams.TDIIndex);
                TDILayoutPanel.SetIsSelected(element, dockingParams.IsSelected);
                element.SetValue(TDILayoutPanel.TDIGroupOrientationProperty, dockingParams.TDIGroupOrientation);
                element.SetValue(TDILayoutPanel.WayOfTDIGroupProperty, dockingParams.WayOfTDIGroup);
                element.SetValue(TDILayoutPanel.SplitPanelOffsetProperty, dockingParams.SplitPanelOffset);
                DockingManager.SetCanMaximize(element, dockingParams.CanMaximize);
                DockingManager.SetCanMinimize(element, dockingParams.CanMinimize);
                DockingManager.SetDockWindowState(element, dockingParams.DockWindowState);
                if (dockingParams.DockWindowState == WindowState.Maximized)
                {
                    maximizedelement = element;
                }
                DockingManager.SetCanResizeInDockedState(element, dockingParams.CanResizeInDockedState);
                DockingManager.SetCanResizeInFloatState(element, dockingParams.CanResizeInFloatState);
                DockingManager.SetCanResizeHeightInDockedState(element, dockingParams.CanResizeHeightInDockedState);
                DockingManager.SetCanResizeWidthInDockedState(element, dockingParams.CanResizeWidthInDockedState);
                DockingManager.SetCanResizeHeightInFloatState(element, dockingParams.CanResizeHeightInFloatState);
                DockingManager.SetCanResizeWidthInFloatState(element, dockingParams.CanResizeWidthInFloatState);
                DockingManager.SetCanFloatMaximize(element, dockingParams.CanFloatMaximize);
                DockingManager.SetIsFixedSize(element, dockingParams.IsFixedSize);
                DockingManager.SetIsFixedHeight(element, dockingParams.IsFixedHeight);
                DockingManager.SetIsFixedWidth(element, dockingParams.IsFixedWidth);
                DockingManager.SetFixedHeight(element, dockingParams.FixedHeight);
                DockingManager.SetFixedWidth(element, dockingParams.FixedWidth);
                DockingManager.SetPreviousDesiredHeightInDockedMode(element, dockingParams.PreviousDesiredHeightInDockedMode);
                DockingManager.SetPreviousDesiredWidthInDockedMode(element, dockingParams.PreviousDesiredWidthInDockedMode);
                DockedElementsContainer.SetPreviousContainerHeight(element, dockingParams.PreviousContainerHeight);
                DockedElementsContainer.SetPreviousContainerWidth(element, dockingParams.PreviousContainerWidth);
                DockedElementTabbedHost.SetPreviousHostHeight(element, dockingParams.PreviousHostHeight);
                DockedElementTabbedHost.SetPreviousHostWidth(element, dockingParams.PreviousHostWidth);
                DockingManager.SetIsSwapped(element, dockingParams.IsSwapped);
                DockingManager.SetDocumentTabOrderIndex(element, dockingParams.TabOrderIndex);
                DockingManager.SetZorderInFloatMode(element,dockingParams.ZorderInFloatMode);
                //if (dockingParams.IsActiveWindow)
                //{
                //    DockingManager.SetNewFocusedElement(element);
                //}
            }
            updatedockflag = true;
            m_loadingState = false;
        }

        private void SetName(string name)
        {
            foreach (FrameworkElement element in Children)
            {
                element.Name = name;
            }
        }
        
        /// <summary>
        /// Loads state persisted (binary).
        /// </summary>
        /// <param name="dockParamsTable">The dock params table.</param>
        /// <property name="flag" value="Finished"/>
        private void LoadState(IEnumerable<KeyValuePair<object, object>> dockParamsTable)
        {
            if (dockParamsTable == null)
            {
                throw new ArgumentNullException("dockParamsTable");
            }

            RectConverter converter = new RectConverter();

            List<FrameworkElement> inElements = new List<FrameworkElement>();
            List<FrameworkElement> outElements = GetCopyThisChildren();
            ResetDocking();
            LockPropertyChangedAction = true;

            try
            {
                foreach (KeyValuePair<object, object> elementPairData in dockParamsTable)
                {
                    Hashtable subtable = (Hashtable)elementPairData.Value;
                    Dictionary<string, string> keyVal = new Dictionary<string, string>();
                    CultureInfo info = new CultureInfo(string.Empty);

                    foreach (DictionaryEntry pair in subtable)
                    {
                        string key = string.Format(info, FORMAT_STRING, pair.Key);
                        string value = string.Format(info, FORMAT_STRING, pair.Value);
                        keyVal.Add(key, value);
                    }
                    CultureInfo cul = CultureInfo.InvariantCulture;

                    DockingParams dockParams = new DockingParams
                    {
                        Name = keyVal[C_NAME],
                        State = (DockState)Enum.Parse(typeof(DockState), keyVal[C_STATE]),
                        SideDocked = (DockSide)Enum.Parse(typeof(DockSide), keyVal[C_SIDEDocked]),
                        SideFloating = (DockSide)Enum.Parse(typeof(DockSide), keyVal[C_SIDEFloating]),
                        TargetDocked = keyVal[C_TARGETDocked],
                        TargetFloating = keyVal[C_TARGETFloating],
                        WindowRect = keyVal[C_WindowRect],
                        DesiredHeightInDockedMode = double.Parse(keyVal[C_DesiredHeightInDockedMode],cul),
                        DesiredHeightInFloatingMode = double.Parse(keyVal[C_DesiredHeightInFloatingMode],cul),
                        DesiredWidthInDockedMode = double.Parse(keyVal[C_DesiredWidthInDockedMode],cul),
                        DesiredWidthInFloatingMode = double.Parse(keyVal[C_DesiredWidthInFloatingMode],cul),
                        NoDock = bool.Parse(keyVal[C_NODock]),
                        CanDock = bool.Parse(keyVal[C_CANDock]),
                        IsSelectedTab = bool.Parse(keyVal[C_ISSelectedTab]),
                        IsActiveWindow = bool.Parse(keyVal[C_ISActiveWindow]),
                        TabGroupName = keyVal[C_TabGroupName],
                        IsTabGroupOwner = bool.Parse(keyVal[C_ISTabGroupOwner]),
                        SideTabOrder = int.Parse(keyVal[C_SideTabOrder]),
                        IndexInDockMode = int.Parse(keyVal[C_IndexInDockMode]),
                        IndexInFloatMode = int.Parse(keyVal[C_IndexInFloatMode]),
                        TabOrderInDockMode = int.Parse(keyVal[C_TabOrderInDockMode]),
                        TabOrderInFloatMode = int.Parse(keyVal[C_TabOrderInFloatMode]),
                        AllowMDIResize = bool.Parse(keyVal[C_AllowMDIResize]),
                        CanClose = bool.Parse(keyVal[C_CANClose]),
                        IsSelected = bool.Parse(keyVal[C_IsSelected]),
                        TDIIndex = int.Parse(keyVal[C_TDIIndex]),
                        SplitPanelOffset = double.Parse(keyVal[C_TDISplitPanelOffset]),
                        MDIWindowState =
                            (MDIWindowState)Enum.Parse(typeof(MDIWindowState), keyVal[C_MDIWindowState]),
                        TDIGroupOrientation =
                            (Orientation)Enum.Parse(typeof(Orientation), keyVal[C_TDIGroupOrientation]),
                        MDIBounds = keyVal[C_MDIBounds],
                        MDIMinimizedBounds = keyVal[C_MDIMinimizedBounds],
                        WayOfTDIGroup = keyVal[C_WayOfTDIGroup]
                    };

                    LoadState(dockParams, inElements, outElements, converter);
                }
            }
            catch (ArgumentException ex)
            {
                throw new XmlException("Data were parsed incorrect", ex);
            }

            //AddListsToChildren(inElements, outElements);
            LockPropertyChangedAction = false;
            LockLayoutUpdate = false;
            m_loadingState = true;
            DockingManager.UpdateLayout(this);
        }

        /// <summary>
        /// Loads state persisted (Registry).
        /// </summary>
        /// <param name="byteArr">The byte arr.</param>
        /// <property name="flag" value="Finished"/>
        private void LoadState(byte[] byteArr)
        {
            if (byteArr == null)
            {
                throw new ArgumentNullException("byteArr");
            }

            char[] ch = Encoding.UTF8.GetChars(byteArr);
            string endStr = new string(ch);
            string[] kvPairs = endStr.Split(';');
            int iterationsCnt = kvPairs.Length / C_DockingParametersCount;
            RectConverter converter = new RectConverter();
            List<FrameworkElement> inElements = new List<FrameworkElement>();
            List<FrameworkElement> outElements = GetCopyThisChildren();
            ResetDocking();
            LockPropertyChangedAction = true;

            try
            {
                CultureInfo culture = CultureInfo.InvariantCulture;
                for (int i = 0; i < iterationsCnt; i++)
                {
                    DockingParams dockingParams = new DockingParams
                    {
                        Name = GetParamFromStr(C_NAME, ref endStr),
                        State = (DockState)Enum.Parse(typeof(DockState), GetParamFromStr(C_STATE, ref endStr)),
                        NoDock = bool.Parse(GetParamFromStr(C_NODock, ref endStr)),
                        CanDock = bool.Parse(GetParamFromStr(C_CANDock, ref endStr)),
                        CanClose = bool.Parse(GetParamFromStr(C_CANClose, ref endStr)),
                        IsSelectedTab = bool.Parse(GetParamFromStr(C_ISSelectedTab, ref endStr)),
                        IsActiveWindow = bool.Parse(GetParamFromStr(C_ISActiveWindow, ref endStr)),
                        SideDocked = (DockSide)Enum.Parse(typeof(DockSide), GetParamFromStr(C_SIDEDocked, ref endStr)),
                        SideFloating = (DockSide)Enum.Parse(typeof(DockSide), GetParamFromStr(C_SIDEFloating, ref endStr)),
                        TargetDocked = GetParamFromStr(C_TARGETDocked, ref endStr),
                        TargetFloating = GetParamFromStr(C_TARGETFloating, ref endStr),
                        DesiredHeightInDockedMode = double.Parse(GetParamFromStr(C_DesiredHeightInDockedMode, ref endStr), culture),
                        DesiredHeightInFloatingMode = double.Parse(GetParamFromStr(C_DesiredHeightInFloatingMode, ref endStr), culture),
                        DesiredWidthInDockedMode = double.Parse(GetParamFromStr(C_DesiredWidthInDockedMode, ref endStr), culture),
                        DesiredWidthInFloatingMode = double.Parse(GetParamFromStr(C_DesiredWidthInFloatingMode, ref endStr), culture),
                        WindowRect = GetParamFromStr(C_WindowRect, ref endStr),
                        TabGroupName = GetParamFromStr(C_TabGroupName, ref endStr),
                        IsTabGroupOwner = bool.Parse(GetParamFromStr(C_ISTabGroupOwner, ref endStr)),
                        SideTabOrder = int.Parse(GetParamFromStr(C_SideTabOrder, ref endStr)),
                        IndexInDockMode = int.Parse(GetParamFromStr(C_IndexInDockMode, ref endStr)),
                        IndexInFloatMode = int.Parse(GetParamFromStr(C_IndexInFloatMode, ref endStr)),
                        TabOrderInDockMode = int.Parse(GetParamFromStr(C_TabOrderInDockMode, ref endStr)),
                        TabOrderInFloatMode = int.Parse(GetParamFromStr(C_TabOrderInFloatMode, ref endStr)),
                        MDIBounds = GetParamFromStr(C_MDIBounds, ref endStr),
                        MDIMinimizedBounds = GetParamFromStr(C_MDIMinimizedBounds, ref endStr),
                        MDIWindowState = (MDIWindowState)Enum.Parse(typeof(MDIWindowState), GetParamFromStr(C_MDIWindowState, ref endStr)),
                        AllowMDIResize = bool.Parse(GetParamFromStr(C_AllowMDIResize, ref endStr)),
                        TDIIndex = int.Parse(GetParamFromStr(C_TDIIndex, ref endStr)),
                        IsSelected = bool.Parse(GetParamFromStr(C_IsSelected, ref endStr)),
                        TDIGroupOrientation = (Orientation)Enum.Parse(typeof(Orientation), GetParamFromStr(C_TDIGroupOrientation, ref endStr)),
                        WayOfTDIGroup = GetParamFromStr(C_WayOfTDIGroup, ref endStr),
                        SplitPanelOffset = double.Parse(GetParamFromStr(C_TDISplitPanelOffset, ref endStr)),
                        CanMaximize = bool.Parse(GetParamFromStr(C_CanMaximize, ref endStr)),
                        CanMinimize = bool.Parse(GetParamFromStr(C_CanMinimize, ref endStr)),
                        DockWindowState = (WindowState)Enum.Parse(typeof(WindowState), GetParamFromStr(C_DockWindowState, ref endStr)),
                        CanResizeInDockedState = bool.Parse(GetParamFromStr(C_CanResizeInDockedState, ref endStr)),
                        CanResizeInFloatState = bool.Parse(GetParamFromStr(C_CanResizeInFloatState, ref endStr)),
                        CanResizeHeightInDockedState = bool.Parse(GetParamFromStr(C_CanResizeHeightInDockedState, ref endStr)),
                        CanResizeWidthInDockedState = bool.Parse(GetParamFromStr(C_CanResizeWidthInDockedState, ref endStr)),
                        CanResizeWidthInFloatState = bool.Parse(GetParamFromStr(C_CanResizeWidthInFloatState, ref endStr)),
                        CanResizeHeightInFloatState = bool.Parse(GetParamFromStr(C_CanResizeHeightInFloatState, ref endStr)),
                        CanFloatMaximize = bool.Parse(GetParamFromStr(C_CanFloatMaximize, ref endStr)),
                        IsFixedSize = bool.Parse(GetParamFromStr(C_IsFixedSize, ref endStr)),
                        IsFixedHeight = bool.Parse(GetParamFromStr(C_IsFixedHeight, ref endStr)),
                        IsFixedWidth = bool.Parse(GetParamFromStr(C_IsFixedWidth, ref endStr)),
                        FixedHeight = double.Parse(GetParamFromStr(C_FixedHeight, ref endStr)),
                        FixedWidth = double.Parse(GetParamFromStr(C_FixedWidth, ref endStr)),
                        PreviousContainerHeight = double.Parse(GetParamFromStr(C_PreviousContainerHeight, ref endStr)),
                        PreviousContainerWidth = double.Parse(GetParamFromStr(C_PreviousContainerWidth, ref endStr)),
                        PreviousHostHeight = double.Parse(GetParamFromStr(C_PreviousHostHeight, ref endStr)),
                        PreviousHostWidth = double.Parse(GetParamFromStr(C_PreviousHostWidth, ref endStr)),
                        IsSwapped = bool.Parse(GetParamFromStr(C_IsSwapped, ref endStr)),
                        ZorderInFloatMode = int.Parse(GetParamFromStr(C_ZorderInFloatMode,ref endStr)),
                        PreviousFloatingWindowRect = GetParamFromStr(C_PreviousFloatWindowRect, ref endStr),
                        FloatWindowState = (WindowState)Enum.Parse(typeof(WindowState),GetParamFromStr(C_FloatingWindowState, ref endStr))
                    };
                
                    LoadState(dockingParams, inElements, outElements, converter);
                }
            }
            catch (FormatException)
            {
                throw;
            }

            //AddListsToChildren(inElements, outElements);
            m_loadingState = true;
            LockLayoutUpdate = false;
            LockPropertyChangedAction = false;
            DockingManager.UpdateLayout(this);
        }

        internal bool m_StopFireShow = false;

        internal FrameworkElement maximizedelement = null;
        /// <summary>
        /// Loads state persisted (xml).
        /// </summary>
        /// <param name="reader">Writer for loading.</param>
        private bool LoadState(XmlReader reader)
        {
            bool result = false;

            XmlSerializer serializer = CreateDefaultXmlSerializer(typeof(List<DockingParams>));
            m_loadflag = true;
            firstdragelement = null;
            dragged = false;
            if (serializer.CanDeserialize(reader))
            {
                try
                {
                    List<DockingParams> dockingParamsList = (List<DockingParams>)serializer.Deserialize(reader);
                    //if (dockingParamsList.Count != Children.Count)
                    //    return false;
                    if (this.MaximizeMode == MaximizeMode.FullScreen)
                    {
                        foreach (FrameworkElement element in Children)
                        {
                            DockingManager.SetDockWindowState(element, WindowState.Normal);
                        }
                    }
                    ResetDocking();
                    LockPropertyChangedAction = true;
                    RectConverter converter = new RectConverter();
                    List<FrameworkElement> inElements = new List<FrameworkElement>();
                    List<FrameworkElement> outElements = GetCopyThisChildren();
                    if (IgnoreNamesOnDeserialize)
                    {
                        ResolveConflict();
                        MapSavedName(dockingParamsList);
                    }
                    //m_tabbedelement = null;
                    //m_tabbedhostheight = 0;
                    //m_tabbedhostwidth = 0;
                    foreach (DockingParams dockingParams in dockingParamsList)
                    {
                        if (IgnoreNamesOnDeserialize)
                        {
                            MapTargetsName(dockingParams);
                        }
                        LoadState(dockingParams, inElements, outElements, converter);
                    }

                    sidepanelmouseover = false;

                    if (dockingParamsList.Count > 0)
                    {   
                        dockingParamsList.Sort(new Comparison<DockingParams>((x, y) => x.IndexInDockMode.CompareTo(y.IndexInDockMode)));
                        
                        if (dockingParamsList[0].IndexInDockMode == -1 && dockingParamsList[0].DoShift)
                        {
                            int maxIndex = dockingParamsList[dockingParamsList.Count - 1].IndexInDockMode;
                            foreach (DockingParams param in dockingParamsList)
                            {
                                FrameworkElement child = FindChildSafe(param.Name);
                                DockingManager.SetIndexInDockMode(child, ++maxIndex);
                                if (param.TargetDocked == String.Empty)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    foreach (DockingParams param in dockingParamsList)
                    {
                        FrameworkElement child = FindChildSafe(param.Name);
                        if (child != null)
                        {
                            List<String> childList = DockingManager.GetPreviousChildElements(child);
                            if (childList != null && childList.Count > 0)
                            {
                                GetTempTargetForAutoHide(child);
                            }
                        }
                    }


                   // AssignCorrectTargets();
                    DirectTabPanel.m_nameCreator = 0;
                    TDILayoutPanel.m_nameSufix = 0;
                    
                    //AddListsToChildren(inElements, outElements);
                    LockPropertyChangedAction = false;
                    LockLayoutUpdate = false;
                    m_loadingState = true;
                    UpdateLayout();
                    ///This below method call has been added for MT2257, 91433, Mt2246, MT2269, Mt2270, 92396 - Component Swapping issue 
                    UpdateInnerDockElements();

                    if (maximizedelement!=null && this.MaximizeMode == MaximizeMode.FullScreen)
                    {
                        m_loadingState = true;
                        EnableFullScreenMode(this.RootContainer, maximizedelement, DockingManager.GetDockHost(maximizedelement));
                        DockingManager.SetDockWindowState(maximizedelement, WindowState.Maximized);
                        DockingManager.SetMaximizeButtonVisibility(maximizedelement, Visibility.Collapsed);
                        m_loadingState = false;
                    }
                    //foreach(DockingParams param in dockingParamsList)
                    //{
                    //    if(param.IsSelectedTab && param.State != DockState.Hidden && param.State != DockState.AutoHidden)
                    //    {
                    //        ActivateWindow(param.Name);
                    //    }
                    //}

                    foreach (DockingParams param in dockingParamsList)
                    {
                       if (param.IsActiveWindow && param.State != DockState.Hidden)
                       {
                           if (param.State == DockState.AutoHidden)
                           {
                               m_StopFireShow = true;
                           }
                           //else if (param.IsSelectedTab)
                           //{
                           //    ActivateWindow(param.Name);
                           //}
                           m_StopFireShow = false;
                       }
                    }
                    if (this.RootContainer != null)
                    {
                        RootContainer.ResetVisibility();
                    }
                    LockLayoutUpdate = true;

                    result = true;
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

            serializer = null;
            return result;
        }

        private void MapSavedName(List<DockingParams> parameters)
        {
            int i=0;
            nameMappings.Clear();
            foreach (DockingParams parameter in parameters)
            {
                if (i < Children.Count)
                {
                    nameMappings.Add(parameter.Name, Children[i].Name);
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        internal void ResolveConflict()
        {
            foreach (FrameworkElement element in Children)
            {
                List<FrameworkElement> elementList = GetElement(element.Name);
                if (elementList.Count > 1)
                {
                    foreach (FrameworkElement elementsub in elementList)
                    {
                        elementsub.Name = elementsub.Name+elementsub.GetHashCode();
                    }
                }
            }
        }

        private List<FrameworkElement> GetElement(string name)
        {
            List<FrameworkElement> listElements=new List<FrameworkElement>();
            foreach (FrameworkElement element in Children)
            {
                if (element.Name == name)
                {
                    listElements.Add(element);
                }
            }
            return listElements;
        }

        /// <summary>
        /// Maps the name of the targets.
        /// </summary>
        /// <param name="param">The param.</param>
        private void MapTargetsName(DockingParams param)
        {
            param.TargetDocked = FindMappingName(param.TargetDocked);
            param.TargetFloating = FindMappingName(param.TargetFloating);
            param.TabParent = FindMappingName(param.TabParent);
            //if (param.PreviousChildElements != null)
            //{
            //    foreach (string child in param.PreviousChildElements)
            //    {
            //        param.PreviousChildElements.re
            //    }
            //}
        }
        /// <summary>
        /// Finds the name of the mapping.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string FindMappingName(string key)
        {
            if (key != null)
            {
                string result = null;
                nameMappings.TryGetValue(key, out result);
                return result == null ? key : result;
            }
            return null;
        }

        /// <summary>
        /// Assign Correct Targets
        /// </summary>
        private void AssignCorrectTargets()
        {
            foreach (FrameworkElement element in Children)
            {
                if (element != null)
                {
                    String targetName = DockingManager.GetTabParent(element);
                    if (targetName != null && targetName != String.Empty && FindVisiblity(targetName))
                    {
                        updatedockflag = false;
                        DockSide side = DockingManager.GetSideRelativetoContainer(element);
                        DockingManager.SetTargetNameInDockedMode(element, targetName);
                        //if (side != null)
                        //{
                        DockingManager.SetSideInDockedMode(element, side);
                        //}
                    }
                }
            }
        }
        /// <summary>
        /// Find Visiblity
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool FindVisiblity(String name)
        {
            FrameworkElement element = FindChildSafe(name);
            if (element != null)
            {
                DockState state=DockingManager.GetState(element);
                if(state==DockState.Dock)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Get Temp Target For Auto Hide
        /// </summary>
        /// <param name="element"></param>
        private void GetTempTargetForAutoHide(FrameworkElement element)
        {
            List<String> childName = DockingManager.GetPreviousChildElements(element);
            FrameworkElement tempElement = null;
            int maxIndex=-1;
            foreach (String source in childName)
            {
                FrameworkElement child = FindChildSafe(source);
                int index=DockingManager.GetIndexInDockMode(child);
                if (index > maxIndex)
                {
                    maxIndex = index;
                    tempElement = child;
                }
            }

            if (tempElement != null)
            {
                updatedockflag = false;
                DockingManager.SetPreviousTargetInDockMode(element, DockingManager.GetTargetNameInDockedMode(element));
                DockingManager.SetTargetNameInDockedMode(element, tempElement.Name);
                DockSide side = DockingManager.GetSideRelativetoContainer(tempElement);
                DockingManager.SetPreviousSideInDockMode(element, DockingManager.GetPreviousSideInDockMode(element));
                DockSide resultSide=DockSide.Left;

                if (side == DockSide.Left)
                {
                    resultSide = DockSide.Right;
                }
                else if (side == DockSide.Right)
                {
                    resultSide = DockSide.Left;
                }
                else if (side == DockSide.Top)
                {
                    resultSide = DockSide.Bottom;
                }
                else if (side == DockSide.Bottom)
                {
                    resultSide = DockSide.Top;
                }

                DockingManager.SetSideInDockedMode(element, resultSide);
                DockingManager.SetIsTargetChanged(element, true);
            }

         }


        /// <summary>
        /// Calls OnPersistStateChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnPersistStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnPersistStateChanged(e);
        }

        /// <summary>
        /// Writes parameter name and value to memory stream.
        /// </summary>
        /// <param name="memStream">Memory stream.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="paramValue">Parameter value.</param>
        /// <property name="flag" value="Finished"/>
        private static void WriteToStream(Stream memStream, string paramName, string paramValue)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(paramName + "=" + paramValue + ";");
            memStream.Write(byteArray, 0, byteArray.Length);
        }

        /// <summary>
        /// Writes all parameters to memory stream.
        /// </summary>
        /// <param name="memStream">Memory stream.</param>
        /// <param name="dockingParams">The docking params.</param>
        /// <property name="flag" value="Finished"/>
        private static void WriteDockParams(Stream memStream, DockingParams dockingParams)
        {
            WriteToStream(memStream, C_NAME, dockingParams.Name);
            WriteToStream(memStream, C_STATE, dockingParams.State.ToString());
            WriteToStream(memStream, C_NODock, dockingParams.NoDock.ToString());
            WriteToStream(memStream, C_CANDock, dockingParams.CanDock.ToString());
            WriteToStream(memStream, C_CANClose, dockingParams.CanClose.ToString());
            WriteToStream(memStream, C_ISSelectedTab, dockingParams.IsSelectedTab.ToString());
            WriteToStream(memStream, C_ISActiveWindow, dockingParams.IsActiveWindow.ToString());
            WriteToStream(memStream, C_SIDEDocked, dockingParams.SideDocked.ToString());
            WriteToStream(memStream, C_SIDEFloating, dockingParams.SideFloating.ToString());
            WriteToStream(memStream, C_TARGETDocked, dockingParams.TargetDocked);
            WriteToStream(memStream, C_TARGETFloating, dockingParams.TargetFloating);
            WriteToStream(memStream, C_DesiredHeightInDockedMode, dockingParams.DesiredHeightInDockedMode.ToString());
            WriteToStream(memStream, C_DesiredHeightInFloatingMode, dockingParams.DesiredHeightInFloatingMode.ToString());
            WriteToStream(memStream, C_DesiredWidthInDockedMode, dockingParams.DesiredWidthInDockedMode.ToString());
            WriteToStream(memStream, C_DesiredWidthInFloatingMode, dockingParams.DesiredWidthInFloatingMode.ToString());
            WriteToStream(memStream, C_WindowRect, dockingParams.WindowRect);
            WriteToStream(memStream, C_TabGroupName, dockingParams.TabGroupName);
            WriteToStream(memStream, C_ISTabGroupOwner, dockingParams.IsTabGroupOwner.ToString());
            WriteToStream(memStream, C_SideTabOrder, dockingParams.SideTabOrder.ToString());
            WriteToStream(memStream, C_IndexInDockMode, dockingParams.IndexInDockMode.ToString());
            WriteToStream(memStream, C_IndexInFloatMode, dockingParams.IndexInFloatMode.ToString());
            WriteToStream(memStream, C_TabOrderInDockMode, dockingParams.TabOrderInDockMode.ToString());
            WriteToStream(memStream, C_TabOrderInFloatMode, dockingParams.TabOrderInFloatMode.ToString());
            WriteToStream(memStream, C_MDIBounds, dockingParams.MDIBounds.ToString());
            WriteToStream(memStream, C_MDIMinimizedBounds, dockingParams.MDIMinimizedBounds.ToString());
            WriteToStream(memStream, C_MDIWindowState, dockingParams.MDIWindowState.ToString());
            WriteToStream(memStream, C_AllowMDIResize, dockingParams.AllowMDIResize.ToString());
            WriteToStream(memStream, C_TDIIndex, dockingParams.TDIIndex.ToString());
            WriteToStream(memStream, C_IsSelected, dockingParams.IsSelected.ToString());
            WriteToStream(memStream, C_TDIGroupOrientation, dockingParams.TDIGroupOrientation.ToString());
            WriteToStream(memStream, C_WayOfTDIGroup, dockingParams.WayOfTDIGroup.ToString());
            WriteToStream(memStream, C_TDISplitPanelOffset, dockingParams.SplitPanelOffset.ToString());
            WriteToStream(memStream, C_CanMaximize, dockingParams.CanMaximize.ToString());
            WriteToStream(memStream, C_CanMinimize, dockingParams.CanMinimize.ToString());
            WriteToStream(memStream, C_DockWindowState, dockingParams.DockWindowState.ToString());
            WriteToStream(memStream, C_CanResizeInDockedState, dockingParams.CanResizeInDockedState.ToString());
            WriteToStream(memStream, C_CanResizeInFloatState, dockingParams.CanResizeInFloatState.ToString());
            WriteToStream(memStream, C_CanResizeHeightInDockedState, dockingParams.CanResizeHeightInDockedState.ToString());
            WriteToStream(memStream, C_CanResizeWidthInDockedState, dockingParams.CanResizeWidthInDockedState.ToString());
            WriteToStream(memStream, C_CanResizeHeightInFloatState, dockingParams.CanResizeHeightInFloatState.ToString());
            WriteToStream(memStream, C_CanResizeWidthInFloatState, dockingParams.CanResizeWidthInFloatState.ToString());
            WriteToStream(memStream, C_CanFloatMaximize, dockingParams.CanFloatMaximize.ToString());
            WriteToStream(memStream, C_IsFixedSize, dockingParams.IsFixedSize.ToString());
            WriteToStream(memStream, C_IsFixedHeight, dockingParams.IsFixedHeight.ToString());
            WriteToStream(memStream, C_IsFixedWidth, dockingParams.IsFixedWidth.ToString());
            WriteToStream(memStream, C_FixedHeight, dockingParams.FixedHeight.ToString());
            WriteToStream(memStream, C_FixedWidth, dockingParams.FixedWidth.ToString());
            WriteToStream(memStream, C_PreviousContainerHeight, dockingParams.PreviousContainerHeight.ToString());
            WriteToStream(memStream, C_PreviousContainerWidth, dockingParams.PreviousContainerWidth.ToString());
            WriteToStream(memStream, C_PreviousHostHeight, dockingParams.PreviousHostHeight.ToString());
            WriteToStream(memStream, C_PreviousHostWidth, dockingParams.PreviousHostWidth.ToString());
            WriteToStream(memStream, C_IsSwapped, dockingParams.IsSwapped.ToString());
            WriteToStream(memStream,C_ZorderInFloatMode,dockingParams.ZorderInFloatMode.ToString());
            WriteToStream(memStream, C_PreviousFloatWindowRect, dockingParams.PreviousFloatingWindowRect.ToString());
            WriteToStream(memStream,C_FloatingWindowState,dockingParams.FloatWindowState.ToString());
        }

        /// <summary>
        /// Saves bytes array to registry.
        /// </summary>
        /// <param name="memStream">The memory stream.</param>
        /// <property name="flag" value="Finished"/>
        private static void SaveToRegistry(MemoryStream memStream)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(C_RegSubKeyName);

            if (regKey != null)
            {
                Registry.SetValue(regKey.ToString(), C_RegParamName, memStream.ToArray(), RegistryValueKind.Binary);
            }
        }

        /// <summary>
        /// Gets Dock parameter value from parameters string.
        /// </summary>
        /// <param name="paramName">dock parameter name</param>
        /// <param name="fromStr">parameters string</param>
        /// <returns>return string</returns>
        private static string GetParamFromStr(string paramName, ref string fromStr)
        {
            int n = fromStr.IndexOf(paramName);
            string val;

            if (n >= 0)
            {
                int i = fromStr.IndexOf('=', n);
                int cnt = fromStr.IndexOf(';', n);

                val = fromStr.Substring(++i, cnt - i);
               
                fromStr = fromStr.Remove(n, cnt - n);
                Debug.WriteLine(paramName, val);
            }
            else
            {
                throw new FormatException(paramName + " not found.");
            }

            return val;
        }

        /// <summary>
        /// Gets the rect from STR.
        /// </summary>
        /// <param name="str">The string value.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>return rect.</returns>
        private static Rect GetRectFromStr(string str, TypeConverter converter)
        {
            Rect rect = Rect.Empty;

            if (!string.IsNullOrEmpty(str) && (str != "Empty"))
            {
                str = str.Replace(';', ',');
                rect = (Rect)converter.ConvertFromInvariantString(str);
            }
            else
            {
                rect = Rect.Empty;
            }

            return rect;
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies PersistState dependency property of the <see cref="DockingManager"/>. This property indicates whether load save state.
        /// </summary>
        public static readonly DependencyProperty PersistStateProperty =
            DependencyProperty.Register("PersistState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPersistStateChanged)));

        /// <summary>
        /// Identifies IgnoreNamesOnDeserialize dependency property.
        /// </summary>
        public static readonly DependencyProperty IgnoreNamesOnDeserializeProperty =
          DependencyProperty.Register("IgnoreNamesOnDeserialize", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));//, new PropertyChangedCallback(OnShowTabItemContextMenuChanged)));


        /// <summary>
        /// Identifies EnableOptimizedKeyHandling dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableOptimizedKeyHandlingProperty =
        DependencyProperty.Register("EnableOptimizedKeyHandling", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies IsStateLoaded dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsStateLoadedProperty =
          DependencyProperty.RegisterAttached("IsStateLoaded", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false));//, new PropertyChangedCallback(OnShowTabItemContextMenuChanged)));

        #endregion
    }
}
