//-------------------------------------------------------------------------------------------------
// <copyright file="TablixRowHierarchy.cs" company="syncfusion">
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
using System.Xml.Linq;
using Syncfusion.RDL.DOM;

namespace Syncfusion.Windows.Reports.Designer.Controls
{

    internal class TablixRowHierarchy   
    {
        public Syncfusion.RDL.DOM.TablixRowHierarchy TablixRowHierarchyBase;

        public object Parent;

        public TablixRowHierarchy()
        {
            this.TablixRowHierarchyBase = new Syncfusion.RDL.DOM.TablixRowHierarchy();
        }
    }
}
