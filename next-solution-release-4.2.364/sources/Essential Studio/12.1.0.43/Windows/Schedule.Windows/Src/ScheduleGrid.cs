//-------------------------------------------------------------------------------------------------
// <copyright file="ScheduleGrid.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
#endif
using System.Windows.Forms;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Schedule
{
	/// <summary>
	/// A ScheduleGrid class illustrates the grid that holds the schedule items, possibly for multiple days.
	/// </summary>
	/// <remarks>
	/// ScheduleGrid is a GridControl derived class that serves as the container for
	/// the schedule items being displayed. Depending upon the ScheduleViewType
	/// being displayed, this grid may show a single day's items or multiple days' 
	/// items. It has class members to reference the navigation calendars.
	/// </remarks>
    [ToolboxItem(false)]
	public class ScheduleGrid : GridControl,IVisualStyle
    {
        #region constructors 

        /// <summary>
		/// Default constructor
		/// </summary>
		public ScheduleGrid()
		{
		}
        #region For Touch

        bool _touchMode = false;

        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
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
                    {
                        ApplyScaleToControl(1.5F);
                    }
                    else
                    {
                        ApplyScaleToControl(1);
                    }

                }
            }
        }
        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            this.BeginUpdate();
            this.SuspendLayout();
            if (sf == 1.5)
            {
                for (int i = 0; i < this.RowCount; i++)
                {
                    if (this.RowHeights[i] != 0)
                        this.RowHeights[i] += 5;
                }
                for (int i = 0; i < this.ColCount; i++)
                {
                    if (this.ColWidths[i] != 0)
                        this.ColWidths[i] += 15;
                }
            }
            else
            {
                for (int i = 0; i < this.RowCount; i++)
                {
                    if (this.RowHeights[i] != 0)
                        this.RowHeights[i] -= 5;
                }
                for (int i = 0; i < this.ColCount; i++)
                {
                    if (this.ColWidths[i] != 0)
                        this.ColWidths[i] -= 15;
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.EndUpdate();
        }
        #endregion

        private DateTime scheduleGridDate;

		/// <summary>
		/// Constructor that initializes controls needed by the ScheduleGrid.
		/// </summary>
		/// <param name="calendar">The <see cref="Calendar"/> used to determine the dates displayed.</param>
		/// <param name="schedule">Gets the <see cref="ScheduleControl"/> that hosts this ScheduleGrid.</param>
		/// <param name="theDate">The date used to set the initial display.</param>
		public ScheduleGrid(NavigationCalendar calendar, ScheduleControl schedule, DateTime theDate) 
        {
             this.scheduleGridDate = theDate;

             if (this.DesignMode)
             {
                 return;
             }

			this.calendar = calendar;
			this.schedule = schedule;
			this.calendar.scheduleGridGrid = this;
			this.calendar.BackColor = this.Schedule.Appearance.NavigationCalendarBackColor;
			this.calendar.scheduleGridGrid.Properties.BackgroundColor = this.Schedule.Appearance.NavigationCalendarBackColor;
            this.calendar.CalenderGrid.QueryCellInfo += new GridQueryCellInfoEventHandler(CalenderGrid_QueryCellInfo);
			this.calendar.appearance = this.Schedule.Appearance;

            ApplyVisualStyle();

			SetUpDayOfWeekStrings(); ////init some strings
 
			this.Calendar.Today = DateTime.Now;
				
			this.ScheduleType = schedule.ScheduleType;
			if (this.Calendar.SelectedDates.Count < 1)
			{
				switch (this.ScheduleType)
				{
					case ScheduleViewType.Day:
					case ScheduleViewType.CustomWeek:
						this.Calendar.SelectedDates.Add(theDate);
						if (theDate > this.Calendar.BottomRightDate
							|| theDate < this.Calendar.TopLeftDate)
						{
							this.Calendar.DateValue = theDate;
						}

						break;
					case ScheduleViewType.Month:
						theDate = theDate.AddDays(-theDate.Day + 1);
						int month = theDate.Month;
						this.Calendar.SelectedDates.BeginUpdate();
						while (theDate.Month == month)
						{
							this.Calendar.SelectedDates.Add(theDate);
							theDate = theDate.AddDays(1);
						}

						this.Calendar.SelectedDates.EndUpdate();
						if (theDate > this.Calendar.BottomRightDate
							|| theDate < this.Calendar.TopLeftDate)
						{
							this.Calendar.DateValue = theDate;
						}

						break;
					case ScheduleViewType.Week:
					case ScheduleViewType.WorkWeek:
                        this.AllowSelection = GridSelectionFlags.Any;
                        DayOfWeek dayOfWeek = this.ScheduleType == ScheduleViewType.WorkWeek ? DayOfWeek.Monday : schedule.Appearance.WeekCalendarStartDayOfWeek;
                        while (theDate.DayOfWeek != dayOfWeek)
                        {
                            theDate = theDate.AddDays(-1);
                        }

                        int count = this.ScheduleType == ScheduleViewType.Week ? 7 : 5;
                        this.Calendar.SelectedDates.BeginUpdate();
                        while (count > 0)
                        {
                            this.Calendar.SelectedDates.Add(theDate);
                            theDate = theDate.AddDays(1);
                            count--;
                        }

                        this.Calendar.SelectedDates.EndUpdate();
                        if (theDate > this.Calendar.BottomRightDate
                            || theDate < this.Calendar.TopLeftDate)
                        {
                            this.Calendar.DateValue = theDate;
                        }

                        break;
					default:
						break;
				}
			}

			this.displayDates = SetRangeInCalendar();
			this.numberPanels = this.displayDates.GetLength(0);
			this.Calendar.SelectedDates.SelectionsChanged += new SelectionsChangedEventHandler(SelectedDates_SelectionsChanged);
			this.Schedule.previousButton.Click += new EventHandler(previousButton_Click);
			this.Schedule.nextButton.Click += new EventHandler(nextButton_Click);
            if (this.schedule.ParentForm != null)
                this.schedule.ParentForm.ResizeEnd += new EventHandler(ParentForm_ResizeEnd);
            this.minutesPerDivision = 60 / this.Schedule.Appearance.DivisionsPerHour;
          	SetHeaderLabel();			
			this.Name = "ScheduleGrid";
			this.Size = new System.Drawing.Size(640, 448);
			////this.ThemesEnabled = true;
			this.ResumeLayout(false);
        }


        /// <summary>
        /// Event raised when the form exits resizing mode.
        /// </summary>
        /// <param name="sender">ScheduleGrid</param>
        /// <param name="e">A <see cref="EventArgs"/> with event data.</param>
        void ParentForm_ResizeEnd(object sender, EventArgs e)
        {
            if (this.schedule.Appearance.AllowProportionalColumnSizing && this.ScheduleType == ScheduleViewType.WorkWeek)
            {
                this.BeginUpdate();
                this.Calendar.DateValue = this.Calendar.SelectedDates[0];
                AdjustSelectedDatesByWeek();
                this.EndUpdate();
            }
        }

        /// <summary>
        ///  Raises a event when the cell mouse hover leave.
        /// </summary>
        /// <param name="sender">Navigation calender</param>
        /// <param name="e">A <see cref="GridCellMouseEventArgs"/> with event data.</param>
        protected override void OnCellMouseHoverLeave(GridCellMouseEventArgs e)
        {
            this.BeginUpdate();
            this.Invalidate();
            this.Update();
            this.EndUpdate(true);
            base.OnCellMouseHoverLeave(e);
        }

        /// <summary>
        /// Used to Draw a triangle.
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="fillBrush">fill color of the triangle</param>
        /// <param name="border">border color of the triangle</param>
        private void DrawTriangle(Graphics g, Rectangle rect, Brush fillBrush, Brush border)
        {
            int halfWidth = rect.Width / 2;
            int halfHeight = rect.Height / 2;
            Point p0 = Point.Empty;
            Point p1 = Point.Empty;
            Point p2 = Point.Empty;
            switch (this.Schedule.Appearance.MoreItemsArrowDirection)
            {
                case ScheduleAppearance.MoreItemsArrowDirections.Up:
                    p0 = new Point(rect.Left + halfWidth, rect.Top);
                    p1 = new Point(rect.Left, rect.Bottom);
                    p2 = new Point(rect.Right, rect.Bottom);
                    break;
                case ScheduleAppearance.MoreItemsArrowDirections.Left:
                    p0 = new Point(rect.Left, rect.Top + halfHeight);
                    p1 = new Point(rect.Right, rect.Top);
                    p2 = new Point(rect.Right, rect.Bottom);
                    break;
                case ScheduleAppearance.MoreItemsArrowDirections.Right:
                    p0 = new Point(rect.Right, rect.Top + halfHeight);
                    p1 = new Point(rect.Left, rect.Bottom);
                    p2 = new Point(rect.Left, rect.Top);
                    break;
                default:
                    p0 = new Point(rect.Left + halfWidth, rect.Bottom);
                    p1 = new Point(rect.Left, rect.Top);
                    p2 = new Point(rect.Right, rect.Top);
                    break;
            }
            g.FillPolygon(fillBrush, new Point[] { p0, p1, p2 });
            g.DrawPolygon(new Pen(border), new Point[] { p0, p1, p2 });
        }
        void CalenderGrid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (this != null && this.scheduleDataList != null)
            {
                DateTime dt;
                if (DateTime.TryParse(e.Style.CellValue.ToString(), out dt))
                {
                    if (scheduledDateList.Contains(dt.Date))
                        e.Style.Font.Bold = true;
                }
            }
        }

        #endregion

        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (ScheduleAppointmentToolTip != null)
				{
					ScheduleAppointmentToolTip.Active = false;
					ScheduleAppointmentToolTip.Dispose();
					ScheduleAppointmentToolTip = null;
				}
                if (customTip != null)
                {
                    customTip.Active = false;
                    customTip.Dispose();
                    customTip = null;
                }
	
                if (!this.Calendar.IsDisposed)
				{
					UnwireEvents(wiredType);
					this.Schedule.previousButton.Click -= new EventHandler(previousButton_Click);
					this.Schedule.nextButton.Click -= new EventHandler(nextButton_Click);
					this.Calendar.SelectedDates.SelectionsChanged -= new SelectionsChangedEventHandler(SelectedDates_SelectionsChanged);
				}
			}

			base.Dispose(disposing);
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

        #region allday span support

#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
        private Dictionary<int, List<TranparentLabel>> allDaySpanAppointments = new Dictionary<int, List<TranparentLabel>>();
#else
        private Hashtable allDaySpanAppointments = new Hashtable();
#endif

        private SpanLayoutManager spanManager = null;

        internal SpanLayoutManager SpanManager
        {
            get 
            {
                if (spanManager == null)
                {
                    spanManager = new SpanLayoutManager(this.schedule);
                }

                return spanManager; 
            }
        }

        private void ClearAllDaySpans()
        {
            foreach (List<TranparentLabel> lt in allDaySpanAppointments.Values)
            {
                foreach (TranparentLabel l in lt)
                {
                    l.DoubleClick -= new EventHandler(l_DoubleClick);
                    l.MouseDown -= new MouseEventHandler(l_MouseDown);
                    l.MouseLeave -= new EventHandler(l_MouseLeave);
                    l.MouseEnter -= new EventHandler(ScheduleGrid_MouseEnter);
                    if (this.Controls.Contains(l))
                    {
                        this.Controls.Remove(l);
                    }
                }

                lt.Clear();
            }

            mouseMoveLabel = null;      
            allDaySpanAppointments.Clear();
            SpanManager.Clear();
        }

        internal ArrayList scheduledDateList = new ArrayList();
        internal void GetScheduledDates()
        {
            scheduledDateList.Clear();
            GridStyleInfo firstDateStyle = this.calendar.CalenderGrid.GetViewStyleInfo(2, 1);
            GridStyleInfo lastDatestyle = this.Calendar.CalenderGrid.GetViewStyleInfo(this.calendar.CalenderGrid.Model.RowCount, this.calendar.CalenderGrid.ColCount);
            if (this.dataProvider != null)
            {
                IScheduleAppointmentList sp = this.dataProvider.GetSchedule(DateTime.Parse(firstDateStyle.CellValue.ToString()), DateTime.Parse(lastDatestyle.CellValue.ToString()));
                foreach (IScheduleAppointment item in sp)
                {
                    DateTime dt;
                    if (DateTime.TryParse(item.StartTime.ToShortDateString(), out dt))
                    {
                        if (!scheduledDateList.Contains(dt.Date))
                            scheduledDateList.Add(dt.Date);
                    }
                }
            }
            this.calendar.Invalidate();
        }

        private Color labelBackColor = Color.LightGoldenrodYellow;

        private bool AddAllDaySpan(Rectangle originalRect, Rectangle rect, IRecurringScheduleAppointment appointment, bool lastColumn, GridStyleInfo style)
        {
            bool b = false;

            if (schedule.ShowMultiDayAppointmentsAsSpans &&
                (appointment.AllDay || schedule.ScheduleType == ScheduleViewType.Month || schedule.ShowAllSpansInAllDayPanel)
                   && !allDaySpanAppointments.ContainsKey(appointment.RecurrenceRuleID))
            {
                b = true;

                DateTime lastDisplayDate = this.displayDates[this.displayDates.GetLength(0) - 1];
                IScheduleDataProvider provider = schedule.DataSource as IScheduleDataProvider;
                bool isMonthView = ScheduleType == ScheduleViewType.Month;

                TranparentLabel l = new TranparentLabel(this, appointment); ////new TranparentLabel();
                l.AutoSize = false;
                List<TranparentLabel> lt = new List<TranparentLabel>();
                lt.Add(l);
                allDaySpanAppointments.Add(appointment.RecurrenceRuleID, lt);
                DateTime firstDate = appointment.DateList.BaseDate.Date;
                while (isMonthView && firstDate < this.displayDates[0])
                {
                    firstDate = firstDate.AddDays(1);
                }
                ////keep track of where this panel should go in the display...
                int pos = SpanManager.GetFreeSlot(firstDate);
                SpanManager.MarkSlotRange(firstDate.AddDays(0), appointment.DateList.TerminalDate.Date, pos);
                l.ScreenSlot = pos;

                int days;
                if (isMonthView)
                {
                    days = 1;
                    DateTime d = appointment.StartTime.Date;
                    DateTime start = d;

                    int dayOffSet = 0;
                    int dayOffSetWidth = 0;

                    DateTime d1 = d;
                    int extraRows = 0;

                    ////get the number of days back to the start of the week
                    while (d1.AddDays(-dayOffSet).DayOfWeek != schedule.Appearance.MonthCalendarStartDayOfWeek)
                    {
                        dayOffSet++;
                    }

                    if (dayOffSet > 0)
                    {
                        dayOffSetWidth = this.ColWidths.GetTotal(1, dayOffSet);
                    }

                    int rectWidthForOneDay = this.ColWidths[dayOffSet + 1]; //// rect.Width;
                    rect.Width = rectWidthForOneDay;
                    Rectangle saveRect = rect;
                    bool spanForOneDay = true;
                    while (d < lastDisplayDate.Date && d <= appointment.DateList.TerminalDate.Date && spanForOneDay)
                    {
                        ////count the number of days in the span panel for this one week
                        int sumWidth = this.ColWidths[dayOffSet + days];
                        while (d < lastDisplayDate.Date && d < appointment.DateList.TerminalDate.Date
                                && d.DayOfWeek != LastDayOfWeekMonthCalendar)
                        {
                            if (d != appointment.DateList.TerminalDate.Date)
                            {
                                d = d.AddDays(1);
                                days++;
                                sumWidth += this.ColWidths[dayOffSet + days];
                            }
                            else
                            {
                                spanForOneDay = false;
                            }
                        }

                        int width = sumWidth - 3;
                        if (schedule.RightToLeft == RightToLeft.Yes)
                        {
                            if (extraRows == 0)
                            {
                                rect.X = rect.X + rectWidthForOneDay - width - 3;
                            }
                            else
                            {
                                rect.X = this.Width - rect.X - width + 1;
                                rect.Width = width;
                            }
                        }

                        if (extraRows > 0)
                        {
                            originalRect.Offset(0, extraRows * originalRect.Height);
                            extraRows = 0;
                        }

                        PopulateTranparentLabel(originalRect, l, rect, width, appointment, d, start);
                        //// code inside this if handles if the span wraps to the next row of the calendar 
                        ////if so, need to add another panel in the proper position of the next row
                        if (d < lastDisplayDate.Date && d <= appointment.DateList.TerminalDate.Date)
                        {
                            ////need to add another span panel in the following row to continnue the span across rows
                            l = new TranparentLabel(this, appointment);
                            l.AutoSize = false;
                            lt.Add(l);
                            days = 1;
                            d = d.AddDays(1);
                            start = d;
                            extraRows++;

                            ////keep track of where this panel should go in the display...
                            int pos1 = SpanManager.GetFreeSlot(d.Date);
                            SpanManager.MarkSlotRange(d.Date.AddDays(1), appointment.DateList.TerminalDate.Date, pos1);
                            l.ScreenSlot = pos1;

                            ////find the rectangle in the grid
                            GridRangeInfo range = this.CoveredRanges.FindRange(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex);
                            if (range.IsEmpty)
                            {
                                range = GridRangeInfo.Cell(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex);
                            }

                            Rectangle r1 = this.RangeInfoToRectangle(range);

                            ////move rectangle back to start of week
                            rect.Offset((extraRows == 1 && schedule.RightToLeft != RightToLeft.Yes) ? (-dayOffSetWidth) : 0, extraRows * r1.Height);
                            dayOffSetWidth = 0;
                            ////now figure out how much to offest it down - need to find the same recurrence appointment
                            ////in the next row....
                            int offset = 0;
                            Point pt = saveRect.Location;
                            pt.Offset(2, 2); ////this is a point inside where a matching appointment might be

                            ////now we need to check to see if the point under pt is the same recurrence appointment
                            ItemHitType mouseDownHitType;
                            IRecurringScheduleAppointment item = this.GetItemAtPoint(pt, out mouseDownHitType) as IRecurringScheduleAppointment;
                            while (pt.Y > r1.Bottom && (item == null || appointment.RecurrenceRuleID != item.RecurrenceRuleID))
                            {
                                offset++;
                                pt.Offset(0, -rect.Height - 2);
                                item = this.GetItemAtPoint(pt, out mouseDownHitType) as IRecurringScheduleAppointment;
                            }

                            ////once we find it, offset the rect so it sits over the same appointment on the new day in the next row
                            if (offset > 0)
                            {
                                rect.Offset(0, -offset * (rect.Height + 2));
                            }
                        }

                        dayOffSet = 0;
                    }
                }
                else
                {
                    ////Day, WorkWeek or CustomWeek view....
                    days = (int)(Math.Min(lastDisplayDate.Date.ToOADate(), appointment.DateList.TerminalDate.Date.ToOADate()) - appointment.StartTime.Date.ToOADate() + 1);
                    rect.Height -= 3; //// 1;

                    if (schedule.RightToLeft == RightToLeft.Yes)
                    {
                        rect.X = rect.X - (rect.Width - markWidth - 1) + allDayGrids[0].Width;
                    }

                    DateTime d = appointment.StartTime.Date;
                    PopulateTranparentLabel(originalRect, l, rect, rect.Width - 6, appointment, d.AddDays(days - 1), d);
                }
            }

            return b;
        }
        void PopulateTranparentLabel(Rectangle originalRect, TranparentLabel l, Rectangle rect, int width, IRecurringScheduleAppointment appointment, DateTime d, DateTime start)
        {
            ////set the location and size
            Point pt = new Point(rect.X, originalRect.Location.Y);
            GridRangeInfo range = this.PointToRangeInfo(pt);
            Rectangle pt2;
            if (!schedule.Appearance.MonthShowFullWeek && range.Left != (int)DayOfWeek.Saturday && l.Item.StartTime.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime startofMonthCalendar = this.Calendar.MondayBeforeDate(this.Calendar.SelectedDates[0]);
                int indexToCalc = ((appointment.DateList.BaseDate.Date - startofMonthCalendar.Date).Days / 7) * 2 + 1;
                pt2 = this.RangeInfoToRectangle(GridRangeInfo.Cell((range.Bottom) + (range.Bottom - indexToCalc), range.Left), GridRangeOptions.None);
                pt.Y = pt2.Location.Y + 1;
            }

            pt.Offset(0, ((l.ScreenSlot + 1) * (rect.Height + 2)) + 2);
           
            l.Location = pt;
            l.Height = rect.Height + 1;
            l.Width = width + 1;
            
            ////set the display text to the left and right of this span panel
            if ((int)(d.Date.ToOADate() - appointment.DateList.TerminalDate.Date.ToOADate()) < 0)
            {
                l.RightText = DisplayStrings[_To] + this.ParseDisplayItem(appointment, this.schedule.Appearance.SpanItemFormatTerminalRightText); ////appointment.DateList.TerminalDate.ToString(schedule.Appearance.WeekHeaderFormat, this.Schedule.Culture);
            }
            else
            {
                IRecurringScheduleAppointment temp = appointment.Clone() as IRecurringScheduleAppointment;
                temp.EndTime = appointment.DateList.TerminalDate;
                l.RightIText = /*appointment.AllDay ? string.Empty :*/ this.ParseDisplayItem(temp, this.schedule.Appearance.SpanItemFormatRightText);
            }

            if ((int)(start.Date.ToOADate() - appointment.DateList.BaseDate.Date.ToOADate()) > 0)
            {
                if (appointment.DateList.BaseDate.Date != appointment.StartTime.Date)
                {
                    IRecurringScheduleAppointment temp = appointment.Clone() as IRecurringScheduleAppointment;
                    temp.StartTime = appointment.DateList.BaseDate;
                    l.LeftText = DisplayStrings[_From] + this.ParseDisplayItem(temp, this.schedule.Appearance.SpanItemFormatTerminalLeftText); ////appointment.DateList.BaseDate.ToString(schedule.Appearance.WeekHeaderFormat, this.Schedule.Culture);
                }
                else
                {
                    l.LeftText = DisplayStrings[_From] + this.ParseDisplayItem(appointment, this.schedule.Appearance.SpanItemFormatTerminalLeftText); ////appointment.DateList.BaseDate.ToString(schedule.Appearance.WeekHeaderFormat, this.Schedule.Culture);
                }
            }
            else
            {
                l.LeftIText = /*appointment.AllDay ? string.Empty : */this.ParseDisplayItem(appointment, this.schedule.Appearance.SpanItemFormatLeftText);
            }

            ////set other properties
            l.BackColor = schedule.Appearance.AllDayBackColor;
            l.BoxColor = schedule.DataSource.GetLabels()[appointment.LabelValue].ColorMember;
            l.MarkerColor = schedule.DataSource.GetLabels()[appointment.MarkerValue].ColorMember;
            l.Schedule = schedule;
            l.Text = this.ParseDisplayItem(appointment, this.schedule.Appearance.SpanItemFormatMiddleText);
            ////add it to the grid's controls...
            this.Controls.Add(l);

            l.BringToFront();
            l.DoubleClick += new EventHandler(l_DoubleClick);
            l.MouseDown += new MouseEventHandler(l_MouseDown);
            l.MouseLeave += new EventHandler(l_MouseLeave);
            l.MouseEnter += new EventHandler(ScheduleGrid_MouseEnter);
        }

        ////used for hittesting...
        TranparentLabel mouseMoveLabel = null;
        
        void ScheduleGrid_MouseEnter(object sender, EventArgs e)
        {
            mouseMoveLabel = sender as TranparentLabel;
        }
        
        void l_MouseLeave(object sender, EventArgs e)
        {
            mouseMoveLabel = null;
        }

        void l_MouseDown(object sender, MouseEventArgs e)
        {
            TranparentLabel l = sender as TranparentLabel;
            mouseDownPoint = l.Location;
            mouseDownPoint.Offset(0, 2);
            mouseDownItem = l.Item;
        }

        ////catch a double click on panel and do an edit
        void l_DoubleClick(object sender, EventArgs e)
        {
            ////raise a cancelable event
            ScheduleAppointmentClickType t = ScheduleAppointmentClickType.LeftDblClick;
            ScheduleAppointmentClickEventArgs args = new ScheduleAppointmentClickEventArgs(t, this.mouseDownItem, this.ScheduleType, ClickDateTime(this.mouseDownRow, this.mouseDownCol));
            this.Schedule.OnScheduleAppointmentClick(args);
            if (args.Cancel)
            {
                return;
            }

            EditItemAtClick();
        }

        #endregion

        #region GridControl overrides
        
        /// <summary>
        /// Overriden to make sure no horizontal scrollbar is seen after sizing...
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override void UpdateScrollBars()
        {
            this.HScroll = false;
            base.UpdateScrollBars();
        }
        
        /// <summary>
		/// Overridden to center text at the top of the grid.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnSizeChanged(EventArgs e)
		{
           base.OnSizeChanged(e);
           //int x = 1;
           //x = (this.Width - this.Schedule.captionPanel.Width) / 2;
           //this.Schedule.captionPanel.DockPadding.Left = x;
           //this.Schedule.captionPanel.DockPadding.Right = x;
           //smallColWidth = -1;

            ClearAllDaySpans();
            if (Control.MouseButtons != MouseButtons.Left)
            {
                this.Refresh();
            }
        }

        ArrayList moreItemsCells = new ArrayList();
        
        /// <summary>
        /// Overridden to support displaying appointments with rounded corners
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        {
            if (moreItemsCells.IndexOf((1000 * e.RowIndex)+ e.ColIndex) > -1)
            {
                moreItemsCells.Remove(1000 * e.RowIndex + e.ColIndex);
            }

            if (!schedule.ShowRoundedCorners && !schedule.ShowMultiDayAppointmentsAsSpans)
            {
                base.OnDrawCellDisplayText(e);
                return;
            }

            ////handle drawing rounded corners...
            int col = e.Style.CellIdentity.ColIndex;
            int row = e.Style.CellIdentity.RowIndex;

            ////handle the monthview...
            if (this.ScheduleType == ScheduleViewType.Month || ScheduleType == ScheduleViewType.Week)
            {
                string[] list = e.DisplayText.Split(new char[] { '\n' });
                int n = list.GetLength(0) - 1;
                if (e.ColIndex == 1 && e.RowIndex == 1 && ScheduleType != ScheduleViewType.Week && this.schedule.Culture.TextInfo.IsRightToLeft)
                {
                    Color c1;
                    if (((this.displayDates[0].Day < this.displayDates[1].Day) && (this.displayDates[0].Day != 1)) || (this.displayDates[1].Day == 1))
                    {
                        c1 = this.Schedule.Appearance.NonPrimeTimeCellColor;
                    }
                    else
                    {
                        c1 = this.Schedule.Appearance.PrimeTimeCellColor;
                    }
                    using (Brush b = new SolidBrush(c1))
                    {
                        Rectangle rect2 = e.TextRectangle;
                        rect2.Width += 2;
                        rect2.X -= 1;
                        rect2.Height += 2;
                        rect2.Y -= 1;
                        e.Graphics.FillRectangle(b, rect2);
                    }

                    ThemedHeaderDrawing.HeaderState headerState = this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex)
                       ? ThemedHeaderDrawing.HeaderState.Pressed : ThemedHeaderDrawing.HeaderState.Normal;
                    int alpha = headerState == ThemedHeaderDrawing.HeaderState.Pressed ? 255 : 128;
                    Rectangle rect = e.TextRectangle;
                    int h = (int)(e.Style.Font.Size + 1);
                    rect.Height = 2 * h - 1;
                    //// draw back ground
                    Rectangle rect1 = e.TextRectangle;
                    rect1.X -= 1;
                    rect1.Width += 1;
                    rect1.Height = 2 * h - 1;
                    if (this.Schedule.Appearance.ThemesEnabled)
                    {
                        if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.SystemTheme)
                        {
                            using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderForeColor), Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderBackColor), System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                            {
                                e.Graphics.FillRectangle(b, rect1);
                            }
                        }
                        else
                        {
                            this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect1, headerState);
                        }
                    }
                    else
                    {
                        using (Brush b = new SolidBrush(Color.FromArgb(alpha, this.Schedule.Appearance.AllDayBackColor)))
                        {
                            e.Graphics.FillRectangle(b, rect1);
                        }
                    }
                }
                if (n > 0)
                {
                    Rectangle rect = e.TextRectangle;
                    int h = (int)(e.Style.Font.Size + 1);
                    rect.Height = 2 * h - 1;
                    bool isRTLCulture = this.schedule.Culture.TextInfo.IsRightToLeft;
                    bool useSpecialFormat = e.ColIndex == 1 && e.RowIndex == 1 && ScheduleType != ScheduleViewType.Week;
                    DateTime dt = (useSpecialFormat && !isRTLCulture) ? DateTime.ParseExact(list[0], this.Schedule.Appearance.WeekMonthNewMonth, this.Schedule.Culture).Date
                            : DateTime.Parse(list[0], this.Schedule.Culture).Date;
                    Font f = e.Style.GdipFont;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        if (dt.Month == 1 || (e.Style.CellIdentity.RowIndex == 1 && e.Style.CellIdentity.ColIndex == 1))
                            f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        else
                            f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    }
                    
                    ////  need to expose an event to allow the user to conditionally draw the background.
                    ////draw whole cell background
                    Color c1 = (dt.Month == columnHeaderDate.Month || ScheduleType == ScheduleViewType.Week) 
                                    ? this.Schedule.Appearance.PrimeTimeCellColor : this.Schedule.Appearance.NonPrimeTimeCellColor;
                    ////this.Schedule.Appearance.PrimeTimeCellColor))//baseCellColorPrime))
                    DateTime dt1 = DateTime.Parse(list[0], this.Schedule.Culture).Date;
                    if (this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex) && Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        c1 = Color.FromArgb(208, 208, 208);
                    }
                    using (Brush b = new SolidBrush(c1))
                    {
                        Rectangle rect2 = e.TextRectangle;
                        rect2.Width += 2;
                        rect2.X -= 1;
                        rect2.Height += 2;
                        rect2.Y -= 1;
                        e.Graphics.FillRectangle(b, rect2); ////e.TextRectangle);
                    }
                    ////draw the header
                    int currentCellPanel;
                    int panel;
                    Rectangle MetroDateRect = rect;
                    if (ScheduleType == ScheduleViewType.Month)
                    {
                        currentCellPanel = GetMonthPanelFromRowCol(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex);
                        panel = GetMonthPanelFromRowCol(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);

                        e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                        e.Style.VerticalAlignment = GridVerticalAlignment.Top;
                        e.Style.WrapText = false;
                        string s;
                        if (dt.Day == 1)
                        {                              ////long date pattern
                            s = (dt.Month == 1) ? GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat) : GetFormattedString(dt, schedule.Appearance.WeekMonthNewMonth);
                        }
                        else
                        {
                            s = (e.Style.CellIdentity.RowIndex == 1 && e.Style.CellIdentity.ColIndex == 1)
                                ? GetFormattedString(dt, schedule.Appearance.WeekMonthNewMonth) : dt.Day.ToString();
                        }

                        ThemedHeaderDrawing.HeaderState headerState = this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex)
                            ? ThemedHeaderDrawing.HeaderState.Pressed : ThemedHeaderDrawing.HeaderState.Normal;
                        int alpha = headerState == ThemedHeaderDrawing.HeaderState.Pressed ? 255 : 128;
                        //// draw back ground
                        Rectangle rect1 = e.TextRectangle;
                        rect1.X -= 1;
                        rect1.Width += 1;
                        rect1.Height = 2 * h - 1;
                        if (this.Schedule.Appearance.ThemesEnabled)
                        {
                            if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.SystemTheme)
                            {
                                //// SolidBrush(c))
                                using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderForeColor), Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderBackColor), System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                                {
                                    e.Graphics.FillRectangle(b, rect1);
                                }
                            }
                            else if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                            {
                                if (dt1.Date == this.schedule.Calendar.Today.Date)
                                {
                                    Rectangle todayRect = e.TextRectangle; 
                                    todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                    todayRect.Y -= 1;
                                    todayRect.X -= 1;
                                    todayRect.Width += 2;
                                    using (Brush todayb = new SolidBrush(this.Schedule.Appearance.TodayBackColor))
                                    {
                                        e.Graphics.FillRectangle(todayb, todayRect);
                                    }
                                }
                                else
                                {
                                    using (Brush b = new SolidBrush(c1))
                                    {
                                        e.Graphics.FillRectangle(b, rect1);
                                    }
                                }
                            }
                            else
                            {
                                this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect1, headerState);
                            }
                        }
                        else
                        {
                            if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                            {
                                using (Brush b = new SolidBrush(c1))
                                {
                                    e.Graphics.FillRectangle(b, rect);
                                }
                            }
                            else
                            {
                                using (Brush b = new SolidBrush(Color.FromArgb(alpha, this.Schedule.Appearance.AllDayBackColor)))
                                {
                                    e.Graphics.FillRectangle(b, rect1);
                                }
                            }
                        }

                        bool rtl = schedule.RightToLeft == RightToLeft.Yes;
                        Color c5 = e.Style.TextColor;
                        if (dt.Date == this.Schedule.Calendar.Today.Date && this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            c5 = this.Schedule.Appearance.TodayBackColor;
                            f = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        }
                        MetroDateRect = new Rectangle(rect1.X, rect1.Y + 3, rect1.Width, rect1.Height);
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            if (e.Graphics.DpiX > 96)
                            {
                                Font dpiFont;
                                if (f.Bold)
                                    dpiFont = new Font(f.FontFamily.Name, 8f, FontStyle.Bold);
                                else
                                    dpiFont = new Font(f.FontFamily.Name, 8f);
                                GridStaticCellRenderer.DrawText(e.Graphics, s, dpiFont, MetroDateRect, e.Style, c5, rtl);
                            }
                            else
                                GridStaticCellRenderer.DrawText(e.Graphics, s, f, MetroDateRect, e.Style, c5, rtl);
                        }
                        else
                            GridStaticCellRenderer.DrawText(e.Graphics, s, f, rect1, e.Style, c5, rtl);
                    }
                    else 
                    {
                        ////draw the week header
                        currentCellPanel = GetWeekPanelFromRowCol(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex); ////Math.Max(0, (this.CurrentCell.ColIndex - 1) * 3 + this.CurrentCell.RowIndex - 1);
                        panel = GetWeekPanelFromRowCol(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex); ////Math.Max(0, (e.Style.CellIdentity.ColIndex -1) * 3 + e.Style.CellIdentity.RowIndex - 1);

                        if (currentCellPanel < this.displayDates.GetLength(0))
                        {
                            if (list[0] == this.displayDates[currentCellPanel].Date.ToString(this.Schedule.Appearance.FullWeekHeaderFormat, this.Schedule.Culture))
                            {
                                if (this.Schedule.Appearance.ThemesEnabled)
                                {
                                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.SystemTheme)
                                    {
                                        //// SolidBrush(c))
                                        using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, this.Schedule.Appearance.MonthWeekHeaderForeColor, this.Schedule.Appearance.MonthWeekHeaderBackColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                                        {
                                            e.Graphics.FillRectangle(b, rect);
                                        }
                                    }
                                    else if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                    {
                                        if (dt1.Date == this.schedule.Calendar.Today.Date)
                                        {
                                            Rectangle todayRect = e.TextRectangle;
                                            todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                            todayRect.Y -= 1;
                                            todayRect.X -= 1;
                                            todayRect.Width += 2;
                                            using (Brush todayb = new SolidBrush(this.Schedule.Appearance.TodayBackColor))
                                            {
                                                e.Graphics.FillRectangle(todayb, todayRect);
                                            }
                                        }
                                        else
                                        {
                                            using (Brush b = new SolidBrush(c1))
                                            {
                                                e.Graphics.FillRectangle(b, rect);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect, ThemedHeaderDrawing.HeaderState.Pressed);
                                    }
                                }
                            }
                            else
                            {
                                if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                {
                                    if (dt1.Date == this.schedule.Calendar.Today.Date)
                                    {
                                        Rectangle todayRect = e.TextRectangle;
                                        todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                        todayRect.Y -= 1;
                                        todayRect.X -= 1;
                                        todayRect.Width += 2;
                                        using (Brush todayb = new SolidBrush(this.Schedule.Appearance.TodayBackColor))
                                        {
                                            e.Graphics.FillRectangle(todayb, todayRect);
                                        }
                                    }
                                    else
                                    {
                                        using (Brush b = new SolidBrush(c1))
                                        {
                                            e.Graphics.FillRectangle(b, rect);
                                        }
                                    }
                                }
                                else
                                {
                                    using (Brush b = new SolidBrush(this.Schedule.Appearance.AllDayBackColor))
                                    {
                                        e.Graphics.FillRectangle(b, rect);
                                    }
                                }
                            }

                            bool rtl = schedule.RightToLeft == RightToLeft.Yes;
                            MetroDateRect = new Rectangle(rect.X, rect.Y + 3, rect.Width, rect.Height);
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                GridStaticCellRenderer.DrawText(e.Graphics, GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat), f, MetroDateRect, e.Style, e.Style.TextColor, rtl);
                            }
                            else
                                GridStaticCellRenderer.DrawText(e.Graphics, GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat), f, rect, e.Style, e.Style.TextColor, rtl);
                        }
                    }

                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                    e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                    e.Style.BackColor = Color.FromArgb(0, Color.Red);
                    e.Style.WrapText = false;

                    int indent = 0;  ////left & right margins are directly proportional to this value
                    rect.X += 3 * indent;
                    rect.Width -= 6 * indent;
                    GridRangeInfo range = this.CoveredRanges.FindRange(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                    if (range.IsEmpty)
                    {
                        range = GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                    }

                    Rectangle textRectangle = this.RangeInfoToRectangle(range);
                    int bottomLimit=0;
                    if (this.GridVisualStyles == GridVisualStyles.Metro)
                        bottomLimit = textRectangle.Bottom;
                    else
                        bottomLimit = textRectangle.Bottom - MoreItemsBitmap.Height - 9;
                    if (panel < startIndexes.Count)
                    {
                        int start = (int)startIndexes[panel] - 1;
                        bool firstPass = true;
                        int skipCount = 0;
                        for (int i = 1; i < n; ++i)
                        {
                            bool skip = false;
                            rect.Y = rect.Bottom;
                            if (rect.Bottom < bottomLimit && start + i < scheduleDataList.Count)
                            {
                                IScheduleAppointment item = scheduleDataList[start + i] as IScheduleAppointment;
                                Color c = Color.FromArgb(100, ((ListObject)this.DataSource.GetLabels()[item.LabelValue]).ColorMember);
                                Rectangle rect1 = rect;
                                rect1.Width += 4 * indent;
                                rect1.X -= 2 * indent;
                                rect1.Y += 1;
                                if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                {
                                    rect1.Y += 4;
                                    rect1.Height += 10;
                                    rect.Y += 16;
                                }
                                else
                                    rect1.Height -= 2;

                                int saveRectY = rect1.Y;
                           
                                Graphics g = e.Graphics;
                                if (item is IRecurringScheduleAppointment && this.ScheduleType == ScheduleViewType.Month)
                                {
                                    IRecurringScheduleAppointment appt = item as IRecurringScheduleAppointment;
                                    if (appt.RecurrenceRule.StartsWith(RecurrenceSupport.SpanMarker))
                                    {
                                        Rectangle rect2 = rect1;
                                        if (schedule.ScheduleType == ScheduleViewType.Week)
                                        {
                                            rect2.Width += markWidth;
                                        }
                                        else
                                        {
                                            rect2.Width = this.ColWidths[3]; ////widths are all the same for month/week view
                                        }

                                        AddAllDaySpan(textRectangle, rect2, appt, dt.Date >= appt.DateList.TerminalDate.Date, e.Style); //// e.ColIndex == this.ColCount);                          
                                        skip = true;
                                        skipCount++;
                                    }
                                }

                                if (!skip)
                                {
                                    if (firstPass && SpanManager.BottomSlot(dt) > skipCount)
                                    {
                                        saveRectY += (SpanManager.BottomSlot(dt) - skipCount) * (rect1.Height + 2);
                                    }

                                    firstPass = false;
                                    if (!schedule.ShowRoundedCorners || this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        base.OnDrawCellDisplayText(e);
                                    }
                                    else
                                    {
                                        rect1.Y = saveRectY;
                                        rect1.Width -= 1;
                                        System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = DrawRoundRect(rect1.X, rect1.Y, rect1.Width, rect1.Height, Math.Min(rect1.Height / 4, 12));
                                        if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                        {
                                            using (Brush b = new SolidBrush(c))
                                            {
                                                g.FillPath(b, myGraphicsPath);
                                            }
                                        }
                                        else
                                        {
                                            using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect1, c, Color.FromArgb(20, c), LinearGradientMode.ForwardDiagonal))
                                            {
                                                g.FillPath(b, myGraphicsPath);
                                            }
                                        }
                                        if (item.Equals(mouseDownItem))
                                        {
                                            mouseDownRectangle = rect1;
                                            using (Pen p = new Pen(this.Schedule.Appearance.ClickItemBorderColor))
                                            {
                                                g.DrawPath(p, myGraphicsPath);
                                            }
                                        }
                                        else
                                        {
                                            using (Pen p = new Pen(c))
                                            {
                                                g.DrawPath(p, myGraphicsPath);
                                            }
                                        }

                                        myGraphicsPath.Dispose();

                                        rect1.Y += 1;
                                        bool rtl = schedule.RightToLeft == RightToLeft.Yes;
                                        GridStaticCellRenderer.DrawText(g, list[i], f, rect1, e.Style, e.Style.TextColor, rtl);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (rect.Bottom >= bottomLimit)
                        {
                            rect = this.RangeInfoToRectangle(GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex));
                            if (this.GridVisualStyles == GridVisualStyles.Metro)
                            {
                                string str = (n - 1).ToString() + " Events";
                                MetroDateRect.X += e.TextRectangle.Width - 55;
                                if (e.Graphics.DpiX > 96)
                                {
                                    Rectangle dpiRect = MetroDateRect;
                                    dpiRect.X = MetroDateRect.X - 5;
                                    Font dpiFont;
                                    if (f.Bold)
                                        dpiFont = new Font(f.FontFamily.Name, 8.25f, FontStyle.Bold);
                                    else
                                        dpiFont = new Font(f.FontFamily.Name, 8.25f);
                                    GridStaticCellRenderer.DrawText(e.Graphics, str, dpiFont, dpiRect, e.Style, e.Style.TextColor, this.IsRightToLeft());
                                }
                                else
                                    GridStaticCellRenderer.DrawText(e.Graphics, str, f, MetroDateRect, e.Style, e.Style.TextColor, this.IsRightToLeft());
                            }
                            else
                            {
                                rect.Y += e.TextRectangle.Height - ScheduleGrid.MoreItemsBitmap.Height - 1;
                                rect.X += e.TextRectangle.Width - ScheduleGrid.MoreItemsBitmap.Width - 1;
                                rect.Height = ScheduleGrid.MoreItemsBitmap.Height;
                                rect.Width = ScheduleGrid.MoreItemsBitmap.Width;
                                iconPainter.PaintIcon(e.Graphics, rect, Point.Empty, ScheduleGrid.MoreItemsBitmap, Color.Black);
                            }
                            moreItemsCells.Add(1000 * e.RowIndex + e.ColIndex);
                        }
                    }

                    e.Cancel = true;
                }
            }            
            else if (schedule.ShowRoundedCorners && !MarkCol(col) && col > this.markCol && row > allDayRow && (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek))
            {
                 ////handle the days view
                Hashtable ht = this.ScheduleAppointmentFromRange;
                int key = this.GetLookupKey(row, col);
                if (ht.ContainsKey(key))
                {
                    Color c = (RowIndexTo24Time(row) < this.Schedule.Appearance.PrimeTimeStart
                                || RowIndexTo24Time(row) >= this.Schedule.Appearance.PrimeTimeEnd)
                                ? this.Schedule.Appearance.NonPrimeTimeCellColor
                                : this.Schedule.Appearance.PrimeTimeCellColor;
                    using (Brush b = new SolidBrush(c))
                    {
                        GridRangeInfo range = CoveredRanges.FindRange(row, col);
                        if (!range.IsEmpty)
                        {
                            e.Graphics.FillRectangle(b, this.RangeInfoToRectangle(range));
                        }
                    }

                    Point t = e.ClipBounds.Location;
                    Rectangle rect = e.TextRectangle;
                    rect.Y -= 2;
                    rect.Height += 1;
                    rect.Width -= 1;

                    int radius = Math.Min(rect.Height / 4, 12);
                    IScheduleAppointment item = ht[key] as IScheduleAppointment;
                    c = ((ListObject)this.Schedule.DataSource.GetLabels()[item.LabelValue]).ColorMember;
                    if (item.Equals(mouseDownItem))
                    {
                        using (Pen p1 = new Pen(this.Schedule.Appearance.ClickItemBorderColor))
                        {
                            DrawRoundedBackGround(e.Graphics, rect, radius, c, p1);
                        }
                    }
                    else
                    {
                        DrawRoundedBackGround(e.Graphics, rect, radius, c);
                    }

                    string text = this.ParseDisplayItem(item, this.Schedule.Appearance.DayItemFormat);

                    rect.Offset(radius / 2, 2);
                    rect.Width -= radius;
                    rect.Height -= 2 * 2;
                    bool rtl = schedule.RightToLeft == RightToLeft.Yes;
                    GridStaticCellRenderer.DrawText(e.Graphics, text, e.Style.GdipFont, rect, e.Style, e.Style.TextColor, rtl); ////e.Style.RightToLeft == RightToLeft.Yes);
                    e.Cancel = true;
                }
            }
            else
            {
                 base.OnDrawCellDisplayText(e);
            }
        }

        protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Month && this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
            {
                string[] list = e.Style.Text.Split(new char[] { '\n' });
                int n = list.GetLength(0);
                if (n > 1)
                {
                    DateTime dt1 = DateTime.Parse(list[0], this.Schedule.Culture).Date;
                    if (dt1.Date == this.schedule.Calendar.Today.Date)
                    {
                        this[0, e.ColIndex].BackColor = this.Schedule.Appearance.TodayBackColor;
                        this[0, e.ColIndex].TextColor = Color.White;
                        this[0, e.ColIndex].Font.Bold = true;
                    }
                }
            }
            else
            {
                base.OnPrepareViewStyleInfo(e);
            }
        }
        /// <summary>
        /// Returns a date formatted as a MMMM d, yyyy that is displayed in a Month view.
        /// </summary>
        /// <param name="dt">The given date.</param>
        /// <param name="format">The string that provides the format.</param>
        /// <returns>The formatted string.</returns>
        public virtual string GetFormattedString(DateTime dt, string format)
        {
            string s = string.Empty;
            if (this.schedule.Culture.TextInfo.IsRightToLeft)
            {
                s = format.Replace("dddd", "~").Replace("ddd", "?").Replace("dd", "^").Replace("d", "^").Replace("~", "dddd").Replace("?", "ddd");
                DateTime time2 = dt.AddDays((double)-(dt.Day + 1));
                if ((this.Schedule.ScheduleType == ScheduleViewType.Month) || (this.Schedule.ScheduleType == ScheduleViewType.Day))
                {
                    int num2 = this.calendar.CalenderGrid.TableStyle.CultureInfo.Calendar.GetMonth(time2);
                    string newValue = this.calendar.CalenderGrid.TableStyle.CultureInfo.DateTimeFormat.GetMonthName(num2);
                    s = s.Replace("MMMM", newValue);
                    s = s.Replace("yyyy", this.calendar.CalenderGrid.TableStyle.CultureInfo.Calendar.GetYear(time2).ToString());
                }
                else if ((this.Schedule.ScheduleType == ScheduleViewType.WorkWeek) || (this.Schedule.ScheduleType == ScheduleViewType.CustomWeek))
                {
                    s = s.Replace("MM", this.calendar.CalenderGrid.TableStyle.CultureInfo.Calendar.GetMonth(time2).ToString());
                    int year = this.calendar.CalenderGrid.TableStyle.CultureInfo.Calendar.GetYear(time2);
                    s = s.Replace("yy", year.ToString().Substring(year.ToString().Length - 2, 2));
                }

                s = dt.ToString(s, this.Schedule.Culture).Replace("^", dt.Day.ToString());
            }
            else
            {
                if (format == "d")
                {
                    s = dt.ToString(format, this.Schedule.Culture);
                }
                else
                {
                    string fmt = format.Replace("dddd", "~").Replace("ddd", "?").Replace("dd", "^").Replace("d", "^");
                    fmt = fmt.Replace("~", "dddd").Replace("?", "ddd");
                    s = dt.ToString(fmt, this.Schedule.Culture);
                    s = s.Replace("^", dt.Day.ToString());
                }
            }

            return s;
        }

        /// <summary>
        /// Overridden to support the appointment being displayed with rounded corners.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            base.OnQueryCellInfo(e);
            if (schedule.ShowRoundedCorners)
            {
                int col = e.Style.CellIdentity.ColIndex;
                int row = e.Style.CellIdentity.RowIndex;
                if (!MarkCol(col) && col > this.markCol && row > allDayRow && (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek))
                {
                     Hashtable ht = ScheduleAppointmentFromRange; //// fi.GetValue(this) as Hashtable;
                    int key = this.GetLookupKey(row, col);
                    if (ht.ContainsKey(key))
                    {
                        e.Style.BackColor = Schedule.Appearance.PrimeTimeCellColor;
                        e.Style.Borders.Top = GridBorder.Empty; //// new GridBorder(GridBorderStyle.Solid, Color.Gray, GridBorderWeight.Thin); ////occupiedBorder;
                        e.Style.BorderMargins.Top = 0;

                        e.Style.Borders.Bottom = (row % 2) == 1 ? new GridBorder(GridBorderStyle.Solid, this.Schedule.Appearance.SolidBorderColor, GridBorderWeight.Thin)
                            : new GridBorder(this.Model.Options.DefaultGridBorderStyle);
                        e.Style.BorderMargins.Bottom = 0;
                    }
                }
            }
        }
        #endregion

        #region Helper Methods

        private ScheduleViewType wiredType;
		private Form parentTopLevelForm = null;
		private void WireEvents(ScheduleViewType t)
		{
            if (wired)
            {
                return;
            }

            //// Console.WriteLine("++++ Wire {0}", t);
			wiredType = t;
			////events for all scheduletypes
			this.QueryColWidth += new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryColWidth);
			this.MouseDown += new MouseEventHandler(Grid_MouseDown);
			this.MouseUp += new MouseEventHandler(Grid_MouseUp);
			this.CellDoubleClick += new GridCellClickEventHandler(Grid_CellDoubleClick);

			////listen for changes to base properties
			WireAppearanceEvents(this.Schedule.Appearance);

			parentTopLevelForm = this.TopLevelControl as Form;
			if (parentTopLevelForm != null)
			{
				parentTopLevelForm.Closing += new CancelEventHandler(ParentFormClosing);
			}
					
			////subscribe to change event to reset the formatted values
			this.Schedule.Appearance.WeekMonthItemFormatChanged += new EventHandler(week_FormatChanged);
			
			////TODO-Remove dependence of CellEmbeddedGrid
			int limit = this.Schedule.Appearance.DayMonthCutoff; //// most can show (t == ScheduleViewType.Month) ? 7 : this.numberPanels;
			this.allDayGrids = new CellEmbeddedGrid[limit];
			for (int i = 0; i < limit; ++i)
			{
				this.allDayGrids[i] = new CellEmbeddedGrid(this.Schedule);
				this.allDayGrids[i].DrawCellDisplayText += new GridDrawCellDisplayTextEventHandler(allDayGrid_DrawCellDisplayText);
          	}

			switch (t)
			{
				case ScheduleViewType.CustomWeek:
				case ScheduleViewType.WorkWeek:
				case ScheduleViewType.Day:
					this.SelectionChanging += new GridSelectionChangingEventHandler(Grid_SelectionChanging);
					this.QueryCellInfo += new Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventHandler(Grid_QueryCellInfo);
					this.DrawCellDisplayText += new GridDrawCellDisplayTextEventHandler(Grid_DrawCellDisplayText);
					this.TopRowChanged += new GridRowColIndexChangedEventHandler(Grid_TopRowChanged);
					this.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(Grid_QueryCoveredRange);
					this.CellDrawn += new GridDrawCellEventHandler(Grid_CellDrawn);
					this.CurrentCellStartEditing += new CancelEventHandler(Grid_CurrentCellStartEditing);
					this.SaveCellInfo += new GridSaveCellInfoEventHandler(Grid_SaveCellInfo);
					this.ResizingRows += new GridResizingRowsEventHandler(Grid_ResizingRows);
					this.ResizingColumns += new GridResizingColumnsEventHandler(Grid_ResizingColumns);
                    this.QueryRowHeight += new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryRowHeightDay);			
					break;
				case ScheduleViewType.Week:
				case ScheduleViewType.Month:
					this.QueryRowHeight += new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryRowHeight);
					this.DrawCellDisplayText += new GridDrawCellDisplayTextEventHandler(Grid_DrawCellDisplayText);

                    this.ResizingRows += new GridResizingRowsEventHandler(Grid_NoResizingRows);
                    this.ResizingColumns += new GridResizingColumnsEventHandler(Grid_NoResizingColumns);
				
					break;
				default:
					break;
			}

			this.wired = true;
		}

      

        private void DrawRoundedBackGround(Graphics g, Rectangle rect, int radius, Color c)
        {
            using (Pen p1 = new Pen(c))
            {
                DrawRoundedBackGround(g, rect, radius, c, p1);
            }
        }

        private void DrawRoundedBackGround(Graphics g, Rectangle rect, int radius, Color c, Pen p1)
        {
            System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, radius);
            using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, c, Color.FromArgb(20, c), LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(b, myGraphicsPath);
            }

            g.DrawPath(p1, myGraphicsPath);
            myGraphicsPath.Dispose();
        }

        /// <summary>
        /// Draws rounded rectangle specified by a co-ordinate pair, width, height and radius.
        /// </summary>
        /// <param name="X">X co-ordinate.</param>
        /// <param name="Y">Y co-ordinate.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <param name="radius">Radius of the rounded corners.</param>
        /// <returns>The GraphicsPath object for the rounded rectangle.</returns>
        public static GraphicsPath DrawRoundRect(float X, float Y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
            gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
            gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
            gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
            gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
            gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
            gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
            gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
            gp.CloseFigure();
            return gp;
        }

        /// <summary>
        /// A method to commit changes while closing parent control.
        /// </summary>
        /// <param name="sender">Parent Form</param>
        /// <param name="e">A <see cref="CancelEventArgs"/> with event data.</param>
		public virtual void ParentFormClosing(object sender, CancelEventArgs e)
		{ 
			if (!this.Schedule.IsDisposed)
			{
				if (this.Schedule.DataSource != null && this.Schedule.DataSource.IsDirty
					&& this.Schedule.DataSource.SaveOnCloseBehaviorAction != SaveOnCloseBehavior.DoNotSave)
				{
					if (this.Schedule.DataSource.SaveOnCloseBehaviorAction == SaveOnCloseBehavior.SaveWithoutPrompt
						|| MessageBox.Show(null, "Do you want to save your changes?", "Scheduler", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						this.Schedule.DataSource.CommitChanges();
					}
				}
			}
		}

		internal void WireAppearanceEvents(ScheduleAppearance appearance)
		{
			 ////listen for changes to base properties
			appearance.PrimeTimeCellColorChanged += new EventHandler(Appearance_PrimeTimeCellColorChanged);
			appearance.NonPrimeTimeCellColorChanged += new EventHandler(Appearance_NonPrimeTimeCellColorChanged);
			appearance.WorkWeekHeaderBackColorChanged += new EventHandler(Appearance_WorkWeekHeaderBackColorChanged);
			appearance.WorkWeekHeaderForeColorChanged += new EventHandler(Appearance_WorkWeekHeaderBackColorChanged);  ////same handler on purpose
			appearance.AllDayBackColorChanged += new EventHandler(Appearance_AllDayBackColorChanged);
			appearance.TimeBackColorChanged += new EventHandler(Appearance_TimeBackColorChanged);
			appearance.PrimeTimeEndChanged += new EventHandler(Appearance_PrimeTimeEndChanged);
			appearance.PrimeTimeStartChanged += new EventHandler(Appearance_PrimeTimeEndChanged); ////same handler on purpose
			appearance.TimeBigFontSizeChanged += new EventHandler(Appearance_TimeBigFontSizeChanged);
			appearance.TimeLittleFontSizeChanged +=new EventHandler(Appearance_TimeBigFontSizeChanged); ////same handler on purpose
			appearance.TextColorChanged += new EventHandler(Appearance_TextColorChanged);
			appearance.CaptionBackColorChanged += new EventHandler(Appearance_CaptionBackColorChanged);
			appearance.SolidBorderColorChanged += new EventHandler(Appearance_SolidBorderColorChanged);
			appearance.ShowCaptionButtonsChanged += new EventHandler(Appearance_ShowCaptionButtonsChanged);
			appearance.ThemesEnabledChanged += new EventHandler(Appearance_ThemeEnabledChanged);
			appearance.ShowCaptionChanged += new EventHandler(Appearance_ShowCaptionChanged);
			appearance.NavigationCalendarBackColorChanged += new EventHandler(Appearance_NavigationCalendarBackColorChanged);
			appearance.SplitterBackColorChanged += new EventHandler(appearance_SplitterBackColorChanged);
			appearance.DragColorChanged += new EventHandler(appearance_DragColorChanged);
            appearance.DivisionsPerHourChanged += new EventHandler(appearance_DivisionsPerHourChanged);
            appearance.MonthShowFullWeekChanged += new EventHandler(appearance_MonthShowFullWeekChanged);
            appearance.FullWeekHeaderFormatChanged += new EventHandler(appearance_FullWeekHeaderFormatChanged);
            appearance.WorkWeekHeaderFormatChanged += new EventHandler(appearance_WorkWeekHeaderFormatChanged);
            appearance.VisualStyleChanged += new EventHandler(appearance_VisualStyleChanged);
            appearance.WeekHeaderFormatChanged += new EventHandler(appearance_WeekHeaderFormatChanged);
            appearance.NavigationCalendarStartDayOfWeekChanged += new EventHandler(appearance_NavigationCalendarStartDayOfWeekChanged);
            appearance.MonthCalendarStartDayOfWeekChanged += new EventHandler(appearance_MonthCalendarStartDayOfWeekChanged);
            appearance.WeekCalendarStartDayOfWeekChanged += new EventHandler(appearance_WeekCalendarStartDayOfWeekChanged);
            appearance.LongHeaderFormatChanged += new EventHandler(appearance_LongHeaderFormatChanged);
            appearance.MonthHeaderFormatChanged += new EventHandler(appearance_MonthHeaderFormatChanged);
            appearance.WeekMonthNewMonthChanged += new EventHandler(appearance_WeekMonthNewMonthChanged);
            appearance.WeekMonthFullFormatChanged += new EventHandler(appearance_WeekMonthFullFormatChanged);
		}

		internal void UnwireAppearanceEvents(ScheduleAppearance appearance)
		{
			 ////listen for changes to base properties
			appearance.PrimeTimeCellColorChanged -= new EventHandler(Appearance_PrimeTimeCellColorChanged);
			appearance.NonPrimeTimeCellColorChanged -= new EventHandler(Appearance_NonPrimeTimeCellColorChanged);
			appearance.WorkWeekHeaderBackColorChanged -= new EventHandler(Appearance_WorkWeekHeaderBackColorChanged);
			appearance.WorkWeekHeaderForeColorChanged -= new EventHandler(Appearance_WorkWeekHeaderBackColorChanged);  ////same handler on purpose
			appearance.AllDayBackColorChanged -= new EventHandler(Appearance_AllDayBackColorChanged);
			appearance.TimeBackColorChanged -= new EventHandler(Appearance_TimeBackColorChanged);
			appearance.PrimeTimeEndChanged -= new EventHandler(Appearance_PrimeTimeEndChanged);
			appearance.PrimeTimeStartChanged -= new EventHandler(Appearance_PrimeTimeEndChanged); ////same handler on purpose
			appearance.TimeBigFontSizeChanged -= new EventHandler(Appearance_TimeBigFontSizeChanged);
			appearance.TimeLittleFontSizeChanged -=new EventHandler(Appearance_TimeBigFontSizeChanged); ////same handler on purpose
			appearance.TextColorChanged -= new EventHandler(Appearance_TextColorChanged);
			appearance.CaptionBackColorChanged -= new EventHandler(Appearance_CaptionBackColorChanged);
			appearance.SolidBorderColorChanged -= new EventHandler(Appearance_SolidBorderColorChanged);
			appearance.ShowCaptionButtonsChanged -= new EventHandler(Appearance_ShowCaptionButtonsChanged);
			appearance.ThemesEnabledChanged -= new EventHandler(Appearance_ThemeEnabledChanged);
			appearance.ShowCaptionChanged -= new EventHandler(Appearance_ShowCaptionChanged);
			appearance.NavigationCalendarBackColorChanged -= new EventHandler(Appearance_NavigationCalendarBackColorChanged);
            appearance.DivisionsPerHourChanged -= new EventHandler(appearance_DivisionsPerHourChanged);
            appearance.MonthShowFullWeekChanged -= new EventHandler(appearance_MonthShowFullWeekChanged);
            appearance.FullWeekHeaderFormatChanged -= new EventHandler(appearance_FullWeekHeaderFormatChanged);
            appearance.WorkWeekHeaderFormatChanged -= new EventHandler(appearance_WorkWeekHeaderFormatChanged);
            appearance.VisualStyleChanged -= new EventHandler(appearance_VisualStyleChanged);
            appearance.WeekHeaderFormatChanged -= new EventHandler(appearance_WeekHeaderFormatChanged);
            appearance.NavigationCalendarStartDayOfWeekChanged -= new EventHandler(appearance_NavigationCalendarStartDayOfWeekChanged);
            appearance.MonthCalendarStartDayOfWeekChanged -= new EventHandler(appearance_MonthCalendarStartDayOfWeekChanged);
            appearance.WeekCalendarStartDayOfWeekChanged -= new EventHandler(appearance_WeekCalendarStartDayOfWeekChanged);
            appearance.LongHeaderFormatChanged -= new EventHandler(appearance_LongHeaderFormatChanged);
            appearance.MonthHeaderFormatChanged -= new EventHandler(appearance_MonthHeaderFormatChanged);
            appearance.WeekMonthNewMonthChanged -= new EventHandler(appearance_WeekMonthNewMonthChanged);
            appearance.WeekMonthFullFormatChanged -= new EventHandler(appearance_WeekMonthFullFormatChanged);
		}

		internal bool wired = false;
		private void UnwireEvents(ScheduleViewType t)
		{
            if (!wired)
            {
                return;
            }

			this.QueryColWidth -= new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryColWidth);
			this.MouseDown -= new MouseEventHandler(Grid_MouseDown);
			this.MouseUp -= new MouseEventHandler(Grid_MouseUp);
			this.CellDoubleClick -= new GridCellClickEventHandler(Grid_CellDoubleClick);
           		
			if (parentTopLevelForm != null)
			{
				parentTopLevelForm.Closing -= new CancelEventHandler(ParentFormClosing);
			}
			
			UnwireAppearanceEvents(this.Schedule.Appearance);

			switch (t)
			{
				case ScheduleViewType.CustomWeek:
				case ScheduleViewType.WorkWeek:
				case ScheduleViewType.Day:
					this.SelectionChanging -= new GridSelectionChangingEventHandler(Grid_SelectionChanging);
					this.QueryCellInfo -= new Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventHandler(Grid_QueryCellInfo);
					this.DrawCellDisplayText -= new GridDrawCellDisplayTextEventHandler(Grid_DrawCellDisplayText);
					this.TopRowChanged -= new GridRowColIndexChangedEventHandler(Grid_TopRowChanged);
					this.QueryCoveredRange -= new GridQueryCoveredRangeEventHandler(Grid_QueryCoveredRange);
					this.CellDrawn -= new GridDrawCellEventHandler(Grid_CellDrawn);
					this.CurrentCellStartEditing -= new CancelEventHandler(Grid_CurrentCellStartEditing);
					this.SaveCellInfo -= new GridSaveCellInfoEventHandler(Grid_SaveCellInfo);
					this.ResizingRows -= new GridResizingRowsEventHandler(Grid_ResizingRows);
					this.ResizingColumns -= new GridResizingColumnsEventHandler(Grid_ResizingColumns);
                    this.QueryRowHeight -= new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryRowHeightDay);	
					break;
				case ScheduleViewType.Week:
				case ScheduleViewType.Month:
					this.QueryRowHeight -= new Syncfusion.Windows.Forms.Grid.GridRowColSizeEventHandler(Grid_QueryRowHeight);
					this.DrawCellDisplayText -= new GridDrawCellDisplayTextEventHandler(Grid_DrawCellDisplayText);
                    this.ResizingRows -= new GridResizingRowsEventHandler(Grid_NoResizingRows);
                    this.ResizingColumns -= new GridResizingColumnsEventHandler(Grid_NoResizingColumns);									
					break;
				default:
					break;
			}

			this.Schedule.Appearance.WeekMonthItemFormatChanged -= new EventHandler(week_FormatChanged);
			this.wired = false;			
		}

		/// <summary>
		/// Initialzes the <see cref="ScheduleGrid"/> through its GridControl properties and events.
		/// </summary>
		/// <remarks>
		/// This methods initializes to various GridControl properties, celltypes and mousecontrollers to 
		/// support the functionality in ScheduleGrid.
		/// </remarks>
		public void SetupChildControls()
		{
			////turn off grid default D&D
			this.ControllerOptions &= ~GridControllerOptions.OleDropTarget & ~GridControllerOptions.OleDataSource;
           
			if (this.ScheduleType == ScheduleViewType.Day ||
				this.ScheduleType == ScheduleViewType.WorkWeek
				|| this.ScheduleType == ScheduleViewType.CustomWeek)
			{			
				GridBaseStyle bs = new GridBaseStyle("PrimeTime", false);
				bs.StyleInfo.BackColor = this.Schedule.Appearance.PrimeTimeCellColor; ////baseCellColorPrime;
				this.BaseStylesMap.Add(bs);
				bs = new GridBaseStyle("OffTime", false);
				bs.StyleInfo.BackColor = this.Schedule.Appearance.NonPrimeTimeCellColor; ////baseCellColorNonPrime;
				this.BaseStylesMap.Add(bs);
				this.ResizeRowsBehavior |= GridResizeCellsBehavior.InsideGrid;
				this.ResizeColsBehavior |= GridResizeCellsBehavior.InsideGrid; //// GridResizeCellsBehavior.None;
			
				////selection support
				this.AllowSelection = GridSelectionFlags.Any & ~GridSelectionFlags.Row & ~GridSelectionFlags.Column & ~GridSelectionFlags.MixRangeType;
				alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
				this.AlphaBlendSelectionColor = Color.FromArgb(0, Color.Red); ////hide it so can draw partial selected row
				this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;

				////use a custom mouse controller
				this.Model.Options.SelectCellsMouseButtonsMask = MouseButtons.Left;
				IMouseController c = this.MouseControllerDispatcher.Find("ResizeCells");
				if (c!= null)
				{
					this.MouseControllerDispatcher.Remove(c);
					this.MouseControllerDispatcher.Add(new ScheduleResizeCellsMouseController(this)); ////, this));
				}
		 
				this.CellModels.Add("AllDayGrid", new GridInCellModel(this.Model));
			}
			else
			{
                ////Month or Week
				this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.AlwaysVisible;
                this.AllowSelection = GridSelectionFlags.AlphaBlend;  ////turn off selections
			}

			this.WireEvents(this.ScheduleType);
			SetupGrid();
			this.Schedule.OnSetupContextMenu();
		}

		#region appearance changes handled
        private void Appearance_WorkWeekHeaderBackColorChanged(object sender, EventArgs e)
        {
            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                row0BrushInfo = new BrushInfo(this.Schedule.Appearance.WorkWeekHeaderBackColor);
            else
                row0BrushInfo = new BrushInfo(GradientStyle.Vertical, this.Schedule.Appearance.WorkWeekHeaderForeColor, this.Schedule.Appearance.WorkWeekHeaderBackColor);
        }

        private void week_FormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Week || this.ScheduleType == ScheduleViewType.Month)
            {
                ////force data to be regenerated
                this.SetDataToDayPanels(true);
            }
        }

        void appearance_WeekMonthFullFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Month || this.ScheduleType == ScheduleViewType.Week)
            {
                ////force data to be regenerated
                this.SwitchTo(this.ScheduleType, true);
            }
        }

        void appearance_WeekMonthNewMonthChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Month)
            {
                ////force data to be regenerated
                this.SwitchTo(this.ScheduleType, true);
            }
        }

        void appearance_MonthHeaderFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Month)
            {
                ////force data to be regenerated
                this.SwitchTo(this.ScheduleType, true);
            }
        }

        void appearance_WorkWeekHeaderFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.CustomWeek
                || this.ScheduleType == ScheduleViewType.WorkWeek)
            {
                this.Schedule.PerformSwitchToScheduleViewTypeClick(this.ScheduleType);
            }
        }

        void appearance_LongHeaderFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Day)
            {
                this.Schedule.PerformSwitchToScheduleViewTypeClick(this.ScheduleType);
            }
        }

        void appearance_VisualStyleChanged(object sender, EventArgs e)
        {
            ApplyVisualStyle();
            this.Refresh();
        }

        void appearance_FullWeekHeaderFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Week)
            {
                this.Schedule.PerformSwitchToScheduleViewTypeClick(this.ScheduleType);
            }
        }

        void appearance_WeekHeaderFormatChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.CustomWeek || this.ScheduleType == ScheduleViewType.WorkWeek)
            {
                ////force data to be regenerated                 
                this.SwitchTo(this.ScheduleType, true); 
            }
        }

        void appearance_MonthCalendarStartDayOfWeekChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Month)
            {
                ////force data to be regenerated
                this.SwitchTo(this.ScheduleType, true); 
            }
        }

        void appearance_WeekCalendarStartDayOfWeekChanged(object sender, EventArgs e)
        {
            if (this.ScheduleType == ScheduleViewType.Week)
            {
                ////force data to be regenerated
                this.SwitchTo(this.ScheduleType, true);
            }
        }

        void appearance_NavigationCalendarStartDayOfWeekChanged(object sender, EventArgs e)
        {
            ////force data to be regenerated
            this.SwitchTo(this.ScheduleType, true); 
        }

		private void appearance_DragColorChanged(object sender, EventArgs e)
		{
			ScheduleResizeCellsMouseController._dragWindow = null;
			ScheduleResizeCellsMouseController._oldBoundsWindow = null;
			ScheduleResizeCellsMouseController.dragColor = this.Schedule.Appearance.DragColor;
 		}

		private void appearance_SplitterBackColorChanged(object sender, EventArgs e)
		{
			this.Schedule.splitterMiddle.BackColor = this.Schedule.Appearance.SplitterBackColor;
			this.Schedule.navigationPanelSplitter.BackColor = this.Schedule.Appearance.SplitterBackColor;
		}

		private void Appearance_NonPrimeTimeCellColorChanged(object sender, EventArgs e)
		{
			this.BaseStylesMap["OffTime"].StyleInfo.BackColor = this.Schedule.Appearance.NonPrimeTimeCellColor;
		}

		private void Appearance_PrimeTimeCellColorChanged(object sender, EventArgs e)
		{
			this.BaseStylesMap["PrimeTime"].StyleInfo.BackColor = this.Schedule.Appearance.PrimeTimeCellColor;
		}

		private void Appearance_ShowCaptionButtonsChanged(object sender, EventArgs e)
		{
			this.Schedule.nextButton.Visible = this.Schedule.Appearance.ShowCaptionButtons;
			this.Schedule.previousButton.Visible = this.Schedule.Appearance.ShowCaptionButtons;
		}

		private void Appearance_ThemeEnabledChanged(object sender, EventArgs e)
		{
			this.Schedule.nextButton.ThemesEnabled = this.Schedule.Appearance.ThemesEnabled;
			this.Schedule.previousButton.ThemesEnabled = this.Schedule.Appearance.ThemesEnabled;
		}

		private void Appearance_ShowCaptionChanged(object sender, EventArgs e)
		{
			this.Schedule.CaptionPanel.Visible = this.Schedule.Appearance.ShowCaption;
		}

		private void Appearance_NavigationCalendarBackColorChanged(object sender, EventArgs e)
		{
			this.Calendar.Paint += new PaintEventHandler(this.Calendar.calendar_Paint);

			this.calendar.BackColor = this.Schedule.Appearance.NavigationCalendarBackColor;
			this.calendar.scheduleGridGrid.Properties.BackgroundColor = this.Schedule.Appearance.NavigationCalendarBackColor;
		}

		private void Appearance_AllDayBackColorChanged(object sender, EventArgs e)
		{
            if (this.ScheduleType != ScheduleViewType.Week && this.ScheduleType != ScheduleViewType.Month)
            {
                for (int i = 0; i < this.numberPanels; ++i)
                {
                    int start = this.markCol + 1 + (i * (this.dayColCount - 1));
                    this[1, start].BackColor = this.Schedule.Appearance.AllDayBackColor; ////allDayRowBackColor;
                    this.allDayGrids[i].TableStyle.BackColor = this.Schedule.Appearance.AllDayBackColor; ////allDayRowBackColor;
                    this.allDayGrids[i].TableStyle.Borders.Right = GridBorder.Empty;
                    this.allDayGrids[i].Properties.BackgroundColor = this.Schedule.Appearance.AllDayBackColor; ////allDayRowBackColor;
                    this.allDayGrids[i][0, 1].Interior = (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? new BrushInfo(this.Schedule.Appearance.AllDayBackColor) : new BrushInfo(GradientStyle.Vertical, Color.FromArgb(20, this.Schedule.Appearance.AllDayBackColor), this.Schedule.Appearance.AllDayBackColor);
                }
            }

			this.BaseStylesMap["Column Header"].StyleInfo.Interior = new BrushInfo(GradientStyle.Vertical, this.Schedule.Appearance.AllDayBackColor, Color.FromArgb(100, this.Schedule.Appearance.AllDayBackColor));
		}

		private void Appearance_SolidBorderColorChanged(object sender, EventArgs e)
		{
			this.solidBorder = new GridBorder(GridBorderStyle.Solid, this.Schedule.Appearance.SolidBorderColor, GridBorderWeight.Thin);
            ApplySolidBorder();
		}

        private void ApplySolidBorder()
        {
            if (this.ScheduleType == ScheduleViewType.Day ||
                this.ScheduleType == ScheduleViewType.WorkWeek ||
                this.ScheduleType == ScheduleViewType.CustomWeek)
            {
                for (int i = allDayRow + 1; i <= this.RowCount; i += this.Schedule.Appearance.DivisionsPerHour)
                {
                    this[i, timeCol].Borders.Bottom = solidBorder;
                    this.RowStyles[i + this.Schedule.Appearance.DivisionsPerHour - 1].Borders.Bottom = solidBorder;
                }

                if (this.numberPanels > 1)
                {
                    this[0, 1].Borders.Bottom = solidBorder;
                }
                else
                {
                    this[1, 1].Borders.Bottom = solidBorder;
                }

                this.Refresh();
            }
            else
            {
                this.BaseStylesMap["Standard"].StyleInfo.Borders.Bottom = solidBorder;
                this.BaseStylesMap["Standard"].StyleInfo.Borders.Right = solidBorder;
            }
        }

		private void Appearance_CaptionBackColorChanged(object sender, EventArgs e)
		{
			this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
			this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
		}

		private void Appearance_TimeBigFontSizeChanged(object sender, EventArgs e)
		{
			this.resetTimeRects = true;
		}

		private void Appearance_TimeBackColorChanged(object sender, EventArgs e)
		{
			this.IgnoreReadOnly = true;
			this.ColStyles[timeCol].BackColor = this.Schedule.Appearance.TimeBackColor;
			this.IgnoreReadOnly = false;
		}

        private void SetBaseStyle(int startRow)
        {
            string styleName = "PrimeTime";
            if (RowIndexTo24Time(startRow) < this.Schedule.Appearance.PrimeTimeStart
                        || RowIndexTo24Time(startRow) >= this.Schedule.Appearance.PrimeTimeEnd)
            {
                styleName = "OffTime";
            }

            for (int j = startRow; j < startRow + this.Schedule.Appearance.DivisionsPerHour; ++j)
            {
                this.RowStyles[j].BaseStyle = styleName;
            }
        }

		private void Appearance_PrimeTimeEndChanged(object sender, EventArgs e)
		{
			if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
			{
                for (int i = allDayRow + 1; i <= this.RowCount; i += this.Schedule.Appearance.DivisionsPerHour)
                {
                    SetBaseStyle(i);
                }
			}
		}

		private void Appearance_TextColorChanged(object sender, EventArgs e)
		{
			if (this.numberPanels == 1)
			{
				this.allDayGrids[0][0, 1].TextColor = this.Schedule.Appearance.TextColor;
			}

			if (this.ScheduleType != ScheduleViewType.Week && this.ScheduleType != ScheduleViewType.Month)
			{
				for (int i = 0; i < this.numberPanels; ++i)
				{
					this.allDayGrids[i].TableStyle.TextColor = this.Schedule.Appearance.TextColor;
					int start = this.markCol + 1 + (i * (this.dayColCount - 1));
					this[row0, start].TextColor = this.Schedule.Appearance.TextColor;
				}
			}

			this.BaseStylesMap["Column Header"].StyleInfo.TextColor = this.Schedule.Appearance.TextColor;
			this.Schedule.HeaderLabel.ForeColor = this.Schedule.Appearance.TextColor;

			this.IgnoreReadOnly = true;
			this.TableStyle.TextColor = this.Schedule.Appearance.TextColor;
			this.IgnoreReadOnly = false;
		}

        void appearance_DivisionsPerHourChanged(object sender, EventArgs e)
        {
            this.minutesPerDivision = 60 / this.Schedule.Appearance.DivisionsPerHour;
             ////trigger a new display to use the new setting
            if (this.scheduleType == ScheduleViewType.CustomWeek
                || scheduleType == ScheduleViewType.Day
                || scheduleType == ScheduleViewType.WorkWeek)
            {
                this.Schedule.PerformSwitchToScheduleViewTypeClick(scheduleType);
            }
        }

        void appearance_MonthShowFullWeekChanged(object sender, EventArgs e)
        {
             ////trigger a new display to use the new setting
            if (this.scheduleType == ScheduleViewType.Month)
            {
                this.SwitchTo(this.scheduleType, true); 
            }
        }

		#endregion

		////these parameters set the allowable number of conflicts...

        private int maxConflicts = 10;
        private int dayColCount = 212; // product of 2 + 1 * 2 * 3 * 5 * 7  - can handle up to maxconflicts conflicts

        ////depends on this.Schedule.Appearance.DivisionsPerHour
       internal int minutesPerDivision = 30;
       internal int secondPerDivision = 30;

   		////private int maxConflicts = 3;
		////private int dayColCount = 8; // product of 2 + 1 * 2 * 3  - can handle up to maxConflicts conflicts
		////private int maxConflicts = 16;
		////private int dayColCount = 2 + 210*11*13; // product of 2 + 1 * 2 * 3 * 5 * 7  - can handle up to maxConflicts conflicts
		////private int maxConflicts = 10;
		////private int dayColCount = 212; // product of 2 + 1 * 2 * 3 * 5 * 7  - can handle up to maxConflicts conflicts
		private DateTime startOfMonthCalendar;
		private DateTime endOfMonthCalendar;

        internal DayOfWeek StartDayOfWeekWeekCalendar
        {
            get
            {
                return schedule.Appearance.WeekCalendarStartDayOfWeek;
            }
        }
        
        internal DayOfWeek StartDayOfWeekMonthCalendar
        {
            get
            {
                return schedule.Appearance.MonthCalendarStartDayOfWeek;
            }
        }

        internal DayOfWeek LastDayOfWeekMonthCalendar
        {
            get
            {
                return (DayOfWeek)((6 + (int)StartDayOfWeekMonthCalendar) % 7);
            }
        }
        /// <summary>
        /// used to set 'how many appointment will be shown in schedule'. set minumnum value for this if you use maximum value, it will decrease the performance of schdule grid.
        /// </summary>
        private int MaxConflicts
        {
            get
            {
                return this.schedule.MaxAppointment;
            }
            set
            {
                maxConflicts = value;
            }
        }
		private void SetupGrid()
		{
             ////used to set the width of covered columns in the schedulegrid
            smallColWidth = -1;
           
			this.BeginUpdate();
			this.SmoothControlResize = false;
			this.RefreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshRow;
			this.ColWidths[0] = 0;
			this.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;
			this.DisableScrollWindow = true;
			this.VerticalThumbTrack = true;
			this.VScrollPixel = true;
            this.DefaultRowHeight = 22;
			this.EnterKeyBehavior = GridDirectionType.Down;
			this.Model.Options.WrapCellBehavior = GridWrapCellBehavior.WrapRow;
			this.Properties.FixedLinesColor = Color.FromArgb(0, this.Properties.FixedLinesColor);  ////hide the dashed fixed line
			this.AllowSelection &= ~GridSelectionFlags.Column;
			noBottomBorder = new GridBorder(GridBorderStyle.None);
			
            row0BrushInfo = (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? new BrushInfo(this.Schedule.Appearance.WorkWeekHeaderBackColor) : new BrushInfo(GradientStyle.Vertical, this.Schedule.Appearance.WorkWeekHeaderForeColor, this.Schedule.Appearance.WorkWeekHeaderBackColor);
			this.TableStyle.TextColor = this.Schedule.Appearance.TextColor;

			switch (this.ScheduleType)
			{
					#region case WorkWeek or Day

				case ScheduleViewType.WorkWeek:
				case ScheduleViewType.Day:
				case ScheduleViewType.CustomWeek:
					this.ColStyles[this.timeCol].Enabled = false;
					this.Properties.DisplayVertLines = false;
					this.HScrollBehavior = GridScrollbarMode.Disabled;  ////never display scrollbar
                    this.ColCount = (this.dayColCount - 1) * this.numberPanels + this.timeCol; ////this.markCol;
                    this.RowCount = this.Schedule.Appearance.DivisionsPerHour * 24 + allDayRow;  ////49;  //2 divisions per hour times 24 hours
					this.Rows.FrozenCount = 1;
                    this.schedule.GetScheduleHost().PersistAppearanceSettings = true;
                    if (ScheduleType != ScheduleViewType.Day)
                    {
                        this.RowHeights[row0] = rowHeight0;
                    }
                    else
                    {
                        this.RowHeights[row0] = 2;
                    }

					solidBorder = new GridBorder(GridBorderStyle.Solid, this.Schedule.Appearance.SolidBorderColor, GridBorderWeight.Thin);
                    defaultBorder = this.TableStyle.Borders.Bottom;
                    this.CoveredRanges.Clear();

					////allday cell that holds alldaygrid
					this.RowHeights[1] = rowHeightAllDayRow;
					for (int i = 0; i < this.numberPanels; ++i)
					{
						int start = this.markCol + 1 + (i * (this.dayColCount - 1));
						int end = start + this.dayColCount - 3;
						this.CoveredRanges.Add(GridRangeInfo.Cells(1, start, 1, end));
						this[1, start].BackColor = this.Schedule.Appearance.AllDayBackColor;

                        this.allDayGrids[i].TableStyle.BackColor = (Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? Color.White : this.Schedule.Appearance.AllDayBackColor;
						this.allDayGrids[i].TableStyle.TextColor = this.Schedule.Appearance.TextColor;
						this.allDayGrids[i].RowCount = 0;
                        this.allDayGrids[i].Properties.BackgroundColor = (Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? Color.White : this.Schedule.Appearance.AllDayBackColor;
						this.allDayGrids[i].TopMargin = 4;
						this.allDayGrids[i].ColCount = this.dayColCount - 1;
						this.allDayGrids[i].ResetVolatileData();
                        this.allDayGrids[i].Intensity = 100;
						this[1, start].CellType = "AllDayGrid";
						this[1, start].Control = this.allDayGrids[i];
						if (this.numberPanels > 1)
						{
							this.allDayGrids[i].LeftMargin = 2;
							this.CoveredRanges.Add(GridRangeInfo.Cells(0, start, 0, end + 1));
							this[row0, start].Enabled = false;
                            this[row0, start].Text = this.Schedule.Appearance.WorkWeekHeaderFormat.Length > 0 ?
                                GetFormattedString(this.displayDates[i], this.Schedule.Appearance.WorkWeekHeaderFormat)  //// this.displayDates[i].ToString(this.Schedule.Appearance.WorkWeekHeaderFormat, this.Schedule.Culture)
                                : " ";
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                this[row0, start].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular));
                                this[row0, start].BackColor = (Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? Color.White : this.Schedule.Appearance.AllDayBackColor;
                                this[row0, start].CellType = GridCellTypeName.Static;
                                this[row0, start].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, this.Schedule.Appearance.SolidBorderColor);

                            }
                            this[row0, start].TextColor = this.Schedule.Appearance.TextColor;
						}
						else
						{
							this.allDayGrids[i].LeftMargin = 10;
							this.allDayGrids[i][0, 1].TextColor = this.Schedule.Appearance.TextColor;
                            this.allDayGrids[i][0, 1].Text = DisplayStrings[_Double_Click_to_Add_All_Day_Event]; ////" Double Click to Add All Day Event";
							this.allDayGrids[i][0, 1].Font.Bold = false;
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                this.allDayGrids[i][0, 1].CellType = GridCellTypeName.Static;
                                this.allDayGrids[i][0, 1].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, this.Schedule.Appearance.SolidBorderColor);
                                this.allDayGrids[i][0, 1].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 19F, System.Drawing.FontStyle.Regular));
                                this.allDayGrids[i].RowHeights[0] = 50;
                                this.CoveredRanges.Add(GridRangeInfo.Cells(0, 2, 0, this.ColCount));
                            }
                            else
                                this.CoveredRanges.Add(GridRangeInfo.Cells(0, 1, 0, this.ColCount));
							this.allDayGrids[i][0, 1].BorderMargins.Right = this.timeWidth - SystemInformation.VerticalScrollBarWidth;
							this[row0, 1].Enabled = false;
						}

						this.allDayGrids[i][0, 1].CellType = "Static";
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            this.allDayGrids[i][0, 1].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular));
                            this.allDayGrids[i][0, 1].Interior = new BrushInfo(Color.White);
                            this.allDayGrids[i].TableStyle.HorizontalAlignment = GridHorizontalAlignment.Center;
                            this.allDayGrids[i].DefaultRowHeight = 35;
                        }
                        else
                        {
                            this.allDayGrids[i][0, 1].Interior = new BrushInfo(
                                GradientStyle.Vertical,
                                Color.FromArgb(20, this.Schedule.Appearance.AllDayBackColor),
                                this.Schedule.Appearance.AllDayBackColor);
                            this.allDayGrids[i].DefaultRowHeight = allDayGridDefaultRowHeight;
                        }
						this.allDayGrids[i].ResetVolatileData();
					}	
										
					this.BaseStylesMap["Column Header"].StyleInfo.Font.Bold = false;
					this.BaseStylesMap["Column Header"].StyleInfo.TextColor = this.Schedule.Appearance.TextColor;

                    for (int i = allDayRow + 1; i <= this.RowCount; i += this.Schedule.Appearance.DivisionsPerHour)
					{
						this.CoveredRanges.Add(GridRangeInfo.Cells(i, timeCol, i + this.Schedule.Appearance.DivisionsPerHour - 1, timeCol));
						this[i, timeCol].Borders.Bottom = solidBorder;
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            this.RowStyles[i + this.Schedule.Appearance.DivisionsPerHour - 1].Borders.Bottom = solidBorder;
                            this.RowStyles[i + this.Schedule.Appearance.DivisionsPerHour].Borders.Bottom = new GridBorder(GridBorderStyle.Dotted, this.Schedule.Appearance.SolidBorderColor, GridBorderWeight.ExtraThin);
                        }
                        else
                            this.RowStyles[i + this.Schedule.Appearance.DivisionsPerHour - 1].Borders.Bottom = solidBorder;
                        SetBaseStyle(i);
                    }

					if (this.numberPanels > 1)
					{
                        this.CoveredRanges.Add(GridRangeInfo.Cells(0, 1, 0, this.Schedule.Appearance.DivisionsPerHour));
						this[0, 1].Text = " ";
						this[0, 1].Borders.Bottom = solidBorder;
					}
					else
					{
                        this.CoveredRanges.Add(GridRangeInfo.Cells(1, 1, 1, 2));
						this[1, 1].Borders.Bottom = solidBorder;
					}

					////set something into time column so DrawCellDisplayText is hit...
					GridStyleInfo style = this.ColStyles[timeCol];
					style.Text = " ";
					style.HorizontalAlignment = GridHorizontalAlignment.Right;
					style.VerticalAlignment = GridVerticalAlignment.Top;
					style.BackColor = this.Schedule.Appearance.TimeBackColor;
					style.CellType = "Static";
					style.ReadOnly = true;

					////set the toprow to start of primetime
					if (!this.IsDisposed && !this.IsDisposed && !this.IsDisposing)
					{
                        this.TopRowIndex = this.Schedule.Appearance.PrimeTimeStart * this.Schedule.Appearance.DivisionsPerHour + allDayRow + 1;
					}
				
					break;
					#endregion

					#region case Week
				case ScheduleViewType.Week:
					this.CancelUpdate();
					this.ColCount = 2;
					this.RowCount = 4;
					this.Properties.DisplayVertLines = true;
					this.DefaultGridBorderStyle = GridBorderStyle.Solid;
                    
					this.ActivateCurrentCellBehavior = GridCellActivateAction.None;
					this.TableStyle.ReadOnly = true;
					this.TableStyle.HorizontalAlignment = GridHorizontalAlignment.Right;
					this.RowHeights[0] = 0;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        for (int i = 0; i < ColCount; i++)
                        {
                            this[1, i + 1].Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(118, 118, 118));
                        }
                    }
					this.CoveredRanges.Add(GridRangeInfo.Cells(3, 1, 4, 1));
					this.VScrollBehavior = GridScrollbarMode.Disabled;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        for (int i = 1; i <= RowCount; i++)
                        {
                            for (int j = 0; j <= ColCount; j++)
                            {
                                this[i, j].Borders.Right = new GridBorder(GridBorderStyle.Solid, this.schedule.Appearance.SolidBorderColor);
                                this[i, j].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, this.schedule.Appearance.SolidBorderColor);
                            }
                        }
                    }
					break;
					#endregion

					#region case Month
				case ScheduleViewType.Month:
                    this.RowCount = GetRowCountInMonthView();
					this.Properties.DisplayVertLines = true;
					this.DefaultGridBorderStyle = GridBorderStyle.Solid;
                    
					this.ActivateCurrentCellBehavior = GridCellActivateAction.None;
					this.TableStyle.ReadOnly = true;
					this.TableStyle.HorizontalAlignment = GridHorizontalAlignment.Right;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        this.BaseStylesMap["Column Header"].StyleInfo.Interior = new BrushInfo(this.Schedule.Appearance.AllDayBackColor);
                    }
                    else
                    {
                        this.BaseStylesMap["Column Header"].StyleInfo.Font.Bold = false;
                        this.BaseStylesMap["Column Header"].StyleInfo.Interior = new BrushInfo(
                            GradientStyle.Vertical,
                            this.Schedule.Appearance.AllDayBackColor,
                            Color.FromArgb(100, this.Schedule.Appearance.AllDayBackColor));
                    }
					this.BaseStylesMap["Column Header"].StyleInfo.TextColor = this.Schedule.Appearance.TextColor;
					this.VScrollBehavior = GridScrollbarMode.Disabled;
                    int offset = this.Schedule.Appearance.MonthShowFullWeek ? 1 : 0;
                    for (int col = 1; col < this.ColCount + offset; col++)
					{
                        if (!this.Schedule.Appearance.MonthShowFullWeek)
                        {
                            this[0, col].Text = dayOfWeekStrings[col - offset];
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                this.Schedule.isMetro = true;
                                this[0, col].CellType = GridCellTypeName.Static;
                                this[0, col].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular));
                                
                            }
                            else
                                this.Schedule.isMetro = false;
                            for (int row = 1; row < this.RowCount; row += 2)
                            {
                                this.CoveredRanges.Add(GridRangeInfo.Cells(row, col, row + 1, col));
                            }
                        }
                        else
                        {
                            this[0, col].Text = dayOfWeekStrings[(col - 1 + (int)StartDayOfWeekMonthCalendar) % 7];
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                this.Schedule.isMetro = true;

                                this[0, col].TextColor = Color.FromArgb(51, 51, 51);
                                this[0, col].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular));
                                this[0, col].CellType = GridCellTypeName.Static;
                            }
                            else
                                this.Schedule.isMetro = false;
                        }
					}

                    if (!this.Schedule.Appearance.MonthShowFullWeek)
                    {
                        this[0, 6].Text = string.Format(this.Schedule.Culture, "{0}/{1}", dayOfWeekShortStrings[(int)DayOfWeek.Saturday].Substring(0, Math.Min(3, dayOfWeekShortStrings[(int)DayOfWeek.Saturday].Length)), dayOfWeekShortStrings[(int)DayOfWeek.Sunday].Substring(0, Math.Min(3, dayOfWeekShortStrings[(int)DayOfWeek.Sunday].Length)));
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            if (this.Schedule.Calendar.Today.DayOfWeek.ToString().ToUpper().StartsWith("SAT") || this.Schedule.Calendar.Today.DayOfWeek.ToString().ToUpper().EndsWith("SUN"))
                            {
                                this[0, 6].TextColor = Color.White;
                                this[0, 6].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold));
                            }
                            else
                            {
                                this[0, 6].TextColor = Color.FromArgb(51, 51, 51);
                                this[0, 6].Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular));
                            }
                            this[0, 6].CellType = GridCellTypeName.Static;
                        }
                    }
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        for (int i = 1; i <= RowCount; i++)
                        {
                            for (int j = 0; j <= ColCount; j++)
                            {
                                this[i, j].Borders.Right = new GridBorder(GridBorderStyle.Solid, this.schedule.Appearance.SolidBorderColor);
                                this[i, j].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, this.schedule.Appearance.SolidBorderColor);
                            }
                        }
                    }
                    else if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Office2007Blue)
                    {
                        Color office2007BorderClr = Color.FromArgb(255, 101, 147, 207);
                        for (int i = 1; i <= RowCount; i++)
                        {
                            for (int j = 0; j <= ColCount; j++)
                            {
                                this[i, j].Borders.Right = new GridBorder(GridBorderStyle.Solid, office2007BorderClr);
                                this[i, j].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, office2007BorderClr);
                            }
                        }
                    }
		 
					break;
					 
					#endregion

				default:
					break;
			}

			this.resetTimeRects = true;

			////raise an event to let the consumer modify look of grid before it is shown
            OnGridSetUpCompleted(EventArgs.Empty);

            if (this.Updating)
            {
                this.EndUpdate();
            }

			this.Refresh();
		}
		
		int GetUseTableFromCol(int col)
		{
			return (col - this.markCol) / (this.dayColCount - this.timeCol);
		}

		bool MarkCol(int col)
		{
			return (col % (this.dayColCount - 1)) == this.markCol;
		}

		int GetFirstColInPanelFromCol(int col)
		{
			return (this.dayColCount - 1) * GetUseTableFromCol(col) + this.markCol + 1;
		}

		int GetLastColInPanelFromCol(int col)
		{
			return (this.dayColCount - 1) * (1 + GetUseTableFromCol(col)) + this.markCol - 1;
		}

        internal void ApplyVisualStyle()
        {
            this.Schedule.FreezePainting = true;
            this.BeginUpdate();
            this.calendar.CalenderGrid.BeginUpdate();

            this.Model.Options.GridVisualStyles = this.Schedule.Appearance.VisualStyle;
            this.Calendar.CalenderGrid.Model.Options.GridVisualStyles = this.Schedule.Appearance.VisualStyle;
            Color backColor;
            Color headerTopColor; 
            Color headerLeftColor;
            this.Calendar.CalenderGrid.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out backColor, out headerTopColor, out headerLeftColor);
            
            switch (this.Schedule.Appearance.VisualStyle)
            {
                case GridVisualStyles.Office2003:
                    #region 2003
                    this.Schedule.Appearance.NavigationCalendarBackColor = SystemColors.Window;
                    this.Schedule.Appearance.NavigationCalendarArrowColor = Color.FromArgb(165, 164, 189);
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = SystemColors.GrayText;
                    this.Schedule.Appearance.NavigationCalendarHeaderColor = Color.FromArgb(216, 219, 223);
                    this.Schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(251, 200, 79);
                    this.Schedule.Appearance.NavigationCalendarTextColor = SystemColors.WindowText;
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.FromArgb(187, 85, 3);
                    this.Schedule.Appearance.PrimeTimeCellColor = SystemColors.Window;
                    this.Schedule.Appearance.SolidBorderColor = headerTopColor;
                    this.Schedule.Appearance.SplitterBackColor = headerLeftColor;
                    this.Schedule.Appearance.CaptionBackColor = Color.FromArgb(201, 202, 211); ////(147, 145, 176);
                    this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.schedule.NavigationPanel.BackColor = Color.FromArgb(201, 202, 211);
                    this.schedule.previousButton.Style = VisualStyle.Office2003;
                    this.schedule.previousButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.schedule.nextButton.Style = VisualStyle.Office2003;
                    this.schedule.nextButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.Schedule.Appearance.TimeBackColor = Color.FromArgb(240, 241, 242);
                    this.Schedule.Appearance.TimeTextColor = Color.FromArgb(11, 112, 140);
                    this.Schedule.Appearance.AllDayBackColor = Color.FromArgb(176, 182, 190);
                    if (this.Schedule.ScheduleType == ScheduleViewType.Month)
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(199, 203, 209);
                    }
                    else
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(210, 213, 218);
                        this.AlphaBlendSelectionColor = this.Schedule.NavigationPanel.BackColor; //// Color.FromArgb(254, 41, 76, 122);
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = this.Schedule.NavigationPanel.BackColor;
                    }
                    #endregion
                    break;
                case GridVisualStyles.Office2007Black:
                    #region black
                    this.Schedule.Appearance.NavigationCalendarBackColor = SystemColors.Window;
                    this.Schedule.Appearance.NavigationCalendarArrowColor = headerTopColor;
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = SystemColors.GrayText;
                    this.Schedule.Appearance.NavigationCalendarHeaderColor = Color.FromArgb(210, 213, 218);
                    this.Schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(251, 200, 79);
                    this.Schedule.Appearance.NavigationCalendarTextColor = SystemColors.WindowText;
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.FromArgb(187, 85, 3);
                    this.Schedule.Appearance.PrimeTimeCellColor = SystemColors.Window;
                    this.Schedule.Appearance.SolidBorderColor = headerTopColor;
                    this.Schedule.Appearance.SplitterBackColor = headerLeftColor;
                    this.Schedule.Appearance.CaptionBackColor = Color.FromArgb(145, 153, 164);
                    this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.schedule.NavigationPanel.BackColor = Color.FromArgb(145, 153, 164);
                    this.schedule.previousButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.previousButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.schedule.nextButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.nextButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.Schedule.Appearance.TimeBackColor = Color.FromArgb(240, 241, 242);
                    this.Schedule.Appearance.TimeTextColor = Color.FromArgb(76, 83, 126);
                    this.Schedule.Appearance.AllDayBackColor = Color.FromArgb(199, 203, 209);
                    if (this.Schedule.ScheduleType == ScheduleViewType.Month)
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(190, 198, 206);
                    }
                    else
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(232, 234, 236);
                        this.AlphaBlendSelectionColor = this.Schedule.NavigationPanel.BackColor; //// Color.FromArgb(254, 41, 76, 122);
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = this.Schedule.NavigationPanel.BackColor;
                    }
                    #endregion
                    break;
                case GridVisualStyles.Office2007Blue:
                    #region blue
                    this.Schedule.Appearance.NavigationCalendarBackColor = SystemColors.Window;
                    this.Schedule.Appearance.NavigationCalendarArrowColor = headerTopColor;
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = SystemColors.GrayText;
                    this.Schedule.Appearance.NavigationCalendarHeaderColor = Color.FromArgb(197, 222, 255);
                    this.Schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(251, 200, 79);
                    this.Schedule.Appearance.NavigationCalendarTextColor = SystemColors.WindowText;
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.FromArgb(187, 85, 3);
                    this.Schedule.Appearance.PrimeTimeCellColor = SystemColors.Window;
                    this.Schedule.Appearance.SolidBorderColor = headerTopColor;
                    this.Schedule.Appearance.SplitterBackColor = headerLeftColor;
                    this.Schedule.Appearance.CaptionBackColor = Color.FromArgb(173, 209, 255);
                    this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.schedule.NavigationPanel.BackColor = Color.FromArgb(173, 209, 255);
                    this.schedule.previousButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.previousButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.schedule.nextButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.nextButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.Schedule.Appearance.TimeBackColor = Color.FromArgb(227, 239, 255);
                    this.Schedule.Appearance.TimeTextColor = Color.FromArgb(148, 147, 215);
                    this.Schedule.Appearance.AllDayBackColor = Color.FromArgb(165, 191, 225);
                    if (this.Schedule.ScheduleType == ScheduleViewType.Month)
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(165, 191, 225);
                    }
                    else
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(230, 237, 247);
                        this.AlphaBlendSelectionColor = this.Schedule.NavigationPanel.BackColor; //// Color.FromArgb(254, 41, 76, 122);
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = this.Schedule.NavigationPanel.BackColor;
                    }
                    #endregion
                    break;
                case GridVisualStyles.Office2007Silver:
                    #region silver
                    this.Schedule.Appearance.NavigationCalendarBackColor = SystemColors.Window;
                    this.Schedule.Appearance.NavigationCalendarArrowColor = Color.FromArgb(165, 164, 189);
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = SystemColors.GrayText;
                    this.Schedule.Appearance.NavigationCalendarHeaderColor = Color.FromArgb(216, 219, 223);
                    this.Schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(251, 200, 79);
                    this.Schedule.Appearance.NavigationCalendarTextColor = SystemColors.WindowText;
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.FromArgb(187, 85, 3);
                    this.Schedule.Appearance.PrimeTimeCellColor = SystemColors.Window;
                    this.Schedule.Appearance.SolidBorderColor = headerTopColor;
                    this.Schedule.Appearance.SplitterBackColor = headerLeftColor;
                    this.Schedule.Appearance.CaptionBackColor = Color.FromArgb(166, 164, 189); ////(147, 145, 176);
                    this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.schedule.NavigationPanel.BackColor = Color.FromArgb(166, 164, 189);
                    this.schedule.previousButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.previousButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.schedule.nextButton.Style = VisualStyle.Office2007Outlook;
                    this.schedule.nextButton.Drawing = this.Model.Options.GridVisualStylesDrawing;
                    this.Schedule.Appearance.TimeBackColor = Color.FromArgb(240, 241, 242);
                    this.Schedule.Appearance.TimeTextColor = Color.FromArgb(11, 112, 140);
                    this.Schedule.Appearance.AllDayBackColor = Color.FromArgb(176, 182, 190);
                    if (this.Schedule.ScheduleType == ScheduleViewType.Month)
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(199, 203, 209);
                    }
                    else
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(210, 213, 218);
                        this.AlphaBlendSelectionColor = this.Schedule.NavigationPanel.BackColor; //// Color.FromArgb(254, 41, 76, 122);
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = this.Schedule.NavigationPanel.BackColor;
                    }
                    #endregion
                    break;
                case GridVisualStyles.Metro:
                #region Metro
                    this.MetroScrollBars = true;
                    this.schedule.Appearance.SolidBorderColor = Color.FromArgb(225, 225, 225);
                    this.Schedule.Appearance.NavigationCalendarBackColor = Color.FromArgb(255, 255, 255);
                    this.schedule.Appearance.MonthWeekHeaderForeColor = Color.White;
                    this.schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(253, 143, 0);
                    this.schedule.Appearance.ThemesEnabled = true;
                    this.Schedule.Appearance.TodayBackColor = Color.FromArgb(124, 204, 196);
                    this.schedule.Appearance.WorkWeekHeaderBackColor = Color.FromArgb(210 ,237, 241);
                    this.Schedule.Appearance.NavigationCalendarArrowColor = Color.FromArgb(0, 0, 0);
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = Color.FromArgb(179, 179, 179);
                    this.Schedule.Appearance.NavigationCalendarHeaderColor =this.Schedule.Appearance.TodayBackColor;
                    this.Schedule.Appearance.NavigationCalendarTextColor = Color.FromArgb(51,51,51);
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.Black;
                    this.schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(232, 248, 252);
                    this.schedule.Appearance.NavigationCalendarTodayBackColor =this.Schedule.Appearance.TodayBackColor;
                    this.schedule.Appearance.NavigationCalendarTodayTextColor =Color.Green;
                    this.Schedule.Appearance.PrimeTimeCellColor = SystemColors.Window;
                    this.Schedule.Appearance.SplitterBackColor = Color.FromArgb(118, 119,119);//headerLeftColor;
                    this.Schedule.Appearance.CaptionBackColor = Color.White;
                    this.Schedule.panelSpacer.BackColor =  Color.FromArgb(235, 235, 235);
                    this.Schedule.captionPanel.BackColor =  Color.FromArgb(255, 255, 255);
                    this.schedule.NavigationPanel.BackColor =Color.White;
                    this.Schedule.NavigationPanel.Size = new Size(202, 348);
                    this.schedule.NavigationPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    this.schedule.HeaderLabel.Font = new System.Drawing.Font("Segoe UI", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    this.schedule.HeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    this.schedule.previousButton.Style = VisualStyle.Metro;
                    this.schedule.previousButton.ForeColor = Color.FromArgb(77, 77, 77);
                    this.schedule.previousButton.BackColor = Color.White;
                    this.schedule.previousButton.Transparent = true;
                    this.Schedule.splitterMiddle.Location = new Point(this.Schedule.splitterMiddle.Location.X, this.Schedule.splitterMiddle.Location.Y + 10);
                    this.schedule.nextButton.Style = VisualStyle.Metro;
                    this.schedule.nextButton.Dock = System.Windows.Forms.DockStyle.Left;
                    this.schedule.nextButton.Transparent = true;
                    this.schedule.nextButton.BackColor = Color.White;
                    this.schedule.nextButton.ForeColor = Color.FromArgb(77, 77, 77);
                    this.schedule.nextButton.Location = new System.Drawing.Point(135, 1);
                    this.Schedule.Appearance.TimeBackColor = Color.White; 
                    this.Schedule.Appearance.TimeTextColor = Color.FromArgb(68 ,68 ,68); 
                    this.schedule.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    this.Schedule.Appearance.AllDayBackColor = Color.FromArgb(210 ,237, 241); 
                    this.Schedule.Appearance.ClickItemBorderColor = Color.Black;
                    this.schedule.Appearance.MoreItemArrowBorderColor = this.Schedule.Appearance.NavigationCalendarHeaderColor;
                    this.Schedule.Appearance.WeekMonthNewMonth = "MMM d";
                    this.Schedule.Appearance.WeekMonthFullFormat = "MMM d, yy";
                    this.Schedule.panelFixedTop.Size = new Size(176, 350);
                    this.schedule.panelFixedTop.Dock = DockStyle.Top;
                    this.Schedule.navigationPanelSplitter.Size = new System.Drawing.Size(176, 1);
                    this.Schedule.splitterMiddle.Size = new System.Drawing.Size(1, 448);
                    this.Schedule.splitterMiddle.BackColor = Color.FromArgb(225, 225, 225);
                    this.Schedule.navigationPanelSplitter.BackColor = Color.FromArgb(225, 225, 225);
                    if (this.Schedule.ScheduleType == ScheduleViewType.Month)
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = Color.FromArgb(236, 236, 236);
                    }
                    else
                    {
                        this.Schedule.Appearance.NonPrimeTimeCellColor = this.schedule.Appearance.SolidBorderColor;
                        this.AlphaBlendSelectionColor = this.Schedule.NavigationPanel.BackColor;
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = Color.FromArgb(117, 117, 177);
                    }
                #endregion
                    break;
                case GridVisualStyles.SystemTheme:
                    #region default
                    this.Schedule.Appearance.NavigationCalendarBackColor = SystemColors.Window;
                    this.Schedule.Appearance.NavigationCalendarArrowColor = Color.Black;
                    this.Schedule.Appearance.NavigationCalendarDisabledTextColor = SystemColors.GrayText;
                    this.Schedule.Appearance.NavigationCalendarHeaderColor = Color.FromArgb(215, 215, 229);
                    this.Schedule.Appearance.NavigationCalendarSelectionColor = Color.FromArgb(251, 230, 148);
                    this.Schedule.Appearance.NavigationCalendarTextColor = Color.Black;
                    this.schedule.Appearance.NavigationCalendarTodayColor = Color.Red;
                    this.Schedule.Appearance.PrimeTimeCellColor = Color.LightGoldenrodYellow;
                    this.Schedule.Appearance.SolidBorderColor = Color.Gray;
                    this.Schedule.Appearance.SplitterBackColor = Color.Gray;
                    this.Schedule.Appearance.CaptionBackColor = Color.LightGray;
                    this.Schedule.panelSpacer.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.Schedule.captionPanel.BackColor = this.Schedule.Appearance.CaptionBackColor;
                    this.schedule.NavigationPanel.BackColor = this.Schedule.BackColor;
                    this.schedule.previousButton.Style = VisualStyle.Default;
                    this.schedule.previousButton.Drawing = null; //// this.Model.Options.GridVisualStylesDrawing;
                    this.schedule.nextButton.Style = VisualStyle.Default;
                    this.schedule.nextButton.Drawing = null; //// this.Model.Options.GridVisualStylesDrawing;
                    this.Schedule.Appearance.TimeBackColor = Color.FromArgb(100, Color.LightGray);
                    this.Schedule.Appearance.TimeTextColor = Color.Black;
                    this.Schedule.Appearance.AllDayBackColor = Color.LightGray;
                    this.Schedule.Appearance.NonPrimeTimeCellColor = Color.Khaki;
                    if (this.Schedule.ScheduleType != ScheduleViewType.Month)
                    {
                        this.AlphaBlendSelectionColor = Color.FromArgb(64, SystemColors.Highlight); //// Color.FromArgb(254, 41, 76, 122);
                        this.alphaBlendSelectionColor = this.AlphaBlendSelectionColor;
                        this.Schedule.Appearance.MarkColumnColor = Color.FromArgb(50, Color.RoyalBlue); 
                    }

                    break;
                    #endregion default
                default:
                    break;
            }

            this.EndUpdate();
            this.calendar.CalenderGrid.EndUpdate();
            this.Schedule.FreezePainting = false;
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                switch (style)
                {
                    case "Office2007Blue":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2007Blue;
                        ApplyVisualStyle();
                        break;
                    case "Office2007Black":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2007Black;
                        ApplyVisualStyle();
                        break;
                    case "Office2007Silver":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2007Silver;
                        ApplyVisualStyle();
                        break;
                    case "Office2010Blue":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2010Blue;
                        ApplyVisualStyle();
                        break;
                    case "Office2010Black":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2010Black;
                        ApplyVisualStyle();
                        break;
                    case "Office2010Silver":
                        this.Schedule.Appearance.VisualStyle = GridVisualStyles.Office2010Silver;
                        ApplyVisualStyle();
                        break;
                    default:
                        break;
                }
            }
        }

		#region MoreItems/Arrow bitmap support
		[ThreadStaticAttribute] internal static IconPaint iconPainter;
		static string RootNamespace = AssemblyInfo.RootNamespace + @".Resources\";
		
        /// <summary>
		/// Gets the bitmap drawn when day on a Week or Month ScheduleViewType has more items
		/// than can be displayed in the grid cell for that day.
		/// </summary>
		public static Bitmap MoreItemsBitmap
		{
			get
			{
                if (iconPainter == null)
                {
                    iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
                }

				return iconPainter.GetBitmap(@"MoreItems.bmp");
			}
		}

        /// <summary>
        /// Gets the bitmap drawn.
        /// </summary>
        /// <param name="bmpName">Name of the bitmap.</param>
        /// <returns>The bitmap drawn.</returns>
        public static Bitmap GetBitmap(string bmpName)
        {
            if (iconPainter == null)
            {
                iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
            }

            return iconPainter.GetBitmap(bmpName);
        }
		#endregion

		#region Code that handles conflicts and calculates coveredranges
		
        ////private members used in the coveredcell generation algorithms

		/// <summary>
		/// Maps a GridRangeInfo to an IScheduleAppointment through GetLookupKey
		/// </summary>
		Hashtable ScheduleAppointmentFromRange = new Hashtable();

		/// <summary>
		/// Produces the lookup key that maps GridRangeInfo to an IScheduleAppointment
		/// </summary>
		/// <param name="rowIndex">the top-most row of the GridRangeInfo</param>
		/// <param name="colIndex">the left-most column of the GridrangeInfo</param>
		/// <returns>A value unique to the passed in parameters</returns>
		private int GetLookupKey(int rowIndex, int colIndex)
		{
			return 10000 * rowIndex + colIndex;
		}

		/// <summary>
		/// Used to track occupied cells and conflicts. The 
		/// triples are(dayPanel, gridrow, confictCount).
		/// </summary>
		int[,,] useTable = null;

		/// <summary>
		/// Tracks the known conflicts per dayPanel. ie; knownConflicts[i] is
		/// the largest number of conflicts for any timeslot in the ith dayPanel.
		/// </summary>
		int[] knownConflicts;

		/// <summary>
		/// A hole is a range to the right of a long IScheduleAppointment with conflicts
		/// which is not occupied by one of the conflicting IScheduleAppointments. Such
		/// wholes are treated as a coveredrange in the grid.
		/// </summary>
		GridRangeInfoList holes = null;

		/// <summary>
		/// A list of lists of AllDay items. ie., allDayItems[i] is an arraylist holding
		/// all the AllDay items for the ith dayPanel.
		/// </summary>
		List<IScheduleAppointment>[] allDayAndSpanItems = null;
    
        private void SyncGridWithAllDayAndSpanList()
        {
            for (int panel = 0; panel < allDayAndSpanItems.GetLength(0); panel++)
            {
                this.allDayGrids[panel].RowCount = allDayAndSpanItems[panel].Count;
                int row = 1;
                foreach (IScheduleAppointment item in allDayAndSpanItems[panel])
                {
                    if (item != null)
                    {
                        this.allDayGrids[panel][row, 1].Text = item.ToString();
                        this.allDayGrids[panel][row, 1].Tag = item;
                    }

                    row++;
                }
            }
        }

		/// <summary>
		/// Implements the algorithm that sets up the covered cells in the grid 
		/// to represent the times assoicated with each IScheduleAppointment. It handles 
		/// conflict allocations as well. 
		/// </summary>
		private void SetupCoveredCells()
		{
			if (coveredCells != null && coveredCells.Count > 0)
			{
                foreach (GridRangeInfo range in coveredCells)
                {
                    this.CoveredRanges.Remove(range);
                }

                foreach (GridRangeInfo range in holes)
                {
                    this.CoveredRanges.Remove(range);
                }
			}
			
			ScheduleAppointmentFromRange.Clear();
#if console
			if(this.numberPanels > 1)
				Console.WriteLine();
#endif
			coveredCells = new GridModelCoveredRanges(this.Model); ////GridRangeInfoList();
			useTable = new int[this.numberPanels, this.RowCount + 2, this.MaxConflicts + 1]; ////+2 used to avoid out of range exception when you add an all day row and move the mouse off the bottom
            allDayAndSpanItems = new List<IScheduleAppointment>[numberPanels]; ////new ArrayList[numberPanels];
			for (int i = 0; i < this.numberPanels; ++i)
			{
                allDayAndSpanItems[i] = new List<IScheduleAppointment>(); ////new ArrayList();
				this.allDayGrids[i].RowCount = 0;
			}

			this.RowHeights[allDayRow] = rowHeightAllDayRow;
            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                this.RowHeights[allDayRow] = Math.Max(this.RowHeights[allDayRow], rowHeightAllDayRow + rowHeight0);
			scheduleDataList.SortStartTime();
			
			knownConflicts = new int[this.numberPanels];
			knownConflicts[0] = 1;
		
			int panel = 0;
			
			for (int index = 0; index < scheduleDataList.Count; ++index)
			{
				IScheduleAppointment item = scheduleDataList[index];

				while (panel < this.numberPanels 
					&& item.StartTime.Date != this.displayDates[panel].Date
					&& item.EndTime.Date != this.displayDates[panel].Date)
				{
					panel += 1;
					if (panel < this.numberPanels)
					{
						knownConflicts[panel] = 1;
						this.allDayGrids[panel].RowCount = 0;
					}

					if (this.ScheduleType == ScheduleViewType.CustomWeek)
					{
						while (index < scheduleDataList.Count - 1
							&& item.StartTime.Date < this.displayDates[panel].Date)
						{
							index++;
							item = scheduleDataList[index];
						}
					}
				}

                if (panel == this.numberPanels)
                {
                    panel = this.numberPanels - 1;
                }

                bool isSpan = RecurrenceSupport.IsSpanItem(item);
                if (item.AllDay || isSpan)
				{
                    allDayAndSpanItems[panel].Add(item);
                    int rowIndex = allDayAndSpanItems[panel].Count;
                    this.RowHeights[allDayRow] = Math.Max(this.RowHeights[allDayRow], rowHeightAllDayRow + allDayAndSpanItems[panel].Count * rowHeight0);
                }

                if (item.AllDay || (isSpan && schedule.ShowAllSpansInAllDayPanel))
                {
                    continue;
                }

				if (item.StartTime.Date <= this.displayDates[this.displayDates.Length-1])
				{
					if (item.StartTime.Date == this.displayDates[panel].Date
						|| item.EndTime.Date == this.displayDates[panel].Date)
					{
                        int start = 0, end = 0;
                        if (this.Schedule != null && this.Schedule.AllowSecondsInAppointment)
                        {
                            start = (item.StartTime.Date == this.displayDates[panel].Date)
                              ? DateTimeToRowIndex(item.StartTime) : allDayRow + 1;
                            end = (item.EndTime.Date == this.displayDates[panel].Date)
                               ? DateTimeToRowIndex(item.EndTime) : this.RowCount;
                        }
                        else
                        {
                            start = (item.StartTime.Date == this.displayDates[panel].Date) ////this.Calendar.DateValue.Date)
                               ? DateTimeToRowIndex(item.StartTime) : allDayRow + 1;
                            end = (item.EndTime.Date == this.displayDates[panel].Date) ////this.Calendar.DateValue.Date)
                               ? DateTimeToRowIndex(item.EndTime.AddMinutes(-2)) : this.RowCount;
                        }
						int colIndex = 1;
						for (int i = start; i <= end; ++i)
						{
							if (this.useTable[panel, i, 0] >= colIndex)
							{
								colIndex = this.useTable[panel, i, 0] + 1;
							}

							if (colIndex >= this.MaxConflicts)
							{
                                colIndex = MaxConflicts - 1;
							}
						}

                        if (colIndex > this.knownConflicts[panel] && colIndex < this.MaxConflicts)
                        {
                            this.knownConflicts[panel] = colIndex;
                        }

                        for (int i = start; i <= end; ++i)
						{
							this.useTable[panel, i, 0] = colIndex;
							this.useTable[panel, i, colIndex] = index + 1;
						}
					}
				}
			}

			DisplayUseTable();
			panel = 0;
			int markCol1 = this.markCol;
			for (int index = 0; index < scheduleDataList.Count; ++index)
			{
				IScheduleAppointment item = scheduleDataList[index];

				while (panel < this.numberPanels 
					&& item.StartTime.Date != this.displayDates[panel].Date)
				{
					panel += 1;
					if (this.ScheduleType == ScheduleViewType.CustomWeek)
					{
						while (panel < this.numberPanels
							&& index < scheduleDataList.Count - 1
							&& item.StartTime.Date < this.displayDates[panel].Date)
						{
							index++;
							item = scheduleDataList[index];
						}
					}			
				}

				if (panel == this.numberPanels)
				{
					panel = this.numberPanels - 1;
				}

				markCol1 = this.markCol + panel * (this.dayColCount - 1);
				if (item.AllDay)
				{
					continue;
				}

				int start = (item.StartTime.Date == this.displayDates[panel].Date) ////this.Calendar.DateValue.Date)
					? DateTimeToRowIndex(item.StartTime) : allDayRow + 1;
				int end = (item.EndTime.Date == this.displayDates[panel].Date) ////this.Calendar.DateValue.Date)
					? DateTimeToRowIndex(item.EndTime.AddMinutes(-2)) : this.RowCount;
				int col = 1;
				int indexP1 = index + 1;
				while (col < this.MaxConflicts && this.useTable[panel, start, col] != indexP1)
				{
					col += 1;
				}

				bool rightMost = true;
				if (col < this.MaxConflicts - 1)
				{
					for (int i = start; i <= end; ++i)
					{
						if (this.useTable[panel, i, col + 1] != 0)
						{
							rightMost = false;
							break;
						}
					}
				}

				if (col < this.MaxConflicts)
				{
					int width = (this.dayColCount - this.markCol) / this.knownConflicts[panel];
					int left = markCol1 + 1 + (col-1) * width;
					int right = rightMost ? this.GetLastColInPanelFromCol(left) : left + width - 1;
					GridRangeInfo range = GridRangeInfo.Cells(start, left, end, right);
#if console
					Console.WriteLine(range);
#endif
					coveredCells.Add(range);
					ScheduleAppointmentFromRange.Add(GetLookupKey(start, left), item);					
				}
				else
				{
					////throw new Exception("Invalid UseTable.");
				}
			}

			MarkHolesAsRanges();
            SyncGridWithAllDayAndSpanList();
		}

		/// <summary>
		/// Computes GridRangeInfo's in a conflicted time-span that does not have 
		/// any appointments displayed in them. ie-holes in the dayPanel next to 
		/// occupied cells.
		/// </summary>
		private void MarkHolesAsRanges()
		{
			holes = new GridRangeInfoList();
			int markCol1 = this.markCol;

			for (int panel = 0; panel < this.numberPanels; ++panel)
			{
				markCol1 = this.markCol + panel * (this.dayColCount - 1);
				for (int i = allDayRow + 1; i <= this.RowCount; ++i)
				{
                    if (this.useTable[panel, i, 0] == 0)
                    {
                        continue;
                    }
                    
					int j = 1;
					while (j < this.MaxConflicts && this.useTable[panel, i, j] != 0)
					{
						j++;
					}

                    if (j == this.MaxConflicts)
                    {
                        continue;
                    }

					int id = this.useTable[panel, i, j - 1];
					bool hole = false;
					int k = i - 1;
					while (!hole && k > allDayRow && this.useTable[panel, k, j - 1] == id)
					{
						hole = this.useTable[panel, k, j] != 0;
						k--;
					}

					k = i + 1;
					while (!hole && k <= this.RowCount && this.useTable[panel, k, j - 1] == id)
					{
						hole = this.useTable[panel, k, j] != 0;
						k++;
					}

					if (hole)
					{
						int width = (this.dayColCount - this.markCol) / this.knownConflicts[panel];
						int left = markCol1 + 1 + (j-1) * width;
						 
						k = j + 1;
                        while (k <= this.knownConflicts[panel] && this.useTable[panel, i, k] == 0)
                        {
                            ++k;
                        }

						int right = k > this.knownConflicts[panel] ? this.GetLastColInPanelFromCol(left)  : left + (k - j) * width - 1;
						GridRangeInfo range = GridRangeInfo.Cells(i, left, i, right);
#if console
					Console.WriteLine("hole=" + range.ToString());
#endif
						holes.Add(range);
					}
				}
			}
		}
		
        /// <summary>
		/// Used only for debug purposes to display the use table.
		/// </summary>
		private void DisplayUseTable()
		{
#if console
			for(int panel = 0; panel < this.numberPanels; ++ panel)
			{
			Console.WriteLine("panel {0}", this.displayDates[panel]);
			for(int i = 0; i < this.useTable.GetLength(0); ++ i)
			{
				for(int j = 0; j < this.useTable.GetLength(1); ++ j)
				{
					if(j == 0)
						Console.Write("{0:000}: ", allDayRow + i + 1);
					Console.Write("{0} ", this.useTable[panel, i, j]);
				}
				Console.WriteLine(string.Empty);
			}
			}
#endif
		}
		#endregion

		/// <summary>
		/// Gets IScheduleAppointments from the IDateProvider based on the parameters passed in through
		/// the <see cref="ScheduleGrid"/> constructor and uses these IScheduleAppointments to populate
		/// this ScheduleGrid with day panels.
		/// </summary>
		public void SetDataToDayPanels()
		{
			SetDataToDayPanels(true);
		}

		ArrayList startIndexes;
		private void SetDataToDayPanels(bool resetData)
		{
			this.BeginUpdate();
			try
			{
				switch (this.ScheduleType)
				{
					case ScheduleViewType.CustomWeek:
					case ScheduleViewType.WorkWeek:
					case ScheduleViewType.Day:
                        if (resetData && scheduleDataList is IDisposable)
                        {
                            ((IDisposable)scheduleDataList).Dispose();
                        }

						if (resetData)
						{
							if (this.numberPanels > 1)
							{
								scheduleDataList = this.dataProvider.GetSchedule(this.displayDates[0], this.displayDates[this.numberPanels-1]);
							}
							else
							{
								this.displayDates = (DateTime[]) this.Calendar.SelectedDates.ToArray(typeof(DateTime));

								scheduleDataList = this.dataProvider.GetScheduleForDay(this.Calendar.SelectedDates[0]);
							}
						}

						SetupCoveredCells();
						if (coveredCells != null && coveredCells.Count > 0)
						{
                            foreach (GridRangeInfo range in coveredCells)
                            {
                                this.CoveredRanges.Add(range);
                            }

                            foreach (GridRangeInfo range in holes)
                            {
                                this.CoveredRanges.Add(range);
                            }
						}

						break;
					case ScheduleViewType.Month:
					{
                        this.IgnoreReadOnly = true;
                        if (resetData && scheduleDataList is IDisposable)
                        {
                            ((IDisposable)scheduleDataList).Dispose();
                        }

                        if (resetData)
                        {
                            scheduleDataList = this.dataProvider.GetSchedule(this.displayDates[0], this.displayDates[numberPanels - 1]);
                        }

                        int index = 0;
                        int itemCount = 0;
                        int row = 1;
                        int col = 1;
                        startIndexes = new ArrayList();
                        startIndexes.Add(0);
                        string text = string.Empty;
                        if (this.schedule.Culture.TextInfo.IsRightToLeft)
                        {
                            text = this.GetFormattedString(this.displayDates[0], this.Schedule.Appearance.WeekMonthNewMonth);
                            text = this.schedule.Culture.CompareInfo.LCID.Equals(1025) ? text : text + "\n"; // To exclude the ar-SA culture 
                        }
                        else
                        {
                            text = this.displayDates[0].ToString(Schedule.Appearance.WeekMonthNewMonth, this.Schedule.Culture) + "\n";
                        }
                        int offset = -1;
                        foreach (IScheduleAppointment item in scheduleDataList)
                        {
                            while (index < this.numberPanels && item.StartTime.Date > this.displayDates[index].Date)
                            {
                                this[row, col].Text = text;
                                if (!this.Schedule.Appearance.MonthShowFullWeek)
                                {
                                    if (col == 5)
                                    {
                                        offset = -1;
                                    }

                                    if (col < 6)
                                    {
                                        col += 1;
                                    }

                                    if (col >= 6 && offset < 1)
                                    {
                                        offset += 1;
                                        row += offset;
                                    }
                                    else if (offset >= 1)
                                    {
                                        row += 1;
                                        offset = -1;
                                        col = 1;
                                    }
                                }
                                else
                                {
                                    if (col < 7)
                                    {
                                        col += 1;
                                    }
                                    else
                                    {
                                        row += 1;
                                        col = 1;
                                    }
                                }

                                index += 1;
                                startIndexes.Add(itemCount);
                                if (index < numberPanels)
                                {
                                    text = this.displayDates[index].ToString("d", this.Schedule.Culture) + "\n";
                                }
                                else
                                {
                                    text = string.Empty;
                                }
                            }

                            itemCount += 1;

                            if (index < numberPanels)
                            {
                                if (item.StartTime.Date == this.displayDates[index].Date)
                                {
                                    text += this.ParseDisplayItem(item, this.WeekMonthItemFormat) + "\n";
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (text.Length > 0)
                        {
                            this[row, col].Text = text;
                        }

                        ////fill up any left at the end
                        index += 1;
                        if (index < this.numberPanels)
                        {
                            DateTime nextDate = this.displayDates[index];
                            while (nextDate <= this.endOfMonthCalendar)
                            {
                                if (!this.Schedule.Appearance.MonthShowFullWeek)
                                {
                                    if (col == 5)
                                    {
                                        offset = -1;
                                    }

                                    if (col < 6)
                                    {
                                        col += 1;
                                    }

                                    if (col >= 6 && offset < 1)
                                    {
                                        offset += 1;
                                        row += offset;
                                    }
                                    else if (offset >= 1)
                                    {
                                        row += 1;
                                        offset = -1;
                                        col = 1;
                                    }
                                }
                                else
                                {
                                    if (col < 7)
                                    {
                                        col += 1;
                                    }
                                    else
                                    {
                                        row += 1;
                                        col = 1;
                                    }
                                }

                                startIndexes.Add(0);
                                if (row <= this.RowCount)
                                {
                                    this[row, col].Text = nextDate.ToString("d", this.Schedule.Culture) + "\n"; ////text;
                                }
                                
                                nextDate = nextDate.AddDays(1);
                            }
                        }
                        
                        this.IgnoreReadOnly = false;
                    }

						break;
					case ScheduleViewType.Week:
					{
						this.IgnoreReadOnly = true;
                        if (resetData && scheduleDataList is IDisposable)
                        {
                            ((IDisposable)scheduleDataList).Dispose();
                        }

						if (resetData)
						{
							scheduleDataList = this.dataProvider.GetSchedule(this.displayDates[0], this.displayDates[numberPanels - 1]);
						}

						int index = 0;
						int itemCount = 0;
						int row = 1; 
						int col = 1;
						startIndexes = new ArrayList();
						startIndexes.Add(0);

						string text = this.Schedule.Appearance.FullWeekHeaderFormat.Length > 0 ?
                                      this.displayDates[0].ToString(this.Schedule.Appearance.FullWeekHeaderFormat, this.Schedule.Culture) + "\n"
                                      : " \n";
						foreach (IScheduleAppointment item in scheduleDataList)
						{
							while (index < this.numberPanels && item.StartTime.Date > this.displayDates[index].Date)
							{
								this[row, col].Text = text;
								row += 1;
								if (row > 3 && col == 1)
								{
									row = 1;
									col = 2;
								}

								index += 1;
								startIndexes.Add(itemCount);

                                if (index < numberPanels)
                                {
                                    text = this.Schedule.Appearance.FullWeekHeaderFormat.Length > 0 ?
                                        this.displayDates[index].ToString(this.Schedule.Appearance.FullWeekHeaderFormat, this.Schedule.Culture) + "\n"
                                        : " \n";
                                }
                                else
                                {
                                    text = string.Empty;
                                }
							}

							itemCount += 1;

                            if (index < numberPanels)
                            {
                                if (item.StartTime.Date == this.displayDates[index].Date)
                                {
                                    text += this.ParseDisplayItem(item, this.WeekMonthItemFormat) + "\n";
                                }
                            }
                            else
                            {
                                break;
                            }
						}

						if (text.Length > 0)
						{
							this[row, col].Text = text;
							row += 1;
							if (row > 3 && col == 1)
							{
								row = 1;
								col = 2;
							}
						}

						////need to fill up any left at the end.....
						for (int i = index+1; i < this.numberPanels; ++i)
						{
                            text = this.Schedule.Appearance.FullWeekHeaderFormat.Length > 0 ?
                                   this.displayDates[i].ToString(this.Schedule.Appearance.FullWeekHeaderFormat, this.Schedule.Culture) + "\n"
                                   : " \n";
							this[row, col].Text = text;
							row += 1;
							if (row > 3 && col == 1)
							{
								row = 1;
								col = 2;
							}
						}
				
						this.IgnoreReadOnly = false;
					}

						break;
					default:
						break;
				}
                GetScheduledDates();
			}
			catch
			{
                if (scheduleDataList is IDisposable)
                {
                    ((IDisposable)scheduleDataList).Dispose();
                }

				scheduleDataList = null;
			}
			finally
			{
				this.EndUpdate();
				this.Refresh();
			}
		}

		#endregion
		
		#region events
		
        /// <summary>
		/// Raised when the GridControl that displays the multiple day panels
		/// has completed its formatting and has been populated.
		/// </summary>
		/// <remarks>
		/// This event is an notification event that this GridControl 
		/// has been fully initialized. At this point, you can modify any of
		/// the GridControl's standard poperties, and your change will take precedence
		/// over the existing settings.
		/// </remarks>
		public event EventHandler GridSetUpCompleted;

        /// <summary>
        /// Raises the GridSetUpCompleted event.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        protected virtual void OnGridSetUpCompleted(EventArgs e)
        {
            if (GridSetUpCompleted != null)
            {
                GridSetUpCompleted(this, e);
            }
        }

        /// <summary>
        /// Allows you to set the default properties on a new schedule items.
        /// </summary>
        public event ScheduleAppointmentChangedEventHandler SetDefaultItemProperties;

        /// <summary>
        /// Sets inital default values on a schedule item and raises the <see cref="SetDefaultItemProperties"/> event
        /// to allow listeners the option of setting default values.
        /// </summary>
        /// <param name="item">The <see cref="ScheduleAppointmentEventArgs"/> holding the proposed current value of the schedule item.</param>
        protected virtual void OnSetDefaultItemProperties(IScheduleAppointment item)
        {
            item.LabelValue = 0;
            item.AllDay = false;
            item.MarkerValue = 2;
            if (SetDefaultItemProperties != null)
            {
                ScheduleAppointmentEventArgs e1 = new ScheduleAppointmentEventArgs(item, ItemAction.Default);
                SetDefaultItemProperties(this, e1);
            }
        }

		#endregion

		#region fields

		/// <summary>
		/// Array on cell grids that hold the allday appointments at top of a ScheduleGrid
		/// </summary>
		private CellEmbeddedGrid[] allDayGrids;
		
        /// <summary>
		/// The date that determines the daterange displayed in the ScheduleGrid.
		/// </summary>
		private DateTime displayDate;
		
        /// <summary>
		/// The determining dates of the daypanels that are in this ScheduleGrid.
		/// </summary>
		private DateTime[] displayDates;
		
        /// <summary>
		/// The active daypanel
		/// </summary>
		private int activePanel = 0;
		
        /// <summary>
		/// number of daypanels in this ScheduleGrid
		/// </summary>
		internal int numberPanels = 1;
		
        /// <summary>
		/// Color used to mark selections in an individual daypanel.
		/// </summary>
		private Color alphaBlendSelectionColor;
		
        /// <summary>
		/// Width of the time column on the left side of the ScheduleGrid
		/// </summary>
		private int timeWidth = 55;
		
        /// <summary>
		/// width of the top-most cell border of an occupied range in a daypanel.
		/// </summary>
		private int topOccupiedMargin = 2;
		
        /// <summary>
		/// Height of row0 (row 0)
		/// </summary>
		private int rowHeight0 = 20;
		
        /// <summary>
		/// default height of the rows inside the embedded allday grids.
		/// </summary>
		private int allDayGridDefaultRowHeight = 20;
		
        /// <summary>
		/// height of the cell that holds the embedded allday grids.
		/// </summary>
		private int rowHeightAllDayRow = 30;
		
        /// <summary>
		/// row holding the allday grids
		/// </summary>
		private int allDayRow = 1;
		
        /// <summary>
		/// the caption row
		/// </summary>
		private int row0 = 0;
		
        /// <summary>
		/// caches the brushinfo used to draw the caption row gradient
		/// </summary>
		private BrushInfo row0BrushInfo = null;
		
        /// <summary>
		/// width of the occupied marker column
		/// </summary>
		private int markWidth = 6;
		
        /// <summary>
		/// col index of the marker column
		/// </summary>
		private int markCol = 2;
		
        /// <summary>
		/// col index of the time col
		/// </summary>
		private int timeCol = 1;
		
        /// <summary>
		/// BorderStyle.None border - used in marker column
		/// </summary>
		private GridBorder noBottomBorder = null;
		
        /// <summary>
		/// a solid border - used in time column
		/// </summary>
		private GridBorder solidBorder = null;
        private GridBorder defaultBorder = null;
		
        /// <summary>
		/// holds text used in the Schedule.HeaderLabel
		/// </summary>
		private string columnHeaderText = string.Empty;
        private DateTime columnHeaderDate = DateTime.Now;

        internal DateTime ColumnHeaderDate
        {
            get
            {
                return columnHeaderDate;
            }

            set
            {
                columnHeaderDate = value;
            }
        }
        
		/// <summary>
		/// internal flag indicates whether the space required for displaying the large and small text in the time col needs to be reset.
		/// </summary>
		private bool resetTimeRects = true;
		
        /// <summary>
		/// size of the large text in the time col display
		/// </summary>
		private Size timeLargeSize;
		
        /// <summary>
		/// size of the small text in the time col display
		/// </summary>
		private Size timeSmallSize;

		/// <summary>
		/// holds the currently loaded IScheduleAppointment objects
		/// </summary>
		internal IScheduleAppointmentList _scheduleDataList = null;
        internal IScheduleAppointmentList scheduleDataList
        {
            get
            {
                return _scheduleDataList;
            }

            set
            {
                _scheduleDataList = value;
                CheckSpanOnlyPrimeTime();
            }
        }
		
		/// <summary>
		/// holds the covered cells used in marking schedule items in the daypanels
		/// </summary>
		private GridModelCoveredRanges coveredCells = null;

		#endregion

		#region Properties

        /// <summary>
        /// The NavigationCalendar used to determine the dates displayed in this ScheduleGrid.
        /// </summary>
        private NavigationCalendar calendar;
        
        /// <summary>
        /// A read-only property that gets the NavigationCalendar used to determine the dates displayed.
        /// </summary>
        public NavigationCalendar Calendar
        {
            get
            {
                return calendar;
            }
        }

        /// <summary>
        /// The ScheduleControl that hosts this ScheduleGrid.
        /// </summary>
        private ScheduleControl schedule;
        
        /// <summary>
        /// A read-only property that gets the ScheduleControl that hosts this ScheduleGrid.
        /// </summary>
        public ScheduleControl Schedule
        {
            get
            {
                return schedule;
            }
        }

		private IScheduleDataProvider dataProvider = null;
		
        /// <summary>
		/// A property that gets or sets the IScheduleDataProvider data source for this ScheduleControl.
		/// </summary>
		[Browsable(false)]
		[Description("The IScheduleDataProvider data source for this control.")]
		public IScheduleDataProvider DataSource
		{
            get
            {
                return dataProvider;
            }

			set
			{ 
				if (dataProvider != value)
				{
					if (dataProvider == null)
					{
						SetupChildControls();
					}

					dataProvider = value;
					SetDataToDayPanels();
				}
			}
		}

		private ScheduleViewType scheduleType;

		/// <summary>
		/// A property that gets / sets the whether a daily, weekly or monthly schdule is displayed.
		/// </summary>
		[Browsable(true)]
		[Description("Gets/sets the whether a daily, weekly or monthly schdule is displayed.")]
		[DefaultValue(ScheduleViewType.Day)]
		public ScheduleViewType ScheduleType
		{
            get
            {
                return scheduleType;
            }

			set
			{
				if (scheduleType != value)
				{
					scheduleType = value;
				}
			}
		}
		#endregion
	
		#region Grid Events

        private int smallColWidth = -1;
        private int smallColAdjust = 0;

		/// <summary>
		/// Determines colwidths dynamically.
		/// </summary>
		/// <param name="sender">Source of the event.</param>
		/// <param name="e">The GridRowColSizeEventArgs.</param>
        private void Grid_QueryColWidth(object sender, Syncfusion.Windows.Forms.Grid.GridRowColSizeEventArgs e)
        {
            if (this != null && !this.IsDisposed && !this.IsDisposing)
            {
                if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
                {
                    if (e.Index == timeCol)
                    {
                        e.Size = this.Schedule.Appearance.ShowTime ? timeWidth : 0;
                        e.Handled = true;
                        return;
                    }
                    else if (this.MarkCol(e.Index))
                    {
                        e.Size = markWidth;

                        e.Handled = true;
                        return;
                    }

                    if (smallColWidth == -1)
                    {
                        smallColWidth = 0; ////prevent recursive call
                        int targetColWidth = Math.Max(this.ClientSize.Width - this.ColWidths.GetTotal(0, this.markCol - 1) - this.numberPanels * markWidth, 0) / this.numberPanels;
                     
                        smallColWidth = (int)((float)targetColWidth / (dayColCount - 2)); ////-2 because of the mark column
                        smallColAdjust = targetColWidth - (dayColCount - 2) * smallColWidth;
                    }
                }

                if (e.Index == this.ColCount)
                {
                    e.Size = Math.Max(this.ClientSize.Width - this.ColWidths.GetTotal(0, this.ColCount - 1), 0);
                    e.Handled = true;
                }
                else if (e.Index < this.ColCount && e.Index > 0)
                {
                    if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
                    {
                        if (e.Index - 3 >= 0 && ((e.Index - 3) % dayColCount < smallColAdjust))
                        {
                            e.Size = smallColWidth + 1;
                        }
                        else
                        {
                            e.Size = smallColWidth;
                        }
                    }
                    else
                    {
                        e.Size = this.ClientSize.Width / this.ColCount;
                    }

                    e.Handled = true;
                }
            }
        }

		/// <summary>
		/// determines the row heights dynamcially
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="e">The GridRowColSizeEventArgs.</param>
		private void Grid_QueryRowHeight(object sender, Syncfusion.Windows.Forms.Grid.GridRowColSizeEventArgs e)
		{ 
			if (this != null && !this.IsDisposed && !this.IsDisposing)
			{
				if (this.ScheduleType == ScheduleViewType.Week)
				{
					int h = this.ClientSize.Height / 3;
					if (e.Index == this.RowCount)
					{
						e.Size = this.ClientSize.Height - this.RowHeights.GetTotal(0, this.RowCount - 1); 
						e.Handled = true;
					}
					else if (e.Index < this.RowCount - 1 && e.Index > 0)
					{
						e.Size = h;
						e.Handled = true;
					}
					else if (e.Index == this.RowCount - 1 && e.Index > 0)
					{
						e.Size = h / 2;
						e.Handled = true;
					}
				} 
				else if (this.ScheduleType == ScheduleViewType.Month)
				{
					int h = this.ClientSize.Height  / this.RowCount - 1;
					if (e.Index == this.RowCount)
					{
						e.Size = this.ClientSize.Height - this.RowHeights.GetTotal(0, this.RowCount - 1); 
						e.Handled = true;
					}
					else if (e.Index > 0)
					{
						e.Size = h; //// + ((e.Index % 2) == 0 ? -1 : 0);
						e.Handled = true;
					}
				}
			}
		}

        private void Grid_QueryRowHeightDay(object sender, Syncfusion.Windows.Forms.Grid.GridRowColSizeEventArgs e)
        {
            if (this != null && !this.IsDisposed && !this.IsDisposing)
            {
                if (schedule.Appearance.DivisionsPerRow > 1 && e.Index > 1)
                {
                    e.Size = Math.Max(this.DefaultRowHeight / schedule.Appearance.DivisionsPerRow, ScheduleGrid.MinimumGridRowHeight);
                    e.Handled = true;
                }
            }
        }

		/// <summary>
		/// provides cell look and values dynamically
		/// </summary>
		/// <param name="sender">Event source.</param>
		/// <param name="e">The GridQueryCellInfoEventArgs.</param>
		private void Grid_QueryCellInfo(object sender, Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventArgs e)
		{
          	if (this.coveredCells != null && this != null && !this.IsDisposed && !this.IsDisposing)
			{
                if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
                {
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro && e.RowIndex == this.row0 && e.ColIndex > 2 || e.RowIndex == this.row0)
                    {
                        e.Style.Interior = row0BrushInfo;
                    }
                    else if (e.RowIndex == this.row0 && e.ColIndex < 3 && this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        e.Style.Interior = new BrushInfo(this.Schedule.Appearance.TimeBackColor);
                    }
                    else if (MarkCol(e.ColIndex) &&
                        (e.RowIndex > allDayRow || this.numberPanels > 1))
                    {
                        e.Style.Borders.Bottom = noBottomBorder;
                        e.Style.Borders.Right = this.solidBorder;
                        e.Style.Enabled = false;

                        if (e.RowIndex <= allDayRow && this.numberPanels > 1)
                        {
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                if (e.RowIndex == 1 && e.ColIndex != 2)
                                {
                                    e.Style.BackColor = Color.White;// this.Schedule.Appearance.AllDayBackColor;
                                }
                                else if (e.ColIndex == 2)
                                {
                                    e.Style.BackColor = this.Schedule.Appearance.TimeBackColor;
                                }
                            }
                            else
                                e.Style.BackColor = this.Schedule.Appearance.MarkColumnColor;
                            if (e.RowIndex == 0)
                            {
                                e.Style.Borders.All = noBottomBorder;
                            }

                            e.Style.Text = " ";
                            return;
                        }

                        if (!this.coveredCells.Ranges.AnyRangeContains(GridRangeInfo.Auto(e.RowIndex, this.GetFirstColInPanelFromCol(e.ColIndex))))
                        {
                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                            {
                                e.Style.BackColor = this.Schedule.Appearance.TimeBackColor;
                            }
                            else
                                e.Style.BackColor = this.Schedule.Appearance.MarkColumnColor;
                        }
                        else
                        {
                            int key = GetLookupKey(e.RowIndex, e.ColIndex + 1);
                            int row = e.RowIndex;
                            while (row > this.allDayRow && !ScheduleAppointmentFromRange.ContainsKey(key))
                            {
                                row--;
                                key = GetLookupKey(row, e.ColIndex + 1);
                            }

                            if (ScheduleAppointmentFromRange.ContainsKey(key))
                            {
                                IScheduleAppointment item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                                if (item != null)
                                {
                                    Color c = ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember;

                                    e.Style.BackColor = c;
                                    if (this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
                                        e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, c, GridBorderWeight.Thin);
                                }
                            }
                        }
                    }
                    else if (!MarkCol(e.ColIndex) && e.RowIndex > allDayRow)
                    {
                        int key = GetLookupKey(e.RowIndex, e.ColIndex);
                        if (ScheduleAppointmentFromRange.ContainsKey(key))
                        {
                            IScheduleAppointment item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                            if (item != null)
                            {
                                Color c1 = ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember;
                                e.Style.Text = item.ToString();
                                ////used to not show covered range as selected when dragging to select
                                if (item.LabelValue > -1 &&
                                    (!this.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex)
                                    || this.Selections.Ranges.ActiveRange.Height > 1))
                                {
                                    Color c = (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? Color.FromArgb(100, ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember) : ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember;
                                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                    {
                                        BrushInfo b = new BrushInfo(c);
                                        e.Style.Interior = b;
                                    }
                                    else
                                    {
                                        BrushInfo b = new BrushInfo(
                                       GradientStyle.ForwardDiagonal,
                                       c,
                                       Color.FromArgb(20, c));
                                        e.Style.Interior = b;
                                        e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, c1, GridBorderWeight.Medium); ////
                                    }
                                }
                                if (this.Schedule.Appearance.VisualStyle != GridVisualStyles.Metro)
                                {
                                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, c1, GridBorderWeight.Medium); ////occupiedBorder;
                                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, c1, GridBorderWeight.Medium); ////occupiedBorder;
                                }
                                if (e.ColIndex != GetFirstColInPanelFromCol(e.ColIndex))
                                {
                                    e.Style.BorderMargins.Left = markWidth;
                                    e.Style.TextMargins.Left = 2;
                                }
                                else
                                {
                                    e.Style.TextMargins.Top = topOccupiedMargin;
                                }
                            }
                        }
                    }
                    else if (e.RowIndex == this.allDayRow && e.ColIndex == this.timeCol)
                    {
                        e.Style.Borders.Bottom = this.solidBorder;
                    }

                    if (schedule.Appearance.DivisionsPerRow > 1)
                    {
                        if (e.ColIndex > timeCol && (e.RowIndex - 1) % schedule.Appearance.DivisionsPerRow != 0)
                        {
                            e.Style.Borders.Bottom = GridBorder.Empty;
                        }
                        else if (e.ColIndex > timeCol && e.Style.Borders.Bottom == GridBorder.Empty)
                        {
                            e.Style.Borders.Bottom = defaultBorder;
                        }
                    }
                }
    		}
		}

        private void FixTextRectangle(GridControl grid, GridDrawCellDisplayTextEventArgs e)
        {
            Rectangle textRectangle = e.TextRectangle;
            textRectangle.Width = grid.GetColWidth(e.ColIndex);
            e.TextRectangle = textRectangle;
            e.Style.WrapText = false;
        }
         
        ////draws the cell text in the allday cell at the top of the schedule
		private void allDayGrid_DrawCellDisplayText(object sender, GridDrawCellDisplayTextEventArgs e)
		{
			if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
			{
                int col = e.Style.CellIdentity.ColIndex;
				int row = e.Style.CellIdentity.RowIndex;
				if (row > 0)
				{
                     IScheduleAppointment item = e.Style.Tag as IScheduleAppointment;
                    if (item != null)
					{
                        CellEmbeddedGrid allDayGrid = sender as CellEmbeddedGrid;
                        Rectangle rect = allDayGrid.RangeInfoToRectangle(GridRangeInfo.Cell(row, col));

						e.DisplayText = this.ParseDisplayItem(item, this.AllDayItemFormat);
                       
                        FixTextRectangle(allDayGrid, e);
                        if (RecurrenceSupport.IsSpanItem(item))
                        {
                            int dateCount = this.displayDates.GetLength(0);
                            DateTime dt = ClickDateTime(row, col).Date;
                            if (item.StartTime.Date >= displayDates[0].Date && item.StartTime.Date <= displayDates[dateCount - 1].Date)
                            {
                                IRecurringScheduleAppointment appt = item as IRecurringScheduleAppointment;
                                DateTime lastDisplayDate = this.displayDates[this.displayDates.GetLength(0) - 1];
                                int baseloc = 0;
                                GridRangeInfo range;
                                this.CoveredRanges.Find(0, 3, out range); ////width of first header on main grid
                                int headerWidth = this.RangeInfoToRectangle(range).Width + 1;
                                while (!allDayGrids[baseloc].Equals(allDayGrid))
                                {
                                    baseloc++;
                                    this.CoveredRanges.Find(0, range.Right + 1, out range);
                                }

                                if (item.StartTime.Date == this.displayDates[baseloc].Date)
                                {
                                    int width = 0;
                                    int loc = baseloc;
                                    while (loc < allDayGrids.GetLength(0) && loc < dateCount
                                         && this.displayDates[loc].Date <= appt.DateList.TerminalDate.Date)
                                    {
                                        width += this.RangeInfoToRectangle(range).Width;
                                        if (this.displayDates[loc].Date == appt.DateList.TerminalDate.Date)
                                        {
                                            width += 1;
                                        }

                                        this.CoveredRanges.Find(0, range.Right + 1, out range);
                                        loc++;
                                    }

                                    rect.Width = width;
                                    if (dateCount == 1)
                                    {
                                        rect.Width -= timeWidth;
                                    }

                                    if (!AddAllDaySpan(Rectangle.Empty, rect, item as IRecurringScheduleAppointment, false, e.Style))
                                    {
                                        e.DisplayText = string.Empty; ////skip it
                                    }
                                }
                            }

                            e.DisplayText = string.Empty;
                        }
                        else
                        {
                            Color c = ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember;
                            if (this.schedule.ShowRoundedCorners && this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
                            {
                                rect.Inflate(-4, -1);
                                System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = ScheduleGrid.DrawRoundRect(
                                    rect.X, 
                                    rect.Y,
                                    rect.Width, 
                                    rect.Height, 
                                    Math.Min(rect.Height / 4, 12));
                                if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                {
                                    c = Color.FromArgb(100, ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember);
                                    using (Brush b = new SolidBrush(c))
                                    {
                                        e.Graphics.FillPath(b, myGraphicsPath);
                                    }
                                }
                                else
                                {
                                    using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, c, Color.FromArgb(20, c), LinearGradientMode.ForwardDiagonal))
                                    {
                                        e.Graphics.FillPath(b, myGraphicsPath);
                                    }
                                }
                                if (item.Equals(mouseDownItem))
                                {
                                    using (Pen p = new Pen(schedule.Appearance.ClickItemBorderColor))
                                    {
                                        e.Graphics.DrawPath(p, myGraphicsPath);
                                    }
                                }

                                myGraphicsPath.Dispose();
                                rect.Y += 2;
                                GridStaticCellRenderer.DrawText(e.Graphics, e.DisplayText, e.Style.GdipFont, rect, e.Style, e.Style.TextColor, false);
                                e.Cancel = true;
                            }
                            else
                            {
                                rect.Height -= 2;
                                rect.Offset(0, -4);
                                if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                {
                                    c = Color.FromArgb(100, ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember);
                                    using (Brush b2 = new SolidBrush(c))
                                    {
                                        e.Graphics.FillRectangle(b2, rect);
                                    }
                                }
                                else
                                {
                                    using (Brush b2 = new System.Drawing.Drawing2D.LinearGradientBrush(rect, c, Color.FromArgb(20, c), LinearGradientMode.ForwardDiagonal))
                                    {
                                        e.Graphics.FillRectangle(b2, rect);
                                    }
                                }
                                if (item.Equals(mouseDownItem))
                                {
                                    using (Pen p = new Pen(schedule.Appearance.ClickItemBorderColor))
                                    {
                                        rect.Width = rect.Width - 1;
                                        rect.Height = rect.Height - 1;
                                        e.Graphics.DrawRectangle(p, rect);
                                    }
                                }

                                rect.Y += 2;
                                GridStaticCellRenderer.DrawText(e.Graphics, e.DisplayText, e.Style.GdipFont, rect, e.Style, e.Style.TextColor, false);
                                e.Cancel = true;
                            }
                        }
					}
				}
			}
		}

        ////draws the warpped text in the week & month display
        private void Grid_DrawCellDisplayText(object sender, GridDrawCellDisplayTextEventArgs e)
        {
            if (this != null && !this.IsDisposed && !this.IsDisposing)
            {
                Rectangle MetroDateRect = e.TextRectangle;
                if (this.ScheduleType == ScheduleViewType.Week)
                {
                    ////draw whole cell background
                    Color cellBackColor = this.Schedule.Appearance.PrimeTimeCellColor;
                    string[] list = e.DisplayText.Split(new char[] { '\n' });
                    int n = list.GetLength(0) - 1;
                    DateTime dt1 = DateTime.Parse(list[0], this.Schedule.Culture).Date;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                    {
                        if (this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex))
                        {
                            cellBackColor = Color.FromArgb(208, 208, 208); 
                            e.Style.Font.Bold = false;
                        }
                    }
                    using (Brush b = new SolidBrush(cellBackColor))
                    {
                        e.Graphics.FillRectangle(b, e.TextRectangle);
                    }
                    if (n > 0)
                    {
                        Rectangle rect = e.TextRectangle;
                        int h = (int)(e.Style.Font.Size + 1);
                        rect.Height = 2 * h;

                        Font f = e.Style.GdipFont;
                        
                        ////draw the header
                        int currentCellPanel = GetWeekPanelFromRowCol(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex); ////Math.Max(0, (this.CurrentCell.ColIndex - 1) * 3 + this.CurrentCell.RowIndex - 1);
                        int panel = GetWeekPanelFromRowCol(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex); ////Math.Max(0, (e.Style.CellIdentity.ColIndex -1) * 3 + e.Style.CellIdentity.RowIndex - 1);

                        if (currentCellPanel < this.displayDates.GetLength(0))
                        {
                            if (list[0] == this.displayDates[currentCellPanel].Date.ToString(this.Schedule.Appearance.FullWeekHeaderFormat, this.Schedule.Culture))
                            {
                                if (this.Schedule.Appearance.ThemesEnabled)
                                {
                                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.SystemTheme)
                                    {
                                        // SolidBrush(c))
                                        using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, this.Schedule.Appearance.MonthWeekHeaderForeColor, this.Schedule.Appearance.MonthWeekHeaderBackColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                                        {
                                            e.Graphics.FillRectangle(b, rect);
                                        }
                                    }
                                    else if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                    {
                                        if (dt1.Date == this.schedule.Calendar.Today.Date)
                                        {
                                            Rectangle todayRect = e.TextRectangle;
                                            todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                            todayRect.Y -= 1;
                                            todayRect.X -= 1;
                                            todayRect.Width += 2;
                                            using (Brush todayb = new SolidBrush(this.Schedule.Appearance.NavigationCalendarHeaderColor))
                                            {
                                                e.Graphics.FillRectangle(todayb, todayRect);
                                            }
                                        }
                                        else
                                        {
                                            using (Brush b = new SolidBrush(cellBackColor))
                                            {
                                                e.Graphics.FillRectangle(b, rect);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect, ThemedHeaderDrawing.HeaderState.Pressed);
                                    }
                                }
                                else
                                {
                                    using (Brush b = new SolidBrush(this.Schedule.Appearance.AllDayBackColor))
                                    {
                                        e.Graphics.FillRectangle(b, rect);
                                    }
                                }
                            }
                            else
                            {
                                if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                                {
                                    if (dt1.Date == this.schedule.Calendar.Today.Date)
                                    {
                                        Rectangle todayRect = e.TextRectangle;
                                        todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                        todayRect.Y -= 1;
                                        todayRect.X -= 1;
                                        todayRect.Width += 2;
                                        using (Brush todayb = new SolidBrush(this.Schedule.Appearance.TodayBackColor))
                                        {
                                            e.Graphics.FillRectangle(todayb, todayRect);
                                        }
                                    }
                                    else
                                    {
                                        using (Brush b = new SolidBrush(cellBackColor))
                                        {
                                            e.Graphics.FillRectangle(b, rect);
                                        }
                                    }
                                }
                                else
                                {
                                    using (Brush b = new SolidBrush(this.Schedule.Appearance.AllDayBackColor))
                                    {
                                        e.Graphics.FillRectangle(b, rect);
                                    }
                                }
                            }
                        }

                        e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                        e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                        e.Style.WrapText = false;

                        DateTime dt = DateTime.Parse(list[0], this.Schedule.Culture).Date;
                        bool rtl = schedule.RightToLeft == RightToLeft.Yes;


                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            MetroDateRect = new Rectangle(rect.X, rect.Y + 3, rect.Width, rect.Height);
                            GridStaticCellRenderer.DrawText(e.Graphics, GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat), f, rect, e.Style, e.Style.TextColor, rtl);
                        }
                        else
                            GridStaticCellRenderer.DrawText(e.Graphics, GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat), f, rect, e.Style, e.Style.TextColor, rtl);

                        e.Style.BackColor = Color.FromArgb(0, Color.Red);
                        e.Style.WrapText = false;

                        int indent = 6;
                        rect.X += 3 * indent;
                        rect.Width -= 6 * indent;
                        GridRangeInfo range = this.CoveredRanges.FindRange(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                        if (range.IsEmpty)
                        {
                            range = GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                        }
                        Rectangle textRectangle = this.RangeInfoToRectangle(range);
                        int bottomLimit = 0;
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                            bottomLimit = textRectangle.Bottom - 18;
                        else
                            bottomLimit = textRectangle.Bottom - MoreItemsBitmap.Height - 9;

                        if (panel < this.startIndexes.Count)
                        {
                            int start = (int)startIndexes[panel] - 1;
                            for (int i = 1; i < n; ++i)
                            {
                                rect.Y = rect.Bottom;
                                if (rect.Bottom < bottomLimit)
                                {
                                    IScheduleAppointment item = this.scheduleDataList[start + i] as IScheduleAppointment;
                                    Color c1 = ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember;
                                    Color c = Color.FromArgb(100, c1);
                                    Rectangle rect1 = rect;
                                    rect1.Width += 4 * indent;
                                    rect1.X -= 2 * indent;
                                    rect1.Y += 1;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        rect1.Y += 4;
                                        rect1.Height += 10;
                                        rect.Y += 16;
                                    }
                                    else
                                        rect1.Height -= 2;
                                    
                                    Color cb2 = Color.FromArgb(255, ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember);
                                        Rectangle rectMarkup = e.ClipBounds;
                                        Color markerColor = Color.FromArgb(100, ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember);
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        rectMarkup.Y = rect1.Y;
                                        rectMarkup.Height = rect1.Height;
                                        rectMarkup.Width = rect1.X - rectMarkup.X;
                                        using (Brush b = new SolidBrush(cb2))
                                        {
                                            e.Graphics.FillRectangle(b, rectMarkup); // fill rectangle
                                        }
                                    }
                                    using (Brush b = new SolidBrush(c))
                                    {
                                        e.Graphics.FillRectangle(b, rect1);
                                        
                                            Rectangle borderRect = rect1;
                                            borderRect.X = rectMarkup.X;
                                            borderRect.Y = rectMarkup.Y;
                                            borderRect.Width = rect1.Width + rectMarkup.Width;
                                            if (item.LabelValue == 0 && item.MarkerValue == 0)
                                            {
                                                using (Pen p = new Pen(this.Schedule.Appearance.SolidBorderColor))
                                                {
                                                    e.Graphics.DrawRectangle(p, borderRect);
                                                }
                                            }
                                            else if (item.LabelValue == 0)
                                            {
                                                using (Pen p = new Pen(markerColor))
                                                {
                                                    e.Graphics.DrawRectangle(p, borderRect);
                                                }
                                            }
                                    }
                                    if (item.Equals(this.mouseDownItem))
                                    {
                                        mouseDownRectangle = rect1;
                                        mouseDownRectangle.X = rectMarkup.X;
                                        mouseDownRectangle.Y = rectMarkup.Y;
                                        mouseDownRectangle.Width = rect1.Width + rectMarkup.Width;
                                        using (Pen p = new Pen(this.Schedule.Appearance.ClickItemBorderColor))
                                        {
                                            e.Graphics.DrawRectangle(p, mouseDownRectangle);
                                        }
                                    }
                                    if (this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
                                    {
                                        using (Pen pen = new Pen(new SolidBrush(cb2)))
                                        {
                                            pen.Width = 2;
                                            e.Graphics.DrawRectangle(pen, rect1);
                                        }
                                    }
                                    rtl = schedule.RightToLeft == RightToLeft.Yes;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        Rectangle textRect = rect;
                                        textRect.Y -= 6;
                                        GridStaticCellRenderer.DrawText(e.Graphics, list[i], f, textRect, e.Style, e.Style.TextColor, rtl);
                                    }
                                    else
                                    {
                                        GridStaticCellRenderer.DrawText(e.Graphics, list[i], f, rect, e.Style, e.Style.TextColor, rtl);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (rect.Bottom > bottomLimit)
                        {
                            rect = this.RangeInfoToRectangle(GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex));
                            if (this.GridVisualStyles == GridVisualStyles.Metro)
                            {
                                string str = (n - 1).ToString() + " Events";
                                MetroDateRect.X += e.TextRectangle.Width - 55;
                                if (e.Graphics.DpiX > 96)
                                {
                                    Font dpiFont = new Font(f.FontFamily.Name, 8.25f);
                                    GridStaticCellRenderer.DrawText(e.Graphics, str, dpiFont, MetroDateRect, e.Style, e.Style.TextColor, this.IsRightToLeft());
                                }
                                else
                                    GridStaticCellRenderer.DrawText(e.Graphics, str, f, MetroDateRect, e.Style, e.Style.TextColor, rtl);
                            }
                            else
                            {
                                rect.Y += e.TextRectangle.Height - ScheduleGrid.MoreItemsBitmap.Height - 1;
                                rect.X += e.TextRectangle.Width - ScheduleGrid.MoreItemsBitmap.Width - 1;
                                rect.Height = ScheduleGrid.MoreItemsBitmap.Height;
                                rect.Width = ScheduleGrid.MoreItemsBitmap.Width;
                                iconPainter.PaintIcon(e.Graphics, rect, Point.Empty, ScheduleGrid.MoreItemsBitmap, Color.Black);
                            }
                        }
                    }

                    e.Cancel = true;
                }
                else if (this.ScheduleType == ScheduleViewType.Month)
                {
                    string[] list = e.DisplayText.Split(new char[] { '\n' });
                    int n = list.GetLength(0) - 1;
                    if (n > 0)
                    {
                        Rectangle rect = e.TextRectangle;
                        int h = (int)(e.Style.Font.Size + 1);
                        rect.Height = 2 * h - 1;

                        bool isFirstCell = e.ColIndex == 1 && e.RowIndex == 1;
                        bool isRTLCulture = this.schedule.Culture.TextInfo.IsRightToLeft;
                        DateTime dt = (isFirstCell && !isRTLCulture) ? DateTime.ParseExact(list[0], this.Schedule.Appearance.WeekMonthNewMonth, this.Schedule.Culture).Date
                            : DateTime.Parse(list[0], this.Schedule.Culture).Date;

                        int slot = SpanManager.BottomSlot(dt);

                        Font f = e.Style.GdipFont;
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            if (dt.Month == 1 || (e.Style.CellIdentity.RowIndex == 1 && e.Style.CellIdentity.ColIndex == 1))
                                f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            else
                                f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        }
                        ////draw whole cell background
                        Color c1 = dt.Month == columnHeaderDate.Month ? this.Schedule.Appearance.PrimeTimeCellColor : this.Schedule.Appearance.NonPrimeTimeCellColor;
                        ////this.Schedule.Appearance.PrimeTimeCellColor))////baseCellColorPrime))
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            if (this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex))
                            {
                                c1 = Color.FromArgb(208, 208, 208);  //Color.FromArgb(169, 169, 169);
                            }
                        }
                        using (Brush b = new SolidBrush(c1))
                        {
                            Rectangle rect2 = e.TextRectangle;
                            rect2.Width += 2;
                            rect2.X -= 1;
                            rect2.Height += 2;
                            rect2.Y -= 1;
                            e.Graphics.FillRectangle(b, rect2); ////e.TextRectangle);
                        }
                        ////draw the header
                        int currentCellPanel = GetMonthPanelFromRowCol(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex);
                        int panel = GetMonthPanelFromRowCol(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);

                        e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                        e.Style.VerticalAlignment = GridVerticalAlignment.Top;
                        e.Style.WrapText = false;
                        string s;
                        if (dt.Day == 1)
                        {                               ////long date pattern
                            s = (dt.Month == 1) ? GetFormattedString(dt, schedule.Appearance.WeekMonthFullFormat) : GetFormattedString(dt, schedule.Appearance.WeekMonthNewMonth);
                        }
                        else
                        {
                            s = (e.Style.CellIdentity.RowIndex == 1 && e.Style.CellIdentity.ColIndex == 1)
                                ? GetFormattedString(dt, schedule.Appearance.WeekMonthNewMonth) : dt.Day.ToString();
                        }

                        ThemedHeaderDrawing.HeaderState headerState = this.CurrentCell.HasCurrentCellAt(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex)
                            ? ThemedHeaderDrawing.HeaderState.Pressed : ThemedHeaderDrawing.HeaderState.Normal;
                        int alpha = headerState == ThemedHeaderDrawing.HeaderState.Pressed ? 255 : 128;
                        //// draw back ground
                        Rectangle rect1 = e.TextRectangle;
                        rect1.Height = 2 * h - 1;
                        if (this.Schedule.Appearance.ThemesEnabled)
                        {

                            if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                            {
                                if (dt.Date == this.schedule.Calendar.Today.Date)
                                {
                                    Rectangle todayRect = e.TextRectangle;
                                    todayRect.Height = (e.TextRectangle.Height / 40) + 2;
                                    todayRect.Y -= 1;
                                    using (Brush todayb = new SolidBrush(this.Schedule.Appearance.TodayBackColor))
                                    {
                                        e.Graphics.FillRectangle(todayb, todayRect);
                                    }
                                }
                                else
                                {
                                    using (Brush b = new SolidBrush(c1))
                                    {
                                        e.Graphics.FillRectangle(b, rect1);
                                    }
                                }
                            }
                            else if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.SystemTheme)
                            {
                                //// SolidBrush(c))
                                using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderForeColor), Color.FromArgb(alpha, this.Schedule.Appearance.MonthWeekHeaderBackColor), System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                                {
                                    e.Graphics.FillRectangle(b, rect1);
                                }
                            }
                            else
                            {
                                this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect1, headerState);
                                if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                {
                                    this.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(e.Graphics, rect1, headerState);
                                }
                            }
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(Color.FromArgb(alpha, this.Schedule.Appearance.AllDayBackColor)))
                            {
                                e.Graphics.FillRectangle(b, rect);
                            }
                        }
                        bool rtl = schedule.RightToLeft == RightToLeft.Yes;
                        Color c5 = Color.Red;
                        if (dt.Date == this.Schedule.Calendar.Today.Date && this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            c5 = this.Schedule.Appearance.TodayBackColor;//Color.White;
                            f = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        }
                        else
                        {
                            c5 = e.Style.TextColor;
                        }
                        MetroDateRect = new Rectangle(rect1.X, rect1.Y + 3, rect1.Width, rect1.Height);
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            if (e.Graphics.DpiX > 96)
                            {
                                Font dpiFont = new Font(f.FontFamily.Name, 8.25f);
                                GridStaticCellRenderer.DrawText(e.Graphics, s, dpiFont, MetroDateRect, e.Style, c5, rtl);
                            }
                            else
                            {
                                GridStaticCellRenderer.DrawText(e.Graphics, s, f, MetroDateRect, e.Style, c5, rtl);
                            }
                            
                        }
                        else
                            GridStaticCellRenderer.DrawText(e.Graphics, s, f, rect, e.Style, c5, rtl);
                        
                        e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                        e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                        e.Style.WrapText = false;

                        int indent = 6;
                        rect.X += 3 * indent;
                        rect.Width -= 6 * indent;
                        GridRangeInfo range = this.CoveredRanges.FindRange(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                        if (range.IsEmpty)
                        {
                            range = GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex);
                        }

                        Rectangle textRectangle = this.RangeInfoToRectangle(range);
                        int bottomLimit = 0, height = 0;
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                        {
                            height = 24;
                            bottomLimit = textRectangle.Bottom;
                        }
                        else
                        {
                            height = MoreItemsBitmap.Height;
                            bottomLimit = textRectangle.Bottom - MoreItemsBitmap.Height - 9;
                        }
                        rect.Offset(0, slot * rect.Height);
                        if (panel < this.startIndexes.Count)
                        {
                            int start = (int)startIndexes[panel] - 1;
                            for (int i = 1; i < n; ++i)
                            {
                                if (rect.Bottom < (bottomLimit - height - 3) && start + i < this.scheduleDataList.Count)
                                {
                                    IScheduleAppointment item = this.scheduleDataList[start + i] as IScheduleAppointment;
                                    if (RecurrenceSupport.IsSpanItem(item))
                                    {
                                        continue;
                                    }

                                    rect.Y = rect.Bottom;

                                    Color c = Color.FromArgb(100, ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember);
                                    Color markUpColor = Color.Red;
                                    markUpColor = ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember;
                                    Rectangle rect2 = rect;
                                    rect2.Width += 4 * indent;
                                    rect2.X -= 2 * indent;
                                    rect2.Y += 1;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        rect2.Y += 4;
                                        rect2.Height += 10;
                                        rect.Y += 17;
                                    }
                                    else
                                        rect2.Height -= 2;
                                    
                                    Rectangle rectMarkup = e.ClipBounds;
                                    rectMarkup.Y = rect2.Y;
                                    rectMarkup.Height = rect2.Height;

                                    rectMarkup.Width = rect2.X - rectMarkup.X;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        using (Brush b = new SolidBrush(markUpColor))
                                        {
                                            e.Graphics.FillRectangle(b, rectMarkup); // fill rectangle
                                        }
                                        Color borderC = Color.FromArgb(100, c.R, c.G, c.B);
                                        Rectangle borderRect = new Rectangle(rectMarkup.X, rectMarkup.Y, rectMarkup.Width - 1, rectMarkup.Height - 1);
                                        using (Pen p = new Pen(borderC))
                                        {
                                            e.Graphics.DrawRectangle(p, borderRect); // border rectangle
                                        }
                                    }
                                    using (Brush b = new SolidBrush(c))
                                    {
                                        e.Graphics.FillRectangle(b, rect2); // fill rectangle
                                    }
                                    Rectangle appointBorderRect = rect2;
                                    appointBorderRect.X = rectMarkup.X;
                                    appointBorderRect.Y = rectMarkup.Y;
                                    appointBorderRect.Width = rect2.Width + rectMarkup.Width;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        if (item.LabelValue == 0 && item.MarkerValue == 0)
                                        {
                                            using (Pen p = new Pen(this.Schedule.Appearance.SolidBorderColor))
                                            {
                                                e.Graphics.DrawRectangle(p, appointBorderRect);
                                            }
                                        }
                                        else if (item.LabelValue == 0)
                                        {
                                            using (Pen p = new Pen(markUpColor))
                                            {
                                                e.Graphics.DrawRectangle(p, appointBorderRect);
                                            }
                                        }
                                    }
                                    if (item.Equals(this.mouseDownItem))
                                    {
                                        mouseDownRectangle = rect2;
                                        mouseDownRectangle.X = rectMarkup.X;
                                        mouseDownRectangle.Y = rectMarkup.Y;
                                        mouseDownRectangle.Width = rect2.Width + rectMarkup.Width;
                                        using (Pen p = new Pen(this.Schedule.Appearance.ClickItemBorderColor))
                                        {
                                            if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                                e.Graphics.DrawRectangle(p, mouseDownRectangle);
                                            else
                                                e.Graphics.DrawRectangle(p, rect2);
                                        }
                                    }
                                    Color cb1 = Color.FromArgb(100, ((ListObject)this.dataProvider.GetLabels()[item.LabelValue]).ColorMember);
                                    if (this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
                                    {
                                        cb1 = Color.FromArgb(255, ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember);
                                        using (Pen pen = new Pen(new SolidBrush(cb1)))
                                        {
                                            pen.Width = 2;
                                            e.Graphics.DrawRectangle(pen, rect2); // border
                                        }
                                    }
                                    rtl = schedule.RightToLeft == RightToLeft.Yes;
                                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                    {
                                        Rectangle textRect = rect;
                                        textRect.Y -= 6;
                                        f = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
                                        GridStaticCellRenderer.DrawText(e.Graphics, list[i], f, textRect, e.Style, e.Style.TextColor, rtl);
                                    }
                                    else
                                    {
                                        GridStaticCellRenderer.DrawText(e.Graphics, list[i], f, rect, e.Style, e.Style.TextColor, rtl);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (rect.Bottom >= bottomLimit)
                            {
                                rect = this.RangeInfoToRectangle(GridRangeInfo.Cell(e.Style.CellIdentity.RowIndex, e.Style.CellIdentity.ColIndex));
                                if (this.GridVisualStyles != GridVisualStyles.Metro)
                                {
                                    rect.Y += e.TextRectangle.Height - ScheduleGrid.MoreItemsBitmap.Height - 1;
                                    rect.X += e.TextRectangle.Width - ScheduleGrid.MoreItemsBitmap.Width - 1;
                                    rect.Height = ScheduleGrid.MoreItemsBitmap.Height;
                                    rect.Width = ScheduleGrid.MoreItemsBitmap.Width;
                                    iconPainter.PaintIcon(e.Graphics, rect, Point.Empty, ScheduleGrid.MoreItemsBitmap, Color.Black);
                                }
                                moreItemsCells.Add(1000 * e.RowIndex + e.ColIndex);
                            }
						}

						e.Cancel = true;
					}
				}
				else if (e.Style.CellIdentity.ColIndex == timeCol && e.Style.CellIdentity.RowIndex > allDayRow
					&& (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek))
				{
					string time = GetTimeDisplayText(e.Style.CellIdentity.RowIndex);
					string small = string.Empty;
					int i = time.IndexOf('|');
					if (i > 0)
					{
						small = time.Substring(i + 1);
						time = time.Substring(0, i);
                        if (!this.Schedule.Appearance.Hours24 &&
                            !(this.TopRowIndex == e.Style.CellIdentity.RowIndex
                            || (this.TopRowIndex + 1 == e.Style.CellIdentity.RowIndex && this.TopRowIndex % 2 == 1))
                            && time != "12")
                        {
                            small = "00";
                        }
					}
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        this.TableStyle.Font.Facename = "Segoe UI";
                        this.TableStyle.Font.Size = 9f;
                    }
					using (Font f = new Font(this.TableStyle.Font.Facename, this.Schedule.Appearance.TimeBigFontSize))
					{
						using (Font f1 = new Font(this.TableStyle.Font.Facename, this.Schedule.Appearance.TimeLittleFontSize))
						{
							if (this.resetTimeRects)
							{
								this.resetTimeRects = false;
								SizeF sz = e.Graphics.MeasureString("23", f);
								this.timeLargeSize = new Size((int)sz.Width, (int)sz.Height);
								sz = e.Graphics.MeasureString("am", f1);
								this.timeSmallSize = new Size((int)sz.Width, (int)sz.Height);
							}

							int width = this.timeLargeSize.Width + this.timeSmallSize.Width;
							Rectangle bigRect = GridUtil.CenterInRect(e.TextRectangle, new Size(width, this.timeLargeSize.Height));

							bigRect = new Rectangle(e.TextRectangle.X + e.TextRectangle.Width - width, bigRect.Y, this.timeLargeSize.Width+1, bigRect.Height);
							
							bool rtl = this.RightToLeft == RightToLeft.Yes || (this.RightToLeft == RightToLeft.Inherit &&
								this.FindParentForm() != null && this.FindParentForm().RightToLeft == RightToLeft.Yes);
                            if (this.PrintingMode && time.Length > 1)
                            {
                                if (rtl)
                                {
                                    bigRect.Offset(-5, 0);
                                    bigRect.Width *= 2;
                                }
                                else
                                {
                                    bigRect.Offset(-bigRect.Width, 0);
                                    bigRect.Width *= 2;
                                }
                            }

                           GridStaticCellRenderer.DrawText(e.Graphics, time, f, bigRect, e.Style, this.Schedule.Appearance.TimeTextColor, rtl); 
							
							bigRect.X = e.TextRectangle.X + e.TextRectangle.Width - this.timeSmallSize.Width - 4;
							bigRect.Width = this.timeSmallSize.Width + 1;
							GridStaticCellRenderer.DrawText(e.Graphics, small, f1, bigRect, e.Style, this.Schedule.Appearance.TimeTextColor, rtl); 
						}
					}

					e.Cancel = true;
				}
				else
				{
                    int col = e.Style.CellIdentity.ColIndex;
                    int row = e.Style.CellIdentity.RowIndex;
                    if (!MarkCol(col) && col > this.markCol && row > allDayRow && (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek))
                    {
                        int key = this.GetLookupKey(row, col);
                        if (ScheduleAppointmentFromRange.ContainsKey(key))
                        {
                            e.DisplayText = this.ParseDisplayItem(ScheduleAppointmentFromRange[key] as IScheduleAppointment, DayItemFormat);
                        }
                    }
                }
            }
        }

        ////handled to redraw time col when schedule scrolls
		private void Grid_TopRowChanged(object sender, GridRowColIndexChangedEventArgs e)
		{
            if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
            {
                this.InvalidateRange(GridRangeInfo.Col(timeCol));
            }
		}

        ////provides the covered ranges
		private void Grid_QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
		{
			if (this != null && !this.IsDisposed && !this.IsDisposing)
			{
				if (this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
				{
					if (e.ColIndex > markCol && !MarkCol(e.ColIndex) && e.RowIndex > allDayRow)
					{
						int i;
						if (this.useTable == null || (((i = GetUseTableFromCol(e.ColIndex)) < this.useTable.GetLength(0)) && this.useTable[i, e.RowIndex, 0] == 0))
						{
							e.Range = GridRangeInfo.Cells(e.RowIndex, GetFirstColInPanelFromCol(e.ColIndex), e.RowIndex, GetLastColInPanelFromCol(e.ColIndex));
                            e.Handled = true;
						}
					}
				}
			}
		}

        ////occues after the cell is drawn to do final decoration of the cell
		private void Grid_CellDrawn(object sender, GridDrawCellEventArgs e)
		{
            int key = GetLookupKey(e.RowIndex, e.ColIndex);
            if (ScheduleAppointmentFromRange.ContainsKey(key))
            {
                IScheduleAppointment item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                if (e.ColIndex > this.GetFirstColInPanelFromCol(e.ColIndex))
                {
                    ////if(ScheduleAppointmentFromRange.ContainsKey(key))
                    {
                       //// IScheduleAppointment item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                        if (item != null)
                        {
                            Color c = ((ListObject)this.dataProvider.GetMarkers()[item.MarkerValue]).ColorMember;
                            using (Brush b = new SolidBrush(c))
                            {
                                e.Graphics.FillRectangle(b, new Rectangle(e.Bounds.Location.X, e.Bounds.Location.Y, this.markWidth, e.Bounds.Height));
                            }
                        }
                    }
                }

                if ((!schedule.ShowRoundedCorners || this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) && item.Equals(mouseDownItem))
                {
                    using (Pen p = new Pen(schedule.Appearance.ClickItemBorderColor))
                    {
                        Rectangle rect = e.Bounds;
                        rect.Y = rect.Y + 2; 
                        rect.Width = rect.Width - 1;
                        rect.Height = rect.Height - 5;
                        e.Graphics.DrawRectangle(p, rect);
                    }
                }
            }

			if ((e.ColIndex > this.markCol &&
				!ScheduleAppointmentFromRange.ContainsKey(key) &&
				this.Selections.Ranges.ActiveRange.Height > 1 &&
				this.Selections.Ranges.AnyRangeContains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
				||
				(this.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex, true) &&
				!this.CurrentCell.IsEditing && 
				this.Selections.Ranges.ActiveRange.Height <= 1))
			{
				using (Brush b = new SolidBrush(this.alphaBlendSelectionColor))
				{
					e.Graphics.FillRectangle(b, e.Bounds); 
				}
			}
		}

        ////make sure no cell is editable
        private void Grid_CurrentCellStartEditing(object sender, CancelEventArgs e)
        {
            GridCurrentCell cc = this.CurrentCell;
            if (GridUtil.IsEmpty(this[cc.RowIndex, cc.ColIndex].Text))
            {
                e.Cancel = true;
            }
        }
        
        ////handle saving changes
        private void Grid_SaveCellInfo(object sender, GridSaveCellInfoEventArgs e)
        {
            if (e.ColIndex > this.markCol && e.RowIndex > this.allDayRow)
            {
                int key = GetLookupKey(e.RowIndex, e.ColIndex);
                if (ScheduleAppointmentFromRange.ContainsKey(key))
                {
                    IScheduleAppointment item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                    item.Dirty = true;
                    item.Subject = e.Style.Text;
                    e.Handled = true;
                }
            }
        }

        private GridRangeInfo topHitRange = GridRangeInfo.EmptyRange();
        private GridRangeInfo bottomHitRange = GridRangeInfo.EmptyRange();

        /// <summary>
        /// A method to control the hit testing for sizing schedule items in Day and WorkWeek view.
        /// </summary>
        /// <param name="sender">Schedule Grid.</param>
        /// <param name="e">A <see cref="GridResizingRowsEventArgs"/> with event data.</param>
        private void Grid_ResizingRows(object sender, GridResizingRowsEventArgs e)
        {
            if (this == null || this.IsDisposing || this.IsDisposed)
            {
                return;
            }

            if (!schedule.AllowAdjustAppointmentsWithMouse)
            {
                e.Cancel = true;
                return;
            }

            if (e.Reason == GridResizeCellsReason.HitTest)
            {
                ////allow resizing hit only if the mouse is over the top 
                ////or bottom of a covered cell which represents a ScheduleAppointment
                int row = -1, col = -1;
                if (!this.PointToRowCol(this.PointToClient(Control.MousePosition), out row, out col))
                {
                    e.Cancel = true;
                    return;
                }

                topHitRange = GridRangeInfo.EmptyRange();
                bottomHitRange = GridRangeInfo.EmptyRange();

                ////is over a coveredcell?
                bool topcheck = this.coveredCells.Find(e.Rows.Top + 1, col, out topHitRange);

                if (topcheck)
                {
                    int key = GetLookupKey(topHitRange.Top, topHitRange.Left);
                    ////make sure it is a schedule item covered cell
                    topcheck = this.ScheduleAppointmentFromRange.ContainsKey(key);
                    ////adjust for hitting from above range
                    if (row == topHitRange.Top - 1)
                    {
                        row = topHitRange.Top;
                    }
                }

                ////only test bottom hit if you have no tophit
                bool bottomcheck = false;
                if (!topcheck)
                {
                    ////is over a coveredcell?
                    bottomcheck = this.coveredCells.Find(e.Rows.Top, col, out bottomHitRange);

                    if (bottomcheck)
                    {
                        int key = GetLookupKey(bottomHitRange.Top, bottomHitRange.Left);
                        ////make sure it is a schedule item covered cell
                        bottomcheck = this.ScheduleAppointmentFromRange.ContainsKey(key);
                        ////adjust for hitting from below range
                        if (row == bottomHitRange.Bottom + 1)
                        {
                            row = bottomHitRange.Bottom;
                        }
                    }
                }

                ////cancel if no hit
                if ((!topcheck || !topHitRange.Contains(GridRangeInfo.Cell(row, col)))
                    &&
                    (!bottomcheck || !bottomHitRange.Contains(GridRangeInfo.Cell(row, col))))
                {
                    e.Cancel = true;
                    topHitRange = GridRangeInfo.EmptyRange();
                    bottomHitRange = GridRangeInfo.EmptyRange();
                }
                else if ((topHitRange.IsEmpty || topHitRange.Top != e.Rows.Top + 1)
                    && (bottomHitRange.IsEmpty || bottomHitRange.Bottom != e.Rows.Bottom))
                { 
                    ////cancel if interior hit
                    e.Cancel = true;
                    topHitRange = GridRangeInfo.EmptyRange();
                    bottomHitRange = GridRangeInfo.EmptyRange();
                }

                ////clear top range if not used
                if (!topcheck && bottomcheck)
                {
                    topHitRange = GridRangeInfo.EmptyRange();
                }
            }
            else if (e.Reason == GridResizeCellsReason.MouseUp)
            {
                //// now handle the change after dragging border of schedule item using
                //// the information from topHitRange and bottomHitRange set in the 
                //// above GridResizeCellsReason.HitTest case
                e.Cancel = true;
                IScheduleAppointment item = null;

                if (!this.topHitRange.IsEmpty && (this.topHitRange.Width > 1 || this.topHitRange.Height > 1))
                {
                    int key = GetLookupKey(topHitRange.Top, topHitRange.Left);
                    if (this.ScheduleAppointmentFromRange.ContainsKey(key))
                    {
                        item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                    }

                    if (item == null)
                    {
                        key = GetLookupKey(bottomHitRange.Top, bottomHitRange.Left);
                        if (this.ScheduleAppointmentFromRange.ContainsKey(key))
                        {
                            item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                        }
                    }

                    if (item != null)
                    {
                        Rectangle rect = this.RangeInfoToRectangle(e.Rows);
                        Point pt = new Point(e.Point.X, rect.Y);
                        int startRow, col;
                        this.PointToRowCol(pt, out startRow, out col);

                        ////Point 
                        pt = new Point(5, rect.Y + e.Height);
                        ////handle roundoff
                        pt.Y -= this.DefaultRowHeight / 2 - 1;
                        int endRow;
                        if (this.PointToRowCol(pt, out endRow, out col))
                        {
                            DateTime dt = item.StartTime.AddMinutes((endRow - startRow) * minutesPerDivision);
                            if (dt >= item.EndTime)
                            {
                                ScheduleAppointmentCancelEventArgs e1 = new ScheduleAppointmentCancelEventArgs(item, null, ItemAction.Delete);
                                schedule.RaiseItemChanging(e1);
                                if (!e1.Cancel)
                                {
                                    if (MessageBox.Show(DisplayStrings[_Remove_this_appointment], DisplayStrings[_Remove_Appointment], MessageBoxButtons.YesNo)
                                        == DialogResult.Yes)
                                    {
                                        this.DataSource.RemoveItem(item);
                                        SetDataToDayPanels(true);
                                        ScheduleAppointmentEventArgs e2 = new ScheduleAppointmentEventArgs(item, ItemAction.Delete);
                                        schedule.RaiseItemChanged(e2);
                                    }
                                }
                            }
                            else
                            {
                                item.StartTime = dt;
                                SetDataToDayPanels(true);
                            }
                        }
                    }
                }
                else if (!this.bottomHitRange.IsEmpty && (this.bottomHitRange.Width > 1 || this.bottomHitRange.Height > 1))
                {
                    int key = GetLookupKey(bottomHitRange.Top, bottomHitRange.Left);
                    if (this.ScheduleAppointmentFromRange.ContainsKey(key))
                    {
                        item = ScheduleAppointmentFromRange[key] as IScheduleAppointment;
                    }

                    Rectangle rect = this.RangeInfoToRectangle(e.Rows);
                    int startRow, col;
                    this.PointToRowCol(rect.Location, out startRow, out col);

                    Point pt = new Point(5, rect.Y + e.Height);
                    ////handle roundoff
                    pt.Y -= this.DefaultRowHeight / 2 - 1;
                    int endRow;
                    if (this.PointToRowCol(pt, out endRow, out col))
                    {
                        DateTime dt = item.EndTime.AddMinutes((endRow - startRow) * minutesPerDivision);
                        if (dt <= item.StartTime)
                        {
                            ScheduleAppointmentCancelEventArgs e1 = new ScheduleAppointmentCancelEventArgs(item, null, ItemAction.Delete);
                            schedule.RaiseItemChanging(e1);
                            if (!e1.Cancel)
                            {
                                if (MessageBox.Show(DisplayStrings[_Remove_this_appointment], DisplayStrings[_Remove_Appointment], MessageBoxButtons.YesNo)
                                    == DialogResult.Yes)
                                {
                                    this.DataSource.RemoveItem(item);
                                    SetDataToDayPanels(true);
                                    ScheduleAppointmentEventArgs e2 = new ScheduleAppointmentEventArgs(item, ItemAction.Delete);
                                    schedule.RaiseItemChanged(e2);
                                }
                            }
                        }
                        else 
                        {
                            ////if(dt > item.StartTime)
                            item.EndTime = dt;
                            SetDataToDayPanels(true);
                        }
                    }
                }
            }
        }

        ////trun off sizing in all grids other than Day, Custom and WorkWeek
        private void Grid_NoResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
            e.Cancel = true;
        }

        private void Grid_NoResizingRows(object sender, GridResizingRowsEventArgs e)
        {
            e.Cancel = true;
        }

        //// handle dragging items
        internal bool scrollLocked = false;
        int startDragPanel = -1;
        int dragHeight = -1;
        IScheduleAppointment dragitem = null;
        GridRangeInfo dragRange = GridRangeInfo.Empty;
        private int maxMouseDown = int.MinValue;
        private int mouseDown = int.MinValue;
        private void Grid_ResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
            if (this == null || this.IsDisposing || this.IsDisposed)
            {
                return;
            }

            if (!schedule.AllowAdjustAppointmentsWithMouse)
            {
                e.Cancel = true;
                return;
            } 

            if (e.Reason == GridResizeCellsReason.HitTest)
            {
                dragRange = GridRangeInfo.Empty;
                int row = -1, col = -1;
                if (!this.PointToRowCol(e.Point, out row, out col))
                {
                    e.Cancel = true;
                    return;
                }

                GridRangeInfo range;
                if (!this.coveredCells.Find(row, col, out range))
                {
                    if (row == this.allDayRow)
                    {
                        ItemHitType hit;
                        IScheduleAppointment i = GetAllDayItemAtPoint(e.Point, out hit);
                    }

                    e.Cancel = true;
                   return;
                }

                int key = GetLookupKey(range.Top, range.Left);
                if (!this.ScheduleAppointmentFromRange.ContainsKey(key))
                {
                    e.Cancel = true;
                    return;
                }

                dragRange = range;
            }
            else if (e.Reason == GridResizeCellsReason.MouseDown)
            {
                if (this.mouseDownItem != null)
                {
                    dragitem = mouseDownItem;
                    startDragPanel = this.mouseDownPanel;
                    int h = this.DefaultRowHeight * dragRange.Height;

                    e.Width = 100 * h + dragRange.Top;
                    dragHeight = h;
                    if (dragitem == null)
                    {
                        throw new NullReferenceException("No item under mouse.");
                    }

                    e.SizeIndicatorBorder = new GridBorder(GridBorderStyle.Dashed, this.Properties.ResizingCellsLinesColor, GridBorderWeight.ExtraExtraThick);
                    maxMouseDown = e.Point.Y;
                    mouseDown = maxMouseDown;
                }
            }
            else if (e.Reason == GridResizeCellsReason.MouseMove)
            {
                if (maxMouseDown < e.Point.Y)
                {
                    maxMouseDown = e.Point.Y;
                }

                this.scrollLocked = (mouseDownItem != null) ? mouseDownItem.AllDay && (maxMouseDown - mouseDown) < 3 * this.DefaultRowHeight : false;
                e.SizeIndicatorBorder = new GridBorder(GridBorderStyle.Dashed, this.Properties.ResizingCellsLinesColor, GridBorderWeight.ExtraExtraThick);
            }
            else if (e.Reason == GridResizeCellsReason.MouseUp)
            {
                e.Cancel = true;
                ////get panel from point
                int panel = -1;
                int row, endCol;

                if (this.PointToRowCol(this.PointToClient(Control.MousePosition), out row, out endCol))
                {
                    GridRangeInfo range = this.CoveredRanges.FindRange(row, endCol);

                    while (range.IsEmpty && endCol > 0)
                    {
                        endCol--;
                        range = this.CoveredRanges.FindRange(row, endCol);
                    }

                    if (!range.IsEmpty)
                    {
                        panel = this.GetUseTableFromCol(range.Left);
                    }

                    ////dropped on border between cells
                    if (panel == -1)
                    {
                        ////throw new Exception("Must drop on the interior of a day cell.");
                        return;
                    }

                    ////endRow may not be at row so get it from tracking variable
                    int endRow = e.Width;

                    ////drop the item
                    DateTime dt;
                    IScheduleAppointment item = dragitem;
                    if (endRow == this.allDayRow)
                    {
                        ////dropped on allday row
                        item.AllDay = true;
                        item.StartTime = item.StartTime.Date;
                        dt = item.StartTime;
                    }
                    else
                    {
                        ////dropped on a grid cell
                        if (item.AllDay)
                        {
                            ////moving from AllDay location
                            item.StartTime = item.StartTime.Date;
                            item.EndTime = item.StartTime.AddMinutes(minutesPerDivision);
                            item.AllDay = false;
                            mouseDownRow += 1;
                        }

                        dt = item.StartTime.AddMinutes((endRow - mouseDownRow) * minutesPerDivision);
                    }

                    ////check if day moved
                    if (panel != this.startDragPanel)
                    {
                        int days = this.displayDates[panel].DayOfYear - this.displayDates[startDragPanel].DayOfYear
                            + 365 * (this.displayDates[panel].Year - this.displayDates[startDragPanel].Year);
                        dt = dt.AddDays(days);
                    }

                    TimeSpan diff = item.EndTime - item.StartTime;
                    item.StartTime = dt;
                    item.EndTime = dt + diff;
                }
                else if (!ScheduleResizeCellsMouseController.navCalendarCenterPoint.IsEmpty
                    && this.Calendar.CalenderGrid.PointToRowCol(this.Calendar.CalenderGrid.PointToClient(ScheduleResizeCellsMouseController.navCalendarCenterPoint), out row, out endCol))
                {
                    ////dropped on the NavigationCalendar
                    DateTime dt = (DateTime) this.Calendar.CalenderGrid[row, endCol].CellValue;
                    IScheduleAppointment item = dragitem;
                    int days = ((TimeSpan)(dt.Date - item.StartTime.Date)).Days;
                    item.StartTime = item.StartTime.AddDays(days);
                    item.EndTime = item.EndTime.AddDays(days);
                }

                this.scheduleDataList.SortStartTime();
                this.dataProvider.IsDirty = true;
                SetDataToDayPanels(true);
            }
        }

        ////force selections to be whole panels in Day & WorkWeek
        ////when selecting empty cells
        private void Grid_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if (!e.Range.IsEmpty)
            {
                if (schedule.ScheduleType != ScheduleViewType.WorkWeek)
                {
                    int top = e.Range.Top;
                    int left = this.GetFirstColInPanelFromCol(e.Range.Left);
                    int right = this.GetLastColInPanelFromCol(e.Range.Left);
                    int bottom = e.Range.Bottom;
                    e.Range = GridRangeInfo.Cells(top, left, bottom, right);
                }
                else
                {
                    int top = e.Range.Top;
                    int left = this.GetFirstColInPanelFromCol(e.Range.Left);
                    int right = this.GetLastColInPanelFromCol(e.Range.Right);
                    int bottom = e.Range.Bottom;
                    e.Range = GridRangeInfo.Cells(top, left, bottom, right);
                }
            }
        }

		#endregion

		#region Selection Support

		private DateTime[] SetRangeInCalendar()
		{
			return SetRangeInCalendar(this.Calendar.SelectedDates[0]);
		}
		
        /// <summary>
		/// Returns the dates to be displayed in the ScheduleGrid
		/// </summary>
		/// <param name="dt">The base date that determines other dates depending 
		/// upon <see cref="ScheduleViewType"/>.</param>
		/// <returns>The dates to be displayed.</returns>
		private DateTime[] SetRangeInCalendar(DateTime dt)
		{
            if (this.ScheduleType == ScheduleViewType.Day)
            {
                this.activePanel = 0;
                return new DateTime[] { dt.Date };
            }

            DateTime start = dt.Date;
            int off = start.DayOfWeek - DayOfWeek.Monday;
            int dayCount = 5;
            switch (this.ScheduleType)
            {
                case ScheduleViewType.WorkWeek:
                    if (off < 0)
                    {
                        start = start.AddDays(-off);
                        DateTime dt1 = this.displayDate;
                        off = 0;
                    }

                    break;
                case ScheduleViewType.Week:
                    off = 0;
                    // DayOfWeek dow = start.AddDays(-off).DayOfWeek;
                    while (start.AddDays(-off).DayOfWeek != calendar.appearance.NavigationCalendarStartDayOfWeek)
                    {
                        off++;
                    }

                    dayCount = 7;
                    break;
                case ScheduleViewType.Month:
                    if (this.Schedule.Appearance.MonthShowFullWeek)
                    {
                        off = start.Day - 1;
                       //// DayOfWeek dow = start.AddDays(-off).DayOfWeek;
                        while (start.AddDays(-off).DayOfWeek != StartDayOfWeekMonthCalendar)
                        {
                            off++;
                        }

                        dayCount = GetRowCountInMonthView() * 7; //// 42;// 35; 
                    }
                    else
                    {
                        off = start.Day - 1;
                        DayOfWeek dow = start.AddDays(-off).DayOfWeek;
                        if (dow == DayOfWeek.Sunday)
                        {
                            off += 6;
                        }
                        else if (dow == DayOfWeek.Saturday)
                        {
                            off += 5;
                        }
                        else
                        {
                            off += dow - DayOfWeek.Monday;
                        }

                        dayCount = GetRowCountInMonthView() / 2 * 7; // 42;// 35; 
                     }

                    break;
                case ScheduleViewType.CustomWeek:
                    off = 0;
                    dayCount = GetCustomDayCount();
                    start = GetCustomStartDate();
                    break;
                default:
                    break;
            }

            DateTime[] dates = new DateTime[dayCount];

            ////assume active panel is the first one (incase of WorkWeek being shown on a Sat)
            this.activePanel = 0;
            if (this.ScheduleType == ScheduleViewType.CustomWeek
                && this.Calendar.SelectedDates.Count > 1)
            {
                dates = (DateTime[])this.Calendar.SelectedDates.ToArray(typeof(DateTime));
            }
            else
            {
                ////contiguous dates
                for (int i = 0; i < dayCount; ++i)
                {
                    dates[i] = start.Date.AddDays(i - off);
                    if (dates[i] == dt.Date)
                    {
                        this.activePanel = i;
                    }
                }
            }

            return dates;
		}

        private int GetRowCountInMonthView()
        {
            this.ColCount = this.Schedule.Appearance.MonthShowFullWeek ? 7 : 6;
            startOfMonthCalendar = this.Calendar.MondayBeforeDate(this.Calendar.SelectedDates[0]);

            endOfMonthCalendar = startOfMonthCalendar.AddDays(8).AddMonths(1);
            endOfMonthCalendar = endOfMonthCalendar.AddDays(-endOfMonthCalendar.Day + 1);
            int count = 0;
            if (this.Schedule.Appearance.MonthShowFullWeek)
            {
                while (LastDayOfWeekMonthCalendar != endOfMonthCalendar.DayOfWeek)
                {
                    endOfMonthCalendar = endOfMonthCalendar.AddDays(1);
                }

                int days = (int)(endOfMonthCalendar.ToOADate() - startOfMonthCalendar.ToOADate());
                count = (days % 7) == 0 ? days / 7 : days / 7 + 1;
            }
            else
            {
                ////increase it to cover thru next sunday (+1 gets sunday)
                if (endOfMonthCalendar.DayOfWeek != DayOfWeek.Sunday)
                {
                    endOfMonthCalendar = endOfMonthCalendar.AddDays(DayOfWeek.Saturday - endOfMonthCalendar.DayOfWeek + 1);
                }
                
                int days = (int)(endOfMonthCalendar.ToOADate() - startOfMonthCalendar.ToOADate());
                count = (days % 7) == 0 ? days / 7 : days / 7 + 1;
                if (!this.Schedule.Appearance.MonthShowFullWeek)
                {
                    count *= 2;
                }
            }
            
            return count;
        }

		private void AdjustStartDate(ref DateTime start, ref int panel)
		{
			if (this.ScheduleType == ScheduleViewType.WorkWeek)
			{
				panel = this.Calendar.DateValue.DayOfWeek - DayOfWeek.Monday;
				if (panel < 0)
				{
					start.AddDays(-panel);
					panel = 0;
				}
			}
			else if (this.ScheduleType == ScheduleViewType.Week)
			{
				switch (this.Calendar.DateValue.DayOfWeek)
				{
					case DayOfWeek.Saturday:
						panel = 5;
						break;
					case DayOfWeek.Sunday:
						panel = 6;
						break;
					default:
						break;
				}
			}
		}
		
		internal IScheduleAppointment GetAllDayItemAtPoint(Point pt, out ItemHitType hit)
		{
			IScheduleAppointment item = null;
			hit = ItemHitType.None;

			////clicked AllDay row
			int yDX = pt.Y - this.RangeInfoToRectangle(GridRangeInfo.Cell(this.allDayRow, 1)).Y;
			int row = 0;

			int row1, colPanel;
			if (this.PointToRowCol(pt, out row1, out colPanel))
			{
				if (this.ScheduleType == ScheduleViewType.WorkWeek ||
					this.ScheduleType == ScheduleViewType.Day
					|| this.ScheduleType == ScheduleViewType.CustomWeek)
				{
					GridRangeInfo range1;
					if (this.CoveredRanges.Find(row1 + 1, colPanel, out range1))
					{
						colPanel = range1.Left;
					}
					
					colPanel = GetUseTableFromCol(colPanel);
				}
			}	
					
			while (row <= this.allDayGrids[colPanel].RowCount && yDX - this.allDayGrids[colPanel].RowHeights[row] > 0)
			{
				yDX -= this.allDayGrids[colPanel].RowHeights[row];
				row += 1;
			}
			
			if (row > 0 && row <= this.allDayAndSpanItems[colPanel].Count)
            {
                ////existing item under point
				item = this.allDayAndSpanItems[colPanel][row-1] as IScheduleAppointment;
				hit = ItemHitType.AllDayItem;
			}
			
			return item;
		}
		
		internal IScheduleAppointment GetItemAtPoint(Point pt, out ItemHitType hit)
		{
			IScheduleAppointment item = null;
			hit = ItemHitType.None;

            if (mouseMoveLabel != null && mouseMoveLabel.Bounds.Contains(pt))
            {
                hit = ItemHitType.Item;
               //// Console.WriteLine(mouseMoveLabel.Item.Subject);
                return mouseMoveLabel.Item;
            }
            
			int row, col;
			if (this.PointToRowCol(pt, out row, out col))
			{
				if (this.ScheduleType == ScheduleViewType.WorkWeek || 
					this.ScheduleType == ScheduleViewType.Day
					|| this.ScheduleType == ScheduleViewType.CustomWeek)
				{
					GridRangeInfo range1;
					if (this.CoveredRanges.Find(row, col, out range1))
					{
						col = range1.Left;
						row = range1.Top;
					}

					int i = (int)GetLookupKey(row, col);
					item = ScheduleAppointmentFromRange[GetLookupKey(row, col)] as IScheduleAppointment;

                    if (item == null)
                    {
                        item = GetAllDayItemAtPoint(pt, out hit);
                    }

                    if (item != null && RecurrenceSupport.IsSpanItem(item) &&
                       (schedule.ShowAllSpansInAllDayPanel || item.AllDay))
                    {
                        item = null; ////should be in a Transparentlabel....
                    }
                    else if (item != null && row == allDayRow
                        && RecurrenceSupport.IsSpanItem(item) && !schedule.ShowAllSpansInAllDayPanel)
                    {
                        item = null; ////should be in lower grid...
                    }

                    if (item != null)
                    {
                        hit = ItemHitType.Item;
                    }
				}
				else
				{ 
                    ////ScheduleViewType.Week or Month
					GridRangeInfo range = GridRangeInfo.Cell(row, col);

					GridRangeInfo range1;
					if (this.CoveredRanges.Find(row, col, out range1))
					{
						range = range1;
						row = range.Top;
					}

					Rectangle cellRect = this.RangeInfoToRectangle(range);
					GridStyleInfo style = this[row, col];

					int y = pt.Y - cellRect.Y;
                    if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        y += 3;
					
					int h = (int)(style.Font.Size + 1);

					if (y < 2 * h)
					{
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            hit = (row == 0) ? ItemHitType.Header : ItemHitType.CellHeader;
                            int X = pt.X;
                            using (Graphics g = this.CreateGraphics())
                            {
                                string text = " 88 Events";
                                Font f = this.Font;
                                Font dpiFont;
                                if (g.DpiX > 96)
                                    dpiFont = new Font(f.FontFamily.Name, 8.25f);
                                X = (int)g.MeasureString(text, f).Width;
                            }
                            int panelPosi = col * cellRect.Width - X;
                            if (pt.X > panelPosi
                                && moreItemsCells.IndexOf(1000 * row + col) > -1)
                            {
                                hit = ItemHitType.MoreItemsBitmap;
                            }
                        }
                        else
                            hit = (row == 0) ? ItemHitType.Header : ItemHitType.CellHeader;
						return item; //// header hit
					}

					string[] list = style.Text.Split(new char[] { '\n' });
					int n = list.GetLength(0) - 1;
					if (n > 0)
					{
						int panel;
                        if (this.ScheduleType == ScheduleViewType.Week)
                        {
                            panel = GetWeekPanelFromRowCol(row, col); ////Math.Max(0, (col - 1) * 3 + row - 1);
                        }
                        else
                        {
                            panel = GetMonthPanelFromRowCol(row, col);
                        }
                        int bottomLimit = 0, width = 0;
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                        {
                            bottomLimit = cellRect.Height + 6;
                            width = 24;
                        }
                        else
                        {
                            bottomLimit = cellRect.Height - MoreItemsBitmap.Height;
                            width = MoreItemsBitmap.Width;
                        }
						if (y > bottomLimit)
                        {
                            if (pt.X > cellRect.Right - width
                                && moreItemsCells.IndexOf(1000 * row + col) > -1)
                            {
                                hit = ItemHitType.MoreItemsBitmap;
                            }
                            
                            return item; //// below list
                        }

						int bottom = 4 * h;
                        if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                        {
                            bottom += 18;
                        }
                        int slot = 0;

                        if (ScheduleType == ScheduleViewType.Month)
                        {
                            bool useSpecialFormat = col == 1 && row == 1;
                            bool isRTLCulture = this.schedule.Culture.TextInfo.IsRightToLeft;
                            DateTime dt = (useSpecialFormat && !isRTLCulture) ? DateTime.ParseExact(list[0], this.Schedule.Appearance.WeekMonthNewMonth, this.Schedule.Culture).Date
                                    : DateTime.Parse(list[0], this.Schedule.Culture).Date;
                            slot = SpanManager.BottomSlot(dt);
                            if (slot > 0)
                            {
                                 bottom += slot * (2 * h - 1);
                            }
                        }

						if (panel < this.startIndexes.Count)
						{
							int start = (int)startIndexes[panel] - 1;
							for (int i = 1; i < n; ++i)
							{
                                if (slot > 0 && RecurrenceSupport.IsSpanItem(this.scheduleDataList[start + i] as IScheduleAppointment))
                                {
                                    continue;
                                }

								if (y < bottom && bottom < bottomLimit)
								{
                                    bool isItemContain = (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro) ? (y > (bottom - 2 * h - 10)) : (y > (bottom - 2 * h));
                                    if (isItemContain)
                                    {
                                        item = this.scheduleDataList[start + i] as IScheduleAppointment;
                                        hit = ItemHitType.Item;
                                        break;
                                    }
								}

								bottom += 2 * h;
                                if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                                {
                                    bottom += 15;
                                }
							}
						}
					}
				}
			}

			return item;
		}

		private int GetWeekPanelFromRowCol(int row, int col)
		{
			return Math.Max(0, (col - 1) * 3 + row - 1);
		}

		private int GetMonthPanelFromRowCol(int row, int col)
		{
            if (schedule.Appearance.MonthShowFullWeek)
            {
                return (int)Math.Max(0, (row - 1) * 7 + col - 1);
            }
            else
            {
                if (row % 2 == 0 && col != 6)
                {
                    row -= 1;
                }

                return (int)Math.Max(0, ((row - 1) / 2) * 7 + col - 1 + (((row % 2) == 0) ? 1 : 0));
            }
		}

		#endregion

		#region String Formatting Support

		// the parsing strings are expected to be lower case - others are case insensitive
		
        /// <summary>
		/// Strings that are used to define the text that is displayed in the ScheduleControl.
		/// </summary>
		/// <remarks> You can control the information that is displayed within a cell within
		/// a schedule control by setting properties like: AllDayItemFormat, WeekMonthItemFormat 
		/// and DayItemFormat. DisplayItemFormatStrings holds the token codes that you can use
		/// to represent the format string. Here is a list of the supported tokens and what they
		/// represent froma an IScheduleAppointment:
		/// [end]			IScheduleAppointment.EndTime (both date and time)
		/// [label]			IScheduleAppointment.LabelValue
		/// [location]		IScheduleAppointment.LocationValue
		/// [marker]		IScheduleAppointment.MarkerValue
		/// [owner]			IScheduleAppointment.Owner
		/// [reminder]		IScheduleAppointment.Reminder
		/// [subject]		IScheduleAppointment.Subject
		/// [start]			IScheduleAppointment.StartTime (both date and time)
		/// [allday]		IScheduleAppointment.AllDay (true or false)
		/// [starttime]		IScheduleAppointment.StartTime (time only)
		/// [endtime]		IScheduleAppointment.EndTime (time only)
		/// [startdate]		IScheduleAppointment.StartTime (date only)
		/// [enddate]		IScheduleAppointment.EndTime (date only)
		/// [content]		IScheduleAppointment.Content  
        /// <para></para>
        /// To include a [ or a ] within your formatted string, use [[ and ]] to represent the single bracket.
		/// </remarks>
		/// <example>
		/// Here is some sample code.
		/// <code lang="C#">
		///			//display only subject in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[subject]";
		///			//display only subject  and time in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[subject] [starttime]";
		///			//display time Re:subject in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[starttime] Re:[subject]";
		/// </code>
		/// <code lang="VB">
		///			'display only subject in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[subject]" 
		///			'display only subject  and time in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[subject] [starttime]" 
		///			'display time Re:subject in weekly view
		///         scheduleControl1.WeekMonthItemFormat = "[starttime] Re:[subject]" 
		/// </code>
		/// </example>
		public string[] DisplayItemFormatStrings = new string[]
									{
                                     "[", "]", "end", "label", "location", 
									 "marker", "owner", "reminder", "subject", "start", 
									 "allday", "starttime", "endtime", 
									 "startdate", "enddate", "content"
                                    };
		private const int _left = 0;
		private const int _right = 1;
		
		private const int _end = 2; ////need to be lower case
		private const int _label = 3;  ////need to be lower case
		private const int _location = 4;  ////need to be lower case
		private const int _marker = 5;  ////need to be lower case
		private const int _owner = 6;  ////need to be lower case
		private const int _reminder = 7;  ////need to be lower case
		private const int _subject = 8;  ////need to be lower case
		private const int _start = 9;  ////need to be lower case
		private const int _allday = 10;  ////need to be lower case
		private const int _starttime = 11;  ////need to be lower case
		private const int _endtime = 12;  ////need to be lower case
		private const int _startdate = 13;  ////need to be lower case
		private const int _enddate = 14;  ////need to be lower case
		private const int _content = 15;  ////need to be lower case

		 ////must be the start and end of the valaues for the tokens - used in TokenToString belwo
		private const int iterationStart = 2;  //// _end 
		private const int iterationEnd = 15;  ////_content  //last item in the list above

        private char placeHolder = (char)130;

		/// <summary>
		/// Returns a string containing information on the IScheduleAppointment.
		/// </summary>
		/// <param name="item">The IScheduleAppointment.</param>
		/// <param name="format">The format string containing tokens from <see cref="DisplayItemFormatStrings"/>.</param>
		/// <returns>A formatted string.</returns>
		/// <example>
		/// Here is a code snippet that returns a string holding the start time and 
		/// subject of someItem separated by a comma.
		/// <code lang="C#">
		/// <para></para>
		///			//get a string holding the starttime and subject
		///         string s = scheduleControl1.GetScheduleHost().ParseDisplayItem(someItem, "[starttime],[subject]");
		/// </code>
		/// <code lang="VB">
		///			'get a string holding the starttime and subject
        ///         Dim s as string = ScheduleControl1.GetScheduleHost().ParseDisplayItem(someItem, "[starttime],[subject]") 
		/// </code>
		/// </example>
		public virtual string ParseDisplayItem(IScheduleAppointment item, string format)
		{
            ParseDisplayItemEventArgs e = new ParseDisplayItemEventArgs(item, format);
            Schedule.RaiseParseDisplayItem(e);
            if (e.Handled)
            {
                return e.FormattedText;
            }

			string left = DisplayItemFormatStrings[_left];
			string right = DisplayItemFormatStrings[_right];

            string doubleLeft = left + left;
            if (format.IndexOf(doubleLeft) > -1)
            {
                format = format.Replace(doubleLeft, placeHolder.ToString());
                doubleLeft = string.Empty;
            }

			string outPut = string.Empty;
			string workingFormat = format.ToLower();
			int i = 0;
			while (i < format.Length)
			{
				int j = workingFormat.IndexOf(left, i);
				if (j > -1)
				{
					outPut += format.Substring(i, j-i);
					i = j + 1;  ////i points to left
					j = workingFormat.IndexOf(right, i);
                    if (j == -1)
                    {
                        throw new ArgumentException(DisplayStrings[_no_closing_delimiter_error]);
                    }

					outPut += TokenToString(item, workingFormat.Substring(i, j - i));
					i = j + 1;
				}
				else
				{
					outPut += format.Substring(i);
					i = format.Length;
				}
			}

            if (doubleLeft.Length == 0)
            {
                string doubleRight = right + right;
                outPut = outPut.Replace(placeHolder.ToString(), left).Replace(doubleRight, right);
            }
             
			return outPut;
		}

		private string TokenToString(IScheduleAppointment item, string token)
		{
			string s = string.Empty;

			int i = iterationStart;
			while (i <= iterationEnd && DisplayItemFormatStrings[i] != token)
			{
				++i;
			}

			if (i <= iterationEnd)
			{
				switch (i)
				{
					case _allday:
						s = item.AllDay.ToString();
						break;
					case _end:
						s = item.EndTime.ToString(this.DateTimeFormat, this.Schedule.Culture);
						break;
					case _label:
						s = this.DataSource.GetLabels()[item.LabelValue].ToString();
						break;
					case _location:
						s = item.LocationValue;
						break;
					case _marker:
						s = this.DataSource.GetMarkers()[item.MarkerValue].ToString();
						break;
					case _owner:
						s = this.DataSource.GetOwners()[item.Owner].ToString(); 
						break;
					case _reminder:
                        if (item.Reminder)
                        {
                            s = this.DataSource.GetReminders()[item.ReminderValue].ToString();
                        }
                        else
                        {
                            s = DisplayStrings[_none];
                        }

						break;
					case _subject:
						s = item.Subject;
						break;
					case _start:
						s = item.StartTime.ToString(this.DateTimeFormat, this.Schedule.Culture);
						break;
					case _starttime:
						s = item.StartTime.ToString(this.TimeFormat, this.Schedule.Culture);
						break;
					case _endtime:
						 s = item.EndTime.ToString(this.TimeFormat, this.Schedule.Culture);
						break;
					case _startdate:
						s = item.StartTime.ToString(this.DateFormat, this.Schedule.Culture);
						break;
					case _enddate:
                        if (item is IRecurringScheduleAppointment
                            && RecurrenceSupport.IsSpanItem(item)
                            && ((IRecurringScheduleAppointment)item).DateList != null)
                        {
                            s = ((IRecurringScheduleAppointment)item).DateList.TerminalDate.ToString(this.DateFormat, this.Schedule.Culture);
                        }
                        else
                        {
                            s = item.EndTime.ToString(this.DateFormat, this.Schedule.Culture);
                        }

						break;
					case _content:
						s = item.Content;
						break;
					default:
						break;
				}
			}

			return s;
		}

		internal string DayItemFormat
		{
            get
            {
                return this.Schedule.Appearance.DayItemFormat;
            }
		}
		
		internal string WeekMonthItemFormat
		{
            get
            {
                return this.Schedule.Appearance.WeekMonthItemFormat;
            }
		}

		internal string AllDayItemFormat
		{
            get
            {
                return this.Schedule.Appearance.AllDayItemFormat;
            }
		}

		/// <summary>
		/// Gets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents combined 
		/// date and time values.
		/// </summary>
		private string DateTimeFormat
		{
            get
            {
                return this.Schedule.Appearance.DateTimeFormat;
            }
		}

		/// <summary>
		/// Gets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents a 
		/// date only value.
		/// </summary>
		private string DateFormat
		{
            get
            {
                return this.Schedule.Appearance.DateFormat;
            }
		}

		/// <summary>
		/// Gets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents combined 
		/// a time only value.
		/// </summary>
		private string TimeFormat
		{
            get
            {
                return this.Schedule.Appearance.TimeFormat;
            }
		}

		/// <summary>
		/// Provides the time string to e displayed in the time column.
		/// </summary>
		/// <param name="rowIndex">Grid row index, normally 2 through 50.</param>
		/// <returns>The string to be displayed. 
		/// Characters after | will be displayed in small text.</returns>
		protected virtual string GetTimeDisplayText(int rowIndex)
		{
			int val = RowIndexTo24Time(rowIndex);
			string amPm = val < 12 ? "|" + Schedule.Culture.DateTimeFormat.AMDesignator : "|" + Schedule.Culture.DateTimeFormat.PMDesignator;
            if (this.Schedule.Appearance.Hours24 || Schedule.Culture.DateTimeFormat.AMDesignator == string.Empty)
			{
				return val.ToString() + "|00";
			}
			else
			{
				val = val % 12;
				if (val == 0)
				{
					val = 12;
				}

				return val.ToString() + amPm;
			}
		}

		private int RowIndexTo24Time(int rowIndex)
		{
			return (rowIndex - 2) / this.Schedule.Appearance.DivisionsPerHour;
		}

		private int DateTimeToRowIndex(DateTime dt)
		{
            if (this.schedule.AllowSecondsInAppointment)
                return 2 + (dt.Hour * this.Schedule.Appearance.DivisionsPerHour) + (dt.Minute / minutesPerDivision) + (dt.Second / secondPerDivision);
            else
                return 2 + (dt.Hour * this.Schedule.Appearance.DivisionsPerHour) + (dt.Minute / minutesPerDivision);
        }

		#endregion

		#region Click Support

		private DateTime ClickDateTime(Point pt)
		{
			int row, col;
			this.PointToRowCol(pt, out row, out col);
			return ClickDateTime(row, col);
		}

		private DateTime ClickDateTime(int rowIndex, int colIndex)
		{
			int panel = 0;
			switch (this.ScheduleType)
			{
				case ScheduleViewType.CustomWeek:
				case ScheduleViewType.Day: 
				case ScheduleViewType.WorkWeek:
					panel = GetUseTableFromCol(colIndex);
                    if (rowIndex > this.allDayRow)
                    {
                        rowIndex--;
                    }

					break;
				case ScheduleViewType.Week:
					panel = GetWeekPanelFromRowCol(rowIndex, colIndex);
					rowIndex = this.allDayRow;
					break;
				case ScheduleViewType.Month:
					panel = GetMonthPanelFromRowCol(rowIndex, colIndex);
					rowIndex = this.allDayRow;
					break;
			}

			DateTime dt = this.displayDates[panel].Date;
            dt = dt.AddMinutes((rowIndex - this.allDayRow) * minutesPerDivision);
			return dt;
		}

		private bool refreshDataSourceOnAdd = false;

		private void Grid_CellDoubleClick(object sender, GridCellClickEventArgs e)
		{
			////raise a cancelable event
			ScheduleAppointmentClickType t = e.MouseEventArgs.Button == MouseButtons.Left
				? ScheduleAppointmentClickType.LeftDblClick : ScheduleAppointmentClickType.RightDblClick;
           
            ScheduleAppointmentClickEventArgs args = new ScheduleAppointmentClickEventArgs(t, this.mouseDownItem, this.ScheduleType, ClickDateTime(e.RowIndex, e.ColIndex));
			this.Schedule.OnScheduleAppointmentClick(args);
            if (args.Cancel)
            {
                return;
            }

			ProcessClickForEditing(e.RowIndex, e.ColIndex);
		}

        internal DateTime GetDisplayDateUnderClick()
        {
            return GetDisplayDateUnderClick(mouseDownPoint);
        }
		
		internal DateTime GetDisplayDateUnderClick(Point point)
		{
			DateTime dt = this.displayDate;
			int rowIndex, colIndex;
            if (!point.IsEmpty && this.PointToRowCol(point, out rowIndex, out colIndex))
			{
				if (this.ScheduleType == ScheduleViewType.Day ||
					this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
				{
					IScheduleAppointment item = this.mouseDownItem;
				
					int panel = GetUseTableFromCol(colIndex);
					dt = this.displayDates[panel];
				}
				else 
				{
                    ////WEEK or MONTH
					int panel = (ScheduleType == ScheduleViewType.Week)
						? GetWeekPanelFromRowCol(rowIndex, colIndex)
					    : GetMonthPanelFromRowCol(rowIndex, colIndex);
						
					if (mouseDownHitType == ItemHitType.MoreItemsBitmap)
					{
						dt = this.displayDates[panel].Date;
					}
					else
					{
						if (mouseDownItem == null)
						{
							dt = this.displayDates[panel].Date;
						}
						else
						{
							dt = mouseDownItem.StartTime.Date;
						}
					}
				}
			}

			return dt;
		}
		
		private void ProcessClickForEditing(int rowIndex, int colIndex)
		{
			if (this.ScheduleType == ScheduleViewType.Day ||
				this.ScheduleType == ScheduleViewType.WorkWeek || this.ScheduleType == ScheduleViewType.CustomWeek)
			{
				IScheduleAppointment item = this.mouseDownItem;
				
				int panel = GetUseTableFromCol(colIndex);
				this.displayDate = this.displayDates[panel];

                bool exist = true;
				if (item == null)
				{
					exist = false;
					if (rowIndex == this.allDayRow)
					{
						item = CreateItem(rowIndex, 0);
						item.AllDay = true;
					}
					else
					{
						item = CreateItem(rowIndex - 1, 1);
					}
				}
				ShowAppointmentForm(item, exist, panel);
			}
			else 
			{
                ////WEEK or MONTH
				int panel = (ScheduleType == ScheduleViewType.Week)
					? GetWeekPanelFromRowCol(rowIndex, colIndex) ////Math.Max(0, (e.ColIndex - 1) * 3 + e.RowIndex - 1)
					: GetMonthPanelFromRowCol(this.CurrentCell.RowIndex, this.CurrentCell.ColIndex);
						
				if (mouseDownHitType == ItemHitType.MoreItemsBitmap)
				{
					this.Calendar.DateValue = this.displayDates[panel].Date;
					this.SwitchTo(ScheduleViewType.Day);
				}
				else
				{
					if (mouseDownItem == null)
					{
						IScheduleAppointment item = CreateItem(16, 1);
						item.StartTime = this.displayDates[panel].Date;
						item.EndTime = this.displayDates[panel].Date;
						item.AllDay = true;
						
						ShowAppointmentForm(item, false, panel);
					}
					else
					{
						ShowAppointmentForm(mouseDownItem, true, panel);
					}
				}
			}
		}

		#endregion

        #region AppointmentForm code
        ////creates a schedule item
        private IScheduleAppointment CreateItem(int rowIndex, int timeIntervals)
		{
			IScheduleAppointment item = this.DataSource.NewScheduleAppointment();
			OnSetDefaultItemProperties(item);
			//DateTime dt = this.displayDate.Date;
            int pan = GetUseTableFromCol(this.Selections.Ranges.ActiveRange.Left);
            DateTime dt = this.displayDates[pan].Date ;
            item.StartTime = (timeIntervals == -1) ? dt : dt.AddMinutes((rowIndex - this.allDayRow) * minutesPerDivision);
			if (timeIntervals == -1)
			{
				//item.EndTime = item.StartTime;
                int panel = GetUseTableFromCol(this.Selections.Ranges.ActiveRange.Right);
                item.EndTime = this.displayDates[panel];
				item.AllDay = true;
			}
			else
			{
               //timeIntervals++;// increment is done at DoAppoinment() method to fix SD7842_S2//incremented to fix already existing issue(last 30 minutes is not getting added properly)
                DateTime dt1 = item.StartTime.AddMinutes(timeIntervals * minutesPerDivision);
                //item.EndTime = item.StartTime.AddMinutes(timeIntervals * minutesPerDivision);
                TimeSpan min = dt1.TimeOfDay;
                int panel = GetUseTableFromCol(this.Selections.Ranges.ActiveRange.Right);
                item.EndTime = this.displayDates[panel].Date.Add(min);
			}

			return item;
		}

        /// <summary>
        /// Displays a dialog to determine the specific action desired by 
        /// the user when a recurring appointment is edited.
        /// </summary>
        /// <returns>A <see cref="RecurringAppointmentEditAction"/> value.</returns>
        public virtual RecurringAppointmentEditAction DisplayRecurringAppointmentEditConfirmation()
        {
            RecurringEditConfirmationForm f = new RecurringEditConfirmationForm();
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
            return f.Result; 
        }

        [ThreadStatic]
        private static int minimumGridRowHeight = 18;

        /// <summary>
        /// Gets or sets the minimum row height in a Day, WorkWeek or Custom view.
        /// </summary>
        public static int MinimumGridRowHeight
        {
            get
            {
                return ScheduleGrid.minimumGridRowHeight;
            }

            set
            {
                ScheduleGrid.minimumGridRowHeight = value;
            }
        }

        [ThreadStatic]
        private static bool spanOnlyPrimeTime = true;

        /// <summary>
        /// Gets or sets whether appointments that span multiple days should
        /// only appear during prime time.
        /// </summary>
        public static bool SpanOnlyPrimeTime
        {
            get
            {
                return ScheduleGrid.spanOnlyPrimeTime;
            }

            set
            {
                ScheduleGrid.spanOnlyPrimeTime = value;
            }
        }

        /// <summary>
        /// Set the expected dates when the SpanOnlyPrimeTime is changed at runtime.
        /// </summary>
        /// <param name="item">IRecurringScheduleAppointment item.</param>
        /// <param name="spanOnlyPrimeTime">Span Only Prime Time.</param>
        /// <returns>True if there was any change in the item start or end time. False otherwise.</returns>
        private bool SetExpectDates(IRecurringScheduleAppointment item, bool spanOnlyPrimeTime)
        {
            DateTime baseDate = item.DateList.BaseDate;
            DateTime terminalDate = item.DateList.TerminalDate;

            int primeStart = Schedule.Appearance.PrimeTimeStart;
            int start = item.StartTime.Hour;
            int primeEnd = Schedule.Appearance.PrimeTimeEnd;
            int end = item.EndTime.Hour;

            bool changed = false;

            if (spanOnlyPrimeTime)
            {
                if (item.StartTime != baseDate && start != primeStart)
                {
                    item.StartTime = item.StartTime.AddHours(primeStart - start);
                    changed = true;
                }

                if (item.EndTime != terminalDate && end != primeEnd && item.EndTime.AddDays(-1) >= item.StartTime.Date)
                {
                    item.EndTime = item.EndTime.AddDays(-1).AddHours(primeEnd);
                    changed = true;
                }
            }
            else
            {
                if (item.StartTime != baseDate && start != 0)
                {
                    item.StartTime = item.StartTime.AddHours(-start);
                    changed = true;
                }

                if (item.EndTime != terminalDate && end != 0)
                {
                    item.EndTime = item.EndTime.AddHours(-end).AddDays(1);
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Changes the currently loaded span appointments start/end time 
        /// with respect to SpanOnlyPrimeTime property at runtime.
        /// </summary>
        internal protected virtual void CheckSpanOnlyPrimeTime()
        {
            if (this.dataProvider is IRecurringScheduleDataProvider)
            {
                IScheduleAppointmentList list = this.scheduleDataList;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        IRecurringScheduleAppointment item = list[i] as IRecurringScheduleAppointment;
                        if (RecurrenceSupport.IsSpanItem(item) && item.DateList != null)
                        {
                            SetExpectDates(item, SpanOnlyPrimeTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the Schedule.ShowingAppointmentForm event. 
        /// </summary>
        /// <param name="e">The Event Arguments.</param>
        protected virtual void OnShowingAppointmentForm(ShowingAppointFormEventArgs e)
        {
            schedule.RaiseShowingAppointmentForm(e);
        }

        /// <summary>
        /// Displays a form that allows the user to edit an appointment.
        /// </summary>
        /// <param name="item">The appointment to be edited.</param>
        /// <param name="existingItem">True if the item being edited already existed.</param>
        /// <param name="panel">An integer that provides the 0-based index of the day if
        /// the ScheduleViewType is a Day, WorkWeek or Custom. If the view type is some other value,
        /// panel is ignored.</param>
		public virtual void ShowAppointmentForm(IScheduleAppointment item, bool existingItem, int panel)
		{
            ////set this property so appointment from can be created RTL if necessary
            ScheduleControl.isMirrored = schedule.RightToLeft == RightToLeft.Yes && ScheduleControl.UseMirroredFormsWithRTL;

            ////bring up the appointment form
            ShowingAppointFormEventArgs args = new ShowingAppointFormEventArgs();
            DialogResult result;
            IScheduleAppointment copyItem;
            ItemAction action;
            if (this.GridVisualStyles == Forms.GridVisualStyles.Metro)
            {
                MetroAppointmentForm mf = args.MetroAppointmentForm;
                mf.SetDataProvider(this.DataSource, item, minutesPerDivision, Schedule.Appearance.TimeFormat);
                mf.StartPosition = FormStartPosition.CenterParent;
                copyItem = existingItem ? item.Clone() as IScheduleAppointment : null;
                action = existingItem ? ItemAction.Edit : ItemAction.Add;

                args.Item = item;

                ////give the programmer a chance to change forms in an event....
                OnShowingAppointmentForm(args);
                if (args.Cancel && !args.Handled)
                {
                    return;
                }

                result = !args.Handled ? mf.ShowDialog() : DialogResult.OK;
            }
            else if (this.schedule.AllowSecondsInAppointment)
            {
                AppointmentForm f = args.AppointmentForm;

                f.SetDataProvider(this.DataSource, item, minutesPerDivision, Schedule.Appearance.TimeFormat, this.schedule);
                f.StartPosition = FormStartPosition.CenterParent;
                copyItem = existingItem ? item.Clone() as IScheduleAppointment : null;
                action = existingItem ? ItemAction.Edit : ItemAction.Add;

                args.Item = item;

                ////give the programmer a chance to change forms in an event....
                OnShowingAppointmentForm(args);
                if (args.Cancel && !args.Handled)
                {
                    return;
                }

                result = !args.Handled ? f.ShowDialog() : DialogResult.OK;
            }
            else
            {
                AppointmentForm f = args.AppointmentForm;

                f.SetDataProvider(this.DataSource, item, minutesPerDivision, Schedule.Appearance.TimeFormat);
                f.StartPosition = FormStartPosition.CenterParent;
                copyItem = existingItem ? item.Clone() as IScheduleAppointment : null;
                action = existingItem ? ItemAction.Edit : ItemAction.Add;

                args.Item = item;

                ////give the programmer a chance to change forms in an event....
                OnShowingAppointmentForm(args);
                if (args.Cancel && !args.Handled)
                {
                    return;
                }

                result = !args.Handled ? f.ShowDialog() : DialogResult.OK;
            }
			if (result == DialogResult.OK)
			{
				////raise event
				ScheduleAppointmentCancelEventArgs icea = new ScheduleAppointmentCancelEventArgs(copyItem, item, action);
				this.Schedule.RaiseItemChanging(icea);
                bool applyChanges = true;
                if (!icea.Cancel)
                {
                    if (item.StartTime.Date < item.EndTime.Date)
                    {
                        try
                        {
                            schedule.GetScheduleHost().BeginUpdate();
                            ////multiple dates
                            int count = ((TimeSpan)(item.EndTime.Date - item.StartTime.Date)).Days + 1;
                            int startHour = ScheduleGrid.SpanOnlyPrimeTime ? this.Schedule.Appearance.PrimeTimeStart : 0;
                            int endHour = ScheduleGrid.SpanOnlyPrimeTime ? this.Schedule.Appearance.PrimeTimeEnd : 0;
                            int nextDayMaybe = endHour != 0 ? 0 : 1;
                            if (item is IRecurringScheduleAppointment)
                            {
                                if (existingItem)
                                {
                                    DeleteRecurringAppointment((IRecurringScheduleAppointment)item);
                                }

                                if (count > 0)
                                {
                                    ClearAllDaySpans();
                                }

                                ////need a unique ID to group the appointments.
                                int id = ((IRecurringScheduleDataProvider)Schedule.DataSource).GetUniqueID();
                                for (int i = 0; i < count; ++i)
                                {
                                    IRecurringScheduleAppointment item1 = item.Clone() as IRecurringScheduleAppointment;
                                    item1.RecurrenceRule = RecurrenceSupport.SpanMarker
                                                            + RecurrenceSupport.RuleDelimiter
                                                            + item.StartTime.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat)
                                                            + RecurrenceSupport.RuleDelimiter
                                                            + item.EndTime.ToString(System.Globalization.CultureInfo.InstalledUICulture.DateTimeFormat);
                                    item1.RecurrenceRuleID = id;
                                    if (i == 0)
                                    {
                                        item1.StartTime = item.StartTime;
                                        DateTime next = item.StartTime.AddDays(nextDayMaybe);
                                        item1.EndTime = new DateTime(next.Year, next.Month, next.Day, endHour, 0, 0);
                                    }
                                    else if (i < count - 1)
                                    {
                                        DateTime next = item.StartTime.AddDays(i);
                                        item1.StartTime = new DateTime(next.Year, next.Month, next.Day, startHour, 0, 0);
                                        next = next.AddDays(nextDayMaybe);
                                        item1.EndTime = new DateTime(next.Year, next.Month, next.Day, endHour, 0, 0);
                                    }
                                    else
                                    {
                                        DateTime next = item.StartTime.AddDays(i);
                                        item1.StartTime = new DateTime(next.Year, next.Month, next.Day, startHour, 0, 0);
                                        item1.EndTime = item.EndTime.AddDays(i - count + 1);
                                    }
                                    
                                    ////if (i != 0 || action == ItemAction.Add)
                                    {
                                        item1.DateList = new RecurrenceList();
                                        item1.DateList.BaseDate = item.StartTime;
                                        item1.DateList.TerminalDate = item.EndTime;
                                        this.dataProvider.AddItem(item1);
                                        this.scheduleDataList.Add(item1);
                                    }
                                }
                                ////sort and force it to reload
                                this.scheduleDataList.SortStartTime();
                                SetDataToDayPanels(true);
                            }
                            else
                            {
                                if (existingItem)
                                {
                                    this.DataSource.RemoveItem(item);
                                    this.DataSource.IsDirty = true;
                                    this.dataProvider.RemoveItem(item);
                                    this.scheduleDataList.Remove(item);
                                }

                                DateTime meanDate = DateTime.Now;
                                DateTime meanStartDate = DateTime.Now;
                                DateTime tempEndTime = DateTime.Now;
                                bool intialized=true;
                                for (int i = 0; i <count; i++)
                                {
                                    IScheduleAppointment item1 = item.Clone() as IScheduleAppointment;
                                    if (intialized)
                                    {
                                        meanStartDate = item.StartTime;
                                        meanDate = item.EndTime.Date;
                                        tempEndTime = item.EndTime;
                                        intialized = false;
                                    }
                                    else
                                        item.StartTime = meanStartDate.AddDays(i).Date;                                 
                                    if (item.StartTime.Date < meanDate)
                                    {
                                        item1.EndTime = item.StartTime.Date;
                                    }
                                    else if (item.StartTime.Date.Equals(meanDate))
                                    {
                                        item1.EndTime = tempEndTime;
                                    }
                                    item1.StartTime = item.StartTime;                                   
                                    this.dataProvider.AddItem(item1);
                                    this.scheduleDataList.Add(item1);
                                }

                                ////sort and force it to reload
                                this.scheduleDataList.SortStartTime();
                                SetDataToDayPanels(false);
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            schedule.GetScheduleHost().EndUpdate();
                            schedule.GetScheduleHost().Refresh();
                        }
                    }
                    else
                    {
                        if (existingItem && RecurrenceSupport.IsSpanItem(item))
                        {
                            DeleteRecurringAppointment((IRecurringScheduleAppointment)item);
                            (item as IRecurringScheduleAppointment).RecurrenceRule = string.Empty;
                            (item as IRecurringScheduleAppointment).RecurrenceRuleID = -1;
                            ClearAllDaySpans();
                            existingItem = false;
                        }

                        IRecurringScheduleAppointment recurItem = item as IRecurringScheduleAppointment;
                        if (recurItem != null &&
                            recurItem.RecurrenceRule.Length > 0)
                        {
                            ////handle recurring appointment
                            IRecurringScheduleDataProvider recurDataProvider = this.DataSource as IRecurringScheduleDataProvider;
                            if (existingItem)
                            {
                                IRecurringScheduleAppointment copy = copyItem as IRecurringScheduleAppointment;
                                if (copy != null && copy.RecurrenceRule.Length == 0)
                                {
                                    ((IScheduleDataProvider)recurDataProvider).RemoveItem(item);
                                    recurDataProvider.RecurringList.Add(recurItem);
                                    recurItem.RecurrenceRuleID = recurDataProvider.GetUniqueID();
                                    recurItem.RecurringOnOverride = true;
                                    DateTime dateLimit = this.displayDates[this.displayDates.GetLength(0) - 1].Date;
                                    recurDataProvider.AddNewRecurringAppointments(recurItem, dateLimit);
                                    SetDataToDayPanels(true);
                                }
                                else
                                {
                                    RecurringAppointmentEditAction editAction = DisplayRecurringAppointmentEditConfirmation();
                                    recurDataProvider.SaveModifiedRecurringAppointment(recurItem, copy, editAction);
                                    if (editAction == RecurringAppointmentEditAction.Cancel)
                                    {
                                        applyChanges = false;
                                    }
                                    else
                                    {
                                        SetDataToDayPanels(true);
                                    }
                                }
                            }
                            else
                            {
                                recurDataProvider.RecurringList.Add(recurItem);
                                recurItem.RecurrenceRuleID = recurDataProvider.GetUniqueID();
                                recurItem.RecurringOnOverride = true;
                                DateTime dateLimit = this.displayDates[this.displayDates.GetLength(0) - 1].Date;
                                recurDataProvider.AddNewRecurringAppointments(recurItem, dateLimit);
                                SetDataToDayPanels(true);
                            }
                        }
                        else
                        {
                            if (existingItem)
                            {
                                ////existing item
                                this.scheduleDataList.SortStartTime();
                                SetDataToDayPanels(false);
                            }
                            else if (allDayAndSpanItems == null || allDayAndSpanItems[panel].IndexOf(item) == -1)
                            {
                                ////new item
                                this.DataSource.AddItem(item);
                                this.DataSource.IsDirty = true;
                                if (refreshDataSourceOnAdd)
                                {
                                    ////redo coveredcells calculations
                                    SetDataToDayPanels(true);
                                }
                                else
                                {
                                    this.scheduleDataList.Add(item);
                                    this.scheduleDataList.SortStartTime();
                                    SetDataToDayPanels(false);
                                }
                            }
                        }
                    }

                    if (applyChanges)
                    {
                        ScheduleAppointmentEventArgs iea = new ScheduleAppointmentEventArgs(item, action);
                        this.Schedule.RaiseItemChanged(iea);

                        this.Selections.Clear();
                        this.CurrentCell.MoveTo(-1, -1);
                        this.Refresh();

                        this.dataProvider.IsDirty = true;
                        this.calendar.Refresh();
                    }
                }
			}
            else if (result == DialogResult.Abort)
            {
                 ////delete the recurring appointment...
                DeleteRecurringAppointment((IRecurringScheduleAppointment)item);
            }
            else
            {
                if (existingItem)
                {
                    this.scheduleDataList.SortStartTime();
                    SetDataToDayPanels(false);
                }

                this.Selections.Clear();
                this.CurrentCell.MoveTo(-1, -1);
                this.Refresh();
            }
        }
        #endregion

     	#region ToolTip Code and ItemDrag support for Month and WeekViews
        internal AdvancedToolTip customTip = null;
        internal IScheduleAppointment mouseMoveItem = null;
        internal ToolTip ScheduleAppointmentToolTip = null;
        private bool inDrag = false;
        private Point dragPoint = Point.Empty;
        IScheduleAppointment tempDragItem = null;
        DateTime dragDate = DateTime.MinValue;
        bool overInitialCell = true;
        DragHitContext dragHit = DragHitContext.No;
        Rectangle oldCalendarRect = Rectangle.Empty;
        int calendarRow = -1;
        int calendarCol = -1;

        /// <override/> 
        protected override void OnClick(EventArgs e)
        {
            ////handles click on the more items button
            base.OnClick(e);
            if (scheduleType == ScheduleViewType.Month || scheduleType == ScheduleViewType.Week)
            {
                ItemHitType mouseDownHitType;
                Point pt = PointToClient(Control.MousePosition);
                IScheduleAppointment item = this.GetItemAtPoint(pt, out mouseDownHitType);
                if (mouseDownHitType == ItemHitType.MoreItemsBitmap)
                {
                    DateTime dt = GetDisplayDateUnderClick(pt);
                    schedule.Calendar.SelectedDates.BeginUpdate();
                    schedule.Calendar.SelectedDates.Clear();
                    schedule.Calendar.SelectedDates.Add(dt);
                    schedule.Calendar.SelectedDates.EndUpdate();
                    SwitchTo(ScheduleViewType.Day, true);
                    schedule.Calendar.DateValue = dt;
                }
            }
        }

        /// <summary>
        /// Custom class AdvancedToolTip for the schedule control.
        /// </summary>
        [ToolboxItem(false)]
        public class AdvancedToolTip : ToolTip
        {
            #region Constructor

            /// <summary>
            /// Constructor customisation AdvancedToolTip.
            /// </summary>
            public AdvancedToolTip()
            {
                this.OwnerDraw = true;
                this.Popup += new PopupEventHandler(this.OnPopup);
                this.Draw += new DrawToolTipEventHandler(this.OnDraw);
            }

            #endregion

            #region Properties and Methods

            /// <summary>
            /// A method which calculates the display of text position inside the category rectangle.
            /// </summary>
            /// <param name="locationText">Entire subject Text.</param>
            /// <param name="str1">Returns the displayable first line text.</param>
            /// <param name="str2">Returns the displayable second line text.</param>
            private void stringcalc(string locationText, out string str1, out string str2)
            {
                if (locationText.Length > 18 && locationText.Length < 28)
                {
                    str1 = locationText.Substring(0, 18);
                    str2 = locationText.Substring(19, locationText.Length);
                }
                else if (locationText.Length > 28)
                {
                    str1 = locationText.Substring(0, 18);
                    str2 = locationText.Substring(19, 26);
                }
                else
                {
                    str1 = locationText.Substring(0, locationText.Length);
                    str2 = string.Empty;
                }
            }

            private IScheduleAppointment item;
            /// <summary>
            /// Gets or sets the IScheduleApponitment. 
            /// </summary>
            [Browsable(false)]
            [Description("The IscheduleAppointment hovered item.")]
            public IScheduleAppointment Item
            {
                get
                {
                    return item;
                }
                set
                {
                    item = value;
                }
            }

            #region Others

            private Color categColor = Color.Red;
            /// <summary>
            /// Gets or Sets the category color.
            /// </summary>
            public Color CategoryColor
            {
                get
                {
                    return categColor;
                }
                set
                {
                    categColor = value;
                }
            }

            private Color showTime = Color.Red;

            /// <summary>
            /// Gets or Sets the ShowTimeColor.
            /// </summary>
            internal Color ShowTimeColor
            {
                get
                {
                    return showTime;
                }
                set
                {
                    showTime = value;
                }
            }
            
            private Point tipBounds = new Point();
            /// <summary>
            /// Used internally, Gets or Sets the ToolTipBounds.
            /// </summary>
            internal Point TipBounds
            {
                get
                {
                    return tipBounds;
                }
                set
                {
                    tipBounds = value;
                }
            }

            internal Color borderclr = ColorTranslator.FromHtml("#076CC8");
            /// <summary>
            /// Used internally, gets or sets the bordercolor.
            /// </summary>
            internal Color BorderColor
            {
                get
                {
                    return borderclr;
                }
                set
                {
                    borderclr = value;
                }
            }
            #endregion
            #endregion

            #region Event

            /// <summary>
            /// customizes the AdvancedToolTip size.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">The PopupEventArgs.</param>
            private void OnPopup(object sender, PopupEventArgs e)
            {
                e.ToolTipSize = new Size(new Point(270, 180));
            }

            /// <summary>
            /// OnDraw to handle the drawing of metrotooltip.
            /// </summary>
            /// <param name="sender">Event source.</param>
            /// <param name="e">The DrawToolTipEventArgs.</param>
            private void OnDraw(object sender, DrawToolTipEventArgs e)
            {
                int arrowWidth = 8,
                    padOne = 1, padTwo = 2,
                    bRectHeight = 160, bRectWidth = 270,
                    leftX = e.AssociatedControl.Location.X,
                    rightY = e.AssociatedControl.Location.Y;
                Color borderColor = this.BorderColor;

                //BASE RECTANGLE.
                bRectWidth -= 20;
                Rectangle baseRect = new Rectangle(leftX + padOne + arrowWidth, rightY + 5, bRectWidth, bRectHeight);
                Pen myPen = new Pen(Color.FromArgb(30, Color.Gray), 0.3f);
                e.Graphics.DrawRectangle(new Pen(borderColor, 1), baseRect);
                e.Graphics.FillRectangle(Brushes.White, baseRect.X + 1, baseRect.Y + padOne, baseRect.Width - padOne, baseRect.Height - padOne);

                //SHADOW MODE GRAPHICSPATH.
                GraphicsPath shadowPath = new GraphicsPath();
                Rectangle shadowPathRect = new Rectangle(leftX + arrowWidth, rightY + 4, bRectWidth + padTwo, bRectHeight + padTwo);
                shadowPath.AddRectangle(shadowPathRect);
                e.Graphics.DrawPath(myPen, shadowPath);

                myPen.Color = Color.FromArgb(20, Color.Gray);
                e.Graphics.DrawRectangle(myPen, shadowPathRect.Location.X - 1, shadowPathRect.Location.Y - 1, shadowPathRect.Width + 2, shadowPathRect.Height + 2);

                myPen.Color = Color.FromArgb(10, Color.Gray);
                e.Graphics.DrawRectangle(myPen, shadowPathRect.Location.X - 2, shadowPathRect.Location.Y - 2, shadowPathRect.Width + 4, shadowPathRect.Height + 4);

                myPen.Color = Color.FromArgb(5, Color.Gray);
                e.Graphics.DrawRectangle(myPen, shadowPathRect.Location.X - 3, shadowPathRect.Location.Y - 3, shadowPathRect.Width + 6, shadowPathRect.Height + 6);

                //CATEG RECTANGLE.
                Rectangle categRect = new Rectangle(leftX + 22, rightY + 13, bRectWidth - 26, bRectHeight - 125);
                Color clr = categColor;
                System.Drawing.SolidBrush myBrush;
                myBrush = new System.Drawing.SolidBrush(clr);
                e.Graphics.FillRectangle(myBrush, categRect.X + 5, categRect.Y + 6, categRect.Width - 1, categRect.Height + 12);

                //SHOWAS RECTANGLE.
                Rectangle showAsRect = new Rectangle(leftX + 20, rightY + 19, categRect.X - 15, categRect.Height + 12);
                e.Graphics.FillRectangle(new SolidBrush(ShowTimeColor), showAsRect);


                //CATEG RECTANGLE TEXT.
                string locationText = item.LocationValue;
                Size length = TextRenderer.MeasureText(locationText, new Font(e.Font, FontStyle.Regular));
                if (locationText.Length > 28)
                    locationText = locationText.Substring(0, 26) + "..";

                e.Graphics.DrawString("Start:\r\n\rEnd:".ToString(), new Font(e.Font, FontStyle.Bold), Brushes.DarkSlateGray,
                new PointF(leftX + 20, rightY + 15 + (categRect.Height + 25)));

                e.Graphics.DrawString("\rLocation:\r\nReminder:".ToString(), new Font(e.Font, FontStyle.Bold), Brushes.DarkSlateGray,
                new PointF(leftX + 20, rightY + 55 + (categRect.Height + 25)));

                e.Graphics.DrawString(item.StartTime.ToString() + "\r\n\r" + item.EndTime.ToString() + "", new Font(e.Font, FontStyle.Regular), Brushes.DimGray,
                new PointF(e.AssociatedControl.Location.X + 60, e.AssociatedControl.Location.Y + 15 + (categRect.Height + 25)));
                
                e.Graphics.DrawString("\r" + locationText.ToString(), new Font(e.Font, FontStyle.Regular), Brushes.DimGray,
                new PointF(leftX + 80, rightY + 55 + (categRect.Height + 25)));

                //ARROWHEAD.
                if (ArrowHead == ArrowHeadDirection.Left)
                {
                    Pen arrowHeadPen = new Pen(borderColor, 1f);
                    int x1 = 9, y1 = 81, x2 = 0, y2 = 90, x3 = 9, y3 = 99;

                    Point[] arrowHeadConnector = { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) };
                    e.Graphics.DrawLines(arrowHeadPen, arrowHeadConnector);
                    for (int i = 1; i < 11; i++)
                    {
                        Point[] Pt2 = { new Point(x1, y1 + i), new Point(x2 + i, y2), new Point(x3, y3 - i) };
                        e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt2);
                    }
                    Point[] Pt3 = { new Point(x1 + 2, y1), new Point(x2 + 9, y2), new Point(x3, y3 - 9) };
                    e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt3);
                }
                else if (ArrowHead == ArrowHeadDirection.Right)
                {
                    Pen arrowHeadPen = new Pen(borderColor, 1f);
                    int x1 = 259, y1 = 81, x2 = 268, y2 = 90, x3 = 259, y3 = 99;

                    Point[] arrowHeadConnector = { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) };
                    e.Graphics.DrawLines(arrowHeadPen, arrowHeadConnector);
                    for (int i = 1; i < 10; i++)
                    {
                        Point[] Pt2 = { new Point(x1, y1 + i), new Point(x2 - i, y2), new Point(x3, y3 - i) };
                        e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt2);
                    }
                    Point[] Pt3 = { new Point(x1, y1 + 2), new Point(x2 - 9, y2), new Point(x3, y3 - 9) };
                    e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt3);                    
                }
                else if (ArrowHead == ArrowHeadDirection.Bottom)
                {
                    Pen p = new Pen(borderColor, 1f);
                    int x1 = baseRect.Width / 2 + 4, y1 = baseRect.Height + 5, x2 = baseRect.Width / 2 + 14, y2 = baseRect.Height + 15, x3 = baseRect.Width / 2 + 24, y3 = baseRect.Height + 5;
                    Point[] Pt1 = { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) };
                    e.Graphics.DrawLines(p, Pt1);
                    for (int i = 1; i < 10; i++)
                    {
                        Point[] Pt2 = { new Point(x1 + i, y1), new Point(x2, y2 - i), new Point(x3 - i, y3) };
                        e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt2);
                    }
                    Point[] Pt3 = { new Point(x1 + 1, y1), new Point(x3 - 1, y3) };
                    e.Graphics.DrawLines(new Pen(Color.White, 1f), Pt3);
                }
                string firstString, secondString;
                Font font2 = new Font("Segoe UI", 10f, FontStyle.Bold);
                stringcalc(item.Subject, out firstString, out secondString);
                TextFormatFlags flags = TextFormatFlags.WordEllipsis | TextFormatFlags.WordBreak;
                if (secondString != string.Empty)
                    TextRenderer.DrawText(e.Graphics, firstString + "\n" + secondString, font2, new Rectangle(categRect.X + 8, categRect.Y + 10, categRect.Width - 10, categRect.Height - 1), Color.Black, flags);
                else
                    TextRenderer.DrawText(e.Graphics, firstString, font2, new Rectangle(categRect.X + 8, categRect.Y + 20, categRect.Width - 10, categRect.Height - 1), Color.Black, flags);

                myPen.Dispose();
                e.Graphics.Dispose();
            }

            private ArrowHeadDirection ArrowDir = ArrowHeadDirection.Left;
            /// <summary>
            /// Determines Arrow Head Direction while showing the ToolTip.
            /// </summary>
            [Browsable(false)]
            public ArrowHeadDirection ArrowHead
            {
                get
                {
                    return ArrowDir;
                }
                set
                {
                    ArrowDir = value;
                }
            }

            /// <summary>
            /// Flags for ArrowHeadDirection of the ToolTip. 
            /// </summary>
            [Browsable(false)]
            public enum ArrowHeadDirection
            {
                Left,
                Right,
                Top,
                Bottom
            }

            #endregion

        }

        /// <summary>
        /// A class which encompasses of the functionality of ShowingAdvancedToolTip event.
        /// </summary>
        public class ShowingAdvancedTooltipEventArgs : CancelEventArgs
        {
            private string subjectText; 
            private Color categoryColor = Color.Red;
            private Color showTimeColor = Color.Red;
            private Color border = ColorTranslator.FromHtml("#076CC8");

            #region Constructor

            /// <summary>
            /// Intilaizes the default setting of the ToolTip.
            /// </summary>
            /// <param name="subject">Display Text of subject Rectangle.</param>
            /// <param name="categoryColor">Backcolor of Subject Rectangle.</param>
            /// <param name="ShowTimeColor">Backcolor of Category Rectangle.</param>
            /// <param name="border">>BorderColor of ToolTip Rectangle.</param>
            public ShowingAdvancedTooltipEventArgs(string subject, Color categoryColor, Color ShowTimeColor, Color border)
            {
                this.subjectText = subject;
                this.categoryColor = categoryColor;
                this.showTimeColor = ShowTimeColor;
                this.border = border;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or Sets the DisplayText of the Subject of ToolTip.
            /// </summary>
            [Browsable(false)]
            [Description("The subject of the item.")]
            public string Subject
            {
                get
                {
                    return subjectText;
                }
                set
                {
                    if (subjectText != value)
                        subjectText = value;
                }
            }

            /// <summary>
            /// Gets or Sets the Subject Rectangle backcolor.
            /// </summary>
            [Browsable(false)]
            [Description("The categorycolor of the item")]
            public Color CategoryBackColor
            {
                get
                {
                    return categoryColor;
                }
                set
                {
                    categoryColor = value;
                }
            }

            /// <summary>
            /// Gets or Sets the ShowTime Rectangle backcolor.
            /// </summary>
            [Browsable(false)]
            [Description("The showtime color of the item")]
            public Color ShowTimeColor
            {
                get
                {
                    return showTimeColor;
                }
                set
                {
                    showTimeColor = value;
                }
            }

            /// <summary>
            /// Gets or Sets the BorderColor of ToolTip bordercolor.
            /// </summary>
            [Browsable(false)]
            [Description("The bordercolor of the tooltip")]
            public Color BorderColor
            {
                get
                {
                    return border;
                }
                set
                {
                    border = value;
                }
            }

            #endregion

        }

        //Event Handler for ShowingAdvancedToolTip event.
        public delegate void ShowingAdvancedToolTipEventHandler(object sender, ShowingAdvancedTooltipEventArgs e);

        //Event declaration for AdvancedToolTip.
        public event ShowingAdvancedToolTipEventHandler ShowingAdvancedToolTip;

        /// <summary>
        /// Raises the <see cref="ShowingAdvancedToolTip" /> event.
        /// </summary>
        /// <param name="e">A <see cref="ShowingAdvancedTooltipEventArgs" /> that contains the event data.</param>
        public virtual void OnShowingAdvancedToolTip(ShowingAdvancedTooltipEventArgs e)
        {
            if (ShowingAdvancedToolTip != null)
            {
                ShowingAdvancedToolTip(this, e);
            }
        }

        /// <summary>
        /// This Method is responsible for raising the ShowingAdvancedToolTipEvent.
        /// </summary>
        /// <param name="item">Scheduled appoitment item which was hovered.</param>
        /// <param name="tip">Object of the AdvacedToolTip</param>
        /// <returns>Returns the subject display of the item.</returns>
        private string AdvancedToolTipFiring(IScheduleAppointment item, AdvancedToolTip tip)
        {
            ShowingAdvancedTooltipEventArgs BeforeTip = new ShowingAdvancedTooltipEventArgs(item.Subject, customTip.CategoryColor, customTip.ShowTimeColor, customTip.BorderColor);
            BeforeTip.CategoryBackColor = customTip.CategoryColor;
            BeforeTip.ShowTimeColor = customTip.ShowTimeColor;
            BeforeTip.BorderColor = customTip.BorderColor;
            this.OnShowingAdvancedToolTip(BeforeTip);
            return item.Subject;
        }

        private bool overMoreItemsBitmapTip = false;
         
		/// <summary>
		/// Overridden to manage tooltip that displays IScheduleAppointment information
		/// as the user hoovers the mouse over an item.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{          
			if (this.schedule.Appearance.ScheduleAppointmentTipsEnabled)
			{
                if (ScheduleAppointmentToolTip == null)
                {
                    customTip = new AdvancedToolTip();
                    customTip.InitialDelay = 100;
                    ScheduleAppointmentToolTip = new ToolTip();
                    ScheduleAppointmentToolTip.InitialDelay = 500;
            	}

                ItemHitType mouseDownHitType;
                IScheduleAppointment item = this.GetItemAtPoint(new Point(e.X, e.Y), out mouseDownHitType);
                if (item != null || mouseDownHitType == ItemHitType.MoreItemsBitmap)
                {
                    if (mouseDownHitType == ItemHitType.MoreItemsBitmap)
                    {
                        if (!overMoreItemsBitmapTip)
                        {
                            overMoreItemsBitmapTip = true;
                            if (ScheduleAppointmentToolTip != null)
                                ScheduleAppointmentToolTip.Active = false; ////turn it off
                            if (customTip != null)
                                customTip.Active = false;
                            if (this.GridVisualStyles == GridVisualStyles.Metro)
                                ScheduleAppointmentToolTip.SetToolTip(this, DisplayStrings[_Click_for_more_Appoinments]);
                            else
                                ScheduleAppointmentToolTip.SetToolTip(this, DisplayStrings[_Click_to_switch_to_Day_view]);
                            ScheduleAppointmentToolTip.Active = true; ////turn it on
                        }
                    }
                    else if (!item.Equals(mouseMoveItem))
                    {
                        overMoreItemsBitmapTip = false;
                        mouseMoveItem = item;
                        if (!this.Schedule.Appearance.EnableAdvancedToolTip)
                        {
                            ScheduleAppointmentToolTip.Active = false; ////turn it off
                            ScheduleAppointmentToolTip.SetToolTip(this, ParseDisplayItem(item, this.schedule.Appearance.ScheduleAppointmentTipFormat));
                            ScheduleAppointmentToolTip.Active = true; ////turn it on
                        }
                        else
                        {
                            customTip.Active = false; ////turn it off  
                            this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Left;
                            this.customTip.CategoryColor = Color.FromArgb(100, ((ListObject)this.DataSource.GetLabels()[item.LabelValue]).ColorMember);// this.schedule.Appearance.ClickItemBorderColor;                            
                            this.customTip.ShowTimeColor = Color.FromArgb(100, ((ListObject)this.DataSource.GetMarkers()[item.MarkerValue]).ColorMember);// this.schedule.Appearance.ClickItemBorderColor;
                            this.customTip.Item = item;
                            item.Subject = AdvancedToolTipFiring(item, customTip);

                            //Showing ToolTip.
                            int noOfRow = 0, noOfCol = 0, rowHt = 0, colWt = 0, displayColumns = 6, cl = 0;
                            Size ToolTipSize = new System.Drawing.Size(270, 180);

                            //Postioning ToolTip for "Month view".
                            Point pt = this.PointToScreen(new Point(e.Location.X, e.Location.Y));
                            this.customTip.TipBounds = pt;
                            int colheadwidth = this.ColWidths[1];
                            if (this.Schedule.Appearance.MonthShowFullWeek)
                                displayColumns = 7;
                            if (this.Schedule.ScheduleType == ScheduleViewType.Week)
                                displayColumns = 2;
                            if (this.schedule.ScheduleType == ScheduleViewType.WorkWeek)
                            {
                                colWt = e.Location.X + 5;
                            }
                            else if (this.Schedule.ScheduleType == ScheduleViewType.Day)
                            {
                                colWt = e.Location.X + 5;
                            }
                            else
                            {
                                for (int i = 1; i <= displayColumns; i++)
                                {
                                    if (e.Location.X >= (colWt += ColWidths[i]))
                                    {
                                        noOfCol = i;
                                    }
                                    else
                                    {
                                        noOfCol = i;
                                        break;
                                    }
                                }
                                colWt = this.ColWidths[1] * noOfCol;
                                rowHt = this.RowHeights[1] * noOfRow;
                            }

                            Point postion = this.PointToScreen(new Point(colWt, e.Location.Y));
                            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                            Rectangle BtRtRect = new Rectangle(postion.X, postion.Y, screenBounds.Width - postion.X, screenBounds.Height - postion.Y);
                            Rectangle TpRtRect = new Rectangle(postion.X, 0, screenBounds.Width - postion.X, postion.Y);
                            Rectangle BtLtRect = new Rectangle(0, postion.Y, postion.X, screenBounds.Height - postion.Y);
                            Rectangle TpLtRect = new Rectangle(0, 0, postion.X, postion.Y);

                            int posX = e.X / this.ColWidths[1];
                            int newposX = (posX + 1) * ColWidths[1];
                            int posY = e.Y / this.RowHeights[1];
                            Point ptClient = this.PointToClient(postion);
                            if (this.schedule.ScheduleType == ScheduleViewType.Month || this.schedule.ScheduleType == ScheduleViewType.Week)
                            {
                                if (BtRtRect.Width > ToolTipSize.Width && BtRtRect.Height > ToolTipSize.Height / 2 + ToolTipSize.Height / 2)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Left;
                                    customTip.Show(item.Subject, this, ptClient.X, ptClient.Y - ToolTipSize.Height / 2 - 3);
                                }
                                else if (BtLtRect.Width > postion.X - ToolTipSize.Width && BtLtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Right;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width - ColWidths[1], ptClient.Y - ToolTipSize.Height / 2);
                                }
                                else if (TpLtRect.Width - Width < ToolTipSize.Width / 2 && TpLtRect.Height < ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Top;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - ToolTipSize.Width / 2, postion.Y);
                                }
                                else if (TpRtRect.Width + Width / 2 > ToolTipSize.Width / 2 && TpRtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Bottom;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - this.ColWidths[1] / 2, ptClient.Y - ToolTipSize.Height);
                                }
                                else
                                {
                                    customTip.Show(item.Subject, this, postion.X, postion.Y);
                                }
                            }
                            else if (this.schedule.ScheduleType == ScheduleViewType.WorkWeek)
                            {
                                if (TpRtRect.Width + Width / 2 > ToolTipSize.Width / 2 && TpRtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Bottom;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - this.ColWidths[1] / 2, ptClient.Y - ToolTipSize.Height);
                                }
                                else if (BtRtRect.Width > ToolTipSize.Width && BtRtRect.Height > ToolTipSize.Height / 2 + ToolTipSize.Height / 2)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Left;
                                    customTip.Show(item.Subject, this, ptClient.X, ptClient.Y - ToolTipSize.Height / 2 - 3);
                                }
                                else if (BtLtRect.Width > postion.X - ToolTipSize.Width && BtLtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Right;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width - ColWidths[1], ptClient.Y - ToolTipSize.Height / 2);
                                }
                                else if (TpLtRect.Width - Width < ToolTipSize.Width / 2 && TpLtRect.Height < ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Top;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - ToolTipSize.Width / 2, postion.Y);
                                }
                                else
                                {
                                    customTip.Show(item.Subject, this, postion.X, postion.Y);
                                }
                            }
                            else if (this.schedule.ScheduleType == ScheduleViewType.Day)
                            {

                                if (TpRtRect.Width + Width / 2 > ToolTipSize.Width / 2 && TpRtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Bottom;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - this.ColWidths[1] / 2, ptClient.Y - ToolTipSize.Height);
                                }
                                else if (BtRtRect.Width > ToolTipSize.Width && BtRtRect.Height > ToolTipSize.Height / 2 + ToolTipSize.Height / 2)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Left;
                                    customTip.Show(item.Subject, this, ptClient.X, ptClient.Y - ToolTipSize.Height / 2 - 3);
                                }
                                else if (BtLtRect.Width > postion.X - ToolTipSize.Width && BtLtRect.Height > ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Right;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width - ColWidths[1], ptClient.Y - ToolTipSize.Height / 2);
                                }
                                else if (TpLtRect.Width - Width < ToolTipSize.Width / 2 && TpLtRect.Height < ToolTipSize.Height)
                                {
                                    this.customTip.ArrowHead = AdvancedToolTip.ArrowHeadDirection.Top;
                                    customTip.Show(item.Subject, this, ptClient.X - ToolTipSize.Width / 2 - ToolTipSize.Width / 2, postion.Y);
                                }
                                else
                                {
                                    customTip.Show(item.Subject, this, postion.X, postion.Y);
                                }
                            }
                          
                            customTip.Active = true;
                        }
                    }
                }
                else
                {
                    if (ScheduleAppointmentToolTip != null)
                        ScheduleAppointmentToolTip.Active = false;
                    if (customTip != null)
                        customTip.Active = false;
                    mouseMoveItem = null;
                    overMoreItemsBitmapTip = false;
                }
            }

            if (!schedule.AllowAdjustAppointmentsWithMouse)
            {
                return;
            }

            if (mouseDownItem != null && Control.MouseButtons == MouseButtons.Left)
            {
                AdjustingAppointmentMouseWithEventArgs args = new AdjustingAppointmentMouseWithEventArgs(mouseDownItem);
                schedule.OnAdjustingAppointmentWithMouse(args);
                if (args.Cancel)
                {
                    return;
                }
            }

			base.OnMouseMove(e);
            if (RecurrenceSupport.IsSpanItem(mouseDownItem))
            {
                inDrag = false;
                return;
            }

            if (mouseDownPanel == -1 && !inDrag && e.Button == MouseButtons.Left
                && mouseDownItem != null && (Math.Abs(mouseDownPoint.X - e.X) > SystemInformation.DragSize.Width
                    || Math.Abs(mouseDownPoint.Y - e.Y) > SystemInformation.DragSize.Height))  
            {
                dragHit = DragHitContext.Schedule;
                overInitialCell = true;
                dragDate = GetDisplayDateUnderClick(new Point(e.X, e.Y));
                inDrag = true;
                
                oldCalendarRect = Rectangle.Empty;
            }
            else if (inDrag && e.Button == MouseButtons.Left)
            {
                int row, col;
                if (this.PointToRowCol(new Point(e.X, e.Y), out row, out col)
                    && (row != dragMarkedRow || col != dragMarkedCol))
                {
                    int panel = (ScheduleType == ScheduleViewType.Week)
                        ? GetWeekPanelFromRowCol(row, col) : GetMonthPanelFromRowCol(row, col);

                    DateTime dt = this.displayDates[panel].Date;
                    //if (dt != dragDate)
                    {
                        if (overInitialCell)
                        { 
                            ////just moved off the initial cell, so need to remove the mousedownitem
                            this.scheduleDataList.Remove(mouseDownItem);
                            overInitialCell = false;
                        }

                        dragMarkedRow = row;
                        dragMarkedCol = col;
                        dragDate = dt;
                        SetTemporaryDragItem();
                       //// mouseDownItem = tempDragItem;
                    }
                    ////this.CurrentCell.MoveTo(row, col);
                }
            }
            else if (inDrag)
            {
                throw new Exception("Add code to handle");
            }
 		}

        /// <summary>
        /// An event that occurs when you click/doubleclick an item.
        /// </summary>
        [Description("Occurs when an item is clicked."),
        Category("Action")]
        public event ScheduleAppointmentClickEventHandler ScheduleAppointmentClick;

        /// <summary>
        /// Raises a ScheduleAppointmentClick event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public virtual void OnScheduleAppointmentClick(ScheduleAppointmentClickEventArgs e)
        {
            if (ScheduleAppointmentClick != null)
            {
                ScheduleAppointmentClick(this, e);
            }
        }
        /// <summary>
        /// Overriden to show drop cursor while moving an appointment in a Month or Week
        /// display.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnCellCursor(GridCellCursorEventArgs e)
        {
            base.OnCellCursor(e);
            if (inDrag)
            {
                Rectangle rect = this.ClientRectangle;
                rect.Y += this.Model.RowHeights[0];
                rect.Height -= this.Model.RowHeights[0];
                if (rect.Contains(this.PointToClient(Control.MousePosition)))
                {
                    dragHit = DragHitContext.Schedule;
                    e.Cursor = GridCursors.DragSelectionCursor;
                    if (!oldCalendarRect.IsEmpty)
                    {
                        this.calendar.CalenderGrid.Invalidate(oldCalendarRect);
                        oldCalendarRect = Rectangle.Empty;
                    }
                }
                else
                {
                    rect = this.calendar.CalenderGrid.ClientRectangle;
                    Point pt = this.calendar.CalenderGrid.PointToClient(Control.MousePosition);
                    if (rect.Contains(pt))
                    {
                        dragHit = DragHitContext.Calendar;
                        e.Cursor = GridCursors.DragSelectionCursor;
                        int row, col;

                        if (this.calendar.CalenderGrid.PointToRowCol(pt, out row, out col))
                        {
                            if (this.calendar.IsDateGridCell(row, col))
                            {
                                int cal = row / this.calendar.rowsPerCal;
                                DateTime dt = (DateTime)this.calendar.CalenderGrid[row, col].CellValue;
                                DateTime dtV = this.calendar.DateValue.AddMonths(cal);
                                if (!((cal == 0 && 100 * dt.Year + dt.Month > 100 * dtV.Year + dtV.Month)
                                    || (cal > 0 && cal < this.calendar.numberCalendars - 1 && dt.Month != dtV.Month)
                                    || (cal == this.calendar.numberCalendars - 1 && 100 * dt.Year + dt.Month < 100 * dtV.Year + dtV.Month)))
                                {
                                    calendarRow = row;
                                    calendarCol = col;
                                    Rectangle r = this.calendar.CalenderGrid.RangeInfoToRectangle(GridRangeInfo.Cell(calendarRow, calendarCol));
                                    r.Inflate(-1, -1);
                                    if (oldCalendarRect != r)
                                    {
                                        this.calendar.CalenderGrid.Invalidate(oldCalendarRect);
                                    }

                                    oldCalendarRect = r;
                                    if (this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
                                    {
                                        using (Pen p = new Pen(ScheduleResizeCellsMouseController.dragColor))
                                        {
                                            using (Graphics g = this.calendar.CalenderGrid.CreateGraphics())
                                            {
                                                g.DrawRectangle(p, r);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        e.Cursor = Cursors.No;
                        dragHit = DragHitContext.No;
                        if (!oldCalendarRect.IsEmpty)
                        {
                            this.calendar.CalenderGrid.Invalidate(oldCalendarRect);
                            oldCalendarRect = Rectangle.Empty;
                        }
                    }
                }

                e.Cancel = true;
            }
        }

        private void SetTemporaryDragItem()
        {
          ////  DateTime dt = GetDisplayDateUnderClick(dragPoint);
            if (tempDragItem == null)
            {
               IScheduleAppointment temp = this.scheduleDataList.NewScheduleAppointment(); //// mouseDownItem.Clone() as IScheduleAppointment;
               tempDragItem = mouseDownItem.Clone() as IScheduleAppointment;
               tempDragItem.UniqueID = temp.UniqueID;
               temp = null;
               this.scheduleDataList.Add(tempDragItem);
            }

            tempDragItem.EndTime = new DateTime(dragDate.Year, dragDate.Month, dragDate.Day, mouseDownItem.EndTime.Hour, mouseDownItem.EndTime.Minute, 0);
            tempDragItem.StartTime = new DateTime(dragDate.Year, dragDate.Month, dragDate.Day, mouseDownItem.StartTime.Hour, mouseDownItem.StartTime.Minute, 0);
            this.scheduleDataList.SortStartTime();
            SetDataToDayPanels(false);
        }
		#endregion
	
		#region ContextMenu support

		// display menu and track click point
		private Point menuPoint = Point.Empty;
		internal bool contextMenuCancelled = false; 
		private void Grid_MouseUp(object sender, MouseEventArgs e)
		{
			menuPoint = Point.Empty;
            ItemAction action = ItemAction.Default;
            IScheduleAppointment proposedDragItem = null;
               
			if (e.Button == MouseButtons.Right)
			{
                menuPoint = new Point(e.X, e.Y);
            }
            else if (!inDrag && e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                action = ScheduleResizeCellsMouseController.isRowSizing ? ItemAction.TimeDrag : ItemAction.ItemDrag;
                proposedDragItem = mouseDownItem;
			}
            else if (inDrag && e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                action = ItemAction.ItemDrag;
                ////handle dragging in Week or Month view
                switch (dragHit)
                {
                    case DragHitContext.Schedule:
                        this.DataSource.RemoveItem(mouseDownItem);
                        if (tempDragItem != null)
                        {
                            this.DataSource.AddItem(tempDragItem);
                            proposedDragItem = tempDragItem;
                        }
                        break;
                    case DragHitContext.Calendar:
                        DateTime dt = (DateTime)this.Calendar.CalenderGrid[calendarRow, calendarCol].CellValue;
                        int days = ((TimeSpan)(dt.Date - mouseDownItem.StartTime.Date)).Days;
                        mouseDownItem.StartTime = mouseDownItem.StartTime.AddDays(days);
                        mouseDownItem.EndTime = mouseDownItem.EndTime.AddDays(days);
                        this.scheduleDataList.Add(mouseDownItem);
                         proposedDragItem = mouseDownItem;
                        SetDataToDayPanels(true);
                        break;
                    case DragHitContext.No:
                    default:
                        this.scheduleDataList.Remove(tempDragItem);
                        this.scheduleDataList.Add(mouseDownItem);
                         proposedDragItem = mouseDownItem;
                        SetDataToDayPanels(true);
                        this.CurrentCell.MoveTo(mouseDownRow, mouseDownCol);   
                        break;
                }

                inDrag = false;
                overInitialCell = true;
                dragMarkedRow = -1;
                dragMarkedCol = -1;
                mouseDownItem = null;
                tempDragItem = null;
            }
             ////raise an event to signal the drag action
            if (e.Button == MouseButtons.Left && e.Clicks == 1 
                && action != ItemAction.Default
                && originalDragItem != null && proposedDragItem != null)
            {
                ScheduleAppointmentCancelEventArgs icea = null;
                if(dragHit == DragHitContext.Schedule)
                    icea = new ScheduleAppointmentCancelEventArgs(originalDragItem, proposedDragItem, action, ItemDragHitContext.Schedule);
                else if(dragHit == DragHitContext.Calendar)
                    icea = new ScheduleAppointmentCancelEventArgs(originalDragItem, proposedDragItem, action, ItemDragHitContext.Calendar);
                else
                    icea = new ScheduleAppointmentCancelEventArgs(originalDragItem, proposedDragItem, action);
                this.Schedule.RaiseItemChanging(icea);
                if (icea.Cancel)
                {
                    ////reverse the changes
                    this.DataSource.RemoveItem(proposedDragItem);
                    this.DataSource.AddItem(originalDragItem);
                    SetDataToDayPanels(true);
                }
                else
                {
                    this.DataSource.IsDirty = true;
                }
            }
		}
        
		#region Menu Handlers - used by the ScheduleControl to expose the functionality to various menu handlers

		internal void EditItemAtClick()
		{
			int row, col;
			if (this.PointToRowCol(mouseDownPoint, out row, out col))
			{
				ProcessClickForEditing(row, col);
                this.calendar.Refresh();
			}
		}

		internal void SwitchTo(ScheduleViewType t)
		{
			SwitchTo(t, false);
		}

        /// <summary>
        /// A method that switches the display to the requested ScheduleViewType.
        /// </summary>
        /// <param name="t">The ScheduleViewType.</param>
        /// <param name="force">Whether or not the display should be reset if the current ScheduleViewType is the same as the requested ScheduleViewType.</param>
		public void SwitchTo(ScheduleViewType t, bool force)
		{
			this.Schedule.FreezePainting = true;
			if (t != this.ScheduleType || t == ScheduleViewType.CustomWeek || force)
			{
				ScheduleViewType oldType = this.ScheduleType;
				this.ScheduleType = t;
				DateTime dt;
                
				switch (t)
				{
					case ScheduleViewType.Day:
						dt = this.Calendar.SelectedDates[0];
						this.Calendar.SelectedDates.BeginUpdate();
						this.Calendar.SelectedDates.Clear();
						this.Calendar.SelectedDates.Add(dt);
						this.Calendar.SelectedDates.EndUpdate(true);
						break;
					case ScheduleViewType.CustomWeek:
						
						break;
					case ScheduleViewType.WorkWeek:
					case ScheduleViewType.Week:
                        dt = this.Calendar.SelectedDates[0];
						this.Calendar.SelectedDates.BeginUpdate();
						this.Calendar.SelectedDates.Clear();
						this.Calendar.SelectedDates.Add(dt);
						this.AdjustSelections();
						this.Calendar.SelectedDates.EndUpdate(true);
						break;
					case ScheduleViewType.Month:
						dt = this.Calendar.FirstDayOfMonth(this.Calendar.SelectedDates[0]);
						this.Calendar.SelectedDates.BeginUpdate();
						this.Calendar.SelectedDates.Clear();
						this.Calendar.SelectedDates.Add(dt);
						this.AdjustSelections();
						this.Calendar.SelectedDates.EndUpdate();
						break;
				}

	    		this.Schedule.ResetProvider(t);
			
				if (this.ScheduleType != ScheduleViewType.CustomWeek)
				{
					this.Schedule.previousButton.Show();
					this.Schedule.nextButton.Show();
				}
				else
				{
					this.Schedule.previousButton.Hide();
					this.Schedule.nextButton.Hide();
				}
			}

			this.Schedule.FreezePainting = false;
		}

        private void DeleteRecurringAppointment(IRecurringScheduleAppointment item)
        {
            ((IRecurringScheduleDataProvider)this.DataSource).RemoveRecurringAppointments(item);
            this.DataSource.IsDirty = true;
            item = null;
            SetDataToDayPanels(true);
        }

        /// <summary>
        /// A method to delete an existing appointment.
        /// </summary>
		internal void DeleteAppointment()
		{
            if (this.mouseDownItem != null)
            {
                if ((this.DataSource is IRecurringScheduleDataProvider && mouseDownItem is IRecurringScheduleAppointment)
                    && ((IRecurringScheduleAppointment)mouseDownItem).RecurrenceRule.Length > 0 && (RecurrenceSupport.IsSpanItem(mouseDownItem) 
                    ||MessageBox.Show("Remove all occurrences of this recurring appointment?", "Recurring Appointment", MessageBoxButtons.YesNo)
                == DialogResult.Yes))
                {
                    if (!RecurrenceSupport.IsSpanItem(mouseDownItem) || MessageBox.Show("Remove this span appointment?", "Span Appointment", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DeleteRecurringAppointment((IRecurringScheduleAppointment)mouseDownItem);
                        if (RecurrenceSupport.IsSpanItem(mouseDownItem))
                        {
                            ClearAllDaySpans(); ////force all to be recreated so deleted window will go away
                        }
                    }
                }
                else if (MessageBox.Show("Remove this item?", "Remove Appointment", MessageBoxButtons.YesNo)
				== DialogResult.Yes)
			{
				this.DataSource.RemoveItem(mouseDownItem);
				this.DataSource.IsDirty = true;
				mouseDownItem = null;
				SetDataToDayPanels(true);
              }
                this.calendar.Refresh();
            }
		}

		private Point GetPointLocation()
		{
			return menuPoint.IsEmpty ? this.mouseDownPoint : menuPoint;
		}

		internal void DoAppointment(bool allDay)
		{
			if (this.ScheduleType == ScheduleViewType.WorkWeek
				|| this.ScheduleType == ScheduleViewType.Day || this.ScheduleType == ScheduleViewType.CustomWeek)
			{
                ////new appointment
				int rowIndex = this.Selections.Ranges.ActiveRange.Top;
				int panel = this.GetUseTableFromCol(this.Selections.Ranges.ActiveRange.Left + 1);
				this.displayDate = this.displayDates[panel];
                int intervals = allDay ? -1 : (this.Selections.Ranges.ActiveRange.Bottom - this.Selections.Ranges.ActiveRange.Top)+1;
                if (intervals <= 1)
                {
                    int col, row;
                    if (this.PointToRowCol(GetPointLocation(), out row, out col))
                    {
                        rowIndex = row;
                        panel = this.GetUseTableFromCol(col + 1);
                        this.displayDate = this.displayDates[panel];
                    }
                }
				IScheduleAppointment item = CreateItem(rowIndex-1, intervals);
				ShowAppointmentForm(item, false, panel);
			}
			else 
			{
                ////Week or month
				int col, row;
				if (this.PointToRowCol(GetPointLocation(), out row, out col))
				{
					int panel = this.ScheduleType == ScheduleViewType.Week 
						? GetWeekPanelFromRowCol(row, col) : GetMonthPanelFromRowCol(row, col); ////Math.Max(0, (col -1) * 3 + row - 1);

					IScheduleAppointment item = CreateItem(16, 1);
					if (allDay)
					{
						item.StartTime = (mouseDownItem == null) ? this.displayDates[panel].Date : mouseDownItem.StartTime.Date;
						item.EndTime = item.StartTime;
					}
					else
					{
						item.StartTime = this.displayDates[panel].Date.AddHours(8);
						item.EndTime = item.StartTime.AddMinutes(minutesPerDivision);
					}

					item.AllDay = allDay;
					ShowAppointmentForm(item, false, panel);
				}
			}
		}

		#endregion
		
		#endregion

		#region track clicked item
		private IScheduleAppointment mouseDownItem = null; 
        private IScheduleAppointment originalDragItem = null; 
		private int mouseDownRow = -1;
        private int mouseDownCol = -1;
        private int dragMarkedRow = -1;
        private int dragMarkedCol = -1;
		ItemHitType mouseDownHitType;
		private int mouseDownPanel = -1;
		private Point mouseDownPoint;
		private Rectangle mouseDownRectangle = Rectangle.Empty;
		private void Grid_MouseDown(object sender, MouseEventArgs e)
		{
			mouseDownPoint = new Point(e.X, e.Y);
			mouseDownItem = GetItemAtPoint(mouseDownPoint, out mouseDownHitType);
            if (mouseDownItem == null)
            {
                mouseDownPoint.Offset(0, 4);
                mouseDownItem = GetItemAtPoint(mouseDownPoint, out mouseDownHitType);
                if (mouseDownItem == null)
                {
                    mouseDownPoint.Offset(0, -8);
                    mouseDownItem = GetItemAtPoint(mouseDownPoint, out mouseDownHitType);
                }
            }

            originalDragItem = mouseDownItem != null ? mouseDownItem.Clone() as IScheduleAppointment : null;
			this.Invalidate(mouseDownRectangle);
            this.PointToRowCol(new Point(e.X, e.Y), out mouseDownRow, out mouseDownCol);
            dragMarkedRow = mouseDownRow;
            dragMarkedCol = mouseDownCol;
            this.InvalidateRange(GridRangeInfo.Cell(mouseDownRow, mouseDownCol), GridRangeOptions.MergeCoveredCells);
			if (this.ScheduleType != ScheduleViewType.Month && this.ScheduleType != ScheduleViewType.Week)
			{
                mouseDownPanel = this.GetUseTableFromCol(mouseDownCol);
			}
			else
			{
				mouseDownPanel = -1;               
			}

            menuPoint = Point.Empty;

            if (e.Button == MouseButtons.Right)
            {
                GridRangeInfo range = this.Selections.Ranges.ActiveRange;
                ////raise a cancelable event
                ScheduleAppointmentClickEventArgs args = new ScheduleAppointmentClickEventArgs(ScheduleAppointmentClickType.RightClick, this.mouseDownItem, this.ScheduleType, ClickDateTime(new Point(e.X, e.Y)));
                this.Schedule.OnScheduleAppointmentClick(args);
                contextMenuCancelled = args.Cancel;
                if (args.Cancel)
                {
                    return;
                }

                menuPoint = new Point(e.X, e.Y);
            }
            else if (!inDrag && e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ////raise a cancelable event
                ScheduleAppointmentClickEventArgs args = new ScheduleAppointmentClickEventArgs(ScheduleAppointmentClickType.LeftClick, this.mouseDownItem, this.ScheduleType, ClickDateTime(new Point(e.X, e.Y)));
                this.Schedule.OnScheduleAppointmentClick(args);
                if (args.Cancel)
                {
                    return;
                }
            }
		}

		internal bool ItemSelected
		{
            get
            {
                return mouseDownItem != null;
            }
		}
		#endregion

        #region utilities

        private int DaysInMonth(DateTime dt)
		{
			DateTime nextMonth = dt.AddMonths(1).Date;
			return nextMonth.AddDays(-nextMonth.Day).Day;
		}
		
		/// <summary>
		/// Maximum number of days displayable as a ScheduleViewType.CustomWeek display.
		/// </summary>
		/// <remarks>If you select more days than this limit, the latter selections
		/// will be ignored.
		/// </remarks>
		[ThreadStaticAttribute] public int MaxLimitOnDisplayDays = 10;

		private int GetCustomDayCount()
		{
			return this.Calendar.SelectedDates.Count;
		}

		private DateTime GetCustomStartDate()
		{
			return this.Calendar.SelectedDates[0];
        }

        private string[] dayOfWeekStrings;
        private string[] dayOfWeekShortStrings;
        private string firstLetterOfDayOfWeek;
        private int charLength = 1;

        /// <summary>
        /// Used internally. Gets or sets the number of bytes used for a single character.
        /// </summary>
        public virtual int CharLength
        {
            get { return charLength; }
            set { charLength = value; }
        }
       
        internal void SetUpDayOfWeekStrings()
        {
            DateTime dt = DateTime.ParseExact(@"10/23/2005", @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture); ////sunday
            dayOfWeekStrings = new string[7];
            dayOfWeekShortStrings = new string[7];
            firstLetterOfDayOfWeek = string.Empty;

            if (this.Schedule.Culture.TextInfo.ANSICodePage != 1252)
            {
                CharLength = 2;
            }

            for (int i = 0; i < 7; ++i)
            {
                if (this.Schedule.Appearance.VisualStyle == Forms.GridVisualStyles.Metro)
                {
                    dayOfWeekStrings[i] = dt.AddDays(i).ToString("dddd", this.Schedule.Culture).ToUpper();
                    dayOfWeekShortStrings[i] = dt.AddDays(i).ToString("ddd", this.Schedule.Culture).ToUpper();
                }
                else
                {
                    dayOfWeekStrings[i] = dt.AddDays(i).ToString("dddd", this.Schedule.Culture);
                    dayOfWeekShortStrings[i] = dt.AddDays(i).ToString("ddd", this.Schedule.Culture);
                }
                if (dayOfWeekShortStrings[i].Length < CharLength)
                {
                    CharLength = dayOfWeekShortStrings[i].Length;
                }

                if (this.Schedule.Culture.TextInfo.IsRightToLeft)
                {
                    firstLetterOfDayOfWeek += dayOfWeekShortStrings[i].Substring(dayOfWeekShortStrings[i].Length - CharLength, CharLength); //// dayOfWeekShortStrings[i][0];
                }
                else
                {
                    firstLetterOfDayOfWeek += dayOfWeekShortStrings[i].Substring(0, CharLength); //// dayOfWeekShortStrings[i][0];
                }
            }

            this.calendar.CalenderGrid.Tag = firstLetterOfDayOfWeek;
        }
        
        /// <summary>
        /// Initializes the labels from <see cref="DisplayStrings"/>that appear in headers of the ScheduleGrid.
        /// </summary>
        /// <remarks>
        /// Call this method after you make any modifications to <see cref="DisplayStrings"/> to make the
        /// ScheduleGrid use the modified DisplayStrings.
        /// </remarks>
        public void SetHeaderLabel()
        {
            DateTime dt = this.Calendar.SelectedDates.Count > 0 ? this.Calendar.SelectedDates[0] : this.Calendar.DateValue; ////always pull from top calendar
            this.Schedule.HeaderLabel.ForeColor = this.Schedule.Appearance.TextColor;

            switch (this.ScheduleType)
            {
                case ScheduleViewType.Day:
                    columnHeaderText = GetFormattedString(dt, schedule.Appearance.LongHeaderFormat); //// dt.ToString(longHeaderFormat, this.Schedule.Culture);
                    this.Schedule.HeaderLabel.Text = columnHeaderText;
                    break;
                case ScheduleViewType.WorkWeek:
                case ScheduleViewType.Week:
                    columnHeaderText = DisplayStrings[_week_starting] + GetFormattedString(dt, schedule.Appearance.WeekHeaderFormat); //// dt.ToString(schedule.Appearance.WeekHeaderFormat, this.Schedule.Culture);
                    this.Schedule.HeaderLabel.Text = columnHeaderText;

                    break;
                case ScheduleViewType.Month:
                    columnHeaderText = GetFormattedString(dt, schedule.Appearance.MonthHeaderFormat); //// dt.ToString(monthHeaderFormat, this.Schedule.Culture);
                    columnHeaderDate = dt;
                    this.Schedule.HeaderLabel.Text = columnHeaderText;
                    break;
                default:
                    this.Schedule.HeaderLabel.Text = DisplayStrings[_starting] + GetFormattedString(this.displayDates[0], schedule.Appearance.WeekHeaderFormat); //// this.displayDates[0].ToString(schedule.Appearance.WeekHeaderFormat, this.Schedule.Culture);
                    break;
            }		
        }

        /// <summary>
        /// Holds various strings used in the ScheduleGrid control.
        /// </summary>
        /// <remarks>
        /// You can change the strings used on the ScheduleGrid by changing the values
        /// in this string array.
        /// </remarks>
        /// <example>
        /// Here are the default values of this string array.
        /// <code lang="C#">
        /// public static string[] DisplayStrings = new string[]
        ///							{	"no closing delimiter error",          // 0
        ///								"(none)",                              // 1
        ///								"Week starting ",                      // 2
        ///								"Starting ",                           // 3  
        ///                             " Double Click to Add All Day Event",   // 4
        ///                                 "To ",                              //5
        ///                                "From ",                             //6
        ///                                "Remove this appointment?",          //7
        ///                                "Remove Appointment",                //8
        ///                                "&amp;New Item",                         //9
        ///                                "New &amp;All Day item",                 //10
        ///                                "&amp;Edit Item",                        //11
        ///                                "De&amp;lete Item",                      //12
        ///                                "&amp;Day",                              //13
        ///                                "&amp;Work Week",                        //14
        ///                                "Wee&amp;k",                             //15
        ///                                "&amp;Month"                             //16
        ///                                "Click to switch to Day view"            //17
        ///                     	};
        /// </code>
        /// <code lang="VB">
        ///			    Public Shared DisplayStrings() As String = _
        ///                    {"no closing delimiter error", _
        ///                     "(none)", _
        ///                     "Week starting ", _
        ///                     "Starting ", _
        ///                     " Double Click to Add All Day Event",  _
        ///                     "To ", _
        ///                     "From ", _
        ///                     "Remove this appointment?", _
        ///                     "Remove Appointment", _
        ///                     "&amp;New Item",   _
        ///                     "New &amp;All Day item",   _
        ///                     "&amp;Edit Item",   _
        ///                     "De&amp;lete Item",   _
        ///                     "&amp;Day",   _
        ///                     "&amp;Work Week",   _
        ///                     "Wee&amp;k",   _
        ///                     "&amp;Month",  _
        ///                     "Click to switch to Day view"   _
        ///                    }
        /// </code>
        /// </example>
        public static string[] DisplayStrings = new string[]
									{	
                                        "no closing delimiter error", 
										"(none)",
										"Week starting ",
										"Starting ",  
                                        " Double Click to Add All Day Event",
                                        "To ",
                                        "From ",
                                        "Remove this appointment?",
                                        "Remove Appointment",
                                        "&New Item",
                                        "New &All Day item",
                                        "&Edit Item",
                                        "De&lete Item",
                                        "&Day",
                                        "&Work Week",
                                        "Wee&k",
                                        "&Month",
                                        "Click to switch to Day view",
                                        "Click for more Appoinments."
                        			};

        private const int _no_closing_delimiter_error = 0;
        private const int _none = 1;
        private const int _week_starting = 2;
        private const int _starting = 3;
        private const int _Double_Click_to_Add_All_Day_Event = 4;
        private const int _To = 5;
        private const int _From = 6;
        private const int _Remove_this_appointment = 7;
        private const int _Remove_Appointment = 8;

        private const int _New_Item = 9;
        private const int _New_All_Day_item = 10;
        private const int _Edit_Item = 11;
        private const int _Delete_Item = 12;
        private const int _Day = 13;
        private const int _Work_Week = 14;
        private const int _Week = 15;
        private const int _Month = 16;
        private const int _Click_to_switch_to_Day_view = 17;
        private const int _Click_for_more_Appoinments = 18;
        
        #endregion

        #region handle selected dates changing

        private bool inSelectedDates_SelectionsChanged = false;
		private void SelectedDates_SelectionsChanged(object sender, EventArgs e)
		{

            if (!this.schedule.SwitchViewStyle)
            {
                SwitchTo(schedule.ScheduleType, true);
            }
			////ignore changes until a click on a cell or until mouseup
            if (this.calendar.ignoreSelectionChanged)
            {
                return;
            }

            if (inSelectedDates_SelectionsChanged)
            {
                return;
            }
            if (!this.Schedule.SwitchViewStyle)
                return;

			inSelectedDates_SelectionsChanged = true;
            ClearAllDaySpans();
			
			this.BeginUpdate();
            ////this.ScheduleType = ScheduleViewType.WorkWeek;
			if (this.Calendar.SelectedDates.Count == 1)
			{
                ////this.Calendar.DateValue = this.Calendar.SelectedDates[0];
				if (this.displayDates == null 
					|| this.displayDates.Length != this.Calendar.SelectedDates.Count 
					|| this.displayDates[0] != this.Calendar.SelectedDates[0])
				{
                    this.displayDates = new DateTime[] { this.Calendar.SelectedDates[0] }; ////SetRangeInCalendar();
					this.numberPanels = this.displayDates.GetLength(0);
					this.SetupGrid();
                    if (this.ScheduleType != ScheduleViewType.Day)
                    {
                        SwitchTo(ScheduleViewType.Day, true);
                    }
                    else
                    {
                        SetDataToDayPanels();
                    }

					SetHeaderLabel();
				}
                else if (this.ScheduleType != ScheduleViewType.Day)
                {
                    SwitchTo(ScheduleViewType.Day, true);
                }
			}
			else if (this.Calendar.SelectedDates.Count <= this.Schedule.Appearance.DayMonthCutoff)
			{
				if (this.Calendar.SelectedDates.Count != 7 && this.ScheduleType == ScheduleViewType.Week)
				{
					SwitchTo(ScheduleViewType.CustomWeek, false);
				}

                if (this.ScheduleType == ScheduleViewType.Week)
                {
                    this.CancelUpdate();
                }

				this.displayDates = (DateTime[]) this.Calendar.SelectedDates.ToArray(typeof(DateTime)); ////SetRangeInCalendar();
				this.numberPanels = this.displayDates.GetLength(0);
				
				ScheduleViewType t = this.ScheduleType != ScheduleViewType.Week ? ((this.ScheduleType != ScheduleViewType.WorkWeek || numberPanels != 5)  ? ScheduleViewType.CustomWeek : ScheduleViewType.WorkWeek) : ScheduleViewType.Week;
				if (t != this.schedule.ScheduleType)
				{
					SwitchTo(t, true);
				}

				this.displayDates = (DateTime[]) this.Calendar.SelectedDates.ToArray(typeof(DateTime)); ////SetRangeInCalendar();
                if (displayDates.Length > 0)
                {
                    this.SetupGrid();
                    SetDataToDayPanels();
                    SetHeaderLabel();
                }
			}
			else
			{
				if (this.schedule.ScheduleType != ScheduleViewType.Month)
				{
					SwitchTo(ScheduleViewType.Month, false);
				}

				AdjustSelections();
				this.displayDates = SetRangeInCalendar();
				this.numberPanels = this.displayDates.GetLength(0);
				this.SetupGrid();
				SetDataToDayPanels();
				SetHeaderLabel();
			}

            if (this.Updating)
            {
                this.EndUpdate();
            }

			this.calendar.Refresh();
            inSelectedDates_SelectionsChanged = false;
		}

		////expand this.Calendar.SelectedDates to proper ScheduleType
		private void AdjustSelections()
		{
			DateTime dt;
			switch (this.ScheduleType)
			{
				case ScheduleViewType.Month:
                    dt = this.Calendar.SelectedDates[(Calendar.SelectedDates.Count - 1)/ 2];
                    this.Calendar.SelectedDates.BeginUpdate();
                    this.Calendar.SelectedDates.Clear();
                    dt = dt.AddDays(-dt.Day + 1);
                    this.Calendar.SelectedDates.Add(dt);
                    while (dt.Month == dt.AddDays(1).Month)
                    {
                        dt = dt.AddDays(1);
                        this.Calendar.SelectedDates.Add(dt);
                    }

                    this.Calendar.SelectedDates.EndUpdate();
                    break;
				case ScheduleViewType.Week:
                    dt = this.Calendar.SelectedDates[0];
                    while (dt.DayOfWeek != schedule.Appearance.NavigationCalendarStartDayOfWeek)
                    {
                        dt = dt.AddDays(-1);
                        this.Calendar.SelectedDates.Insert(0, dt);
                    }

                    dt = this.Calendar.SelectedDates[Calendar.SelectedDates.Count - 1];
                    while (this.Calendar.SelectedDates.Count != 7)
                    {
                        dt = dt.AddDays(1);
                        this.Calendar.SelectedDates.Add(dt);
                    }

                   break;

				case ScheduleViewType.WorkWeek:
					dt = this.Calendar.SelectedDates[0];
					if (dt.DayOfWeek == DayOfWeek.Saturday || 
						dt.DayOfWeek == DayOfWeek.Sunday)
					{
						dt = dt.AddDays(-2);
						this.Calendar.SelectedDates.Clear();
						this.Calendar.SelectedDates.Add(dt);
					}

					while (dt.DayOfWeek != DayOfWeek.Monday)
					{
						dt = dt.AddDays(-1);
						this.Calendar.SelectedDates.Insert(0, dt);
					}

					dt = this.Calendar.SelectedDates[Calendar.SelectedDates.Count-1];
					while (dt.DayOfWeek != DayOfWeek.Friday)
					{
						dt = dt.AddDays(1);
						this.Calendar.SelectedDates.Add(dt);
					}

					break;
				default:
					break;
			}
        }
        #endregion

        #region button handlers

        private void previousButton_Click(object sender, EventArgs e)
		{
            MoveInDir(false);
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
            MoveInDir(true);
        }

        private void MoveInDir(bool forward)
        {
            string save = this.Schedule.HeaderLabel.Text;

            this.BeginUpdate();

            switch (this.ScheduleType)
            {
                case ScheduleViewType.Day:
                    {
                        DateTime dt = this.Calendar.SelectedDates[0];
                        dt = dt.AddDays(forward ? 1 : -1);
                        this.Calendar.SelectedDates.BeginUpdate();
                        this.Calendar.SelectedDates.Clear();
                        this.Calendar.SelectedDates.Add(dt);
                        this.Calendar.SelectedDates.EndUpdate(false);
                    }

                    break;
                case ScheduleViewType.Week:
                case ScheduleViewType.WorkWeek:
                    {
                        DateTime dt = this.Calendar.SelectedDates[0];
                        dt = dt.AddDays(forward ? 7 : -7);
                        this.Calendar.DateValue = dt;
                        AdjustSelectedDatesByWeek();
                    }

                    break;
                case ScheduleViewType.Month:
                    this.Calendar.BeginUpdate();
                    this.Calendar.AdjustSelectionsByMonth(forward ? 1 : -1);
                    this.Calendar.DateValue = this.Calendar.DateValue.AddMonths(forward ? 1 : -1);
                    this.Schedule.GetScheduleHost().RefreshRange(GridRangeInfo.Row(0));
                    this.Calendar.EndUpdate();
                    break;
                default:
                    ////do nothing
                    break;
            }

            this.EndUpdate();
            if (!this.schedule.NavigationPanel.Visible && this.Schedule.HeaderLabel.Text == save && this.ScheduleType != ScheduleViewType.CustomWeek)
            {
                MoveInDir(forward);
            }
        }

        private void AdjustSelectedDatesByWeek()
        {
            int count = this.ScheduleType == ScheduleViewType.Week ? 7 : 5;
            this.Calendar.SelectedDates.BeginUpdate();
            DateTime theDate = this.Calendar.DateValue;
            while (theDate.DayOfWeek != schedule.Appearance.NavigationCalendarStartDayOfWeek)
            {
                theDate = theDate.AddDays(-1);
            }

            this.Calendar.SelectedDates.Clear();
            while (count > 0)
            {
                this.Calendar.SelectedDates.Add(theDate);
                theDate = theDate.AddDays(1);
                count--;
            }

            this.Calendar.SelectedDates.EndUpdate();
        }
        #endregion
	}
   
    ////used to indicate where the mouse is during a appointment drag in a Week or Month view.
    internal enum DragHitContext
    {
        /// <summary>
        /// No mouse hit.
        /// </summary>
        No, 

        /// <summary>
        /// Mouse is over the schedule control.
        /// </summary>
        Schedule,
        
        /// <summary>
        /// Mouse is over the calendar.
        /// </summary>
        Calendar
    }

    #region ParseDisplayItem Event

    /// <summary>
    /// Event delegate for the <see cref="ScheduleControl.ParseDisplayItem"/> event 
    /// </summary>
    public delegate void ParseDisplayItemEventHandler(object sender, ParseDisplayItemEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="ScheduleControl.ParseDisplayItem"/> event.
    /// </summary>
    public class ParseDisplayItemEventArgs : Syncfusion.ComponentModel.SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ParseDisplayItemEventArgs()
        {
        }

        /// <summary>
        /// Initialize a new object with ScheduleAppointment item and the display format string.
        /// </summary>
        /// <param name="item">ScheduleAppointment item.</param>
        /// <param name="format">Format string.</param>
        public ParseDisplayItemEventArgs(IScheduleAppointment item, string format)
        {
            this.item = item;
            this.format = format;
        }

        string format;
        string formatedText;
        IScheduleAppointment item;

        /// <summary>
        /// A property that gets or sets the format string to be applied to the display message.
        /// </summary>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// A property that gets or sets the formatted display text for the ScheduleAppointment item.
        /// </summary>
        public string FormattedText
        {
            get { return formatedText; }
            set { formatedText = value; }
        }

        /// <summary>
        /// A read-only property that gets the ScheduleAppointment item.
        /// </summary>
        public IScheduleAppointment Item
        {
            get { return item; }
        }
    }

    #endregion

    /// <summary>
    /// Creates a transparent label that is used to display the schedule appointments.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude(),
     ToolboxItem(false)]
    public class TranparentLabel : Panel ////Label
    {
        TranparentLabel(IRecurringScheduleAppointment item)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.item = item;
        }

        /// <summary>
        /// Initializes a <see cref="TranparentLabel"/>.
        /// </summary>
        /// <param name="parent">Schedule grid.</param>
        /// <param name="item">Recurring appointment.</param>
        public TranparentLabel(ScheduleGrid parent, IRecurringScheduleAppointment item)
            : base()
        {
            this.parent = parent;
            this.item = item;
        }

        private string rightText = string.Empty;
        private string leftText = string.Empty;
        private string rightIText = string.Empty;
        private string leftIText = string.Empty;
        private Bitmap rightBmp = null, leftBmp = null;
        private ScheduleGrid parent = null;
        private IRecurringScheduleAppointment item;
        private int screenSlot = -1;

        internal int ScreenSlot
        {
            get { return screenSlot; }
            set { screenSlot = value; }
        }

        /// <summary>
        /// A property that gets or sets the Recurring Appointment item.
        /// </summary>
        public IRecurringScheduleAppointment Item
        {
            get { return item; }
            set { item = value; }
        }

        /// <summary>
        /// Gets the Schedule Grid.
        /// </summary>
        public ScheduleGrid ScheduleGrid
        {
            get { return parent; }
        }

        /// <summary>
        /// A property that gets or sets the text displayed on open right-side of the label.
        /// </summary>
        public string RightText
        {
            get 
            {
                string s = string.Empty;
                string s1 = string.Empty;
                
                if (ScheduleGrid.RightToLeft == RightToLeft.Yes)
                {
                    s1 = leftText;
                    s = leftText.Length == 0 ? leftIText : leftText;
                }
                else
                {
                    s1 = rightText;
                    s = rightText.Length == 0 ? rightIText : rightText;
                }

                if (s1.Length > 0 && rightBmp == null)
                {
                    rightBmp = CreateArrowBitmap(false, string.Empty); ////rightText);
                }

                return s;
            }

            set 
            { 
                rightText = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the text displayed on open left-side of the label.
        /// </summary>
        public string LeftText
        {
            get
            {
                string s = string.Empty;
                string s1 = string.Empty;

                if (ScheduleGrid.RightToLeft == RightToLeft.Yes)
                {
                    s1 = rightText;
                    s = rightText.Length == 0 ? rightIText : rightText;
                }
                else
                {
                    s1 = leftText;
                    s = leftText.Length == 0 ? leftIText : leftText;
                }

                if (s1.Length > 0 && leftBmp == null)
                {
                    leftBmp = CreateArrowBitmap(true, string.Empty);
                }

                return s;
            }

            set 
            { 
                leftText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed on interior right-side of the label.
        /// </summary>
        public string RightIText
        {
            get
            {
                return rightIText;
            }

            set
            {
                rightIText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed on interior left-side of the label.
        /// </summary>
        public string LeftIText
        {
            get
            {
                return leftIText;
            }

            set
            {
                leftIText = value;
            }
        }

        private Color boxColor;
        private Color markerColor;

        /// <summary>
        /// A property that gets or sets the color used to draw the label background.
        /// </summary>
        public Color BoxColor
        {
            get { return boxColor; }
            set { boxColor = value; }
        }
      
        /// <summary>
        /// A property that gets or sets the color used to draw the label background.
        /// </summary>
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        /// A read-only property that specifies whether the label should have rounded corners.
        /// </summary>
        public bool DrawRounded
        {
            get 
            {
                return schedule.ShowRoundedCorners; 
            }
        }
      
        private ScheduleControl schedule = null;

       /// <summary>
       /// A property that gets or sets the ScheduleControl.
       /// </summary>
        public ScheduleControl Schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        private Color GetGridBackColor(int row, int col)
        {
            Color c = parent.Schedule.Appearance.AllDayBackColor;
            GridStyleInfo style = parent.GetViewStyleInfo(row, col);

            switch (parent.ScheduleType)
            {
                case ScheduleViewType.Month:
                    DateTime dt;
                    ////adjust row so it is the leading odd row
                    row = ((row % 2) == 0 && row > 0) ? row - 1 : row;
                    if (parent[row, col].CellValue != null)
                    {
                        string s = parent[row, col].CellValue.ToString();
                        int i = -1;
                        if ((i = s.IndexOf('\n')) > -1)
                        {
                            s = s.Substring(0, i);
                        }

                        if (DateTime.TryParse(s, this.Schedule.Culture.DateTimeFormat, DateTimeStyles.None, out dt))
                        {
                            c = dt.Month == parent.ColumnHeaderDate.Month ? parent.Schedule.Appearance.PrimeTimeCellColor : parent.Schedule.Appearance.NonPrimeTimeCellColor;
                        }
                    }

                    break;
                case ScheduleViewType.Week:
                    c = parent.Schedule.Appearance.NonPrimeTimeCellColor;
                    break;
                default:
                    break;
            }

             return c;
        }

        /// <override/>
        protected override void OnPaint(PaintEventArgs e)
        {
           // Rectangle rect = this.ClientRectangle;
            Rectangle rect = this.parent.ClientRectangle;
            rect.Intersect(this.ClientRectangle);
            if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
            {
                this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            }
            if (parent != null && schedule.ShowRoundedCorners && this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
            {
                ////draw little rectangles to match the grid cell background
                Rectangle r1 = this.Bounds;
                r1.Width = 20;
                int row, col;
                if (parent.PointToRowCol(r1.Location, out row, out col))
                {
                     using (Brush b = new SolidBrush(GetGridBackColor(row, col)))
                    {
                        r1 = rect;
                        r1.Width = 20;
                        e.Graphics.FillRectangle(b, r1);
                    }
                }

                r1 = this.Bounds;
                r1.X = rect.Right - 20;
                if (parent.PointToRowCol(r1.Location, out row, out col))
                {
                    ////Console.WriteLine("[loc={2}    {0}.{1}]  value={3}", row, col, r1.Location, parent[row, col].CellValue);
                    using (Brush b = new SolidBrush(GetGridBackColor(row, col)))
                    {
                        r1 = rect;
                        r1.X = rect.Right - 20;
                        e.Graphics.FillRectangle(b, r1);
                    }
                }
            }

            if (DrawRounded && this.Schedule.Appearance.VisualStyle != Forms.GridVisualStyles.Metro)
            {
                rect.Width -= 8;
                rect.X += 4;
                rect.Height -= 1;
            }

            if (LeftText == string.Empty && leftBmp != null)
            {
                leftBmp = null;
            }

            if (RightText == string.Empty && rightBmp != null)
            {
                rightBmp = null;
            }

            Point pt = rect.Location;
            if (schedule.ScheduleType != ScheduleViewType.Week)
            {
                pt.Y += (schedule.ScheduleType == ScheduleViewType.Month) ? 1 : 2;
            }

            int doLeftBm = 0;
            int doLeftText = 0;
            int doText = 0;
            int doRightText = 0;
            int doRightBm = 0;
            CheckIfRoom(out doLeftBm, out doLeftText, out doText, out doRightText, out doRightBm, pt, rect.Width);

            using (Brush b1 = new SolidBrush(this.ForeColor))
            {
                if (DrawRounded)
                {
                    System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = ScheduleGrid.DrawRoundRect(rect.X, rect.Y, rect.Width, rect.Height, Math.Min(rect.Height / 4, 12));
                    using (Brush b = new SolidBrush(SystemColors.Window))
                    {
                        e.Graphics.FillPath(b, myGraphicsPath);
                    }
                    Color cr = boxColor;
                    if (this.Schedule.Appearance.EnableTransparentSpan)
                    {
                        if (this.Schedule.Appearance.SpanTransparencyLevel > 0 && this.Schedule.Appearance.SpanTransparencyLevel < 255)
                            cr = Color.FromArgb(this.Schedule.Appearance.SpanTransparencyLevel, BoxColor);
                    }
                    using (Brush b = new System.Drawing.Drawing2D.LinearGradientBrush(rect, cr, cr, 90, true))
                    {
                        e.Graphics.FillPath(b, myGraphicsPath);
                    }

                    Color c1 = BoxColor;
                    using (Pen p = new Pen(c1))
                    {
                        e.Graphics.DrawPath(p, myGraphicsPath);
                    }

                    myGraphicsPath.Dispose();
                }
                else
                {
                    rect.Width -= 1;
                    Color cr = boxColor;
                    if (this.Schedule.Appearance.EnableTransparentSpan)
                    {
                        if (this.Schedule.Appearance.SpanTransparencyLevel > 0 && this.Schedule.Appearance.SpanTransparencyLevel < 255)
                            cr = Color.FromArgb( this.Schedule.Appearance.SpanTransparencyLevel , BoxColor);
                    }
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        Rectangle lableRect = rect;
                        lableRect.X += 8;
                        lableRect.Width -= 8;
                        using (Brush b2 = new SolidBrush(cr))
                        {
                            e.Graphics.FillRectangle(b2, lableRect);

                            if (BoxColor.R == this.Schedule.Appearance.PrimeTimeCellColor.R
                                && BoxColor.G == this.Schedule.Appearance.PrimeTimeCellColor.G
                                && BoxColor.B == this.Schedule.Appearance.PrimeTimeCellColor.B)
                            {
                                Color c = Color.FromArgb((BoxColor.R + 128) % 255, (BoxColor.G + 128) % 255, (BoxColor.B + 128) % 255);
                                using (Pen p = new Pen(c))
                                {
                                    rect.Height -= 1;
                                    e.Graphics.DrawRectangle(p, rect);
                                }
                            }
                        }
                        Rectangle markerRect = rect;
                        using (Brush b2 = new SolidBrush(Color.FromArgb(100, MarkerColor)))
                        {
                            markerRect.Width = rect.Width - lableRect.Width;
                            e.Graphics.FillRectangle(b2, markerRect);
                        }
                    }
                    else
                    {
                        using (Brush b2 = new SolidBrush(cr))
                        {
                            e.Graphics.FillRectangle(b2, rect);

                            if (BoxColor.R == this.Schedule.Appearance.PrimeTimeCellColor.R
                                && BoxColor.G == this.Schedule.Appearance.PrimeTimeCellColor.G
                                && BoxColor.B == this.Schedule.Appearance.PrimeTimeCellColor.B)
                            {
                                Color c = Color.FromArgb((BoxColor.R + 128) % 255, (BoxColor.G + 128) % 255, (BoxColor.B + 128) % 255);
                                using (Pen p = new Pen(c))
                                {
                                    rect.Height -= 1;
                                    e.Graphics.DrawRectangle(p, rect);
                                }
                            }
                        }
                    }
                }
                if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                {
                    pt.Y += 4;
                }
                if (doText > 0) 
                {
                    e.Graphics.DrawString(this.Text, this.Font, b1, new Point(doText, pt.Y));
                }

                if (doLeftBm > 0)
                {
                    e.Graphics.DrawImageUnscaled(leftBmp, new Point(doLeftBm, rect.Location.Y));
                }

                if (doLeftText > 0)
                {
                    if (DrawRounded && doLeftBm == 0)
                    {
                        doLeftText += 2;
                    }

                    e.Graphics.DrawString(LeftText, this.Font, b1, new Point(doLeftText, pt.Y));
                }

                if (doRightText > 0)
                {
                    if (DrawRounded)
                    {
                        doRightText -= 2;
                        if (doRightBm == 0)
                        {
                            doRightText += 6;
                        }
                    }

                    e.Graphics.DrawString(RightText, this.Font, b1, new Point(doRightText, pt.Y));
                }

                if (doRightBm > 0)
                {
                    e.Graphics.DrawImageUnscaled(rightBmp, new Point(doRightBm, rect.Location.Y));
                }
            }
        }

        private void CheckIfRoom(out int doLeftBm, out int doLeftText, out int doText, out int doRightText, out int doRightBm, Point pt, int width1)
        {
            doLeftBm = 0;
            doLeftText = 0;
            doText = 0;
            doRightText = 0;
            doRightBm = 0;

            int width = width1;
            int leftTextLen = 0;
            int textLen = 0;
            int rightTextLen = 0;
            int leftBmpWidth = leftBmp != null ? leftBmp.Width : 0;
            if (LeftText.Length > 0)
            {
                if (leftBmpWidth > 0 && width > leftBmpWidth)
                {
                    width = width - leftBmpWidth - 2;
                    doLeftBm = pt.X + 2;
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        doLeftBm += 8;
                    }
                }

                leftTextLen = TextRenderer.MeasureText(LeftText, Font).Width;
                if (width > leftTextLen + 2)
                {
                    width = width - leftTextLen - 2;
                    doLeftText = Math.Max(2, doLeftBm + leftBmpWidth - 1);
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        doLeftText += 8;
                    }
                }
            }

            if (width > 0 && RightText.Length > 0)
            {
                int rightBmpWidth = rightBmp != null ? rightBmp.Width : 0;
                if (rightBmpWidth > 0 && width > rightBmpWidth)
                {
                    doRightBm = width1 - rightBmpWidth - (DrawRounded ? -3 : 1);
                    width = width - rightBmpWidth - 2;
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        doRightBm -= 4;
                    }
                }

                rightTextLen = TextRenderer.MeasureText(RightText, Font).Width;
                if (doRightBm > 0 && width > rightTextLen + 2)
                {
                    doRightText = doRightBm - rightTextLen + (DrawRounded ? 4 : 2);
                    width = width - rightTextLen - 2;
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        doRightText -= 4;
                    }
                }
                else if (width > rightTextLen + 2)
                {
                    doRightText = width1 - rightTextLen + (DrawRounded ? 4 : 2);
                    width = width - rightTextLen - 2;
                    if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                    {
                        doRightText -= 4;
                    }
                }
                if (!this.Schedule.Appearance.MonthShowFullWeek)
                {
                    int row, col, location = 14;
                    if (parent.PointToRowCol(this.Bounds.Location, out row, out col))
                    {
                        if (col > 1 && col < 6)
                        {
                            doRightText -= rightTextLen - location - (DrawRounded ? 3 : 1);
                            doRightBm -= rightTextLen - location - (DrawRounded ? 3 : 1);
                        }
                        if (col == 6)
                        {
                            doRightText += 20;
                            doRightBm -= rightTextLen - location - (DrawRounded ? 3 : 1);
                        }
                        if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                        {
                            doRightText -= 4;
                        }
                    }
                }
            }

            if (width > 0)
            {
                textLen = TextRenderer.MeasureText(Text, Font).Width;
                if (width > textLen + 2)
                {
                    doText = ((doLeftText > 0) ? leftTextLen : 0) + ((doLeftBm > 0) ? leftBmpWidth : 0) + (width - textLen) / 2;
                }
                if (this.Schedule.Appearance.VisualStyle == GridVisualStyles.Metro)
                {
                    doText -= 5;
                }
            }
        }

        /// <override/>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            ////draw default backcolor
            Rectangle rect = this.ClientRectangle;
            // this.BackColor)) //Color.Red))//GridColor))
            using (Brush b = new SolidBrush(schedule.GetScheduleHost().BackColor))
            {
                pevent.Graphics.FillRectangle(b, rect);
            }

            // this.BackColor)) //Color.Red))//GridColor))
            using (Brush b = new SolidBrush(schedule.GetScheduleHost().BackColor))
            {
                pevent.Graphics.FillRectangle(b, new Rectangle(rect.X, rect.Y, 4, rect.Height));
            }

            // this.BackColor)) //Color.Red))//GridColor))
            using (Brush b = new SolidBrush(schedule.GetScheduleHost().BackColor))
            {
                pevent.Graphics.FillRectangle(b, new Rectangle(rect.Width - 4, rect.Y, 5, rect.Height));
            }
        }

        private Bitmap CreateArrowBitmap(bool left, string label)
        {
            Bitmap bm = null;
            Graphics g = null;

            Bitmap arrowBitmap = ScheduleGrid.GetBitmap((left ? "LeftArrow.bmp" : "RightArrow.bmp"));
              
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
            Size labelSize = Size.Empty; //// TextRenderer.MeasureText(label, this.Font);
#else
			SizeF labelSize = Size.Empty;//this.CreateGraphics().MeasureString(label, this.Font);
#endif
      
            Size size = new Size((int)labelSize.Width + arrowBitmap.Width, this.Height);

            Rectangle bounds = new Rectangle(Point.Empty, size);

            try
            {
                bm = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(bm);

                g.FillRectangle(new SolidBrush(BackColor), new Rectangle(Point.Empty, size));
                int y = (this.Height / 2) - (arrowBitmap.Height / 2);
                
                g.DrawImageUnscaled(arrowBitmap, left ? 0 : (int)labelSize.Width, y);
                y = (this.Height / 2) - ((int)labelSize.Height / 2);
                g.DrawString(label, this.Font, new SolidBrush(this.ForeColor), left ? arrowBitmap.Width : 0, y);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            bm.MakeTransparent(BackColor);
            return bm;
        }

        /// <override/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ScheduleGrid != null && ScheduleGrid.Schedule.Appearance.ScheduleAppointmentTipsEnabled)
            {
                if (ScheduleGrid.ScheduleAppointmentToolTip == null)
                {
                    ScheduleGrid.ScheduleAppointmentToolTip = new ToolTip();
                    ScheduleGrid.ScheduleAppointmentToolTip.InitialDelay = 500;
                }

                ItemHitType hit;
                Point pt = this.Location;
                pt.Offset(0, 2);
                IScheduleAppointment item = ScheduleGrid.GetItemAtPoint(pt, out hit);
                if (item != null)
                {
                    if (!item.Equals(ScheduleGrid.mouseMoveItem))
                    {
                        ScheduleGrid.mouseMoveItem = item;
                        ScheduleGrid.ScheduleAppointmentToolTip.Active = false; ////turn it off
                        ScheduleGrid.ScheduleAppointmentToolTip.SetToolTip(this, ScheduleGrid.ParseDisplayItem(item, ScheduleGrid.Schedule.Appearance.ScheduleAppointmentTipFormat));
                        ScheduleGrid.ScheduleAppointmentToolTip.Active = true; ////turn it on
                    }
                }
                else
                {
                    ScheduleGrid.ScheduleAppointmentToolTip.Active = false;
                    ScheduleGrid.mouseMoveItem = null;
                }
            }

            base.OnMouseMove(e);
        }
    }

    /// <summary>Only for internal use.</summary>
    /// <internalonly/>
   [Syncfusion.Documentation.DocumentationExclude()]
   public class SpanLayoutManager
   {
        internal Dictionary<DateTime, string> layMarker;
        private ScheduleControl schedule;

        /// <summary>Only for internal use.</summary>
        /// <param name="schedule">Schedule control.</param>
        /// <internalonly/>
       public SpanLayoutManager(ScheduleControl schedule)
       {
           layMarker = new Dictionary<DateTime, string>();
           this.schedule = schedule;
       }

       /// <summary>A method that clears the layout. Only for internal use.</summary>
       /// <internalonly/>
       public void Clear()
       {
           layMarker.Clear();
       }

       /// <summary>Only for internal use.</summary>
       /// <internalonly/>
       public void MarkSlotRange(DateTime start, DateTime end, int pos)
       {
           for (DateTime dt = start.Date; dt <= end.Date; dt = dt.AddDays(1))
           {
               if (schedule.ScheduleType == ScheduleViewType.Month && dt > start.Date
                    && dt.DayOfWeek == schedule.Appearance.MonthCalendarStartDayOfWeek)
               {
                   break;
               }

               MarkSlot(dt, pos);
           }
       }

       /// <summary>Only for internal use.</summary>
       /// <internalonly/>
       public void MarkSlot(DateTime dt, int pos)
       {
           if (layMarker.ContainsKey(dt))
           {
               string s = layMarker[dt];
               if (pos > s.Length - 1)
               {
                   s += pos == s.Length ? "1" : new string('0', pos - s.Length) + "1";
               }
               else if (pos == 0)
               {
                   s = "1" + s.Substring(1);
               }
               else
               {
                   s = s.Substring(0, pos) + "1" + s.Substring(pos + 1);
               }

               layMarker[dt] = s;
           }
           else
           {
               string s = pos == 0 ? "1" : new string('0', pos) + "1";
               layMarker.Add(dt, s);
           }
       }

       /// <summary>Only for internal use.</summary>
       /// <internalonly/>
       /// <returns>Positon of free slot.</returns>
       public int GetFreeSlot(DateTime dt)
       {
           int pos = 0;

           if (layMarker.ContainsKey(dt))
           {
               string s = layMarker[dt];
               pos = s.IndexOf("0");
               if (pos == -1)
               {
                   pos = s.Length;
                   s += "1";
               }
               else
               {
                   s = s.Substring(0, pos + 1).Replace('0', '1') + s.Substring(pos + 1);
               }

               layMarker[dt] = s;
           }
           else
           {
               layMarker.Add(dt, "1");
           }

           return pos;
       }

       internal int BottomSlot(DateTime dt)
       {
           int i = 0;
           if (layMarker.ContainsKey(dt))
           {
               i = layMarker[dt].LastIndexOf("1") + 1;
           }

           return i;
       }
   }
}
