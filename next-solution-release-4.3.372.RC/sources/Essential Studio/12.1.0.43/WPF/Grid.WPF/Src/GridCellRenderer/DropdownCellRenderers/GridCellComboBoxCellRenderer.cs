#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Collections;
    using System.Collections;
    using System.Windows.Controls;
    using System.Globalization;
    using System.Windows.Input;
    using System.Windows;
    using System.Diagnostics;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Controls.Cells;
    using System.ComponentModel;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Data;

    /// <summary>
    /// Implements the model part of a combobox cell.
    /// </summary>
    public class GridCellComboBoxCellModel : GridCellDropDownCellModel<GridCellComboBoxCellRenderer>
    {
    }

    /// <summary>
    /// Renders a combobox control in a grid cell.
    /// </summary>
    public class GridCellComboBoxCellRenderer : GridCellDropDownCellRenderer<GridCellComboBoxDropDown>
    {
        //Flag to enable or disable usage of AutoCompleteList - Used for Incremental Filtering
        public bool UseAutoCompleteList = false;
        /// <summary>
        /// Initializes a new <see cref="GridCellComboBoxCellRenderer"/>.
        /// </summary>
        public GridCellComboBoxCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.IsFocusable = true;            
            this.AllowKeepAliveOnlyCurrentCell = true; 
        }

        private GridCellComboBoxCellModel ComboBoxModel
        {
            get
            {
                return (this.CellModel as GridCellComboBoxCellModel);
            }
        }


        /// flag set for listent the F2 Key press.
        /// </summary>
        private bool flag = false;

        /// <summary>
        /// Initializes the content of the combobox cell using
        /// the information from the cell style (value, text, behavior etc.).
        /// </summary>       
        /// <param name="dropDownControl">The combobox control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(GridCellComboBoxDropDown dropDownControl, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(dropDownControl, style);            
            if (this.GridControl is GridDataControlBaseImpl  && ((GridDataTableModel)this.GridControl.Model).TableProperties.EnableVisualStyleForEditors)
            {
                dropDownControl = this.GetDropDownVisualStyle(dropDownControl, style);
            }
            //if (falg && (CurrentStyle.DropDownStyle != GridDropDownStyle.Exclusive)) In this previous code, flag doesnt set any where so while pressing F2 it doesnt enter edit mode.
            if ( dropDownControl.TextBoxPart!=null)
            {
                if (CurrentStyle.DropDownStyle != GridDropDownStyle.Exclusive)
                {
                    dropDownControl.IsReadOnly = false;
                    if (flag)
                    {
                        dropDownControl.TextBoxPart.Focus();
                        dropDownControl.TextBoxPart.CaretIndex = dropDownControl.TextBoxPart.Text.Length;
                        style.CellValue = dropDownControl.TextBoxPart.Text;
                    }
                }
                if (style.DropDownStyle == GridDropDownStyle.Exclusive)
                {
                    dropDownControl.IsReadOnly = true;
                }

                dropDownControl.Background = Brushes.White;
                dropDownControl.Foreground = Brushes.Black;
                dropDownControl.IsMouseTrackingEnabled = style.IsMouseTrackingEnabled;
            }


            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                dropDownControl.FlowDirection = style.FlowDirection;

                double m11 = -1;
                double m22 = 1;
                double offsetX = dropDownControl.Width;
                double offsetY = 0;
                dropDownControl.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                dropDownControl.LayoutTransform = MatrixTransform.Identity;
            }
        }

        protected override void WireTemplateParts(GridCellComboBoxDropDown uiElement)
        {
            if (uiElement.ListBoxPart != null)
            {
                uiElement.ListBoxPart.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ListBoxPart_MouseLeftButtonUp);
                uiElement.ListBoxPart.MouseDown += new MouseButtonEventHandler(ListBoxPart_MouseDown);
                if (uiElement.ListBoxPart.ItemContainerGenerator.Status == GeneratorStatus.NotStarted)
                    uiElement.ListBoxPart.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
            }

            if (uiElement.TextBoxPart != null)
            {
                uiElement.TextBoxPart.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
            }

            base.WireTemplateParts(uiElement);
        }

        protected override void OnTextBoxPartTextChanged(object sender, TextChangedEventArgs e)
        {
            base.OnTextBoxPartTextChanged(sender, e);
            var textBox = sender as TextBox;
            if (this.CurrentCellUIElement != null && (sender as TextBox).Text.Length > 0 && this.CurrentCellUIElement.DropDownStyle == GridDropDownStyle.AutoComplete)
            {
                if (CurrentStyle.IncrementalFilter == IncrementalFilter.Enable && textBox.Tag != null)
                {
                    GenerateAutoCompleteList(sender);
                }
                if (!(this.CurrentCellUIElement.ListBoxPart as HoverListBox).DropDownPopupHost.PopupHost.IsOpen)
                    (this.CurrentCellUIElement.ListBoxPart as HoverListBox).DropDownPopupHost.PopupHost.IsOpen = true;
            }
        }

        /// <summary>
        /// Defines and Updates Itemsource for the Dropdown List, for every Keystroke for Incremental Filtering
        /// </summary>
        protected void GenerateAutoCompleteList(object sender)
        {
            var textBox = sender as TextBox;
            List<object> AutoCompleteList = new List<object>();
            string text = textBox.Tag.ToString();
            if (!string.IsNullOrEmpty(text)&&textBox.Text.Length>0)
            {
                    foreach (var s in CurrentCellUIElement.AutoSuggestionList)
                    {
                        var it = this.GridListModel.ListModel.GetDisplayValue(s);
                        string Content = Convert.ToString(it);
                        if (Content.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) && it != null)
                        {
                            AutoCompleteList.Add(s);
                            UseAutoCompleteList = true;
                        }
                    }
                    textBox.Tag = null;
                    this.CurrentCellUIElement.ListBoxPart.ItemsSource = AutoCompleteList;
                    SetSelectedIndex(0);
                    SetHighlightedItem(0); //Since the first item is needed to be selected for every keystrokes, we set the highligted item to 0th Index
             }
        }
        /// <summary>
        /// OnTextBoxPartPreviewKeyDown is used to handle Backspace and Delete Keys, for Incremental Filtering
        /// </summary>
        
        protected override void OnTextBoxPartPreviewKeyDown(object sender, KeyEventArgs e)
        {
            base.OnTextBoxPartPreviewKeyDown(sender, e);
            if (e.Key == Key.Back && CurrentStyle.IncrementalFilter == IncrementalFilter.Enable && (sender as TextBox).Text.Length > 0 && UseAutoCompleteList == true)
                GenerateAutoCompleteList(sender);
            else
                UseAutoCompleteList = false;
        }

        void ListBoxPart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Syncfusion.Windows.Controls.Grid.HoverListBox)sender).DropDownPopupHost.PopupHost.IsOpen)
                e.Handled = true;
        }

        protected override void OnEnteredEditMode()
        {
            if (CurrentCellUIElement != null)
            {
                GridRenderStyleInfo style = CurrentStyle;
                var text = GetControlText(style);
                if(!(text.Equals(this.CurrentCellUIElement.Text)))
                    CurrentCellUIElement.Text = text;
            }
            else
            {
                GridControl.InvalidateCell(CellRowColumnIndex);
            }
        }

       
        void ListBoxPart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var control = sender as HoverListBox;
            var mousePosition = e.GetPosition(control);
             //Previous Code if (!this.IsInArrange && control.SelectedItem != null)
            // In this Previous condition we have added mousePosition.X <= control.ActualWidth - 20. Because if we click on the ComboBox DropDown'S ScrollBar then it will leads to close the dropdown
            //So Here this condition will satisfy only if we click on the DropDown items.
            if (!this.IsInArrange && control.SelectedItem != null && mousePosition.X <= control.ItemPresenter.ActualWidth)
            {
                var item = control.SelectedItem;
                item = GetDisplayValue(control, item);
                if (!this.AlreadyTextChanged)
                {
                    //SettingControlText will refresh the TextBoxPart of CurrentCellUIElement also. No need to refresh the TextBoxPart separately
                    this.CurrentCellUIElement.Text = item.ToString();
                    CurrentStyleCopy.Tag = control.SelectedIndex;
                    SetControlText(item == null ? string.Empty : item.ToString());
                    CurrentStyleCopy.Tag = null;
                    RaiseSelectedItemChangedEvent(CellRowColumnIndex, item); 
                }
                if (this.HasCurrentCellState && this.CurrentCellUIElement!= null && this.CurrentCellUIElement.IsDropDownOpen)
                {
                   
                    this.CurrentCellUIElement.IsDropDownOpen = false;
                }
            }
        }

        private static object GetDisplayValue(HoverListBox control, object item)
        {
            if (item == null)
            {
                return null;
            }
            var type = item.GetType();
            if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
            {
                if (control.ItemsSource is ITypedList)
                {
                    var propcoll = ((ITypedList)(control.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                    item = propcoll.GetValue(item, control.DisplayMemberPath).ToString();
                }
                else
                    item = TypeDescriptor.GetProperties(item.GetType())[control.DisplayMemberPath].GetValue(item).ToString();
            }
            else 
            {
                return item.ToString();
            }
            return item;
        }

        protected override void UnwireTemplateParts(GridCellComboBoxDropDown uiElement)
        {
            if (uiElement.ListBoxPart != null)
            {
                uiElement.ListBoxPart.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(ListBoxPart_MouseLeftButtonUp);
                uiElement.ListBoxPart.MouseDown -= new MouseButtonEventHandler(ListBoxPart_MouseDown);
                uiElement.ListBoxPart.ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);
            }

            if (uiElement.TextBoxPart != null)
            {
                uiElement.TextBoxPart.RemoveHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted));
            }

            base.UnwireTemplateParts(uiElement);
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridCellComboBoxDropDown uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);
            var dropDownControl = uiElement;
                if (dropDownControl.ListBoxPart != null)
                {
                    var ComboBoxDataSource = this.ComboBoxModel.GetDataSource(style);
                    if (!UseAutoCompleteList)
                        dropDownControl.ListBoxPart.ItemsSource = ComboBoxDataSource;
                    dropDownControl.AutoSuggestionList = ComboBoxDataSource;
                    dropDownControl.ListBoxPart.DisplayMemberPath = style.HasDisplayMember ? style.DisplayMember : string.Empty;

                    dropDownControl.ListBoxPart.SelectedValuePath = style.ValueMember;
                    if (dropDownControl.ListBoxPart.Items != null && dropDownControl.ListBoxPart.Items.CurrentItem!=null)

                    if (this.GetControlValue(style) != null && this.GetControlValue(style).ToString() != string.Empty)
                    {

                        if (dropDownControl.ListBoxPart.IsSynchronizedWithCurrentItem != null)

                        {
                            dropDownControl.ListBoxPart.SelectedValue = this.GetControlValue(style);
                            dropDownControl.ListBoxPart.SelectedValue = this.GetControlValue(style).ToString();
                        }
                    }
                    dropDownControl.DropDownStyle = style.DropDownStyle;
                    if (style.HasValueMember)

                    {
                        dropDownControl.ListBoxPart.SelectedItem = dropDownControl.ListBoxPart.Items.CurrentItem;
                    }
                    dropDownControl.DropDownStyle = style.DropDownStyle;                  
                
            }
                if (style.IncrementalFilter == IncrementalFilter.Disable)
                {
                    var selectedIndex = this.ComboBoxModel.FindValue(style, style.CellValue);
                    if (selectedIndex > -1)
                    {
                        if (dropDownControl.ListBoxPart != null)
                        {
                            dropDownControl.ListBoxPart.SelectedIndex = selectedIndex;
                            var listBox = dropDownControl.ListBoxPart;
                            var item = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
                            if (item != null && !item.IsHighlighted)
                            {
                                item.IsHighlighted = true;
                            }
                        }
                    }
                }
        }

        protected override void OnIsDropDownOpenChanged(object sender, EventArgs e)
        {
            base.OnIsDropDownOpenChanged(sender, e);
            var dropDownControl = sender as GridCellComboBoxDropDown;
            var datagrid = this.GridControl.FindParentElementOfType<GridDataControl>();
            if (datagrid != null && !datagrid.AllowEdit)
                dropDownControl.IsDropDownOpen = false;
            if (dropDownControl.IsDropDownOpen)
            {
                if (dropDownControl.ListBoxPart.ItemsSource == null)
                {
                    dropDownControl.ListBoxPart.ItemsSource = CurrentStyle.ItemsSource as IEnumerable;
                }
                if (this.CurrentCell != null && !this.CurrentCell.IsEditing)
                {
                    if (!this.GridControl.IsKeyboardFocusWithin)
                    {
                        this.GridControl.Focus();
                    }
                }

                var listBox = dropDownControl.ListBoxPart;
                if (listBox.SelectedIndex == -1&& listBox.Items.Count>0)
                {
                    //This is Because if Bind the External itemssource means selectedindex set as previous cell.
                    //To Avoid that we have set selected index as 0.
                    listBox.SelectedIndex = 0;
                    listBox.Items.MoveCurrentTo(listBox.Items[0]);
                }

                if (dropDownControl.TextBoxPart.Text == string.Empty)
                {
                    listBox.SelectedIndex = -1;
                    listBox.HighlightedItem = null;
                }
                //Below code is for selecting proper item form textbox to the list.
                int count = this.GridListModel.ListModel.SourceList.Count;
                var text = dropDownControl.TextBoxPart.Text;
                for (int i = 0; i < count; i++)
                {
                    var itemText = this.GridListModel.ListModel.GetDisplayText(i);
                    if (text.Equals(itemText))
                    {
                        listBox.SelectedIndex = i;
                        break;
                    }
                }
                
                if (listBox.SelectedIndex > -1)
                {
                    listBox.ScrollIntoView(listBox.SelectedItem);
                    this.SetHighlightedItem(listBox.SelectedIndex);
                }
            }
        }


        protected void SetHighlightedItem(int index)
        {
            if (this.CurrentCellUIElement.ListBoxPart.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                //listBox.UpdateLayout();
                for (int i = 0; i < this.CurrentCellUIElement.ListBoxPart.Items.Count; i++)
                {
                    HoverListBoxItem highLightedItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
                    if (highLightedItem != null)
                        highLightedItem.IsHighlighted = false;
                }
               
                HoverListBoxItem highLightItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(index);
                if (highLightItem != null && !highLightItem.IsHighlighted)
                {
                    highLightItem.IsHighlighted = true;
                }
            }
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.ListBoxPart != null && this.CurrentCellUIElement.ListBoxPart.SelectedIndex >= 0)
                this.SetHighlightedItem(this.CurrentCellUIElement.ListBoxPart.SelectedIndex);
        }

        protected override void SetSelectedIndex(int index)
        {
            if (index != this.CurrentCellUIElement.ListBoxPart.SelectedIndex && index >= 0)
            {
                this.CurrentCellUIElement.ListBoxPart.SelectedIndex = index;
                this.SetHighlightedItem(index);
            }
        }

        protected override void OnActivated()
        {
            flag = false;
            base.OnActivated();            
        }

    

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (this.CurrentCellUIElement != null && !this.IsDisabled)
            {
                if (this.CurrentCellUIElement.DropDownStyle == GridDropDownStyle.AutoComplete)
                {
                    if (this.CurrentCellUIElement.ListBoxPart.SelectedIndex == -1)
                    {
                        this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                    }
                    (this.CurrentCellUIElement.ListBoxPart as HoverListBox).DropDownPopupHost.PopupHost.IsOpen = false;
                }
                UseAutoCompleteList = false;
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isAltKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;

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

            // to Open / Close the DropDown on Pressing Alt + up
            if (isAltKey && e.SystemKey == Key.Up)
            {
                // handle Alt + Up combination
                if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
                {
                    this.CurrentCellUIElement.IsDropDownOpen = !this.CurrentCellUIElement.IsDropDownOpen;
                    e.Handled = true;
                }
            }
            HoverListBox listBox;
            HoverListBoxItem listBoxItem;

            if ((isControlKey || isShiftKey) && this.CurrentCell.IsEditing && e.Key != Key.Tab)
            {
                return false;
            }

            switch (e.Key)
            {
                case Key.Tab:
                    if (this.CurrentCell.IsEditing)
                    {
                        listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                        this.EnsureCurrentItem(listBox);
                        if (!this.IsInArrange && listBox.SelectedItem != null && GridListModel.ListModel.CurrentIndex > -1)
                        {

                            if (this.IsDroppedDown)
                                listBox.SelectedItem = listBox.Items.CurrentItem;
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

                            if (!this.AlreadyTextChanged)
                            {
                                SetControlText(item == null ? string.Empty : item.ToString());
                                RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);
                            }
                        }
                    }
                    return true;
                case Key.Left:
                    if (this.CurrentCell.IsEditing && this.CurrentCellUIElement.DropDownStyle != GridDropDownStyle.Exclusive)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                        {
                            if (tb.CaretIndex == 0 && tb.SelectionLength == 0)
                            {
                                e.Handled = true;
                                return true;
                            }
                            else if (tb.SelectionLength == tb.Text.Length)
                            {
                                tb.CaretIndex = 0;
                                e.Handled = true;
                                return false;
                            }
                            else
                                return false;
                        }
                    }
                    return true;
                case Key.Right:
                    if (this.CurrentCell.IsEditing && this.CurrentCellUIElement.DropDownStyle != GridDropDownStyle.Exclusive)
                    {
                        TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                        if (tb != null)
                        {
                            if (tb.CaretIndex == 0 && tb.SelectionLength == 0)
                            {
                                //e.Handled = true;
                                return false;
                            }
                            else if (tb.SelectionLength == tb.Text.Length)
                            {
                                tb.CaretIndex = tb.Text.Length;
                                e.Handled = true;
                                return false;
                            }
                            else if (tb.CaretIndex == tb.Text.Length)
                                return true;

                            else
                                return false;
                        }
                    }
                    return true;
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

                        if (listBox.SelectedIndex < listBox.Items.Count)
                        {
                            // When the combo box is open while pressing DownKey the secon item is selected.
                            //Because the selected item is again increasedd by one in this method.
                            //if (listBox.SelectedIndex == -1)
                            //    listBox.SelectedIndex = 0;
                            if (listBox.Items.CurrentItem != null)
                            {
                                listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.Items.CurrentItem));
                                if (listBoxItem != null)
                                {
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                    listBoxItem.IsHighlighted = false;
                                    listBox.Items.MoveCurrentToNext();
                                    // listBox.SelectedItem = listBox.Items.CurrentItem;
                                    listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.SelectedItem) + 1);//(listBox.Items.CurrentItem));
                                    if (listBoxItem == null)
                                        listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.SelectedItem));//(listBox.Items.CurrentItem));
                                    listBox.NotifyListBoxItemEnter(listBoxItem);
                                    this.CanProcessKey = false;
                                    CurrentStyleCopy.Tag = listBox.SelectedIndex;
                                    //var setControl = SetControlText(listBoxItem == null ? string.Empty : listBoxItem.ToString());
                                    AlreadyTextChanged = true;
                                    this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                                    AlreadyTextChanged = false;
                                    this.CurrentCellUIElement.TextBoxPart.Text = GetDisplayValue(listBox, listBox.SelectedItem).ToString();// listBox.SelectedItem.ToString();
                                    CurrentStyleCopy.Tag = null;
                                    this.CurrentCellUIElement.TextBoxPart.SelectAll();
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                }
                                else
                                {
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                    this.SetHighlightedItem(listBox.SelectedIndex);
                                    
                                }
                            }
                        }
                    }
                    else
                    {
                        if (CurrentCellUIElement != null)
                        {
                            TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                            if (tb != null)
                                return true;
                        }
                        else
                        {
                            return true;
                        }
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
                                    // listBox.SelectedItem = listBox.Items.CurrentItem;
                                    listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(listBox.Items.IndexOf(listBox.SelectedItem) - 1);//(listBox.Items.CurrentItem));
                                    listBox.NotifyListBoxItemEnter(listBoxItem);
                                    this.CanProcessKey = false;
                                    CurrentStyleCopy.Tag = listBox.SelectedIndex;
                                    //var setControl = SetControlText(listBoxItem == null ? string.Empty : listBoxItem.ToString());
                                    AlreadyTextChanged = true;
                                    this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                                    AlreadyTextChanged = false;
                                    this.CurrentCellUIElement.TextBoxPart.Text = GetDisplayValue(listBox, listBox.SelectedItem).ToString();//listBox.SelectedItem.ToString();
                                    CurrentStyleCopy.Tag = null;
                                    this.CurrentCellUIElement.TextBoxPart.SelectAll();
                                    listBox.ScrollIntoView(listBox.SelectedItem);
                                }
                            }
                            else
                            {
                                listBox.ScrollIntoView(listBox.SelectedItem);
                                this.SetHighlightedItem(listBox.SelectedIndex);
                            }
                        }
                    }
                    else
                    {
                        if (this.CurrentCellUIElement != null)
                        {
                            TextBox tb = this.CurrentCellUIElement.TextBoxPart as TextBox;
                            if (tb != null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                    e.Handled = true;
                    return false;

                case Key.Return:
                    if (this.CurrentCell.IsEditing)
                    {
                        listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                        this.EnsureCurrentItem(listBox);
                        if (!this.IsInArrange && listBox.SelectedItem != null && GridListModel.ListModel.CurrentIndex > -1)
                        {

                            if (this.IsDroppedDown)
                                listBox.SelectedItem = listBox.Items.CurrentItem;

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
                        }
                        if (isShiftKey)
                        {
                            break;
                        }
                        else
                        {
                            if (this.CurrentStyle != null)
                            {
                                var renderer = this.CurrentCell.Renderer;
                                if (renderer != null)
                                {
                                    GridDataStyleInfo sif = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                                    if (sif != null && sif.CellIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell)
                                    {
                                        e.Handled = true;
                                    }
                                    else if (sif != null && sif.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell && !CurrentCell.IsEditing)
                                    {
                                        e.Handled = true;
                                    }
                                }
                            }
                            //if (CurrentCell.IsEditing)
                            //    {
                            //    CurrentCell.EndEdit();
                            //    e.Handled = true;
                            //    }
                            RaiseSelectedItemChangedEvent(CellRowColumnIndex, listBox.SelectedItem);
                            CurrentCell.MoveRight();
                            return true;
                            // break; warning CS0162
                        }
                    }
                    else
                    {
                        this.CurrentCell.MoveRight();
                    }
                    e.Handled = true;
                    return false;
                case Key.Escape:
                    if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
                    {
                        this.CurrentCellUIElement.IsDropDownOpen = false;
                        //Setting UserAutoCompleteList to false, forces the UI to load Default Itemssource - When the User
                        //Presses Escape Key for Autocomplete Combo box in Incremental Filtering
                        UseAutoCompleteList = false;
                    }
                    this.CurrentCell.CancelEdit();
                    this.CurrentCell.Refresh();
                    e.Handled = true;
                    break;
                case Key.F2:
                    {
                        if (this.CurrentCell.IsEditing && this.CurrentStyle.DropDownStyle != GridDropDownStyle.Exclusive)
                        {
                            var dropDownControl = this.CurrentCellUIElement as GridCellComboBoxDropDown;
                            var style = this.CurrentStyle;
                            if (!(dropDownControl.TextBoxPart as TextBox).IsFocused)
                            {
                                dropDownControl.TextBoxPart.Focus();
                                dropDownControl.TextBoxPart.CaretIndex = dropDownControl.TextBoxPart.Text.Length;
                                style.CellValue = dropDownControl.TextBoxPart.Text;
                            }
                            else
                            {
                                this.CurrentCell.EndEdit();
                            }
                            e.Handled = true;
                            return false;
                        }
                        else if (!this.CurrentCell.IsEditing)
                        {
                            flag = true;
                        }
                    }
                    break;
            }

            return true;
        }

        private void EnsureCurrentItem(HoverListBox listBox)
        {
            if ( listBox.SelectedIndex > -1)
            {
                listBox.Items.MoveCurrentTo(listBox.SelectedItem);
            }
        }

        protected override object GetControlValueFromEditor()
        {
            var control = this.CurrentCellUIElement as GridCellComboBoxDropDown;
            if (control != null && control.ListBoxPart != null && control.ListBoxPart.SelectedItem != null)
            {
                object item = control.ListBoxPart.SelectedItem;
                var type = item.GetType();
                if (control != null && NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type) && control.ListBoxPart != null && control.ListBoxPart.SelectedValuePath != "" && item != null && !(item is DBNull))
                {
                    if (control.ListBoxPart.ItemsSource is ITypedList)
                    {
                        var propcoll = ((ITypedList)(control.ListBoxPart.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                        item = propcoll.GetValue(item, control.ListBoxPart.SelectedValuePath);
                    }
                    else
                        item = TypeDescriptor.GetProperties(type)[control.ListBoxPart.SelectedValuePath].GetValue(item);

                    return item;
                }
            }
            return base.GetControlValueFromEditor();
        }
        public override object ControlValue
        {
            get
            {
                object item = base.ControlValue;
                if (item != null)
                {

                    var type = item.GetType();
                    var control = this.CurrentCellUIElement as GridCellComboBoxDropDown;
                    if (control != null && NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type) && control.ListBoxPart != null && control.ListBoxPart.SelectedValuePath != "" && item != null && !(item is DBNull))
                    {
                        if (control.ListBoxPart.ItemsSource is ITypedList)
                        {
                            var propcoll = ((ITypedList)(control.ListBoxPart.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                            item = propcoll.GetValue(item, control.ListBoxPart.SelectedValuePath);
                        }
                        else
                            item = TypeDescriptor.GetProperties(type)[control.ListBoxPart.SelectedValuePath].GetValue(item);
                    }
                    else if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type) && this.CurrentStyle != null && !string.IsNullOrEmpty(this.CurrentStyle.ValueMember) && item != null && !(item is DBNull))
                    {
                        item = TypeDescriptor.GetProperties(type)[this.CurrentStyle.ValueMember].GetValue(item);
                    }

                }
                return item;
            }
            set
            {
                base.ControlValue = value;
            }
        }

        #region handling ApplicationCommands.Paste
        private void CommandExecuted(object sender, RoutedEventArgs e)
        {
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Paste)
            {
                if (e.Handled)
                {
                    var textBox = sender as TextBox;
                    var actualText = textBox.Text;
                    actualText = actualText.Trim();
                    var filterItemText = string.Empty;
                    var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
                    HoverListBox listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
                    if (idx > -1)
                    {
                        this.SetSelectedIndex(idx);
                        listBox.HighlightedItem = null;
                        listBox.Items.MoveCurrentTo(actualText);
                        listBox.Items.MoveCurrentTo(actualText);
                        listBox.SelectedItem = listBox.Items.CurrentItem;
                        var itemIndex = listBox.Items.IndexOf(listBox.Items.CurrentItem);

                        if (itemIndex > -1)
                        {
                            this.CurrentCellUIElement.IsDropDownOpen = true;
                            var listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(itemIndex);
                            listBox.NotifyListBoxItemEnter(listBoxItem);
                            this.CurrentCellUIElement.IsDropDownOpen = false;
                        }
                    }
                    else if (this.CurrentStyle.DropDownStyle == GridDropDownStyle.AutoComplete || this.CurrentStyle.DropDownStyle == GridDropDownStyle.Exclusive)
                    {
                        this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                        listBox.Items.MoveCurrentTo(actualText);
                        listBox.HighlightedItem = null;
                        listBox.SelectedItem = null;
                    }
                    else
                    {
                        listBox.Items.MoveCurrentTo(actualText);
                        listBox.HighlightedItem = null;
                        listBox.SelectedItem = null;
                    }
                }
            }
        }

        //protected override void OnClipboardPaste(GridCutPasteEventArgs args)
        //{
        //    ////var actualText = args.ClipboardText;
        //    ////if(actualText==string.Empty)
        //    ////if(this.GridControl.Model.TextDataExchange.TabDelimiter != string.Empty)
        //    ////actualText = actualText.Replace(this.GridControl.Model.TextDataExchange.TabDelimiter, string.Empty);
        //    ////var filterItemText = string.Empty;
        //    ////var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
        //    ////HoverListBox listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
        //    ////if (idx > -1)
        //    ////{
        //    ////    this.SetSelectedIndex(idx);
        //    ////    listBox.HighlightedItem = null;
        //    ////    listBox.Items.MoveCurrentTo(actualText);
        //    ////    listBox.Items.MoveCurrentTo(actualText);
        //    ////    listBox.SelectedItem = listBox.Items.CurrentItem;
        //    ////    var itemIndex = listBox.Items.IndexOf(listBox.Items.CurrentItem);

        //    ////    if (itemIndex > -1)
        //    ////    {
        //    ////        this.CurrentCellUIElement.IsDropDownOpen = true;
        //    ////        var listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(itemIndex);
        //    ////        listBox.NotifyListBoxItemEnter(listBoxItem);
        //    ////        this.CurrentCellUIElement.IsDropDownOpen = false;
        //    ////    }
        //    ////    args.Handled = true;
        //    ////}
        //    ////else
        //    ////{
        //    ////    args.Handled = true;
        //    ////}

        //   base.OnClipboardPaste(args);
        //}
        /// <summary>
        /// This event after clipboard paste
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClipboardPasted(GridCutPasteEventArgs args)
        {
            var actualText = args.ClipboardText;
            if (string.IsNullOrEmpty(string.Empty))
            {
                base.OnClipboardPasted(args);
                if (!this.CurrentCell.IsEditing)
                    this.CurrentCell.EndEdit();
                return;
            }
            actualText = actualText.Replace(this.GridControl.Model.TextDataExchange.TabDelimiter, string.Empty);
            var filterItemText = string.Empty;
            var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
            HoverListBox listBox = this.CurrentCellUIElement.ListBoxPart as HoverListBox;
            if (idx > -1)
            {
                this.SetSelectedIndex(idx);
                listBox.HighlightedItem = null;
                listBox.Items.MoveCurrentTo(actualText);
                listBox.Items.MoveCurrentTo(actualText);
                listBox.SelectedItem = listBox.Items.CurrentItem;
                var itemIndex = listBox.Items.IndexOf(listBox.Items.CurrentItem);

                if (itemIndex > -1)
                {
                    this.CurrentCellUIElement.IsDropDownOpen = true;
                    var listBoxItem = (HoverListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(itemIndex);
                    listBox.NotifyListBoxItemEnter(listBoxItem);
                    this.CurrentCellUIElement.IsDropDownOpen = false;
                }
                args.Handled = true;
            }
            else
            {
                args.Handled = true;
            }

            base.OnClipboardPasted(args);
        }
        #endregion


    }
}