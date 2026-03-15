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
    public class MobileEditorProperties :EditorPropertiesBase, IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Light;
        private bool showBorder = false;

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
        [DefaultValue(Theme.Light)]
        public Theme Theme { get { return theme; } set { theme = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [show border].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show border]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showBorder")]
        [DefaultValue(false)]
        public bool ShowBorder { get { return showBorder; } set { showBorder = value; } }


        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileEditorWindowsProperties Windows { get; set; }


        #endregion

       #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileEditorProperties"/> class.
        /// </summary>
        public MobileEditorProperties()
        {
            this.Windows = new MobileEditorWindowsProperties();
        }
       #endregion

    }
}
