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
    /// Menu Clientside Events
    /// </summary>
    public class MobileMenuClientSideEventsBuilder
    {
        #region Fields
        private MobileMenuProperties mMenuModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuClientSideEventsBuilder"/> class.
        /// </summary>
        /// <param name="mMenuModel">The m menu model.</param>
        public MobileMenuClientSideEventsBuilder(MobileMenuProperties mMenuModel)
        {
            this.mMenuModel = mMenuModel;
        }
        #endregion

        #region Events
        /// <summary>
        /// Called when [load].
        /// </summary>
        /// <param name="Load">The load.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder Load(string load)
        {
            mMenuModel.Load = load;
            return this;
        }

        /// <summary>
        /// Called when [load complete].
        /// </summary>
        /// <param name="LoadComplete">The load complete.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder LoadComplete(string loadComplete)
        {
            mMenuModel.LoadComplete = loadComplete;
            return this;
        }

        /// <summary>
        /// Called when [touch start].
        /// </summary>
        /// <param name="TouchStart">The touch start.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder TouchStart(string tStart)
        {
            mMenuModel.TouchStart = tStart;
            return this;
        }

        /// <summary>
        /// Called when [touch end].
        /// </summary>
        /// <param name="TouchEnd">The touch end.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder TouchEnd(string tEnd)
        {
            mMenuModel.TouchEnd = tEnd;
            return this;
        }

        /// <summary>
        /// Called when [show].
        /// </summary>
        /// <param name="onShow">The on show.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder Show(string onShow)
        {
            mMenuModel.Show = onShow;
            return this;
        }

        /// <summary>
        /// Called when [hide].
        /// </summary>
        /// <param name="onHide">The on hide.</param>
        /// <returns></returns>
        public MobileMenuClientSideEventsBuilder Hide(string onHide)
        {
            mMenuModel.Hide = onHide;
            return this;
        }
        #endregion
    }
}
