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
    /// Class for Date picker client side events
    /// </summary>
    public class MobileDatePickerClientSideEventsBuilder
    {
        #region Fields
        private MobileDatePickerProperties mDatePickerModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mDatePickerModel">The m date picker model.</param>
        public MobileDatePickerClientSideEventsBuilder(MobileDatePickerProperties mDatePickerModel)
        {
            this.mDatePickerModel = mDatePickerModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Called when [date selected].
        /// </summary>
        /// <param name="Select">The on date selected.</param>
        /// <returns></returns>
        public MobileDatePickerClientSideEventsBuilder Select(string select)
        {
            mDatePickerModel.Select=select;
            return this;
        }

        /// <summary>
        /// Called when [date picker load].
        /// </summary>
        /// <param name="Load">The on date picker load.</param>
        /// <returns></returns>
        public MobileDatePickerClientSideEventsBuilder Load(string load)
        {
            mDatePickerModel.Load = load;
            return this;
        }

        #endregion
    }
}
