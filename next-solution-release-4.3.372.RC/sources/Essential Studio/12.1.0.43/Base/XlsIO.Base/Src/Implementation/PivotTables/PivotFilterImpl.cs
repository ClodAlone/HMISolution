#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotFilterImpl : IPivotFilter
    {
        #region Members
        private string str_value;
        #endregion
        #region Property
        public string Value1
        {
            get
            {
                return str_value;
            }
            set
            {
                str_value = value;
            }
        }
        #endregion

    }
}
