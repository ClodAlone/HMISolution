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
    public class TablixRows:List<TablixRow>
    {
    }

    public class TablixRow
    {
        public Size Height { get; set; }
        public TablixCells TablixCells { get; set; }

        public object Clone()
        {
            TablixRow tablixRow = new TablixRow();
            tablixRow.Height = (Size)this.Height.Clone();
            tablixRow.TablixCells = new TablixCells();
            tablixRow.TablixCells.AddRange(this.TablixCells.Clone());
            return tablixRow;
        }
    }
}
