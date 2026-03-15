using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
#if !NET_STANDARD
using ScreenSettings.Documents;
using Utilities.Animations;
using Utilities.WPF;
using ScriptVariableValues;
using WPFUtilities.Converters;
using UIMsgBoxAlertService.ComponentService;
using ScreenManager.ComponentService;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Utilities.ProgressDialog;
using VFS;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Automation;
using System.Windows.Markup;
using UFInterfaces.Converters;
using System.Windows.Threading;
using UFInterfaces.Scriptable;
using WinWrap.Basic;
using ScriptManager.ComponentService;
using ScreenParametersEditor.ComponentService;
using UnitConverterManager.ComponentService;
using UFInterfaces.PropertyControl;
#endif
using UFUAEditor.ComponentService;
using log4net;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#endif
using System.Xml;
using AnimationManager;
using CommandManager;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using ScreenSettings.Entities;
using ScreenSettings.Properties;
using UFInterfaces;
using UFInterfaces.Constants;
using Utilities;
using ViewModelLib;
using System.Xml.Serialization;
using System.Dynamic;
using WPFUtilities;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using System.IO.IsolatedStorage;
using System.Text;
using Utilities.Converters;
using UFInterfaces.Editors;
using DocumentManager.ComponentService.Helpers;
using ClientEditor.ComponentService;
using UFProjectManager.ComponentService;

namespace ScreenSettings
{
    [DataContract(Name = "ScreenDocument", Namespace = Namespaces.UriProgea)]
    public class ScreenDocument : ViewModelBase, IDocument, IEntityReference
#if !NET_STANDARD
        , IDocumentTranslator
#if !WINDOWS_UWP
        , ICloneable, IScriptable, INotifyPropertyVisibilityChanged
#endif
#endif
    {
#region Declarations
#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
#if !WINDOWS_UWP
        public dynamic dataContextExpando = new ExpandoObject();
#else
        public ExpandoObject dataContextExpando = new ExpandoObject();
#endif
        public Dictionary<String, MonitoredItemViewModel> mapDataContextExpando = new Dictionary<string, MonitoredItemViewModel>();

        Dictionary<UIElement, Action<bool>> mapItemSourceAction;
        Dictionary<UIElement, PropertyChangeNotifier> mapItemSourceChangeNotifier;
        Dictionary<UIElement, DispatcherOperation> mapItemSourceDispatcherOperation;
        DispatcherOperation d1;

        bool inExecution;
        bool isInLibrary;
        public bool bIsUntranslatedMode;
#if !WINDOWS_UWP
        bool inTest;
        bool bIsBlindServer;
        List<PropertyChangeNotifier> propertyChangeNotifierVisibilityList;
#endif

        String currentSessionName;
#endif

        XamlDocument xamlDocument;
        ILog syslog;

#if !NET_STANDARD
        Dictionary<String, ResourceDictionary> mapLoadedResources;
        Dictionary<String, UIElement> mapProblematicXamlWriterBag = new Dictionary<String, UIElement>();
        List<ContentControl> loadedContentControls;
        IUFProjectManager iUFProjectManager;
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        private bool bRecalculateListScriptVariableUsed;
        
        static string[] webExtensions = Properties.Settings.Default.WebExportExtensions.Split('|');
#endif
        #endregion Declarations

        #region Persistance

        [DataMember]
        ScreenObjectsSettingsMap mapScreenEntities = new ScreenObjectsSettingsMap();

        [DataMember]
        List<String> listResources;

        [DataMember]
        List<String> listAssemblies;

        [DataMember]
        bool showInTaskbar = true;
        [DataMember]
        double top;
        [DataMember]
        double left;
        [DataMember]
        WindowState windowState = WindowState.Maximized;
        [DataMember]
        WindowStyle windowStyle = WindowStyle.None;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        ResizeMode resizeMode = ResizeMode.CanResize;
        [DataMember]
        WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen;
        [DataMember]
        double windowOpacity = 1;
        [DataMember]
        bool windowOpacityOnlyInactive = false;
        [DataMember]
        bool applyWindowSettingsOnLoad = true;
#endif
#if !NET_STANDARD
        Brush background;
#endif
        [DataMember]
        uint delayUnloadSecs;
        [DataMember]
        bool scanChildrenElements = true;
#if !WINDOWS_UWP
        [DataMember]
        bool showNavigation = false;
        [DataMember]
        bool showHeader = false;
        [DataMember]
        bool showTabStrip = false;
        [DataMember]
        byte monitorNumber;
#endif
        [DataMember]
        bool resizeControlsOnFit;
        [DataMember]
        bool fitInWindow;
        [DataMember]
        bool keepAspectRatio;
        [DataMember]
        bool toolbarVisible = true;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        bool forceWritingOnServer;
#endif
#if !WINDOWS_UWP
        [DataMember]
        bool layoutView;
        [DataMember]
        bool layoutViewEditable;
#endif
        [DataMember]
        bool requireUserLogin;
        [DataMember]
        double width;
        [DataMember]
        double height;
        [DataMember]
        bool keepAlwaysInMemory = false;
        [DataMember]
        int delayLoadSymbols;
        [DataMember]
        string sessionName;
        [DataMember]
        int removeDisabledItemAfterSecs = 30;
        [DataMember]
        int maxCleanCount = 2;
        [DataMember]
        bool useAlwaysSecureConnections = false;
        [DataMember]
        int slowSamplingInterval = 5000;
        [DataMember]
        bool disableWhenNotUsed = true;
        [DataMember]
        int publishingInterval = 1000;
        [DataMember]
        int fastSamplingInterval = 500;
        [DataMember]
        bool loadFromRepositorySynchro;
        [DataMember]
        bool hideScrollBars = false;
        [DataMember]
        bool disableZoom = false;
        [DataMember]
        double zoomLevelCacheModeX = 2;
        [DataMember]
        double zoomLevelCacheModeY = 2;

        [DataMember]
        Dictionary<String, String> mapAlias;

        [DataMember]
        bool hideLayoutScreens = false;

#if !WINDOWS_UWP
#if !NET_STANDARD
        [DataMember]
        String sCode;
        [DataMember]
        int[] breakpoints;
        [DataMember]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        List<String> listScriptVariableUsed;
        [DataMember]
        bool useIntelliSense = true;
#endif
        [DataMember]
        Guid id;
#if !NET_STANDARD
        [DataMember]
        int writeTimeout = 0;
#endif
        [DataMember]
        Guid idDocument;
#if !NET_STANDARD
        [DataMember]
        Dictionary<String, List<String>> mapScriptVariableUsed;
        [DataMember]
        String variableSettings;
        [DataMember]
        String localVariableSettings;
#endif
        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
#if !NET_STANDARD
            if (sCode != null && !sCode.Contains('\r'))
                sCode = sCode.Replace("\n", Environment.NewLine);
#endif
            if (idDocument == null || idDocument == Guid.Empty)
                idDocument = Guid.NewGuid();
#if !NET_STANDARD
            if (mapScriptVariableUsed == null)
            {
                bRecalculateListScriptVariableUsed = true;
            }
            else
            {
                var list = new List<string>();
                foreach (var v in mapScriptVariableUsed.Values)
                {
                    list.AddRange(v);
                }
                if (list.Count > 0)
                {
                    listScriptVariableUsed = new List<string>();
                    listScriptVariableUsed.AddRange(list.Distinct());
                }
            }
#endif
        }
#endif
        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            showInTaskbar = true;
#if !WINDOWS_UWP && !NET_STANDARD
            resizeMode = ResizeMode.CanResize;
            windowStartupLocation = WindowStartupLocation.CenterScreen;
            windowState = WindowState.Maximized;
            windowStyle = WindowStyle.None;
            windowOpacity = 1;
            windowOpacityOnlyInactive = false;
            applyWindowSettingsOnLoad = true;
            useIntelliSense = true;
            writeTimeout = 0;
#endif
#if !NET_STANDARD
            background = new SolidColorBrush(Colors.DarkGray);
#endif
            toolbarVisible = true;
#if !WINDOWS_UWP && !NET_STANDARD
            forceWritingOnServer = false;
#endif
#if !WINDOWS_UWP
            showNavigation = true;
            showHeader = true;
            showTabStrip = true;
#endif
            keepAlwaysInMemory = false;
            delayLoadSymbols = 500;
            hideScrollBars = false;
            disableZoom = false;
            hideLayoutScreens = false;
            zoomLevelCacheModeX = 2;
            zoomLevelCacheModeY = 2;
            delayUnloadSecs = 5;
            scanChildrenElements = true;

#if !NET_STANDARD
            dataContextExpando = new ExpandoObject();
            mapDataContextExpando = new Dictionary<string, MonitoredItemViewModel>();
            mapProblematicXamlWriterBag = new Dictionary<String, UIElement>();
#endif

            maxCleanCount = 2;
            removeDisabledItemAfterSecs = 30;
            useAlwaysSecureConnections = false;
            slowSamplingInterval = 5000;
            disableWhenNotUsed = true;
            publishingInterval = 250;
            fastSamplingInterval = 500;

#if !WINDOWS_UWP && !NET_STANDARD
            lockObjectVariables = new Object();
#endif
        }

#endregion Persistance

#region Constructors

        public ScreenDocument()
        {
            xamlDocument = new XamlDocument("");
#if !WINDOWS_UWP
            xamlDocument.InitializeSourceText(Settings.Default.WPFDefaultXAML);
#endif
#if !NET_STANDARD
            iUFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
#endif
        }

        //public ScreenDocument(ScreenEditorView sev)
        //{
        //    ScreenEditorView = sev;
        //    xamlDocument = new XamlDocument("");
        //    xamlDocument.InitializeSourceText(Settings.Default.WPFDefaultXAML);
        //}

#endregion Constructors

#region Attached Properties
#if !NET_STANDARD
        /// <summary>
        /// Provides attached properties used to communicate with a screen document.
        /// </summary>
        public static readonly DependencyProperty ScreenDocumentProperty = DependencyProperty.RegisterAttached("ScreenDocument", typeof(IDocument), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(null));
#endif
        public static void SetScreenDocument(UIElement element, IDocument value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ScreenDocumentProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static IDocument GetScreenDocument(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (IDocument)(element.GetValue(ScreenDocumentProperty));
        }

        public static readonly DependencyProperty RunningOnServerProperty = DependencyProperty.RegisterAttached("RunningOnServer", typeof(bool), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(false));
#endif
        public static void SetRunningOnServer(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(RunningOnServerProperty, value);
            if (value == true)
                element.SetValue(RunningOnSlowPCProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static bool GetRunningOnServer(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)(element.GetValue(RunningOnServerProperty));
        }

        public static readonly DependencyProperty RunningOnSlowPCProperty = DependencyProperty.RegisterAttached("RunningOnSlowPCMachine", typeof(bool), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(false));
#endif
        public static void SetRunningOnSlowPC(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(RunningOnSlowPCProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static bool GetRunningOnSlowPC(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)(element.GetValue(RunningOnSlowPCProperty));
        }

        public static readonly DependencyProperty AutoForceDynamicOnClientProperty = DependencyProperty.RegisterAttached("AutoForceDynamicOnClient", typeof(bool), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(false));
#endif
        public static void SetAutoForceDynamicOnClient(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(AutoForceDynamicOnClientProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static bool GetAutoForceDynamicOnClient(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)(element.GetValue(AutoForceDynamicOnClientProperty));
        }

        public static readonly DependencyProperty ClientTimezoneOffsetProperty = DependencyProperty.RegisterAttached("ClientTimezoneOffset", typeof(int), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(0));
#endif
        public static void SetClientTimezoneOffset(UIElement element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(ClientTimezoneOffsetProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static int GetClientTimezoneOffset(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (int)(element.GetValue(ClientTimezoneOffsetProperty));
        }

        public static readonly DependencyProperty AccessRoleProperty = DependencyProperty.RegisterAttached("AccessRole", typeof(String), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(null));
#endif
        public static void SetAccessRole(UIElement element, String value)
        {
            element.SetValue(AccessRoleProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static String GetAccessRole(UIElement element)
        {
            return (String)element.GetValue(AccessRoleProperty);
        }

        public static readonly DependencyProperty AccessLevelProperty = DependencyProperty.RegisterAttached("AccessLevel", typeof(int), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(-1));
#endif
        public static void SetAccessLevel(UIElement element, int value)
        {
            element.SetValue(AccessLevelProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static int GetAccessLevel(UIElement element)
        {
            return (int)element.GetValue(AccessLevelProperty);
        }

        public static readonly DependencyProperty AccessMaskProperty = DependencyProperty.RegisterAttached("AccessMask", typeof(int), typeof(ScreenDocument),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));
#else
            new PropertyMetadata(0));
#endif
        public static void SetAccessMask(UIElement element, int value)
        {
            element.SetValue(AccessMaskProperty, value);
        }

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public static int GetAccessMask(UIElement element)
        {
            return (int)element.GetValue(AccessMaskProperty);
        }

#endif
        #endregion

        #region Static Methods

#if !WINDOWS_UWP
        public static readonly String compiledExt = Properties.Settings.Default.CompiledExt;
        public static readonly String bamlExt = Properties.Settings.Default.BamlExt;
        static readonly String settingsExt = Properties.Settings.Default.SettingsExt;
        static readonly String styleExt = Properties.Settings.Default.StyleExt;
        static readonly String layoutExt = Properties.Settings.Default.LayoutExt;
#else
        static readonly String settingsExt = "Settings";
        static readonly String styleExt = "Style";
        static readonly String layoutExt = "Layout";
#endif
        public static String GetXamlFileName(String fullPath)
        {
            if (Path.GetExtension(fullPath).ToLower() != settingsExt.ToLower())
                return fullPath;

            return fullPath.Replace(settingsExt, "");
        }
        
        public static String GetFileWithExt(String fullPath, String extension)
        {
            if (Path.GetExtension(fullPath).ToLower() == extension.ToLower())
                return fullPath;

            return String.Format("{0}.{1}", fullPath, extension);
        }

        public static String GetSettingsFileName(String fullPath)
        {
            if (Path.GetExtension(fullPath).ToLower() == settingsExt.ToLower())
                return fullPath;

            return String.Format("{0}.{1}", fullPath, settingsExt);
        }

        public static String GetStyleFileName(String fullPath)
        {
            if (Path.GetExtension(fullPath).ToLower() == settingsExt.ToLower())
                return fullPath;

            return String.Format("{0}.{1}", fullPath, styleExt);
        }

        public static String GetSettingsBinaryFileName(String fullPath)
        {
            if (Path.GetExtension(fullPath).ToLower() == settingsExt.ToLower())
                return fullPath;

            return String.Format("{0}.{1}.bin", fullPath, settingsExt);
        }

        public static String GetLayoutFileName(String fullPath)
        {
            if (Path.GetExtension(fullPath).ToLower() == layoutExt.ToLower())
                return fullPath;

            return String.Format("{0}.{1}", fullPath, layoutExt);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public static void RemoveFile(string fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            var styleFileName = GetStyleFileName(fullPath);
            var settingsFileName = GetSettingsFileName(fullPath);
            var file = Path.ChangeExtension(fullPath, "png");
            if (fileSystemProvider != null)
            {
                var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, settingsFileName);
                if (fileSystemProvider.Exists(fileManagerFileSettings))
                {
                    fileSystemProvider.DeleteFile(fileManagerFileSettings);
                }
                var fileManagerFileStyle = new FileManagerFile(fileSystemProvider, styleFileName);
                if (fileSystemProvider.Exists(fileManagerFileStyle))
                {
                    fileSystemProvider.DeleteFile(fileManagerFileStyle);
                }
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
                var fileManagerFilePng = new FileManagerFile(fileSystemProvider, file);
                if (fileSystemProvider.Exists(fileManagerFilePng))
                {
                    fileSystemProvider.DeleteFile(fileManagerFilePng);
                }
            }
            else
            {
                if (File.Exists(settingsFileName))
                    File.Delete(settingsFileName);
                if (File.Exists(styleFileName))
                    File.Delete(styleFileName);
                settingsFileName = GetSettingsBinaryFileName(fullPath);
                if (File.Exists(settingsFileName))
                    File.Delete(settingsFileName);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                if (File.Exists(file))
                    File.Delete(file);

                if (webExtensions.Length > 0)
                {
                    var folder = Path.GetDirectoryName(fullPath);
                    var fileName = Path.GetFileNameWithoutExtension(fullPath);

                    foreach (var extension in webExtensions)
                    {
                        var webFile = $"{folder}\\{fileName}.{extension}";
                        if (File.Exists(webFile))
                            File.Delete(webFile);
                    };
                }
            }
        }

        public static String RenameFile(String fullPath, String oldName, String newName, FileSystemProviderBase fileSystemProvider = null)
        {
            var folder = Path.GetDirectoryName(fullPath);
            var newPathName = String.Format("{0}\\{1}{2}", folder, newName, Path.GetExtension(fullPath));

            var sourcesettingsFileName = GetSettingsFileName(fullPath);
            var sourcestyleFileName = GetStyleFileName(fullPath);
            if (fileSystemProvider != null)
            {
                var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, sourcesettingsFileName);
                if (fileSystemProvider.Exists(fileManagerFileSettings))
                {
                    var destsettingsFileName = GetSettingsFileName(newPathName);
                    fileSystemProvider.RenameFile(fileManagerFileSettings, Path.GetFileName(destsettingsFileName));
                }

                var fileManagerFileStyle = new FileManagerFile(fileSystemProvider, sourcestyleFileName);
                if (fileSystemProvider.Exists(fileManagerFileStyle))
                {
                    var deststyleFileName = GetStyleFileName(newPathName);
                    fileSystemProvider.RenameFile(fileManagerFileSettings, Path.GetFileName(deststyleFileName));
                }

                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.RenameFile(fileManagerFile, String.Format("{0}{1}", newName, fileManagerFile.Extension));
                }

                var file = Path.ChangeExtension(fullPath, "png");
                var fileManagerFilePng = new FileManagerFile(fileSystemProvider, file);
                if (fileSystemProvider.Exists(fileManagerFilePng))
                {
                    fileSystemProvider.RenameFile(fileManagerFilePng, String.Format("{0}{1}", newName, fileManagerFilePng.Extension));
                }
            }
            else
            {
                var destsettingsFileName = GetSettingsFileName(newPathName);

                if (File.Exists(sourcesettingsFileName))
                    File.Move(sourcesettingsFileName, destsettingsFileName);

                var deststyleFileName = GetStyleFileName(newPathName);

                if (File.Exists(sourcestyleFileName))
                    File.Move(sourcestyleFileName, deststyleFileName);

                destsettingsFileName = GetSettingsBinaryFileName(newPathName);
                sourcesettingsFileName = GetSettingsBinaryFileName(fullPath);
                if (File.Exists(sourcesettingsFileName))
                    File.Move(sourcesettingsFileName, destsettingsFileName);

                if (File.Exists(fullPath))
                    File.Move(fullPath, newPathName);

                var file = Path.ChangeExtension(fullPath, "png");
                var filenew = Path.ChangeExtension(newPathName, "png");
                if (File.Exists(file))
                    File.Move(file, filenew);

                if (webExtensions.Length > 0)
                {
                    foreach (var extension in webExtensions)
                    {
                        destsettingsFileName = $"{folder}\\{newName}.{extension}";
                        if (File.Exists($"{folder}\\{oldName}.{extension}"))
                            File.Move($"{folder}\\{oldName}.{extension}", destsettingsFileName);
                    }
                }
            }

            return newPathName;
        }

        static void ScreenDocumentFileCopy(string sourceFileName, string destFileName, IDocument parent, FileSystemProviderBase targetVFS, bool bCopy)
        {
            if (File.Exists(sourceFileName))
            {
                if (targetVFS != null)
                    targetVFS.UploadFile(null, sourceFileName.Replace(parent.rootBase, parent.rootBaseDB),
                        File.ReadAllBytes(sourceFileName));
                else
                    File.Copy(sourceFileName, destFileName, true);
            }

            if (!bCopy)
                RemoveFile(sourceFileName);
        }

        static void FileSystemProviderCopy(FileSystemProviderBase fileSystemProvider, FileSystemProviderBase targetVFS, string fileName, string fileDest, bool bDisposeTargetVFS, bool bCopy)
        {
            var fileManagerFileControls = new FileManagerFile(fileSystemProvider, fileName);
            if (fileSystemProvider.Exists(fileManagerFileControls))
            {
                var data = fileSystemProvider.ReadFile(fileManagerFileControls);
                if (targetVFS != null)
                {
                    if (bDisposeTargetVFS)
                        targetVFS.UploadFile(null, fileName, data);
                    else
                        targetVFS.UploadFile(null, fileDest, data);
                }
                else
                {
                    File.WriteAllBytes(fileDest, data);
                }

                if (!bCopy)
                    fileSystemProvider.DeleteFile(fileManagerFileControls);
            }
        }

        public static void CopyFile(String fullPath, String newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            FileSystemProviderBase targetVFS = null;
            bool bDisposeTargetVFS = true;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(newPath);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                    {
                        ConnectionString = newPath
                    };
            }
            else
            {
                if (!Path.IsPathRooted(newPath))
                {
                    targetVFS = fileSystemProvider;
                    bDisposeTargetVFS = false;
                }
            }

            try
            {
                var sourcesettingsFileName = GetSettingsFileName(fullPath);
                var sourcestyleFileName = GetStyleFileName(fullPath);
                byte[] dataSettings = null;
                if (fileSystemProvider != null)
                {
                    var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, sourcesettingsFileName);
                    if (fileSystemProvider.Exists(fileManagerFileSettings))
                    {
                        var fileDest = GetSettingsFileName(newPath);
                        var data = fileSystemProvider.ReadFile(fileManagerFileSettings);
                        dataSettings = data;
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, sourcesettingsFileName, data);
                            else
                                targetVFS.UploadFile(null, fileDest, data);
                        }
                        else
                        {
                            File.WriteAllBytes(fileDest, data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFileSettings);
                    }

                    var fileManagerFileStyle = new FileManagerFile(fileSystemProvider, sourcestyleFileName);
                    if (fileSystemProvider.Exists(fileManagerFileStyle))
                    {
                        var fileDest = GetStyleFileName(newPath);
                        var data = fileSystemProvider.ReadFile(fileManagerFileStyle);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, sourcestyleFileName, data);
                            else
                                targetVFS.UploadFile(null, fileDest, data);
                        }
                        else
                        {
                            File.WriteAllBytes(fileDest, data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFileStyle);
                    }

                    var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fullPath, data);
                            else
                                targetVFS.UploadFile(null, newPath, data);
                        }
                        else
                        {
                            var utf8Data = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, data);
                            if (bUploading)
                            {
                                var fileTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                                fileTemp = Path.ChangeExtension(fileTemp, "xaml");
                                var fileTempsettings = GetSettingsFileName(fileTemp);
                                var fileTempsettingsBinary = GetSettingsBinaryFileName(fileTemp);
                                try
                                {
                                    File.WriteAllBytes(fileTemp, utf8Data);
                                    if (dataSettings != null)
                                        File.WriteAllBytes(fileTempsettings, dataSettings);

                                    ConvertToFile(fileTemp, parent);
                                    File.Copy(fileTemp, newPath, true);
                                    var fileDest = GetSettingsFileName(newPath);
                                    File.Copy(fileTempsettings, fileDest, true);
                                    fileDest = GetSettingsBinaryFileName(newPath);
                                    File.Copy(fileTempsettingsBinary, fileDest, true);
                                }
                                finally
                                {
                                    try
                                    {
                                        File.Delete(fileTemp);
                                    }
                                    catch { }
                                    try
                                    {
                                        File.Delete(fileTempsettings);
                                    }
                                    catch { }
                                    try
                                    {
                                        File.Delete(fileTempsettingsBinary);
                                    }
                                    catch { }
                                }
                            }
                            else
                                File.WriteAllBytes(newPath, utf8Data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }

                    var file = Path.ChangeExtension(fullPath, "png");
                    var fileManagerFilePng = new FileManagerFile(fileSystemProvider, file);
                    if (fileSystemProvider.Exists(fileManagerFilePng))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFilePng);
                        var fileDest = Path.ChangeExtension(newPath, "png");
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fileDest, data);
                            else
                                targetVFS.UploadFile(null, fileDest, data);
                        }
                        else
                            File.WriteAllBytes(fileDest, data);

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFilePng);
                    }

                    var sourcelayoutFileName = GetLayoutFileName(fullPath);
                    var fileManagerFileLayout = new FileManagerFile(fileSystemProvider, sourcelayoutFileName);
                    if (fileSystemProvider.Exists(fileManagerFileLayout))
                    {
                        var fileDest = GetLayoutFileName(newPath);
                        var data = fileSystemProvider.ReadFile(fileManagerFileLayout);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, sourcelayoutFileName, data);
                            else
                                targetVFS.UploadFile(null, fileDest, data);
                        }
                        else
                        {
                            File.WriteAllBytes(fileDest, data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFileLayout);
                    }

                    if (targetVFS == null)
                    {
                        foreach (var ext in webExtensions)
                        {
                            var fileName = Path.ChangeExtension(fullPath, ext);
                            FileSystemProviderCopy(fileSystemProvider, targetVFS, fileName, fileName, bDisposeTargetVFS, bCopy);
                        }
                    }
                }
                else
                {
                    var destsettingsFileName = GetSettingsFileName(newPath);
                    if (File.Exists(sourcesettingsFileName))
                    {
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, sourcesettingsFileName.Replace(parent.rootBase, parent.rootBaseDB), 
                                File.ReadAllBytes(sourcesettingsFileName));
                        else
                            File.Copy(sourcesettingsFileName, destsettingsFileName, true);
                    }

                    if (!bCopy)
                        RemoveFile(sourcesettingsFileName);

                    var deststyleFileName = GetStyleFileName(newPath);
                    if (File.Exists(sourcestyleFileName))
                    {
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, sourcestyleFileName.Replace(parent.rootBase, parent.rootBaseDB),
                                File.ReadAllBytes(sourcestyleFileName));
                        else
                            File.Copy(sourcestyleFileName, deststyleFileName, true);
                    }

                    if (!bCopy)
                        RemoveFile(sourcestyleFileName);

                    var destsettingsBinaryFileName = GetSettingsBinaryFileName(newPath);
                    if (targetVFS == null)
                    {
                        var sourcesettingsBinaryFileName = GetSettingsBinaryFileName(fullPath);
                        if (File.Exists(sourcesettingsBinaryFileName))
                        {
                            File.Copy(sourcesettingsBinaryFileName, destsettingsBinaryFileName, true);
                        }

                        if (!bCopy)
                            RemoveFile(sourcesettingsBinaryFileName);
                    }

                    if (File.Exists(fullPath))
                    {
                        if (targetVFS != null)
                        {
                            var text = File.ReadAllText(fullPath);
                            var data = System.Text.Encoding.Unicode.GetBytes(text);
                            targetVFS.UploadFile(null, fullPath.Replace(parent.rootBase, parent.rootBaseDB), data);
                        }
                        else
                        {
                            if (bUploading)
                            {
                                var fileTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                                fileTemp = Path.ChangeExtension(fileTemp, "xaml");
                                var fileTempsettings = GetSettingsFileName(fileTemp);
                                var fileTempsettingsBinary = GetSettingsBinaryFileName(fileTemp);
                                try
                                {
                                    File.Copy(fullPath, fileTemp, true);
                                    File.Copy(sourcesettingsFileName, fileTempsettings, true);
                                    ConvertToFile(fileTemp, parent);
                                    File.Copy(fileTemp, newPath, true);
                                    File.Copy(fileTempsettings, destsettingsFileName, true);
                                    File.Copy(fileTempsettingsBinary, destsettingsBinaryFileName, true);
                                }
                                finally
                                {
                                    try
                                    {
                                        File.Delete(fileTemp);
                                    }
                                    catch { }
                                    try
                                    {
                                        File.Delete(fileTempsettings);
                                    }
                                    catch { }
                                    try
                                    {
                                        File.Delete(fileTempsettingsBinary);
                                    }
                                    catch { }
                                }
                            }
                            else
                                File.Copy(fullPath, newPath, true);
                        }
                    }

                    if (!bCopy)
                        RemoveFile(fullPath);

                    var file = Path.ChangeExtension(fullPath, "png");
                    if (File.Exists(file))
                    {
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, file.Replace(parent.rootBase, parent.rootBaseDB), File.ReadAllBytes(file));
                        else
                        {
                            var filenew = Path.ChangeExtension(newPath, "png");
                            File.Copy(file, filenew, true);
                        }
                    }

                    if (!bCopy)
                        RemoveFile(file);

                    var sourcelayoutFileName = GetLayoutFileName(fullPath);
                    var destlayoutFileName = GetLayoutFileName(newPath);
                    if (File.Exists(sourcelayoutFileName))
                    {
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, sourcelayoutFileName.Replace(parent.rootBase, parent.rootBaseDB),
                                File.ReadAllBytes(sourcelayoutFileName));
                        else
                            File.Copy(sourcelayoutFileName, destlayoutFileName, true);
                    }

                    if (!bCopy)
                        RemoveFile(sourcelayoutFileName);

                    if (targetVFS == null)
                    {
                        foreach (var ext in webExtensions)
                            ScreenDocumentFileCopy(Path.ChangeExtension(fullPath, ext), Path.ChangeExtension(newPath, ext), parent, targetVFS, bCopy);
                    }
                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }

        static readonly String szAssemblyTag = "clr-namespace:";
        public static String AdaptXamlToUWP(String xamlCode)
        {
            var ret = xamlCode.Replace("xmlns=\"clr-namespace:System;assembly=mscorlib\"", "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            ret = ret.Replace(", ValidatesOnDataErrors=True", "");
            ret = ret.Replace("syncfusion:SkinStorage.OverrideVisualStyle=\"True\"", "");
            ret = ret.Replace("IsManipulationEnabled=\"True\"", "");
            ret = ret.Replace("IsManipulationEnabled=\"False\"", "");

            ret = ret.Replace(";assembly=", ";using:");
            while (true)
            {
                int index = ret.IndexOf(szAssemblyTag);
                if (index < 0)
                    break;
                int lastindex = ret.IndexOf(';', index);
                var strNamespace = ret.Substring(index, lastindex - index);
                strNamespace = strNamespace.Replace(szAssemblyTag, "");

                int indexEqual = index;
                while (indexEqual > 0 && ret[indexEqual] != '=')
                    --indexEqual;
                int indexColon = indexEqual;
                while (indexColon > 0 && ret[indexColon] != ':')
                    --indexColon;
                var strNamespaceToReplace = ret.Substring(indexColon + 1, indexEqual - indexColon - 1);

                ret = ret.Remove(index, lastindex - index + 1);

                //ret = ret.Replace(String.Format("{0}:", strNamespaceToReplace),
                //                  String.Format("{0}:{1}.", strNamespaceToReplace, strNamespace));
            }
            return ret;
        }

        static void ConvertToFile(String file, IDocument parent)
        {
            var document = ScreenDocument.FromFile(file, parent);
            if (document == null)
                return;

            if (document.MapScreenEntities != null)
            {
                foreach (var entry in document.MapScreenEntities)
                {
                    if (!String.IsNullOrEmpty(entry.Value.ProblematicXaml))
                        entry.Value.ProblematicXaml = AdaptXamlToUWP(entry.Value.ProblematicXaml);

                    if (entry.Value.MapProblematicXamlWriterProperties != null)
                    {
                        var newBag = new ProblematicXamlWriterProperties();
                        foreach (var pair in entry.Value.MapProblematicXamlWriterProperties)
                            newBag.Add(pair.Key, AdaptXamlToUWP(pair.Value));
                        entry.Value.MapProblematicXamlWriterProperties = newBag;
                    }

                    if (!String.IsNullOrEmpty(entry.Value.TagBrush))
                        entry.Value.TagBrush = AdaptXamlToUWP(entry.Value.TagBrush);
                    if (!String.IsNullOrEmpty(entry.Value.TagPen))
                        entry.Value.TagPen = AdaptXamlToUWP(entry.Value.TagPen);
                }
            }

            var canvas = AdaptXamlToUWP(document.xamlDocument.SourceText);
            var list = (from entry in document.MapScreenEntities.AsParallel()
                        where entry.Value.SourceSymbolLinked
                        select entry).ToList();
            if (list.Count > 0)
            {
                using (var cursor = new WaitCursor())
                {
                    list.ForEach(entry =>
                    {
                        try
                        {
                            var value = STRL.STRL.GetSymbolElement(entry.Value.SourceSymbolProvider,
                                                                    entry.Value.SourceSymbolPath, 
                                                                    document.rootBase, true);
                            var settings = STRL.STRL.GetSymbolSettings(entry.Value.SourceSymbolProvider,
                                                                    entry.Value.SourceSymbolPath, document.rootBase);
                            var style = STRL.STRL.GetSymbolStyle(entry.Value.SourceSymbolProvider,
                                                                    entry.Value.SourceSymbolPath, true);
                            var key = STRL.STRL.GetSymbolStyleKey(entry.Value.SourceSymbolProvider,
                                                                    entry.Value.SourceSymbolPath);

                            if (!String.IsNullOrEmpty(style) && !String.IsNullOrEmpty(key))
                            {
                                // var styleKey = String.Format(" Style=\"{StaticResource {0}}\"", key);
                                var styleKey = " Style=\"{StaticResource " + key + "}\"";
                                int foundToken = value.IndexOf(" ");
                                if (foundToken >= 0)
                                    value = value.Insert(foundToken, styleKey);

                                style = style.Replace("<?xml version=\"1.0\"?>", "");
                                foundToken = canvas.IndexOf(style);
                                if (foundToken < 0)
                                {
                                    var canvasResources = "<Canvas.Resources>";
                                    foundToken = canvas.IndexOf(canvasResources);
                                    if (foundToken < 0)
                                    {
                                        foundToken = canvas.IndexOf(">");
                                        if (foundToken >= 0)
                                        {
                                            var styleResource = String.Format("\n<Canvas.Resources>\n<ResourceDictionary>\n<ResourceDictionary.MergedDictionaries>\n{0}\n</ResourceDictionary.MergedDictionaries>\n</ResourceDictionary>\n</Canvas.Resources>\n", style);
                                            canvas = canvas.Insert(foundToken + 1, styleResource);
                                        }
                                    }
                                    else
                                    {
                                        var mergedResourcesDictionary = "<ResourceDictionary.MergedDictionaries>";
                                        var foundTokenMerged = canvas.IndexOf(mergedResourcesDictionary);
                                        if (foundTokenMerged < 0)
                                        {
                                            var resourceDictionary = "<ResourceDictionary>";
                                            foundToken = canvas.IndexOf(resourceDictionary);
                                            if (foundToken < 0)
                                            {
                                                var styleResource = String.Format("\n<ResourceDictionary>\n<ResourceDictionary.MergedDictionaries>\n{0}\n</ResourceDictionary.MergedDictionaries>\n</ResourceDictionary>\n", style);
                                                canvas = canvas.Insert(foundToken + canvasResources.Length + 1, styleResource);
                                            }
                                            else
                                            {
                                                var styleResource = String.Format("\n<ResourceDictionary.MergedDictionaries>\n{0}\n</ResourceDictionary.MergedDictionaries>\n", style);
                                                canvas = canvas.Insert(foundToken + resourceDictionary.Length + 1, styleResource);
                                            }
                                        }
                                        else
                                            canvas = canvas.Insert(foundTokenMerged + mergedResourcesDictionary.Length + 1, style);
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(value))
                            {
                                var findControl = String.Format(" Name=\"{0}\"", entry.Key);
                                int foundToken = canvas.IndexOf(findControl);
                                if (foundToken >= 0)
                                {
                                    bool bRemoveContent = false;
                                    int foundTokenEnd = canvas.IndexOf(">", foundToken);
                                    if (foundTokenEnd > 0)
                                    {
                                        if (canvas[foundTokenEnd - 1] != '/')
                                            bRemoveContent = true;
                                        else
                                            foundTokenEnd--;
                                    }
                                    if (foundTokenEnd >= 0)
                                    {
                                        while(foundToken >= 0)
                                        {
                                            if (canvas[foundToken] == '<')
                                                break;
                                            --foundToken;
                                        }

                                        int count = foundTokenEnd - foundToken + 2;
                                        if (bRemoveContent)
                                            --count;
                                        var contentControlString = canvas.Substring(foundToken, count);
                                        if (bRemoveContent)
                                        {
                                            var closingControl = "</ContentControl>";
                                            int foundTokenClosingControl = canvas.IndexOf(closingControl, foundToken);
                                            int start = foundTokenEnd + 2;
                                            if (bRemoveContent)
                                                --start;
                                            count = foundTokenClosingControl + closingControl.Length - start;
                                            canvas = canvas.Remove(start, count);
                                        }
                                        canvas = canvas.Replace(contentControlString, "");
                                        contentControlString = contentControlString.Replace("<ContentControl", "");
                                        contentControlString = contentControlString.Replace("/>", "");
                                        contentControlString = contentControlString.Replace(">", "");

                                        var widthTag = "Width=\"";
                                        int nFoundWidth = value.IndexOf(widthTag);
                                        if (nFoundWidth >= 0)
                                        {
                                            int nFoundWithEnd = value.IndexOf("\"", nFoundWidth + widthTag.Length + 1);
                                            value = value.Remove(nFoundWidth, nFoundWithEnd - nFoundWidth + 1);
                                        }

                                        var heightTag = "Height=\"";
                                        int nFoundHeight = value.IndexOf(heightTag);
                                        if (nFoundHeight >= 0)
                                        {
                                            int nFoundHeightEnd = value.IndexOf("\"", nFoundHeight + heightTag.Length + 1);
                                            value = value.Remove(nFoundHeight, nFoundHeightEnd - nFoundHeight + 1);
                                        }

                                        int foundTag = value.IndexOf("<");
                                        if (foundTag >= 0)
                                        {
                                            int foundSpace = value.IndexOf(" ", foundTag);
                                            if (foundSpace >= 0)
                                            {
                                                value = value.Insert(foundSpace + 1, contentControlString);
                                                canvas = canvas.Insert(foundToken, value);
                                            }
                                            else
                                                throw new Exception();
                                        }
                                        else
                                            throw new Exception();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(String.Format("Cannot find the symbol IoT for the control {0}", entry.Key));
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                }
            }

            document.xamlDocument.SourceText = canvas;
            document.SaveToFile(bUploading:true);
            document.Dispose();
        }
#endif
        static ScreenDocument ReadFromStream(Stream reader)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(ScreenDocument));
                var document = formatter.ReadObject(reader) as ScreenDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static ScreenDocument ReadBinaryFromStream(Stream stream)
        {
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
            {
                try
                {
                    var formatter = new DataContractSerializer(typeof(ScreenDocument));
                    var document = formatter.ReadObject(reader) as ScreenDocument;
                    return document;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SubscribeToDocumentChangeEntries()
        {
            foreach (var entity in mapScreenEntities.Values)
            {
                entity.PropertyChanged += entity_PropertyChanged;

                if (entity.MapHashInner3DEntities != null)
                {
                    foreach (var e in entity.MapHashInner3DEntities.Values)
                        e.PropertyChanged += entity_PropertyChanged;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnsubscribeToDocumentChangeEntries()
        {
            foreach (var entity in mapScreenEntities.Values)
            {
                entity.PropertyChanged -= entity_PropertyChanged;

                if (entity.MapHashInner3DEntities != null)
                {
                    foreach (var e in entity.MapHashInner3DEntities.Values)
                        e.PropertyChanged -= entity_PropertyChanged;
                }
            }
        }

        void entity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "sCode" || e.PropertyName == "UseIntelliSense")
                bRecalculateListScriptVariableUsed = true;

            NeedsSave = true;
        }
#endif
        public static bool ExistFile(String fullPath
#if !WINDOWS_UWP && !NET_STANDARD
            , FileSystemProviderBase fileSystemProvider = null
#endif
            )
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProvider != null)
                return fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath));
#endif
            return File.Exists(fullPath);
        }

        public static ScreenDocument FromFile(String fullPath, IDocument parent)
        {
            return FromFile(fullPath, parent, false);
        }

        public static ScreenDocument FromFile(String fullPath, IDocument parent, bool isRuntime)
        {
            ScreenDocument document = null;
            var settingsFileName = GetSettingsFileName(fullPath);
            var settingsBinaryFileName = GetSettingsBinaryFileName(fullPath);
            var compiledFileName = GetFileWithExt(fullPath, compiledExt);
            var bamlFileName = GetFileWithExt(compiledFileName, bamlExt); 

            bool bReadCompiledFiles = false;
            if (isRuntime
#if !NET_STANDARD
                && ScreenCompilerManager.ScreenCompilerManager.IsEnabled()
#endif
                )
            {
                DateTime dtSource = DateTime.MaxValue;
                DateTime dtCompiled = DateTime.MinValue;

                try
                {
                    dtSource = File.GetLastWriteTime(fullPath);
                }
                catch { }
                try
                {
                    dtCompiled = File.GetLastWriteTime(compiledFileName);
                }
                catch { }

                if (dtCompiled >= dtSource)
                    bReadCompiledFiles = true;
            }

            try
            {
#if !WINDOWS_UWP && !NET_STANDARD
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (fileSystemProvider != null)
                {
                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, settingsFileName)))
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, settingsFileName));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            document = ReadFromStream(memoryStream);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            /*
                            XmlReaderSettings settings = new XmlReaderSettings
                            {
                                ConformanceLevel = ConformanceLevel.Document,
                                CloseInput = true
                            };

                            using (XmlReader reader = XmlReader.Create(memoryStream, settings))
                            {
                                var formatter = new DataContractSerializer(typeof(ScreenDocument));
                                document = formatter.ReadObject(reader) as ScreenDocument;
                            }
                             * */
                        }
                    }

                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath)))
                    {
                        var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fullPath));
                        if (document != null && document.listAssemblies != null)
                        {
                            var listToRemove = new List<String>();
                            document.listAssemblies.ForEach(s =>
                            {
                                try
                                {
                                    Assembly.Load(s);
                                }
                                catch (Exception ex)
                                {
                                    listToRemove.Add(s);
                                }
                            });

                            if (listToRemove.Count > 0)
                            {
                                listToRemove.ForEach(s => document.listAssemblies.Remove(s));
                                document.NeedsSave = true;
                            }
                        }

                        if (document == null)
                            document = new ScreenDocument();

                        document.xamlDocument = new XamlDocument(Path.GetDirectoryName(fullPath));
                        document.xamlDocument.InitializeSourceText(System.Text.Encoding.Unicode.GetString(xamlData));
                        document.xamlDocument.FullPath = fullPath;
                        return document;
                    }
                }
#endif
                if (!isRuntime)
                {
                    document = GetDocumentSettings(parent, fullPath, settingsBinaryFileName, settingsFileName);
                    if (document == null)
                        return null;
                }
                else
                {
                    if (bReadCompiledFiles)
                    {
                        var settingsCompiledFileName = GetSettingsFileName(compiledFileName);
                        var settingsBinaryCompiledFileName = GetSettingsBinaryFileName(compiledFileName);
                        document = GetDocumentSettings(parent, compiledFileName, settingsBinaryCompiledFileName, settingsCompiledFileName);
                    }
                    if (document == null)
                    {
                        document = GetDocumentSettings(parent, fullPath, settingsBinaryFileName, settingsFileName);
                        if (document == null)
                            return null;
                    }
                }
                

                if ((isRuntime && (File.Exists(bamlFileName) || File.Exists(compiledFileName))) || File.Exists(fullPath))
                {
#if !NET_STANDARD
                    if (document != null && document.listAssemblies != null)
                        document.listAssemblies.ForEach(s =>
                            {
                                try
                                {
#if WINDOWS_UWP
                                    var asName = new AssemblyName();
                                    asName.Name = s;
                                    Assembly assembly = Assembly.Load(asName);
#else
                                    Assembly.Load(s);
#endif
                                }
                                catch (Exception ex)
                                {

                                }
                            });


                    String sourceText = null;
#if !WINDOWS_UWP
                    Canvas canvas = null;
                    if (isRuntime)
                    {
                        if (bReadCompiledFiles && File.Exists(bamlFileName))
                        {
                            var ret = File.ReadAllBytes(bamlFileName);
                            try
                            {
                                canvas = Utilities.WPF.XmlHelper.LoadBaml<Canvas>(ret);
                            }
                            catch (Exception ex)
                            {
                                if (File.Exists(compiledFileName))
                                {
                                    sourceText = File.ReadAllText(compiledFileName);
                                }
                                else if (File.Exists(fullPath))
                                {
                                    sourceText = File.ReadAllText(fullPath);
                                }
                            }
                        }
                        else if (bReadCompiledFiles && File.Exists(compiledFileName))
                        {
                            sourceText = File.ReadAllText(compiledFileName);
                        }
                        else if (File.Exists(fullPath))
                        {
                            sourceText = File.ReadAllText(fullPath);
                        }
                    } 
                    else if (File.Exists(fullPath))
#endif
                    {
                        sourceText = File.ReadAllText(fullPath);
                    }

#if !WINDOWS_UWP
                    if (!string.IsNullOrEmpty(sourceText) && (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath)))
                    {
                        sourceText = WPFUtilities.CryptString.CryptString.DecryptString(sourceText);
                    }
#endif

#endif

                    if (document == null)
                        document = new ScreenDocument();

#if !NET_STANDARD
                    document.xamlDocument = new XamlDocument(Path.GetDirectoryName(fullPath));
                    if (canvas != null)
                        document.xamlDocument.InitializeCanvas(canvas);
                    else
                        document.xamlDocument.InitializeSourceText(sourceText);

                    document.xamlDocument.FullPath = fullPath;
#endif
                    document.fullPath = fullPath;
                    return document;
                }
            }
            catch(Exception ex)
            {
#if !WINDOWS_UWP
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
#endif
            }

            return null;
        }


        static ScreenDocument GetDocumentSettings(IDocument parent, string fullPath, string settingsBinaryFileName, string settingsFileName)
        {
            ScreenDocument document = null;
            try
            {
                if (!parent.Protected && File.Exists(settingsBinaryFileName))
                {
                    using (var fileStream = new FileStream(settingsBinaryFileName, FileMode.Open, FileAccess.Read))
                    {
                        document = ReadBinaryFromStream(fileStream);
                    }
                }

                if (document == null && File.Exists(settingsFileName))
                {
#if !WINDOWS_UWP
                    if (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(settingsFileName))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(settingsFileName));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            document = ReadFromStream(reader);
                            if (document == null)
                            {
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                    var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                    syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
#else
                                    var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                    syslog.ErrorFormat(Properties.Resources.ErrorValidatingDocument, fullPath);
#endif
                                return null;
                            }
                        }
                    }
                    else
#endif
                    {
                        using (var fileStream = new FileStream(settingsFileName, FileMode.Open, FileAccess.Read))
                        {
                            document = ReadFromStream(fileStream);
#if !WINDOWS_UWP
                            if (document == null)
                            {
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
#endif
                        }
                    }
                }
            }
            catch (Exception)
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
            }
            return document;
        }
#endregion Static Methods

#region Events
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GetTagListEventArgs> GetTagList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnGetTagList(GetTagListEventArgs ea)
        {
            if (GetTagList != null)
                GetTagList(this, ea);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GetPrototypeListEventArgs> GetPrototypeList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnGetPrototypeList(GetPrototypeListEventArgs ea)
        {
            if (GetPrototypeList != null)
                GetPrototypeList(this, ea);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GetTagEntityReference> GetTagEntityReference;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnGetTagEntityReference(GetTagEntityReference ea)
        {
            if (GetTagEntityReference != null)
                GetTagEntityReference(this, ea);
        }

        public event EventHandler<VariableChangedEventArgs> VariableChanged;
        bool bPendingVariableChangedEvent;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnVariableChanged(VariableChangedEventArgs ea)
        {
            if (VariableChanged != null)
            {
                try
                {
                    bPendingVariableChangedEvent = true;
                    VariableChanged(null/*this*/, ea);
                }
                finally
                {
                    bPendingVariableChangedEvent = false;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<CancelEventArgs> CommandExecuting;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnCommandExecuting(Object sender, CancelEventArgs ea)
        {
            if (CommandExecuting != null)
                CommandExecuting(sender, ea);
        }
#endif
#endregion

#region Methods

#if !WINDOWS_UWP
        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }
#endif

        public void UpdateSessionSettings()
        {
            if (String.IsNullOrEmpty(SessionName))
                return;

            String[] serverUriArray = null;
#if !WINDOWS_UWP
            var ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
                serverUriArray = ufuaEditor.GetServerUriArray(this);
#endif
            RealTimeConnectionManagerViewModel.AddSessionSettings(SessionString, new SessionSettings()
            {
                ParentTitle = Parent.Title,
                RemoveDisabledItemAfterSecs = this.RemoveDisabledItemAfterSecs,
                MaxCleanCount = this.MaxCleanCount,
                UseAlwaysSecureConnections = this.UseAlwaysSecureConnections,
                SlowSamplingInterval = this.SlowSamplingInterval,
                DisableWhenNotUsed = this.DisableWhenNotUsed,
                PublishingInterval = this.PublishingInterval,
                ServerArray = serverUriArray,
                FastSamplingInterval = this.FastSamplingInterval
            });
        }

#if !WINDOWS_UWP && !NET_STANDARD
        List<VariableValues> variableValues;
        List<VariableValues> variableValuesRuntime;
        Dictionary<String, VariableValues> mapvariableValuesRuntime;
        List<OPCUAEntityReference> inuseVariableValueReferences;
        List<PropertyObserver<OPCUAEntityReference>> listPropertyObserverEntityReferenceResolveVariable;
        Dictionary<String, PropertyObserver<MonitoredItemViewModel>> listPropertyObserverMonitoredItemResolveVariable;
        Dictionary<OPCUAEntityReference, IEntityReference> mapOPCItemToEntity;
        Dictionary<String, OPCUAEntityReference> mapResolvedVariables;

        bool bTerminateVariableValues;
        internal void TerminateVariableValues()
        {
            bTerminateVariableValues = true;

            lock (lockObjectVariables)
            {
                if (inuseVariableValueReferences != null && mapOPCItemToEntity != null)
                {
                    inuseVariableValueReferences.ToList().ForEach(item =>
                        {
                            item.SetInUse(mapOPCItemToEntity[item], false);
                        });
                    inuseVariableValueReferences.Clear();
                    mapOPCItemToEntity.Clear();
                }
                if (listPropertyObserverMonitoredItemResolveVariable != null)
                {
                    listPropertyObserverMonitoredItemResolveVariable.Values.ToList().ForEach(item =>
                        {
                            if (item != null)
                                item.Dispose();
                        });
                    listPropertyObserverMonitoredItemResolveVariable.Clear();
                }
                if (listPropertyObserverEntityReferenceResolveVariable != null)
                {
                    listPropertyObserverEntityReferenceResolveVariable.ToList().ForEach(item =>
                        {
                            item.Dispose();
                        });
                    listPropertyObserverEntityReferenceResolveVariable.Clear();
                }
                if (mapResolvedVariables != null)
                {
                    mapResolvedVariables.Clear();
                    mapResolvedVariables = null;
                }
                if (variableValuesRuntime != null)
                {
                    var list = variableValuesRuntime.ToList();
                    variableValuesRuntime.Clear();
                    mapvariableValuesRuntime.Clear();
                    variableValuesRuntime = null;
                    list.ForEach(value =>
                    {
                        value.TerminateResolvedItem();
                    });
                }
            }
        }

        static readonly String qualityTag = "{0}Quality";
        static readonly String timestampTag = "{0}Timestamp";
        static readonly String qualityTagName = "Quality";
        static readonly String timestampTagName = "Timestamp";

        VariableValues SubscribeVariableValuesChanged(IEntityReference reference, String instance = null, bool isDataService = false)
        {
            VariableValues varvalues = null;
            if (String.IsNullOrEmpty(instance))
                varvalues = new VariableValues();
            else
                varvalues = new VariableValues(instance);

            varvalues.WriteVariable += (o, e) =>
            {
                if (bPendingVariableChangedEvent)
                    throw new StackOverflowException("Cannot set a variable value inside a variable value changed event");

                var instanceName = e.Name;
                if (!String.IsNullOrEmpty(instance))
                    instanceName = String.Format("{0}-{1}", instance, e.Name);

                try
                {
                    bool bFound = false;
                    var dateTime = DateTime.Now;
                    int timetoWait = WriteTimeout < 5000 ? 5000 : WriteTimeout;
                    var timeTo = dateTime.AddMilliseconds(timetoWait);
                    do
                    {
                        lock (lockObjectVariables)
                        {
                            if (mapResolvedVariables != null &&
                                mapResolvedVariables.ContainsKey(instanceName) &&
                                mapResolvedVariables[instanceName].MonitoredItemViewModel != null)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        // WaitForPriority.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    } while (timetoWait > 0 && timeTo > DateTime.Now);

                    if (bFound)
                    {
                        try
                        {
                            if (!ForceWritingOnServer && e.Value != null)
                            {
                                var v = Convert.ToString(e.Value);
                                if (e.Value is Array)
                                {
                                    var variant = new Opc.Ua.Variant(e.Value);
                                    v = String.Format(CultureInfo.InvariantCulture, "{0}", variant);
                                }
                                if (mapResolvedVariables[instanceName].MonitoredItemViewModel.IsValueEqual(v))
                                    return;
                            }
                        }
                        catch { }

                        dateTime = DateTime.Now;
                        timeTo = dateTime.AddMilliseconds(WriteTimeout > 0 ? WriteTimeout : 0);
                        Exception ex = null;
                        do
                        {
                            try
                            {
                                mapResolvedVariables[instanceName].MonitoredItemViewModel.WriteValue(e.Value);
                                ex = null;
                                break;
                            }
                            catch (Exception exception)
                            {
                                ex = exception;
                                if (WriteTimeout > 0)
                                    Thread.Sleep(500);
                                else
                                    break;
                            }
                        } while (WriteTimeout > 0 && timeTo > DateTime.Now);

                        if (ex != null)
                            throw ex;

                        varvalues.SetVariableValue(e.Name, e.Value);

                        // OnVariableChanged(new VariableChangedEventArgs(instanceName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue));
                    }
                    else
                        throw new Exception(String.Format("Cannot find the variable {0} for writing the value {1}", instanceName, e.Value));
                }
                catch (Exception ex)
                {
                    var syslog = LogManager.GetLogger(Title);
                    syslog.Error(String.Format(Properties.Resources.WriteVariableException, instanceName), ex);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(this, Properties.Resources.LicenseManager,
                        DateTime.UtcNow, $"{String.Format(Properties.Resources.WriteVariableException, instanceName)}: {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                    throw ex;
                }
            };

            var mapLocalResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
            varvalues.ResolveVariable += (o, e) =>
            {
                var varName = e.Name;
                var instanceName = e.Name;
                if (!String.IsNullOrEmpty(instance))
                    instanceName = String.Format("{0}-{1}", instance, e.Name);

                lock (lockObjectVariables)
                {
                    if (mapResolvedVariables == null)
                        mapResolvedVariables = new Dictionary<String, OPCUAEntityReference>();
                    if (!mapResolvedVariables.ContainsKey(instanceName))
                    {
                        if (mapLocalResolvedVariables.ContainsKey(instanceName))
                            mapResolvedVariables.Add(instanceName, mapLocalResolvedVariables[instanceName]);
                        else
                        {
                            if (isDataService)
                            {
                                var data = OPCUAEntityReference.GetDataSinkInterface(instance);
                                if (data != null)
                                {
                                    string name = e.Name.Replace('\\', '&');
                                    var entity = data.GetReference(name);
                                    if (entity != null)
                                        mapResolvedVariables.Add(instanceName, entity);
                                    else
                                        throw new Exception("Tag not found !");
                                }
                            }
                            else
                            {
                                var original = e.Name;
                                //var originalCounter = 1;
                                //var secondloop = false;
                                var nameToCheck = e.Name;
                                //while (true)
                                //{
                                var eaEntity = new GetTagEntityReference(nameToCheck, instance);
                                OnGetTagEntityReference(eaEntity);
                                if (eaEntity.entityReference != null)
                                {
                                    // varName = nameToCheck;
                                    if (!mapResolvedVariables.ContainsKey(instanceName))
                                        mapResolvedVariables.Add(instanceName, eaEntity.entityReference);
                                    // break;
                                }
                                else
                                {
                                    if (nameToCheck.EndsWith(qualityTagName))
                                    {
                                        var nameNoQuality = nameToCheck.Substring(0, nameToCheck.Length - qualityTagName.Length);
                                        eaEntity = new GetTagEntityReference(nameNoQuality, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                        {
                                        // varName = nameNoQuality;
                                            varName = instanceName = nameNoQuality;
                                            mapResolvedVariables.Add(nameNoQuality, eaEntity.entityReference);
                                            // break;
                                        }
                                        else
                                            throw new Exception("Tag not found !");
                                    }
                                    else if (nameToCheck.EndsWith(timestampTagName))
                                    {
                                        var nameNoTimestamp = e.Name.Substring(0, nameToCheck.Length - timestampTagName.Length);
                                        eaEntity = new GetTagEntityReference(nameNoTimestamp, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                        {
                                        // varName = nameNoTimestamp;
                                            varName = instanceName = nameNoTimestamp;
                                            mapResolvedVariables.Add(nameNoTimestamp, eaEntity.entityReference);
                                            // break;
                                        }
                                        else
                                            throw new Exception("Tag not found !");
                                    }
                                    else
                                        throw new Exception("Tag not found !");

                                    //    var replace = RegexExt.ReplaceFirst(nameToCheck, "_", "\\");
                                    //    if (replace == nameToCheck || secondloop)
                                    //    {
                                    //        restartLoop:
                                    //        if (originalCounter == -1)
                                    //            originalCounter = original.Split(new char[] { '_' }).Length;
                                    //        if (originalCounter < 2)
                                    //        {
                                    //            if (secondloop)
                                    //                throw new Exception("Tag not found !");
                                    //            else
                                    //            {
                                    //                secondloop = true;
                                    //                originalCounter = -1;
                                    //                nameToCheck = original;
                                    //                goto restartLoop;
                                    //            }
                                    //        }

                                    //        var indexof = RegexExt.IndexOfNth(original, "_", 0, --originalCounter);
                                    //        if (indexof == -1)
                                    //        {
                                    //            if (secondloop)
                                    //                throw new Exception("Tag not found !");
                                    //            else
                                    //            {
                                    //                secondloop = true;
                                    //                originalCounter = -1;
                                    //                nameToCheck = original;
                                    //                goto restartLoop;
                                    //            }
                                    //        }

                                    //        var sb = new StringBuilder(secondloop ? nameToCheck : original);
                                    //        sb[indexof] = '\\';
                                    //        nameToCheck = sb.ToString();
                                    //    }
                                    //    else
                                    //        nameToCheck = replace;
                                    //}
                                }
                            }
                        }
                    }
                }

                if (mapResolvedVariables.ContainsKey(instanceName))
                {
                    if (inuseVariableValueReferences == null)
                        inuseVariableValueReferences = new List<OPCUAEntityReference>();
                    if (mapOPCItemToEntity == null)
                        mapOPCItemToEntity = new Dictionary<OPCUAEntityReference, IEntityReference>();

                    if (!inuseVariableValueReferences.Contains(mapResolvedVariables[instanceName]))
                    {
                        varvalues.AddVariable(varName, false);
                        varvalues.AddVariable(String.Format(qualityTag, varName), false, true);
                        varvalues.AddVariable(String.Format(timestampTag, varName), false, true);

                        var observer = new PropertyObserver<OPCUAEntityReference>(mapResolvedVariables[instanceName]);
                        if (listPropertyObserverEntityReferenceResolveVariable == null)
                            listPropertyObserverEntityReferenceResolveVariable = new List<PropertyObserver<OPCUAEntityReference>>();
                        listPropertyObserverEntityReferenceResolveVariable.Add(observer);
                        observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                        {
                            if (n.MonitoredItemViewModel == null)
                                return;

                            // observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                            var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                            lock (lockObjectVariables)
                            {
                                //listPropertyObserverEntityReferenceResolveVariable.Remove(observer);
                                //observer.Dispose();
                                if (listPropertyObserverMonitoredItemResolveVariable == null)
                                    listPropertyObserverMonitoredItemResolveVariable = new Dictionary<String, PropertyObserver<MonitoredItemViewModel>>();
                                else if (listPropertyObserverMonitoredItemResolveVariable.ContainsKey(instanceName))
                                {
                                    listPropertyObserverMonitoredItemResolveVariable[instanceName].Dispose();
                                    listPropertyObserverMonitoredItemResolveVariable.Remove(instanceName);
                                }
                                listPropertyObserverMonitoredItemResolveVariable.Add(instanceName, observerMonitoredModel);
                            }
                            observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                            {
                                varvalues.SetVariableValue(varName, m.DataValue.Value);
                                varvalues.SetVariableValue(String.Format(qualityTag, varName), m.DataValue.StatusCode.ToString());
                                varvalues.SetVariableValue(String.Format(timestampTag, varName), m.DataValue.SourceTimestamp);

                                // pump read attributes outside UI thread
                                try
                                {
                                    var name = n.NodeIdViewModel.BrowseName.Name;
                                    var path = n.NodeIdViewModel.CompletePath;
                                }
                                catch { }

                                Action action1 = () =>
                                {
                                    OnVariableChanged(new VariableChangedEventArgs(instanceName, m.DataValue));

                                    var value = String.Empty;
                                    try
                                    {
                                        var dataMap = dataContextExpando as IDictionary<String, Object>;
                                        if (m.Value != null)
                                            value = m.Value;
                                        if (n.NodeIdViewModel != null)
                                        {
                                            dataMap[n.NodeIdViewModel.BrowseName.Name] = value;
                                            dataMap[n.NodeIdViewModel.CompletePath] = value;
                                        }
                                        else
                                            dataMap[n.RelativePath] = value;
                                        dataMap[mapResolvedVariables[instanceName].HumanReadableNoProject] = value;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                };
                                if (ActiveView != null)
                                    ActiveView.Dispatcher.BeginInvokeIfRequired(action1);
                                else if (feparentExecute !=  null)
                                    feparentExecute.Dispatcher.BeginInvokeIfRequired(action1);
                                else
                                    action1();
                            });

                            if (n.MonitoredItemViewModel != null && n.MonitoredItemViewModel.DataValue != null &&
                                n.MonitoredItemViewModel.DataValue.Value != null)
                            {
                                varvalues.SetVariableValue(varName, n.MonitoredItemViewModel.DataValue.Value);
                                varvalues.SetVariableValue(String.Format(qualityTag, varName), n.MonitoredItemViewModel.DataValue.StatusCode.ToString());
                                varvalues.SetVariableValue(String.Format(timestampTag, varName), n.MonitoredItemViewModel.DataValue.SourceTimestamp);

                                OnVariableChanged(new VariableChangedEventArgs(instanceName, n.MonitoredItemViewModel.DataValue));
                            }
                        });

                        //MonitoredItemViewModel temporary = null;
                        //if (mapDataContextExpando.ContainsKey(mapResolvedVariables[instanceName].HumanReadableNoProject))
                        //    temporary = mapDataContextExpando[mapResolvedVariables[instanceName].HumanReadableNoProject];
                        mapResolvedVariables[instanceName].Resolve(SessionString/*, temporary*/, parent);
                        mapResolvedVariables[instanceName].SetInUse(reference, true);
                        mapOPCItemToEntity.Add(mapResolvedVariables[instanceName], reference);
                        inuseVariableValueReferences.Add(mapResolvedVariables[instanceName]);

                        var valueData = String.Empty;
                        if (mapResolvedVariables[instanceName].MonitoredItemViewModel != null &&
                            mapResolvedVariables[instanceName].MonitoredItemViewModel.Value != null)
                        {
                            valueData = mapResolvedVariables[instanceName].MonitoredItemViewModel.Value;
                            varvalues.SetVariableValue(String.Format(qualityTag, varName), mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.StatusCode.ToString());
                            varvalues.SetVariableValue(String.Format(timestampTag, varName), mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.SourceTimestamp);
                            varvalues.SetVariableValue(varName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue.Value);

                            OnVariableChanged(new VariableChangedEventArgs(instanceName, mapResolvedVariables[instanceName].MonitoredItemViewModel.DataValue));
                        }
                        else
                        {
                            varvalues.SetVariableValue(String.Format(qualityTag, varName), VariableValues.initialQuality);
                            varvalues.SetVariableValue(String.Format(timestampTag, varName), DateTime.MinValue);

                            OnVariableChanged(new VariableChangedEventArgs(instanceName, null));
                        }

                        Action action = () =>
                        {
                            try
                            {
                                var dataMap = dataContextExpando as IDictionary<String, Object>;
                                if (mapResolvedVariables[instanceName].NodeIdViewModel != null)
                                {
                                    dataMap[mapResolvedVariables[instanceName].NodeIdViewModel.BrowseName.Name] = valueData;
                                    dataMap[mapResolvedVariables[instanceName].NodeIdViewModel.CompletePath] = valueData;
                                }
                                else
                                    dataMap[mapResolvedVariables[instanceName].RelativePath] = valueData;
                                dataMap[mapResolvedVariables[instanceName].HumanReadableNoProject] = valueData;
                            }
                            catch (Exception ex)
                            {

                            }
                        };
                        if (ActiveView != null)
                            ActiveView.Dispatcher.BeginInvokeIfRequired(action);
                        else if (feparentExecute != null)
                            feparentExecute.Dispatcher.BeginInvokeIfRequired(action);
                        else
                            action();
                    }
                }
            };

            if (listScriptVariableUsed != null)
            {
                var found = (from c in listScriptVariableUsed.AsParallel()
                             where (String.IsNullOrEmpty(instance) && !c.Contains("-") ||
                                    c.StartsWith(String.Format("{0}-", instance)))
                             select c).ToList();
                found.ForEach(var =>
                {
                    var instanceName = var;
                    if (!String.IsNullOrEmpty(instance))
                        var = var.Replace(String.Format("{0}-", instance), "");

                    if (!mapLocalResolvedVariables.ContainsKey(instanceName))
                    {
                        if (isDataService)
                        {
                            var data = OPCUAEntityReference.GetDataSinkInterface(instance);
                            if (data != null)
                            {
                                string name = var.Replace('\\', '&');
                                var entity = data.GetReference(name);
                                if (entity != null)
                                    mapLocalResolvedVariables.Add(instanceName, entity);
                            }
                        }
                        else
                        {
                            var original = var;
                            var nameToCheck = var;
                            var eaEntity = new GetTagEntityReference(nameToCheck, instance);
                            OnGetTagEntityReference(eaEntity);
                            if (eaEntity.entityReference != null)
                                mapLocalResolvedVariables.Add(instanceName, eaEntity.entityReference);
                            else
                            {
                                if (nameToCheck.EndsWith(qualityTagName))
                                {
                                    var nameNoQuality = nameToCheck.Substring(0, nameToCheck.Length - qualityTagName.Length);
                                    if (!mapLocalResolvedVariables.ContainsKey(nameNoQuality))
                                    {
                                        eaEntity = new GetTagEntityReference(nameNoQuality, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                            mapLocalResolvedVariables.Add(nameNoQuality, eaEntity.entityReference);
                                    }
                                }
                                else if (nameToCheck.EndsWith(timestampTagName))
                                {
                                    var nameNoTimestamp = var.Substring(0, nameToCheck.Length - timestampTagName.Length);
                                    if (!mapLocalResolvedVariables.ContainsKey(nameNoTimestamp))
                                    {
                                        eaEntity = new GetTagEntityReference(nameNoTimestamp, instance);
                                        OnGetTagEntityReference(eaEntity);
                                        if (eaEntity.entityReference != null)
                                            mapLocalResolvedVariables.Add(nameNoTimestamp, eaEntity.entityReference);
                                    }
                                }
                            }
                        }
                    }
                });

                if (mapLocalResolvedVariables.Count > 0)
                {
                    foreach (var var in mapLocalResolvedVariables.Keys)
                    {
                        if (String.IsNullOrEmpty(instance))
                            varvalues.OnResolveVariable(new ResolveVariableEventArgs(var));
                        else
                            varvalues.OnResolveVariable(new ResolveVariableEventArgs(var.Replace(String.Format("{0}-", instance), "")));
                    }
                    mapLocalResolvedVariables.Clear();
                }
            }

            return varvalues;
        }

        Object lockObjectVariables = new Object();
        internal List<VariableValues> GetVariableObjectDispatcher(IEntityReference reference)
        {
            var ret = new List<VariableValues>();

            lock (lockObjectVariables)
            {
                if (inExecution)
                {
                    if (variableValuesRuntime == null)
                    {
                        variableValuesRuntime = new List<VariableValues>();
                        mapvariableValuesRuntime = new Dictionary<String, VariableValues>();
                        var varvalues = SubscribeVariableValuesChanged(reference);
                        variableValuesRuntime.Add(varvalues);

                        ret.Add(varvalues);

                        //varvalues.FoundVariableNameSuspect += (o, e) =>
                        //{
                        //    var ea = new GetTagListEventArgs();
                        //    OnGetTagList(ea);
                        //    varvalues.PrepareAddNewVariable();
                        //    if (ea.list != null)
                        //    {
                        //        foreach (var tag in ea.list)
                        //        {
                        //            varvalues.AddVariable(tag);
                        //            varvalues.AddVariable(String.Format(qualityTag, tag), false);
                        //            varvalues.AddVariable(String.Format(timestampTag, tag), false);
                        //        }
                        //    }
                        //    varvalues.EndAddNewVariable();
                        //};

                        //var ea = new GetTagListEventArgs();
                        //OnGetTagList(ea);
                        var eap = new GetPrototypeListEventArgs();
                        OnGetPrototypeList(eap);
                        /*
                        varvalues.PrepareAddNewVariable();
                        if (ea.list != null)
                        {
                            foreach (var tag in ea.list)
                            {
                                varvalues.AddVariable(tag);
                                varvalues.AddVariable(String.Format(qualityTag, tag), false);
                                varvalues.AddVariable(String.Format(timestampTag, tag), false);
                            }
                        }
                        varvalues.EndAddNewVariable();
                        */

                        if (eap.mapPrototypes != null && eap.mapDefinitions != null)
                        {
                            foreach (var instance in eap.mapPrototypes.Keys)
                            {
                                if (eap.mapPrototypes[instance] == null || !eap.mapDefinitions.ContainsKey(eap.mapPrototypes[instance]))
                                    continue;
                                var prototypesValues = SubscribeVariableValuesChanged(reference, instance);
                                variableValuesRuntime.Add(prototypesValues);
                                mapvariableValuesRuntime.Add(instance, prototypesValues);
                                /*
                                prototypesValues.PrepareAddNewVariable();

                                foreach (var member in eap.mapDefinitions[eap.mapPrototypes[instance]])
                                {
                                    prototypesValues.AddVariable(member);
                                    prototypesValues.AddVariable(String.Format(qualityTag, member), false);
                                    prototypesValues.AddVariable(String.Format(timestampTag, member), false);
                                }
                                prototypesValues.EndAddNewVariable();
                                */
                                ret.Add(prototypesValues);
                            }
                        }

                        var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                        listDataSinkInterfaces.ForEach(dataInterface =>
                        {
                            var prototypesValues = SubscribeVariableValuesChanged(reference, dataInterface, true);
                            variableValuesRuntime.Add(prototypesValues);
                            mapvariableValuesRuntime.Add(dataInterface, prototypesValues);
                            prototypesValues.PrepareAddNewVariable();

                            var data = OPCUAEntityReference.GetDataSinkInterface(dataInterface);
                            foreach (var member in data.GetVariables(this))
                            {
                                var _member = member.Replace('&', '\\');
                                prototypesValues.AddVariable(_member);
                                prototypesValues.AddVariable(String.Format(qualityTag, _member), false, true);
                                prototypesValues.AddVariable(String.Format(timestampTag, _member), false, true);
                            }
                            prototypesValues.EndAddNewVariable();
                            prototypesValues.isDataService = true;
                            ret.Add(prototypesValues);
                        });
                    }
                    else
                        ret.AddRange(variableValuesRuntime);
                }
                else
                {
                    if (variableValues == null)
                        variableValues = new List<VariableValues>();
                    variableValues.ForEach(value =>
                        {
                            value.Dispose();
                        });
                    variableValues.Clear();

                    var varvalue = new VariableValues(false);
                    variableValues.Add(varvalue);

                    var ea = new GetTagListEventArgs();
                    OnGetTagList(ea);
                    var eap = new GetPrototypeListEventArgs();
                    OnGetPrototypeList(eap);
                    varvalue.PrepareAddNewVariable();
                    if (ea.list != null)
                    {
                        var list = ea.list.ToList();
                        varvalue.AddList(list);
                        //foreach (var tag in ea.list)
                        //{
                        //    varvalue.AddVariable(tag);
                        //    varvalue.AddVariable(String.Format(qualityTag, tag), false, true);
                        //    varvalue.AddVariable(String.Format(timestampTag, tag), false, true);
                        //}
                    }
                    varvalue.EndAddNewVariable();
                    ret.Add(varvalue);

                    if (eap.mapPrototypes != null && eap.mapDefinitions != null)
                    {
                        foreach (var instance in eap.mapPrototypes.Keys)
                        {
                            if (eap.mapPrototypes[instance] == null || !eap.mapDefinitions.ContainsKey(eap.mapPrototypes[instance]))
                                continue;
                            var prototypesValues = new VariableValues(instance, false);
                            variableValues.Add(prototypesValues);
                            prototypesValues.PrepareAddNewVariable();

                            foreach (var member in eap.mapDefinitions[eap.mapPrototypes[instance]])
                            {
                                prototypesValues.AddVariable(member);
                                prototypesValues.AddVariable(String.Format(qualityTag, member), false, true);
                                prototypesValues.AddVariable(String.Format(timestampTag, member), false, true);
                            }
                            prototypesValues.EndAddNewVariable();
                            ret.Add(prototypesValues);
                        }
                    }

                    var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                    listDataSinkInterfaces.ForEach(dataInterface =>
                    {
                        var prototypesValues = new VariableValues(dataInterface, false);
                        variableValues.Add(prototypesValues);
                        prototypesValues.PrepareAddNewVariable();

                        var data = OPCUAEntityReference.GetDataSinkInterface(dataInterface);
                        foreach (var member in data.GetVariables(this))
                        {
                            var _member = member.Replace('&', '\\');

                            prototypesValues.AddVariable(_member);
                            prototypesValues.AddVariable(String.Format(qualityTag, _member), false, true);
                            prototypesValues.AddVariable(String.Format(timestampTag, _member), false, true);
                        }
                        prototypesValues.EndAddNewVariable();
                        prototypesValues.isDataService = true;
                        ret.Add(prototypesValues);
                    });
                }
            }

            return ret;
        }

        VariableValues GetInstanceVariableValues(String name)
        {
            if (inExecution && variableValuesRuntime == null)
                GetVariableObjectDispatcher(this);

            if (variableValuesRuntime == null || variableValuesRuntime.Count < 1)
                return null;

            var names = name.Replace(':', '.').Split('.');
            if (names.Length < 2)
                return variableValuesRuntime[0];
            if (mapvariableValuesRuntime.ContainsKey(names[0]))
                return mapvariableValuesRuntime[names[0]];
            return null;
        }

        String GetInstanceVariableName(String name)
        {
            if (variableValuesRuntime == null || variableValuesRuntime.Count < 1)
                return name;

            var names = name.Replace(':', '.').Split('.');
            if (names.Length < 2)
                return name;
            return names[1];
        }

        public Object GetVariableValue(String Name)
        {
            if (AliasHelper.ContainsAlias(Name))
                Name = Alias.ReplaceAlias(Name, mapAlias, this);

            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');
            variableValue.VerifyResolvedVariable(name);
            Object ret = variableValue.GetVariableValue(name);
            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            while (ret == null && timeout > DateTime.UtcNow)
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name)) as String;
                if (!String.IsNullOrEmpty(quality) && quality.StartsWith("Good"))
                    break;

                Thread.Sleep(200);
                ret = variableValue.GetVariableValue(name);
            }

            return ret;
        }

        public Object GetVariableQuality(String Name)
        {
            if (AliasHelper.ContainsAlias(Name))
                Name = Alias.ReplaceAlias(Name, mapAlias, this);

            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');
            variableValue.VerifyResolvedVariable(name);
            Object ret = variableValue.GetVariableValue(String.Format(qualityTag, name));

            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            var quality = ret as String;
            while ((quality == null || quality == VariableValues.initialQuality) && timeout > DateTime.UtcNow)
            {
                Thread.Sleep(200);
                ret = variableValue.GetVariableValue(String.Format(qualityTag, name));
                quality = ret as String;
            }

            return ret;
        }

        public bool IsVariableQualityGood(String Name)
        {
            if (AliasHelper.ContainsAlias(Name))
                Name = Alias.ReplaceAlias(Name, mapAlias, this);

            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return false;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            try
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name));
                var iQuality = Convert.ToUInt32(quality);
                var statusCode = new Opc.Ua.StatusCode(iQuality);
                return Opc.Ua.StatusCode.IsGood(statusCode);
            }
            catch (Exception ex)
            {
                var quality = variableValue.GetVariableValue(String.Format(qualityTag, name));
                if (quality is String)
                {
                    var strQuality = quality as String;
                    var statusCode = new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.Good);
                    var compare = statusCode.ToString();
                    if (strQuality == compare)
                        return true;
                }

                return false;
            }
        }

        public Object GetVariableTimestamp(String Name)
        {
            if (AliasHelper.ContainsAlias(Name))
                Name = Alias.ReplaceAlias(Name, mapAlias, this);

            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            Object ret = variableValue.GetVariableValue(String.Format(timestampTag, name));

            var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            var timestamp = ret is DateTime ? (DateTime)ret : DateTime.MinValue;
            while (timestamp == DateTime.MinValue && timeout > DateTime.UtcNow)
            {
                Thread.Sleep(200);
                ret = variableValue.GetVariableValue(String.Format(timestampTag, name));
                timestamp = ret is DateTime ? (DateTime)ret : DateTime.MinValue;
            }

            return ret;
        }

        public Object SetVariableValue(String Name, Object value)
        {
            if (AliasHelper.ContainsAlias(Name))
                Name = Alias.ReplaceAlias(Name, mapAlias, this);

            var variableValue = GetInstanceVariableValues(Name);
            if (variableValue == null)
                return null;
            var name = GetInstanceVariableName(Name);
            // Name = Name.Replace('_', '\\');

            variableValue.VerifyResolvedVariable(name);
            variableValue.OnWriteVariable(new WriteVariableEventArgs(name, value));
            return variableValue.GetVariableValue(name);
        }
#endif

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void LoadResources(FrameworkElement fe)
        {
            if (listResources == null)
                return;

            if (ListResources.Count > 0 && mapLoadedResources == null)
                mapLoadedResources = new Dictionary<String, ResourceDictionary>();

            ListResources.ForEach(resource =>
                {
                    try
                    {
                        ResourceDictionary dict = new ResourceDictionary
                        {
                            Source = new Uri(resource, UriKind.RelativeOrAbsolute)
                        };
                        fe.Resources.MergedDictionaries.Add(dict);
                    }
                    catch (Exception ex)
                    {
#if !WINDOWS_UWP
                        MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    }
                });


            var list = (from entry in MapScreenEntities.AsParallel()
                        where !String.IsNullOrEmpty(entry.Value.StyleResource)
                        select entry.Value).ToList();
            list.ForEach(resource =>
                {
#if !WINDOWS_UWP
                    if (fe.Resources.Contains(resource.StyleResource))
                        fe.Resources.Remove(resource.StyleResource);
                    var res = fe.TryFindResource(resource.StyleResource);
#else
                    if (fe.Resources.ContainsKey(resource.StyleResource))
                        fe.Resources.Remove(resource.StyleResource);
                    Object res;
                    fe.Resources.TryGetValue(resource.StyleResource, out res);
#endif
                    fe.Resources.Add(resource.StyleResource, res);
                });
        }
#endif
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetImagesBaseUri(Canvas canvas, bool bRecursive = false)
        {
            string dest = System.IO.Path.GetDirectoryName(FilePath) + "\\";
            var AbsolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
            var AbsolutePath2 = GetSpecialFolder(SpecialFolders.Images);

            if (canvas.Background is VisualBrush &&
                                                    (canvas.Background as VisualBrush).Visual is MediaElement)
            {
                var oldme = (canvas.Background as VisualBrush).Visual as MediaElement;
                var me = new MediaElement();
                me.Source = oldme.Source;
                var listme = new List<MediaElement>() { me };
                SetBaseUri(listme, AbsolutePath, AbsolutePath2);
                Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listme);

                canvas.Background = new VisualBrush(me);
            }
            else if (canvas.Background is VisualBrush &&
                                                    (canvas.Background as VisualBrush).Visual is Image)
            {
                var image = (canvas.Background as VisualBrush).Visual as Image;
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                        where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                        select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = AbsolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = AbsolutePath2;
                    binding.UpdateTarget();
                });
            }

            if (bRecursive)
            {
                var listChildren = canvas.GetChildrenOfType<UIElement>().ToList();
                SetImagesBaseUri(listChildren);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetImagesBaseUri(List<UIElement> listChildren)
        {
            string dest = System.IO.Path.GetDirectoryName(FilePath) + "\\";
            var AbsolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
            var AbsolutePath2 = GetSpecialFolder(SpecialFolders.Images);

            var listImages = (from c in listChildren.OfType<Image>() select c).ToList();
            var listImagesToSet = new List<BitmapImage>();
            foreach (var c in listImages)
            {
                try
                {
                    if (c.Source != null && c.Source is BitmapImage &&
                    (c.Source as BitmapImage).UriSource != null &&
                    !(c.Source as BitmapImage).UriSource.IsAbsoluteUri &&
                    (c.Source as BitmapImage).UriSource.IsFile &&
                    !File.Exists((c.Source as BitmapImage).UriSource.GetPathString()))
                    {
                        listImagesToSet.Add(c.Source as BitmapImage);
                    }
                }
                catch { }
            }
            
            SetBaseUri(listImagesToSet);
            listImagesToSet.ForEach(bmp =>
            {
                var name = System.IO.Path.GetFileName(bmp.UriSource.GetPathString());
                var uriFound = new Uri(name, UriKind.RelativeOrAbsolute);
                bmp.UriSource = uriFound;
            });

            var listBorders = (from c in listChildren.OfType<Border>() select c).ToList(); ;
            var listBordersWithMediaBrush = (from c in listBorders
                                             where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is MediaElement
                                            select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            SetBaseUri(listBordersWithMediaBrush, AbsolutePath, AbsolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listBordersWithMediaBrush);

            var listBordersWithImageBrush = (from c in listBorders
                                            where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is Image
                                            select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listBordersWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                      where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                      select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = AbsolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = AbsolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listShapes = (from c in listChildren.OfType<System.Windows.Shapes.Shape>() select c).ToList(); ;
            var listShapesWithMediaBrush = (from c in listShapes
                                            where c.Fill is VisualBrush &&
                                                  (c.Fill as VisualBrush).Visual is MediaElement
                                            select ((c.Fill as VisualBrush).Visual as MediaElement)).ToList();
            SetBaseUri(listShapesWithMediaBrush, AbsolutePath, AbsolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listShapesWithMediaBrush);

            var listShapesWithImageBrush = (from c in listShapes
                                            where c.Fill is VisualBrush &&
                                                  (c.Fill as VisualBrush).Visual is Image
                                            select ((c.Fill as VisualBrush).Visual as Image)).ToList();
            listShapesWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                      where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                      select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = AbsolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = AbsolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listControls = (from c in listChildren.OfType<Control>() select c).ToList();
            var listControlsWithMediaBrush = (from c in listControls
                                              where c.Background is VisualBrush &&
                                                    (c.Background as VisualBrush).Visual is MediaElement
                                              select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            SetBaseUri(listControlsWithMediaBrush, AbsolutePath, AbsolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listControlsWithMediaBrush);

            var listControlsWithImageBrush = (from c in listControls
                                              where c.Background is VisualBrush &&
                                                    (c.Background as VisualBrush).Visual is Image
                                              select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listControlsWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                      where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                      select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = AbsolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = AbsolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listPanels = (from c in listChildren.OfType<Panel>() select c).ToList();
            var listPanelsWithMediaBrush = (from c in listPanels
                                            where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is MediaElement
                                            select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            SetBaseUri(listPanelsWithMediaBrush, AbsolutePath, AbsolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listPanelsWithMediaBrush);

            var listPanelsWithImageBrush = (from c in listPanels
                                            where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is Image
                                            select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listPanelsWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                      where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                      select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = AbsolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = AbsolutePath2;
                    binding.UpdateTarget();
                });
            });
        }

        public void SetBaseUri(List<MediaElement> list, Uri uri1, Uri uri2)
        {
            var dest = System.IO.Path.GetDirectoryName(FullPath) + "\\";
            list.ForEach(element =>
            {
                try
                {
                    var uridest = new Uri(uri1, element.Source);
                    if (File.Exists(uridest.GetPathString()))
                        (element as IUriContext).BaseUri = uri1;
                    else
                    {
                        uridest = new Uri(uri2, element.Source);
                        if (File.Exists(uridest.GetPathString()))
                            (element as IUriContext).BaseUri = uri2;
                        else
                            (element as IUriContext).BaseUri = new Uri(dest, UriKind.RelativeOrAbsolute);
                    }
                }
                catch
                {
                    if (fileSystemProviderBase != null)
                    {
                        try
                        {
                            var vfsFile = new FileManagerFile(fileSystemProviderBase, new FileManagerFolder(fileSystemProviderBase, uri1.GetPathString()), element.Source.GetPathString());
                            if (!fileSystemProviderBase.Exists(vfsFile))
                                vfsFile = new FileManagerFile(fileSystemProviderBase, new FileManagerFolder(fileSystemProviderBase, uri2.GetPathString()), element.Source.GetPathString());
                            if (!fileSystemProviderBase.Exists(vfsFile))
                                return;

                            var destFile = Parent != null ? Parent.Title : Title;
                            foreach (var c in System.IO.Path.GetInvalidPathChars())
                                destFile = destFile.Replace(c, '_');
                            destFile = String.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), destFile, vfsFile.FullName);
                            if (!File.Exists(destFile))
                            {
                                var data = fileSystemProviderBase.ReadFile(vfsFile);
                                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                                File.WriteAllBytes(destFile, data);
                            }

                            dest = String.Format("{0}\\", Path.GetDirectoryName(destFile));
                            (element as IUriContext).BaseUri = new Uri(dest, UriKind.RelativeOrAbsolute);
                        }
                        catch
                        { }
                    }
                }
            });
        }

        public void SetBaseUri(List<BitmapImage> list)
        {
            var dest = System.IO.Path.GetDirectoryName(FullPath) + "\\";
            list.ForEach(element =>
            {
                (element as IUriContext).BaseUri = new Uri(dest, UriKind.RelativeOrAbsolute);
            });
        }
#endif
        public List<String> GetEnableManipulationList()
        {
            var list = (from entry in MapScreenEntities.AsParallel()
                        where entry.Value.EnableManipulation
                        select entry.Key).ToList();
            return list;
        }

#if !WINDOWS_UWP
        public List<String> GetContextMenuList()
        {
            var list = (from entry in MapScreenEntities.AsParallel()
                        where entry.Value.MenuName != null
                        select entry.Key).ToList();
            return list;
        }
#endif
        public List<String> GetEnableMouseOverList()
        {
            var list = (from entry in MapScreenEntities.AsParallel()
                        where entry.Value.EnableMouseOver
                        select entry.Key).ToList();
            return list;
        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnloadResources(FrameworkElement fe)
        {
            if (mapLoadedResources == null)
                return;

            foreach (var resource in mapLoadedResources.Values)
            {
                if (fe.Resources.MergedDictionaries.Contains(resource))
                    fe.Resources.MergedDictionaries.Remove(resource);
            }

            mapLoadedResources = null;
        }

        public void RenameReferences(Dictionary<string, string> nodeIdMap, UFInterfaces.Editors.CrossReferenceModel model)
        {
            using (var cursor = new WaitCursor())
            {
                var map = GetDynamicMapForElementAndChilds();
                List<OPCUAViewModel.OPCUAEntityReference> list = new List<OPCUAViewModel.OPCUAEntityReference>();
                var mapTagChanged = new Dictionary<String, String>();
                foreach (var el in map.Keys)
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    System.Threading.Tasks.Parallel.ForEach(map[el], item => { item.Name = el; });
                    list.AddRange(map[el]);
                }

                bool bDirty = false;
                var UFUAEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (UFUAEditor != null)
                {
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null && c.IsValid
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var preMap = (from n in nodeIdMap
                                  where nodelist.Contains(n.Key)
                                  select n).ToDictionary(x => x.Key, x => x.Value);
                    var toGetList = (from c in nodelist
                                      where !preMap.ContainsKey(c)
                                      select c).Distinct().ToList();

                    var mapNodes = UFUAEditor.GetListNodeNames(this, toGetList);
                    if (mapNodes == null)
                        return;
                    mapNodes.ToList().ForEach(n => nodeIdMap.Add(n.Key, n.Value));
                    preMap.ToList().ForEach(n => mapNodes.Add(n.Key, n.Value));

                    var aplicationName = UFUAEditor.GetAplicationName(this, true);
                    var endpointslist = UFUAEditor.GetEndpoints(this);
                    var defaultlocalendpoint = UFUAEditor.GetDefaultLocalEndpoint(this, true);
                    foreach (var node in mapNodes.Keys)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        var found = (from c in list
                                     where c.ResolvedNodeId != null &&
                                     c.ResolvedNodeId.ToString() == node
                                     select c).ToList();
                        found.ForEach(item =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var preChanged = item.ToXml();
                            var shortname = mapNodes[node];
                            bool changeEndpoint = !endpointslist.Contains(item.EndpointUrl);
                            if(changeEndpoint)
                            {
                                item.EndpointUrl = item.EndpointUrl.Replace(item.AppName, aplicationName);
                                if (!endpointslist.Contains(item.EndpointUrl))
                                    item.EndpointUrl = defaultlocalendpoint;
                                item.AppName = aplicationName;
                            }
                            var newName = String.Format("{0} ({1})", shortname, item.AppName);
                            if (item.HumanReadable != newName || changeEndpoint)
                            {

                                item.HumanReadable = newName;
                                item.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.ReadablePath);
                                item.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.RelativePath);
                                var postChanged = item.ToXml();
                                if (!mapTagChanged.ContainsKey(preChanged))
                                    mapTagChanged.Add(preChanged, postChanged);
                                bDirty = true;
                            }
                        });
                    }
                    if (mapTagChanged.Count > 0)
                    {
                        map.Keys.ToList().ForEach(key =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            MapScreenEntities[key].ReplaceEntityReferences(mapTagChanged, key);
                        });
                    }
                }
                if (model.RenamedMap.Count > 0)
                {
                    var commandList = (from e in MapScreenEntities.Values
                                       where (e.CommandList as CommandManagerList).Count() > 0
                                       select (e.CommandList as CommandManagerList)).ToList();
                    var mapHashInner3DEntities = (from e in MapScreenEntities.Values
                                                  where (e.MapHashInner3DEntities != null && e.MapHashInner3DEntities.Count > 0)
                                                  select e).ToList();
                    commandList.AddRange(GetScreen3DCommandList(mapHashInner3DEntities));

                    foreach (var listCommand in commandList)
                    {
                        for (int i = 0; i < listCommand.Count(); i++)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var command = listCommand[i];
                            CommandManager.OpenScreenCommand scommand = command as CommandManager.OpenScreenCommand;
                            var path = scommand?.ScreenName.GetPathString();
                            if (scommand != null && !string.IsNullOrEmpty(path))
                            {
                                if (model.RenamedMap.ContainsKey(path))
                                {
                                    scommand.ScreenName = new Uri(model.RenamedMap[path], UriKind.RelativeOrAbsolute);
                                    bDirty = true;
                                }
                            }
                        }
                    }
                }

                if (bDirty)
                    try
                    {
                        SaveToFile();
                    }
                    catch (Exception ex)
                    {
                    }
            }
        }

        private List<CommandManagerList> GetScreen3DCommandList(List<ScreenEntity> mapHashInner3DEntities)
        {
            List<CommandManagerList> commandManagers = new List<CommandManagerList>();
            foreach (var entity in mapHashInner3DEntities)
            {
                foreach (var entry in entity.MapHashInner3DEntities.Values)
                {
                    if ((entry.CommandList as CommandManagerList).Count > 0)
                        commandManagers.Add(entry.CommandList as CommandManagerList);
                    if ((entry.MapHashInner3DEntities != null && entry.MapHashInner3DEntities.Count > 0))
                        commandManagers.AddRange(GetScreen3DCommandList(entry.MapHashInner3DEntities.Values.ToList()));
                }
            }
            return commandManagers;
        }
#endif
        public List<String> GetListElementsUsingProviderPath(String SourceSymbolProvider, String SourceSymbolPath)
        {
            return (from c in MapScreenEntities.Keys.AsParallel()
                     where
                         MapScreenEntities[c].SourceSymbolProvider == SourceSymbolProvider &&
                         MapScreenEntities[c].SourceSymbolPath == SourceSymbolPath
                     select c).ToList();
        }

#if !NET_STANDARD
#region OnRepositoryItemLoaded
        public event EventHandler RepositoryItemLoaded;
        /// <summary>
        /// Triggers the RepositoryItemLoaded event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRepositoryItemLoaded(FrameworkElement element, EventArgs ea)
        {
            if (RepositoryItemLoaded != null)
                RepositoryItemLoaded(element, ea);
        }
#endregion

#region OnRepositoryItemsLoading
        public event EventHandler RepositoryItemsLoading;
        /// <summary>
        /// Triggers the RepositoryItemLoaded event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRepositoryItemsLoading(object sender, EventArgs ea)
        {
            if (RepositoryItemsLoading != null)
                RepositoryItemsLoading(sender, ea);
        }
#endregion

#region OnRepositoryItemsLoaded
        public event EventHandler RepositoryItemsLoaded;
        /// <summary>
        /// Triggers the RepositoryItemLoaded event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnRepositoryItemsLoaded(object sender, EventArgs ea)
        {
            if (RepositoryItemsLoaded != null)
                RepositoryItemsLoaded(sender, ea);
        }
#endregion

#region OnParameterFileApplied
        public event EventHandler ParameterFileChanged;
        /// <summary>
        /// Triggers the RepositoryItemLoaded event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnParameterFileChanged(object sender, EventArgs ea)
        {
            if (ParameterFileChanged != null)
                ParameterFileChanged(sender, ea);
        }
#endregion
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearRepositoryItem(FrameworkElement canvas,
                                       String name, bool bCheckInners = true)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              select c).ToList();
            if (!bCheckInners)
                listInners.Clear();
            if (listInners.Count > 0)
            {
                listInners.ForEach(innelement =>
                {
                    if (MapScreenEntities.ContainsKey(innelement) &&
                        MapScreenEntities[innelement].SourceSymbolLinked)
                        ClearRepositoryItem(canvas, innelement, bCheckInners: false);
                });
            }

            var controlName = name;
            ContentControl contentControl = FindInnerControl(canvas, name) as ContentControl;
            if (contentControl != null && !(contentControl.Content is String) && 
                !(contentControl is Button))
            {
                contentControl.ClearValue(ContentControl.ContentProperty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DispatcherOperation LoadRepositoryItem(FrameworkElement fe, FrameworkElement canvas, 
                                       String name, String value = null, 
                                       String settings = null, String style = null, String key = null, 
                                       bool bSetSize = false, bool bAnimate = true, 
                                       bool bCheckInners = true, bool bThrowIfError = true, bool bRecreateResources = false, 
                                       bool bLoadRepositoryInners = true)
        {
            DispatcherOperation ret = null;
            if (bDisposed)
                return ret;

            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listInners = new List<String>();
            if (bCheckInners)
                listInners = (from c in MapScreenEntities.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              orderby System.Text.RegularExpressions.Regex.Matches(c, innerEntityNameFormat).Count
                              select c).ToList();

            if (!MapScreenEntities.ContainsKey(name))
                return ret;
            var entry = MapScreenEntities[name];
            if (!entry.SourceSymbolLinked && String.IsNullOrEmpty(value))
            {
                if (listInners.Count > 0)
                {
                    var controlName2 = name;
                    var contentControl2 = FindInnerControl(canvas, name);
                    if (contentControl2 != null)
                    {
                        FrameworkElement fecontent2 = contentControl2;
                        if (contentControl2 is ContentControl)
                            fecontent2 = ((contentControl2 as ContentControl).Content as FrameworkElement);
                        LoadInners(fe, canvas, bSetSize, bAnimate, bRecreateResources, listInners, controlName2, contentControl2,
                            fecontent2, bLoadRepositoryInners);
                    }
                }
                return ret;
            }
            entry.SourceSymbolLinkedResolved = false;

            if (String.IsNullOrEmpty(value))
            {
                try
                {
                    value = STRL.STRL.GetSymbolElement(entry.SourceSymbolProvider, entry.SourceSymbolPath, rootBase);
                }
                catch (Exception ex)
                {
                    return ret;
                }
            }
            if (String.IsNullOrEmpty(style))
            {
                try
                {
                    style = STRL.STRL.GetSymbolStyle(entry.SourceSymbolProvider, entry.SourceSymbolPath);
                }
                catch (Exception ex)
                {
                    return ret;
                }
            }
            if (String.IsNullOrEmpty(key))
            {
                try
                {
                    key = STRL.STRL.GetSymbolStyleKey(entry.SourceSymbolProvider, entry.SourceSymbolPath);
                }
                catch (Exception ex)
                {
                    return ret;
                }
            }
            if (String.IsNullOrEmpty(settings))
            {
                try
                {
                    settings = STRL.STRL.GetSymbolSettings(entry.SourceSymbolProvider, entry.SourceSymbolPath, rootBase);
                }
                catch (Exception ex)
                {
                    // return ret;
                }
            }

            //var element = value.ReadUIElement();
            //var cc = fe.FindName(name) as ContentControl;
            //cc.Content = element;

            var data = STRL.STRL.GetSymbolElementData(entry.SourceSymbolProvider, entry.SourceSymbolPath, rootBase);

            UIElement element = null;

            var controlName = name;
            ContentControl contentControl;
            var control = FindInnerControl(canvas, name);
            contentControl = control as ContentControl;
            if (contentControl != null)
            {
                if (entry.Entity == null)
                    entry.Entity = contentControl;

                List<String> postlistInners;
                MergeDocumentInnerXamlPropertiesBags(settings, name, out postlistInners);

                bool bAddResource = false;
                var resourceName = DependencyObjectExtensions.AdaptName(String.Format("_{0}", entry.ID));
                if (inExecution || IsOnBlindServer()/* || !bCheckInners*/) // in runtime we don't need to protect symbol (it is not saved), so we can speed it up (Resource.Remove is slow on fe)
                {
                    if (contentControl.Resources.Contains(resourceName))
                        element = contentControl.Resources[resourceName] as UIElement;
                }
                else
                {
                    resourceName = DependencyObjectExtensions.AdaptName(String.Format("{0}_{1}", name, entry.ID));
                    if (fe.Resources.Contains(resourceName))
                        element = fe.Resources[resourceName] as UIElement;
                }

                if (element == null || bRecreateResources)
                {
                    if (data == null)
                        element = value.ReadUIElement();
                    else
                        element = Utilities.WPF.XmlHelper.LoadBaml<UIElement>(data);
                    bAddResource = true;
                }

                if (!String.IsNullOrEmpty(style) && !String.IsNullOrEmpty(key))
                {
                    if (!fe.Resources.Contains(key))
                    {
                        data = STRL.STRL.GetSymbolStyleData(entry.SourceSymbolProvider, entry.SourceSymbolPath);
                        if (data == null)
                        {
                            var fileTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                            fileTemp = Path.ChangeExtension(fileTemp, "xaml");
                            File.WriteAllText(fileTemp, style);
                            try
                            {
                                var map = ResourceDictionaryExtensions.LoadFromFile(fileTemp, typeof(Style));

                                // fe.Resources.Remove(key);
                                fe.Resources.Add(key, map[key]);
                            }
                            finally
                            {
                                File.Delete(fileTemp);
                            }
                        }
                        else
                        {
                            var map = Utilities.WPF.XmlHelper.LoadBaml<ResourceDictionary>(data);
                            // fe.Resources.Remove(key);
                            fe.Resources.Add(key, map[key]);
                        }
                    }
                    try
                    {
                        (element as FrameworkElement).SetResourceReference(FrameworkElement.StyleProperty, key);
                    }
                    catch (Exception)
                    {
                    }
                }

                var felement = element as FrameworkElement;
                if (!String.IsNullOrEmpty(felement.Name))
                {
                    felement.Uid = felement.Name;
                    felement.ClearValue(FrameworkElement.NameProperty);
                }

                UnsubscribePropertyChangeXamlWriterProperties(contentControl);

                contentControl.SetResourceReference(ContentControl.ContentProperty, resourceName);
                //if (listInners.Count > 0)
                //{
                //    var frameElement = element as FrameworkElement;
                //    frameElement.GetChildrenOfType<ContentControl>().ToList().ForEach(control =>
                //        {
                //            control.SetResourceReference(ContentControl.ContentProperty, "");
                //        });
                //}

                if (bAddResource)
                {
                    if (inExecution || IsOnBlindServer()/* || !bCheckInners*/) // in runtime we don't need to protect symbol (it is not saved), so we can speed it up (Resource.Remove is slow on fe)
                    {
                        if (contentControl.Resources.Contains(resourceName))
                            contentControl.Resources.Remove(resourceName);
                        contentControl.Resources.Add(resourceName, element);
                    }
                    else
                    {
                        if (fe.Resources.Contains(resourceName))
                            fe.Resources.Remove(resourceName);
                        fe.Resources.Add(resourceName, element);
                    }
                }

                bAnimate = false;
                var oldOpacity = contentControl.Opacity;
                if (bAnimate && !IsOnBlindServer() && DelayLoadSymbols != 0 && contentControl.Visibility == Visibility.Visible)
                    contentControl.Opacity = 0;

                if (!Double.IsNaN(felement.Width) && !Double.IsNaN(felement.Height) && felement.Height != 0)
                    entry.LinkedAspectRatio = felement.Width / felement.Height;

                var fecontent = (contentControl.Content as FrameworkElement);
                try
                {
                    var bindingWidth = new Binding()
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                        Path = new PropertyPath("ActualWidth")
                    };
                    var bindingHeight = new Binding()
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                        Path = new PropertyPath("ActualHeight")
                    };

                    if (fecontent != null)
                    {
                        if (bSetSize || !entry.PreserveSize)
                        {
                            contentControl.Width = fecontent.Width;
                            contentControl.Height = fecontent.Height;
                        }

                        fecontent.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                        fecontent.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                    }

                    try
                    {
                        value = STRL.STRL.GetSymbolSettings(entry.SourceSymbolProvider, entry.SourceSymbolPath, rootBase);
                        if (!String.IsNullOrEmpty(value))
                        {
                            contentControl.GetChildrenOfType<Panel>().ToList()
                            .ForEach(panel =>
                            {
                                RestoreProblematicXamlWriter(panel, controlName, canvas, true);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    PostMergeDocumentInnerXamlPropertiesBags(canvas, settings, name);

                    // if (bCheck)
                    {
                        // force eventually to throw the exception here of what might be wrong
                        Action OnControlFirstLoaded = () =>
                        {
                            if (bDisposed)
                                return;

                            element.Measure(contentControl.RenderSize); //Important
                            element.Arrange(new Rect(contentControl.RenderSize)); //Important

                            var list = new List<UIElement>() { felement };
                            if (fecontent != null)
                            {
                                fecontent.ApplyTemplate();
                                fecontent.UpdateLayout();
                                list.AddRange(fecontent.GetVisualChildrenOfType<UIElement>().ToList());
                            }

                            var resolveEntityBrushAndPenAction = new Action(() =>
                            {
                                if (inExecution || IsOnBlindServer())
                                {
                                    if (!entry.BrushAndPenTagResolved)
                                    {
                                        ResolveEntityBrushAndPen(canvas, contentControl, controlName, listInners);
                                        entry.BrushAndPenTagResolved = true;
                                    }
                                }
                                else
                                    ResolveEntityBrushAndPen(canvas, contentControl, controlName, listInners);
                            });

                            if (contentControl.IsVisible)
                                resolveEntityBrushAndPenAction();
                            else
                            {
                                var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                                var notifierVisibility = new PropertyChangeNotifier(contentControl, propDesc.Name);
                                if (propertyChangeNotifierVisibilityList == null)
                                    propertyChangeNotifierVisibilityList = new List<PropertyChangeNotifier>();
                                propertyChangeNotifierVisibilityList.Add(notifierVisibility);
                                notifierVisibility.ValueChanged += (o, e) =>
                                {
                                    if (contentControl.IsVisible)
                                    {
                                        notifierVisibility.Dispose();
                                        propertyChangeNotifierVisibilityList.Remove(notifierVisibility);
                                        if (!entry.BrushAndPenTagResolved)
                                        {
                                            contentControl.ApplyTemplate();
                                            contentControl.UpdateLayout();
                                            resolveEntityBrushAndPenAction();
                                        }
                                    }
                                };
                            }

                            SetImagesBaseUri(list);

                            if (bCheckInners && postlistInners.Count > 0)
                            {
                                var ordered = (from c in postlistInners.AsParallel()
                                               orderby System.Text.RegularExpressions.Regex.Matches(c, innerEntityNameFormat).Count
                                               select c).ToList();
                                ordered.ForEach(innelement =>
                                {
                                    if (MapScreenEntities.ContainsKey(innelement) &&
                                        MapScreenEntities[innelement].SourceSymbolLinked)
                                    {
                                        LoadRepositoryItem(fe, canvas, innelement, bCheckInners: false,
                                            bAnimate: bAnimate, bSetSize: bSetSize, bThrowIfError: false,
                                            bRecreateResources: bRecreateResources, bLoadRepositoryInners: false);
                                    }
                                });
                            }

                            LoadInners(fe, canvas, bSetSize, bAnimate, bRecreateResources, listInners, controlName, contentControl, fecontent, bLoadRepositoryInners);
                            entry.SourceSymbolLinkedResolved = true;

                            bAnimate = false;
                            if (!bAnimate || IsOnBlindServer() || DelayLoadSymbols == 0 || contentControl.Visibility != Visibility.Visible)
                            {
                                if (inExecution || IsOnBlindServer()) // wait for the symbol been loaded
                                {
                                    if (!InTest && entry.HasCommands)
                                    {
                                        var listCommands = entry.CommandList as CommandManagerList;
                                        listCommands.ForEach(command =>
                                        {
                                            command.sExpression = entry.Expression;
                                            command.sReverseExpression = entry.ReverseExpression;
                                            command.mapDynamics = dataContextExpando;
                                            command.mapCurrentParameteItems = mapCurrentParameteItems;

                                            entry.ReplaceAlias(command);

                                            command.Init(entry, this, currentSessionName, entry.ExpressionEntity_ParserError, entry.ExpressionEntity_ExecutionError);
                                        });

                                        if (fecontent != null)
                                            Stylus.SetIsPressAndHoldEnabled(fecontent, false);
                                        if (fecontent is ICommandSource)
                                        {
                                            fecontent.GetType().GetProperty("CommandParameter").SetValue(fecontent, controlName, null);
                                            fecontent.GetType().GetProperty("Command").SetValue(fecontent, Command, null);
                                        }
                                    }

                                    entry.PrepareExecution(this, canvas, currentSessionName);
                                    entry.ExecuteScriptCode();

                                    if (listInners.Count > 0)
                                    {
                                        //ret = fe.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                        //{
                                        //    if (!bDisposed)
                                        //    {
                                        PrepareExecution(canvas, currentSessionName, listInners, true);
                                        OnRepositoryItemLoaded(contentControl, EventArgs.Empty);
                                        //    }
                                        //});
                                    }
                                    else
                                        OnRepositoryItemLoaded(contentControl, EventArgs.Empty);
                                }
                            }
                            else
                            {
                                contentControl.Fade(oldOpacity, DelayLoadSymbols, new SineEase() { EasingMode = EasingMode.EaseOut },
                                    (ob, ev) =>
                                    {
                                        contentControl.Fade(oldOpacity, -1, new SineEase() { EasingMode = EasingMode.EaseOut });
                                        contentControl.Opacity = oldOpacity;

                                        if (inExecution) // wait for the symbol been loaded
                                        {
                                            if (!InTest && entry.HasCommands)
                                            {
                                                var listCommands = entry.CommandList as CommandManagerList;
                                                listCommands.ForEach(command =>
                                                {
                                                    entry.Entity = contentControl;
                                                    command.sExpression = entry.Expression;
                                                    command.sReverseExpression = entry.ReverseExpression;
                                                    command.mapDynamics = dataContextExpando;
                                                    command.mapCurrentParameteItems = mapCurrentParameteItems;
                                                    command.Init(entry, this, currentSessionName, entry.ExpressionEntity_ParserError, entry.ExpressionEntity_ExecutionError);
                                                });

                                                if (fecontent != null)
                                                    Stylus.SetIsPressAndHoldEnabled(fecontent, false);
                                                if (fecontent is ICommandSource)
                                                {
                                                    fecontent.GetType().GetProperty("CommandParameter").SetValue(fecontent, controlName, null);
                                                    fecontent.GetType().GetProperty("Command").SetValue(fecontent, Command, null);
                                                }
                                            }

                                            entry.PrepareExecution(this, canvas, currentSessionName);
                                            entry.ExecuteScriptCode();

                                            if (listInners.Count > 0)
                                            {
                                                //ret = fe.Dispatcher.BeginInvokeAsynchronously(() =>
                                                //{
                                                //    if (!bDisposed)
                                                //    {
                                                PrepareExecution(canvas, currentSessionName, listInners, true);
                                                OnRepositoryItemLoaded(contentControl, EventArgs.Empty);
                                                //    }
                                                //});
                                            }
                                            else
                                                OnRepositoryItemLoaded(contentControl, EventArgs.Empty);
                                        }
                                    });
                            }
                        };

                        if (contentControl.IsLoaded)
                            ret = fe.Dispatcher.BeginInvokeAsynchronouslyInRender(OnControlFirstLoaded);
                        else
                        {
                            if (loadedContentControls == null)
                                loadedContentControls = new List<ContentControl>();
                            contentControl.Loaded += (s, e) =>
                            {
                                if (!loadedContentControls.Contains(contentControl))
                                {
                                    loadedContentControls.Add(contentControl);
                                    fe.Dispatcher.BeginInvokeAsynchronouslyInRender(OnControlFirstLoaded);
                                }
                            };
                        }
                        //contentControl.ApplyTemplate();
                        //contentControl.UpdateLayout();
                    }
                }
                catch (Exception ex)
                {
                    contentControl.Opacity = oldOpacity;
                    felement.Style = null;
                    if (bThrowIfError)
                        throw;
                }

            }
            else
            {
                entry.SourceSymbolLinkedResolved = true;
                if (control != null && !String.IsNullOrEmpty(entry.OpcuaEntityReference?.ParentTypeDefinitionName) && entry.OpcuaEntityReference?.ResolvedItem == false) // Merged/compiled screen symbols Viewbox
                {
                    foreach (var innerName in listInners)
                    {
                        var uie = FindInnerControl(canvas, innerName) as FrameworkElement;
                        if (uie == null)
                            continue;
                        ResolveRelativeItems(uie, innerName, canvas, false);
                    }
                }
            }

            //MergeDocument(settings, null);
            //LoadResources(fe);
            return ret;
        }

        private void LoadInners(FrameworkElement fe, FrameworkElement canvas, bool bSetSize, bool bAnimate, bool bRecreateResources, List<string> listInners, 
            string controlName, FrameworkElement contentControl, FrameworkElement fecontent, bool bLoadRepositoryInners)
        {
            if (bLoadRepositoryInners && listInners.Count > 0)
            {
                //fe.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                //    {
                //        if (!bDisposed)
                //        {
                listInners.ForEach(innelement =>
                {
                    if (MapScreenEntities.ContainsKey(innelement) &&
                        MapScreenEntities[innelement].SourceSymbolLinked)
                    {
                        try
                        {
                            LoadRepositoryItem(fe, canvas, innelement, bCheckInners: false,
                                bAnimate: bAnimate, bSetSize: bSetSize, bThrowIfError: false, bRecreateResources: bRecreateResources, bLoadRepositoryInners: false);
                        }
                        catch
                        { }
                    }
                });
                //    }
                //});
            }

            var listProblematicXaml = (from c in listInners.AsParallel()
                                        where !String.IsNullOrEmpty(MapScreenEntities[c].ProblematicXaml)
                                        select c).ToList();
            bool bProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fecontent, "");
            if (bProblematic ||
                listProblematicXaml.Count > 0)
            {
                if (bProblematic)
                    UpdateProblematicXamlWriterProperties(contentControl, controlName, true);

                listProblematicXaml.ForEach(c =>
                {
                    var el = FindInnerControl(canvas, c);
                    if (el != null)
                        UpdateProblematicXamlWriterProperties(el, c, true);
                });

                OnRepositoryItemLoaded(contentControl, EventArgs.Empty);
            }
        }

        void SetContentControl(FrameworkElement fe, FrameworkElement canvas, String name, String resource, 
            String tooltip, bool bTooltipError = false)
        {
            ContentControl contentControl = FindInnerControl(canvas, name) as ContentControl;
            if (contentControl == null)
                return;

            var element = fe.TryFindResource(resource) as FrameworkElement;
            if (element == null)
                return;

            // var resourceName = DependencyObjectExtensions.AdaptName(name);
            // fe.Resources.Remove(resourceName);
            // var cloned = element.Clone() as FrameworkElement;
            //contentControl.ToolTip = bTooltipError ? String.Format(Properties.Resources.ErrorLoadingSymbol,
            //    name, tooltip) : tooltip;
            // fe.Resources.Add(resourceName, element);
            contentControl.SetResourceReference(ContentControl.ContentProperty, resource);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CleanRepositoryItemBindings(FrameworkElement fe)
        {
            var list = (from entry in MapScreenEntities.AsParallel()
                        where entry.Value.SourceSymbolLinked
                        select entry).ToList();
            {
                using (var cursor = new WaitCursor())
                {
                    list.ForEach(entry =>
                    {
                        var uie = FindInnerControl(fe, entry.Key) as ContentControl;
                        if (uie != null)
                            uie.ClearValue(ContentControl.ContentProperty);
                    });
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceUnload()
        {
            STRL.STRL.ForceUnload();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateRepositoryItems(FrameworkElement fe, FrameworkElement canvas,
            bool bSync = false, bool bCleanCache = false, bool UpdateOnlyThoseWithStyle = false, bool bRecreateResources = false, string entityname = null)
        {
            if (bCleanCache)
                ForceUnload();

            var list = (from entry in MapScreenEntities.AsParallel()
                        where (entry.Value.SourceSymbolLinked || (entry.Value.SourceSymbolPath != null && !String.IsNullOrEmpty(entry.Value.OpcuaEntityReference?.ParentTypeDefinitionName) && entry.Value.OpcuaEntityReference?.ResolvedItem == false)) && (entityname == null || (entityname != null && entry.Key == entityname))
                        orderby System.Text.RegularExpressions.Regex.Matches(entry.Key, innerEntityNameFormat).Count
                        select entry.Key).ToList();
            /*
            var listToRemove = new List<String>();
            list.ForEach(entry =>
            {
                var checkinner = String.Format("{0}{1}", entry, innerEntityNameFormat);
                var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                                  where c.StartsWith(checkinner)
                                  select c).ToList();
                listToRemove.AddRange(listInners);
            });
            listToRemove.ForEach(name => list.Remove(name));
            */
            if (bSync || LoadFromRepositorySynchro)
            {
                using (var cursor = new WaitCursor())
                {
                    list.ForEach(entry =>
                    {
                        try
                        {
                            var value = STRL.STRL.GetSymbolElement(MapScreenEntities[entry].SourceSymbolProvider,
                                                                   MapScreenEntities[entry].SourceSymbolPath, rootBase);
                            var settings = STRL.STRL.GetSymbolSettings(MapScreenEntities[entry].SourceSymbolProvider,
                                                                   MapScreenEntities[entry].SourceSymbolPath, rootBase);
                            var style = STRL.STRL.GetSymbolStyle(MapScreenEntities[entry].SourceSymbolProvider,
                                                                   MapScreenEntities[entry].SourceSymbolPath);
                            var key = STRL.STRL.GetSymbolStyleKey(MapScreenEntities[entry].SourceSymbolProvider,
                                                                   MapScreenEntities[entry].SourceSymbolPath);

                            if (!UpdateOnlyThoseWithStyle || !String.IsNullOrEmpty(style))
                            {
                                //var action = new Action(() =>
                                //{
                                    // SetContentControl(fe, canvas, entry.Key, "WaitingRepositoryItem", entry.Key);
                                    try
                                    {
                                        LoadRepositoryItem(fe, canvas, entry, value, settings, style, key, bAnimate: false, bRecreateResources: bRecreateResources, bLoadRepositoryInners: entityname != null);
                                    }
                                    catch (Exception ex)
                                    {
                                        SetContentControl(fe, canvas, entry, "ErrorRepositoryItem", ex.Message, true);
                                    }
                                //});
                                //if (IsOnBlindServer())
                                //{
                                //    action();
                                //}
                                //else
                                //{
                                //    fe.Dispatcher.BeginInvokeAsynchronouslyInBackground(action);
                                //}
                            }
                        }
                        catch (Exception ex)
                        {
                            SetContentControl(fe, canvas, entry, "ErrorRepositoryItem", ex.Message, true);
                        }
                    });
                }
            }
            else if (list.Count > 0)
            {
                /*
                list.ForEach(entry =>
                    {
                        SetContentControl(fe, canvas, entry.Key, "WaitingRepositoryItem", entry.Key);
                    });
                */
                // var listTasks = new List<Task>();
                var bSavedNeed = NeedsSave;

                var t = Task.Factory.StartNew(() =>
                    {
#if DEBUG
                        var text = String.Format("Async Loading Repository Symbol for {0} took", FullPath) + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            OnRepositoryItemsLoading(this, EventArgs.Empty);
                            list.ForEach(entry =>
                            {
                                try
                                {
                                    var value = STRL.STRL.GetSymbolElement(MapScreenEntities[entry].SourceSymbolProvider,
                                                                           MapScreenEntities[entry].SourceSymbolPath, rootBase);
                                    var settings = STRL.STRL.GetSymbolSettings(MapScreenEntities[entry].SourceSymbolProvider,
                                                                           MapScreenEntities[entry].SourceSymbolPath, rootBase);
                                    var style = STRL.STRL.GetSymbolStyle(MapScreenEntities[entry].SourceSymbolProvider,
                                                                           MapScreenEntities[entry].SourceSymbolPath);
                                    var key = STRL.STRL.GetSymbolStyleKey(MapScreenEntities[entry].SourceSymbolProvider,
                                                                           MapScreenEntities[entry].SourceSymbolPath);

                                    // pre-fetch data
                                    var data = STRL.STRL.GetSymbolElementData(MapScreenEntities[entry].SourceSymbolProvider, MapScreenEntities[entry].SourceSymbolPath, rootBase);
                                    if (!String.IsNullOrEmpty(style) && !String.IsNullOrEmpty(key))
                                        data = STRL.STRL.GetSymbolStyleData(MapScreenEntities[entry].SourceSymbolProvider, MapScreenEntities[entry].SourceSymbolPath);

                                    fe.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                    {
                                        try
                                        {
                                            var dp = LoadRepositoryItem(fe, canvas, entry, value, settings, style, key, bAnimate: false, bRecreateResources: bRecreateResources, bLoadRepositoryInners: entityname != null);

                                            if (entry == list.Last())
                                            {
                                                if (dp == null || dp.Status == DispatcherOperationStatus.Completed)
                                                    OnRepositoryItemsLoaded(this, EventArgs.Empty);
                                                else
                                                {
                                                    if (dp.Status != DispatcherOperationStatus.Completed)
                                                    {
                                                        dp.Completed += (o, e) =>
                                                        {
                                                            OnRepositoryItemsLoaded(this, EventArgs.Empty);
                                                        };
                                                    }
                                                    else
                                                        OnRepositoryItemsLoaded(this, EventArgs.Empty);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SetContentControl(fe, canvas, entry, "ErrorRepositoryItem", ex.Message, true);
                                        }
                                    });
                                }
                                catch (Exception ex)
                                {
                                    var dp = fe.Dispatcher.BeginInvokeAsynchronously(() =>
                                    {
                                        SetContentControl(fe, canvas, entry, "ErrorRepositoryItem", ex.Message, true);
                                    });

                                    if (entry == list.Last())
                                    {
                                        if (dp.Status != DispatcherOperationStatus.Completed)
                                        {
                                            dp.Completed += (o, e) =>
                                            {
                                                OnRepositoryItemsLoaded(this, EventArgs.Empty);
                                            };
                                        }
                                        else
                                            OnRepositoryItemsLoaded(this, EventArgs.Empty);
                                    }
                                }
                            });
                        }
                        //lock (listTasks)
                        //{
                        //    listTasks.Add(t);
                        //}
                    });

                if (!inExecution && !inTest && !inRuntime && !isInLibrary)
                {
                    t.ContinueWith(ret =>
                    {
                        fe.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            NeedsSave = bSavedNeed;
                        });
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            //if (bSync)
            //    Task.WaitAll(listTasks.ToArray());
        }
        public void InitXamlDocument(string fullPath, string sourceText)
        {
            xamlDocument = new XamlDocument(Path.GetDirectoryName(fullPath));
            xamlDocument.InitializeSourceText(sourceText);
            xamlDocument.FullPath = fullPath;
        }
        public void SetCurrentXamlDocument(string sourceText)
        {
            xamlDocument.InitializeSourceText(sourceText);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SaveCurrentDocument(Canvas cv)
        {
            using (new WaitCursor())
            {
                if (!NeedsSave)
                    return true;

                if (String.IsNullOrEmpty(xamlDocument.FullPath))
                    return false;

                if (bRecalculateListScriptVariableUsed)
                    RecalculateListVariableUsed(cv);
                listScriptVariableUsed = null;

                //foreach(var data in MapScreenEntities)
                //{
                /*
                if (MapScreenEntities[data.Key].OpcuaEntityReference == null ||
                    !MapScreenEntities[data.Key].OpcuaEntityReference.IsValid)
                    continue;
                var checkinner = String.Format("{0}{1}", data.Key, innerEntityNameFormat);
                var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                                    where c.StartsWith(checkinner) &&
                                    (MapScreenEntities[c].HasCommands || MapScreenEntities[c].HasAnimations ||
                                    !String.IsNullOrEmpty(MapScreenEntities[c].Expression)) &&
                                    //(MapScreenEntities[c].OpcuaEntityReference == null ||
                                    //!MapScreenEntities[c].OpcuaEntityReference.IsValid)
                                    MapScreenEntities[c].OpcuaEntityReference != null &&
                                    MapScreenEntities[c].OpcuaEntityReference.IsRelative
                                  select c).ToList();
                if (listInners.Count == 0)
                    continue;
                var xml = MapScreenEntities[data.Key].OpcuaEntityReference.ToXml();
                listInners.ForEach(inner =>
                {
                    MapScreenEntities[inner].OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
                });
                */

                    //var tobeResolved = GetMapItemsToBeResolved(cv, data.Key, true);
                    //data.Value.MapItemsToBeResolved = tobeResolved;
                    //}

                ResolveEntityStyleBinding(cv);
                // ResolveProblematicXamlOnCanvas(cv);
                xamlDocument.SourceText = cv.XamlWriterFormatted();
                bool bRet = SaveToFile();
                if (bRet)
                    NeedsSave = false;
                return bRet;
            }
        }
     
        public void RecalculateListVariableUsed(Canvas cv)
        {
            bRecalculateListScriptVariableUsed = false;
            mapScriptVariableUsed = null;
            if (MapScreenEntities != null)
            {
                var listScripts = (from c in MapScreenEntities.AsParallel() where c.Value.ContainsCode() select c.Key).ToList();
                listScripts.ForEach(key =>
                {
                    if (MapScreenEntities[key].Entity == null)
                    {
                        UIElement uie = FindInnerControl(cv, key);
                        if (uie != null)
                        {
                            MapScreenEntities[key].Entity = uie;
                            MapScreenEntities[key].Document = this;
                        }
                    }
                    var map = GetListUsedVariables(key, MapScreenEntities[key], skipDataService: true);
                    if (map != null && map.Count > 0)
                    {
                        if (mapScriptVariableUsed == null)
                            mapScriptVariableUsed = new Dictionary<string, List<string>>();

                        if (!mapScriptVariableUsed.ContainsKey(key))
                            mapScriptVariableUsed.Add(key, map.Keys.ToList());
                    }
                });
            }

            var list = GetListUsedVariables(skipDataService: true);
            if (list != null && list.Count > 0)
            {
                if (mapScriptVariableUsed == null)
                    mapScriptVariableUsed = new Dictionary<string, List<string>>();

                if (!mapScriptVariableUsed.ContainsKey(innerEntityNameFormat))
                    mapScriptVariableUsed.Add(innerEntityNameFormat, list.Keys.ToList());
            }
        }
        public void SaveToPng(FrameworkElement c)
        {
            if (c.RenderSize.IsEmpty || String.IsNullOrEmpty(xamlDocument.FullPath))
                return;

            try
            {
                var file = Path.ChangeExtension(xamlDocument.FullPath, "png");
                if (fileSystemProviderBase != null)
                {
                    file = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                }

                var rtbdisk = new RenderTargetBitmap((int)c.RenderSize.Width,
                                                    (int)c.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
                c.Measure(c.RenderSize); //Important
                c.Arrange(new Rect(c.RenderSize)); //Important
                (c as FrameworkElement).ApplyTemplate();
                c.UpdateLayout();
                rtbdisk.Render(c);
                rtbdisk.Freeze();

                var encoderdisk = new PngBitmapEncoder();
                encoderdisk.Frames.Add(BitmapFrame.Create(rtbdisk));

                using (Stream fsx = File.Create(file))
                {
                    encoderdisk.Save(fsx);
                }

                if (fileSystemProviderBase != null)
                {
                    var data = File.ReadAllBytes(file);
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {

                    }
                    fileSystemProviderBase.UploadFile(null, Path.ChangeExtension(xamlDocument.FullPath, "png"), data);
                }

                rtbdisk.Clear();
            }
            catch (Exception ex)
            { }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SaveAllProblematicXamlOnCanvas(FrameworkElement cv)
        {
            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

            listProblematicXaml.ForEach(key =>
            {
                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    SaveProblematicXamlWriterProperties(uie, key);
                }
            });
        }
        /*
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveProblematicXamlOnCanvas(Panel cv, String name = null)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where (String.IsNullOrEmpty(name) || entry.Key.StartsWith(checkinner)) && !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

            var order = (from c in listProblematicXaml.AsParallel()
                         where c.StartsWith(checkinner)
                         select new
                         {
                             occurences = System.Text.RegularExpressions.Regex.Matches(c, checkinner).Count,
                             Xaml = c
                         }).ToList();
            var ordered = (from c in order.AsParallel() orderby c.occurences descending select c.Xaml).ToList();
            listProblematicXaml.RemoveAll(c => ordered.Contains(c));
            ordered.AddRange(listProblematicXaml);

            foreach(var key in ordered)
            {
                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    SaveProblematicXamlWriterProperties(uie, key);
                    UnsubscribePropertyChangeXamlWriterProperties(uie);

                    var names = key.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Length > 1)
                    {
                        var bFound = false;
                        for (int i = 0; i < names.Length - 1; ++i)
                        {
                            var checkname = names[0];
                            for (int j = 0; j < i; ++j)
                            {
                                checkname += innerEntityNameFormat;
                                checkname += names[j + 1];
                            }

                            if (MapScreenEntities.ContainsKey(checkname) &&
                                MapScreenEntities[checkname].SourceSymbolLinked)
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (bFound)
                            continue;
                    }

                    UIElement newuie = new ContentControl();
                    if (newuie != null)
                    {
                        int n = cv.Children.IndexOf(uie);
                        if (n < 0)
                        {
                            var parent = uie.FindFirstParent<Panel>();
                            if (parent != null && (uie is Canvas || uie is InkCanvas))
                                parent = parent.FindFirstParent<Panel>();
                            if (parent != null)
                            {
                                n = parent.Children.IndexOf(uie);
                                parent.Children.Insert(n, newuie);
                                parent.Children.Remove(uie);
                                InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                                InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                                Canvas.SetTop(newuie, Canvas.GetTop(uie));
                                Canvas.SetLeft(newuie, Canvas.GetLeft(uie));
                            }
                            else
                            {
                                var decorator = uie.FindFirstParent<Decorator>();
                                if (decorator != null)
                                {
                                    decorator.Child = newuie;
                                }
                                else
                                {
                                    var content = uie.FindFirstParent<ContentControl>();
                                    if (content != null)
                                        content.Content = newuie;
                                    else
                                    {
                                        content = uie.FindFirstAncestor<ContentControl>();
                                        if (content != null)
                                            content.Content = newuie;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cv.Children.Insert(n, newuie);
                            cv.Children.Remove(uie);
                        }

                        //just add the element, it wasn't another Canvas that we
                        //added to the dataobject as a container
                        if (newuie is FrameworkElement)
                        {
                            var fe = newuie as FrameworkElement;
                            fe.Width = uie.Width;
                            fe.Height = uie.Height;
                            if (!String.IsNullOrEmpty(uie.Name))
                            {
                                fe.Name = uie.Name;

                                Utilities.WPF.DependencyObjectExtensions.UnregisterName(cv, uie as FrameworkElement);
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, fe as FrameworkElement, bIsNested:false);
                            }
                            else if (uie.ReadLocalValue(FrameworkElement.UidProperty) != DependencyProperty.UnsetValue)
                                fe.Uid = uie.Uid;

                            fe.IsManipulationEnabled = uie.IsManipulationEnabled;
                        }

                        Canvas.SetLeft(newuie, Canvas.GetLeft(uie));
                        Canvas.SetTop(newuie, Canvas.GetTop(uie));
                    }
                }
            }
        }
        */
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CleanProblematicXamlBags()
        {
            //(from value in mapProblematicXamlWriterBag.Values.OfType<IDisposable>() don't uncomment here 
            // select value as IDisposable).ToList().ForEach(k => { k.Dispose(); });
            mapProblematicXamlWriterBag.Clear();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveProblematicXamlOnCanvas(Canvas cv, String name = null)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where (String.IsNullOrEmpty(name) || entry.Key.StartsWith(checkinner)) && !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

            var order = (from c in listProblematicXaml.AsParallel()
                         where c.StartsWith(checkinner)
                         select new
                         {
                             occurences = System.Text.RegularExpressions.Regex.Matches(c, checkinner).Count,
                             Xaml = c
                         }).ToList();
            var ordered = (from c in order.AsParallel() orderby c.occurences descending select c.Xaml).ToList();
            listProblematicXaml.RemoveAll(c => ordered.Contains(c));
            ordered.AddRange(listProblematicXaml);

            foreach(var key in ordered)
            {
                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    SaveProblematicXamlWriterProperties(uie, key);
                    UnsubscribePropertyChangeXamlWriterProperties(uie);

                    var names = key.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
                    if (names.Length > 1)
                    {
                        var bFound = false;
                        for (int i = 0; i < names.Length - 1; ++i)
                        {
                            var checkname = names[0];
                            for (int j = 0; j < i; ++j)
                            {
                                checkname += innerEntityNameFormat;
                                checkname += names[j + 1];
                            }

                            if (MapScreenEntities.ContainsKey(checkname) &&
                                MapScreenEntities[checkname].SourceSymbolLinked)
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (bFound)
                            continue;
                    }

                    // UnsubscribePropertyChangeXamlWriterProperties(uie);
                    if (!mapProblematicXamlWriterBag.ContainsKey(MapScreenEntities[key].ID.ToString()))
                        mapProblematicXamlWriterBag.Add(MapScreenEntities[key].ID.ToString(), uie);

                    UIElement newuie = new ContentControl();
                    if (newuie != null)
                    {
                        int n = cv.Children.IndexOf(uie);
                        if (n < 0)
                        {
                            var parent = uie.FindFirstParent<Panel>();
                            if (parent != null && (uie is Canvas || uie is InkCanvas))
                                parent = parent.FindFirstParent<Panel>();
                            if (parent != null)
                            {
                                n = parent.Children.IndexOf(uie);
                                parent.Children.Insert(n, newuie);
                                parent.Children.Remove(uie);
                                InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                                InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                                Canvas.SetTop(newuie, Canvas.GetTop(uie));
                                Canvas.SetLeft(newuie, Canvas.GetLeft(uie));

                                Grid.SetColumn(newuie, Grid.GetColumn(uie));
                                Grid.SetRow(newuie, Grid.GetRow(uie));
                                Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                                Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));
                            }
                            else
                            {
                                var decorator = uie.FindFirstParent<Decorator>();
                                if (decorator != null)
                                {
                                    decorator.Child = newuie;
                                }
                                else
                                {
                                    var content = uie.FindFirstParent<ContentControl>();
                                    if (content != null)
                                        content.Content = newuie;
                                    else
                                    {
                                        content = uie.FindFirstAncestor<ContentControl>();
                                        if (content != null)
                                            content.Content = newuie;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cv.Children.Insert(n, newuie);
                            cv.Children.Remove(uie);
                        }

                        //just add the element, it wasn't another Canvas that we
                        //added to the dataobject as a container
                        if (newuie is FrameworkElement)
                        {
                            var fe = newuie as FrameworkElement;
                            fe.Width = uie.Width;
                            fe.Height = uie.Height;
                            if (!String.IsNullOrEmpty(uie.Name))
                            {
                                fe.Name = uie.Name;

                                Utilities.WPF.DependencyObjectExtensions.UnregisterName(cv, uie as FrameworkElement);
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, fe as FrameworkElement, bIsNested:false);
                            }
                            else if (uie.ReadLocalValue(FrameworkElement.UidProperty) != DependencyProperty.UnsetValue)
                                fe.Uid = uie.Uid;

                            fe.IsManipulationEnabled = uie.IsManipulationEnabled;
                        }

                        InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                        InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                        Canvas.SetLeft(newuie, Canvas.GetLeft(uie));
                        Canvas.SetTop(newuie, Canvas.GetTop(uie));

                        Grid.SetColumn(newuie, Grid.GetColumn(uie));
                        Grid.SetRow(newuie, Grid.GetRow(uie));
                        Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                        Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));
                    }
                }
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetIsBlindServer()
        {
            bIsBlindServer = true;
        }

        public bool IsOnBlindServer()
        {
            return bIsBlindServer;
        }
#endif
#if !NET_STANDARD
        public Canvas GetCurrentXamlDocument()
        {
            return GetCurrentXamlDocument(true);
        }

        public Canvas GetCurrentXamlDocument(bool restoreProblematicXaml)
        {
            var cv = xamlDocument.ParseLoadedDocument();
            if (restoreProblematicXaml)
            {
#if !WINDOWS_UWP
                if (!String.IsNullOrEmpty(cv.Uid))
                {
                    Guid guid;
                    if (!Guid.TryParse(cv.Uid, out guid) || guid != Id)
                    {
                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, xamlDocument.FullPath));

                        return new Canvas();
                    }
                }
#endif
                RestoreProblematicXamlWriter(cv, bUpdate: false);
            }
            return cv;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RestoreProblematicXamlWriter(Panel cv, String name = null, FrameworkElement container = null, 
                                                    bool bUpdate = true)
        {
#if !WINDOWS_UWP
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
#endif
            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where (String.IsNullOrEmpty(name)
#if !WINDOWS_UWP
                                       || entry.Key.StartsWith(checkinner)
#endif
                                       ) && !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

#if !WINDOWS_UWP
            var order = (from c in listProblematicXaml.AsParallel() where c.StartsWith(checkinner) select new 
                {
                    occurences = System.Text.RegularExpressions.Regex.Matches(c, checkinner).Count,
                    Xaml = c
                }).ToList();
            var ordered = (from c in order.AsParallel() orderby c.occurences descending select c.Xaml).ToList();
            listProblematicXaml.RemoveAll(c => ordered.Contains(c));
            ordered.AddRange(listProblematicXaml);
            List<UIElement> newuieList = new List<UIElement>();
            foreach(var key in ordered)
#else
            foreach (var key in listProblematicXaml)
#endif
            {
#if !WINDOWS_UWP
                var names = key.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
                if (!bUpdate && names.Length > 1)
                {
                    var bFound = false;
                    for (int i = 0; i < names.Length - 1; ++i)
                    {
                        var checkname = names[0];
                        for (int j = 0; j < i; ++j)
                        {
                            checkname += innerEntityNameFormat;
                            checkname += names[j + 1];
                        }

                        if (MapScreenEntities.ContainsKey(checkname) &&
                            MapScreenEntities[checkname].SourceSymbolLinked)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        continue;
                }
#endif
                var uie = FindInnerControl(container != null ? container : cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    UIElement newuie = null;
                    try
                    {
                        if (mapProblematicXamlWriterBag.ContainsKey(MapScreenEntities[key].ID.ToString()))
                            newuie = mapProblematicXamlWriterBag[MapScreenEntities[key].ID.ToString()];
                        else
                            newuie = MapScreenEntities[key].ProblematicXaml.ReadUIElement();
                    }
                    catch (Exception ex)
                    {
#if !WINDOWS_UWP
                        if (IsOnBlindServer())
#endif
                            newuie = new TextBlock() { Text = ex.Message };
#if !WINDOWS_UWP
                        else
                            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    }
                    if (newuie != null && newuie != uie)
                    {
                        newuieList.Add(newuie);
                        int n = cv.Children.IndexOf(uie);
                        if (n < 0)
                        {
                            var parent = uie.FindFirstParent<Panel>();
                            if (parent != null && (uie is Canvas || uie is InkCanvas))
                                parent = parent.FindFirstParent<Panel>();
                            if (parent != null)
                            {
                                n = parent.Children.IndexOf(uie);
                                parent.Children.Remove(uie);
                                parent.Children.Insert(n, newuie);
#if !WINDOWS_UWP
                                InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                                InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
#endif
                                Canvas.SetTop(newuie, Canvas.GetTop(uie));
                                Canvas.SetLeft(newuie, Canvas.GetLeft(uie));

                                Grid.SetColumn(newuie, Grid.GetColumn(uie));
                                Grid.SetRow(newuie, Grid.GetRow(uie));
                                Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                                Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));
                            }
                            else
                            {
#if !WINDOWS_UWP
                                var decorator = uie.FindFirstParent<Decorator>();
                                if (decorator != null)
                                {
                                    decorator.Child = newuie;
                                }
                                else
#endif
                                {
                                    var content = uie.FindFirstParent<ContentControl>();
                                    if (content != null)
                                        content.Content = newuie;
#if !WINDOWS_UWP
                                    else
                                    {
                                        content = uie.FindFirstAncestor<ContentControl>();
                                        if (content != null)
                                            content.Content = newuie;
                                    }
#endif
                                }
                            }
                        }
                        else
                        {
                            cv.Children.Insert(n, newuie);
                            cv.Children.Remove(uie);
                        }

                        //just add the element, it wasn't another Canvas that we
                        //added to the dataobject as a container
                        if (newuie is FrameworkElement)
                        {
                            var fe = newuie as FrameworkElement;
                            fe.Width = uie.Width;
                            fe.Height = uie.Height;
                            if (!String.IsNullOrEmpty(uie.Name))
                            {
                                fe.Name = uie.Name;

#if !WINDOWS_UWP
                                Utilities.WPF.DependencyObjectExtensions.UnregisterName(cv, uie as FrameworkElement);
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, fe as FrameworkElement, bIsNested:false);
#endif
                            }
#if !WINDOWS_UWP
                            else if (uie.ReadLocalValue(FrameworkElement.UidProperty) != DependencyProperty.UnsetValue)
                                fe.Uid = uie.Uid;

                            fe.IsManipulationEnabled = uie.IsManipulationEnabled;
                            if (fe.RenderTransform != null && 
                                fe.ReadLocalValue(UIElement.RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
                                fe.RenderTransformOrigin = new Point(0.5, 0.5);
#endif
                        }

                        Utilities.WPF.XmlHelper.SetProblematicXamlWriter(newuie, MapScreenEntities[key].ProblematicXaml);
                        if (bUpdate)
                            UpdateProblematicXamlWriterProperties(newuie as FrameworkElement, key);

#if !WINDOWS_UWP
                        InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                        InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
#endif
                        Canvas.SetLeft(newuie, Canvas.GetLeft(uie));
                        Canvas.SetTop(newuie, Canvas.GetTop(uie));

                        Grid.SetColumn(newuie, Grid.GetColumn(uie));
                        Grid.SetRow(newuie, Grid.GetRow(uie));
                        Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                        Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));

                        uie.Dispose();
                    }
                }
            }
            SetImagesBaseUri(newuieList);
        }
#if !WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RestoreProblematicXamlWriter(InkCanvas cv, String name = null, bool bUpdate = true)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where (String.IsNullOrEmpty(name) || entry.Key.StartsWith(checkinner)) && !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

            var order = (from c in listProblematicXaml
                         where c.StartsWith(checkinner)
                         select new
                         {
                             occurences = System.Text.RegularExpressions.Regex.Matches(c, checkinner).Count,
                             Xaml = c
                         }).ToList();
            var ordered = (from c in order orderby c.occurences descending select c.Xaml).ToList();
            listProblematicXaml.RemoveAll(c => ordered.Contains(c));
            ordered.AddRange(listProblematicXaml);

            foreach(var key in ordered)
            {
                var names = key.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
                if (!bUpdate && names.Length > 1)
                {
                    var bFound = false;
                    for(int i = 0; i < names.Length - 1; ++i)
                    {
                        var checkname = names[0];
                        for (int j = 0; j < i; ++j)
                        {
                            checkname += innerEntityNameFormat;
                            checkname += names[j + 1];
                        }

                        if (MapScreenEntities.ContainsKey(checkname) &&
                            MapScreenEntities[checkname].SourceSymbolLinked)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound)
                        continue;
                }

                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    UIElement newuie = null;
                    try
                    {
                        if (mapProblematicXamlWriterBag.ContainsKey(MapScreenEntities[key].ID.ToString()))
                            newuie = mapProblematicXamlWriterBag[MapScreenEntities[key].ID.ToString()];
                        else
                            newuie = MapScreenEntities[key].ProblematicXaml.ReadUIElement();
                    }
                    catch (Exception ex)
                    {
                        if (IsOnBlindServer())
                            newuie = new TextBlock() { Text = ex.Message };
                        else
                            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (newuie != null && newuie != uie)
                    {
                        int n = cv.Children.IndexOf(uie);
                        if (n < 0)
                        {
                            var parent = uie.FindFirstParent<Panel>();
                            if (parent != null && (uie is Canvas || uie is InkCanvas))
                                parent = parent.FindFirstParent<Panel>();
                            if (parent != null)
                            {
                                n = parent.Children.IndexOf(uie);
                                parent.Children.Insert(n, newuie);
                                parent.Children.Remove(uie);
                                InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                                InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                                Canvas.SetTop(newuie, Canvas.GetTop(uie));
                                Canvas.SetLeft(newuie, Canvas.GetLeft(uie));

                                Grid.SetColumn(newuie, Grid.GetColumn(uie));
                                Grid.SetRow(newuie, Grid.GetRow(uie));
                                Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                                Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));
                            }
                            else
                            {
                                var decorator = uie.FindFirstParent<Decorator>();
                                if (decorator != null)
                                {
                                    decorator.Child = newuie;
                                }
                                else
                                {
                                    var content = uie.FindFirstParent<ContentControl>();
                                    if (content != null)
                                        content.Content = newuie;
                                    else
                                    {
                                        content = uie.FindFirstAncestor<ContentControl>();
                                        if (content != null)
                                            content.Content = newuie;
                                    }
                                }
                            }
                        }
                        else
                        {
                            cv.Children.Insert(n, newuie);
                            cv.Children.Remove(uie);
                        }

                        //just add the element, it wasn't another Canvas that we
                        //added to the dataobject as a container
                        if (newuie is FrameworkElement)
                        {
                            var fe = newuie as FrameworkElement;
                            // UpdateProblematicXamlWriterProperties(fe, key);

                            fe.Width = uie.Width;
                            fe.Height = uie.Height;
                            if (!String.IsNullOrEmpty(uie.Name))
                            {
                                fe.Name = uie.Name;

                                Utilities.WPF.DependencyObjectExtensions.UnregisterName(cv, uie as FrameworkElement);
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, fe as FrameworkElement, bIsNested:false);
                            }
                            else if (uie.ReadLocalValue(FrameworkElement.UidProperty) != DependencyProperty.UnsetValue)
                                fe.Uid = uie.Uid;

                            fe.IsManipulationEnabled = uie.IsManipulationEnabled;
                            if (fe.RenderTransform != null && 
                                fe.ReadLocalValue(UIElement.RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
                                fe.RenderTransformOrigin = new Point(0.5, 0.5);
                        }

                        Utilities.WPF.XmlHelper.SetProblematicXamlWriter(newuie, MapScreenEntities[key].ProblematicXaml);
                        if (bUpdate)
                            UpdateProblematicXamlWriterProperties(newuie as FrameworkElement, key);

                        // SubscribePropertyChangeXamlWriterProperties(newuie as FrameworkElement, key);

                        InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(uie));
                        InkCanvas.SetTop(newuie, InkCanvas.GetTop(uie));
                        Canvas.SetLeft(newuie, Canvas.GetLeft(uie));
                        Canvas.SetTop(newuie, Canvas.GetTop(uie));

                        Grid.SetColumn(newuie, Grid.GetColumn(uie));
                        Grid.SetRow(newuie, Grid.GetRow(uie));
                        Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(uie));
                        Grid.SetRowSpan(newuie, Grid.GetRowSpan(uie));

                        uie.Dispose();
                    }
                }
            }
        }
#endif
#endif
        internal string GetCurrentXamlSourceText()
        {
            return xamlDocument.SourceText;
        }
        public int GetCurrentXamlDocumentHash()
        {
            return xamlDocument.SourceText.GetHashCode();
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public void CreateScreenTemplate(string startingFolder)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(xamlDocument.Filename); 
            var ext = System.IO.Path.GetExtension(xamlDocument.Filename);
            int i = 0;
            string b = string.Empty;
            int val;
            for (int j = name.Length - 1; j >= 0; j--)
            {
                if (Char.IsDigit(name[j]))
                    b = b.Insert(0, name[j].ToString());
                else
                    break;
            }
            if (b.Length > 0)
            {
                i = int.Parse(b);
                name = name.Replace(b, "");
            }
            var newName = name + b;

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;

            if (string.IsNullOrEmpty(startingFolder))
                startingFolder = String.Format("{0}.{2}\\NewScreenTypes\\{1}\\", ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), Parent.Title,mainversion);

            if (!System.IO.Directory.Exists(startingFolder))
                System.IO.Directory.CreateDirectory(startingFolder);

            var ResourceFileName = String.Format("{0}{1}{2}", startingFolder, newName, ext);
            while (File.Exists(ResourceFileName))
            {
                newName = String.Format("{0}{1}", name, ++i);
                ResourceFileName = String.Format("{0}{1}{2}", startingFolder, newName, ext);
            }

            CopyFile(xamlDocument.FullPath, ResourceFileName, true, Parent, false);

            //if (File.Exists(ResourceFileName))
            //    File.Delete(ResourceFileName);
            //File.Copy(xamlDocument.FullPath, ResourceFileName);
            //File.Copy(xamlDocument.FullPath, ResourceFileName);
        }

        private bool WriteProjectDataStream(Stream ostrm)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(ScreenDocument));
                    serializer.WriteObject(writer, this);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        private bool WriteBinaryProjectDataStream(Stream ostrm)
        {
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(ScreenDocument));
                    serializer.WriteObject(writer, this);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        public bool SaveToFile(bool forceEncryption = false, bool bUploading = false, bool bThrowOnError = false)
        {
            try
            {
                var settingsFileName = GetSettingsFileName(xamlDocument.FullPath);
                var settingsBinaryFileName = GetSettingsBinaryFileName(xamlDocument.FullPath);
                
                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        fileSystemProviderBase.UploadFile(null, settingsFileName, memoryStream.ToArray());
                        fileSystemProviderBase.UploadFile(null, xamlDocument.FullPath, 
                            System.Text.Encoding.Unicode.GetBytes(xamlDocument.SourceText));
                        return true;
                    }
                }

           
                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

                if (!bUploading && (forceEncryption || Protected))
                {
                    id = Id;
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(settingsFileName, toWrite);

                        try
                        {
                            File.Delete(settingsBinaryFileName);
                        }
                        catch
                        {

                        }

                        xamlDocument.AddProtectionCode(id);
                        File.WriteAllText(FullPath, WPFUtilities.CryptString.CryptString.EncryptString(xamlDocument.SourceText));
                    }
                }
                else
                {
                    id = Guid.Empty;
                    using (var ostrm = File.Open(settingsFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (!WriteProjectDataStream(ostrm))
                            return false;
                    }

                    using (var ostrm = File.Open(settingsBinaryFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (!WriteBinaryProjectDataStream(ostrm))
                            return false;
                    }

                    if (!bUploading)
                        xamlDocument.RemoveProtectionCode(Id);
                    File.WriteAllText(FullPath, xamlDocument.SourceText);
                }
                
                if (Parent != null)
                    ScreenCompilerManager.ScreenCompilerManager.CompileScreen(FullPath, Parent.FilePath); 
            }
            catch (Exception ex)
            {
                if (bThrowOnError)
                    throw ex;

                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static readonly String innerEntityNameFormat = "-@-";
        public static readonly String[] innerEntityNameFormatDelimeters = new String[] { innerEntityNameFormat };
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String CleanInnerName(String name, String replaceText = " - ")
        {
            return name.Replace(innerEntityNameFormat, replaceText);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String FormatContainerNames(List<String> list)
        {
            var ret = String.Empty;
            list.ForEach(name =>
                {
                    if (String.IsNullOrEmpty(ret))
                        ret = name;
                    else
                        ret = String.Format("{0}{1}{2}", ret, innerEntityNameFormat, name);
                });
            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInnerEntity(String name)
        {
            return name.Contains(innerEntityNameFormat);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<String> GetInnerEntityList(String name)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              select c).ToList();
            return listInners;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Dictionary<String, ScreenEntity> CleanMapInnners(Dictionary<String, ScreenEntity> map, String name)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listInners = (from c in map.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              select c).ToList();
            var ret = new Dictionary<String, ScreenEntity>();
            listInners.ForEach(key =>
                {
                    ret.Add(key, map[key]);
                    map.Remove(key);
                });
            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddInnersEntity(String source, String dest, Dictionary<String, ScreenEntity> map, 
            FrameworkElement canvas = null)
        {
            var checkinner = String.Format("{0}{1}", source, innerEntityNameFormat);
            var destinner = String.Format("{0}{1}", dest, innerEntityNameFormat);
            var listInners = (from c in map.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              select c).ToList();

            listInners.ForEach(inner =>
                {
                    var regex = new Regex(Regex.Escape(checkinner));
                    var newinner = regex.Replace(inner, destinner, 1);

                    AddDynamicEntity(newinner);
                    MapScreenEntities[newinner].CopyAll(map[inner]);

                    if (canvas != null && !String.IsNullOrEmpty(MapScreenEntities[newinner].ProblematicXaml))
                    {
                        var el = FindInnerControl(canvas, inner);
                        if (el != null)
                            Utilities.WPF.XmlHelper.SetProblematicXamlWriter(el, MapScreenEntities[newinner].ProblematicXaml);
                    }
                });
        }

        public List<String> GetListInners(String name)
        {
            var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                              where c.StartsWith(checkinner)
                              select c).ToList();
            return listInners;
        }
#endif

#if !NET_STANDARD
        public FrameworkElement FindParentInnerControl(FrameworkElement canvas, String name)
        {
            var parentName = name;
#if !WINDOWS_UWP
            var names = name.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
            if (names != null && names.Length > 1)
                parentName = names[0];
#endif
            var contentControl = canvas.FindName(parentName) as FrameworkElement;
#if !WINDOWS_UWP
            if (contentControl == null && !String.IsNullOrEmpty(parentName))
            {
                var found = (from c in canvas.GetChildrenOfType<FrameworkElement>()
                             where c.Uid as String == parentName || c.Name == parentName
                             select c).ToList();
                if (found.Count > 0)
                    contentControl = found[0];
            }
#endif
            return contentControl;
        }
        
        public FrameworkElement FindInnerControl(FrameworkElement canvas, String name)
        {
            DependencyObjectExtensions.CleanChildrenOfTypeCache();

            FrameworkElement contentControl = null;
#if !WINDOWS_UWP
            var names = name.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
            if (names != null && names.Length > 1)
            {
                int i = 0;
                var containerControl = canvas;
                while (i < names.Length - 1)
                {
                    name = names[i + 1];
                    if (containerControl.Name == names[i] || containerControl.Uid == names[i])
                        containerControl = containerControl;
                    else
                        containerControl = containerControl.FindName(names[i]) as FrameworkElement;
                    if (containerControl != null)
                    {
                        //var foundcontrol = containerControl.FindName(names[i + 1]) as FrameworkElement;
                        //contentControl = foundcontrol as ContentControl;
                        //if (contentControl == null && containerControl is ContentControl)
                        //{
                        //    contentControl = foundcontrol;
                        //    foundcontrol = (containerControl as ContentControl).Content as FrameworkElement;
                        //    if (foundcontrol != null)
                        //    {
                        //        if (foundcontrol.Name == names[i + 1] || foundcontrol.Uid == names[i + 1])
                        //        {
                        //            containerControl = foundcontrol;
                        //        }
                        //        else
                        //        {
                        //            var found = (from c in foundcontrol.GetChildrenOfType<FrameworkElement>()
                        //                         where c.Uid as String == names[i + 1] || c.Name == names[i + 1]
                        //                         select c).ToList();
                        //            if (found.Count > 0)
                        //                foundcontrol = found[0];
                        //        }
                        //    }
                        //}
                        //else
                        {
                            FrameworkElement foundcontrol = null;
                            // if (foundcontrol == null)
                            {
                                var list = containerControl.GetChildrenOfTypeBreadthFirst<FrameworkElement>();
                                var found = (from c in list
                                             where c.Uid as String == names[i + 1] || c.Name == names[i + 1]
                                             select c).ToList();
                                if (found.Count > 0)
                                    foundcontrol = found[found.Count - 1];
                                else
                                {
                                    foreach(var first in list)
                                    {
                                        var listinner = first.GetChildrenOfTypeBreadthFirst<FrameworkElement>();
                                        found = (from c in listinner
                                                     where c.Uid as String == names[i + 1] || c.Name == names[i + 1]
                                                     select c).ToList();
                                        if (found.Count > 0)
                                        {
                                            foundcontrol = found[found.Count - 1];
                                            break;
                                        }
                                        else
                                        {
                                            listinner = first.GetChildrenOfType<FrameworkElement>();
                                            found = (from c in listinner
                                                     where c.Uid as String == names[i + 1] || c.Name == names[i + 1]
                                                     select c).ToList();
                                            if (found.Count > 0)
                                            {
                                                foundcontrol = found[found.Count - 1];
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            containerControl = contentControl = foundcontrol;
                            if (containerControl == null)
                                break;
                        }
                    }
                    else
                        break;
                    ++i;
                }
            }
            else
#endif
            {
                contentControl = canvas.FindName(name) as FrameworkElement;
#if !WINDOWS_UWP
                if (contentControl == null && !String.IsNullOrEmpty(name))
                {
                    var found = (from c in canvas.GetChildrenOfTypeBreadthFirst<FrameworkElement>()
                                 where c.Uid as String == name || c.Name == name
                                 select c).ToList();
                    if (found.Count > 0)
                        contentControl = found[found.Count - 1];
                }
#else
                if (contentControl == null)
                {
                    var found = (from c in canvas.GetVisualChildrenOfType<FrameworkElement>()
                                 where c.Name == name
                                 select c).ToList();
                    if (found.Count > 0)
                        contentControl = found[found.Count - 1];
                }
#endif
            }

            return contentControl;
        }
#endif
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RenameControl(FrameworkElement cv, String name, String rename)
        {
            var checkInner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var toCheck = (from c in MapScreenEntities.Keys.AsParallel()
                           where c.StartsWith(checkInner)
                           select c).ToList();
            toCheck.ForEach(entry =>
                {
                    if (MapScreenEntities.ContainsKey(entry))
                    {
                        var entity = MapScreenEntities[entry];

                        var el = FindInnerControl(cv, entry);
                        //bool bResubscribe = false;
                        if (el != null && elementSubscribed != null)
                        {
                            if (elementSubscribed.Contains(el))
                            {
                                UnsubscribePropertyChangeXamlWriterProperties(el);
                                //bResubscribe = true;
                            }
                            else if (el is ContentControl && !(el is UserControl) && (el as ContentControl).Content is FrameworkElement)
                            {
                                el = (el as ContentControl).Content as FrameworkElement;
                                if (elementSubscribed.Contains(el))
                                {
                                    UnsubscribePropertyChangeXamlWriterProperties(el);
                                    //bResubscribe = true;
                                }
                            }
                        }

                        MapScreenEntities.Remove(entry);
                        var regex = new Regex(Regex.Escape(checkInner));
                        var newname = String.Format("{0}{1}{2}", rename, innerEntityNameFormat, regex.Replace(entry, "", 1));
                        MapScreenEntities.Add(newname, entity);

                        //if (bResubscribe)
                        //    SubscribePropertyChangeXamlWriterProperties(el, newname);
                    }
                });

            if (MapScreenEntities.ContainsKey(name))
            {
                var entity = MapScreenEntities[name];

                var el = FindInnerControl(cv, name);
                bool bResubscribe = false;
                if (el != null && elementSubscribed != null)
                {
                    if (elementSubscribed.Contains(el))
                    {
                        UnsubscribePropertyChangeXamlWriterProperties(el);
                        bResubscribe = true;
                    }
                    else if (el is ContentControl && !(el is UserControl) && (el as ContentControl).Content is FrameworkElement)
                    {
                        el = (el as ContentControl).Content as FrameworkElement;
                        if (elementSubscribed.Contains(el))
                        {
                            UnsubscribePropertyChangeXamlWriterProperties(el);
                            bResubscribe = true;
                        }
                    }
                }

                MapScreenEntities.Remove(name);
                MapScreenEntities.Add(rename, entity);

                if (bResubscribe)
                    SubscribePropertyChangeXamlWriterProperties(el, rename);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TransformToInners(FrameworkElement cv, String container, List<String> childs)
        {
            childs.ForEach(child =>
                {
                    var checkInner = String.Format("{0}{1}", child, innerEntityNameFormat);
                    var toCheck = (from c in MapScreenEntities.Keys.AsParallel()
                                   where c.StartsWith(checkInner)
                                   select c).ToList();
                    TransformToInners(cv, container, toCheck);

                    if (MapScreenEntities.ContainsKey(child))
                    {
                        var entry = MapScreenEntities[child];

                        var el = FindInnerControl(cv, child);
                        bool bResubscribe = false;
                        if (el != null && elementSubscribed != null)
                        {
                            if (elementSubscribed.Contains(el))
                            {
                                UnsubscribePropertyChangeXamlWriterProperties(el);
                                bResubscribe = true;
                            }
                            else if (el is ContentControl && !(el is UserControl) && (el as ContentControl).Content is FrameworkElement)
                            {
                                el = (el as ContentControl).Content as FrameworkElement;
                                if (elementSubscribed.Contains(el))
                                {
                                    UnsubscribePropertyChangeXamlWriterProperties(el);
                                    bResubscribe = true;
                                }
                            }
                        }

                        MapScreenEntities.Remove(child);
                        var newname = String.Format("{0}{1}{2}", container, innerEntityNameFormat, child);
                        if (MapScreenEntities.ContainsKey(newname))
                            MapScreenEntities.Remove(newname);
                        MapScreenEntities.Add(newname, entry);
                        //if (bResubscribe)
                        //    SubscribePropertyChangeXamlWriterProperties(el, newname);
                    }
                });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TransformFromInners(FrameworkElement cv, String container, IDictionary<String, String> renamed)
        {
            var checkRemove = String.Format("{0}{1}", container, innerEntityNameFormat);
            var toRemove = (from c in MapScreenEntities.Keys.AsParallel()
                            where c.StartsWith(checkRemove)
                            select c).ToList();

            toRemove.ForEach(child =>
            {
                if (MapScreenEntities.ContainsKey(child))
                {
                    var entry = MapScreenEntities[child];

                    var el = FindInnerControl(cv, child);
                    bool bResubscribe = false;
                    if (el != null && elementSubscribed != null)
                    {
                        if (elementSubscribed.Contains(el))
                        {
                            UnsubscribePropertyChangeXamlWriterProperties(el);
                            bResubscribe = true;
                        }
                        else if (el is ContentControl && !(el is UserControl) && (el as ContentControl).Content is FrameworkElement)
                        {
                            el = (el as ContentControl).Content as FrameworkElement;
                            if (elementSubscribed.Contains(el))
                            {
                                UnsubscribePropertyChangeXamlWriterProperties(el);
                                bResubscribe = true;
                            }
                        }
                    }

                    MapScreenEntities.Remove(child);

                    var regex = new Regex(Regex.Escape(checkRemove));
                    var newname = regex.Replace(child, "", 1);
                    // var newname = child.Replace(checkRemove, "");
                    if (renamed.ContainsKey(newname))
                        newname = renamed[newname];
                    else
                    {
                        var names = newname.Split(innerEntityNameFormatDelimeters, StringSplitOptions.RemoveEmptyEntries);
                        if (names.Length > 1)
                        {
                            if (renamed.ContainsKey(names[0]))
                            {
                                var innernewname = renamed[names[0]];
                                newname = innernewname;
                                for(int i = 1; i < names.Length; ++i)
                                {
                                    newname += innerEntityNameFormat;
                                    newname += names[i];
                                }
                            }
                        }
                    }

                    if (MapScreenEntities.ContainsKey(newname))
                        MapScreenEntities.Remove(newname);
                    MapScreenEntities.Add(newname, entry);

                    if (el == null)
                    {
                        el = FindInnerControl(cv, newname);
                        if (el != null && elementSubscribed != null)
                        {
                            if (elementSubscribed.Contains(el))
                            {
                                UnsubscribePropertyChangeXamlWriterProperties(el);
                                bResubscribe = true;
                            }
                            else if (el is ContentControl && !(el is UserControl) && (el as ContentControl).Content is FrameworkElement)
                            {
                                el = (el as ContentControl).Content as FrameworkElement;
                                if (elementSubscribed.Contains(el))
                                {
                                    UnsubscribePropertyChangeXamlWriterProperties(el);
                                    bResubscribe = true;
                                }
                            }
                        }
                    }

                    //if (bResubscribe)
                    //    SubscribePropertyChangeXamlWriterProperties(el, newname);
                }
            });
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<String> MergeDocument(String settings, IDictionary<String, String> map, 
            String container = null, String Code = null, IDictionary<String, String> renamed = null)
        {
            var ret = new List<String>();
            if (String.IsNullOrEmpty(settings))
                return ret;

            var doc = settings.FromXml<ScreenDocument>();
            if (!String.IsNullOrEmpty(doc.variableSettings) ||
                !String.IsNullOrEmpty(doc.localVariableSettings))
            {
                var ClientEditor = GetService(typeof(IClientEditorManager)) as IClientEditorManager;
                var UFUAEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (UFUAEditor != null && ClientEditor != null)
                {
                    if (!String.IsNullOrEmpty(Code))
                    {
                        ScreenEntity entity = null;
                        if (!String.IsNullOrEmpty(container) && doc.MapScreenEntities.ContainsKey(container))
                            entity = doc.MapScreenEntities[container];
                        else if (doc.MapScreenEntities.Count > 0)
                            entity = doc.MapScreenEntities.First().Value;
                        currentDroppingCode = new DroppingCode(Code, Title, null, entity);

                        UFUAEditor.CreatingVariable += UFUAEditor_CreatingVariable;
                        UFUAEditor.VariableCreated += UFUAEditor_VariableCreated;
                        ClientEditor.CreatingVariable += UFUAEditor_CreatingVariable;
                        ClientEditor.VariableCreated += UFUAEditor_VariableCreated;
                    }

                    try
                    {
                        var renamedlocal = UFUAEditor.CheckAndUpdateVariableListSettingsFlat(this, doc.variableSettings);
                        var tempRenamed = ClientEditor.CheckAndUpdateVariableListSettingsFlat(this, doc.localVariableSettings);
                        if (renamedlocal != null)
                            tempRenamed?.Keys.ToList().ForEach(k => renamedlocal.Add(k, tempRenamed[k]));
                        else
                            renamed = tempRenamed;

                        if (renamed != null)
                        {
                            foreach (var pair in renamedlocal)
                                renamed.Add(pair.Key, pair.Value);
                        }
                    }
                    finally
                    {
                        if (!String.IsNullOrEmpty(Code))
                        {
                            UFUAEditor.CreatingVariable -= UFUAEditor_CreatingVariable;
                            UFUAEditor.VariableCreated -= UFUAEditor_VariableCreated;
                            ClientEditor.CreatingVariable -= UFUAEditor_CreatingVariable;
                            ClientEditor.VariableCreated -= UFUAEditor_VariableCreated;
                        }

                        if (currentDroppingCode != null)
                        {
                            currentDroppingCode.Dispose();
                            currentDroppingCode = null;
                        }
                    }
                }
            }

            var listEntities = doc.MapScreenEntities.Keys.ToList();
            while (listEntities.Count > 0)
            {
                var elName = listEntities[0];
                listEntities.Remove(elName);

                doc.MapScreenEntities[elName].RenewUniqueId();

                bool bNewName = false;
                String newName = null;
                if (!map.TryGetValue(elName, out newName))
                    newName = elName;
                else
                    bNewName = true;
                if (!String.IsNullOrEmpty(container))
                    newName = String.Format("{0}{1}{2}", container, innerEntityNameFormat, newName);
                else if (bNewName)
                {
                    if (elName.Contains(innerEntityNameFormat))
                        continue;

                    var checkInner = String.Format("{0}{1}", elName, innerEntityNameFormat);
                    var toCheck = (from c in doc.MapScreenEntities.Keys.AsParallel()
                                    where c.StartsWith(checkInner)
                                    select c).ToList();
                    toCheck.ForEach(inner =>
                        {
                            var regex = new Regex(Regex.Escape(checkInner));
                            var newinnername = String.Format("{0}{1}{2}", newName, innerEntityNameFormat, regex.Replace(inner, "", 1));
                            if (MapScreenEntities.ContainsKey(newinnername))
                                MapScreenEntities.Remove(newinnername);

                            doc.MapScreenEntities[inner].RenewUniqueId();
                            listEntities.Remove(inner);
                            MapScreenEntities.Add(newinnername, doc.MapScreenEntities[inner]);
                            ret.Add(newinnername);
                        });
                }

                if (MapScreenEntities.ContainsKey(newName))
                    MapScreenEntities.Remove(newName);
                MapScreenEntities.Add(newName, doc.MapScreenEntities[elName]);
                ret.Add(newName);
            }
            foreach (var res in doc.ListResources)
            {
                if (!ListResources.Contains(res))
                    ListResources.Add(res);
            }
            foreach (var res in doc.ListAssemblies)
            {
                if (!ListAssemblies.Contains(res) && !IsLocalAssembly(res))
                    ListAssemblies.Add(res);
            }
            assems = null;
            UpdateResources();

            return ret;
        }

        DroppingCode currentDroppingCode;
        private void UFUAEditor_VariableCreated(object sender, VariableEventArgs e)
        {
            var cancelEventArg = new CancelEventArgs();
            currentDroppingCode.ExecuteScriptCode(cancelEventArg, e);
        }

        private void UFUAEditor_CreatingVariable(object sender, VariableEventArgs e)
        {
            var cancelEventArg = new CancelEventArgs();
            currentDroppingCode.ExecuteScriptCode(cancelEventArg, e, true);
        }

        public void PostMergeDocumentInnerXamlPropertiesBags(FrameworkElement canvas, String settings, String container)
        {
            if (String.IsNullOrEmpty(settings))
                return;
            bool bSavedNeed = NeedsSave;

            var doc = settings.FromXml<ScreenDocument>();
            var listEntities = doc.MapScreenEntities.Keys.ToList();
            while (listEntities.Count > 0)
            {
                var elName = listEntities[0];
                listEntities.Remove(elName);
                var checkInner = String.Format("{0}{1}", elName, innerEntityNameFormat);
                var regex = new Regex(Regex.Escape(checkInner));
                var destname = String.Format("{0}{1}{2}", container, innerEntityNameFormat, regex.Replace(elName, "", 1));
                var parentname = Regex.Split(destname, innerEntityNameFormat)[0];
                if (MapScreenEntities.ContainsKey(parentname))
                {
                    if (!elName.Contains(innerEntityNameFormat) && !MapScreenEntities[parentname].SourceSymbolLinkedActive)
                        continue;
                }
                if (MapScreenEntities.ContainsKey(destname))
                {
                    var source = doc.MapScreenEntities[elName];
                    var dest = MapScreenEntities[destname];

                    var control = FindInnerControl(canvas, destname);
                    UIElement uie = control as UIElement;
                    if (uie != null)
                    {
                        dest.Entity = uie;
                        dest.Document = this;
                    }

                    if (control != null)
                    {
                        if (dest.PreserveSize)
                        {
                            if (!Double.IsNaN(dest.PreservedWidth) && dest.PreservedWidth != 0)
                                control.Width = dest.PreservedWidth;
                            if (!Double.IsNaN(dest.PreservedHeight) && dest.PreservedHeight != 0)
                                control.Height = dest.PreservedHeight;
                        }
                        if (dest.PreserveColors)
                        {
                            if (!String.IsNullOrEmpty(dest.PreservedBrush))
                            {
                                try
                                {
                                    using (var reader = new StringReader(dest.PreservedBrush))
                                    {
                                        // object obj = s.Deserialize(reader);
                                        using (var textReader = new XmlTextReader(reader))
                                        {
                                            var brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;

                                            if (control is Panel)
                                                (control as Panel).Background = brush;
                                            else if (control is Microsoft.Expression.Media.IShape)
                                                (control as Microsoft.Expression.Media.IShape).Fill = brush;
                                            else if (control is Control)
                                                (control as Control).Background = brush;
                                            else if (control is Border)
                                                (control as Border).Background = brush;
                                            else if (control is System.Windows.Shapes.Shape)
                                                (control as System.Windows.Shapes.Shape).Fill = brush;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                            if (!String.IsNullOrEmpty(dest.PreservedPen))
                            {
                                try
                                {
                                    using (var reader = new StringReader(dest.PreservedPen))
                                    {
                                        // object obj = s.Deserialize(reader);
                                        using (var textReader = new XmlTextReader(reader))
                                        {
                                            var brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;

                                            if (control is Microsoft.Expression.Media.IShape)
                                                (control as Microsoft.Expression.Media.IShape).Stroke = brush;
                                            else if (control is Control)
                                            {
                                                (control as Control).BorderBrush = brush;
                                                (control as Control).Foreground = brush;
                                            }
                                            else if (control is System.Windows.Shapes.Shape)
                                                (control as System.Windows.Shapes.Shape).Stroke = brush;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                        if (dest.PreserveText && !String.IsNullOrEmpty(dest.PreservedText))
                        {
                            Type t = control.GetType();
                            var p = t.GetProperty("Text");
                            if (p != null && !(control is ComboBox) && !(control is ListBox))
                                p.SetValue(control, dest.PreservedText);
                            else
                            {
                                var contentControl = control as ContentControl;
                                if (contentControl != null)
                                {
                                    if (contentControl.Content is String)
                                        contentControl.Content = dest.PreservedText;
                                    else if (!(control is HeaderedContentControl) && !(contentControl.Content is String))
                                    {
                                        UIElement ue = (UIElement)control;

                                        var currentList = (from c in ue.GetVisualChildrenOfType<ContentControl>()
                                                           where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                                           select c).FirstOrDefault();
                                        if (currentList != null)
                                        {
                                            currentList.ClearValue(ContentControl.ContentProperty);
                                            currentList.Content = dest.PreservedText;
                                        }
                                    }
                                    else if (control is HeaderedContentControl &&
                                            (control as HeaderedContentControl).Header is String ||
                                            (control as HeaderedContentControl).Header == null)
                                    {
                                        (control as HeaderedContentControl).Header = dest.PreservedText;
                                    }
                                }
                            }
                        }
                    }

                    if (!dest.PreserveFontSettingList)
                        dest.CopyFontSettingList(source);
                    if (!dest.PreserveStyle)
                        dest.CopyStyle(source);
                    if (!dest.PreserveCode)
                        dest.CopyCode(source);
                    if (!dest.PreserveCommands)
                        dest.CopyCommands(source);
                    if (!dest.PreserveAnimations)
                        dest.CopyAnimations(source);
                    if (!dest.PreserveMenu)
                        dest.CopyMenu(source);
                    if (!dest.PreserveExpression)
                        dest.CopyExpressions(source);
                    if (!dest.PreserveSecurity)
                        dest.CopySecurity(source);
                    if (!dest.PreserveVisibility)
                        dest.CopyVisibility(source);
                    if (!dest.PreserveVariables)
                    {
                        dest.CopyVariables(source);
                        dest.CopyAlias(source);
                    }

                    if (!dest.PreserveCustomControlProperties)
                        dest.CopyCustomProperties(source);

                    if (inExecution)
                    {
                        if (dest.ListAnimations != null)
                            dest.ListAnimations.ForEach((animation) => animation.PreInit(dest));
                    }
                }
            }

            NeedsSave = bSavedNeed;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void MergeDocumentInnerXamlPropertiesBags(String settings, String container, out List<String> listNewInners)
        {
            listNewInners = new List<String>();
            if (String.IsNullOrEmpty(settings))
                return;

            bool bSavedNeed = NeedsSave;

            var doc = settings.FromXml<ScreenDocument>();
            var listEntities = doc.MapScreenEntities.Keys.ToList();
            while (listEntities.Count > 0)
            {
                var elName = listEntities[0];
                listEntities.Remove(elName);
                if (!elName.Contains(innerEntityNameFormat))
                    continue;

                var checkInner = String.Format("{0}{1}", elName, innerEntityNameFormat);
                var regex = new Regex(Regex.Escape(checkInner));
                var destname = String.Format("{0}{1}{2}", container, innerEntityNameFormat, regex.Replace(elName, "", 1));

                if (!MapScreenEntities.ContainsKey(destname))
                {
                    listNewInners.Add(destname);
                    MapScreenEntities.Add(destname, doc.MapScreenEntities[elName]);
                }
                else
                {
                    var source = doc.MapScreenEntities[elName];
                    var dest = MapScreenEntities[destname];
                    var sourceMap = doc.MapScreenEntities[elName].MapProblematicXamlWriterProperties;
                    if (sourceMap == null)
                        sourceMap = new ProblematicXamlWriterProperties();
                    if (dest.MapProblematicXamlWriterProperties == null)
                        dest.MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();

                    var sourceString = dest.MapProblematicXamlWriterProperties.ToXml();
                    var previousmap = sourceString.FromXml<ProblematicXamlWriterProperties>();

                    if (!dest.PreserveSize)
                    {
                        if (previousmap.ContainsKey("Width"))
                            previousmap.Remove("Width");
                        if (previousmap.ContainsKey("Height"))
                            previousmap.Remove("Height");
                    }
                    dest.ProblematicXaml = source.ProblematicXaml;
                    if (!dest.PreserveColors)
                    {
                        foreach (var c in dest.MapProblematicXamlWriterProperties)
                        {
                            using (var reader = new StringReader(c.Value))
                            {
                                // object obj = s.Deserialize(reader);
                                using (var textReader = new XmlTextReader(reader))
                                {
                                    try
                                    {
                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                        if (obj is Brush || obj is Color)
                                        {
                                            if (previousmap.ContainsKey(c.Key))
                                                previousmap.Remove(c.Key);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                        if (sourceMap != null)
                        {
                            foreach (var c in sourceMap)
                            {
                                using (var reader = new StringReader(c.Value))
                                {
                                    // object obj = s.Deserialize(reader);
                                    using (var textReader = new XmlTextReader(reader))
                                    {
                                        try
                                        {
                                            var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                            if (obj is Brush || obj is Color)
                                            {
                                                previousmap.Add(c.Key, c.Value);
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        dest.CopyColors(source);
                    }
                    if (!dest.PreserveText)
                    {
                        if (previousmap.ContainsKey("Text"))
                            previousmap.Remove("Text");
                        if (sourceMap != null && sourceMap.ContainsKey("Text"))
                            previousmap.Add("Text", sourceMap["Text"]);
                        if (previousmap.ContainsKey("Content"))
                            previousmap.Remove("Content");
                        if (sourceMap != null && sourceMap.ContainsKey("Content"))
                            previousmap.Add("Content", sourceMap["Content"]);

                        dest.CopyTextDecorators(source);
                    }

                    sourceString = previousmap.ToXml();
                    dest.MapProblematicXamlWriterProperties = sourceString.FromXml<ProblematicXamlWriterProperties>();
                }
            }

            NeedsSave = bSavedNeed;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveEntity(FrameworkElement element)
        {
            UnsubscribePropertyChangeXamlWriterProperties(element);
            ClearTranslationMaps(element as UIElement);
            if (MapScreenEntities.ContainsKey(element.Name))
            {
                if (MapScreenEntities[element.Name].ContainsCode())
                    bRecalculateListScriptVariableUsed = true;
                MapScreenEntities.Remove(element.Name);
            }
            var checkRemove = String.Format("{0}{1}", element.Name, innerEntityNameFormat);
            var toRemove = (from c in MapScreenEntities.Keys.AsParallel()
                            where c.StartsWith(checkRemove)
                            select c).ToList();
            toRemove.ForEach(el => 
            {
                if (MapScreenEntities[el].ContainsCode())
                    bRecalculateListScriptVariableUsed = true;
                MapScreenEntities.Remove(el);
            });
            element.Dispose();
        }
        public bool IsDynamicEntity(FrameworkElement element)
        {
            if (element == null || String.IsNullOrEmpty(element.Name))
                return false;

            if (GetAutoForceDynamicOnClient(element))
                return true;

            return IsDynamicEntity(element.Name);
        }

        public bool HasCommandOrInnerHasCommand(String name)
        {
            if (String.IsNullOrEmpty(name))
                return false;
            if (MapScreenEntities.ContainsKey(name) &&
                MapScreenEntities[name].HasCommands)
                return true;

            foreach (var inner in GetListInners(name))
            {
                if (MapScreenEntities.ContainsKey(inner) &&
                    MapScreenEntities[inner].HasCommands)
                    return true;
            }

            return false;
        }

        public bool IsDynamicEntity(String name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            if (MapScreenEntities.ContainsKey(name) &&
                (MapScreenEntities[name].IsDynamicEntity() ||
                 !String.IsNullOrEmpty(MapScreenEntities[name].TagBrush) ||
                 !String.IsNullOrEmpty(MapScreenEntities[name].TagPen) ||
                 !String.IsNullOrEmpty(MapScreenEntities[name].PreservedText)))
                return true;

            foreach (var inner in GetListInners(name))
            {
                if (MapScreenEntities.ContainsKey(inner) &&
                    MapScreenEntities[inner].IsDynamicEntity())
                    return true;
            }

            return MapScreenEntities.ContainsKey(name) && 
                MapScreenEntities[name].ForceDynamicOnClient;
        }

        public bool ContainsDynamic(FrameworkElement element)
        {
            if (element == null || String.IsNullOrEmpty(element.Name))
                return false;

            if (MapScreenEntities.ContainsKey(element.Name) &&
                MapScreenEntities[element.Name].ContainsDynamic())
                return true;

            return false;
        }

        public bool ContainsScript(FrameworkElement element)
        {
            if (element == null || String.IsNullOrEmpty(element.Name))
                return false;

            if (MapScreenEntities.ContainsKey(element.Name) &&
                MapScreenEntities[element.Name].ContainsCode())
                return true;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddDynamicEntity(FrameworkElement element)
        {
            if (String.IsNullOrEmpty(element.Name))
                return;
            if (!MapScreenEntities.ContainsKey(element.Name))
            {
                ScreenEntity newScreenEntity = new ScreenEntity(element);
                newScreenEntity.PropertyChanged += entity_PropertyChanged;

                if (newScreenEntity.MapHashInner3DEntities != null)
                {
                    foreach (var e in newScreenEntity.MapHashInner3DEntities.Values)
                        e.PropertyChanged += entity_PropertyChanged;
                }
                MapScreenEntities.Add(element.Name, newScreenEntity);
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddDynamicEntity(String name)
        {
            if (String.IsNullOrEmpty(name))
                return;

            if (!MapScreenEntities.ContainsKey(name))
            {
                ScreenEntity newScreenEntity = new ScreenEntity();
                newScreenEntity.PropertyChanged += entity_PropertyChanged;

                if (newScreenEntity.MapHashInner3DEntities != null)
                {
                    foreach (var e in newScreenEntity.MapHashInner3DEntities.Values)
                        e.PropertyChanged += entity_PropertyChanged;
                }
                MapScreenEntities.Add(name, newScreenEntity);
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CopyDynamic(FrameworkElement from, FrameworkElement to, bool bCleanXamlPropertyBag = false)
        {
            AddDynamicEntity(to);
            MapScreenEntities[to.Name].CopyDynamics(MapScreenEntities[from.Name]);
            if (bCleanXamlPropertyBag)
                MapScreenEntities[to.Name].MapProblematicXamlWriterProperties = null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDynamicEntityStyle(FrameworkElement element, String resourceName)
        {
            AddDynamicEntity(element);
            MapScreenEntities[element.Name].StyleResource = resourceName;
        }

        //public void SetDynamicEntityBrush(FrameworkElement element, String resourceName)
        //{
        //    AddDynamicEntity(element);
        //    MapScreenEntities[element.Name].BrushResource = resourceName;
        //}
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDynamicEntityTagBrush(ScreenEntity entity, Brush brush)
        {
            if (brush != null)
            {
                entity.TagBrush = BindingExpressionHelper.Save(brush);
            }
            else
                entity.TagBrush = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetEntityPreservedBrush(ScreenEntity entity, Brush brush)
        {
            if (brush != null)
                entity.PreservedBrush = BindingExpressionHelper.Save(brush);
            else
                entity.PreservedBrush = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush GetEntityPreservedBrush(ScreenEntity entity)
        {
            if (!String.IsNullOrEmpty(entity.PreservedBrush))
            {
                try
                {
                    using (var reader = new StringReader(entity.PreservedBrush))
                    {
                        // object obj = s.Deserialize(reader);
                        using (var textReader = new XmlTextReader(reader))
                        {
                            var brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                            return brush;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush GetEntityPreservedPen(ScreenEntity entity)
        {
            if (!String.IsNullOrEmpty(entity.PreservedPen))
            {
                try
                {
                    using (var reader = new StringReader(entity.PreservedPen))
                    {
                        // object obj = s.Deserialize(reader);
                        using (var textReader = new XmlTextReader(reader))
                        {
                            var brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                            return brush;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }

        //public void SetDynamicEntityPen(FrameworkElement element, String resourceName)
        //{
        //    AddDynamicEntity(element);
        //    MapScreenEntities[element.Name].PenResource = resourceName;
        //}

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetEntityPreservedPen(ScreenEntity entity, Brush pen)
        {
            if (pen != null)
                entity.PreservedPen = BindingExpressionHelper.Save(pen); 
            else
                entity.PreservedPen = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetDynamicEntityTagPen(ScreenEntity entity, Brush pen)
        {
            if (pen != null)
                entity.TagPen = BindingExpressionHelper.Save(pen);
            else
                entity.TagPen = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetProblematicXaml(FrameworkElement element, String xaml)
        {
            AddDynamicEntity(element);
            MapScreenEntities[element.Name].ProblematicXaml = xaml;
            //SubscribePropertyChangeXamlWriterProperties(element, element.Name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetSourceProviderPath(FrameworkElement element, String provider, String path, bool bPassive = false)
        {
            AddDynamicEntity(element);
            MapScreenEntities[element.Name].SourceSymbolProvider = provider;
            MapScreenEntities[element.Name].SourceSymbolPath = path;
            MapScreenEntities[element.Name].SourceSymbolLinked = !bPassive;
            MapScreenEntities[element.Name].SourceSymbolLinkedPassive = bPassive;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveSourceProviderPath(string entityName)
        {
            MapScreenEntities[entityName].SourceSymbolProvider = null;
            MapScreenEntities[entityName].SourceSymbolPath = null;
            MapScreenEntities[entityName].SourceSymbolLinked = false;
            MapScreenEntities[entityName].SourceSymbolLinkedPassive = false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetSourceProviderPath(FrameworkElement element)
        {
            AddDynamicEntity(element);
            MapScreenEntities[element.Name].SourceSymbolLinked = false;
            MapScreenEntities[element.Name].SourceSymbolLinkedPassive = false;
        }

        Assembly[] assems;
        bool IsLocalAssembly(String assembly)
        {
            var path = System.IO.Path.GetPathRoot(assembly);
            if (String.IsNullOrEmpty(path))
                return true;

            if (assems == null)
                assems = AppDomain.CurrentDomain.GetAssemblies();

            var listLoaded = (from c in assems where c.Location == assembly || System.IO.Path.GetFileNameWithoutExtension(c.Location) == assembly select c).ToList();
            if (listLoaded.Count > 0)
                return true;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddListAssembly(IEnumerable<string> list)
        {
            if (list == null)
                return;

            foreach (var s in list)
            {
                if (!ListAssemblies.Contains(s) && !IsLocalAssembly(s))
                    ListAssemblies.Add(s);
            }
            assems = null;
        }
#endif
#if !WINDOWS_UWP
        static readonly String tagBackground = ScreenSettings.Properties.Settings.Default.BackgroundTag;
#else
        static readonly String tagBackground = "Background";
#endif

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveEntityBrushAndPen(FrameworkElement cv, FrameworkElement uie, String key = null, List<String> listInners = null)
        {
            if (listInners != null)
            {
                var preservedColorInners = (from c in MapScreenEntities.AsParallel()
                                            where listInners.Contains(c.Key) && c.Value.PreserveColors
                                            select c).ToList();
                preservedColorInners.ForEach(pair =>
                {
                    var contentControl = FindInnerControl(cv, pair.Key);
                    if (contentControl != null)
                        ResolveEntityBrushAndPen(cv, contentControl, pair.Key);
                });
            }

            if (String.IsNullOrEmpty(key))
                key = uie.Name;
            if (!MapScreenEntities.ContainsKey(key))
                return;

            //if (!String.IsNullOrEmpty(MapScreenEntities[key].BrushResource))
            //{
            //    uie.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //        {
            //            if (uie != null && cv.TryFindResource(MapScreenEntities[key].BrushResource) != null)
            //            {
            //                if (uie is Panel)
            //                    // (uie as Panel).SetBinding(Panel.BackgroundProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //                    uie.SetResourceReference(Panel.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //                else if (uie is Control)
            //                    // (uie as Control).SetBinding(Control.BackgroundProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //                    uie.SetResourceReference(Control.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //                else if (uie is System.Windows.Shapes.Shape)
            //                    // (uie as System.Windows.Shapes.Shape).SetBinding(System.Windows.Shapes.Shape.FillProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //                    uie.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, MapScreenEntities[key].BrushResource);

            //                (from c in uie.GetVisualChildrenOfType<Panel>()
            //                 where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //                 select c).ToList().ForEach(child =>
            //                 {
            //                     child.SetResourceReference(Panel.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //                 });
            //                (from c in uie.GetVisualChildrenOfType<Control>()
            //                 where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //                 select c).ToList().ForEach(child =>
            //                 {
            //                     child.SetResourceReference(Control.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //                 });
            //                (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
            //                 where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //                 select c).ToList().ForEach(child =>
            //                 {
            //                     child.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, MapScreenEntities[key].BrushResource);
            //                 });
            //            }
            //        });
            //}

            //if (!String.IsNullOrEmpty(MapScreenEntities[key].PenResource))
            //{
            //    uie.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //        {
            //            if (uie != null && cv.TryFindResource(MapScreenEntities[key].PenResource) != null)
            //            {
            //                if (uie is Control)
            //                    // (uie as Control).SetBinding(Control.BorderBrushProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].PenResource) });
            //                    uie.SetResourceReference(Control.BorderBrushProperty, MapScreenEntities[key].PenResource);
            //                else if (uie is System.Windows.Shapes.Shape)
            //                    // (uie as System.Windows.Shapes.Shape).SetBinding(System.Windows.Shapes.Shape.StrokeProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].PenResource) });
            //                    uie.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, MapScreenEntities[key].PenResource);

            //                (from c in uie.GetVisualChildrenOfType<Control>()
            //                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag
            //                 select c).ToList().ForEach(child =>
            //                 {
            //                     child.SetResourceReference(Control.BorderBrushProperty, MapScreenEntities[key].PenResource);
            //                 });
            //                (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
            //                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag
            //                 select c).ToList().ForEach(child =>
            //                 {
            //                     child.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, MapScreenEntities[key].PenResource);
            //                 });
            //            }
            //        });
            //}

            if (!String.IsNullOrEmpty(MapScreenEntities[key].TagBrush))
            {
                Brush brush = null;
                try
                {
#if !WINDOWS_UWP
                    using (var reader = new StringReader(MapScreenEntities[key].TagBrush))
                    {
                        // object obj = s.Deserialize(reader);
                        using (var textReader = new XmlTextReader(reader))
                        {
                            brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                            if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                            {
                                var me = (brush as VisualBrush).Visual as MediaElement;
                                SetBaseUri(me);
                                var list = new List<MediaElement>() { me };
                                Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(list);
                            }
                        }
                    }
#else
                    brush = Windows.UI.Xaml.Markup.XamlReader.Load(MapScreenEntities[key].TagBrush) as Brush;
#endif
                }
                catch (Exception ex)
                {
                    brush = null;
                }

                if (brush != null)
                {
                    (from c in uie.GetVisualChildrenOfType<Panel>()
                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                     select c).ToList().ForEach(child =>
                        {
                            child.Background = brush;
                        });
                    (from c in uie.GetVisualChildrenOfType<Control>()
                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                     select c).ToList().ForEach(child =>
                        {
                            child.Background = brush;
                        });
#if !WINDOWS_UWP
                    (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                     where (c.Tag as String) == tagBackground && c.Opacity != 0
#else
                    (from c in uie.GetVisualChildrenOfType<Windows.UI.Xaml.Shapes.Shape>()
                     where (c.Tag as String) == tagBackground && c.Opacity != 0
#endif
                     select c).ToList().ForEach(child =>
                        {
                            child.Fill = brush;
                        });

                    if (MapScreenEntities[key].PreserveColors)
                    {
                        if (uie is Panel)
                            (uie as Panel).Background = brush;
                        else if (uie is Microsoft.Expression.Media.IShape)
                            (uie as Microsoft.Expression.Media.IShape).Fill = brush;
                        else if (uie is Control)
                            (uie as Control).Background = brush;
                        else if (uie is Border)
                            (uie as Border).Background = brush;
                        else if (uie is System.Windows.Shapes.Shape)
                            (uie as System.Windows.Shapes.Shape).Fill = brush;
                    }
                }
            }

            if (!String.IsNullOrEmpty(MapScreenEntities[key].TagPen))
            {
                Brush brush = null;
                try
                {
#if !WINDOWS_UWP
                    using (var reader = new StringReader(MapScreenEntities[key].TagPen))
                    {
                        // object obj = s.Deserialize(reader);
                        using (var textReader = new XmlTextReader(reader))
                        {
                            brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                            if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                            {
                                var me = (brush as VisualBrush).Visual as MediaElement;
                                SetBaseUri(me);
                                var list = new List<MediaElement>() { me };
                                Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(list);
                            }
                        }
                    }
#else
                    brush = Windows.UI.Xaml.Markup.XamlReader.Load(MapScreenEntities[key].TagPen) as Brush;
#endif
                }
                catch (Exception ex)
                {
                    brush = null;
                }

                if (brush != null)
                {
                    (from c in uie.GetVisualChildrenOfType<Control>()
                        where (c.Tag as String) == tagBackground && c.Opacity != 0
                     select c).ToList().ForEach(child =>
                        {
                            child.BorderBrush = brush;
                            child.Foreground = brush;
                        });
#if !WINDOWS_UWP
                    (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                        where (c.Tag as String) == tagBackground && c.Opacity != 0
#else
                    (from c in uie.GetVisualChildrenOfType<Windows.UI.Xaml.Shapes.Shape>()
                     where (c.Tag as String) == tagBackground && c.Opacity != 0
#endif
                     select c).ToList().ForEach(child =>
                        {
                            child.Stroke = brush;
                        });

                    if (MapScreenEntities[key].PreserveColors)
                    {
                        if (uie is Control)
                        {
                            (uie as Control).BorderBrush = brush;
                            (uie as Control).Foreground = brush;
                        }
                        else if (uie is System.Windows.Shapes.Shape)
                            (uie as System.Windows.Shapes.Shape).Stroke = brush;
                    }
                }
            }
        }

#if !WINDOWS_UWP
        public void SetBaseUri(MediaElement element)
        {
            if (fileSystemProviderBase != null)
                return;

            var dest = System.IO.Path.GetDirectoryName(FullPath) + "\\";
            (element as IUriContext).BaseUri = new Uri(dest, UriKind.RelativeOrAbsolute);
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveEntityBrushAndPen(FrameworkElement cv)
        {
            //var listBrush = (from entry in MapScreenEntities.AsParallel()
            //                 where !String.IsNullOrEmpty(entry.Value.BrushResource)
            //                 select entry.Key).ToList();

            //listBrush.ForEach(key =>
            //{
            //    var uie = FindInnerControl(cv, key);
            //    // var uie = cv.FindName(key) as FrameworkElement;
            //    if (uie != null && cv.TryFindResource(MapScreenEntities[key].BrushResource) != null)
            //    {
            //        if (uie is Panel)
            //            // (uie as Panel).SetBinding(Panel.BackgroundProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //            uie.SetResourceReference(Panel.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //        else if (uie is Control)
            //            // (uie as Control).SetBinding(Control.BackgroundProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //            uie.SetResourceReference(Control.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //        else if (uie is System.Windows.Shapes.Shape)
            //            // (uie as System.Windows.Shapes.Shape).SetBinding(System.Windows.Shapes.Shape.FillProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].BrushResource) });
            //            uie.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, MapScreenEntities[key].BrushResource);

            //        (from c in uie.GetVisualChildrenOfType<Panel>()
            //         where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //         select c).ToList().ForEach(child =>
            //         {
            //             child.SetResourceReference(Panel.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //         });
            //        (from c in uie.GetVisualChildrenOfType<Control>()
            //         where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //         select c).ToList().ForEach(child =>
            //         {
            //             child.SetResourceReference(Control.BackgroundProperty, MapScreenEntities[key].BrushResource);
            //         });
            //        (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
            //         where (c.Tag as String) == Properties.Settings.Default.BackgroundTag
            //         select c).ToList().ForEach(child =>
            //         {
            //             child.SetResourceReference(System.Windows.Shapes.Shape.FillProperty, MapScreenEntities[key].BrushResource);
            //         });
            //    }
            //});

            //var listPen = (from entry in MapScreenEntities.AsParallel()
            //               where !String.IsNullOrEmpty(entry.Value.PenResource)
            //               select entry.Key).ToList();

            //listPen.ForEach(key =>
            //{
            //    var uie = FindInnerControl(cv, key);
            //    // var uie = cv.FindName(key) as FrameworkElement;
            //    if (uie != null && cv.TryFindResource(MapScreenEntities[key].PenResource) != null)
            //    {
            //        if (uie is Control)
            //            // (uie as Control).SetBinding(Control.BorderBrushProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].PenResource) });
            //            uie.SetResourceReference(Control.BorderBrushProperty, MapScreenEntities[key].PenResource);
            //        else if (uie is System.Windows.Shapes.Shape)
            //            // (uie as System.Windows.Shapes.Shape).SetBinding(System.Windows.Shapes.Shape.StrokeProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].PenResource) });
            //            uie.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, MapScreenEntities[key].PenResource);

            //        (from c in uie.GetVisualChildrenOfType<Control>()
            //         where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag
            //         select c).ToList().ForEach(child =>
            //         {
            //             child.SetResourceReference(Control.BorderBrushProperty, MapScreenEntities[key].PenResource);
            //         });
            //        (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
            //         where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag
            //         select c).ToList().ForEach(child =>
            //         {
            //             child.SetResourceReference(System.Windows.Shapes.Shape.StrokeProperty, MapScreenEntities[key].PenResource);
            //         });
            //    }
            //});

            var listTagBrush = (from entry in MapScreenEntities
                                where !String.IsNullOrEmpty(entry.Value.TagBrush)
                                select entry.Key).ToList();
            listTagBrush.ForEach(key =>
                {
                    var uie = FindInnerControl(cv, key);
                    // var uie = cv.FindName(key) as FrameworkElement;
                    if (uie != null)
                    {
                        Brush brush = null;
                        try
                        {
#if !WINDOWS_UWP
                            using (var reader = new StringReader(MapScreenEntities[key].TagBrush))
                            {
                                // object obj = s.Deserialize(reader);
                                using (var textReader = new XmlTextReader(reader))
                                {
                                    brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                                    if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                                    {
                                        var me = (brush as VisualBrush).Visual as MediaElement;
                                        SetBaseUri(me);
                                        var list = new List<MediaElement>() { me };
                                        Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(list);
                                    }
                                }
                            }
#else
                            brush = Windows.UI.Xaml.Markup.XamlReader.Load(MapScreenEntities[key].TagBrush) as Brush;
#endif
                        }
                        catch (Exception ex)
                        {
                            brush = null;
                        }

                        if (brush != null)
                        {
                            (from c in uie.GetVisualChildrenOfType<Panel>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
                             {
                                 child.Background = brush;
                             });
                            (from c in uie.GetVisualChildrenOfType<Control>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
                             {
                                 child.Background = brush;
                             });
#if !WINDOWS_UWP
                            (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
#else
                            (from c in uie.GetVisualChildrenOfType<Windows.UI.Xaml.Shapes.Shape>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
#endif
                             {
                                 child.Fill = brush;
                             });
                        }
                    }
                });

            var listTagPen = (from entry in MapScreenEntities
                              where !String.IsNullOrEmpty(entry.Value.TagPen)
                              select entry.Key).ToList();
            listTagPen.ForEach(key =>
                {
                    var uie = FindInnerControl(cv, key);
                    // var uie = cv.FindName(key) as FrameworkElement;
                    if (uie != null)
                    {
                        Brush brush = null;
                        try
                        {
                            using (var reader = new StringReader(MapScreenEntities[key].TagPen))
                            {
                                // object obj = s.Deserialize(reader);
#if !WINDOWS_UWP
                                using (var textReader = new XmlTextReader(reader))
                                {
                                    brush = System.Windows.Markup.XamlReader.Load(textReader) as Brush;
                                    if (brush is VisualBrush && (brush as VisualBrush).Visual is MediaElement)
                                    {
                                        var me = (brush as VisualBrush).Visual as MediaElement;
                                        SetBaseUri(me);
                                        var list = new List<MediaElement>() { me };
                                        Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(list);
                                    }
                                }
#else
                                brush = Windows.UI.Xaml.Markup.XamlReader.Load(MapScreenEntities[key].TagPen) as Brush;
#endif
                            }
                        }
                        catch (Exception ex)
                        {
                            brush = null;
                        }

                        if (brush != null)
                        {
                            (from c in uie.GetVisualChildrenOfType<Control>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
                            {
                                child.BorderBrush = brush;
                                child.Foreground = brush;
                            });
#if !WINDOWS_UWP
                            (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
#else
                            (from c in uie.GetVisualChildrenOfType<Windows.UI.Xaml.Shapes.Shape>()
                             where (c.Tag as String) == tagBackground && c.Opacity != 0
                             select c).ToList().ForEach(child =>
#endif
                             {
                                 child.Stroke = brush;
                            });
                        }
                    }
                });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveEntityStyleBinding(FrameworkElement cv)
        {
            var listStyled = (from entry in MapScreenEntities.AsParallel()
                              where !String.IsNullOrEmpty(entry.Value.StyleResource)
                              select entry.Key).ToList();

            listStyled.ForEach(key =>
                {
                    var uie = FindInnerControl(cv, key);
                    // var uie = cv.FindName(key) as FrameworkElement;
#if !WINDOWS_UWP
                    if (uie != null && cv.TryFindResource(MapScreenEntities[key].StyleResource) != null)
                        // uie.SetBinding(FrameworkElement.StyleProperty, new Binding() { Source = new StaticResourceExtension(MapScreenEntities[key].StyleResource) });
                        uie.SetResourceReference(FrameworkElement.StyleProperty, MapScreenEntities[key].StyleResource);
#else
                    Object res;
                    if (uie != null && cv.Resources.TryGetValue(MapScreenEntities[key].StyleResource, out res))
                        uie.Style = res as Style;
#endif
                });

            ResolveEntityBrushAndPen(cv);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CleanEntityStyleBinding(FrameworkElement cv)
        {
            var listStyled = (from entry in MapScreenEntities.AsParallel()
                              where !String.IsNullOrEmpty(entry.Value.StyleResource)
                              select entry.Key).ToList();

            listStyled.ForEach(key =>
            {
                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                    uie.Style = null;
            });

            //var listBrush = (from entry in MapScreenEntities.AsParallel()
            //                 where !String.IsNullOrEmpty(entry.Value.BrushResource)
            //                 select entry.Key).ToList();

            //listBrush.ForEach(key =>
            //{
            //    var uie = FindInnerControl(cv, key);
            //    // var uie = cv.FindName(key) as FrameworkElement;
            //    if (uie != null)
            //    {
            //        if (uie is Panel)
            //            (uie as Panel).Background = null;
            //        else if (uie is Control)
            //            (uie as Control).Background = null;
            //        else if (uie is System.Windows.Shapes.Shape)
            //            (uie as System.Windows.Shapes.Shape).Fill = null;
            //    }
            //});

            //var listPen = (from entry in MapScreenEntities.AsParallel()
            //               where !String.IsNullOrEmpty(entry.Value.PenResource)
            //               select entry.Key).ToList();

            //listPen.ForEach(key =>
            //{
            //    var uie = FindInnerControl(cv, key);
            //    // var uie = cv.FindName(key) as FrameworkElement;
            //    if (uie != null)
            //    {
            //        if (uie is Control)
            //        {
            //            (uie as Control).BorderBrush = null;
            //            (uie as Control).Foreground = null;
            //        }
            //        else if (uie is System.Windows.Shapes.Shape)
            //            (uie as System.Windows.Shapes.Shape).Stroke = null;
            //    }
            //});
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RefreshEntityStyleBinding(FrameworkElement cv, bool bStyle = true,string entityname = null)
        {
            if (bStyle)
            {
                var listStyled = (from entry in MapScreenEntities.AsParallel()
                                  where !String.IsNullOrEmpty(entry.Value.StyleResource) && (entityname == null || (entityname != null && entry.Key == entityname))
                                  select entry.Key).ToList();

                listStyled.ForEach(key =>
                {
                    var uie = FindInnerControl(cv, key);
                    // var uie = cv.FindName(key) as FrameworkElement;
                    if (uie != null)
                    {
#if !WINDOWS_UWP
                        uie.Style = cv.TryFindResource(MapScreenEntities[key].StyleResource) as Style;
#else
                        Object res;
                        if (uie.Resources.TryGetValue(MapScreenEntities[key].StyleResource, out res))
                            uie.Style = res as Style;
#endif
                    }
                });
            }

            ResolveEntityBrushAndPen(cv);

            var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
                                       where !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
                                       select entry.Key).ToList();

            listProblematicXaml.ForEach(key =>
            {
                var uie = FindInnerControl(cv, key);
                // var uie = cv.FindName(key) as FrameworkElement;
                if (uie != null)
                {
                    Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(uie, MapScreenEntities[key].ProblematicXaml);
                    UpdateProblematicXamlWriterProperties(uie, key);
                }
            });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveEntityTextDecorators(FrameworkElement cv)
        {
            var listTextDecorators = (from entry in MapScreenEntities.AsParallel()
                                      where entry.Value.TagDecorators != 0
                                      select entry.Key).ToList();

            listTextDecorators.ForEach(key =>
            {
                var decorators = MapScreenEntities[key].TagDecorators;
                if (BitOperations.CheckBitValue((byte)TextDecorators.Underline, decorators))
                {
                    var uie = FindInnerControl(cv, key);
                    if (uie != null)
                        Converters.TextBoxProperties.ApplyUnderlineTextToChilds(uie);
                }
            });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveEntityTextDecorators(FrameworkElement uie, String key)
        {
            if (MapScreenEntities.ContainsKey(key))
            {
                var decorators = MapScreenEntities[key].TagDecorators;
                if (BitOperations.CheckBitValue((byte)TextDecorators.Underline, decorators))
                {
                    if (uie != null)
                        Converters.TextBoxProperties.ApplyUnderlineTextToChilds(uie);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateProblematicXamlWriterProperties(FrameworkElement uie, String key, bool bDontUpdateSize = false)
        {
            var original = uie;

            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as FrameworkElement;

            if (uie == null)
                return;
            bool bUpdateBaseUri = false;
            if (MapScreenEntities.ContainsKey(key) && 
                MapScreenEntities[key].MapProblematicXamlWriterProperties != null)
            {
                var listofkeys = (from c in MapScreenEntities[key].MapProblematicXamlWriterProperties.AsParallel()
                                  where
                                      !String.IsNullOrEmpty(c.Value)
#if !WINDOWS_UWP
                                      && IsValidPropertyXamlWriter(c.Key)
#endif
                                  select c.Key).ToList();

                if (bDontUpdateSize)
                {
                    if (listofkeys.Contains("Width"))
                        listofkeys.Remove("Width");
                    if (listofkeys.Contains("Height"))
                        listofkeys.Remove("Height");
                }
                MapScreenEntities[key].bUpdatingProperty = true;

                try
                {
                    var type = uie.GetType();
                    if(MapScreenEntities[key].MapProblematicXamlWriterProperties != null)
                    listofkeys.ForEach(propName =>
                    {
                        /*
                        if (propName.Contains('.'))
                        {
                            try
                            {
                                var names = propName.Split(new char[] { '.' });
                                Type ownerType = null;
                                if (names[0] == "Canvas")
                                    ownerType = typeof(Canvas);
                                else if (names[0] == "InkCanvas")
                                    ownerType = typeof(InkCanvas);
                                var descriptor = DependencyPropertyDescriptor.FromName(
                                    names[1],
                                    ownerType,
                                    uie.GetType());

                                var t = descriptor.PropertyType;
                                var valueString = MapScreenEntities[key].MapProblematicXamlWriterProperties[propName];
                                //var s = new XmlSerializer(t, new Type[] 
                                //                                            {
                                //                                                typeof(Brush),
                                //                                                typeof(BitmapCacheBrush),
                                //                                                typeof(GradientBrush),
                                //                                                typeof(LinearGradientBrush),
                                //                                                typeof(RadialGradientBrush),
                                //                                                typeof(VisualBrush),
                                //                                                typeof(SolidColorBrush),
                                //                                                typeof(TileBrush),
                                //                                                typeof(MatrixTransform)
                                //                                            });
                                using (var reader = new StringReader(valueString))
                                {
                                    // object obj = s.Deserialize(reader);
                                    using (var textReader = new XmlTextReader(reader))
                                    {
                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                        descriptor.SetValue(uie, obj);
                                    }
                                }

                                MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(propName);
                            }
                            catch (Exception ex)
                            {
                                MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(propName);
                            }
                        }
                        else
                        */
                        {
                            var prop = type.GetProperty(propName);
                            if (prop != null)
                            {
                                var valueString = MapScreenEntities[key].MapProblematicXamlWriterProperties[propName];
                                if (!String.IsNullOrEmpty(valueString))
                                {
                                    var t = prop.PropertyType;
                                    try
                                    {
                                        //var s = new XmlSerializer(t, new Type[] 
                                        //                                    {
                                        //                                        typeof(Brush),
                                        //                                        typeof(BitmapCacheBrush),
                                        //                                        typeof(GradientBrush),
                                        //                                        typeof(LinearGradientBrush),
                                        //                                        typeof(RadialGradientBrush),
                                        //                                        typeof(VisualBrush),
                                        //                                        typeof(SolidColorBrush),
                                        //                                        typeof(TileBrush),
                                        //                                        typeof(MatrixTransform)
                                        //                                    });
#if !WINDOWS_UWP
                                        using (var reader = new StringReader(valueString))
#endif
                                        {
                                            // object obj = s.Deserialize(reader);
#if !WINDOWS_UWP
                                            using (var textReader = new XmlTextReader(reader))
#endif
                                            {
#if !WINDOWS_UWP
                                                var obj = System.Windows.Markup.XamlReader.Load(textReader);
#else
                                                var obj = Windows.UI.Xaml.Markup.XamlReader.Load(valueString);
#endif
                                                if (obj is VisualBrush)
                                                    bUpdateBaseUri = true;
                                                if (propName == "Background" || propName == "Fill")
                                                {
                                                    (from c in uie.GetVisualChildrenOfType<Panel>()
                                                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                                                     select c).ToList().ForEach(child =>
                                                     {
                                                         child.Background = obj as Brush;
                                                     });

                                                    (from c in uie.GetVisualChildrenOfType<Control>()
                                                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                                                     select c).ToList().ForEach(child =>
                                                     {
                                                         child.Background = obj as Brush;
                                                     });

#if !WINDOWS_UWP
                                                    (from c in uie.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
#else
                                                    (from c in uie.GetVisualChildrenOfType<Windows.UI.Xaml.Shapes.Shape>()
#endif
                                                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                                                     select c).ToList().ForEach(child =>
                                                     {
                                                         child.Fill = obj as Brush;
                                                     });
                                                }
                                                else if (propName == "BorderBrush" || propName == "Foreground" || propName == "Stroke")
                                                {
                                                    (from c in uie.GetVisualChildrenOfType<Control>()
                                                     where (c.Tag as String) == tagBackground && c.Opacity != 0
                                                     select c).ToList().ForEach(child =>
                                                     {
                                                         child.BorderBrush = obj as Brush;
                                                         child.Foreground = obj as Brush;
                                                     });
                                                }

#if !WINDOWS_UWP
                                                var accessor = FastReflectionCaches.PropertyAccessorCache.Get(prop);
                                                accessor.SetValue(uie, obj);
#else
                                                prop.SetValue(uie, obj);
#endif
                                            }
                                            // prop.SetValue(uie, obj, null);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(propName);
                                    }
                                }
                            }
                        }
                    });
                    if(bUpdateBaseUri)
                        SetImagesBaseUri(new List<UIElement>() { uie });
                    ResolveEntityTextDecorators(uie, key);
                }
                finally
                {
                    MapScreenEntities[key].bUpdatingProperty = false;
                }
            }
            //SubscribePropertyChangeXamlWriterProperties(original, key);
        }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public void SubscribeAllChangeXamlWriterProperties(FrameworkElement cv)
        //{
        //    if (InRuntime)
        //        return;

//    var listProblematicXaml = (from entry in MapScreenEntities.AsParallel()
//                               where !String.IsNullOrEmpty(entry.Value.ProblematicXaml)
//                               select entry.Key).ToList();

//    listProblematicXaml.ForEach(key =>
//    {
//        var uie = FindInnerControl(cv, key);
//        // var uie = cv.FindName(key) as FrameworkElement;
//        if (uie != null)
//        {
//            SubscribePropertyChangeXamlWriterProperties(uie, key);
//        }
//    });
//}
#endif
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnsubscribeAllChangeXamlWriterProperties()
        {
            if (InRuntime || elementSubscribed == null)
                return;

            var list = new List<UIElement>(elementSubscribed);
            list.ForEach(uie =>
                {
                    UnsubscribePropertyChangeXamlWriterProperties(uie);
                });
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSubscribePropertyChangeXamlWriterProperties(UIElement uie)
        {
            if (InRuntime)
                return false;

            if (elementSubscribed == null || propertyChangeNotifierList == null)
                return false;
            return elementSubscribed.Contains(uie);
        }

        List<UIElement> elementSubscribed;
        List<PropertyChangeNotifier> propertyChangeNotifierList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnsubscribePropertyChangeXamlWriterProperties(UIElement uie)
        {
            if (InRuntime)
                return;

            if (elementSubscribed == null || propertyChangeNotifierList == null)
                return;
            if (!elementSubscribed.Contains(uie))
            {
                if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                {
                    var contentuie = (uie as ContentControl).Content as FrameworkElement;
                    if (contentuie != null)
                        UnsubscribePropertyChangeXamlWriterProperties(contentuie);
                }
                return;
            }
            elementSubscribed.Remove(uie);

            var original = uie;
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as FrameworkElement;

            var list = (from c in propertyChangeNotifierList
                        where c.PropertySource == uie || c.PropertySource == original
                        select c).ToList();
            list.ForEach(prop =>
                {
                    propertyChangeNotifierList.Remove(prop);
                    prop.Dispose();
                });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateXamlWriterProperty(UIElement uie, String key, DependencyProperty dp)
        {
            if (InRuntime || IsInLibrary || inExecution || IsOnBlindServer() || String.IsNullOrEmpty(key) ||
               !MapScreenEntities.ContainsKey(key) || (String.IsNullOrEmpty(MapScreenEntities[key].ProblematicXaml) &&
               !MapScreenEntities[key].SourceSymbolLinked))
                return;
            var original = uie;
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;
            if (MapScreenEntities.ContainsKey(key) && !MapScreenEntities[key].bUpdatingProperty)
            {
                var value = uie.ReadLocalValue(dp);
                if (value != null && value != DependencyProperty.UnsetValue && value != this &&
                    !(value is BindingExpression))
                {
                    var type = dp.PropertyType;
                    try
                    {
                        var sw = System.Windows.Markup.XamlWriter.Save(value);
                        if (MapScreenEntities[key].MapProblematicXamlWriterProperties == null)
                            MapScreenEntities[key].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();
                        if (!MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dp.Name))
                        {
                            MapScreenEntities[key].MapProblematicXamlWriterProperties.Add(dp.Name, sw);
                            NeedsSave = true;
                        }
                        else
                        {
                            if (MapScreenEntities[key].MapProblematicXamlWriterProperties[dp.Name] != sw)
                            {
                                MapScreenEntities[key].MapProblematicXamlWriterProperties[dp.Name] = sw;
                                NeedsSave = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (MapScreenEntities.ContainsKey(key) &&
                            MapScreenEntities[key].MapProblematicXamlWriterProperties != null &&
                            MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dp.Name))
                            MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(dp.Name);
                    }
                }
                else
                {
                    if (MapScreenEntities.ContainsKey(key) &&
                        MapScreenEntities[key].MapProblematicXamlWriterProperties != null &&
                        MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dp.Name))
                        MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(dp.Name);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SubscribePropertyChangeXamlWriterProperties(UIElement uie, String key)
        {
            if (InRuntime || IsInLibrary || inExecution || IsOnBlindServer() || String.IsNullOrEmpty(key) ||
                !MapScreenEntities.ContainsKey(key) || (String.IsNullOrEmpty(MapScreenEntities[key].ProblematicXaml) &&
                !MapScreenEntities[key].SourceSymbolLinked))
                return;

            if (elementSubscribed == null)
                elementSubscribed = new List<UIElement>();
            if (propertyChangeNotifierList == null)
                propertyChangeNotifierList = new List<PropertyChangeNotifier>();
            if (elementSubscribed.Contains(uie))
                return;
            elementSubscribed.Add(uie);

            var original = uie;
            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;

            using (var cursor = new WaitCursor())
            {
                var map = new ProblematicXamlWriterProperties();
                foreach (PropertyDescriptor pd in PropertyChangeNotifier.GetPropertyList(uie.GetType()))
                {
                    DependencyPropertyDescriptor dpd =
                        DependencyPropertyDescriptor.FromProperty(pd);

                    if (dpd != null &&
                        !dpd.IsReadOnly && IsValidPropertyXamlWriter(dpd.Name))
                    {
                        bool bXmlIgnore = false;
                        foreach (var attribute in dpd.Attributes)
                        {
                            if (attribute is XmlIgnoreAttribute)
                            {
                                bXmlIgnore = true;
                                break;
                            }
                        }
                        if (bXmlIgnore)
                        {
                            if (MapScreenEntities.ContainsKey(key) &&
                                MapScreenEntities[key].MapProblematicXamlWriterProperties != null &&
                                MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dpd.Name))
                            {
                                MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(dpd.Name);
                                NeedsSave = true;
                            }
                            continue;
                        }

                        var notifier = new PropertyChangeNotifier(uie, dpd.Name);
                        notifier.ValueChanged += (o, e) =>
                            {
                                // AddDynamicEntity(original as FrameworkElement);
                                if (MapScreenEntities.ContainsKey(key) && !MapScreenEntities[key].bUpdatingProperty)
                                {
                                    var value = uie.ReadLocalValue(dpd.DependencyProperty);
                                    if (value != null && value != DependencyProperty.UnsetValue && value != this &&
                                        !(value is BindingExpression))
                                    {
                                        var type = pd.PropertyType;
                                        try
                                        {
                                            var sw = System.Windows.Markup.XamlWriter.Save(value);
                                            //var serializer = new XmlSerializer(pd.PropertyType,
                                            //                                    new Type[] 
                                            //                                {
                                            //                                    typeof(Brush),
                                            //                                    typeof(BitmapCacheBrush),
                                            //                                    typeof(GradientBrush),
                                            //                                    typeof(LinearGradientBrush),
                                            //                                    typeof(RadialGradientBrush),
                                            //                                    typeof(VisualBrush),
                                            //                                    typeof(SolidColorBrush),
                                            //                                    typeof(TileBrush),
                                            //                                    typeof(MatrixTransform)
                                            //                                });
                                            //using (var sw = new StringWriter())
                                            {
                                                // serializer.Serialize(sw, value);

                                                if (MapScreenEntities[key].MapProblematicXamlWriterProperties == null)
                                                    MapScreenEntities[key].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();
                                                if (!MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dpd.Name))
                                                {
                                                    MapScreenEntities[key].MapProblematicXamlWriterProperties.Add(dpd.Name, sw);
                                                    NeedsSave = true;
                                                }
                                                else
                                                {
                                                    if (MapScreenEntities[key].MapProblematicXamlWriterProperties[dpd.Name] != sw)
                                                    {
                                                        MapScreenEntities[key].MapProblematicXamlWriterProperties[dpd.Name] = sw;
                                                        NeedsSave = true;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (MapScreenEntities.ContainsKey(key) &&
                                                MapScreenEntities[key].MapProblematicXamlWriterProperties != null &&
                                                MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dpd.Name))
                                            {
                                                MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(dpd.Name);
                                                NeedsSave = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (MapScreenEntities.ContainsKey(key) &&
                                            MapScreenEntities[key].MapProblematicXamlWriterProperties != null &&
                                            MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(dpd.Name))
                                        {
                                            MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(dpd.Name);
                                            NeedsSave = true;
                                        }
                                    }
                                }
                            };
                        propertyChangeNotifierList.Add(notifier);
                    }
                }
            }
        }

        static bool IsValidPropertyXamlWriter(String Name)
        {
            return Name != "Tag" && Name != "DataContext" && Name != "Style" && Name != "Uid" &&
                        !Name.Contains("XmlnsDictionary") &&
                        !Name.Contains("XmlNamespaceMaps") &&
                        !Name.Contains("IsMouseOver") &&
                        !Name.Contains("Template") &&
                        !Name.Contains("HeaderTemplate") &&
                        !Name.Contains("IsVisible") &&
                        //!Name.Contains("Visibility") &&
                        !Name.Contains("ContextMenu") &&
                        !Name.Contains("NameScope") &&
                        !Name.Contains("DesignerProperties.IsInDesignMode") &&
                        !Name.Contains("BaseUriHelper.BaseUri") &&
                        String.Compare(Name, "Name", true) != 0;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SaveProblematicXamlWriterProperties(UIElement uie, String key, bool bForce = false)
        {
            if (InRuntime)
                return;

            if (!MapScreenEntities.ContainsKey(key))
                return;
            if (String.IsNullOrEmpty(MapScreenEntities[key].ProblematicXaml) ||
                MapScreenEntities[key].MapProblematicXamlWriterProperties == null)
                return;

            if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                uie = (uie as ContentControl).Content as UIElement;

            if (bForce)
            {
                var map = new ProblematicXamlWriterProperties();
                foreach (PropertyDescriptor pd in PropertyChangeNotifier.GetPropertyList(uie.GetType()))
                {
                    DependencyPropertyDescriptor dpd =
                        DependencyPropertyDescriptor.FromProperty(pd);

                    if (dpd != null &&
                        !dpd.IsReadOnly && IsValidPropertyXamlWriter(dpd.Name))
                    {
                        var value = uie.ReadLocalValue(dpd.DependencyProperty);
                        if (value != DependencyProperty.UnsetValue && value != this &&
                            !(value is BindingExpression))
                        {
                            var type = pd.PropertyType;
                            try
                            {
                                var sw = System.Windows.Markup.XamlWriter.Save(value);
                                //var serializer = new XmlSerializer(pd.PropertyType,
                                //                                    new Type[] 
                                //                                        {
                                //                                            typeof(Brush),
                                //                                            typeof(BitmapCacheBrush),
                                //                                            typeof(GradientBrush),
                                //                                            typeof(LinearGradientBrush),
                                //                                            typeof(RadialGradientBrush),
                                //                                            typeof(VisualBrush),
                                //                                            typeof(SolidColorBrush),
                                //                                            typeof(TileBrush),
                                //                                            typeof(MatrixTransform)
                                //                                        });
                                //using (var sw = new StringWriter())
                                {
                                    // serializer.Serialize(sw, value);
                                    map.Add(dpd.Name, sw);
                                }
                                NeedsSave = true;
                            }
                            catch (Exception ex)
                            {
                                if (map.ContainsKey(dpd.Name))
                                    map.Remove(dpd.Name);
                            }
                        }
                    }
                }
                MapScreenEntities[key].MapProblematicXamlWriterProperties = map.Count > 0 ? map : null;
            }
            
            {
                var list = (from c in MapScreenEntities[key].MapProblematicXamlWriterProperties.AsParallel()
                            where c.Value == String.Empty || !IsValidPropertyXamlWriter(c.Key)
                            select c).ToList();
                list.ForEach(c => MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(c.Key));

                /*
                var type = uie.GetType();
                list.ForEach(c =>
                    {
                        var prop = type.GetProperty(c.Key);
                        if (prop != null)
                        {
                            var t = prop.PropertyType;
                            try
                            {
                                var serializer = new XmlSerializer(prop.PropertyType, 
                                                                    new Type[] 
                                                                        {
                                                                            typeof(Brush),
                                                                            typeof(BitmapCacheBrush),
                                                                            typeof(GradientBrush),
                                                                            typeof(LinearGradientBrush),
                                                                            typeof(RadialGradientBrush),
                                                                            typeof(VisualBrush),
                                                                            typeof(SolidColorBrush),
                                                                            typeof(TileBrush),
                                                                            typeof(MatrixTransform)
                                                                        });

                                using (var sw = new StringWriter())
                                {
                                    var accessor = FastReflectionCaches.PropertyAccessorCache.Get(prop);
                                    serializer.Serialize(sw, accessor.GetValue(uie));
                                    MapScreenEntities[key].MapProblematicXamlWriterProperties[c.Key] = sw.ToString();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (MapScreenEntities[key].MapProblematicXamlWriterProperties.ContainsKey(c.Key))
                                    MapScreenEntities[key].MapProblematicXamlWriterProperties.Remove(c.Key);
                            }
                        }
                    });
                 * */
            }
        }
#endif

        public List<OPCUAEntityReference> GetDynamicMapForElement(String element)
        {
            lock (lockObject)
            {
                var list = new List<OPCUAEntityReference>();
                if (!MapScreenEntities.ContainsKey(element))
                    return list;

                var entity = MapScreenEntities[element];
                var listDynamic = (from p in entity.GetAllSourceEntityReferences().AsParallel()
#if !WINDOWS_UWP && !NET_STANDARD
                                   where p.IsValid 
#endif
                                   select p).ToList();
                if (listDynamic.Count() > 0)
                    list.AddRange(listDynamic);

                return list;
            }
        }

#if !NET_STANDARD
        public void UpdateDynamicMapForElementAndChilds(FrameworkElement element, String name, Dictionary<String, String> map)
        {
            lock (lockObject)
            {
                if (MapScreenEntities.ContainsKey(name))
                {
                    var entity = MapScreenEntities[name];
                    entity.UpdateDynamicMapForElement(map, name);
                }

#if !WINDOWS_UWP
                var checkInner = String.Format("{0}{1}", name, innerEntityNameFormat);
                var toCheck = (from c in MapScreenEntities.Keys.AsParallel()
                               where c.StartsWith(checkInner)
                               select c).ToList();

                toCheck.ForEach(el =>
                {
                    var entity = MapScreenEntities[el];
                    entity.UpdateDynamicMapForElement(map, el);
                });
#endif
            }
        }

        Dictionary<String, Dictionary<String, IEnumerable<OPCUAEntityReference>>> mapCacheDynamicMap;
        public Dictionary<String, IEnumerable<OPCUAEntityReference>> GetDynamicMapForElementAndChilds(FrameworkElement element, String name, 
                                        bool bChild = true)
        {
            lock (lockObject)
            {
                if (InRuntime)
                {
                    if (mapCacheDynamicMap == null)
                        mapCacheDynamicMap = new Dictionary<string, Dictionary<string, IEnumerable<OPCUAEntityReference>>>();
                    if (mapCacheDynamicMap.ContainsKey(name))
                        return mapCacheDynamicMap[name];
                }
                var map = new Dictionary<String, IEnumerable<OPCUAEntityReference>>();
                if (MapScreenEntities.ContainsKey(name))
                {
                    var entity = MapScreenEntities[name];
                    var listDynamic = (from p in entity.GetAllSourceEntityReferences()
#if !WINDOWS_UWP
                                       where p.IsValid 
#endif
                                       select p).ToList();
                    if (listDynamic.Count() > 0)
                        map.Add(name, listDynamic);
                }

#if !WINDOWS_UWP
                if (bChild && !IsInnerEntity(name))
                {
                    var checkInner = String.Format("{0}{1}", name, innerEntityNameFormat);
                    var toCheck = (from c in MapScreenEntities.Keys.AsParallel()
                                   where c.StartsWith(checkInner)
                                   select c).ToList();

                    toCheck.ForEach(el =>
                    {
                        var entity = MapScreenEntities[el];
                        var listDynamic = (from p in entity.GetAllSourceEntityReferences() where p.IsValid select p).ToList();
                        if (listDynamic.Count() > 0)
                            map.Add(el, listDynamic);
                    });
                }
#endif

                if (InRuntime)
                {
                    if (mapCacheDynamicMap.ContainsKey(name))
                        mapCacheDynamicMap.Remove(name);
                    mapCacheDynamicMap.Add(name, map);
                }
                return map;
            }
        }
#endif

        public List<String> GetDynamicEntriesAndInners(String name)
        {
            var ret = new List<String>();

#if !WINDOWS_UWP && !NET_STANDARD
            var checkInner = String.Format("{0}{1}", name, innerEntityNameFormat);
            var toCheck = (from c in MapScreenEntities.Keys.AsParallel()
                           where c.StartsWith(checkInner)
                           select c).ToList();
            toCheck.ForEach(inner =>
                {
                    if (MapScreenEntities[inner].IsDynamicEntity())
                        ret.Add(inner);
                });
#endif
            if (MapScreenEntities.ContainsKey(name) && MapScreenEntities[name].IsDynamicEntity())
                ret.Add(name);

            return ret;
        }

        public Dictionary<String, IEnumerable<OPCUAEntityReference>> GetDynamicMapForElementAndChilds()
        {
            lock (lockObject)
            {
                var map = new Dictionary<String, IEnumerable<OPCUAEntityReference>>();
                foreach (var key in MapScreenEntities.Keys)
                {
                    var entity = MapScreenEntities[key];
                    var listDynamic = (from p in entity.GetAllSourceEntityReferences()/*.AsParallel()*/
#if !WINDOWS_UWP && !NET_STANDARD
                                       where p.IsValid 
#endif
                                       select p).ToList();
                    if (listDynamic.Count() > 0)
                        map.Add(key, listDynamic);
                }

                return map;
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD

        public class ConnectionSettings
        {
            public string EventDefaultConnection { get; set; }
            public string HistorianDefaultConnection { get; set; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveDeadEntities(Canvas activeLayer)
        {
            var listDeadEntities = new List<String>();
            foreach (var name in MapScreenEntities.Keys)
            {
                UIElement uie = FindInnerControl(activeLayer, name);
                if (uie == null)
                {
                    var parent = FindParentInnerControl(activeLayer, name);
                    if (parent == null || !IsInnerEntity(name))
                    {
                        listDeadEntities.AddRange(GetInnerEntityList(name));
                        listDeadEntities.Add(name);
                    }
                    continue;
                }
                var entity = MapScreenEntities[name];
                entity.Entity = uie;
                entity.Document = this;
            }
            listDeadEntities.ForEach(name => MapScreenEntities.Remove(name));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void GetDynamicMapDetailsForElementAndChilds(CRMapsHeler cRMapsHeler, UFInterfaces.Editors.CrossReferenceModel model, bool resolveEntities = false)
        {
            lock (lockObject)
            {
                if (mapScriptVariableUsed == null)
                    mapScriptVariableUsed = new Dictionary<string, List<string>>();

                var map = new Dictionary<String, IEnumerable<object>>();
                if (cRMapsHeler == null)
                    return;
                bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
                bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
                bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
                bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Screens);
                if (!getTexts && !getTags && !getConnections && !getScreens)
                    return;
                IUFUAEditorManager uFUAEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                var historianDefaultConnection = getConnections ? uFUAEditorManager?.GetHistorianDefaultConnection(this) : string.Empty;
                var eventDefaultConnection = getConnections ? uFUAEditorManager?.GetEventDefaultConnection(this) : string.Empty;

                Canvas activeLayer = null;
                Dictionary<string, object> scriptTagMap = model.ScriptTagMap;
                CancellationToken quitEvent = model.QuitEvent;

                ConnectionSettings connectionSettings = new ConnectionSettings()
                {
                    EventDefaultConnection = eventDefaultConnection,
                    HistorianDefaultConnection = historianDefaultConnection,
                };

                {
                    if (getTexts && cRMapsHeler.UpdateStrings && activeLayer == null)
                    {
                        activeLayer = GetCurrentXamlDocument();
                        SetScreenDocument(activeLayer, this);
                        LoadResources(activeLayer);
                        RemoveDeadEntities(activeLayer);
                        RefreshEntityStyleBinding(activeLayer, true);
                    }

                    foreach (var key in MapScreenEntities.Keys)
                    {

                        var entity = MapScreenEntities[key];
                        if (entity == null)
                            continue;

                        MapScreenEntities[key].Document = this;

                        if(getTexts && cRMapsHeler.UpdateStrings)
                        {
                            var listDynamic = GetAllSourceScreenStringDetails(key, quitEvent, activeLayer, cRMapsHeler: cRMapsHeler)?.ToList();
                            if (listDynamic != null && listDynamic.Count() > 0)
                                cRMapsHeler.StringIDs.Add(CleanInnerName(key), listDynamic);
                        }
                        if (getConnections)
                        {
                            var listDynamic = GetAllSourceScreenConnectionDetails(connectionSettings, key, quitEvent, cRMapsHeler: cRMapsHeler)?.ToList();
                            if (listDynamic != null && listDynamic.Count() > 0)
                                cRMapsHeler.ConnectionString.Add(CleanInnerName(key), listDynamic);
                        }
                        if(getScreens)
                        {
                            var listDynamic = entity.GetAllSourceScreenCommandDetails(quitEvent)?.ToList();
                            if (listDynamic != null && listDynamic.Count() > 0)
                                cRMapsHeler.ScreenLinks.Add(CleanInnerName(key), listDynamic);
                        }
                        if (getTags)
                        {
                            var listDynamic = GetAllSourceEntityReferencesDetails(key, entity, scriptTagMap, quitEvent)?.ToList();
                            if (listDynamic != null && listDynamic.Count() > 0)
                                cRMapsHeler.TagInScreenEntities.Add(CleanInnerName(key), listDynamic);
                            if (!string.IsNullOrEmpty(entity.Code) && bRecalculateListScriptVariableUsed)
                            {
                                var listSDynamic = GetScriptEntityReferences(key, cRMapsHeler, scriptTagMap, quitEvent)?.ToList();
                                if (listSDynamic != null && listSDynamic.Count() > 0)
                                {
                                    if (cRMapsHeler.TagInScreenEntities.ContainsKey(key))
                                        cRMapsHeler.TagInScreenEntities[CleanInnerName(key)].Append(listSDynamic);
                                    else
                                        cRMapsHeler.TagInScreenEntities.Add(CleanInnerName(key), listSDynamic);
                                }
                            }
                        }
                    };
                    if (getTags)
                    {
                        var listScreenDynamic = GetAllSourceEntityReferencesDetails(scriptTagMap: scriptTagMap, quitEvent: quitEvent, cRMapsHeler: cRMapsHeler)?.ToList();
                        if (listScreenDynamic != null && listScreenDynamic.Count() > 0)
                            cRMapsHeler.TagInScreen.Add("#", listScreenDynamic);
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<dynamic> GetAllSourceScreenStringDetails(string name, CancellationToken quitEvent, Canvas activeLayer, CRMapsHeler cRMapsHeler = null)
        {
            var entity = MapScreenEntities.ContainsKey(name) ? mapScreenEntities[name] : null;
            if (entity != null && entity.Entity != null)
            {
                var _fe = entity.Entity as FrameworkElement;
                UpdateProblematicXamlWriterProperties(_fe, name /*entity.EntityName*/, false);
                if (entity.SourceSymbolLinked && _fe is ContentControl && !(_fe is UserControl) && (_fe as ContentControl).Content is FrameworkElement)
                    _fe = (_fe as ContentControl).Content as FrameworkElement;

                if ((_fe is ListBox || _fe is ComboBox) && entity.IsItemSourceEntity())
                {
                    (_fe as ItemsControl).ItemsSource = entity.GetItemSources();
                    int i = 1;
                    foreach(var item in (_fe as ItemsControl).Items)
                    {
                        string txt = null;
                        var propertyInfo = item.GetType().GetProperty("OptionContent");
                        if (propertyInfo != null)
                            txt = propertyInfo.GetValue(item, null).ToString();
                        else
                            txt = item as String;

                        if (!string.IsNullOrEmpty(txt))
                        {
                            var dictionary = new Dictionary<string, string>();
                            if ((_fe as ItemsControl).Items.Count == 1)
                                dictionary[string.Format(Properties.Resources.ListItemSources, "")] = txt;
                            else
                                dictionary[string.Format(Properties.Resources.ListItemSources,i)] = txt;
                            i++;
                            yield return dictionary;
                        }
                    }
                }

                if (_fe is UserControl)
                {
#if !WINDOWS_UWP
                    if ((_fe as UserControl).ToolTip is string && !string.IsNullOrEmpty((_fe as UserControl).ToolTip as string))
                    {
                        var dictionary = new Dictionary<string, string>();
                        dictionary[$"{Properties.Resources.ScreenEntityStringTooltip}"] = (_fe as UserControl).ToolTip as string;
                        yield return dictionary;
                    }
#endif
                    if ((_fe as UserControl).Content is string && !string.IsNullOrEmpty((_fe as UserControl).Content as string))
                    {
                        var dictionary = new Dictionary<string, string>();
                        dictionary[$"{Properties.Resources.ScreenEntityStringContent}"] = (_fe as UserControl).Content as string;
                        yield return dictionary;
                    }
                }
                else
                {
                    var listControlCulture = GetDynControlsList(_fe);

                    if (listControlCulture.Count > 0)
                        foreach (var _control in listControlCulture)
                        {
                            var fe = _control as FrameworkElement;
                            TranslateElement(_control, new Dictionary<string, string>(), true);
                            var keys = mapIsTranslatable.Keys.ToList();
                            if (mapIsTranslatable.ContainsKey(_control))
                            {
                                if (mapControlId.ContainsKey(_control))
                                {
                                    string text = mapControlId[_control].untranslated;
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        var dictionary = new Dictionary<string, string>();
                                        dictionary[$"{Properties.Resources.ScreenEntityStringContent}"] = text;
                                        yield return dictionary;
                                    }

                                }
#if !WINDOWS_UWP
                                if (mapControlTooltip.ContainsKey(_control))
                                {
                                    string text = mapControlTooltip[_control].untranslated;
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        var dictionary = new Dictionary<string, string>();
                                        dictionary[$"{Properties.Resources.ScreenEntityStringTooltip}"] = text;
                                        yield return dictionary;
                                    }

                                }

                                if (mapControlListId.ContainsKey(_control))
                                {
                                    string text = mapControlListId[_control].untranslated;
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        var dictionary = new Dictionary<string, string>();
                                        dictionary[$"{Properties.Resources.ScreenEntityStringContentList}"] = text;
                                        yield return dictionary;
                                    }

                                }
#endif
                            }
                        }
                }
                if (!string.IsNullOrEmpty(entity.SpeechCommand))
                {
                    var dictionary = new Dictionary<string, string>();
                    dictionary[$"{Properties.Resources.ScreenEntitySpeechCommand}"] = entity.SpeechCommand;
                    yield return dictionary;
                }
                if (_fe is IStringIDAware)
                {
                    var stringMap = (_fe as IStringIDAware).GetPropertyToStringIDMap();
                    foreach (var key in stringMap.Keys)
                    {
                        var dictionary = new Dictionary<string, string>();
                        dictionary[$"{key}"] = stringMap[key];
                        yield return dictionary;
                    }
                }
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<dynamic> GetAllSourceScreenConnectionDetails(ConnectionSettings connectionSettings, string name, CancellationToken quitEvent, bool resolveEntities = false, CRMapsHeler cRMapsHeler = null)
        {
            
            var entity = MapScreenEntities.ContainsKey(name) ? MapScreenEntities[name] : null;
            if (entity != null && entity.Entity == null && !string.IsNullOrEmpty(entity.ProblematicXaml))
            {
                UIElement uie = null;
                try
                {
                    uie = entity.ProblematicXaml.ReadUIElement();
                }
                catch (Exception ex)
                {
                    cRMapsHeler?.ErrorMessages.Add($"{Title}: {ex.Message}");
                }
                if (uie != null)
                {
                    entity.Entity = uie;
                    entity.Document = this;
                }
            }

            if (entity?.Entity != null)
            {
                var fe = entity.Entity as FrameworkElement;
                UpdateProblematicXamlWriterProperties(fe, name /*entity.EntityName*/);
                if (entity.Entity is IConnectionAware)
                {
                    var connectionAware = entity.Entity as IConnectionAware;
                    string connection = connectionAware.GetConnectionString();
                    bool useDefault = false;
                    if (string.IsNullOrEmpty(connection))
                    {
                        var connectionStringBind = fe.Resources[Properties.Settings.Default.ConnectionStringBindTag];
                        var eventConnectionStringBind = fe.Resources[Properties.Settings.Default.EventConnectionStringBindTag];

                        useDefault = true;
                        connection = connectionStringBind != null ? connectionSettings.HistorianDefaultConnection : 
                            eventConnectionStringBind != null  ? connectionSettings.EventDefaultConnection : null;
                    }

                    if (!string.IsNullOrEmpty(connection))
                    {
                        var dictionary = new Dictionary<string, string>();
                        dictionary[string.Format("{0}\\{1}", Properties.Resources.ScreenEntityConnectionHeader, useDefault ? name + "@UseDefault": name)] = connection;
                        yield return dictionary;
                    }
                }
            }

            if (entity?.ReaderItemSources != null)
            {
                string connection = String.Format("DataProvider={0};{1}", entity.ReaderItemSources.DataProvider, entity.ReaderItemSources.Connection);
                if (!string.IsNullOrEmpty(connection))
                {
                    var dictionary = new Dictionary<string, string>();
                    dictionary[string.Format("{0}\\{1}", Properties.Resources.ScreenEntityConnectionHeader, name)] = connection;
                    yield return dictionary;
                }
            }
        }

        internal Dictionary<String, OPCUAEntityReference> GetListUsedVariables(string key = null, ScreenEntity sentity = null, CRMapsHeler cRMapsHeler = null, bool skipDataService = false)
        {
            var service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

            if (service == null)
                return null;

            string code = null;
            string caption = null;
            IList listVariables = null;

            if (sentity != null)
            {
                FrameworkElement fe = sentity.Entity as FrameworkElement;
                if (fe != null)
                {
                    fe.Dispatcher.InvokeIfRequired(() =>
                    {
                        code = sentity.Code;
                        caption = sentity.EntityName;
	                    if (!String.IsNullOrEmpty(code))
                        {
                            listVariables = sentity.GetReferenceList(cRMapsHeler);
                        }
                    });
                }
            }
            else
            {
                code = Code;
                caption = Title;
            }

            if (String.IsNullOrEmpty(code))
                return null;

            if (listVariables == null)
                listVariables = GetReferenceList(cRMapsHeler);
            var mapResolvedVariables = new Dictionary<String, OPCUAEntityReference>();

            bDontRaiseSecondError = false;
            var basicCtl = new BasicNoUIObj();

#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
            basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicCtl.Initialize();
            basicCtl.Caption = caption;
            basicCtl.LargeIcon = null;
            basicCtl.SmallIcon = null;
            basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;

            if (bIsBlindServer)
                Util.IgnoreDialogs = true;

            basicCtl.AttachToWindow(null, ManageConstants.Disconnecting);

            var error = String.Empty;
            basicCtl.ErrorAlert += (o, e) =>
            {
                error = String.Format(Properties.Resources.ScriptCheckError, key != null ? key : Title, Path.GetFileNameWithoutExtension(FilePath), basicCtl.Error.Description);
            };
            basicCtl.DoEvents += (o, e) =>
            {
                if (bWindowCreated)
                    WaitForPriority.DoEvents();
            };
            basicCtl.ReadMacro += (o, e) =>
            {
                if (e.FileName.StartsWith("*"))
                {
                    var filename = e.FileName.Replace("*", "");
                    var scriptManager = GetService(typeof(IScriptManager)) as IScriptManager;
                    if (scriptManager != null)
                    {
                        e.Code = scriptManager.GetScriptCode(this, filename);
                        e.Changed = true;
                        e.Cancel = false;
                    };
                }
            };

            var opcua = typeof(Opc.Ua.DataValue).Assembly;
            basicCtl.AddExtension("#", opcua);

            basicCtl.AddExtension("$Feature ExtensionCache False", null);

            foreach (var reference in listVariables)
            {
                var referenceGetTypeAssembly = reference.GetType().Assembly;
                var referenceName =  GetReferenceName(reference);

                if (sentity != null)
                {
                    FrameworkElement fe = sentity.Entity as FrameworkElement;
                    if (fe != null)
                    {
                        fe.Dispatcher.InvokeIfRequired(() =>
                        {
                            referenceName = sentity.GetReferenceName(reference);
                        });
                    }
                }

                if (referenceName.StartsWith("%"))
                    basicCtl.AddExtension(referenceName, reference);
                else
                {
                    basicCtl.AddExtension("#", referenceGetTypeAssembly);
                    basicCtl.AddExtensionObjectWithEvents(referenceName, reference);
                }

                if (reference is VariableValues)
                {
                    var variableValue = reference as VariableValues;
                    variableValue.ForceResolveVariables();

                    var name = variableValue.GetName();
                    if (String.IsNullOrEmpty(name))
                        basicCtl.AddExtension("%", variableValue);
                    else
                        basicCtl.AddExtension(String.Format("%{0}.", name), variableValue);

                    variableValue.ResolveVariable += (o, e) =>
                    {
                        if (skipDataService && variableValue.isDataService)
                            return;
                            
                        var instanceName = e.Name;
                        if (!String.IsNullOrEmpty(variableValue.GetName()))
                            instanceName = String.Format("{0}-{1}", variableValue.GetName(), e.Name);

                        if (!mapResolvedVariables.ContainsKey(instanceName))
                        {
                            if (variableValue.isDataService)
                            {
                                var data = OPCUAEntityReference.GetDataSinkInterface(variableValue.GetName());
                                if (data != null)
                                {
                                    string _name = e.Name.Replace('\\', '&');
                                    var entity = data.GetReference(_name);
                                    if (entity != null)
                                        mapResolvedVariables.Add(instanceName, entity);
                                    else
                                        throw new Exception("Tag not found !");
                                }
                            }
                            else
                            {
                                var nameToCheck = e.Name;
                                var erString = service.GetTagEntityReference(this, e.Name, variableValue.GetName(), inExecution, useCachedUow: true);
                                if (erString != null)
                                {
                                    var entityReference = erString.FromXml<OPCUAEntityReference>();
                                    if (entityReference != null)
                                        mapResolvedVariables.Add(instanceName, entityReference);
                                    else
                                        throw new Exception("Tag not found !");
                                }
                                else
                                {
                                    if (nameToCheck.EndsWith(qualityTagName))
                                    {
                                        var nameNoQuality = nameToCheck.Substring(0, nameToCheck.Length - qualityTagName.Length);
                                        if (!mapResolvedVariables.ContainsKey(nameNoQuality))
                                        {
                                            erString = service.GetTagEntityReference(this, nameNoQuality, variableValue.GetName(), inExecution, useCachedUow: true);
                                            if (erString != null)
                                            {
                                                var entityReference = erString.FromXml<OPCUAEntityReference>();
                                                if (entityReference != null)
                                                    mapResolvedVariables.Add(nameNoQuality, entityReference);
                                                else
                                                    throw new Exception("Tag not found !");
                                            }
                                        }
                                    }
                                    else if (nameToCheck.EndsWith(timestampTagName))
                                    {
                                        var nameNoTimestamp = e.Name.Substring(0, nameToCheck.Length - timestampTagName.Length);
                                        if (!mapResolvedVariables.ContainsKey(nameNoTimestamp))
                                        {
                                            erString = service.GetTagEntityReference(this, nameNoTimestamp, variableValue.GetName(), inExecution, useCachedUow: true);
                                            if (erString != null)
                                            {
                                                var entityReference = erString.FromXml<OPCUAEntityReference>();
                                                if (entityReference != null)
                                                    mapResolvedVariables.Add(nameNoTimestamp, entityReference);
                                                else
                                                    throw new Exception("Tag not found !");
                                            }
                                        }
                                    }
                                    else
                                        throw new Exception("Tag not found !");
                                }
                            }
                        }
                    };
                }
            }
            basicCtl.ReadMacro += (o, e) =>
           {
             if (e.FileName.StartsWith("*"))
             {
                var filename = e.FileName.Replace("*", "");
                var scriptManager = GetService(typeof(IScriptManager)) as IScriptManager;
                if (scriptManager != null)
                {
                    e.Code = scriptManager.GetScriptCode(this, filename);
                    e.Changed = true;
                    e.Cancel = false;
                };
            }
          };

            basicCtl.FileTools = false;
            basicCtl.EventMode = true;
            basicCtl.Code = code;
            basicCtl.Changed = false;

            var check = basicCtl.SyntaxCheck();

            while (basicCtl.Shutdown() < 0)
                WaitForPriority.DoEvents();

            basicCtl.Disconnect();

            if (!check || !String.IsNullOrEmpty(error))
                return null;

            return mapResolvedVariables;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<dynamic> GetAllSourceEntityReferencesDetails(string key = null, ScreenEntity entity = null, Dictionary<string, object> scriptTagMap = null, CancellationToken? quitEvent = null, CRMapsHeler cRMapsHeler = null)
        {
            if (scriptTagMap == null)
                scriptTagMap = new Dictionary<string, object>();
            {
                List<string> listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                IUFUAEditorManager service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (entity != null)
                {
                    if (entity.OpcuaEntityReference != null)
                    {
                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                        dictionary[Properties.Resources.ItemTagHeader] = entity.OpcuaEntityReference;
                        yield return dictionary;
                    }

                    if (entity.OpcuaEntityReference3DZoom != null)
                    {
                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                        dictionary[Properties.Resources.Item3DZoomHeader] = entity.OpcuaEntityReference3DZoom;
                        yield return dictionary;
                    }

                    if (!string.IsNullOrEmpty(entity.Code) && mapScriptVariableUsed != null && service != null)
                    {
                        List<string> map = null;
                        bool bExist = false;
                        OPCUAEntityReference tag = null;
                        if (!bRecalculateListScriptVariableUsed)
                        {
                            if (mapScriptVariableUsed.ContainsKey(key)) map = mapScriptVariableUsed[key];
                            if (map != null)
                            {
                                var listReference = map;
                                for (int i = 0; i < listReference.Count(); i++)
                                {
                                    var reference = listReference[i];
                                    lock (lockObject)
                                    {
                                        bExist = scriptTagMap.ContainsKey(reference);
                                        if (bExist)
                                            tag = (OPCUAEntityReference)scriptTagMap[reference];
                                    }
                                    if (bExist)
                                    {
                                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                        dictionary[string.Format("{0}", Properties.Resources.ItemScriptHeader)] = tag;
                                        yield return dictionary;
                                    }
                                    else
                                    {
                                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                        var split = reference.Split('-');
                                        var instance = split[0];
                                        var name = split[0];
                                        if (split.Length > 1)
                                            name = split[1];
                                        else
                                            instance = null;
                                        if (listDataSinkInterfaces.Contains(instance))
                                        {
                                            var datasync = OPCUAEntityReference.GetDataSinkInterface(instance);
                                            tag = datasync?.GetReference(name);
                                        }
                                        else
                                        {
                                            var xml = service.GetTagEntityReference(this, name, instance, useCachedUow: true);
                                            if (!String.IsNullOrEmpty(xml))
                                                tag = xml.FromXml<OPCUAEntityReference>();
                                        }
                                        if (tag != null)
                                        {
                                            lock (lockObject)
                                            {
                                                if (!scriptTagMap.ContainsKey(reference))
                                                    scriptTagMap.Add(reference, tag);
                                            }
                                            dictionary[string.Format("{0}", Properties.Resources.ItemScriptHeader)] = tag;
                                            yield return dictionary;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var listAnimation = entity.AnimationList as AnimationManagerList;
                    foreach (var animation in listAnimation)
                    {
                        if (!string.IsNullOrEmpty(animation.Expression))
                        {
                            List<string> expressionTags = ExpressionValueConverterHelper.GetListVarInExpression(animation.Expression);
                            OPCUAEntityReference tag = null;
                            for (int i = 0; i < expressionTags.Count; i++)
                            {
                                var reference = expressionTags[i];
                                var dictionary = GetReferenceTag(listDataSinkInterfaces, reference,
                                    $"{Properties.Resources.AnimationHeader}\\{animation.Name} - {Properties.Resources.ItemsExpressionTagHeader} {i}", scriptTagMap, service);
                                if (dictionary != null)
                                    yield return dictionary;
                            }
                        }

                        if (animation.OpcuaEntityReference != null)
                        {
                            var dictionary = new Dictionary<string, OPCUAEntityReference>();
                            dictionary[string.Format("{0}\\{1}", Properties.Resources.AnimationHeader, animation.Name)] = animation.OpcuaEntityReference;
                            yield return dictionary;
                        }
                    }

                    var listCommand = entity.CommandList as CommandManagerList;
                    foreach (var command in listCommand)
                    {
                        if (!string.IsNullOrEmpty(command.Expression))
                        {
                            List<string> expressionTags = ExpressionValueConverterHelper.GetListVarInExpression(command.Expression);
                            OPCUAEntityReference tag = null;
                            for (int i = 0; i < expressionTags.Count; i++)
                            {
                                var reference = expressionTags[i];
                                var dictionary = GetReferenceTag(listDataSinkInterfaces, reference,
                                    $"{Properties.Resources.CommandTagHeader}\\{command.Name} - {Properties.Resources.ItemsExpressionTagHeader} {i}", scriptTagMap, service);
                                if (dictionary != null)
                                    yield return dictionary;
                            }
                        }
                        var list = command.ListTags;
                        foreach(var tag in list)
                        {
                            var dictionary = new Dictionary<string, OPCUAEntityReference>();
                            dictionary[string.Format("{0}\\{1}", Properties.Resources.CommandTagHeader, command.Name)] = tag;
                            yield return dictionary;
                        }
                    }

                    if(entity.MapHashInner3DEntities != null && entity.MapHashInner3DEntities.Count > 0)
                    {
                        foreach (var entry in entity.MapHashInner3DEntities.Keys)
                        {
                            var entityRefList = GetAllSourceEntityReferencesDetails(entry, entity.MapHashInner3DEntities[entry], scriptTagMap, quitEvent,cRMapsHeler);
                            foreach (var tagValuePair in entityRefList)
                            {
                                var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                var _3DRef = tagValuePair as Dictionary<string, OPCUAEntityReference>;
                                if(_3DRef != null && _3DRef.Count > 0)
                                {
                                    dictionary[string.Format("{0} - {1}", _3DRef.Keys.FirstOrDefault(), entry)] = _3DRef.Values.FirstOrDefault();
                                    yield return dictionary;
                                }
                            }
                        }
                    }

                    if (entity.MapProblematicXamlWriterProperties != null)
                    {
                        var opcProp = typeof(OPCUAEntityReference).Name;
                        var opcXMLProp = typeof(OPCUAXMLEntityReference).Name;
                        var attributes = typeof(OPCUAEntityReference).GetCustomAttributes(typeof(DataContractAttribute), false);
                        DataContractAttribute dataContractAttribute = null;
                        if (attributes.Count() > 0)
                            dataContractAttribute = attributes[0] as DataContractAttribute;
                        var opcDataContractProp = dataContractAttribute?.Name;
                        List<string> listofkeys = null;

                        if(!string.IsNullOrEmpty(opcDataContractProp))
                            listofkeys = (from c in MapScreenEntities[key].MapProblematicXamlWriterProperties//.AsParallel()
                                              where !String.IsNullOrEmpty(c.Value) && 
                                              (c.Value.Contains(opcProp) ||
                                          c.Value.Contains(opcXMLProp) ||
                                          c.Value.Contains(opcDataContractProp))
#if !WINDOWS_UWP
                                          && IsValidPropertyXamlWriter(c.Key)
#endif
                                          select c.Key).ToList();
                        else

                            listofkeys = (from c in MapScreenEntities[key].MapProblematicXamlWriterProperties//.AsParallel()
                                          where !String.IsNullOrEmpty(c.Value) &&
                                          (c.Value.Contains(opcProp) ||
                                          c.Value.Contains(opcXMLProp))
#if !WINDOWS_UWP
                                          && IsValidPropertyXamlWriter(c.Key)
#endif
                                          select c.Key).ToList();
                        try
                        {
                            foreach (var propName in listofkeys)
                            {
                                var valueString = entity.MapProblematicXamlWriterProperties[propName];
                                if (!String.IsNullOrEmpty(valueString))
                                {
                                    object obj = null;
                                    try
                                    {
                                        using (var reader = new StringReader(valueString))
                                        {
                                            using (var textReader = new XmlTextReader(reader))
                                            {
                                                obj = XamlReader.Load(textReader);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        cRMapsHeler?.ErrorMessages.Add($"{Title}: {ex.Message}");
                                    }
                                    if (obj != null)
                                    {
                                        if (obj is OPCUAXMLEntityReference)
                                        {
                                            var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                            dictionary[string.Format("{0}\\{1}", Properties.Resources.DynamicTagHeader, propName)] = (obj as OPCUAXMLEntityReference).TagReference;
                                            yield return dictionary;
                                        }
                                        else if (obj is OPCUAEntityReference)
                                        {
                                            var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                            dictionary[string.Format("{0}\\{1}", Properties.Resources.DynamicTagHeader, propName)] = (obj as OPCUAEntityReference);
                                            yield return dictionary;
                                        }
                                        else if (obj is IList)
                                        {
                                            var objlist = (obj as IList);
                                            for (int i = 0; i < objlist.Count; i++)
                                            {
                                                var o = objlist[i];
                                                var opcProperties = (from p in o.GetType().GetProperties() where p.PropertyType == typeof(OPCUAViewModel.OPCUAEntityReference) 
                                                                     && (
                                                                        !Attribute.IsDefined(p, typeof(BrowsableAttribute)) ||
                                                                        (p.GetCustomAttributes(typeof(BrowsableAttribute), true).FirstOrDefault() as BrowsableAttribute).Browsable
                                                                        )
                                                                        select p).ToList();
                                                var opcXMLProperties = (from p in o.GetType().GetProperties() where p.PropertyType == typeof(OPCUAViewModel.OPCUAXMLEntityReference)
                                                                     && (
                                                                        !Attribute.IsDefined(p, typeof(BrowsableAttribute)) ||
                                                                        (p.GetCustomAttributes(typeof(BrowsableAttribute), true).FirstOrDefault() as BrowsableAttribute).Browsable
                                                                        )
                                                                        select p).ToList();
                                                foreach (var prop in opcProperties)
                                                {
                                                    var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                                    dictionary[string.Format("{0}\\{1}", Properties.Resources.DynamicTagHeader, propName)] = (prop.GetValue(o) as OPCUAEntityReference);
                                                    yield return dictionary;
                                                };
                                                foreach (var prop in opcXMLProperties)
                                                {
                                                    var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                                    dictionary[string.Format("{0}\\{1}", Properties.Resources.DynamicTagHeader, propName)] = (prop.GetValue(o) as OPCUAXMLEntityReference)?.TagReference;
                                                    yield return dictionary;
                                                };
                                            }
                                        }
                                    }
                                }
                            };
                        }
                        finally
                        {
                        }
                    }

                    if(!string.IsNullOrEmpty(entity.Expression))
                    {
                        List<string> expressionTags = ExpressionValueConverterHelper.GetListVarInExpression(entity.Expression);
                        List<string> map = null;
                        OPCUAEntityReference tag = null;
                        for (int i = 0; i < expressionTags.Count; i++)
                        {
                            var reference = expressionTags[i];
                            var dictionary = GetReferenceTag(listDataSinkInterfaces, reference, $"{Properties.Resources.ItemsExpressionTagHeader} {i}", scriptTagMap, service);
                            if (dictionary != null)
                                yield return dictionary;
                        }
                    }
                    if (!string.IsNullOrEmpty(entity.ReverseExpression))
                    {
                        List<string> expressionTags = ExpressionValueConverterHelper.GetListVarInExpression(entity.ReverseExpression);
                        List<string> map = null;
                        OPCUAEntityReference tag = null;
                        for (int i = 0; i < expressionTags.Count; i++)
                        {
                            var reference = expressionTags[i];
                            var dictionary = GetReferenceTag(listDataSinkInterfaces, reference, $"{Properties.Resources.ItemsReverseExpressionTagHeader} {i}", scriptTagMap, service);
                            if (dictionary != null)
                                yield return dictionary;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sCode) && mapScriptVariableUsed != null && service != null)
                    {
                        List<string> map = null;
                        bool bExist = false;
                        OPCUAEntityReference tag = null;
                        if (!bRecalculateListScriptVariableUsed)
                        {
                            if (mapScriptVariableUsed.ContainsKey(innerEntityNameFormat)) map = mapScriptVariableUsed[innerEntityNameFormat];
                            if (map != null)
                            {
                                var listReference = map;
                                for (int i = 0; i < listReference.Count(); i++)
                                {
                                    var reference = listReference[i];
                                    lock (lockObject)
                                    {
                                        bExist = scriptTagMap.ContainsKey(reference);
                                        if (bExist)
                                            tag = (OPCUAEntityReference)scriptTagMap[reference];
                                    }
                                    if (bExist)
                                    {
                                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                        dictionary[string.Format("{0}", Properties.Resources.ScreenScriptHeader)] = tag;
                                        yield return dictionary;
                                    }
                                    else
                                    {
                                        var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                        var split = reference.Split('-');
                                        var instance = split[0];
                                        var name = split[0];
                                        if (split.Length > 1)
                                            name = split[1];
                                        else
                                            instance = null;
                                        if (listDataSinkInterfaces.Contains(instance))
                                        {
                                            var datasync = OPCUAEntityReference.GetDataSinkInterface(instance);
                                            tag = datasync?.GetReference(name);
                                        }
                                        else
                                        {
                                            var xml = service.GetTagEntityReference(this, name, instance, useCachedUow: true);
                                            if (!String.IsNullOrEmpty(xml))
                                                tag = xml.FromXml<OPCUAEntityReference>();
                                        }
                                        if (tag != null)
                                        {
                                            lock (lockObject)
                                            {
                                                if (!scriptTagMap.ContainsKey(reference))
                                                    scriptTagMap.Add(reference, tag);
                                            }
                                            dictionary[string.Format("{0}", Properties.Resources.ScreenScriptHeader)] = tag;
                                            yield return dictionary;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var smap = GetListUsedVariables(cRMapsHeler: cRMapsHeler);
                            if (smap != null && smap.Count > 0)
                            {
                                map = smap.Keys.ToList();
                                if (!mapScriptVariableUsed.ContainsKey(innerEntityNameFormat))
                                    mapScriptVariableUsed.Add(innerEntityNameFormat, smap.Keys.ToList());
                                for (int i = 0; i < map.Count(); i++)
                                {
                                    var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                    tag = smap[map[i]];
                                    lock (lockObject)
                                    {
                                        if (!scriptTagMap.ContainsKey(map[i]))
                                            scriptTagMap.Add(map[i], tag);
                                    }
                                    dictionary[string.Format("{0}", Properties.Resources.ScreenScriptHeader)] = tag;
                                    yield return dictionary;
                                }
                            }
                        }
                    }

                }
            }
        }


        OPCUAEntityReference GetReferenceTag(List<string> listDataSinkInterfaces, string reference, Dictionary<string, object> usedTagMap, IUFUAEditorManager service)
        {
            List<string> map = null;
            bool bExist = false;
            OPCUAEntityReference tag = null;
            if (lockObject == null)
                lockObject = new Object();
            lock (lockObject)
            {
                bExist = usedTagMap.ContainsKey(reference);
                if (bExist)
                    tag = (OPCUAEntityReference)usedTagMap[reference];
            }
            if (bExist)
            {
                return tag;
            }
            else
            {
                var split = reference.Split('-');
                var instance = split[0];
                var name = split[0];
                if (split.Length > 1)
                    name = split[1];
                else
                    instance = null;
                if (listDataSinkInterfaces.Contains(instance))
                {
                    var datasync = OPCUAEntityReference.GetDataSinkInterface(instance);
                    tag = datasync?.GetReference(name);
                }
                else
                {
                    var xml = service.GetTagEntityReference(this, name, instance, useCachedUow: true);
                    if (!String.IsNullOrEmpty(xml))
                        tag = xml.FromXml<OPCUAEntityReference>();
                }
                if (tag != null)
                {
                    lock (lockObject)
                    {
                        if (!usedTagMap.ContainsKey(reference))
                            usedTagMap.Add(reference, tag);
                    }
                    return tag;
                }
            }

            return null;

        }

        Dictionary<string, OPCUAEntityReference> GetReferenceTag(List<string> listDataSinkInterfaces, string reference, string itemsHeader, Dictionary<string, object> tagMap, IUFUAEditorManager service)
        {
            OPCUAEntityReference tag = GetReferenceTag(listDataSinkInterfaces, reference, tagMap, service);
            if (tag == null)
                tag = new OPCUAEntityReference() { RelativePath = reference, AppName = service.GetAplicationName(this, true) };
            return new Dictionary<string, OPCUAEntityReference>() { { $"{itemsHeader}", tag } };
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<dynamic> GetScriptEntityReferences(string key = null, CRMapsHeler cRMapsHeler = null, Dictionary<string, object> scriptTagMap = null, CancellationToken? quitEvent = null)
        {
            if (scriptTagMap == null)
                scriptTagMap = new Dictionary<string, object>();
            {
                List<string> listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                IUFUAEditorManager service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (cRMapsHeler != null)
                {
                    var entity = MapScreenEntities.ContainsKey(key) ? MapScreenEntities[key] : null;
                    if (entity != null && !string.IsNullOrEmpty(entity.Code) && mapScriptVariableUsed != null && service != null)
                    {
                        List<string> map = null;
                        OPCUAEntityReference tag = null;
                        if (entity.Entity == null && !string.IsNullOrEmpty(entity.ProblematicXaml))
                        {
                            UIElement uie = null;
                            try
                            {
                                uie = entity.ProblematicXaml.ReadUIElement();
                            }
                            catch (Exception ex)
                            {
                                cRMapsHeler?.ErrorMessages.Add($"{Title}: {ex.Message}");
                            }

                            if (uie != null)
                            {
                                entity.Entity = uie;
                                entity.Document = this;
                            }
                        }
                        var smap = GetListUsedVariables(key, entity, cRMapsHeler);
                        if (smap != null && smap.Count > 0)
                        {
                            map = smap.Keys.ToList();
                            if (!mapScriptVariableUsed.ContainsKey(key))
                                mapScriptVariableUsed.Add(key, smap.Keys.ToList());
                            for (int i = 0; i < map.Count(); i++)
                            {
                                var dictionary = new Dictionary<string, OPCUAEntityReference>();
                                tag = smap[map[i]];
                                lock (lockObject)
                                {
                                    if (!scriptTagMap.ContainsKey(map[i]))
                                        scriptTagMap.Add(map[i], tag);
                                }
                                dictionary[string.Format("{0}", Properties.Resources.ItemScriptHeader)] = tag;
                                yield return dictionary;
                            }
                        }
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScreenDocument CreateSmartTemplateDocument(FrameworkElement element)
        {
            ScreenDocument Document = new ScreenDocument();

            var UFUAEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor != null)
            {
                var map = GetDynamicMapForElementAndChilds(element, element.Name);
                var list = new List<Object>();
                foreach(var pair in map)
                {
                    foreach(var reference in pair.Value)
                    {
                        list.Add(reference);
                    }
                }

                var ret = UFUAEditor.GetVariableListSettingsFlat(this, list);
                Document.variableSettings = ret.Item1;
                Document.localVariableSettings = ret.Item2;
            }

            if (MapScreenEntities.ContainsKey(element.Name))
                Document.MapScreenEntities.Add(element.Name, MapScreenEntities[element.Name].Clone() as ScreenEntity);

            var inners = GetListInners(element.Name);
            inners.ForEach(inner =>
            {
                if (MapScreenEntities.ContainsKey(inner))
                {
                    Document.MapScreenEntities.Add(inner, MapScreenEntities[inner].Clone() as ScreenEntity);
                }
            });

            Document.ListResources = ListResources;
            Document.UpdateResources();
            Document.ListAssemblies = ListAssemblies;
            return Document;
        }

        public IList<String> GetListResourceUsed()
        {
            var listStyles = (from entry in MapScreenEntities.AsParallel()
                              where !String.IsNullOrEmpty(entry.Value.StyleResource)
                              select entry.Key).ToList();
            return listStyles;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateResources()
        {
            //var listStyles = (from entry in MapScreenEntities.AsParallel()
            //                 where !String.IsNullOrEmpty(entry.Value.StyleResource)
            //                 select entry.Key).ToList();

            //var cleanList = new List<String>();
            //ListResources.ForEach(resource =>
            //{
            //    if (listStyles.Count > 0)
            //    {
            //        try
            //        {
            //            ResourceDictionary dict = new ResourceDictionary
            //            {
            //                Source = new Uri(resource, UriKind.RelativeOrAbsolute)
            //            };

            //            foreach (var res in listStyles)
            //            {
            //                if (dict.Contains(res))
            //                {
            //                    cleanList.Add(resource);
            //                    listStyles.Remove(res);
            //                    break;
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //});

            //ListResources = cleanList;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CreateSmartTemplate(FrameworkElement element)
        {
            var inners = GetListInners(element.Name);
            inners.ForEach(inner =>
            {
                if (MapScreenEntities.ContainsKey(inner))
                {
                    var entity = MapScreenEntities[inner];
                    if (entity.Element == null)
                    {
                        UIElement uie = FindInnerControl(element, inner);
                        entity.Entity = uie;
                        entity.Document = this;
                    }
                }
            });

            var map = GetDynamicMapForElementAndChilds(element, element.Name);

            var updatedItems = new Dictionary<String, String>();
            map.Keys.ToList().ForEach(el =>
                {
                    map[el].ToList().ForEach(item =>
                        {
                            if (item.NodeIdViewModel == null/* && item.ParentNodeId == null*/)
                            {
                                item.CreateNodeIdViewModel(SessionString);

                                if (item.NodeIdViewModel == null)
                                {
                                    MessageBox.Show(String.Format(Properties.Resources.CannotConnectToGetParent, el, item.HumanReadable, item.EndpointUrl), 
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }

                            if (item.NodeIdViewModel != null && item.NodeIdViewModel.ParentedTypeId != null)
                            {
                                var oldItem = item.ToXml();

                                item.RelativePath = item.NodeIdViewModel.RelativePath;
                                item.StartingAddress = item.NodeIdViewModel.StartingAddress;
                                item.ParentNodeId = item.NodeIdViewModel.ParentedTypeId;
                                item.ParentTypeDefinitionNodeId = item.NodeIdViewModel.ParentedTypeDefinition;
                                item.ParentTypeDefinitionName = item.NodeIdViewModel.ParentedTypeDefinitionName;
                                item.ParentTypeDefinitionNameList = item.NodeIdViewModel.ParentedTypeDefinitionNameList;
                                item.ParentTypeDefinitionIdList = item.NodeIdViewModel.ParentedTypeDefinitionIdList;

                                item.ResolvedNodeId = null;
                                item.ResolvedStartingNodeId = null;

                                if (!updatedItems.ContainsKey(oldItem))
                                    updatedItems.Add(oldItem, item.ToXml());
                            }
                        });
                });

            UpdateDynamicMapForElementAndChilds(element, element.Name, updatedItems);
        }
#endif
#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, List<OPCUAEntityReference>> GetMapItemsWithTypeDef(FrameworkElement element)
        {
            var map = GetDynamicMapForElementAndChilds(element, element.Name);
            var tobeResolved = new Dictionary<String, List<OPCUAEntityReference>>();
            Parallel.ForEach(map.Keys, el =>
            {
                var list = (from c in map[el].AsParallel() where (!String.IsNullOrEmpty(c.TypeDefinitionName)) select c).ToList();
                if (list.Count > 0)
                {
                    lock (tobeResolved)
                    {
                        tobeResolved.Add(el, list);
                    }
                }
            });

            return tobeResolved;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, List<OPCUAEntityReference>> GetMapItemsToBeResolved(FrameworkElement element, String name, 
                        bool bForce = false, bool bChild = true)
        {
            ScreenEntity entity = null;
            if (InRuntime && MapScreenEntities.ContainsKey(name))
            {
                entity = MapScreenEntities[name];
                if (!bForce && entity.MapItemsToBeResolved != null)
                    return entity.MapItemsToBeResolved;
            }

            var map = GetDynamicMapForElementAndChilds(element, name, bChild);
            var tobeResolved = new Dictionary<String, List<OPCUAEntityReference>>();
            foreach(var el in map.Keys)
            {
                var list = (from c in map[el].AsParallel()
                            where !String.IsNullOrEmpty(c.RelativePath) &&
                            !String.IsNullOrEmpty(c.StartingAddress) &&
                            !String.IsNullOrEmpty(c.ParentTypeDefinitionName) && !c.ResolvedItem
                            select c).ToList();
                if (list.Count > 0)
                {
                    // lock (tobeResolved)
                    {
                        tobeResolved.Add(el, list);
                    }
                }
            }

            if (entity != null && InRuntime)
                entity.MapItemsToBeResolved = tobeResolved;
            return tobeResolved;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, List<OPCUAEntityReference>> GetMapItemsToBeResolved()
        {
            var map = GetDynamicMapForElementAndChilds();
            var tobeResolved = new Dictionary<String, List<OPCUAEntityReference>>();
            Parallel.ForEach(map.Keys, el =>
            {
                var list = (from c in map[el].AsParallel() where (!String.IsNullOrEmpty(c.RelativePath) && !String.IsNullOrEmpty(c.ParentTypeDefinitionName)) select c).ToList();
                if (list.Count > 0)
                {
                    lock (tobeResolved)
                    {
                        tobeResolved.Add(el, list);
                    }
                }
            });

            return tobeResolved;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<String> GetListTypeDefinitionName()
        {
            var map = GetMapItemsToBeResolved();
            var list = (from p in map.Values.AsParallel()
                        let listref = p
                        from c in listref.AsParallel()
                        where (!String.IsNullOrEmpty(c.RelativePath) && !String.IsNullOrEmpty(c.ParentTypeDefinitionName) &&
                                c.ParentTypeDefinitionNameList != null) 
                        let listtype = c.ParentTypeDefinitionNameList
                        from l in listtype.AsParallel()
                        select l).ToList();
            var listnoDuplicates = (list.GroupBy(x => x).Select(y => y.First())).ToList();
            return listnoDuplicates;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<OPCUAEntityReference> GetListTypeDefinition()
        {
            var map = GetMapItemsToBeResolved();
            var list = (from p in map.Values.AsParallel()
                        let listref = p
                        from c in listref.AsParallel()
                        where (!String.IsNullOrEmpty(c.RelativePath) && !String.IsNullOrEmpty(c.ParentTypeDefinitionName))
                        select c).ToList();
            var listnoDuplicates = (list.GroupBy(x => x.ParentTypeDefinitionName).Select(y => y.First())).ToList();
            return listnoDuplicates;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResolveRelativeItems(FrameworkElement element, String name, FrameworkElement owner, bool bChild = true)
        {
            var tobeResolved = GetMapItemsToBeResolved(element, name, false, bChild);
            if (tobeResolved.Count > 0)
            {
                //ProgressDialog dlg = new ProgressDialog();
                //try
                //{
                //    if (ActiveView != null)
                //        dlg.Owner = ActiveView.FindParent<Window>() ?? Application.Current.MainWindow;
                //    else
                //        dlg.Owner = Application.Current.MainWindow;
                //}
                //catch (Exception ex)
                //{

//}
//dlg.ProgressBarIndeterminate = true;
//dlg.DialogText = String.Format("Resolving {0}...", element.Name);
//dlg.IsCancellingEnabled = false;

////start processing and submit the start value
//if (dlg.RunWorkerThread(null, (o, ev) =>
//{
#if !WINDOWS_UWP
                    element.Dispatcher.InvokeIfRequired(() =>
#else
                    RunOnUIThread.RunIfRequired(() =>
#endif
                        {
                            foreach (var el in tobeResolved.Keys)
                            {
                                var nameItem = el;
                                var index = nameItem.LastIndexOf(innerEntityNameFormat);
                                while (index != -1)
                                {
                                    nameItem = nameItem.Substring(0, index);
                                    if (MapScreenEntities.ContainsKey(nameItem))
                                    {
                                        var entity = MapScreenEntities[nameItem];

                                        foreach (var item in tobeResolved[el])
                                        {
                                            bool bFound1 = false;
                                            bool bFound2 = false;
                                            String itemVar = null;

                                            if (!bFound1)
                                            {
                                                if (entity.OpcuaEntityReference != null && entity.Element != null)
                                                {
                                                    var opcuaentityreference = entity.OpcuaEntityReference.ToXml();

                                                    if (String.IsNullOrEmpty(itemVar))
                                                        itemVar = item.ToXml();

                                                    bFound2 = entity.Element.GetChildrenOfType<IDynamicTagAware>().ToList().TrueForAll(dyamicAware => dyamicAware.MatchTypeDefinition(itemVar, opcuaentityreference));
                                                }

                                                if (entity.IsMatchTypeDefinition(item))
                                                {
                                                    bFound1 = true;
                                                    string oldVar = !string.IsNullOrEmpty(item.StartingAddress) ? item.StartingAddress : item.ReadablePath;
                                                    entity.SubstituteExpressionVariable(
                                                            oldVar,
                                                            entity.OpcuaEntityReference.ReadablePath
                                                        );

                                                    GetInnerEntityList(nameItem).ForEach(inner =>
                                                    {
                                                        MapScreenEntities[inner].SubstituteExpressionVariable(
                                                                oldVar,
                                                                entity.OpcuaEntityReference.ReadablePath
                                                            );
                                                    });

                                                    entity.MatchTypeDefinition(item);
                                                }
                                            }

                                            //if (bFound1 && bFound2)
                                            //    break;
                                        }
                                    }

                                    index = nameItem.LastIndexOf(innerEntityNameFormat);
                                }

//                                var parent = FindInnerControl(owner, el);
//                                var savedparent = parent;
//                                foreach (var item in tobeResolved[el])
//                                {
//                                    parent = savedparent;

//                                    bool bFound1 = false;
//                                    bool bFound2 = false;
//                                    String itemVar = null;
//                                    while (parent != null)
//                                    {
//                                        var preparent = parent.FindParent<FrameworkElement>();
//                                        var parentName = parent.Name;
//#if !WINDOWS_UWP
//                                        if (String.IsNullOrEmpty(parentName))
//                                        {
//                                            var n = parent.Uid as String;
//                                            if (!String.IsNullOrEmpty(n))
//                                                parentName = n;
//                                        }
//#endif
//                                        if (!bFound1 && !String.IsNullOrEmpty(parentName) && !(preparent is ContentPresenter) &&
//                                            MapScreenEntities.ContainsKey(parentName))
//                                        {
//                                            if (MapScreenEntities[parentName].OpcuaEntityReference != null)
//                                            {
//                                                var opcuaentityreference = MapScreenEntities[parentName].OpcuaEntityReference.ToXml();

//                                                if (String.IsNullOrEmpty(itemVar))
//                                                    itemVar = item.ToXml();
//                                                foreach (var dyamicAware in parent.GetChildrenOfType<IDynamicTagAware>())
//                                                {
//                                                    if (dyamicAware.MatchTypeDefinition(itemVar, opcuaentityreference))
//                                                    {
//                                                        bFound2 = true;
//                                                    }
//                                                }
//                                            }

//                                            if(MapScreenEntities[parentName].MatchTypeDefinition(item))
//                                                bFound1 = true;
//                                        }


//                                        if (bFound1 && bFound2)
//                                            break;

//                                        parent = preparent;
//                                        if (parent == owner)
//                                            break;
//                                    }

//                                    /*
//                                    if (!bFound)
//                                    {
//                                        parent = el;
//                                        while (parent != null)
//                                        {
//                                            lock (lockObject)
//                                            {
//                                                if (MapScreenEntities.ContainsKey(parent.Name) &&
//                                                    MapScreenEntities[parent.Name].MatchTypeDefinitionInChild(item, UISessionString))
//                                                {
//                                                    bFound = true;
//                                                    break;
//                                                }
//                                            }

//                                            parent = parent.FindParent<FrameworkElement>();
//                                        }
//                                    }
//                                    */
//                                }
                            }
                        });
                //}))
                //{
                //}
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, List<OPCUAEntityReference>> GetTypeDefMapForElementAndChilds(FrameworkElement element)
        {
            var map = GetDynamicMapForElementAndChilds(element, element.Name);
            var ret = new Dictionary<String, List<OPCUAEntityReference>>();

            Parallel.ForEach(map.Keys, el =>
            {
                Parallel.ForEach(map[el], item =>
                {
                    if (item.TypeDefinitionNodeId != null)
                    {
                        lock (ret)
                        {
                            if (!ret.ContainsKey(el))
                                ret.Add(el, new List<OPCUAEntityReference>());
                            ret[el].Add(item);
                        }
                    }
                });
            });

            return ret;
        }
#endif
        public IEnumerable<String> DynamicElements()
        {
            return MapScreenEntities.Keys;
        }
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetFriendObjects(FrameworkElement element, GetFriendObjectsEventArgs e, bool bTestOnly = false)
        {
            if (e.friendList == null)
                e.friendList = new List<ScreenEntity>();

            var found = (from c in MapScreenEntities.Keys where MapScreenEntities[c].Entity == element select c).ToList();
            if (found.Count > 0)
            {
                e.Name = found[0];
                e.friendList.Add(MapScreenEntities[found[0]]);
            }
            else
            {
                var name = e.Name;
                if (String.IsNullOrEmpty(name))
                {
                    name = element.Name;
                    if (String.IsNullOrEmpty(element.Name))
                    {
                        // element.Name = element.DependencyObjectType.Name;
                        name = element.DependencyObjectType.Name;
                    }
                }
                if (!String.IsNullOrEmpty(e.Container))
                {
                    name = String.Format("{0}{1}{2}", e.Container, innerEntityNameFormat, name);
                }

                if (!bTestOnly)
                {
                    AddDynamicEntity(name);
                    MapScreenEntities[name].Entity = element;
                    MapScreenEntities[name].Document = this;

                    e.Name = name;
                    e.friendList.Add(MapScreenEntities[name]);
                }
            }

            return true;
        }
#endif
#if !NET_STANDARD
        public static void CheckPreBinding(List<FrameworkElement> listAlarm,
            List<FrameworkElement> listConnectionString,
            List<FrameworkElement> listEventConnectionString,
            List<FrameworkElement> listScheduler,
            FrameworkElement fe,
            List<FrameworkElement> listRecipe = null)
        {
            var list = fe.GetChildrenOfType<FrameworkElement>().ToList();
            list.Add(fe);
            foreach (var control in list)
            {
                if (listRecipe != null)
                {
                    try
                    {
#if !WINDOWS_UWP
                        var recipeBind = control.Resources[Properties.Settings.Default.RecipeBindTag];
#else
                    var recipeBind = control.Resources["RecipeDataSource"];
#endif
                        if (recipeBind != null)
                            listRecipe.Add(control);
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
#if !WINDOWS_UWP
                    var alarmBind = control.Resources[Properties.Settings.Default.AlarmBindTag];
#else
                    var alarmBind = control.Resources["AlarmDataSource"];
#endif
                    if (alarmBind != null)
                        listAlarm.Add(control);
                }
                catch (Exception)
                {
                } 
                try
                {
#if !WINDOWS_UWP
                    var connectionStringBind = control.Resources[Properties.Settings.Default.ConnectionStringBindTag];
#else
                    var connectionStringBind = control.Resources["ConnectionStringBind"];
#endif
                    if (connectionStringBind != null)
                        listConnectionString.Add(control);
                }
                catch (Exception)
                {
                }
                try
                {
#if !WINDOWS_UWP
                    var eventConnectionStringBind = control.Resources[Properties.Settings.Default.EventConnectionStringBindTag];
#else
                    var eventConnectionStringBind = control.Resources["EventConnectionStringBind"];
#endif
                    if (eventConnectionStringBind != null)
                        listEventConnectionString.Add(control);
                }
                catch (Exception)
                {
                }
                try
                {
#if !WINDOWS_UWP
                    var schedulerBind = control.Resources[Properties.Settings.Default.SchedulerBindTag];
#else
                    var schedulerBind = control.Resources["SchedulerDataSource"];
#endif
                    if (schedulerBind != null)
                        listScheduler.Add(control);
                }
                catch (Exception)
                {
                } 
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreBindConnectionStringSource(List<FrameworkElement> list, String connectionString, bool bDesign = false)
        {
            try
            {
                foreach (var fe in list)
                {
#if !WINDOWS_UWP
                    fe.SetValue(AutomationProperties.AutomationIdProperty, Title); // can be used in the control
#endif
                    var t = fe.GetType();
                    var p = t.GetProperty(bDesign ? "ConnectionStringDesignMode" : "ConnectionString");
                    if (p != null && String.IsNullOrEmpty(p.GetValue(fe, null) as String))
                        p.SetValue(fe, connectionString, null);
                }
            }
            catch (Exception ex)
            {

            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreBindSchedulerSource(List<FrameworkElement> list, String server)
        {
            PreBindAlarmSource(list, server);
        }
        public void PreBindRecipeSource(List<FrameworkElement> list, String clientSessionName)
        {
            try
            {
                foreach (var fe in list)
                {
                    var t = fe.GetType();
                    var p = t.GetProperty("ClientSessionName");
                    if (p != null && String.IsNullOrEmpty(p.GetValue(fe, null) as String))
                        p.SetValue(fe, clientSessionName, null); // can be used in the control
                }
            }
            catch (Exception ex)
            {

            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreBindAlarmSource(List<FrameworkElement> list, String server)
        {
            try
            {
                var serverReference = server.FromXml<OPCUAEntityReference>();
                if (serverReference == null)
                    return;

#if !WINDOWS_UWP
                foreach (var fe in list)
                {
                    fe.SetValue(AutomationProperties.AutomationIdProperty, Title); // can be used in the control
                }
#endif
                var data = (from c in list
                            where (GetEntityFromElement(c).OpcuaEntityReference == null
#if !WINDOWS_UWP
                                  || !GetEntityFromElement(c).OpcuaEntityReference.IsValid
#endif
                            )
                            select GetEntityFromElement(c)).ToList();
                Parallel.ForEach(data, d => 
                    {
                        d.OpcuaEntityReference = server.FromXml<OPCUAEntityReference>();
                    });
            }
            catch (Exception ex)
            {
                
            }
        }

        ScreenEntity GetEntityFromElement(FrameworkElement fe)
        {
            var name = GetEntityName(fe);
            return MapScreenEntities[name];
        }

        public String GetEntityName(FrameworkElement fe, bool bAdd = true)
        {
            if (fe == null)
                return String.Empty;

            if (!String.IsNullOrEmpty(fe.Name))
            {
                if (bAdd)
                {
                    if (!MapScreenEntities.ContainsKey(fe.Name))
                        MapScreenEntities.Add(fe.Name, new ScreenEntity());
                }
                return fe.Name;
            }
#if !WINDOWS_UWP
            else
            {
                var listContainer = new List<String>();
                var element = fe;
                while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                {
                    if (String.IsNullOrEmpty(element.Name))
                    {
                        var name = element.Uid as String;
                        if (String.IsNullOrEmpty(name))
                            name = element.Name;
                        if (!String.IsNullOrEmpty(name))
                            listContainer.Insert(0, name);
                    }
                    else
                    {
                        listContainer.Insert(0, element.Name);
                        break;
                    }
                }

                var nameEntity = FormatContainerNames(listContainer);
                if (!String.IsNullOrEmpty(fe.Uid))
                    nameEntity = String.Format("{0}{1}{2}", nameEntity,
                                    innerEntityNameFormat, fe.Uid as String);

                if (bAdd)
                {
                    if (!MapScreenEntities.ContainsKey(nameEntity))
                        MapScreenEntities.Add(nameEntity, new ScreenEntity());
                }

                return nameEntity;
            }
#else
            return String.Empty;
#endif
        }

        String parameterFile;
        public void SetParameterFile(String file)
        {
            parameterFile = file;

#if !WINDOWS_UWP
            parameterFile = GetFullParameterFilePath(file);
#endif
        }
#endif
        public string GetFullParameterFilePath(String file)
        {
            var ret = file;
#if !WINDOWS_UWP
            if (!String.IsNullOrEmpty(file) && !file.Contains('.'))
            {
#if !NET_STANDARD
                var paramenterDocumentManager = GetService(typeof(IScreenParametersEditorManager)) as IDocumentManager;
                if (paramenterDocumentManager != null)
                    ret = String.Format("{0}{1}{2}{3}", paramenterDocumentManager.TypeLabel, Path.DirectorySeparatorChar, file, paramenterDocumentManager.FileType);
                else if (IsOnBlindServer())
                    ret = String.Format("{0}{1}{2}.{3}", Properties.Settings.Default.DefaultParametersFolder, Path.DirectorySeparatorChar, file, Properties.Settings.Default.DefaultParametersExtension);
#else
                ret = String.Format("{0}{1}{2}.{3}", Properties.Settings.Default.DefaultParametersFolder, Path.DirectorySeparatorChar, file, Properties.Settings.Default.DefaultParametersExtension);
#endif
            }
#endif
            return ret;
        }

        public Dictionary<String, String> LoadParameterFile(String parameterFile)
        {
            var ret = new Dictionary<String, String>();

            var file = parameterFile;
            if (Parent != null)
            {
                var uri = new Uri(file, UriKind.RelativeOrAbsolute);
                uri = Parent.MakeAbosoluteUri(uri);
                file = uri.GetPathString();
            }

#if !WINDOWS_UWP && !NET_STANDARD
            bool bTempFile = false;
            if (fileSystemProviderBase != null)
            {
                if (fileSystemProviderBase.Exists(new FileManagerFile(fileSystemProviderBase, file)))
                {
                    var data = fileSystemProviderBase.ReadFile(new FileManagerFile(fileSystemProviderBase, file));
                    var tempFile = new TemporaryFile();
                    try
                    {
                        File.WriteAllBytes(tempFile.FilePath, data);
                    }
                    catch (Exception ex)
                    {
                        var syslog = LogManager.GetLogger(Title);
                        syslog.Error(String.Format(Properties.Resources.ErrorReadingParameterFile, file), ex);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(this, Properties.Resources.LicenseManager,
                            DateTime.UtcNow, $"{String.Format(Properties.Resources.ErrorReadingParameterFile, file)}: {ex.Message}",
                            System.Diagnostics.EventLogEntryType.Error);
                        return null;
                    }

                    file = tempFile.FilePath;
                    bTempFile = true;
                }
            }
#endif
            try
            {
                var text = String.Empty;
#if !WINDOWS_UWP
                if ((Parent != null && Parent.Protected) || !Utilities.IO.FileSystem.IsXmlFile(file))
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(file));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        using (StreamReader d = new StreamReader(reader))
                        {
                            text = d.ReadToEnd();
                        }
                    }
                }
                else
#endif
                    text = File.ReadAllText(file);

#if !WINDOWS_UWP && !NET_STANDARD
                if (bTempFile)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
#endif

                ret = new Dictionary<string, string>();
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(text, "parameters", true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            ret.Add(r.Key.ToString(), r.Value.ToString());
#if !NET_STANDARD
                            var key = GetHumanReadablePath(r.Key.ToString());
                            if (!ret.ContainsKey(key))
                                ret.Add(key, r.Value.ToString());
#endif
                        });
                    });
                }
            }
            catch (Exception e)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                var syslog = LogManager.GetLogger(Title);
                syslog.Error(String.Format(Properties.Resources.ErrorReadingParameterFile, file), e);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(this, Properties.Resources.LicenseManager,
                    DateTime.UtcNow, $"{String.Format(Properties.Resources.ErrorReadingParameterFile, file)}: {e.Message}",
                    System.Diagnostics.EventLogEntryType.Error);
#endif
                return null;
            }

            return ret;
        }
#if !NET_STANDARD
        internal Dictionary<String, String> mapCurrentParameteItems;
        List<String> pendingTerminatingScript;
        DispatcherTimer delayTerminate;
        String lastParameterFile;
        public void ApplyParameterFile()
        {
            if (lastParameterFile != null && parameterFile != null && 
                String.Compare(lastParameterFile, parameterFile, true) == 0)
                return;
            if (lastParameterFile == null && parameterFile == null)
                return;
            try
            {
                lastParameterFile = parameterFile;

                mapCurrentParameteItems = null;
                if (String.IsNullOrEmpty(parameterFile))
                    return;

                mapCurrentParameteItems = LoadParameterFile(parameterFile);
                if (mapCurrentParameteItems == null)
                    return;

                var listNamesChanged = new List<String>();
                var map = GetDynamicMapForElementAndChilds();
                var mapChanged = new Dictionary<String, String>();
                foreach (var key in map.Keys)
                {
                    foreach (var item in map[key])
                    {
                        var beforeChanging = item.ToXml();
                        var ret = item.UpdatePathFromMap(mapCurrentParameteItems);
                        if (ret)
                        {
                            // lock (listNamesChanged)
                            {
                                if (!listNamesChanged.Contains(key))
                                    listNamesChanged.Add(key);
                                if (!mapChanged.ContainsKey(beforeChanging))
                                    mapChanged.Add(beforeChanging, item.ToXml());
                            }
                        }
                    }
                }

                //if (mapChanged.Count > 0)
                //{
                //    listNamesChanged.ForEach(key =>
                //    {
                //        if (MapScreenEntities[key].Element is IDynamicTagAware)
                //        {
                //            var dyamicAware = MapScreenEntities[key].Element as IDynamicTagAware;
                //            dyamicAware.UpdateMapDynamics(mapChanged);
                //        }
                //    });
                //}

                if (inExecution)
                {
                    listNamesChanged.ForEach(key =>
                    {
#if !WINDOWS_UWP
                    if (!MapScreenEntities[key].TerminateScriptCode())
                        {
                            if (pendingTerminatingScript == null)
                                pendingTerminatingScript = new List<String>();
                            if (!pendingTerminatingScript.Contains(key))
                            {
                                pendingTerminatingScript.Add(key);

                                if (delayTerminate == null)
                                {
                                    delayTerminate = new DispatcherTimer();
                                    delayTerminate.Interval = TimeSpan.FromSeconds(1);
                                    delayTerminate.Tick += (o, e) =>
                                    {
                                        delayTerminate.Stop();

                                        if (!bDisposed)
                                        {
                                            pendingTerminatingScript.ToList().ForEach(element =>
                                            {
                                                if (MapScreenEntities.ContainsKey(element) &&
                                                    MapScreenEntities[element].TerminateScriptCode())
                                                {
                                                    pendingTerminatingScript.Remove(element);

                                                    var listCommands = MapScreenEntities[element].CommandList as CommandManagerList;
                                                    listCommands.ForEach(command =>
                                                    {
                                                        command.Terminate();
                                                    });

                                                    var listAnimation = MapScreenEntities[element].AnimationList as AnimationManagerList;
                                                    listAnimation.ForEach(animation =>
                                                    {
                                                        animation.Stop();
                                                        animation.Terminate();
                                                    });

                                                    MapScreenEntities[element].TerminateExecution();

                                                    ApplyMatchTypeDefinition(MapScreenEntities[element].Element, mapChanged);
                                                    MapScreenEntities[element].RefreshAllEntityReferences();

	                                                var entity = MapScreenEntities[element];
	                                                var control = entity.Entity;
	                                                entity.Entity = null;
	                                                entity.Entity = control;
	
#if !WINDOWS_UWP
	                                                if (!entity.SourceSymbolLinked || entity.SourceSymbolLinkedResolved) // wait for the symbol been loaded
#endif
	                                                {
	                                                    listCommands.ForEach(command =>
	                                                    {
	                                                        command.sExpression = entity.Expression;
	                                                        command.sReverseExpression = entity.ReverseExpression;
	                                                        command.mapDynamics = dataContextExpando;
                                                            command.mapCurrentParameteItems = mapCurrentParameteItems;

                                                            entity.ReplaceAlias(command);
	
	                                                        command.Init(entity, this, currentSessionName, entity.ExpressionEntity_ParserError, entity.ExpressionEntity_ExecutionError);
	                                                    });
	
	                                                    MapScreenEntities[element].PrepareExecution(this, feparentExecute, currentSessionName);
#if !WINDOWS_UWP
	                                                    MapScreenEntities[element].ExecuteScriptCode();
#endif
	                                                }
	                                            }
                                            });

                                            if (pendingTerminatingScript.Count > 0)
                                                delayTerminate.Start();
                                        }
                                    };
                                }

                                delayTerminate.Start();
                            }
                        }
#endif
                    if (pendingTerminatingScript == null || !pendingTerminatingScript.Contains(key))
                        {
                            var listCommands = MapScreenEntities[key].CommandList as CommandManagerList;
                            listCommands.ForEach(command =>
                            {
                                command.Terminate();
                            });

                            var listAnimation = MapScreenEntities[key].AnimationList as AnimationManagerList;
                            listAnimation.ForEach(animation =>
                            {
                                animation.Stop();
                                animation.Terminate();
                            });

                            MapScreenEntities[key].TerminateExecution();
                        }
                    });


                    listNamesChanged.ForEach(key =>
                    {
                        if (pendingTerminatingScript == null || !pendingTerminatingScript.Contains(key))
                        {
                            ApplyMatchTypeDefinition(MapScreenEntities[key].Element, mapChanged);
                            MapScreenEntities[key].RefreshAllEntityReferences();

                            var entity = MapScreenEntities[key];
                            var control = entity.Entity;
                            entity.Entity = null;
                            entity.Entity = control;

#if !WINDOWS_UWP
                        if (!entity.SourceSymbolLinked || entity.SourceSymbolLinkedResolved) // wait for the symbol been loaded
#endif
                        {
                            var listCommands = entity.CommandList as CommandManagerList;
                            listCommands.ForEach(command =>
                            {
                                command.sExpression = entity.Expression;
                                command.sReverseExpression = entity.ReverseExpression;
                                command.mapDynamics = dataContextExpando;
                                command.mapCurrentParameteItems = mapCurrentParameteItems;

                                entity.ReplaceAlias(command);

                                command.Init(entity, this, currentSessionName, entity.ExpressionEntity_ParserError, entity.ExpressionEntity_ExecutionError);
                            });

                            MapScreenEntities[key].PrepareExecution(this, feparentExecute, currentSessionName);
#if !WINDOWS_UWP
                            MapScreenEntities[key].ExecuteScriptCode();
#endif
                        }
                    }
                    });
                }
                else
                {
                    listNamesChanged.ForEach(key =>
                    {
                        ApplyMatchTypeDefinition(MapScreenEntities[key].Element, mapChanged);
                    });
                }
            }
            finally
            {
                OnParameterFileChanged(this, EventArgs.Empty);
            }
        }

        public string GetParameterVariable(string alias)
        {
            if (mapCurrentParameteItems == null || !mapCurrentParameteItems.ContainsKey(alias))
                return String.Empty;

            return GetHumanReadablePath(mapCurrentParameteItems[alias]);
        }

        static char separator = ':';
        static string rootTags = "Tags/";
        string GetHumanReadablePath(string relativePath)
        {
            var readablePath = relativePath;
            int nFound = relativePath.LastIndexOf(separator);
            if (nFound > 0)
            {
                var ns = String.Format("{0}{1}", relativePath[nFound - 1], separator);
                readablePath = relativePath.Replace(ns, "");
                if (readablePath.StartsWith(rootTags))
                    readablePath = readablePath.Remove(0, rootTags.Length);
                readablePath = readablePath.Replace("/", "\\");
            }

            return readablePath;
        }

        void ApplyMatchTypeDefinition(UIElement element, Dictionary<string,string> mapChanged)
        {
            if (element is IDynamicTagAware)
            {
                var dyamicAware = element as IDynamicTagAware;
                mapChanged.Keys.ToList().ForEach(k =>
                {
                    dyamicAware.MatchTypeDefinition(k, mapChanged[k]);
                });
            }
        }

        internal String FindAlias(String key, ScreenEntity child)
        {
            var found = (from c in MapScreenEntities.AsParallel()
                                  where c.Value == child
                                  select c.Key).ToList();
            if (found.Count > 0)
            {
                var name = found[0];
                var index = name.LastIndexOf(innerEntityNameFormat);
                while(index != -1)
                {
                    name = name.Substring(0, index);
                    if (MapScreenEntities.ContainsKey(name))
                    {
                        var entity = MapScreenEntities[name];
                        if (entity.MapAlias != null && entity.MapAlias.ContainsKey(key) && 
                            !String.IsNullOrEmpty(entity.MapAlias[key]))
                            return entity.MapAlias[key];
                    }

                    index = name.LastIndexOf(innerEntityNameFormat);
                }
            }

            if (mapAlias != null && mapAlias.ContainsKey(key) && 
                !String.IsNullOrEmpty(mapAlias[key]))
                return mapAlias[key];

            return null;
        }

        FrameworkElement feparentExecute;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PrepareExecution(FrameworkElement parent, String sessionname, List<String> keys, 
                bool bEvaluateInner = false)
        {
            if (inExecution && !bEvaluateInner)
                return;

            feparentExecute = parent;
            UpdateSessionSettings();
            var map = new Dictionary<String, UIElement>();
            var mapPendingPrepare = new Dictionary<String, ScreenEntity>();
            foreach (var name in keys)
            {
                UIElement uie = FindInnerControl(parent, name);
                if (uie == null)
                    continue;
                var entity = MapScreenEntities[name];
                entity.Entity = uie;
                entity.Document = this;
                mapPendingPrepare.Add(name, entity);
                map.Add(name, uie);
            }

            if (!bEvaluateInner)
            {
                ApplyParameterFile();
                inExecution = true;
            }

            currentSessionName = sessionname;

            var dynKeys = (from c in keys.AsParallel()
                           where MapScreenEntities[c].OpcuaEntityReference != null &&
                                 MapScreenEntities[c].OpcuaEntityReference.IsValid
                           select c).ToList();
            foreach (var name in dynKeys)
            {
                //if (MapScreenEntities[name].OpcuaEntityReference == null ||
                //    !MapScreenEntities[name].OpcuaEntityReference.IsValid)
                //    continue;
                var checkinner = String.Format("{0}{1}", name, innerEntityNameFormat);
                var listInners = (from c in MapScreenEntities.Keys.AsParallel()
                                  where c.StartsWith(checkinner) &&
                                  (MapScreenEntities[c].HasCommands || MapScreenEntities[c].HasAnimations ||
                                  !String.IsNullOrEmpty(MapScreenEntities[c].Expression)) &&
                                  (MapScreenEntities[c].OpcuaEntityReference == null ||
                                  !MapScreenEntities[c].OpcuaEntityReference.IsValid &&
                                  !MapScreenEntities[c].OpcuaEntityReference.IsRelative)
                                  select c).ToList();
                if (listInners.Count == 0)
                    continue;
                var xml = MapScreenEntities[name].OpcuaEntityReference.ToXml();
                listInners.ForEach(inner =>
                {
                    MapScreenEntities[inner].OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
                    MapScreenEntities[inner].Expression = MapScreenEntities[name].Expression;
                    MapScreenEntities[inner].ReverseExpression = MapScreenEntities[name].ReverseExpression;
                });
            }

            foreach (var name in mapPendingPrepare.Keys)
            {
                var entity = mapPendingPrepare[name];
                entity.PrePrepareExecution(sessionname);
                if (entity.ListAnimations != null)
                    entity.ListAnimations.ForEach((animation) => animation.PreInit(entity));
            }

            foreach (var name in map.Keys)
            {
                var uie = map[name];
                var entity = MapScreenEntities[name];

                if (!IsInnerEntity(name) || bEvaluateInner || !entity.SourceSymbolLinked)
                    ResolveRelativeItems(uie as FrameworkElement, name, parent, false);
                if (IsInnerEntity(name) && bEvaluateInner && !entity.BrushAndPenTagResolved)
                {
                    if (uie.IsVisible)
                    {
                        ResolveEntityBrushAndPen(parent, uie as FrameworkElement, name);
                        entity.BrushAndPenTagResolved = true;
                    }
                    else
                    {
                        var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                        var notifierVisibility = new PropertyChangeNotifier(uie, propDesc.Name);
                        if (propertyChangeNotifierVisibilityList == null)
                            propertyChangeNotifierVisibilityList = new List<PropertyChangeNotifier>();
                        propertyChangeNotifierVisibilityList.Add(notifierVisibility);
                        notifierVisibility.ValueChanged += (o, e) =>
                        {
                            if (uie.IsVisible)
                            {
                                notifierVisibility.Dispose();
                                propertyChangeNotifierVisibilityList.Remove(notifierVisibility);
                                if (!entity.BrushAndPenTagResolved)
                                {
                                    if (uie is FrameworkElement)
                                        (uie as FrameworkElement).ApplyTemplate();
                                    uie.UpdateLayout();
                                    ResolveEntityBrushAndPen(parent, uie as FrameworkElement, name);
                                    entity.BrushAndPenTagResolved = true;
                                }
                            }
                        };
                    }
                }

                if (
#if !WINDOWS_UWP
                    !InTest &&
#endif
                    !entity.SourceSymbolLinked && entity.HasCommands)
                {
                    var control = uie;
                    /*
#if WINDOWS_UWP
                    var commandProperty = control.GetType().GetProperty("Command");
                    if (uie is ContentControl && commandProperty == null)
#else
                    if (uie is ContentControl && !(uie is ICommandSource))
#endif
                        control = (uie as ContentControl).Content as UIElement;
#if !WINDOWS_UWP
                    if (control is ICommandSource)
#else
                    if (commandProperty != null)
#endif
                    */
                    {
                        var listCommands = entity.CommandList as CommandManagerList;
                        listCommands.ForEach(command =>
                        {
                            entity.Entity = control;
                            command.sExpression = entity.Expression;
                            command.sReverseExpression = entity.ReverseExpression;
                            command.mapDynamics = dataContextExpando;
                            command.mapCurrentParameteItems = mapCurrentParameteItems;

                            entity.ReplaceAlias(command);

                            command.Init(entity, this, sessionname, entity.ExpressionEntity_ParserError, entity.ExpressionEntity_ExecutionError);
                        });

#if WINDOWS_UWP
                        var commandProperty = control.GetType().GetProperty("Command");
                        if (uie is ContentControl && !(uie is UserControl) && commandProperty == null)
#else
                        if (uie is ContentControl && !(uie is UserControl) && !(uie is ICommandSource))
#endif
                            control = (uie as ContentControl).Content as UIElement;
#if !WINDOWS_UWP
                        if (control is ICommandSource)
#else
                        if (commandProperty != null)
#endif
                        {
                            control.GetType().GetProperty("CommandParameter").SetValue(control, name, null);
#if !WINDOWS_UWP
                            control.GetType().GetProperty("Command").SetValue(control, Command, null);
#else
                            commandProperty.SetValue(control, Command, null);
#endif
                        }
                        if (control != null)
                            Stylus.SetIsPressAndHoldEnabled(control, false);
                    }
                }

                //if (entity.IsDynamicEntity() && !(uie is Grid))
                //{
                //    bool bSet = false;
                //    Effect oldFx = null;
                //    bool oldClipToBound = false;
                //    uie.MouseEnter += (o, ev) =>
                //        {
                //            if (!bSet)
                //            {
                //                bSet = true;
                //                uie.Scale(0.1, 150, false, false, new BackEase() { EasingMode = EasingMode.EaseOut }, false, null, true);

//                if (uie.Effect == null)
//                {
//                    oldFx = uie.Effect;
//                    oldClipToBound = uie.ClipToBounds;
//                    var effect = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 50, Opacity = 0.5 };
//                    uie.Effect = effect;
//                    uie.ClipToBounds = false;
//                }
//            }
//        };
//    uie.MouseLeave += (o, ev) =>
//        {
//            if (bSet)
//            {
//                bSet = false;
//                if (oldFx != null)
//                {
//                    uie.Effect = oldFx;
//                    uie.ClipToBounds = oldClipToBound;
//                }

//                uie.Scale(-0.1, 500, false, false, new BackEase() { EasingMode = EasingMode.EaseOut }, false, null, true);
//            }
//        };
//}

#if !WINDOWS_UWP
                if (!entity.SourceSymbolLinked || entity.SourceSymbolLinkedResolved) // wait for the symbol been loaded
#endif
                {
                    entity.PrepareExecution(this, parent, sessionname);
#if !WINDOWS_UWP
                    entity.ExecuteScriptCode();
#endif
                }
            }

            // Mediator.NotifyColleagues<ScreenDocument>("PreparedExecution", this);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TerminateExecution(FrameworkElement parent)
        {
            if (!inExecution)
                return;

            // Mediator.NotifyColleagues<ScreenDocument>("TerminatingExecution", this);
            inExecution = false;

            foreach (var name in MapScreenEntities.Keys)
            {
                var uie = FindInnerControl(parent, name);
                // var uie = parent.FindName(name) as UIElement;
                ScreenEntity entity = MapScreenEntities[name];

//#if !WINDOWS_UWP
//                entity.TerminateScriptCode();
//#endif
                entity.TerminateExecution();

                if (
#if !WINDOWS_UWP
                    !InTest && 
#endif
                    entity.HasCommands)
                {
                    var control = uie;
                    /*
#if WINDOWS_UWP
                    var commandProperty = control.GetType().GetProperty("Command");
                    if (uie is ContentControl && commandProperty == null)
#else
                    if (uie is ContentControl && !(uie is ICommandSource))
#endif
                        control = (uie as ContentControl).Content as FrameworkElement;
#if !WINDOWS_UWP
                    if (control is ICommandSource)
#else
                    if (commandProperty != null)
#endif
                    */
                    {
                        var listCommands = entity.CommandList as CommandManagerList;
                        listCommands.ForEach(command =>
                        {
                            command.Terminate();
                        });
#if WINDOWS_UWP
                        var commandProperty = control.GetType().GetProperty("Command");
                        if (uie is ContentControl && !(uie is UserControl) && commandProperty == null)
#else
                        if (uie is ContentControl && !(uie is UserControl) && !(uie is ICommandSource))
#endif
                            control = (uie as ContentControl).Content as FrameworkElement;
#if !WINDOWS_UWP
                        if (control is ICommandSource)
#else
                        if (commandProperty != null)
#endif
                        {
#if !WINDOWS_UWP
                            control.GetType().GetProperty("Command").SetValue(control, null, null);
#else
                            commandProperty.SetValue(control, null, null);
#endif
                            control.GetType().GetProperty("CommandParameter").SetValue(control, null, null);
                        }
                        // Stylus.SetIsPressAndHoldEnabled(control, false);
                    }
                }

                var listAnimation = entity.AnimationList as AnimationManagerList;
                listAnimation.ForEach(animation =>
                {
                    animation.Stop();
                    animation.Terminate();
                });
            }

#if !WINDOWS_UWP
            TerminateVariableValues();
#endif
            feparentExecute = null;
            // Mediator.NotifyColleagues<ScreenDocument>("TerminatedExecution", this);
        }

        public bool CanUnload()
        {
            return keepAlwaysInMemory != true;
        }
#endif
#endregion Methods

#region Properties

        String lastSession;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public string SessionString
        {
            get
            {
                if (!String.IsNullOrEmpty(sessionName))
                {
                    return sessionName;
                }

                if (!String.IsNullOrEmpty(lastSession))
                {
                    return lastSession;
                }

                var docParent = Parent;
                if (Parent != null)
                    docParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                lastSession = docParent != null ? docParent.Title : Title;
                return lastSession;
            }
        }

        public string SessionName
        {
            get { return sessionName; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (value == sessionName)
                    return;
                sessionName = value;
                NeedsSave = true;
                UpdateSessionSettings();
                OnPropertyChanged("SessionName");
                OnPropertyVisiblityChanged("SessionName");
            }
#endif
        }

        public int RemoveDisabledItemAfterSecs
        {
            get { return removeDisabledItemAfterSecs; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (removeDisabledItemAfterSecs == value)
                    return;
                removeDisabledItemAfterSecs = value;
                OnPropertyChanged("RemoveDisabledItemAfterSecs");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int MaxCleanCount
        {
            get { return maxCleanCount; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (maxCleanCount == value)
                    return;
                maxCleanCount = value;
                OnPropertyChanged("MaxCleanCount");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool UseAlwaysSecureConnections
        {
            get { return useAlwaysSecureConnections; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (useAlwaysSecureConnections == value)
                    return;
                useAlwaysSecureConnections = value;
                OnPropertyChanged("UseAlwaysSecureConnections");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int FastSamplingInterval
        {
            get
            {
                return fastSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (fastSamplingInterval == value)
                    return;
                fastSamplingInterval = value;
                OnPropertyChanged("FastSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int SlowSamplingInterval
        {
            get
            {
                return slowSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (slowSamplingInterval == value)
                    return;
                slowSamplingInterval = value;
                OnPropertyChanged("SlowSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool DisableWhenNotUsed
        {
            get { return disableWhenNotUsed; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (disableWhenNotUsed == value)
                    return;
                disableWhenNotUsed = value;
                OnPropertyChanged("DisableWhenNotUsed");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int PublishingInterval
        {
            get
            {
                return publishingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (publishingInterval == value)
                    return;
                publishingInterval = value;
                OnPropertyChanged("PublishingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool LoadFromRepositorySynchro
        {
            get
            {
                return loadFromRepositorySynchro;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (loadFromRepositorySynchro == value)
                    return;
                loadFromRepositorySynchro = value;
                OnPropertyChanged("LoadFromRepositorySynchro");
                NeedsSave = true;
            }
#endif
        }

        public Guid IdDocument
        {
            get
            {
                return idDocument;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public int WriteTimeout
        {
            get { return writeTimeout; }
            set
            {
                if (value == writeTimeout)
                    return;
                writeTimeout = value;
                OnPropertyChanged("WriteTimeout");
                NeedsSave = true;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInLibrary
        {
            get
            {
                return isInLibrary;
            }
            set
            {
                if (value == isInLibrary)
                    return;

                isInLibrary = value;
                OnPropertyChanged("IsInLibrary");
            }
        }
#endif

        bool inRuntime;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool InRuntime
        {
            get
            {
                return inRuntime;
            }
            set
            {
                if (value == inRuntime)
                    return;

            	inRuntime = value;
                OnPropertyChanged("InRuntime");
            }
        }

#if !WINDOWS_UWP
#if !NET_STANDARD
        [Browsable(false)]
        public bool InTest
        {
            get
            {
                return inTest;
            }
            set
            {
                if (value == inTest)
                    return;

                inTest = value;
                OnPropertyChanged("InTest");
            }
        }
#endif

        static public String DefaultXaml
        {
            get
            {
                return Properties.Settings.Default.WPFDefaultXAML;
            }
        }
#endif
        string fullPath;
#if !WINDOWS_UWP
        [ReadOnly(true)]
#endif
        public String FullPath
        {
            get
            {
                if (!String.IsNullOrEmpty(fullPath))
                    return fullPath;
                if (xamlDocument != null)
                    return xamlDocument.FullPath;
                return String.Empty;
            }

            set
            {
                fullPath = value;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        private bool _NeedsSave = false;
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return _NeedsSave; }
            set
            {
                if (_NeedsSave == value)
                    return;

                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }

        private bool _LayoutChanged = false;
        [Browsable(false)]
        public bool LayoutChanged
        {
            get { return _LayoutChanged; }
            set
            {
                if (_LayoutChanged == value)
                    return;

                _LayoutChanged = value;
                OnPropertyChanged("LayoutChanged");
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public ScreenObjectsSettingsMap MapScreenEntities
        {
            get
            {
                if (mapScreenEntities == null)
                    mapScreenEntities = new ScreenObjectsSettingsMap();
                return mapScreenEntities;
            }
            set
            {
                if (mapScreenEntities == value)
                    return;
                mapScreenEntities = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public List<String> ListResources
        {
            get
            {
                if (listResources == null)
                    listResources = new List<String>();
                return listResources;
            }
            set
            {
                if (listResources == value)
                    return;
                listResources = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public List<String> ListAssemblies
        {
            get
            {
                if (listAssemblies == null)
                    listAssemblies = new List<String>();
                return listAssemblies;
            }
            set
            {
                if (listAssemblies == value)
                    return;
                listAssemblies = value;
            }
        }

        public bool ShowInTaskbar
        {
            get
            {
                return showInTaskbar;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (showInTaskbar == value)
                    return;
                showInTaskbar = value;
                OnPropertyChanged("ShowInTaskbar");
                NeedsSave = true;
            }
#endif
        }

        public double Top
        {
            get
            {
                return top;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (top == value)
                    return;
                top = value;
                OnPropertyChanged("Top");
                NeedsSave = true;
            }
#endif
        }

        public double Left
        {
            get
            {
                return left;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (left == value)
                    return;
                left = value;
                OnPropertyChanged("Left");
                NeedsSave = true;
            }
#endif
        }

        public double PreviewWidth;
        public double Width
        {
            get
            {
                return width;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (width == value)
                    return;
                PreviewWidth = width;
                PreviewHeight = height;
                width = value;
                OnPropertyChanged("Width");
                NeedsSave = true;
            }
#endif
        }

        public double PreviewHeight;
        public double Height
        {
            get
            {
                return height;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (height == value)
                    return;
                PreviewWidth = width;
                PreviewHeight = height;
                height = value;
                OnPropertyChanged("Height");
                NeedsSave = true;
            }
#endif
        }

        public WindowState WindowState
        {
            get
            {
                if (windowState == WindowState.Minimized)
                    return WindowState.Normal;
                return windowState;
            }
            set
            {
                if (windowState == value)
                    return;
                windowState = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("WindowState");
                NeedsSave = true;
#endif
            }
        }

        public WindowStyle WindowStyle
        {
            get
            {
                return windowStyle;
            }
            set
            {
                if (windowStyle == value)
                    return;
                windowStyle = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("WindowStyle");
                NeedsSave = true;
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public ResizeMode ResizeMode
        {
            get
            {
                return resizeMode;
            }
            set
            {
                if (resizeMode == value)
                    return;
                resizeMode = value;
                OnPropertyChanged("ResizeMode");
                NeedsSave = true;
            }
        }

        public WindowStartupLocation WindowStartupLocation
        {
            get
            {
                return windowStartupLocation;
            }
            set
            {
                if (windowStartupLocation == value)
                    return;
                windowStartupLocation = value;
                OnPropertyChanged("WindowStartupLocation");
                NeedsSave = true;
            }
        }

        public double WindowOpacity
        {
            get
            {
                return windowOpacity;
            }
            set
            {
                if (windowOpacity == value)
                    return;
                windowOpacity = value;
                OnPropertyChanged("WindowOpacity");
                NeedsSave = true;
            }
        }

        public bool WindowOpacityOnlyInactive
        {
            get
            {
                return windowOpacityOnlyInactive;
            }
            set
            {
                if (windowOpacityOnlyInactive == value)
                    return;
                windowOpacityOnlyInactive = value;
                OnPropertyChanged("WindowOpacityOnlyInactive");
                NeedsSave = true;
            }
        }

        public bool ApplyWindowSettingsOnLoad
        {
            get
            {
                return applyWindowSettingsOnLoad;
            }
            set
            {
                if (applyWindowSettingsOnLoad == value)
                    return;
                applyWindowSettingsOnLoad = value;
                OnPropertyChanged("ApplyWindowSettingsOnLoad");
                NeedsSave = true;
            }
        }
    
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
#if !NET_STANDARD
        public Brush Background
        {
            get
            {
                return background;
            }
#if !WINDOWS_UWP
            set
            {
                if (background == value)
                    return;
                background = value;
                OnPropertyChanged("Background");
                NeedsSave = true;
            }
#endif
        }
#endif
        public uint DelayUnloadSecs
        {
            get
            {
                return delayUnloadSecs;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (delayUnloadSecs == value)
                    return;
                delayUnloadSecs = value;
                OnPropertyChanged("DelayUnloadSecs");
                NeedsSave = true;
            }
#endif
        }
               
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("GeoScada")]
        public bool GeoScada
        {
            get { return false; }
        }

        public bool ScanChildrenElements
        {
            get
            {
                return scanChildrenElements;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (scanChildrenElements == value)
                    return;
                scanChildrenElements = value;
                OnPropertyChanged("ScanChildrenElements");
                NeedsSave = true;
            }
#endif
        }

        public bool KeepAlwaysInMemory
        {
            get
            {
                return keepAlwaysInMemory;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (keepAlwaysInMemory == value)
                    return;
                keepAlwaysInMemory = value;
                OnPropertyChanged("KeepAlwaysInMemory");
                NeedsSave = true;
            }
#endif
        }

        public int DelayLoadSymbols
        {
            get
            {
                return delayLoadSymbols;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (delayLoadSymbols == value)
                    return;
                delayLoadSymbols = value;
                OnPropertyChanged("DelayLoadSymbols");
                NeedsSave = true;
            }
#endif
        }

        public bool HideScrollBars
        {
            get
            {
                return hideScrollBars;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (hideScrollBars == value)
                    return;
                hideScrollBars = value;
                OnPropertyChanged("HideScrollBars");
                NeedsSave = true;
            }
#endif
        }

        public double ZoomLevelCacheModeX
        {
            get
            {
                return zoomLevelCacheModeX;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (zoomLevelCacheModeX == value)
                    return;
                zoomLevelCacheModeX = value;
                OnPropertyChanged("ZoomLevelCacheModeX");
                NeedsSave = true;
            }
#endif
        }

        public double ZoomLevelCacheModeY
        {
            get
            {
                return zoomLevelCacheModeY;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (zoomLevelCacheModeY == value)
                    return;
                zoomLevelCacheModeY = value;
                OnPropertyChanged("ZoomLevelCacheModeY");
                NeedsSave = true;
            }
#endif
        }

        public bool DisableZoom
        {
            get
            {
                return disableZoom;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (disableZoom == value)
                    return;
                disableZoom = value;
                OnPropertyChanged("DisableZoom");
                NeedsSave = true;
            }
#endif
        }

        public bool HideLayoutScreens
        {
            get
            {
                return hideLayoutScreens;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (hideLayoutScreens == value)
                    return;
                hideLayoutScreens = value;
                OnPropertyChanged("HideLayoutScreens");
                NeedsSave = true;
            }
#endif
        }
      
#if !WINDOWS_UWP && !NET_STANDARD
        public bool UseIntelliSense
        {
            get
            { return useIntelliSense; }
            set
            {
                if (value == useIntelliSense)
                    return;
                useIntelliSense = value;
                OnPropertyChanged("UseIntelliSense");
                NeedsSave = true;
            }
        }
#endif

#if !WINDOWS_UWP
        public bool ShowNavigation
        {
            get
            {
                return showNavigation;
            }
#if !NET_STANDARD
            set
            {
                if (showNavigation == value)
                    return;
                showNavigation = value;
                OnPropertyChanged("ShowNavigation");
                NeedsSave = true;
            }
#endif
        }

        public bool ShowHeader
        {
            get
            {
                return showHeader;
            }
#if !NET_STANDARD
            set
            {
                if (showHeader == value)
                    return;
                showHeader = value;
                OnPropertyChanged("ShowHeader");
                NeedsSave = true;
            }
#endif
        }

        public bool ShowTabStrip
        {
            get
            {
                return showTabStrip;
            }
#if !NET_STANDARD
            set
            {
                if (showTabStrip == value)
                    return;
                showTabStrip = value;
                OnPropertyChanged("ShowTabStrip");
                NeedsSave = true;
            }
#endif
        }

        public byte MonitorNumber
        {
            get
            {
                return monitorNumber;
            }
#if !NET_STANDARD
            set
            {
                if (monitorNumber == value)
                    return;
                monitorNumber = value;
                OnPropertyChanged("MonitorNumber");
                NeedsSave = true;
            }
#endif
        }
#endif
        
        public bool ResizeControlsOnFit
        {
            get
            {
                return resizeControlsOnFit;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (resizeControlsOnFit == value)
                    return;
                resizeControlsOnFit = value;
                OnPropertyChanged("ResizeControlsOnFit");
                NeedsSave = true;
            }
#endif
        }

        public bool FitInWindow
        {
            get
            {
                return fitInWindow;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (fitInWindow == value)
                    return;
                fitInWindow = value;
                OnPropertyChanged("FitInWindow");
                OnPropertyVisiblityChanged("ResizeControlsOnFit");
                NeedsSave = true;
            }
#endif
        }

        public bool KeepAspectRatio
        {
            get
            {
                return keepAspectRatio;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (keepAspectRatio == value)
                    return;
                keepAspectRatio = value;
                OnPropertyChanged("KeepAspectRatio");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool ForceWritingOnServer
        {
            get { return forceWritingOnServer; }
            set
            {
                if (forceWritingOnServer == value)
                    return;
                forceWritingOnServer = value;
                OnPropertyChanged("ForceWritingOnServer");
                NeedsSave = true;
            }
        }
#endif

        public bool ToolbarVisible
        {
            get
            {
                return toolbarVisible;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (toolbarVisible == value)
                    return;
                toolbarVisible = value;
                OnPropertyChanged("ToolbarVisible");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP
        public bool LayoutView
        {
            get
            {
                return layoutView;
            }
#if !NET_STANDARD
            set
            {
                if (layoutView == value)
                    return;
                layoutView = value;
                OnPropertyChanged("LayoutView");
                NeedsSave = true;
            }
#endif
        }

        public bool LayoutViewEditable
        {
            get
            {
                return layoutViewEditable;
            }
#if !NET_STANDARD
            set
            {
                if (layoutViewEditable == value)
                    return;
                layoutViewEditable = value;
                OnPropertyChanged("LayoutViewEditable");
                NeedsSave = true;
            }
#endif
        }
#endif
        public bool RequireUserLogin
        {
            get
            {
                return requireUserLogin;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (requireUserLogin == value)
                    return;
                requireUserLogin = value;
                OnPropertyChanged("RequireUserLogin");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Dictionary<String, String> MapAlias
        {
            get { return mapAlias; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (value == mapAlias)
                    return;
                mapAlias = value;
                OnPropertyChanged("MapAlias");
                NeedsSave = true;
            }
#endif
        }

#if !NET_STANDARD
        [Browsable(false)]
        public String ParameterFile
        {
            get
            {
                return parameterFile;
            }
        }
#endif

#endregion Properties

#region Commands
#if !NET_STANDARD
        RelayCommand _Command;

        public ICommand Command
        {
            get
            {
                if (_Command == null)
                {
                    _Command = new RelayCommand(
                        param => ExecuteCommand(param),
                        param => CanExecuteCommand(param)
                        );
                }
                return _Command;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<RemoteExecute> GetPendingCommands()
        {
            var ret = new List<RemoteExecute>();
            lock (lockObject)
            {
                if (pendingCommands != null)
                {
                    ret.AddRange(pendingCommands);
                    pendingCommands.Clear();
                }
            }
            return ret;
        }

        List<RemoteExecute> pendingCommands;
        void ExecuteCommand(Object param)
        {
            var name = param as String;
            if (name == null)
                return;
            ScreenEntity entity = MapScreenEntities[name];
            if (ActiveView != null)
            {
                var cancelEvent = new CancelEventArgs();
                OnCommandExecuting(entity.Entity, cancelEvent);
                if (!cancelEvent.Cancel)
                    entity.ExecuteCommands();
            }
#if !WINDOWS_UWP
            else
            {
                var ret = entity.RemoteExecuteCommands();
                lock (lockObject)
                {
                    if (pendingCommands == null)
                        pendingCommands = new List<RemoteExecute>();
                    pendingCommands.AddRange(ret);
                }
            }
#endif
        }

#if !WINDOWS_UWP
        public void RemoteExecuteCommand(String name)
        {
            if (!MapScreenEntities.ContainsKey(name))
                return;
            if (!CanExecuteCommand(name))
                return;
            var entity = MapScreenEntities[name];
            var ret = entity.RemoteExecuteCommands();
            lock (lockObject)
            {
                if (pendingCommands == null)
                    pendingCommands = new List<RemoteExecute>();
                pendingCommands.AddRange(ret);
            }
        }
#endif
      
        bool CanExecuteCommand(Object param)
        {
            var name = param as String;
            if (name == null)
                return false;
            ScreenEntity entity = MapScreenEntities[name];
            return entity.CanExecuteCommands();
        }
#endif
#endregion Commands

#region ICloneable Members

#if !WINDOWS_UWP && !NET_STANDARD
        ScreenDocument(ScreenDocument template)
        {
            if (template == null)
                return;

            foreach (var v in template.mapScreenEntities.Keys)
                mapScreenEntities.Add(v, template.mapScreenEntities[v].Clone() as ScreenEntity);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Clone()
        {
            return new ScreenDocument(this);
        }
#endif
#endregion ICloneable Members

#region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            // CommandManager.InvalidateRequerySuggested();
            if (propertyName == "Width")
            {
                if (Width <= 0 || Width > Properties.Settings.Default.ScreenMaxWidth)
                    return Properties.Resources.HeightWidthOutOfRange;
            }
            else if (propertyName == "Height")
            {
                if (Height <= 0 || Height > Properties.Settings.Default.ScreenMaxHeight)
                    return Properties.Resources.HeightWidthOutOfRange;
            }
            else if (propertyName == "WindowOpacity")
            {
                if (WindowOpacity < 0.1 || WindowOpacity > 1)
                    return Properties.Resources.WindowOpacityMustBeGreater;
            }

            return null;
        }
#endif
#endregion Validations

#region IDocument Members

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

#if !NET_STANDARD
        UserControl activeView;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

        UserControl view;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UserControl View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
            }
        }
#endif

        IDocument parent;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

            return new Uri(new Uri(System.IO.Path.GetDirectoryName(FullPath) + "\\"), relative);
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

            return new Uri(System.IO.Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return null;
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String rootBase
        {
            get
            {
                try
                {
                    if (Parent != null)
                        return Parent.rootBase;
                    return System.IO.Path.GetDirectoryName(FullPath);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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
                return System.IO.Path.GetFileNameWithoutExtension(FullPath);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String FilePath
        {
            get
            {
                return FullPath;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsEmpty
        {
            get
            {
                return MapScreenEntities.Count == 0;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

#endregion IDocument Members

#region IDisposable Members

        bool bDisposed;
        protected override void OnDispose()
        {
            bDisposed = true;
            OnDisposing(this);

            base.OnDispose();

            lock (lockObject)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (elementSubscribed != null)
                {
                    elementSubscribed.Clear();
                    elementSubscribed = null;
                }
                if (propertyChangeNotifierList != null)
                {
                    propertyChangeNotifierList.ForEach(c => c.Dispose());
                    propertyChangeNotifierList.Clear();
                    propertyChangeNotifierList = null;
                }
                if (propertyChangeNotifierVisibilityList != null)
                {
                    propertyChangeNotifierVisibilityList.ForEach(c => c.Dispose());
                    propertyChangeNotifierVisibilityList.Clear();
                    propertyChangeNotifierVisibilityList = null;
                }
                if (mapItemSourceChangeNotifier != null)
                {
                    mapItemSourceChangeNotifier.Values.ToList().ForEach(c => c?.Dispose());
                    mapItemSourceChangeNotifier.Clear();
                }
                if (mapItemSourceDispatcherOperation != null)
                {
                    mapItemSourceDispatcherOperation.Values.ToList().ForEach(op => 
                    {
                        if (op.Status != DispatcherOperationStatus.Aborted &&
                            op.Status != DispatcherOperationStatus.Completed)
                            op.Abort();
                    });
                    mapItemSourceDispatcherOperation.Clear();
                }
                if (mapItemSourceAction != null)
                    mapItemSourceAction.Clear();

#if !WINDOWS_UWP
                if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted &&
                    d1.Status != DispatcherOperationStatus.Completed)
                {
                    d1.Abort();
                    // d1 = null;
                }
#endif
                if (variableValues != null)
                {
                    variableValues.ForEach(value =>
                        {
                            value.Dispose();
                        });
                    variableValues.Clear();
                    variableValues = null;
                }

                if (variableValuesRuntime != null)
                {
                    var list = variableValuesRuntime.ToList();
                    variableValuesRuntime.Clear();
                    mapvariableValuesRuntime.Clear();
                    variableValuesRuntime = null;
                    list.ForEach(value =>
                    {
                        value.Dispose();
                    });
                }
#endif

#if !NET_STANDARD
                (from value in mapProblematicXamlWriterBag.Values.OfType<IDisposable>()
                 select value as IDisposable).ToList().ForEach(k => { k.Dispose(); });
                CleanProblematicXamlBags();
#endif
            }

#if !NET_STANDARD
            foreach (var mi in mapDataContextExpando.Values)
                mi.Dispose();

            if (mapControlId != null)
                mapControlId.Clear();
#if !WINDOWS_UWP
            if (mapControlTooltip != null)
                mapControlTooltip.Clear();
#endif
            if (mapControlListId != null)
                mapControlListId.Clear();

            if (mapIsTranslatable != null)
                mapIsTranslatable.Clear();

            DependencyObjectExtensions.CleanChildrenOfTypeCache();
            loadedContentControls?.Clear();
#endif
        }

#endregion IDisposable Members

#region Properties for UI

        bool isLayouEditMode;
#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsLayouEditMode
        {
            get
            {
                return isLayouEditMode;
            }
            set
            {

                if (value == isLayouEditMode)
                    return;
                isLayouEditMode = value;
                OnPropertyChanged("IsLayouEditMode");
            }
        }

        bool isShowAdornerExpander;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsShowAdornerExpander
        {
            get
            {
                return isShowAdornerExpander;
            }
            set
            {

                if (value == isShowAdornerExpander)
                    return;
                isShowAdornerExpander = value;
                OnPropertyChanged("IsShowAdornerExpander");
            }
        }
        
        bool isGridMode;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsGridMode
        {
            get
            {
                return isGridMode;
            }
            set
            {

                //if (value == isGridMode)
                //    return;
                isGridMode = value;
                OnPropertyChanged("IsGridMode");
            }
        }

        bool isGridMode5;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsGridMode5
        {
            get
            {
                return isGridMode5;
            }
            set
            {

                //if (value == isGridMode5)
                //    return;
                isGridMode5 = value;
                OnPropertyChanged("IsGridMode5");
            }
        }

        bool isGridMode10;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsGridMode10
        {
            get
            {
                return isGridMode10;
            }
            set
            {

                //if (value == isGridMode10)
                //    return;
                isGridMode10 = value;
                OnPropertyChanged("IsGridMode10");
            }
        }

        bool isGridMode20;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsGridMode20
        {
            get
            {
                return isGridMode20;
            }
            set
            {

                //if (value == isGridMode20)
                //    return;
                isGridMode20 = value;
                OnPropertyChanged("IsGridMode20");
            }
        }

        bool isGridMode40;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsGridMode40
        {
            get
            {
                return isGridMode40;
            }
            set
            {

                //if (value == isGridMode40)
                //    return;
                isGridMode40 = value;
                OnPropertyChanged("IsGridMode40");
            }
        }

        bool isGridMode100;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsGridMode100
        {
            get
            {
                return isGridMode100;
            }
            set
            {

                //if (value == isGridMode100)
                //    return;
                isGridMode100 = value;
                OnPropertyChanged("IsGridMode100");
            }
        }

        bool isSnapToGrid;
        [Browsable(false)]
        public bool IsSnapToGrid
        {
            get
            {
                return isSnapToGrid;
            }
            set
            {

                if (value == isSnapToGrid)
                    return;
                isSnapToGrid = value;
                OnPropertyChanged("IsSnapToGrid");
            }
        }

        bool isSmartSnap;
        [Browsable(false)]
        public bool IsSmartSnap
        {
            get
            {
                return isSmartSnap;
            }
            set
            {

                if (value == isSmartSnap)
                    return;
                isSmartSnap = value;
                OnPropertyChanged("IsSmartSnap");
            }
        }
#endif
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
        List<ScreenEntity> listUnitConverters;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ChangeUnitConverter(String converter, IUnitConverterEditorManager converterEditor, string elementName = null)
        {
            if (listUnitConverters == null)
                listUnitConverters = (from c in MapScreenEntities.Values.AsParallel()
                                      where !String.IsNullOrEmpty(c.IdUnitConverter)
                                      select c).ToList();
            List<ScreenEntity> _list = new List<ScreenEntity>();

            if (!string.IsNullOrEmpty(elementName))
            {
                if(MapScreenEntities.ContainsKey(elementName))
                {
                    var entity = MapScreenEntities[elementName];
                    _list.Add(entity);
                    GetListInners(elementName).ForEach(I =>
                    {
                        _list.Add(MapScreenEntities[I]);
                    });
                }
            }
            else
                _list = listUnitConverters;

            _list.ForEach(entity =>
            {
                if(string.IsNullOrEmpty(converter))
                    entity.ClearUnitConverter();
                else
                {
                    var input = converterEditor.GetInputExpression(this, entity.IdUnitConverter, converter);
                    var output = converterEditor.GetOutputExpression(this, entity.IdUnitConverter, converter);
                    var unit = converterEditor.GetUnitLabel(this, entity.IdUnitConverter, converter);
                    entity.ApplyUnitConverter(unit, input, output);
                }
            });
        }
#endif
#if !NET_STANDARD
        Dictionary<UIElement, ElementTranslation> mapControlId;
#if !WINDOWS_UWP
        Dictionary<UIElement, ElementTranslation> mapControlTooltip;
#endif
        Dictionary<UIElement, ElementTranslation> mapControlListId;
        Dictionary<UIElement, bool> mapIsTranslatable;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<UIElement> ChangeLanguage(String culture, FrameworkElement mainControl, IDictionary<String, String> map, bool bDesign = false, List<UserControl> embedViews = null)
        {
            var ret = new List<UIElement>();

            using (var cursor = new WaitCursor())
            {
                if (map == null || map.Count == 0)
                    return ret;

                if (!bDesign)
                {
                    try
                    {
#if !WINDOWS_UWP
                        var cultureInfo = new CultureInfo(culture, true);
                        if (!cultureInfo.Equals(System.Threading.Thread.CurrentThread.CurrentUICulture))
                        {
                            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
                            // if(GetRunningOnServer(mainControl))
                            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

                            var preferredLocales = new Opc.Ua.StringCollection();
                            if (bIsBlindServer)
                                preferredLocales.Add(cultureInfo.Name);

                            try
                            {
                                RealTimeConnectionManagerViewModel.SetUserIdentity(SessionString, null, preferredLocales);
                            }
                            catch (Exception ex)
                            {
                                // ScreenComponent.UIInterface.ShowError(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                            }
                        }
#endif
                        foreach (var name in MapScreenEntities.Keys)
                        {
                            var entity = MapScreenEntities[name];
                            var listAnimation = entity.AnimationList as AnimationManagerList;
                            listAnimation.ForEach(animation =>
                            {
                                animation.ExecuteChangeLanguage(true);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        var UIMsgBox = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (UIMsgBox != null)
                            UIMsgBox.ShowError(String.Format(Properties.Resources.FailedToActivateCulture, ex.Message));
                        return ret;
                    }
                }

                List<UIElement> listControlCulture = GetDynControls(mainControl, map, embedViews);
                listControlCulture.ForEach(_control =>
                {
                    ret.AddRange(TranslateElement(_control, map));
                });

                ret.AddRange(listControlCulture);
            }

            if (!bDesign)
            {
                foreach (var name in MapScreenEntities.Keys)
                {
                    var entity = MapScreenEntities[name];
                    var listAnimation = entity.AnimationList as AnimationManagerList;
                    listAnimation.ForEach(animation =>
                    {
                        animation.ExecuteChangeLanguage();
                    });
                    if (entity.Entity == null)
                    {
                        UIElement uie = FindInnerControl(mainControl, name);
                        if (uie != null)
                        {
                            entity.Entity = uie;
                            entity.Document = this;
                        }
                    }
                    if (entity.Entity is Control)
                        entity.ApplyCultureFontSettings(culture);
                }
            }

            return ret;
        }

        public class ElementTranslation
        {
            public string translated;
            public string untranslated;
            public List<String> translatedStrings;
            public List<String> untranslatedStrings;
            public bool isTranslated;
            public ElementTranslation (string text)
            {
                untranslated = text;
                translated = text;
            }
            public ElementTranslation(List<String> texts)
            {
                translatedStrings = texts;
                untranslatedStrings = texts;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsTranslated(UIElement control, bool tooltip = false)
        {
            if (tooltip && mapControlTooltip != null && mapControlTooltip.ContainsKey(control))
                return mapControlTooltip[control].isTranslated;
            if ((control is ComboBox || control is ListBox) && mapControlListId != null && mapControlListId.ContainsKey(control))
                return mapControlListId[control].isTranslated;
            else if (mapControlId != null && mapControlId.ContainsKey(control))
                return mapControlId[control].isTranslated;
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<UIElement> TranslateElement(UIElement _control, IDictionary<string, string> map, bool bUntranslate = false)
        {
            var ret = new List<UIElement>();

            //if (bDesign != DesignerProperties.GetIsInDesignMode(_control))
            //    return ret;

            if (mapControlId == null)
                mapControlId = new Dictionary<UIElement, ElementTranslation>();
#if !WINDOWS_UWP
            if (mapControlTooltip == null)
                mapControlTooltip = new Dictionary<UIElement, ElementTranslation>();
#endif
            if (mapControlListId == null)
                mapControlListId = new Dictionary<UIElement, ElementTranslation>();

            if (mapIsTranslatable == null)
                mapIsTranslatable = new Dictionary<UIElement, bool>();

            if (!mapIsTranslatable.ContainsKey(_control))
            {
                var bNoTextAnimations = true;
                var name = GetEntityName(_control as FrameworkElement, bAdd: false);
                if (!String.IsNullOrEmpty(name) && MapScreenEntities.ContainsKey(name) && MapScreenEntities[name].Element == _control)
                {
                    var entity = MapScreenEntities[name];
                    if (entity != null && entity.ListAnimations != null)
                        bNoTextAnimations = (from c in entity.ListAnimations.AsParallel() where c is TextAnimation select c).ToList().Count == 0;
                }
                mapIsTranslatable[_control] = bNoTextAnimations;
            }
            if (!mapIsTranslatable[_control])
                return ret;

#if !WINDOWS_UWP
            if (_control is FrameworkElement && (_control as FrameworkElement).ToolTip is String)
            {
                FrameworkElement control = _control as FrameworkElement;
                bool bValid = true;
                if (!mapControlTooltip.ContainsKey(control))
                {
                    var content = control.ToolTip as String;
                    if (String.IsNullOrEmpty(content))
                        bValid = false;
                    else
                        mapControlTooltip.Add(control, new ElementTranslation(content));
                }

                if (bValid)
                {
                    if (bUntranslate)
                    {
                        var text = mapControlTooltip[control].untranslated;
                        if (control.ToolTip as String != text)
                            control.ToolTip = text;
                    }
                    else
                    {
                        var _text = mapControlTooltip[control].untranslated;
                        var _replaced = TranslationHelpers.TranslationHelper.TranslateComposedText(_text, map, _text);
                        if (_text != _replaced || control.ToolTip as String != _replaced)
                        {
                            control.ToolTip = _replaced;
                            mapControlTooltip[control].translated = _replaced;
                            ret.Add(control);
                        }
                    }
                    mapControlTooltip[control].isTranslated = !bUntranslate;
                }
            }
#endif
            if (_control is ContentControl && (_control as ContentControl).Content is String)
            {
                ContentControl control = _control as ContentControl;
                bool bValid = true;
                if (!mapControlId.ContainsKey(control))
                {
                    var content = (control as ContentControl).Content as String;
                    if (String.IsNullOrEmpty(content))
                        bValid = false;
                    else
                        mapControlId.Add(control, new ElementTranslation(content));
                }

                if (bValid)
                {
                    if (bUntranslate)
                    {
                        var text = mapControlId[control].untranslated;
                        if ((control as ContentControl).Content as String != text)
                            (control as ContentControl).Content = text;
                    }
                    else
                    {
                        var _text = mapControlId[control].untranslated;
                        var _replaced = TranslationHelpers.TranslationHelper.TranslateComposedText(_text, map, _text);
                        if (_text != _replaced || (control as ContentControl).Content as String != _replaced)
                        {
                            (control as ContentControl).Content = _replaced;
                            mapControlId[control].translated = _replaced;
                            ret.Add(control);
                        }
                    }
                    mapControlId[control].isTranslated = !bUntranslate;
                }
            }
            if (_control is TextBox && (_control as TextBox).IsReadOnly == true)
            {
                TextBox control = _control as TextBox;
                if (!mapControlId.ContainsKey(control))
                    mapControlId.Add(control, new ElementTranslation(control.Text));

                if (bUntranslate)
                {
                    var text = mapControlId[control].untranslated;
                    if (control.Text != text)
                        control.Text = text;
                }
                else
                {
                    var _text = mapControlId[control].untranslated;
                    var _replaced = TranslationHelpers.TranslationHelper.TranslateComposedText(_text, map, _text);
                    if (_text != _replaced || control.Text != _replaced)
                    {
                        control.Text = _replaced;
                        mapControlId[control].translated = _replaced;
                        ret.Add(control);
                    }
                }
                mapControlId[control].isTranslated = !bUntranslate;
            }
            if (_control is TextBlock)
            {
                TextBlock control = _control as TextBlock;
                if (!mapControlId.ContainsKey(control))
                    mapControlId.Add(control, new ElementTranslation(control.Text));

                if (bUntranslate)
                {
                    var text = mapControlId[control].untranslated;
                    if (control.Text != text)
                        control.Text = text;
                }
                else
                {
                    var _text = mapControlId[control].untranslated;
                    var _replaced = TranslationHelpers.TranslationHelper.TranslateComposedText(_text, map, _text);
                    if (_text != _replaced || control.Text != _replaced)
                    {
                        control.Text = _replaced;
                        mapControlId[control].translated = _replaced;
                        ret.Add(control);
                    }
                }
                mapControlId[control].isTranslated = !bUntranslate;
            }
#if !WINDOWS_UWP
            if (_control is HeaderedContentControl && (_control as HeaderedContentControl).Header is String)
            {
                HeaderedContentControl control = _control as HeaderedContentControl;
                if (!mapControlId.ContainsKey(control))
                    mapControlId.Add(control, new ElementTranslation(control.Header as String));

                if (bUntranslate)
                {
                    var text = mapControlId[control].untranslated;
                    if (control.Header as String != text)
                        control.Header = text;
                }
                else
                {
                    var _text = mapControlId[control].untranslated;
                    var _replaced = TranslationHelpers.TranslationHelper.TranslateComposedText(_text, map, _text);
                    if (_text != _replaced || control.Header as String != _replaced)
                    {
                        control.Header = _replaced;
                        mapControlId[control].translated = _replaced;
                        ret.Add(control);
                    }
                }
                mapControlId[control].isTranslated = !bUntranslate;
            }
#endif
            Action<bool> action = null;
            if ((_control is ListBox || _control is ComboBox) && !DesignerProperties.GetIsInDesignMode(_control))
            {
                Selector control = _control as Selector;
                action = new Action<bool>((bInit) =>
                {
                    if (control.ItemsSource != null)
                    {
                        var list = (from c in (control.ItemsSource as IEnumerable).OfType<String>() select c).ToList();
                        if (list.Count > 0)
                        {
                            if (!mapControlListId.ContainsKey(control))
                                mapControlListId.Add(control, new ElementTranslation(list));
                            else if (!bInit)
                            {
                                foreach (var id in list)
                                {
                                    if (!mapControlListId[control].translatedStrings.Contains(id) &&
                                        !mapControlListId[control].untranslatedStrings.Contains(id))
                                    {
                                        bInit = true;
                                        mapControlListId.Remove(control);
                                        mapControlListId.Add(control, new ElementTranslation(list));
                                        break;
                                    }
                                }
                            }

                            var newSource = new List<String>();
                            mapControlListId[control].untranslatedStrings.ForEach(id =>
                            {
                                if (map.ContainsKey(id) &&
                                    !String.IsNullOrEmpty(map[id]))
                                    newSource.Add(map[id]);
                                else
                                    newSource.Add(id);
                            });
                            if (bInit)
                                mapControlListId[control].translatedStrings = new List<string>(newSource);
                            
                            var oldSource = control.ItemsSource as IEnumerable<String>;
                            if (oldSource == null || !oldSource.SequenceEqual(newSource))
                            {
                                var index = control.SelectedIndex;
                                var isSyncronized = control.IsSynchronizedWithCurrentItem;
                                control.BeginInit();
                                control.IsSynchronizedWithCurrentItem = false;
                                control.ItemsSource = newSource;
                                control.IsSynchronizedWithCurrentItem = isSyncronized;
                                control.SelectedIndex = index;
                                control.EndInit();

                                ret.Add(control);
                            }
                        }
                    }
                    if (mapControlListId.ContainsKey(control))
                        mapControlListId[control].isTranslated = bInit;
                });
            }

            if (action != null)
            {
                if (mapItemSourceAction == null)
                    mapItemSourceAction = new Dictionary<UIElement, Action<bool>>();
                mapItemSourceAction[_control] = action;

                action(!bUntranslate);
                if (mapItemSourceChangeNotifier == null)
                    mapItemSourceChangeNotifier = new Dictionary<UIElement, PropertyChangeNotifier>();
                if (!mapItemSourceChangeNotifier.ContainsKey(_control))
                {
                    var name = GetEntityName(_control as FrameworkElement, bAdd: false);
                    if (!String.IsNullOrEmpty(name) && MapScreenEntities.ContainsKey(name) && MapScreenEntities[name].Element == _control)
                    {
                        var notifier = new PropertyChangeNotifier(_control, ItemsControl.ItemsSourceProperty);
                        mapItemSourceChangeNotifier.Add(_control, notifier);
                        notifier.ValueChanged += (o, e) =>
                        {
                            if (mapItemSourceDispatcherOperation == null)
                                mapItemSourceDispatcherOperation = new Dictionary<UIElement, DispatcherOperation>();

                            DispatcherOperation operation = null;
                            if (mapItemSourceDispatcherOperation.ContainsKey(_control))
                                operation = mapItemSourceDispatcherOperation[_control];

                            if (operation == null ||
                                operation.Status == DispatcherOperationStatus.Aborted ||
                                operation.Status == DispatcherOperationStatus.Completed)
                            {
                                mapItemSourceDispatcherOperation[_control] = _control.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                {
                                    if (mapItemSourceAction.ContainsKey(_control))
                                        mapItemSourceAction[_control](false);
                                });
                            }
                        };
                    }
                    else
                        mapItemSourceChangeNotifier.Add(_control, null);
                }
            }
            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Tuple<string, string> GetStringID(UIElement _control, string propertyName)
        {
            if (propertyName == FrameworkElement.ToolTipProperty.Name && _control is FrameworkElement && ((_control as FrameworkElement).ToolTip is String || (_control as FrameworkElement).ToolTip == null))
            {
                if (mapControlTooltip != null && mapControlTooltip.ContainsKey(_control))
                    return new Tuple<string, string>(FrameworkElement.ToolTipProperty.Name, mapControlTooltip[_control].untranslated);
                else
                    return new Tuple<string, string>(FrameworkElement.ToolTipProperty.Name, (_control as FrameworkElement).ToolTip as String);
            }
#if !WINDOWS_UWP
            if (_control is HeaderedContentControl && ((_control as HeaderedContentControl).Header is String || (_control as HeaderedContentControl).Header == null))
            {
                if (mapControlId != null && mapControlId.ContainsKey(_control))
                    return new Tuple<string, string>(HeaderedContentControl.HeaderProperty.Name, mapControlId[_control as HeaderedContentControl].untranslated);
                else
                    return new Tuple<string, string>(HeaderedContentControl.HeaderProperty.Name, (_control as HeaderedContentControl).Header as String);
            }
#endif
            if (_control is ContentControl && ((_control as ContentControl).Content is String))
            {
                if (mapControlId != null && mapControlId.ContainsKey(_control))
                    return new Tuple<string, string>(ContentControl.ContentProperty.Name, mapControlId[_control].untranslated);
                else
                    return new Tuple<string, string>(ContentControl.ContentProperty.Name, (_control as ContentControl).Content as String);
            }
            if (_control is TextBox && (_control as TextBox).IsReadOnly == true)
            {
                if (mapControlId != null && mapControlId.ContainsKey(_control))
                    return new Tuple<string, string>(TextBox.TextProperty.Name, mapControlId[_control].untranslated);
                else
                    return new Tuple<string, string>(TextBox.TextProperty.Name, (_control as TextBox).Text);
            }
            if (_control is TextBlock)
            {
                if (mapControlId != null && mapControlId.ContainsKey(_control))
                    return new Tuple<string, string>(TextBlock.TextProperty.Name, mapControlId[_control as TextBlock].untranslated);
                else
                    return new Tuple<string, string>(TextBlock.TextProperty.Name, (_control as TextBlock).Text);
            }
            return null;
        }

        public void ClearTranslationMaps(UIElement el, bool bClearTexts = true, bool bClearTooltips = true)
        {
            if (el == null)
                return;

            UIElement screenElement = el;
            List<UIElement> screenElements = new List<UIElement>();
            if (Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
            {
                if (el is ContentControl && (el as ContentControl).Content is String && el.Visibility == Visibility.Visible)
                    screenElement = el;
                else
                {
                    screenElements = (from c in el.GetVisualChildrenOfType<ContentControl>()
                                     where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                     select c as UIElement).ToList();

                    screenElements.AddRange((from c in el.GetVisualChildrenOfType<TextBox>()
                                      where c.IsReadOnly && c.Visibility == System.Windows.Visibility.Visible
                                      select c as UIElement).ToList());

                    screenElements.AddRange((from c in el.GetVisualChildrenOfType<TextBlock>()
                                             where c.Visibility == System.Windows.Visibility.Visible
                                             select c as UIElement).ToList());
#if !WINDOWS_UWP
                    screenElements.AddRange((from c in el.GetVisualChildrenOfType<HeaderedContentControl>()
                                             where c.Header is String && c.Visibility == System.Windows.Visibility.Visible
                                             select c as UIElement).ToList());
#endif
                }
            }
                var tooltipSubject = screenElement ?? el;

            if (bClearTooltips)
            {
                if (mapControlTooltip != null && mapControlTooltip.ContainsKey(tooltipSubject))
                    mapControlTooltip.Remove(tooltipSubject);
            }

            if(bClearTexts)
            {
                if (screenElement != null)
                {
                    if (mapControlId != null && mapControlId.ContainsKey(screenElement))
                        mapControlId.Remove(screenElement);

                    if (mapControlListId != null && mapControlListId.ContainsKey(screenElement))
                        mapControlListId.Remove(screenElement);
                }

                screenElements.ForEach(sel =>
                {
                    if (mapControlId != null && mapControlId.ContainsKey(sel))
                        mapControlId.Remove(sel);

                    if (mapControlListId != null && mapControlListId.ContainsKey(sel))
                        mapControlListId.Remove(sel);
                });
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<UIElement> GetDynControls(FrameworkElement mainControl, IDictionary<String, String> map, List<UserControl> embedViews)
        {
            var ret = new List<UIElement>();

            if (map != null && map.Count != 0)
                ret = GetDynControlsList(mainControl, embedViews);

            return ret;
        }

        public List<UIElement> GetDynControlsList(FrameworkElement mainControl, List<UserControl> embedViews = null)
        {
            var ret = new List<UIElement>();
            
            using (var cursor = new WaitCursor())
            {
                if (embedViews == null)
                    embedViews = new List<UserControl>();
#if !WINDOWS_UWP
                var listChildTooltip = (from c in mainControl.GetChildrenOfType<FrameworkElement>() where c.ToolTip is String && (from v in embedViews where v.GetChildrenOfType<FrameworkElement>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                listChildTooltip.ForEach(control =>
                {
                    var content = control.ToolTip as String;
                    if (!string.IsNullOrEmpty(content))
                        ret.Add(control);
                });
#endif
                var listControls = mainControl.GetChildrenOfType<ContentControl>().ToList();
                if (mainControl is ContentControl)
                    listControls.Add(mainControl as ContentControl);
                var listControlsText = (from c in listControls where c.Content is String && (from v in embedViews where v.GetChildrenOfType<ContentControl>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                listControlsText.ForEach(control =>
                {
                    var content = control.Content as String;
                    if (!String.IsNullOrEmpty(content))
                        ret.Add(control);
                });

                var listTextBoxes = (from c in mainControl.GetChildrenOfType<TextBox>().ToList() where c.IsReadOnly == true && (from v in embedViews where v.GetChildrenOfType<TextBox>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                if (mainControl is TextBox)
                    listTextBoxes.Add(mainControl as TextBox);
                listTextBoxes.ForEach(control =>
                {
                    ret.Add(control);
                });

                var listTextBlockes = (from c in mainControl.GetChildrenOfType<TextBlock>().ToList() where (from v in embedViews where v.GetChildrenOfType<TextBlock>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                if (mainControl is TextBlock)
                    listTextBlockes.Add(mainControl as TextBlock);
                listTextBlockes.ForEach(control =>
                {
                    ret.Add(control);
                });
#if !WINDOWS_UWP
                var listHeaderControls = (from c in mainControl.GetChildrenOfType<HeaderedContentControl>().ToList() where (from v in embedViews where v.GetChildrenOfType<HeaderedContentControl>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                if (mainControl is HeaderedContentControl)
                    listHeaderControls.Add(mainControl as HeaderedContentControl);
                var listHeaderContentControls = (from c in listHeaderControls where c.Header is String select c).ToList();
                listHeaderContentControls.ForEach(control =>
                {
                    ret.Add(control);
                });
#endif
                var listComboBoxBlockes = (from c in mainControl.GetChildrenOfType<ComboBox>().ToList() where (from v in embedViews where v.GetChildrenOfType<ComboBox>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                if (mainControl is ComboBox)
                    listComboBoxBlockes.Add(mainControl as ComboBox);
                ret.AddRange(listComboBoxBlockes);
                //listComboBoxBlockes.ForEach(control =>
                //{
                //    if (control.ItemsSource != null)
                //        ret.Add(control);
                //});

                var listListBoxBlockes = (from c in mainControl.GetChildrenOfType<ListBox>().ToList() where (from v in embedViews where v.GetChildrenOfType<ListBox>().Contains(c) select v).FirstOrDefault() == null select c).ToList();
                if (mainControl is ListBox)
                    listListBoxBlockes.Add(mainControl as ListBox);
                ret.AddRange(listListBoxBlockes);
                //listListBoxBlockes.ForEach(control =>
                //{
                //    if (control.ItemsSource != null)
                //        ret.Add(control);
                //});

                if (mainControl.ToolTip is String && !ret.Contains(mainControl))
                    ret.Add(mainControl);
            }
            return ret;
        }
#endif
#region IEntityReference
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get 
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

#endregion

#region IScriptable Members

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        String IScriptable.Name
        {
            get
            {
                return Title;
            }
        }

        [Browsable(false)]
        public String Code
        {
            get
            {
                return sCode;
            }
            set
            {
                if (String.Compare(value, sCode, false) == 0)
                    return;

                sCode = value;
                NeedsSave = true;
                bRecalculateListScriptVariableUsed = true;
                OnPropertyChanged("sCode");
            }
        }

        [Browsable(false)]
        public int[] Breakpoints
        {
            get
            {
                return breakpoints;
            }
            set
            {
                var arraysAreEqual = breakpoints != null && Enumerable.SequenceEqual(value, breakpoints);
                if (arraysAreEqual)
                    return;
                NeedsSave = true;
                breakpoints = value;
                OnPropertyChanged("Breakpoints");
            }
        }

        [Browsable(false)]
        public bool CanReadMacro
        {
            get
            {
                return true;
            }
        }

        [Browsable(false)]
        public bool CanEdit
        {
            get
            {
                return true;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetQuickReferenceList()
        {
            var list = new List<Object>();
            list.Add(this);

            var parent = Parent;
            while (parent != null && parent.GetType() == GetType())
                parent = parent.Parent;
            if (parent == null)
                parent = Parent;
            if (parent != null)
                list.Add(parent);
            return list;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetReferenceList(CRMapsHeler cRMapsHeler = null)
        {
            if (cRMapsHeler == null)
                return GetReferenceList();

            var list = new List<Object>();

            list.AddRange(GetQuickReferenceList() as List<object>);

            if (UseIntelliSense && cRMapsHeler.DocDispatcherReferenceList != null)
                list.AddRange(cRMapsHeler.DocDispatcherReferenceList);

            return list;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetReferenceList()
        {
            var list = new List<Object>();
            list.Add(this);

            var parent = Parent;
            while (parent != null && parent.GetType() == GetType())
                parent = parent.Parent;
            if (parent == null)
                parent = Parent;
            if (parent != null)
                list.Add(parent);

            if (UseIntelliSense)
            {
                var variableDispatcher = GetVariableObjectDispatcher(this);
                variableDispatcher.ForEach(dispatcher =>
                {
                    list.Add(dispatcher);
                });
            }
            return list;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Object> GetQuickVariableObjectDispatcherList(List<String> tagList, IDictionary<String, String> mapPrototypes, IDictionary<String, IList<String>> mapDefinitions)
        {
            var list = new List<Object>();

            var variableDispatcher = GetVariableObjectDispatcher(tagList, mapPrototypes, mapDefinitions);
            variableDispatcher.ForEach(dispatcher =>
            {
                list.Add(dispatcher);
            });

            return list;
        }
        internal List<VariableValues> GetVariableObjectDispatcher(List<String> tagList, IDictionary<String, String> mapPrototypes, IDictionary<String, IList<String>> mapDefinitions)
        {
            var ret = new List<VariableValues>();

            if (variableValues == null)
                variableValues = new List<VariableValues>();
            variableValues.ForEach(value =>
            {
                value.Dispose();
            });
            variableValues.Clear();

            var varvalue = new VariableValues(false);
            variableValues.Add(varvalue);

            //varvalue.PrepareAddNewVariable();
            if (tagList != null)
                varvalue.AddList(tagList);
            //varvalue.EndAddNewVariable(false);
            ret.Add(varvalue);

            if (mapPrototypes != null && mapDefinitions != null)
            {
                foreach (var instance in mapPrototypes.Keys)
                {
                    if (mapPrototypes[instance] == null || !mapDefinitions.ContainsKey(mapPrototypes[instance]))
                        continue;
                    var prototypesValues = new VariableValues(instance, false);
                    variableValues.Add(prototypesValues);
                    prototypesValues.PrepareAddNewVariable();

                    foreach (var member in mapDefinitions[mapPrototypes[instance]])
                    {
                        prototypesValues.AddVariable(member);
                        prototypesValues.AddVariable(String.Format(qualityTag, member), false, true);
                        prototypesValues.AddVariable(String.Format(timestampTag, member), false, true);
                    }
                    prototypesValues.EndAddNewVariable(false);
                    ret.Add(prototypesValues);
                }
            }

            var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            listDataSinkInterfaces.ForEach(dataInterface =>
            {
                var prototypesValues = new VariableValues(dataInterface, false);
                variableValues.Add(prototypesValues);
                //prototypesValues.PrepareAddNewVariable();

                var data = OPCUAEntityReference.GetDataSinkInterface(dataInterface);
                foreach (var member in data.GetVariables(this))
                {
                    var _member = member.Replace('&', '\\');

                    prototypesValues.AddVariable(_member);
                    prototypesValues.AddVariable(String.Format(qualityTag, _member), false, true);
                    prototypesValues.AddVariable(String.Format(timestampTag, _member), false, true);
                }
                //prototypesValues.EndAddNewVariable(false);
                prototypesValues.isDataService = true;
                ret.Add(prototypesValues);
            });

            return ret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetReferenceName(Object var)
        {
            if (var is VariableValues)
            {
                var ret = var as VariableValues;
                var name = ret.GetName();
                if (String.IsNullOrEmpty(name))
                    return "%";
                else
                    return String.Format("%{0}.", name);
            }

            //String title = var.GetType().ToString().Replace('.', '_');
            //if (var == entity)
            //{
            //    UIElement ui = entity as UIElement;
            //    if (ui is FrameworkElement)
            //    {
            //        var child = ui as FrameworkElement;
            //        if (String.IsNullOrEmpty(child.Name) && child.Uid is String)
            //            title = child.Uid as String;
            //        else
            //            title = ui is FrameworkElement && !(String.IsNullOrEmpty((ui as FrameworkElement).Name)) ? (ui as FrameworkElement).Name : ui.DependencyObjectType.Name;
            //    }
            //}

            if (var == this)
                return "Document";
            return "Parent";
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetListProcedures(IList<String> list)
        {

        }

        public event EventHandler ScriptLoaded;
        void OnScriptLoaded()
        {
            var t = ScriptLoaded;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler ScreenLoaded;
        public void OnScreenLoaded()
        {
            var t = ScreenLoaded;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler ScreenUnloaded;
        public void OnScreenUnloaded()
        {
            var t = ScreenUnloaded;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler Activated;
        public void OnActivated()
        {
            var t = Activated;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler Deactivated;
        public void OnDeactivated()
        {
            var t = Deactivated;
            if (t != null)
                t(this, EventArgs.Empty);
        }
#endif
#endregion

#region ScriptCode
#if !WINDOWS_UWP && !NET_STANDARD
        BasicNoUIObj basicCtl;
        bool bDontRaiseSecondError;
        bool bWindowCreated;
        public void ExecuteScriptCode()
        {
            if (!String.IsNullOrEmpty(Code))
            {
#if !DEBUG
                //if (bIsBlindServer)
                //    return;

                var enableVB = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxN+rOYP4u2+aLWnhBhIXJ4A=="/* VB */);
                if (enableVB == false)
                {
                    //logLicense.Warn(Properties.Resources.NoVBLicense);
                    if(iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(this, Properties.Resources.LicenseManager, 
                        DateTime.UtcNow, Properties.Resources.NoVBLicense, 
                        System.Diagnostics.EventLogEntryType.Warning);
                   return;
                }
#endif

                var bIsOnlyRuntime = ApplicationPropertiesHelper.GetProperty<bool>("IsOnlyRuntime", false);

                bDontRaiseSecondError = false;
                if (Environment.UserInteractive && !bIsOnlyRuntime && Breakpoints != null && Breakpoints.Length > 0 && !bIsBlindServer)
                    basicCtl = new BasicIdeObj();
                else
                    basicCtl = new BasicNoUIObj();

#if DEBUG
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
                basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

                basicCtl.Initialize();
                basicCtl.Caption = Title;
                basicCtl.LargeIcon = null;
                basicCtl.SmallIcon = null;
                basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;

                if (bIsBlindServer)
                    Util.IgnoreDialogs = true;

                basicCtl.AttachToWindow(null, ManageConstants.Disconnecting);

                if (syslog == null)
                    syslog = LogManager.GetLogger(String.Format(Properties.Resources.ScriptScreen, Title));

                // basicCtl.AttachToWindow(Window.GetWindow(Entity), ManageConstants.All);

                //basicCtl.OverrideModalWindowOwner += (o, e) =>
                //{
                //    e.OwnerHandle = new WindowInteropHelper(Window.GetWindow(Entity)).Handle;
                //};
                basicCtl.ErrorAlert += (o, e) =>
                {
                    var error = basicCtl.Error.ToString();
                    if (!bDontRaiseSecondError)
                    {
                        bDontRaiseSecondError = true;
                        if (basicCtl is BasicIdeObj)
                        {
                            bWindowCreated = true;
                            var ctrl = basicCtl as BasicIdeObj;
                            ctrl.CreateOverlappedWindow();
                            ctrl.WindowState = WindowState.Maximized;
                            ctrl.ActivateWindow();
                            basicCtl.Run = true;
                        }
                        else
                            basicCtl.Run = false;

                        var UIMsgBox = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (UIMsgBox != null)
                            UIMsgBox.ShowError(error);
                    }
                    syslog.Error(error);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(this, String.Format(Properties.Resources.ScriptScreen, Title),
                        DateTime.UtcNow, error,
                        System.Diagnostics.EventLogEntryType.Error);
                };
                basicCtl.DebugPrint += (o, e) =>
                {
                    syslog.Debug(e.Text);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(this, String.Format(Properties.Resources.ScriptScreen, Title),
                        DateTime.UtcNow, e.Text,
                        System.Diagnostics.EventLogEntryType.Information);
                };
                basicCtl.DoEvents += (o, e) =>
                {
                    if (bWindowCreated)
                        WaitForPriority.DoEvents();
                };
                basicCtl.ReadMacro += (o, e) =>
                {
                    if (e.FileName.StartsWith("*"))
                    {
                        var filename = e.FileName.Replace("*", "");
                        var scriptManager = GetService(typeof(IScriptManager)) as IScriptManager;
                        if (scriptManager != null)
                        {
                            e.Code = scriptManager.GetScriptCode(this, filename);
                            e.Changed = true;
                            e.Cancel = false;
                        };
                    }
                };

                //Assembly wpfCoreAssembly = typeof(CommandBinding).Assembly;
                //basicCtl.AddExtension("#", wpfCoreAssembly);
                //Assembly wpfFrameworkAssembly = typeof(Window).Assembly;
                //basicCtl.AddExtension("#", wpfFrameworkAssembly);

                var opcua = typeof(Opc.Ua.DataValue).Assembly;
                basicCtl.AddExtension("#", opcua);

                basicCtl.AddExtension("$Feature ExtensionCache False", null);
                foreach (var reference in GetReferenceList())
                {
                    var referenceGetTypeAssembly = reference.GetType().Assembly;
                    var referenceName = GetReferenceName(reference);

                    if (referenceName.StartsWith("%"))
                        basicCtl.AddExtension(referenceName, reference);
                    else
                    {
                        basicCtl.AddExtension("#", referenceGetTypeAssembly);
                        basicCtl.AddExtensionObjectWithEvents(referenceName, reference);
                    }
                }
                basicCtl.FileTools = false;
                basicCtl.EventMode = true;
                basicCtl.Code = Code;
                if (Environment.UserInteractive && !bIsOnlyRuntime && Breakpoints != null && Breakpoints.Length > 0)
                {
                    var ctrl = basicCtl as BasicIdeObj;
                    ctrl.CreateOverlappedWindow();
                    ctrl.WindowState = WindowState.Maximized;
                    ctrl.BreakPoints = Breakpoints;
                    ctrl.ActivateWindow();
                    bWindowCreated = true;
                }
                basicCtl.Changed = false;
                basicCtl.Run = true;

                OnScriptLoaded();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TerminateScriptCode()
        {
            var bRet = true;
            foreach (var name in MapScreenEntities.Keys)
            {
                // var uie = parent.FindName(name) as UIElement;
                var entity = MapScreenEntities[name];
                bRet &= entity.TerminateScriptCode();
            }

            if (basicCtl == null)
                return bRet;

            try
            {
                basicCtl.Run = false;
                var ret = basicCtl.Shutdown();
                if (ret < 0)
                    return false;

                basicCtl.Disconnect();
                basicCtl.Dispose();
            }
            catch { }

            basicCtl = null;
            return bRet;
        }

        public void OpenScreenNormal(String name, String parameterFile = null)
        {
            if (!IsOnBlindServer())
                throw new Exception("This method is supported only on blid server, please use the Screen interface");

            var documentManager = Parent.GetService(typeof(IScreenManager)) as IDocumentManager;

            var file = String.Format("{0}\\{1}{2}", documentManager.TypeLabel, name, documentManager.FileType);
            var newCommand = new OpenScreenCommandRemoteExecute()
            {
                executionMode = ExecutionMode.Normal,
                uri = new Uri(file, UriKind.RelativeOrAbsolute)
            };
            if (parameterFile != null)
                newCommand.ParameterFile = new Uri(parameterFile, UriKind.RelativeOrAbsolute);

            lock (lockObject)
            {
                if (pendingCommands == null)
                    pendingCommands = new List<RemoteExecute>();
                pendingCommands.Add(newCommand);
            }
        }
#endif
#endregion

#region INotifyPropertyVisibilityChanged Members
#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "RemoveDisabledItemAfterSecs" ||
                    propertyName == "MaxCleanCount" ||
                    propertyName == "UseAlwaysSecureConnections" ||
                    propertyName == "SlowSamplingInterval" ||
                    propertyName == "DisableWhenNotUsed" ||
                    propertyName == "PublishingInterval" ||
                    propertyName == "FastSamplingInterval")
                {
                    return !String.IsNullOrEmpty(SessionName);
                }
                else if (propertyName == "ResizeControlsOnFit")
                {
                    return FitInWindow == true;
                }
                else if (propertyName == "GeoScada")
                {
                    IWorkspace workspace = this.GetService(typeof(IWorkspace)) as IWorkspace;
                    if(workspace != null)
                        return workspace.ContextDocument == this && ActiveView != null;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
#endif
        #endregion

    }

#if NET_STANDARD
public enum WindowState
    {
        //
        // Summary:
        //     The window is restored.
        Normal = 0,
        //
        // Summary:
        //     The window is minimized.
        Minimized = 1,
        //
        // Summary:
        //     The window is maximized.
        Maximized = 2
    }
    //
    // Summary:
    //     Specifies the type of border that a System.Windows.Window has. Used by the System.Windows.Window.WindowStyle
    //     property.
    public enum WindowStyle
    {
        //
        // Summary:
        //     Only the client area is visible - the title bar and border are not shown. A System.Windows.Navigation.NavigationWindow
        //     with a System.Windows.Window.WindowStyle of System.Windows.WindowStyle.None will
        //     still display the navigation user interface (UI).
        None = 0,
        //
        // Summary:
        //     A window with a single border. This is the default value.
        SingleBorderWindow = 1,
        //
        // Summary:
        //     A window with a 3-D border.
        ThreeDBorderWindow = 2,
        //
        // Summary:
        //     A fixed tool window.
        ToolWindow = 3
    }
#endif
}