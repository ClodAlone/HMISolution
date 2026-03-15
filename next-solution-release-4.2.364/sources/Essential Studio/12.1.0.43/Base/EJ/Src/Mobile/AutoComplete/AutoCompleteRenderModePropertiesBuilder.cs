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
    /// Class for Auto complete Windows Property Builder
    /// </summary>
    public class MobileAutoCompleteWindowsPropertiesBuilder
    {
        #region Fields

        private MobileAutoCompleteProperties MAutoCompleteModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAutoCompleteWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mAutoCompleteModel">The m automatic complete model.</param>
        public MobileAutoCompleteWindowsPropertiesBuilder(MobileAutoCompleteProperties mAutoCompleteModel)
        {
            this.MAutoCompleteModel = mAutoCompleteModel;
        }

        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        /// <returns></returns>
        public MobileAutoCompleteWindowsPropertiesBuilder RenderDefault(bool render)
        {
            this.MAutoCompleteModel.Windows.RenderDefault = render;
            return this;
        }
        #endregion
    }

}
