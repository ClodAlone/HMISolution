#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.TreeMap
{
    internal class GroupValueInfo
    {
        public List<object> DataSources { get; set; }

        public SummaryBase Value { get; set; }

        public SummaryBase ColorValue { get; set; }

        public string GroupName { get; set; }

        public GroupValueInfo()
        {
            DataSources = new List<object>();
        }
    }

    internal class TreeMapValueField
    {
        public string Name { get; set; }

        public string FieldName { get; set; }

        internal SummaryBase GetSummaryInstance()
        {
            SummaryBase sb = new DoubleTotalSummary();
            return sb;
        }

        #region ICloneable Members

        public object Clone()
        {
            var field = new TreeMapValueField {Name = Name, FieldName = FieldName};
            return field;
        }

        #endregion
    }
}
