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
    /// Class for MobileRadioButtonClientSideEventsBuilder
    /// </summary>
    public class MobileRadioButtonClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m RadioButton model
        /// </summary>
        private MobileRadioButtonProperties mRadioButtonModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MRadioButtonClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mRadioButtonModel">The m RadioButton model.</param>
        public MobileRadioButtonClientSideEventsBuilder(MobileRadioButtonProperties mRadioButtonModel)
        {
            this.mRadioButtonModel = mRadioButtonModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="touchStart">The touch start.</param>
        /// <returns></returns>
        public MobileRadioButtonClientSideEventsBuilder touchStart(string touchStart)
        {
            mRadioButtonModel.TouchStart = touchStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="touchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileRadioButtonClientSideEventsBuilder touchEnd(string touchEnd)
        {
            mRadioButtonModel.TouchEnd = touchEnd;
            return this;
        }
        #endregion
    }
}
