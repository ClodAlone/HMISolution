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
    public class DrillDownLevel : IKeyword
    {
        public string Name { get; set; }

        public IMember MemberName { get; set; }

        #region IKeyword Members

        public bool Validate()
        {
            if (this.Name == KeywordConstants.DrillDownLevel && ValidateMember(this.MemberName))
            {
                return true;
            }
            return false;
        }

        private bool ValidateMember(IMember memberName)
        {
            if (memberName.UniqueName.ToUpper().Contains(KeywordConstants.Measures))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
