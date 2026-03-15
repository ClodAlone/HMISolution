//-------------------------------------------------------------------------------------------------
// <copyright file="GaugeTickMarks.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class GaugeTickMarks : TickMarkStyle
    {
        #region Members

        private float interval;

        private float intervalOffset;

        #endregion

        #region Public Properties

        public float Interval
        {
            get { return interval; }

            set { interval = value; }
        }

        public float IntervalOffset
        {
            get { return intervalOffset; }

            set { intervalOffset = value; }
        }

        #endregion

        #region Constructors
        public GaugeTickMarks()
        {
        }
        #endregion
    }
}
