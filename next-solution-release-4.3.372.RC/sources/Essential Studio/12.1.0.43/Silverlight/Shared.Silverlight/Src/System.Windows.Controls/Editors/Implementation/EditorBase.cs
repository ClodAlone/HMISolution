#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

#if WPF

namespace Syncfusion.Windows.Shared
#endif

#if SILVERLIGHT
namespace Syncfusion.Windows.Tools.Controls
#endif
{
    /// <summary>
    ///
    /// </summary>
    public class EditorBase : TextBox
    {
        #region Events

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback CultureChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback NumberFormatChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback WaterMarkTemplateChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback WaterMarkTextChanged;

        /// <summary>
        /// Occurs when <see cref="IsUndoEnabled"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsUndoEnabledChanged;

        /// <summary>
        /// Occurs when [text selection on focus changed].
        /// </summary>
        public event PropertyChangedCallback TextSelectionOnFocusChanged;

        /// <summary>
        /// Event that is raised when <see cref="NegativeForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback NegativeForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsValueNegativeChanged"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsValueNegativeChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnterToMoveNext"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnterToMoveNextChanged;

        #endregion Events

        internal bool minusPressed = false;
#if WPF
        private AdornerLayer aLayer;
        private TextBoxSelectionAdorner txtSelectionAdorner1;
        private ExtendedScrollingAdorner vAdorner;
#endif

        /// <summary>
        ///
        /// </summary>
        public EditorBase()
        {
#if WPF
            this.MouseDoubleClick += new MouseButtonEventHandler(EditorBase_MouseDoubleClick);
            this.Loaded += new RoutedEventHandler(EditorBase_Loaded);
#endif
        }

#if WPF

        private void EditorBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (EnableExtendedScrolling)
            {
                if (aLayer == null)
                    aLayer = AdornerLayer.GetAdornerLayer(this);

                if (aLayer != null && vAdorner == null)
                {
                    vAdorner = new ExtendedScrollingAdorner(this);
                }
                if (aLayer != null && vAdorner != null)
                {
                    if (aLayer.GetAdorners(this) == null)
                        aLayer.Add(vAdorner);
                }
            }

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
        }

#endif

#if WPF

        private void EditorBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.SelectAll();
        }

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

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = true;
            base.OnDrop(e);
        }

#endif

        /// <summary>
        ///
        /// </summary>
#if WPF

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
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
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(EditorBase), new PropertyMetadata(OnCornerRadiusChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

#if WPF

        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnEnableTouchChanged));

        public static void OnEnableTouchChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj as EditorBase != null)
                (obj as EditorBase).OnEnableTouchChanged(args);
        }

        private void OnEnableTouchChanged(DependencyPropertyChangedEventArgs args)
        {
            if (aLayer == null)
                aLayer = AdornerLayer.GetAdornerLayer(this);

            if ((bool)args.NewValue)
            {
                if (aLayer != null)
                {
                    txtSelectionAdorner1 = new TextBoxSelectionAdorner(this);
                    aLayer.Add(txtSelectionAdorner1);
                }
            }
            else
            {
                if (aLayer != null)
                {
                    aLayer.Remove(txtSelectionAdorner1);
                }
            }
        }

        public bool EnableRangeAdorner
        {
            get { return (bool)GetValue(EnableRangeAdornerProperty); }
            set { SetValue(EnableRangeAdornerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableRangeAdorner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableRangeAdornerProperty =
            DependencyProperty.Register("EnableRangeAdorner", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        public bool EnableExtendedScrolling
        {
            get { return (bool)GetValue(EnableExtendedScrollingProperty); }
            set { SetValue(EnableExtendedScrollingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableExtendedScrolling.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableExtendedScrollingProperty =
            DependencyProperty.Register("EnableExtendedScrolling", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnEnableExtendedScrollingChanged));

        public static void OnEnableExtendedScrollingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj as EditorBase != null)
                (obj as EditorBase).OnEnableExtendedScrollingChanged(args);
        }

        private void OnEnableExtendedScrollingChanged(DependencyPropertyChangedEventArgs args)
        {
            if (aLayer == null)
                aLayer = AdornerLayer.GetAdornerLayer(this);

            if ((bool)args.NewValue)
            {
                if (aLayer != null)
                {
                    vAdorner = new ExtendedScrollingAdorner(this);
                    aLayer.Add(vAdorner);
                }
            }
            else
            {
                if (aLayer != null)
                {
                    aLayer.Remove(vAdorner);
                }
            }
        }

        public Brush RangeAdornerBackground
        {
            get { return (Brush)GetValue(RangeAdornerBackgroundProperty); }
            set { SetValue(RangeAdornerBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeAdornerBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeAdornerBackgroundProperty =
            DependencyProperty.Register("RangeAdornerBackground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(Brushes.LightGray));

#endif

        // [Obsolete("Property will not help due to internal arhitecture changes")]
        internal Brush FocusedBackground
        {
            get { return (Brush)GetValue(FocusedBackgroundProperty); }
            set { SetValue(FocusedBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FocusedBackgroundProperty =
            DependencyProperty.Register("FocusedBackground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(OnFocusedBackgroundChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnFocusedBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        //[Obsolete("Property will not help due to internal arhitecture changes")]
        internal Brush FocusedForeground
        {
            get { return (Brush)GetValue(FocusedForegroundProperty); }
            set { SetValue(FocusedForegroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FocusedForegroundProperty =
            DependencyProperty.Register("FocusedForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(OnFocusedForegroundChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnFocusedForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase e = (EditorBase)obj;
            if (e != null)
            {
                e.OnFocusedForegroundChanged(args);
            }
        }

        private void OnFocusedForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
        }

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
            DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(EditorBase), new PropertyMetadata(OnFocusedBorderBrushChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnFocusedBorderBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

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
            DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(OnReadOnlyBackgroundChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnReadOnlyBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

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

        public static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

#endif
#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        public new static readonly DependencyProperty SelectionForegroundProperty =
            DependencyProperty.Register("SelectionForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif

        /// <summary>
        /// Gets or sets a value indicating whether [enable focus colors].
        /// </summary>
        /// <value><c>true</c> if [enable focus colors]; otherwise, <c>false</c>.</value>
        public bool EnableFocusColors
        {
            get { return (bool)GetValue(EnableFocusColorsProperty); }
            set { SetValue(EnableFocusColorsProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EnableFocusColorsProperty =
            DependencyProperty.Register("EnableFocusColors", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        #region Properties

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number format.
        /// </summary>
        /// <value>The number format.</value>
        public NumberFormatInfo NumberFormat
        {
            get { return (NumberFormatInfo)GetValue(NumberFormatProperty); }
            set { SetValue(NumberFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the editor foreground.
        /// </summary>
        /// <value>The editor foreground.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush EditorForeground
        {
            get { return (Brush)GetValue(EditorForegroundProperty); }
            set { SetValue(EditorForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the positive foreground.
        /// </summary>
        /// <value>The positive foreground.</value>
        public Brush PositiveForeground
        {
            get { return (Brush)GetValue(PositiveForegroundProperty); }
            set { SetValue(PositiveForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable negative.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enable negative; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyNegativeForeground
        {
            get { return (bool)GetValue(ApplyNegativeForegroundProperty); }
            set { SetValue(ApplyNegativeForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is negative.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is negative; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public bool IsNegative
        {
            get { return (bool)GetValue(IsNegativeProperty); }
            set { SetValue(IsNegativeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the negative foreground.
        /// </summary>
        /// <value>The negative foreground.</value>
        public Brush NegativeForeground
        {
            get { return (Brush)GetValue(NegativeForegroundProperty); }
            set { SetValue(NegativeForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is zero.
        /// </summary>
        /// <value><c>true</c> if this instance is zero; otherwise, <c>false</c>.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public bool IsZero
        {
            get { return (bool)GetValue(IsZeroProperty); }
            set { SetValue(IsZeroProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is apply zero color.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is apply zero color; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyZeroColor
        {
            get { return (bool)GetValue(ApplyZeroColorProperty); }
            set { SetValue(ApplyZeroColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the zero.
        /// </summary>
        /// <value>The color of the zero.</value>
        public Brush ZeroColor
        {
            get { return (Brush)GetValue(ZeroColorProperty); }
            set { SetValue(ZeroColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use null option].
        /// </summary>
        /// <value><c>true</c> if [use null option]; otherwise, <c>false</c>.</value>
        public bool UseNullOption
        {
            get { return (bool)GetValue(UseNullOptionProperty); }
            set { SetValue(UseNullOptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNull
        {
            get { return (bool)GetValue(IsNullProperty); }
            set { SetValue(IsNullProperty, value); }
        }

        /// <summary>
        /// Gets or sets the max validation.
        /// </summary>
        /// <value>The max validation.</value>
        public MaxValidation MaxValidation
        {
            get { return (MaxValidation)GetValue(MaxValidationProperty); }
            set { SetValue(MaxValidationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the min validation.
        /// </summary>
        /// <value>The min validation.</value>
        public MinValidation MinValidation
        {
            get { return (MinValidation)GetValue(MinValidationProperty); }
            set { SetValue(MinValidationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [max value on exceed max digit].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [max value on exceed max digit]; otherwise, <c>false</c>.
        /// </value>
        public bool MaxValueOnExceedMaxDigit
        {
            get { return (bool)GetValue(MaxValueOnExceedMaxDigitProperty); }
            set { SetValue(MaxValueOnExceedMaxDigitProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [min value on exceed min digit].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [min value on exceed min digit]; otherwise, <c>false</c>.
        /// </value>
        public bool MinValueOnExceedMinDigit
        {
            get { return (bool)GetValue(MinValueOnExceedMinDigitProperty); }
            set { SetValue(MinValueOnExceedMinDigitProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is undo enabled.
        /// </summary>
        /// <value>
        /// To remove Warnings
        /// 	<c>true</c> if this instance is undo enabled; otherwise, <c>false</c>.
        /// </value>

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

        /// <summary>
        /// Gets or sets the masked text.
        /// </summary>
        /// <value>The masked text.</value>
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

        #endregion Properties

        #region DependencyProperty

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty PositiveForegroundProperty =
            DependencyProperty.Register("PositiveForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(OnPositiveForegroundChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnPositiveForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase e = (EditorBase)obj;
            if (e != null)
            {
                e.OnPositiveForegroundChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnPositiveForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsZero && this.ApplyZeroColor)
            {
                return;
            }
            else if (this.IsNegative && this.ApplyNegativeForeground)
            {
                return;
            }
            this.Foreground = this.PositiveForeground;
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EditorForegroundProperty =
            DependencyProperty.Register("EditorForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnForegroundChanged));

        /// <summary>
        /// Identifies the NegativeForegroundProperty.
        /// </summary>
        public static readonly DependencyProperty NegativeForegroundProperty =
            DependencyProperty.Register("NegativeForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnNegativeForegroundChanged));

        /// <summary>
        /// Identifies the IsApplyNegativeColorProperty.
        /// </summary>
        public static readonly DependencyProperty ApplyNegativeForegroundProperty =
            DependencyProperty.Register("ApplyNegativeForeground", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnApplyNegativeForegroundChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnApplyNegativeForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase b = (EditorBase)obj;
            if (b != null)
                b.OnApplyNegativeForegroundChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnApplyNegativeForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
            this.SetForeground();
        }

        /// <summary>
        /// Identifies the IsNegativeProperty.
        /// </summary>
        public static readonly DependencyProperty IsNegativeProperty =
            DependencyProperty.Register("IsNegative", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnIsNegativeChanged));

        /// <summary>
        /// Identifies the IsZeroProperty.
        /// </summary>
        public static readonly DependencyProperty IsZeroProperty =
            DependencyProperty.Register("IsZero", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnIsZeroChanged));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValidationProperty =
            DependencyProperty.Register("MaxValidation", typeof(MaxValidation), typeof(EditorBase), new PropertyMetadata(MaxValidation.OnKeyPress));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValidationProperty =
            DependencyProperty.Register("MinValidation", typeof(MinValidation), typeof(EditorBase), new PropertyMetadata(MinValidation.OnKeyPress, OnMinValidationChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMinValidationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaxValueOnExceedMaxDigitProperty =
            DependencyProperty.Register("MaxValueOnExceedMaxDigit", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MinValueOnExceedMinDigitProperty =
            DependencyProperty.Register("MinValueOnExceedMinDigit", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsNullProperty =
            DependencyProperty.Register("IsNull", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, new PropertyChangedCallback(OnIsNullChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty UseNullOptionProperty =
            DependencyProperty.Register("UseNullOption", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, new PropertyChangedCallback(OnUseNullOptionChanged)));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(EditorBase), new PropertyMetadata(CultureInfo.CurrentCulture, new PropertyChangedCallback(OnCultureChanged)));

        /// <summary>
        /// Identifies the ZeroColorProperty.
        /// </summary>
        public static readonly DependencyProperty ZeroColorProperty =
            DependencyProperty.Register("ZeroColor", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush(Colors.Green), OnZeroNegativeColorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnZeroNegativeColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase b = (EditorBase)obj;
            if (b != null)
                b.OnZeroNegativeColorChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnZeroNegativeColorChanged(DependencyPropertyChangedEventArgs args)
        {
            this.SetForeground();
        }

        /// <summary>
        /// Identifies the IsApplyZeroColorProperty.
        /// </summary>
        public static readonly DependencyProperty ApplyZeroColorProperty =
            DependencyProperty.Register("ApplyZeroColor", typeof(bool), typeof(EditorBase), new PropertyMetadata(false, OnApplyZeroColorChanged));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnApplyZeroColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase b = (EditorBase)obj;
            if (b != null)
                b.OnApplyZeroColorChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnApplyZeroColorChanged(DependencyPropertyChangedEventArgs args)
        {
            this.SetForeground();
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NumberFormatProperty =
            DependencyProperty.Register("NumberFormat", typeof(NumberFormatInfo), typeof(EditorBase), new PropertyMetadata(null, new PropertyChangedCallback(OnNumberFormatChanged)));

        //To remove Warnings
#if WPF

        public new static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(EditorBase), new PropertyMetadata(true, OnIsUndoEnabledChanged));

#endif
#if SILVERLIGHT
         /// <summary>
         ///
         /// </summary>
         public static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(EditorBase), new PropertyMetadata(true, OnIsUndoEnabledChanged));
#endif

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty MaskedTextProperty =
             DependencyProperty.Register("MaskedText", typeof(string), typeof(EditorBase), new PropertyMetadata(string.Empty));

        #endregion DependencyProperty

        #region Methods

#if SILVERLIGHT
        private bool IsFocused = false;
#endif

        internal void SetForeground()
        {
            if (this.ApplyZeroColor && this.IsZero)
            {
                this.Foreground = this.ZeroColor;
            }
            else if (this.ApplyNegativeForeground && this.IsNegative)
            {
                this.Foreground = this.NegativeForeground;
            }
            else if (!this.IsNegative)
            {
                this.Foreground = this.PositiveForeground;
            }
            else
                this.Foreground = new SolidColorBrush(Colors.Black);
        }

        #endregion Methods

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
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(EditorBase), new PropertyMetadata(OnWaterMarkTemplateChanged));

        /// <summary>
        /// Called when [water mark template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnWaterMarkTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnWaterMarkTemplateChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWaterMarkTemplateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.WaterMarkTemplateChanged != null)
            {
                this.WaterMarkTemplateChanged(this, args);
            }
        }

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
        /// Identifies the WaterMarkText dependency property.
        /// </summary>
        public static DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(EditorBase), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnWaterMarkTextChanged)));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnWaterMarkTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnWaterMarkTextChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWaterMarkTextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.WaterMarkTextChanged != null)
                this.WaterMarkTextChanged(this, args);
        }

        /// <summary>
        /// Gets or sets the water mark visibility.
        /// </summary>
        /// <value>The water mark visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility WatermarkVisibility
        {
            set
            {
                SetValue(WatermarkVisibilityProperty, value);
            }
            get { return (Visibility)GetValue(WatermarkVisibilityProperty); }
        }

        private static object CoerceWatermarkVisibility(DependencyObject d, object baseValue)
        {
            EditorBase editorBase = (EditorBase)d;

            if (editorBase.WatermarkTextIsVisible && (((Visibility)baseValue) == Visibility.Visible))
            {
                editorBase.ContentElementVisibility = Visibility.Collapsed;
                return Visibility.Visible;
            }
            else
            {
                editorBase.ContentElementVisibility = Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Identifies the WaterMarkVisibility dependency property.
        /// </summary>
#if WPF

        public static DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(EditorBase), new PropertyMetadata(Visibility.Collapsed, OnWatermarkVisibilityPropertyChanged, new CoerceValueCallback(CoerceWatermarkVisibility)));

#endif
#if SILVERLIGHT
        public static DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(EditorBase), new PropertyMetadata(Visibility.Collapsed));
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnWatermarkVisibilityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase _editorbase = (EditorBase)obj;
            if (_editorbase != null)
                _editorbase.OnWatermarkVisibilityPropertyChanged(args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnWatermarkVisibilityPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            object _value = CoerceWatermarkVisibility(this, this.WatermarkVisibility);
            if (this.WatermarkVisibility != (Visibility)_value)
            {
                this.WatermarkVisibility = (Visibility)_value;
            }
#endif
        }

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
            DependencyProperty.Register("ContentElementVisibility", typeof(Visibility), typeof(EditorBase), new PropertyMetadata(Visibility.Visible));

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
            DependencyProperty.Register("WatermarkTextForeground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush()));

        /// <summary>
        ///
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush WatermarkBackground
        {
            get { return (Brush)GetValue(WatermarkBackgroundProperty); }
            set { SetValue(WatermarkBackgroundProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty WatermarkBackgroundProperty =
            DependencyProperty.Register("WatermarkBackground", typeof(Brush), typeof(EditorBase), new PropertyMetadata(new SolidColorBrush()));

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
            DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(EditorBase), new PropertyMetadata((double)0.5));

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
            DependencyProperty.Register("WatermarkTextIsVisible", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if SILVERLIGHT
            IsFocused = false;
#endif
            SetForeground();
            //SetBackground();
            base.OnLostFocus(e);
            if (this.IsNull)
            {
                this.WatermarkVisibility = Visibility.Visible;
            }
        }

#if WPF

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DoubleValueHandler.doubleValueHandler.AllowSelectionStart = false;
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
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
#if SILVERLIGHT
            IsFocused = true;
#endif
            this.SetForeground();
            base.OnGotFocus(e);
            this.WatermarkVisibility = Visibility.Collapsed;
            if (TextSelectionOnFocus)
                this.SelectAll();
        }

        #endregion WaterMark

        #region Property ChangedCallbacks

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsUndoEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase e = (EditorBase)obj;
            if (e != null)
            {
                e.OnIsUndoEnabledChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsUndoEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsUndoEnabledChanged != null)
            {
                this.IsUndoEnabledChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnNegativeForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase e = (EditorBase)obj;
            if (e != null)
            {
                e.OnNegativeForegroundChanged(args);
                e.SetForeground();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnNegativeForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
            if (NegativeForegroundChanged != null)
            {
                this.NegativeForegroundChanged(this, args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnEnterToMoveNextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase editorbase = (EditorBase)obj;
            if (editorbase != null)
            {
                editorbase.OnEnterToMoveNextChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnEnterToMoveNextChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.EnterToMoveNextChanged != null)
                this.EnterToMoveNextChanged(this, args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnForegroundChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.IsZero && this.ApplyZeroColor)
            {
                return;
            }
            else if (this.IsNegative && this.ApplyNegativeForeground)
            {
                return;
            }
        }

        public static void OnUseNullOptionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnUseNullOptionChanged(args);
            }
        }

        public virtual void OnUseNullOptionChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsNegativeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnIsNegativeChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsNegativeChanged(DependencyPropertyChangedEventArgs args)
        {
            this.SetForeground();
            if (this.IsValueNegativeChanged != null)
                this.IsValueNegativeChanged(this, args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsZeroChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnIsZeroChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsZeroChanged(DependencyPropertyChangedEventArgs args)
        {
            this.SetForeground();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsNullChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((EditorBase)obj) != null)
            {
                ((EditorBase)obj).OnIsNullChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsNullChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Called when [culture changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCultureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((EditorBase)obj != null)
            {
                ((EditorBase)obj).OnCultureChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CultureChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnCultureChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.CultureChanged != null)
                this.CultureChanged(this, args);
            this.OnCultureChanged();
        }

        /// <summary>
        /// Called when [number format changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnNumberFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((EditorBase)obj != null)
            {
                ((EditorBase)obj).OnNumberFormatChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NumberFormatChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnNumberFormatChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.NumberFormatChanged != null)
                this.NumberFormatChanged(this, args);
            this.OnNumberFormatChanged();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnTextSelectionOnFocusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            EditorBase e = (EditorBase)obj;
            if (e != null)
            {
                e.OnTextSelectionOnFocusChanged(args);
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

        #endregion Property ChangedCallbacks

        /// <summary>
        ///
        /// </summary>
        public bool IsScrollingOnCircle
        {
            get { return (bool)GetValue(IsScrollingOnCircleProperty); }
            set { SetValue(IsScrollingOnCircleProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsScrollingOnCircleProperty =
            DependencyProperty.Register("IsScrollingOnCircle", typeof(bool), typeof(EditorBase), new PropertyMetadata(true));

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
        public static readonly DependencyProperty EnterToMoveNextProperty =
            DependencyProperty.Register("EnterToMoveNext", typeof(bool), typeof(EditorBase), new PropertyMetadata(true, OnEnterToMoveNextChanged));

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
        public static readonly DependencyProperty TextSelectionOnFocusProperty =
            DependencyProperty.Register("TextSelectionOnFocus", typeof(bool), typeof(EditorBase), new PropertyMetadata(true, OnTextSelectionOnFocusChanged));

#if WPF

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
#endif
#if SILVERLIGHT
        /// <summary>
        /// Called when <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
#endif
            if (e.Key == Key.Enter)
            {
                if (this.SelectionStart + 1 <= this.MaskedText.Length)
                    this.SelectionStart = this.SelectionStart + 1;
            }
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.C)
                {
                }
                if (e.Key == Key.V)
                {
                }
                if (e.Key == Key.X)
                {
                }
            }
        }

        #region VirtualMethods

        internal virtual void OnCultureChanged()
        {
        }

        internal virtual void OnNumberFormatChanged()
        {
        }

        #endregion VirtualMethods

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
            DependencyProperty.Register("IsCaretAnimationEnabled", typeof(bool), typeof(EditorBase), new PropertyMetadata(false));

        // [Obsolete("Use SelectionStart Property")]
        //To remove Warnings
#if WPF

        internal new int CaretIndex
#endif
#if SILVERLIGHT
        internal int CaretIndex
#endif
        {
            get
            {
                return SelectionStart;
            }
            set
            {
                this.SelectionStart = value;
                SetValue(CaretIndexProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.Register("CaretIndex", typeof(int), typeof(EditorBase), new PropertyMetadata((int)0));
    }
}