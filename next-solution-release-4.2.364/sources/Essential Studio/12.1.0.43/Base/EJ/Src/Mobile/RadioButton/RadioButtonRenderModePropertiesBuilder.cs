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
    /// <summary>
    /// Class for MobileRadioButtonWindowsPropertiesBuilder
    /// </summary>
    public class MobileRadioButtonWindowsPropertiesBuilder
    {
        #region Fields
        private MobileRadioButtonProperties MobileRadioButtonModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Ms the tab windows properties builder.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileRadioButtonWindowsPropertiesBuilder(MobileRadioButtonProperties mRadioButtonModel)
        {
            this.MobileRadioButtonModel = mRadioButtonModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileRadioButtonWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileRadioButtonModel.Windows.RenderDefault = renderDefault;
            return this;
        }
        #endregion

    }
}
