#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows.Markup;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;

[assembly: ProvideMetadata(typeof(Syncfusion.SfGrid.WPF.Expression.Design.Metadata))]

namespace Syncfusion.SfGrid.WPF.Expression.Design
{
    public class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                var builder = new AttributeTableBuilder();

                // tool box filtering
                builder.AddCustomAttributes(typeof(SfDataGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SfDataPager), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SfMultiColumnDropDownControl), new ToolboxBrowsableAttribute(true));

                builder.AddCustomAttributes(typeof(GridCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridGroupSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCaptionSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridIndentCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridHeaderIndentCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTableSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridExpanderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridStackedHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VirtualizingCellsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeaderRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TableSummaryRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CaptionSummaryRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupSummaryRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupDropArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupDropAreaItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NumericButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridFilterControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SortButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollableContentPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollableContentViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UpIndicatorContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DownIndicatorContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NumericButtonPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OrientedCellsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VisualContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DetailsViewDataGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DetailsViewContentPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DetailsViewRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DetailsViewRowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDetailsViewExpanderCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDetailsViewIndentCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackgroundVisualHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BusyDecorator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AddNewRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColumnChooser), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColumnChooserItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridRowHeaderIndentCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridRowHeaderCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintGridCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintHeaderCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintCaptionSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintGroupSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintTableSummaryCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintPageControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintPagePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridPrintPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintPreviewAreaControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintPreviewPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintOptionsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FilterToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdvancedFilterControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CheckboxFilterControl), new ToolboxBrowsableAttribute(false));

                //Data Grid
                var att = new NewItemTypesAttribute(
                  typeof(GridDateTimeColumn),
                  typeof(GridCheckBoxColumn),
                  typeof(GridNumericColumn),
                  typeof(GridTextColumn),
                  typeof(GridTemplateColumn),
                  typeof(GridMultiColumnDropDownList),
                  typeof(GridComboBoxColumn),
                  typeof(GridImageColumn),
                  typeof(GridEditorColumn),
                  typeof(GridCurrencyColumn),
                  typeof(GridPercentColumn),
                  typeof(GridMaskColumn),
                  typeof(GridTimeSpanColumn),
                  typeof(GridHyperlinkColumn),
                  typeof(GridUnBoundColumn));
                att.FactoryType = typeof(GridColumnFactory);
                builder.AddCustomAttributes(typeof(SfDataGrid), "Columns", att);

                var columnAtt = new CategoryAttribute("Columns");
                builder.AddCustomAttributes(typeof(SfDataGrid), "FrozenColumnCount", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AutoGenerateColumns", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CellStyle", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CellTemplateSelector", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CellStyleSelector", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupColumnDescriptions", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ColumnSizer", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ShowColumnWhenGrouped", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowResizingColumns", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowResizingHiddenColumns", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowDraggingColumns", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "Columns", columnAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ItemsSource", columnAtt);

                var rowAtt = new CategoryAttribute("Rows");
                builder.AddCustomAttributes(typeof(SfDataGrid), "RowStyle", rowAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectionMode", rowAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "RowHeight", rowAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "DataFetchSize", rowAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "RowStyleSelector", rowAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "NavigationMode", rowAtt);

                var sortAtt = new CategoryAttribute("Sorting");
                builder.AddCustomAttributes(typeof(SfDataGrid), "SortClickAction", sortAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowTriStateSorting", sortAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ShowSortNumbers", sortAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowSorting", sortAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SortColumnDescriptions", sortAtt);

                var headerAtt = new CategoryAttribute("Headers");
                builder.AddCustomAttributes(typeof(SfDataGrid), "StackedHeaderRows", headerAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "HeaderTemplate", headerAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "HeaderStyle", headerAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "HeaderRowHeight", headerAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ShowRowHeader", headerAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "RowHeaderWidth", headerAtt);

                var contxtMenuAtt = new CategoryAttribute("Context Menus");
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupDropItemContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupDropAreaContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupCaptionContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "HeaderContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryContextMenu", contxtMenuAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "RecordContextMenu", contxtMenuAtt);

                var groupingAtt = new CategoryAttribute("Grouping");
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupCaptionTextFormat", groupingAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowGrouping", groupingAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupDropAreaText", groupingAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AutoExpandGroups", groupingAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ShowGroupDropArea", groupingAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowFrozenGroupHeaders", groupingAtt);

                var filteringAtt = new CategoryAttribute("Filtering");
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowFiltering", filteringAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "FilterPopupStyle", filteringAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "FilterPopupTemplate", filteringAtt);

                var summaryAtt = new CategoryAttribute("Summaries");
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryCellStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryRows", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CaptionSummaryRow", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryRows", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CaptionSummaryRowStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryRowStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryRowStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryCellStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CaptionSummaryCellStyle", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CaptionSummaryRowStyleSelector", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryRowStyleSelector", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryRowStyleSelector", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GroupSummaryCellStyleSelector", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CaptionSummaryCellStyleSelector", summaryAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "TableSummaryCellStyleSelector", summaryAtt);

                var commonAtt = new CategoryAttribute("Common");
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectedIndex", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectedItem", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GridValidationMode", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GridCopyPasteOption", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "GridCopyPaste", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "EditTrigger", commonAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowEditing", commonAtt);

                var selectionAtt = new CategoryAttribute("Selection");
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectedItems", selectionAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowRowHoverHighlighting", selectionAtt);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AllowSelectionOnPointerPressed", selectionAtt);

                builder.AddCustomAttributes(typeof(SfDataGrid), "ShowBusyIndicator", CategoryAttribute.Appearance);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CurrentCellBorderThickness", CategoryAttribute.Appearance);
                builder.AddCustomAttributes(typeof(SfDataGrid), "DetailsViewDefinition", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "DetailsViewPadding", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "HideEmptyGridViewDefinition", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectedDetailsViewGrid", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "IsDynamicItemsSource", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SourceType", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "UsePLINQ", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AutoGenerateRelations", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "IsGroupDropAreaExpanded", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "CellRenderers", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "LiveDataUpdateMode", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "SelectionController", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "View", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SfDataGrid), "ItemsSource", BindableAttribute.Yes);
                builder.AddCustomAttributes(typeof(SfDataGrid), new DefaultBindingPropertyAttribute("ItemsSource"));
                builder.AddCustomAttributes(typeof(SfDataGrid), new ComplexBindingPropertiesAttribute("ItemsSource", ""));
                builder.AddCustomAttributes(typeof(SfDataGrid), new DefaultEventAttribute("SelectionChanged"));
                builder.AddCustomAttributes(typeof(SfDataGrid), "ItemsSource", BrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(SfDataGrid), "AutoGenerateColumns", BrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(SfDataGrid), new FeatureAttribute(typeof(GridMenuProvider)));

                //Grid Columns
                builder.AddCustomAttributes(typeof(GridColumn), "HeaderStyle", headerAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "HeaderTemplate", headerAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "HeaderText", headerAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowFiltering", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "ImmediateUpdateColumnFilter", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "FilterPopupStyle", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "FilterPopupTemplate", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowBlankFilters", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "FilterBehavior", filteringAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "FilterPredicates", BrowsableAttribute.No);

                var columnToooltipAtt = new CategoryAttribute("ToolTip");
                builder.AddCustomAttributes(typeof(GridColumn), "ToolTipTemplateSelector", columnToooltipAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "HeaderToolTipTemplate", columnToooltipAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "ToolTipTemplate", columnToooltipAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowDragging", commonAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowFocus", commonAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowEditing", commonAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "UpdateTrigger", commonAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "CellStyle", columnAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "CellStyleSelector", columnAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowGrouping", columnAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowSorting", columnAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "ColumnSizer", columnAtt);

                var cellBindingAtt = new CategoryAttribute("Cell Binding");
                builder.AddCustomAttributes(typeof(GridColumn), "MappingName", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "ValueBinding", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "DisplayBinding", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "UseBindingValue", cellBindingAtt);

                var columnTextAtt = new CategoryAttribute("Text");
                builder.AddCustomAttributes(typeof(GridColumn), "TextAlignment", columnTextAtt);
                builder.AddCustomAttributes(typeof(GridColumn), "Padding", CategoryAttribute.Appearance);
                builder.AddCustomAttributes(typeof(GridColumn), "IsHidden", CategoryAttribute.Appearance);
                builder.AddCustomAttributes(typeof(GridColumn), "HorizontalHeaderContentAlignment", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridColumn), "Width", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridColumn), "MaximumWidth", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridColumn), "MinimumWidth", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridColumn), "AllowResizing", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridColumn), "ActualWidth", BrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(GridColumn), "IsTextReadOnly", BrowsableAttribute.No);

                //Grid Template Column
                var cellAtt = new CategoryAttribute("Templates");
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "CellTemplate", cellAtt);
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "CellTemplateSelector", cellAtt);
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "EditTemplateSelector", cellAtt);
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "EditTemplate", cellAtt);
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "HorizontalAlignment", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridTemplateColumn), "VerticalAlignment", CategoryAttribute.Layout);

                //Grid Text Column
                builder.AddCustomAttributes(typeof(GridTextColumn), "TextWrapping", columnTextAtt);

                //Grid MultiColumn DropDown List
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "ItemsSource", columnAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "AllowIncrementalFiltering", filteringAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "AllowCasingforFilter", filteringAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "AllowAutoComplete", commonAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "SelectedIndex", commonAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "SelectedItem", commonAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpMaxHeight", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpMaxWidth", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpMinHeight", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpMinWidth", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpHeight", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "PopUpWidth", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "ShowResizeThumb", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "AllowSpinOnMouseWheel", CategoryAttribute.Appearance);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "DisplayMember", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "ValueMember", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridMultiColumnDropDownList), "ItemsSource", BrowsableAttribute.Yes);

                //Grid ComboBox Column
                builder.AddCustomAttributes(typeof(GridComboBoxColumn), "ItemsSource", columnAtt);
                builder.AddCustomAttributes(typeof(GridComboBoxColumn), "SelectedValuePath", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridComboBoxColumn), "DisplayMemberPath", cellBindingAtt);
                builder.AddCustomAttributes(typeof(GridComboBoxColumn), "ItemsSource", BrowsableAttribute.Yes);

                //Grid CheckBox Column
                builder.AddCustomAttributes(typeof(GridCheckBoxColumn), "IsThreeState", commonAtt);
                builder.AddCustomAttributes(typeof(GridCheckBoxColumn), "HorizontalAlignment", CategoryAttribute.Layout);
                builder.AddCustomAttributes(typeof(GridCheckBoxColumn), "VerticalAlignment", CategoryAttribute.Layout);

                //Grid Image Column
                var imageAtt = new CategoryAttribute("Image");
                builder.AddCustomAttributes(typeof(GridImageColumn), "Stretch", imageAtt);
                builder.AddCustomAttributes(typeof(GridImageColumn), "StretchDirection", imageAtt);
                builder.AddCustomAttributes(typeof(GridImageColumn), "ImageWidth", imageAtt);
                builder.AddCustomAttributes(typeof(GridImageColumn), "ImageHeight", imageAtt);

                //Grid Editor Column
                var validationAtt = new CategoryAttribute("Validation");
                builder.AddCustomAttributes(typeof(GridEditorColumn), "AllowNullValue", validationAtt);
                builder.AddCustomAttributes(typeof(GridEditorColumn), "MaxValidation", validationAtt);
                builder.AddCustomAttributes(typeof(GridEditorColumn), "MinValidation", validationAtt);
                builder.AddCustomAttributes(typeof(GridEditorColumn), "MinValue", validationAtt);
                builder.AddCustomAttributes(typeof(GridEditorColumn), "MaxValue", validationAtt);
                builder.AddCustomAttributes(typeof(GridEditorColumn), "AllowScrollingOnCircle", commonAtt);

                //Grid Currency Column
                var currencyAtt = new CategoryAttribute("Currency");
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyDecimalDigits", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyGroupSeparator", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencySymbol", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyDecimalSeparator", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyGroupSizes", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyPositivePattern", currencyAtt);
                builder.AddCustomAttributes(typeof(GridCurrencyColumn), "CurrencyNegativePattern", currencyAtt);

                //Grid Percent Column
                var percentAtt = new CategoryAttribute("Percent");
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentDecimalDigits", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentDecimalSeparator", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentGroupSeparator", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentGroupSizes", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentNegativePattern", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentPositivePattern", percentAtt);
                builder.AddCustomAttributes(typeof(GridPercentColumn), "PercentSymbol", percentAtt);

                //Grid Numeric Column
                var numbersAtt = new CategoryAttribute("Numbers");
                builder.AddCustomAttributes(typeof(GridNumericColumn), "NumberDecimalDigits", numbersAtt);
                builder.AddCustomAttributes(typeof(GridNumericColumn), "NumberDecimalSeparator", numbersAtt);
                builder.AddCustomAttributes(typeof(GridNumericColumn), "NumberGroupSeparator", numbersAtt);
                builder.AddCustomAttributes(typeof(GridNumericColumn), "NumberGroupSizes", numbersAtt);
                builder.AddCustomAttributes(typeof(GridNumericColumn), "NumberNegativePattern", numbersAtt);

                //Grid Mask Column
                var maskAtt = new CategoryAttribute("Mask");
                builder.AddCustomAttributes(typeof(GridMaskColumn), "SelectTextOnFocus", maskAtt);
                builder.AddCustomAttributes(typeof(GridMaskColumn), "Mask", maskAtt);
                builder.AddCustomAttributes(typeof(GridMaskColumn), "MaskFormat", maskAtt);
                builder.AddCustomAttributes(typeof(GridMaskColumn), "IsNumeric", commonAtt);

                //Grid TimeSpan Column
                builder.AddCustomAttributes(typeof(GridTimeSpanColumn), "AllowNull", commonAtt);
                builder.AddCustomAttributes(typeof(GridTimeSpanColumn), "AllowScrollingOnCircle", commonAtt);

                //Grid DateTime Column
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "AllowScrollingOnCircle", commonAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "AllowNullValue", commonAtt);
                var datetimeAtt = new CategoryAttribute("DateTime");
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "EnableClassicStyle", datetimeAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "DisableDateSelection", datetimeAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "ShowRepeatButton", datetimeAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "DateTime", datetimeAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "Pattern", datetimeAtt);
                builder.AddCustomAttributes(typeof(GridDateTimeColumn), "DateTimeFormat", datetimeAtt);

                return builder.CreateTable();
            }
        }
    }
}