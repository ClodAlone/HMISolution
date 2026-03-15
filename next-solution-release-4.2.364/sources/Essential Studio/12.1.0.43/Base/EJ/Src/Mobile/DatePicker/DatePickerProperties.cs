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
    /// Class for DatePicker Properties
    /// </summary>
    public class MobileDatePickerProperties : DatePickerPropertiesBase , IMobileBase
    {
        #region Fields

        private RenderMode renderMode = RenderMode.Auto;
        private Theme theme = Theme.Auto;
        private string dateFormat = "";
        private string minDate = "1/1/2000";
        private string maxDate = "12/31/2020";
        private string defaultDate = DateTime.Now.ToString();

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
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        [JsonProperty("dateFormat")]
        [DefaultValue("MM/dd/yyyy")]
        public string DateFormat { get { return dateFormat; } set { dateFormat = value; } }

        /// <summary>
        /// Gets or sets the minimum date.
        /// </summary>
        /// <value>
        /// The minimum date.
        /// </value>
        [JsonProperty("minDate")]
        [DefaultValue("1/1/2000")]
        public string MinDate { get { return minDate; } set { minDate = value; } }

        /// <summary>
        /// Gets or sets the maximum date.
        /// </summary>
        /// <value>
        /// The maximum date.
        /// </value>
        [JsonProperty("maxDate")]
        [DefaultValue("12/31/2020")]
        public string MaxDate { get { return maxDate; } set { maxDate = value; } }

        /// <summary>
        /// Gets or sets the default date.
        /// </summary>
        /// <value>
        /// The default date.
        /// </value>
        [JsonProperty("defaultDate")]
        public string DefaultDate { get { return defaultDate; } set { defaultDate = value; } }

        /// <summary>
        /// Gets or sets the on date selected.
        /// </summary>
        /// <value>
        /// The on date selected.
        /// </value>
        [JsonProperty("Select")]
        [DefaultValue("")]
        public string Select { get; set; }

        /// <summary>
        /// Gets or sets the on date picker load.
        /// </summary>
        /// <value>
        /// The on date picker load.
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
        public MobileDatePickerIOS7Properties IOS7 { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileDatePickerWindowsProperties Windows { get; set; }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileDatePickerPropertiesBuilder"/> class.
        /// </summary>
        public MobileDatePickerProperties()
        {
            this.IOS7 = new MobileDatePickerIOS7Properties();
            this.Windows = new MobileDatePickerWindowsProperties();
            return;
        }
        #endregion

    }
}
