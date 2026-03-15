//-------------------------------------------------------------------------------------------------
// <copyright file="TablixColumn.cs" company="syncfusion">
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
    
    internal class TablixColumn
    {
        public Syncfusion.RDL.DOM.TablixColumn TablixColumnBase;

        public object Parent;

        public TablixColumn()
        {
            this.TablixColumnBase = new Syncfusion.RDL.DOM.TablixColumn();
        } 
    }
}
