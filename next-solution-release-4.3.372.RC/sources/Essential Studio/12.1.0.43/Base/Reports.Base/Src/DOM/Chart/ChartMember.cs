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
    public class ChartMembers : List<ChartMember>
    {

    }
    public class ChartMember
    {
        public Group Group { get; set; }
        public SortExpressions SortExpressions { get; set; }
        public ChartMembers ChartMembers { get; set; }
        public string Label { get; set; }
        public CustomProperties CustomProperties { get; set; }
        public string DataElementName { get; set; }
        public DataElementOutputs DataElementOutput { get; set; }

        public bool ShouldSerializeCustomProperties()
        {
            return CustomProperties != null && CustomProperties.Count > 0;
        }

        public void ResetCustomProperties()
        {
            this.CustomProperties = new CustomProperties();
        }

        public bool ShouldSerializeDataElementOutput()
        {
            return DataElementOutput != DataElementOutputs.Auto;
        }

        public void ResetDataElementOutput()
        {
            this.DataElementOutput = DataElementOutputs.Auto;
        }
    }
}
