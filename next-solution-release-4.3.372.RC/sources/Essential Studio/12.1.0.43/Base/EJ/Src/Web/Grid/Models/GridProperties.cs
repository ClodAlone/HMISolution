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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.Shared;


namespace Syncfusion.JavaScript.Models
{
    public class GridProperties<T> where T : class
    {
        #region Fields

        private bool allowPaging = false;
        private bool allowFiltering = false;
        private bool allowGrouping = false;
        private bool allowEditing = false;
        private bool allowSelection = true;
        private bool allowSorting = false;
        private bool allowResizing = false;
        private bool allowResizeToFit = false;
        private int selectedRow = -1;
        private object dataSource = new object();
        private string query = null;
        private bool enableEffects = true;
        private bool allowSummary = false;
        private string cssClass = null;
        private bool rowHover = true;
        private bool enablePersist = false;
        private bool editOnDoubleClick = true;
        private bool allowSearching = false;
        private bool headerEffect = false;
        private bool allowReordering = false;
        private bool allowKeyboardNavigation = true;
        private bool allowScrolling = false;
        private SelectionType selectionType = SelectionType.Single;
        private String localization = "en-US";
        private bool autoSaveOnRowSelection = true;
        private bool allowMultiSorting = false;
        private String detailTemplate = null;
        private String rowTemplate = null;
        private bool rtl = false;
        private bool altRow = true;
        private List<Column<T>> columns = new List<Column<T>>();
        private PageOptions<T> pageOption = new PageOptions<T>();
        private GroupOptions<T> groupOption = new GroupOptions<T>();
        private FilterOptions<T> filterOption = new FilterOptions<T>();
        private SortOptions<T> sortOption = new SortOptions<T>();
        private ScrollOptions<T> scrollOption = new ScrollOptions<T>();
        private EditOptions<T> editOption = new EditOptions<T>();
        private ToolBar<T> toolBar = new ToolBar<T>();
        private List<SummaryRows<T>> summaryRows = new List<SummaryRows<T>>();
        private String load = null;
        private String create = null;
        private String destroy = null;
        private String actionbegin = null;
        private String actioncomplete = null;
        private String beginedit = null;
        private String rowdatabound = null;
        private String rowselecting = null;
        private String rowselected = null;
        private String drag = null;
        private String dragstop = null;
        private String detaildata = null;
        private String detailexpand = null;
        private String detailcollapse = null;
        private String dragStart = null;
        private String querycellinfo = null;
        private String cellSave = null;
        private String beforeBulkDelete = null;
        private String bulkDelete = null;
        private String beforeBulkAdd = null;
        private String bulkAdd = null;
        private String beforeBulkSave = null;
        private String cellEdit = null;
        private String toolBarClick = null;
        private string recordDoubleClick = null;
        private string recordClick = null;
        private string gridRightClick = null;
        private string resizeStart = null;
        private string resizeEnd = null;
        private string resized = null;

        #endregion
        public GridProperties() { }
        //public GridProperties(String id) { new Grid<T>(id); }

        #region Properties

        [JsonProperty("allowPaging")]
        [DefaultValue(false)]
        public bool AllowPaging
        {
            get { return this.allowPaging; }
            set { this.allowPaging = value; }
        }
        [JsonProperty("allowSorting")]
        [DefaultValue(false)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }
        [JsonProperty("allowFiltering")]
        [DefaultValue(false)]
        public bool AllowFiltering
        {
            get { return this.allowFiltering; }
            set { this.allowFiltering = value; }
        }
        [JsonProperty("allowGrouping")]
        [DefaultValue(false)]
        public bool AllowGrouping
        {
            get { return this.allowGrouping; }
            set { this.allowGrouping = value; }
        }
        [JsonProperty("allowEditing")]
        [DefaultValue(false)]
        public bool AllowEditing
        {
            get { return this.allowEditing; }
            set { this.allowEditing = value; }
        }
        [JsonProperty("allowSelection")]
        [DefaultValue(true)]
        public bool AllowSelection
        {
            get { return this.allowSelection; }
            set { this.allowSelection = value; }
        }
        [JsonProperty("allowResizing")]
        [DefaultValue(false)]
        public bool AllowResizing
        {
            get { return this.allowResizing; }
            set { this.allowResizing = value; }
        }
        [JsonProperty("allowResizeToFit")]
        [DefaultValue(false)]
        public bool AllowResizeToFit
        {
            get { return this.allowResizeToFit; }
            set { this.allowResizeToFit = value; }
        }
        [JsonProperty("selectedRow")]
        [DefaultValue(-1)]
        public int SelectedRow
        {
            get { return this.selectedRow; }
            set { this.selectedRow = value; }
        }
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }

        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.query; }
            set { this.query = value; }
        }

        [JsonProperty("enableEffects")]
        [DefaultValue(true)]
        public bool EnableEffects
        {
            get { return this.enableEffects; }
            set { this.enableEffects = value; }
        }
        [JsonProperty("allowSummary")]
        [DefaultValue(false)]
        public bool AllowSummary
        {
            get { return this.allowSummary; }
            set { this.allowSummary = value; }
        }
        [JsonProperty("rowHover")]
        [DefaultValue(true)]
        public bool RowHover
        {
            get { return this.rowHover; }
            set { this.rowHover = value; }
        }
        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool EnablePersist
        {
            get { return this.enablePersist; }
            set { this.enablePersist = value; }
        }
        [JsonProperty("editOnDoubleClick")]
        [DefaultValue(true)]
        public bool EditOndoubleClick
        {
            get { return this.editOnDoubleClick; }
            set { this.editOnDoubleClick = value; }
        }
        [JsonProperty("allowSearching")]
        [DefaultValue(false)]
        public bool AllowSearching
        {
            get { return this.allowSearching; }
            set { this.allowSearching = value; }
        }
        [JsonProperty("headerEffects")]
        [DefaultValue(false)]
        public bool HeaderEffect
        {
            get { return this.headerEffect; }
            set { this.headerEffect = value; }
        }
        [JsonProperty("allowReordering")]
        [DefaultValue(false)]
        public bool AllowReordering
        {
            get { return this.allowReordering; }
            set { this.allowReordering = value; }
        }
        [JsonProperty("allowKeyboardNavigation")]
        [DefaultValue(true)]
        public bool AllowKeyboardNavigation
        {
            get { return this.allowKeyboardNavigation; }
            set { this.allowKeyboardNavigation = value; }
        }
        [JsonProperty("selectionType")]
        [DefaultValue(SelectionType.Single)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SelectionType Selectiontype
        {
            get { return this.selectionType; }
            set { this.selectionType = value; }
        }
        [JsonProperty("allowScrolling")]
        [DefaultValue(false)]
        public bool AllowScrolling
        {
            get { return this.allowScrolling; }
            set { this.allowScrolling = value; }
        }
        [JsonProperty("localization")]
        [DefaultValue("EN-US")]
        public String Localization
        {
            get { return this.localization; }
            set { this.localization = value; }
        }
        [JsonProperty("autoSaveOnRowSelection")]
        [DefaultValue(true)]
        public bool AutoSaveOnRowSelection
        {
            get { return this.autoSaveOnRowSelection; }
            set { this.autoSaveOnRowSelection = value; }
        }
        [JsonProperty("allowMultiSorting")]
        [DefaultValue(false)]
        public bool AllowMultiSorting
        {
            get { return this.allowMultiSorting; }
            set { this.allowMultiSorting = value; }
        }
        [JsonProperty("detailTemplate")]
        [DefaultValue(null)]
        public String DetailTemplate
        {
            get { return this.detailTemplate; }
            set { this.detailTemplate = value; }
        }
        [JsonProperty("rowTemplate")]
        [DefaultValue(null)]
        public String RowTemplate
        {
            get { return this.rowTemplate; }
            set { this.rowTemplate = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool RTL
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("altRow")]
        [DefaultValue(true)]
        public bool AltRow
        {
            get { return this.altRow; }
            set { this.altRow = value; }
        }
        [JsonProperty("columns")]
        public List<Column<T>> Columns
        {
            get { return this.columns; }
            set { this.columns = value; }
        }
        [JsonProperty("pageSettings")]

        public PageOptions<T> PageOption
        {
            get { return this.pageOption; }
            set { this.pageOption = value; }
        }

        [JsonProperty("groupSettings")]

        public GroupOptions<T> GroupOption
        {
            get { return this.groupOption; }
            set { this.groupOption = value; }
        }
        [JsonProperty("filterSettings")]

        public FilterOptions<T> FilterOption
        {
            get { return this.filterOption; }
            set { this.filterOption = value; }
        }
        [JsonProperty("sortSettings")]

        public SortOptions<T> SortOption
        {
            get { return this.sortOption; }
            set { this.sortOption = new SortOptions<T>(); this.sortOption = value; }
        }

        [JsonProperty("scrolling")]

        public ScrollOptions<T> ScrollOption
        {
            get { return this.scrollOption; }
            set { this.scrollOption = value; }
        }
        [JsonProperty("toolBar")]

        public ToolBar<T> Toolbar
        {
            get { return this.toolBar; }
            set { this.toolBar = value; }
        }
        [JsonProperty("edit")]

        public EditOptions<T> EditOption
        {

            get { return this.editOption; }
            set
            {

                this.editOption = value;
            }
        }
        [JsonProperty("summaryRows")]
        public List<SummaryRows<T>> SummaryRow
        {
            get { return this.summaryRows; }
            set { this.summaryRows = value; }
        }

        [JsonProperty("load")]
        [DefaultValue(null)]
        public String Load
        {
            get { return this.load; }
            set { this.load = value; }
        }
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        [JsonProperty("actionBegin")]
        [DefaultValue(null)]
        public String ActionBegin
        {
            get { return this.actionbegin; }
            set { this.actionbegin = value; }
        }
        [JsonProperty("actionComplete")]
        [DefaultValue(null)]
        public String ActionComplete
        {
            get { return this.actioncomplete; }
            set { this.actioncomplete = value; }
        }
        [JsonProperty("beginEdit")]
        [DefaultValue(null)]
        public String BeginEdit
        {
            get { return this.beginedit; }
            set { this.beginedit = value; }
        }
        [JsonProperty("rowDataBound")]
        [DefaultValue(null)]
        public String RowDataBound
        {
            get { return this.rowdatabound; }
            set { this.rowdatabound = value; }
        }
        [JsonProperty("rowSelecting")]
        [DefaultValue(null)]
        public String RowSelecting
        {
            get { return this.rowselecting; }
            set { this.rowselecting = value; }
        }
        [JsonProperty("rowSelected")]
        [DefaultValue(null)]
        public String RowSelected
        {
            get { return this.rowselected; }
            set { this.rowselected = value; }
        }
        [JsonProperty("drag")]
        [DefaultValue(null)]
        public String Drag
        {
            get { return this.drag; }
            set { this.drag = value; }
        }
        [JsonProperty("dragStop")]
        [DefaultValue(null)]
        public String DragStop
        {
            get { return this.dragstop; }
            set { this.dragstop = value; }
        }
        [JsonProperty("detailData")]
        [DefaultValue(null)]
        public String DetailData
        {
            get { return this.detaildata; }
            set { this.detaildata = value; }
        }
        [JsonProperty("detailExpand")]
        [DefaultValue(null)]
        public String DetailExpand
        {
            get { return this.detailexpand; }
            set { this.detailexpand = value; }
        }
        [JsonProperty("detailCollapse")]
        [DefaultValue(null)]
        public String DetailCollapse
        {
            get { return this.detailcollapse; }
            set { this.detailcollapse = value; }
        }
        [JsonProperty("dragStart")]
        [DefaultValue(null)]
        public string DragStart
        {
            get { return this.dragStart; }
            set { this.dragStart = value; }
        }
        [JsonProperty("queryCellInfo")]
        [DefaultValue(null)]
        public string QueryCellInfo
        {
            get { return this.querycellinfo; }
            set { this.querycellinfo = value; }
        }
        [JsonProperty("cellSave")]
        [DefaultValue(null)]
        public string CellSave
        {
            get { return this.cellSave; }
            set { this.cellSave = value; }
        }
        [JsonProperty("cellEdit")]
        [DefaultValue(null)]
        public string CellEdit
        {
            get { return this.cellEdit; }
            set { this.cellEdit = value; }
        }
        [JsonProperty("beforeBulkDelete")]
        [DefaultValue(null)]
        public string BeforeBulkDelete
        {
            get { return this.beforeBulkDelete; }
            set { this.beforeBulkDelete = value; }
        }
        [JsonProperty("bulkDelete")]
        [DefaultValue(null)]
        public string BulkDelete
        {
            get { return this.bulkDelete; }
            set { this.bulkDelete = value; }
        }
        [JsonProperty("beforeBulkAdd")]
        [DefaultValue(null)]
        public string BeforeBulkAdd
        {
            get { return this.beforeBulkAdd; }
            set { this.beforeBulkAdd = value; }
        }
        [JsonProperty("bulkAdd")]
        [DefaultValue(null)]
        public string BulkAdd
        {
            get { return this.bulkAdd; }
            set { this.bulkAdd = value; }
        }
        [JsonProperty("beforeBulkSave")]
        [DefaultValue(null)]
        public string BeforeBulkSave
        {
            get { return this.beforeBulkSave; }
            set { this.beforeBulkSave = value; }
        }
        [JsonProperty("toolBarClick")]
        [DefaultValue(null)]
        public string ToolBarClick
        {
            get { return this.toolBarClick; }
            set { this.toolBarClick = value; }
        }
        [JsonProperty("recordDoubleClick")]
        [DefaultValue(null)]
        public string RecordDoubleClick
        {
            get { return this.recordDoubleClick; }
            set { this.recordDoubleClick = value; }
        }
        [JsonProperty("recordClick")]
        [DefaultValue(null)]
        public string RecordClick
        {
            get { return this.recordClick; }
            set { this.recordClick = value; }
        }
        [JsonProperty("gridRightClick")]
        [DefaultValue(null)]
        public string GridRightClick
        {
            get { return this.gridRightClick; }
            set { this.gridRightClick = value; }
        }
        [JsonProperty("resizeStart")]
        [DefaultValue(null)]
        public String ResizeStart
        {
            get { return this.resizeStart; }
            set { this.resizeStart = value; }
        }
        [JsonProperty("resizeEnd")]
        [DefaultValue(null)]
        public String ResizeEnd
        {
            get { return this.resizeEnd; }
            set { this.resizeEnd = value; }
        }
        [JsonProperty("resized")]
        [DefaultValue(null)]
        public String Resized
        {
            get { return this.resized; }
            set { this.resized = value; }
        }
        
        #endregion


        #region ShouldSerialize Methods

        public bool ShouldSerializePageOption()
        {
            if (Utils.PropertyCompare(PageOption, new PageOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeGroupOption()
        {
            if (Utils.PropertyCompare(GroupOption, new GroupOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeScrollOption()
        {
            if (Utils.PropertyCompare(ScrollOption, new ScrollOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeEditOption()
        {
            if (Utils.PropertyCompare(EditOption, new EditOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFilterOption()
        {

            if (Utils.PropertyCompare(FilterOption, new FilterOptions<T>()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSortOption()
        {
            if (Utils.PropertyCompare(SortOption, new SortOptions<T>()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeToolbar()
        {
            if (Utils.PropertyCompare(Toolbar, new ToolBar<T>()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeColumns()
        {
            if (Columns.Count != 0)
                return true;
            else
                return false;
        }

        public bool ShouldSerializeSummaryRow()
        {
            if (SummaryRow.Count != 0)
                return true;
            else
                return false;
        }


        #endregion


    }
}
