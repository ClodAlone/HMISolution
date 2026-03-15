//-------------------------------------------------------------------------------------------------
// <copyright file="NavigationCalender.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Schedule;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Schedule
{
	/// <summary>
	/// A class to display one or more calendars that can be used to select particular dates 
	/// or date ranges to be displayed in the ScheduleControl.
	/// </summary>
	/// <remarks>
	/// This class has DateValueChanging and DateValueChanged events that let you listen
	/// for changes in the date that determines which month is displayed in the top-most 
	/// calendar. 
	/// As you size this control, additional calendars with subsequent months will be
	/// displayed, allowing your user to see multiple months in this navigation tool.
	/// </remarks>
    [ToolboxItem(false)]
	public class NavigationCalendar : Panel, ISupportInitialize
	{
		private GridControl calendar; ////used to display the calendars and manage date selections
		
		////used in layout of calendars in the grid
		private int targetWidth = 0; ////set in the constructor
		private int targetHeight = 0; ////set in the constructor
		int dXPad = 5; ////left and right pad
		int dYPad = 5; ////top and bottom pad
		private int headerHeight = 16; ////height of the calendars headers
		internal int rowsPerCal = 8; ////number of grid rows per calendar
		internal int numberCalendars = 3; ////number of calendars in the grid
		internal ScheduleGrid scheduleGridGrid;

        private ScheduleAppearance appearance2 = null;
		internal ScheduleAppearance appearance
        {
            get 
            {
                if (appearance2 == null)
                {
                    ////design mode
                    appearance2 = new ScheduleAppearance(null);
                    this.ShowWeekNumbers = false;
                }

                return appearance2; 
            }

            set
            {
                appearance2 = value;
            }
        }  
		
		#region Constructor
		/// <summary>
		/// The class constructor.
		/// </summary>
		/// <remarks>
		/// NavigationCalendar is derived Panel that displays multiple calendars using
		/// a GridControl. You can use a derived GridControl by overriding the 
		/// CreateCalendarGrid method and returning an instance of your derived
		/// GrodControl.
		/// </remarks>
		public NavigationCalendar()
		{
			this.calendar = CreateCalendarGrid();
            this.scheduleGridGrid = new ScheduleGrid();
			#region layout calendar
			((System.ComponentModel.ISupportInitialize)this.calendar).BeginInit();
			this.SuspendLayout();

			//// 
			//// calendar
			////
			
			this.calendar.BaseStylesMap["Standard"].StyleInfo.Font.Facename = "Arial"; ////"Verdana";
			this.calendar.HScrollBehavior = GridScrollbarMode.Disabled;
			this.calendar.VScrollBehavior = GridScrollbarMode.Disabled;
			this.calendar.RowCount = (this.numberCalendars * rowsPerCal) - 1; //// + 2;
			this.calendar.ColCount = 7;
			this.calendar.Cols.Hidden[0] = true;
			this.calendar.AllowSelection = GridSelectionFlags.Any & ~GridSelectionFlags.Column & ~GridSelectionFlags.Row & ~GridSelectionFlags.Table;
			this.calendar.Model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
			this.calendar.ControllerOptions &= ~GridControllerOptions.OleDataSource;

			this.calendar.ActivateCurrentCellBehavior = GridCellActivateAction.None;
			this.calendar.Model.Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
			this.calendar.DefaultRowHeight = 15;
			this.calendar.SmoothControlResize = false;

			////hide the alpha blend color in the grid
			this.calendar.AlphaBlendSelectionColor = Color.FromArgb(0, 0, 0, 0);
			
			using (Graphics g = this.calendar.CreateGraphics())
			{
				this.targetWidth = (int)((15 * g.MeasureString("8", this.calendar.Font).Width) + (2 * this.pad));
				this.targetHeight = this.calendar.Model.RowHeights.GetTotal(0, this.calendar.RowCount);
			}
			
			this.calendar.Dock = System.Windows.Forms.DockStyle.None;
			this.calendar.Location = new System.Drawing.Point(Math.Max(dXPad, (this.Width - targetWidth) / 2), dYPad);
             
			this.calendar.Size = new Size(targetWidth, targetHeight);
			this.calendar.Name = "calendar";
			this.calendar.CellDrawn += new Syncfusion.Windows.Forms.Grid.GridDrawCellEventHandler(grid_CellDrawn);
            this.calendar.RightToLeftChanged += new EventHandler(calendar_RightToLeftChanged);
			
			this.calendar.Model.SelectionChanging += new GridSelectionChangingEventHandler(Model_SelectionChanging);
			this.calendar.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
			this.calendar.CellClick += new GridCellClickEventHandler(calendar_CellClick);
			this.calendar.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(calendar_QueryCoveredRange);
			this.calendar.QueryRowHeight += new GridRowColSizeEventHandler(calendar_QueryRowHeight);
			this.calendar.QueryColWidth += new GridRowColSizeEventHandler(calendar_QueryColWidth);
			this.calendar.QueryCellInfo += new GridQueryCellInfoEventHandler(calendar_QueryCellInfo);
			this.calendar.MouseDown += new MouseEventHandler(calendar_MouseDown);
			this.calendar.DrawCellDisplayText += new GridDrawCellDisplayTextEventHandler(calendar_DrawCellDisplayText);
			this.calendar.Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
			this.calendar.TableStyle.HorizontalAlignment = GridHorizontalAlignment.Center;
			this.calendar.TableStyle.TextMargins.Left = 0;
			this.calendar.TableStyle.TextMargins.Right = 0;
            this.calendar.CellMouseHover += new GridCellMouseEventHandler(calendar_CellMouseHover);
            this.calendar.CellMouseHoverLeave += new GridCellMouseEventHandler(calendar_CellMouseHoverLeave);
			this.Controls.Add(this.calendar);
			
			this.calendar.TabIndex = 0;

			////create a celltype to draw the edge cells
			this.calendar.CellModels.Add("EdgeCell", new EdgeCellModel(this.calendar.Model));
			((EdgeCellRenderer)this.calendar.CellRenderers["EdgeCell"]).navigationPanel = this;
            if (this.ShowWeekNumbers)
            {
                this.calendar.ColStyles[1].CellType = "EdgeCell";
            }
			#endregion

			((System.ComponentModel.ISupportInitialize)this.calendar).EndInit();
			this.ResumeLayout(false);

			this.calendar.Paint += new PaintEventHandler(calendar_Paint);
            
		}

        /// <summary>
        ///  Raises a event when the cell mouse hover leave.
        /// </summary>
        /// <param name="sender">Navigation calender</param>
        /// <param name="e">A <see cref="GridCellMouseEventArgs"/> with event data.</param>
        void calendar_CellMouseHoverLeave(object sender, GridCellMouseEventArgs e)
        {
            this.calendar.BeginUpdate();
            this.calendar.Invalidate();
            this.calendar.Update();
            this.calendar.EndUpdate(true);
        }

         /// <summary>
        ///  Raises a event when the cell mouse hover.
        /// </summary>
        /// <param name="sender">Navigation calender</param>
        /// <param name="e">A <see cref="GridCellMouseEventArgs"/> with event data.</param>
        void calendar_CellMouseHover(object sender, GridCellMouseEventArgs e)
        {
            GridStyleInfo style = this.calendar.GetViewStyleInfo(e.RowIndex, e.ColIndex);
            DateTime dt = new DateTime();
            if (DateTime.TryParse(style.CellValue.ToString(), out dt) && style.CellType != GridCellTypeName.Static && this.scheduleGridGrid.GridVisualStyles== GridVisualStyles.Metro)
            {
                Rectangle r = this.calendar.RangeInfoToRectangle(GridRangeInfo.Cell(e.RowIndex, e.ColIndex));
                if (e.RowIndex == this.CalenderGrid.Model.RowCount)
                {
                    if (e.ColIndex == 7)
                    {
                        r = new Rectangle(r.X, r.Y, r.Width - 10, r.Height - 2);
                    }
                    else if (e.ColIndex == 1)
                    {
                        r = new Rectangle(r.X + 13, r.Y, r.Width - 13, r.Height - 2);
                    }
                    else
                        r = new Rectangle(r.X, r.Y, r.Width, r.Height - 2);
                }
                else
                {
                    if (e.ColIndex == 7)
                    {
                        r = new Rectangle(r.X, r.Y, r.Width - 10, r.Height);
                    }
                    else if (e.ColIndex == 1)
                    {
                        r = new Rectangle(r.X + 13, r.Y, r.Width - 13, r.Height);
                    }
                }
                using (Pen p = new Pen(appearance.NavigationCalendarTodayColor))
                {
                    using (Graphics g = this.calendar.CreateGraphics())
                    {
                        g.DrawRectangle(p, r);
                    }
                }
            }
        }

		////needed to get the proper color on very first paint
		internal void calendar_Paint(object sender, PaintEventArgs e)
		{
            if (appearance == null)
            {
                return;
            }

			this.calendar.Paint -= new PaintEventHandler(calendar_Paint);
			this.calendar.Properties.BackgroundColor = appearance.NavigationCalendarBackColor;
			this.calendar.Invalidate();
			this.BackColor = appearance.NavigationCalendarBackColor; ////this.calendar.Properties.BackgroundColor;
		}

		#endregion

		#region Events
		/// <summary>
		/// A cancelable event raised before the <see cref="DateValue"/> property changes.
		/// </summary>
		public event DateValueChangingEventHandler DateValueChanging;

		/// <summary>
		/// A notification event raised after the <see cref="DateValue"/> property changed.
		/// </summary>
		public event DateValueChangedEventHandler DateValueChanged;
				
		#endregion

		#region Properties
		
		#region DateValue 

			private DateTime dateValue = DateTime.Now;

			////-used to offset display by a full week if MM/1/YYYY falls on a sunday
			////-see a display of October 2006-whole first row is from Sept
			////-this is being done to match outlook
			private int firstRowAdjustment = 0; 

			/// <summary>
			/// A property that gets/sets the date value for the navigation control.
			/// </summary>
			/// <remarks>
			/// This value is used to determine the month displayed in
			/// the topmost calendar (and hence the other calendars as
			/// well.) Before this value changes, the cancelable DateValueChanging
			/// is raised.
			/// </remarks>
			public DateTime DateValue
			{
				get
				{
					return dateValue;
				}

				set
				{
					if (dateValue != value && !OnDateValueChanging(dateValue, value))
					{
						BeginUpdate();
						
						dateValue = value;
                        this.firstRowAdjustment = dateValue.AddDays(-this.dateValue.Day + 1).DayOfWeek == FirstDayOfWeek ? -7 : 0;
                        if (this.scheduleGridGrid != null && this.SelectedDates.Count > 0 && this.scheduleGridGrid.ScheduleType != ScheduleViewType.Week)
						{
                            ////if(this.SelectedDates[0] < this.TopLeftDate.AddMonths(firstRowAdjustment == 0 ? 1 : 0).AddDays(-this.TopLeftDate.Day + 1))
                            if (this.SelectedDates[0] < this.DateValue.AddDays(-this.DateValue.Day + 1))
                            {
                                AdjustSelectionsByMonth(1);
                            }
                            else if (this.SelectedDates[this.SelectedDates.Count - 1] > this.BottomRightDate)
                            {
                                AdjustSelectionsByMonth(-1);
                            }
						}

						EndUpdate();
						OnDateValueChanged();
					}
				}
			}

		/// <summary>
		/// Sets the value of the DateValue property without raising DateValueChanging
		/// or DateValueChanged events.
		/// </summary>
		/// <param name="dt">The new date value.</param>
		/// <remarks>The property DateValue is used to determine the Month that appears
		/// in the top most calendar. All other dates in the calendars are determines by this month.
		/// This method allows the DateValue value to change without raising the normal change events.</remarks>
		public void SetDateValue(DateTime dt)
		{
			dateValue = dt;
		}

		/// <summary>
		/// Shifts selected dates by month offsets.
		/// </summary>
		/// <param name="offSet">The number of months by which the dates in SelectedDates will be adjusted.</param>
		public void AdjustSelectionsByMonth(int offSet)
		{
			if (offSet != 0)
			{
				if (this.scheduleGridGrid.ScheduleType == ScheduleViewType.Week)
				{
                    for (int i = 0; i < this.SelectedDates.Count; ++i)
                    {
                        this.SelectedDates[i] = ((DateTime)this.SelectedDates[i]).AddDays(offSet * 28);
                    }
				}
				else
				{
                    for (int i = 0; i < this.SelectedDates.Count; ++i)
                    {
                        this.SelectedDates[i] = ((DateTime)this.SelectedDates[i]).AddMonths(offSet);
                    }
				}
                if (this.scheduleGridGrid.ScheduleType == ScheduleViewType.Month)
                {
                    this.scheduleGridGrid.RefreshRange(GridRangeInfo.Row(0));
                }
				this.SelectedDates.OnSelectionsChanged(); ////raise event 
			}
		}

		/// <summary>
		/// Raises a cancelable event before a change to DateValue occurs.
		/// </summary>
		/// <param name="oldValue">Existing value of DateValue</param>
		/// <param name="newValue">Proposed value of DateValue</param>
		/// <returns>Whether or not the change should take place.</returns>
		protected virtual bool OnDateValueChanging(DateTime oldValue, DateTime newValue)
		{
			DateValueEventArgs e = new DateValueEventArgs(oldValue, newValue);
			if (DateValueChanging != null)
			{
				DateValueChanging(this, e);
			}

			return e.Cancel;
		}

		/// <summary>
		/// Raises a notification event that the DateValue has changed.
		/// </summary>
		protected virtual void OnDateValueChanged()
		{
			if (DateValueChanged != null)
			{
				EventArgs e = new EventArgs();
				DateValueChanged(this, e);               
			}

			return;
		}
			#endregion
		
		/// <summary>
		/// Gets the date value in the bottom-rightmost active-month cell of the navigation control.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime BottomRightDate
		{
			get
			{
				return this.DateValue.AddMonths(this.numberCalendars).AddDays(-this.DateValue.Day);
			}
		}

		/// <summary>
		/// Gets the date value in the top-left cell of the navigation control.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateTime TopLeftDate
		{
			get
			{
				return (DateTime) this.calendar.GetViewStyleInfo(2, 1, true).CellValue;
			}
		}

		#region Today

			private DateTime today = DateTime.MinValue; ////don't show it
			
            /// <summary>
			/// A property that gets or sets the DateTime value of the current day
			/// </summary>
			/// <remarks>
			/// By default, this value is set to DateTime.MinValue. When
			/// Today is DateTime.MinValue, the Today is not marked in the
			/// navigation calendars.
			/// </remarks>
			[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public DateTime Today
			{
                get
                {
                    return today;
                }

				set
				{
					today = value;
					this.Refresh();
				}
			}

			#endregion

		#endregion

		#region Panel overrides
		private int correction = 15; // 5 * 3  
		
        /// <summary>
		/// Used to position the GridControl in the panel.
		/// </summary>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

            this.calendar.Location = this.calendar.IsRightToLeft() 
                 ? new System.Drawing.Point(1, dYPad)
                 : new System.Drawing.Point(Math.Max(dXPad, (this.Width - targetWidth + correction) / 2), dYPad);
            
			int possibleCalendars = this.Height / (this.calendar.Model.RowHeights.GetTotal(0, 7) + 1);
			if (possibleCalendars > 1 && possibleCalendars != this.numberCalendars)
			{
				this.numberCalendars = possibleCalendars;
				this.calendar.RowCount = (this.numberCalendars * rowsPerCal) - 1;
				this.targetHeight = this.calendar.Model.RowHeights.GetTotal(0, this.calendar.RowCount);
			}
            if (possibleCalendars < 3 && this.calendar.GridVisualStyles == GridVisualStyles.Metro)
            {
                this.targetHeight = this.calendar.Model.RowHeights.GetTotal(0, this.calendar.RowCount);
            }
			this.calendar.Size = new Size(targetWidth, targetHeight);
		}
		#endregion

		#region Calendar grid creation / access

		/// <summary>
		/// Override this method to make this control use an
		/// instance of your derived GridControl class.
		/// </summary>
		/// <returns>The GridControl used to display the calendars.</returns>
		virtual public GridControl CreateCalendarGrid()
		{
			return new GridControl();
		}

		/// <summary>
		/// Returns the GridControl that is used to display the calendars.
		/// </summary>
		/// <returns>The GridControl.</returns>
		/// <remarks>
		/// Use this method to access the GridControl used for the calendars. For examples,
		/// you can get the date in the top-left most cell with this code.
		/// </remarks>
		/// <example>
		/// The following code changes border information for cells:
		/// <code lang="C#">
		///			//row 2 is the first row holding dates in the display grid
		///         DateTime dt = (DateTime) this.CalenderGrid[2,1].CellValue;
		/// </code>
		/// <code lang="VB">
		///         'row 2 is the first row holding dates in the display grid
		///         Dim dt as DateTime = Me.CalenderGrid(2,1).CellValue
		/// </code>
		/// </example>
		[////DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
		  Browsable(false)]
		public GridControl CalenderGrid
		{
            get
            {
                return this.calendar;
            }
		}

		#endregion

		#region Miscellaneous methods

		/// <summary>
		/// A method that returns the date of the Monday prior to the passed-in date.
		/// </summary>
		/// <param name="dt">The date passed-in.</param>
		/// <returns>The date of the Monday prior to the passed-in date.</returns>
		public DateTime MondayBeforeDate(DateTime dt)
		{
            while (dt.DayOfWeek != scheduleGridGrid.Schedule.Appearance.MonthCalendarStartDayOfWeek)
            {
                dt = dt.AddDays(-1);
            }

            return dt;
		}

		/// <summary>
		/// A method that returns the Sunday after the given date.
		/// </summary>
		/// <param name="dt">The date passed-in.</param>
		/// <returns>The Sunday following the given date.</returns>
		public DateTime SundayAfterDate(DateTime dt)
		{
			return (dt.DayOfWeek == DayOfWeek.Sunday) ? dt : dt.AddDays(7 - (int)dt.DayOfWeek);
		}

		/// <summary>
		/// A method that returns the date of the first day of month of the passed-in date.
		/// </summary>
		/// <param name="dt">The date passed-in.</param>
		/// <returns>The first day of the month containing the passed-in date.</returns>
		public DateTime FirstDayOfMonth(DateTime dt)
		{
			return dt.AddDays(-dt.Day + 1);
		}

		internal bool IsDateGridCell(int row, int col)
		{
			return  ////col > 0 && 
				!((col == 1 && row % rowsPerCal == 0)
			   ||(col > 0 && row % rowsPerCal == 1))
				&& row > 1;
		}

		private bool IsInActiveDate(int cal, DateTime dt)
		{
			int month = this.dateValue.AddMonths(cal).Month;
			return dt.Month != this.dateValue.AddMonths(cal).Month
				&& !(cal == 0 && dt.Month > month)
				|| (cal > 0 && cal < (this.numberCalendars - 1) && (dt.Month != month))
				|| (cal == (this.numberCalendars - 1) && (dt.Month < month));
		}

		////used to trigger button click paging
		////done in mouse down as it is more responsive than waiting for mouseup
		private void calendar_MouseDown(object sender, MouseEventArgs e)
		{
			int row, col;
			Point pt = new Point(e.X, e.Y);
			if (this.calendar.PointToRowCol(pt, out row, out col))
			{
				if (row == 0)
				{
                    //// 4 makes a bigger sweet spot
					if (e.X < buttonXOffset + buttonDX + 4)
					{
						this.DateValue = this.DateValue.AddMonths(-1);
					}
					else if (e.X > this.calendar.ColWidths.GetTotal(0, this.calendar.ColCount) - buttonXOffset - buttonDX - 4)
					{
						this.DateValue = this.DateValue.AddMonths(1);
					}

					this.calendar.Invalidate();
					this.calendar.Update();
				}
			}
		}

		private bool showWeekNumbers = true; ////false;

		/// <summary>
		/// A property that gets or sets whether week numbers should be displayed to the left of the navigation calendars.
		/// </summary>
		public bool ShowWeekNumbers
		{
            get
            {
                return showWeekNumbers;
            }

			set
			{
				if (showWeekNumbers != value)
				{
					showWeekNumbers = value;
					if (value)
					{
						this.calendar.ColStyles[1].CellType = "EdgeCell";
					}
					else
					{
						this.calendar.ColStyles[1].CellType = "Static";
					}

					this.calendar.Refresh();
				}
			}
		}

        void calendar_RightToLeftChanged(object sender, EventArgs e)
        {
            ////force location to be recomputed
            OnSizeChanged(EventArgs.Empty);
        }

		internal int weekNumberIndent = 12;

		int buttonDX = 5;
		int buttonXOffset = 8;
		int buttonHeight = 10;
		////handled to draw box around Today
		private void grid_CellDrawn(object sender, GridDrawCellEventArgs e)
		{
			GridControl grid = sender as GridControl;
			if (grid != null)
			{
				if (e.RowIndex == 0)
				{
					using (Brush b = new SolidBrush(appearance.NavigationCalendarArrowColor))
					{
						int dy = Math.Max(1, (e.Bounds.Height - this.buttonHeight) / 2);
						int x1 = e.Bounds.Location.X + buttonXOffset + buttonDX;
						int y1 = e.Bounds.Location.Y + dy;
						int x2 = e.Bounds.Location.X + buttonXOffset;
						int y2 = e.Bounds.Location.Y + dy + (buttonHeight / 2);
						int x3 = x1;
						int y3 = e.Bounds.Location.Y + dy + buttonHeight;
                        Point[] poygonPoints = new Point[] 
                        {
                            new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x1, y1)
                        };
                        e.Graphics.FillPolygon(b, poygonPoints);

						x1 = e.Bounds.Right - buttonXOffset - buttonDX;
						x2 = e.Bounds.Right - buttonXOffset;
						x3 = x1;
						y3 = e.Bounds.Location.Y + dy + buttonHeight;
                        poygonPoints = new Point[] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3), new Point(x1, y1) };
						e.Graphics.FillPolygon(b, poygonPoints);
					}
				}
				else if (IsDateGridCell(e.RowIndex, e.ColIndex) && this.scheduleGridGrid.GridVisualStyles!= GridVisualStyles.Metro)
				{
					DateTime dt = (DateTime)e.Style.CellValue;
					if (this.Today != DateTime.MinValue &&
						dt.Date == this.Today.Date)
					{
						int cal = e.RowIndex / rowsPerCal; ////integer division
								
						if (dt.Month == this.dateValue.AddMonths(cal).Month)
						{
							using (Pen p = new Pen(appearance.NavigationCalendarTodayColor))
							{
								Rectangle r = e.Bounds;
								r.Inflate(-1, -1);
								r.Width -= 1;
								e.Graphics.DrawRectangle(p, r);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Date Selections

		private DateSelections selectedDates;
		private DateTime lastMouseDownDate = DateTime.MinValue;
		private bool keepFullRows = false;

		/// <summary>
		/// A read-only property that gets the DateSelections collection, holding the dates selected in the navigation calendars.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DateSelections SelectedDates
		{
			get
			{
                if (selectedDates == null)
                {
                    selectedDates = new DateSelections();
                }

				return selectedDates;
			}
		}

		////returns whether control or shift is pressed
		private bool controlKeyDown
		{
			get
			{
				return (Control.ModifierKeys & Keys.Control) != Keys.None ||
					(Control.ModifierKeys & Keys.Shift) != Keys.None;
			}
		}
      
        internal DayOfWeek FirstDayOfWeek
        {
            get { return appearance.NavigationCalendarStartDayOfWeek; }
        }
        
        internal DayOfWeek LastDayOfWeek
        {
            get
            {
                return (DayOfWeek)(((int)FirstDayOfWeek + 6) % 7);
            }
        }

		////selection make use of 3 events. Only the mouseup either in Model_SelectionChanged
		//// or in calendar_CellClick are actually ahdnled in the ScheduleGrid. The ignoreSelectionChanged
		//// flag is used to tell ScheduleGrid when to ignore the changing.
		internal bool ignoreSelectionChanged = false;	
		//// catch mouseup during a selection and make sure the 
		//// selecteddates collection is sorted.
		private void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (this.scheduleGridGrid.Schedule == null)
            {
                return;
            }

            if (this.scheduleGridGrid.Schedule.DataSource == null)
            {
                return;
            }

			ignoreSelectionChanged = e.Reason == GridSelectionReason.MouseMove;
			//// This if-block is used to handle a single click 
			//// or the start of a drag selection.
			if (e.Reason == GridSelectionReason.MouseDown)
			{
				return;
			}
			else
			{
				if (e.Range != null)
				{
					DateTime dt;

                    if (lastMouseDownDate == DateTime.MinValue)
                    {
                        return;
                    }
					////find where the mouse is and process that point if it is over the calendar
					int mouserow, mousecol;
					if (this.calendar.PointToRowCol(this.calendar.PointToClient(Control.MousePosition), out mouserow, out mousecol))
					{
						dt = this.GetCellValue(mouserow, mousecol);
                        if (this.SelectedDates.Count > 0)
						{
							////create a new collection and add the dates
							////between the initial mousedown and the current mousedown
							bool refresh = false;
							DateSelections a = new DateSelections();
                            
                             while (dt < lastMouseDownDate)
                             {
                                 a.Add(dt);
                                 refresh = true;
                                 dt = dt.AddDays(1);
                             }

                             while (dt >= lastMouseDownDate)
                             {
                                    a.Add(dt);
                                    refresh = true;
                                    dt = dt.AddDays(-1);
                             }
                            
							////set the new collection into SelectedDates
							if (controlKeyDown)
							{
                                ////add to what is already there
								foreach (DateTime d in a)
								{
                                    if (this.SelectedDates.IndexOf(d) == -1)
                                    {
                                        this.SelectedDates.Add(d);
                                    }
								}
							}
							else
							{
                                ////add all at once
								this.SelectedDates.BeginUpdate();
								this.SelectedDates.Clear();
								this.SelectedDates.AddRange(a);
								this.SelectedDates.EndUpdate();
							}

							////check if need to expand the selection to full rows
							this.SelectedDates.Sort();
							int count = this.SelectedDates.Count;
							DateTime start = (DateTime)this.SelectedDates[0];
							DateTime end = (DateTime)this.SelectedDates[count - 1];
							if ((this.SelectedDates.Count > this.scheduleGridGrid.Schedule.Appearance.DayMonthCutoff || keepFullRows) ////8 is from Outlook
								&& !controlKeyDown && 
                                (this.scheduleGridGrid.ScheduleType != ScheduleViewType.Month
                                && this.scheduleGridGrid.ScheduleType != ScheduleViewType.Week))
							{
								keepFullRows = true;
								////fillin back to sunday
                                while (start.DayOfWeek != FirstDayOfWeek)
								{
									start = start.AddDays(-1);
									this.SelectedDates.Add(start);
									refresh = true;
								}
								////fillin forward to saturday
                                while (end.DayOfWeek != LastDayOfWeek)
								{
									end = end.AddDays(1);
									this.SelectedDates.Add(end);
									refresh = true;
								}
							}

							////refresh display if needed
							if (refresh)
							{
								this.calendar.Invalidate();
								this.calendar.Update();
                    		}
						}
					}
				}
			}
		}
        
		private void calendar_CellClick(object sender, GridCellClickEventArgs e)
        {
            if (this.scheduleGridGrid.Schedule == null)
            {
                return;
            }

            if (this.scheduleGridGrid.Schedule.DataSource == null)
            {
                return;
            }

			ignoreSelectionChanged = false;
			int row, col;
			Point pt = this.calendar.PointToClient(Control.MousePosition);
			if (this.calendar.PointToRowCol(pt, out row, out col))
			{
				if (!this.ShowWeekNumbers || col != 1 || pt.X >= this.weekNumberIndent)
				{
					this.SelectedDates.BeginUpdate();

					////only process if click is not on a header
					if (row % rowsPerCal > 1)
					{
						if (!controlKeyDown)
						{
							////clear existing selected dates
							if (this.SelectedDates.Count > 0)
							{
								this.SelectedDates.Clear();
							}
						}

						DateTime dt = GetCellValue(row, col);
						////reset class members
						this.lastMouseDownDate = dt;
						this.keepFullRows = false;

						////add the clicked date to the collection
						if (this.SelectedDates.IndexOf(dt) == -1)
						{
                            if (this.SelectedDates.Count == 0 && scheduleGridGrid.ScheduleType == ScheduleViewType.Week)
                            {
                                ////preserve the week for a single click
                                while (dt.DayOfWeek != this.appearance.NavigationCalendarStartDayOfWeek)
                                {
                                    dt = dt.AddDays(-1);
                                }

                                for (int i = 0; i < 7; i++)
                                {
                                    this.SelectedDates.Add(dt.AddDays(i));
                                }
                            }
                            else
                            {
                                this.SelectedDates.Add(dt);
                            }
						}
						else if (controlKeyDown)
						{
							this.SelectedDates.Remove(dt);
							this.SelectedDates.Add(dt);
						}
					}
                  
					this.SelectedDates.EndUpdate();
				}
               
			}
		}

		////-used to manage the date selection process - works with the SelectedDates collection
		////-is an event handler for the GridControl.SelectionsChanging event
		private void Model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
		{
			ignoreSelectionChanged = true;
			//// This if-block is used to handle a single click 
			//// or the start of a drag selection.
			if (e.Reason == GridSelectionReason.MouseDown)
			{
                if (e.ClickRange != null && !e.ClickRange.IsEmpty && !controlKeyDown)
                {
                    return; ////avoids raising SelectionsChanged event twice if you click on
                    ////on a date after having selected several dates previously.
                }

				int row, col;
				Point pt = this.calendar.PointToClient(Control.MousePosition);
				if (this.calendar.PointToRowCol(pt, out row, out col))
				{
					if (!this.ShowWeekNumbers || col != 1 || pt.X >= this.weekNumberIndent)
					{
						this.SelectedDates.BeginUpdate();

						////only process if click is not on a header
						if (row % rowsPerCal > 1)
						{
							if (!controlKeyDown)
							{
								////clear existing selected dates
								if (this.SelectedDates.Count > 0)
								{
									this.SelectedDates.Clear();
									this.calendar.Invalidate();
									this.calendar.Update();
								}
							}

							DateTime dt = GetCellValue(row, col);
							////reset class members
							this.lastMouseDownDate = dt;
							this.keepFullRows = false;

							////add the clicked date to the collection
							if (this.SelectedDates.IndexOf(dt) == -1)
							{
								this.SelectedDates.Add(dt);
							}
						}

						this.SelectedDates.EndUpdate();
					}
				}
			}
		}

		#endregion

		#region freeze drawing code

		/// <summary>
		/// A method that suspends the drawing of this NavigationCalendar instance.
		/// </summary>
		public void BeginUpdate()
		{
            if (!FreezePainting)
            {
                FreezePainting = true;
            }
		}

		/// <summary>
		/// A method that resumes drawing of this NavigationCalendar instance.
		/// </summary>
		public void EndUpdate()
		{
            if (FreezePainting)
            {
                FreezePainting = false;
            }
		}

		private const int WM_SETREDRAW = 0xB;
		private int paintFrozen = 0;

		[DllImport("User32")]
		private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		private bool FreezePainting 
		{
            get
            {
                return paintFrozen > 0;
            }

			set 
			{
				if (value && IsHandleCreated && this.Visible) 
				{
					if (0 == paintFrozen++) 
					{
						SendMessage(Handle, WM_SETREDRAW, 0, 0);
					}
				}

				if (!value) 
				{
					if (paintFrozen == 0)
					{
						return;
					}

					if (0 == --paintFrozen) 
					{
						SendMessage(Handle, WM_SETREDRAW, 1, 0);
						Invalidate(true);
					}
				}
			}
		}
		#endregion

		#region Grid Event Handlers

		////makes the header rows (holding the month name) look like one cell 
		////in the virtual grid displaying the calendars
		private void calendar_QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
		{
			if (e.RowIndex % rowsPerCal == 0)
			{
				e.Range = GridRangeInfo.Cells(e.RowIndex, 1, e.RowIndex, 7);
				e.Handled = true;
			}
		}

		////controls the header row heights (holding the month name) in 
		////the virtual grid displaying the calendars
		private void calendar_QueryRowHeight(object sender, GridRowColSizeEventArgs e)
		{
			if (e.Index % rowsPerCal == 0)
			{
                e.Size = (this.appearance2 != null && this.appearance.VisualStyle == GridVisualStyles.Metro) ? headerHeight + 4 : headerHeight;
			}
            else
                e.Size = (this.appearance2 != null && this.appearance.VisualStyle == GridVisualStyles.Metro) ? this.calendar.DefaultRowHeight + 4 : this.calendar.DefaultRowHeight;
            e.Handled = true; ;

		}

		////controls the widths of the columns in the virtual grid displaying the calendars
		private int pad = 10;
		private void calendar_QueryColWidth(object sender, GridRowColSizeEventArgs e)
		{
            if (e.Index > 0)
            {
                int w = (this.calendar.Width - (2 * pad)) / 7;
                w = (this.appearance2 != null && this.appearance.VisualStyle == GridVisualStyles.Metro) ? w + 2 : w;
                if (e.Index == 1)
                {
                    e.Size = w + pad + ((this.RightToLeft == RightToLeft.Yes && this.showWeekNumbers)? 15 : 0);
                }
                else if (e.Index == 7)
                {
                    e.Size = w + pad + ((this.RightToLeft == RightToLeft.Yes && !this.showWeekNumbers) ? 15 : 0);
                }
                else
                {
                    e.Size = w - 3; ////the minus three tightens the display like OutLook
                }

                e.Handled = true;
            }
		}
        bool navigated = true;
        public bool isCurrentMonth = true;
		////An event handler that provides the proper cell values and styles 
		////based on the value of e.RowIndex and e.ColIndex.
		private void calendar_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColIndex >= 0)
				{
					e.Style.BackColor = appearance.NavigationCalendarBackColor;
				}
            ////non header
			if (e.RowIndex % rowsPerCal != 0)
			{
               ////pad first and last col for non-headers
                if (e.ColIndex == 1)
                {
                    e.Style.TextMargins.Left = this.pad;
                }
                else if (e.ColIndex == 7)
                {
                    ////e.Style.TextMargins.Right = this.pad;
                    e.Style.TextMargins.Right = this.pad + ((this.RightToLeft == RightToLeft.Yes && !this.ShowWeekNumbers) ? 15 : 0);
                }
     		}
			else
			{
				////dont pad the headers
				if (e.ColIndex == 1)
				{
					e.Style.TextMargins.Left = 0;
				}
				else if (e.ColIndex == 7)
				{
					e.Style.TextMargins.Right = 0;
				}
			}

			if (e.ColIndex == 1 && e.RowIndex % rowsPerCal == 0)
			{
                ////handle the header row for each of the calendars
				e.Style.BackColor = appearance.NavigationCalendarHeaderColor;
				int cal = e.RowIndex / rowsPerCal;
				DateTime dt = this.dateValue.AddMonths(cal);
                dt = dt.AddDays(-dt.Day + 1);
				e.Style.Text = dt.ToString("MMMM yyyy", this.calendar.TableStyle.CultureInfo);
               	e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
				e.Style.CellType = "Static"; ////"Header";
				e.Style.Font.Bold = false;
                if (appearance.VisualStyle == GridVisualStyles.Metro)
                {
                    e.Style.Font.Facename = "Segoe UI";
                    e.Style.Font.Size = 9f;
                }
                e.Style.TextColor = (appearance.VisualStyle == GridVisualStyles.Metro) ? Color.White : appearance.NavigationCalendarTextColor;
                isCurrentMonth = dt.Month == this.Today.Month;
			}
		    else if (e.ColIndex > 0 && e.RowIndex % rowsPerCal == 1)
			{
                ////handle the first row after the headers
                if (this.calendar.Tag == null)
                {
                       this.calendar.Tag = GetFirstLettersOfDayOfWeek();
                }

                e.Style.CellValue = this.calendar.Tag.ToString().Substring(((e.ColIndex - 1 + (int)FirstDayOfWeek) % 7) * this.scheduleGridGrid.CharLength, this.scheduleGridGrid.CharLength);
				if (this.ShowWeekNumbers && e.ColIndex == 1)
				{
					e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.None);
				}
				else
				{
					e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, appearance.NavigationCalendarHeaderColor);
				}

				e.Style.TextColor = appearance.NavigationCalendarTextColor;
			}
			else if (e.RowIndex > 1 && e.ColIndex > 0)
			{
                //// cells with dates in them
				int cal = e.RowIndex / rowsPerCal;

				DateTime dt;
				e.Style.CellValue = dt = GetCellValue(e.RowIndex, e.ColIndex);

				int month = this.dateValue.AddMonths(cal).Month;
				if (dt.Month != this.dateValue.AddMonths(cal).Month)
				{
                    ////disable the text in these
					e.Style.TextColor = appearance.NavigationCalendarDisabledTextColor;
				}
				else
				{
                    ////normal text
					e.Style.TextColor = appearance.NavigationCalendarTextColor;
				}

				if ((!this.ShowWeekNumbers || e.ColIndex > 1) && this.SelectedDates.IndexOf(dt) > -1)
				{
                    ////mark if selected
					DateTime dtV = this.dateValue.AddMonths(cal);
					if (!((cal == 0 && (100 * dt.Year) + dt.Month > (100 * dtV.Year) + dtV.Month)
						|| (cal > 0 && cal < this.numberCalendars - 1 && dt.Month != dtV.Month)
						|| (cal == this.numberCalendars - 1 && (100 * dt.Year) + dt.Month < (100 * dtV.Year) + dtV.Month)))
					{
                        e.Style.BackColor = appearance.NavigationCalendarSelectionColor; ////this.SelectionColor;
					}
				}
			}
            
            DateTime dat = new DateTime();
            if (IsDateGridCell(e.RowIndex, e.ColIndex) && DateTime.TryParse(e.Style.CellValue.ToString(), out dat) 
                && this.scheduleGridGrid.GridVisualStyles == GridVisualStyles.Metro)
            {

                if (this.Today != DateTime.MinValue &&
                    dat.Date == this.Today.Date && isCurrentMonth)
                {
                    e.Style.BackColor = appearance.NavigationCalendarTodayBackColor;
                    e.Style.TextColor = appearance.NavigationCalendarTodayTextColor;
                }
            }
        }
        

        ////Utility code to handle case of initializing days of week for an empty datasource
        private string GetFirstLettersOfDayOfWeek()
        {
            DateTime dt = DateTime.ParseExact(@"10/23/2005", @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture); ////sunday
            string firstLetterOfDayOfWeek = string.Empty;
            for (int i = 0; i < 7; ++i)
            {
                firstLetterOfDayOfWeek += dt.AddDays(i).ToString("ddd", this.calendar.TableStyle.CultureInfo).Substring(0, this.scheduleGridGrid.CharLength); //// dt.AddDays(i).ToString("ddd", this.calendar.TableStyle.CultureInfo)[0];
            }

            return firstLetterOfDayOfWeek;
        }

		////A utility to return date at a particular row/col in 
		////the grid. row 2 is first row with dates.
		private DateTime GetCellValue(int row, int col)
		{
			////int cal = row < rowsPerCal ? 0 : (row < 2 * rowsPerCal) ? 1 : 2;
			int cal = row / rowsPerCal;
			DateTime dt = this.dateValue;
            DateTime dt1 = dt;
            int offSet = 0;
			switch (cal)
			{
				case 0:
                    dt = this.dateValue.AddDays(-this.dateValue.Day + 1);
                    if (dt.DayOfWeek < FirstDayOfWeek)
                    {
                        offSet = -7;
                    }

                    offSet += ((row - 2) * 7) + col - 1 - (int)dt.DayOfWeek + (int)FirstDayOfWeek + firstRowAdjustment;
                    dt1 = dt;
                    if (offSet > 0 || !this.DesignMode)
                    {
                        dt = dt.AddDays(offSet);
                    }

                  	break;			
				default:
                    dt = this.dateValue.AddMonths(cal).AddDays(-this.dateValue.Day + 1);
                    if (dt.DayOfWeek < FirstDayOfWeek)
                    {
                        offSet = -7;
                    }

                    dt = dt.AddDays(((row - 2 - (cal * this.rowsPerCal)) * 7) + col - 1 - (int)dt.DayOfWeek + (int)FirstDayOfWeek);
                    if (offSet > 0 || !this.DesignMode)
                    {
                        dt = dt.AddDays(offSet);
                    }

                   break;
			}

			return dt;
		}

		////Event handler used to conditionally display blanks above 
		////and below active dates in the 'interior' calendars. 
		private void calendar_DrawCellDisplayText(object sender, GridDrawCellDisplayTextEventArgs e)
		{
			if (IsDateGridCell(e.RowIndex, e.ColIndex))
			{
				int cal = e.RowIndex /rowsPerCal; ////e.RowIndex < rowsPerCal ? 0 : (e.RowIndex < 2 * rowsPerCal) ? 1 : 2;
				DateTime dt = (DateTime)e.Style.CellValue;
				DateTime dtV = this.dateValue.AddMonths(cal);
                if ((cal == 0 && (100 * dt.Year) + dt.Month > (100 * dtV.Year) + dtV.Month)
                    || (cal > 0 && cal < this.numberCalendars - 1 && dt.Month != dtV.Month)
                    || (cal == this.numberCalendars - 1 && (100 * dt.Year) + dt.Month < (100 * dtV.Year) + dtV.Month))
                {
                    e.DisplayText = " ";
                }
                else
                {
                    if (this.appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        e.Style.Font.Facename = "Segoe UI";
                        e.Style.Font.Size = 9f;
                    }
                    e.DisplayText = dt.Day.ToString();
                }
          	}
		}
		#endregion

		#region ISupportInitialize Members
		/// <summary>
		/// Empty ISupportInitialize.BeginInit implementation
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void BeginInit()
		{
			// TODO:  Add NavigationCalendar.BeginInit implementation
		}

		/// <summary>
		/// Empty ISupportInitialize.EndInit implementation
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void EndInit()
		{
			// TODO:  Add NavigationCalendar.EndInit implementation
		}

		#endregion
	}

	#region DateValueChanging/Changed support
	/// <summary>
	/// Represents a method that handles an event with <see cref="DateValueEventArgs"/> arguments 
	/// which is raised when a DateValue is about to change.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">Contains the event data.</param>
	public delegate void DateValueChangingEventHandler(object sender, DateValueEventArgs e);

	/// <summary>
	/// Represents a method that handles an event with <see cref="DateValueEventArgs"/> arguments 
	/// which is raised when a DateValue has changed.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">Contains the event data.</param>
	public delegate void DateValueChangedEventHandler(object sender, EventArgs e);

	/// <summary>
	/// Events arguments for the both the DateValueChanging
	/// and the DateValueChanged events.
	/// </summary>
	public class DateValueEventArgs : CancelEventArgs
	{
		private DateTime oldDate;
		private DateTime proposedDate;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="oldDate">The value of ScheduleControl.DateValue</param> 
		/// before the change.
		/// <param name="proposedDate">The value of ScheduleControl.DateValue
		/// after the change if the event is not cancelled.</param>
		public DateValueEventArgs(DateTime oldDate, DateTime proposedDate)
		{
			this.oldDate = oldDate;
			this.proposedDate = proposedDate;
		}

		/// <summary>
		/// The new value for ScheduleControl.DateValue.
		/// </summary>
		public DateTime ProposedDate
		{
            get
            {
                return this.proposedDate;
            }

            set
            {
                this.proposedDate = value;
            }
		}

		/// <summary>
		/// The existing value of ScheduleControl.DateValue.
		/// </summary>
		public DateTime OldDate
		{
            get
            {
                return this.oldDate;
            }

            set
            {
                this.oldDate = value;
            }
		}
	}
		
	#endregion

	#region Date Selections support

	/// <summary>
	/// Represents a method that handles an event  
	/// which is raised when ScheduleControl.Selections has changed.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">Contains the event data.</param>
	public delegate void SelectionsChangedEventHandler(object sender, EventArgs e);

	/// <summary>
	/// Holds a collection of DateTime objects.
	/// </summary>
	/// <remarks>
	/// This class hold the dates that are the currently selected dates for 
	/// the ScheduleControl. As your user clicks and drags in the NavigationControl
	/// panel, an instance of this class holds the currently selected dates.
	/// </remarks>
	public class DateSelections : ArrayList
	{
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DateSelections()
            : base()
        {
        }
        
		/// <summary>
		/// Returns the DateTime value at position i in this collection.
		/// </summary>
        /// <param name="i">The position used to retrieve the desired DateTime value.</param>
		public new DateTime this[int i]
		{
            get
            {
                return (DateTime)base[i];
            }

            set
            {
                base[i] = value;
            }
		}

		/// <summary>
		/// A method that conditionally adds an object to the collection.
		/// </summary>
		/// <param name="t">The object to be added.</param>
		/// <remarks>
		/// This collection will only hold unique occurrences of each date.
		/// This Add method hides the ArrayList.Add method to make
		/// sure no duplicates are added. It also raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public new void Add(object t)
		{
			if (base.IndexOf(t) == -1)
			{
				base.Add(t);
				OnSelectionsChanged();
			}
		}

		/// <summary>
		/// A method that adds a collection of objects to this collection.
		/// </summary>
		/// <param name="timeArray">The collection to be added.</param>
		/// <remarks>
		/// This method raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public new void AddRange(System.Collections.ICollection timeArray)
		{
			base.AddRange(timeArray);
			OnSelectionsChanged();
		}

		/// <summary>
		/// A method that clears this collection.
		/// </summary>
		/// <remarks>
		/// This method raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public new void Clear()
		{
			base.Clear();
			OnSelectionsChanged();
		}

		/// <summary>
		/// A method that removes a DateTime from this collection.
		/// </summary>
		/// <param name="dt">The date to be removed.</param>
		/// <remarks>
		/// This method raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public void Remove(DateTime dt)
		{
			base.Remove(dt);
			OnSelectionsChanged();
		}

		/// <summary>
		/// A method that removes a item from this collection at the index passed inAddControlToNavigationPanel.
		/// </summary>
		/// <param name="index">The index of the item to be removed.</param>
		/// <remarks>
		/// This method raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			OnSelectionsChanged();
		}

		/// <summary>
		/// Removes a range of items from this collection.
		/// </summary>
		/// <param name="index">The index of the first item to be removed.</param>
		/// <param name="count">The number of items to be removed.</param>
		/// <remarks>
		/// This method raises the 
		/// SelectionsChanged event.
		/// </remarks>
		public new void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			OnSelectionsChanged();
		}

		#region Events
		
        /// <summary>
		/// Occurs when items in the collection are added or removed.
		/// </summary>
		public event SelectionsChangedEventHandler SelectionsChanged;
		#endregion

		private int locked = 0;
		private bool changed = false;
		
        /// <summary>
		/// A method that suspends the collection from raising the <see cref="SelectionsChanged"/> event.
		/// </summary>
		/// <remarks>
		/// You can call this method before making a series of additions/removals 
		/// from this collection. This will prevent any listener to SelectionsChanged from
        /// seeing these changes. You can then use EndUpdate to resume the
		/// SelectionsChanged event being raised.
		/// </remarks>
		public void BeginUpdate()
		{
			locked++;
		}

		/// <summary>
		/// A method that resumes the raising of the <see cref="SelectionsChanged"/> event. 
		/// </summary>
		/// <param name="ignoreSelectionsChanged">False if you want the SelectionsChanged event
		/// to be raised to alert event listeners to the current state of the collection.
		/// </param>
		public void EndUpdate(bool ignoreSelectionsChanged)
		{
            if (ignoreSelectionsChanged)
            {
                changed = false;
            }

			EndUpdate();
		}

		/// <summary>
		/// A method that resumes the raising of the <see cref="SelectionsChanged"/> event. 
		/// </summary>
		public void EndUpdate()
		{
			locked--;
			if (locked == 0 && changed)
			{
				this.Sort();
				OnSelectionsChanged();
			}

			changed = false;
		}

		/// <summary>
		/// Raises the SelectionsChanged event.
		/// </summary>
		public virtual void OnSelectionsChanged()
		{
			if (SelectionsChanged != null && locked == 0)
			{
				EventArgs e = new EventArgs();
				SelectionsChanged(this, e);
				changed = false;
			}
			else
			{
				changed = true;
			}
		}
	}
	#endregion
}
