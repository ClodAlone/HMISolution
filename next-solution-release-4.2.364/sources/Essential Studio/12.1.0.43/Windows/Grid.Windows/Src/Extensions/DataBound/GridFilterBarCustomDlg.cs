//-------------------------------------------------------------------------------------------------
// <copyright file="GridFilterBarCustomDlg.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridFilterBarCustomDlg : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.ComboBox comboBox3;
        private ButtonAdv okButton;
        private ButtonAdv cancelButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        internal System.Windows.Forms.Label colLabel;

        internal string FilterString;

        internal int _none_ = 0;
        internal int _custom_ = 1;
        internal int _equals_ = 2;
        internal int does_not_equal = 3;
        internal int is_greater_than = 4;
        internal int is_greater_than_or_equal_to = 5;
        internal int is_less_than = 6;
        internal int is_less_than_or_equal_to = 7;
        internal int begins_with = 8;
        internal int does_not_begin_with = 9;
        internal int ends_with = 10;
        internal int does_not_end_with = 11;
        internal int _contains_ = 12;
        internal int does_not_contain = 13;
        internal int Use_to_represent_any_series_of_characters = 14;
        internal int Show_rows_where = 15;
        internal int improper_custom_filter = 16;
        internal int _and_ = 17;
        internal int custom_Row_Filter_Or_ = 18;
        internal int _OK_ = 19;
        internal int _Cancel_ = 20;
        internal int custom_Row_Filter = 21;
        internal int custom_Row_Filter_And = 22;

        internal GridFilterBarCustomDlg()
        {
            //
            // Required for Windows Form Designer support.
            //
            InitializeComponent();

            FilterString = string.Empty;
        }

        internal void SetStrings(string[] gridFilterBarStrings)
        {
            this.comboBox1.Items.AddRange(new object[] 
            {
                                                           string.Empty,
                                                           gridFilterBarStrings[_equals_],
                                                           gridFilterBarStrings[does_not_equal],
                                                           gridFilterBarStrings[is_greater_than],
                                                           gridFilterBarStrings[is_greater_than_or_equal_to],
                                                           gridFilterBarStrings[is_less_than],
                                                           gridFilterBarStrings[is_less_than_or_equal_to],
                                                           gridFilterBarStrings[begins_with],
                                                           gridFilterBarStrings[does_not_begin_with],
                                                           gridFilterBarStrings[ends_with],
                                                           gridFilterBarStrings[does_not_end_with],
                                                           gridFilterBarStrings[_contains_],
                                                           gridFilterBarStrings[does_not_contain]
            });

            this.comboBox3.Items.AddRange(new object[] 
            {
                                                           string.Empty,
                                                           gridFilterBarStrings[_equals_],
                                                           gridFilterBarStrings[does_not_equal],
                                                           gridFilterBarStrings[is_greater_than],
                                                           gridFilterBarStrings[is_greater_than_or_equal_to],
                                                           gridFilterBarStrings[is_less_than],
                                                           gridFilterBarStrings[is_less_than_or_equal_to],
                                                           gridFilterBarStrings[begins_with],
                                                           gridFilterBarStrings[does_not_begin_with],
                                                           gridFilterBarStrings[ends_with],
                                                           gridFilterBarStrings[does_not_end_with],
                                                           gridFilterBarStrings[_contains_],
                                                           gridFilterBarStrings[does_not_contain]
            });

            this.label4.Text = gridFilterBarStrings[Use_to_represent_any_series_of_characters];
            this.label1.Text = gridFilterBarStrings[Show_rows_where];
            this.radioButton1.Text = gridFilterBarStrings[custom_Row_Filter_And];
            this.radioButton2.Text = gridFilterBarStrings[custom_Row_Filter_Or_];
            this.cancelButton.Text = gridFilterBarStrings[_Cancel_];
            this.okButton.Text = gridFilterBarStrings[_OK_];
            this.Text = gridFilterBarStrings[custom_Row_Filter];
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
            this.label1 = new System.Windows.Forms.Label();
            this.colLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.okButton = new ButtonAdv();
            this.cancelButton = new ButtonAdv();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Show rows where:";
            // 
            // colLabel
            // 
            this.colLabel.AutoSize = true;
            this.colLabel.Location = new System.Drawing.Point(24, 16);
            this.colLabel.Name = "colLabel";
            this.colLabel.Size = new System.Drawing.Size(41, 13);
            this.colLabel.TabIndex = 1;
            this.colLabel.Text = "column";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(24, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 1);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.comboBox1.Location = new System.Drawing.Point(24, 40);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(176, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(64, 72);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(48, 24);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "and";
            this.radioButton1.Click += new System.EventHandler(this.radioButton1_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(128, 72);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(48, 24);
            this.radioButton2.TabIndex = 6;
            this.radioButton2.Text = "or";
            this.radioButton2.Click += new System.EventHandler(this.radioButton2_Click);
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.comboBox3.Location = new System.Drawing.Point(24, 104);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(176, 21);
            this.comboBox3.TabIndex = 7;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(248, 168);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(64, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(328, 168);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(64, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(216, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(176, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = string.Empty;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(216, 104);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(176, 20);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = string.Empty;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(316, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "Use * to repesent any series of characters";
            // 
            // GridFilterBarCustomDlg
            // 
#if !SyncfusionFramework2_0
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
#endif
            this.ClientSize = new System.Drawing.Size(416, 197);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.label4,
                                                                          this.textBox2,
                                                                          this.textBox1,
                                                                          this.cancelButton,
                                                                          this.okButton,
                                                                          this.comboBox3,
                                                                          this.radioButton2,
                                                                          this.radioButton1,
                                                                          this.comboBox1,
                                                                          this.groupBox1,
                                                                          this.colLabel,
                                                                          this.label1});
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridFilterBarCustomDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Row Filter";
            this.Icon = new Icon(AssemblyInfo.Assembly.GetManifestResourceStream(AssemblyInfo.RootNamespace + @".Resources." + "sfgrid.ico"));
            this.Load += new System.EventHandler(this.GridFilterBarCustomDlg_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #region Event Handlers

        private void GridFilterBarCustomDlg_Load(object sender, System.EventArgs e)
        {
            ////Adjust the start of the line to start at end of column label.
            this.groupBox1.Location = new Point(this.groupBox1.Location.X + this.colLabel.Width, this.groupBox1.Location.Y);
            this.groupBox1.Width -= this.colLabel.Width;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            ////ok...
            if (this.comboBox1.Text.Length > 0 && this.textBox1.Text.Length > 0)
            {
                FilterString = '[' + colLabel.Text + ']' + GetFilter(this.comboBox1.SelectedIndex, this.textBox1.Text);
            }

            if (this.comboBox3.Text.Length > 0 && this.textBox2.Text.Length > 0)
            {
                FilterString += this.radioButton1.Checked ? " AND " : " OR ";
                FilterString += '[' + colLabel.Text + ']' + GetFilter(this.comboBox3.SelectedIndex, this.textBox2.Text);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            ////cancel...
            this.Close();
        }

        private void radioButton1_Click(object sender, System.EventArgs e)
        {
            this.radioButton1.Checked = true;
            this.radioButton2.Checked = false;
        }

        private void radioButton2_Click(object sender, System.EventArgs e)
        {
            this.radioButton2.Checked = true;
            this.radioButton1.Checked = false;
        }
        #endregion

        #region Utility Methods
        private string GetFilter(int relation, string text)
        {
            string s = string.Empty;

            switch (relation)
            {
                case 1: ////equals
                    s = " = '" + text + "'";
                    break;
                case 2: ////does not equal
                    s = " <> '" + text + "'";
                    break;
                case 3: ////is greater than
                    s = " > '" + text + "'";
                    break;
                case 4: ////is greater than or equal
                    s = " >= '" + text + "'";
                    break;
                case 5: ////is less than
                    s = " < '" + text + "'";
                    break;
                case 6: ////is less than or equal
                    s = " <= '" + text + "'";
                    break;
                case 7: ////begins with
                    s = " LIKE '" + text + "*'";
                    break;
                case 8: ////does not begins with
                    s = " NOT LIKE '" + text + "*'";
                    break;
                case 9: ////ends with
                    s = " LIKE '*" + text + "'";
                    break;
                case 10: ////does not end with
                    s = " NOT LIKE '*" + text + "'";
                    break;
                case 11: ////contains
                    s = " LIKE '*" + text + "*'";
                    break;
                case 12: ////does not contain
                    s = " NOT LIKE '*" + text + "*'";
                    break;
                default:
                    break;
            }

            return s;
        }
        #endregion
    }
}
