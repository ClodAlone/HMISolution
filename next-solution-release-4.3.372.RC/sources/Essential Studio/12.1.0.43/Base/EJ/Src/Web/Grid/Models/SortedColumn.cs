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
using Syncfusion.JavaScript.Shared;



namespace Syncfusion.JavaScript.Models
{
    public class SortedColumn<T> where T : class
    {
        private String field = null;
        private Direction direction =(Direction)SortOrder.Ascending;

        [JsonProperty("field")]
        [DefaultValue(null)]
        public String Field
        {
            get { return this.field; }
            set { this.field = value; }
        }
        [JsonProperty("direction")]
        [DefaultValue(SortOrder.Ascending)]
        public Direction Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }
    }
}
