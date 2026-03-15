//-------------------------------------------------------------------------------------------------
// <copyright file="ScaleLabels.cs" company="syncfusion">
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
    public class ScaleLabels
    {
        #region Public Properties

        public Style Style
        {
            get;
            set;
        }

        public float Interval
        {
            get;
            set;
        }

        public float IntervalOffset
        {
            get;
            set;
        }

        public bool AllowUpsideDown
        {
            get;
            set;
        }

        public float DistanceFromScale
        {
            get;
            set;
        }

        public float FontAngle
        {
            get;
            set;
        }

        public Placement Placement
        {
            get;
            set;
        }

        public bool RotateLabels
        {
            get;
            set;
        }

        public bool ShowEndLabels
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        public bool UseFontPercent
        {
            get;
            set;
        }

        #endregion

        #region Constructors
        public ScaleLabels()
        {
        }
        #endregion
    }
}
