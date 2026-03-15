// <copyright file="Calendar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a control that enables the user to select a date using a visual calendar display
    /// that depends on the culture settings.
    /// <para>
    /// The control supports Windows themes ( Default, Silver, Metallic, Zune, Royale and Aero) and skins (Office2003, Office2007Blue,
    /// Office2007Black, Office2007Silver and Blend ). Also the user can define own skin by setting necessary properties of the class.
    /// </para>
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class CalendarEdit : Control</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><CalendarEdit Name="calendarEdit"/></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="CalendarEdit"/> control in XAML.
    /// <code>
    /// <Window x:Class="CalendarEditSample.Window1"
    ///    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///    xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
    ///    Title="Window1" Height="300" Width="300">
    ///    <Grid>
    ///            <local:CalendarEdit
    ///                        Name="calendarEdit"
    ///                        Date="11/07/1985"
    ///                        IsMonthNameAbbreviated="True"
    ///                        IsDayNamesAbbreviated="False"
    ///                        SelectionRangeMode="WholeColumn"
    ///                        HeaderBackground="YellowGreen"
    ///                        TodayRowIsVisible="True"
    ///                        ChangeModeTime="700"
    ///                        FrameMovingTime="700"
    ///                        CalendarStyle="Vista"/>
    ///    </Grid>
    ///    </Window>
    /// </code>
    /// <para/>The following example shows how to create a <see cref="CalendarEdit"/> control in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Tools.Controls;
    /// using Syncfusion.Windows.Tools;
    /// <para></para>
    /// namespace CalendarEditSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        public Window1()
    ///        {
    ///             InitializeComponent();
    ///             <para></para>
    ///             //Create a new instance of the CalendarEdit
    ///             CalendarEdit calendarEdit = new CalendarEdit();
    ///             // Add calendarEdit to grid
    ///             this.grid1.Children.Add( calendarEdit );
    ///             //Set the date of the calendar
    ///             calendarEdit.Date = new DateTime(1985, 7, 11);
    ///             //Month names will be displayed fully
    ///             calendarEdit.IsMonthNameAbbreviated = false;
    ///             //Day names will be displayed fully
    ///             calendarEdit.IsDayNamesAbbreviated = false;
    ///             //Ability to select the whole column with dates that does not belong to the current month
    ///             calendarEdit.SelectionRangeMode = SelectionRangeMode.WholeColumn;
    ///             //Calendar change mode animation time
    ///             calendarEdit.ChangeModeTime = 700;
    ///             //Month changing animation time
    ///             calendarEdit.FrameMovingTime = 700;
    ///             //Today bar will be displayed at the bottom of the calendar
    ///             calendarEdit.TodayRowIsVisible = true;
    ///             //Background for the header of the calendar
    ///             calendarEdit.HeaderBackground = new SolidColorBrush( Colors.YellowGreen );
    ///             //Vista style calendar
    ///             calendarEdit.CalendarStyle = CalendarStyle.Vista;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
  Type = typeof(CalendarEdit), XamlResource = "/Syncfusion.Shared.Wpf;component/Controls/Calendar/Themes/TransparentStyle.xaml")]
    public partial class CalendarEdit : Control
    {
        #region Constants

        /// <summary>
        /// Contains calendar template grid name.
        /// </summary>
        private const string CmainGrid = "MainGrid";

        /// <summary>
        /// Contains week numbers container name.
        /// </summary>
        private const string CweekNumbers = "PART_WeekNumbers";

        /// <summary>
        /// Contains current year week numbers container name.
        /// </summary>
        private const string CWeekNumbersGridCurrent = "WeekNumbersForYearCurrent";

        /// <summary>
        /// Contains following year week numbers container name.
        /// </summary>
        private const string CWeekNumbersGridFollow = "WeekNumbersForYearFollow";

        /// <summary>
        /// Contains name of the UpDown control that is responsible for year editing.
        /// </summary>
        private const string CyearUpDown = "PART_YearUpDown";

        /// <summary>
        /// Name of the panel that contains UpDown control for years editing.
        /// </summary>
        private const string CyearUpDownPanel = "PART_YearUpDownPanel";

        /// <summary>
        /// Name of the text block that contains month when years editing.
        /// </summary>
        private const string CeditMonthName = "PART_EditMonthName";

        /// <summary>
        /// Name of the next MonthButton
        /// </summary>
        private const string CnextMonthButtonName = "PART_NextMonthButton";

        /// <summary>
        /// Name of the previous MonthButton
        /// </summary>
        private const string CprevMonthButtonName = "PART_PrevMonthButton";

        #endregion Constants

        #region Enums

        /// <summary>
        /// Defines month changing direction.
        /// </summary>
        protected enum MoveDirection
        {
            /// <summary>
            /// Next month changing direction.
            /// </summary>
            Next,

            /// <summary>
            /// Previous month changing direction.
            /// </summary>
            Prev
        }

        /// <summary>
        /// Defines highlighting animation state.
        /// </summary>
        protected enum HighlightSate
        {
            /// <summary>
            /// Begins highlighting animation.
            /// </summary>
            Begin,

            /// <summary>
            /// Stops highlighting animation.
            /// </summary>
            Stop
        }

        /// <summary>
        /// Defines whether calendar should be scrolled to the previous/next month,
        /// if the cell that does not belong to the current month is clicked.
        /// </summary>
        protected enum ChangeMonthMode
        {
            /// <summary>
            /// Scrolling is enabled.
            /// </summary>
            Enabled,

            /// <summary>
            /// Scrolling is disabled.
            /// </summary>
            Disabled
        }

        /// <summary>
        /// Defines actions available for the collection of this type.
        /// </summary>
        protected enum CollectionChangedAction
        {
            /// <summary>
            /// Add action.
            /// </summary>
            Add,

            /// <summary>
            /// Remove action.
            /// </summary>
            Remove,

            /// <summary>
            /// Reset action.
            /// </summary>
            Reset,

            /// <summary>
            /// Replace action.
            /// </summary>
            Replace
        }

        /// <summary>
        /// Defines visual mode changing direction.
        /// </summary>
        protected enum ChangeVisualModeDirection
        {
            /// <summary>
            /// Up scrolling direction.
            /// </summary>
            Up,

            /// <summary>
            /// Down scrolling direction.
            /// </summary>
            Down
        }

        #endregion Enums

        #region Structures

        /// <summary>
        /// Describes previous and current calendar visual mode.
        /// </summary>
        protected struct VisualModeHistory
        {
            /// <summary>
            /// Defines old calendar visual mode.
            /// </summary>
            public CalendarVisualMode OldMode;

            /// <summary>
            /// Defines new calendar visual mode.
            /// </summary>
            public CalendarVisualMode NewMode;

            /// <summary>
            /// Initializes a new instance of the VisualModeHistory struct.
            /// </summary>
            /// <param name="oldMode">Old visual mode.</param>
            /// <param name="newMode">New visual mode.</param>
            public VisualModeHistory(CalendarVisualMode oldMode, CalendarVisualMode newMode)
            {
                OldMode = oldMode;
                NewMode = newMode;
            }
        }

        #endregion Structures

        #region Private members

        ///// <summary>
        ///// Minimum date supported by the current <see cref="System.Globalization.Calendar"/>.
        ///// </summary>
        /////

        ///// <summary>
        ///// Maximum date supported by the current <see cref="System.Globalization.Calendar"/>.
        ///// </summary>

        /// <summary>
        /// Storyboard that contains month changing animation.
        /// </summary>
        private Storyboard mmonthStoryboard;

        /// <summary>
        /// Instance of the storyboard containing next or previous moving
        /// animations.
        /// </summary>
        private Storyboard mmoveStoryboard;

        /// <summary>
        /// Storyboard that contains visual mode changing animation.
        /// </summary>
        private Storyboard mvisualModeStoryboard;

        /// <summary>
        /// Start date in the range selection.
        /// </summary>
        private DateTime m_shiftDate;

        /// <summary>
        /// Enables shift date changing.
        /// </summary>
        private bool m_shiftDateChangeEnabled;

        /// <summary>
        /// Defines whether selected dates update is locked.
        /// </summary>
        private bool mbselectedDatesUpdateLocked;

        /// <summary>
        /// Selected dates update lock counter.
        /// </summary>
        private int milockCounter;

        /// <summary>
        /// Instance of the pressed day cell button.
        /// </summary>
        private Cell mpressedCell;

        /// <summary>
        /// Sorted list of <see cref="Syncfusion.Windows.Shared.Date"/>
        /// items, that duplicates <see cref="SelectedDates"/> collection.
        /// </summary>
        private List<Date> mselectedDatesList;

        private List<Date> mInvalidDateList;

        /// <summary>
        /// Imitates month changing queue.
        /// </summary>
        private int m_iscrollCounter;

        /// <summary>
        /// Defines whether <see cref="Date"/> property has been set from GUI or
        /// directly from the code.
        /// </summary>
        private bool m_dateSetManual;

        /// <summary>
        /// The <see cref="Syncfusion.Windows.Shared.NavigateButton"/> that is
        /// responsible for the next month switching.
        /// </summary>
        private NavigateButton m_nextButton;

        /// <summary>
        /// The <see cref="Syncfusion.Windows.Shared.NavigateButton"/> that is
        /// responsible for the previous month switching.
        /// </summary>
        private NavigateButton m_prevButton;

        /// <summary>
        /// The <see cref="Syncfusion.Windows.Shared.MonthButton"/>
        /// that is responsible for the month name showing.
        /// </summary>
        private MonthButton m_monthButton1;

        /// <summary>
        /// The <see cref="Syncfusion.Windows.Shared.MonthButton"/>
        /// that is responsible for the month name showing.
        /// </summary>
        private MonthButton m_monthButton2;

        /// <summary>
        /// Month popup that shows the list of next and
        /// previous months.
        /// </summary>
        private MonthPopup m_popup;

        ///// <summary>
        ///// Defines calendar visual mode.
        ///// </summary>
        ////private CalendarVisualMode mvisualMode = CalendarVisualMode.Months;

        /// <summary>
        /// Defines current and previous visual mode.
        /// </summary>
        private VisualModeHistory mvisualModeInfo;

        /// <summary>
        /// Queue that stores consecution of the transitions from the current
        /// mode to days mode.
        /// </summary>
        /// <remarks>
        /// Uses when calendar is switched from vista <see cref="CalendarStyle"/> to standard <see cref="CalendarStyle"/>.
        /// </remarks>
        private Queue<VisualModeHistory> m_visualModeQueue;

        /// <summary>
        /// True if date is trying to be scrolled when moving animation is in progress.
        /// </summary>
        private bool m_postScrollNeed;

        /// <summary>
        /// Defines today button.
        /// </summary>
        internal Button m_todayButton;

        /// <summary>
        /// Contains true if click on cell has been done.
        /// </summary>
        private bool m_cellClicked = false;

        /// <summary>
        /// Contains true if click on week number cell has been done.
        /// </summary>
        private bool wcellClicked = false;

        internal DateTime miDate;

        internal DateTime mxDate;

        ///// <summary>
        ///// Default foreground.
        ///// </summary>
        ////private Brush m_defaultForeground = Brushes.Black;
        ///// <summary>
        ///// Default background.
        ///// </summary>
        ////private Brush m_defaultBackground = Brushes.Transparent;
        ///// <summary>
        ///// Default  header background.
        ///// </summary>
        ////private Brush m_defaultHeaderBackground = Brushes.Transparent;
        ///// <summary>
        ///// Default header foreground.
        ///// </summary>
        ////private Brush m_defaultHeaderForeground = Brushes.Black;
        ///// <summary>
        ///// Default selection border brush.
        ///// </summary>
        ////private Brush m_defaultSelectionBorderBrush = new SolidColorBrush(Color.FromArgb(255, 57, 136, 215));

        /// <summary>
        /// Contains min date of current culture.
        /// </summary>
        private static DateTime m_currentCultureMinDate = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;

        /// <summary>
        /// Contains min date of old culture value (when culture changed).
        /// </summary>
        private DateTime m_oldMinDate = m_currentCultureMinDate;

        /// <summary>
        /// Contains min date of new culture value (when culture changed).
        /// </summary>
        private DateTime m_newMinDate = m_currentCultureMinDate;

        /// <summary>
        /// Identifies if firing events is suspended.
        /// </summary>
        private bool m_bsuspendEventFire = false;

        /// <summary>
        /// Contains main grid of the calendar.
        /// </summary>
        private Grid m_mainGrid = null;

        /// <summary>
        /// Contains week numbers.
        /// </summary>
        private ContentPresenter m_weekNumbersContainer = null;

        /// <summary>
        /// Contains week numbers for current year.
        /// </summary>
        private ContentPresenter wcurrentweekNumbersContainer = null;

        /// <summary>
        /// Contains week numbers for following year.
        /// </summary>
        private ContentPresenter wfollowweekNumbersContainer = null;

        /// <summary>
        /// Contains UpDown for year editing.
        /// </summary>
        private UpDown m_yearUpDown = null;

        /// <summary>
        /// Contains UpDown panel for year editing.
        /// </summary>
        private StackPanel m_yearUpDownPanel = null;

        /// <summary>
        /// Contains timer to define time (some seconds) when MonthButton will be changed to year editing.
        /// </summary>
        private DispatcherTimer m_timer = null;

        /// <summary>
        /// Contains month text block when year editing.
        /// </summary>
        private TextBlock m_editMonthName = null;

        /// <summary>
        /// Contains tooltips.
        /// </summary>
        private Hashtable m_toolTipDates = new Hashtable();

        /// <summary>
        /// Starting year
        /// </summary>
        public static int startYear;

        /// <summary>
        /// Ending year
        /// </summary>
        public static int endYear;

        /// <summary>
        /// Primary Date
        /// </summary>
        public static int primaryDate;

        /// <summary>
        /// Selected Week Number.
        /// </summary>
        public static string clickedweeknumber;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="CalendarEdit"/> class.  It overrides some dependency properties.
        /// </summary>
        static CalendarEdit()
        {
            //EnvironmentTest.ValidateLicense(typeof(CalendarEdit));

            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarEdit), new FrameworkPropertyMetadata(typeof(CalendarEdit)));
            NextCommand = new RoutedUICommand("Next month", "NextCommand", typeof(CalendarEdit));
            PrevCommand = new RoutedUICommand("Prev month", "PrevCommand", typeof(CalendarEdit));
            UpCommand = new RoutedUICommand("VisualMode level up", "UpCommand", typeof(CalendarEdit));
            NextCommand.InputGestures.Add(new KeyGesture(Key.Right, ModifierKeys.Alt));
            PrevCommand.InputGestures.Add(new KeyGesture(Key.Left, ModifierKeys.Alt));
            UpCommand.InputGestures.Add(new KeyGesture(Key.Back, ModifierKeys.Alt));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEdit"/> class.
        /// </summary>
        public CalendarEdit()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(CalendarEdit));
            }
            CommandBinding nextCommandBinding = new CommandBinding(NextCommand, NextCommandExecute, NextCommandCanExecute);
            CommandBinding prevCommandBinding = new CommandBinding(PrevCommand, PrevCommandExecute, PrevCommandCanExecute);
            CommandBinding upcommandBinding = new CommandBinding(UpCommand, UpCommandExecute, UpCommandCanExecute);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
                catch (InvalidOperationException)
                {
                    if (Application.Current != null && Application.Current.Dispatcher != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                            () =>
                            {
                                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                            }));
                    }
                }
            }
            m_postScrollNeed = false;
            milockCounter = 0;
            m_iscrollCounter = 0;
            m_shiftDate = Date;
            m_shiftDateChangeEnabled = true;
            m_dateSetManual = false;
            m_visualModeQueue = new Queue<VisualModeHistory>();
            SelectedDates = new DatesCollection();
            SelectedDatesList = new List<Date>();
            InvalidDates = new List<Date>();
            SelectedDates.AllowInsert = AllowSelection;
            VisualMode = CalendarVisualMode.Days;
            VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.Days);
            CurrentMonthGrid = new MonthGrid();
            CurrentYearGrid = new YearGrid();
            CurrentYearRangeGrid = new YearRangeGrid();
            FollowingMonthGrid = new MonthGrid();
            FollowingYearGrid = new YearGrid();
            FollowingYearRangeGrid = new YearRangeGrid();
            DayNamesGrid = new DayNamesGrid();
            FollowingDayGrid = new DayGrid();
            CurrentDayGrid = new DayGrid();
            WeekNumbersGrid = new WeekNumbersGrid();
            CurrentWeekNumbersGrid = new WeekNumberGridPanel();
            FollowingWeekNumbersGrid = new WeekNumberGridPanel();
            CurrentDayGrid.SelectionBorder.CornerRadius = SelectionBorderCornerRadius;
            FollowingDayGrid.SelectionBorder.CornerRadius = SelectionBorderCornerRadius;
            DateDataTemplates = new DataTemplatesDictionary();
            DateStyles = new StylesDictionary();
            AddLogicalChild(DayNamesGrid);
            AddLogicalChild(WeekNumbersGrid);
            AddLogicalChild(FollowingDayGrid);
            AddLogicalChild(CurrentDayGrid);
            AddLogicalChild(CurrentMonthGrid);
            AddLogicalChild(CurrentYearGrid);
            AddLogicalChild(CurrentYearRangeGrid);
            AddLogicalChild(FollowingMonthGrid);
            AddLogicalChild(FollowingYearGrid);
            AddLogicalChild(FollowingYearRangeGrid);
            AddLogicalChild(CurrentWeekNumbersGrid);
            AddLogicalChild(FollowingWeekNumbersGrid);
            CommandBindings.Add(nextCommandBinding);
            CommandBindings.Add(prevCommandBinding);
            CommandBindings.Add(upcommandBinding);
            CurrentYearRangeGrid.Visibility = Visibility.Hidden;
            CurrentYearGrid.Visibility = Visibility.Hidden;
            CurrentMonthGrid.Visibility = Visibility.Hidden;
            FollowingYearRangeGrid.Visibility = Visibility.Hidden;
            FollowingYearGrid.Visibility = Visibility.Hidden;
            FollowingMonthGrid.Visibility = Visibility.Hidden;
            CurrentDayGrid.Visibility = Visibility.Visible;
            FollowingDayGrid.Visibility = Visibility.Hidden;
            CurrentWeekNumbersGrid.Visibility = Visibility.Hidden;
            FollowingWeekNumbersGrid.Visibility = Visibility.Hidden;
            DayNameCellMouseLeftButtonDown += new MouseButtonEventHandler(DayNameCell_OnMouseLeftButtonDown);
            DayNamesGrid.MouseMove += new MouseEventHandler(DayNamesGrid_OnMouseMove);
            DayNamesGrid.MouseLeave += new MouseEventHandler(DayNamesGrid_OnMouseLeave);
            CurrentDayGrid.KeyDown += new KeyEventHandler(DayGrid_OnKeyDown);
            FollowingDayGrid.KeyDown += new KeyEventHandler(DayGrid_OnKeyDown);
            CurrentMonthGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            FollowingMonthGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            CurrentYearGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            FollowingYearGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            CurrentYearRangeGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            FollowingYearRangeGrid.KeyDown += new KeyEventHandler(VisualModeGrid_OnKeyDown);
            FollowingDayGrid.MouseMove += new MouseEventHandler(DayGrid_OnMouseMove);
            CurrentDayGrid.MouseMove += new MouseEventHandler(DayGrid_OnMouseMove);
            SelectedDates.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedDates_OnCollectionChanged);
            DayCellMouseLeftButtonDown += new MouseButtonEventHandler(DayCell_OnMouseLeftButtonDown);
            DayCellMouseLeftButtonUp += new MouseButtonEventHandler(DayCell_OnMouseLeftButtonUp);
            MonthCellMouseLeftButtonDown += new MouseButtonEventHandler(MonthCell_OnMouseLeftButtonDown);
            MonthCellMouseLeftButtonUp += new MouseButtonEventHandler(MonthCell_OnMouseLeftButtonUp);
            YearCellMouseLeftButtonDown += new MouseButtonEventHandler(YearCell_OnMouseLeftButtonDown);
            YearCellMouseLeftButtonUp += new MouseButtonEventHandler(YearCell_OnMouseLeftButtonUp);
            YearRangeCellMouseLeftButtonDown += new MouseButtonEventHandler(YearRangeCell_OnMouseLeftButtonDown);
            YearRangeCellMouseLeftButtonUp += new MouseButtonEventHandler(YearRangeCell_OnMouseLeftButtonUp);
            this.WeekNumberCellPanelMouseLeftButtonDown += new MouseButtonEventHandler(WeekNumberCellPanel_OnMouseLeftButtonDown);
            this.WeekNumberCellMouseLeftButtonDown += new MouseButtonEventHandler(WeekNumberCell_OnMouseLeftButtonDown);
            UpdateVisibleData();
            miDate = Calendar.MinSupportedDateTime;
            mxDate = Calendar.MaxSupportedDateTime;
            if (BlackoutDates == null)
                BlackoutDates = new BlackDatesCollection();

            MinDate = Calendar.MinSupportedDateTime;
            MaxDate = Calendar.MaxSupportedDateTime;
            TodayDate = DateTime.Now.ToString("D", Culture.DateTimeFormat);
            this.BlackoutDates.CollectionChanged += new NotifyCollectionChangedEventHandler(BlackoutDates_CollectionChanged);
            this.Unloaded += new RoutedEventHandler(CalendarEdit_Unloaded);

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
            this.IsManipulationEnabled = true;
#endif
        }

        private void CalendarEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            this.BlackoutDates.CollectionChanged -= new NotifyCollectionChangedEventHandler(BlackoutDates_CollectionChanged);
        }

        private void BlackoutDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // CalendarEdit instance = (CalendarEdit)d;

            this.OnBlackoutDatesCollectionChanged(e);
        }

        private void OnBlackoutDatesCollectionChanged(NotifyCollectionChangedEventArgs e)
        {

            if (e != null && e.NewItems != null && e.NewItems[0] != null)
            {
                List<DateTime> dates;
                DateTime d1 = (DateTime)(e.NewItems[0] as BlackoutDatesRange).StartDate;
                DateTime d2 = (DateTime)(e.NewItems[0] as BlackoutDatesRange).EndDate;
                dates = GetDateRange(d1, d2);
                for (int i = 0; i < dates.Count; i++)
                {
                    Date date = new Date(dates[i], Calendar);
                    InvalidDates.Add(date);
                }
            }

        }

        #endregion Initialization

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="Date"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DateChanged;

        /// <summary>
        /// Event that is raised when <see cref="DisableDateSelection"/> property is changed.
        /// </summary>
        public event EventHandler MonthChanged;

        /// <summary>
        /// Event that is raised when <see cref="CultureInfo"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CultureChanged;

        /// <summary>
        /// Occurs when [is today button clicked].
        /// </summary>
        internal event PropertyChangedCallback IsTodayButtonClickedChanged;

        /// <summary>
        /// Event that is raised when <see cref="Calendar"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalendarChanged;

        /// <summary>
        /// Event that is raised when <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalendarStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="DisableDateSelection"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DisableDateSelectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="AllowSelection"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AllowSelectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="AllowMultiplySelection"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowMultiplySelectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsDayNamesAbbreviated"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback IsDayNamesAbbreviatedChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsMonthNameAbbreviated"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback IsMonthNameAbbreviatedChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectionRangeMode"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback SelectionRangeModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectionBorderBrush"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback SelectionBorderBrushChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MouseOverBorderBrushChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MouseOverBackgroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback MouseOverForegroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback SelectedDayCellBorderBrushChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback TodayCellBorderBrushChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback TodayCellForegroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback TodayCellBackgroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback TodayCellSelectedBorderBrushChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback TodayCellSelectedBackgroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback SelectedDayCellHoverBackgroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback SelectedDayCellBackgroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback SelectedDayCellForegroundChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback NotCurrentMonthForegroundChanged;

        /// <summary>
        /// Occurs when [selection foreground changed].
        /// </summary>
        public event PropertyChangedCallback SelectionForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberSelectionBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberSelectionBorderBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberSelectionBorderThickness"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberSelectionBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberSelectionBorderCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberSelectionBorderCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberHoverForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberHoverForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberSelectionForeground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberSelectionForegroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberSelectionBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberSelectionBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberHoverBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberHoverBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="WeekNumberHoverBorderBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeekNumberHoverBorderBrushChanged;

        /// <summary>
        /// Occurs when <see cref="WeekNumberBorderBrush"/> [week number border brush changed].
        /// </summary>
        public event PropertyChangedCallback WeekNumberBorderBrushChanged;

        /// <summary>
        /// Occurs when <see cref="WeekNumberBorderThickness"/> [week number border thickness changed].
        /// </summary>
        public event PropertyChangedCallback WeekNumberBorderThicknessChanged;

        /// <summary>
        /// Occurs when <see cref="WeekNumberCornerRadius"/> [week number corner radius changed].
        /// </summary>
        public event PropertyChangedCallback WeekNumberCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectionBorderCornerRadius"/>
        /// property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectionBorderCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameMovingTime"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback FrameMovingTimeChanged;

        /// <summary>
        /// Event that is raised when <see cref="ChangeModeTime"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ChangeModeTimeChanged;

        /// <summary>
        /// Event that is raised when <see cref="MonthChangeDirection"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback MonthChangeDirectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayNameCellsDataTemplate"/> property
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback DayNameCellsDataTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="PreviousScrollButtonTemplate"/> property
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback PreviousScrollButtonTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="NextScrollButtonTemplate"/> property
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback NextScrollButtonTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayCellsDataTemplate"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback DayCellsDataTemplateChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayCellsStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DayCellsStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayNameCellsStyle"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback DayNameCellsStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayCellsDataTemplateSelector"/>
        /// property is changed.
        /// </summary>
        public event PropertyChangedCallback DayCellsDataTemplateSelectorChanged;

        /// <summary>
        /// Event that is raised when <see cref="DayNameCellsDataTemplateSelector"/>
        /// property is changed.
        /// </summary>
        public event PropertyChangedCallback DayNameCellsDataTemplateSelectorChanged;

        /// <summary>
        /// Event that is raised when <see cref="DateDataTemplates"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback DateDataTemplatesChanged;

        /// <summary>
        /// Event that is raised when <see cref="DateStyles"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DateStylesChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScrollToDateEnabled"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback ScrollToDateEnabledChanged;

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedCallback VisualModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="TodayRowIsVisible"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TodayRowIsVisibleChanged;

        /// <summary>
        /// Event that is raised when <see cref="MinMaxHiddenChanged"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinMaxHiddenChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsShowWeekNumbers"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsShowWeekNumbersChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsShowWeekNumbersGrid"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsShowWeekNumbersGridChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsAllowYearSelection"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAllowYearSelectionChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShowPreviousMonthDays"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowPreviousMonthDaysChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShowNextMonthDays"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowNextMonthDaysChanged;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="YearRangeCellMouseLeftButtonUp"/> event is
        /// raised on the <see cref="YearRangeCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler YearRangeCellMouseLeftButtonUp;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="YearRangeCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="YearRangeCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler YearRangeCellMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="YearCellMouseLeftButtonUp"/> event is
        /// raised on the <see cref="YearCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler YearCellMouseLeftButtonUp;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="YearCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="YearCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler YearCellMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="MonthCellMouseLeftButtonUp"/> event is
        /// raised on the <see cref="MonthCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler MonthCellMouseLeftButtonUp;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="MonthCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="MonthCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler MonthCellMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="DayCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler DayCellMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayCellMouseLeftButtonUp"/> event is
        /// raised on the <see cref="DayCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler DayCellMouseLeftButtonUp;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayNameCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="DayNameCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler DayNameCellMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="WeekNumberCellPanelMouseLeftButtonDown"/> event is
        /// raised on the <see cref="WeekNumberCellPanel"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler WeekNumberCellPanelMouseLeftButtonDown;

        /// <summary>
        /// Invoked whenever an unhandled <see cref="WeekNumberCellMouseLeftButtonDown"/> event is
        /// raised on the <see cref="WeekNumberCell"/> element.
        /// </summary>
        protected internal event MouseButtonEventHandler WeekNumberCellMouseLeftButtonDown;

        /// <summary>
        /// Event that is raised when <see cref="VisibleData"/> property is changed.
        /// </summary>
        protected internal event PropertyChangedCallback VisibleDataChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the selected dates list.
        /// </summary>
        /// <value>
        /// Type: <see cref="List"/>
        /// </value>
        /// <seealso cref="List"/>
        public List<Date> SelectedDatesList
        {
            get
            {
                return mselectedDatesList;
            }

            set
            {
                mselectedDatesList = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public List<Date> InvalidDates
        {
            get
            {
                return mInvalidDateList;
            }
            set
            {
                mInvalidDateList = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether control is initialized.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsInitializeComplete
        {
            get
            {
                return CurrentDayGrid.ParentCalendar != null && FollowingDayGrid.ParentCalendar != null;
            }
        }

        /// <summary>
        /// Gets or sets the calendar visual mode.
        /// </summary>
        public CalendarVisualMode VisualMode
        {
            get { return (CalendarVisualMode)GetValue(VisualModeProperty); }
            set { SetValue(VisualModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets current and previous visual mode.
        /// </summary>
        protected VisualModeHistory VisualModeInfo
        {
            get
            {
                return mvisualModeInfo;
            }

            set
            {
                mvisualModeInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether week number cell is clicked.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        internal bool IsWeekCellClicked
        {
            get
            {
                return this.wcellClicked;
            }

            set
            {
                this.wcellClicked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cell clicked value.
        /// </summary>
        internal bool IsCellClicked
        {
            get
            {
                return m_cellClicked;
            }

            set
            {
                m_cellClicked = value;
            }
        }

        /// <summary>
        /// Gets tooltip from the date.
        /// </summary>
        internal Hashtable TooltipDates
        {
            get
            {
                return m_toolTipDates;
            }
        }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Sets tooltip to the day cell.
        /// </summary>
        /// <param name="rowIndex">Row number of the cell (starts from 0)</param>
        /// <param name="colIndex">Column number of the cell (starts from 0)</param>
        /// <param name="tooltip">The tooltip.</param>
        public void SetToolTip(int rowIndex, int colIndex, ToolTip tooltip)
        {
            SetCellToolTip(rowIndex, colIndex, CurrentDayGrid, tooltip);
            SetCellToolTip(rowIndex, colIndex, FollowingDayGrid, tooltip);
        }

        /// <summary>
        /// Sets tooltip to the day cell.
        /// </summary>
        /// <param name="date">Date of the cell to set tooltip.</param>
        /// <param name="tooltip">Tooltip that should be set.</param>
        public void SetToolTip(Date date, ToolTip tooltip)
        {
            if (m_toolTipDates.ContainsKey(date))
            {
                m_toolTipDates.Remove(date);
            }

            m_toolTipDates.Add(date, tooltip);

            CurrentDayGrid.Initialize(VisibleData, Culture, Calendar);
            FollowingDayGrid.Initialize(VisibleData, Culture, Calendar);
        }

        /// <summary>
        /// Gets week number from the current date.
        /// </summary>
        /// <param name="dt">Date to get week number from.</param>
        /// <returns>Week number</returns>
        internal int GetWeekNumber(DateTime dt)
        {
            int dayOfYear = dt.DayOfYear;
            DayOfWeek firstDayName = Culture.Calendar.GetDayOfWeek(new DateTime(dt.Year, 1, 1));
            int doy1st = (int)firstDayName;
            int shift = (doy1st <= 4) ? (4 - doy1st) : (7 + 4 - doy1st);
            int dayOfYear1thir = dayOfYear + shift;

            int result = ((dayOfYear - shift) / 7) + 1;

            return result;
        }

        /// <summary>
        /// Locks visual update of the selecting dates.
        /// </summary>
        public void LockSelectedDatesUpdate()
        {
            milockCounter++;
            mbselectedDatesUpdateLocked = true;
        }

        /// <summary>
        /// Unlocks visual update of the selecting dates.
        /// </summary>
        public void UnLockSelectedDatesUpdate()
        {
            int count = milockCounter;

            if (--count == 0)
            {
                mbselectedDatesUpdateLocked = false;
                milockCounter = 0;
            }
        }

        /// <summary>
        /// Sets the month for displaying in the calendar.
        /// </summary>
        /// <param name="month">The month to be set.</param>
        public void SetVisibleMonth(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", month, "Not a valid parameter value, it must be in the range 1-12");
            }

            VisibleDate tmp = VisibleData;
            tmp.VisibleMonth = month;
            VisibleData = tmp;
        }

        /// <summary>
        /// Sets the year for displaying in the calendar.
        /// </summary>
        /// <param name="year">The year to be set.</param>
        public void SetVisibleYear(int year)
        {
            int maxYear = Calendar.GetYear(Calendar.MaxSupportedDateTime);
            int minYear = Calendar.GetYear(Calendar.MinSupportedDateTime);

            if (year < minYear || year > maxYear)
            {
                throw new ArgumentOutOfRangeException("year", year, "Not a valid parameter value, it must be in the range " + minYear.ToString() + "- " + maxYear.ToString());
            }

            VisibleDate tmp = VisibleData;
            tmp.VisibleYear = year;
            VisibleData = tmp;
        }

        #endregion Public methods

        #region Implementation

        #region protected virtual

        /// <summary>
        /// Scrolls the calendar to the new date.
        /// </summary>
        protected virtual void ScrollToDate()
        {
            Date newDate = new Date(Date.Year, Date.Month, Date.Day);
            VisibleDate newVisibleDate = new VisibleDate(newDate.Year, newDate.Month, newDate.Day);

            if (VisualMode == CalendarVisualMode.Days)
            {
                if (!IsStoryboardActive(mmonthStoryboard))
                {
                    DateTime endDate;
                    DateTime startDate;
                    int startYear = VisibleData.VisibleYear;
                    int startMonth = VisibleData.VisibleMonth;
                    int endYear = Calendar.GetYear(Date);
                    int endMonth = Calendar.GetMonth(Date);
                    DateTime maxDate;
                    DateTime minDate;
                    if (MinMaxHidden)
                    {
                        maxDate = new DateTime(Calendar.GetYear(MaxDate), Calendar.GetMonth(MaxDate), Calendar.GetDayOfMonth(MaxDate));
                        minDate = new DateTime(Calendar.GetYear(MinDate), Calendar.GetMonth(MinDate), Calendar.GetDayOfMonth(MinDate));
                    }
                    else
                    {
                        maxDate = new DateTime(Calendar.GetYear(mxDate), Calendar.GetMonth(mxDate), Calendar.GetDayOfMonth(mxDate));
                        minDate = new DateTime(Calendar.GetYear(miDate), Calendar.GetMonth(miDate), Calendar.GetDayOfMonth(miDate));
                    }
                    startDate = new DateTime(startYear, startMonth, 1);

                    if (startDate > maxDate || startDate < minDate)
                    {
                        startYear = Calendar.GetYear(Date);
                        startMonth = Calendar.GetMonth(Date);
                        startDate = new DateTime(startYear, startMonth, 1, Calendar);
                        endDate = new DateTime(endYear, endMonth, 1, Calendar);
                    }
                    else
                    {
                        endDate = new DateTime(endYear, endMonth, 1);
                    }

                    int delta = CalculateMonthDelta(startDate, endDate);

                    if (delta != 0)
                    {
                        if (IsAnimationRequired())
                        {
                            if (startDate < endDate)
                            {
                                BeginMoving(MoveDirection.Next, delta);
                            }
                            else
                            {
                                BeginMoving(MoveDirection.Prev, delta);
                            }
                        }
                        else
                        {
                            VisibleData = newVisibleDate;
                        }
                    }
                }
                else
                {
                    m_postScrollNeed = true;
                }
            }
            else
            {
                if (this.DisableDateSelection == false)
                {
                    if (IsAnimationRequired())
                    {
                        if (m_visualModeQueue.Count != 0 && !IsStoryboardActive(mvisualModeStoryboard))
                        {
                            VisualModeHistory info = m_visualModeQueue.Dequeue();
                            VisualMode = info.NewMode;
                            VisualModeInfo = info;
                            ChangeVisualModePreview(newVisibleDate);
                        }
                    }
                    else
                    {
                        FindCurrentGrid(VisualMode).Visibility = Visibility.Hidden;
                        VisualMode = CalendarVisualMode.Days;
                        DayNamesGrid.RenderTransform = new ScaleTransform(1, 1);
                        FindCurrentGrid(VisualMode).RenderTransform = new ScaleTransform(1, 1);
                        FindCurrentGrid(VisualMode).Visibility = Visibility.Visible;
                        DayNamesGrid.Visibility = Visibility.Visible;
                        VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.Days);
                        VisibleData = newVisibleDate;
                    }
                }
            }
        }

        /// <summary>
        /// Starts animation on the month changing.
        /// </summary>
        /// <param name="direction">Changing direction.</param>
        /// <param name="month">Number of months that will be added
        /// to the current month.</param>
        protected virtual void BeginMoving(MoveDirection direction, int month)
        {
            if (!IsOutOfDateRange(month))
            {
                TranslateTransform moveFollowing = new TranslateTransform();
                TranslateTransform moveCurrent = new TranslateTransform();
                DoubleAnimation followingAnimation = new DoubleAnimation();
                DoubleAnimation currentAnimation = new DoubleAnimation();
                DoubleAnimation opacityAnimation = new DoubleAnimation();
                DoubleAnimation monthButton1OpacityAnimation = new DoubleAnimation();
                DoubleAnimation monthButton2OpacityAnimation = new DoubleAnimation();
                VisibleDate oldData = VisibleData;
                mmonthStoryboard = new Storyboard();
                Timeline.SetDesiredFrameRate(mmonthStoryboard, 24);
                mmonthStoryboard.AccelerationRatio = 0.7;
                mmonthStoryboard.DecelerationRatio = 0.3;
                followingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FrameMovingTime));
                currentAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FrameMovingTime));
                opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FrameMovingTime));
                monthButton1OpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FrameMovingTime));
                monthButton2OpacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(FrameMovingTime / 2));
                NameScope.SetNameScope(this, new NameScope());
                if (this.DisableDateSelection == false)
                {
                    FollowingDayGrid.Visibility = Visibility.Visible;
                    CurrentDayGrid.Visibility = Visibility.Visible;
                    m_monthButton1.Visibility = Visibility.Visible;
                    m_monthButton2.Visibility = Visibility.Visible;
                    CurrentDayGrid.SelectionBorder.Visibility = Visibility.Hidden;
                    FollowingDayGrid.SelectionBorder.Visibility = Visibility.Hidden;

                    AddMonth(month);
                    FollowingDayGrid.Initialize(VisibleData, Culture, Calendar);

                    WeekNumbersGrid.SetWeekNumbers(FollowingDayGrid.WeekNumbers);

                    CurrentDayGrid.Initialize(oldData, Culture, Calendar);
                }
                else
                {
                    AddMonth(month);
                }
                m_monthButton1.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
                m_monthButton2.Initialize(oldData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
                followingAnimation.To = 0;

                if (direction == MoveDirection.Next)
                {
                    if (MonthChangeDirection == AnimationDirection.Horizontal)
                    {
                        moveFollowing.X = CurrentDayGrid.ActualWidth;
                        currentAnimation.To = -CurrentDayGrid.ActualWidth;
                    }

                    if (MonthChangeDirection == AnimationDirection.Vertical)
                    {
                        double ilayOnRatio = CalculateLayOnValue(CurrentDayGrid);
                        moveFollowing.Y = CurrentDayGrid.ActualHeight - ilayOnRatio;
                        currentAnimation.To = -CurrentDayGrid.ActualHeight + ilayOnRatio;
                    }
                }

                if (direction == MoveDirection.Prev)
                {
                    if (MonthChangeDirection == AnimationDirection.Horizontal)
                    {
                        moveFollowing.X = -CurrentDayGrid.ActualWidth;
                        currentAnimation.To = CurrentDayGrid.ActualWidth;
                    }

                    if (MonthChangeDirection == AnimationDirection.Vertical)
                    {
                        double ilayOnRatio = CalculateLayOnValue(FollowingDayGrid);
                        moveFollowing.Y = -CurrentDayGrid.ActualHeight + ilayOnRatio;
                        currentAnimation.To = CurrentDayGrid.ActualHeight - ilayOnRatio;
                    }
                }

                opacityAnimation.To = 1;
                monthButton1OpacityAnimation.To = 1;
                monthButton2OpacityAnimation.To = 0;

                FollowingDayGrid.BeginAnimation(OpacityProperty, null);
                m_monthButton1.BeginAnimation(OpacityProperty, null);
                m_monthButton2.BeginAnimation(OpacityProperty, null);
                FollowingDayGrid.Opacity = 0.01;
                m_monthButton1.Opacity = 0.01;

                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(DayGrid.OpacityProperty));
                Storyboard.SetTargetProperty(monthButton1OpacityAnimation, new PropertyPath(MonthButton.OpacityProperty));
                Storyboard.SetTargetProperty(monthButton2OpacityAnimation, new PropertyPath(MonthButton.OpacityProperty));
                CurrentDayGrid.RenderTransform = moveCurrent;
                FollowingDayGrid.RenderTransform = moveFollowing;
                RegisterName("MoveFollowing", moveFollowing);
                RegisterName("MoveCurrent", moveCurrent);
                RegisterName("NextDayGrid", FollowingDayGrid);
                RegisterName("MonthButton1", m_monthButton1);
                RegisterName("MonthButton2", m_monthButton2);
                Storyboard.SetTargetName(followingAnimation, "MoveFollowing");
                Storyboard.SetTargetName(currentAnimation, "MoveCurrent");
                Storyboard.SetTargetName(opacityAnimation, "NextDayGrid");
                Storyboard.SetTargetName(monthButton1OpacityAnimation, "MonthButton1");
                Storyboard.SetTargetName(monthButton2OpacityAnimation, "MonthButton2");

                if (MonthChangeDirection == AnimationDirection.Horizontal)
                {
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath(TranslateTransform.XProperty));
                    Storyboard.SetTargetProperty(currentAnimation, new PropertyPath(TranslateTransform.XProperty));
                }

                if (MonthChangeDirection == AnimationDirection.Vertical)
                {
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath(TranslateTransform.YProperty));
                    Storyboard.SetTargetProperty(currentAnimation, new PropertyPath(TranslateTransform.YProperty));
                }

                mmonthStoryboard.Children.Add(followingAnimation);
                mmonthStoryboard.Children.Add(currentAnimation);
                mmonthStoryboard.Children.Add(opacityAnimation);
                mmonthStoryboard.Children.Add(monthButton1OpacityAnimation);
                mmonthStoryboard.Children.Add(monthButton2OpacityAnimation);

                // if( MonthChangeDirection == AnimationDirection.Vertical )
                // {
                //    DoubleAnimation verticalOpacityAnimation = new DoubleAnimation( );
                //    verticalOpacityAnimation.Duration = new Duration( TimeSpan.FromMilliseconds( FrameMovingTime ) );
                //    verticalOpacityAnimation.To = 0;
                //    CurrentDayGrid.BeginAnimation( OpacityProperty, null );
                //    CurrentDayGrid.Opacity = 1;
                //    Storyboard.SetTargetProperty( verticalOpacityAnimation, new PropertyPath( DayGrid.OpacityProperty ) );
                //    RegisterName( "CurrentDayGrid", CurrentDayGrid );
                //    Storyboard.SetTargetName( verticalOpacityAnimation, "CurrentDayGrid" );
                //    m_MonthStoryboard.Children.Add( verticalOpacityAnimation );
                // }

                #region Focus hiding

                Style hiddenFocusStyle = new Style(typeof(Control));
                ControlTemplate emptyTemplate = new ControlTemplate();
                Setter item = new Setter(Control.TemplateProperty, null);
                hiddenFocusStyle.Setters.Add(item);
                CurrentDayGrid.FocusVisualStyle = hiddenFocusStyle;
                FollowingDayGrid.FocusVisualStyle = hiddenFocusStyle;

                #endregion Focus hiding

                mmonthStoryboard.Completed += new EventHandler(OnAnimationCompleted);
                mmonthStoryboard.Begin(this, true);
            }
        }

        /// <summary>
        /// Starts animation on year and years range changing.
        /// </summary>
        /// <param name="direction">Changing direction.</param>
        protected virtual void Move(MoveDirection direction)
        {
            CalendarEditGrid currentGrid = null;
            CalendarEditGrid followingGrid = null;
            Date nextDate = new Date();

            if (!IsOutOfDateRange(direction, ref nextDate))
            {
                if (VisualMode == CalendarVisualMode.Months)
                {
                    currentGrid = CurrentMonthGrid;
                    followingGrid = FollowingMonthGrid;
                }

                if (VisualMode == CalendarVisualMode.Years)
                {
                    currentGrid = CurrentYearGrid;
                    followingGrid = FollowingYearGrid;
                }

                if (VisualMode == CalendarVisualMode.YearsRange)
                {
                    currentGrid = CurrentYearRangeGrid;
                    followingGrid = FollowingYearRangeGrid;
                }

                if (VisualMode == CalendarVisualMode.WeekNumbers)
                {
                    currentGrid = CurrentWeekNumbersGrid;
                    followingGrid = FollowingWeekNumbersGrid;
                }
            }
            else
            {
                return;
            }

            VisibleDate nextVisibleDate = new VisibleDate(nextDate.Year, nextDate.Month, nextDate.Day);
            VisibleDate prevVisibleDate = VisibleData;
            TranslateTransform moveFollowing = new TranslateTransform();
            TranslateTransform moveCurrent = new TranslateTransform();
            DoubleAnimation followingAnimation = new DoubleAnimation();
            DoubleAnimation currentAnimation = new DoubleAnimation();
            DoubleAnimation monthButton1OpacityAnimation = new DoubleAnimation();
            DoubleAnimation monthButton2OpacityAnimation = new DoubleAnimation();
            currentAnimation.Duration = TimeSpan.FromMilliseconds(FrameMovingTime);
            followingAnimation.Duration = TimeSpan.FromMilliseconds(FrameMovingTime);
            monthButton1OpacityAnimation.Duration = TimeSpan.FromMilliseconds(FrameMovingTime);
            monthButton2OpacityAnimation.Duration = TimeSpan.FromMilliseconds(FrameMovingTime);
            VisibleData = nextVisibleDate;
            currentGrid.Initialize(prevVisibleDate, Culture, Calendar);
            followingGrid.Initialize(nextVisibleDate, Culture, Calendar);
            if (this.DisableDateSelection == true && followingGrid != this.CurrentMonthGrid)
                FollowingMonthGrid.Visibility = Visibility.Hidden;
            currentGrid.Visibility = Visibility.Visible;
            followingGrid.Visibility = Visibility.Visible;
            m_monthButton1.Visibility = Visibility.Visible;
            m_monthButton2.Visibility = Visibility.Visible;
            m_monthButton1.Initialize(nextVisibleDate, Calendar, Culture, IsDayNamesAbbreviated, VisualMode);
            m_monthButton2.Initialize(prevVisibleDate, Calendar, Culture, IsDayNamesAbbreviated, VisualMode);
            followingAnimation.To = 0;
            monthButton1OpacityAnimation.To = 1;
            monthButton2OpacityAnimation.To = 0;
            VisibleData = nextVisibleDate;
            m_monthButton1.BeginAnimation(OpacityProperty, null);
            m_monthButton2.BeginAnimation(OpacityProperty, null);
            m_monthButton1.Opacity = 0.01;

            if (direction == MoveDirection.Next)
            {
                if (MonthChangeDirection == AnimationDirection.Horizontal)
                {
                    moveFollowing.X = currentGrid.ActualWidth;
                    currentAnimation.To = -currentGrid.ActualWidth;
                }

                if (MonthChangeDirection == AnimationDirection.Vertical)
                {
                    moveFollowing.Y = currentGrid.ActualHeight;
                    currentAnimation.To = -currentGrid.ActualHeight;
                }
            }

            if (direction == MoveDirection.Prev)
            {
                if (MonthChangeDirection == AnimationDirection.Horizontal)
                {
                    moveFollowing.X = -currentGrid.ActualWidth;
                    currentAnimation.To = currentGrid.ActualWidth;
                }

                if (MonthChangeDirection == AnimationDirection.Vertical)
                {
                    moveFollowing.Y = -currentGrid.ActualHeight;
                    currentAnimation.To = currentGrid.ActualHeight;
                }
            }

            NameScope.SetNameScope(this, new NameScope());
            currentGrid.RenderTransform = moveCurrent;
            followingGrid.RenderTransform = moveFollowing;
            RegisterName("MoveFollowing", moveFollowing);
            RegisterName("MoveCurrent", moveCurrent);
            RegisterName("MonthButton1", m_monthButton1);
            RegisterName("MonthButton2", m_monthButton2);
            Storyboard.SetTargetName(followingAnimation, "MoveFollowing");
            Storyboard.SetTargetName(currentAnimation, "MoveCurrent");
            Storyboard.SetTargetName(monthButton1OpacityAnimation, "MonthButton1");
            Storyboard.SetTargetName(monthButton2OpacityAnimation, "MonthButton2");

            if (MonthChangeDirection == AnimationDirection.Horizontal)
            {
                Storyboard.SetTargetProperty(followingAnimation, new PropertyPath(TranslateTransform.XProperty));
                Storyboard.SetTargetProperty(currentAnimation, new PropertyPath(TranslateTransform.XProperty));
            }

            if (MonthChangeDirection == AnimationDirection.Vertical)
            {
                Storyboard.SetTargetProperty(followingAnimation, new PropertyPath(TranslateTransform.YProperty));
                Storyboard.SetTargetProperty(currentAnimation, new PropertyPath(TranslateTransform.YProperty));
            }

            Storyboard.SetTargetProperty(monthButton1OpacityAnimation, new PropertyPath(MonthButton.OpacityProperty));
            Storyboard.SetTargetProperty(monthButton2OpacityAnimation, new PropertyPath(MonthButton.OpacityProperty));
            mmoveStoryboard = new Storyboard();
            Timeline.SetDesiredFrameRate(mmoveStoryboard, 28);
            mmoveStoryboard.Children.Add(followingAnimation);
            mmoveStoryboard.Children.Add(currentAnimation);
            mmoveStoryboard.Children.Add(monthButton1OpacityAnimation);
            mmoveStoryboard.Children.Add(monthButton2OpacityAnimation);
            mmoveStoryboard.Completed += new EventHandler(MoveStoryboard_Completed);
            mmoveStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Previews settings before setting the visual mode.
        /// </summary>
        /// <param name="date">Date that should be set.</param>
        protected virtual void ChangeVisualModePreview(VisibleDate date)
        {
            VisibleDate oldData = VisibleData;
            VisibleData = date;

            if (VisibleData.Equals(oldData))
            {
                OnVisibleDataChanged(new DependencyPropertyChangedEventArgs());
            }

            FindCurrentGrid(VisualMode).Visibility = Visibility.Visible;
            ChangeVisualModeIndeed();
        }

        /// <summary>
        /// Changes the visual mode depending on the <see cref="VisualModeInfo"/> property.
        /// </summary>
        protected virtual void ChangeVisualModeIndeed()
        {
            int selectedColumn, selectedRow;
            double mode1ScaleX, mode1ScaleY, dayNameScaleX, dayNameScaleY, centerX = 0, centerY = 0;
            double param = DayNamesGrid.ActualWidth / 24;
            double dayGridWidth = FindCurrentGrid(CalendarVisualMode.Days).ActualWidth;
            double dayGridHeight = FindCurrentGrid(CalendarVisualMode.Days).ActualHeight;
            double dayNameGridWidth = DayNamesGrid.ActualWidth;
            double dayNameGridHeight = DayNamesGrid.ActualHeight;
            ArrayList cellsCollection = new ArrayList();
            CalendarVisualMode oldMode = VisualModeInfo.OldMode;
            CalendarVisualMode newMode = VisualModeInfo.NewMode;
            Cell selectedCell = null;
            Duration duration = TimeSpan.FromMilliseconds(ChangeModeTime);
            ScaleTransform mode1Scale = new ScaleTransform();
            ScaleTransform mode2Scale = new ScaleTransform();
            ScaleTransform dayNameScale = new ScaleTransform();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            DoubleAnimation mode1ScaleAnimationX = new DoubleAnimation();
            DoubleAnimation mode1ScaleAnimationY = new DoubleAnimation();
            DoubleAnimation mode2ScaleAnimationX = new DoubleAnimation();
            DoubleAnimation mode2ScaleAnimationY = new DoubleAnimation();
            DoubleAnimation dayNameScaleAnimationX = new DoubleAnimation();
            DoubleAnimation dayNameScaleAnimationY = new DoubleAnimation();
            NameScope.SetNameScope(this, new NameScope());
            opacityAnimation.Duration = duration;
            mode1ScaleAnimationX.Duration = duration;
            mode1ScaleAnimationY.Duration = duration;
            mode2ScaleAnimationX.Duration = duration;
            mode2ScaleAnimationY.Duration = duration;
            dayNameScaleAnimationX.Duration = duration;
            dayNameScaleAnimationY.Duration = duration;
            opacityAnimation.FillBehavior = FillBehavior.Stop;
            CalendarEdit calendarEdit = new CalendarEdit();

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.WeekNumbers)
            {
                cellsCollection = this.FindCurrentGrid(CalendarVisualMode.WeekNumbers).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Months)
            {
                cellsCollection = this.FindCurrentGrid(CalendarVisualMode.Months).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Days)
            {
                cellsCollection = this.FindCurrentGrid(CalendarVisualMode.Days).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.Months)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.Months).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Days)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.Months).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Years)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.Years).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.Months)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.Years).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.YearsRange)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.YearsRange).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.YearsRange && newMode == CalendarVisualMode.Years)
            {
                cellsCollection = FindCurrentGrid(CalendarVisualMode.YearsRange).CellsCollection;
            }

            foreach (Cell cell in cellsCollection)
            {
                if (cell.IsSelected)
                {
                    selectedCell = cell;
                }
            }

            if (selectedCell != null)
            {
                selectedColumn = Grid.GetColumn((UIElement)selectedCell);
                selectedRow = Grid.GetRow((UIElement)selectedCell);
            }
            else
            {
                throw new ArgumentNullException("Selected cell can not be null");
            }

            switch (selectedColumn)
            {
                case 0:
                    centerX = 0;
                    break;

                case 1:
                    centerX = ((selectedCell.ActualWidth + selectedCell.ActualWidth) / 2) - param;
                    break;

                case 2:
                    centerX = 2 * (((selectedCell.ActualWidth + selectedCell.ActualWidth) / 2) + param);
                    break;

                case 3:
                    centerX = 4 * selectedCell.ActualWidth;
                    break;

                case 4:
                    centerX = 6 * selectedCell.ActualWidth;
                    break;

                case 5:
                    centerX = 8 * selectedCell.ActualWidth;
                    break;

                case 6:
                    centerX = 10 * selectedCell.ActualWidth;
                    break;

                case 7:
                    centerX = 12 * selectedCell.ActualWidth;
                    break;
            }

            switch (selectedRow)
            {
                case 0:
                    centerY = 0;
                    break;

                case 1:
                    centerY = (selectedCell.ActualHeight + selectedCell.ActualHeight) / 2;
                    break;

                case 2:
                    centerY = 3 * selectedCell.ActualHeight;
                    break;

                case 3:
                    centerY = 5 * selectedCell.ActualHeight;
                    break;

                case 4:
                    centerY = 7 * selectedCell.ActualHeight;
                    break;

                case 5:
                    centerY = 9 * selectedCell.ActualHeight;
                    break;

                case 6:
                    centerY = 11 * selectedCell.ActualHeight;
                    break;
            }

            mode1Scale.CenterX = centerX;
            mode1Scale.CenterY = centerY;
            mode2Scale.CenterX = centerX;
            mode2Scale.CenterY = centerY;
            dayNameScale.CenterX = centerX;
            dayNameScale.CenterY = centerY;

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.Months)
            {
                mode1ScaleX = selectedCell.ActualWidth / dayGridWidth;
                mode1ScaleY = selectedCell.ActualHeight / (dayGridHeight + dayNameGridHeight);
                mode1Scale.CenterX = centerX;
                mode1Scale.CenterY = centerY - 15;
                mode1ScaleAnimationX.To = mode1ScaleX;
                mode1ScaleAnimationY.To = mode1ScaleY;
                mode2Scale.ScaleX = 4;
                mode2Scale.ScaleY = 3;
                mode2ScaleAnimationX.To = 1;
                mode2ScaleAnimationY.To = 1;
                dayNameScaleX = selectedCell.ActualWidth / dayNameGridWidth;
                dayNameScaleY = selectedCell.ActualHeight / (dayGridHeight + dayNameGridHeight);
                dayNameScaleAnimationX.To = dayNameScaleX;
                dayNameScaleAnimationY.To = dayNameScaleY;
                selectedCell.Opacity = 0;
                opacityAnimation.To = 1;
                FindCurrentGrid(CalendarVisualMode.Months).RenderTransform = mode2Scale;
                FindCurrentGrid(CalendarVisualMode.Days).RenderTransform = mode1Scale;
                DayNamesGrid.RenderTransform = dayNameScale;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Days)
            {
                mode1ScaleX = selectedCell.ActualWidth / dayGridWidth;
                mode1ScaleY = selectedCell.ActualHeight / (dayGridHeight + dayNameGridHeight);
                mode1Scale.ScaleX = mode1ScaleX;
                mode1Scale.ScaleY = mode1ScaleY;
                mode1Scale.CenterX = centerX;
                mode1Scale.CenterY = centerY - 15;
                mode1ScaleAnimationX.To = 1;
                mode1ScaleAnimationY.To = 1;
                mode2Scale.ScaleX = 1;
                mode2Scale.ScaleY = 1;
                mode2ScaleAnimationX.To = 4;
                mode2ScaleAnimationY.To = 3;
                dayNameScaleX = selectedCell.ActualWidth / dayNameGridWidth;
                dayNameScaleY = selectedCell.ActualHeight / (dayGridHeight + dayNameGridHeight);
                dayNameScale.ScaleX = dayNameScaleX;
                dayNameScale.ScaleY = dayNameScaleY;
                dayNameScaleAnimationX.To = 1;
                dayNameScaleAnimationY.To = 1;
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                FindCurrentGrid(CalendarVisualMode.Months).RenderTransform = mode2Scale;
                FindCurrentGrid(CalendarVisualMode.Days).RenderTransform = mode1Scale;
                DayNamesGrid.RenderTransform = dayNameScale;
                DayNamesGrid.Visibility = Visibility.Visible;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Years)
            {
                mode1ScaleX = selectedCell.ActualWidth / CurrentMonthGrid.ActualWidth;
                mode1ScaleY = selectedCell.ActualHeight / CurrentMonthGrid.ActualHeight;
                mode1ScaleAnimationX.To = mode1ScaleX;
                mode1ScaleAnimationY.To = mode1ScaleY;
                mode2Scale.ScaleX = 4;
                mode2Scale.ScaleY = 3;
                mode2ScaleAnimationX.To = 1;
                mode2ScaleAnimationY.To = 1;
                selectedCell.Opacity = 0;
                opacityAnimation.To = 1;
                FindCurrentGrid(CalendarVisualMode.Months).RenderTransform = mode1Scale;
                FindCurrentGrid(CalendarVisualMode.Years).RenderTransform = mode2Scale;
                if (this.DisableDateSelection == true)
                {
                    CurrentDayGrid.Visibility = Visibility.Hidden;
                    DayNamesGrid.Visibility = Visibility.Hidden;
                }
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.Months)
            {
                mode1ScaleX = selectedCell.ActualWidth / CurrentMonthGrid.ActualWidth;
                mode1ScaleY = selectedCell.ActualHeight / CurrentMonthGrid.ActualHeight;
                mode1ScaleAnimationX.To = 4;
                mode1ScaleAnimationY.To = 3;
                mode2Scale.ScaleX = mode1ScaleX;
                mode2Scale.ScaleY = mode1ScaleY;
                mode2ScaleAnimationX.To = 1;
                mode2ScaleAnimationY.To = 1;
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                FindCurrentGrid(CalendarVisualMode.Years).RenderTransform = mode1Scale;
                FindCurrentGrid(CalendarVisualMode.Months).RenderTransform = mode2Scale;
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.YearsRange)
            {
                mode1ScaleX = selectedCell.ActualWidth / CurrentYearGrid.ActualWidth;
                mode1ScaleY = selectedCell.ActualHeight / CurrentYearGrid.ActualHeight;
                mode1ScaleAnimationX.To = mode1ScaleX;
                mode1ScaleAnimationY.To = mode1ScaleY;
                mode2Scale.ScaleX = 4;
                mode2Scale.ScaleY = 3;
                mode2ScaleAnimationX.To = 1;
                mode2ScaleAnimationY.To = 1;
                selectedCell.Opacity = 0;
                opacityAnimation.To = 1;
                FindCurrentGrid(CalendarVisualMode.Years).RenderTransform = mode1Scale;
                FindCurrentGrid(CalendarVisualMode.YearsRange).RenderTransform = mode2Scale;
            }

            if (oldMode == CalendarVisualMode.YearsRange && newMode == CalendarVisualMode.Years)
            {
                mode1ScaleX = selectedCell.ActualWidth / CurrentYearGrid.ActualWidth;
                mode1ScaleY = selectedCell.ActualHeight / CurrentYearGrid.ActualHeight;
                mode1ScaleAnimationX.To = 4;
                mode1ScaleAnimationY.To = 3;
                mode2Scale.ScaleX = mode1ScaleX;
                mode2Scale.ScaleY = mode1ScaleY;
                mode2ScaleAnimationX.To = 1;
                mode2ScaleAnimationY.To = 1;
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                FindCurrentGrid(CalendarVisualMode.YearsRange).RenderTransform = mode1Scale;
                FindCurrentGrid(CalendarVisualMode.Years).RenderTransform = mode2Scale;
            }

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.WeekNumbers)
            {
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                this.FindCurrentGrid(CalendarVisualMode.Days).RenderTransform = mode2Scale;
                this.FindCurrentGrid(CalendarVisualMode.WeekNumbers).RenderTransform = mode1Scale;
                DayNamesGrid.Visibility = Visibility.Hidden;
                CurrentWeekNumbersGrid.Visibility = Visibility.Visible;
                UpdateWeekNumbersContainer();
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Months)
            {
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                this.FindCurrentGrid(CalendarVisualMode.WeekNumbers).RenderTransform = mode2Scale;
                this.FindCurrentGrid(CalendarVisualMode.Months).RenderTransform = mode1Scale;
                DayNamesGrid.Visibility = Visibility.Hidden;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Days)
            {
                selectedCell.Opacity = 1;
                opacityAnimation.To = 0;
                this.FindCurrentGrid(CalendarVisualMode.WeekNumbers).RenderTransform = mode2Scale;
                this.FindCurrentGrid(CalendarVisualMode.Days).RenderTransform = mode1Scale;
            }

            RegisterName("Mode1Scale", mode1Scale);
            RegisterName("Mode2Scale", mode2Scale);
            RegisterName("SelecteCell", selectedCell);
            Storyboard.SetTargetName(mode1ScaleAnimationX, "Mode1Scale");
            Storyboard.SetTargetName(mode1ScaleAnimationY, "Mode1Scale");
            Storyboard.SetTargetName(mode2ScaleAnimationX, "Mode2Scale");
            Storyboard.SetTargetName(mode2ScaleAnimationY, "Mode2Scale");
            Storyboard.SetTargetName(opacityAnimation, "SelecteCell");
            Storyboard.SetTargetProperty(mode1ScaleAnimationX, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(mode1ScaleAnimationY, new PropertyPath(ScaleTransform.ScaleYProperty));
            Storyboard.SetTargetProperty(mode2ScaleAnimationX, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(mode2ScaleAnimationY, new PropertyPath(ScaleTransform.ScaleYProperty));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(MonthCell.OpacityProperty));
            mvisualModeStoryboard = new Storyboard();
            Timeline.SetDesiredFrameRate(mvisualModeStoryboard, 28);

            mvisualModeStoryboard.Children.Add(mode1ScaleAnimationX);
            mvisualModeStoryboard.Children.Add(mode1ScaleAnimationY);
            mvisualModeStoryboard.Children.Add(mode2ScaleAnimationX);
            mvisualModeStoryboard.Children.Add(mode2ScaleAnimationY);
            mvisualModeStoryboard.Children.Add(opacityAnimation);

            if (oldMode == CalendarVisualMode.Days || newMode == CalendarVisualMode.Days)
            {
                RegisterName("DayNameScale", dayNameScale);
                Storyboard.SetTargetName(dayNameScaleAnimationX, "DayNameScale");
                Storyboard.SetTargetName(dayNameScaleAnimationY, "DayNameScale");
                Storyboard.SetTargetProperty(dayNameScaleAnimationX, new PropertyPath(ScaleTransform.ScaleXProperty));
                Storyboard.SetTargetProperty(dayNameScaleAnimationY, new PropertyPath(ScaleTransform.ScaleYProperty));
                mvisualModeStoryboard.Children.Add(dayNameScaleAnimationX);
                mvisualModeStoryboard.Children.Add(dayNameScaleAnimationY);
            }

            FindCurrentGrid(oldMode).Focus();
            mvisualModeStoryboard.Completed += new EventHandler(VisualModeStoryboard_OnCompleted);
            mvisualModeStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Changes the visual mode in up direction.
        /// </summary>
        /// <param name="direction">The direction mode.</param>
        protected virtual void ChangeVisualMode(ChangeVisualModeDirection direction)
        {
            CalendarVisualMode mode = VisualMode;
            CalendarEditGrid currentGrid = null;
            this.HideWeekNumbersForYearContainer();

            if (mode == CalendarVisualMode.Days & this.IsWeekCellClicked == true)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualMode = CalendarVisualMode.WeekNumbers;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.WeekNumbers);
                    this.ShowWeekNumbersForYearContainer();
                }
                else
                {
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.WeekNumbers, CalendarVisualMode.WeekNumbers);
                }
            }

            if (mode == CalendarVisualMode.Days)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualMode = CalendarVisualMode.Months;
                    if (VisualMode != CalendarVisualMode.Days)
                    {
                        Rectangle rect1 = this.GetTemplateChild("rect") as Rectangle;
                        if (rect1 != null)
                        {
                            rect1.Visibility = Visibility.Collapsed;
                        }
                    }
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.Months);
                }
                else
                {
                    if (VisualMode != CalendarVisualMode.Days)
                    {
                        Rectangle rect1 = this.GetTemplateChild("rect") as Rectangle;
                        if (rect1 != null)
                        {
                            rect1.Visibility = Visibility.Collapsed;
                        }
                    }
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.Days);
                }
            }

            if (mode == CalendarVisualMode.Months)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualMode = CalendarVisualMode.Years;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Years);
                }
                else
                {
                    VisualMode = CalendarVisualMode.Days;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days);
                }
            }

            if (mode == CalendarVisualMode.Years)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualMode = CalendarVisualMode.YearsRange;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Years, CalendarVisualMode.YearsRange);
                }
                else
                {
                    VisualMode = CalendarVisualMode.Months;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Years, CalendarVisualMode.Months);
                }
            }

            if (mode == CalendarVisualMode.YearsRange)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.YearsRange, CalendarVisualMode.YearsRange);
                }
                else
                {
                    VisualMode = CalendarVisualMode.Years;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.YearsRange, CalendarVisualMode.Years);
                }
            }

            if (mode == CalendarVisualMode.WeekNumbers)
            {
                if (direction == ChangeVisualModeDirection.Up)
                {
                    VisualMode = CalendarVisualMode.Months;
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.WeekNumbers, CalendarVisualMode.Months);
                }
                else
                {
                    VisualModeInfo = new VisualModeHistory(CalendarVisualMode.WeekNumbers, CalendarVisualMode.Days);
                }
            }

            if (VisualModeInfo.OldMode != VisualModeInfo.NewMode)
            {
                m_monthButton1.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
                m_monthButton2.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
                currentGrid = FindCurrentGrid(VisualMode);
                currentGrid.Initialize(VisibleData, Culture, Calendar);
                currentGrid.Visibility = Visibility.Visible;
                currentGrid.Focus();
                ChangeVisualModeIndeed();
                UpdateWeekNumbersContainer();
            }
        }

        /// <summary>
        /// Hides week numbers container.
        /// </summary>
        private void HideWeekNumbersContainer()
        {
            m_weekNumbersContainer.Visibility = Visibility.Collapsed;
            m_mainGrid.ColumnDefinitions[1].Width = GridLength.Auto;
        }

        /// <summary>
        /// Shows week numbers container.
        /// </summary>
        private void ShowWeekNumbersContainer()
        {
            m_weekNumbersContainer.Visibility = Visibility.Visible;
            m_mainGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
        }

        /// <summary>
        /// Shows week numbers for year container.
        /// </summary>
        private void ShowWeekNumbersForYearContainer()
        {
            this.wcurrentweekNumbersContainer.Visibility = Visibility.Visible;
            this.wfollowweekNumbersContainer.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides week numbers for year container.
        /// </summary>
        private void HideWeekNumbersForYearContainer()
        {
            this.wcurrentweekNumbersContainer.Visibility = Visibility.Hidden;
            this.wfollowweekNumbersContainer.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Verifies whether navigate buttons are enabled.
        /// </summary>
        protected virtual void NavigateButtonVerify()
        {
            int nextYear, prevYear;
            Date nextCheckDate = new Date();
            Date prevCheckDate = new Date();
            Date minDate;
            Date maxDate;
            if (MinMaxHidden)
            {
                minDate = new Date(MinDate, Calendar);
                if (MaxDate > Calendar.MaxSupportedDateTime)
                    MaxDate = Calendar.MaxSupportedDateTime;
                maxDate = new Date(MaxDate, Calendar);
            }
            else
            {
                minDate = new Date(miDate, Calendar);
                maxDate = new Date(mxDate, Calendar);
            }

            //Date minDate = new Date(MinDate, Calendar);
            //Date maxDate = new Date(MaxDate, Calendar);
            Date curDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
            Cell nextCell = null, prevCell = null;
            ArrayList cellsCollection = FindCurrentGrid(VisualMode).CellsCollection;

            if (VisualMode == CalendarVisualMode.WeekNumbers)
            {
                nextCheckDate = new Date(VisibleData.VisibleYear + 1, VisibleData.VisibleMonth, 1);
                prevCheckDate = new Date(VisibleData.VisibleYear - 1, VisibleData.VisibleMonth, 1);
            }

            if (VisualMode == CalendarVisualMode.Days)
            {
                nextCheckDate = curDate.AddMonthToDate(1);
                prevCheckDate = curDate.AddMonthToDate(-1);
            }
            else if (VisualMode == CalendarVisualMode.Months)
            {
                nextCheckDate = new Date(VisibleData.VisibleYear + 1, VisibleData.VisibleMonth, 1);
                prevCheckDate = new Date(VisibleData.VisibleYear - 1, VisibleData.VisibleMonth, 1);
            }
            else if (VisualMode == CalendarVisualMode.Years)
            {
                nextCell = cellsCollection[cellsCollection.Count - 1] as YearCell;
                prevCell = cellsCollection[0] as YearCell;
                nextYear = (nextCell as YearCell).Year;
                prevYear = (prevCell as YearCell).Year;
                nextCheckDate = new Date(nextYear + 1, VisibleData.VisibleMonth, 1);
                prevCheckDate = new Date(prevYear - 1, VisibleData.VisibleMonth, 1);
            }
            else if (VisualMode == CalendarVisualMode.YearsRange)
            {
                nextCell = cellsCollection[cellsCollection.Count - 1] as YearRangeCell;
                prevCell = cellsCollection[0] as YearRangeCell;
                nextYear = (nextCell as YearRangeCell).Years.StartYear;
                prevYear = (prevCell as YearRangeCell).Years.StartYear;
                nextCheckDate = new Date(nextYear + 10, VisibleData.VisibleMonth, 1);
                prevCheckDate = new Date(prevYear - 10, VisibleData.VisibleMonth, 1);
            }

            if (m_nextButton != null && m_prevButton != null)
            {
                if (VisualMode != CalendarVisualMode.Days && VisualMode != CalendarVisualMode.Months && VisualMode != CalendarVisualMode.WeekNumbers)
                {
                    if (nextCheckDate > maxDate || nextCell.Visibility != Visibility.Visible)
                    {
                        m_nextButton.Enabled = false;
                    }
                    else
                    {
                        m_nextButton.Enabled = true;
                    }

                    if (prevCheckDate < minDate || prevCell.Visibility != Visibility.Visible)
                    {
                        m_prevButton.Enabled = false;
                    }
                    else
                    {
                        m_prevButton.Enabled = true;
                    }
                }
                else
                {
                    if (nextCheckDate > maxDate)
                    {
                        m_nextButton.Enabled = false;
                    }
                    else
                    {
                        m_nextButton.Enabled = true;
                    }

                    if (prevCheckDate < minDate)
                    {
                        m_prevButton.Enabled = false;
                    }
                    else
                    {
                        m_prevButton.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the background for each day cell during vertical animation.
        /// </summary>
        /// <param name="dayGrid">The <see cref="DayGrid"/> that contains day
        /// cells.</param>
        /// <param name="currentBrush">The brush for the background of current month cells.</param>
        /// <param name="followingBrush">The brush for the background of following month
        /// cells.</param>
        /// <param name="isCurrent">Indicates whether day grid is current.</param>
        protected internal void SetBackground(DayGrid dayGrid, SolidColorBrush currentBrush, SolidColorBrush followingBrush, bool isCurrent)
        {
            foreach (DayCell dc in CurrentDayGrid.CellsCollection)
            {
                if (isCurrent)
                {
                    if (dc.IsCurrentMonth)
                    {
                        dc.Background = currentBrush;
                    }
                    else
                    {
                        dc.Background = followingBrush;
                    }
                }
                else
                {
                    if (dc.IsCurrentMonth)
                    {
                        dc.Background = followingBrush;
                    }
                    else
                    {
                        dc.Background = currentBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the layOn ratio for the horizontal animation.
        /// </summary>
        /// <param name="current">The <see cref="DayGrid"/> for which the
        /// value should be calculated.</param>
        /// <returns>
        /// The layOn ratio.
        /// </returns>
        protected virtual double CalculateLayOnValue(DayGrid current)
        {
            int k = 0;
            int idayCellsCount = current.CellsCollection.Count;
            DayCell tmp = (DayCell)current.CellsCollection[0];
            double ilayOnRatio = tmp.ActualHeight;

            for (int i = idayCellsCount / 2; i < idayCellsCount; i++)
            {
                DayCell dc = (DayCell)current.CellsCollection[i];
                if (!dc.IsCurrentMonth)
                {
                    k++;
                }
            }

            if (k > 7)
            {
                ilayOnRatio = 2 * ilayOnRatio;
            }

            return ilayOnRatio;
        }

        /// <summary>
        /// Shows or hides the selection border when mouse enters or
        /// leaves the <see cref="DayNameCell"/>.
        /// </summary>
        /// <param name="border">The <see cref="System.Windows.Controls.Border"/>
        /// object that will be highlighted.</param>
        /// <param name="state">The state of the highlight
        /// animation.</param>
        protected virtual void Highlight(Border border, HighlightSate state)
        {
            Duration duration = new Duration(TimeSpan.FromMilliseconds(600));
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            ThicknessAnimation thicknessAnimation = new ThicknessAnimation();

            if (state == HighlightSate.Begin)
            {
                opacityAnimation.To = 1;
                thicknessAnimation.To = new Thickness(1);
            }
            else
            {
                opacityAnimation.To = 0;
                thicknessAnimation.To = new Thickness(0);
            }

            opacityAnimation.Duration = duration;
            thicknessAnimation.Duration = duration;
            NameScope.SetNameScope(this, new NameScope());
            RegisterName("SelectionBorder", border);
            Storyboard sb = new Storyboard();
            Timeline.SetDesiredFrameRate(sb, 30);
            Storyboard.SetTargetName(opacityAnimation, "SelectionBorder");
            Storyboard.SetTargetName(thicknessAnimation, "SelectionBorder");
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Border.OpacityProperty));
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(Border.BorderThicknessProperty));
            sb.Children.Add(opacityAnimation);
            sb.Children.Add(thicknessAnimation);
            sb.Begin(border);
        }

        /// <summary>
        /// Applies the changes in the <see cref="DataTemplatesDictionary"/>.
        /// </summary>
        /// <param name="current">DayGrid that is visible in the current
        /// moment.</param>
        /// <param name="action">The action of DayGrid.</param>
        /// <param name="item">The <see cref="DataTemplateItem"/> changes were occurred on.</param>
        protected virtual void SetDateDataTemplates(DayGrid current, CollectionChangedAction action, DataTemplateItem item)
        {
            DataTemplate template = DayCellsDataTemplate;
            DataTemplateSelector selector = DayCellsDataTemplateSelector;

            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            if (item == null && action != CollectionChangedAction.Reset)
            {
                throw new ArgumentNullException("item");
            }

            if (action == CollectionChangedAction.Reset)
            {
                current.UpdateTemplateAndSelector(template, selector, null);
            }
            else
            {
                Hashtable cellsCollection = current.DateCells;

                if (cellsCollection.ContainsKey(item.Date))
                {
                    DayCell dc = (DayCell)cellsCollection[item.Date];

                    if (action == CollectionChangedAction.Add)
                    {
                        dc.SetTemplate(item.Template);
                    }

                    if (action == CollectionChangedAction.Remove)
                    {
                        dc.UpdateCellTemplateAndSelector(template, selector);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the changes in the <see cref="StylesDictionary"/>.
        /// </summary>
        /// <param name="current">DayGrid that is visible in the current
        /// moment.</param>
        /// <param name="action">The current action.</param>
        /// <param name="item">The <see cref="StyleItem"/> changes were occurred on.</param>
        protected virtual void SetDateStyles(DayGrid current, CollectionChangedAction action, StyleItem item)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }

            if (item == null && action != CollectionChangedAction.Reset)
            {
                throw new ArgumentNullException("item");
            }

            if (action == CollectionChangedAction.Reset)
            {
                current.UpdateStyles(DayCellsStyle, null);
            }
            else
            {
                Hashtable cellsCollection = current.DateCells;

                if (cellsCollection.ContainsKey(item.Date))
                {
                    DayCell dc = (DayCell)cellsCollection[item.Date];

                    if (action == CollectionChangedAction.Add)
                    {
                        dc.SetStyle(item.Style);
                    }

                    if (action == CollectionChangedAction.Remove)
                    {
                        dc.SetStyle(DayCellsStyle);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the <see cref="DayCell"/> templates.
        /// </summary>
        /// <param name="current">The <see cref="DayGrid"/> that is visible in the current
        /// moment.</param>
        protected internal virtual void InitilizeDayCellTemplates(DayGrid current)
        {
            current.UpdateTemplateAndSelector(DayCellsDataTemplate, DayCellsDataTemplateSelector, DateDataTemplates);
        }

        /// <summary>
        /// Initializes the <see cref="DayCell"/> styles.
        /// </summary>
        /// <param name="current">The <see cref="DayGrid"/> that is visible in the current
        /// moment.</param>
        protected internal virtual void InitilizeDayCellStyles(DayGrid current)
        {
            current.UpdateStyles(DayCellsStyle, DateStyles);
        }

        #endregion protected virtual

        #region private static

        /// <summary>
        /// Validates the <see cref="FrameMovingTime"/> property.
        /// </summary>
        /// <param name="value">Value that should be validated.</param>
        /// <returns>Validation result.</returns>
        private static bool ValidateFrameMovingTime(object value)
        {
            int ivalue = (int)value;
            return 0 <= ivalue;
        }

        #endregion private static

        #region private

        /// <summary>
        /// Sets tooltip for the cell.
        /// </summary>
        /// <param name="rowIndex">row index to set tooltip</param>
        /// <param name="colIndex">column index to set tooltip</param>
        /// <param name="current">current days grid</param>
        /// <param name="tooltip">The tooltip.</param>
        private void SetCellToolTip(int rowIndex, int colIndex, DayGrid current, ToolTip tooltip)
        {
            ArrayList cellsCollection = current.CellsCollection;

            for (int i = 0, cnt = cellsCollection.Count; i < cnt; i++)
            {
                int row = Grid.GetRow((UIElement)cellsCollection[i]);
                int col = Grid.GetColumn((UIElement)cellsCollection[i]);

                if (row == rowIndex && col == colIndex)
                {
                    DayCell dc = current.CellsCollection[i] as DayCell;
                    dc.ToolTip = tooltip;
                }
            }
        }

        /// <summary>
        /// Checks whether the month changing is available.
        /// </summary>
        /// <param name="month">Number of months that should be added to
        /// the current month. </param>
        /// <returns>
        /// True, if changing is not available.
        /// </returns>
        /// <remarks>
        /// For the days mode only.
        /// </remarks>
        private bool IsOutOfDateRange(int month)
        {
            if (VisualMode != CalendarVisualMode.Days)
            {
                throw new NotSupportedException("This function should be called in Days mode only.");
            }
            else
            {
                Date minDate;
                Date maxDate;
                if (MinMaxHidden)
                {
                    minDate = new Date(MinDate, Calendar);
                    maxDate = new Date(MaxDate, Calendar);
                }
                else
                {
                    minDate = new Date(miDate, Calendar);
                    maxDate = new Date(mxDate, Calendar);
                }
                //Date minDate = new Date(MinDate, Calendar);
                //Date maxDate = new Date(MaxDate, Calendar);
                int day = month > 0 ? 1 : 31;
                Date curDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, day);
                Date nextDate = new Date();
                nextDate = curDate.AddMonthToDate(month);

                if (nextDate > maxDate || nextDate < minDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks whether the year, years range changing is available.
        /// </summary>
        /// <param name="direction">Changing direction.</param>
        /// <param name="returnDate">Next visible date.</param>
        /// <returns>
        /// True, if changing is not available.
        /// </returns>
        /// <remarks>
        /// Is not used in the days mode.
        /// </remarks>
        private bool IsOutOfDateRange(MoveDirection direction, ref Date returnDate)
        {
            if (VisualMode == CalendarVisualMode.Days)
            {
                throw new NotSupportedException("This function should not be called in Days mode.");
            }
            else
            {
                int year = 0, corection = 0;
                Date minDate;
                Date maxDate;
                if (MinMaxHidden)
                {
                    minDate = new Date(MinDate, Calendar);
                    maxDate = new Date(MaxDate, Calendar);
                }
                else
                {
                    minDate = new Date(miDate, Calendar);
                    maxDate = new Date(mxDate, Calendar);
                }

                //Date minDate = new Date(MinDate, Calendar);
                //Date maxDate = new Date(MaxDate, Calendar);
                Date curDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                Date nextDate = new Date();
                Date checkDate = new Date();
                Cell cell = null;
                ArrayList cellsCollection = FindCurrentGrid(VisualMode).CellsCollection;

                if (VisualMode == CalendarVisualMode.WeekNumbers)
                {
                    if (direction == MoveDirection.Next)
                    {
                        year = curDate.Year + 1;
                    }
                    else
                    {
                        year = curDate.Year - 1;
                    }
                }

                if (VisualMode == CalendarVisualMode.Months)
                {
                    if (direction == MoveDirection.Next)
                    {
                        year = curDate.Year + 1;
                    }
                    else
                    {
                        year = curDate.Year - 1;
                    }

                    checkDate = nextDate;
                }

                if (VisualMode == CalendarVisualMode.Years)
                {
                    if (direction == MoveDirection.Next)
                    {
                        corection = 1;
                        cell = cellsCollection[cellsCollection.Count - 1] as YearCell;
                        year = (cell as YearCell).Year;
                    }
                    else
                    {
                        corection = -1;
                        cell = cellsCollection[0] as YearCell;
                        year = (cell as YearCell).Year; // -8
                    }

                    if (cell.Visibility != Visibility.Visible)
                    {
                        return true;
                    }
                }

                if (VisualMode == CalendarVisualMode.YearsRange)
                {
                    if (direction == MoveDirection.Next)
                    {
                        corection = 10;
                        cell = cellsCollection[cellsCollection.Count - 1] as YearRangeCell;
                        year = (cell as YearRangeCell).Years.StartYear;
                    }
                    else
                    {
                        corection = -10;
                        cell = cellsCollection[0] as YearRangeCell;
                        year = (cell as YearRangeCell).Years.StartYear; // - 90;
                    }

                    if (cell.Visibility != Visibility.Visible)
                    {
                        return true;
                    }
                }

                nextDate = new Date(year, curDate.Month, 1);
                checkDate = new Date(year + corection, curDate.Month, 1);
                returnDate = nextDate;

                if (checkDate > maxDate || checkDate < minDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds the specified number of months to the visible month.
        /// </summary>
        /// <param name="month">Number of months. It can be
        /// negative or positive.</param>
        private void AddMonth(int month)
        {
            VisibleDate result = VisibleData;
            int count = Math.Abs(month);
            int months = 0;
            int years = 0;

            if (count >= 12)
            {
                years = month / 12;
                months = month % 12;
            }
            else
            {
                months = month;
            }

            int tempMonth = VisibleData.VisibleMonth + months;
            result.VisibleYear += years;

            if (tempMonth > 12)
            {
                result.VisibleYear += tempMonth / 12;
                result.VisibleMonth = tempMonth % 12;
            }

            if (tempMonth < 1)
            {
                result.VisibleMonth = 12 + tempMonth;
                result.VisibleYear--;
            }

            if (tempMonth >= 1 && tempMonth <= 12)
            {
                result.VisibleMonth = tempMonth;
            }

            VisibleData = result;
        }

        /// <summary>
        /// Scrolls the calendar to the next or previous month if the date
        /// that does not belong to the current month.
        /// </summary>
        /// <param name="month">Next month.</param>
        private void ScrollMonth(int month)
        {
            int ifutureMonth = DateUtils.AddMonth(VisibleData.VisibleMonth, 1);

            if (!IsStoryboardActive(mmonthStoryboard))
            {
                if (month == ifutureMonth)
                {
                    BeginMoving(MoveDirection.Next, 1);
                }
                else
                {
                    BeginMoving(MoveDirection.Prev, -1);
                }
            }
        }

        /// <summary>
        /// Adds the specified number of years to the visible year.
        /// </summary>
        /// <param name="year">Number of years. It can be
        /// negative or positive.</param>
        private void AddYear(int year)
        {
            VisibleDate result = VisibleData;
            result.VisibleYear += year;
            VisibleData = result;
        }

        /// <summary>
        /// Calculates the month delta.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>Return the month data</returns>
        private int CalculateMonthDelta(DateTime startDate, DateTime endDate)
        {
            int result = 0;

            while (startDate != endDate)
            {
                if (startDate < endDate)
                {
                    result++;
                    startDate = startDate.AddMonths(1);
                }
                else
                {
                    result--;
                    startDate = startDate.AddMonths(-1);
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies the select.
        /// </summary>
        /// <param name="date">The system date.</param>
        /// <param name="modifiers">The modifiers.</param>
        private void MultiplySelect(DateTime date, ModifierKeys modifiers)
        {
            DayGrid current;

            if (modifiers == ModifierKeys.Control)
            {
                // Selection with Control
                if (SelectedDates.Contains(date))
                {
                    SelectedDates.Remove(date);
                    m_dateSetManual = true;
                    Date = date;
                }
                else
                {
                    SelectedDates.Add(date);

                    m_dateSetManual = true;
                    Date = date;
                }
            }
            else
            {
                // Selection with Shift or with Control & Shift
                current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
                DateTime startDate, endDate;
                startDate = m_shiftDate;
                endDate = date;
                LockSelectedDatesUpdate();

                if (modifiers == ModifierKeys.Shift)
                {
                    SelectedDates.Clear();
                }

                if (startDate <= endDate)
                {
                    while (startDate <= endDate)
                    {
                        SelectedDates.Add(startDate);
                        startDate = startDate.AddDays(1);
                    }
                }
                else
                {
                    while (startDate >= endDate)
                    {
                        SelectedDates.Add(startDate);
                        startDate = startDate.AddDays(-1);
                    }
                }

                m_dateSetManual = true;
                Date = date;
                UnLockSelectedDatesUpdate();
                SelectedDatesList.Clear();
                //NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SelectedDates);
                //ProcessSelectedDatesCollectionChange(args);
            }
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SelectedDates);
            ProcessSelectedDatesCollectionChange(args);
        }

        /// <summary>
        /// Checks whether the storyboard is active.
        /// </summary>
        /// <param name="sb">The storyboard to be checked.</param>
        /// <returns>
        /// True if storyboard is active; otherwise false.
        /// </returns>
        private bool IsStoryboardActive(Storyboard sb)
        {
            if (sb == null)
            {
                return false;
            }
            else
            {
                ClockState cs = sb.GetCurrentState(this);
                if (cs == ClockState.Active)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Switches calendar to the month to which the <see cref="Date"/>
        /// property value belongs.
        /// </summary>
        /// <returns>
        /// Index of the cell which date is equal to the <see cref="Date"/> property.
        /// </returns>
        private int ValidateFocusIndex()
        {
            DayGrid current;
            int inewFocusedCellIndex = 0;
            current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
            bool hasDate = false;

            for (int i = 0; i < current.CellsCollection.Count; i++)
            {
                DayCell dc = (DayCell)current.CellsCollection[i];
                if (dc.IsDate)
                {
                    hasDate = true;
                    inewFocusedCellIndex = i;
                }
            }

            if (!hasDate)
            {
                VisibleDate result = VisibleData;
                result.VisibleMonth = Calendar.GetMonth(Date);
                result.VisibleYear = Calendar.GetYear(Date);
                VisibleData = result;

                for (int i = 0; i < current.CellsCollection.Count; i++)
                {
                    DayCell dc = (DayCell)current.CellsCollection[i];
                    if (dc.IsDate)
                    {
                        inewFocusedCellIndex = i;
                    }
                }
            }

            return inewFocusedCellIndex;
        }

        /// <summary>
        /// Calculates new focused cell index.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="direction">-1 for the left direction, 1 for the right direction</param>
        /// <param name="moved">True if month was changed.</param>
        /// <returns>New focused cell index.</returns>
        private int SetFocusedCellIndex(int oldValue, int direction, out bool moved)
        {
            if (direction != 1 && direction != -1)
            {
                throw new ArgumentException("admissible values are: 1 and -1");
            }

            DayCell dc;
            int day;
            int icelsCount = CurrentDayGrid.CellsCollection.Count;
            int inewFocusedCellIndex = oldValue;
            DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
            inewFocusedCellIndex += direction;
            moved = false;

            if (inewFocusedCellIndex < 0 || inewFocusedCellIndex >= icelsCount)
            {
                dc = (DayCell)current.CellsCollection[current.FocusedCellIndex];
                day = dc.Date.Day;
                BeginMoving(MoveDirection.Prev, direction);
                moved = true;
                current = FollowingDayGrid;

                for (int i = 0; i < current.CellsCollection.Count; i++)
                {
                    DayCell cell = (DayCell)current.CellsCollection[i];

                    if (cell.Date.Day == day && cell.IsCurrentMonth)
                    {
                        inewFocusedCellIndex = i;
                    }
                }

                inewFocusedCellIndex += direction;
            }

            return inewFocusedCellIndex;
        }

        /// <summary>
        /// Changes the <see cref="VisibleData"/> property in accordance with the calendar.
        /// </summary>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> object used for setting the
        /// <see cref="VisibleData"/> property.</param>
        private void CoerceVisibleData(Calendar calendar)
        {
            SetCorrectDate();

            VisibleDate result = VisibleData;

            result.VisibleMonth = calendar.GetMonth(Date);
            result.VisibleYear = calendar.GetYear(Date);
            UpdateSelectedDatesList();

            if (VisibleData.Equals(result))
            {
                OnVisibleDataChanged(new DependencyPropertyChangedEventArgs());
            }

            //if (result.VisibleYear < MinDate.Year)
            //{
            //    result.VisibleMonth = calendar.GetMonth(MinDate);
            //    result.VisibleYear = calendar.GetYear(MinDate);
            //}

            VisibleData = result;
        }

        /// <summary>
        /// Sets correct date that depends on MaxDate and MinDate.
        /// </summary>
        private void SetCorrectDate()
        {
            if (MinMaxHidden)
            {
                if (Date > MaxDate)
                {
                    Date = MaxDate;
                }

                if (Date < MinDate)
                {
                    Date = MinDate;
                }
            }
            else
            {
                if (Date > mxDate)
                {
                    Date = mxDate;
                }

                if (Date < miDate)
                {
                    Date = miDate;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        public void OnDatesCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (((DatesCollection)e.NewValue).Count > 0)
            {
                for (int i = 0; i < ((DatesCollection)e.NewValue).Count; i++)
                {
                    DateTime date1 = ((DatesCollection)e.NewValue)[i];
                    Date date = new Date(date1, Calendar);
                    SelectedDatesList.Add(date);
                }
            }
        }

        private List<DateTime> GetDateRange(DateTime StartingDate, DateTime EndingDate)
        {
            if (StartingDate > EndingDate)
            {
                return null;
            }
            List<DateTime> rv = new List<DateTime>();
            DateTime tmpDate = StartingDate;
            do
            {
                rv.Add(tmpDate);
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= EndingDate);
            return rv;
        }

        /// <summary>
        /// Processes the selected dates changing.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance
        /// that contains the event data.</param>
        private void ProcessSelectedDatesCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            VisibleDate data = this.VisibleData;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                SelectedDatesList.Clear();
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 1)
                {
                    foreach (DateTime datetime in e.NewItems)
                    {
                        Date date = new Date(datetime, Calendar);
                        if (!SelectedDatesList.Contains(date))
                            SelectedDatesList.Add(date);
                    }

                    SelectedDatesList.Sort();
                }
                else
                {
                    DateTime addedDate = (DateTime)e.NewItems[0];
                    Date item = new Date(addedDate, Calendar);

                    if (!SelectedDatesList.Contains(item))
                    {
                        int searchResult = SelectedDatesList.BinarySearch(item);

                        SelectedDatesList.Insert(~searchResult, item);
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                DateTime removedDate = (DateTime)e.OldItems[0];
                Date item = new Date(removedDate, Calendar);
                SelectedDatesList.Remove(item);
            }

            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                throw new NotImplementedException();
            }

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                throw new NotImplementedException();
            }

            CurrentDayGrid.SetIsSelected();
            FollowingDayGrid.SetIsSelected();
        }

        /// <summary>
        /// Finds current object of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> type
        /// that belongs to the specified mode.
        /// </summary>
        /// <param name="mode">The specified mode.</param>
        /// <returns>
        /// Grid that is visible in the current mode.
        /// </returns>
        private CalendarEditGrid FindCurrentGrid(CalendarVisualMode mode)
        {
            if (mode == CalendarVisualMode.WeekNumbers)
            {
                CurrentWeekNumbersGrid.Visibility = Visibility.Visible;
                //FollowingWeekNumbersGrid.Visibility = Visibility.Hidden;

                // Clearing the previous Selections
                int numberofweeks = WeekNumberGridPanel.NumberOfWeeks;
                for (int i = 1; i <= FollowingWeekNumbersGrid.CellsCollection.Count; i++)
                {
                    Cell cell = FollowingWeekNumbersGrid.CellsCollection[i - 1] as Cell;

                    if (i <= numberofweeks)
                    {
                        cell.BorderBrush = this.WeekNumberSelectionBorderBrush;
                    }
                    else
                    {
                        cell.BorderBrush = null;
                        cell.IsEnabled = false;
                    }

                    cell.Background = null;
                }

                for (int i = 1; i <= numberofweeks; i++)
                {
                    Cell cell = FollowingWeekNumbersGrid.CellsCollection[i - 1] as Cell;

                    if (i == FollowingWeekNumbersGrid.FocusedCellIndex)
                    {
                        cell.IsSelected = true;

                        // cell.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FFFF"));
                        cell.Background = WeekNumberSelectionBorderBrush;
                    }
                    else
                    {
                        cell.IsSelected = false;
                    }
                }

                if (CurrentWeekNumbersGrid.Visibility == Visibility.Visible)
                {
                    return CurrentWeekNumbersGrid;
                }
                else
                {
                    return FollowingWeekNumbersGrid;
                }
            }

            if (mode == CalendarVisualMode.Days)
            {
                if (CurrentDayGrid.Visibility == Visibility.Visible)
                {
                    return CurrentDayGrid;
                }
                else
                {
                    return FollowingDayGrid;
                }
            }

            if (mode == CalendarVisualMode.Months)
            {
                if (CurrentMonthGrid.Visibility == Visibility.Visible)
                {
                    return CurrentMonthGrid;
                }
                else
                {
                    return FollowingMonthGrid;
                }
            }

            if (mode == CalendarVisualMode.Years)
            {
                if (CurrentYearGrid.Visibility == Visibility.Visible)
                {
                    return CurrentYearGrid;
                }
                else
                {
                    return FollowingYearGrid;
                }
            }

            if (mode == CalendarVisualMode.YearsRange)
            {
                if (CurrentYearRangeGrid.Visibility == Visibility.Visible)
                {
                    return CurrentYearRangeGrid;
                }
                else
                {
                    return FollowingYearRangeGrid;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the <see cref="SelectedDatesList"/> according to the current
        /// Culture.
        /// </summary>
        private void UpdateSelectedDatesList()
        {
            if (SelectedDatesList != null)
            {
                SelectedDatesList.Clear();

                foreach (DateTime date in SelectedDates)
                {
                    if (MinMaxHidden)
                    {
                        if (date < MaxDate && date > MinDate)
                        {
                            SelectedDatesList.Add(new Date(date, Calendar));
                        }
                    }
                    else
                    {
                        if (date < miDate && date > miDate)
                        {
                            SelectedDatesList.Add(new Date(date, Calendar));
                        }
                    }
                    //// TODO: else: clear not supported dates from collection
                }

                SelectedDatesList.Sort();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool Invalidateflag = true;

        /// <summary>
        /// Implements keyboard navigation and selection
        /// logic.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.DayCell"/> click event is raised
        /// on.</param>
        /// <param name="mode">Defines whether month scrolling is
        /// enabled.</param>
        /// <param name="modifiers">The <see cref="System.Windows.Input.ModifierKeys"/> used for
        /// implementing keyboard navigation and selection logic.</param>
        private void OnDayCellClick(DayCell sender, ChangeMonthMode mode, ModifierKeys modifiers)
        {
            Date tempDate = sender.Date;
            Invalidateflag = true;
            for (int i = 0; i < InvalidDates.Count; i++)
            {
                int s = tempDate.CompareTo(InvalidDates[i]);
                if (s == 0)
                {
                    Invalidateflag = false;
                    break;
                }
            }
            DateTime newDate = tempDate.ToDateTime(Calendar);

            // Change to a valid month when user tries to select some date that does not belong to the currently selected month
            if (!sender.IsCurrentMonth && !(mode == ChangeMonthMode.Enabled))
            {
                int idayCellMonth = tempDate.Month;
                ScrollMonth(idayCellMonth);
            }

            if (AllowSelection && Invalidateflag)
            {
                if (AllowMultiplySelection && modifiers != ModifierKeys.None)
                {
                    MultiplySelect(newDate, modifiers);
                }
                else
                {
                    // Select
                    if (SelectedDates.Count == 0)
                    {
                        SelectedDates.Add(newDate);
                        m_dateSetManual = true;
                        Date = newDate;
                    }
                    else
                    {
                        SelectedDates.Clear();
                        SelectedDates.Add(newDate);

                        SelectedDatesList.Clear();
                        NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SelectedDates);

                        ProcessSelectedDatesCollectionChange(args);
                        m_dateSetManual = true;

                        if (newDate == Date && DateChanged != null)
                        {
                            DateChanged(this, new DependencyPropertyChangedEventArgs(DateProperty, newDate, newDate));
                        }

                        Date = newDate;
                        UpdateCellClickedValue();
                    }
                }
            }
            else
            {
                m_dateSetManual = true;
            }
        }

        /// <summary>
        /// Updates cell clicked value if NoneDateText value selected.
        /// </summary>
        private void UpdateCellClickedValue()
        {
            if (MinMaxHidden)
            {
                if (Date == MinDate)
                {
                    IsCellClicked = true;
                }
            }
        }

        /// <summary>
        /// Executes on the <see cref="Syncfusion.Windows.Shared.YearCell"/>
        /// click.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.YearCell"/>.</param>
        private void OnYearCellClick(YearCell sender)
        {
            VisualMode = CalendarVisualMode.Months;
            VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Years, CalendarVisualMode.Months);
            VisibleDate date = new VisibleDate(sender.Year, VisibleData.VisibleMonth, VisibleData.VisibleDay);
            ChangeVisualModePreview(date);
        }

        /// <summary>
        /// Executes on the <see cref="Syncfusion.Windows.Shared.YearRangeCell"/>
        /// click.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.YearRangeCell"/>.</param>
        private void OnYearRangeCellClick(YearRangeCell sender)
        {
            VisibleDate date;

            VisualMode = CalendarVisualMode.Years;
            VisualModeInfo = new VisualModeHistory(CalendarVisualMode.YearsRange, CalendarVisualMode.Years);
            if (VisibleData.VisibleYear >= sender.Years.StartYear && VisibleData.VisibleYear <= sender.Years.EndYear)
            {
                startYear = sender.Years.StartYear;
                endYear = sender.Years.EndYear;
                primaryDate = VisibleData.VisibleYear;
                date = new VisibleDate(VisibleData.VisibleYear, VisibleData.VisibleMonth, VisibleData.VisibleDay);
            }
            else
            {
                if (startYear != sender.Years.StartYear && endYear != sender.Years.EndYear)
                {
                    date = new VisibleDate(sender.Years.StartYear, VisibleData.VisibleMonth, VisibleData.VisibleDay);
                }
                else
                {
                    date = new VisibleDate(primaryDate, VisibleData.VisibleMonth, VisibleData.VisibleDay);
                }
            }

            ChangeVisualModePreview(date);
        }

        /// <summary>
        /// Executes on the <see cref="Syncfusion.Windows.Shared.MonthCell"/>
        /// click.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.MonthCell"/>.</param>
        private void OnMonthCellClick(MonthCell sender)
        {
            if (this.DisableDateSelection == false)
            {
                VisualMode = CalendarVisualMode.Days;
                VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days);
                VisibleDate date = new VisibleDate(VisibleData.VisibleYear, sender.MonthNumber, VisibleData.VisibleDay);
                DayNamesGrid.Visibility = Visibility.Visible;
                ChangeVisualModePreview(date);
            }
            else
            {
                if (AllowSelection && Invalidateflag)
                {
                    if (sender.IsSelected == false)
                    {
                        ArrayList cellsCollection = new ArrayList();
                        cellsCollection = this.FindCurrentGrid(CalendarVisualMode.Months).CellsCollection;

                        foreach (Cell cell in cellsCollection)
                        {
                            if (cell.IsSelected)
                            {
                                if (cell != mpressedCell)
                                    cell.IsSelected = false;
                            }
                        }
                        sender.IsSelected = true;
                        sender.Focus();
                        Date newdate = new Date();
                        newdate.Month = sender.MonthNumber;
                        string curmonth = m_monthButton1.Content.ToString();
                        newdate.Year = int.Parse(curmonth);
                        newdate.Day = DateTime.DaysInMonth(newdate.Year, newdate.Month);
                        DateTime newval = newdate.ToDateTime(Calendar);
                        this.VisualMode = CalendarVisualMode.Months;
                        this.VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Months);
                        this.Date = newval;
                        DateTimeFormatInfo format = Culture.DateTimeFormat;
                        VisibleData = new VisibleDate(this.Date.Year, this.Date.Month, this.Date.Day);
                        VisibleDataToMinSupportedDate(format);
                    }
                }
            }
        }

        /// <summary>
        /// Executes on the <see cref="Syncfusion.Windows.Shared.WeekNumberCell"/>
        /// click.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.WeekNumberCell"/>.</param>
        private void OnWeekNumbersCellClick(WeekNumberCell sender)
        {
            if (this.DisableDateSelection == false)
            {
                CurrentDayGrid.Visibility = Visibility.Hidden;
                FollowingDayGrid.Visibility = Visibility.Hidden;
                VisualMode = CalendarVisualMode.WeekNumbers;
                UpdateWeekNumbersContainer();
                VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Days, CalendarVisualMode.WeekNumbers);
                VisibleDate date = new VisibleDate(VisibleData.VisibleYear, VisibleData.VisibleMonth, VisibleData.VisibleDay);
                //CurrentWeekNumbersGrid.Visibility = Visibility.Visible;
                //FollowingWeekNumbersGrid.Visibility = Visibility.Hidden;
                FollowingWeekNumbersGrid.Visibility = Visibility.Visible;
                CurrentWeekNumbersGrid.Visibility = Visibility.Visible;
                // Clearing the previous Selections
                int numberofweeks = WeekNumberGridPanel.NumberOfWeeks;
                CalendarEditGrid currentgrid = null;
                //currentgrid = CurrentWeekNumbersGrid;
                currentgrid = FollowingWeekNumbersGrid;
                ChangeVisualModePreview(date);
            }
        }

        /// <summary>
        /// Updates the <see cref="VisibleData"/> property according to the current <see cref="Date"/> value.
        /// </summary>
        private void UpdateVisibleData()
        {
            VisibleDate tmp;
            tmp.VisibleMonth = Calendar.GetMonth(Date);
            tmp.VisibleYear = Calendar.GetYear(Date);
            tmp.VisibleDay = Calendar.GetDayOfMonth(Date);
            VisibleData = tmp;
        }

        /// <summary>
        /// Defines whether the animation is required.
        /// </summary>
        /// <returns>
        /// True, if animation is required; otherwise, false.
        /// </returns>
        private bool IsAnimationRequired()
        {
            if (Visibility != Visibility.Visible)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Occurs when the today button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance that contains the event data.</param>
        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDates.Clear();
            DateTime todayDate = DateTime.Now.Date;
            Date = todayDate;
            if (AllowSelection)
            {
                SelectedDates.Add(todayDate);
            }

            IsTodayButtonClicked = true;
            UpdateVisibleData();
        }

        #endregion private

        #region overrides

        /// <summary>
        /// Invoked when an unhandled <see cref="OnMouseLeftButtonDown"/> routed event is
        /// raised on this element. Implement this method to add class
        /// handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that
        /// contains the event data. The event data reports that the
        /// left mouse button was pressed.</param>
        /// <remarks>
        /// The MouseLeftButtonDown event appears to travel a bubbling
        /// route but actually travels in an indirect way.
        /// Mouse.MouseDown is the underlying event that is bubble
        /// routed, and each ContentElement along the event route uses
        /// identical handling to raise the direct routed event
        /// MouseLeftButtonDown. Although you can mark the
        /// MouseLeftButtonDown event as handled for purposes of this
        /// element, the handled state does not perpetuate to other
        /// elements along the event route. However, you might want to
        /// mark the event as handled in order to prevent general
        /// instance handlers (those that did not specify
        /// handledEventsToo) from being invoked.
        /// The default implementation for general mouse event handling
        /// in ContentElement listens for Mouse.MouseDown and converts it
        /// to an appropriate local event. If you want to override this
        /// logic, you must create a derived class. In the static
        /// constructor of your derived class, register an alternative
        /// class handler for Mouse.MouseDown. You cannot change the
        /// mouse handling behavior of ContentElement by overriding
        /// OnMouseLeftButtonDown.
        /// Alternatively, you can override this method in order to
        /// change event handling for a specific mouse state. Whether you
        /// choose to call the base implementation depends on your
        /// scenario. Failing to call base disables default input
        /// handling for that mouse event on ancestor classes that also
        /// expect to invoke OnMouseLeftButtonDown. For example, you can
        /// derive from Button and override OnMouseLeftButtonDown in your
        /// derived class without calling the base implementation;
        /// however, this override disables the Click event.
        /// </remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!IsStoryboardActive(mvisualModeStoryboard) && !IsStoryboardActive(mmoveStoryboard) && !IsStoryboardActive(mmonthStoryboard))
            {
                // TODO sets focus to the grid depends on current CalendarVisualMode
                if (VisualMode == CalendarVisualMode.Months)
                {
                    this.HideWeekNumbersForYearContainer();
                }
                if (this.DisableDateSelection == false)
                {
                    if (VisualMode == CalendarVisualMode.Days)
                    {
                        this.HideWeekNumbersForYearContainer();
                        DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
                        current.Focus();
                    }

                    if (e.Source is DayCell)
                    {
                        FireDayCellMouseLeftButtonDown(e);
                    }

                    if (e.Source is DayNameCell)
                    {
                        FireDayNameCellMouseLeftButtonDown(e);
                    }
                }

                if (e.Source is MonthCell)
                {
                    FireMonthCellMouseLeftButtonDown(e);
                }

                if (e.Source is YearCell)
                {
                    FireYearCellMouseLeftButtonDown(e);
                }

                if (e.Source is YearRangeCell)
                {
                    FireYearRangeCellMouseLeftButtonDown(e);
                }

                if (this.DisableDateSelection == false)
                {
                    if (e.Source is WeekNumberCell)
                    {
                        this.FireWeekNumberCellMouseLeftButtonDown(e);
                    }

                    if (e.Source is WeekNumberCellPanel)
                    {
                        this.FireWeekNumberCellPanelMouseLeftButtonDown(e);
                    }
                }
            }

            e.Handled = false;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!IsStoryboardActive(mvisualModeStoryboard))
            {
                if (this.DisableDateSelection == false)
                {
                    if (e.Source is DayCell)
                    {
                        FireDayCellMouseLeftButtonUp(e);
                    }
                }

                if (e.Source is MonthCell)
                {
                    FireMonthCellMouseLeftButtonUp(e);
                }

                if (e.Source is YearCell)
                {
                    FireYearCellMouseLeftButtonUp(e);
                }

                if (e.Source is YearRangeCell)
                {
                    FireYearRangeCellMouseLeftButtonUp(e);
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (CalendarStyle == CalendarStyle.Vista)
                {
                    if (!IsStoryboardActive(mvisualModeStoryboard))
                    {
                        if (e.Delta > 0)
                        {
                            ChangeVisualMode(ChangeVisualModeDirection.Up);
                        }
                        else
                        {
                            Cell selectedCell = null;
                            ArrayList cells = FindCurrentGrid(VisualMode).CellsCollection;

                            foreach (Cell cell in cells)
                            {
                                if (cell.IsMouseOver)
                                {
                                    selectedCell = cell;
                                }
                            }

                            if (selectedCell is YearRangeCell)
                            {
                                OnYearRangeCellClick(selectedCell as YearRangeCell);
                            }

                            if (selectedCell is YearCell)
                            {
                                OnYearCellClick(selectedCell as YearCell);
                            }

                            if (selectedCell is MonthCell)
                            {
                                OnMonthCellClick(selectedCell as MonthCell);
                            }

                            if (selectedCell is WeekNumberCellPanel)
                            {
                                this.OnWeekNumberCellPanelClick(selectedCell as WeekNumberCellPanel);
                            }
                        }
                    }
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    NextCommand.Execute(null, this);
                }
                else
                {
                    PrevCommand.Execute(null, this);
                }
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// <see cref="System.Windows.FrameworkElement.ApplyTemplate"/> method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ListBox listbox = new ListBox();

            ApplyYearEditingTemplate();
            ApplyEditMonthTemplate();
            ApplyWeekNumbersTemplate();
            ApplyWeekNumbersForYearTemplate();
            Rectangle rect1 = this.GetTemplateChild("rect") as Rectangle;
            if (rect1 != null)
            {
                if (VisualMode != CalendarVisualMode.Days)
                {
                    rect1.Visibility = Visibility.Collapsed;
                }
                if (VisualMode == CalendarVisualMode.Days)
                {
                    rect1.Visibility = Visibility.Visible;
                }
            }
            foreach (Visual vis in VisualUtils.EnumChildrenOfType(this, typeof(NavigateButton)))
            {
                NavigateButton btn_Navigate = vis as NavigateButton;

                if (btn_Navigate != null && btn_Navigate.Name == CnextMonthButtonName)
                {
                    m_nextButton = btn_Navigate;
                }

                if (btn_Navigate != null && btn_Navigate.Name == CprevMonthButtonName)
                {
                    m_prevButton = btn_Navigate;
                }
            }

            NextButtonUpdate();
            PrevButtonUpdate();

            foreach (Visual vis in VisualUtils.EnumChildrenOfType(this, typeof(MonthButton)))
            {
                MonthButton btn_Month = vis as MonthButton;

                if (btn_Month != null && btn_Month.Name == "PART_Month1")
                {
                    m_monthButton1 = btn_Month;
                    AddMonthButtonsEvents();
                }

                if (btn_Month != null && btn_Month.Name == "PART_Month2")
                {
                    m_monthButton2 = btn_Month;
                }
            }

            foreach (Visual vis in VisualUtils.EnumChildrenOfType(this, typeof(Popup)))
            {
                Popup monthPopup = vis as Popup;

                if (monthPopup != null && monthPopup.Name == "PART_MonthPopup")
                {
                    monthPopup.PlacementTarget = m_monthButton1;
                    if (MinMaxHidden)
                    {
                        m_popup = new MonthPopup(monthPopup, new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1), Culture.DateTimeFormat, new Date(MinDate, Calendar), new Date(MaxDate, Calendar));
                    }
                    else
                    {
                        m_popup = new MonthPopup(monthPopup, new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1), Culture.DateTimeFormat, new Date(miDate, Calendar), new Date(mxDate, Calendar));
                    }
                    m_popup.HidePopup += new EventHandler<HidePopupEventArgs>(Popup_OnHidePopup);
                }
                else
                {
                    throw new ArgumentException("MonthPopup is not found");
                }
            }

            if (ScrollToDateEnabled)
            {
                UpdateVisibleData();
            }

            m_monthButton1.MouseLeftButtonDown += new MouseButtonEventHandler(MonthButton_OnMouseLeftButtonDown);
            m_monthButton1.Initialize(VisibleData, Calendar, Culture, IsMonthNameAbbreviated, VisualMode);
            m_monthButton2.Visibility = Visibility.Hidden;

            m_todayButton = GetTemplateChild("PART_TodayButton") as Button;
            if (m_todayButton != null)
            {
                m_todayButton.Click += new RoutedEventHandler(TodayButton_Click);
            }
        }

        /// <summary>
        /// Adds to calendar PrevButton some event handlers, updates template.
        /// </summary>
        private void PrevButtonUpdate()
        {
            if (m_prevButton != null)
            {
                m_prevButton.MouseEnter += new MouseEventHandler(NavigateButton_OnMouseEnter);
                m_prevButton.UpdateCellTemplate(PreviousScrollButtonTemplate);
            }
        }

        /// <summary>
        /// Adds to calendar NextButton some event handlers, updates template.
        /// </summary>
        private void NextButtonUpdate()
        {
            if (m_nextButton != null)
            {
                m_nextButton.MouseEnter += new MouseEventHandler(NavigateButton_OnMouseEnter);
                m_nextButton.UpdateCellTemplate(NextScrollButtonTemplate);
            }
        }

        /// <summary>
        /// Adds event handlers for year editing to MonthButton1.
        /// </summary>
        private void AddMonthButtonsEvents()
        {
            if (IsYearSelectionInStandardStyle() && m_monthButton1 != null)
            {
                m_monthButton1.MouseEnter += new MouseEventHandler(MonthButton1_MouseEnter);
                m_monthButton1.MouseLeave += new MouseEventHandler(MonthButton1_MouseLeave);
            }
        }

        /// <summary>
        /// Removes event handlers for year editing to MonthButton1.
        /// </summary>
        private void DeleteMonthButtonsEvents()
        {
            if (!IsYearSelectionInStandardStyle() && m_monthButton1 != null)
            {
                m_monthButton1.MouseEnter -= new MouseEventHandler(MonthButton1_MouseEnter);
                m_monthButton1.MouseLeave -= new MouseEventHandler(MonthButton1_MouseLeave);
            }
        }

        /// <summary>
        /// Initializes variables (from calendar template) to work with week numbers.
        /// </summary>
        private void ApplyWeekNumbersTemplate()
        {
            m_weekNumbersContainer = GetWeekNumbersContainer();
            m_mainGrid = GetMainGrid();

            if (IsShowWeekNumbers)
            {
                ShowWeekNumbersContainer();
            }
        }

        /// <summary>
        /// Initializes variables (from calendar template) to work with week numbers for year.
        /// </summary>
        private void ApplyWeekNumbersForYearTemplate()
        {
            wcurrentweekNumbersContainer = GetWeekNumbersPanelCurrent();
            wfollowweekNumbersContainer = GetWeekNumbersPanelFollow();

            if (IsShowWeekNumbersGrid)
            {
                ShowWeekNumbersForYearContainer();
            }
        }

        /// <summary>
        /// Initializes variables (from calendar template) to work with years editing.
        /// </summary>
        private void ApplyYearEditingTemplate()
        {
            m_yearUpDown = GetYearUpDown();
            m_yearUpDownPanel = GetYearUpDownPanel();
        }

        /// <summary>
        /// Initializes variables (from calendar template) to work with years editing.
        /// </summary>
        private void ApplyEditMonthTemplate()
        {
            m_editMonthName = Template.FindName(CeditMonthName, this) as TextBlock;

            if (IsYearSelectionInStandardStyle() && m_editMonthName != null)
            {
                SetEditMonthText();
            }
        }

        /// <summary>
        /// Gets if YearSelection is allowed and calendar style is standard.
        /// </summary>
        /// <returns>true if YearSelection is allowed and calendar style is standard</returns>
        private bool IsYearSelectionInStandardStyle()
        {
            return IsAllowYearSelection && CalendarStyle == CalendarStyle.Standard;
        }

        /// <summary>
        /// Initializes month name when years editing is visible.
        /// </summary>
        private void SetEditMonthText()
        {
            int imonth = VisibleData.VisibleMonth;
            int iyear = VisibleData.VisibleYear;
            DateTimeFormatInfo format = Culture.DateTimeFormat;

            if (VisualMode == CalendarVisualMode.Days)
            {
                if (IsMonthNameAbbreviated)
                {
                    m_editMonthName.Text = format.AbbreviatedMonthNames[imonth - 1];
                }
                else
                {
                    m_editMonthName.Text = format.MonthNames[imonth - 1];
                }
            }
            else
            {
                m_editMonthName.Text = string.Empty;
            }
        }

        /// <summary>
        /// Gets panel that contains years editing UpDown from Calendar template
        /// </summary>
        /// <returns>Grid from calendar</returns>
        private StackPanel GetYearUpDownPanel()
        {
            StackPanel yearUpDownPanel = Template.FindName(CyearUpDownPanel, this) as StackPanel;

            if (yearUpDownPanel != null)
            {
                yearUpDownPanel.MouseLeave += new MouseEventHandler(UpDownPanel_MouseLeave);
            }

            return yearUpDownPanel;
        }

        /// <summary>
        /// Gets UpDown (without decimal digits) for years editing.
        /// </summary>
        /// <returns>UpDown from the calendar template.</returns>
        private UpDown GetYearUpDown()
        {
            UpDown yearUpDown = Template.FindName(CyearUpDown, this) as UpDown;

            if (yearUpDown != null && yearUpDown.NumberFormatInfo != null)
            {
                yearUpDown.NumberFormatInfo.NumberDecimalDigits = 0;
                yearUpDown.NumberFormatInfo.NumberGroupSeparator = string.Empty;
                yearUpDown.Value = VisibleData.VisibleYear;
            }

            return yearUpDown;
        }

        /// <summary>
        /// Invoked when mouse leaves Month button. Then timer stops and year editing UpDown is collapsed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Instance that contains the event data.</param>
        private void MonthButton1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsUpDownPanelVisible())
            {
                m_timer.Stop();
            }
        }

        /// <summary>
        /// Invoked when mouse leaves the UpDown container. Then, if the UpDown is visible,
        /// UpDown becomes collapsed and Month button becomes visible.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Instance that contains the event data.</param>
        private void UpDownPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsUpDownPanelVisible())
            {
                m_yearUpDownPanel.Visibility = Visibility.Collapsed;
                m_monthButton1.Visibility = Visibility.Visible;

                VisibleData = new VisibleDate((int)m_yearUpDown.Value, VisibleData.VisibleMonth, VisibleData.VisibleDay);
            }
        }

        /// <summary>
        /// Invoked when mouse enters Month button. Then timer starts and after 2 seconds year editing UpDown is shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Instance that contains the event data.</param>
        private void MonthButton1_MouseEnter(object sender, MouseEventArgs e)
        {
            m_timer = new DispatcherTimer();
            m_timer.Interval = TimeSpan.FromSeconds(1);
            m_timer.Tick += new EventHandler(Timer_Tick);
            m_timer.Start();
        }

        /// <summary>
        /// Invoked when the timer interval has elapsed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The System.EventArgs instance that contains the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            m_yearUpDownPanel.Visibility = Visibility.Visible;
            m_monthButton1.Visibility = Visibility.Collapsed;
            SetEditMonthText();

            m_yearUpDown.Value = VisibleData.VisibleYear;
        }

        /// <summary>
        /// Gets if year upDown panel is visible.
        /// </summary>
        /// <returns>True if year upDown panel is visible</returns>
        private bool IsUpDownPanelVisible()
        {
            return m_yearUpDownPanel.Visibility == Visibility.Visible;
        }

        /// <summary>
        /// Gets week numbers container of the calendar template.
        /// </summary>
        /// <returns>content presenter that is container for week numbers</returns>
        private ContentPresenter GetWeekNumbersContainer()
        {
            return Template.FindName(CweekNumbers, this) as ContentPresenter;
        }

        /// <summary>
        /// Gets week numbers for current year container of the calendar template.
        /// </summary>
        /// <returns>content presenter that is container for week numbers for current year</returns>
        private ContentPresenter GetWeekNumbersPanelCurrent()
        {
            return Template.FindName(CWeekNumbersGridCurrent, this) as ContentPresenter;
        }

        /// <summary>
        /// Gets week numbers for following year container of the calendar template.
        /// </summary>
        /// <returns>content presenter that is container for week numbers for following year</returns>
        private ContentPresenter GetWeekNumbersPanelFollow()
        {
            return Template.FindName(CWeekNumbersGridFollow, this) as ContentPresenter;
        }

        /// <summary>
        /// Gets main grid of the calendar template.
        /// </summary>
        /// <returns>main grid of the calendar</returns>
        private Grid GetMainGrid()
        {
            return Template.FindName(CmainGrid, this) as Grid;
        }

        ///// <summary>
        ///// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        ///// </summary>
        ///// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        //// protected override void OnInitialized(EventArgs e)
        //// {
        ////    base.OnInitialized(e);

        ////    if (SkinStorage.GetVisualStyle(this) == "Default")
        ////    {
        ////        Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
        ////        if (list1 != null)
        ////        {
        ////            Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
        ////            if (list2 != null)
        ////            {
        ////                if (list2.ContainsKey("CalendarFill"))
        ////                    Background = list2["CalendarFill"] as Brush;

        ////                if (list2.ContainsKey("DayCellForeground"))
        ////                    Foreground = list2["DayCellForeground"] as Brush;

        ////                HeaderBackground = Brushes.Transparent;

        ////                if (list2.ContainsKey("HeaderColor"))
        ////                    HeaderForeground = list2["HeaderColor"] as Brush;

        ////                if (list2.ContainsKey("SelectionBorderBrush"))
        ////                    SelectionBorderBrush = list2["SelectionBorderBrush"] as Brush;
        ////            }
        ////        }
        ////    }
        //// }

        /////// <summary>
        /////// Called when some dependencyProperty is changed.
        /////// </summary>
        /////// <param name="e">Event args.</param>
        //// protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //// {
        ////    base.OnPropertyChanged(e);

        ////    if (e.Property == SkinStorage.VisualStyleProperty)
        ////    {
        ////        if ((string)e.NewValue == "Default")
        ////        {
        ////            Background = m_defaultBackground;
        ////            Foreground = m_defaultForeground;
        ////            HeaderBackground = m_defaultHeaderBackground;
        ////            HeaderForeground = m_defaultHeaderForeground;
        ////            SelectionBorderBrush = m_defaultSelectionBorderBrush;
        ////        }

        ////        UpdateSelectionBorderBrush();
        ////    }
        ////    else if(e.Property == BackgroundProperty)
        ////    {
        ////        m_defaultBackground = Background;
        ////    }
        ////    else if (e.Property == ForegroundProperty)
        ////    {
        ////        m_defaultForeground = Foreground;
        ////    }
        ////    else if(e.Property == HeaderBackgroundProperty)
        ////    {
        ////        m_defaultHeaderBackground = HeaderBackground;
        ////    }
        ////    else if(e.Property == HeaderForegroundProperty)
        ////    {
        ////        m_defaultHeaderForeground = HeaderForeground;
        ////    }
        ////    else if(e.Property == SelectionBorderBrushProperty)
        ////    {
        ////        m_defaultSelectionBorderBrush = SelectionBorderBrush;
        ////        UpdateSelectionBorderBrush();
        ////    }
        //// }

        /// <summary>
        /// Updates the selection border brush.
        /// </summary>
        private void UpdateSelectionBorderBrush()
        {
            CurrentDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            FollowingDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            ////string style = SkinStorage.GetVisualStyle(this);

            ////if (style == "Default")
            ////{
            ////    CurrentDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            ////    FollowingDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            ////}
            ////else
            ////{
            ////    Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////    if (list1 != null)
            ////    {
            ////        Shared.DictionaryList list2 = list1[style] as Shared.DictionaryList;
            ////        if (list2 != null)
            ////        {
            ////            if (list2.ContainsKey("SelectionBorderBrush"))
            ////            {
            ////                // DefaultSelectionBorderBrush = list2["SelectionBorderBrush"] as Brush;
            ////                SelectionBorderBrush = list2["SelectionBorderBrush"] as Brush;
            ////            }
            ////        }
            ////    }

            ////    // CurrentDayGrid.SelectionBorder.BorderBrush = DefaultSelectionBorderBrush;
            ////    // FollowingDayGrid.SelectionBorder.BorderBrush = DefaultSelectionBorderBrush;
            ////    CurrentDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            ////    FollowingDayGrid.SelectionBorder.BorderBrush = SelectionBorderBrush;
            ////}
        }

        #endregion overrides

        #region event raisers

        /// <summary>
        /// Raises the <see cref="YearRangeCellMouseLeftButtonUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireYearRangeCellMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (YearRangeCellMouseLeftButtonUp != null)
            {
                YearRangeCellMouseLeftButtonUp(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="YearCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireYearRangeCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (YearRangeCellMouseLeftButtonDown != null)
            {
                YearRangeCellMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="YearCellMouseLeftButtonUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireYearCellMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (YearCellMouseLeftButtonUp != null)
            {
                YearCellMouseLeftButtonUp(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="YearCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireYearCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (YearCellMouseLeftButtonDown != null)
            {
                YearCellMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="MonthCellMouseLeftButtonUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireMonthCellMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (MonthCellMouseLeftButtonUp != null)
            {
                MonthCellMouseLeftButtonUp(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="MonthCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireMonthCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (MonthCellMouseLeftButtonDown != null)
            {
                MonthCellMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DayCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireDayCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (DayCellMouseLeftButtonDown != null)
            {
                DayCellMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="WeekNumberCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireWeekNumberCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.WeekNumberCellMouseLeftButtonDown != null)
            {
                this.WeekNumberCellMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="WeekNumberCellPanelMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireWeekNumberCellPanelMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.WeekNumberCellPanelMouseLeftButtonDown != null)
            {
                this.WeekNumberCellPanelMouseLeftButtonDown(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DayCellMouseLeftButtonUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireDayCellMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (DayCellMouseLeftButtonUp != null)
            {
                DayCellMouseLeftButtonUp(e.Source, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DayNameCellMouseLeftButtonDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event data.</param>
        protected virtual void FireDayNameCellMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (DayNameCellMouseLeftButtonDown != null)
            {
                DayNameCellMouseLeftButtonDown(e.Source, e);
            }
        }

        #endregion event raisers

        #region event handlers

        /// <summary>
        /// Handles the OnMouseLeftButtonDown event of the MonthButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MonthButton_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalendarEdit calendarEdit = new CalendarEdit();

            if (!IsStoryboardActive(mvisualModeStoryboard))
            {
                if (CalendarStyle == CalendarStyle.Standard)
                {
                    if (VisualMode == CalendarVisualMode.Days)
                    {
                        m_popup.Show();
                    }
                    else
                    {
                        calendarEdit.CalendarStyle = CalendarStyle.Vista;
                        ChangeVisualMode(ChangeVisualModeDirection.Up);
                    }
                }

                if (CalendarStyle == CalendarStyle.Vista)
                {
                    ChangeVisualMode(ChangeVisualModeDirection.Up);
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked whenever visual mode storyboard animation is
        /// completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> containing the event
        /// data.</param>
        private void VisualModeStoryboard_OnCompleted(object sender, EventArgs e)
        {
            CalendarVisualMode oldMode = VisualModeInfo.OldMode;
            CalendarVisualMode newMode = VisualModeInfo.NewMode;
            CalendarEditGrid currentGrid = null;
            ArrayList cellsCollection = new ArrayList();

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.Months)
            {
                DayNamesGrid.Visibility = Visibility.Hidden;
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Days)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.Years)
            {
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.Months)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Years && newMode == CalendarVisualMode.YearsRange)
            {
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.YearsRange && newMode == CalendarVisualMode.Years)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.WeekNumbers)
            {
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Days)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Days)
            {
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Days && newMode == CalendarVisualMode.WeekNumbers)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.WeekNumbers && newMode == CalendarVisualMode.Months)
            {
                cellsCollection = FindCurrentGrid(newMode).CellsCollection;
            }

            if (oldMode == CalendarVisualMode.Months && newMode == CalendarVisualMode.WeekNumbers)
            {
                cellsCollection = FindCurrentGrid(oldMode).CellsCollection;
            }

            currentGrid = FindCurrentGrid(oldMode);
            currentGrid.Visibility = Visibility.Hidden;
            FindCurrentGrid(newMode).Focus();

            foreach (Cell cell in cellsCollection)
            {
                if (cell.IsSelected)
                {
                    cell.Opacity = 1;
                }
            }

            NavigateButtonVerify();

            if (m_visualModeQueue.Count != 0)
            {
                ScrollToDate();
            }
        }

        /// <summary>
        /// Invoked whenever move storyboard animation is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> containing the event
        /// data.</param>
        private void MoveStoryboard_Completed(object sender, EventArgs e)
        {
            CalendarEditGrid currentGrid = null;

            if (VisualMode == CalendarVisualMode.WeekNumbers)
            {
                currentGrid = CurrentWeekNumbersGrid;
            }

            if (VisualMode == CalendarVisualMode.Months)
            {
                currentGrid = CurrentMonthGrid;
            }

            if (VisualMode == CalendarVisualMode.Years)
            {
                currentGrid = CurrentYearGrid;
            }

            if (VisualMode == CalendarVisualMode.YearsRange)
            {
                currentGrid = CurrentYearRangeGrid;
            }

            m_monthButton2.Visibility = Visibility.Hidden;

            if (currentGrid != null)
            {
                currentGrid.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Handles the OnHidePopup event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.HidePopupEventArgs"/> instance containing the event data.</param>
        private void Popup_OnHidePopup(object sender, HidePopupEventArgs e)
        {
            Date selectedDate = e.SelectedDate;
            Date date = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime endDate = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int delta = CalculateMonthDelta(startDate, endDate);

            if (delta != 0)
            {
                if (startDate < endDate)
                {
                    BeginMoving(MoveDirection.Next, delta);
                }
                else
                {
                    BeginMoving(MoveDirection.Prev, delta);
                }
            }
        }

        /// <summary>
        /// Handles the OnMouseEnter event of the NavigateButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void NavigateButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
            {
                if (!IsStoryboardActive(mmonthStoryboard))
                {
                    DayGrid current;
                    NavigateButton btnNavigate = (NavigateButton)sender;
                    int inextMonth = 0;
                    int icurrentMonth = VisibleData.VisibleMonth;

                    if (btnNavigate.Name == CnextMonthButtonName)
                    {
                        inextMonth = DateUtils.AddMonth(icurrentMonth, 1);
                    }

                    if (btnNavigate.Name == CprevMonthButtonName)
                    {
                        inextMonth = DateUtils.AddMonth(icurrentMonth, -1);
                    }

                    current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);

                    foreach (DayCell dc in current.CellsCollection)
                    {
                        if (dc.Date.Month == inextMonth)
                        {
                            dc.FireHighlightEvent();
                        }
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the OnKeyDown event of the DayGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void DayGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (!IsStoryboardActive(mmonthStoryboard) && Keyboard.Modifiers == ModifierKeys.None)
                {
                    bool moved;
                    bool changingEnabled = false;
                    int inewFocusedCellIndex;
                    int icelsCount = CurrentDayGrid.CellsCollection.Count;
                    int icolCount = CurrentDayGrid.ColumnsCount;
                    DayCell dc;
                    DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
                    inewFocusedCellIndex = ValidateFocusIndex();

                    switch (e.Key)
                    {
                        case Key.Down:
                            inewFocusedCellIndex += icolCount;

                            if (inewFocusedCellIndex >= icelsCount)
                            {
                                inewFocusedCellIndex -= icolCount;
                            }

                            e.Handled = true;
                            break;

                        case Key.Left:
                            inewFocusedCellIndex = SetFocusedCellIndex(inewFocusedCellIndex, -1, out moved);
                            if (moved)
                            {
                                current = FollowingDayGrid;
                            }

                            e.Handled = true;
                            break;

                        case Key.Right:
                            inewFocusedCellIndex = SetFocusedCellIndex(inewFocusedCellIndex, 1, out moved);
                            if (moved)
                            {
                                current = FollowingDayGrid;
                            }

                            e.Handled = true;
                            break;

                        case Key.Up:
                            inewFocusedCellIndex -= icolCount;
                            if (inewFocusedCellIndex < 0)
                            {
                                inewFocusedCellIndex += icolCount;
                            }

                            e.Handled = true;
                            break;

                        case Key.Enter:
                            changingEnabled = true;
                            inewFocusedCellIndex = ValidateFocusIndex();
                            e.Handled = true;
                            break;

                        default:
                            break;
                    }

                    if (!changingEnabled)
                    {
                        current.FocusedCellIndex = inewFocusedCellIndex;
                    }

                    dc = (DayCell)current.CellsCollection[inewFocusedCellIndex];

                    if (dc.Visibility == Visibility.Visible)
                    {
                        if (changingEnabled)
                        {
                            OnDayCellClick(dc, ChangeMonthMode.Enabled, Keyboard.Modifiers);
                        }
                        else
                        {
                            OnDayCellClick(dc, ChangeMonthMode.Disabled, Keyboard.Modifiers);
                        }
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Alt && (Keyboard.GetKeyStates(Key.Right) != KeyStates.Down || Keyboard.GetKeyStates(Key.Left) != KeyStates.Down))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the OnKeyDown event of the VisualModeGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void VisualModeGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (!IsStoryboardActive(mmoveStoryboard) && !IsStoryboardActive(mvisualModeStoryboard))
                {
                    CalendarEditGrid currentGrid = FindCurrentGrid(VisualMode);
                    int inewFocusedCellIndex = currentGrid.FocusedCellIndex;
                    int icelsCount = currentGrid.CellsCollection.Count;
                    int icolCount = currentGrid.ColumnsCount;

                    if (inewFocusedCellIndex < 0)
                    {
                        inewFocusedCellIndex = currentGrid.CellsCollection.Count - 1;
                    }

                    if (inewFocusedCellIndex > currentGrid.CellsCollection.Count - 1)
                    {
                        inewFocusedCellIndex = 0;
                    }

                    switch (e.Key)
                    {
                        case Key.Down:
                            inewFocusedCellIndex += icolCount;
                            if (inewFocusedCellIndex >= icelsCount)
                            {
                                inewFocusedCellIndex -= icolCount;
                            }

                            currentGrid.FocusedCellIndex = inewFocusedCellIndex;
                            e.Handled = true;
                            break;

                        case Key.Left:
                            if (--inewFocusedCellIndex < 0)
                            {
                                PrevCommand.Execute(null, this);
                                inewFocusedCellIndex = icelsCount - 1;
                            }

                            currentGrid.FocusedCellIndex = inewFocusedCellIndex;
                            e.Handled = true;
                            break;

                        case Key.Right:
                            if (++inewFocusedCellIndex >= icelsCount)
                            {
                                NextCommand.Execute(null, this);
                                inewFocusedCellIndex = 0;
                            }

                            currentGrid.FocusedCellIndex = inewFocusedCellIndex;
                            e.Handled = true;
                            break;

                        case Key.Up:
                            inewFocusedCellIndex -= icolCount;

                            if (inewFocusedCellIndex < 0)
                            {
                                inewFocusedCellIndex += icolCount;
                            }

                            currentGrid.FocusedCellIndex = inewFocusedCellIndex;
                            e.Handled = true;
                            break;

                        case Key.Enter:
                            Cell cell = currentGrid.CellsCollection[currentGrid.FocusedCellIndex] as Cell;

                            if (VisualMode == CalendarVisualMode.Months)
                            {
                                OnMonthCellClick((MonthCell)cell);
                            }

                            if (VisualMode == CalendarVisualMode.Years)
                            {
                                OnYearCellClick((YearCell)cell);
                            }

                            if (VisualMode == CalendarVisualMode.YearsRange)
                            {
                                OnYearRangeCellClick((YearRangeCell)cell);
                            }

                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < currentGrid.CellsCollection.Count; i++)
                    {
                        Cell cell = currentGrid.CellsCollection[i] as Cell;

                        if (i == currentGrid.FocusedCellIndex)
                        {
                            cell.IsSelected = true;
                        }
                        else
                        {
                            cell.IsSelected = false;
                        }
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Alt && (Keyboard.GetKeyStates(Key.Right) != KeyStates.Down || Keyboard.GetKeyStates(Key.Left) != KeyStates.Down))
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the OnMouseLeftButtonUp event of the YearRangeCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void YearRangeCell_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                YearRangeCell releasedCell = (YearRangeCell)sender;

                if (mpressedCell == releasedCell)
                {
                    OnYearRangeCellClick(releasedCell);
                }

                mpressedCell = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the OnMouseLeftButtonDown event of the YearRangeCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void YearRangeCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                mpressedCell = (YearRangeCell)sender;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the OnMouseLeftButtonUp event of the YearCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void YearCell_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                YearCell releasedCell = (YearCell)sender;

                if (mpressedCell == releasedCell)
                {
                    OnYearCellClick(releasedCell);
                }

                mpressedCell = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the OnMouseLeftButtonDown event of the YearCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void YearCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                mpressedCell = (YearCell)sender;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="MonthCell_OnMouseLeftButtonUp"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.MonthCell"/> element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void MonthCell_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                MonthCell releasedCell = (MonthCell)sender;

                if (mpressedCell == releasedCell)
                {
                    OnMonthCellClick(releasedCell);
                }

                mpressedCell = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="MonthCell_OnMouseLeftButtonDown"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.MonthCell" />
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void MonthCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                if (this.DisableDateSelection == false)
                {
                    mpressedCell = (MonthCell)sender;
                    e.Handled = true;
                }
                else
                {
                    mpressedCell = (MonthCell)sender;
                    OnMonthCellClick((MonthCell)sender);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="WeekNumberCell_OnMouseLeftButtonDown"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.WeekNumberCell" />
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void WeekNumberCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                if (IsShowWeekNumbersGrid == true)
                {
                    mpressedCell = (WeekNumberCell)sender;

                    try
                    {
                        WeekNumberCell releasedCell = (WeekNumberCell)sender;
                        TextBlock t = (TextBlock)e.OriginalSource;
                        clickedweeknumber = t.Text;
                        this.wcellClicked = true;
                        CultureInfo cultureSA = new CultureInfo("ar-SA");
                        if (Culture.Equals((object)cultureSA))
                        {
                        }
                        else
                        {
                            FollowingWeekNumbersGrid.Initialize(VisibleData, Culture, Calendar);
                            if (mpressedCell == releasedCell)
                            {
                                this.OnWeekNumbersCellClick(releasedCell);
                                this.ShowWeekNumbersForYearContainer();
                            }

                            mpressedCell = null;
                            e.Handled = true;
                        }
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="WeekNumberCellPanel_OnMouseLeftButtonDown"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.WeekNumberCellPanel" />
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void WeekNumberCellPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                mpressedCell = (WeekNumberCellPanel)sender;
                WeekNumberCellPanel releasedCell = (WeekNumberCellPanel)sender;
                clickedweeknumber = releasedCell.WeekNumber;
                CultureInfo cultureSA = new CultureInfo("ar-SA");
                if (Culture.Equals((object)cultureSA))
                {
                }
                else
                {
                    FollowingWeekNumbersGrid.Initialize(VisibleData, Culture, Calendar);
                    if (mpressedCell == releasedCell)
                    {
                        this.OnWeekNumberCellPanelClick(releasedCell);
                    }

                    mpressedCell = null;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Executes on the <see cref="Syncfusion.Windows.Shared.WeekNumberCellPanel"/>
        /// click.
        /// </summary>
        /// <param name="sender">The <see cref="Syncfusion.Windows.Shared.WeekNumberCellPanel"/>.</param>
        private void OnWeekNumberCellPanelClick(WeekNumberCellPanel sender)
        {
            UpdateWeekNumbersContainer();
            int weekNum;
            weekNum = System.Convert.ToInt16(clickedweeknumber);
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            DateTime jan1 = new DateTime(VisibleData.VisibleYear, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset);
            int firstWeek;
            CalendarWeekRule rule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;

            if (firstMonday.DayOfWeek == DayOfWeek.Monday)
            {
                firstWeek = cal.GetWeekOfYear(jan1, rule, DayOfWeek.Monday);

                if (firstWeek <= 1)
                {
                    weekNum -= 1;
                }
            }

            DateTime result = firstMonday.AddDays(weekNum * 7);
            VisualMode = CalendarVisualMode.Days;
            VisualModeInfo = new VisualModeHistory(CalendarVisualMode.Months, CalendarVisualMode.Days);
            VisibleDate date;

            if (VisibleData.VisibleYear == result.Year)
            {
                date = new VisibleDate(VisibleData.VisibleYear, result.Month, VisibleData.VisibleDay);
            }
            else
            {
                if (System.Convert.ToInt32(clickedweeknumber) == 53)
                {
                    date = new VisibleDate(VisibleData.VisibleYear, (result.Month % 12) + 11, VisibleData.VisibleDay);
                }
                else
                {
                    date = new VisibleDate(VisibleData.VisibleYear, (result.Month % 12) + 1, VisibleData.VisibleDay);
                }
            }

            DayNamesGrid.Visibility = Visibility.Visible;
            ChangeVisualModePreview(date);
            this.HideWeekNumbersForYearContainer();
            CurrentWeekNumbersGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayCell_OnMouseLeftButtonDown"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.DayCell"/> element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void DayCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                if (Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    if (m_shiftDateChangeEnabled)
                    {
                        m_shiftDate = Date;
                    }
                }

                mpressedCell = (DayCell)sender;
                OnDayCellClick((DayCell)sender, ChangeMonthMode.Disabled, Keyboard.Modifiers);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayCell_OnMouseLeftButtonUp"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.DayCell"/> element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void DayCell_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                DayCell releasedCell = (DayCell)sender;

                if (releasedCell == mpressedCell)
                {
                    if (!releasedCell.IsCurrentMonth)
                    {
                        int idayCellMonth = releasedCell.Date.Month;
                        ScrollMonth(idayCellMonth);
                    }
                }

                mpressedCell = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="DayNameCell_OnMouseLeftButtonDown"/> event is
        /// raised on the <see cref="Syncfusion.Windows.Shared.DayNameCell"/> element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> containing the event
        /// data.</param>
        private void DayNameCell_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                if (AllowSelection && AllowMultiplySelection && ModifierKeys.Control == Keyboard.Modifiers)
                {
                    ArrayList cells = new ArrayList();
                    int index = Grid.GetColumn((UIElement)sender);
                    DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);

                    for (int i = 0; i < current.RowsCount; i++)
                    {
                        DayCell dc = (DayCell)current.CellsCollection[index];

                        if (dc.Visibility != Visibility.Hidden)
                        {
                            cells.Add(dc);
                        }

                        index += current.ColumnsCount;
                    }

                    DateTime newDate;

                    foreach (DayCell dc in cells)
                    {
                        newDate = dc.Date.ToDateTime(Calendar);

                        if (SelectionRangeMode.WholeColumn == SelectionRangeMode)
                        {
                            SelectedDates.Add(newDate);
                        }
                        else
                        {
                            if (dc.IsCurrentMonth)
                            {
                                SelectedDates.Add(newDate);
                            }
                        }
                    }
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the OnMouseLeave event of the DayNamesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DayNamesGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
            {
                DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
                Highlight(current.SelectionBorder, HighlightSate.Stop);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the OnMouseMove event of the DayNamesGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DayNamesGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Source is DayNameCell)
                {
                    int index = Grid.GetColumn((UIElement)e.Source);
                    DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);
                    Grid.SetColumn((UIElement)current.SelectionBorder, index);
                    Highlight(current.SelectionBorder, HighlightSate.Begin);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles the OnMouseMove event of the DayGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void DayGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!e.Handled)
            {
                DayCell dayCellSender = e.Source as DayCell;

                if (dayCellSender != null)
                {
                    bool senderChanged = false;
                    DayGrid current = (DayGrid)FindCurrentGrid(CalendarVisualMode.Days);

                    foreach (DayCell dc in current.CellsCollection)
                    {
                        if (dc.IsDate && dc != dayCellSender)
                        {
                            senderChanged = true;
                        }
                    }

                    if (senderChanged && e.LeftButton == MouseButtonState.Pressed)
                    {
                        // MultiplySelect( dc );
                        ModifierKeys modifiers;
                        m_shiftDateChangeEnabled = false;

                        if (Keyboard.Modifiers == ModifierKeys.None)
                        {
                            modifiers = ModifierKeys.Shift;
                        }
                        else
                        {
                            modifiers = Keyboard.Modifiers;
                        }

                        OnDayCellClick(dayCellSender, ChangeMonthMode.Disabled, modifiers);
                    }
                    else
                    {
                        m_shiftDateChangeEnabled = true;
                    }
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedDates"/> collection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> containing the event
        /// data.</param>
        private void SelectedDates_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!mbselectedDatesUpdateLocked && IsInitializeComplete)
            {
                ProcessSelectedDatesCollectionChange(e);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="DateStyles"/> collection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> containing the event
        /// data.</param>
        private void DateStyles_OnPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            StyleItem changedItem = null;
            CollectionChangedAction action = CollectionChangedAction.Reset;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    action = CollectionChangedAction.Add;
                    changedItem = (StyleItem)e.NewItems[0];
                    break;

                case NotifyCollectionChangedAction.Remove:
                    action = CollectionChangedAction.Remove;
                    changedItem = (StyleItem)e.OldItems[0];
                    break;

                case NotifyCollectionChangedAction.Reset:
                    action = CollectionChangedAction.Reset;
                    break;

                default:
                    break;
            }

            SetDateStyles(CurrentDayGrid, action, changedItem);
            SetDateStyles(FollowingDayGrid, action, changedItem);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DateDataTemplates"/> collection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> containing the event
        /// data.</param>
        private void DateDataTemplates_OnPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DataTemplateItem changedItem = null;
            CollectionChangedAction action = CollectionChangedAction.Reset;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    action = CollectionChangedAction.Add;
                    changedItem = (DataTemplateItem)e.NewItems[0];
                    break;

                case NotifyCollectionChangedAction.Remove:
                    action = CollectionChangedAction.Remove;
                    changedItem = (DataTemplateItem)e.OldItems[0];
                    break;

                case NotifyCollectionChangedAction.Reset:
                    action = CollectionChangedAction.Reset;
                    break;

                default:
                    break;
            }

            SetDateDataTemplates(CurrentDayGrid, action, changedItem);
            SetDateDataTemplates(FollowingDayGrid, action, changedItem);
        }

        /// <summary>
        /// Shows or hides week number container in Days visual mode.
        /// </summary>
        private void UpdateWeekNumbersContainer()
        {
            if (VisualModeInfo.NewMode == CalendarVisualMode.Days && IsShowWeekNumbers)
            {
                ShowWeekNumbersContainer();
            }
            else
            {
                HideWeekNumbersContainer();
            }
        }

        #endregion event handlers

        #region command handlers

        /// <summary>
        /// Next command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> containing the event
        /// data.</param>
        public void NextCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (VisualMode == CalendarVisualMode.Days)
            {
                if (IsStoryboardActive(mmonthStoryboard) || IsStoryboardActive(mvisualModeStoryboard))
                {
                    m_iscrollCounter++;
                }
                else
                {
                    BeginMoving(MoveDirection.Next, 1);
                }
            }
            else
            {
                if (IsStoryboardActive(mmoveStoryboard) || IsStoryboardActive(mvisualModeStoryboard))
                {
                    return;
                }
                else
                {
                    Move(MoveDirection.Next);
                }
            }
        }

        /// <summary>
        /// Determines whether next command can be executed in its current
        /// state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> containing the event
        /// data.</param>
        public void NextCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Previous command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> containing the event
        /// data.</param>
        public void PrevCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (VisualMode == CalendarVisualMode.Days)
            {
                if (IsStoryboardActive(mmonthStoryboard) || IsStoryboardActive(mvisualModeStoryboard))
                {
                    m_iscrollCounter--;
                }
                else
                {
                    BeginMoving(MoveDirection.Prev, -1);
                }
            }
            else
            {
                if (IsStoryboardActive(mmoveStoryboard) || IsStoryboardActive(mvisualModeStoryboard))
                {
                    return;
                }
                else
                {
                    Move(MoveDirection.Prev);
                }
            }
        }

        /// <summary>
        /// Determines whether previous command can be executed in its current
        /// state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> containing the event
        /// data.</param>
        public void PrevCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Up command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> containing the event
        /// data.</param>
        public void UpCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsStoryboardActive(mvisualModeStoryboard))
            {
                if (CalendarStyle == CalendarStyle.Vista)
                {
                    ChangeVisualMode(ChangeVisualModeDirection.Up);
                }
            }
        }

        /// <summary>
        /// Determines whether up command can be executed in its current
        /// state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> containing the event
        /// data.</param>
        public void UpCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion command handlers

        #endregion Implementation

        #region Commands

        /// <summary>
        /// Command that is responsible for the next month switching.
        /// </summary>
        public readonly static RoutedUICommand NextCommand;

        /// <summary>
        /// Command that is responsible for the previous month switching.
        /// </summary>
        public readonly static RoutedUICommand PrevCommand;

        /// <summary>
        /// Command that is responsible for the calendar visual mode changing.
        /// </summary>
        public readonly static RoutedUICommand UpCommand;

        #endregion Commands

        #region Touch Support

#if SyncfusionFramework4_0 || SyncfusionFramework4_5

        private double manipulationStartingPoint = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTouchDown(TouchEventArgs e)
        {
            manipulationStartingPoint = e.GetTouchPoint(this).Position.X;
            base.OnTouchDown(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (manipulationStartingPoint - e.ManipulationOrigin.X >= 70)
                BeginMoving(MoveDirection.Next, 1);
            else if (manipulationStartingPoint - e.ManipulationOrigin.X <= -70)
                BeginMoving(MoveDirection.Prev, -1);

            e.Handled = true;
            manipulationStartingPoint = 0;

            base.OnManipulationCompleted(e);
        }

#endif

        #endregion Touch Support
    }
}