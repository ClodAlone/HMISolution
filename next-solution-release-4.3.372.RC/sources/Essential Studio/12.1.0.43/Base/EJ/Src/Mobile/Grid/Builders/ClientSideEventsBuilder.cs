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
using Syncfusion.JavaScript.Mobile.Models;
namespace Syncfusion.JavaScript
{
    public class MobileClientSideEventsBuilder<T> where T:class
    {
        private MobileGridProperties<T> gridModel;
        public MobileClientSideEventsBuilder(MobileGridProperties<T> gridProp)
        {
            gridModel = gridProp;
        }
        public MobileClientSideEventsBuilder<T> Load(String load)
        {
            gridModel.Load = load;
            return this;
        }
        public MobileClientSideEventsBuilder<T> Destroy(String destroy)
        {
            gridModel.Destroy = destroy;
            return this;
        }
        public MobileClientSideEventsBuilder<T> Create(String create)
        {
            gridModel.Create = create;
            return this;
        }
        public MobileClientSideEventsBuilder<T> ActionBegin(String actionbegin)
        {
            gridModel.ActionBegin = actionbegin;
            return this;
        }
        public MobileClientSideEventsBuilder<T> ActionComplete(String actioncomplete)
        {
            gridModel.ActionComplete = actioncomplete;
            return this;
        }
        public MobileClientSideEventsBuilder<T> RowDataBound(String rowdatabound)
        {
            gridModel.RowDataBound = rowdatabound;
            return this;
        }
        public MobileClientSideEventsBuilder<T> RowSelected(String rowselected)
        {
            gridModel.RowSelected = rowselected;
            return this;
        }
        public MobileClientSideEventsBuilder<T> RowSelecting(String rowselecting)
        {
            gridModel.RowSelecting = rowselecting;
            return this;
        }
        public MobileClientSideEventsBuilder<T> QueryCellInfo(String querycellinfo)
        {
            gridModel.QueryCellInfo = querycellinfo;
            return this;
        }
       
    }
}
