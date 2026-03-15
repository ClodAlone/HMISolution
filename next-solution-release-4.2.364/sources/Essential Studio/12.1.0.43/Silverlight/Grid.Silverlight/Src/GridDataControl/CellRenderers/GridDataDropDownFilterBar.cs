#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Input;
    using System.Xml.Serialization;
    using System.Collections;

    public class GridDataDropDownFilterBarCellModel : GridCellModel<GridDataDropDownFilterBar>
    {
    }

    public class GridDataDropDownFilterBar : GridVirtualizingCellRenderer<ComboBox>
    {

        public GridDataDropDownFilterBar()
        {
            this.AllowRecycle = true;
            this.SupportsRenderOptimization = true;
            this.IsFocusable = true;
            this.IsControlTextShown = false;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.IsDropDownable = true;
        }
        
       
        public override void OnInitializeContent(ComboBox uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);           
        }
        /// <summary>
        /// This method used to set the itemssource for the Combobox
        /// </summary>
        /// <param name="combobox"></param>
        void SetItemSource(ComboBox combobox)
        {
            var viscol = GetCurrentColumn();
           
            if (viscol != null && viscol.FilterBarStyle.ItemsSource != null)
            {
                combobox.ItemsSource = viscol.FilterBarStyle.ItemsSource;
                if (viscol.FilterBarStyle.ValueMember != null)
                {
                    combobox.SelectedValuePath = viscol.FilterBarStyle.ValueMember;
                }
                if (viscol.FilterBarStyle.DisplayMember != null)
                {
                    combobox.DisplayMemberPath = viscol.FilterBarStyle.DisplayMember;
                }

            }
            else
            {
                var colIdx = ((GridDataTableModel)this.GridControl.Model).ResolvePositionToVisibleColumnIndex(this.GridControl.Model.CurrentCellState.ColumnIndex); 
                if (colIdx <= -1 || ((GridDataTableModel)this.GridControl.Model).View.Records == null)
                {
                    return;
                }

                var itemsSource = this.GetFilterChoices(colIdx);
                var collection = itemsSource.ToList();

                // take out distinct values only and sort
                var finalList = collection.Distinct()
                    .OrderBy(o => o != null ? (o.GetType() != typeof(DBNull) ? o : (o == DBNull.Value ? ((GridDataTableModel)this.GridControl.Model).TableProperties.NullFilterText : o)) : o)
                    .Select(o =>
                    (object)o != null ? o : ((GridDataTableModel)this.GridControl.Model).TableProperties.NullFilterText
                    ).ToList();

                finalList.Insert(0, "(All)");
                combobox.ItemsSource = finalList;
            }
        }
       
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isAltKey = (Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
            if (e.Key == Key.Space)
            {
                if (this.CurrentCellUIElement != null)
                {

                    if (this.CurrentCellUIElement.IsDropDownOpen)
                    {
                        this.CurrentCellUIElement.IsDropDownOpen = false;
                    }
                    else
                    {
                        this.CurrentCellUIElement.IsDropDownOpen = true;

                    }
                    return false;
                }
            }
          
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        public virtual IEnumerable<object> GetFilterChoices(int colIdx)
        {
            var tableModel = this.GridControl.Model as GridDataTableModel;
            tableModel.SuspendEvents();
            var visibleColumn = tableModel.TableProperties.VisibleColumns[colIdx];           
            tableModel.ResumeEvents();
            for (int i = 0; i < tableModel.View.Records.Count; i++)
            {
                object value = null;
                if (!visibleColumn.IsUnbound)
                {
                    value = tableModel.Table.GetValue(tableModel.View.Records[i].Data, visibleColumn.MappingName);
                }
                else
                {
                    value = tableModel.Table.GetUnboundValue(i, visibleColumn.MappingName);
                }
                yield return value;
            }
        }
        protected override void OnWireUIElement(ComboBox uiElement)
        {
            base.OnWireUIElement(uiElement);           
            uiElement.DropDownOpened += new EventHandler(uiElement_DropDownOpened);
          
            uiElement.SelectionChanged += new SelectionChangedEventHandler(uiElement_SelectionChanged);          
            uiElement.DropDownClosed += new EventHandler(uiElement_DropDownClosed);
        } 

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            base.RaiseGridCellClick(rowIndex, colIndex, e);
            this.CurrentCell.BeginEdit();            
        }

        void uiElement_DropDownClosed(object sender, EventArgs e)
        {          
            this.CurrentCell.EndEdit();
            this.CurrentCell.BeginEdit();
            this.CurrentCell.UnloadCurrentCellUIElement();      
         
        }      
        
        protected override void OnUnwireUIElement(ComboBox uiElement)
        {
            base.OnUnwireUIElement(uiElement);           
            uiElement.DropDownOpened -= new EventHandler(uiElement_DropDownOpened);
            uiElement.DropDownClosed -= new EventHandler(uiElement_DropDownClosed);           
            uiElement.SelectionChanged -= new SelectionChangedEventHandler(uiElement_SelectionChanged); 
        }
        
        /// <summary>
        /// This function used to set the filterstate
        /// </summary>
        private void SetFilterState()
        {
            var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
            var visibleColumns = datagrid.VisibleColumns;
            datagrid.Model.IsInFilter = false;
            foreach (var items in visibleColumns)
            {
                if (items.Filters.Count > 0)
                {
                    datagrid.Model.IsInFilter = true;
                }
            }
        }
         
        /// <summary>
        /// This method used to Get the current coloum from the visible columns collection
        /// </summary>
        /// <returns></returns>
        private GridDataVisibleColumn GetCurrentColumn()
        {
            GridDataVisibleColumn column = default(GridDataVisibleColumn);
            if (this.HasCurrentCellState)
            {
                var currentColIndex = this.CellRowColumnIndex.ColumnIndex;
                var gdcModel = this.GridControl.Model as GridDataTableModel;
                if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0 && currentColIndex >= 0)
                {
                    var colOffset = gdcModel.ResolveDefaultColumnOffset();
                    currentColIndex = currentColIndex - colOffset;
                    if (currentColIndex < 0) currentColIndex = 0;
                    if (gdcModel.TableProperties.VisibleColumns.Count > currentColIndex)
                        column = gdcModel.TableProperties.VisibleColumns[currentColIndex];
                }
            }

            return column;
        }

        string FilterValue = string.Empty;

        private void GenerateFilter()
        {
            // var filterType = FilterType.Undefined;
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            var visCol = GetCurrentColumn();
            if (visCol == null) return;
            var currentColIndex = gdcModel.TableProperties.VisibleColumns.IndexOf(visCol);
            if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0 && currentColIndex >= 0 && gdcModel.TableProperties.VisibleColumns.Count > currentColIndex)
            {                
                gdcModel.FilterColumn(visCol, FilterValue, FilterType.Equals, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                return;
            }
        }

        void uiElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as ComboBox;
            if (control.SelectedItem != null)
            {
                if (control.SelectedItem.ToString() == "(All)")
                {
                    var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();

                    var viscol = GetCurrentColumn();
                    if (viscol.Filters.Count > 0)
                    {
                        viscol.Filters.Clear();                       

                    }
                    control.SelectedIndex = 0;
                }
                else
                {
                    FilterValue = control.SelectedValue.ToString();
                    GenerateFilter();
                    control.SelectedItem = FilterValue;
                }
            }
        }

        void uiElement_DropDownOpened(object sender, EventArgs e)
        {          
            var dropDownControl = sender as ComboBox;
            if (dropDownControl != null)
            {
                if (dropDownControl.IsDropDownOpen)
                {
                  this.SetItemSource(dropDownControl);
                 
                      var text = this.ControlValue.ToString();
                      if (this.ControlValue.ToString().Contains("="))
                      {
                          if (this.ControlValue.ToString().Length > 1 && this.ControlValue.ToString().Remove(1) == "=")
                          {
                              text = this.ControlValue.ToString().Remove(0, 1); 
                          }
                      }
                      
                      var item = dropDownControl.Items;
                      List<string> stringList = new List<string>();
                      foreach (var i in item) 
                          stringList.Add(i.ToString());
                     var index= stringList.IndexOf(text);
                     dropDownControl.SelectedIndex = index;
                      
                  }
                }             
                      
        }
    }
}
