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
    public class MobileGroupButtonClientSideEventsBuilder
    {
        #region Fields

        /// <summary>
        /// The mobile group button model
        /// </summary>
        private MobileGroupButtonProperties mobileGroupButtonModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileGroupButtonClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mobileGroupButtonModel">The mobile group button model.</param>
        public MobileGroupButtonClientSideEventsBuilder(MobileGroupButtonProperties mobileGroupButtonModel)
        {
            this.mobileGroupButtonModel = mobileGroupButtonModel;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="touchStart">The on touch start.</param>
        /// <returns></returns>
        public MobileGroupButtonClientSideEventsBuilder touchStart(string touchStart)
        {
            mobileGroupButtonModel.touchStart = touchStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="touchEnd">The on touch end.</param>
        /// <returns></returns>
        public MobileGroupButtonClientSideEventsBuilder touchEnd(string touchEnd)
        {
            mobileGroupButtonModel.touchEnd = touchEnd;
            return this;
        }

        #endregion        
    }
}
