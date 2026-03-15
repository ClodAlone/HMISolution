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
    public class MobileFooterClientSideEventsBuilder
    {
        #region Fields

        private MobileFooterProperties mFooterModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileFooterClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mFooterModel">The m Footer model.</param>
        public MobileFooterClientSideEventsBuilder(MobileFooterProperties mFooterModel)
        {
            this.mFooterModel = mFooterModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Called when [left button click].
        /// </summary>
        /// <param name="onleftbuttonclick">The onleftbuttonclick.</param>
        /// <returns></returns>
        public MobileFooterClientSideEventsBuilder LeftButtonTap(string leftbuttonclick)
        {
            mFooterModel.LeftButtonTap = leftbuttonclick;
            return this;
        }

        /// <summary>
        /// Called when [right button click].
        /// </summary>
        /// <param name="onrightbuttonclick">The onrightbuttonclick.</param>
        /// <returns></returns>
        public MobileFooterClientSideEventsBuilder RightButtonTap(string rightbuttonclick)
        {
            mFooterModel.LeftButtonTap = rightbuttonclick;
            return this;
        }
        #endregion
    }
}
