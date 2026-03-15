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
    /// <summary>
    /// Class for TimePicker Client side events
    /// </summary>
    public class MobileTimePickerClientSideEventsBuilder
    {
        #region Fields
        private MobileTimePickerProperties mTimePickerModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mTimePickerModel">The m time picker model.</param>
        public MobileTimePickerClientSideEventsBuilder(MobileTimePickerProperties mTimePickerModel)
        {
            this.mTimePickerModel = mTimePickerModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Called when [time selected].
        /// </summary>
        /// <param name="Select">The on time selected.</param>
        /// <returns></returns>
        public MobileTimePickerClientSideEventsBuilder Select(string select)
        {
            mTimePickerModel.Select = select;
            return this;
        }

        /// <summary>
        /// Called when [time picker load].
        /// </summary>
        /// <param name="Load">The on time picker load.</param>
        /// <returns></returns>
        public MobileTimePickerClientSideEventsBuilder Load(string load)
        {
            mTimePickerModel.Load = load;
            return this;
        }

        #endregion
    }
}
