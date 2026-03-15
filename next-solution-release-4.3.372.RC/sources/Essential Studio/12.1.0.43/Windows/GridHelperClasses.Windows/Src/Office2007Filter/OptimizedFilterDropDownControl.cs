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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// User control to provide the design for optimized excel like filter dialog.
    /// </summary>
    internal partial class OptimizedFilterDropDownControl : UserControl
    {
        public event EventHandler UserControlSave;
        public event EventHandler UserControlCancel;
        private bool initDone = false;
        private Syncfusion.Windows.Forms.Tools.TreeViewAdv treeViewAdv1;
        /// <summary>
        /// used to indicate changes to the values
        /// </summary>
        private bool changed = false;
       /// <summary>
       /// Constructor for excel like filter dialog.
       /// </summary>
        public OptimizedFilterDropDownControl(string style, bool search)
        {
            isSearchOption = search;
            InitializeComponent();

            checkedListBox1.CheckOnClick = true;
            checkedListBox1.ItemCheck += new ItemCheckEventHandler(checkedListBox1_ItemCheck);             
            checkedListBox1.Items.Add(SR.GetString(SR.SelectAll));
            this.checkedListBox1.MouseLeave += new EventHandler(checkedListBox1_MouseLeave);
        }


        void treeViewAdv1_LostFocus(object sender, EventArgs e)
        {

        }

        void treeViewAdv1_NodeMouseClick(object sender, TreeViewAdvMouseClickEventArgs e)
        {
            if (e.Node == this.treeViewAdv1.Nodes[0])
            {
                isParentCheck = true;
            }
            else
                isChildCheck = true;
        }

        bool isParentCheck = true;
        bool isChildCheck = true;
        void treeViewAdv1_AfterCheck(object sender, TreeNodeAdvEventArgs e)
        {
            this.changed = true;
            this.okButton.Enabled = true;
            if (treeViewAdv1.Nodes.Count > 0)
            {
                int count = 0;
                if (e.Node == this.treeViewAdv1.Nodes[0] && isParentCheck)
                {
                    this.okButton.Enabled = (e.Node.CheckState == CheckState.Checked) ? true : false;
                    for (int i = 0; i < this.treeViewAdv1.Nodes.Count; i++)
                    {
                        isChildCheck = false;
                        if (e.Node.CheckState == CheckState.Unchecked)
                            this.treeViewAdv1.Nodes[i].CheckState = CheckState.Unchecked;
                        else
                            this.treeViewAdv1.Nodes[i].CheckState = CheckState.Checked;
                    }
                }
                for (int i = 1; i < this.treeViewAdv1.Nodes.Count; i++)
                {
                    if (this.treeViewAdv1.Nodes[i].CheckState == CheckState.Indeterminate)
                    {
                        if (isChildCheck)
                        {
                            isParentCheck = false;
                            isChildCheck = true;
                            this.treeViewAdv1.Nodes[0].CheckState = CheckState.Indeterminate;
                        }
                        return;
                    }
                    else if (this.treeViewAdv1.Nodes[i].CheckState == CheckState.Unchecked)
                        count++;
                }
                if (count == this.treeViewAdv1.Nodes.Count - 1)
                {
                    if (isChildCheck)
                    {
                        isParentCheck = false;
                        isChildCheck = true;
                        this.treeViewAdv1.Nodes[0].CheckState = CheckState.Unchecked;
                    }
                }
                else if (count == 0)
                {
                    if (isChildCheck)
                    {
                        isParentCheck = false;
                        isChildCheck = true;
                        this.treeViewAdv1.Nodes[0].CheckState = CheckState.Checked;
                    }
                }
            }
        }
        internal bool firstTab = true;
        protected override bool ProcessKeyPreview(ref Message m)
        {
            KeyEventArgs pressedKey = new KeyEventArgs(((Keys)((int)m.WParam)) | Control.ModifierKeys);
            switch (pressedKey.KeyCode)
            {
                case Keys.Enter:
                    bool saveChanged = this.changed;
                    this.changed = false;
                    if (saveChanged && UserControlSave != null)
                        UserControlSave(this, EventArgs.Empty);
                    else if (UserControlCancel != null)
                        UserControlCancel(this, EventArgs.Empty);
                    break;
                case Keys.Tab:
                    if (firstTab)
                    {
                        this.ActiveControl = this.Controls[0];
                        this.ActiveControl.Focus();
                        firstTab = false;
                    }
                    else if (this.ActiveControl is Syncfusion.Windows.Forms.ButtonAdv && this.ActiveControl.Text == SR.GetString(SR.Office2007FilterCancel))
                        firstTab = true;
                    if (this.ActiveControl is CheckedListBox)
                    {
                        this.ActiveControl = this.Controls[0];
                        this.ActiveControl.Focus();
                        firstTab = false;
                    }
                    if (this.ActiveControl is Syncfusion.Windows.Forms.VScrollBarCustomDraw)
                        this.ActiveControl = this.Controls[1];
                    break;
                default:
                    break;
            }
            return base.ProcessKeyPreview(ref m);
        }
        void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            this.Parent.Focus();
        }          

        /// <summary>
        /// Indicates the check box checking.
        /// </summary>
        private bool inItemCheck = false;
        /// <summary>
        /// Occurs when checked listbox clicked.
        /// </summary>
        /// <param name="sender">check box</param>
        /// <param name="e">event data</param>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!initDone || inItemCheck)
                return;
            changed = true;
            inItemCheck = true;
            if (e.Index == 0)
            {
                this.okButton.Enabled = e.NewValue == CheckState.Unchecked ? false : true;
                for (int i = 1; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, e.NewValue);
                }
            }
            else
            {
                if (e.NewValue == CheckState.Checked)
                {
                    this.checkedListBox1.SetItemCheckState(0, CheckState.Indeterminate);
                }
                else if (e.NewValue == CheckState.Indeterminate)
                {
                    this.checkedListBox1.SetItemCheckState(e.Index, CheckState.Unchecked);
                }
             }
            //delay ResetSelectAll so new check action is completed...
           // this.checkedListBox1.BeginInvoke(new System.Action(() => {ResetSelectAll(e);}), null); //requires .Net 3.5
            this.checkedListBox1.BeginInvoke(new ResetSelectAllDelegate(ResetSelectAll), new object[] { e }); //.NET 2.0 code
           
            inItemCheck = false;
        }

        /// <summary>
        /// Delegate for ResetSelectAll method.
        /// </summary>
        /// <param name="e">event data</param>
        private delegate void ResetSelectAllDelegate(ItemCheckEventArgs e);
        /// <summary>
        /// Resets the filter choices.
        /// </summary>
        /// <param name="e">event data</param>
        private void ResetSelectAll(ItemCheckEventArgs e)
        {
            inItemCheck = true;
            if (e.Index == 0)
            {
                if (e.NewValue == CheckState.Indeterminate)
                    checkedListBox1.SetItemCheckState(0, CheckState.Unchecked);
                if(checkedListBox1.GetItemCheckState(0) == CheckState.Unchecked)
                    this.checkedListBox1.ClearSelected();
            } 
            else if (checkedListBox1.CheckedIndices.Count == 1 && e.NewValue == CheckState.Unchecked && checkedListBox1.GetItemCheckState(0) != CheckState.Unchecked)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Unchecked);
            }
            else if (checkedListBox1.CheckedIndices.Count == checkedListBox1.Items.Count && e.Index != 0 && e.NewValue == CheckState.Checked && checkedListBox1.GetItemCheckState(0) != CheckState.Checked)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Checked);
            }
            else if (checkedListBox1.CheckedIndices.Count < checkedListBox1.Items.Count)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Indeterminate);
            }

            this.okButton.Enabled = this.checkedListBox1.GetItemCheckState(0) == CheckState.Unchecked ? false : true;
            inItemCheck = false;
        }

        /// <summary>
        /// Occurs when check state of the CheckBox has changed.
        /// </summary>
        /// <param name="sender">check box</param>
        /// <param name="e">event data</param>
        private void DropDownFilter_CheckStateChanged(object sender, EventArgs e)
        {
            if (!this.initDone)
                return;
            changed = true;            
            this.okButton.Enabled = this.checkedListBox1.GetItemCheckState(0) == CheckState.Unchecked  ? false : true;
        }

        ArrayList itemCollection = new ArrayList();
        /// <summary>
        /// Adds the filter choices to the checked listbox.
        /// </summary>
        /// <param name="items">filter choices</param>
        /// <param name="filteredValues">filtered values</param>
        /// <param name="formatStyle">cell style</param>
        public void SetItems(object[] items, IList filteredValues, GridTableCellStyleInfo formatStyle)
        {

            this.initDone = false;
            checkedListBox1.Show();
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.Add(SR.GetString(SR.SelectAll));
            bool allChecked = true, intermidiate = false;
            List<string> values = new List<string>();
            foreach (object it in items)
            {
                int loc = checkedListBox1.Items.Add(it.ToString());
                if (filteredValues.Contains(it.ToString()) || filteredValues.Count == 0)
                {
                    if (!(formatStyle.TableCellIdentity != null) && formatStyle.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(formatStyle.Text))
                    {
                        checkedListBox1.SetItemCheckState(loc, CheckState.Unchecked);
                        checkedListBox1.SetItemCheckState(0, CheckState.Unchecked);
                    }
                    else
                    {
                        checkedListBox1.SetItemCheckState(loc, CheckState.Checked);
                        intermidiate = true;
                    }
                }
                else
                    allChecked = false;
            }

            if (checkedListBox1.Items.Contains("(Blanks)"))
            {
                int chkIndex = checkedListBox1.Items.IndexOf("(Blanks)");
                bool chkState = checkedListBox1.GetItemChecked(chkIndex);
                this.checkedListBox1.Items.Remove("(Blanks)");
                this.checkedListBox1.Items.Insert(this.checkedListBox1.Items.Count, "(Blanks)");
                int chkedIndex = checkedListBox1.Items.IndexOf("(Blanks)");
                if (chkState)
                    checkedListBox1.SetItemCheckState(chkedIndex, CheckState.Checked);
            }

            if (allChecked && intermidiate)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Checked);
            }
            if (!allChecked && intermidiate)
            {
                checkedListBox1.SetItemCheckState(0, CheckState.Indeterminate);
            }
            this.initDone = true;
            this.okButton.Enabled = GetValues().Count > 0 ? true : false;
        }

        public ArrayList months = new ArrayList();
        public void SetDateTimeItems(object[] items, IList filteredValues, GridTableCellStyleInfo formatStyle, GridVisualStyles style, int filterCount)
        {
            this.treeViewAdv1 = new Syncfusion.Windows.Forms.Tools.TreeViewAdv();
            this.Controls.Add(this.treeViewAdv1);
            this.treeViewAdv1.Size = this.checkedListBox1.Size;
            this.treeViewAdv1.BackColor = System.Drawing.Color.White;
            this.treeViewAdv1.Location = this.checkedListBox1.Location;
            this.treeViewAdv1.BackColor = System.Drawing.Color.White;
            this.treeViewAdv1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewAdv1.BorderColor = Color.LightGray;
            this.treeViewAdv1.VScroll = true;
            this.treeViewAdv1.UpdateScrollBars();
            this.treeViewAdv1.HScroll = true;
            this.treeViewAdv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                           | System.Windows.Forms.AnchorStyles.Left)
                           | System.Windows.Forms.AnchorStyles.Right)));
            string[] months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            this.months.AddRange(months);
            treeViewAdv1.AfterCheck += new TreeNodeAdvEventHandler(treeViewAdv1_AfterCheck);
            treeViewAdv1.NodeMouseClick += new TreeNodeAdvMouseClickArgs(treeViewAdv1_NodeMouseClick);
            this.isParentCheck = true;
            this.treeViewAdv1.LabelEdit = false;
            this.treeViewAdv1.Visible = true;
            if (treeViewAdv1.Nodes.Count > 0)
                this.treeViewAdv1.Nodes.Clear();
            this.treeViewAdv1.InteractiveCheckBoxes = true;
            this.checkedListBox1.Hide();
            this.initDone = false;
            checkedListBox1.Items.Clear();
            this.treeViewAdv1.Style = TreeStyle.Default;
            if (style == GridVisualStyles.Metro)
                this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.None;
            checkedListBox1.Items.Add(SR.GetString(SR.SelectAll));
            bool allChecked = true, intermidiate = false;
            List<string> values = new List<string>();
            int yearIndex = -1, monthIndex = -1;
            treeViewAdv1.ShowRootLines = true;
            treeViewAdv1.ShowLines = true;
            treeViewAdv1.Nodes.Add(new TreeNodeAdv(SR.GetString(SR.SelectAll)));
            treeViewAdv1.Nodes[0].ShowCheckBox = true;
            //treeViewAdv1.Nodes[0].ShowPlusMinus = true;
            treeViewAdv1.LineColor = Color.Black;
            yearIndex++;
            int i = 0;
            bool isBlank = false;
            List<DateTime> dateValues = new List<DateTime>();
            foreach (object it in items)
            {
                try
                {
                    dateValues.Add(DateTime.Parse(it.ToString()));
                }
                catch (Exception ex)
                {
                    isBlank = true;
                    break;
                }
            }
            dateValues.Sort();
            foreach (DateTime date1 in dateValues)
            {
                string monthString = months[date1.Month-1].ToString();
                if (treeViewAdv1.Nodes.Count == 1)
                {
                    treeViewAdv1.Nodes.Add(new TreeNodeAdv(date1.Year.ToString()));
                    treeViewAdv1.Nodes[1].ShowCheckBox = true;
                    treeViewAdv1.Nodes[1].ShowPlusMinus = true;
                }
                else if (treeViewAdv1.Nodes[yearIndex + 1].Text != date1.Year.ToString())
                {
                    treeViewAdv1.Nodes.Add(new TreeNodeAdv(date1.Year.ToString()));
                    treeViewAdv1.Nodes[yearIndex + 1].ShowCheckBox = true;
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes[0].ShowPlusMinus = true;
                    yearIndex++;
                    treeViewAdv1.Nodes[yearIndex + 1].ShowCheckBox = true;
                    monthIndex = -1;
                    i = 0;
                }
                if (treeViewAdv1.Nodes[yearIndex + 1].Nodes.Count == 0)
                {
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes.Add(new TreeNodeAdv(monthString));
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes[0].ShowCheckBox = true;
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes[0].ShowPlusMinus = true;
                }
                else if (treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Text != monthString)
                {
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes.Add(new TreeNodeAdv(monthString));
                    monthIndex++;
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].ShowCheckBox = true;
                    treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].ShowPlusMinus = true;
                    i = 0;
                }
                treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes.Add(new TreeNodeAdv(date1.Day.ToString()));
                treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes[i++].ShowCheckBox = true;
                treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes[i - 1].ShowPlusMinus = false;
                treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes[i - 1].Nodes.Add(new TreeNodeAdv(date1.ToString()));

                int loc = checkedListBox1.Items.Add(date1.ToString());
                if (filteredValues.Contains(date1.ToString()) || filteredValues.Count == 0)
                {
                    if (formatStyle.TableCellIdentity != null && formatStyle.TableCellIdentity.Table.TableDescriptor.RecordFilters.Contains(formatStyle.Text))
                    {
                        treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes[i - 1].Checked = true;
                        intermidiate = true;
                    }
                    else
                    {
                        treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].Nodes[i - 1].Checked = true;
                        intermidiate = true;
                    }
                }
                else
                    allChecked = false;
            }
            if (isBlank)
            {
                treeViewAdv1.Nodes.Add(new TreeNodeAdv("(Blanks)"));
                treeViewAdv1.Nodes[treeViewAdv1.Nodes.Count - 1].ShowCheckBox = true;
                treeViewAdv1.Nodes[treeViewAdv1.Nodes.Count - 1].Checked = true;
                treeViewAdv1.Nodes[treeViewAdv1.Nodes.Count - 1].ShowPlusMinus = true;
            }

            if (allChecked && intermidiate)
            {
                treeViewAdv1.Nodes[yearIndex + 1].Nodes[monthIndex + 1].CheckState = CheckState.Checked;
                treeViewAdv1.Nodes[yearIndex + 1].CheckState = CheckState.Checked;
                treeViewAdv1.Nodes[0].CheckState = CheckState.Unchecked;
                treeViewAdv1.Nodes[0].CheckState = CheckState.Checked;
            }
            if (!allChecked && intermidiate)
            {
                treeViewAdv1.Nodes[0].CheckState = CheckState.Indeterminate;
            }
            this.treeViewAdv1.Show();
            this.treeViewAdv1.Visible = true;
            if (treeViewAdv1.Nodes.Count == 2)
                treeViewAdv1.Nodes[1].Expand();
            treeViewAdv1.ShowRootLines = true;
            treeViewAdv1.ShowLines = true;
            this.treeViewAdv1.EndEdit();
            this.initDone = true;
            this.okButton.Enabled = (allChecked || intermidiate) ? true : false;
        }

        /// <summary>
        /// Gets the choices from the checked listbox
        /// </summary>
        /// <returns>the checked items list</returns>
        public List<string> GetValues()
        {
            List<string> checkedItem = new List<string>();
            CheckedNodesColection checkedNodes = new CheckedNodesColection();
            //start at one to skip SelectAll
            if (treeViewAdv1 != null && treeViewAdv1.Visible)
            {
                checkedNodes = treeViewAdv1.CheckedNodes;
                checkedItem.Clear();
            }
            else
            {
                for (int i = 1; i < checkedListBox1.Items.Count; ++i)
                {
                    if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                        checkedItem.Add(checkedListBox1.Items[i].ToString());
                }
            }
            if (treeViewAdv1 != null)
            {
                foreach (TreeNodeAdv nodeAdv in checkedNodes)
                {
                    if (nodeAdv.Text == "(Blanks)")
                        checkedItem.Add("(Blanks)");
                    else if (nodeAdv.Nodes.Count > 0 && !nodeAdv.Nodes[0].HasChildren)
                        checkedItem.Add(nodeAdv.Nodes[0].Text);
                }
            }
            return checkedItem;
        }

        /// <summary>
        /// Performs filter operation for Ok button.
        /// </summary>
        /// <param name="sender">Ok button</param>
        /// <param name="e">event data</param>
        private void button1_Click(object sender, EventArgs e)
        {
            bool saveChanged = this.changed;
            this.changed = false;
            if (saveChanged && UserControlSave != null)
                UserControlSave(this, EventArgs.Empty);
            else if (UserControlCancel != null)
                UserControlCancel(this, EventArgs.Empty);
        }

        /// <summary>
        /// Performs cancel operation.
        /// </summary>
        /// <param name="sender">Cancel button</param>
        /// <param name="e">event data</param>
        private void button2_Click(object sender, EventArgs e)
        {
            changed = false;
            if (UserControlCancel != null)
                UserControlCancel(this, EventArgs.Empty);
        }
    }
}
