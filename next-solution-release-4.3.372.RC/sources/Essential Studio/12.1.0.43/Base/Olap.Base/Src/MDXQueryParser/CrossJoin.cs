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
    public class CrossJoin
    {
        public string FunctionName { get; set; }
        public IMember member { get; set; }
        public Tuple tuple { get; set; }
        
        public bool Validate()
        {
            if (this.tuple != null)
            {
                return this.tuple.Validate();
            }
            if (this.member != null)
            {
                return this.member.Validate();
            }
            else
            {
                return true;
            }
        }
    }
}
