#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#if !(SILVERLIGHT) && !(WINRT) && !SyncfusionFramework2_0 && !(WP)
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
#endif
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represent pivot table implementation.
    /// </summary>
    public class PivotTableImpl
      : CommonObject
      , ICloneParent
      , IPivotTable
    {
        #region Class constants
        /// <summary>
        /// Default starting name of the pivot data field.
        /// </summary>
        public const string DefaultDataFieldStart = "Sum of ";
        /// <summary>
        /// Code of the first record for the pivot table.
        /// </summary>
        public const TBIFFRecord DEF_FIRSTRECORD_CODE = TBIFFRecord.PivotViewDefinition;
        /// <summary>
        /// Unknown records that belong to pivot table.
        /// </summary>
        private static readonly TBIFFRecord[] DEF_UNKNOWN_PIVOTRECORDS = new TBIFFRecord[]
    {
      ( TBIFFRecord )2050,
      ( TBIFFRecord )2064,
      ( TBIFFRecord )2148,
      ( TBIFFRecord )0xF4,
      ( TBIFFRecord )0xF5,

      TBIFFRecord.PivotFormat,
      TBIFFRecord.RuleData,
      TBIFFRecord.RuleFilter,
      TBIFFRecord.SelectionInfo,
      TBIFFRecord.DBQueryExt
    };
        /// <summary>
        /// When Pivot table create, updated and refreshed
        /// then  Excel 2007 stores this version
        /// </summary>
        public const byte Excel2007Version = 3;
        /// <summary>
        /// When Pivot table create, updated and refreshed
        /// then  Excel 2010 stores this version
        /// </summary>
        private const byte Excel2010Version = 4;
        /// <summary>
        /// When Pivot table create, updated and refreshed
        /// then  Excel 2013 stores this version
        /// </summary>
        private const byte Excel2013Version = 5;
        #endregion

        #region Class members
        /// <summary>
        /// View definition.
        /// </summary>
        private PivotViewDefinitionRecord m_viewDefinition = (PivotViewDefinitionRecord)
          BiffRecordFactory.GetRecord(TBIFFRecord.PivotViewDefinition);
        /// <summary>
        /// Fields collection.
        /// </summary>
        private List<PivotFieldImpl> m_arrFields = new List<PivotFieldImpl>();
        /// <summary>
        /// Row / column field ids.
        /// </summary>
        private RowColumnFiledIdRecord[] m_arrRowColumnFiledId = new RowColumnFiledIdRecord[] { null, null };
        /// <summary>
        /// Collection of all LineItemArray records needed for this pivot table.
        /// </summary>
        private List<LineItemArrayRecord> m_arrLineItems = new List<LineItemArrayRecord>();
        /// <summary>
        /// This record contains information about additional features
        /// added to PivotTables in Excel.
        /// </summary>
        private ViewExtendedInfoRecord m_viewExInfo = (ViewExtendedInfoRecord)
          BiffRecordFactory.GetRecord(TBIFFRecord.ViewExtendedInfo);
        /// <summary>
        /// Page item.
        /// </summary>
        private PageItemRecord m_pageItem;
        /// <summary>
        /// Data items.
        /// </summary>
        private List<DataItemRecord> m_arrDataItems = new List<DataItemRecord>();
        /// <summary>
        /// Unknown records that belong to pivot table.
        /// </summary>
        private List<BiffRecordRaw> m_arrUnknown = new List<BiffRecordRaw>();
        /// <summary>
        /// Parent workbook.
        /// </summary>
        private WorkbookImpl m_book;
        /// <summary>
        /// Collection of pivot table fields.
        /// </summary>
        private PivotTableFields m_arrPivotFields;
        /// <summary>
        /// Table location.
        /// </summary>
        private IRange m_location;
        /// <summary>
        /// Table end location.
        /// </summary>
        private IRange m_endLocation;
        
        /// <summary>
        /// Contains all data fields.
        /// </summary>
        private PivotDataFields m_dataFields;
        /// <summary>
        /// True if row, column, and item labels appear on the first row of each page when
        /// the specified PivotTable report is printed. False if labels are printed only on
        /// the first page. The default value is True.
        /// </summary>
        private bool m_bItemPrintTitles = true;
        /// <summary>
        /// Built-in style.
        /// </summary>
        private PivotBuiltInStyles? m_builtInStyle;
        /// <summary>
        /// Custom Style Name
        /// </summary>
        private string customStyleName;
        /// <summary>
        /// Parent worksheet.
        /// </summary>
        private WorksheetImpl m_worksheet;
        private List<PivotFieldImpl> m_lstRowFields = new List<PivotFieldImpl>();
        private List<PivotFieldImpl> m_lstColumnFields = new List<PivotFieldImpl>();
        private List<PivotFieldImpl> m_lstPageFields = new List<PivotFieldImpl>();
        /// <summary>
        /// Represents the Pivot Table Options
        /// </summary>
        private PivotTableOptions m_options;
        /// <summary>
        /// Specifies the first column of the PivotTable data, relative to the top left cell in the ref
        ///value
        /// </summary>
        private int m_iFirstDataCol;
        /// <summary>
        /// Specifies the first column of the PivotTable data, relative to the top left cell in the ref
        ///value
        /// </summary>
        private int m_iFirstDataRow;
        /// <summary>
        /// Specifies the first row of the PivotTable header, relative to the top left cell in the ref
        ///value.
        /// </summary>
        private int m_iFirstHeaderRow;
        /// <summary>
        /// Specifies the number of columns per page for this PivotTable that the filter area will
        ///occupy.
        /// </summary>
        private int m_iColumnsPerPage;
        /// <summary>
        /// Specifies the number of rows per page for this PivotTable that the filter area will occupy.
        /// </summary>
        private int m_iRowsPerPage;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column headers for the table.
        /// </summary>
        private bool m_bShowColHeaderStyle;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column stripe formatting for
        ///the table.
        /// </summary>
        private bool m_bShowColStripes;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the last column.
        /// </summary>
        private bool m_bShowLastCol;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row headers for the table.
        /// </summary>
        private bool m_bShowRowHeaderStyle;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row stripe formatting for the
        ///table.
        /// </summary>
        private bool m_bShowRowStripes;
        /// <summary>
        /// Represents the Pivot table column items in Stream
        /// </summary>
        private Stream m_colItemsStream;
        /// <summary>
        /// Represents the Pivot Table row items in stream
        /// </summary>
        private Stream m_rowItemsStream;
        /// <summary>
        /// Represents the pivot table to add the Calculated 
        /// Data field in rows
        /// </summary>
        private bool m_bShowDataFieldInRow;
        /// <summary>
        /// Preserves the XlsIO unsupported elements
        /// </summary>
        private Dictionary<string, Stream> m_preservedElements;
        /// <summary>
        /// Returns the calculated field collection represents all
        /// the calculated fields int the specified pivot table.
        /// </summary>
        private PivotCalculatedFields m_calculatedFields;
        /// <summary>
        /// Indicates wheather the pivot table modified.
        /// </summary>
        private bool m_bIsChanged;
        private List<IPivotField> m_rowFields;
        private List<IPivotField> m_pageFields;
        /// <summary>
        /// Represents the column field order.
        /// </summary>
        private List<int> m_colFieldsOrder;
        /// <summary>
        /// Represents the Row fields order
        /// </summary>
        private List<int> m_rowFieldsOrder;
        /// <summary>
        /// Variable for pivot engine.
        /// </summary>
        #if !SILVERLIGHT && !WINRT && !WP && !(WP) && !SyncfusionFramework2_0
        private PivotEngine m_PivotEngine;
        #endif

        /// <summary>
        /// Collection of Filters
        /// </summary>
        private PivotTableFilters  m_filters;

        private PivotTableLayout m_PivotTableLayout;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates collection and sets its Application and Parent values.
        /// </summary>
        /// <param name="application">
        /// Application object that represents the Excel application.
        /// </param>
        /// <param name="parent">Parent object of this collection.</param>
        public PivotTableImpl(IApplication application, object parent)
            : base(application, parent)
        {
            SetWorkbook();
            RowGrand = true;
            ColumnGrand = true;
            m_options = new PivotTableOptions(this, m_viewExInfo, m_viewDefinition);
            m_bShowRowHeaderStyle = true;
            m_bShowRowStripes = false;
            m_bShowColStripes = false;
            m_bShowColHeaderStyle = true;
            FirstDataRow = 2;
            FirstDataCol = 1;
            FirstHeaderRow = 1;
            RowsPerPage = 1;
            ColumnsPerPage = 1;
            ShowDataFieldInRow = false;
            m_filters = new PivotTableFilters();
        }
        /// <summary>
        /// Creates collection and sets its Application and Parent values.
        /// </summary>
        /// <param name="application">
        /// Application object that represents the Excel application.
        /// </param>
        /// <param name="parent">Parent object of this collection.</param>
        /// <param name="cacheIndex">Cache index.</param>
        /// <param name="location">Pivot table location.</param>
        public PivotTableImpl(IApplication application, object parent, int cacheIndex, IRange location)
            : this(application, parent)
        {
            CacheIndex = cacheIndex;
            m_arrPivotFields = new PivotTableFields(this);
            m_dataFields = new PivotDataFields(application, this);
            m_location = location;
            
        }
        /// <summary>
        /// Sets parent workbook to the correct value.
        /// </summary>
        private void SetWorkbook()
        {
            m_worksheet = FindParent(typeof(WorksheetImpl)) as WorksheetImpl;

            if (m_worksheet == null)
                throw new ArgumentNullException("Cannot find parent worksheet.");

            m_book = m_worksheet.ParentWorkbook;
        }
        #endregion

        #region Class properties

        internal PivotTableLayout PivotLayout
        {
            get
            {
                return m_PivotTableLayout;
            }
            set
            {
                m_PivotTableLayout = value;
            }
        }
        /// <summary>
        /// Property for pivot filter collections
        /// </summary>
        internal PivotTableFilters Filters
        {
            get
            {
                return m_filters;
            }
            set
            {
                m_filters = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        #if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WP)
        public PivotEngine PivotEngineValues
        {
            get
            {
                return m_PivotEngine;
            }
            set
            {
                m_PivotEngine = value;
            }
        }
        #endif

        /// <summary>
        /// Index of the pivot table cache
        /// </summary>
        public int CacheIndex
        {
            get
            {
                return m_viewDefinition.CacheIndex;
            }
            set
            {
                if (value < 0 || value > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException("CacheIndex");

                m_viewDefinition.CacheIndex = (ushort)value;
            }
        }
        /// <summary>
        /// True if the PivotTable report displays a custom error string in cells
        /// that contain errors. The default value is False.
        /// </summary>
        public bool DisplayErrorString
        {
            get
            {
                return m_viewExInfo.IsDisplayErrorString;
            }
            set
            {
                m_viewExInfo.IsDisplayErrorString = value;
            }
        }
        /// <summary>
        /// True if the PivotTable report displays a custom string in cells
        /// that contain null values. The default value is True. 
        /// </summary>
        public bool DisplayNullString
        {
            get
            {
                return m_viewExInfo.IsDisplayNullString;
            }
            set
            {
                m_viewExInfo.IsDisplayNullString = value;
            }
        }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains column with grand totals for rows (same as ColumnGrand in VBA).
        /// </summary>
        public bool ColumnGrand
        {
            get
            {
                return m_viewDefinition.IsColumnGrand;
            }
            set
            {
                m_viewDefinition.IsColumnGrand = value;
            }
        }
        /// <summary>
        /// True if drilldown is enabled. The default value is True.
        /// </summary>
        public bool EnableDrilldown
        {
            get
            {
                return m_viewExInfo.IsEnableDrilldown;
            }
            set
            {
                m_viewExInfo.IsEnableDrilldown = value;
            }
        }
        /// <summary>
        /// True if the PivotTable Field dialog box is available when the user
        /// double-clicks the PivotTable field. The default value is True.
        /// </summary>
        public bool EnableFieldDialog
        {
            get
            {
                return m_viewExInfo.IsEnableFieldDialog;
            }
            set
            {
                m_viewExInfo.IsEnableFieldDialog = value;
            }
        }
        /// <summary>
        /// True if the PivotTable Wizard is available.
        /// The default value is True.
        /// </summary>
        public bool EnableWizard
        {
            get
            {
                return m_viewExInfo.IsEnableWizard;
            }
            set
            {
                m_viewExInfo.IsEnableWizard = value;
            }
        }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain errors
        /// when the DisplayErrorString property is True.
        /// </summary>
        public string ErrorString
        {
            get
            {
                return m_viewExInfo.ErrorString;
            }
            set
            {
                m_viewExInfo.ErrorString = value;
            }
        }
        /// <summary>
        /// True if the PivotTable report is recalculated only at the user's request.
        /// The default value is False.
        /// </summary>
        public bool ManualUpdate
        {
            get
            {
                return m_viewExInfo.IsManualUpdate;
            }
            set
            {
                m_viewExInfo.IsManualUpdate = value;
            }
        }
        /// <summary>
        /// True if the specified PivotTable report�s outer-row item,
        /// column item, subtotal, and grand total labels use merged cells.
        /// </summary>
        public bool MergeLabels
        {
            get
            {
                return m_viewExInfo.IsMergeLabels;
            }
            set
            {
                m_viewExInfo.IsMergeLabels = value;
            }
        }
        /// <summary>
        /// PivotTable name.
        /// </summary>
        public string Name
        {
            get
            {
                return m_viewDefinition.TableName;
            }
            set
            {
                m_viewDefinition.TableName = value;
            }
        }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain null
        /// values when the DisplayNullString property is True.
        /// </summary>
        public string NullString
        {
            get
            {
                return m_viewExInfo.NullString;
            }
            set
            {
                m_viewExInfo.NullString = value;
            }
        }
        /// <summary>
        /// Returns or sets the order in which page fields are added to the PivotTable report�s layout.
        /// </summary>
        public ExcelPagesOrder PageFieldOrder
        {
            get
            {
                return m_viewExInfo.IsAcrossPageLay ? ExcelPagesOrder.OverThenDown : ExcelPagesOrder.DownThenOver;
            }
            set
            {
                m_viewExInfo.IsAcrossPageLay = (ExcelPagesOrder.OverThenDown == value);
            }
        }
        /// <summary>
        /// Returns or sets the style used in the bound page field area.
        /// The default value is a null string (no style is applied by default).
        /// </summary>
        public string PageFieldStyle
        {
            get
            {
                return m_viewExInfo.PageFieldStyle;
            }
            set
            {
                m_viewExInfo.PageFieldStyle = value;
            }
        }
        /// <summary>
        /// Returns or sets the number of page fields in each column
        /// or row in the PivotTable report.
        /// </summary>
        public int PageFieldWrapCount
        {
            get
            {
                return m_options.PageFieldWrapCount;
            }
            set
            {
                m_options.PageFieldWrapCount = (ushort)value;
            }
        }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains row with grand totals for columns (same as RowGrand in VBA).
        /// </summary>
        public bool RowGrand
        {
            get
            {
                return m_viewDefinition.IsRowGrand;
            }
            set
            {
                m_viewDefinition.IsRowGrand = value;
            }
        }
        //    /// <summary>
        //    /// Indicates whether the PivotTable has an autoformat applied.
        //    /// </summary>
        //    public bool IsAutoFormat
        //    {
        //      get
        //      {
        //        return m_viewDefinition.IsAutoFormat;
        //      }
        //      set
        //      {
        //        m_viewDefinition.IsAutoFormat = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Name of the data field.
        //    /// </summary>
        //    public string DataFieldName
        //    {
        //      get
        //      {
        //        return m_viewDefinition.DataFieldName;
        //      }
        //      set
        //      {
        //        m_viewDefinition.DataFieldName = value;
        //      }
        //    }
        /// <summary>
        /// Returns cache used by this pivot table. Read-only.
        /// </summary>
        public PivotCacheImpl Cache
        {
            get
            {
                return m_book.PivotCaches[CacheIndex];
            }
        }
        /// <summary>
        /// Returns pivot table location. Read-only.
        /// </summary>
        public IRange Location
        {
            get
            {
                return m_location;
            }
            set
            {
                if(!Workbook.Loading)
                    SetChanged(true);
                m_location = value;
            }
        }
       /// <summary>
        /// Returns pivot table end location. Read-only.
        /// </summary>
        public IRange EndLocation
        {
            get
            {
                return m_endLocation;
            }
            set
            {
                m_endLocation = value;
            }
        }
        /// <summary>
        /// Returns collection of pivot fields. Read-only.
        /// </summary>
        internal PivotTableFields InternalFields
        {
            get
            {
                if (m_arrPivotFields == null)
                    m_arrPivotFields = new PivotTableFields(this);

                return m_arrPivotFields;
            }
        }
        /// <summary>
        /// Returns collection of pivot fields. Read-only.
        /// </summary>
        public PivotTableFields Fields
        {
            get
            {
                return m_arrPivotFields;
            }
        }
        /// <summary>
        /// Returns collection of pivot fields. Read-only.
        /// </summary>
        IPivotFields IPivotTable.Fields
        {
            get
            {
                return m_arrPivotFields;
            }
        }
        /// <summary>
        /// Gets collection of pivot table data fields. Read-only.
        /// </summary>
        public PivotDataFields DataFields
        {
            get
            {
                if (m_dataFields == null)
                    m_dataFields = new PivotDataFields(Application, this);
                return m_dataFields;
            }
        }
        /// <summary>
        /// Gets collection of pivot table data fields. Read-only.
        /// </summary>
        IPivotDataFields IPivotTable.DataFields
        {
            get
            {
                return m_dataFields;
            }
        }
        /// <summary>
        /// Gets parent workbook. Read-only.
        /// </summary>
        public WorkbookImpl Workbook
        {
            get
            {
                return m_book;
            }
        }
        public WorksheetImpl Worksheet
        {
            get
            {
                return m_worksheet;
            }
        }
        /// <summary>
        /// The ShowDrillIndicators property is used for toggling the display of
        /// drill indicators in the PivotTable.
        /// </summary>
        public bool ShowDrillIndicators
        {
            get
            {
                return m_options.ShowDrillIndicators;
            }
            set
            {
                m_options.ShowDrillIndicators = value;
            }
        }
        /// <summary>
        /// Gets/sets value controlling whether or not filter buttons and PivotField
        /// captions for rows and columns are displayed in the grid.
        /// </summary>
        public bool DisplayFieldCaptions
        {
            get
            {
                return m_options.DisplayFieldCaptions;
            }
            set
            {
                m_options.DisplayFieldCaptions = value;
            }
        }
        /// <summary>
        /// True if row, column, and item labels appear on the first row of each page when
        /// the specified PivotTable report is printed. False if labels are printed only on
        /// the first page. The default value is True.
        /// </summary>
        public bool RepeatItemsOnEachPrintedPage
        {
            get
            {
                return m_bItemPrintTitles;
            }
            set
            {
                m_bItemPrintTitles = value;
            }
        }
        /// <summary>
        /// Gets/sets built-in pivot style.
        /// </summary>
        public PivotBuiltInStyles? BuiltInStyle
        {
            get
            {
                return m_builtInStyle;
            }
            set
            {
                m_builtInStyle = value;
            }
        }
        /// <summary>
        /// Gets/Sets value for Name of Custom Styles
        /// </summary>
        public string CustomStyleName
        {
            get
            {
                return customStyleName;
            }
            set
            {
                customStyleName = value;
            }
        }
      
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains grand totals for rows.
        /// </summary>
        public bool ShowRowGrand
        {
            get
            {
                return ColumnGrand;
            }
            set
            {
                ColumnGrand = value;
            }
        }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains grand totals for columns.
        /// </summary>
        public bool ShowColumnGrand
        {
            get
            {
                return RowGrand;
            }
            set
            {
                RowGrand = value;
            }
        }
        
        /// <summary>
        /// Represents the pivot table options.
        /// </summary>
        public IPivotTableOptions Options
        {
            get
            {
                return m_options;
            }
        }
        /// <summary>
        /// Specifies the first column of the PivotTable data, relative to the top left cell in the ref
        ///value
        /// </summary>
        public int FirstDataCol
        {
            get
            {
                return m_iFirstDataCol;
            }
            set
            {
                m_iFirstDataCol = value;
            }
        }
        /// <summary>
        /// Specifies the first column of the PivotTable data, relative to the top left cell in the ref
        ///value
        /// </summary>
        public int FirstDataRow
        {
            get
            {
                return m_iFirstDataRow;
            }
            set
            {
                m_iFirstDataRow = value;
            }
        }
        /// <summary>
        /// Specifies the first row of the PivotTable header, relative to the top left cell in the ref
        ///value.
        /// </summary>
        public int FirstHeaderRow
        {
            get
            {
                return m_iFirstHeaderRow;
            }
            set
            {
                m_iFirstHeaderRow = value;
            }
        }
        /// <summary>
        /// Specifies the number of columns per page for this PivotTable that the filter area will
        ///occupy.
        /// </summary>
        public int ColumnsPerPage
        {
            get
            {
                return m_iColumnsPerPage;
            }
            set
            {
                m_iColumnsPerPage = value;
            }
        }
        /// <summary>
        /// Specifies the number of rows per page for this PivotTable that the filter area will occupy.
        /// </summary>
        public int RowsPerPage
        {
            get
            {
                return m_iRowsPerPage;
            }
            set
            {
                m_iRowsPerPage = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column headers for the table.
        /// </summary>
        public bool ShowColHeaderStyle
        {
            get
            {
                return m_bShowColHeaderStyle;
            }
            set
            {
                m_bShowColHeaderStyle = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show column stripe formatting for
        ///the table.
        /// </summary>
        public bool ShowColStripes
        {
            get
            {
                return m_bShowColStripes;
            }
            set
            {
                m_bShowColStripes = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the last column.
        /// </summary>
        public bool ShowLastCol
        {
            get
            {
                return m_bShowLastCol;
            }
            set
            {
                m_bShowLastCol = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row headers for the table.
        /// </summary>
        public bool ShowRowHeaderStyle
        {
            get
            {
                return m_bShowRowHeaderStyle;
            }
            set
            {
                m_bShowRowHeaderStyle = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show row stripe formatting for the
        ///table.
        /// </summary>
        public bool ShowRowStripes
        {
            get
            {
                return m_bShowRowStripes;
            }
            set
            {
                m_bShowRowStripes = value;
            }
        }
        /// <summary>
        /// Represents the Pivot table column items in Stream
        /// </summary>
        internal Stream ColumnItemsStream
        {
            get
            {
                return m_colItemsStream;
            }
            set
            {
                m_colItemsStream = value;
            }
        }
        /// <summary>
        /// Represents the Pivot Table row items in stream
        /// </summary>
        internal Stream RowItemsStream
        {
            get
            {
                return m_rowItemsStream;
            }
            set
            {
                m_rowItemsStream = value;
            }

        }
        /// <summary>
        /// Represents the pivot table to add the Calculated 
        /// Data field in rows
        /// </summary>
        public bool ShowDataFieldInRow
        {
            get
            {
                return m_bShowDataFieldInRow;
            }
            set
            {
                if (m_bShowDataFieldInRow != value)
                {
                    if(!m_book.Loading)
                        SetChanged(true);
                    m_bShowDataFieldInRow = value;
                }
            }
        }
        /// <summary>
        /// MS Excel Application object 
        /// </summary>
        public IApplication Application
        {
            get
            {
                return m_book.Application;
            }
        }
        ///// <summary>
        ///// Represents the row fields of the pivot table
        ///// </summary>
        //internal List<PivotFieldImpl> RowFields
        //{
        //    get
        //    {
        //        return m_lstRowFields;
        //    }
        //}
        /// <summary>
        /// Preserves the XlsIO unsupported elements
        /// </summary>
        internal Dictionary<string, Stream> PreservedElements
        {
            get
            {
                if (m_preservedElements == null)
                    m_preservedElements = new Dictionary<string, Stream>();
                return m_preservedElements;
            }
        }
        /// <summary>
        /// Returns the calculated field collection represents all
        /// the calculated fields int the specified pivot table.
        /// </summary>
        public IPivotCalculatedFields CalculatedFields
        {
            get
            {
                return GetCalculatedFields();
            }
        }
        /// <summary>
        /// Returns the collection of page field for the specified pivot table,
        /// </summary>
        public IPivotFields PageFields
        {
            get
            {
                return GetPivotFields(PivotAxisTypes.Page);
            }
        }
        /// <summary>
        /// Returns the collection of Pivot Row field for the specified pivot table.
        /// </summary>
        internal List<IPivotField> PivotRowFields
        {
            get
            {
                if (m_rowFields == null)
                    m_rowFields = new List<IPivotField>();

                return m_rowFields;
               
            }
        }
        /// <summary>
        /// Returns the collection of Pivot Page field for the specified pivot table.
        /// </summary>
        /// <value>The pivot page fields.</value>
        internal List<IPivotField> PivotPageFields
        {
            get
            {
                if (m_pageFields == null)
                    m_pageFields = new List<IPivotField>();

                return m_pageFields;
            }
        }
        /// <summary>
        /// Returns the collection of Row field for the specified pivot table.Read-only.
        /// </summary>
        /// <value></value>
        public IPivotFields RowFields
        {
            get
            {
                return GetPivotFields(PivotAxisTypes.Row);
            }
        }
        /// <summary>
        /// Returns the collection of Column field for the specified pivot table.
        /// </summary>
        public IPivotFields ColumnFields
        {
            get
            {
                return GetPivotFields(PivotAxisTypes.Column);
            }
        }
        /// <summary>
        /// Represents the pivot table modified.
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return m_bIsChanged;
            }
            set
            {
                m_bIsChanged = value;
            }
        }
        /// <summary>
        /// Gets the col fields order.
        /// </summary>
        internal List<int> ColFieldsOrder
        {
            get
            {
                if (m_colFieldsOrder == null)
                    m_colFieldsOrder = new List<int>();
                return m_colFieldsOrder;
            }
        }

        /// <summary>
        /// Gets the Row fields order.
        /// </summary>
        internal List<int> RowFieldsOrder
        {
            get
            {
                if (m_rowFieldsOrder == null)
                    m_rowFieldsOrder = new List<int>();
                return m_rowFieldsOrder;
            }
        }
        #endregion

        #region Class parse methods
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
        /// <summary>
        /// Method to fill the sheet with pivot engine values.
        /// </summary>
        public void Layout()
        {

            //IRange range = this .Workbook .PivotCaches 

            Syncfusion.XlsIO.Implementation.PivotAnalysis.PivotEngine pivotEngine;
            Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables.PivotEngineSerialization serialize = new Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables.PivotEngineSerialization();

            PivotTableImpl table = (PivotTableImpl)this;

            if (Cache.SourceRange != null)
            {
                for (int i = 0; i < table.Cache.CacheFields.Count; i++)
                {
                    PivotCacheFieldImpl fieldImpl = table.Cache.CacheFields[i];
                    if(fieldImpl .ItemRange != null )
                     fieldImpl.Name = this.Cache.SourceRange.Worksheet[fieldImpl.ItemRange.Row - 1, fieldImpl.ItemRange.Column].Value;
                    if (table.Fields[i].Name != fieldImpl.Name)
                        table.Fields[i].Name = fieldImpl.Name;
                    fieldImpl.ItemRange = this.Cache.SourceRange.Worksheet[Cache.SourceRange.Row + 1, Cache.SourceRange.Column + i, Cache.SourceRange.LastRow, Cache.SourceRange.Column + i];
                    fieldImpl.Items = null;
                    fieldImpl.IsParsed = true;
                    fieldImpl.Fill(this.Cache.SourceRange.Worksheet, Cache.SourceRange.Row, Cache.SourceRange.LastRow, Cache.SourceRange.Column + i);
                }
            }
            table.Cache.IsRefreshOnLoad = false;
            pivotEngine = serialize.PopulatePivotEngine(this.Worksheet as IWorksheet, table);
            table.PivotEngineValues = pivotEngine;

        }
#endif
        /// <summary>
        /// Parses pivot table.
        /// </summary>
        /// <param name="data">Records with pivot table data.</param>
        /// <param name="iPos">Offset to the first pivot table record.</param>
        /// <returns>Offset to the record after table records.</returns>
        public int Parse(IList data, int iPos)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (iPos < 0 || iPos > data.Count - 1)
                throw new ArgumentOutOfRangeException("iPos", "Value cannot be less than 0 and greater than data.Length - 1");

            ClearCollections();

            BiffRecordRaw record = (BiffRecordRaw)data[iPos];
            record.CheckTypeCode(TBIFFRecord.PivotViewDefinition);
            m_viewDefinition = (PivotViewDefinitionRecord)record;
            iPos++;

            record = (BiffRecordRaw)data[iPos];

            for (int i = 0, len = m_viewDefinition.FieldsNumber; i < len; i++)
            {
                PivotFieldImpl field = new PivotFieldImpl(this);
                iPos = field.Parse(data, iPos);
                m_arrFields.Add(field);
            }

            record = (BiffRecordRaw)data[iPos];
            int index = 0;

            while (record.TypeCode == TBIFFRecord.RowColumnFieldId)
            {
                m_arrRowColumnFiledId[index] = (RowColumnFiledIdRecord)record;
                index++;
                iPos++;
                record = (BiffRecordRaw)data[iPos];
            }

            int[] arrFieldsCount = new int[] { m_viewDefinition.RowFieldsNumber,
                                         m_viewDefinition.ColumnFieldsNumber };

            int iLineItemIndex = 0;

            if (record.TypeCode == TBIFFRecord.PageItem)
            {
                m_pageItem = (PageItemRecord)record;
                iPos++;
                record = (BiffRecordRaw)data[iPos];
            }

            while (record.TypeCode == TBIFFRecord.DataItem)
            {
                m_arrDataItems.Add((DataItemRecord)record);
                iPos++;
                record = (BiffRecordRaw)data[iPos];
            }

            while (record.TypeCode == TBIFFRecord.LineItemArray)
            {
                LineItemArrayRecord lineItem = (LineItemArrayRecord)record;
                lineItem.ParseStructure(arrFieldsCount[iLineItemIndex++]);
                m_arrLineItems.Add(lineItem);
                iPos++;
                record = (BiffRecordRaw)data[iPos];
            }

            record.CheckTypeCode(TBIFFRecord.ViewExtendedInfo);
            m_viewExInfo = (ViewExtendedInfoRecord)record;

            iPos++;
            record = (BiffRecordRaw)data[iPos];

            while (Array.IndexOf(DEF_UNKNOWN_PIVOTRECORDS, record.TypeCode) != -1)
            {
                m_arrUnknown.Add(record);
                iPos++;
                record = (BiffRecordRaw)data[iPos];
            }

            return iPos;
        }
        #endregion

        #region Class serialization methods
        /// <summary>
        /// Saves pivot table into OffsetArrayList.
        /// </summary>
        /// <param name="records">OffsetArrayList that will get all pivot table records.</param>
        [CLSCompliant(false)]
        public void Serialize(OffsetArrayList records)
        {
            if (records == null)
                throw new ArgumentNullException("records");

            records.Add(m_viewDefinition);

            for (int i = 0, len = m_arrFields.Count; i < len; i++)
            {
                PivotFieldImpl field = m_arrFields[i];
                field.Serialize(records);
            }

            for (int i = 0, len = m_arrRowColumnFiledId.Length; i < len; i++)
            {
                RowColumnFiledIdRecord id = m_arrRowColumnFiledId[i];

                if (id == null) break;

                records.Add(id);
            }

            if (m_pageItem != null) records.Add(m_pageItem);

            records.AddList(m_arrDataItems);
            records.AddList(m_arrLineItems);
            records.Add(m_viewExInfo);
            records.AddList(m_arrUnknown);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Clears all internal collections.
        /// </summary>
        private void ClearCollections()
        {
            m_arrFields.Clear();
            m_arrLineItems.Clear();
            m_arrDataItems.Clear();
            m_arrUnknown.Clear();
        }

        internal void RemovePivotField(PivotAxisTypes pivotAxisTypes, PivotFieldImpl field)
        {
            List<PivotFieldImpl> fields = GetFields(pivotAxisTypes);
            switch (pivotAxisTypes)
            {
                case PivotAxisTypes.Row:
                    PivotRowFields.Remove(field);
                    break;
                case PivotAxisTypes.Page:
                    PivotPageFields.Remove(field);
                    break;
            }
            if (fields != null)
                fields.Remove(field);
        }

        internal void AddPivotField(PivotAxisTypes pivotAxisTypes, PivotFieldImpl field,bool isData)
        {
            List<PivotFieldImpl> fields = GetFields(pivotAxisTypes);

            if (fields != null)
                fields.Add(field);
            else if (m_arrPivotFields.Count < Cache.CacheFields.Count)
            {
                m_arrPivotFields.Add(field);
                    field.Axis= pivotAxisTypes;
            }
            if (isData)
            {
                string name = DefaultDataFieldStart + " " + field.Name;
                DataFields.Add(field, name, PivotSubtotalTypes.Sum);
            }

        }
        internal List<PivotFieldImpl> GetFields(PivotAxisTypes pivotAxisTypes)
        {
            List<PivotFieldImpl> fields = null;

            switch (pivotAxisTypes)
            {
                case PivotAxisTypes.Row:
                    fields = m_lstRowFields;
                    break;

                case PivotAxisTypes.Column:
                    fields = m_lstColumnFields;
                    break;

                case PivotAxisTypes.Page:
                    fields = m_lstPageFields;
                    break;
            }

            return fields;
        }
        /// <summary>
        /// Gets the collection of pivot fields based on pivot axis.
        /// </summary>
        /// <param name="pivotAxisTypes">axis type.</param>
        /// <returns>Pivot fields collection.</returns>
        internal PivotTableFields GetPivotFields(PivotAxisTypes pivotAxisTypes)
        {
            PivotTableFields pivotFields = new PivotTableFields(Workbook.Application, this);
            foreach(PivotFieldImpl field in m_arrPivotFields)
                if(field.Axis== pivotAxisTypes)
                    pivotFields.Add(field);
            return pivotFields;
        }

        /// <summary>
        /// Get the Workbook equalent of 
        /// Pivot table version
        /// </summary>
        /// <returns></returns>
        internal byte GetPivotVersion()
        {
            if (m_book.Version == ExcelVersion.Excel2007)
                return Excel2007Version;
            else if (m_book.Version == ExcelVersion.Excel2010)
                return Excel2010Version;
            else if (m_book.Version == ExcelVersion.Excel2013)
                return Excel2013Version;
            else
                return 2;
        }
        #endregion

        #region ICloneParent Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="parent">Parent object for a copy of this instance.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone(object parent)
        {
            return Clone(parent, CacheIndex, null);
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="parent">Parent object for a copy of this instance.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone(object parent, int cacheIndex, Dictionary<string, string> hashWorksheetNames)
        {
            PivotTableImpl result = (PivotTableImpl)MemberwiseClone();
            result.SetParent(parent);
            result.SetWorkbook();

            result.m_viewDefinition = (PivotViewDefinitionRecord)CloneUtils.CloneCloneable(m_viewDefinition);
            result.m_viewExInfo = (ViewExtendedInfoRecord)CloneUtils.CloneCloneable(m_viewExInfo);
            result.m_pageItem = (PageItemRecord)CloneUtils.CloneCloneable(m_pageItem);
            result.m_arrFields = CloneUtils.CloneCloneable(m_arrFields, result);
            result.m_arrLineItems = CloneUtils.CloneCloneable(m_arrLineItems);
            result.m_arrDataItems = CloneUtils.CloneCloneable(m_arrDataItems);
            result.m_arrUnknown = CloneUtils.CloneCloneable(m_arrUnknown);
            result.m_arrRowColumnFiledId = new RowColumnFiledIdRecord[] { null, null };

            result.m_arrRowColumnFiledId[0] = (RowColumnFiledIdRecord)
              CloneUtils.CloneCloneable(m_arrRowColumnFiledId[0]);

            result.m_arrRowColumnFiledId[1] = (RowColumnFiledIdRecord)
              CloneUtils.CloneCloneable(m_arrRowColumnFiledId[1]);

            result.CacheIndex = cacheIndex;

            result.m_arrPivotFields = (PivotTableFields)CloneUtils.CloneCloneable((ICloneParent)m_arrPivotFields, result);
            result.m_dataFields = (PivotDataFields)CloneUtils.CloneCloneable((ICloneParent)m_dataFields, result);

            object newParent = result.FindParent(typeof(WorksheetImpl));

            if (m_location != null)
                result.m_location = ((ICombinedRange)m_location).Clone(newParent, hashWorksheetNames, result.m_book);

            return result;

            //private List<PivotFieldImpl> m_arrFields = new List<PivotFieldImpl>();
            //private PivotTableFields m_arrPivotFields;
            //private IRange m_location;
            //private PivotDataFields m_dataFields;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <param name="parent">Parent object for a copy of this instance.</param>
        /// <returns>A new object that is a copy of this instance.</returns>
        internal PivotTableImpl Clone(PivotTableCollection tables, Dictionary<string, string> hashWorksheetNames)
        {
            WorksheetImpl worksheet = tables.ParentWorksheet;
            WorkbookImpl newBook = worksheet.ParentWorkbook;
            int iCacheIndex = CacheIndex;

            // Copy cache object if necessary.
            if (newBook != Workbook)
            {
                if (newBook.Version != Workbook.Version)
                    throw new InvalidOperationException("Cannot copy pivot tables between workbooks with different versions");

                //if( newBook.Version == ExcelVersion.Excel2007 || newbook.Version == ExcelVersion.Excel2010 )
                {
                    // 1. Pivot cache was parsed
                    PivotCacheImpl cache = Workbook.PivotCaches[CacheIndex];
                    PivotCacheCollection caches = (newBook as IWorkbook).PivotCaches as PivotCacheCollection; // cause cache creation.
                    cache = (PivotCacheImpl)cache.Clone(caches, hashWorksheetNames);
                    caches.Add(cache);
                    iCacheIndex = cache.Index ;
                }

                //else
                //{
                //  // 2. Pivot cache wasn't parsed.
                //  throw new NotImplementedException();
                //}
            }

            // Create copy of all internal data
            PivotTableImpl result = (PivotTableImpl)Clone(tables, iCacheIndex, hashWorksheetNames);

            return result;
        }
        #endregion

        /// <summary>
        /// This method autofit the tabular layout pivot table only.
        /// Note: Compact layout and outline layout are autofitted by MS Excel itself.
        /// this method should be removed once tabular form layout is supported using Pivot engine layout.
        /// </summary>
        /// <param name="m_pivotTable"></param>
        public void AutoFitPivotTable(PivotTableImpl m_pivotTable)
        {

            List<IPivotField> rowFields = m_pivotTable.PivotRowFields;
           
            int columnNumber = m_pivotTable.Location.Column;
            foreach (IPivotField field in m_pivotTable.PivotRowFields)
            {
                int currentRow = m_pivotTable.Location.Row;

                //Temp value for dropdown symbol which exists in field header
                string tempDropDown = "drp";

                PivotFieldImpl fieldImpl = field as PivotFieldImpl;
                string maxValue = fieldImpl.CacheField.Name + tempDropDown  ;
                int maxLength = maxValue.Length;
                PivotCacheFieldImpl cacheField = fieldImpl.CacheField;
                
                for (int i = 0; i < cacheField.Items.Count; i++)
                {
                    string currentValue = cacheField.Items[i].ToString();
                    if (fieldImpl.Subtotals != PivotSubtotalTypes.None)
                        currentValue = currentValue + "Totals";

                    if (currentValue.Length > maxLength)
                    {
                        maxLength = currentValue.Length;
                        maxValue = currentValue;
                    }
                }

                m_pivotTable.Worksheet.Range[currentRow, columnNumber].Text = maxValue;
                if (maxValue != null || maxValue != string.Empty)
                    m_pivotTable.Worksheet.AutofitColumn(columnNumber);
                m_pivotTable.Worksheet.Range[currentRow, columnNumber].Text = string.Empty;
                
                columnNumber++;
            }
        }

        internal void MoveLocation(int delta)
        {
            //throw new NotImplementedException();
            int iFirstRow = m_location.Row;
            int iLastRow = m_location.LastRow;
            int iFirstCol = m_location.Column;
            int iLastCol = m_location.LastColumn;

            int iCurrentCount = GetPageFieldsCount();

            int iNewCount = iCurrentCount + delta;

            if (iCurrentCount != 0)
                iCurrentCount++;

            if (iNewCount != 0)
                iNewCount++;
            m_location = m_location[iFirstRow - iCurrentCount + iNewCount, iFirstCol,
              iLastRow - iCurrentCount + iNewCount, iLastCol];
        }


        private int GetPageFieldsCount()
        {
            int result = 0;

            for (int i = 0, len = Fields.Count; i < len; i++)
            {
                if (Fields[i].Axis == PivotAxisTypes.Page)
                    result++;
            }

            return result;
        }
        /// <summary>
        /// Invoke when changes made in the pivo table.
        /// </summary>
        internal void SetChanged(bool isClearPivot)
        {
            m_bIsChanged = true;
#if !SILVERLIGHT && !WINRT && !WP && !SyncfusionFramework2_0 && !(WINRT )
            if (this .PivotEngineValues == null )
            Cache.IsRefreshOnLoad = true;
            else
                Cache .IsRefreshOnLoad = false;
#else 
           Cache.IsRefreshOnLoad = true; 
#endif
            if (isClearPivot)
            {
                ClearPivotRange();
                ColumnItemsStream = null;
                RowItemsStream = null;
                PreservedElements.Clear();
            }
        }
        /// <summary>
        /// Clear the data and the format of the pivot table
        /// </summary>
        internal void ClearPivotRange()
        {
            if (m_location != null)
            {

                IRange range = m_location;

                m_location = m_worksheet[range.Row, range.Column];

                int row = range.Row - (1 + PageFields.Count);
                if (row == 0)
                    row = 1;
                range = m_worksheet[row, range.Column, range.LastRow, range.LastColumn+1];
                range.Clear(true);
            }
        }

        /// <summary>
        /// Returns the collection of calculated fields in the specified pivot table
        /// </summary>
        /// <returns>pivot calculated field collection</returns>
        internal PivotCalculatedFields GetCalculatedFields()
        {
            PivotCalculatedFields fields=new PivotCalculatedFields(this);
            for (int i = 0; i < Fields.Count; i++)
            {
                if (Fields[i].IsFormulaField)
                    fields.Add(Fields[i]);
            }
            return fields;
        }
        /// <summary>
        /// This method clears all the fields, deletes all filtering and sorting applied to the PivotTable.
        /// </summary>
        public void ClearTable()
        {
            ClearPivotRange();
            foreach (PivotFieldImpl field in Fields)
            {
                field.Axis = PivotAxisTypes.None;
                field.IsDataField = false;
            }

        }
    }
}
