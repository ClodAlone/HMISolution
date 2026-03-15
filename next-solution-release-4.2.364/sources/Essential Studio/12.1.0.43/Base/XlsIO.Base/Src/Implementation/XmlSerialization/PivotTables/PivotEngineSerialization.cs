#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
using System.Data;
using Syncfusion.XlsIO.Implementation.PivotTables;
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables
{
    public class PivotEngineSerialization
    {
        public PivotEngine PopulatePivotEngine(IWorksheet sheet, PivotTableImpl pivotTable)
        {
            PivotEngine pivotEngine = new PivotEngine();
            PivotCacheImpl cache = pivotTable.Cache;

            if (cache.SourceRange != null)
            {
                IWorksheet sheet_1 = cache.SourceRange.Worksheet;
                WorksheetImpl worksheet = sheet_1 as WorksheetImpl;
                DataTable dt = worksheet.PEExportDataTable(sheet_1[cache.SourceRange.AddressLocal], ExcelExportDataTableOptions.ColumnNames |
                   ExcelExportDataTableOptions.DetectColumnTypes,pivotTable );
                pivotEngine.DataSource = dt;
                if (pivotTable.RowFields.Count > 0)
                {
                    List<int> lstFields = new List<int>();
                    PivotTableFields fields = pivotTable.Fields;
                    List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Row);

                    for (int filterFieldsIndex = 0, filterFieldsCount = filteredFields.Count; filterFieldsIndex < filterFieldsCount; filterFieldsIndex++)
                    {
                        PivotFieldImpl field = filteredFields[filterFieldsIndex];

                        lstFields.Add(fields.IndexOf(field));
                    }
                    for (int filterFieldIndex = 0,filterFieldCount = pivotTable .RowFields .Count ; filterFieldIndex < filterFieldCount; filterFieldIndex++)
                    {
                        PivotCacheFieldImpl cacheFields = pivotTable.Cache.CacheFields[lstFields[filterFieldIndex]];
                        if ((cacheFields.DataType & PivotDataType.String) != 0)
                            pivotEngine.PivotRows.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldIndex]].Name, Comparer = new CustomComparer() });
                        else if (((cacheFields.DataType & PivotDataType.Number) != 0) && ((cacheFields.DataType & PivotDataType.Integer) == 0))
                            pivotEngine.PivotRows.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldIndex]].Name, Comparer = new CustomComparer() });
                        else
                            pivotEngine.PivotRows.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldIndex]].Name, Comparer = new CustomComparer() });
                        PivotFilterCollections filterCollections = pivotTable.Fields[lstFields[filterFieldIndex]].PivotFilters as PivotFilterCollections;
                        if ( filterCollections.ValueFilter != null)
                        {
                            IPivotValueLableFilter valueFilter = filterCollections.ValueFilter;
                            string ExpressionValue = null;
                            if (valueFilter.Type == PivotFilterType.CaptionEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " = ","'", valueFilter.Value1,"'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionGreaterThan)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " > ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionGreaterThanOrEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name , " >= " , "'" , valueFilter.Value1 , "'" );
                                pivotEngine.Filters.Add(new FilterExpression { Expression =ExpressionValue  , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionLessThan)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " < ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression =ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionLessThanOrEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " <= ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionBetween)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " >= ", "'", valueFilter.Value1, "'","AND " , pivotTable.Fields[lstFields[filterFieldIndex]].Name , " <= " , "'" , valueFilter.Value2 , "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionNotEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldIndex]].Name, " <> ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue, Name = "A1" });
                            }
                            else
                                throw new ArgumentException(valueFilter.Type.ToString() + " Filter type is not supported in Layout");
                        }
                        int fieldIndex = lstFields[filterFieldIndex];
                        Dictionary<int, Dictionary<string, int>> dictAllFieldsvalues = new Dictionary<int, Dictionary<string, int>>();
                        int key = 0;
                        foreach (int currentFieldIndex in lstFields)
                        {
                            PivotFieldImpl field = fields[currentFieldIndex];
                            SortedList<PivotTableSerializator.ComparisonPair, object> lstFieldsData = PivotTableSerializator.SortFieldValues(field.CacheField);
                            Dictionary<string, int> dictFieldValues = new Dictionary<string, int>();
                            int lstFieldsDataCount = 0;
                            while (lstFieldsDataCount < lstFieldsData.Count)
                            {
                                if (lstFieldsData.Keys[lstFieldsDataCount].Value != null)
                                    dictFieldValues.Add(lstFieldsData.Keys[lstFieldsDataCount].Value.ToString(), lstFieldsDataCount);
                                lstFieldsDataCount++;
                            }
                            dictAllFieldsvalues.Add(key++, dictFieldValues);
                        }
                        Dictionary<string, int> fieldValues = dictAllFieldsvalues[lstFields.IndexOf(fieldIndex)];
                        if (pivotTable.Fields[fieldIndex].IsMultiSelected)
                        {
                            string filterValue = null;
                            bool bFirst = true;
                            for (int itemIndex = 0, itemCount = pivotTable.Fields[fieldIndex].Items.Count;itemIndex<itemCount  ; itemIndex++)
                            {
                                if (pivotTable.Fields[fieldIndex].Items[itemIndex].Visible == true)
                                {
                                    PivotFieldItem item = pivotTable.Fields[fieldIndex].Items[itemIndex] as PivotFieldItem;
                                    if (item.Text != null && fieldValues.ContainsKey(item.Text))
                                    {
                                        if (!bFirst)
                                            filterValue = filterValue + " OR ";
                                        filterValue = filterValue + pivotTable.Fields[fieldIndex].Name + "=";
                                        filterValue = filterValue + "'" + item.Text + "'";
                                        bFirst = false;
                                    }
                                }
                            }
                            pivotEngine.Filters.Add(new FilterExpression { Expression = filterValue, Name = pivotTable.Fields[fieldIndex].Name });
                        }
                    }
                }
                if (pivotTable.ColumnFields.Count > 0)
                {
                    List<int> lstFields = new List<int>();
                    PivotTableFields fields = pivotTable.Fields;
                    List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Column);

                    for (int fieldIndex = 0, filterFieldsCount = filteredFields.Count; fieldIndex < filterFieldsCount; fieldIndex++)
                    {
                        PivotFieldImpl field = filteredFields[fieldIndex];

                        lstFields.Add(fields.IndexOf(field));
                    }
                    if (pivotTable.ColFieldsOrder.Count > 0)
                    {
                        lstFields.Clear();
                        for (int i = 0; i < pivotTable.ColFieldsOrder.Count; i++)
                        {
                            lstFields.Add(pivotTable.ColFieldsOrder[i]);
                        }
                    }
                    for (int filterFieldsIndex = 0, filterFieldsCount = pivotTable.ColumnFields.Count; filterFieldsIndex < filterFieldsCount; filterFieldsIndex++)
                    {
                        PivotCacheFieldImpl cacheFields = pivotTable.Cache.CacheFields[lstFields[filterFieldsIndex]];
                        if ((cacheFields.DataType & PivotDataType.String) != 0)
                            pivotEngine.PivotColumns.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldsIndex]].Name });
                        else if (((cacheFields.DataType & PivotDataType.Number) != 0) && ((cacheFields.DataType & PivotDataType.Integer) == 0))
                            pivotEngine.PivotRows.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldsIndex]].Name, Comparer = new CustomComparer() });
                        else
                            pivotEngine.PivotColumns.Add(new PivotItem { FieldMappingName = pivotTable.Fields[lstFields[filterFieldsIndex]].Name, Comparer = new CustomComparer() });
                        PivotFilterCollections filterCollections = pivotTable.Fields[lstFields[filterFieldsIndex]].PivotFilters as PivotFilterCollections;
                        if (filterCollections.ValueFilter != null)
                        {
                            IPivotValueLableFilter valueFilter = filterCollections.ValueFilter;
                            string ExpressionValue = null;
                            if (valueFilter.Type == PivotFilterType.CaptionEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " = ","'", valueFilter.Value1,"'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionGreaterThan)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " > ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionGreaterThanOrEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name , " >= " , "'" , valueFilter.Value1 , "'" );
                                pivotEngine.Filters.Add(new FilterExpression { Expression =ExpressionValue  , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionLessThan)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " < ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression =ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionLessThanOrEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " <= ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionBetween)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " >= ", "'", valueFilter.Value1, "'","AND " , pivotTable.Fields[lstFields[filterFieldsIndex]].Name , " <= " , "'" , valueFilter.Value2 , "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue , Name = "A1" });
                            }
                            else if (valueFilter.Type == PivotFilterType.CaptionNotEqual)
                            {
                                ExpressionValue = string.Format("{0}{1}{2}{3}{4}", pivotTable.Fields[lstFields[filterFieldsIndex]].Name, " <> ", "'", valueFilter.Value1, "'");
                                pivotEngine.Filters.Add(new FilterExpression { Expression = ExpressionValue, Name = "A1" });
                            }
                            else
                                throw new ArgumentException(valueFilter.Type.ToString() + " Filter type is not supported in Layout");
                        }

                        int fieldIndex = lstFields[filterFieldsIndex];
                        Dictionary<int, Dictionary<string, int>> dictAllFieldsvalues = new Dictionary<int, Dictionary<string, int>>();
                        int key = 0;
                        foreach (int currentFieldIndex in lstFields)
                        {
                            PivotFieldImpl field = fields[currentFieldIndex];
                            SortedList<PivotTableSerializator.ComparisonPair, object> lstFieldsData = PivotTableSerializator.SortFieldValues(field.CacheField);
                            Dictionary<string, int> dictFieldValues = new Dictionary<string, int>();
                            int lstFieldsDataCount = 0;
                            while (lstFieldsDataCount < lstFieldsData.Count)
                            {
                                if (lstFieldsData.Keys[lstFieldsDataCount].Value != null)
                                    dictFieldValues.Add(lstFieldsData.Keys[lstFieldsDataCount].Value.ToString(), lstFieldsDataCount);
                                lstFieldsDataCount++;
                            }
                            dictAllFieldsvalues.Add(key++, dictFieldValues);
                        }
                        Dictionary<string, int> fieldValues = dictAllFieldsvalues[lstFields.IndexOf(fieldIndex)];
                        if (pivotTable.Fields[fieldIndex].IsMultiSelected)
                        {
                            string filterValue = null;
                            bool bFirst = true;
                            for (int itemIndex = 0,itemCount = pivotTable.Fields[fieldIndex].Items.Count;itemIndex < itemCount ; itemIndex++)
                            {
                                if (pivotTable.Fields[fieldIndex].Items[itemIndex].Visible == true)
                                {
                                    PivotFieldItem item = pivotTable.Fields[fieldIndex].Items[itemIndex] as PivotFieldItem;
                                    if (item.Text != null && fieldValues.ContainsKey(item.Text))
                                    {
                                        if (!bFirst)
                                            filterValue = filterValue + " OR ";
                                        filterValue = filterValue + pivotTable.Fields[fieldIndex].Name + "=";
                                        filterValue = filterValue + "'" + item.Text + "'";
                                        bFirst = false;
                                    }
                                }
                            }
                            pivotEngine.Filters.Add(new FilterExpression { Expression = filterValue, Name = pivotTable.Fields[fieldIndex].Name });
                        }
                    }
                }
                if (pivotTable.DataFields.Count > 0)
                {
                    for (int i = 0; i < pivotTable.DataFields.Count; i++)
                    {
                        if (pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Sum || pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Default || pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.None)
                            pivotEngine.PivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTable.DataFields[i].Field.Name, FieldHeader = pivotTable.DataFields[i].Name, SummaryType = SummaryType.DoubleTotalSum, Format = "#,###0.0" });
                        else if (pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Count)
                            pivotEngine.PivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTable.DataFields[i].Field.Name, FieldHeader = pivotTable.DataFields[i].Name, SummaryType = SummaryType.Count, Format = "#,###0.0" });
                        else if (pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Max)
                            pivotEngine.PivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTable.DataFields[i].Field.Name, FieldHeader = pivotTable.DataFields[i].Name, SummaryType = SummaryType.DoubleMaximum, Format = "#,###0.0" });
                        else if (pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Average)
                            pivotEngine.PivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTable.DataFields[i].Field.Name, FieldHeader = pivotTable.DataFields[i].Name, SummaryType = SummaryType.DoubleAverage, Format = "#,##0.0" });
                        else if (pivotTable.DataFields[i].Subtotal == PivotSubtotalTypes.Min)
                            pivotEngine.PivotCalculations.Add(new PivotComputationInfo { FieldName = pivotTable.DataFields[i].Field.Name, FieldHeader = pivotTable.DataFields[i].Name, SummaryType = SummaryType.DoubleMinimum, Format = "#,###0.0" });
                    }
                }
                if (pivotTable.PageFields.Count > 0)
                {
                    List<int> lstFields = new List<int>();
                    PivotTableFields fields = pivotTable.Fields;
                    List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Page);

                    for (int fieldsIndex = 0, filterFielsCount = filteredFields.Count; fieldsIndex < filterFielsCount; fieldsIndex++)
                    {
                        PivotFieldImpl field = filteredFields[fieldsIndex];

                        lstFields.Add(fields.IndexOf(field));
                    }
                    Dictionary<int, Dictionary<string, int>> dictAllFieldsvalues = new Dictionary<int, Dictionary<string, int>>();
                    int key = 0;
                    foreach (int filteredFieldsIndex in lstFields)
                    {
                        PivotFieldImpl field = fields[filteredFieldsIndex];
                        SortedList<PivotTableSerializator.ComparisonPair, object> lstFieldsData = PivotTableSerializator.SortFieldValues(field.CacheField);
                        Dictionary<string, int> dictCurrentFieldValues = new Dictionary<string, int>();
                        int lstFieldsDataCount = 0;
                        while (lstFieldsDataCount < lstFieldsData.Count)
                        {
                            if (lstFieldsData.Keys[lstFieldsDataCount].Value != null)
                                dictCurrentFieldValues.Add(lstFieldsData.Keys[lstFieldsDataCount].Value.ToString(), lstFieldsDataCount);
                            lstFieldsDataCount++;
                        }
                        dictAllFieldsvalues.Add(key++, dictCurrentFieldValues);
                    }
                    foreach (int currentIndex in lstFields)
                    {
                        Dictionary<string, int> dictFieldValues = dictAllFieldsvalues[lstFields.IndexOf(currentIndex)];
                        if (!pivotTable.Fields[currentIndex].IsMultiSelected)
                        {
                            if (pivotTable.Fields[currentIndex].PivotFilters[0] != null)
                            {
                                string FilterValue = pivotTable.Fields[currentIndex].PivotFilters[0].Value1;
                                if (FilterValue != null)
                                {
                                    if (dictFieldValues.ContainsKey(FilterValue))
                                    {
                                        //  string strFilterValue = pivotTable.PageFields[i].FilterValue;
                                        int intFilterValue; double doubleFilterValue;
                                        bool b_intFilter = int.TryParse(FilterValue, out intFilterValue);
                                        bool b_doubleFilter = double.TryParse(FilterValue, out doubleFilterValue);
                                        if (!b_intFilter && !b_doubleFilter)
                                            FilterValue = "'" + FilterValue + "'"; pivotEngine.Filters.Add(new FilterExpression { Expression = pivotTable.Fields[currentIndex].Name + "=" + FilterValue, Name = pivotTable.Fields[currentIndex].Name });
                                    }
                                }
                            }
                        }
                        else if (pivotTable.Fields[currentIndex].IsMultiSelected)
                        {
                            string filterValue = null;
                            bool bFirst = true;
                            for (int filterIndex = 0,filterCount = pivotTable.Fields[currentIndex].Items.Count;filterIndex < filterCount; filterIndex++)
                            {
                                if (pivotTable.Fields[currentIndex].Items[filterIndex].Visible == true)
                                {
                                    PivotFieldItem item = pivotTable.Fields[currentIndex].Items[filterIndex] as PivotFieldItem;
                                    if (item.Text != null && dictFieldValues.ContainsKey(item.Text))
                                    {
                                        if (!bFirst)
                                            filterValue = filterValue + " OR ";
                                        filterValue = filterValue + pivotTable.Fields[currentIndex].Name + "=";
                                        filterValue = filterValue + "'" + item.Text + "'";
                                        bFirst = false;
                                    }
                                }
                            }
                            pivotEngine.Filters.Add(new FilterExpression { Expression = filterValue, Name = pivotTable.Fields[currentIndex].Name });
                        }
                    }
                }
                // pivotEngine.ShowGrandTotals = pivotTable.ColumnGrand;
                pivotTable.RowsPerPage = pivotTable.PageFields.Count;
                pivotEngine.Populate();
                RenderPivotTable(pivotEngine, pivotTable.Worksheet, pivotTable);
                return pivotEngine;
            }
            else if (cache.HasNamedRange)
                throw new ArgumentNullException("PivotTable cannot be created for table with Named Range");
            else
                return null;

        }

        ExtendedFormatImpl extendedFormat;
        PivotTableImpl pivotTableImple;
        /// <summary>
        /// Filling the Pivot engine values 
        /// </summary>
        /// <param name="pivotEngine"></param>
        /// <param name="pivotSheet"></param>
        /// <param name="pivotTable"></param>
        /// <param name="sheetRow"></param>
        public void RenderPivotTable(PivotEngine pivotEngine, IWorksheet pivotSheet, PivotTableImpl pivotTable)
        {
            string delimiter = new string((char)131, 1);
            pivotTableImple = pivotTable;
            if (pivotTable.PageFields.Count > 0)
            {
                int currentPlace = 2;
                for (int pageFieldsCount = pivotTable.PageFields.Count - 1; pageFieldsCount >= 0; pageFieldsCount--)
                {
                    pivotSheet[pivotTable.Location.Row - currentPlace, pivotTable.Location.Column].Value2 = pivotTable.PageFields[pageFieldsCount].Name;
                    {
                        PivotFieldImpl field = pivotTable.PageFields[pageFieldsCount] as PivotFieldImpl;
                        if (!field.IsMultiSelected)
                        {
                            if (pivotTable.PageFields[pageFieldsCount].PivotFilters[0] != null)
                                pivotSheet[pivotTable.Location.Row - currentPlace, pivotTable.Location.Column + 1].Value2 = pivotTable.PageFields[pageFieldsCount].PivotFilters[0].Value1;
                            else
                                pivotSheet[pivotTable.Location.Row - currentPlace, pivotTable.Location.Column + 1].Value2 = "(All)";
                        }
                        else
                            pivotSheet[pivotTable.Location.Row - currentPlace, pivotTable.Location.Column + 1].Value2 = "(Multiple Items)";
                    }
                    pivotSheet[pivotTable.Location.Row - currentPlace, pivotTable.Location.Column + 1].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                    currentPlace++;
                }
            }
            int maxColumnCount = 0;
            int sheetRow = pivotTable.Location.Row;
            int initialSheetRow = sheetRow;
            int adjustRow = sheetRow + 1;
            if (pivotTable.ColumnFields.Count <= 0)
                adjustRow = sheetRow;
            int adjustColumn = pivotTable.Location.Column;
            bool insertRowLabel = true;
            if (pivotTable.RowFields.Count <= 0)
                insertRowLabel = false;
            int sheetRowFieldCount = 0;
            if (pivotTable.RowFields.Count > 0)
                sheetRowFieldCount = -(pivotTable.RowFields.Count - 1);//Number of Row Fields - 2
            if (pivotTable.RowFields.Count == 0 && pivotTable.DataFields.Count > 1)
            {
                sheetRowFieldCount = sheetRowFieldCount - 1;
            }
            int sheetColumn = pivotTable.Location.Column;
            if (pivotTable.ColumnFields.Count > 0 && pivotTable.RowFields.Count >= 1 || (pivotTable.DataFields.Count == 1 && pivotTable.ColumnFields.Count > 0))
            {
                pivotSheet[sheetRow, sheetColumn + 1].Value2 = "Column Labels";
                if (pivotTable.Options.ColumnHeaderCaption != null)
                    pivotSheet[sheetRow, sheetColumn + 1].Value2 = pivotTable.Options.ColumnHeaderCaption;
                pivotSheet[sheetRow, sheetColumn + 2].Value2 = " ";
                maxColumnCount = sheetColumn + 1;
                sheetRow++;
            }
            else
            {
                if (pivotTable.ColumnFields.Count > 0)
                {
                    pivotSheet[sheetRow, sheetColumn].Value2 = "Column Labels";
                    pivotSheet[sheetRow, sheetColumn + 1].Value2 = " ";
                    maxColumnCount = sheetColumn;
                }

            }

            PivotTableLayout layOut = new PivotTableLayout();
            PivotValueCollections pivotValue = new PivotValueCollections();
            int minusSheetRow = pivotTable.Location.Row;
            int minusSheetColumn = pivotTable.Location.Column;

            extendedFormat = new ExtendedFormatImpl(pivotTable.Workbook.Application, pivotTable.Workbook);
            int RowLabelsRow = pivotTable.Location.Row;
            int maxRow = 0;
            int indentLevel = 0;
            string checkNextExpanderValue = null;
            int rowExpanderCount = -1;
            int columnExpanderCount = -1;
            bool makeExpanderCount = false;
            PivotTableParts previousRowStyle = PivotTableParts.None;
            PivotTableParts previousColumnStyle = PivotTableParts.None;
            PivotTableParts pivotPartStyle = PivotTableParts.WholeTable;
            List<int> lstTotalCell = new List<int>();
            for (int i = 0; i < pivotEngine.RowCount; i++)
            {
                columnExpanderCount++;
                for (int j = 0; j < pivotEngine.ColumnCount; j++)
                {
                    if (pivotEngine[i, j] != null)
                    {
                        if ((pivotEngine[i, j].CellType & PivotCellType.ColumnHeaderCell) == 0 && pivotTable.RowFields.Count > 0)
                        {
                            //Like MS Excel
                            if (pivotTable.ColumnFields.Count == 0 && insertRowLabel)
                            {
                                pivotSheet[sheetRow, sheetColumn].Value2 = "Row Labels";
                                sheetRow++; insertRowLabel = false;
                                FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                    "Row Labels", pivotEngine[i, j].CellType, layOut, PivotTableParts.WholeTable);

                            }
                            if ((pivotEngine[i, j].CellType & PivotCellType.TopLeftCell) != 0)
                            {
                                pivotSheet[sheetRow, sheetColumn].Value2 = pivotSheet[sheetRow, sheetColumn].Value2.ToString() + pivotEngine[i, j].Value;
                                pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.FirstColumn | PivotTableParts.HeaderRow | PivotTableParts.FirstHeaderCell;
                                FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                    pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);

                                if (pivotEngine[i, j].Value != null)
                                {
                                    pivotSheet[sheetRow, sheetColumn].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    pivotSheet[sheetRow, sheetColumn].CellStyle.IndentLevel = j;
                                }
                            }
                            else if ((pivotEngine[i, j].CellType & PivotCellType.ExpanderCell) != 0)
                            {
                                if (makeExpanderCount)
                                {
                                    rowExpanderCount = -1;
                                    makeExpanderCount = false;
                                }
                                rowExpanderCount++;
                                sheetRow++;
                                sheetColumn = pivotTable.Location.Column;
                                if (pivotEngine[i, j].ParentCell == null)
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.FirstColumn | GetRowHeading(rowExpanderCount);
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                    previousRowStyle = pivotPartStyle;
                                    previousRowStyle &= ~PivotTableParts.FirstColumn;

                                }
                                pivotSheet[sheetRow, sheetColumn].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                pivotSheet[sheetRow, sheetColumn].CellStyle.IndentLevel = j;
                                int k = i;
                                while ((pivotEngine[k, j].CellType & PivotCellType.TotalCell) == 0)
                                {
                                    k++;
                                }
                                int currentPlaceOfColumn = sheetColumn + 1;
                                for (int m = j; m < pivotEngine.ColumnCount; m++)
                                {
                                    if (pivotEngine[k, m] != null)
                                    {
                                        if ((pivotEngine[k, m].CellType & PivotCellType.ValueCell) != 0 && m > pivotTable.RowFields.Count - 1)
                                        {
                                            if (pivotEngine[k, m].ParentCell == null && pivotEngine[k, m].Value != null)//If column fields is zero , then x added. (check with 4 row fields).
                                            {
                                                if (pivotEngine[k, m].Value.ToString() != "0")
                                                {
                                                    pivotSheet[sheetRow, currentPlaceOfColumn].Value2 = pivotEngine[k, m].Value;
                                                    pivotPartStyle = PivotTableParts.WholeTable | GetRowHeading(rowExpanderCount);
                                                    if ((pivotEngine[k, m].CellType & PivotCellType.TotalCell) != 0)
                                                        pivotPartStyle |= PivotTableParts.SubtotalColumn1;
                                                    if (!lstTotalCell.Contains(currentPlaceOfColumn))
                                                        pivotPartStyle &= ~PivotTableParts.SubtotalColumn1;
                                                    FillPivotValue(sheetRow - minusSheetRow, currentPlaceOfColumn - minusSheetColumn,
                                   pivotSheet[sheetRow, currentPlaceOfColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                                    previousRowStyle = pivotPartStyle;
                                                }
                                                else
                                                {
                                                    pivotPartStyle = PivotTableParts.WholeTable | GetRowHeading(rowExpanderCount);
                                                    if ((pivotEngine[k, m].CellType & PivotCellType.TotalCell) != 0)
                                                        pivotPartStyle |= PivotTableParts.SubtotalColumn1;
                                                    if (!lstTotalCell.Contains(currentPlaceOfColumn))
                                                        pivotPartStyle &= ~PivotTableParts.SubtotalColumn1;
                                                    FillPivotValue(sheetRow - minusSheetRow, currentPlaceOfColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, currentPlaceOfColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                                }
                                            }
                                            currentPlaceOfColumn++;
                                        }
                                    }
                                }
                            }
                            else if (((pivotEngine[i, j].CellType & PivotCellType.RowHeaderCell) != 0) && (pivotEngine[i, j].CellType & PivotCellType.TotalCell) == 0)
                            {
                                sheetRow++;
                                sheetColumn = pivotTable.Location.Column;
                                pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                  pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, PivotTableParts.WholeTable | PivotTableParts.FirstColumn);
                                previousRowStyle = PivotTableParts.WholeTable ;
                                makeExpanderCount = true;
                                if (pivotEngine[i, j].Value != null)
                                {
                                    pivotSheet[sheetRow, sheetColumn].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    pivotSheet[sheetRow, sheetColumn].CellStyle.IndentLevel = j;
                                }
                                if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0) //handled grandtotal of row here.
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 =ReplaceDelimiter ( pivotSheet[sheetRow, sheetColumn].Value2,delimiter ," ")  + "Total";
                                    pivotPartStyle = PivotTableParts.WholeTable;
                                    if (j == 0)
                                        pivotPartStyle |= PivotTableParts.FirstColumn;
                                    pivotPartStyle |= PivotTableParts.GrandTotalRow;
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                  pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                    previousRowStyle = PivotTableParts .WholeTable | PivotTableParts.GrandTotalRow;
                                    j = pivotTable.RowFields.Count - 1;
                                    pivotSheet[sheetRow, sheetColumn].CellStyle.IndentLevel = 0;
                                }
                            }
                            else if ((pivotEngine[i, j].CellType & PivotCellType.ValueCell) != 0)
                            {
                                if (j > pivotTable.RowFields.Count - 1)//to avoid values  in row header part of pivot engine
                                {
                                    sheetColumn++;
                                    if (pivotTable.RowFields.Count == 0)
                                    {
                                        if (pivotEngine[i, j].Value != null)
                                            if (pivotEngine[i, j].Value.ToString() != "0")
                                            {
                                                pivotSheet[sheetRow, sheetColumn - 1].Value2 = pivotEngine[i, j].Value;
                                                FillPivotValue(sheetRow - minusSheetRow, sheetColumn - (1 + minusSheetColumn),
                   pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousRowStyle);

                                            }
                                    }
                                    else
                                    {
                                        if (pivotEngine[i, j].Value != null)
                                        {
                                            if (pivotEngine[i, j].Value.ToString() != "0")
                                            {
                                                pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                                if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)
                                                {

                                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                     pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousRowStyle);
                                                }
                                                else
                                                {
                                                    if((pivotEngine[i, j].CellType & PivotCellType.TotalCell )!=0)
                                                        previousRowStyle |= PivotTableParts.SubtotalColumn1;
                                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                   pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousRowStyle);
                                                    previousRowStyle &= ~PivotTableParts.SubtotalColumn1;
                                                }
                                            }
                                            else
                                            {
                                                if ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) != 0)
                                                    previousRowStyle |= PivotTableParts.SubtotalColumn1;
                                                FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousRowStyle);
                                                previousRowStyle &= ~PivotTableParts.SubtotalColumn1;
                                            }
                                        }
                                        else
                                        {
                                            FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousRowStyle);
                                        }
                                    }
                                }
                            }
                            else if ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) != 0)
                            {
                                j = pivotEngine.ColumnCount; indentLevel = 0;
                            }
                            if (sheetColumn > maxColumnCount)
                            {
                                maxColumnCount = sheetColumn;
                                if (pivotTable.RowFields.Count == 1 && pivotTable.DataFields.Count == 0 && pivotTable.ColumnFields.Count == 0)
                                    maxColumnCount = 1;
                            }
                            if (sheetRow > maxRow)
                                maxRow = sheetRow;
                        }
                        else //Column header cells
                        {

                            sheetRow = i + adjustRow;//i=0;adjustrow=2;
                            sheetColumn = j + adjustColumn + sheetRowFieldCount;
                            if ((pivotEngine[i, j].CellType & PivotCellType.ValueCell) == 0 && (pivotEngine[i, j + 1] != null))
                                maxRow = sheetRow;
                            if (sheetColumn > maxColumnCount)
                                maxColumnCount = sheetColumn;
                            if ((pivotEngine[i, j].CellType & PivotCellType.RowHeaderCell) != 0 && (pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)//For filling the datafields name. check with 2 column fields, one datafields.
                            {
                                if ((pivotEngine[i, j + 1] != null) && pivotTable.DataFields.Count == 1)
                                {
                                    if ((pivotEngine[i, j + 1].CellType & PivotCellType.ValueCell) != 0)
                                    {
                                        pivotSheet[sheetRow, sheetColumn].Value2 = pivotTable.DataFields[0].Name;
                                        pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.FirstColumn | PivotTableParts.FirstHeaderCell;
                                        FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                  pivotTable.DataFields[0].Name, pivotEngine[i, j].CellType, layOut, pivotPartStyle);

                                    }
                                }
                            }
                            if (pivotEngine[i, j].ParentCell == null && pivotEngine[i, j].Value != null)
                            {
                                if ((pivotEngine[i, j].CellType & PivotCellType.TopLeftCell) != 0)
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 = pivotSheet[sheetRow, sheetColumn].Value2.ToString() + pivotEngine[i, j].Value;
                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.FirstColumn | PivotTableParts.FirstHeaderCell;
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                          pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);

                                    if (pivotEngine[i, j].Value != null)
                                    {
                                        checkNextExpanderValue = pivotEngine[i, j].Value.ToString();
                                        pivotTable.FirstDataCol = 0;
                                    }
                                }
                                else if ((pivotEngine[i, j].CellType & PivotCellType.ExpanderCell) != 0)
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                    checkNextExpanderValue = pivotEngine[i, j].Value.ToString();//for not filling the same expander value to its child field (ex : try with 2 row field and two datafields [may be correct sample for this said example])
                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow | GetColumnHeading(columnExpanderCount);
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                              pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                    previousColumnStyle = pivotPartStyle;
                                }
                                else if ((pivotEngine[i, j].CellType & PivotCellType.HeaderCell) != 0 && (pivotEngine[i, j].Value.ToString() != checkNextExpanderValue ) && ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) == 0))
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow | GetColumnHeading(columnExpanderCount);
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                    previousColumnStyle = pivotPartStyle;
                                }
                                else if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) != 0 && ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) == 0) && ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) == 0))
                                {
                                    pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow | GetColumnHeading(columnExpanderCount);
                                    FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                    previousColumnStyle = pivotPartStyle;
                                    columnExpanderCount++;
                                }
                                else if ((((pivotEngine[i, j].CellType & PivotCellType.TotalCell) != 0) || ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)) && pivotTable.ColumnGrand == true)
                                {
                                    if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) == 0)
                                    {
                                        int currentSheetColumnLoop = sheetColumn;//new variable for put the datafield to values (ex: 1 row , 2 column ,2 datafields)
                                        int currentPivotEngineColumn = j;
                                        if (pivotEngine[i, j].CellRange != null)
                                        {
                                            int tempValue = sheetRow;
                                            for (int rightrange = pivotEngine[i, j].CellRange.Left; rightrange <= pivotEngine[i, j].CellRange.Right; rightrange++)
                                            {
                                                CoveredCellRange cellRange = pivotEngine[i, j].CellRange;
                                                if (pivotTable.DataFields.Count >= 2 && ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0))
                                                {
                                                    pivotSheet[sheetRow, currentSheetColumnLoop].Value2 = "Total";
                                                }
                                                else
                                                {
                                                    pivotSheet[sheetRow, currentSheetColumnLoop].Value2 = pivotEngine[i, j].Value;
                                                }
                                                if (pivotTable.DataFields.Count > 1)
                                                {
                                                    pivotSheet[sheetRow, currentSheetColumnLoop].Value2 = pivotSheet[sheetRow, currentSheetColumnLoop].Value2.ToString() + " " + pivotEngine[pivotEngine[i, j].CellRange.Bottom + 1, currentPivotEngineColumn + sheetRowFieldCount].Value.ToString();
                                                }
                                                else if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0 || ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) != 0)) // check 1 row , 2 column , 1 data field
                                                {
                                                    pivotSheet[sheetRow, currentSheetColumnLoop].Value2 = ReplaceDelimiter(pivotEngine[i, j].Value, delimiter, " ") + "Total";// pivotEngine[i, j].Value.ToString().Replace(delimiter, " ") + "Total";
                                                }
                                                for (int topRange = cellRange.Top; topRange <= cellRange.Bottom; topRange++)
                                                {
                                                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow| GetColumnHeading(columnExpanderCount);
                                                    pivotPartStyle |= PivotTableParts.SubtotalColumn1;
                                                    if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)
                                                        pivotPartStyle &= ~PivotTableParts.SubtotalColumn1;
                                                    if(topRange == cellRange .Top )
                                                    FillPivotValue(tempValue++ - minusSheetRow, currentSheetColumnLoop - minusSheetColumn,
                                     pivotSheet[sheetRow, currentSheetColumnLoop].Value2, pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                                    else
                                                        FillPivotValue(tempValue++ - minusSheetRow, currentSheetColumnLoop - minusSheetColumn,
                                     "", pivotEngine[i, j].CellType, layOut, pivotPartStyle);
                                                    if (!lstTotalCell.Contains(currentSheetColumnLoop))
                                                        lstTotalCell.Add(currentSheetColumnLoop);
                                                }
                                                previousColumnStyle = pivotPartStyle;
                                                currentSheetColumnLoop++;
                                                currentPivotEngineColumn++;
                                            }
                                        }
                                        else
                                        {
                                            pivotSheet[sheetRow, sheetColumn].Value2 = pivotEngine[i, j].Value;
                                            if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0 && (pivotEngine[i, j].CellType & PivotCellType.ValueCell) == 0)
                                                pivotSheet[sheetRow, sheetColumn].Value2 =ReplaceDelimiter ( pivotSheet[sheetRow, sheetColumn].Value2,delimiter ," ") + "Total";
                                            pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow;
                                            FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                                 pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousColumnStyle);
                                        }
                                    }
                                }
                            }
                            else
                            {
                               // FillPivotValue(sheetRow - minusSheetRow, sheetColumn - minusSheetColumn,
                               //pivotSheet[sheetRow, sheetColumn].Value2, pivotEngine[i, j].CellType, layOut, previousColumnStyle);
                            }
                        }
                    }
                }
            }
            if (pivotEngine.RowCount == 1 && pivotEngine.ColumnCount == 1)
            {
                if (pivotTable.ColumnGrand && pivotTable.RowGrand)
                {
                    pivotSheet[maxRow++, maxColumnCount].Value2 = "Grand Total";
                    pivotSheet[maxRow, maxColumnCount - 1].Value2 = "Grand Total";
                }
            }
            int insertFirstValueRow = pivotTable.Location.Row;
            int insertFirstColumn = pivotTable.Location.Column;
            for (int firstValueRow = 0; firstValueRow <= layOut.maxColumnCount; firstValueRow++)
            {
                if (firstValueRow == 0)
                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow | PivotTableParts.FirstColumn;
                else
                    pivotPartStyle = PivotTableParts.WholeTable | PivotTableParts.HeaderRow;
                FillPivotValue(insertFirstValueRow - minusSheetRow, insertFirstColumn++ - minusSheetColumn,
                                        " ", PivotCellType.TopLeftCell, layOut, pivotPartStyle);
            }
           
            //if (pivotTable.ColumnGrand == false)
            //    maxColumnCount = maxColumnCount - 1;
            if (pivotTable.RowFields.Count > 0)
            {
                RowLabelsRow = pivotTable.ColumnFields.Count + RowLabelsRow;
                if (pivotTable.DataFields.Count > 1 && pivotTable.ColumnFields.Count > 0)
                    RowLabelsRow++;
                pivotSheet[RowLabelsRow, pivotTable.Location.Column].Value = "Row Labels";
                FillPivotValue(RowLabelsRow - minusSheetRow, pivotTable.Location.Column - minusSheetColumn,
                                        " ", PivotCellType.TopLeftCell, layOut, PivotTableParts.HeaderRow|PivotTableParts .FirstColumn | PivotTableParts.WholeTable);
            }
            if (pivotTable.RowFields.Count == 0 && pivotTable.DataFields.Count > 1)
                pivotTable.FirstDataCol = 0;
            if (pivotTable.ColumnFields.Count > 0 && pivotTable.RowFields.Count > 0 && pivotTable.DataFields.Count == 1)
                pivotSheet[pivotTable.Location.Row, pivotTable.Location.Column].Value = pivotTable.DataFields[0].Name;
            if (maxRow > 0 && maxColumnCount > 0)
            {
                pivotTable.EndLocation = pivotSheet.Range[maxRow, maxColumnCount];
            }
            SetSubtotalColumn(layOut);
            SetExtendedFormat(layOut);
            PivotTableStyleRenderer styleRender = new PivotTableStyleRenderer();
            styleRender.DrawPivotBorder(layOut,pivotTable .BuiltInStyle);

            pivotTable.PivotLayout = layOut;
            //   pivotTable.Cache.IsRefreshOnLoad = true;
            if (pivotTable.Location.Row <= maxRow && pivotTable.Location.Column <= maxColumnCount)
                AutoFitPivotTable((pivotSheet[pivotTable.Location.Row, pivotTable.Location.Column, maxRow, maxColumnCount]), pivotTable);
        }

        /// <summary>
        /// Replace the delimiter from grand total
        /// </summary>
        /// <param name="replacableValue"></param>
        /// <param name="delimiter"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public object ReplaceDelimiter(object replacableValue,string delimiter, string newValue)
        {
            object replacedValue = replacableValue.ToString().Replace(delimiter ,newValue);
            return replacedValue;
        }
        /// <summary>
        /// AutoFit the pivot table
        /// </summary>
        /// <param name="range"></param>
        /// <param name="pivotTable"></param>
        public void AutoFitPivotTable(IRange range,IPivotTable pivotTable)
        {
            int columnHeadersCount = 0;
            if (pivotTable.DataFields.Count <= 1)
                columnHeadersCount = pivotTable.ColumnFields.Count+1;
            else
                columnHeadersCount = pivotTable.ColumnFields.Count + 2;
            string biggestString = string .Empty; int biggestStringLength = 0;
            IRange biggestStringRange= range.Worksheet[range.Row, range .Column];
            IRange currentRange;
            for (int firstColumn = range.Column; firstColumn <= range.LastColumn; firstColumn++)
            {
                biggestString = string.Empty;
                for (int firstRow = range.Row; firstRow <= range.LastRow; firstRow++)
                {
                    string tempString = range.Worksheet[firstRow, firstColumn].Value;
                    if (tempString.Length > biggestString.Length)
                    {
                        biggestStringRange = range.Worksheet[firstRow, firstColumn];
                        biggestString = tempString;
                    }
                }
              biggestStringRange.AutofitColumns();
              double width = biggestStringRange.ColumnWidth;
              if (biggestStringRange.IndentLevel > 0)
              {
                  if (range.Row + columnHeadersCount < biggestStringRange.Row)
                      width = width + (biggestStringRange.IndentLevel * 1) + 1.85;
                  else
                      width = width + 1.85;
              }
              biggestStringRange.ColumnWidth = width;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="value"></param>
        /// <param name="cellType"></param>
        /// <returns></returns>
        private void FillPivotValue(int rowIndex, int columnIndex,
            object value, PivotCellType cellType, PivotTableLayout layOut, PivotTableParts partStyle)
        {
            PivotValueCollections pivotValue = new PivotValueCollections();
            pivotValue.Value = value.ToString();
            PivotTableStyleRenderer styleRender = new PivotTableStyleRenderer(this.pivotTableImple.Worksheet);
            //pivotValue.XF = styleRender.ApplyStyles(pT.BuiltInStyle, partStyle);
            pivotValue.PivotTablePartStyle = partStyle;
            // pivotValue.XF = new ExtendedFormatImpl(this.pT.Worksheet.Application, this.pT.Workbook);
            layOut[rowIndex, columnIndex] = pivotValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layout"></param>
        private void SetSubtotalColumn(PivotTableLayout layout)
        {
            bool bFirstSubtotalRow = true;
            PivotTableParts previousSubtotalColumn = PivotTableParts.SubtotalColumn2;
            int FirstSubtotalRowIndex = 0;
            List<int> FirstSubtotalColIndex = new List<int>();
            for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
            {
                for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                {
                    if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                    {
                        if (bFirstSubtotalRow)
                        {
                            FirstSubtotalRowIndex = rowIndex;
                            bFirstSubtotalRow = false;
                        }
                        if (FirstSubtotalRowIndex == rowIndex)
                        {
                            if (layout[rowIndex, colIndex].Value != null)
                            FirstSubtotalColIndex.Add(colIndex);
                        }
                        if (FirstSubtotalRowIndex != rowIndex)
                        {
                            if (!FirstSubtotalColIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].PivotTablePartStyle &= ~PivotTableParts.SubtotalColumn1;
                                layout[rowIndex, colIndex].PivotTablePartStyle |= previousSubtotalColumn;
                                if (previousSubtotalColumn == PivotTableParts.SubtotalColumn2)
                                    previousSubtotalColumn = PivotTableParts.SubtotalColumn3;
                                else
                                    previousSubtotalColumn = PivotTableParts.SubtotalColumn2;
                            }
                        }
                        else if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                        {
                            if (!FirstSubtotalColIndex.Contains(colIndex))
                            layout[rowIndex, colIndex].PivotTablePartStyle &= ~PivotTableParts.SubtotalColumn1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layout"></param>
        private void SetExtendedFormat(PivotTableLayout layout)
        {
            for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
            {
                for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                {
                    {
                        PivotTableStyleRenderer styleRender = new PivotTableStyleRenderer(this.pivotTableImple.Worksheet);
                        layout[rowIndex ,colIndex ].XF = styleRender.ApplyStyles(pivotTableImple.BuiltInStyle, layout[rowIndex ,colIndex ].PivotTablePartStyle);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expanderCount"></param>
        /// <returns></returns>
        private PivotTableParts GetRowHeading(int expanderCount)
        {
            if (expanderCount % 3 == 0)
                return PivotTableParts.RowSubHeading1;
            else if (expanderCount % 3 == 1)
                return PivotTableParts.RowSubHeading2;
            else if (expanderCount % 3 == 2)
                return PivotTableParts.RowSubHeading3;
            return PivotTableParts.RowSubHeading1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expanderCount"></param>
        /// <returns></returns>
        private PivotTableParts GetColumnHeading(int expanderCount)
        {
            if (expanderCount % 3 == 0)
                return PivotTableParts.ColumnSubHeading1;
            else if (expanderCount % 3 == 1)
                return PivotTableParts.ColumnSubHeading2;
            else if (expanderCount % 3 == 2)
                return PivotTableParts.ColumnSubHeading3;
            return PivotTableParts.ColumnSubHeading1;
        }

        /// <summary>
        /// Pivot Engine Comparer class.
        /// </summary>
        public class CustomComparer : IComparer
        {
            public int Compare(object a, object b)
            {
                if (a == b)
                {
                    return 0;
                }
                if (a == null)
                {
                    return -1;
                }
                if (b == null)
                {
                    return 1;
                }

                double tempDouble1;
                double tempDouble2;
                bool bDouble1 = double.TryParse(a.ToString(), out tempDouble1);
                bool bDouble2 = double.TryParse(b.ToString(), out tempDouble2);

                DateTime outDateTime1;
                DateTime outDateTime2;
                bool bDateTime1 = DateTime.TryParse(a.ToString(), out outDateTime1);
                bool bDateTime2 = DateTime.TryParse(b.ToString(), out outDateTime2);


                int tempIntValue1;
                int tempIntValue2;
                bool bInt1 = int.TryParse(a.ToString(), out tempIntValue1);
                bool bInt2 = int.TryParse(b.ToString(), out tempIntValue2);
                if (bDouble1 && bDouble2)
                {
                    return tempDouble1.CompareTo(tempDouble2);
                }
                else if (bDateTime1 && bDateTime2)
                {
                    int result = DateTime.Compare(outDateTime1, outDateTime2);
                    return result > 0 ? 1 : -1;
                }
                else if (bInt1 && bInt2)
                {
                    if (Int32.Parse(b.ToString()) == Int32.Parse(a.ToString()))
                        return 0;
                    return Int32.Parse(b.ToString()) > Int32.Parse(a.ToString()) ? 1 : -1;
                }
                else
                {
                    IComparable comparable = a as IComparable;
                    if (comparable == null)
                    {
                        throw new ArgumentException("Argument_ImplementIComparable");
                    }

                    return comparable.CompareTo(b);
                }
                
            }
        }
    }
}
