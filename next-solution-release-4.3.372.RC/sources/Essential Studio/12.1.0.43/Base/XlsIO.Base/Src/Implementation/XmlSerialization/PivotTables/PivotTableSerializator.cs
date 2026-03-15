#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using System.IO;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
#endif
#if  SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.WP.Implementation.Extensions;
#elif !(SILVERLIGHT)&& !SyncfusionFramework2_0 && !(WINRT )
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
#endif


namespace Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables
{
    class PivotTableSerializator
    {
        #region Constants
        /// <summary>
        /// Index of the data field.
        /// </summary>
        private const int DataFieldsIndex = -2;
        private const string PercentageIndexFormat = "10";
        #endregion
        
        #region Methods
        /// <summary>
        /// Serializes pivot table object in xml format.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="pivotTable">Pivot table to serialize.</param>
        public static void SerializePivotTable(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            //pivotTable.PrepareSerialization();
			WorksheetImpl sheetImpl = pivotTable.Worksheet as WorksheetImpl;
            if (sheetImpl.PreservePivotTables.Count > 0)
            {
                Stream stream = sheetImpl.PreservePivotTables[0];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
                sheetImpl.PreservePivotTables.RemoveAt(0);
                return;
            }

            PivotTableOptions settings = pivotTable.Options as PivotTableOptions;

            //    <pivotTableDefinition xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"
            //    name="PivotTable1" cacheId="1" applyNumberFormats="0" applyBorderFormats="0" applyFontFormats="0"
            //    applyPatternFormats="0" applyAlignmentFormats="0" applyWidthHeightFormats="1" dataCaption="Values"
            //    updatedVersion="3" minRefreshableVersion="3" showCalcMbrs="0" useAutoFormatting="1" itemPrintTitles="1"
            //    createdVersion="3" indent="0" outline="1" outlineData="1" multipleFieldFilters="0">

            writer.WriteStartElement(PivotTable.PivotTableDefinition, Excel2007Serializator.XmlNamespaceMain);
            writer.WriteAttributeString(PivotTable.NameAttribute, pivotTable.Name);
            // TODO: change id to correct value.
            writer.WriteAttributeString(PivotTable.PivotCacheId, (pivotTable.CacheIndex + 1).ToString());

            SerializeAttributeString(writer, PivotTable.ApplyNumberFormats, settings.IsNumberAutoFormat, true);
            SerializeAttributeString(writer, PivotTable.ApplyBorderFormats, settings.IsBorderAutoFormat, true);
            SerializeAttributeString(writer, PivotTable.ApplyFontFormats, settings.IsFontAutoFormat, true);
            SerializeAttributeString(writer, PivotTable.ApplyPatternFormats, settings.IsPatternAutoFormat, true);
            SerializeAttributeString(writer, PivotTable.ApplyAlignmentFormats, settings.IsAlignAutoFormat, true);
            SerializeAttributeString(writer, PivotTable.ApplyWidthHeightFormats, settings.IsWHAutoFormat, false);

            // For some reason Excel has those values switched.
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ColumnGrandTotal, pivotTable.ColumnGrand, true);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.RowGrandTotal, pivotTable.RowGrand, true);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FieldPrintTitles, settings.PrintTitles, false);

            string colHeaderCaption = settings.ColumnHeaderCaption;
            if (colHeaderCaption != null && colHeaderCaption.Length > 0)
                writer.WriteAttributeString(PivotTable.ColumnHeaderCaption, settings.ColumnHeaderCaption);

            string rowHeaderCaption = settings.RowHeaderCaption;
            if (rowHeaderCaption != null && rowHeaderCaption.Length > 0)
                writer.WriteAttributeString(PivotTable.RowHeaderCaption, settings.RowHeaderCaption);

            if (settings.RowLayout == PivotTableRowLayout.Compact)
            {
                writer.WriteAttributeString(PivotTable.Outline, "1");
                writer.WriteAttributeString(PivotTable.OutlineData, "1");
            }
            else if (settings.RowLayout == PivotTableRowLayout.Outline)
            {
                writer.WriteAttributeString(PivotTable.Compact, "0");
                writer.WriteAttributeString(PivotTable.CompactData, "0");
                writer.WriteAttributeString(PivotTable.Outline, "1");
                writer.WriteAttributeString(PivotTable.OutlineData, "1");
            }
            else
            {
                writer.WriteAttributeString(PivotTable.Compact, "0");
                writer.WriteAttributeString(PivotTable.CompactData, "0");
            }

            SerializeAttributeString(writer, PivotTable.CreatedVersion, settings.CreatedVersion);
            SerializeAttributeString(writer, PivotTable.UpdatedVersion, settings.UpdatedVersion);
            
            settings.MiniRefreshVersion = PivotTableImpl.Excel2007Version;
            SerializeAttributeString(writer, PivotTable.MinRefreshableVersion, settings.MiniRefreshVersion);

            SerializeAttributeString(writer, PivotTable.CustomListSort, settings.ShowCustomSortList, true);
            writer.WriteAttributeString(PivotTable.DataCaption, settings.DataCaption);






            SerializeAttributeString(writer, PivotTable.DataOnRows, pivotTable.ShowDataFieldInRow, false);

            if (settings.DataPosition > 0)
                SerializeAttributeString(writer, PivotTable.DataPosition, settings.DataPosition);
            SerializeAttributeString(writer, PivotTable.AllowEditData, settings.IsDataEditable, false);
            SerializeAttributeString(writer, PivotTable.EnableDrillDown, pivotTable.EnableDrilldown, false);
            SerializeAttributeString(writer, PivotTable.EnableFieldProerties, settings.EnableFieldProperties, false);
            SerializeAttributeString(writer, PivotTable.EnableWizard, pivotTable.EnableWizard, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.AsteriskTotalAttribute, settings.ShowAsteriskTotals, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.MergeItem, settings.MergeLabels, false);


            SerializeAttributeString(writer, PivotTable.ShowCalcMbrs, settings.ShowCalcMembers, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowDrill, pivotTable.ShowDrillIndicators, true);

            SerializeAttributeString(writer, PivotTable.UseAutoFormatting, settings.IsAutoFormat, false);

            SerializeAttributeString(writer, PivotTable.ShowDataTips, settings.ShowTooltips, true);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ItemPrintTitles, pivotTable.RepeatItemsOnEachPrintedPage, false);


            if(settings.Indent<=settings.MaxIndent)
                SerializeAttributeString(writer, PivotTable.Indent, settings.Indent);
            else
                SerializeAttributeString(writer, PivotTable.Indent, settings.MaxIndent);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowHeaders, pivotTable.DisplayFieldCaptions, true);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.PageWrap, settings.PageFieldWrapCount, 0);

            SerializeAttributeString(writer, PivotTable.MultipleFieldFilters, settings.IsMultiFieldFilter, true);
            SerializeAttributeString(writer, PivotTable.ShowGridDropZone, settings.ShowGridDropZone, false);

            if (settings.PageFieldsOrder == PivotPageAreaFieldsOrder.OverThenDown)
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.PageOverThenDown, true, false);

            SerializeAttributeString(writer, PivotTable.ShowMissing, settings.DisplayNullString, true);

            if (settings.NullString.Length > 0)
                writer.WriteAttributeString(PivotTable.MissingCapiton, settings.NullString);

            SerializeAttributeString(writer, PivotTable.ShowError, settings.DisplayErrorString, false);

            if (settings.ErrorString.Length > 0)
                writer.WriteAttributeString(PivotTable.ErrorCaption, settings.ErrorString);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.PreserveFormatting, settings.PreserveFormatting, true);

            if(settings.IsDefaultAutoSort)
            writer.WriteAttributeString(PivotTable.DefaultAutoSort, Excel2007Serializator.TrueValue);
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
            PivotEngine pivotEngine = new PivotEngine();
            pivotEngine = pivotTable.PivotEngineValues;
#endif
            SerializeLocation(writer, pivotTable);
            SerializePivotFields(writer, pivotTable);

            SerializeRowFields(writer, pivotTable);
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT ) 
            if (pivotTable.PivotEngineValues != null)
            SerializeRowItems( writer, pivotTable,pivotEngine );
#endif
            SerializeColumnFields(writer, pivotTable);
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
            if (pivotTable .PivotEngineValues!= null )
            SerializeColumnItems( writer, pivotTable,pivotEngine );
#endif
            SerializePageFields(writer, pivotTable);
            SerializeDataFields(writer, pivotTable);
            SerializeCustomFormats(writer, pivotTable);
            SerializeConditionalFormats(writer, pivotTable);
            SerializeChartFormats(writer, pivotTable);
            SerializePivotHierarchies(writer, pivotTable);
            SerializeStyle(writer, pivotTable);
            SerializeRowHierarchies(writer, pivotTable);
            SerializeFilters(writer, pivotTable);
            if (sheetImpl.Version == ExcelVersion.Excel2010 || sheetImpl.Version == ExcelVersion.Excel2013)
                SerializeTableDefinitionExtensionList(writer, pivotTable);
            writer.WriteEndElement();
        }
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WP)
        /// <summary>
        /// Serialize the row items of pivottableDefinition.xml
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pivotTable"></param>
        /// <param name="pivotEngine"></param>
        private static void SerializeRowItems(XmlWriter writer, PivotTableImpl pivotTable,PivotEngine pivotEngine)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (pivotTable.RowFields.Count <= 0)
                return;

            List<int> listFields = new List<int>();
            PivotTableFields fields = pivotTable.Fields;
            List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Row);
            foreach(PivotFieldImpl field in filteredFields)
            listFields.Add(fields.IndexOf(field));
            Dictionary<int, Dictionary<string, int>> allFieldsValues = new Dictionary<int, Dictionary<string, int>>();
            int key = 0;
            foreach (int listFieldsCount in listFields)
            {
                PivotFieldImpl field = fields[listFieldsCount];
                SortedList<ComparisonPair, object> listData = SortFieldValues(field.CacheField);
                Dictionary<string, int> fieldValues = new Dictionary<string, int>();
                int lstDataCount = 0;
                while (lstDataCount < listData.Count)
                {
                    fieldValues.Add(listData.Keys[lstDataCount].Value.ToString(), lstDataCount);
                    lstDataCount++;
                }
                if (field.Axis == PivotAxisTypes.Row)
                    allFieldsValues.Add(key++, fieldValues);
            }
            if (!(pivotEngine.ColumnCount == 1 && pivotEngine.RowCount == 1))
            {
                if (pivotEngine.RowCount > 0)
                    writer.WriteStartElement(PivotTable.RowItems);
                int count = pivotTable.RowFields.Count;
                for (int i = 0; i < pivotEngine.RowCount; i++)
                {

                    for (int j = 0; j < count; j++)
                    {
                        if ((pivotEngine[i, j] != null))
                        {
                            if ((pivotEngine[i, j].CellType & PivotCellType.RowHeaderCell) != 0 || ((pivotEngine[i, j].CellType & PivotCellType.TopLeftCell) != 0))
                            {
                                if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) == 0 && ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) == 0))
                                {
                                    if (pivotEngine[i, j].Value != null && pivotEngine[i, j].Value != PivotTable.RowLabels)
                                    {
                                        Dictionary<string, int> currentFieldValues = allFieldsValues[j];
                                        if (currentFieldValues.ContainsKey(pivotEngine[i, j].Value.ToString()))
                                        {
                                            writer.WriteStartElement(PivotTable.Item);
                                            if (j != 0)
                                                writer.WriteAttributeString(PivotTable.RepeatItemsCount, j.ToString());
                                            writer.WriteStartElement(PivotTable.ValueItem);
                                            if (currentFieldValues.ContainsKey(pivotEngine[i, j].Value.ToString()))
                                                if (Convert.ToInt32(currentFieldValues[pivotEngine[i, j].Value.ToString()]) != 0)
                                                    writer.WriteAttributeString(PivotTable.ValueAttribute, currentFieldValues[pivotEngine[i, j].Value.ToString()].ToString());
                                            writer.WriteEndElement();
                                            writer.WriteEndElement();
                                        }
                                    }
                                }
                                else if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)
                                {
                                    writer.WriteStartElement(PivotTable.Item);
                                    writer.WriteAttributeString(PivotTable.ItemTypeAttribute, PivotTable.GrandTotalAttribute);
                                    writer.WriteStartElement(PivotTable.ValueItem);
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();
                                }
                            }
                        }
                    }

                }
                if (pivotEngine.RowCount > 0)
                    writer.WriteEndElement();
            }
        }
#endif
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WP)
        /// <summary>
        /// Serialize the column items of PivottableDefinition.xml
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="pivotTable"></param>
        /// <param name="pivotEngine"></param>
        private static void SerializeColumnItems(XmlWriter writer, PivotTableImpl pivotTable,PivotEngine pivotEngine)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");
            if (pivotTable.ColumnFields.Count <= 0)
                return;

            List<int> listFields = new List<int>();
            PivotTableFields fields = pivotTable.Fields;
            List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Column);

            for (int i = 0, length = filteredFields.Count; i < length; i++)
            {
                PivotFieldImpl field = filteredFields[i];

                listFields.Add(fields.IndexOf(field));
            }
            if (!pivotTable.ShowDataFieldInRow)
            {
                PivotDataFields dataFields = pivotTable.DataFields;

                if (dataFields.Count > 1)
                    listFields.Add(DataFieldsIndex);
            }
            Dictionary<int, Dictionary<string, int>> allFieldValues = new Dictionary<int, Dictionary<string, int>>();
            int allFieldsValuesKey = 0;
            foreach (int listFieldsCount in listFields)
            {
                if (listFieldsCount >= 0)
                {
                    PivotFieldImpl field = fields[listFieldsCount];
                    SortedList<ComparisonPair, object> listData = SortFieldValues(field.CacheField);
                    Dictionary<string, int> fieldValues = new Dictionary<string, int>();
                    int lstDataCount = 0;
                    while (lstDataCount < listData.Count)
                    {
                        fieldValues.Add(listData.Keys[lstDataCount].Value.ToString(), lstDataCount);
                        lstDataCount++;
                    }
                    if (field.Axis == PivotAxisTypes.Column)
                        allFieldValues.Add(allFieldsValuesKey++, fieldValues);
                }
                //
                else
                {
                    Dictionary<string, int> fieldValues = new Dictionary<string, int>();
                    for(int currentDataField =0 ;currentDataField <pivotTable .DataFields .Count ;currentDataField ++)
                    {
                        fieldValues.Add(pivotTable.DataFields[currentDataField].Name, currentDataField );
                    }
                    allFieldValues.Add(allFieldsValuesKey++, fieldValues);
                }
            }
            if (!(pivotEngine.ColumnCount == 1 && pivotEngine.RowCount == 1))
            {
                if (pivotEngine.ColumnCount > 0)
                    writer.WriteStartElement(PivotTable.ColumnItems);
                //starting of row items
                int rowCount = pivotTable.RowFields.Count;
                int colCount = pivotTable.ColumnFields.Count;
                if (pivotTable.DataFields.Count > 1)
                    colCount++;
                bool writePivotItem = true;
                string previousExpanderValue = null;
                int lastExpanderRow = 0;
                bool writeEndItem = false;
                for (int j = 0; j < pivotEngine.ColumnCount; j++)
                {

                    writePivotItem = true;
                    for (int i = 0; i < colCount; i++)
                    {
                        if ((pivotEngine[i, j] != null))
                        {
                            if ((pivotEngine[i, j].CellType & PivotCellType.ColumnHeaderCell) != 0 || (pivotEngine[i, j].CellType & PivotCellType.TopLeftCell) != 0)
                            {
                                if ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) == 0)
                                {
                                    if (pivotEngine[i, j].Value != null)
                                    {
                                        if (i == 0)
                                        {
                                            for (int k = 0; k < colCount; k++)
                                            {
                                                if ((pivotEngine[k, j].CellType & PivotCellType.TotalCell) != 0)
                                                {
                                                    break;
                                                }
                                                if ((pivotEngine[k, j].CellType & PivotCellType.ExpanderCell) != 0 || (pivotEngine[k, j].CellType & PivotCellType.TopLeftCell) != 0)// else may be required
                                                {
                                                    i = k;
                                                    break;
                                                }
                                            }
                                        }
                                        if ((pivotEngine[i, j].CellType & PivotCellType.ExpanderCell) != 0)
                                        {
                                            if (i > lastExpanderRow)
                                            {
                                                lastExpanderRow = i;
                                            }
                                        }
                                        if ((pivotEngine[i, j].CellType & PivotCellType.GrandTotalCell) != 0)
                                        {
                                            writer.WriteStartElement(PivotTable.Item);
                                            writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, PivotTable.GrandTotalAttribute);
                                            writer.WriteStartElement(PivotTable.ValueItem);
                                            writer.WriteEndElement();
                                            writer.WriteEndElement();
                                            break;

                                        }
                                        else if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) == 0)
                                        {
                                            bool CheckColumnHeaders =true ;
                                            if (pivotTable.ColumnFields.Count > 1)
                                            {
                                                if (i <= lastExpanderRow && (pivotEngine[i, j].CellType & PivotCellType.ExpanderCell) == 0)
                                                {
                                                    CheckColumnHeaders = false; // just for skipping the column headers in expander row of pivot engine.
                                                }
                                            }
                                            if (CheckColumnHeaders)
                                            {
                                                Dictionary<string, int> currentFieldValues = allFieldValues[i];
                                                if (currentFieldValues.ContainsKey(pivotEngine[i, j].Value.ToString()))
                                                {
                                                    if (writePivotItem)
                                                    {
                                                        writer.WriteStartElement(PivotTable.Item);
                                                        if (i != 0)
                                                            writer.WriteAttributeString(PivotTable.RepeatItemsCount, i.ToString());
                                                        writePivotItem = false;
                                                        writeEndItem = true;
                                                    }
                                                    writer.WriteStartElement(PivotTable.ValueItem);

                                                    if (Convert.ToInt32(currentFieldValues[pivotEngine[i, j].Value.ToString()]) != 0)
                                                        writer.WriteAttributeString(PivotTable.ValueAttribute, currentFieldValues[pivotEngine[i, j].Value.ToString()].ToString());
                                                    writer.WriteEndElement();
                                                }
                                            }
                                        else 
                                            i = i;
                                        }
                                        else if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) != 0)
                                        {
                                            Dictionary<string, int> calculationHeaderCell = new Dictionary<string, int>();
                                            for (int m = 0; m < pivotTable.DataFields.Count; m++)
                                                calculationHeaderCell.Add(pivotTable.DataFields[m].Name, m);
                                            if (writePivotItem)
                                            {
                                                writer.WriteStartElement(PivotTable.Item);
                                                if (i != 0)
                                                    writer.WriteAttributeString(PivotTable.RepeatItemsCount, i.ToString());
                                                if (Convert.ToInt32(calculationHeaderCell[pivotEngine[i, j].Value.ToString()]) != 0)
                                                    writer.WriteAttributeString(PivotTable.Item, calculationHeaderCell[pivotEngine[i, j].Value.ToString()].ToString());
                                                writePivotItem = false;
                                                writeEndItem = true;
                                            }
                                            writer.WriteStartElement(PivotTable.ValueItem);
                                            string value ;
                                            if (calculationHeaderCell.ContainsKey(value = pivotEngine[i, j].Value.ToString()))
                                                if ((Convert.ToInt32(calculationHeaderCell[value]) != 0))
                                                    writer.WriteAttributeString(PivotTable.ValueAttribute, calculationHeaderCell[pivotEngine[i, j].Value.ToString()].ToString());
                                            writer.WriteEndElement();
                                        }
                                        if (i == colCount - 1 && writeEndItem)
                                        {
                                            writer.WriteEndElement();
                                            writeEndItem = false;
                                            writePivotItem = true;
                                        }
                                    }
                                }
                                else if ((pivotEngine[i, j].CellType & PivotCellType.TotalCell) != 0)
                                {
                                    if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) == 0)
                                    {
                                        Dictionary<string, int> fieldValues = allFieldValues[i];
                                        writer.WriteStartElement(PivotTable.Item);
                                        writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, PivotTable.DefaultConst);
                                        if (i != 0)
                                            writer.WriteAttributeString(PivotTable.RepeatItemsCount, i.ToString());
                                        writer.WriteStartElement(PivotTable.ValueItem);
                                        string key = pivotEngine[i, j].Value.ToString();
                                        string FinalKey = key.Remove(key.Length - 1);
                                        if (fieldValues.ContainsKey(FinalKey))
                                            if (Convert.ToInt32(fieldValues[FinalKey]) != 0)
                                                writer.WriteAttributeString(PivotTable.ValueAttribute, fieldValues[FinalKey].ToString());
                                        writer.WriteEndElement();
                                        writer.WriteEndElement();
                                        i = colCount;
                                    }
                                    else if ((pivotEngine[i, j].CellType & PivotCellType.CalculationHeaderCell) != 0)
                                    {
                                        Dictionary<string, int> calculationHeaderCell = new Dictionary<string, int>();
                                        for (int m = 0; m < pivotTable.DataFields.Count; m++)
                                            calculationHeaderCell.Add(pivotTable.DataFields[m].Name, m);
                                        writer.WriteStartElement(PivotTable.Item);
                                        writer.WriteAttributeString("t", PivotTable.DefaultConst);
                                        if (Convert.ToInt32(calculationHeaderCell[pivotEngine[i, j].Value.ToString()]) != 0)
                                            writer.WriteAttributeString(PivotTable.Item, calculationHeaderCell[pivotEngine[i, j].Value.ToString()].ToString());
                                        writer.WriteStartElement(PivotTable.ValueItem);
                                        if (calculationHeaderCell.ContainsKey(pivotEngine[i, j].Value.ToString()))
                                            if (Convert.ToInt32(calculationHeaderCell[pivotEngine[i, j].Value.ToString()]) != 0)
                                                writer.WriteAttributeString(PivotTable.ValueAttribute, calculationHeaderCell[pivotEngine[i, j].Value.ToString()].ToString());
                                        writer.WriteEndElement();
                                        writer.WriteEndElement();
                                        i = colCount;
                                    }
                                }
                            }
                        }
                    }

                }
                if (pivotEngine.ColumnCount > 0)
                    writer.WriteEndElement();
            }
        }
#endif
        /// <summary>
        /// Serializes the Extern List of Pivot table Definition.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the chart formats into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializeTableDefinitionExtensionList(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");
            if (pivotTable.PreservedElements.ContainsKey(Excel2007Serializator.Extensionlist))
            {
                Stream stream = pivotTable.PreservedElements[Excel2007Serializator .Extensionlist ];
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
            else
            {
                writer.WriteStartElement(Excel2007Serializator.Extensionlist);
                writer.WriteStartElement(Excel2007Serializator.Extension);
                writer.WriteAttributeString(SparkConstants.UriAttribute, Excel2007Serializator.ExternListUri);
                writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.X14Prefix, null, Excel2007Serializator.X14Namespace);
                writer.WriteStartElement(Excel2007Serializator.X14Prefix, Excel2007Serializator.x14PivotTableDefinitionAttributes,null );
                writer.WriteAttributeString(Excel2007Serializator.HideValuesRowAttribute, "1");
                writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.MSPrefix, null, Excel2007Serializator.MSNamespaceMain);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }
        /// <summary>
        /// Serializes the Chart format elements associated with the pivot table fields.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the chart formats into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializeChartFormats(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (!pivotTable.PreservedElements.ContainsKey(PivotTable.ChartFormats))
                return;

            Stream stream = pivotTable.PreservedElements[PivotTable.ChartFormats];
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }
        /// <summary>
        /// Serializes the Custom format elements of the pivot table fields.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the custom formats into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializeCustomFormats(XmlWriter writer, PivotTableImpl pivotTable)
        {
            //throw new NotImplementedException();
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (!pivotTable.PreservedElements.ContainsKey(PivotTable.CustomFormats))
                return;

            Stream stream = pivotTable.PreservedElements[PivotTable.CustomFormats];
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }
        /// <summary>
        /// Serializes the filters elements of the pivot table fields.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the fileter into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializeFilters(XmlWriter writer, PivotTableImpl pivotTable)
        {
            //throw new NotImplementedException();
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            
            if (pivotTable.Filters.Count <= 0)
            {
                return;
            }

            writer.WriteStartElement(PivotTable.Filters);

            PivotTableFilters filters = pivotTable.Filters;
            if (filters.Count > 0)
            {
                for(int i=0 ;i<filters .Count ;i++)
                SerializePivotFilter(writer, pivotTable.Filters[i]);
            }
         
            writer.WriteEndElement();

            
        }

        /// <summary>
        /// Serialize the filter of the pivot filters.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializePivotFilter(XmlWriter writer, PivotTableFilter filter)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (filter == null)
                throw new ArgumentNullException("filter");
            
            writer.WriteStartElement(PivotTable .Filter);
            if (filter .DescriptionAttribute !=null )
                writer.WriteAttributeString(PivotTable.Description, filter.DescriptionAttribute);
            if(filter .EvalOrder !=0)
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.EvalOrderAttribute ,
                                filter .EvalOrder , 0);
            writer.WriteAttributeString(PivotTable.FieldAttribute, filter.Field.ToString());
            writer.WriteAttributeString(PivotTable.MeasureFldAttribute, filter.MeasureFld.ToString());
            writer.WriteAttributeString(PivotTable.PivotFilterId, filter.FilterId.ToString());
            if (filter.Value1 != null)
                writer.WriteAttributeString(PivotTable.Value1, filter.Value1);
            if (filter.Value2 != null)
                writer.WriteAttributeString(PivotTable.Value2, filter.Value2);
            writer.WriteAttributeString(PivotTable.TypeAttribute, ((PivotFilterType2007)filter.Type).ToString());

            if (filter.Count>0)
            {
                for(int i=0;i<filter .Count ;i++)
                SerializeAutoFilter(writer, filter[i]);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize the filter of the pivot filters.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeAutoFilter(XmlWriter writer, PivotAutoFilter autoFilters)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (autoFilters == null)
                throw new ArgumentNullException("autoFilters");
            writer.WriteStartElement(PivotTable.AutoFilterElement);
            if (autoFilters.FilterRange != null)
                writer.WriteAttributeString(PivotTable.ReferenceAddress, autoFilters.FilterRange);
            if (autoFilters.Count > 0)
            {
                for(int i=0;i<autoFilters .Count ;i++)
                SerializeFilterColumn(writer, autoFilters [i]);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize the filter of the pivot filters.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeFilterColumn(XmlWriter writer, PivotFilterColumn filterColumn)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (filterColumn == null)
                throw new ArgumentNullException("filterColumn");
            writer.WriteStartElement(PivotTable .FilterColumnElement );
           
            writer.WriteAttributeString(PivotTable.ColumnIdAttribute, filterColumn .ColumnId.ToString ());
            SerializeAttributeString(writer, PivotTable.HiddenButtonAttribute,
                 filterColumn .HiddenButton ,false );
            SerializeAttributeString(writer, PivotTable.ShowButtonAttribute,
                filterColumn.ShowButton, true);
          
            if (filterColumn.CustomFilters != null )
                SerializeCustomFilters(writer, filterColumn.CustomFilters);
            if (filterColumn.FilterColumnFilter != null)
                SerializeFilterColumnFilters(writer, filterColumn.FilterColumnFilter);
            if (filterColumn.Top10Filters != null)
                SerializeTop10Filter(writer, filterColumn.Top10Filters);
            writer.WriteEndElement();
       }
        /// <summary>
        /// Serialize the Custom filters of the Filter Column.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeCustomFilters(XmlWriter writer, PivotCustomFilters customFilters)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (customFilters == null)
                throw new ArgumentNullException("colorFilter");
            writer.WriteStartElement(PivotTable .CustomFiltersElement);
            
            SerializeAttributeString(writer, PivotTable.AndAttributeName,
                customFilters.HasAnd, false );
            for (int i = 0; i < customFilters.Count; i++)
            SerializeCustomFilter (writer ,customFilters [i]);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize the custom filter of the custom filters.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeCustomFilter(XmlWriter writer, PivotCustomFilter customFilter)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (customFilter == null)
                throw new ArgumentNullException("colorFilter");

            writer.WriteStartElement(PivotTable .CustomFilterElement);

            if (customFilter.FilterOperator != FilterOperator2007.Equal)
                writer.WriteAttributeString(PivotTable.OperatorAttribute, ((FilterOperator)customFilter.FilterOperator).ToString());
            if (customFilter.Value !=null )
                writer.WriteAttributeString(PivotTable.ValAttibuteName , customFilter.Value);
            writer.WriteEndElement();
        }

         /// <summary>
        /// Serialize the color filter of the Filter Column.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeFilterColumnFilters(XmlWriter writer,FilterColumnFilters filters)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            
            if (filters == null)
                throw new ArgumentNullException("filters");

            writer.WriteStartElement(PivotTable .Filters);

            if (filters.Count() > 0)
            {
                writer.WriteStartElement(PivotTable.Filter);
                for (int i = 0; i < filters.Count(); i++)
                {
                    writer.WriteAttributeString(PivotTable.ValAttibuteName, filters[i]);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Serialize the custom filter of the custom filters.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeTop10Filter(XmlWriter writer, PivotTop10Filter top10Filter)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (top10Filter == null)
                throw new ArgumentNullException("top10Filter");

            writer.WriteStartElement(PivotTable .Top10FilterElement );

            SerializeAttributeString(writer, PivotTable.PercentAttributeName ,
                top10Filter .IsPercent , false);
            SerializeAttributeString(writer, PivotTable.TopAttributeName,
               top10Filter.IsTop, true);
            writer.WriteAttributeString(PivotTable.FilterValueAttributeName, top10Filter.FilterValue.ToString ());
            writer.WriteAttributeString(PivotTable.ValAttibuteName, top10Filter.Value.ToString());

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serialize the conditional Formats of the pivot table.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize the pivot table into.</param>
        /// <param name="pivotTable">pivot table object to serialize.</param>
        private static void SerializeConditionalFormats(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (!pivotTable.PreservedElements.ContainsKey(PivotTable.ConditionalFormats))
                return;

            Stream stream = pivotTable.PreservedElements[PivotTable.ConditionalFormats];
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }



        private static void SerializePageFields(XmlWriter writer, PivotTableImpl pivotTable)
        {

            bool bFirst = true;
            bool bVersionCheck = false ;
            int iCount= pivotTable.PageFields.Count;
            if (pivotTable.Workbook.Version != ExcelVersion.Excel97to2003 
                && pivotTable.GetFields(PivotAxisTypes.Page).Count== pivotTable.PivotPageFields.Count)
            {
                bVersionCheck = true;
                iCount = pivotTable.PivotPageFields.Count;               
            }
            Dictionary<int, Dictionary<string, int>> allFieldValues = new Dictionary<int, Dictionary<string, int>>();
            if (pivotTable.PageFields.Count > 0)
            {
                List<int> lstFields = new List<int>();
                PivotTableFields fields = pivotTable.Fields;
                List<PivotFieldImpl> filteredFields = pivotTable.GetFields(PivotAxisTypes.Page);

                for (int i = 0, len = filteredFields.Count; i < len; i++)
                {
                    PivotFieldImpl field = filteredFields[i];

                    lstFields.Add(fields.IndexOf(field));
                }

                int key = 0;
                foreach (int fieldIndex in lstFields)
                {
                    PivotFieldImpl field = fields[fieldIndex];
                    SortedList<PivotTableSerializator.ComparisonPair, object> lstData = PivotTableSerializator.SortFieldValues(field.CacheField);
                    Dictionary<string, int> currentFieldValues = new Dictionary<string, int>();
                    int lstDataCount = 0;
                    while (lstDataCount < lstData.Count)
                    {
                        if (lstData.Keys[lstDataCount].Value != null)
                            currentFieldValues.Add(lstData.Keys[lstDataCount].Value.ToString(), lstDataCount);
                        lstDataCount++;
                    }
                    allFieldValues.Add(key++, currentFieldValues);
                }
            }
            for (int i = 0, len = iCount; i < len; i++)
            {
                PivotFieldImpl field = bVersionCheck ? (pivotTable.PivotPageFields[i] as PivotFieldImpl) : pivotTable.PageFields[i] as PivotFieldImpl ;

                if (field.Axis == PivotAxisTypes.Page)
                {
                    if (bFirst)
                    {
                        writer.WriteStartElement(PivotTable.PageFields);
                        bFirst = false;
                        //writer.WriteAttributeString( PivotTable.CountAttribute, pageFields.Count.ToString() );
                    }
                    if (field.PivotFilters[0]!= null)
                    {
                        Dictionary<string, int> fieldValues = allFieldValues[i];
                        if (fieldValues.ContainsKey(field.PivotFilters [0].Value1))
                            field.ItemIndex = fieldValues[field.PivotFilters[0].Value1.ToString()];
                    }
                    writer.WriteStartElement(PivotTable.PageField);
                    writer.WriteAttributeString(PivotTable.FieldAttribute, pivotTable.Fields.IndexOf(field).ToString());
                    if(field.ItemIndex > -1 && !field .IsMultiSelected)
                        writer.WriteAttributeString(PivotTable.FieldItem, field.ItemIndex.ToString());
                    writer.WriteAttributeString("hier", "-1");
                    writer.WriteEndElement();
                }
            }

            if (!bFirst)
                writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes pivot style.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize pivot style into.</param>
        /// <param name="pivotTable">Pivot table to serialize style for.</param>
        private static void SerializeStyle(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            // <pivotTableStyleInfo showRowHeaders="1" showColHeaders="1" showRowStripes="0" showColStripes="0"
            // showLastColumn="1"/>
            writer.WriteStartElement(PivotTable.StyleInfo);
            PivotBuiltInStyles? style = pivotTable.BuiltInStyle;

                if (style != null  )
                    writer.WriteAttributeString(PivotTable.NameAttribute, style.ToString());
                else if(pivotTable .CustomStyleName != null )
                    writer.WriteAttributeString(PivotTable.NameAttribute, pivotTable.CustomStyleName);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowRowHeaders, pivotTable.ShowRowHeaderStyle, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowColumnHeaders, pivotTable.ShowColHeaderStyle, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowRowStripes, pivotTable.ShowRowStripes, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowColumnStripes, pivotTable.ShowColStripes, false);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ShowLastColumn, pivotTable.ShowLastCol, false);

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes row fields.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize row fields into.</param>
        /// <param name="pivotTable">Pivot table to serialize row fields for.</param>
        private static void SerializeRowFields(XmlWriter writer, PivotTableImpl pivotTable)
        {
            SerializeFields(writer, pivotTable, PivotAxisTypes.Row, PivotTable.RowFields, PivotTable.RowItems, pivotTable.ShowDataFieldInRow);
        }
        /// <summary>
        /// Serailizes column fields.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="pivotTable">Pivot table to serialize column fields for.</param>
        private static void SerializeColumnFields(XmlWriter writer, PivotTableImpl pivotTable)
        {
            SerializeFields(writer, pivotTable, PivotAxisTypes.Column, PivotTable.ColumnFields, PivotTable.ColumnItems, !pivotTable.ShowDataFieldInRow);
        }
        /// <summary>
        /// Serializes fields (row or column).
        /// </summary>
        /// <param name="writer">XmlWriter to serialize fields into.</param>
        /// <param name="pivotTable">Pivot table to serialize fields for.</param>
        /// <param name="axis">Field axis to detect fields to serialize.</param>
        /// <param name="tagName">Tag name for the fields to use.</param>
        /// <param name="itemsTagName">Tag for single field.</param>
        /// <param name="bAddDataFields">Indicates whether to add field used for data fields.</param>
        private static void SerializeFields(XmlWriter writer, PivotTableImpl pivotTable,
          PivotAxisTypes axis, string tagName, string itemsTagName, bool bAddDataFields)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            List<int> lstFields = new List<int>();
            PivotTableFields fields = pivotTable.Fields;
            List<PivotFieldImpl> filteredFields = pivotTable.GetFields(axis);

            for (int i = 0, len = filteredFields.Count; i < len; i++)
            {
                PivotFieldImpl field = filteredFields[i];

                lstFields.Add(fields.IndexOf(field));
            }
            if (lstFields.Count > 0)
            {
                lstFields.Clear();
                if (axis == PivotAxisTypes.Row)
                    lstFields = pivotTable.RowFieldsOrder;
                else if (axis == PivotAxisTypes.Column)
                    lstFields = pivotTable.ColFieldsOrder;
            }
            if (bAddDataFields )
            {
                PivotDataFields dataFields = pivotTable.DataFields;

                if (dataFields.Count > 1)
                {
                    if(!lstFields .Contains (DataFieldsIndex))
                        lstFields.Add(DataFieldsIndex);
                }
            }
            int iFieldCount = lstFields.Count;

            if (iFieldCount > 0)
            {
                writer.WriteStartElement(tagName);

                for (int i = 0; i < iFieldCount; i++)
                {
                    int index = lstFields[i];
                    //PivotFieldImpl field = fields[ index ];
                    writer.WriteStartElement(PivotTable.Field);
                    writer.WriteAttributeString(PivotTable.IndexAttribute, index.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
                if(pivotTable .PivotEngineValues== null )
                SerializeColumnItems(writer, lstFields, itemsTagName, pivotTable);
#else
   SerializeColumnItems(writer, lstFields, itemsTagName, pivotTable);
#endif
            }
        }

        /// <summary>
        /// Checks the is equal.
        /// </summary>
        /// <param name="lstFields">The LST fields.</param>
        /// <param name="copyIndexs">The copy indexs.</param>
        private static void CheckIsEqual(List<int> lstFields, int[] copyIndexs)
        {
            if (lstFields.Count == copyIndexs.Length)
                return;

            if (lstFields.Count == 0 && copyIndexs.Length != 0)
                lstFields.AddRange(copyIndexs);

            for (int i = 0; i < copyIndexs.Length; i++)
            {
                if (copyIndexs[i] == -2)
                    lstFields.Add(-2);
            }
        }
        /// <summary>
        /// Serializes column items.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="lstFields">List of field indexes to serialize.</param>
        /// <param name="itemsTagName">Name of the main xml tag that will contain serialized fields.</param>
        /// <param name="table">Parent pivot table.</param>
        private static void SerializeColumnItems(XmlWriter writer, List<int> lstFields,
          string itemsTagName, PivotTableImpl table)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (lstFields == null)
                throw new ArgumentNullException("lstFields");

            if (itemsTagName == PivotTable.ColumnItems && table.ColumnItemsStream != null)
            {
                table.ColumnItemsStream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, table.ColumnItemsStream);
                return;
            }
            //TODO: Need to Serialize the row items 
            else if (itemsTagName == PivotTable.RowItems && table.RowItemsStream != null)
            {
                table.RowItemsStream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, table.RowItemsStream);
                return;
            }

            writer.WriteStartElement(itemsTagName);

            for (int i = 0, len = lstFields.Count; i < len; i++)
            {
                writer.WriteStartElement(PivotTable.Item);
                writer.WriteStartElement(PivotTable.ValueItem);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes data fields.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize fields into.</param>
        /// <param name="pivotTable">PivotTable to serialize data fields for.</param>
        private static void SerializeDataFields(XmlWriter writer, PivotTableImpl pivotTable)
        {
            
            List<int> lstIndexes = new List<int>();

            List<PivotFieldImpl> dataFields = new List<PivotFieldImpl> ();

            for (int i = 0; i < pivotTable.Fields.Count; i++)
            {
                int iFieldCount = 0;
                if (pivotTable.Fields[i].IsDataField)
                {
                    
                    dataFields.Add(pivotTable.Fields[i]);
                    for (int j = 0; j < pivotTable.DataFields.Count; j++)
                    {
                        if (pivotTable.DataFields[j].Field.CacheField.Index == i)
                        {
                            iFieldCount++;
                        }
                    }
                    if (iFieldCount > 1)
                    {
                        for (int k = 1; k < iFieldCount; k++)
                            dataFields.Add(pivotTable.Fields[i]);
                    }
                }
            }

            int iCount = dataFields.Count;

            if (iCount > 0)
            {
                writer.WriteStartElement(PivotTable.DataFields);
                writer.WriteAttributeString(PivotTable.CountAttribute, iCount.ToString());

                for (int i = 0; i < iCount; i++)
                {
                    writer.WriteStartElement(PivotTable.DataField);  
					//<dataField name="Sum of Title3" k baseField="0" baseItem="0" />                  
                    PivotFieldImpl dataField = dataFields[i];                    
                    string name = dataField.Name;
                    int index = dataField.CacheField.Index;
                    if (dataField.Name != pivotTable.DataFields[i].Name)
                    {
                        name = pivotTable.DataFields[i].Name;
                        index = pivotTable.DataFields[i].Field.CacheField.Index;
                    }
                    dataField = pivotTable.DataFields[i].Field;
                    writer.WriteAttributeString(PivotTable.NameAttribute, name);
                    writer.WriteAttributeString(PivotTable.CacheFieldIndex, index.ToString());
                    SerializeSubtotal(writer, pivotTable.DataFields[i].Subtotal);
                    if (pivotTable.DataFields[i].ShowDataAs != PivotFieldDataFormat.Normal && !pivotTable.DataFields[i].IsExcel2010Data())
                        writer.WriteAttributeString(PivotTable.ShowDataAs, pivotTable.DataFields[i].GetShowData(pivotTable.DataFields[i].ShowDataAs));
                    writer.WriteAttributeString(PivotTable.BaseField, pivotTable.DataFields[i].BaseField.ToString());
                    writer.WriteAttributeString(PivotTable.BaseItem, pivotTable.DataFields[i].BaseItem.ToString());
                    string FieldNumberFormatIndex = GetFieldNumberFormatIndex(pivotTable.DataFields[i].ShowDataAs, pivotTable.DataFields[i].Field.NumberFormatIndex.ToString());
                    if (FieldNumberFormatIndex != null )
                        writer.WriteAttributeString(PivotTable.NumberFormatAttribute, FieldNumberFormatIndex);
                   
                    if (pivotTable.DataFields[i].IsExcel2010Data())
                    {
                        writer.WriteStartElement(Excel2007Serializator.Extensionlist);
                        writer.WriteStartElement(Excel2007Serializator.Extension);
                        writer.WriteAttributeString(SparkConstants.UriAttribute, Excel2007Serializator.ExternListUri);
                        writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.X14Prefix, null, Excel2007Serializator.X14Namespace);
                        writer.WriteStartElement(Excel2007Serializator.X14Prefix, PivotTable.DataField, Excel2007Serializator.X14Namespace);
                        writer.WriteAttributeString(PivotTable.PivotShowAs, pivotTable.DataFields[i].GetShowData(pivotTable.DataFields[i].ShowDataAs));
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();

                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Return the Number Format Index Field.
        /// </summary>
        /// <param name="dataFormat"></param>
        /// <param name="FieldIndex"></param>
        /// <returns></returns>
        internal static string GetFieldNumberFormatIndex(PivotFieldDataFormat dataFormat , string FieldIndex)
        {
            switch (dataFormat)
            {
                case PivotFieldDataFormat.Difference:
                    FieldIndex = null;
                    break;
                case PivotFieldDataFormat.Percent:
                case PivotFieldDataFormat.PercentageOfColumn:
                case PivotFieldDataFormat.PercentageOfDifference:
                case PivotFieldDataFormat.PercentageOfParent:
                case PivotFieldDataFormat.PercentageOfParentColumn:
                case PivotFieldDataFormat.PercentageOfParentRow:
                case PivotFieldDataFormat.PercentageOfRow:
                case PivotFieldDataFormat .PercentageOfRunningTotal:
                case PivotFieldDataFormat.PercentageOfTotal:
                    FieldIndex = PercentageIndexFormat;
                    break;
            }
            return FieldIndex;
        }
        /// <summary>
        /// Serializes subtotal value.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="pivotSubtotalTypes">Subtotal type to serialize.</param>
        internal static void SerializeSubtotal(XmlWriter writer, PivotSubtotalTypes pivotSubtotalTypes)
        {
            if (pivotSubtotalTypes != PivotSubtotalTypes.Default && pivotSubtotalTypes != PivotSubtotalTypes.None)
            {
                PivotSubtotalTypes2007 subTotal = (PivotSubtotalTypes2007)pivotSubtotalTypes;
                writer.WriteAttributeString(PivotTable.Subtotal, subTotal.ToString());
            }
        }
        /// <summary>
        /// Serializes pivot fields.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize fields into.</param>
        /// <param name="pivotTable">Pivot table to serialize fields for.</param>
        private static List<PivotFieldImpl> SerializePivotFields(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            writer.WriteStartElement(PivotTable.PivotFields);
            PivotTableFields fields = pivotTable.Fields;
            List<PivotFieldImpl> pageFields = new List<PivotFieldImpl>();

            for (int i = 0, len = fields.Count; i < len; i++)
            {
                PivotFieldImpl field = fields[i];

                if (field.Axis == PivotAxisTypes.Page)
                {
                    pageFields.Add(field);
                }

                SerializePivotField(writer, field, pivotTable);
            }

            writer.WriteEndElement();
            return pageFields;
        }
        /// <summary>
        /// Serializes single pivot field.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize pivot field into.</param>
        /// <param name="field">Field to serialize.</param>
        private static void SerializePivotField(XmlWriter writer, PivotFieldImpl pivotField, PivotTableImpl table)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotField == null)
                throw new ArgumentNullException("field");

            writer.WriteStartElement(PivotTable.PivotField);

            if (pivotField.Name != pivotField.CacheField.Name)
                writer.WriteAttributeString(PivotTable.NameAttribute, pivotField.Name);

            if (pivotField.SubTotalName != null)
                writer.WriteAttributeString(PivotTable.SubTotalCaption, pivotField.SubTotalName);

            PivotAxisTypes axis = pivotField.Axis;

            if (axis != PivotAxisTypes.None && axis != PivotAxisTypes.Data)
            {
                PivotAxisTypes2007 axis2007 = (PivotAxisTypes2007)axis;
                writer.WriteAttributeString(PivotTable.AxisAttribute, axis2007.ToString());
            }

            SerializeAttributeString(writer, PivotTable.AutoShowAttribute,
                pivotField.IsAutoShow, false);

            PivotTableRowLayout layout = table.Options.RowLayout;
            if (layout == PivotTableRowLayout.Outline)
            {
                writer.WriteAttributeString(PivotTable.Compact, "0");
            }
            else if (layout == PivotTableRowLayout.Tabular)
            {
                writer.WriteAttributeString(PivotTable.Outline, "0");
                writer.WriteAttributeString(PivotTable.Compact, "0");
            }



            SerializeAttributeString(writer, PivotTable.DragOffAttribute,
                pivotField.CanDragOff, true);

            SerializeAttributeString(writer, PivotTable.DragToColAttribute,
                pivotField.CanDragToColumn, true);

            SerializeAttributeString(writer, PivotTable.DragToData,
                pivotField.CanDragToData, true);

            SerializeAttributeString(writer, PivotTable.DragToPage,
                pivotField.CanDragToPage, true);

            SerializeAttributeString(writer, PivotTable.DragToRow,
                pivotField.CanDragToRow, true);

            SerializeAttributeString(writer, PivotTable.HideNewItemAttribute,
                !pivotField.ShowNewItemsOnRefresh, true);

            SerializeAttributeString(writer, PivotTable.IncludeNewItemFilter,
                pivotField.ShowNewItemsInFilter, false);

            SerializeAttributeString(writer, PivotTable.InsertBlankRow,
                pivotField.ShowBlankRow, false);

            SerializeAttributeString(writer, PivotTable.InsertPageBreak,
                pivotField.ShowPageBreak, false);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ItemsPerPage,
                 pivotField.ItemsPerPage, 10);

            SerializeAttributeString(writer, PivotTable.MeasureFilterAttribute,
                pivotField.IsMeasureField, false);

            SerializeAttributeString(writer, PivotTable.MultiItemSelction,
                pivotField.IsMultiSelected, false);
            if(pivotField .m_table.Options .RowLayout !=  PivotTableRowLayout.Tabular)
                SerializeAttributeString(writer, PivotTable.Outline,
                    pivotField.ShowOutline, true);

            SerializeAttributeString(writer, PivotTable.ShowAllAttribute,
                pivotField.IsShowAllItems, true);

            SerializeAttributeString(writer, PivotTable.ShowDropDownAttribute,
                pivotField.ShowDropDown, false);

            SerializeAttributeString(writer, PivotTable.ShowPropAsCaption,
                pivotField.ShowPropAsCaption, false);

            SerializeAttributeString(writer, PivotTable.ShowPropToolTip,
                pivotField.ShowToolTip, false);

            SerializeAttributeString(writer, PivotTable.DefaultAttributeDrillState,
                pivotField.IsDefaultDrill, false);

            SerializeAttributeString(writer, PivotTable.DataSourceSort,
                pivotField.IsDataSourceSorted, false);

            SerializeAttributeString(writer, PivotTable.AllDrilled,
                pivotField.IsAllDrilled, false);

            if (pivotField.SortType != null)
                writer.WriteAttributeString(PivotTable.SortTypeAttribute, pivotField.SortType.ToString().ToLower());

            string uniqueName = pivotField.Caption;
            if (uniqueName != null && uniqueName.Length > 0)
                writer.WriteAttributeString(PivotTable.UniqueMemberProperty, uniqueName);


            Excel2007Serializator.SerializeAttribute(writer, PivotTable.NumberFormatAttribute, pivotField.NumberFormatIndex, 0);

            if (pivotField.IsDataField)
                SerializeDataField(writer, pivotField);

            SerializeSubtotalFlags(writer, pivotField.Subtotals);

            if (axis != PivotAxisTypes.Data && axis != PivotAxisTypes.None)
            {
                SerializeOrdinaryField(writer, pivotField);
            }
            if (pivotField.FutureDataStorageStream != null)
            {
                pivotField.FutureDataStorageStream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, pivotField.FutureDataStorageStream);
            }
            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes subtotal flags as set of pivot field attributes.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="subtotal">Subtotal options to serialize.</param>
        private static void SerializeSubtotalFlags(XmlWriter writer, PivotSubtotalTypes subtotal)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (subtotal == PivotSubtotalTypes.Default)
                return;

            if (subtotal == PivotSubtotalTypes.None)
            {
                Excel2007Serializator.SerializeAttribute(writer, PivotTable.DefaultSubtotal, false, true);
            }
            else
            {
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Sum, false, PivotTable.SumSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Counta, false, PivotTable.CountASubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Average, false, PivotTable.AverageSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Max, false, PivotTable.MaxSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Min, false, PivotTable.MinSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Product, false, PivotTable.ProductSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Count, false, PivotTable.CountSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Stdev, false, PivotTable.StdDevSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Stdevp, false, PivotTable.StdDevPSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Var, false, PivotTable.VarSubtotal);
                SerializeSubtotalFlags(writer, subtotal, PivotSubtotalTypes.Varp, false, PivotTable.VarPSubtotal);
            }
        }
        /// <summary>
        /// Serializes single subtotal flag if necessary.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="subtotal">Subtotal options to serialize.</param>
        /// <param name="subtotalItem">Single subtotal option to check.</param>
        /// <param name="defaultValue">Default option value.</param>
        /// <param name="attributeName">Attribute name to store option value if necessary.</param>
        private static void SerializeSubtotalFlags(XmlWriter writer, PivotSubtotalTypes subtotal,
          PivotSubtotalTypes subtotalItem, bool defaultValue, string attributeName)
        {
            bool bFlagValue = (subtotal & subtotalItem) != 0;
            Excel2007Serializator.SerializeAttribute(writer, attributeName, bFlagValue, defaultValue);
        }
        /// <summary>
        /// Serializes data field special attributes.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize data field into.</param>
        /// <param name="field">Field to serialize.</param>
        private static void SerializeDataField(XmlWriter writer, PivotFieldImpl field)
        {
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.DataFieldAttribute, field.IsDataField, false);
        }
        /// <summary>
        /// Serializes ordinary (not data) field special attributes.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="field">Field to serialize.</param>
        private static void SerializeOrdinaryField(XmlWriter writer, PivotFieldImpl field)
        {
            SortedList<ComparisonPair, object> lstData = SortFieldValues(field.CacheField);
            bool writeEndElement = false;
            Dictionary<int, PivotItemOptions> items = field.ItemOptions;
            IList<object> values = field.CacheField.Items;
            if (items == null)
            {
                for (int i = 0; i < lstData.Count; i++)
                {
                    field.ItemOptions.Add(lstData.Keys[i].Index, null);
                }
                PivotItemOptions itemOption = new PivotItemOptions();
                itemOption.ItemType = PivotItemType.Default;
                field.ItemOptions.Add(-1, itemOption);
            }
            if (items.Count > 0)
            {
                int currentItemIndex = 0; bool checkHidden = true ;
                writer.WriteStartElement(PivotTable.FieldItems);
                writeEndElement = true;
                foreach (KeyValuePair<int, PivotItemOptions> item in items)
                {
                    if (!(field.Subtotals == PivotSubtotalTypes.None && item.Key == -1))
                    {
                        writer.WriteStartElement(PivotTable.FieldItem);
                        if (item.Key != -1)
                            writer.WriteAttributeString(PivotTable.IndexAttribute, item.Key.ToString());
                        if (field.Items.Count > 0 && (!field.Items[currentItemIndex].Visible && (field.Axis == PivotAxisTypes.Page || field.Axis == PivotAxisTypes.Row || field.Axis == PivotAxisTypes.Column) && checkHidden && field.IsMultiSelected))
                        {
                            writer.WriteAttributeString(PivotTable.HiddenAttribute, Excel2007Serializator.TrueValue);
                            if (item.Value != null)
                                item.Value.IsHidden = false;
                        }
                        if (item.Value != null)
                        {

                            SerializeFieldItem(writer, item.Key, item.Value);
                        }
                        writer.WriteEndElement();
                        if (field.Items.Count > currentItemIndex + 1)
                            currentItemIndex++;
                        else
                            checkHidden = false;
                    }
                }
            }
            else if (lstData.Count > 0)
            {
                writer.WriteStartElement(PivotTable.FieldItems);
                writeEndElement = true;
                for (int i = 0, len = lstData.Count; i < len; i++)
                {
                    writer.WriteStartElement(PivotTable.FieldItem);
                    writer.WriteAttributeString(PivotTable.IndexAttribute, lstData.Keys[i].Index.ToString());
                   if (field.Items.Count > 0)
                    {
                        if (!field.Items[i].Visible && field.IsMultiSelected)
                            writer.WriteAttributeString(PivotTable.HiddenAttribute, Excel2007Serializator.TrueValue);
                    }
                  writer.WriteEndElement();
                }
            }
            if (!(lstData.Count > 0 || items.Count > 0))
                writer.WriteStartElement(PivotTable.FieldItems);
            SerializeSubtotalItems(writer, field.Subtotals);
            if (!(lstData.Count > 0 || items.Count > 0))
                writer.WriteEndElement();
            
            if (writeEndElement)
                writer.WriteEndElement();
            SerializeAutoSortScope(writer, field);
        }
        /// <summary>
        /// Serializes the sorting scope of the pivot field.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="field">pivot field obj</param>
        private static void SerializeAutoSortScope(XmlWriter writer, PivotFieldImpl field)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (field == null)
                throw new ArgumentNullException("field");

            Stream stream = field.PreservedAutoSort;
            if (stream != null)
            {
                stream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, stream);
            }
        }
        private static void SerializeFieldItem(XmlWriter writer, int index, PivotItemOptions item)
        {
            if (index == -1)
            {
                SerializeSubtotalItems(writer, item.ItemType);
                return;
            }

            if (item.HasChildItems)
                writer.WriteAttributeString(PivotTable.ChildItemsAttribute, Excel2007Serializator.TrueValue);

            if (item.IsExpaned)
                writer.WriteAttributeString(PivotTable.ExpandAttribute, Excel2007Serializator.TrueValue);

            if (item.DrillAcross)
                writer.WriteAttributeString(PivotTable.DrillAcrossAtribute, Excel2007Serializator.TrueValue);

            if (item.IsCalculatedItem)
                writer.WriteAttributeString(PivotTable.CalculatedMemberAttribute, Excel2007Serializator.TrueValue);

            if (item.IsHidden)
                writer.WriteAttributeString(PivotTable.HiddenAttribute, Excel2007Serializator.TrueValue);

            if (item.IsMissing)
                writer.WriteAttributeString(PivotTable.MissingAttribute, Excel2007Serializator.TrueValue);

            if (item.UserCaption != null)
                writer.WriteAttributeString(PivotTable.ItemCaptionAttribute, item.UserCaption);

            if (item.IsChar)
                writer.WriteAttributeString(PivotTable.CharAttribute, Excel2007Serializator.TrueValue);

            if (item.IsHiddenDetails)
                writer.WriteAttributeString(PivotTable.HideDetailAttribute, Excel2007Serializator.TrueValue);

        }
        /// <summary>
        /// Serializes subtotal items.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="subtotal">Subtotal values to serialize.</param>
        private static void SerializeSubtotalItems(XmlWriter writer, PivotItemType subtotal)
        {
            string strItemType;
            PivotSubtotalItems2007[] arrPossibleTypes =
#if !SILVERLIGHT && !WINRT && !WP
 (PivotSubtotalItems2007[])Enum.GetValues(
              typeof(PivotItemType2007));
#else
PivotSubtotalItems2007Extension.GetValues();
            //PivotItemType2007Extension.GetValues();
#endif

            PivotItemType2007 subtotal2007 = (PivotItemType2007)subtotal;
            if (subtotal2007 == PivotItemType2007.defaults)
                writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, PivotTable.FieldSummaryDefault);
            else
                writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, subtotal2007.ToString());

        }
        //    /// <summary>
        //    /// Serializes ordinary (not data) field special attributes.
        //    /// </summary>
        //    /// <param name="writer">XmlWriter to serialize into.</param>
        //    /// <param name="field">Field to serialize.</param>
        //    private static void SerializeOrdinaryField( XmlWriter writer, PivotFieldImpl field )
        //    {
        //      SortedList<ComparisonPair, object> lstData = SortFieldValues( field.CacheField );

        //      if( lstData.Count > 0 )
        //      {
        //        writer.WriteStartElement( PivotTable.FieldItems );

        //        for( int i = 0, len = lstData.Count; i < len; i++ )
        //        {
        //          writer.WriteStartElement( PivotTable.FieldItem );
        //          writer.WriteAttributeString( PivotTable.IndexAttribute, lstData.Keys[ i ].Index.ToString() );
        //          writer.WriteEndElement();
        //        }

        //        SerializeSubtotalItems( writer, field.Subtotals );

        //        writer.WriteEndElement();
        //      }
        //    }
        /// <summary>
        /// Serializes subtotal items.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="subtotal">Subtotal values to serialize.</param>
        private static void SerializeSubtotalItems(XmlWriter writer, PivotSubtotalTypes subtotal)
        {
            if (subtotal != PivotSubtotalTypes.None)
            {
                if (subtotal == PivotSubtotalTypes.Default)
                {
                    writer.WriteStartElement(PivotTable.FieldItem);
                    writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, PivotTable.FieldSummaryDefault);
                    writer.WriteEndElement();
                }
                else
                {
                    PivotSubtotalItems2007[] arrPossibleTypes =
#if !SILVERLIGHT && !WINRT && !WP
 (PivotSubtotalItems2007[])Enum.GetValues(
                      typeof(PivotSubtotalItems2007));
#else
                    PivotSubtotalItems2007Extension.GetValues();
#endif

                    PivotSubtotalItems2007 subtotal2007 = (PivotSubtotalItems2007)subtotal;

                    for (int i = 0, len = arrPossibleTypes.Length; i < len; i++)
                    {
                        PivotSubtotalItems2007 currentType = arrPossibleTypes[i];

                        if (currentType != 0)
                        {
                            if ((subtotal2007 & currentType) != 0)
                            {
                                writer.WriteStartElement(PivotTable.FieldItem);
                                writer.WriteAttributeString(PivotTable.FieldSummaryTypeAttibute, currentType.ToString());
                                writer.WriteEndElement();
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sorts field values.
        /// </summary>
        /// <param name="field">Cache field to sort items for.</param>
        /// <returns>Sorted list with field values: key - field value, value - item index.</returns>
        public static SortedList<ComparisonPair, object> SortFieldValues(PivotCacheFieldImpl field)
        {
            SortedList<ComparisonPair, object> result = new SortedList<ComparisonPair, object>();

            for (int i = 0, len = field.ItemCount; i < len; i++)
            {
                object value = field.GetValue(i);

                if (Convert .ToString (value ).Length >=0)
                {
                    //value = null;

                    ComparisonPair pair = new ComparisonPair();
                    pair.Value = value;
                    pair.Index = i;

                    result[pair] = null;
                }
            }

            return result;
        }
        /// <summary>
        /// Serializes pivot table location.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize location into.</param>
        /// <param name="pivotTable">Pivot table to serialize location for.</param>
        private static void SerializeLocation(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            //<location ref="A3:C5" firstHeaderRow="1" firstDataRow="2" firstDataCol="0" /> 
            writer.WriteStartElement(PivotTable.Location);
            string[] words = pivotTable .Location .AddressLocal.Split(':');
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
            if (pivotTable.PivotEngineValues == null || words.Length == 2)
                writer.WriteAttributeString(PivotTable.ReferenceAddress, pivotTable.Location.AddressLocal);
            else if (pivotTable.EndLocation != null)
                writer.WriteAttributeString(PivotTable.ReferenceAddress, pivotTable.Location.AddressLocal + ":" + pivotTable.EndLocation.AddressLocal);
            else
                writer.WriteAttributeString(PivotTable.ReferenceAddress, pivotTable.Location.AddressLocal);
#else
            writer.WriteAttributeString(PivotTable.ReferenceAddress, pivotTable.Location.AddressLocal);
#endif
            
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.ColumnsPerPage, pivotTable.ColumnsPerPage, 0);

            Excel2007Serializator.SerializeAttribute(writer, PivotTable.RowsPerPage, pivotTable.RowsPerPage, 0);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FirstHeaderRow, pivotTable.FirstHeaderRow, -1);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FirstDataRow, pivotTable.FirstDataRow, -1);
            Excel2007Serializator.SerializeAttribute(writer, PivotTable.FirstDataColumn, pivotTable.FirstDataCol, -1);

            writer.WriteEndElement();
        }
        /// <summary>
        /// Serializes  the OLAP hierarchies associated with the PivotTable.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the chart formats into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializePivotHierarchies(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (!pivotTable.PreservedElements.ContainsKey(PivotTable.PivotHierarchies))
                return;

            Stream stream = pivotTable.PreservedElements[PivotTable.PivotHierarchies];
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }
        /// <summary>
        /// Serializes references to OLAP hierarchies on the row axis of a PivotTable.
        /// </summary>
        /// <param name="writer">Xml writer to serialize the chart formats into.</param>
        /// <param name="pivotTable">pivot table to serialize the filters</param>
        private static void SerializeRowHierarchies(XmlWriter writer, PivotTableImpl pivotTable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (pivotTable == null)
                throw new ArgumentNullException("pivotTable");

            if (!pivotTable.PreservedElements.ContainsKey(PivotTable.RowHierarchiesUsage))
                return;

            Stream stream = pivotTable.PreservedElements[PivotTable.RowHierarchiesUsage];
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }
        /// <summary>
        /// This class represents value-index pair and used to sort pivot field values.
        /// </summary>
        public class ComparisonPair : IComparable
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

        #region Helper Methods
        internal static void SerializeAttributeString(XmlWriter writer, string attributeName, bool value, bool defaultValue)
        {
            Excel2007Serializator.SerializeAttribute(writer, attributeName, value, defaultValue);
        }
        internal static void SerializeAttributeString(XmlWriter writer, string attributeName, byte value)
        {
            writer.WriteAttributeString(attributeName, XmlConvert.ToString(value));
        }
        internal static void SerializeAttributeString(XmlWriter writer, string attributeName, ushort value)
        {
            writer.WriteAttributeString(attributeName, XmlConvert.ToString(value));
        }
        internal static void SerializeAttributeString(XmlWriter writer, string attributeName, uint value)
        {
            writer.WriteAttributeString(attributeName, XmlConvert.ToString(value));
        }
        internal static void SerializeAttributeString(XmlWriter writer, string attributeName, int value)
        {
            writer.WriteAttributeString(attributeName, XmlConvert.ToString(value));
        }
        internal static bool IsDataFieldsInRow(PivotTableImpl table)
        {
            for (int i = 0, len = table.ColumnFields.Count; i < len; i++)
            {
                PivotFieldImpl field = table.ColumnFields[i] as PivotFieldImpl;
                if (field.IsDataField)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
