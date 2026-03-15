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
    /// Menu Properties
    /// </summary>
    public class MobileMenuProperties : IMobileBase
    {
        #region Fields
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private bool allowScrolling = true;
        private string openMenu = "tap";
        private bool scrollbars = true;
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
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonProperty("height")]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [JsonProperty("width")]
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow scrolling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow scrolling]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowScrolling")]
        [DefaultValue(true)]
        public bool AllowScrolling { get { return allowScrolling; } set { allowScrolling = value; } }

        /// <summary>
        /// Gets or sets the target identifier.
        /// </summary>
        /// <value>
        /// The target identifier.
        /// </value>
        [JsonProperty("targetId")]
        public string TargetId { get; set; }

        /// <summary>
        /// Gets or sets the open menu.
        /// </summary>
        /// <value>
        /// The open menu.
        /// </value>
        [JsonProperty("openMenu")]
        [DefaultValue("tap")]
        public string OpenMenu { get { return openMenu; } set { openMenu = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [render template].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render template]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("renderTemplate")]
        [DefaultValue(false)]
        public bool RenderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        [JsonProperty("templateId")]
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileMenuProperties"/> is scrollbars.
        /// </summary>
        /// <value>
        ///   <c>true</c> if scrollbars; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("scrollbars")]
        [DefaultValue(true)]
        public bool Scrollbars { get { return scrollbars; } set { scrollbars = value; } }

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
        /// Gets or sets the on hide.
        /// </summary>
        /// <value>
        /// The on hide.
        /// </value>
        [JsonProperty("hide")]
        public string Hide { get; set; }

        /// <summary>
        /// Gets or sets the on show.
        /// </summary>
        /// <value>
        /// The on show.
        /// </value>
        [JsonProperty("show")]
        public string Show { get; set; }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        public MobileMenuIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        public MobileMenuAndroidProperties Android { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public MobileMenuWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileMenuBaseItem> Items
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileMenuProperties"/> class.
        /// </summary>
        public MobileMenuProperties()
        {
            this.IOS7 = new MobileMenuIOS7Properties();
            this.Android = new MobileMenuAndroidProperties();
            this.Windows = new MobileMenuWindowsProperties();
            this.Items = new List<MobileMenuBaseItem>();
        }
        #endregion
    }
}
