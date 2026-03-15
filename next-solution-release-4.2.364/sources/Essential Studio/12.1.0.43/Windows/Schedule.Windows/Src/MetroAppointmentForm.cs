//-------------------------------------------------------------------------------------------------
// <copyright file="MetroAppointmentForm.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Displays a form allowing a user to edit information in a <see cref="IScheduleAppointment"/>.
    /// </summary>
    public class MetroAppointmentForm : MetroForm
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private RTLPanel panel1;
        private System.Windows.Forms.TextBox subjectTextBox;
        private System.Windows.Forms.TextBox locationTextBox;

        private IScheduleDataProvider dataProvider;
        private IScheduleAppointment item;
        private RTLPanel panel2;
        private Syncfusion.Windows.Forms.ButtonAdv button1;
        private Syncfusion.Windows.Forms.ButtonAdv button2;
        private System.Windows.Forms.TextBox contentTextBox;
        private System.Windows.Forms.Label label3;
        private RTLGroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv startDateTimePicker1;
        private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv endDateTimePicker2;
        private ColorComboBox labelComboBox;
        private Syncfusion.Windows.Forms.Grid.GridListControl labelGridListControl;
        private Syncfusion.Windows.Forms.Tools.CheckBoxAdv allDayEventCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox startListBox;
        private System.Windows.Forms.ListBox endListBox;
        private RTLComboBoxBase startComboBoxBase;
        private RTLComboBoxBase endComboBoxBase;
        private Syncfusion.Windows.Forms.Tools.CheckBoxAdv reminderCheckBox;
        private ColorComboBox reminderComboBox;
        private Syncfusion.Windows.Forms.Grid.GridListControl reminderGridListControl;
        private Syncfusion.Windows.Forms.Grid.GridListControl markerGridListControl;
        private System.Windows.Forms.Label label6;
        private ColorComboBox markerComboBox;
        private int minimumTimeSlot;
        public ButtonAdv ShowRecurDialog;
        private string recurringRule = string.Empty;
        private IContainer components;
        private ScheduleControl scheduleCtrl = null;
        private string oldStartTime = string.Empty;
        private string oldEndTime = string.Empty;
        /// <summary>
        /// Initializes the <see cref="AppointmentForm"/>.
        /// </summary>
        public MetroAppointmentForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (ScheduleControl.isMirrored && !IgnoreScheduleRTLSettings)
            {
                switchRTL(this.Controls);
            }

            ////set up strings again to allow localization
            InitStrings();

            ////subscribe to events
            this.endComboBoxBase.DropDown += new System.EventHandler(this.endComboBoxBase_DropDown);
            this.reminderCheckBox.CheckedChanged += new System.EventHandler(this.reminderCheckBox_CheckedChanged);
            this.allDayEventCheckBox.CheckedChanged += new System.EventHandler(this.allDayEventCheckBox_CheckedChanged);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.ShowRecurDialog.Click += new System.EventHandler(this.button3_Click);           
            hooked = true;
        }

        #region RightToLeft support
        private void switchRTL(IEnumerable controls)
        {
            foreach (Control c in controls)
            {
                c.RightToLeft = RightToLeft.Yes;
                switchRTL(c.Controls);
            }
        }

        private static bool ignoreScheduleRTLSettings = false;

        /// <summary>
        /// Gets or sets whether this form will use the RTL settings from 
        /// the ScheduleControl to decide whether to mirror the form.
        /// </summary>
        public static bool IgnoreScheduleRTLSettings
        {
            get { return ignoreScheduleRTLSettings; }
            set { ignoreScheduleRTLSettings = value; }
        }

        const int WS_EX_LAYOUTRTL = 0x400000;
        const int WS_EX_NOINHERITLAYOUT = 0x100000;
        
        /// <override/> 
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!base.DesignMode && ScheduleControl.isMirrored && !IgnoreScheduleRTLSettings)
                {
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
                }

                return cp;
            }
        }
        #endregion

        /// <summary>
        /// Overridden to conditionally display the recurring event button.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.ShowRecurDialog.Visible = dataProvider as IRecurringScheduleDataProvider != null;
            oldEndTime = this.endComboBoxBase.Text;
            oldStartTime = this.startComboBoxBase.Text;
            try
            {
                System.Drawing.Icon ico = new System.Drawing.Icon(GetIconFile(@"Resources\table_icon.ico"));
                this.Icon = ico;
            }
            catch { }
        }

        private string GetIconFile(string bitmapName)
        {
            for (int n = 0; n < 10; n++)
            {
                if (System.IO.File.Exists(bitmapName))
                    return bitmapName;

                bitmapName = @"..\" + bitmapName;
            }

            return bitmapName;
        }

        private bool hooked = false;

        /// <summary>
        /// Use this method unsubcribe to the click event that opens the default recurrence dialog.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void UnhookDefaultRecurrenceDialog()
        {
            if (hooked)
            {
                this.ShowRecurDialog.Click -= new System.EventHandler(this.button3_Click);
            }

            hooked = false;
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

        #region Windows Form Designer generated code
        /// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroAppointmentForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new Syncfusion.Windows.Forms.Schedule.RTLPanel();
            this.ShowRecurDialog = new Syncfusion.Windows.Forms.ButtonAdv();
            this.markerComboBox = new Syncfusion.Windows.Forms.Schedule.ColorComboBox();
            this.markerGridListControl = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.label6 = new System.Windows.Forms.Label();
            this.reminderComboBox = new Syncfusion.Windows.Forms.Schedule.ColorComboBox();
            this.reminderGridListControl = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.reminderCheckBox = new Syncfusion.Windows.Forms.Tools.CheckBoxAdv();
            this.endComboBoxBase = new Syncfusion.Windows.Forms.Schedule.RTLComboBoxBase();
            this.endListBox = new System.Windows.Forms.ListBox();
            this.startComboBoxBase = new Syncfusion.Windows.Forms.Schedule.RTLComboBoxBase();
            this.startListBox = new System.Windows.Forms.ListBox();
            this.allDayEventCheckBox = new Syncfusion.Windows.Forms.Tools.CheckBoxAdv();
            this.labelComboBox = new Syncfusion.Windows.Forms.Schedule.ColorComboBox();
            this.labelGridListControl = new Syncfusion.Windows.Forms.Grid.GridListControl();
            this.endDateTimePicker2 = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
            this.startDateTimePicker1 = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new Syncfusion.Windows.Forms.Schedule.RTLGroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.subjectTextBox = new System.Windows.Forms.TextBox();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel2 = new Syncfusion.Windows.Forms.Schedule.RTLPanel();
            this.button2 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.button1 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.contentTextBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.markerComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.markerGridListControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderGridListControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderCheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endComboBoxBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startComboBoxBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allDayEventCheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelGridListControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateTimePicker2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateTimePicker2.Calendar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateTimePicker1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateTimePicker1.Calendar)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(37, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sub&ject:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(37, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Location:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ShowRecurDialog);
            this.panel1.Controls.Add(this.markerComboBox);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.reminderComboBox);
            this.panel1.Controls.Add(this.reminderCheckBox);
            this.panel1.Controls.Add(this.endComboBoxBase);
            this.panel1.Controls.Add(this.startComboBoxBase);
            this.panel1.Controls.Add(this.allDayEventCheckBox);
            this.panel1.Controls.Add(this.labelComboBox);
            this.panel1.Controls.Add(this.endDateTimePicker2);
            this.panel1.Controls.Add(this.startDateTimePicker1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.subjectTextBox);
            this.panel1.Controls.Add(this.locationTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(632, 264);
            this.panel1.TabIndex = 0;
            // 
            // ShowRecurDialog
            // 
            this.ShowRecurDialog.Location = new System.Drawing.Point(493, 112);
            this.ShowRecurDialog.Name = "ShowRecurDialog";
            this.ShowRecurDialog.Size = new System.Drawing.Size(107, 23);
            this.ShowRecurDialog.TabIndex = 8;
            this.ShowRecurDialog.Text = "Make Recurring";
            this.ShowRecurDialog.Appearance = ButtonAppearance.Metro;
            this.ShowRecurDialog.UseVisualStyleBackColor = true;
            this.ShowRecurDialog.UseVisualStyle = true;
            this.ShowRecurDialog.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowRecurDialog.ForeColor = System.Drawing.Color.White;
            // 
            // markerComboBox
            // 
            this.markerComboBox.BackColor = System.Drawing.Color.White;
            this.markerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.markerComboBox.ListControl = this.markerGridListControl;
            this.markerComboBox.Location = new System.Drawing.Point(352, 208);
            this.markerComboBox.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.markerComboBox.Name = "markerComboBox";
            this.markerComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.markerComboBox.ShowColor = true;
            this.markerComboBox.Size = new System.Drawing.Size(121, 21);
            this.markerComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.markerComboBox.TabIndex = 11;
            this.markerComboBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // markerGridListControl
            // 
            this.markerGridListControl.BackColor = System.Drawing.Color.White;
            this.markerGridListControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.markerGridListControl.FillLastColumn = true;
            this.markerGridListControl.ItemHeight = 25;
            this.markerGridListControl.Location = new System.Drawing.Point(168, 16);
            this.markerGridListControl.MultiColumn = false;
            this.markerGridListControl.Name = "markerGridListControl";
            this.markerGridListControl.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.markerGridListControl.Properties.MarkColHeader = false;
            this.markerGridListControl.Properties.MarkRowHeader = false;
            this.markerGridListControl.SelectedIndex = -1;
            this.markerGridListControl.Properties.DisplayHorzLines = false;
            this.markerGridListControl.ShowColumnHeader = false;
            this.markerGridListControl.Size = new System.Drawing.Size(40, 74);
            this.markerGridListControl.TabIndex = 8;
            this.markerGridListControl.TabStop = false;
            this.markerGridListControl.TopIndex = 0;
            this.markerGridListControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(264, 210);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "Sho&w time as:";
            // 
            // reminderComboBox
            // 
            this.reminderComboBox.BackColor = System.Drawing.Color.White;
            this.reminderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.reminderComboBox.ListControl = this.reminderGridListControl;
            this.reminderComboBox.Location = new System.Drawing.Point(112, 208);
            this.reminderComboBox.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.reminderComboBox.Name = "reminderComboBox";
            this.reminderComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.reminderComboBox.ShowColor = false;
            this.reminderComboBox.Size = new System.Drawing.Size(121, 21);
            this.reminderComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.reminderComboBox.TabIndex = 10;
            // 
            // reminderGridListControl
            // 
            this.reminderGridListControl.BackColor = System.Drawing.Color.White;
            this.reminderGridListControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.reminderGridListControl.FillLastColumn = true;
            this.reminderGridListControl.ItemHeight = 17;
            this.reminderGridListControl.Location = new System.Drawing.Point(112, 8);
            this.reminderGridListControl.MultiColumn = false;
            this.reminderGridListControl.Name = "reminderGridListControl";
            this.reminderGridListControl.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.reminderGridListControl.Properties.MarkColHeader = false;
            this.reminderGridListControl.Properties.MarkRowHeader = false;
            this.reminderGridListControl.SelectedIndex = -1;
            this.reminderGridListControl.ShowColumnHeader = false;
            this.reminderGridListControl.Size = new System.Drawing.Size(40, 144);
            this.reminderGridListControl.TabIndex = 7;
            this.reminderGridListControl.TabStop = false;
            this.reminderGridListControl.TopIndex = 0;
            // 
            // reminderCheckBox
            // 
            this.reminderCheckBox.DrawFocusRectangle = false;
            this.reminderCheckBox.Location = new System.Drawing.Point(40, 208);
            this.reminderCheckBox.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.reminderCheckBox.Name = "reminderCheckBox";
            this.reminderCheckBox.Size = new System.Drawing.Size(80, 24);
            this.reminderCheckBox.Style = Syncfusion.Windows.Forms.Tools.CheckBoxAdvStyle.Metro;
            this.reminderCheckBox.TabIndex = 9;
            this.reminderCheckBox.Text = "&Reminder:";
            this.reminderCheckBox.ThemesEnabled = false;
            // 
            // endComboBoxBase
            // 
            this.endComboBoxBase.BackColor = System.Drawing.Color.White;
            this.endComboBoxBase.ListControl = this.endListBox;
            this.endComboBoxBase.Location = new System.Drawing.Point(240, 144);
            this.endComboBoxBase.Name = "endComboBoxBase";
            this.endComboBoxBase.Size = new System.Drawing.Size(128, 21);
            this.endComboBoxBase.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.endComboBoxBase.TabIndex = 6;
            this.endComboBoxBase.Text = "comboBoxBase1";
            // 
            // endListBox
            // 
            this.endListBox.BackColor = System.Drawing.Color.White;
            this.endListBox.Location = new System.Drawing.Point(288, 16);
            this.endListBox.Name = "endListBox";
            this.endListBox.Size = new System.Drawing.Size(48, 95);
            this.endListBox.TabIndex = 6;
            this.endListBox.TabStop = false;
            // 
            // startComboBoxBase
            // 
            this.startComboBoxBase.BackColor = System.Drawing.Color.White;
            this.startComboBoxBase.ListControl = this.startListBox;
            this.startComboBoxBase.Location = new System.Drawing.Point(240, 112);
            this.startComboBoxBase.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startComboBoxBase.Name = "startComboBoxBase";
            this.startComboBoxBase.Size = new System.Drawing.Size(128, 21);
            this.startComboBoxBase.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.startComboBoxBase.TabIndex = 4;
            this.startComboBoxBase.Text = "comboBoxBase1";
            // 
            // startListBox
            // 
            this.startListBox.BackColor = System.Drawing.Color.White;
            this.startListBox.Location = new System.Drawing.Point(224, 16);
            this.startListBox.Name = "startListBox";
            this.startListBox.Size = new System.Drawing.Size(48, 95);
            this.startListBox.TabIndex = 5;
            this.startListBox.TabStop = false;
            // 
            // allDayEventCheckBox
            // 
            this.allDayEventCheckBox.DrawFocusRectangle = false;
            this.allDayEventCheckBox.Location = new System.Drawing.Point(384, 112);
            this.allDayEventCheckBox.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.allDayEventCheckBox.Name = "allDayEventCheckBox";
            this.allDayEventCheckBox.Size = new System.Drawing.Size(96, 20);
            this.allDayEventCheckBox.Style = Syncfusion.Windows.Forms.Tools.CheckBoxAdvStyle.Metro;
            this.allDayEventCheckBox.TabIndex = 7;
            this.allDayEventCheckBox.Text = "All Day Event";
            this.allDayEventCheckBox.ThemesEnabled = false;
            // 
            // labelComboBox
            // 
            this.labelComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelComboBox.BackColor = System.Drawing.Color.White;
            this.labelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.labelComboBox.ListControl = this.labelGridListControl;
            this.labelComboBox.Location = new System.Drawing.Point(400, 56);
            this.labelComboBox.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.labelComboBox.Name = "labelComboBox";
            this.labelComboBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelComboBox.ShowColor = true;
            this.labelComboBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelComboBox.Size = new System.Drawing.Size(200, 25);
            this.labelComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.labelComboBox.TabIndex = 2;
            this.labelComboBox.Border3DStyle = Border3DStyle.Flat;
            // 
            // labelGridListControl
            // 
            this.labelGridListControl.BackColor = System.Drawing.Color.White;
            this.labelGridListControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelGridListControl.Properties.DisplayHorzLines = false;
            this.labelGridListControl.FillLastColumn = true;
            this.labelGridListControl.ItemHeight = 25;
            this.labelGridListControl.Location = new System.Drawing.Point(56, 8);
            this.labelGridListControl.MultiColumn = false;
            this.labelGridListControl.Name = "labelGridListControl";
            this.labelGridListControl.Properties.BackgroundColor = System.Drawing.SystemColors.Window;
            this.labelGridListControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGridListControl.Properties.MarkColHeader = false;
            this.labelGridListControl.Properties.MarkRowHeader = false;
            this.labelGridListControl.SelectedIndex = -1;
            this.labelGridListControl.ShowColumnHeader = false;
            this.labelGridListControl.Size = new System.Drawing.Size(40, 144);
            this.labelGridListControl.TabIndex = 4;
            this.labelGridListControl.TabStop = false;
            this.labelGridListControl.TopIndex = 0;
            // 
            // endDateTimePicker2
            // 
            this.endDateTimePicker2.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.endDateTimePicker2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // 
            // 
            this.endDateTimePicker2.Calendar.Culture = new System.Globalization.CultureInfo("en-US");
            this.endDateTimePicker2.Calendar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.endDateTimePicker2.Calendar.GridLines = Syncfusion.Windows.Forms.Grid.GridBorderStyle.None;
            this.endDateTimePicker2.Calendar.Location = new System.Drawing.Point(0, 0);
            this.endDateTimePicker2.Calendar.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.Calendar.Name = "monthCalendar";
            this.endDateTimePicker2.Calendar.ScrollButtonSize = new System.Drawing.Size(24, 24);
            this.endDateTimePicker2.Calendar.SelectedDates = new System.DateTime[0];
            this.endDateTimePicker2.Calendar.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.endDateTimePicker2.Calendar.TabIndex = 0;
            this.endDateTimePicker2.Calendar.WeekFont = new System.Drawing.Font("Verdana", 8F);
            this.endDateTimePicker2.Calendar.WeekInterior = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.Color.PeachPuff, System.Drawing.Color.AntiqueWhite);
            // 
            // 
            // 
            this.endDateTimePicker2.Calendar.NoneButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.endDateTimePicker2.Calendar.NoneButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.Calendar.NoneButton.Location = new System.Drawing.Point(78, 0);
            this.endDateTimePicker2.Calendar.NoneButton.Size = new System.Drawing.Size(72, 20);
            this.endDateTimePicker2.Calendar.NoneButton.Text = "None";
            this.endDateTimePicker2.Calendar.NoneButton.UseVisualStyle = true;
            // 
            // 
            // 
            this.endDateTimePicker2.Calendar.TodayButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.endDateTimePicker2.Calendar.TodayButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.Calendar.TodayButton.Location = new System.Drawing.Point(0, 0);
            this.endDateTimePicker2.Calendar.TodayButton.Size = new System.Drawing.Size(78, 20);
            this.endDateTimePicker2.Calendar.TodayButton.Text = "Today";
            this.endDateTimePicker2.Calendar.TodayButton.UseVisualStyle = true;
            this.endDateTimePicker2.CalendarSize = new System.Drawing.Size(189, 176);
            this.endDateTimePicker2.DropDownImage = null;
            this.endDateTimePicker2.DropDownNormalColor = System.Drawing.Color.White;
            this.endDateTimePicker2.DropDownPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.DropDownSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.endDateTimePicker2.Location = new System.Drawing.Point(104, 144);
            this.endDateTimePicker2.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.endDateTimePicker2.MinValue = new System.DateTime(((long)(0)));
            this.endDateTimePicker2.Name = "endDateTimePicker2";
            this.endDateTimePicker2.ShowCheckBox = false;
            this.endDateTimePicker2.Size = new System.Drawing.Size(112, 20);
            this.endDateTimePicker2.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.endDateTimePicker2.TabIndex = 5;
            this.endDateTimePicker2.Value = new System.DateTime(2013, 8, 6, 7, 22, 59, 505);
            // 
            // startDateTimePicker1
            // 
            this.startDateTimePicker1.BackColor = System.Drawing.Color.White;
            this.startDateTimePicker1.Border3DStyle = System.Windows.Forms.Border3DStyle.Flat;
            this.startDateTimePicker1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            // 
            // 
            // 
            this.startDateTimePicker1.Calendar.Culture = new System.Globalization.CultureInfo("en-US");
            this.startDateTimePicker1.Calendar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.startDateTimePicker1.Calendar.GridLines = Syncfusion.Windows.Forms.Grid.GridBorderStyle.None;
            this.startDateTimePicker1.Calendar.Location = new System.Drawing.Point(0, 0);
            this.startDateTimePicker1.Calendar.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.Calendar.Name = "monthCalendar";
            this.startDateTimePicker1.Calendar.ScrollButtonSize = new System.Drawing.Size(24, 24);
            this.startDateTimePicker1.Calendar.SelectedDates = new System.DateTime[0];
            this.startDateTimePicker1.Calendar.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.startDateTimePicker1.Calendar.TabIndex = 0;
            this.startDateTimePicker1.Calendar.WeekFont = new System.Drawing.Font("Verdana", 8F);
            this.startDateTimePicker1.Calendar.WeekInterior = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.Color.PeachPuff, System.Drawing.Color.AntiqueWhite);
            // 
            // 
            // 
            this.startDateTimePicker1.Calendar.NoneButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.startDateTimePicker1.Calendar.NoneButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.Calendar.NoneButton.Location = new System.Drawing.Point(78, 0);
            this.startDateTimePicker1.Calendar.NoneButton.Size = new System.Drawing.Size(72, 20);
            this.startDateTimePicker1.Calendar.NoneButton.Text = "None";
            this.startDateTimePicker1.Calendar.NoneButton.UseVisualStyle = true;
            // 
            // 
            // 
            this.startDateTimePicker1.Calendar.TodayButton.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.startDateTimePicker1.Calendar.TodayButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.Calendar.TodayButton.Location = new System.Drawing.Point(0, 0);
            this.startDateTimePicker1.Calendar.TodayButton.Size = new System.Drawing.Size(78, 20);
            this.startDateTimePicker1.Calendar.TodayButton.Text = "Today";
            this.startDateTimePicker1.Calendar.TodayButton.UseVisualStyle = true;
            this.startDateTimePicker1.CalendarSize = new System.Drawing.Size(189, 176);
            this.startDateTimePicker1.DropDownImage = null;
            this.startDateTimePicker1.DropDownNormalColor = System.Drawing.Color.White;
            this.startDateTimePicker1.DropDownPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.DropDownSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.startDateTimePicker1.Location = new System.Drawing.Point(104, 112);
            this.startDateTimePicker1.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.startDateTimePicker1.MinValue = new System.DateTime(((long)(0)));
            this.startDateTimePicker1.Name = "startDateTimePicker1";
            this.startDateTimePicker1.ShowCheckBox = false;
            this.startDateTimePicker1.Size = new System.Drawing.Size(112, 20);
            this.startDateTimePicker1.Style = Syncfusion.Windows.Forms.VisualStyle.Metro;
            this.startDateTimePicker1.TabIndex = 3;
            this.startDateTimePicker1.Value = new System.DateTime(2013, 8, 6, 7, 22, 59, 567);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(37, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "En&d Time:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(37, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 19);
            this.label4.TabIndex = 7;
            this.label4.Text = "Start Time&:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(40, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(560, 8);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(355, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "La&bel:";
            // 
            // subjectTextBox
            // 
            this.subjectTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subjectTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.subjectTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subjectTextBox.ForeColor = System.Drawing.Color.DimGray;
            this.subjectTextBox.Location = new System.Drawing.Point(96, 24);
            this.subjectTextBox.Name = "subjectTextBox";
            this.subjectTextBox.Size = new System.Drawing.Size(504, 22);
            this.subjectTextBox.TabIndex = 0;
            this.subjectTextBox.Text = "subject";
            // 
            // locationTextBox
            // 
            this.locationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.locationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locationTextBox.ForeColor = System.Drawing.Color.DimGray;
            this.locationTextBox.Location = new System.Drawing.Point(96, 56);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(240, 22);
            this.locationTextBox.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(40, 176);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(560, 8);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.markerGridListControl);
            this.panel2.Controls.Add(this.reminderGridListControl);
            this.panel2.Controls.Add(this.endListBox);
            this.panel2.Controls.Add(this.startListBox);
            this.panel2.Controls.Add(this.labelGridListControl);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 446);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(632, 72);
            this.panel2.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.button2.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.RaisedInner;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(520, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyle = true;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Metro;
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.button1.BorderStyleAdv = Syncfusion.Windows.Forms.ButtonAdvBorderStyle.RaisedInner;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(376, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Save and Close";
            this.button1.UseVisualStyle = true;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // contentTextBox
            // 
            this.contentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentTextBox.ForeColor = System.Drawing.Color.DimGray;
            this.contentTextBox.Location = new System.Drawing.Point(0, 264);
            this.contentTextBox.Multiline = true;
            this.contentTextBox.Name = "contentTextBox";
            this.contentTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.contentTextBox.Size = new System.Drawing.Size(632, 182);
            this.contentTextBox.TabIndex = 1;
            this.contentTextBox.Text = "contentTextBox";
            // 
            // MetroAppointmentForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.BorderThickness = 2;
            this.CancelButton = this.button2;
            this.CaptionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.ClientSize = new System.Drawing.Size(632, 518);
            this.Controls.Add(this.contentTextBox);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.DropShadow = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(161)))), ((int)(((byte)(226)))));
            this.MinimizeBox = false;
            this.Name = "MetroAppointmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter Appointment";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.markerComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.markerGridListControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderGridListControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reminderCheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endComboBoxBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startComboBoxBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allDayEventCheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.labelGridListControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateTimePicker2.Calendar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.endDateTimePicker2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateTimePicker1.Calendar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateTimePicker1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        #region utilites
        /// <summary>
        /// Initializes the form using the <see cref="IScheduleDataProvider"/> that provides
        /// the data for the ScheduleControl. 
        /// </summary>
        /// <param name="provider">The IScheduleDataProvider object.</param>
        /// <param name="item">The current item to be displayed on this form.</param>
        /// <param name="minimumTimeSlot">The time increment used in the time dropdowns.</param>
        /// <param name="format">The time format used in the time dropdowns.</param>
        /// <remarks>
        /// IScheduleDataProvider is the object that provides the data to the ScheduleControl.
        /// This includes items like the DropLists that provide the options for many of the
        /// properties you see in IScheduleAppointments inaddition to the IScheduleAppointments themselves. This
        /// method uses the information from the IScheduleDataProvider to populate the controls
        /// on this form.
        /// </remarks>
       public void SetDataProvider(IScheduleDataProvider provider, IScheduleAppointment item, int minimumTimeSlot, string format)
        {
            this.dataProvider = provider;
            this.item = item;
            this.minimumTimeSlot = minimumTimeSlot;

            PopulateTimeListBoxes(minimumTimeSlot, format);

            this.labelComboBox.ListControl.DataSource = provider.GetLabels();
            this.labelComboBox.ListControl.ValueMember = "ValueMember";
            this.labelComboBox.ListControl.DisplayMember = "DisplayMember";
            this.reminderComboBox.ListControl.DataSource = provider.GetReminders();
            this.reminderComboBox.ListControl.ValueMember = "ValueMember";
            this.reminderComboBox.ListControl.DisplayMember = "DisplayMember";

            this.markerComboBox.ListControl.DataSource = provider.GetMarkers();
            this.markerComboBox.ListControl.ValueMember = "ValueMember";
            this.markerComboBox.ListControl.DisplayMember = "DisplayMember";

            SetItemIntoForm();
        }

       public void SetDataProvider(IScheduleDataProvider provider, IScheduleAppointment item, int minimumTimeSlot, string format,ScheduleControl schedule)
       {
           scheduleCtrl = new ScheduleControl();
           scheduleCtrl = schedule;
           this.dataProvider = provider;
           this.item = item;
           this.minimumTimeSlot = minimumTimeSlot;
           PopulateTimeListBoxes(minimumTimeSlot, format);
           this.labelComboBox.ListControl.DataSource = provider.GetLabels();
           this.labelComboBox.ListControl.ValueMember = "ValueMember";
           this.labelComboBox.ListControl.DisplayMember = "DisplayMember";
           this.reminderComboBox.ListControl.DataSource = provider.GetReminders();
           this.reminderComboBox.ListControl.ValueMember = "ValueMember";
           this.reminderComboBox.ListControl.DisplayMember = "DisplayMember";
           this.markerComboBox.ListControl.DataSource = provider.GetMarkers();
           this.markerComboBox.ListControl.ValueMember = "ValueMember";
           this.markerComboBox.ListControl.DisplayMember = "DisplayMember";
           SetItemIntoForm();
       }

        /// <summary>
        /// Initializes the form using the <see cref="IScheduleDataProvider"/> that provides
        /// the data for the ScheduleControl. 
        /// </summary>
        /// <param name="provider">The IScheduleDataProvider object.</param>
        /// <param name="item">The current item to be displayed on this form.</param>
        /// <remarks>
        /// IScheduleDataProvider is the object that provides the data to the ScheduleControl.
        /// This includes items like the DropLists that provide the options for many of the
        /// properties you see in IScheduleAppointments inaddition to the IScheduleAppointments themselves. This
        /// method uses the information from the IScheduleDataProvider to populate the controls
        /// on this form.
        /// </remarks>
        public void SetDataProvider(IScheduleDataProvider provider, IScheduleAppointment item)
        {
            SetDataProvider(provider, item, 30, "h:mm tt");
        }

        ////called from SetDataProvider to populate the form with the item
        private void SetItemIntoForm()
        {
            this.subjectTextBox.Text = this.item.Subject;
            this.contentTextBox.Text = this.item.Content;
            this.locationTextBox.Text = this.item.LocationValue;
            this.labelComboBox.ListControl.SelectedValue = this.item.LabelValue;
            ////Span Start and EndDate
            if (RecurrenceSupport.IsSpanItem(item) 
                && ((IRecurringScheduleAppointment)item).DateList != null)
            {
                this.startDateTimePicker1.Value = ((IRecurringScheduleAppointment)item).DateList.BaseDate;
                this.endDateTimePicker2.Value = ((IRecurringScheduleAppointment)item).DateList.TerminalDate;
                if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
                {
                    this.startComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(((IRecurringScheduleAppointment)item).DateList.BaseDate.ToLongTimeString());
                    this.endComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(((IRecurringScheduleAppointment)item).DateList.TerminalDate.ToLongTimeString());
                }
                else
                {
                    this.startComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(((IRecurringScheduleAppointment)item).DateList.BaseDate);
                    this.endComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(((IRecurringScheduleAppointment)item).DateList.TerminalDate);
                }
            }
            else
            {
                if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
                {
                    this.startDateTimePicker1.Value = this.item.StartTime;
                    this.endDateTimePicker2.Value = this.item.EndTime;
                    this.startComboBoxBase.ListControl.Text = this.item.StartTime.ToLongTimeString();
                    this.endComboBoxBase.ListControl.Text = this.item.EndTime.ToLongTimeString();
                }
                else
                {
                    this.startDateTimePicker1.Value = this.item.StartTime.Date;
                    this.endDateTimePicker2.Value = this.item.EndTime.Date;
                    this.startComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(this.item.StartTime);
                    this.endComboBoxBase.ListControl.SelectedIndex = DateTimeToIndex(this.item.EndTime);
                }
            }

            ////handle problem of initial text not showing
            if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
            {
                this.startComboBoxBase.Text = this.startComboBoxBase.ListControl.Text;
                this.endComboBoxBase.Text = this.endComboBoxBase.ListControl.Text;               
            }
            else
            {
                this.startComboBoxBase.Text = ((ListBox)this.startComboBoxBase.ListControl).Items[this.startComboBoxBase.ListControl.SelectedIndex].ToString();
                this.endComboBoxBase.Text = ((ListBox)this.endComboBoxBase.ListControl).Items[this.endComboBoxBase.ListControl.SelectedIndex].ToString();
            }
            this.contentTextBox.Text = this.item.Content;
            this.allDayEventCheckBox.Checked = this.item.AllDay; 
            this.reminderCheckBox.Checked = this.item.Reminder;
            this.reminderComboBox.ListControl.SelectedValue = this.item.ReminderValue;
            this.reminderComboBox.Enabled = this.item.Reminder;
            this.markerComboBox.ListControl.SelectedValue = this.item.MarkerValue;
            if (item is IRecurringScheduleAppointment && dataProvider is IRecurringScheduleDataProvider)
            {
                IRecurringScheduleAppointment recurItem = item as IRecurringScheduleAppointment;
                SetTextInRecurButton(recurItem);
                this.recurringRule = recurItem.RecurrenceRule;
            }
        }

        private void SetTextInRecurButton(IRecurringScheduleAppointment recurItem)
        {
            this.ShowRecurDialog.Text = recurItem.RecurrenceRule.Length > 0
                   ? (RecurrenceSupport.IsSpanItem(recurItem) ? DisplayStrings[_Delete_Span] : DisplayStrings[_Edit_Recurring])
                   : DisplayStrings[_Make_Recurring];
        }

        ////called from endComboBoxBase_DropDown to set the droplist
        private void SetEndComboBoxList()
        {
            if (ScheduleControl.isMirrored || !ScheduleControl.DisplayTimeSpansInEndTimeDropDown)
            {
                return;
            }

            bool span = this.startDateTimePicker1.Value.Date < this.endDateTimePicker2.Value.Date;
            int start = span ? 0 : this.startComboBoxBase.ListControl.SelectedIndex;
            if (start < 0)
            {
                start = FindClosestChoice(this.startComboBoxBase.Text, this.startDateTimePicker1.Value.Date);
            }

            ListBox lb = (ListBox)this.endComboBoxBase.ListControl;
            ListBox lbs = (ListBox)this.startComboBoxBase.ListControl;
            lb.Items.Clear();
            int count = lbs.Items.Count;
            double d = 60d / minimumTimeSlot;
            for (int i = start; i < count; ++i)
            {
                int time = (i - start) * minimumTimeSlot;
                if (span)
                {
                    lb.Items.Add(lbs.Items[i].ToString());
                }
                else if (time <= 60)
                {
                    lb.Items.Add(string.Format(lbs.Items[i].ToString() + " ({0} minutes)", time));
                }
                else
                {
                    lb.Items.Add(string.Format("{0} ({1:#.##} hours)", lbs.Items[i], (i - start) / d));
                }
            }
        }

        ////maps the datetime to index in the droplist
        private int DateTimeToIndex(DateTime dt)
        {
            return (dt.Hour * (60 / minimumTimeSlot)) + (dt.Minute / minimumTimeSlot);
        }

        ////maps the datetime to index in the droplist when allow seconds for appointment.
        /// <summary>
        /// Returns the selected index of the drop down.
        /// </summary>
        /// <param name="str">long time</param>
        /// <returns>index of the dropdown</returns>
        private int DateTimeToIndex(string str)
        {
            DateTime dt;
            DateTime.TryParse(str, out dt);
            return (dt.Hour * (60 / minimumTimeSlot)) + (dt.Minute / minimumTimeSlot) + (dt.Second / minimumTimeSlot);
        }

        /// <summary>
        /// Moves values displayed in the form's Controls to the Item object.
        /// </summary>
        public void SaveChangesToItem()
        {
            GetItemFromForm();
        }

        ////populates the item from the values in the form
        private void GetItemFromForm()
        {
            if (this.item.Subject != this.subjectTextBox.Text)
            {
                this.item.Subject = this.subjectTextBox.Text;
                this.item.Dirty = true;
            }

            if (this.item.Content != this.contentTextBox.Text)
            {
                this.item.Content = this.contentTextBox.Text;
                this.item.Dirty = true;
            }

            if (this.item.LocationValue != this.locationTextBox.Text)
            {
                this.item.LocationValue = this.locationTextBox.Text;
                this.item.Dirty = true;
            }

            ////make sure starttime/endtime is set...
            if (!string.IsNullOrEmpty(startComboBoxBase.Text))
            {
                if (this.scheduleCtrl == null)
                    this.startComboBoxBase.ListControl.SelectedIndex = FindClosestChoice(this.startComboBoxBase.Text, this.startDateTimePicker1.Value.Date);                
            }

            if (!string.IsNullOrEmpty(this.endComboBoxBase.Text))
            {
                if (this.scheduleCtrl == null)
                    this.endComboBoxBase.ListControl.SelectedIndex = FindSelectIndex(this.endComboBoxBase.Text, this.endDateTimePicker2.Value.Date);               
            }
            int parseIndex = endComboBoxBase.Text.IndexOf("(");
            string endTime = string.Empty;
            if (parseIndex > 0)
                endTime = this.endComboBoxBase.Text.Substring(0, parseIndex);
            else
                endTime = endComboBoxBase.Text;
            if (this.startDateTimePicker1.Value.Date.Equals(this.endDateTimePicker2.Value.Date) && this.startComboBoxBase.Text.Equals(endTime.Trim()))
            {
                DateTime stTime = Convert.ToDateTime(startComboBoxBase.Text);
                DateTime eTime = Convert.ToDateTime(startComboBoxBase.Text).AddMinutes(30);
                endTime = eTime.ToString();
                if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
                    this.endComboBoxBase.ListControl.SelectedIndex = FindSelectIndex(eTime.ToLongTimeString(), this.endDateTimePicker2.Value.Date.ToLongTimeString());                         
                else
                    this.endComboBoxBase.ListControl.SelectedIndex = FindSelectIndex(eTime.ToString(), this.endDateTimePicker2.Value.Date); 
            }

            DateTime dt;
            if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
            {
                DateTime startDt;
                DateTime.TryParse(this.startComboBoxBase.Text, out startDt);
                TimeSpan startTs;
                TimeSpan.TryParse(string.Format(@"{0:hh\:mm\:ss}", startDt), out startTs);
                dt = this.endDateTimePicker2.Value.Date.Add(startTs);
            }
            else
                dt = this.startDateTimePicker1.Value.Date.AddMinutes(this.startComboBoxBase.ListControl.SelectedIndex * minimumTimeSlot);
            if (this.item.StartTime != dt)
            {
                this.item.StartTime = dt;
                this.item.Dirty = true;
            }
            
            DateTime endDt;
            if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
            {
                DateTime endDat;
                DateTime.TryParse(this.endComboBoxBase.Text, out endDat);
                TimeSpan endTs;
                TimeSpan.TryParse(string.Format(@"{0:hh\:mm\:ss}", endDat), out endTs);
                endDt = this.endDateTimePicker2.Value.Date.Add(endTs);
            }               
            else            
                endDt = this.endDateTimePicker2.Value.Date.AddMinutes(FindClosestChoice(endTime, this.endDateTimePicker2.Value.Date) * minimumTimeSlot);
            if (this.item.EndTime != endDt)
            {
                this.item.EndTime = endDt;
                this.item.Dirty = true;
            }
            
            if (this.item.Content != this.contentTextBox.Text)
            {
                this.item.Content = this.contentTextBox.Text;
                this.item.Dirty = true;
            }

            if (this.labelComboBox.ListControl.SelectedIndex != this.item.LabelValue)
            {
                this.item.LabelValue = this.labelComboBox.ListControl.SelectedIndex;
                this.item.Dirty = true;
            }

            if (this.allDayEventCheckBox.Checked != this.item.AllDay)
            {
                this.item.AllDay = this.allDayEventCheckBox.Checked;
                this.item.Dirty = true;
            }

            if (this.reminderCheckBox.Checked != this.item.Reminder)
            {
                this.item.Reminder = this.reminderCheckBox.Checked;
                this.item.Dirty = true;
            }

            if (this.reminderComboBox.ListControl.SelectedIndex != this.item.ReminderValue)
            {
                this.item.ReminderValue = this.reminderComboBox.ListControl.SelectedIndex;
                this.item.Dirty = true;
            }

            if (this.markerComboBox.ListControl.SelectedIndex != this.item.MarkerValue)
            {
                this.item.MarkerValue = this.markerComboBox.ListControl.SelectedIndex;
                this.item.Dirty = true;
            }

            if (item is IRecurringScheduleAppointment && dataProvider is IRecurringScheduleDataProvider)
            {
                IRecurringScheduleAppointment recurItem = item as IRecurringScheduleAppointment;
                if (this.recurringRule != recurItem.RecurrenceRule)
                {
                    recurItem.RecurrenceRule = this.recurringRule;
                }                
                SetTextInRecurButton(recurItem);
            }
        }

        private int FindSelectIndex(string s, string str)
        {
            DateTime dateTime;
            DateTime.TryParse(str, out dateTime);
            string strartTimeStart = ((ListBox)this.startComboBoxBase.ListControl).Items[0].ToString();
            string endTimeStart = ((ListBox)this.endComboBoxBase.ListControl).Items[0].ToString();
            DateTime t;
            int totalMinimumTimeSlot = 48;
            DateTime tempDate;
            tempDate = dateTime;
            int result = 0;
            int praseIndex = s.IndexOf('(');
            if (praseIndex > 0)
            {
                s = this.endComboBoxBase.Text.Substring(0, praseIndex);
            }
            if (DateTime.TryParse(s, out t))
            {
                TimeSpan diff = tempDate.Date.Subtract(t.Date);
                t = t.Add(diff);
                while (dateTime.AddMinutes(result * minimumTimeSlot) <= t)
                {
                    result++;
                }
                result = result - (totalMinimumTimeSlot - ((ListBox)this.endComboBoxBase.ListControl).Items.Count);
                result--;
            }
            return result;
        }

        private int FindSelectIndex(string s, DateTime dateTime)
        {
            string strartTimeStart = ((ListBox)this.startComboBoxBase.ListControl).Items[0].ToString();
            string endTimeStart = ((ListBox)this.endComboBoxBase.ListControl).Items[0].ToString();
            DateTime t;
            int totalMinimumTimeSlot = 48;
            DateTime tempDate;
            tempDate = dateTime;
            int result = 0;
            int praseIndex = s.IndexOf('(');            
            if (praseIndex > 0)
            {
                s = this.endComboBoxBase.Text.Substring(0, praseIndex);               
            }
            if (DateTime.TryParse(s, out t))
            {
                TimeSpan diff = tempDate.Date.Subtract(t.Date);
                t = t.Add(diff);
                while (dateTime.AddMinutes(result * minimumTimeSlot) <= t)
                {
                    result++;
                }
                result = result - (totalMinimumTimeSlot - ((ListBox)this.endComboBoxBase.ListControl).Items.Count);
                result--;
            }
            return result;
        }

        private int FindClosestChoice(string s, DateTime dateTime)
        {
            string strartTimeStart = ((ListBox)this.startComboBoxBase.ListControl).Items[0].ToString();
            string endTimeStart = ((ListBox)this.endComboBoxBase.ListControl).Items[0].ToString();
            DateTime t;
            DateTime tempDate;
            tempDate = dateTime;
            int result = 0;
            int praseIndex = s.IndexOf('(');
            string endDateStr = string.Empty;
            if (praseIndex > 0)
            {
                s = this.endComboBoxBase.Text.Substring(0, praseIndex);               
            }
            if (DateTime.TryParse(s, out t))
            {
                TimeSpan diff = tempDate.Date.Subtract(t.Date);
                t = t.Add(diff);
                while (dateTime.AddMinutes(result * minimumTimeSlot) <= t)
                {
                    result++;
                }                
                result--;
            }

            return result;
        }

        private int FindClosestChoice(string s, string str)
        {
            DateTime dateTime;
            DateTime.TryParse(str, out dateTime);
            string strartTimeStart = string.Empty;
            string endTimeStart = string.Empty;
            if (scheduleCtrl != null && !scheduleCtrl.AllowSecondsInAppointment)
            {
                strartTimeStart = ((ListBox)this.startComboBoxBase.ListControl).Items[0].ToString();
                endTimeStart = ((ListBox)this.endComboBoxBase.ListControl).Items[0].ToString();
            }
            else
            {
                strartTimeStart = this.startComboBoxBase.ListControl.Text;
                endTimeStart = this.endComboBoxBase.ListControl.Text;
            }
            DateTime t;
            DateTime tempDate;
            tempDate = dateTime;
            int result = 0;
            int praseIndex = s.IndexOf('(');
            string endDateStr = string.Empty;
            if (praseIndex > 0)
            {
                s = this.endComboBoxBase.Text.Substring(0, praseIndex);
            }
            if (DateTime.TryParse(s, out t))
            {
                TimeSpan diff = tempDate.Date.Subtract(t.Date);
                t = t.Add(diff);
                while (dateTime.AddMinutes(result * minimumTimeSlot) <= t)
                {
                    result++;
                }
                result--;
            }

            return result;
        }

        #endregion

        #region button handlers

        ////OK button
        private void button1_Click(object sender, System.EventArgs e)
        {
            int praseIndex = this.endComboBoxBase.Text.IndexOf('(');
            string endDateStr = string.Empty;
            if (praseIndex > 0)
                endDateStr = this.endComboBoxBase.Text.Substring(0, praseIndex);
            else
                endDateStr = this.endComboBoxBase.Text;
            DateTime startDateResult, endDateResult;
            DateTime endDate = endDateTimePicker2.Value;
            DateTime startDate = startDateTimePicker1.Value;
            if ((!string.IsNullOrEmpty(startComboBoxBase.Text)) && DateTime.TryParse(startComboBoxBase.Text, out startDateResult))
            {
                if ((!string.IsNullOrEmpty(endDateStr)) && DateTime.TryParse(endDateStr, out endDateResult))
                {
                    startDate = startDate.Add(startDateResult.TimeOfDay);
                    endDate = endDate.Add(endDateResult.TimeOfDay);
                    if (startDate <= endDate)
                    {
                        GetItemFromForm();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        this.startComboBoxBase.ListControl.Text = string.Empty;
                        this.endComboBoxBase.ListControl.Text = string.Empty;
                        this.startComboBoxBase.ListControl.Text = oldStartTime;
                        this.endComboBoxBase.ListControl.Text = oldEndTime;
                        this.endComboBoxBase.Focus();
                    }
                }
                else
                {
                    this.endComboBoxBase.Text = string.Empty;
                    this.endComboBoxBase.Text = oldEndTime;
                    this.endComboBoxBase.Focus();
                }
            }
            else
            {
                this.startComboBoxBase.Text = string.Empty;
                this.startComboBoxBase.Text = oldStartTime;
                this.startComboBoxBase.Focus();
            }

        }

        ////cancel button
        private void button2_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
      
        ////recurring appointment button - only visible for IRecurringScheduleDateProvider
        void button3_Click(object sender, EventArgs e)
        {
            IRecurringScheduleAppointment recurItem = item as IRecurringScheduleAppointment;
            IRecurringScheduleDataProvider dp = dataProvider as IRecurringScheduleDataProvider; 
            
            ////check if trying to delete a span...
            if (RecurrenceSupport.IsSpanItem(recurItem))
            {
                if (MessageBox.Show(
                    DisplayStrings[_Delete_this_appointment_that_spans_more_than_one_day],
                    DisplayStrings[_Delete_a_Span], 
                    MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                   ////delete button pressed and confirmed; delete done on Abort return code
                    this.DialogResult = DialogResult.Abort;
                    this.Close();
                }

                return;
            }

            ////display a recurring form....
            RecurringForm f = new RecurringForm();
            
            f.StartPosition = FormStartPosition.CenterParent;
            
            f.defaultDate = recurItem.StartTime.Date;
            f.RuleToLocalSettings(recurItem != null ? recurItem.RecurrenceRule : string.Empty);
           
            ////the delete button
            f.button3.Enabled = recurItem.RecurrenceRule.Length > 0;

            DialogResult result = f.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (recurItem == null)
                {
                    recurItem = dp.RecurringList.NewScheduleAppointment() as IRecurringScheduleAppointment;
                }

                recurItem.RecurrenceRule = f.LocalSettingsToRuleString();
                
                this.recurringRule = recurItem.RecurrenceRule;
                ////this.button3.Text = recurItem.RecurrenceRule.Length > 0 ? "Edit Recurring" : "Make Recurring";
                SetTextInRecurButton(recurItem);
            }
            else if (result == DialogResult.Abort)
            {
                ////delete button pressed and confirmed; delete done on Abort return code
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        #endregion

        #region Event handlers
        ////used to populate the drop list

        private void endComboBoxBase_DropDown(object sender, System.EventArgs e)
        {
            SetEndComboBoxList();
        }

        ////used to enable/disable controls dynamically
        private void allDayEventCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {          
            this.startComboBoxBase.Enabled = !this.allDayEventCheckBox.Checked;
            this.endComboBoxBase.Enabled = !this.allDayEventCheckBox.Checked;

            ////this.endDateTimePicker2.Enabled = !this.allDayEventCheckBox.Checked;
            ////this.startDateTimePicker1.Enabled = !this.allDayEventCheckBox.Checked;

            if (this.allDayEventCheckBox.Checked)
            {
                if (this.scheduleCtrl != null && this.scheduleCtrl.AllowSecondsInAppointment)
                {
                    this.startComboBoxBase.Text = "12:00:00 AM";
                    this.endComboBoxBase.Text = "12:00:00 AM";                   
                }
                else
                {
                    this.startComboBoxBase.Text = "12:00 AM";
                    this.endComboBoxBase.Text = "12:00 AM";
                }
            }
         }

        ////used to enable/disable checkbox dynamically
        private void reminderCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            this.reminderComboBox.Enabled = this.reminderCheckBox.Checked;
        }
        #endregion

        #region Strings

        /// <summary>
        /// Holds various strings used in the AppointmentForm.
        /// </summary>
        /// <remarks>
        /// Modifying the strings in this array will affect the display of the AppointmentForm.
        /// You can use these strings to localize the labels that appear on the form.
        /// </remarks>
        /// <example>
        /// Here are the default values of this string array.
        /// <code lang="C#">
        /// public static string[] DisplayStrings = new string[]
        ///                        {    "Subject:",        // 0
        ///                             "Location:",       // 1
        ///                             "Show time as:",   // 2
        ///                             "Reminder:",       // 3
        ///                             "All Day Event",    // 4
        ///                             "End Time:",       // 5
        ///                             "Start Time:",     // 6
        ///                             "Label:",          // 7
        ///                             "subject",          // 8
        ///                             "Cancel",           // 9
        ///                             "Save and Close",   // 10
        ///                             "Enter Appointment", // 11
        ///                             "Delete this appointment that spans more than one day?", // 12
        ///                             "Delete a Span",    // 13
        ///                             "Delete Span",      // 14
        ///                             "Edit Recurring",   // 15
        ///                             "Make Recurring"    // 16
        ///                         };
        /// </code>
        /// <code lang="VB">
        ///          Public Shared DisplayStrings() As String = _
        ///                    {"Subject:", "Location:", _
        ///                     "Show time as:", "Reminder:", _
        ///                     "All Day Event", "End Time:", _
        ///                     "Start Time:", "Label:", _
        ///                     "subject", "Cancel", _
        ///                     "Save and Close", _
        ///                     "Enter Appointment", _
        ///                     "Delete this appointment that spans more than one day?",  _
        ///                     "Delete a Span", _
        ///                     "Delete Span",  _
        ///                     "Edit Recurring", _
        ///                     "Make Recurring"   _
        ///                     }
        /// </code>
        /// </example>
        public static string[] DisplayStrings = new string[]
                                    {
                                        "Sub&ject:", 
                                        "&Location:",
                                        "Sho&w time as:",
                                        "&Reminder:",  
                                        "All Day Event",  
                                        "En&d Time:",  
                                        "Start Time&:",  
                                        "La&bel:",  
                                        "subject",  
                                        "Cancel",  
                                        "Save and Close",  
                                        "Enter Appointment",
                                        "Delete this appointment that spans more than one day?", 
                                        "Delete a Span",
                                        "Delete Span",
                                        "Edit Recurring",
                                        "Make Recurring"
                                        };

        private const int _Subject = 0;
        private const int _Location = 1;
        private const int _Show_time_as = 2;
        private const int _Reminder = 3;
        private const int _All_Day_Event = 4;
        private const int _End_Time = 5;
        private const int _Start_Time = 6;
        private const int _Label = 7;
        private const int _subject = 8;
        private const int _Cancel = 9;
        private const int _Save_and_Close = 10;
        private const int _Enter_Appointment = 11;
        private const int _Delete_this_appointment_that_spans_more_than_one_day = 12;
        private const int _Delete_a_Span = 13;
        private const int _Delete_Span = 14;
        private const int _Edit_Recurring = 15;
        private const int _Make_Recurring = 16;

        private void InitStrings()
        {
            this.label1.Text = DisplayStrings[_Subject];
            this.label2.Text = DisplayStrings[_Location];
            this.label6.Text = DisplayStrings[_Show_time_as];
            this.reminderCheckBox.Text = DisplayStrings[_Reminder];
            this.allDayEventCheckBox.Text = DisplayStrings[_All_Day_Event];
            this.label5.Text = DisplayStrings[_End_Time];
            this.label4.Text = DisplayStrings[_Start_Time];
            this.label3.Text = DisplayStrings[_Label];
            this.subjectTextBox.Text = DisplayStrings[_subject];
            this.button2.Text = DisplayStrings[_Cancel];
            this.button1.Text = DisplayStrings[_Save_and_Close];
            this.Text = DisplayStrings[_Enter_Appointment];
        }

        private void PopulateTimeListBoxes(int minimumTimeSlot, string format)
        {
            int numberItems = 24 * 60 / minimumTimeSlot;
            object[] list = new object[numberItems];
            DateTime dt = DateTime.Now.Date;
            for (int i = 0; i < numberItems; i++)
            {
                list[i] = dt.AddMinutes(i * minimumTimeSlot).ToString(format);
            }

            this.endListBox.Items.AddRange(list);
            this.startListBox.Items.AddRange(list);
        }
        #endregion
    }
}
