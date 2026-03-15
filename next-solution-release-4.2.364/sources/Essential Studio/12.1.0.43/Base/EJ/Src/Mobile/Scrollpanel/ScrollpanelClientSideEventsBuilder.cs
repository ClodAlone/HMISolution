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
    public class MobileScrollpanelClientSideEventsBuilder
    {
        #region Fields
        private MobileScrollpanelProperties mScrollpanelModel;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileScrollpanelClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mScrollpanelModel">The m scrollpanel model.</param>
        public MobileScrollpanelClientSideEventsBuilder(MobileScrollpanelProperties mScrollpanelModel)
        {
            this.mScrollpanelModel = mScrollpanelModel;
        }
        #endregion 

        #region Events
        /// <summary>
        /// Scrolls the start.
        /// </summary>
        /// <param name="scrollStart">The scroll start.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder ScrollStart(string scrollStart)
        {
            mScrollpanelModel.ScrollStart = scrollStart;
            return this;
        }
        /// <summary>
        /// Scrolls the move.
        /// </summary>
        /// <param name="scrollMove">The scroll move.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder ScrollMove(string scrollMove)
        {
            mScrollpanelModel.ScrollMove = scrollMove;
            return this;
        }
        /// <summary>
        /// Scrolls the end.
        /// </summary>
        /// <param name="scrollEnd">The scroll end.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder ScrollEnd(string scrollEnd)
        {
            mScrollpanelModel.ScrollEnd = scrollEnd;
            return this;
        }
        /// <summary>
        /// Befores the scroll start.
        /// </summary>
        /// <param name="beforeScrollStart">The before scroll start.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder BeforeScrollStart(string beforeScrollStart)
        {
            mScrollpanelModel.BeforeScrollStart = beforeScrollStart;
            return this;
        }
        /// <summary>
        /// Zooms the start.
        /// </summary>
        /// <param name="zoomStart">The zoom start.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder ZoomStart(string zoomStart)
        {
            mScrollpanelModel.ZoomStart = zoomStart;
            return this;
        }

        /// <summary>
        /// Zooms the end.
        /// </summary>
        /// <param name="zoomEnd">The zoom end.</param>
        /// <returns></returns>
        public MobileScrollpanelClientSideEventsBuilder ZoomEnd(string zoomEnd)
        {
            mScrollpanelModel.ZoomEnd = zoomEnd;
            return this;
        }
            
        #endregion        
    }
}
