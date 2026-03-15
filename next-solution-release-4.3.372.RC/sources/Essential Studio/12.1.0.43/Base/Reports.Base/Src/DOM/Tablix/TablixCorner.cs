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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class TablixCorner
    {
        public TablixCornerRows TablixCornerRows { get; set; }

        public object Clone()
        {
            TablixCorner tablixCorner = new TablixCorner();
            tablixCorner.TablixCornerRows = new TablixCornerRows();
            tablixCorner.TablixCornerRows.AddRange(this.TablixCornerRows.Clone());
            return tablixCorner;
        }
    }

    public class TablixCornerRows:List<TablixCornerRow>
    {
    }


    public class TablixCornerRow
    {
        [XmlElement("TablixCornerCell", typeof(TablixCornerCell))]
        public TablixCornerCells TablixCornerCells { get; set; }

        public object Clone()
        {
            TablixCornerRow tablixCornerRow = new TablixCornerRow();
            tablixCornerRow.TablixCornerCells = new TablixCornerCells();
            tablixCornerRow.TablixCornerCells.AddRange(this.TablixCornerCells.Clone());
            return tablixCornerRow;
        }
    }
    
    public class TablixCornerCells: List<TablixCornerCell>
    {
    }

    public class TablixCornerCell
    {
        public CellContents CellContents { get; set; }

        public object Clone()
        {
            TablixCornerCell tablixCornerCell = new TablixCornerCell();
            if (this.CellContents != null)
            {
                tablixCornerCell.CellContents = (CellContents)this.CellContents.Clone();
            }
            return tablixCornerCell;
        }
    }
}
