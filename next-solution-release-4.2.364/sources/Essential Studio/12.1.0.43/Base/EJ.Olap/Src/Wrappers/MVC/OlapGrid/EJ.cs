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
using Syncfusion.JavaScript.Olap;
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public static partial class EJExtension
    {
        public static OlapGridPropertiesBuilder OlapGrid(this OlapControls ej, string id)
        {
            var model = new OlapGridProperties();
            var olapGrid = new OlapGrid(id, model);
            return new OlapGridPropertiesBuilder(olapGrid);
        }
        public static OlapGrid OlapGrid(this OlapControls ej, string id, OlapGridProperties model)
        {
            var olapGrid = new OlapGrid(id, model);
            return olapGrid;
        }
    }
}
