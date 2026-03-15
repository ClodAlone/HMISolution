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
using Syncfusion.JavaScript.Models;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Models
{

    public class StripLinesOptions
    {
        private List<StripLines> stripLines = new List<StripLines>();

        [JsonProperty("stripLines")]
        public List<StripLines> StripLines
        {
            get { return this.stripLines; }
            set { this.stripLines = value; }
        }

        #region ShouldSerialize Methods

        public bool ShouldSerializeStripLines()
        {

            if (Utils.PropertyCompare(StripLines, new StripLines()))
                return true;
            else
                return false;
        
        }
        #endregion
    }
}