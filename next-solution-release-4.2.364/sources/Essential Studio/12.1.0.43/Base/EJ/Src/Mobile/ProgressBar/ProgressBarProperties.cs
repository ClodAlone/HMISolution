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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Mobile;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.Mobile.Models
{

    /// <summary>
    /// Class for ProgressBar Properties 
    /// </summary>
    public class MobileProgressBarProperties : ProgressBarPropertiesBase, IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private double width = 0;
        private double height = 0;
        private Orientation orientation = Orientation.Horizontal;

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
        /// Gets or sets a value indicating whether [allow custom text].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow custom text]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("allowCustomText")]
        [DefaultValue(false)]
        public bool AllowCustomText { get; set; }

        /// <summary>
        /// Gets or sets the on custom text.
        /// </summary>
        /// <value>
        /// The on custom text.
        /// </value>
        [JsonProperty("onCustomText")]
        [DefaultValue("")]
        public string OnCustomText { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [JsonProperty("width")]
        [DefaultValue(0)]
        public double Width { get { return width; } set { width = value; } }


        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonProperty("height")]
        [DefaultValue(0)]
        public double Height { get { return height; } set { height = value; } }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        [JsonProperty("orientation")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation { get { return orientation; } set { orientation = value; } }

        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>
        /// The create.
        /// </value>
        [JsonProperty("create")]
        public string Create { get; set; }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get; set; }

        /// <summary>
        /// Gets or sets the complete.
        /// </summary>
        /// <value>
        /// The complete.
        /// </value>
        [JsonProperty("complete")]
        public string Complete { get; set; }

        #endregion

        #region Constructor

        #endregion

    }
}
