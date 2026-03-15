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
    /// <summary>
    /// Class for MobileToggleButtonClientSideEventsBuilder
    /// </summary>
    public class MobileToggleButtonClientSideEventsBuilder
    {
        #region Fields
        private MobileToggleButtonProperties mobileToggleButtonModel;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MToggleButtonClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mToggleButtonModel">The m toggle button model.</param>
        public MobileToggleButtonClientSideEventsBuilder(MobileToggleButtonProperties mobileToggleButtonModel)
        {
            this.mobileToggleButtonModel = mobileToggleButtonModel;
        }
        #endregion

        #region Events
         /// <summary>
         /// Called when [touch start].
         /// </summary>
         /// <param name="touchStart">The touch start.</param>
         /// <returns></returns>
        public MobileToggleButtonClientSideEventsBuilder TouchStart(string touchStart)
         {
             mobileToggleButtonModel.TouchStart = touchStart;
             return this;
         }

         /// <summary>
         /// Called when [touch end].
         /// </summary>
         /// <param name="touchEnd">The touch end.</param>
         /// <returns></returns>
        public MobileToggleButtonClientSideEventsBuilder TouchEnd(string touchEnd)
         {
             mobileToggleButtonModel.TouchEnd = touchEnd;
             return this;
         }

         /// <summary>
         /// Called when [state change].
         /// </summary>
         /// <param name="onStateChange">The on state change.</param>
         /// <returns></returns>
        public MobileToggleButtonClientSideEventsBuilder StateChange(string stateChange)
         {
             mobileToggleButtonModel.StateChange = stateChange;
             return this;
         }
        #endregion
    }
}
