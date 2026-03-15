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
    public class TablixColumns : List<TablixColumn>
    {

    }

    public class TablixColumn
    {
        public Size Width { get; set; }

        public object Clone()
        {
            TablixColumn tablixColumn = new TablixColumn();
            tablixColumn.Width = (Size)this.Width.Clone();
            return tablixColumn;
        }
    }
}
