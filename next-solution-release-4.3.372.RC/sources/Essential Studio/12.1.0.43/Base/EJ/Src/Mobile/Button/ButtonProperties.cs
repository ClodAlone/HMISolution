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
    /// Class for MobileButtonProperties
    /// </summary>
    public class MobileButtonProperties : ButtonPropertiesBase, IMobileBase
    {
        #region Fields
        private bool bindBack = true;
        private string imageClass;
        private Theme theme = Theme.Light;
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
        public RenderMode RenderMode { get; set; }

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
              

        /// <summary>
        /// Gets or sets a value indicating whether [bind back].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bind back]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("bindBack")]
        [DefaultValue(true)]
        public bool BindBack { get { return bindBack; } set { bindBack = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }


        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileButtonIOS7Properties IOS7 { get; set; }


        /// <summary>
        /// Gets or sets the android.
        /// </summary>
        /// <value>
        /// The android.
        /// </value>
        [JsonProperty("android")]
        public MobileButtonAndroidProperties Android { get; set; }


        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileButtonWindowsProperties Windows { get; set; }


        /// <summary>
        /// Gets or sets the flat.
        /// </summary>
        /// <value>
        /// The flat.
        /// </value>
        [JsonProperty("flat")]
        public MobileButtonFlatProperties Flat { get; set; }

        #endregion      

         #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileButtonProperties"/> class.
        /// </summary>
        public MobileButtonProperties()
        {
            this.IOS7 = new MobileButtonIOS7Properties();
            this.Android = new MobileButtonAndroidProperties();
            this.Windows = new MobileButtonWindowsProperties();
            this.Flat = new MobileButtonFlatProperties();
        }
        #endregion

    }
}
