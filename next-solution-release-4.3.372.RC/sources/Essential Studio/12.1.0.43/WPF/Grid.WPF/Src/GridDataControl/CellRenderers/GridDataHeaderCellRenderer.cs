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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Shared;

    public class GridDataHeaderCellModel : GridCellModel<GridDataHeaderCellRenderer>
    {
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
            size = AddBorderMargins(size, style.Padding.ToThickness());
            var model = style.GridModel as GridDataTableModel;
            if (model != null && model.TableProperties.ReserveSpaceForIcons)
            {
                size.Width += 3 * GridDataHeaderCellControl.MinimumWidth;
            }
            else
            {
                size = this.AddColumnOptionsVisibiltiyToSize(size, style);
                size = this.AddFilterVisibiltiyToSize(size, style);
                size = this.AddSortVisibilityToSize(size, style);
            }
            size.Width = Math.Ceiling(size.Width);
            size.Height = Math.Ceiling(size.Height);

            return size;
        }

        private Size AddFilterVisibiltiyToSize(Size size, GridStyleInfo style)
        {
            if (this.CanShowFilterButton)
            {
                if (style is GridDataStyleInfo)
                {
                    var column = ((GridDataStyleInfo)style).CellIdentity.Column;
                    if (column.AllowFilter)
                    {
                        size.Width += GridDataHeaderCellControl.MinimumWidth;
                        return size;
                    }
                }
            }

            return size;
        }

        private Size AddColumnOptionsVisibiltiyToSize(Size size, GridStyleInfo style)
        {
            if (this.CanShowColumnOptionsButton)
            {
                if (style is GridDataStyleInfo)
                {
                    var column = ((GridDataStyleInfo)style).CellIdentity.Column;
                    if (column.ShowColumnOptions)
                    {
                        size.Width += GridDataHeaderCellControl.MinimumWidth;
                        return size;
                    }
                }
            }

            return size;
        }

        private Size AddSortVisibilityToSize(Size size, GridStyleInfo style)
        {
            if (style.Tag != null)
            {
                if (style is GridDataStyleInfo)
                {
                    var column = ((GridDataStyleInfo)style).CellIdentity.Column;
                    var gridDataTableModel = this.GridModel as GridDataTableModel;
                    var hasSortColumn = gridDataTableModel.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == column.MappingName) != null;
                    if (hasSortColumn)
                    {
                        size.Width += GridDataHeaderCellControl.MinimumWidth;
                        return size;
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Gets the filter choices by iterating the cell values as in Excel document. This can be further modified as per custom implementation.
        /// </summary>
        /// <param name="rowColIndex">Index of the row col.</param>
        /// <returns></returns>
        public virtual IEnumerable<object> GetFilterChoices(int colIdx)
        {
            var tableModel = this.GridModel as GridDataTableModel;
            tableModel.SuspendEvents();
            var visibleColumn = tableModel.TableProperties.VisibleColumns[colIdx];
            tableModel.ResumeEvents();
            for (int i = 0; i < tableModel.View.Records.Count; i++)
            {
                object value = null;
                if (!visibleColumn.IsUnbound)
                    value = tableModel.Table.GetValue(tableModel.View.Records[i].Data, visibleColumn.MappingName);
                else
                    value = tableModel.Table.GetUnboundValue(i, visibleColumn.MappingName);
                yield return value;
            }
        }

        public bool CanShowFilterButton
        {
            get;
            set;
        }

        public bool CanShowColumnOptionsButton
        {
            get;
            set;
        }
    }

    public class GridDataHeaderCellRenderer : GridVirtualizingCellRenderer<GridDataHeaderCellControl>
    {
        
        public GridDataHeaderCellRenderer()
        {
            this.AllowRecycle = true;
            this.SupportsRenderOptimization = false;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.IsEditable = false;
        }

        #region CLR Properties
        public GridDataHeaderCellModel HeaderCellModel
        {
            get
            {
                return this.CellModel as GridDataHeaderCellModel;
            }
        }

        private bool CanFilter
        {
            get
            {
                return this.Column != null && this.Column.AllowFilter && this.HeaderCellModel.CanShowFilterButton;
            }
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var gridDataTableModel = this.GridControl.Model as GridDataTableModel;
                if (gridDataTableModel == null)
                {
                    var groupModel = this.GridControl.Model as IGridDataGroupDropAreaModel;
                    if (groupModel != null)
                    {
                        gridDataTableModel = groupModel.TableModel;
                    }
                }
                return gridDataTableModel;
            }
        }

        public GridDataVisibleColumn Column
        {
            get
            {
                if (this.TableModel != null)
                {
                    var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(this.CellRowColumnIndex.ColumnIndex);
                    if (colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count)
                    {
                        var column = this.TableModel.TableProperties.VisibleColumns[colIdx];
                        return column;
                    }
                }

                return null;
            }
        }
        #endregion

        //This function won't call because Measure will be made in UIElement.
        protected override void OnElementMeasured(UIElement el, Size size)
        {
            var headerCellControl = el as GridDataHeaderCellControl;
            var style = GridControlBase.GetRenderStyleInfo(el);
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var column = ((GridDataStyleInfo)style.ModelStyle).CellIdentity.Column;
                if (this.HeaderCellModel.CanShowFilterButton)
                {
                    if (column.AllowFilter)// SD17144 condition changed due to wrong filterpop open
                    {
                        headerCellControl.FilterButtonVisibility = column.AllowFilter ? Visibility.Visible : Visibility.Hidden;
                        headerCellControl.RefreshFilteringMode(false, style.ModelStyle);
                    }
                }

                if (this.HeaderCellModel.CanShowColumnOptionsButton)
                {
                    headerCellControl.ColumnOptionsButtonVisibility = column.ShowColumnOptions ? Visibility.Visible : Visibility.Hidden;
                }
            }
            base.OnElementMeasured(el, size);
        }

       

        protected override void OnArrange(ArrangeCellArgs aca, GridRenderStyleInfo style)
        {
            var left = this.GridControl.Model.TableStyle.Borders.Left != null ? this.GridControl.Model.TableStyle.Borders.Left.Thickness : 0;
            var right = this.GridControl.Model.TableStyle.Borders.Right != null ? this.GridControl.Model.TableStyle.Borders.Right.Thickness : 0;
            var top = this.GridControl.Model.TableStyle.Borders.Top != null ? this.GridControl.Model.TableStyle.Borders.Top.Thickness : 0;
            var bottom = this.GridControl.Model.TableStyle.Borders.Bottom != null ? this.GridControl.Model.TableStyle.Borders.Bottom.Thickness : 0;
            //X, Y position is adjusted due to the SnapsToDevicePixels clips the blured borders.
            aca.CellRect = new Rect(aca.CellRect.X - left, aca.CellRect.Y - top, aca.CellRect.Width + (left + right), aca.CellRect.Height + (top + bottom));
            base.OnArrange(aca, style);
        }

        protected override void OnRenderForPrinting(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
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
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            /// If we resize the column means Text will displace from its origional.
            /// Because While increasing the column size margin.left also increase.
            /// So I have set the Constant value for margin.Left
            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            if (textRectangle.IsEmpty)
                return;

            string text;
            if (IsCurrentCell(style) && HasControlText && this.CurrentCell.IsEditing)
                text = ControlText;
            else
                text = GetControlText(style);

            double dValue = 0;

            if (style.CellValue != null && double.TryParse(style.CellValue.ToString(), out dValue))
            {
                if (dValue < 0)
                {
                    style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                }
            }

            if (text.Length != 0 && text.Length > style.MaxLength && style.HasMaxLength)
                text = text.Remove(style.MaxLength);
            // Draw the formatted text string to the DrawingContext of the control.
          
            var visiblecolumn = ((GridDataStyleInfo)style.ModelStyle).CellIdentity.Column;
            bool CanSetBackground = visiblecolumn != null && visiblecolumn.HeaderStyle != null && visiblecolumn.HeaderStyle.HasBackground;
          
            if (CanSetBackground && (!style.HasBackground))
                style.Background = visiblecolumn.HeaderStyle.Background;
            else if (style.HasBackground && !CanSetBackground)
                style.Background = style.Background;
            else if (style.HasBackground && CanSetBackground)
                style.Background = style.Background;
            else if (!this.TableModel.HeaderStyle.HasHeaderBackGround)
                style.Background = this.TableModel.GetHeaderBackground();

            bool CanSetForeground = visiblecolumn != null && visiblecolumn.HeaderStyle != null && visiblecolumn.HeaderStyle.HasForeground;

            if (CanSetForeground && (!style.HasForeground))
                style.Foreground = visiblecolumn.HeaderStyle.Foreground;
            else if (style.HasForeground && !CanSetForeground)
                style.Foreground = style.Foreground;
            else if (style.HasForeground && CanSetForeground)
                style.Foreground = style.Foreground;
            else if (!this.TableModel.HeaderStyle.HasHeaderForeGround)
                style.Foreground = this.TableModel.GetHeaderForeground();     

            style.TextMargins = TableModel.GridVisualStyle.HeaderTextMargins;
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        public override void CreateRendererElement(GridDataHeaderCellControl headerCellControl, GridRenderStyleInfo style)
        {
            headerCellControl.IsInSuspend = true;
            base.CreateRendererElement(headerCellControl, style);
            this.InitUIElementProperties(headerCellControl, style);

            var gridTableModel = CellModel.GridModel as GridDataTableModel;
            if (gridTableModel != null && !gridTableModel.TableProperties.IsLegacyStyleEnabled)
            {
                headerCellControl.Resources = gridTableModel.GetVisualStyleDictionary(headerCellControl);
            }
        }

        private void InitUIElementProperties(GridDataHeaderCellControl headerCellControl, GridRenderStyleInfo style)
        {
            this.OnUnwireUIElement(headerCellControl);
            headerCellControl.WireExcelLikeFilteringEvents();
            ////headerCellControl.Measure(this.size);
            VisualContainer.SetWantsMouseInput(headerCellControl, false);
            headerCellControl.StaysOpenOnEdit = style.HasStaysOpenOnEdit ? style.StaysOpenOnEdit : false;
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var column = ((GridDataStyleInfo)style.ModelStyle).CellIdentity.Column;
                headerCellControl.SetVisibleColumn(column);
                if (this.HeaderCellModel.CanShowFilterButton)
                {
                    headerCellControl.FilterButtonVisibility = column.AllowFilter ? Visibility.Visible : Visibility.Hidden;
                    if (column.AllowFilter)// SD17144 condition changed due to wrong filterpop open
                    {                        
                        headerCellControl.RefreshFilteringMode(false, style.ModelStyle);
                    }  
                }

                if (this.HeaderCellModel.CanShowColumnOptionsButton)
                {
                    headerCellControl.ColumnOptionsButtonVisibility = column.ShowColumnOptions ? Visibility.Visible : Visibility.Hidden;
                }
            }
            headerCellControl.RenderStyle = style;
            // set the header text
            headerCellControl.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            if (style.CellItemTemplate != null)
                headerCellControl.ContentDataTemplate = style.CellItemTemplate;

            bool showSortIcon = true;

            if (headerCellControl.VisibleColumn != null)
                showSortIcon = headerCellControl.VisibleColumn.AllowSort;

            if (this.TableModel is GridDataChildTableModel && 
                !(this.TableModel.TableProperties.SortColumns.Any(s => s.ColumnName == headerCellControl.VisibleColumn.MappingName)))
                    showSortIcon = false;

            if (style.Tag != null && showSortIcon && this.TableModel.TableProperties.AllowSort)            {
                headerCellControl.SortVisibility = Visibility.Visible;
                headerCellControl.SortDirection = (ListSortDirection)style.Tag;
                if (headerCellControl.PART_FilterPopupHost != null)
                    headerCellControl.PART_FilterPopupHost.SortOrder = headerCellControl.SortDirection.ToString();
               
            }
            else
            {
                headerCellControl.SortVisibility = Visibility.Hidden;
            }

            if (this.TableModel != null)
            {
                var visualStyle = this.TableModel.GridVisualStyle;
                if (visualStyle != null)
                {
                    headerCellControl.HeaderInnerBorderBrush = this.TableModel.GetHeaderInnerBorder();
                    headerCellControl.HeaderInnerBorderThickness = this.TableModel.GetHeaderInnerBorderThickness();
                    //This background overrides the Background colour set in GridDataHeaderCellControl.cs
                    bool CanSetBackground = headerCellControl.VisibleColumn != null && headerCellControl.VisibleColumn.HeaderStyle != null && headerCellControl.VisibleColumn.HeaderStyle.HasBackground;

                    if (CanSetBackground && (!style.HasBackground))
                        headerCellControl.Background = headerCellControl.VisibleColumn.HeaderStyle.Background;
                    else if (style.HasBackground && !CanSetBackground)
                        headerCellControl.Background = style.Background;
                    else if (style.HasBackground && CanSetBackground)
                        headerCellControl.Background = style.Background;
                    else if (!this.TableModel.HeaderStyle.HasHeaderBackGround)
                        headerCellControl.Background = this.TableModel.GetHeaderBackground();

                    bool CanSetForeground = headerCellControl.VisibleColumn != null && headerCellControl.VisibleColumn.HeaderStyle != null && headerCellControl.VisibleColumn.HeaderStyle.HasForeground;

                    if (CanSetForeground && (!style.HasForeground))
                        headerCellControl.Foreground = headerCellControl.VisibleColumn.HeaderStyle.Foreground;
                    else if (style.HasForeground && !CanSetForeground)
                        headerCellControl.Foreground = style.Foreground;
                    else if (style.HasForeground && CanSetForeground)
                        headerCellControl.Foreground = style.Foreground;
                    else if (!this.TableModel.HeaderStyle.HasHeaderForeGround)
                        headerCellControl.Foreground = this.TableModel.GetHeaderForeground();                   
                   

                    headerCellControl.ColumnOptionsBackground = this.TableModel.GetColumnOptionsPopupBackground();

                    headerCellControl.ColumnOptionsForeground = visualStyle.ColumnOptionsPopupForeground;
                    headerCellControl.HoverBackground = this.TableModel.GetHeaderHoverBackground();
                    headerCellControl.HoverForeground = this.TableModel.GetHeaderHoverForeground();
                    headerCellControl.ColumnOptionsButtonBackground = this.TableModel.GetColumnOptionsButtonBackground();
                    headerCellControl.ColumnOptionsButtonBorderBrush = this.TableModel.GetColumnOptionsButtonBorderBrush();
                    headerCellControl.ColumnOptionsCloseButtonBrush = this.TableModel.GetColumnOptionsCloseButtonBrush();
                    headerCellControl.SortBrush = this.TableModel.GetSortWidgetBrush();
                    headerCellControl.FilterInnerBrush = this.TableModel.GetFilterButtonInnerBrush();
                    headerCellControl.FilterOuterBrush = this.TableModel.GetFilterButtonOuterBrush();
                    headerCellControl.FilterHoverInnerBrush = this.TableModel.GetFilterButtonHoverInnerBrush();
                    headerCellControl.FilterHoverOuterBrush = this.TableModel.GetFilterButtonHoverOuterBrush();
                    headerCellControl.FilterAppliedInnerBrush = this.TableModel.GetFilterButtonAppliedBrush();
                    headerCellControl.HeaderOptionsHoverBackground = this.TableModel.GetHeaderOptionsHoverBackground();
                    headerCellControl.HeaderOptionsCheckedBackground = visualStyle.HeaderOptionsCheckedBackground;
                    headerCellControl.HeaderOptionsBorderBrush = this.TableModel.GetHeaderOptionsBorderBrush();

                    ResourceDictionary rdictionary = new ResourceDictionary();
                    bool applyOldSkin = false;
                    if (this.GridControl.Model is GridDataTableModel)
                        applyOldSkin = (this.GridControl.Model as GridDataTableModel).TableProperties.IsLegacyStyleEnabled;
                    else if (this.GridControl.Model is GridDataGroupDropAreaModel)
                        applyOldSkin = (this.GridControl.Model as GridDataGroupDropAreaModel).TableProperties.IsLegacyStyleEnabled;
                    headerCellControl.LegacyEnabled = applyOldSkin;


                    if (applyOldSkin && headerCellControl.Style == null)
                    {
                        //Earlier Resource Dictionary source was set above this if loop, that leads to performance degradation. Also the source should be set when the applyOldSkin is True.
                        rdictionary.Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/CellRenderers/Themes/generic.xaml", UriKind.RelativeOrAbsolute);
                        headerCellControl.Style = rdictionary["EnableLegacyStyle"] as Style;
                    }
                }
                if (this.TableModel.TableProperties.ShowSortNumber)
                {
                    bool multiColumnSorting = this.TableModel.TableProperties.SortColumns.Count > 1;
                    if (multiColumnSorting && headerCellControl.VisibleColumn != null)
                    {
                        int loc = this.FindPropertyNameInStates(headerCellControl.VisibleColumn.MappingName);
                        if (loc > -1)
                        {
                            loc = loc + 1;
                            headerCellControl.SortString = loc.ToString();
                        }
                    }
                    else
                    {
                        headerCellControl.SortString = string.Empty;
                    }
                }
            }
            this.OnWireUIElement(headerCellControl);
            headerCellControl.IsInSuspend = false;

            var grid = this.GridControl as GridDataControlBaseImpl;
            GridDataControl gridDataControl = this.GridControl.FindParentElementOfType<GridDataControl>();
            //if (grid == null && this.GridControl is GridDataGroupDropAreaGridImpl)
            {
                //var groupGrid = this.GridControl as GridDataGroupDropAreaGridImpl;
                //if (this.GridControl is GridDataGroupDropAreaGridImpl)
                //{
                    if (gridDataControl != null && gridDataControl.EnableBlendStyling && gridDataControl.Model != null && gridDataControl.Model.TableProperties != null && gridDataControl.Model.TableProperties.HeaderStyle != null)
                        headerCellControl.Style = gridDataControl.Model.TableProperties.HeaderStyle;
                //}
            }

            //if (grid != null && grid.EnableBlendStyling && grid.HeaderStyle != null)
            //{
            //    headerCellControl.Style = grid.HeaderStyle;
            //}

            if (grid != null && grid.EnableBlendStyling && grid.ColumnOptionPaneStyle != null)
            {
                headerCellControl.ColumnOptionPaneStyle = grid.ColumnOptionPaneStyle;
            }
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                headerCellControl.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = headerCellControl.ActualWidth;
                double offsetY = 0;
                headerCellControl.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                headerCellControl.LayoutTransform = MatrixTransform.Identity;
            }
        }

        internal int FindPropertyNameInStates(string name)
        {
            int loc = 0;
            bool found = false;
            foreach (GridDataSortColumn state in this.TableModel.TableProperties.SortColumns)
            {
                if (state.ColumnName == name)
                {
                    found = true;
                    break;
                }
                loc++;
            }
            if (!found)
            {
                loc = -1;
            }
            return loc;
        }

        public override void OnInitializeContent(GridDataHeaderCellControl headerCellControl, GridRenderStyleInfo style)
        {
            headerCellControl.IsInSuspend = true;
            base.OnInitializeContent(headerCellControl, style);
            this.InitUIElementProperties(headerCellControl, style);
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                headerCellControl.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = headerCellControl.Width;
                double offsetY = 0;
                headerCellControl.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }
            else
            {
                headerCellControl.LayoutTransform = MatrixTransform.Identity;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (this.CurrentCellUIElement != null)
            {
                if (this.CanFilter && !this.TableModel.IsInFilter)
                {
                    // clear the current UIElement and close the drop down, this will refresh the items in the other opened drop down properly.
                    //if (!this.Column.IsAdvancedFilteringMode)
                    //{
                    //    this.UnwireItems(this.CurrentCellUIElement.CheckedListBoxPart);
                    //    this.CurrentCellUIElement.CheckedListBoxPart.Items.Clear();
                    //}
                    this.CurrentCellUIElement.CloseFilterDropDown();
                }
                if (this.HeaderCellModel.CanShowColumnOptionsButton)
                {
                    this.CurrentCellUIElement.CloseColumnOptionsDropDown();
                }
            }
        }

        protected override void OnActivated()
        {
            ///Comment the below condition to improve the ContextMenu opening performance and sorting performance. 
            ///To overcome this add the same in PopupHost_Opened event of HeaderCellsControl
            //if (this.CanFilter && this.CurrentCellUIElement != null && !this.Column.IsAdvancedFilteringMode)
            //{
            //    this.CurrentCellUIElement.UpdateFilters();
            //}

            if (this.ShouldShowAdvancedFilterPaneOnActivated && this.CanFilter && this.CurrentCellUIElement != null && this.Column.IsAdvancedFilteringMode)
            {
                this.ShouldShowAdvancedFilterPaneOnActivated = false;
                // since the next column on TAB can also be a normal filter column
                this.CurrentCellUIElement.IsDropDownOpen = true;
            }
        }

        public bool ShouldShowAdvancedFilterPaneOnActivated
        {
            get;
            set;
        }

        protected override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (this.CurrentCellUIElement != null)
            {
                // even if shift is pressed, TAB code would automatically open the dropdown
                bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
                if (isShiftKey)
                {
                    switch (e.Key)
                    {
                        case Key.Tab:
                            if (this.CanFilter && this.Column.IsAdvancedFilteringMode)
                            {
                                this.ShouldShowAdvancedFilterPaneOnActivated = true;
                            }
                            return false;
                    }
                }

                if (e.Key!= Key.Escape && this.CurrentCellUIElement.IsDropDownOpen)
                {
                    return false;
                }

                switch (e.Key)
                {
                    case Key.Escape:
                    case Key.Enter:
                        if (this.CanFilter)
                        {
                            this.CurrentCellUIElement.CloseFilterDropDown();
                        }
                        if (this.Column.ShowColumnOptions && this.CurrentCellUIElement.IsColumnOptionsDropDownOpen)
                        {
                            this.CurrentCellUIElement.CloseColumnOptionsDropDown();
                        }
                        return false;
                    case Key.Right:
                    case Key.Left:
                    case Key.Down:
                    case Key.Up:
                        if (this.Column.AllowFilter && this.CurrentCellUIElement.IsDropDownOpen)
                        {
                            return false;
                        }
                        else if (this.Column.ShowColumnOptions && this.CurrentCellUIElement.IsColumnOptionsDropDownOpen)
                        {
                            return false;
                        }
                        return true;
                    case Key.Home:
                    case Key.End:
                        if ( this.CurrentCellUIElement.IsDropDownOpen || (this.Column.ShowColumnOptions && this.CurrentCellUIElement.IsColumnOptionsDropDownOpen))
                            return false;
                        break;
                    case Key.Tab:
                        if (this.CanFilter && this.Column.IsAdvancedFilteringMode)
                        {
                            this.ShouldShowAdvancedFilterPaneOnActivated = true;
                        }

                        if (!this.CurrentCellUIElement.IsTabStop)
                            return false;

                        return true;
                }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        internal override void UnRegisterUIElement(GridDataHeaderCellControl uiElement)
        {
            if (uiElement != null)
            {
                uiElement.Dispose();
            }
        }
    }
}
