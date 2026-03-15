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
using System.Runtime.Serialization;
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for TimePicker Property
    /// </summary>
    public class MobileTimePickerProperties : TimePickerPropertiesBase , IMobileBase
    {
        #region Fields

        private HourMode hourMode=HourMode.Hours24;
        private string timeFormat = "hh:mm tt";
        private string defaultTime = DateTime.Now.ToString();
        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;

        #endregion

        #region Properties

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
        /// Gets or sets the render mode.
        /// </summary>
        /// <value>
        /// The render mode.
        /// </value>
        [JsonProperty("renderMode")]
        [DefaultValue(RenderMode.Auto)]
        public RenderMode RenderMode { get { return renderMode; } set { renderMode = value; } }

        /// <summary>
        /// Gets or sets the hour mode.
        /// </summary>
        /// <value>
        /// The hour mode.
        /// </value>
        [JsonProperty("hourMode")]
        [DefaultValue(HourMode.Hours24)]
        public HourMode HourMode { get { return hourMode; } set { hourMode = value; } }

        /// <summary>
        /// Gets or sets the time format.
        /// </summary>
        /// <value>
        /// The time format.
        /// </value>
        [JsonProperty("timeFormat")]
        [DefaultValue("hh:mm tt")]
        public string TimeFormat { get { return timeFormat; } set { timeFormat = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether [display default time].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [display default time]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("displayDefaultTime")]
        [DefaultValue(false)]
        public bool DisplayDefaultTime { get; set; }

        /// <summary>
        /// Gets or sets the default time.
        /// </summary>
        /// <value>
        /// The default time.
        /// </value>
        [JsonProperty("defaultDate")]
        public string DefaultTime { get { return defaultTime; } set { defaultTime = value; } }

        /// <summary>
        /// Gets or sets the on time selected.
        /// </summary>
        /// <value>
        /// The on time selected.
        /// </value>
        [JsonProperty("Select")]
        [DefaultValue("")]
        public string Select { get; set; }

        /// <summary>
        /// Gets or sets the on time picker load.
        /// </summary>
        /// <value>
        /// The on time picker load.
        /// </value>
        [JsonProperty("Load")]
        [DefaultValue("")]
        public string Load { get; set; }

        /// <summary>
        /// Gets or sets the io s7.
        /// </summary>
        /// <value>
        /// The io s7.
        /// </value>
        [JsonProperty("ios7")]
        public MobileTimePickerIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileTimePickerWindowsProperties Windows { get; set; }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileTimePickerPropertiesBuilder"/> class.
        /// </summary>
        public MobileTimePickerProperties()
        {
            this.IOS7 = new MobileTimePickerIOS7Properties();
            this.Windows = new MobileTimePickerWindowsProperties();
            return;
        }
        #endregion

    }
}
