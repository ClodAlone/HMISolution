#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;



namespace Syncfusion.JavaScript.Models
{
    public class FilterOptions<T> where T:class
    {
        private FilterType filterType = FilterType.FilterBar;
        
        private FilterBarMode filterMode =FilterBarMode.Immediate;
        private int statusBarWidth = 450;
        private bool showPredicate = false;
        private bool showFilterbarMessage = true;
    //Properties
        [JsonProperty("filterType")]
        [DefaultValue(FilterType.FilterBar)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterType FilterType
        { 
            get { 
                return this.filterType; }
            set { this.filterType = value; }
        }
        [JsonProperty("filterBarMode")]
        [DefaultValue(FilterBarMode.Immediate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterBarMode FilterBarMode
        {
            get { return this.filterMode; }
            set { this.filterMode = value; }
        }
        [JsonProperty("statusBarWidth")]
        [DefaultValue(450)]
        public int StatusBarWidth
        {
            get { return this.statusBarWidth; }
            set { this.statusBarWidth = value; }
        }
        [JsonProperty("showPredicate")]
        [DefaultValue(false)]
        public bool ShowPredicate
        {
            get { return this.showPredicate; }
            set { this.showPredicate=value; }
        }
        [JsonProperty("showFilterBarMessage")]
        [DefaultValue(true)]
        public bool ShowFilterBarMessage
        {
            get { return this.showFilterbarMessage; }
            set { this.showFilterbarMessage = value; }
        }
    }
}
