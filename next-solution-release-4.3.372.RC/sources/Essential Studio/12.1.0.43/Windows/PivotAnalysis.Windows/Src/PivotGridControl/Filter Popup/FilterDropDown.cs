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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    [ToolboxItem(false)]
    public partial class FilterDropDown : UserControl
    {
        private FilterItemsCollection m_FilterItemsCollection;
        private FilterItemElement m_FilterItemElement;
        private DragDropHelper helper;
        //public ListBox FilterListBox { get; set; }
        internal static List<string> FilterDimensions = new List<string>();

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl PivotControl { get; set; }
        public FilterItemsCollection FilterList
        {
            get
            {
                return m_FilterItemsCollection;
            }
            set
            {
                m_FilterItemsCollection = value;
                setItems();
            }
        }
        private void setItems()
        {
            Color clrBack, headerBorderTop, headerBorderLeft;
            this.PivotControl.TableModel.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft);
            TreeNodeAdv childNodes = null;
            TreeNodeAdv grandChildNodes = null;
            for (int i = 0; i < m_FilterItemsCollection.Count; i++)
            {
                if (i == 0)
                {
                    childNodes = new TreeNodeAdv();
                    this.parentNode.Root.Nodes.Add(childNodes);
                    this.parentNode.Root.Nodes[i].ShowCheckBox = true;
                    this.parentNode.Root.Nodes[i].InteractiveCheckBox = true;
                    this.parentNode.BackColor = clrBack;
                    this.parentNode.Root.Nodes[i].Checked = Convert.ToBoolean(m_FilterItemsCollection[i].IsSelected);
                    this.parentNode.Root.Nodes[i].Text = m_FilterItemsCollection[i].Key.ToString();
                    this.parentNode.Root.Nodes[i].EnsureDefaultOptionedChild = true;
                    this.parentNode.Root.Nodes[i].ChildStyle.EnsureDefaultOptionedChild = true;
                    this.parentNode.Root.Nodes[i].Expanded = true;
                }
                else
                {
                    grandChildNodes = new TreeNodeAdv();
                    this.parentNode.Nodes[0].Nodes.Add(grandChildNodes);
                    this.parentNode.Nodes[0].Nodes[i - 1].ShowCheckBox = true;
                    this.parentNode.Nodes[0].Nodes[i - 1].EnsureDefaultOptionedChild = true;
                    this.parentNode.Nodes[0].Nodes[i - 1].ChildStyle.EnsureDefaultOptionedChild = true;
                    this.parentNode.Nodes[0].Nodes[i - 1].Checked = Convert.ToBoolean(m_FilterItemsCollection[i].IsSelected);
                    this.parentNode.Nodes[0].Nodes[i - 1].Text = m_FilterItemsCollection[i].Key.ToString();
                    this.parentNode.Nodes[0].Nodes[i - 1].CheckStateChanged += new EventHandler(FilterDropDown_CheckStateChanged);
                }
            }

            if (this.PivotControl.GridVisualStyles == GridVisualStyles.Office2007Black || this.PivotControl.GridVisualStyles == GridVisualStyles.Office2010Black)
            {
                this.btnOk.Office2007ColorScheme = Office2007Theme.Black;
                this.BtnCancel.Office2007ColorScheme = Office2007Theme.Black;
            }
            else if (this.PivotControl.GridVisualStyles == GridVisualStyles.Office2007Blue || this.PivotControl.GridVisualStyles == GridVisualStyles.Office2010Blue)
            {
                this.btnOk.Office2007ColorScheme = Office2007Theme.Blue;
                this.BtnCancel.Office2007ColorScheme = Office2007Theme.Blue;

            }
            else if (this.PivotControl.GridVisualStyles == GridVisualStyles.Office2007Silver || this.PivotControl.GridVisualStyles == GridVisualStyles.Office2010Silver)
            {
                this.btnOk.Office2007ColorScheme = Office2007Theme.Silver;
                this.BtnCancel.Office2007ColorScheme = Office2007Theme.Silver;
            }
            else if (this.PivotControl.GridVisualStyles == GridVisualStyles.Metro)
            {
                this.Font = new Font("Segoe UI", 8.25f);
                this.ForeColor = Color.FromArgb(92, 92, 92);
                this.BackColor = Color.White;
                this.parentNode.MetroScrollBars = true;
                this.parentNode.BackColor = Color.White;
                WireButton(this.btnOk);
                WireButton(this.BtnCancel);
            }
        }
        /// <summary>
        /// Sets Metro button.
        /// </summary>
        /// <param name="button">button control</param>
        private void WireButton(ButtonAdv button)
        {
            button.Appearance = ButtonAppearance.Metro;
            button.BackColor = Color.FromArgb(35, 130, 195);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 8.25f, FontStyle.Bold);
        }

        void FilterDropDown_CheckStateChanged(object sender, EventArgs e)
        {
            if (!SelectedAtLeastOne())
                this.btnOk.Enabled = false;
            else
                this.btnOk.Enabled = true;
        }

        private bool SelectedAtLeastOne()
        {
            bool selected = false;
            for (int i = 0; i < this.parentNode.Nodes[0].Nodes.Count; i++)
            {
                if (this.parentNode.Nodes[0].Nodes[i].Checked)
                {
                    selected = true;
                    break;
                }
            }
            return selected;
        }

        public FilterDropDown(PivotGridControl GridControl)
        {
            InitializeComponent();
            this.PivotControl = GridControl;
            helper = new DragDropHelper(this.PivotControl);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            
            if (this.PivotControl != null)
            {
                this.PivotControl.TableControl.BeginUpdate();
                m_FilterItemElement = new FilterItemElement();
                if (this.parentNode.Nodes[0].Checked)
                {
                    this.FilterList[0].IsSelected = true;
                    this.FilterList.AllFilterItem.IsSelected = true;
                    this.FilterList[0].SelectedState = true;
                }
                else
                {
                    this.FilterList[0].IsSelected = false;
                    this.FilterList.AllFilterItem.IsSelected = false;
                    this.FilterList[0].SelectedState = false;
                }
                for (int i = 0; i < this.parentNode.Nodes[0].Nodes.Count; i++)
                {
                    if (!this.parentNode.Nodes[0].Nodes[i].Checked)
                    {
                        this.FilterList[i + 1].IsSelected = false;
                        this.FilterList[i + 1].SelectedState = false;
                    }
                    else
                    {
                        this.FilterList[i + 1].IsSelected = true;
                        this.FilterList[i + 1].SelectedState = true;
                    }
                    this.FilterList[i].AcceptChanges();
                }
                FilterExpression filterExpression = this.PivotControl.Filters.Where(x => x.DimensionName == this.FilterList.Name).FirstOrDefault();
                if (filterExpression == null)
                {
                    bool isExist = (from o in this.PivotControl.Filters.Where(l => l.DimensionName == this.FilterList.Name) select o).Any();
                    if (!isExist && isModified )
                    {
                        if (this.PivotControl.ItemSource is DataView || this.PivotControl.ItemSource is DataTable)
                        {
                            this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpressionForDataView(), Tag = this.FilterList, DimensionName = this.FilterList.Name });
                        }
                        else if (this.PivotControl.ItemSource is IEnumerable)
                        {
                            this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(true), Tag = this.FilterList, DimensionName = this.FilterList.Name });
                        }
                        else if (this.PivotControl.ItemSource is IListSource)
                        {
                            this.PivotControl.Filters.Add(new FilterExpression { Name = this.FilterList.Name, Expression = this.FilterList.GetFilterExpression(false), Tag = this.FilterList, DimensionName = this.FilterList.Name });
                        }
                        
                        if (!FilterDimensions.Contains(this.FilterList.Name))
                            FilterDimensions.Add(this.FilterList.Name);
                    }
                }
                else
                {
                    if (this.PivotControl.ItemSource is DataView || this.PivotControl.ItemSource is DataTable)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpressionForDataView();
                    }
                    else if (this.PivotControl.ItemSource is IEnumerable)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpression(true);
                    }
                    else if (this.PivotControl.ItemSource is IListSource)
                    {
                        filterExpression.Expression = this.FilterList.GetFilterExpression(false);
                    }

                    this.PivotControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                    if (this.FilterList.AllFilterItem.SelectedState == true)
                    {
                        if (FilterDimensions.Contains(this.FilterList.Name))
                            FilterDimensions.Remove(this.FilterList.Name);
                    }
                    else
                    {
                        if (!FilterDimensions.Contains(this.FilterList.Name))
                            FilterDimensions.Add(this.FilterList.Name);
                    }
                }
                this.PivotControl.PivotEngine.Populate();
                this.PivotControl.TableModel.CoveredRanges.Clear();
                foreach (var range in this.PivotControl.PivotEngine.CoveredRanges)
                {
                    this.PivotControl.TableModel.CoveredRanges.Add(GridRangeInfo.Cells(range.Top + 1, range.Left + 1, range.Bottom + 1, range.Right + 1));
                }
                this.PivotControl.Refresh();
                helper.RefreshLayout();
                if (this.Parent is Syncfusion.Windows.Forms.PopupControlContainer)
                {
                    Syncfusion.Windows.Forms.PopupControlContainer popup = this.Parent as Syncfusion.Windows.Forms.PopupControlContainer;
                    if (popup.IsShowing())
                        popup.HidePopup(PopupCloseType.Done);
                }
                isModified = false;
                this.PivotControl.TableControl.EndUpdate();
            }
        }

        bool isModified = false;
        void SelectedNodes_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                isModified = true;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.FilterList.RejectChanges();
            if (this.Parent is Syncfusion.Windows.Forms.PopupControlContainer)
            {
                Syncfusion.Windows.Forms.PopupControlContainer popup = this.Parent as Syncfusion.Windows.Forms.PopupControlContainer;
                if (popup.IsShowing())
                    popup.HidePopup();
            }
        }       


    }
}
