//-------------------------------------------------------------------------------------------------
// <copyright file="AlertForm.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Schedule;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Displays appointment reminder alerts.
    /// </summary>
    public class AlertForm : Form
    {
        ////holds the list of potential alert items
        private IScheduleAppointmentList scheduleDataList = null;
        ////the data provide for the ScheduleControl
        private IScheduleDataProvider dataProvider = null;
        ////the ScheduleControl
        ScheduleControl schedule = null;
        ////the list of items which have a pending alert
        private ArrayList list = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AlertForm()
        {
            InitializeComponent();
        }

        #region generated code
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.labelSubject = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.Grid = new Syncfusion.Windows.Forms.Grid.GridControl();
			this.buttonDismissAll = new System.Windows.Forms.Button();
			this.buttonDismiss = new System.Windows.Forms.Button();
			this.buttonSnooze = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
			this.SuspendLayout();
			// 
			// labelSubject
			// 
			this.labelSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelSubject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSubject.Location = new System.Drawing.Point(29, 24);
			this.labelSubject.Name = "labelSubject";
			this.labelSubject.Size = new System.Drawing.Size(411, 23);
			this.labelSubject.TabIndex = 0;
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(29, 53);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(411, 23);
			this.labelTime.TabIndex = 1;
			// 
			// Grid
			// 
			this.Grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.Grid.Location = new System.Drawing.Point(32, 91);
			this.Grid.Name = "Grid";
			this.Grid.SerializeCellsBehavior = Syncfusion.Windows.Forms.Grid.GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
			this.Grid.Size = new System.Drawing.Size(412, 163);
			this.Grid.SmartSizeBox = false;
			this.Grid.TabIndex = 2;
			this.Grid.Text = "gridControl1";
			// 
			// buttonDismissAll
			// 
			this.buttonDismissAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDismissAll.Location = new System.Drawing.Point(41, 274);
			this.buttonDismissAll.Name = "buttonDismissAll";
			this.buttonDismissAll.TabIndex = 3;
			this.buttonDismissAll.Text = "Dismiss All";
			// 
			// buttonDismiss
			// 
			this.buttonDismiss.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDismiss.Location = new System.Drawing.Point(369, 274);
			this.buttonDismiss.Name = "buttonDismiss";
			this.buttonDismiss.TabIndex = 4;
			this.buttonDismiss.Text = "Dismiss";
			// 
			// buttonSnooze
			// 
			this.buttonSnooze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSnooze.Location = new System.Drawing.Point(273, 274);
			this.buttonSnooze.Name = "buttonSnooze";
			this.buttonSnooze.TabIndex = 5;
			this.buttonSnooze.Text = "Snooze";
			// 
			// AlertForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(476, 318);
			this.Controls.Add(this.buttonSnooze);
			this.Controls.Add(this.buttonDismiss);
			this.Controls.Add(this.buttonDismissAll);
			this.Controls.Add(this.Grid);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.labelSubject);
			this.Name = "AlertForm";
			this.Text = "Alert";
			this.Load += new System.EventHandler(this.AlertForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
			this.ResumeLayout(false);

		}

        #endregion

        #endregion

        #region initialization

        ////sets up the grid and buttons
        private void AlertForm_Load(object sender, EventArgs e)
        {
            if (this.scheduleDataList == null)
            {
                throw new ArgumentNullException("AlertForm.schedule", "Use SetScheduleControl to provide the ScheduleControl to the Alert Window.");
            }

            this.Grid.QueryCellInfo += new GridQueryCellInfoEventHandler(Grid_QueryCellInfo);
            this.Grid.QueryRowCount += new GridRowColCountEventHandler(Grid_QueryRowCount);
            this.Grid.QueryColWidth += new GridRowColSizeEventHandler(Grid_QueryColWidth);
            this.Grid.CurrentCellMoved += new GridCurrentCellMovedEventHandler(Grid_CurrentCellMoved);
            this.Grid.ColWidths[0] = 0;
            this.Grid.ColCount = 2;
            this.Grid[0, 1].Text = AlertStrings[_subject];
            this.Grid[0, 2].Text = AlertStrings[_time];
            this.Grid.ListBoxSelectionMode = SelectionMode.One;
            this.Grid.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(Grid_CurrentCellActivating);
            this.Grid.BackColor = SystemColors.Window;
            this.Grid.Properties.BackgroundColor = SystemColors.Window;
            this.Grid.DefaultGridBorderStyle = GridBorderStyle.None;
            this.Grid.BorderStyle = BorderStyle.FixedSingle;
            this.Grid.SmoothControlResize = false;
            this.Grid.ControllerOptions = GridControllerOptions.SelectCells;
            this.Grid.ResetVolatileData();

            this.buttonDismissAll.Click += new EventHandler(button1_Click);
            this.buttonDismiss.Click += new EventHandler(button2_Click);
            this.buttonSnooze.Click += new System.EventHandler(this.button3_Click);
        }

        private bool inReset = false;

        /// <summary>
        /// Clears all events and data used by this AlertWindow and closes it.
        /// </summary>
        public void ResetAlertWindow()
        {
            inReset = true;
            timerCheckMode = false;
            snoozing = false;
            if (alertTimer != null)
            {
                alertTimer.Enabled = false;
                alertTimer.Tick -= new EventHandler(alertTimer_Tick);
                alertTimer.Dispose();
                alertTimer = null;
            }

            this.Grid.QueryCellInfo -= new GridQueryCellInfoEventHandler(Grid_QueryCellInfo);
            this.Grid.QueryRowCount -= new GridRowColCountEventHandler(Grid_QueryRowCount);
            this.Grid.QueryColWidth -= new GridRowColSizeEventHandler(Grid_QueryColWidth);
            this.Grid.CurrentCellMoved -= new GridCurrentCellMovedEventHandler(Grid_CurrentCellMoved);

            this.buttonDismissAll.Click -= new EventHandler(button1_Click);
            this.buttonDismiss.Click -= new EventHandler(button2_Click);
            this.buttonSnooze.Click -= new System.EventHandler(this.button3_Click);

            this.list.Clear();
            this.Visible = false;
            this.Close();
            inReset = false;
        }

        /// <summary>
        /// Informs the form of the active IScheduleAppointmentList and the IDataProvider. 
        /// </summary>
        /// <param name="schedule">The ScheduleControl.</param>
        public void SetDataInfo(ScheduleControl schedule) ////IScheduleAppointmentList scheduleDataList, IScheduleDataProvider dataProvider)
        {
            if(schedule.GetScheduleHost() != null)
                this.scheduleDataList = schedule.GetScheduleHost().scheduleDataList;
            this.dataProvider = schedule.DataSource;
            this.schedule = schedule;
        }
        #endregion

        #region Timer support code

        /// <summary>
        /// Creates a list of items that require alerts at the time of thge call.
        /// </summary>
        /// <remarks>
        /// This method is called when ScheduleControl.EnableAlerts is set true.
        /// </remarks>
        public void AdjustAlertList()
        {
            if (list == null)
            {
                list = new ArrayList();
            }

            if (alertTimer == null)
            {
                alertTimer = new Timer();
                ////1 minute - interval set to update displayed time in a visible alertwindow
                alertTimer.Interval = 60 * 1000; 
                alertTimer.Tick += new EventHandler(alertTimer_Tick);
                alertTimer.Enabled = true;
            }

            DateTime now = DateTime.Now;
            if (scheduleDataList != null)
            {
                foreach (IScheduleAppointment item in scheduleDataList)
                {
                    if (item.Reminder && !list.Contains(item))
                    {
                        string s = this.dataProvider.GetReminders()[item.ReminderValue].DisplayMember;
                        DateTime time = now.AddMinutes(GetMinutesFromDisplayValue(s) + checkTimerIntervalInMinutes);
                        ////Console.Write("{0}  {1}  {2}", item.Subject, item.StartTime, time);
                        if (item.StartTime.CompareTo(time) <= 0)
                        {
                            list.Add(item);
                            ////Console.Write(" added");
                        }
                        ////Console.WriteLine();
                    }
                }
            }

            if (list.Count > 0 && !snoozing && !this.Visible)
            {
                this.Visible = true;
                timerCheckMode = false;
            }

            if (this.Grid.CurrentCell.RowIndex <= 0)
            {
                this.Grid.Focus();
                this.Grid.CurrentCell.MoveTo(1, 1);
                this.Grid.ForceCurrentCellMoveTo = true;
            }

            if (list.Count == 0)
            {
                alertTimer.Enabled = false;
                alertTimer.Interval = checkTimerIntervalInMinutes * 1000 * 60;
                timerCheckMode = true;
                alertTimer.Enabled = true;
            }
            else if (list.Count > 0)
            {
                timerCheckMode = false;
            }
        }
        
        private bool timerCheckMode = false;
        void alertTimer_Tick(object sender, EventArgs e)
        {
            if (inReset)
            {
                return;
            }

            if (timerCheckMode)
            {
                AdjustAlertList();
            }
            else
            {
                if (snoozing)
                {
                    snoozing = false;
                }

                if (this.Visible && this.WindowState != FormWindowState.Minimized)
                {
                    this.Grid.RefreshRange(GridRangeInfo.Cols(2, 2));
                }
                else
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Visible = true;
                    this.alertTimer.Interval = 500; // 1000 * 60; //flash rate
                    FlashMe();
                }
            }
        }
        #endregion

        #region properties and fields

        /// <summary>
        /// GridControl that lists the alert items.
        /// </summary>
        /// <remarks>
        /// You can directly access this GridControl to change its look. Here is some code that
        /// changes its look.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///     this.ScheduleControl1.AlertWindow.BackColor = Color.FromArgb(240, 240, 255);
        ///     this.ScheduleControl1.AlertWindow.Grid.TableStyle.BackColor = Color.FromArgb(100, 240, 240, 255);
        ///     this.ScheduleControl1.AlertWindow.Grid.Properties.BackgroundColor = Color.FromArgb(100, Color.Blue);
        ///     Syncfusion.Windows.Forms.Grid.GridStyleInfo headerStyle =
        ///                this.ScheduleControl1.AlertWindow.Grid.BaseStylesMap["Header"].StyleInfo;
        ///     headerStyle.Interior = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, Color.Blue, Color.White);
        ///     this.ScheduleControl1.AlertWindow.Grid.Refresh();
        /// </code>
        /// <code lang="VB">
        ///     Me.ScheduleControl1.AlertWindow.BackColor = Color.FromArgb(240, 240, 255)
        ///     Me.ScheduleControl1.AlertWindow.Grid.TableStyle.BackColor = Color.FromArgb(100, 240, 240, 255)
        ///     Me.ScheduleControl1.AlertWindow.Grid.Properties.BackgroundColor = Color.FromArgb(100, Color.Blue)
        ///     Dim headerStyle As Syncfusion.Windows.Forms.Grid.GridStyleInfo = _
        ///                             Me.ScheduleControl1.AlertWindow.Grid.BaseStylesMap("Header").StyleInfo
        ///     headerStyle.Interior = New Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, Color.Blue, Color.White)
        ///     Me.ScheduleControl1.AlertWindow.Grid.Refresh()
        /// </code>
        /// </example>
        public Syncfusion.Windows.Forms.Grid.GridControl Grid;

        /// <summary>
        /// The Label that displays the Subject of the item.
        /// </summary>
        public System.Windows.Forms.Label labelSubject;

        /// <summary>
        /// The Label that displays the StartTime of the item.
        /// </summary>
        public System.Windows.Forms.Label labelTime;

        /// <summary>
        /// The Button that dismisses all the listed items.
        /// </summary>
        public System.Windows.Forms.Button buttonDismissAll;

        /// <summary>
        /// The Button that removes the AlertWindow and redisplays it in a specified period, SnoozeTime.
        /// </summary>
        public System.Windows.Forms.Button buttonSnooze;

        /// <summary>
        /// The Button that dismisses the selected item.
        /// </summary>
        public System.Windows.Forms.Button buttonDismiss;

        /// <summary>
        /// Gets the i-th IScheduleAppointment in the alert list.
        /// </summary>
        /// <param name="i">An integer between 0 and <see cref="Count"/>.</param>
        /// <returns>An IScheduleAppointment.</returns>
        /// <remarks>
        /// If the indexer is outside the valid number of items in the alert list, null is returned.
        /// </remarks>
        public IScheduleAppointment this[int i]
        {
            get
            {
                if (list == null || i < 0 || i >= list.Count)
                {
                    return null;
                }

                return list[i] as IScheduleAppointment;
            }
        }

        /// <summary>
        /// Gets the number of items in the current alert list.
        /// </summary>
        public int Count
        {
            get
            {
                return (list == null) ? 0 : list.Count;
            }
        }

        // timer used for all alerts - initialized in AdjustAlertList
        private Timer alertTimer = null;

        private string[] alertStrings = new string[]
            {
                "minute(s)",
                "hour(s)",
                " ago",
                " from now",
                "Subject",
                "Time"
            };

        /// <summary>
        /// Gets or sets the strings used as part of the alert window.
        /// </summary>
        public string[] AlertStrings
        {
            get { return alertStrings; }
            set { alertStrings = value; }
        }

        private int _minute = 0;
        private int _hour = 1;
        private int _ago = 2;
        private int _fromNow = 3;
        private int _subject = 4;
        private int _time = 5;

        private int checkTimerIntervalInMinutes = 1;

        /// <summary>
        /// The time interval (in minutes) between checks for any item needing an alert.
        /// </summary>
        public int CheckTimerIntervalInMinutes
        {
            get { return checkTimerIntervalInMinutes; }
            set { checkTimerIntervalInMinutes = value; }
        }

        private bool snoozing = false;

        private int snoozeMinutes = 5;

        /// <summary>
        /// Gets or sets the number of minutes that pressing the Snooze Button will hide the Alert window. 
        /// </summary>
        public int SnoozeMinutes
        {
            get { return snoozeMinutes; }
            set { snoozeMinutes = value; }
        }

        private bool soundAlert = true;

        /// <summary>
        /// Gets or sets whether a MeesageBeep should sound when the AlertWindow is shown.
        /// </summary>
        public bool SoundAlert
        {
            get { return soundAlert; }
            set { soundAlert = value; }
        }
        #endregion

        #region Utility methods

        // returns integer equivalents of the strings in the Remider dropdown
        private int GetMinutesFromDisplayValue(string s)
        {
            int i = s.IndexOf(AlertStrings[_minute].Substring(0, 5));
            if (i > -1)
            {
                i = 0;
                int k = 0;
                while (k < s.Length && char.IsDigit(s[k]))
                {
                    i = (10 * i) + (int)s[k] - 48;
                    k++;
                }
            }
            else
            {
                i = s.IndexOf(AlertStrings[_hour].Substring(0, 4));
                if (i > -1)
                {
                    i = 0;
                    int k = 0;
                    while (k < s.Length && char.IsDigit(s[k]))
                    {
                        i = (10 * i) + (int)s[k] - 48;
                        k++;
                    }

                    i = 60 * i;
                }
            }

            return i;
        }

        ////returns the string to be displayed in the Time column
        private string GetTimeString(double minutes)
        {
            string s = (minutes < 0) ? AlertStrings[_ago] : AlertStrings[_fromNow];
            double val = Math.Abs(minutes);
            s = (minutes > 60) ? string.Format("{0:0.0} {2}{1}", val / 60, s, AlertStrings[_hour]) : string.Format("{0:0} {2}{1}", val, s, AlertStrings[_minute]);
            return s;
        }

        #endregion

        #region Button Handlers

        private void button3_Click(object sender, EventArgs e)
        {
            ////snooze
            this.alertTimer.Enabled = false;
            this.alertTimer.Interval = 1000 * 60 * snoozeMinutes; ////3000;// 1000 * 60 * snoozeMinutes; //change this *****************
            this.alertTimer.Enabled = true;
            this.Hide();
            snoozing = true;
        }

        void button2_Click(object sender, EventArgs e)
        {
            ////dismiss an item - remove it from the list 
            //// and mark it as no reminder but do not make it dirty
             GridCurrentCell cc = this.Grid.CurrentCell;
             if (cc.RowIndex > 0 && cc.RowIndex <= list.Count)
             {
                 IScheduleAppointment item = list[cc.RowIndex - 1] as IScheduleAppointment;
                 bool b = item.Dirty;
                 item.Reminder = false;
                 item.Dirty = b;
                 list.Remove(item);
                 if (list.Count == 0)
                 {
                     this.Close();
                 }
                 else
                 {
                     this.Grid.Refresh();
                     if (cc.RowIndex > list.Count)
                     {
                         this.Grid.CurrentCell.MoveTo(list.Count, 1);
                     }
                 }
             }
             else if (list.Count == 0)
             {
                 this.Close();
             }
        }

        void button1_Click(object sender, EventArgs e)
        {
            ////dismiss all
            foreach (IScheduleAppointment item in list)
            {
                bool b = item.Dirty;
                item.Reminder = false;
                item.Dirty = b;
            }

            list.Clear();
            this.Close();
        }

        #endregion

        #region Form Overrides

        /// <summary>
        /// Overrided to sound a beep when an alert appears.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible && this.WindowState != FormWindowState.Minimized)
            {
                CancelEventArgs e1 = new CancelEventArgs();
                OnAlertSounding(e1);
                if (!e1.Cancel && this.SoundAlert)
                {
                    MessageBeep(0);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="AlertSounding"/> event.
        /// </summary>
        /// <param name="e">A CanceEventArgs object.</param>
        protected virtual void OnAlertSounding(CancelEventArgs e)
        {
            if (AlertSounding != null)
            {
                 AlertSounding(this, e);
            }
        }

        /// <summary>
        /// Raised when an AlertWindow is made visible and SoundAlert is true.
        /// </summary>
        /// <remarks>
        /// You can use this event to apply custom notification sounds if desired. Setting 
        /// e.Cancel = true will prevent the default MessageBeep. The sender object will
        /// be the ALertForm, and you can index it to retrieve the current IScheduleAppointments
        /// waiting in the alert list. You can use <see cref="Count"/> to get the number of items
        /// in the current alert list.
        /// </remarks>
        public CancelEventHandler AlertSounding;

        /// <summary>
        /// Overriden to make sure timer is off and hide the form instead of closing it.
        /// </summary>
        /// <param name="e">The CancelEventArgs.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.Visible)
            {
                e.Cancel = true;
                this.Visible = false;

                alertTimer.Enabled = false;
                alertTimer.Interval = checkTimerIntervalInMinutes * 1000 * 60;  ////15000;//  checkTimerIntervalInMinutes * 1000 * 60;  //change this *****************
                timerCheckMode = true;
                alertTimer.Enabled = true;
             }
        }
        #endregion

        #region Grid events

        ////used to sync up lables on the form with the current selected item in the grid.
        void Grid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
        {
            GridCurrentCell cc = this.Grid.CurrentCell;
            if (cc.RowIndex != cc.MoveFromRowIndex && cc.RowIndex > 0 && cc.RowIndex <= list.Count)
            {
                IScheduleAppointment item = list[cc.RowIndex - 1] as IScheduleAppointment;
                this.labelSubject.Text = item.Subject;
                this.labelTime.Text = item.StartTime.ToString(schedule.Appearance.DateTimeFormat);
            }
        }

        ////used to make col1 take up the remaining client width
        void Grid_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            GridControl grid = sender as GridControl;
            if (e.Index == 1)
            {
                e.Size = Math.Max(0, grid.ClientSize.Width - grid.ColWidths[2]);
                e.Handled = true;
            }
            else if (e.Index == 2)
            {
                e.Size = 140;
                e.Handled = true;
            }
        }

        ////avoid any activations
        void Grid_CurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs e)
        {
            e.ColIndex = 0;
        }

        ////set the rowcount from the list
        void Grid_QueryRowCount(object sender, GridRowColCountEventArgs e)
        {
            if (list != null)
            {
                e.Count = this.list.Count;
                e.Handled = true;
            }
        }

        ////set the values from the list
        void Grid_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (list != null && e.RowIndex > 0 && e.ColIndex > 0 && e.RowIndex <= list.Count)
            {
                IScheduleAppointment item = list[e.RowIndex - 1] as IScheduleAppointment;
                if (item != null)
                {
                    if (e.ColIndex == 1)
                    {
                        e.Style.Text = item.Subject;
                    }
                    else if (e.ColIndex == 2)
                    {
                        TimeSpan ts = item.StartTime - DateTime.Now;
                        e.Style.Text = GetTimeString(ts.TotalMinutes);
                    }
                }
            }
        }   
        
        #endregion

        #region Interop methods
        [DllImport("user32")]
        private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
        private void FlashMe()
        {
            FlashWindow(this.Handle, true);
        }

        [DllImport("user32.dll")]
        extern static void MessageBeep(int beep);

        #endregion
    }
}