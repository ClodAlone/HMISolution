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

namespace Syncfusion.Olap.MDXQueryParser
{
    public class DrillDownMember : IKeyword
    {
        public string Name { get; set; }

        //public string FunctionName { get; set; }

        public DrillDownMember ChildDrillDownMember { get; set; }

        public DrillDownLevel DrillDownLevel { get; set; }

        public IMember MemberName { get; set; }

        #region IKeyword Members


        public bool Validate()
        {
            if (this.Name == string.Empty)
            {
                return false;
            }
            else if (this.Name != KeywordConstants.DrillDownMember)
            {
                return false;
            }
            if (this.ChildDrillDownMember != null && this.DrillDownLevel != null)
            {
                return (this.ChildDrillDownMember.Validate() && this.DrillDownLevel.Validate());
            }
            else if (this.DrillDownLevel != null)
            {
                return (this.DrillDownLevel.Validate());
            }
            else if (this.Name == KeywordConstants.DrillDownMember)
            {
                return (this.MemberName.Validate());
            }
            else return false;
        }

        #endregion
    }
}
