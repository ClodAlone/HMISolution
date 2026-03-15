#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.JavaScript.Shared.Serializer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class TreeMapLevel
    {
        #region Fields

       private string itemsLayoutMode = "Squarified";
       private string groupPath = null;
       private bool  showItem =  false;
       private double groupGap = 0;
       private double  headerHeight = 0;
       private bool  showLabels = false;
       private string  headerTemplate = null;
       private string  labelTemplate = null;
       private List<TreeMapItem> treeMapItems = new List<TreeMapItem>();

       #endregion

       #region Properties

       [JsonProperty("showItem")]
       [DefaultValue(false)]
       public bool ShowItem
       {
           get { return this.showItem; }
           set { this.showItem = value; }
       }

       [JsonProperty("showLabels")]
       [DefaultValue(false)]
       public bool ShowLabels
       {
           get { return this.showLabels; }
           set { this.showLabels = value; }
       }

       [JsonProperty("headerHeight")]
       [DefaultValue(0)]
       public double HeaderHeight
       {
           get { return this.headerHeight; }
           set { this.headerHeight = value; }
       }

       [JsonProperty("groupGap")]
       [DefaultValue(0)]
       public double GroupGap
       {
           get { return this.groupGap; }
           set { this.groupGap = value; }
       }

       [JsonProperty("itemsLayoutMode")]
       [DefaultValue("Squarified")]
       public string ItemsLayoutMode
       {
           get { return this.itemsLayoutMode; }
           set { this.itemsLayoutMode = value; }
       }

       [JsonProperty("groupPath")]
       [DefaultValue(null)]
       public string GroupPath
       {
           get { return this.groupPath; }
           set { this.groupPath = value; }
       }

       [JsonProperty("headerTemplate")]
       [DefaultValue(null)]
       public string HeaderTemplate
       {
           get { return this.headerTemplate; }
           set { this.headerTemplate = value; }
       }

       [JsonProperty("labelTemplate")]
       [DefaultValue(null)]
       public string LabelTemplate
       {
           get { return this.labelTemplate; }
           set { this.labelTemplate = value; }
       }

       #endregion
    }
}
