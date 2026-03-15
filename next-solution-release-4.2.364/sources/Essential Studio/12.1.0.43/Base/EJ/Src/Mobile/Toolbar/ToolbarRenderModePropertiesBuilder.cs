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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{

    public class MobileToolbarWindowsPropertiesBuilder
    {
        #region Fields

        /// <summary>
        /// Gets or sets the mobile toolbar model.
        /// </summary>
        /// <value>
        /// The mobile toolbar model.
        /// </value>
        private MobileToolbarProperties MobileToolbarModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mobileToolbarModel">The mobile toolbar model.</param>
        public MobileToolbarWindowsPropertiesBuilder(MobileToolbarProperties mobileToolbarModel)
        {
            this.MobileToolbarModel = mobileToolbarModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Menus the target identifier.
        /// </summary>
        /// <param name="menuTargetId">The menu target identifier.</param>
        /// <returns></returns>
        public MobileToolbarWindowsPropertiesBuilder MenuTargetId(string menuTargetId)
        {
            this.MobileToolbarModel.Windows.MenuTargetId = menuTargetId;
            return this;
        }

        /// <summary>
        /// Renders the default.
        /// </summary>
        /// <param name="renderDefault">if set to <c>true</c> [render default].</param>
        /// <returns></returns>
        public MobileToolbarWindowsPropertiesBuilder RenderDefault(bool renderDefault)
        {
            this.MobileToolbarModel.Windows.RenderDefault = renderDefault;
            return this;
        }

        #endregion
    }

    public class MobileToolbarAndroidPropertiesBuilder
    {
        #region Fields

        /// <summary>
        /// Gets or sets the mobile toolbar model.
        /// </summary>
        /// <value>
        /// The mobile toolbar model.
        /// </value>
        private MobileToolbarProperties MobileToolbarModel { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mobileToolbarModel">The mobile toolbar model.</param>
        public MobileToolbarAndroidPropertiesBuilder(MobileToolbarProperties mobileToolbarModel)
        {
            this.MobileToolbarModel = mobileToolbarModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Menus the target identifier.
        /// </summary>
        /// <param name="menuTargetId">The menu target identifier.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder MenuTargetId(string menuTargetId)
        {
            this.MobileToolbarModel.Android.MenuTargetId = menuTargetId;
            return this;
        }

        /// <summary>
        /// Titles the icon URL.
        /// </summary>
        /// <param name="titleIconUrl">The title icon URL.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder TitleIconUrl(string titleIconUrl)
        {
            this.MobileToolbarModel.Android.TitleIconUrl = titleIconUrl;
            return this;
        }

        /// <summary>
        /// Titles the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder Title(string title)
        {
            this.MobileToolbarModel.Android.Title = title;
            return this;
        }

        /// <summary>
        /// Called when [ellipsis touch start].
        /// </summary>
        /// <param name="onEllipsisTouchStart">The on ellipsis touch start.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder OnEllipsisTouchStart(string onEllipsisTouchStart)
        {
            this.MobileToolbarModel.Android.OnEllipsisTouchStart = onEllipsisTouchStart;
            return this;
        }

        /// <summary>
        /// Called when [ellipsis touch end].
        /// </summary>
        /// <param name="onEllipsisTouchEnd">The on ellipsis touch end.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder OnEllipsisTouchEnd(string onEllipsisTouchEnd)
        {
            this.MobileToolbarModel.Android.OnEllipsisTouchEnd = onEllipsisTouchEnd;
            return this;
        }

        /// <summary>
        /// Called when [back navigator touch start].
        /// </summary>
        /// <param name="onBackNavigatorTouchStart">The on back navigator touch start.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder OnBackNavigatorTouchStart(string onBackNavigatorTouchStart)
        {
            this.MobileToolbarModel.Android.OnBackNavigatorTouchStart = onBackNavigatorTouchStart;
            return this;
        }

        /// <summary>
        /// Called when [back navigator touch end].
        /// </summary>
        /// <param name="onBackNavigatorTouchEnd">The on back navigator touch end.</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder OnBackNavigatorTouchEnd(string onBackNavigatorTouchEnd)
        {
            this.MobileToolbarModel.Android.OnBackNavigatorTouchEnd = onBackNavigatorTouchEnd;
            return this;
        }

        /// <summary>
        /// Shows the back navigator.
        /// </summary>
        /// <param name="showBackNavigator">if set to <c>true</c> [show back navigator].</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder ShowBackNavigator(bool showBackNavigator)
        {
            this.MobileToolbarModel.Android.ShowBackNavigator = showBackNavigator;
            return this;
        }

        /// <summary>
        /// Shows the title icon navigator.
        /// </summary>
        /// <param name="showTitleIconNavigator">if set to <c>true</c> [show title icon navigator].</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder ShowTitleIconNavigator(bool showTitleIconNavigator)
        {
            this.MobileToolbarModel.Android.ShowTitleIconNavigator = showTitleIconNavigator;
            return this;
        }

        /// <summary>
        /// Splits the view.
        /// </summary>
        /// <param name="splitView">if set to <c>true</c> [split view].</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder SplitView(bool splitView)
        {
            this.MobileToolbarModel.Android.SplitView = splitView;
            return this;
        }

        /// <summary>
        /// Shows the ellipsis.
        /// </summary>
        /// <param name="showEllipsis">if set to <c>true</c> [show ellipsis].</param>
        /// <returns></returns>
        public MobileToolbarAndroidPropertiesBuilder ShowEllipsis(bool showEllipsis)
        {
            this.MobileToolbarModel.Android.ShowEllipsis = showEllipsis;
            return this;
        }

        #endregion
    }
}
