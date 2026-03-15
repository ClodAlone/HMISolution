#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System.Collections.Generic;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.WP.Implementation.Extensions;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders.PivotTables
{
    /// <summary>
    /// This class is repsonsible for parse hte pivot table part
    /// </summary>
    class PivotTableParser
    {
        #region Constants
        /// <summary>
        /// Index of the data m_bRefreshOnLoad.
        /// </summary>
        private const int DataFieldsIndex = -2;
        #endregion

        #region initializer
        public PivotTableParser()
        {
        }
        #endregion

        #region Methods
        public static void ParsePivotTable(XmlReader reader, PivotTableImpl pivotTable)
        {
            PivotTableOptions settings = pivotTable.Options as PivotTableOptions;
            WorksheetImpl sheetImpl = pivotTable.Worksheet as WorksheetImpl;
            WorkbookImpl bookImpl = sheetImpl.Workbook as WorkbookImpl;
            if (bookImpl.Options== ExcelParseOptions.DoNotParsePivotTable)
            {
                if (reader.MoveToAttribute(PivotTable.PivotCacheId))
                    pivotTable.CacheIndex = XmlConvert.ToInt32(reader.Value);
                reader.MoveToElement();
                sheetImpl.PreservePivotTables.Add(ShapeParser.ReadNodeAsStream(reader));
                return;
            }
            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                pivotTable.Name = reader.Value;

            if (reader.MoveToAttribute(PivotTable.PivotCacheId))
                pivotTable.CacheIndex = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyNumberFormats))
                settings.IsNumberAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyBorderFormats))
                settings.IsBorderAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyFontFormats))
                settings.IsFontAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyPatternFormats))
                settings.IsPatternAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyAlignmentFormats))
                settings.IsAlignAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ApplyWidthHeightFormats))
                settings.IsWHAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.AsteriskTotalAttribute))
                settings.ShowAsteriskTotals = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ColumnGrandTotal))
                pivotTable.ShowColumnGrand = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ColumnHeaderCaption))
                settings.ColumnHeaderCaption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.RowHeaderCaption))
                settings.RowHeaderCaption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.CreatedVersion))
                settings.CreatedVersion = XmlConvert.ToByte(reader.Value);

            if (reader.MoveToAttribute(PivotTable.UpdatedVersion))
                settings.UpdatedVersion = XmlConvert.ToByte(reader.Value);

            if (reader.MoveToAttribute(PivotTable.MinRefreshableVersion))
                settings.MiniRefreshVersion = XmlConvert.ToByte(reader.Value);

            if (reader.MoveToAttribute(PivotTable.CustomListSort))
                settings.ShowCustomSortList = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DataCaption))
                settings.DataCaption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.DataOnRows))
                pivotTable.ShowDataFieldInRow = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DataPosition))
                settings.DataPosition = XmlConvert.ToUInt16(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DisableFieldList))
                settings.ShowFieldList = !XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.AllowEditData))
                settings.IsDataEditable = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.EnableDrillDown))
                pivotTable.EnableDrilldown = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.EnableFieldProerties))
                settings.EnableFieldProperties = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.EnableWizard))
                pivotTable.EnableWizard = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ErrorCaption))
                pivotTable.ErrorString = reader.Value;

            if (reader.MoveToAttribute(PivotTable.ShowHeaders))
                settings.DisplayFieldCaptions = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowError))
                pivotTable.DisplayErrorString = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.MissingCapiton))
                pivotTable.NullString = reader.Value;

            if (reader.MoveToAttribute(PivotTable.ShowMissing))
                pivotTable.DisplayNullString = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowCalcMbrs))
                settings.ShowCalcMembers = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ItemPrintTitles))
                pivotTable.RepeatItemsOnEachPrintedPage = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.Indent))
                settings.Indent = XmlConvert.ToUInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.RowGrandTotal))
                pivotTable.RowGrand = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ColumnGrandTotal))
                pivotTable.ColumnGrand = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowGridDropZone))
                settings.ShowGridDropZone = XmlConvert.ToBoolean(reader.Value);

            {
                bool? bOutline = null;
                bool? bCompact = null;
                if (reader.MoveToAttribute(PivotTable.Outline))
                    bOutline = XmlConvert.ToBoolean(reader.Value);

                //if (reader.MoveToAttribute(PivotTable.OutlineData))
                //    settings.OutlineData = XmlConvert.ToBoolean(reader.Value);

                if (reader.MoveToAttribute(PivotTable.Compact))
                    bCompact = XmlConvert.ToBoolean(reader.Value);

                //if (reader.MoveToAttribute(PivotTable.CompactData))
                //    settings.IsCompactData = XmlConvert.ToBoolean(reader.Value);



                if (bOutline == null)
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;
                else if (bCompact == null)
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Compact;
                else
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Outline;

            }

            if (reader.MoveToAttribute(PivotTable.PageOverThenDown))
            {
                bool isPageOver = XmlConvert.ToBoolean(reader.Value);
                if (isPageOver)
                    settings.PageFieldsOrder = PivotPageAreaFieldsOrder.OverThenDown;
            }

            if (reader.MoveToAttribute(PivotTable.PreserveFormatting))
                settings.PreserveFormatting = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.UseAutoFormatting))
                settings.IsAutoFormat = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowDataTips))
                settings.ShowTooltips = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.FieldPrintTitles))
                settings.PrintTitles = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.AsteriskTotalAttribute))
                settings.ShowAsteriskTotals = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.MergeItem))
                settings.MergeLabels = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.PageWrap))
                settings.PageFieldWrapCount = XmlConvert.ToInt32(reader.Value);

            if(reader.MoveToAttribute(PivotTable.MultipleFieldFilters))
                settings.IsMultiFieldFilter=XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DefaultAutoSort))
                settings.IsDefaultAutoSort = XmlConvert.ToBoolean(reader.Value);

            reader.Read();

            ParseLocation(reader, pivotTable);
            ParsePivotFields(reader, pivotTable);
            ParseRowFields(reader, pivotTable);
            ParseRowItems(reader, pivotTable);
            ParseColumnFields(reader, pivotTable);
            ParseColumnItems(reader, pivotTable);
            ParsePageFields(reader, pivotTable);
            ParseDataFields(reader, pivotTable);
            ParseCustomFormats(reader, pivotTable);
            ParseConditionalFormats(reader, pivotTable);
            ParseChartFormats(reader, pivotTable);
            ParsePivotHierarchies(reader, pivotTable);
            ParsePivotStyle(reader, pivotTable);
            ParseRowHierarchies(reader, pivotTable);
            ParseFilters(reader, pivotTable);
            ParseTableDefinitionExtensionList(reader ,pivotTable );
            int lastRow = 0;
            int lastColumn = 0;


        }
        /// <summary>
        /// Parse Extern List of Pivot table Definition
        /// </summary>
        /// <param name="reader">Xml reader to extract from.</param>
        /// <param name="pivotTable"></param>
        private static void ParseTableDefinitionExtensionList(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != Excel2007Serializator .Extensionlist)
                return;

            Stream stream = ShapeParser.ReadNodeAsStream(reader);
            pivotTable.PreservedElements.Add(Excel2007Serializator .Extensionlist , stream);
        }
        /// <summary>
        /// Parse chart formats of the pivot chart that is associated with this PivotTable.
        /// </summary>
        /// <param name="reader">Xml reader to extract from.</param>
        /// <param name="pivotTable"></param>
        private static void ParseChartFormats(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.ChartFormats)
                return;

            Stream stream = ShapeParser.ReadNodeAsStream(reader);
            pivotTable.PreservedElements.Add(PivotTable.ChartFormats, stream);
        }

        private static void ParseCustomFormats(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.CustomFormats)
                return;

            Stream stream = ShapeParser.ReadNodeAsStream(reader);
            pivotTable.PreservedElements.Add(PivotTable.CustomFormats, stream);
        }

        /// <summary>
        /// Parsing pivot filters of pivot tables
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pivotTable"></param>
        private static void ParseFilters(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.Filters)
                return;

            PivotTableFilters pivotFilters = new PivotTableFilters();
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.Filter)
                {
                    PivotTableFilter pivotFilter = new PivotTableFilter();
                    PivotValueLableFilter valueFilter = new PivotValueLableFilter();
                    ParseFilter(reader, pivotFilter, valueFilter );

                    
                    if(pivotFilter.Value1 != null )
                    valueFilter.Value1 = pivotFilter.Value1;
                    if(pivotFilter .Value2 != null )
                    valueFilter.Value2 = pivotFilter.Value2;
                    valueFilter.Type = pivotFilter.Type;
                    valueFilter.DataField = pivotTable.Fields[0];
                    PivotTableFields fields = pivotTable.Fields;
                    foreach (PivotFieldImpl field in fields)
                    {
                        if (fields.IndexOf(field) == pivotFilter.Field)
                        {
                            PivotFilterCollections filterCollections = field.PivotFilters as PivotFilterCollections;
                            filterCollections.ValueFilter = valueFilter;
                        }
                    }
                    pivotFilters.Add(pivotFilter);
                }
            }
            pivotTable.Filters = pivotFilters;
            reader.Read();
        }

        /// <summary>
        /// Parsing pivot filter of filter collections
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="pivotTable"></param>
        private static void ParseFilter(XmlReader reader, PivotTableFilter pivotFilter, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotFilter  == null)
                throw new ArgumentNullException("pivotFilters");

            if (reader.LocalName != PivotTable.Filter)
                return;
            
            if (reader.MoveToAttribute(PivotTable.Description))
                pivotFilter.DescriptionAttribute = Convert.ToString(reader.Value);
            if (reader.MoveToAttribute(PivotTable.EvalOrderAttribute))
                pivotFilter.EvalOrder = XmlConvert.ToInt32(reader.Value);
            if (reader.MoveToAttribute(PivotTable.CacheFieldIndex))
                pivotFilter.Field = XmlConvert.ToInt32(reader.Value);
            if (reader.MoveToAttribute(PivotTable.PivotFilterId))
                pivotFilter.FilterId = XmlConvert.ToInt16(reader.Value);
            if(reader .MoveToAttribute (PivotTable .MeasureFldAttribute))
                pivotFilter .MeasureFld = XmlConvert .ToInt16 (reader .Value );
            if (reader.MoveToAttribute(PivotTable.Value1))
                pivotFilter.Value1 = Convert.ToString(reader.Value);
            if (reader.MoveToAttribute(PivotTable.Value2))
                pivotFilter.Value2 = Convert.ToString(reader.Value);
            if (reader.MoveToAttribute(PivotTable.TypeAttribute))
                pivotFilter.Type = (PivotFilterType)Enum.Parse(typeof(PivotFilterType2007), reader.Value,true );
           

            reader.Read();
           
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.AutoFilterElement)
                {
                    PivotAutoFilter autoFilter = new PivotAutoFilter();
                    ParseAutoFilter(reader, autoFilter, valueFilter);
                    pivotFilter.Add(autoFilter);
                }
            }

            reader.Read();
         }

        /// <summary>
        /// Parsing autofilter of pivot table
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="autoFilter"></param>
        private static void ParseAutoFilter(XmlReader reader, PivotAutoFilter autoFilter, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (autoFilter == null)
                throw new ArgumentNullException("Filters");

            if (reader.MoveToAttribute(PivotTable .ReferenceAddress))
                autoFilter.FilterRange = Convert.ToString(reader.Value);

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.FilterColumnElement)
                {
                    PivotFilterColumn filterColumn = new PivotFilterColumn();
                    ParseFilterColumn(reader, filterColumn,valueFilter);
                    autoFilter.Add(filterColumn);
                }
            }
            reader.Read();
        }


        /// <summary>
        /// Parsing filter column filter of pivot table filter
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="autoFilter"></param>
        private static void ParseFilterColumn(XmlReader reader, PivotFilterColumn filterColumn, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (filterColumn == null)
                throw new ArgumentNullException("filterColumn");
           
            if (reader.MoveToAttribute(PivotTable.ColumnIdAttribute))
                filterColumn.ColumnId = XmlConvert.ToInt32(reader.Value);
            if (reader.MoveToAttribute(PivotTable.HiddenButtonAttribute))
                filterColumn.HiddenButton = XmlConvert.ToBoolean(reader.Value);
            if (reader.MoveToAttribute(PivotTable.ShowButtonAttribute))
                filterColumn.ShowButton = XmlConvert.ToBoolean(reader.Value);
            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.LocalName)
                {
                    case PivotTable.CustomFiltersElement:
                        {
                            PivotCustomFilters customFilters = new PivotCustomFilters();
                            ParseCustomFilters(reader, customFilters, valueFilter);
                            filterColumn.CustomFilters = customFilters;
                            break;
                        }
                    case PivotTable.Filters:
                        {
                            FilterColumnFilters Filters = new FilterColumnFilters();
                            ParseFilterColumnFilters(reader, Filters, valueFilter);
                            filterColumn.FilterColumnFilter = Filters;
                            break;

                        }
                    case PivotTable.Top10FilterElement:
                        {
                            PivotTop10Filter top10Filter = new PivotTop10Filter();
                            ParseTop10Filter(reader, top10Filter, valueFilter);
                            filterColumn.Top10Filters = top10Filter;
                            break;
                        }
                }
            }
            reader.Read();
        }

        /// <summary>
        /// Parsing custom filter type of pivot table.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="autoFilter"></param>
        private static void ParseCustomFilters(XmlReader reader, PivotCustomFilters customFilters, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (customFilters == null)
                throw new ArgumentNullException("customFilters");

            if (reader.MoveToAttribute(PivotTable.AndAttributeName ))
                customFilters.HasAnd = XmlConvert.ToBoolean(reader.Value);

            reader.Read();
           while (reader .NodeType != XmlNodeType .EndElement )
           {
               PivotCustomFilter customFilter = new PivotCustomFilter();
               ParseCustomFilter (reader ,customFilter,valueFilter );
               customFilters.Add(customFilter);
           }
            reader.Read();
        }
        /// <summary>
        /// Parsing custom filter of custom filter collection of pivot filter.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="autoFilter"></param>
        private static void ParseCustomFilter(XmlReader reader, PivotCustomFilter customFilter, PivotValueLableFilter valueFilter)
        {
             if (reader == null)
                throw new ArgumentNullException("writer");

            if (customFilter == null)
                throw new ArgumentNullException("customFilters");

            if (reader.MoveToAttribute(PivotTable.OperatorAttribute))
                customFilter.FilterOperator =(FilterOperator2007 )Enum .Parse (typeof (FilterOperator ),reader .Value,true  );
            if (reader .MoveToAttribute (PivotTable .ValAttibuteName ))
                customFilter .Value = reader .Value ;
            string Value = customFilter.Value;
            if(Value.Contains ("*"))
            {
                char[] removeLetter = new char[] { '*' };
                string[] words = Value.Split(removeLetter, StringSplitOptions.RemoveEmptyEntries);
                Value = words[0];
            }
            if (valueFilter.Value1 == null)
                valueFilter.Value1 = Value;
            else
                valueFilter.Value2 = Value;
           reader.Read();
        }

        /// <summary>
        /// Parsing filter column filter of pivot filter
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnFilters"></param>
        private static void ParseFilterColumnFilters(XmlReader reader, FilterColumnFilters columnFilters, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (columnFilters == null)
                throw new ArgumentNullException("filters");

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.Filter)
                {
                    if(reader .MoveToAttribute (PivotTable .ValAttibuteName))
                    columnFilters.Add (reader .Value );
                    if (valueFilter.Value1 == null)
                        valueFilter.Value1 = reader .Value;
                    reader.Read();
                }
            }

            reader.Read();
        }

        /// <summary>
        /// Parsing Top 10 filter of pivot filter.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="top10Filter"></param>
        private static void ParseTop10Filter(XmlReader reader, PivotTop10Filter top10Filter, PivotValueLableFilter valueFilter)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (top10Filter == null)
                throw new ArgumentNullException("top10Filter");

            if (reader.MoveToAttribute(PivotTable.ValAttibuteName))
                top10Filter.FilterValue = XmlConvert.ToInt32(reader.Value);
            if (reader.MoveToAttribute(PivotTable.FilterValueAttributeName))
                top10Filter.Value = XmlConvert.ToInt32(reader.Value);
            valueFilter.Value1 = top10Filter.Value.ToString ();
            if (reader.MoveToAttribute(PivotTable.PercentAttributeName))
                top10Filter.IsPercent = XmlConvert.ToBoolean(reader.Value);
            if (reader.MoveToAttribute(PivotTable.TopAttributeName ))
                top10Filter.IsTop = XmlConvert.ToBoolean(reader.Value);
            reader.Read();
        }

        /// <summary>
        /// Serializes pivot table location.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize location into.</param>
        /// <param name="pivotTable">Pivot table to serialize location for.</param>
        private static void ParseLocation(XmlReader reader, PivotTableImpl pivotTable)
        {
            string strReference = null;
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.MoveToAttribute(PivotTable.ReferenceAddress))
                strReference = reader.Value;

            if (strReference == null)
                throw new Exception("Reference");

            if (reader.MoveToAttribute(PivotTable.ColumnsPerPage))
                pivotTable.ColumnsPerPage = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.RowsPerPage))
                pivotTable.RowsPerPage = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.FirstHeaderRow))
                pivotTable.FirstHeaderRow = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.FirstDataRow))
                pivotTable.FirstDataRow = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.FirstDataColumn))
                pivotTable.FirstDataCol = XmlConvert.ToInt32(reader.Value);

            WorkbookImpl book = pivotTable.Workbook as WorkbookImpl;
            FormulaUtil formulaUtil = book.DataHolder.Parser.FormulaUtil;
            Ptg[] token = formulaUtil.ParseString(strReference);
            IRangeGetter rangeGetter = token[0] as IRangeGetter;
            IWorksheet sheet = pivotTable.Worksheet;
            pivotTable.Location = rangeGetter.GetRange(book, sheet);
            reader.Read();
        }
        /// <summary>
        /// Parse pivot fields.
        /// </summary>
        /// <param name="reader">XmlReader to Parse fields from.</param>
        /// <param name="pivotTable">Pivot table to serialize fields for.</param>
        private static void ParsePivotFields(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            PivotTableFields fields = pivotTable.InternalFields;

            PivotFieldImpl pivotField;
            PivotCacheImpl cache = pivotTable.Cache;
            PivotCacheFieldsCollection cacheFields = cache.CacheFields;
            reader.Read();
            int iCurrentIndex = 0;
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.PivotField)
                {
                    pivotField = fields[iCurrentIndex];
                    ParsePivotField(reader, pivotField, pivotTable);
                    iCurrentIndex++;

                }
            }
            reader.Read();
        }
        /// <summary>
        /// Serializes single pivot m_bRefreshOnLoad.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize pivot m_bRefreshOnLoad into.</param>
        /// <param name="m_bRefreshOnLoad">Field to serialize.</param>
        private static void ParsePivotField(XmlReader reader, PivotFieldImpl pivotField, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (pivotField == null)
                throw new ArgumentException("pivot Field");

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                pivotField.Name = reader.Value;

            if (reader.MoveToAttribute(PivotTable.SubTotalCaption))
                pivotField.SubTotalName = reader.Value;

            if (reader.MoveToAttribute(PivotTable.AutoShowAttribute))
                pivotField.IsAutoShow = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.AxisAttribute))
                pivotField.Axis = (PivotAxisTypes)Enum.Parse(typeof(PivotAxisTypes2007), reader.Value, false);

            pivotField.Subtotals = ParseSubtotalFlags(reader);

            if (reader.MoveToAttribute(PivotTable.Compact))
                pivotField.Compact = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DragOffAttribute))
                pivotField.CanDragOff = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DragToColAttribute))
                pivotField.CanDragToColumn = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DragToData))
                pivotField.CanDragToData = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DragToPage))
                pivotField.CanDragToPage = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DragToRow))
                pivotField.CanDragToRow = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.HideNewItemAttribute))
                pivotField.ShowNewItemsOnRefresh = !XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.IncludeNewItemFilter))
                pivotField.ShowNewItemsInFilter = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.InsertBlankRow))
                pivotField.ShowBlankRow = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.InsertPageBreak))
                pivotField.ShowPageBreak = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ItemsPerPage))
                pivotField.ItemsPerPage = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.MeasureFilterAttribute))
                pivotField.IsMeasureField = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.MultiItemSelction))
                pivotField.IsMultiSelected = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.Outline))
            {
                pivotField.ShowOutline = XmlConvert.ToBoolean(reader.Value);
                if ( pivotTable.Options.RowLayout == PivotTableRowLayout.Outline || pivotTable.Options.RowLayout == PivotTableRowLayout.Compact )
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;
            }

            if (reader.MoveToAttribute(PivotTable.ShowAllAttribute))
                pivotField.IsShowAllItems = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowDropDownAttribute))
                pivotField.ShowDropDown = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowPropAsCaption))
                pivotField.ShowPropAsCaption = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowPropToolTip))
                pivotField.ShowToolTip = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.SortTypeAttribute))
                pivotField.SortType = (PivotFieldSortType)Enum.Parse(typeof(PivotFieldSortType), reader.Value, true);

            if (reader.MoveToAttribute(PivotTable.UniqueMemberProperty))
                pivotField.Caption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.DataFieldAttribute))
                pivotField.IsDataField = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.NumberFormatAttribute))
                pivotField.NumberFormatIndex = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DataSourceSort))
                pivotField.IsDataSourceSorted = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.AllDrilled))
                pivotField.IsAllDrilled = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.DefaultAttributeDrillState))
                pivotField.IsDefaultDrill = XmlConvert.ToBoolean(reader.Value);

            reader.Read();

            bool hasChild = false;
            hasChild = ParseFieldItems(reader, pivotField);
            if (reader.LocalName == Excel2007Serializator.Extensionlist)
            {
                pivotField.FutureDataStorageStream = ShapeParser.ReadNodeAsStream(reader);
                hasChild = true;
            }
            hasChild |= ParseAutoSortScope(reader, pivotField);

            if (hasChild)
                reader.Read();
        }
        /// <summary>
        /// Parse the Sorting scope of the pivot field
        /// </summary>
        /// <param name="reader">XmlReader to parse from.</param>
        /// <param name="pivotField">pivot field object.</param>
        private static bool ParseAutoSortScope(XmlReader reader, PivotFieldImpl pivotField)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.AutoSortScope)
                return false;

            if (pivotField == null)
                throw new ArgumentException("pivot Field");

            Stream stream = ShapeParser.ReadNodeAsStream(reader);
            pivotField.PreservedAutoSort = stream;
            return true;
        }
        private static bool ParseFieldItems(XmlReader reader, PivotFieldImpl field)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.FieldItems)
                return false;

            if (field == null)
                throw new ArgumentException("pivot Field");

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.FieldItem)
                {
                    ParseFieldItem(reader, field);
                }
                reader.Read();
            }

            reader.Read();
            
            return true;
        }
        private static void ParseFieldItem(XmlReader reader, PivotFieldImpl field)
        {
            if (reader == null)
                throw new ArgumentNullException("writer");

            if (field == null)
                throw new ArgumentException("pivot Field");

            int index = 0;
            bool bAdd = false;
            PivotItemOptions item = new PivotItemOptions();
            if (reader.MoveToAttribute(PivotTable.IndexAttribute))
                index = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ExpandAttribute))
            {
                bAdd = true;
                item.IsExpaned = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.DrillAcrossAtribute))
            {
                bAdd = true;
                item.DrillAcross = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.CalculatedMemberAttribute))
            {
                bAdd = true;
                item.IsCalculatedItem = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.HiddenAttribute))
            {
                bAdd = true;
                item.IsHidden = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.MissingAttribute))
            {
                bAdd = true;
                item.IsMissing = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.ItemCaptionAttribute))
            {
                bAdd = true;
                item.UserCaption = reader.Value;
            }
            if (reader.MoveToAttribute(PivotTable.CharAttribute))
            {
                bAdd = true;
                item.IsChar = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.HideDetailAttribute))
            {
                bAdd = true;
                item.IsHiddenDetails = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.MissingAttribute))
            {
                bAdd = true;
                item.IsMissing = XmlConvert.ToBoolean(reader.Value);
            }
            if (reader.MoveToAttribute(PivotTable.ItemTypeAttribute))
            {
                index = -1;
                bAdd = true;
                string strItemType = reader.Value;
                //check when the attribute value is default which is c# keyword so append 's'
                if (strItemType == PivotTable.FieldSummaryDefault)
                    strItemType += "s";

                item.ItemType = (PivotItemType)Enum.Parse(typeof(PivotItemType2007), strItemType, false);
            }
            if (field.ItemOptions.ContainsKey(index))
                return;
            if (bAdd)
                field.AddItemOption(index, item);
            else
                field.AddItemOption(index);


        }

        /// <summary>
        /// Serializes subtotal value.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="pivotSubtotalTypes">Subtotal type to serialize.</param>
        public static PivotSubtotalTypes ParseSubtotalFlags(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            PivotSubtotalTypes subTotalTypes = PivotSubtotalTypes.Default;

            if (reader.MoveToAttribute(PivotTable.DefaultSubtotal))
            {
                bool isDefault = XmlConvert.ToBoolean(reader.Value);
                if (!isDefault)
                    subTotalTypes = PivotSubtotalTypes.None;
            }

            if (reader.MoveToAttribute(PivotTable.SumSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Sum, reader.Value);

            if (reader.MoveToAttribute(PivotTable.CountASubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Counta, reader.Value);

            if (reader.MoveToAttribute(PivotTable.AverageSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Average, reader.Value);

            if (reader.MoveToAttribute(PivotTable.MaxSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Max, reader.Value);

            if (reader.MoveToAttribute(PivotTable.MinSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Min, reader.Value);

            if (reader.MoveToAttribute(PivotTable.ProductSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Product, reader.Value);

            if (reader.MoveToAttribute(PivotTable.CountSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Count, reader.Value);

            if (reader.MoveToAttribute(PivotTable.StdDevSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Stdev, reader.Value);

            if (reader.MoveToAttribute(PivotTable.StdDevPSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Stdevp, reader.Value);

            if (reader.MoveToAttribute(PivotTable.VarSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Var, reader.Value);

            if (reader.MoveToAttribute(PivotTable.VarPSubtotal))
                subTotalTypes |= GetSubTotalTypes(PivotSubtotalTypes.Varp, reader.Value);

            return subTotalTypes;
        }

        private static void ParseRowFields(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.RowFields)
                return;

            reader.Read();
            //TODO:  full support to parse row fields 
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.Field)
                {
                    int index = -1;
                    if (reader.MoveToAttribute(PivotTable.IndexAttribute))
                    {
                        index = XmlConvert.ToInt32(reader.Value);                        
                    }
                    if (index == -2)
                        pivotTable.ShowDataFieldInRow = true;
                    else
                    {
                        pivotTable.PivotRowFields.Add(pivotTable.Fields[index]);
                        pivotTable.RowFieldsOrder.Add(index);
                    }
                    reader.Read();
                }
            }
            reader.Read();
        }
        private static void ParseRowItems(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.RowItems)
                return;

            pivotTable.RowItemsStream = ShapeParser.ReadNodeAsStream(reader);
        }

        private static void ParseColumnFields(XmlReader reader, PivotTableImpl pivotTable)
        {

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.ColumnFields)
                return;
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.Field)
                {
                    if (reader.MoveToAttribute(PivotTable.IndexAttribute))
                        pivotTable.ColFieldsOrder.Add(Convert.ToInt16(reader.Value));
                  
                }
                reader.Read();
            }
            reader.Read();
        }

        private static void ParseColumnItems(XmlReader reader, PivotTableImpl pivotTable)
        {

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.ColumnItems)
                return;

            pivotTable.ColumnItemsStream = ShapeParser.ReadNodeAsStream(reader);
        }

        private static void ParsePageFields(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.PageFields)
                return;

            PivotTableFields fields = pivotTable.Fields;
            int index = 0;
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.PageField)
                {
                    ParsePageField(reader, pivotTable);
                }
                reader.Read();
            }
            reader.Read();
        }
        private static void ParsePageField(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            PivotTableFields fields = pivotTable.Fields;
            PivotFieldImpl field = null;
            int index = 0;
            string name = null;
            string caption = null;
            

            if (reader.MoveToAttribute(PivotTable.CacheFieldIndex))
                index = XmlConvert.ToInt32(reader.Value);

            field = fields[index];

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                name = reader.Value;

            if (reader.MoveToAttribute(PivotTable.Caption))
                caption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.FieldItem))                  
                field.ItemIndex = XmlConvert.ToInt32(reader.Value);

            pivotTable.PivotPageFields.Add(fields[index]);
            // reader.Read();

        }
        /// <summary>
        /// Serializes data fields.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize fields into.</param>
        /// <param name="pivotTable">PivotTable to serialize data fields for.</param>
        private static void ParseDataFields(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.DataFields)
                return;

            PivotDataFields dataFields = pivotTable.DataFields;
            PivotDataField dataField;
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.DataField)
                {
                    dataField = ParseDataField(reader, pivotTable);
                    dataFields.Add(dataField);
                }
                reader.Read();
            }
            reader.Read();
        }



        /// <summary>
        /// Parses the data Field of the pivot table
        /// </summary>
        /// <param name="writer">XmlWriter to serialize data m_bRefreshOnLoad into.</param>
        /// <param name="m_bRefreshOnLoad">Field to serialize.</param>
        private static PivotDataField ParseDataField(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            bool isEmpty = reader.IsEmptyElement;

            PivotCacheFieldsCollection caches = pivotTable.Cache.CacheFields;
            PivotFieldImpl field;
            PivotDataField dataField;
            string name = null;
            PivotSubtotalTypes subtotals = PivotSubtotalTypes.Default;
            int index = 0;

            if (reader.MoveToAttribute(PivotTable.CacheFieldIndex))
                index = XmlConvert.ToInt32(reader.Value);

            field = new PivotFieldImpl(caches[index], pivotTable);

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                name = reader.Value;

            if (reader.MoveToAttribute(PivotTable.NumberFormatAttribute))
                field.NumberFormatIndex = XmlConvert.ToInt32(reader.Value);

            if (reader.MoveToAttribute(PivotTable.Subtotal))
                subtotals = (PivotSubtotalTypes)Enum.Parse(typeof(PivotSubtotalTypes2007), reader.Value, false);

            dataField = new PivotDataField(name, subtotals, field);

            if (reader.MoveToAttribute(PivotTable.ShowDataAs)) 
                dataField.ShowDataAs = dataField.SetShowData(reader.Value);

            if (reader.MoveToAttribute(PivotTable.BaseField))
            {
                int baseFieldValue = XmlConvert.ToInt32(reader.Value);
                if (baseFieldValue != -1)
                    dataField.BaseField = baseFieldValue;
            }
            if (reader.MoveToAttribute(PivotTable.BaseItem))
                dataField.BaseItem = XmlConvert.ToInt32(reader.Value);

            if (!isEmpty)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case Excel2007Serializator.Extensionlist:
                                ParseExtensionListCollection(reader, dataField);
                                break;
                        }
                    }
                }

                reader.Read();
            }
            return dataField;
        }

        private static void ParseExtensionListCollection(XmlReader reader, PivotDataField dataField)
        {
            if (reader.LocalName != Excel2007Serializator.Extensionlist)
                throw new ArgumentException("reader");

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Excel2007Serializator.Extension:
                            ParseExtensionList(reader, dataField);
                            break;
                    }
                }
            }
            reader.Read();
        }

        private static void ParseExtensionList(XmlReader reader, PivotDataField dataField)
        {
            if (reader.LocalName != Excel2007Serializator.Extension)
                throw new ArgumentException("reader");

            reader.Read();

            if (reader.IsEmptyElement)
            {
                if (reader.MoveToAttribute(PivotTable.PivotShowAs)) ;
                    dataField.SetShowData(reader.Value);

                reader.Read();
            }

        }
        /// <summary>
        /// Parses the condition formats of the pivot table.
        /// </summary>
        /// <param name="reader">XmlReader to parses from.</param>
        /// <param name="pivotTable">extract the pivot table .</param>
        private static void ParseConditionalFormats(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");


            if (reader.LocalName != PivotTable.ConditionalFormats)
                return;

            // throw new NotSupportedException("Pivot Conditional Format");
            //reader.Skip();
            Stream stream = ShapeParser.ReadNodeAsStream(reader);
            pivotTable.PreservedElements.Add(PivotTable.ConditionalFormats, stream);
        }
        /// <summary>
        /// Parses  the OLAP hierarchies associated with the PivotTable.
        /// </summary>
        /// <param name="reader">XmlReader to parses from.</param>
        /// <param name="pivotTable">extract the pivot table .</param>
        private static void ParsePivotHierarchies(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.PivotHierarchies)
                return;

            if (pivotTable.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                pivotTable.PreservedElements.Add(PivotTable.PivotHierarchies, stream);
            }
        }

        private static void ParsePivotStyle(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
            {
                string strStyleName = reader.Value;
                pivotTable.BuiltInStyle = null;

#if  (SILVERLIGHT) || (WINRT) || (WP)
                PivotBuiltInStyles outStyle= PivotBuiltInStyles.PivotStyleMedium1;
                Enum.TryParse<PivotBuiltInStyles>(strStyleName, false,out outStyle);
                pivotTable.BuiltInStyle = outStyle;

#else
                foreach (PivotBuiltInStyles val in Enum.GetValues(typeof(PivotBuiltInStyles )))
                if(val.ToString().Equals(strStyleName))
                {
                    pivotTable.BuiltInStyle = (PivotBuiltInStyles)Enum.Parse(typeof(PivotBuiltInStyles), strStyleName, false);
                    break;
                }

#endif

                if (pivotTable.BuiltInStyle==null)
                {
                    pivotTable.CustomStyleName = reader.Value;
                }
            }

            if (reader.MoveToAttribute(PivotTable.ShowRowHeaders))
                pivotTable.ShowRowHeaderStyle = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowColumnHeaders))
                pivotTable.ShowColHeaderStyle = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowRowStripes))
                pivotTable.ShowRowStripes = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowColumnStripes))
                pivotTable.ShowColStripes = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.ShowLastColumn))
                pivotTable.ShowLastCol = XmlConvert.ToBoolean(reader.Value);

            reader.Read();

        }
        /// <summary>
        /// Parses references to OLAP hierarchies on the row axis of a PivotTable.
        /// </summary>
        /// <param name="reader">XmlReader to parses from.</param>
        /// <param name="pivotTable">extract the pivot table .</param>
        private static void ParseRowHierarchies(XmlReader reader, PivotTableImpl pivotTable)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (reader.LocalName != PivotTable.RowHierarchiesUsage)
                return;

            if (pivotTable.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                pivotTable.PreservedElements.Add(PivotTable.RowHierarchiesUsage, stream);
            }
        }
        /// <summary>
        /// This class represents value-index pair and used to sort pivot m_bRefreshOnLoad values.
        /// </summary>
        private class ComparisonPair : IComparable
        {
            #region Members
            public object Value;
            public int Index;
            public IComparer Comparer = new GeneralComparer();//System.Collections.Comparer.DefaultInvariant;
            #endregion

            #region IComparable Members

            public int CompareTo(object obj)
            {
                ComparisonPair pair = obj as ComparisonPair;
                int result = 1;

                if (pair != null)
                {
                    result = Comparer.Compare(Value, pair.Value);

                    if (result == 0)
                        result = Comparer.Compare(Index, pair.Index);
                }

                return result;
            }

            #endregion

            #region Comparer
            private class GeneralComparer : IComparer
            {
                #region Members
                private IComparer m_comparer = System.Collections.Generic.Comparer<object>.Default;
                #endregion

                #region IComparer Members

                public int Compare(object x, object y)
                {
                    try
                    {
                        return m_comparer.Compare(x, y);
                    }
                    catch
                    {
                        return x.GetHashCode() - y.GetHashCode();
                    }
                }

                #endregion
            }
            #endregion
        }
        #endregion

        #region Helper methods
        public static PivotSubtotalTypes GetSubTotalTypes(PivotSubtotalTypes subtotalTypes, string isEnabled)
        {
            if (XmlConvert.ToBoolean(isEnabled))
                return subtotalTypes;
            return PivotSubtotalTypes.None;
        }
        #endregion
    }
}
