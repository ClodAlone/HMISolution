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
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.Collections;
    using Syncfusion.Linq;
    using System.ComponentModel;
    using Syncfusion.Windows.Data;

    /// <summary>
    /// Implements the model part of the drop down list cells.
    /// </summary>
    public class GridCellGridListControlDropDownCellModel : GridCellDropDownCellModel<GridCellGridListControlDropDownCellRenderer>
    {
    }

    /// <summary>
    /// Implements the renderer part of the drop down list cells.
    /// </summary>
    public class GridCellGridListControlDropDownCellRenderer : GridCellDropDownCellRenderer<GridCellGridListControlDropDown>
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellGridListControlDropDownCellRenderer"/>.
        /// </summary>
        public GridCellGridListControlDropDownCellRenderer()
        {
            //this.EnableMouseTracking = true;
            this.AllowRecycle = false;
        }

        //public bool EnableMouseTracking
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Returns the model of drop-down list control.
        /// </summary>
        public GridCellGridListControlDropDownCellModel Model
        {
            get
            {
                return (this.CellModel as GridCellGridListControlDropDownCellModel);
            }
        }

        private Rect cellRect;
        protected override void OnArrange(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            base.OnArrange(aca, style);
            this.cellRect = aca.CellRect;
        }

        /// <summary>
        /// Initializes the content of the drop down list cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="dropDownControl">The drop-down list control.</param>
        /// <param name="style">The cell style.</param>
        public override void OnInitializeContent(GridCellGridListControlDropDown dropDownControl, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(dropDownControl, style);
            /*
             * Wait for the GridListControlPart to be loaded, When ArrangeUIElement is called
             * the template for GridListControl gets loaded, again OnInitializeContent would be called.
             * Issue occurs when setting the ItemsSource property in ArrangeUIElement, If the ItemsSource is set in QueryCellInfo as "new SomeItemsSource()"
             * it always sets the ItemsSource in the GLC, which causes rendering issues. Setting the ItemsSource in OnInitializeContent, resolves the issue.
             */
            if (style.ItemsSource != null && dropDownControl.GridListControlPart != null)
            {
                dropDownControl.GridListControlPart.Model.AutoPopulateDropDownColumns = style.AutoPopulateDropDownColumns;
                dropDownControl.GridListControlPart.Model.DropDownVisibleColumns = style.DropDownVisibleColumns;
                dropDownControl.GridListControlPart.Model.DropDownColumnSizer = style.DropDownColumnSizer;
                dropDownControl.GridListControlPart.Model.DisplayMember = style.HasDisplayMember ? style.DisplayMember : string.Empty;
                dropDownControl.GridListControlPart.Model.ValueMember = style.HasValueMember ? style.ValueMember : string.Empty;
                if (dropDownControl.GridListControlPart.Model.ItemsSource == null)
                    dropDownControl.GridListControlPart.Model.ItemsSource = this.Model.GetDataSource(style);
                //dropDownControl.EnableMouseTracking = this.EnableMouseTracking;
                if (style.HasShowDropDownHeaders)
                {
                    var showHeaders = style.ShowDropDownHeaders;
                    if (showHeaders)
                    {
                        dropDownControl.GridListControlPart.Model.RowHeights.SetHidden(0, 0, false);
                    }
                    else
                    {
                        dropDownControl.GridListControlPart.Model.RowHeights.SetHidden(0, 0, true);
                    }
                }
                dropDownControl.GridListControlPart.Model.Sizer.ResizeColumns();
            }
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridCellGridListControlDropDown uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);
            var dropDownControl = uiElement;
            // call arrange to load the internal grid thru Templates.
            if (dropDownControl.PopupContent != null)
            {
                dropDownControl.PopupContent.Measure(this.cellRect.Size);
                dropDownControl.PopupContent.Arrange(this.cellRect);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (this.CurrentCellUIElement != null)
            {
                var dropDownControl = this.CurrentCellUIElement;
                if (dropDownControl.GridListControlPart != null)
                {
                    var controlListModel = CurrentCellUIElement.GridListControlPart.Model;
                    IEnumerable itemsSource = controlListModel.GetStyleDataSource(this.CurrentStyle);
                    var list = itemsSource as IList;
                    controlListModel.SourceList = list;                    
                }
            }
        }


        public override object ControlValue
        {
            get
            {
                object item = base.ControlValue;
                var control = this.CurrentCellUIElement as GridCellGridListControlDropDown;
                if (item != System.DBNull.Value && control != null)
                {
                    if (control.GridListControlPart.Model.ValueMember != String.Empty)
                    {
                        var type = item.GetType();
                        if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type))
                        {
                            //item = TypeDescriptor.GetProperties(type)[control.GridListControlPart.Model.DisplayMember].GetValue(item).ToString();
                            if (control.GridListControlPart.Model.ItemsSource is ITypedList)
                            {
                                var propcoll = ((ITypedList)(control.GridListControlPart.Model.ItemsSource)).GetItemProperties(null) as PropertyDescriptorCollection;
                                item = propcoll.GetValue(item, control.GridListControlPart.Model.ValueMember);
                            }
                            else
                                item = TypeDescriptor.GetProperties(type)[control.GridListControlPart.Model.ValueMember].GetValue(item);
                        }
                    }
                }
                return item;
            }
            set
            {
                base.ControlValue = value;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (this.CurrentCell != null && this.CurrentCell.ActivateOptions != null)
            {
                var isMouseDownTriggeredByMouse = this.CurrentCell.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement;
                if (!isMouseDownTriggeredByMouse)
                {
                    this.GridControl.Focus();
                }
            }
        }

        protected override void OnSetFocus()
        {
            if (this.CurrentCellUIElement != null && !this.IsDisabled)
            {
                if (this.CurrentCellUIElement.TextBoxPart != null)
                {
                    this.CurrentCellUIElement.TextBoxPart.Focus();
                }
            }
        }

        protected override void SetSelectedIndex(int index)
        {
            if (index != this.CurrentCellUIElement.GridListControlPart.Model.CurrentIndex)
            {
                this.CurrentCellUIElement.GridListControlPart.Model.CurrentIndex = index;
            }
        }

        void OnModelCurrentIndexChanged(object sender, EventArgs e)
        {
            var controlListModel = ((GridListModel)sender);
            // update the CellModel's currentIndex, this is different than the UI model
            if (this.Model != null)
                this.Model.ListModel.CurrentIndex = this.Model.FindValue(this.CurrentStyle, controlListModel.GetValue(controlListModel.CurrentItem));
            if (!this.AlreadyTextChanged && this.CurrentCellUIElement != null)
            {
                this.CanProcessKey = false;
                if (CurrentCellUIElement != null)
                {
                    if (this.CurrentStyle.DisplayMember.ToString() != String.Empty)
                    {
                        controlListModel.DisplayMember = this.CurrentStyle.DisplayMember;                      
                    }
                
                    //Console.WriteLine(this.CurrentCellUIElement.TextBoxPart.Text.ToString());
                    this.CurrentCellUIElement.TextBoxPart.Text = controlListModel.CurrentDisplayText;
                    //Console.WriteLine(this.CurrentCellUIElement.TextBoxPart.Text.ToString());
                    this.CurrentCellUIElement.TextBoxPart.SelectAll();
                }
            }
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            var result = base.ShouldGridTryToHandlePreviewKeyDown(e);
            if (e.Handled || !result)
            {
                return result;
            }

            if (this.CurrentCell.Renderer is GridCellGridListControlDropDownCellRenderer && this.CurrentCellUIElement != null && this.CurrentCellUIElement.IsDropDownOpen)
            {
                var controlListModel = this.CurrentCellUIElement.GridListControlPart.Model;

                switch (e.Key)
                {
                    case Key.Up:
                        if (controlListModel.CurrentIndex != 0 && !this.CurrentStyle.ShowDropDownHeaders)
                        {
                            controlListModel.MoveUp();
                        }
                        e.Handled = true;
                        return false;
                    case Key.Down:
                        controlListModel.MoveDown();
                        e.Handled = true;
                        return false;
                    case Key.PageUp:
                        controlListModel.PageUp();
                        e.Handled = true;
                        return false;
                    case Key.PageDown:
                        controlListModel.PageDown();
                        e.Handled = true;
                        return false;
                    case Key.Home:
                        controlListModel.MoveToTop();
                        e.Handled = true;
                        return false;
                    case Key.End:
                        controlListModel.MoveToBottom();
                        e.Handled = true;
                        return false;
                }
            }

            return true;
        }


        protected override void WireTemplateParts(GridCellGridListControlDropDown uiElement)
        {
            if (uiElement.TextBoxPart != null)
            {
                uiElement.TextBoxPart.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
            }
            if (uiElement != null)
            {
                uiElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(uiElement_PreviewMouseLeftButtonDown);
                uiElement.PreviewMouseRightButtonDown += new MouseButtonEventHandler(uiElement_PreviewMouseRightButtonDown);
            }


            if (uiElement.GridListControlPart != null)
            {
                uiElement.GridListControlPart.Model.CurrentIndexChanged += new EventHandler(OnModelCurrentIndexChanged);
                
            }
            base.WireTemplateParts(uiElement);
        }

        void uiElement_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        

        void uiElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // var control = sender as GridCellGridListControlDropDown; Unused local variable
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
            {
                this.IsDroppedDown = true;
            }
            //if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell)
            //{
            //    if (e.ClickCount == 2)
            //    {
            //        this.CurrentCellUIElement.TextBoxPart.IsReadOnly = false;
            //    }
            //    else
            //    {
            //        this.CurrentCellUIElement.TextBoxPart.IsReadOnly = true;
            //    }

            //}
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
            {
                this.CurrentCellUIElement.TextBoxPart.CaretIndex = this.CurrentCellUIElement.TextBoxPart.Text.Length;
            }
            if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
            {
                this.CurrentCellUIElement.TextBoxPart.SelectAll();
            }
        }

        protected override void UnwireTemplateParts(GridCellGridListControlDropDown uiElement)
        {
            if (uiElement.TextBoxPart != null)
            {
                uiElement.TextBoxPart.RemoveHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted));
            }
            if (uiElement != null)
            {
                uiElement.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(uiElement_PreviewMouseLeftButtonDown);
                uiElement.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(uiElement_PreviewMouseRightButtonDown);
            }

            if (uiElement.GridListControlPart != null)
            {
                uiElement.GridListControlPart.Model.CurrentIndexChanged -= new EventHandler(OnModelCurrentIndexChanged);
            }
            base.UnwireTemplateParts(uiElement);
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
                    var filterItemText = string.Empty;
                    var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
                    if (idx > -1)
                    {
                        this.CurrentCellUIElement.GridListControlPart.CurrentCell.MoveTo(idx + 1, 0);
                        this.CurrentCellUIElement.IsDropDownOpen = true;
                    }
                    else if (this.CurrentStyle.DropDownStyle == GridDropDownStyle.AutoComplete || this.CurrentStyle.DropDownStyle == GridDropDownStyle.Exclusive)
                    {
                        this.CurrentCellUIElement.TextBoxPart.Text = string.Empty;
                        this.CurrentCellUIElement.GridListControlPart.Model.SelectedRanges.Clear();
                    }
                    else
                    {
                        this.CurrentCellUIElement.GridListControlPart.Model.SelectedRanges.Clear();
                    }
                }
            }
        }

        protected override void OnClipboardPaste(GridCutPasteEventArgs args)
        {
            var actualText = args.ClipboardText;
            actualText = actualText.Replace(this.GridControl.Model.TextDataExchange.TabDelimiter, string.Empty);
            var filterItemText = string.Empty;
            // When the cell is not in edit mode, CurrentCellUIElement is null
            if (this.CurrentCellUIElement != null)
            {
                var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
                if (idx > -1)
                {
                    this.CurrentCellUIElement.GridListControlPart.CurrentCell.MoveTo(idx + 1, 0);
                    this.CurrentCellUIElement.IsDropDownOpen = true;
                    args.Handled = true;
                }
            }
        }

        /// <summary>
        /// This event calls after clipboardPasted
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClipboardPasted(GridCutPasteEventArgs args)
        {
            var actualText = args.ClipboardText;
            actualText = actualText.Replace(this.GridControl.Model.TextDataExchange.TabDelimiter, string.Empty);
            var filterItemText = string.Empty;

            base.OnClipboardPasted(args);
            if (!this.CurrentCell.IsEditing)
                this.CurrentCell.EndEdit();
            // When the cell is not in edit mode, CurrentCellUIElement is null
            if (CurrentCellUIElement != null)
            {
                var idx = this.FindItem(actualText, true, -1, true, out filterItemText);
                if (idx > -1)
                {
                    this.CurrentCellUIElement.GridListControlPart.CurrentCell.MoveTo(idx + 1, 0);
                    this.CurrentCellUIElement.IsDropDownOpen = true;
                    args.Handled = true;
                }
            }
        }
        #endregion
    }
}
