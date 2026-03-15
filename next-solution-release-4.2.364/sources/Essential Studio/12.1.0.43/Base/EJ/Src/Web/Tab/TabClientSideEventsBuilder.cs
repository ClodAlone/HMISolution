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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;



namespace Syncfusion.JavaScript
{
    public class TabClientSideEventsBuilder
    {
        private TabProperties tabModel;
        public TabClientSideEventsBuilder(TabProperties tabProp)
        {
            tabModel = tabProp;
        }
        //Events
        public TabClientSideEventsBuilder Create(String create)
        {
            tabModel.Create = create;
            return this;
        }
        public TabClientSideEventsBuilder AjaxLoad(String ajaxLoad)
        {
            tabModel.AjaxLoad = ajaxLoad;
            return this;
        }
        public TabClientSideEventsBuilder AjaxBeforeLoad(String ajaxBeforeLoad)
        {
            tabModel.AjaxBeforeLoad = ajaxBeforeLoad;
            return this;
        }
        public TabClientSideEventsBuilder Active(String active)
        {
            tabModel.Active = active;
            return this;
        }
        public TabClientSideEventsBuilder BeforeActive(String beforeActive)
        {
            tabModel.BeforeActive = beforeActive;
            return this;
        }
        public TabClientSideEventsBuilder ItemAdd(String itemAdd)
        {
            tabModel.ItemAdd = itemAdd;
            return this;
        }
        public TabClientSideEventsBuilder ItemRemove(String itemRemove)
        {
            tabModel.ItemRemove = itemRemove;
            return this;
        }
        public TabClientSideEventsBuilder BeforeItemRemove(String beforeItemRemove)
        {
            tabModel.BeforeItemRemove = beforeItemRemove;
            return this;
        }
        public TabClientSideEventsBuilder ItemEnable(String itemEnable)
        {
            tabModel.ItemEnable = itemEnable;
            return this;
        }
        public TabClientSideEventsBuilder ItemDisable(String itemDisable)
        {
            tabModel.ItemDisable = itemDisable;
            return this;
        }
        public TabClientSideEventsBuilder Destroy(String destroy)
        {
            tabModel.Destroy = destroy;
            return this;
        }
       
    }
}
