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
    public class GanttProperties
    {
        #region Fields  

        private bool allowSorting = false;
        private bool allowColumnResize = false;
        private bool allowSelection = false;
        private bool enableRowHover = true;
        private object dataSource = new object();
        private string taskIdMapping = null;
        private string taskNameMapping = null;
        private string startDateMapping = null;
        private string endDateMapping = null;
        private string baselineStartDateMapping = null;
        private string baselineEndDateMapping = null;
        private string childMapping = null;
        private string durationMapping = null;
        private string mileStoneMapping = null;
        private string progressMapping = null;
        private string predecessorMapping = null;
        private string resourceInfoMapping = null;
        private object resourceCollection = new object();
        private HolidaysOptions holidaysOption = new HolidaysOptions();
        private bool highlightWeekEnds = true;
        private bool canResizeProgressBar = true;
        private object scheduleStartDate = null;
        private object scheduleEndDate = null;
        private int rowHeight = 35;
        private bool includeWeekend = true;
        private string taskbarBackground = "#DE8080";
        private string progressbarBackground = "#C44647";
        private string connectorLineBackground = "#383838";
        private string parentTaskbarBackground = "#383838";
        private string parentProgressbarBackground = "#1C1C1C";
        private int connectorlineWidth = 1;
        private bool showTaskNames = true;
        private bool showProgressStatus = true;
        private bool showResourceNames = true;
        private bool showTaskbarDragTooltip = true;
        private bool showTaskbarTooltip = true;
        private bool enableContextMenu = true;
        private ToolBarOptions toolBarOption = new ToolBarOptions();
        private StripLinesOptions stripLinesOption = new StripLinesOptions();
        private ScheduleHeaderOptions scheduleHeaderOption = new ScheduleHeaderOptions();
        private GanttEditOptions editOption = new GanttEditOptions();
        private SortSettingsOptions sortSettingsOption = new SortSettingsOptions();
        private SizeOptions sizeOption = new SizeOptions();
        private string expanderImagePath = null;
        private string collapsedIamgePath = null;
        private bool allowKeyboardNavigation = true;
        private string cssClass = null;
        private string localization = "en-US";
        private bool autoSaveOnRowSelection = true;
        private bool allowMultiSorting = false;
        private string rowTemplate = null;
        private string rowDataBound = null;
        private bool rtl = false;
        private bool altRow = true;
        private bool enableVirtualization = false;
        private int progressbarHeight = 100;
        private string taskbarTooltipTemplate = null;
        private bool allowZooming = false;
        private string dateFormat = null;
        private string resourceIdMapping = null;
        private string resourceNameMapping = null;
        private string progressbarTooltipTemplateID = null;
        private string taskbarEditingTooltipTemplateID = null;
        private int selectedRowIndex = -1;
        private string queryCellInfo = null;
        private string queryTaskbarInfo = null;
        private bool allowGanttChartEditing = true;
        private string selectedItem = null;
        private string weekendBackground = "#F2F2F2";
        private string baselineColor = "#fba41c";
        private bool renderBaseline = false;

        private string rowSelecting = null;
        private string rowSelected = null;
        private string actionBegin = null;
        private string actionComplete = null;
        private string beginEdit = null;
        private string endEdit = null;
        private string expanding = null;
        private string collapsing = null;
        private string expanded = null;
        private string collapsed = null;
        private string refreshRow = null;
        private string cancelEditCell = null;
        private string taskbarEditing = null;
        private string rowHover = null;
        private string taskbarEdited = null;
        private string contextMenuOpen = null;
        private string load = null;
        private string destroy = null;
        private string create = null;


        #endregion

        public GanttProperties()
        {
        }

        #region properties

        [JsonProperty("allowSorting")]
        [DefaultValue(false)]
        public bool AllowSorting
        {
            get { return this.allowSorting; }
            set { this.allowSorting = value; }
        }

        [JsonProperty("EnableContextMenu")]
        [DefaultValue(true)]
        public bool EnableContextMenu
        {
            get { return this.enableContextMenu; }
            set { this.enableContextMenu = value; }
        }

        [JsonProperty("allowColumnResize")]
        [DefaultValue(false)]
        public bool AllowColumnResize
        {
            get { return this.allowColumnResize; }
            set { this.allowColumnResize = value; }
        }

        [JsonProperty("allowSelection")]
        [DefaultValue(false)]
        public bool AllowSelection
        {
            get { return this.allowSelection; }
            set { this.allowSelection = value; }
        }


        [JsonProperty("enableRowHover")]
        [DefaultValue(true)]
        public bool EnableRowHover
        {
            get { return this.enableRowHover; }
            set { this.enableRowHover = value; }
        }



        [JsonProperty("dataSource")]
        [JsonConverter(typeof (DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }

        [JsonProperty("taskIdMapping")]
        [DefaultValue(null)]
        public string TaskIdMapping
        {
            get { return this.taskIdMapping; }
            set { this.taskIdMapping = value; }
        }

        [JsonProperty("taskNameMapping")]
        [DefaultValue(null)]
        public string TaskNameMapping
        {
            get { return this.taskNameMapping; }
            set { this.taskNameMapping = value; }
        }

        [JsonProperty("startDateMapping")]
        [DefaultValue(null)]
        public string StartDateMapping
        {
            get { return this.startDateMapping; }
            set { this.startDateMapping = value; }
        }

        [JsonProperty("endDateMapping")]
        [DefaultValue(null)]
        public string EndDateMapping
        {
            get { return this.endDateMapping; }
            set { this.endDateMapping = value; }
        }

        [JsonProperty("baselineStartDateMapping")]
        [DefaultValue(null)]
        public string BaselineStartDateMapping
        {
            get { return this.baselineStartDateMapping; }
            set { this.baselineStartDateMapping = value; }
        }

        [JsonProperty("baselineEndDateMapping")]
        [DefaultValue(null)]
        public string BaselineEndDateMapping
        {
            get { return this.baselineEndDateMapping; }
            set { this.baselineEndDateMapping = value; }
        }

        [JsonProperty("childMapping")]
        [DefaultValue(null)]
        public string ChildMapping
        {
            get { return this.childMapping; }
            set { this.childMapping = value; }
        }

        [JsonProperty("durationMapping")]
        [DefaultValue(null)]
        public string DurationMapping
        {
            get { return this.durationMapping; }
            set { this.durationMapping = value; }
        }

        [JsonProperty("mileStoneMapping")]
        [DefaultValue(null)]
        public string MileStoneMapping
        {
            get { return this.mileStoneMapping; }
            set { this.mileStoneMapping = value; }
        }

        [JsonProperty("progressMapping")]
        [DefaultValue(null)]
        public string ProgressMapping
        {
            get { return this.progressMapping; }
            set { this.progressMapping = value; }
        }

        [JsonProperty("predecessorMapping")]
        [DefaultValue(null)]
        public string PredecessorMapping
        {
            get { return this.predecessorMapping; }
            set { this.predecessorMapping = value; }
        }

        [JsonProperty("resourceInfoMapping")]
        [DefaultValue(null)]
        public string ResourceInfoMapping
        {
            get { return this.resourceInfoMapping; }
            set { this.resourceInfoMapping = value; }
        }

        [JsonProperty("resourceCollection")]
        [JsonConverter(typeof (DataManagerConverter))]
        public object ResourceCollection
        {
            get { return this.resourceCollection; }
            set { this.resourceCollection = value; }
        }

        [JsonProperty("holidays")]
        public HolidaysOptions HolidaysOption
        {
            get { return this.holidaysOption; }
            set { this.holidaysOption = value; }
        }

        [JsonProperty("highlightWeekEnds")]
        [DefaultValue(true)]
        public bool HighlightWeekEnds
        {
            get { return this.highlightWeekEnds; }
            set { this.highlightWeekEnds = value; }
        }

        [JsonProperty("canResizeProgressBar")]
        [DefaultValue(true)]
        public bool CanResizeProgressBar
        {
            get { return this.canResizeProgressBar; }
            set { this.canResizeProgressBar = value; }
        }

        [JsonProperty("scheduleStartDate")]
        [DefaultValue(null)]
        public object ScheduleStartDate
        {
            get { return this.scheduleStartDate; }
            set { this.scheduleStartDate = value; }
        }

        [JsonProperty("scheduleEndDate")]
        [DefaultValue(null)]
        public object ScheduleEndDate
        {
            get { return this.scheduleEndDate; }
            set { this.scheduleEndDate = value; }
        }

        [JsonProperty("rowHeight")]
        [DefaultValue(35)]
        public int RowHeight
        {
            get { return this.rowHeight; }
            set { this.rowHeight = value; }
        }

        [JsonProperty("includeWeekend")]
        [DefaultValue(true)]
        public bool IncludeWeekend
        {
            get { return this.includeWeekend; }
            set { this.includeWeekend = value; }
        }

        [JsonProperty("taskbarBackground")]
        [DefaultValue("#DE8080")]
        public string TaskbarBackground
        {
            get { return this.taskbarBackground; }
            set { this.taskbarBackground = value; }
        }

        [JsonProperty("progressbarBackground")]
        [DefaultValue("#C44647")]
        public string ProgressbarBackground
        {
            get { return this.progressbarBackground; }
            set { this.progressbarBackground = value; }
        }

        [JsonProperty("connectorLineBackground")]
        [DefaultValue("#383838")]
        public string ConnectorLineBackground
        {
            get { return this.connectorLineBackground; }
            set { this.connectorLineBackground = value; }
        }

        [JsonProperty("parentTaskbarBackground")]
        [DefaultValue("#383838")]
        public string ParentTaskbarBackground
        {
            get { return this.parentTaskbarBackground; }
            set { this.parentTaskbarBackground = value; }
        }

        [JsonProperty("parentProgressbarBackground")]
        [DefaultValue("#1C1C1C")]
        public string ParentProgressbarBackground
        {
            get { return this.parentProgressbarBackground; }
            set { this.parentProgressbarBackground = value; }
        }

        [JsonProperty("connectorlineWidth")]
        [DefaultValue(1)]
        public int ConnectorlineWidth
        {
            get { return this.connectorlineWidth; }
            set { this.connectorlineWidth = value; }
        }

        [JsonProperty("showTaskNames")]
        [DefaultValue(true)]
        public bool ShowTaskNames
        {
            get { return this.showTaskNames; }
            set { this.showTaskNames = value; }
        }

        [JsonProperty("showProgressStatus")]
        [DefaultValue(true)]
        public bool ShowProgressStatus
        {
            get { return this.showProgressStatus; }
            set { this.showProgressStatus = value; }
        }

        [JsonProperty("showResourceNames")]
        [DefaultValue(true)]
        public bool ShowResourceNames
        {
            get { return this.showResourceNames; }
            set { this.showResourceNames = value; }
        }

        [JsonProperty("showTaskbarDragTooltip")]
        [DefaultValue(true)]
        public bool ShowTaskbarDragTooltip
        {
            get { return this.showTaskbarDragTooltip; }
            set { this.showTaskbarDragTooltip = value; }
        }

        [JsonProperty("showTaskbarTooltip")]
        [DefaultValue(true)]
        public bool ShowTaskbarTooltip
        {
            get { return this.showTaskbarTooltip; }
            set { this.showTaskbarTooltip = value; }
        }

        [JsonProperty("toolBar")]
        public ToolBarOptions ToolBarOption
        {
            get { return this.toolBarOption; }
            set
            {
                this.toolBarOption = new ToolBarOptions();
                this.toolBarOption = value;
            }
        }

        [JsonProperty("stripLines")]
        public StripLinesOptions StripLinesOption
        {
            get { return this.stripLinesOption; }
            set
            {
                this.stripLinesOption = new StripLinesOptions();
                this.stripLinesOption = value;
            }
        }

        [JsonProperty("scheduleHeaderOption")]
        public ScheduleHeaderOptions ScheduleHeaderOption
        {
            get { return this.scheduleHeaderOption; }
            set
            {
                this.scheduleHeaderOption = new ScheduleHeaderOptions();
                this.scheduleHeaderOption = value;
            }
        }


        [JsonProperty("edit")]
        public GanttEditOptions EditOption
        {
            get { return this.editOption; }
            set
            {
                this.editOption = new GanttEditOptions();
                this.editOption = value;
            }
        }

        [JsonProperty("sortSettings")]
        public SortSettingsOptions SortSettingsOption
        {
            get { return this.sortSettingsOption; }
            set
            {
                this.sortSettingsOption = new SortSettingsOptions();
                this.sortSettingsOption = value;
            }
        }

        [JsonProperty("size")]
        public SizeOptions SizeOption
        {
            get { return this.sizeOption; }
            set
            {
                this.sizeOption = new SizeOptions();
                this.sizeOption = value;
            }
        }

        [JsonProperty("expanderImagePath")]
        [DefaultValue(null)]
        public string ExpanderImagePath
        {
            get { return this.expanderImagePath; }
            set { this.expanderImagePath = value; }
        }

        [JsonProperty("collapsedIamgePath")]
        [DefaultValue(null)]
        public string CollapsedIamgePath
        {
            get { return this.collapsedIamgePath; }
            set { this.collapsedIamgePath = value; }
        }

        [JsonProperty("allowKeyboardNavigation")]
        [DefaultValue(true)]
        public bool AllowKeyboardNavigation
        {
            get { return this.allowKeyboardNavigation; }
            set { this.allowKeyboardNavigation = value; }
        }

        [JsonProperty("cssClass")]
        [DefaultValue(null)]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        [JsonProperty("localization")]
        [DefaultValue("en-US")]
        public string Localization
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

        [JsonProperty("rowTemplate")]
        [DefaultValue(null)]
        public string RowTemplate
        {
            get { return this.rowTemplate; }
            set { this.rowTemplate = value; }
        }

        [JsonProperty("rowDataBound")]
        [DefaultValue(null)]
        public string RowDataBound
        {
            get { return this.rowDataBound; }
            set { this.rowDataBound = value; }
        }

        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
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

        [JsonProperty("enableVirtualization")]
        [DefaultValue(false)]
        public bool EnableVirtualization
        {
            get { return this.enableVirtualization; }
            set { this.enableVirtualization = value; }
        }

        [JsonProperty("progressbarHeight")]
        [DefaultValue(100)]
        public int ProgressbarHeight
        {
            get { return this.progressbarHeight; }
            set { this.progressbarHeight = value; }
        }

        [JsonProperty("taskbarTooltipTemplate")]
        [DefaultValue(null)]
        public string TaskbarTooltipTemplate
        {
            get { return this.taskbarTooltipTemplate; }
            set { this.taskbarTooltipTemplate = value; }
        }

        [JsonProperty("allowZooming")]
        [DefaultValue(false)]
        public bool AllowZooming
        {
            get { return this.allowZooming; }
            set { this.allowZooming = value; }
        }

        [JsonProperty("dateFormat")]
        [DefaultValue(null)]
        public string DateFormat
        {
            get { return this.dateFormat; }
            set { this.dateFormat = value; }
        }

        [JsonProperty("resourceIdMapping")]
        [DefaultValue(null)]
        public string ResourceIdMapping
        {
            get { return this.resourceIdMapping; }
            set { this.resourceIdMapping = value; }
        }

        [JsonProperty("resourceNameMapping")]
        [DefaultValue(null)]
        public string ResourceNameMapping
        {
            get { return this.resourceNameMapping; }
            set { this.resourceNameMapping = value; }
        }

        [JsonProperty("progressbarTooltipTemplateID")]
        [DefaultValue(null)]
        public string ProgressbarTooltipTemplateID
        {
            get { return this.progressbarTooltipTemplateID; }
            set { this.progressbarTooltipTemplateID = value; }
        }

        [JsonProperty("taskbarEditingTooltipTemplateID")]
        [DefaultValue(null)]
        public string TaskbarEditingTooltipTemplateID
        {
            get { return this.taskbarEditingTooltipTemplateID; }
            set { this.taskbarEditingTooltipTemplateID = value; }
        }

        [JsonProperty("selectedRowIndex")]
        [DefaultValue(-1)]
        public int SelectedRowIndex
        {
            get { return this.selectedRowIndex; }
            set { this.selectedRowIndex = value; }
        }

        [JsonProperty("queryCellInfo")]
        [DefaultValue(null)]
        public string QueryCellInfo
        {
            get { return this.queryCellInfo; }
            set { this.queryCellInfo = value; }
        }

        [JsonProperty("queryTaskbarInfo")]
        [DefaultValue(null)]
        public string QueryTaskbarInfo
        {
            get { return this.queryTaskbarInfo; }
            set { this.queryTaskbarInfo = value; }
        }

        [JsonProperty("allowGanttChartEditing")]
        [DefaultValue(true)]
        public bool AllowGanttChartEditing
        {
            get { return this.allowGanttChartEditing; }
            set { this.allowGanttChartEditing = value; }
        }

        [JsonProperty("selectedItem")]
        [DefaultValue(null)]
        public string SelectedItem
        {
            get { return this.selectedItem; }
            set { this.selectedItem = value; }
        }

        [JsonProperty("weekendBackground")]
        [DefaultValue("#F2F2F2")]
        public string WeekendBackground
        {
            get { return this.weekendBackground; }
            set { this.weekendBackground = value; }
        }

        [JsonProperty("baselineColor")]
        [DefaultValue("#fba41c")]
        public string BaselineColor
        {
            get { return this.baselineColor; }
            set { this.baselineColor = value; }
        }

        [JsonProperty("renderBaseline")]
        [DefaultValue(false)]
        public bool RenderBaseline
        {
            get { return this.renderBaseline; }
            set { this.renderBaseline = value; }
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

        [JsonProperty("rowSelecting")]
        [DefaultValue(null)]
        public string RowSelecting
        {
            get { return this.rowSelecting; }
            set { this.rowSelecting = value; }
        }

        [JsonProperty("rowSelected")]
        [DefaultValue(null)]
        public string RowSelected
        {
            get { return this.rowSelected; }
            set { this.rowSelected = value; }
        }

        [JsonProperty("actionBegin")]
        [DefaultValue(null)]
        public string ActionBegin
        {
            get { return this.actionBegin; }
            set { this.actionBegin = value; }
        }

        [JsonProperty("actionComplete")]
        [DefaultValue(null)]
        public string ActionComplete
        {
            get { return this.actionComplete; }
            set { this.actionComplete = value; }
        }

        [JsonProperty("beginEdit")]
        [DefaultValue(null)]
        public string BeginEdit
        {
            get { return this.beginEdit; }
            set { this.beginEdit = value; }
        }

        [JsonProperty("endEdit")]
        [DefaultValue(null)]
        public string EndEdit
        {
            get { return this.endEdit; }
            set { this.endEdit = value; }
        }

        [JsonProperty("expanding")]
        [DefaultValue(null)]
        public string Expanding
        {
            get { return this.expanding; }
            set { this.expanding = value; }
        }

        [JsonProperty("collapsing")]
        [DefaultValue(null)]
        public string Collapsing
        {
            get { return this.collapsing; }
            set { this.collapsing = value; }
        }

        [JsonProperty("expanded")]
        [DefaultValue(null)]
        public string Expanded
        {
            get { return this.expanded; }
            set { this.expanded = value; }
        }

        [JsonProperty("collapsed")]
        [DefaultValue(null)]
        public string Collapsed
        {
            get { return this.collapsed; }
            set { this.collapsed = value; }
        }

        [JsonProperty("refreshRow")]
        [DefaultValue(null)]
        public string RefreshRow
        {
            get { return this.refreshRow; }
            set { this.refreshRow = value; }
        }

        [JsonProperty("cancelEditCell")]
        [DefaultValue(null)]
        public string CancelEditCell
        {
            get { return this.cancelEditCell; }
            set { this.cancelEditCell = value; }
        }

        [JsonProperty("taskbarEditing")]
        [DefaultValue(null)]
        public string TaskbarEditing
        {
            get { return this.taskbarEditing; }
            set { this.taskbarEditing = value; }
        }

        [JsonProperty("rowHover")]
        [DefaultValue(null)]
        public string RowHover
        {
            get { return this.rowHover; }
            set { this.rowHover = value; }
        }

        [JsonProperty("taskbarEdited")]
        [DefaultValue(null)]
        public string TaskbarEdited
        {
            get { return this.taskbarEdited; }
            set { this.taskbarEdited = value; }
        }

        [JsonProperty("contextMenuOpen")]
        [DefaultValue(null)]
        public string ContextMenuOpen
        {
            get { return this.contextMenuOpen; }
            set { this.contextMenuOpen = value; }
        }

        #endregion

        #region ShouldSerialize Methods

        public bool ShouldSerializeHolidaysOption()
        {
            if (Utils.PropertyCompare(HolidaysOption, new HolidaysOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeToolBarOption()
        {
            if (Utils.PropertyCompare(ToolBarOption, new ToolBarOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeStripLinesOption()
        {
            if (Utils.PropertyCompare(StripLinesOption, new StripLinesOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeScheduleHeaderOption()
        {
            if (Utils.PropertyCompare(ScheduleHeaderOption, new ScheduleHeaderOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeEditOption()
        {
            if (Utils.PropertyCompare(EditOption, new GanttEditOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeSortSettingsOption()
        {
            if (Utils.PropertyCompare(SortSettingsOption, new SortSettingsOptions()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeSizeOption()
        {
            if (Utils.PropertyCompare(SizeOption, new SizeOptions()))
                return true;
            else
                return false;
        }

        #endregion
    }
}
