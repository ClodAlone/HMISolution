using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
#if !NET_STANDARD
using Utilities.Animations;
using Utilities.WPF;
using ScriptVariableValues;
using WPFUtilities.Converters;
using UIMsgBoxAlertService.ComponentService;
using ScreenSettings.Documents;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using UFInterfaces.Animatable;
using UFInterfaces.Commandable;
using UFInterfaces.Scriptable;
using System.Windows.Threading;
using WinWrap.Basic;
using System.Windows.Interop;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using ScreenSettings.Adorners;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using Converters;
using System.Windows.Media.Media3D;
using DevExpress.Xpf.WindowsUI;
using UFUAEditor.ComponentService;
using ScriptManager.ComponentService;
using UFInterfaces.PropertyControl;
#endif
using DataReader;
using log4net;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif
using UFInterfaces.AuthenticationCredentialsProvider;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using UFInterfaces.Constants;
using CommandManager;
using AnimationManager;
using System.Reflection;
using OPCUAViewModel;
using UFInterfaces;
using Utilities;
using ViewModelLib;
using DocumentManager.ComponentService;
using System.Threading.Tasks;
using System.Dynamic;
using WPFUtilities;
using System.Xml;
using Utilities.Converters;
using System.Threading;
using UFProjectManager.ComponentService;
using ExpressionManager;

namespace ScreenSettings.Entities
{
    [CollectionDataContract
                (Name = "ProblematicXamlWriterProperties",
                ItemName = "entry")]
    public class ProblematicXamlWriterProperties : Dictionary<String, String> 
    {
    }

#if !WINDOWS_UWP && !NET_STANDARD
    public class CameraTranforms
    {
        public CameraTranforms()
        {
            AnimationTime = 1000;
        }

        public String Name { get; set; }
        public ScaleTransform3D Scale { get; set; }
        public AxisAngleRotation3D Rotation { get; set; }
        public TranslateTransform TranslateTransform2D { get; set; }

        public int AnimationTime { get; set; }
    }

    internal class UnitConverterSettings
    {
        public String Unitlabel { get; set; }
        public String InputExpression { get; set; }
        public String OutputExpression { get; set; }
    }
#endif

    [DataContract(Name = "ScreenEntity", Namespace = Namespaces.UriProgea)]

    public class ScreenEntity : IEntityReference
#if !WINDOWS_UWP && !NET_STANDARD
        , UFInterfaces.Animatable.IAnimatable, ICommandable, IScriptable, ICloneable, IDataErrorInfo, INotifyPropertyChanged, INotifyPropertyVisibilityChanged
#endif
    {
#region Declarations
#if !NET_STANDARD
        PropertyObserver<OPCUAEntityReference> observer;
#if !WINDOWS_UWP
        PropertyObserver<OPCUAEntityReference> observer3DZoom;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel3DZoom;
#endif
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        PropertyObserver<SessionViewModel> observerSessionModel;
        ExpressionEntity expressionEntity;
        ExpressionEntity expressionUnitConverter;
        UnitConverterSettings lastUnitConverterSettings;
        String SessionName;
        bool bExecuted;
#if !WINDOWS_UWP
        bool bIsBlindServer;
#endif

        string lastExpressionError;
        IUFProjectManager iUFProjectManager;
        bool bInvisibleForSecurity;
        bool bInvisibleForZoom;
        internal bool BrushAndPenTagResolved;
#endif

        ScreenDocument document;

#if !NET_STANDARD
        Dictionary<String, List<OPCUAEntityReference>> mapItemsToBeResolved;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool bUpdatingProperty;
#endif

        ILog syslog;
#if !WINDOWS_UWP && !NET_STANDARD
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        ILog entitylog;
        string entityName;
#endif
#endregion

#region Pesistent Declarations

#if !WINDOWS_UWP
#if !NET_STANDARD
        [DataMember]
        String sCode;
        [DataMember]
        int[] breakpoints;
#endif
        [DataMember]
        DataReaderModel readerItemSources;
#endif
        [DataMember]
        String listItemSources;
        [DataMember]
        bool accessLevelFromTag;
        [DataMember]
        int accessLevel;
        [DataMember]
        String accessRole;
        [DataMember]
        int readableAccessMask;
        [DataMember]
        int writableAccessMask;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        int nStartSel;
        [DataMember]
        int nSelLength;
#endif
        [DataMember]
        String sStyleResource;
        //[DataMember]
        //String sBrushResource;
        //[DataMember]
        //String sPenResource;
        [DataMember]
        byte tagDecorators;
        [DataMember]
        String tagBrush;
        [DataMember]
        String tagPen;
        [DataMember]
        Guid id;

        [DataMember]
        String sProblematicXaml;
        [DataMember]
        OPCUAEntityReference opcuaEntityReference;
        [DataMember]
        List<OPCUAEntityReference> listopcuaSmartEntityReference;
        [DataMember]
        CommandManagerList listCommands;
        [DataMember]
        AnimationManagerList listAnimations;
        [DataMember]
        bool bEnableManipulation;
        [DataMember]
        bool bEnableMouseOver;
        [DataMember]
        bool bVisibleOnClient = true;
        [DataMember]
        bool bForceDynamicOnClient;
        [DataMember]
        double zoomLevelVisibilityX = 0;
        [DataMember]
        double zoomLevelVisibilityY = 0;
        [DataMember]
        String sSourceSymbolProvider;
        [DataMember]
        String sSourceSymbolPath;
        [DataMember]
        bool bSourceSymbolLinked = false;
        [DataMember]
        bool bSourceSymbolLinkedPassive = false;
        [DataMember]
        ProblematicXamlWriterProperties mapProblematicXamlWriterProperties;
        [DataMember]
        String sExpression;
        [DataMember]
        String sReverseExpression;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        Dictionary<string, FontSettings> fontSettingList;
        [DataMember]
        Dictionary<String, ScreenEntity> mapHashInner3DEntities;
        [DataMember]
        List<CameraTranforms> listCameraTransforms;
        [DataMember]
        Uri screen3DUri;
        [DataMember]
        String screen3DParameter;
#endif
        [DataMember]
        Uri menuName;
        [DataMember]
        bool showMenuOnLeft;
        [DataMember]
        String speechCommand;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        double max3DRotationAngleX;
        [DataMember]
        double max3DRotationAngleY;
        [DataMember]
        double max3DRotationAngleZ;
        [DataMember]
        double min3DRotationAngleX;
        [DataMember]
        double min3DRotationAngleY;
        [DataMember]
        double min3DRotationAngleZ;
        [DataMember]
        double max3DTranslateOffsetX;
        [DataMember]
        double min3DTranslateOffsetX;
        [DataMember]
        double max3DTranslateOffsetY;
        [DataMember]
        double min3DTranslateOffsetY;
        [DataMember]
        double max3DZoom;
        [DataMember]
        double min3DZoom;
        [DataMember]
        OPCUAEntityReference opcuaEntityReference3DZoom;
#endif
        [DataMember]
        int visibilityLevel = Int32.MaxValue;
        [DataMember]
        bool lockMovement;

        [DataMember]
        bool preserveFontSettingList;
        [DataMember]
        bool preserveStyle;
        [DataMember]
        bool preserveCode;
        [DataMember]
        bool preserveColors;
        [DataMember]
        bool preserveSize = true;
        [DataMember]
        bool preserveCommands;
        [DataMember]
        bool preserveAnimations;
        [DataMember]
        bool preserveMenu;
        [DataMember]
        bool preserveExpression;
        [DataMember]
        bool preserveSecurity;
        [DataMember]
        bool preserveVisibility;
        [DataMember]
        bool preserveVariables = true;
        [DataMember]
        bool preserveText = true;
        [DataMember]
        bool preserveCustomControlProperties = true;


        [DataMember]
        String preservedBrush;
        [DataMember]
        String preservedPen;
        [DataMember]
        String preservedText;
        [DataMember]
        double preservedWidth;
        [DataMember]
        double preservedHeight;

#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        String sIdUnitConverter;
        [DataMember]
        bool useIntelliSense = true;
#endif
        [DataMember]
        Dictionary<String, String> mapProps;

        [DataMember]
        bool bExecuteAnyEnabledCommands = true;
        [DataMember]
        bool bShowTooltipWhenDisabled;

        [DataMember]
        Dictionary<String, String> mapAlias;

#endregion

#region Constructors

        public ScreenEntity(ScreenEntity source)
        {
            if (source == null)
                return;

            CopyAll(source);
            id = Guid.NewGuid();
        }

#if !NET_STANDARD
        public ScreenEntity(FrameworkElement fe)
        {
            entity = fe;
            id = Guid.NewGuid();
        }
#endif

        public ScreenEntity()
        {
            id = Guid.NewGuid();
        }
#endregion

#region Methods

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            preserveSize = true;
            preserveVariables = true;
            preserveText = true;
            preserveCustomControlProperties = true;
#if !WINDOWS_UWP && !NET_STANDARD
            useIntelliSense = true;
#endif

            bExecuteAnyEnabledCommands = true;

            visibilityLevel = Int32.MaxValue;
            id = Guid.NewGuid();
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (sCode != null && !sCode.Contains('\r'))
                sCode = sCode.Replace("\n", Environment.NewLine);
#endif
            if (id == null || id == Guid.Empty)
                id = Guid.NewGuid();

#if !WINDOWS_UWP && !NET_STANDARD
            dLast3dScaleValue = 1;
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RenewUniqueId()
        {
            id = Guid.NewGuid();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Contains3DElement()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (Element == null)
                return false;
            var list3Ds = Element.GetChildrenOfType<Viewport3D>().ToList();
            if (Element is Viewport3D)
                list3Ds.Insert(0, Element as Viewport3D);
            if (list3Ds.Count > 0)
                return true;
#endif     
            return false;
        }

        public void CopyAll(ScreenEntity source)
        {
            CopyData(source);
            CopyDynamics(source);
        }

        public void CopyData(ScreenEntity source)
        {
            sStyleResource = source.sStyleResource;
            //sBrushResource = source.sBrushResource;
            //sPenResource = source.sPenResource;
            sProblematicXaml = source.sProblematicXaml;
            tagDecorators = source.tagDecorators;
            tagBrush = source.tagBrush;
            tagPen = source.tagPen;
            sSourceSymbolProvider = source.sSourceSymbolProvider;
            sSourceSymbolPath = source.sSourceSymbolPath;
            sExpression = source.sExpression;
            menuName = source.menuName;
            showMenuOnLeft = source.showMenuOnLeft;
#if !WINDOWS_UWP && !NET_STANDARD
            screen3DUri = source.screen3DUri;
            screen3DParameter = source.screen3DParameter;

            sIdUnitConverter = source.sIdUnitConverter;
#endif
            sReverseExpression = source.sReverseExpression;
            bSourceSymbolLinked = source.bSourceSymbolLinked;
            bSourceSymbolLinkedPassive = source.bSourceSymbolLinkedPassive;
            accessLevel = source.accessLevel;
            visibilityLevel = source.visibilityLevel;
            lockMovement = source.lockMovement;
            accessLevelFromTag = source.accessLevelFromTag;
            accessRole = source.accessRole;
            readableAccessMask = source.readableAccessMask;
            writableAccessMask = source.writableAccessMask;
            zoomLevelVisibilityX = source.zoomLevelVisibilityX;
            zoomLevelVisibilityY = source.zoomLevelVisibilityY;
            speechCommand = source.speechCommand;

            preserveFontSettingList = source.preserveFontSettingList;
            preserveStyle = source.preserveStyle;
            preserveCode = source.preserveCode;
            preserveColors = source.preserveColors;
            preserveSize = source.preserveSize;
            preserveCommands = source.preserveCommands;
            preserveAnimations = source.preserveAnimations;
            preserveMenu = source.preserveMenu;
            preserveExpression = source.preserveExpression;
            preserveSecurity = source.preserveSecurity;
            preserveVisibility = source.preserveVisibility;
            preserveVariables = source.preserveVariables;
            preserveText = source.preserveText;
            preserveCustomControlProperties = source.preserveCustomControlProperties;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public void CopyCode(ScreenEntity source)
        {
            sCode = source.sCode;
            breakpoints = source.breakpoints;
            nStartSel = source.nStartSel;
            nSelLength = source.nSelLength;
            useIntelliSense = source.useIntelliSense;
        }
        public void CopyFontSettingList(ScreenEntity source)
        {
            FontSettingList = source.FontSettingList;
        }
#endif       
        public void CopyStyle(ScreenEntity source)
        {
            StyleResource = source.StyleResource;
            SourceSymbolProvider = source.SourceSymbolProvider;
            SourceSymbolPath = source.SourceSymbolPath;
        }

        public void CopyMenu(ScreenEntity source)
        {
            menuName = source.menuName;
            showMenuOnLeft = source.showMenuOnLeft;
            speechCommand = source.speechCommand;
        }

        public void CopyExpressions(ScreenEntity source)
        {
            sExpression = source.sExpression;
            sReverseExpression = source.sReverseExpression;
        }

        public void CopySecurity(ScreenEntity source)
        {
            accessLevel = source.accessLevel;
            accessLevelFromTag = source.accessLevelFromTag;
            accessRole = source.accessRole;
            readableAccessMask = source.readableAccessMask;
            writableAccessMask = source.writableAccessMask;
        }

        public void CopyVisibility(ScreenEntity source)
        {
            visibilityLevel = source.visibilityLevel;
            zoomLevelVisibilityX = source.zoomLevelVisibilityX;
            zoomLevelVisibilityY = source.zoomLevelVisibilityY;
        }

        public void CopyTextDecorators(ScreenEntity source)
        {
            TagDecorators = source.TagDecorators;
        }

        public void CopyColors(ScreenEntity source)
        {
            TagBrush = source.TagBrush;
            TagPen = source.TagPen;
        }

        public void CopyVariables(ScreenEntity source)
        {
            if (source.opcuaEntityReference != null)
            {
                var sourceString = source.opcuaEntityReference.ToXml();
                opcuaEntityReference = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                opcuaEntityReference = null;

#if !WINDOWS_UWP && !NET_STANDARD
            if (source.opcuaEntityReference3DZoom != null)
            {
                var sourceString = source.opcuaEntityReference3DZoom.ToXml();
                opcuaEntityReference3DZoom = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                opcuaEntityReference3DZoom = null;
#endif
            if (source.listopcuaSmartEntityReference != null)
            {
                var sourceString = source.listopcuaSmartEntityReference.ToXml();
                listopcuaSmartEntityReference = sourceString.FromXml<List<OPCUAEntityReference>>();
            }
            else
                listopcuaSmartEntityReference = null;

#if !NET_STANDARD
            if (source.MapProblematicXamlWriterProperties != null && Element is IDynamicTagAware)
            {
                var sourceMap = source.MapProblematicXamlWriterProperties;

                var dyamicAware = Element as IDynamicTagAware;
                dyamicAware.PreserveTagsFromMap(sourceMap);
            }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
            sIdUnitConverter = source.sIdUnitConverter;
#endif
        }

        public void CopyAnimations(ScreenEntity source)
        {
            if (source.listAnimations != null)
            {
                var sourceString = source.listAnimations.ToXml();
                listAnimations = sourceString.FromXml<AnimationManagerList>();
            }
            else
                listAnimations = null;
        }

        public void CopyCommands(ScreenEntity source)
        {
            if (source.listCommands != null)
            {
                var sourceString = source.listCommands.ToXml();
                listCommands = sourceString.FromXml<CommandManagerList>();
            }
            else
                listCommands = null;

            bExecuteAnyEnabledCommands = source.bExecuteAnyEnabledCommands;
        }

        public void CopyAlias(ScreenEntity source)
        {
            if (source.mapAlias != null)
            {
                var sourceString = source.mapAlias.ToXml();
                mapAlias = sourceString.FromXml<Dictionary<String, String>>();
            }
            else
                mapAlias = null;
        }

        public void CopyCustomProperties(ScreenEntity source)
        {
            if (source.mapProblematicXamlWriterProperties != null)
            {
                var sourceString = source.mapProblematicXamlWriterProperties.ToXml();
                mapProblematicXamlWriterProperties = sourceString.FromXml<ProblematicXamlWriterProperties>();
            }
            else
                mapProblematicXamlWriterProperties = null;
        }        

        public void CopyDynamics(ScreenEntity source)
        {
#if !WINDOWS_UWP
#if !NET_STANDARD
            Code = source.sCode;
            breakpoints = source.breakpoints;
            nStartSel = source.nStartSel;
            nSelLength = source.nSelLength;
            useIntelliSense = source.useIntelliSense;
#endif
            readerItemSources = source.readerItemSources;
#endif
            bShowTooltipWhenDisabled = source.bShowTooltipWhenDisabled;

            listItemSources = source.listItemSources;
            if (source.opcuaEntityReference != null)
            {
                var sourceString = source.opcuaEntityReference.ToXml();
                opcuaEntityReference = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                opcuaEntityReference = null;

#if !WINDOWS_UWP && !NET_STANDARD
            if (source.opcuaEntityReference3DZoom != null)
            {
                var sourceString = source.opcuaEntityReference3DZoom.ToXml();
                opcuaEntityReference3DZoom = sourceString.FromXml<OPCUAEntityReference>();
            }
            else
                opcuaEntityReference3DZoom = null;

            if (source.fontSettingList != null)
            {
                var sourceString = source.fontSettingList.ToXml();
                fontSettingList = sourceString.FromXml<Dictionary<string, FontSettings>>();
            }
            else
                fontSettingList = null;
#endif
            if (source.listopcuaSmartEntityReference != null)
            {
                var sourceString = source.listopcuaSmartEntityReference.ToXml();
                listopcuaSmartEntityReference = sourceString.FromXml<List<OPCUAEntityReference>>();
            }
            else
                listopcuaSmartEntityReference = null;

            bEnableMouseOver = source.bEnableMouseOver;
            bEnableManipulation = source.bEnableManipulation;
            bEnableMouseOver = source.bEnableMouseOver;
            bVisibleOnClient = source.bVisibleOnClient;
            bForceDynamicOnClient = source.bForceDynamicOnClient;
            zoomLevelVisibilityX = source.zoomLevelVisibilityX;
            zoomLevelVisibilityY = source.zoomLevelVisibilityY;
            visibilityLevel = source.visibilityLevel;

            bExecuteAnyEnabledCommands = source.bExecuteAnyEnabledCommands;

            if (source.listCommands != null)
            {
                var sourceString = source.listCommands.ToXml();
                listCommands = sourceString.FromXml<CommandManagerList>();
            }
            else
                listCommands = null;

            if (source.listAnimations != null)
            {
                var sourceString = source.listAnimations.ToXml();
                listAnimations = sourceString.FromXml<AnimationManagerList>();
            }
            else
                listAnimations = null;

            if (source.mapAlias != null)
            {
                var sourceString = source.mapAlias.ToXml();
                mapAlias = sourceString.FromXml<Dictionary<String, String>>();
            }
            else
                mapAlias = null;

            if (source.mapProblematicXamlWriterProperties != null)
            {
                var sourceString = source.mapProblematicXamlWriterProperties.ToXml();
                mapProblematicXamlWriterProperties = sourceString.FromXml<ProblematicXamlWriterProperties>();
            }
            else
                mapProblematicXamlWriterProperties = null;

            if (source.mapProps != null)
            { 
                var sourceString = source.mapProps.ToXml();
                mapProps = sourceString.FromXml<Dictionary<String, String>>();
            }
            else
                mapProps = null;

#if !WINDOWS_UWP && !NET_STANDARD
            if (source.mapHashInner3DEntities != null)
            {
                var sourceString = source.mapHashInner3DEntities.ToXml();
                mapHashInner3DEntities = sourceString.FromXml<Dictionary<String, ScreenEntity>>();
            }
            else
                mapHashInner3DEntities = null;

            if (source.listCameraTransforms != null)
            {
                var sourceString = source.listCameraTransforms.ToXml();
                listCameraTransforms = sourceString.FromXml<List<CameraTranforms>>();
            }
            else
                listCameraTransforms = null;
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ScreenEntity AddOrFind3DInnerModel(Model3D model, String hash)
        {
            if (mapHashInner3DEntities == null)
                mapHashInner3DEntities = new Dictionary<String, ScreenEntity>();
            if (!mapHashInner3DEntities.ContainsKey(hash))
            {
                var entity = new ScreenEntity() { Entity3D = model };
                mapHashInner3DEntities.Add(hash, entity);
            }
            else
                mapHashInner3DEntities[hash].Entity3D = model;

            return mapHashInner3DEntities[hash];
        }

        void Init3DInnerEntities(FrameworkElement parent, FrameworkElement element,
                                        String sessionname, IDocument documentParent)
        {
            if (mapHashInner3DEntities == null)
                return;
            foreach (var hash in mapHashInner3DEntities.Keys)
            {
                mapHashInner3DEntities[hash].entity3D = (element as FrameworkElement).FindName(hash) as Model3D;
                if (mapHashInner3DEntities[hash].entity3D == null)
                    mapHashInner3DEntities[hash].entity3D = parent.FindName(hash) as Model3D;
                if (mapHashInner3DEntities[hash].entity3D == null && element is ContentControl)
                {
                    var contentControl = element as ContentControl;
                    if (contentControl.Content is FrameworkElement)
                        mapHashInner3DEntities[hash].entity3D = (contentControl.Content as FrameworkElement).FindName(hash) as Model3D;
                }

                if (mapHashInner3DEntities[hash].entity3D == null)
                    mapHashInner3DEntities[hash].entity3D = DependencyObjectExtensions.GetMatchingModel3D(element, hash);

                if (mapHashInner3DEntities[hash].entity3D != null)
                {
                    mapHashInner3DEntities[hash].Entity = element;
                    var listAnimation = mapHashInner3DEntities[hash].AnimationList as AnimationManagerList;

                    string xml = null;
                    if (OpcuaEntityReference != null)
                        xml = OpcuaEntityReference.ToXml();

                    string innerXml = null;
                    if (mapHashInner3DEntities[hash].OpcuaEntityReference != null)
                        innerXml = mapHashInner3DEntities[hash].OpcuaEntityReference.ToXml();

                    listAnimation.ForEach(animation =>
                    {
                        if (animation.OpcuaEntityReference == null)
                        {
                            if (!String.IsNullOrEmpty(innerXml))
                            {
                                if (String.IsNullOrEmpty(animation.Expression))
                                    animation.Expression = mapHashInner3DEntities[hash].Expression;

                                animation.OpcuaEntityReference = innerXml.FromXml<OPCUAEntityReference>();
                            }
                            else if (!String.IsNullOrEmpty(xml))
                            {
                                if (String.IsNullOrEmpty(animation.Expression))
                                    animation.Expression = Expression;

                                animation.OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
                            }
                        }
                        ReplaceAlias(animation);
                        animation.Control = element;
                        animation.Control3D = mapHashInner3DEntities[hash].entity3D;
                        animation.mapDynamics = Document.dataContextExpando;
                        animation.mapCurrentParameteItems = Document.mapCurrentParameteItems;
                        animation.Init(mapHashInner3DEntities[hash], documentParent, sessionname, ExpressionEntity_ParserError, ExpressionEntity_ExecutionError);
                    });

                    var listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
                    listCommand.ForEach(command =>
                    {
                        if (command.OpcuaEntityReference == null)
                        {
                            if (!String.IsNullOrEmpty(innerXml))
                            {
                                if (String.IsNullOrEmpty(command.Expression))
                                {
                                    command.sExpression = mapHashInner3DEntities[hash].sExpression;
                                    command.sReverseExpression = mapHashInner3DEntities[hash].sReverseExpression;
                                }

                                command.OpcuaEntityReference = innerXml.FromXml<OPCUAEntityReference>();
                            }
                            else if (!String.IsNullOrEmpty(xml))
                            {
                                if (String.IsNullOrEmpty(command.Expression))
                                {
                                    command.sExpression = mapHashInner3DEntities[hash].sExpression;
                                    command.sReverseExpression = mapHashInner3DEntities[hash].sReverseExpression;
                                }

                                command.OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
                            }
                        }

                        ReplaceAlias(command);
                        command.Control = element;
                        command.Control3D = mapHashInner3DEntities[hash].entity3D;
                        command.mapDynamics = Document.dataContextExpando;
                        command.mapCurrentParameteItems = Document.mapCurrentParameteItems;
                        command.Init(mapHashInner3DEntities[hash], documentParent, sessionname, ExpressionEntity_ParserError, ExpressionEntity_ExecutionError);
                        if (!String.IsNullOrEmpty(command.sExpression))
                            command.sExpression = sExpression;
                        if (!String.IsNullOrEmpty(command.sReverseExpression))
                            command.sReverseExpression = sReverseExpression;
                    });
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Execute3DInnerCommand(String hash)
        {
            if (mapHashInner3DEntities == null || !mapHashInner3DEntities.ContainsKey(hash))
                return false;
            var listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
            if (listCommand.Count == 0)
                return false;

            bool bRet = false;
            listCommand.ForEach(command =>
            {
                if (command.CanExecute())
                {
                    bRet = true;
                    try
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeAsynchronously(() =>
                        {
                            command.Execute();
                        });
                    }
                    catch (Exception ex)
                    {
                        SetEntityError(ex.Message);
                    }
                }
            });
            return bRet;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Has3DInnerCommand(String hash)
        {
            if (mapHashInner3DEntities == null || !mapHashInner3DEntities.ContainsKey(hash))
                return false;
            var listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
            if (listCommand.Count == 0)
                return false;

            return true;
        }

        void Terminte3DInnerEntities()
        {
            if (mapHashInner3DEntities == null)
                return;
            foreach (var hash in mapHashInner3DEntities.Keys)
            {
                if (mapHashInner3DEntities[hash].entity3D != null)
                {
                    var listAnimation = mapHashInner3DEntities[hash].AnimationList as AnimationManagerList;
                    listAnimation.ForEach(animation =>
                    {
                        animation.Stop();
                        animation.Terminate();
                    });

                    var listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
                    listCommand.ForEach(command =>
                    {
                        command.Terminate();
                    });
                }
            }
        }

        void Get3DInnerEmptyAnimation(AnimationManagerList list)
        {
            if (mapHashInner3DEntities == null)
                return;
            foreach (var hash in mapHashInner3DEntities.Keys)
            {
                if (mapHashInner3DEntities[hash].entity3D != null)
                {
                    var listAnimation = mapHashInner3DEntities[hash].AnimationList as AnimationManagerList;
                    listAnimation.ForEach(animation =>
                        {
                            if (animation.OpcuaEntityReference == null)
                            {
                                list.Add(animation);
                            }
                        });
                }
            }
        }

        void Get3DInnerEmptyCommand(CommandManagerList list)
        {
            if (mapHashInner3DEntities == null)
                return;
            foreach (var hash in mapHashInner3DEntities.Keys)
            {
                if (mapHashInner3DEntities[hash].entity3D != null)
                {
                    var listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
                    listCommand.ForEach(command =>
                    {
                        list.Add(command);
                    });
                }
            }
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(OPCUAEntityReference value)
        {
            if (ListopcuaSmartEntityReference != null)
            {
                foreach (var item in ListopcuaSmartEntityReference)
                {
                    if (item.MatchTypeDefintion(value))
                    {
                        value.Merge(item);
                        //value.HostName = item.HostName;
                        //value.EndpointUrl = item.EndpointUrl;
                        //// value.StartingAddress = item.RelativePath;
                        //value.ResolvedNodeId = null;
                        //value.ResolvedItem = true;
                        return true;
                    }
                }
            }

            if (OpcuaEntityReference != null && OpcuaEntityReference.MatchTypeDefintion(value))
            {
                value.Merge(OpcuaEntityReference);
                //value.AppName = OpcuaEntityReference.AppName;
                //value.HostName = OpcuaEntityReference.HostName;
                //value.EndpointUrl = OpcuaEntityReference.EndpointUrl;
                //// value.StartingAddress = OpcuaEntityReference.RelativePath;
                //value.ResolvedNodeId = null;
                //value.ResolvedItem = true;
                return true;
            }

            return false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsMatchTypeDefinition(OPCUAEntityReference value)
        {
            if (ListopcuaSmartEntityReference != null)
            {
                foreach (var item in ListopcuaSmartEntityReference)
                {
                    if (item.MatchTypeDefintion(value))
                        return true;
                }
            }

            if (OpcuaEntityReference != null && OpcuaEntityReference.MatchTypeDefintion(value))
                return true;

            return false;
        }
        string GetReplacingExpressionPath(string path)
        {
            if (!path.StartsWith(NamespaceTableConverter.FormattedRootTags))
            {
                var names = path.Split(':');
                if (names.Length > 1)
                    path = names[0];
                path = path.Replace("\\", "/");
                path = path.Replace("/", String.Format("/{0}", NamespaceTableConverter.FormattedNsChars));
                path = String.Format("{0}{1}{2}/", NamespaceTableConverter.FormattedRootTags, NamespaceTableConverter.FormattedNsChars, path);
            }
            return path;
        }

        internal void SubstituteExpressionVariable(String oldVar, String newVar)
        {
            oldVar = GetReplacingExpressionPath(oldVar);
            newVar = GetReplacingExpressionPath(newVar);
            
            if (!String.IsNullOrEmpty(Expression))
                Expression = Expression.Replace(oldVar, newVar);
            if (!String.IsNullOrEmpty(ReverseExpression))
                ReverseExpression = ReverseExpression.Replace(oldVar, newVar);

            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (!String.IsNullOrEmpty(animation.Expression))
                    animation.Expression = animation.Expression.Replace(oldVar, newVar);
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                if (!String.IsNullOrEmpty(command.Expression))
                    command.Expression = command.Expression.Replace(oldVar, newVar);
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinitionInChild(OPCUAEntityReference value, String session)
        {
            if (value.ResolvedItem)
                return true;

            if (OpcuaEntityReference == null)
                return false;

            var sessionViewModel = OpcuaEntityReference.GetSession(session);
            if (sessionViewModel == null)
                return false;
            var dt = DateTime.Now;
            while (sessionViewModel.Connected == false)
            {
                if (dt.AddMilliseconds(5000) < DateTime.Now)
                    return false;
                Utilities.WaitForPriority.DoEventsSync();
                System.Threading.Thread.Sleep(100);
            }
            var Browser = sessionViewModel.CreateBrowser(OpcuaEntityReference.ResolvedNodeId, true);

            var list = Browser.BrowseForTypeDefinition(value.ParentTypeDefinitionNodeId);
            if (list.Count > 0)
            {
                value.RelativePath = String.Format("{0}{1}/{2}", OpcuaEntityReference.RelativePath, list[0].CompletePath, value.RelativePath);
                value.StartingAddress = null;
                value.ResolvedNodeId = null;
                value.ResolvedItem = true;
                return true;
            }
            return false;
        }
#endif
#if !NET_STANDARD
        public void ExecuteCommands()
        {
            if (listCommands == null)
                return;

            listCommands.ForEach(c =>
                {
                    try
                    {
                        c.Execute();
                    }
                    catch (Exception ex)
                    {
                        SetEntityError(ex.Message);
                    }
                });

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
#if !WINDOWS_UWP
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
        }

#if !WINDOWS_UWP
        public List<RemoteExecute> RemoteExecuteCommands()
        {
            if (listCommands == null)
                return null;

            var list = new List<RemoteExecute>();
            listCommands.ForEach(c =>
            {
                try
                {
                    var r = c.RemoteExecute();
                    if (r != null)
                        list.Add(r);
                }
                catch (Exception ex)
                {
                }
            });

            return list;
        }
#endif
        public bool CanExecuteCommands()
        {
            if (listCommands != null)
            {
                if (bExecuteAnyEnabledCommands)
                {
                    foreach (var c in listCommands)
                    {
                        if (c.CanExecute())
                            return true;
                    }
                }
                else
                {
                    foreach (var c in listCommands)
                    {
                        if (!c.CanExecute())
                            return false;
                    }

                    return true;
                }
            }

            return false;            
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EvaluateCanExecuteCommands()
        {
            ExecuteCommandState = CanExecuteCommands();   
        }

        void SetDataContext()
        {
            var fe = Entity as FrameworkElement;
            if (SourceSymbolLinked && fe is ContentControl && (fe as ContentControl).Content is FrameworkElement)
                fe = (fe as ContentControl).Content as FrameworkElement;
            if (fe == null)
                return;

            try
            {
                if (expressionEntity != null)
                    fe.DataContext = expressionEntity.TempVariable;
                else if (expressionUnitConverter != null)
                    fe.DataContext = expressionUnitConverter.TempVariable;
                else if (OpcuaEntityReference != null)
                    fe.DataContext = OpcuaEntityReference.MonitoredItemViewModel;
                else
                    fe.DataContext = null;
            }
            catch (Exception ex)
            {
                SetEntityError(ex.Message);
            }
        }

#if !WINDOWS_UWP
        Effect previousEffect;
        BusyAdorner busyAdorner;
        bool previousClipToBounds;
#endif
        bool errorEffectOn;
        private void SetEntityError(String error, bool bShowMessageBox = true, bool bSetTooltip = false)
        {
            if (String.IsNullOrEmpty(error))
            {
                if (errorEffectOn)
                {
#if !WINDOWS_UWP
                    Entity.Effect = previousEffect;
                    Entity.ClipToBounds = previousClipToBounds;
                    // Entity.Blink(-1, 0, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
                    previousEffect = null;
#else
                    Entity.Projection = null;
#endif

                    if (Entity.Effect != null)
                        DependencyObjectExtensions.UpdateAllBindings(Entity.Effect);
                    errorEffectOn = false;
                }
                SetBusyEffect(false);
            }
            else
            {
                if (!errorEffectOn)
                {
                    errorEffectOn = true;
#if !WINDOWS_UWP
                    previousEffect = Entity.Effect;
                    previousClipToBounds = Entity.ClipToBounds;

                    var effect = new DropShadowEffect 
                    { 
                        ShadowDepth = 0, 
                        BlurRadius = 10, 
                        Color = Colors.Red 
                    };
                    Entity.Effect = effect;
                    Entity.ClipToBounds = false;
                    /*
                    Entity.Blink(1000, 0.2, 1, new BackEase() { EasingMode = EasingMode.EaseOut },
                        (o, e) =>
                        {
                            if (errorEffectOn)
                            {
                                Entity.Blink(0, 0, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
                                Entity.Effect = previousEffect;
                                Entity.ClipToBounds = previousClipToBounds;
                                Entity.Opacity = 1;
                                previousEffect = null;
                                errorEffectOn = false;
                            }
                        });
                    */
#else
                    Entity.Projection = new Windows.UI.Xaml.Media.PlaneProjection() { RotationZ = 15 };
#endif
                    // SetBusyEffect(true);
                }

                if (bSetTooltip && Entity is FrameworkElement)
                    (Entity as FrameworkElement).ToolTip = error;

                if (bShowMessageBox
#if !WINDOWS_UWP
                    && !bIsBlindServer
#endif
                    )
                // MessageBox.Show(error);
                {
                    if (Document != null)
                    {
                        var ui = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (ui != null)
                            ui.ShowError(error);
#if !WINDOWS_UWP
                            else
                                WinUIMessageBox.Show(
                                                Window.GetWindow(Element),
                                                error,
                                                EntityName,
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error,
                                                MessageBoxResult.None, MessageBoxOptions.None,
                                                DevExpress.Xpf.Core.FloatingMode.Window
                                                );
#endif
                    }
#if !WINDOWS_UWP
                    else
                        WinUIMessageBox.Show(
                                        Window.GetWindow(Element),
                                        error,
                                        EntityName,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error,
                                        MessageBoxResult.None, MessageBoxOptions.None,
                                        DevExpress.Xpf.Core.FloatingMode.Window
                                        );
#endif
                }
            }

            LastErrorMessage = error ?? String.Empty;
        }
#if !WINDOWS_UWP
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetIsBlindServer()
        {
            bIsBlindServer = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsOnBlindServer()
        {
            return bIsBlindServer;
        }
#endif
        // double oldOpacity;
#if !WINDOWS_UWP
        object oldTooltip;
#endif
        bool isBusy;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetBusyEffect(bool bSet)
        {
            if (OpcuaEntityReference == null)
                return;

            if (bSet)
            {
                if (!isBusy)
                {
                    isBusy = true;
                    // oldOpacity = Entity.Opacity;
#if !WINDOWS_UWP
                    oldTooltip = (Entity as FrameworkElement).ToolTip;
#endif
                    // Entity.Opacity = 0.2;
#if !WINDOWS_UWP
                    if (!bIsBlindServer)
                    {
                        (Entity as FrameworkElement).ToolTip = OpcuaEntityReference.LastMessage;
                        (Entity as FrameworkElement).ToolTipOpening += ScreenEntity_ToolTipOpening;
                    }
#endif
                }
                /*
                if (busyAdorner == null)
                {
                    busyAdorner = new BusyAdorner(Entity) { ToolTip = String.Format("Searching... {0}", OpcuaEntityReference.HumanReadable) };
                    var adornerLayer = AdornerLayer.GetAdornerLayer(Entity);
                    if (adornerLayer != null)
                        adornerLayer.Add(busyAdorner);

                    busyAdorner.ToolTipOpening += (o, e) =>
                        {
                            var sessionViewModel = OpcuaEntityReference.GetSession(SessionName);
                            if (sessionViewModel != null)
                            {
                                busyAdorner.ToolTip = String.Format("Last Error {0} at {1} for {2}",
                                    sessionViewModel.LastMessage,
                                    sessionViewModel.LastCurrentTime,
                                    OpcuaEntityReference.HumanReadable);
                            }
                            else
                            {
                                var realtimeModel = OpcuaEntityReference.GetRealTimeModel();
                                if (realtimeModel != null)
                                {
                                    busyAdorner.ToolTip = String.Format("Last Error {0} looking for {1}",
                                        realtimeModel.LastMessage,
                                        OpcuaEntityReference.HumanReadable);
                                }
                            }
                        };
                }
                */
            }
            else
            {
                if (isBusy)
                {
                    isBusy = false;
                    // Entity.Opacity = oldOpacity;
#if !WINDOWS_UWP
                    if (!bIsBlindServer)
                    {
                        (Entity as FrameworkElement).ToolTip = oldTooltip;
                        (Entity as FrameworkElement).ToolTipOpening -= ScreenEntity_ToolTipOpening;
                    }
#endif
                }
                /*
                if (busyAdorner != null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(Entity);
                    if (adornerLayer != null)
                        adornerLayer.Remove(busyAdorner);
                    busyAdorner = null;
                }
                */
            }
        }

#if !WINDOWS_UWP
        void ScreenEntity_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            (Entity as FrameworkElement).ToolTip = OpcuaEntityReference.LastMessage;
        }

        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (!bAddedMenuItemConnectionStatus)
            {
                bAddedMenuItemConnectionStatus = true;

                var fe = Entity as FrameworkElement;
                if (SourceSymbolLinked && fe is ContentControl && (fe as ContentControl).Content is FrameworkElement)
                    fe = (fe as ContentControl).Content as FrameworkElement;
                if (fe != null && fe.DataContext is MonitoredItemViewModel)
                    AddContextSysMenu(sender as ContextMenu, fe.DataContext as MonitoredItemViewModel);
            }
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateDynamicMapForElement(Dictionary<String, String> map)
        {
            if (Element is IDynamicTagAware)
            {
                var dyamicAware = Element as IDynamicTagAware;
                dyamicAware.UpdateMapDynamics(map);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateDynamicMapForElement(Dictionary<String, String> map, string name)
        {
            if (Element is IDynamicTagAware)
            {
#if !WINDOWS_UWP
                Document.SubscribePropertyChangeXamlWriterProperties(Element, name);
#endif
                var dyamicAware = Element as IDynamicTagAware;
                dyamicAware.UpdateMapDynamics(map);
#if !WINDOWS_UWP
                Document.UnsubscribePropertyChangeXamlWriterProperties(Element);
#endif
            }
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<OPCUAEntityReference> GetAllSourceEntityReferences()
        {
            if (OpcuaEntityReference != null)
                yield return OpcuaEntityReference;

#if !WINDOWS_UWP && !NET_STANDARD
            if (opcuaEntityReference3DZoom != null)
                yield return opcuaEntityReference3DZoom;

            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    var list = entity.GetAllSourceEntityReferences();
                    foreach (var tag in list)
                        yield return tag;
                }
            }
#endif
            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (animation.OpcuaEntityReference != null)
                    yield return animation.OpcuaEntityReference;
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                //if (command.OpcuaEntityReference != null)
                //{
                    var list = command.ListTags;
                    foreach (var tag in list)
                        yield return tag;
                //}
            }

#if !WINDOWS_UWP && !NET_STANDARD
            if (MapHashInner3DEntities != null && MapHashInner3DEntities.Count > 0)
            {
                foreach (var entity in MapHashInner3DEntities.Values)
                {
                    foreach(var tag in entity.GetAllSourceEntityReferences())
                        yield return tag;
                }
            }
#endif

#if !NET_STANDARD
            if (Element is IDynamicTagAware)
            {
                var dyamicAware = Element as IDynamicTagAware;
                var dynamicMap = dyamicAware.GetMapDynamics();
                foreach (var key in dynamicMap.Keys)
                {
                    var reference = dynamicMap[key].FromXml<OPCUAViewModel.OPCUAEntityReference>();
                    yield return reference;
                }
            }
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<String> GetAllSourceExpressions()
        {
            if (!String.IsNullOrEmpty(Expression))
                yield return Expression;
            if (!String.IsNullOrEmpty(ReverseExpression))
                yield return ReverseExpression;

            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (!String.IsNullOrEmpty(animation.Expression))
                    yield return animation.Expression;
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                if (!String.IsNullOrEmpty(command.Expression))
                    yield return command.Expression;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GetTagListEventArgs> GetTagList;
        public virtual void OnGetTagList(GetTagListEventArgs ea)
        {
            if (GetTagList != null)
                GetTagList(null/*this*/, ea);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<GetPrototypeListEventArgs> GetPrototypeList;
        public virtual void OnGetPrototypeList(GetPrototypeListEventArgs ea)
        {
            if (GetPrototypeList != null)
                GetPrototypeList(null/*this*/, ea);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Dictionary<string, string>> GetAllSourceScreenCommandDetails(CancellationToken quitEvent)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
            var listCommand = CommandList as CommandManagerList;
            for (int i = 0; i < listCommand.Count(); i++)
            {
                var command = listCommand[i];
                if (quitEvent.IsCancellationRequested)
                    break;
                if (command is OpenScreenCommand && (command as OpenScreenCommand).ScreenName != null)
                {
                    var dictionary = new Dictionary<string, string>();
                    dictionary[string.Format("{0}\\{1}", Properties.Resources.CommandTagHeader, command.Name)] = (command as OpenScreenCommand).ScreenName.GetPathString();
                    ret.Add(dictionary);
                }
            }
            if (MapHashInner3DEntities != null && MapHashInner3DEntities.Count > 0)
            {
                foreach (var entry in MapHashInner3DEntities.Keys)
                {
                    var listDynamic3D = MapHashInner3DEntities[entry].GetAllSourceScreenCommandDetails(quitEvent);
                    ret.AddRange(listDynamic3D);
                }
            }
            return ret;
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<OPCUAEntityReference> GetMonitorRemoteSourceEntityReferences()
        {
            if (OpcuaEntityReference != null)
                yield return OpcuaEntityReference;

#if !WINDOWS_UWP && !NET_STANDARD
            if (opcuaEntityReference3DZoom != null)
                yield return opcuaEntityReference3DZoom;
#endif
            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (animation.OpcuaEntityReference != null)
                    yield return animation.OpcuaEntityReference;
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        String CreateUniqueName(String name, List<String> list)
        {
            if (!list.Contains(name))
                return name;
            var newname = name;
            int i = 0;
            while(list.Contains(newname))
                newname = String.Format("{0} {1}", name, ++i);

            return newname;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, OPCUAEntityReference> GetMapDynamics()
        {
            var ret = new Dictionary<String, OPCUAEntityReference>();
            if (OpcuaEntityReference != null && (OpcuaEntityReference.IsValid || AliasHelper.ContainsAlias(OpcuaEntityReference.RelativePath)))
                ret.Add(CreateUniqueName(Properties.Resources.ItemTagHeader, ret.Keys.ToList()), OpcuaEntityReference);

            if (opcuaEntityReference3DZoom != null && (opcuaEntityReference3DZoom.IsValid || AliasHelper.ContainsAlias(opcuaEntityReference3DZoom.RelativePath)))
                ret.Add(CreateUniqueName(Properties.Resources.Item3DZoomHeader, ret.Keys.ToList()), opcuaEntityReference3DZoom);

            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (animation.OpcuaEntityReference != null && (animation.OpcuaEntityReference.IsValid || AliasHelper.ContainsAlias(animation.OpcuaEntityReference.RelativePath)))
                    ret.Add(CreateUniqueName(string.Format(Properties.Resources.AnimationList, animation.Name, animation.AnimationSummary), ret.Keys.ToList()), animation.OpcuaEntityReference);
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                foreach (var item in command.TagsMap)
                {
                    var tag = item.Key;
                    if (tag != null && (tag.IsValid || AliasHelper.ContainsAlias(tag.RelativePath)))
                        ret.Add(CreateUniqueName(string.Format(Properties.Resources.CommandList, command.Name, item.Value), ret.Keys.ToList()), tag);
                }
            }

            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    var map = entity.GetMapDynamics();
                    foreach (var entry in map)
                        ret.Add(CreateUniqueName(entry.Key, ret.Keys.ToList()), entry.Value);
                }
            }

            return ret;
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
                        !Name.Contains("Visibility") &&
                        !Name.Contains("ContextMenu") &&
                        !Name.Contains("NameScope") &&
                        !Name.Contains("DesignerProperties.IsInDesignMode") &&
                        !Name.Contains("BaseUriHelper.BaseUri") &&
                        String.Compare(Name, "Name", true) != 0;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<dynamic> GetIDynamicMap()
        {
            if (MapProblematicXamlWriterProperties != null)
            {
                var opcProp = typeof(OPCUAEntityReference).Name;
                var opcXMLProp = typeof(OPCUAXMLEntityReference).Name;
                var attributes = typeof(OPCUAEntityReference).GetCustomAttributes(typeof(DataContractAttribute), false);
                DataContractAttribute dataContractAttribute = null;
                if (attributes.Count() > 0)
                    dataContractAttribute = attributes[0] as DataContractAttribute;
                var opcDataContractProp = dataContractAttribute?.Name;
                List<string> listofkeys = null;

                if (!string.IsNullOrEmpty(opcDataContractProp))
                    listofkeys = (from c in MapProblematicXamlWriterProperties//.AsParallel()
                                  where !String.IsNullOrEmpty(c.Value) &&
                                  (c.Value.Contains(opcProp) ||
                              c.Value.Contains(opcXMLProp) ||
                              c.Value.Contains(opcDataContractProp))
#if !WINDOWS_UWP
                                          && IsValidPropertyXamlWriter(c.Key)
#endif
                                  select c.Key).ToList();
                else

                    listofkeys = (from c in MapProblematicXamlWriterProperties//.AsParallel()
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
                        var valueString = MapProblematicXamlWriterProperties[propName];
                        if (!String.IsNullOrEmpty(valueString))
                        {
                            object obj = null;
                            try
                            {
#if !WINDOWS_UWP
                                using (var reader = new StringReader(valueString))
#endif
                                {
#if !WINDOWS_UWP
                                    using (var textReader = new XmlTextReader(reader))
#endif
                                    {
#if !WINDOWS_UWP
                                        obj = System.Windows.Markup.XamlReader.Load(textReader);
#else
                                            obj = Windows.UI.Xaml.Markup.XamlReader.Load(valueString);
#endif
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
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
                                        var opcProperties = (from p in o.GetType().GetProperties() where p.PropertyType == typeof(OPCUAViewModel.OPCUAEntityReference) select p).ToList();
                                        var opcXMLProperties = (from p in o.GetType().GetProperties() where p.PropertyType == typeof(OPCUAViewModel.OPCUAXMLEntityReference) select p).ToList();
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
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, String> GetMapDynamicExpressions()
        {
            var ret = new Dictionary<String, String>();
            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                if (animation["Expression"])
                    ret.Add(CreateUniqueName(string.Format(Properties.Resources.AnimationListExpression, animation.Name), ret.Keys.ToList()), animation.Expression);
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                if (command["Expression"])
                    ret.Add(CreateUniqueName(string.Format(Properties.Resources.CommandListExpression, command.Name), ret.Keys.ToList()), command.Expression);
            }

            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    var map = entity.GetMapDynamicExpressions();
                    foreach (var entry in map)
                        ret.Add(CreateUniqueName(entry.Key, ret.Keys.ToList()), entry.Value);
                }
            }

            return ret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetMapDynamicExpressions(Dictionary<String, Object> selection)
        {
            var ret = new Dictionary<String, String>();
            var listAnimation = AnimationList as AnimationManagerList;
            foreach (var animation in listAnimation)
            {
                //if (animation.OpcuaEntityReference != null && animation.OpcuaEntityReference.IsValid)
                {
                    string _key = CreateUniqueName(string.Format(Properties.Resources.AnimationListExpression, animation.Name), ret.Keys.ToList());
                    ret.Add(_key, animation.Expression);
                    if (selection.ContainsKey(_key) && selection[_key] is string)
                        animation.Expression = (string)selection[_key];
                }
            }

            var listCommand = CommandList as CommandManagerList;
            foreach (var command in listCommand)
            {
                //if (command.OpcuaEntityReference != null && command.OpcuaEntityReference.IsValid)
                {
                    string _key = CreateUniqueName(string.Format(Properties.Resources.CommandListExpression, command.Name), ret.Keys.ToList());
                    ret.Add(_key, command.Expression);
                    if (selection.ContainsKey(_key) && selection[_key] is string)
                        command.Expression = (string)selection[_key];
                }
            }

            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    entity.SetMapDynamicExpressions(selection);
                }
            } 
            
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<dynamic> GetItemSources()
        {
#if !WINDOWS_UWP
            if (readerItemSources != null)
            {
                readerItemSources.NormalizeConnectionString(Document?.rootBase);
                if (!String.IsNullOrEmpty(readerItemSources.xmlUri) && 
                    !String.IsNullOrEmpty(readerItemSources.xmlItems))
                    return Utilities.XmlHelper.GetExpandoCTSFromXml(readerItemSources.xmlUri,
                        readerItemSources.xmlItems, null, readerItemSources.MaxTake).ToList();

                if (!String.IsNullOrEmpty(readerItemSources.DataProvider) &&
                    !String.IsNullOrEmpty(readerItemSources.Connection) &&
                    !String.IsNullOrEmpty(readerItemSources.Select))
                    return DataReader.DataReader.GetDynamicSqlData(readerItemSources.DataProvider, 
                                                                   readerItemSources.Connection,
                                                readerItemSources.Select, readerItemSources.Where, 
                                                readerItemSources.GroupBy, readerItemSources.Sort, 
                                                null, readerItemSources.MaxTake).ToList();
            }
            else 
#endif
            if (!String.IsNullOrEmpty(listItemSources))
            {
                var items = listItemSources.Split(new Char[] { '|', '\n', '\r' });
                List<dynamic> list = new List<dynamic>();
                foreach (var item in items)
                {
                    list.Add(item);
                }

                return list;
            }

            return null;
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsDynamic
        {
            get
            {
                return IsDynamicEntity();
            }
        }

        public bool IsDynamicEntity()
        {
#if !NET_STANDARD
            if (Entity is IDynamicTagAware && (Entity as IDynamicTagAware).GetMapDynamics().Count > 0 )
                return true;
#endif

            if (OpcuaEntityReference != null
#if !WINDOWS_UWP && !NET_STANDARD
                && (OpcuaEntityReference.IsValid || AliasHelper.ContainsAlias(OpcuaEntityReference.RelativePath))
#endif
                )
                return true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (opcuaEntityReference3DZoom != null && (opcuaEntityReference3DZoom.IsValid || AliasHelper.ContainsAlias(opcuaEntityReference3DZoom.RelativePath)))
                return true;
            if (!String.IsNullOrEmpty(Code))
                return true;
#endif

            if (listCommands != null && listCommands.Count > 0)
                return true;

            var listDynamic = (from p in GetMonitorRemoteSourceEntityReferences()/*.AsParallel()*/
#if !WINDOWS_UWP && !NET_STANDARD
                               where (p.IsValid || AliasHelper.ContainsAlias(p.RelativePath))
#endif
                               select p).ToList();
            if (listDynamic.Count() > 0)
                return true;

            if (IsItemSourceEntity())
                return true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    if (entity.IsDynamicEntity())
                        return true;
                }
            }
#endif
            return false;
        }

        public bool ContainsDynamic()
        {
#if !NET_STANDARD
            if (Entity is IDynamicTagAware && (Entity as IDynamicTagAware).GetMapDynamics().Count > 0)
                return true;
#endif

            if (OpcuaEntityReference != null
#if !WINDOWS_UWP && !NET_STANDARD
                && OpcuaEntityReference.IsValid
#endif
                )
                return true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (opcuaEntityReference3DZoom != null && opcuaEntityReference3DZoom.IsValid)
                return true;
#endif
            if (listCommands != null && listCommands.Count > 0)
                return true;

            var listDynamic = (from p in GetMonitorRemoteSourceEntityReferences()/*.AsParallel()*/
#if !WINDOWS_UWP && !NET_STANDARD
                               where p.IsValid 
#endif
                               select p).ToList();
            if (listDynamic.Count() > 0)
                return true;

            if (IsItemSourceEntity())
                return true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    if (entity.IsDynamicEntity())
                        return true;
                }
            }
#endif
            return false;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool ContainsCode()
        {
            if (!String.IsNullOrEmpty(Code))
                return true;
            return false;
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsItemSourceEntity()
        {
            if (!String.IsNullOrEmpty(ListItemSources)
#if !WINDOWS_UWP
                || readerItemSources != null
#endif
                )
                return true;

            return false;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        double dLast3dScaleValue = 1;
        bool bUpdating3DZomTag;
        public void Update3DZoomTag(double value)
        {
            dLast3dScaleValue = value;
            if (!Has3DZoomTag || opcuaEntityReference3DZoom.MonitoredItemViewModel == null)
                return;
            try
            {
                bUpdating3DZomTag = true;
                opcuaEntityReference3DZoom.MonitoredItemViewModel.Value = dLast3dScaleValue.ToString();
            }
            catch(Exception ex)
            {
                // SetEntityError(ex.Message);
            }
            finally
            {
                bUpdating3DZomTag = false;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler _3DZoomTagChanged;
        void On3DZoomTagChanged()
        {
            var t = _3DZoomTagChanged;
            if (t != null)
                t(entity, EventArgs.Empty);
        }

        public double GetLast3DZoomTagChanged()
        {
            return dLast3dScaleValue;
        }
#endif

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PrePrepareExecution(String sessionname)
        {
            entityName = EntityName;
            if (OpcuaEntityReference == null || !(Entity is FrameworkElement))
                return;

#if !WINDOWS_UWP
            if (!OpcuaEntityReference.IsValid && !AliasHelper.ContainsAlias(OpcuaEntityReference.RelativePath))
            {
                if (OpcuaEntityReference.HasValidValue)
                    SetEntityError(Properties.Resources.InvalidEntityReference, false, true);
                return;
            }
#endif

            if (
#if !WINDOWS_UWP
                !bIsBlindServer && 
#endif
                OpcuaEntityReference.NodeIdViewModel == null)
                SetBusyEffect(true);
        }
#endif

#region Alias
#if !NET_STANDARD
        OPCUAEntityReference GetReference(String name)
        {
            var names = name.Split('.');
            if (names.Length > 1)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface(names[0]);
                var eaEntity = datasync?.GetReference(name.Substring(names[0].Length + 1).Replace('\\', '&').Replace('/', '&'));
                return eaEntity;
            }

            names = name.Split(':');
            if (names.Length < 2)
            {
                var eaEntity = new GetTagEntityReference(name, null);
                Document.OnGetTagEntityReference(eaEntity);
                return eaEntity.entityReference;
            }
            else
            {
                var eaEntity = new GetTagEntityReference(names[1], names[0]);
                Document.OnGetTagEntityReference(eaEntity);
                return eaEntity.entityReference;
            }
        }

        public void ReplaceAllAliasInOnce()
        {
            var sourceString = OpcuaEntityReference?.ToXml();
            if(HasAnimations)
                ListAnimations.ForEach(animation =>
                {
                    ReplaceAlias(animation);
                });

            foreach (CommandManager.CommandManager command in CommandList)
                ReplaceAlias(command);

            ReplaceAlias();
        }

        internal void ReplaceAlias(CommandManager.CommandManager command)
        {
            var tags = command.ListTags;
            tags.ForEach(r =>
            {
                if (AliasHelper.ContainsAlias(r.RelativePath))
                {
                    var reference = GetReference(Alias.ReplaceAlias(r.RelativePath,
                        mapAlias, Document, this));
                    if (reference != null)
                        r.UpdateValue(reference);
                }
            });

            if (AliasHelper.ContainsAlias(command.Expression))
                command.Expression = Alias.ReplaceAlias(command.Expression,
                    mapAlias, Document, this);
        }

        void ReplaceAlias(AnimationManager.AnimationManager animation)
        {
            if (animation.OpcuaEntityReference != null && AliasHelper.ContainsAlias(animation.OpcuaEntityReference.RelativePath))
            {
                var reference = GetReference(Alias.ReplaceAlias(animation.OpcuaEntityReference.RelativePath,
                    mapAlias, Document, this));
                if (reference != null)
                    animation.OpcuaEntityReference = reference;
            }

            if (AliasHelper.ContainsAlias(animation.Expression))
                animation.Expression = Alias.ReplaceAlias(animation.Expression, mapAlias, Document, this);
        }

        void ReplaceAlias()
        {
            if (OpcuaEntityReference != null && AliasHelper.ContainsAlias(OpcuaEntityReference.RelativePath))
            {
                var reference = GetReference(Alias.ReplaceAlias(OpcuaEntityReference.RelativePath,
                        mapAlias, Document, this));
                if (reference != null)
                    OpcuaEntityReference = reference;
            }

            if (OpcuaEntityReference3DZoom != null && AliasHelper.ContainsAlias(OpcuaEntityReference3DZoom.RelativePath))
            {
                var reference = GetReference(Alias.ReplaceAlias(OpcuaEntityReference3DZoom.RelativePath,
                        mapAlias, Document, this));
                if (reference != null)
                    OpcuaEntityReference3DZoom = reference;
            }

            if (AliasHelper.ContainsAlias(Expression))
                Expression = Alias.ReplaceAlias(Expression, mapAlias, Document, this);
            if (AliasHelper.ContainsAlias(ReverseExpression))
                ReverseExpression = Alias.ReplaceAlias(ReverseExpression, mapAlias, Document, this);

            if (Element is IDynamicTagAware)
            {
                var dyamicAware = Element as IDynamicTagAware;
                var dynamicMap = dyamicAware.GetMapDynamics();
                var newMap = new Dictionary<String, String>();
                foreach (var key in dynamicMap.Keys)
                {
                    bool bUpdate = false;
                    string refKey = dynamicMap[key];
                    var reference = refKey.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                    if (AliasHelper.ContainsAlias(reference.RelativePath))
                    {
                        var r = GetReference(Alias.ReplaceAlias(reference.RelativePath, mapAlias, Document, this));
                        if (r != null)
                        {
                            reference = r;
                            bUpdate = true;
                        }
                    }

                    if (bUpdate && refKey != null && !newMap.ContainsKey(refKey))
                        newMap.Add(refKey, reference.ToXml());
                }

                if (newMap.Count > 0)
                {
                    newMap.Keys.ToList().ForEach(k =>
                    {
                        dyamicAware.MatchTypeDefinition(k, newMap[k]);
                    });
                }
            }
        }
#endif
#endregion

#region Replace
#if !NET_STANDARD
        public void ReplaceEntityReferences(Dictionary<String, String> map, string name)
        {
            foreach (var pair in map)
            {
                var source = pair.Key.FromXml<OPCUAEntityReference>();
                var dest = pair.Value.FromXml<OPCUAEntityReference>();

                SubstituteExpressionVariable(source.ReadablePath, dest.ReadablePath);

                if (listCommands != null)
                    listCommands.ForEach(command =>
                    {
                        command.UpdateTags(source, dest);
                    });

                if (listAnimations != null)
                    listAnimations.ForEach(animation =>
                    {
                        if (animation.OpcuaEntityReference != null && animation.OpcuaEntityReference.HumanReadableNoProject == source.HumanReadableNoProject)
                            animation.OpcuaEntityReference = dest;
                    });

                if (OpcuaEntityReference != null && OpcuaEntityReference.HumanReadableNoProject == source.HumanReadableNoProject)
                    OpcuaEntityReference = dest;

                if (OpcuaEntityReference3DZoom != null && OpcuaEntityReference3DZoom.HumanReadableNoProject == source.HumanReadableNoProject)
                    OpcuaEntityReference3DZoom = dest;
            }

            if (Element is IDynamicTagAware && !string.IsNullOrEmpty(name))
                UpdateDynamicMapForElement(map, name);

            if (mapHashInner3DEntities != null)
            {
                foreach (var entity in mapHashInner3DEntities.Values)
                {
                    entity.ReplaceEntityReferences(map, null);
                }
            }
        }
#endif
        #endregion

        internal void RefreshAllEntityReferences()
        {
            if (OpcuaEntityReference != null)
            {
                var xml = OpcuaEntityReference.ToXml();
                OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
            }

            var listAnimation = AnimationList as AnimationManagerList;
            var listCommand = CommandList as CommandManagerList;

            listAnimation.ForEach(animation =>
            {
#if !NET_STANDARD
                animation.Terminate();
#endif
                animation.RefreshAllEntityReferences();
            });

            listCommand.ForEach(command =>
            {
#if !NET_STANDARD
                command.Terminate();
#endif
                command.RefreshAllEntityReferences();
            });

#if !WINDOWS_UWP && !NET_STANDARD
            if (mapHashInner3DEntities != null)
            {
                foreach (var hash in mapHashInner3DEntities.Keys)
                {
                    if (mapHashInner3DEntities[hash].entity3D != null)
                    {
                        listAnimation = mapHashInner3DEntities[hash].AnimationList as AnimationManagerList;
                        listAnimation.ForEach(animation =>
                        {
                            animation.Terminate();
                            animation.RefreshAllEntityReferences();
                        });

                        listCommand = mapHashInner3DEntities[hash].CommandList as CommandManagerList;
                        listCommand.ForEach(command =>
                        {
                            command.Terminate();
                            command.RefreshAllEntityReferences();
                        });
                    }
                }
            }
#endif
        }

#if !NET_STANDARD
        //List<ExpressionValueConverter> listConverters;
#if !WINDOWS_UWP
        PropertyChangeNotifier notifierVisibility;
        DispatcherOperation d1, d2,/* d3,*/ d4;
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PrepareExecution(IDocument documentParent, FrameworkElement parent, String sessionname)
        {
            if (bExecuted)
                return;
            bExecuted = true;

            var fe = Entity as FrameworkElement;
            if (SourceSymbolLinked && fe is ContentControl && (fe as ContentControl).Content is FrameworkElement)
                fe = (fe as ContentControl).Content as FrameworkElement;
            if (fe == null)
            {
                bExecuted = false;
                return;
            }

            var listAnimation = AnimationList as AnimationManagerList;
            var listCommand = CommandList as CommandManagerList;

            var sourceString = OpcuaEntityReference?.ToXml();

            listAnimation.ForEach(animation =>
            {
                if (animation.OpcuaEntityReference == null)
                {
                    if (String.IsNullOrEmpty(animation.Expression))
                        animation.Expression = Expression;
                    animation.OpcuaEntityReference = sourceString?.FromXml<OPCUAEntityReference>();
                }

                ReplaceAlias(animation);

                if (animation.Control == null)
                    animation.Control = fe;

                animation.mapDynamics = Document.dataContextExpando;
                animation.mapCurrentParameteItems = Document.mapCurrentParameteItems;
                animation.Init(this, documentParent, sessionname, ExpressionEntity_ParserError, ExpressionEntity_ExecutionError);
            });

            listCommand.ForEach(command =>
            {
                if (command.Control == null)
                    command.Control = fe;
            });

#if !WINDOWS_UWP
            Init3DInnerEntities(parent, fe, sessionname, documentParent);
#endif
            RefreshItemSource();

            ReplaceAlias();

#if !WINDOWS_UWP
            if (OpcuaEntityReference3DZoom != null && OpcuaEntityReference3DZoom.IsValid)
            {
                observer3DZoom = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference3DZoom)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        Update3DZoomTag(dLast3dScaleValue);

                        if (n.MonitoredItemViewModel != null)
                        {
                            observerMonitoredModel3DZoom = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                            observerMonitoredModel3DZoom.RegisterHandler(m => m.DataValue, m =>
                            {
                                if (!bUpdating3DZomTag)
                                {
                                    fe.Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (m.DataValue != null)
                                        {
                                            if (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                                                m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                                            {
                                                if (m.DataValue.Value != null)
                                                {
                                                    try
                                                    {
                                                        var datavalue = m.DataValue.Value.ToString();
                                                        dLast3dScaleValue = Convert.ToDouble(datavalue);
                                                        On3DZoomTagChanged();
                                                    }
                                                    catch
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                    });
                                }
                            });
                        }
                    });

                OpcuaEntityReference3DZoom.Resolve(sessionname, documentParent);
                OpcuaEntityReference3DZoom.SetInUse(this, true);
            }
#endif
            if (OpcuaEntityReference == null || !(Entity is FrameworkElement)
#if !WINDOWS_UWP
                || !OpcuaEntityReference.IsValid
#endif
                )
                return;

            String browseName = String.Empty;
            String completePath = String.Empty;
            String relativePath = String.Empty;

            // SetEntityError(Properties.Resources.Connecting, false);

            // Entity.Opacity = 0.2;
            // SetBusyEffect(true);
            observer = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    try
                    {
                        var evn = n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier;
                    }
                    catch { }
#if !WINDOWS_UWP
                    d4 = fe.Dispatcher.BeginInvokeAsynchronously(() =>
#else
                    RunOnUIThread.RunIfRequired(() =>
#endif
                    {
                        // observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                        //if (!n.NodeIdViewModel.IsVariable && !n.NodeIdViewModel.IsEventNotifier)
                        //    SetBusyEffect(false);

                        //EvaluateAccessLevel();

                        if (expressionEntity != null)
                        {
                            expressionEntity.ParserError -= ExpressionEntity_ParserError;
                            expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                            expressionEntity.Dispose();
                            expressionEntity = null;
                        }

                        if (expressionUnitConverter != null)
                        {
                            expressionUnitConverter.ParserError -= ExpressionEntity_ParserError;
                            expressionUnitConverter.ExecutionError -= ExpressionEntity_ExecutionError;
                            expressionUnitConverter.Dispose();
                            expressionUnitConverter = null;
                        }

                        if (n.MonitoredItemViewModel != null)
                        {
                            var monitoredItemViewModel = n.MonitoredItemViewModel;
                            if (lastUnitConverterSettings != null && !String.IsNullOrEmpty(lastUnitConverterSettings.InputExpression) && Document != null)
                            {
                                expressionUnitConverter = ExpressionBucket.GetInstance(Document).AddExpression(n.MonitoredItemViewModel, lastUnitConverterSettings.InputExpression, lastUnitConverterSettings.OutputExpression, Document.mapCurrentParameteItems);
                                expressionUnitConverter.ParserError += ExpressionEntity_ParserError;
                                expressionUnitConverter.ExecutionError += ExpressionEntity_ExecutionError;
                                monitoredItemViewModel = expressionUnitConverter.TempVariable;
                            }

                            if (!String.IsNullOrEmpty(Expression) && Document != null)
                            {
                                expressionEntity = ExpressionBucket.GetInstance(Document).AddExpression(monitoredItemViewModel, Expression, ReverseExpression, Document.mapCurrentParameteItems);
                                expressionEntity.ParserError += ExpressionEntity_ParserError;
                                expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                            }

                            /*
                            var subscription = n.MonitoredItemViewModel.GetSubscriptionViewModelParent();
                            if (subscription != null)
                            {
                                var session = subscription.GetSessionViewModelParent();
                                if (session != null)
                                {
                                    if (observerSessionModel != null)
                                        observerSessionModel.Dispose();
                                    observerSessionModel = new PropertyObserver<SessionViewModel>(session)
                                    .RegisterHandler(m => m.LastMessage, m =>
                                    {
                                        fe.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                        {
                                            if (!session.Connected && !session.Disposed)
                                                SetEntityError(m.LastMessage, false);
                                            else
                                                SetEntityError(null);
                                        });
                                    });
                                }
                            }
                            */

                            if (observerMonitoredModel != null)
                                observerMonitoredModel.Dispose();
                            observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);

#if !WINDOWS_UWP
                            if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted &&
                                d1.Status != DispatcherOperationStatus.Completed)
                            {
                                d1.Abort();
                                // d1 = null;
                            }
                            //if (d3 != null && d3.Status != DispatcherOperationStatus.Aborted &&
                            //    d3.Status != DispatcherOperationStatus.Completed)
                            //{
                            //    d3.Abort();
                            //    // d3 = null;
                            //}
#endif
                            /*
                            .RegisterHandler(m => m.LastMessage, m =>
                            {
                                fe.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                {
                                    if (subscription != null)
                                    {
                                        var session = subscription.GetSessionViewModelParent();
                                        if (session != null)
                                        {
                                            if (!session.Connected && !session.Disposed)
                                                SetEntityError(m.LastMessage);
                                            else
                                                SetEntityError(null);
                                        }
                                    }
                                });
                            });
                            */

                            try
                            {
                                if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                    !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                {
                                    var value = String.Empty;
                                    var dataMap = Document.dataContextExpando as IDictionary<String, Object>;
                                    if (n.MonitoredItemViewModel.Value != null)
                                        value = n.MonitoredItemViewModel.Value;
                                    if (n.NodeIdViewModel != null)
                                    {
                                        dataMap[browseName] = value;
                                        dataMap[completePath] = value;
                                    }
                                    else if (!String.IsNullOrEmpty(relativePath))
                                        dataMap[relativePath] = value;
                                    dataMap[OpcuaEntityReference.HumanReadableNoProject] = value;
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            if (!n.MonitoredItemViewModel.IsReadOnly && n.MonitoredItemViewModel.DataValue != null)
                            {
                                if (Opc.Ua.StatusCode.IsGood(n.MonitoredItemViewModel.DataValue.StatusCode) ||
                                    n.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                                {
                                    SetEntityError(null);
                                }
                                else
                                {
#if !WINDOWS_UWP
                                    if (d2 == null ||
                                        d2.Status == DispatcherOperationStatus.Completed ||
                                        d2.Status == DispatcherOperationStatus.Aborted)
#endif
                                    {
#if !WINDOWS_UWP
                                        d2 = fe.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
#else
                                    RunOnUIThread.RunIfRequired(() =>
#endif
                                        {
                                            var dataValue = n.MonitoredItemViewModel?.DataValue;
                                            if (dataValue != null &&
                                                !Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) &&
                                                dataValue.StatusCode != Opc.Ua.StatusCodes.UncertainLastUsableValue)
                                            {
                                                SetEntityError(n.MonitoredItemViewModel.DataValue.StatusCode.ToString(), false);
                                            }
                                        });
                                    }
                                }
                            }

                            observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                            {
#if !WINDOWS_UWP
                                if (d1 == null || d1.Status == DispatcherOperationStatus.Completed ||
                                    d1.Status == DispatcherOperationStatus.Aborted)
#endif
                                {
#if !WINDOWS_UWP
                                    d1 = fe.Dispatcher.BeginInvokeAsynchronously(fe, () =>
#else
                                RunOnUIThread.RunIfRequired(() =>
#endif
                                    {
                                        if (m.DataValue != null)
                                        {
                                            if (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                                                m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                                            {
                                                SetEntityError(null);
                                                if (m.DataValue.Value != null)
                                                {
                                                    try
                                                    {
                                                        if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                                            !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                                                        {
                                                            var datavalue = n.MonitoredItemViewModel.Value;
                                                            var dataMap = Document.dataContextExpando as IDictionary<String, Object>;
                                                            if (n.NodeIdViewModel != null)
                                                            {
                                                                dataMap[browseName] = datavalue;
                                                                dataMap[completePath] = datavalue;
                                                            }
                                                            else if (!String.IsNullOrEmpty(relativePath))
                                                                dataMap[relativePath] = datavalue;
                                                            dataMap[OpcuaEntityReference.HumanReadableNoProject] = datavalue;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var dataValue = m.DataValue;
                                                if (dataValue != null &&
                                                    !Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) &&
                                                    dataValue.StatusCode != Opc.Ua.StatusCodes.UncertainLastUsableValue)
                                                {
                                                    SetEntityError(m.DataValue.StatusCode.ToString(), false);
                                                }
                                            }
                                        }
#if !WINDOWS_UWP
                                        if (Document != null)
                                            OnVariableChanged(new VariableChangedEventArgs(n.HumanReadableNoProject/*"_DataItem_"*/, m.DataValue));
#endif
                                    });
                                    /*
    #if !WINDOWS_UWP
                                    if (d1 != null)
                                    {
                                        if (d1.Status != DispatcherOperationStatus.Completed)
                                            d1.Completed += (o, e) => { d1 = null; };
                                        else
                                            d1 = null;
                                    }
    #endif
                                    */
                                }
                            });

                            if ((n.MonitoredItemViewModel.NodeIdModel == null ||
                                n.MonitoredItemViewModel.NodeIdModel.IsVariable) &&
                                n.MonitoredItemViewModel.DataValue != null &&
                                (Opc.Ua.StatusCode.IsGood(n.MonitoredItemViewModel.DataValue.StatusCode) ||
                                n.MonitoredItemViewModel.DataValue != null &&
                                n.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue))
                                n.MonitoredItemViewModel.ForceValuePropertyChanges();
                        }

                        SetDataContext();
#if !WINDOWS_UWP
                        if (!bIsBlindServer)
                        {
                            bAddedMenuItemConnectionStatus = false;
                            if (menuItemConnectionStatus == null)
                            {
                                if (fe.ContextMenu != null)
                                    fe.ContextMenu.Opened -= ContextMenu_Opened;
                                if (fe.ContextMenu == null)
                                    fe.ContextMenu = new ContextMenu();
                                fe.ContextMenu.Opened += ContextMenu_Opened;
                            }
                        }
#endif
                        BindDataSourceStyleTag(Entity as FrameworkElement);

                        // Entity.Opacity = 1.0;
                        // SetBusyEffect(false);
                    });
                });

            observer.RegisterHandler(n => n.NodeIdViewModel, n =>
            {
                var nodeIdViewModel = n.NodeIdViewModel;
                if (nodeIdViewModel != null && !nodeIdViewModel.IsEventNotifier)
                {
                    try
                    {
                        browseName = nodeIdViewModel.BrowseName.Name;
                        completePath = nodeIdViewModel.CompletePath;
                        relativePath = nodeIdViewModel.RelativePath;
                    }
                    catch { };
                }

                // observer.UnregisterHandler(p => p.NodeIdViewModel);
#if !WINDOWS_UWP
                fe.Dispatcher.BeginInvokeIfRequired(() =>
#else
                    RunOnUIThread.RunIfRequired(() =>
#endif
                {
                    // Entity.Opacity = 1.0;

                    // if (!n.NodeIdViewModel.IsVariable && !n.NodeIdViewModel.IsEventNotifier)
                    SetBusyEffect(false);

                    EvaluateAccessLevel();
                });
            });

#if !WINDOWS_UWP
            var action = new Action(
                delegate
                {
                    bool bEnable = Entity.Visibility == Visibility.Visible;
                    (from c in listAnimations where !(c is VisibilityAnimation) select c).ToList().ForEach(c =>
                    {
                        c.Enable(bEnable);
                    });
                });
            action();
            var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(UIElement));
            if (notifierVisibility == null)
            {
                notifierVisibility = new PropertyChangeNotifier(Entity, propDesc.Name);
                notifierVisibility.ValueChanged += (o, e) =>
                {
                    action();
                };
            }
#endif
            MonitoredItemViewModel temporary = null;
            if (Document.mapDataContextExpando != null && Document.mapDataContextExpando.ContainsKey(OpcuaEntityReference.HumanReadableNoProject))
                temporary = Document.mapDataContextExpando[OpcuaEntityReference.HumanReadableNoProject];
            if (temporary == null)
            {
                if (String.IsNullOrEmpty(opcuaEntityReference.TypeDefinitionName) || opcuaEntityReference.ParentTypeDefinitionIdList != null)
                    SetEntityError(String.Format(Properties.Resources.ConnectingReference, OpcuaEntityReference.RelativePath), false, true);
            }

            SessionName = sessionname;
            OpcuaEntityReference.Resolve(sessionname, temporary, documentParent);
            OpcuaEntityReference.SetInUse(this, true);
        }

        internal void ExpressionEntity_ParserError(object sender, EventArgs e)
        {
            var expressionEntity = (ExpressionEntity)sender;
            var fe = Entity as FrameworkElement;
            if (fe != null)
            {
                var error = expressionEntity.GetParserError();
                if (lastExpressionError != error)
                {
                    lastExpressionError = error;
                    if (!String.IsNullOrEmpty(error))
                    {
                        if (entitylog == null)
                            entitylog = LogManager.GetLogger(String.Format(Properties.Resources.ScreenLog, Document.Title));
                        entitylog.Error(String.Format(Properties.Resources.ExpressionError, expressionEntity.Formula, error, entityName));
                        fe.Dispatcher.BeginInvokeAsynchronously(() =>
                        {
                            SetEntityError(String.Format(Properties.Resources.ExpressionError, expressionEntity.Formula, error, entityName));
                        });
                    }
                }
            }
        }

        internal void ExpressionEntity_ExecutionError(object sender, EventArgs e)
        {
            var expressionEntity = (ExpressionEntity)sender;
            var exception = expressionEntity.GetExecutionError();
            if (exception != null)
            {
                if (entitylog == null)
                    entitylog = LogManager.GetLogger(String.Format(Properties.Resources.ScreenLog, Document.Title));
                entitylog.Error(String.Format(Properties.Resources.ExpressionError, expressionEntity.Formula, exception.Message, entityName));
            }
        }

        public void RefreshItemSource(bool bSync = false)
        {
            if (!String.IsNullOrEmpty(ListItemSources)
#if !WINDOWS_UWP
                || readerItemSources != null
#endif
                )
            {
                var entity = Entity;
                if (SourceSymbolLinked && entity is ContentControl && (entity as ContentControl).Content is UIElement)
                    entity = (Entity as ContentControl).Content as UIElement;
                Type t = entity.GetType();
                var p = t.GetProperty("DataSource");
#if !WINDOWS_UWP
                if (p != null &&
                    !String.IsNullOrEmpty(readerItemSources.DataProvider) &&
                    !String.IsNullOrEmpty(readerItemSources.Connection) &&
                    !String.IsNullOrEmpty(readerItemSources.Select))
                {
                    try
                    {
                        readerItemSources.NormalizeConnectionString(Document?.rootBase);
                        var dataset = DataReader.DataReader.GetDataSetSqlData(readerItemSources.DataProvider,
                                                                                readerItemSources.Connection,
                                                                                readerItemSources.Select, readerItemSources.Where,
                                                                                readerItemSources.GroupBy, readerItemSources.Sort);
                        p.SetValue(entity, dataset, null);
                    }
                    catch (Exception ex)
                    {
                        SetEntityError(ex.Message);
                    }
                }
                else
#endif
                {
#if !WINDOWS_UWP
                    if (p == null)
#endif
                        p = t.GetProperty("ItemsSource");
                    if (p != null)
                    {
#if !WINDOWS_UWP
                        if (busyAdorner == null)
                        {
                            busyAdorner = new BusyAdorner(Entity) { ToolTip = Properties.Resources.LoadingItemSources };
                            var adornerLayer = AdornerLayer.GetAdornerLayer(Entity);
                            if (adornerLayer != null)
                                adornerLayer.Add(busyAdorner);
                        }
#endif
                        var task3 = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                return GetItemSources();
                            }
                            catch (Exception ex)
                            {
                                var ret = new List<dynamic>();
                                ret.Add(ex.Message);
                                return ret;
                            }
                        });
                        if (!String.IsNullOrEmpty(listItemSources) || bSync)
                            task3.Wait();
                        var task4 = task3.ContinueWith(ret =>
                        {
                            if (entity is Selector)
                            {
                                if (ret.Result.Count > 0 && ret.Result[0] is ExpandoObject)
                                {
                                    var list = new List<String>();
                                    foreach (var item in ret.Result)
                                    {
                                        var dic = item as IDictionary<string, object>;
                                        if (dic == null)
                                            continue;
                                        String value = String.Empty;
                                        foreach (var str in dic.Keys)
                                        {
                                            if (!String.IsNullOrEmpty(value))
                                                value += " ";
                                            value += dic[str].ToString();
                                        }
                                        list.Add(value);
                                    }
                                    p.SetValue(entity, list, null);
                                }
                                else
                                    p.SetValue(entity, ret.Result, null);
                            }
                            else
                                p.SetValue(entity, ret.Result, null);

#if !WINDOWS_UWP
                            if (busyAdorner != null)
                            {
                                var adornerLayer = AdornerLayer.GetAdornerLayer(Entity);
                                if (adornerLayer != null)
                                    adornerLayer.Remove(busyAdorner);
                                busyAdorner = null;
                            }
#endif
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        if (!String.IsNullOrEmpty(listItemSources) || bSync)
                            task4.Wait();
                    }
                    // if (Entity is ItemsControl && !String.IsNullOrEmpty(ListItemSources))
                    // (Entity as ItemsControl).ItemsSource = GetItemSources();
                }
            }
        }

#if !WINDOWS_UWP
        MenuItem menuItemConnectionStatus;
        bool bAddedMenuItemConnectionStatus;
        void AddContextSysMenu(ContextMenu menu, MonitoredItemViewModel model)
        {
            var statusUserControl = new OPCUAViewModel.UserControls.MonitoredItemStatusViewModel();
            statusUserControl.DataContext = model;
            if (menuItemConnectionStatus == null)
            {
                menuItemConnectionStatus = new MenuItem() { Header = Properties.Resources.ConnectionStatus };
                menuItemConnectionStatus.Items.Add(new MenuItem()
                {
                    Header = statusUserControl
                });
                menu.Items.Add(menuItemConnectionStatus);
            }
            else
            {
                (menuItemConnectionStatus.Items[0] as MenuItem).Header = statusUserControl;
            }
        }
#endif

        public void ReexecuteVisibility()
        {
            if (listAnimations == null)
                return;

            (from c in listAnimations where c is VisibilityAnimation select c).ToList().ForEach(c =>
            {
                c.Execute();
            });
        }

        public void ReexecuteEnable()
        {
            if (listAnimations == null)
                return;

            (from c in listAnimations where c is EnableAnimation select c).ToList().ForEach(c =>
            {
                c.Execute();
            });
        }

        private void BindDataSourceStyleTag(FrameworkElement uie, bool bChild = true)
        {
            if (uie == null)
                return;

            if (uie.Tag is String)
            {
#if !WINDOWS_UWP
                var style = uie.TryFindResource(uie.Tag as String) as Style;
#else
                Style style = null;
                Object res;
                if (uie.Resources.TryGetValue(uie.Tag as String, out res))
                    style = res as Style;
#endif
                if (style != null)
                    uie.Style = style;
            }

            //if (uie is DevExpress.Xpf.Charts.ChartControl)
            //{
            //    var chart = uie as DevExpress.Xpf.Charts.ChartControl;
            //    foreach (var serie in chart.Diagram.Series)
            //    {
            //        if (serie.Tag is String)
            //        {
            //            var style = uie.TryFindResource(serie.Tag as String) as Style;
            //            if (style != null)
            //                serie.Style = style;
            //        }
            //    }
            //}
            if (!bChild)
                return;

            var list = uie.GetChildrenOfType<FrameworkElement>();
            var listTag = (from c in list where c.Tag is String select c).ToList();
            foreach (var ue in listTag)
                BindDataSourceStyleTag(ue, false);
            //if (uie is Panel)
            //{
            //    foreach (FrameworkElement item in (uie as Panel).Children)
            //        BindDataSourceStyleTag(ue);
            //}
            //else if (uie is Viewbox)
            //    BindDataSourceStyleTag((uie as Viewbox).Child as FrameworkElement);
            //else if (uie is ContentControl)
            //    BindDataSourceStyleTag((uie as ContentControl).Content as FrameworkElement);
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable<Guid> GetAnimationsListInvolved(OPCUAEntityReference reference)
        {
            if (listAnimations != null)
            {
                foreach(var animation in listAnimations)
                {
                    if ((animation.OpcuaEntityReference == null
#if !WINDOWS_UWP && !NET_STANDARD
                        || !animation.OpcuaEntityReference.IsValid
#endif
                        ) && Object.ReferenceEquals(OpcuaEntityReference, reference) ||
                        Object.ReferenceEquals(animation.OpcuaEntityReference, reference))
                        yield return animation.ID;
                }
            }

        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

#if !WINDOWS_UWP
            if (notifierVisibility != null)
            {
                notifierVisibility.Dispose();
                notifierVisibility = null;
            }
#endif
            if (observer != null)
                observer.Dispose();
            if (observerMonitoredModel != null)
                observerMonitoredModel.Dispose();
#if !WINDOWS_UWP
            if (observer3DZoom != null)
                observer3DZoom.Dispose();
            if (observerMonitoredModel3DZoom != null)
                observerMonitoredModel3DZoom.Dispose();
#endif
            if (observerSessionModel != null)
                observerSessionModel.Dispose();
            observer = null;
            observerMonitoredModel = null;
#if !WINDOWS_UWP
            observer3DZoom = null;
            observerMonitoredModel3DZoom = null;
#endif
            observerSessionModel = null;

#if !WINDOWS_UWP
            if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted && 
                d1.Status != DispatcherOperationStatus.Completed)
            {
                d1.Abort();
                // d1 = null;
            }
            if (d2 != null && d2.Status != DispatcherOperationStatus.Aborted &&
                d2.Status != DispatcherOperationStatus.Completed)
            {
                d2.Abort();
                // d2 = null;
            }
            //if (d3 != null && d3.Status != DispatcherOperationStatus.Aborted &&
            //    d3.Status != DispatcherOperationStatus.Completed)
            //{
            //    d3.Abort();
            //    // d3 = null;
            //}
            if (d4 != null && d4.Status != DispatcherOperationStatus.Aborted &&
                d4.Status != DispatcherOperationStatus.Completed)
            {
                d4.Abort();
                // d4 = null;
            }
#endif
            if (OpcuaEntityReference != null
#if !WINDOWS_UWP
                && OpcuaEntityReference.IsValid
#endif
                )
                OpcuaEntityReference.SetInUse(this, false);
            // OpcuaEntityReference = null;

#if !WINDOWS_UWP
            if (OpcuaEntityReference3DZoom != null && OpcuaEntityReference3DZoom.IsValid)
                OpcuaEntityReference3DZoom.SetInUse(this, false);
            // OpcuaEntityReference3DZoom = null;
#endif
            //if (listConverters != null)
            //{
            //    listConverters.ForEach(converter => converter.Dispose());
            //    listConverters.Clear();
            //}

            if (expressionEntity != null)
            {
                expressionEntity.ParserError -= ExpressionEntity_ParserError;
                expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                expressionEntity.Dispose();
                expressionEntity = null;
            }

            if (expressionUnitConverter != null)
            {
                expressionUnitConverter.ParserError -= ExpressionEntity_ParserError;
                expressionUnitConverter.ExecutionError -= ExpressionEntity_ExecutionError;
                expressionUnitConverter.Dispose();
                expressionUnitConverter = null;
                lastUnitConverterSettings = null;
            }

#if !WINDOWS_UWP
            Terminte3DInnerEntities();

            if (!bIsBlindServer)
            {
                var fe = Entity as FrameworkElement;
                if (SourceSymbolLinked && fe is ContentControl && (fe as ContentControl).Content is FrameworkElement)
                    fe = (fe as ContentControl).Content as FrameworkElement;
                if (fe != null && fe.ContextMenu != null)
                    fe.ContextMenu.Opened -= ContextMenu_Opened;
            }
#endif

            /*
            var fe = Entity as FrameworkElement;
            if (SourceSymbolLinked && fe is ContentControl && (fe as ContentControl).Content is FrameworkElement)
                fe = (fe as ContentControl).Content as FrameworkElement;
            if (fe != null)
                fe.DataContext = null;
            */
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        BasicNoUIObj basicCtl;
        bool bDontRaiseSecondError;
        bool bWindowCreated;
        bool bSubscribedScriptCreation;
        public void ExecuteScriptCode(bool bCheck = true)
        {
            if (basicCtl != null)
                return;

            if (!String.IsNullOrEmpty(Code))
            {
#if !DEBUG
                //if (bIsBlindServer)
                //    return;

                var enableVB = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxN+rOYP4u2+aLWnhBhIXJ4A=="/* VB */);
                if (enableVB == false)
                {
                    //logLicense.Warn(Properties.Resources.NoVBLicense);
                    if (iUFProjectManager == null)
                        iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.LicenseManager, 
                        DateTime.UtcNow, Properties.Resources.NoVBLicense, 
                        System.Diagnostics.EventLogEntryType.Warning);
                    return;
                }
#endif

                if (bCheck)
                {
                    if (!bIsBlindServer)
                    {
                        if (!Code.Contains("_Loaded(") && 
                            !Code.Contains("_ScriptLoaded(") && 
                            !Code.Contains("_VariableChanged("))
                        {
                            bSubscribedScriptCreation = true;
                            Entity.MouseEnter += Entity_MouseEnter;
                            Entity.KeyDown += Entity_KeyDown; 
                            return;
                        }
                    }
                }

                using (var cursor = new WaitCursor())
                {
                    bDontRaiseSecondError = false;

                    var bIsOnlyRuntime = ApplicationPropertiesHelper.GetProperty<bool>("IsOnlyRuntime", false);

                    if (Environment.UserInteractive && !bIsOnlyRuntime && Breakpoints != null && Breakpoints.Length > 0 && !bIsBlindServer)
                        basicCtl = new BasicIdeObj();
                    else
                        basicCtl = new BasicNoUIObj();

#if DEBUG
                    if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
                    basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

                    basicCtl.Initialize();
                    basicCtl.Caption = EntityName;
                    basicCtl.LargeIcon = null;
                    basicCtl.SmallIcon = null;
                    basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;

                    if (bIsBlindServer)
                        Util.IgnoreDialogs = true;

                    basicCtl.AttachToWindow(null, ManageConstants.Disconnecting);

                    // basicCtl.AttachToWindow(Window.GetWindow(Entity), ManageConstants.All);

                    if (!bIsBlindServer)
                    {
                        basicCtl.OverrideModalWindowOwner += (o, e) =>
                        {
                            var wnd = Window.GetWindow(Entity);
                            if (wnd != null)
                            {
                                if (wnd != null)
                                {
                                    e.OwnerHandle = new WindowInteropHelper(wnd).Handle;
                                    wnd.Focus();
                                }
                            }
                        };
                    }

                    if (syslog == null)
                        syslog = LogManager.GetLogger(String.Format(Properties.Resources.Script, Document?.Title, EntityName));

                    if (iUFProjectManager == null)
                        iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                    basicCtl.ErrorAlert += (o, e) =>
                    {
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

                            var text = basicCtl.Error.ToString();
                            syslog.Error(text);
                            if(iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(Document, String.Format(Properties.Resources.Script, Document?.Title, EntityName),
                                  DateTime.UtcNow, text,
                                  System.Diagnostics.EventLogEntryType.Error);
                            SetEntityError(text);
                        }
                    };
                    basicCtl.DebugPrint += (o, e) =>
                    {
                        syslog.Debug(e.Text);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, String.Format(Properties.Resources.Script, Document?.Title, EntityName),
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
                            var scriptManager = Document.GetService(typeof(IScriptManager)) as IScriptManager;
                            if (scriptManager != null)
                            {
                                e.Code = scriptManager.GetScriptCode(Document, filename);
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
                    if (Environment.UserInteractive && !bIsOnlyRuntime && Breakpoints != null && Breakpoints.Length > 0 && !bIsBlindServer)
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
        }

        void UnSubscribedScriptCreation()
        {
            if (!bSubscribedScriptCreation || Entity == null)
                return;
            bSubscribedScriptCreation = false;
            Entity.MouseEnter -= Entity_MouseEnter;
            Entity.KeyDown -= Entity_KeyDown;
        }

        void Entity_KeyDown(object sender, KeyEventArgs e)
        {
            UnSubscribedScriptCreation();
            ExecuteScriptCode(false);
        }

        void Entity_MouseEnter(object sender, MouseEventArgs e)
        {
            UnSubscribedScriptCreation();
            ExecuteScriptCode(false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TerminateScriptCode()
        {
            UnSubscribedScriptCreation();

            if (basicCtl == null)
                return true;

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
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool OnDropUri(Uri uri)
        {
            var control = Entity;
            if (control is ContentControl)
                control = (control as ContentControl).Content as UIElement;
            if (control == null)
                control = Entity;

            if (control is ICommandSource && !(control is ToggleButton))
            {
                String type = null;
                if (uri.IsAbsoluteUri)
                    type = Path.GetExtension(uri.AbsolutePath);
                else
                    type = Path.GetExtension(uri.GetPathString());

                CommandManager.CommandManager cm = null;
                if (type.ToLower() == ".script")
                {
                    cm = new RunScriptCommand
                    {
                        ScriptName = uri
                    };
                }
                else if (type.ToLower() == ".logic")
                {
                    cm = new RunLogicCommand
                    {
                        LogicName = uri
                    };
                }
                else if (type.ToLower() == ".report")
                {
                    cm = new ReportCommand
                    {
                        ReportName = uri
                    };
                }
                else if (type.ToLower() == ".ufrecipe")
                {
                    cm = new RecipeCommand
                    {
                        RecipeName = uri
                    };
                }
                else if (type.ToLower() == ".xaml")
                {
                    cm = new OpenScreenCommand
                    {
                        ScreenName = uri,
                        ExecutionMode = ExecutionMode.Normal
                    };
                }
                else
                    return false;

                if (listCommands == null)
                    listCommands = new CommandManagerList();
                else
                    listCommands.Clear();

                listCommands.Add(cm);

                var text = String.Empty;
                if (uri.IsAbsoluteUri)
                    text = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
                else
                    text = Path.GetFileNameWithoutExtension(uri.GetPathString());

                Type t = control.GetType();
                var p = t.GetProperty("Text");
                if (p != null)
                {
                    try
                    {
                        p.SetValue(control, text, null);
                    }
                    catch (Exception ex)
                    {
                        SetEntityError(ex.Message);
                    }
                }

                if (Entity is ContentControl && (Entity as ContentControl).Content is String)
                    (Entity as ContentControl).Content = text;

                return true;
            }

            return false;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool OnDropReference(OPCUAEntityReference reference)
        {
            OpcuaEntityReference = reference.Clone() as OPCUAEntityReference;
            return true;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool OnDropReference(ReferenceDescriptionViewModel reference)
        {
            var control = Entity;
            if (control is ContentControl)
                control = (control as ContentControl).Content as UIElement;
            if (control == null)
                control = Entity;

            if (reference.IsMethod && control is ICommandSource && !(control is ToggleButton))
            {
                var CallMethod = new CallMethodCommand 
                { 
                    OpcuaEntityReference = reference.CreateEntityReference(null) 
                };
                if (!CallMethod.EditDataCommandSetting())
                    return false;

                if (listCommands == null)
                    listCommands = new CommandManagerList();
                else
                    listCommands.Clear();

                listCommands.Add(CallMethod);

                if (control is ContentControl && (control as ContentControl).Content is String)
                    (control as ContentControl).Content = reference.DisplayName.ToString();

                return true;
            }

            //if (reference.IsEventNotifier)
            //{
            //    if (MessageBox.Show("Would you like to subscribe to Audit event type ?", "Event Choice", 
            //                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //        bAuditEventItemSource = true;
            //    else
            //        bAuditEventItemSource = false;
            //}

            OpcuaEntityReference = reference.CreateEntityReference(null);
            return true;
        }
#endif
        public bool HasAccessLevel(string accessRole, int accessLevel)
        {
            if (!String.IsNullOrEmpty(AccessRole) && (accessRole == null || AccessRole != accessRole))
                return false;
            if (AccessLevel > 0 && (accessLevel < 0 || accessLevel < AccessLevel))
                return false;
            return true;
        }

        public bool HasReadableAccess(int accessMask)
        {
            if (ReadableAccessMask != 0 && (accessMask & ReadableAccessMask) == 0)
                return false;
            return true;
        }

        public bool HasWritableAccess(int accessMask)
        {
            if (WritableAccessMask != 0 && (accessMask & WritableAccessMask) == 0)
                return false;
            return true;
        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EvaluateAccessLevel()
        {
            var fe = Entity as FrameworkElement;
            if (!AccessLevelFromTag || fe == null || opcuaEntityReference == null || opcuaEntityReference.NodeIdViewModel == null)
                return;

            opcuaEntityReference.NodeIdViewModel.InvalidateAttributes();
#if !WINDOWS_UWP
            if (!opcuaEntityReference.NodeIdViewModel.IsUserWritable || !opcuaEntityReference.NodeIdViewModel.IsWritable)
            {
                // fe.ToolTip = Properties.Resources.ItemNotWritable;
                fe.IsEnabled = false;
            }
            else
                fe.IsEnabled = true;
            AnimationManager.AnimationManager.SetIsAccessDenied(fe, !fe.IsEnabled);
            if (fe.IsEnabled)
                ReexecuteEnable();

#endif
            if (!opcuaEntityReference.NodeIdViewModel.IsUserReadable || !opcuaEntityReference.NodeIdViewModel.IsReadable)
            {
                SetInvisibleForSecurity(true);
                fe.Visibility = Visibility.Collapsed;
                // fe.ToolTip = Properties.Resources.ItemNotReadable;
            }
            else
            {
                SetInvisibleForSecurity(false);
                fe.Visibility = Visibility.Visible;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetInvisibleForSecurity(bool bset)
        {
            bInvisibleForSecurity = bset;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInvisibleForSecurity()
        {
            return bInvisibleForSecurity;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetInvisibleForZoom(bool bset)
        {
            bInvisibleForZoom = bset;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsInvisibleForZoom()
        {
            return bInvisibleForZoom;
        }
#endif
#endregion

#region Properties

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Guid ID
        {
            get
            {
                return id;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        OPCUAEntityReference OpcuaDataTypeEntityReference
        {
            get
            {
                if (listopcuaSmartEntityReference != null && listopcuaSmartEntityReference.Count > 0)
                    return listopcuaSmartEntityReference[0];
                return null;
            }
        }

        String lastErrorMessage;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String LastErrorMessage
        {
            get
            {
                return lastErrorMessage ?? String.Empty;
            }
            set
            {
                if (lastErrorMessage == value)
                    return;

                lastErrorMessage = value;
                OnPropertyChanged("LastErrorMessage");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ProblematicXamlWriterProperties MapProblematicXamlWriterProperties
        {
            get
            {
                return mapProblematicXamlWriterProperties;
            }
            set
            {
                mapProblematicXamlWriterProperties = value;
                // OnPropertyChanged("MapProblematicXamlWriterProperties");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Dictionary<String, String> Props
        {
            get
            {
                if (mapProps == null)
                    mapProps = new Dictionary<string, string>();
                return mapProps;
            }
            set
            {
                mapProps = value;
                OnPropertyChanged("Props");
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public String IdUnitConverter
        {
            get
            {
                return sIdUnitConverter;
            }
            set
            {
                if (value == sIdUnitConverter)
                    return;
                sIdUnitConverter = value;
                OnPropertyChanged("IdUnitConverter");
            }
        }

        public Dictionary<string, FontSettings> FontSettingList
        {
            get
            {
                return fontSettingList;
            }
            set
            {
                fontSettingList = value;
                OnPropertyChanged("FontSettingList");
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, ScreenEntity> MapHashInner3DEntities
        {
            get
            {
                return mapHashInner3DEntities;
            }
            set
            {
                mapHashInner3DEntities = value;
                // OnPropertyChanged("MapHashInner3DEntities");
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<String> GetListInner3DScreens()
        {
            if (mapHashInner3DEntities == null)
                return null;

            return (from c in MapHashInner3DEntities.Keys
                    where mapHashInner3DEntities[c].Screen3DUri != null && 
                    !String.IsNullOrEmpty(mapHashInner3DEntities[c].Screen3DUri.OriginalString)
                    select c).ToList();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri Screen3DUri
        {
            get
            {
                return screen3DUri;
            }
            set
            {
                if (value == null || String.IsNullOrEmpty(value.OriginalString))
                    screen3DUri = null;
                else
                    screen3DUri = value;
                // OnPropertyChanged("MapHashInner3DEntities");
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String Screen3DParameter
        {
            get
            {
                return screen3DParameter;
            }
            set
            {
                screen3DParameter = value;
                // OnPropertyChanged("MapHashInner3DEntities");
            }
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<CameraTranforms> ListCameraTransforms
        {
            get
            {
                return listCameraTransforms;
            }
            set
            {
                listCameraTransforms = value;
                // OnPropertyChanged("MapHashInner3DEntities");
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Is3DElement
        {
            get
            {
                if (Element == null)
                    return false;
                Viewport3D viewportElement = Element as Viewport3D;
                if(viewportElement == null)
                    if(Element is ContentControl)
                    {
                        viewportElement = (Element as ContentControl).Content as Viewport3D;
                        if(viewportElement == null && (Element as ContentControl).Content is Viewbox)
                            viewportElement = ((Element as ContentControl).Content as Viewbox).Child as Viewport3D;
                    }
                    else if (Element is Viewbox)
                    {
                        viewportElement = (Element as Viewbox).Child as Viewport3D;
                    }
                return viewportElement != null;
            }
        }

        public double Max3DZoom
        {
            get
            {
                return max3DZoom;
            }
            set
            {
                max3DZoom = value;
                OnPropertyChanged("Max3DZoom");
            }
        }

        public double Min3DZoom
        {
            get
            {
                return min3DZoom;
            }
            set
            {
                min3DZoom = value;
                OnPropertyChanged("Min3DZoom");
            }
        }

        public double Max3DRotationAngleX
        {
            get
            {
                return max3DRotationAngleX;
            }
            set
            {
                max3DRotationAngleX = value;
                OnPropertyChanged("Max3DRotationAngleX");
            }
        }

        public double Max3DRotationAngleY
        {
            get
            {
                return max3DRotationAngleY;
            }
            set
            {
                max3DRotationAngleY = value;
                OnPropertyChanged("Max3DRotationAngleY");
            }
        }

        public double Max3DRotationAngleZ
        {
            get
            {
                return max3DRotationAngleZ;
            }
            set
            {
                max3DRotationAngleZ = value;
                OnPropertyChanged("Max3DRotationAngleZ");
            }
        }

        public double Min3DRotationAngleX
        {
            get
            {
                return min3DRotationAngleX;
            }
            set
            {
                min3DRotationAngleX = value;
                OnPropertyChanged("Min3DRotationAngleX");
            }
        }

        public double Min3DRotationAngleY
        {
            get
            {
                return min3DRotationAngleY;
            }
            set
            {
                min3DRotationAngleY = value;
                OnPropertyChanged("Min3DRotationAngleY");
            }
        }

        public double Min3DRotationAngleZ
        {
            get
            {
                return min3DRotationAngleZ;
            }
            set
            {
                min3DRotationAngleZ = value;
                OnPropertyChanged("Min3DRotationAngleZ");
            }
        }

        public double Max3DTranslateOffsetX
        {
            get
            {
                return max3DTranslateOffsetX;
            }
            set
            {
                max3DTranslateOffsetX = value;
                OnPropertyChanged("Max3DTranslateOffsetX");
            }
        }

        public double Min3DTranslateOffsetX
        {
            get
            {
                return min3DTranslateOffsetX;
            }
            set
            {
                min3DTranslateOffsetX = value;
                OnPropertyChanged("Min3DTranslateOffsetX");
            }
        }

        public double Max3DTranslateOffsetY
        {
            get
            {
                return max3DTranslateOffsetY;
            }
            set
            {
                max3DTranslateOffsetY = value;
                OnPropertyChanged("Max3DTranslateOffsetY");
            }
        }

        public double Min3DTranslateOffsetY
        {
            get
            {
                return min3DTranslateOffsetY;
            }
            set
            {
                min3DTranslateOffsetY = value;
                OnPropertyChanged("Min3DTranslateOffsetY");
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public AnimationManagerList ListAnimations
        {
            get
            {
                return listAnimations;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasAnimations
        {
            get
            {
                return listAnimations != null && listAnimations.Count > 0;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasAccessControl
        {
            get
            {
                return ReadableAccessMask != AccessCodes.AccessLevelAll &&
                       ReadableAccessMask != AccessCodes.AccessLevelNone ||
                       WritableAccessMask != AccessCodes.AccessLevelAll &&
                       WritableAccessMask != AccessCodes.AccessLevelNone;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasAccessLevelControl
        {
            get
            {
                return AccessLevel > 0 || !String.IsNullOrEmpty(AccessRole);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasZoomVisibility
        {
            get
            {
                return zoomLevelVisibilityX > 0 || zoomLevelVisibilityY > 0;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public ScreenDocument Document
        {
            get
            {
                return document;
            }
            set
            {
                document = value;
            }
        }

#if !NET_STANDARD
        public String EntityName
        {
            get
            {
                if (Entity == null)
                    return String.Empty;

                if (Entity is FrameworkElement)
                {
                    var fe = Entity as FrameworkElement;
                    if (!String.IsNullOrEmpty(fe.Name))
                        return fe.Name;

#if !WINDOWS_UWP
                    if (!String.IsNullOrEmpty(fe.Uid))
                        return fe.Uid;
#endif
                }

#if !WINDOWS_UWP
                return Entity.DependencyObjectType.Name;
#else
                return Entity.GetType().Name;
#endif
            }
        }

        UIElement entity;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UIElement Entity
        {
            get
            {
                return entity;
            }

            set
            {
                //if (entity == value)
                //    return;

                entity = value;

                if (listAnimations != null)
                    listAnimations.ForEach(e =>
                    {
                        e.Control = entity;
                    });
                if (listCommands != null)
                    listCommands.ForEach(e =>
                    {
                        e.Control = entity;
                    });
            }
        }
#endif

        double linkedAspectRatio;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public double LinkedAspectRatio
        {
            get
            {
                return linkedAspectRatio;
            }
            set
            {
                linkedAspectRatio = value;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        Model3D entity3D;
        [Browsable(false)]
        public Model3D Entity3D
        {
            get
            {
                return entity3D;
            }

            set
            {
                if (entity3D == value)
                    return;

                entity3D = value;

                if (listAnimations != null)
                    listAnimations.ForEach(e =>
                    {
                        e.Control3D = entity3D;
                    });

                if (listCommands != null)
                    listCommands.ForEach(e =>
                    {
                        e.Control3D = entity3D;
                    });
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasCommands
        {
            get
            {
                return listCommands != null && listCommands.Count > 0;
            }
        }

        bool executeCommandState;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool ExecuteCommandState
        {
            get
            {
                return executeCommandState;
            }
            set
            {
                if (value == executeCommandState)
                    return;
                executeCommandState = value;
                OnPropertyChanged("ExecuteCommandState");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String StyleResource
        {
            get { return sStyleResource; }
            set
            {
                if (value == sStyleResource)
                    return;
                sStyleResource = value;
                OnPropertyChanged("StyleResource");
            }
        }

        //[Browsable(false)]
        //public String BrushResource
        //{
        //    get { return sBrushResource; }
        //    set
        //    {
        //        if (value == sBrushResource)
        //            return;
        //        sBrushResource = value;
        //        OnPropertyChanged("BrushResource");
        //    }
        //}

//[Browsable(false)]
//public String PenResource
//{
//    get { return sPenResource; }
//    set
//    {
//        if (value == sPenResource)
//            return;
//        sPenResource = value;
//        OnPropertyChanged("PenResource");
//    }
//}

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte TagDecorators
        {
            get { return tagDecorators; }
            set
            {
                if (value == tagDecorators)
                    return;
                tagDecorators = value;
                OnPropertyChanged("TagDecorators");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String TagBrush
        {
            get { return tagBrush; }
            set
            {
                if (value == tagBrush)
                    return;
                tagBrush = value;
                OnPropertyChanged("TagBrush");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String TagPen
        {
            get { return tagPen; }
            set
            {
                if (value == tagPen)
                    return;
                tagPen = value;
                OnPropertyChanged("TagPen");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String ProblematicXaml
        {
            get { return sProblematicXaml; }
            set
            {
                if (value == sProblematicXaml)
                    return;
                sProblematicXaml = value;
                OnPropertyChanged("ProblematicXaml");
            }
        }

#if !WINDOWS_UWP
        [DisplayName("Tag")]
#endif
        public OPCUAEntityReference OpcuaEntityReference
        {
            get { return opcuaEntityReference; }
            set
            {
                if (value == opcuaEntityReference)
                    return;
                opcuaEntityReference = value;
                OnPropertyChanged("OpcuaEntityReference");
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [DisplayName("3DZoomTag")]
        public OPCUAEntityReference OpcuaEntityReference3DZoom
        {
            get { return opcuaEntityReference3DZoom; }
            set
            {
                if (value == opcuaEntityReference3DZoom)
                    return;
                opcuaEntityReference3DZoom = value;
                OnPropertyChanged("OpcuaEntityReference3DZoom");
            }
        }

        [Browsable(false)]
        public bool Has3DZoomTag
        {
            get
            {
                return opcuaEntityReference3DZoom != null && opcuaEntityReference3DZoom.IsValid;
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<OPCUAEntityReference> ListopcuaSmartEntityReference
        {
            get { return listopcuaSmartEntityReference; }
            set
            {
                if (value == listopcuaSmartEntityReference)
                    return;
                listopcuaSmartEntityReference = value;
                OnPropertyChanged("ListopcuaSmartEntityReference");
            }
        }

//#if !WINDOWS_UWP
//        [Browsable(false)]
//#endif
//        [EditorBrowsable(EditorBrowsableState.Never)]
        public String ListItemSources
        {
            get { return listItemSources; }
            set
            {
                if (value == listItemSources)
                    return;
                listItemSources = value;
                OnPropertyChanged("ListItemSources");
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyVisiblityChanged("ListItemSources");
#endif
            }
        }

#if !WINDOWS_UWP
        //[Browsable(false)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public DataReaderModel ReaderItemSources
        {
            get { return readerItemSources; }
            set
            {
                if (value == readerItemSources)
                    return;
                readerItemSources = value;
                OnPropertyChanged("ReaderItemSources");
#if !NET_STANDARD
                OnPropertyVisiblityChanged("ReaderItemSources");
                OnPropertyVisiblityChanged("ListItemSources");
#endif
            }
        }
#endif
        public bool EnableManipulation
        {
            get { return bEnableManipulation; }
            set
            {
                if (value == bEnableManipulation)
                    return;
                bEnableManipulation = value;
                OnPropertyChanged("EnableManipulation");
            }
        }

        public bool EnableMouseOver
        {
            get { return bEnableMouseOver; }
            set
            {
                if (value == bEnableMouseOver)
                    return;
                bEnableMouseOver = value;
                OnPropertyChanged("EnableMouseOver");
            }
        }

        public bool ExecuteAnyEnabledCommands
        {
            get { return bExecuteAnyEnabledCommands; }
            set
            {
                if (value == bExecuteAnyEnabledCommands)
                    return;
                bExecuteAnyEnabledCommands = value;
                OnPropertyChanged("ExecuteAnyEnabledCommands");
            }
        }

        public bool ShowTooltipWhenDisabled
        {
            get { return bShowTooltipWhenDisabled; }
            set
            {
                if (value == bShowTooltipWhenDisabled)
                    return;
                bShowTooltipWhenDisabled = value;
                OnPropertyChanged("ShowTooltipWhenDisabled");
            }
        }

#if !WINDOWS_UWP
        public bool VisibleOnClient
        {
            get { return bVisibleOnClient; }
            set
            {
                if (value == bVisibleOnClient)
                    return;
                bVisibleOnClient = value;
                OnPropertyChanged("VisibleOnClient");
            }
        }

        public bool ForceDynamicOnClient
        {
            get { return bForceDynamicOnClient; }
            set
            {
                if (value == bForceDynamicOnClient)
                    return;
                bForceDynamicOnClient = value;
                OnPropertyChanged("ForceDynamicOnClient");
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String SourceSymbolProvider
        {
            get
            {
                if (sSourceSymbolProvider == null)
                    return sSourceSymbolProvider;
                return sSourceSymbolProvider;
            }
            set
            {
                if (value == sSourceSymbolProvider)
                    return;
                sSourceSymbolProvider = value;
                OnPropertyChanged("SourceSymbolProvider");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String SourceSymbolPath
        {
            get
            {
                if (sSourceSymbolPath == null)
                    return sSourceSymbolPath;
                return sSourceSymbolPath;
            }
            set
            {
                if (value == sSourceSymbolPath)
                    return;
                sSourceSymbolPath = value;
                OnPropertyChanged("SourceSymbolPath");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SourceSymbolLinked
        {
            get { return bSourceSymbolLinked; }
            internal set
            {
                if (value == bSourceSymbolLinked)
                    return;
                bSourceSymbolLinked = value;
                OnPropertyChanged("SourceSymbolLinked");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool SourceSymbolLinkedResolved { get; internal set; }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SourceSymbolLinkedActive
        {
            get { return bSourceSymbolLinked && !SourceSymbolLinkedPassive; }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SourceSymbolLinkedPassive
        {
            get { return bSourceSymbolLinkedPassive; }
            internal set
            {
                if (value == bSourceSymbolLinkedPassive)
                    return;
                bSourceSymbolLinkedPassive = value;
                OnPropertyChanged("SourceSymbolLinkedPassive");
            }
        }

        public String AccessRole
        {
            get { return accessRole; }
            set
            {
                if (accessRole != null && value == accessRole)
                    return;
                accessRole = value;
                OnPropertyChanged("AccessRole");
            }
        }

        public bool AccessLevelFromTag
        {
            get { return accessLevelFromTag; }
            set
            {
                if (value == accessLevelFromTag)
                    return;
                accessLevelFromTag = value;
                OnPropertyChanged("AccessLevelFromTag");
            }
        }

        public int AccessLevel
        {
            get { return accessLevel; }
            set
            {
                if (value == accessLevel)
                    return;
                accessLevel = value;
                OnPropertyChanged("AccessLevel");
            }
        }

        public int VisibilityLevel
        {
            get { return visibilityLevel; }
            set
            {
                if (value == visibilityLevel)
                    return;
                visibilityLevel = value;
                OnPropertyChanged("VisibililyLevel");
            }
        }

        public bool LockMovement
        {
            get { return lockMovement; }
            set
            {
                if (value == lockMovement)
                    return;
                lockMovement = value;
                OnPropertyChanged("LockMovement");
            }
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
            }
        }
#endif

        public int ReadableAccessMask
        {
            get { return readableAccessMask; }
            set
            {
                if (value == readableAccessMask)
                    return;
                readableAccessMask = value;
                OnPropertyChanged("ReadableAccessMask");
            }
        }

        public int WritableAccessMask
        {
            get { return writableAccessMask; }
            set
            {
                if (value == writableAccessMask)
                    return;
                writableAccessMask = value;
                OnPropertyChanged("WritableAccessMask");
            }
        }

        public String Expression
        {
            get
            {
                if (sExpression == null)
                    return sExpression;
                return sExpression;
            }
            set
            {
                if (value == sExpression)
                    return;
                sExpression = value;
                OnPropertyChanged("Expression");
            }
        }

#if !WINDOWS_UWP
        public Uri MenuName
        {
            get
            {
                return menuName;
            }
            set
            {
                if (value == menuName)
                    return;
                menuName = value;
                OnPropertyChanged("MenuName");
            }
        }

        public bool ShowMenuOnLeft
        {
            get
            {
                return showMenuOnLeft;
            }
            set
            {
                if (value == showMenuOnLeft)
                    return;
                showMenuOnLeft = value;
                OnPropertyChanged("ShowMenuOnLeft");
            }
        }
#endif

        public String ReverseExpression
        {
            get
            {
                if (sReverseExpression == null)
                    return sReverseExpression;
                return sReverseExpression;
            }
            set
            {
                if (value == sReverseExpression)
                    return;
                sReverseExpression = value;
                OnPropertyChanged("ReverseExpression");
            }
        }

        public double ZoomLevelVisibilityX
        {
            get { return zoomLevelVisibilityX; }
            set
            {
                if (value == zoomLevelVisibilityX)
                    return;
                zoomLevelVisibilityX = value;
                OnPropertyChanged("ZoomLevelVisibilityX");
            }
        }

        public double ZoomLevelVisibilityY
        {
            get { return zoomLevelVisibilityY; }
            set
            {
                if (value == zoomLevelVisibilityY)
                    return;
                zoomLevelVisibilityY = value;
                OnPropertyChanged("ZoomLevelVisibilityY");
            }
        }

        public String SpeechCommand
        {
            get
            {
                if (speechCommand == null)
                    return speechCommand;
                return speechCommand;
            }
            set
            {
                if (value == speechCommand)
                    return;
                speechCommand = value;
                OnPropertyChanged("SpeechCommand");
            }
        }

        public bool PreserveFontSettingList
        {
            get { return preserveFontSettingList; }
            set
            {
                if (value == preserveFontSettingList)
                    return;
                preserveFontSettingList = value;
                OnPropertyChanged("PreserveFontSettingList");
            }
        }

        public bool PreserveStyle
        {
            get { return preserveStyle; }
            set
            {
                if (value == preserveStyle)
                    return;
                preserveStyle = value;
                OnPropertyChanged("PreserveStyle");
            }
        }

        public bool PreserveCode
        {
            get { return preserveCode; }
            set
            {
                if (value == preserveCode)
                    return;
                preserveCode = value;
                OnPropertyChanged("PreserveCode");
            }
        }

        public bool PreserveCustomControlProperties
        {
            get { return preserveCustomControlProperties; }
            set
            {
                if (value == preserveCustomControlProperties)
                    return;
                preserveCustomControlProperties = value;
                OnPropertyChanged("PreserveCustomControlProperties");
            }
        }

        public bool PreserveColors
        {
            get { return preserveColors; }
            set
            {
                if (value == preserveColors)
                    return;
                preserveColors = value;
                OnPropertyChanged("PreserveColors");
            }
        }

        public bool PreserveSize
        {
            get { return preserveSize; }
            set
            {
                if (value == preserveSize)
                    return;
                preserveSize = value;
                OnPropertyChanged("PreserveSize");
            }
        }

        public bool PreserveCommands
        {
            get { return preserveCommands; }
            set
            {
                if (value == preserveCommands)
                    return;
                preserveCommands = value;
                OnPropertyChanged("PreserveCommands");
            }
        }

        public bool PreserveAnimations
        {
            get { return preserveAnimations; }
            set
            {
                if (value == preserveAnimations)
                    return;
                preserveAnimations = value;
                OnPropertyChanged("PreserveAnimations");
            }
        }

        public bool PreserveMenu
        {
            get { return preserveMenu; }
            set
            {
                if (value == preserveMenu)
                    return;
                preserveMenu = value;
                OnPropertyChanged("PreserveMenu");
            }
        }

        public bool PreserveExpression
        {
            get { return preserveExpression; }
            set
            {
                if (value == preserveExpression)
                    return;
                preserveExpression = value;
                OnPropertyChanged("PreserveExpression");
            }
        }

        public bool PreserveSecurity
        {
            get { return preserveSecurity; }
            set
            {
                if (value == preserveSecurity)
                    return;
                preserveSecurity = value;
                OnPropertyChanged("PreserveSecurity");
            }
        }

        public bool PreserveVisibility
        {
            get { return preserveVisibility; }
            set
            {
                if (value == preserveVisibility)
                    return;
                preserveVisibility = value;
                OnPropertyChanged("PreserveVisibility");
            }
        }

        public bool PreserveVariables
        {
            get { return preserveVariables; }
            set
            {
                if (value == preserveVariables)
                    return;
                preserveVariables = value;
                OnPropertyChanged("PreserveVariables");
            }
        }

        public bool PreserveText
        {
            get { return preserveText; }
            set
            {
                if (value == preserveText)
                    return;
                preserveText = value;
                OnPropertyChanged("PreserveText");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String PreservedBrush
        {
            get { return preservedBrush; }
            set
            {
                if (value == preservedBrush)
                    return;
                preservedBrush = value;
                // OnPropertyChanged("PreservedBrush");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String PreservedPen
        {
            get { return preservedPen; }
            set
            {
                if (value == preservedPen)
                    return;
                preservedPen = value;
                // OnPropertyChanged("PreservedPen");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String PreservedText
        {
            get { return preservedText; }
            set
            {
                if (value == preservedText)
                    return;
                preservedText = value;
                OnPropertyChanged("PreservedText");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public double PreservedWidth
        {
            get { return preservedWidth; }
            set
            {
                if (value == preservedWidth)
                    return;
                preservedWidth = value;
                OnPropertyChanged("PreservedWidth");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public double PreservedHeight
        {
            get { return preservedHeight; }
            set
            {
                if (value == preservedHeight)
                    return;
                preservedHeight = value;
                OnPropertyChanged("PreservedHeight");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Dictionary<String, String> MapAlias
        {
            get { return mapAlias; }
            set
            {
                if (value == mapAlias)
                    return;
                mapAlias = value;
                OnPropertyChanged("MapAlias");
            }
        }

#if !NET_STANDARD
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Dictionary<String, List<OPCUAEntityReference>> MapItemsToBeResolved
        {
            get { return mapItemsToBeResolved; }
            set
            {
                if (value == mapItemsToBeResolved/* || mapItemsToBeResolved == null && value != null && value.Count == 0*/)
                    return;
                mapItemsToBeResolved = value;
                // OnPropertyChanged("MapItemsToBeResolved");
            }
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
#if !WINDOWS_UWP
        [Category("Animations")]
        public bool Animations
        {
            get { return false; }
            set
            {
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Commands")]
        public bool Commands
        {
            get { return false; }
            set
            {
            }
        }
#endif
#endregion

#region IScriptable Members
#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        String IScriptable.Name
        {
            get
            {
                return EntityName;
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
                return Entity3D == null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetQuickReferenceList()
        {
            var list = new List<Object>();
            if (entity != null)
            {
                list.Add(entity);
                entity.Dispatcher.InvokeIfRequired(() =>
                {
                    if (entity is ContentControl)
                    {
                        var contentControl = entity as ContentControl;
                        if (contentControl.Content is UIElement)
                            list.Add(contentControl.Content);
                    }
                });
            }

            list.Add(this);
            if (Document != null)
            {
                list.Add(Document);
                var parent = Document.Parent;
                while (parent != null && parent.GetType() == Document.GetType())
                    parent = parent.Parent;
                if (parent == null)
                    parent = Document.Parent;
                if (parent != null)
                    list.Add(parent);
            }
            return list;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetReferenceList(CRMapsHeler cRMapsHeler = null)
        {
            if(cRMapsHeler == null)
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
            if (entity != null)
            {
                list.Add(entity);
                entity.Dispatcher.InvokeIfRequired(() =>
                {
                    if (entity is ContentControl)
                    {
                        var contentControl = entity as ContentControl;
                        if (contentControl.Content is UIElement)
                            list.Add(contentControl.Content);
                    }
                });
            }

            list.Add(this);

            if (Document != null)
            {
                list.Add(Document);
                var parent = Document.Parent;
                while(parent != null && (parent.GetType() == Document.GetType() || !parent.IsRoot))
                    parent = parent.Parent;
                if (parent == null)
                    parent = Document.Parent;
                if (parent != null)
                    list.Add(parent);

                if (UseIntelliSense)
                {
                    var variableDispatcher = Document.GetVariableObjectDispatcher(this);
                    variableDispatcher.ForEach(dispatcher =>
                    {
                        list.Add(dispatcher);
                    });
                }
            }
            return list;
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

            //return title;
            if (entity is ContentControl)
            {
                var contentControl = entity as ContentControl;
                if (var == contentControl.Content)
                    return var.GetType().Name.Replace('.', '_');
            }

            if (var == this)
                return "Entity";
            else if (var == Document)
                return "Document";
            else if (var == entity)
                return "Control";
            else
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
                t(entity, EventArgs.Empty);
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
#endif
#endregion

#region ICloneable Members
#if !WINDOWS_UWP && !NET_STANDARD

        public object Clone()
        {
            return new ScreenEntity(this);
        }
#endif
#endregion

#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                    handler(this, e);
            }
        }

#endregion

#region ICommandable Members
#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        String ICommandable.Name
        {
            get
            {
                return EntityName;
            }
        }

        [Browsable(false)]
#endif
        public IEnumerable CommandList
        {
            get
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                return listCommands;
            }

            set
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                listCommands.Clear();
                foreach (var v in value)
                    listCommands.Add(v as CommandManager.CommandManager);

                OnPropertyChanged("CommandList");
            }
        }
        [Browsable(false)]
        public bool WebHMISupported
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region IAnimatable Members
#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        String UFInterfaces.Animatable.IAnimatable.Name
        {
            get
            {
                return EntityName;
            }
        }

        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IEnumerable AnimationList
        {
            get 
            {
                if (listAnimations == null)
                    listAnimations = new AnimationManagerList();
                return listAnimations; 
            }

            set
            {
                if (listAnimations == null)
                    listAnimations = new AnimationManagerList();
                listAnimations.Clear();
                foreach (var v in value)
                    listAnimations.Add(v as AnimationManager.AnimationManager);

                OnPropertyChanged("AnimationList");
            }
        }

#if !NET_STANDARD
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UIElement Element
        {
            get
            {
                return entity;
            }
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Model3D Element3D
        {
            get
            {
                return entity3D;
            }
        }
#endif
#endregion

#region IEntityReference Members

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get 
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (Element != null && Element is FrameworkElement)
                {
                    var fe = Element as FrameworkElement;
                    try
                    {
                        if (!Double.IsNaN(fe.Width) && !Double.IsNaN(fe.Height))
                        {
                            Element.Measure(new Size((int)fe.Width, (int)fe.Height));
                            Element.Arrange(new Rect(new Size((int)fe.Width, (int)fe.Height)));
                            (Element as FrameworkElement).ApplyTemplate();
                            Element.UpdateLayout();
                        }
                        return RenderHelper.ElementToBitmap(fe);
                    }
                    catch (Exception)
                    {
                    }
                }
#endif
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get 
            { 
                return null; 
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
#if !WINDOWS_UWP && !NET_STANDARD
        public System.Windows.Controls.ContextMenu contextMenu
#else
        public ContextMenu contextMenu
#endif
        {
            get 
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get 
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public object ContainedObject
        {
            get 
            {
#if !NET_STANDARD
                //if (bSourceSymbolLinked)
                //    return null;
                return Element;
#else
                return null;
#endif
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get 
            {
#if !NET_STANDARD
#if !WINDOWS_UWP
                return LogicalTreeHelper.GetParent(Element); 
#else
                return Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(Element);
#endif
#else
                return null;
#endif
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get 
            {
                return null;
            }
        }

#endregion

#region INotifyPropertyVisibilityChanged Members
#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        public virtual bool this[string propertyName]
        {
            get
            {
                if (propertyName == "Max3DRotationAngleX" ||
                    propertyName == "Max3DRotationAngleY" ||
                    propertyName == "Max3DRotationAngleZ" ||
                    propertyName == "Min3DRotationAngleX" ||
                    propertyName == "Min3DRotationAngleY" ||
                    propertyName == "Min3DRotationAngleZ" ||
                    propertyName == "Max3DTranslateOffsetX" ||
                    propertyName == "Min3DTranslateOffsetX" ||
                    propertyName == "Max3DTranslateOffsetY" ||
                    propertyName == "Min3DTranslateOffsetY" ||
                    propertyName == "Max3DZoom" ||
                    propertyName == "Min3DZoom" ||
                    propertyName == "OpcuaEntityReference3DZoom")
                {
                    return Contains3DElement();
                }
                else if(propertyName == "ListItemSources")
                {
                    Type t = Entity?.GetType();
                    if (t != null && (t.GetProperty("ItemsSource") != null || t.GetProperty("DataSource") != null))
                    {
                        if (ReaderItemSources != null)
                            return false;
                        return true;
                    }
                    return false;
                }
                else if (propertyName == "ReaderItemSources")
                {
                    Type t = Entity?.GetType();
                    if (t != null && (t.GetProperty("ItemsSource") != null || t.GetProperty("DataSource") != null))
                    {
                        return true;
                    }
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
        protected virtual void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
#endif
#endregion

#region IDataErrorInfo
#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
#endif
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Expression" || propertyName == "ReverseExpression")
            {
                var expression = Expression;
                if (propertyName == "ReverseExpression")
                    expression = ReverseExpression;
                if (!String.IsNullOrEmpty(expression) && AliasHelper.GetAliasCount(expression) == 0)
                {
                    var error = ExpressionBucket.CheckExpression(expression, null);
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }

            return null;
        }
#endif

        #region Screen Compiler
        public void SetSourceSymbolLinkedResolved()
        {
            SourceSymbolLinked = false;
        }
        #endregion

        #region Unit Converter
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ClearUnitConverter()
        {
            if (lastUnitConverterSettings != null)
            {
                lastUnitConverterSettings = null;
                if (expressionUnitConverter != null)
                {
                    expressionUnitConverter.ParserError -= ExpressionEntity_ParserError;
                    expressionUnitConverter.ExecutionError -= ExpressionEntity_ExecutionError;
                    expressionUnitConverter.Dispose();
                    expressionUnitConverter = null;
                }

                if (expressionEntity != null)
                {
                    expressionEntity.ParserError -= ExpressionEntity_ParserError;
                    expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                    expressionEntity.Dispose();
                    expressionEntity = null;
                }

                var monitoredItemViewModel = OpcuaEntityReference?.MonitoredItemViewModel;
                if (monitoredItemViewModel != null && !String.IsNullOrEmpty(Expression) && Document != null)
                {
                    expressionEntity = ExpressionBucket.GetInstance(Document).AddExpression(monitoredItemViewModel, Expression, ReverseExpression, Document.mapCurrentParameteItems);
                    expressionEntity.ParserError += ExpressionEntity_ParserError;
                    expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                }

                SetDataContext();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ApplyUnitConverter(String unitlabel, String inputExpression, String outputExpression)
        {
            if (lastUnitConverterSettings != null &&
                lastUnitConverterSettings.Unitlabel == unitlabel &&
                lastUnitConverterSettings.InputExpression == inputExpression &&
                lastUnitConverterSettings.OutputExpression == outputExpression)
                return;

            lastUnitConverterSettings = new UnitConverterSettings()
            {
                Unitlabel = unitlabel,
                InputExpression = inputExpression,
                OutputExpression = outputExpression
            };

            if (expressionUnitConverter != null)
            {
                expressionUnitConverter.ParserError -= ExpressionEntity_ParserError;
                expressionUnitConverter.ExecutionError -= ExpressionEntity_ExecutionError;
                expressionUnitConverter.Dispose();
                expressionUnitConverter = null;
            }

            if (expressionEntity != null)
            {
                expressionEntity.ParserError -= ExpressionEntity_ParserError;
                expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                expressionEntity.Dispose();
                expressionEntity = null;
            }

            var monitoredItemViewModel = OpcuaEntityReference?.MonitoredItemViewModel;
            if (monitoredItemViewModel != null)
            {
                if (!String.IsNullOrEmpty(inputExpression) && Document != null)
                {
                    expressionUnitConverter = ExpressionBucket.GetInstance(Document).AddExpression(monitoredItemViewModel, inputExpression, outputExpression, Document.mapCurrentParameteItems);
                    expressionUnitConverter.ParserError += ExpressionEntity_ParserError;
                    expressionUnitConverter.ExecutionError += ExpressionEntity_ExecutionError;
                    monitoredItemViewModel = expressionUnitConverter.TempVariable;
                }

                if (!String.IsNullOrEmpty(Expression) && Document != null)
                {
                    expressionEntity = ExpressionBucket.GetInstance(Document).AddExpression(monitoredItemViewModel, Expression, ReverseExpression, Document.mapCurrentParameteItems);
                    expressionEntity.ParserError += ExpressionEntity_ParserError;
                    expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                }
            }

            SetDataContext();

            if (Element is IDynamicTagAware)
            {
                var dyamicAware = Element as IDynamicTagAware;
                dyamicAware.SetConverterLabel(unitlabel);
            }
        }

        internal void ApplyCultureFontSettings(string culture)
        {
            if (Entity == null || !(Entity is Control))
                return;

            FontSettings settings;
            if (FontSettingList == null || FontSettingList.Count == 0)
                return;
            else
            {
                settings = (from s in FontSettingList.Keys where s == culture && FontSettingList[s] != null select FontSettingList[s]).FirstOrDefault();
                if (settings == null)
                    return;
            }
            Control uic = Entity as Control;
            if (Entity is ContentControl && (Entity as ContentControl).Content is Control && !(Entity is UserControl))
                uic = (Entity as ContentControl).Content as Control;

            uic.FontFamily = new FontFamily();
            uic.FontSize = settings.FontSize > 1 ? 1 : 2;
            uic.FontWeight = settings.FontWeight == FontWeights.Heavy ? FontWeights.Light : FontWeights.Heavy;
            uic.FontStyle = settings.FontStyle == FontStyles.Oblique ? FontStyles.Italic : FontStyles.Oblique;
            uic.FontFamily = settings.FontFamily;
            uic.FontSize = settings.FontSize;
            uic.FontWeight = settings.FontWeight;
            uic.FontStyle = settings.FontStyle;
        }
#endif
        #endregion
    }
}
