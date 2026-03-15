#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Data;
    using System.ComponentModel;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Grid.GridCellRenderer.DropdownCellRenderers;

    /// <summary>
    /// Provides interface for DropDownList Control cells.
    /// Defines a property that returns the model of the DropDownList control
    /// and a method to define the choice list.
    /// </summary>
    public interface IGridListModel
    {
        GridListModel ListModel
        {
            get;
        }

        void FillWithChoices(GridStyleInfo style, out bool exclusive);
    }

    /// <summary>
    /// Implements the model part for drop-down list cells.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridCellDropDownCellModel<T> : GridCellModel<T>, IGridListModel
        where T : IGridCellRenderer, new()
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellDropDownCellModel"/>.
        /// </summary>
        public GridCellDropDownCellModel()
        {
            this.ListModel = new GridListModel();
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds"></param>
        /// <returns>The optimal size of the cell.</returns>
        protected override Size OnQueryPrefferedClientSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            string text = style.CellValue != null ? style.CellValue.ToString() : string.Empty;

            if (string.IsNullOrEmpty(text))
                text = MeasureEmptyCellString;

            Size clientSize = GetCellClientSize(rowIndex, colIndex, style);

            Size textSize = GridTextBoxPaint.MeasureText(clientSize, text, style, queryBounds);

            return textSize;
        }

        /// <summary>
        /// Calculates the preferred size of the drop-down list cell based on its content, including cell margins. 
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="colIndex">Cell column index.</param>
        /// <param name="style">Cell style information.</param>
        /// <param name="queryBounds">Graphical bounds.</param>
        /// <returns>The optimal size of the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
            {
                return Size.Empty;
            }

            Thickness margins = style.TextMargins.ToThickness();

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            size.Width += SystemParameters.VerticalScrollBarWidth + 5d; // dropdown button size
            return size;
        }

        /// <summary>
        /// Returns the model of the drop-down list control. 
        /// </summary>
        public GridListModel ListModel
        {
            get;
            private set;
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value based on the ChoiceList or ItemsSource,
        /// DisplayMember and ValueMember of the combobox cell.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            if (text.Length > 0 && (style.ChoiceList == null || style.ChoiceList.Count == 0))
            {
                if (style.ItemsSource != null && style.DisplayMember != style.ValueMember)
                {
                    bool exclusive;
                    this.FillWithChoices(style, out exclusive);
                    int index;
                    if (style.Tag == null || !int.TryParse(style.Tag.ToString(), out index))
                        index = this.ListModel.IndexOfItemByDisplayText(text);
                  
                    if (index != -1)
                    {
                        //GridModel.SelectedIndex = index;
                        style.CellValue = this.ListModel.GetItem(index); /*this.ListModel.GetValue(this.ListModel.GetItem(index));*/
                        return true;
                    }
                    if (exclusive)
                    {
                        //Throw new GridException(text + " is not found in choice list").
                        return false;
                    }
                }
                else if (style.ItemsSource != null && style.CellValueType != null && style.CellValueType.IsEnum)
                {
                    style.CellValue = text;
                    return true;
                }
            }


            return base.ApplyFormattedText(style, text, textInfo);
        }

        //private bool CheckEnumType(object p)
        //{
        //    var enumerator = (p as IEnumerable).GetEnumerator();
        //    if (enumerator.MoveNext())
        //    {
        //        var firstItemType = enumerator.Current.GetType();
        //        if (firstItemType.IsEnum)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        /// <summary>
        /// Initializes a <see cref="ListModel"/> with data binding information from a <see cref="GridStyleInfo"/>
        /// object.
        /// </summary>
        /// <param name="style">The style object with binding information.</param>
        /// <param name="exclusive">A place holder that returns whether the list box is filled with an exclusive
        /// list of possible choices or if non-standard values are allowed.
        /// </param>
        public virtual void FillWithChoices(GridStyleInfo style, out bool exclusive)
        {
            ListModel.AutoPopulateDropDownColumns = style.AutoPopulateDropDownColumns;
            ListModel.DropDownVisibleColumns = style.DropDownVisibleColumns;
            ListModel.DropDownColumnSizer = style.DropDownColumnSizer;
            
            IEnumerable itemsSource = this.GetDataSource(style);

            if (itemsSource != null)
            {
                if (itemsSource != ListModel.ItemsSource
                    || ListModel.DisplayMember != style.DisplayMember
                    || ListModel.ValueMember != style.ValueMember)
                {
                    // clear out
                    this.ListModel.ItemsSource = null;
                    this.ListModel.DisplayMember = string.Empty;
                    this.ListModel.ValueMember = string.Empty;

                    // and initialize with settings
                    this.ListModel.ItemsSource = itemsSource;
                    if (style.ChoiceList == null || style.ChoiceList.Count == 0)
                    {
                        this.ListModel.DisplayMember = style.DisplayMember;
                        this.ListModel.ValueMember = style.ValueMember;
                    }
                }
            }

            exclusive = !style.IsEditable;
        }

        /// <summary>
        /// Returns the index in the drop-down list box for the specified cell value / key.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value (same as ValueMember).</param>
        /// <returns>The index in the drop-down list box or -1 if not found.</returns>
        public int FindValue(GridStyleInfo style, object value)
        {
            if (value == null || value is string && value.Equals(string.Empty))
            {
                return -1;
            }

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                bool exclusive;
                this.FillWithChoices(style, out exclusive);
                if (this.ListModel.ItemsSource != null)
                {
                    //GridModel.SelectedIndex = -1;
                    //GridModel.SelectedValue = value;
                    return this.ListModel.IndexOfItemByValue(value);
                }
            }
            else if (style.ChoiceList != null && value is string)
            {
                return style.ChoiceList.IndexOf((string)value);
            }

            return -1;
        }

        /// <summary>
        /// This is called to initialize data source on demand. This lets you calculate the datasource
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.ItemsSource is returned.
        /// </summary>
        /// <param name="style">The cell style information.</param>
        /// <returns>The style datasource.</returns>
        public virtual IEnumerable GetDataSource(GridStyleInfo style)
        {
            return this.ListModel.GetStyleDataSource(style);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                if (style.ItemsSource != null && style.DisplayMember != style.ValueMember)
                {
                    bool exclusive;
                    this.FillWithChoices(style, out exclusive);
                    if (ListModel.ItemsSource != null)
                    {
                        if (style.ValueMember == string.Empty)
                        {
                            if (value == null || value is string || value.GetType().IsPrimitive)
                            {
                                if (exclusive)
                                {
                                    return string.Empty;
                                }
                            }
                            else
                            {
                                return this.ListModel.GetDisplayText(value);
                            }
                        }
                        else
                        {
                            // returns the ValueMemberPD value
                            var searchValue = value != null && value.ToString() != string.Empty ? value : null;
                            int index = this.ListModel.IndexOfItemByValue(searchValue);
                            if (index != -1)
                            {
                                return this.ListModel.GetDisplayText(ListModel.GetItem(index));
                            }

                            if (exclusive)
                            {
                                return string.Empty;
                            }
                        }
                    }
                }
            }
            else if (style.ChoiceList.Count > 0 && this.ListModel.CurrentIndex > -1)
            {
                bool exclusive;
                this.FillWithChoices(style, out exclusive);
                //if (this.ListModel.CurrentIndex > -1 && this.ListModel.CurrentIndex < style.ChoiceList.Count)
                //{
                //    return style.ChoiceList[this.ListModel.CurrentIndex];
                //}
                var idx = this.FindValue(style, value);
                if (idx > -1 && idx < style.ChoiceList.Count)
                {
                    return style.ChoiceList[idx];
                }

                // just return the first index if we do not find a match
                //return style.ChoiceList[0];
            }
            return base.GetText(style, value);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            return this.GetText(style, value);
        }

        /// <summary>
        /// Returns the value for the ValueMember of the specified item.
        /// </summary>
        /// <param name="item">The row item.</param>
        /// <param name="itemsSource">The list</param>
        /// <param name="valueMember">The name of the value member</param>
        /// <returns>The value of the ValueMember.</returns>
        public object GetItemValue(object itemsSource, string valueMember, object item)
        {
            return ListUtil.GetItemValue(itemsSource, valueMember, item);
        }
    }

    /// <summary>
    /// Renders drop-down list control in a grid cell.
    /// </summary>
    /// <typeparam name="T">The <see cref="GridCellDropDownControlBase"/>.</typeparam>
    public class GridCellDropDownCellRenderer<T> : GridVirtualizingCellRenderer<T>
        where T : GridCellDropDownControlBase, new()
    {
      
        internal DropDownPaint dropDownPaint;
        private string CurrentVisualStyle = string.Empty;
        /// <summary>
        /// Initializes the <see cref="GridCellDropDownCellRenderer"/> object.
        /// </summary>
        public GridCellDropDownCellRenderer()
        {
            this.SupportsRenderOptimization = true;
            this.AllowRecycle = true;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.AllowKeepAliveOnlyCurrentCell = true;
            this.dropDownPaint = new DropDownPaint();
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
               // margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            //margins.Left = Math.Max(margins.Left, 2);
            //margins.Right = Math.Max(margins.Right, 2);

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            if (textRectangle.IsEmpty)
            {
                return;
            }

            string text = string.Empty;
            if (this.IsCurrentCell(style) && this.HasControlText)
            {
                text = this.ControlText;
            }
            else
            {
                text = this.GetControlText(style);
            }

            //this.dropDownPaint.DrawDropDown(dc, textRectangle, text, style);
            // Draw the formatted text string to the DrawingContext of the control.
            //GridTextBoxPaint.DrawText(dc, textRectangle, text, style);

            //By default ShowButton is set to True
            //Dropdown button wont appear if ShowButton property is False.
            if (style.DropdownEdit.ShowButton)
                this.dropDownPaint.DrawDropDown(dc, textRectangle, text, style);
            else
                GridTextBoxPaint.DrawText(dc, textRectangle, text, style);

        }

        protected override string GetControlTextFromEditorCore(T uiElement)
        {
            return uiElement.TextBoxPart == null ? uiElement.Text : uiElement.TextBoxPart.Text;
        }

        /// <summary>
        /// Initializes the content of the drop-down list cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="dropDownControl">The drop-down list control.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(T dropDownControl, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(dropDownControl, style);            
            if (this.GridControl is GridDataControlBaseImpl && ((GridDataTableModel)this.GridControl.Model).TableProperties.EnableVisualStyleForEditors)
            {
                dropDownControl = this.GetDropDownVisualStyle(dropDownControl, style);
            }
            
            //Thickness margins = style.TextMargins.ToThickness();
            //margins.Left = Math.Max(0, margins.Left - 2);
            //margins.Right = Math.Max(0, margins.Right - 2);
            //dropDownControl.Padding = margins;
            dropDownControl.BorderThickness = new Thickness(0);
            // TextBoxPart will only be set once template has been applied with .Arrange call.
            if (this.IsCurrentCell(style) && this.HasControlText)
            {
                dropDownControl.Text = this.ControlText;
            }
            else
            {
                dropDownControl.Text = this.GetControlText(style);
            }

            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                dropDownControl.FlowDirection = style.FlowDirection;

                double m11 = -1;
                double m22 = 1;
                double offsetX = dropDownControl.Width;
                double offsetY = 0;
                dropDownControl.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            }
            else
            {
                dropDownControl.LayoutTransform = MatrixTransform.Identity;
            }

            ///if (this.GridControl is GridDataControlBaseImpl && ((GridDataTableModel)this.GridControl.Model).TableProperties.EnableVisualStyleForEditors)
            {
                if (dropDownControl.TextBoxPart != null)
                {
                    dropDownControl.TextBoxPart.Background = Brushes.White;
                    dropDownControl.TextBoxPart.Foreground = Brushes.Black;
                }

            }
            dropDownControl.IsMouseTrackingEnabled = style.IsMouseTrackingEnabled;
        }

        public T GetDropDownVisualStyle(T dropdown, GridStyleInfo style)
        {
            if (style.GridModel is GridDataTableModel)
            {
                var gridTableModel = style.GridModel as GridDataTableModel;
                if (style.IsThemed)
                    gridTableModel.GetVisualStyleDictionary(dropdown);
            }
            return dropdown;
        }

        /// <summary>
        /// Gets the GridListModel, i.e. the model of the drop-down control.
        /// </summary>
        public IGridListModel GridListModel
        {
            get
            {
                return ((IGridListModel)this.CellModel);
            }
        }

        /// <summary>
        /// Finds text in the list box.
        /// </summary>
        /// <param name="prefix">The text (or prefix) to find.</param>
        /// <param name="selectItem">True if you want to select the text in the list box.</param>
        /// <param name="start">The first index to start searching.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive.</param>
        /// <param name="filteredText">The filtered text.</param>
        /// <returns>The index of the entry that starts with the text; -1 if
        /// no entry could be found.</returns>
        public virtual int FindItem(string prefix, bool selectItem, int start, bool ignoreCase, out string filteredText)
        {
            if (this.GridListModel.ListModel.ItemsSource == null)
            {
                filteredText = string.Empty;
                return -1;
            }

            if (ignoreCase)
            {
                prefix = prefix.ToUpper();
            }

            int count = this.GridListModel.ListModel.SourceList.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + start + 1) % count;
                string itemText = this.GridListModel.ListModel.GetDisplayText(index); //this.GridListModel.Model.SourceList[index].ToString();
                if (itemText.Length >= prefix.Length)
                {
                    itemText = itemText.Substring(0, prefix.Length);

                    if (ignoreCase)
                    {
                        itemText = itemText.ToUpper();
                    }

                    if (prefix == itemText)
                    {
                        //if (!this.IsAutoComplete)
                        //{
                            this.SetSelectedIndex(index);
                        //}

                        filteredText = this.GridListModel.ListModel.GetDisplayText(index);//this.GridListModel.Model.SourceList[index].ToString();
                        return index;
                    }
                }
            }

            if (selectItem)
            {
                this.ClearSelectedIndex();
            }
            filteredText = string.Empty;
            this.actualText = "";
            return -1;
        }

        protected virtual void SetSelectedIndex(int index)
        {
            // this will be called when the item is searched inside the Model's SourceList
        }

        protected virtual void ClearSelectedIndex()
        {
            // if item was not found, clear the index in the derived control
            this.GridListModel.ListModel.CurrentIndex = -1;
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, T uiElement, GridRenderStyleInfo style)
        {
            // we have the OnApplyTemplate called in our base control in this function, and so we wire/unwire here
            //this.OnUnwireUIElement(uiElement);
            base.ArrangeUIElement(aca, uiElement, style);
            if (style.HasStaysOpenOnEdit)
            {
                uiElement.StaysOpenOnEdit = style.StaysOpenOnEdit;
            }
            bool exclusive;
            this.GridListModel.FillWithChoices(style, out exclusive);
            // this.OnWireUIElement(uiElement);
        }


        protected override void OnWireUIElement(T uiElement)
        {
            base.OnWireUIElement(uiElement);
            if (uiElement.TextBoxPart == null)
                uiElement.AppliedTemplate += new EventHandler(uiElement_AppliedTemplate);
            else
                WireTemplateParts(uiElement);
            uiElement.Unloaded += new RoutedEventHandler(uiElement_Unloaded);
            this.GridControl.ScrollRows.Changed += new EventHandler(Scroll_Changed);
            this.GridControl.ScrollColumns.Changed += new EventHandler(Scroll_Changed);
        }

        //Since the OnUnwireUIElements were not called for GridListControl, Unload event is listened and the scroll change events are unwired.
        void uiElement_Unloaded(object sender, RoutedEventArgs e)
        {
            this.GridControl.ScrollRows.Changed -= new EventHandler(Scroll_Changed);
            this.GridControl.ScrollColumns.Changed -= new EventHandler(Scroll_Changed);
            (sender as GridCellDropDownControlBase).Unloaded -= new RoutedEventHandler(uiElement_Unloaded);
        }

       
        void Scroll_Changed(object sender, EventArgs e)
        {
            if (this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
            {
                this.CurrentCellUIElement.IsDropDownOpen = false;
                //Here the focus of the UIElement is lost the focus while the dropdown part is closed. so the focus is set to textbox part.
                if (this.CurrentCellUIElement.TextBoxPart != null && !this.CurrentCellUIElement.TextBoxPart.IsReadOnly)
                    this.CurrentCellUIElement.TextBoxPart.Focus();
            }
        }


        protected override void OnUnwireUIElement(T uiElement)
        {
            base.OnUnwireUIElement(uiElement);
            if (uiElement.TextBoxPart == null)
                uiElement.AppliedTemplate -= new EventHandler(uiElement_AppliedTemplate);
            else
                UnwireTemplateParts(uiElement);
            this.GridControl.ScrollRows.Changed -= new EventHandler(Scroll_Changed);
            this.GridControl.ScrollColumns.Changed -= new EventHandler(Scroll_Changed);
            uiElement.Unloaded -= new RoutedEventHandler(uiElement_Unloaded);
        }

        void uiElement_AppliedTemplate(object sender, EventArgs e)
        {
            T uiElement = (T)sender;
            WireTemplateParts(uiElement);
            uiElement.AppliedTemplate -= new EventHandler(uiElement_AppliedTemplate);
        }

        protected virtual void WireTemplateParts(T uiElement)
        {    
            uiElement.PopupContent.MouseLeftButtonUp += new MouseButtonEventHandler(PopupContent_MouseLeftButtonUp);
            uiElement.TextBoxPart.TextChanged += new System.Windows.Controls.TextChangedEventHandler(OnTextBoxPartTextChanged);
            uiElement.TextBoxPart.MouseRightButtonUp += new MouseButtonEventHandler(TextBoxPart_MouseRightButtonUp);
            uiElement.TextBoxPart.PreviewKeyDown += new KeyEventHandler(OnTextBoxPartPreviewKeyDown);
            uiElement.TextBoxPart.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(OnTextBoxPartKeyDown), true);           
            uiElement.IsDropDownOpenChanged += new EventHandler(OnIsDropDownOpenChanged);
            uiElement.IsDropDownChanging += new GridCellShowingDropDownEventHandler(OnIsDropDownChanging);
        }


        internal RowColumnIndex mouseDownCell = RowColumnIndex.Empty;
        internal bool overComboButton = false;
        double comboWidth = 15;


        protected virtual void UnwireTemplateParts(T uiElement)
        {
            uiElement.PopupContent.MouseLeftButtonUp -= new MouseButtonEventHandler(PopupContent_MouseLeftButtonUp);
            uiElement.TextBoxPart.TextChanged -= new System.Windows.Controls.TextChangedEventHandler(OnTextBoxPartTextChanged);
            uiElement.TextBoxPart.MouseRightButtonUp -= new MouseButtonEventHandler(TextBoxPart_MouseRightButtonUp);
            uiElement.TextBoxPart.PreviewKeyDown -= new KeyEventHandler(OnTextBoxPartPreviewKeyDown);           
            uiElement.TextBoxPart.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(OnTextBoxPartKeyDown));
            uiElement.IsDropDownOpenChanged -= new EventHandler(OnIsDropDownOpenChanged);
            uiElement.IsDropDownChanging -= new GridCellShowingDropDownEventHandler(OnIsDropDownChanging);
        }

        void TextBoxPart_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.GridControl.Model.DisableEditorsContextMenu)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// This method used to get the Displayvalue
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>

        private static object GetDisplayValue(GridListModel model, object item)
        {
           
            if (model != null && item!=null)
            {
                var type = item.GetType();
                if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
                {
                    if (model.ItemsSource is ITypedList)
                    {
                        var propcoll = ((ITypedList)(model.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;

                        item = propcoll.GetValue(item, model.DisplayMember);

                    }
                    else
                    {
                        if(model.DisplayMember!=string.Empty)
                            item = TypeDescriptor.GetProperties(item.GetType())[model.DisplayMember].GetValue(item);
                        else
                            return item;
                    }
                }
                else
                {
                    return item;
                }
            }           
            return item;
        }

        /// <summary>
        /// This method used to get the Value Member
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static object GetValueMember(GridListModel model, object item)
        {
            if (model != null && item != null && !string.IsNullOrEmpty(item.ToString()))
            {
                var type = item.GetType();
                if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
                {
                    if (model.ItemsSource is ITypedList)
                    {
                        var propcoll = ((ITypedList)(model.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                        item = propcoll.GetValue(item, model.ValueMember);
                    }
                    else
                    {
                        if (model.ValueMember != string.Empty)
                            item = TypeDescriptor.GetProperties(item.GetType())[model.ValueMember].GetValue(item);
                        else
                            return item;
                    }
                }
                else
                {
                    return item;
                }
            }
            return item;
        } 


        void PopupContent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var control = e.Source as GridListControl;
            if (control!=null && !this.IsInArrange )
            {
                var item = control.Model.CurrentItem;
                if (item == null)
                {
                    return;
                }
               
                item = GetDisplayValue(control.Model,item).ToString();

                if (!this.AlreadyTextChanged)
                {
                    CurrentStyleCopy.Tag = control.Model.CurrentIndex;
                    SetControlText(item == null ? string.Empty : item.ToString());
                    CurrentStyleCopy.Tag = null;
                    RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);
                 
                }
            }      
          
        }

        private void OnIsDropDownChanging(object sender, GridCellShowingDropDownEventArgs e)
        {
            //Some times Item source doesnt set Because GridListControlPart are null in OnInitializeContent So we have to set the itemsource while opening the DropDown
            var ListBoxPart = sender as GridCellGridListControlDropDown;
            if (ListBoxPart != null && ListBoxPart.GridListControlPart.Model.ItemsSource == null)
            {
                
                ListBoxPart.GridListControlPart.Model.DisplayMember = this.GridListModel.ListModel.DisplayMember;
                ListBoxPart.GridListControlPart.Model.ValueMember = this.GridListModel.ListModel.ValueMember;
                ListBoxPart.GridListControlPart.Model.ItemsSource = this.GridListModel.ListModel.ItemsSource;

                if (this.CurrentStyle.HasShowDropDownHeaders)
                {
                    var showHeaders = this.CurrentStyle.ShowDropDownHeaders;
                    if (showHeaders)
                    {
                        ListBoxPart.GridListControlPart.Model.RowHeights.SetHidden(0, 0, false);
                    }
                    else
                    {
                        ListBoxPart.GridListControlPart.Model.RowHeights.SetHidden(0, 0, true);
                    }
                }
                ListBoxPart.GridListControlPart.Model.Sizer.ResizeColumns();
            }
            if (ListBoxPart != null)
            {
                var item = GetValueMember(ListBoxPart.GridListControlPart.Model, this.GetControlValueFromEditor());
                var index = ListBoxPart.GridListControlPart.Model.IndexOfItemByValue(item);
                if (index > -1)
                {
                    this.GridControl.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ListBoxPart.GridListControlPart.Model.CurrentIndex = index;
                        ListBoxPart.GridListControlPart.Model.SelectedRanges.Clear();
                        ListBoxPart.GridListControlPart.ScrollInView(new RowColumnIndex(index + 1, 0));
                        ListBoxPart.GridListControlPart.Model.SelectedRanges.Add(GridRangeInfo.Row(index + 1));
                    }));
                    
                }
            }

            if (this.SuspendEvents)
            {
                return;
            }

            e.Cancel = !this.GridControl.RaiseCurrentCellShowingDropDown(e.IsDropDownOpen);

        }

        protected virtual void OnIsDropDownOpenChanged(object sender, EventArgs e)
        {
            if (this.SuspendEvents)
            {
                return;
            }

            var dropDownControl = sender as GridCellDropDownControlBase;
            if (!dropDownControl.IsDropDownOpen)
            {
                if (Mouse.Captured != this.GridControl)
                {
                    this.GridControl.Focus();
                }
            }

            if (dropDownControl.IsDropDownOpen)
            {
                this.GridControl.RaiseCurrentCellShowedDropDown();
            }
            else
            {
                this.GridControl.RaiseCurrentCellClosedDropDown();
            }

            if (!dropDownControl.IsDropDownOpen && escFlag)
            {
                this.CurrentCell.EndEdit();
            }

            escFlag = false;
        }

        int ccSelectionStart, ccSelectionLength;
        /// <summary>
        /// Use this variable to check if the key can be processed
        /// </summary>
        protected bool CanProcessKey
        {
            get;
            set;
        }

        private bool escFlag = false;

        protected virtual void OnTextBoxPartPreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.actualText = string.Empty;// This code Added Because In AutoComplete ComboBox Two times key press needed to enter to enter the text.
           
            if (this.SuspendEvents)
            {
                return;
            }

            var textBox = (TextBox)sender;
            switch (e.Key)
            {
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (textBox != null)
                    {
                        ccSelectionStart = textBox.SelectionStart;
                        ccSelectionLength = textBox.SelectionLength;
                    }
                    break;


            }
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (this.CurrentCell.IsEditing)
                {
                    if (this.CurrentStyle.DropDownStyle == GridDropDownStyle.AutoComplete)
                    {
                        var index = this.CurrentCellUIElement.TextBoxPart.CaretIndex;
                        var textlength = this.CurrentCellUIElement.TextBoxPart.Text.Length;
                        if (index >= 1)
                        {
                            this.CurrentCellUIElement.TextBoxPart.CaretIndex = index - 1;
                            var newindex = this.CurrentCellUIElement.TextBoxPart.CaretIndex;
                            if (newindex <= 0)
                            {
                                newindex = 0;
                            }
                            this.CurrentCellUIElement.TextBoxPart.Select(newindex, textlength);
                            string _temp = CurrentCellUIElement.TextBoxPart.Text.ToString();
                            textBox.Tag = _temp.Substring(0, this.CurrentCellUIElement.TextBoxPart.CaretIndex);
                        }
                        if (this.CurrentCellUIElement.TextBoxPart.SelectionLength == this.CurrentCellUIElement.TextBoxPart.Text.Length)
                        {
                            this.CurrentCellUIElement.TextBoxPart.Text = "";
                        }
                        e.Handled = true;
                        return;
                    }
                }
                this.CanProcessKey = false;
                this.actualText = string.Empty;
            }
            else
            {
                this.CanProcessKey = true;
            }
            if (!this.IsAllowNewEntries && char.IsLetterOrDigit(e.Key.ToString(), 0) && this.actualText != string.Empty)
            {
                var key = e.Key.ToString();
                if (e.Key == Key.Space)
                {
                    // handle space with special condition here,
                    key = " ";
                }
                if (e.Key == Key.OemPeriod)
                {
                    // handle dot(Period) with special condition here,
                    key = ".";
                }
                var text = this.actualText + key;
                int value = -1;
                // numpad 
                if ((int)e.Key >= ((int)Key.NumPad0) && (int)e.Key <= ((int)Key.NumPad9))
                {
                    value = (int)e.Key - ((int)Key.NumPad0);
                }
                // regular numbers  
                else if ((int)e.Key >= ((int)Key.D0) && (int)e.Key <= ((int)Key.D9))
                {
                    value = (int)e.Key - ((int)Key.D0);
                }

                if (value != -1)
                {
                    text = this.actualText + value.ToString();
                }

                var filterItemText = string.Empty;
                var idx = this.FindItem(text, true, -1, true, out filterItemText);
                if (idx == -1)
                {
                    e.Handled = true;
                }
            }
            // !Fix -> if we have the textbox selection length and the actual text same, we simply clear out the actual text, because the user has 
            // entered a new text
            if (textBox != null && textBox.SelectionLength == this.actualText.Length)
            {
                this.actualText = string.Empty;
            }
        }

        protected virtual void OnTextBoxPartKeyDown(object sender, KeyEventArgs e)
        {
            if (this.SuspendEvents)
            {
                return;
            }

            TextBox textBox = (TextBox)sender;
            switch (e.Key)
            {
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (ccSelectionStart == textBox.SelectionStart
                        && ccSelectionLength == textBox.SelectionLength)
                        this.GridControl.MoveCurrentCellWithArrowKey(e);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text is already changed.
        /// </summary>
        /// <value><c>true</c> if [already text changed]; otherwise, <c>false</c>.</value>
        protected bool AlreadyTextChanged
        {
            get;
            set;
        }

        protected bool SuspendEvents
        {
            get;
            set;
        }
        string str = string.Empty;
        int _ccSelectionstart = 0, _ccSelectionLength = 0;
        protected virtual void OnTextBoxPartTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.AlreadyTextChanged || InInitializeContent || this.SuspendEvents)
            {
                return;
            }

            var textBox = sender as TextBox;
            this.AlreadyTextChanged = true;
            try
            {
                if (textBox.Text != string.Empty && this.CanProcessKey && !this.IsDisabled)
                {
                    // have a currentText index value for knowing where the current location of item is
                    this.actualText = this.GetActualText(textBox.Text);
                    var filterItemText = string.Empty;
                    var idx = this.FindItem(this.actualText, true, -1, true, out filterItemText);
                    this.ccSelectionStart = textBox.Text.Length;
                    this.ccSelectionLength = Math.Max(0, filterItemText.Length - textBox.Text.Length);
                    textBox.Tag = actualText;
                    if (this.IsAutoComplete)
                    {
                        if (idx > -1)
                        {
                            // when the selection values are calculated, fill the textbox in autocomplete mode
                            str = filterItemText;
                            _ccSelectionstart = this.ccSelectionStart;
                            _ccSelectionLength = this.ccSelectionLength;
                            textBox.Text = filterItemText;
                            textBox.Select(this.ccSelectionStart, this.ccSelectionLength);
                        }
                        if (idx == -1 && this.CurrentStyle.DropDownStyle == GridDropDownStyle.AutoComplete)
                        {
                            textBox.Text = str;
                            textBox.Select(_ccSelectionstart, _ccSelectionLength);
                        }
                    }
                }
                else if (!this.CanProcessKey)
                {
                    this.actualText = string.Empty;
                    this.ClearSelectedIndex();
                }
            }
            finally
            {
                this.AlreadyTextChanged = false;
            }


            if (!this.IsInArrange && this.IsCurrentCell(textBox) && !this.CurrentCell.IsInEndEdit)
            {
                if (!this.SetControlText(textBox.Text))
                {
                    RefreshContent();
                }
            }
        }

        // this represents the real text value that is entered thru keys.
        private string actualText = string.Empty;
        private string GetActualText(string text)
        {
            if (this.actualText == string.Empty)
            {
                // if the actual text is empty, we would need to append some default values based on the incoming text
                this.actualText = text.Substring(0, text.Length - 1);
            }
            //Addinf this code to to check and ActualText length and Text length.
            if (text.Length < this.actualText.Length)
                return string.Empty;
            else
                return this.actualText + text.Substring(this.actualText.Length, 1);
        }

        // when the item is disabled, we need to find the same recurring values
        // say we have Andrew, Anne. When A is pressed the first time, it finds Andrew, and then again it finds Anne
        private int indexWhenItemIsDisabled = -1;
        /// <summary>
        /// This method only for Exclusive DropDown. If we type char means take the item from the List box
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            this.CurrentCell.ScrollInView();
            if (this.IsDisabled && this.HasCurrentCellState && CurrentCellUIElement != null && CurrentStyle.DropDownStyle == GridDropDownStyle.Exclusive)
            {
                var text = e.Text;
                var filterItemText = string.Empty;
                // find item will select the index in the underlying listbox
                this.indexWhenItemIsDisabled = this.FindItem(text, true, this.indexWhenItemIsDisabled, true, out filterItemText);
                if (filterItemText != string.Empty)
                {
                    this.CurrentCellUIElement.TextBoxPart.Text = filterItemText;
                }
                e.Handled = true;
            }
        }

        protected override void OnSetFocus()
        {
            if (this.HasCurrentCellState && this.CurrentCellUIElement != null && !this.IsDisabled && this.CurrentCellUIElement.TextBoxPart != null)
            {
                if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCell.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
                {
                    if (this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
                    {
                        this.CurrentCellUIElement.TextBoxPart.Focus();
                    }
                    if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)// || this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell)
                    {
                        if (this.CurrentCellUIElement.Text!=null)
                            this.CurrentCellUIElement.TextBoxPart.CaretIndex = this.CurrentCellUIElement.Text.Length;
                    }
                    if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
                    {
                        this.CurrentCellUIElement.TextBoxPart.Select(0, this.CurrentCellUIElement.TextBoxPart.Text.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBoxPart should be disabled for editing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disabled; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDisabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBoxPart should AutoComplete words.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is auto complete; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAutoComplete
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBoxPart will allow new text other than the values present in the ItemsSource / ChoiceList.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is allow new entries; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAllowNewEntries
        {
            get;
            set;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ControlText = this.GetControlText(this.CurrentStyle);
        }

        protected override void OnEnteredEditMode()
        {
            this.UpdateDropDownControl();
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnActivated()
        {
            GridControl.PreviewMouseMove += new MouseEventHandler(GridControl_PreviewMouseMove);
            GridControl.PreviewKeyDown += new KeyEventHandler(GridControl_PreviewKeyDown);
            //Following Code are commented out becaue of while key navication it will goes to edit mode.
            //if (this.HasCurrentCellState && !CurrentCell.IsEditing && (!(this.GridControl is GridTreeControlImpl)))// && this.CurrentCell.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
            //{
            //  CurrentCell.BeginEdit(true);

            //}
            this.UpdateDropDownControl();
           
        }

        

        void GridControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        //Root Cause:
            //If StaysOpen = false then, PopUpHost will closes if we click out side of the PopUpHost.
            // If we set StaysOpen as False, then While click on the ToggleButton, first time PopUpHost opens properly.
            //Then if we try to close first PopUpHost Closed then again it will opens. This is because , we have set StaysOpen False
            //While click on the ToggleButton this will leads to close PopUpHost(Click Out side of the PopUpHost ie-Toggle Button) 
            //After that PopUpHost will opens . Because of ToggleButton IsChecked  Property.
        //Fix Detais:
            //Here  StaysOpen set as True if mouse hover on the Toggle Button. Otherwise StaysOpen Set  as false.
            if (HasCurrentCellState&& this.CurrentCellUIElement != null)
            {
                Point point = e.GetPosition(this.CurrentCellUIElement.ToggleButton);
                if (point.X >= 0 && point.X <= this.CurrentCellUIElement.ToggleButton.ActualWidth && point.Y >= 0 && point.Y <= this.CurrentCellUIElement.ToggleButton.ActualHeight)
                {
                   this.CurrentCellUIElement.PopupHost.StaysOpen = true;
                }
                else
                {
                   this.CurrentCellUIElement.PopupHost.StaysOpen = false;

                }
            }
        }
        
        protected override void OnConfirmChangesFailed()
        {
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        protected override void OnEditingComplete()
        {
            if (this.HasCurrentCellState && CurrentCellUIElement != null)
            {
                this.IsDroppedDown = false;
            }
            this.actualText = string.Empty;
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);

        }

        internal int ClickCount
        {
            get;
            set;
        }

        protected void GridControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.HasCurrentCellState)
                return;

            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            UpdateDropDownControl();//This is called we have to refresh the DropDown style after entering the edit mode.
          

            if (!GridControl.CurrentCell.IsEditing && !isControlKey && !isShiftKey)
            {
                GridControl.CurrentCell.BeginEdit();
                UpdateDropDownControl();//This is called we have to refresh the DropDown style after entering the edit mode.
                if(this.CurrentCellUIElement!=null && this.CurrentCellUIElement.TextBoxPart==null && this.CurrentCellUIElement.DropDownStyle== GridDropDownStyle.Editable)
                    this.CurrentCellUIElement.Measure(new Size(this.CurrentCellUIElement.Width, this.CurrentCellUIElement.Height));//After loading the grid if we key press on the ComboBox cell, TextBoxPart comes here null. so we have to force to create TextBoxPart.
                if (ClickCount == 1)
                {
                    if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement.TextBoxPart != null)
                    {
                        this.CurrentCellUIElement.TextBoxPart.IsReadOnly = false;
                        this.CurrentCellUIElement.TextBoxPart.SelectAll();
                    }
                }
                ClickCount = 0;

                //OnTextBoxPartPreviewKeyDown(this.CurrentCellUIElement.TextBoxPart, e);
                if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
                {
                    OnTextBoxPartPreviewKeyDown(this.CurrentCellUIElement.TextBoxPart, e);
                }
            };
           
            if (!isControlKey && !isShiftKey)//Following code are changed like this beacuse while ActivateCurrentCellBehavior= Click on cell editing doesnt tale place.
            //if((this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell && !isControlKey && !isShiftKey))
            {
                if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement.TextBoxPart != null && !this.CurrentCellUIElement.TextBoxPart.IsFocused)
                {
                    this.CurrentCellUIElement.TextBoxPart.Focus();
                }
            }
        }

        protected override void OnDeactivated()
        {
            GridControl.PreviewMouseMove -= new MouseEventHandler(GridControl_PreviewMouseMove);
            GridControl.PreviewKeyDown -= new KeyEventHandler(GridControl_PreviewKeyDown);

            if (this.HasCurrentCellState && CurrentCellUIElement != null)
            {
                CurrentCellUIElement.IsDropDownOpen = false;
                this.ClearSelectedIndex();
                RefreshContent();
            }
            str = string.Empty;
            this.GridControl.InvalidateCell(this.CellRowColumnIndex);
        }

        private void UpdateDropDownControl()
        {
            if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
            {
                this.ClearSelectedIndex();
                // SH: Removed call to CurrentCellUIElement.Focus here since this is causing 
                // exceptions, especially when called from EditingComplete or OnDeactivated.
                if (this.GetControlText(this.CurrentStyle) != string.Empty)
                    // reset the values for the internal property references
                    this.actualText = string.Empty;
                this.ccSelectionLength = 0;
                this.ccSelectionStart = 0;
                this.indexWhenItemIsDisabled = -1;
                var style = this.CurrentStyle;
                switch (style.DropDownStyle)
                {
                    case GridDropDownStyle.Editable:
                        this.IsAutoComplete = false;
                        this.IsAllowNewEntries = true;
                        this.IsDisabled = false;
                       
                            this.CurrentCellUIElement.IsReadOnly = false;
                        break;
                    case GridDropDownStyle.AutoComplete:
                        this.IsAutoComplete = true;
                        this.IsAllowNewEntries = false;
                        this.IsDisabled = false;
                       
                            this.CurrentCellUIElement.IsReadOnly = false;
                        break;
                    case GridDropDownStyle.Exclusive:
                        this.IsDisabled = true;
                        this.IsAutoComplete = false;
                        this.IsAllowNewEntries = false;
                        this.CurrentCellUIElement.IsReadOnly = true;
                        break;
                }
                bool exclusive;
                this.GridListModel.FillWithChoices(style, out exclusive);
            }
            else
            {
                this.GridControl.InvalidateCell(this.CellRowColumnIndex);
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            bool isAltKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
            if ((isControlKey && !this.CurrentCell.IsEditing) || e.Handled)
            {
                return true;
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
            if (isAltKey && e.SystemKey == Key.Up)
            {
                // handle Alt + Up combination
                if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
                {
                    this.CurrentCellUIElement.IsDropDownOpen = !this.CurrentCellUIElement.IsDropDownOpen;
                    e.Handled = true;
                }
            }

            if (isShiftKey && e.Key == Key.Tab)
                return true;
            if ((isControlKey || isShiftKey) && this.CurrentCell.IsEditing)
            {
                return false;
            }
            switch (e.Key)
            {
                case Key.Tab:
                    if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
                    {
                        var textBox = this.CurrentCellUIElement.TextBoxPart;
                        if (textBox != null)
                        {
                            // when tabbing out, we set the start index, if we have a big word, then it gets truncated
                            textBox.SelectionStart = 0;
                        }
                    }
                    return true;
                case Key.Space:
                    if (this.HasCurrentCellState && this.CurrentCellUIElement != null)
                    {
                        if (this.CurrentStyle.DropDownStyle == GridDropDownStyle.Exclusive)
                        {
                            if (this.CurrentCellUIElement.IsDropDownOpen)
                            {
                                this.CurrentCellUIElement.IsDropDownOpen = false;
                            }
                            else
                            {
                                this.CurrentCellUIElement.IsDropDownOpen = true;
                                e.Handled = true;
                                return false;
                            }
                        }
                    }
                    break;
                case Key.Up:
                case Key.Down:
                        return true;
                case Key.Home:
                case Key.End:
                    if (this.CurrentCell.IsEditing)
                        return false;
                    else
                        return true;
                case Key.Left:
                    if (this.CurrentCell.IsEditing)
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
                    if (this.CurrentCell.IsEditing)
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
                case Key.Delete:
                    {
                        // do not call this, since the TextBox in the drop down will handle the delete key press event and then the CurrentStyle would get replaced properly
                        // SetControlText(string.Empty);
                        if (this.IsDisabled)
                        {
                            this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                        }
                        CurrentCell.BeginEdit(true);
                        return false;
                    }

                case Key.Escape:
                    escFlag = true;
                    this.CurrentCell.CancelEdit();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    if (isShiftKey)
                    {
                        break;
                    }
                    else
                    {

                        var control = this.CurrentCellUIElement as GridCellGridListControlDropDown;
                        if (control != null && !this.IsInArrange)
                        {
                            var item = control.GridListControlPart.Model.CurrentItem;
                            item = GetDisplayValue(control.GridListControlPart.Model, item);

                            if (!this.AlreadyTextChanged)
                            {
                                CurrentStyleCopy.Tag = control.GridListControlPart.Model.CurrentIndex;
                                SetControlText(item == null ? string.Empty : item.ToString());
                                CurrentStyleCopy.Tag = null;
                                RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);

                            }
                        }
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
                        CurrentCell.EndEdit();
                        CurrentCell.MoveRight();

                        return true;
                        // break; Unreachable code
                    }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        /// <summary>
        /// Raises GridCellClick event.
        /// </summary>
        /// <param name="rowIndex">The cell row index.</param>
        /// <param name="colIndex">The cell column index.</param>
        /// <param name="e">A <see cref="MouseControllerEventArgs"/> object.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            //if (!this.HasCurrentCellState)
            //    return;
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                 && !CurrentCell.IsEditing
                 && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0)
                 && (GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.None))
            {
                CurrentCell.BeginEdit(true);
                this.UpdateDropDownControl();

            }
            base.RaiseGridCellClick(rowIndex, colIndex, e);
            if ((GridControl.Model.EnableContextMenu && (e.SourceEventArgs as MouseButtonEventArgs).ChangedButton == MouseButton.Right) || !this.HasCurrentCellState)
                return;
           // if (this.CurrentCellUIElement != null)
            {
                if ( ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll ) != 0))
                {
                    Point pt = e.Location;
                    //if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell || this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
                    {
                        if (this.CurrentCell.Renderer != null)
                        {
                            var v = this.CurrentCell.Renderer;
                            bool b = v is GridCellComboBoxCellRenderer || v is GridCellGridListControlDropDownCellRenderer || v is GridDataControlDropDownCellRenderer || v is GridDataDropDownFilterBar;
                            if (b)
                            {
                                if (v.HasCurrentCellState && mouseDownCell.RowIndex > v.CurrentCell.RowIndex &&
                                    ((v is GridCellComboBoxCellRenderer && ((GridCellComboBoxCellRenderer)v).CurrentCellUIElement.IsDropDownOpen)
                                      || (v is GridCellGridListControlDropDownCellRenderer && ((GridCellGridListControlDropDownCellRenderer)v).CurrentCellUIElement.IsDropDownOpen)))
                                    //|| (v is GridDataDropDownFilterBar && ((GridDataDropDownFilterBar)v).CurrentCellUIElement.IsDropDownOpen)
                                {
                                    return;
                                }
                                Rect r = this.GridControl.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(rowIndex, colIndex), false, true);
                                if (r != Rect.Empty)
                                {
                                    r.X = r.Right - comboWidth;
                                    r.Width = comboWidth;
                                    overComboButton = r.Contains(pt);
                                    if (overComboButton)
                                    {
                                        if(!this.CurrentCell.IsEditing)
                                            this.CurrentCell.BeginEdit();//If we click on the dropdownbutton then dropdownshould open on single click even Activate current cell behavior DblClick on cell.So BeginEdit have to call here.
                                        if (!this.CurrentCell.IsEditing) //This condition check will disable the Activation of Dropdown when the Cell is Readonly
                                            return;
                                        if(this.CurrentCellUIElement!=null)
                                            this.CurrentCellUIElement.Focus();
                                        this.UpdateDropDownControl();//After creating UIElement we have to refresh the DropDownStyle
                                        e.Handled = overComboButton;
                                        this.CurrentCell.MoveTo(mouseDownCell, new GridActivateCurrentCellOptions() { ShouldBeginEdit = false });
                                        if (v.HasCurrentCellState && v.CurrentCellUIElement != null)
                                        {
                                            this.GridControl.Dispatcher.BeginInvoke(new Action(() =>
                                            {
                                                if (v is GridCellComboBoxCellRenderer)
                                                {
                                                    ((GridCellComboBoxCellRenderer)v).CurrentCellUIElement.IsDropDownOpen = !((GridCellComboBoxCellRenderer)v).CurrentCellUIElement.IsDropDownOpen;
                                                }
                                                else if (v is GridCellGridListControlDropDownCellRenderer)
                                                {
                                                    ((GridCellGridListControlDropDownCellRenderer)v).CurrentCellUIElement.IsDropDownOpen = !((GridCellGridListControlDropDownCellRenderer)v).CurrentCellUIElement.IsDropDownOpen;
                                                }
                                                else if (v is GridDataControlDropDownCellRenderer)
                                                {
                                                    ((GridDataControlDropDownCellRenderer)v).CurrentCellUIElement.IsDropDownOpen = !((GridDataControlDropDownCellRenderer)v).CurrentCellUIElement.IsDropDownOpen;
                                                }
                                                else if (v is GridDataDropDownFilterBar)
                                                {
                                                    ((GridDataDropDownFilterBar)v).CurrentCellUIElement.IsDropDownOpen = !((GridDataDropDownFilterBar)v).CurrentCellUIElement.IsDropDownOpen;
                                                }
                                            }), null);
                                            e.Handled = true;
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if (this.HasCurrentCellState && this.CurrentCellUIElement != null && this.CurrentCellUIElement.TextBoxPart != null)
                    {
                        if ((this.GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != GridCellActivateAction.None)
                        {
                            if (this.CurrentCellUIElement.TextBoxPart != null)
                                this.CurrentCellUIElement.TextBoxPart.Select(0, this.CurrentCellUIElement.TextBoxPart.Text.Length);
                        }
                        else
                        {
                            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
                            {
                                this.CurrentCellUIElement.TextBoxPart.Focus();
                                this.CurrentCellUIElement.TextBoxPart.CaretIndex = this.CurrentCellUIElement.Text.Length;

                            }
                            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell &&
                                e.ClickCount == 2 && this.CurrentCellUIElement.DropDownStyle != GridDropDownStyle.Exclusive)
                            {
                                //this.CurrentCellUIElement.TextBoxPart.SelectAll();
                                this.CurrentCellUIElement.TextBoxPart.Focus();
                                this.CurrentCellUIElement.TextBoxPart.CaretIndex = this.CurrentCellUIElement.TextBoxPart.Text.Length;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void RaiseSelectedItemChangedEvent(RowColumnIndex cellRowColumnIndex, object item)
        {
            this.GridControl.RaiseGridDropDownSelectionChanged(cellRowColumnIndex, item);
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridControlBase.DropDownSelectionChanged"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The object that originated this event.</param>
    /// <param name="args">The <see cref="GridCellComboValueChangedEventArgs"/> that contains the event data.</param>
    public delegate void GridCellComboValueChangedEventHandler(object sender, GridCellComboValueChangedEventArgs args);

    /// <summary>
    /// Provides data about the <see cref="GridControlBase.DropDownSelectionChanged"/> event.
    /// </summary>
    public class GridCellComboValueChangedEventArgs : SyncfusionRoutedEventArgs
    {
        /// <summary>
        /// Intializes a new <see cref="GridCellComboValueChangedEventArgs"/>.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">The source of the event.</param>
        public GridCellComboValueChangedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        /// <summary>
        /// Intializes a new <see cref="GridCellComboValueChangedEventArgs"/>.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="source">Event source.</param>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        /// <param name="selectedItem">The selected item in the combobox.</param>
        public GridCellComboValueChangedEventArgs(RoutedEvent routedEvent, object source, RowColumnIndex cellRowColumnIndex, object selectedItem)
            : this(routedEvent, source)
        {
            this.CellRowColumnIndex = cellRowColumnIndex;
            this.SelectedItem = selectedItem;
        }

        /// <summary>
        /// Gets the cell row column index.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Returns the selected item of the combobox.
        /// </summary>
        public object SelectedItem
        {
            get;
            internal set;
        }
    }

    public class DropDownPaint
    {
        private Dictionary<Size, VisualBrush> brushes = new Dictionary<Size, VisualBrush>();
        private string VisualStyle = string.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DropDownPaint()
        {
        }

        /// <summary>
        /// Draws the GridCellDropDownControlBase in the cell rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rc">Cell rectangle.</param>
        /// <param name="text">Text to be drawn over the button.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>Cell margins.</returns>
        public void DrawDropDown(DrawingContext dc, Rect rc, string text, GridStyleInfo style)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size, style);
            dc.DrawRectangle(vb, null, rc);
            rc = this.ResetTextAreaSize(rc);
            GridTextBoxPaint.DrawText(dc, rc, text, style);
        }

        private Rect ResetTextAreaSize(Rect rc)
        {
            rc.Width = (rc.Width - SystemParameters.VerticalScrollBarWidth) > 0 ? rc.Width - SystemParameters.VerticalScrollBarWidth :0;
            return rc;
        }

        private VisualBrush GetVisualBrush(Size size, GridStyleInfo style)
        {
            if (style.GridModel is GridDataTableModel)
            {
                var gridDataTableModel = style.GridModel as GridDataTableModel;
                var canRecycle = gridDataTableModel.TableProperties.VisualStyle.ToString().Equals(VisualStyle);
                if (!canRecycle)
                    brushes.Clear();
                VisualStyle = gridDataTableModel.TableProperties.VisualStyle.ToString();
            }

            if (this.brushes.ContainsKey(size))
            {
                return this.brushes[size];
            }

            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                GridUtil.IsAnimated = false;

                VisualBrush visualBrush;
                GridCellDropDownControlBase b = new GridCellDropDownControlBase();
                b.BeginInit();
                b.Text = null;
                b.Width = size.Width;
                b.Height = size.Height;
                b.EndInit();
                b.Measure(size);
                b.Arrange(new Rect(size));
                b = GetDropDownVisualStyle(b, style);
                visualBrush = new VisualBrush();
                visualBrush.Visual = b;
                this.brushes[size] = visualBrush;
                return visualBrush;
            }
            finally
            {
                GridUtil.IsAnimated = wasAnimated;
            }
        }

        public GridCellDropDownControlBase GetDropDownVisualStyle(GridCellDropDownControlBase dropdown, GridStyleInfo style)
        {
            if (style.GridModel is GridDataTableModel)
            {
                var gridTableModel = style.GridModel as GridDataTableModel;
                if (style.IsThemed)
                    gridTableModel.GetVisualStyleDictionary(dropdown);
            }
            return dropdown;
        }
    }
}
