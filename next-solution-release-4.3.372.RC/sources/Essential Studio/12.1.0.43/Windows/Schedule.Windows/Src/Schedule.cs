//-------------------------------------------------------------------------------------------------
// <copyright file="Schedule.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// A ScheduleControl class that provides the basic scheduling functionality.
    /// It serves as a wrapper class for a ScheduleGrid and a NavigationCalendar that work together to provide
    /// this scheduling functionality.
    /// </summary>
    /// <remarks>
    /// To use the ScheduleControl, place it on Form or UserControl, and then set its <see cref="DataSource"/>
    /// property to an object that implements <see cref="IScheduleDataProvider"/> that provides a data store
    /// for the schedule items that you want to use in the ScheduleControl. If you do not set ScheduleControl.DataSource,
    /// then an IScheduleDataProvider implementation based on ArrayLists will be used. See 
    /// <see cref="ArrayListDataProvider"/> for more information.
    /// </remarks>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(ScheduleControl), "ToolboxIcons.ScheduleControl.bmp")]
    public class ScheduleControl : System.Windows.Forms.UserControl
    {
        #region internal UI objects

        private System.Windows.Forms.Panel navigationPanel;
        internal System.Windows.Forms.Splitter splitterMiddle;
        private System.Windows.Forms.Panel panelFill;
        internal System.Windows.Forms.Panel panelFixedTop;

        internal System.Windows.Forms.Panel panelSpacer;

        private FillPanel fillPanel;

        internal System.Windows.Forms.Splitter navigationPanelSplitter;
        internal OfficeThemedButton previousButton;
        internal OfficeThemedButton nextButton;

        private System.Windows.Forms.Label headerLabel;
        private System.ComponentModel.IContainer components = null;
        internal bool isMetro = false;
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ScheduleControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.nextButton.Visible = this.Appearance.ShowCaptionButtons;
            this.previousButton.Visible = this.Appearance.ShowCaptionButtons;
            this.nextButton.ThemesEnabled = this.Appearance.ThemesEnabled;
            this.previousButton.ThemesEnabled = this.Appearance.ThemesEnabled;
            this.CaptionPanel.Visible = this.Appearance.ShowCaption;
            this.HorizontalScroll.Enabled = false;
            this.HorizontalScroll.Visible = false;
        }

        private NavigationCalendar calendar = null;

        /// <summary>
        /// A read-only property that gets the NavigationCalendar associated with is SceduleControl.
        /// </summary>
        /// <remarks>
        /// The NavigationCalendar is a derive GridControl that displays
        /// one or more Calendars that let the user select dates to be displayed
        /// in the ScheduleControl.
        /// If you want to use a derived NavigationCalendar, then you can do so 
        /// by overriding CreateNavigationCalendar and returning an instance of 
        /// your derived NavigationCalendar.
        /// </remarks>
        public NavigationCalendar Calendar
        {
            get
            {
                if (calendar == null)
                {
                    calendar = CreateNavigationCalendar();
                }

                return calendar;
            }
        }

        private bool allowSecondsInAppointment = false;
    
        /// <summary>
        /// Gets/Sets enable seconds support for schedule appoinment.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets/Sets enable seconds support for schedule appoinment.")]
        [XmlIgnore]
        public bool AllowSecondsInAppointment
        {
            get
            {
                return allowSecondsInAppointment;
            }
            set
            {
                if (allowSecondsInAppointment != value)
                {
                    this.Appearance.TimeFormat = "hh:mm:ss tt";
                    allowSecondsInAppointment = value;
                }
            }
        }

        internal System.Windows.Forms.Panel captionPanel;

        /// <summary>
        /// A read-only property that gets the Panel, holding the caption above the Calendar. This panel may hold two navigation buttons and <see cref="HeaderLabel"/>.
        /// </summary>
        public Panel CaptionPanel
        {
            get { return this.captionPanel; }
        }

        /// <summary>
        /// Override this method to have a derived ScheduleControl use a
        /// derived NavigationCalendar.
        /// </summary>
        /// <returns>A NavigationCalendar object that this ScheduleControl
        /// will use as its Calendar object.
        /// </returns>
        public virtual NavigationCalendar CreateNavigationCalendar()
        {
            return new NavigationCalendar();
        }

        /// <override/> 
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            if(GetScheduleHost() != null)
                GetScheduleHost().SwitchTo(this.ScheduleType, true);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
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
			this.navigationPanel = new System.Windows.Forms.Panel();
			this.navigationPanelSplitter = new System.Windows.Forms.Splitter();
			this.panelFixedTop = new System.Windows.Forms.Panel();
			this.panelSpacer = new System.Windows.Forms.Panel();
			this.splitterMiddle = new System.Windows.Forms.Splitter();
			this.panelFill = new System.Windows.Forms.Panel();
			this.fillPanel = new Syncfusion.Windows.Forms.Schedule.FillPanel();
			this.captionPanel = new System.Windows.Forms.Panel();
			this.headerLabel = new System.Windows.Forms.Label();
            this.nextButton = new OfficeThemedButton();
            this.previousButton = new OfficeThemedButton();
			this.navigationPanel.SuspendLayout();
			this.panelFixedTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Calendar)).BeginInit();
			this.panelFill.SuspendLayout();
			this.captionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// navigationPanel
			// 
			this.navigationPanel.Controls.Add(this.navigationPanelSplitter);
            this.navigationPanel.Controls.Add(this.panelFixedTop);
			this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.navigationPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F);
			this.navigationPanel.Location = new System.Drawing.Point(0, 0);
			this.navigationPanel.Name = "navigationPanel";
			this.navigationPanel.Size = new System.Drawing.Size(176, 448);
			this.navigationPanel.TabIndex = 3;
			// 
			// navigationPanelSplitter
			// 
			this.navigationPanelSplitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.navigationPanelSplitter.Location = new System.Drawing.Point(0, 328);
			this.navigationPanelSplitter.Name = "navigationPanelSplitter";
			this.navigationPanelSplitter.Size = new System.Drawing.Size(176, 3);
			this.navigationPanelSplitter.TabIndex = 2;
			this.navigationPanelSplitter.TabStop = false;
			// 
			// panelFixedTop
			// 
			this.panelFixedTop.Controls.Add(this.Calendar);
			//this.panelFixedTop.Controls.Add(this.panelSpacer);//xxxx
			this.panelFixedTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelFixedTop.DockPadding.All = 1;
			this.panelFixedTop.Location = new System.Drawing.Point(0, 0);
			this.panelFixedTop.Name = "panelFixedTop";
			this.panelFixedTop.Size = new System.Drawing.Size(176, 284);
			this.panelFixedTop.TabIndex = 1;
            
			// 
			// Calendar
			// 
			this.Calendar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Calendar.Location = new System.Drawing.Point(1, 29);
			this.Calendar.Name = "Calendar";
			this.Calendar.Size = new System.Drawing.Size(174, 298);
			this.Calendar.TabIndex = 0;
			// 
			// panelSpacer
			// 
			this.panelSpacer.Dock = System.Windows.Forms.DockStyle.Left;//xxxx .Top;
			this.panelSpacer.Location = new System.Drawing.Point(1, 1);
			this.panelSpacer.Name = "panelSpacer";
			this.panelSpacer.Size = new System.Drawing.Size(174, 28);
			this.panelSpacer.TabIndex = 1;
			// 
			// splitterMiddle
			// 
			this.splitterMiddle.Location = new System.Drawing.Point(176, 0);
			this.splitterMiddle.Name = "splitterMiddle";
			this.splitterMiddle.Size = new System.Drawing.Size(3, 448);
			this.splitterMiddle.TabIndex = 1;
			this.splitterMiddle.TabStop = false;
			// 
			// panelFill
			// 
			this.panelFill.Controls.Add(this.fillPanel);
			//this.panelFill.Controls.Add(this.captionPanel);
			this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelFill.Location = new System.Drawing.Point(179, 0);
			this.panelFill.Name = "panelFill";
			this.panelFill.Size = new System.Drawing.Size(461, 448);
			this.panelFill.TabIndex = 2;
			// 
			// fillPanel
			// 
			this.fillPanel.BackColor = System.Drawing.Color.PaleGoldenrod;
			this.fillPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fillPanel.Location = new System.Drawing.Point(0, 28);
			this.fillPanel.Name = "fillPanel";
			this.fillPanel.Size = new System.Drawing.Size(461, 420);
			this.fillPanel.TabIndex = 2;
			// 
			// captionPanel
			// 
			this.captionPanel.Controls.Add(this.headerLabel);
			this.captionPanel.Controls.Add(this.nextButton);
			this.captionPanel.Controls.Add(this.previousButton);
            //this.captionPanel.Controls.Add(this.panelSpacer);
			this.captionPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.captionPanel.DockPadding.Bottom = 1;
            this.captionPanel.DockPadding.Left = 1;
            this.captionPanel.DockPadding.Right = 1;
			this.captionPanel.DockPadding.Top = 1;
			this.captionPanel.Location = new System.Drawing.Point(0, 0);
			this.captionPanel.Name = "captionPanel";
			this.captionPanel.Size = new System.Drawing.Size(461, 28);
			this.captionPanel.TabIndex = 1;
			// 
			// headerLabel
			// 
			this.headerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.headerLabel.Location = new System.Drawing.Point(75, 1);
			this.headerLabel.Name = "headerLabel";
			this.headerLabel.Size = new System.Drawing.Size(311, 26);
			this.headerLabel.TabIndex = 2;
			this.headerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// nextButton
			// 
			this.nextButton.BackColor = System.Drawing.SystemColors.Control;
			this.nextButton.ButtonType = System.Windows.Forms.ScrollButton.Right;
			this.nextButton.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.nextButton.DefaultButtonState = System.Windows.Forms.ButtonState.Flat;
			this.nextButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.nextButton.DrawText = false;
			this.nextButton.FlatColor = System.Drawing.SystemColors.ControlDark;
			this.nextButton.Location = new System.Drawing.Point(386, 1);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(35, 26);
			this.nextButton.Style = Syncfusion.Windows.Forms.VisualStyle.Default;
			this.nextButton.TabIndex = 1;
			this.nextButton.Text = ">";
			this.nextButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.nextButton.ThemesEnabled = true;
			this.nextButton.Transparent = false;

			// 
			// previousButton
			// 
			this.previousButton.BackColor = System.Drawing.SystemColors.Control;
			this.previousButton.ButtonType = System.Windows.Forms.ScrollButton.Left;
			this.previousButton.CheckState = System.Windows.Forms.CheckState.Unchecked;
			this.previousButton.DefaultButtonState = System.Windows.Forms.ButtonState.Flat;
			this.previousButton.Dock = System.Windows.Forms.DockStyle.Left;
			this.previousButton.DrawText = false;
			this.previousButton.FlatColor = System.Drawing.SystemColors.ControlDark;
			this.previousButton.Location = new System.Drawing.Point(40, 1);
			this.previousButton.Name = "previousButton";
			this.previousButton.Size = new System.Drawing.Size(35, 26);
			this.previousButton.Style = Syncfusion.Windows.Forms.VisualStyle.Default;
			this.previousButton.TabIndex = 0;
			this.previousButton.Text = "<";
			this.previousButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.previousButton.ThemesEnabled = true;
			this.previousButton.Transparent = false;
			// 
			// ScheduleControl
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((byte)(192)), ((byte)(201)), ((byte)(219)));
			this.Controls.Add(this.panelFill);
		
			this.Controls.Add(this.splitterMiddle);
			this.Controls.Add(this.navigationPanel);
			this.Controls.Add(this.captionPanel);
			this.Name = "ScheduleControl";
			this.Size = new System.Drawing.Size(640, 448);
			this.navigationPanel.ResumeLayout(false);
			this.panelFixedTop.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Calendar)).EndInit();
			this.panelFill.ResumeLayout(false);
			this.captionPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        #region Helper Methods

        /// <summary>
        /// A method that adds a Control to the NavigationPanel underneath the NavigationCalendar.
        /// </summary>
        /// <param name="c">The Control to be added.</param>
        public void AddControlToNavigationPanel(Control c)
        {
            if (this.navigationPanel.Controls.IndexOf(c) == -1)
            {
                this.FreezePainting = true;
                Control[] a = new Control[this.navigationPanel.Controls.Count + 1];
                a[0] = c;
                for (int i = 0; i < this.navigationPanel.Controls.Count; ++i)
                {
                    a[i + 1] = this.navigationPanel.Controls[i];
                }

                this.navigationPanel.Controls.Clear();

                foreach (Control c1 in a)
                {
                    this.navigationPanel.Controls.Add(c1);
                }

                this.FreezePainting = false;
                this.Refresh();
            }
        }

        /// <summary>
        /// Removes a Control to the NavigationPanel underneath the NavigationCalendar.
        /// </summary>
        /// <param name="c">The Control to be removed.</param>
        public void RemoveControlFromNavigationPanel(Control c)
        {
            if (this.navigationPanel.Controls.IndexOf(c) > -1)
            {
                this.navigationPanel.Controls.Remove(c);
            }
        }

        /// <summary>
        /// Sets the position of the NavigationPanel.
        /// </summary>
        /// <param name="newPosition">Specifies the left or right position of the NavigationPanel, 
        /// or whether the NavigationPanel is hidden.</param>
        public void SetNavigationPanelPosition(CalendarNavigationPanelPosition newPosition)
        {
            if (this.activePanel != null)
            {
                this.activePanel.BeginUpdate();
            }

            this.FreezePainting = true;
            this.navigationPanelPosition = newPosition;
            switch (newPosition)
            {
                case CalendarNavigationPanelPosition.Hidden:
                    this.navigationPanel.Hide();
                    break;
                case CalendarNavigationPanelPosition.Left:
                    this.navigationPanel.Dock = DockStyle.Left;
                    this.navigationPanel.Show();
                    break;
                case CalendarNavigationPanelPosition.Right:
                    this.navigationPanel.Dock = DockStyle.Right;
                    this.navigationPanel.Show();
                    break;
                default:
                    break;
            }

            this.FreezePainting = false;
            if (this.activePanel != null)
            {
                this.activePanel.Refresh();
                this.activePanel.EndUpdate();
            }
        }

        ScheduleGrid activePanel;

        /// <summary>
        /// Returns the ScheduleGrid that holds the schedule.
        /// </summary>
        /// <returns>A ScheduleGrid.</returns>
        /// <remarks>
        /// The <see cref="ScheduleGrid"/> is a GridControl derived class that displays the
        /// IScheduleAppointments. The appearance and layout of this grid varies with the
        /// <see cref="ScheduleViewType"/>.
        /// </remarks>
        public ScheduleGrid GetScheduleHost()
        {
            return activePanel;
        }

        /// <summary>
        /// Creates the ScheduleGrid used by this ScheduleControl.
        /// </summary>
        /// <param name="calendar">The NavigationCalendar to be used by this ScheduleControl.</param>
        /// <param name="schedule">The ScheduleControl that is the parent of this ScheduleGrid.</param>
        /// <param name="initialDate">The initial DateTime to be displayed.</param>
        /// <returns>ScheduleGrid used by this ScheduleControl.</returns>
        /// <remarks>
        /// Override this method if you want your ScheduleControl to use a derived ScheduleGrid.
        /// </remarks>
        public virtual ScheduleGrid CreateScheduleGrid(NavigationCalendar calendar, ScheduleControl schedule, DateTime initialDate)
        {
            ////raise an event to allow listeners to subcribe to grid events or use a derived ScheduleGrid
            ScheduleGridCreatedEventArgs e = new ScheduleGridCreatedEventArgs(calendar, schedule, initialDate);
            OnScheduleGridCreated(e);
            if (e.Handled && e.Grid != null)
            {
                return e.Grid;
            }

            return new ScheduleGrid(calendar, schedule, initialDate);
        }

        /// <summary>
        /// Returns information about the IScheduleAppointment, if any, under the point.
        /// </summary>
        /// <param name="pt">The Point to be tested.</param>
        /// <param name="hit">Returns information regarding the the location of the point.</param>
        /// <returns>The IScheduleAppointment under the point. Returns null if no item under the point.</returns>
        public IScheduleAppointment GetItemAtPoint(Point pt, out ItemHitType hit)
        {
            hit = ItemHitType.None;
            if (activePanel == null)
            {
                return null;
            }

            return this.activePanel.GetItemAtPoint(pt, out hit);
        }

        /// <summary>
        /// Resets any actively displayed schedules and 
        /// re-displays them using the passed in schedule type.
        /// </summary>
        /// <param name="t">The ScheduleViewType that will be used to display 
        /// the current schedule items.
        /// </param>
        /// <remarks>Use this method to change the current ScheduleViewType
        /// to a new ScheduleViewType.</remarks>
        public void ResetProvider(ScheduleViewType t)
        {
            if (activePanel == null)
            {
                return;
            }

            ////this.Cursor = Cursors.WaitCursor;
            ////freeze the painting to avoid flickers
            this.FreezePainting = true;

            ////cache the data
            IScheduleDataProvider data = this.dataProvider;

            ////remove the schedule
            this.fillPanel.Controls.Remove(activePanel);
            activePanel.Dispose();
            activePanel = null;
            this.dataProvider = null;

            ////reset the data 
            this.DataSource = data;

            ////set the view type - this creates a new active panel
            this.ScheduleType = t;
            ////populate the panel with data
            activePanel.DataSource = data;

            this.FreezePainting = false;
            ////this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// A method that adds a multiday span appointment to a dataProvider.
        /// </summary>
        /// <param name="item">Holds the appointment to be added.</param>
        /// <param name="dataProvider">The DataProvider.</param>
        /// <remarks>
        /// Multiday appointments are not simply single entries in the appointment list. This method takes the 
        /// information in an <see cref="IRecurringScheduleAppointment"/> and adds it to the DataProvider as 
        /// a span appointment covering more than a single day. The process properly populates the 
        /// IRecurringScheduleAppointment.DateList collection.
        /// </remarks>
        public void AddSpanAppointment(IRecurringScheduleAppointment item, IRecurringScheduleDataProvider dataProvider)
        {
            int startHour = ScheduleGrid.SpanOnlyPrimeTime ? this.Appearance.PrimeTimeStart : 0;
            int endHour = ScheduleGrid.SpanOnlyPrimeTime ? this.Appearance.PrimeTimeEnd : 0;
        
            AddSpanAppointment(item, dataProvider, startHour, endHour);
        }

        /// <summary>
        /// A static method that adds a multiday span appointment to a dataProvider.
        /// </summary>
        /// <param name="item">Holds the appointment to be added.</param>
        /// <param name="dataProvider">The DataProvider.</param>
        /// <param name="startHour">The cutoff value for the start Hour of the daily appointments.</param>
        /// <param name="endHour">The cutoff value for the end Hour of the daily appointments. Use 0 for midnight.</param>
        /// <remarks>
        /// Multiday appointments are not simply single entries in the appointment list. This method takes the 
        /// information in an <see cref="IRecurringScheduleAppointment"/> and adds it to the DataProvider as 
        /// a span appointment covering more than a single day. The process properly populates the 
        /// IRecurringScheduleAppointment.DateList collection.
        /// The startHour and endHour can be used to limit the daily time span for the appointment slots. For example, the
        /// code below will limit the appointment to the PrimeTime hours as set in a ScheduleControls.Appearance object. To
        /// have no restrictions on the time slots, pass zeros for both startHour and endHour.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///     ArrayListAppointment item = new ArrayListAppointment();
        ///     item.Subject = "Blue Production Run";
        ///     item.StartTime = new DateTime(2008, 11, 12, 9, 0, 0);
        ///     item.EndTime = new DateTime(2008, 11, 14, 10, 30, 0);
        ///     item.AllDay = false;
        ///     item.LabelValue = 3;
        ///     int startHour = ScheduleGrid.SpanOnlyPrimeTime ? scheduleControl1.Appearance.PrimeTimeStart : 0;
        ///     int endHour = ScheduleGrid.SpanOnlyPrimeTime ? scheduleControl1.Appearance.PrimeTimeEnd : 0;
        ///     ScheduleControl.AddSpanAppointment(item, myArrayListDataProvider, startHour, endHour);
        /// </code>
        /// </example>  
        public static void AddSpanAppointment(IRecurringScheduleAppointment item, IRecurringScheduleDataProvider dataProvider, int startHour, int endHour)
        {
            if (item == null || dataProvider == null)
            {
                throw new ArgumentNullException();
            }

            int count = ((TimeSpan)(item.EndTime.Date - item.StartTime.Date)).Days + 1;
           
            int nextDayMaybe = endHour != 0 ? 0 : 1;
            if (item is IRecurringScheduleAppointment)
            {
                ////need a unique ID to group the appointments.
                int id = dataProvider.GetUniqueID();
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

                    item1.DateList = new RecurrenceList();
                    item1.DateList.BaseDate = item.StartTime;
                    item1.DateList.TerminalDate = item.EndTime;
                    ((IScheduleDataProvider)dataProvider).AddItem(item1);
                }
            }
        }

        private void SetupChildControls()
        {
            this.splitterMiddle.BackColor = this.Appearance.SplitterBackColor;
            this.navigationPanelSplitter.BackColor = this.Appearance.SplitterBackColor;
            this.panelSpacer.BackColor = this.Appearance.CaptionBackColor;
            this.captionPanel.BackColor = this.Appearance.CaptionBackColor;

            DateTime theDate = (this.Calendar.SelectedDates != null && this.Calendar.SelectedDates.Count > 0
                                && (this.Calendar.SelectedDates[0] > this.Calendar.BottomRightDate
                                || this.Calendar.SelectedDates[0] < this.Calendar.TopLeftDate))
                                ? this.Calendar.SelectedDates[0] : this.Calendar.DateValue;

            if (activePanel == null)
                activePanel = CreateScheduleGrid(this.Calendar, this, theDate);

            if (this.dataProvider == null)
            {
                activePanel.DefaultRowHeight = 0;
                activePanel.RowHeights[0] = 0;
            }

            activePanel.Dock = DockStyle.Fill;
            this.fillPanel.Controls.Add(activePanel);
        }

        /// <summary>
        /// Changes the currently loaded span appointments start/end time 
        /// with respect to SpanOnlyPrimeTime property at runtime.
        /// </summary>
        public void SpanOnlyPrimeTimeChanged()
        {
            GetScheduleHost().CheckSpanOnlyPrimeTime();
            GetScheduleHost().SetDataToDayPanels();
        }

        #endregion

        #region events

        /// <summary>
        /// A cancelable event that occurs before the Appointment form is displayed. You can swap the
        /// the displayed form through the events arguments.
        /// </summary>
        [Description("Occurs before the Appointment Form is displayed."),
        Category("Behavior")]
        public event ShowingAppointmentFormHandler ShowingAppointmentForm;

        /// <summary>
        /// Raises the ShowingAppointmentForm event. 
        /// </summary>
        /// <param name="e">The Event Arguments.</param>
        public virtual void RaiseShowingAppointmentForm(ShowingAppointFormEventArgs e)
        {
            OnShowingAppointmentForm(e);
        }

        /// <summary>
        /// Raises the ShowingAppointmentForm event. 
        /// </summary>
        /// <param name="e">The Event Arguments.</param>
        protected virtual void OnShowingAppointmentForm(ShowingAppointFormEventArgs e)
        {
            if (ShowingAppointmentForm != null)
            {
                ShowingAppointmentForm(this, e);
            }
        }

        /// <summary>
        /// A cancelable event raised before an IScheduleAppointment is modified.
        /// </summary>
        [Description("Occurs before an IScheduleAppointment is modified."),
        Category("Behavior")]
        public event ScheduleAppointmentChangingEventHandler ItemChanging;

        /// <summary>
        /// A notification event raised after an IScheduleAppointment is modified.
        /// </summary>
        [Description("Occurs after an IScheduleAppointment is modified."),
        Category("Behavior")]
        public event ScheduleAppointmentChangedEventHandler ItemChanged;

        /// <summary>
        /// Raises an ItemChanging event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseItemChanging(ScheduleAppointmentCancelEventArgs e)
        {
            OnItemChanging(e);
        }

        /// <summary>
        /// Raises an ItemChanged event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseItemChanged(ScheduleAppointmentEventArgs e)
        {
            OnItemChanged(e);
        }

        /// <summary>
        /// Occurs each time when the ScheduleGrid.ParseDisplayItem is called to set the display text for a ScheduleAppointment.
        /// </summary>
        [Description("Occurs each time when the ScheduleGrid.ParseDisplayItem is called to set the display text for a ScheduleAppointment."),
        Category("Behavior")]
        public event ParseDisplayItemEventHandler ParseDisplayItem;

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseParseDisplayItem(ParseDisplayItemEventArgs e)
        {
            OnParseDisplayItem(e);
        }

        /// <summary>
        /// Raises the <see cref="ParseDisplayItem"/> event.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnParseDisplayItem(ParseDisplayItemEventArgs e)
        {
            if (ParseDisplayItem != null)
            {
                ParseDisplayItem(this, e);
            }
        }

        /// <summary>
        /// Raises an ItemChanging event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnItemChanging(ScheduleAppointmentCancelEventArgs e)
        {
            ////Console.WriteLine("{0} {1}", "OnItemChanging", e);
            if (ItemChanging != null)
            {
                ItemChanging(this, e);
            }
        }

        /// <summary>
        /// Raises an ItemChanged event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnItemChanged(ScheduleAppointmentEventArgs e)
        {
            ////Console.WriteLine("{0} {1}", "OnItemChanged", e);
            if (ItemChanged != null)
            {
                ItemChanged(this, e);
            }
        }

        #endregion

        #region properties

        #region Alert stuff

        private bool enableAlerts = false;
        
        /// <summary>
        /// A property that gets or sets whether the alerts should be raised as appointment time approaches.
        /// </summary>
        /// <remarks>
        /// Setting this property to true dynamically creates the AlertWindow used by this ScheduleControl.
        /// Setting it to false will dispose of the associated AlertWindow.
        /// </remarks>
        [Browsable(true)]
        [Description("Indicates whether the alerts should be raised as appointment time approaches.")]
        [DefaultValue(false)]
        [Category("Misc")]
        public bool EnableAlerts
        {
            get
            {
                return enableAlerts;
            }

            set
            {
                if (value != enableAlerts)
                {
                    enableAlerts = value;
                    if (EnableAlertsChanged != null)
                    {
                        EnableAlertsChanged(this, EventArgs.Empty);
                    }

                    if (enableAlerts)
                    {
                        AlertForm f = this.AlertWindow;
                        f.SetDataInfo(this);
                        ////f.Show();
                        f.AdjustAlertList();
                    }
                    else if (this.AlertWindow != null)
                    {
                        this.AlertWindow.ResetAlertWindow();
                        this.AlertWindow.Dispose();
                        this.alertWindow = null;
                    }
                }
            }
        }

        // this property allows the Maximum appointment
        private int maxAppointment = 10;
        /// <summary>
        /// used to set 'how many appointment will be shown in schedule'. set minumnum value for this if you use maximum value, it will decrease the performance of schdule grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxAppointment
        {
            get
            {
                return maxAppointment;
            }
            set
            {
                maxAppointment = value;
            }
        }
        /// <summary>
        /// A notification event that is raised after ShowTime is modified. 
        /// </summary>
        [Description("Occurs after ShowTime is modified."),
        Category("Action")]
        public event EventHandler EnableAlertsChanged;
         
        private AlertForm alertWindow = null;

        /// <summary>
        /// Gets the AlertForm that displays the alerts.
        /// </summary>
        [Browsable(true)]
        [Description("The AlertForm that displays the alerts.")]
        public AlertForm AlertWindow
        {
            get 
            {
                if (alertWindow == null)
                {
                    alertWindow = new AlertForm();
                }

                return alertWindow; 
            }
           // set { alertWindow = value; }
        }

        #endregion
        internal static bool isMirrored = false;

        private static bool useMirroredFormsWithRTL = true;
       
        /// <summary>
        /// Gets/sets whether the forms used with editing appointments are mirrored when 
        /// ScheduleControl.RightToLeft is set to Yes.
        /// <para/>Default value is true.
        /// </summary>
        [Browsable(false)]
        public static bool UseMirroredFormsWithRTL
        {
            get
            {
                return useMirroredFormsWithRTL;
            }

            set 
            { 
                useMirroredFormsWithRTL = value; 
            }
        }

        private bool switchViewStyle = true;
        /// <summary>
        /// Gets/Sets whether the view style should be changed in the Control
        /// <para>Default value is true.</para>
        /// </summary>
        [DefaultValue(true), Description("Gets/Sets whether the View Style should be changed in the Control"), Category("Behavior")]
        public bool SwitchViewStyle
        {
            get
            {
                return switchViewStyle;
            }
            set
            {
                switchViewStyle = value;
            }
        }

        private static bool displayTimeSpansInEndTimeDropDown = true;
        
        /// <summary>
        /// Gets/sets whether the EndTime dropdown in the AppointmentsForm tries to
        /// parenthetically display the time span in minutes.
        /// <para/>Default value is true.
        /// </summary>
        [Browsable(false)]
        public static bool DisplayTimeSpansInEndTimeDropDown
        {
            get
            {
                return displayTimeSpansInEndTimeDropDown;
            }

            set
            {
                displayTimeSpansInEndTimeDropDown = value;
            }
        }

        private bool allowAdjustAppointmentsWithMouse = true;
        
        /// <summary>
        /// Gets/sets whether the user can change an appointment time or date with the mouse.
        /// <para/>Default value is true.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets/sets whether the user can use the mouse to change an appointment.")]
        public bool AllowAdjustAppointmentsWithMouse
        {
            get
            {
                return allowAdjustAppointmentsWithMouse;
            }

            set
            {
                allowAdjustAppointmentsWithMouse = value;
            }
        }

        private bool showRoundedCorners = false;
        
        /// <summary>
        /// Gets/sets whether the appointments are displayed with rounded corners.
        /// <para/>Default value is false.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets/sets whether the appointments are displayed with rounded corners.")]
        public bool ShowRoundedCorners
        {
            get { return showRoundedCorners; }
            set { showRoundedCorners = value; }
        }

        private bool showAllSpansInAllDayPanel = false;
        
        /// <summary>
        /// Gets/sets whether non-AllDay multiday appointments are displayed in the AllDay area of the Day, WorkWeek and CustomDay views.
        /// <para/>Default value is false.
        /// </summary>
        /// <remarks>
        /// By default, if a multi-day appointment is not marked as AllDay, this appoint will not show across multiple days in WorkWeek
        /// or CustomDay views. Instead, the appointent is shown only in the appropriate time slots during the day. Setting ShowAllSpansInAllDayPanel
        /// to true will also show non-AllDay multiday appointments across multiple days.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets/sets whether non-AllDay multiday appointments are displayed in the AllDay area of the Day, WorkWeek and CustomDay views.")]
        public bool ShowAllSpansInAllDayPanel
        {
            get { return showAllSpansInAllDayPanel; }
            set { showAllSpansInAllDayPanel = value; }
        }

        private bool showMultiDayAppointmentsAsSpans = true;
        
        /// <summary>
        /// Gets/sets whether the multiday appointments are displayed across days in Month, Day, WorkWeek and CustomDay views.
        /// <para/>Default value is true.
        /// </summary>
        /// <remarks>
        /// When this property is set to true (the default setting) and the <see cref="DataSource"/> implements <see cref="IRecurringScheduleDataProvider"/>,
        /// then an AllDay appointment that spans more than one day will display as a single panel across the multiple-day span of the appointment
        /// for ScheduleViewTypes Day, CustomWeek, WorkWeek and Month (but not Week). When an appointment is shown as a multiday span,
        /// the user will not be able to use the mouse to change the day span or drag it to a new location. The only way to edit such 
        /// spans is through the Edit dialog which normally appears on a double left-click or from the right-click context menu.
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets/sets whether the multiday appointments are displayed across days in Month, Day, WorkWeek and CustomDay views.")]
        public bool ShowMultiDayAppointmentsAsSpans
        {
            get { return showMultiDayAppointmentsAsSpans; }
            set { showMultiDayAppointmentsAsSpans = value; }
        }

        /// <summary>
        /// Gets or sets the Label Control that serves as the caption 
        /// at the top of the ScheduleControl.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Label HeaderLabel
        {
            get
            {
                return this.headerLabel;
            }

            set
            {
                this.headerLabel = value;
            }
        }

        /// <summary>
        /// A property that gets or sets the panel where you can place additional controls and have them appear adjacent to the ScheduleControl.
        /// </summary>
        [Browsable(true)]
        [Description("Panel to host other controls.")]
        public Panel NavigationPanel
        {
            get { return this.navigationPanel; }
        }

        private bool navigationPanelFillWithCalendar = false;

        /// <summary>
        /// Gets or sets whether the Calendar occupies the entire <see cref="NavigationPanel"/> 
        /// and sizes dynamically as the ScheduleControl is sized.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets whether NavigationPanel is filled with the Calendar.")]
        public bool NavigationPanelFillWithCalendar
        {
            get
            {
                return navigationPanelFillWithCalendar;
            }

            set
            {
                if (value != navigationPanelFillWithCalendar)
                {
                    navigationPanelFillWithCalendar = value;
                    NavigationCalendar cal = Calendar;
                    object o = cal.Dock;
                    if (navigationPanelFillWithCalendar)
                    {
                        navigationPanelSplitter.Dock = DockStyle.Bottom;
                        panelFixedTop.Dock = DockStyle.Fill;
                    }
                    else
                    {
                        panelFixedTop.Dock = DockStyle.Top;
                        navigationPanelSplitter.Dock = DockStyle.Top;
                   }
                }
            }
        }

        private CalendarNavigationPanelPosition navigationPanelPosition = CalendarNavigationPanelPosition.Left;

        /// <summary>
        /// Indicates the location of the <see cref="NavigationPanel"/>.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates where the NavigationPanel appears.")]
        public CalendarNavigationPanelPosition NavigationPanelPosition
        {
            get
            {
                return navigationPanelPosition;
            }

            set
            {
                if (value != navigationPanelPosition)
                {
                    SetNavigationPanelPosition(value);
                    navigationPanelPosition = value;
                }
            }
        }

        private bool iso8601CalenderFormat = false;

        /// <summary>
        /// Gets or Sets a value indicating whether ISO 8601 calender format is applied or not.
        /// </summary>
        [
        Browsable(false)        
        ]        
        public bool ISO8601CalenderFormat
        {
            get
            {
                return this.iso8601CalenderFormat;
            }
            set
            {
                if (this.iso8601CalenderFormat != value)
                {
                    this.iso8601CalenderFormat = value;
                    this.Appearance.ISO8601CalenderFormat = value;
                    this.NavigationPanel.Invalidate(true);
                }
            }
        }

        private ScheduleAppearance appearance;

        /// <summary>
        /// A property that gets or sets a <see cref="ScheduleAppearance"/> object that controls
        /// the visual properties of the ScheduleControl.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Browsable(true)]
        [Description("Provides the appearance properties for this ScheduleControl.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScheduleAppearance Appearance
        {
            get
            {
                if (this.appearance == null)
                {
                    this.appearance = new ScheduleAppearance(this);
                }

                return appearance;
            }

            set
            {
                appearance = value;
            }
        }

        private IScheduleDataProvider dataProvider = null;

        /// <summary>
        /// A property of type <see cref="IScheduleDataProvider"/> that gets or sets data source for this ScheduleControl.
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
                    dataProvider = value;
                    if (this.activePanel != null)
                    {
                        this.activePanel.DataSource = dataProvider;
                        if (dataProvider != null && this.activePanel.ScheduleType == ScheduleViewType.Month)
                        {
                            if (this.isMetro)
                                this.activePanel.RowHeights[0] = 35;
                            else
                                this.activePanel.RowHeights[0] = 21;  ////make sure row not hidden initially defect 13519
                        }
                    }
                }
            }
        }

        private ScheduleViewType scheduleType = ScheduleViewType.Day;

        /// <summary>
        /// A property that gets or sets whether a daily, weekly or monthly schedule is displayed.
        /// </summary>
        [Browsable(false)]
        [Description("Gets/sets the whether a daily, weekly or monthly schedule is displayed.")]
        [DefaultValue(ScheduleViewType.Day)]
        public ScheduleViewType ScheduleType
        {
            get
            {
                return scheduleType;
            }

            set
            {
                scheduleType = value;
                this.SetupChildControls();
            }
        }

        private CultureInfo culture = null;

        /// <summary>
        /// A property that gets / sets the culture used for the date formatting.
        /// </summary>
        /// <remarks>Defaults to the CultureInfo.InvariantCulture setting.</remarks>
        [Browsable(true)]
        [Description("Gets/sets the culture used for the date formatting.")]
        public CultureInfo Culture
        {
            get
            {
                if (this.culture == null)
                {
                    this.culture = CultureInfo.InvariantCulture;
                }

                return this.culture;
            }

            set
            {
                if (value != this.culture)
                {
                    this.culture = value;
                    this.Calendar.CalenderGrid.TableStyle.CultureInfo = this.culture;

                    ApplyCultureToAppearance(this.culture, this.Appearance);

                    if (this.activePanel != null)
                    {                        
                        this.activePanel.SetUpDayOfWeekStrings();
                        this.activePanel.SwitchTo(this.ScheduleType, true);
                        this.activePanel.Refresh();
                    }
                    if (CultureChanged != null)
                    {
                        CultureChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after Culture is modified. 
        /// </summary>
        /// <remarks>
        /// You can use this event to move Culture settings into the ScheduleControl's Appearance
        /// object to control how the Culture settings affect the look of the ScheduleControl.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// void scheduleControl1_CultureChanged(object sender, EventArgs e)
        /// {
        ///     ScheduleControl sc = sender as ScheduleControl;
        ///     if (sc != null)
        ///     {
        ///         sc.Appearance.WeekHeaderFormat = sc.Culture.DateTimeFormat.MonthDayPattern;
        ///     }
        /// }
        /// </code>
        /// </example>  
        [Description("Occurs when the culture has been modified."),
        Category("Behavior")]
        public event EventHandler CultureChanged;


        /// <summary>
        /// This method is called by the ScheduleControl when the ScheduleControl.Culture property is set to
        /// move the specific culture settings to the Appearance object.
        /// </summary>
        /// <param name="currentCultureInfo">The new Culture.</param>
        /// <param name="currentAppearance">The Appearance object to be set.</param>
        /// <remarks>
        /// Override this method to control how your Culture settings will affect the Appearance. The default settings
        /// are as below
        /// <code lang="C#">
        /// currentAppearance.WorkWeekHeaderFormat = currentCultureInfo.DateTimeFormat.ShortDatePattern;
        /// currentAppearance.WeekHeaderFormat = currentCultureInfo.DateTimeFormat.MonthDayPattern;
        /// currentAppearance.WeekMonthFullFormat = currentCultureInfo.DateTimeFormat.LongDatePattern;
        /// </code>
        /// <code lang ="VB">
        /// currentAppearance.WorkWeekHeaderFormat = currentCultureInfo.DateTimeFormat.ShortDatePattern
        /// currentAppearance.WeekHeaderFormat = currentCultureInfo.DateTimeFormat.MonthDayPattern
        /// currentAppearance.WeekMonthFullFormat = currentCultureInfo.DateTimeFormat.LongDatePattern
        /// </code>
        /// </remarks>
        public virtual void ApplyCultureToAppearance(CultureInfo currentCultureInfo, ScheduleAppearance currentAppearance)
        {
            //override this method to move culture settings to appearance object
            currentAppearance.WorkWeekHeaderFormat = currentCultureInfo.DateTimeFormat.ShortDatePattern;
            currentAppearance.WeekHeaderFormat = currentCultureInfo.DateTimeFormat.MonthDayPattern;
            currentAppearance.WeekMonthFullFormat = currentCultureInfo.DateTimeFormat.LongDatePattern;
        }

        #endregion

        #region ContextMenu support
        //// Create and initialize a ParentBarItem.

        #region windows forms version
        ////      private ContextMenu popupMenu;
        //        protected virtual void SetupPopupMenu()
        //        {
        //        popupMenu = new ContextMenu();
        //        MenuItem menu1 = new MenuItem();
        //        menu1.Text = "New App&ointment";
        //        menu1.Click += new EventHandler(menu1_Selected);
        //        
        //	        MenuItem menu2 = new MenuItem();
        //        menu2.Text = "New AllDay &Event";
        //        menu2.Click += new EventHandler(menu2_Selected);
        //	         
        //	        popupMenu.MenuItems.AddRange(new MenuItem[] { menu1, menu2});
        //		        
        //        //this.Grid.ContextMenu = menu;
        //	       }
		#endregion

		/// <summary>
		/// Subscribe to this event to set up your own Context menu for the ScheduleControl.
		/// </summary>
		/// <remarks>
		/// This menu allows you to control the Context menu that is displayed when your user
		/// right clicks the DayContainerPanel that hosts the items in the ScheduleControl. Your 
		/// event handler code should set the Context menu for the ScheduleGrid returned by
		/// ScheduleControl.GetScheduleHost. After doing so, your code should set e.Cancel = true.
		/// Otherwise, the default implementation of a Windows Forms ContextMenu will override your
		/// settings.
		/// </remarks>
        [Description("To be subscribed to set up own ContextMenu for the ScheduleControl."),
        Category("Grid")]
		public event CancelEventHandler SetupContextMenu;

		/// <summary>
		/// Sets up the context menu associated with the ScheduleControl.
		/// </summary>
		/// <remarks>Override this method to create your own context menus.</remarks>
		public virtual void OnSetupContextMenu()
		{
			CancelEventArgs e = new CancelEventArgs(false);
			if (SetupContextMenu != null)
			{
				SetupContextMenu(this, e);
			}

            if (!e.Cancel && this.GetScheduleHost() != null)
            {
                if (this.Appearance.VisualStyle == GridVisualStyles.Metro)
                    this.GetScheduleHost().ContextMenuStrip = MetroContextMenu();
                else
                    this.GetScheduleHost().ContextMenu = DefaultWindowsFormsContextMenu();
            }
		}

        private const int _New_Item = 9;
        private const int _New_All_Day_item = 10;
        private const int _Edit_Item = 11;
        private const int _Delete_Item = 12;
        private const int _Day = 13;
        private const int _Work_Week = 14;
        private const int _Week = 15;
        private const int _Month = 16;
		
		/// <summary>
		/// Returns a Metro ContextMenu implementing standard set of menu items for metro theme
		/// </summary>
		/// <returns>A ContextMenu.</returns>
		/// <remarks>
		/// If you use this default implementation of a ContextMenu, you can dynamically
		/// cancel the execution of a menu selection by subscribing to the 
		/// ScheduleControl.ScheduleAppointmentClick event and setting e.Cancel = true when 
		/// e.ClickType is RightClick.
		/// </remarks>
        public ContextMenuStrip MetroContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_New_Item], (Image)ScheduleGrid.GetBitmap(@"AddNew.png"), new EventHandler(newItemClick)));
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_New_All_Day_item], null, new EventHandler(newAllDayItemClick)));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Edit_Item], (Image)ScheduleGrid.GetBitmap("Edit.png"), new EventHandler(editItemClick)));
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Delete_Item], (Image)ScheduleGrid.GetBitmap("Delete.png"), new EventHandler(deleteItemClick)));
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Day], (Image)ScheduleGrid.GetBitmap("Day.png"), new EventHandler(dayItemClick)));
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Work_Week], (Image)ScheduleGrid.GetBitmap("Workweek.png"), new EventHandler(workWeekItemClick)));
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Week], (Image)ScheduleGrid.GetBitmap("Week.png"), new EventHandler(weekItemClick)));
            menu.Items.Add(new ToolStripMenuItem(ScheduleGrid.DisplayStrings[_Month], (Image)ScheduleGrid.GetBitmap("Month.png"), new EventHandler(monthItemClick)));
            menu.ShowImageMargin = true;
            menu.DropShadowEnabled = false;
            menu.Renderer = new ContextMenuRenderer();
            menu.BackColor = Color.White;
            return menu;
        }

        /// <summary>
        /// Returns a default ContextMenu implementing standard set of menu items.
        /// </summary>
        /// <returns>A ContextMenu.</returns>
        /// <remarks>
        /// If you use this default implementation of a ContextMenu, you can dynamically
        /// cancel the execution of a menu selection by subscribing to the 
        /// ScheduleControl.ScheduleAppointmentClick event and setting e.Cancel = true when 
        /// e.ClickType is RightClick.
        /// </remarks>
        public ContextMenu DefaultWindowsFormsContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem newItem = new MenuItem(ScheduleGrid.DisplayStrings[_New_Item], new EventHandler(newItemClick));
            MenuItem newAllDayItem = new MenuItem(ScheduleGrid.DisplayStrings[_New_All_Day_item], new EventHandler(newAllDayItemClick));
            MenuItem sep = new MenuItem("-");
            MenuItem editItem = new MenuItem(ScheduleGrid.DisplayStrings[_Edit_Item], new EventHandler(editItemClick));
            MenuItem deleteItem = new MenuItem(ScheduleGrid.DisplayStrings[_Delete_Item], new EventHandler(deleteItemClick));
            MenuItem sep1 = new MenuItem("-");
            MenuItem dayItem = new MenuItem(ScheduleGrid.DisplayStrings[_Day], new EventHandler(dayItemClick));
            MenuItem workWeekItem = new MenuItem(ScheduleGrid.DisplayStrings[_Work_Week], new EventHandler(workWeekItemClick));
            MenuItem weekItem = new MenuItem(ScheduleGrid.DisplayStrings[_Week], new EventHandler(weekItemClick));
            MenuItem monthItem = new MenuItem(ScheduleGrid.DisplayStrings[_Month], new EventHandler(monthItemClick));
			
			menu.MenuItems.AddRange(new MenuItem[]
            {
                newItem, newAllDayItem, sep, editItem, deleteItem, sep1, dayItem, workWeekItem, weekItem, monthItem
            });

			return menu;
		}
		
		#region public event tasks code

		/// <summary>
		/// A method that switches the display to the specified ScheduleViewType.
		/// </summary>
        /// <param name="t">The requested ScheduleViewType.</param>
		public void PerformSwitchToScheduleViewTypeClick(ScheduleViewType t)
		{
            DateTime dt = this.GetScheduleHost().GetDisplayDateUnderClick();
            if (dt.Year == 1)
            {
                if (this.GetScheduleHost().Calendar.SelectedDates.Count == 0)
                    dt = this.GetScheduleHost().Calendar.DateValue;
                else
                    dt = this.GetScheduleHost().Calendar.SelectedDates[0];
            }
            if (dt.Year > 1 )
            {
                this.GetScheduleHost().Calendar.DateValue = dt;
                this.GetScheduleHost().Calendar.SelectedDates[0] = dt;
                //this.GetScheduleHost().Calendar.DateValue = this.GetScheduleHost().Calendar.SelectedDates[0];
            }

			this.GetScheduleHost().SwitchTo(t, true);
		}

        /// <summary>
        /// Switches the display to the specified date with ScheduleViewType.Day.
        /// </summary>
        /// <param name="date">The requested date.</param>
        public void SwitchToScheduleViewTypeDay(DateTime date)
        {
            this.GetScheduleHost().Calendar.SelectedDates[0] = date.Date;
            this.GetScheduleHost().Calendar.DateValue = date.Date;
            this.GetScheduleHost().SwitchTo(ScheduleViewType.Day, true);
        }

////		/// <summary>
//		/// Switches the display to the a month ScheduleViewType
//		/// </summary>
//		public void PerformMonthItemClick()
//		{
//			this.GetScheduleHost().Calendar.SelectedDates[0] = this.GetScheduleHost().GetDisplayDateUnderClick();
//			this.GetScheduleHost().Calendar.DateValue = this.GetScheduleHost().Calendar.SelectedDates[0];
//			this.GetScheduleHost().SwitchTo(ScheduleViewType.Month, true);
//		}

		/// <summary>
		/// A method that displays a dialog allowing you to delete an IScheduleAppointment.
		/// </summary>
		public void PerformDeleteItemClick()
		{
			this.GetScheduleHost().DeleteAppointment();
		}

		/// <summary>
		/// A method that displays a dialog allowing you to edit an IScheduleAppointment.
		/// </summary>
		public void PerformEditItemClick()
		{
			this.GetScheduleHost().EditItemAtClick();
		}

		/// <summary>
		/// A method that displays a dialog allowing you to enter a new IScheduleAppointment.
		/// </summary>
		public void PerformNewItemClick()
		{
			this.GetScheduleHost().DoAppointment(false);
		}

		/// <summary>
		/// A method that displays a Dialog allowing you to enter a new AllDay IScheduleAppointment.
		/// </summary>
		public void PerformNewAllDayItemClick()
		{
			this.GetScheduleHost().DoAppointment(true);
		}

		/// <summary>
		/// Gets whether an ISchedule item has input focus (i.e., has been clicked)
		/// </summary>
		/// <remarks>
		/// You can use this property to determine whether menu items should be enabled.
		/// </remarks>
        [Browsable(false)]
        [Description("Gets whether an IScheduleAppointment has input focus.")]
		public bool ItemSelected
		{
            get
            {
                if(this.GetScheduleHost() != null)
                    return this.GetScheduleHost().ItemSelected;
                return false;
            }
		}

		#endregion

		#region private menu event click handlers

		private void dayItemClick(object sender, EventArgs e)
		{
            if (!this.GetScheduleHost().contextMenuCancelled && this.SwitchViewStyle)
			{
				PerformSwitchToScheduleViewTypeClick(ScheduleViewType.Day);
			}
		}

		private void workWeekItemClick(object sender, EventArgs e)
		{
			if (!this.GetScheduleHost().contextMenuCancelled && this.SwitchViewStyle)
			{
				PerformSwitchToScheduleViewTypeClick(ScheduleViewType.WorkWeek);
			}
		}

		private void weekItemClick(object sender, EventArgs e)
		{
            if (!this.GetScheduleHost().contextMenuCancelled && this.SwitchViewStyle)
			{
				PerformSwitchToScheduleViewTypeClick(ScheduleViewType.Week);
			}
		}

		private void monthItemClick(object sender, EventArgs e)
		{
            if (!this.GetScheduleHost().contextMenuCancelled && this.SwitchViewStyle)
			{
				PerformSwitchToScheduleViewTypeClick(ScheduleViewType.Month);
			}
		}

		private void newAllDayItemClick(object sender, EventArgs e)
		{
			if (!this.GetScheduleHost().contextMenuCancelled)
			{
				PerformNewAllDayItemClick();
			}
		}

		private void newItemClick(object sender, EventArgs e)
		{
			if (!this.GetScheduleHost().contextMenuCancelled)
			{
				PerformNewItemClick();
			}
		}

		private void editItemClick(object sender, EventArgs e)
		{
			if (!this.GetScheduleHost().contextMenuCancelled)
			{
				PerformEditItemClick();
			}
		}

		private void deleteItemClick(object sender, EventArgs e)
		{
			if (!this.GetScheduleHost().contextMenuCancelled)
			{
				PerformDeleteItemClick();
			}
		}

		#endregion
	 
		#endregion

		#region freeze drawing code
		private const int WM_SETREDRAW = 0xB;
		private int paintFrozen = 0;

		[DllImport("User32")]
		private static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		internal bool FreezePainting 
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

		#region Click event

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
		#endregion

        #region AdjustingAppointmentWithMouse event

        /// <summary>
        /// An event that lets you cancel appointment by appointment whether an appointment can be adjusted using the mouse.
        /// </summary>
        [Description("Occurs when an appointment is adjusted using the mouse."),
        Category("Action")]
        public event AdjustingAppointmentWithMouseEventHandler AdjustingAppointmentWithMouse;

        /// <summary>
        /// Raises a AdjustingAppointmentWithMouse event that lets you cancel appointment by appointment whether an appointment can be adjusted using the mouse.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public virtual void OnAdjustingAppointmentWithMouse(AdjustingAppointmentMouseWithEventArgs e)
        {
            if (AdjustingAppointmentWithMouse != null)
            {
                AdjustingAppointmentWithMouse(this, e);
            }
        }
        #endregion

        #region ScheduleGridCreated event

        /// <summary>
        /// An event that lets you either use a derived ScheduleGridControl or to subscribe to events on the ScheduleGridControl.
        /// </summary>
        /// <remarks>
        /// As you change ScheduleViewTypes within the ScheduleControl, new ScheduleGridControls are created to reflect the
        /// new view type, say switching form a Month view to a Day view. This event is raised after the new ScheduleGrid
        /// is created, allowing you to either swap it out for a derived ScheduleGrid, or subscribe to events on the ScheduleGrid.
        /// <para>To catch the initial creation of a ScheduleGrid with this event, you should make sure you subscribe to the event
        /// before you set the <see cref="ScheduleControl.ScheduleType"/> property.</para>
        /// </remarks>
        [Description("To be subscribed to use a derived ScheduleGrid or to subscribe to events on it."),
        Category("Grid")]
        public event ScheduleGridCreatedEventHandler ScheduleGridCreated;

        /// <summary>
        /// Raises a ScheduleGridCreated event that lets you either use a derived ScheduleGridControl or to subscribe to events on the ScheduleGridControl.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public virtual void OnScheduleGridCreated(ScheduleGridCreatedEventArgs e)
        {
            if (ScheduleGridCreated != null)
            {
                ScheduleGridCreated(this, e);
            }
        }
        #endregion
    }

    #region class FillPanel
    /// <summary>
	/// Layout panel
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
    [ToolboxItem(false)]
	public class FillPanel : Panel
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public FillPanel()
		{
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles	.AllPaintingInWmPaint, true);
		}
    }
    #endregion

    #region enum ScheduleViewType
    
    /// <summary>
	/// Specifies the possible display types you can see in a ScheduleControl.
	/// </summary>
	public enum ScheduleViewType
	{
		/// <summary>
		/// Displays a single day of IScheduleAppointment objects in the ScheduleControl.
		/// </summary>
		Day,
		
        /// <summary>
		/// Displays a five day workweek of IScheduleAppointment objects in the ScheduleControl.
		/// </summary>
		WorkWeek,
		
        /// <summary>
		/// Displays a seven day week of IScheduleAppointment objects in the ScheduleControl.
		/// </summary>
		Week,
		
        /// <summary>
		/// Displays a month of IScheduleAppointment objects in the ScheduleControl.
		/// </summary>
		Month,
		
        /// <summary>
		/// Displays a variable number of days of IScheduleAppointment objects in the ScheduleControl.
		/// </summary>
		CustomWeek
    }

    #endregion

    /// <summary>
    /// Renderer is override for metro ContextMenu.
    /// </summary>
    public class ContextMenuRenderer : ToolStripProfessionalRenderer
    {
        /// <summary>
        /// base method 
        /// </summary>
        public ContextMenuRenderer()
        {
        }
        /// <summary>
        /// base ToolStripBorder
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);
        }
        /// <summary>
        /// Is triggered when the margin for the image is rendered.
        /// </summary>
        /// <param name="e">ToolStripRenderEventArgs</param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Rectangle marginRect = e.AffectedBounds;
            using (SolidBrush backBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(backBrush, marginRect);
            }
        }

    }

    internal class OfficeThemedButton : ThemedScrollButton
    {
        private Syncfusion.Windows.Forms.IVisualStylesDrawing drawing = null;

        internal Syncfusion.Windows.Forms.IVisualStylesDrawing Drawing
        {
            get { return drawing; }
            set { drawing = value; }
        }

        protected override void DrawStyledControl(Graphics g, ButtonState buttonState, CheckState checkState)
        {
            if (drawing == null)
            {
                base.DrawStyledControl(g, buttonState, checkState);
            }
            else
            {
                drawing.DrawPushButtonStyle(g, ClientRectangle, buttonState);
            }
        }

        protected override void DrawControlText(Graphics g)
        {
            this.DrawText = drawing != null;
            base.DrawControlText(g);
        }
    }
}
