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
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class FeatureMeasureBuilder
    {
        private FeatureMeasure feature = new FeatureMeasure();
        private QuantitativeScale qs_model;
        private List<FeatureMeasure> f_measures;

        public FeatureMeasureBuilder(FeatureMeasure options)
        {
            this.feature = options;
        }

        public FeatureMeasureBuilder(FeatureMeasure options, QuantitativeScale scale)
        {

            this.feature = options;
            this.qs_model = scale;
            f_measures = new List<FeatureMeasure>();
            this.qs_model.FeatureMeasure = new List<FeatureMeasure>();
        }

        public void Add()
        {
            this.qs_model.FeatureMeasure.Add(feature);
            this.f_measures.Add(feature);
            feature = new FeatureMeasure();
        }

        public FeatureMeasureBuilder FeatureMeasureValue(double value)
        {
            this.feature.Value = value;
            return this;
        }

        public FeatureMeasureBuilder ComparativeMeasureValue(double value)
        {
            this.feature.ComparativeMeasure = value;
            return this;
        }

        public FeatureMeasureBuilder CategoryValue(String value)
        {
            this.feature.Category = value;
            return this;
        }

    }
}
