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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Runtime.Serialization;
using Syncfusion.JavaScript.Mobile;
using Syncfusion.JavaScript.Mobile.Models;


namespace Syncfusion.JavaScript.Mobile
{
    public class MobileAccordionProperties : AccordionPropertiesBase, IMobileBase
    {
        #region Fields
        /// <summary>
        /// The theme
        /// </summary>
        private Theme theme=Theme.Auto;
        /// <summary>
        /// The selected item index
        /// </summary>
        private int[] selectedItemIndex= new int[] { };
        /// <summary>
        /// The show header icon
        /// </summary>
        private bool showHeaderIcon;
        /// <summary>
        /// The cache
        /// </summary>
        private bool cache;
        /// <summary>
        /// The active
        /// </summary>
        private string active;
        /// <summary>
        /// The before active
        /// </summary>
        private string beforeActive;
        /// <summary>
        /// The ajax load
        /// </summary>
        private string ajaxLoad;
        /// <summary>
        /// The ajax before load
        /// </summary>
        private string ajaxBeforeLoad;
        /// <summary>
        /// The ajax success
        /// </summary>
        private string ajaxSuccess;
        /// <summary>
        /// The ajax error
        /// </summary>
        private string ajaxError;
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
        [DefaultValue(Theme.Auto)]
        public Theme Theme { get { return theme; } set { theme = value; } }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        /// <value>
        /// The index of the selected item.
        /// </value>
        [JsonProperty("selectedItemIndex")]
        [DefaultValue(new int[] {0})]
        public int[] SelectedItemIndex { get { return selectedItemIndex; } set { selectedItemIndex=value;} }

        /// <summary>
        /// Gets or sets a value indicating whether [show header icon].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show header icon]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("showHeaderIcon")]
        [DefaultValue(false)]
        public bool ShowHeaderIcon { get { return showHeaderIcon ;} set { showHeaderIcon = value ;} }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MobileAccordionProperties"/> is cache.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cache; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("cache")]
        [DefaultValue(true)]
        public bool Cache { get { return cache ;} set { cache=value ;} }

        /// <summary>
        /// Gets or sets the active.
        /// </summary>
        /// <value>
        /// The active.
        /// </value>
        [JsonProperty("active")]
        [DefaultValue("undefined")]
        public string Active { get { return active; } set { active = value;} }

        /// <summary>
        /// Gets or sets the before active.
        /// </summary>
        /// <value>
        /// The before active.
        /// </value>
        [JsonProperty("beforeActive")]
        [DefaultValue("undefined")]
        public string BeforeActive { get { return beforeActive; } set { beforeActive = value;} }

        /// <summary>
        /// Gets or sets the ajax load.
        /// </summary>
        /// <value>
        /// The ajax load.
        /// </value>
        [JsonProperty("ajaxLoad")]
        [DefaultValue("undefined")]
        public string AjaxLoad { get { return ajaxLoad; } set { ajaxLoad = value;} }

        /// <summary>
        /// Gets or sets the ajax before load.
        /// </summary>
        /// <value>
        /// The ajax before load.
        /// </value>
        [JsonProperty("ajaxBeforeLoad")]
        [DefaultValue("undefined")]
        public string AjaxBeforeLoad { get { return ajaxBeforeLoad; } set { ajaxBeforeLoad = value;} }

        /// <summary>
        /// Gets or sets the ajax success.
        /// </summary>
        /// <value>
        /// The ajax success.
        /// </value>
        [JsonProperty("ajaxSuccess")]
        [DefaultValue("undefined")]
        public string AjaxSuccess { get { return ajaxSuccess; } set { ajaxSuccess = value;} }

        /// <summary>
        /// Gets or sets the ajax error.
        /// </summary>
        /// <value>
        /// The ajax error.
        /// </value>
        [JsonProperty("ajaxError")]
        [DefaultValue("undefined")]
        public string AjaxError { get { return ajaxError; } set { ajaxError = value;} }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileAccordionWindowsProperties Windows { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonIgnore]
        public List<MobileAccordionBaseItem> Items
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAccordionProperties"/> class.
        /// </summary>
        public MobileAccordionProperties()
        {
            this.Items = new List<MobileAccordionBaseItem>();
        }
        #endregion
    }
}
