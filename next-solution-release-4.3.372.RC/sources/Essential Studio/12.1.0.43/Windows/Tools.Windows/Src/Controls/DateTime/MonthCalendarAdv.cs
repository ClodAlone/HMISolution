#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Drawing;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using System.Collections.Specialized;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	using GridRangeInfo=GridRangeInfoAdv;

	/// <summary>
	/// The MonthCalendarAdv extends the Windows Forms MonthCalendar control and provides several important features:
	/// Internationalization - The MonthCalendarAdv control is fully culture aware. 
	///	XP Themes - The MonthCalendarAdv control supports XP themes when applicable. 
	///	Multiple selection - The MonthCalendarAdv can select multiple non-consecutive dates.
	/// </summary>
	[
	Designer( typeof( Syncfusion.Windows.Forms.Tools.MonthCalendarAdvDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) ),
	TypeConverter( typeof( MonthCalendarAdvConverter ) ),
	ToolboxItem( true ),
	ToolboxBitmap( typeof( MonthCalendarAdv ), "ToolboxIcons.MonthCalendarExt.bmp" ),
	Description( "Represents advanced MonthCalendar with Themes, Internationalization, Multiple Selection support." ),
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
 DesignerSerializer( typeof( MonthCalendarAdvDesignSerializer ), typeof( CodeDomSerializer ) )
#endif
]
	public class MonthCalendarAdv:
		UserControl,
		ISupportInitialize,
		INonClientPaintingSupport,
		ISupportCulture,
        IVisualStyle 
	{
		#region Constants
		private const int c_iBorderThickness = 2;
		private const int DEF_SAUDI_ARABIA_LCID = 1025;
		private readonly DateTime c_validMinValue = DateTime.MinValue.AddMinutes( 1 );
		private readonly DateTime c_validMaxValue = DateTime.MaxValue;
		private readonly Font c_defaultHeaderFont = FontUtil.CreateFont( "Microsoft Sans Serif", 8.25F, FontStyle.Bold );
		#endregion

		/// <summary>
		/// Fired when user selects disabled date value.
		/// </summary>
		internal event EventHandler InvalidDateSelected;

		private System.ComponentModel.IContainer components;
		#region variables
        /// <summary>
        /// Specifies a value indicating whether the calendar control should handle Enter
        /// key to move between cells. Set this to False if Enter should be ignored instead.
        /// </summary>
        private bool wantEnterKey = true;
		/// <summary>
		/// Mouse drag multiselect mode.
		/// </summary>
		private bool m_bMouseDragMultiselect = false;
		/// <summary>
		/// Moused drag multiselect firts row col point.
		/// X - row. Y - col.
		/// </summary>
		private Point m_ptMouseDragMultiselectFirtsRowCol = Point.Empty;
		/// <summary>
		/// Moused drag multiselect last row col point.
		/// X - row. Y - col.
		/// </summary>
		private Point m_ptMouseDragMultiselectLastRowCol = Point.Empty;
		/// <summary>
		/// When need skip CheckCell on CellClick event this member set to true;
		/// </summary>
		private bool m_bSkipCheckCell = false;
		/// <summary>
		/// Holds time when last mouse down event took place.
		/// </summary>
		/// <remarks>Internal usage variable</remarks>
		private long m_tiksLastClickTime;
		/// <summary>
		/// Holds cursor position while last mouse down event.
		/// </summary>
		/// <remarks>Internal usage variable</remarks>
		private Point m_ptLastMouseDown = Point.Empty;
		/// <summary>
		/// Indicates whether MonthCalendarAdv was double clicked.
		/// </summary>
		/// <remarks>Internal usage variable</remarks>
		private bool m_bDoubleClicked = false;
		private bool dateTimePickerCalendar = false;
		internal bool DateTimePickerCalendar
		{
			get { return dateTimePickerCalendar; }
			set { dateTimePickerCalendar = value; }
		}
		private CalendarGrid grid = null;
		private CalendarModel model = null;
		private System.Windows.Forms.Panel headerPanel;
		private System.Windows.Forms.Panel bottomPanel;
		private MonthCalendarTodayButton todayButton;
		private MonthCalendarNoneButton nullButton;
		private System.Windows.Forms.Label monthLabel;
		private ThemedScrollButton leftButton;
		private ThemedScrollButton rightButton;
		private Syncfusion.Windows.Forms.Tools.YearNumericUpDown yearUD;
		private System.Windows.Forms.Label yearLabel;
		private CultureInfo culture = CultureInfo.InvariantCulture;
		private DateTime dvalue = DateTime.Now;
		private DateTime minValue = DateTime.MinValue.AddMinutes( 1 );
		private DateTime maxValue = DateTime.MaxValue;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem gotoToday;
		private GridFontInfo dayNamesFontInfo = null;
		private Font dayNamesFont = null;
		private int dayNamesHeight = 17;
		private Font daysFont = null;
		private GridFontInfo daysFontInfo;
		private Color daysColor = SystemColors.ControlText;
		private Color headerStartColor = SystemColors.ActiveCaption;
		private Color headerEndColor = SystemColors.ControlDark;
		private bool headGradient = false;
		private Image headImage;
		private Brush headerBrush = new SolidBrush( SystemColors.ActiveCaptionText );
		private bool headerVerticalGradient = true;
		private int prevMonthCell;
		private int nextMonthCell;
		private ArrayList virtualCells;
		private Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu popupMonths;
		private Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem parentBarItem1;
		private string[] days = new string[] { "", "", "", "", "", "", "", "" };
		private string[] months = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "" };
		private int buttonOffset = 10;
		private GridHorizontalAlignment horizontalAlignment = GridHorizontalAlignment.Center;
		private GridVerticalAlignment verticalAlignment = GridVerticalAlignment.Middle;
		private bool m_bCellClicked = false;
		private bool cancelMoveTo = false;
		private Color highlightColor = Color.Red;
		private Color inactiveMonthColor = SystemColors.InactiveCaptionText;
		private Color dayNamesColor = SystemColors.ControlText;
		private Color todayFontColor = SystemColors.ActiveCaption;
		private bool sizeToFit = false;
		private bool needsRefresh = false;
        private DateTime[] selectedDates = new DateTime[0];
		private bool allowMultipleSelection = true;
        private bool clearSelectionOnNone = false;
		private bool m_bShortestDayNames = true;
		private VisualStyle style = VisualStyle.Default;
		private Office2007Theme m_office2007Theme = Office2007Theme.Blue;
		private Office2007Colors m_office2007ColorTable = null;
        private Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        private Office2010Colors m_office2010ColorTable = null;
		/// <summary>
		///the circle color.
		/// </summary>
        private Color circleColor = Color.FromArgb(204, 204, 204);
		/// <summary>
		/// value of mouse over.
		/// </summary>
        private bool b_mousehover = false;
		/// <summary>
		///value of mouse down press.
		/// </summary>
        private bool b_mousedown = false;
		/// <summary>
		///gets the value on left button click.
		/// </summary>
        private bool b_leftbutton = false;
		/// <summary>
		/// gets the value on right button click.
		/// </summary>
        private bool b_rightbutton = false;
		/// <summary>
		/// The border type of the text box control.
		/// </summary>
		private BorderStyle m_borderStyle = BorderStyle.None;
		/// <summary>
		/// The border sides for which you want the 3D border style applied.
		/// </summary>
		private Border3DSide m_borderSides = Border3DSide.All;
		/// <summary>
		/// The 3D border style for the control.
		/// </summary>
		private Border3DStyle m_border3DStyle = Border3DStyle.Sunken;
		/// <summary>
		/// The single border color for the control.
		/// </summary>
		private Color m_borderColor = Color.FromArgb(209,211,212);
		/// <summary>
		/// Is need Draw themed border.
		/// </summary>
		private bool m_bThemedBorder = true;
		/// <summary>
		/// Use for drawing themed border.
		/// </summary>
		private ThemedEditDrawing m_themedEditDrawing;
		/// <summary>
		/// Use for drawing system border.
		/// </summary>
		private ControlDrawing m_controlDrawing = new ControlDrawing();

		/// <summary>
		/// The first day of the week.
		/// </summary>
		private Day m_firstDayOfWeek = Day.Default;

		/// <summary/>
		private bool m_bThemedEnabledGrid;

		/// <summary/>
		private Color m_gridBackColor = SystemColors.Window;
        /// <summary></summary>
        private Color m_metroColor = Color.FromArgb(22,165,220);
        /// <summary>
        /// Default Control Size
        /// </summary>
        private Size CTRLSIZE = default(Size);
        /// <summary>
        /// Default header height
        /// </summary>
        private static int HDRHEIGHT = default(int);
        /// <summary>
        /// Default Bottom height
        /// </summary>
        private static int BTMHEIGHT = default(int);
		/// <summary>
		/// Occurs when the user makes a date selection.
		/// </summary>
		[Description( "Occurs when the user makes a date selection." )]
		public event EventHandler DateSelected;

		/// <summary>
		/// Occurs when the date selected in the MonthCalendarAdv changes.
		/// </summary>
		[Description( "Occurs when the date selected in the MonthCalendarAdv changes." )]
		public event EventHandler DateChanged;

		/// <summary>
		/// Occurs when the selection of date within the control has changed.
		/// </summary>
		[Description( "Occurs when the selection of date within the control has changed." )]
		public event EventHandler SelectionChanged;

		/// <summary>
		/// Occurs when a NoneButton on the MonthCalendarAdv is clicked.
		/// </summary>
		[Description( "Occurs when a NoneButton on the MonthCalendarAdv is clicked." )]
		public event EventHandler NoneButtonClick;

		/// <summary>
		/// This event is raised if the BorderStyle property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the BorderStyle property is changed." )]
		public event EventHandler BorderStyleChanged;

		/// <summary>
		/// This event is raised if the BorderSides property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the BorderSides property is changed." )]
		public event EventHandler BorderSidesChanged;

		/// <summary>
		/// This event is raised if the Border3DStyle property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the Border3DStyle property is changed." )]
		public event EventHandler Border3DStyleChanged;

		/// <summary>
		/// This event is raised if the BorderColor property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the BorderColor property is changed." )]
		public event EventHandler BorderColorChanged;

		/// <summary>
		/// This event is raised if the ThemedBorder property is changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "This event is raised if the ThemedBorder property is changed." )]
		public event EventHandler ThemedBorderChanged;

		/// <summary>
		/// Occurs when <see cref="StretchScrollImage"/> is changed.
		/// </summary>
		[
		Category( "Property Changed" ),
		Description( "Occurs when StretchScrollImage is changed." )
		]
		public event EventHandler StretchScrollImageChanged;

		/// <summary>
		/// Occurs when FirstDayOfWeek is changed.
		/// </summary>
		[
		Description( "Occurs when FirstDayOfWeek is changed." ),
		Category( "Behavior" )
		]
		public event EventHandler FirstDayOfWeekChanged;

		/// <summary>
		/// Raises the BorderStyleChanged event.
		/// </summary>
		protected virtual void OnBorderStyleChanged()
		{
			if( this.BorderStyleChanged != null )
			{
				this.BorderStyleChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Raises the BorderSidesChanged event.
		/// </summary>
		protected virtual void OnBorderSidesChanged()
		{
			if( this.BorderSidesChanged != null )
			{
				this.BorderSidesChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Raises the Border3DStyleChanged event.
		/// </summary>
		protected virtual void OnBorder3DStyleChanged()
		{
			if( this.Border3DStyleChanged != null )
			{
				this.Border3DStyleChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Raises the BorderColorChanged event.
		/// </summary>
		protected virtual void OnBorderColorChanged()
		{
			if( this.BorderColorChanged != null )
			{
				this.BorderColorChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Raises the ThemedBorderChanged event.
		/// </summary>
		protected virtual void OnThemedBorderChanged()
		{
			if( this.ThemedBorderChanged != null )
			{
				this.ThemedBorderChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Handle this event to provide custom formatting for calendar cells.
		/// </summary>
		[Description( "Handle this event to provide custom formatting for calendar cells." )]
		public event DateCellQueryInfoEventHandler DateCellQueryInfo;

		/// <summary>
		/// Raises the NoneButton_Click event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnNoneButtonClick( EventArgs e )
		{
			if( NoneButtonClick!= null )
			{
				NoneButtonClick( this, e );
			}
		}

		/// <summary>
		/// Raises the SelectionChanged event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnSelectionChanged( EventArgs e )
		{
			if( SelectionChanged!= null )
			{
				SelectionChanged( this, e );
			}
		}
		/// <summary>
		/// Raises the DateSelected event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void OnDateSelected( EventArgs e )
		{
			if( DateSelected!=null )
			{
				DateSelected( this, e );
			}
		}
		/// <summary>
		/// Raises the DateChanged event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void OnDateChanged( EventArgs e )
		{
			if( DateChanged != null )
			{
				DateChanged( this, e );
			}
		}
		#endregion
		#region Properties
        /// <summary>
        /// Gets or sets the theme color of the MonthCalenderAdv
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the background color of the control.")
        ]
        public Color MetroColor
        {
            get { return m_metroColor; }
            set
            {
                m_metroColor = value;
            }
        }
        private bool ShouldSerializeMetroColor()
        {
           return  this.MetroColor != Color.Empty;
        }
        private void ResetMetroColor()
        {
            this.MetroColor = Color.Empty;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the calendar control should handle Enter
        /// key to move between cells. Set this to False if Enter should be ignored instead.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WantEnterKey
        {
            get
            {
                return this.wantEnterKey;
            }
            set
            {
                if (this.wantEnterKey != value)
                {
                    this.wantEnterKey = value;
                    if (this.grid != null && this.grid.GridControl.WantEnterKey != value)
                        this.grid.GridControl.WantEnterKey = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets mouse drag multiselect mode.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[Description( "Gets or sets mouse drag multiselect mode." )]
		public bool MouseDragMultiselect
		{
			get
			{
				return m_bMouseDragMultiselect;
			}
			set
			{
				m_bMouseDragMultiselect = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicating whether the shortest name of the weekdays should be used.	This property value have influence only under Framework 2.0 and higher.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Appearance" )]
		[Description( "Gets or sets value indicating whether the shortest name of the weekdays should be used.	This property value have influence only under Framework 2.0 and higher." )]
		public bool UseShortestDayNames
		{
			get
			{
				return m_bShortestDayNames;
			}
			set
			{
				if( m_bShortestDayNames != value )
				{
					m_bShortestDayNames = value;
					RefreshNames();
				}
			}
		}
		/// <summary>
		/// Gets or sets the visual style for the MonthCalendarAdv.
		/// </summary>
		[TypeConverter( typeof( DefaultVisualStyleEnumFilter ) )]
		[Description( "Gets or sets the visual style for the MonthCalendarAdv." )]
        [DefaultValue(typeof(VisualStyle), "Default")]
		public VisualStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				if( style != value )
				{
					style = value;
					leftButton.Style = value;
					rightButton.Style = value;

					this.yearUD.ThemesEnabled = true;
					this.yearUD.ThemedBorder = true;
					this.yearUD.VisualStyle = value;

					if( this.DesignMode || !this.m_bIsInitializing )
					{
						this.HeadForeColor = SystemColors.ActiveCaptionText;
                        this.TodayButton.ForeColor = this.NoneButton.ForeColor = DefaultForeColor;
                        this.rightButton.Location = new System.Drawing.Point(168, 8);
                        this.leftButton.Location = new System.Drawing.Point(8, 8);
						this.HeadGradient = false;
					}

					if( style == Syncfusion.Windows.Forms.VisualStyle.Default )
					{
						this.ResetHeaderStartColor();
						this.ResetHeaderEndColor();
						this.ResetHeaderFont();
						this.HeaderFont = new Font( monthLabel.Font, FontStyle.Bold );
						this.leftButton.FlatColor = SystemColors.ControlDark;
						this.rightButton.FlatColor = SystemColors.ControlDark;
						this.leftButton.Transparent = false;
						this.rightButton.Transparent = false;
						this.leftButton.BackColor = BackColor;
						this.rightButton.BackColor = BackColor;
						buttonOffset = 10;
						this.ScrollButtonSize = new Size( 17, 19 );
						this.HeaderHeight = 32;
						this.ResetDaysHeaderInterior();
						this.GridLines = Syncfusion.Windows.Forms.Grid.GridBorderStyle.Dotted;
						this.TodayButton.UseVisualStyle = false;
						this.NoneButton.UseVisualStyle = false;
						this.TodayButton.Appearance = ButtonAppearance.None;
						this.NoneButton.Appearance = ButtonAppearance.None;
					}
					else
					{
						if( this.Style == VisualStyle.OfficeXP )
						{
							if( this.DesignMode )
								MenuColors.UpdateMenuColors();

							if( this.DesignMode || !this.m_bIsInitializing )
							{
								this.HeaderStartColor = MenuColors.PressedSelColor;
								this.ResetHeaderEndColor();
								this.ResetHeaderFont();
								this.HeaderFont = new Font( monthLabel.Font, FontStyle.Regular );
							}

							this.TodayButton.UseVisualStyle = true;
							this.NoneButton.UseVisualStyle = true;
							this.TodayButton.Appearance = ButtonAppearance.OfficeXP;
							this.NoneButton.Appearance = ButtonAppearance.OfficeXP;
						}
						else if( this.Style == VisualStyle.Office2007 )
						{
							m_office2007ColorTable = Office2007Colors.GetColorTable( m_office2007Theme );

							if( this.DesignMode || !this.m_bIsInitializing )
							{
								this.HeadGradient = true;
								this.ResetHeaderFont();
								this.HeaderFont = new Font( monthLabel.Font, FontStyle.Regular );
								this.HeaderStartColor = m_office2007ColorTable.MonthCalendarHeaderStartColor;
								this.HeaderEndColor = m_office2007ColorTable.MonthCalendarHeaderEndColor;
								this.HeadForeColor = m_office2007ColorTable.MonthCalendarForeColor;
							}
							this.yearUD.BorderStyle = BorderStyle.FixedSingle;
							leftButton.ArrowColor = m_office2007ColorTable.MonthCalendarForeColor;
							rightButton.ArrowColor = m_office2007ColorTable.MonthCalendarForeColor;

							this.TodayButton.UseVisualStyle = true;
							this.NoneButton.UseVisualStyle = true;
							this.TodayButton.Appearance = ButtonAppearance.Office2007;
							this.TodayButton.Office2007ColorScheme = m_office2007Theme;
							this.NoneButton.Appearance = ButtonAppearance.Office2007;
							this.NoneButton.Office2007ColorScheme = m_office2007Theme;

						}
                        else if (this.Style == VisualStyle.Office2010)
                        {
                            m_office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);

                            if (this.DesignMode || !this.m_bIsInitializing)
                            {
                                this.HeadGradient = true;
                                this.ResetHeaderFont();
                                this.HeaderFont = new Font(monthLabel.Font, FontStyle.Regular);
                                this.HeaderStartColor = m_office2010ColorTable.MonthCalendarHeaderStartColor;
                                this.HeaderEndColor = m_office2010ColorTable.MonthCalendarHeaderEndColor;
                                this.HeadForeColor = m_office2010ColorTable.MonthCalendarForeColor;
                                this.GridBackColor = m_office2010ColorTable.MonthCalendarBackgroundColor;
                            }
                            this.yearUD.BorderStyle = BorderStyle.FixedSingle;
                            leftButton.ArrowColor = m_office2010ColorTable.MonthCalendarForeColor;
                            rightButton.ArrowColor = m_office2010ColorTable.MonthCalendarForeColor;

                            this.TodayButton.UseVisualStyle = true;
                            this.NoneButton.UseVisualStyle = true;
                            this.TodayButton.Appearance = ButtonAppearance.Office2010;
                            this.TodayButton.Office2010ColorScheme = m_office2010Theme;
                            this.NoneButton.Appearance = ButtonAppearance.Office2010;
                            this.NoneButton.Office2010ColorScheme = m_office2010Theme;

                        }
						else if( this.Style == VisualStyle.Office2003 )
						{
							Office2003Colors.UpdateMenuColors();

							if( this.DesignMode || !this.m_bIsInitializing )
							{
								this.HeaderStartColor = Office2003Colors.PressedSelColor;
								this.ResetHeaderEndColor();
								this.ResetHeaderFont();
								this.HeaderFont = new Font( monthLabel.Font, FontStyle.Regular );
							}

							this.TodayButton.UseVisualStyle = true;
							this.NoneButton.UseVisualStyle = true;
							this.TodayButton.Appearance = ButtonAppearance.Office2003;
							this.NoneButton.Appearance = ButtonAppearance.Office2003;
						}
                        else if (this.Style == VisualStyle.Metro)
                        {
                            if (this.DesignMode || !this.m_bIsInitializing)
                            {
                                this.HeaderFont = new Font(monthLabel.Font, FontStyle.Regular);
                                this.HeaderStartColor = Color.White;
                                this.DayNamesColor = MetroColor;
                                this.ResetHeaderEndColor();
                                this.ResetHeaderFont();
                                this.DayNamesFont = new Font(monthLabel.Font, FontStyle.Bold);
                                this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                                this.leftButton.ArrowColor = Color.FromArgb(111, 111, 111);
                                this.rightButton.ArrowColor = Color.FromArgb(111,111,111);
                            }
                            this.Size = new Size(213, 183);
                            this.HighlightColor = Color.White;
                            this.TodayButton.UseVisualStyle = true;
                            this.NoneButton.UseVisualStyle = true;                          
                            this.TodayButton.Appearance = ButtonAppearance.Metro;
                            this.NoneButton.Appearance = ButtonAppearance.Metro;
                            this.TodayButton.BackColor = MetroColor;
                            this.NoneButton.BackColor = MetroColor;
                            this.Invalidate();
                            this.bottomPanel.Size = new System.Drawing.Size(192, 25);
                            this.leftButton.Location = new System.Drawing.Point(6, 8);
                            this.rightButton.Location = new System.Drawing.Point(170, 8);
                        }
						else
						{
							this.TodayButton.UseVisualStyle = false;
							this.NoneButton.UseVisualStyle = false;
							this.TodayButton.Appearance = ButtonAppearance.Classic;
							this.NoneButton.Appearance = ButtonAppearance.Classic;
						}

						if( this.DesignMode || !this.m_bIsInitializing )
						{
                            if (this.style == VisualStyle.Metro)
                            {
                                this.HeaderFont = new Font(monthLabel.Font, FontStyle.Bold);
                                this.HeaderHeight = 34;
                            }
                            else
                            {
                                this.HeaderHeight = 20;
                                this.ResetHeaderFont();
                                this.HeaderFont = new Font(monthLabel.Font, FontStyle.Regular);
                                this.TodayButton.ForeColor = this.NoneButton.ForeColor = DefaultForeColor;
                            }
						}

						this.leftButton.FlatColor = Color.Transparent;
						this.rightButton.FlatColor = Color.Transparent;
						this.leftButton.Transparent = true;
						this.rightButton.Transparent = true;
						this.leftButton.BackColor = Color.Transparent;
						this.rightButton.BackColor = Color.Transparent;
						buttonOffset = 0;
						this.ScrollButtonSize = new Size( 24, 24 );

						if( grid != null )
						{
							this.DaysHeaderInterior = new BrushInfo( grid.BackColor );
						}

						this.GridLines = Syncfusion.Windows.Forms.Grid.GridBorderStyle.None;
					}

                    if (style != VisualStyle.Metro)
                    {
                        this.TodayButton.BackColor = BackColor;
                        this.NoneButton.BackColor = BackColor;
                    }
                    if (style != VisualStyle.Office2010)
                    {
                        this.GridBackColor=SystemColors.Window;
                        this.Invalidate();
                    }
					RefreshNames();
				}
			}
		}

		/// <summary>
		/// Resets the style of the control to VisualStyle.Default.
		/// </summary>
		protected void ResetStyle()
		{
			this.Style = VisualStyle.Default;
		}
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;

                if (value == "Office2007Blue")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Style = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = VisualStyle.Office2007;
                    Office2010Theme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    Style = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Managed;
                }
                else if (value == "Default")
                    Style = VisualStyle.Default;
                else if (value == "Office2003")
                    Style = VisualStyle.Office2003;
                else if (value == "OfficeXP")
                    Style = VisualStyle.OfficeXP;
                else if (value == "Office2007Outlook")
                    Style = VisualStyle.Office2007Outlook;
                else if (value == "VS2005")
                    Style = VisualStyle.VS2005;
                else if (value == "VS2010")
                    Style = VisualStyle.VS2010;
                else if (value == "Metro")
                    Style = VisualStyle.Metro;
           }
        }
		/// <summary>
		/// Indicates the Office2007 theme used for drawing the control.
		/// </summary>
		[
		Description( "Gets or sets a value indicating the Office2007 theme used for drawing the control." ),
		Category( "Appearance" ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007Theme
		{
			get { return m_office2007Theme; }
			set
			{
				if( m_office2007Theme != value )
				{
					m_office2007Theme = value;

					OnOffice2007ThemeChanged();
				}
			}
		}
        /// <summary>
        /// Indicates the Office2010 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets or sets a value indicating the Office2010 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get { return m_office2010Theme; }
            set
            {
                if (m_office2010Theme != value)
                {
                    m_office2010Theme = value;

                    OnOffice2010ThemeChanged();
                }
            }
        }
		/// <summary>
		/// Gets or sets the context menu for the MonthCalendarAdv.
		/// </summary>
		public new ContextMenu ContextMenu
		{
			get
			{
				return base.ContextMenu;
			}
			set
			{
				base.ContextMenu = value;
				grid.ContextMenu = Menu;
			}
		}
		private ContextMenu Menu
		{
			get
			{
				this.gotoToday.Text = SR.GetString(culture, SR.GotoToday, this);
				if( this.ContextMenu == null )
				{
					return contextMenu;
				}
				else
				{
					return ContextMenu;
				}
			}
		}
		/// <summary>
		/// Indicates whether the calendar will adapt it`s width to fit the contents of the day names.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Indicates whether the calendar will adapt it`s width to fit the contents of the day names." )]
		public bool SizeToFit
		{
			get { return sizeToFit; }
			set
			{
				if( sizeToFit!=value )
				{
					sizeToFit = value;
					RecalculateWidth();
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the today date text in the calendar.
		/// </summary>
		[Description( "Indicates the color of the today date text in the calendar." )]
		[Category( "Appearance" )]
		public Color TodayFontColor
		{
			get { return todayFontColor; }
			set
			{
				if( todayFontColor!=value )
				{
					todayFontColor = value;

					if( grid != null )
						grid.Refresh();
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeTodayFontColor()
		{
			return todayFontColor!=SystemColors.ActiveCaption;
		}
		/// <summary>
		/// Resets the color of the today font to SystemColors.ActiveCaption.
		/// </summary>
		protected void ResetTodayFontColor()
		{
			TodayFontColor = SystemColors.ActiveCaption;
		}

		/// <summary>
		/// Returns the today button at the bottom of the calendar.
		/// </summary>
		[Description( "The today button at the bottom of the calendar." )]
		[Category( "Appearance" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public MonthCalendarButton TodayButton
		{
			get { return todayButton; }
		}

		/// <summary>
		/// Returns the none button at the bottom of the calendar.
		/// </summary>
		[Description( "The none button at the bottom of the calendar." )]
		[Category( "Appearance" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public MonthCalendarButton NoneButton
		{
			get { return nullButton; }
		}

		/// <summary>
		/// Gets or sets the minimum value selectable by the calendar.
		/// </summary>
		[Description( "Indicates the minimum value selectable by the calendar." )]
		[Category( "Behavior" )]
		[Editor( typeof( DateTimeEditorAdv ), typeof( UITypeEditor ) )]
		public DateTime MinValue
		{
			get
			{
				return minValue;
			}
			set
			{
				if( value > maxValue )
				{
					throw new ArgumentException( "MaxValue must be greater then MinValue.", "value" );
				}
				else
				{
					minValue = ( value >= c_validMinValue ) ? value.Date : c_validMinValue.Date;
					if( minValue > dvalue )
					{
						Value = minValue;
					}
                    
                    if(this.minValue >= CurrentCalendar.MinSupportedDateTime)
                        this.yearUD.Minimum = CurrentCalendar.GetYear(this.minValue);
				}
			}
		}

		private bool ShouldSerializeMinValue()
		{
			if( this.MinValue <= c_validMinValue )
				return false;
			else
				return true;
		}

		private void ResetMinValue()
		{
			this.MinValue = c_validMinValue;
		}

		/// <summary>
		/// Gets or sets the maximum value selectable by the calendar.
		/// </summary>
		[Description( "Indicates the maximum value selectable by the calendar." )]
		[Category( "Behavior" )]
		[Editor( typeof( DateTimeEditorAdv ), typeof( UITypeEditor ) )]
		public DateTime MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{

				if( value < minValue )
				{
					throw new ArgumentException( "MaxValue must be greater then MinValue.", "value" );
				}
				else
				{
					maxValue = ( value <= c_validMaxValue ) ? value : c_validMaxValue;
					if( maxValue < dvalue )
					{
						Value = maxValue;
					}

                    if(this.maxValue <= CurrentCalendar.MaxSupportedDateTime)
                        this.yearUD.Maximum = CurrentCalendar.GetYear(this.maxValue);
				}
			}
		}


		private bool ShouldSerializeMaxValue()
		{
			if( this.MaxValue == c_validMaxValue )
				return false;
			else
				return true;
		}

		private void ResetMaxValue()
		{
			this.MaxValue = c_validMaxValue;
		}


		/// <summary>
		/// Gets or sets the selection types of the calendar.
		/// </summary>
		[Description( "Indicates the selection types of the calendar." )]
		[Category( "Behavior" )]
		[DefaultValue( GridSelectionFlags.Any )]
		[Obsolete( "This enum has been replaced by the AllowMultipleSellection bool property" )]
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public GridSelectionFlags AllowSelection
		{
			get
			{
				return model.Options.AllowSelection;
			}
			set
			{
				model.Options.AllowSelection = value;
			}
		}

        private bool iso8601Calenderformat = false;
        public bool Iso8601CalenderFormat
        {
            get
            {
                return iso8601Calenderformat;
            }
            set
            {
                iso8601Calenderformat = value;
            }
        }

		/// <summary>
		/// Indicates the selection types of the calendar.
		/// </summary>
		[Description( "Indicates the selection types of the calendar." )]
		[Category( "Behavior" )]
		[DefaultValue( true )]
		public bool AllowMultipleSelection
		{
			get { return allowMultipleSelection; }
			set { allowMultipleSelection = value; }
		}
		/// <summary>
		/// Sets the PopupParent.
		/// </summary>
		CalendarPopup popupParent;
		[Browsable( false )]
		public CalendarPopup PopupParent
		{
			set
			{
				this.popupParent = value;
			}
		}
        /// <summary>
        /// Sets the Selected date to null.
        /// </summary>
        [Description("Whether to clear the selected dates on none button click")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ClearSelectionOnNone
        {
            get { return clearSelectionOnNone; }
            set { clearSelectionOnNone = value; }
        }


		/// <summary>
		/// Gets or sets the imagelist of the popup menu.
		/// </summary>
		[Description( "Indicates the imagelist of the popup menu." )]
		[Category( "Appearance" )]
		[DefaultValue( null )]
		public ImageList MonthImageList
		{
			get { return parentBarItem1.ImageList; }
			set
			{
				parentBarItem1.ImageList = value;
				for( int i=0; i<this.parentBarItem1.Items.Count; i++ )
				{
					( (BarItem)this.parentBarItem1.Items[i] ).ImageList = value;
					( (BarItem)this.parentBarItem1.Items[i] ).ImageIndex = i;
				}
			}
		}
		/// <summary>
		/// Gets or sets the color of the day names.
		/// </summary>
		[Description( "Indicates the color of the day names." )]
		[Category( "Appearance" )]
		public Color DayNamesColor
		{
			get { return dayNamesColor; }
			set
			{
				if( dayNamesColor == value )
					return;

				dayNamesColor = value;

				if( grid != null )
					grid.Refresh();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDayNamesColor()
		{
			return DayNamesColor != SystemColors.ControlText;
		}
		/// <summary>
		/// Resets the color of the day names to SystemColors.ControlText.
		/// </summary>
		protected void ResetDayNamesColor()
		{
			DayNamesColor = SystemColors.ControlText;
		}

		/// <summary>
		/// Gets or sets the forecolor of the header.
		/// </summary>
		[Description( "Indicates the fore color of the header." )]
		[Category( "Appearance" )]
		public Color HeadForeColor
		{
			get { return monthLabel.ForeColor; }
			set
			{
				monthLabel.ForeColor = value;
				yearLabel.ForeColor = value;
				headerPanel.Invalidate();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHeadForeColor()
		{
			return HeadForeColor!=SystemColors.ActiveCaptionText;
		}
		/// <summary>
		/// Resets the HeaderForeColor to SystemColors.ActiveCaptionText
		/// </summary>
		protected void ResetHeaderForeColor()
		{
			HeadForeColor = SystemColors.ActiveCaptionText;
		}
		/// <summary>
		/// Gets or sets the trailing forecolor of the calendar.
		/// </summary>
		[Description( "Indicates the trailing fore color of the calendar." )]
		[Category( "Appearance" )]
		public Color InactiveMonthColor
		{
			get { return inactiveMonthColor; }
			set
			{
				if( inactiveMonthColor == value )
					return;

				inactiveMonthColor = value;

				if( grid != null )
					grid.Refresh();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeInactiveMonthColor()
		{
			return inactiveMonthColor!=SystemColors.InactiveCaptionText;
		}
		/// <summary>
		/// Resets the color of the inactive month to SystemColors.InactiveCaptionText.
		/// </summary>
		protected void ResetInactiveMonthColor()
		{
			InactiveMonthColor = SystemColors.InactiveCaptionText;
		}

		/// <summary>
		/// Gets or sets the color of the selected date.
		/// </summary>
		[Description( "Indicates the color of the selected date." )]
		[Category( "Appearance" )]
		public Color HighlightColor
		{
			get { return highlightColor; }
			set
			{
				if( highlightColor == value )
					return;

				highlightColor = value;

				if( grid != null )
					grid.Refresh();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHighlightColor()
		{
			return highlightColor!=Color.Red;
		}
		/// <summary>
		/// Resets the  highlight color to Red.
		/// </summary>
		protected void ResetHighlightColor()
		{
			HighlightColor = Color.Red;
		}

		/// <summary>
		/// Indicates the themed state of the scroll buttons.
		/// </summary>
		[Description( "Indicates the themed state of the scroll buttons." )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public bool ThemedEnabledScrollButtons
		{
			get { return leftButton.ThemesEnabled; }
			set { leftButton.ThemesEnabled = value; rightButton.ThemesEnabled = value; }
		}
		/// <summary>
		/// Indicates the themed state of the grid.
		/// </summary>
		[Description( "Indicates the themed state of the grid." )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		public bool ThemedEnabledGrid
		{
			get
			{
				return m_bThemedEnabledGrid;
			}
			set
			{
				m_bThemedEnabledGrid = value;

				if( grid != null )
				{
					grid.ThemesEnabled = m_bThemedEnabledGrid;
				}
			}
		}

		/// <summary>
		/// Gets or sets the size of the scroll buttons.
		/// </summary>
		[Description( "Indicates the size of the scroll buttons." )]
		[Category( "Appearance" )]
		public Size ScrollButtonSize
		{
			get { return leftButton.Size; }
			set
			{
				leftButton.Size = value;
				rightButton.Size = value;
				SetHeaderPositions();
			}
		}

		/// <summary>
		/// Gets or sets image for left scroll button.
		/// If this value is null then draw default button.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "Image for left scroll button." ),
		DefaultValue( null )
		]
		public Image LeftScrollButtonImage
		{
			get
			{
				return leftButton.Image;
			}
			set
			{
				if( value != leftButton.Image )
				{
					leftButton.Image = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets image for right scroll button.
		/// If this value is null then draw default button.
		/// </summary>
		[
		Category( "Appearance" ),
		Description( "Image for reight scroll button." ),
		DefaultValue( null )
		]
		public Image RightScrollButtonImage
		{
			get
			{
				return rightButton.Image;
			}
			set
			{
				if( value != rightButton.Image )
				{
					rightButton.Image = value;
				}
			}
		}

		/// <summary>
		/// Indicate wether the image for scroll button 
		/// is stretched or shrunk to fit the size of the scroll button.
		/// </summary>
		private bool m_stretchScrollImage = false;

		/// <summary>
		/// Indicate wether the image for scroll button 
		/// is stretched or shrunk to fit the size of the scroll button.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Indicate wether the image for scroll button is stretched or shrunk to fit the size of the scroll button." ),
		Category( "Behavior" )
		]
		public bool StretchScrollImage
		{
			get
			{
				return m_stretchScrollImage;
			}
			set
			{
				if( value != m_stretchScrollImage )
				{
					m_stretchScrollImage = value;
					OnStretchScrollImageChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets first day of week.
		/// </summary>
		[
		Description( "First day of week." ),
		DefaultValue( typeof( Day ), "Default" ),
		Category( "Behavior" )
		]
		public Day FirstDayOfWeek
		{
			get
			{
				return m_firstDayOfWeek;
			}
			set
			{
				if( value != m_firstDayOfWeek )
				{
					m_firstDayOfWeek = value;
					OnFirstDayOfWeekChanged();
				}
			}
		}

		private void RaiseFirstDayOfWeekChanged()
		{
			if( FirstDayOfWeekChanged != null )
			{
				FirstDayOfWeekChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnFirstDayOfWeekChanged()
		{
			CultureInfo cultureInfo = this.Culture.Clone() as CultureInfo;

			if( cultureInfo != null )
			{
				int dayInt = (int)FirstDayOfWeek;
				DayOfWeek dayWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

				if( dayInt < DEF_DAYS_COUNT - 1 )
				{
					dayWeek = (DayOfWeek)dayInt + 1;
				}
				else if( dayInt == DEF_DAYS_COUNT - 1 )
				{
					dayWeek = DayOfWeek.Sunday;
				}

				cultureInfo.DateTimeFormat.FirstDayOfWeek = dayWeek;

				this.Culture = cultureInfo;
			}

			RaiseFirstDayOfWeekChanged();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeScrollButtonSize()
		{
			return ScrollButtonSize.Width != 17 || ScrollButtonSize.Height !=19;
		}

		/// <summary>
		/// Resets the size of the scroll button to (17,19).
		/// </summary>
		protected void ResetScrollButtonSize()
		{
			ScrollButtonSize = new Size( 17, 19 );
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the grid.
		/// </summary>
		[Description( "Indicates the horizontal allignment of the grid." )]
		[Category( "Appearance" )]
		[DefaultValue( GridHorizontalAlignment.Center )]
		public GridHorizontalAlignment HorizontalAlignment
		{
			get { return horizontalAlignment; }
			set
			{
				if( horizontalAlignment != value )
				{
					horizontalAlignment = value;

					if( grid != null )
						grid.Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the grid.
		/// </summary>
		[Description( "Indicates the vertical alignment of the grid." )]
		[Category( "Appearance" )]
		[DefaultValue( GridVerticalAlignment.Middle )]
		public GridVerticalAlignment VerticalAlignment
		{
			get { return verticalAlignment; }
			set
			{
				if( verticalAlignment != value )
				{
					verticalAlignment = value;

					if( grid != null )
						grid.Refresh();
				}
			}
		}

		//		/// <summary>
		//		/// Indicates the visibility of the bottom buttons.
		//		/// </summary>
		//		[Description("Indicates the visibility of the bottom buttons.")]
		//		[Category("Appearance")]
		//		public bool BottomPanelVisible
		//		{
		//			get{return bottomPanel.Visible;}
		//			set{bottomPanel.Visible = value;this.MonthCalendarAdv_SizeChanged(this,EventArgs.Empty);}
		//		}

		/// <summary>
		/// Gets or sets the selected dates.
		/// </summary>
		[Description( "Gets or sets the selected dates." )]
		[Category( "Behavior" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		public DateTime[] SelectedDates
		{
			get
			{
				ArrayList dates = new ArrayList();

				foreach( DictionaryEntry entry in m_selectedDates )
				{
					DateTime key = (DateTime)entry.Key;
					GridRangeInfoList list = (GridRangeInfoList)entry.Value;

					foreach( GridRangeInfo rinfo in list )
					{
						dates.AddRange( rinfo.Dates );
					}
				}

				return dates.ToArray( typeof( DateTime ) ) as DateTime[];
			}
			set
			{
                if (selectedDates != value)
                {
                    if (this.selectedDates.Length != 0)
                    {
                        Array.Sort<DateTime>(value);
                        for (int i = 0; i < value.Length; i++)
                        {
                            if (value[i] <= this.maxValue && value[0] >= this.minValue)
                            {
                                this.Value = value[i];
                                break;
                            }
                        }
                    }

                    this.m_selectedDates.Clear();
                    this.selectedDates = value;
                    if (grid != null)
                        ValueChanged(true);
                }
			}
		}

		/// <summary>
		/// Gets or sets the backcolor of the grid.
		/// </summary>
		[Description( "Indicates the back color of the grid." )]
		[Category( "Appearance" )]
		public Color GridBackColor
		{
			get
			{
				return m_gridBackColor;
			}
			set
			{
				m_gridBackColor = value;

				if( this.grid != null )
				{
					grid.BackColor = m_gridBackColor;
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeGridBackColor()
		{
			return GridBackColor != SystemColors.Window;
		}
		/// <summary>
		/// Resets the color of the grid background to SystemColors.Window.
		/// </summary>
		protected void ResetGridBackColor()
		{
			GridBackColor = SystemColors.Window;
		}

		/// <summary>
		/// Gets or sets the height of the day names.
		/// </summary>
		[Description( "Indicates the height of the day names." )]
		[Category( "Appearance" )]
		[DefaultValue( 17 )]
		public int DayNamesHeight
		{
			get { return dayNamesHeight; }
			set
			{
				dayNamesHeight = value;

				if( grid == null )
					model.RowHeights[0] = value;
				else
					grid.Model.RowHeights[0] = value;

				this.MonthCalendarAdv_SizeChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Gets or sets the font of the day names.
		/// </summary>
		[Description( "Indicates the font of the day names." )]
		[Category( "Appearance" )]
		public Font DayNamesFont
		{
			get
			{
				if( dayNamesFont == null )
					dayNamesFont = Syncfusion.Drawing.FontUtil.CreateFont( "Verdana", 8 );
				return dayNamesFont;
			}
			set
			{
				dayNamesFont = value;

				if( dayNamesFontInfo == null )
					dayNamesFontInfo = new GridFontInfo();

				dayNamesFontInfo.Facename = dayNamesFont.Name;
				dayNamesFontInfo.Bold = dayNamesFont.Bold;
				dayNamesFontInfo.Italic = dayNamesFont.Italic;
				dayNamesFontInfo.Underline = dayNamesFont.Underline;
				dayNamesFontInfo.Size = dayNamesFont.Size;
				dayNamesFontInfo.Strikeout = dayNamesFont.Strikeout;

				if( grid != null )
				{
					grid.Model.IgnoreReadOnly = true;
					grid.RefreshRange( GridRangeInfo.Cells( 0, 0, 0, DEF_DAYS_COUNT ) );
					grid.Model.IgnoreReadOnly = false;
				}
			}
		}

		/// <summary>
		/// Resets the day names font.
		/// </summary>
		protected void ResetDayNamesFont()
		{
			DayNamesFont = Syncfusion.Drawing.FontUtil.CreateFont( "Verdana", 8 );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDayNamesFont()
		{
			return DayNamesFont.FontFamily.Name != "Verdana" || DayNamesFont.Size !=8;
		}
		/// <summary>
		/// Gets or sets the style of the grid lines.
		/// </summary>
		[Description( "Determines the style of the grid lines." )]
		[Category( "Appearance" )]
		[DefaultValue( GridBorderStyle.Dotted )]
		public GridBorderStyle GridLines
		{
			get
			{
				return model.Options.DefaultGridBorderStyle;
			}
			set
			{
				model.Options.DefaultGridBorderStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets the height of the bottom controls.
		/// </summary>
		[Description( "Indicates the height of the bottom controls." )]
		[Category( "Appearance" )]
		[DefaultValue( 20 )]
		public int BottomHeight
		{
			get
			{
				return bottomPanel.Height;
			}
			set
			{
				bottomPanel.Height = value;
				this.MonthCalendarAdv_SizeChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Gets or sets the height of the header.
		/// </summary>
		[Description( "Indicates the height of the header." )]
		[Category( "Appearance" )]
		[DefaultValue( 32 )]
		public int HeaderHeight
		{
			get
			{
				return headerPanel.Height;
			}
			set
			{
				headerPanel.Height = value;
				this.MonthCalendarAdv_SizeChanged( this, EventArgs.Empty );
			}
		}

		/// <summary>
		/// Gets or sets the color of the days.
		/// </summary>
		[Description( "Indicates the color of the days." )]
		[Category( "Appearance" )]
		public Color DaysColor
		{
			get { return daysColor; }
			set
			{
				if( daysColor == value ) return;
				daysColor = value;

				for( int i = 1; i <= 6; i++ )
				{
					for( int j = 1; j <= DEF_DAYS_COUNT; j++ )
					{
						GetInfo( i, j ).TextColor = value;
					}
				}

				if( grid != null )
				{
					grid.Model.IgnoreReadOnly = true;
					grid.RefreshRange( GridRangeInfo.Cells( 1, 1, 6, DEF_DAYS_COUNT ) );
					grid.Model.IgnoreReadOnly = false;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDaysColor()
		{
			return daysColor!=SystemColors.ControlText;
		}
		/// <summary>
		/// Resets the color of the days to SystemColors.ControlText.
		/// </summary>
		protected void ResetDaysColor()
		{
			DaysColor = SystemColors.ControlText;
		}

		/// <summary>
		/// Gets or sets the background image of the calendar.
		/// </summary>
		[Description( "Indicates the background image of the calendar." )]
		[Category( "Appearance" )]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;

				if( grid != null )
				{
					grid.BackgroundImage = value;
				}
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Gets or sets the background image layout of the calendar.
		/// </summary>
		[Description( "Indicates the background image layout of the calendar." )]
		[Category( "Appearance" )]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;

				if( this.grid != null )
				{
					grid.BackgroundImageLayout = value;
				}
			}
		}
#endif

		/// <summary>
		/// Indicates whether the grid should wrap text.
		/// </summary>
		[Description( "Indicates of the grid should wrap text." )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public bool WrapText
		{
			get
			{
				return model.TableStyle.WrapText;
			}
			set
			{
				model.IgnoreReadOnly = true;
				model.TableStyle.WrapText = value;
				model.RowStyles[0].WrapText = value;
				model.IgnoreReadOnly = false;
			}
		}

		/// <summary>
		/// Gets or sets the font of the header.
		/// </summary>
		[Description( "Indicates the font of the header." )]
		[Category( "Appearance" )]
		public Font HeaderFont
		{
			get { return monthLabel.Font; }
			set
			{
				if( monthLabel.Font == value ) return;
				monthLabel.Font = value;
				yearLabel.Font = value;
				yearUD.Font = value;
				SetHeaderPositions();
				headerPanel.Invalidate();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHeaderFont()
		{
			return HeaderFont != c_defaultHeaderFont;
		}
		/// <summary>
		/// Resets the header font to Microsoft Sans Serif.
		/// </summary>
		protected void ResetHeaderFont()
		{
			HeaderFont = c_defaultHeaderFont;
		}

		/// <summary>
		/// Indicates whether the header gradient will be vertical.
		/// </summary>
		[Description( "Indicates if the header gradient will be vertical." )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		public bool HeaderVerticalGradient
		{
			get { return headerVerticalGradient; }
			set
			{
				if( headerVerticalGradient == value ) return;
				headerVerticalGradient = value;
				RefreshHeaderBrush();
				headerPanel.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the image of the header.
		/// </summary>
		[Description( "Indicates the image of the header." )]
		[Category( "Appearance" )]
		[DefaultValue( null )]
		public Image HeaderImage
		{
			get { return headImage; }
			set
			{
				if( headImage!=value )
				{
					headImage = value;
					headerPanel.BackgroundImage = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the header will show a gradient background.
		/// </summary>
		[Description( "Determines if the header will show a gradient background." )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		public bool HeadGradient
		{
			get { return headGradient; }
			set
			{
				if( headGradient!=value )
				{
					headGradient = value;
					headerPanel.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the end color of the header gradient.
		/// </summary>
		[Description( "Indicates the end color of the header gradient." )]
		[Category( "Appearance" )]
		public Color HeaderEndColor
		{
			get { return headerEndColor; }
			set
			{
				if( headerEndColor == value ) return;

				headerEndColor = value;
				RefreshHeaderBrush();

				if( headGradient )
					headerPanel.Invalidate();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHeaderEndColor()
		{
			return headerEndColor!=SystemColors.ControlDark;
		}
		/// <summary>
		/// Resets the end color of the header to SystemColors.ControlDark.
		/// </summary>
		protected void ResetHeaderEndColor()
		{
			headerEndColor = SystemColors.ControlDark;
		}
		/// <summary>
		/// Gets or sets the start color of the header gradient. Also used as backcolor.
		/// </summary>
		[Description( "Indicates the start color of the header gradient. Also used as backcolor." )]
		[Category( "Appearance" )]
		public Color HeaderStartColor
		{
			get { return headerStartColor; }
			set
			{
				if( headerStartColor == value ) return;
				headerStartColor = value;
				headerPanel.BackColor = value;
				RefreshHeaderBrush();

				if( headGradient )
					headerPanel.Invalidate();
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHeaderStartColor()
		{
			return headerStartColor!=SystemColors.ActiveCaption;
		}
		/// <summary>
		/// Resets the start color of the header to SystemColors.ActiveCaption.
		/// </summary>
		protected void ResetHeaderStartColor()
		{
			HeaderStartColor = SystemColors.ActiveCaption;
		}


		/// <summary>
		/// Gets or sets the font of the days.
		/// </summary>
		[Description( "Indicates the font of the days." )]
		[Category( "Appearance" )]
		public Font DaysFont
		{
			get
			{
				if( daysFont == null )
					daysFont = Syncfusion.Drawing.FontUtil.CreateFont( "Verdana", 8 );
				return daysFont;
			}
			set
			{
				if( daysFont == value )
					return;

				daysFont = value;

				if( grid != null )
				{
					UpdateDaysFont();
				}
			}
		}

		private void UpdateDaysFont()
		{
			if( this.daysFont != null )
			{
				GridFontInfo fontInfo = new GridFontInfo();

				fontInfo.Facename = daysFont.Name;
				fontInfo.Bold = daysFont.Bold;
				fontInfo.Italic = daysFont.Italic;
				fontInfo.Underline = daysFont.Underline;
				fontInfo.Size = daysFont.Size;
				fontInfo.Strikeout = daysFont.Strikeout;

				daysFontInfo = fontInfo;
				for( int i = 1; i <= 6; i++ )
				{
					for( int j = 1; j <= DEF_DAYS_COUNT; j++ )
					{
						GetInfo( i, j ).Font = fontInfo;
					}
				}

				grid.Model.IgnoreReadOnly = true;
				grid.Refresh();
				grid.Invalidate();
				grid.Model.IgnoreReadOnly = false;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDaysFont()
		{
			return DaysFont.FontFamily.Name != "Microsoft Sans Serif" || DaysFont.Size != 8;
		}

		/// <summary>
		/// Resets the days font to Microsoft Sans Serif.
		/// </summary>
		protected void ResetDaysFont()
		{
			DaysFont = Syncfusion.Drawing.FontUtil.CreateFont( "Microsoft Sans Serif", 8 );
		}

		/// <summary>
		/// Gets or sets the current value of the calendar.
		/// </summary>
		[Description( "Indicates the current value of the calendar." )]
		[Category( "Behavior" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Editor( typeof( DateTimeEditorAdv ), typeof( UITypeEditor ) )]
		public DateTime Value
		{
			get
			{
				return dvalue;
			}
			set
			{
				if( value < minValue || value > maxValue ) return;
				if( dvalue != value )
				{
					DateTime oldValue = dvalue;
					bool sameYear = CurrentCalendar.GetYear( dvalue ) == CurrentCalendar.GetYear( value );
					bool sameMonth = CurrentCalendar.GetMonth( dvalue ) == CurrentCalendar.GetMonth( value );
					bool ctl = ( ( Control.ModifierKeys & Keys.Control ) != 0 );
					if( !ctl && sameYear && sameMonth )
					{
						this.selectedDates = new DateTime[0];

						if( grid != null )
						{
							this.CurrentSelDates.Clear();
							grid.Refresh();
						}
					}
					dvalue = value;
					OnDateChanged( EventArgs.Empty );
					if( grid != null )
					{
						ValueChanged( sameYear && sameMonth );
					}
				}
			}
		}

        internal protected void GridFocus()
        {
            if (grid != null)
            {
                grid.GridControl.Focus();
            }
        }

		/// <summary>
		/// Gets or sets the culture of the calendar.
		/// </summary>
		[Description( "Indicates the culture of the calendar." )]
		[Category( "Appearance" )]
		[TypeConverter( typeof( SpecificCultureInfoTypeConverter ) )]
		public CultureInfo Culture
		{
			get { return culture; }
			set
			{
				if( culture != value )
				{
					culture = value;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

					DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                        new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );

					this.yearUD.Maximum = CurrentCalendar.GetYear( CurrentCalendar.MaxSupportedDateTime );
					this.yearUD.Minimum = CurrentCalendar.GetYear( CurrentCalendar.MinSupportedDateTime );

					this.yearUD.Value = datetime.Year;

#else
          // Do not look on this code please. It is done just to make it work.
          this.yearUD.Maximum = c_validMaxValue.Year; 
          this.yearUD.Minimum = c_validMinValue.Year; 
#endif
					if( grid != null )
					{
						RefreshNames();
						this.m_selectedDates.Clear();
						ValueChanged( true );
						SetHeaderPositions();
						headerPanel.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the border type of the text box control.
		/// </summary>
		[DefaultValue( BorderStyle.None )]
		[Category( "Appearance" )]
		[Description( "Gets or sets the border type of the text box control." )]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public BorderStyle BorderStyle
#else
		public new BorderStyle BorderStyle
#endif
		{
			get
			{
				return m_borderStyle;
			}
			set
			{
				if( m_borderStyle != value )
				{
					m_borderStyle = value;
                    if (Style == VisualStyle.Metro)
                    {
                        if(BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
                            this.bottomPanel.Padding = new System.Windows.Forms.Padding(2, 1, 1, 3);
                        else
                            this.bottomPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
                    }
					this.RecreateHandle();
					this.OnBorderStyleChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border sides for which you want the 3D border style applied.
		/// </summary>
		[DefaultValue( Border3DSide.All )]
		[Category( "Appearance" )]
		[Description( "Gets or sets the border sides for which you want the 3D border style applied." )]
		public Border3DSide BorderSides
		{
			get
			{
				return m_borderSides;
			}
			set
			{
				if( m_borderSides != value )
				{
					m_borderSides = value;

					this.OnBorderSidesChanged();

					if( grid != null )
						this.InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the 3D border style for the control.
		/// </summary>
		[DefaultValue( Border3DStyle.Sunken )]
		[Category( "Appearance" )]
		[Description( "Gets or sets the 3D border style for the control." )]
		public Border3DStyle Border3DStyle
		{
			get
			{
				return m_border3DStyle;
			}
			set
			{
				if( m_border3DStyle != value )
				{
					m_border3DStyle = value;

					this.OnBorder3DStyleChanged();

					if( grid != null )
						this.InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the single border color for the control.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the single border color for the control." )]
		public Color BorderColor
		{
			get
			{
				return m_borderColor;
			}
			set
			{
				if( m_borderColor != value )
				{
					m_borderColor = value;

					this.OnBorderColorChanged();

					if( grid != null )
						this.InvalidateWindow();
				}
			}
		}

		/// <summary>
		/// Gets or sets the draw themed border.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Appearance" )]
		[Description( "Gets or sets the draw themed border." )]
		public bool ThemedBorder
		{
			get
			{
				return m_bThemedBorder;
			}
			set
			{
				if( m_bThemedBorder != value )
				{
					m_bThemedBorder = value;

					this.OnThemedBorderChanged();

					if( grid != null )
						this.InvalidateWindow();
				}
			}
		}

		#endregion
		internal MonthCalendarAdv( bool dtCalendar )
			: this()
		{
			this.dateTimePickerCalendar = dtCalendar;
		}

		private BrushInfo daysHeaderInterior = new BrushInfo( GradientStyle.Vertical, Color.PeachPuff, Color.AntiqueWhite );
		/// <summary>
		/// Gets/sets the background color, gradient, etc. for the days header. 
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Category( "Appearance" ),
		Description( "Lets you set the background color, gradient, etc. for the days header." )
		]

		public BrushInfo DaysHeaderInterior
		{
			get { return daysHeaderInterior; }
			set
			{
				daysHeaderInterior = value;

				if( grid != null )
					grid.Refresh();
			}
		}

		private bool ShouldSerializeDaysHeaderInterior()
		{
			if( this.DaysHeaderInterior.GradientStyle == GradientStyle.Vertical &&
				this.DaysHeaderInterior.ForeColor == Color.PeachPuff &&
				this.DaysHeaderInterior.BackColor == Color.AntiqueWhite )
				return false;
			else
				return true;
		}

		private void ResetDaysHeaderInterior()
		{
			this.DaysHeaderInterior = new BrushInfo( GradientStyle.Vertical, Color.PeachPuff, Color.AntiqueWhite );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonthCalendarAdv"/> class.
		/// </summary>
		public MonthCalendarAdv()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( MonthCalendarAdv ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			m_themedEditDrawing = new ThemedEditDrawing( this );

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			InitializeMonthCalendarAdv();//Replaces InitializeComponent            

			daysHeaderInterior = new BrushInfo( GradientStyle.Vertical, Color.PeachPuff, Color.AntiqueWhite );

			SetStyle( ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true );

			m_selectedDates = new HybridDictionary();

			GridModel grModel = new GridModel();

			grModel.CommandStack.Enabled = false;
			grModel.Rows.DefaultSize = 17;
			grModel.Cols.DefaultSize = 65;
			grModel.RowHeights[0] = 21;
			grModel.ColWidths[0] = 35;
			grModel.RowHeights.ResetModified();
			grModel.ColWidths.ResetModified();
			grModel.Options.ExcelLikeCurrentCell = false;
			grModel.Options.ExcelLikeSelectionFrame = false;
			grModel.Options.AllowDragSelectedCols = false;
			grModel.Options.AllowDragSelectedRows = false;
			grModel.CommandStack.Enabled = false;
			grModel.Options.FloatCellsMode = GridFloatCellsMode.None;

			ResetBaseStylesMap( grModel );

			grModel.RowCount = 10;
			grModel.ColCount = 10;

			this.model = new CalendarModel( grModel );

			CreateVirtualCells();
            CTRLSIZE = this.Size;
            HDRHEIGHT = this.HeaderHeight;
            BTMHEIGHT = this.BottomHeight;
		}

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
		[DefaultValue(false)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * sf), (int)(CTRLSIZE.Height * sf));
            this.HeaderHeight = (int)(HDRHEIGHT * sf);
            this.BottomHeight = (int)(BTMHEIGHT * sf);
            isScaling = false;
            this.Invalidate();
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
        }
		private void ResetBaseStylesMap( GridModel model )
		{
			model.BaseStylesMap.RegisterStandardStyles();

			GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
			GridStyleInfo header = model.BaseStylesMap["Header"].StyleInfo;
			GridStyleInfo rowHeader = model.BaseStylesMap["Row Header"].StyleInfo;
			GridStyleInfo colHeader = model.BaseStylesMap["Column Header"].StyleInfo;

			GridFontInfo boldFont = new GridFontInfo();
			boldFont.Bold = true;
			boldFont.Size = 8;
			boldFont.Facename = "Verdana";

			header.Interior = new BrushInfo( GradientStyle.Vertical, Color.FromArgb( 203, 199, 184 ), Color.FromArgb( 238, 234, 216 ) );
			rowHeader.Interior = new BrushInfo( GradientStyle.Horizontal, Color.FromArgb( 203, 199, 184 ), Color.FromArgb( 238, 234, 216 ) );
			standard.Font.Facename = "Tahoma";

			model.BaseStylesMap.Modified = false;
		}

		/// <summary>
		/// Is control initializing at this time.
		/// </summary>
		private bool m_bIsInitializing = false;

		/// <summary>
		/// Gets is control initializing at this time.
		/// </summary>
		protected internal bool IsInitializing
		{
			get
			{
				return m_bIsInitializing;
			}
		}

		void ISupportInitialize.BeginInit()
		{
			m_bIsInitializing = true;
		}

		void ISupportInitialize.EndInit()
		{
			m_bIsInitializing = false;
            this.todayButton.Text = (this.TodayButton.Text == SR.GetString(SR.Today, this)) ? SR.GetString(SR.Today, this):this.TodayButton.Text;
            this.nullButton.Text = (this.nullButton.Text == SR.GetString(SR.None, this)) ? SR.GetString(SR.None, this) : this.NoneButton.Text;
			if( this.Style == VisualStyle.Office2007 && this.Office2007Theme == Office2007Theme.Managed )
			{
				ApplyOffice2007Theme();
			}
            else if (this.Style == VisualStyle.Office2010 && this.Office2010Theme == Office2010Theme.Managed)
            {
                ApplyOffice2010Theme();
            }
            else if (this.Style == VisualStyle.Metro)
            {
                ApplyMetroTheme();
            }
			SetHeaderPositions();
		}

		/// <summary>
		/// Contains map of month to GridRangeInfoList.
		/// </summary>
		private HybridDictionary m_selectedDates;

		private bool SelectCell( int rowIndex, int colIndex, bool multiSelect )
		{
			DateTime date = dvalue;
			int cellNo = ( rowIndex - 1 ) * DEF_DAYS_COUNT + colIndex;

			if( cellNo < prevMonthCell )
			{
				date = dvalue.AddMonths( -1 );
			}
			else if( cellNo > nextMonthCell )
			{
				date = dvalue.AddMonths( 1 );
			}
			date = new DateTime( date.Year, date.Month, this.GetDayFromCell( rowIndex, colIndex ) );

			bool isDateValid = date <= this.MaxValue && date >= this.MinValue;
			if( isDateValid )
			{
				if( !multiSelect )
				{
					m_selectedDates.Clear();
				}

				GridRangeInfo cellRange = GridRangeInfoAdv.FromGridRangeInfo( GridRangeInfo.Cell( rowIndex, colIndex ) );

				SetRangeDates( cellRange );

				this.CurrentSelDates.Add( cellRange );
			}
			return isDateValid;
		}

		private void CreateGrid()
		{
			grid = new CalendarGrid();

            if(this.grid.GridControl.WantEnterKey != this.WantEnterKey)
                this.grid.GridControl.WantEnterKey = this.WantEnterKey;
		}
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
            base.OnHandleDestroyed(e);
        }
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );
            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
			if( this.RecreatingHandle )
				return;

			InitializeMenu();
			InitializeGrid( ref grid );
			SetHeaderPositions();

			RefreshCalendar( true );
			RecalculateWidth();

			grid.Invalidate();
		}

		/// <summary>
		/// Initializes the grid.
		/// </summary>
		/// <param name="grid">The CalendarGrid.</param>
		protected virtual void InitializeGrid( ref CalendarGrid grid )
		{
			if( grid == null )
				this.CreateGrid();

			model.RowCount = DEF_ROW_COUNT;
			model.ColCount = DEF_DAYS_COUNT;

			model.Cols.Hidden[0] = true;

			model.Options.AllowSelection = GridSelectionFlags.None;
			model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.None;
			model.Options.ControllerOptions = GridControllerOptions.None;
			model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;

			GridStyleInfo info = new GridStyleInfo();
			info.CellType = "Static";

			model.ChangeCells( GridRangeInfo.Cells( 1, 1, 6, DEF_DAYS_COUNT /*+ WeekColumnOffset*/), info );

			model.Rows.FrozenCount = DEF_ROW_COUNT;
			model.Cols.FrozenCount = DEF_DAYS_COUNT + WeekColumnOffset;

			model.TableStyle.HorizontalAlignment = GridHorizontalAlignment.Center;

			model.SelectionChanged += new GridSelectionChangedEventHandler( GridSelectionChanged );
			model.QueryRowCount += new GridRowColCountEventHandler( QueryRowCount );
			model.QueryColCount += new GridRowColCountEventHandler( QueryColCount );
			model.QueryCellInfo += new GridQueryCellInfoEventHandler( QueryCellInfo );

			grid.Model = model;

			grid.HScrollBehavior = GridScrollbarMode.Disabled;
			grid.VScrollBehavior = GridScrollbarMode.Disabled;

			grid.MouseControllerDispatcher.Add( new GridSelectCellsMouseController( grid.GridControl ) );

			grid.ContextMenu = this.Menu;

			grid.CellDrawn += new GridDrawCellEventHandler( grid_CellDrawn );
			grid.CellClick += new GridCellClickEventHandler( CellClick );
			grid.CellMouseDown += new GridCellMouseEventHandler( grid_CellMouseDown );
			grid.CurrentCellMoving += new GridCurrentCellMovingEventHandler(CurrentCellMoving);
			grid.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			grid.Click += new EventHandler( this.child_Click );

			grid.GridControl.CellMouseMove += new GridCellMouseEventHandler(grid_CellMouseMove);
			grid.GridControl.CellMouseUp += new GridCellMouseEventHandler( grid_CellMouseUp );

			grid.MoveCurrentCellDirection += new GridMoveCurrentCellDirectionEventHandler(grid_MoveCurrentCellDirection);

			Controls.Add( grid.GridControl );

			grid.BringToFront();
			grid.Dock = DockStyle.Fill;
			grid.Model.ReadOnly = true;

			grid.ForceCurrentCellMoveTo = true;
			grid.CurrentCell.MoveTo( 1, 1, GridSetCurrentCellOptions.SetFocus );
			grid.CurrentCell.Activate( 1, 1, GridSetCurrentCellOptions.SetFocus );

			grid.ThemesEnabled = m_bThemedEnabledGrid;
			grid.BackColor = m_gridBackColor;

			if( this.Style != VisualStyle.Default )
			{
				this.DaysHeaderInterior = new BrushInfo( grid.BackColor );
			}

			grid.BackgroundImage = base.BackgroundImage;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			grid.BackgroundImageLayout = base.BackgroundImageLayout;
#endif
			UpdateDaysFont();

			MonthCalendarAdv_SizeChanged( this, EventArgs.Empty );
			RefreshNames();
            ValueChanged(true);
		}

		#region Event handlers

		private void grid_CellDrawn( object sender, GridDrawCellEventArgs e )
		{
            highlightColor = Color.Black;
            
            if (this.dvalue.Year == DateTime.Now.Year && dvalue.Month == DateTime.Now.Month)
            {
                int day = this.GetDayFromCell(e.RowIndex, e.ColIndex);
                int dayCurrent = CurrentCalendar.GetDayOfMonth(DateTime.Now);
                if (day == dayCurrent && e.RowIndex !=0)
                {
                    if ((day <= 7 && e.RowIndex < 5) || (day > 20 && e.RowIndex > 1))
                    {
                        Brush br = new SolidBrush(Color.FromArgb(216, 216, 217));
                        e.Graphics.FillRectangle(br, e.Bounds);
                        Brush brush = new SolidBrush(highlightColor);
                        Pen pen = new Pen(MetroColor, 1);
                        e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                        PointF size = e.Graphics.MeasureString(e.Style.CellValue.ToString(), monthLabel.Font).ToPointF();
                        e.Graphics.DrawString(e.Style.CellValue.ToString(), monthLabel.Font, brush, (e.Bounds.X + (e.Bounds.Width / 2) - (size.X / 2)), (e.Bounds.Y + (e.Bounds.Height / 2) - (size.Y / 2)));
                        brush.Dispose();
                        br.Dispose();
                    }
                }
            }
            
            if (this.BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
            {
                e.Graphics.DrawLine(new Pen(this.BorderColor, 1), new Point(0, 0), new Point(0, this.grid.GridControl.Height));
                e.Graphics.DrawLine(new Pen(this.BorderColor, 1), new Point(this.grid.GridControl.Width - 1, 0), new Point(this.grid.GridControl.Width - 1, this.grid.GridControl.Height));
            }
            if (!same_Month)
                ClearSelection();
			if( this.CurrentSelDates.AnyRangeContains( GridRangeInfo.Cell( e.RowIndex, e.ColIndex ) ) && e.Style.Enabled )
            {
                if (Style == VisualStyle.Metro)
                {
                    using (Brush br = new SolidBrush(Color.FromArgb(100,216, 216, 217)))
                    {
                        highlightColor = Color.White;
                        Brush brush = new SolidBrush(highlightColor);
                        e.Graphics.FillRectangle(br, e.Bounds);
                        PointF size = e.Graphics.MeasureString(e.Style.CellValue.ToString(), monthLabel.Font).ToPointF();
                        //e.Graphics.DrawString(e.Style.CellValue.ToString(), monthLabel.Font, brush, (e.Bounds.X + (e.Bounds.Width / 2) - (size.X / 2)), (e.Bounds.Y + (e.Bounds.Height / 2) - (size.Y / 2)));
                        brush.Dispose();
                    }
                }
                else
                {
                    using (Brush br = new SolidBrush(this.grid.Model.Options.AlphaBlendSelectionColor))
                    {
                        e.Graphics.FillRectangle(br, e.Bounds);
                    }
                }
			}
		}

		private void grid_CellMouseMove( object sender, GridCellMouseEventArgs e )
		{
			if( this.MouseDragMultiselect && m_ptMouseDragMultiselectFirtsRowCol != Point.Empty )
			{
				int col, row;
				grid.GridControl.PointToRowCol( new Point( e.MouseEventArgs.X, e.MouseEventArgs.Y ), out row, out col );
				if (this.ShowWeekNumbers)
					col -= WeekColumnOffset;
				Point pt = new Point( row, col );
				if( col > 0 && row > 0 && m_ptMouseDragMultiselectLastRowCol != pt )
				{
					m_ptMouseDragMultiselectLastRowCol = pt;
					ProccesMouseDragMultiselect( row, col );
				}
			}
		}

		private void grid_CellMouseUp( object sender, GridCellMouseEventArgs e )
		{
			if( m_ptMouseDragMultiselectFirtsRowCol != m_ptMouseDragMultiselectLastRowCol )
			{
				m_bSkipCheckCell = true;
			}
			m_ptMouseDragMultiselectFirtsRowCol = Point.Empty;
			m_ptMouseDragMultiselectLastRowCol = Point.Empty;
		}

		private void grid_MoveCurrentCellDirection(object sender, GridMoveCurrentCellDirectionEventArgs e)
		{
			switch (e.Direction)
			{
				case GridDirectionType.Left:
					this.Value -= new TimeSpan(1, 0, 0, 0, 0);
					break;
				case GridDirectionType.Right:
					this.Value += new TimeSpan(1,0,0,0,0);
					break;
				case GridDirectionType.Up:
					this.Value -= new TimeSpan(7, 0, 0, 0, 0);
					break;
				case GridDirectionType.Down:
					this.Value += new TimeSpan(7, 0, 0, 0, 0);
					break;
				default:
					return;
			}

			OnSelectionChanged(EventArgs.Empty);

			e.Handled = true;
			e.Result = true;
		}

		#endregion

		private void ProccesMouseDragMultiselect( int row, int col )
		{
			this.CurrentSelDates.Clear();

			Point pt1 = m_ptMouseDragMultiselectFirtsRowCol;
			Point pt2 = new Point( row, col );

			bool invertSelection;

			if( pt1.X == pt2.X )
			{
				invertSelection = ( pt1.Y > pt2.Y );
			}
			else
			{
				invertSelection = ( pt1.X > pt2.X );
			}

			if( invertSelection )
			{
				for( int i = pt1.X; i >= pt2.X; i-- )
				{
					int selectCount = ( ( i == pt2.X ) ? pt2.Y : 1 );
					int selectStart = ( ( i == pt1.X ) ? pt1.Y : 7 );

					for( int j = selectStart; j >= selectCount; j-- )
					{
						this.SelectCell( i, j, true );
					}
				}
			}
			else
			{
				for( int i = pt1.X; i <= pt2.X; i++ )
				{
					int selectEnd = ( ( i == pt2.X ) ? pt2.Y : 7 );
					int selectStart = ( ( i == pt1.X ) ? pt1.Y : 1 );

					for( int j = selectStart; j <= selectEnd; j++ )
					{
						this.SelectCell( i, j, true );
					}
				}
			}

			this.grid.Invalidate();

			this.OnSelectionChanged( EventArgs.Empty );
		}

		private void ToggleSelectRowCol( int row, int col )
		{
			if( !this.grid.Model[row, col].Enabled ) return;
			bool ctl = ( ( Control.ModifierKeys & Keys.Control ) != 0 );
			GridRangeInfo cellRange = GridRangeInfoAdv.FromGridRangeInfo( GridRangeInfo.Cell( row, col ) );
			bool cellSelectedAlready = this.CurrentSelDates.AnyRangeContains( cellRange );
            if (this.AllowMultipleSelection)
            {
                if (ctl && cellSelectedAlready)
                {
                    this.CurrentSelDates.Remove(cellRange);
                }
                else if (ctl && !cellSelectedAlready)
                {
                    this.SelectCell(row, col, this.allowMultipleSelection);
                }
                else if (!ctl)
                {
                    GridRangeInfoList cellsToRefresh = this.CurrentSelDates.Clone();

                    if (SelectCell(row, col, false))
                    {
                        cellsToRefresh.Add(cellRange);
                    }
                    foreach (GridRangeInfo range in cellsToRefresh)
                    {
                        this.grid.RefreshRange(range, true);
                    }
                }
            }
            else if (ctl)
            {                        
                    this.CurrentSelDates.Clear();
                    this.grid.Refresh();               
            }
			this.OnSelectionChanged( EventArgs.Empty );
		}

		private void InitializeMenu()
		{
			this.parentBarItem1.Items.Clear();
			for( int i = 0; i < 12; i++ )
			{
				BarItem item = new BarItem( months[i + 1], new EventHandler( ItemClick ) );
				item.ImageIndex = i;
				item.ImageList = this.parentBarItem1.ImageList;
				this.parentBarItem1.Items.Add( item );
			}
		}

		private void ItemClick( object sender, EventArgs e )
		{
			int iNewMonth = this.parentBarItem1.Items.IndexOf( sender ) + 1;

			if( iNewMonth > 0 )
			{
				DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                    new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );

				int days = CurrentCalendar.GetDaysInMonth( datetime.Year, datetime.Month );
				int daysNew = CurrentCalendar.GetDaysInMonth( datetime.Year, iNewMonth );

				if( datetime.Day > daysNew )
				{
					datetime.Day = daysNew;
				}

				datetime.Month = iNewMonth;

				Value = datetime.DateTime;
			}
		}

		/// <summary>
		/// Returns the current style for a cell.
		/// </summary>
		/// <param name="i">The row index of the cell.</param>
		/// <param name="j">The column index of the cell.</param>
		/// <returns>The style of the cell.</returns>
		public GridStyleInfo GetInfo( int i, int j )
		{
			GridStyleInfo gsi = null;
			if( virtualCells.Count > i )
			{
				if( ( (ArrayList)virtualCells[i] ).Count > j )
					gsi = ( (GridStyleInfo)( (ArrayList)virtualCells[i] )[j] );
			}

			return gsi;
		}

		protected GridRangeInfoList CurrentSelDates
		{
			get
			{
				GridRangeInfoList dates;

				int m = CurrentCalendar.GetMonth( dvalue );
				int y = CurrentCalendar.GetYear( dvalue );

				DateTime date = new DateTime(y, m, 1, CurrentCalendar);

				if( !this.m_selectedDates.Contains( date ) )
				{
					dates = new GridRangeInfoList();
					this.m_selectedDates.Add( date, dates );
				}
				else
				{
					dates = (GridRangeInfoList)this.m_selectedDates[date];
				}

				return dates;
			}
		}

		/// <summary>
		/// Sets the GridStyleInfo for the cell.
		/// </summary>
		/// <param name="i">The row index of the cell.</param>
		/// <param name="j">The column index of the cell.</param>
		/// <param name="style">The new style of the cell.</param>
		public void SetInfo( int i, int j, GridStyleInfo style )
		{
			( (ArrayList)virtualCells[i] )[j] = style;
		}

		/// <summary>
		/// Clears selected dates.
		/// </summary>
		public void ClearSelection()
		{
			this.SelectCell( this.grid.CurrentCell.RowIndex, this.grid.CurrentCell.ColIndex, false );

			if( grid != null )
			{
				this.grid.Invalidate();
			}
		}

		protected internal void CorrectCurrentSelectedCell()
		{
			// Move Current cell to last selected date cell,
			// so selected date becomes correctly highlited
			DateTimePickerAdv.CultureSpecifiecDateTime datetime =
				new DateTimePickerAdv.CultureSpecifiecDateTime( Value, CurrentCalendar );

			int currentDay = datetime.Day;

			for( int i = 1, rowCount = grid.Model.RowCount; i <= rowCount; i++ )
			{
				for( int j = 1, colCount = grid.Model.ColCount; j <= colCount; j++ )
				{
					int cellNo = ( i - 1 ) * DEF_DAYS_COUNT + ( j - 1 );

					int day = this.GetDayFromCell( i, j );

					// if date in this cell is date from selected month, 
					// move current cell to it.
					if( day == currentDay && cellNo >= prevMonthCell - 1 && cellNo <= nextMonthCell )
					{
						grid.CurrentCell.MoveTo( i, j );
						return;
					}
				}
			}
		}
		/// <summary>
		/// Raises the DateCellQueryInfo event.
		/// </summary>
		/// <param name="args">The <see cref="Syncfusion.Windows.Forms.Tools.DateCellQueryInfoEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		protected virtual bool OnDateCellQueryInfo( DateCellQueryInfoEventArgs args )
		{
			if( this.DateCellQueryInfo != null )
				DateCellQueryInfo( this, args );

			if( args.Handled )
			{
				return true;
			}
			else
			{
				return false;
			}
			//			return args.Handled;
		}

		/// <summary>
		/// Fires InvalidDateSelected event.
		/// </summary>
		protected virtual void RaiseInvalidDateSelected()
		{
			if( InvalidDateSelected == null ) return;

			InvalidDateSelected( this, EventArgs.Empty );
		}

		/// <summary>
		/// Refreshes the calendar.
		/// </summary>
		public void RefreshCalendar()
		{
			RefreshCalendar( this.needsRefresh );
		}

		/// <summary>
		/// Refreshes the calendar if set to true.
		/// </summary>
		/// <param name="forceRefresh">if set to <c>true</c> refreshes the calendar.</param>
		public void RefreshCalendar( bool forceRefresh )
		{
			if( forceRefresh && this.grid != null )
			{
				this.grid.Refresh();
				this.needsRefresh = false;
			}
		}

		private void QueryCellInfo( object sender, GridQueryCellInfoEventArgs e )
		{
			if( e.ColIndex == DEF_WEEK_COLUMN_INDEX && m_bShowWeekNumbers )
			{
				SetWeek( e );
				e.Handled = true;
			}
			else if( e.RowIndex >= 0 && e.ColIndex >= 0 )
			{
				int cellNo = ( e.RowIndex - 1 ) * DEF_DAYS_COUNT + e.ColIndex;
				GridStyleInfo info = null;
				info = GetInfo( e.RowIndex, e.ColIndex );
				e.Style.Text = info.Text;
				e.Style.Font = daysFontInfo;
				e.Style.HorizontalAlignment = horizontalAlignment;
				e.Style.VerticalAlignment = verticalAlignment;
				e.Style.TextColor = info.TextColor;
				if( e.RowIndex == 0 )
				{
					e.Style.Text = days[e.ColIndex];
					e.Style.Font = dayNamesFontInfo;
					e.Style.TextColor = dayNamesColor;
					e.Style.WrapText = this.WrapText;
					e.Style.Interior = daysHeaderInterior;

					if( style == VisualStyle.Default )
					{
						e.Style.BaseStyle = "Header";
					}
					else
					{
						e.Style.BaseStyle = "Static";
                        if(Style == VisualStyle.Metro)
						    e.Style.Borders.Bottom = new GridBorder( GridBorderStyle.None );
                        else
                            e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid);
					}
				}

				if( e.RowIndex != 0 && e.ColIndex != 0 )
				{
					bool isCurrentCell = false;
					bool isOutsideRange = false;
					object dateValue = null;
					CalendarCurrentCell currCell = grid.CurrentCell;
					int row = currCell.RowIndex, col = currCell.ColIndex;

					if( e.RowIndex == row && e.ColIndex == col )
					{
						isCurrentCell = true;
					}

					if( ( cellNo<prevMonthCell || cellNo>nextMonthCell ) )
					{
						isOutsideRange = true;
					}

					try
					{
						if( info.Text != null && info.Text != String.Empty && this.IsNumber( info.Text ) )
						{
							int day = this.GetDayFromCell( e.RowIndex, e.ColIndex );
							DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                                new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue.Date, CurrentCalendar );

							int days = CurrentCalendar.GetDaysInMonth( datetime.Year, datetime.Month );

							if( cellNo<prevMonthCell )
							{
								datetime.Day = 1;
								datetime.DateTime = CurrentCalendar.AddMonths( datetime.DateTime, -1 );
								datetime.Day = day;
							}
							else if( cellNo > nextMonthCell )
							{
								datetime.Day = 1;
								datetime.DateTime = CurrentCalendar.AddMonths( datetime.DateTime, 1 );
								datetime.Day = day;
							}
							else if( day <= days )
							{
								datetime.Day = day;
							}

							dateValue = datetime.DateTime;
						}

					}
					catch { }

					DateCellQueryInfoEventArgs args = new DateCellQueryInfoEventArgs( e.RowIndex, e.ColIndex, e.Style/*info*/, dateValue, isCurrentCell, isOutsideRange );

					if( ( cellNo < prevMonthCell || cellNo > nextMonthCell ) )
					{
						e.Style.TextColor = inactiveMonthColor;
					}
					else if( this.dvalue.Year == DateTime.Now.Year && dvalue.Month == DateTime.Now.Month )
					{
						try
						{
							int day = this.GetDayFromCell( e.RowIndex, e.ColIndex );
							int dayCurrent = CurrentCalendar.GetDayOfMonth( DateTime.Now );

							if( day == dayCurrent )
							{
                                if (this.Style == VisualStyle.Office2010)
                                    e.Style.TextColor = Color.Red;
                                else if (this.Style == VisualStyle.Metro)
                                {
                                    e.Style.TextColor = SystemColors.ControlText;
                                }
                                else
                                    e.Style.TextColor = todayFontColor;
							}
						}
						catch { }
					}

					if( isCurrentCell )
					{
						e.Style.TextColor = highlightColor;
					}

					e.Style.CellType = info.CellType;
					e.Style.Interior = BrushInfo.Empty;
					e.Style.Enabled = true;

					if( this.OnDateCellQueryInfo( args ) )
					{
						this.needsRefresh = true;
					}

					if( dateValue != null && ( (DateTime)dateValue < this.minValue || (DateTime)dateValue > this.MaxValue ) )
					{
						e.Style.Enabled = false;
					}

					if( isCurrentCell && e.Style.Enabled )
					{
						GridBordersInfo borders = e.Style.Borders;
						GridBorder top = borders.Top;

						borders.All = new GridBorder( GridBorderStyle.None );
					}
				}
				e.Handled = true;
			}
		}
		private void QueryRowCount( object sender, GridRowColCountEventArgs e )
		{
			e.Count = 6;
			e.Handled = true;
		}
		private void QueryColCount( object sender, GridRowColCountEventArgs e )
		{
			e.Count = DEF_DAYS_COUNT;
			e.Handled = true;
		}
		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                headerBrush.Dispose();
				if( grid != null )
				{
					this.grid.GridControl.CellMouseMove -= new GridCellMouseEventHandler( grid_CellMouseMove );
					this.grid.GridControl.CellMouseUp -= new GridCellMouseEventHandler( grid_CellMouseUp );
				}

				if( this.nullButton != null )
				{
					this.nullButton.Click -= new System.EventHandler( this.nullButton_Click );
					this.nullButton.MouseDown -= new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
					this.nullButton.Click -= new EventHandler( this.child_Click );
					this.nullButton.Dispose();
					this.nullButton = null;
				}

				if( this.todayButton != null )
				{
					this.todayButton.Click -= new System.EventHandler( this.todayButton_Click );
					this.todayButton.MouseDown -= new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
					this.todayButton.Click -= new EventHandler( this.child_Click );
					this.todayButton.Dispose();
					this.todayButton = null;
				}

				if( null != m_themedEditDrawing )
				{
					m_themedEditDrawing.Dispose();
					m_themedEditDrawing = null;
				}

                Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);

				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		private void InitializeMonthCalendarAdv()
		{
			this.components = new System.ComponentModel.Container();
			this.headerPanel = new System.Windows.Forms.Panel();
			this.rightButton = new Syncfusion.Windows.Forms.ThemedScrollButton();
			this.leftButton = new Syncfusion.Windows.Forms.ThemedScrollButton();
			this.yearLabel = new System.Windows.Forms.Label();
			this.yearUD = new Syncfusion.Windows.Forms.Tools.YearNumericUpDown();
			this.monthLabel = new System.Windows.Forms.Label();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.nullButton = new MonthCalendarNoneButton();
			this.todayButton = new MonthCalendarTodayButton();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.gotoToday = new System.Windows.Forms.MenuItem();
			this.parentBarItem1 = new Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem();
			this.headerPanel.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)( this.yearUD ) ).BeginInit();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// headerPanel
			// 
			this.headerPanel.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.headerPanel.Controls.AddRange( new System.Windows.Forms.Control[] {
																					  this.rightButton,
																					  this.leftButton,
																					  this.yearLabel,
																					  this.yearUD,
																					  this.monthLabel} );
			this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size( 192, 32 );
			this.headerPanel.TabIndex = 0;
			this.headerPanel.Paint += new System.Windows.Forms.PaintEventHandler( this.headerPanel_Paint );
			this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.headerPanel_MouseDown );
			this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.headerPanel.MouseEnter += new EventHandler( headerPanel_MouseEnter );
			this.headerPanel.MouseLeave += new EventHandler( headerPanel_MouseLeave );
			this.headerPanel.Click += new EventHandler( this.child_Click );
			// 
			// rightButton
			// 
			this.rightButton.Anchor = ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right );
			this.rightButton.BackColor = System.Drawing.SystemColors.Control;
			this.rightButton.ButtonType = System.Windows.Forms.ScrollButton.Right;
			this.rightButton.DefaultButtonState = System.Windows.Forms.ButtonState.Flat;
			this.rightButton.Location = new System.Drawing.Point( 168, 8 );
			this.rightButton.Name = "rightButton";
			this.rightButton.Size = new System.Drawing.Size( 16, 18 );
			this.rightButton.TabIndex = 4;
			this.rightButton.Text = ">";
			this.rightButton.TabStop = false;
			this.rightButton.ThemesEnabled = true;
			this.rightButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.rightButton_MouseDown );
			this.rightButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.rightButton.MouseEnter += new EventHandler( rightButton_MouseEnter );
			this.rightButton.MouseLeave += new EventHandler( rightButton_MouseLeave );
			this.rightButton.Click += new EventHandler( this.child_Click );
			// 
			// leftButton
			// 
			this.leftButton.BackColor = System.Drawing.SystemColors.Control;
			this.leftButton.ButtonType = System.Windows.Forms.ScrollButton.Left;
			this.leftButton.DefaultButtonState = System.Windows.Forms.ButtonState.Flat;
			this.leftButton.Location = new System.Drawing.Point( 8, 8 );
			this.leftButton.Name = "leftButton";
			this.leftButton.Size = new System.Drawing.Size( 17, 19 );
			this.leftButton.TabIndex = 3;
			this.leftButton.Text = "<";
			this.leftButton.TabStop = false;
			this.leftButton.ThemesEnabled = true;
			this.leftButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.leftButton_MouseDown );
			this.leftButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.leftButton.MouseEnter += new EventHandler( rightButton_MouseEnter );
			this.leftButton.MouseLeave += new EventHandler( rightButton_MouseLeave );
			this.leftButton.Click += new EventHandler(this.child_Click);
			// 
			// yearLabel
			// 
			this.yearLabel.BackColor = System.Drawing.Color.Transparent;
			this.yearLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
			this.yearLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.yearLabel.Location = new System.Drawing.Point( 96, 8 );
			this.yearLabel.Name = "yearLabel";
			this.yearLabel.Size = new System.Drawing.Size( 45, 16 );
			this.yearLabel.TabIndex = 2;
			this.yearLabel.Text = "";
			this.yearLabel.Visible = false;
			this.yearLabel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.yearLabel_MouseDown );
			this.yearLabel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.yearLabel.Click += new EventHandler( this.child_Click );
			// 
			// yearUD
			// 
			this.yearUD.Location = new System.Drawing.Point( 96, 8 );
			this.yearUD.Maximum = new System.Decimal( new int[] {
																   c_validMaxValue.Year,
																   0,
																   0,
																   0} );
			this.yearUD.Minimum = new System.Decimal( new int[] {
																   c_validMinValue.Year,
																   0,
																   0,
																   0} );
			this.yearUD.Name = "yearUD";
			this.yearUD.Size = new System.Drawing.Size( 48, 20 );
			this.yearUD.TabIndex = 1;
			this.yearUD.ThemesEnabled = true;
			this.yearUD.Value = new System.Decimal( new int[] {
																 2003,
																 0,
																 0,
																 0} );
			this.yearUD.Visible = false;
			this.yearUD.VisibleChanged += new System.EventHandler( this.yearUD_VisibleChanged );
			this.yearUD.Leave += new System.EventHandler( this.yearUD_Leave );
			this.yearUD.ValueChanged += new System.EventHandler( this.yearUD_ValueChanged );
			this.yearUD.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.yearUD.Click += new EventHandler( this.child_Click );
			// 
			// monthLabel
			// 
			this.monthLabel.BackColor = System.Drawing.Color.Transparent;
			this.monthLabel.Font = new System.Drawing.Font( "Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
			this.monthLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.monthLabel.Location = new System.Drawing.Point( 32, 11 );
			this.monthLabel.Name = "monthLabel";
			this.monthLabel.Size = new System.Drawing.Size( 48, 16 );
			this.monthLabel.TabIndex = 0;
			this.monthLabel.Text = "January";
			this.monthLabel.Visible = false;
			this.monthLabel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.monthLabel_MouseDown );
			this.monthLabel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.monthLabel.Click += new EventHandler( this.child_Click );
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.AddRange( new System.Windows.Forms.Control[] {
																					  this.nullButton,
																					  this.todayButton} );
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point( 0, 148 );
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size( 192, 20 );
			this.bottomPanel.TabIndex = 1;
			this.bottomPanel.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.bottomPanel.Click += new EventHandler( this.child_Click );
			this.bottomPanel.VisibleChanged += new EventHandler( bottomPanel_VisibleChanged );
			// 
			// nullButton
			// 
			this.nullButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.nullButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.nullButton.Location = new System.Drawing.Point( 120, 0 );
			this.nullButton.Name = "nullButton";
			this.nullButton.Size = new System.Drawing.Size( 72, 20 );
			this.nullButton.TabIndex = 1;
			this.nullButton.Text = SR.GetString(culture, SR.None, this);
			this.nullButton.Click += new System.EventHandler( this.nullButton_Click );
			this.nullButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.nullButton.Click += new EventHandler( this.child_Click );
			// 
			// todayButton
			// 
			//			this.todayButton.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			//				| System.Windows.Forms.AnchorStyles.Left) 
			//				| System.Windows.Forms.AnchorStyles.Right);
			this.todayButton.Dock = DockStyle.Fill;
			this.todayButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.todayButton.Name = "todayButton";
			this.todayButton.Size = new System.Drawing.Size( 120, 20 );
			this.todayButton.TabIndex = 0;
            this.todayButton.Text = SR.GetString(culture, SR.Today, this);
			this.todayButton.Click += new System.EventHandler( this.todayButton_Click );
			this.todayButton.BringToFront();
			this.todayButton.MouseDown += new System.Windows.Forms.MouseEventHandler( this.child_MouseDown );
			this.todayButton.Click += new EventHandler( this.child_Click );
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange( new System.Windows.Forms.MenuItem[] {
																						this.gotoToday} );
			// 
			// gotoToday
			// 
			this.gotoToday.Index = 0;
			this.gotoToday.Text = "Go to Today";
			this.gotoToday.Click += new System.EventHandler( this.gotoToday_Click );
			// 
			// parentBarItem1
			// 
			this.parentBarItem1.CategoryIndex = -1;
			// 
			// MonthCalendarAdv
			// 
			this.Controls.AddRange( new System.Windows.Forms.Control[] {
																		  this.bottomPanel,
																		  this.headerPanel} );
            this.bottomPanel.Paint += new PaintEventHandler(bottomPanel_Paint);
			this.Name = "MonthCalendarAdv";
			this.Size = new System.Drawing.Size( 192, 168 );
			this.SizeChanged += new System.EventHandler( this.MonthCalendarAdv_SizeChanged );
			this.headerPanel.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)( this.yearUD ) ).EndInit();
			this.bottomPanel.ResumeLayout( false );
			this.ResumeLayout( false );
		}

        void bottomPanel_Paint(object sender, PaintEventArgs e)
        {
            if(this.BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
                e.Graphics.DrawRectangle(new Pen(this.BorderColor, 2), new Rectangle(0,-1,this.bottomPanel.Width,this.bottomPanel.Height-2));            
        }
		private void CreateVirtualCells()
		{
			if( virtualCells == null )
			{
				virtualCells = new ArrayList();

				for( int i = 0; i <= 6; i++ )
				{
					virtualCells.Add( new ArrayList() );
					for( int j = 0; j <= DEF_DAYS_COUNT; j++ )
					{
						GridStyleInfo info = new GridStyleInfo();
						if( i == 0 )
							info.CellType = "Header";
						else
							info.CellType = "Static";
						( (ArrayList)virtualCells[i] ).Add( info );
					}
				}
			}
		}

		private void InitMonthsPopup()
		{
			this.popupMonths = new Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu();
			this.popupMonths.ParentBarItem = this.parentBarItem1;
		}

		private void rightButton_MouseEnter( object sender, EventArgs e )
		{
			ThemedScrollButton button = sender as ThemedScrollButton;

			if( this.Style == VisualStyle.Office2007 )
			{
				button.ArrowColor = m_office2007ColorTable.DataTimePickerHighLightedForeColor;
				button.Invalidate();
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                button.ArrowColor = m_office2010ColorTable.DataTimePickerHighLightedForeColor;
                button.Invalidate();
            }
            else if (this.style == VisualStyle.Metro)
            {
                b_mousehover = true;
                if (button.Text == "<")
                {
                    b_leftbutton = true;
                    b_rightbutton = false;
                }
                else if (button.Text == ">")
                {
                    b_leftbutton = false;
                    b_rightbutton = true;
                }
            }
		}

		private void rightButton_MouseLeave( object sender, EventArgs e )
		{
			ThemedScrollButton button = sender as ThemedScrollButton;

			if( this.Style == VisualStyle.Office2007 )
			{
				button.ArrowColor = m_office2007ColorTable.MonthCalendarForeColor;
				button.Invalidate();
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                button.ArrowColor = m_office2010ColorTable.MonthCalendarForeColor;
                button.Invalidate();
            }
            else if (style == VisualStyle.Metro)
            {
                b_mousehover = false;
            }
		}

		private void headerPanel_MouseEnter( object sender, EventArgs e )
		{
			if( this.Style == VisualStyle.Office2007 )
			{
				this.yearLabel.ForeColor = m_office2007ColorTable.DataTimePickerHighLightedForeColor;
				this.monthLabel.ForeColor = m_office2007ColorTable.DataTimePickerHighLightedForeColor;
				this.headerPanel.Invalidate( true );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                this.yearLabel.ForeColor = m_office2010ColorTable.DataTimePickerHighLightedForeColor;
                this.monthLabel.ForeColor = m_office2010ColorTable.DataTimePickerHighLightedForeColor;
                this.headerPanel.Invalidate(true);
            }
		}

		private void headerPanel_MouseLeave( object sender, EventArgs e )
		{
			if( this.Style == VisualStyle.Office2007 )
			{
				this.yearLabel.ForeColor = m_office2007ColorTable.MonthCalendarForeColor;
				this.monthLabel.ForeColor = m_office2007ColorTable.MonthCalendarForeColor;
				this.headerPanel.Invalidate( true );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                this.yearLabel.ForeColor = m_office2010ColorTable.MonthCalendarForeColor;
                this.monthLabel.ForeColor = m_office2010ColorTable.MonthCalendarForeColor;
                this.headerPanel.Invalidate(true);
            }
            Refresh();
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		private void child_Click( object sender, EventArgs e )
		{
			this.OnClick( e );
		}

		private void child_MouseDown( object sender, MouseEventArgs e )
		{
			Point pt = new Point( e.X, e.Y );
			if( sender is Control )
			{
				pt = this.PointToClient( ( (Control)sender ).PointToScreen( pt ) );
			}
			this.OnMouseDown( new MouseEventArgs( e.Button, e.Clicks, pt.X, pt.Y, e.Delta ) );
		}
		private void GridSelectionChanged( object sender, GridSelectionChangedEventArgs e )
		{
			if( e.Reason == GridSelectionReason.ArrowKey )
				//			if(e.Reason!= GridSelectionReason.MouseMove && e.Reason!= GridSelectionReason.MouseDown && grid.CurrentCell!=null)
				try { CheckCell( ( grid.CurrentCell.RowIndex-1 )*DEF_DAYS_COUNT + grid.CurrentCell.ColIndex ); }
				catch { }
			OnSelectionChanged( EventArgs.Empty );
		}
		private void CurrentCellMoving( object sender, GridCurrentCellMovingEventArgs e )
		{
			e.Cancel = this.cancelMoveTo;
			if( cancelMoveTo ) cancelMoveTo = false;
			if( e.RowIndex == 0 ) e.Cancel = true;
		}
		private bool IsNumber( string text )
		{
			if( text.Length == 0 ) return false;
			for( int i=0; i<text.Length; i++ )
			{
				if( !char.IsNumber( text, i ) )
				{
					return false;
				}
			}
			return true;
		}
		private void CheckCell( int cellNo )
		{
			int col = ( cellNo ) % grid.Model.ColCount;
			col = ( col == 0 ) ? grid.Model.ColCount : col;

			int row = ( cellNo - col ) / grid.Model.ColCount + 1;
			//row = ( row == 0 ) ? grid.Model.RowCount : row;

			if( !grid.Model[row, col].Enabled )
			{
				RaiseInvalidDateSelected();

				return;
			}
			if( !IsNumber( grid.Model[grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex].Text ) )
			{
				return;
			}

			DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );

			if( cellNo < prevMonthCell && !( Control.ModifierKeys == Keys.Control ) )
			{
				if( datetime.Month == 1 )
				{
					if( Value.Year == c_validMinValue.Year ) return;
					datetime.Year -= 1;
					datetime.Month = 12;
				}
				else
				{
					datetime.Month -= 1;
				}
			}
			else if( cellNo > nextMonthCell && !( Control.ModifierKeys == Keys.Control ) )
			{
				if( datetime.Month == 12 )
				{
					if( Value.Year == c_validMaxValue.Year ) return;
					datetime.Month = 1;
					datetime.Year += 1;
				}
				else
				{
					datetime.Month += 1;
				}
			}

			datetime.Day = this.GetDayFromCell( grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex );
			Value = datetime.DateTime;
		}


		private void grid_CellMouseDown( object sender, GridCellMouseEventArgs e )
		{
			if( e.RowIndex > 0 && e.ColIndex > 0 )
			{
				m_ptMouseDragMultiselectFirtsRowCol = new Point( e.RowIndex, e.ColIndex );
				m_ptMouseDragMultiselectLastRowCol = new Point( e.RowIndex, e.ColIndex );
			}
			ToggleSelectRowCol( e.RowIndex, e.ColIndex );
		}


		#region Display Week numbers property implementation

		private const string DEF_FONT_NAME = "Verdana";
		private const int DEF_FONT_SIZE = 8;
		private const int DEF_OFFSET = 1;
		private const int DEF_WEEK_COLUMN_INDEX = 0;
		private const int DEF_ROW_COUNT = 6;
		private const int DEF_DAYS_COUNT = 7;
		private const int DEF_DAYS_OFFSET = 1;
		private const string DEF_STYLE_HEADER = "Header";
		private const string DEF_STYLE_STATIC = "Static";
		private const string DEF_EMPTY_CAPTION = " ";

		/// <summary>
		/// Indicates whether week numbers displayed.
		/// </summary>
		private bool m_bShowWeekNumbers = false;
		private Font m_weekFont = null;
		private GridFontInfo m_weekFontInfo = new GridFontInfo();
		private Color m_weekTextColor = SystemColors.ControlText;
		private BrushInfo m_weekInterior = new BrushInfo( GradientStyle.Vertical,
			Color.PeachPuff, Color.AntiqueWhite );
		private int m_weekColumnOffset = 0;

		/// <summary>
		/// Gets the week column offset.
		/// </summary>

		protected int WeekColumnOffset
		{
			get
			{
				return m_weekColumnOffset;
			}
		}

		/// <summary>
		/// Gets or sets the font of the week numbers column.
		/// </summary>
		[Category( "Appearance" ),
		Description( "Indicates the font of the week numbers column." )]
		public Font WeekFont
		{
			get
			{
				if( m_weekFont == null )
				{
					ResetWeekFont();
				}

				return m_weekFont;
			}
			set
			{
				if( value == null )
				{
					ResetWeekFont();
				}
				else
				{
					m_weekFont = value;
				}

				m_weekFontInfo.Facename = m_weekFont.Name;
				m_weekFontInfo.Bold = m_weekFont.Bold;
				m_weekFontInfo.Italic = m_weekFont.Italic;
				m_weekFontInfo.Underline = m_weekFont.Underline;
				m_weekFontInfo.Size = m_weekFont.Size;
				m_weekFontInfo.Strikeout = m_weekFont.Strikeout;

				if( grid != null )
				{
					grid.Model.IgnoreReadOnly = true;
					grid.Refresh();
					grid.Model.IgnoreReadOnly = false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the text color for weeks column.
		/// </summary>
		[Category( "Appearance" )]
		[DefaultValue( typeof( Color ), "ControlText" )]
		[Description( "Gets or sets the text color for weeks column." )]
		public Color WeekTextColor
		{
			get
			{
				return m_weekTextColor;
			}
			set
			{
				if( value != m_weekTextColor )
				{
					m_weekTextColor = value;

					if( grid != null )
						grid.Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color, gradient, etc. for the week numbers column.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
	   Category( "Appearance" ),
	   Description( "Lets you set the background color, gradient, etc. for the week numbers column." )]
		public BrushInfo WeekInterior
		{
			get
			{
				return m_weekInterior;
			}
			set
			{
				if( value != m_weekInterior )
				{
					if( value == null )
					{
						ResetWeekInterior();
					}

					m_weekInterior = value;

					if( grid != null )
						grid.Refresh();
				}
			}
		}

		/// <summary>
		/// Indicates whether to display column with week numbers.
		/// </summary>
		[DefaultValue( false ),
	   Category( "Appearance" ),
	   Browsable( true ),
	   Description( "Indicates, are week numbers displayed or not." )]
		public bool ShowWeekNumbers
		{
			get
			{
				return m_bShowWeekNumbers;
			}
			set
			{
				if( value != m_bShowWeekNumbers )
				{
					m_bShowWeekNumbers = value;

					OnShowWeekNumbersChanged();
				}
			}
		}

		/// <summary>
		/// Notifies that the numbering of the weeks have changed.
		/// </summary>
		[Description( "Notifies that the numbering of the weeks have changed." )]
		public event EventHandler ShowWeekNumbersChanged;

		/// <summary>
		/// Called when the numbering of the weeks have changed.
		/// </summary>
		protected virtual void OnShowWeekNumbersChanged()
		{
			m_weekColumnOffset = m_bShowWeekNumbers ? DEF_OFFSET : 0;

			if( grid == null )
				model.ColumnOffset = m_weekColumnOffset;
			else
				grid.ColumnOffset = m_weekColumnOffset;

			if( grid == null )
			{
				model.RowCount = DEF_ROW_COUNT;
				model.ColCount = DEF_DAYS_COUNT;
				model.Rows.FrozenCount = DEF_ROW_COUNT;
				model.Cols.FrozenCount = DEF_DAYS_COUNT + WeekColumnOffset;
			}
			else
			{
				grid.Model.RowCount = DEF_ROW_COUNT;
				grid.Model.ColCount = DEF_DAYS_COUNT;
				grid.Model.Rows.FrozenCount = DEF_ROW_COUNT;
				grid.Model.Cols.FrozenCount = DEF_DAYS_COUNT + WeekColumnOffset;

                if (m_bShowWeekNumbers)
                    grid.CurrentCell.Activate(grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex + 1, GridSetCurrentCellOptions.SetFocus);
                else
                    grid.CurrentCell.Activate(grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex - 1, GridSetCurrentCellOptions.SetFocus);
            }

			this.RecalculateWidth();

			MonthCalendarAdv_SizeChanged( this, EventArgs.Empty );

			this.RaiseShowWeekNumbersChanged();
		}

		/// <summary>
		/// Raises the ShowWeekNumbersChanged event.
		/// </summary>
		protected void RaiseShowWeekNumbersChanged()
		{
			if( this.ShowWeekNumbersChanged != null )
			{
				ShowWeekNumbersChanged( this, EventArgs.Empty );
			}
		}

		private void RaiseStretchScrollImageChanged()
		{
			if( StretchScrollImageChanged != null )
			{
				StretchScrollImageChanged( this, EventArgs.Empty );
			}
		}

		protected virtual void OnStretchScrollImageChanged()
		{
			leftButton.StretchImage = this.StretchScrollImage;
			rightButton.StretchImage = this.StretchScrollImage;

			RaiseStretchScrollImageChanged();
		}


		private void ResetWeekFont()
		{
			m_weekFont = FontUtil.CreateFont( DEF_FONT_NAME, DEF_FONT_SIZE );
		}

		private void ResetWeekInterior()
		{
			this.WeekInterior = new BrushInfo( GradientStyle.Vertical,
				Color.PeachPuff, Color.AntiqueWhite );
		}

		private void SetWeekCaption( GridQueryCellInfoEventArgs e )
		{
			e.Style.Text = DEF_EMPTY_CAPTION;

			e.Style.Font = dayNamesFontInfo;
			e.Style.TextColor = dayNamesColor;
			e.Style.WrapText = this.WrapText;
			e.Style.Interior = daysHeaderInterior;

			if( style == VisualStyle.Default )
			{
				e.Style.BaseStyle = DEF_STYLE_HEADER;
			}
			else
			{
				e.Style.BaseStyle = DEF_STYLE_STATIC;
                if (Style == VisualStyle.Metro)
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.None);
                else
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid);
			}
		}

		private void SetWeekNumber( GridQueryCellInfoEventArgs e )
		{
			int colIndex = DEF_WEEK_COLUMN_INDEX + WeekColumnOffset;
			int rowIndex = e.RowIndex;

			int cellNo = ( ( rowIndex - 1 ) * DEF_DAYS_COUNT + colIndex );
			GridStyleInfo info = GetInfo( rowIndex, colIndex );
			string text = info.Text;

			if( text != null && text.Length > 0 && this.IsNumber( text ) )
			{
				int day = this.GetDayFromCell( rowIndex, colIndex );

				DateTimePickerAdv.CultureSpecifiecDateTime localizedDate =
					new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );

				int days = CurrentCalendar.GetDaysInMonth( localizedDate.Year,
					localizedDate.Month );
				if( cellNo < prevMonthCell )
				{
					localizedDate.Day = DEF_DAYS_OFFSET;
					localizedDate.DateTime = CurrentCalendar.AddMonths( localizedDate.DateTime,
						-DEF_DAYS_OFFSET );
					localizedDate.Day = day;
				}
				else if( cellNo > nextMonthCell )
				{
					localizedDate.Day = DEF_DAYS_OFFSET;
					localizedDate.DateTime = CurrentCalendar.AddMonths( localizedDate.DateTime,
						DEF_DAYS_OFFSET );
					localizedDate.Day = day;
				}
				else if( day <= days )
				{
					localizedDate.Day = day;
				}
                if (Iso8601CalenderFormat)
                {
                    e.Style.Text = localizedDate.GetWeekOfYearIso8601(this.Culture.DateTimeFormat.CalendarWeekRule, this.Culture.DateTimeFormat.FirstDayOfWeek).ToString();
                }
                else
                {
                    if (localizedDate.Year != dvalue.Year)
                    {
                        localizedDate.DateTime = localizedDate.DateTime.AddDays(6.0);
                    }
                    e.Style.Text = localizedDate.GetWeekOfYear(this.Culture.DateTimeFormat.CalendarWeekRule, this.Culture.DateTimeFormat.FirstDayOfWeek).ToString();
                }           
				e.Style.TextColor = this.WeekTextColor;

				if( m_weekFontInfo != null )
				{
					e.Style.Font = m_weekFontInfo;
				}

				if( this.WeekInterior != null )
				{
					e.Style.Interior = this.WeekInterior;
				}
			}
		}

		private void SetWeek( GridQueryCellInfoEventArgs e )
		{
			e.Style.Enabled = false;

			if( e.RowIndex == 0 )
			{
				SetWeekCaption( e );
			}
			else if( e.RowIndex > 0 )
			{
				SetWeekNumber( e );
			}
		}

		#endregion

		private void CellClick( object sender, GridCellClickEventArgs e )
		{
			m_bCellClicked = true;

			int cellNo = ( e.RowIndex-1 )*DEF_DAYS_COUNT + e.ColIndex;
			if( e.RowIndex == 0 || ( e.ColIndex == DEF_WEEK_COLUMN_INDEX && m_bShowWeekNumbers ) )
			{
				return;
			}
			this.selectedDates = new DateTime[0];

			if( !m_bSkipCheckCell )
			{
				CheckCell( cellNo );
			}
			else
			{
				m_bSkipCheckCell = false;
			}

			OnDateSelected( EventArgs.Empty );

			m_bCellClicked = false;
		}

		private int maxColWidth = 67;
		private void RecalculateWidth()
		{
			if( !sizeToFit || grid == null )
				return;

			grid.Model.ColWidths.ResizeToFit( GridRangeInfo.Row( 0 ) );
			maxColWidth = 0;
			int daysCount = DEF_DAYS_COUNT + WeekColumnOffset;
			for( int i = 1; i <= daysCount; i++ )
			{
				maxColWidth = Math.Max( maxColWidth, grid.Model.ColWidths[i] );
			}
			for( int i = 1; i<= daysCount; i++ )
			{
				grid.Model.ColWidths[i] = maxColWidth;
			}
			if( Width != maxColWidth * daysCount )
			{
				Width = maxColWidth * daysCount;
			}
		}
		private void MonthCalendarAdv_SizeChanged( object sender, System.EventArgs e )
		{
			if( this.grid == null )
				return;

			grid.DefaultRowHeight = ( Height - headerPanel.Height - bottomPanel.Height - this.dayNamesHeight ) / grid.Model.RowCount;
			grid.SetRowHeight( 0, 0, dayNamesHeight );
			grid.Model.RowHeights[grid.Model.RowCount] = Height - headerPanel.Height - bottomPanel.Height - this.dayNamesHeight - grid.DefaultRowHeight * ( grid.Model.RowCount - 1 );

			int nColCount = DEF_DAYS_COUNT + WeekColumnOffset;

			if( this.sizeToFit )
			{
				if( Width != maxColWidth * nColCount )
				{
					Width = maxColWidth * nColCount;
				}
			}
			else
			{
				int nDefColWidth = Width / nColCount;
				int gap = this.Width - nDefColWidth * nColCount;

				grid.DefaultColWidth = nDefColWidth;

				for( int i = 1; i <= nColCount; i++ )
				{
					grid.Model.ColWidths[i] = nDefColWidth + ( ( i <= gap ) ? 1 : 0 );
				}
			}

			SetHeaderPositions();
			RefreshHeaderBrush();

			headerPanel.Invalidate();
		}

		private void bottomPanel_VisibleChanged( object sender, EventArgs e )
		{
			//	this.MonthCalendarAdv_SizeChanged(this,e);
		}
		private void RefreshHeaderBrush()
		{
			try
			{
				headerBrush = new LinearGradientBrush( headerPanel.ClientRectangle, headerStartColor, headerEndColor, headerVerticalGradient?90:0 );
			}
			catch { }
		}
		private void RefreshNames()
		{
			if( grid == null )
				return;

			DateTimeFormatInfo format = this.Culture.DateTimeFormat;

			for( int i = 1; i <= DEF_DAYS_COUNT; i++ )
			{
				int dayIndex = ( i - 1 + (int)format.FirstDayOfWeek ) % DEF_DAYS_COUNT;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				string dayName = m_bShortestDayNames 
					? format.ShortestDayNames[dayIndex]
					: format.AbbreviatedDayNames[dayIndex];
#else
                string dayName = format.AbbreviatedDayNames[ dayIndex ];
#endif
				days[i] = dayName;
			}
			for( int i = 1; i <= 12; i++ )
			{
				months[i] = format.MonthNames[i - 1];
			}
			grid.RefreshRange( GridRangeInfo.Cells( 0, 0, 0, DEF_DAYS_COUNT ) );
			int iMonth = CurrentCalendar.GetMonth( dvalue );
			monthLabel.Text = months[iMonth];
			InitializeMenu();

			if( this.sizeToFit )
			{
				this.RecalculateWidth();
			}
		}

		private DateTime GetDateFromCell(int row, int col)
		{
			int day;
			
			int month = CurrentCalendar.GetMonth(dvalue);
			int year = CurrentCalendar.GetYear(dvalue);

			int cell = col + DEF_DAYS_COUNT * (row - 1);

			if (cell < this.prevMonthCell)
			{
				day = this.GetPrevMonthDayCount() - prevMonthCell + cell + 1;

				month -= 1;

				if (month < 1)
				{
					month = 12;
					year -= 1;
				}
			}
			else
			{
				if (cell > this.nextMonthCell)
				{
					day = cell - this.prevMonthCell - CurrentCalendar.GetDaysInMonth(year, month) + 1;

					month += 1;

					if (month > 12)
					{
						month = 1;
						year += 1;
					}
				}
				else
				{
					day = cell - this.prevMonthCell + 1;
				}
			}

			return new DateTime(year, month, day, CurrentCalendar);
		}

		private int GetDayFromCell( int rowIndex, int colIndex )
		{
			int cell = colIndex+DEF_DAYS_COUNT*( rowIndex-1 );
			if( cell < this.prevMonthCell )
			{
				return this.GetPrevMonthDayCount() - prevMonthCell + cell+1;
			}
			if( cell > this.nextMonthCell )
			{
				int iYear = CurrentCalendar.GetYear( dvalue );
				int iMonth = CurrentCalendar.GetMonth( dvalue );
				return cell - this.prevMonthCell - CurrentCalendar.GetDaysInMonth( iYear, iMonth ) + 1;
			}
			return cell - this.prevMonthCell+1;
		}
		private int GetPrevMonthDayCount()
		{
			int iMonth = CurrentCalendar.GetMonth( dvalue );
			int iYear = CurrentCalendar.GetYear( dvalue );

			if( iMonth == 1 )
			{
				if( dvalue.Year == DateTime.MinValue.Year ) return 30;
				return CurrentCalendar.GetDaysInMonth( iYear - 1, 12 );
			}
			else
			{
				return CurrentCalendar.GetDaysInMonth( iYear, iMonth-1 );
			}
		}

		private void SetHeaderPositions()
		{
			if( this.IsInitializing )
				return;

			Graphics g = monthLabel.CreateGraphics();
			Size monthSize = g.MeasureString( monthLabel.Text, monthLabel.Font ).ToSize();
			monthSize.Width+=4;
			monthLabel.Size = monthSize;
			g.Dispose();

			g = yearLabel.CreateGraphics();
			Size yearSize = g.MeasureString( yearLabel.Text, yearLabel.Font ).ToSize();
			g.Dispose();
			yearSize.Width +=4;
			yearLabel.Size = yearSize;
			yearUD.Size = new Size( yearSize.Width+20, yearUD.Height );

			monthLabel.Location = new Point( ( Width- monthSize.Width - yearLabel.Width )/2, ( headerPanel.Height- monthLabel.Height )/2 );
			yearLabel.Location = new Point( monthLabel.Location.X+monthLabel.Width, ( headerPanel.Height- yearLabel.Height )/2 );
			yearUD.Location = new Point( monthLabel.Location.X+monthLabel.Width, ( headerPanel.Height- yearUD.Height )/2 );
            if (Style == VisualStyle.Metro)
            {
                leftButton.Location = new Point(buttonOffset + 15, (headerPanel.Height - leftButton.Height) / 2);
                rightButton.Location = new Point(Width - buttonOffset - rightButton.Width - 15, (headerPanel.Height - rightButton.Height) / 2);
            }
            else
            {
                leftButton.Location = new Point(buttonOffset, (headerPanel.Height - leftButton.Height) / 2);
                rightButton.Location = new Point(Width - buttonOffset - rightButton.Width, (headerPanel.Height - rightButton.Height) / 2);
            }

		}
		private void SetHeaderPanelTexts( string month, int year )
		{
			monthLabel.Text = month;
			yearLabel.Text = year.ToString();
			yearUD.Value = year;

			SetHeaderPositions();
		}
        private bool same_Month = true;
		private void ValueChanged( bool sameMonth )
		{
            same_Month = sameMonth;
			DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );
			int iDayOfWeek = (int)CurrentCalendar.GetDayOfWeek( dvalue );

			SetHeaderPanelTexts( this.Culture.DateTimeFormat.MonthNames[datetime.Month - 1], datetime.Year );
			grid.Model.IgnoreReadOnly = true;
			//Added this
			int dayPrev = datetime.Day;
			datetime.Day = 1;
			int iFirstDay = (int)this.Culture.DateTimeFormat.FirstDayOfWeek;
			int dow = ( DEF_DAYS_COUNT - iFirstDay + datetime.DayOfWeek ) % DEF_DAYS_COUNT + 1;
			datetime.Day = dayPrev;
			int prevDayCount = GetPrevMonthDayCount();
			prevMonthCell = dow;
			for( int i = 1; i < dow; i++ )
			{
				GridStyleInfo info = GetInfo( 1, i );

				info.Text = ( prevDayCount - dow + i + 1 ).ToString();
				info.TextColor = SystemColors.InactiveCaptionText;
				info.Interior = BrushInfo.Empty;
				info.Enabled = true;
				for (int j = 0; j < selectedDates.Length; j++)
				{
					DateTimePickerAdv.CultureSpecifiecDateTime sd =
						new DateTimePickerAdv.CultureSpecifiecDateTime(selectedDates[j], CurrentCalendar);
					if (info.Text == sd.Day.ToString())
					{
						DateTime tempdate = new DateTime(sd.Year, sd.Month, Int32.Parse(info.Text));

						foreach (DateTime dtvalue in selectedDates)
						{
							if (sd.Year == datetime.Year && tempdate == dtvalue && dtvalue.Month == (this.Value.Month - 1))
								this.SelectCell(1, i, this.allowMultipleSelection);
						}
					}
				}
			}
			int dim = CurrentCalendar.GetDaysInMonth( datetime.Year, datetime.Month );
			nextMonthCell = dim + dow - 1;

			for( int i = dow - 1; i < dim + dow - 1; i++ )
			{
				int row = i / DEF_DAYS_COUNT + 1;
				int col = i % DEF_DAYS_COUNT + 1;
				GridStyleInfo info = GetInfo( row, col );

				info.Text = ( i - dow + 2 ).ToString();
				info.TextColor = daysColor;
				info.Interior = BrushInfo.Empty;
				info.Enabled = true;

				if( ( i - dow + 2 ) == datetime.Day )
				{
					grid.CurrentCell.MoveTo( row, col );
				}
				for( int j = 0; j < selectedDates.Length; j++ )
				{
					DateTimePickerAdv.CultureSpecifiecDateTime sd =
                        new DateTimePickerAdv.CultureSpecifiecDateTime( selectedDates[j], CurrentCalendar );

					if( sd.Year == datetime.Year && sd.Month == datetime.Month && sd.Day == i - dow + 2 )
					{
						this.SelectCell( row, col, this.allowMultipleSelection );
					}
				}
			}
			for( int i = dim + dow - 1; i < 42; i++ )
			{
				GridStyleInfo info = GetInfo( i / DEF_DAYS_COUNT + 1, i % DEF_DAYS_COUNT + 1 );
				int row = i / DEF_DAYS_COUNT + 1;
				int col = i % DEF_DAYS_COUNT + 1;
				info.Text = ( i - ( dim + dow - 1 ) + 1 ).ToString();
				info.TextColor = SystemColors.InactiveCaptionText;
				info.Interior = BrushInfo.Empty;
				info.Enabled = true;
				for (int j = 0; j < selectedDates.Length; j++)
				{
					DateTimePickerAdv.CultureSpecifiecDateTime sd =
					new DateTimePickerAdv.CultureSpecifiecDateTime(selectedDates[j], CurrentCalendar);
					DateTime tempdate = new DateTime(sd.Year, sd.Month, sd.Day);

					if (sd.Year == datetime.Year && sd.Month == datetime.Month && sd.Day == i - dow + 2)
					{
						this.SelectCell(row, col, this.allowMultipleSelection);
					}

				}
			}
            for (int i = 0; i < 42; i++)
            {
               // GridStyleInfo info = GetInfo(i / DEF_DAYS_COUNT + 1, i % DEF_DAYS_COUNT + 1);
                int row = i / DEF_DAYS_COUNT + 1;
                int col = i % DEF_DAYS_COUNT + 1;

                if (i <= dow)
                {
                    int prevMonth = datetime.Month - 1;
                    int year = datetime.Year;
                    if (prevMonth == 0)
                    {
                        year = datetime.Year - 1;
                        prevMonth = 12;
                    }
                    int s = Int32.Parse((prevDayCount - dow + i + 1).ToString());
                    for (int j = 0; j < selectedDates.Length; j++)
                    {
                        DateTimePickerAdv.CultureSpecifiecDateTime sd =
                        new DateTimePickerAdv.CultureSpecifiecDateTime(selectedDates[j], CurrentCalendar);
                        if (sd.Year == year && prevMonth == sd.Month && s == sd.Day)
                            this.SelectCell(row, col, this.allowMultipleSelection);
                    }
                }
                if (i >= (dim + dow - 1))
                {
                    int nextmonth = datetime.Month + 1;
                    int year = datetime.Year;
                    if (nextmonth > 12)
                    {
                        year = datetime.Year + 1;
                        nextmonth = 1;
                    }
                    int s = Int32.Parse((i - (dim + dow-1 )+1 ).ToString());
                    for (int j = 0; j < selectedDates.Length; j++)
                    {
                        DateTimePickerAdv.CultureSpecifiecDateTime sd =
                        new DateTimePickerAdv.CultureSpecifiecDateTime(selectedDates[j], CurrentCalendar);
                        if (sd.Year == year && nextmonth == sd.Month && s == sd.Day)
                            this.SelectCell(row, col, this.allowMultipleSelection);
                    }
                }
            }
			if( !sameMonth || this.CurrentSelDates.Count == 0 )
			{
				if( !sameMonth )
				{
					CorrectCurrentSelectedCell();
					cancelMoveTo = m_bCellClicked;
				}

				GridRangeInfo range = GridRangeInfoAdv.FromGridRangeInfo( GridRangeInfo.Cell( grid.CurrentCell.RowIndex, grid.CurrentCell.ColIndex ) );

				SetRangeDates( range );

				this.CurrentSelDates.Add( range );

				grid.RefreshRange( GridRangeInfo.Cells( 1, 1, 6, DEF_DAYS_COUNT ) );
				grid.Refresh();
			}
			else
			{
				grid.Invalidate();
			}
			grid.Model.IgnoreReadOnly = false;

			headerPanel.Refresh();
		}

		private void SetRangeDates( GridRangeInfo range )
		{
			int year = CurrentCalendar.GetYear( dvalue );
			int month = CurrentCalendar.GetMonth( dvalue );
			int row, col;

			if( range.GetFirstCell( out row, out col ) )
			{
				range.Dates.Add( GetDateFromCell(row, col) );

				while( range.GetNextCell( ref row, ref col ) )
				{
					range.Dates.Add(GetDateFromCell(row, col));
				}
			}
		}

		private void todayButton_Click( object sender, System.EventArgs e )
		{
			this.CurrentSelDates.Clear();
			this.grid.Refresh();
			Value = DateTime.Now;
			OnDateSelected( EventArgs.Empty );
		}

		private void leftButton_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			SelectNextMonth(this.RightToLeft == RightToLeft.Yes);
		}

		private void rightButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SelectNextMonth(this.RightToLeft == RightToLeft.No);
		}

		private void SelectNextMonth(bool bForward)
		{
			if (bForward)
			{
				if (Value.Year != c_validMaxValue.Year || Value.Month != 12)
				{
					DateTime value = CurrentCalendar.AddMonths(dvalue, 1);

					if (value > this.MaxValue)
					{
						value = this.MaxValue;
					}

					SelectMonth(value);
				}
			}
			else
			{
				if (Value.Year != DateTime.MinValue.Year || Value.Month != 1)
				{
					DateTime value = CurrentCalendar.AddMonths(dvalue, -1);

					if (value < this.MinValue)
					{
						value = this.MinValue;
					}

					SelectMonth(value);
				}
			}
		}

		private void SelectMonth( DateTime value )
		{
			this.SuspendLayout();
			grid.GridControl.BeginUpdate();

			if( ( Control.ModifierKeys & Keys.Control ) == 0 )
			{
				m_selectedDates.Clear();
				this.Value = value;
			}
			else
			{
				this.Value = value;
				Deselect(value);
			}

			grid.GridControl.EndUpdate();
			this.ResumeLayout();
		}

		private void Deselect(DateTime value)
		{
			DateTime dtKey = new DateTime(value.Year, value.Month, 1);
			
			GridRangeInfoList lstInfo = m_selectedDates[dtKey] as GridRangeInfoList;
			if (lstInfo != null)
			{
				for (int i = 0; i < lstInfo.Count; i++)
				{
					GridRangeInfoAdv info = lstInfo[i] as GridRangeInfoAdv;
					if (info != null)
					{
						if (info.Dates.Count > 0 && info.Dates[0] == value)
						{
							lstInfo.RemoveAt(i);
							break;
						}
					}
				}
			}
		}

		private void nullButton_Click( object sender, System.EventArgs e )
		{
            if (ClearSelectionOnNone)
            {
                this.m_selectedDates.Clear();
                grid.Refresh();
            }
			grid.CurrentCell.Deactivate( true );
			OnNoneButtonClick( EventArgs.Empty );
		}

		private void gotoToday_Click( object sender, System.EventArgs e )
		{
			Value = DateTime.Now;
		}

		private void headerPanel_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			base.OnPaint( e );

            Graphics g = e.Graphics;
            Brush br = new SolidBrush(monthLabel.ForeColor);
            if (headGradient)
            {
                g.FillRectangle(headerBrush, headerPanel.ClientRectangle);
            }
            g.DrawString(monthLabel.Text, monthLabel.Font, br, monthLabel.Location);
            DrawMetroHeaderBackGround(e);


            if (!yearUD.Visible)
            {
                br = new SolidBrush(yearLabel.ForeColor);
                g.DrawString(yearLabel.Text, yearLabel.Font, br, yearLabel.Location);
            }
            br.Dispose();
        }		/// <summary>
		///draws the metroheader background.
		/// </summary>

        private void DrawMetroHeaderBackGround(PaintEventArgs e)
        {
            if (this.style == VisualStyle.Metro && this.LeftScrollButtonImage == null && this.RightScrollButtonImage == null)
            {
                SmoothingMode mode = SmoothingMode.AntiAlias;
                e.Graphics.SmoothingMode = mode;
                if (b_mousehover)
                {
                    using (Pen darkpen = new Pen(ControlPaint.Dark(circleColor), 1))
                    {
                        using (Pen lightpen = new Pen(circleColor, 1))
                        {
                            Rectangle rect = new Rectangle(Control.MousePosition.X, Control.MousePosition.Y, 1, 1);
                            if (b_leftbutton)
                            {
                                e.Graphics.DrawEllipse(darkpen, leftButton.Location.X, leftButton.Location.Y, leftButton.Width - 1, leftButton.Height - 1);
                                e.Graphics.DrawEllipse(lightpen, rightButton.Location.X+1, leftButton.Location.Y, rightButton.Width-2, rightButton.Height-1);
                            }
                            else if (b_rightbutton)
                            {
                                e.Graphics.DrawEllipse(darkpen, rightButton.Location.X+1, leftButton.Location.Y, rightButton.Width - 2, rightButton.Height - 1);
                                e.Graphics.DrawEllipse(lightpen, leftButton.Location.X, leftButton.Location.Y, leftButton.Width-1, leftButton.Height-1);
                            }
                        }
                    }
                }
                else
                {
                    using (Pen pen = new Pen(circleColor, 1))
                    {
                        e.Graphics.DrawEllipse(pen, leftButton.Location.X, leftButton.Location.Y, leftButton.Width-1, leftButton.Height-1);
                        e.Graphics.DrawEllipse(pen, rightButton.Location.X+1, leftButton.Location.Y, rightButton.Width-2, rightButton.Height-1);
                    }
                }
            }
            if (this.BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
            {
                using (Pen pen = new Pen(BorderColor, 1))
                {
                    e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height));
                }
            }
            Invalidate();
        }
		private void headerPanel_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			if( monthLabel.ClientRectangle.Contains( monthLabel.PointToClient( Control.MousePosition ) ) )
			{
				this.ShowMonthPopup();
			}

			if( yearLabel.ClientRectangle.Contains( yearLabel.PointToClient( Control.MousePosition ) ) )
			{
				yearUD.Visible = true;
				yearUD.Focus();
			}
			else
			{
				yearUD.Visible = false;
			}

		}

		private void yearUD_ValueChanged( object sender, System.EventArgs e )
		{
			DateTimePickerAdv.CultureSpecifiecDateTime datetime =
                new DateTimePickerAdv.CultureSpecifiecDateTime( dvalue, CurrentCalendar );

			datetime.Year = (int)yearUD.Value;
			datetime.Day = Math.Min( datetime.Day, datetime.DaysInMonth );

			Value = datetime.DateTime;
		}

		private void yearUD_VisibleChanged( object sender, System.EventArgs e )
		{
			//	yearLabel.Visible = !yearUD.Visible;
		}
		private void yearUD_Leave( object sender, System.EventArgs e )
		{
			yearUD.Visible = false;
		}

		private void monthLabel_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			yearUD.Visible = false;
			this.ShowMonthPopup();
		}

		private void ShowMonthPopup()
		{
			try
			{
				if( this.popupParent != null )
				{
					this.popupParent.TogglePopup( Control.MousePosition, this.parentBarItem1 );
				}
				else
				{
					if( this.popupMonths == null )
					{
						InitMonthsPopup();
					}

					if( this.popupMonths.IsShowing() )
					{
						this.popupMonths.Hide();
					}
					else
					{
						this.popupMonths.ParentBarItem.Style = this.Style;
						// This will be the case when the calendar is dropped directly on the Form.
						this.popupMonths.Show( this, this.PointToClient( Control.MousePosition ) );
					}
				}
			}
			catch
			{
			}
		}

		private void yearLabel_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			yearUD.Visible = true;
			yearUD.BringToFront();
			yearUD.Focus();
		}

		/// <summary>
		/// Raises the MouseDown event.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			// Raising Double click event routine
			if( !( m_bDoubleClicked ) && IsLessDoubleClickTimeInterval() && ( m_ptLastMouseDown.Equals( Cursor.Position ) ) )
			{
				OnDoubleClick( EventArgs.Empty );
				m_bDoubleClicked = true;
			}
			else
			{
				m_bDoubleClicked = false;
			}

			m_ptLastMouseDown = Cursor.Position;
			m_tiksLastClickTime = DateTime.Now.Ticks;

			base.OnMouseDown( e );
		}

		/// <summary>
		/// Check whether time interval between two last mouse down events is less 
		/// than SystemInformation.DoubleClickTime.
		/// </summary>
		/// <returns>value indicating whether time interval between two last mouse down events is less 
		/// than SystemInformation.DoubleClickTime</returns>
		private bool IsLessDoubleClickTimeInterval()
		{
			return ( DateTime.Now.Ticks - m_tiksLastClickTime ) <= ( SystemInformation.DoubleClickTime * 10000 );
		}

		private void InvalidateWindow()
		{
			NativeMethods.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero,
				NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW
				| NativeMethods.RDW_INVALIDATE );
		}

		protected override void WndProc( ref Message m )
		{
			base.WndProc( ref m );
		}

		/// <summary>
		/// Gets currently used calendar.
		/// </summary>
		[Browsable( false )]
		public Calendar CurrentCalendar
		{
			get
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				if( this.Culture.LCID == DEF_SAUDI_ARABIA_LCID )
				{
					if( m_umAlCuma != null )
						return m_umAlCuma;

                    if (m_hijriCalendar != null)
                        return m_hijriCalendar;

					foreach( Calendar cal in this.Culture.OptionalCalendars )
					{
						UmAlQuraCalendar saudi_arabia_cal = cal as UmAlQuraCalendar;

						if( saudi_arabia_cal != null )
						{
							return ( m_umAlCuma = saudi_arabia_cal );
						}

                        HijriCalendar hijri = cal as HijriCalendar;

                        if (hijri != null)
                            return m_hijriCalendar = hijri;
					}
				}
#endif

				return this.Culture.Calendar;
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private UmAlQuraCalendar m_umAlCuma;
        private HijriCalendar m_hijriCalendar;
#endif

		#region INonClientPaintingSupport

		/// <summary>
		/// Implement this method and draw your NonClient area using the passed in params.
		/// </summary>
		/// <param name="e">The PaintEventArgs using this to draw the nonclient area.</param>
		/// <param name="displayRect">The control's window bounds into which to draw. Left and Top are usually 0.</param>
		/// <param name="windowRectInScreen">The control's bounds in screen co-ords.</param>
		/// <returns>
		/// Return a HRgn (as IntPtr) that excludes the region you just drew in the displayRect.
		/// </returns>
		public System.IntPtr NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			Graphics g = e.Graphics;
			Rectangle bounds = displayRect;

			Point pt = this.PointToScreen( new Point( 0, 0 ) );
			pt.Offset( -windowRectInScreen.X, -windowRectInScreen.Y );
			Rectangle rcClient = new Rectangle( pt, this.ClientSize );

			// Fill the border-rectangles with the bg brush, since some of the 
			// 3d border types are only 1 pixel wide.
			using( Region rg = new Region( bounds ) )
			{
				rg.Exclude( rcClient );

				using( Brush backColorBrush = new SolidBrush( this.BackColor ) )
				{
					g.FillRegion( backColorBrush, rg );
				}
			}

			bool bThemesEnabled = ( XPThemes.IsThemedOS && XPThemes.IsThemeActive &&
				this.ThemedBorder );

			if( bThemesEnabled && this.BorderStyle == BorderStyle.Fixed3D )
			{
				Region oldRegion = g.Clip;
				g.ExcludeClip( rcClient );

				m_themedEditDrawing.DrawEditBoxBackground( g, bounds, this.Enabled );

				g.Clip = oldRegion;
			}
			else
			{
				if( this.BorderSides != Border3DSide.All )
				{
					if( this.BorderSides != Border3DSide.Middle )
					{
						m_controlDrawing.DrawBorder( g, bounds, BorderStyle, this.Border3DStyle,
							ButtonBorderStyle.Solid, this.BorderColor, this.BorderSides );
					}
				}
				else
				{
					m_controlDrawing.DrawBorder( g, bounds, BorderStyle,
						this.Border3DStyle, ButtonBorderStyle.Solid, this.BorderColor );
				}
			}

			// Return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn( windowRectInScreen.Left + c_iBorderThickness,
				windowRectInScreen.Top + c_iBorderThickness, windowRectInScreen.Right - c_iBorderThickness,
				windowRectInScreen.Bottom - c_iBorderThickness );
		}

		#endregion

		private void OnOffice2007ThemeChanged()
		{
			ApplyOffice2007ThemeToChildren();

			if( this.Style == VisualStyle.Office2007 )
			{
				m_office2007ColorTable = Office2007Colors.GetColorTable( m_office2007Theme );

				ApplyOffice2007Theme();

				this.Invalidate();
			}
		}

		private void ApplyOffice2007Theme()
		{
            if (this.Style == VisualStyle.Office2007)
            {
                this.HeadForeColor = m_office2007ColorTable.MonthCalendarForeColor;
                this.HeaderStartColor = m_office2007ColorTable.MonthCalendarHeaderStartColor;
                this.HeaderEndColor = m_office2007ColorTable.MonthCalendarHeaderEndColor;

                this.leftButton.ArrowColor = m_office2007ColorTable.MonthCalendarForeColor;
                this.rightButton.ArrowColor = m_office2007ColorTable.MonthCalendarForeColor;
            }
        }

        private void ApplyOffice2007ThemeToChildren()
        {
            this.yearUD.ColorScheme = m_office2007Theme;
            this.TodayButton.Office2007ColorScheme = m_office2007Theme;
            this.NoneButton.Office2007ColorScheme = m_office2007Theme;
        }

        private void Office2007ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            ApplyOffice2007Theme();
        }
        private void Office2010ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            ApplyOffice2010Theme();
        }
		private void OnOffice2010ThemeChanged()
		{
			ApplyOffice2010ThemeToChildren();

			if( this.Style == VisualStyle.Office2010 )
			{
				m_office2010ColorTable = Office2010Colors.GetColorTable( m_office2010Theme );

				ApplyOffice2010Theme();

				this.Invalidate();
			}
		}

		private void ApplyOffice2010Theme()
		{
            if (this.Style == VisualStyle.Office2010)
            {
                this.HeadForeColor = m_office2010ColorTable.MonthCalendarForeColor;
                this.HeaderStartColor = m_office2010ColorTable.MonthCalendarHeaderStartColor;
                this.HeaderEndColor = m_office2010ColorTable.MonthCalendarHeaderEndColor;
                this.GridBackColor = m_office2010ColorTable.MonthCalendarBackgroundColor;
                this.leftButton.ArrowColor = m_office2010ColorTable.MonthCalendarForeColor;
                this.rightButton.ArrowColor = m_office2010ColorTable.MonthCalendarForeColor;
            }
		}
		/// <summary>
		/// Applies the metro style.
		/// </summary>
        private void ApplyMetroTheme()
        {
            if (this.Style == VisualStyle.Metro)
            {
                this.leftButton.ArrowColor = Color.FromArgb(111,111,111);
                this.rightButton.ArrowColor = Color.FromArgb(111, 111, 111);
            }
        }
		private void ApplyOffice2010ThemeToChildren()
		{
			this.yearUD.ColorScheme = m_office2007Theme;
			this.TodayButton.Office2010ColorScheme = m_office2010Theme;
			this.NoneButton.Office2010ColorScheme = m_office2010Theme;
		}

		private void Office2010ManagedColorsApplied( Office2010Colors.ManagedColorsAppliedEventArgs args )
		{
			ApplyOffice2010Theme();
		}
	}

	// Custom PopupControlContainer that implements the IPopupParent interface,
	// espcially to listen to ChildClosing events.
	[ToolboxItem( false )]
	public class CalendarPopup: PopupControlContainer
	{
		private MonthCalendarAdv calendar;

		[Obsolete( "This constructor is equal to default constructor. Use default constructor instead of this, because PopupControlContainer is Control, not component." )]
		public CalendarPopup( IContainer container )
			: base( container )
		{
		}

		public CalendarPopup()
		{
		}

		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public MonthCalendarAdv Calendar
		{
			get { return this.calendar; }
			set
			{
				this.calendar = value;
				this.calendar.PopupParent = this;
			}
		}

		// pos in screen co-ords
		internal void TogglePopup( Point pos, ParentBarItem parentBarItem )
		{
			if( this.CurrentPopupChild != null )
			{
				CurrentPopupChild = null;
				return;
			}
			else
			{
				this.ShowPopupMenu( pos, parentBarItem );
			}
		}
		internal void ShowPopupMenu( Point pos, ParentBarItem parentBarItem )
		{
			// The menuGrid will release itself based on settings
			MenuGrid menuGrid = XPMenuGridFactory.GetMenuGridToDeploy();

			this.CurrentPopupChild = menuGrid;

			//Show the menu
			menuGrid.Show( parentBarItem, pos, this, false );
            menuGrid.RightToLeft = this.Calendar.RightToLeft;		 
            if (!menuGrid.IsShowing())
			{
				( (IPopupParent)this ).ChildClosing( menuGrid, PopupCloseType.Canceled );
				return;
			}
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			if( null != this.calendar )
			{
				this.calendar.PopupParent = null;
				this.calendar = null;
			}
		}

		protected override PopupHost CreatePopupHost()
		{
			return new CalendarPopupHost();
		}
	}

	[ToolboxItem( false )]
	public class CalendarPopupHost: PopupHost
	{
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			if( this.PopupControl != null )
			{
				this.PopupControl.SizeChanged += new EventHandler( PopupControl_SizeChanged );
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( this.PopupControl != null )
			{
				this.PopupControl.SizeChanged -= new EventHandler( PopupControl_SizeChanged );
			}

			base.Dispose( disposing );
		}

		private void PopupControl_SizeChanged( object sender, EventArgs e )
		{
			if( this.IsShowing() )
			{
				this.ComputeLayout();
			}
		}
	}

	/// <summary>
	/// Handler for the <see cref="MonthCalendarAdv.DateCellQueryInfo"/> event.
	/// </summary>
	public delegate void DateCellQueryInfoEventHandler( object sender, DateCellQueryInfoEventArgs e );

	/// <summary>
	/// Provides information about the current calendar date cell that is being formatted.
	/// </summary>
	/// <remarks>
	/// The <see cref="DateValue"/> property provides the date that this
	/// cell represents. The <see cref="Style"/> property can be set to any
	/// valid <see cref="GridStyleInfo"/> values and it will be applied to the
	/// calendar cell. Set the <see cref="Handled"/> property to true for the
	/// changes to be accepted.
	/// </remarks>
	public class DateCellQueryInfoEventArgs: SyncfusionHandledEventArgs
	{
		int rowIndex;
		int colIndex;
		GridStyleInfo style;
		bool isCurrentCell;
		bool isOutsideRange;
		object dateValue = null;

		public DateCellQueryInfoEventArgs( int rowIndex, int colIndex, GridStyleInfo style, object dateValue, bool isCurrentCell, bool isOutsideRange )
		{
			this.rowIndex = rowIndex;
			this.colIndex = colIndex;
			this.style = style;
			if( dateValue != null )
				this.dateValue = dateValue;
			this.isCurrentCell = isCurrentCell;
			this.isOutsideRange = isOutsideRange;
		}

		[TraceProperty( true )]
		public int RowIndex
		{
			get
			{
				return rowIndex;
			}
		}
		[TraceProperty( true )]
		public int ColIndex
		{
			get
			{
				return colIndex;
			}
		}
		[TraceProperty( true )]
		public GridStyleInfo Style
		{
			get
			{
				return style;
			}
		}

		[TraceProperty( true )]
		public object DateValue
		{
			get
			{
				if( this.dateValue == null )
					return null;
				else
					return dateValue;
			}
		}

		[TraceProperty( true )]
		public bool IsCurrentCell
		{
			get
			{
				return isCurrentCell;
			}
		}

		[TraceProperty( true )]
		public bool IsOutsideRange
		{
			get
			{
				return isOutsideRange;
			}
		}
	}

	/// <summary>
	/// The MonthCalendarExt type will soon be replaced with the MonthCalendarAdv for consistency in 
	/// Control naming in our library. 
	/// Please replace all occurrences of MonthCalendarExt with MonthCalendarAdv in your app.
	/// </summary>
	[Obsolete( "The MonthCalendarExt type will soon be replaced with the MonthCalendarAdv for consistency in naming in our library. Please replace all occurences of MonthCalendarExt with MonthCalendarAdv in your app." ),
	ToolboxItem( false )]
	public class MonthCalendarExt: MonthCalendarAdv
	{
		public MonthCalendarExt() : base() { }
		internal MonthCalendarExt( bool dtCalendar ) : base( dtCalendar ) { }
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	[ToolboxItem( false )]
	public class MonthCalendarButton: ButtonAdv
	{
		#region Class Members
		private bool m_bInitializing = false;
		private bool m_bUseVisualStyle = false;
		private ButtonAppearance m_appearance;
		#endregion

		#region Class Properties

		internal bool Initializing
		{
			get
			{
				return m_bInitializing;
			}
			set
			{
				if( value != m_bInitializing )
				{
					m_bInitializing = value;

					if( !m_bInitializing )
					{
						RevertInitialAppearance();
					}
				}
			}
		}

		public new ButtonAppearance Appearance
		{
			get
			{
				return base.Appearance;
			}
			set
			{
				if( m_bInitializing )
				{
					m_appearance = value;
				}
				else
				{
					base.Appearance = value;
				}
			}
		}

		public override bool UseVisualStyle
		{
			get
			{
				return base.UseVisualStyle;
			}
			set
			{
				if( m_bInitializing )
				{
					m_bUseVisualStyle = value;
				}
				else
				{
					base.UseVisualStyle = value;
				}
			}
		}

		[DefaultValue( FlatStyle.Popup )]
		public new FlatStyle FlatStyle
		{
			get
			{
				return base.FlatStyle;
			}

			set
			{
				base.FlatStyle = value;
			}
		}

		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new string Name
		{
			get
			{
				return base.Name;
			}

			set
			{
				base.Name = value;
			}
		}

		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new int TabIndex
		{
			get
			{
				return base.TabIndex;
			}

			set
			{
				base.TabIndex = value;
			}
		}

		public new Size Size
		{
			get
			{
				return base.Size;
			}

			set
			{
				base.Size = value;
			}
		}

		#endregion

		#region Class Utility Methods

		private void RevertInitialAppearance()
		{
			base.Appearance = m_appearance;
			base.UseVisualStyle = m_bUseVisualStyle;
		}

		#endregion
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	[ToolboxItem( false )]
	public class MonthCalendarNoneButton: MonthCalendarButton
	{
		#region Class Properties
		[DefaultValue( DockStyle.Right )]
		public new DockStyle Dock
		{
			get
			{
				return base.Dock;
			}

			set
			{
				base.Dock = value;
			}
		}

		public new Point Location
		{
			get
			{
				return base.Location;
			}

			set
			{
				base.Location = value;
			}
		}
		#endregion

		#region Class Utility Methods
		private bool ShouldSerializeLocation()
		{
			if( this.Location.X == 120 && this.Location.Y == 0 )
				return false;
			else
				return true;
		}

		private bool ShouldSerializeSize()
		{
			if( this.Size.Width == 72 && this.Size.Height == 20 )
				return false;
			else
				return true;
		}

		private bool ShouldSerializeText()
		{
			if( this.Text == "None" )
				return false;
			else
				return true;
		}
		#endregion
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	[ToolboxItem( false )]
	public class MonthCalendarTodayButton: MonthCalendarButton
	{
		#region Class Properties
		[DefaultValue( DockStyle.Fill )]
		public new DockStyle Dock
		{
			get
			{
				return base.Dock;
			}

			set
			{
				base.Dock = value;
			}
		}
		#endregion

		#region Class Utility Methods
		private bool ShouldSerializeSize()
		{
			if( this.Size.Width == 120 && this.Size.Height == 20 )
				return false;
			else
				return true;
		}

		private bool ShouldSerializeText()
		{
			if( this.Text == "Today" )
				return false;
			else
				return true;
		}
		#endregion
	}
	#region Designer class
	public class MonthCalendarAdvDesigner: System.Windows.Forms.Design.ControlDesigner
	{
		public MonthCalendarAdvDesigner()
			: base()
		{

		}

		public override void Initialize( IComponent component )
		{
			base.Initialize( component );
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		System.ComponentModel.Design.DesignerActionListCollection actionLists;

		public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
		{
			get
			{
				if( null == actionLists )
				{
					actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
					actionLists.Add(
						new MonthCalendarAdvActionList( this.Component ) );
				}
				return actionLists;
			}
		}

#endif
	}
	#endregion

	#region ActionList class
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	public class MonthCalendarAdvActionList: SyncActionListBase<MonthCalendarAdv>
	{

		public MonthCalendarAdvActionList( IComponent component )
			: base( component )
		{
		}

		protected override void InitializeActionList()
		{
			this.AddDesignerActionHeaderItem( "Essential Tools - MonthCalendarAdv" );

			//Appearance category.
			this.AddDesignerActionHeaderItem( "Appearance" );
			this.AddDesignerActionPropertyItem( "Culture", "Culture", "Appearance", "Specfies the culture for the calendar." );
			this.AddDesignerActionPropertyItem( "Style", "Visual Style", "Appearance", "Specifies visual style for the control." );
			this.AddDesignerActionPropertyItem( "ThemedBorder", "ThemedBorder", "Appearance", "Specifies whether border should be themed." );
			this.AddDesignerActionPropertyItem( "ThemedEnabledScrollButtons", "ShowColumnHeader", "Appearance", "Specifies whether column header should be shown or not." );

			//Behavior Category
			this.AddDesignerActionHeaderItem( "Behavior" );
			this.AddDesignerActionPropertyItem( "AllowMultipleSelection", "AllowMultipleSelection", "Behavior", "Specifies the width of the dropdown." );
			this.AddDesignerActionPropertyItem( "FirstDayOfWeek", "First Day of Week", "Behavior", "Specfies the maximum number of items to be displayed in the combobox dropdown." );
			this.AddDesignerActionPropertyItem( "SelectedDates", "Selected Dates", "Behavior", "Specifies whether the text in the edit portion can be changed or not." );

		}

		[TypeConverter( typeof( SpecificCultureInfoTypeConverter ) )]
		public CultureInfo Culture
		{
			get
			{
				CultureInfo culture = CultureInfo.CurrentCulture;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					culture = control.Culture;
				}
				return culture;
			}
			set
			{
				SetValue( "Culture", value );
			}
		}

		public VisualStyle Style
		{
			get
			{
				VisualStyle style = VisualStyle.Default;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					style = control.Style;
				}
				return style;
			}
			set
			{
				SetValue( "Style", value );
			}
		}

		public bool ThemedBorder
		{
			get
			{
				bool themedBorder = true;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					themedBorder = control.ThemedBorder;
				}
				return themedBorder;
			}
			set
			{
				SetValue( "ThemedBorder", value );
			}
		}
		public bool ThemedEnabledScrollButtons
		{
			get
			{
				bool themedEnabledScrollButtons = true;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					themedEnabledScrollButtons = control.ThemedEnabledScrollButtons;
				}
				return themedEnabledScrollButtons;
			}
			set
			{
				SetValue( "ThemedEnabledScrollButtons", value );
			}
		}

		public bool AllowMultipleSelection
		{
			get
			{
				bool allowMultipleSelection = true;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					allowMultipleSelection = control.AllowMultipleSelection;
				}
				return allowMultipleSelection;
			}
			set
			{
				SetValue( "AllowMultipleSelection", value );
			}
		}

		public Day FirstDayOfWeek
		{
			get
			{
				Day day = Day.Default;
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					day = control.FirstDayOfWeek;
				}
				return day;
			}
			set
			{
				SetValue( "FirstDayOfWeek", value );
			}
		}

		public DateTime[] SelectedDates
		{
			get
			{
				DateTime[] dateTime = new DateTime[100];
				if( this.Control != null )
				{
					MonthCalendarAdv control = this.Control as MonthCalendarAdv;
					dateTime = control.SelectedDates;
				}
				return dateTime;
			}
			set
			{
				SetValue( "SelectedDates", value );
			}
		}


	}
#endif
	#endregion
}
