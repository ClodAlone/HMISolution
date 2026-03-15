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

namespace Syncfusion.JavaScript.Mobile.Models
{
    /// <summary>
    /// Class for Auto Complete Property
    /// </summary>
    public class MobileAutoCompleteProperties : AutoCompletePropertiesBase , IMobileBase
    {
        #region Fields
        private bool showCheckbox = true;
        private string delimiter = ",";
        private string watermarkText = "Search";
        private string mapper = null;
        private double listSize = 5;
        private string field = "text";
        private string imageClass = null;
        private string imageField = null;
        private object dataSource = new object();
        private FilterMode filterMode = FilterMode.Server;
        private MobileSortOrder sortOrder = MobileSortOrder.Ascending;
        private MobileFilterType filterType = MobileFilterType.Contains;
        private Mode mode = Mode.Default;
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
        /// Gets or sets the filter mode.
        /// </summary>
        /// <value>
        /// The filter mode.
        /// </value>
        [JsonProperty("filterMode")]
        [DefaultValue(FilterMode.Server)]
        public FilterMode FilterMode { get { return filterMode; } set { filterMode=value;} }

        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        [JsonProperty("filterType")]
        [DefaultValue(MobileFilterType.Contains)]
        public MobileFilterType FilterType { get { return filterType; } set { filterType = value; } }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        [JsonProperty("mode")]
        [DefaultValue(Mode.Default)]
        public Mode Mode { get { return mode; } set { mode = value; } }


        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        [JsonProperty("sortOrder")]
        [DefaultValue(MobileSortOrder.Ascending)]
        public MobileSortOrder SortOrder { get { return sortOrder; } set { sortOrder = value; } }
        
        [JsonProperty("showCheckbox")]
        [DefaultValue(true)]
        public bool ShowCheckbox { get { return showCheckbox; } set { showCheckbox = value; } }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>
        /// The delimiter.
        /// </value>
        [JsonProperty("delimiter")]
        [DefaultValue(",")]
        public string Delimiter { get { return delimiter; } set { delimiter = value; } }

        /// <summary>
        /// Gets or sets the watermark text.
        /// </summary>
        /// <value>
        /// The watermark text.
        /// </value>
        [JsonProperty("watermarkText")]
        [DefaultValue("Search")]
        public string WatermarkText { get { return watermarkText; } set { watermarkText = value; } }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        [DefaultValue("")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        [JsonProperty("mapper")]
        [DefaultValue(null)]
        public string Mapper { get { return mapper; } set { mapper = value; } }

        /// <summary>
        /// Gets or sets the size of the list.
        /// </summary>
        /// <value>
        /// The size of the list.
        /// </value>
        [JsonProperty("listSize")]
        [DefaultValue(5)]
        public double ListSize { get { return listSize; } set { listSize = value; } }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [JsonProperty("field")]
        [DefaultValue("text")]
        public string Field { get { return field; } set { field = value; } }

        /// <summary>
        /// Gets or sets the image class.
        /// </summary>
        /// <value>
        /// The image class.
        /// </value>
        [JsonProperty("imageClass")]
        [DefaultValue(null)]
        public string ImageClass { get { return imageClass; } set { imageClass = value; } }

        /// <summary>
        /// Gets or sets the image field.
        /// </summary>
        /// <value>
        /// The image field.
        /// </value>
        [JsonProperty("imageField")]
        [DefaultValue(null)]
        public string ImageField { get { return imageField; } set { imageField = value; } }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        [JsonProperty("dataSource")]
        [DefaultValue(null)]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource { get { return dataSource; } set { dataSource = value; } }

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
        /// Gets or sets the key press.
        /// </summary>
        /// <value>
        /// The key press.
        /// </value>
        [JsonProperty("keyPress")]
        [DefaultValue(null)]
        public string KeyPress { get; set; }

        /// <summary>
        /// Gets or sets the select.
        /// </summary>
        /// <value>
        /// The select.
        /// </value>
        [JsonProperty("select")]
        [DefaultValue(null)]
        public string Select { get; set; }

        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>
        /// The change.
        /// </value>
        [JsonProperty("change")]
        [DefaultValue(null)]
        public string Change { get; set; }

        /// <summary>
        /// Gets or sets the focus in.
        /// </summary>
        /// <value>
        /// The focus in.
        /// </value>
        [JsonProperty("focusIn")]
        [DefaultValue(null)]
        public string FocusIn { get; set; }

        /// <summary>
        /// Gets or sets the focus out.
        /// </summary>
        /// <value>
        /// The focus out.
        /// </value>
        [JsonProperty("focusOut")]
        [DefaultValue(null)]
        public string FocusOut { get; set; }

        /// <summary>
        /// Gets or sets the windows.
        /// </summary>
        /// <value>
        /// The windows.
        /// </value>
        [JsonProperty("windows")]
        public MobileAutoCompleteWindowsProperties Windows { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MobileAutoCompletePropertiesBuilder"/> class.
        /// </summary>
        public MobileAutoCompleteProperties()
        {
            this.Windows = new MobileAutoCompleteWindowsProperties();
            return;
        }
        #endregion

    }
}
