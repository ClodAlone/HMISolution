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
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// Class for Accordion Client Side EventBuilder
    /// </summary>
    public class MobileAccordionClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m accordion model
        /// </summary>
        private MobileAccordionProperties mAccordionModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mAccordionModel">The m accordion model.</param>
        public MobileAccordionClientSideEventsBuilder(MobileAccordionProperties mAccordionModel)
        {
            this.mAccordionModel = mAccordionModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Actives the specified active.
        /// </summary>
        /// <param name="Active">The active.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder Active(string Active)
        {
            mAccordionModel.Active = Active;
            return this;
        }

        /// <summary>
        /// Befores the active.
        /// </summary>
        /// <param name="beforeActive">The before active.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder BeforeActive(string beforeActive)
        {
            mAccordionModel.BeforeActive = beforeActive;
            return this;
        }

        /// <summary>
        /// Ajaxes the load.
        /// </summary>
        /// <param name="ajaxLoad">The ajax load.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder AjaxLoad(string ajaxLoad)
        {
            mAccordionModel.AjaxLoad = ajaxLoad;
            return this;
        }

        /// <summary>
        /// Ajaxes the before load.
        /// </summary>
        /// <param name="ajaxBeforeLoad">The ajax before load.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder AjaxBeforeLoad(string ajaxBeforeLoad)
        {
            mAccordionModel.AjaxBeforeLoad = ajaxBeforeLoad;
            return this;
        }

        /// <summary>
        /// Ajaxes the success.
        /// </summary>
        /// <param name="ajaxSuccess">The ajax success.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder AjaxSuccess(string ajaxSuccess)
        {
            mAccordionModel.AjaxSuccess = ajaxSuccess;
            return this;
        }

        /// <summary>
        /// Ajaxes the error.
        /// </summary>
        /// <param name="ajaxError">The ajax error.</param>
        /// <returns></returns>
        public MobileAccordionClientSideEventsBuilder AjaxError(string ajaxError)
        {
            mAccordionModel.AjaxError = ajaxError;
            return this;
        }
        #endregion
    }
}
