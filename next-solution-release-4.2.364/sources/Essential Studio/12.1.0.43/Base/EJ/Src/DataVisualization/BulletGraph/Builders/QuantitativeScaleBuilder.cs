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
using System.Drawing;
using System.ComponentModel;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class QuantitativeScaleBuilder
    {
        private BulletGraphProperties bg_model;
        private QuantitativeScale scale; 

        public QuantitativeScaleBuilder(BulletGraph bullet, QuantitativeScale scale1)
        {
            this.scale = scale1;
            this.bg_model = bullet.BulletGraphModel;
            this.bg_model.QuantitativeScale = scale1;
            
        }

        public QuantitativeScaleBuilder(QuantitativeScale scale)
        {
            this.scale = scale;
        }

        //public QuantitativeScaleBuilder Location(Point val)
        //{
        //    this.scale.Location = val;
        //    return this;
        //}
        public QuantitativeScaleBuilder Minimum(int min)
        {
            this.scale.Minimum = min;
            return this;
        }
        public QuantitativeScaleBuilder Maximum(int max)
        {
            this.scale.Maximum = max;
            return this;
        }
        public QuantitativeScaleBuilder Interval(int interval)
        {
            this.scale.Interval = interval;
            return this;
        }

        public QuantitativeScaleBuilder MinorTicksPerInterval(int tickPerInterval)
        {
            this.scale.MinorTicksPerInterval = tickPerInterval;
            return this;
        }

        public QuantitativeScaleBuilder MajorTickSize(int majorTick)
        {
            this.scale.MajorTickSize = majorTick;
            return this;
        }

        public QuantitativeScaleBuilder MinorTickSize(int minorTick)
        {
            this.scale.MinorTickSize = minorTick;
            return this;
        }

        public QuantitativeScaleBuilder MajorTickStroke(Color majorTick)
        {
            this.scale.MajorTickStroke = Convert.ToString(majorTick.Name);
            return this;
        }

        public QuantitativeScaleBuilder MinorTickStroke(Color minorTick)
        {
            this.scale.MinorTickStroke = Convert.ToString(minorTick.Name);
            return this;
        }

        public QuantitativeScaleBuilder MajorTickStrokeThickness(double majorTick)
        {
            this.scale.MajorTickStrokeThickness = majorTick;
            return this;
        }

        public QuantitativeScaleBuilder MinorTickStrokeThickness(double minorTick)
        {
            this.scale.MinorTickStrokeThickness = minorTick;
            return this;
        }

        public QuantitativeScaleBuilder TickPosition(TickPosition tickPos)
        {
            this.scale.TickPosition = tickPos;
            return this;
        }

        public QuantitativeScaleBuilder FeaturedMeasureBarStrokeThickness(int measure)
        {
            this.scale.FeaturedMeasureBarStrokeThickness = measure;
            return this;
        }

        public QuantitativeScaleBuilder ComparativeMeasureSymbolStrokeThickness(int measure)
        {
            this.scale.ComparativeMeasureSymbolStrokeThickness = measure;
            return this;
        }

        public QuantitativeScaleBuilder FeaturedMeasureBarStroke(Color color)
        {
            this.scale.FeaturedMeasureBarStroke = Convert.ToString(color.Name);
            return this;
        }

        public QuantitativeScaleBuilder ComparativeMeasureSymbolStroke(Color color)
        {
            this.scale.ComparativeMeasureSymbolStroke = Convert.ToString(color.Name);
            return this;
        }

        public QuantitativeScaleBuilder Labels(Action<BulletLabelsBuilder> labelOptions)
        {
            var obj = new BulletLabels();
          //  scale.Labels = labelOptions;
            var builder = new BulletLabelsBuilder(obj,this.scale);
            if (labelOptions != null)
                labelOptions.Invoke(builder);
            return this;
        }

        public QuantitativeScaleBuilder Location(Action<BulletLocationBuilder> locationOptions)
        {
            var obj = new BulletLocation();
          //  scale.Location = locationOptions;
            var builder = new BulletLocationBuilder(obj, this.scale);
            if (locationOptions != null)
                locationOptions.Invoke(builder);
            return this;
        }

        public QuantitativeScaleBuilder FeatureMeasure(Action<FeatureMeasureBuilder> measureOptions)
        {
            var obj = new FeatureMeasure();
            var builder = new FeatureMeasureBuilder(obj, this.scale);
            if (measureOptions != null)
                measureOptions.Invoke(builder);
            return this;
        }

        public QuantitativeScaleBuilder FeatureMeasure(List<FeatureMeasure> defaultItems)
        {
            this.scale.FeatureMeasure = defaultItems;
            return this;
        }




    }
}
