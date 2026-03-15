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
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGridClientSideEventsBuilder
    {
        private OlapGridProperties olapGridModel;
        public OlapGridClientSideEventsBuilder(OlapGridProperties olapGridProp)
        {
            olapGridModel = olapGridProp;
        }
        public OlapGridClientSideEventsBuilder BeforeServiceInvoke(string beforeServiceInvoke)
        {
            olapGridModel.BeforeServiceInvoke = beforeServiceInvoke;
            return this;
        }
        public OlapGridClientSideEventsBuilder AfterServiceInvoke(string afterServiceInvoke)
        {
            olapGridModel.AfterServiceInvoke = afterServiceInvoke;
            return this;
        }
        public OlapGridClientSideEventsBuilder DrillSuccess(string drillSuccess)
        {
            olapGridModel.DrillSuccess = drillSuccess;
            return this;
        }
        public OlapGridClientSideEventsBuilder CellContextEvent(string cellContextEvent)
        {
            olapGridModel.CellContextEvent = cellContextEvent;
            return this;
        }
        public OlapGridClientSideEventsBuilder ValueCellHyperlinkClick(string valueCellHyperlinkClick)
        {
            olapGridModel.ValueCellHyperlinkClick = valueCellHyperlinkClick;
            return this;
        }
        public OlapGridClientSideEventsBuilder RowHeaderHyperlinkClick(string rowHeaderHyperlinkClick)
        {
            olapGridModel.RowHeaderHyperlinkClick = rowHeaderHyperlinkClick;
            return this;
        }
        public OlapGridClientSideEventsBuilder ColumnHeaderHyperlinkClick(string columnHeaderHyperlinkClick)
        {
            olapGridModel.ColumnHeaderHyperlinkClick = columnHeaderHyperlinkClick;
            return this;
        }
        public OlapGridClientSideEventsBuilder SummaryCellHyperlinkClick(string summaryCellHyperlinkClick)
        {
            olapGridModel.SummaryCellHyperlinkClick = summaryCellHyperlinkClick;
            return this;
        }
        public OlapGridClientSideEventsBuilder Destroy(string destroy)
        {
            olapGridModel.Destroy = destroy;
            return this;
        }
    }
}
