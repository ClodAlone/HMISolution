#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    /// <summary>
    /// SplitPane IOS7 Properties Builder
    /// </summary>
    public class MobileSplitPaneIOS7PropertiesBuilder
    {
        #region Fields
        private MobileSplitPaneProperties MobileSplitPaneModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneIOS7PropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPaneModel">The m SplitPane model.</param>
        public MobileSplitPaneIOS7PropertiesBuilder(MobileSplitPaneProperties mSplitPaneModel)
        {
            this.MobileSplitPaneModel = mSplitPaneModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Shows the left header.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showLeftHeader].</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder ShowLeftHeader(bool showLeftHeader)
        {
            this.MobileSplitPaneModel.IOS7.ShowLeftHeader = showLeftHeader;
            return this;
        }

        /// <summary>
        /// Shows the right header.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showRightHeader].</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder ShowRightHeader(bool showRightHeader)
        {
            this.MobileSplitPaneModel.IOS7.ShowRightHeader = showRightHeader;
            return this;
        }

        /// <summary>
        /// Shows the header right button.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showHeaderRightButton].</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder ShowHeaderRightButton(bool showHeaderRightButton)
        {
            this.MobileSplitPaneModel.IOS7.ShowHeaderRightButton = showHeaderRightButton;
            return this;
        }

        /// <summary>
        /// Shows the header left button.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showHeaderLeftButton].</param>
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder ShowHeaderLeftButton(bool showHeaderLeftButton)
        {
            this.MobileSplitPaneModel.IOS7.ShowHeaderLeftButton = showHeaderLeftButton;
            return this;
        }


        /// <summary>
        /// sets the LeftButton Caption
        /// </summary>
        /// sets the leftbutton caption
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder HeaderLeftButtonCaption(string headerLeftButtonCaption)
        {
            this.MobileSplitPaneModel.IOS7.HeaderLeftButtonCaption = headerLeftButtonCaption;
            return this;
        }


        /// <summary>
        /// sets the RightButton Caption
        /// </summary>
        /// sets the Rightbutton caption
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder HeaderRightButtonCaption(string headerRightButtonCaption)
        {
            this.MobileSplitPaneModel.IOS7.HeaderRightButtonCaption = headerRightButtonCaption;
            return this;
        }

        /// <summary>
        /// sets the Left Button Style
        /// </summary>
        /// sets the leftbutton style
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder headerLeftButtonStyle(string headerLeftButtonStyle)
        {
            this.MobileSplitPaneModel.IOS7.HeaderLeftButtonStyle = headerLeftButtonStyle;
            return this;
        }

        /// <summary>
        /// sets the Right Button Style
        /// </summary>
        /// sets the rightbutton style
        /// <returns></returns>
        public MobileSplitPaneIOS7PropertiesBuilder headerRightButtonStyle(string headerRightButtonStyle)
        {
            this.MobileSplitPaneModel.IOS7.HeaderRightButtonStyle = headerRightButtonStyle;
            return this;
        }
        #endregion

    }

    /// <summary>
    /// SplitPane Android Properties Builder
    /// </summary>
    public class MobileSplitPaneAndroidPropertiesBuilder
    {
        #region Fields
        private MobileSplitPaneProperties MobileSplitPaneModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneAndroidPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPaneModel">The m SplitPane model.</param>
        public MobileSplitPaneAndroidPropertiesBuilder(MobileSplitPaneProperties mSplitPaneModel)
        {
            this.MobileSplitPaneModel = mSplitPaneModel;
        }
        #endregion

        #region Builder
        /// <summary>
        /// Shows the Toolbar.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showToolbar].</param>
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder showToolbar(bool showToolbar)
        {
            this.MobileSplitPaneModel.Android.ShowToolbar = showToolbar;
            return this;
        }

        /// <summary>
        /// Shows the ToolbarIcon.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showToolbarIcon].</param>
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder showToolbarIcon(bool showToolbarIcon)
        {
            this.MobileSplitPaneModel.Android.ShowToolbarIcon = showToolbarIcon;
            return this;
        }

        /// <summary>
        /// Shows the ToolbarBackNavigator.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [ToolbarBackNavigator].</param>
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder showToolbarBackNavigator(bool showToolbarBackNavigator)
        {
            this.MobileSplitPaneModel.Android.ShowToolbarBackNavigator = showToolbarBackNavigator;
            return this;
        }

        /// <summary>
        /// Shows the ToolbarEllipsis.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showToolbarEllipsis].</param>
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder ShowToolbarEllipsis(bool showToolbarEllipsis)
        {
            this.MobileSplitPaneModel.Android.ShowToolbarEllipsis = showToolbarEllipsis;
            return this;
        }

        /// <summary>
        /// Shows the ToolbarEllipsis.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [showToolbarEllipsis].</param>
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder ToolbarTitle(string toolbarTitle)
        {
            this.MobileSplitPaneModel.Android.ToolbarTitle = toolbarTitle;
            return this;
        }

        /// <summary>
        /// Shows the toolbarTheme.
        /// </summary>
        /// toolbarTheme
        /// <returns></returns>
        public MobileSplitPaneAndroidPropertiesBuilder ToolbarTheme(string toolbarTheme)
        {
            this.MobileSplitPaneModel.Android.ToolbarTheme = toolbarTheme;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// SplitPane Windows Properties Builder
    /// </summary>
    public class MobileSplitPaneWindowsPropertiesBuilder
    {
        #region Fields
        private MobileSplitPaneProperties MobileSplitPaneModel { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneWindowsPropertiesBuilder"/> class.
        /// </summary>
        /// <param name="mSplitPaneModel">The m SplitPane model.</param>
        public MobileSplitPaneWindowsPropertiesBuilder(MobileSplitPaneProperties mSplitPaneModel)
        {
            this.MobileSplitPaneModel = mSplitPaneModel;
        }
        #endregion

        #region Builder

        /// <summary>
        /// Shows the Header.
        /// </summary>
        /// <param name="showImage">if set to <c>true</c> [ShowHeader].</param>
        /// <returns></returns>
        public MobileSplitPaneWindowsPropertiesBuilder ShowHeader(bool showHeader)
        {
            this.MobileSplitPaneModel.Windows.ShowHeader = showHeader;
            return this;
        }
        #endregion
    }
}
