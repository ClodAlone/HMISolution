#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.DataVisualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public TreeMapPropertiesBuilder TreeMap(string id)
        {
            var model = new TreeMapProperties();
            var treemap = new TreeMap(id, model);
            return new TreeMapPropertiesBuilder(treemap);
        }

        public TreeMapPropertiesBuilder TreeMap(String id, double width, double height)
        {
            var model = new TreeMapProperties();
            var treemap = new TreeMap(id, model);
            treemap.Width = width;
            treemap.Height = height;
            return new TreeMapPropertiesBuilder(treemap);
        }

        public TreeMap TreeMap(String id, TreeMapProperties model, double width, double height)
        {
            var treemap = new TreeMap(id, model);
            treemap.Width = width;
            treemap.Height = height;
            return treemap;
        }

        public TreeMap TreeMap(String id, TreeMapProperties model)
        {
            var treemap = new TreeMap(id, model);
            return treemap;
        }
    }
}

