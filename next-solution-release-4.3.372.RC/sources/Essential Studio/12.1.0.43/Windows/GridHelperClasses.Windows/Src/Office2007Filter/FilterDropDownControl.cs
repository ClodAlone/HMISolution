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
    internal partial class FilterDropDownControl : UserControl
    {
        public event EventHandler UserControlSave;
        public event EventHandler UserControlCancel;
        private bool initDone = false;
        //used to indicate changes to the values
        private bool changed = false;
        private TreeNodeAdv childNode;

        public FilterDropDownControl(string style)
        {
            InitializeComponent();            
            this.treeViewAdv1.StandardStyle.EnsureDefaultOptionedChild = true;
            this.treeViewAdv1.ShowRootLines = false;
            this.treeViewAdv1.MouseLeave += new EventHandler(treeViewAdv1_MouseLeave);
            TreeNodeAdv allNode = new TreeNodeAdv();
            allNode.Text = SR.GetString(SR.SelectAll);
            allNode.ShowCheckBox = true;            
            allNode.ShowPlusMinus = false;
            allNode.Expand();            
            allNode.InteractiveCheckBox = true;          
            this.treeViewAdv1.Nodes.Add(allNode);
            this.treeViewAdv1.Nodes[0].CheckStateChanged += new EventHandler(DropDownFilter_CheckStateChanged);
            childNode = new TreeNodeAdv();
            this.button1.UseVisualStyle = true;
            this.button2.UseVisualStyle = true;
            switch (style)
            {
                case "SystemTheme":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.None;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.None;
                    break;
                case "Metro":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Metro;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Metro;
                    this.button1.BackColor = Color.FromArgb(22, 165, 220);
                    this.button2.BackColor = Color.FromArgb(22, 165, 220);
                    this.button1.ForeColor = Color.White;
                    this.button2.ForeColor = Color.White;
                    this.treeViewAdv1.Style = TreeStyle.Metro;
                    break;
                case "Custom":
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Managed;
                    this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    this.treeViewAdv1.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Managed;
                    break;
                case "Office2003":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2003;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2003;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;                   
                    this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    this.treeViewAdv1.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                    break;
                case "Office2007Blue":
                case "Office2010Blue":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Blue;                   
                    if (style.Equals("Office2007Blue"))
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.treeViewAdv1.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                    }
                    else
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.treeViewAdv1.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                    }
                    break;
                case "Office2007Black":
                case "Office2010Black":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Black;                  
                    if (style.Equals("Office2007Black"))
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.treeViewAdv1.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                    }
                    else
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.treeViewAdv1.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                    }
                    break;
                case "Office2007Silver":
                case "Office2010Silver":
                    this.button1.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button2.Appearance = Windows.Forms.ButtonAppearance.Office2007;
                    this.button1.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;
                    this.button2.Office2007ColorScheme = Windows.Forms.Office2007Theme.Silver;                   
                    if (style.Equals("Office2007Silver"))
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.treeViewAdv1.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                    }
                    else
                    {
                        this.treeViewAdv1.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.treeViewAdv1.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                    }
                    break;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (((keyData & Keys.Alt) == Keys.Alt))
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
            }
            bool processed = base.ProcessDialogKey(keyData);
            if (!processed && (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)))
            {
                bool backward = (keyData & Keys.Shift) == Keys.Shift;
                this.SelectNextControl(null, !backward, true, true, true);
            }
            return processed;
        }  internal bool firstTab = true;
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
        void treeViewAdv1_MouseLeave(object sender, EventArgs e)
        {
            this.Parent.Focus();
        }      
        void DropDownFilter_CheckStateChanged(object sender, EventArgs e)
        {
            if (!this.initDone)
                return;
            changed = true;            
            this.button1.Enabled=this.treeViewAdv1.Nodes[0].CheckState == CheckState.Unchecked ? false:true;
        }

        /// <summary>
        /// Sets Metro button.
        /// </summary>
        /// <param name="button">button control</param>
        private void WireButton(ButtonAdv button)
        {
            if (button.Enabled)
                button.BackColor = Color.FromArgb(22, 165, 220);
            button.Font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
        }

        ArrayList itemCollection = new ArrayList();
        public void SetItems(object[] items, IList filteredValues, GridTableCellStyleInfo formatStyle, GridTableCellStyleInfo sty, bool isFiltered, bool isContains)
        {
            if (sty.GetActiveGridView() != null && sty.GetActiveGridView().GridOfficeScrollBars == Syncfusion.Windows.Forms.OfficeScrollBars.Metro)
            {
                WireButton(button1);
                WireButton(button2);
                this.treeViewAdv1.LineColor = Color.FromArgb(208, 208, 208);
                this.treeViewAdv1.LineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                this.treeViewAdv1.MetroScrollBars = true;
            }
            int count = sty.TableCellIdentity.Table.Records.Count;
            this.initDone = false;
            this.treeViewAdv1.Nodes[0].Nodes.Clear();
            List<string> values = new List<string>();
            List<object> list = new List<object>(items);
            foreach (object it in items)
            {
                string formattedValue = string.Empty;
                formattedValue = formatStyle.GetFormattedText(it);
                if (!values.Contains(formattedValue))
                {
                    if ((items.Length - 1) <= count)
                    {
                        if (!string.IsNullOrEmpty(formattedValue) || ((items.Length - 1) != count && !((items.Length - 1) < count)))
                        {
                            childNode = new TreeNodeAdv();
                            childNode.ShowCheckBox = true;
                            childNode.Text = formattedValue;
                            childNode.InteractiveCheckBox = true;

                            if ((filteredValues.Contains(formattedValue) 
                                || (filteredValues.Contains("(null)") && formattedValue.Equals("(Blanks)")))
                                && !isContains)
                            {
                                childNode.CheckState = CheckState.Checked;
                            }
                            values.Add(formattedValue);
                            childNode.CheckStateChanged += new EventHandler(DropDownFilter_CheckStateChanged);
                            this.treeViewAdv1.Nodes[0].Nodes.Add(childNode);
                        }
                        else
                        {
                            if ((items.Length - 1) != count)
                            {
                                if (itemCollection.Count > 0)
                                    itemCollection.Clear();
                                for (int i = 1; i <= sty.TableCellIdentity.Table.Records.Count; i++)
                                {
                                    itemCollection.Add(sty.TableCellIdentity.Table.TableModel[i + 1, sty.TableCellIdentity.ColIndex].CellValue);
                                }

                                if (isFiltered || (filteredValues.Count == 0 && itemCollection.Contains(System.DBNull.Value)) || (filteredValues.Count == 0 && itemCollection.Contains(string.Empty) && list.Contains(string.Empty)))
                                {
                                    childNode = new TreeNodeAdv();
                                    childNode.ShowCheckBox = true;
                                    childNode.Text = formattedValue;
                                    childNode.InteractiveCheckBox = true;

                                    if (filteredValues.Contains(formattedValue) && !isContains)
                                    {
                                        childNode.CheckState = CheckState.Checked;
                                    }
                                    if (string.IsNullOrEmpty(childNode.Text))
                                    {
                                        childNode.Text = "(Blanks)";
                                    }
                                    values.Add(formattedValue);
                                    childNode.CheckStateChanged += new EventHandler(DropDownFilter_CheckStateChanged);
                                    this.treeViewAdv1.Nodes[0].Nodes.Add(childNode);
                                }
                            }
                        }
                    }
                }
            }
            if (this.treeViewAdv1.Nodes[0].Nodes[0].Text.Equals("(Blanks)"))
            {
                this.treeViewAdv1.Nodes[0].Nodes.Move(0, this.treeViewAdv1.Nodes[0].Nodes.Count - 1, 1);
            }
            if (filteredValues.Count == 0)
            {
                this.treeViewAdv1.Nodes[0].CheckState = CheckState.Checked;
                button1.Enabled = true;
            }
            this.initDone = true;
            UnWireEvents();
        }

        private  void WireEvents()
        {

        }

        private void UnWireEvents()
        {
            if (childNode != null)
                childNode.CheckStateChanged -= new EventHandler(DropDownFilter_CheckStateChanged);
        }

        public IList GetValues()
        {
            List<string> checkedItem = new List<string>();
            if (this.treeViewAdv1.Nodes[0].CheckState == CheckState.Checked && this.treeViewAdv1.Nodes[0].CheckState!=CheckState.Indeterminate)
            {
                return checkedItem;
            }

            foreach (TreeNodeAdv  val in this.treeViewAdv1.Nodes[0].Nodes)
            {
                if(val.CheckState==CheckState.Checked)
                    checkedItem.Add(val.Text);
            }
            return checkedItem;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool saveChanged = this.changed;
            this.changed = false;
            if (saveChanged && UserControlSave != null)
                UserControlSave(this, EventArgs.Empty);
            else if (UserControlCancel != null)
                UserControlCancel(this, EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            changed = false;
            if (UserControlCancel != null)
                UserControlCancel(this, EventArgs.Empty);
        }        
    }
}
