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
    /// Class for MobileButtonClientSideEventsBuilder
    /// </summary>
    public class MobileButtonClientSideEventsBuilder
    {
        #region Fields
        /// <summary>
        /// The m button model
        /// </summary>
        private MobileButtonProperties mButtonModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MButtonClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="MButtonModel">The m tab model.</param>
        public MobileButtonClientSideEventsBuilder(MobileButtonProperties mButtonModel)
        {
            this.mButtonModel = mButtonModel;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="touchStart">The touch start.</param>
        /// <returns></returns>
        public MobileButtonClientSideEventsBuilder TouchStart(string touchStart)
        {
            mButtonModel.TouchStart = touchStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="touchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileButtonClientSideEventsBuilder TouchEnd(string touchEnd)
        {
            mButtonModel.TouchEnd = touchEnd;
            return this;
        }

        #endregion        
    }
}
