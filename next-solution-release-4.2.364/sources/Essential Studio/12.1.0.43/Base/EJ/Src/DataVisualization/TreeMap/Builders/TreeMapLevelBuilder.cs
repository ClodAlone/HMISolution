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

namespace Syncfusion.JavaScript.DataVisualization.Builders
{
    public class TreeMapLevelBuilder
    {
        public TreeMapLevel treeMapLevel;

        public TreeMapProperties mapProperties;

        public TreeMapLevelBuilder(TreeMapLevel treeMapLevel, TreeMapProperties treeMapProperties)
        {
            this.treeMapLevel = treeMapLevel;
            this.mapProperties = treeMapProperties;
        }

        public void Add()
        {
            if (this.mapProperties.Levels == null)
            {
                this.mapProperties.Levels = new List<TreeMapLevel>();                
            }
            this.mapProperties.Levels.Add(treeMapLevel);
            this.treeMapLevel = new TreeMapLevel();
        }

        public TreeMapLevelBuilder ItemsLayoutMode(string itemsLayoutMode)
        {
            treeMapLevel.ItemsLayoutMode = itemsLayoutMode;
            return this;
        }

        public TreeMapLevelBuilder GroupPath(string groupPath)
        {
            treeMapLevel.GroupPath = groupPath;
            return this;
        }

        public TreeMapLevelBuilder ShowItem(bool showItem)
        {
            treeMapLevel.ShowItem = showItem;
            return this;
        }

        public TreeMapLevelBuilder GroupGap(double groupGap)
        {
            treeMapLevel.GroupGap = groupGap;
            return this;
        }

        public TreeMapLevelBuilder HeaderHeight(double headerHeight)
        {
            treeMapLevel.HeaderHeight = headerHeight;
            return this;
        }

        public TreeMapLevelBuilder ShowLabels(bool showLabels)
        {
            treeMapLevel.ShowLabels = showLabels;
            return this;
        }

        public TreeMapLevelBuilder HeaderTemplate(string headerTemplate)
        {
            treeMapLevel.HeaderTemplate = headerTemplate;
            return this;
        }

        public TreeMapLevelBuilder LabelTemplate(string labelTemplate)
        {
            treeMapLevel.LabelTemplate = labelTemplate;
            return this;
        }

    }
}
