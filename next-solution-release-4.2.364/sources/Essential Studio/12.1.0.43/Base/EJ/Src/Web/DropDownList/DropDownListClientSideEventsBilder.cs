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
    public class DropDownListClientSideEventsBuilder
    {
        private DropDownListProperties dropDownListModel;
        public DropDownListClientSideEventsBuilder(DropDownListProperties dropDownListProp)
        {
            dropDownListModel = dropDownListProp;
        }
        //Events
        public DropDownListClientSideEventsBuilder Create(String create)
        {
            dropDownListModel.Create = create;
            return this;
        }
        public DropDownListClientSideEventsBuilder PopupHide(String popupHide)
        {
            dropDownListModel.PopupHide = popupHide;
            return this;
        }
        public DropDownListClientSideEventsBuilder PopupShown(String popupShown)
        {
            dropDownListModel.PopupShown = popupShown;
            return this;
        }
        public DropDownListClientSideEventsBuilder BeforePopupShown(String beforePopupShown)
        {
            dropDownListModel.BeforePopupShown = beforePopupShown;
            return this;
        }
        public DropDownListClientSideEventsBuilder Change(String change)
        {
            dropDownListModel.Change = change;
            return this;
        }
        public DropDownListClientSideEventsBuilder Select(String select)
        {
            dropDownListModel.Select = select;
            return this;
        }
        public DropDownListClientSideEventsBuilder CheckChange(String checkChange)
        {
            dropDownListModel.CheckChange = checkChange;
            return this;
        }
        public DropDownListClientSideEventsBuilder Destroy(String destroy)
        {
            dropDownListModel.Destroy = destroy;
            return this;
        }
    }
}
