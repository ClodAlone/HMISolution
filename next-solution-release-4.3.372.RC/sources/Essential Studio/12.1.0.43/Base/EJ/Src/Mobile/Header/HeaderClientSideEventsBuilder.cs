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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileHeaderClientSideEventsBuilder
    {
        #region Fields

        private MobileHeaderProperties mHeaderModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileHeaderClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mHeaderModel">The m header model.</param>
        public MobileHeaderClientSideEventsBuilder(MobileHeaderProperties mHeaderModel)
        {
            this.mHeaderModel = mHeaderModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Called when [left button click].
        /// </summary>
        /// <param name="onleftbuttonclick">The onleftbuttonclick.</param>
        /// <returns></returns>
        public MobileHeaderClientSideEventsBuilder LeftButtonTap(string leftbuttonclick)
        {
            mHeaderModel.LeftButtonTap = leftbuttonclick;
            return this;
        }

        /// <summary>
        /// Called when [right button click].
        /// </summary>
        /// <param name="onrightbuttonclick">The onrightbuttonclick.</param>
        /// <returns></returns>
        public MobileHeaderClientSideEventsBuilder RightButtonTap(string rightbuttonclick)
        {
            mHeaderModel.RightButtonTap = rightbuttonclick;
            return this;
        }
        #endregion
    }
}
