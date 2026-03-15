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
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{

    public class GanttPropertiesBuilder 
    {
        public Gantt gantt;

        public GanttPropertiesBuilder(Gantt gantt)
        {
            this.gantt = new Gantt(gantt.ID, gantt.GanttModel);
        }

        public GanttPropertiesBuilder()
        {
        }
        public GanttPropertiesBuilder EnableContextMenu()
        {
            gantt.GanttModel.EnableContextMenu = true;
            return this;
        }

        public GanttPropertiesBuilder EnableContextMenu(bool enableContextMenu)
        {
            gantt.GanttModel.EnableContextMenu = enableContextMenu;
            return this;
        }
        public GanttPropertiesBuilder AllowSorting()
        {
            gantt.GanttModel.AllowSorting = false;
            return this;
        }

        public GanttPropertiesBuilder AllowSorting(bool allowSorting)
        {
            gantt.GanttModel.AllowSorting = allowSorting;
            return this;
        }

        public GanttPropertiesBuilder AllowColumnResize()
        {
            gantt.GanttModel.AllowColumnResize = false;
            return this;
        }

        public GanttPropertiesBuilder AllowColumnResize(bool allowColumnResize)
        {
            gantt.GanttModel.AllowColumnResize = allowColumnResize;
            return this;
        }

        public GanttPropertiesBuilder AllowSelection()
        {
            gantt.GanttModel.AllowSelection = false;
            return this;
        }

        public GanttPropertiesBuilder AllowSelection(bool allowSelection)
        {
            gantt.GanttModel.AllowSelection = allowSelection;
            return this;
        }


        public GanttPropertiesBuilder EnableRowHover()
        {
            gantt.GanttModel.EnableRowHover = true;
            return this;
        }

        public GanttPropertiesBuilder EnableRowHover(bool enableRowHover)
        {
            gantt.GanttModel.EnableRowHover = enableRowHover;
            return this;
        }

        public GanttPropertiesBuilder EnableVirtualization()
        {
            gantt.GanttModel.EnableVirtualization = true;
            return this;
        }

        public GanttPropertiesBuilder EnableVirtualization(bool enableRowHover)
        {
            gantt.GanttModel.EnableVirtualization = enableRowHover;
            return this;
        }
        public GanttPropertiesBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            gantt.GanttModel.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder Datasource(DataSource dataSource)
        {
            gantt.GanttModel.DataSource = dataSource;
            return this;
        }
        public GanttPropertiesBuilder Datasource(object dataSource)
        {
            gantt.GanttModel.DataSource = dataSource;
            return this;
        }

        public GanttPropertiesBuilder Datasource(String dataURL)
        {
            gantt.GanttModel.DataSource = dataURL;
            return this;
        }

        public GanttPropertiesBuilder Datasource(IEnumerable dataSource)
        {
            gantt.GanttModel.DataSource = dataSource;
            return this;
        }
        

        public GanttPropertiesBuilder TaskIdMapping(String taskIdMapping)
        {
            gantt.GanttModel.TaskIdMapping = taskIdMapping;
            return this;
        }

        public GanttPropertiesBuilder TaskNameMapping(String taskNameMapping)
        {
            gantt.GanttModel.TaskNameMapping = taskNameMapping;
            return this;
        }

        public GanttPropertiesBuilder StartDateMapping(String startDateMapping)
        {
            gantt.GanttModel.StartDateMapping = startDateMapping;
            return this;
        }

        public GanttPropertiesBuilder EndDateMapping(String endDateMapping)
        {
            gantt.GanttModel.EndDateMapping = endDateMapping;
            return this;
        }

        public GanttPropertiesBuilder BaselineStartDateMapping(String baselineStartDateMapping)
        {
            gantt.GanttModel.BaselineStartDateMapping = baselineStartDateMapping;
            return this;
        }

        public GanttPropertiesBuilder BaselineEndDateMapping(String baselineEndDateMapping)
        {
            gantt.GanttModel.BaselineEndDateMapping = baselineEndDateMapping;
            return this;
        }

        public GanttPropertiesBuilder ChildMapping(String childMapping)
        {
            gantt.GanttModel.ChildMapping = childMapping;
            return this;
        }

        public GanttPropertiesBuilder DurationMapping(String durationMapping)
        {
            gantt.GanttModel.DurationMapping = durationMapping;
            return this;
        }

        public GanttPropertiesBuilder MileStoneMapping(String mileStoneMapping)
        {
            gantt.GanttModel.MileStoneMapping = mileStoneMapping;
            return this;
        }

        public GanttPropertiesBuilder ProgressMapping(String progressMapping)
        {
            gantt.GanttModel.ProgressMapping = progressMapping;
            return this;
        }

        public GanttPropertiesBuilder PredecessorMapping(String predecessorMapping)
        {
            gantt.GanttModel.PredecessorMapping = predecessorMapping;
            return this;
        }

        public GanttPropertiesBuilder ResourceInfoMapping(String resourceInfoMapping)
        {
            gantt.GanttModel.ResourceInfoMapping = resourceInfoMapping;
            return this;
        }

        public GanttPropertiesBuilder ResourceCollection(object resourceCollection)
        {
            gantt.GanttModel.ResourceCollection = resourceCollection;
            return this;
        }
        public GanttPropertiesBuilder ResourceCollection(IEnumerable resourceCollection)
        {
            gantt.GanttModel.ResourceCollection = resourceCollection;
            return this;
        }

        public GanttPropertiesBuilder HighlightWeekEnds()
        {
            gantt.GanttModel.HighlightWeekEnds = true;
            return this;
        }

        public GanttPropertiesBuilder HighlightWeekEnds(bool highlightWeekEnds)
        {
            gantt.GanttModel.HighlightWeekEnds = highlightWeekEnds;
            return this;
        }

        public GanttPropertiesBuilder CanResizeProgressBar()
        {
            gantt.GanttModel.CanResizeProgressBar = true;
            return this;
        }

        public GanttPropertiesBuilder CanResizeProgressBar(bool canResizeProgressBar)
        {
            gantt.GanttModel.CanResizeProgressBar = canResizeProgressBar;
            return this;
        }
        
        public GanttPropertiesBuilder ScheduleStartDate(object scheduleStartDate)
        {
            gantt.GanttModel.ScheduleStartDate = scheduleStartDate;
            return this;
        }

        public GanttPropertiesBuilder ScheduleEndDate(object scheduleEndDate)
        {
            gantt.GanttModel.ScheduleEndDate = scheduleEndDate;
            return this;
        }
        
        public GanttPropertiesBuilder RowHeight(int rowHeight)
        {
            gantt.GanttModel.RowHeight = rowHeight;
            return this;
        }

        public GanttPropertiesBuilder IncludeWeekend()
        {
            gantt.GanttModel.IncludeWeekend = true;
            return this;
        }

        public GanttPropertiesBuilder IncludeWeekend(bool includeWeekend)
        {
            gantt.GanttModel.IncludeWeekend = includeWeekend;
            return this;
        }

        public GanttPropertiesBuilder TaskbarBackground(string taskbarBackground)
        {
            gantt.GanttModel.TaskbarBackground = taskbarBackground;
            return this;
        }

        public GanttPropertiesBuilder ProgressbarBackground(string progressbarBackground)
        {
            gantt.GanttModel.ProgressbarBackground = progressbarBackground;
            return this;
        }

        public GanttPropertiesBuilder ConnectorLineBackground(string connectorLineBackground)
        {
            gantt.GanttModel.ConnectorLineBackground = connectorLineBackground;
            return this;
        }

        public GanttPropertiesBuilder ParentTaskbarBackground(string parentTaskbarBackground)
        {
            gantt.GanttModel.ParentTaskbarBackground = parentTaskbarBackground;
            return this;
        }

        public GanttPropertiesBuilder ParentProgressbarBackground(string parentProgressbarBackground)
        {
            gantt.GanttModel.ParentProgressbarBackground = parentProgressbarBackground;
            return this;
        }

        public GanttPropertiesBuilder ConnectorlineWidth(int connectorlineWidth)
        {
            gantt.GanttModel.ConnectorlineWidth = connectorlineWidth;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskNames()
        {
            gantt.GanttModel.ShowTaskNames = true;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskNames(bool showTaskNames)
        {
            gantt.GanttModel.ShowTaskNames = showTaskNames;
            return this;
        }

        public GanttPropertiesBuilder ShowProgressStatus()
        {
            gantt.GanttModel.ShowProgressStatus = true;
            return this;
        }

        public GanttPropertiesBuilder ShowProgressStatus(bool showProgressStatus)
        {
            gantt.GanttModel.ShowProgressStatus = showProgressStatus;
            return this;
        }

        public GanttPropertiesBuilder ShowResourceNames()
        {
            gantt.GanttModel.ShowResourceNames = true;
            return this;
        }

        public GanttPropertiesBuilder ShowResourceNames(bool showResourceNames)
        {
            gantt.GanttModel.ShowResourceNames = showResourceNames;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskbarDragTooltip()
        {
            gantt.GanttModel.ShowTaskbarDragTooltip = true;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskbarDragTooltip(bool showTaskbarDragTooltip)
        {
            gantt.GanttModel.ShowTaskbarDragTooltip = showTaskbarDragTooltip;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskbarTooltip()
        {
            gantt.GanttModel.ShowTaskbarTooltip = true;
            return this;
        }

        public GanttPropertiesBuilder ShowTaskbarTooltip(bool showTaskbarTooltip)
        {
            gantt.GanttModel.ShowTaskbarTooltip = showTaskbarTooltip;
            return this;
        }

        public GanttPropertiesBuilder ToolBar(Action<ToolBarOptionsBuilder> toolBarOption)
        {
            var obj = new ToolBarOptions();
            gantt.GanttModel.ToolBarOption = obj;
            var builder = new ToolBarOptionsBuilder(obj);
            if (toolBarOption != null)
                toolBarOption.Invoke(builder);
            return this;
            //this.gantt.GanttModel.ToolBarOption=new ToolBarOptions();
            //var builder = new ToolBarOptionsBuilder(this.gantt.GanttModel.ToolBarOption);
            //if (toolBarOption != null)
            //    toolBarOption.Invoke(builder);
            //return this;
        }

        public GanttPropertiesBuilder ToolBarOption(ToolBarOptions toolBarOption)
        {
            gantt.GanttModel.ToolBarOption = toolBarOption;
            return this;
        }

        public GanttPropertiesBuilder StripLinesOption(Action<StripLinesOptionsBuilder> stripLinesOption)
        {
            var builder = new StripLinesOptionsBuilder(this.gantt.GanttModel.StripLinesOption);
            if (stripLinesOption != null)
                stripLinesOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder StripLinesOption(StripLinesOptions stripLinesOption)
        {
            gantt.GanttModel.StripLinesOption = stripLinesOption;
            return this;
        }

        public GanttPropertiesBuilder ScheduleHeaderOption(
            Action<ScheduleHeaderOptionsBuilder> scheduleHeaderOption)
        {
            var builder = new ScheduleHeaderOptionsBuilder(this.gantt.GanttModel.ScheduleHeaderOption);
            if (scheduleHeaderOption != null)
                scheduleHeaderOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder ScheduleHeaderOption(ScheduleHeaderOptions scheduleHeaderOption)
        {
            gantt.GanttModel.ScheduleHeaderOption = scheduleHeaderOption;
            return this;
        }

        public GanttPropertiesBuilder EditOption(Action<GanttEditOptionsBuilder> editOption)
        {
            var builder = new GanttEditOptionsBuilder(this.gantt.GanttModel.EditOption);
            if (editOption != null)
                editOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder EditOption(GanttEditOptions editOption)
        {
            gantt.GanttModel.EditOption = editOption;
            return this;
        }

        public GanttPropertiesBuilder SortSettingsOption(Action<SortSettingsOptionsBuilder> sortSettingsOption)
        {
            var builder = new SortSettingsOptionsBuilder(this.gantt.GanttModel.SortSettingsOption);
            if (sortSettingsOption != null)
                sortSettingsOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder SortSettingsOption(SortSettingsOptions sortSettingsOption)
        {
            gantt.GanttModel.SortSettingsOption = sortSettingsOption;
            return this;
        }

        public GanttPropertiesBuilder HolidaysOption(Action<HolidaysOptionsBuilder> holidaysOption)
        {
            var builder = new HolidaysOptionsBuilder(this.gantt.GanttModel.HolidaysOption);
            if (holidaysOption != null)
                holidaysOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder HolidaysOption(HolidaysOptions holidaysOption)
        {
            gantt.GanttModel.HolidaysOption = holidaysOption;
            return this;
        }

        public GanttPropertiesBuilder SizeOption(Action<SizeOptionsBuilder> sizeOption)
        {
            var builder = new SizeOptionsBuilder(this.gantt.GanttModel.SizeOption);
            if (sizeOption != null)
                sizeOption.Invoke(builder);
            return this;
        }

        public GanttPropertiesBuilder SizeOption(SizeOptions sizeOption)
        {
            gantt.GanttModel.SizeOption = sizeOption;
            return this;
        }

        public GanttPropertiesBuilder ExpanderImagePath(string expanderImagePath)
        {
            gantt.GanttModel.ExpanderImagePath = expanderImagePath;
            return this;
        }

        public GanttPropertiesBuilder CollapsedIamgePath(string collapsedIamgePath)
        {
            gantt.GanttModel.CollapsedIamgePath = collapsedIamgePath;
            return this;
        }


        public GanttPropertiesBuilder AllowKeyboardNavigation()
        {
            gantt.GanttModel.AllowKeyboardNavigation = true;
            return this;
        }

        public GanttPropertiesBuilder AllowKeyboardNavigation(bool allowKeyboardNavigation)
        {
            gantt.GanttModel.AllowKeyboardNavigation = allowKeyboardNavigation;
            return this;
        }

        public GanttPropertiesBuilder CssClass(string cssClass)
        {
            gantt.GanttModel.CssClass = cssClass;
            return this;
        }

        public GanttPropertiesBuilder Localization(string localization)
        {
            gantt.GanttModel.Localization = localization;
            return this;
        }

        public GanttPropertiesBuilder AutoSaveOnRowSelection()
        {
            gantt.GanttModel.AutoSaveOnRowSelection = true;
            return this;
        }

        public GanttPropertiesBuilder AutoSaveOnRowSelection(bool autoSaveOnRowSelection)
        {
            gantt.GanttModel.AutoSaveOnRowSelection = autoSaveOnRowSelection;
            return this;
        }

        public GanttPropertiesBuilder AllowMultiSorting()
        {
            gantt.GanttModel.AllowMultiSorting = false;
            return this;
        }

        public GanttPropertiesBuilder AllowMultiSorting(bool allowMultiSorting)
        {
            gantt.GanttModel.AllowMultiSorting = allowMultiSorting;
            return this;
        }

        public GanttPropertiesBuilder RowTemplate(string rowTemplate)
        {
            gantt.GanttModel.RowTemplate = rowTemplate;
            return this;
        }

        public GanttPropertiesBuilder RowDataBound(string rowDataBound)
        {
            gantt.GanttModel.RowDataBound = rowDataBound;
            return this;
        }

        public GanttPropertiesBuilder ProgressbarHeight(int progressbarHeight)
        {
            gantt.GanttModel.ProgressbarHeight = progressbarHeight;
            return this;
        }

        public GanttPropertiesBuilder TaskbarTooltipTemplate(string taskbarTooltipTemplate)
        {
            gantt.GanttModel.TaskbarTooltipTemplate = taskbarTooltipTemplate;
            return this;
        }

        public GanttPropertiesBuilder AllowZooming()
        {
            gantt.GanttModel.AllowZooming = false;
            return this;
        }

        public GanttPropertiesBuilder AllowZooming(bool allowZooming)
        {
            gantt.GanttModel.AllowZooming = allowZooming;
            return this;
        }

        public GanttPropertiesBuilder DateFormat(string dateFormat)
        {
            gantt.GanttModel.DateFormat = dateFormat;
            return this;
        }

        public GanttPropertiesBuilder ResourceIdMapping(string resourceIdMapping)
        {
            gantt.GanttModel.ResourceIdMapping = resourceIdMapping;
            return this;
        }

        public GanttPropertiesBuilder ResourceNameMapping(string resourceNameMapping)
        {
            gantt.GanttModel.ResourceNameMapping = resourceNameMapping;
            return this;
        }

        public GanttPropertiesBuilder ProgressbarTooltipTemplateID(string progressbarTooltipTemplateID)
        {
            gantt.GanttModel.ProgressbarTooltipTemplateID = progressbarTooltipTemplateID;
            return this;
        }

        public GanttPropertiesBuilder TaskbarEditingTooltipTemplateID(string taskbarEditingTooltipTemplateID)
        {
            gantt.GanttModel.TaskbarEditingTooltipTemplateID = taskbarEditingTooltipTemplateID;
            return this;
        }

        public GanttPropertiesBuilder SelectedRowIndex(int selectedRowIndex)
        {
            gantt.GanttModel.SelectedRowIndex = selectedRowIndex;
            return this;
        }

        public GanttPropertiesBuilder QueryCellInfo(string queryCellInfo)
        {
            gantt.GanttModel.QueryCellInfo = queryCellInfo;
            return this;
        }

        public GanttPropertiesBuilder QueryTaskbarInfo(string queryTaskbarInfo)
        {
            gantt.GanttModel.QueryTaskbarInfo = queryTaskbarInfo;
            return this;
        }

        public GanttPropertiesBuilder SelectedItem(string selectedItem)
        {
            gantt.GanttModel.SelectedItem = selectedItem;
            return this;
        }

        public GanttPropertiesBuilder AllowGanttChartEditing()
        {
            gantt.GanttModel.AllowGanttChartEditing = true;
            return this;
        }

        public GanttPropertiesBuilder AllowGanttChartEditing(bool allowGanttChartEditing)
        {
            gantt.GanttModel.AllowGanttChartEditing = allowGanttChartEditing;
            return this;
        }

        public GanttPropertiesBuilder WeekendBackground(string weekendBackground)
        {
            gantt.GanttModel.WeekendBackground = weekendBackground;
            return this;
        }

        public GanttPropertiesBuilder BaselineColor(string baselineColor)
        {
            gantt.GanttModel.BaselineColor = baselineColor;
            return this;
        }

        public GanttPropertiesBuilder RenderBaseline()
        {
            gantt.GanttModel.RenderBaseline = false;
            return this;
        }

        public GanttPropertiesBuilder RenderBaseline(bool renderBaseline)
        {
            gantt.GanttModel.RenderBaseline = renderBaseline;
            return this;
        }

        public GanttPropertiesBuilder ClientSideEvents(Action<GanttClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new GanttClientSideEventsBuilder(this.gantt.GanttModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
       
        public HtmlString Render()
        {
            return new HtmlString(gantt.Render().ToString());
        }

        public override String ToString()
        {
            return Render().ToString();
        }
    }

}