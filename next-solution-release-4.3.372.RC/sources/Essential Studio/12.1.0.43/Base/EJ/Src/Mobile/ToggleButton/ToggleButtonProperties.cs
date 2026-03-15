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
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for MobileToggleButtonProperties
    /// </summary>
    public class MobileToggleButtonProperties : IMobileBase 
    {
        #region Fields

        private bool animate = true;
        private bool togglestate = true;
        private bool persist = false;
        private bool enabled = true;
        private RenderMode rendermode = RenderMode.Auto;
        private Theme theme = Theme.Auto;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the rendermode.
        /// </summary>
        /// <value>
        /// The rendermode.
        /// </value>
        [JsonProperty("rendermode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return rendermode; } set { rendermode = value; } }

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
        /// Gets or sets a value indicating whether this <see cref="MToggleButtonProperties"/> is animate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("animate")]
        [DefaultValue(true)]
        public bool Animate { get { return animate; } set { animate = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [toggle state].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [toggle state]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("toggleState")]
        [DefaultValue(true)]
        public bool ToggleState { get { return togglestate; } set { togglestate = value; } }

        /// <summary>
        /// Gets or sets the state change.
        /// </summary>
        /// <value>
        /// The state change.
        /// </value>
        [JsonProperty("stateChange")]
        [DefaultValue(null)]
        public string StateChange { get; set; }

        /// <summary>
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("touchStart")]
        [DefaultValue(null)]
        public string TouchStart { get; set; }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("touchEnd")]
        [DefaultValue(null)]
        public string TouchEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MToggleButtonProperties"/> is animate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MToggleButtonProperties"/> is animate.
        /// </summary>
        /// <value>
        ///   <c>true</c> if animate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist { get { return persist; } set { persist = value; } }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public MobileToggleButtonWindowsProperties Windows { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileToggleButtonProperties"/> class.
        /// </summary>
        public MobileToggleButtonProperties()
        {
            this.Windows = new MobileToggleButtonWindowsProperties();
        }

        #endregion
    }
  
}
