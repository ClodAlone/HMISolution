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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class TablixCells:List<TablixCell>
    {

    }
    public class TablixCell
    {
        public CellContents CellContents { get; set; }
        public string DataElementName { get; set; }

        [DefaultValue(DataElementOutputs.Auto)]
        public DataElementOutputs DataElementOutput { get; set; }

        public object Clone()
        {
            TablixCell tablixCell = new TablixCell();
            if (this.CellContents != null)
            {
                tablixCell.CellContents = (CellContents)this.CellContents.Clone();
            }
            tablixCell.DataElementName = this.DataElementName;
            tablixCell.DataElementOutput = this.DataElementOutput;
            return tablixCell;
        }
    }
}
