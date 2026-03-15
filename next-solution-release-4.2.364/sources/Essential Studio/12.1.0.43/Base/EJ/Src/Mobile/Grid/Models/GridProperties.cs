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
using Syncfusion.JavaScript.Shared;
using Syncfusion.JavaScript.Models;


namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileGridProperties<T> : GridPropertiesBase<T>, IMobileBase where T : class
    {
        #region Fields

        private Theme theme = Theme.Light;
        private bool showCaption = false;
        private bool showColumnSelector = false;
        private string caption = String.Empty;
        private List<MobileColumn<T>> columns = new List<MobileColumn<T>>();
        private MobilePageOptions<T> pageOption = new MobilePageOptions<T>();
        private MobileFilterOptions<T> filterOption = new MobileFilterOptions<T>();
        private MobileSortOptions<T> sortOption = new MobileSortOptions<T>();
        private MobileScrollOptions<T> scrollOption = new MobileScrollOptions<T>();

        #endregion
        public MobileGridProperties() { }
        

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

        [JsonProperty("showCaption")]
        [DefaultValue(false)]
        public bool ShowCaption
        {
            get { return this.showCaption; }
            set { this.showCaption = value; }
        }
        [JsonProperty("showColumnSelector")]
        [DefaultValue(false)]
        public bool ShowColumnSelector
        {
            get { return this.showColumnSelector; }
            set { this.showColumnSelector = value; }
        }
        [JsonProperty("caption")]
        [DefaultValue("")]
        public string Caption
        {
            get { return this.caption; }
            set { this.caption = value; }
        }
       
        [JsonProperty("columns")]
        public List<MobileColumn<T>> Columns
        {
            get { return this.columns; }
            set { this.columns = value; }
        }
        [JsonProperty("pageSettings")]

        public MobilePageOptions<T> PageOption
        {
            get { return this.pageOption; }
            set { this.pageOption = value; }
        }

        [JsonProperty("filterSettings")]
        public MobileFilterOptions<T> FilterOption
        {
            get { return this.filterOption; }
            set { this.filterOption = value; }
        }
        [JsonProperty("sortSettings")]

        public MobileSortOptions<T> SortOption
        {
            get { return this.sortOption; }
            set { this.sortOption = new MobileSortOptions<T>(); this.sortOption = value; }
        }

        [JsonProperty("scrollSettings")]
        public MobileScrollOptions<T> ScrollOption
        {
            get { return this.scrollOption; }
            set { this.scrollOption = value; }
        }
       
        #endregion


        #region ShouldSerialize Methods

        public bool ShouldSerializePageOption()
        {
            if (Utils.PropertyCompare(PageOption, new MobilePageOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeScrollOption()
        {
            if (Utils.PropertyCompare(ScrollOption, new MobileScrollOptions<T>()))
                return true;
            else
                return false;
        }

        public bool ShouldSerializeFilterOption()
        {

            if (Utils.PropertyCompare(FilterOption, new MobileFilterOptions<T>()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeSortOption()
        {
            if (Utils.PropertyCompare(SortOption, new MobileSortOptions<T>()))
                return true;
            else
                return false;
        }
       
        public bool ShouldSerializeColumns()
        {
            if (Columns.Count != 0)
                return true;
            else
                return false;
        }

       
        #endregion


    }
}
