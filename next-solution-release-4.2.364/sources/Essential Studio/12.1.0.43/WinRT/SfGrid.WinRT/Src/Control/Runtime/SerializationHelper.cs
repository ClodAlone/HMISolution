#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Syncfusion.Data;
#if WinRT
using Syncfusion.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
#else
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;

#endif

namespace Syncfusion.UI.Xaml.Grid
{
    internal class SerializationHelper
    {
#if !SILVERLIGHT
        public static object CreateSerializableInstance(object source, Type targettype)
        {
            var target = Activator.CreateInstance(targettype);
#if WinRT
            var sourceproperty = source.GetType().GetRuntimeProperties();
            var targetproperty = target.GetType().GetRuntimeProperties();
#else
            var sourceproperty = source.GetType().GetProperties();
            var targetproperty = target.GetType().GetProperties();
#endif

            foreach (var targetpropertyInfo in targetproperty)
            {
                if (!targetpropertyInfo.CanWrite) continue;
                var sourcepropertyInfo = sourceproperty.FirstOrDefault(property => property.Name.Equals(targetpropertyInfo.Name));
                if (sourcepropertyInfo != null && sourcepropertyInfo.CanWrite)
                {
#if WinRT
                    if (sourcepropertyInfo.PropertyType.GetTypeInfo().IsSerializable)
                    {
                        var value = sourcepropertyInfo.GetValue(source);
                        if (typeof (IList).GetTypeInfo().IsAssignableFrom(sourcepropertyInfo.PropertyType.GetTypeInfo()))
                        {
                            var list = value as IList;
                            var result = Activator.CreateInstance(targetpropertyInfo.PropertyType);
                            var addmethodinfo = typeof(IList).GetTypeInfo().DeclaredMethods.FirstOrDefault(method => method.Name.Equals("Add"));
#else
                    if (sourcepropertyInfo.PropertyType.IsSerializable)
                    {
                        var value = sourcepropertyInfo.GetValue(source,null);
                        if (typeof(IList).IsAssignableFrom(sourcepropertyInfo.PropertyType))
                        {
                            var list = value as IList;
                            var result = Activator.CreateInstance(targetpropertyInfo.PropertyType);
                            var addmethodinfo = typeof(IList).GetMethods().FirstOrDefault(method => method.Name.Equals("Add"));
#endif
                            foreach (var items in list)
                            {
                                var type = Type.GetType(string.Format("Syncfusion.UI.Xaml.Grid.Serializable{0}", items.GetType().Name)) ?? items.GetType();
                                var serializableitem = CreateSerializableInstance(items, type);
                                addmethodinfo.Invoke(result, new[] { serializableitem });
                            }
                            targetpropertyInfo.SetValue(target, result, null);
                        }
                        else
                            targetpropertyInfo.SetValue(target, value, null);
                    }
                    else
                    {
                        var value = sourcepropertyInfo.GetValue(source, null);
                        if (value == null)
                        {
                            targetpropertyInfo.SetValue(target, null, null);
                            continue;
                        }
                        var result = CreateSerializableInstance(value, targetpropertyInfo.PropertyType);
#if WinRT
                        if (result != null && (typeof (IList).GetTypeInfo().IsAssignableFrom(sourcepropertyInfo.PropertyType.GetTypeInfo())))
                        {
                            var list = value as IList;
                            var addmethodinfo = typeof(IList).GetTypeInfo().DeclaredMethods.FirstOrDefault(method => method.Name.Equals("Add"));
#else
                        if (result != null && (typeof(IList).IsAssignableFrom(sourcepropertyInfo.PropertyType)))
                        {
                            var list = value as IList;
                            var addmethodinfo = typeof(IList).GetMethods().FirstOrDefault(method => method.Name.Equals("Add"));
#endif
                            foreach (var items in list)
                            {
                                var type = Type.GetType(string.Format("Syncfusion.UI.Xaml.Grid.Serializable{0}", items.GetType().Name));
                                var serializableitem = CreateSerializableInstance(items, type);
                                addmethodinfo.Invoke(result, new[] {serializableitem});
                            }
                        }
                        targetpropertyInfo.SetValue(target, result, null);
                    }
                }
            }
            return target;
        }

        public static void UpdateValueFromDeSerializableInstance(object source, object target)
        {
#if WinRT
            var sourceproperty = source.GetType().GetRuntimeProperties();
            var targetproperty = target.GetType().GetRuntimeProperties();
#else
            var sourceproperty = source.GetType().GetProperties();
            var targetproperty = target.GetType().GetProperties();
#endif

            foreach (var sourcepropertyInfo in sourceproperty)
            {
                var targetpropertyInfo = targetproperty.FirstOrDefault(property => property.Name == sourcepropertyInfo.Name);
                if (!targetpropertyInfo.CanWrite) continue;
#if WinRT
                if (targetpropertyInfo.PropertyType.GetTypeInfo().IsSerializable)
                {
                    var value = sourcepropertyInfo.GetValue(source);
                    if (typeof (IList).GetTypeInfo().IsAssignableFrom(targetpropertyInfo.PropertyType.GetTypeInfo()))
                    {
                        var list = value as IList;
                        var result = Activator.CreateInstance(targetpropertyInfo.PropertyType);
                        var addmethodinfo = typeof (IList).GetTypeInfo().DeclaredMethods.FirstOrDefault(method => method.Name.Equals("Add"));
#else
                if (targetpropertyInfo.PropertyType.IsSerializable)
                {
                    var value = sourcepropertyInfo.GetValue(source, null);
                    if (typeof(IList).IsAssignableFrom(targetpropertyInfo.PropertyType))
                    {
                        var list = value as IList;
                        var result = Activator.CreateInstance(targetpropertyInfo.PropertyType);
                        var addmethodinfo = typeof(IList).GetMethods().FirstOrDefault(method => method.Name.Equals("Add"));

#endif
                        foreach (var items in list)
                        {
                            var name = items.GetType().FullName.Replace("Serializable", "");
                            var newitem = Activator.CreateInstance(Type.GetType(name));
                            UpdateValueFromDeSerializableInstance(items, newitem);
                            addmethodinfo.Invoke(result, new[] {newitem});
                        }
                        targetpropertyInfo.SetValue(target, result, null);
                    }
                    else
                        targetpropertyInfo.SetValue(target, value, null);
                }
                else
                {
                    var value = sourcepropertyInfo.GetValue(source, null);
                    if (value == null)
                    {
                        targetpropertyInfo.SetValue(target, null, null);
                        continue;
                    }
                    var result = Activator.CreateInstance(targetpropertyInfo.PropertyType);
                    UpdateValueFromDeSerializableInstance(value, result);
#if WinRT
                    if (result != null && (typeof(IList).GetTypeInfo().IsAssignableFrom(targetpropertyInfo.PropertyType.GetTypeInfo())))
                    {                        
                        var list = value as IList;
                        var addmethodinfo = typeof(IList).GetTypeInfo().DeclaredMethods.FirstOrDefault(method => method.Name.Equals("Add"));
#else
                    if (result != null && (typeof(IList).IsAssignableFrom(targetpropertyInfo.PropertyType)))
                    {
                        var list = value as IList;
                        var addmethodinfo = typeof(IList).GetMethods().FirstOrDefault(method => method.Name.Equals("Add"));

#endif
                        foreach (var items in list)
                        {
                            var name = items.GetType().FullName.Replace("Serializable", "");
                            var newitem = Activator.CreateInstance(Type.GetType(name));
                            UpdateValueFromDeSerializableInstance(items, newitem);
                            addmethodinfo.Invoke(result, new[] {newitem});
                        }
                    }
                    targetpropertyInfo.SetValue(target, result, null);
                }
            }
        }
#endif

        public static SerializableDataGrid CopyFromDataGrid(SfDataGrid dataGrid)
        {
            var serializableDataGrid = new SerializableDataGrid
                {
                    AllowDraggingColumns = dataGrid.AllowDraggingColumns,
#if !WINDOWS_PHONE8
                    AllowEditing = dataGrid.AllowEditing,
                    AllowFiltering = dataGrid.AllowFiltering,
                    CurrentCellBorderThickness = dataGrid.CurrentCellBorderThickness,
                    EditTrigger = dataGrid.EditTrigger,
#endif
                    AllowFrozenGroupHeaders = dataGrid.AllowFrozenGroupHeaders,
                    AllowGrouping = dataGrid.AllowGrouping,
                    AllowResizingColumns = dataGrid.AllowResizingColumns,
                    AllowResizingHiddenColumns=dataGrid.AllowResizingHiddenColumns,
                    AllowSelectionOnPointerPressed = dataGrid.AllowSelectionOnPointerPressed,
                    AllowSort = dataGrid.AllowSorting,
                    AllowTriStateSorting = dataGrid.AllowTriStateSorting,
                    AutoExpandGroups = dataGrid.AutoExpandGroups,
                    AutoGenerateColumns = dataGrid.AutoGenerateColumns,
                    AutoGenerateColumnsMode = dataGrid.AutoGenerateColumnsMode,
                    ColumnSizer = dataGrid.ColumnSizer,
                    FrozenColumnCount = dataGrid.FrozenColumnCount,
                    GroupCaptionTextFormat = dataGrid.GroupCaptionTextFormat,
                    GroupDropAreaText = dataGrid.GroupDropAreaText,
                    HeaderRowHeight = dataGrid.HeaderRowHeight,
                    IsGroupDropAreaExpanded = dataGrid.IsGroupDropAreaExpanded,
                    LiveDataUpdateMode = dataGrid.LiveDataUpdateMode,
                    RowHeight = dataGrid.RowHeight,
                    SelectionMode = dataGrid.SelectionMode,
                    ShowColumnWhenGrouped = dataGrid.ShowColumnWhenGrouped,
                    ShowGroupDropArea = dataGrid.ShowGroupDropArea,
                    ShowSortNumbers = dataGrid.ShowSortNumbers,
                    SortClickAction = dataGrid.SortClickAction,
                    Columns = new SerializableColumns(),
                    SortColumnDescriptions = new SerializableSortColumnDescriptions(),
                    GroupColumnDescriptions = new SerializableGroupColumnDescriptions(),
                    GroupSummaryRows = new SerializableGridSummaryRows(),
                    CaptionSummaryRow = dataGrid.CaptionSummaryRow != null ? CopyFromGridSummaryRow(dataGrid.CaptionSummaryRow): null,
                    TableSummaryRows = new SerializableGridSummaryRows(),
                    StackedHeaderRows = new SerializableStackedHeaderRows(),
                    AddNewRowPosition = dataGrid.AddNewRowPosition,
                    AllowDeleting = dataGrid.AllowDeleting,
                    AllowRowHoverHighlighting = dataGrid.AllowRowHoverHighlighting,
                    DataFetchSize = dataGrid.DataFetchSize,
                    GridValidationMode = dataGrid.GridValidationMode,
                    NavigationMode = dataGrid.NavigationMode,
                    ShowRowHeader = dataGrid.ShowRowHeader,
                    EditorSelectionBehavior= dataGrid.EditorSelectionBehavior,
                    UsePLINQ = dataGrid.UsePLINQ,
#if !SILVERLIGHT
                    IsDynamicItemsSource = dataGrid.IsDynamicItemsSource,
#endif
                    RowHeaderWidth = dataGrid.RowHeaderWidth,
                    GridCopyPasteOption = dataGrid.GridCopyPasteOption,
                };


            foreach (var column in dataGrid.Columns)
            {
                SerializableGridColumn serializableColumn;
                if (column is GridTextColumn)
                {
                    serializableColumn = new SerializableGridTextColumn
                        {
                            TextWrapping = (column as GridTextColumn).TextWrapping,
#if WinRT
                            IsSpellCheckEnabled = (column as GridTextColumn).IsSpellCheckEnabled,
#endif
                        };
                }
                else if (column is GridComboBoxColumn)
                {
                    serializableColumn = new SerializableGridComboBoxColumn()
                        {
                            SelectedValuePath = (column as GridComboBoxColumn).SelectedValuePath,
                            DisplayMemberPath = (column as GridComboBoxColumn).DisplayMemberPath,
#if WPF
                            StaysOpenOnEdit = (column as GridComboBoxColumn).StaysOpenOnEdit,
                            IsEditable = (column as GridComboBoxColumn).IsEditable
#endif
                        };
                }
                else if (column is GridCheckBoxColumn)
                {
                    serializableColumn = new SerializableGridCheckBoxColumn()
                        {
                            HorizontalAlignment = (column as GridCheckBoxColumn).HorizontalAlignment,
                            VerticalAlignment = (column as GridCheckBoxColumn).VerticalAlignment,
                            IsThreeState = (column as GridCheckBoxColumn).IsThreeState
                        };
                }
                else if(column is GridMultiColumnDropDownList)
                {
                    serializableColumn = new SerializableGridMultiColumnDropDownList()
                        {
                            DisplayMember = (column as GridMultiColumnDropDownList).DisplayMember,
                            ValueMember = (column as GridMultiColumnDropDownList).ValueMember,
                            ShowResizeThumb = (column as GridMultiColumnDropDownList).ShowResizeThumb,
                            PopUpHeight = (column as GridMultiColumnDropDownList).PopUpHeight,
                            PopUpWidth = (column as GridMultiColumnDropDownList).PopUpWidth,
                            AllowAutoComplete = (column as GridMultiColumnDropDownList).AllowAutoComplete,
                            AllowSpinOnMouseWheel = (column as GridMultiColumnDropDownList).AllowSpinOnMouseWheel,
                            AllowIncrementalFiltering = (column as GridMultiColumnDropDownList).AllowIncrementalFiltering,
                            AllowCasingforFilter = (column as GridMultiColumnDropDownList).AllowCasingforFilter,
                            PopUpMaxHeight = (column as GridMultiColumnDropDownList).PopUpMaxHeight,
                            PopUpMaxWidth = (column as GridMultiColumnDropDownList).PopUpMaxWidth,
                            PopUpMinHeight = (column as GridMultiColumnDropDownList).PopUpMinHeight,
                            PopUpMinWidth = (column as GridMultiColumnDropDownList).PopUpMinWidth,
                            IsTextReadOnly = (column as GridMultiColumnDropDownList).IsTextReadOnly,
                            AllowNullInput = (column as GridMultiColumnDropDownList).AllowNullInput,
                            AutoGenerateColumns = (column as GridMultiColumnDropDownList).AutoGenerateColumns,
                            GridColumnSizer = (column as GridMultiColumnDropDownList).GridColumnSizer,
                            IsAutoPopupSize = (column as GridMultiColumnDropDownList).IsAutoPopupSize,
                        };
                }
#if WinRT
                    else if (column is GridUpDownColumn)
                    {
                        serializableColumn = new SerializableGridUpDownColumn()
                            {
                            Step = (column as GridUpDownColumn).Step,
                            MinValue = (column as GridUpDownColumn).MinValue,
                            MaxValue = (column as GridUpDownColumn).MaxValue,
                            NumberDecimalDigits = (column as GridUpDownColumn).NumberDecimalDigits,
                            AutoReverse = (column as GridUpDownColumn).AutoReverse,
                            ParsingMode = (column as GridUpDownColumn).ParsingMode,
                            };
                    }
#endif
#if !WinRT && !WINDOWS_PHONE8
                else if (column is GridCurrencyColumn)
                {
                    serializableColumn = new SerializableGridCurrencyColumn()
                    {
                        AllowNullValue = (column as GridCurrencyColumn).AllowNullValue,
                        AllowScrollingOnCircle = (column as GridCurrencyColumn).AllowScrollingOnCircle,
                        CurrencyDecimalDigits = (column as GridCurrencyColumn).CurrencyDecimalDigits,
                        CurrencyDecimalSeparator=(column as GridCurrencyColumn).CurrencyDecimalSeparator,
                        CurrencyGroupSeparator=(column as GridCurrencyColumn).CurrencyGroupSeparator,
                        CurrencyGroupSizes=(column as GridCurrencyColumn).CurrencyGroupSizes,
                        CurrencyNegativePattern=(column as GridCurrencyColumn).CurrencyNegativePattern,
                        CurrencyPositivePattern=(column as GridCurrencyColumn).CurrencyPositivePattern,
                        CurrencySymbol=(column as GridCurrencyColumn).CurrencySymbol,
                        MaxValue = (column as GridCurrencyColumn).MinValue,
                        MinValue = (column as GridCurrencyColumn).MaxValue,
                        NullValue = (column as GridCurrencyColumn).NullValue,
                        NullText = (column as GridCurrencyColumn).NullText,
                        MaxValidation = (column as GridCurrencyColumn).MaxValidation,
                        MinValidation = (column as GridCurrencyColumn).MinValidation,
                    };
                }
                else if (column is GridPercentColumn)
                {
                    serializableColumn = new SerializableGridPercentageColumn()
                    {
                        AllowNullValue = (column as GridPercentColumn).AllowNullValue,
                        AllowScrollingOnCircle = (column as GridPercentColumn).AllowScrollingOnCircle,
                        PercentDecimalDigits = (column as GridPercentColumn).PercentDecimalDigits,
                        PercentDecimalSeparator=(column as GridPercentColumn).PercentDecimalSeparator,
                        PercentGroupSeparator = (column as GridPercentColumn).PercentGroupSeparator,
                        PercentGroupSizes = (column as GridPercentColumn).PercentGroupSizes,
                        PercentNegativePattern = (column as GridPercentColumn).PercentNegativePattern,
                        PercentPositivePattern = (column as GridPercentColumn).PercentPositivePattern,
                        PercentSymbol = (column as GridPercentColumn).PercentSymbol,
                        MaxValue = (column as GridPercentColumn).MinValue,
                        MinValue = (column as GridPercentColumn).MaxValue,
                        PercentEditMode = (column as GridPercentColumn).PercentEditMode,
                        NullValue = (column as GridPercentColumn).NullValue,
                        NullText = (column as GridPercentColumn).NullText,
                        MaxValidation = (column as GridPercentColumn).MaxValidation,
                        MinValidation = (column as GridPercentColumn).MinValidation,
                    };
                }
                else if (column is GridTimeSpanColumn)
                {
                    serializableColumn = new SerializableGridTimeSpanColumn()
                    {
                        AllowNull=(column as GridTimeSpanColumn).AllowNull,
                        AllowScrollingOnCircle = (column as GridTimeSpanColumn).AllowScrollingOnCircle,
                        NullText = (column as GridTimeSpanColumn).NullText,
                        Format = (column as GridTimeSpanColumn).Format,
                        ShowArrowButtons = (column as GridTimeSpanColumn).ShowArrowButtons,
                        MaxValue = (column as GridTimeSpanColumn).MaxValue,
                        MinValue = (column as GridTimeSpanColumn).MinValue,
                    };
                }
                else if (column is GridMaskColumn)
                {
                    serializableColumn = new SerializableGridMaskColumn()
                    {
                        SelectTextOnFocus  =(column as GridMaskColumn).SelectTextOnFocus ,
                        IsNumeric  =(column as GridMaskColumn).IsNumeric ,
                        Mask  = (column as GridMaskColumn).Mask,
                        MaskFormat = (column as GridMaskColumn).MaskFormat,
                        DateSeparator = (column as GridMaskColumn).DateSeparator,
                        DecimalSeparator = (column as GridMaskColumn).DecimalSeparator,
                        PromptChar = (column as GridMaskColumn).PromptChar,
                        TimeSeparator = (column as GridMaskColumn).TimeSeparator,
                    };
                }
#endif
#if !WINDOWS_PHONE8
                else if (column is GridDateTimeColumn)
                {
                    serializableColumn = new SerializableGridDateTimeColumn()
                    {
#if WinRT
                    AllowInlineEditing = (column as GridDateTimeColumn).AllowInlineEditing,
                    FormatString = (column as GridDateTimeColumn).FormatString,
                    ShowDropDownButton = (column as GridDateTimeColumn).ShowDropDownButton,
#else
                    AllowScrollingOnCircle = (column as GridDateTimeColumn).AllowScrollingOnCircle,
                    AllowNullValue = (column as GridDateTimeColumn).AllowNullValue,
                    EnableClassicStyle = (column as GridDateTimeColumn).EnableClassicStyle,
                    DisableDateSelection = (column as GridDateTimeColumn).DisableDateSelection,
                    ShowRepeatButton =(column as GridDateTimeColumn).ShowRepeatButton,
                    NullValue = (column as GridDateTimeColumn).NullValue,
                    NullText = (column as GridDateTimeColumn).NullText,
                    DateTimeFormat = (column as GridDateTimeColumn).DateTimeFormat,
                    CanEdit = (column as GridDateTimeColumn).CanEdit,
                    EnableBackspaceKey = (column as GridDateTimeColumn).EnableBackspaceKey,
                    EnableDeleteKey = (column as GridDateTimeColumn).EnableDeleteKey,
#endif
                    };
                }
                else if (column is GridHyperlinkColumn)
                {
                    serializableColumn = new SerializableGridHyperlinkColumn()
                    {
                        HorizontalAlignment = (column as GridHyperlinkColumn).HorizontalAlignment,
                        VerticalAlignment = (column as GridHyperlinkColumn).VerticalAlignment,
                    };
                }
                else if (column is GridNumericColumn)
                {
                    serializableColumn = new SerializableGridNumericColumn()
                    {
#if WinRT         
          BlockCharactersOnTextInput= (column as GridNumericColumn).BlockCharactersOnTextInput,
          AllowNullInput= (column as GridNumericColumn).AllowNullInput,
          FormatString= (column as GridNumericColumn).FormatString,
          ParsingMode= (column as GridNumericColumn).ParsingMode,
#else
#if WPF
          AllowScrollingOnCircle = (column as GridNumericColumn).AllowScrollingOnCircle, 
          AllowNullValue= (column as GridNumericColumn).AllowNullValue,
          NumberGroupSizes= (column as GridNumericColumn).NumberGroupSizes,
          MinValue = (column as GridNumericColumn).MinValue,
          MaxValue= (column as GridNumericColumn).MaxValue,
#endif
          NumberDecimalDigits = (column as GridNumericColumn).NumberDecimalDigits,
          NumberDecimalSeparator=(column as GridNumericColumn).NumberDecimalSeparator,
          NumberGroupSeparator=(column as GridNumericColumn).NumberGroupSeparator,
#if SILVERLIGHT
          NumberGroupSizes= (column as GridNumericColumn).NumberGroupSizes,         
#endif
          NumberNegativePattern= (column as GridNumericColumn).NumberNegativePattern,
          NullValue = (column as GridNumericColumn).NullValue,
          NullText = (column as GridNumericColumn).NullText,
          MaxValidation = (column as GridNumericColumn).MaxValidation,
          MinValidation = (column as GridNumericColumn).MinValidation,
#endif
                    };
                }
#endif
                else if(column is GridUnBoundColumn)
                {
                    serializableColumn = new SerializableGridUnBoundColumn()
                        {
                            CaseSensitive = (column as GridUnBoundColumn).CaseSensitive,
                            Format = (column as GridUnBoundColumn).Format,
                            Expression = (column as GridUnBoundColumn).Expression,
                        };
                }
                else if (column is GridTemplateColumn)
                {
                    serializableColumn = new SerializableGridTemplateColumn()
                        {
                            HorizontalAlignment = (column as GridTemplateColumn).HorizontalAlignment,
                            VerticalAlignment = (column as GridTemplateColumn).VerticalAlignment,
                        };
                }
                else
                {
                    serializableColumn = new SerializableGridTextColumn();
                }
                serializableColumn.AllowDragging = column.ReadLocalValue(GridColumn.AllowDraggingProperty) ==
                                               DependencyProperty.UnsetValue
                                                   ? serializableDataGrid.AllowDraggingColumns
                                                   : column.AllowDragging;
#if !WINDOWS_PHONE8
                serializableColumn.AllowFiltering = column.ReadLocalValue(GridColumn.AllowFilteringProperty) ==
                                                    DependencyProperty.UnsetValue
                                                        ? serializableDataGrid.AllowFiltering
                                                        : column.AllowFiltering;

                serializableColumn.AllowEditing = column.ReadLocalValue(GridColumn.AllowEditingProperty) ==
                                                  DependencyProperty.UnsetValue
                                                      ? serializableDataGrid.AllowEditing
                                                      : column.AllowEditing;
#endif
                serializableColumn.AllowGrouping = column.ReadLocalValue(GridColumn.AllowGroupingProperty) ==
                                                DependencyProperty.UnsetValue
                                                    ? serializableDataGrid.AllowGrouping
                                                    : column.AllowGrouping;
                serializableColumn.AllowResizing = column.ReadLocalValue(GridColumn.AllowResizingProperty) ==
                                                 DependencyProperty.UnsetValue
                                                     ? serializableDataGrid.AllowResizingColumns
                                                     : column.AllowResizing;
                serializableColumn.AllowSorting = column.ReadLocalValue(GridColumn.AllowSortingProperty) ==
                                               DependencyProperty.UnsetValue
                                                   ? serializableDataGrid.AllowSort
                                                   : column.AllowSorting;
                
                serializableColumn.ColumnSizer = column.ColumnSizer;
                serializableColumn.HeaderText = column.HeaderText;
                serializableColumn.HorizontalHeaderContentAlignment = column.HorizontalHeaderContentAlignment;
                serializableColumn.IsHidden = column.IsHidden;
                serializableColumn.MappingName = column.MappingName;
#if !WINDOWS_PHONE8
                serializableColumn.ImmediateUpdateColumnFilter = column.ImmediateUpdateColumnFilter;
                serializableColumn.AllowBlankFilters = column.AllowBlankFilters;
                serializableColumn.AllowFocus = column.AllowFocus;
#endif
                serializableColumn.MaximumWidth = column.MaximumWidth;
                serializableColumn.MinimumWidth = column.MinimumWidth;
                serializableColumn.Width = column.Width;
                serializableColumn.TextAlignment = column.TextAlignment;
                serializableColumn.UseBindingValue = column.UseBindingValue;

                serializableDataGrid.Columns.Add(serializableColumn);
            }

            foreach (var columnDescription in dataGrid.SortColumnDescriptions)
            {
                var sortColumn = new SerializableSortColumnDescription
                    {
                        ColumnName = columnDescription.ColumnName,
                        SortDirection = columnDescription.SortDirection
                    };
                serializableDataGrid.SortColumnDescriptions.Add(sortColumn);
            }

            foreach (var columnDescription in dataGrid.GroupColumnDescriptions)
            {
                var groupColumn = new SerializableGroupColumnDescription {ColumnName = columnDescription.ColumnName};
                serializableDataGrid.GroupColumnDescriptions.Add(groupColumn);
            }

            foreach (var groupSummaryRow in dataGrid.GroupSummaryRows)
            {
                var summaryRow = CopyFromGridSummaryRow(groupSummaryRow);
                serializableDataGrid.GroupSummaryRows.Add(summaryRow);
            }

            foreach (var tableSummaryRow in dataGrid.TableSummaryRows)
            {
                var summaryRow = CopyFromGridSummaryRow(tableSummaryRow);
                serializableDataGrid.TableSummaryRows.Add(summaryRow);
            }

            foreach (var stackedHeaderRow in dataGrid.StackedHeaderRows)
            {
                var headerRow = CopyFromGridStackedHeaderRow(stackedHeaderRow);
                serializableDataGrid.StackedHeaderRows.Add(headerRow);
            }
            return serializableDataGrid;
        }

        private static SerializableGridSummaryRow CopyFromGridSummaryRow(GridSummaryRow gridSummaryRow)
        {
            var summaryRow = new SerializableGridSummaryRow
                {
                    Name = gridSummaryRow.Name,
                    ShowSummaryInRow = gridSummaryRow.ShowSummaryInRow,
                    Title = gridSummaryRow.Title,
                    //TitleColumnCount = gridSummaryRow.TitleColumnCount,
                    SummaryColumns = new ObservableCollection<SerializableGridSummaryColumn>()
                };
            foreach (var summaryColumn in gridSummaryRow.SummaryColumns)
            {
                var column = new SerializableGridSummaryColumn
                {
                    Name = summaryColumn.Name,
                    MappingName = summaryColumn.MappingName,
                    Format = summaryColumn.Format,
                    SummaryType = summaryColumn.SummaryType
                };
                summaryRow.SummaryColumns.Add(column);
            }
            return summaryRow;
        }

        private static SerializableStackedHeaderRow CopyFromGridStackedHeaderRow(StackedHeaderRow stackedHeaderRow)
        {
            var headerRow = new SerializableStackedHeaderRow()
                {
                    Name = stackedHeaderRow.Name,
                    StackedColumns = new SerializableStackedColumns()
                };

            foreach (var stackedColumn in stackedHeaderRow.StackedColumns)
            {
                var stackheadercolumn = new SerializableStackedColumn()
                    {
                        ChildColumns = stackedColumn.ChildColumns,
                        HeaderText = stackedColumn.HeaderText
                    };
                headerRow.StackedColumns.Add(stackheadercolumn);
            }

            return headerRow;
        }

        public static void CopyToDataGrid(SerializableDataGrid serializableDataGrid, SfDataGrid dataGrid)
        {
            dataGrid.AllowDraggingColumns = serializableDataGrid.AllowDraggingColumns;
            
#if !WINDOWS_PHONE8
            dataGrid.AllowFiltering = serializableDataGrid.AllowFiltering;
            dataGrid.CurrentCellBorderThickness = serializableDataGrid.CurrentCellBorderThickness;
            dataGrid.EditTrigger = serializableDataGrid.EditTrigger;
            dataGrid.AllowEditing = serializableDataGrid.AllowEditing;
#endif
            dataGrid.AllowFrozenGroupHeaders = serializableDataGrid.AllowFrozenGroupHeaders;
            dataGrid.AllowGrouping = serializableDataGrid.AllowGrouping;
            dataGrid.AllowResizingColumns = serializableDataGrid.AllowResizingColumns;
            dataGrid.AllowResizingHiddenColumns = serializableDataGrid.AllowResizingHiddenColumns;
            dataGrid.AllowSelectionOnPointerPressed = serializableDataGrid.AllowSelectionOnPointerPressed;
            dataGrid.AllowSorting = serializableDataGrid.AllowSort;
            dataGrid.AllowTriStateSorting = serializableDataGrid.AllowTriStateSorting;
            dataGrid.AutoExpandGroups = serializableDataGrid.AutoExpandGroups;
            dataGrid.AutoGenerateColumnsMode = serializableDataGrid.AutoGenerateColumnsMode;
            dataGrid.AutoGenerateColumns = serializableDataGrid.AutoGenerateColumns;
            dataGrid.ColumnSizer = serializableDataGrid.ColumnSizer;
            dataGrid.FrozenColumnCount = serializableDataGrid.FrozenColumnCount;
            dataGrid.GroupCaptionTextFormat = serializableDataGrid.GroupCaptionTextFormat;
            dataGrid.GroupDropAreaText = serializableDataGrid.GroupDropAreaText;
            dataGrid.HeaderRowHeight = serializableDataGrid.HeaderRowHeight;
            dataGrid.IsGroupDropAreaExpanded = serializableDataGrid.IsGroupDropAreaExpanded;
            dataGrid.LiveDataUpdateMode = serializableDataGrid.LiveDataUpdateMode;
            dataGrid.RowHeight = serializableDataGrid.RowHeight;
            dataGrid.SelectionMode = serializableDataGrid.SelectionMode;
            dataGrid.ShowColumnWhenGrouped = serializableDataGrid.ShowColumnWhenGrouped;
            dataGrid.ShowGroupDropArea = serializableDataGrid.ShowGroupDropArea;
            dataGrid.ShowSortNumbers = serializableDataGrid.ShowSortNumbers;
            dataGrid.SortClickAction = serializableDataGrid.SortClickAction;
            dataGrid.AddNewRowPosition = serializableDataGrid.AddNewRowPosition;
            dataGrid.AllowDeleting= serializableDataGrid.AllowDeleting;
            dataGrid.AllowRowHoverHighlighting = serializableDataGrid.AllowRowHoverHighlighting;
            dataGrid.DataFetchSize = serializableDataGrid.DataFetchSize;
            dataGrid.GridValidationMode = serializableDataGrid.GridValidationMode;
            dataGrid.NavigationMode = serializableDataGrid.NavigationMode;
            dataGrid.ShowRowHeader = serializableDataGrid.ShowRowHeader;
            dataGrid.EditorSelectionBehavior = serializableDataGrid.EditorSelectionBehavior;
            dataGrid.UsePLINQ = serializableDataGrid.UsePLINQ;
#if !SILVERLIGHT
            dataGrid.IsDynamicItemsSource = serializableDataGrid.IsDynamicItemsSource;
#endif
            dataGrid.RowHeaderWidth = serializableDataGrid.RowHeaderWidth;
            dataGrid.GridCopyPasteOption = serializableDataGrid.GridCopyPasteOption;

            //Columns = new SerializableColumns(),
            var columns = new Columns();
            foreach (var serializableColumn in serializableDataGrid.Columns)
            {
                GridColumn column;
                if (serializableColumn is SerializableGridTextColumn)
                {
                    column = new GridTextColumn
                        {
                            TextWrapping = (serializableColumn as SerializableGridTextColumn).TextWrapping,
#if WinRT
                            IsSpellCheckEnabled = (serializableColumn as SerializableGridTextColumn).IsSpellCheckEnabled,
#endif
                        };
                }
                else if (serializableColumn is SerializableGridComboBoxColumn)
                {
                    column = new GridComboBoxColumn()
                        {
                            SelectedValuePath = (serializableColumn as SerializableGridComboBoxColumn).SelectedValuePath,
                            DisplayMemberPath = (serializableColumn as SerializableGridComboBoxColumn).DisplayMemberPath,
#if WPF
                            StaysOpenOnEdit = (serializableColumn as SerializableGridComboBoxColumn).StaysOpenOnEdit,
                            IsEditable = (serializableColumn as SerializableGridComboBoxColumn).IsEditable
#endif
                        };
                }
                else if(serializableColumn is SerializableGridCheckBoxColumn)
                {
                    column = new GridCheckBoxColumn()
                        {
                            HorizontalAlignment = (serializableColumn as SerializableGridCheckBoxColumn).HorizontalAlignment,
                            VerticalAlignment = (serializableColumn as SerializableGridCheckBoxColumn).VerticalAlignment,
                            IsThreeState = (serializableColumn as SerializableGridCheckBoxColumn).IsThreeState
                        };
                }
                else if (serializableColumn is SerializableGridMultiColumnDropDownList)
                {
                    column = new GridMultiColumnDropDownList()
                    {
                        DisplayMember = (serializableColumn as SerializableGridMultiColumnDropDownList).DisplayMember,
                        ValueMember = (serializableColumn as SerializableGridMultiColumnDropDownList).ValueMember,
                        ShowResizeThumb = (serializableColumn as SerializableGridMultiColumnDropDownList).ShowResizeThumb,
                        PopUpHeight = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpHeight,
                        PopUpWidth = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpWidth,
                        AllowAutoComplete = (serializableColumn as SerializableGridMultiColumnDropDownList).AllowAutoComplete,
                        AllowSpinOnMouseWheel = (serializableColumn as SerializableGridMultiColumnDropDownList).AllowSpinOnMouseWheel,
                        AllowIncrementalFiltering = (serializableColumn as SerializableGridMultiColumnDropDownList).AllowIncrementalFiltering,
                        AllowCasingforFilter = (serializableColumn as SerializableGridMultiColumnDropDownList).AllowCasingforFilter,
                        PopUpMaxHeight = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpMaxHeight,
                        PopUpMaxWidth = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpMaxWidth,
                        PopUpMinHeight = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpMinHeight,
                        PopUpMinWidth = (serializableColumn as SerializableGridMultiColumnDropDownList).PopUpMinWidth,
                        IsTextReadOnly = (serializableColumn as SerializableGridMultiColumnDropDownList).IsTextReadOnly,
                        AllowNullInput = (serializableColumn as SerializableGridMultiColumnDropDownList).AllowNullInput,
                        AutoGenerateColumns = (serializableColumn as SerializableGridMultiColumnDropDownList).AutoGenerateColumns,
                        GridColumnSizer = (serializableColumn as SerializableGridMultiColumnDropDownList).GridColumnSizer,
                        IsAutoPopupSize = (serializableColumn as SerializableGridMultiColumnDropDownList).IsAutoPopupSize,
                    };
                }

                    #if WinRT
                    else if (serializableColumn is SerializableGridUpDownColumn)
                    {
                        column =new GridUpDownColumn()
                            {
                            Step = (serializableColumn as SerializableGridUpDownColumn).Step,
                            MinValue = (serializableColumn as SerializableGridUpDownColumn).MinValue,
                            MaxValue = (serializableColumn as SerializableGridUpDownColumn).MaxValue,
                            NumberDecimalDigits = (serializableColumn as SerializableGridUpDownColumn).NumberDecimalDigits,
                            AutoReverse = (serializableColumn as SerializableGridUpDownColumn).AutoReverse,
                            ParsingMode = (serializableColumn as SerializableGridUpDownColumn).ParsingMode,
                            };
                    }
#endif
#if !WinRT && !WINDOWS_PHONE8
                else if (serializableColumn is SerializableGridCurrencyColumn)
                {
                    column =new GridCurrencyColumn()
                    {
                        AllowNullValue = (serializableColumn as SerializableGridCurrencyColumn).AllowNullValue,
                        AllowScrollingOnCircle = (serializableColumn as SerializableGridCurrencyColumn).AllowScrollingOnCircle,
                        CurrencyDecimalDigits = (serializableColumn as SerializableGridCurrencyColumn).CurrencyDecimalDigits,
                        CurrencyDecimalSeparator=(serializableColumn as SerializableGridCurrencyColumn).CurrencyDecimalSeparator,
                        CurrencyGroupSeparator=(serializableColumn as SerializableGridCurrencyColumn).CurrencyGroupSeparator,
                        CurrencyGroupSizes=(serializableColumn as SerializableGridCurrencyColumn).CurrencyGroupSizes,
                        CurrencyNegativePattern=(serializableColumn as SerializableGridCurrencyColumn).CurrencyNegativePattern,
                        CurrencyPositivePattern=(serializableColumn as SerializableGridCurrencyColumn).CurrencyPositivePattern,
                        CurrencySymbol=(serializableColumn as SerializableGridCurrencyColumn).CurrencySymbol,
                        MaxValue = (serializableColumn as SerializableGridCurrencyColumn).MinValue,
                        MinValue = (serializableColumn as SerializableGridCurrencyColumn).MaxValue,
                        NullValue = (serializableColumn as SerializableGridCurrencyColumn).NullValue,
                        NullText = (serializableColumn as SerializableGridCurrencyColumn).NullText,
                        MaxValidation = (serializableColumn as SerializableGridCurrencyColumn).MaxValidation,
                        MinValidation = (serializableColumn as SerializableGridCurrencyColumn).MinValidation,
                    };
                }
                else if (serializableColumn is SerializableGridPercentageColumn)
                {
                    column =new GridPercentColumn()
                    {
                        AllowNullValue = (serializableColumn as SerializableGridPercentageColumn).AllowNullValue,
                        AllowScrollingOnCircle = (serializableColumn as SerializableGridPercentageColumn).AllowScrollingOnCircle,
                        PercentDecimalDigits = (serializableColumn as SerializableGridPercentageColumn).PercentDecimalDigits,
                        PercentDecimalSeparator = (serializableColumn as SerializableGridPercentageColumn).PercentDecimalSeparator,
                        PercentGroupSeparator = (serializableColumn as SerializableGridPercentageColumn).PercentGroupSeparator,
                        PercentGroupSizes = (serializableColumn as SerializableGridPercentageColumn).PercentGroupSizes,
                        PercentNegativePattern = (serializableColumn as SerializableGridPercentageColumn).PercentNegativePattern,
                        PercentPositivePattern = (serializableColumn as SerializableGridPercentageColumn).PercentPositivePattern,
                        PercentSymbol = (serializableColumn as SerializableGridPercentageColumn).PercentSymbol,
                        MaxValue = (serializableColumn as SerializableGridPercentageColumn).MinValue,
                        MinValue = (serializableColumn as SerializableGridPercentageColumn).MaxValue,
                        PercentEditMode = (serializableColumn as SerializableGridPercentageColumn).PercentEditMode,
                        NullValue = (serializableColumn as SerializableGridPercentageColumn).NullValue,
                        NullText = (serializableColumn as SerializableGridPercentageColumn).NullText,
                        MaxValidation = (serializableColumn as SerializableGridPercentageColumn).MaxValidation,
                        MinValidation = (serializableColumn as SerializableGridPercentageColumn).MinValidation,
                    };
                }
                else if (serializableColumn is SerializableGridTimeSpanColumn)
                {
                    column =new GridTimeSpanColumn()
                    {
                        AllowNull=(serializableColumn as SerializableGridTimeSpanColumn).AllowNull,
                        AllowScrollingOnCircle = (serializableColumn as SerializableGridTimeSpanColumn).AllowScrollingOnCircle,
                        NullText = (serializableColumn as SerializableGridTimeSpanColumn).NullText,
                        Format = (serializableColumn as SerializableGridTimeSpanColumn).Format,
                        ShowArrowButtons = (serializableColumn as SerializableGridTimeSpanColumn).ShowArrowButtons,
                        MaxValue = (serializableColumn as SerializableGridTimeSpanColumn).MaxValue,
                        MinValue = (serializableColumn as SerializableGridTimeSpanColumn).MinValue,
                    };
                }
                else if (serializableColumn is SerializableGridMaskColumn)
                {
                    column =new GridMaskColumn()
                    {
                        SelectTextOnFocus  =(serializableColumn as SerializableGridMaskColumn).SelectTextOnFocus ,
                        IsNumeric  =(serializableColumn as SerializableGridMaskColumn).IsNumeric ,
                        Mask  = (serializableColumn as SerializableGridMaskColumn).Mask,
                        MaskFormat = (serializableColumn as SerializableGridMaskColumn).MaskFormat,
                        DateSeparator = (serializableColumn as SerializableGridMaskColumn).DateSeparator,
                        DecimalSeparator = (serializableColumn as SerializableGridMaskColumn).DecimalSeparator,
                        PromptChar = (serializableColumn as SerializableGridMaskColumn).PromptChar,
                        TimeSeparator = (serializableColumn as SerializableGridMaskColumn).TimeSeparator,
                    };
                }
#endif
#if !WINDOWS_PHONE8
                else if (serializableColumn is SerializableGridDateTimeColumn)
                {
                    column =new GridDateTimeColumn()
                    {
#if WinRT
                    AllowInlineEditing = (serializableColumn as SerializableGridDateTimeColumn).AllowInlineEditing,
                    FormatString = (serializableColumn as SerializableGridDateTimeColumn).FormatString,
                    ShowDropDownButton = (serializableColumn as SerializableGridDateTimeColumn).ShowDropDownButton,
#else
                    AllowScrollingOnCircle = (serializableColumn as SerializableGridDateTimeColumn).AllowScrollingOnCircle,
                    AllowNullValue = (serializableColumn as SerializableGridDateTimeColumn).AllowNullValue,
                    EnableClassicStyle = (serializableColumn as SerializableGridDateTimeColumn).EnableClassicStyle,
                    DisableDateSelection = (serializableColumn as SerializableGridDateTimeColumn).DisableDateSelection,
                    ShowRepeatButton =(serializableColumn as SerializableGridDateTimeColumn).ShowRepeatButton,
                    NullValue = (serializableColumn as SerializableGridDateTimeColumn).NullValue,
                    NullText = (serializableColumn as SerializableGridDateTimeColumn).NullText,
                    DateTimeFormat = (serializableColumn as SerializableGridDateTimeColumn).DateTimeFormat,
                    CanEdit = (serializableColumn as SerializableGridDateTimeColumn).CanEdit,
                    EnableBackspaceKey = (serializableColumn as SerializableGridDateTimeColumn).EnableBackspaceKey,
                    EnableDeleteKey = (serializableColumn as SerializableGridDateTimeColumn).EnableDeleteKey,
#endif
                    };
                }
                else if (serializableColumn is SerializableGridHyperlinkColumn)
                {
                    column = new GridHyperlinkColumn()
                    {
                        HorizontalAlignment = (serializableColumn as SerializableGridHyperlinkColumn).HorizontalAlignment,
                        VerticalAlignment = (serializableColumn as SerializableGridHyperlinkColumn).VerticalAlignment,
                    };
                }
                else if (serializableColumn is SerializableGridNumericColumn)
                {
                    column =new GridNumericColumn()
                    {
#if WinRT         
          BlockCharactersOnTextInput= (serializableColumn as SerializableGridNumericColumn).BlockCharactersOnTextInput,
          AllowNullInput= (serializableColumn as SerializableGridNumericColumn).AllowNullInput,
          FormatString= (serializableColumn as SerializableGridNumericColumn).FormatString,
          ParsingMode= (serializableColumn as SerializableGridNumericColumn).ParsingMode,
#else
#if !SILVERLIGHT
          AllowScrollingOnCircle = (serializableColumn as SerializableGridNumericColumn).AllowScrollingOnCircle, 
          AllowNullValue= (serializableColumn as SerializableGridNumericColumn).AllowNullValue,
          MinValue = (serializableColumn as SerializableGridNumericColumn).MinValue,
          MaxValue = (serializableColumn as SerializableGridNumericColumn).MaxValue,
#endif
#if WPF
          NumberGroupSizes= (serializableColumn as SerializableGridNumericColumn).NumberGroupSizes,
#endif
          NumberDecimalDigits = (serializableColumn as SerializableGridNumericColumn).NumberDecimalDigits,
          NumberDecimalSeparator=(serializableColumn as SerializableGridNumericColumn).NumberDecimalSeparator ,
          NumberGroupSeparator=(serializableColumn as SerializableGridNumericColumn).NumberGroupSeparator ,
#if SILVERLIGHT
          NumberGroupSizes= (serializableColumn as SerializableGridNumericColumn).NumberGroupSizes,         
#endif
          NumberNegativePattern= (serializableColumn as SerializableGridNumericColumn).NumberNegativePattern,
          NullValue = (serializableColumn as SerializableGridNumericColumn).NullValue,
          NullText = (serializableColumn as SerializableGridNumericColumn).NullText,
          MaxValidation = (serializableColumn as SerializableGridNumericColumn).MaxValidation,
          MinValidation = (serializableColumn as SerializableGridNumericColumn).MinValidation,
#endif
                    };
                }
#endif
                else if (serializableColumn is SerializableGridUnBoundColumn)
                {
                    column = new GridUnBoundColumn()
                    {
                        CaseSensitive = (serializableColumn as SerializableGridUnBoundColumn).CaseSensitive,
                        Format = (serializableColumn as SerializableGridUnBoundColumn).Format,
                        Expression = (serializableColumn as SerializableGridUnBoundColumn).Expression,
                    };
                }
                else if (serializableColumn is SerializableGridTemplateColumn)
                {
                    column = new GridTemplateColumn()
                        {
                            HorizontalAlignment = (serializableColumn as SerializableGridTemplateColumn).HorizontalAlignment,
                            VerticalAlignment = (serializableColumn as SerializableGridTemplateColumn).VerticalAlignment,
                        };
                }
                else
                {
                    column = new GridTextColumn();
                }

                column.AllowDragging = serializableColumn.AllowDragging;
                column.AllowGrouping = serializableColumn.AllowGrouping;
                column.AllowResizing = serializableColumn.AllowResizing;
                column.AllowSorting = serializableColumn.AllowSorting;
                column.ColumnSizer = serializableColumn.ColumnSizer;
                column.HeaderText = serializableColumn.HeaderText;
                column.HorizontalHeaderContentAlignment = serializableColumn.HorizontalHeaderContentAlignment;
                column.IsHidden = serializableColumn.IsHidden;
#if !WINDOWS_PHONE8
                column.AllowEditing = serializableColumn.AllowEditing;
                column.AllowFocus = serializableColumn.AllowFocus;
                column.AllowFiltering = serializableColumn.AllowFiltering;
                column.ImmediateUpdateColumnFilter = serializableColumn.ImmediateUpdateColumnFilter;
                column.AllowBlankFilters = serializableColumn.AllowBlankFilters;
#endif
                column.MappingName = serializableColumn.MappingName;
                column.MaximumWidth = serializableColumn.MaximumWidth;
                column.MinimumWidth = serializableColumn.MinimumWidth;
                column.Width = serializableColumn.Width;
                column.TextAlignment = serializableColumn.TextAlignment;
                column.UseBindingValue = serializableColumn.UseBindingValue;
                
                columns.Add(column);
            }
            dataGrid.Columns = columns;
            var sortColumnDescriptions = new SortColumnDescriptions();
            foreach (var sortColumnDescription in serializableDataGrid.SortColumnDescriptions)
            {
                var sortColumn = new SortColumnDescription
                    {
                        ColumnName = sortColumnDescription.ColumnName,
                        SortDirection = sortColumnDescription.SortDirection
                    };
                dataGrid.SortColumnDescriptions.Add(sortColumn);
            }
            var groupColumnDescriptions = new GroupColumnDescriptions();
            foreach (var groupColumnDescription in serializableDataGrid.GroupColumnDescriptions)
            {
                var groupColumn = new GroupColumnDescription { ColumnName = groupColumnDescription.ColumnName };
                dataGrid.GroupColumnDescriptions.Add(groupColumn);
            }
            var groupSummaryRows = new ObservableCollection<GridSummaryRow>();
            foreach (var gridSummaryRow in serializableDataGrid.GroupSummaryRows)
            {
                var summaryRow = CopyToGridSummaryRow(gridSummaryRow);
                groupSummaryRows.Add(summaryRow);
            }
            dataGrid.GroupSummaryRows = groupSummaryRows;
            var captionSummaryRow = serializableDataGrid.CaptionSummaryRow != null ? CopyToGridSummaryRow(serializableDataGrid.CaptionSummaryRow) : null;
            dataGrid.CaptionSummaryRow = captionSummaryRow;
            var tableSummaryRows = new ObservableCollection<GridSummaryRow>();
            foreach (var serializableGridSummaryRow in serializableDataGrid.TableSummaryRows)
            {
                var summaryRow = CopyToGridSummaryRow(serializableGridSummaryRow);
                tableSummaryRows.Add(summaryRow);
            }
            dataGrid.TableSummaryRows = tableSummaryRows;


        }

        private static GridSummaryRow CopyToGridSummaryRow(SerializableGridSummaryRow gridSummaryRow)
        {
            var summaryRow = new GridSummaryRow
            {
                Name = gridSummaryRow.Name,
                ShowSummaryInRow = gridSummaryRow.ShowSummaryInRow,
                Title = gridSummaryRow.Title,
                //TitleColumnCount = gridSummaryRow.TitleColumnCount,
                SummaryColumns = new ObservableCollection<ISummaryColumn>()
            };
            foreach (var summaryColumn in gridSummaryRow.SummaryColumns)
            {
                var column = new GridSummaryColumn
                {
                    Name = summaryColumn.Name,
                    MappingName = summaryColumn.MappingName,
                    Format = summaryColumn.Format,
                    SummaryType = summaryColumn.SummaryType
                };
                summaryRow.SummaryColumns.Add(column);
            }
            return summaryRow;
        }

        private static StackedHeaderRow CopyToGridStackedHeaderRow(SerializableStackedHeaderRow stackedHeaderRow)
        {
            var headerRow = new StackedHeaderRow()
                {
                    Name = stackedHeaderRow.Name,
                };

            foreach (var stackedColumn in stackedHeaderRow.StackedColumns)
            {
                var stackedheadercolumn = new StackedColumn()
                    {
                        ChildColumns = stackedColumn.ChildColumns,
                        HeaderText = stackedColumn.HeaderText
                    };
                headerRow.StackedColumns.Add(stackedheadercolumn);
            }

            return headerRow;
        }
    }

    [DataContract(Name="SfDataGrid")]
#if SILVERLIGHT
    public 
#else
    internal
#endif
    class SerializableDataGrid
    {
        [DataMember(EmitDefaultValue=false)]
        public bool AllowDraggingColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowEditing { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowFiltering { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowFrozenGroupHeaders { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowGrouping { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowResizingColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowResizingHiddenColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowSelectionOnPointerPressed { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowSort { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowTriStateSorting { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AutoExpandGroups { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AutoGenerateColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public AutoGenerateColumnsMode AutoGenerateColumnsMode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridLengthUnitType ColumnSizer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Thickness CurrentCellBorderThickness { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public EditTrigger EditTrigger { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int FrozenColumnCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string GroupCaptionTextFormat { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string GroupDropAreaText { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double HeaderRowHeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsGroupDropAreaExpanded { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public LiveDataUpdateMode LiveDataUpdateMode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double RowHeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridSelectionMode SelectionMode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowColumnWhenGrouped { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowGroupDropArea { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowSortNumbers { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SortClickAction SortClickAction { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableColumns Columns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableSortColumnDescriptions SortColumnDescriptions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableGroupColumnDescriptions GroupColumnDescriptions { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableGridSummaryRows GroupSummaryRows { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableGridSummaryRow CaptionSummaryRow { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableGridSummaryRows TableSummaryRows { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableStackedHeaderRows StackedHeaderRows { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowDeleting { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public AddNewRowPosition AddNewRowPosition { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridValidationMode GridValidationMode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowRowHoverHighlighting { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int DataFetchSize { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public NavigationMode NavigationMode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowRowHeader { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public EditorSelectionBehavior EditorSelectionBehavior { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool UsePLINQ { get; set; }

#if !SILVERLIGHT
        [DataMember(EmitDefaultValue = false)]
        public bool IsDynamicItemsSource { get; set; }
#endif

        [DataMember(EmitDefaultValue = false)]
        public double RowHeaderWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridCopyPasteOption GridCopyPasteOption { get; set; }
    }

    [KnownType(typeof(SerializableGridTextColumn))]
    [KnownType(typeof(SerializableGridUnBoundColumn))]
    [KnownType(typeof(SerializableGridTemplateColumn))]
    [KnownType(typeof(SerializableGridComboBoxColumn))]
    [KnownType(typeof(SerializableGridCheckBoxColumn))]
    [KnownType(typeof(SerializableGridMultiColumnDropDownList))]
    [KnownType(typeof(SerializableGridDateTimeColumn))]
    [KnownType(typeof(SerializableGridHyperlinkColumn))]
    [KnownType(typeof(SerializableGridNumericColumn))]
#if !WinRT
    [KnownType(typeof(SerializableGridTimeSpanColumn))]
    [KnownType(typeof(SerializableGridCurrencyColumn))]
    [KnownType(typeof(SerializableGridMaskColumn))]
    [KnownType(typeof(SerializableGridPercentageColumn))]
#endif
#if WinRT
    [KnownType(typeof(SerializableGridUpDownColumn))]
#endif
    [DataContract(Name = "GridColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    abstract class SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public string MappingName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public HorizontalAlignment HorizontalHeaderContentAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string HeaderText { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowSorting { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridLengthUnitType ColumnSizer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double Width { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsHidden { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double MaximumWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double MinimumWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowDragging { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowGrouping { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowResizing { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowFiltering { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ImmediateUpdateColumnFilter { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowBlankFilters { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowEditing { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowFocus { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TextAlignment TextAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool UseBindingValue { get; set; }
    }

    [DataContract(Name = "GridTextColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGridTextColumn : SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public TextWrapping TextWrapping { get; set; }
#if WinRT
        [DataMember(EmitDefaultValue = false)]
        public bool IsSpellCheckEnabled { get; set; }
#endif
    }

    [DataContract(Name = "GridTemplateColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGridTemplateColumn : SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public VerticalAlignment VerticalAlignment { get; set; }
    }

    [DataContract(Name = "GridComboBoxColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridComboBoxColumn: SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public string SelectedValuePath { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DisplayMemberPath { get; set; }

#if WPF
        [DataMember(EmitDefaultValue = false)]
        public bool StaysOpenOnEdit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsEditable { get; set; }
#endif
    }

    [DataContract(Name = "GridCheckBoxColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridCheckBoxColumn : SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public VerticalAlignment VerticalAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsThreeState { get; set; }
    }

    [DataContract(Name = "GridMultiColumnDropDownList")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridMultiColumnDropDownList : SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public string DisplayMember { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ValueMember { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Visibility ShowResizeThumb { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpHeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowAutoComplete { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowSpinOnMouseWheel { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowIncrementalFiltering { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowCasingforFilter { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpMaxHeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpMaxWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpMinHeight { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double PopUpMinWidth { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsTextReadOnly { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowNullInput { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AutoGenerateColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public GridLengthUnitType GridColumnSizer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsAutoPopupSize { get; set; }
    }

    [DataContract(Name = "GridUnBoundColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridUnBoundColumn : SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public bool CaseSensitive { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Format { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Expression { get; set; }
    }

#if !WINDOWS_PHONE8
    [DataContract(Name = "GridDateTimeColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridDateTimeColumn : SerializableGridColumn
    {
#if WinRT
        [DataMember(EmitDefaultValue = false)]
        public bool AllowInlineEditing { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FormatString { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowDropDownButton { get; set; }
#else
        [DataMember(EmitDefaultValue = false)]
        public bool AllowScrollingOnCircle { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowNullValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool EnableClassicStyle { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool DisableDateSelection { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowRepeatButton { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? NullValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NullText { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTimeFormatInfo DateTimeFormat { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool EnableBackspaceKey { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool EnableDeleteKey { get; set; }
#endif
    }

    [DataContract(Name="GridHyperlinkColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridHyperlinkColumn:SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public HorizontalAlignment HorizontalAlignment { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public VerticalAlignment VerticalAlignment { get; set; }
    }

    [DataContract(Name = "GridEditorColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
 class SerializableGridEditorColumn:SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public bool AllowScrollingOnCircle { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowNullValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal MinValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public decimal MaxValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object NullValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NullText { get; set; }

#if !WinRT
        [DataMember(EmitDefaultValue = false)]
        public MaxValidation MaxValidation { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MinValidation MinValidation { get; set; }
#endif
    }

    [DataContract(Name="GridNumericColumn")]
#if SILVERLIGHT
    public class SerializableGridNumericColumn:SerializableGridEditorColumn
#elif WPF
    internal class SerializableGridNumericColumn:SerializableGridEditorColumn 
#else
    internal class SerializableGridNumericColumn:SerializableGridColumn
#endif
    {
#if WinRT
        [DataMember(EmitDefaultValue = false)]
        public bool BlockCharactersOnTextInput { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowNullInput { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string FormatString { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Parsers ParsingMode { get; set; }

#else
        [DataMember(EmitDefaultValue = false)]
        public int NumberDecimalDigits { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NumberDecimalSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NumberGroupSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool EnableGroupSeparator { get; set; }

#if SILVERLIGHT

        [DataMember(EmitDefaultValue = false)]
        public int[] NumberGroupSizes { get; set; }

#elif WPF

        [DataMember(EmitDefaultValue = false)]
        public Int32Collection NumberGroupSizes { get; set; }

#endif
        [DataMember(EmitDefaultValue = false)]
        public int NumberNegativePattern { get; set; }
#endif
    }

#if WinRT
    [DataContract(Name = "GridUpDownColumn")]
    internal class SerializableGridUpDownColumn:SerializableGridColumn
    {
        [DataMember(EmitDefaultValue=false)]
        public double Step { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public CultureInfo Culture { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public double MinValue { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public double MaxValue { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public int NumberDecimalDigits { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public bool AutoReverse { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public Parsers ParsingMode { get; set; }
    }
#endif

#if !WinRT

    [DataContract(Name="GridCurrencyColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridCurrencyColumn:SerializableGridEditorColumn
    {
        [DataMember(EmitDefaultValue=false)]
        public int CurrencyDecimalDigits { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public string CurrencyGroupSeparator { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public string CurrencySymbol { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public string CurrencyDecimalSeparator { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public int CurrencyPositivePattern { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public int CurrencyNegativePattern { get; set; }

#if SILVERLIGHT
        [DataMember(EmitDefaultValue=false)]
        public int[] CurrencyGroupSizes { get; set; }
#else
        [DataMember(EmitDefaultValue=false)]
        public Int32Collection CurrencyGroupSizes { get; set; }
#endif
    }

    [DataContract(Name="GridMaskColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridMaskColumn:SerializableGridColumn
    {   
        [DataMember(EmitDefaultValue = false)]
        public bool SelectTextOnFocus { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsNumeric { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Mask { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MaskFormat MaskFormat { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DateSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DecimalSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public char PromptChar { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string TimeSeparator { get; set; }
    }

    [DataContract(Name="GridPercentageColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridPercentageColumn:SerializableGridEditorColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public int PercentDecimalDigits { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PercentDecimalSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PercentGroupSeparator { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int PercentNegativePattern { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int PercentPositivePattern { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PercentSymbol { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PercentEditMode PercentEditMode { get; set; }

#if SILVERLIGHT
        [DataMember(EmitDefaultValue = false)]
        public int[] PercentGroupSizes { get; set; }
#else
        [DataMember(EmitDefaultValue = false)]
        public Int32Collection PercentGroupSizes { get; set; }
#endif
    }

    [DataContract(Name="GridTimeSpanColumn")]
#if SILVERLIGHT
    public
#else
    internal 
#endif
    class SerializableGridTimeSpanColumn:SerializableGridColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public bool AllowNull { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool AllowScrollingOnCircle { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string NullText { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Format { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowArrowButtons { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TimeSpan MaxValue { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TimeSpan MinValue { get; set; }
    }

#endif
#endif

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableColumns : ObservableCollection<SerializableGridColumn>
    {
    }

    [DataContract(Name = "SortColumnDescription")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableSortColumnDescription
    {
        [DataMember(EmitDefaultValue = false)]
        public string ColumnName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ListSortDirection SortDirection { get; set; }
    }

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableSortColumnDescriptions : ObservableCollection<SerializableSortColumnDescription>
    {
        
    }

    [DataContract(Name = "GroupColumnDescription")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGroupColumnDescription
    {
        [DataMember(EmitDefaultValue = false)]
        public string ColumnName { get; set; }
    }

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGroupColumnDescriptions : ObservableCollection<SerializableGroupColumnDescription>
    {
        
    }

    [DataContract(Name = "GridSummaryRow")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGridSummaryRow
    {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool ShowSummaryInRow { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ObservableCollection<SerializableGridSummaryColumn> SummaryColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TitleColumnCount { get; set; }
    }

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGridSummaryRows : ObservableCollection<SerializableGridSummaryRow>
    {
        
    }

    [DataContract(Name = "GridSummaryColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableGridSummaryColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public string Format { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MappingName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SummaryType SummaryType { get; set; }
    }

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableStackedHeaderRows : ObservableCollection<SerializableStackedHeaderRow>
    {

    }


    [DataContract(Name = "StackedHeaderRow")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableStackedHeaderRow
    {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SerializableStackedColumns StackedColumns { get; set; }
    }

#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableStackedColumns : ObservableCollection<SerializableStackedColumn>
    {

    }

    [DataContract(Name = "StackedColumn")]
#if SILVERLIGHT
    public
#else
    internal
#endif
    class SerializableStackedColumn
    {
        [DataMember(EmitDefaultValue = false)]
        public string ChildColumns { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string HeaderText { get; set; }
    }
}
