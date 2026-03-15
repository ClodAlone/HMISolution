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

namespace Syncfusion.RDL.DOM
{
    public class ChartGridLines
    {
        public BooleanOptions Enabled { get; set; }
        public Style Style { get; set; }
        public float Interval { get; set; }
        public IntervalType IntervalType { get; set; }
        public float IntervalOffset { get; set; }
        public IntervalType IntervalOffsetType { get; set; }

        public bool ShouldSerializeIntervalOffsetType()
        {
            return IntervalOffsetType != DOM.IntervalType.Auto;
        }

        public void ResetIntervalOffsetType()
        {
            this.IntervalOffsetType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeIntervalType()
        {
            return IntervalType != DOM.IntervalType.Auto;
        }

        public void ResetIntervalType()
        {
            this.IntervalType = DOM.IntervalType.Auto;
        }

        public bool ShouldSerializeIntervalOffset()
        {
            return IntervalOffset != 0;
        }

        public void ResetIntervalOffset()
        {
            this.IntervalOffset = 0;
        }

        public bool ShouldSerializeInterval()
        {
            return Interval != 0;
        }

        public void ResetInterval()
        {
            this.Interval = 0;
        }
    }

    public class ChartMajorGridLines : ChartGridLines
    {

    }

    public class ChartMinorGridLines : ChartGridLines
    {

    }
}
