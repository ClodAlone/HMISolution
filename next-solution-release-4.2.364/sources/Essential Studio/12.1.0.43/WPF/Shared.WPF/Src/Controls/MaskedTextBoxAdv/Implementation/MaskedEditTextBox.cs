#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{

#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Blend;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Default;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/MaskedTextBox.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(MaskedTextBoxAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/MaskedTextBox.xaml")]
#endif

    public class MaskedTextBoxAdv : TextBox
    {

#if WPF
        /// <summary>
        /// Gets or sets the bg background.
        /// </summary>
        /// <value>The bg background.</value>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush bgBackground
        {
            get { return (Brush)GetValue(bgBackgroundProperty); }
            set { SetValue(bgBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bgBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bgBackgroundProperty =
            DependencyProperty.Register("bgBackground", typeof(Brush), typeof(MaskedTextBoxAdv));


#endif
        /// /// <summary>
        /// Occurs when a Value of the MaskedTextBoxAdv is changed.
        /// </summary>
        public DependencyPropertyChangedEventHandler ValueChanged;

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<DateTimeProperties> mDateTimeProperties = new ObservableCollection<DateTimeProperties>();
        /// <summary>
        /// Gets or sets the date time properties.
        /// </summary>
        /// <value>The date time properties.</value>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObservableCollection<DateTimeProperties> DateTimeProperties
        {
            get { return mDateTimeProperties; }
            set { mDateTimeProperties = value; }
        }

        #region PrivateMembers
        private bool IsLoaded = false;

        #endregion

        #region Constructor

#if WPF
        static MaskedTextBoxAdv()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MaskedTextBoxAdv), new FrameworkPropertyMetadata(typeof(MaskedTextBoxAdv)));
        }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedTextBoxAdv"/> class.
        /// </summary>
        public MaskedTextBoxAdv()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(MaskedTextBoxAdv);
#endif
            base.Loaded += MaskTextBox_Loaded;
            this.SelectionChanged += new RoutedEventHandler(MaskedTextBox_SelectionChanged);

            Binding foregroundbinding = new Binding();
            foregroundbinding.Source = this;
            foregroundbinding.Mode = BindingMode.TwoWay;
            foregroundbinding.Path = new PropertyPath("Foreground");
            this.SetBinding(MaskedTextBoxAdv._ForegroundProperty, foregroundbinding);
        }

        internal bool selectionChanged = true;
        void MaskedTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.MaskType == MaskType.DateTime)
            {
                if (selectionChanged == true)
                    DateTimeHandler.dateTimeHandler.HandleSelection(this);
                else
                    selectionChanged = true;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                e.Handled = KeyHandler.keyHandler.HandleKeyDown(this);// this.HandleMouseWheelDown(this);
            else
                e.Handled = KeyHandler.keyHandler.HandleKeyUp(this);//this.HandleMouseWheelUp(this);
            base.OnMouseWheel(e);
        }

        #endregion

        #region Events
        /// <summary>
        /// Handles the Loaded event of the MaskTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MaskTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            if (IsEnableWaterMark == true)
            {
                this.WaterMarkVisibility = Visibility.Visible;
                this.ContentElementVisibility = Visibility.Collapsed;
            }
            else
            {
                this.WaterMarkVisibility = Visibility.Collapsed;
                this.ContentElementVisibility = Visibility.Visible;
            }
            this.LoadTextBox();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Loads the text box.
        /// </summary>
        internal void LoadTextBox()
        {
            if (IsEnableDropDownCalender && this.MaskType == MaskType.DateTime)
                this.DropDownButtonVisibility = Visibility.Visible;
            else
                this.DropDownButtonVisibility = Visibility.Collapsed;

            NumberFormatInfo numberFormat = this.GetCulture().NumberFormat;
            if (MaskType == MaskType.Standard)
            {
                this.mValue = this.Value ?? "";
                CharCollection = MaskEditModel.maskEditorModelHelp.CreateRegularExpression(this);
                this.MaskedText = MaskEditModel.maskEditorModelHelp.CreateDisplayText(this);
                if (Value != null)
                {
                    mValueChanged = false;
                    this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                    mValueChanged = true;
                }
            }

            #region Double
            else if (MaskType == MaskType.Double)
            {
                double preValue;
                if (this.Value == null)
                {
                    this.mValue = 0.0;
                    if (double.TryParse(this.mValue.ToString(), NumberStyles.Number, numberFormat, out preValue))
                    {
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;
                        this.mValue = preValue;
                        this.MaskedText = preValue.ToString("N", numberFormat);
                    }
                }
                else
                {
                    if (double.TryParse(this.Value.ToString(), NumberStyles.Number, numberFormat, out preValue))
                    {
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;

                        this.mValue = preValue;
                        this.mValueChanged = false;
                        this.Value = this.mValue;
                        this.mValueChanged = true;

                        this.MaskedText = preValue.ToString("N", numberFormat);
                    }
                    else
                    {
                        this.mValue = 0.0;
                        if (double.TryParse(this.mValue.ToString(), NumberStyles.Number, numberFormat, out preValue))
                        {
                            if (preValue > this.MaxValue)
                                preValue = this.MaxValue;
                            if (preValue < this.MinValue)
                                preValue = this.MinValue;

                            this.mValue = preValue;
                            this.mValueChanged = false;
                            this.Value = this.mValue;
                            this.mValueChanged = true;

                            this.MaskedText = preValue.ToString("N", numberFormat);

                        }
                    }
                }
            }
            #endregion

            #region Integer
            else if (MaskType == MaskType.Integer)
            {
                if (Value != null)
                {
                    Int64 preValue;
                    if (Int64.TryParse(this.Value.ToString(), NumberStyles.Number, numberFormat, out preValue))
                    {
                        if (preValue > this.IntegerMaxValue)
                            preValue = this.IntegerMaxValue;
                        if (preValue < this.IntegerMinValue)
                            preValue = this.IntegerMinValue;

                        numberFormat.NumberDecimalDigits = 0;
                        this.MaskedText = preValue.ToString("N", numberFormat);
                        mValueChanged = false;
                        this.Value = preValue;
                        mValueChanged = true;
                    }
                    else
                    {
                        this.mValue = (Int64)0;
                        if (Int64.TryParse(this.mValue.ToString(), NumberStyles.Number, numberFormat, out preValue))
                        {
                            if (preValue > this.IntegerMaxValue)
                                preValue = this.IntegerMaxValue;
                            if (preValue < this.IntegerMinValue)
                                preValue = this.IntegerMinValue;

                            numberFormat.NumberDecimalDigits = 0;
                            this.MaskedText = preValue.ToString("N", numberFormat);
                        }
                    }
                }
                else
                {
                    Int64 preValue;
                    this.mValue = 0;
                    if (Int64.TryParse(this.mValue.ToString(), NumberStyles.Number, numberFormat, out preValue))
                    {
                        if (preValue > this.IntegerMaxValue)
                            preValue = this.IntegerMaxValue;
                        if (preValue < this.IntegerMinValue)
                            preValue = this.IntegerMinValue;

                        numberFormat.NumberDecimalDigits = 0;
                        this.mValue = preValue;
                        this.MaskedText = preValue.ToString("N", numberFormat);
                    }
                }
            }
            #endregion

            #region Currency
            else if (MaskType == MaskType.Currency)
            {
                double preValue;
                if (this.Value == null)
                {
                    this.mValue = 0.0;
                    if (double.TryParse(this.mValue.ToString(), NumberStyles.Currency, numberFormat, out preValue))
                    {
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;
                        this.mValue = preValue;
                        this.MaskedText = preValue.ToString("C", numberFormat);
                    }
                }
                else
                {
                    if (double.TryParse(this.Value.ToString(), NumberStyles.Currency, numberFormat, out preValue))
                    {
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;

                        this.mValue = preValue;
                        this.mValueChanged = false;
                        this.Value = this.mValue;
                        this.mValueChanged = true;

                        this.MaskedText = preValue.ToString("C", numberFormat);
                    }
                    else
                    {
                        this.mValue = 0.0;
                        if (double.TryParse(this.mValue.ToString(), NumberStyles.Currency, numberFormat, out preValue))
                        {
                            if (preValue > this.MaxValue)
                                preValue = this.MaxValue;
                            if (preValue < this.MinValue)
                                preValue = this.MinValue;

                            this.mValue = preValue;
                            this.mValueChanged = false;
                            this.Value = this.mValue;
                            this.mValueChanged = true;

                            this.MaskedText = preValue.ToString("C", numberFormat);

                        }
                    }
                }
            }
            #endregion

            #region Percentage
            else if (MaskType == MaskType.Percentage)
            {
                double preValue;
                if (this.Value == null)
                {
                    this.mValue = 0.0;
                    if (double.TryParse(this.mValue.ToString(), NumberStyles.Any, numberFormat, out preValue))
                    {
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;
                        this.mValue = preValue;
                        this.MaskedText = preValue.ToString("P", numberFormat);
                    }
                }
                else
                {
                    if (double.TryParse(this.Value.ToString(), NumberStyles.Any, numberFormat, out preValue))
                    {
                        preValue = preValue / 100;
                        if (preValue > this.MaxValue)
                            preValue = this.MaxValue;
                        if (preValue < this.MinValue)
                            preValue = this.MinValue;

                        this.mValue = preValue;
                        this.mValueChanged = false;
                        this.Value = this.mValue;
                        this.mValueChanged = true;

                        this.MaskedText = preValue.ToString("P", numberFormat);
                    }
                    else
                    {
                        this.mValue = 0.0;
                        if (double.TryParse(this.mValue.ToString(), NumberStyles.Currency, numberFormat, out preValue))
                        {
                            if (preValue > this.MaxValue)
                                preValue = this.MaxValue;
                            if (preValue < this.MinValue)
                                preValue = this.MinValue;

                            this.mValue = preValue;
                            this.mValueChanged = false;
                            this.Value = this.mValue;
                            this.mValueChanged = true;

                            this.MaskedText = preValue.ToString("P", numberFormat);
                        }
                    }
                }
            }
            #endregion

            else if (MaskType == MaskType.DateTime)
            {
                DateTime preVal;
                if (Value == null)
                {
                    preVal = DateTime.Now;
                }
                else
                {
                    DateTime.TryParse(this.Value.ToString(), out preVal);
                }
                DateTimeFormatInfo datetimeFormat = this.GetCulture().DateTimeFormat as DateTimeFormatInfo;
                mValue = preVal;
                DateTimeProperties = DateTimeHandler.dateTimeHandler.CreateDateTimePatteren(this);

                this.MaskedText = preVal.ToString(DateTimeProperties[DateTimeProperties.Count - 1].Pattern, datetimeFormat);
                this.MaskedText = DateTimeHandler.dateTimeHandler.CreateDisplayText(this);
            }
            else
            {

            }
            this.EnableNegativeOptions();
        }

        internal void EnableNegativeOptions()
        {
            #region Foreground
            if (this.IsApplyNegativeColor)
            {
                if (this.MaskType == MaskType.Integer)
                {
                    Int64 preVal = (Int64)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                    if (preVal < 0)
                    {
                        IsPositiveForeground = false;
                        _Foreground = this.NegativeForeground;
                        //this.Foreground = NegativeForeground;
                    }
                    else
                        this._Foreground = this.m_Foreground;
                    //this.Foreground = this.m_Foreground;
                }
                else if (this.MaskType == MaskType.Double)
                {
                    double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);

                    if (preVal < 0)
                    {
                        IsPositiveForeground = false;
                        //this.Foreground = NegativeForeground;
                        _Foreground = this.NegativeForeground;
                    }
                    else
                        this._Foreground = this.m_Foreground;
                    //this.Foreground = this.m_Foreground;
                }
                else if (this.MaskType == MaskType.Currency)
                {
                    double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);

                    if (preVal < 0)
                    {
                        IsPositiveForeground = false;
                        //this.Foreground = NegativeForeground;
                        _Foreground = this.NegativeForeground;
                    }
                    else
                        this._Foreground = this.m_Foreground;
                    //this.Foreground = this.m_Foreground;
                }
                else if (this.MaskType == MaskType.Percentage)
                {
                    double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                    if (preVal < 0)
                    {
                        IsPositiveForeground = false;
                        //this.Foreground = NegativeForeground;
                        _Foreground = this.NegativeForeground;
                    }
                    else
                        this._Foreground = this.m_Foreground;
                    //this.Foreground = this.m_Foreground;
                }
            }
            else
            {
                //this.Foreground = this.m_Foreground;
                this._Foreground = this.m_Foreground;
            }
            #endregion
            if (this.MaskType == MaskType.Integer)
            {
                Int64 preVal = (Int64)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                if (preVal < 0)
                    this.IsNegative = true;
                else
                    this.IsNegative = false;
            }
            else if (this.MaskType == MaskType.Double)
            {
                double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                if (preVal < 0)
                    this.IsNegative = true;
                else
                    this.IsNegative = false;

            }
            else if (this.MaskType == MaskType.Currency)
            {
                double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                if (preVal < 0)
                    this.IsNegative = true;
                else
                    this.IsNegative = false;
            }
            else if (this.MaskType == MaskType.Percentage)
            {
                double preVal = (Double)MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                if (preVal < 0)
                    this.IsNegative = true;
                else
                    this.IsNegative = false;
            }

        }

        internal int mSelectedCollection;
        /// <summary>
        /// Loads the on value changed.
        /// </summary>
        internal void LoadOnValueChanged()
        {
            if (IsEnableDropDownCalender && this.MaskType == MaskType.DateTime)
                this.DropDownButtonVisibility = Visibility.Visible;
            else
                this.DropDownButtonVisibility = Visibility.Collapsed;
            if (this.MaskType == MaskType.DateTime)
            {
                DateTime preVal;
                if (Value.ToString() == "")
                    preVal = DateTime.Now;
                else
                    preVal = (DateTime)this.Value;
#if WPF
                int i = mSelectedCollection;
#endif

                this.MaskedText = DateTimeHandler.dateTimeHandler.CreateDisplayText(this);
#if WPF
                this.selectionChanged = false;
                this.Select(this.DateTimeProperties[i].StartPosition, this.DateTimeProperties[i].Lenghth);
                mSelectedCollection = i;
                this.selectionChanged = true;
#endif
#if SILVERLIGHT
                if (mSelectedCollection > -1)
                    this.Select(this.DateTimeProperties[mSelectedCollection].StartPosition, this.DateTimeProperties[mSelectedCollection].Lenghth);
#endif
            }
        }
        #endregion

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <returns></returns>
        internal CultureInfo GetCulture()
        {
            CultureInfo cultureInfo;
            if (Culture != null)
            {
                cultureInfo = this.Culture.Clone() as CultureInfo;
            }
            else
            {
                cultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;
                //cultureInfo = new CultureInfo("en-US");
            }

            if (NumberFormat != null)
            {
                cultureInfo.NumberFormat = NumberFormat;
            }

            if (DecimalDigits >= 0)
            {
                cultureInfo.NumberFormat.CurrencyDecimalDigits = DecimalDigits;
                cultureInfo.NumberFormat.PercentDecimalDigits = DecimalDigits;
                cultureInfo.NumberFormat.NumberDecimalDigits = DecimalDigits;
            }

            if (!DecimalSeparator.Equals(string.Empty))
            {
                cultureInfo.NumberFormat.CurrencyDecimalSeparator = DecimalSeparator;
                cultureInfo.NumberFormat.PercentDecimalSeparator = DecimalSeparator;
                cultureInfo.NumberFormat.NumberDecimalSeparator = DecimalSeparator;
            }
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = cultureInfo.NumberFormat.CurrencyDecimalSeparator[0].ToString();
            cultureInfo.NumberFormat.PercentDecimalSeparator = cultureInfo.NumberFormat.PercentDecimalSeparator[0].ToString();
            cultureInfo.NumberFormat.NumberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0].ToString();

            if (!GroupSeparator.Equals(string.Empty))
            {
                cultureInfo.NumberFormat.CurrencyGroupSeparator = GroupSeparator;
                cultureInfo.NumberFormat.PercentGroupSeparator = GroupSeparator;
                cultureInfo.NumberFormat.NumberGroupSeparator = GroupSeparator;
            }
            cultureInfo.NumberFormat.CurrencyGroupSeparator = cultureInfo.NumberFormat.CurrencyGroupSeparator[0].ToString();
            cultureInfo.NumberFormat.PercentGroupSeparator = cultureInfo.NumberFormat.PercentGroupSeparator[0].ToString();
            cultureInfo.NumberFormat.NumberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator[0].ToString();

            if (!CurrencySymbol.Equals(string.Empty))
            {
                cultureInfo.NumberFormat.CurrencySymbol = CurrencySymbol;
            }
            cultureInfo.NumberFormat.CurrencySymbol = cultureInfo.NumberFormat.CurrencySymbol[0].ToString();

            if (!PercentSymbol.Equals(string.Empty))
            {
                cultureInfo.NumberFormat.PercentSymbol = PercentSymbol;

            }
            cultureInfo.NumberFormat.PercentSymbol = cultureInfo.NumberFormat.PercentSymbol[0].ToString();
            //cultureInfo.NumberFormat.PercentSymbol = cultureInfo.NumberFormat.PercentSymbol[0].ToString();
            //GroupSizes
            return cultureInfo;
        }

        #region Overrides
        Popup _popUp;
        Button PART_Button;
        ContentControl PART_WaterMark;
        ScrollViewer ContentElement;
#if WPF
        Syncfusion.Windows.Controls.Calendar calender;
#else
        System.Windows.Controls.Calendar calender;
#endif

        Button Button_CurrDate;
        Button Button_NoDate;
        Grid _root;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_popUp != null)
            {
                _popUp.Child = null;
            }
            if (Button_CurrDate != null)
            {
                Button_CurrDate.Click -= new RoutedEventHandler(Button_CurrDate_Click);
            }
            if (Button_NoDate != null)
            {
                Button_NoDate.Click -= new RoutedEventHandler(Button_NoDate_Click);
            }
            if (calender != null)
            {
                calender.SelectedDatesChanged -= new EventHandler<SelectionChangedEventArgs>(calender_SelectedDatesChanged);
                calender = null;
            }

            if (PART_Button != null)
                PART_Button.Click -= new RoutedEventHandler(button_Click);

#if WPF
            calender = this.GetTemplateChild("Calender") as Syncfusion.Windows.Controls.Calendar;
#else
            calender = this.GetTemplateChild("Calender") as System.Windows.Controls.Calendar;
            calender.SizeChanged += new SizeChangedEventHandler(calender_SizeChanged);
#endif

            ContentElement = this.GetTemplateChild("ContentElement") as ScrollViewer;
            _popUp = this.GetTemplateChild("PART_Popup") as Popup;
            Button_CurrDate = this.GetTemplateChild("Button_CurrDate") as Button;
            Button_NoDate = this.GetTemplateChild("Button_NoDate") as Button;
            PART_Button = this.GetTemplateChild("PART_Button") as Button;
            _root = this.GetTemplateChild("RootElement") as Grid;
            PART_WaterMark = this.GetTemplateChild("PART_Watermark") as ContentControl;

            if (PART_Button != null)
                PART_Button.Click += new RoutedEventHandler(button_Click);

            if (calender != null)
                calender.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(calender_SelectedDatesChanged);

            if (Button_CurrDate != null)
                Button_CurrDate.Click += new RoutedEventHandler(Button_CurrDate_Click);

            if (Button_NoDate != null)
                Button_NoDate.Click += new RoutedEventHandler(Button_NoDate_Click);

#if SILVERLIGHT
            if (_outsidePopupCanvas != null)
            {
                _outsidePopupCanvas.MouseLeftButtonDown -= new MouseButtonEventHandler(_outsidePopupCanvas_MouseLeftButtonDown);
            }
            _outsideCanvas = this.GetTemplateChild("OutsideCanvas") as Canvas;
            _outsidePopupCanvas = this.GetTemplateChild("OutsidePopupCanvas") as Canvas;
            _popupGrid = this.GetTemplateChild("PART_PopupGrid") as Grid;
            if (this._outsidePopupCanvas != null)
                _outsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(_outsidePopupCanvas_MouseLeftButtonDown);
#endif
        }

        /// <summary>
        /// Handles the Click event of the Button_NoDate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Button_NoDate_Click(object sender, RoutedEventArgs e)
        {
            this.WaterMarkVisibility = Visibility.Visible;
            this.ContentElementVisibility = Visibility.Collapsed;
            this.WaterMarkText = "No date is selected";
            this.Value = null;
            this._popUp.IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the Button_CurrDate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Button_CurrDate_Click(object sender, RoutedEventArgs e)
        {
            this.Value = DateTime.Now;
            this._popUp.IsOpen = false;
        }

        /// <summary>
        /// Handles the SelectedDatesChanged event of the calender control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void calender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            mValueChanged = false;
#if WPF
            this.Value = ((Syncfusion.Windows.Controls.Calendar)sender).SelectedDate;
#endif
#if SILVERLIGHT
            this.Value = ((System.Windows.Controls.Calendar)sender).SelectedDate;
#endif
            mValueChanged = true;
            this._popUp.IsOpen = false;
        }



#if SILVERLIGHT
        Canvas _outsideCanvas;
        Canvas _outsidePopupCanvas;
        Grid _popupGrid;
        /// <summary>
        /// Handles the Click event of the button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void button_Click(object sender, RoutedEventArgs e)
        {
          FrameworkElement page = (Application.Current != null) ?
                     Application.Current.RootVisual as FrameworkElement :
                     null;

            if (page != null)
            {
                SetPopUpPosition();
                _popUp.IsOpen = !_popUp.IsOpen;
                if (_popUp.IsOpen == true)
                {
                    DateTime t;
                    DateTime.TryParse(this.mValue.ToString(), out t);
                    calender.DisplayDate = t;
                    if (this.WaterMarkVisibility == Visibility.Visible)
                    {
                        this.WaterMarkVisibility = Visibility.Collapsed;
                        this.ContentElementVisibility = Visibility.Visible;
                    }
                }
            }
        }
        void calender_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPopUpPosition();
        }

        private void SetPopUpPosition()
        {
            if (this.calender != null && Application.Current != null && Application.Current.Host != null && Application.Current.Host.Content != null)
            {
                double pageHeight = Application.Current.Host.Content.ActualHeight;
                double pageWidth = Application.Current.Host.Content.ActualWidth;
                double calendarHeight = this.calender.ActualHeight;
                double actualHeight = this.ActualHeight;

                if (this._root != null)
                {
                    GeneralTransform gt = this._root.TransformToVisual(null);

                    if (gt != null)
                    {
                        Point point00 = new Point(0, 0);
                        Point point10 = new Point(1, 0);
                        Point point01 = new Point(0, 1);
                        Point transform00 = gt.Transform(point00);
                        Point transform10 = gt.Transform(point10);
                        Point transform01 = gt.Transform(point01);

                        double dpX = transform00.X;
                        double dpY = transform00.Y;

                        double calendarX = dpX;
                        double calendarY = dpY + actualHeight;

                        // if the page height is less then the total height of
                        // the PopUp + DatePicker or if we can fit the PopUp
                        // inside the page, we want to place the PopUp to the
                        // bottom
                        if (pageHeight < calendarY + calendarHeight)
                        {
                            calendarY = dpY - calendarHeight;
                        }
                        this._popUp.HorizontalOffset = 0;
                        this._popUp.VerticalOffset = 0;
                        this._outsidePopupCanvas.Width = pageWidth;
                        this._outsidePopupCanvas.Height = pageHeight;
                        this._popUp.HorizontalAlignment = HorizontalAlignment.Left;
                        this._popUp.VerticalAlignment = VerticalAlignment.Top;
                        Canvas.SetLeft(this._popUp, calendarX - dpX);
                        Canvas.SetTop(this._popUp, calendarY - dpY);

                        // Transform the invisible canvas to plugin coordinate
                        // space origin
                        Matrix transformToRootMatrix = Matrix.Identity;
                        transformToRootMatrix.M11 = transform10.X - transform00.X;
                        transformToRootMatrix.M12 = transform10.Y - transform00.Y;
                        transformToRootMatrix.M21 = transform01.X - transform00.X;
                        transformToRootMatrix.M22 = transform01.Y - transform00.Y;
                        transformToRootMatrix.OffsetX = transform00.X;
                        transformToRootMatrix.OffsetY = transform00.Y;
                        MatrixTransform mt = new MatrixTransform();
                        InvertMatrix(ref transformToRootMatrix);
                        mt.Matrix = transformToRootMatrix;
                        this._outsidePopupCanvas.RenderTransform = mt;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the matrix to its inverse.
        /// </summary>
        /// <param name="matrix">Matrix to be inverted.</param>
        /// <returns>
        /// True if the Matrix is invertible, false otherwise.
        /// </returns>
        private static bool InvertMatrix(ref Matrix matrix)
        {
            double determinant = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;

            if (determinant == 0.0)
            {
                return false;
            }

            Matrix matCopy = matrix;
            matrix.M11 = matCopy.M22 / determinant;
            matrix.M12 = -1 * matCopy.M12 / determinant;
            matrix.M21 = -1 * matCopy.M21 / determinant;
            matrix.M22 = matCopy.M11 / determinant;
            matrix.OffsetX = (matCopy.OffsetY * matCopy.M21 - matCopy.OffsetX * matCopy.M22) / determinant;
            matrix.OffsetY = (matCopy.OffsetX * matCopy.M12 - matCopy.OffsetY * matCopy.M11) / determinant;

            return true;
        }

        void _outsidePopupCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this._popUp.IsOpen = false;
        }
#endif

#if WPF
        void button_Click(object sender, RoutedEventArgs e)
        {
            _popUp.IsOpen = !_popUp.IsOpen;
            if (_popUp.IsOpen == true)
            {
                DateTime t;
                DateTime.TryParse(this.mValue.ToString(), out t);
                calender.DisplayDate = t;
                if (this.WaterMarkVisibility == Visibility.Visible)
                {
                    this.WaterMarkVisibility = Visibility.Collapsed;
                    this.ContentElementVisibility = Visibility.Visible;
                }
            }
        }
#endif

#if WPF
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.V)
                {
                    e.Handled = true;
                }
                if (e.Key == Key.C)
                {
                    System.Windows.Clipboard.SetText(this.mValue.ToString());
                    e.Handled = true;
                }
                if (e.Key == Key.Z)
                {
                    this.Value = this.mOldValue;
                    e.Handled = true;
                }
            }
            else
            {
                if (MaskType == MaskType.Standard)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Double)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Integer)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Currency)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Percentage)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.DateTime)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                }
            }
            this.EnableNegativeOptions();
            base.OnPreviewKeyDown(e);
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Called when <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ModifierKeys.Control == Keyboard.Modifiers)
            {
                if (e.Key == Key.V)
                {
                    e.Handled = true;
                }
                if (e.Key == Key.C)
                {
                    System.Windows.Clipboard.SetText(this.mValue.ToString());
                    e.Handled = true;
                }
                if (e.Key == Key.Z)
                {
                    this.Value = this.mOldValue;
                    e.Handled = true;
                }
            }
            else
            {
                if (MaskType == MaskType.Standard)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Double)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Integer)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Currency)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.Percentage)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                    if (e.Handled == true)
                    {
                        mValueChanged = false;
                        this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                        mValueChanged = true;
                    }
                }
                else if (MaskType == MaskType.DateTime)
                {
                    e.Handled = KeyHandler.keyHandler.HandleKeyDown(this, e);
                }
            }
            this.EnableNegativeOptions();
            base.OnKeyDown(e);
        }
#endif

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.KeyUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if WPF
            if (this.IsEnableWaterMark && this.Value == null)
            {
                this.WaterMarkVisibility = Visibility.Visible;
                this.ContentElementVisibility = Visibility.Collapsed;
            }
#endif
            base.OnLostFocus(e);
#if SILVERLIGHT
            if (this.IsEnableWaterMark && this.Value == null)
            {
                this.WaterMarkVisibility = Visibility.Visible;
                this.ContentElementVisibility = Visibility.Collapsed;
            }
#endif
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

#if WPF
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.ContentElementVisibility = Visibility.Visible;
            this.WaterMarkVisibility = Visibility.Collapsed;
            this.SelectionStart = 0;
            base.OnPreviewMouseLeftButtonDown(e);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.ContentElementVisibility = Visibility.Visible;
            this.WaterMarkVisibility = Visibility.Collapsed;
            this.SelectionStart = 0;
            base.OnMouseLeftButtonDown(e);
        }
#endif
        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInput"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (MaskType == MaskType.Standard)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                mValueChanged = false;
                this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                mValueChanged = true;
            }

            else if (MaskType == MaskType.Integer)//if (Regex.IsMatch(e.Text, "[\\d]*"))
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                mValueChanged = false;
                this.Value = (MaskEditModel.maskEditorModelHelp.CreateValueFromText(this));
                mValueChanged = true;
            }

            else if (MaskType == MaskType.Double)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                mValueChanged = false;
                this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                mValueChanged = true;
            }
            else if (MaskType == MaskType.Currency)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                mValueChanged = false;
                this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                mValueChanged = true;
            }
            else if (MaskType == MaskType.Percentage)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
                mValueChanged = false;
                this.Value = MaskEditModel.maskEditorModelHelp.CreateValueFromText(this);
                mValueChanged = true;
            }
            else if (MaskType == MaskType.DateTime)
            {
                e.Handled = MaskHandler.maskHandler.MatchWithMask(this, e.Text);
            }
            this.EnableNegativeOptions();
            base.OnTextInput(e);
        }

#if SILVERLIGHT
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInputStart"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInputStart(TextCompositionEventArgs e)
        {
            //e.Handled = true;
            base.OnTextInputStart(e);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.TextInputUpdate"/> event occurs.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnTextInputUpdate(TextCompositionEventArgs e)
        {
            base.OnTextInputUpdate(e);
        }
#endif
        #endregion

        #region Properties

        //public DateTimeFormatInfo DateTimeFormat
        //{
        //    get { return (DateTimeFormatInfo) GetValue(DateTimeFormatProperty); }
        //    set { SetValue(DateTimeFormatProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the number format.
        /// </summary>
        /// <value>The number format.</value>
        [Category("Number Mask")]
        public NumberFormatInfo NumberFormat
        {
            get { return (NumberFormatInfo)GetValue(NumberFormatProperty); }
            set { SetValue(NumberFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the integer max value.
        /// </summary>
        /// <value>The integer max value.</value>
        [Category("Number Mask")]
        public int IntegerMaxValue
        {
            set { SetValue(IntegerMaxValueProperty, value); }
            get { return (int)GetValue(IntegerMaxValueProperty); }
        }

        /// <summary>
        /// Gets or sets the integer min value.
        /// </summary>
        /// <value>The integer min value.</value>
        [Category("Number Mask")]
        public int IntegerMinValue
        {
            set { SetValue(IntegerMinValueProperty, value); }
            get { return (int)GetValue(IntegerMinValueProperty); }
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        [Category("Number Mask")]
        public double MaxValue
        {
            set { SetValue(MaxValueProperty, value); }
            get { return (double)GetValue(MaxValueProperty); }
        }

        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        [Category("Number Mask")]
        public double MinValue
        {
            set { SetValue(MinValueProperty, value); }
            get { return (double)GetValue(MinValueProperty); }
        }

        /// <summary>
        /// Gets or sets the type of the mask.
        /// </summary>
        /// <value>The type of the mask.</value>
        [Category("MaskedTextBoxAdv Properties")]
        public MaskType MaskType
        {
            set { SetValue(MaskTypeProperty, value); }
            get { return (MaskType)GetValue(MaskTypeProperty); }
        }

        /// <summary>
        /// Gets or sets the masked text.
        /// </summary>
        /// <value>The masked text.</value>
        [Category("Standard Mask")]
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
        /// Gets or sets the prompt char.
        /// </summary>
        /// <value>The prompt char.</value>
        [Category("Standard Mask")]
        public char PromptChar
        {
            get { return (char)GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>The culture.</value>
        [Category("MaskedTextBoxAdv Properties")]
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>The mask.</value>
        [Category("Standard Mask")]
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Category("MaskedTextBoxAdv Properties")]
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        internal object mValue;
        internal object mOldValue;

        private ObservableCollection<CharacterProperties> mCharCollection = new ObservableCollection<CharacterProperties>();
        internal ObservableCollection<CharacterProperties> CharCollection
        {
            get { return mCharCollection; }
            set { mCharCollection = value; }
        }

        /// <summary>
        /// Gets or sets the decimal digits.
        /// </summary>
        /// <value>The decimal digits.</value>
        [Category("Number Mask")]
        public int DecimalDigits
        {
            set { SetValue(DecimalDigitsProperty, value); }
            get { return (int)GetValue(DecimalDigitsProperty); }
        }

        /// <summary>
        /// Gets or sets the decimal separator.
        /// </summary>
        /// <value>The decimal separator.</value>
        [Category("Number Mask")]
        public string DecimalSeparator
        {
            get { return (string)GetValue(DecimalSeparatorProperty); }
            set { SetValue(DecimalSeparatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the group separator.
        /// </summary>
        /// <value>The group separator.</value>
        [Category("Number Mask")]
        public string GroupSeparator
        {
            get { return (string)GetValue(GroupSeparatorProperty); }
            set { SetValue(GroupSeparatorProperty, value); }
        }

        private int[] _groupSizes;
        /// <summary>
        /// Gets or sets the group sizes.
        /// </summary>
        /// <value>The group sizes.</value>
        [Category("Number Mask")]
        public int[] GroupSizes
        {
            set { _groupSizes = value; }
            get { return _groupSizes; }
        }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        /// <value>The currency symbol.</value>
        [Category("Number Mask")]
        public string CurrencySymbol
        {
            get { return (string)GetValue(CurrencySymbolProperty); }
            set { SetValue(CurrencySymbolProperty, value); }
        }

        /// <summary>
        /// Gets or sets the percent symbol.
        /// </summary>
        /// <value>The percent symbol.</value>
        [Category("Number Mask")]
        public string PercentSymbol
        {
            get { return (string)GetValue(PercentSymbolProperty); }
            set { SetValue(PercentSymbolProperty, value); }
        }
        #endregion

        #region Dependency

        //public static DependencyProperty DateTimeFormatProperty =
        //    DependencyProperty.Register("DateTimeFormat", typeof (DateTimeFormatInfo), typeof (MaskedTextBoxAdv),
        //                                new PropertyMetadata(null, new PropertyChangedCallback(OnDateTimeFormatChanged)));

        /// <summary>
        /// Identifies the NumberFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberFormatProperty =
            DependencyProperty.Register("NumberFormat", typeof(NumberFormatInfo), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(null, new PropertyChangedCallback(OnNumberFormatChanged)));

        /// <summary>
        /// Identifies the IntegerMaxValue dependency property.
        /// </summary>
        public static readonly DependencyProperty IntegerMaxValueProperty =
            DependencyProperty.Register("IntegerMaxValue", typeof(int), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(int.MaxValue,
                                                             new PropertyChangedCallback(OnIntegerMaxValueChanged)));
        /// <summary>
        /// Identifies the IntegerMinValue dependency property.
        /// </summary>
        public static readonly DependencyProperty IntegerMinValueProperty =
            DependencyProperty.Register("IntegerMinValue", typeof(int), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(int.MinValue,
                                                             new PropertyChangedCallback(OnIntegerMinValueChanged)));
        /// <summary>
        /// Identifies the MinValue dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(double.MinValue,
                                                             new PropertyChangedCallback(OnMinValueChanged)));
        /// <summary>
        /// Identifies the MaxValue dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(double.MaxValue,
                                                             new PropertyChangedCallback(OnMaxValueChanged)));
        /// <summary>
        /// Identifies the MaskType dependency property.
        /// </summary>
        public static readonly DependencyProperty MaskTypeProperty =
            DependencyProperty.Register("MaskType", typeof(MaskType), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(MaskType.None,
                                                             new PropertyChangedCallback(OnMaskTypeChanged)));
        /// <summary>
        /// Identifies the MaskedText dependency property.
        /// </summary>
        public static readonly DependencyProperty MaskedTextProperty =
            DependencyProperty.Register("MaskedText", typeof(string), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(string.Empty));
        /// <summary>
        /// Identifies the PromptChar dependency property.
        /// </summary>
        public static readonly DependencyProperty PromptCharProperty =
            DependencyProperty.Register("PromptChar", typeof(char), typeof(MaskedTextBoxAdv), new PropertyMetadata('_'));
        /// <summary>
        /// Identifies the Culture dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(CultureInfo.CurrentCulture,
                                                             new PropertyChangedCallback(OnCultureChanged)));
        /// <summary>
        /// Identifies the Mask dependency property.
        /// </summary>
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnMaskChanged)));
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));
        /// <summary>
        /// Identifies the DecimalDigits dependency property.
        /// </summary>
        public static DependencyProperty DecimalDigitsProperty =
            DependencyProperty.Register("DecimalDigits", typeof(int), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(-1, new PropertyChangedCallback(OnDecimalDigitsChanged)));
        /// <summary>
        /// Identifies the DecimalSeparator dependency property.
        /// </summary>
        public static DependencyProperty DecimalSeparatorProperty =
            DependencyProperty.Register("DecimalSeparator", typeof(string), typeof(MaskedTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnDecimalSeparatorChanged)));
        /// <summary>
        /// Identifies the GroupSeparator dependency property.
        /// </summary>
        public static DependencyProperty GroupSeparatorProperty =
            DependencyProperty.Register("GroupSeparator", typeof(string), typeof(MaskedTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnGroupSeparatorChanged)));
        /// <summary>
        /// Identifies the CurrencySymbol dependency property.
        /// </summary>
        public static DependencyProperty CurrencySymbolProperty =
            DependencyProperty.Register("CurrencySymbol", typeof(string), typeof(MaskedTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnCurrencySymbolChanged)));

        /// <summary>
        /// Identifies the PercentSymbol dependency property.
        /// </summary>
        public static DependencyProperty PercentSymbolProperty =
            DependencyProperty.Register("PercentSymbol", typeof(string), typeof(MaskedTextBoxAdv), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPercentSymbolChanged)));
        #endregion

        #region Callbacks

        /// <summary>
        /// Called when [date time format changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDateTimeFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnDateTimeFormatChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DateTimeFormatChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDateTimeFormatChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [number format changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnNumberFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnNumberFormatChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NumberFormatChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnNumberFormatChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [integer max value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIntegerMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnIntegerMaxValueChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IntegerMaxValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIntegerMaxValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded && this.MaskType == MaskType.Integer)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [integer min value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIntegerMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnIntegerMinValueChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IntegerMinValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnIntegerMinValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded && this.MaskType == MaskType.Integer)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [max value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMaxValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnMaxValueChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MaxValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnMaxValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded && this.MaskType == MaskType.Integer)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [min value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMinValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnMinValueChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MinValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnMinValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded && (this.MaskType == MaskType.Currency || this.MaskType == MaskType.Double || this.MaskType == MaskType.Currency || this.MaskType == MaskType.Percentage))
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [mask type changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMaskTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnMaskTypeChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MaskTypeChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnMaskTypeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [culture changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCultureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnCultureChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CultureChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnCultureChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                this.LoadTextBox();
            }
        }

        /// <summary>
        /// Called when [mask changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMaskChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
            {
                ((MaskedTextBoxAdv)obj).OnMaskChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MaskChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnMaskChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                this.LoadTextBox();
                //this.CharCollection = MaskEditModel.maskEditorModelHelp.CreateRegularExpression(this);
            }
        }

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnValueChanged(args);
        }

        internal bool mValueChanged = true;
        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsLoaded)
            {
                mValue = Value;
                if (mValueChanged)
                {
                    mOldValue = args.OldValue;
                    if (this.ValueChanged != null)
                        this.ValueChanged(this, args);
                    mValue = Value;
                    this.LoadTextBox();

                }
                else
                {
                    if (this.ValueChanged != null)
                        this.ValueChanged(this, args);

                    if (this.MaskType == MaskType.DateTime)
                    {
                        mOldValue = args.OldValue;
                        mValue = Value;
                        this.LoadOnValueChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Called when [decimal digits changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDecimalDigitsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnDecimalDigitsChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:DecimalDigitsChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDecimalDigitsChanged(DependencyPropertyChangedEventArgs args) { }

        /// <summary>
        /// Called when [decimal separator changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnDecimalSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnDecimalSeparatorChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:DecimalSeparatorChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnDecimalSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Called when [group separator changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnGroupSeparatorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnGroupSeparatorChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:GroupSeparatorChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnGroupSeparatorChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Called when [currency symbol changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnCurrencySymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnCurrencySymbolChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:CurrencySymbolChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnCurrencySymbolChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Called when [percent symbol changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnPercentSymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((MaskedTextBoxAdv)obj != null)
                ((MaskedTextBoxAdv)obj).OnPercentSymbolChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:PercentSymbolChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPercentSymbolChanged(DependencyPropertyChangedEventArgs args)
        {
        }
        #endregion


        /// <summary>
        /// Gets or sets the date time format.
        /// </summary>
        /// <value>The date time format.</value>
        [Category("DateTime Mask")]
        public string DateTimeFormat
        {
            get { return (string)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }
        /// <summary>
        /// Identifies the DateTimeFormat dependency property.
        /// </summary>
        public static DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnDateTimeFormatChanged)));


        /// <summary>
        /// Gets or sets the drop down button visibility.
        /// </summary>
        /// <value>The drop down button visibility.</value>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility DropDownButtonVisibility
        {
            get { return (Visibility)GetValue(DropDownButtonVisibilityProperty); }
            set { SetValue(DropDownButtonVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the DropDownButtonVisibility dependency property.
        /// </summary>
        internal static DependencyProperty DropDownButtonVisibilityProperty =
            DependencyProperty.Register("DropDownButtonVisibility", typeof(Visibility), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable drop down calender.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enable drop down calender; otherwise, <c>false</c>.
        /// </value>
        [Category("DateTime Mask")]
        public bool IsEnableDropDownCalender
        {
            set { SetValue(IsEnableDropDownCalenderProperty, value); }
            get { return (bool)GetValue(IsEnableDropDownCalenderProperty); }
        }

        /// <summary>
        /// Identifies the IsEnableDropDownCalenderProperty.
        /// </summary>
        public static DependencyProperty IsEnableDropDownCalenderProperty =
            DependencyProperty.Register("IsEnableDropDownCalender", typeof(bool), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the water mark template.
        /// </summary>
        /// <value>The water mark template.</value>
        [Category("MaskedTextBoxAdv Properties")]
        public DataTemplate WaterMarkTemplate
        {
            get { return (DataTemplate)GetValue(WaterMarkTemplateProperty); }
            set { SetValue(WaterMarkTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the WaterMarkTemplate dependency property.
        /// </summary>
        public static DependencyProperty WaterMarkTemplateProperty =
            DependencyProperty.Register("WaterMarkTemplate", typeof(DataTemplate), typeof(MaskedTextBoxAdv), new PropertyMetadata(OnWaterMarkTemplateChanged));

        /// <summary>
        /// Called when [water mark template changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnWaterMarkTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Gets or sets the water mark text.
        /// </summary>
        /// <value>The water mark text.</value>
        [Category("MaskedTextBoxAdv Properties")]
        public string WaterMarkText
        {
            set { SetValue(WaterMarkTextProperty, value); }
            get { return (string)GetValue(WaterMarkTextProperty); }
        }
        /// <summary>
        /// Identifies the WaterMarkText dependency property.
        /// </summary>
        public static DependencyProperty WaterMarkTextProperty =
            DependencyProperty.Register("WaterMarkText", typeof(string), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the water mark visibility.
        /// </summary>
        /// <value>The water mark visibility.</value>
        [BrowsableAttribute(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Visibility WaterMarkVisibility
        {
            set { SetValue(WaterMarkVisibilityProperty, value); }
            get { return (Visibility)GetValue(WaterMarkVisibilityProperty); }
        }
        /// <summary>
        /// Identifies the WaterMarkVisibility dependency property.
        /// </summary>
        public static DependencyProperty WaterMarkVisibilityProperty =
            DependencyProperty.Register("WaterMarkVisibility", typeof(Visibility), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the content element visibility.
        /// </summary>
        /// <value>The content element visibility.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Visibility ContentElementVisibility
        {
            set { SetValue(ContentElementVisibilityProperty, value); }
            get { return (Visibility)GetValue(ContentElementVisibilityProperty); }
        }
        /// <summary>
        /// Identifies the ContentElementVisibility dependency property.
        /// </summary>
        public static DependencyProperty ContentElementVisibilityProperty =
            DependencyProperty.Register("ContentElementVisibility", typeof(Visibility), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable water mark.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enable water mark; otherwise, <c>false</c>.
        /// </value>
        [Category("MaskedTextBoxAdv Properties")]
        public bool IsEnableWaterMark
        {
            set { SetValue(IsEnableWaterMarkProperty, value); }
            get { return (bool)GetValue(IsEnableWaterMarkProperty); }
        }
        /// <summary>
        /// Identifies the IsEnableWaterMark dependency property.
        /// </summary>
        public static DependencyProperty IsEnableWaterMarkProperty =
            DependencyProperty.Register("IsEnableWaterMark", typeof(bool), typeof(MaskedTextBoxAdv),
                                        new PropertyMetadata(false));

        /// <summary>
        /// Contains the Positive value of Foreground
        /// </summary>
        private Brush m_Foreground;
        /// <summary>
        /// 
        /// </summary>
        private bool IsPositiveForeground = true;
        /// <summary>
        /// Gets or sets the _ foreground.
        /// </summary>
        /// <value>The _ foreground.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush _Foreground
        {
            get { return (Brush)GetValue(_ForegroundProperty); }
            set
            {
                SetValue(_ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the m foreground.
        /// </summary>
        /// <value>The m foreground.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public Brush mForeground
        {
            get { return (Brush)GetValue(mForegroundProperty); }
            set { SetValue(mForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the mForegroundProperty Property.
        /// </summary>
        public static readonly DependencyProperty mForegroundProperty =
            DependencyProperty.Register("mForeground", typeof(Brush), typeof(MaskedTextBoxAdv), new PropertyMetadata(new PropertyChangedCallback(OnmForegroundChanged)));

        /// <summary>
        /// Called when [foreground changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnmForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var sender = (MaskedTextBoxAdv)obj;
            if (sender != null)
            {
                if (sender.IsNegative && (sender.MaskType == MaskType.Currency || sender.MaskType == MaskType.Double || sender.MaskType == MaskType.Integer ||
                    sender.MaskType == MaskType.Percentage))
                {
                    sender.m_Foreground = args.NewValue as Brush;
                }
                else
                {
                    sender.Foreground = args.NewValue as Brush;
                }
            }
        }

        /// <summary>
        /// Identifies the _ForegroundProperty.
        /// </summary>
        public static readonly DependencyProperty _ForegroundProperty =
            DependencyProperty.Register("_Foreground", typeof(Brush), typeof(MaskedTextBoxAdv), new PropertyMetadata(new PropertyChangedCallback(OnForegroundChanged)));

        /// <summary>
        /// Called when [foreground changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MaskedTextBoxAdv textbox = (MaskedTextBoxAdv)obj;
            if (textbox != null)
            {
                textbox.OnForegroundChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ForegroundChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnForegroundChanged(DependencyPropertyChangedEventArgs args)
        {
            if (IsPositiveForeground)
            {
                m_Foreground = _Foreground;
            }
            IsPositiveForeground = true;
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
        /// Identifies the NegativeForegroundProperty.
        /// </summary>
        public static readonly DependencyProperty NegativeForegroundProperty =
            DependencyProperty.Register("NegativeForeground", typeof(Brush), typeof(MaskedTextBoxAdv), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enable negative.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enable negative; otherwise, <c>false</c>.
        /// </value>
        public bool IsApplyNegativeColor
        {
            get { return (bool)GetValue(IsApplyNegativeColorProperty); }
            set { SetValue(IsApplyNegativeColorProperty, value); }
        }

        /// <summary>
        /// Identifies the IsApplyNegativeColorProperty.
        /// </summary>
        public static readonly DependencyProperty IsApplyNegativeColorProperty =
            DependencyProperty.Register("IsApplyNegativeColor", typeof(bool), typeof(MaskedTextBoxAdv), new PropertyMetadata(true));

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
        /// Identifies the IsNegativeProperty.
        /// </summary>
        public static readonly DependencyProperty IsNegativeProperty =
            DependencyProperty.Register("IsNegative", typeof(bool), typeof(MaskedTextBoxAdv), new PropertyMetadata(false));

        
    }

}
