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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlSerialization.PivotTables;
#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#endif
#if WP
using Syncfusion.XlsIO.Implementation.WP;
#endif
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif



namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Summary description for PivotFieldImpl.
    /// </summary>
    public class PivotFieldImpl :
      ICloneParent,
      IPivotField
    {
        #region Class members
        /// <summary>
        /// View field record.
        /// </summary>
        private PivotViewFieldsRecord m_viewFields = (PivotViewFieldsRecord)
          BiffRecordFactory.GetRecord(TBIFFRecord.PivotViewFields);
        /// <summary>
        /// Extended view fields record.
        /// </summary>
        private PivotViewFieldsExRecord m_viewFieldsEx = (PivotViewFieldsExRecord)
          BiffRecordFactory.GetRecord(TBIFFRecord.PivotViewFieldsEx);
        /// <summary>
        /// View items.
        /// </summary>
        private List<PivotViewItemRecord> m_arrItems = new List<PivotViewItemRecord>();
        /// <summary>
        /// Cache fields which this field is based on.
        /// </summary>
        private PivotCacheFieldImpl m_cacheField;
        /// <summary>
        /// Indicates whether this is data field.
        /// </summary>
        private bool m_bDataField;
        /// <summary>
        /// Parent pivot table.
        /// </summary>
        public PivotTableImpl m_table;
        /// <summary>
        /// Specifies a boolean value that indicates whether the application will display fields
        ///compactly in the sheet on which this PivotTable resides
        /// </summary>
        private bool m_bCompact;
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be removed from the
        ///PivotTable.
        /// </summary>
        private bool m_bIsDragOff;
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can be dragged to the data
        /// region.
        /// </summary>
        private bool m_bIsDragToData;
        /// <summary>
        /// Specifies a boolean value that indicates whether manual filter is in inclusive mode.
        /// </summary>
        private bool m_bShowNewItemsInFilter;
        /// <summary>
        /// Specifies a boolean value that indicates whether new items that appear after a refresh
        ///should be hidden by default.
        /// </summary>
        private bool m_bShowNewItemsOnRefresh;
        /// <summary>
        /// Specifies a boolean value that indicates whether to insert a blank row after each item.
        /// </summary>
        private bool m_bShowBlankRow;
        /// <summary>
        /// Specifies a boolean value that indicates whether to insert a page break after each item.
        /// </summary>
        private bool m_bShowPageBreak;
        /// <summary>
        /// Specifies the number of items showed per page in the PivotTable.
        /// </summary>
        private int m_iItemsPerPage = 10;
        /// <summary>
        /// Specifies the index of the item in the pivot cache
        /// </summary>
        private int m_iItemIndex = -1;
        /// <summary>
        /// Specifies a boolean value that indicates whether field has a measure based filter.
        /// </summary>
        private bool m_bMeasureField;
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can have multiple items
        ///selected in the page field.
        /// </summary>
        private bool m_bIsMultiSelected;
        /// <summary>
        /// Specifies a boolean value that indicates whether the items in this field should be shown
        ///in Outline form.
        /// </summary>
        private bool m_bShowOutline = true;
        /// <summary>
        /// Specifies a boolean value that indicates whether to hide drop down buttons on PivotField
        ///headers.
        /// </summary>
        private bool m_bShowDropDown;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the property as a member
        ///caption.
        /// </summary>
        private bool m_bShowPropAsCaption;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the member property value in a
        ///PivotTable cell.
        /// </summary>
        private bool m_bShowItemPropAsCaption;
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the member property value in a
        /// tooltip on the appropriate PivotTable cells.
        /// </summary>
        private bool m_bShowToolTip;
        /// <summary>
        /// Specifies the type of sort that is applied to this field.
        /// </summary>
        private PivotFieldSortType? m_sortType;
        /// <summary>
        /// Specifies a boolean value that indicates whether an AutoShow filter applied to this field is
        ///set to show the top ranked values
        /// </summary>
        private bool m_bIsAutoFiltersByRank;
        /// <summary>
        /// Specifies the unique name of the member property to be used as a caption for the field
        ///and field items.
        /// </summary>
        private string m_uniqueName;
        /// <summary>
        /// Represents the item attributes
        /// </summary>
        private Dictionary<int, PivotItemOptions> m_fieldItemOptions;
        /// <summary>
        /// Preserves the sorting elements of the field
        /// </summary>
        private Stream m_preservedAutoSort;
        /// <summary>
        /// Indicates whether all items in the field are expanded.
        /// </summary>
        private bool m_bIsAllDrilled;
        /// <summary>
        /// Indicates whether sort is applied to this field in the data source.
        /// </summary>
        private bool m_bIsDataSourceSorted;
        /// <summary>
        /// Indicates the drill state of the attribute hierarchy in an OLAP-based PivotTable.
        /// </summary>
        private bool m_bIsDefaultDrill;
        /// <summary>
        /// Indicates flexible storage extensions for pivot Field
        /// </summary>
        private Stream m_futureDataStorage;
        /// <summary>
        /// Indicates values of filter in page field.
        /// </summary>
        private string m_FilterValue;
        /// <summary>
        /// Pivot Filter collection of pivot fields
        /// </summary>
        private PivotFilterCollections m_pivotFilters;
        /// <summary>
        /// Pivot field items collections
        /// </summary>
        private PivotFieldItemsCollections m_PivotFieldItems;
        
        /// <summary>
        /// Number item of field set as invisible
        /// </summary>
        public int m_iItemInvisibleCount;
        #endregion

        #region Properties

       
        /// <summary>
        /// Get/Sets flexible storage extensions as stream for pivot Field
        /// </summary>
        internal Stream FutureDataStorageStream
        {
            get
            {
                return m_futureDataStorage;
            }
            set
            {
                m_futureDataStorage = value;
            }
        }
        /// <summary>
        /// Setting the position of the fields
        /// </summary>
        public int Position
        {
            get
            {
                int fieldPosition = 0;
                int fieldIndex;
                if (!m_table.Workbook.Loading)
                {
                    switch (this.Axis)
                    {
                        case PivotAxisTypes.Row:
                            fieldIndex = this.CacheField.Index;


                            foreach (int index in m_table.RowFieldsOrder)
                            {
                                if (index == fieldIndex)
                                    return fieldPosition;
                                fieldPosition++;
                            }
                            break;
                        case PivotAxisTypes.Column:
                            fieldIndex = this.CacheField.Index;
                            foreach (int index in m_table.ColFieldsOrder)
                            {
                                if (index == fieldIndex)
                                    return fieldPosition;
                                fieldPosition++;
                            }
                       
                            break;
                        case PivotAxisTypes.Page:
                             List<IPivotField > pageFields = m_table.PivotPageFields;
                             for (int index = 0; index < pageFields.Count; index++)
                                {
                                    
                                    if (((pageFields [index ]) as PivotFieldImpl).CacheField.Index == this.CacheField.Index)
                                    {
                                        return fieldPosition;

                                    }
                                    fieldPosition++;
                                }
                            break;
                        case PivotAxisTypes.None:
                        case PivotAxisTypes.Data:
                            if (this.IsDataField)
                            {
                                PivotDataFields dataFields = m_table.DataFields;
                                for (int index = 0; index < dataFields.Count; index++)
                                {
                                    
                                    PivotFieldImpl field = dataFields[index].Field;
                                    if (field .CacheField .Index == this.CacheField.Index)
                                    {
                                        return fieldPosition;

                                    }
                                    fieldPosition++;
                                }
                            }
                            else
                                throw new Exception("Specified field does not belongs to any fields type");
                            break;
                        default:
                            throw new Exception("Specified field does not belongs to any fields type");
                            break;
                    }
                }
                return fieldPosition+1;
            }
            set
            {
                if (!m_table.Workbook.Loading)
                {
                    m_table.SetChanged(true);
                    switch (this .Axis )
                    {
                        case PivotAxisTypes .Row :
                            MovePivotRowsFields(value );
                            break;
                        case PivotAxisTypes.Column :
                            MovePivotColumnFields(value);
                            break ;
                        case PivotAxisTypes.Page:
                            MovePivotPageFields(value);
                            break;
                        case PivotAxisTypes.None:
                        case PivotAxisTypes.Data:
                            if(this .IsDataField )
                                MovePivotDataFields(value );
                            else
                                throw new Exception("Specified field does not belongs to any fields type");
                            break ;
                        default :
                            throw new Exception("Specified field does not belongs to any fields type");
                            break;
                    }
                }
            }
        }

       
        /// <summary>
        /// Gets/sets field axis.
        /// </summary>
        public PivotAxisTypes Axis
        {
            get
            {
                return m_viewFields.Axis;
            }
            set
            {
                //bool isData = false;
                if (m_viewFields.Axis != value)
                {
                    if (!m_table.Workbook.Loading)
                    {
                        m_table.SetChanged(true);
                        if (m_viewFields.Axis == PivotAxisTypes.Page)
                            m_table.MoveLocation(-1);

                        if (value == PivotAxisTypes.Page)
                            m_table.MoveLocation(1);

                        if (m_viewFields.Axis == PivotAxisTypes.Column)
                            m_table.ColumnItemsStream = null;
                        else if (m_viewFields.Axis == PivotAxisTypes.Row)
                            m_table.RowItemsStream = null;
                        //if (value == PivotAxisTypes.Data)
                        //    isData = true;
                        if (value == PivotAxisTypes.Row)
                        {
                            m_table.PivotRowFields.Add(this);
                            m_table.RowFieldsOrder.Add(this.m_table.Fields.IndexOf(this));
                            if (m_table.ColFieldsOrder.Contains(this.m_table.Fields.IndexOf(this)))
                                m_table.ColFieldsOrder.Remove(this.m_table.Fields.IndexOf(this));
                        }
                        else if (value == PivotAxisTypes.Column)
                        {
                            m_table.ColFieldsOrder.Add(this.m_table.Fields.IndexOf(this));
                            if (m_table.RowFieldsOrder.Contains(this.m_table.Fields.IndexOf(this)))
                            {
                                m_table.RowFieldsOrder.Remove(this.m_table.Fields.IndexOf(this));
                                m_table.PivotRowFields.Remove (this);
                            }
                        }
                        else if (value == PivotAxisTypes.None || value == PivotAxisTypes.Page)
                        {
                            if (m_table.RowFieldsOrder.Contains(this.m_table.Fields.IndexOf(this)))
                            {
                                m_table.RowFieldsOrder.Remove(this.m_table.Fields.IndexOf(this));
                                m_table.PivotRowFields.Remove(this);
                            }
                            if (m_table.ColFieldsOrder.Contains(this.m_table.Fields.IndexOf(this)))
                                m_table.ColFieldsOrder.Remove(this.m_table.Fields.IndexOf(this));
                        }
                    }
                   
                    m_table.RemovePivotField(m_viewFields.Axis, this);
                    m_viewFields.Axis = value;
                    m_table.AddPivotField(value, this, false);

                }
            }
        }
        /// <summary>
        /// Gets/sets field Value.
        /// </summary>
        public string FilterValue
        {
            get
            {
                return m_FilterValue;
            }
            set
            {
                m_FilterValue = value;
            }
        }
        /// <summary>
        /// Returns pivot field name. Read-only.
        /// </summary>
        public string Name
        {
            get
            {
                return m_viewFields.Name;
            }
            set
            {
                m_viewFields.Name = value;
                if (this.IsDataField)
                {
                    foreach (PivotDataField dataField in m_table.DataFields)
                    {
                        if (dataField.Field.CacheField.Index == this.CacheField.Index)
                        {
                            dataField.Name = value;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Returns parent cache field. Read-only.
        /// </summary>
        public PivotCacheFieldImpl CacheField
        {
            get
            {
                return m_cacheField;
            }
        }
        /// <summary>
        /// Gets/sets value indicating whether this is data field.
        /// </summary>
        public bool IsDataField
        {
            get
            {
                return m_bDataField;
            }
            set
            {
                m_bDataField = value;
            }
        }
        /// <summary>
        /// Gets / sets number format index.
        /// </summary>
        public int NumberFormatIndex
        {
            get
            {
                return m_viewFieldsEx.NumberFormat;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                m_viewFieldsEx.NumberFormat = (ushort)value;
            }
        }
        /// <summary>
        /// Gets/sets number format.
        /// </summary>
        public string NumberFormat
        {
            get
            {
                FormatImpl format = m_table.Workbook.InnerFormats[NumberFormatIndex];
                return (format != null) ? format.FormatString : null;
            }
            set
            {
                NumberFormatIndex = m_table.Workbook.InnerFormats.FindOrCreateFormat(value);
            }
        }
        /// <summary>
        /// Gets or sets type of field subtotals.
        /// </summary>
        public PivotSubtotalTypes Subtotals
        {
            get
            {
                return m_viewFields.SubtotalType;
            }
            set
            {
                m_viewFields.SubtotalType = value;
            }
        }
        /// <summary>
        /// Specifies the custom text that is displayed for the subtotals label.
        /// </summary>
        internal string SubTotalName
        {
            get
            {
                return m_viewFieldsEx.SubTotalName;
            }
            set
            {
                m_viewFieldsEx.SubTotalName = value;
            }
        }
        /// <summary>
        /// Autoshow is enabled.
        /// </summary>
        public bool IsAutoShow
        {
            get
            {
                return m_viewFieldsEx.IsAutoShow;
            }
            set
            {
                m_viewFieldsEx.IsAutoShow = value;
            }
        }
        /// <summary>
        /// User can drag field to row area.
        /// </summary>
        public bool CanDragToRow
        {
            get
            {
                return m_viewFieldsEx.IsDragToRow;
            }
            set
            {
                m_viewFieldsEx.IsDragToRow = value;
            }
        }
        /// <summary>
        /// User can drag field to column area.
        /// </summary>
        public bool CanDragToColumn
        {
            get
            {
                return m_viewFieldsEx.IsDragToColumn;
            }
            set
            {
                m_viewFieldsEx.IsDragToColumn = value;
            }
        }
        /// <summary>
        /// User can drag field to page area.
        /// </summary>
        public bool CanDragToPage
        {
            get
            {
                return m_viewFieldsEx.IsDragToPage;
            }
            set
            {
                m_viewFieldsEx.IsDragToPage = value;
            }
        }
        /// <summary>
        /// User can remove field from fiew.
        /// </summary>
        public bool IsDragToHide
        {
            get
            {
                return m_viewFieldsEx.IsDragToHide;
            }
            set
            {
                m_viewFieldsEx.IsDragToHide = value;
            }
        }
        /// <summary>
        /// True if the field can be hidden by being dragged off the PivotTable report. The default value is True
        /// </summary>
        public bool CanDragOff
        {
            get
            {
                return m_bIsDragOff;
            }
            set
            {
                m_bIsDragOff = value;
            }
        }
		 /// <summary>
        /// Indicates whether excluded or included items should be tracked when manual filtering is applied to pivot field.
        /// </summary>
        public bool IncludeNewItemsInFilter
        {
            get
            {
                return m_bShowNewItemsInFilter;
            }
            set
            {
                m_bShowNewItemsInFilter = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether manual filter is in inclusive mode.
        /// </summary>
        public bool ShowNewItemsInFilter
        {
            get
            {
                return m_bShowNewItemsInFilter;
            }
            set
            {
                m_bShowNewItemsInFilter = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether new items that appear after a refresh
        ///should be hidden by default.
        /// </summary>
        public bool ShowNewItemsOnRefresh
        {
            get
            {
                return m_bShowNewItemsOnRefresh;
            }
            set
            {
                m_bShowNewItemsOnRefresh = value;
            }
        }
        /// <summary>
        /// True if a blank row is inserted after the specified row field in a PivotTable report.
        /// </summary>
        public bool ShowBlankRow
        {
            get
            {
                return m_bShowBlankRow;
            }
            set
            {
                m_bShowBlankRow = value;
            }
        }
        /// <summary>
        /// True if a page break is inserted after each field. 
        /// </summary>
        public bool ShowPageBreak
        {
            get
            {
                return m_bShowPageBreak;
            }
            set
            {
                m_bShowPageBreak = value;
            }
        }
        /// <summary>
        /// Specifies the number of items showed per page in the PivotTable.
        /// </summary>
        public int ItemsPerPage
        {
            get
            {
                return m_iItemsPerPage;
            }
            set
            {
                m_iItemsPerPage = value;
            }
        }
        /// <summary>
        /// Specifies the index of the item in the pivot cache
        /// </summary>
        internal int ItemIndex
        {
            get
            {
                return m_iItemIndex;
            }
            set
            {
                m_iItemIndex = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether field has a measure based filter.
        /// </summary>
        public bool IsMeasureField
        {
            get
            {
                return m_bMeasureField;
            }
            set
            {
                m_bMeasureField = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the field can have multiple items
        ///selected in the page field.
        /// </summary>
        public bool IsMultiSelected
        {
            get
            {
                return m_bIsMultiSelected;
            }
            set
            {
                m_bIsMultiSelected = value;
            }
        }

        /// <summary>
        /// Show all items for this field.
        /// </summary>
        public bool IsShowAllItems
        {
            get
            {
                return m_viewFieldsEx.IsShowAllItems;
            }
            set
            {
                m_viewFieldsEx.IsShowAllItems = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the items in this field should be shown
        ///in Outline form.
        /// </summary>
        public bool ShowOutline
        {
            get
            {
                return m_bShowOutline;
            }
            set
            {
                m_bShowOutline = value;
            }
        }
        /// <summary>
        /// True if the flag for the specified PivotTable field or PivotTable item is set to "drilled" (expanded, or visible).
        /// </summary>
        public bool ShowDropDown
        {
            get
            {
                return m_bShowDropDown;
            }
            set
            {
                m_bShowDropDown = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the property as a member
        ///caption.
        /// </summary>
        public bool ShowPropAsCaption
        {
            get
            {
                return m_bShowPropAsCaption;
            }
            set
            {
                m_bShowPropAsCaption = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the member property value in a
        ///PivotTable cell.
        /// </summary>
        public bool ShowItemPropAsCaption
        {
            get
            {
                return m_bShowItemPropAsCaption;
            }
            set
            {
                m_bShowItemPropAsCaption = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether to show the member property value in a
        /// tooltip on the appropriate PivotTable cells.
        /// </summary>
        public bool ShowToolTip
        {
            get
            {
                return m_bShowToolTip;
            }
            set
            {
                m_bShowToolTip = value;
            }
        }


        /// <summary>
        /// Specifies the type of sort that is applied to this field.
        /// </summary>
        public PivotFieldSortType? SortType
        {
            get
            {
                return m_sortType;
            }
            set
            {
                m_sortType = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether an AutoShow filter applied to this field is
        ///set to show the top ranked values
        /// </summary>
        public bool IsAutoFiltersByRank
        {
            get
            {
                return m_bIsAutoFiltersByRank;
            }
            set
            {
                m_bIsAutoFiltersByRank = value;
            }
        }
        /// <summary>
        /// Specifies the unique name of the member property to be used as a caption for the field
        ///and field items.
        /// </summary>
        public string Caption
        {
            get
            {
                return m_uniqueName;
            }
            set
            {
                m_uniqueName = value;
            }
        }
        /// <summary>
        /// Represents the item attributes
        /// </summary>
        internal Dictionary<int, PivotItemOptions> ItemOptions
        {
            get
            {
                if (m_fieldItemOptions == null)
                    m_fieldItemOptions = new Dictionary<int, PivotItemOptions>();
                return m_fieldItemOptions;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the application will display fields
        ///compactly in the sheet on which this PivotTable resides
        /// </summary>
        public bool Compact
        {
            get
            {
                return m_bCompact;
            }
            set
            {
                m_bCompact = value;
            }
        }
        /// <summary>
        /// True if the specified field can be dragged to the data position. The default value is True.
        /// </summary>
        public bool CanDragToData
        {
            get
            {
                return m_bIsDragToData;
            }
            set
            {
                m_bIsDragToData = value;
            }
        }
        /// <summary>
        /// Specifies the formula for the calculated field
        /// </summary>
        public string Formula
        {
            get
            {
                return m_cacheField.Formula;
            }
            set
            {
                m_cacheField.Formula = value;
            }
        }
        /// <summary>
        /// Indicates whether this field is formula field
        /// </summary>
        public bool IsFormulaField
        {
            get
            {
                return m_cacheField.IsFormulaField;
            }
        }
        /// <summary>
        /// Preserves the sorting elements of the field
        /// </summary>
        public Stream PreservedAutoSort
        {
            get
            {
                return m_preservedAutoSort;
            }
            set
            {
                m_preservedAutoSort = value;
            }

        }
        /// <summary>
        /// Indicates whether all items in the field are expanded.
        /// </summary>
        internal bool IsAllDrilled
        {
            get
            {
                return m_bIsAllDrilled;
            }
            set
            {
                m_bIsAllDrilled = value;
            }
        }
        /// <summary>
        /// Indicates whether sort is applied to this field in the data source.
        /// </summary>
        internal bool IsDataSourceSorted
        {
            get
            {
                return m_bIsDataSourceSorted;
            }
            set
            {
                m_bIsDataSourceSorted = value;
            }
        }
        /// <summary>
        /// Indicates the drill state of the attribute hierarchy in an OLAP-based PivotTable.
        /// </summary>
        internal bool IsDefaultDrill
        {
            get
            {
                return m_bIsDefaultDrill;
            }
            set
            {
                m_bIsDefaultDrill = value;
            }
        }

        /// <summary>
        /// Property for Pivot filter collections of pivot fields
        /// </summary>
        public IPivotFilters PivotFilters
        {
            get
            {
                if (m_pivotFilters == null)
                    m_pivotFilters = new PivotFilterCollections(this );
                return m_pivotFilters;
            }
        }

        /// <summary>
        /// Property for Pivot fields items.
        /// </summary>
        public IPivotFieldItems Items
        {
            get
            {
                if (m_PivotFieldItems == null)
                    m_PivotFieldItems = new PivotFieldItemsCollections();
                return m_PivotFieldItems;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        //public PivotFieldImpl( IApplication application, object parent )
        //{
        //  m_book = ( WorkbookImpl )CommonObject.FindParent( parent, typeof( WorkbookImpl ) );

        //  if( m_book == null )
        //    throw new ArgumentException( "parent" );
        //}
        /// <summary>
        /// Creates collection and sets its Application and Parent values.
        /// </summary>
        /// <param name="book">Parent workbook.</param>
        internal PivotFieldImpl(PivotTableImpl table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            m_table = table;
            Subtotals = PivotSubtotalTypes.Default;
            IsDragToHide = true;
            CanDragOff = true;
            CanDragToColumn = true;
            CanDragToData = true;
            CanDragToPage = true;
            CanDragToRow = true;
        }
        /// <summary>
        /// Initializes new instance of the pivot field.
        /// </summary>
        /// <param name="book">Parent workbook.</param>
        /// <param name="cacheField">Cache field that corresponds to this field.</param>
        public PivotFieldImpl(PivotCacheFieldImpl cacheField, PivotTableImpl table)
            : this(table)
        {
            m_viewFields.Name = cacheField.Name;
            m_viewFields.NumberItems = (ushort)cacheField.ItemCount;
            m_cacheField = cacheField;
            PivotFieldItemsCollections fieldItems = new PivotFieldItemsCollections();
            SortedList<PivotTableSerializator.ComparisonPair, object> lstFieldsData = PivotTableSerializator.SortFieldValues(this.CacheField);
            if (lstFieldsData.Count > 0)
            {
                int lstFieldsDataCount = 0;
                for (int fieldsIndex = 0; fieldsIndex < lstFieldsData.Count; fieldsIndex++)
                {
                    string value = "";
                    if (lstFieldsData.Keys[lstFieldsDataCount].Value != null && lstFieldsData.Keys[lstFieldsDataCount].Value != string.Empty)
                        value = lstFieldsData.Keys[lstFieldsDataCount].Value.ToString();
                    else
                        value = null;
                    PivotFieldItemsCollections Items = this.Items as PivotFieldItemsCollections;
                    Items.Add(this, value);
                    lstFieldsDataCount++;
                }
            }
        }
        #endregion

        #region Parse and serialize Methods
        /// <summary>
        /// Parses pivot field.
        /// </summary>
        /// <param name="data">Records collection with data.</param>
        /// <param name="iPos">Offset to the first pivot field record.</param>
        /// <returns>Offset to the record after field's records.</returns>
        public int Parse(IList data, int iPos)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (iPos < 0 || iPos > data.Count - 1)
                throw new ArgumentOutOfRangeException("iPos", "Value cannot be less than 0 and greater than data.Count - 1");

            BiffRecordRaw raw = (BiffRecordRaw)data[iPos];
            raw.CheckTypeCode(TBIFFRecord.PivotViewFields);
            m_viewFields = (PivotViewFieldsRecord)raw;
            iPos++;

            for (int i = 0, len = m_viewFields.NumberItems; i < len; i++)
            {
                raw = (BiffRecordRaw)data[iPos];
                raw.CheckTypeCode(TBIFFRecord.PivotViewItem);
                m_arrItems.Add((PivotViewItemRecord)raw);
                iPos++;
            }

            raw = (BiffRecordRaw)data[iPos];

            if (raw.TypeCode == TBIFFRecord.PivotViewFieldsEx)
            {
                m_viewFieldsEx = (PivotViewFieldsExRecord)raw;
                iPos++;
            }

            return iPos;
        }
        /// <summary>
        /// Saves pivot table into OffsetArrayList.
        /// </summary>
        /// <param name="records">OffsetArrayList that will get all pivot table records.</param>
        [CLSCompliant(false)]
        public void Serialize(OffsetArrayList records)
        {
            if (records == null)
                throw new ArgumentNullException("records");

            records.Add(m_viewFields);
            records.AddList(m_arrItems);
            records.Add(m_viewFieldsEx);
        }
        #endregion

        #region Methods
        public void AddItemOption(int index, PivotItemOptions item)
        {
            ItemOptions.Add(index, item);
        }
        public void AddItemOption(int index)
        {
            ItemOptions.Add(index, null);
        }

        /// <summary>
        /// Moves the position of the Row Field among the Row fields.
        /// </summary>
        /// <param name="newIndex"></param>
        private void MovePivotRowsFields(int newIndex)
        {
            
            if (newIndex >= m_table .PivotRowFields .Count)
                throw new ArgumentOutOfRangeException("Exceeds the max Row fields count");

            int maxRowFieldsCount = m_table.PivotRowFields.Count;
            int fieldIndex = this.CacheField.Index;
            m_table.RowFieldsOrder.Remove(fieldIndex);
            List<IPivotField> rowFields = m_table.PivotRowFields;

            rowFields.Remove(this);
            if (rowFields .Count> newIndex)
                rowFields.Insert(newIndex, this);
            else
                rowFields.Add(this);
            
            if (maxRowFieldsCount > newIndex)
                m_table.RowFieldsOrder.Insert(newIndex, fieldIndex);
            else
                m_table.RowFieldsOrder.Add(fieldIndex);
        }

        /// <summary>
        /// Move the position of the column field among column fields.
        /// </summary>
        /// <param name="newIndex"></param>
        private void MovePivotColumnFields(int newIndex)
        {
            if (newIndex > m_table.ColumnFields.Count)
                throw new ArgumentOutOfRangeException("Exceeds the max Column fields count");

            int fieldIndex = this.CacheField.Index;

            List<int> columnFieldsOrder = m_table.ColFieldsOrder;
            columnFieldsOrder.Remove(fieldIndex);
            
            if (columnFieldsOrder.Count > newIndex)
                columnFieldsOrder.Insert(newIndex, fieldIndex);
            else
               columnFieldsOrder.Add(fieldIndex);
        }

        /// <summary>
        /// Moves the position of the data field among the data fields
        /// </summary>
        /// <param name="newIndex"></param>
        private void MovePivotDataFields(int newIndex)
        {

            if (newIndex > m_table.DataFields.Count)
                throw new ArgumentOutOfRangeException("Exceeds the max Data fields count");

            int value = newIndex;
            int iFieldCount = -1;
            for (int i = 0; i < m_table.Fields.Count; i++)
            {
                if (m_table.Fields[i].IsDataField)
                    iFieldCount++;
                if (m_table.Fields[i].CacheField.Index == this.CacheField.Index)
                {

                    PivotDataFields dataFields = m_table.DataFields;
                    PivotDataField dataField = dataFields[iFieldCount];
                    dataFields.Remove(dataField);
                    if (dataFields.Count > value)
                        dataFields.Insert(value, dataField);
                    else
                        dataFields.Add(dataField);
                }
            }
        }

        /// <summary>
        /// Moves the position of page field among the page fields.
        /// </summary>
        /// <param name="newIndex"></param>
        private void MovePivotPageFields(int newIndex)
        {
            if (newIndex > m_table.PivotPageFields.Count)
                throw new ArgumentOutOfRangeException("Exceeds the max Page fields count");

            int fieldIndex = this.CacheField.Index;
            m_table.PivotPageFields.Remove(this);
            if (m_table.PivotPageFields.Count > newIndex)
                m_table.PivotPageFields.Insert(newIndex, this);
            else
                m_table.PivotPageFields.Add(this);
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
            PivotFieldImpl result = (PivotFieldImpl)MemberwiseClone();
            //result.m_book = ( WorkbookImpl )CommonObject.FindParent( parent, typeof( WorkbookImpl ) );
            result.m_table = (PivotTableImpl)CommonObject.FindParent(parent, typeof(PivotTableImpl));
            //SetParent( parent );
            //PivotTableImpl table = ( PivotTableImpl )CommonObject.FindParent( parent, typeof( PivotTableImpl ) );
            PivotCacheImpl cache = result.m_table.Cache;

            result.m_viewFields = (PivotViewFieldsRecord)CloneUtils.CloneCloneable(m_viewFields);
            result.m_viewFieldsEx = (PivotViewFieldsExRecord)CloneUtils.CloneCloneable(m_viewFieldsEx);
            result.m_arrItems = CloneUtils.CloneCloneable(m_arrItems);

            if (m_cacheField != null)
            {
                int iFieldIndex = m_cacheField.Index;
                result.m_cacheField = cache.CacheFields[iFieldIndex];
            }

            return result;
        }

        #endregion


    }
}
