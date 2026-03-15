#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Syncfusion.Linq;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Grid.GridCellRenderer.DropdownCellRenderers
{
    public class GridDataControlDropDownCellModel : GridCellDropDownCellModel<GridDataControlDropDownCellRenderer>
    {
    }

    public class GridDataControlDropDownCellRenderer : GridCellDropDownCellRenderer<GridDataControlDropDown>
    {
        public GridDataControlDropDownCellRenderer()
        {
            this.DisplayMember = string.Empty;
            //this.IsEditable = false;
        }

        public GridDataControlDropDownCellModel GridDataControlDropDownCellModel
        {
            get
            {
                return this.CellModel as GridDataControlDropDownCellModel;
            }
        }

        public override void OnInitializeContent(GridDataControlDropDown dropDownControl, GridRenderStyleInfo style)
        {

            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            //margins.Left = Math.Max(0, margins.Left - 2);
            //margins.Right = Math.Max(0, margins.Right - 2);


            GridFontInfo font = style.ReadOnlyFont;
            dropDownControl.SetValue(TextBox.FontFamilyProperty, font.FontFamily);
            dropDownControl.SetValue(TextBox.FontSizeProperty, font.FontSize);
            dropDownControl.SetValue(TextBox.FontStretchProperty, font.FontStretch);
            dropDownControl.SetValue(TextBox.FontWeightProperty, font.FontWeight);
            dropDownControl.SetValue(TextBox.FontStyleProperty, font.FontStyle);
            dropDownControl.SetValue(TextBox.TextDecorationsProperty, font.TextDecorations);
            dropDownControl.SetValue(TextBox.TextAlignmentProperty, HorizontalAlignmentToTextAlignment(style.HorizontalAlignment));
            dropDownControl.SetValue(TextBox.HorizontalContentAlignmentProperty, style.HorizontalAlignment);
            dropDownControl.SetValue(TextBox.HorizontalAlignmentProperty, style.HorizontalAlignment);
            dropDownControl.SetValue(TextBox.VerticalAlignmentProperty, style.VerticalAlignment);
            dropDownControl.SetValue(TextBox.VerticalContentAlignmentProperty, style.VerticalAlignment);
            if (font.Orientation != 0)
                dropDownControl.RenderTransform = new RotateTransform(font.Orientation);


#if SyncfusionFramework4_0
            dropDownControl.SetValue(TextBox.SelectionBrushProperty, style.SelectionBrush);
            dropDownControl.SetValue(TextBox.SelectionOpacityProperty, style.SelectionOpacity);
            dropDownControl.SetValue(TextBox.CaretBrushProperty, style.CaretBrush);
#endif

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

            dropDownControl.IsMouseTrackingEnabled = style.IsMouseTrackingEnabled;
        }

        protected virtual void SetTablePropertiesFromOther(GridDataTableProperties tableProperties, GridDataTableProperties other)
        {
            tableProperties.AllowDragColumns = other.AllowDragColumns;
            tableProperties.AllowResizeColumns = other.AllowResizeColumns;
            tableProperties.AllowResizeRows = other.AllowResizeRows;
            tableProperties.AllowSort = other.AllowSort;
            tableProperties.AlternatingRowCount = other.AlternatingRowCount;            
            tableProperties.AutoFocusCurrentItem = other.AutoFocusCurrentItem;
            tableProperties.AutoPopulateColumns = other.AutoPopulateColumns;
            tableProperties.DefaultColumnWidth = other.DefaultColumnWidth;
            tableProperties.DefaultHeaderRowHeight = other.DefaultHeaderRowHeight;
            tableProperties.DragIndicatorInnerBrush = other.DragIndicatorInnerBrush;
            tableProperties.FooterColumns = other.FooterColumns;
            tableProperties.FooterRows = other.FooterRows;
            tableProperties.FrozenColumns = other.FrozenColumns;
            tableProperties.FrozenRows = other.FrozenRows;
            tableProperties.HeaderColumns = other.HeaderColumns;
            tableProperties.HeaderRows = other.HeaderRows;
            tableProperties.IsInternalChange = true;
            if (!tableProperties.IsRowBackgroundChangedExternally)
                tableProperties.RowBackground = other.RowBackground;
            if (!tableProperties.IsAlternatingRowBackgroundChangedExternally)
                tableProperties.AlternatingRowBackground = other.AlternatingRowBackground;
            tableProperties.IsInternalChange = false;
            tableProperties.AlternatingRowForeground = other.AlternatingRowForeground;
            tableProperties.RowForeground = other.RowForeground;
            tableProperties.ShowRowHeader = other.ShowRowHeader;
            tableProperties.ShowRowHeaderArrow = other.ShowRowHeaderArrow;
            tableProperties.ShowTableSummaries = other.ShowTableSummaries;
            tableProperties.AutoGenerateColumnsInfo = other.AutoGenerateColumnsInfo;

            if(!other.AutoPopulateColumns)
                tableProperties.VisibleColumns = other.VisibleColumns;

            tableProperties.VisualStyle = other.VisualStyle;
            tableProperties.SummaryRows = other.SummaryRows;
            tableProperties.TableSummaryRows = other.TableSummaryRows;
            tableProperties.TableSummaryPosition = other.TableSummaryPosition;
            tableProperties.EnableLegacyStyle = false;

#if !SILVERLIGHT
            tableProperties.DragIndicatorOuterBrush = other.DragIndicatorOuterBrush;
            tableProperties.SortClickAction = other.SortClickAction;
#endif
        }

        protected override void ArrangeUIElement(Syncfusion.Windows.Controls.Cells.ArrangeCellArgs aca, GridDataControlDropDown uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style);

            GridDataTableProperties tableProperties = style.CellValue2 as GridDataTableProperties;

            if (tableProperties != null && uiElement.DropDownGrid.TableProperties != tableProperties)
            {
                this.SetTablePropertiesFromOther(uiElement.DropDownGrid.TableProperties, tableProperties);
            }
            
            if (style.ItemsSource != null && uiElement.DropDownGrid.ItemsSource != style.ItemsSource)
            {
                uiElement.DropDownGrid.ItemsSource = style.ItemsSource;
            }
            if (style.ChildRelationalColumn != null)
            {
                if (this.DisplayMember != style.ChildRelationalColumn)
                {
                    this.DisplayMember = style.ChildRelationalColumn;
                }
            }
            else
            {
                this.DisplayMember = style.DisplayMember;
            }
        }

        protected override void WireTemplateParts(GridDataControlDropDown uiElement)
        {            
            base.WireTemplateParts(uiElement);
            uiElement.DropDownGrid.RecordsSelectionChanged += new GridDataRecordsSelectionChangedEventHandler(OnDropdownGridRecordsSelectionChanged);
        }

        protected override void UnwireTemplateParts(GridDataControlDropDown uiElement)
        {
            uiElement.DropDownGrid.RecordsSelectionChanged -= new GridDataRecordsSelectionChangedEventHandler(OnDropdownGridRecordsSelectionChanged);
            base.UnwireTemplateParts(uiElement);
        }
        
        /// <summary>
        /// Display member of the dropdown grid.
        /// </summary>        
        public string DisplayMember
        {
            get;set;
        }

        public virtual void OnDropdownGridRecordsSelectionChanged(object sender, GridDataRecordsSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (!this.IsInArrange && e.AddedItems.Count > 0)
                {
                    var item = e.AddedItems[0].ToString();

                    var type = e.AddedItems[0].GetType();

                    if (NullableHelperInternal.IsComplexType(type) && type.BaseType != typeof(Type) && !DisplayMember.Equals(string.Empty))
                    {
                        item = TypeDescriptor.GetProperties(e.AddedItems[0].GetType())[DisplayMember].GetValue(e.AddedItems[0]).ToString();
                    }

                    if (this.CurrentCellUIElement != null && !this.CurrentCellUIElement.Text.Equals(item))
                    {
                        this.SuspendEvents = true;
                        this.CurrentCellUIElement.Text = item;
                        this.SuspendEvents = false;
                    }
                    
                    if (!this.AlreadyTextChanged)
                    {
                        SetControlText(item);
                        RaiseSelectedItemChangedEvent(CellRowColumnIndex, item);
                    }
                }
            }
        }

    }
}
