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
    public class MobileListboxClientSideEventsBuilder
    {
        #region Fields
        private MobileListboxProperties mListboxModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileListboxClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mListboxModel">The m listbox model.</param>
        public MobileListboxClientSideEventsBuilder(MobileListboxProperties mListboxModel)
        {
            this.mListboxModel = mListboxModel;
        }
        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="Load">The load.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder Load(string load)
        {
            mListboxModel.Load = load;
            return this;
        }
        /// <summary>
        /// Called when [load complete].
        /// </summary>
        /// <param name="LoadComplete">The load complete.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder LoadComplete(string loadComplete)
        {
            mListboxModel.LoadComplete = loadComplete;
            return this;
        }
        /// <summary>
        /// Called when [ajax before send].
        /// </summary>
        /// <param name="onAjaxBeforeSend">The on ajax before send.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder AjaxBeforeSend(string onAjaxBeforeSend)
        {
            mListboxModel.AjaxBeforeSend = onAjaxBeforeSend;
            return this;
        }
        /// <summary>
        /// Called when [ajax load success].
        /// </summary>
        /// <param name="onAjaxLoadSuccess">The on ajax load success.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder AjaxLoadSuccess(string onAjaxLoadSuccess)
        {
            mListboxModel.AjaxLoadSuccess = onAjaxLoadSuccess;
            return this;
        }
        /// <summary>
        /// Called when [ajax load error].
        /// </summary>
        /// <param name="onAjaxLoadError">The on ajax load error.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder AjaxLoadError(string onAjaxLoadError)
        {
            mListboxModel.AjaxLoadError = onAjaxLoadError;
            return this;
        }

        /// <summary>
        /// Called when [ajax load complete].
        /// </summary>
        /// <param name="onAjaxLoadComplete">The on ajax load complete.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder AjaxLoadComplete(string onAjaxLoadComplete)
        {
            mListboxModel.AjaxLoadComplete = onAjaxLoadComplete;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder TouchStart(string tStart)
        {
            mListboxModel.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchStart">The touch end.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder TouchEnd(string tEnd)
        {
            mListboxModel.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// Headers the button click.
        /// </summary>
        /// <param name="headerButtonClick">The header button click.</param>
        /// <returns></returns>
        public MobileListboxClientSideEventsBuilder HeaderButtonTap(string headerButtonClick)
        {
            mListboxModel.HeaderButtonTap = headerButtonClick;
            return this;
        }
            
        #endregion        
    }
}
