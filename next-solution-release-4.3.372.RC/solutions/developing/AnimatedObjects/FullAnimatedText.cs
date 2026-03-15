using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using ScreenSettings;
using StringManager.ComponentService;
using Utilities;
using UFInterfaces;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Threading;
using AnimatedObjects;
using UFInterfaces.PropertyControl;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WPFPenHelpers;
using DynamicTagAwareHelper;
using System.Windows.Media.Effects;
using Opc.Ua;
using WPFUtilities.Extensions;
using System.Xml.Serialization;
using TranslationHelpers;

namespace AnimatedObjects
{
    public class FullAnimatedText : AnimatedObject, IDisposable, IDynamicTagAware, IEntityReference, IStringIDAware
    {

        #region DP 

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }

        #region OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(FullAnimatedText));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(FullAnimatedText));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as FullAnimatedText;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }

        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && (DesignerProperties.GetIsInDesignMode(this) || bDesign))
                UpdateTextLayout();
        }
        #endregion

        #region DefaultText
        public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register("DefaultText", typeof(string), typeof(FullAnimatedText), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDefaultTextChanged), new CoerceValueCallback(OnCoerceDefaultText)));

        private static object OnCoerceDefaultText(DependencyObject o, object value)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                return control.OnCoerceDefaultText((string)value);
            else
                return value;
        }

        private static void OnDefaultTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                control.OnDefaultTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDefaultText(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDefaultTextChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit) && templateApplied)
                UpdateTextLayout();
        }

        private void UpdateText(string newValue)
        {
            if(baseText!= null)
            {
                string _newValue = newValue;
                if (string.IsNullOrEmpty(newValue))
                    _newValue = AnimatedObjects.Properties.Resources.AnimatedText;
                UpdateBaseTextDimension(_newValue);
                if(!bDesign)
                    StartAnimation(actualAnimation);
            }
            //if (backImage != null)
            //{
            //    backImage.Stretch = Stretch.None;
            //    BackImage = null;
            //}
        }

        private void UpdateBaseTextDimension(string newValue)
        {
            if (baseText != null && mainGrid != null)
            {
                baseText.Text = newValue;
                var controlSize = GetLayoutSize(this);
                baseText.Height = double.NaN;
                if (TextWrapping == TextWrapping.WrapWithOverflow || TextWrapping == TextWrapping.Wrap)
                {
                    baseText.Measure(new Size(this.Width, double.PositiveInfinity));
                    baseText.Arrange(new Rect(baseText.DesiredSize));
                    baseText.Width = controlSize.Width;
                    mainGrid.Width = controlSize.Width;
                    mainGrid.Height = Math.Max(controlSize.Height, baseText.ActualHeight);
                    //if (!ControlClipToBounds)
                        mainGrid.Height = double.NaN;

                    externalGrid.ClipToBounds = ControlClipToBounds;
                }
                else
                {
                    baseText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    if(baseText.DesiredSize.Width > controlSize.Width)
                        baseText.Arrange(new Rect(baseText.DesiredSize));
                    mainGrid.Width = Math.Max(controlSize.Width, baseText.ActualWidth);
                    mainGrid.Height = controlSize.Height;
                    if (!ControlClipToBounds)
                        mainGrid.Width = double.NaN;
                }
            }
        }
  
        Size GetLayoutSize(FrameworkElement fe)
        {
            double width;
            double height;
            if (!double.IsNaN(fe.Width) && !double.IsInfinity(fe.Width))
            {
                width = fe.Width;
            }
            else if ((fe.ActualWidth == 0.0) && (fe.DesiredSize.Width > 0.0))
            {
                width = fe.DesiredSize.Width;
            }
            else
            {
                width = fe.ActualWidth;
            }
            if (!double.IsNaN(fe.Height) && !double.IsInfinity(fe.Height))
            {
                height = fe.Height;
            }
            else if ((fe.ActualHeight == 0.0) && (fe.DesiredSize.Height > 0.0))
            {
                height = fe.DesiredSize.Height;
            }
            else
            {
                height = fe.ActualHeight;
            }
            return new Size(width, height);
        }

        [Category("AdvancedOptions")]
        public string DefaultText
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DefaultTextProperty);
            }
            set
            {
                SetValue(DefaultTextProperty, value);
            }
        }

        #endregion

        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(FullAnimatedText), new UIPropertyMetadata(TextWrapping.Wrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FullAnimatedText control = o as FullAnimatedText;
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
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(FullAnimatedText), new UIPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnTextAlignmentChanged), new CoerceValueCallback(OnCoerceTextAlignment)));

        private static object OnCoerceTextAlignment(DependencyObject o, object value)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                return control.OnCoerceTextAlignment((TextAlignment)value);
            else
                return value;
        }

        private static void OnTextAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FullAnimatedText control = o as FullAnimatedText;
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

        #region TextAnimation
        public static readonly DependencyProperty TextAnimationProperty = DependencyProperty.Register("TextAnimation", typeof(AnimationType), typeof(FullAnimatedText), new UIPropertyMetadata(AnimationType.None, new PropertyChangedCallback(OnTextAnimationChanged), new CoerceValueCallback(OnCoerceTextAnimation)));

        private static object OnCoerceTextAnimation(DependencyObject o, object value)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                return control.OnCoerceTextAnimation((AnimationType)value);
            else
                return value;
        }

        private static void OnTextAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FullAnimatedText control = o as FullAnimatedText;
            if (control != null)
                control.OnTextAnimationChanged((AnimationType)e.OldValue, (AnimationType)e.NewValue);
        }

        protected virtual AnimationType OnCoerceTextAnimation(AnimationType value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextAnimationChanged(AnimationType oldValue, AnimationType newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (!DesignerProperties.GetIsInDesignMode(this) && !bDesign)
            //    ManageAnimations(DefAnimation);
        }
        [Category("AdvancedOptions")]
        public AnimationType TextAnimation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (AnimationType)GetValue(TextAnimationProperty);
            }
            set
            {
                SetValue(TextAnimationProperty, value);
            }
        }

        #endregion

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new AnimatedObjects.Controls.SmartControl(this);
            }
        }

        #endregion

        #region Declarations
        bool bInit;
        IWorkspace workspace;
        IStringEditorManager stringeditorManager;
        List<string> matchChangedMap = new List<string>();
        IDictionary<String, String> stringlist;
        Dictionary<string, object> viewList = new Dictionary<string, object>();
        Dictionary<string, OPCUAEntityReference> opcuaEntityReference;
        Dictionary<string, PenItemHelper> mapHandlers;
        TypeHelper typeHelper = new TypeHelper();
        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        DispatcherOperation dpUpdateValue;
        Dictionary<string, object> queuedValues = new Dictionary<string, object>();
        protected override AnimationItem DefAnimation
        {
            get
            {
                if (defAnimation == null)
                {
                    defAnimation = new AnimationItem();
                    defAnimation.Value = 0.0;
                }

                defAnimation.Foreground = Foreground;
                defAnimation.Background = ControlBackground;
                defAnimation.Animation = TextAnimation;
                defAnimation.AnimationTime = AnimationTime;
                defAnimation.Text = DefaultText;
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
        #endregion

        #region ctor
        static FullAnimatedText()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(FullAnimatedText), new FrameworkPropertyMetadata(typeof(FullAnimatedText)));
        }
        public FullAnimatedText()
        {
            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bLDisposed)
                {
                    bLoaded = true;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                    {
                        if (stringeditorManager == null)
                            stringeditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringeditorManager != null)
                        {
                            OnCultureChanged(null, null);
                            stringeditorManager.CultureChanged += OnCultureChanged;
                        }
                    }

                    if (!DesignerProperties.GetIsInDesignMode(this))
                    {
                        BackImage = null;

                        if (mapHandlers == null)
                            mapHandlers = new Dictionary<string, PenItemHelper>();

                        AnimationList?.ToList().ForEach(data => { if (data.NodeId == null) data.NodeId = Guid.NewGuid().ToString(); });

                        try
                        {
                            if (OpcuaEntityReference.Count > 0)
                                foreach (var key in OpcuaEntityReference.Keys)
                                {
                                    if (!OpcuaEntityReference[key].IsRelative && !matchChangedMap.Contains(key))
                                        PrepareExecution(key);
                                }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    bInit = true;
                }

                if (!bLDisposed && !DesignerProperties.GetIsInDesignMode(this) && templateApplied)
                    UpdateTextLayout();
            };
        }
        #endregion

        #region  Methods

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.externalGrid = base.GetTemplateChild("externalGrid") as Grid;
            this.baseText = base.GetTemplateChild("baseText") as TextBlock;
            this.templateApplied = true;
            UpdateTextLayout();
        }
        static internal Brush defBaseTextColor = Brushes.White;
        void UpdateTextLayout()
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && monitoredItemViewModel != null)
            {
                if (textBinding == null)
                    UpdateMonitoredValue(monitoredItemViewModel);
                else if (Value != null)
                    base.UpdateAnimation(Value);
            }
            else if (baseText != null)
            {
                if (this.ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                    this.baseText.Foreground = defBaseTextColor;
                else
                    baseText.Foreground = Foreground;

                if (!string.IsNullOrEmpty(DefaultText))
                    UpdateText(GetText(DefaultText));
                else if (AnimationList != null && AnimationList.Count > 0)
                    UpdateText(GetText(AnimationList[0].Text));
                else
                    UpdateText(string.Empty);

                //baseText.Text = GetText(DefaultText);
                //BackImage = null;
            }
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bLDisposed)
                    return;

                bool bUntranslated = bDesign && stringeditorManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringeditorManager.GetListStringForCulture(Document, stringeditorManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                if (bDesign)
                {
                    UpdateTextLayout();
                }
                else
                {
                    if (!bInit)
                    {
                        InitAnimation();
                    }
                    else if (monitoredItemViewModel != null)
                    {
                        if (textBinding == null)
                            UpdateMonitoredValue(monitoredItemViewModel);
                        else
                            base.UpdateAnimation(Value);
                    }
                }
            });
        }

        private void InitAnimation()
        {
            if (baseText != null)
            {
                string newValue = null;
                if (actualAnimation != null)
                {
                    newValue = !string.IsNullOrEmpty(actualAnimation.Text) ? GetText(actualAnimation.Text) : GetText(DefaultText);
                    baseText.Foreground = actualAnimation.Foreground;
                }
                else
                {
                    newValue = !string.IsNullOrEmpty(DefaultText) ? GetText(DefaultText) : string.Empty;
                    if (this.ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                        this.baseText.Foreground = defBaseTextColor;
                    else
                        baseText.Foreground = Foreground;
                }

                UpdateBaseTextDimension(newValue);
                if (!bDesign)
                    StartAnimation(actualAnimation);
            }
        }

        private string GetText(string text)
        {
            return TranslationHelper.TranslateComposedText(text, stringlist, text);
        }

        protected override void ManageAnimations(AnimationItem animation, string newValue)
        {
            if (bDatacontextChanging || bLDisposed)
                return;

            var text = string.Empty;
            if (TagType == FormatEnum.String)
            {
                if (newValue?.IndexOf(':') >= 0)
                    text = GetText(newValue.Substring(newValue.IndexOf(':') + 1));
                else
                    text = GetText(newValue);
            }
            else if (animation.TagReference != null && viewList.ContainsKey(animation.NodeId))
            {
                var value = viewList[animation.NodeId] as DataValue;
                text = GetText(value?.Value?.ToString());
            }
            else
                text = !string.IsNullOrEmpty(animation.Text) ? GetText(animation.Text) : GetText(DefaultText);

            if (baseText != null && !baseText.Text.Equals(text))
            {
                baseText.Text = text;
                UpdateBaseTextDimension(text);
            }

            if (animation == actualAnimation)
                return;

            StopAnimations();

            base.ManageAnimations(animation, newValue);

            if (baseText != null)
            {
                baseText.Foreground = animation.Foreground;
                StartAnimation(animation);
            }
        }

        private void StartAnimation(AnimationItem animation)
        {
            if (animation == null)
                return;
            if (animation.AnimationTime > 0)
            {
                switch (animation.Animation)
                {
                    case AnimationType.None:
                        break;
                    case AnimationType.LeftToRight:
                        LeftToRightMarquee(animation);
                        break;
                    case AnimationType.RightToLeft:
                        RightToLeftMarquee(animation);
                        break;
                    case AnimationType.TopToBottom:
                        TopToBottomMarquee(animation);
                        break;
                    case AnimationType.BottomToTop:
                        BottomToTopMarquee(animation);
                        break;
                    case AnimationType.RotateLeft:
                        LeftToRightRotation(animation);
                        break;
                    case AnimationType.RotateRight:
                        RightToLeftRotation(animation);
                        break;
                    case AnimationType.Blink:
                        Bilnk(animation);
                        break;
                    case AnimationType.ScaleX:
                        Scale(animation, 0);
                        break;
                    case AnimationType.ScaleY:
                        Scale(animation, 1);
                        break;
                    case AnimationType.ScaleXY:
                        Scale(animation, 2);
                        break;
                    case AnimationType.SkewX:
                        Skew(animation, 0);
                        break;
                    case AnimationType.SkewY:
                        Skew(animation, 1);
                        break;
                    case AnimationType.SkewXY:
                        Skew(animation, 2);
                        break;
                    default:
                        break;
                }
            }

        }

        List<string> storyboardNames = new List<string> { "scaleX", "scaleY", "skewX", "skewY", "blink", "translationX", "translationY", "rotation" };
        private void StopAnimations()
        {
            if (baseText == null)
                return;
            storyboardNames.ForEach(x =>
            {
                try
                {
                    Storyboard story = baseText.FindResource(x) as Storyboard;
                    if (story != null)
                    {
                        story.Stop();
                    }
                }
                catch (Exception)
                {
                }
            });
        }
        private void Bilnk(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("blink") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));
                story.Begin();
            }
        }
        private void LeftToRightRotation(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("rotation") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = 0;
                doubleAnimation.To = 360;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void RightToLeftRotation(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("rotation") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = 360;
                doubleAnimation.To = 0;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void LeftToRightMarquee(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("translationX") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = -baseText.ActualWidth;
                doubleAnimation.To = this.ActualWidth;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void RightToLeftMarquee(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("translationX") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = this.ActualWidth;
                doubleAnimation.To = -baseText.ActualWidth;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void TopToBottomMarquee(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("translationY") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = -baseText.ActualHeight;
                doubleAnimation.To = this.ActualHeight;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void BottomToTopMarquee(AnimationItem animation)
        {
            if (baseText == null)
                return;
            Storyboard story = baseText.FindResource("translationY") as Storyboard;
            if (story != null)
            {
                DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                doubleAnimation.From = this.ActualHeight;
                doubleAnimation.To = -baseText.ActualHeight;
                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                story.Begin();
            }
        }
        private void Skew(AnimationItem animation, int type)
        {
            if (baseText == null)
                return;
            switch (type)
            {
                case 0:
                    Storyboard story = baseText.FindResource("skewX") as Storyboard;
                    if (story != null)
                    {
                        DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        story.Begin();
                    }
                    break;
                case 1:
                    Storyboard ystory = baseText.FindResource("skewY") as Storyboard;
                    if (ystory != null)
                    {
                        DoubleAnimation doubleAnimation = ystory.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        ystory.Begin();
                    }
                    break;
                case 2:
                    Storyboard _story = baseText.FindResource("skewX") as Storyboard;
                    if (_story != null)
                    {
                        DoubleAnimation doubleAnimation = _story.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        _story.Begin();
                    }
                    Storyboard _ystory = baseText.FindResource("skewY") as Storyboard;
                    if (_ystory != null)
                    {
                        DoubleAnimation doubleAnimation = _ystory.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        _ystory.Begin();
                    }
                    break;
                default:
                    break;
            }
        }
        private void Scale(AnimationItem animation, int type)
        {
            if (baseText == null)
                return;
            switch (type)
            {
                case 0:
                    Storyboard story = baseText.FindResource("scaleX") as Storyboard;
                    if (story != null)
                    {
                        DoubleAnimation doubleAnimation = story.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        story.Begin();
                    }
                    break;
                case 1:
                    Storyboard ystory = baseText.FindResource("scaleY") as Storyboard;
                    if (ystory != null)
                    {
                        DoubleAnimation doubleAnimation = ystory.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        ystory.Begin();
                    }
                    break;
                case 2:
                    Storyboard _story = baseText.FindResource("scaleX") as Storyboard;
                    if (_story != null)
                    {
                        DoubleAnimation doubleAnimation = _story.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        _story.Begin();
                    }
                    Storyboard _ystory = baseText.FindResource("scaleY") as Storyboard;
                    if (_ystory != null)
                    {
                        DoubleAnimation doubleAnimation = _ystory.Children[0] as DoubleAnimation;
                        doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animation.AnimationTime));
                        _ystory.Begin();
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region IDIsposable
        bool bLDisposed;
        public override void Dispose()
        {
            if (bLDisposed)
                return;
            bLDisposed = true;

            if (stringeditorManager != null)
                stringeditorManager.CultureChanged -= OnCultureChanged;
            
            lock (queuedValues)
            {
                if (dpUpdateValue != null &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                    dpUpdateValue.Abort();
                queuedValues.Clear();
            }

            TerminateExecution();

            viewList.Clear();

            if (opcuaEntityReference != null)
                opcuaEntityReference.Clear();

            if (mapHandlers != null)
                mapHandlers.Clear();

            typeHelper.Dispose();

            base.Dispose();

            DetachOverrideBaseProperties();
        }
        #endregion

        #region Override
        protected override IDictionary<DependencyProperty, DataTemplate> GetDataTemplates()
        {
            var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

            if (workspace == null)
            {
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
            }
            if (workspace != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                mapDataTemplates.Add(DefaultTextProperty, dt);
            }

            return mapDataTemplates;
        }
        #endregion

        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(DefaultText))
                list.Add(DefaultText);
            if (AnimationList != null)
            {
                var _list = AnimationList.Where(x => !string.IsNullOrEmpty(x.Text)).Select(x => x.Text);
                if (_list != null && _list.Count() > 0)
                    list.AddRange(_list);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(DefaultText))
            {
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (Document, typeof(FullAnimatedText), DefaultTextProperty).DisplayName;
                map.Add(propertyName, DefaultText);
            }
            int i = 1;
            AnimationList?.Where(x => !string.IsNullOrEmpty(x.Text)).ToList().ForEach(x =>
            {
                map.Add(string.Format(Properties.Resources.ThresholdTextProperty, i), x.Text);
                i++;
            });
            return map;
        }
        #endregion

        #region IDynamicTagAware
        private Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                //if (opcuaEntityReference == null)
                //{
                //    opcuaEntityReference = new Dictionary<int, OPCUAEntityReference>();
                //    foreach (PenItem item in PenReferenceList)
                //    {
                //        opcuaEntityReference[PenReferenceList.IndexOf(item)] = item.TagReference;
                //    }
                //}
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    if (AnimationList != null)
                    {
                        for (int i = 0; i < AnimationList.Count(); i++)
                        {
                            opcuaEntityReference[AnimationList[i].NodeId] = AnimationList[i].TagReference;
                        }
                    }
                }

                return opcuaEntityReference;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (AnimationList != null)
                AnimationList.OrderBy(a => a.Value).ToList().ForEach(a =>
                {
                    if (a.TagReference != null /*&& pen.XTagReference.IsValid*/)
                        ret.Add(string.Format(a.CreateUniqueName(ret.Keys.ToList())), a.TagReferenceXml);
                });
            return ret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;
            bool ret = false;
            bool cret = false;

            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();

            AnimationItemList animationList = new AnimationItemList(AnimationList);
            var animationTagList = (from pen in animationList where pen.TagReference != null select pen).ToList();
            int mapCount = animationTagList.Count;

            foreach (var pen in animationTagList)
            {
                //if (pen.TagReference != null /*&& pen.TagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesign || DesignerProperties.GetIsInDesignMode(this))
                            ret = relative == pen.TagReferenceXml;
                        else if (_absolute != null)
                        {
                            if (relative == pen.TagReferenceXml)
                            {
                                matchChangedMap.Add(pen.NodeId);
                                string key = pen.NodeId;
                                TerminateExecution(key);
                                if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.TagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                }
                                else
                                {
                                    pen.TagReference = _absolute;
                                    opcuaEntityReference[key] = _absolute;
                                }

                                PrepareExecution(key);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (!bDesign)
                ret = matchChangedMap.Count == mapCount;

            return ret || cret;
        }

        private void PrepareExecution(string key)
        {
            if (bLDisposed || !OpcuaEntityReference.ContainsKey(key))
                return;
            try
            {
                if (mapHandlers == null)
                    mapHandlers = new Dictionary<string, PenItemHelper>();

                if (!OpcuaEntityReference[key].IsValid)
                    return;

                if (mapHandlers.ContainsKey(key))
                    TerminateExecution(key);

                mapHandlers[key] = new PenItemHelper(key);
                mapHandlers[key].Error += PenItem_OnError;
                mapHandlers[key].ValueChanged += PenItem_ValueChanged;

                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document as ScreenDocument, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
            }
            catch (Exception)
            {
            }
        }

        private void TerminateExecution(string key)
        {
            //*************
            //set not in use
            //*************
            if (OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key] != null)
            {
                if (mapHandlers == null)
                    mapHandlers = new Dictionary<string, PenItemHelper>();
                if (mapHandlers.ContainsKey(key))
                {
                    typeHelper.TerminateExecution(this, mapHandlers[key].opcuaEntityReference_PropertyChanged, mapHandlers[key].monitoredItemViewModel_PropertyChanged, OpcuaEntityReference[key], OpcuaEntityReference[key].MonitoredItemViewModel);
                    mapHandlers[key].Error -= PenItem_OnError;
                    mapHandlers[key].ValueChanged -= PenItem_ValueChanged;
                    mapHandlers[key].Dispose();
                    mapHandlers.Remove(key);
                }
            }
        }

        private void TerminateExecution()
        {
            if (AnimationList == null || AnimationList.Count == 0)
                return;

            //*************
            //set not in use
            //*************
            foreach (var key in OpcuaEntityReference.Keys)
            {
                TerminateExecution(key);
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            AnimationItemList penList = new AnimationItemList(AnimationList);
            foreach (var pen in penList)
            {
                if (pen.TagReference != null /*&& pen.TagReference.IsValid*/)
                {
                    if (map.ContainsKey(pen.NodeId))
                        pen.TagReferenceXml = map[pen.NodeId];
                    else
                        pen.TagReferenceXml = typeHelper.UpdateTag(pen.TagReferenceXml, map);
                }
            }
            AnimationList = penList;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreserveTagsFromMap(Dictionary<string, string> map)
        {
            var tobeupdated = typeHelper.PreserveTagsFromMap(GetMapDynamics(), map);
            UpdateMapDynamics(tobeupdated);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetConverterLabel(string label)
        {
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
        {
        }
        #endregion

        #region PenItemHelper Event Handlers
        private void PenItem_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            SetEntityError(e.ErrorMessage);
        }

        private void PenItem_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;
            UpdateMonitoredValue(helper.Key, e.NewValue);
        }

        void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bLDisposed || externalGrid == null)
                    return;

                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        (externalGrid as UIElement).Effect = previousEffect;
                        (externalGrid as UIElement).ClipToBounds = previousClipToBounds;
                        //(this as UIElement).Opacity = 1;
                        previousEffect = null;
                        errorEffectOn = false;
                    }
                }
                else
                {
                    if (!errorEffectOn)
                    {
                        errorEffectOn = true;
                        previousEffect = (externalGrid as UIElement).Effect;
                        previousClipToBounds = (externalGrid as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (externalGrid as UIElement).Effect = effect;
                        (externalGrid as UIElement).ClipToBounds = false;
                    }
                }
            });
        }

        void UpdateMonitoredValue(string key, object newValue)
        {
            bool bExecute = false;
            lock (queuedValues)
            {
                bExecute = queuedValues.Count() == 0;
                queuedValues[key] = newValue;

                if (bExecute)
                {
                    dpUpdateValue = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (bLDisposed)
                            return;

                        Dictionary<string, object> tmpqueued;
                        lock (queuedValues)
                        {
                            tmpqueued = new Dictionary<string, object>(queuedValues);
                            queuedValues.Clear();
                        }

                        foreach (var k in tmpqueued.Keys)
                        {
                            if (tmpqueued[k] != null)
                                viewList[k] = tmpqueued[k];
                        }

                        if (monitoredItemViewModel != null)
                        {
                            if (textBinding == null)
                                UpdateMonitoredValue(monitoredItemViewModel);
                            else
                                base.UpdateAnimation(Value);
                        }
                    });
                }
            }
        }
        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            return new Dictionary<string, Brush>();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is AnimatedText)
            {
                AnimatedText control = sender as AnimatedText;
                if (control.ReadLocalValue(AnimatedText.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", FullAnimatedText.defBaseTextColor);
            }
            else if (sender is FullAnimatedText)
            {
                FullAnimatedText control = sender as FullAnimatedText;
                if (control.ReadLocalValue(FullAnimatedText.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", FullAnimatedText.defBaseTextColor);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            return value;
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
