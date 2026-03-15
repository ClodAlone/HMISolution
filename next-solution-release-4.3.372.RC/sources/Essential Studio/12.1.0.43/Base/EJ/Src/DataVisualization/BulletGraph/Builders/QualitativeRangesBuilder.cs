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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Drawing;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class QualitativeRangesBuilder
    {
        private QualitativeRanges range = new QualitativeRanges();
        private BulletGraphProperties q_model;
        private List<QualitativeRanges> q_ranges; 
        public QualitativeRangesBuilder(QualitativeRanges options)
        {
            this.range = options;
        }
        public QualitativeRangesBuilder(QualitativeRanges options, BulletGraph bullet)
         {
             this.range = options;
             this.q_model = bullet.BulletGraphModel;

             q_ranges = new List<QualitativeRanges>();
             this.q_model.QualitativeRanges = new List<QualitativeRanges>();
         }
        public void Add()
        {
            this.q_model.QualitativeRanges.Add(range);
            this.q_ranges.Add(range);
            range = new QualitativeRanges();
        }
        public QualitativeRangesBuilder RangeEnd(double value)
        {
            this.range.RangeEnd = value;
            return this;
        }

        public QualitativeRangesBuilder RangeStroke(Color value)
        {
            this.range.RangeStroke = Convert.ToString(value.Name);
            return this;
        }

        public QualitativeRangesBuilder RangeOpacity(double value)
        {
            this.range.RangeOpacity = value;
            return this;
        }

    }
}
