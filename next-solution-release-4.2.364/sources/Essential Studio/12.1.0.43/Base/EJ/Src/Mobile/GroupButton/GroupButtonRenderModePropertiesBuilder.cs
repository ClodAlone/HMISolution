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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{

    public class MobileGroupButtonWindowsPropertiesBuilder
    {
        #region Fields

        private MobileGroupButtonProperties MobileGroupButtonModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButtonWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mobileGroupButtonModel">The mobile group button model.</param>
        public MobileGroupButtonWindowsPropertiesBuilder(MobileGroupButtonProperties mobileGroupButtonModel)
        {
            this.MobileGroupButtonModel = mobileGroupButtonModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileGroupButtonWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileGroupButtonModel.Windows.RenderDefault = renderDefault;
            return this;
        }

        #endregion
    }
}
