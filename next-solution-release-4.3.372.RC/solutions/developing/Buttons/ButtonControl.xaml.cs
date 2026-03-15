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
using CommonControls.PropertyDataTemplate;
using Converters;
using DocumentManager.ComponentService;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.WPF;
using WPFUtilities.Converters;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UFInterfaces;
using System.Xml.Serialization;
using ScreenSettings;
using System.IO;

namespace Buttons
{
    /// <summary>
    /// Interaction logic for ButtonControl.xaml
    /// </summary>
    public partial class ButtonControl : Button, IDisposable, IContainPropertyEditors, IDataErrorInfo, INotifyPropertyVisibilityChanged
    {
        #region DP

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ContentProperty, typeof(ButtonControl));
            dpd.AddValueChangedSafe(this, OnContentChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ContentProperty, typeof(ButtonControl));
            dpd.RemoveValueChangedSafe(this, OnContentChanged);
        }
        private void OnContentChanged(object sender, EventArgs e)
        {
            var control = sender as ButtonControl;
            if (control != null)
            {
                control.OnContentChanged();
            }
        }
        protected virtual void OnContentChanged()
        {
            bInitializing = true;
            Text = Content?.ToString();
        }
        bool bInitializing;
        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(ButtonControl), new UIPropertyMetadata("", new PropertyChangedCallback(OnTextChanged)));

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                control.OnTextChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual void OnTextChanged(String oldValue, String newValue)
        {
            if (oldValue != newValue && !bInitializing)
                Content = newValue;

            bInitializing = false;
        }

        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use the Content property insted of this.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public String Text
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return Content?.ToString();
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
        #endregion

        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ButtonControl), new UIPropertyMetadata(TextWrapping.Wrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ButtonControl), new UIPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnTextAlignmentChanged), new CoerceValueCallback(OnCoerceTextAlignment)));

        private static object OnCoerceTextAlignment(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceTextAlignment((TextAlignment)value);
            else
                return value;
        }

        private static void OnTextAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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

        #region ControlFontSettings
        [Browsable(false)]
        [SvgValueConverter(false)]
        public FontSettings ControlFontSettings  { get; set;}

        #endregion

        #region PressedIcon
        public static readonly DependencyProperty PressedIconProperty = DependencyProperty.Register("PressedIcon", typeof(Uri), typeof(ButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPressedIconChanged), new CoerceValueCallback(OnCoercePressedIcon)));

        private static object OnCoercePressedIcon(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoercePressedIcon((Uri)value);
            else
                return value;
        }

        private static void OnPressedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        [SvgValueConverter(typeof(ConvertToSvgValue), RequiredKey = true)]
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
        public static readonly DependencyProperty ReleasedIconProperty = DependencyProperty.Register("ReleasedIcon", typeof(Uri), typeof(ButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReleasedIconChanged), new CoerceValueCallback(OnCoerceReleasedIcon)));

        private static object OnCoerceReleasedIcon(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceReleasedIcon((Uri)value);
            else
                return value;
        }

        private static void OnReleasedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        [SvgValueConverter(typeof(ConvertToSvgValue), RequiredKey = true)]
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
        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.Register("CheckedIcon", typeof(Uri), typeof(ButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCheckedIconChanged), new CoerceValueCallback(OnCoerceCheckedIcon)));

        private static object OnCoerceCheckedIcon(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceCheckedIcon((Uri)value);
            else
                return value;
        }

        private static void OnCheckedIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        [SvgValueConverter(typeof(ConvertToSvgValue), RequiredKey = true)]
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
        public static readonly DependencyProperty DisabledIconProperty = DependencyProperty.Register("DisabledIcon", typeof(Uri), typeof(ButtonControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDisabledIconChanged), new CoerceValueCallback(OnCoerceDisabledIcon)));

        private static object OnCoerceDisabledIcon(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceDisabledIcon((Uri)value);
            else
                return value;
        }

        private static void OnDisabledIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        [SvgValueConverter(typeof(ConvertToSvgValue), RequiredKey = true)]
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
        public static readonly DependencyProperty OverlapedImageTextProperty = DependencyProperty.Register("OverlapedImageText", typeof(bool), typeof(ButtonControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnOverlapedImageTextChanged), new CoerceValueCallback(OnCoerceOverlapedImageText)));

        private static object OnCoerceOverlapedImageText(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceOverlapedImageText((bool)value);
            else
                return value;
        }

        private static void OnOverlapedImageTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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
        public static readonly DependencyProperty ImageStretchProperty = DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(ButtonControl), new UIPropertyMetadata(Stretch.Uniform, new PropertyChangedCallback(OnImageStretchChanged), new CoerceValueCallback(OnCoerceImageStretch)));

        private static object OnCoerceImageStretch(DependencyObject o, object value)
        {
            ButtonControl control = o as ButtonControl;
            if (control != null)
                return control.OnCoerceImageStretch((Stretch)value);
            else
                return value;
        }

        private static void OnImageStretchChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ButtonControl control = o as ButtonControl;
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


        #region SvgBackground
        public static readonly DependencyProperty SvgBackgroundProperty = DependencyProperty.Register("SvgBackground", typeof(string), typeof(ButtonControl), new UIPropertyMetadata(null));
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


        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonControl), new UIPropertyMetadata(new CornerRadius(3d)));

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

        #endregion

        #region Declarations
        Image backImageInTemplate;
        Image sideImageInTemplate;
        VisualStateGroup commonGroup;

        bool isTemplateApplied;
        #endregion

        #region Constructors
        public ButtonControl()
        {
            InitializeComponent();
            
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            bool bLoaded = false;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    OverrideBaseProperties();
                }
            };
        }
        #endregion

        #region Overrides
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            backImageInTemplate = (Image)this.Template.FindName("BackImage", this);
            sideImageInTemplate = (Image)this.Template.FindName("SideImage", this);

            isTemplateApplied = backImageInTemplate != null && sideImageInTemplate != null;

            if (!isTemplateApplied)
                return;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (commonGroup != null)
                    commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

                var groups = VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(this, 0) as FrameworkElement);
                commonGroup = groups.Cast<VisualStateGroup>().FirstOrDefault(g => g.Name == VisualStates.CommonStatesGroupName);
                if (commonGroup != null)
                    commonGroup.CurrentStateChanged += VisualStateGroup_CurrentStateChanged;
            }

            UpdateStyle();
        }
        #endregion

        #region Methods
        void UpdateStyle()
        {
            UpdateStretch();
            UpdateOverlapped();
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
                case VisualStates.NormalStateName:
                    UpdateImage(ReleasedIcon);
                    break;
                case VisualStates.MouseOverStateName:
                    UpdateImage(ReleasedIcon);
                    break;
                case VisualStates.DisabledStateName:
                    UpdateImage(DisabledIcon);
                    break;
                case VisualStates.PressedStateName:
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
                if (commonGroup == null || commonGroup.CurrentState == null)
                    return VisualStates.NormalStateName;
                else
                    return commonGroup.CurrentState.Name;
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

            if (commonGroup != null)
                commonGroup.CurrentStateChanged -= VisualStateGroup_CurrentStateChanged;

            DetachOverrideBaseProperties();
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

                //dt = new DataTemplate();
                //factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                //factory.SetValue(TextPropertyEditor.WorkspaceProperty, Workspace);
                //dt.DataType = typeof(String);
                //dt.VisualTree = factory;
                //mapDataTemplates.Add(TextProperty, dt);

                return mapDataTemplates;
            }
        }

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
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ReleasedIcon")
            {
                if (ReleasedIcon != null && UriToAbsoluteUriConverter.Convert(ReleasedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                {
                    return Properties.Resources.InvalidSource;
                }
            }
            else if (propertyName == "PressedIcon")
            {
                if (PressedIcon != null && UriToAbsoluteUriConverter.Convert(PressedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                {
                    return Properties.Resources.InvalidSource;
                }
            }
            else if (propertyName == "CheckedIcon")
            {
                if (CheckedIcon != null && UriToAbsoluteUriConverter.Convert(CheckedIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                {
                    return Properties.Resources.InvalidSource;
                }
            }
            else if (propertyName == "DisabledIcon")
            {
                if (DisabledIcon != null && UriToAbsoluteUriConverter.Convert(DisabledIcon, ScreenSettings.ScreenDocument.GetScreenDocument(this), SpecialFolders.Images) == null)
                {
                    return Properties.Resources.InvalidSource;
                }
            }


            return null;
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

        private void Button_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            VisualStateManager.GoToState((Button)sender, "Pressed", false);
        }

        private void Button_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            VisualStateManager.GoToState((Button)sender, "MouseOver", false);
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState((Button)sender, "Normal", false);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState((Button)sender, "MouseOver", false);
        }
    }


    internal class ConvertToSvgValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender is ButtonControl)
                return new Dictionary<string, Brush>() {
                            { "SvgBackground", (sender as ButtonControl).ReadLocalValue(ButtonControl.BackgroundProperty) == DependencyProperty.UnsetValue ? null : (sender as ButtonControl).Background }
                        };
            return null;
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            if (sender is CheckBoxControl)
            {
                var chk = sender as CheckBoxControl;
                var ret = new Dictionary<string, Brush>() {
                            { "SvgBackground", chk.ReadLocalValue(CheckBoxControl.BackgroundProperty) == DependencyProperty.UnsetValue ? null : (sender as CheckBoxControl).Background },
                            { "SvgBackgroundOn", chk.ReadLocalValue(CheckBoxControl.BackgroundOnProperty) == DependencyProperty.UnsetValue ? null : (sender as CheckBoxControl).BackgroundOn }
                        };
                var doc = document as ScreenDocument;
                if (doc?.MapScreenEntities[doc.GetEntityName(chk)].SourceSymbolLinked != false)
                    ret["BackgroundOn"] = (sender as CheckBoxControl).ReadLocalValue(CheckBoxControl.BackgroundOnProperty) == DependencyProperty.UnsetValue ? null : (sender as CheckBoxControl).BackgroundOn;
                return ret;
            }
            else
                return ConvertFromStorageType(value, sender);
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null || value == null || document == null || !(document is IDocument))
                return null;
            if (!(value is Uri)) {
                if (sender is CheckBoxControl)
                    return value;
                else
                    return null;
            }

            IDocument doc = document as IDocument;
            Uri uri = value as Uri;
            if (File.Exists(uri.GetPathString()))
                return uri.GetPathString();

            return WPFUtilities.ImageHelper.GetBindingSource(new UriToUriAbsoluteImageConverter() 
            { AbsolutePath = doc.GetSpecialFolder(SpecialFolders.Images),
              AbsolutePath2 = doc.GetSpecialFolder(SpecialFolders.Documents),
            }, doc, uri.GetPathString(), true);
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
