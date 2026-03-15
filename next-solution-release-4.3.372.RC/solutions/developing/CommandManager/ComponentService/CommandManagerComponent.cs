using System;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using System.Windows;
using CommandManagerService.ComponentService;
#if !WINDOWS_UWP
using PropertyControl.ComponentService;
using CommandManager.PropertyDataTemplate;
using ReportManager.ComponentService;
using UFUAEditor.ComponentService;
using CommonControls.PropertyDataTemplate;
using WPFUtilities.PropertyDataTemplate;
using UFUserEditor.ComponentService;
using CommandExplorer.ComponentService;
using UnitConverterManager.ComponentService;
#endif
using UriResolver.ComponentService;
using UFProjectManager.ComponentService;
using StringManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;
using OPCUAViewModel.PropertyDataTemplate;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media.Media3D;
#endif


namespace CommandManager.ComponentService
{
    public class CommandManagerComponent : ComponentBase<ICommandManagerService>, ICommandManagerService
    {
        #region Declaration

#if !WINDOWS_UWP
        public static IPropertyControl propertyService { get; protected set; }
        public static IWorkspace workspace { get;  protected set; }
        public static bool propertyServiceAvailable { get { return propertyService != null; } }
#endif
        public static IUriRisolver uriRisolver { get; protected set; }
        public static bool uriRisolverServiceAvailable { get { return uriRisolver != null; } }

        static CommandManagerComponent commandManagerComponent;
#endregion

#region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
#endregion

#if !WINDOWS_UWP
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
#endif

        private void GetComponentInterfaces()
        {
            commandManagerComponent = this;
#if !WINDOWS_UWP
            if (propertyService == null)
                propertyService = GetService(typeof(IPropertyControl)) as IPropertyControl;
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
#endif
            if (uriRisolver == null)
                uriRisolver = GetService(typeof(IUriRisolver)) as IUriRisolver;

#if !WINDOWS_UWP
            CreatePropertyDataTemplates();
#endif
        }

#if !WINDOWS_UWP
        static IReportManager _reportManager;
        public static IReportManager reportManager
        {
            get
            {
                if (_reportManager == null)
                    _reportManager = commandManagerComponent.GetService(typeof(IReportManager)) as IReportManager;
                return _reportManager;
            }
        }

        static IUFProjectManager _projectManager;
        public static IUFProjectManager projectManager
        {
            get
            {
                if (_projectManager == null)
                    _projectManager = commandManagerComponent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                return _projectManager;
            }
        }

        IHelpProvider helpProvider;
        public IHelpProvider HelpProvider
        {
            get
            {
                if (helpProvider == null)
                    helpProvider = GetService(typeof(IHelpProvider)) as IHelpProvider;
                return helpProvider;
            }
        }

        static IStringEditorManager _stringEditorManager;
        public static IStringEditorManager stringEditorManager
        {
            get
            {
                if (_stringEditorManager == null)
                    _stringEditorManager = commandManagerComponent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return _stringEditorManager;
            }
        }

        static IUnitConverterEditorManager _unitConverterEditorManager;
        public static IUnitConverterEditorManager unitConverterEditorManager
        {
            get
            {
                if (_unitConverterEditorManager == null)
                    _unitConverterEditorManager = commandManagerComponent.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                return _unitConverterEditorManager;
            }
        }

        static IUFUAEditorManager _uFUAEditorManager;
        public static IUFUAEditorManager uFUAEditorManager
        {
            get
            {
                if (_uFUAEditorManager == null)
                    _uFUAEditorManager = commandManagerComponent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return _uFUAEditorManager;
            }
        }

        static IUFUserEditorManager _uFUserEditorManager;
        public static IUFUserEditorManager uFUserEditorManager
        {
            get
            {
                if (_uFUserEditorManager == null)
                    _uFUserEditorManager = commandManagerComponent.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return _uFUserEditorManager;
            }
        }

        static ICommandExplorer _commandExplorer;
        public static ICommandExplorer commandExplorer
        {
            get
            {
                if (_commandExplorer == null)
                    _commandExplorer = commandManagerComponent.GetService(typeof(ICommandExplorer)) as ICommandExplorer;
                return _commandExplorer;
            }
        }
        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        void CreatePropertyDataTemplates()
        {
            if (propertyService == null)
                return;

            var dt = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(ScreenUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ScreenName", typeof(Uri), typeof(OpenScreenCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(VoiceNamePropertyEditor));
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("VoiceName", typeof(String), typeof(TTSCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ScreenParameterUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ParameterFile", typeof(Uri), typeof(OpenScreenCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(GeoMapOpeningParametersEditor));
            factory.SetValue(GeoMapOpeningParametersEditor.WorkspaceProperty, workspace);
            dt.DataType = typeof(Point3D);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("GeoMapOpeningParameters", typeof(Point3D), typeof(OpenMapCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ScriptUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ScriptName", typeof(Uri), typeof(RunScriptCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(LogicUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("LogicName", typeof(Uri), typeof(RunLogicCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ReportUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ReportName", typeof(Uri), typeof(ReportCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ReportParameters.PropertyDataTemplate.ReportParametersPropertyEditor));
            dt.DataType = typeof(ReportParameters.ParameterCollection);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("Parameters", typeof(ReportParameters.ParameterCollection), typeof(ReportCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.RecipeUriPropertyEditor));
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("RecipeName", typeof(Uri), typeof(RecipeCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(CulturePropertyControlxaml));
            factory.SetValue(CulturePropertyControlxaml.WorkspaceProperty, workspace);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("CultureName", typeof(String), typeof(ChangeCultureCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ConverterPropertyControl));
            factory.SetValue(ConverterPropertyControl.WorkspaceProperty, workspace);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ConverterName", typeof(String), typeof(ChangeConverterCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(OPCUAViewModel.PropertyDataTemplate.AlarmsSourcePropertyEditor));
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("AlarmsSource", typeof(String), typeof(AlarmCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
            factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
            factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("ConnectionString", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("ConnectionString", typeof(String), typeof(ReportCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(SelectPathPropertyEditor));
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("FolderPath", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("FolderPath", typeof(String), typeof(ReportCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, Double.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
            factory.SetValue(NumericUpDownPropertyEditor.DisplayFormatStringProperty, "d");
            dt.DataType = typeof(double);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("MinValue", typeof(double), typeof(ValueCommand), dt);
            propertyService.AddPropertyEditor("MaxValue", typeof(double), typeof(ValueCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, -1d);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
            factory.SetValue(NumericUpDownPropertyEditor.DisplayFormatStringProperty, "d");
            dt.DataType = typeof(double);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("X", typeof(double), typeof(ValueCommand), dt);
            propertyService.AddPropertyEditor("Y", typeof(double), typeof(ValueCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, Double.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
            factory.SetValue(NumericUpDownPropertyEditor.DisplayFormatStringProperty, "d");
            factory.SetValue(NumericUpDownPropertyEditor.MaskUseAsDisplayFormatProperty, false);
            dt.DataType = typeof(double);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("X", typeof(double), typeof(OpenScreenCommand), dt);
            propertyService.AddPropertyEditor("Y", typeof(double), typeof(OpenScreenCommand), dt);


            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(RecipientPropertyEditor));
            factory.SetValue(RecipientPropertyEditor.WorkspaceProperty, workspace);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("Recipient", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("Recipient", typeof(String), typeof(ReportCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
            factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
            dt.DataType = typeof(string);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("From", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("FromAlias", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("MailSubject", typeof(String), typeof(AlarmCommand), dt);
            propertyService.AddPropertyEditor("MailObject", typeof(String), typeof(AlarmCommand), dt);

            propertyService.AddPropertyEditor("From", typeof(String), typeof(ReportCommand), dt);
            propertyService.AddPropertyEditor("FromAlias", typeof(String), typeof(ReportCommand), dt);
            propertyService.AddPropertyEditor("MailSubject", typeof(String), typeof(ReportCommand), dt);
            propertyService.AddPropertyEditor("MailObject", typeof(String), typeof(ReportCommand), dt);

            propertyService.AddPropertyEditor("Speak", typeof(String), typeof(TTSCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ExpressionPropertyEditor));
            factory.SetValue(ExpressionPropertyEditor.WorkspaceProperty, workspace); 
            dt.DataType = typeof(string);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("Expression", typeof(String), typeof(CommandManager), dt);
            propertyService.AddPropertyEditor("ReverseExpression", typeof(String), typeof(ValueCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(CommandSettingsPropertyEditor));
            dt.DataType = typeof(CommandManager);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("CommandSettings", typeof(CommandManager), typeof(CommandManager), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
            factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
            factory.SetValue(TextPropertyEditor.HideEditButtonProperty, true);
            dt.DataType = typeof(string);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("Value", typeof(String), typeof(ValueCommand), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.IdUnitConverterPropertyEditor));
            factory.SetValue(IdUnitConverterPropertyEditor.WorkspaceProperty, workspace);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            propertyService.AddPropertyEditor("IdUnitConverter", typeof(String), typeof(ValueCommand), dt);

        }
#endif
    }
}
