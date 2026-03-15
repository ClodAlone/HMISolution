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
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileAccordionWindowsPropertiesBuilder
    {
        #region Fields
        /// <summary>
        /// Gets or sets the mobile accordion model.
        /// </summary>
        /// <value>
        /// The mobile accordion model.
        /// </value>
        private MobileAccordionProperties MobileAccordionModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mAccordionModel">The m accordion model.</param>
        public MobileAccordionWindowsPropertiesBuilder(MobileAccordionProperties mAccordionModel)
        {
            this.MobileAccordionModel = mAccordionModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileAccordionWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileAccordionModel.Windows.RenderDefault = renderDefault;
            return this;
        }
        #endregion

    }
}
