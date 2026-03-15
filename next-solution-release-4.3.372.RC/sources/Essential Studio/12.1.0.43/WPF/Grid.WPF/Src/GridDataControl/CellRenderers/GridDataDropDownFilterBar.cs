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
    using System.Windows.Media;



    public class GridDataDropDownFilterBarCellModel : GridCellDropDownCellModel<GridDataDropDownFilterBar>
    {
    }

    public class GridDataDropDownFilterBar : GridCellDropDownCellRenderer<GridCellComboBoxDropDown>
    {
        public GridDataDropDownFilterBar()
        {
            this.SupportsRenderOptimization = true;
        }

        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, GridCellComboBoxDropDown dropDownControl, GridRenderStyleInfo style)
        {
            //this.InitializeContent(dropDownControl, style);
            base.ArrangeUIElement(aca, dropDownControl, style);
        }

        public override void OnInitializeContent(GridCellComboBoxDropDown dropDownControl, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(dropDownControl, style);
            this.UnwireTemplateParts(dropDownControl);
            this.InitializeContent(dropDownControl, style);
            this.WireTemplateParts(dropDownControl);
        }


        new void InitializeContent(GridCellComboBoxDropDown dropDownControl, GridRenderStyleInfo style)
        {
            if (dropDownControl.ListBoxPart != null && dropDownControl.ListBoxPart.Items.Count == 0)
            {
                SetItemSource(dropDownControl, style);
                dropDownControl.ListBoxPart.SelectedIndex = 0;
            }

            if (dropDownControl.TextBoxPart != null)
                dropDownControl.TextBoxPart.Foreground = new SolidColorBrush(Colors.Black);

            if (dropDownControl.TextBoxPart != null && dropDownControl.ListBoxPart != null)
            {
                var text = style.CellValue.ToString();
                var dataGrid = this.GridControl.FindParentElementOfType<GridDataControl>();
                if (text.Length > 1 && text.Remove(1) == "=")
                {
                    if (!dataGrid.Model.Table.HasGroups || (dataGrid.Model.Table.HasGroups && dropDownControl.TextBoxPart.Text == string.Empty))
                        dropDownControl.TextBoxPart.Text = text.Remove(0, 1); //item.ToString();
                    else if (dropDownControl.TextBoxPart.Text.Length > 1 && dropDownControl.TextBoxPart.Text.Remove(1) == "=")
                        dropDownControl.TextBoxPart.Text = dropDownControl.TextBoxPart.Text.Remove(0, 1);
                }
                else
                {
                    dropDownControl.TextBoxPart.Text = text;
                }
            }           
        }

        protected override void OnActivated()
        {
            if(this.CurrentCellUIElement!=null)
                SetItemSource(this.CurrentCellUIElement, this.CurrentStyle);
            UpdateDropDown();
            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.Focus();
                if (this.CurrentCellUIElement.DropDownStyle == GridDropDownStyle.AutoComplete)
                {
                  
                    this.CurrentCell.BeginEdit();
                    this.CurrentCellUIElement.TextBoxPart.Focus();
                    this.CurrentCellUIElement.TextBoxPart.Select(0, this.CurrentCellUIElement.TextBoxPart.Text.Length);
                }
            }
        }

        /// <summary>
        /// This method used to update the Property for Dropdown
        /// </summary>
        internal void UpdateDropDown()
        {
             var viscol = GetCurrentColumn();
            // base.ClearActualText();
             if (viscol != null && viscol.FilterBarStyle.IsEditable && this.CurrentCellUIElement!=null)//Temp Added for Editable DropDownFilter Bar.
             {
                 this.CurrentStyle.DropDownStyle = GridDropDownStyle.AutoComplete;
                 this.CurrentCellUIElement.DropDownStyle = GridDropDownStyle.AutoComplete;
                 this.IsAutoComplete = true;
                 this.IsAllowNewEntries = false;
                 this.IsDisabled = false;
                 this.CurrentCellUIElement.IsReadOnly = false;
             }
             else if(viscol!=null && !viscol.FilterBarStyle.IsEditable && this.CurrentCellUIElement!=null)
             {
                 this.CurrentStyle.DropDownStyle = GridDropDownStyle.Exclusive;
                 this.CurrentCellUIElement.DropDownStyle = GridDropDownStyle.Exclusive;
                 this.IsDisabled = true;
                 this.IsAutoComplete = false;
                 this.IsAllowNewEntries = false;
                 this.CurrentCellUIElement.IsReadOnly = true;
             }
        }
        
        public override void RefreshContent()
        {           
            base.RefreshContent();           
        }

        protected override void OnDeactivated()
        {
            this.CurrentCell.IsInDropDownFilterCell = false;
            base.OnDeactivated();           
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnRender(DrawingContext dc, Cells.RenderCellArgs rca, GridRenderStyleInfo style)
        {
            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// This method used to set the item source for the DropDownList
        /// </summary>
        /// <param name="combobox"></param>
        /// <param name="style"></param>
        void SetItemSource(GridCellComboBoxDropDown combobox, GridRenderStyleInfo style)
        {
            var viscol = GetCurrentColumn();
            if (viscol != null && viscol.FilterBarStyle.ItemsSource != null)
            {
                combobox.ListBoxPart.ItemsSource = viscol.FilterBarStyle.ItemsSource;
                this.GridListModel.ListModel.ItemsSource = viscol.FilterBarStyle.ItemsSource;
                if (viscol.FilterBarStyle.ValueMember != null)
                {
                    this.GridListModel.ListModel.ValueMember = viscol.FilterBarStyle.ValueMember;
                    combobox.ListBoxPart.SelectedValuePath = viscol.FilterBarStyle.ValueMember;
                }
                if (viscol.FilterBarStyle.DisplayMember != null)
                {
                    this.GridListModel.ListModel.DisplayMember = viscol.FilterBarStyle.DisplayMember;
                    combobox.ListBoxPart.DisplayMemberPath = viscol.FilterBarStyle.DisplayMember;
                }
            }
            else
            {
                var colIdx = ((GridDataTableModel)this.GridControl.Model).ResolvePositionToVisibleColumnIndex(style.CellRowColumnIndex.ColumnIndex);
                if (colIdx == -1 || ((GridDataTableModel)this.GridControl.Model).View.Records == null)
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

                finalList.Insert(0, GridDataResourceWrapper.AllFilter);
                if (combobox.ListBoxPart != null)
                    combobox.ListBoxPart.ItemsSource = finalList;
                this.GridListModel.ListModel.ItemsSource = finalList;
            }
        }

        void dropDownControl_IsDropDownOpenChanged(object sender, EventArgs e)
        {
            var dropDownControl = sender as GridCellComboBoxDropDown;
            if (dropDownControl != null)
            {
                if (dropDownControl.IsDropDownOpen)
                {
                    SetItemSource(dropDownControl, this.CurrentStyle);

                    var listBox = dropDownControl.ListBoxPart;
                    if (listBox.SelectedIndex > -1)
                        for (int i = 0; i < listBox.Items.Count; i++)
                        {
                            if (listBox.ItemContainerGenerator.Status != GeneratorStatus.NotStarted)
                            {
                                var item = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
                                if (item != null)
                                    item.IsHighlighted = false;
                            }
                        }
                    if (dropDownControl.TextBoxPart.Text == string.Empty)
                        listBox.SelectedIndex = -1;

                    if (listBox.SelectedIndex > -1)
                    {
                        var highLightItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.SelectedIndex);
                        if (highLightItem != null && !highLightItem.IsHighlighted)
                        {
                            highLightItem.IsHighlighted = true;
                        }
                    }
                    this.EnsureCurrentItem(dropDownControl.ListBoxPart as HoverListBox);
                }
            }
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

        protected override void WireTemplateParts(GridCellComboBoxDropDown uiElement)
        {            
            if (uiElement.ListBoxPart != null)
            {
                uiElement.IsDropDownOpenChanged += new EventHandler(dropDownControl_IsDropDownOpenChanged);                
                uiElement.ListBoxPart.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBoxPart_PreviewMouseLeftButtonUp);             
                 
                uiElement.TextBoxPart.PreviewMouseDown += new MouseButtonEventHandler(TextBoxPart_PreviewMouseDown);               
                uiElement.LostFocus += new RoutedEventHandler(uiElement_LostFocus);               
                uiElement.TextBoxPart.PreviewKeyDown += new KeyEventHandler(OnTextBoxPartPreviewKeyDown);               
                uiElement.TextBoxPart.TextChanged += new System.Windows.Controls.TextChangedEventHandler(OnTextBoxPartTextChanged);                
                uiElement.TextBoxPart.KeyUp += new KeyEventHandler(TextBoxPart_KeyUp);           
                
            }
        }
       
        void TextBoxPart_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viscol = GetCurrentColumn();
            if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.TextBoxPart != null && viscol != null && viscol.FilterBarStyle.IsEditable)//Temp Added for Editable DropDownFilter Bar.
            {
                this.CurrentCell.BeginEdit(true);
                this.CurrentCellUIElement.TextBoxPart.IsReadOnly = false;
                this.CurrentCellUIElement.IsReadOnly = false;
                this.CurrentCellUIElement.TextBoxPart.SelectAll();
                this.CurrentCellUIElement.TextBoxPart.Focus();
                this.CurrentCellUIElement.TextBoxPart.Select(0, this.CurrentCellUIElement.TextBoxPart.Text.Length);
            }
            else
            {
                if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.TextBoxPart != null)
                    this.CurrentCellUIElement.TextBoxPart.IsReadOnly = true;
            }
        }

        void uiElement_LostFocus(object sender, RoutedEventArgs e)
        {          
            if (this.CurrentCellUIElement.TextBoxPart.Text == "")
            {
                var viscol = GetCurrentColumn();
                if (viscol.Filters.Count > 0)
                {
                    viscol.Filters.RemoveAt(0);                   
                }
                this.CurrentCellUIElement.TextBoxPart.Text = GridDataResourceWrapper.AllFilter;
                this.SetCurrentStatusMessage();                
            }            
        }

        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            base.RaiseGridCellClick(rowIndex, colIndex, e);
            var viscol = GetCurrentColumn();
            if (this.HasCurrentCellState && this.CurrentCellUIElement !=null && this.CurrentCellUIElement.TextBoxPart != null && viscol != null && viscol.FilterBarStyle.IsEditable)//Temp Added for Editable DropDownFilter Bar.
            {
                this.CurrentCell.BeginEdit(true);
                this.CurrentCellUIElement.TextBoxPart.IsReadOnly = false;
                this.CurrentCellUIElement.IsReadOnly = false;
                this.CurrentCellUIElement.TextBoxPart.SelectAll();
                this.CurrentCellUIElement.TextBoxPart.Focus();
                this.CurrentCellUIElement.TextBoxPart.Select(0, this.CurrentCellUIElement.TextBoxPart.Text.Length);
            }
            else
            {
                if (this.CurrentCellUIElement!=null && this.CurrentCellUIElement.TextBoxPart!=null)
                    this.CurrentCellUIElement.TextBoxPart.IsReadOnly = true;
            }   
        }
      
        protected override void OnTextBoxPartPreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.UpdateDropDown();           
            base.OnTextBoxPartPreviewKeyDown(sender, e);            
        }

        protected override void OnTextBoxPartTextChanged(object sender, TextChangedEventArgs e)
        {
            var viscol = GetCurrentColumn();

            base.OnTextBoxPartTextChanged(sender, e);
            if ((sender as TextBox).Text == string.Empty && viscol.FilterBarStyle.ItemsSource == null)
            {
                this.SetStatusMessage("Invalid Data Filter");
            }
        }

        void TextBoxPart_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            try
            {
                this.CurrentCell.IsInDropDownFilterCell = true;

                var viscol = GetCurrentColumn();

                if (viscol.FilterBarStyle.ItemsSource == null)
                    FilterValue = textBox.Text;
                else 
                {
                    if (textBox.Text == GridDataResourceWrapper.AllFilter || textBox.Text == "")
                    {
                        if (viscol.Filters.Count > 0)
                            viscol.Filters.RemoveAt(0);
                    }
                    else
                        FilterValue = this.GridListModel.ListModel.GetValue(ControlValue).ToString();
                }

               
               var gdcModel = (GridDataTableModel)this.GridControl.Model;
               if (gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.Immediate)
                {
                    if (textBox.Text != GridDataResourceWrapper.AllFilter && textBox.Text != "")
                        GenerateFilter();
                }
           


                if (textBox.Text == GridDataResourceWrapper.AllFilter)
                {
                    if (viscol.Filters.Count > 0)
                    {
                        viscol.Filters.RemoveAt(0);
                    }
                }
            }
            catch
            {
                if (textBox.Text == GridDataResourceWrapper.AllFilter)
                {
                    var viscol = GetCurrentColumn();
                    if (viscol.Filters.Count > 0)
                    {
                        viscol.Filters.RemoveAt(0);

                    }
                }
            }
            string filterItemText;
            var idx = base.FindItem(this.CurrentCellUIElement.TextBoxPart.Text, true, -1, true, out filterItemText);
            this.CurrentCellUIElement.ListBoxPart.SelectedIndex = idx;
            this.EnsureCurrentItem(this.CurrentCellUIElement.ListBoxPart as HoverListBox);

            if (textBox.Text != string.Empty)
            {
                this.SetCurrentStatusMessage();
            }
            this.CurrentCell.IsInDropDownFilterCell = false;
        }

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
       
        private GridDataDropDownFilterBarCellModel ComboBoxModel
        {
            get
            {
                return (this.CellModel as GridDataDropDownFilterBarCellModel);
            }
        }

        void ListBoxPart_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var control = sender as HoverListBox;
            var mousePosition = e.GetPosition(control);
            if (!this.IsInArrange && control.SelectedItem != null && mousePosition.X <= control.ItemPresenter.ActualWidth)
            {
                this.CurrentCell.BeginEdit();
                var item = control.SelectedItem;
                item = GetDisplayValue(control, item);
                if(!(item.ToString().Equals(GridDataResourceWrapper.AllFilter)))
                    item = "=" + item;
                if (this.CurrentCellUIElement != null)
                {
                    this.SuspendEvents = true;
                    this.CurrentCellUIElement.Text = item == null ? string.Empty : item.ToString();
                    this.SuspendEvents = false;

                }

                if (!this.AlreadyTextChanged)
                {
                    if (HasCurrentCellState)
                    {
                        SetControlText(item == null ? string.Empty : item.ToString());
                        RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);
                    }
                }
                this.CurrentCell.IsInDropDownFilterCell = true;
                if (control.SelectedItem.ToString() == GridDataResourceWrapper.AllFilter)
                {
                    // var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>(); Unused local variable

                    var viscol = GetCurrentColumn();
                    if (viscol != null && viscol.Filters.Count > 0)
                    {
                        viscol.Filters.RemoveAt(0);

                    }
                }
                else
                {

                    FilterValue = control.SelectedValue.ToString();
                    GenerateFilter();

                }
                SetCurrentStatusMessage();
                this.CurrentCell.IsInDropDownFilterCell = false;
                this.CurrentCellUIElement.ListBoxPart.SelectedItem = control.SelectedValue;
                this.EnsureCurrentItem(this.CurrentCellUIElement.ListBoxPart);

            }
        }

        private GridDataVisibleColumn GetCurrentColumn()
        {
            GridDataVisibleColumn column = default(GridDataVisibleColumn);
            if (this.HasCurrentCellState)
            {

                var gdcModel = this.GridControl.Model as GridDataTableModel;
                var currentColIndex = gdcModel.ResolvePositionToVisibleColumnIndex(this.CellRowColumnIndex.ColumnIndex);
                if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0 && currentColIndex >= 0)
                {
                    if (currentColIndex < 0) currentColIndex = 0;
                    if (gdcModel.TableProperties.VisibleColumns.Count > currentColIndex)
                        column = gdcModel.TableProperties.VisibleColumns[currentColIndex];
                }
            }
            return column;
        }

        string FilterValue = string.Empty;

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isAltKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;

            HoverListBox listBox = null; 
            HoverListBoxItem listBoxItem;
            if (e.Handled || (this.CurrentCellUIElement == null && this.CurrentCell.IsEditing))
            {
                return false;
            }
            // to Open / Close the DropDown on Pressing Alt + Down, F4, & Space bar on exclusive mode alone
            if ((isAltKey && e.SystemKey == Key.Down) || e.Key == Key.F4 || (this.CurrentStyle.DropDownStyle == GridDropDownStyle.Exclusive && e.Key == Key.Space))
            {
                // handle Alt + Down combination
                if (this.CurrentCellUIElement != null)
                {
                    this.CurrentCellUIElement.IsDropDownOpen = !this.CurrentCellUIElement.IsDropDownOpen;
                    e.Handled = true;
                }
            }

            if ((isControlKey || isShiftKey) && this.CurrentCell.IsEditing && e.Key != Key.Tab)
            {
                return false;
            }

            if (this.IsDroppedDown || this.CurrentCell.IsEditing)
                listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
            switch (e.Key)
            {
                case Key.Tab:
                    this.CurrentCell.IsInDropDownFilterCell = false;

                    if (this.CurrentCellUIElement != null)
                    {
                        var textBox = this.CurrentCellUIElement.TextBoxPart;
                        if (textBox != null)
                        {
                            // when tabbing out, we set the start index, if we have a big word, then it gets truncated
                            textBox.SelectionStart = 0;
                        }
                    }
                    return true;

                case Key.Left:
                    if (this.CurrentCell.IsEditing)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                        {
                            if (tb.SelectionStart == 0)
                                return true;
                            else
                                return false;
                        }
                        return false;
                    }
                    return true;
                    // e.Handled = true; Unreachable code.
                case Key.Right:
                    if (this.CurrentCell.IsEditing)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                        {
                            if (tb.SelectionLength == tb.Text.Length)
                                return true;
                            else
                                return false;
                        }
                        return false;
                    }
                    return true;
                    // e.Handled = true; Unreachable code
                case Key.End:
                case Key.Home:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Down:

                    if (this.IsDroppedDown)
                    {

                        listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                        this.EnsureCurrentItem(listBox);
                        if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex <= listBox.Items.Count - 2)
                        {
                            if (listBox.SelectedIndex == -1)
                                listBox.SelectedIndex = 0;

                            if (listBox.Items.CurrentItem != null)
                            {
                                listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.Items.CurrentItem));

                                if (listBoxItem != null)
                                {
                                    listBoxItem.IsHighlighted = false;
                                    listBox.Items.MoveCurrentToNext();

                                    {
                                        listBox.SelectedItem = listBox.Items.CurrentItem;
                                        listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.Items.CurrentItem));
                                        listBox.NotifyListBoxItemEnter(listBoxItem);
                                        this.CanProcessKey = false;
                                        this.CurrentCellUIElement.TextBoxPart.Text = GetDisplayValue(listBox, listBoxItem).ToString();// listBox.SelectedItem.ToString();
                                        this.CurrentCellUIElement.TextBoxPart.SelectAll();
                                        listBox.SelectedIndex = listBox.Items.IndexOf(listBox.Items.CurrentItem);
                                    }
                                }
                            }
                        }

                    }
                    else if( this.CurrentCell.IsEditing)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                            return true;
                    }

                    e.Handled = true;
                    return false;
                case Key.Up:
                    if (this.IsDroppedDown)
                    {
                        listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                        this.EnsureCurrentItem(listBox);
                        if (listBox.SelectedIndex > 0)
                        {
                            listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.Items.CurrentItem));
                            if (listBoxItem != null)
                            {
                                listBoxItem.IsHighlighted = false;
                                listBox.Items.MoveCurrentToPrevious();
                                if (listBox.Items.CurrentItem != null)
                                {
                                    listBox.SelectedItem = listBox.Items.CurrentItem;
                                    listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.Items.CurrentItem));
                                    listBox.NotifyListBoxItemEnter(listBoxItem);
                                    this.CanProcessKey = false;
                                    this.CurrentCellUIElement.TextBoxPart.Text = GetDisplayValue(listBox, listBoxItem).ToString();//listBox.SelectedItem.ToString();
                                    this.CurrentCellUIElement.TextBoxPart.SelectAll();
                                }
                            }
                        }
                    }
                    else if( this.CurrentCell.IsEditing)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                        {
                            return true;
                        }
                    }

                    e.Handled = true;
                    return false;

                case Key.Return:
                    if (!this.IsInArrange && listBox.SelectedItem != null)
                    {
                        //if (this.IsDroppedDown)
                        //    listBox.SelectedItem = listBox.Items.CurrentItem;
                        var item = listBox.SelectedItem;
                        var type = listBox.SelectedItem.GetType();
                        if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
                        {
                            if (listBox.ItemsSource is ITypedList)
                            {
                                var propcoll = ((ITypedList)(listBox.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                                item = propcoll.GetValue(listBox.SelectedItem, listBox.DisplayMemberPath).ToString();
                            }
                            else
                                item = TypeDescriptor.GetProperties(listBox.SelectedItem.GetType())[listBox.DisplayMemberPath].GetValue(listBox.SelectedItem).ToString();
                        }

                        if (this.CurrentCellUIElement != null)
                        {
                            this.SuspendEvents = true;
                            this.CurrentCellUIElement.Text = item == null ? string.Empty : item.ToString();
                            this.SuspendEvents = false;
                            this.CurrentCell.BeginEdit();
                            this.CurrentCellUIElement.IsDropDownOpen = false;
                        }
                        this.ComboBoxModel.ListModel.CurrentIndex = this.ComboBoxModel.FindValue(this.CurrentStyle, item);
                        if (!this.AlreadyTextChanged)
                        {
                            SetControlText(item == null ? string.Empty : item.ToString());
                            RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);
                            e.Handled = true;
                        }


                        if (listBox.SelectedItem.ToString() == GridDataResourceWrapper.AllFilter)
                        {
                            //var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>(); Unused local variable
                            var viscol = GetCurrentColumn();
                            if (viscol != null && viscol.Filters.Count > 0)
                            {
                                viscol.Filters.RemoveAt(0);

                            }
                        }
                        else
                        {
                            FilterValue = listBox.SelectedValue.ToString();
                            GenerateFilter();

                        }
                        SetCurrentStatusMessage();

                    }
                    if (isShiftKey)
                    {
                        break;
                    }
                    else
                    {
                        CurrentCell.EndEdit();
                        CurrentCell.MoveRight();
                        e.Handled = true;
                        break;
                    }


/* Unreachable code
                    e.Handled = true;
                    return false;
*/
                case Key.Escape:
                    if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
                    {
                        this.CurrentCellUIElement.IsDropDownOpen = false;
                    }

                    this.CurrentCell.CancelEdit();
                    this.CurrentCell.Refresh();
                    e.Handled = true;
                    break;
            }
            return true;
        }

        private void EnsureCurrentItem(HoverListBox listBox)
        {
            if (listBox.Items.CurrentItem == null && listBox.SelectedIndex > -1)
            {
                listBox.Items.MoveCurrentTo(listBox.SelectedItem);
            }
            else
            {
                string filterItemText;
                var idx = base.FindItem(this.CurrentCellUIElement.TextBoxPart.Text, true, -1, true, out filterItemText);
                listBox.SelectedIndex = idx;

                if (idx >= 0)
                {
                    var listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(idx);
                    listBox.HighlightedItem = listBoxItem;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
            }
        }

        private static object GetDisplayValue(HoverListBox control, object item)
        {
            var type = control.SelectedItem.GetType();
            if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
            {
                if (control.ItemsSource is ITypedList)
                {
                    var propcoll = ((ITypedList)(control.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                    item = propcoll.GetValue(control.SelectedItem, control.DisplayMemberPath).ToString();
                }
                else
                    item = TypeDescriptor.GetProperties(control.SelectedItem.GetType())[control.DisplayMemberPath].GetValue(control.SelectedItem).ToString();
            }
            else
            {
                return control.SelectedItem.ToString();
            }
            return item;
        }

        protected override void UnwireTemplateParts(GridCellComboBoxDropDown uiElement)
        {
            if (uiElement.ListBoxPart != null)
            {
                uiElement.IsDropDownOpenChanged -= new EventHandler(dropDownControl_IsDropDownOpenChanged);
                uiElement.ListBoxPart.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(ListBoxPart_PreviewMouseLeftButtonUp);
                uiElement.TextBoxPart.PreviewMouseDown -= new MouseButtonEventHandler(TextBoxPart_PreviewMouseDown);
                uiElement.LostFocus -= new RoutedEventHandler(uiElement_LostFocus);
                uiElement.TextBoxPart.PreviewKeyDown -= new KeyEventHandler(OnTextBoxPartPreviewKeyDown);
                uiElement.TextBoxPart.TextChanged -= new System.Windows.Controls.TextChangedEventHandler(OnTextBoxPartTextChanged);
                uiElement.TextBoxPart.KeyUp -= new KeyEventHandler(TextBoxPart_KeyUp);
            }
        }

        #region FilterstatusMessage

        private void SetCurrentStatusMessage()
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel != null && gdcModel.Grid != null)
            {
                string filters = "";
                var multipleFilters = 0;
                foreach (var item in gdcModel.TableProperties.VisibleColumns)
                {
                    if (item != null && item.Filters.Count > 0)
                    {
                        foreach (FilterPredicate filitem in item.Filters.OfType<FilterPredicate>())
                        {
                            if (filitem != null)
                            {
                                if (multipleFilters <= 0)
                                    filters += GetFilterTypeStrindEqu(filitem, item) + " ";
                                else
                                    filters += " " + filitem.PredicateType.ToString() + " " + GetFilterTypeStrindEqu(filitem, item);
                                multipleFilters++;
                            }
                        }
                    }
                }

                if (filters.Trim() == "")
                    SetStatusMessage("");
                else
                    SetStatusMessage(filters);
            }
        }

        private void SetStatusMessage(string statusMessage)
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel == null || gdcModel.Grid == null) return;
            var gdc = gdcModel.Grid.FindParentElementOfType<GridDataControl>();
            if (gdc != null)
            {
                gdc.StatusBarMessage = statusMessage;
            }
        }

        private string GetStatusMessage()
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel == null || gdcModel.Grid == null) return "";
            var gdc = gdcModel.Grid.FindParentElementOfType<GridDataControl>();
            if (gdc != null)
            {
                return gdc.StatusBarMessage;
            }
            return "";
        }

        private void GenerateFilter()
        {
            // var filterType = FilterType.Undefined; Variable is assigned but it is never used.
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            var visCol = GetCurrentColumn();
            if (visCol == null) return;
            var currentColIndex = gdcModel.ResolvePositionToVisibleColumnIndex(this.CellRowColumnIndex.ColumnIndex); //gdcModel.ResolvePositionToVisibleColumnIndex(gdcModel.TableProperties.VisibleColumns.IndexOf(visCol));
            if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0 && currentColIndex >= 0 && gdcModel.TableProperties.VisibleColumns.Count > currentColIndex)
            {
                gdcModel.FilterColumn(visCol, FilterValue, FilterType.Equals, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                return;
            }
        }

        private string GetFilterTypeStrindEqu(FilterPredicate filter, GridDataVisibleColumn visColumn)
        {
            string filtervalue = filter.FilterValue.ToString();
            Linq.FilterType filterType = filter.FilterType;
            string colname = "";
            if (visColumn != null)
            {
                colname = (visColumn.HeaderText != "") ? visColumn.HeaderText : (visColumn.MappingName != "" ? visColumn.MappingName : "");
            }
            colname = "[" + colname + "] ";
            switch (filterType)
            {
                case FilterType.LessThan:
                    return colname + "'<" + filtervalue + "'";
                case FilterType.LessThanOrEqual:
                    return colname + "'<=" + filtervalue + "'";
                case FilterType.Equals:
                    return colname + "'=" + filtervalue + "'";
                case FilterType.NotEquals:
                    return colname + "'!" + filtervalue + "'";
                case FilterType.GreaterThanOrEqual:
                    return colname + "'>=" + filtervalue + "'";
                case FilterType.GreaterThan:
                    return colname + "'>" + filtervalue + "'";
                case FilterType.EndsWith:
                    return colname + "'" + "%" + filtervalue + "'";
                case FilterType.StartsWith:
                    return colname + "'" + filtervalue + "%" + "'";
                case FilterType.Contains:
                    return colname + "'#" + filtervalue + "'";
                case FilterType.Undefined:
                default:
                    return colname + "'" + filtervalue + "'";
            }
        }

        #endregion
    }
  
}
