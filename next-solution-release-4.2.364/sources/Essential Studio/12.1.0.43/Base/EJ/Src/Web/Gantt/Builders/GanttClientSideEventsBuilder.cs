#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class GanttClientSideEventsBuilder
    {
        private GanttProperties GanttModel;

        public GanttClientSideEventsBuilder(GanttProperties ganttProp)
        {
            GanttModel = ganttProp;
            
        }
        public GanttClientSideEventsBuilder Load(String load)
        {
            GanttModel.Load = load;
            return this;
        }
        public GanttClientSideEventsBuilder Destroy(String destroy)
        {
            GanttModel.Destroy = destroy;
            return this;
        }
        public GanttClientSideEventsBuilder Create(String create)
        {
            GanttModel.Create = create;
            return this;
        }
        public GanttClientSideEventsBuilder RowSelecting(String rowSelecting)
        {
            GanttModel.RowSelecting = rowSelecting;
            return this;
        }

        public GanttClientSideEventsBuilder RowSelected(String rowSelected)
        {
            GanttModel.RowSelected = rowSelected;
            return this;
        }

        public GanttClientSideEventsBuilder ActionBegin(String actionBegin)
        {
            GanttModel.ActionBegin = actionBegin;
            return this;
        }

        public GanttClientSideEventsBuilder ActionComplete(String actionComplete)
        {
            GanttModel.ActionComplete = actionComplete;
            return this;
        }

        public GanttClientSideEventsBuilder QueryCellInfo(String queryCellInfo)
        {
            GanttModel.QueryCellInfo = queryCellInfo;
            return this;
        }

        public GanttClientSideEventsBuilder RowDataBound(String rowDataBound)
        {
            GanttModel.RowDataBound = rowDataBound;
            return this;
        }

        public GanttClientSideEventsBuilder BeginEdit(String beginEdit)
        {
            GanttModel.BeginEdit = beginEdit;
            return this;
        }

        public GanttClientSideEventsBuilder EndEdit(String endEdit)
        {
            GanttModel.EndEdit = endEdit;
            return this;
        }

        public GanttClientSideEventsBuilder Expanding(String expanding)
        {
            GanttModel.Expanding = expanding;
            return this;
        }

        public GanttClientSideEventsBuilder Collapsing(String collapsing)
        {
            GanttModel.Collapsing = collapsing;
            return this;
        }

        public GanttClientSideEventsBuilder Expanded(String expanded)
        {
            GanttModel.Expanded = expanded;
            return this;
        }

        public GanttClientSideEventsBuilder Collapsed(String collapsed)
        {
            GanttModel.Collapsed = collapsed;
            return this;
        }

        public GanttClientSideEventsBuilder RefreshRow(String refreshRow)
        {
            GanttModel.RefreshRow = refreshRow;
            return this;
        }

        public GanttClientSideEventsBuilder CancelEditCell(String cancelEditCell)
        {
            GanttModel.CancelEditCell = cancelEditCell;
            return this;
        }

        public GanttClientSideEventsBuilder QueryTaskbarInfo(String queryTaskbarInfo)
        {
            GanttModel.QueryTaskbarInfo = queryTaskbarInfo;
            return this;
        }

        public GanttClientSideEventsBuilder TaskbarEditing(String taskbarEditing)
        {
            GanttModel.TaskbarEditing = taskbarEditing;
            return this;
        }

        public GanttClientSideEventsBuilder RowHover(String rowHover)
        {
            GanttModel.RowHover = rowHover;
            return this;
        }

        public GanttClientSideEventsBuilder TaskbarEdited(String taskbarEdited)
        {
            GanttModel.TaskbarEdited = taskbarEdited;
            return this;
        }

        public GanttClientSideEventsBuilder ContextMenuOpen(String contextMenuOpen)
        {
            GanttModel.ContextMenuOpen = contextMenuOpen;
            return this;
        }
    }
}