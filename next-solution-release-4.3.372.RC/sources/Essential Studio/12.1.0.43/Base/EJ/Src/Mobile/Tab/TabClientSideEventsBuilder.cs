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
    /// Tab Clientside Events
    /// </summary>
    public class MobileTabClientSideEventsBuilder
    {
        #region Fields
        private MobileTabProperties mTabModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mTabModel">The m tab model.</param>
        public MobileTabClientSideEventsBuilder(MobileTabProperties mTabModel)
        {
            this.mTabModel = mTabModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="Load">The load.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder Load(string load)
        {
            mTabModel.Load = load;
            return this;
        }

        /// <summary>
        /// Called when [load complete].
        /// </summary>
        /// <param name="LoadComplete">The load complete.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder LoadComplete(string loadComplete)
        {
            mTabModel.LoadComplete = loadComplete;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder TouchStart(string tStart)
        {
            mTabModel.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder TouchEnd(string tEnd)
        {
            mTabModel.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// Called when [ajax load success].
        /// </summary>
        /// <param name="onAjaxLoadSuccess">The on ajax load success.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder AjaxLoadSuccess(string onAjaxLoadSuccess)
        {
            mTabModel.AjaxLoadSuccess = onAjaxLoadSuccess;
            return this;
        }

        /// <summary>
        /// Called when [ajax load error].
        /// </summary>
        /// <param name="onAjaxLoadError">The on ajax load error.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder AjaxLoadError(string onAjaxLoadError)
        {
            mTabModel.AjaxLoadError = onAjaxLoadError;
            return this;
        }

        /// <summary>
        /// Called when [ajax load complete].
        /// </summary>
        /// <param name="onAjaxLoadComplete">The on ajax load complete.</param>
        /// <returns></returns>
        public MobileTabClientSideEventsBuilder AjaxLoadComplete(string onAjaxLoadComplete)
        {
            mTabModel.AjaxLoadComplete = onAjaxLoadComplete;
            return this;
        }
        #endregion
    }
}
