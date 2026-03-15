#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Runtime.InteropServices;

#if WPF

using Syncfusion.Licensing;

#endif

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
#if WPF
    /// <summary>
    /// 
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
     Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
 Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
 Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    /// <summary>
    ///
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Blend;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Default;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Windows7;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.VS2010;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Metro;component/Editors/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(MaskedTextBox), XamlResource = "/Syncfusion.Theming.Transparent;component/Editors/MaskedTextBox.xaml")]
#endif
    public class MaskedTextBox : TextBox
    {
        #region Events

        /// <summary>
        /// Event than is raised after the <see cref="Mask"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback MaskChanged;

        /// <summary>
        /// Event than is raised after the <see cref="ValidationString"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback ValidationStringChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MaskCompletedChanged;

        /// <summary>
        /// Event than is raised after the <see cref="InvalidValueBehavior"/> property has changed.
        /// </summary>
        [Obsolete("Event will not help due to internal arhitecture changes")]
        public event PropertyChangedCallback InvalidValueBehaviorChanged;

        /// <summary>
        /// Event than is raised after the <see cref="DateSeparator"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback DateSeparatorChanged;

        /// <summary>
        /// Event than is raised after the <see cref="TimeSeparator"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback TimeSeparatorChanged;

        /// <summary>
        /// Event than is raised after the <see cref="DecimalSeparator"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback DecimalSeparatorChanged;

        /// <summary>
        /// Event than is raised after the <see cref="NumberGroupSeparator"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback NumberGroupSeparatorChanged;

        /// <summary>
        /// Event than is raised after the <see cref="CurrencySymbol"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback CurrencySymbolChanged;

        /// <summary>
        /// Event than is raised after the <see cref="PromptChar"/> property has changed.
        /// </summary>
        public event PropertyChangedCallback PromptCharChanged;

        /// <summary>
        /// Event that is raised when <see cref="WatermarkTextMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WatermarkTextModeChanged;

        ///// <summary>
        ///// Event that is raised when <see cref="WatermarkText"/> property is changed.
        ///// </summary>
        //public event PropertyChangedCallback WatermarkTextChanged;

        /// <summary>
        /// Event that is raised when <see cref="WatermarkTextIsVisible"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WatermarkTextIsVisibleChanged;

        /// <summary>
        /// Occurs when the control is validating.
        /// </summary>
        public event CancelEventHandler Validating;

        /// <summary>
        /// Occurs when the control is finished validating.
        /// </summary>
        public event EventHandler Validated;

        /// <summary>
        /// Occurs when MaskedTextBox has finished validating the current value using the ValidatingString property.
        /// NOTE: event occurs only when ValidatingString is not empty.
        /// </summary>
        public event StringValidationCompletedEventHandler StringValidationCompleted;

        /// <summary>
        /// Occurs when [text selection on focus changed].
        /// </summary>
        public event PropertyChangedCallback TextSelectionOnFocusChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnterToMoveNext"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnterToMoveNextChanged;

        #endregion Events

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MinLengthChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback WatermarkTemplateChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback WatermarkTextChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        internal string mValue;
        internal bool mIsLoaded = false;
        internal bool mValueChanged = true;
        internal string oldValue;

        internal static EditorBase editorBase;

#if WPF
        private AdornerLayer aLayer;
        private TextBoxSelectionAdorner txtSelectionAdorner1;
#endif

        #region WaterMark

        /// <summary>
        /// Gets or sets the water mark template.
        /// </summary>
        /// <value>The water mark template.</value>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the WaterMarkTemplate dependency property.
        /// </summary>
        public static DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(MaskedTextBox), new PropertyMetadata(OnWaterMarkTemplateChanged));

        /// <summary>
        /// Gets or sets the water mark text.
        /// </summary>
        /// <value>The water mark text.</value>
        public string WatermarkText
        {
            set { SetValue(WatermarkTextProperty, value); }
            get { return (string)GetValue(WatermarkTextProperty); }
        }

        /// <summary>
        /// Get or set numeric mode
        /// </summary>
        public bool IsNumeric
        {
            get { return (bool)GetValue(IsNumericProperty); }
            set { SetValue(IsNumericProperty, value); }
        }

        /// <summary>
        /// Indicates numeric mode
        /// </summary>
        public static readonly DependencyProperty IsNumericProperty =
          DependencyProperty.Register("IsNumeric", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false, new PropertyChangedCallback(OnMinLengthChanged)));

        /// <summary>
        ///
        /// </summary>
        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        /// <summary>
        /// Identifies MinLength dependency property
        /// </summary>
        public static readonly DependencyProperty MinLengthProperty =
            DependencyProperty.Register("MinLength", typeof(int), typeof(MaskedTextBox), new PropertyMetadata(0, new PropertyChangedCallback(OnMinLengthChanged)));

        /// <summary>
        /// Identifies the WaterMarkText dependency property.
        /// </summary>
        public static DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(MaskedTextBox), new PropertyMetadata("Type here...", new PropertyChangedCallback(OnWaterMarkTextChanged)));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnWaterMarkTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((MaskedTextBox)obj) != null)
            {
                ((MaskedTextBox)obj).OnWaterMarkTextChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWaterMarkTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.WatermarkTextChanged != null)
                this.WatermarkTextChanged(this, args);
        }

        /// <summary>
        /// Gets or sets the water mark visibility.
        /// </summary>
        /// <value>The water mark visibility.</value>
        public Visibility WatermarkVisibility
        {
            set
            {
                value = CoerceWatermarkVisibility(this, value);
                SetValue(WatermarkVisibilityProperty, value);
            }
            get { return (Visibility)GetValue(WatermarkVisibilityProperty); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static Visibility CoerceWatermarkVisibility(DependencyObject d, object baseValue)
        {
            var maskedTextBox = (MaskedTextBox)d;

            if (maskedTextBox.WatermarkTextIsVisible && (((Visibility)baseValue) == Visibility.Visible))
            {
                maskedTextBox.ContentElementVisibility = Visibility.Collapsed;
                return Visibility.Visible;
            }
            else
            {
                maskedTextBox.ContentElementVisibility = Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Identifies the WaterMarkVisibility dependency property.
        /// </summary>
        public static DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(MaskedTextBox), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        ///
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility ContentElementVisibility
        {
            get { return (Visibility)GetValue(ContentElementVisibilityProperty); }
            set { SetValue(ContentElementVisibilityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ContentElementVisibilityProperty =
            DependencyProperty.Register("ContentElementVisibility", typeof(Visibility), typeof(MaskedTextBox), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        ///
        /// </summary>
        public Brush WatermarkTextForeground
        {
            get { return (Brush)GetValue(WatermarkTextForegroundProperty); }
            set { SetValue(WatermarkTextForegroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkTextForegroundProperty =
            DependencyProperty.Register("WatermarkTextForeground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        public Brush WatermarkBackground
        {
            get { return (Brush)GetValue(WatermarkBackgroundProperty); }
            set { SetValue(WatermarkBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkBackgroundProperty =
            DependencyProperty.Register("WatermarkBackground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        public double WatermarkOpacity
        {
            get { return (double)GetValue(WatermarkOpacityProperty); }
            set { SetValue(WatermarkOpacityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkOpacityProperty =
            DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(MaskedTextBox), new PropertyMetadata((double)0.5));

        /// <summary>
        ///
        /// </summary>
        public bool WatermarkTextIsVisible
        {
            get { return (bool)GetValue(WatermarkTextIsVisibleProperty); }
            set { SetValue(WatermarkTextIsVisibleProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkTextIsVisibleProperty =
            DependencyProperty.Register("WatermarkTextIsVisible", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(true, OnWatermarkTextIsVisibleChanged));

        #region Property Changed Callback

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnWatermarkTextIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnWatermarkTextIsVisibleChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWatermarkTextIsVisibleChanged(DependencyPropertyChangedEventArgs args)
        {
            if (MaskedText != null && CharCollection != null)
            {
                string maskValue = MaskHandler.maskHandler.ValueFromMaskedText(this, MaskFormat.ExcludePromptAndLiterals, MaskedText, CharCollection);
                if (this.WatermarkTextIsVisible && String.IsNullOrEmpty(maskValue))
                {
                    this.ContentElementVisibility = Visibility.Collapsed;
                    this.WatermarkVisibility = Visibility.Visible;
                }
                else
                {
                    this.WatermarkVisibility = Visibility.Collapsed;
                    this.ContentElementVisibility = Visibility.Visible;
                }
            }
            if (this.WatermarkTextIsVisibleChanged != null)
            {
                this.WatermarkTextIsVisibleChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinLengthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var box = obj as MaskedTextBox;
            if (box != null)
            {
                box.OnMinLengthChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMinLengthChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MinLengthChanged != null)
            {
                this.MinLengthChanged(this, args);
            }
            if (MinLength > MaxLength || MinLength < 0)
            {
                throw new InvalidOperationException("MinLength should be less than MaxLength");
            }
        }

        /// <summary>
        /// Called when [water mark template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnWaterMarkTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var box = obj as MaskedTextBox;
            if (box != null)
            {
                box.OnWaterMarkTemplateChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWaterMarkTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.WatermarkTemplateChanged != null)
            {
                this.WatermarkTemplateChanged(this, args);
            }
        }

        #endregion Property Changed Callback

        #endregion WaterMark

        #region Constructor

#if WPF
        /// <summary>
        /// 
        /// </summary>
        static MaskedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskedTextBox), new FrameworkPropertyMetadata(typeof(MaskedTextBox)));
        }

#endif

        /// <summary>
        ///
        /// </summary>
        public ICommand pastecommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ICommand copycommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ICommand cutcommand { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public MaskedTextBox()
        {
            editorBase = new EditorBase();
            pastecommand = new DelegateCommand<object>(_pastecommand, Canpaste);
            copycommand = new DelegateCommand<object>(_copycommand, Canpaste);
            cutcommand = new DelegateCommand<object>(_cutcommand, Canpaste);
#if WPF
            this.AddHandler(CommandManager.PreviewExecutedEvent,
new ExecutedRoutedEventHandler(CommandExecuted), true);
            this.CommandBindings.Add(new CommandBinding(EditorCommands.Clear, ExecuteClearCommand, CanExecuteClearCommand));
#endif
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(MaskedTextBox));
            }
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(MaskedTextBox);
#endif
            this.Loaded += MaskedTextBox_Loaded;

            this.LostFocus += MaskedTextBox_LostFocus;
            this.TextChanged += MaskedTextBox_TextChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _pastecommand(object parameter)
        {
            MaskHandler.maskHandler.HandlePaste(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _copycommand(object parameter)
        {
            copy();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void _cutcommand(object parameter)
        {
            cut();
        }

        /// <summary>
        /// 
        /// </summary>
        private void copy()
        {
            try
            {
                Clipboard.SetText(this.SelectedText);
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private bool Canpaste(object parameter)
        {
            return true;
        }

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                MaskHandler.maskHandler.HandlePaste(this);
                e.Handled = true;
            }
            if (e.Command == ApplicationCommands.Cut)
            {
                cut();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteClearCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.Text.Length > 0;
        }

#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.Mask))
            {
#if WPF
#if SyncfusionFramework3_5
                     this.Value = this.Text;
#elif SyncfusionFramework4_0
                this.SetCurrentValue(ValueProperty, this.Text);
#endif
#endif
            }
            else if (!string.IsNullOrEmpty(this.Mask) && this.Text == "")
            {
#if WPF
#if SyncfusionFramework3_5
                     this.Value = "";
#elif SyncfusionFramework4_0
                this.SetCurrentValue(ValueProperty, "");
#endif
#endif
            }
        }

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Use IsReadOnly Property")]
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskedTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.mIsLoaded = true;
            CoerceWatermarkVisibility(this, this.WatermarkVisibility);
            this.LoadTextBox();

#if WPF
            if (EnableTouch)
            {
                if (aLayer == null)
                    aLayer = AdornerLayer.GetAdornerLayer(this);
                if (aLayer != null && txtSelectionAdorner1 == null)
                {
                    txtSelectionAdorner1 = new TextBoxSelectionAdorner(this);
                    aLayer.Add(txtSelectionAdorner1);
                }
            }
#endif
        }

        #endregion Constructor

        #region EditorBase

#if WPF
        /// <summary>
        /// 
        /// </summary>
        internal new int CaretIndex
#endif
#if SILVERLIGHT
        internal int CaretIndex
#endif
        {
            get { return (int)GetValue(CaretIndexProperty); }
            set
            {
                SelectionStart = value;
                SetValue(CaretIndexProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.Register("CaretIndex", typeof(int), typeof(MaskedTextBox), new PropertyMetadata((int)0));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
#if WPF
        public new bool IsUndoEnabled
#endif
#if SILVERLIGHT
         public bool IsUndoEnabled
#endif
        {
            get { return (bool)GetValue(IsUndoEnabledProperty); }
            set { SetValue(IsUndoEnabledProperty, value); }
        }

#if WPF

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false));

#endif
#if SILVERLIGHT
         /// <summary>
         ///
         /// </summary>
         public static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(MaskedTextBox), new   PropertyMetadata(false));
#endif

#if WPF
        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
#endif
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MaskedTextBox), new PropertyMetadata(new CornerRadius(1)));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush FocusedBackground
        {
            get { return (Brush)GetValue(FocusedBackgroundProperty); }
            set { SetValue(FocusedBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FocusedBackgroundProperty =
            DependencyProperty.Register("FocusedBackground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush FocusedForeground
        {
            get { return (Brush)GetValue(FocusedForegroundProperty); }
            set { SetValue(FocusedForegroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FocusedForegroundProperty =
            DependencyProperty.Register("FocusedForeground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush FocusedBorderBrush
        {
            get { return (Brush)GetValue(FocusedBorderBrushProperty); }
            set { SetValue(FocusedBorderBrushProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FocusedBorderBrushProperty =
            DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public bool IsCaretAnimationEnabled
        {
            get { return (bool)GetValue(IsCaretAnimationEnabledProperty); }
            set { SetValue(IsCaretAnimationEnabledProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsCaretAnimationEnabledProperty =
            DependencyProperty.Register("IsCaretAnimationEnabled", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
        public Brush ReadOnlyBackground
        {
            get { return (Brush)GetValue(ReadOnlyBackgroundProperty); }
            set { SetValue(ReadOnlyBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ReadOnlyBackgroundProperty =
            DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Property will not help due to internal arhitecture changes")]
#if WPF
        public Brush SelectionForeground
#endif
#if SILVERLIGHT
         public new Brush SelectionForeground
#endif
        {
            get { return (Brush)GetValue(SelectionForegroundProperty); }
            set { SetValue(SelectionForegroundProperty, value); }
        }

#if WPF

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public new static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(MaskedTextBox), new PropertyMetadata(new SolidColorBrush()));
#endif

        /// <summary>
        ///
        /// </summary>
        public InvalidInputBehavior InvalidValueBehavior
        {
            get { return (InvalidInputBehavior)GetValue(InvalidValueBehaviorProperty); }
            set { SetValue(InvalidValueBehaviorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty InvalidValueBehaviorProperty =
            DependencyProperty.Register("InvalidValueBehavior", typeof(InvalidInputBehavior), typeof(MaskedTextBox), new PropertyMetadata(InvalidInputBehavior.None, OnInvalidValueBehaviorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnInvalidValueBehaviorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MaskedTextBox m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnInvalidValueBehaviorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnInvalidValueBehaviorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.InvalidValueBehaviorChanged != null)
            {
                InvalidValueBehaviorChanged(this, args);
            }
        }

        #endregion EditorBase

        #region Properties

        /// <summary>
        ///
        /// </summary>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MaskedText
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
                SetValue(MaskedTextProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<CharacterProperties> mCharCollection = new ObservableCollection<CharacterProperties>();

        /// <summary>
        /// 
        /// </summary>
        internal ObservableCollection<CharacterProperties> CharCollection
        {
            get { return mCharCollection; }
            set { mCharCollection = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public char PromptChar
        {
            get { return (char)GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string CurrencySymbol
        {
            get { return (string)GetValue(CurrencySymbolProperty); }
            set { SetValue(CurrencySymbolProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string DateSeparator
        {
            get { return (string)GetValue(DateSeparatorProperty); }
            set { SetValue(DateSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string TimeSeparator
        {
            get { return (string)GetValue(TimeSeparatorProperty); }
            set { SetValue(TimeSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string DecimalSeparator
        {
            get { return (string)GetValue(DecimalSeparatorProperty); }
            set { SetValue(DecimalSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string NumberGroupSeparator
        {
            get { return (string)GetValue(NumberGroupSeparatorProperty); }
            set { SetValue(NumberGroupSeparatorProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public WatermarkTextMode WatermarkTextMode
        {
            get { return (WatermarkTextMode)GetValue(WatermarkTextModeProperty); }
            set { SetValue(WatermarkTextModeProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public MaskFormat TextMaskFormat
        {
            get { return (MaskFormat)GetValue(TextMaskFormatProperty); }
            set { SetValue(TextMaskFormatProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object CoerceMaskValue(DependencyObject d, object baseValue)
        {
            var maskedTextbox = (MaskedTextBox)d;
            if (maskedTextbox != null && (!string.IsNullOrEmpty(maskedTextbox.Mask) || baseValue == null))
            {
                if (maskedTextbox.mValueChanged && maskedTextbox.mIsLoaded)
                {
                    if (baseValue == null)
                        baseValue = "";
                    maskedTextbox.CharCollection = MaskHandler.maskHandler.CreateRegularExpression(maskedTextbox);

                    var displaytext = MaskHandler.maskHandler.CoerceValue(maskedTextbox, baseValue.ToString(), MaskFormat.IncludePromptAndLiterals);
                    if (maskedTextbox.CharCollection != null)
                    {
                        baseValue = MaskHandler.maskHandler.ValueFromMaskedText(maskedTextbox,
                            maskedTextbox.TextMaskFormat, displaytext, maskedTextbox.CharCollection);
                        maskedTextbox.mValue = MaskHandler.maskHandler.ValueFromMaskedText(maskedTextbox,
                            MaskFormat.ExcludePromptAndLiterals, displaytext, maskedTextbox.CharCollection);
                    }
                    maskedTextbox.MaskedText = displaytext;
                }
            }
            else
            {
                if (maskedTextbox != null) maskedTextbox.MaskedText = baseValue.ToString();
            }
            Int32 val;
            if (baseValue != null)
            {
                Int32.TryParse(baseValue.ToString(), out val);
                if (val < 0 && maskedTextbox.Mask.StartsWith("(") && maskedTextbox.Mask.EndsWith(")"))
                {
                    return val.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = 0, NumberNegativePattern = 0 });
                }
            }
            return baseValue;
        }

        /// <summary>
        ///
        /// </summary>
        public StringValidation StringValidation
        {
            get { return (StringValidation)GetValue(StringValidationProperty); }
            set { SetValue(StringValidationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        [Obsolete("Use MaxLength property")]
        public int MaxCharLength
        {
            get { return (int)GetValue(MaxCharLengthProperty); }
            set { SetValue(MaxCharLengthProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string ValidationString
        {
            get { return (string)GetValue(ValidationStringProperty); }
            set { SetValue(ValidationStringProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool EnterToMoveNext
        {
            get { return (bool)GetValue(EnterToMoveNextProperty); }
            set { SetValue(EnterToMoveNextProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool TextSelectionOnFocus
        {
            get { return (bool)GetValue(TextSelectionOnFocusProperty); }
            set { SetValue(TextSelectionOnFocusProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnTextSelectionOnFocusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MaskedTextBox m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnTextSelectionOnFocusChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnTextSelectionOnFocusChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.TextSelectionOnFocusChanged != null)
            {
                this.TextSelectionOnFocusChanged(this, args);
            }
        }

        #endregion Properties

        #region PropertyChanged Callback

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnEnterToMoveNextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MaskedTextBox m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnEnterToMoveNextChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnEnterToMoveNextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.EnterToMoveNextChanged != null)
            {
                this.EnterToMoveNextChanged(this, args);
            }
        }

        #endregion PropertyChanged Callback

        #region DependencyProperties

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EnterToMoveNextProperty =
            DependencyProperty.Register("EnterToMoveNext", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false, OnEnterToMoveNextChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValidationStringProperty =
            DependencyProperty.Register("ValidationString", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnValidationStringChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValidationStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnValidationStringChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValidationStringChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ValidationStringChanged != null)
            {
                ValidationStringChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxCharLengthProperty =
            DependencyProperty.Register("MaxCharLength", typeof(int), typeof(MaskedTextBox), new PropertyMetadata(int.MaxValue));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty StringValidationProperty =
            DependencyProperty.Register("StringValidation", typeof(StringValidation), typeof(MaskedTextBox), new PropertyMetadata(StringValidation.OnLostFocus));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TextSelectionOnFocusProperty =
            DependencyProperty.Register("TextSelectionOnFocus", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(true, OnTextSelectionOnFocusChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PromptCharProperty =
            DependencyProperty.Register("PromptChar", typeof(char), typeof(MaskedTextBox), new PropertyMetadata('_', OnPromptCharChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPromptCharChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnPromptCharChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPromptCharChanged(DependencyPropertyChangedEventArgs args)
        {
            if (PromptCharChanged != null)
            {
                PromptCharChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaskedTextProperty =
             DependencyProperty.Register("MaskedText", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnMaskChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaskChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnMaskChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaskChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MaskChanged != null)
            {
                this.MaskChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(MaskedTextBox), new PropertyMetadata(CultureInfo.CurrentCulture));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CurrencySymbolProperty =
          DependencyProperty.Register("CurrencySymbol", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnCurrencySymbolChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCurrencySymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnCurrencySymbolChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnCurrencySymbolChanged(DependencyPropertyChangedEventArgs args)
        {
            if (CurrencySymbolChanged != null)
            {
                CurrencySymbolChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DateSeparatorProperty =
            DependencyProperty.Register("DateSeparator", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnDateSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnDateSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnDateSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnDateSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (DateSeparatorChanged != null)
            {
                DateSeparatorChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TimeSeparatorProperty =
            DependencyProperty.Register("TimeSeparator", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnTimeSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnTimeSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnTimeSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnTimeSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (TimeSeparatorChanged != null)
            {
                TimeSeparatorChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DecimalSeparatorProperty =
            DependencyProperty.Register("DecimalSeparator", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnDecimalSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnDecimalSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnDecimalSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnDecimalSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (DecimalSeparatorChanged != null)
            {
                DecimalSeparatorChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool GroupSeperatorEnabled
        {
            get { return (bool)GetValue(GroupSeperatorEnabledProperty); }
            set { SetValue(GroupSeperatorEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupSeperatorEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupSeperatorEnabledProperty =
            DependencyProperty.Register("GroupSeperatorEnabled", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(true, new PropertyChangedCallback(OnNumberGroupSeparatorChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberGroupSeparatorProperty =
            DependencyProperty.Register("NumberGroupSeparator", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnNumberGroupSeparatorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNumberGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnNumberGroupSeparatorChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNumberGroupSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
            if (NumberGroupSeparatorChanged != null)
            {
                NumberGroupSeparatorChanged(this, args);
            }
            if (mIsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkTextModeProperty =
            DependencyProperty.Register("WatermarkTextMode", typeof(WatermarkTextMode), typeof(MaskedTextBox), new PropertyMetadata(WatermarkTextMode.HideTextOnFocus, OnWatermarkTextModeChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnWatermarkTextModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnWatermarkTextModeChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWatermarkTextModeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (WatermarkTextModeChanged != null)
            {
                WatermarkTextModeChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TextMaskFormatProperty =
            DependencyProperty.Register("TextMaskFormat", typeof(MaskFormat), typeof(MaskedTextBox), new PropertyMetadata(MaskFormat.IncludeLiterals,OnTestMaskFormatChanged));
     /// <summary>
     /// 
     /// </summary>
     /// <param name="obj"></param>
     /// <param name="args"></param>
        public static void OnTestMaskFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var m = (MaskedTextBox)obj;
            if (m != null)
            {
                m.OnTestMaskFormatChanged(args);
            }
        }

        protected void OnTestMaskFormatChanged(DependencyPropertyChangedEventArgs args)
        {
            LoadTextBox();
        }
#if WPF
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(MaskedTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, new CoerceValueCallback(CoerceMaskValue), false, UpdateSourceTrigger.LostFocus));

        /// <summary>
        /// 
        /// </summary>
        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false, OnEnableTouchChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnEnableTouchChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj as MaskedTextBox != null)
                (obj as MaskedTextBox).OnEnableTouchChanged(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        private void OnEnableTouchChanged(DependencyPropertyChangedEventArgs args)
        {
            if (aLayer == null)
                aLayer = AdornerLayer.GetAdornerLayer(this);

            if ((bool)args.NewValue)
            {
                if (aLayer != null)
                {
                    txtSelectionAdorner1 = new TextBoxSelectionAdorner(this);
                    if(txtSelectionAdorner1 != null)
                        aLayer.Add(txtSelectionAdorner1);
                }
            }
            else
            {
                if (aLayer != null && txtSelectionAdorner1 != null)
                {
                    aLayer.Remove(txtSelectionAdorner1);
                }
            }
        }

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(string.Empty, OnValueChanged));
#endif

        #endregion DependencyProperties

#if WPF
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (this.IsReadOnly == true)
            {
                e.Handled = true;
                base.OnContextMenuOpening(e);
            }
            else
            {
                base.OnContextMenuOpening(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = true;
            base.OnDrop(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.TextSelectionOnFocus)
            {
                if (this.IsFocused == false)
                {
                    e.Handled = true;
                    this.Focus();
                }
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var box = obj as MaskedTextBox;
            if (box != null)
            {
                box.OnValueChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            if (mValueChanged && this.mIsLoaded)
            {
                string tempVal = CoerceMaskValue(this, this.Value).ToString();
                if (tempVal != this.Value)
                {
                    this.SetValue(false, tempVal);
                    return;
                }
            }
#endif
            if (this.mIsLoaded)
            {
                if (string.IsNullOrEmpty(this.Mask))
                {
                    MaskCompleted = Regex.IsMatch(this.MaskedText, this.ValidationString);
                }
                else
                {
                    MaskCompleted = MaskHandler.maskHandler.ValueFromMaskedText(this, MaskFormat.IncludeLiterals, this.MaskedText, this.CharCollection) == this.MaskedText;
                }
            }

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, args);
            }
            oldValue = (string)args.OldValue;
            if (!string.IsNullOrEmpty(this.Value))
            {
                this.WatermarkVisibility = Visibility.Collapsed;
            }
            else
            {
                if (this.Focus() != true)
                    this.WatermarkVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool MaskCompleted
        {
            get { return (bool)GetValue(MaskCompletedProperty); }
            set { SetValue(MaskCompletedProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaskCompletedProperty =
            DependencyProperty.Register("MaskCompleted", typeof(bool), typeof(MaskedTextBox), new PropertyMetadata(false, OnMaskCompletedPropertyChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMaskCompletedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var mask = (MaskedTextBox)obj;
            if (mask != null)
                mask.OnMaskCompletedPropertyChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnMaskCompletedPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.MaskCompletedChanged != null)
            {
                this.MaskCompletedChanged(this, args);
            }
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (mMouseLeftButtonDown == false)
            {
                if (string.IsNullOrEmpty(this.Mask))
                {
                    if (!OnValidating(new CancelEventArgs(false)))
                    {
                        var maskedText = this.MaskedText;
                        var validationerror = "";
                        var validationstatus = true;
                        if (this.MaskedText.Length < this.MinLength && this.MaskedText.Length > 0)
                        {
                            this.MaskedText = oldValue.Length >= this.MinLength ? oldValue : "";
                        }
                        validationstatus = Regex.IsMatch(maskedText, this.ValidationString);
                        string message = validationstatus ? "String validation succeeded" : "String validation failed";

                        if (!validationstatus)
                        {
                            switch (InvalidValueBehavior)
                            {
                                case InvalidInputBehavior.DisplayErrorMessage:
                                    OnStringValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, this.ValidationString));
                                    OnValidated(EventArgs.Empty);
                                    MessageBox.Show(message, "Invalid value", MessageBoxButton.OK);
                                    break;

                                case InvalidInputBehavior.None:
                                    OnStringValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, this.ValidationString));
                                    OnValidated(EventArgs.Empty);
                                    break;

                                case InvalidInputBehavior.ResetValue:
                                    OnStringValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, this.ValidationString));
                                    OnValidated(EventArgs.Empty);
#if WPF
#if SyncfusionFramework3_5
                                        this.Value = "";
#elif SyncfusionFramework4_0
                                    this.SetCurrentValue(ValueProperty, "");
#endif
#endif
                                    this.WatermarkVisibility = Visibility.Visible;
                                    return;
                            }
                        }
                        else
                        {
                            this.OnStringValidationCompleted(new StringValidationEventArgs(validationstatus, validationerror, this.ValidationString));
                            this.OnValidated(EventArgs.Empty);
                        }
                    }
                }
                base.OnLostFocus(e);
            }
            else
            {
                this.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaskedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!mMouseLeftButtonDown)
            {
                var mask = this.Mask;
                if (mask != null && string.IsNullOrEmpty(mask))
                {
                    var maskedText = this.MaskedText;
                    if (maskedText != null && string.IsNullOrEmpty(maskedText))
                    {
                        this.WatermarkVisibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (CharCollection != null && MaskedText != null)
                    {
                        var tempVal = MaskHandler.maskHandler.ValueFromMaskedText(this, MaskFormat.ExcludePromptAndLiterals, this.MaskedText, CharCollection);
                        if (string.IsNullOrEmpty(tempVal))
                        {
                            this.WatermarkVisibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void OnStringValidationCompleted(StringValidationEventArgs e)
        {
            if (StringValidationCompleted != null)
            {
                StringValidationCompleted(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void OnValidated(EventArgs e)
        {
            if (Validated != null)
            {
                Validated(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool OnValidating(CancelEventArgs e)
        {
            if (Validating != null)
            {
                Validating(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (WatermarkTextMode.HideTextOnFocus == WatermarkTextMode)
                this.WatermarkVisibility = Visibility.Collapsed;
            if (TextSelectionOnFocus)
            {
                this.SelectAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void LoadTextBox()
        {
            var tempVal = CoerceMaskValue(this, this.Value).ToString();
            if (this.Value != tempVal && this.Value != string.Empty)
            {
                this.SetValue(false, tempVal);
            }
            else if (this.Value == string.Empty)
            {
                this.SetValue(false, this.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal CultureInfo GetCulture()
        {
            CultureInfo cultureInfo;
            if (Culture != null && !Equals(Culture, CultureInfo.InvariantCulture))
            {
                cultureInfo = this.Culture.Clone() as CultureInfo;
            }
            else
            {
                cultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            }

            if (cultureInfo != null)
            {
                if (CurrencySymbol != string.Empty)
                    cultureInfo.NumberFormat.CurrencySymbol = CurrencySymbol;
                cultureInfo.NumberFormat.CurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol[0].ToString();

                if (DecimalSeparator != string.Empty)
                    cultureInfo.NumberFormat.NumberDecimalSeparator = DecimalSeparator;
                cultureInfo.NumberFormat.NumberDecimalSeparator =
                    cultureInfo.NumberFormat.NumberDecimalSeparator[0].ToString();

                if (!GroupSeperatorEnabled)
                {
                    cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
                }

                if (GroupSeperatorEnabled)
                {
                    if (NumberGroupSeparator != string.Empty)
                        cultureInfo.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
                    cultureInfo.NumberFormat.NumberGroupSeparator =
                        cultureInfo.NumberFormat.NumberGroupSeparator[0].ToString();
                }
            }

            return cultureInfo;
        }

#if WPF

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Equals(e.Key, Key.Space))
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, " ");
                if (e.Handled)
                {
                    if (e.Handled)
                    {
                        var tempVal = MaskHandler.maskHandler.CreateValueFromText(this);
                        if (tempVal != this.Value)
                        {
                            this.SetValue(false, tempVal);
                        }
                    }
                }
            }
            if (e.Key == Key.Z)
            {
                e.Handled = true;
            }
            else
            {
                if (this.SelectedText != string.Empty && !this.IsReadOnly && Keyboard.Modifiers == ModifierKeys.None && e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Down)
                {
                    if (this.Mask == "")
                    {
                        this.SelectedText = string.Empty;
                    }
                    else
                    {
                        MaskHandler.maskHandler.HandleBackSpaceKey(this);
                        if (SelectionStart > 0)
                            this.CaretIndex = this.SelectionStart + 1;
                        else
                            this.CaretIndex = 0;
                    }
                }

                e.Handled = MaskHandler.maskHandler.HandleKeyDown(this, e);

                if (e.Handled)
                {
                    var tempVal = MaskHandler.maskHandler.CreateValueFromText(this);
                    if (tempVal != this.Value)
                    {
                        this.SetValue(false, tempVal);
                    }
                }
            }
            base.OnPreviewKeyDown(e);
        }

#endif

        /// <summary>
        /// Called when <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (WatermarkTextMode.HideTextOnTyping == WatermarkTextMode)
                this.WatermarkVisibility = Visibility.Collapsed;

            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.V)
                {
                    MaskHandler.maskHandler.HandlePaste(this);
                    e.Handled = true;
                }
                if (e.Key == Key.C)
                {
                }
                if (e.Key == Key.X)
                {
                    cut();
                    e.Handled = true;
                }
            }
#if SILVERLIGHT
            else
            {
                if (e.Key == Key.Space)
                {
                    e.Handled = MaskHandler.maskHandler.MatchWithMask(this, " ");
                    if (e.Handled == true)
                    {
                        if (e.Handled == true)
                        {
                            string tempVal = MaskHandler.maskHandler.CreateValueFromText(this);
                            if (tempVal != this.Value)
                            {
                                this.SetValue(false, tempVal);
                            }
                        }
                    }
                }

                else
                {
                    if (this.SelectedText != string.Empty && Keyboard.Modifiers == ModifierKeys.None && e.Key != Key.Enter && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Down)
                    {
                        if (this.Mask == "")
                        {
                            this.SelectedText = string.Empty;
                        }
                        else
                        {
                            MaskHandler.maskHandler.HandleBackSpaceKey(this);
                            if (SelectionStart > 0)
                                this.CaretIndex = this.SelectionStart + 1;
                            else
                                this.CaretIndex = 0;
                        }
                    }

                    e.Handled = MaskHandler.maskHandler.HandleKeyDown(this, e);

                    if (e.Handled == true)
                    {
                        string tempVal = MaskHandler.maskHandler.CreateValueFromText(this);
                        if (tempVal != this.Value)
                        {
                            this.SetValue(false, tempVal);
                        }
                    }
                }
            }
#endif
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        private void cut()
        {
            try
            {
                if (this.SelectionLength > 0)
                {
                    Clipboard.SetText(this.SelectedText);
                    MaskHandler.maskHandler.HandleDeleteKey(this);
                }
            }
            catch (COMException)
            {
                //To handle COMException for another application accessing already opened Clipboard.
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (MaskHandler.maskHandler != null)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                if (!string.IsNullOrEmpty(this.Mask))
                {
                    if (e.Handled)
                    {
                        var tempVal = MaskHandler.maskHandler.CreateValueFromText(this);
                        if (tempVal != this.Value)
                        {
                            this.SetValue(false, tempVal);
                        }
                    }
                }
            }
            base.OnTextInput(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsReload"></param>
        /// <param name="_Value"></param>
        internal void SetValue(bool? IsReload, object _Value)
        {
            mValueChanged = false;
#if WPF
#if SyncfusionFramework3_5
                this.Value = _Value.ToString();
#elif SyncfusionFramework4_0
            this.SetCurrentValue(ValueProperty, _Value.ToString());
#endif
#endif
            if (MaskHandler.maskHandler != null)
                this.mValue = MaskHandler.maskHandler.ValueFromMaskedText(this, MaskFormat.ExcludePromptAndLiterals, this.MaskedText, this.CharCollection);
#if SILVERLIGHT
            this.Value = this.mValue;
#endif
            mValueChanged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private ContentControl PART_Watermark;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Watermark = this.GetTemplateChild("PART_Watermark") as ContentControl;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool mMouseLeftButtonDown = false;

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            mMouseLeftButtonDown = true;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            mMouseLeftButtonDown = false;
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            mMouseLeftButtonDown = false;
            base.OnMouseLeave(e);
        }
    }
}