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
    public class MobileRotatorClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m Rotator model
        /// </summary>
        private MobileRotatorProperties mobileRotatorModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileRotatorClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mobileRotatorModel">The m Rotator model.</param>
        public MobileRotatorClientSideEventsBuilder(MobileRotatorProperties mobileRotatorModel)
        {
            this.mobileRotatorModel = mobileRotatorModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when [swipeLeft].
        /// </summary>
        /// <param name="swipeLeft">The swipeLeft.</param>
        /// <returns></returns>
        public MobileRotatorClientSideEventsBuilder SwipeLeft(string swipeLeft)
        {
            mobileRotatorModel.SwipeLeft = swipeLeft;
            return this;
        }

        /// <summary>
        /// Called when [swipeRight].
        /// </summary>
        /// <param name="swipeRight">The swipeRight.</param>
        /// <returns></returns>
        public MobileRotatorClientSideEventsBuilder SwipeRight(string swipeRight)
        {
            mobileRotatorModel.SwipeRight = swipeRight;
            return this;
        }        
        #endregion
    }
}
