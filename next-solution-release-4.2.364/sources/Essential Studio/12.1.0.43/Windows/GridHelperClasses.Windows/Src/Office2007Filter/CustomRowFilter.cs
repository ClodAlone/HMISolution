#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Summary description for CustomRowFilter.
    /// </summary>
    internal class CustomRowFilter : Syncfusion.Windows.Forms.Office2007Form
    {
        public Syncfusion.Windows.Forms.Tools.ComboBoxAdv comboBox1;
        public Syncfusion.Windows.Forms.Tools.ComboBoxAdv comboBox2;
        private System.Windows.Forms.Panel panel1;
        private Syncfusion.Windows.Forms.Tools.RadioButtonAdv radioButton1;
        private Syncfusion.Windows.Forms.Tools.RadioButtonAdv radioButton2;
        private Syncfusion.Windows.Forms.ButtonAdv button1;
        private Syncfusion.Windows.Forms.ButtonAdv button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        internal string compareText1, compareText2;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv compareList1;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv compareList2;
        private Button button4;
        private Button button3;
        private PopupControlContainer popupControlContainer1;
        private PopupControlContainer popupControlContainer2;
        private MonthCalendarAdv monthCalendar1;
        private MonthCalendarAdv monthCalendar2;
        internal int filterNumber = 0;
        private IContainer components;
        private object[] uniqueList;
        Image datePickerImg;
        private Label label3;
        private string styles;

        public CustomRowFilter(string styles)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.comboBox1.DataSource = GetComboTable();
            this.comboBox1.DisplayMember = "FilterString";
            this.comboBox1.ValueMember = "ValueString";
            this.comboBox2.DataSource = GetComboTable();
            this.comboBox2.DisplayMember = "FilterString";
            this.comboBox2.ValueMember = "ValueString";
            datePickerImg = (Image)DynamicFilterBitmaps.GetBitmap("datepicker");
            this.button3.Image = datePickerImg;
            this.button3.ImageAlign = ContentAlignment.MiddleRight;
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button4.Image = datePickerImg;
            this.button4.ImageAlign = ContentAlignment.MiddleRight;
            this.button4.FlatStyle = FlatStyle.Flat;
            this.button4.BackColor = Color.Transparent;
            this.styles = styles;
        }

        public CustomRowFilter(string styles, object[] uniqueList)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.comboBox1.DataSource = GetComboTable();
            this.comboBox1.DisplayMember = "FilterString";
            this.comboBox1.ValueMember = "ValueString";
            this.comboBox2.DataSource = GetComboTable();
            this.comboBox2.DisplayMember = "FilterString";
            this.comboBox2.ValueMember = "ValueString";
            datePickerImg = (Image)DynamicFilterBitmaps.GetBitmap("datepicker");
            this.button3.Image = datePickerImg;
            this.button3.ImageAlign = ContentAlignment.MiddleRight;
            this.button3.FlatStyle = FlatStyle.Flat;
            this.button4.Image = datePickerImg;
            this.button4.ImageAlign = ContentAlignment.MiddleRight;
            this.button4.FlatStyle = FlatStyle.Flat;
            this.styles = styles;
            this.uniqueList = uniqueList;
            this.compareList1.DataSource = uniqueList;
            this.compareList2.DataSource = uniqueList.Clone();
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
            this.comboBox1 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.comboBox2 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton2 = new Syncfusion.Windows.Forms.Tools.RadioButtonAdv();
            this.radioButton1 = new Syncfusion.Windows.Forms.Tools.RadioButtonAdv();
            this.button1 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.button2 = new Syncfusion.Windows.Forms.ButtonAdv();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.compareList1 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.compareList2 = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.monthCalendar1 = new Syncfusion.Windows.Forms.Tools.MonthCalendarAdv();
            this.popupControlContainer2 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.monthCalendar2 = new Syncfusion.Windows.Forms.Tools.MonthCalendarAdv();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareList1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareList2)).BeginInit();
            this.popupControlContainer1.SuspendLayout();
            this.popupControlContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.Location = new System.Drawing.Point(28, 58);
            this.comboBox1.MaxDropDownItems = 4;
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(168, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            // 
            // comboBox2
            // 
            this.comboBox2.BeforeTouchSize = new System.Drawing.Size(168, 21);
            this.comboBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.Location = new System.Drawing.Point(28, 104);
            this.comboBox2.MaxDropDownItems = 4;
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(168, 21);
            this.comboBox2.TabIndex = 2;
            this.comboBox2.DropDown += new System.EventHandler(this.comboBox2_DropDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(28, 79);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(168, 21);
            this.panel1.TabIndex = 4;
            // 
            // radioButton2
            // 
            this.radioButton2.BeforeTouchSize = new System.Drawing.Size(150, 21);
            this.radioButton2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(79, 3);
            this.radioButton2.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(89)))), ((int)(((byte)(91)))));
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(48, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = SR.GetString(SR.Office2007FilterCustomFilteror);
            // 
            // radioButton1
            // 
            this.radioButton1.BeforeTouchSize = new System.Drawing.Size(150, 21);
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(25, 3);
            this.radioButton1.MetroColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(89)))), ((int)(((byte)(91)))));
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = SR.GetString(SR.Office2007FilterCustomFilterand);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.IsBackStageButton = false;
            this.button1.Location = new System.Drawing.Point(183, 141);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 24);
            this.button1.TabIndex = 5;
            this.button1.Text = SR.GetString(SR.CustomAutoFilterOK);
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.IsBackStageButton = false;
            this.button2.Location = new System.Drawing.Point(287, 141);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 24);
            this.button2.TabIndex = 6;
            this.button2.Text = SR.GetString(SR.CustomAutoFilterCancel);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(331, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = SR.GetString(SR.ShowRowsWhere);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(216, 16);
            this.label2.TabIndex = 8;
            // 
            // compareList1
            // 
            this.compareList1.BeforeTouchSize = new System.Drawing.Size(157, 21);
            this.compareList1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compareList1.Location = new System.Drawing.Point(202, 58);
            this.compareList1.MaxDropDownItems = 4;
            this.compareList1.Name = "compareList1";
            this.compareList1.Size = new System.Drawing.Size(157, 21);
            this.compareList1.TabIndex = 9;
            // 
            // compareList2
            // 
            this.compareList2.BeforeTouchSize = new System.Drawing.Size(157, 21);
            this.compareList2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.compareList2.Location = new System.Drawing.Point(202, 104);
            this.compareList2.MaxDropDownItems = 4;
            this.compareList2.Name = "compareList2";
            this.compareList2.Size = new System.Drawing.Size(157, 21);
            this.compareList2.TabIndex = 10;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(365, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(32, 21);
            this.button4.TabIndex = 11;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(365, 104);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(32, 21);
            this.button3.TabIndex = 12;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // popupControlContainer1
            // 
            this.popupControlContainer1.Controls.Add(this.monthCalendar1);
            this.popupControlContainer1.Location = new System.Drawing.Point(365, 123);
            this.popupControlContainer1.Name = "popupControlContainer1";
            this.popupControlContainer1.Size = new System.Drawing.Size(226, 162);
            this.popupControlContainer1.TabIndex = 13;
            this.popupControlContainer1.Visible = false;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthCalendar1.Location = new System.Drawing.Point(0, 0);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateChanged += new EventHandler(monthCalendar1_DateChanged);
            this.monthCalendar1.MouseLeave += new EventHandler(this.monthCalendar1_MouseLeave);
            // 
            // popupControlContainer2
            // 
            this.popupControlContainer2.Controls.Add(this.monthCalendar2);
            this.popupControlContainer2.Location = new System.Drawing.Point(365, 77);
            this.popupControlContainer2.Name = "popupControlContainer2";
            this.popupControlContainer2.Size = new System.Drawing.Size(228, 161);
            this.popupControlContainer2.TabIndex = 14;
            this.popupControlContainer2.Visible = false;
            // 
            // monthCalendar2
            // 
            this.monthCalendar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthCalendar2.Location = new System.Drawing.Point(0, 0);
            this.monthCalendar2.Name = "monthCalendar2";
            this.monthCalendar2.TabIndex = 0;
            this.monthCalendar2.DateChanged += new EventHandler(this.monthCalendar2_DateChanged);
            this.monthCalendar2.MouseLeave += new System.EventHandler(this.monthCalendar2_MouseLeave);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(202, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 0);
            this.label3.TabIndex = 15;
            // 
            // CustomRowFilter
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(400, 287);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.popupControlContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.popupControlContainer2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.compareList2);
            this.Controls.Add(this.compareList1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomRowFilter";
            this.Text = SR.GetString(SR.CustomAutoFilterDialogBox);
            this.Load += new System.EventHandler(this.CustomRowFilter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radioButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compareList2)).EndInit();
            this.popupControlContainer1.ResumeLayout(false);
            this.popupControlContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void button1_Click(object sender, System.EventArgs e)
        {
            FilterString = '[' + mappingName + ']';
            switch (filterNumber)
            {
                case 0:
                    FilterString += GetFilter(this.comboBox1.SelectedIndex, this.compareList1.Text);
                    break;
                case 1:
                    FilterString += GetDateFilter(this.comboBox1.SelectedIndex, this.compareList1.Text);
                    break;
                case 2:
                    FilterString += GetNumberFilter(this.comboBox1.SelectedIndex, this.compareList1.Text);
                    break;
            }
            if (!string.IsNullOrEmpty(this.compareList2.Text))
            {
                FilterString += (this.radioButton1.Checked) ? " AND " : " OR ";
                FilterString += '[' + mappingName + ']';
                switch (filterNumber)
                {
                    case 0:
                        FilterString += GetFilter(this.comboBox2.SelectedIndex, this.compareList2.Text);
                        break;
                    case 1:
                        FilterString += GetDateFilter(this.comboBox2.SelectedIndex, this.compareList2.Text);
                        break;
                    case 2:
                        FilterString += GetNumberFilter(this.comboBox2.SelectedIndex, this.compareList2.Text);
                        break;
                }
            }
            compareText1 = this.compareList1.Text;
            compareText2 = this.compareList2.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public void SetFilterString(int index, bool isAlldates)
        {
            int indexValue = index;
            string filterValue = string.Empty;
            DateTime tempDate = DateTime.Now;
            int currQuarter = (tempDate.Month - 1) / 3 + 1;
            if (isAlldates)
            {
                switch (indexValue)
                {
                    case 0:
                        FilterString = '[' + mappingName + ']' + " like '" + "Quarter1'";
                        break;
                    case 1:
                        FilterString = '[' + mappingName + ']' + " like '" + "Quarter2'";
                        break;
                    case 2:
                        FilterString = '[' + mappingName + ']' + " like '" + "Quarter3'";
                        break;
                    case 3:
                        FilterString = '[' + mappingName + ']' + " like '" + "Quarter4'";
                        break;
                    case 5:
                        FilterString = '[' + mappingName + ']' + " like '" + "January'"; break;
                    case 6:
                        FilterString = '[' + mappingName + ']' + " like '" + "February'"; break;
                    case 7:
                        FilterString = '[' + mappingName + ']' + " like '" + "March'"; break;
                    case 8:
                        FilterString = '[' + mappingName + ']' + " like '" + "April'"; break;
                    case 9:
                        FilterString = '[' + mappingName + ']' + " like '" + "May'"; break;
                    case 10:
                        FilterString = '[' + mappingName + ']' + " like '" + "June'"; break;
                    case 11:
                        FilterString = '[' + mappingName + ']' + " like '" + "July'"; break;
                    case 12:
                        FilterString = '[' + mappingName + ']' + " like '" + "August'"; break;
                    case 13:
                        FilterString = '[' + mappingName + ']' + " like '" + "September'"; break;
                    case 14:
                        FilterString = '[' + mappingName + ']' + " like '" + "October'"; break;
                    case 15:
                        FilterString = '[' + mappingName + ']' + " like '" + "November'"; break;
                    case 16:
                        FilterString = '[' + mappingName + ']' + " like '" + "December'"; break;
                }
            }
            else
            {
                switch (indexValue)
                {
                    case 6:
                        FilterString = '[' + mappingName + ']' + " = '" + tempDate.AddDays(1).ToShortDateString() + "'";
                        break;
                    case 7:
                        FilterString = '[' + mappingName + ']' + " = '" + tempDate.ToShortDateString() + "'";
                        break;
                    case 8:
                        FilterString = '[' + mappingName + ']' + " = '" + tempDate.AddDays(-1).ToShortDateString() + "'";
                        break;
                    case 10:
                        tempDate = DateTime.Now.AddDays((int)DateTime.Now.DayOfWeek * -1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddDays(7).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddDays(14).ToShortDateString() + "'";
                        break;
                    case 11:
                        tempDate = DateTime.Now.AddDays((int)DateTime.Now.DayOfWeek * -1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddDays(7).ToShortDateString() + "'";
                        break;
                    case 12:
                        tempDate = DateTime.Now.AddDays((int)DateTime.Now.DayOfWeek * -1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddDays(-7).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.ToShortDateString() + "'";
                        break;
                    case 14:
                        tempDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddMonths(1).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddMonths(2).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 15:
                        tempDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddMonths(1).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 16:
                        tempDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddMonths(-1).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 18:
                        tempDate = new DateTime(DateTime.Now.Year, 3 * currQuarter - 2, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddMonths(3).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddMonths(6).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 19:
                        tempDate = new DateTime(DateTime.Now.Year, 3 * currQuarter - 2, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddMonths(3).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 20:
                        tempDate = new DateTime(DateTime.Now.Year, 3 * currQuarter - 2, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddMonths(-3).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 22:
                        tempDate = new DateTime(DateTime.Now.Year, 1, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddYears(1).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddYears(2).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 23:
                    case 26:
                        tempDate = new DateTime(DateTime.Now.Year, 1, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddYears(1).AddSeconds(-1).ToShortDateString() + "'";
                        break;
                    case 24:
                        tempDate = new DateTime(DateTime.Now.Year, 1, 1);
                        FilterString = '[' + mappingName + ']' + " >= '" + tempDate.AddYears(-1).ToShortDateString() + "'" + " AND " + '[' + mappingName + ']' + " <= '" + tempDate.AddSeconds(-1).ToShortDateString() + "'";
                        break;
                }
            }
        }
        public void SetDateCombo(int index)
        {
            this.comboBox2.Text = String.Empty;
            switch (index)
            {
                case 0:
                    this.comboBox1.SelectedIndex = 0;
                    break;
                case 2:
                    this.comboBox1.SelectedIndex = 4;
                    break;
                case 3:
                    this.comboBox1.SelectedIndex = 2;
                    break;
                case 4:
                    this.comboBox1.SelectedIndex = 3;
                    this.comboBox2.SelectedIndex = 5;
                    this.radioButton1.Checked = true;
                    break;
            }
            this.compareList1.Focus();
        }
        public void SetNumberCombo(int index)
        {
            this.comboBox2.Text = String.Empty;
            switch (index)
            {
                case 0:
                    this.comboBox1.SelectedIndex = 0;
                    break;
                case 1:
                    this.comboBox1.SelectedIndex = 1;
                    break;
                case 2:
                case 3:
                    this.comboBox1.SelectedIndex = 2;
                    break;
                case 4:
                    this.comboBox1.SelectedIndex = 3;
                    break;
                case 5:
                    this.comboBox1.SelectedIndex = 4;
                    break;
                case 6:
                    this.comboBox1.SelectedIndex = 5;
                    break;
                case 7:
                    this.comboBox1.SelectedIndex = 3;
                    this.comboBox2.SelectedIndex = 5;
                    this.radioButton1.Checked = true;
                    break;

            }
            this.compareList1.Focus();

        }
        public void SetCombo(int index)
        {
            switch (index)
            {
                case 0:
                case 7:
                    this.comboBox1.SelectedIndex = 0;
                    break;
                case 1:
                    this.comboBox1.SelectedIndex = 1;
                    break;
                case 2:
                case 3:
                    this.comboBox1.SelectedIndex = 8;
                    break;
                case 4:
                    this.comboBox1.SelectedIndex = 9;
                    break;
                case 5:
                    this.comboBox1.SelectedIndex = 7;
                    break;
            }
            this.comboBox2.Text = String.Empty;
        }

        private string GetFilter(int index, string text)
        {
            string s = "";
            switch (index)
            {
                case 0:
                    s = " = '" + text + "'";
                    break;
                case 1:
                    s = " <> '" + text + "'";
                    break;
                case 2:
                    s = " > '" + text + "'";
                    break;
                case 3:
                    s = " >= '" + text + "'";
                    break;
                case 4:
                    s = " < '" + text + "'";
                    break;
                case 5:
                    s = " <= '" + text + "'";
                    break;
                case 6:
                    s = " like '" + text + "'";
                    break;
                case 7:
                    s = " like '*" + text + "*'";
                    break;
                case 8:
                    s = " like '" + text + "*'";
                    break;
                case 9:
                    s = " like '*" + text + "'";
                    break;
            }
            return s;
        }
        private string GetDateFilter(int index, string text)
        {
            string s = "";
            switch (index)
            {
                case 0:
                    s = " = '" + text + "'"; // equal
                    break;
                case 1:
                    s = " <> '" + text + "'"; // not equal
                    break;
                case 2:
                    s = " > '" + text + "'"; // after
                    break;
                case 3:
                    s = " >= '" + text + "'"; // between
                    break;
                case 4:
                    s = " < '" + text + "'";  //Is before
                    break;
                case 5:
                    s = " <= '" + text + "'"; // between
                    break;
            }
            return s;
        }

        private string GetNumberFilter(int index, string text)
        {
            string s = "";
            switch (index)
            {
                case 0:
                    s = " = '" + text + "'";
                    break;
                case 1:
                    s = " <> '" + text + "'";
                    break;
                case 2:
                    s = " > '" + text + "'";
                    break;
                case 3:
                    s = " >= '" + text + "'";
                    break;
                case 4:
                    s = " < '" + text + "'";
                    break;
                case 5:
                    s = " <= '" + text + "'";
                    break;
                case 6:
                    s = " like '" + text + "'";
                    break;
                case 7:
                    s = " < '" + text + "'";
                    break;
                case 8:
                    s = " > '" + text + "'";
                    break;
                case 9:
                    s = " >= '" + text + "'";
                    break;
            }
            return s;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        void comboBox1_DropDown(object sender, EventArgs e)
        {
            this.comboBox1.DataSource = GetComboTable();
            this.comboBox1.DisplayMember = "FilterString";
            this.comboBox1.ValueMember = "ValueString";
        }
        void comboBox2_DropDown(object sender, EventArgs e)
        {
            this.comboBox2.DataSource = GetComboTable();
            this.comboBox2.DisplayMember = "FilterString";
            this.comboBox2.ValueMember = "ValueString";
        }
        void compareList1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.compareList1.Text) || !string.IsNullOrEmpty(this.compareList2.Text))
                this.button1.Enabled = true;
            else
                this.button1.Enabled = false;
        }

        public string columnName = string.Empty;
        public string mappingName = String.Empty;
        public string FilterString = string.Empty;


        private void CustomRowFilter_Load(object sender, System.EventArgs e)
        {
            this.label2.Text = columnName;
            compareList1.Text = string.Empty;
            compareList2.Text = string.Empty;
            SizeF stringSize = new SizeF();
            stringSize = this.CreateGraphics().MeasureString(columnName,this.label2.Font);
            this.label3.Location = new Point(this.label2.Location.X + (int)stringSize.Width + 4, this.label2.Location.Y + ((int)stringSize.Height / 2));
            this.label3.Size = new Size(this.label3.Location.X + this.button3.Location.X, 1);
            this.label3.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.button1.UseVisualStyle = true;
            this.button2.UseVisualStyle = true;
            this.UseOffice2007SchemeBackColor = true;
            this.label1.BackColor = Color.Transparent;
            this.label2.BackColor = Color.Transparent;
            this.button1.BackColor = Color.Transparent;
            this.button2.BackColor = Color.Transparent;
            this.panel1.BackColor = Color.Transparent;
            this.button1.Enabled = true;
            this.comboBox1.ListBox.Height = 50;
            this.comboBox2.ListBox.Height = 50;
            this.compareList1.ListBox.Height = 50;
            this.compareList2.ListBox.Height = 50;

            switch (this.styles)
            {
                case "SystemTheme":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.None;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.None;
                    break;
                case "Metro":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Metro;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Metro;
                    break;
                case "Custom":
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    break;
                case "Office2003":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2003;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2003;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.comboBox1.Style = Windows.Forms.VisualStyle.Office2003;
                    this.comboBox2.Style = Windows.Forms.VisualStyle.Office2003;
                    this.comboBox1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.comboBox2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.radioButton1.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton2.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.radioButton2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.compareList1.Style = Windows.Forms.VisualStyle.Office2003;
                    this.compareList2.Style = Windows.Forms.VisualStyle.Office2003;
                    this.compareList1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.compareList2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    break;
                case "Office2007Blue":
                case "Office2010Blue":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.comboBox1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.comboBox2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.radioButton1.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton2.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.radioButton2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.compareList1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.compareList2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Blue;
                    this.ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    break;
                case "Office2007Black":
                case "Office2010Black":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;
                    this.comboBox1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Black;
                    this.comboBox2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Black;
                    this.radioButton1.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton2.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;
                    this.radioButton2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;
                    this.compareList1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Black;
                    this.compareList2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Black;
                    this.ColorScheme = Windows.Forms.Office2007Theme.Black;
                    break;
                case "Office2007Silver":
                case "Office2010Silver":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    this.comboBox1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.comboBox1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Silver;
                    this.comboBox2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Silver;
                    this.radioButton1.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton2.Style = Windows.Forms.Tools.RadioButtonAdvStyle.Office2007;
                    this.radioButton1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    this.radioButton2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    this.compareList1.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList2.Style = Windows.Forms.VisualStyle.Office2007;
                    this.compareList1.Office2007ColorTheme = Windows.Forms.Office2007Theme.Silver;
                    this.compareList2.Office2007ColorTheme = Windows.Forms.Office2007Theme.Silver;
                    this.ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    break;
            }
            this.compareList1.Select();
        }
        public DataTable GetComboTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FilterString");
            dt.Columns.Add("ValueString", typeof(FilterCompareOperator));
            if (filterNumber == 1)
            {
                this.Controls.Add(button4);
                this.Controls.Add(button3);
                this.ClientSize = new System.Drawing.Size(406, 316);
            }
            else
            {
                this.Controls.Remove(button4);
                this.Controls.Remove(button3);
                this.ClientSize = new System.Drawing.Size(371, 198);
            }
            if (filterNumber == 1)
            {
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterEqual), FilterCompareOperator.Equals });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterNotequal), FilterCompareOperator.NotEquals });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterafter), FilterCompareOperator.GreaterThan });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterAfterthanOrEqual), FilterCompareOperator.GreaterThanOrEqualTo });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterBefore), FilterCompareOperator.LessThan });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterBeforethanOrEqual), FilterCompareOperator.LessThanOrEqualTo });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterLike), FilterCompareOperator.Like });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterMatch), FilterCompareOperator.Match });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterBeginsWith), 8 });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterEndsWith), 9 });
            }
            else
            {
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterEqual), FilterCompareOperator.Equals });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterNotequal), FilterCompareOperator.NotEquals });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterGreaterthan), FilterCompareOperator.GreaterThan });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterGreaterthanOrEqual), FilterCompareOperator.GreaterThanOrEqualTo });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterLessthan), FilterCompareOperator.LessThan });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterlessthanOrEqual), FilterCompareOperator.LessThanOrEqualTo });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterLike), FilterCompareOperator.Like });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterMatch), FilterCompareOperator.Match });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterBeginsWith), 8 });
                dt.Rows.Add(new object[] { SR.GetString(SR.CustomComboboxAutoFilterEndsWith), 9 });
            }
            dt.AcceptChanges();
            return dt;
        }
        /// <summary>
        /// overridien to process key messages to CustomRowFilter.
        /// </summary>
        /// <param name="m">keystroke information.</param>
        /// <returns>returns a boolean value.</returns>
        protected override bool ProcessKeyPreview(ref Message m)
        {
            KeyEventArgs pressedKey = new KeyEventArgs(((Keys)((int)m.WParam)) | Control.ModifierKeys);
            switch (pressedKey.KeyCode)
            {
                case Keys.Enter:
                    FilterString = '[' + mappingName + ']' + GetFilter(this.comboBox1.SelectedIndex, this.compareList1.Text); FilterString += (this.radioButton1.Checked) ? " AND " : " OR ";
                    FilterString += '[' + mappingName + ']' + GetFilter(this.comboBox2.SelectedIndex, this.compareList2.Text);
                    compareText1 = this.compareList1.Text;
                    compareText2 = this.compareList2.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
                default:
                    break;
            }
            return base.ProcessKeyPreview(ref m);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.popupControlContainer2.Hide();
            if (!this.popupControlContainer1.Visible)
            {
                this.popupControlContainer1.Location = new Point(this.button3.Location.X - this.popupControlContainer1.Width + this.button3.Width, this.popupControlContainer1.Location.Y);
                this.popupControlContainer1.Show();
            }
            else
                this.popupControlContainer1.Hide();
        }

        void monthCalendar1_MouseLeave(object sender, EventArgs e)
        {
            this.popupControlContainer1.Hide();
        }

        private void monthCalendar1_DateChanged(object sender, EventArgs e)
        {
            compareList2.Text = this.monthCalendar1.SelectedDates[0].ToShortDateString();
            this.popupControlContainer1.Hide();
        }
        void monthCalendar2_MouseLeave(object sender, EventArgs e)
        {
            this.popupControlContainer2.Hide();
        }

        private void monthCalendar2_DateChanged(object sender, EventArgs e)
        {
            compareList1.Text = this.monthCalendar2.SelectedDates[0].ToShortDateString();
            this.popupControlContainer2.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.popupControlContainer1.Hide();
            if (!this.popupControlContainer2.Visible)
            {
                this.popupControlContainer2.Location = new Point(this.button4.Location.X + this.button4.Width - this.popupControlContainer2.Width, this.popupControlContainer2.Location.Y);
                this.popupControlContainer2.Show();
            }
            else
                this.popupControlContainer2.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}