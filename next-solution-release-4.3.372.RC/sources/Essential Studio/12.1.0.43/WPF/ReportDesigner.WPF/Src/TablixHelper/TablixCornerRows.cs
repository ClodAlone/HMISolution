//-------------------------------------------------------------------------------------------------
// <copyright file="TablixCornerRows.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;

namespace Syncfusion.Windows.Reports.Designer.Controls
{   
    internal class TablixCornerRows        
    {
        //public Syncfusion.RDL.DOM.TablixCornerRows TablixCornerRowsBase;
        //public List<Syncfusion.RDL.DOM.TablixCornerRow> TablixCornerRowsBase;
        //public List<List<Syncfusion.RDL.DOM.TablixCornerCell>> TablixCornerRowsBase;

        public List<Syncfusion.RDL.DOM.TablixCornerRow> TablixCornerRowsBase;

        public object Parent;

        public TablixCornerRows()
        {
            //this.TablixCornerRowsBase = new Syncfusion.RDL.DOM.TablixCornerRows();

            this.TablixCornerRowsBase = new List<Syncfusion.RDL.DOM.TablixCornerRow>();


            //this.TablixCornerRowsBase = new List<Syncfusion.RDL.DOM.TablixCornerRow>();
        }
    }
}
