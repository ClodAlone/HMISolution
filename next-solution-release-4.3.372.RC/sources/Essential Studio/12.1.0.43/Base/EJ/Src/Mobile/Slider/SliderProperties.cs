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
    /// Class for Slider Properties 
    /// </summary>
    public class MobileSliderProperties : SliderPropertiesBase, IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private int[] values = { 30, 80 };

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
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        [JsonProperty("values")]
        [DefaultValue(null)]
        public int[] Values
        {
            get { return this.values; }
            set { this.values = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        [JsonProperty("start")]
        public string Start { get; set; }

        /// <summary>
        /// Gets or sets the stop.
        /// </summary>
        /// <value>
        /// The stop.
        /// </value>
        [JsonProperty("stop")]
        public string Stop { get; set; }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        public string Change { get; set; }

        /// <summary>
        /// Gets or sets the slide.
        /// </summary>
        /// <value>
        /// The slide.
        /// </value>
        [JsonProperty("slide")]
        public string Slide { get; set; }

        /// <summary>
        /// Gets or sets the load.
        /// </summary>
        /// <value>
        /// The load.
        /// </value>
        [JsonProperty("load")]
        public string Load { get; set; }



        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The ios7.
        /// </value>
        public MobileSliderIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        public MobileSliderWindowsProperties Windows { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileSliderProperties"/> class.
        /// </summary>
        public MobileSliderProperties()
        {
            this.IOS7 = new MobileSliderIOS7Properties();
            this.Windows = new MobileSliderWindowsProperties();
        }
        #endregion

    }


}
