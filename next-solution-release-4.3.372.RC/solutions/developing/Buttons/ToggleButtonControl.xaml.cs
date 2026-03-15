using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonControls.PropertyDataTemplate;
using Converters;
using DocumentManager.ComponentService;
using UFInterfaces.PropertyControl;
using Utilities;
using WPFUtilities.Converters;
using WPFUtilities.PropertyDataTemplate;
using UFInterfaces;
using AuditTrace;
using OPCUAViewModel;
using ScreenSettings;
using Utilities.WPF;
using Utilities.Converters;

namespace Buttons
{
    /// <summary>
    /// Interaction logic for ToggleButtonControl.xaml
    /// </summary>
    public partial class ToggleButtonControl : CheckBox, IDisposable, IContainPropertyEditors, IDataErrorInfo, INotifyPropertyVisibilityChanged
    {
        #region DP

        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ToggleButtonControl), new UIPropertyMetadata(new CornerRadius(3d)));

        public CornerRadius CornerRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        #endregion
        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ToggleButtonControl), new UIPropertyMetadata(TextWrapping.Wrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnTextWrappingChanged((TextWrapping)e.OldValue, (TextWrapping)e.NewValue);
        }

        protected virtual TextWrapping OnCoerceTextWrapping(TextWrapping value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextWrappingChanged(TextWrapping oldValue, TextWrapping newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("UButtonOptions")]
        public TextWrapping TextWrapping
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextWrapping)GetValue(TextWrappingProperty);
            }
            set
            {
                SetValue(TextWrappingProperty, value);
            }
        }

        #endregion

        #region TextAlignment
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ToggleButtonControl), new UIPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnTextAlignmentChanged), new CoerceValueCallback(OnCoerceTextAlignment)));

        private static object OnCoerceTextAlignment(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceTextAlignment((TextAlignment)value);
            else
                return value;
        }

        private static void OnTextAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnTextAlignmentChanged((TextAlignment)e.OldValue, (TextAlignment)e.NewValue);
        }

        protected virtual TextAlignment OnCoerceTextAlignment(TextAlignment value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextAlignmentChanged(TextAlignment oldValue, TextAlignment newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("UButtonOptions")]
        public TextAlignment TextAlignment
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextAlignment)GetValue(TextAlignmentProperty);
            }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        #endregion

        #region ControlFontSettings
        [Browsable(false)]
        [SvgValueConverter(false)]
        public FontSettings ControlFontSettings { get; set; }

        #endregion

        #region PressedIcon
        public static readonly DependencyProperty PressedIconProperty = DependencyProperty.Register("PressedIcon", typeof(Uri), typeof(ToggleButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPressedIconChanged), new CoerceValueCallback(OnCoercePressedIcon)));

        private static object OnCoercePressedIcon(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoercePressedIcon((Uri)value);
            else
                return value;
        }

        private static void OnPressedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnPressedIconChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoercePressedIcon(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPressedIconChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied && CurrentStateName == VisualStates.PressedStateName)
                UpdateImage(newValue);
        }

        [Category("AdvancedOptions")]
        public Uri PressedIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(PressedIconProperty);
            }
            set
            {
                SetValue(PressedIconProperty, value);
            }
        }

        #endregion

        #region ReleasedIcon
        public static readonly DependencyProperty ReleasedIconProperty = DependencyProperty.Register("ReleasedIcon", typeof(Uri), typeof(ToggleButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReleasedIconChanged), new CoerceValueCallback(OnCoerceReleasedIcon)));

        private static object OnCoerceReleasedIcon(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceReleasedIcon((Uri)value);
            else
                return value;
        }

        private static void OnReleasedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnReleasedIconChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceReleasedIcon(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReleasedIconChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied && CurrentStateName == VisualStates.NormalStateName)
                UpdateImage(newValue);
        }

        [Category("AdvancedOptions")]
        public Uri ReleasedIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(ReleasedIconProperty);
            }
            set
            {
                SetValue(ReleasedIconProperty, value);
            }
        }

        #endregion

        #region CheckedIcon
        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.Register("CheckedIcon", typeof(Uri), typeof(ToggleButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCheckedIconChanged), new CoerceValueCallback(OnCoerceCheckedIcon)));

        private static object OnCoerceCheckedIcon(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceCheckedIcon((Uri)value);
            else
                return value;
        }

        private static void OnCheckedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnCheckedIconChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceCheckedIcon(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCheckedIconChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied && CurrentStateName == VisualStates.CheckedStatedName)
                UpdateImage(newValue);
        }

        [Category("AdvancedOptions")]
        public Uri CheckedIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(CheckedIconProperty);
            }
            set
            {
                SetValue(CheckedIconProperty, value);
            }
        }

        #endregion

        #region DisabledIcon
        public static readonly DependencyProperty DisabledIconProperty = DependencyProperty.Register("DisabledIcon", typeof(Uri), typeof(ToggleButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDisabledIconChanged), new CoerceValueCallback(OnCoerceDisabledIcon)));

        private static object OnCoerceDisabledIcon(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceDisabledIcon((Uri)value);
            else
                return value;
        }

        private static void OnDisabledIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnDisabledIconChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceDisabledIcon(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisabledIconChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied && CurrentStateName == VisualStates.DisabledStateName)
                UpdateImage(newValue);
        }

        [Category("AdvancedOptions")]
        public Uri DisabledIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(DisabledIconProperty);
            }
            set
            {
                SetValue(DisabledIconProperty, value);
            }
        }

        #endregion

        #region OverlapedImageText
        public static readonly DependencyProperty OverlapedImageTextProperty = DependencyProperty.Register("OverlapedImageText", typeof(bool), typeof(ToggleButtonControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnOverlapedImageTextChanged), new CoerceValueCallback(OnCoerceOverlapedImageText)));

        private static object OnCoerceOverlapedImageText(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceOverlapedImageText((bool)value);
            else
                return value;
        }

        private static void OnOverlapedImageTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnOverlapedImageTextChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceOverlapedImageText(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOverlapedImageTextChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied)
                UpdateOverlapped();
        }
        [Category("AdvancedOptions")]
        public bool OverlapedImageText
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(OverlapedImageTextProperty);
            }
            set
            {
                SetValue(OverlapedImageTextProperty, value);
            }
        }

        #endregion

        #region ImageStretch
        public static readonly DependencyProperty ImageStretchProperty = DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(ToggleButtonControl), new UIPropertyMetadata(Stretch.Uniform, new PropertyChangedCallback(OnImageStretchChanged), new CoerceValueCallback(OnCoerceImageStretch)));

        private static object OnCoerceImageStretch(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceImageStretch((Stretch)value);
            else
                return value;
        }

        private static void OnImageStretchChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnImageStretchChanged((Stretch)e.OldValue, (Stretch)e.NewValue);
        }

        protected virtual Stretch OnCoerceImageStretch(Stretch value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnImageStretchChanged(Stretch oldValue, Stretch newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied)
                UpdateStretch();
        }

        [Category("AdvancedOptions")]
        public Stretch ImageStretch
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Stretch)GetValue(ImageStretchProperty);
            }
            set
            {
                SetValue(ImageStretchProperty, value);
            }
        }

        #endregion

        #region BackgroundOn
        public static readonly DependencyProperty BackgroundOnProperty = DependencyProperty.Register("BackgroundOn", typeof(Brush), typeof(ToggleButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundOnChanged), new CoerceValueCallback(OnCoerceBackgroundOn)));

        private static object OnCoerceBackgroundOn(DependencyObject o, object value)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                return control.OnCoerceBackgroundOn((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundOnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ToggleButtonControl control = o as ToggleButtonControl;
            if (control != null)
                control.OnBackgroundOnChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceBackgroundOn(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBackgroundOnChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (isTemplateApplied)
                UpdateStyle();
        }

        [Category("Style")]
        public Brush BackgroundOn
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BackgroundOnProperty);
            }
            set
            {
                SetValue(BackgroundOnProperty, value);
            }
        }

        #endregion

        #endregion

        #region Declarations
        Image backImageInTemplate;
        Image sideImageInTemplate;
        Border backBorderInTemplate;
        VisualStateGroup commonGroup;
        VisualStateGroup checkedGroup;
        Brush oldBackground;

        string lastCheckedStateName;
        string lastCommonStateName;

        SolidColorBrush _Transparent = new SolidColorBrush(Colors.Transparent);

        AuditTraceViewModel auditTraceViewModel;
        IValueConverter previousConverter;

        bool isBindingApplied;
        bool isTemplateApplied;
        #endregion

        #region Constructors
        public ToggleButtonControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            bool bLoaded = false;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                }
            };

            DataContextChanged += (s, e) =>
            {
                if (bDispose)
                    return;

                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this))
                {
                    if (auditTraceViewModel != null)
                    {
                        auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                        auditTraceViewModel.Dispose();
                        auditTraceViewModel = null;
                    }

                    var monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel != null)
                    {
                        if (monitoredItemViewModel.ReferenceViewModel != null)
                            monitoredItemViewModel = monitoredItemViewModel.ReferenceViewModel;
                        var document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                        if (!RunningOnServer && document != null && monitoredItemViewModel.monitoredItem != null)
                        {
                            var name = document.GetEntityName(this, bAdd: false);
                            if (!String.IsNullOrEmpty(name) && document.MapScreenEntities.ContainsKey(name))
                            {
                                var entity = document.MapScreenEntities[name];
                                auditTraceViewModel = new AuditTraceViewModel(monitoredItemViewModel, entity, document, document.SessionString)
                                {
                                    Control = entity.Element
                                };
                                auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                                IsEnabled = false;

                                var bindingExpression = GetBindingExpression(IsCheckedProperty);
                                if (bindingExpression != null)
                                {
                                    var binding = DependencyObjectExtensions.CloneBinding(bindingExpression.ParentBinding) as Binding;
                                    if (!isBindingApplied)
                                    {
                                        isBindingApplied = true;
                                        previousConverter = binding.Converter;
                                    }

                                    var converter = new AuditTrace.Converters.AuditTraceValueConverter(auditTraceViewModel)
                                    {
                                        Converter = previousConverter
                                    };

                                    if (previousConverter != null)
                                    {
                                        var combinedConverter = new CombiningConverter()
                                        {
                                            Converter1 = converter,
                                            Converter2 = previousConverter
                                        };
                                        binding.Converter = combinedConverter;
                                    }
                                    else
                                    {
                                        binding.Converter = converter;
                                    }
                                    binding.ConverterParameter = bindingExpression.ParentBinding.ConverterParameter;
                                    SetBinding(IsCheckedProperty, binding);
                                }
                            }
                        }
                    }
                }
            };
        }
        #endregion

        #region Event Handlers
        void OnAuditFetched(object s, EventArgs ev)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose)
                    return;

                IsEnabled = true;
                var entity = auditTraceViewModel?.Entity as ScreenSettings.Entities.ScreenEntity;
                if (entity != null)
                    entity.ReexecuteEnable();
            });
        }
        #endregion


        #region Overrides
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            backImageInTemplate = (Image)this.Template.FindName("BackImage", this);
            sideImageInTemplate = (Image)this.Template.FindName("SideImage", this);
            backBorderInTemplate = (Border)this.Template.FindName("Chrome_Copy", this);

            isTemplateApplied = backImageInTemplate != null && sideImageInTemplate != null && backBorderInTemplate != null;

            if (!isTemplateApplied)
                return;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var groups = VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(this, 0) as FrameworkElement);

                if (commonGroup != null)
                    commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;
                if (checkedGroup != null)
                    checkedGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

                commonGroup = groups.Cast<VisualStateGroup>().FirstOrDefault(g => g.Name == VisualStates.CommonStatesGroupName);
                if (commonGroup != null)
                    commonGroup.CurrentStateChanged += VisualStateGroup_CurrentStateChanged;

                checkedGroup = groups.Cast<VisualStateGroup>().FirstOrDefault(g => g.Name == VisualStates.CheckStatesGroupName);
                if (checkedGroup != null)
                    checkedGroup.CurrentStateChanged += VisualStateGroup_CurrentStateChanged;
            }

            UpdateStyle();
        }
        #endregion

        #region Methods
        private void UpdateStyle()
        {
            UpdateStretch();
            UpdateOverlapped();
            UpdateBackground();
            UpdateImage();
        }

        void UpdateStretch()
        {
            backImageInTemplate.Stretch = sideImageInTemplate.Stretch = ImageStretch;
        }

        void UpdateOverlapped()
        {
            if (OverlapedImageText)
            {
                backImageInTemplate.Visibility = Visibility.Visible;
                sideImageInTemplate.Visibility = Visibility.Collapsed;
            }
            else
            {
                backImageInTemplate.Visibility = Visibility.Collapsed;
                sideImageInTemplate.Visibility = Visibility.Visible;
            }
        }

        void UpdateBackground()
        {
            if (backBorderInTemplate != null)
                backBorderInTemplate.Visibility = Background == null || Background.ToString() == _Transparent.ToString() ? Visibility.Collapsed : Visibility.Visible;
        }

        void UpdateImage()
        {
            var currentStateName = CurrentStateName;
            if (currentStateName != null)
                UpdateImage(currentStateName);
        }

        void UpdateImage(string newStateName)
        {
            switch (newStateName)
            {
                case VisualStates.UncheckedStatedName:
                    lastCheckedStateName = newStateName;
                    if (lastCommonStateName != VisualStates.DisabledStateName)
                        UpdateImage(ReleasedIcon);
                    if (oldBackground != null)
                    {
                        Background = oldBackground;
                        oldBackground = null;
                    }
                    break;
                case VisualStates.CheckedStatedName:
                    lastCheckedStateName = newStateName;
                    if (lastCommonStateName != VisualStates.DisabledStateName)
                        UpdateImage(CheckedIcon);
                    if (BackgroundOn != null && oldBackground == null)
                    {
                        oldBackground = Background;
                        Background = BackgroundOn;
                    }
                    break;
                case VisualStates.NormalStateName:
                    lastCommonStateName = newStateName;
                    if (lastCheckedStateName != null)
                        UpdateImage(lastCheckedStateName);
                    break;
                case VisualStates.DisabledStateName:
                    lastCommonStateName = newStateName;
                    UpdateImage(DisabledIcon);
                    break;
                case VisualStates.PressedStateName:
                    lastCommonStateName = newStateName;
                    UpdateImage(PressedIcon);
                    break;
                default:
                    break;
            }
        }

        void UpdateImage(Uri icon)
        {
            if (icon != null)
            {
                try
                {
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(UriToAbsoluteUriConverter.Convert(icon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images).GetPathString(), UriKind.RelativeOrAbsolute);
                    bmp.EndInit();

                    backImageInTemplate.Source = bmp;
                    sideImageInTemplate.Source = bmp;
                }
                catch
                { }
            }
            else
            {
                backImageInTemplate.Source = null;
                sideImageInTemplate.Source = null;
            }
        }

        void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateImage(e.NewState.Name);
        }
        #endregion

        #region Properties
        String CurrentStateName
        {
            get
            {
                if (commonGroup != null && commonGroup.CurrentState != null &&
                    (commonGroup.CurrentState.Name == VisualStates.DisabledStateName ||
                    commonGroup.CurrentState.Name == VisualStates.PressedStateName))
                    return commonGroup.CurrentState.Name;
                else if (checkedGroup != null && checkedGroup.CurrentState != null)
                    return checkedGroup.CurrentState.Name;
                else 
                    return VisualStates.UncheckedStatedName;
            }
        }

        [Browsable(false)]
        public bool RunningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion

        #region IDIsposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            if (auditTraceViewModel != null)
            {
                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                auditTraceViewModel.Dispose();
                auditTraceViewModel = null;
            }

            if (commonGroup != null)
                commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;
            if (checkedGroup != null)
                checkedGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

            backImageInTemplate = null;
            sideImageInTemplate = null;
        }
        #endregion

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                IDocument Document;
                IWorkspace Workspace;
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                Workspace = Document?.GetService(typeof(IWorkspace)) as IWorkspace;

                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                // Defines Data Template for 'DocumentPathProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.FilterProperty, Properties.Resources.AllPictureFiles);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask); 
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Images); 
                factory.SetValue(SourceFilePropertyEditor.DefaultExtProperty, "ico");
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ReleasedIconProperty, dt);
                mapDataTemplates.Add(PressedIconProperty, dt);
                mapDataTemplates.Add(CheckedIconProperty, dt);
                mapDataTemplates.Add(DisabledIconProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region INotifyPropertyVisibilityChanged
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == BackgroundProperty.Name)
                {
                    var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    if (doc?.MapScreenEntities[doc.GetEntityName(this)].SourceSymbolLinked != false && Helpers.TagUIElementHelper.IsStyledSymbol(this))
                        return Helpers.TagUIElementHelper.TagNameExists(this, Properties.Settings.Default.TagBackground);
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
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region IDataErrorInfo Members

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

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == ReleasedIconProperty.Name)
                {
                    if (ReleasedIcon != null && UriToAbsoluteUriConverter.Convert(ReleasedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                    {
                        return Properties.Resources.InvalidSource;
                    }
                }
                else if (propertyName == PressedIconProperty.Name)
                {
                    if (PressedIcon != null && UriToAbsoluteUriConverter.Convert(PressedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                    {
                        return Properties.Resources.InvalidSource;
                    }
                }
                else if (propertyName == CheckedIconProperty.Name)
                {
                    if (CheckedIcon != null && UriToAbsoluteUriConverter.Convert(CheckedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                    {
                        return Properties.Resources.InvalidSource;
                    }
                }
                else if (propertyName == DisabledIconProperty.Name)
                {
                    if (DisabledIcon != null && UriToAbsoluteUriConverter.Convert(DisabledIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                    {
                        return Properties.Resources.InvalidSource;
                    }
                }

                return null;
            }
        }

        #endregion


    }
}
