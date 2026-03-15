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
    public class AccordionClientSideEventsBuilder
    {
        private AccordionProperties accordionModel;
        public AccordionClientSideEventsBuilder(AccordionProperties accordionProp)
        {
            accordionModel = accordionProp;
        }
        //Events
        public AccordionClientSideEventsBuilder Create(String create)
        {
            accordionModel.Create = create;
            return this;
        }
        public AccordionClientSideEventsBuilder AjaxLoad(String ajaxLoad)
        {
            accordionModel.AjaxLoad = ajaxLoad;
            return this;
        }
        public AccordionClientSideEventsBuilder AjaxBeforeLoad(String ajaxBeforeLoad)
        {
            accordionModel.AjaxBeforeLoad = ajaxBeforeLoad;
            return this;
        }
        public AccordionClientSideEventsBuilder Active(String active)
        {
            accordionModel.Active = active;
            return this;
        }
        public AccordionClientSideEventsBuilder BeforeActive(String beforeActive)
        {
            accordionModel.BeforeActive = beforeActive;
            return this;
        }
        public AccordionClientSideEventsBuilder AjaxSuccess(String ajaxSuccess)
        {
            accordionModel.AjaxSuccess = ajaxSuccess;
            return this;
        }
        public AccordionClientSideEventsBuilder AjaxError(String ajaxError)
        {
            accordionModel.AjaxError = ajaxError;
            return this;
        }
        public AccordionClientSideEventsBuilder Destroy(String destroy)
        {
            accordionModel.Destroy = destroy;
            return this;
        }

    }
}
