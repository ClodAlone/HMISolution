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
    public class MapLabelSettingBuilder
    {
        public MapLabelSetting labelSetting;

        public ShapeFileLayer shapeFileLayer;

        public MapLabelSettingBuilder(MapLabelSetting labelSetting, ShapeFileLayer layer)
        {
            this.labelSetting = labelSetting;
            this.shapeFileLayer = layer;
            this.shapeFileLayer.LabelSetting = this.labelSetting;
        }

        public MapLabelSettingBuilder ShowLabels(bool showLabels)
        {
            this.labelSetting.ShowLabels = showLabels;
            return this;
        }

        public MapLabelSettingBuilder EnableSmartLabel(bool enableSmartLabel)
        {
            this.labelSetting.EnableSmartLabel = enableSmartLabel;
            return this;
        }

        public MapLabelSettingBuilder SmartLabelSize(LabelSize smartLabelSize)
        {
            this.labelSetting.SmartLabelSize = smartLabelSize;
            return this;
        }

        public MapLabelSettingBuilder LabelLength(double labelLength)
        {
            this.labelSetting.LabelLength = labelLength;
            return this;
        }

        public MapLabelSettingBuilder LabelPath(string labelPath)
        {
            this.labelSetting.LabelPath = labelPath;
            return this;
        }
    }
}
