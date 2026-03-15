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
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class BulletLabelsBuilder
    {
        private BulletLabels label = new BulletLabels();
        private QuantitativeScale qs_model;

        public BulletLabelsBuilder(BulletLabels options)
        {
            this.label = options;
        }

        public BulletLabelsBuilder(BulletLabels options, QuantitativeScale scale)
        {

            this.label = options;
            this.qs_model = scale;
            this.qs_model.Labels = options;
        }

        public BulletLabelsBuilder LabelStroke(Color stroke)
        {
            this.label.LabelStroke = Convert.ToString(stroke.Name);
            return this;
        }
        public BulletLabelsBuilder LabelSize(int size)
        {
            this.label.LabelSize = size;
            return this;
        }

        public BulletLabelsBuilder LabelOffset(int offset)
        {
            this.label.LabelOffset = offset;
            return this;
        }

        public BulletLabelsBuilder LabelPosition(LabelPosition position)
        {
            this.label.Position = position;
            return this;
        }

        public BulletLabelsBuilder Font(Action<BulletFontBuilder> fontOptions)
        {
            var obj = new BulletFont();
            var builder = new BulletFontBuilder(this.label,obj);
            if (fontOptions != null)
                fontOptions.Invoke(builder);
            return this;
        }

    }
}
