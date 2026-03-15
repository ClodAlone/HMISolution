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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Mobile;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// SplitPane IOS7 Properties
    /// </summary>
    public class MobileSplitPaneIOS7Properties
    {
        #region Fields
        private bool showLeftHeader= true;
        private bool showRightHeader= true;
        private string headerLeftButtonCaption= "Back";
        private string headerLeftButtonStyle = "back";
        private bool showHeaderLeftButton= false;
        private string headerRightButtonCaption= "Button";
        private string headerRightButtonStyle="header";
        private bool showHeaderRightButton = false;
        private string onHeaderLeftButtonClick = "";
        private string onHeaderRightButtonClick = "";
        #endregion

        #region IOS7Properties
        /// <summary>
        /// Gets or sets a value showLeftHeader.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showLeftHeader]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showLeftHeader")]
        [DefaultValue(true)]
        public bool ShowLeftHeader { get { return showLeftHeader; } set { showLeftHeader = value; } }

        /// <summary>
        /// Gets or sets a value showRightHeader.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showRightHeader]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showRightHeader")]
        [DefaultValue(true)]
        public bool ShowRightHeader { get { return showRightHeader; } set { showRightHeader = value; } }

        /// <summary>
        /// Gets or sets the on header left button click.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("onHeaderRightButtonClick")]
        [DefaultValue("")]
        public string OnHeaderRightButtonClick { get { return onHeaderRightButtonClick; } set { onHeaderRightButtonClick = value; } }

        /// <summary>
        /// Gets or sets the on header left button click.
        /// </summary>
        /// <value>
        /// Header title.
        /// </value>
        [JsonProperty("onHeaderLeftButtonClick")]
        [DefaultValue("")]
        public string OnHeaderLeftButtonClick { get { return onHeaderLeftButtonClick; } set { onHeaderLeftButtonClick = value; } }


        /// <summary>
        /// Gets or sets a value showHeaderLeftButton.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showHeaderLeftButton]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showHeaderLeftButton")]
        [DefaultValue(false)]
        public bool ShowHeaderLeftButton { get { return showHeaderLeftButton; } set { showHeaderLeftButton = value; } }

        /// <summary>
        /// Gets or sets a value showHeaderLeftButton.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showHeaderLeftButton]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showHeaderRightButton")]
        [DefaultValue(false)]
        public bool ShowHeaderRightButton { get { return showHeaderRightButton; } set { showHeaderRightButton = value; } }

        /// <summary>
        /// Gets or sets a value headerLeftButtonCaption.
        /// </summary>
        /// <value>
        ///   header LeftButton Caption
        /// </value>
        [JsonProperty("headerLeftButtonCaption")]
        [DefaultValue("Back")]
        public string HeaderLeftButtonCaption { get { return headerLeftButtonCaption; } set { headerLeftButtonCaption = value; } }

        /// <summary>
        /// Gets or sets a value headerRightButtonCaption.
        /// </summary>
        /// <value>
        ///   header RightButton Caption
        /// </value>
        [JsonProperty("headerRightButtonCaption")]
        [DefaultValue("Button")]
        public string HeaderRightButtonCaption { get { return headerRightButtonCaption; } set { headerRightButtonCaption = value; } }


        /// <summary>
        /// Gets or sets a value headerLeftButtonStyle.
        /// </summary>
        /// <value>
        ///   header LeftButton Style
        /// </value>
        [JsonProperty("headerLeftButtonStyle")]
        [DefaultValue("back")]
        public string HeaderLeftButtonStyle { get { return headerLeftButtonStyle; } set { headerLeftButtonStyle = value; } }

        /// <summary>
        /// Gets or sets a value headerRightButtonStyle.
        /// </summary>
        /// <value>
        ///   header RightButton Style
        /// </value>
        [JsonProperty("headerRightButtonStyle")]
        [DefaultValue("header")]
        public string HeaderRightButtonStyle { get { return headerRightButtonStyle; } set { headerRightButtonStyle = value; } }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneIOS7Properties"/> class.
        /// </summary>
        public MobileSplitPaneIOS7Properties() { }
        #endregion
    }

    /// <summary>
    /// SplitPane Android Properties
    /// </summary>
    public class MobileSplitPaneAndroidProperties
    {
        #region Fields
            private bool showToolbar= true;
			private string toolbarTitle= "Settings"; 
			private string toolbarTheme= "dark"; 
			private bool showToolbarIcon= true;
			private bool showToolbarBackNavigator= false;
            private bool showToolbarEllipsis = false;
        #endregion

        #region AndroidProperties
            /// <summary>
            /// Gets or sets a value indicating whether [show Toolbar].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [show Toolbar]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("showToolbar")]
            [DefaultValue(true)]
            public bool ShowToolbar { get { return showToolbar; } set { showToolbar = value; } }

            /// <summary>
            /// Gets or sets a value indicating whether [showToolbarIcon].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [showToolbarIcon]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("showToolbarIcon")]
            [DefaultValue(true)]
            public bool ShowToolbarIcon { get { return showToolbarIcon; } set { showToolbarIcon = value; } }

            /// <summary>
            /// Gets or sets a value indicating whether [showToolbarBackNavigator].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [showToolbarBackNavigator]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("showToolbarBackNavigator")]
            [DefaultValue(false)]
            public bool ShowToolbarBackNavigator { get { return showToolbarBackNavigator; } set { showToolbarBackNavigator = value; } }

            /// <summary>
            /// Gets or sets a value indicating whether [showToolbarEllipsis].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [showToolbarEllipsis]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("showToolbarEllipsis")]
            [DefaultValue(false)]
            public bool ShowToolbarEllipsis { get { return showToolbarEllipsis; } set { showToolbarEllipsis = value; } }

            /// <summary>
            /// Gets or sets a value indicating whether [showToolbarEllipsis].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [showToolbarEllipsis]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("toolbarTitle")]
            [DefaultValue("Settings")]
            public string ToolbarTitle { get { return toolbarTitle; } set { toolbarTitle = value; } }

            /// <summary>
            /// Gets or sets a value indicating whether [toolbarTheme].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [toolbarTheme]; otherwise, <c>false</c>.
            /// </value>
            [JsonProperty("toolbarTheme")]
            [DefaultValue("dark")]
            public string ToolbarTheme { get { return toolbarTheme; } set { toolbarTheme = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneAndroidProperties"/> class.
        /// </summary>
        public MobileSplitPaneAndroidProperties() { }
        #endregion
    }

    /// <summary>
    /// SplitPane Windows Properties
    /// </summary>
    public class MobileSplitPaneWindowsProperties : WindowsBase
    {
        #region Fields
        private bool showHeader = true;
        #endregion

        #region WindowsProperties
        /// <summary>
        /// Gets or sets a value indicating whether [showHeader].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [showHeader]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showHeader")]
        [DefaultValue(true)]
        public bool ShowHeader { get { return showHeader; } set { showHeader = value; } }
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneWindowsProperties"/> class.
        /// </summary>
        public MobileSplitPaneWindowsProperties() { }
        #endregion
    }

    /// <summary>
    /// SplitPane Flat Properties
    /// </summary>
    public class MobileSplitPaneFlatProperties
    {
        #region Fields
        private ControlPosition position = ControlPosition.Fixed;
        #endregion

        #region FlatProperties
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        [JsonProperty("postion")]
        [DefaultValue(ControlPosition.Fixed)]
        public ControlPosition Position { get { return position; } set { position = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSplitPaneFlatProperties"/> class.
        /// </summary>
        public MobileSplitPaneFlatProperties() { }
        #endregion
    }
}
