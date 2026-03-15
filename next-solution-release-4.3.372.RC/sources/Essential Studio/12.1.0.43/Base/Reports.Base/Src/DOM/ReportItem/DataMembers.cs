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

namespace Syncfusion.RDL.DOM
{
    public class DataMembers : List<DataMember>
    {

    }

    public class DataMember
    {
        public Group Group { get; set; }
        public SortExpressions SortExpressions { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public DataMembers DataMembers { get; set; }
        public bool Subtotal { get; set; }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }
    }
}
