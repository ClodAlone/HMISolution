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
    /// Tab Properties
    /// </summary>
    public class MobileTabProperties : IMobileBase
    {
        #region Fields
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool allowScrolling = false;
        private bool loadAjaxContent = false;
        private bool allowCache = false;
        private int selectedItemIndex = 1;
        private bool showBadge = false;
        private double badgeValue = 1;
        private double maxBadgeValue = 100;
        private object ajaxOptions = new jQueryAjaxOptions();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>
        /// The theme.
        /// </value>
        [JsonProperty("theme")]
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }
        /// <summary>
        /// Gets or sets a value indicating whether [allow scrolling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow scrolling]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowScrolling")]
        [DefaultValue(false)]
        public bool AllowScrolling { get { return allowScrolling; } set { allowScrolling = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [load ajax content].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [load ajax content]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("loadAjaxContent")]
        [DefaultValue(false)]
        public bool LoadAjaxContent { get { return loadAjaxContent; } set { loadAjaxContent = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [allow cache].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow cache]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowCache")]
        [DefaultValue(false)]
        public bool AllowCache { get { return allowCache; } set { allowCache = value; } }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        /// <value>
        /// The index of the selected item.
        /// </value>
        [JsonProperty("selectedItemIndex")]
        [DefaultValue(1)]
        public int SelectedItemIndex { get { return selectedItemIndex; } set { selectedItemIndex = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show badge].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show badge]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showBadge")]
        [DefaultValue(false)]
        public bool ShowBadge { get { return showBadge; } set { showBadge = value; } }

        /// <summary>
        /// Gets or sets the badge value.
        /// </summary>
        /// <value>
        /// The badge value.
        /// </value>
        [JsonProperty("badgeValue")]
        [DefaultValue(0)]
        public double BadgeValue { get { return badgeValue; } set { badgeValue = value; } }

        /// <summary>
        /// Gets or sets the maximum badge value.
        /// </summary>
        /// <value>
        /// The maximum badge value.
        /// </value>
        [JsonProperty("maxBadgeValue")]
        [DefaultValue(100)]
        public double MaxBadgeValue { get { return maxBadgeValue; } set { maxBadgeValue = value; } }

        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        public string TouchStart { get; set; }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        public string TouchEnd { get; set; }

        /// <summary>
        /// Gets or sets the load.
        /// </summary>
        /// <value>
        /// The load.
        /// </value>
        [JsonProperty("load")]
        public string Load { get; set; }

        /// <summary>
        /// Gets or sets the load complete.
        /// </summary>
        /// <value>
        /// The load complete.
        /// </value>
        [JsonProperty("loadComplete")]
        public string LoadComplete { get; set; }

        /// <summary>
        /// Gets or sets the ajax load success.
        /// </summary>
        /// <value>
        /// The ajax load success.
        /// </value>
        [JsonProperty("ajaxLoadSuccess")]
        public string AjaxLoadSuccess { get; set; }

        /// <summary>
        /// Gets or sets the ajax load error.
        /// </summary>
        /// <value>
        /// The ajax load error.
        /// </value>
        [JsonProperty("ajaxLoadError")]
        public string AjaxLoadError { get; set; }

        /// <summary>
        /// Gets or sets the ajax load error.
        /// </summary>
        /// <value>
        /// The ajax load error.
        /// </value>
        [JsonProperty("ajaxLoadComplete")]
        public string AjaxLoadComplete { get; set; }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileTabIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        [JsonProperty("android")]
        public MobileTabAndroidProperties Android { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileTabWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the flat.
        /// </summary>
        /// <value>
        /// The flat.
        /// </value>
        [JsonProperty("flat")]
        public MobileTabWindowsProperties Flat { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileTabBaseItem> Items
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the ajax options.
        /// </summary>
        /// <value>
        /// The ajax options.
        /// </value>
        [JsonProperty("ajaxOptions")]
        public object AjaxOptions
        {
            get { return this.ajaxOptions; }
            set { this.ajaxOptions = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTabProperties"/> class.
        /// </summary>
        public MobileTabProperties()
        {
            this.IOS7 = new MobileTabIOS7Properties();
            this.Android = new MobileTabAndroidProperties();
            this.Windows = new MobileTabWindowsProperties();
            this.Flat = new MobileTabWindowsProperties();
            this.Items = new List<MobileTabBaseItem>();
        }
        #endregion

    }
}
