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
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Builders
{
    public class TreeMapLegendBuilder
    {
        public TreeMapLegend treeMapLegend;

        public TreeMapProperties mapProperties;

        public TreeMapLegendBuilder(TreeMapLegend treeMapLegend, TreeMapProperties treeMapProperties)
        {
            this.treeMapLegend = treeMapLegend;
            this.mapProperties = treeMapProperties;
            this.mapProperties.TreeMapLegend = this.treeMapLegend;
        }

        public TreeMapLegendBuilder LegendTemplate(string legendTemplate)
        {
            treeMapLegend.LegendTemplate = legendTemplate;
            return this;
        }

        public TreeMapLegendBuilder LegendIconHeight(double legendIconHeight)
        {
            treeMapLegend.LegendIconHeight = legendIconHeight;
            return this;
        }

        public TreeMapLegendBuilder LegendIconWidth(double legendIconWidth)
        {
            treeMapLegend.LegendIconWidth = legendIconWidth;
            return this;
        }

    }
}
