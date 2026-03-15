#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
    sealed class PivotTable
    {
        #region Constants
        /// <summary>
        /// Specifies a boolean value that indicates whether the application will
        /// refresh the cache when the workbook has been opened.
        /// </summary>
        public const string RefreshOnLoad = "refreshOnLoad";
        /// <summary>
        /// Specifies the name of the end-user who last refreshed the cache.
        /// </summary>
        public const string RefreshedBy = "refreshedBy";
        /// <summary>
        /// Specifies the date when the cache was last refreshed.
        /// </summary>
        public const string RefreshedDate = "refreshedDate";
        /// <summary>
        /// Specifies the version of the application that created the cache.
        /// </summary>
        public const string CreatedVersion = "createdVersion";
        /// <summary>
        /// Specifies the version of the application that last refreshed the cache.
        /// </summary>
        public const string RefreshedVersion = "refreshedVersion";
        /// <summary>
        /// Specifies the earliest version of the application that is required to refresh the cache.
        /// </summary>
        public const string MinRefreshableVersion = "minRefreshableVersion";
        /// <summary>
        /// Specifies the number of records in the cache.
        /// </summary>
        public const string RecordCount = "recordCount";
        /// <summary>
        /// Represents the description of data source whose data is stored in the pivot cache.
        /// </summary>
        public const string CacheSource = "cacheSource";
        /// <summary>
        /// Specifies the cache type.
        /// </summary>
        public const string TypeAttribute = "type";
        /// <summary>
        /// Indicates that the cache contains data that
        /// consolidates ranges.
        /// </summary>
        public const string ConsolidationTypeValue = "consolidation";
        /// <summary>
        /// Indicates that the cache contains data from an
        /// external data source.
        /// </summary>
        public const string ExternalTypeValue = "external";
        /// <summary>
        /// Indicates that the cache contains a scenario summary
        /// report  
        /// </summary>
        public const string ScenarioTypeValue = "scenario";
        /// <summary>
        /// Indicates that the cache contains worksheet data.   
        /// </summary>
        public const string WorksheetSourceType = "worksheet";
        /// <summary>
        /// Represents the location of the source of the data that is stored in the cache.
        /// </summary>
        public const string WorksheetSource = "worksheetSource";
        /// <summary>
        /// Specifies the reference that defines a cell range that is the source of the data.
        /// </summary>
        public const string ReferenceAddress = "ref";
        /// <summary>
        /// Specifies the name of the sheet that is the source for the cached data.
        /// </summary>
        public const string SheetAttribute = "sheet";
        /// <summary>
        /// Represents the collection of field definitions in the source data.
        /// </summary>
        public const string CacheFields = "cacheFields";
        /// <summary>
        /// Specifies the number of fields in the cache.
        /// </summary>
        public const string CountAttribute = "count";
        /// <summary>
        /// Represent a single field in the PivotCache.
        /// </summary>
        public const string CacheField = "cacheField";
        /// <summary>
        /// Specifies the name of the cache field.
        /// </summary>
        public const string NameAttribute = "name";
        /// <summary>
        /// Represents the PivotTable root element for non-null PivotTables.
        /// </summary>
        public const string PivotTableDefinition = "pivotTableDefinition";
        /// <summary>
        /// This element enumerates pivot cache definition parts used by pivot tables and formulas in this workbook.
        /// </summary>
        public const string PivotCachesTag = "pivotCaches";
        /// <summary>
        /// This element represents a cache of data for pivot tables and formulas in the workbook.
        /// </summary>
        public const string PivotCacheTag = "pivotCache";
        /// <summary>
        /// Specifies the unique identifier for the pivot cache for this workbook in the pivot cache part.
        /// </summary>
        public const string PivotCacheId = "cacheId";
        public const string ApplyNumberFormats = "applyNumberFormats";
        public const string ApplyBorderFormats = "applyBorderFormats";
        public const string ApplyFontFormats = "applyFontFormats";
        public const string ApplyPatternFormats = "applyPatternFormats";
        public const string ApplyAlignmentFormats = "applyAlignmentFormats";
        public const string ApplyWidthHeightFormats = "applyWidthHeightFormats";
        /// <summary>
        /// Specifies the name of the value area field header in the PivotTable.
        /// </summary>
        public const string DataCaption = "dataCaption";
        /// <summary>
        /// Specifies the version of the application that last updated the PivotTable view.
        /// </summary>
        public const string UpdatedVersion = "updatedVersion";
        /// <summary>
        /// Specifies a boolean value that indicates whether calculated members should be shown
        /// in the PivotTable view. This attribute applies to PivotTables from OLAP-sources only.
        /// </summary>
        public const string ShowCalcMbrs = "showCalcMbrs";
        /// <summary>
        /// Specifies a boolean value that indicates whether auto formatting has
        /// been applied to the PivotTable view.
        /// </summary>
        public const string UseAutoFormatting = "useAutoFormatting";
        /// <summary>
        /// Specifies a boolean value that indicates whether PivotItem names should
        /// be repeated at the top of each printed page.
        /// </summary>
        public const string ItemPrintTitles = "itemPrintTitles";
        /// <summary>
        /// Specifies the indentation increment for compact axis and can be used to
        /// set the Report Layout to Compact Form.
        /// </summary>
        public const string Indent = "indent";
        /// <summary>
        /// Specifies a boolean value that indicates whether new fields should have
        /// their outline flag set to true.
        /// </summary>
        public const string Outline = "outline";
        /// <summary>
        /// Specifies a boolean value that indicates whether data fields in the
        /// PivotTable should be displayed in outline form.
        /// </summary>
        public const string OutlineData = "outlineData";
        /// <summary>
        /// Specifies a boolean value that indicates whether the fields of a PivotTable
        /// can have multiple filters set on them.
        /// </summary>
        public const string MultipleFieldFilters = "multipleFieldFilters";
        /// <summary>
        /// Represents the collection of fields that appear on the PivotTable.
        /// </summary>
        public const string PivotFields = "pivotFields";
        /// <summary>
        /// Represents a single field in the PivotTable.
        /// </summary>
        public const string PivotField = "pivotField";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show all items for this field.
        /// </summary>
        public const string ShowAllAttribute = "showAll";
        /// <summary>
        /// Represents location information for the PivotTable.
        /// </summary>
        public const string Location = "location";
        /// <summary>
        /// Specifies the first row of the PivotTable header, relative to the top
        /// left cell in the ref value.
        /// </summary>
        public const string FirstHeaderRow = "firstHeaderRow";
        /// <summary>
        /// Specifies the first row of the PivotTable data, relative to the top left cell in the ref value.
        /// </summary>
        public const string FirstDataRow = "firstDataRow";
        /// <summary>
        /// Specifies the first column of the PivotTable data, relative to the top left cell in the ref value.
        /// </summary>
        public const string FirstDataColumn = "firstDataCol";
        /// <summary>
        /// Represents the pivotCacheDefinition part. This part defines each field
        /// in the source data, including the name, the string resources of the
        /// instance data (for shared items), and information about the type of
        /// data that appears in the field.
        /// </summary>
        public const string PivotCacheDefinition = "pivotCacheDefinition";
        /// <summary>
        /// Represents the collection of row fields for the PivotTable.
        /// </summary>
        public const string RowFields = "rowFields";
        /// <summary>
        /// Represents the collection of items in row axis of the PivotTable.
        /// </summary>
        public const string RowItems = "rowItems";
        /// <summary>
        /// Represents the collection of fields that are on the column axis of the PivotTable.
        /// </summary>
        public const string ColumnFields = "colFields";
        /// <summary>
        /// Represents the collection of column items of the PivotTable.
        /// </summary>
        public const string ColumnItems = "colItems";
        /// <summary>
        /// Specifies the index to the number format applied to this data field.
        /// </summary>
        public const string NumberFormatAttribute = "numFmtId";
        /// <summary>
        /// Represent information on style applied to the PivotTable.
        /// </summary>
        public const string StyleInfo = "pivotTableStyleInfo";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row headers for the table.
        /// </summary>
        public const string ShowRowHeaders = "showRowHeaders";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column headers for the table.
        /// </summary>
        public const string ShowColumnHeaders = "showColHeaders";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row
        /// stripe formatting for the table.
        /// </summary>
        public const string ShowRowStripes = "showRowStripes";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column stripe
        /// formatting for the table.
        /// </summary>
        public const string ShowColumnStripes = "showColStripes";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the last column.
        /// </summary>
        public const string ShowLastColumn = "showLastColumn";
        /// <summary>
        /// Specifies a boolean value that indicates whether grand totals should be
        /// displayed for the PivotTable columns.
        /// </summary>
        public const string ColumnGrandTotal = "colGrandTotals";
        /// <summary>
        /// Specifies a boolean value that indicates whether grand totals should be
        /// displayed for the PivotTable rows.
        /// </summary>
        public const string RowGrandTotal = "rowGrandTotals";
        /// <summary>
        /// Specifies a boolean value that indicates whether drill indicators should be hidden.
        /// </summary>
        public const string ShowDrill = "showDrill";
        /// <summary>
        /// Represents the collection of unique items for a field in the PivotCacheDefinition.
        /// </summary>
        public const string SharedItems = "sharedItems";
        public const string ContainsMixedTypes = "containsMixedTypes";
        /// <summary>
        /// Specifies a boolean value that indicates that this field contains text values.
        /// The field may also contain a mix of other data type and blank values.
        /// </summary>
        public const string ContainsSemiMixedTypes = "containsSemiMixedTypes";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field contains a text value.
        /// </summary>
        public const string ContainsString = "containsString";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field contains numeric values.
        /// </summary>
        public const string ContainsNumber = "containsNumber";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field contains integer values.
        /// </summary>
        public const string ContainsInteger = "containsInteger";
        /// <summary>
        /// Specifies a boolean value that indicates that the field contains at least one date.
        /// </summary>
        public const string ContainsDate = "containsDate";
        /// <summary>
        /// Specifies a boolean value that indicates that the field contains at
        /// least one value that is not a date.
        /// </summary>
        public const string ContainsNonDate = "containsNonDate";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field contains a blank value.
        /// </summary>
        public const string ContainsBlank = "containsBlank";
        /// <summary>
        /// Specifies a boolean value that indicates wheter this field contains a long Text.
        /// </summary>
        public const string LongText = "longText";
        /// <summary>
        /// Specifies a boolean value that indicates whether to suppress display of pivot field.
        /// </summary>
        public const string ShowHeaders = "showHeaders";
        /// <summary>
        /// Represents a generic field that can appear either on the column or the row region of the PivotTable.
        /// </summary>
        public const string Field = "field";
        /// <summary>
        /// Specifies the index to a pivotField item value.
        /// </summary>
        public const string IndexAttribute = "x";
        /// <summary>
        /// Represents the collection of items in the row or column region of the PivotTable.
        /// </summary>
        public const string Item = "i";
        /// <summary>
        /// GrandTotal constant
        /// </summary>
        public const string GrandTotalAttribute = "grand";
        /// <summary>
        /// Row Labels of pivot table
        /// </summary>
        public const string RowLabels = "Row Labels";
        /// <summary>
        /// Specifies the type of the item.
        /// </summary>
        public const string RepeatItemsCount = "r";
        /// <summary>
        /// constant
        /// </summary>
        public const string DefaultConst = "default";
        /// <summary>
        /// Represents an array of indexes to cached member property values.
        /// </summary>
        public const string ValueItem = "x";
        /// <summary>
        /// Represents the collection of items in the data region of the PivotTable.
        /// </summary>
        public const string DataFields = "dataFields";
        /// <summary>
        /// Represents a field from a source list, table, or database that contains data that is summarized in a PivotTable.
        /// </summary>
        public const string DataField = "dataField";
        /// <summary>
        /// Specifies the index to the field (&lt;r&gt;) in the pivotCacheRecords part that this data item summarizes.
        /// </summary>
        public const string CacheFieldIndex = "fld";
        /// <summary>
        /// Specifies the index to the base field when the ShowDataAs calculation is in use.
        /// </summary>
        public const string BaseField = "baseField";
        /// <summary>
        /// Specifies the index to the base item when the ShowDataAs calculation is in use.
        /// </summary>
        public const string BaseItem = "baseItem";
        /// <summary>
        /// Specifies the aggregation function that applies to this data field.
        /// </summary>
        public const string Subtotal = "subtotal";
        public const string ShowDataAs = "showDataAs";
        public const string PivotShowAs = "pivotShowAs";
        /// <summary>
        /// Specifies the custom text that is displayed for the subtotals label.
        /// </summary>
        public const string SubTotalCaption = "subtotalCaption";
        /// <summary>
        /// Specifies the region of the PivotTable that this field is displayed.
        /// </summary>
        public const string AxisAttribute = "axis";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field appears in the
        /// data region of the PivotTable.
        /// </summary>
        public const string DataFieldAttribute = "dataField";
        /// <summary>
        /// Represents the collection of items in a PivotTable field.
        /// </summary>
        public const string FieldItems = "items";
        /// <summary>
        /// Represents a single item in PivotTable field.
        /// </summary>
        public const string FieldItem = "item";
        /// <summary>
        /// Specifies the type of this item. A value of 'default' indicates the subtotal or total item.
        /// </summary>
        public const string FieldSummaryTypeAttibute = "t";
        /// <summary>
        /// Indicates the pivot item represents the default type for this PivotTable.
        /// The default pivot item type is the "total" aggregate function.
        /// </summary>
        public const string FieldSummaryDefault = "default";
        /// <summary>
        /// Represents the collection of records in the PivotCache.
        /// </summary>
        public const string PivotCacheRecords = "pivotCacheRecords";
        /// <summary>
        /// Represents a single record of data in the PivotCache.
        /// </summary>
        public const string PivotCacheRecord = "r";
        /// <summary>
        /// Represents a numeric value in the PivotTable.
        /// </summary>
        public const string NumberTag = "n";
        /// <summary>
        /// Represents a character value in a PivotTable.
        /// </summary>
        public const string StringTag = "s";
        /// <summary>
        /// Represents a value that was not specified.
        /// </summary>
        public const string EmptyTag = "m";
        /// <summary>
        /// Represents a date-time value in the PivotTable.
        /// </summary>
        public const string DateTag = "d";
        /// <summary>
        /// Represents a boolean value for an item in the PivotTable.
        /// </summary>
        public const string BooleanTag = "b";
        /// <summary>
        /// Unified date time format.
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss";
        /// <summary>
        /// Specifies the value of the item.
        /// </summary>
        public const string ValueAttribute = "v";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'Average'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string AverageSubtotal = "avgSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'countA'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string CountASubtotal = "countASubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'count'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string CountSubtotal = "countSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'max'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string MaxSubtotal = "maxSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'min'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string MinSubtotal = "minSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply 'product'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string ProductSubtotal = "productSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'stdDevP'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string StdDevPSubtotal = "stdDevPSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to use 'stdDev'
        /// in the subtotal of this field.
        /// </summary>
        public const string StdDevSubtotal = "stdDevSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether apply the 'sum'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string SumSubtotal = "sumSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'varP'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string VarPSubtotal = "varPSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether to apply the 'variance'
        /// aggregation function in the subtotal of this field.
        /// </summary>
        public const string VarSubtotal = "varSubtotal";
        /// <summary>
        /// Specifies a boolean value that indicates whether the default subtotal
        /// aggregation function is displayed for this field.
        /// </summary>
        public const string DefaultSubtotal = "defaultSubtotal";
        public const string PageFields = "pageFields";
        public const string PageField = "pageField";
        public const string FieldAttribute = "fld";
        /// <summary>
        /// Specifies a boolean value that indicates whether the application should query and
        /// retrieve records asynchronously from the cache.
        /// </summary>
        public const string BackgroundQueryAttribute = "backgroundQuery";
        /// <summary>
        /// Specifies a boolean value that indicates whether the user can refresh the cache.
        /// </summary>
        public const string EnableRefreshAttribute = "enableRefresh";
        /// <summary>   
        /// Specifies a boolean value that indicates whether the cache needs to be refreshed.
        /// </summary>
        public const string InvalidAttribute = "invalid";
        /// <summary>
        /// Specifies a boolean value that indicates whether the application will apply optimizations
        /// to the cache to reduce memory usage
        /// </summary>
        public const string OptimizeMemoryAttribute = "optimizeMemory";
        /// <summary>
        /// Specifies a boolean value that indicates whether the pivot records are saved with the
        /// cache.
        /// </summary>
        public const string SaveDataAttribute = "saveData";
        /// <summary>
        /// Specifies whether the cache's data source supports attribute drilldown
        /// </summary>
        public const string SupportAdvancedDrillAttribute = "supportAdvancedDrill";
        /// <summary>
        /// Specifies whether the cache's data source supports subqueries.
        /// </summary>
        public const string SupportSubQueryAttribute = "supportSubquery";
        /// <summary>
        /// Specifies a boolean value that indicates whether the cache is scheduled for version
        /// upgrade.
        /// </summary>
        public const string UpgradeOnRefreshAttribute = "upgradeOnRefresh";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field came from the source
        /// database
        /// </summary>
        public const string DatabaseFieldAttribute = "databaseField";
        /// <summary>
        /// Specifies the formula for the calculated field
        /// </summary>
        public const string FormulaAttribute = "formula";
        /// <summary>
        /// Represents the collection of properties for a field group.
        /// </summary>
        public const string FieldGroupElement = "fieldGroup";
        /// <summary>
        /// Specifies a boolean value that indicates whether an asterisks should be displayed in
        ///subtotals and totals
        /// </summary>
        public const string AsteriskTotalAttribute = "asteriskTotals";
        /// <summary>
        /// Specifies the string to be displayed in column header in compact mode. This attribute
        ///depends on whether the application implements a compact mode for displaying
        ///PivotTables in the user interface.
        /// </summary>
        public const string ColumnHeaderCaption = "colHeaderCaption";
        /// <summary>
        /// Specifies the compact new fileds
        /// </summary>
        public const string Compact = "compact";
        /// <summary>
        /// Specifies the display compact Data
        /// </summary>
        public const string CompactData = "compactData";
        /// <summary>
        /// This attribute indicates the wheater to auto sort pivot table or not
        /// </summary>
        public const string CustomListSort = "customListSort";
        /// <summary>
        /// This attribute represents the multiple fields in the data region 
        /// is located in the row area or the column area
        /// </summary>
        public const string DataOnRows = "dataOnRows";
        /// <summary>
        /// This attribute represents the position for the field which representing multiple data field in the PivotTable
        /// </summary>
        public const string DataPosition = "dataPosition";
        /// <summary>
        /// this attribute indicates whether to disable the PivotTable field list.
        /// </summary>
        public const string DisableFieldList = "disableFieldList";
        /// <summary>
        /// This attribute that indicates whether the user is allowed to edit the cells in
        ///the data area of the PivotTable.
        /// </summary>
        public const string AllowEditData = "editData";
        /// <summary>
        /// This attribute that indicates whether the user is prevented from drilling down
        ///on a PivotItem or aggregate value.
        /// </summary>
        public const string EnableDrillDown = "enableDrill";
        /// <summary>
        /// indicates whether the user is prevented from displaying
        ///PivotField properties.
        /// </summary>
        public const string EnableFieldProerties = "enableFieldProperties";
        /// <summary>
        /// This attribute that indicates whether the user is prevented from displaying
        ///the PivotTable wizard.
        /// </summary>
        public const string EnableWizard = "enableWizard";
        /// <summary>
        /// This attribute Specifies the string to be displayed in cells that contain errors.
        /// </summary>
        public const string ErrorCaption = "errorCaption";
        /// <summary>
        /// this attribute that indicates whether fields in the PivotTable are sorted in
        ///non-default order in the field list.
        /// </summary>
        public const string DefaultAutoSort = "fieldListSortAscending";
        /// <summary>
        /// Specifies the display name
        /// </summary>
        public const string Caption = "cap";
        /// <summary>
        /// Specifies the number of columns per page for this PivotTable that the filter area will
        ///occupy.
        /// </summary>
        public const string ColumnsPerPage = "colPageCount";
        /// <summary>
        /// Specifies the number of rows per page for this PivotTable that the filter area will occupy.
        /// </summary>
        public const string RowsPerPage = "rowPageCount";
        /// <summary>
        /// Specifies a boolean value that indicates whether the approximate number of child items
        ///for this item is greater than zero.
        /// </summary>
        public const string ChildItemsAttribute = "c";
        /// <summary>
        /// Specifies a boolean value that indicates whether this item has been expanded in the
        ///PivotTable view.
        /// </summary>
        public const string ExpandAttribute = "d";
        /// <summary>
        /// Specifies a boolean value that indicates whether attribute hierarchies nested next to
        ///each other on a PivotTable row or column will offer drilling "across" each other or not.
        /// </summary>
        public const string DrillAcrossAtribute = "e";
        /// <summary>
        /// Specifies a boolean value that indicates whether this item is a calculated member.
        /// </summary>
        public const string CalculatedMemberAttribute = "f";
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is hidden.
        /// </summary>
        public const string HiddenAttribute = "h";
        /// <summary>
        /// Specifies the user caption of the item.
        /// </summary>
        public const string ItemCaptionAttribute = "n";
        /// <summary>
        /// Specifies a boolean value that indicates whether the item has a character value.
        /// </summary>
        public const string CharAttribute = "s";
        /// <summary>
        /// Specifies a boolean value that indicates whether the details are hidden for this item.
        /// </summary>
        public const string HideDetailAttribute = "sd";
        /// <summary>
        /// Specifies a boolean value that indicate whether the item has a missing value.
        /// </summary>
        public const string MissingAttribute = "m";
        /// <summary>
        /// Specifies the type of this item. A value of 'default' indicates the subtotal or total item.
        /// </summary>
        public const string ItemTypeAttribute = "t";
        /// <summary>
        /// Specifies the base of this field,
        /// </summary>
        public const string BaseFieldAttribute = "base";
        /// <summary>
        /// Specifies the parent of this field,
        /// </summary>
        public const string ParentFieldAttribute = "par";
        /// <summary>
        /// Represents the collection of discrete grouping properties for a field group
        /// </summary>
        public const string DiscretePrElement = "discretePr";
        /// <summary>
        /// Represents the collection of items in a field group.
        /// </summary>
        public const string GroupItemsElement = "groupItems";
        /// <summary>
        /// Represents the collection of range grouping properties.
        /// </summary>
        public const string RangePrElement = "rangePr";
        /// <summary>
        /// Specifies a boolean value that indicates whether the application uses the source data to
        ///set the ending range value.
        /// </summary>
        public const string AutoEndAttribute = "autoEnd";
        /// <summary>
        /// Specifies a boolean value that indicates whether we use source data to set the beginning
        ///range value.
        /// </summary>
        public const string AutoStartAttribute = "autoStart";
        /// <summary>
        /// Specifies the ending value for date grouping if autoEnd is false.
        /// </summary>
        public const string EndDateAttribute = "endDate";
        /// <summary>
        /// Specifies the ending value for numeric grouping if autoEnd is false.
        /// </summary>
        public const string EndNumberAttribute = "endNum";
        /// <summary>
        /// Specifies the grouping.
        /// </summary>
        public const string GroupByAttribute = "groupBy";
        /// <summary>
        /// Specifies the grouping interval for numeric range grouping. Specifies the number of days
        ///to group by in date range grouping
        /// </summary>
        public const string GroupIntervalAttribute = "groupInterval";
        /// <summary>
        /// Specifies the starting value for date grouping if autoStart is false.
        /// </summary>
        public const string StartDateAttribute = "startDate";
        /// <summary>
        /// Specifies the starting value for numeric grouping if autoStart is false.
        /// </summary>
        public const string StartNumAttribute = "startNum";
        /// <summary>
        /// Specifies a boolean value that indicates whether an "AutoShow" filter is applied to this
        /// field.
        /// </summary>
        public const string AutoShowAttribute = "autoShow";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be removed from the
        ///PivotTable.
        /// </summary>
        public const string DragOffAttribute = "dragOff";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be dragged to the column
        ///axis.
        /// </summary>
        public const string DragToColAttribute = "dragToCol";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be dragged to the data
        /// region.
        /// </summary>
        public const string DragToData = "dragToData";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be dragged to the page
        /// region
        /// </summary>
        public const string DragToPage = "dragToPage";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be dragged to the row axis
        /// </summary>
        public const string DragToRow = "dragToRow";
        /// <summary>
        /// Specifies a boolean value that indicates whether new items that appear after a refresh
        /// should be hidden by default.
        /// </summary>
        public const string HideNewItemAttribute = "hideNewItems";
        /// <summary>
        /// Specifies a boolean value that indicates whether manual filter is in inclusive mode.
        /// </summary>
        public const string IncludeNewItemFilter = "includeNewItemsInFilter";
        /// <summary>
        /// Specifies a boolean value that indicates whether to insert a blank row after each item.
        /// </summary>
        public const string InsertBlankRow = "insertBlankRow";
        /// <summary>
        /// Specifies a boolean value that indicates whether to insert a page break after each item.
        /// </summary>
        public const string InsertPageBreak = "insertPageBreak";
        /// <summary>
        /// Specifies the number of items showed per page in the PivotTable.
        /// </summary>
        public const string ItemsPerPage = "itemPageCount";
        /// <summary>
        /// Specifies a boolean value that indicates whether field has a measure based filter.
        /// </summary>
        public const string MeasureFilterAttribute = "measureFilter";
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can have multiple items
        /// selected in the page field.
        /// </summary>
        public const string MultiItemSelction = "multipleItemSelectionAllowed";
        /// <summary>
        /// Specifies a boolean value that indicates whether to hide drop down buttons on PivotField
        /// headers
        /// </summary>
        public const string ShowDropDownAttribute = "showDropDowns";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the property as a member
        /// caption.
        /// </summary>
        public const string ShowPropAsCaption = "showPropAsCaption";

        /// <summary>
        /// Specifies a boolean value that indicates whether to show the member property value in a
        /// tooltip on the appropriate PivotTable cells.
        /// </summary>
        public const string ShowPropToolTip = "showPropTip";
        /// <summary>
        /// Specifies the type of sort that is applied to this field.
        /// </summary>
        public const string SortTypeAttribute = "sortType";
        /// <summary>
        /// Specifies the unique name of the member property to be used as a caption for the field
        /// and field items.
        /// </summary>
        public const string UniqueMemberProperty = "uniqueMemberProperty";
        /// <summary>
        /// Represents an item within a PivotTable field that uses a formula
        /// </summary>
        public const string CalculatedItems = "calculatedItems";
        /// <summary>
        /// Represents an item within a PivotTable field that uses a formula
        /// </summary>
        public const string CalculatedItem = "calculatedItem";
        /// <summary>
        /// Represents a set of selected fields and selected items within those fields
        /// </summary>
        public const string References = "references";
        /// <summary>
        /// Represents a set of selected fields and selected items within those fields
        /// </summary>
        public const string Reference = "reference";
        /// <summary>
        /// Flag indicating whether any indexes refer to fields or items in the Pivot cache
        /// </summary>
        public const string CacheIndex = "cacheIndex";
        /// <summary>
        /// Flag indicating whether the column grand total is included.
        /// </summary>
        public const string ColumnGrand = "grandCol";
        /// <summary>
        /// Flag indicating whether only the data values
        /// </summary>
        public const string DataOnly = "dataOnly";
        /// <summary>
        /// Position of the field within the axis to which this rule applies.
        /// </summary>
        public const string FieldPosition = "fieldPosition";
        /// <summary>
        /// Flag indicating whether the row grand total is included
        /// </summary>
        public const string RowGrand = "grandRow";
        /// <summary>
        /// Flag indicating whether only the item labels for an item selection 
        /// are selected and does
        ///not include the data values
        /// </summary>
        public const string LableOnly = "labelOnly";
        /// <summary>
        /// A Reference that specifies a subset of the selection area.
        /// </summary>
        public const string Offset = "offset";
        /// <summary>
        /// Specifies a boolean value that indicates whether the 
        /// item is referred to by position rather than item index.
        /// </summary>
        public const string ReferByPosition = "byPosition";
        /// <summary>
        /// Specifies a boolean value that indicates whether the item is 
        /// referred to by a relative reference rather than an absolute reference
        /// </summary>
        public const string ReferByRelative = "relative";
        /// <summary>
        /// Specifies a boolean value that indicates whether this field has selection
        /// </summary>
        public const string Selected = "selected";
        /// <summary>
        /// Pivot Area tag
        /// </summary>
        public const string PivotAreaTag = "pivotArea";
        /// <summary>
        /// Pivot Area Type
        /// </summary>
        public const string PivotAreaTypeAttribute = "areaType";
        /// <summary>
        /// Represents the collection of conditional formats applied to a PivotTable
        /// </summary>
        public const string ConditionalFormats = "conditionalFormats";
        /// <summary>
        /// Represents the conditional formatting defined in the PivotTable.
        /// </summary>
        public const string ConditionalFormat = "conditionalFormat";
        /// <summary>
        /// Specifies a boolean value that indicates whether the in-grid drop zones should be
        ///displayed at runtime, and whether classic layout is applied.
        /// </summary>
        public const string ShowGridDropZone = "gridDropZones";
        /// <summary>
        /// Specifies a boolean value that indicates how the page fields are laid out 
        /// when there are multiple PivotFields in the page area. 
        /// </summary>
        public const string PageOverThenDown = "pageOverThenDown";
        /// <summary>
        /// Specifies a boolean value that indicates whether to show error messages in cells
        /// </summary>
        public const string ShowError = "showError";
        /// <summary>
        /// Specifies a boolean value that indicates 
        /// whether to show a message in cells with no value.
        /// </summary>
        public const string MissingCapiton = "missingCaption";
        /// <summary>
        /// Specifies a boolean value that indicates whether to 
        /// show a message in cells with no value.
        /// </summary>
        public const string ShowMissing = "showMissing ";
        /// <summary>
        /// Specifies a boolean value that indicates whether the formatting applied by 
        /// the user to the PivotTable cells is discarded on refresh.
        /// </summary>
        public const string PreserveFormatting = "preserveFormatting";
        /// <summary>
        /// Specifies a boolean value that indicates whether tooltips should be 
        /// displayed for PivotTable data cells. 
        /// </summary>
        public const string ShowDataTips = "showDataTips";
        /// <summary>
        /// 
        /// </summary>
        public const string FieldPrintTitles = "fieldPrintTitles";
        /// <summary>
        /// Specifies a boolean value that indicates whether row or column titles that span multiple
        ///cells should be merged into a single cell.
        /// </summary>
        public const string MergeItem = "mergeItem";
        /// <summary>
        /// Specifies the number of page fields to display before starting another row or column.
        /// </summary>
        public const string PageWrap = "pageWrap";
        /// <summary>
        /// Represents the collection of filters that apply to this PivotTable.
        /// </summary>
        public const string Filters = "filters";

        ///<summary>
        ///Specifies the description of the pivot filter.
        /// </summary>
        public const string Description = "description";
        ///<summary>
        ///Specifies the evaluation order of the pivot filter.
        /// </summary>
        public const string EvalOrderAttribute = "evalOrder";
        ///<summary>
        ///Specifies the index of the measure field.
        /// </summary>
        public const string MeasureFldAttribute = "iMeasureFld";
        ///<summary>
        ///Specifies the index of the measure cube field.
        /// </summary>
        public const string MeasureHierAttribute = "iMeasureHier";
        ///<summary>
        ///Indicates whether the AutoFilter button for this column is hidden
        /// </summary>
        public const string HiddenButtonAttribute = "hiddenButton";
        ///<summary>
        ///Flag indicating whether the filter button is visible.
        /// </summary>
        public const string ShowButtonAttribute = "showButton";
         /// <summary>
        /// Specifies the element for Auto filter.
        /// </summary>
        public const string AutoFilterElement = "autoFilter";
        /// <summary>
        /// Specifies the element for filter column
        /// </summary>
        public const string FilterColumnElement = "filterColumn";
        /// <summary>
        /// Specifies the attributes for operator which is applied in pivot filter.
        /// </summary>
        public const string OperatorAttribute = "operator";
        ///<summary>
        ///Specifies the string value "1" used by label pivot filters.
        /// </summary>
        public const string Value1 = "stringValue1";
        /// <summary>
        /// Zero-based index indicating the AutoFilter column to which this filter information applies.
        /// </summary>
        public const string ColumnIdAttribute = "colId";
        ///<summary>
        ///Specifies the string value "2" used by label pivot filters.
        /// </summary>
        public const string Value2 = "stringValue2";
        ///<summary>
        ///Specifies the unique identifier of the pivot filter as assigned by the PivotTable.
        /// </summary>
        public const string PivotFilterId = "id";
        /// <summary>
        /// Specifies the element for Custom filters.
        /// </summary>
        public const string CustomFiltersElement = "customFilters";
        /// <summary>
        /// Specifies the element for Custom filter
        /// </summary>
        public const string CustomFilterElement = "customFilter";
        /// <summary>
        /// Attribute specifying whether and operator is used.
        /// </summary>
        public const string AndAttributeName = "and";
        /// <summary>
        /// Top or bottom value to use as the filter criteria.
        /// </summary>
        public const string ValAttibuteName = "val";
        /// <summary>
        /// Flag indicating whether to filter by blank.
        /// </summary>
        public const string BlankAttributeName = "blank";
        /// <summary>
        /// The actual cell value in the range which is used to perform the comparison for this filter.
        /// </summary>
        public const string FilterValueAttributeName = "filterVal";
        /// <summary>
        /// Flag indicating whether or not to filter by percent value of the column.
        /// </summary>
        public const string PercentAttributeName = "percent";
        /// <summary>
        /// Flag indicating whether or not to filter by top order
        /// </summary>
        public const string TopAttributeName = "top";
        /// <summary>
        /// Specifies the element for Top 10 filter
        /// </summary>
        public const string Top10FilterElement = "top10";
        /// <summary>
        /// Represents the collection of filter that apply to this PivotFilters.
        /// </summary>
        public const string Filter = "filter";
        /// <summary>
        /// Represents the collection of custom formats apply to the pivot table.
        /// </summary>
        public const string CustomFormats = "formats";
        /// <summary>
        /// Represents the collection of formats applied to PivotChart.
        /// </summary>
        public const string ChartFormats = "chartFormats";
        /// <summary>
        /// Specifies the string to be displayed in row header in compact mode.
        /// </summary>
        public const string RowHeaderCaption = "rowHeaderCaption";
        /// <summary>
        /// Represents the sorting scope for the PivotTable.
        /// </summary>
        public const string AutoSortScope = "autoSortScope";
        /// <summary>
        ///  Represents a Error value in a PivotTable.
        /// </summary>
        public const string ErrorTag = "e";
        /// <summary>
        /// Represents the collection of OLAP hierarchies in the PivotCache.
        /// </summary>
        public const string CacheHierarchies = "cacheHierarchies";
        /// <summary>
        /// Represents the collection of Key Performance Indicators (KPIs) 
        /// defined on the OLAP server and stored in the PivotCache.
        /// </summary>
        public const string PivotOLAPKPIs = "kpis";
        /// <summary>
        /// Represents the collection of PivotTable OLAP dimensions.
        /// </summary>
        public const string OLAPDimensions = "dimensions";
        /// <summary>
        /// Represents a PivotTable OLAP measure group.
        /// </summary>
        public const string OLAPMeasureGroups = "measureGroups";
        /// <summary>
        /// Represents the PivotTable OLAP measure group - Dimension maps.
        /// </summary>
        public const string OLAPMaps="maps";
        /// <summary>
        /// Represents the collection of OLAP hierarchies associated with the PivotTable.
        /// </summary>
        public const string PivotHierarchies = "pivotHierarchies";
        /// <summary>
        /// Represents the collection of references to OLAP hierarchies on the row axis of a PivotTable.
        /// </summary>
        public const string RowHierarchiesUsage = "rowHierarchiesUsage";
        public const string FieldItemCaption = "c";
        /// <summary>
        /// Specifies the hierarchy that this field is part of.
        /// </summary>
        public const string HierarchyAttribute = "hierarchy";
        /// <summary>
        /// Specifies the hierarchy level that this field is part of.
        /// </summary>
        public const string HierarchyLevel = "level";
        /// <summary>
        /// Represents the Pivot Field Captions.
        /// </summary>
        public const string FieldCaption = "caption";
        /// <summary>
        /// Specifies a boolean value that indicates whether sort is applied to this field in the datasource.
        /// </summary>
        public const string DataSourceSort = "dataSourceSort";
        /// <summary>
        /// Specifies a boolean value that indicates the drill state of the attribute hierarchy in an
        /// OLAP-based PivotTable.
        /// </summary>
        public const string DefaultAttributeDrillState="defaultAttributeDrillState";
        /// <summary>
        /// Specifies a boolean value that indicates whether all items in the field are expanded.
        /// Applies only to OLAP PivotTables.
        /// </summary>
        public const string AllDrilled = "allDrilled";
        #endregion

        #region Methods
        /// <summary>
        /// Constructor used to prevent from creating instances of this class.
        /// </summary>
        private PivotTable()
        {
        }
        #endregion
    }
}
