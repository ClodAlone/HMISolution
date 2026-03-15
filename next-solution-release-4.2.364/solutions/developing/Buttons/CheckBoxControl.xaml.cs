using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AuditTrace;
using CommonControls.PropertyDataTemplate;
using Converters;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using ScreenSettings;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.Converters;
using Utilities.WPF;
using WPFUtilities.Converters;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UFInterfaces;
using System.Xml.Serialization;

namespace Buttons
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class CheckBoxControl : CheckBox, INotifyPropertyVisibilityChanged, IDisposable
    {
        #region DP

        #region BackgroundOn
        public static readonly DependencyProperty BackgroundOnProperty = DependencyProperty.Register("BackgroundOn", typeof(Brush), typeof(CheckBoxControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundOnChanged), new CoerceValueCallback(OnCoerceBackgroundOn)));

        private static object OnCoerceBackgroundOn(DependencyObject o, object value)
        {
            CheckBoxControl control = o as CheckBoxControl;
            if (control != null)
                return control.OnCoerceBackgroundOn((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundOnChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControl control = o as CheckBoxControl;
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
            if (isTemplateApplied && !DesignerProperties.GetIsInDesignMode(this))
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

        #region SvgBackground
        public static readonly DependencyProperty SvgBackgroundProperty = DependencyProperty.Register("SvgBackground", typeof(string), typeof(CheckBoxControl), new UIPropertyMetadata(null));
        [SvgValueConverter(typeof(ConvertToSvgValue), RequiredKey = true, NeedSVGUrlBrushes = true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [XmlIgnore]
        public string SvgBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SvgBackgroundProperty);
            }
            set
            {
                SetValue(SvgBackgroundProperty, value);
            }
        }
        #endregion

        #region SvgBackgroundOn
        public static readonly DependencyProperty SvgBackgroundOnProperty = DependencyProperty.Register("SvgBackgroundOn", typeof(string), typeof(CheckBoxControl), new UIPropertyMetadata(null));
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [XmlIgnore]
        public string SvgBackgroundOn
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SvgBackgroundOnProperty);
            }
            set
            {
                SetValue(SvgBackgroundOnProperty, value);
            }
        }
        #endregion
        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(CheckBoxControl), new UIPropertyMetadata(TextWrapping.Wrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            CheckBoxControl control = o as CheckBoxControl;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControl control = o as CheckBoxControl;
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
        [Category("AdvancedOptions")]
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
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(CheckBoxControl), new UIPropertyMetadata(TextAlignment.Left, new PropertyChangedCallback(OnTextAlignmentChanged), new CoerceValueCallback(OnCoerceTextAlignment)));

        private static object OnCoerceTextAlignment(DependencyObject o, object value)
        {
            CheckBoxControl control = o as CheckBoxControl;
            if (control != null)
                return control.OnCoerceTextAlignment((TextAlignment)value);
            else
                return value;
        }

        private static void OnTextAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControl control = o as CheckBoxControl;
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
        [Category("AdvancedOptions")]
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

        #region OptionBorderBrush
        public static readonly DependencyProperty OptionBorderBrushProperty = DependencyProperty.Register("OptionBorderBrush", typeof(Brush), typeof(CheckBoxControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 201, 255))));
        public Brush OptionBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(OptionBorderBrushProperty);
            }
            set
            {
                SetValue(OptionBorderBrushProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        Dictionary<UIElement, Brush> mapOldBrush;
        VisualStateGroup commonGroup;

        AuditTraceViewModel auditTraceViewModel;
        IValueConverter previousConverter;

        bool isBindingApplied;
        bool isTemplateApplied;
        #endregion

        #region Constructors
        public CheckBoxControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            OverrideBaseProperties();

            DataContextChanged += (s, e) =>
            {
                if (bDisposed)
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
                if (bDisposed)
                    return;

                IsEnabled = true;
                var entity = auditTraceViewModel?.Entity as ScreenSettings.Entities.ScreenEntity;
                if (entity != null)
                    entity.ReexecuteEnable();
            });
        }
        #endregion

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitBrush();
            isTemplateApplied = mapOldBrush != null && mapOldBrush.Count > 0;

            if (!isTemplateApplied)
                return;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (commonGroup != null)
                    commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

                var groups = VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(this, 0) as FrameworkElement);
                commonGroup = groups.Cast<VisualStateGroup>().FirstOrDefault(g => g.Name == VisualStates.CheckStatesGroupName);
                if (commonGroup != null)
                    commonGroup.CurrentStateChanged += VisualStateGroup_CurrentStateChanged;
            }

            UpdateStyle();
        }
        #endregion

        #region Methods
        void InitBrush()
        {
            if (mapOldBrush == null)
                mapOldBrush = new Dictionary<UIElement, Brush>();
            else
                mapOldBrush.Clear();

            DependencyObjectExtensions.CleanChildrenOfTypeCache();

            (from c in this.GetVisualChildrenOfType<Panel>()
                where (c.Tag as String) == Properties.Settings.Default.TagBackground
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Background);
                });
            (from c in this.GetVisualChildrenOfType<Control>()
                where (c.Tag as String) == Properties.Settings.Default.TagBackground
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Background);
                });
            (from c in this.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
                where (c.Tag as String) == Properties.Settings.Default.TagBackground
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Fill);
                });
            (from c in this.GetVisualChildrenOfType<Border>()
                where (c.Tag as String) == Properties.Settings.Default.TagBackground
                select c).ToList().ForEach(child =>
                {
                    if (!mapOldBrush.ContainsKey(child))
                        mapOldBrush.Add(child, child.Background);
                });
        }

        void UpdateStyle()
        {
            var currentStateName = CurrentStateName;
            if (currentStateName != null)
                UpdateBackColor(currentStateName);
        }

        void UpdateBackColor(string newStateName)
        {
            if (CommonControls.CommonProperties.GetIsBrushAnimating(this))
                return;

            switch (newStateName)
            {
                case VisualStates.UncheckedStatedName:
                    SetBackColor(null);
                    break;
                case VisualStates.CheckedStatedName:
                case VisualStates.IndeterminateStatedName:
                    SetBackColor(BackgroundOn);
                    break;
                default:
                    break;
            }
        }

        void SetBackColor(Brush brush)
        {
            if (mapOldBrush != null)
            {
                mapOldBrush.Keys.ToList().ForEach(control =>
                {
                    if (control is Panel)
                        (control as Panel).Background = brush ?? mapOldBrush[control];
                    else if (control is Control)
                        (control as Control).Background = brush ?? mapOldBrush[control];
                    else if (control is Shape)
                        (control as Shape).Fill = brush ?? mapOldBrush[control];
                    else if (control is Border)
                        (control as Border).Background = brush ?? mapOldBrush[control];
                });
            }
        }

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(CheckBoxControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(CheckBoxControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            if (isTemplateApplied)
            {
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    InitBrush();
                    UpdateStyle();
                }
                else
                    SetBackColor(Background);
            }
        }

        void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateBackColor(e.NewState.Name);
        }
        #endregion

        #region Properties
        String CurrentStateName
        {
            get
            {
                if (commonGroup == null || commonGroup.CurrentState == null)
                    return VisualStates.UncheckedStatedName;
                else
                    return commonGroup.CurrentState.Name;
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
                if (propertyName == BackgroundProperty.Name || 
                    propertyName == BackgroundOnProperty.Name)
                {
                    var screenDocument = ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    if (screenDocument != null)
                    {
                        var name = screenDocument.GetEntityName(this, bAdd: false);
                        if (!String.IsNullOrEmpty(name) && screenDocument.MapScreenEntities.ContainsKey(name))
                        {
                            var entity = screenDocument.MapScreenEntities[name];
                            var selected = (entity as IEntityReference).ContainedObject;
                            if (selected is ContentControl &&
                                (selected as ContentControl).Content is UIElement &&
                                !(selected is UserControl))
                            {
                                // is a styled symbol.
                                return mapOldBrush != null && mapOldBrush.Count > 0;
                            }
                        }
                    }
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

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            DetachOverrideBaseProperties();

            if (auditTraceViewModel != null)
            {
                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                auditTraceViewModel.Dispose();
                auditTraceViewModel = null;
            }

            if (commonGroup != null)
                commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

            if (mapOldBrush != null)
                mapOldBrush.Clear();
            mapOldBrush = null;
        }
        #endregion
    }
}
