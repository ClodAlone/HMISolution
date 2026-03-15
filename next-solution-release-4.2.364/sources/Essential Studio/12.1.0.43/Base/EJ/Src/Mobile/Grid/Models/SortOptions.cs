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
namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileSortOptions<T> where T : class
    {
        private List<MobileSortedColumn<T>> sortedColumn = new List<MobileSortedColumn<T>>();

        [JsonProperty("sortedColumns")]
        public List<MobileSortedColumn<T>> SortedColumn
        {
            get { return this.sortedColumn; }
            set {  this.sortedColumn = value; }
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
