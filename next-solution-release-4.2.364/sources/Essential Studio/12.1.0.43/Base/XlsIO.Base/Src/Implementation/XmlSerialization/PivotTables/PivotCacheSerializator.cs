#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables
{
    /// <summary>
    /// This class is repsonsible for pivot cache serialization in Excel 2007 format.
    /// </summary>
    class PivotCacheSerializator
    {
        #region Constants
        /// <summary>
        /// Default application version.
        /// </summary>
        private const int ApplicationVersion = 3;
        #endregion

        #region Methods
        /// <summary>
        /// Serializes pivot cache definition.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">Cache to serialize.</param>
        /// <param name="book">Current workbook.</param>
        /// <param name="relationId">Relation id to the pivot cache records.</param>
        public static void SerializePivotCacheDefinition(XmlWriter writer, PivotCacheImpl cache, IWorkbook book,
          string relationId, RelationCollection relations)
        {
            WorkbookImpl bookImpl = book as WorkbookImpl;
            if (bookImpl.PreservesPivotCache.Count > 0)
            {
                Stream stream = bookImpl.PreservesPivotCache[0];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
                bookImpl.PreservesPivotCache.RemoveAt(0);
                return;
            }

            writer.WriteStartElement(PivotTable.PivotCacheDefinition, Excel2007Serializator.XmlNamespaceMain);
            if (relationId != null || cache.RelationId != null)
            {
                if (cache.RelationId != null)
                {
                    relationId = cache.RelationId;
                    cache.IsSaveData = true;
                }
                writer.WriteAttributeString(Excel2007Serializator.RelationAttribute,
              Excel2007Serializator.RelationNamespace, relationId);
            }




            writer.WriteAttributeString(PivotTable.RefreshedBy, book.Author);
            
#if !(WINRT )
            //TO add support for this in ( WINRT )
            // refreshedDate="39877.710187268516"
            double refreshDate = cache.RefreshDate.ToOADate();
            writer.WriteAttributeString(PivotTable.RefreshedDate, XmlConvert.ToString(refreshDate));
#endif
            // createdVersion="3"
            writer.WriteAttributeString(PivotTable.CreatedVersion, ApplicationVersion.ToString());
            // refreshedVersion="3"
            writer.WriteAttributeString(PivotTable.RefreshedVersion, ApplicationVersion.ToString());
            // minRefreshableVersion="3"
            writer.WriteAttributeString(PivotTable.MinRefreshableVersion, ApplicationVersion.ToString());

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.BackgroundQueryAttribute, cache.IsBackgroundQuery, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.EnableRefreshAttribute, cache.EnableRefresh, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.RefreshOnLoad, cache.IsRefreshOnLoad, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.InvalidAttribute, cache.IsInvalidData, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.OptimizeMemoryAttribute, cache.IsOptimizedCache, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.UpgradeOnRefreshAttribute, cache.IsUpgradeOnRefresh, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.SupportAdvancedDrillAttribute, cache.SupportAdvancedDrill, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.SupportSubQueryAttribute, cache.IsSupportSubQuery, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.SaveDataAttribute, cache.IsSaveData, true);

            // recordCount="6">
            writer.WriteAttributeString(PivotTable.RecordCount, cache.RecordCount.ToString());

            SerializeCacheSource(writer, cache, relations);
            SerializeCacheFields(writer, cache.CacheFields, cache.HasNamedRange);

            SerializeCalculatdItems(writer, cache.CacheFields);
            SerializeCacheHierarchies(writer, cache);
            SerializeOLAPKPIs(writer, cache);
            SerializeOLAPDimesions(writer, cache);
            SerializeOLAPMeasureGroups(writer, cache);
            SerializeOLAPMaps(writer, cache);
            SerializeCacheExtensions(writer, cache);
            writer.WriteEndElement();

            ExternalRange range = cache.SourceRange as ExternalRange;

            if (range != null)
                RegisterStrings(book as WorkbookImpl, range);
        }

        private static void RegisterStrings(WorkbookImpl workbookImpl, ExternalRange range)
        {
            IWorksheet sheet = range.Worksheet;

            for (int iRow = range.Row; iRow <= range.LastRow; iRow++)
            {
                for (int iCol = range.Column; iCol <= range.LastColumn; iCol++)
                {
                    IRange cell = range[iRow, iCol];

                    if (cell.HasString)
                        workbookImpl.InnerSST.AddIncrease(cell.Text, true);
                }
            }
        }
        /// <summary>
        /// Serializes cache fields.
        /// </summary>
        /// <param name="writer">Writer to serialize into.</param>
        /// <param name="fieldsCollection">Fields collection.</param>
        private static void SerializeCacheFields(XmlWriter writer, PivotCacheFieldsCollection fieldsCollection, bool hasNamedRange)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (fieldsCollection == null)
                throw new ArgumentNullException("fieldsCollection");

            int iCount = fieldsCollection.Count;
            writer.WriteStartElement(PivotTable.CacheFields);
            writer.WriteAttributeString(PivotTable.CountAttribute, iCount.ToString());
            Dictionary <string,int> dictFieldName = new Dictionary <string,int>();
            for (int i = 0; i < iCount; i++)
            {
                PivotCacheFieldImpl field = fieldsCollection[i];
                if (!dictFieldName.ContainsKey(field.Name))
                {
                    dictFieldName.Add(field.Name,dictFieldName .Count+1);
                }
                else
                {
                    int FieldNameCount = 0;
                    string FieldName = field.Name;
                    while (dictFieldName.ContainsKey(FieldName))
                    {
                        FieldNameCount++;
                        FieldName = field.Name + FieldNameCount.ToString();
                    }
                    field.Name = FieldName;
                    dictFieldName.Add (field.Name,dictFieldName.Count+1);
                }
                SerializeCacheField(writer, field, hasNamedRange);
            }

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes cache field.
        /// </summary>
        /// <param name="writer">Writer to serialize into.</param>
        /// <param name="field">Field to serialize.</param>
        private static void SerializeCacheField(XmlWriter writer, PivotCacheFieldImpl field, bool hasNamedRange)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (field == null)
                throw new ArgumentNullException("field");

            // - <cacheField name="Row" numFmtId="0">
            // <sharedItems containsSemiMixedTypes="0" containsString="0" containsNumber="1" containsInteger="1" minValue="1" maxValue="4" /> 
            // </cacheField>

            // This is minimum information about cache field.
            writer.WriteStartElement(PivotTable.CacheField);
            writer.WriteAttributeString(PivotTable.NameAttribute, field.Name);
            writer.WriteAttributeString(PivotTable.NumberFormatAttribute, field.NumFormatIndex.ToString());

            if(field.Hierarchy>0)
                writer.WriteAttributeString(PivotTable.HierarchyAttribute, field.Hierarchy.ToString());
            if (field.Level > 0)
                writer.WriteAttributeString(PivotTable.HierarchyLevel, field.Level.ToString());

            if (field.IsFormulaField)
                writer.WriteAttributeString(PivotTable.FormulaAttribute, field.Formula);
            if (field.Caption != null)
                writer.WriteAttributeString(PivotTable.FieldCaption, field.Caption);

            if (field.IsFieldGroup && field.FieldGroup.IsDiscrete || field.IsFormulaField)
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.DatabaseFieldAttribute, field.IsDataBaseField, true);
            //<sharedItems containsSemiMixedTypes="0" containsString="0" containsNumber="1" containsInteger="1"
            // minValue="1" maxValue="3"/>

            else
                SerializeSharedItems(writer, field, hasNamedRange);
            if (field.IsFieldGroup)
                SerializeFieldGroup(writer, field);
            else if (field.ParentFeildGroupIndex != -1)
                SerializeFieldGroupParent(writer, field, field.ParentFeildGroupIndex);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes field shared items.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize shared items into.</param>
        /// <param name="field">Field to serialzie shared items for.</param>
        private static void SerializeSharedItems(XmlWriter writer, PivotCacheFieldImpl field, bool HasNamedRange)
        {
            IList<object> values = field.Items;
            writer.WriteStartElement(PivotTable.SharedItems);
            PivotDataType dataType = field.DataType;
            bool bString = (dataType & PivotDataType.String) != 0;
            bool bInteger = (dataType & PivotDataType.Integer) != 0 &&
              (dataType & PivotDataType.Float) == 0;
            bool bNumber = (dataType & PivotDataType.Number) != 0;//( dataType & ~( PivotDataType.Number | PivotDataType.Integer | PivotDataType.Float ) ) == 0;
            bool bDate = (dataType & PivotDataType.Date) != 0;
            bool bNonDate = (dataType & ~(PivotDataType.Date | PivotDataType.Blank)) != 0;
            bool bBlank = (dataType & PivotDataType.Blank) != 0;
            bool bMixedTypes = bNumber && bString || bNumber && bDate || bDate && bString;
            bool bValueType = false;
            bool bLongText=(dataType & PivotDataType.LongText)!=0;
            if ((dataType & (PivotDataType.Number | PivotDataType.Date)) != 0)
            {
                if (!bBlank &&
                    dataType != (PivotDataType.Date | PivotDataType.Blank)
                  && dataType != (PivotDataType.Number | PivotDataType.Blank)
                  && dataType != (PivotDataType.Number | PivotDataType.Blank | PivotDataType.Integer)
                  && dataType != (PivotDataType.Number | PivotDataType.Blank | PivotDataType.Integer | PivotDataType.Float)
                  && !bMixedTypes)
                {
                    writer.WriteAttributeString(PivotTable.ContainsSemiMixedTypes, "0");
                    bValueType = false;
                }

                if (!bDate)
                {
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsMixedTypes, bMixedTypes, false);
                    if (!bMixedTypes)
                        Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsString, bString, !bString);
                    else
                        bValueType = false;
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsBlank, bBlank, false);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsNumber, bNumber, !bNumber);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsInteger, bInteger, !bInteger);
                }
                else if(bDate && bString )
                {
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsDate, bDate, !bDate);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsMixedTypes, bMixedTypes, false);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsBlank, bBlank, false);
                    bValueType = false;
                }
                else
                {
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsNonDate, bNonDate, !bNonDate);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsDate, bDate, !bDate);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsString, bString, !bString);
                    Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsBlank, bBlank, false);
                    bValueType = false;
                }
            }
            else
            {
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.ContainsBlank, bBlank, false);
            }
            if (bBlank)
                bValueType = false;
            if (bLongText)
            {
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.LongText, bLongText, false);
                bValueType = false;
            }
            if (field.IsParsed != null)
            {
                if (field.IsParsed== true)
                    bValueType = false;
                else
                    bValueType = true;
            }
            //writer.WriteAttributeString( "minValue", "1" );
            //writer.WriteAttributeString( "maxValue", "3" );
            List<int> indexes = PrepareCalculatedItemOption(field);
            if ((indexes.Count > 0 || !bValueType))
            {
                for (int i = 0, len = values.Count; i < len; i++)
                {
                    bool isCalculated = indexes.Contains(i);
                    object value = values[i];
                    SerializePivotCacheValue(writer, value, isCalculated);
                }
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes cache source.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">Cache to serialize.</param>
        private static void SerializeCacheSource(XmlWriter writer, PivotCacheImpl cache,
          RelationCollection relations)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            switch (cache.SourceType)
            {
                case ExcelDataSourceType.Worksheet:
                    SerializeWorksheetSource(writer, cache, relations);
                    break;
                case ExcelDataSourceType.Consolidation:
                    //TODO: support consolidation
                    SerializeConsolidation(writer, cache);
                    break;
                case ExcelDataSourceType.ScenarioPivotTable:
                    SerializeScenarioSource(writer, cache);
                    break;
                case ExcelDataSourceType.ExternalData:
                    SerializeExternalSource(writer, cache);
                    break;
            }

        }
        /// <summary>
        /// Serializes the pivot table scenario
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">pivot cache</param>
        private static void SerializeScenarioSource(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            writer.WriteStartElement(PivotTable.CacheSource);
            writer.WriteAttributeString(PivotTable.TypeAttribute, PivotTable.ScenarioTypeValue);
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the pivot cache worksheet source
        /// </summary>
        /// <param name="writer">Xml writer to Serialize</param>
        /// <param name="cache">cache to serialized the data</param>
        private static void SerializeWorksheetSource(XmlWriter writer, PivotCacheImpl cache, RelationCollection relations)
        {
           
            writer.WriteStartElement(PivotTable.CacheSource);
            writer.WriteAttributeString(PivotTable.TypeAttribute, PivotTable.WorksheetSourceType);
            if (cache.PreservedElements.ContainsKey(PivotTable.WorksheetSource))
            {
                Stream stream = cache.PreservedElements[PivotTable.WorksheetSource];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
            else
            {
                writer.WriteStartElement(PivotTable.WorksheetSource);

                IRange range = cache.SourceRange;
                ExternalRange externalRange = range as ExternalRange;
                string strExternalRelation = null;

                if (externalRange != null)
                {
                    strExternalRelation = SerializeExternalRelation(externalRange, relations);
                }

                if (cache.HasNamedRange)
                {
                    writer.WriteAttributeString(PivotTable.NameAttribute, cache.RangeName);
                }
                else
                {

                    writer.WriteAttributeString(PivotTable.ReferenceAddress, range.AddressLocal);
                    writer.WriteAttributeString(PivotTable.SheetAttribute, (range as ICombinedRange).WorksheetName);
                }

                if (strExternalRelation != null)
                {
                    writer.WriteAttributeString(Excel2007Serializator.RelationAttribute,
                      Excel2007Serializator.RelationNamespace, strExternalRelation);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        private static string SerializeExternalRelation(ExternalRange externalRange, RelationCollection relations)
        {
            /*
             <Relationship Id="rId2"
             * Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLinkPath"
             * Target="Sample.xlsx" TargetMode="External" /> 
             */
            string resultId = relations.GenerateRelationId();
            string fileName = Path.GetFileName(externalRange.ExternSheet.Workbook.URL);
            relations[resultId] = new Relation(fileName, RelationTypes.ExternLinkPath, true);

            return resultId;
        }
        /// <summary>
        /// Serializes pivot cache External Source
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">Cache to serialize.</param>
        public static void SerializeExternalSource(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.ExternalTypeValue))
            {
                Stream stream = cache.PreservedElements[PivotTable.ExternalTypeValue];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        /// <summary>
        /// Serializes pivot cache consolidation Source
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">Cache to serialize.</param>
        public static void SerializeConsolidation(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.ConsolidationTypeValue))
            {
                Stream stream = cache.PreservedElements[PivotTable.ConsolidationTypeValue ];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        /// <summary>
        /// Serializes pivot cache definition.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="cache">Cache to serialize.</param>
        public static void SerializePivotCacheRecords(XmlWriter writer, PivotCacheImpl cache,MemoryStream memory)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");
            bool sharedStrings = false;
            writer.WriteStartElement(PivotTable.PivotCacheRecords, Excel2007Serializator.XmlNamespaceMain);
            PivotCacheFieldsCollection fields = cache.CacheFields;
            int iFieldsCount = cache.CacheFields.GetOrdinaryFieldCount();
           // cache.RecordCount = cache.SourceRange.Rows.Length;
            if (cache.SourceRange != null)
            {
                IWorkbook workbook = cache.SourceRange.Worksheet.Workbook;
                string WorksheetName = cache.SourceRange.Worksheet.Name;
                if (workbook.Worksheets[WorksheetName] != null && cache.RecordCount > 0)
                {
                    IMigrantRange migrantRange = cache.SourceRange.Worksheet.MigrantRange;
                    RangeImpl tRange = cache.SourceRange as RangeImpl;
                    for (int i = 1, len = cache.SourceRange.Rows.Length; i < len; i++)
                    {
                        writer.WriteStartElement(PivotTable.PivotCacheRecord);

                        for (int j = 0; j < iFieldsCount; j++)
                        {
                            //   object value1 = cache.GetValue(j, i);
                            migrantRange.ResetRowColumn(i + 1, j + 1);
                            object value = migrantRange.Value2;
                            if (migrantRange.HasFormula)
                            {
                                PivotDataType fieldType = 0;
                                bool isString = true;
                                if (migrantRange.HasFormulaNumberValue)
                                    value = migrantRange.FormulaNumberValue;
                                else if (migrantRange.HasFormulaDateTime)
                                    value = migrantRange.FormulaDateTime;
                                else if (migrantRange.HasFormulaBoolValue)
                                    value = migrantRange.FormulaBoolValue;
                                else if (migrantRange.HasFormulaStringValue)
                                    value = migrantRange.FormulaStringValue;
                            }
                            int count = 0;
                            while (count < cache.CacheFields.InnerList[j].Items.Count)
                            {
                                if (cache.CacheFields.InnerList[j].Items[count] != null)
                                {
                                    if (cache.CacheFields.InnerList[j].Items[count].Equals(value))
                                    {
                                        value = count;
                                        sharedStrings = true;
                                    }
                                }
                                count++;
                            }
                            if (sharedStrings)
                            {
                                writer.WriteStartElement(PivotTable.IndexAttribute);
                                writer.WriteAttributeString(PivotTable.ValueAttribute, value.ToString());
                                writer.WriteEndElement();
                                sharedStrings = false;
                            }
                            else
                            {
                                SerializePivotCacheValue(writer, value, false);
                            }
                        }

                        writer.WriteEndElement();
                        writer.Flush();
                        //if (memory.Length >= memory.Capacity - 2000)
                        //   memory.SetLength(2000);
                    }
                }
            }
            //memory.SetLength(memory.Position);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes pivot cache value.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize value into.</param>
        /// <param name="value">Value to serialize.</param>
        private static void SerializePivotCacheValue(XmlWriter writer, object value, bool isCalculated)
        {
           
            string tagName = null;
            string strValue = null;

            if (value is double)
            {
                tagName = PivotTable.NumberTag;
                strValue = XmlConvert.ToString((double)value);
            }
            else if (value is string)
            {
                strValue = (string)value;
                tagName = PivotTable.StringTag;
            }
            else if (value is DateTime)
            {
                tagName = PivotTable.DateTag;
                strValue = ((DateTime)value).ToString(PivotTable.DateTimeFormat);
            }
            else if (value is bool)
            {
                tagName = PivotTable.BooleanTag;
                strValue = XmlConvert.ToString((bool)value);
            }
            else if (value is ushort)//error
            {
                tagName = PivotTable.ErrorTag;
                ushort svalue = (ushort)value;
                strValue = GetErrorString(svalue);
            }
            else if (value == null)
            {
                strValue = null;
                tagName = PivotTable.EmptyTag;
            }
            else
            {
                throw new NotImplementedException();
            }

            writer.WriteStartElement(tagName);

            if (strValue != null)
                writer.WriteAttributeString(PivotTable.ValueAttribute, strValue);

            if (isCalculated)
                writer.WriteAttributeString(PivotTable.CalculatedMemberAttribute, "1");

            writer.WriteEndElement();
        }
        private static void SerializeCalculatdItems(XmlWriter writer, PivotCacheFieldsCollection fields)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (fields == null)
                throw new ArgumentNullException("fields");

            List<PivotCalculatedItems> itemsColl = new List<PivotCalculatedItems>();
            foreach (PivotCacheFieldImpl field in fields)
                if (field.CalculatedItems != null && field.CalculatedItems.Count > 0)
                    itemsColl.Add(field.CalculatedItems);

            if (itemsColl.Count > 0)
            {
                writer.WriteStartElement(PivotTable.CalculatedItems);

                foreach (PivotCalculatedItems items in itemsColl)
                    SerializeCalculatedItems(writer, items);

                writer.WriteEndElement();
            }
        }
        /// <summary>
        /// Serialize Calculated items of Cache field
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cacheField">calculated item to write</param>
        private static void SerializeCalculatedItems(XmlWriter writer, PivotCalculatedItems items)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (items == null)
                throw new ArgumentNullException("cache");

            if (items.Count == 0)
                return;




            foreach (PivotCalculatedItemImpl item in items)
                SerializeCalculatedItem(writer, item);


        }
        /// <summary>
        /// Serialize the Calculated Item of Cache Field
        /// </summary>
        /// <param name="writer">Xml writer to write</param>
        /// <param name="item">item to write </param>
        private static void SerializeCalculatedItem(XmlWriter writer, PivotCalculatedItemImpl item)
        {
            writer.WriteStartElement(PivotTable.CalculatedItem);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.Field, item.FieldIndex, -1);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FormulaAttribute, item.Formula, null);

            SerializePivotArea(writer, item.PivotArea);

            writer.WriteEndElement();
        }



        /// <summary>
        /// Serialize the Calculated Pivot item which the formula applies to 
        /// </summary>
        /// <param name="writer">xml writer to serialize into</param>
        /// <param name="area">area to serialize</param>
        private static void SerializePivotArea(XmlWriter writer, PivotArea area)
        {
            writer.WriteStartElement(PivotTable.PivotAreaTag);


            SerializeAttribute(writer, PivotTable.AxisAttribute, area.Axis, PivotAxisTypes.None);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.CacheIndex, area.IsCacheIndex, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.DataOnly, area.IsDataOnly, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.Field, area.FieldIndex, -1);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FieldPosition, area.FieldPosition, -1);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ColumnGrandTotal, area.HasColumnGrand, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.RowGrand, area.HasRowGrand, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.LableOnly, area.IsLableOnly, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.Outline, area.IsOutline, true);

            SerializeAttribute(writer, PivotTable.PivotAreaTypeAttribute, area.AreaType, PivotAreaType.None);

            SerializePivotAreaReferences(writer, area.References);

            writer.WriteEndElement();

        }
        /// <summary>
        /// Serialize the pivot area references
        /// </summary>
        /// <param name="writer">Xml Writer to serialize into</param>
        /// <param name="references">references to seralize to</param>
        private static void SerializePivotAreaReferences(XmlWriter writer, PivotAreaReferences references)
        {
            writer.WriteStartElement(PivotTable.References);
            writer.WriteAttributeString(PivotTable.CountAttribute, references.Count.ToString());

            foreach (PivotAreaReference reference in references)
                SerializePivotAreaReference(writer, reference);

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the Pivot area reference
        /// </summary>
        /// <param name="writer">XMl Writer to Serialize into </param>
        /// <param name="reference">reference to serialize</param>
        private static void SerializePivotAreaReference(XmlWriter writer, PivotAreaReference reference)
        {
            writer.WriteStartElement(PivotTable.Reference);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.Field, reference.FieldIndex, -1);

            if (reference.Subtotal != PivotSubtotalTypes.None)
                PivotTableSerializator.SerializeSubtotal(writer, reference.Subtotal);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ReferByPosition, reference.IsReferByPosition, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ReferByRelative, reference.IsRelativeReference, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.Selected, reference.IsSelected, false);

            foreach (int i in reference.Indexes)
            {
                writer.WriteStartElement(PivotTable.IndexAttribute);

                writer.WriteAttributeString(PivotTable.ValueAttribute, i.ToString());

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes field group
        /// </summary>
        /// <param name="writer">Xml writer to serialize into</param>
        /// <param name="field">field to serialize group</param>
        private static void SerializeFieldGroupParent(XmlWriter writer, PivotCacheFieldImpl field, int parentIndex)
        {
            writer.WriteStartElement(PivotTable.FieldGroupElement);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ParentFieldAttribute, parentIndex, -1);
            writer.WriteEndElement();

        }
        /// <summary>
        /// Serializes field group
        /// </summary>
        /// <param name="writer">Xml writer to serialize into</param>
        /// <param name="field">field to serialize group</param>
        private static void SerializeFieldGroup(XmlWriter writer, PivotCacheFieldImpl field)
        {
            FieldGroupImpl fieldGroup = field.FieldGroup;

            writer.WriteStartElement(PivotTable.FieldGroupElement);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ParentFieldAttribute, fieldGroup.ParentFieldIndex, -1);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.BaseFieldAttribute, fieldGroup.PivotCacheFieldIndex, -1);

            if (fieldGroup.IsDiscrete)
            {
                SerializeDiscreteProperties(writer, fieldGroup);
                SerializeGroupItems(writer, fieldGroup.PivotDiscreteGroupNames);
            }
            else
            {
                SerializeRangeProperties(writer, fieldGroup);
                SerializeGroupItems(writer, fieldGroup.PivotRangeGroupNames);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the Field group items
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="fieldGroup"></param>
        private static void SerializeGroupItems(XmlWriter writer, List<string> items)
        {
            if (items.Count == 0)
                return;
            writer.WriteStartElement(PivotTable.GroupItemsElement);
            foreach (string item in items)
            {
                writer.WriteStartElement(PivotTable.StringTag);
                writer.WriteAttributeString(PivotTable.ValueAttribute, item);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the Field Group Range Properties
        /// </summary>
        /// <param name="writer">Xml writer to serialize into</param>
        /// <param name="fieldGroup">field group to serialize </param>
        private static void SerializeRangeProperties(XmlWriter writer, FieldGroupImpl fieldGroup)
        {
            writer.WriteStartElement(PivotTable.RangePrElement);


            if (fieldGroup.HasDateTime)
            {
                string endDate = fieldGroup.EndDate.ToUniversalTime().ToString(DocProp.DateTimeFormatStructure, System.Globalization.CultureInfo.InvariantCulture);
                string startDate = fieldGroup.StartDate.ToUniversalTime().ToString(DocProp.DateTimeFormatStructure, System.Globalization.CultureInfo.InvariantCulture);
                writer.WriteAttributeString(PivotTable.EndDateAttribute, endDate );
                writer.WriteAttributeString(PivotTable.StartDateAttribute, startDate );
            }
            else if (fieldGroup.HasNumber)
            {
                writer.WriteAttributeString(PivotTable.EndNumberAttribute, fieldGroup.EndNumber.ToString());
                writer.WriteAttributeString(PivotTable.StartNumAttribute, fieldGroup.StartNumber.ToString());
            }
            else
            {
                writer.WriteAttributeString(PivotTable.AutoEndAttribute, fieldGroup.AutoEndRange.ToString());
                writer.WriteAttributeString(PivotTable.AutoStartAttribute, fieldGroup.AutoStartRange.ToString());
            }

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.GroupByAttribute, fieldGroup.GroupBy, PivotFieldGroupType.None);
            if(fieldGroup .HasGroupInterval)
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.GroupIntervalAttribute, fieldGroup.GroupInterval, -1);

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes the Filed Group discrete property
        /// </summary>
        /// <param name="writer">Xml writer to serialize into</param>
        /// <param name="fieldGroup">Feild Group </param>
        private static void SerializeDiscreteProperties(XmlWriter writer, FieldGroupImpl fieldGroup)
        {

            writer.WriteStartElement(PivotTable.DiscretePrElement);
            byte[] indexes = fieldGroup.DiscreteGroupIndexes;
            foreach (byte index in indexes)
            {
                writer.WriteStartElement(PivotTable.IndexAttribute);
                writer.WriteAttributeString(PivotTable.ValueAttribute, index.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

        }
        /// <summary>
        /// Serialize cache olap hierarchy collections
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeCacheHierarchies(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.CacheHierarchies))
            {
                Stream stream = cache.PreservedElements[PivotTable.CacheHierarchies];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }

        /// <summary>
        /// Serialize pivot cache extension
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeCacheExtensions(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("Extension");

            if (cache.PreservedElements.ContainsKey("extLst"))
            {
                Stream stream = cache.PreservedElements["extLst"];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }

        /// <summary>
        /// Serializes Pivot Key Performance Indicators (KPIs) defined in the OLAP.
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeOLAPKPIs(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.PivotOLAPKPIs))
            {
                Stream stream = cache.PreservedElements[PivotTable.PivotOLAPKPIs];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        /// <summary>
        /// Serializes PivotTable OLAP dimension collection.
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeOLAPDimesions(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.OLAPDimensions))
            {
                Stream stream = cache.PreservedElements[PivotTable.OLAPDimensions];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        /// <summary>
        /// Serializes the PivotTable OLAP measure group - Dimension maps. 
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeOLAPMeasureGroups(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.OLAPMeasureGroups))
            {
                Stream stream = cache.PreservedElements[PivotTable.OLAPMeasureGroups];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        /// <summary>
        /// Serializes the PivotTable OLAP Dimension maps.
        /// </summary>
        /// <param name="writer">xml writer to write items</param>
        /// <param name="cache">cache item to serialize.</param>
        private static void SerializeOLAPMaps(XmlWriter writer, PivotCacheImpl cache)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (cache == null)
                throw new ArgumentNullException("cache");

            if (cache.PreservedElements.ContainsKey(PivotTable.OLAPMaps))
            {
                Stream stream = cache.PreservedElements[PivotTable.OLAPMaps];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        
        /// <summary>
        /// collects the item index of calculated item
        /// </summary>
        /// <param name="field">field contains caluclated item</param>
        private static List<int> PrepareCalculatedItemOption(PivotCacheFieldImpl field)
        {
            PivotCalculatedItems items = field.CalculatedItems;
            List<int> indexes = new List<int>();
            foreach (PivotCalculatedItemImpl item in items)
                indexes.Add(item.PivotArea.References[0].Indexes[0]);
            return indexes;
        }
        /// <summary>
        /// Serializes attribute if it differs from default value.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        /// <param name="defaultValue">Default value.</param>
        internal static void SerializeAttribute(XmlWriter writer, string attributeName,
          Enum value, Enum defaultValue)
        {
            if (value.CompareTo(defaultValue) != 0)
            {
                writer.WriteAttributeString(attributeName, Excel2007Serializator.LowerFirstLetter(value.ToString()));
            }
        }
        #endregion
        #region Helper Methods
        private static string GetErrorString(ushort value)
        {
            string defualtErrorString = "#N/A";
            if (value == 1)
                return defualtErrorString;
            throw new NotImplementedException("Pivot Error String");
        }
        #endregion
    }
}
