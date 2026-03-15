#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using System.Globalization;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.PivotTables
{
    /// <summary>
    /// This class is repsonsible for parse pivot cache part
    /// </summary>
    class PivotCacheParser
    {

        #region Initializer
        public PivotCacheParser()
        {
        }

        #endregion

        #region Methods
        /// <summary>
        /// Parse pivot cache definition.
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="cache">Cache to parse.</param>
        /// <param name="book">Current workbook.</param>
        /// <parm name="path">path of the parent</parm>
        /// <param name="relationId">Relation id to the pivot cache records.</param>
        public static void ParsePivotCacheDefinition(XmlReader reader, PivotCacheImpl cache, IWorkbook book,
          string path, RelationCollection relations,out string cacheRecordRelationID)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            if (book == null)
                throw new ArgumentNullException("workbook");


            if (reader.LocalName != PivotTable.PivotCacheDefinition)
                new XmlException("Unexpected xml tag.");
            cacheRecordRelationID = null;
            string relationId = null;
            WorkbookImpl bookImpl = book as WorkbookImpl;
            if (bookImpl.Options== ExcelParseOptions.DoNotParsePivotTable)
            {
                bookImpl.PreservesPivotCache.Add(ShapeParser.ReadNodeAsStream(reader));
                return;
            }
            if (reader.MoveToAttribute(Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace))
            {
                relationId = reader.Value;
                cache.RelationId = reader.Value;
                cacheRecordRelationID = reader.Value;
            }



         

            cache.IsBackgroundQuery = ParseBoolAttribute(reader, PivotTable.BackgroundQueryAttribute);

            cache.CreatedVersion = ParseIntAttribute(reader, PivotTable.CreatedVersion);

            cache.EnableRefresh = ParseBoolAttribute(reader, PivotTable.EnableRefreshAttribute);

            cache.IsRefreshOnLoad = ParseBoolAttribute(reader, PivotTable.RefreshOnLoad);

            cache.IsInvalidData = ParseBoolAttribute(reader, PivotTable.InvalidAttribute);

            cache.MinRefreshableVersion = ParseIntAttribute(reader, PivotTable.MinRefreshableVersion);

            cache.IsOptimizedCache = ParseBoolAttribute(reader, PivotTable.OptimizeMemoryAttribute);

            if (reader.MoveToAttribute(PivotTable.RefreshedBy))
                cache.RefreshedBy = reader.Value;

            if (reader.MoveToAttribute(PivotTable.RefreshedDate))
            {
                double refreshDate;
                CultureInfo standardCulture = new CultureInfo("en-US");
                bool isDoubleValue = Double.TryParse(reader.Value,NumberStyles.Any,standardCulture , out refreshDate);
                if (isDoubleValue)
                {
                    cache.RefreshDate =
#if ( WINRT )
 Syncfusion.XlsIO.Implementation.DateTimeExtension.FromOADate(refreshDate);
#else
           DateTime.FromOADate(refreshDate);
#endif
                }
                else
                {
                    cache.RefreshDate = Convert.ToDateTime(reader.Value);
                }
            }

            cache.RefreshedVersion = ParseIntAttribute(reader, PivotTable.RefreshedVersion);

            cache.IsRefreshOnLoad = ParseBoolAttribute(reader, PivotTable.RefreshOnLoad);

            cache.IsSaveData = ParseBoolAttribute(reader, PivotTable.SaveDataAttribute);

            cache.SupportAdvancedDrill = ParseBoolAttribute(reader, PivotTable.SupportAdvancedDrillAttribute);

            cache.IsSupportSubQuery = ParseBoolAttribute(reader, PivotTable.SupportSubQueryAttribute);

            cache.IsUpgradeOnRefresh = ParseBoolAttribute(reader, PivotTable.UpgradeOnRefreshAttribute);

            reader.Read();

            ParseCacheSource(reader, cache, book, relations);
            ParseCacheFields(reader, cache);
            ParseCalculatedItems(reader, cache.CacheFields);
            ParseCacheHierarchies(reader, cache);
            ParseOLAPKPIs(reader, cache);
            ParseOLAPDimensions(reader, cache);
            ParseOLAPMeasureGroups(reader,cache);
            ParseOLAPMaps(reader, cache);
            ParseCacheExtension(reader, cache);
        }
        /// <summary>
        /// Creates the Cache Record Reader from the relation part
        /// </summary>
        /// <param name="relationID">corresponding pivot cache record id </param>
        /// <param name="path">parent path</param>
        /// <param name="book">workbook of the pivot cache</param>
        /// <param name="relations">relations collection of the pivot cache records</param>
        /// <returns>XmlReader of the Pivot cache record</returns>
        private static XmlReader CreateCacheRecordReader(string relationID, string path, WorkbookImpl book, RelationCollection relations, out string itemName)
        {
            if (relationID == null)
                throw new ArgumentNullException("relationId");

            if (book == null)
                throw new ArgumentNullException("workbook");

            if (relations == null)
                throw new ArgumentNullException("relations");

            Relation relation = relations[relationID];
            string strTarget = relation.Target;
            string strPath;
            return book.DataHolder.CreateReader(relation, path, out itemName);
        }
        /// <summary>
        /// Parse cache source.
        /// </summary>
        /// <param name="reader">XmlReader to Parse from.</param>
        /// <param name="cache">Cache to parse.</param>
        /// <parm name="book">Parent book of the cache</parm>
        /// <param name="relations">relation collection of the cache source</param>
        private static void ParseCacheSource(XmlReader reader, PivotCacheImpl cache, IWorkbook book,
          RelationCollection relations)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            if (book == null)
                throw new ArgumentNullException("workbook");

            if (reader.LocalName != PivotTable.CacheSource)
                new XmlException("Unexpected xml tag.");

            if (reader.MoveToAttribute(PivotTable.TypeAttribute))
                cache.SourceType = GetSourceType(reader.Value);

            

            switch (cache.SourceType)
            {
                case ExcelDataSourceType.Worksheet:
                    reader.Read();
                    ParseWorksheetSource(reader, cache, book, relations);
                    break;
                case ExcelDataSourceType.Consolidation:
                    //TODO: ParseConsolidation Ranges
                    ParseConsolidation(reader,cache,book,relations);
                    break;
                case ExcelDataSourceType.ExternalData:
                    ParseExternalSource(reader, cache, book, relations);
                    break;
                case ExcelDataSourceType.ScenarioPivotTable:
                    reader.Read();
                    break;
            }

 			reader.MoveToElement();
            if (reader.LocalName == PivotTable.CacheSource)
                reader.Read();

        }

        /// <summary>
        /// Parsing the consolidation source of the pivottable
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cache"></param>
        /// <param name="book"></param>
        /// <param name="relations"></param>
        private static void ParseConsolidation(XmlReader reader, PivotCacheImpl cache, IWorkbook book, RelationCollection relations)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            if (book == null)
                throw new ArgumentNullException("workbook");

            //TODO: Add support to parse External connection.
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.ConsolidationTypeValue, stream);
            }
        }
        /// <summary>
        /// Parses the pivot cache worksheet source
        /// </summary>
        /// <param name="reader">XmlReader for parse</param>
        /// <param name="cache">cache to preserve the data</param>
        /// <param name="book">parent book of the cache</param>
        /// <param name="relations">relation collection of the worksheet source</param>
        private static void ParseWorksheetSource(XmlReader reader, PivotCacheImpl cache,
            IWorkbook book,
            RelationCollection relations)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            if (book == null)
                throw new ArgumentNullException("workbook");

            if (reader.LocalName != PivotTable.WorksheetSource)
                new XmlException("Unexpected xml tag.");

            WorkbookImpl bookImpl = book as WorkbookImpl;
            IRange sourceRange = null;
            string strRelationId = null;
            string strRange = null;
            string strNamedRange = null;
            string strSheetName = null;

            if (reader.MoveToAttribute(Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace))
                strRelationId = reader.Value;

            if (reader.MoveToAttribute(PivotTable.ReferenceAddress))
                strRange = reader.Value;

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                strNamedRange = reader.Value;

            if (reader.MoveToAttribute(PivotTable.SheetAttribute))
                strSheetName = reader.Value;

            bool isExternalRange = strRelationId != null;
            bool isNamedRange = strNamedRange != null;

            if (!isExternalRange)
            {
                IListObject listTable ;
                if (isNamedRange)
                {
                   
                    IName name = book.Names[strNamedRange];
                    if (name != null)
                        sourceRange = name.RefersToRange;
                    else
                    {
                        for (int j = 0; j < book.Worksheets.Count; j++)
                        {
                            for (int i = 0; i < book.Worksheets[j].ListObjects.Count; i++)
                            {
                                listTable = book.Worksheets[j].ListObjects[i];// sheet.ListObjects.Create(name, location);
                                if (listTable.Name.ToString() == strNamedRange)
                                {
                                    sourceRange = listTable.Location;
                                }
                            }
                        }
                    }
                    cache.RangeName = strNamedRange;
                }
                else
                {
                    FormulaUtil formulaUtil = bookImpl.DataHolder.Parser.FormulaUtil;
                    Ptg[] token = formulaUtil.ParseString(strRange);
                    IRangeGetter rangeGetter = token[0] as IRangeGetter;
                    IWorksheet sheet = book.Worksheets[strSheetName];
                    sourceRange = rangeGetter.GetRange(book, sheet);
                }
            }
            else
            {
                cache.RelationId = strRelationId;
                cache.PreservedExtenalRelation = relations[strRelationId];
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.WorksheetSource, stream);
                //throw new NotImplementedException("External Pivot Cache source");
                //TODO: Parse External worksheet references
            }

            cache.SourceRange = sourceRange;
            if (reader.NodeType != XmlNodeType.EndElement)
                reader.Read();

        }
        /// <summary>
        /// Parses the pivot cache external worksheet source
        /// </summary>
        /// <param name="reader">XmlReader for parse</param>
        /// <param name="cache">cache to preserve the data</param>
        /// <param name="book">parent book of the cache</param>
        /// <param name="relations">relation collection of the worksheet source</param>
        private static void ParseExternalSource(XmlReader reader, PivotCacheImpl cache,
            IWorkbook book,
            RelationCollection relations)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            if (book == null)
                throw new ArgumentNullException("workbook");

            //TODO: Add support to parse External connection.
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.ExternalTypeValue, stream);
            }
        }
        /// <summary>
        /// Parse cache fields.
        /// </summary>
        /// <param name="reader">XmlReader to parse from.</param>
        /// <param name="fieldsCollection">Fields collection.</param>
        private static void ParseCacheFields(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache ");



            if (reader.LocalName != PivotTable.CacheFields)
                throw new XmlException("CacheFields");

            PivotCacheFieldsCollection fieldsCollection = cache.CacheFields;

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == PivotTable.CacheField)
                    {
                        PivotCacheFieldImpl cacheField = new PivotCacheFieldImpl();
                        ParseCacheField(reader, cacheField, cache);
                        fieldsCollection.Add(cacheField);
                    }
                    reader.Read();

                }
            }
            reader.Read();
        }
        /// <summary>
        /// Parse cache Cache Field.
        /// </summary>
        /// <param name="reader">XmlReader to parse from.</param>
        /// <param name="field">Field to serialize.</param>
        private static void ParseCacheField(XmlReader reader, PivotCacheFieldImpl field, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (field == null)
                throw new ArgumentNullException("field");

            if (reader.LocalName != PivotTable.CacheField)
                new XmlException("Unexpected xml tag.");

            if (reader.MoveToAttribute(PivotTable.NameAttribute))
                field.Name = reader.Value;

            field.IsDataBaseField = ParseBoolAttribute(reader, PivotTable.DatabaseFieldAttribute);

            if (reader.MoveToAttribute(PivotTable.FormulaAttribute))
                field.Formula = reader.Value;

            if (reader.MoveToAttribute(PivotTable.NumberFormatAttribute))
                field.NumFormatIndex = XmlConvert.ToInt16(reader.Value);

            if (reader.MoveToAttribute(PivotTable.Caption))
                field.Caption = reader.Value;

            if (reader.MoveToAttribute(PivotTable.HierarchyAttribute))
                field.Hierarchy = ParseIntAttribute(reader, PivotTable.HierarchyAttribute);

            if (reader.MoveToAttribute(PivotTable.HierarchyLevel))
                field.Level = ParseIntAttribute(reader, PivotTable.HierarchyLevel);

            reader.MoveToElement();
            if (reader.IsEmptyElement)
                return;

            reader.Read();

            ParseSharedItems(reader, field);
            ParseFieldGroup(reader, field, cache);



        }
        /// <summary>
        /// Parse the Pivot Cache Field values
        /// </summary>
        /// <param name="reader">XmlReader to parse from.</param>
        /// <param name="field">Field to Pares shared items </param>
        private static void ParseSharedItems(XmlReader reader, PivotCacheFieldImpl field)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (field == null)
                throw new ArgumentNullException("cache field");

            if (reader.LocalName != PivotTable.SharedItems)
                return;

            PivotDataType pivotDataType = new PivotDataType();
            bool bIsBlank = false, bIsNumber = false, bIsDate = false;
            bool bIsString = false, bIsInt = false, bIsLongText = false;

            bIsBlank = ParseBoolAttribute(reader, PivotTable.ContainsBlank);

            if (bIsBlank)
                pivotDataType |= PivotDataType.Blank;

            bIsNumber = ParseBoolAttribute(reader, PivotTable.ContainsNumber);

            if (bIsNumber)
                pivotDataType |= PivotDataType.Number;

            bIsDate = ParseBoolAttribute(reader, PivotTable.ContainsDate);

            if (bIsDate)
                pivotDataType |= PivotDataType.Date;

            bIsString = ParseBoolAttribute(reader, PivotTable.ContainsString);

            if (bIsString)
                pivotDataType |= PivotDataType.String;

            bIsInt = ParseBoolAttribute(reader, PivotTable.ContainsInteger);

            if (bIsInt)
                pivotDataType |= PivotDataType.Integer;

            bIsLongText = ParseBoolAttribute(reader, PivotTable.LongText);

            if (bIsLongText)
                pivotDataType |= PivotDataType.LongText;

            field.DataType = pivotDataType;
            reader.MoveToElement();

            if (!reader.IsEmptyElement)
                reader.Read();

            string strValue = null;
            double dValue = 0.0;
            field.IsParsed = false;
            if (reader.LocalName != PivotTable.SharedItems)
            {
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    string strType = reader.LocalName;
                    if (reader.MoveToAttribute(PivotTable.ValueAttribute))
                    {
                        strValue = reader.Value;
                        field.IsParsed = true;
                    }
                    switch (strType)
                    {
                        case PivotTable.NumberTag:
                            dValue = XmlConvert.ToDouble(strValue);
                            field.AddValue(dValue);
                            break;
                        case PivotTable.StringTag:
                            field.AddValue(strValue);
                            break;
                        case PivotTable.DateTag:
                            DateTime dt =
#if ( WINRT )
 XmlConvert.ToDateTimeOffset(reader.Value).Date;
#else
                            XmlConvert.ToDateTime(reader.Value, XmlDateTimeSerializationMode.Unspecified);
#endif
                            field.AddValue(dt);
                            break;
                        case PivotTable.BooleanTag:
                            bool bValue = XmlConvert.ToBoolean(strValue);
                            field.AddValue(bValue);
                            break;
                        case PivotTable.EmptyTag:
                            field.IsParsed = true;
                            field.AddValue(null);
                            break;
                        case PivotTable.ErrorTag:
                            field.AddValue(strValue);
                            break;
                    }
                    reader.Read();
                }
            }
            reader.Read();
        }
        public static void ParseFieldGroup(XmlReader reader, PivotCacheFieldImpl field, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.FieldGroupElement)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache ");

            PivotCacheFieldImpl baseField = null;
            int parentFieldIndex = -1;
            int baseFieldIndex = -1;

            if (reader.MoveToAttribute(PivotTable.ParentFieldAttribute))
                parentFieldIndex = XmlConvert.ToInt32(reader.Value);



            if (reader.MoveToAttribute(PivotTable.BaseFieldAttribute))
            {
                baseFieldIndex = XmlConvert.ToInt32(reader.Value);

                if (baseFieldIndex == cache.CacheFields.Count)
                    baseField = field;
                else
                    baseField = cache.CacheFields[baseFieldIndex];
            }

            if (parentFieldIndex == -1)
            {
                field.FieldGroup = new FieldGroupImpl(baseField);
            }
            else
            {
                if (baseFieldIndex == -1)
                {
                    field.ParentFeildGroupIndex = parentFieldIndex;
                    reader.Read();
                    return;
                }
                field.FieldGroup = new FieldGroupImpl(baseField, parentFieldIndex);
            }
            int[] indexes = null;
            string[] items = null;
            reader.Read();
            indexes = ParseDiscretePropeties(reader, field);
            ParseRangeProperties(reader, field);
            items = ParseGroupItems(reader, field);
            if (indexes != null && items != null)
                field.FieldGroup.FillDiscreteGroup(indexes, items);
            else
                field.FieldGroup.FillRangeGroup(items);
            reader.Read();
            reader.Read();
        }
        public static int[] ParseDiscretePropeties(XmlReader reader, PivotCacheFieldImpl field)
        {

            if (reader.LocalName != PivotTable.DiscretePrElement)
                return null;

            List<int> indexes = new List<int>();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.IndexAttribute)
                {
                    if (reader.MoveToAttribute(PivotTable.ValueAttribute))
                        indexes.Add(XmlConvert.ToByte(reader.Value));
                }
                reader.Read();
            }
            reader.Read();
            return indexes.ToArray();
        }
        public static string[] ParseGroupItems(XmlReader reader, PivotCacheFieldImpl field)
        {
            if (reader.LocalName != PivotTable.GroupItemsElement)
                return null;

            reader.Read();
            List<string> values = new List<string>();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.CharAttribute)
                {
                    if (reader.MoveToAttribute(PivotTable.ValueAttribute))
                        values.Add(reader.Value);
                }
                reader.Read();
            }
            return values.ToArray();
        }
        public static void ParseRangeProperties(XmlReader reader, PivotCacheFieldImpl field)
        {
            if (reader.LocalName != PivotTable.RangePrElement)
                return;
            FieldGroupImpl fieldGroup = field.FieldGroup;


            if (reader.MoveToAttribute(PivotTable.AutoEndAttribute))
                fieldGroup.AutoEndRange = XmlConvert.ToBoolean(reader.Value);

            if (reader.MoveToAttribute(PivotTable.EndDateAttribute))
                fieldGroup.EndDate = Convert.ToDateTime(reader.Value);

            if (reader.MoveToAttribute(PivotTable.EndNumberAttribute))
                fieldGroup.EndNumber = XmlConvert.ToDouble(reader.Value);

            if (reader.MoveToAttribute(PivotTable.GroupByAttribute))
                fieldGroup.GroupBy = (PivotFieldGroupType)Enum.Parse(typeof(PivotFieldGroupType), reader.Value, true);

            if (reader.MoveToAttribute(PivotTable.GroupIntervalAttribute))
            {
                fieldGroup.GroupInterval = XmlConvert.ToDouble(reader.Value);
                fieldGroup.HasGroupInterval = true;
            }

            if (reader.MoveToAttribute(PivotTable.StartDateAttribute))
                fieldGroup.StartDate = Convert.ToDateTime(reader.Value);

            if (reader.MoveToAttribute(PivotTable.StartNumAttribute))
                fieldGroup.StartNumber = XmlConvert.ToDouble(reader.Value);

            reader.Read();


        }
        /// <summary>
        /// Parse pivot cache definition.
        /// </summary>
        /// <param name="reader">XmlReader to parse from.</param>
        /// <param name="cache">Cache to Parse.</param>
        public static void ParsePivotCacheRecords(XmlReader reader, PivotCacheImpl cache)
        {

            int iRowCount = 0;

            ParseCacheRecordRows(reader, cache, iRowCount);

        }
        /// <summary>
        /// Parse Cache Record Rows
        /// </summary>
        /// <param name="reader">XmlReader to parse</param>
        /// <param name="cache">cache to parse</param>
        /// <param name="rowCount">Rows Count</param>
        public static void ParseCacheRecordRows(XmlReader reader, PivotCacheImpl cache, int rowCount)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");

            reader.Read();
            byte[] indexes = null;
            int iCurrentRow = 0;
            int iFieldsCount = cache.CacheFields.GetOrdinaryFieldCount();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.PivotCacheRecord)
                {
                    indexes = ParseCacheRecordRow(reader, cache, rowCount, iFieldsCount);
                   
                    iCurrentRow++;
                    reader.Read();
                }
            }
        }
        /// <summary>
        /// Parse cache single row from the records
        /// </summary>
        /// <param name="reader">XmlReader to parse</param>
        /// <param name="cache">cache to parse the records</param>
        /// <param name="currentRow">current row index to parse</param>
        /// <returns>parsed row records</returns>
        public static byte[] ParseCacheRecordRow(XmlReader reader, PivotCacheImpl cache, int currentRow, int iFieldsCount)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (cache == null)
                throw new ArgumentNullException("pivot cache");



            int iRowCount = 0;
            int iCurrentItem = 0;
            double dValue;
            string strValue = null;
            byte[] indexes = new byte[iFieldsCount];

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                string strType = reader.LocalName;
                if (reader.MoveToAttribute(PivotTable.ValueAttribute))
                    strValue = reader.Value;
                switch (strType)
                {
                    case PivotTable.IndexAttribute:
                        uint index = XmlConvert.ToUInt32(strValue);
                        indexes[iCurrentItem] = (byte)index;
                        break;
                    case PivotTable.NumberTag:
                        dValue = XmlConvert.ToDouble(strValue);
                        indexes[iCurrentItem] = cache.PutValue(iCurrentItem, dValue);
                        break;
                    case PivotTable.StringTag:
                        indexes[iCurrentItem] = cache.PutValue(iCurrentItem, strValue);
                        break;
                    case PivotTable.DateTag:
                        DateTime dt =
#if ( WINRT ) 
                            XmlConvert.ToDateTimeOffset(reader.Value).Date;
#else
                            XmlConvert.ToDateTime(reader.Value, XmlDateTimeSerializationMode.Unspecified);
#endif
                        //dValue = UtilityMethods.ConvertDateTimeToNumber(dt);
                        indexes[iCurrentItem] = cache.PutValue(iCurrentItem, dt);
                        break;
                    case PivotTable.BooleanTag:
                        bool bValue = XmlConvert.ToBoolean(strValue);
                        indexes[iCurrentItem] = cache.PutValue(iCurrentItem, bValue);
                        break;
                    case PivotTable.EmptyTag:
                        indexes[iCurrentItem] = cache.PutValue(iCurrentItem, null);
                        break;
                }

                iCurrentItem++;
                reader.Read();
            }
            return indexes;
        }
        private static void ParseCalculatedItems(XmlReader reader, PivotCacheFieldsCollection cacheFields)
        {
            if (reader.LocalName != PivotTable.CalculatedItems)
                return;

            if (cacheFields == null)
                throw new ArgumentNullException("cacheField");

            reader.Read();
            string strFormula = null;
            int defaultIndex = 0;
            int fieldIndex = -1;
            PivotCacheFieldImpl cacheField = cacheFields[0];
            while (reader.NodeType != XmlNodeType.EndElement)
            {

                if (reader.LocalName == PivotTable.CalculatedItem)
                {
                    fieldIndex = ParseIntAttribute(reader, PivotTable.FieldAttribute, -1);

                    if (reader.MoveToAttribute(PivotTable.FormulaAttribute))
                        strFormula = reader.Value;

                    if (fieldIndex == -1)
                    {
                        string fieldName = GetFieldName(strFormula);
                        cacheField = cacheFields[fieldName];
                        fieldIndex = cacheField.Index;
                    }


                    PivotCalculatedItemImpl calculatedItem = new PivotCalculatedItemImpl(cacheField);
                    PivotArea pivotArea = calculatedItem.PivotArea;

                    if (strFormula != null)
                        calculatedItem.Formula = strFormula;

                    ParsePivotArea(reader, pivotArea);

                    if (fieldIndex == -1)
                    {
                        int index = pivotArea.References[0].FieldIndex;
                        cacheField = cacheFields[index];
                        calculatedItem.FieldIndex = index;
                    }
                    cacheField.CalculatedItems.Add(calculatedItem);

                    reader.Read();

                }
            }

        }
        /// <summary>
        /// Rule describing a PivotTable selection.
        /// </summary>
        /// <param name="reader">XmlReader to parse</param>
        /// <param name="area">Pivot Area to parse</param>
        private static void ParsePivotArea(XmlReader reader, PivotArea area)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (area == null)
                throw new ArgumentNullException("Pivot Area");

            reader.Read();

            if (reader.MoveToAttribute(PivotTable.AxisAttribute))
                area.Axis = (PivotAxisTypes)Enum.Parse(typeof(PivotAxisTypes2007), reader.Value, true);

            area.IsCacheIndex = ParseBoolAttribute(reader, PivotTable.CacheIndex);

            area.IsDataOnly = ParseBoolAttribute(reader, PivotTable.DataOnly);

            area.FieldIndex = ParseIntAttribute(reader, PivotTable.Field, -1);

            area.FieldPosition = ParseIntAttribute(reader, PivotTable.FieldPosition, -1);

            area.HasColumnGrand = ParseBoolAttribute(reader, PivotTable.ColumnGrandTotal);

            area.HasRowGrand = ParseBoolAttribute(reader, PivotTable.RowGrand);

            area.IsLableOnly = ParseBoolAttribute(reader, PivotTable.LableOnly);

            area.IsOutline = ParseBoolAttribute(reader, PivotTable.Outline, true);

            if (reader.MoveToAttribute(PivotTable.PivotAreaTypeAttribute))
                area.AreaType = (PivotAreaType)Enum.Parse(typeof(PivotAreaType), reader.Value, false);

            reader.Read();

            ParsePivotAreaRefereces(reader, area);

            reader.Read();
        }
        /// <summary>
        /// Parsses the Pivot Area selection reference
        /// </summary>
        /// <param name="reader">Xml Readers to read tags</param>
        /// <param name="pivotArea">pivot area to parse References</param>
        private static void ParsePivotAreaRefereces(XmlReader reader, PivotArea pivotArea)
        {

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (pivotArea == null)
                throw new ArgumentNullException("Pivot Area");

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == PivotTable.Reference)
                {
                    PivotAreaReference reference = new PivotAreaReference();

                    ParsePivotAreaReference(reader, reference);
                    pivotArea.References.Add(reference);
                    reader.Read();
                }
            }
            reader.Read();
        }
        /// <summary>
        /// Parse Pivot Area Reference
        /// </summary>
        /// <param name="reader">Xml Reader to read tags</param>
        /// <param name="reference">Pivot Area reference </param>
        private static void ParsePivotAreaReference(XmlReader reader, PivotAreaReference reference)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reference == null)
                throw new ArgumentNullException("reference");


            reference.FieldIndex = ParseIntAttribute(reader, PivotTable.Field, -1);

            reference.Subtotal = PivotTableParser.ParseSubtotalFlags(reader);

            reference.IsReferByPosition = ParseBoolAttribute(reader, PivotTable.ReferByPosition);

            reference.IsRelativeReference = ParseBoolAttribute(reader, PivotTable.ReferByRelative);

            reference.IsSelected = ParseBoolAttribute(reader, PivotTable.Selected);

            reader.Read();
            if (reader.LocalName != PivotTable.IndexAttribute)
                throw new XmlException("unexpected Element tag");

            int itemIndex = ParseIntAttribute(reader, PivotTable.ValueAttribute);

            reference.Indexes.Add(itemIndex);
            reader.Read();
        }

        /// <summary>
        /// Parse the pivot cache extension
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseCacheExtension(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != "extLst")
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add("extLst", stream);
            }
        }

        /// <summary>
        /// Parse Pivot cache hiearachy
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseCacheHierarchies(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.CacheHierarchies)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

            //TODO: Add support to parse Pivot Cache Hierarchies.
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.CacheHierarchies, stream);
            }
        }
        /// <summary>
        /// Parse Pivot Key Performance Indicators (KPIs) defined in the OLAP.
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseOLAPKPIs(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.PivotOLAPKPIs)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

            //TODO: Add support to parse Pivot OLAP KPIs.
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.PivotOLAPKPIs, stream);
            }
        }

        /// <summary>
        /// Parse PivotTable OLAP dimension collection.
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseOLAPDimensions(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.OLAPDimensions)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

            //TODO: Add support to parse Pivot OLAP Dimensions
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.OLAPDimensions, stream);
            }
        }
        /// <summary>
        /// Parses the PivotTable OLAP measure group - Dimension maps
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseOLAPMeasureGroups(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.OLAPMeasureGroups)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

            //TODO: Add support to parse Pivot OLAP Dimensions
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.OLAPMeasureGroups, stream);
            }
        }
        /// <summary>
        /// Parses the PivotTable OLAP Dimension maps.
        /// </summary>
        /// <param name="reader">Xml Reader to read tags.</param>
        /// <param name="cache">Pivot cache to parse. </param>
        private static void ParseOLAPMaps(XmlReader reader, PivotCacheImpl cache)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != PivotTable.OLAPMaps)
                return;

            if (cache == null)
                throw new ArgumentNullException("cache");

          
            //TODO: Add support to parse Pivot OLAP Maps
            if (cache.PreservedElements != null)
            {
                reader.MoveToElement();
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                cache.PreservedElements.Add(PivotTable.OLAPMaps, stream);
            }
        }
        #endregion
        #region Helper Methods
        internal static ExcelDataSourceType GetSourceType(string strSourceType)
        {
            ExcelDataSourceType sourceType = ExcelDataSourceType.Worksheet; ;
            switch (strSourceType)
            {
                case PivotTable.WorksheetSourceType:
                    sourceType = ExcelDataSourceType.Worksheet;
                    break;
                case PivotTable.ConsolidationTypeValue:
                    sourceType = ExcelDataSourceType.Consolidation;
                    break;
                case PivotTable.ExternalTypeValue:
                    sourceType = ExcelDataSourceType.ExternalData;
                    break;
                case PivotTable.ScenarioTypeValue:
                    sourceType = ExcelDataSourceType.ScenarioPivotTable;
                    break;
            }
            return sourceType;

        }
        #endregion

        #region XmlReader Utilities
        /// <summary>
        /// Moves to attribute and parse int attribute
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="attributeName">attribute name to parse</param>
        /// <returns>parsed int value</returns>
        private static int ParseIntAttribute(XmlReader reader, string attributeName)
        {
            if (reader.MoveToAttribute(attributeName))
            {
                string value = reader.Value;
                return XmlConvert.ToInt32(value);
            }
            return 0;
        }
        /// <summary>
        /// Moves to attribute and parse int attribute
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="attributeName">attribute name to parse</param>
        /// <returns>parsed int value</returns>
        private static int ParseIntAttribute(XmlReader reader, string attributeName, int defaultValue)
        {
            if (reader.MoveToAttribute(attributeName))
            {
                string value = reader.Value;
                return XmlConvert.ToInt32(value);
            }
            return defaultValue;
        }
        /// <summary>
        /// Moves to attribute and parse bool value
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="attributeName">attribute to parse</param>
        /// <returns>bool value</returns>
        private static bool ParseBoolAttribute(XmlReader reader, string attributeName)
        {
            if (reader.MoveToAttribute(attributeName))
            {
                string value = reader.Value;
                return XmlConvert.ToBoolean(value);
            }
            return false;
        }
        /// <summary>
        /// Moves to attribute and parse bool value
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="attributeName">attribute to parse</param>
        /// <returns>bool value</returns>
        private static bool ParseBoolAttribute(XmlReader reader, string attributeName, bool defaultValue)
        {
            if (reader.MoveToAttribute(attributeName))
            {
                string value = reader.Value;
                return XmlConvert.ToBoolean(value);
            }
            return defaultValue;
        }
        /// <summary>
        /// Moves to attribute and parse string value
        /// </summary>
        /// <param name="reader">XmlReader to parse from</param>
        /// <param name="attributeName">attribute to parse</param>
        /// <returns>string value</returns>
        private static string ParseStringAttribute(XmlReader reader, string attributeName)
        {
            if (reader.MoveToAttribute(attributeName))
                return reader.Value;
            return null;
        }
        private static string GetFieldName(string formula)
        {
            char formulaChar = '[';
            string name = formula.Split(formulaChar)[0];
            if (name.Contains("'"))
                name = name.Split('\'')[1];
            return name;
        }
        #endregion
    }
}
