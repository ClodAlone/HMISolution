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
using System.Web;
using Syncfusion.JavaScript.DataVisualization.Builders;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class TreeMapPropertiesBuilder
    {
        public TreeMap treemap;

        public TreeMapPropertiesBuilder(TreeMap treemap)
        {
            this.treemap = new TreeMap(treemap.ID, treemap.TreeMapModel);
        }

        public TreeMapPropertiesBuilder()
        {
        }

        public TreeMapPropertiesBuilder DataSource(object itemSource)
        {
            treemap.TreeMapModel.DataSource = itemSource;
            return this;
        }

        public TreeMapPropertiesBuilder ColorValuePath(string colorValuePath)
        {
            treemap.TreeMapModel.ColorValuePath = colorValuePath;
            return this;
        }

        public TreeMapPropertiesBuilder WeightValuePath(string weightValuePath)
        {
            treemap.TreeMapModel.WeightValuePath = weightValuePath;
            return this;
        }

        public TreeMapPropertiesBuilder ShowLegend(bool showLegend)
        {
            treemap.TreeMapModel.ShowLegend = showLegend;
            return this;
        }

        public TreeMapPropertiesBuilder BorderBrush(string borderBrush)
        {
            treemap.TreeMapModel.BorderBrush = borderBrush;
            return this;
        }

        public TreeMapPropertiesBuilder BorderThickness(double borderThickness)
        {
            treemap.TreeMapModel.BorderThickness = borderThickness;
            return this;
        }

        public TreeMapPropertiesBuilder ItemsLayoutMode(string itemsLayoutMode)
        {
            treemap.TreeMapModel.ItemsLayoutMode = itemsLayoutMode;
            return this;
        }

        public TreeMapPropertiesBuilder Levels(Action<TreeMapLevelBuilder> levels)
        {
            var obj = new TreeMapLevel();
            var builder = new TreeMapLevelBuilder(obj, treemap.TreeMapModel);
            if (levels != null)
                levels.Invoke(builder);
            return this;
        }


        public TreeMapPropertiesBuilder TreeMapRangeColorMappings(Action<TreeMapRangeColorMappingBuilder> rangecolormapping)
        {
            var obj = new TreeMapRangeColorMapping();
            var builder = new TreeMapRangeColorMappingBuilder(obj, treemap.TreeMapModel);
            if (rangecolormapping != null)
                rangecolormapping.Invoke(builder);
            return this;
        }

        public TreeMapPropertiesBuilder TreeMapLegend(Action<TreeMapLegendBuilder> treeMapLegend)
        {
            var obj = new TreeMapLegend();
            var builder = new TreeMapLegendBuilder(obj, treemap.TreeMapModel);
            if (treeMapLegend != null)
                treeMapLegend.Invoke(builder);
            return this;
        }

        public TreeMapPropertiesBuilder TreeMapDesaturationColorMapping(Action<TreeMapDesaturationColorMappingBuilder> desaturationcolormapping)
        {
            var obj = new TreeMapDesaturationColorMapping();
            var builder = new TreeMapDesaturationColorMappingBuilder(obj, treemap.TreeMapModel);
            if (desaturationcolormapping != null)
                desaturationcolormapping.Invoke(builder);
            return this;
        }

        public TreeMapPropertiesBuilder TreeMapUniColorMapping(Action<TreeMapUniColorMappingBuilder> unicolormapping)
        {
            var obj = new TreeMapUniColorMapping();
            var builder = new TreeMapUniColorMappingBuilder(obj, treemap.TreeMapModel);
            if (unicolormapping != null)
                unicolormapping.Invoke(builder);
            return this;
        }

        public TreeMapPropertiesBuilder TreeMapPaletteColorMappingMapping(Action<TreeMapPaletteColorMappingBuilder> palettecolormapping)
        {
            var obj = new TreeMapPaletteColorMapping();
            var builder = new TreeMapPaletteColorMappingBuilder(obj, treemap.TreeMapModel);
            if (palettecolormapping != null)
                palettecolormapping.Invoke(builder);
            return this;
        }

        public TreeMapPropertiesBuilder HighlightOnSelection(bool highlightOnSelection)
        {
            treemap.TreeMapModel.HighlightOnSelection = highlightOnSelection;
            return this;
        }

        public TreeMapPropertiesBuilder ShowTooltip(bool showTooltip)
        {
            treemap.TreeMapModel.ShowTooltip = showTooltip;
            return this;
        }

        public TreeMapPropertiesBuilder CanResize(bool resize)
        {
            treemap.TreeMapModel.CanResize = resize;
            return this;
        }


        public TreeMapPropertiesBuilder TooltipTemplate(string tooltipTemplate)
        {
            treemap.TreeMapModel.TooltipTemplate = tooltipTemplate;
            return this;
        }

        public TreeMapPropertiesBuilder HighlightBorderThickness(double highlightBorderThickness)
        {
            treemap.TreeMapModel.HighlightBorderThickness = highlightBorderThickness;
            return this;
        }

        public TreeMapPropertiesBuilder HighlightBorderBrush(string highlightBorderBrush)
        {
            treemap.TreeMapModel.HighlightBorderBrush = highlightBorderBrush;
            return this;
        }

        public TreeMapPropertiesBuilder TreeMapItemSelected(string treeMapItemSelected)
        {
            treemap.TreeMapModel.TreeMapItemSelected = treeMapItemSelected;
            return this;
        }

      

        public HtmlString Render()
        {
            return new HtmlString(treemap.Render().ToString());
        }

        public override String ToString()
        {
            return Render().ToString();
        }


    }
}
