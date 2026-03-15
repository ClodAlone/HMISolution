#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace Syncfusion.GridHelperClasses
{
    public partial class Top10AutoFilter : Form
    {
        object[] uniqueList;
        public string FilterString = string.Empty;
        public string MappingName = String.Empty;
        public Top10AutoFilter()
        {
            InitializeComponent();
            StringCollection topSelection = new StringCollection();
            topSelection.Add("Top");
            topSelection.Add("Bottom");
            StringCollection items = new StringCollection();
            items.Add("Items");
            items.Add("Percent");
            this.topComboBox1.DataSource = topSelection;
            this.topComboBox1.SelectedIndex = 0;
            this.topComboBox1.MaxDropDownItems = 2;
            this.itemscomboBoxAdv1.MaxDropDownItems = 2;
            this.itemscomboBoxAdv1.DataSource = items;
            this.itemscomboBoxAdv1.SelectedIndex = 0;
        }

        public Top10AutoFilter(List<double> numberList)
        {
            InitializeComponent();
            StringCollection topSelection = new StringCollection();
            topSelection.Add("Top");
            topSelection.Add("Bottom");
            StringCollection items = new StringCollection();
            items.Add("Items");
            items.Add("Percent");
            this.topComboBox1.DataSource = topSelection;
            this.topComboBox1.SelectedIndex = 0;
            this.topComboBox1.MaxDropDownItems = 2;
            this.itemscomboBoxAdv1.MaxDropDownItems = 2;
            this.itemscomboBoxAdv1.DataSource = items;
            this.itemscomboBoxAdv1.SelectedIndex = 0;
            this.numberList = numberList;
        }
        List<double> numberList = new List<double>();
        private void okButton_Click(object sender, EventArgs e)
        {
            FilterString = '[' + MappingName + ']';
            if (topComboBox1.SelectedIndex == 0)
            {
                if (itemscomboBoxAdv1.SelectedIndex == 0)
                    FilterString += " > '" + numberList[numberList.Count - (int)numericUpDown1.Value - 1] + "'" + " AND " + '[' + MappingName + ']' + " <= '" + numberList[numberList.Count - 1] + "'";
                else
                {
                    double percentage = (numberList.Count * (int)numericUpDown1.Value) / 100;
                    FilterString += " >= '" + numberList[numberList.Count - (int)percentage] + "'" + " AND " + '[' + MappingName + ']' + " <= '" + numberList[numberList.Count - 1] + "'";
                }
            }
            else
            {
                if (itemscomboBoxAdv1.SelectedIndex == 0)
                    FilterString += " >= '" + numberList[0] + "'" + " AND " + '[' + MappingName + ']' + " < '" + numberList[(int)numericUpDown1.Value] + "'";
                else
                {
                    double percentage = (numberList.Count * (int)numericUpDown1.Value) / 100;
                    FilterString += " >= '" + numberList[0] + "'" + " AND " + '[' + MappingName + ']' + " < '" + numberList[(int)percentage] + "'";
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
