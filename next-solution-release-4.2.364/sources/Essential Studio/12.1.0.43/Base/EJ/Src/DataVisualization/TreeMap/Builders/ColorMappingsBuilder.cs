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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class TreeMapRangeColorMappingBuilder
    {
        public TreeMapRangeColorMapping treeMapColorMapping;

        public TreeMapProperties mapProperties;

        public TreeMapRangeColorMappingBuilder(TreeMapRangeColorMapping colorMapping,TreeMapProperties treeMapProperties)
        {
            this.treeMapColorMapping = colorMapping;
            this.mapProperties = treeMapProperties;
        }

        public void Add()
        {

            if (this.mapProperties.TreeMapRangeColorMappings == null)
            {
                this.mapProperties.TreeMapRangeColorMappings = new List<TreeMapRangeColorMapping>();
            }
            this.mapProperties.TreeMapRangeColorMappings.Add(treeMapColorMapping);
            this.treeMapColorMapping = new TreeMapRangeColorMapping();
        }

        public TreeMapRangeColorMappingBuilder From(double value)
        {
            this.treeMapColorMapping.From = value;
            return this;
        }

        public TreeMapRangeColorMappingBuilder To(double value)
        {
            this.treeMapColorMapping.To = value;
            return this;
        }

        public TreeMapRangeColorMappingBuilder Color(string color)
        {
            this.treeMapColorMapping.Color = color;
            return this;
        }

        public TreeMapRangeColorMappingBuilder Legendlabel(string legendLabel)
        {
            this.treeMapColorMapping.Legendlabel = legendLabel;
            return this;
        }
    }

    public class TreeMapDesaturationColorMappingBuilder
    {
        public TreeMapDesaturationColorMapping treeMapDesaturationColorMapping;

        public TreeMapProperties mapProperties;

        public TreeMapDesaturationColorMappingBuilder(TreeMapDesaturationColorMapping colorMapping, TreeMapProperties treeMapProperties)
        {
            this.treeMapDesaturationColorMapping = colorMapping;
            this.mapProperties = treeMapProperties;
            this.mapProperties.TreeMapDesaturationColorMapping = this.treeMapDesaturationColorMapping;
        }
       
        public TreeMapDesaturationColorMappingBuilder From(double value)
        {
            this.treeMapDesaturationColorMapping.From = value;
            return this;
        }

        public TreeMapDesaturationColorMappingBuilder To(double value)
        {
            this.treeMapDesaturationColorMapping.To = value;
            return this;
        }

        public TreeMapDesaturationColorMappingBuilder Color(string color)
        {
            this.treeMapDesaturationColorMapping.Color = color;
            return this;
        }

        public TreeMapDesaturationColorMappingBuilder RangeMinimum(double rangeMinimum)
        {
            this.treeMapDesaturationColorMapping.RangeMinimum = rangeMinimum;
            return this;
        }

        public TreeMapDesaturationColorMappingBuilder RangeMaximum(double rangeMaximum)
        {
            this.treeMapDesaturationColorMapping.RangeMaximum = rangeMaximum;
            return this;
        }
    }

    public class TreeMapUniColorMappingBuilder
    {
        public TreeMapUniColorMapping treeMapUniColorMapping;

        public TreeMapProperties mapProperties;

        public TreeMapUniColorMappingBuilder(TreeMapUniColorMapping colorMapping, TreeMapProperties treeMapProperties)
        {
            this.treeMapUniColorMapping = colorMapping;
            this.mapProperties = treeMapProperties;
            this.mapProperties.TreeMapUniColorMapping = this.treeMapUniColorMapping;
        }
        public TreeMapUniColorMappingBuilder Color(string color)
        {
            this.treeMapUniColorMapping.Color = color;
            return this;
        }

    }

    public class TreeMapPaletteColorMappingBuilder
    {
        public TreeMapPaletteColorMapping treeMapPaletteColorMapping;

        public TreeMapProperties mapProperties;

        public TreeMapPaletteColorMappingBuilder(TreeMapPaletteColorMapping colorMapping, TreeMapProperties treeMapProperties)
        {
            this.treeMapPaletteColorMapping = colorMapping;
            this.mapProperties = treeMapProperties;
            this.mapProperties.TreeMapPaletteColorMapping = this.treeMapPaletteColorMapping;
        }

        public TreeMapPaletteColorMappingBuilder Color(string color)
        {
            this.treeMapPaletteColorMapping.Color = color;
            return this;
        }

        public TreeMapPaletteColorMappingBuilder Colors(List<string> colors)
        {
            this.treeMapPaletteColorMapping.Colors = colors;
            return this;
        }

    }

}
