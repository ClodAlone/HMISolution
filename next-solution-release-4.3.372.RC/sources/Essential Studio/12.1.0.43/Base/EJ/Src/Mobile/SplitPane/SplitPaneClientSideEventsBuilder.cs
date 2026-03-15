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
    /// SplitPane Clientside Events
    /// </summary>
    public class MobileSplitPaneClientSideEventsBuilder
    {
        #region Fields
        private MobileSplitPaneProperties mSplitPaneModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPaneModel">The m SplitPane model.</param>
        public MobileSplitPaneClientSideEventsBuilder(MobileSplitPaneProperties mSplitPaneModel)
        {
            this.mSplitPaneModel = mSplitPaneModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Ioes the s7.
        /// </summary>
        /// <param name="ios7Model">The ios7 model.</param>
        /// <returns></returns>
        public MobileSplitPaneClientSideEventsBuilder IOS7(Action<MobileSplitPaneIOS7ClientSideEventsBuilder> ios7Model)
        {
            var builder = new MobileSplitPaneIOS7ClientSideEventsBuilder(this.mSplitPaneModel);
            if (ios7Model != null)
                ios7Model.Invoke(builder);
            return this;
        }
        #endregion
    }
    /// <summary>
    /// SplitPane Clientside Events
    /// </summary>
    public class MobileSplitPaneIOS7ClientSideEventsBuilder
    {
        private MobileSplitPaneProperties mSplitPaneModel;
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPaneModel">The m SplitPane model.</param>
        public MobileSplitPaneIOS7ClientSideEventsBuilder(MobileSplitPaneProperties mSplitPaneModel)
        {
            this.mSplitPaneModel = mSplitPaneModel;
        }
        #endregion
        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="onLoad">The on load.</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7ClientSideEventsBuilder OnHeaderLeftButtonClick(string click)
        {
            mSplitPaneModel.IOS7.OnHeaderLeftButtonClick = click;
            return this;
        }
        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="onLoad">The on load.</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7ClientSideEventsBuilder OnHeaderRightButtonClick(string click)
        {
            mSplitPaneModel.IOS7.OnHeaderRightButtonClick = click;
            return this;
        }
    }
}
