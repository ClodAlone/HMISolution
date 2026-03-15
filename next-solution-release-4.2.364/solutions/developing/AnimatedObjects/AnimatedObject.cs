using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows;
using OPCUAViewModel;
using DocumentManager.ComponentService;
using System.Windows.Threading;
using System.ComponentModel;
using Utilities;
using System.Windows.Media.Animation;
using ScreenSettings;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Opc.Ua;
using UFInterfaces;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using System.Xml.Serialization;
using AnimatedObjects.Controls;

namespace AnimatedObjects
{
    public class AnimatedObject : Control, IDisposable, IContainPropertyEditors
    {
        #region DP
        #region OverrideBaseProperties
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(AnimatedObject));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(AnimatedObject));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as AnimatedObject;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bLoaded && (DesignerProperties.GetIsInDesignMode(this) || bDesign))
            if(!bOverride && bLoaded && !IsManipulationEnabled)
                ControlBackground = Background;
        }

        #endregion

        #region ControlBackground
        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(AnimatedObject), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnControlBackgroundChanged), new CoerceValueCallback(OnCoerceControlBackground)));

        private static object OnCoerceControlBackground(DependencyObject o, object value)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                return control.OnCoerceControlBackground((Brush)value);
            else
                return value;
        }

        private static void OnControlBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                control.OnControlBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceControlBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool bOverride;
        protected virtual void OnControlBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bLoaded)
            {
                UpdateMainBorderBackground(newValue);
            }
        }

        private void UpdateMainBorderBackground(Brush background)
        {
            bOverride = true;
            Background = background;
            bOverride = false;

            if (mainBorder != null)
            {
                if(!(background is VisualBrush) && !(background is ImageBrush))
                {
                    BaseImage = null;
                    mainBorder.Background = background;
                }
                else
                {
                    try
                    {
                        if (Document == null)
                            Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                        BaseImage = WPFUtilities.ImageHelper.BrushToImageUri(Document, background);
                        mainBorder.Background = null;
                        UpdateBackImage();
                    }
                    catch
                    {
                        BaseImage = null;
                        mainBorder.Background = background;
                    }
                }
            }
        }
        
        [Obsolete("Use the Background property insted of this.")]
        [Browsable(false)]
        public Brush ControlBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ControlBackgroundProperty);
            }
            set
            {
                SetValue(ControlBackgroundProperty, value);
            }
        }

        #endregion


        #region AnimationList
        public static readonly DependencyProperty AnimationListProperty = DependencyProperty.Register("AnimationList", typeof(AnimationItemList), typeof(AnimatedObject), new UIPropertyMetadata(null));
        [Category("Advanced")]
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertAnimationItemList), RequiredKey = true)]
        public AnimationItemList AnimationList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (AnimationItemList)GetValue(AnimationListProperty);
            }
            set
            {
                SetValue(AnimationListProperty, value);
            }
        }

        #endregion

        #region ControlClipToBounds
        public static readonly DependencyProperty ControlClipToBoundsProperty = DependencyProperty.Register("ControlClipToBounds", typeof(bool), typeof(AnimatedObject), new UIPropertyMetadata(true));
        [Category("AdvancedOptions")]
        public bool ControlClipToBounds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ControlClipToBoundsProperty);
            }
            set
            {
                SetValue(ControlClipToBoundsProperty, value);
            }
        }

        #endregion

        #region AnimationTime
        public static readonly DependencyProperty AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(double), typeof(AnimatedObject), new UIPropertyMetadata(1000.0));
        [Category("AdvancedOptions")]
        public double AnimationTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(AnimationTimeProperty);
            }
            set
            {
                SetValue(AnimationTimeProperty, value);
            }
        }

        #endregion

        #region ControlRenderTransformOrigin
        public static readonly DependencyProperty ControlRenderTransformOriginProperty = DependencyProperty.Register("ControlRenderTransformOrigin", typeof(Point), typeof(AnimatedObject), new UIPropertyMetadata(new Point(0.0, 0.0)));
        [Category("AdvancedOptions")]
        public Point ControlRenderTransformOrigin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Point)GetValue(ControlRenderTransformOriginProperty);
            }
            set
            {
                SetValue(ControlRenderTransformOriginProperty, value);
            }
        }

        #endregion

        #region BackImage
        public static readonly DependencyProperty BackImageProperty = DependencyProperty.Register("BackImage", typeof(Uri), typeof(AnimatedObject), new UIPropertyMetadata(null));
        [Browsable(false)]
        [XmlIgnore]
        public Uri BackImage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(BackImageProperty);
            }
            set
            {
                SetValue(BackImageProperty, value);
            }
        }

        #endregion


        #region BaseImage
        public static readonly DependencyProperty BaseImageProperty = DependencyProperty.Register("BaseImage", typeof(Uri), typeof(AnimatedObject), new UIPropertyMetadata(null));
        [Browsable(false)]
        [XmlIgnore]
        public Uri BaseImage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(BaseImageProperty);
            }
            set
            {
                SetValue(BaseImageProperty, value);
            }
        }

        #endregion

        #region Stretch
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(AnimatedObject), new UIPropertyMetadata(Stretch.None, new PropertyChangedCallback(OnStretchChanged), new CoerceValueCallback(OnCoerceStretch)));

        private static object OnCoerceStretch(DependencyObject o, object value)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                return control.OnCoerceStretch((Stretch)value);
            else
                return value;
        }

        private static void OnStretchChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                control.OnStretchChanged((Stretch)e.OldValue, (Stretch)e.NewValue);
        }

        protected virtual Stretch OnCoerceStretch(Stretch value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStretchChanged(Stretch oldValue, Stretch newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bDisposed && (DesignerProperties.GetIsInDesignMode(this) || bDesign))
                this.UpdateBackImage();
        }

        public Stretch Stretch
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }

        #endregion


        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(AnimatedObject), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                return control.OnCoerceValue((string)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AnimatedObject control = o as AnimatedObject;
            if (control != null)
                control.OnValueChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceValue(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                if (!bDesign)
                {
                    TagType = TryGetType();
                    if (monitoredItemViewModel != null)
                    {
                        if (textBinding == null)
                            UpdateMonitoredValue(monitoredItemViewModel);
                        else
                            UpdateAnimation(newValue);
                    }
                }
            }
            catch
            {
            }
        }

        public string Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(AnimatedObject), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SmartProperties
        {
            get
            {
                return (bool)GetValue(SmartPropertiesProperty);
            }
        }

        #endregion
        #region ctor
        public AnimatedObject()
        {
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    base.HorizontalContentAlignment = this.ReadLocalValue(HorizontalContentAlignmentProperty) == DependencyProperty.UnsetValue ? HorizontalAlignment.Stretch : HorizontalContentAlignment;
                    base.VerticalContentAlignment = this.ReadLocalValue(VerticalContentAlignmentProperty) == DependencyProperty.UnsetValue ? VerticalAlignment.Stretch : VerticalContentAlignment;
                    base.FontSize = this.ReadLocalValue(FontSizeProperty) == DependencyProperty.UnsetValue ? 20 : FontSize;
                    base.FontFamily = this.ReadLocalValue(FontFamilyProperty) == DependencyProperty.UnsetValue ? new FontFamily("Segoe UI") : FontFamily;
                    base.FontStyle = this.ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue ? FontStyles.Normal : FontStyle;
                    base.FontWeight = this.ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue ? FontWeights.SemiBold : FontWeight;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                        this.UpdateBackImage();
                    }

                    if (templateApplied)
                        outerBorder.Visibility = bDesign ? Visibility.Visible : Visibility.Collapsed;
                }
            };
            DataContextChanged += (o, e) =>
            {
                if (bDisposed)
                    return;

                if (DataContext is MonitoredItemViewModel && !bDesign && !DesignerProperties.GetIsInDesignMode(this))
                {
                    bDatacontextChanging = true;

                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;

                    textBinding = BindingOperations.GetBindingExpression(this, AnimatedObject.ValueProperty);

                    TagType = TryGetType();
                    if (textBinding == null)
                        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                    
                    UpdateMonitoredValue(monitoredItemViewModel);

                    bDatacontextChanging = false;

                }
            };
        }

        protected void UpdateMonitoredValue(MonitoredItemViewModel monitoreditem)
        {
            var dataValue = monitoreditem?.DataValue;
            if (dataValue != null && dataValue.Value != null)
            {
                if (!bDataTypeFetched && Opc.Ua.StatusCode.IsGood(dataValue.StatusCode))
                {
                    bDataTypeFetched = true;
                    TagType = TryGetType();
                }
                if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) || dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                {
                    string newValue = null;
                    if (dataValue.Value is Array)
                    {
                        var dataValueCollection = monitoreditem.DataValueCollectionDouble;
                        if (dataValueCollection != null && dataValueCollection.Count > 0)
                            newValue = dataValueCollection[0].Value?.ToString();
                    }
                    else
                        newValue = dataValue.Value.ToString();

                    UpdateAnimation(newValue);
                }
            }
        }

        bool bDataTypeFetched;
        DispatcherOperation dp;
        object lockObject = new object();
        string lastValue;
        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
                if (textBinding == null)
                {
                    UpdateMonitoredValue(sender as MonitoredItemViewModel);
                }
                else if (monitoredItemViewModel != null)
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
            }
        }
        #endregion
        #region Declarations
        internal BindingExpression textBinding;
        [Browsable(false)]
        protected bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        [Browsable(false)]
        protected bool NotRunningOnServer
        {
            get
            {
                return !RunningOnServer;
            }
        }


        protected IDocument Document;
        protected DispatcherTimer backImageTimer;
        internal AnimationItem actualAnimation;
        protected AnimationItem defAnimation;
        protected virtual AnimationItem DefAnimation
        {
            get
            {
                if (defAnimation == null)
                {
                    defAnimation = new AnimationItem();
                    defAnimation.Value = 0.0;
                }

                defAnimation.Background = ControlBackground;
                defAnimation.AnimationTime = AnimationTime;
                defAnimation.BackImage = null;
                defAnimation.ImageStretch = Stretch.None;
                defAnimation.BackImageList.Clear();
                return defAnimation;
            }
            set
            {
                if (defAnimation != value)
                    defAnimation = value;
            }
        }
        protected MonitoredItemViewModel monitoredItemViewModel;
        protected int EnumStringCount;
        protected FormatEnum _tagType = FormatEnum.String;
        protected FormatEnum TagType
        {
            get
            {
                return _tagType;
            }
            set
            {
                if (_tagType != value)
                {
                    _tagType = value;
                }
            }
        }
        protected int iCount;
        protected bool bLoaded;
        protected bool bDesign;
        protected bool bDatacontextChanging;
        protected string specialForder;
        protected string documentFolder;
        protected Grid mainGrid;
        protected Grid externalGrid;
        protected Image backImage;
        protected TextBlock baseText;
        protected Border outerBorder;
        protected bool templateApplied;
        protected Border mainBorder;
        #endregion

        #region Method
        internal void MakeRelativeImages(IDocument document)
        {
            if (document == null)
                return;

            var parent = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(document, true);
            AnimationList?.ToList().ForEach(a =>
            {
                if (a.BackImage != null && !a.BackImage.IsAbsoluteUri)
                    a.BackImage = GetHMIPath(a.BackImage, parent);
                a.BackImageList.ToList().ForEach(al =>
                {
                    al.Value = GetHMIPath(al.Value, parent);
                });
            });
        }

        Uri GetHMIPath(Uri source, IDocument document)
        {
            if (source == null)
                return null;
            if (source.IsAbsoluteUri)
                return new Uri(source.GetPathString().Replace("/","\\"));

            source = new Uri(source.GetPathString().Replace("/", "\\"), UriKind.RelativeOrAbsolute);
            string images = $"{SpecialFolders.Images.ToString().First<char>().ToString().ToLower()}{SpecialFolders.Images.ToString().Substring(1)}";
            var title = System.IO.Path.GetFileNameWithoutExtension(document.FilePath);
            if (XpoHelpers.XpoHelper.IsDataSource(document.FilePath))
                title = XpoHelpers.XpoHelper.GetDataSourceTitle(document.FilePath, onlytitle: true);
            string prefix = $"{images}\\{title}";
            if (source.GetPathString().StartsWith(SpecialFolders.Images.ToString()))
                return new Uri($"{prefix}\\{System.IO.Path.GetFileName(source.GetPathString())}", UriKind.RelativeOrAbsolute);
            else
            {
                string imagefolder = document.GetSpecialFolder(SpecialFolders.Images).GetPathString();
                if (source.GetPathString().StartsWith(imagefolder))
                    return new Uri($"{prefix}\\{source.GetPathString().Replace(imagefolder, "")}", UriKind.RelativeOrAbsolute);
                else
                    return new Uri($"{prefix}\\{source.GetPathString()}", UriKind.RelativeOrAbsolute);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainGrid = base.GetTemplateChild("mainGrid") as Grid;
            this.backImage = base.GetTemplateChild("backImage") as Image;
            this.outerBorder = base.GetTemplateChild("outerBorder") as Border;
            this.mainBorder = base.GetTemplateChild("mainBorder") as Border;
            UpdateMainBorderBackground(ControlBackground);
        }

        internal void UpdateAnimation(string newValue)
        {
            bool bForceDispatcherOperation = false;
            lock (lockObject)
            {
                bForceDispatcherOperation = lastValue == null;
                lastValue = newValue;
            }

            if (dp == null || bForceDispatcherOperation ||
            dp.Status == DispatcherOperationStatus.Completed ||
            dp.Status == DispatcherOperationStatus.Aborted)
            {
                dp = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;
                    string value;
                    lock (lockObject)
                    {
                        value = lastValue;
                        lastValue = null;
                    }
                    try
                    {
                        if (AnimationList != null && AnimationList.Count > 0)
                        {
                            string _sval = value;
                            CultureInfo culture = CultureInfo.CurrentCulture;

                            if (value.IndexOf(':') >= 0)
                            {
                                try
                                {
                                    culture = new CultureInfo(value.Substring(0, value.IndexOf(':')));
                                    _sval = value.Substring(value.IndexOf(':') + 1);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            double _Value = 0.0;
                            AnimationItem animation = null;
                            if (!string.IsNullOrEmpty(_sval))
                            {
                                if (String.Compare(_sval, "True", true) == 0 || String.Compare(_sval as String, "False", true) == 0)
                                    TagType = FormatEnum.Boolean;

                                switch (TagType)
                                {
                                    case FormatEnum.Digital:
                                    case FormatEnum.Boolean:
                                        if (String.Compare(_sval, "True", true) == 0)
                                            _Value = 1.0;
                                        else if (String.Compare(_sval, "False", true) == 0)
                                            _Value = 0.0;
                                        else
                                            _Value = System.Convert.ToDouble(_sval, culture);
                                        break;
                                    case FormatEnum.String:
                                        _Value = AnimationList.OrderBy(x => x.Value).Select(x => x.Value).FirstOrDefault();
                                        break;
                                    default:
                                        _Value = System.Convert.ToDouble(_sval, culture);
                                        break;
                                }
                                animation = AnimationList.OrderBy(x => x.Value).Where(x => x.Value <= _Value).Select(x => x).DefaultIfEmpty(DefAnimation).LastOrDefault();
                                ManageAnimations(animation, value);
                            }
                            else
                                ManageAnimations(DefAnimation, value);
                        }
                        else
                            ManageAnimations(DefAnimation, value);
                    }
                    catch (Exception)
                    {
                        ManageAnimations(DefAnimation, value);
                    }

                });
            }
        }
        protected virtual internal void UpdateBackImage()
        {
            if (AnimationList != null && AnimationList.Count > 0)
                TryBackImageUpdate(AnimationList[0], false);
            else
                SetDefaultImage();
        }

        private void SetDefaultImage()
        {
            BackImage = null;
        }

        protected virtual IDictionary<DependencyProperty, DataTemplate> GetDataTemplates()
        {
            return new Dictionary<DependencyProperty, DataTemplate>();
        }

        protected virtual void ManageAnimations(AnimationItem animation, string newValue)
        {
            if (bDatacontextChanging || bDisposed || newValue == null)
                return;

            actualAnimation = animation;

            if (backImageTimer != null)
            {
                backImageTimer.Stop();
                backImageTimer.Tick -= backImageTimer_Tick;
            }

            if (animation.BackImageList.Count > 0)
            {
                iCount = 0;
                backImageTimer_Tick(null, null);

                if (animation.AnimationTime > 0)
                {
                    if (backImageTimer == null)
                        backImageTimer = new DispatcherTimer();

                    backImageTimer.Tick += backImageTimer_Tick;
                    backImageTimer.Interval = TimeSpan.FromMilliseconds(animation.AnimationTime);
                    backImageTimer.Start();
                }
            }
            else
            {
                BackImage = null;
                if (backImage != null)
                    backImage.Stretch = Stretch.None;
            }
        }
        public FormatEnum TryGetType()
        {
            FormatEnum TagType = FormatEnum.String;
            try
            {
                EnumStringCount = 0;
                if (monitoredItemViewModel == null)
                {
                    TagType = FormatEnum.String;
                }
                else if (monitoredItemViewModel.EnumStrings != null && monitoredItemViewModel.EnumStrings.Length > 0)
                {
                    TagType = FormatEnum.Enumerated;
                    EnumStringCount = monitoredItemViewModel.EnumStrings.Length;
                }
                else if (monitoredItemViewModel.NodeIdModel != null && monitoredItemViewModel.NodeIdModel.TrueState != null && monitoredItemViewModel.NodeIdModel.FalseState != null)
                {
                    TagType = FormatEnum.Digital;
                }
                else if (monitoredItemViewModel.DataType != null)
                {
                    if (monitoredItemViewModel.DataType == "String")
                    {
                        TagType = FormatEnum.String;
                    }
                    else if (monitoredItemViewModel.DataType == "Boolean")
                    {
                        TagType = FormatEnum.Boolean;
                    }
                    else
                    {
                        TagType = FormatEnum.Numeric;
                    }
                }
                else if (monitoredItemViewModel.DataValueCollection != null)
                {
                    var _value = monitoredItemViewModel.DataValueCollection[0].Value;
                    if (_value is Byte ||
                         _value is SByte ||
                         _value is Int16 ||
                         _value is UInt16 ||
                         _value is Int32 ||
                         _value is UInt32 ||
                         _value is Int64 ||
                         _value is UInt64 ||
                         _value is float ||
                         _value is double ||
                         _value is Double)
                        TagType = FormatEnum.Numeric;
                    else if (_value is Boolean)
                        TagType = FormatEnum.Boolean;
                    else if (_value is String)
                        TagType = FormatEnum.String;
                    else
                        TagType = FormatEnum.String;
                }
                else if (monitoredItemViewModel.DataValue != null)
                {
                    var _value = monitoredItemViewModel.DataValue.Value;
                    if (_value is Byte ||
                         _value is SByte ||
                         _value is Int16 ||
                         _value is UInt16 ||
                         _value is Int32 ||
                         _value is UInt32 ||
                         _value is Int64 ||
                         _value is UInt64 ||
                         _value is float ||
                         _value is double ||
                         _value is Double)
                        TagType = FormatEnum.Numeric;
                    else if (_value is Boolean)
                        TagType = FormatEnum.Boolean;
                    else if (_value is String)
                        TagType = FormatEnum.String;
                    else
                        TagType = FormatEnum.String;

                }
                else
                {
                    TagType = FormatEnum.String;
                }
            }
            catch
            {
                TagType = FormatEnum.String;
            }

            return TagType;
        }
        private void backImageTimer_Tick(object sender, EventArgs e)
        {
            TryBackImageUpdate(actualAnimation);
        }

        private void TryBackImageUpdate(AnimationItem animation, bool useCount = true)
        {
            if (animation != null)
            {
                if (animation.BackImageList != null && animation.BackImageList.Count > 0)
                {
                    if (useCount)
                    {
                        if (iCount >= animation.BackImageList.Count)
                            iCount = 0;

                        BackImageUpdate(animation.BackImageList[iCount], animation.BackImageList[0], iCount, animation.ImageStretch);
                        iCount++;
                    }
                    else
                        BackImageUpdate(animation.BackImageList[0], animation.BackImageList[0], 0, AnimationList[0].ImageStretch);
                }
                else
                    SetDefaultImage();
            }
            else
                SetDefaultImage();
        }

        private void BackImageUpdate(BackImage backImageWanted, BackImage fallbackBackImage, int iCount, Stretch stretchWanted)
        {
            bool backIsValid = BackImageIsValid(backImageWanted);
            bool fallBackIsValid = BackImageIsValid(fallbackBackImage);
            if (Document == null || (!backIsValid && !fallBackIsValid))
            {
                SetDefaultImage();
                return;
            }
            Uri source = null;
            if (backIsValid)
                source = WPFUtilities.ImageHelper.GetRelativeUriSource(Document, backImageWanted.Value);

            if (!File.Exists(source.GetPathString()) && fallBackIsValid)
                source = WPFUtilities.ImageHelper.GetRelativeUriSource(Document, fallbackBackImage.Value);

            if (!File.Exists(source.GetPathString()))
            {
                SetDefaultImage();
                return;
            }

            BackImage = source;
            if (backImage != null)
                backImage.Stretch = stretchWanted;
        }
        bool BackImageIsValid(BackImage backImage)
        {
            return backImage != null && backImage.Value != null;
        }
        #endregion

        #region IDIsposable
        bool bDisposed;
        public virtual void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (dp != null &&
                dp.Status != DispatcherOperationStatus.Aborted &&
                dp.Status != DispatcherOperationStatus.Completed)
                dp.Abort();

            if (backImageTimer != null)
            {
                backImageTimer.Stop();
                backImageTimer.Tick -= backImageTimer_Tick;
                backImageTimer = null;
            }
            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            DetachOverrideBaseProperties();
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
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                var dt1 = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(SmartPropertiesEditor));
                factory.SetValue(SmartPropertiesEditor.DocumentProperty, Document);
                dt1.DataType = typeof(bool);
                dt1.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt1);

                var dataTemplates = GetDataTemplates();
                foreach(var dt in dataTemplates)
                {
                    if (!mapDataTemplates.ContainsKey(dt.Key))
                        mapDataTemplates.Add(dt.Key, dt.Value);
                }

                return mapDataTemplates;
            }
        }

        #endregion
    }
}
