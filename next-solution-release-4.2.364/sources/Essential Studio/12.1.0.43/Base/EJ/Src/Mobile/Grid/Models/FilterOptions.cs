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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript.Mobile.Models
{
    public class MobileFilterOptions<T> : FilterOptionsBase<T> where T : class
    {
        #region private fields

        private int interval = 1500;

        #endregion private fields

        #region properties

        [JsonProperty("interval")]
        [DefaultValue(1500)]
        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }

        #endregion properties
    }
}
