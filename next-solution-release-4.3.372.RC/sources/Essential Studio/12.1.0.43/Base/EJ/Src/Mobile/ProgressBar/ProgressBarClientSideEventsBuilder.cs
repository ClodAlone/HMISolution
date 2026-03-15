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
    /// Class for ProgrssBar clientSide Events
    /// </summary>
    public class MobileProgressBarClientSideEventsBuilder
    {
        #region Fields

        private MobileProgressBarProperties mProgressBarModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileProgressBarClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mProgressBarModel">The m progress bar model.</param>
        public MobileProgressBarClientSideEventsBuilder(MobileProgressBarProperties mProgressBarModel)
        {
            this.mProgressBarModel = mProgressBarModel;
        }
        #endregion

        #region Events

        /// <summary>
        /// Creates the specified create.
        /// </summary>
        /// <param name="create">The create.</param>
        /// <returns></returns>
        public MobileProgressBarClientSideEventsBuilder Create(string create)
        {
            mProgressBarModel.Create = create;
            return this;
        }


        /// <summary>
        /// Changes the specified change.
        /// </summary>
        /// <param name="change">The change.</param>
        /// <returns></returns>
        public MobileProgressBarClientSideEventsBuilder Change(string change)
        {
            mProgressBarModel.Change = change;
            return this;
        }


        /// <summary>
        /// Completes the specified complete.
        /// </summary>
        /// <param name="complete">The complete.</param>
        /// <returns></returns>
        public MobileProgressBarClientSideEventsBuilder Complete(string complete)
        {
            mProgressBarModel.Complete = complete;
            return this;
        }
        #endregion
    }
}
