#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

/// <summary>
/// Main class for the DateTime Edit Control.
/// </summary>
namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using Syncfusion.Windows.Shared;
    

    /// <summary>
    /// DateTime Edit Control is setting Date Time with different culture and
    /// properties.
    /// </summary>
    /// <remarks>
    /// DateTime Edit Control is used to set various
    /// <para></para>
    /// <para>Date Time format with various effects.</para>
    /// <para></para>
    /// <para><img src="DateTimeEdit1.png"/></para>
    /// </remarks>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Blend;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2007Black;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Default;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2003;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2010Black;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/DateTimeEdit.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(DateTimeEdit), XamlResource = "/Syncfusion.Theming.Windows7;component/DateTimeEdit.xaml")]
    public class DateTimeEdit : Control
    {
        #region - Public DP -
        /// <summary>
        /// Identifies the CaretBackground dependency property
        /// </summary>
        public static readonly DependencyProperty CaretBackgroundProperty = DependencyProperty.Register("CaretBackground", typeof(Color), typeof(DateTimeEdit), new PropertyMetadata(Colors.Gray, new PropertyChangedCallback(OnCaretBackgroundChanged)));

        /// <summary>
        /// Identifies the CaretCornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CaretCornerRadiusProperty = DependencyProperty.Register("CaretCornerRadius", typeof(double), typeof(DateTimeEdit), new PropertyMetadata(13.0, new PropertyChangedCallback(OnCaretCornerRadiusChanged)));

        /// <summary>
        /// Identifies the Culture dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(DateTimeEdit), new PropertyMetadata(CultureInfo.CurrentCulture, new PropertyChangedCallback(OnCultureChanged)));

        /// <summary>
        /// Identifies the CustomFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomFormatProperty = DependencyProperty.Register("CustomFormat", typeof(string), typeof(DateTimeEdit), new PropertyMetadata("dd-MMMM-yyyy hh:mm:ss.ff tt", new PropertyChangedCallback(OnCustomFormatChanged)));

        /// <summary>
        /// Identifies the FontSize dependency property.
        /// </summary>
        public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(DateTimeEdit), new PropertyMetadata(14.0, new PropertyChangedCallback(OnFontSizeChanged)));

        /// <summary>
        /// Identifies the HorizontalContentAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(HorizontalAlignment), typeof(DateTimeEdit), new PropertyMetadata(HorizontalAlignment.Left, new PropertyChangedCallback(OnTextAlignmentChanged)));

        /// <summary>
        /// Identifies the IncorrectForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty IncorrectForegroundProperty = DependencyProperty.Register("IncorrectForeground", typeof(Brush), typeof(DateTimeEdit), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnIncorrectForegroundChanged)));

        /// <summary>
        /// Identifies the IsEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(DateTimeEdit), new PropertyMetadata(true, new PropertyChangedCallback(OnIsEditableChanged)));

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(DateTimeEdit), new PropertyMetadata(true, new PropertyChangedCallback(OnIsReadOnlyChanged)));

        /// <summary>
        /// Identifies the MonthValueBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthValueBackgroundProperty = DependencyProperty.Register("MonthValueBackground", typeof(Brush), typeof(DateTimeEdit), new PropertyMetadata(new SolidColorBrush(Colors.Yellow), new PropertyChangedCallback(OnMonthValueBackgroundChanged)));

        /// <summary>
        /// Identifies the NonDate dependency property.
        /// </summary>
        public static readonly DependencyProperty NonDateProperty = DependencyProperty.Register("NonDate", typeof(string), typeof(DateTimeEdit), new PropertyMetadata("No Date is Selected", new PropertyChangedCallback(OnNonDateChanged)));

        /// <summary>
        /// Identifies the Pattern dependency property.
        /// </summary>
        public static readonly DependencyProperty PatternProperty = DependencyProperty.Register("Pattern", typeof(string), typeof(DateTimeEdit), new PropertyMetadata("dd-MMMM-yyyy hh:mm:ss.ff tt", new PropertyChangedCallback(OnPatternChanged)));

        /// <summary>
        /// Identifies the PopUpDelay dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpDelayProperty = DependencyProperty.Register("PopUpDelay", typeof(double), typeof(DateTimeEdit), new PropertyMetadata(300.00, new PropertyChangedCallback(OnPopUpDelayChanged)));

        /// <summary>
        /// Identifies the ShowEmptyDate dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowEmptyDateProperty = DependencyProperty.Register("ShowEmptyDate", typeof(bool), typeof(DateTimeEdit), new PropertyMetadata(true, new PropertyChangedCallback(OnShowEmptyDateChanged)));

        /// <summary>
        /// Identifies the StepValue dependency property.
        /// </summary>
        public static readonly DependencyProperty StepValueProperty = DependencyProperty.Register("StepValue", typeof(int), typeof(DateTimeEdit), new PropertyMetadata(1, new PropertyChangedCallback(OnStepValueChanged)));

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime), typeof(DateTimeEdit), new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnValueChanged)));

        #endregion

        #region - Private Members -
        private char[] strTempPattern;

        /// <summary>
        /// Used to select Date with Calendar format.
        /// </summary>
        /// <remarks>
        /// The day can select using different month and year. 
        /// <para>The changing of Month and Year is very easy to change.</para>
        /// </remarks>
        /// <returns>
        /// Calendar
        /// </returns>
        private CalendarControl popUpCalendar = new CalendarControl();

        /// <summary>
        /// This canvas is used to display the highlight.
        /// </summary>
        private Canvas csDT;

        /// <summary>
        /// This popup is used to display the PopUp for Showing Calendar, None as Link.
        /// </summary>
        private Popup cPopUp;

        /// <summary>
        /// Determines whether the control is focused or not
        /// </summary>
        private bool isFocused = false;

        /// <summary>
        /// This canvas is used to display the PopUp for Calendar Control.
        /// </summary>
        private Popup cCalendarControl;

        /// <summary>
        /// This border is used to set outer border for DateTime Edit Control.
        /// </summary>
        private Border brdrDateTime;

        /// <summary>
        /// This border is used to show the highlight as cursor.
        /// </summary>
        private Border bdHighlight;

        /// <summary>
        /// This is used to show the border for PopUp.
        /// </summary>
        private Border bPopUp;

        /// <summary>
        /// This is used to check whether moving new block.
        /// </summary>
        private bool bNewNlock;

        /// <summary>
        /// This is used to check whether new character is entered on DateTime Edit Control.
        /// </summary>
        private bool biCharEntered;

        /// <summary>
        /// Used to set as flag for Mouse Scroll is scrolled or not.
        /// </summary>
        private bool scroll = false;

        /// <summary>
        /// Whether None option is clicked from PopUp.
        /// </summary>
        private bool noneClicked = false;

        /// <summary>
        /// This is used to affect when up button clicked.
        /// </summary>
        private ToggleButton btnUparrow;

        /// <summary>
        /// This is used to affect when down button clicked.
        /// </summary>
        private ToggleButton btnDownArrow;

        /// <summary>
        /// This is used to store DateTime Values Temporary.
        /// </summary>
        private string strDateTime;

        /// <summary>
        /// This Timer Used to close popup
        /// </summary>
        private DispatcherTimer closePopup;

        /// <summary>
        /// This is used to set which Button is clicked, whether &quot;Up&quot; or
        /// &quot;Down&quot; button.
        /// </summary>
        private string buttonClicked = string.Empty;

        /// <summary>
        /// This is used to set previous value.
        /// </summary>
        private string sBeforeTBk = string.Empty;

        /// <summary>
        /// This is used to set Pattern as Temporary.
        /// </summary>
        private string strPattern;

        /// <summary>
        /// To check with the entered string, this variable is used.
        /// </summary>
        private string strTmpCombine = string.Empty;

        /// <summary>
        /// To check and pattern is "dd" pattern.
        /// </summary>
        private string strDD = string.Empty;

        /// <summary>
        /// The Animation for the Cursor Highlighting
        /// </summary>
        private ColorAnimation caAnimation;

        /// <summary>
        /// This is used to give timer for delay the PopUp.
        /// </summary>
        private DispatcherTimer dptPopUpTimer = new DispatcherTimer();

        /// <summary>
        /// This variable sets the Final value to set in DateTime Edit Control.
        /// </summary>
        private DateTime dt = DateTime.Now;

        /// <summary>
        /// This variable is used to set Pattern date.
        /// </summary>
        private DateTime dtPat;

        /// <summary>
        /// Before we entered or changed value of DateTime Edit, value maintain in this
        /// variable.
        /// </summary>
        private DateTime dtBeforeTBk;

        /// <summary>
        /// Getting the mouse position on which we clicked in DateTime Edit Control.
        /// </summary>
        private double eGetPosition;

        /// <summary>
        /// Stores which character now highlighting in a textblock.
        /// </summary>
        private double iSingChar = 0;

        /// <summary>
        /// Stores character position temporary.
        /// </summary>
        private double iTmpSingChar = 0;

        /// <summary>
        /// Stores total number of characters in a selected block.
        /// </summary>
        private double iChara;

        private string maxminLimitDisplay="Reached scroll Limit";

        /// <summary>
        /// This variable is used to set the textblock with inside function.
        /// </summary>
        private double tbWidth;

        /// <summary>
        /// DateTime Edit Control contains grid to show date and buttons.
        /// </summary>
        private Grid grdDateTimeEdit;

        /// <summary>
        /// This link is used to show the calendar control.
        /// </summary>
        private HyperlinkButton hlbCalendar;

        /// <summary>
        /// This link is used to show the none.
        /// </summary>
        private HyperlinkButton hlbNone;

        /// <summary>
        /// This value is set for new size format of DateTime Edit Control.
        /// </summary>
        private int iNewSizeFormat = 1;

        /// <summary>
        /// Currently selected character index position will be maintained in this variable.
        /// </summary>
        private int iSelectedChar;

        /// <summary>
        /// Currently selected textblock index position will be maintained in this variable.
        /// </summary>
        private int iChildrenIndex;

        /// <summary>
        /// Stores which block of textbox is focusing, and checks with iChildrenIndex.
        /// </summary>
        private int iCIndex;

        /// <summary>
        /// Stores numeric value whether dd pattern is selected.
        /// </summary>
        private int iDD = 0;

        /// <summary>
        /// Stores numeric value whether hh pattern is selected.
        /// </summary>
        private int iHH = 0;

        /// <summary>
        /// This rectangle shape is used for highlight.
        /// </summary>
        private Storyboard sbRectangle;

        /// <summary>
        /// This is used to set the DateTime Edit Control inside 
        /// </summary>
        private StackPanel spDT;

        /// <summary>
        /// This is used to set the PopUp inside 
        /// </summary>
        private StackPanel spPopUp;

        /// <summary>
        /// The set of string values will stores as Array List for given Pattern.
        /// </summary>
        private string[] strDTPattern = new string[1];

        /// <summary>
        /// The set of string values will stores as temporary.
        /// </summary>
        private string[] strTmpDT;

        /// <summary>
        /// Each and every block of pattern will set in this textblock and add in stack
        /// panel.
        /// </summary>
        private TextBlock tbk;

        /// <summary>
        /// Each and every block of pattern will set in this textblock temporary and add in
        /// stack panel.
        /// </summary>
        private TextBlock tmpTBk;

        /// <summary>
        /// Each and every block of pattern will set in this textblock temporary and add in
        /// stack panel.
        /// </summary>
        private TextBlock tmpTB;

        /// <summary>
        /// Each and every block of pattern will set in this textblock temporary and add in
        /// stack panel.
        /// </summary>
        private TextBlock tbTmp;

        /// <summary>
        /// Each and every block of pattern will set in this textblock temporary and add in
        /// stack panel.
        /// </summary>
        private TextBlock tbGlobal;

        /// <summary>
        /// dtMinDate is used to store value for ar-SA format
        /// </summary>
        private DateTime dtMinDate;

        /// <summary>
        /// dtMaxDate is used to store value for ar-SA format
        /// </summary>
        private DateTime dtMaxDate;

        /// <summary>
        /// TextGrid Holds the TextBlocks and Cursor inside it.
        /// </summary>
        private Grid textGrid = new Grid();

        /// <summary>
        /// Whether date is selected or not on PopUpCalendar. Default value is false.
        /// </summary>
        private bool bPopUpCalendarClicked = false;
        #endregion

        #region - Constructor -
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.DateTimeEdit">DateTimeEdit</see>
        /// class.
        /// </summary>
        /// <remarks>
        /// Constructor of the DateTime Edit Control, which will initialize all events and
        /// properties.
        /// </remarks>
        public DateTimeEdit()
        {
            DefaultStyleKey = typeof(DateTimeEdit);
            this.Loaded += new RoutedEventHandler(DateTimeEdit_Loaded);
        }

        //static DateTimeEdit()
        //{
        //    if (System.ComponentModel.DesignerProperties.IsInDesignTool)
        //    {
        //        Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
        //        load = null;
        //    }

        //}
        protected internal bool isLoaded = false;
        void DateTimeEdit_Loaded(object sender, RoutedEventArgs e)
        {
            closePopup = new DispatcherTimer();
            closePopup.Interval = new TimeSpan(0, 0, 2);
            closePopup.Tick += new EventHandler(closePopup_Tick);
            isLoaded = true;
        }
        #endregion

        #region - Public Events -
        /// <summary>
        /// Event that is raised when CaretBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback CaretBackgroundChanged;

        /// <summary>
        /// Event that is raised when CaretCornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback CaretCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when Culture property is changed.
        /// </summary>
        public event PropertyChangedCallback CultureChanged;

        /// <summary>
        /// Event that is raised when CustomFormat property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomFormatChanged;

        /// <summary>
        /// Event that is raised when FontSize property is changed.
        /// </summary>
        public event PropertyChangedCallback FontSizeChanged;

        /// <summary>
        /// Event that is raised when HorizontalContentAlignment property is changed.
        /// </summary>
        public event PropertyChangedCallback TextAlignmentChanged;

       

        /// <summary>
        /// Event that is raised when IncorrectForeground property is changed.
        /// </summary>
        public event PropertyChangedCallback IncorrectForegroundChanged;

        /// <summary>
        /// Event that is raised when IsEditable property is changed.
        /// </summary>
        public event PropertyChangedCallback IsEditableChanged;

        /// <summary>
        /// Event that is raised when IsReadOnly property is changed.
        /// </summary>
        public event PropertyChangedCallback IsReadOnlyChanged;

        /// <summary>
        /// Event that is raised when MonthValueBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback MonthValueBackgroundChanged;

        /// <summary>
        /// Event that is raised when NonDate property is changed.
        /// </summary>
        public event PropertyChangedCallback NonDateChanged;

        /// <summary>
        /// Event that is raised when Pattern property is changed.
        /// </summary>
        public event PropertyChangedCallback PatternChanged;

        /// <summary>
        /// Event that is raised when PopUpDelay property is changed.
        /// </summary>
        public event PropertyChangedCallback PopUpDelayChanged;

        /// <summary>
        /// Event that is raised when ShowEmptyDate property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowEmptyDateChanged;

        /// <summary>
        /// Event that is raised when StepValue property is changed.
        /// </summary>
        public event PropertyChangedCallback StepValueChanged;

        /// <summary>
        /// Event that is raised when Value property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;
        #endregion

        #region - Public Properties (Getter and Setter) -
        /// <summary>
        /// Gets or sets the value for CaretBackground Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// CaretBackground=&quot;CadetBlue&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.CaretBackground=Colors.Gray; </para>
        /// </remarks>
        /// <value>
        /// Type: Color 
        /// <para>Default Value: Gray</para>
        /// </value>
        public Color CaretBackground
        {
            get { return (Color)GetValue(CaretBackgroundProperty); }
            set { SetValue(CaretBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for StepValue Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// CaretCornerRadius=&quot;13&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.CaretCornerRadius = 13;</para>
        /// </remarks>
        /// <value>
        /// Type: Double 
        /// <para>Default Value: CaretCornerRadius=&quot;13&quot;</para>
        /// </value>
        public double CaretCornerRadius
        {
            get { return (double)GetValue(CaretCornerRadiusProperty); }
            set { SetValue(CaretCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for Culture Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.Culture=&quot;en-US&quot;; </para>
        /// </remarks>
        /// <value>
        /// Type: CultureInfo 
        /// <para>Default Value: en-US</para>
        /// </value>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for CustomFormat Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// CustomFormat=&quot;dd-MMMM-yyyy hh:mm:ss.ff tt&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.CustomFormat = &quot;dd-MMMM-yyyy hh:mm:ss.ff
        /// tt&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type: String 
        /// <para>Default Value: &quot;dd-MMMM-yyyy hh:mm:ss.ff tt&quot;</para>
        /// </value>
        public string CustomFormat
        {
            get { return (string)GetValue(CustomFormatProperty); }
            set { SetValue(CustomFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for FontSize Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// FontSize=&quot;14&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.FontSize = 14;</para>
        /// </remarks>
        /// <value>
        /// Type: Double 
        /// <para>Default Value: &quot;14&quot;</para>
        /// </value>
        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for HorizontalContentAlignment Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// HorizontalContentAlignment=&quot;Left&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.HorizontalContentAlignment =
        /// HorizontalAlignment.Left;</para>
        /// </remarks>
        /// <value>
        /// Type: HorizontalAlignment 
        /// <para>Default Value: HorizontalContentAlignment=&quot;Left&quot;</para>
        /// </value>
        public HorizontalAlignment TextAlignment
        {
            get { return (HorizontalAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        

        /// <summary>
        /// Gets or sets the value for IncorrectForeground Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// IncorrectForeground=&quot;Red&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.IncorrectForeground = new
        /// SolidColorBrush(Colors.Red);</para>
        /// </remarks>
        /// <value>
        /// Type: Brush 
        /// <para>Default Value: IncorrectForeground=&quot;Red&quot;</para>
        /// </value>
        public Brush IncorrectForeground
        {
            get { return (Brush)GetValue(IncorrectForegroundProperty); }
            set { SetValue(IncorrectForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsEditable Dependency Property focused.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// IsEditable=&quot;True&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.IsEditable = true;</para>
        /// </remarks>
        /// <value>
        /// Type: Boolean 
        /// <para>Default Value: True</para>
        /// </value>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsReadOnly Dependency Property focused.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// IsReadOnly=&quot;True&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.IsReadOnly = true;</para>
        /// </remarks>
        /// <value>
        /// Type: Boolean 
        /// <para>Default Value: True</para>
        /// </value>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for MonthValueBackground Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// MonthValueBackground=&quot;White&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.MonthValueBackground = new
        /// SolidColorBrush(Colors.White);</para>
        /// </remarks>
        /// <value>
        /// Type: Brush 
        /// <para>Default Value: MonthValueBackground = &quot;White&quot;</para>
        /// </value>
        public Brush MonthValueBackground
        {
            get { return (Brush)GetValue(MonthValueBackgroundProperty); }
            set { SetValue(MonthValueBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for NonDate Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot; NonDate=&quot;No
        /// Date is Selected&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.NonDate = &quot;No Date is Selected&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type: String 
        /// <para>Default Value: &quot;No Date is Selected&quot;</para>
        /// </value>
        public string NonDate
        {
            get { return (string)GetValue(NonDateProperty); }
            set { SetValue(NonDateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for Pattern Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// Pattern=&quot;dd-MMMM-yyyy hh:mm:ss.ff tt&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.Pattern=&quot;dd-MMMM-yyyy hh:mm:ss.ff tt&quot;;
        /// </para>
        /// </remarks>
        /// <value>
        /// Type: String 
        /// <para>Default Value: dd-MMMM-yyyy hh:mm:ss.ff tt</para>
        /// </value>
        public string Pattern
        {
            get { return (string)GetValue(PatternProperty); }
            set { SetValue(PatternProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for PopUpDelay Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// PopUpDelay=&quot;300&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.PopUpDelay = 300;</para>
        /// </remarks>
        /// <value>
        /// Type: Brush 
        /// <para>Default Value: PopUpDelay=&quot;300&quot;</para>
        /// </value>
        public double PopUpDelay
        {
            get { return (double)GetValue(PopUpDelayProperty); }
            set { SetValue(PopUpDelayProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowEmptyDate Dependency Property focused.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// ShowEmptyDate=&quot;True&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.ShowEmptyDate = true;</para>
        /// </remarks>
        /// <value>
        /// Type: Boolean 
        /// <para>Default Value: True</para>
        /// </value>
        public bool ShowEmptyDate
        {
            get { return (bool)GetValue(ShowEmptyDateProperty); }
            set { SetValue(ShowEmptyDateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value for StepValue Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// StepValue=&quot;1&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.StepValue = 1;</para>
        /// </remarks>
        /// <value>
        /// Type: Int 
        /// <para>Default Value: StepValue=1</para>
        /// </value>
        public int StepValue
        {
            get { return (int)GetValue(StepValueProperty); }
            set { SetValue(StepValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets in Value Dependency Property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:DateTimeEdit Name=&quot;DateTimeEdit&quot;
        /// Value=&quot;DateTime.Now&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>DateTimeEdit DateTimeEditControl=new DateTimeEdit();</para>
        /// <para>DateTimeEditControl.Value=&quot;DateTime.Now&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type: DateTime 
        /// <para>Default Value: DateTime.Now</para>
        /// </value>
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Called when an internal process or application calls.
        /// </summary>
        /// <remarks>
        /// Initialize controls regarding DateTime Edit Control.
        /// </remarks>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.GotFocus += new RoutedEventHandler(DateTimeEdit_GotFocus);
            this.LostFocus += new RoutedEventHandler(DateTimeEdit_LostFocus);
            this.MouseEnter += new MouseEventHandler(DateTimeEdit_MouseEnter);
            this.MouseLeave += new MouseEventHandler(DateTimeEdit_MouseLeave);
            grdDateTimeEdit = GetTemplateChild("grdDateTimeEdit") as Grid;

            brdrDateTime = GetTemplateChild("brdrDateTime") as Border;
            brdrDateTime.MouseLeave += new MouseEventHandler(BrdrDateTime_MouseLeave);
            brdrDateTime.MouseMove += new MouseEventHandler(BrdrDateTime_MouseMove);
            brdrDateTime.SizeChanged += new SizeChangedEventHandler(BrdrDateTime_SizeChanged);

            spPopUp = GetTemplateChild("spPopUp") as StackPanel;
            spPopUp.Width = grdDateTimeEdit.Width;

            bPopUp = GetTemplateChild("bPopUp") as Border;
            bPopUp.MouseLeave += new MouseEventHandler(bPopUp_MouseLeave);
            bPopUp.Background = new SolidColorBrush(Colors.Transparent);
            cPopUp = GetTemplateChild("cPopUp") as Popup;
            cPopUp.MouseLeave += new MouseEventHandler(cPopUp_MouseLeave);

            cCalendarControl = GetTemplateChild("cCalendarControl") as Popup;
            cCalendarControl.IsOpen = false;
            popUpCalendar = GetTemplateChild("ccCalendarControl") as CalendarControl;
            popUpCalendar.AllowMultipleSelection = false;
            popUpCalendar.MouseLeave += new MouseEventHandler(PopUpCalendar_MouseLeave);
            popUpCalendar.SelectedDateChanged += new PropertyChangedCallback(PopUpCalendar_SelectedDateChanged);
            popUpCalendar.MouseLeftButtonUp += new MouseButtonEventHandler(PopUpCalendar_MouseLeftButtonUp);

            hlbCalendar = GetTemplateChild("hlbCalendar") as HyperlinkButton;
            hlbCalendar.Click += new RoutedEventHandler(HlbCalendar_Click);

            hlbNone = GetTemplateChild("hlbNone") as HyperlinkButton;

            hlbNone.Click += new RoutedEventHandler(HlbNone_Click);

            textGrid = GetTemplateChild("TextGrid") as Grid;

            spDT = GetTemplateChild("SPDT") as StackPanel;
            spDT.Children.Clear();

            csDT = GetTemplateChild("CSDT") as Canvas;
            csDT.Opacity = 0.5;
            btnUparrow = GetTemplateChild("btnUpArrow") as ToggleButton;
            btnDownArrow = GetTemplateChild("btnDownArrow") as ToggleButton;

            spPopUp.MouseMove += new MouseEventHandler(SpPopup_MouseMove);

            btnUparrow.Click += new RoutedEventHandler(BtnUpArrow_Click);
            btnDownArrow.Click += new RoutedEventHandler(BtnDownArrow_Click);           
            SplitDateTime();

            ////  Mouse Scrolling functions
            //HtmlPage.Window.AttachEvent("DOMMouseScroll", OnScroll);
            //HtmlPage.Window.AttachEvent("onmousewheel", OnScroll);
            //HtmlPage.Document.AttachEvent("onmousewheel", OnScroll);

            ////  VerticalContentAlignment Changed   
            textGrid.HorizontalAlignment = TextAlignment;
            textGrid.VerticalAlignment = VerticalAlignment.Center;
        }
        #endregion

        #region - internal Methods -
        /// <summary>
        /// This function gets the value for iChildrenIndex, and textblock width using
        /// SPDT.Children.
        /// </summary>
        /// <param name="tblock">Passing Current highlighting textblock to find Width and
        /// Index.</param>
        /// <returns>
        /// Type : double
        /// </returns>
        internal double GetTBPosition(TextBlock tblock)
        {
            /*  Get TextBlock Width on Storyboard, Current Highlighting TextBlock, Current TextBlock Width  */
                tbWidth = 0;
                iChildrenIndex = -1;
                double dwidth = 0;
                spDT.UpdateLayout();
                iChildrenIndex = spDT.Children.IndexOf(tblock);
                if (tblock != null&&iChildrenIndex>=0)
                {
                    GeneralTransform trans = tblock.TransformToVisual(spDT);
                    Point distance = trans.Transform(new Point(0, 0));
                    return distance.X;
                }

            return dwidth;
        }
        #endregion

        #region - Protected Methods -
        /// <summary>
        /// Updates property value cache and raised IsEditableChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnIsEditableChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            Focus();
            if (IsEditableChanged != null)
            {
                IsEditableChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised IsReadOnlyChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnIsReadOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            brdrDateTime.IsHitTestVisible = IsReadOnly;
            Focus();
            if (IsReadOnlyChanged != null)
            {
                IsReadOnlyChanged(this, e);
            }
        }

        /// <summary>
        /// When key down the value gets changed in DateTime Edit Control. Depends upon the
        /// key pressed the corresponding property will get update.
        /// </summary>
        /// <remarks>
        /// Change DateTime Edit Control value depends upon the key pressed. The value can
        /// be updated using up and down arrow, and also using keys.
        /// </remarks>
        /// <param name="e">Gets Keys from Keyboard and implements the values.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (sbRectangle != null)
            {
                if (e.Key == Key.Up)
                {
                    if (tbk != null)
                    {
                        buttonClicked = "Up";
                        ChangeDTValue();
                    }
                }
                else if (e.Key == Key.Down)
                {
                    if (tbk != null)
                    {
                        buttonClicked = "Down";
                        ChangeDTValue();
                    }
                }
                else if (e.Key == Key.Left)
                {
                    buttonClicked = "Left";
                    if (iChildrenIndex >= 0)
                    {
                        for (int iCI = 0; iCI <= iChildrenIndex; iCI++)
                        {
                            if (iCI == iChildrenIndex)
                            {
                                if ((0 == iChildrenIndex) && (iSelectedChar == 0 || tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM"))
                                {
                                    goto stop;
                                }

                                if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt")
                                {
                                    break;
                                }
                                else
                                {
                                    if (iSelectedChar > 0)
                                    {
                                        if (eGetPosition >= 1)
                                        {
                                            eGetPosition -= iSingChar;
                                        }

                                        MoveLeftRightPosition("Left");
                                        if (iSelectedChar == 0)
                                        {
                                            iDD = 0;
                                        }

                                        ReturnDTE();    /*  Sets the Modified Date in "Value" Dependency Property   */
                                        return;
                                    }
                                    else if (iSelectedChar < 0 && iChildrenIndex == 0)
                                    {
                                        /*  Even if we move left on first Character from first TextBlock    */
                                        goto stop;
                                    }
                                }
                            }
                        }

                        /*  TextBlock Last character over and then checks next Textblock is symbol or valid format, regarding moves to next textblock on Left Direction */
                        iChildrenIndex -= 1;

                        FindChildNextBlock(iChildrenIndex, "Left");

                        MoveLeftRightPosition("Left");
                    }
                }
                else if (e.Key == Key.Right)
                {
                    buttonClicked = "Right";
                    if (iChildrenIndex <= spDT.Children.Count)
                    {
                        for (int iCI = 0; iCI <= iChildrenIndex; iCI++)
                        {
                            if (iCI == iChildrenIndex)
                            {
                                if (spDT.Children.Count - 1 == iChildrenIndex && (iSelectedChar == tbk.Text.Length - 1 || tbk.Tag.ToString() == "t" || tbk.Tag.ToString() == "tt"))
                                {
                                    goto stop;
                                }

                                if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt")
                                {
                                    break;
                                }
                                else
                                {
                                    if (iSelectedChar < tbk.Text.Length - 1)
                                    {
                                        /*  TextBlock still has next character  */
                                        iCIndex = iChildrenIndex;
                                        MoveLeftRightPosition("Right");
                                        ReturnDTE();    /*  Sets the Modified Date in "Value" Dependency Property   */
                                        return;
                                    }
                                    else if (iSelectedChar == tbk.Text.Length - 1 && iCI == spDT.Children.Count - 1)
                                    {
                                        /*  Even if we move left on first Character from first TextBlock    */
                                        goto stop;
                                    }
                                }
                            }
                        }

                        /*  TextBlock Last character over and then checks next Textblock is symbol or valid format, regarding moves to next textblock on Right Direction */
                        iChildrenIndex += 1;
                        if (iChildrenIndex == spDT.Children.Count - 1)
                        {
                            GetTBWidth(iChildrenIndex);
                            string strFirstPattern = tbk.Tag.ToString();
                            if (!(strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" || strFirstPattern[0].ToString() == "y" ||
                                strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" || strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" ||
                                strFirstPattern[0].ToString() == "s" || strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                                strFirstPattern[0].ToString() == "t"))
                            {
                                iChildrenIndex -= 1;
                                goto stop;
                            }
                        }

                        FindChildNextBlock(iChildrenIndex, "Right");
                        MoveLeftRightPosition("Right");
                    }
                }
                else if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                {
                    if (IsEditable == false)
                    {
                        goto stop;
                    }

                    if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM"))
                    {
                        /*  Not a Month Field   */
                        strTmpCombine = string.Empty;
                        string strBeforeTBk = tbk.Text;
                        char[] strTmpTBk = tbk.Text.ToCharArray();

                        if (strTmpTBk[0] >= 48 && strTmpTBk[0] <= 57)
                        {
                            TextBlock tbbeforeupdate = tbk as TextBlock;

                            try
                            {
                                dtBeforeTBk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                            }
                            catch (FormatException)
                            {
                                if (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH" || tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm" || tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss" || tbk.Tag.ToString() == "f" || tbk.Tag.ToString() == "F" || tbk.Tag.ToString() == "ff" || tbk.Tag.ToString() == "FF" || tbk.Tag.ToString() == "fff" || tbk.Tag.ToString() == "FFF")
                                {
                                    iHH = 1;
                                }
                                else
                                {
                                    dtBeforeTBk = dt;
                                    tbk.Text = strBeforeTBk;
                                    goto stop;
                                }
                            }

                            if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                            {
                                strTmpTBk[iSelectedChar] = '0';
                            }
                            else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                            {
                                strTmpTBk[iSelectedChar] = '1';
                            }
                            else if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                            {
                                strTmpTBk[iSelectedChar] = '2';
                            }
                            else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                            {
                                strTmpTBk[iSelectedChar] = '3';
                            }
                            else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                            {
                                strTmpTBk[iSelectedChar] = '4';
                            }
                            else if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                            {
                                strTmpTBk[iSelectedChar] = '5';
                            }
                            else if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                            {
                                strTmpTBk[iSelectedChar] = '6';
                            }
                            else if (e.Key == Key.D7 || e.Key == Key.NumPad7)
                            {
                                strTmpTBk[iSelectedChar] = '7';
                            }
                            else if (e.Key == Key.D8 || e.Key == Key.NumPad8)
                            {
                                strTmpTBk[iSelectedChar] = '8';
                            }
                            else if (e.Key == Key.D9 || e.Key == Key.NumPad9)
                            {
                                strTmpTBk[iSelectedChar] = '9';
                            }

                            for (int i = 0; i < strTmpTBk.Length; i++)
                            {
                                strTmpCombine += strTmpTBk[i].ToString();
                            }

                            if (strTmpCombine == "00" && tbk.Tag.ToString() == "dd")
                            {
                                strTmpCombine = "01";
                                strDD = "01";
                            }
                            else if (Convert.ToInt16(strTmpCombine) >= 31 && tbk.Tag.ToString() == "dd")
                            {
                                strTmpCombine = 31.ToString();
                                strDD = 31.ToString();
                            }

                            tbk.Text = strTmpCombine;

                            try
                            {
                                if (tbk.Tag.ToString() == "d" || tbk.Tag.ToString() == "dd")
                                {
                                    if (Convert.ToInt16(tbk.Text) <= DateTime.DaysInMonth(dt.Year, dt.Month))
                                    {
                                        DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                        UpdateDTE(dtaftertbk, dtBeforeTBk);
                                    }
                                    else
                                    {
                                        iDD = Convert.ToInt16(DateTime.DaysInMonth(dt.Year, dt.Month));

                                        spDT.Children.RemoveAt(iChildrenIndex);
                                        spDT.Children.Insert(iChildrenIndex, tbk);

                                        SetHighlighPosition(true);
                                    }
                                }
                                else if (tbk.Tag.ToString() == "M" || tbk.Tag.ToString() == "MM")
                                {
                                    DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, "MM", Culture);
                                    int i = dtaftertbk.Month - dtBeforeTBk.Month;
                                    dtPat = dt;
                                    dt = dtPat.AddMonths(i);

                                    if (iDD != 0 && DateTime.DaysInMonth(dt.Year, dt.Month) > iDD)
                                    {
                                        dtPat = dt;
                                        dt = dtPat.AddDays(Convert.ToInt16(strDD) - dt.Day);
                                        SplitDateTime();
                                    }
                                    else if (iDD <= DateTime.DaysInMonth(dt.Year, dt.Month) && iSelectedChar == 1)
                                    {
                                        dtPat = dt;
                                        dt = dtPat.AddDays(iDD - Convert.ToInt16(dt.Day));
                                        SplitDateTime();
                                    }
                                    else if (dtPat.Day > DateTime.DaysInMonth(dt.Year, dt.Month))
                                    {
                                        iDD = dtPat.Day;
                                    }

                                    SetHighlighPosition(true);
                                }
                                else if (tbk.Tag.ToString() == "y" || tbk.Tag.ToString() == "yy" || tbk.Tag.ToString() == "yyy" || tbk.Tag.ToString() == "yyyy" || tbk.Tag.ToString() == "Y" || tbk.Tag.ToString() == "YY" || tbk.Tag.ToString() == "YYY" || tbk.Tag.ToString() == "YYYY")
                                {
                                    if (iDD == 0)
                                    {
                                        DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                        UpdateDTE(dtaftertbk, dtBeforeTBk);
                                    }
                                    else
                                    {
                                        if (iDD != 0 && DateTime.DaysInMonth(dt.Year, dt.Month) > iDD)
                                        {
                                            DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                            UpdateDTE(dtaftertbk, dtBeforeTBk);
                                        }
                                        else if (DateTime.DaysInMonth(Convert.ToInt16(tbk.Text), dt.Month) < iDD && iDD != 0)
                                        {
                                            if (iSelectedChar != 0)
                                            {
                                                DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                                dt = dtPat.AddYears(dtaftertbk.Year - dtBeforeTBk.Year);
                                            }

                                            dtPat = dt;
                                            dt = dtPat.AddDays(DateTime.DaysInMonth(Convert.ToInt16(tbk.Text), dtPat.Month) - dtPat.Day);

                                            SplitDateTime();

                                            SetHighlighPosition(false);
                                        }
                                        else
                                        {
                                            DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                            dt = dtPat.AddYears(dtaftertbk.Year - dtBeforeTBk.Year);

                                            dtPat = dt;
                                            dt = dtPat.AddDays(DateTime.DaysInMonth(dtPat.Year, dtPat.Month) - dtPat.Day);

                                            SplitDateTime();

                                            SetHighlighPosition(false);
                                        }
                                    }
                                }
                                else if (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH")
                                {
                                    if (Convert.ToInt16(tbk.Text) <= DateTime.MaxValue.Hour)
                                    {
                                        DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                        UpdateDTE(dtaftertbk, dtBeforeTBk);
                                    }
                                    else
                                    {
                                        spDT.Children.RemoveAt(iChildrenIndex);
                                        spDT.Children.Insert(iChildrenIndex, tbk);

                                        SetHighlighPosition(true);
                                    }
                                }
                                else if (tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm")
                                {
                                    if (Convert.ToInt16(tbk.Text) <= DateTime.MaxValue.Minute)
                                    {
                                        DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                        UpdateDTE(dtaftertbk, dtBeforeTBk);
                                    }
                                    else
                                    {
                                        spDT.Children.RemoveAt(iChildrenIndex);
                                        spDT.Children.Insert(iChildrenIndex, tbk);

                                        SetHighlighPosition(true);
                                    }
                                }
                                else if (tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss")
                                {
                                    if (Convert.ToInt16(tbk.Text) <= DateTime.MaxValue.Second)
                                    {
                                        DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                        UpdateDTE(dtaftertbk, dtBeforeTBk);
                                    }
                                    else
                                    {
                                        spDT.Children.RemoveAt(iChildrenIndex);
                                        spDT.Children.Insert(iChildrenIndex, tbk);

                                        SetHighlighPosition(true);
                                    }
                                }
                                else
                                {
                                    iHH = 0;
                                    DateTime dtaftertbk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                                    UpdateDTE(dtaftertbk, dtBeforeTBk);
                                }
                            }
                            catch (FormatException)
                            {
                                spDT.Children.RemoveAt(iChildrenIndex);
                                spDT.Children.Insert(iChildrenIndex, tbk);

                                SetHighlighPosition(true);
                            }
                        }
                    }
                    else if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM")
                    {
                        /*  Month Field */
                        dtBeforeTBk = DateTime.ParseExact(tbk.Text, tbk.Tag.ToString(), Culture);
                        int ibeforetbk = dtBeforeTBk.Month;

                        if ((e.Key == Key.D0 || e.Key == Key.NumPad0) || (e.Key == Key.D1 || e.Key == Key.NumPad1))
                        {
                            /*  If 0 or 1, then continue to get next character from user    */
                            if (e.Key == Key.D0 || e.Key == Key.NumPad0)
                            {
                                sBeforeTBk += "0";
                                if (biCharEntered == false)
                                {
                                    sBeforeTBk = "0";
                                }
                            }
                            else if (e.Key == Key.D1 || e.Key == Key.NumPad1)
                            {
                                sBeforeTBk += "1";
                                if (biCharEntered == false)
                                {
                                    sBeforeTBk = "1";
                                }
                            }

                            if (biCharEntered == false)
                            {
                                biCharEntered = true;
                                TextBox tbmonth = new TextBox();
                                tbmonth.Background = MonthValueBackground;
                                tbmonth.Width = GetTBWidth(iChildrenIndex);
                                tbmonth.HorizontalAlignment = HorizontalAlignment.Center;
                                sbRectangle.Pause();
                                bdHighlight.Opacity = 0.3;
                                bdHighlight.Name = "bdHighlight";
                                tbk.Opacity = 0.3;
                                bdHighlight.Child = tbmonth;
                                csDT.Children.Clear();
                                csDT.Children.Add(bdHighlight);
                                return;
                            }
                        }

                        biCharEntered = false;
                        if (e.Key == Key.D2 || e.Key == Key.NumPad2)
                        {
                            sBeforeTBk += "2";
                        }
                        else if (e.Key == Key.D3 || e.Key == Key.NumPad3)
                        {
                            sBeforeTBk += "3";
                        }
                        else if (e.Key == Key.D4 || e.Key == Key.NumPad4)
                        {
                            sBeforeTBk += "4";
                        }
                        else if (e.Key == Key.D5 || e.Key == Key.NumPad5)
                        {
                            sBeforeTBk += "5";
                        }
                        else if (e.Key == Key.D6 || e.Key == Key.NumPad6)
                        {
                            sBeforeTBk += "6";
                        }
                        else if (e.Key == Key.D7 || e.Key == Key.NumPad7)
                        {
                            sBeforeTBk += "7";
                        }
                        else if (e.Key == Key.D8 || e.Key == Key.NumPad8)
                        {
                            sBeforeTBk += "8";
                        }
                        else if (e.Key == Key.D9 || e.Key == Key.NumPad9)
                        {
                            sBeforeTBk += "9";
                        }

                        try
                        {
                            /*  To Change Month */
                            DateTime dtaftertbk = DateTime.ParseExact(sBeforeTBk, "MM", Culture);
                            int i = dtaftertbk.Month - dtBeforeTBk.Month;
                            dtPat = dt;
                            dt = dtPat.AddMonths(i);

                            /*  To Check with Given Day, with wrong value   */
                            try
                            {
                                if (iDD != 0 && DateTime.DaysInMonth(dt.Year, dt.Month) > iDD)
                                {
                                    dtPat = dt;
                                    /*  On June Maximum value is 30, should minus from 30 to current date. Eg: (31-Dec-2009 to 31-Jun-2009) */
                                    dt = dtPat.AddDays(DateTime.DaysInMonth(dt.Year, dt.Month) - dt.Day);
                                }
                                else if (iDD != 0 && iDD <= DateTime.DaysInMonth(dt.Year, dt.Month))
                                {
                                    dtPat = dt;
                                    dt = dtPat.AddDays(iDD - Convert.ToInt16(dt.Day));
                                }
                                else if (dtPat.Day > DateTime.DaysInMonth(dt.Year, dt.Month))
                                {
                                    iDD = dtPat.Day;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        catch (FormatException)
                        {
                        }

                        SplitDateTime();

                        double dw = GetTBWidth(iChildrenIndex);
                        GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);

                        sbRectangle.Resume();
                        bdHighlight.Opacity = 1;
                        bdHighlight.Child = null;
                        bdHighlight.Name = "bdHighlight";
                        tbk.Opacity = 1;
                        csDT.Children.Clear();
                        csDT.Children.Add(bdHighlight);

                        /*  To Move Next Character or Next Textblock After a Key is Pressed */
                        if (iChildrenIndex <= spDT.Children.Count)
                        {
                            for (int iCI = 0; iCI <= iChildrenIndex; iCI++)
                            {
                                if (iCI == iChildrenIndex)
                                {
                                    if (spDT.Children.Count - 1 == iChildrenIndex && (iSelectedChar == tbk.Text.Length - 1 || tbk.Tag.ToString() == "t" || tbk.Tag.ToString() == "tt"))
                                    {
                                        goto stop;
                                    }

                                    if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (iSelectedChar < tbk.Text.Length - 1)
                                        {
                                            iCIndex = iChildrenIndex;
                                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * (iSelectedChar + 1)));
                                            iSelectedChar += 1;
                                            goto stop;
                                        }
                                        else if (iSelectedChar == tbk.Text.Length - 1 && iCI == spDT.Children.Count - 1)
                                        {
                                            goto stop;
                                        }
                                    }
                                }
                            }

                            iChildrenIndex += 1;
                            if (iChildrenIndex == spDT.Children.Count - 1)
                            {
                                GetTBWidth(iChildrenIndex);
                                string strFirstPattern = tbk.Tag.ToString();
                                if (!(strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" || strFirstPattern[0].ToString() == "y" ||
                                    strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" || strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" ||
                                    strFirstPattern[0].ToString() == "s" || strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                                    strFirstPattern[0].ToString() == "t"))
                                {
                                    iChildrenIndex -= 1;
                                    goto stop;
                                }
                            }

                            FindChildNextBlock(iChildrenIndex, "Right");
                            GetTBWidth(iChildrenIndex);
                            GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);
                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * ((iSelectedChar == 0) ? iSelectedChar : iSelectedChar + 1)));
                            bdHighlight.Width = iSingChar;
                        }
                    }
                }
            }

        stop:
            ReturnDTE();    /*  Sets the Modified Date in "Value" Dependency Property   */
        }

        /// <summary>
        /// Updates property value cache and raised MonthValueBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnMonthValueBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            Focus();
            if (MonthValueBackgroundChanged != null)
            {
                MonthValueBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised NonDateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnNonDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            if (noneClicked == true)
            {
                Pattern = NonDate;
            }

            if (NonDateChanged != null)
            {
                NonDateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises PatternChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnPatternChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            SplitDateTime();
            Focus();
            if (PatternChanged != null)
            {
                PatternChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised PopUpDelayChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnPopUpDelayChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PopUpDelayChanged != null)
            {
                PopUpDelayChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises StepValueChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnStepValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StepValueChanged != null)
            {
                StepValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised ShowEmptyDateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnShowEmptyDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowEmptyDate == true)
            {
                hlbNone.Visibility = Visibility.Visible;
            }
            else if (ShowEmptyDate == false)
            {
                hlbNone.Visibility = Visibility.Collapsed;
            }

            csDT.Children.Clear();
            Focus();
            if (ShowEmptyDateChanged != null)
            {
                ShowEmptyDateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised ValueChanged event. Updates on both
        /// DateTime Edit Control and Calendar Control
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (csDT != null)
            {
                csDT.Children.Clear();
            }

            SetDTEDefaultValue();
            popUpCalendar.Date = Value; /*  When Value changes, then reflect at Calendar Control Also   */
            if (bdHighlight != null)
            {
                csDT.Children.Clear();
                csDT.Children.Add(bdHighlight);
            }

            Focus();
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CaretBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnCaretBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {         
            if (sbRectangle != null)
            {
                caAnimation.To = CaretBackground;
            }

            Focus();
            if (CaretBackgroundChanged != null)
            {
                CaretBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CaretCornerRadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnCaretCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (bdHighlight != null)
            {
                bdHighlight.CornerRadius = new CornerRadius(CaretCornerRadius);
            }

            if (CaretCornerRadiusChanged != null)
            {
                CaretCornerRadiusChanged(this, e);
            }
        }

        bool leftFlowDirection = false;

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnCultureChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            if (spDT != null)
            {
                SplitDateTime();
                popUpCalendar.Culture = Culture;   /*  When Culture changes, then affect at Calendar Control Also  */
            }

            if (Culture.ToString() == "ar-SA" || Culture.ToString() == "he-IL")
            {
                leftFlowDirection = true;
                this.grdDateTimeEdit.RowDefinitions.Clear();
                this.grdDateTimeEdit.ColumnDefinitions.Clear();
                RowDefinition r1 = new RowDefinition();
                RowDefinition r2 = new RowDefinition();
                ColumnDefinition c1 = new ColumnDefinition();
                ColumnDefinition c2 = new ColumnDefinition();
                GridLength width2 = new GridLength(25);
                c2.Width = width2;
                this.grdDateTimeEdit.RowDefinitions.Add(r1);
                this.grdDateTimeEdit.RowDefinitions.Add(r2);
                this.grdDateTimeEdit.ColumnDefinitions.Add(c2);
                this.grdDateTimeEdit.ColumnDefinitions.Add(c1);
                this.textGrid.SetValue(Grid.RowProperty, (int)0);
                this.textGrid.SetValue(Grid.ColumnProperty, (int)1);
                this.textGrid.SetValue(Grid.RowSpanProperty, (int)2);
                this.textGrid.HorizontalAlignment = HorizontalAlignment.Right;
                this.btnUparrow.SetValue(Grid.RowProperty, (int)0);
                this.btnUparrow.SetValue(Grid.ColumnProperty, (int)0);
                this.btnDownArrow.SetValue(Grid.RowProperty, (int)1);
                this.btnDownArrow.SetValue(Grid.ColumnProperty, (int)0);
                Thickness updownborder = new Thickness(0, 0, 1, 0);
                this.btnUparrow.BorderThickness = updownborder;
                this.btnDownArrow.BorderThickness = updownborder;
            }
            else
            {
                if (grdDateTimeEdit != null)
                {
                    leftFlowDirection = false;
                    this.grdDateTimeEdit.RowDefinitions.Clear();
                    this.grdDateTimeEdit.ColumnDefinitions.Clear();
                    RowDefinition r1 = new RowDefinition();
                    RowDefinition r2 = new RowDefinition();
                    ColumnDefinition c1 = new ColumnDefinition();
                    ColumnDefinition c2 = new ColumnDefinition();
                    GridLength width2 = new GridLength(25);
                    c2.Width = width2;
                    this.grdDateTimeEdit.RowDefinitions.Add(r1);
                    this.grdDateTimeEdit.RowDefinitions.Add(r2);
                    this.grdDateTimeEdit.ColumnDefinitions.Add(c1);
                    this.grdDateTimeEdit.ColumnDefinitions.Add(c2);
                    this.textGrid.SetValue(Grid.RowProperty, (int)0);
                    this.textGrid.SetValue(Grid.ColumnProperty, (int)0);
                    this.textGrid.SetValue(Grid.RowSpanProperty, (int)2);
                    this.textGrid.HorizontalAlignment = TextAlignment;
                    this.spDT.HorizontalAlignment = TextAlignment;
                    this.btnUparrow.SetValue(Grid.RowProperty, (int)0);
                    this.btnUparrow.SetValue(Grid.ColumnProperty, (int)1);
                    this.btnDownArrow.SetValue(Grid.RowProperty, (int)1);
                    this.btnDownArrow.SetValue(Grid.ColumnProperty, (int)1);
                    Thickness updownborder = new Thickness(1, 0, 0, 0);
                    this.btnUparrow.BorderThickness = updownborder;
                    this.btnDownArrow.BorderThickness = updownborder;
                }
            }

            Focus();
            if (CultureChanged != null)
            {
                CultureChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised CustomFormatChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnCustomFormatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
                csDT.Children.Clear();
            }

            this.Pattern = this.CustomFormat;
            if (CustomFormatChanged != null)
            {
                CustomFormatChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised FontSizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (spDT != null)
            {
                int iLength = 0;
                foreach (TextBlock tb in spDT.Children)
                {
                    tb.FontSize = FontSize;
                    iLength += (int)tb.ActualWidth;
                }

                if (iLength + 25 > brdrDateTime.ActualWidth)
                {
                    TextAlignment = HorizontalAlignment.Left;
                }
            }

            if (sbRectangle != null)
            {
                sbRectangle.Stop();
                sbRectangle = null;
            }

            if (csDT != null)
            {
                csDT.Children.Clear();
            }

            if (FontSizeChanged != null)
            {
                FontSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raised HorizontalContentAlignmentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textGrid != null)
            {
                textGrid.HorizontalAlignment = TextAlignment;
                if (bdHighlight != null)
                {
                    Focus();
                }
            }

            if (TextAlignmentChanged != null)
            {
                TextAlignmentChanged(this, e);
            }
        }

       

        /// <summary>
        /// Updates property value cache and raises IncorrectForegroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected void OnIncorrectForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            SetInvalidFields(e);
            Focus();
            if (IncorrectForegroundChanged != null)
            {
                IncorrectForegroundChanged(this, e);
            }
        }
        #endregion

        #region - Static Methods -
        /// <summary>
        /// Sets Caret Background Color.
        /// </summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Caret Background Color.</param>
        private static void OnCaretBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnCaretBackgroundChanged(e);
        }

        /// <summary>
        /// Sets Caret CornerRadius Color.
        /// </summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Caret CornerRadius.</param>
        private static void OnCaretCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnCaretCornerRadiusChanged(e);
        }

        /// <summary>
        /// Sets ShowEmptyDate.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as ShowEmptyDate.</param>
        private static void OnShowEmptyDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnShowEmptyDateChanged(e);
        }

        /// <summary>
        /// Sets Step Value Color.
        /// </summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Step Value Color.</param>
        private static void OnStepValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnStepValueChanged(e);
        }

        /// <summary>
        /// Sets Value.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as Value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnValueChanged(e);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// Sets TextAlignment.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as
        /// TextAlignment.</param>
        private static void OnTextAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnTextAlignmentChanged(e);
        }

       
        /// <summary>
        /// Sets Incorrect Foreground Color.
        /// </summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Incorrect Foreground
        /// Color.</param>
        private static void OnIncorrectForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnIncorrectForegroundChanged(e);
        }

        /// <summary>
        /// Sets NonDate.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as NonDate.</param>
        private static void OnNonDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnNonDateChanged(e);
        }

        /// <summary>
        /// Sets IsEditable.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as IsEditable.</param>
        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnIsEditableChanged(e);
        }

        /// <summary>
        /// Sets MonthValueBackground.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as Background.</param>
        private static void OnMonthValueBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnMonthValueBackgroundChanged(e);
        }

        /// <summary>
        /// Sets IsReadOnly.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as IsReadOnly.</param>
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnIsReadOnlyChanged(e);
        }

        /// <summary>
        /// Sets Pattern.
        /// </summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Pattern.</param>
        private static void OnPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnPatternChanged(e);
        }

        /// <summary>
        /// Sets PopUpDelay.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as PopUpDelay.</param>
        private static void OnPopUpDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnPopUpDelayChanged(e);
        }

        /// <summary>
        /// Sets Culture.</summary>
        /// <param name="d">DateTimeEdit object, the change occures on.</param>
        /// <param name="e">Property change details, such as Culture.</param>
        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnCultureChanged(e);
        }

        /// <summary>
        /// Sets CustomFormat.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as CustomFormat.</param>
        private static void OnCustomFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnCustomFormatChanged(e);
        }

        /// <summary>
        /// Sets FontSize.
        /// </summary>
        /// <param name="d">DateTimeEdit Object, the change occurs on.</param>
        /// <param name="e">Property change details, such as FontSize.</param>
        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeEdit instance = (DateTimeEdit)d;
            instance.OnFontSizeChanged(e);
        }

        /// <summary>
        /// Creates dynamic TextBlock and sets events and values, then add in Stack Panel
        /// (SPDT).
        /// </summary>
        /// <param name="strDT">Passes pattern part values.</param>
        private void AddDTPattern(string strDT)
        {
            /*  Sets TextBox Property Values and Store in StackPanel    */
            if (strDT == string.Empty)
            {
                return;
            }

            int ii = spDT.Children.Count;
            strTmpDT = new string[iNewSizeFormat];
            strDTPattern.CopyTo(strTmpDT, 0);
            strTmpDT[iNewSizeFormat - 1] = strDT;
            tbk = new TextBlock();
            try
            {
                /*  GMT or T format in Pattern  */
                if (strDT == "GMT" || strDT == "T")
                {
                    tbk.Text = strDT;
                }
                else
                {
                    /*  Other Than GMT and T in Pattern */
                    if (strDT == "d")
                    {
                        /* Single Character converts into Double Charcter in Pattern and converts into DateTime Value   */
                        tbk.Text = int.Parse(dt.ToString("dd", Culture)).ToString();
                    }
                    else if (strDT == "M")
                    {
                        tbk.Text = int.Parse(dt.ToString("MM", Culture)).ToString();
                    }
                    else if (strDT == "y")
                    {
                        tbk.Text = int.Parse(dt.ToString("yy", Culture)).ToString();
                    }
                    else if (strDT == "h")
                    {
                        tbk.Text = int.Parse(dt.ToString("hh", Culture)).ToString();
                    }
                    else if (strDT == "H")
                    {
                        tbk.Text = int.Parse(dt.ToString("HH", Culture)).ToString();
                    }
                    else if (strDT == "m")
                    {
                        tbk.Text = int.Parse(dt.ToString("mm", Culture)).ToString();
                    }
                    else if (strDT == "s")
                    {
                        tbk.Text = int.Parse(dt.ToString("ss", Culture)).ToString();
                    }
                    else
                    {
                        /*  Single & Double Characters, and Symbols */
                        string strFirstPattern = strDT;
                        if (strFirstPattern != string.Empty)
                        {
                            if (strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" || strFirstPattern[0].ToString() == "y" ||
                                  strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" || strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" ||
                                  strFirstPattern[0].ToString() == "s" || strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                                  strFirstPattern[0].ToString() == "t")
                            {
                                tbk.Text = dt.ToString(strDT, Culture); /*  Valid Pattern Converts into DateTime Value. */
                            }
                            else
                            {
                                tbk.Text = strDT;   /*  Invalid Pattern Stores as it is in TextBlock.   */
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                tbk.Text = strDT;
            }

            tbk.FontSize = FontSize;
            tbk.Tag = strDT;

            Grid.SetRowSpan(tbk, 2);

            if (strDT == "d" || strDT == "dd" || strDT=="dddd" || strDT == "M" || strDT == "MM" || strDT == "MMM" || strDT == "MMMM" || strDT == "y" || strDT == "yy" || strDT == "yyy" || strDT == "yyyy"
                || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h" || strDT == "hh" || strDT == "H" || strDT == "HH" || strDT == "m" || strDT == "mm"
                || strDT == "s" || strDT == "ss" || strDT == "S" || strDT == "SS" || strDT == "f" || strDT == "ff" || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF"
                || strDT == "t" || strDT == "tt")
            {
                tbk.Cursor = Cursors.IBeam; /*  Sets Character Cursor for Valid Values    */
            }
            else
            {
                tbk.Cursor = Cursors.Arrow; /*  Sets Arrow Cursor for InValid Value */
            }

            tbk.MouseLeftButtonDown += new MouseButtonEventHandler(TBk_MouseLeftButtonDown);
            tbk.MouseMove += new MouseEventHandler(TBk_MouseMove);
            spDT.Children.Add(tbk);
            strDTPattern = strTmpDT;
            iNewSizeFormat += 1;
        }
        
        /// <summary>
        ///  When the control Lost focus
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Contains the focus event changes</param>
        private void DateTimeEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            isFocused = false;
            csDT.Children.Remove(bdHighlight);
            VisualStateManager.GoToState(this, "Unfocused", false);
        }

        /// <summary>
        /// When the control got focused
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Contains the event changes</param>
        private void DateTimeEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            isFocused = true;
            VisualStateManager.GoToState(this, "Normal", false);
            VisualStateManager.GoToState(this, "Focused", false); 
        }

        /// <summary>
        /// When Mouse Leave Datetime Control
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Contains the change in mouse status</param>
        void DateTimeEdit_MouseLeave(object sender, MouseEventArgs e)
        {
           VisualStateManager.GoToState(this, "Normal", false);
        }

        /// <summary>
        /// When Mouse Enters Datetime Control
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Contains the change in mouse status</param>
        void DateTimeEdit_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!isFocused)
            {
                VisualStateManager.GoToState(this, "MouseOver", false);
            }
            if (isLoaded)
            {
                HtmlPage.Window.AttachEvent("DOMMouseScroll", OnScroll);
                HtmlPage.Window.AttachEvent("onmousewheel", OnScroll);
                HtmlPage.Document.AttachEvent("onmousewheel", OnScroll);
                isLoaded = false;   
            }
        }

        /// <summary>
        /// When mouse leaves DateTime Edit Control, the PopUP will disabled.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BrdrDateTime_MouseLeave(object sender, MouseEventArgs e)
        {
            dptPopUpTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            dptPopUpTimer.Stop();
            scroll = false;
        }

        /// <summary>
        /// Show PopUp when mouse cursor moves on Highlighting Border.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BrdrDateTime_MouseMove(object sender, MouseEventArgs e)
        {
            if (sbRectangle != null || Pattern == NonDate)
            {
               // ShowPopUpDispatcherTimer();
            }

            scroll = true;
        }

        /// <summary>
        /// If DateTime Edit Control size changed, then highlighting position changes and
        /// pointed same position.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BrdrDateTime_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = brdrDateTime.ActualHeight;
           // this.SetRectangleAnimation(spDT.Children.IndexOf(tbTmp));
            if (cPopUp.IsOpen == true || cCalendarControl.IsOpen == true)
            {
                cPopUp.IsOpen = false;
                cCalendarControl.IsOpen = false;
            }
        }

        /// <summary>
        /// When Down Button clicked, then the MouseUpDownArrow function called to update
        /// corresponding value.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BtnDownArrow_Click(object sender, RoutedEventArgs e)
        {                                
            if (Pattern != NonDate && Pattern!=maxminLimitDisplay)
            {
                MouseUpDownArrow("Down");
            }
            else
            {
                this.SetTodaysDate();
            }           
        }

        /// <summary>
        /// When Up Button clicked, then the MouseUpDownArrow function called to update
        /// corresponding value.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BtnUpArrow_Click(object sender, RoutedEventArgs e)
        {                      
            if (Pattern != NonDate && Pattern != maxminLimitDisplay)
            {
                MouseUpDownArrow("Up");
            }
            else
            {
                this.SetTodaysDate();
            }            
        }

        /// <summary>
        /// When we leave the mouse from the calendar control, then calendar will disable.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PopUpCalendar_MouseLeave(object sender, MouseEventArgs e)
        {
            cCalendarControl.IsOpen = false;
            scroll = false;
        }

        /// <summary>
        /// When the calendar control date changes, then gets date and adds time fields.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new
        /// value.</param>
        private void PopUpCalendar_SelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int idthour = dt.Hour;
            int idtminute = dt.Minute;
            int idtsecond = dt.Second;
            dt = popUpCalendar.SelectedDate;
            dt = dt.AddHours(Convert.ToDouble(idthour));
            dt = dt.AddMinutes(Convert.ToDouble(idtminute));
            dt = dt.AddSeconds(Convert.ToDouble(idtsecond));

            SplitDateTime();

            double dw = GetTBWidth(iChildrenIndex);
            GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);

            string strFirstChar = tbk.Text;
            if (!(strFirstChar[0] >= 48 && strFirstChar[0] <= 57))
            {
                bdHighlight.Width = GetTBWidth(iChildrenIndex);
                bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
            }
            else
                if (buttonClicked == "Right" || buttonClicked == "Up" || buttonClicked == "Down")
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                }
                else if (buttonClicked == "Left" || buttonClicked == "Up" || buttonClicked == "Down")
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * (iSelectedChar + 1)));
                }
                else
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                }

            ReturnDTE();
            bPopUpCalendarClicked = true;
        }

        /// <summary>
        /// When Date selected from PopUp Calendar, this control should collapsed using this function.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PopUpCalendar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (bPopUpCalendarClicked == true && Value == popUpCalendar.Date)
            {
                cCalendarControl.IsOpen = false;
                bPopUpCalendarClicked = false;
            }
        }

        /// <summary>
        /// Sets the size of the position for large.
        /// </summary>
        private void SetPositionForLargeSize()
        {
            if (buttonClicked == "Right")
            {
                if (GetTBPosition(tbk) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - 25)
                {
                    RemoveFirstChild();

                    if (iChildrenIndex < spDT.Children.Count - 1)
                    {
                        if (GetTBPosition(tbk) + (spDT.Children[iChildrenIndex + 1] as TextBlock).ActualWidth < brdrDateTime.ActualWidth - (brdrDateTime.ActualWidth*.2))
                        {
                            RemoveFirstChild();
                        }
                    }
                }
            }
            else if (buttonClicked == "Left")
            {
                string strDT = (spDT.Children[iChildrenIndex] as TextBlock).Tag.ToString();

                if ((iChildrenIndex == 1 || iChildrenIndex == 0) &&
                    (((strDT == "d" || strDT == "dd" || strDT == "y"
                    || strDT == "yy" || strDT == "yyy" || strDT == "yyyy" || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h"
                    || strDT == "hh" || strDT == "H" || strDT == "HH" || strDT == "m" || strDT == "mm" || strDT == "s" || strDT == "ss" || strDT == "S"
                    || strDT == "SS" || strDT == "f" || strDT == "ff" || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF" || strDT == "t"
                    || strDT == "tt") && (iSelectedChar == 0)) ||
                    (strDT == "M" || strDT == "MM" || strDT == "MMM" || strDT == "MMMM")))
                {
                    MoveCursorLeft();
                }
            }
        }

        /// <summary>
        /// Removes the first child.
        /// </summary>
        private void RemoveFirstChild()
        {
            spDT.Children.RemoveAt(0);
            csDT.Width = spDT.ActualWidth;
            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbk));
        }

        /// <summary>
        /// Moves the cursor left.
        /// </summary>
        private void MoveCursorLeft()
        {
            for (int i = strTmpDT.Length - 1; i >= 0; i--)
            {
                for (int j = spDT.Children.Count - 1; j >= 0; j--)
                {
                    if (strTmpDT[i] == (spDT.Children[iChildrenIndex] as TextBlock).Tag.ToString())
                    {
                        if (i - 2 >= 0)
                        {
                            string strDT = strTmpDT[i - 1];
                            if ((iChildrenIndex == 0) && !(strDT == "d" || strDT == "dd" || strDT == "y"
                                || strDT == "yy" || strDT == "yyy" || strDT == "yyyy" || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h"
                                || strDT == "hh" || strDT == "H" || strDT == "HH" || strDT == "m" || strDT == "mm" || strDT == "s" || strDT == "ss" || strDT == "S"
                                || strDT == "SS" || strDT == "f" || strDT == "ff" || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF" || strDT == "t"
                                || strDT == "tt"))
                            {
                                TextBlock tt = new TextBlock();
                                try
                                {
                                    tt.Text = dt.ToString(strTmpDT[i - 1], Culture);
                                }
                                catch (Exception)
                                {
                                    tt.Text = strTmpDT[i - 1];
                                }

                                if (strDT == "d" || strDT == "dd" || strDT == "M" || strDT == "MM" || strDT == "MMM" || strDT == "MMMM" || strDT == "y" || strDT == "yy"
                                    || strDT == "yyy" || strDT == "yyyy" || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h" || strDT == "hh"
                                    || strDT == "H" || strDT == "HH" || strDT == "m" || strDT == "mm" || strDT == "s" || strDT == "ss" || strDT == "S" || strDT == "SS"
                                    || strDT == "f" || strDT == "ff" || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF" || strDT == "t" || strDT == "tt")
                                {
                                    tt.Cursor = Cursors.IBeam; /*  Sets Character Cursor for Valid Values    */
                                }
                                else
                                {
                                    tt.Cursor = Cursors.Arrow; /*  Sets Arrow Cursor for InValid Value */
                                }

                                tt.MouseEnter += new MouseEventHandler(TBk_MouseEnter);
                                tt.MouseLeftButtonDown += new MouseButtonEventHandler(TBk_MouseLeftButtonDown);
                                tt.MouseMove += new MouseEventHandler(TBk_MouseMove);
                                bdHighlight.MouseLeftButtonDown += new MouseButtonEventHandler(bdHighlight_MouseLeftButtonDown);

                                tt.Tag = strTmpDT[i - 1];
                                tt.FontSize = FontSize;
                                spDT.Children.Insert(0, tt);
                            }

                            TextBlock t = new TextBlock();
                            try
                            {
                                t.Text = dt.ToString(strTmpDT[i - 2], Culture);
                            }
                            catch (Exception)
                            {
                                t.Text = strTmpDT[i - 2];
                            }

                            t.Tag = strTmpDT[i - 2];
                            t.FontSize = FontSize;

                            if (strDT == "d" || strDT == "dd" || strDT == "M" || strDT == "MM" || strDT == "MMM" || strDT == "MMMM" || strDT == "y" || strDT == "yy" || strDT == "yyy"
                            || strDT == "yyyy" || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h" || strDT == "hh" || strDT == "H"
                            || strDT == "HH" || strDT == "m" || strDT == "mm" || strDT == "s" || strDT == "ss" || strDT == "S" || strDT == "SS" || strDT == "f" || strDT == "ff"
                            || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF" || strDT == "t" || strDT == "tt")
                            {
                                t.Cursor = Cursors.IBeam; /*  Sets Character Cursor for Valid Values    */
                            }
                            else
                            {
                                t.Cursor = Cursors.Arrow; /*  Sets Arrow Cursor for InValid Value */
                            }

                            t.MouseEnter += new MouseEventHandler(TBk_MouseEnter);
                            t.MouseLeftButtonDown += new MouseButtonEventHandler(TBk_MouseLeftButtonDown);
                            t.MouseMove += new MouseEventHandler(TBk_MouseMove);
                            bdHighlight.MouseLeftButtonDown += new MouseButtonEventHandler(bdHighlight_MouseLeftButtonDown);

                            spDT.Children.Insert(0, t);

                            int columnindex = 1;
                            if (leftFlowDirection)
                            {
                                columnindex = 0;
                            }

                            if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                            {
                                double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - (brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth) - 5;
                                if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                                {
                                    bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                                }
                            }
                            else
                            {
                                bdHighlight.Width = GetTBWidth(iChildrenIndex);
                            }

                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                            iCIndex = iChildrenIndex;
                        }

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// When Up / Down button clicked, or scrolled, then the value get updated.
        /// </summary>
        private void ChangeDTValue()
        {
            /*  When key Up or Down is pressed from Valid Format, then Increment or Decrement done  */
            dtPat = dt;
            if (iHH == 0)
            {
                if (tbk.Tag.ToString() == "d" || tbk.Tag.ToString() == "dd" || tbk.Tag.ToString() == "dddd" || tbk.Tag.ToString() =="ddd")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddDays(StepValue) : dtPat.AddDays(-StepValue);
                }
                else if (tbk.Tag.ToString() == "M" || tbk.Tag.ToString() == "MM" || tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddMonths(StepValue) : dtPat.AddMonths(-StepValue);
                }
                else if (tbk.Tag.ToString() == "y" || tbk.Tag.ToString() == "yy" || tbk.Tag.ToString() == "yyy" || tbk.Tag.ToString() == "yyyy" || tbk.Tag.ToString() == "Y" || tbk.Tag.ToString() == "YY" || tbk.Tag.ToString() == "YYY" || tbk.Tag.ToString() == "YYYY")
                {
                    if (buttonClicked == "Up")
                    {
                        if (dtPat.Year < Culture.Calendar.MaxSupportedDateTime.Year)
                        {
                            dt = buttonClicked == "Up" ? dtPat.AddYears(StepValue) : dtPat.AddYears(-StepValue);
                        }
                        else
                        {
                            Pattern = maxminLimitDisplay;
                            return;
                        }
                    }
                    else if (buttonClicked == "Down")
                    {
                        if (dtPat.Year > Culture.Calendar.MinSupportedDateTime.Year)
                        {
                            dt = buttonClicked == "Up" ? dtPat.AddYears(-StepValue) : dtPat.AddYears(-StepValue);
                        }
                        else
                        {
                            Pattern = maxminLimitDisplay;
                            return;
                        }
                    }
                }
                else if (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddHours(StepValue) : dtPat.AddHours(-StepValue);
                }
                else if (tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddMinutes(StepValue) : dtPat.AddMinutes(-StepValue);
                }
                else if (tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss" || tbk.Tag.ToString() == "S" || tbk.Tag.ToString() == "SS")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddSeconds(StepValue) : dtPat.AddSeconds(-StepValue);
                }
                else if (tbk.Tag.ToString() == "f" || tbk.Tag.ToString() == "F")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddMilliseconds(StepValue * 100) : dtPat.AddMilliseconds(-StepValue * 100);
                }
                else if (tbk.Tag.ToString() == "ff" || tbk.Tag.ToString() == "FF")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddMilliseconds(StepValue * 10) : dtPat.AddMilliseconds(-StepValue * 10);
                }
                else if (tbk.Tag.ToString() == "fff" || tbk.Tag.ToString() == "FFF")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddMilliseconds(StepValue) : dtPat.AddMilliseconds(-StepValue);
                }
                else if (tbk.Tag.ToString() == "t" || tbk.Tag.ToString() == "tt")
                {
                    dt = buttonClicked == "Up" ? dtPat.AddHours(12) : dtPat.AddHours(-12);
                }
            }
            else if (iHH != 0 && buttonClicked == "Up" && (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH"))
            {
                dt = dtPat.AddHours(DateTime.MaxValue.Hour - dt.Hour);
                iHH = 0;
            }
            else if (iHH != 0 && buttonClicked == "Down" && (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH"))
            {
                dt = dtPat.AddHours(DateTime.MinValue.Hour - dt.Hour);
                iHH = 0;
            }
            else if (iHH != 0 && buttonClicked == "Up" && (tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm"))
            {
                dt = dtPat.AddMinutes(DateTime.MaxValue.Minute - dt.Minute);
                iHH = 0;
            }
            else if (iHH != 0 && buttonClicked == "Down" && (tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm"))
            {
                dt = dtPat.AddMinutes(DateTime.MinValue.Minute - dt.Minute);
                iHH = 0;
            }
            else if (iHH != 0 && buttonClicked == "Up" && (tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss"))
            {
                dt = dtPat.AddSeconds(DateTime.MaxValue.Second - dt.Second);
                iHH = 0;
            }
            else if (iHH != 0 && buttonClicked == "Down" && (tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss"))
            {
                dt = dtPat.AddSeconds(DateTime.MinValue.Second - dt.Second);
                iHH = 0;
            }

            int iTotalChildren = spDT.Children.Count;
            TextBlock tbkCurrentPosition = new TextBlock();
            tbkCurrentPosition = tbk;
            TextBlock tbkFirstChild = new TextBlock();
            tbkFirstChild = spDT.Children[0] as TextBlock;

            SplitDateTime();    /*  After Increment or Decrement, to fetch the value.   Eg: 2009 to 2010, then to set in spDT.  */

            MaintainingPosition(tbkFirstChild, iTotalChildren);
            spDT.UpdateLayout();
            if (!leftFlowDirection)
            {
                double dw = GetTBWidth(iChildrenIndex);
            }
            else
            {
                double dw = GetTBWidth(iTotalChildren-iChildrenIndex-1);        
            }

            csDT.Children.Remove(bdHighlight);

            string strFirstChar;
            if (tbk.Text == string.Empty)
            {
                strFirstChar = " ";
            }
            else
            {
                strFirstChar = tbk.Text;
            }

            if (!(strFirstChar[0] >= 48 && strFirstChar[0] <= 57))
            {
                int columnindex = 1;
                if (leftFlowDirection)
                {
                    columnindex = 0;

                    if (GetTBPosition(tbTmp) + GetTBWidth(iTotalChildren - iChildrenIndex - 1) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                    {
                        double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iTotalChildren - iChildrenIndex - 1)) - (brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth) - 5;
                        if (GetTBWidth(iTotalChildren - iChildrenIndex - 1) > dRemoveWidth)
                        {
                            bdHighlight.Width = GetTBWidth(iTotalChildren - iChildrenIndex - 1) - dRemoveWidth;
                        }
                    }
                    else
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex);
                    }

                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                }
                else
                {
                    if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                    {
                        double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - (brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth) - 5;
                        if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                        {
                            bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                        }
                    }
                    else
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex);
                    }

                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                }
            }
            else
            {
                GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);
                if (buttonClicked == "Right" || buttonClicked == "Up" || buttonClicked == "Down")
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                }
                else if (buttonClicked == "Left" || buttonClicked == "Up" || buttonClicked == "Down")
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + ((iSingChar * iSelectedChar) + 1));
                }
            }

            csDT.Children.Add(bdHighlight);
            if (buttonClicked == "Left" || buttonClicked == "Right")
            {
                SetPositionForLargeSize();
            }
        }

        /// <summary>
        /// Maintainings the position.
        /// </summary>
        /// <param name="tbkFirstChild">The TBK first child.</param>
        /// <param name="iTotalChildren">The i total children.</param>
        private void MaintainingPosition(TextBlock tbkFirstChild, int iTotalChildren)
        {
            int ii = -1;

            foreach (TextBlock elem in spDT.Children)
            {
                ii++;
                if (elem.Tag.ToString() == tbkFirstChild.Tag.ToString())
                {
                    spDT.Children.Clear();
                    for (int i = ii; i < ii + iTotalChildren; i++)
                    {
                        TextBlock tt = new TextBlock();
                        try
                        {
                            if (strTmpDT[i] == "d")
                            {
                                tt.Text = int.Parse(dt.ToString("dd", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "M")
                            {
                                tt.Text = int.Parse(dt.ToString("MM", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "y")
                            {
                                tt.Text = int.Parse(dt.ToString("yy", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "h")
                            {
                                tt.Text = int.Parse(dt.ToString("hh", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "H")
                            {
                                tt.Text = int.Parse(dt.ToString("HH", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "m")
                            {
                                tt.Text = int.Parse(dt.ToString("mm", Culture)).ToString();
                            }
                            else if (strTmpDT[i] == "s")
                            {
                                tbk.Text = int.Parse(dt.ToString("ss", Culture)).ToString();
                            }
                            else
                            {
                                if (strTmpDT[i] == "T")
                                {
                                    tt.Text = strTmpDT[i];
                                }
                                else if (strTmpDT[i] == "GMT")
                                {
                                    tt.Text = "GMT";
                                }
                                else
                                {
                                    tt.Text = dt.ToString(strTmpDT[i], Culture);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            if (i <= strTmpDT.Count() - 1)
                            {
                                tt.Text = strTmpDT[i];
                            }
                        }

                        if (i < strTmpDT.Count())
                        {
                            tt.Tag = strTmpDT[i];
                            string strDT = strTmpDT[i];

                            tt.FontSize = FontSize;

                            if (strDT == "d" || strDT == "dd" || strDT == "M" || strDT == "MM" || strDT == "MMM" || strDT == "MMMM" || strDT == "y" || strDT == "yy" || strDT == "yyy"
                                || strDT == "yyyy" || strDT == "Y" || strDT == "YY" || strDT == "YYY" || strDT == "YYYY" || strDT == "h" || strDT == "hh" || strDT == "H"
                                || strDT == "HH" || strDT == "m" || strDT == "mm" || strDT == "s" || strDT == "ss" || strDT == "S" || strDT == "SS" || strDT == "f" || strDT == "ff"
                                || strDT == "fff" || strDT == "F" || strDT == "FF" || strDT == "FFF" || strDT == "t" || strDT == "tt")
                            {
                                tt.Cursor = Cursors.IBeam; /*  Sets Character Cursor for Valid Values    */
                            }
                            else
                            {
                                tt.Cursor = Cursors.Arrow; /*  Sets Arrow Cursor for InValid Value */
                            }

                            tt.MouseEnter += new MouseEventHandler(TBk_MouseEnter);
                            tt.MouseLeftButtonDown += new MouseButtonEventHandler(TBk_MouseLeftButtonDown);
                            tt.MouseMove += new MouseEventHandler(TBk_MouseMove);

                            spDT.Children.Add(tt);
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// When timer gets started, then Calendar and None popup will show.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void DptPopUpTimer_Tick(object sender, EventArgs e)
        {
            /*  PopUp Timer Tick    */
            cPopUp.IsOpen = true;
            dptPopUpTimer.Stop();
            closePopup.Start();
        }

        /// <summary>
        /// To check whether next or previous block is valid value or invalid value, using
        /// textblock position and direction (Left / Right)
        /// </summary>
        /// <param name="ici">Passes index value on which text block of current
        /// position.</param>
        /// <param name="strLeftRight">passing whether Left or Right direction to move the
        /// position.</param>
        private void FindChildNextBlock(int ici, string strLeftRight)
        {
            /*  To Check whether Next or Previous block is Valid or Invalid Format  */
            int i = -1;
            foreach (TextBlock element in spDT.Children)
            {
                i += 1;
                if (ici == i)
                {
                    string strFirstPattern = element.Tag.ToString();
                    if (!(strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" ||
                    strFirstPattern[0].ToString() == "y" || strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" ||
                    strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" || strFirstPattern[0].ToString() == "s" ||
                    strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                    strFirstPattern[0].ToString() == "t"))
                    {
                        if (strLeftRight == "Right")
                        {
                            iChildrenIndex += 1;
                            eGetPosition = -1;
                            GetTBWidth(iChildrenIndex);
                            bNewNlock = true;
                            break;
                        }
                        else if (strLeftRight == "Left")
                        {
                            iChildrenIndex -= 1;
                            iCIndex = iChildrenIndex;
                            eGetPosition = GetTBWidth(iChildrenIndex) - 1;
                            bNewNlock = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Getting single character width from a text block. Setting total number of
        /// characters in a text block. Focusing character position can find.
        /// </summary>
        /// <param name="tbtext">Passing current textblock to get Text, Length</param>
        /// <param name="dtbfullwidth">Passing textblock width</param>
        /// <param name="dtbgetpos">Passing mouse clicked position. using this we have to
        /// find which character to focus.</param>
        /// <returns>
        /// Type : double
        /// </returns>
        private double GetSingleCharacterWidth(TextBlock tbtext, double dtbfullwidth, double dtbgetpos)
        {
            /*  Get TextBlock Width of Single Character on Numeric Fields   */
            string chara = tbtext.Text;
            iChara = tbtext.Text.ToCharArray().Length;
            iSingChar = dtbfullwidth / iChara;
            iTmpSingChar = 0;
            iSelectedChar = -1;
            tmpTB = new TextBlock();
            for (int i = 0; i < iChara; i++)
            {
                iSelectedChar += 1;
                if (dtbgetpos <= iSingChar * (i + 1))
                {
                    tmpTB.Text = chara[i].ToString();
                    tmpTB.FontSize = FontSize;
                    return tmpTB.ActualWidth;
                }
                else
                {
                    iTmpSingChar += iSingChar;
                }
            }

            return 0;
        }

        /// <summary>
        /// This function gets TextBlock Width using the index.
        /// </summary>
        /// <param name="ici">Passing the current highlighting position value.</param>
        /// <returns>Type : double</returns>
        private double GetTBWidth(int ici)
        {
            /*  Get TextBlock Width from StackPanel Index   */
            int i = -1;
            if (spDT != null)
            {
                foreach (TextBlock element in spDT.Children)
                {
                    i += 1;
                    if (ici == i)
                    {
                        tbTmp = element;
                        tbk = element;
                        return element.ActualWidth;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// When calendar is clicked, the calendar control will display.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void HlbCalendar_Click(object sender, RoutedEventArgs e)
        {
            Pattern = strPattern;
            cPopUp.IsOpen = false;
            cCalendarControl.Margin = cPopUp.Margin;

            cCalendarControl.HorizontalOffset = 0;
            cCalendarControl.VerticalOffset = 0;
            GeneralTransform gt = cCalendarControl.TransformToVisual(Application.Current.RootVisual as UIElement);
            Point offset=gt.Transform(new Point(0, 0));
            FrameworkElement popupchild=cCalendarControl.Child as FrameworkElement;
            if (leftFlowDirection)
            {
                cCalendarControl.Margin = new Thickness((grdDateTimeEdit.ActualWidth - popupchild.Width), grdDateTimeEdit.ActualHeight, 0, 0);
            }

            if (offset.X+popupchild.Width>Application.Current.Host.Content.ActualWidth)
            {               
                cCalendarControl.HorizontalOffset = -popupchild.Width;
            }

            if (offset.Y + popupchild.Height > Application.Current.Host.Content.ActualHeight)
            {
                cCalendarControl.VerticalOffset = -popupchild.Height;
            }

            cCalendarControl.IsOpen = true;
            scroll = false;
            noneClicked = false;
            Canvas.SetZIndex(this, 1);
        }

        /// <summary>
        /// When None is clicked, DateTime Edit Control will show None string.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void HlbNone_Click(object sender, RoutedEventArgs e)
        {
            Pattern = NonDate;
            cPopUp.IsOpen = false;
            noneClicked = true;
        }

        /// <summary>
        /// This function is used to update the DateTime Edit Control value when Up or Down
        /// button clicked.
        /// </summary>
        /// <param name="strUpDown">This variable gets to check whether Up / Down direction
        /// to move</param>
        private void MouseUpDownArrow(string strUpDown)
        {
            if (tbk != null && sbRectangle != null)
            {
                if (strUpDown == "Up")
                {
                    buttonClicked = "Up";
                }
                else
                {
                    buttonClicked = "Down";
                }
            }
            else
            {
                iChildrenIndex = -1;
                foreach (TextBlock element in spDT.Children)
                {
                    iChildrenIndex += 1;
                    string s = element.Tag.ToString();
                    if (element.Tag.ToString() == "dd")
                    {
                        tbk = element;
                        SetRectangleAnimation(1);
                        break;
                    }
                    else if ((element.Tag.ToString() == "GMT" || element.Tag.ToString() == "Z" || element.Tag.ToString() == "T" || element.Tag.ToString() == "ddd" ||
                        element.Tag.ToString() == "dddd" || element.Tag.ToString() == "t" || element.Tag.ToString() == "tt") || (!(s[0].ToString() == "D" || s[0].ToString() == "M" ||
                        s[0].ToString() == "y" || s[0].ToString() == "Y" || s[0].ToString() == "h" || s[0].ToString() == "H" || s[0].ToString() == "m" || s[0].ToString() == "s" ||
                        s[0].ToString() == "S" || s[0].ToString() == "f" || s[0].ToString() == "F" || s[0].ToString() == "t")))
                    {
                    }
                    else
                    {
                        tbk = element;
                        SetRectangleAnimation(1);
                        break;
                    }
                }
            }

            ChangeDTValue();
            ReturnDTE();    /*  Sets the Modified Date in "Value" Dependency Property   */
        }

        /// <summary>
        /// This function is used to move position of the cursor on Left or Right direction.
        /// </summary>
        /// <param name="strLeftRight">Passing string as &quot;Right&quot; /
        /// &quot;Left&quot; direction</param>
        private void MoveLeftRightPosition(string strLeftRight)
        {
            /*  To Move the Cursor Position from Left or Right in Valid Format  */
            if (sbRectangle != null)
            {
                sbRectangle.Stop();
            }

            bdHighlight = new Border();
            bdHighlight.MouseLeftButtonDown += new MouseButtonEventHandler(bdHighlight_MouseLeftButtonDown);
            bdHighlight.Background = new SolidColorBrush(Colors.Transparent);
            bdHighlight.BorderBrush = new SolidColorBrush(Colors.Black);
            bdHighlight.Height = brdrDateTime.ActualHeight <= tbk.ActualHeight ? brdrDateTime.ActualHeight : tbk.ActualHeight;

            if (tbk.Tag.ToString() == "ddd" || tbk.Tag.ToString() == "dddd")
            {
                if (strLeftRight == "Left" && iChildrenIndex - 2 >= 0)
                {
                    iChildrenIndex -= 2;
                }
                else if (strLeftRight == "Left" && iChildrenIndex == 0)
                {
                    iChildrenIndex += 2;

                    eGetPosition = -1;
                }

                if (strLeftRight == "Right")
                {
                    iChildrenIndex += 2;
                    eGetPosition = -1;
                }
            }

            GetTBWidth(iChildrenIndex);

            if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt") || tbk.Tag.ToString() == "GMT")
            {
                GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);
            }

            if (strLeftRight == "Left")
            {
                if (bNewNlock == true)
                {
                    if (tbk.Tag.ToString() == "ddd" || tbk.Tag.ToString() == "dddd")
                    {
                        bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                        return;
                    }

                    if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt"))
                    {
                        bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                    }
                    else
                    {
                        int columnindex = 1;
                        if (leftFlowDirection)
                        {
                            columnindex = 0;
                        }

                        if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                        {
                            double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth - 5;
                            if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                            {
                                bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                            }
                        }
                        else
                        {
                            bdHighlight.Width = GetTBWidth(iChildrenIndex);
                        }

                        bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                    }

                    bNewNlock = false;
                }
                else if (iSelectedChar >= 0)
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * (iSelectedChar == 0 ? iSelectedChar : iSelectedChar)));
                }
            }
            else if (strLeftRight == "Right")
            {
                if (tbk.Tag.ToString() == "GMT" || tbk.Tag.ToString() == "Z")
                {
                    GetTBWidth(iChildrenIndex -= 2);
                    iSelectedChar = tbk.Text.Length - 1;
                    sbRectangle.Begin();
                    return;
                }

                if (iCIndex != iChildrenIndex)
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                    iSelectedChar = -1;
                }
                else if (iSelectedChar <= iChara - 1)
                {
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * (bNewNlock == true && iSelectedChar == 0 ? 0 : iSelectedChar + 1)));
                }

                bNewNlock = false;
                eGetPosition += iSingChar;
            }

            if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt")
            {
                int columnindex = 1;
                if (leftFlowDirection)
                {
                    columnindex = 0;
                }

                if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                {
                    double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth - 5;
                    if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                    }
                }
                else
                {
                    bdHighlight.Width = GetTBWidth(iChildrenIndex);
                }

                iSelectedChar = (int)iChara;
            }
            else
            {
                bdHighlight.Width = iSingChar;
            }

            SetAnimation();

            SetPositionForLargeSize();

            csDT.Children.Add(bdHighlight);

            if (strLeftRight == "Left")
            {
                if (iCIndex != iChildrenIndex)
                {
                    iSelectedChar = (int)iChara - 1;
                }
                else
                {
                    if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt"))
                    {
                        if (iSelectedChar < 0)
                        {
                            iSelectedChar -= 1;
                        }
                    }
                }
            }
            else if (strLeftRight == "Right")
            {
                if (iCIndex != iChildrenIndex)
                {
                    iSelectedChar = 0;
                }
                else
                {
                    if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt"))
                    {
                        iSelectedChar += 1;
                    }
                }
            }

            tbGlobal = tbk;
            csDT.Children.Clear();
            csDT.Children.Add(bdHighlight);
        }

        /// <summary>
        /// This function is used to update the DateTime Edit Control values, when mouse
        /// scrolled.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">Maintaining the mouse scrolling events</param>
        private void OnScroll(object sender, HtmlEventArgs args)
        {
            /*  Mouse Scroll Events */
            if (scroll && cCalendarControl.IsOpen==false)
            {
                if (sbRectangle != null)
                {
                    double delta = 0;
                    ScriptObject e = args.EventObject;
                    if (e.GetProperty("detail") != null)
                    {
                        delta = (double)e.GetProperty("detail");
                    }
                    else if (e.GetProperty("wheelDelta") != null)
                    {
                        delta = (double)e.GetProperty("wheelDelta");
                    }

                    delta = Math.Sign(delta);

                    if (delta > 0)
                    {
                        MouseUpDownArrow("Up");
                    }

                    if (delta < 0)
                    {
                        MouseUpDownArrow("Down");
                    }
                }
            }

            if (Pattern == NonDate || Pattern== maxminLimitDisplay)
            {
                this.SetTodaysDate();
            }
        }

        /// <summary>
        /// Sets the todays date.
        /// </summary>
        private void SetTodaysDate()
        {
            Pattern = strPattern;
            dt = DateTime.Now.Date;
            dt = dt.AddHours(Convert.ToDouble(DateTime.Now.Hour));
            dt = dt.AddMinutes(Convert.ToDouble(DateTime.Now.Minute));
            dt = dt.AddSeconds(Convert.ToDouble(DateTime.Now.Second));
            SplitDateTime();
            double dw = GetTBWidth(iChildrenIndex);
            GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);

            string strFirstChar = tbk.Text;
            if (!(strFirstChar[0] >= 48 && strFirstChar[0] <= 57))
            {
                bdHighlight.Width = GetTBWidth(iChildrenIndex);
                bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
            }
            else
            {
                bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
            }

            ReturnDTE();
        }

        /// <summary>
        /// For each and every change on DateTime Edit Control, the value return to user
        /// using this function.
        /// </summary>
        private void ReturnDTE()
        {
            /*  Assigns DateTime Value  */
            string strReturnValue = string.Empty;
            foreach (object obj in spDT.Children)
            {
                TextBlock tbreturnvalue = obj as TextBlock;
                strReturnValue += tbreturnvalue.Text;
            }

            try
            {
            if (strReturnValue != NonDate && strReturnValue != maxminLimitDisplay)
            {
                Value = DateTime.ParseExact(strReturnValue, strPattern, Culture, DateTimeStyles.None);
                popUpCalendar.Date = Value;
            }
            }
            catch (FormatException)
            {
            }
        }

        /// <summary>
        /// The Animation is set using this function for Color.
        /// </summary>
        private void SetAnimation()
        {
            /*  To Set ColorAnimation for Highlight using StoryBoard    */
            caAnimation = new ColorAnimation();
            caAnimation.To = CaretBackground;
            caAnimation.AutoReverse = true;
            caAnimation.RepeatBehavior = RepeatBehavior.Forever;
            caAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400));
            sbRectangle = new Storyboard();
            Storyboard.SetTarget(caAnimation, (Border)bdHighlight);
            Storyboard.SetTargetProperty(caAnimation, new PropertyPath("(bdHighlight.Background).(SolidColorBrush.Color)"));
            sbRectangle.Children.Add(caAnimation);
            if (sbRectangle != null)
            {
                sbRectangle.Begin();
            }
        }

        /// <summary>
        /// This function is called only when value updated.
        /// </summary>
        private void SetDTEDefaultValue()
        {
            /*  On Initial DateTimeEdit Control Value Sets  */
            try
            {
                tbGlobal = tbk;
                dt = Value;
                SplitDateTime();
                tbk = tbGlobal;
                double dw;
                if (!leftFlowDirection)
                {
                     dw = GetTBWidth(iChildrenIndex);
                }
                else
                {
                   dw= GetTBWidth(spDT.Children.Count-iChildrenIndex-1);
                }

                if (tbk != null)
                {
                    string strFirstChar = tbk.Text;
                    if (!(strFirstChar[0] >= 48 && strFirstChar[0] <= 57))
                    {
                        int columnindex = 1;
                        if (leftFlowDirection)
                        {
                            columnindex = 0;
                            if (GetTBPosition(tbTmp) + dw > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                            {
                                double dRemoveWidth = (GetTBPosition(tbTmp) + dw) - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth - 5;
                                if (dw > dRemoveWidth)
                                {
                                    bdHighlight.Width = dw - dRemoveWidth;
                                }
                            }
                            else
                            {
                                bdHighlight.Width = dw;
                            }

                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                        }
                        else
                        {
                            if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                            {
                                double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth - 5;
                                if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                                {
                                    bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                                }
                            }
                            else
                            {
                                bdHighlight.Width = GetTBWidth(iChildrenIndex);
                            }

                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                        }
                    }
                    else
                    {
                        if (bdHighlight != null)
                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                        else
                            dt = Value;
                    }
                }
                else
                {
                    dt = Value;
                }
            }
            catch
            {
                dt = DateTime.Now;
            }
        }

        /// <summary>
        /// Setting the position of cursor in DateTime Edit Control. If invalid character is
        /// entered, then foreground color will change.
        /// </summary>
        /// <param name="invalid">This variable is used to check whether entered value is
        /// invalid or valid, regarding that foreground color will change.</param>
        private void SetHighlighPosition(bool invalid)
        {
            int iTotalChildren;
            TextBlock tbkFirstChild = new TextBlock();

            double dw = GetTBWidth(iChildrenIndex);

            if ((!(tbk.Tag.ToString() == "f" || tbk.Tag.ToString() == "F" || tbk.Tag.ToString() == "ff" || tbk.Tag.ToString() == "FF" || tbk.Tag.ToString() == "fff" || tbk.Tag.ToString() == "FFF")) && (invalid == true))
            {
                tbk.Foreground = IncorrectForeground;
            }

            if (iChildrenIndex <= spDT.Children.Count)
            {
                for (int iCI = 0; iCI <= iChildrenIndex; iCI++)
                {
                    if (iCI == iChildrenIndex)
                    {
                        if (spDT.Children.Count - 1 == iChildrenIndex && (iSelectedChar == tbk.Text.Length - 1 || tbk.Tag.ToString() == "t" || tbk.Tag.ToString() == "tt"))
                        {
                            bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                            bdHighlight.Width = iSingChar;
                            return;
                        }

                        if (tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt")
                        {
                            break;
                        }
                        else
                        {
                            if (iSelectedChar < tbk.Text.Length - 1)
                            {
                                iCIndex = iChildrenIndex;
                                bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * (iSelectedChar + 1)));
                                eGetPosition += iSingChar;
                                iSelectedChar += 1;

                                buttonClicked = "Right";
                                SetPositionForLargeSize();

                                iTotalChildren = iChildrenIndex;
                                tbkFirstChild = spDT.Children[0] as TextBlock;

                                if (GetTBPosition(tbk) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - 25)
                                {
                                    MaintainingPosition(tbkFirstChild, iTotalChildren);
                                }

                                return;
                            }
                            else if (iSelectedChar == tbk.Text.Length - 1 && iCI == spDT.Children.Count - 1)
                            {
                                return;
                            }
                        }
                    }
                }

                iChildrenIndex += 1;
                if (iChildrenIndex == spDT.Children.Count - 1)
                {
                    GetTBWidth(iChildrenIndex);

                    string strFirstPattern = tbk.Tag.ToString();
                    if (!(strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" || strFirstPattern[0].ToString() == "y" ||
                        strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" || strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" ||
                        strFirstPattern[0].ToString() == "s" || strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                        strFirstPattern[0].ToString() == "t"))
                    {
                        iChildrenIndex -= 1;
                        GetTBWidth(iChildrenIndex);
                        eGetPosition = GetTBWidth(iChildrenIndex) - 1;
                        iSelectedChar = tbk.Text.Length - 1;
                        bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                        bdHighlight.Width = iSingChar;
                        return;
                    }
                }

                FindChildNextBlock(iChildrenIndex, "Right");

                if (tbk.Tag.ToString() == "GMT" && spDT.Children.Count - 1 == iChildrenIndex)
                {
                    iChildrenIndex -= 2;
                    GetTBWidth(iChildrenIndex);
                    eGetPosition = GetTBWidth(iChildrenIndex) - 1;
                    iSelectedChar = tbk.Text.Length - 1;
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * iSelectedChar));
                    bdHighlight.Width = iSingChar;
                    return;
                }
                else if (!(tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM" || tbk.Tag.ToString() == "tt"))
                {
                    GetTBWidth(iChildrenIndex);
                    GetSingleCharacterWidth(tbTmp, tbTmp.ActualWidth, eGetPosition);
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp) + (iSingChar * ((iSelectedChar == 0) ? iSelectedChar : iSelectedChar + 1)));
                    bdHighlight.Width = iSingChar;
                }
                else
                {
                    bdHighlight.Width = GetTBWidth(iChildrenIndex);
                    bdHighlight.SetValue(Canvas.LeftProperty, GetTBPosition(tbTmp));
                }

                buttonClicked = "Right";
                SetPositionForLargeSize();

                iTotalChildren = iChildrenIndex;
                tbkFirstChild = spDT.Children[0] as TextBlock;

                if (GetTBPosition(tbk) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - 25)
                {
                    MaintainingPosition(tbkFirstChild, iTotalChildren);
                }
            }
        }

        /// <summary>
        /// When Incorrect value is given, then using this function, foreground will change
        /// regarding block position.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        private void SetInvalidFields(DependencyPropertyChangedEventArgs e)
        {
            if (spDT != null)
            {
                foreach (TextBlock element in spDT.Children)
                {
                    if (element.Foreground.Equals(e.OldValue) == true)
                    {
                        element.Foreground = IncorrectForeground;
                    }
                }
            }
        }

        /// <summary>
        /// This function sets Rectangle shape border for cursor in DateTime Edit Control.
        /// </summary>
        /// <param name="egetpos">This variable sets the position on which the cursor should focus.</param>
        private void SetRectangleAnimation(double egetpos)
        {
            /*  To set Highlight Animation for Selected Position    */
            string strFirstChar = tbk.Text;
            string strFirstPattern = tbk.Tag.ToString();

            if (!(strFirstPattern[0].ToString() == "d" || strFirstPattern[0].ToString() == "D" || strFirstPattern[0].ToString() == "M" || strFirstPattern[0].ToString() == "y" ||
                strFirstPattern[0].ToString() == "Y" || strFirstPattern[0].ToString() == "h" || strFirstPattern[0].ToString() == "H" || strFirstPattern[0].ToString() == "m" ||
                strFirstPattern[0].ToString() == "s" || strFirstPattern[0].ToString() == "S" || strFirstPattern[0].ToString() == "f" || strFirstPattern[0].ToString() == "F" ||
                strFirstPattern[0].ToString() == "t"))
            {
                return;
            }

            if (sbRectangle != null)
            {
                sbRectangle.Stop();
            }

            double dw = GetTBWidth(iChildrenIndex);
            if (tbTmp != null)
            {
                tbk = tbTmp;
            }

            double dtbp = GetTBPosition(tbk);

            bdHighlight = new Border();
            bdHighlight.Name = "bdHighlight";
            bdHighlight.MouseLeftButtonDown += new MouseButtonEventHandler(bdHighlight_MouseLeftButtonDown);
            bdHighlight.Background = new SolidColorBrush(Colors.Transparent);
            bdHighlight.BorderBrush = new SolidColorBrush(Colors.Black);
            bdHighlight.Height = brdrDateTime.ActualHeight <= tbk.ActualHeight ? brdrDateTime.ActualHeight : tbk.ActualHeight;

            bdHighlight.Cursor = Cursors.IBeam;
            int columnindex = 1;
            if (leftFlowDirection)
            {
                columnindex = 0;
            }

            if (strFirstChar[0] >= 48 && strFirstChar[0] <= 57)
            {
                if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                {
                    double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - (brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth) - 5;

                    if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                    }
                    else
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex);
                    }
                }
                else
                {
                    bdHighlight.Width = GetSingleCharacterWidth(tbk, tbk.ActualWidth, egetpos);
                }
            }
            else
            {
                if (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth)
                {
                    double dRemoveWidth = (GetTBPosition(tbTmp) + GetTBWidth(iChildrenIndex)) - (brdrDateTime.ActualWidth - grdDateTimeEdit.ColumnDefinitions[columnindex].ActualWidth) - 5;                 
                    if (GetTBWidth(iChildrenIndex) > dRemoveWidth)
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex) - dRemoveWidth;
                    }
                    else
                    {
                        bdHighlight.Width = GetTBWidth(iChildrenIndex);
                    }
                }
                else
                {
                    bdHighlight.Width = GetTBWidth(iChildrenIndex);
                }
            }

            if (strFirstChar[0] >= 48 && strFirstChar[0] <= 57)
            {
                    bdHighlight.SetValue(Canvas.LeftProperty, dtbp + iTmpSingChar);
            }
            else
            {
                bdHighlight.SetValue(Canvas.LeftProperty, dtbp);
            }

            SetAnimation();
            csDT.Height = spDT.ActualHeight;
            csDT.Width = spDT.ActualWidth;
            while (csDT.Children.Count > 0)
            {
                csDT.Children.RemoveAt(csDT.Children.Count - 1);
            }

            csDT.Children.Add(bdHighlight);
            cPopUp.Margin = new Thickness(0, grdDateTimeEdit.ActualHeight, 0, 0);          
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the bdHighlight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void bdHighlight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                    GetTBPosition(tbk);
                    eGetPosition = e.GetPosition(tbk).X;
                    SetRectangleAnimation(eGetPosition);

                e.Handled = true;
                if (bdHighlight != null)
                {
                    Focus();
                }
            }
        }

        /// <summary>
        /// Setting PopUP Dispatcher Timer to start for Calendar and None.
        /// </summary>
        private void ShowPopUpDispatcherTimer()
        {
            /*  Show PopUp Window using Timer   */
            if (!leftFlowDirection)
            {
                cPopUp.Margin = new Thickness(0, grdDateTimeEdit.ActualHeight, 0, 0);
                spPopUp.Children.Clear();
                spPopUp.Children.Add(hlbCalendar);
                spPopUp.Children.Add(hlbNone);
            }
            else
            {
                FrameworkElement popupchild = cPopUp.Child as FrameworkElement;
                cPopUp.Margin = new Thickness((grdDateTimeEdit.ActualWidth - popupchild.Width), grdDateTimeEdit.ActualHeight, 0, 0);
                spPopUp.Children.Clear();
                spPopUp.Children.Add(hlbNone);
                spPopUp.Children.Add(hlbCalendar);
            }

            dptPopUpTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)PopUpDelay);
            dptPopUpTimer.Tick += new EventHandler(DptPopUpTimer_Tick);
            dptPopUpTimer.Start();
        }

        /// <summary>
        /// Handles the Tick event of the closePopup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void closePopup_Tick(object sender, EventArgs e)
        {
            if (cPopUp.IsOpen == true && hlbCalendar.IsMouseOver==false && hlbNone.IsMouseOver==false)
            {
                cPopUp.IsOpen = false;
            } 
        }

        /// <summary>
        /// When Calendar Control left the focus using mouse, to disable popup, this
        /// function is used.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void SpCalendarControl_MouseLeave(object sender, MouseEventArgs e)
        {
            dptPopUpTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            dptPopUpTimer.Stop();
            scroll = false;
        }

        /// <summary>
        /// When Calendar Control moves using mouse cursor, the PopUp will enable using this
        /// function.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void SpCalendarControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (sbRectangle != null)
            {
               // ShowPopUpDispatcherTimer();
            }

            scroll = false;
        }

        /// <summary>
        /// This function is Most important function, its mainly used to check the selected
        /// pattern and assigns to Pattern Dependency Property.
        /// <para>First checks for Pattern with None, what pattern to set.</para>
        /// <para>Using the Regex, splits each blocks and send to AddDTPattern function to
        /// create dynamic textblock and insert into Stackpanel.</para>
        /// <para>Horizontal and Vertical Content Alignment is set using this
        /// function.</para>
        /// </summary>
        [System.ComponentModel.Description("This function is Most important function, its mainly used to check the selected pattern and assigns to Pattern Dependency Property. $para$First checks for Pattern with None, what pattern to set.$para-end$ $para$Using the Regex, splits each blocks and send to AddDTPattern function to create dynamic textblock and insert into Stackpanel.$para-end$ $para$Horizontal and Vertical Content Alignment is set using this function.$para-end$")]
        private void SplitDateTime()
        {
            /*  On Each and Every Value Change, to Clear and Set TextBlocks in StackPanel   */
            if (spDT != null)
            {
                spDT.Children.Clear();

                if (Pattern == NonDate)
                {
                    tbk = new TextBlock();
                    tbk.Text = NonDate;
                    tbk.FontSize = FontSize;
                    tbk.MouseEnter += new MouseEventHandler(TBk_MouseEnter);
                    spDT.Children.Add(tbk);
                    if (leftFlowDirection)
                    {
                        spDT.HorizontalAlignment = HorizontalAlignment.Right;
                        textGrid.HorizontalAlignment = HorizontalAlignment.Right;
                    }

                    return;
                }
                else if (Pattern == maxminLimitDisplay)
                {
                    tbk = new TextBlock();
                    tbk.Text = maxminLimitDisplay;
                    tbk.FontSize = FontSize;
                    tbk.MouseEnter += new MouseEventHandler(TBk_MouseEnter);
                    spDT.Children.Add(tbk);
                    if (leftFlowDirection)
                    {
                        spDT.HorizontalAlignment = HorizontalAlignment.Right;
                        textGrid.HorizontalAlignment = HorizontalAlignment.Right;
                    }

                    return;
                }
                else
                {
                    strPattern = Pattern;
                    if (Pattern == "u")
                    {
                        strPattern = Culture.DateTimeFormat.UniversalSortableDateTimePattern;
                    }
                    else if (Pattern == "s")
                    {
                        strPattern = Culture.DateTimeFormat.SortableDateTimePattern;
                    }
                    else if (Pattern == "r" || Pattern == "R")
                    {
                        strPattern = Culture.DateTimeFormat.RFC1123Pattern;
                    }
                    else if (Pattern == "o" || Pattern == "O")
                    {
                        strPattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzz";
                    }
                    else if (Pattern == "ShortDateFormat")
                    {
                        strPattern = Culture.DateTimeFormat.ShortDatePattern;
                    }
                    else if (Pattern == "LongDateFormat")
                    {
                        strPattern = Culture.DateTimeFormat.LongDatePattern;
                    }
                    else if (Pattern == "ShortTimeFormat")
                    {
                        strPattern = Culture.DateTimeFormat.ShortTimePattern;
                    }
                    else if (Pattern == "LongTimeFormat")
                    {
                        strPattern = Culture.DateTimeFormat.LongTimePattern;
                    }
                    else if (Pattern == "FullDateTimeFormat")
                    {
                        strPattern = Culture.DateTimeFormat.FullDateTimePattern;
                    }
                    else if (Pattern == "MonthDayFormat")
                    {
                        strPattern = Culture.DateTimeFormat.MonthDayPattern;
                    }
                    else if (Pattern == "RFC1123Format")
                    {
                        strPattern = Culture.DateTimeFormat.RFC1123Pattern;
                    }
                    else if (Pattern == "SortableDateTimeFormat")
                    {
                        strPattern = Culture.DateTimeFormat.SortableDateTimePattern;
                    }
                    else if (Pattern == "UniversalSortableDateTimeFormat")
                    {
                        strPattern = Culture.DateTimeFormat.UniversalSortableDateTimePattern;
                    }
                    else if (Pattern == "YearMonthFormat")
                    {
                        strPattern = Culture.DateTimeFormat.YearMonthPattern;
                    }
                    else if (Pattern == "d")
                    {
                        strPattern = Culture.DateTimeFormat.ShortDatePattern;
                    }
                    else if (Pattern == "m" || Pattern == "M")
                    {
                        strPattern = Culture.DateTimeFormat.MonthDayPattern;
                    }
                    else if (Pattern == "y" || Pattern == "Y")
                    {
                        strPattern = Culture.DateTimeFormat.YearMonthPattern;
                    }
                    else if (Pattern == "f")
                    {
                        strPattern = Culture.DateTimeFormat.FullDateTimePattern;
                    }
                    else if (Pattern == "t")
                    {
                        strPattern = Culture.DateTimeFormat.ShortTimePattern;
                    }

                    try
                    {
                        /*  If Arabic Culture   */
                        if (Culture.ToString() == "ar-SA")
                        {
                            dtMinDate = DateTime.Parse("01-January-1318", Culture);
                            long lid = DateDiff(DateInterval.Day, dtMinDate, dt.Date, DayOfWeek.Sunday);
                            long lim = DateDiff(DateInterval.Month, dtMinDate, dt.Date, DayOfWeek.Sunday);
                            long liy = DateDiff(DateInterval.Year, dtMinDate, dt.Date, DayOfWeek.Sunday);
                            dtMaxDate = DateTime.Parse("29-December-1450", Culture);
                            long lad = DateDiff(DateInterval.Day, dtMaxDate, dt.Date, DayOfWeek.Sunday);
                            long lam = DateDiff(DateInterval.Month, dtMaxDate, dt.Date, DayOfWeek.Sunday);
                            long lay = DateDiff(DateInterval.Year, dtMaxDate, dt.Date, DayOfWeek.Sunday);

                            if (lid <= 0 && lim <= 0 && liy <= 0)
                            {
                                dtMaxDate = DateTime.Parse("01-January-1318 00:00:00", Culture);
                                dtPat = dt;
                                dt = dtPat.AddYears(dtMaxDate.Year - dt.Year);
                                dtPat = dt;
                                dt = dtPat.AddMonths(dtMaxDate.Month - dt.Month);
                                dtPat = dt;
                                dt = dtPat.AddDays(dtMaxDate.Day - dt.Day);
                                popUpCalendar.Date = dt;
                            }
                            else if (lad >= 1 && lam >= 0 && lay >= 0)
                            {
                                dtMaxDate = DateTime.Parse("29-December-1450 23:59:59", Culture);
                                dtPat = dt;
                                dt = dtPat.AddDays(dtMaxDate.Day - dt.Day);
                                dtPat = dt;
                                dt = dtPat.AddMonths(dtMaxDate.Month - dt.Month);
                                dtPat = dt;
                                dt = dtPat.AddYears(dtMaxDate.Year - dt.Year);
                                popUpCalendar.Date = dt;
                            }
                        }
                        else
                        {
                            /*  If Not an Arabic Culture    */
                            strDateTime = dt.ToString(strPattern, Culture);
                        }
                    }
                    catch (FormatException)
                    {
                        strDateTime = dt.ToString("dd-MMMM-yyyy hh:mm:ss.ff tt", Culture);
                    }

                    strTempPattern = strPattern.ToCharArray().Where(c => c != '\'').ToArray();
                    string strr = new string(strTempPattern);

                    MatchCollection mcdt = Regex.Matches(strr, "[a-z]+|T|[A-Z]+");

                    /*  To Find Pattern - dd MMMM yyyy hh mm ss ff tt   */
                    int iLengthMtch = 0;
                    strDTPattern = null;
                    strDTPattern = new string[1];
                    iNewSizeFormat = 1;
                    foreach (Match mtch in mcdt)
                    {
                        string strTmpDT1 = strr.Substring(iLengthMtch, mtch.Captures[0].Index - iLengthMtch);
                        AddDTPattern(strTmpDT1);
                        AddDTPattern(mtch.Value);
                        iLengthMtch = mtch.Captures[0].Index + mtch.Captures[0].Length;
                    }

                    AddDTPattern(strr.Substring(iLengthMtch));
                }

                IEnumerable<UIElement> reverseelement;

                if (Culture.ToString() == "ar-SA"|| Culture.ToString() == "he-IL")
                {
                   reverseelement= spDT.Children.Reverse().ToList();
                   spDT.Children.Clear();
                   foreach (TextBlock tempelement in reverseelement)
                   {
                       spDT.Children.Add(tempelement);
                   }
                }

                /*  HorizontalContentAlignment Changed for Month fields.    */
                double iLength = 0;
                foreach (TextBlock element in spDT.Children)
                {
                    iLength += element.ActualWidth;
                }

                csDT.Width = iLength;

                if (brdrDateTime.Width >= iLength)
                {
                    spDT.HorizontalAlignment = TextAlignment;
                    csDT.HorizontalAlignment = TextAlignment;
                }
                else if (brdrDateTime.ActualWidth < iLength)
                {
                    spDT.HorizontalAlignment = HorizontalAlignment.Left;
                    csDT.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
        }

        /// <summary>
        /// DateInterval enum is used to select.
        /// </summary>
        private enum DateInterval
        {
            /// <summary>
            /// Day is selected for DateInterval
            /// </summary>
            Day,

            /// <summary>
            /// DayOfYear is selected for DateInterval
            /// </summary>
            DayOfYear,

            /// <summary>
            /// Hour is selected for DateInterval
            /// </summary>
            Hour,

            /// <summary>
            /// Minute is selected for DateInterval
            /// </summary>
            Minute,

            /// <summary>
            /// Month is selected for DateInterval
            /// </summary>
            Month,

            /// <summary>
            /// Quarter is selected for DateInterval
            /// </summary>
            Quarter,

            /// <summary>
            /// Second is selected for DateInterval
            /// </summary>
            Second,

            /// <summary>
            /// Weekday is selected for DateInterval
            /// </summary>
            Weekday,

            /// <summary>
            /// WeekOfYear is selected for DateInterval
            /// </summary>
            WeekOfYear,

            /// <summary>
            /// Year is selected for DateInterval
            /// </summary>
            Year
        }

        /// <summary>
        /// This function is used to compare two dates and return different between dates.
        /// </summary>
        /// <param name="interval">Interval is used to select on which basis to find difference.</param>
        /// <param name="dt1">This is a First Date.</param>
        /// <param name="dt2">This is a Second Date.</param>
        /// <returns>Type : long in both positive and negative value.</returns>
        private static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        /// <summary>
        /// This function is used to pass month and check on which quarterly comes in a year.
        /// </summary>
        /// <param name="nMonth">Passing month value as numeric</param>
        /// <returns>Type : int</returns>
        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
            {
                return 1;
            }

            if (nMonth <= 6)
            {
                return 2;
            }

            if (nMonth <= 9)
            {
                return 3;
            }

            return 4;
        }

        /// <summary>
        /// This function is used to compare two dates and return different between dates.
        /// </summary>
        /// <param name="interval">Interval is used to select on which basis to find difference.</param>
        /// <param name="dt1">This is a First Date.</param>
        /// <param name="dt2">This is a Second Date.</param>
        /// <param name="eFirstDayOfWeek">Intimating First day of week to check which finding difference</param>
        /// <returns>Type : long</returns>
        private static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            if (interval == DateInterval.Year)
            {
                return dt2.Year - dt1.Year;
            }

            if (interval == DateInterval.Month)
            {
                return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));
            }

            TimeSpan ts = dt2 - dt1;

            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear)
            {
                return Round(ts.TotalDays);
            }

            if (interval == DateInterval.Hour)
            {
                return Round(ts.TotalHours);
            }

            if (interval == DateInterval.Minute)
            {
                return Round(ts.TotalMinutes);
            }

            if (interval == DateInterval.Second)
            {
                return Round(ts.TotalSeconds);
            }

            if (interval == DateInterval.Weekday)
            {
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.WeekOfYear)
            {
                while (dt2.DayOfWeek != eFirstDayOfWeek)
                {
                    dt2 = dt2.AddDays(-1);
                }

                while (dt1.DayOfWeek != eFirstDayOfWeek)
                {
                    dt1 = dt1.AddDays(-1);
                }

                ts = dt2 - dt1;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = GetQuarter(dt1.Month);
                double d2Quarter = GetQuarter(dt2.Month);
                double d1 = d2Quarter - d1Quarter;
                double d2 = 4 * (dt2.Year - dt1.Year);
                return Round(d1 + d2);
            }

            return 0;
        }

        /// <summary>
        /// The date difference function may return exponential values, so this function used to round that values.
        /// </summary>
        /// <param name="dVal">Passing exponential value as double type</param>
        /// <returns>Type : long</returns>
        private static long Round(double dVal)
        {
            if (dVal >= 0)
            {
                return (long)Math.Floor(dVal);
            }

            return (long)Math.Ceiling(dVal);
        }

        /// <summary>
        /// Using this function, when mouse leave from popup, then stops interval and disable PopUp.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        void bPopUp_MouseLeave(object sender, MouseEventArgs e)
        {
            cPopUp.IsOpen = false;
        }

        /// <summary>
        /// Using this function, when mouse leave from popup, then stops interval and disable PopUp.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        void cPopUp_MouseLeave(object sender, MouseEventArgs e)
        {
            dptPopUpTimer.Stop();
            cPopUp.IsOpen = false;
            scroll = false;
        }

        /// <summary>
        /// When Mouse move from the PopUp, call ShowPopUpDispatcherTimer function to set
        /// PopUP Dispatcher Timer to start for Calendar and None.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void SpPopup_MouseMove(object sender, MouseEventArgs e)
        {
            scroll = false;
        }

        /// <summary>
        /// When Mouse get entered into TextBlock then it will call ShowPopUpDispatcherTimer
        /// function to set PopUP Dispatcher Timer to start for Calendar and None.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TBk_MouseEnter(object sender, MouseEventArgs e)
        {
                ShowPopUpDispatcherTimer();
        }

        /// <summary>
        /// When we click on TextBlock at any position, this function first remove children
        /// from canvas.
        /// <para>Then, finds TextBlock Position.</para>
        /// <para>Then, sets children index Position.</para>
        /// <para>Then, gets mouse cursor position on textblock.</para>
        /// <para>Then, calls SetRectangleAnimation function to create cursor.</para>
        /// <para>At last, it will call ShowPopUpDispatcherTimer function to set PopUP
        /// Dispatcher Timer to start for Calendar and None.</para>
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TBk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                    csDT.Children.Remove(bdHighlight);
                    tmpTBk = sender as TextBlock;
                    tbk = tmpTBk;
                    GetTBPosition(tbk);
                    iCIndex = iChildrenIndex;
                    eGetPosition = e.GetPosition(tbk).X;
                    SetRectangleAnimation(eGetPosition);

                ShowPopUpDispatcherTimer();

                e.Handled = true;
                if (bdHighlight != null)
                {
                    Focus();
                }
            }
        }

        /// <summary>
        /// When mouse moves on TextBlock, then it will call ShowPopUpDispatcherTimer
        /// function to set PopUP Dispatcher Timer to start for Calendar and None.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TBk_MouseMove(object sender, MouseEventArgs e)
        {
            ShowPopUpDispatcherTimer();
        }

        /// <summary>
        /// When we are clicking on the DateTime Edit Control, the value will update
        /// regarding previous value and currently entered value for current block.
        /// <para>Then it will call SetHighlighPosition function to set cursor on
        /// position.</para>
        /// </summary>
        /// <param name="dtA">Gets the DateTime value of after key entered.</param>
        /// <param name="dtB">Gets the DateTime value of before key entered.</param>
        private void UpdateDTE(DateTime dtA, DateTime dtB)
        {
            /*  When Numeric Value is Given on Valid Format, then Corresponding Value will Change   */
            int idtB = 0;
            dtPat = dt;
            if (tbk.Tag.ToString() == "d" || tbk.Tag.ToString() == "dd")
            {
                if (dtB.Day == dt.Day)
                {
                    idtB = dtA.Day - dtB.Day;
                }
                else
                {
                    idtB = DateTime.DaysInMonth(dt.Year, dt.Month) - dt.Day;
                }
            }

            if (tbk.Tag.ToString() == "d" || tbk.Tag.ToString() == "dd")
            {
                dt = dtPat.AddDays(idtB);
            }
            else if (tbk.Tag.ToString() == "M" || tbk.Tag.ToString() == "MM" || tbk.Tag.ToString() == "MMM" || tbk.Tag.ToString() == "MMMM")
            {
                dt = dtPat.AddMonths(dtA.Month - dtB.Month);
            }
            else if (tbk.Tag.ToString() == "y" || tbk.Tag.ToString() == "yy" || tbk.Tag.ToString() == "yyy" || tbk.Tag.ToString() == "yyyy" || tbk.Tag.ToString() == "Y" || tbk.Tag.ToString() == "YY" || tbk.Tag.ToString() == "YYY" || tbk.Tag.ToString() == "YYYY")
            {
                dt = dtPat.AddYears(dtA.Year - dtB.Year);
            }
            else if (tbk.Tag.ToString() == "h" || tbk.Tag.ToString() == "hh" || tbk.Tag.ToString() == "H" || tbk.Tag.ToString() == "HH")
            {
                dt = dtPat.AddHours(dtA.Hour - dtB.Hour);
            }
            else if (tbk.Tag.ToString() == "m" || tbk.Tag.ToString() == "mm")
            {
                dt = dtPat.AddMinutes(dtA.Minute - dtB.Minute);
            }
            else if (tbk.Tag.ToString() == "s" || tbk.Tag.ToString() == "ss" || tbk.Tag.ToString() == "S" || tbk.Tag.ToString() == "SS")
            {
                dt = dtPat.AddSeconds(dtA.Second - dtB.Second);
            }
            else if (tbk.Tag.ToString() == "f" || tbk.Tag.ToString() == "F")
            {
                dt = dtPat.AddMilliseconds(dtA.Millisecond - dtB.Millisecond);
            }
            else if (tbk.Tag.ToString() == "ff" || tbk.Tag.ToString() == "FF")
            {
                dt = dtPat.AddMilliseconds(dtA.Millisecond - dtB.Millisecond);
            }
            else if (tbk.Tag.ToString() == "fff" || tbk.Tag.ToString() == "FFF")
            {
                dt = dtPat.AddMilliseconds(dtA.Millisecond - dtB.Millisecond);
            }

            int iTotalChildren = spDT.Children.Count;
            TextBlock tbkCurrentPosition = new TextBlock();
            tbkCurrentPosition = tbk;
            TextBlock tbkFirstChild = new TextBlock();
            tbkFirstChild = spDT.Children[0] as TextBlock;

            if (GetTBPosition(tbk) + GetTBWidth(iChildrenIndex) > brdrDateTime.ActualWidth - 25)
            {
                MaintainingPosition(tbkFirstChild, iTotalChildren);
            }

            SetHighlighPosition(false);
        }
        #endregion
    }
}