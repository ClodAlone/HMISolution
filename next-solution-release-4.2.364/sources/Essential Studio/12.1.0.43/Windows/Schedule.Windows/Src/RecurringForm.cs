//-------------------------------------------------------------------------------------------------
// <copyright file="RecurringForm.cs" company="syncfusion">
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
using Syncfusion.Schedule;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Form which displays the recurring appointment information.
    /// </summary>
    public class RecurringForm : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public RecurringForm()
        {
            InitializeComponent();
            InitStrings();

            if (!IgnoreScheduleRTLSettings && ScheduleControl.isMirrored)
            {
                switchRTL(this.Controls);
            }

            helper = new RecurrenceSupport();
            HidePanel();
            ShowPanel();
        }
        private static bool isRecurringOnOverride = false;
        public bool IsRecurringOnOverride
        {
            get
            {
                return isRecurringOnOverride;
            }
            set
            {
                isRecurringOnOverride = value;
            }
        }
        #region RightToLeft support

        private void switchRTL(IEnumerable controls)
        {
            foreach (Control c in controls)
            {
                if (!(c is GroupBox))
                {
                    c.RightToLeft = RightToLeft.Yes;
                }

                switchRTL(c.Controls);
            }
        }

        private static bool ignoreScheduleRTLSettings = false;

        /// <summary>
        /// Gets or sets whether this form will  use the RTL settings from 
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
                if (!base.DesignMode && ScheduleControl.isMirrored)
                {
                    cp.ExStyle = cp.ExStyle | WS_EX_LAYOUTRTL | WS_EX_NOINHERITLAYOUT;
                }

                return cp;
            }
        }
        #endregion

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
            this.groupBox1 = new RTLGroupBox();
            this.panelWeekly = new RTLPanel();
            this.checkSat = new System.Windows.Forms.CheckBox();
            this.checkFri = new System.Windows.Forms.CheckBox();
            this.checkThu = new System.Windows.Forms.CheckBox();
            this.checkWed = new System.Windows.Forms.CheckBox();
            this.checkTue = new System.Windows.Forms.CheckBox();
            this.checkMon = new System.Windows.Forms.CheckBox();
            this.checkSun = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panelYearly = new RTLPanel();
            this.panel1 = new RTLPanel();
            this.radioButton14 = new System.Windows.Forms.RadioButton();
            this.radioButton13 = new System.Windows.Forms.RadioButton();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.comboBox9 = new System.Windows.Forms.ComboBox();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.panelQuarterly = new RTLPanel();
            this.panel4 = new RTLPanel();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.panelMonthly = new RTLPanel();
            this.panel3 = new RTLPanel();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.panelDaily = new RTLPanel();
            this.panel2 = new RTLPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioButton8 = new System.Windows.Forms.RadioButton();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new RTLGroupBox();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new RTLGroupBox();
            this.panel6 = new RTLPanel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.panel5 = new RTLPanel();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.radioButton15 = new System.Windows.Forms.RadioButton();
            this.radioButton16 = new System.Windows.Forms.RadioButton();
            this.radioButton17 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panelWeekly.SuspendLayout();
            this.panelYearly.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelQuarterly.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelMonthly.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panelDaily.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelWeekly);
            this.groupBox1.Controls.Add(this.panelMonthly);
            this.groupBox1.Controls.Add(this.panelDaily);
            this.groupBox1.Controls.Add(this.panelYearly);
            this.groupBox1.Controls.Add(this.panelQuarterly);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.radioButton5);
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(449, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recurrence Pattern";
            // 
            // panelWeekly
            // 
            this.panelWeekly.Controls.Add(this.checkSat);
            this.panelWeekly.Controls.Add(this.checkFri);
            this.panelWeekly.Controls.Add(this.checkThu);
            this.panelWeekly.Controls.Add(this.checkWed);
            this.panelWeekly.Controls.Add(this.checkTue);
            this.panelWeekly.Controls.Add(this.checkMon);
            this.panelWeekly.Controls.Add(this.checkSun);
            this.panelWeekly.Controls.Add(this.label3);
            this.panelWeekly.Controls.Add(this.label2);
            this.panelWeekly.Controls.Add(this.textBox2);
            this.panelWeekly.Location = new System.Drawing.Point(85, 26);
            this.panelWeekly.Name = "panelWeekly";
            this.panelWeekly.Size = new System.Drawing.Size(339, 100);
            this.panelWeekly.TabIndex = 6;
            // 
            // checkSat
            // 
            this.checkSat.Location = new System.Drawing.Point(158, 75);
            this.checkSat.Name = "checkSat";
            this.checkSat.Size = new System.Drawing.Size(87, 17);
            this.checkSat.TabIndex = 13;
            this.checkSat.Text = "Saturday";
            this.checkSat.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkFri
            // 
            this.checkFri.Location = new System.Drawing.Point(93, 75);
            this.checkFri.Name = "checkFri";
            this.checkFri.Size = new System.Drawing.Size(64, 17);
            this.checkFri.TabIndex = 12;
            this.checkFri.Text = "Friday";
            this.checkFri.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkThu
            // 
            this.checkThu.Location = new System.Drawing.Point(19, 75);
            this.checkThu.Name = "checkThu";
            this.checkThu.Size = new System.Drawing.Size(70, 17);
            this.checkThu.TabIndex = 11;
            this.checkThu.Text = "Thursday";
            this.checkThu.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkWed
            // 
            this.checkWed.Location = new System.Drawing.Point(231, 52);
            this.checkWed.Name = "checkWed";
            this.checkWed.Size = new System.Drawing.Size(95, 17);
            this.checkWed.TabIndex = 10;
            this.checkWed.Text = "Wednesday";
            this.checkWed.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkTue
            // 
            this.checkTue.Location = new System.Drawing.Point(158, 52);
            this.checkTue.Name = "checkTue";
            this.checkTue.Size = new System.Drawing.Size(72, 17);
            this.checkTue.TabIndex = 9;
            this.checkTue.Text = "Tuesday";
            this.checkTue.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkMon
            // 
            this.checkMon.Location = new System.Drawing.Point(93, 52);
            this.checkMon.Name = "checkMon";
            this.checkMon.Size = new System.Drawing.Size(64, 17);
            this.checkMon.TabIndex = 8;
            this.checkMon.Text = "Monday";
            this.checkMon.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // checkSun
            // 
            this.checkSun.Location = new System.Drawing.Point(19, 52);
            this.checkSun.Name = "checkSun";
            this.checkSun.Size = new System.Drawing.Size(62, 17);
            this.checkSun.TabIndex = 7;
            this.checkSun.Text = "Sunday";
            this.checkSun.Click += new System.EventHandler(this.checkBoxWeek_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Recur every";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(117, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "week(s)";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(85, 22);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(31, 20);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "1";
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPressNumericOnly);
            this.textBox2.Validating += new System.ComponentModel.CancelEventHandler(this.textBox2_Validating);
            // 
            // panelYearly
            // 
            this.panelYearly.Controls.Add(this.panel1);
            this.panelYearly.Controls.Add(this.comboBox7);
            this.panelYearly.Controls.Add(this.comboBox8);
            this.panelYearly.Controls.Add(this.comboBox9);
            this.panelYearly.Controls.Add(this.comboBox10);
            this.panelYearly.Controls.Add(this.label13);
            this.panelYearly.Controls.Add(this.label15);
            this.panelYearly.Controls.Add(this.textBox6);
            this.panelYearly.Location = new System.Drawing.Point(82, 29);
            this.panelYearly.Name = "panelYearly";
            this.panelYearly.Size = new System.Drawing.Size(345, 89);
            this.panelYearly.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton14);
            this.panel1.Controls.Add(this.radioButton13);
            this.panel1.Location = new System.Drawing.Point(12, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(48, 81);
            this.panel1.TabIndex = 2;
            // 
            // radioButton14
            // 
            this.radioButton14.Location = new System.Drawing.Point(7, 18);
            this.radioButton14.Name = "radioButton14";
            this.radioButton14.Size = new System.Drawing.Size(44, 17);
            this.radioButton14.TabIndex = 1;
            this.radioButton14.TabStop = true;
            this.radioButton14.Text = "Day";
            this.radioButton14.Click += new System.EventHandler(this.YearPanelButtons_Click);
            // 
            // radioButton13
            // 
            this.radioButton13.Location = new System.Drawing.Point(7, 48);
            this.radioButton13.Name = "radioButton13";
            this.radioButton13.Size = new System.Drawing.Size(44, 17);
            this.radioButton13.TabIndex = 2;
            this.radioButton13.Text = "The";
            this.radioButton13.Click += new System.EventHandler(this.YearPanelButtons_Click);
            // 
            // comboBox7
            // 
            this.comboBox7.Items.AddRange(new object[] {
            "January",
            "February",
            "March",
            "April",
            "may",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"});
            this.comboBox7.Location = new System.Drawing.Point(236, 49);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(84, 21);
            this.comboBox7.TabIndex = 14;
            this.comboBox7.Text = "January";
            this.comboBox7.Enter += new System.EventHandler(this.comboBox10_Enter);
            this.comboBox7.SelectedIndexChanged += new System.EventHandler(this.comboBox7_SelectedIndexChanged);
            // 
            // comboBox8
            // 
            this.comboBox8.Items.AddRange(new object[] {
            "January",
            "February",
            "March",
            "April",
            "may",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"});
            this.comboBox8.Location = new System.Drawing.Point(118, 20);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(84, 21);
            this.comboBox8.TabIndex = 13;
            this.comboBox8.Text = "January";
            this.comboBox8.Enter += new System.EventHandler(this.textBox6_Enter);
            this.comboBox8.SelectedIndexChanged += new System.EventHandler(this.comboBox8_SelectedIndexChanged);
            // 
            // comboBox9
            // 
            this.comboBox9.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.comboBox9.Location = new System.Drawing.Point(137, 49);
            this.comboBox9.Name = "comboBox9";
            this.comboBox9.Size = new System.Drawing.Size(76, 21);
            this.comboBox9.TabIndex = 12;
            this.comboBox9.Text = "Monday";
            this.comboBox9.Enter += new System.EventHandler(this.comboBox10_Enter);
            this.comboBox9.SelectedIndexChanged += new System.EventHandler(this.comboBox9_SelectedIndexChanged);
            // 
            // comboBox10
            // 
            this.comboBox10.Items.AddRange(new object[] {
            "first",
            "second",
            "third",
            "fourth"});
            this.comboBox10.Location = new System.Drawing.Point(69, 49);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(62, 21);
            this.comboBox10.TabIndex = 11;
            this.comboBox10.Text = "first";
            this.comboBox10.Enter += new System.EventHandler(this.comboBox10_Enter);
            this.comboBox10.SelectedIndexChanged += new System.EventHandler(this.comboBox10_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(217, 53);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(16, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "of";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(101, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 13);
            this.label15.TabIndex = 5;
            this.label15.Text = "of";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(69, 21);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(31, 20);
            this.textBox6.TabIndex = 4;
            this.textBox6.Text = "20";
            this.textBox6.Enter += new System.EventHandler(this.textBox6_Enter);
            this.textBox6.Validating += new System.ComponentModel.CancelEventHandler(this.textBox6_Validating);
            // 
            // panelQuarterly
            // 
            this.panelQuarterly.Controls.Add(this.panel4);
            this.panelQuarterly.Controls.Add(this.comboBox6);
            this.panelQuarterly.Controls.Add(this.comboBox5);
            this.panelQuarterly.Controls.Add(this.comboBox3);
            this.panelQuarterly.Controls.Add(this.comboBox4);
            this.panelQuarterly.Controls.Add(this.label8);
            this.panelQuarterly.Controls.Add(this.label9);
            this.panelQuarterly.Controls.Add(this.label10);
            this.panelQuarterly.Controls.Add(this.label11);
            this.panelQuarterly.Controls.Add(this.textBox8);
            this.panelQuarterly.Location = new System.Drawing.Point(84, 28);
            this.panelQuarterly.Name = "panelQuarterly";
            this.panelQuarterly.Size = new System.Drawing.Size(355, 89);
            this.panelQuarterly.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.radioButton12);
            this.panel4.Controls.Add(this.radioButton9);
            this.panel4.Location = new System.Drawing.Point(14, 18);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(49, 53);
            this.panel4.TabIndex = 2;
            // 
            // radioButton12
            // 
            this.radioButton12.Location = new System.Drawing.Point(3, 3);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(44, 17);
            this.radioButton12.TabIndex = 1;
            this.radioButton12.TabStop = true;
            this.radioButton12.Text = "Day";
            this.radioButton12.Click += new System.EventHandler(this.quarterPanelButtons_Click);
            // 
            // radioButton9
            // 
            this.radioButton9.Location = new System.Drawing.Point(3, 33);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(44, 17);
            this.radioButton9.TabIndex = 2;
            this.radioButton9.TabStop = true;
            this.radioButton9.Text = "The";
            this.radioButton9.Click += new System.EventHandler(this.quarterPanelButtons_Click);
            // 
            // comboBox6
            // 
            this.comboBox6.Items.AddRange(new object[] {
            "first",
            "second",
            "third"});
            this.comboBox6.Location = new System.Drawing.Point(248, 50);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(63, 21);
            this.comboBox6.TabIndex = 14;
            this.comboBox6.Text = "first";
            this.comboBox6.Enter += new System.EventHandler(this.comboBox4_Enter);
            this.comboBox6.SelectedIndexChanged += new System.EventHandler(this.comboBox6_SelectedIndexChanged);
            // 
            // comboBox5
            // 
            this.comboBox5.Items.AddRange(new object[] {
            "first",
            "second",
            "third"});
            this.comboBox5.Location = new System.Drawing.Point(133, 21);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(67, 21);
            this.comboBox5.TabIndex = 13;
            this.comboBox5.Text = "first";
            this.comboBox5.Enter += new System.EventHandler(this.textBox8_Enter);
            this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.comboBox5_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            this.comboBox3.Location = new System.Drawing.Point(133, 50);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(74, 21);
            this.comboBox3.TabIndex = 12;
            this.comboBox3.Text = "Monday";
            this.comboBox3.Enter += new System.EventHandler(this.comboBox4_Enter);
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // comboBox4
            // 
            this.comboBox4.Items.AddRange(new object[] {
            "first",
            "second",
            "third",
            "fourth"});
            this.comboBox4.Location = new System.Drawing.Point(64, 50);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(63, 21);
            this.comboBox4.TabIndex = 11;
            this.comboBox4.Text = "first";
            this.comboBox4.Enter += new System.EventHandler(this.comboBox4_Enter);
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(312, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "month";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(212, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "of the";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(204, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "month";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(99, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "of the";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(67, 22);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(31, 20);
            this.textBox8.TabIndex = 4;
            this.textBox8.Text = "20";
            this.textBox8.Enter += new System.EventHandler(this.textBox8_Enter);
            this.textBox8.Validating += new System.ComponentModel.CancelEventHandler(this.textBox8_Validating);
            // 
            // panelMonthly
            // 
            this.panelMonthly.Controls.Add(this.panel3);
            this.panelMonthly.Controls.Add(this.comboBox2);
            this.panelMonthly.Controls.Add(this.comboBox1);
            this.panelMonthly.Controls.Add(this.label6);
            this.panelMonthly.Controls.Add(this.textBox5);
            this.panelMonthly.Controls.Add(this.label7);
            this.panelMonthly.Controls.Add(this.label5);
            this.panelMonthly.Controls.Add(this.textBox4);
            this.panelMonthly.Controls.Add(this.label4);
            this.panelMonthly.Controls.Add(this.textBox3);
            this.panelMonthly.Location = new System.Drawing.Point(86, 27);
            this.panelMonthly.Name = "panelMonthly";
            this.panelMonthly.Size = new System.Drawing.Size(353, 86);
            this.panelMonthly.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.radioButton11);
            this.panel3.Controls.Add(this.radioButton10);
            this.panel3.Location = new System.Drawing.Point(11, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(50, 56);
            this.panel3.TabIndex = 2;
            // 
            // radioButton11
            // 
            this.radioButton11.Location = new System.Drawing.Point(4, 3);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(44, 17);
            this.radioButton11.TabIndex = 1;
            this.radioButton11.TabStop = true;
            this.radioButton11.Text = "Day";
            this.radioButton11.Click += new System.EventHandler(this.monthPanelButtons_Click);
            // 
            // radioButton10
            // 
            this.radioButton10.Location = new System.Drawing.Point(4, 33);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(44, 17);
            this.radioButton10.TabIndex = 2;
            this.radioButton10.TabStop = true;
            this.radioButton10.Text = "The";
            this.radioButton10.Click += new System.EventHandler(this.monthPanelButtons_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            this.comboBox2.Location = new System.Drawing.Point(138, 50);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(83, 21);
            this.comboBox2.TabIndex = 12;
            this.comboBox2.Text = "Monday";
            this.comboBox2.Enter += new System.EventHandler(this.comboBox1_Enter);
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Items.AddRange(new object[] {
            "first",
            "second",
            "third",
            "fourth"});
            this.comboBox1.Location = new System.Drawing.Point(65, 51);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(67, 21);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.Text = "first";
            this.comboBox1.Enter += new System.EventHandler(this.comboBox1_Enter);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(296, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "month(s)";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(263, 52);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(31, 20);
            this.textBox5.TabIndex = 9;
            this.textBox5.Text = "1";
            this.textBox5.Enter += new System.EventHandler(this.comboBox1_Enter);
            this.textBox5.Validating += new System.ComponentModel.CancelEventHandler(this.textBox5_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(219, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "of every ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(174, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "month(s)";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(141, 23);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(31, 20);
            this.textBox4.TabIndex = 6;
            this.textBox4.Text = "1";
            this.textBox4.Enter += new System.EventHandler(this.textBox3_Enter);
            this.textBox4.Validating += new System.ComponentModel.CancelEventHandler(this.textBox4_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "of every ";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(65, 23);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(31, 20);
            this.textBox3.TabIndex = 4;
            this.textBox3.Text = "20";
            this.textBox3.Enter += new System.EventHandler(this.textBox3_Enter);
            this.textBox3.Validating += new System.ComponentModel.CancelEventHandler(this.textBox3_Validating);
            // 
            // panelDaily
            // 
            this.panelDaily.Controls.Add(this.panel2);
            this.panelDaily.Controls.Add(this.radioButton8);
            this.panelDaily.Controls.Add(this.radioButton7);
            this.panelDaily.Controls.Add(this.radioButton6);
            this.panelDaily.Location = new System.Drawing.Point(85, 26);
            this.panelDaily.Name = "panelDaily";
            this.panelDaily.Size = new System.Drawing.Size(302, 109);
            this.panelDaily.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Location = new System.Drawing.Point(67, 14);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(85, 30);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "day(s)";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(31, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "1";
            this.textBox1.Enter += new System.EventHandler(this.textBox1_Enter);
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPressNumericOnly);
            this.textBox1.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            // 
            // radioButton8
            // 
            this.radioButton8.Location = new System.Drawing.Point(16, 81);
            this.radioButton8.Name = "radioButton8";
            this.radioButton8.Size = new System.Drawing.Size(102, 17);
            this.radioButton8.TabIndex = 3;
            this.radioButton8.TabStop = true;
            this.radioButton8.Text = "Every Weekend";
            this.radioButton8.Click += new System.EventHandler(this.dayPanelButtons_Click);
            // 
            // radioButton7
            // 
            this.radioButton7.Location = new System.Drawing.Point(16, 53);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(101, 17);
            this.radioButton7.TabIndex = 2;
            this.radioButton7.TabStop = true;
            this.radioButton7.Text = "Every Weekday";
            this.radioButton7.Click += new System.EventHandler(this.dayPanelButtons_Click);
            // 
            // radioButton6
            // 
            this.radioButton6.Location = new System.Drawing.Point(16, 23);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(55, 17);
            this.radioButton6.TabIndex = 1;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "Every ";
            this.radioButton6.Click += new System.EventHandler(this.dayPanelButtons_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(77, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(2, 112);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            // 
            // radioButton5
            // 
            this.radioButton5.Location = new System.Drawing.Point(5, 118);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(54, 17);
            this.radioButton5.TabIndex = 4;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "Yearly";
            this.radioButton5.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // radioButton4
            // 
            this.radioButton4.Location = new System.Drawing.Point(5, 95);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(83, 17);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Quarterly";
            this.radioButton4.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // radioButton3
            // 
            this.radioButton3.Location = new System.Drawing.Point(6, 72);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(62, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Monthly";
            this.radioButton3.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(6, 49);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(61, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Weekly";
            this.radioButton2.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.Location = new System.Drawing.Point(6, 26);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Daily";
            this.radioButton1.Click += new System.EventHandler(this.radioButton5_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel6);
            this.groupBox2.Controls.Add(this.panel5);
            this.groupBox2.Controls.Add(this.radioButton15);
            this.groupBox2.Controls.Add(this.radioButton16);
            this.groupBox2.Controls.Add(this.radioButton17);
            this.groupBox2.Location = new System.Drawing.Point(13, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(449, 147);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recurrence Range";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.dateTimePicker1);
            this.panel6.Controls.Add(this.label12);
            this.panel6.Location = new System.Drawing.Point(6, 31);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(142, 35);
            this.panel6.TabIndex = 21;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(38, 3);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(99, 20);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(2, 5);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Start:";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.textBox7);
            this.panel5.Controls.Add(this.label14);
            this.panel5.Controls.Add(this.dateTimePicker2);
            this.panel5.Location = new System.Drawing.Point(238, 54);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(142, 64);
            this.panel5.TabIndex = 20;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(4, 5);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(49, 20);
            this.textBox7.TabIndex = 15;
            this.textBox7.Text = "20";
            this.textBox7.Enter += new System.EventHandler(this.textBox7_Enter);
            this.textBox7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPressNumericOnly);
            this.textBox7.Validating += new System.ComponentModel.CancelEventHandler(this.textBox7_Validating);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(54, 8);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(63, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "recurrences";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(4, 36);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(99, 20);
            this.dateTimePicker2.TabIndex = 19;
            this.dateTimePicker2.Enter += new System.EventHandler(this.dateTimePicker2_Enter);
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // radioButton15
            // 
            this.radioButton15.Location = new System.Drawing.Point(171, 91);
            this.radioButton15.Name = "radioButton15";
            this.radioButton15.Size = new System.Drawing.Size(58, 17);
            this.radioButton15.TabIndex = 18;
            this.radioButton15.TabStop = true;
            this.radioButton15.Text = "End by";
            this.radioButton15.Click += new System.EventHandler(this.rangeSelectionButton_Click);
            // 
            // radioButton16
            // 
            this.radioButton16.Location = new System.Drawing.Point(171, 61);
            this.radioButton16.Name = "radioButton16";
            this.radioButton16.Size = new System.Drawing.Size(71, 17);
            this.radioButton16.TabIndex = 17;
            this.radioButton16.TabStop = true;
            this.radioButton16.Text = "End after ";
            this.radioButton16.Click += new System.EventHandler(this.rangeSelectionButton_Click);
            // 
            // radioButton17
            // 
            this.radioButton17.Location = new System.Drawing.Point(171, 31);
            this.radioButton17.Name = "radioButton17";
            this.radioButton17.Size = new System.Drawing.Size(87, 17);
            this.radioButton17.TabIndex = 16;
            this.radioButton17.TabStop = true;
            this.radioButton17.Text = "No end date.";
            this.radioButton17.Click += new System.EventHandler(this.rangeSelectionButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(119, 345);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.CausesValidation = false;
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(212, 345);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(304, 345);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(136, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Delete Recurring Event";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // RecurringForm
            // 
            this.AcceptButton = this.button1;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(479, 384);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "RecurringForm";
            this.Text = "Appointment Recurrence";
            this.groupBox1.ResumeLayout(false);
            this.panelWeekly.ResumeLayout(false);
            this.panelWeekly.PerformLayout();
            this.panelYearly.ResumeLayout(false);
            this.panelYearly.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelQuarterly.ResumeLayout(false);
            this.panelQuarterly.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panelMonthly.ResumeLayout(false);
            this.panelMonthly.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panelDaily.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private RTLGroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private RTLPanel panelDaily;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButton8;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton6;
        private RTLPanel panelWeekly;
        private System.Windows.Forms.CheckBox checkTue;
        private System.Windows.Forms.CheckBox checkMon;
        private System.Windows.Forms.CheckBox checkSun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox checkSat;
        private System.Windows.Forms.CheckBox checkFri;
        private System.Windows.Forms.CheckBox checkThu;
        private System.Windows.Forms.CheckBox checkWed;
        private RTLPanel panelMonthly;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.RadioButton radioButton10;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox4;
        private RTLPanel panelQuarterly;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton12;
        private RTLPanel panelYearly;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.ComboBox comboBox9;
        private System.Windows.Forms.ComboBox comboBox10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.RadioButton radioButton13;
        private System.Windows.Forms.RadioButton radioButton14;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.RadioButton radioButton15;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.RadioButton radioButton16;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton radioButton17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox7;
        private RTLPanel panel1;
        private RTLPanel panel2;
        private RTLPanel panel3;
        private RTLPanel panel4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private RTLGroupBox groupBox3;
        private RTLPanel panel5;
        private RTLPanel panel6;
        internal System.Windows.Forms.Button button3;
    
        internal DateTime defaultDate = DateTime.Now.Date;

        internal void RuleToLocalSettings(string rule)
        {   
         // set up empty display ....
            ////uncheck days of the week
            this.checkFri.Checked = false;
            this.checkSat.Checked = false;
            this.checkSun.Checked = false;
            this.checkMon.Checked = false;
            this.checkTue.Checked = false;
            this.checkWed.Checked = false;
            this.checkThu.Checked = false;

            ////make sure every panel has a checked Radio button
            this.radioButton6.Checked = true;
            this.radioButton11.Checked = true;
            this.radioButton12.Checked = true;
            this.radioButton14.Checked = true;
            this.radioButton17.Checked = true;

            this.radioButton6.Checked = true; ////dayButtons.every checkbox (1st checkbox on day panel)
            every = ParseTokens.DAY;

            ////set recurrencerange stuff
            start = defaultDate;
            rangeSelection = RangeButtons.NoEnd;
            this.dateTimePicker1.Value = start.Date;
           
            this.button3.Enabled = rule.Length > 0;

        //// end set up empty display ....

            if (rule.Length > 0)
            {
                string[] pieces = rule.Split(new char[] { RecurrenceSupport.RuleDelimiter });
                if (pieces.GetLength(0) < 3)
                {
                    return; ////not a valid string
                }

                int count = pieces.GetLength(0) - 2;
                RecurrenceRule r;
                for (int i = 0; i < count; ++i)
                {
                    r = helper.GetRecurrenceRule(pieces[2 + i]);
                    every = r.Every;
                    switch (r.Every)
                    {
                        case ParseTokens.WEEKDAY:
                        case ParseTokens.WEEKEND:
                        case ParseTokens.DAY:
                            this.radioButton1.Checked = true; ////daily
                            every = ParseTokens.DAY;
                            dayNumber = r.EveryCount;
                            dayButton = (r.Every == ParseTokens.DAY) ? DayButtons.every
                                : (r.Every == ParseTokens.WEEKEND ? DayButtons.everyWeekEnd : DayButtons.everyWeekday);
                            switch (dayButton)
                            {
                                case DayButtons.every:
                                    this.radioButton6.Checked = true;
                                    break;
                                case DayButtons.everyWeekday:
                                    this.radioButton7.Checked = true;
                                    break;
                                case DayButtons.everyWeekEnd:
                                    this.radioButton8.Checked = true;
                                    break;
                            }

                            this.textBox1.Text = dayNumber.ToString();
                            break;   
                        case ParseTokens.WEEK:
                            this.radioButton2.Checked = true; ////weekly
                            every = ParseTokens.WEEK;
                            weekNumber = r.EveryCount;
                            bool noneChecked = false;
                            switch (r.On)
                            {
                                case ParseTokens.SUN:
                                    this.checkSun.Checked = true;
                                    break;
                                case ParseTokens.MON:
                                    this.checkMon.Checked = true;
                                    break;
                                case ParseTokens.TUE:
                                    this.checkTue.Checked = true;
                                    break;
                                case ParseTokens.WED:
                                    this.checkWed.Checked = true;
                                    break;
                                case ParseTokens.THU:
                                    this.checkThu.Checked = true;
                                    break;
                                case ParseTokens.FRI:
                                    this.checkFri.Checked = true;
                                    break;
                                case ParseTokens.SAT:
                                    this.checkSat.Checked = true;
                                    break;
                                default:
                                    noneChecked = true;                                    
                                    break;
                            }

                            if (noneChecked)
                            {
                                this.checkMon.Checked = true;
                            }

                            this.textBox2.Text = weekNumber.ToString();
                            break;
                        case ParseTokens.MONTH:
                            this.radioButton3.Checked = true; ////monthly
                            every = ParseTokens.MONTH;

                            monthButton = r.On == ParseTokens.NULL ? MonthButtons.Day : MonthButtons.DayOfWeek;
                            switch (monthButton)
                            {
                                case MonthButtons.Day:
                                    this.radioButton11.Checked = true;
                                    break;
                                case MonthButtons.DayOfWeek:
                                    this.radioButton10.Checked = true;
                                    break;
                            }

                            monthNumber = monthButton == MonthButtons.Day ? r.EveryCount : 1;
                            monthDayNumber = monthButton == MonthButtons.Day ? r.OnCount : 20;
                            monthFirstValue = monthButton != MonthButtons.Day ? (PositionCount)Math.Max(r.OnCount - 1, 0) : PositionCount.first;
                            if (monthButton != MonthButtons.Day)
                            {
                                monthDayOfWeekValue = (MonthDayOfWeek)((int)r.On - (int)ParseTokens.MON);
                            }
                            else
                            {
                                monthDayOfWeekValue = MonthDayOfWeek.MON;
                            }

                            monthEveryValue = Math.Max(r.EveryCount, 1);
                            this.textBox3.Text = monthDayNumber.ToString();
                            this.textBox4.Text = monthNumber.ToString();
                            comboBox1.SelectedIndex = (int)monthFirstValue;
                            comboBox2.SelectedIndex = (int)monthDayOfWeekValue;
                            this.textBox5.Text = monthEveryValue.ToString();
                            break;
                        case ParseTokens.QUARTER:
                            this.radioButton4.Checked = true; ////quarterly
                            every = ParseTokens.QUARTER;

                            quarterButton = r.On == ParseTokens.NULL ? MonthButtons.Day : MonthButtons.DayOfWeek;
                            switch (quarterButton)
                            {
                                case MonthButtons.Day:
                                    this.radioButton12.Checked = true;
                                    break;
                                case MonthButtons.DayOfWeek:
                                    this.radioButton9.Checked = true;
                                    break;
                            }

                            quarterMonthNumber = quarterButton == MonthButtons.Day ? (PositionCount)Math.Max(r.BeforeAfterCount-1, 0) : PositionCount.first;
                            quarterDayNumber = quarterButton == MonthButtons.Day ? r.OnCount : 20;

                            quarterFirstValue = quarterButton != MonthButtons.Day ? (PositionCount)Math.Max(r.OnCount-1, 0) : PositionCount.first;
                            if (quarterButton != MonthButtons.Day)
                            {
                                quarterDayOfWeekValue = (MonthDayOfWeek)((int)r.On - (int)ParseTokens.MON);
                              
                                quarterMonthValue = (PositionCount)Math.Max(0, r.BeforeAfterCount-1); 
                            }
                            else
                            {
                                quarterMonthValue = PositionCount.first; 
                                quarterDayOfWeekValue = MonthDayOfWeek.MON;
                            }

                            this.textBox8.Text = quarterDayNumber.ToString();
                            this.comboBox5.SelectedIndex = (int)quarterMonthNumber;
                            this.comboBox4.SelectedIndex = (int)quarterFirstValue;
                            this.comboBox3.SelectedIndex = (int)quarterDayOfWeekValue;
                            this.comboBox6.SelectedIndex = (int)quarterMonthValue;
                            break;
                         case ParseTokens.YEAR:
                             this.radioButton1.Checked = true; ////yearly
                             every = ParseTokens.YEAR;

                             yearButton = r.On == ParseTokens.NULL ? MonthButtons.Day : MonthButtons.DayOfWeek;
                             switch (yearButton)
                             {
                                 case MonthButtons.Day:
                                     this.radioButton14.Checked = true;
                                     break;
                                 case MonthButtons.DayOfWeek:
                                     this.radioButton13.Checked = true;
                                     break;
                             }

                             if (yearButton == MonthButtons.Day)
                             {
                                 yearMonthNumber = (Months)MonthTokens.IndexOf(r.On.ToString()); ////maps to 0-11
                             }
                             else
                             {
                                 yearMonthNumber = Months.January;
                             }

                             yearDayNumber = yearButton == MonthButtons.Day ? r.OnCount : 20;

                             yearFirstValue = yearButton != MonthButtons.Day ? (PositionCount)Math.Max(r.OnCount-1, 0) : PositionCount.first;
                             if (yearButton != MonthButtons.Day)
                             {
                                 yearDayOfWeekValue = (MonthDayOfWeek)((int)r.On - (int)ParseTokens.MON);
                                 yearMonthValue = (Months)r.BeforeAfterCount;
                             }
                             else
                             {
                                 yearMonthValue = Months.January;
                                 yearDayOfWeekValue = MonthDayOfWeek.MON;
                             } 

                            break;
                        case ParseTokens.NULL:
                            break;
                        default:
                            break;
                    }
                }

                ////set up the Recurrence Range panel
                rangeSelection = pieces[1].Length > 0 ? RangeButtons.EndBy : RangeButtons.NoEnd;
                if (this.IsRecurringOnOverride)
                    start = DateTime.ParseExact(pieces[0], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                else
                    start = DateTime.Parse(pieces[0], System.Globalization.CultureInfo.InvariantCulture);
                if (rangeSelection == RangeButtons.EndBy)
                {
                    if (this.IsRecurringOnOverride)
                        end = DateTime.ParseExact(pieces[1], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    else
                        end = DateTime.Parse(pieces[1], System.Globalization.CultureInfo.InvariantCulture);
                    RecurrenceList dates = new RecurrenceList(pieces[2], start, null);
                    dates.IsValidRecurrence(end);
                    this.RecurrenceCount = dates.Count.ToString();
                    this.radioButton15.Checked = true; 
                }
                else
                {
                    end = start;
                    this.RecurrenceCount = "0";
                    this.radioButton17.Checked = true; 
                }

                this.textBox7.Text = this.RecurrenceCount;
                this.dateTimePicker1.Value = start.Date;
                this.dateTimePicker2.Value = end.Date;
            }
        }

        private void CheckWeekDays(bool check, string day, ref string s)
        {
            if (check)
            {
                if (s.Length > 0)
                {
                    s += RecurrenceSupport.RuleDelimiter;
                }

                s += string.Format("Every WEEK on {0}", day);
            }
        }

        internal string LocalSettingsToRuleString()
        {
            return LocalSettingsToRuleString(true);
        }
        
        internal string LocalSettingsToRuleString(bool includeDates)
        {
            string rule = "Every " + Enum.GetName(typeof(ParseTokens), every) + " ";

            switch (every)
            {
                case ParseTokens.DAY:
                case ParseTokens.WEEKDAY:
                case ParseTokens.WEEKEND:
                    switch (dayButton)
                    {
                        case DayButtons.every:
                            rule = string.Format("Every DAY:{0}", dayNumber);
                            break;
                        case DayButtons.everyWeekday:
                            rule = "Every WEEKDAY";
                            break;
                        case DayButtons.everyWeekEnd:
                            rule = "Every WEEKEND";
                            break;
                        default:
                            break;
                    }

                   break;
                case ParseTokens.WEEK:
                    string s = string.Empty;
                    CheckWeekDays(checkSun.Checked, "SUN", ref s);
                    CheckWeekDays(checkMon.Checked, "MON", ref s);
                    CheckWeekDays(checkTue.Checked, "TUE", ref s);
                    CheckWeekDays(checkWed.Checked, "WED", ref s);
                    CheckWeekDays(checkThu.Checked, "THU", ref s);
                    CheckWeekDays(checkFri.Checked, "FRI", ref s);
                    CheckWeekDays(checkSat.Checked, "SAT", ref s);
                    if (s.Length == 0)
                    {
                        CheckWeekDays(true, "MON", ref s); ////must check something
                    }

                    if (weekNumber > 1)
                    {
                        s = s.Replace("WEEK on", string.Format("WEEK:{0} on", weekNumber));
                    }

                    rule = s;
                    break;
                case ParseTokens.MONTH:
                    switch (monthButton)
                    {
                        case MonthButtons.Day:
                            rule = "Every MONTH" + ((monthNumber > 1) ? string.Format(":{0} on {1}", monthNumber, monthDayNumber) 
                                                                   : string.Format(" on {0}", monthDayNumber));
                            break;
                        case MonthButtons.DayOfWeek:
                            rule = "Every MONTH" + ((monthEveryValue > 1) ? string.Format(":{0} on ", monthEveryValue)
                                                                   : " on ");
                            string day = Enum.GetName(typeof(MonthDayOfWeek), monthDayOfWeekValue);
                            rule = rule + (((int)monthFirstValue) > 0 ? string.Format("{1}:{0}", 1 + (int)monthFirstValue, day)
                                                                   : day);
                            break;
                    }

                    break;
                case ParseTokens.QUARTER:
                    switch (quarterButton)
                    {
                        case MonthButtons.Day:
                            rule = string.Format("Every QUARTER on {0} after MONTH:{1}", quarterDayNumber, 1 + (int)quarterMonthNumber);
                            break;
                        case MonthButtons.DayOfWeek:
                            string day = Enum.GetName(typeof(MonthDayOfWeek), quarterDayOfWeekValue);

                            rule = string.Format("Every QUARTER on {0}:{1} after MONTH:{2}", day, 1 + (int)quarterFirstValue, 1 + (int)quarterMonthValue);
                            break;
                    }

                   break;
                case ParseTokens.YEAR:
                    switch (yearButton)
                    {
                        case MonthButtons.Day:
                            rule = string.Format("Every YEAR on {0} {1}", MonthTokens.Substring(3 *(int)yearMonthNumber, 3), yearDayNumber);
                            break;
                        case MonthButtons.DayOfWeek:
                            string day = Enum.GetName(typeof(MonthDayOfWeek), yearDayOfWeekValue);

                            rule = string.Format("Every YEAR on {0}:{1} after {2}", day, 1 + (int)yearFirstValue, MonthTokens.Substring(3 * (int)yearMonthNumber, 3));
                            break;
                    }

                    break;
                  
                default:
                    break;
            }

            if (includeDates)
            {
                ////set up the Recurrence Range panel
                 string dates;
                 if (this.IsRecurringOnOverride)
                     dates = start.ToString("dd.MM.yyyy"); //start.ToString("d", System.Globalization.CultureInfo.InvariantCulture);
                 else
                     dates = start.ToString("d", System.Globalization.CultureInfo.InvariantCulture);

                 if (rangeSelection != RangeButtons.NoEnd)
                 {
                     if (this.IsRecurringOnOverride)
                         dates += RecurrenceSupport.RuleDelimiter + end.ToString("dd.MM.yyyy");
                     else
                         dates += RecurrenceSupport.RuleDelimiter + end.ToString("d", System.Globalization.CultureInfo.InvariantCulture);
                 }
                 else
                 {
                     dates += RecurrenceSupport.RuleDelimiter;
                 }

                rule = dates + RecurrenceSupport.RuleDelimiter + rule;
            }

            return rule;
        }

        private void UpdateDisplayAfterChange()
        {
            if (rangeSelection != RangeButtons.NoEnd)
            {
                string s = LocalSettingsToRuleString(false);
                if (s.Length > 0)
                {
                    RecurrenceList dates = new RecurrenceList(s, start, null);
                    if (rangeSelection == RangeButtons.EndAfter)
                    {
                        int count = int.Parse(this.RecurrenceCount);
                        dates.IsValidRecurrence(start);
                        while (dates.Count < count)
                        {
                           dates.IsValidRecurrence(dates[dates.Count - 1].AddDays(1));
                        }

                        end = dates[dates.Count - 1];
                    }
                    else
                    {
                        dates.IsValidRecurrence(end);
                        this.RecurrenceCount = dates.Count.ToString();
                    }
                }
                else
                {
                    this.RecurrenceCount = "0";
                }

                this.textBox7.Text = this.RecurrenceCount;
                this.dateTimePicker1.Value = start.Date;
                this.dateTimePicker2.Value = end.Date;
            }
        }

        ////used by all textboxes to restrict to digits only
        private void textBox_KeyPressNumericOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private int GetIntegerFromTextBox(TextBox tb)
        {
            double d;
            int i = 0;
            if (double.TryParse(tb.Text, System.Globalization.NumberStyles.Integer, null, out d))
            {
                i = (int)d;
            }
            else
            {
                ////error state ... ignore 
            }

            return i;
        }

        private string monthTokens;

        private string MonthTokens
        {
            get
            {
                if (monthTokens == null || monthTokens.Length == 0)
                {
                    monthTokens = ParseTokens.JAN.ToString() + ParseTokens.FEB.ToString() + ParseTokens.MAR.ToString() + ParseTokens.APR.ToString()
                             + ParseTokens.MAY.ToString() + ParseTokens.JUN.ToString() + ParseTokens.JUL.ToString() + ParseTokens.AUG.ToString()
                             + ParseTokens.SEP.ToString() + ParseTokens.OCT.ToString() + ParseTokens.NOV.ToString() + ParseTokens.DEC.ToString();
                }
            
                return monthTokens; 
            }

            set
            {
                monthTokens = value;
            }
        }

        private RecurrenceSupport helper;
        ParseTokens every = ParseTokens.DAY;
        
        bool inevent = false;

        #region button clicks and textbox changes

        #region main group of radio buttons on the left

        ////radio buttons 1,2,3,4,5
        private void radioButton5_Click(object sender, EventArgs e)
        {
            if (!inevent)
            {
                inevent = true;
                HidePanel();
                ShowPanel();
                inevent = false;
            }
        }

        private void ShowPanel()
        {
            ////daily
            if (this.radioButton1.Checked && !panelDaily.Visible) 
            {
                panelDaily.Visible = true;
                
                every = ParseTokens.DAY;
            }
            else if (this.radioButton2.Checked && !panelWeekly.Visible) 
            {
                ////weekly
                panelWeekly.Visible = true;
                every = ParseTokens.WEEK;
            }
            else if (this.radioButton3.Checked && !panelMonthly.Visible) 
            {
                ////monthly
                panelMonthly.Visible = true;
                every = ParseTokens.MONTH;
            }
            else if (this.radioButton4.Checked && !panelQuarterly.Visible) 
            {
                ////quarterly
                panelQuarterly.Visible = true;
                every = ParseTokens.QUARTER;
            }
            else if (this.radioButton5.Checked && !panelYearly.Visible) 
            {
                ////yearly
                panelYearly.Visible = true;
                every = ParseTokens.YEAR;
            }

            this.start = this.dateTimePicker1.Value;
            this.end = this.dateTimePicker2.Value;

            UpdateDisplayAfterChange();
        }

        private void HidePanel()
        {
            panelDaily.Visible = false;
            panelWeekly.Visible = false;
            panelMonthly.Visible = false;
            panelQuarterly.Visible = false;
            panelYearly.Visible = false;
        }
        
        #endregion

        #region Day Panel code
        
        /// <summary>
        /// DayButtons enumeration.
        /// </summary>
        private enum DayButtons
        {
            /// <summary>
            /// Every day.
            /// </summary>
            every,
           
            /// <summary>
            /// Every week day.
            /// </summary>
            everyWeekday,
           
            /// <summary>
            /// Every week end.
            /// </summary>
            everyWeekEnd
        }

        DayButtons dayButton = DayButtons.every;
        int dayNumber = 1;

        ////readio button 6, 7 and 8
        private void dayPanelButtons_Click(object sender, EventArgs e)
        {
            dayButton = radioButton6.Checked ? DayButtons.every : (radioButton7.Checked ? DayButtons.everyWeekday : DayButtons.everyWeekEnd);
            UpdateDisplayAfterChange();
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            dayNumber = GetIntegerFromTextBox(textBox1);
            UpdateDisplayAfterChange();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            radioButton6.Checked = true;
            dayButton = DayButtons.every;
        }

        #endregion

        #region Week Panel code
        int weekNumber = 1;

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            weekNumber = GetIntegerFromTextBox(textBox2);
            UpdateDisplayAfterChange();
        }

        private void checkBoxWeek_Click(object sender, EventArgs e)
        {
            UpdateDisplayAfterChange();
        }

        #endregion

        #region Month Panel code
        private enum MonthButtons
        {
            /// <summary>
            /// The day button.
            /// </summary>
            Day,
            
            /// <summary>
            /// Day of the week button.
            /// </summary>
            DayOfWeek
        }

        private enum PositionCount
        {
            /// <summary>
            /// First position.
            /// </summary>
            first,
            
            /// <summary>
            /// Second position.
            /// </summary>
            second,
            
            /// <summary>
            /// Third position.
            /// </summary>
            third,
            
            /// <summary>
            /// Fourth position.
            /// </summary>
            fourth
        }

        private enum MonthDayOfWeek
        {
            /// <summary>
            /// Monday of the week.
            /// </summary>
            MON,
           
            /// <summary>
            /// Tuesday of the week.
            /// </summary>
            TUE,
           
            /// <summary>
            /// Wednesday of the week.
            /// </summary>
            WED,
            
            /// <summary>
            /// Thursday of the week.
            /// </summary>
            THU,
            
            /// <summary>
            /// Friday of the week.
            /// </summary>
            FRI,
            
            /// <summary>
            /// Saturday of the week.
            /// </summary>
            SAT,
            
            /// <summary>
            /// Sunday of the week.
            /// </summary>
            SUN
        }

        MonthButtons monthButton = MonthButtons.Day;
        int monthNumber = 1;
        int monthDayNumber = 20;

        PositionCount monthFirstValue = PositionCount.first;
        MonthDayOfWeek monthDayOfWeekValue = MonthDayOfWeek.MON;
        int monthEveryValue = 1;

        ////radioButton 10 and 11.
        private void monthPanelButtons_Click(object sender, EventArgs e)
        {
            monthButton = radioButton11.Checked ? MonthButtons.Day : MonthButtons.DayOfWeek;
            UpdateDisplayAfterChange();
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
           monthDayNumber = GetIntegerFromTextBox(textBox3);
           UpdateDisplayAfterChange();
        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {
            monthNumber = GetIntegerFromTextBox(textBox4);
            UpdateDisplayAfterChange();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            monthFirstValue = (PositionCount)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            monthDayOfWeekValue = (MonthDayOfWeek)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void textBox5_Validating(object sender, CancelEventArgs e)
        {
            monthEveryValue = GetIntegerFromTextBox(textBox5);
            UpdateDisplayAfterChange();
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            this.radioButton11.Checked = true;
            monthButton = MonthButtons.Day;
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            this.radioButton10.Checked = true;
            monthButton = MonthButtons.DayOfWeek;
        }

        #endregion

        #region Quarter Panel code
        
        MonthButtons quarterButton = MonthButtons.Day;
        PositionCount quarterMonthNumber = PositionCount.first;
        int quarterDayNumber = 20;

        PositionCount quarterFirstValue = PositionCount.first;
        MonthDayOfWeek quarterDayOfWeekValue = MonthDayOfWeek.MON;
        PositionCount quarterMonthValue = PositionCount.first;

        ////button 9 and 12
        private void quarterPanelButtons_Click(object sender, EventArgs e)
        {
            quarterButton = radioButton12.Checked ? MonthButtons.Day : MonthButtons.DayOfWeek;
            UpdateDisplayAfterChange();
        }

        private void textBox8_Validating(object sender, CancelEventArgs e)
        {
            quarterDayNumber = GetIntegerFromTextBox(textBox8);
            UpdateDisplayAfterChange();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            quarterMonthNumber = (PositionCount)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            quarterFirstValue = (PositionCount)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            quarterDayOfWeekValue = (MonthDayOfWeek)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            quarterMonthValue = (PositionCount)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void textBox8_Enter(object sender, EventArgs e)
        {
            this.radioButton12.Checked = true;
            quarterButton = MonthButtons.Day;
        }

        private void comboBox4_Enter(object sender, EventArgs e)
        {
            this.radioButton9.Checked = true;
            quarterButton = MonthButtons.DayOfWeek;
        }

        #endregion

        #region Year Panel code

        private enum Months
        {
            /// <summary>
            /// The month of January.
            /// </summary>
            January,
           
            /// <summary>
            /// The month of February.
            /// </summary>
            February,
            
            /// <summary>
            /// The month of March.
            /// </summary>
            March,
            
            /// <summary>
            /// The month of April.
            /// </summary>
            April,
            
            /// <summary>
            /// The month of May.
            /// </summary>
            May,
            
            /// <summary>
            /// The month of June.
            /// </summary>
            June,
            
            /// <summary>
            /// The month of July.
            /// </summary>
            July,
            
            /// <summary>
            /// The month of August.
            /// </summary>
            August,
            
            /// <summary>
            /// The month of September.
            /// </summary>
            September,
            
            /// <summary>
            /// The month of October.
            /// </summary>
            October,
            
            /// <summary>
            /// The month of November.
            /// </summary>
            November,
            
            /// <summary>
            /// The month of December.
            /// </summary>
            December
        }

        MonthButtons yearButton = MonthButtons.Day;
        Months yearMonthNumber = Months.January;
        int yearDayNumber = 20;

        PositionCount yearFirstValue = PositionCount.first;
        MonthDayOfWeek yearDayOfWeekValue = MonthDayOfWeek.MON;
        Months yearMonthValue = Months.January;

        ////button 14 and 13
        private void YearPanelButtons_Click(object sender, EventArgs e)
        {
            yearButton = radioButton14.Checked ? MonthButtons.Day : MonthButtons.DayOfWeek;
            UpdateDisplayAfterChange();
        }

        private void textBox6_Validating(object sender, CancelEventArgs e)
        {
            yearDayNumber = GetIntegerFromTextBox(textBox6);
            UpdateDisplayAfterChange();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            yearMonthNumber = (Months)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            yearFirstValue = (PositionCount)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            yearDayOfWeekValue = (MonthDayOfWeek)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            yearMonthValue = (Months)cb.SelectedIndex;
            UpdateDisplayAfterChange();
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            this.radioButton14.Checked = true;
            yearButton = MonthButtons.Day;
        }

        private void comboBox10_Enter(object sender, EventArgs e)
        {
            this.radioButton13.Checked = true;
            yearButton = MonthButtons.DayOfWeek;
        }

        #endregion

        #region Recurrence Range Panel
        DateTime start;
        DateTime end;

        private enum RangeButtons
        {
            /// <summary>
            /// No end for the recurrence.
            /// </summary>
            NoEnd,
            
            /// <summary>
            /// End after the specified date.
            /// </summary>
            EndAfter,
            
            /// <summary>
            /// End by the date specified.
            /// </summary>
            EndBy
        }

        private RangeButtons rangeSelection = RangeButtons.NoEnd;

        ////button 17, 16, 15
        private void rangeSelectionButton_Click(object sender, EventArgs e)
        {
            rangeSelection = radioButton17.Checked ? RangeButtons.NoEnd :
                (radioButton16.Checked ? RangeButtons.EndAfter : RangeButtons.EndBy);
            UpdateDisplayAfterChange();
        }

        private string recurrenceCount = "20";

        private string RecurrenceCount
        {
            get { return recurrenceCount; }
            set { recurrenceCount = value; }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker tp = sender as DateTimePicker;
            this.start = tp.Value;
            UpdateDisplayAfterChange();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker tp = sender as DateTimePicker;
            this.end = tp.Value;
            rangeSelection = RangeButtons.EndBy;
            UpdateDisplayAfterChange();
        }

        private void textBox7_Validating(object sender, CancelEventArgs e)
        {
            recurrenceCount = this.textBox7.Text;
            rangeSelection = RangeButtons.EndAfter;
            UpdateDisplayAfterChange();
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            rangeSelection = RangeButtons.EndAfter;
            this.radioButton16.Checked = true;
        }

        private void dateTimePicker2_Enter(object sender, EventArgs e)
        {
            rangeSelection = RangeButtons.EndBy;
            this.radioButton15.Checked = true;
        }

        #endregion

        ////OK button
        private void button1_Click(object sender, EventArgs e)
        {
               this.Close();
                this.DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(DisplayStrings[_Confirm_the_deletion_of_all_occurrences], DisplayStrings[_Delete_Confirmation], MessageBoxButtons.YesNo)
                == DialogResult.Yes)
            {
                this.Close();
                this.DialogResult = DialogResult.Abort;
            }
        }

        #endregion

        #region Strings

        /// <summary>
        /// Holds various strings used in the RecurringForm.
        /// </summary>
        /// <remarks>
        /// Modifying the strings in this array will affect the display of the RecurringForm.
        /// You can use these strings to localize the labels that appear on the form.
        /// Please note that some of the strings used in this form represent tokens and
        /// parseable elements which cannot be localized.
        /// </remarks>
        /// <example>
        /// Here are the default values of this string array.
        /// <code lang="C#">
        /// public static string[] DisplayStrings = new string[]
        ///                        {       "Confirm the deletion of all occurrences?", // 0
        ///                                "Delete Confirmation",                   // 1
        ///                                "Recurrence Pattern",                    // 2
        ///                                "Recurrence Range",                      // 3
        ///                                "OK",                                    // 4
        ///                                "Cancel",                                // 5
        ///                                "Delete Recurring Event",                // 6
        ///                                "Appointment Recurrence",                // 7
        ///                                 "Recur every",                          // 8
        ///                                "week(s)",                               // 9
        ///                                "of",                                    // 10
        ///                                "month",                                 // 11
        ///                                "of the",                                // 12
        ///                                "month(s)",                              // 13
        ///                                "of every ",                             // 14
        ///                                "day(s)",                                // 15
        ///                                "Start:",                                // 16
        ///                                "recurrences"                            // 17
        ///                        };
        /// </code>
        /// </example>
        public static string[] DisplayStrings = new string[]
                                {
                                        "Confirm the deletion of all occurrences?", 
                                        "Delete Confirmation",
                                        "Recurrence Pattern",
                                        "Recurrence Range",
                                        "OK",
                                        "Cancel",
                                        "Delete Recurring Event",
                                        "Appointment Recurrence",
                                        "Recur every",
                                        "week(s)",
                                        "of",
                                        "month",
                                        "of the",
                                        "month(s)",
                                        "of every ",
                                        "day(s)",
                                        "Start:",
                                        "recurrences"
                                };

        private const int _Confirm_the_deletion_of_all_occurrences = 0;
        private const int _Delete_Confirmation = 1;
        private const int _Recurrence_Pattern = 2;
        private const int _Recurrence_Range = 3;
        private const int _OK = 4;
        private const int _Cancel = 5;
        private const int _Delete_Recurring_Event = 6;
        private const int _Appointment_Recurrence = 7;
        private const int _Recur_every = 8;
        private const int _Week_s_ = 9;
        private const int _Of = 10;
        private const int _Month = 11;
        private const int _Of_the = 12;
        private const int _Month_s_ = 13;
        private const int _Of_every_ = 14;
        private const int _Day_s_ = 15;
        private const int _Start_ = 16;
        private const int _Recurrences = 17;

        private void InitStrings()
        {
            this.groupBox1.Text = DisplayStrings[_Recurrence_Pattern];
            this.groupBox2.Text = DisplayStrings[_Recurrence_Range];
            this.button1.Text = DisplayStrings[_OK];
            this.button2.Text = DisplayStrings[_Cancel];
            this.button3.Text = DisplayStrings[_Delete_Recurring_Event];
            this.Text = DisplayStrings[_Appointment_Recurrence];

            this.label3.Text = DisplayStrings[_Recur_every];
            this.label2.Text = DisplayStrings[_Week_s_];
            this.label13.Text = DisplayStrings[_Of];
            this.label15.Text = DisplayStrings[_Of];
            this.label8.Text = DisplayStrings[_Month];
            this.label9.Text = DisplayStrings[_Of_the];
            this.label10.Text = DisplayStrings[_Month];
            this.label11.Text = DisplayStrings[_Of_the];
            this.label6.Text = DisplayStrings[_Month_s_];
            this.label7.Text = DisplayStrings[_Of_every_];
            this.label5.Text = DisplayStrings[_Month_s_];
            this.label4.Text = DisplayStrings[_Of_every_];
            this.label1.Text = DisplayStrings[_Day_s_];
            this.label12.Text = DisplayStrings[_Start_];
            this.label14.Text = DisplayStrings[_Recurrences];
        }

        #endregion
    }
}