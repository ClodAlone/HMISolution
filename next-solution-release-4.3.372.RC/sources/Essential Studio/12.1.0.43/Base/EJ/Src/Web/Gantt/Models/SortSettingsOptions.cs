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

    public class SortSettingsOptions
    {
        private List<GanttSortedColumn> sortedColumn = new List<GanttSortedColumn>();

        [JsonProperty("sortedColumns")]
        public List<GanttSortedColumn> SortedColumn
        {
            get { return this.sortedColumn; }
            set { this.sortedColumn = value; }
        }

        #region ShouldSerialize Methods

        public bool ShouldSerializeSortedColumn()
        {
            if (SortedColumn.Count != 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}