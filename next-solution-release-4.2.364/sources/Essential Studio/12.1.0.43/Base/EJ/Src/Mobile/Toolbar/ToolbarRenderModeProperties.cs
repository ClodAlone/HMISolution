#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Mobile
{
    public class MobileToolbarWindowsProperties : WindowsBase
    {
        #region Fields

        private bool renderDefault = false;

        #endregion

        #region WindowsToolbarProperties

        [JsonProperty("menuTargetId")]
        public string MenuTargetId { get; set; }

        [JsonProperty("renderDefault")]
        [DefaultValue(false)]
        public bool RenderDefault { get { return renderDefault; } set { renderDefault = value; } }

        #endregion

        #region Constructors

        public MobileToolbarWindowsProperties() { }

        #endregion

    }

    public class MobileToolbarAndroidProperties
    {
        #region Fields

        private bool showTitleIconNavigator = false;
        private bool splitView = false;
        private bool showEllipsis = false;
        private bool showBackNavigator = false;

        #endregion

        #region AndroidToolbarProperties

        /// <summary>
        /// Gets or sets the title icon URL.
        /// </summary>
        /// <value>
        /// The title icon URL.
        /// </value>
        [JsonProperty("titleIconUrl")]
        public string TitleIconUrl { get; set; }

        /// <summary>
        /// Gets or sets the menu target identifier.
        /// </summary>
        /// <value>
        /// The menu target identifier.
        /// </value>
        [JsonProperty("menuTargetId")]
        public string MenuTargetId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the on ellipsis touch start.
        /// </summary>
        /// <value>
        /// The on ellipsis touch start.
        /// </value>
        [JsonProperty("onEllipsisTouchStart")]
        public string OnEllipsisTouchStart { get; set; }

        /// <summary>
        /// Gets or sets the on ellipsis touch end.
        /// </summary>
        /// <value>
        /// The on ellipsis touch end.
        /// </value>
        [JsonProperty("onEllipsisTouchEnd")]
        public string OnEllipsisTouchEnd { get; set; }

        /// <summary>
        /// Gets or sets the on back navigator touch start.
        /// </summary>
        /// <value>
        /// The on back navigator touch start.
        /// </value>
        [JsonProperty("onBackNavigatorTouchStart")]
        public string OnBackNavigatorTouchStart { get; set; }

        /// <summary>
        /// Gets or sets the on back navigator touch end.
        /// </summary>
        /// <value>
        /// The on back navigator touch end.
        /// </value>
        [JsonProperty("onBackNavigatorTouchEnd")]
        public string OnBackNavigatorTouchEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show back navigator].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show back navigator]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showBackNavigator")]
        [DefaultValue(false)]
        public bool ShowBackNavigator { get { return showBackNavigator; } set { showBackNavigator = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show title icon navigator].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show title icon navigator]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showTitleIconNavigator")]
        [DefaultValue(false)]
        public bool ShowTitleIconNavigator { get { return showTitleIconNavigator; } set { showTitleIconNavigator = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [split view].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [split view]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("splitView")]
        [DefaultValue(false)]
        public bool SplitView { get { return splitView; } set { splitView = value; } }

        [JsonProperty("showEllipsis")]
        [DefaultValue(false)]
        public bool ShowEllipsis { get { return showEllipsis; } set { showEllipsis = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToolbarAndroidProperties"/> class.
        /// </summary>
        public MobileToolbarAndroidProperties() { }

        #endregion

    }
}
