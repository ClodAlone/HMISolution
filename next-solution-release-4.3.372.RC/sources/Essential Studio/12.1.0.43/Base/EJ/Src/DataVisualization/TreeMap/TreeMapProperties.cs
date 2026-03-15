#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class TreeMapProperties
    {
        #region Fields

           private object dataSource = null;
           private string colorValuePath =  null;
           private string weightValuePath =  null;
           private List<TreeMapItem> treeMapItems = new List<TreeMapItem>();
           private bool showLegend =  false;
           private string  borderBrush =  "white";
           private double borderThickness =  1;
           private bool canResize = true;
           private string  itemsLayoutMode =  "Squarified";
           private List<TreeMapLevel> levels =  new List<TreeMapLevel>();           
           private TreeMapLegend treeMapLegend =  null;         
           private bool highlightOnSelection =  false;
           private bool showTooltip =  false;
           private string tooltipTemplate =  null;
           private double highlightBorderThickness = 5;
           private string  highlightBorderBrush =  "gray";
           private List<TreeMapRangeColorMapping> rangeColorMapping = null;
           private TreeMapDesaturationColorMapping desaturationColorMapping = null;
           private TreeMapPaletteColorMapping paletteColorMapping = null; 
           private TreeMapUniColorMapping uniColorMapping = null;
           private string treeMapItemSelected = null;

        #endregion


        #region Properties


           [JsonProperty("canResize")]
           [DefaultValue(true)]
           public bool CanResize
           {
               get { return this.canResize; }
               set { this.canResize = value; }
           }

           [JsonProperty("itemsLayoutMode")]
           [DefaultValue("Squarified")]
           public string ItemsLayoutMode
           {
               get { return this.itemsLayoutMode; }
               set { this.itemsLayoutMode = value; }
           }

           [JsonProperty("highlightOnSelection")]
           [DefaultValue(false)]
           public bool HighlightOnSelection
           {
               get { return this.highlightOnSelection; }
               set { this.highlightOnSelection = value; }
           }

           [JsonProperty("showTooltip")]
           [DefaultValue(false)]
           public bool ShowTooltip
           {
               get { return this.showTooltip; }
               set { this.showTooltip = value; }
           }

           [JsonProperty("showLegend")]
           [DefaultValue(false)]
           public bool ShowLegend
           {
               get { return this.showLegend; }
               set { this.showLegend = value; }
           }


           [JsonProperty("borderThickness")]
           [DefaultValue(1)]
           public double BorderThickness
           {
               get { return this.borderThickness; }
               set { this.borderThickness = value; }
           }

           [JsonProperty("highlightBorderThickness")]
           [DefaultValue(5)]
           public double HighlightBorderThickness
           {
               get { return this.highlightBorderThickness; }
               set { this.highlightBorderThickness = value; }
           }

           [JsonProperty("colorValuePath")]
           [DefaultValue(null)]
           public string ColorValuePath
           {
               get { return this.colorValuePath; }
               set { this.colorValuePath = value; }
           }

           [JsonProperty("weightValuePath")]
           [DefaultValue(null)]
           public string WeightValuePath
           {
               get { return this.weightValuePath; }
               set { this.weightValuePath = value; }
           }

           [JsonProperty("borderBrush")]
           [DefaultValue("white")]
           public string BorderBrush
           {
               get { return this.borderBrush; }
               set { this.borderBrush = value; }
           }

           [JsonProperty("tooltipTemplate")]
           [DefaultValue(null)]
           public string TooltipTemplate
           {
               get { return this.tooltipTemplate; }
               set { this.tooltipTemplate = value; }
           }

           [JsonProperty("highlightBorderBrush")]
           [DefaultValue("gray")]
           public string HighlightBorderBrush
           {
               get { return this.highlightBorderBrush; }
               set { this.highlightBorderBrush = value; }
           }

           [JsonProperty("treeMapItems")]
           public List<TreeMapItem> TreeMapItems
           {
               get { return this.treeMapItems; }
               set { this.treeMapItems = value; }
           }

           [JsonProperty("levels")]
           public List<TreeMapLevel> Levels
           {
               get { return this.levels; }
               set { this.levels = value; }
           }

           [JsonProperty("dataSource")]
           [JsonConverter(typeof(DataManagerConverter))]
           public object DataSource
           {
               get { return this.dataSource; }
               set { this.dataSource = value; }
           }

           [JsonProperty("treeMapLegend")]
           [DefaultValue(null)]
           public TreeMapLegend TreeMapLegend
           {
               get { return this.treeMapLegend; }
               set { this.treeMapLegend = value; }
           }

           [JsonProperty("desaturationColorMapping")]
           [DefaultValue(null)]
           public TreeMapDesaturationColorMapping TreeMapDesaturationColorMapping
           {
               get { return this.desaturationColorMapping; }
               set { this.desaturationColorMapping = value; }
           }

           [JsonProperty("uniColorMapping")]
           [DefaultValue(null)]
           public TreeMapUniColorMapping TreeMapUniColorMapping
           {
               get { return this.uniColorMapping; }
               set { this.uniColorMapping = value; }
           }

           [JsonProperty("paletteColorMapping")]
           public TreeMapPaletteColorMapping TreeMapPaletteColorMapping
           {
               get { return this.paletteColorMapping; }
               set { this.paletteColorMapping = value; }
           }

           [JsonProperty("rangeColorMapping")]           
           public List<TreeMapRangeColorMapping> TreeMapRangeColorMappings
           {
               get { return this.rangeColorMapping; }
               set { this.rangeColorMapping = value; }
           }

           [JsonProperty("treeMapItemSelected")]
           [DefaultValue(null)]
           public string TreeMapItemSelected
           {
               get { return this.treeMapItemSelected; }
               set { this.treeMapItemSelected = value; }
           }
        
        #endregion

           public bool ShouldSerializeDataSource()
           {
               if (typeof(DataSource).IsAssignableFrom(this.DataSource.GetType()))
               {
                   if (Utils.PropertyCompare(DataSource, new DataSource()))
                       return true;
                   else
                       return false;
               }
               else if (this.DataSource is IEnumerable)
               {
                   ICollection data = DataSource as ICollection;
                   if (data.Count != 0)
                       return true;
                   else
                       return false;
               }
               else
                   return false;
           }

    }

    

}
