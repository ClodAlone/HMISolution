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
    public class GroupOptions<T> where T : class
    {
        private bool toggleGroup = false;
        private bool groupedColumnShow = true;
        private bool showUngroupButton = true;
        private bool showAnimateButton = true;
        private List<String> groupedColumn = new List<String>();
        [JsonProperty("toggleGrouping")]
        [DefaultValue(false)]
        public bool ToggleGroup
        {
            get { return this.toggleGroup; }
            set { this.toggleGroup = value; }
        }
        [JsonProperty("groupedColumnShow")]
        [DefaultValue(true)]
        public bool GroupedColumnShow
        {
            get { return this.groupedColumnShow; }
            set { this.groupedColumnShow = value; }
        }
        [JsonProperty("showUngroupButton")]
        [DefaultValue(true)]
        public bool ShowUngroupButton
        {
            get { return this.showUngroupButton; }
            set { this.showUngroupButton = value; }
        }
        [JsonProperty("showAnimateButton")]
        [DefaultValue(true)]
        public bool ShowAnimateButton
        {
            get { return this.showAnimateButton; }
            set { this.showAnimateButton = value; }
        }
        [JsonProperty("groupedColumns")]
       
        public List<String> GroupedColumn
        {
            get { return this.groupedColumn; }
            set { this.groupedColumn = value; }
        }



        #region ShouldSerialize Methods

        public bool ShouldSerializeGroupedColumn()
        {
            if (GroupedColumn.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
