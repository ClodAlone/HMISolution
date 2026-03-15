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
    public class FilterOptionsBase<T> where T:class
    {
        #region private fields

        private FilterBarMode filterMode =FilterBarMode.Immediate;

        #endregion private fields

        #region properties

        [JsonProperty("filterBarMode")]
        [DefaultValue(FilterBarMode.Immediate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterBarMode FilterBarMode
        {
            get { return this.filterMode; }
            set { this.filterMode = value; }
        }

        #endregion properties
    }
}
