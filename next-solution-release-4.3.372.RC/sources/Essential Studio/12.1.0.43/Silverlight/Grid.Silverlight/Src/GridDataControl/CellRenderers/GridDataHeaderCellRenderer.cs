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
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Controls;
    using System.Windows.Input;

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
            var model = style.GridModel as GridDataTableModel;
            if (model != null && model.TableProperties.ReserveSpaceForIcons)
            {
                size.Width += 3 * GridDataHeaderCellControl.MinWidth;
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
                        size.Width += GridDataHeaderCellControl.MinWidth;
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
                        size.Width += GridDataHeaderCellControl.MinWidth;
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
                        size.Width += GridDataHeaderCellControl.MinWidth;
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
        public virtual IEnumerable<string> GetFilterChoices(int colIdx)
        {
            var tableModel = this.GridModel as GridDataTableModel;
            tableModel.SuspendEvents();
            var visibleColumn = tableModel.TableProperties.VisibleColumns[colIdx];
            tableModel.ResumeEvents();

            for (int i = 0; i < tableModel.View.Records.Count; i++)
            {
                object value = null;
                if (!visibleColumn.IsUnbound)
                {
                    value = tableModel.Table.GetValue(tableModel.View.Records[i].Data, visibleColumn.MappingName);
                    //value = tableModel.Table.GetValue(i, visibleColumn.MappingName);
                }
                else
                {
                    value = tableModel.Table.GetUnboundValue(i, visibleColumn.MappingName);
                }

                if (value != null)
                    yield return value.ToString();
                else
                    yield return tableModel.TableProperties.NullFilterText;
            }

            /// The following code are commented to fix the duplicate key value in excel like filtering.
            //int count = 0;
            //if (tableModel.Table.HasGroups)
            //{
            //    count = this.GetSourceCollectionList(tableModel).Count;
            //}
            //else
            //{
            //    count = tableModel.View.Records.Count;
            //}

            //for (int i = 0; i < count; i++)
            //{
            //    object value = null;
            //    if (!visibleColumn.IsUnbound)
            //    {
            //        if (!tableModel.Table.HasGroups)
            //            value = tableModel.Table.GetValue(tableModel.View.Records[i].Data, visibleColumn.MappingName);
            //        else
            //        {
            //            var list = this.GetSourceCollectionList(tableModel);
            //            value = tableModel.Table.GetValue(list[i], visibleColumn.MappingName);
            //        }
            //        //value = tableModel.Table.GetValue(i, visibleColumn.MappingName);
            //    }
            //    else
            //    {
            //        value = tableModel.Table.GetUnboundValue(i, visibleColumn.MappingName);
            //    }
            //    if (value != null)
            //        yield return value.ToString();
            //    else
            //        yield return tableModel.TableProperties.NullFilterText;
            //}
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
            this.AllowRecycle = false;
            this.SupportsRenderOptimization = false;
            this.IsControlTextShown = true;
            this.IsFocusable = true;
            this.IsEditable = false;
        }

        public GridDataHeaderCellModel HeaderCellModel
        {
            get
            {
                return this.CellModel as GridDataHeaderCellModel;
            }
        }

        protected override void OnElementMeasured(UIElement el, Size size)
        {
            var headerCellControl = el as GridDataHeaderCellControl;
            var style = GridControlBase.GetRenderStyleInfo(el);
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var column = ((GridDataStyleInfo)style.ModelStyle).CellIdentity.Column;
                if (this.HeaderCellModel.CanShowFilterButton)
                {
                    headerCellControl.FilterButtonVisibility = column.AllowFilter ? Visibility.Visible : Visibility.Collapsed;
                }

                //if (this.HeaderCellModel.CanShowColumnOptionsButton)
                //{
                //    headerCellControl.ColumnOptionsButtonVisibility = column.ShowColumnOptions ? Visibility.Visible : Visibility.Collapsed;
                //}
            }
            base.OnElementMeasured(el, size);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, GridDataHeaderCellControl uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);
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
            headerCellControl.FontFamily = style.Font.FontFamily;
            headerCellControl.FontSize = style.Font.FontSize;
            //base.OnInitializeContent(headerCellControl, style);
            this.OnUnwireUIElement(headerCellControl);

            // headerCellControl.Measure(this.size);
            VisualContainer.SetWantsMouseInput(headerCellControl, false);
            headerCellControl.StaysOpenOnEdit = style.HasStaysOpenOnEdit ? style.StaysOpenOnEdit : false;
            if (style.ModelStyle is GridDataStyleInfo)
            {
                var column = ((GridDataStyleInfo)style.ModelStyle).CellIdentity.Column;
                headerCellControl.SetVisibleColumn(column);
            }
            headerCellControl.RenderStyle = style;
            // set the header text
            headerCellControl.Text = style.CellValue != null && style.CellValue.ToString() != string.Empty ? style.CellValue.ToString() : string.Empty;
            if (style.Tag != null)
            {
                headerCellControl.SortVisibility = Visibility.Visible;
                var sortDirection = (ListSortDirection)style.Tag;
                if (sortDirection == ListSortDirection.Ascending)
                {
                    headerCellControl.AscVisibility = Visibility.Visible;
                    headerCellControl.DescVisibility = Visibility.Collapsed;
                }
                else if (sortDirection == ListSortDirection.Descending)
                {
                    headerCellControl.AscVisibility = Visibility.Collapsed;
                    headerCellControl.DescVisibility = Visibility.Visible;
                }
                headerCellControl.SortDirection = sortDirection;
            }
            else
            {
                headerCellControl.SortVisibility = Visibility.Collapsed;
                headerCellControl.AscVisibility = Visibility.Collapsed;
                headerCellControl.DescVisibility = Visibility.Collapsed;
            }

            if (this.TableModel != null)
            {
                var visualStyle = this.TableModel.GridVisualStyle;
                headerCellControl.HeaderInnerBorderBrush = this.TableModel.GetHeaderInnerBorder();
                headerCellControl.HeaderInnerBorderThickness = this.TableModel.GetHeaderInnerBorderThickness();
                headerCellControl.Background = this.TableModel.GetHeaderBackground();
                headerCellControl.ColumnOptionsBackground = this.TableModel.GetColumnOptionsPopupBackground();
                //if(!this.TableModel.HeaderStyle.HasForeground)
                //    headerCellControl.Foreground = this.TableModel.GetHeaderForeground();

                if (headerCellControl.VisibleColumn != null && headerCellControl.VisibleColumn.HeaderStyle != null && headerCellControl.VisibleColumn.HeaderStyle.HasBackground)
                    headerCellControl.Background = headerCellControl.VisibleColumn.HeaderStyle.Background;
                else if (!this.TableModel.HeaderStyle.HasBackground) 
                    headerCellControl.Background = this.TableModel.GetHeaderBackground();
                if (headerCellControl.VisibleColumn != null && headerCellControl.VisibleColumn.HeaderStyle != null && headerCellControl.VisibleColumn.HeaderStyle.HasForeground)
                    headerCellControl.Foreground = headerCellControl.VisibleColumn.HeaderStyle.Foreground;
                else if (!this.TableModel.HeaderStyle.HasForeground) 
                    headerCellControl.Foreground = this.TableModel.GetHeaderForeground();

                headerCellControl.ColumnOptionsForeground = this.TableModel.GetColumnOptionsPopupForeground();
                headerCellControl.HoverBackground = this.TableModel.GetHeaderHoverBackground();
                headerCellControl.HoverForeground = this.TableModel.GetHeaderHoverForeground();
                headerCellControl.ColumnOptionsButtonBackground = this.TableModel.GetColumnOptionsButtonBackground();
                headerCellControl.ColumnOptionsCloseButtonBrush = this.TableModel.GetColumnOptionsCloseButtonBrush();
                headerCellControl.SortBrush = this.TableModel.GetSortWidgetBrush();
                headerCellControl.FilterInnerBrush = this.TableModel.GetFilterButtonInnerBrush();
                headerCellControl.FilterOuterBrush = this.TableModel.GetFilterButtonOuterBrush();
                headerCellControl.FilterHoverInnerBrush = this.TableModel.GetFilterButtonHoverInnerBrush();
                headerCellControl.FilterHoverOuterBrush = this.TableModel.GetFilterButtonHoverOuterBrush();
                //headerCellControl.GroupingIndicatorInnerBrush = visualStyle.GroupingIndicatorInnerBrush;
                //headerCellControl.GroupingIndicatorOuterBrush = visualStyle.GroupingIndicatorOuterBrush;
                //headerCellControl.GroupingIndicatorHoverInnerBrush = visualStyle.GroupingIndicatorHoverInnerBrush;
                headerCellControl.FilterAppliedInnerBrush = this.TableModel.GetFilterButtonAppliedBrush();
                headerCellControl.HeaderOptionsBorderBrush = this.TableModel.GetHeaderOptionsBorderBrush();
                headerCellControl.HeaderOptionsHoverBackground = this.TableModel.GetHeaderOptionsHoverBackground();
                headerCellControl.HeaderOptionsCheckedBackground = this.TableModel.GetHeaderOptionsCheckedBackground();

                ResourceDictionary rdictionary = new ResourceDictionary();
                bool applyOldSkin = false;
                if (this.GridControl.Model is GridDataTableModel)
                    applyOldSkin = (this.GridControl.Model as GridDataTableModel).TableProperties.IsLegacyStyleEnabled;
                else if (this.GridControl.Model is GridDataGroupDropAreaModel)
                    applyOldSkin = (this.GridControl.Model as GridDataGroupDropAreaModel).TableProperties.IsLegacyStyleEnabled;
                rdictionary.Source = new Uri("/Syncfusion.Grid.Silverlight;component/GridDataControl/CellRenderers/Themes/generic.xaml", UriKind.RelativeOrAbsolute);
                if (applyOldSkin && headerCellControl.Style == null)
                {
                    headerCellControl.Style = rdictionary["EnableLegacyStyle"] as Style;
                }
                if (this.TableModel.TableProperties.ShowSortNumber && headerCellControl.VisibleColumn != null)
                {
                    bool multiColumnSorting = this.TableModel.TableProperties.SortColumns.Count > 1;
                    if (multiColumnSorting)
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

            var gridTableModel = CellModel.GridModel as GridDataTableModel;
            if (gridTableModel != null && !gridTableModel.TableProperties.IsLegacyStyleEnabled)
            {
                if (gridTableModel.TableProperties.VisualStyle == VisualStyle.BureauBlue || gridTableModel.TableProperties.VisualStyle == VisualStyle.ShinyRed || gridTableModel.TableProperties.VisualStyle == VisualStyle.GlassyGreen
                    || gridTableModel.TableProperties.VisualStyle == VisualStyle.TwilightBlue || gridTableModel.TableProperties.VisualStyle == VisualStyle.Windows7 || gridTableModel.TableProperties.VisualStyle == VisualStyle.SyncfusionTheme
                    || gridTableModel.TableProperties.VisualStyle == VisualStyle.SunBlack || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office2007Blue || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office2007Black
                    || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office2007Silver || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office14Blue || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office14Black
                    || gridTableModel.TableProperties.VisualStyle == VisualStyle.Office14Silver || gridTableModel.TableProperties.VisualStyle == VisualStyle.Blend || gridTableModel.TableProperties.VisualStyle == VisualStyle.VS2010
                    || gridTableModel.TableProperties.VisualStyle == VisualStyle.ShinyBlue || gridTableModel.TableProperties.VisualStyle ==  VisualStyle.Metro)
                {
                    if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
                    {
                        string styleLoc = "/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/" + gridTableModel.TableProperties.VisualStyle + "Style.xaml";
                        if (RemoveDictionaryIfExist(headerCellControl, styleLoc))
                        {
                            ResourceDictionary dictionary = new ResourceDictionary();
                            dictionary.Source = new Uri(styleLoc, UriKind.RelativeOrAbsolute);
                            headerCellControl.Resources.MergedDictionaries.Add(dictionary);
                        }
                    }
                }
            }

            var grid = this.GridControl as GridDataControlBaseImpl;

            //if (grid == null && this.GridControl is GridDataGroupDropAreaGridImpl)
            {
                //var groupGrid = this.GridControl as GridDataGroupDropAreaGridImpl;
                //if (groupGrid != null)
                {
                  //  GridDataControl gridDataControl = groupGrid.FindParentElementOfType<GridDataControl>();
                    //if (gridDataControl != null && gridDataControl.EnableBlendStyling && gridDataControl.HeaderStyle != null)
                      //  headerCellControl.Style = gridDataControl.HeaderStyle;
                }
            }

            //if (grid != null && grid.EnableBlendStyling && grid.HeaderStyle != null)
            //{
              //  headerCellControl.Style = grid.HeaderStyle;
            //}

            
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
        }

        private static bool RemoveDictionaryIfExist(GridDataHeaderCellControl headerCellControl, string dictionary)
        {
            if (headerCellControl != null)
            {
                for (int i = 0; i < headerCellControl.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = headerCellControl.Resources.MergedDictionaries[i];

                    if (rdic.Source.ToString() == dictionary)
                    {
                        return false;
                    }
                    else if (rdic.Source.ToString().Contains("/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/"))
                    {
                        headerCellControl.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }

                return true;
            }

            return false;
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
            if (this.CanFilter && this.CurrentCellUIElement != null && this.ShouldShowAdvancedFilterPaneOnActivated && this.Column.IsAdvancedFilteringMode)
            {
                this.ShouldShowAdvancedFilterPaneOnActivated = false;
                // since the next column on TAB can also be a normal filter column
                this.CurrentCellUIElement.IsDropDownOpen = true;
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
                    if (colIdx > -1)
                    {
                        var column = this.TableModel.TableProperties.VisibleColumns[colIdx];
                        return column;
                    }
                }

                return null;
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
                // bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
                switch (e.Key)
                {
                    case Key.Escape:
                        if (this.CanFilter)
                        {
                            this.CurrentCellUIElement.CloseFilterDropDown();
                        }
                        //if (this.Column.ShowColumnOptions && this.CurrentCellUIElement.IsColumnOptionsDropDownOpen)
                        //{
                        //    this.CurrentCellUIElement.CloseColumnOptionsDropDown();
                        //}
                        return false;
                    case Key.Right:
                    case Key.Left:
                    case Key.Down:
                    case Key.Up:
                        return false;
                    case Key.Tab:
                        if (this.CanFilter /*&& this.Column.IsAdvancedFilteringMode*/)
                        {
                            this.ShouldShowAdvancedFilterPaneOnActivated = true;
                        }
                        return true;
                }
            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }

        internal override void UnRegisterUIElement(GridDataHeaderCellControl uiElement)
        {
            if (uiElement != null)
                uiElement.Dispose();
        }
    }
}
