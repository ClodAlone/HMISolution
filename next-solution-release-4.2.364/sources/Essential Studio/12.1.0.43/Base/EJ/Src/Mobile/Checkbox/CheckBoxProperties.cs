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
    /// Class for MobileCheckBoxProperties
    /// </summary>
    public class MobileCheckBoxProperties : CheckBoxPropertiesBase,IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;


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
        /// Gets or sets the touch start.
        /// </summary>
        /// <value>
        /// The touch start.
        /// </value>
        [JsonProperty("TouchStart")]
        public string TouchStart { get; set; }

        /// <summary>
        /// Gets or sets the touch end.
        /// </summary>
        /// <value>
        /// The touch end.
        /// </value>
        [JsonProperty("TouchEnd")]
        public string TouchEnd { get; set; }

        
        #endregion


        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileCheckBoxWindowsProperties Windows { get; set; }

        
       #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileCheckBoxProperties"/> class.
        /// </summary>
        public  MobileCheckBoxProperties()
        {
            this.Windows = new MobileCheckBoxWindowsProperties();
        }
       #endregion

    }
}
