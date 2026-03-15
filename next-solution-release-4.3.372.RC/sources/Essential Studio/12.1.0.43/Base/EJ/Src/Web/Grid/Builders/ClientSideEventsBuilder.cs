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
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Models;
namespace Syncfusion.JavaScript
{
    public class ClientSideEventsBuilder<T> where T:class
    {
       
        private GridProperties<T> gridModel;
        public ClientSideEventsBuilder(GridProperties<T> gridProp)
        {
            gridModel = gridProp;
        }
        public ClientSideEventsBuilder<T> Load(String load)
        {
            gridModel.Load = load;
            return this;
        }
        public ClientSideEventsBuilder<T> Destroy(String destroy)
        {
            gridModel.Destroy = destroy;
            return this;
        }
        public ClientSideEventsBuilder<T> Create(String create)
        {
            gridModel.Create = create;
            return this;
        }
        public ClientSideEventsBuilder<T> ActionBegin(String actionbegin)
        {
            gridModel.ActionBegin = actionbegin;
            return this;
        }
        public ClientSideEventsBuilder<T> ActionComplete(String actioncomplete)
        {
            gridModel.ActionComplete = actioncomplete;
            return this;
        }
        public ClientSideEventsBuilder<T> BeginEdit(String beginedit)
        {
            gridModel.BeginEdit = beginedit;
            return this;
        }
        public ClientSideEventsBuilder<T> RowDataBound(String rowdatabound)
        {
            gridModel.RowDataBound = rowdatabound;
            return this;
        }
        public ClientSideEventsBuilder<T> RowSelected(String rowselected)
        {
            gridModel.RowSelected = rowselected;
            return this;
        }
        public ClientSideEventsBuilder<T> RowSelecting(String rowselecting)
        {
            gridModel.RowSelecting = rowselecting;
            return this;
        }
        public ClientSideEventsBuilder<T> Drag(String drag)
        {
            gridModel.Drag = drag;
            return this;
        }
        public ClientSideEventsBuilder<T> DragStop(String dragstop)
        {
            gridModel.DragStop = dragstop;
            return this;
        }
        public ClientSideEventsBuilder<T> DetailData(String detaildata)
        {
            gridModel.DetailData = detaildata;
            return this;
        }
        public ClientSideEventsBuilder<T> DetailExpand(String detailexpand)
        {
            gridModel.DetailExpand = detailexpand;
            return this;
        }
        public ClientSideEventsBuilder<T> DetailCollapse(String detailcollapse)
        {
            gridModel.DetailCollapse =detailcollapse ;
            return this;
        }
        public ClientSideEventsBuilder<T> DragStart(String dragstart)
        {
            gridModel.DragStart = dragstart;
            return this;
        }
        public ClientSideEventsBuilder<T> QueryCellInfo(String querycellinfo)
        {
            gridModel.QueryCellInfo = querycellinfo;
            return this;
        }
        public ClientSideEventsBuilder<T> CellSave(String cellsave)
        {
            gridModel.CellSave = cellsave;
            return this;
        }
        public ClientSideEventsBuilder<T> BeforeBulkDelete(String beforebulkdelete)
        {
            gridModel.BeforeBulkDelete = beforebulkdelete;
            return this;
        }
        public ClientSideEventsBuilder<T> BulkDelete(String bulkdelete)
        {
            gridModel.BulkDelete = bulkdelete;
            return this;
        }
        public ClientSideEventsBuilder<T> BeforeBulkAdd(String beforebulkadd)
        {
            gridModel.BeforeBulkAdd = beforebulkadd;
            return this;
        }
        public ClientSideEventsBuilder<T> BulkAdd(String bulkadd)
        {
            gridModel.BulkAdd = bulkadd;
            return this;
        }
        public ClientSideEventsBuilder<T> BeforeBulkSave(String beforebulksave)
        {
            gridModel.BeforeBulkSave = beforebulksave;
            return this;
        }
        public ClientSideEventsBuilder<T> CellEdit(String celledit)
        {
            gridModel.CellEdit = celledit;
            return this;
        }
        public ClientSideEventsBuilder<T> ToolBarClick(String toolbarclick)
        {
            gridModel.ToolBarClick = toolbarclick;
            return this;
        }
        public ClientSideEventsBuilder<T> RecordDoubleClick(String recordDoubleClick)
        {
            gridModel.RecordDoubleClick = recordDoubleClick;
            return this;
        }
        public ClientSideEventsBuilder<T> RecordClick(String recordClick)
        {
            gridModel.RecordClick = recordClick;
            return this;
        }
        public ClientSideEventsBuilder<T> GridRightClick(String gridRightClick)
        {
            gridModel.GridRightClick = gridRightClick;
            return this;
        }
        public ClientSideEventsBuilder<T> ResizeStart(String resizeStart)
        {
            gridModel.ResizeStart = resizeStart;
            return this;
        }
        public ClientSideEventsBuilder<T> ResizeEnd(String resizeEnd)
        {
            gridModel.ResizeEnd = resizeEnd;
            return this;
        }
        public ClientSideEventsBuilder<T> Resized(String resized)
        {
            gridModel.Resized = resized;
            return this;
        }
    }   
}
